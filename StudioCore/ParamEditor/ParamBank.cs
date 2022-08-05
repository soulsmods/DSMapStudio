using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SoulsFormats;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using FSParam;
using StudioCore.Editor;

namespace StudioCore.ParamEditor
{
    /// <summary>
    /// Utilities for dealing with global params for a game
    /// </summary>
    public class ParamBank
    {
        private static PARAM EnemyParam = null;
        internal static AssetLocator AssetLocator = null;

        private static Dictionary<string, Param> _params = null;
        private static Dictionary<string, Param> _vanillaParams = null;
        private static Dictionary<string, PARAMDEF> _paramdefs = null;
        private static Dictionary<string, HashSet<int>> _paramDirtyCache = null; //If param != vanillaparam

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

        //DS2 Only
        private static PARAM GetParam(BND4 parambnd, string paramfile)
        {
            var bndfile = parambnd.Files.Find(x => Path.GetFileName(x.Name) == paramfile);
            if (bndfile != null)
            {
                return PARAM.Read(bndfile.Bytes);
            }

            // Otherwise the param is a loose param
            if (File.Exists($@"{AssetLocator.GameModDirectory}\Param\{paramfile}"))
            {
                return PARAM.Read($@"{AssetLocator.GameModDirectory}\Param\{paramfile}");
            }
            if (File.Exists($@"{AssetLocator.GameRootDirectory}\Param\{paramfile}"))
            {
                return PARAM.Read($@"{AssetLocator.GameRootDirectory}\Param\{paramfile}");
            }
            return null;
        }

        private static List<(string, PARAMDEF)> LoadParamdefs()
        {
            _paramdefs = new Dictionary<string, PARAMDEF>();
            var dir = AssetLocator.GetParamdefDir();
            var files = Directory.GetFiles(dir, "*.xml");
            List<(string, PARAMDEF)> defPairs = new List<(string, PARAMDEF)>();
            foreach (var f in files)
            {
                var pdef = PARAMDEF.XmlDeserialize(f);
                _paramdefs.Add(pdef.ParamType, pdef);
                defPairs.Add((f, pdef));
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
            while (IsLoadingParams); //super hack
                Thread.Sleep(100);
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

        private static void LoadParamFromBinder(IBinder parambnd, ref Dictionary<string, FSParam.Param> paramBank)
        {
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
                if (!_paramdefs.ContainsKey(p.ParamType))
                {
                    continue;
                }
                p.ApplyParamdef(_paramdefs[p.ParamType]);
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

            LoadParamFromBinder(paramBnd, ref _params);
            return dir;
        }
        private static void LoadVParamsDES(string dir)
        {
            string paramBinderName = "gameparam.parambnd.dcx";
            if (Directory.GetParent(dir).Parent.FullName.Contains("BLUS"))
            {
                paramBinderName = "gameparamna.parambnd.dcx";
            }
            LoadParamFromBinder(BND3.Read($@"{dir}\param\gameparam\{paramBinderName}"), ref _vanillaParams);
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

            LoadParamFromBinder(paramBnd, ref _params);
            return dir;
        }
        private static void LoadVParamsDS1(string dir)
        {
            LoadParamFromBinder(BND3.Read($@"{dir}\param\GameParam\GameParam.parambnd"), ref _vanillaParams);
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

            LoadParamFromBinder(paramBnd, ref _params);
            return dir;
        }
        private static void LoadVParamsDS1R(string dir)
        {
            LoadParamFromBinder(BND3.Read($@"{dir}\param\GameParam\GameParam.parambnd.dcx"), ref _vanillaParams);
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

            LoadParamFromBinder(paramBnd, ref _params);
            return dir;
        }
        private static void LoadVParamsBBSekrio(string dir)
        {
            LoadParamFromBinder(BND4.Read($@"{dir}\param\gameparam\gameparam.parambnd.dcx"), ref _vanillaParams);
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

            LoadParamFromBinder(paramBnd, ref _params);
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
            LoadParamFromBinder(vParamBnd, ref _vanillaParams);
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

                LoadParamFromBinder(lparamBnd, ref _params);
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
                LoadParamFromBinder(paramBnd, ref _params);
            }
            return vparam;
        }
        private static void LoadVParamsDS3(string vparam)
        {
            BND4 vParamBnd = SFUtil.DecryptDS3Regulation(vparam);
            LoadParamFromBinder(vParamBnd, ref _vanillaParams);
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

            LoadParamFromBinder(paramBnd, ref _params);

            param = $@"{mod}\regulation.bin";
            if (partial && File.Exists(param))
            {
                BND4 pParamBnd = SFUtil.DecryptERRegulation(param);
                Dictionary<string, Param> cParamBank = new Dictionary<string, Param>();
                LoadParamFromBinder(pParamBnd, ref cParamBank);
                foreach (var pair in cParamBank)
                {
                    Param baseParam = _params[pair.Key];
                    foreach (var row in pair.Value.Rows)
                    {
                        Param.Row bRow = baseParam[row.ID];
                        if (bRow == null)
                        {
                            baseParam.Rows.Add(row);
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
            LoadParamFromBinder(SFUtil.DecryptERRegulation($@"{dir}\regulation.bin"), ref _vanillaParams);
        }

        //Some returns and repetition, but it keeps all threading and loading-flags visible inside this method
        public static void ReloadParams(ProjectSettings settings)
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
                Param vp = _vanillaParams[param];
                foreach (Param.Row row in _params[param].Rows.ToList())
                {
                    refreshParamRowDirtyCache(row, vp, cache);
                }
            }
            _paramDirtyCache = newCache;
        }
        public static void refreshParamRowDirtyCache(Param.Row row, Param vanillaParam, HashSet<int> cache)
        {
            Param.Row vrow = vanillaParam[row.ID];
            if (IsChanged(row, vanillaParam))
                cache.Add(row.ID);
            else
                cache.Remove(row.ID);
        }
        private static bool IsChanged(Param.Row row, Param vanilla)
        {
            List<Param.Row> vanils = vanilla.Rows.Where(cell => cell.ID == row.ID).ToList();
            if (vanils.Count == 0)
            {
                return true;
            }
            foreach (Param.Row vrow in vanils)
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
            if (!File.Exists(param))
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
                    List<Param.Row> backup = paramFile.Rows;
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

        public static string GetChrIDForEnemy(long enemyID)
        {
            if (EnemyParam != null)
            {
                if (EnemyParam[(int)enemyID] != null)
                {
                    return $@"{EnemyParam[(int)enemyID]["Chr ID"].Value:D4}";
                }
            }
            return null;
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
