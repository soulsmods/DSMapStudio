using System;
using System.Collections.Generic;
using System.Text;

namespace StudioCore.MsbEditor
{
    public abstract class EditorScreen
    {
        public abstract void OnProjectChanged(Editor.ProjectSettings newSettings);

        public abstract void DrawEditorMenu();

        public abstract void Save();
        public abstract void SaveAll();
    }
}
