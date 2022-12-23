using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudioCore
{
    public class CFG
    {
        public static bool IsEnabled = true;

        private static object _lock_SaveLoadCFG = new();

        public const string FolderName = "DSMapStudio";
        public const string Config_FileName = "DSMapStudio_Config.json";
        public const string Keybinds_FileName = "DSMapStudio_Keybinds.json";
        public static CFG Current { get; private set; } = null;
        public static CFG Default { get; private set; } = new();

        // JsonExtensionData stores info in config file not present in class in order to retain settings between versions.
#pragma warning disable IDE0051
        [JsonExtensionData]
        private IDictionary<string, JToken> _additionalData;
#pragma warning restore IDE0051

        public const int MAX_RECENT_PROJECTS = 10;

        public static string GetConfigFilePath()
        {
            return $@"{GetConfigFolderPath()}\{Config_FileName}";
        }
        public static string GetBindingsFilePath()
        {
            return $@"{GetConfigFolderPath()}\{Keybinds_FileName}";
        }
        public static string GetConfigFolderPath()
        {
            return $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{FolderName}";
        }
        private static void LoadConfig()
        {
            if (!File.Exists(GetConfigFilePath()))
            {
                Current = new CFG();
                SaveConfig();
            }
            else
            {
                do
                {
                    try
                    {
                        Current = JsonConvert.DeserializeObject<CFG>(
                        File.ReadAllText(GetConfigFilePath()));
                    }
                    catch (JsonException e)
                    {
                        if (MessageBox.Show($"{e.Message}\n\nReset config settings?", $"{Config_FileName} Load Error",
                            MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            Current = new CFG();
                        }
                    }
                }
                while (Current == null);
            }
        }

        private static void LoadKeybinds()
        {
            if (!File.Exists(GetBindingsFilePath()))
            {
                KeyBindings.Current = new KeyBindings.Bindings();
                SaveKeybinds();
            }
            else
            {
                do
                {
                    try
                    {
                        KeyBindings.Current = JsonConvert.DeserializeObject<KeyBindings.Bindings>(
                        File.ReadAllText(GetBindingsFilePath()));
                    }
                    catch (JsonException e)
                    {
                        if (MessageBox.Show($"{e.Message}\n\nReset keybinds?", $"{Keybinds_FileName} Load Error",
                            MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            KeyBindings.Current = new KeyBindings.Bindings();
                        }
                    }
                }
                while (KeyBindings.Current == null);
            }
        }

        private static void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(
                Current, Formatting.Indented);
            File.WriteAllText(GetConfigFilePath(), json);
        }

        private static void SaveKeybinds()
        {
            var json = JsonConvert.SerializeObject(
                KeyBindings.Current, Formatting.Indented);
            File.WriteAllText(GetBindingsFilePath(), json);
        }

        public static void Save()
        {
            if (IsEnabled)
            {
                lock (_lock_SaveLoadCFG)
                {
                    if (!Directory.Exists(GetConfigFolderPath()))
                        Directory.CreateDirectory(GetConfigFolderPath());

                    SaveConfig();
                    SaveKeybinds();
                }
            }
        }

        public static void AttemptLoadOrDefault()
        {
            if (IsEnabled)
            {
                lock (_lock_SaveLoadCFG)
                {
                    if (!Directory.Exists(GetConfigFolderPath()))
                        Directory.CreateDirectory(GetConfigFolderPath());
                    LoadConfig();
                    LoadKeybinds();
                }
            }
        }

        public class RecentProject
        {
            // JsonExtensionData stores info in config file not present in class in order to retain settings between versions.
#pragma warning disable IDE0051
            [JsonExtensionData]
            private IDictionary<string, JToken> _additionalData;
#pragma warning restore IDE0051

            public string Name;
            public string ProjectFile;
            public GameType GameType;
        }

        public string LastProjectFile { get; set; } = "";
        public List<RecentProject> RecentProjects { get; set; } = new List<RecentProject>();

        public GameType Game_Type { get; set; } = GameType.Undefined;

        public Scene.RenderFilter LastSceneFilter = Scene.RenderFilter.All ^ Scene.RenderFilter.Light;

        public class RenderFilterPreset
        {
            public string Name;
            public Scene.RenderFilter Filters;
            public RenderFilterPreset(string name, Scene.RenderFilter filters)
            {
                Name = name;
                Filters = filters;
            }
        }

        public RenderFilterPreset SceneFilter_Preset_01 = new("Map", Scene.RenderFilter.MapPiece | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region);
        public RenderFilterPreset SceneFilter_Preset_02 = new("Collision", Scene.RenderFilter.Collision | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region);
        public RenderFilterPreset SceneFilter_Preset_03 = new("Collision & Navmesh", Scene.RenderFilter.Collision | Scene.RenderFilter.Navmesh | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region);
        public RenderFilterPreset SceneFilter_Preset_04 = new("Lighting (Map)", Scene.RenderFilter.MapPiece | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Light);
        public RenderFilterPreset SceneFilter_Preset_05 = new("Lighting (Collision)", Scene.RenderFilter.Collision | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Light);
        public RenderFilterPreset SceneFilter_Preset_06 = new("All", Scene.RenderFilter.All);

        public bool EnableTexturing = false;

        public int GFX_Display_Width { get; set; } = 1920;
        public int GFX_Display_Height { get; set; } = 1057;

        public int GFX_Display_X { get; set; } = 0;
        public int GFX_Display_Y { get; set; } = 23;

        public float GFX_Camera_FOV { get; set; } = 60.0f;
        public float GFX_Camera_MoveSpeed_Slow { get; set; } = 1.0f;
        public float GFX_Camera_MoveSpeed_Normal { get; set; } = 20.0f;
        public float GFX_Camera_MoveSpeed_Fast { get; set; } = 200.0f;
        public float GFX_RenderDistance_Max { get; set; } = 50000.0f;

        // Map Editor settings
        public bool Map_AlwaysListLoadedMaps = true;

        // Font settings
        public bool FontChinese = false;
        public bool FontKorean = false;
        public bool FontThai = false;
        public bool FontVietnamese = false;
        public bool FontCyrillic = false;
        public float UIScale = 1.0f;

        // FMG Editor settings
        public bool FMG_ShowOriginalNames = false;
        public bool FMG_NoGroupedFmgEntries = false;
        public bool FMG_NoFmgPatching = false;

        // Param settings
        public bool Param_ShowAltNames = true;
        public bool Param_AlwaysShowOriginalName = true;
        public bool Param_HideReferenceRows = false;
        public bool Param_HideEnums = false;
        public bool Param_AllowFieldReorder = true;
        public bool Param_AlphabeticalParams = true;
        public bool Param_ShowVanillaParams = true;
        public bool Param_PasteAfterSelection = false;
        public bool Param_DisableRowGrouping = false;

        //private string _Param_Export_Array_Delimiter = "|";
        private string _Param_Export_Delimiter = ",";
        public string Param_Export_Delimiter
        {
            get
            {
                if (_Param_Export_Delimiter.Length == 0)
                    _Param_Export_Delimiter = CFG.Default.Param_Export_Delimiter;
                else if (_Param_Export_Delimiter == "|")
                    _Param_Export_Delimiter = CFG.Default.Param_Export_Delimiter; // Temporary measure to prevent conflicts with byte array delimiters. Will be removed later.
                return _Param_Export_Delimiter;
            }
            set { _Param_Export_Delimiter = value; }
        }

        public bool EnableEldenRingAutoMapOffset = true;
        public bool EnableSoapstone = true;
    }
}
