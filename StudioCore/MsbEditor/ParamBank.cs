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

        private static void LoadParamsDS2()
        {
            var dir = AssetLocator.GameRootDirectory;
            if (!File.Exists($@"{dir}\enc_regulation.bnd.dcx"))
            {
                MessageBox.Show("Could not find DS2 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!BND4.Is($@"{dir}\enc_regulation.bnd.dcx"))
            {
                MessageBox.Show("Use yapped to decrypt your DS2 regulation file. Functionality will be limited.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Load params
            BND4 paramBnd = BND4.Read($@"{dir}\enc_regulation.bnd.dcx");
            EnemyParam = PARAM.Read(paramBnd.Files.Find(x => Path.GetFileName(x.Name) == "EnemyParam.param").Bytes);
            PARAM.Layout layout = PARAM.Layout.ReadXMLFile($@"Assets\ParamLayouts\DS2SOTFS\{EnemyParam.ID}.xml");
            EnemyParam.SetLayout(layout);
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
                    return $@"{EnemyParam[(int)enemyID]["ChrID"].Value:D4}";
                }
            }
            return null;
        }
    }
}
