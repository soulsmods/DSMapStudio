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

        private string _currentSearchString = "";
        private string _activeParam = null;
        private PARAM.Row _activeRow = null;
        private List<PARAM.Row> _selectionRows = new List<PARAM.Row>();

        // Clipboard vars
        private string _clipboardParam = null;
        private List<PARAM.Row> _clipboardRows = new List<PARAM.Row>();
        private string _currentCtrlVInput = "0";

        // MassEdit Popup vars
        private string _currentMEditRegexInput = "";
        private string _lastMEditRegexInput = "";
        private string _mEditRegexResult = "";
        private string _currentMEditCSVInput = "";
        private string _currentMEditCSVOutput = "";
        private string _mEditCSVResult = "";

        private bool _isMEditPopupOpen = false;
        private bool _isShortcutPopupOpen = false;

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
            // Menu Options
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
                if (ImGui.MenuItem("Mass Edit", null, false, true))
                {
                    openMEditRegex = true;
                }
                if (ImGui.MenuItem("Export CSV (Slow!)", null, false, _activeParam != null))
                {
                    openMEditCSVExport = true;
                }
                if (ImGui.MenuItem("Import CSV", null, false, _activeParam != null))
                {
                    openMEditCSVImport = true;
                }
                ImGui.EndMenu();
            }
            // Menu Popups -- imgui scoping
            if (openMEditRegex)
            {
                ImGui.OpenPopup("massEditMenuRegex");
                _isMEditPopupOpen = true;
            }
            if (openMEditCSVExport)
            {
                if (_activeParam != null)
                    _currentMEditCSVOutput = MassParamEditCSV.GenerateCSV(ParamBank.Params[_activeParam]);
                ImGui.OpenPopup("massEditMenuCSVExport");
                _isMEditPopupOpen = true;
            }
            if (openMEditCSVImport)
            {
                ImGui.OpenPopup("massEditMenuCSVImport");
                _isMEditPopupOpen = true;
            }
            MassEditPopups();
        }
        public void MassEditPopups()
        {
            // Popup size relies on magic numbers. Multiline maxlength is also arbitrary.
            if (ImGui.BeginPopup("massEditMenuRegex"))
            {
                ImGui.Text("selection: FIELD: ((=|+|-|*|/) VALUE | ref ROW);");
                ImGui.Text("param PARAM: (id VALUE | name ROW | prop FIELD VALUE | propref FIELD ROW): FIELD: ((=|+|-|*|/) VALUE | ref ROW);");
                ImGui.InputTextMultiline("MEditRegexInput", ref _currentMEditRegexInput, 65536, new Vector2(1024, 256));
                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    MassEditResult r = MassParamEditRegex.PerformMassEdit(_currentMEditRegexInput, EditorActionManager, _activeParam, _selectionRows);
                    if (r.Type == MassEditResultType.SUCCESS)
                    {
                        _lastMEditRegexInput = _currentMEditRegexInput;
                        _currentMEditRegexInput = "";
                    }
                    _mEditRegexResult = r.Information;
                }
                ImGui.Text(_mEditRegexResult);
                ImGui.InputTextMultiline("MEditRegexOutput", ref _lastMEditRegexInput, 65536, new Vector2(1024,256), ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuCSVExport"))
            {
                ImGui.InputTextMultiline("MEditOutput", ref _currentMEditCSVOutput, 65536, new Vector2(1024,256), ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuCSVImport"))
            {
                ImGui.InputTextMultiline("MEditRegexInput", ref _currentMEditCSVInput, 256 * 65536, new Vector2(1024, 256));
                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    MassEditResult r = MassParamEditCSV.PerformMassEdit(_currentMEditCSVInput, EditorActionManager, _activeParam);
                    if (r.Type == MassEditResultType.SUCCESS)
                    {
                        _lastMEditRegexInput = _currentMEditRegexInput;
                        _currentMEditRegexInput = "";
                    }
                    _mEditCSVResult = r.Information;
                }
                ImGui.Text(_mEditCSVResult);
                ImGui.EndPopup();
            }
            else
            {
                _isMEditPopupOpen = false;
                _currentMEditCSVOutput = "";
            }
        }

        public void OnGUI(string[] initcmd)
        {
            if (!_isMEditPopupOpen && !_isShortcutPopupOpen)// Are shortcuts active? Presently just checks for massEdit popup.
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
                if (InputTracker.GetControlShortcut(Key.C))
                {
                    _clipboardParam = _activeParam;
                    _clipboardRows.Clear();
                    foreach (PARAM.Row r in _selectionRows)
                        _clipboardRows.Add(new PARAM.Row(r));// make a clone
                }
                if (_activeParam != null && _clipboardParam == _activeParam && InputTracker.GetControlShortcut(Key.V))
                {
                    ImGui.OpenPopup("ctrlVPopup");
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

            ShortcutPopups();

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
            ImGui.Text("id VALUE | name ROW | prop FIELD VALUE | propref FIELD ROW");
            ImGui.InputText("Search rows...", ref _currentSearchString, 256);
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

                PARAM para = ParamBank.Params[_activeParam];
                List<PARAM.Row> p;
                Match m = new Regex(MassParamEditRegex.rowfilterRx).Match(_currentSearchString);
                if (!m.Success)
                    p = para.Rows;
                else
                    p = MassParamEditRegex.GetMatchingParamRows(para, m, true, true);

                foreach (var r in p)
                {
                    if (ImGui.Selectable($@"{r.ID} {r.Name}", _selectionRows.Contains(r)))
                    {
                        if (InputTracker.GetKey(Key.LControl))
                        {
                            if (!_selectionRows.Contains(r))
                                _selectionRows.Add(r);
                            else
                                _selectionRows.Remove(r);
                        }
                        else
                        {
                            _selectionRows.Clear();
                            _selectionRows.Add(r);
                            if (InputTracker.GetKey(Key.LShift))
                            {
                                int start = p.IndexOf(_activeRow);
                                int end = p.IndexOf(r);
                                if (start != end)
                                {
                                    foreach (var r2 in p.GetRange(start < end ? start : end, Math.Abs(end - start)))
                                        _selectionRows.Add(r2);
                                }
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

        public void ShortcutPopups()
        {
            if (ImGui.BeginPopup("ctrlVPopup"))
            {
                ImGui.InputText("Offset", ref _currentCtrlVInput, 20);
                long offset = 0;
                try
                {
                    offset = int.Parse(_currentCtrlVInput);
                }
                catch
                {
                    ImGui.EndPopup();
                    return;
                }
                if (ImGui.Selectable("Submit"))
                {
                List<PARAM.Row> rowsToInsert = new List<PARAM.Row>();
                    if (_activeRow != null)
                    {
                        foreach (PARAM.Row r in _clipboardRows)
                        {
                            PARAM.Row newrow = new PARAM.Row(r);// more cloning
                            newrow.ID = r.ID + offset;
                            rowsToInsert.Add(newrow);
                        }
                    }
                    EditorActionManager.ExecuteAction(new AddParamsAction(ParamBank.Params[_clipboardParam], "legacystring", rowsToInsert, false));
                }
                ImGui.EndPopup();
            }

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
