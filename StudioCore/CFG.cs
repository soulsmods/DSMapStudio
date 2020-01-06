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

        public static string GetConfigFilePath()
        {
            return Utils.Frankenpath(new FileInfo(typeof(CFG).Assembly.Location).DirectoryName, FileName);
        }

        public static void ResetGraphics()
        {
            if (IsEnabled)
            {
                Current = new CFG();

                /*GFX.LODMode = LODMode.Automatic;
                GFX.LOD1Distance = 200;
                GFX.LOD2Distance = 400;
                GFX.EnableFrustumCulling = false;
                GFX.EnableTextures = true;
                GFX.Wireframe = false;
                //GFX.EnableLighting = true;
                //GFX.TestLightSpin = false;
                //GFX.EnableHeadlight = true;

                DBG.ShowModelNames = true;
                DBG.ShowModelBoundingBoxes = false;
                DBG.ShowModelSubmeshBoundingBoxes = false;
                DBG.ShowPrimitiveNametags = true;
                DBG.PrimitiveNametagSize = 0.1f;
                DBG.ShowGrid = true;
                //DBG.SimpleTextLabelSize = true;

                GFX.World.FieldOfView = 43;
                GFX.World.NearClipDistance = 0.1f;
                GFX.World.FarClipDistance = 10000f;*/

                Save();
                Load();
            }
        }

        public static void ResetControls()
        {
            if (IsEnabled)
            {
                //GFX.World.CameraMoveSpeed = 1;
                //GFX.World.CameraTurnSpeedGamepad = 1.5f * 0.5f;
                //GFX.World.CameraTurnSpeedMouse = 1.5f * 0.5f;

                Save();
                Load();
            }
        }

        public static void ResetDisplay()
        {
            if (IsEnabled)
            {
                //GFX.Display.Width = 1600;
                //GFX.Display.Height = 900;
                //GFX.Display.Format = SurfaceFormat.Color;
                //GFX.Display.Vsync = true;
                //GFX.Display.Fullscreen = false;
                //GFX.Display.SimpleMSAA = true;

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

                    //InterrootLoader.Interroot = Current.InterrootLoader_Interroot;
                    //InterrootLoader.Type = Current.InterrootLoader_Type;

                    /*GFX.LODMode = Current.GFX_LODMode;
                    GFX.LOD1Distance = Current.GFX_LOD1Distance;
                    GFX.LOD2Distance = Current.GFX_LOD2Distance;
                    GFX.EnableFrustumCulling = Current.GFX_EnableFrustumCulling;
                    GFX.EnableTextures = Current.GFX_EnableTextures;
                    GFX.Wireframe = Current.GFX_Wireframe;
                    //GFX.EnableLighting = Current.GFX_EnableLighting;
                    //GFX.TestLightSpin = Current.GFX_TestLightSpin;
                    //GFX.EnableHeadlight = Current.GFX_EnableHeadlight;

                    DBG.ShowModelNames = Current.DBG_ShowModelNames;
                    DBG.ShowModelBoundingBoxes = Current.DBG_ShowModelBoundingBoxes;
                    DBG.ShowModelSubmeshBoundingBoxes = Current.DBG_ShowModelSubmeshBoundingBoxes;
                    DBG.ShowPrimitiveNametags = Current.DBG_ShowPrimitiveNametags;
                    //DBG.PrimitiveNametagSize = Current.DBG_PrimitiveNametagSize;
                    DBG.ShowGrid = Current.DBG_ShowGrid;
                    //DBG.SimpleTextLabelSize = Current.DBG_SimpleTextLabelSize;

                    GFX.World.CameraMoveSpeed = Current.GFX_World_CameraMoveSpeed;
                    GFX.World.CameraTurnSpeedGamepad = Current.GFX_World_CameraTurnSpeedGamepad;
                    GFX.World.CameraTurnSpeedMouse = Current.GFX_World_CameraTurnSpeedMouse;
                    GFX.World.FieldOfView = Current.GFX_World_FieldOfView;
                    GFX.World.NearClipDistance = Current.GFX_World_NearClipDistance;
                    GFX.World.FarClipDistance = Current.GFX_World_FarClipDistance;

                    GFX.Display.Width = Current.GFX_Display_Width;
                    GFX.Display.Height = Current.GFX_Display_Height;
                    GFX.Display.Format = Current.GFX_Display_Format;
                    GFX.Display.Vsync = Current.GFX_Display_Vsync;
                    GFX.Display.Fullscreen = Current.GFX_Display_Fullscreen;
                    GFX.Display.SimpleMSAA = Current.GFX_Display_SimpleMSAA;

                    GFX.Display.Apply();*/
                }
            }
        }

        public static void Save()
        {
            if (IsEnabled)
            {
                lock (_lock_SaveLoadCFG)
                {
                    //Current.InterrootLoader_Interroot = InterrootLoader.Interroot;
                    //Current.InterrootLoader_Type = InterrootLoader.Type;

                    /*Current.GFX_LODMode = GFX.LODMode;
                    Current.GFX_LOD1Distance = GFX.LOD1Distance;
                    Current.GFX_LOD2Distance = GFX.LOD2Distance;
                    Current.GFX_EnableFrustumCulling = GFX.EnableFrustumCulling;
                    Current.GFX_EnableTextures = GFX.EnableTextures;
                    Current.GFX_Wireframe = GFX.Wireframe;
                    //Current.GFX_EnableLighting = GFX.EnableLighting;
                    //Current.GFX_TestLightSpin = GFX.TestLightSpin;
                    //Current.GFX_EnableHeadlight = GFX.EnableHeadlight;

                    Current.DBG_ShowModelNames = DBG.ShowModelNames;
                    Current.DBG_ShowModelBoundingBoxes = DBG.ShowModelBoundingBoxes;
                    Current.DBG_ShowModelSubmeshBoundingBoxes = DBG.ShowModelSubmeshBoundingBoxes;
                    Current.DBG_ShowPrimitiveNametags = DBG.ShowPrimitiveNametags;
                    //Current.DBG_PrimitiveNametagSize = DBG.PrimitiveNametagSize;

                    Current.DBG_ShowGrid = DBG.ShowGrid;
                    //Current.DBG_SimpleTextLabelSize = true;

                    Current.GFX_World_CameraMoveSpeed = GFX.World.CameraMoveSpeed;
                    Current.GFX_World_CameraTurnSpeedGamepad = GFX.World.CameraTurnSpeedGamepad;
                    Current.GFX_World_CameraTurnSpeedMouse = GFX.World.CameraTurnSpeedMouse;
                    Current.GFX_World_FieldOfView = GFX.World.FieldOfView;
                    Current.GFX_World_NearClipDistance = GFX.World.NearClipDistance;
                    Current.GFX_World_FarClipDistance = GFX.World.FarClipDistance;

                    Current.GFX_Display_Width = GFX.Display.Width;
                    Current.GFX_Display_Height = GFX.Display.Height;
                    Current.GFX_Display_Format = GFX.Display.Format;
                    Current.GFX_Display_Vsync = GFX.Display.Vsync;
                    Current.GFX_Display_Fullscreen = GFX.Display.Fullscreen;
                    Current.GFX_Display_SimpleMSAA = GFX.Display.SimpleMSAA;*/

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

        //public string InterrootLoader_Interroot { get; set; } 
        //    = @"C:\Program Files (x86)\steam\steamapps\common\Dark Souls Prepare to Die Edition\DATA";

        //public InterrootLoader.InterrootType InterrootLoader_Type { get; set; }
        //    = InterrootLoader.InterrootType.InterrootDS1;

        public string Interroot_Directory { get; set; } = "";
        public string Mod_Directory { get; set; } = "";
        public GameType Game_Type { get; set; } = GameType.Undefined;

        public int GFX_Display_Width { get; set; } = 1920;
        public int GFX_Display_Height { get; set; } = 1057;

        public int GFX_Display_X { get; set; } = 0;
        public int GFX_Display_Y { get; set; } = 23;

        public float GFX_LOD1Distance { get; set; } = 200.0f;
        public float GFX_LOD2Distance { get; set; } = 400.0f;
        public bool GFX_EnableTextures { get; set; } = true;
        public bool GFX_Wireframe { get; set; } = false;
        public bool GFX_EnableFrustumCulling { get; set; } = false;
        public bool GFX_EnableLighting { get; set; } = true;

        //public bool GFX_TestLightSpin { get; set; } = false;
        public bool GFX_EnableHeadlight { get; set; } = true;

        public bool DBG_ShowModelNames { get; set; } = false;
        public bool DBG_ShowModelBoundingBoxes { get; set; } = false;
        public bool DBG_ShowModelSubmeshBoundingBoxes { get; set; } = false;
        public bool DBG_ShowGrid { get; set; } = true;
        public bool DBG_ShowPrimitiveNametags { get; set; } = false;
        //public float DBG_PrimitiveNametagSize { get; set; } = 1.0f;
        //public bool DBG_SimpleTextLabelSize { get; set; } = false;

        public float GFX_World_FieldOfView { get; set; } = 43.0f;
        public float GFX_World_CameraTurnSpeedGamepad { get; set; } = 1.5f;
        public float GFX_World_CameraTurnSpeedMouse { get; set; } = 1.5f;
        public float GFX_World_CameraMoveSpeed { get; set; } = 10.0f;
        public float GFX_World_NearClipDistance { get; set; } = 0.1f;
        public float GFX_World_FarClipDistance { get; set; } = 10000.0f;

        public bool GFX_Display_Vsync { get; set; } = true;
        public bool GFX_Display_Fullscreen { get; set; } = false;
        public bool GFX_Display_SimpleMSAA { get; set; } = true;
    }
}
