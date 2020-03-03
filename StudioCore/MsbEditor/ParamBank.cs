using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SoulsFormats;

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
                var fname = p.ParamType;
                if (AssetLocator.Type == GameType.DarkSoulsPTDE || AssetLocator.Type == GameType.DarkSoulsRemastered)
                {
                    fname = Path.GetFileNameWithoutExtension(f.Name);
                    if (fname == "BehaviorParam_PC")
                    {
                        fname = "BehaviorParam";
                    }
                    if (fname == "AtkParam_Pc")
                    {
                        fname = "AtkParam";
                    }
                    if (fname == "AtkParam_Npc")
                    {
                        fname = "AtkParam";
                    }
                    if (fname == "Magic")
                    {
                        fname = "MagicParam";
                    }
                    if (fname == "Bullet")
                    {
                        fname = "BulletParam";
                    }
                    if (fname == "SpEffectParam")
                    {
                        fname = "SpEffect";
                    }
                    if (fname == "SpEffectVfxParam")
                    {
                        fname = "SpEffectVfx";
                    }
                    if (fname == "MenuColorTableParam")
                    {
                        fname = "MenuParamColorTable";
                    }
                    if (fname == "QwcChange")
                    {
                        fname = "QwcChangeParam";
                    }
                    if (fname == "QwcJudge")
                    {
                        fname = "QwcJudgeParam";
                    }
                }
                PARAMDEF def = AssetLocator.GetParamdefForParam(fname);
                p.ApplyParamdef(def);
                _params.Add(Path.GetFileNameWithoutExtension(f.Name), p);
            }
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
            if (mod != null)
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
            _params = new Dictionary<string, PARAM>();
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
            Utils.WriteWithBackup(dir, mod, @"param\GameParam\GameParam.parambnd", paramBnd);
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
