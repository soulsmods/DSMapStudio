using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudioCore.Assetdex
{
    /// <summary>
    /// Class <c>AssetdexCore</c> contains the <c>AssetReference</c> dictionaries that host the documentation for each asset.
    /// </summary>
    public class AssetdexCore
    {
        private Dictionary<GameType, AssetdexResource> resourceDict = new Dictionary<GameType, AssetdexResource>();

        private Dictionary<string, AssetReference> chrRefs = new Dictionary<string, AssetReference>();
        private Dictionary<string, AssetReference> objRefs = new Dictionary<string, AssetReference>();
        private Dictionary<string, AssetReference> partRefs = new Dictionary<string, AssetReference>();
        private Dictionary<string, AssetReference> mapPieceRefs = new Dictionary<string, AssetReference>();

        private AssetLocator _locator;

        public AssetdexCore(AssetLocator locator)
        {
            _locator = locator;

            resourceDict.Add(GameType.DemonsSouls, LoadAssetdexJSON("DES"));
            resourceDict.Add(GameType.DarkSoulsPTDE, LoadAssetdexJSON("DS1"));
            resourceDict.Add(GameType.DarkSoulsRemastered, LoadAssetdexJSON("DS1R"));
            resourceDict.Add(GameType.DarkSoulsIISOTFS, LoadAssetdexJSON("DS2"));
            resourceDict.Add(GameType.Bloodborne, LoadAssetdexJSON("BB"));
            resourceDict.Add(GameType.DarkSoulsIII, LoadAssetdexJSON("DS3"));
            resourceDict.Add(GameType.EldenRing, LoadAssetdexJSON("ER"));
            resourceDict.Add(GameType.Sekiro, LoadAssetdexJSON("SDT"));
            resourceDict.Add(GameType.ArmoredCoreVI, LoadAssetdexJSON("AC6"));
        }

        /// <summary>
        /// Load a supported <c>GameType</c> JSON file.
        /// </summary>
        /// <returns>An AssetdexResource object with the serialized documentation.</returns>
        private AssetdexResource LoadAssetdexJSON(string gametype)
        {
            AssetdexResource resource = new AssetdexResource();

            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\Assetdex_{gametype}.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                resource = JsonSerializer.Deserialize<AssetdexResource>(File.OpenRead(json_filepath), options);
            }

            return resource;
        }

        /// <summary>
        /// Update the Assetdex when the project has changed.
        /// </summary>
        public void OnProjectChanged()
        {
            UpdateAssetReferences(_locator.Type);
        }

        /// <summary>
        /// Update the <c>AssetReference</c> dictionaries to use serialized documentation suitable for the currently loaded <c>GameType</c>.
        /// </summary>
        private void UpdateAssetReferences(GameType type)
        {
            GameReference game = resourceDict[type].GameReference[0];

            chrRefs.Clear();
            objRefs.Clear();
            partRefs.Clear();
            mapPieceRefs.Clear();

            foreach (AssetReference entry in game.chrList)
            {
                if (!chrRefs.ContainsKey(entry.id))
                    chrRefs.Add(entry.id, entry);
            }
            foreach (AssetReference entry in game.objList)
            {
                if (!objRefs.ContainsKey(entry.id))
                    objRefs.Add(entry.id, entry);
            }
            foreach (AssetReference entry in game.partList)
            {
                if (!partRefs.ContainsKey(entry.id))
                    partRefs.Add(entry.id, entry);
            }
            foreach (AssetReference entry in game.mapPieceList)
            {
                if (!mapPieceRefs.ContainsKey(entry.id))
                    mapPieceRefs.Add(entry.id, entry);
            }
        }

        /// <returns>
        /// The <c>AssetReference</c> dictionary for Chrs.
        /// </returns>
        public Dictionary<string, AssetReference> GetChrReferences()
        {
            return chrRefs;
        }

        /// <returns>
        /// The <c>AssetReference</c> dictionary for Obj/AEGs.
        /// </returns>
        public Dictionary<string, AssetReference> GetObjReferences()
        {
            return objRefs;
        }

        /// <returns>
        /// The <c>AssetReference</c> dictionary for Parts.
        /// </returns>
        public Dictionary<string, AssetReference> GetPartReferences()
        {
            return partRefs;
        }

        /// <returns>
        /// The <c>AssetReference</c> dictionary for Map Pieces.
        /// </returns>
        public Dictionary<string, AssetReference> GetMapPieceReferences()
        {
            return mapPieceRefs;
        }
    }
}
