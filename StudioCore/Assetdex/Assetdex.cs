using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudioCore.Assetdex
{
    public class Assetdex
    {
        public Dictionary<GameType, AssetdexResource> resourceDict = new Dictionary<GameType, AssetdexResource>();

        public Assetdex()
        {
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

        public AssetdexResource LoadAssetdexJSON(string gametype)
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
    }
}
