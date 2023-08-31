using StudioCore.Platform;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudioCore
{
    [JsonSourceGenerationOptions(WriteIndented = true, 
        GenerationMode = JsonSourceGenerationMode.Metadata, IncludeFields = true)]
    [JsonSerializable(typeof(CFG))]
    internal partial class CfgSerializerContext : JsonSerializerContext
    {
    }
    
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
        public IDictionary<string, JsonElement> AdditionalData;
#pragma warning restore IDE0051

        public const int MAX_RECENT_PROJECTS = 20;

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
            }
            else
            {
                try
                {
                    var options = new JsonSerializerOptions();
                    Current = JsonSerializer.Deserialize(File.ReadAllText(GetConfigFilePath()),
                        CfgSerializerContext.Default.CFG);
                    if (Current == null)
                        throw new Exception("JsonConvert returned null");
                }
                catch (Exception e)
                {
                    var result = PlatformUtils.Instance.MessageBox($"{e.Message}\n\nConfig could not be loaded. Reset settings?",
                        $"{Config_FileName} Load Error", MessageBoxButtons.YesNo);
                    if ( result == DialogResult.No)
                    {
                        throw new Exception($"{Config_FileName} could not be loaded.\n\n{e.Message}");
                    }
                    Current = new CFG();
                    SaveConfig();
                }
            }
        }

        private static void LoadKeybinds()
        {
            if (!File.Exists(GetBindingsFilePath()))
            {
                KeyBindings.Current = new KeyBindings.Bindings();
            }
            else
            {
                try
                {
                    KeyBindings.Current = JsonSerializer.Deserialize(File.ReadAllText(GetBindingsFilePath()),
                        KeybindingsSerializerContext.Default.Bindings);
                    if (KeyBindings.Current == null)
                        throw new Exception("JsonConvert returned null");
                }
                catch (Exception e)
                {
                    var result = PlatformUtils.Instance.MessageBox($"{e.Message}\n\nKeybinds could not be loaded. Reset keybinds?",
                        $"{Keybinds_FileName} Load Error", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        throw new Exception($"{Keybinds_FileName} could not be loaded.\n\n{e.Message}");
                    }
                    KeyBindings.Current = new KeyBindings.Bindings();
                    SaveKeybinds();
                }
            }
        }

        private static void SaveConfig()
        {
            var json = JsonSerializer.Serialize(
                Current, CfgSerializerContext.Default.CFG);
            File.WriteAllText(GetConfigFilePath(), json);
        }

        private static void SaveKeybinds()
        {
            var json = JsonSerializer.Serialize(
                KeyBindings.Current, KeybindingsSerializerContext.Default.Bindings);
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
            public IDictionary<string, JsonElement> AdditionalData { get; set; }
#pragma warning restore IDE0051

            public string Name { get; set; }
            public string ProjectFile { get; set; }
            public GameType GameType { get; set; }
        }

        public string LastProjectFile { get; set; } = "";
        public List<RecentProject> RecentProjects { get; set; } = new List<RecentProject>();

        public GameType Game_Type { get; set; } = GameType.Undefined;

        public Scene.RenderFilter LastSceneFilter { get; set; } = Scene.RenderFilter.All ^ Scene.RenderFilter.Light;

        public class RenderFilterPreset
        {
            public string Name  { get; set; }
            public Scene.RenderFilter Filters  { get; set; }

            [JsonConstructor]
            public RenderFilterPreset()
            {
            }
            
            public RenderFilterPreset(string name, Scene.RenderFilter filters)
            {
                Name = name;
                Filters = filters;
            }
        }

        public RenderFilterPreset SceneFilter_Preset_01 { get; set; } = new("Map", Scene.RenderFilter.MapPiece | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region);
        public RenderFilterPreset SceneFilter_Preset_02 { get; set; } = new("Collision", Scene.RenderFilter.Collision | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region);
        public RenderFilterPreset SceneFilter_Preset_03 { get; set; } = new("Collision & Navmesh", Scene.RenderFilter.Collision | Scene.RenderFilter.Navmesh | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region);
        public RenderFilterPreset SceneFilter_Preset_04 { get; set; } = new("Lighting (Map)", Scene.RenderFilter.MapPiece | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Light);
        public RenderFilterPreset SceneFilter_Preset_05 { get; set; } = new("Lighting (Collision)", Scene.RenderFilter.Collision | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Light);
        public RenderFilterPreset SceneFilter_Preset_06 { get; set; } = new("All", Scene.RenderFilter.All);

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
        public float GFX_Wireframe_Color_Variance = 0.11f;

        public int GFX_Limit_Renderables = 50000;
        public uint GFX_Limit_Buffer_Indirect_Draw = 50000;
        public uint GFX_Limit_Buffer_Flver_Bone = 65536;

        public Vector3 GFX_Gizmo_X_BaseColor = new Vector3(0.952f, 0.211f, 0.325f);
        public Vector3 GFX_Gizmo_X_HighlightColor = new Vector3(1.0f, 0.4f, 0.513f);

        public Vector3 GFX_Gizmo_Y_BaseColor = new Vector3(0.525f, 0.784f, 0.082f);
        public Vector3 GFX_Gizmo_Y_HighlightColor = new Vector3(0.713f, 0.972f, 0.270f);

        public Vector3 GFX_Gizmo_Z_BaseColor = new Vector3(0.219f, 0.564f, 0.929f);
        public Vector3 GFX_Gizmo_Z_HighlightColor = new Vector3(0.407f, 0.690f, 1.0f);

        // Map Editor settings
        public bool Map_AlwaysListLoadedMaps = true;
        public float Map_ArbitraryRotation_X_Shift { get; set; } = 90.0f;
        public float Map_ArbitraryRotation_Y_Shift { get; set; } = 90.0f;
        public float Map_MoveSelectionToCamera_Radius = 3.0f;

        // Font settings
        public bool FontChinese = false;
        public bool FontKorean = false;
        public bool FontThai = false;
        public bool FontVietnamese = false;
        public bool FontCyrillic = false;

        // UI settings
        public float UIScale = 1.0f;
        public bool UI_CompactParams = false;

        // FMG Editor settings
        public bool FMG_ShowOriginalNames = false;
        public bool FMG_NoGroupedFmgEntries = false;
        public bool FMG_NoFmgPatching = false;

        // Param settings
        public bool Param_MakeMetaNamesPrimary = true;
        public bool Param_ShowSecondaryNames = true;
        public bool Param_ShowFieldOffsets = false; 
        public bool Param_HideReferenceRows = false;
        public bool Param_HideEnums = false;
        public bool Param_AllowFieldReorder = true;
        public bool Param_AlphabeticalParams = true;
        public bool Param_ShowVanillaParams = true;
        public bool Param_PasteAfterSelection = false;
        public bool Param_DisableRowGrouping = false; 
        public bool Param_AdvancedMassedit = false; 

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
        public bool EnableCheckProgramUpdate = true;
    }
}
