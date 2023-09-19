using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudioCore.HelpMenu
{
    public class ResourceEntry
    {
        public string Title { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Contents { get; set; }
    }

    public class HelpMenuResource
    {
        public List<ResourceEntry> Entries { get; set; }

        public static HelpMenuResource Static { get; }

        static HelpMenuResource()
        {

        string json_filepath = AppContext.BaseDirectory + $"\\Assets\\HelpMenu\\Core.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<HelpMenuResource>(File.OpenRead(json_filepath), options);
            }
        }
    }

    public class HelpTutorialResource
    {
        public List<ResourceEntry> Entries { get; set; }

        public static HelpTutorialResource Static { get; }

        static HelpTutorialResource()
        {

            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\HelpMenu\\Tutorial.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<HelpTutorialResource>(File.OpenRead(json_filepath), options);
            }
        }
    }

    public class CreditMenuResource
    {
        public List<string> Text { get; set; }

        public static CreditMenuResource Static { get; }

        static CreditMenuResource()
        {
            string json_filepath = AppContext.BaseDirectory + $"\\Assets\\HelpMenu\\Credits.json";

            if (File.Exists(json_filepath))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                Static = JsonSerializer.Deserialize<CreditMenuResource>(File.OpenRead(json_filepath), options);
            }
        }
    }
}
