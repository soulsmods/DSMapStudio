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
        public bool ControlKey = false;
        public bool AltKey = false;
        public bool ShiftKey = false;
        [JsonIgnore]
        public string KeyShortcutText
        {
            get
            {
                string str = "";
                if (ControlKey)
                    str += "Ctrl+";
                if (AltKey)
                    str += "Alt+";
                if (ShiftKey)
                    str += "Shift+";
                str += PrimaryKey.ToString();
                return str;
            }
        }
        public KeyBind(Key primaryKey, bool ctrlKey = false, bool altKey = false, bool shiftKey = false)
        {
            PrimaryKey = primaryKey;
            ControlKey = ctrlKey;
            AltKey = altKey;
            ShiftKey = shiftKey;
        }
    }

    public class KeyBindings
    {
        public static Bindings Current { get; set; } = null;
        public static Bindings Default { get; set; } = new();

        public static void ResetKeyBinds()
        {
            Current = new Bindings();
        }

        public class Bindings
        {
            // JsonExtensionData stores info in config file not present in class in order to retain settings between versions.
            [JsonExtensionData]
            private IDictionary<string, JToken> _additionalData;

            // Core
            public KeyBind Core_SaveCurrentEditor = new(Key.S, true);
            public KeyBind Core_SaveAllEditors = new(Key.S, true, true);
            public KeyBind Core_NewProject = new(Key.N, true);
            public KeyBind Core_Undo = new(Key.Z, true);
            public KeyBind Core_Redo = new(Key.Y, true);
            public KeyBind Core_Delete = new(Key.Delete);

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
            public KeyBind Viewport_ToggleOrigin = new(Key.Home);
            public KeyBind Viewport_FrameSelection = new(Key.F);

            // Map
            public KeyBind Map_PropSearch = new(Key.F, true);
            public KeyBind Map_Duplicate = new(Key.D, true);
            public KeyBind Map_DuplicateToMap = new(Key.D, false, false, true); // TODO2: implement
            public KeyBind Map_RenderGroup_ShowAll = new(Key.R, true);
            public KeyBind Map_RenderGroup_GetDisp = new(Key.G, true);
            public KeyBind Map_HideToggle = new(Key.H, true);
            public KeyBind Map_UnhideAll = new(Key.H, false, true);
            public KeyBind Map_Goto = new(Key.G);
            public KeyBind Map_UnDummify = new(Key.Comma, false, false, true);
            public KeyBind Map_Dummify = new(Key.Period, false, false, true);

            // Parameters
            public KeyBind Param_SelectAll = new(Key.A, true);
            public KeyBind Param_Copy = new(Key.C, true);
            public KeyBind Param_Paste = new(Key.V, true);
            public KeyBind Param_Duplicate = new(Key.D, true);
            public KeyBind Param_GotoRow = new(Key.G, true);
            public KeyBind Param_SearchParam = new(Key.P, true);
            public KeyBind Param_SearchRow = new(Key.F, true);
            public KeyBind Param_SearchField = new(Key.N, true);
            public KeyBind Param_HotReload = new(Key.F5);

            // Text Editor
            public KeyBind TextFMG_Search = new(Key.F, true);
            public KeyBind TextFMG_Duplicate = new(Key.D, true);
        }
    }
}