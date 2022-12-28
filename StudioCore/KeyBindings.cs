using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ImGuiNET;
using Veldrid;

namespace StudioCore
{
    public class KeyBind
    {
        public Key PrimaryKey;
        public bool Ctrl_Pressed;
        public bool Alt_Pressed;
        public bool Shift_Pressed;

        [JsonIgnore]
        public string HintText
        {
            get
            {
                if (PrimaryKey == Key.Unknown)
                    return "";

                string str = "";
                if (Ctrl_Pressed)
                    str += "Ctrl+";
                if (Alt_Pressed)
                    str += "Alt+";
                if (Shift_Pressed)
                    str += "Shift+";
                str += PrimaryKey.ToString();
                return str;
            }
        }

        public KeyBind(Key primaryKey = Key.Unknown, bool ctrlKey = false, bool altKey = false, bool shiftKey = false)
        {
            PrimaryKey = primaryKey;
            Ctrl_Pressed = ctrlKey;
            Alt_Pressed = altKey;
            Shift_Pressed = shiftKey;
        }
    }

    public class KeyBindings
    {
        public static Bindings Current { get; set; } = null;
        //public static Bindings Default { get; set; } = new();

        public static void ResetKeyBinds()
        {
            Current = new Bindings();
        }

        public class Bindings
        {
#pragma warning disable IDE0051
            // JsonExtensionData stores info in config file not present in class in order to retain settings between versions.
            [JsonExtensionData]
            private IDictionary<string, JToken> _additionalData;
#pragma warning restore IDE0051

            // Core
            public KeyBind Core_SaveCurrentEditor = new(Key.S, true);
            public KeyBind Core_SaveAllEditors = new();
            public KeyBind Core_Undo = new(Key.Z, true);
            public KeyBind Core_Redo = new(Key.Y, true);
            public KeyBind Core_Delete = new(Key.Delete);
            public KeyBind Core_Duplicate = new(Key.D, true);

            // Viewport (Map & Model)
            public KeyBind Viewport_Cam_Forward = new(Key.W);
            public KeyBind Viewport_Cam_Left = new(Key.A);
            public KeyBind Viewport_Cam_Back = new(Key.S);
            public KeyBind Viewport_Cam_Right = new(Key.D);
            public KeyBind Viewport_Cam_Up = new(Key.E);
            public KeyBind Viewport_Cam_Down = new(Key.Q);
            public KeyBind Viewport_Cam_Reset = new(Key.R);
            public KeyBind Viewport_TranslateMode = new(Key.W);
            public KeyBind Viewport_RotationMode = new(Key.E);
            public KeyBind Viewport_ToggleGizmoSpace = new();
            public KeyBind Viewport_ToggleGizmoOrigin = new(Key.Home);
            public KeyBind Viewport_FrameSelection = new(Key.F);

            // Map
            public KeyBind Map_PropSearch = new(Key.F, true);
            public KeyBind Map_DuplicateToMap = new(Key.D, false, false, true);
            public KeyBind Map_RenderGroup_ShowAll = new(Key.R, true);
            public KeyBind Map_RenderGroup_HideAll = new();
            public KeyBind Map_RenderGroup_GetDisp = new(Key.G, true);
            public KeyBind Map_RenderGroup_GetDraw = new();
            public KeyBind Map_RenderGroup_GiveDisp = new();
            public KeyBind Map_RenderGroup_GiveDraw = new();
            public KeyBind Map_HideToggle = new(Key.H, true);
            public KeyBind Map_UnhideAll = new(Key.H, false, true);
            public KeyBind Map_GotoSelectionInObjectList = new(Key.G);
            public KeyBind Map_ResetRotation = new(Key.L);
            public KeyBind Map_ArbitraryRotationX = new(Key.J);
            public KeyBind Map_ArbitraryRotationY = new(Key.K);
            public KeyBind Map_Dummify = new(Key.Comma, false, false, true);
            public KeyBind Map_UnDummify = new(Key.Period, false, false, true);
            public KeyBind Map_MoveSelectionToCamera = new(Key.X);

            // Parameters
            public KeyBind Param_SelectAll = new(Key.A, true);
            public KeyBind Param_Copy = new(Key.C, true);
            public KeyBind Param_Paste = new(Key.V, true);
            public KeyBind Param_GotoRow = new(Key.G, true);
            public KeyBind Param_SearchParam = new(Key.P, true);
            public KeyBind Param_SearchRow = new(Key.F, true);
            public KeyBind Param_SearchField = new(Key.N, true);
            public KeyBind Param_MassEdit = new();
            public KeyBind Param_ExportCSV = new();
            public KeyBind Param_ImportCSV = new();
            public KeyBind Param_HotReload = new(Key.F5);
            public KeyBind Param_HotReloadAll = new(Key.F5, false, false, true);

            // Text Editor
            public KeyBind TextFMG_Search = new(Key.F, true);
            public KeyBind TextFMG_Import = new();
            public KeyBind TextFMG_ExportAll = new();
        }
    }
}