using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using ImGuiNET;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    class ParamEditorScreen
    {
        public ActionManager EditorActionManager = new ActionManager();

        private string _activeParam = null;
        private PARAM.Row _activeRow = null;

        private PropertyEditor _propEditor = null;

        public ParamEditorScreen(Sdl2Window window, GraphicsDevice device)
        {
            _propEditor = new PropertyEditor(EditorActionManager);
        }

        public void OnGUI()
        {
            // Docking setup
            //var vp = ImGui.GetMainViewport();
            var wins = ImGui.GetWindowSize();
            var winp = ImGui.GetWindowPos();
            winp.Y += 20.0f;
            wins.Y -= 20.0f;
            ImGui.SetNextWindowPos(winp);
            ImGui.SetNextWindowSize(wins);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0.0f);
            ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            flags |= ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
            flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
            flags |= ImGuiWindowFlags.NoBackground;
            //ImGui.Begin("DockSpace_MapEdit", flags);
            ImGui.PopStyleVar(4);
            //var dsid = ImGui.GetID("DockSpace_ParamEdit");
            //ImGui.DockSpace(dsid, new Vector2(0, 0));

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Set Interroot..", "CTRL+I") || InputTracker.GetControlShortcut(Key.I))
                    {
                        
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo", "CTRL+Z", false, EditorActionManager.CanUndo()))
                    {
                        EditorActionManager.UndoAction();
                    }
                    if (ImGui.MenuItem("Redo", "Ctrl+Y", false, EditorActionManager.CanRedo()))
                    {
                        EditorActionManager.RedoAction();
                    }
                    if (ImGui.MenuItem("Delete", "Delete", false, Selection.IsSelection()))
                    {
                    }
                    if (ImGui.MenuItem("Duplicate", "Ctrl+D", false, Selection.IsSelection()))
                    {
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }

            // Keyboard shortcuts
            if (EditorActionManager.CanUndo() && InputTracker.GetControlShortcut(Key.Z))
            {
                EditorActionManager.UndoAction();
            }
            if (EditorActionManager.CanRedo() && InputTracker.GetControlShortcut(Key.Y))
            {
                EditorActionManager.RedoAction();
            }
            

            //ImGui.SetNextWindowSize(new Vector2(300, 500), ImGuiCond.FirstUseEver);
            //ImGui.SetNextWindowPos(new Vector2(20, 20), ImGuiCond.FirstUseEver);

            if (ParamBank.Params == null)
            {
                return;
            }
            ImGui.Columns(3);
            ImGui.BeginChild("params");
            foreach (var param in ParamBank.Params)
            {
                if (ImGui.Selectable(param.Key, param.Key == _activeParam))
                {
                    _activeParam = param.Key;
                    _activeRow = null;
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();
            ImGui.BeginChild("rows");
            if (_activeParam == null)
            {
                ImGui.Text("Select a param to see rows");
            }
            else
            {
                var p = ParamBank.Params[_activeParam];
                foreach (var r in p.Rows)
                {
                    if (ImGui.Selectable($@"{r.ID} {r.Name}", _activeRow == r))
                    {
                        _activeRow = r;
                    }
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();
            ImGui.BeginChild("columns");
            if (_activeRow == null)
            {
                ImGui.Text("Select a row to see properties");
            }
            else
            {
                _propEditor.PropEditorParamRow(_activeRow);
            }
            ImGui.EndChild();
        }
    }
}
