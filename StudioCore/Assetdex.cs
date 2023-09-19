using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudioCore
{
    public class GameReference
    {
        public string gameType { get; set; }
        public List<ChrReference> chrReferences { get; set; }
        public List<ObjReference> objReferences { get; set; }
        public List<PartReference> partReferences { get; set; }
        public List<MapPieceReference> mapPieceReferences { get; set; }
    }
    public class ChrReference
    {
        public string fileName { get; set; }
        public string referenceName { get; set; }
        public List<string> tags { get; set; }
    }

    public class ObjReference
    {
        public string fileName { get; set; }
        public string referenceName { get; set; }
        public List<string> tags { get; set; }
    }

    public class PartReference
    {
        public string fileName { get; set; }
        public string referenceName { get; set; }
        public List<string> tags { get; set; }
    }

    public class MapPieceReference
    {
        public string fileName { get; set; }
        public string referenceName { get; set; }
        public List<string> tags { get; set; }
    }

    // Not ideal, but the static requirement for deserialization makes this the simplest method to support individual assetdex files for each game.
    public class Assetdex_AC6
    {
        public List<GameReference> GameReference { get; set; }

        public static Assetdex_AC6 Static { get; }

        static Assetdex_AC6()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\AC6\\Assetdex.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<Assetdex_AC6>(File.OpenRead(json_filepath), options);
            }
        }
    }
    public class Assetdex_BB
    {
        public List<GameReference> GameReference { get; set; }

        public static Assetdex_BB Static { get; }

        static Assetdex_BB()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\BB\\Assetdex.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<Assetdex_BB>(File.OpenRead(json_filepath), options);
            }
        }
    }
    public class Assetdex_DES
    {
        public List<GameReference> GameReference { get; set; }

        public static Assetdex_DES Static { get; }

        static Assetdex_DES()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\DES\\Assetdex.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<Assetdex_DES>(File.OpenRead(json_filepath), options);
            }
        }
    }
    public class Assetdex_DS1
    {
        public List<GameReference> GameReference { get; set; }

        public static Assetdex_DS1 Static { get; }

        static Assetdex_DS1()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\DS1\\Assetdex.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<Assetdex_DS1>(File.OpenRead(json_filepath), options);
            }
        }
    }
    public class Assetdex_DS1R
    {
        public List<GameReference> GameReference { get; set; }

        public static Assetdex_DS1R Static { get; }

        static Assetdex_DS1R()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\DS1R\\Assetdex.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<Assetdex_DS1R>(File.OpenRead(json_filepath), options);
            }
        }
    }
    public class Assetdex_DS2S2
    {
        public List<GameReference> GameReference { get; set; }

        public static Assetdex_DS2S2 Static { get; }

        static Assetdex_DS2S2()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\DS2S2\\Assetdex.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<Assetdex_DS2S2>(File.OpenRead(json_filepath), options);
            }
        }
    }
    public class Assetdex_DS3
    {
        public List<GameReference> GameReference { get; set; }

        public static Assetdex_DS3 Static { get; }

        static Assetdex_DS3()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\DS3\\Assetdex.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<Assetdex_DS3>(File.OpenRead(json_filepath), options);
            }
        }
    }
    public class Assetdex_ER
    {
        public List<GameReference> GameReference { get; set; }

        public static Assetdex_ER Static { get; }

        static Assetdex_ER()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\ER\\Assetdex.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<Assetdex_ER>(File.OpenRead(json_filepath), options);
            }
        }
    }
    public class Assetdex_SDT
    {
        public List<GameReference> GameReference { get; set; }

        public static Assetdex_SDT Static { get; }

        static Assetdex_SDT()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\SDT\\Assetdex.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<Assetdex_SDT>(File.OpenRead(json_filepath), options);
            }
        }
    }

    public class AssetdexUtils
    {
        public static GameReference GetCurrentGameAssetdex(GameType type)
        {
            switch (type)
            {
                case GameType.DemonsSouls:
                    return Assetdex_DES.Static.GameReference[0];
                case GameType.DarkSoulsPTDE:
                    return Assetdex_DS1.Static.GameReference[0];
                case GameType.DarkSoulsRemastered:
                    return Assetdex_DS1R.Static.GameReference[0];
                case GameType.DarkSoulsIISOTFS:
                    return Assetdex_DS2S2.Static.GameReference[0];
                case GameType.Bloodborne:
                    return Assetdex_BB.Static.GameReference[0];
                case GameType.DarkSoulsIII:
                    return Assetdex_DS3.Static.GameReference[0];
                case GameType.Sekiro:
                    return Assetdex_SDT.Static.GameReference[0];
                case GameType.EldenRing:
                    return Assetdex_ER.Static.GameReference[0];
                case GameType.ArmoredCoreVI:
                    return Assetdex_AC6.Static.GameReference[0];
                default:
                    throw new Exception("Game type not set");
            }
        }

        public static bool MatchSearchInput(string inputStr, string fileName, string referenceName, List<string> tags)
        {
            bool match = false;

            string curInput = inputStr.Trim();

            if (curInput.Equals(""))
            {
                match = true; // If input is empty, show all
                return match;
            }

            // Match: Filename
            if (curInput == fileName)
                match = true;

            // Match: Reference Name
            if (curInput == referenceName)
                match = true;

            // Match: Reference Segments
            string[] refSegments = referenceName.Split(" ");
            foreach (string refStr in refSegments)
            {
                if (curInput == refStr.Trim())
                    match = true;
            }

            // Match: Tags
            foreach (string tagStr in tags)
            {
                if (curInput == tagStr)
                    match = true;
            }

            return match;
        }
    }
}
