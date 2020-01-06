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

        private static void LoadParamsDS2()
        {
            var dir = AssetLocator.GameRootDirectory;
            var mod = AssetLocator.GameModDirectory;
            if (!File.Exists($@"{dir}\enc_regulation.bnd.dcx"))
            {
                MessageBox.Show("Could not find DS2 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!BND4.Is($@"{dir}\enc_regulation.bnd.dcx"))
            {
                MessageBox.Show("Use yapped to decrypt your DS2 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                PARAM.Layout layout = PARAM.Layout.ReadXMLFile($@"Assets\ParamLayouts\DS2SOTFS\{EnemyParam.ID}.xml");
                EnemyParam.SetLayout(layout);
            }
        }

        public static void ReloadParams()
        {
            if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                LoadParamsDS2();
            }
        }

        public static void LoadParams(AssetLocator l)
        {
            AssetLocator = l;
            ReloadParams();
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
