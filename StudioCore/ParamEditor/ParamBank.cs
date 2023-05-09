using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SoulsFormats;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FSParam;
using StudioCore.Editor;

namespace StudioCore.ParamEditor
{
    /// <summary>
    /// Utilities for dealing with global params for a game
    /// </summary>
    public class ParamBank
    {
        public static ParamBank PrimaryBank = new ParamBank();
        public static ParamBank VanillaBank = new ParamBank();
        public static Dictionary<string, ParamBank> AuxBanks = new Dictionary<string, ParamBank>();


        public static string ClipboardParam = null;
        public static List<Param.Row> ClipboardRows = new List<Param.Row>();

        private static Dictionary<string, PARAMDEF> _paramdefs = null;
        private static Dictionary<string, Dictionary<ulong, PARAMDEF>> _patchParamdefs = null;


        private Param EnemyParam = null;
        internal AssetLocator AssetLocator = null;

        private Dictionary<string, Param> _params = null;
        private Dictionary<string, HashSet<int>> _vanillaDiffCache = null; //If param != vanillaparam
        private Dictionary<string, HashSet<int>> _primaryDiffCache = null; //If param != primaryparam

        private bool _pendingUpgrade = false;

        public static bool IsDefsLoaded { get; private set; } = false;
        public static bool IsMetaLoaded { get; private set; } = false;
        public bool IsLoadingParams { get; private set; } = false;

        public IReadOnlyDictionary<string, Param> Params
        {
            get
            {
                if (IsLoadingParams)
                {
                    return null;
                }
                return _params;
            }
        }

        private ulong _paramVersion;
        public ulong ParamVersion
        {
            get => _paramVersion;
        }

        public IReadOnlyDictionary<string, HashSet<int>> VanillaDiffCache
        {
            get
            {
                if (IsLoadingParams)
                {
                    return null;
                }
                {
                if (VanillaBank == this)
                    return null;
                }
                return _vanillaDiffCache;
            }
        }
        public IReadOnlyDictionary<string, HashSet<int>> PrimaryDiffCache
        {
            get
            {
                if (IsLoadingParams)
                {
                    return null;
                }
                {
                if (PrimaryBank == this)
                    return null;
                }
                return _primaryDiffCache;
            }
        }

        private static List<(string, PARAMDEF)> LoadParamdefs(AssetLocator assetLocator)
        {
            _paramdefs = new Dictionary<string, PARAMDEF>();
            _patchParamdefs = new Dictionary<string, Dictionary<ulong, PARAMDEF>>();
            var dir = assetLocator.GetParamdefDir();
            var files = Directory.GetFiles(dir, "*.xml");
            List<(string, PARAMDEF)> defPairs = new List<(string, PARAMDEF)>();
            foreach (var f in files)
            {
                var pdef = PARAMDEF.XmlDeserialize(f);
                _paramdefs.Add(pdef.ParamType, pdef);
                defPairs.Add((f, pdef));
            }

            // Load patch paramdefs
            var patches = assetLocator.GetParamdefPatches();
            foreach (var patch in patches)
            {
                var pdir = assetLocator.GetParamdefPatchDir(patch);
                var pfiles = Directory.GetFiles(pdir, "*.xml");
                foreach (var f in pfiles)
                {
                    var pdef = PARAMDEF.XmlDeserialize(f);
                    defPairs.Add((f, pdef));
                    if (!_patchParamdefs.ContainsKey(pdef.ParamType))
                    {
                        _patchParamdefs[pdef.ParamType] = new Dictionary<ulong, PARAMDEF>();
                    }
                    _patchParamdefs[pdef.ParamType].Add(patch, pdef);
                }
            }

            return defPairs;
        }

        public static void LoadParamMeta(List<(string, PARAMDEF)> defPairs, AssetLocator assetLocator)
        {
            var mdir = assetLocator.GetParammetaDir();
            foreach ((string f, PARAMDEF pdef) in defPairs)
            {
                var fName = f.Substring(f.LastIndexOf('\\') + 1);
                ParamMetaData.XmlDeserialize($@"{mdir}\{fName}", pdef);
            }
        }

        public CompoundAction LoadParamDefaultNames(string param = null, bool onlyAffectEmptyNames = false)
        {
            string dir = AssetLocator.GetParamNamesDir();
            string[] files = param == null ? Directory.GetFiles(dir, "*.txt") : new[]
            {
                Path.Combine(dir, $"{param}.txt"),
            };
            var actions = new List<EditorAction>();
            foreach (string f in files)
            {
                string fName = Path.GetFileNameWithoutExtension(f);
                if (!_params.ContainsKey(fName))
                    continue;
                string names = File.ReadAllText(f);
                (MassEditResult r, CompoundAction a) = MassParamEditCSV.PerformSingleMassEdit(this, names, fName, "Name", ' ', true, onlyAffectEmptyNames);
                if (r.Type != MassEditResultType.SUCCESS)
                {
                    TaskManager.warningList.TryAdd($"ParamNameImportFail {fName}", $"Could not apply name files for {fName}");
                    continue;
                }
                actions.Add(a);
            }
            return new CompoundAction(actions);
        }

        public ActionManager TrimNewlineChrsFromNames()
        {
            (MassEditResult r, ActionManager child) =
                MassParamEditRegex.PerformMassEdit(this, "param .*: id .*: name: replace \r:0", null);
            return child;
        }

        private void LoadParamFromBinder(IBinder parambnd, ref Dictionary<string, FSParam.Param> paramBank, out ulong version, bool checkVersion = false)
        {
            bool success = ulong.TryParse(parambnd.Version, out version);
            if (checkVersion && !success)
            {
                throw new Exception($@"Failed to get regulation version. Params might be corrupt.");
            }

            // Load every param in the regulation
            // _params = new Dictionary<string, PARAM>();
            foreach (var f in parambnd.Files)
            {
                if (!f.Name.ToUpper().EndsWith(".PARAM"))
                {
                    continue;
                }
                if (paramBank.ContainsKey(Path.GetFileNameWithoutExtension(f.Name)))
                {
                    continue;
                }
                if (f.Name.EndsWith("LoadBalancerParam.param") && AssetLocator.Type != GameType.EldenRing)
                {
                    continue;
                }
                FSParam.Param p = FSParam.Param.Read(f.Bytes);
                if (!_paramdefs.ContainsKey(p.ParamType) && !_patchParamdefs.ContainsKey(p.ParamType))
                {
                    continue;
                }

                // Try to fixup Elden Ring ChrModelParam for ER 1.06 because many have been saving botched params and
                // it's an easy fixup
                if (AssetLocator.Type == GameType.EldenRing &&
                    p.ParamType == "CHR_MODEL_PARAM_ST" &&
                    _paramVersion == 10601000)
                {
                    p.FixupERChrModelParam();
                }

                // Lookup the correct paramdef based on the version
                PARAMDEF def = null;
                if (_patchParamdefs.ContainsKey(p.ParamType))
                {
                    var keys = _patchParamdefs[p.ParamType].Keys.OrderByDescending(e => e);
                    foreach (var k in keys)
                    {
                        if (version >= k)
                        {
                            def = _patchParamdefs[p.ParamType][k];
                            break;
                        }
                    }
                }

                // If no patched paramdef was found for this regulation version, fallback to vanilla defs
                if (def == null)
                    def = _paramdefs[p.ParamType];

                try
                {
                    p.ApplyParamdef(def);
                    paramBank.Add(Path.GetFileNameWithoutExtension(f.Name), p);
                }
                catch(Exception e)
                {
                    var name = f.Name.Split("\\").Last();
                    TaskManager.warningList.TryAdd($"{name} DefFail",$"Could not apply ParamDef for {name}");
                }
            }
        }

        /// <summary>
        /// Checks for DeS paramBNDs and returns the name of the parambnd with the highest priority.
        /// </summary>
        private string GetDesGameparamName(string rootDirectory)
        {
            string name = "";
            name = "gameparamna.parambnd.dcx";
            if (File.Exists($@"{rootDirectory}\param\gameparam\{name}"))
            {
                return name;
            }
            name = "gameparamna.parambnd";
            if (File.Exists($@"{rootDirectory}\param\gameparam\{name}"))
            {
                return name;
            }
            name = "gameparam.parambnd.dcx";
            if (File.Exists($@"{rootDirectory}\param\gameparam\{name}"))
            {
                return name;
            }
            name = "gameparam.parambnd";
            if (File.Exists($@"{rootDirectory}\param\gameparam\{name}"))
            {
                return name;
            }
            return "";
        }

        private void LoadParamsDES()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;

            string paramBinderName = GetDesGameparamName(mod);
            if (paramBinderName == "")
            {
                paramBinderName = GetDesGameparamName(dir);
            }

            // Load params
            var param = $@"{mod}\param\gameparam\{paramBinderName}";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\gameparam\{paramBinderName}";
            }

            if (!File.Exists(param))
            {
                throw new FileNotFoundException("Could not find DES parambnds. Functionality will be limited.");
            }
            LoadParamsDESFromFile(param);

            //DrawParam
            Dictionary<string, string> drawparams = new();
            if (Directory.Exists($@"{dir}\param\drawparam"))
            {
                foreach (var p in Directory.GetFiles($@"{dir}\param\drawparam", "*.parambnd.dcx"))
                {
                    drawparams[Path.GetFileNameWithoutExtension(p)] = p;
                }
            }
            if (Directory.Exists($@"{mod}\param\drawparam"))
            {
                foreach (var p in Directory.GetFiles($@"{mod}\param\drawparam", "*.parambnd.dcx"))
                {
                    drawparams[Path.GetFileNameWithoutExtension(p)] = p;
                }
            }
            foreach (var drawparam in drawparams)
            {
                LoadParamsDESFromFile(drawparam.Value);
            }
        }
        private void LoadVParamsDES()
        {
            string paramBinderName = GetDesGameparamName(AssetLocator.GameRootDirectory);

            LoadParamsDESFromFile($@"{AssetLocator.GameRootDirectory}\param\gameparam\{paramBinderName}");
            if (Directory.Exists($@"{AssetLocator.GameRootDirectory}\param\drawparam"))
            {
                foreach (var p in Directory.GetFiles($@"{AssetLocator.GameRootDirectory}\param\drawparam", "*.parambnd.dcx"))
                {
                    LoadParamsDS1FromFile(p);
                }
            }
        }
        private void LoadParamsDESFromFile(string path)
        {
            LoadParamFromBinder(BND3.Read(path), ref _params, out _paramVersion);
        }

        private void LoadParamsDS1()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\GameParam\GameParam.parambnd"))
            {
                //MessageBox.Show("Could not find DS1 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
                throw new FileNotFoundException("Could not find DS1 parambnd. Functionality will be limited.");
            }
            // Load params
            var param = $@"{mod}\param\GameParam\GameParam.parambnd";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\GameParam\GameParam.parambnd";
            }
            LoadParamsDS1FromFile(param);

            //DrawParam
            Dictionary<string, string> drawparams = new();
            if (Directory.Exists($@"{dir}\param\DrawParam"))
            {
                foreach (var p in Directory.GetFiles($@"{dir}\param\DrawParam", "*.parambnd"))
                {
                    drawparams[Path.GetFileNameWithoutExtension(p)] = p;
                }
            }
            if (Directory.Exists($@"{mod}\param\DrawParam"))
            {
                foreach (var p in Directory.GetFiles($@"{mod}\param\DrawParam", "*.parambnd"))
                {
                    drawparams[Path.GetFileNameWithoutExtension(p)] = p;
                }
            }
            foreach (var drawparam in drawparams)
            {
                LoadParamsDS1FromFile(drawparam.Value);
            }
        }
        private void LoadVParamsDS1()
        {
            LoadParamsDS1FromFile($@"{AssetLocator.GameRootDirectory}\param\GameParam\GameParam.parambnd");
            if (Directory.Exists($@"{AssetLocator.GameRootDirectory}\param\DrawParam"))
            {
                foreach (var p in Directory.GetFiles($@"{AssetLocator.GameRootDirectory}\param\DrawParam", "*.parambnd"))
                {
                    LoadParamsDS1FromFile(p);
                }
            }
        }
        private void LoadParamsDS1FromFile(string path)
        {
            LoadParamFromBinder(BND3.Read(path), ref _params, out _paramVersion);
        }

        private void LoadParamsDS1R()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\GameParam\GameParam.parambnd.dcx"))
            {
                //MessageBox.Show("Could not find DS1 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
                throw new FileNotFoundException("Could not find DS1 parambnd. Functionality will be limited.");
            }

            // Load params
            var param = $@"{mod}\param\GameParam\GameParam.parambnd.dcx";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\GameParam\GameParam.parambnd.dcx";
            }
            LoadParamsDS1RFromFile(param);

            //DrawParam
            Dictionary<string, string> drawparams = new();
            if (Directory.Exists($@"{dir}\param\DrawParam"))
            {
                foreach (var p in Directory.GetFiles($@"{dir}\param\DrawParam", "*.parambnd.dcx"))
                {
                    drawparams[Path.GetFileNameWithoutExtension(p)] = p;
                }
            }
            if (Directory.Exists($@"{mod}\param\DrawParam"))
            {
                foreach (var p in Directory.GetFiles($@"{mod}\param\DrawParam", "*.parambnd.dcx"))
                {
                    drawparams[Path.GetFileNameWithoutExtension(p)] = p;
                }
            }
            foreach (var drawparam in drawparams)
            {
                LoadParamsDS1RFromFile(drawparam.Value);
            }
        }
        private void LoadVParamsDS1R()
        {
            LoadParamsDS1RFromFile($@"{AssetLocator.GameRootDirectory}\param\GameParam\GameParam.parambnd.dcx");
            if (Directory.Exists($@"{AssetLocator.GameRootDirectory}\param\DrawParam"))
            {
                foreach (var p in Directory.GetFiles($@"{AssetLocator.GameRootDirectory}\param\DrawParam", "*.parambnd.dcx"))
                {
                    LoadParamsDS1FromFile(p);
                }
            }
        }
        private void LoadParamsDS1RFromFile(string path)
        {
            LoadParamFromBinder(BND3.Read(path), ref _params, out _paramVersion);
        }

        private void LoadParamsBBSekiro()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\gameparam\gameparam.parambnd.dcx"))
            {
                //MessageBox.Show("Could not find param file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
                throw new FileNotFoundException("Could not find param file. Functionality will be limited.");
            }

            // Load params
            var param = $@"{mod}\param\gameparam\gameparam.parambnd.dcx";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\gameparam\gameparam.parambnd.dcx";
            }
            LoadParamsBBSekiroFromFile(param);
        }
        private void LoadVParamsBBSekiro()
        {
            LoadParamsBBSekiroFromFile($@"{AssetLocator.GameRootDirectory}\param\gameparam\gameparam.parambnd.dcx");
        }
        private void LoadParamsBBSekiroFromFile(string path)
        {
            LoadParamFromBinder(BND4.Read(path), ref _params, out _paramVersion);
        }

        /// <summary>
        /// Map related params.
        /// </summary>
        public readonly static List<string> DS2MapParamlist = new List<string>()
        {
            "demopointlight",
            "demospotlight",
            "eventlocation",
            "eventparam",
            "GeneralLocationEventParam",
            "generatorparam",
            "generatorregistparam",
            "generatorlocation",
            "generatordbglocation",
            "hitgroupparam",
            "intrudepointparam",
            "mapobjectinstanceparam",
            "maptargetdirparam",
            "npctalkparam",
            "treasureboxparam",
        };

        private void LoadParamsDS2()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\enc_regulation.bnd.dcx"))
            {
                //MessageBox.Show("Could not find DS2 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
                throw new FileNotFoundException("Could not find DS2 regulation file. Functionality will be limited.");
            }
            if (!BND4.Is($@"{dir}\enc_regulation.bnd.dcx"))
            {
                MessageBox.Show("Attempting to decrypt DS2 regulation file, else functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
            }

            // Load loose params
            List<string> scandir = new List<string>();
            if (mod != null && Directory.Exists($@"{mod}\Param"))
            {
                scandir.Add($@"{mod}\Param");
            }
            scandir.Add($@"{dir}\Param");

            // Load reg params
            var param = $@"{mod}\enc_regulation.bnd.dcx";
            if (!File.Exists(param))
            {
                param = $@"{dir}\enc_regulation.bnd.dcx";
            }
            string enemyFile = $@"{mod}\Param\EnemyParam.param";
            if (!File.Exists(enemyFile))
            {
                enemyFile = $@"{dir}\Param\EnemyParam.param";
            }
            LoadParamsDS2FromFile(scandir, param, enemyFile);
        }
        private void LoadVParamsDS2()
        {
            if (!File.Exists($@"{AssetLocator.GameRootDirectory}\enc_regulation.bnd.dcx"))
            {
                throw new FileNotFoundException("Could not find Vanilla DS2 regulation file. Functionality will be limited.");
            }
            if (!BND4.Is($@"{AssetLocator.GameRootDirectory}\enc_regulation.bnd.dcx"))
            {
                MessageBox.Show("Attempting to decrypt DS2 regulation file, else functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Load loose params
            List<string> scandir = new List<string>();
            scandir.Add($@"{AssetLocator.GameRootDirectory}\Param");

            LoadParamsDS2FromFile(scandir, $@"{AssetLocator.GameRootDirectory}\enc_regulation.bnd.dcx", $@"{AssetLocator.GameRootDirectory}\Param\EnemyParam.param");
        }
        private void LoadParamsDS2FromFile(List<string> loosedir, string path, string enemypath)
        {
            foreach (var d in loosedir)
            {
                var paramfiles = Directory.GetFileSystemEntries(d, @"*.param");
                foreach (var p in paramfiles)
                {
                    var name = Path.GetFileNameWithoutExtension(p);
                    var lp = Param.Read(p);
                    var fname = lp.ParamType;

                    try
                    {
                        PARAMDEF def = AssetLocator.GetParamdefForParam(fname);
                        lp.ApplyParamdef(def);
                        if (!_params.ContainsKey(name))
                        {
                            _params.Add(name, lp);
                        }
                    }
                    catch (Exception e)
                    {
                        TaskManager.warningList.TryAdd($"{fname} DefFail", $"Could not apply ParamDef for {fname}");
                    }
                }
            }

            BND4 paramBnd;
            if (!BND4.Is(path))
            {
                paramBnd = SFUtil.DecryptDS2Regulation(path);
            }
            // No need to decrypt
            else
            {
                paramBnd = BND4.Read(path);
            }
            var bndfile = paramBnd.Files.Find(x => Path.GetFileName(x.Name) == "EnemyParam.param");
            if (bndfile != null)
            {
                EnemyParam = Param.Read(bndfile.Bytes);
            }

            // Otherwise the param is a loose param
            if (File.Exists(enemypath))
            {
                EnemyParam = Param.Read(enemypath);
            }
            if (EnemyParam != null)
            {
                PARAMDEF def = AssetLocator.GetParamdefForParam(EnemyParam.ParamType);
                try
                {
                    EnemyParam.ApplyParamdef(def);
                }
                catch (Exception e)
                {
                    TaskManager.warningList.TryAdd($"{EnemyParam.ParamType} DefFail", $"Could not apply ParamDef for {EnemyParam.ParamType}");
                }
            }
            LoadParamFromBinder(paramBnd, ref _params, out _paramVersion);
        }

        private void LoadParamsDS3(bool loose)
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\Data0.bdt"))
            {
                //MessageBox.Show("Could not find DS3 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
                throw new FileNotFoundException("Could not find DS3 regulation file. Functionality will be limited.");
            }

            var vparam = $@"{dir}\Data0.bdt";
            // Load loose params if they exist
            if (loose && File.Exists($@"{mod}\\param\gameparam\gameparam_dlc2.parambnd.dcx"))
            {
                LoadParamsDS3FromFile($@"{mod}\param\gameparam\gameparam_dlc2.parambnd.dcx", true);
            }
            else
            {
                var param = $@"{mod}\Data0.bdt";
                if (!File.Exists(param))
                {
                    param = vparam;
                }
                LoadParamsDS3FromFile(param, false);
            }
        }
        private void LoadVParamsDS3()
        {
            LoadParamsDS3FromFile($@"{AssetLocator.GameRootDirectory}\Data0.bdt", false);
        }
        private void LoadParamsDS3FromFile(string path, bool isLoose)
        {
            BND4 lparamBnd = isLoose ? BND4.Read(path) : SFUtil.DecryptDS3Regulation(path);
            LoadParamFromBinder(lparamBnd, ref _params, out _paramVersion);
        }

        private void LoadParamsER(bool partial)
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\regulation.bin"))
            {
                //MessageBox.Show("Could not find param file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
                throw new FileNotFoundException("Could not find param file. Functionality will be limited.");
            }

            // Load params
            var param = $@"{mod}\regulation.bin";
            if (!File.Exists(param) || partial)
            {
                param = $@"{dir}\regulation.bin";
            }
            LoadParamsERFromFile(param);

            param = $@"{mod}\regulation.bin";
            if (partial && File.Exists(param))
            {
                BND4 pParamBnd = SFUtil.DecryptERRegulation(param);
                Dictionary<string, Param> cParamBank = new Dictionary<string, Param>();
                ulong v;
                LoadParamFromBinder(pParamBnd, ref cParamBank, out v, true);
                foreach (var pair in cParamBank)
                {
                    Param baseParam = _params[pair.Key];
                    foreach (var row in pair.Value.Rows)
                    {
                        Param.Row bRow = baseParam[row.ID];
                        if (bRow == null)
                        {
                            baseParam.AddRow(row);
                        }
                        else
                        {
                            bRow.Name = row.Name;
                            foreach (var field in bRow.Cells)
                            {
                                var cell = bRow[field];
                                cell.Value = row[field].Value;
                            }
                        }
                    }
                }
            }
        }
        private void LoadVParamsER()
        {
            LoadParamsERFromFile($@"{AssetLocator.GameRootDirectory}\regulation.bin");
        }
        private void LoadParamsERFromFile(string path)
        {
            LoadParamFromBinder(SFUtil.DecryptERRegulation(path), ref _params, out _paramVersion, true);
        }

        //Some returns and repetition, but it keeps all threading and loading-flags visible inside this method
        public static void ReloadParams(ProjectSettings settings, NewProjectOptions options)
        {
            // Steal assetlocator from PrimaryBank.
            AssetLocator locator = PrimaryBank.AssetLocator;

            _paramdefs = new Dictionary<string, PARAMDEF>();
            IsDefsLoaded = false;

            AuxBanks = new Dictionary<string, ParamBank>();

            PrimaryBank._params = new Dictionary<string, Param>();
            PrimaryBank.IsLoadingParams = true;

            CacheBank.ClearCaches();

            TaskManager.Run("PB:LoadParams", true, false, false, () =>
            {
                if (PrimaryBank.AssetLocator.Type != GameType.Undefined)
                {
                    List<(string, PARAMDEF)> defPairs = LoadParamdefs(locator);
                    IsDefsLoaded = true;
                    TaskManager.Run("PB:LoadParamMeta", true, false, false, () =>
                    {
                        IsMetaLoaded = false;
                        LoadParamMeta(defPairs, locator);
                        IsMetaLoaded = true;
                    });
                }
                if (locator.Type == GameType.DemonsSouls)
                {
                    PrimaryBank.LoadParamsDES();
                }
                if (locator.Type == GameType.DarkSoulsPTDE)
                {
                    PrimaryBank.LoadParamsDS1();
                }
                if (locator.Type == GameType.DarkSoulsRemastered)
                {
                    PrimaryBank.LoadParamsDS1R();
                }
                if (locator.Type == GameType.DarkSoulsIISOTFS)
                {
                    PrimaryBank.LoadParamsDS2();
                }
                if (locator.Type == GameType.DarkSoulsIII)
                {
                    PrimaryBank.LoadParamsDS3(settings.UseLooseParams);
                }
                if (locator.Type == GameType.Bloodborne || locator.Type == GameType.Sekiro)
                {
                    PrimaryBank.LoadParamsBBSekiro();
                }
                if (locator.Type == GameType.EldenRing)
                {
                    PrimaryBank.LoadParamsER(settings.PartialParams);
                }

                PrimaryBank.ClearParamDiffCaches();
                PrimaryBank.IsLoadingParams = false;

                VanillaBank.IsLoadingParams = true;
                VanillaBank._params = new Dictionary<string, Param>();
                TaskManager.Run("PB:LoadVParams", true, false, false, () =>
                {
                    if (locator.Type == GameType.DemonsSouls)
                    {
                        VanillaBank.LoadVParamsDES();
                    }
                    if (locator.Type == GameType.DarkSoulsPTDE)
                    {
                        VanillaBank.LoadVParamsDS1();
                    }
                    if (locator.Type == GameType.DarkSoulsRemastered)
                    {
                        VanillaBank.LoadVParamsDS1R();
                    }
                    if (locator.Type == GameType.DarkSoulsIISOTFS)
                    {
                        VanillaBank.LoadVParamsDS2();
                    }
                    if (locator.Type == GameType.DarkSoulsIII)
                    {
                        VanillaBank.LoadVParamsDS3();
                    }
                    if (locator.Type == GameType.Bloodborne || locator.Type == GameType.Sekiro)
                    {
                        VanillaBank.LoadVParamsBBSekiro();
                    }
                    if (locator.Type == GameType.EldenRing)
                    {
                        VanillaBank.LoadVParamsER();
                    }
                    VanillaBank.IsLoadingParams = false;

                    TaskManager.Run("PB:RefreshDirtyCache", true, false, false, () => PrimaryBank.RefreshParamDiffCaches());
                });

                if (options != null)
                {
                    if (options.loadDefaultNames)
                    {
                        try
                        {
                            new Editor.ActionManager().ExecuteAction(PrimaryBank.LoadParamDefaultNames());
                            PrimaryBank.SaveParams(settings.UseLooseParams);
                        }
                        catch
                        {
                            TaskManager.warningList.TryAdd($"ParamNameImportFail", $"Could not locate or apply name files for this game.");
                        }
                    }
                }
            });
        }
        public static void LoadAuxBank(string path, string looseDir, string enemyPath)
        {
            // Steal assetlocator
            AssetLocator locator = PrimaryBank.AssetLocator;
            ParamBank newBank = new ParamBank();
            newBank.SetAssetLocator(locator);
            newBank._params = new Dictionary<string, Param>();
            newBank.IsLoadingParams = true;
            if (locator.Type == GameType.EldenRing)
            {
                newBank.LoadParamsERFromFile(path);
            }
            else if (locator.Type == GameType.Sekiro)
            {
                newBank.LoadParamsBBSekiroFromFile(path);
            }
            else if (locator.Type == GameType.DarkSoulsIII)
            {
                newBank.LoadParamsDS3FromFile(path, path.Trim().ToLower().EndsWith(".dcx"));
            }
            else if (locator.Type == GameType.Bloodborne)
            {
                newBank.LoadParamsBBSekiroFromFile(path);
            }
            else if (locator.Type == GameType.DarkSoulsIISOTFS)
            {
                newBank.LoadParamsDS2FromFile(new List<string>{looseDir}, path, enemyPath);
            }
            else if (locator.Type == GameType.DarkSoulsRemastered)
            {
                newBank.LoadParamsDS1RFromFile(path);
            }
            else if (locator.Type == GameType.DarkSoulsPTDE)
            {
                newBank.LoadParamsDS1FromFile(path);
            }
            else if (locator.Type == GameType.DemonsSouls)
            {
                newBank.LoadParamsDESFromFile(path);
            }
            newBank.ClearParamDiffCaches();
            newBank.IsLoadingParams = false;
            newBank.RefreshParamDiffCaches();
            AuxBanks[Path.GetFileName(Path.GetDirectoryName(path))] = newBank;
        }


        public void ClearParamDiffCaches()
        {
            _vanillaDiffCache = new Dictionary<string, HashSet<int>>();
            _primaryDiffCache = new Dictionary<string, HashSet<int>>();
            foreach (string param in _params.Keys)
            {
                _vanillaDiffCache.Add(param, new HashSet<int>());
                _primaryDiffCache.Add(param, new HashSet<int>());
            }
        }
        public void RefreshParamDiffCaches()
        {
            if (this != VanillaBank)
                _vanillaDiffCache = GetParamDiff(VanillaBank);
            if (this != PrimaryBank)
                _primaryDiffCache = GetParamDiff(PrimaryBank);
        }
        private Dictionary<string, HashSet<int>> GetParamDiff(ParamBank otherBank)
        {
            if (IsLoadingParams || otherBank == null || otherBank.IsLoadingParams)
                return null;
            Dictionary<string, HashSet<int>> newCache = new Dictionary<string, HashSet<int>>();
            foreach (string param in _params.Keys)
            {
                HashSet<int> cache = new HashSet<int>();
                newCache.Add(param, cache);
                Param p = _params[param];
                if (!otherBank._params.ContainsKey(param))
                {
                    Console.WriteLine("Missing vanilla param "+param);
                    continue;
                }

                var rows = _params[param].Rows.OrderBy(r => r.ID).ToArray();
                var vrows = otherBank._params[param].Rows.OrderBy(r => r.ID).ToArray();

                var vanillaIndex = 0;
                int lastID = -1;
                ReadOnlySpan<Param.Row> lastVanillaRows = default;
                for (int i = 0; i < rows.Length; i++)
                {
                    int ID = rows[i].ID;
                    if (ID == lastID)
                    {
                        RefreshParamRowDiffCache(rows[i], lastVanillaRows, cache);
                    }
                    else
                    {
                        lastID = ID;
                        while (vanillaIndex < vrows.Length && vrows[vanillaIndex].ID < ID)
                            vanillaIndex++;
                        if (vanillaIndex >= vrows.Length)
                        {
                            RefreshParamRowDiffCache(rows[i], Span<Param.Row>.Empty, cache);
                        }
                        else
                        {
                            int count = 0;
                            while (vanillaIndex + count < vrows.Length && vrows[vanillaIndex + count].ID == ID)
                                count++;
                            lastVanillaRows = new ReadOnlySpan<Param.Row>(vrows, vanillaIndex, count);
                            RefreshParamRowDiffCache(rows[i], lastVanillaRows, cache);
                            vanillaIndex += count;
                        }
                    }
                }
            }
            return newCache;
        }
        private static void RefreshParamRowDiffCache(Param.Row row, ReadOnlySpan<Param.Row> otherBankRows, HashSet<int> cache)
        {
            if (IsChanged(row, otherBankRows))
                cache.Add(row.ID);
            else
                cache.Remove(row.ID);
        }

        public void RefreshParamRowVanillaDiff(Param.Row row, string param)
        {
            if (param == null)
                return;
            if (!VanillaBank.Params.ContainsKey(param) || VanillaDiffCache == null || !VanillaDiffCache.ContainsKey(param))
                return; // Don't try for now
            var otherBankRows = VanillaBank.Params[param].Rows.Where(cell => cell.ID == row.ID).ToArray();
            if (IsChanged(row, otherBankRows))
                VanillaDiffCache[param].Add(row.ID);
            else
                VanillaDiffCache[param].Remove(row.ID);
        }

        private static bool IsChanged(Param.Row row, ReadOnlySpan<Param.Row> vanillaRows)
        {
            //List<Param.Row> vanils = vanilla.Rows.Where(cell => cell.ID == row.ID).ToList();
            if (vanillaRows.Length == 0)
            {
                return true;
            }
            foreach (Param.Row vrow in vanillaRows)
            {
                if (ParamUtils.RowMatches(row, vrow))
                    return false;//if we find a matching vanilla row
            }
            return true;
        }

        public void SetAssetLocator(AssetLocator l)
        {
            AssetLocator = l;
            //ReloadParams();
        }

        private void SaveParamsDS1()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\GameParam\GameParam.parambnd"))
            {
                MessageBox.Show("Could not find DS1 param file. Cannot save.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load params
            var param = $@"{mod}\param\GameParam\GameParam.parambnd";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\GameParam\GameParam.parambnd";
            }
            BND3 paramBnd = BND3.Read(param);

            // Replace params with edited ones
            foreach (var p in paramBnd.Files)
            {
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                {
                    p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                }
            }
            Utils.WriteWithBackup(dir, mod, @"param\GameParam\GameParam.parambnd", paramBnd);

            // Drawparam
            if (Directory.Exists($@"{AssetLocator.GameRootDirectory}\param\DrawParam"))
            {
                foreach (var bnd in Directory.GetFiles($@"{AssetLocator.GameRootDirectory}\param\DrawParam", "*.parambnd"))
                {
                    paramBnd = BND3.Read(bnd);
                    foreach (var p in paramBnd.Files)
                    {
                        if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                        {
                            p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                        }
                    }
                    Utils.WriteWithBackup(dir, mod, @$"param\DrawParam\{Path.GetFileName(bnd)}", paramBnd);
                }
            }
        }
        private void SaveParamsDS1R()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\GameParam\GameParam.parambnd.dcx"))
            {
                MessageBox.Show("Could not find DS1R param file. Cannot save.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load params
            var param = $@"{mod}\param\GameParam\GameParam.parambnd.dcx";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\GameParam\GameParam.parambnd.dcx";
            }
            BND3 paramBnd = BND3.Read(param);

            // Replace params with edited ones
            foreach (var p in paramBnd.Files)
            {
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                {
                    p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                }
            }
            Utils.WriteWithBackup(dir, mod, @"param\GameParam\GameParam.parambnd.dcx", paramBnd);

            // Drawparam
            if (Directory.Exists($@"{AssetLocator.GameRootDirectory}\param\DrawParam"))
            {
                foreach (var bnd in Directory.GetFiles($@"{AssetLocator.GameRootDirectory}\param\DrawParam", "*.parambnd.dcx"))
                {
                    paramBnd = BND3.Read(bnd);
                    foreach (var p in paramBnd.Files)
                    {
                        if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                        {
                            p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                        }
                    }
                    Utils.WriteWithBackup(dir, mod, @$"param\DrawParam\{Path.GetFileName(bnd)}", paramBnd);
                }
            }
        }

        private void SaveParamsDS2(bool loose)
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\enc_regulation.bnd.dcx"))
            {
                MessageBox.Show("Could not find DS2 regulation file. Cannot save.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load params
            var param = $@"{mod}\enc_regulation.bnd.dcx";
            BND4 paramBnd;
            if (!File.Exists(param))
            {
                // If there is no mod file, check the base file. Decrypt it if you have to.
                param = $@"{dir}\enc_regulation.bnd.dcx";
                if (!BND4.Is($@"{dir}\enc_regulation.bnd.dcx"))
                {
                    // Decrypt the file
                    paramBnd = SFUtil.DecryptDS2Regulation(param);

                    // Since the file is encrypted, check for a backup. If it has none, then make one and write a decrypted one.
                    if (!File.Exists($@"{param}.bak"))
                    {
                        File.Copy(param, $@"{param}.bak", true);
                        paramBnd.Write(param);
                    }
                }
                // No need to decrypt
                else
                {
                    paramBnd = BND4.Read(param);
                }
            }
            // Mod file exists, use that.
            else
            {
                paramBnd = BND4.Read(param);
            }

            // If params aren't loose, replace params with edited ones
            if (!loose)
            {
                // Replace params in paramBND, write remaining params loosely
                if (paramBnd.Files.Find(e => e.Name.EndsWith(".param")) == null)
                {
                    if (MessageBox.Show("It appears that you are trying to save params non-loosely with an \"enc_regulation.bnd\" that has previously been saved loosely." +
                        "\n\nWould you like to reinsert params into the bnd that were previously stripped out?", "DS2 de-loose param",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        param = $@"{dir}\enc_regulation.bnd.dcx";
                        if (!BND4.Is($@"{dir}\enc_regulation.bnd.dcx"))
                        {
                            // Decrypt the file
                            paramBnd = SFUtil.DecryptDS2Regulation(param);

                            // Since the file is encrypted, check for a backup. If it has none, then make one and write a decrypted one.
                            if (!File.Exists($@"{param}.bak"))
                            {
                                File.Copy(param, $@"{param}.bak", true);
                                paramBnd.Write(param);
                            }
                        }
                        else
                        {
                            paramBnd = BND4.Read(param);
                        }
                    }
                }

                foreach (var p in _params)
                {
                    var bnd = paramBnd.Files.Find(e => Path.GetFileNameWithoutExtension(e.Name) == p.Key);
                    if (bnd != null)
                    {
                        bnd.Bytes = p.Value.Write();
                    }
                    else
                    {
                        Utils.WriteWithBackup(dir, mod, $@"Param\{p.Key}.param", p.Value);
                    }
                }
            }
            else
            {
                // strip all the params from the regulation
                List<BinderFile> newFiles = new List<BinderFile>();
                foreach (var p in paramBnd.Files)
                {
                    if (!p.Name.ToUpper().Contains(".PARAM"))
                    {
                        newFiles.Add(p);
                    }
                }
                paramBnd.Files = newFiles;

                // Write all the params out loose
                foreach (var p in _params)
                {
                    Utils.WriteWithBackup(dir, mod, $@"Param\{p.Key}.param", p.Value);
                }

            }
            Utils.WriteWithBackup(dir, mod, @"enc_regulation.bnd.dcx", paramBnd);
        }

        private void SaveParamsDS3(bool loose)
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\Data0.bdt"))
            {
                MessageBox.Show("Could not find DS3 regulation file. Cannot save.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load params
            var param = $@"{mod}\Data0.bdt";
            if (!File.Exists(param))
            {
                param = $@"{dir}\Data0.bdt";
            }
            BND4 paramBnd = SFUtil.DecryptDS3Regulation(param);

            // Replace params with edited ones
            foreach (var p in paramBnd.Files)
            {
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                {
                    p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                }
            }

            // If not loose write out the new regulation
            if (!loose)
            {
                Utils.WriteWithBackup(dir, mod, @"Data0.bdt", paramBnd, GameType.DarkSoulsIII);
            }
            else
            {
                // Otherwise write them out as parambnds
                BND4 paramBND = new BND4
                {
                    BigEndian = false,
                    Compression = DCX.Type.DCX_DFLT_10000_44_9,
                    Extended = 0x04,
                    Unk04 = false,
                    Unk05 = false,
                    Format = Binder.Format.Compression | Binder.Format.Flag6 | Binder.Format.LongOffsets | Binder.Format.Names1,
                    Unicode = true,
                    Files = paramBnd.Files.Where(f => f.Name.EndsWith(".param")).ToList()
                };

                /*BND4 stayBND = new BND4
                {
                    BigEndian = false,
                    Compression = DCX.Type.DCX_DFLT_10000_44_9,
                    Extended = 0x04,
                    Unk04 = false,
                    Unk05 = false,
                    Format = Binder.Format.Compression | Binder.Format.Flag6 | Binder.Format.LongOffsets | Binder.Format.Names1,
                    Unicode = true,
                    Files = paramBnd.Files.Where(f => f.Name.EndsWith(".stayparam")).ToList()
                };*/

                Utils.WriteWithBackup(dir, mod, @"param\gameparam\gameparam_dlc2.parambnd.dcx", paramBND);
                //Utils.WriteWithBackup(dir, mod, @"param\stayparam\stayparam.parambnd.dcx", stayBND);
            }
        }

        private void SaveParamsBBSekiro()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\gameparam\gameparam.parambnd.dcx"))
            {
                MessageBox.Show("Could not find param file. Cannot save.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load params
            var param = $@"{mod}\param\gameparam\gameparam.parambnd.dcx";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\gameparam\gameparam.parambnd.dcx";
            }
            BND4 paramBnd = BND4.Read(param);

            // Replace params with edited ones
            foreach (var p in paramBnd.Files)
            {
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                {
                    p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                }
            }
            Utils.WriteWithBackup(dir, mod, @"param\gameparam\gameparam.parambnd.dcx", paramBnd);
        }
        private void SaveParamsDES()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;

            string paramBinderName = GetDesGameparamName(mod);
            if (paramBinderName == "")
            {
                paramBinderName = GetDesGameparamName(dir);
            }

            // Load params
            var param = $@"{mod}\param\gameparam\{paramBinderName}";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\gameparam\{paramBinderName}";
            }

            if (!File.Exists(param))
            {
                MessageBox.Show("Could not find param file. Cannot save.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            BND3 paramBnd = BND3.Read(param);

            // Replace params with edited ones
            foreach (var p in paramBnd.Files)
            {
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                {
                    p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                }
            }

            // Write all gameparam variations since we don't know which one the the game will use.
            // Compressed
            paramBnd.Compression = DCX.Type.DCX_EDGE;
            string naParamPath = $@"param\gameparam\gameparamna.parambnd.dcx";
            if (File.Exists($@"{dir}\{naParamPath}"))
            {
                Utils.WriteWithBackup(dir, mod, naParamPath, paramBnd);
            }
            Utils.WriteWithBackup(dir, mod, $@"param\gameparam\gameparam.parambnd.dcx", paramBnd);

            // Decompressed
            paramBnd.Compression = DCX.Type.None;
            naParamPath = $@"param\gameparam\gameparamna.parambnd";
            if (File.Exists($@"{dir}\{naParamPath}"))
            {
                Utils.WriteWithBackup(dir, mod, naParamPath, paramBnd);
            }
            Utils.WriteWithBackup(dir, mod, $@"param\gameparam\gameparam.parambnd", paramBnd);

            // Drawparam
            List<string> drawParambndPaths = new();
            if (Directory.Exists($@"{AssetLocator.GameRootDirectory}\param\drawparam"))
            {
                foreach (var bnd in Directory.GetFiles($@"{AssetLocator.GameRootDirectory}\param\drawparam", "*.parambnd.dcx"))
                {
                    drawParambndPaths.Add(bnd);
                }
                // Also save decompressed parambnds because DeS debug uses them.
                foreach (var bnd in Directory.GetFiles($@"{AssetLocator.GameRootDirectory}\param\drawparam", "*.parambnd"))
                {
                    drawParambndPaths.Add(bnd);
                }
                foreach (var bnd in drawParambndPaths)
                {
                    paramBnd = BND3.Read(bnd);
                    foreach (var p in paramBnd.Files)
                    {
                        if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                        {
                            p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                        }
                    }
                    Utils.WriteWithBackup(dir, mod, @$"param\drawparam\{Path.GetFileName(bnd)}", paramBnd);
                }
            }
        }
        private void SaveParamsER(bool partial)
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\regulation.bin"))
            {
                MessageBox.Show("Could not find param file. Cannot save.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load params
            var param = $@"{mod}\regulation.bin";
            if (!File.Exists(param) || _pendingUpgrade)
            {
                param = $@"{dir}\regulation.bin";
            }
            BND4 paramBnd = SFUtil.DecryptERRegulation(param);

            // Replace params with edited ones
            foreach (var p in paramBnd.Files)
            {
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                {
                    Param paramFile = _params[Path.GetFileNameWithoutExtension(p.Name)];
                    IReadOnlyList<Param.Row> backup = paramFile.Rows;
                    List<Param.Row> changed = new List<Param.Row>();
                    if (partial)
                    {
                        TaskManager.WaitAll();//wait on dirtycache update
                        HashSet<int> dirtyCache = _vanillaDiffCache[Path.GetFileNameWithoutExtension(p.Name)];
                        foreach (Param.Row row in paramFile.Rows)
                        {
                            if (dirtyCache.Contains(row.ID))
                                changed.Add(row);
                        }
                        paramFile.Rows = changed;
                    }
                    p.Bytes = paramFile.Write();
                    paramFile.Rows = backup;
                }
            }
            Utils.WriteWithBackup(dir, mod, @"regulation.bin", paramBnd, GameType.EldenRing);
            _pendingUpgrade = false;
        }

        public void SaveParams(bool loose = false, bool partialParams = false)
        {
            if (_params == null)
            {
                return;
            }
            if (AssetLocator.Type == GameType.DarkSoulsPTDE)
            {
                SaveParamsDS1();
            }
            if (AssetLocator.Type == GameType.DarkSoulsRemastered)
            {
                SaveParamsDS1R();
            }
            if (AssetLocator.Type == GameType.DemonsSouls)
            {
                SaveParamsDES();
            }
            if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                SaveParamsDS2(loose);
            }
            if (AssetLocator.Type == GameType.DarkSoulsIII)
            {
                SaveParamsDS3(loose);
            }
            if (AssetLocator.Type == GameType.Bloodborne || AssetLocator.Type == GameType.Sekiro)
            {
                SaveParamsBBSekiro();
            }
            if (AssetLocator.Type == GameType.EldenRing)
            {
                SaveParamsER(partialParams);
            }
        }

        public enum ParamUpgradeResult
        {
            Success = 0,
            RowConflictsFound = -1,
            OldRegulationNotFound = -2,
            OldRegulationVersionMismatch = -3,
        }

        private enum EditOperation
        {
            Add,
            Delete,
            Modify,
            NameChange,
            Match,
        }

        private static Param UpgradeParam(Param source, Param oldVanilla, Param newVanilla, HashSet<int> rowConflicts)
        {
            // Presorting this would make it easier, but we're trying to preserve order as much as possible
            // Unfortunately given that rows aren't guaranteed to be sorted and there can be duplicate IDs,
            // we try to respect the existing order and IDs as much as possible.

            // In order to assemble the final param, the param needs to know where to sort rows from given the
            // following rules:
            // 1. If a row with a given ID is unchanged from source to oldVanilla, we source from newVanilla
            // 2. If a row with a given ID is deleted from source compared to oldVanilla, we don't take any row
            // 3. If a row with a given ID is changed from source compared to oldVanilla, we source from source
            // 4. If a row has duplicate IDs, we treat them as if the rows were deduplicated and process them
            //    in the order they appear.

            // List of rows that are in source but not oldVanilla
            Dictionary<int, List<Param.Row>> addedRows = new Dictionary<int, List<Param.Row>>(source.Rows.Count);

            // List of rows in oldVanilla that aren't in source
            Dictionary<int, List<Param.Row>> deletedRows = new Dictionary<int, List<Param.Row>>(source.Rows.Count);

            // List of rows that are in source and oldVanilla, but are modified
            Dictionary<int, List<Param.Row>> modifiedRows = new Dictionary<int, List<Param.Row>>(source.Rows.Count);

            // List of rows that only had the name changed
            Dictionary<int, List<Param.Row>> renamedRows = new Dictionary<int, List<Param.Row>>(source.Rows.Count);

            // List of ordered edit operations for each ID
            Dictionary<int, List<EditOperation>> editOperations = new Dictionary<int, List<EditOperation>>(source.Rows.Count);

            // First off we go through source and everything starts as an added param
            foreach (var row in source.Rows)
            {
                if (!addedRows.ContainsKey(row.ID))
                    addedRows.Add(row.ID, new List<Param.Row>());
                addedRows[row.ID].Add(row);
            }

            // Next we go through oldVanilla to determine if a row is added, deleted, modified, or unmodified
            foreach (var row in oldVanilla.Rows)
            {
                // First off if the row did not exist in the source, it's deleted
                if (!addedRows.ContainsKey(row.ID))
                {
                    if (!deletedRows.ContainsKey(row.ID))
                        deletedRows.Add(row.ID, new List<Param.Row>());
                    deletedRows[row.ID].Add(row);
                    if (!editOperations.ContainsKey(row.ID))
                        editOperations.Add(row.ID, new List<EditOperation>());
                    editOperations[row.ID].Add(EditOperation.Delete);
                    continue;
                }

                // Otherwise the row exists in source. Time to classify it.
                var list = addedRows[row.ID];

                // First we see if we match the first target row. If so we can remove it.
                if (row.DataEquals(list[0]))
                {
                    var modrow = list[0];
                    list.RemoveAt(0);
                    if (list.Count == 0)
                        addedRows.Remove(row.ID);
                    if (!editOperations.ContainsKey(row.ID))
                        editOperations.Add(row.ID, new List<EditOperation>());

                    // See if the name was not updated
                    if ((modrow.Name == null && row.Name == null) ||
                        (modrow.Name != null && row.Name != null && modrow.Name == row.Name))
                    {
                        editOperations[row.ID].Add(EditOperation.Match);
                        continue;
                    }

                    // Name was updated
                    editOperations[row.ID].Add(EditOperation.NameChange);
                    if (!renamedRows.ContainsKey(row.ID))
                        renamedRows.Add(row.ID, new List<Param.Row>());
                    renamedRows[row.ID].Add(modrow);

                    continue;
                }

                // Otherwise it is modified
                if (!modifiedRows.ContainsKey(row.ID))
                    modifiedRows.Add(row.ID, new List<Param.Row>());
                modifiedRows[row.ID].Add(list[0]);
                list.RemoveAt(0);
                if (list.Count == 0)
                    addedRows.Remove(row.ID);
                if (!editOperations.ContainsKey(row.ID))
                    editOperations.Add(row.ID, new List<EditOperation>());
                editOperations[row.ID].Add(EditOperation.Modify);
            }

            // Mark all remaining rows as added
            foreach (var entry in addedRows)
            {
                if (!editOperations.ContainsKey(entry.Key))
                    editOperations.Add(entry.Key, new List<EditOperation>());
                foreach (var k in editOperations.Values)
                    editOperations[entry.Key].Add(EditOperation.Add);
            }

            Param dest = new Param(newVanilla);

            // Now try to build the destination from the new regulation with the edit operations in mind
            var pendingAdds = addedRows.Keys.OrderBy(e => e).ToArray();
            int currPendingAdd = 0;
            int lastID = 0;
            foreach (var row in newVanilla.Rows)
            {
                // See if we have any pending adds we can slot in
                while (currPendingAdd < pendingAdds.Length &&
                       pendingAdds[currPendingAdd] >= lastID &&
                       pendingAdds[currPendingAdd] < row.ID)
                {
                    if (!addedRows.ContainsKey(pendingAdds[currPendingAdd]))
                    {
                        currPendingAdd++;
                        continue;
                    }
                    foreach (var arow in addedRows[pendingAdds[currPendingAdd]])
                    {
                        dest.AddRow(new Param.Row(arow, dest));
                    }

                    addedRows.Remove(pendingAdds[currPendingAdd]);
                    editOperations.Remove(pendingAdds[currPendingAdd]);
                    currPendingAdd++;
                }

                lastID = row.ID;

                if (!editOperations.ContainsKey(row.ID))
                {
                    // No edit operations for this ID, so just add it (likely a new row in the update)
                    dest.AddRow(new Param.Row(row, dest));
                    continue;
                }

                // Pop the latest operation we need to do
                var operation = editOperations[row.ID][0];
                editOperations[row.ID].RemoveAt(0);
                if (editOperations[row.ID].Count == 0)
                    editOperations.Remove(row.ID);

                if (operation == EditOperation.Add)
                {
                    // Getting here means both the mod and the updated regulation added a row. Our current strategy is
                    // to overwrite the new vanilla row with the modded one and add to the conflict log to give the user
                    rowConflicts.Add(row.ID);
                    dest.AddRow(new Param.Row(addedRows[row.ID][0], dest));
                    addedRows[row.ID].RemoveAt(0);
                    if (addedRows[row.ID].Count == 0)
                        addedRows.Remove(row.ID);
                }
                else if (operation == EditOperation.Match)
                {
                    // Match means we inherit updated param
                    dest.AddRow(new Param.Row(row, dest));
                }
                else if (operation == EditOperation.Delete)
                {
                    // deleted means we don't add anything
                    deletedRows[row.ID].RemoveAt(0);
                    if (deletedRows[row.ID].Count == 0)
                        deletedRows.Remove(row.ID);
                }
                else if (operation == EditOperation.Modify)
                {
                    // Modified means we use the modded regulation's param
                    dest.AddRow(new Param.Row(modifiedRows[row.ID][0], dest));
                    modifiedRows[row.ID].RemoveAt(0);
                    if (modifiedRows[row.ID].Count == 0)
                        modifiedRows.Remove(row.ID);
                }
                else if (operation == EditOperation.NameChange)
                {
                    // Inherit name
                    var newRow = new Param.Row(row, dest);
                    newRow.Name = renamedRows[row.ID][0].Name;
                    dest.AddRow(newRow);
                    renamedRows[row.ID].RemoveAt(0);
                    if (renamedRows[row.ID].Count == 0)
                        renamedRows.Remove(row.ID);
                }
            }

            // Take care of any more pending adds
            for (; currPendingAdd < pendingAdds.Length; currPendingAdd++)
            {
                // If the pending add doesn't exist in the added rows list, it was a conflicting row
                if (!addedRows.ContainsKey(pendingAdds[currPendingAdd]))
                    continue;

                foreach (var arow in addedRows[pendingAdds[currPendingAdd]])
                {
                    dest.AddRow(new Param.Row(arow, dest));
                }

                addedRows.Remove(pendingAdds[currPendingAdd]);
                editOperations.Remove(pendingAdds[currPendingAdd]);
            }

            return dest;
        }

        // Param upgrade. Currently for Elden Ring only.
        public ParamUpgradeResult UpgradeRegulation(ParamBank vanillaBank, string oldVanillaParamPath,
            Dictionary<string, HashSet<int>> conflictingParams)
        {
            // First we need to load the old regulation
            if (!File.Exists(oldVanillaParamPath))
                return ParamUpgradeResult.OldRegulationNotFound;

            // Load old vanilla regulation
            BND4 oldVanillaParamBnd = SFUtil.DecryptERRegulation(oldVanillaParamPath);
            var oldVanillaParams = new Dictionary<string, Param>();
            ulong version;
            LoadParamFromBinder(oldVanillaParamBnd, ref oldVanillaParams, out version, true);
            if (version != ParamVersion)
                return ParamUpgradeResult.OldRegulationVersionMismatch;

            var updatedParams = new Dictionary<string, Param>();
            // Now we must diff everything to try and find changed/added rows for each param
            foreach (var k in vanillaBank.Params.Keys)
            {
                // If the param is completely new, just take it
                if (!oldVanillaParams.ContainsKey(k) || !Params.ContainsKey(k))
                {
                    updatedParams.Add(k, vanillaBank.Params[k]);
                    continue;
                }

                // Otherwise try to upgrade
                var conflicts = new HashSet<int>();
                var res = UpgradeParam(Params[k], oldVanillaParams[k], vanillaBank.Params[k], conflicts);
                updatedParams.Add(k, res);

                if (conflicts.Count > 0)
                    conflictingParams.Add(k, conflicts);
            }

            ulong oldVersion = _paramVersion;

            // Set new params
            _params = updatedParams;
            _paramVersion = VanillaBank.ParamVersion;
            _pendingUpgrade = true;

            // Refresh dirty cache
            CacheBank.ClearCaches();
            RefreshParamDiffCaches();

            return conflictingParams.Count > 0 ? ParamUpgradeResult.RowConflictsFound : ParamUpgradeResult.Success;
        }

        public (List<string>, List<string>) RunUpgradeEdits(ulong startVersion, ulong endVersion)
        {
            // Temporary data could be moved somewhere static
            (ulong, string, string)[] paramUpgradeTasks = new (ulong, string, string)[0];
            if (AssetLocator.Type == GameType.EldenRing)
            {
                // Note these all use modified as any unmodified row already matches the target. This only fails if a mod pre-empts fromsoft's exact change.
                paramUpgradeTasks = new (ulong, string, string)[]{
                    (10701000l, "1.07 - (SwordArtsParam) Move swordArtsType to swordArtsTypeNew", "param SwordArtsParam: modified: swordArtsTypeNew: = field swordArtsType;"),
                    (10701000l, "1.07 - (SwordArtsParam) Set swordArtsType to 0", "param SwordArtsParam: modified && !added: swordArtsType: = 0;"),
                    (10701000l, "1.07 - (AtkParam PC/NPC) Set added finalAttackDamageRate refs to -1", "param AtkParam_(Pc|Npc): modified && added: finalDamageRateId: = -1;"),
                    (10701000l, "1.07 - (AtkParam PC/NPC) Set not-added finalAttackDamageRate refs to vanilla", "param AtkParam_(Pc|Npc): modified && !added: finalDamageRateId: = vanillafield finalDamageRateId;"),
                    (10701000l, "1.07 - (AssetEnvironmentGeometryParam) Set reserved_124 to Vanilla v1.07 values", "param GameSystemCommonParam: modified && !added: reserved_124: = vanillafield reserved_124;"),
                    (10701000l, "1.07 - (AssetEnvironmentGeometryParam) Set reserved41 to Vanilla v1.07 values", "param PlayerCommonParam: modified: reserved41: = vanillafield reserved41;"),
                    (10701000l, "1.07 - (AssetEnvironmentGeometryParam) Set Reserve_1 to Vanilla v1.07 values", "param AssetEnvironmentGeometryParam: modified: Reserve_1: = vanillafield Reserve_1;"),
                    (10701000l, "1.07 - (AssetEnvironmentGeometryParam) Set Reserve_2 to Vanilla v1.07 values", "param AssetEnvironmentGeometryParam: modified: Reserve_2: = vanillafield Reserve_2;"),
                    (10701000l, "1.07 - (AssetEnvironmentGeometryParam) Set Reserve_3 to Vanilla v1.07 values", "param AssetEnvironmentGeometryParam: modified: Reserve_3: = vanillafield Reserve_3;"),
                    (10701000l, "1.07 - (AssetEnvironmentGeometryParam) Set Reserve_4 to Vanilla v1.07 values", "param AssetEnvironmentGeometryParam: modified: Reserve_4: = vanillafield Reserve_4;"),
                    (10801000l, "1.08 - (BuddyParam) Set Unk1 to default value", "param BuddyParam: modified: Unk1: = 1410;"),
                    (10801000l, "1.08 - (BuddyParam) Set Unk2 to default value", "param BuddyParam: modified: Unk2: = 1420;"),
                    (10801000l, "1.08 - (BuddyParam) Set Unk11 to default value", "param BuddyParam: modified: Unk11: = 1400;"),
                    (10900000l, "1.09 - (GameSystemCommonParam) Set reserved_124 to Vanilla v1.09 values", "param GameSystemCommonParam: id 0: reserved_124: = vanillafield reserved_124;")
                };
            }

            List<string> performed = new List<string>();
            List<string> unperformed = new List<string>();

            bool hasFailed = false;
            foreach (var (version, task, command) in paramUpgradeTasks)
            {
                // Don't bother updating modified cache between edits
                if (version <= startVersion || version > endVersion)
                    continue;

                if (!hasFailed)
                {
                    try {
                        var (result, actions) = MassParamEditRegex.PerformMassEdit(this, command, null);
                        if (result.Type != MassEditResultType.SUCCESS)
                            hasFailed = true;
                    }
                    catch (Exception e)
                    {
                        hasFailed = true;
                    }
                }
                if (!hasFailed)
                    performed.Add(task);
                else
                    unperformed.Add(task);
            }
            return (performed, unperformed);
        }

        public string GetChrIDForEnemy(long enemyID)
        {
            var enemy = EnemyParam?[(int)enemyID];
            return enemy != null ? $@"{enemy.GetCellHandleOrThrow("chr_id").Value:D4}" : null;
        }

        public string GetKeyForParam(Param param)
        {
            foreach (KeyValuePair<string, Param> pair in Params)
            {
                if (param == pair.Value)
                    return pair.Key;
            }
            return null;
        }

        public Param GetParamFromName(string param)
        {
            foreach (KeyValuePair<string, Param> pair in Params)
            {
                if (param == pair.Key)
                    return pair.Value;
            }
            return null;
        }

        private static HashSet<int> EMPTYSET = new HashSet<int>();
        public HashSet<int> GetVanillaDiffRows(string param)
        {
            var allDiffs = VanillaDiffCache;
            if (allDiffs == null || !allDiffs.ContainsKey(param))
                return EMPTYSET;
            return allDiffs[param];
        }
        public HashSet<int> GetPrimaryDiffRows(string param)
        {
            var allDiffs = PrimaryDiffCache;
            if (allDiffs == null || !allDiffs.ContainsKey(param))
                return EMPTYSET;
            return allDiffs[param];
        }
    }
}
