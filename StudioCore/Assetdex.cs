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
        public List<Tag> tags { get; set; }
    }

    public class ObjReference
    {
        public string fileName { get; set; }
        public string referenceName { get; set; }
        public List<Tag> tags { get; set; }
    }

    public class PartReference
    {
        public string fileName { get; set; }
        public string referenceName { get; set; }
        public List<Tag> tags { get; set; }
    }

    public class MapPieceReference
    {
        public string fileName { get; set; }
        public string referenceName { get; set; }
        public List<Tag> tags { get; set; }
    }

    public class Tag
    {
        public string tag { get; set; }
    }

    public class Assetdex
    {
        public List<GameReference> GameReference { get; set; }

        public static Assetdex Static { get; }

        static Assetdex()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\Assetdex\\Assetdex.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<Assetdex>(File.OpenRead(json_filepath), options);
            }
        }
    }
}
