using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SoulsFormats;
using System.Linq;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Utilities for dealing with global params for a game
    /// </summary>
    public class ParamBank
    {
        private static PARAM EnemyParam = null;
        private static AssetLocator AssetLocator = null;

        private static Dictionary<string, PARAM> _params = null;
        private static Dictionary<string, PARAMDEF> _paramdefs = null;

        public static IReadOnlyDictionary<string, PARAM> Params
        {
            get
            {
                return _params;
            }
        }

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

        private static void LoadParamdefs()
        {
            _paramdefs = new Dictionary<string, PARAMDEF>();
            var dir = AssetLocator.GetParamdefDir();
            var files = Directory.GetFiles(dir, "*.xml");
            var mdir = AssetLocator.GetParammetaDir();
            foreach (var f in files)
            {
                var fName = f.Substring(f.LastIndexOf('\\')+1);
                var pdef = PARAMDEF.XmlDeserialize(f, $@"{mdir}\{fName}");
                _paramdefs.Add(pdef.ParamType, pdef);
            }
        }

        private static void LoadParamFromBinder(IBinder parambnd)
        {
            // Load every param in the regulation
            //_params = new Dictionary<string, PARAM>();
            foreach (var f in parambnd.Files)
            {
                if (!f.Name.ToUpper().EndsWith(".PARAM") || Path.GetFileNameWithoutExtension(f.Name).StartsWith("default_"))
                {
                    continue;
                }
                if (f.Name.EndsWith("LoadBalancerParam.param"))
                {
                    continue;
                }
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(f.Name)))
                {
                    continue;
                }
                PARAM p = PARAM.Read(f.Bytes);
                p.ApplyParamdef(_paramdefs[p.ParamType]);
                _params.Add(Path.GetFileNameWithoutExtension(f.Name), p);
            }
        }

        private static void LoadParamsDES()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\gameparam\gameparam.parambnd.dcx"))
            {
                MessageBox.Show("Could not find DES regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load params
            var param = $@"{mod}\param\gameparam\gameparam.parambnd.dcx";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\gameparam\gameparam.parambnd.dcx";
            }
            BND3 paramBnd = BND3.Read(param);

            LoadParamFromBinder(paramBnd);
        }

        private static void LoadParamsDS1()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\GameParam\GameParam.parambnd"))
            {
                MessageBox.Show("Could not find DS1 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load params
            var param = $@"{mod}\param\GameParam\GameParam.parambnd";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\GameParam\GameParam.parambnd";
            }
            BND3 paramBnd = BND3.Read(param);

            LoadParamFromBinder(paramBnd);
        }

        private static void LoadParamsBBSekrio()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\\param\gameparam\gameparam.parambnd.dcx"))
            {
                MessageBox.Show("Could not find param file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load params
            var param = $@"{mod}\param\gameparam\gameparam.parambnd.dcx";
            if (!File.Exists(param))
            {
                param = $@"{dir}\param\gameparam\gameparam.parambnd.dcx";
            }
            BND4 paramBnd = BND4.Read(param);

            LoadParamFromBinder(paramBnd);
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
            "generatorlocation",
            "generatorparam",
            "generatorregistparam",
            "hitgroupparam",
            "intrudepointparam",
            "mapobjectinstanceparam",
            "maptargetdirparam",
            "npctalkparam",
            "treasureboxparam",
        };

        private static void LoadParamsDS2()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\enc_regulation.bnd.dcx"))
            {
                MessageBox.Show("Could not find DS2 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!BND4.Is($@"{dir}\enc_regulation.bnd.dcx"))
            {
                MessageBox.Show("Use yapped to decrypt your DS2 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Keep track of loaded params as we load loose and regulation params
            HashSet<string> loadedParams = new HashSet<string>();

            // Load params
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

                    var lp = PARAM.Read(p);
                    var fname = lp.ParamType;
                    PARAMDEF def = AssetLocator.GetParamdefForParam(fname);
                    lp.ApplyParamdef(def);
                    if (!_params.ContainsKey(name))
                    {
                        _params.Add(name, lp);
                    }
                }
            }

            // Load params
            var param = $@"{mod}\enc_regulation.bnd.dcx";
            if (!File.Exists(param))
            {
                param = $@"{dir}\enc_regulation.bnd.dcx";
            }
            BND4 paramBnd = BND4.Read(param);
            EnemyParam = GetParam(paramBnd, "EnemyParam.param");
            if (EnemyParam != null)
            {
                PARAMDEF def = AssetLocator.GetParamdefForParam(EnemyParam.ParamType);
                EnemyParam.ApplyParamdef(def);
            }

            LoadParamFromBinder(paramBnd);
        }

        private static void LoadParamsDS3()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\Data0.bdt"))
            {
                MessageBox.Show("Could not find DS3 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load loose params if they exist
            if (File.Exists($@"{mod}\\param\gameparam\gameparam_dlc2.parambnd.dcx"))
            {
                // Load params
                var lparam = $@"{mod}\param\gameparam\gameparam_dlc2.parambnd.dcx";
                BND4 lparamBnd = BND4.Read(lparam);

                LoadParamFromBinder(lparamBnd);
                return;
            }

            // Load params
            var param = $@"{mod}\Data0.bdt";
            if (!File.Exists(param))
            {
                param = $@"{dir}\Data0.bdt";
            }
            BND4 paramBnd = SFUtil.DecryptDS3Regulation(param);
            LoadParamFromBinder(paramBnd);
        }

        public static void ReloadParams()
        {
            if (AssetLocator.Type != GameType.Undefined)
            {
                LoadParamdefs();
            }

            _params = new Dictionary<string, PARAM>();
            if (AssetLocator.Type == GameType.DemonsSouls)
            {
                LoadParamsDES();
            }
            if (AssetLocator.Type == GameType.DarkSoulsPTDE)
            {
                LoadParamsDS1();
            }
            if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                LoadParamsDS2();
            }
            if (AssetLocator.Type == GameType.DarkSoulsIII)
            {
                LoadParamsDS3();
            }
            if (AssetLocator.Type == GameType.Bloodborne || AssetLocator.Type == GameType.Sekiro)
            {
                LoadParamsBBSekrio();
            }
        }

        public static void LoadParams(AssetLocator l)
        {
            AssetLocator = l;
            ReloadParams();
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
            Utils.WriteWithBackup(dir, null, @"param\GameParam\GameParam.parambnd", paramBnd);
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
            if (!File.Exists(param))
            {
                param = $@"{dir}\enc_regulation.bnd.dcx";
            }
            BND4 paramBnd = BND4.Read(param);

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
                Utils.WriteWithBackup(dir, mod, @"Data0.bdt", paramBnd, true);
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

                BND4 stayBND = new BND4
                {
                    BigEndian = false,
                    Compression = DCX.Type.DCX_DFLT_10000_44_9,
                    Extended = 0x04,
                    Unk04 = false,
                    Unk05 = false,
                    Format = Binder.Format.Compression | Binder.Format.Flag6 | Binder.Format.LongOffsets | Binder.Format.Names1,
                    Unicode = true,
                    Files = paramBnd.Files.Where(f => f.Name.EndsWith(".stayparam")).ToList()
                };

                Utils.WriteWithBackup(dir, mod, @"param\gameparam\gameparam_dlc2.parambnd.dcx", paramBND);
                Utils.WriteWithBackup(dir, mod, @"param\gameparam\stayparam.parambnd.dcx", stayBND);
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

        public static void SaveParams(bool loose=false)
        {
            if (_params == null)
            {
                return;
            }
            if (AssetLocator.Type == GameType.DarkSoulsPTDE)
            {
                SaveParamsDS1();
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
    }
}
