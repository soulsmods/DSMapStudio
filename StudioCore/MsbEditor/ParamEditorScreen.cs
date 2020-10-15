using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Linq;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using ImGuiNET;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Interface for decorating param rows with additional information (such as english
    /// strings sourced from FMG files)
    /// </summary>
    public interface IParamDecorator
    {
        public void DecorateParam(PARAM.Row row);

        public void DecorateContextMenu(PARAM.Row row);
    }

    public class FMGItemParamDecorator : IParamDecorator
    {
        private FMGBank.ItemCategory _category = FMGBank.ItemCategory.None;

        private Dictionary<int, FMG.Entry> _entryCache = new Dictionary<int, FMG.Entry>();

        public FMGItemParamDecorator(FMGBank.ItemCategory cat)
        {
            _category = cat;
        }

        public void DecorateParam(PARAM.Row row)
        {
            if (!_entryCache.ContainsKey((int)row.ID))
            {
                _entryCache.Add((int)row.ID, FMGBank.LookupItemID((int)row.ID, _category));
            }
            var entry = _entryCache[(int)row.ID];
            if (entry != null)
            {
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), $@" <{entry.Text}>");
            }
        }

        public void DecorateContextMenu(PARAM.Row row)
        {
            if (!_entryCache.ContainsKey((int)row.ID))
            {
                return;
            }
            if (ImGui.BeginPopupContextItem(row.ID.ToString()))
            {
                if (ImGui.Selectable($@"Goto {_category.ToString()} Text"))
                {
                    EditorCommandQueue.AddCommand($@"text/select/{_category.ToString()}/{row.ID}");
                }
                ImGui.EndPopup();
            }
        }
    }

    public class ParamEditorScreen : EditorScreen
    {
        public ActionManager EditorActionManager = new ActionManager();

        private string _activeParam = null;
        private PARAM.Row _activeRow = null;
        private List<PARAM.Row> _selectionRows = new List<PARAM.Row>();

        //MassEdit Popup vars
        private string currentMEditRegexInput = "";
        private string lastMEditRegexInput = "";
        private string mEditRegexResult = "";
        private string currentMEditCSVInput = "";
        private string currentMEditCSVOutput = "";//This is going to be a disgusting string
        private string mEditCSVResult = "";
        private bool isMEditOpen = false;//blanket for any being open - used to turn off keyboard shortcuts while popup is open

        private PropertyEditor _propEditor = null;

        private Dictionary<string, IParamDecorator> _decorators = new Dictionary<string, IParamDecorator>();

        private ProjectSettings _projectSettings = null;
        public ParamEditorScreen(Sdl2Window window, GraphicsDevice device)
        {
            _propEditor = new PropertyEditor(EditorActionManager);

            _decorators.Add("EquipParamAccessory", new FMGItemParamDecorator(FMGBank.ItemCategory.Rings));
            _decorators.Add("EquipParamGoods", new FMGItemParamDecorator(FMGBank.ItemCategory.Goods));
            _decorators.Add("EquipParamProtector", new FMGItemParamDecorator(FMGBank.ItemCategory.Armor));
            _decorators.Add("EquipParamWeapon", new FMGItemParamDecorator(FMGBank.ItemCategory.Weapons));
        }

        public override void DrawEditorMenu()
        {
            bool openMEditRegex = false;
            bool openMEditCSVExport = false;
            bool openMEditCSVImport = false;
            //Menu Options
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
                if (ImGui.MenuItem("Delete", "Delete", false, _activeParam != null && _activeRow != null))
                {
                    if (_activeParam != null && _activeRow != null)
                    {
                        var act = new DeleteParamsAction(ParamBank.Params[_activeParam], new List<PARAM.Row>() { _activeRow });
                        EditorActionManager.ExecuteAction(act);
                        _activeRow = null;
                    }
                }
                if (ImGui.MenuItem("Duplicate", "Ctrl+D", false, _activeParam != null && _activeRow != null))
                {
                    if (_activeParam != null && _activeRow != null)
                    {
                        var act = new AddParamsAction(ParamBank.Params[_activeParam], _activeParam, new List<PARAM.Row>() { _activeRow }, true);
                        EditorActionManager.ExecuteAction(act);
                    }
                }
                if(ImGui.MenuItem("Mass Edit", null, false, true))
                {
                    openMEditRegex = true;
                }
                if(ImGui.MenuItem("Export CSV (Slow!)", null, false, _activeParam != null))
                {
                    openMEditCSVExport = true;
                }
                if(ImGui.MenuItem("Import CSV", null, false, _activeParam != null))
                {
                    openMEditCSVImport = true;
                }
                ImGui.EndMenu();
            }
            //Menu Popups -- imgui scoping
            if(openMEditRegex)
            {
                ImGui.OpenPopup("massEditMenuRegex");
                isMEditOpen = true;
            }
            if(openMEditCSVExport)
            {
                if(_activeParam!=null)
                    currentMEditCSVOutput = MassParamEditCSV.GenerateCSV(ParamBank.Params[_activeParam]);
                ImGui.OpenPopup("massEditMenuCSVExport");
                isMEditOpen = true;
            }
            if(openMEditCSVImport)
            {
                ImGui.OpenPopup("massEditMenuCSVImport");
                isMEditOpen = true;
            }
            MassEditPopups();
        }
        public void MassEditPopups(){
            if(ImGui.BeginPopup("massEditMenuRegex"))
            {
                ImGui.Text("selection: FIELD: ((=|+|-|*|/) VALUE | ref ROW);");
                ImGui.Text("PARAM: (id VALUE | name ROW | prop FIELD VALUE | propref FIELD ROW): FIELD: ((=|+|-|*|/) VALUE | ref ROW);");
                ImGui.InputTextMultiline("MEditRegexInput", ref currentMEditRegexInput, 65536, new Vector2(1024, 256));
                if(ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    MassEditResult r = MassParamEditRegex.PerformMassEdit(currentMEditRegexInput, EditorActionManager, _activeParam, _selectionRows);
                    if(r.type == MassEditResultType.SUCCESS){
                        lastMEditRegexInput = currentMEditRegexInput;
                        currentMEditRegexInput = "";
                    }
                    mEditRegexResult = r.information;
                }
                ImGui.Text(mEditRegexResult);
                ImGui.InputTextMultiline("MEditRegexOutput", ref lastMEditRegexInput, 65536, new Vector2(1024,256), ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if(ImGui.BeginPopup("massEditMenuCSVExport"))
            {
                ImGui.InputTextMultiline("MEditOutput", ref currentMEditCSVOutput, 65536, new Vector2(1024,256), ImGuiInputTextFlags.ReadOnly);//size problems lol
                ImGui.EndPopup();
            }
            else if(ImGui.BeginPopup("massEditMenuCSVImport"))
            {
                ImGui.InputTextMultiline("MEditRegexInput", ref currentMEditCSVInput, 256*65536, new Vector2(1024, 256));
                if(ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    MassEditResult r = MassParamEditCSV.PerformMassEdit(currentMEditCSVInput, EditorActionManager, _activeParam);
                    if(r.type == MassEditResultType.SUCCESS){
                        lastMEditRegexInput = currentMEditRegexInput;
                        currentMEditRegexInput = "";
                    }
                    mEditCSVResult = r.information;
                }
                ImGui.Text(mEditCSVResult);
                ImGui.EndPopup();
            }
            else
            {
                isMEditOpen = false;//busy setting :/
                currentMEditCSVOutput = "";
            }
        }

        public void OnGUI(string[] initcmd)
        {
            if(!isMEditOpen)//Are shortcuts active? Presently just checks for massEdit popup. Why is this even a thing? because accidentally pressing delete when editing
            {
                // Keyboard shortcuts
                if (EditorActionManager.CanUndo() && InputTracker.GetControlShortcut(Key.Z))
                {
                    EditorActionManager.UndoAction();
                }
                if (EditorActionManager.CanRedo() && InputTracker.GetControlShortcut(Key.Y))
                {
                    EditorActionManager.RedoAction();
                }
                if (InputTracker.GetControlShortcut(Key.D))
                {
                    if (_activeParam != null && _activeRow != null)
                    {
                        var act = new AddParamsAction(ParamBank.Params[_activeParam], _activeParam, new List<PARAM.Row>() { _activeRow }, true);
                        EditorActionManager.ExecuteAction(act);
                    }
                }
                if (InputTracker.GetKeyDown(Key.Delete))
                {
                    if (_activeParam != null && _activeRow != null)
                    {
                        var act = new DeleteParamsAction(ParamBank.Params[_activeParam], new List<PARAM.Row>() { _activeRow });
                        EditorActionManager.ExecuteAction(act);
                        _activeRow = null;
                        _selectionRows.Remove(_activeRow);
                    }
                }
            }

            if (ParamBank.Params == null)
            {
                return;
            }

            bool doFocus = false;
            // Parse select commands
            if (initcmd != null && initcmd[0] == "select")
            {
                if (initcmd.Length > 1 && ParamBank.Params.ContainsKey(initcmd[1]))
                {
                    doFocus = true;
                    _activeParam = initcmd[1];
                    if (initcmd.Length > 2)
                    {
                        _selectionRows.Clear();
                        var p = ParamBank.Params[_activeParam];
                        int id;
                        var parsed = int.TryParse(initcmd[2], out id);
                        if (parsed)
                        {
                            var r = p.Rows.FirstOrDefault(r => r.ID == id);
                            if (r != null)
                            {
                                _activeRow = r;
                                _selectionRows.Add(r);
                            }
                        }
                    }
                }
            }

            ImGui.Columns(3);
            ImGui.BeginChild("params");
            foreach (var param in ParamBank.Params)
            {
                if (ImGui.Selectable(param.Key, param.Key == _activeParam))
                {
                    _activeParam = param.Key;
                    _selectionRows.Clear();
                    _activeRow = null;
                }
                if (doFocus && param.Key == _activeParam)
                {
                    ImGui.SetScrollHereY();
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
                IParamDecorator decorator = null;
                if (_decorators.ContainsKey(_activeParam))
                {
                    decorator = _decorators[_activeParam];
                }
                var p = ParamBank.Params[_activeParam];
                foreach (var r in p.Rows)
                {
                    if (ImGui.Selectable($@"{r.ID} {r.Name}", _selectionRows.Contains(r)))
                    {
                        if(InputTracker.GetKey(Key.LControl))
                        {
                            if(!_selectionRows.Contains(r))
                                _selectionRows.Add(r);
                            else
                                _selectionRows.Remove(r);
                        }
                        else
                        {
                            _selectionRows.Clear();
                            _selectionRows.Add(r);
                            if(InputTracker.GetKey(Key.LShift))
                            {
                                int start = p.Rows.IndexOf(_activeRow);
                                int end = p.Rows.IndexOf(r);
                                foreach(var r2 in p.Rows.GetRange(start<end?start:end, start<end?end-start:start-end))
                                    _selectionRows.Add(r2);
                            }
                            else
                                _activeRow = r;
                        }
                    }
                    if (decorator != null)
                    {
                        decorator.DecorateContextMenu(r);
                        decorator.DecorateParam(r);
                    }
                    if (doFocus && _activeRow == r)
                    {
                        ImGui.SetScrollHereY();
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

        public override void OnProjectChanged(ProjectSettings newSettings)
        {
            _projectSettings = newSettings;
            _activeParam = null;
            _selectionRows.Clear();
            _activeRow = null;
        }

        public override void Save()
        {
            if (_projectSettings != null)
            {
                ParamBank.SaveParams(_projectSettings.UseLooseParams);
            }
        }

        public override void SaveAll()
        {
            if (_projectSettings != null)
            {
                ParamBank.SaveParams(_projectSettings.UseLooseParams);
            }
        }
    }
}
