using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore
{
    public class CFG
    {
        // Set to false currently for TAE editor stuff.
        public static bool IsEnabled = true;

        private static object _lock_SaveLoadCFG = new object();

        public const string FileName = "DSMapStudio_Config.json";
        public static CFG Current { get; private set; } = null;
        public static CFG Default { get; private set; } = new();

        public const int MAX_RECENT_PROJECTS = 10;

        public static string GetConfigFilePath()
        {
            return Utils.Frankenpath(new FileInfo(typeof(CFG).Assembly.Location).DirectoryName, FileName);
        }

        public static void ResetGraphics()
        {
            if (IsEnabled)
            {
                Current = new CFG();
                Save();
                Load();
            }
        }

        public static void ResetControls()
        {
            if (IsEnabled)
            {
                Save();
                Load();
            }
        }

        public static void ResetDisplay()
        {
            if (IsEnabled)
            {
                Save();
                Load();
            }
        }

        public static void Load()
        {
            if (IsEnabled)
            {
                lock (_lock_SaveLoadCFG)
                {
                    Current = Newtonsoft.Json.JsonConvert.DeserializeObject<CFG>(
                    File.ReadAllText(GetConfigFilePath()));
                }
            }
        }

        public static void Save()
        {
            if (IsEnabled)
            {
                lock (_lock_SaveLoadCFG)
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(
                        Current, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(GetConfigFilePath(), json);
                }
            }
        }

        public static void AttemptLoadOrDefault()
        {
            if (IsEnabled)
            {
                if (File.Exists(GetConfigFilePath()))
                {
                    Load();
                }
                else
                {
                    Current = new CFG();
                    ResetControls();
                    ResetGraphics();
                    ResetDisplay();
                    Save();
                }
            }
        }

        public class RecentProject
        {
            public string Name;
            public string ProjectFile;
            public GameType GameType;
        }

        public string LastProjectFile { get; set; } = "";
        public List<RecentProject> RecentProjects { get; set; } = new List<RecentProject>();
        public GameType Game_Type { get; set; } = GameType.Undefined;

        public Scene.RenderFilter LastSceneFilter = Scene.RenderFilter.All;

        public bool EnableTexturing { get; set; } = false;

        public int GFX_Display_Width { get; set; } = 1920;
        public int GFX_Display_Height { get; set; } = 1057;

        public int GFX_Display_X { get; set; } = 0;
        public int GFX_Display_Y { get; set; } = 23;

        public float GFX_Camera_FOV { get; set; } = 60.0f;
        public float GFX_Camera_MoveSpeed_Slow { get; set; } = 1.0f;
        public float GFX_Camera_MoveSpeed_Normal { get; set; } = 20.0f;
        public float GFX_Camera_MoveSpeed_Fast { get; set; } = 200.0f;
        public float GFX_RenderDistance_Max { get; set; } = 50000.0f;
        
        // Font settings
        public bool FontChinese = false;
        public bool FontKorean = false;
        public bool FontThai = false;
        public bool FontVietnamese = false;
        public bool FontCyrillic = false;
        public float FontSizeScale = 1.0f;

        public bool FMG_ShowOriginalNames = false;

        public bool EnableEldenRingAutoMapOffset { get; set; } = true;
    }
}
