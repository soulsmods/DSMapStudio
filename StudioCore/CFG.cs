using Newtonsoft.Json;
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
        public const string FileName = "DSMapStudio_Config.json";
        public static CFG Current { get; private set; } = null;
        public static CFG Default { get; private set; } = new();

        public const int MAX_RECENT_PROJECTS = 10;

        public static string GetConfigFilePath()
        {
            return $@"{GetConfigFolderPath()}\{FileName}";
        }
        public static string GetConfigFolderPath()
        {
            //return $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\{FolderName}";
            return $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{FolderName}";
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
                    do
                    {
                        try
                        {
                            Current = JsonConvert.DeserializeObject<CFG>(
                            File.ReadAllText(GetConfigFilePath()));
                        }
                        catch (JsonReaderException e)
                        {
                            if (MessageBox.Show($"{e.Message}\n\nReset config settings?", $"{FileName} Load Error", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                Current = new CFG();
                            }
                        }
                    }
                    while (Current == null);
                }
            }
        }

        public static void Save()
        {
            if (IsEnabled)
            {
                lock (_lock_SaveLoadCFG)
                {
                    var json = JsonConvert.SerializeObject(
                        Current, Formatting.Indented);
                    if (!Directory.Exists(GetConfigFolderPath()))
                        Directory.CreateDirectory(GetConfigFolderPath());
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

        // Param settings
        public bool Param_ShowAltNames = true;
        public bool Param_AlwaysShowOriginalName = true;
        public bool Param_HideReferenceRows = true;
        public bool Param_HideEnums = true;
        public bool Param_AllowFieldReorder = true;
        public bool Param_AlphabeticalParams = true;
        public bool Param_ShowVanillaParams = true;

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
        
        public bool FMG_ShowOriginalNames = false;

        public bool EnableEldenRingAutoMapOffset { get; set; } = true;
    }
}
