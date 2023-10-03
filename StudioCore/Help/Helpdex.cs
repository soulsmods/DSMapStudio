using StudioCore.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudioCore.Help
{
    public class Helpdex
    {
        private List<HelpEntry> helpArticles = new List<HelpEntry>();
        private List<HelpEntry> helpTutorials = new List<HelpEntry>();
        private List<HelpEntry> helpGlossary = new List<HelpEntry>();
        private LinkEntries links = new LinkEntries();
        private TextEntry credits = new TextEntry();

        public Helpdex()
        {
            helpArticles = LoadHelpResource("Articles");
            helpTutorials = LoadHelpResource("Tutorials");
            helpGlossary = LoadHelpResource("Glossary");
            links = JsonSerializer.Deserialize<LinkEntries>(File.OpenRead(AppContext.BaseDirectory + $"\\Assets\\Help\\Links.json"));
            credits = JsonSerializer.Deserialize<TextEntry>(File.OpenRead(AppContext.BaseDirectory + $"\\Assets\\Help\\Credits.json"));
        }

        private List<HelpEntry> LoadHelpResource(string directory)
        {
            List<HelpEntry> helpEntries = new List<HelpEntry>();

            var articleDirFiles = from file in Directory.EnumerateFiles(AppContext.BaseDirectory + $"\\Assets\\Help\\{directory}\\") select file;
            foreach (var file in articleDirFiles)
            {
                helpEntries.Add(LoadHelpJSON(file));
            }

            return helpEntries;
        }

        private HelpEntry LoadHelpJSON(string path)
        {
            HelpEntry resource = new HelpEntry();

            if (File.Exists(path))
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };
                resource = JsonSerializer.Deserialize<HelpEntry>(File.OpenRead(path), options);
            }

            return resource;
        }

        public List<HelpEntry> GetArticles()
        {
            return helpArticles;
        }

        public List<HelpEntry> GetTutorials()
        {
            return helpTutorials;
        }

        public List<HelpEntry> GetGlossaryEntries()
        {
            return helpGlossary;
        }

        public List<LinkEntry> GetLinks()
        {
            return links.Links;
        }

        public TextEntry GetCredits()
        {
            return credits;
        }
    }
}
