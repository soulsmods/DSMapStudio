using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Newtonsoft.Json.Linq;

namespace StudioCore.Editor
{
    /// <summary>
    /// Settings for a modding project. Gets serialized to JSON
    /// </summary>
    public class ProjectSettings
    {
        public string ProjectName { get; set; } = "";
        public string GameRoot { get; set; } = "";
        public GameType GameType { get; set; } = GameType.Undefined;

        // JsonExtensionData stores info in config file not present in class in order to retain settings between versions.
#pragma warning disable IDE0051
        [JsonExtensionData]
        private IDictionary<string, JToken> _additionalData;
#pragma warning restore IDE0051

        // Params
        public List<string> PinnedParams { get; set; } = new List<string>();
        public Dictionary<string, List<int>> PinnedRows { get; set; } = new Dictionary<string, List<int>>();
        public Dictionary<string, List<string>> PinnedFields { get; set; } = new Dictionary<string, List<string>>();

        /// <summary>
        /// Has different meanings depending on the game, but for supported games
        /// (DS2 and DS3) this means that params are written as "loose" i.e. outside
        /// the regulation file.
        /// </summary>
        public bool UseLooseParams { get; set; } = false;
        public bool PartialParams { get; set; } = false;

        // FMG editor
        public string LastFmgLanguageUsed { get; set; } = "";

        public void Serialize(string path)
        {
            var jsonString = JsonSerializer.SerializeToUtf8Bytes(this);
            File.WriteAllBytes(path, jsonString);
        }

        public static ProjectSettings Deserialize(string path)
        {
            var jsonString = File.ReadAllBytes(path);
            var readOnlySpan = new ReadOnlySpan<byte>(jsonString);
            return JsonSerializer.Deserialize<ProjectSettings>(readOnlySpan);
        }
    }

    public class NewProjectOptions
    {
        public Editor.ProjectSettings settings;
        public string directory = "";
        public bool loadDefaultNames = true;
    }
}
