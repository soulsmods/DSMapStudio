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
        private static Param EnemyParam = null;
        internal static AssetLocator AssetLocator = null;

        private static Dictionary<string, Param> _params = null;
        private static Dictionary<string, Param> _vanillaParams = null;
        private static Dictionary<string, PARAMDEF> _paramdefs = null;
        private static Dictionary<string, Dictionary<ulong, PARAMDEF>> _patchParamdefs = null;
        private static Dictionary<string, HashSet<int>> _paramDirtyCache = null; //If param != vanillaparam

        private static bool _pendingUpgrade = false;
        
        public static bool IsDefsLoaded { get; private set; } = false;
        public static bool IsMetaLoaded { get; private set; } = false;
        public static bool IsLoadingParams { get; private set; } = false;
        public static bool IsLoadingVParams { get; private set; } = false;

        public static IReadOnlyDictionary<string, Param> Params
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
        public static IReadOnlyDictionary<string, Param> VanillaParams
        {
            get
            {
                if (IsLoadingVParams)
                {
                    return null;
                }
                return _vanillaParams;
            }
        }

        private static ulong _paramVersion;
        public static ulong ParamVersion
        {
            get => _paramVersion;
        }

        private static ulong _vanillaParamVersion;
        public static ulong VanillaParamVersion
        {
            get => _vanillaParamVersion;
        }
        
        public static IReadOnlyDictionary<string, HashSet<int>> DirtyParamCache
        {
            get
            {
                if (IsLoadingParams)
                {
                    return null;
                }
                return _paramDirtyCache;
            }
        }

        // DS2 Only
        private static Param GetParam(BND4 parambnd, string paramfile)
        {
            var bndfile = parambnd.Files.Find(x => Path.GetFileName(x.Name) == paramfile);
            if (bndfile != null)
            {
                return Param.Read(bndfile.Bytes);
            }

            // Otherwise the param is a loose param
            if (File.Exists($@"{AssetLocator.GameModDirectory}\Param\{paramfile}"))
            {
                return Param.Read($@"{AssetLocator.GameModDirectory}\Param\{paramfile}");
            }
            if (File.Exists($@"{AssetLocator.GameRootDirectory}\Param\{paramfile}"))
            {
                return Param.Read($@"{AssetLocator.GameRootDirectory}\Param\{paramfile}");
            }
            return null;
        }

        private static List<(string, PARAMDEF)> LoadParamdefs()
        {
            _paramdefs = new Dictionary<string, PARAMDEF>();
            _patchParamdefs = new Dictionary<string, Dictionary<ulong, PARAMDEF>>();
            var dir = AssetLocator.GetParamdefDir();
            var files = Directory.GetFiles(dir, "*.xml");
            List<(string, PARAMDEF)> defPairs = new List<(string, PARAMDEF)>();
            foreach (var f in files)
            {
                var pdef = PARAMDEF.XmlDeserialize(f);
                _paramdefs.Add(pdef.ParamType, pdef);
                defPairs.Add((f, pdef));
            }
            
            // Load patch paramdefs
            var patches = AssetLocator.GetParamdefPatches();
            foreach (var patch in patches)
            {
                var pdir = AssetLocator.GetParamdefPatchDir(patch);
                var pfiles = Directory.GetFiles(pdir, "*.xml");
                foreach (var f in pfiles)
                {
                    var pdef = PARAMDEF.XmlDeserialize(f);
                    defPairs.Add((pdef.ParamType, pdef));
                    if (!_patchParamdefs.ContainsKey(pdef.ParamType))
                    {
                        _patchParamdefs[pdef.ParamType] = new Dictionary<ulong, PARAMDEF>();
                    }
                    _patchParamdefs[pdef.ParamType].Add(patch, pdef);
                }
            }
            
            return defPairs;
        }

        public static void LoadParamMeta(List<(string, PARAMDEF)> defPairs)
        {
            var mdir = AssetLocator.GetParammetaDir();
            foreach ((string f, PARAMDEF pdef) in defPairs)
            {
                var fName = f.Substring(f.LastIndexOf('\\') + 1);
                ParamMetaData.XmlDeserialize($@"{mdir}\{fName}", pdef);
            }
        }

        public static CompoundAction LoadParamDefaultNames()
        {
            var dir = AssetLocator.GetParamNamesDir();
            var files = Directory.GetFiles(dir, "*.txt");
            List<EditorAction> actions = new List<EditorAction>();
            foreach (var f in files)
            {
                int last = f.LastIndexOf('\\') + 1;
                string file = f.Substring(last);
                string param = file.Substring(0, file.Length - 4);
                if (!_params.ContainsKey(param))
                    continue;
                string names = File.ReadAllText(f);
                (MassEditResult r, CompoundAction a) = MassParamEditCSV.PerformSingleMassEdit(names, param, "Name", ' ');
                actions.Add(a);
            }
            return new CompoundAction(actions);
        }

        public static ActionManager TrimNewlineChrsFromNames()
        {
            (MassEditResult r, ActionManager child) =
                MassParamEditRegex.PerformMassEdit("param .*: id .*: name: replace \r:0", null);
            return child;
        }

        private static void LoadParamFromBinder(IBinder parambnd, ref Dictionary<string, FSParam.Param> paramBank, out ulong version)
        {
            bool success = ulong.TryParse(parambnd.Version, out version);
            if (!success)
            {
                throw new Exception($@"Failed to get regulation version. Params might be corrupt.");
            }
            
            // Load every param in the regulation
            // _params = new Dictionary<string, PARAM>();
            foreach (var f in parambnd.Files)
            {
                if (!f.Name.ToUpper().EndsWith(".PARAM") || Path.GetFileNameWithoutExtension(f.Name).StartsWith("default_"))
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
                
                p.ApplyParamdef(def);
                paramBank.Add(Path.GetFileNameWithoutExtension(f.Name), p);
            }
        }

        private static string LoadParamsDES()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;

            string paramBinderName = "gameparam.parambnd.dcx";

            if (Directory.GetParent(dir).Parent.FullName.Contains("BLUS"))
            {
                paramBinderName = "gameparamna.parambnd.dcx";
            }

            if (!File.Exists($@"{dir}\\param\gameparam\{paramBinderName}"))
            {
                //MessageBox.Show("Could not find DES regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
                throw new FileNotFoundException("Could not find DES regulation file. Functionality will be limited.");
            }

            // Load params
            var param = $@"{mod}\param\gameparam\{paramBinderName}";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\gameparam\{paramBinderName}";
            }
            BND3 paramBnd = BND3.Read(param);

            LoadParamFromBinder(paramBnd, ref _params, out _paramVersion);
            return dir;
        }
        private static void LoadVParamsDES(string dir)
        {
            string paramBinderName = "gameparam.parambnd.dcx";
            if (Directory.GetParent(dir).Parent.FullName.Contains("BLUS"))
            {
                paramBinderName = "gameparamna.parambnd.dcx";
            }
            LoadParamFromBinder(BND3.Read($@"{dir}\param\gameparam\{paramBinderName}"), ref _vanillaParams, out _vanillaParamVersion);
        }

        private static string LoadParamsDS1()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\GameParam\GameParam.parambnd"))
            {
                //MessageBox.Show("Could not find DS1 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
                throw new FileNotFoundException("Could not find DS1 regulation file. Functionality will be limited.");
            }

            // Load params
            var param = $@"{mod}\param\GameParam\GameParam.parambnd";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\GameParam\GameParam.parambnd";
            }
            BND3 paramBnd = BND3.Read(param);

            LoadParamFromBinder(paramBnd, ref _params, out _paramVersion);
            return dir;
        }
        private static void LoadVParamsDS1(string dir)
        {
            LoadParamFromBinder(BND3.Read($@"{dir}\param\GameParam\GameParam.parambnd"), ref _vanillaParams, out _vanillaParamVersion);
        }

        private static string LoadParamsDS1R()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\GameParam\GameParam.parambnd.dcx"))
            {
                //MessageBox.Show("Could not find DS1 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
                throw new FileNotFoundException("Could not find DS1 regulation file. Functionality will be limited.");
            }

            // Load params
            var param = $@"{mod}\param\GameParam\GameParam.parambnd.dcx";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\GameParam\GameParam.parambnd.dcx";
            }
            BND3 paramBnd = BND3.Read(param);

            LoadParamFromBinder(paramBnd, ref _params, out _paramVersion);
            return dir;
        }
        private static void LoadVParamsDS1R(string dir)
        {
            LoadParamFromBinder(BND3.Read($@"{dir}\param\GameParam\GameParam.parambnd.dcx"), ref _vanillaParams, out _vanillaParamVersion);
        }

        private static string LoadParamsBBSekrio()
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
            BND4 paramBnd = BND4.Read(param);

            LoadParamFromBinder(paramBnd, ref _params, out _paramVersion);
            return dir;
        }
        private static void LoadVParamsBBSekrio(string dir)
        {
            LoadParamFromBinder(BND4.Read($@"{dir}\param\gameparam\gameparam.parambnd.dcx"), ref _vanillaParams, out _vanillaParamVersion);
        }

        /// <summary>
        /// Map related params that should not be in the param editor
        /// </summary>
        private static List<string> _ds2ParamBlacklist = new List<string>()
        {
            "demopointlight",
            "demospotlight",
            "eventlocation",
            "eventparam",
            "generatordbglocation",
            "hitgroupparam",
            "intrudepointparam",
            "mapobjectinstanceparam",
            "maptargetdirparam",
            "npctalkparam",
            "treasureboxparam",
        };

        private static string LoadParamsDS2()
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
            foreach (var d in scandir)
            {
                var paramfiles = Directory.GetFileSystemEntries(d, @"*.param");
                foreach (var p in paramfiles)
                {
                    bool blacklisted = false;
                    var name = Path.GetFileNameWithoutExtension(p);
                    foreach (var bl in _ds2ParamBlacklist)
                    {
                        if (name.StartsWith(bl))
                        {
                            blacklisted = true;
                        }
                    }
                    if (blacklisted)
                    {
                        continue;
                    }

                    var lp = Param.Read(p);
                    var fname = lp.ParamType;
                    PARAMDEF def = AssetLocator.GetParamdefForParam(fname);
                    lp.ApplyParamdef(def);
                    if (!_params.ContainsKey(name))
                    {
                        _params.Add(name, lp);
                    }
                }
            }

            // Load reg params
            var param = $@"{mod}\enc_regulation.bnd.dcx";
            BND4 paramBnd;
            if (!File.Exists(param))
            {
                // If there is no mod file, check the base file. Decrypt it if you have to.
                param = $@"{dir}\enc_regulation.bnd.dcx";
                if (!BND4.Is($@"{dir}\enc_regulation.bnd.dcx"))
                {
                    paramBnd = SFUtil.DecryptDS2Regulation(param);
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

            EnemyParam = GetParam(paramBnd, "EnemyParam.param");
            if (EnemyParam != null)
            {
                PARAMDEF def = AssetLocator.GetParamdefForParam(EnemyParam.ParamType);
                EnemyParam.ApplyParamdef(def);
            }

            LoadParamFromBinder(paramBnd, ref _params, out _paramVersion);
            return dir;
        }
        private static void LoadVParamsDS2(string dir)
        {
            // Load loose params
            var paramfiles = Directory.GetFileSystemEntries($@"{dir}\Param", @"*.param");
            foreach (var p in paramfiles)
            {
                bool blacklisted = false;
                var name = Path.GetFileNameWithoutExtension(p);
                foreach (var bl in _ds2ParamBlacklist)
                {
                    if (name.StartsWith(bl))
                    {
                        blacklisted = true;
                    }
                }
                if (blacklisted)
                {
                    continue;
                }

                var lp = Param.Read(p);
                var fname = lp.ParamType;
                PARAMDEF def = AssetLocator.GetParamdefForParam(fname);
                lp.ApplyParamdef(def);
                if (!_vanillaParams.ContainsKey(name))
                {
                    _vanillaParams.Add(name, lp);
                }
            }
            // Load reg params
            BND4 vParamBnd = null;
            if (!BND4.Is($@"{dir}\enc_regulation.bnd.dcx"))
            {
                vParamBnd = SFUtil.DecryptDS2Regulation($@"{dir}\enc_regulation.bnd.dcx");
            }
            // No need to decrypt
            else
            {
                vParamBnd = BND4.Read($@"{dir}\enc_regulation.bnd.dcx");
            }
            LoadParamFromBinder(vParamBnd, ref _vanillaParams, out _vanillaParamVersion);
        }

        private static string LoadParamsDS3()
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
            if (File.Exists($@"{mod}\\param\gameparam\gameparam_dlc2.parambnd.dcx"))
            {
                // Load params
                var lparam = $@"{mod}\param\gameparam\gameparam_dlc2.parambnd.dcx";
                BND4 lparamBnd = BND4.Read(lparam);

                LoadParamFromBinder(lparamBnd, ref _params, out _paramVersion);
            }
            else
            {
                // Load params
                var param = $@"{mod}\Data0.bdt";
                if (!File.Exists(param))
                {
                    param = vparam;
                }
                BND4 paramBnd = SFUtil.DecryptDS3Regulation(param);
                LoadParamFromBinder(paramBnd, ref _params, out _paramVersion);
            }
            return vparam;
        }
        private static void LoadVParamsDS3(string vparam)
        {
            BND4 vParamBnd = SFUtil.DecryptDS3Regulation(vparam);
            LoadParamFromBinder(vParamBnd, ref _vanillaParams, out _vanillaParamVersion);
        }

        private static string LoadParamsER(bool partial)
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
            BND4 paramBnd = SFUtil.DecryptERRegulation(param);

            LoadParamFromBinder(paramBnd, ref _params, out _paramVersion);

            param = $@"{mod}\regulation.bin";
            if (partial && File.Exists(param))
            {
                BND4 pParamBnd = SFUtil.DecryptERRegulation(param);
                Dictionary<string, Param> cParamBank = new Dictionary<string, Param>();
                ulong v;
                LoadParamFromBinder(pParamBnd, ref cParamBank, out v);
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

            return dir;
        }
        private static void LoadVParamsER(string dir)
        {
            //LoadParamFromBinder(SFUtil.DecryptERRegulation($@"{dir}\regulation.bin"), ref _vanillaParams);
            LoadParamFromBinder(SFUtil.DecryptERRegulation($@"{dir}\regulation.bin"), ref _vanillaParams, out _vanillaParamVersion);
        }

        //Some returns and repetition, but it keeps all threading and loading-flags visible inside this method
        public static void ReloadParams(ProjectSettings settings, NewProjectOptions options)
        {
            _paramdefs = new Dictionary<string, PARAMDEF>();
            _params = new Dictionary<string, Param>();
            IsDefsLoaded = false;
            IsLoadingParams = true;

            TaskManager.Run("PB:LoadParams", true, false, true, () =>
            {
                if (AssetLocator.Type != GameType.Undefined)
                {
                    List<(string, PARAMDEF)> defPairs = LoadParamdefs();
                    IsDefsLoaded = true;
                    TaskManager.Run("PB:LoadParamMeta", true, false, false, () =>
                    {
                        IsMetaLoaded = false;
                        LoadParamMeta(defPairs);
                        IsMetaLoaded = true;
                    });
                }
                string vparamDir = null;
                if (AssetLocator.Type == GameType.DemonsSouls)
                {
                    vparamDir = LoadParamsDES();
                }
                if (AssetLocator.Type == GameType.DarkSoulsPTDE)
                {
                    vparamDir = LoadParamsDS1();
                }
                if (AssetLocator.Type == GameType.DarkSoulsRemastered)
                {
                    vparamDir = LoadParamsDS1R();
                }
                if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
                {
                    vparamDir = LoadParamsDS2();
                }
                if (AssetLocator.Type == GameType.DarkSoulsIII)
                {
                    vparamDir = LoadParamsDS3();
                }
                if (AssetLocator.Type == GameType.Bloodborne || AssetLocator.Type == GameType.Sekiro)
                {
                    vparamDir = LoadParamsBBSekrio();
                }
                if (AssetLocator.Type == GameType.EldenRing)
                {
                    vparamDir = LoadParamsER(settings.PartialParams);
                }

                if (vparamDir != null)
                {
                    IsLoadingVParams = true;
                    _vanillaParams = new Dictionary<string, Param>();
                    TaskManager.Run("PB:LoadVParams", true, false, false, () =>
                    {
                        if (AssetLocator.Type == GameType.DemonsSouls)
                        {
                            LoadVParamsDES(vparamDir);
                        }
                        if (AssetLocator.Type == GameType.DarkSoulsPTDE)
                        {
                            LoadVParamsDS1(vparamDir);
                        }
                        if (AssetLocator.Type == GameType.DarkSoulsRemastered)
                        {
                            LoadVParamsDS1R(vparamDir);
                        }
                        if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
                        {
                            LoadVParamsDS2(vparamDir);
                        }
                        if (AssetLocator.Type == GameType.DarkSoulsIII)
                        {
                            LoadVParamsDS3(vparamDir);
                        }
                        if (AssetLocator.Type == GameType.Bloodborne || AssetLocator.Type == GameType.Sekiro)
                        {
                            LoadVParamsBBSekrio(vparamDir);
                        }
                        if (AssetLocator.Type == GameType.EldenRing)
                        {
                            LoadVParamsER(vparamDir);
                        }
                        IsLoadingVParams = false;
                    });
                }

                _paramDirtyCache = new Dictionary<string, HashSet<int>>();
                foreach (string param in _params.Keys)
                    _paramDirtyCache.Add(param, new HashSet<int>());

                IsLoadingParams = false;

                if (options != null)
                {
                    if (options.loadDefaultNames)
                    {
                        new Editor.ActionManager().ExecuteAction(ParamEditor.ParamBank.LoadParamDefaultNames());
                        ParamEditor.ParamBank.SaveParams(settings.UseLooseParams);
                    }
                }
            });
        }

        public static void refreshParamDirtyCache()
        {
            if (IsLoadingParams || IsLoadingVParams)
                return;
            Dictionary<string, HashSet<int>> newCache = new Dictionary<string, HashSet<int>>();
            foreach (string param in _params.Keys)
            {
                HashSet<int> cache = new HashSet<int>();
                newCache.Add(param, cache);
                Param p = _params[param];
                if (!_vanillaParams.ContainsKey(param))
                {
                    Console.WriteLine("Missing vanilla param "+param);
                    continue;
                }

                var rows = _params[param].Rows.OrderBy(r => r.ID).ToArray();
                var vrows = _vanillaParams[param].Rows.OrderBy(r => r.ID).ToArray();
                
                var vanillaIndex = 0;
                int lastID = -1;
                Span<Param.Row> lastVanillaRows = default;
                for (int i = 0; i < rows.Length; i++)
                {
                    int ID = rows[i].ID;
                    if (ID == lastID)
                    {
                        refreshParamRowDirtyCache(rows[i], lastVanillaRows, cache);
                    }
                    else
                    {
                        while (vanillaIndex < vrows.Length && vrows[vanillaIndex].ID < ID)
                            vanillaIndex++;
                        if (vanillaIndex >= vrows.Length)
                        {
                            refreshParamRowDirtyCache(rows[i], Span<Param.Row>.Empty, cache);
                        }
                        else
                        {
                            int count = 0;
                            while (vanillaIndex + count < vrows.Length && vrows[vanillaIndex + count].ID == ID)
                                count++;
                            refreshParamRowDirtyCache(rows[i], new ReadOnlySpan<Param.Row>(vrows, vanillaIndex, count), cache);
                            vanillaIndex += count;
                        }
                    }
                }
            }
            _paramDirtyCache = newCache;
        }
        public static void refreshParamRowDirtyCache(Param.Row row, ReadOnlySpan<Param.Row> vanillaRows, HashSet<int> cache)
        {
            if (IsChanged(row, vanillaRows))
                cache.Add(row.ID);
            else
                cache.Remove(row.ID);
        }
        
        public static void refreshParamRowDirtyCache(Param.Row row, Param vanillaParam, HashSet<int> cache)
        {
            var vanillaRows = vanillaParam.Rows.Where(cell => cell.ID == row.ID).ToArray();
            if (IsChanged(row, vanillaRows))
                cache.Add(row.ID);
            else
                cache.Remove(row.ID);
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

        public static void SetAssetLocator(AssetLocator l)
        {
            AssetLocator = l;
            //ReloadParams();
        }

        private static void SaveParamsDS1()
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
            // Don't write to mod dir for now
            Utils.WriteWithBackup(dir, mod, @"param\GameParam\GameParam.parambnd", paramBnd);
        }
        private static void SaveParamsDS1R()
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
        }

        private static void SaveParamsDS2(bool loose)
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
                foreach (var p in paramBnd.Files)
                {
                    if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                    {
                        p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
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

        private static void SaveParamsDS3(bool loose)
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

        private static void SaveParamsBBSekiro()
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
        private static void SaveParamsDES()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;

            string paramBinderName = "gameparam.parambnd.dcx";

            if (Directory.GetParent(dir).Parent.FullName.Contains("BLUS"))
            {
                paramBinderName = "gameparamna.parambnd.dcx";
            }

            Debug.WriteLine(paramBinderName);

            if (!File.Exists($@"{dir}\\param\gameparam\{paramBinderName}"))
            {
                MessageBox.Show("Could not find param file. Cannot save.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load params
            var param = $@"{mod}\param\gameparam\{paramBinderName}";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\gameparam\{paramBinderName}";
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
            Utils.WriteWithBackup(dir, mod, $@"param\gameparam\{paramBinderName}", paramBnd);
        }
        private static void SaveParamsER(bool partial)
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
                        HashSet<int> dirtyCache = _paramDirtyCache[Path.GetFileNameWithoutExtension(p.Name)];
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

        public static void SaveParams(bool loose = false, bool partialParams = false)
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
                if (row.Equals(list[0]))
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
        public static ParamUpgradeResult UpgradeRegulation(string oldVanillaParamPath, 
            Dictionary<string, HashSet<int>> conflictingParams)
        {
            // First we need to load the old regulation
            if (!File.Exists(oldVanillaParamPath))
                return ParamUpgradeResult.OldRegulationNotFound;

            // Load old vanilla regulation
            BND4 oldVanillaParamBnd = SFUtil.DecryptERRegulation(oldVanillaParamPath);
            var oldVanillaParams = new Dictionary<string, Param>();
            ulong version;
            LoadParamFromBinder(oldVanillaParamBnd, ref oldVanillaParams, out version);
            if (version != ParamVersion)
                return ParamUpgradeResult.OldRegulationVersionMismatch;

            var updatedParams = new Dictionary<string, Param>();
            // Now we must diff everything to try and find changed/added rows for each param
            foreach (var k in VanillaParams.Keys)
            {
                // If the param is completely new, just take it
                if (!oldVanillaParams.ContainsKey(k) || !Params.ContainsKey(k))
                {
                    updatedParams.Add(k, VanillaParams[k]);
                    continue;
                }
                
                // Otherwise try to upgrade
                var conflicts = new HashSet<int>();
                var res = UpgradeParam(Params[k], oldVanillaParams[k], VanillaParams[k], conflicts);
                updatedParams.Add(k, res);
                
                if (conflicts.Count > 0)
                    conflictingParams.Add(k, conflicts);
            }
            
            // Set new params
            _params = updatedParams;
            _paramVersion = VanillaParamVersion;
            _pendingUpgrade = true;

            return conflictingParams.Count > 0 ? ParamUpgradeResult.RowConflictsFound : ParamUpgradeResult.Success;
        }

        public static string GetChrIDForEnemy(long enemyID)
        {
            var enemy = EnemyParam?[(int)enemyID];
            return enemy != null ? $@"{enemy.GetCellHandleOrThrow("Chr ID").Value:D4}" : null;
        }

        public static string GetKeyForParam(Param param)
        {
            foreach (KeyValuePair<string, Param> pair in ParamBank.Params)
            {
                if (param == pair.Value)
                    return pair.Key;
            }
            return null;
        }
    }
}
