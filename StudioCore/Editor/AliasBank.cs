using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SoulsFormats;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using StudioCore.Editor;

namespace StudioCore.Editor
{
    /// <summary>
    /// Utilities for dealing with global params for a game
    /// </summary>
    public class AliasBank
    {
        private static AssetLocator AssetLocator = null;

        private static Dictionary<string, string> _mapNames = null;
        public static bool IsLoadingAliases { get; private set; } = false;

        public static IReadOnlyDictionary<string, string> MapNames
        {
            get
            {
                if (IsLoadingAliases)
                {
                    return null;
                }
                return _mapNames;
            }
        }

        private static void LoadMapNames()
        {
            try {
                var dir = AssetLocator.GetAliasAssetsDir();
                string [] mapNames = File.ReadAllLines(dir+"/MapNames.txt");
                foreach (string pair in mapNames)
                {
                    string[] parts = pair.Split(' ', 2);
                    _mapNames[parts[0]] = parts[1];
                }
            }
            catch (Exception e)
            {
                //should log the error really. Or just fill in the missing alias files
            }
        }
        public static void ReloadAliases()
        {
            TaskManager.Run("AB:LoadAliases", true, false, false, () =>
            {
                _mapNames = new Dictionary<string, string>();
                IsLoadingAliases = true;
                if (AssetLocator.Type != GameType.Undefined)
                {
                    LoadMapNames();
                }
                IsLoadingAliases = false;
            });
        }

        public static void SetAssetLocator(AssetLocator l)
        {
            AssetLocator = l;
        }
    }
}
