using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Settings for a modding project. Gets serialized to JSON
    /// </summary>
    public class ProjectSettings
    {
        public string ProjectName { get; set; } = "";
        public string GameRoot { get; set; } = "";
        public GameType GameType { get; set; } = GameType.Undefined;

        /// <summary>
        /// Has different meanings depending on the game, but for supported games
        /// (DS2 and DS3) this means that params are written as "loose" i.e. outside
        /// the regulation file.
        /// </summary>
        public bool UseLooseParams { get; set; } = false;

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
}
