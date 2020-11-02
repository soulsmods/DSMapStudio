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

    public class ParamEditorSelectionState
    {
        private static string _globalSearchString = "";
        private string _activeParam = null;
        private Dictionary<string, ParamEditorParamSelectionState> _paramStates = new Dictionary<string, ParamEditorParamSelectionState>();

        public bool paramSelectionExists()
        {
            return _activeParam != null;
        }
        public string getActiveParam()
        {
            return _activeParam;
        }
        public void setActiveParam(string param)
        {
            _activeParam = param;
            if(!_paramStates.ContainsKey(_activeParam))
                _paramStates.Add(_activeParam, new ParamEditorParamSelectionState());
        }
        public ref string getCurrentSearchString()
        {
            if(_activeParam == null)
                return ref _globalSearchString;
            return ref _paramStates[_activeParam].currentSearchString;
        }
        public bool rowSelectionExists()
        {
            return _activeParam != null && _paramStates[_activeParam].activeRow != null;
        }
        public PARAM.Row getActiveRow()
        {
            if(_activeParam == null)
                return null;
            return _paramStates[_activeParam].activeRow;
        }
        public void SetActiveRow(PARAM.Row row)
        {
            if(_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                s.activeRow = row;
                s.selectionRows.Clear();
                s.selectionRows.Add(row);
            }
        }
        public void toggleRowInSelection(PARAM.Row row)
        {
            if(_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                if(s.selectionRows.Contains(row))
                    s.selectionRows.Remove(row);
                else
                    s.selectionRows.Add(row);
            }
        }
        public void addRowToSelection(PARAM.Row row)
        {
            if(_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                if(!s.selectionRows.Contains(row))
                    s.selectionRows.Add(row);
            }
        }
        public void removeRowFromSelection(PARAM.Row row)
        {
            if(_activeParam != null)
                _paramStates[_activeParam].selectionRows.Remove(row);
        }
        public List<PARAM.Row> getSelectedRows()
        {
            if(_activeParam == null)
                return null;
            return _paramStates[_activeParam].selectionRows;
        }
        public void cleanSelectedRows()
        {
            if(_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                s.selectionRows.Clear();
                if(s.activeRow != null)
                    s.selectionRows.Add(s.activeRow);
            }
        }
        public void cleanAllSelectionState()
        {
            _activeParam = null;
            _paramStates.Clear();
        }
    }

    internal class ParamEditorParamSelectionState
    {
        internal string currentSearchString = "";
        internal PARAM.Row activeRow = null;
        internal List<PARAM.Row> selectionRows = new List<PARAM.Row>();
    }

    public class ParamEditorScreen : EditorScreen
    {
        public ActionManager EditorActionManager = new ActionManager();

        private ParamEditorSelectionState _selection = new ParamEditorSelectionState();

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

        private bool _isSearchBarActive = false;
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
                if (ImGui.MenuItem("Delete", "Delete", false, _selection.rowSelectionExists()))
                {
                    if (_selection.rowSelectionExists())
                    {
                        var act = new DeleteParamsAction(ParamBank.Params[_selection.getActiveParam()], new List<PARAM.Row>() { _selection.getActiveRow() });
                        EditorActionManager.ExecuteAction(act);
                        _selection.SetActiveRow(null);
                    }
                }
                if (ImGui.MenuItem("Duplicate", "Ctrl+D", false, _selection.rowSelectionExists()))
                {
                    if (_selection.rowSelectionExists())
                    {
                        var act = new AddParamsAction(ParamBank.Params[_selection.getActiveParam()], _selection.getActiveParam(), new List<PARAM.Row>() { _selection.getActiveRow() }, true);
                        EditorActionManager.ExecuteAction(act);
                    }
                }
                if (ImGui.MenuItem("Mass Edit", null, false, true))
                {
                    openMEditRegex = true;
                }
                if (ImGui.MenuItem("Export CSV (Slow!)", null, false, _selection.paramSelectionExists()))
                {
                    openMEditCSVExport = true;
                }
                if (ImGui.MenuItem("Import CSV", null, false, _selection.paramSelectionExists()))
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
                if (_selection.paramSelectionExists())
                    _currentMEditCSVOutput = MassParamEditCSV.GenerateCSV(ParamBank.Params[_selection.getActiveParam()]);
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
                    MassEditResult r = MassParamEditRegex.PerformMassEdit(_currentMEditRegexInput, EditorActionManager, _selection.getActiveParam(), _selection.getSelectedRows());
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
                    MassEditResult r = MassParamEditCSV.PerformMassEdit(_currentMEditCSVInput, EditorActionManager, _selection.getActiveParam());
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
            if (!_isMEditPopupOpen && !_isShortcutPopupOpen && !_isSearchBarActive)// Are shortcuts active? Presently just checks for massEdit popup.
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
                    _clipboardParam = _selection.getActiveParam();
                    _clipboardRows.Clear();
                    foreach (PARAM.Row r in _selection.getSelectedRows())
                        _clipboardRows.Add(new PARAM.Row(r));// make a clone
                }
                if (_selection.paramSelectionExists() && _clipboardParam == _selection.getActiveParam() && InputTracker.GetControlShortcut(Key.V))
                {
                    ImGui.OpenPopup("ctrlVPopup");
                }
                if (InputTracker.GetControlShortcut(Key.D))
                {
                    if (_selection.rowSelectionExists())
                    {
                        var act = new AddParamsAction(ParamBank.Params[_selection.getActiveParam()], _selection.getActiveParam(), new List<PARAM.Row>() { _selection.getActiveRow() }, true);
                        EditorActionManager.ExecuteAction(act);
                    }
                }
                if (InputTracker.GetKeyDown(Key.Delete))
                {
                    if (_selection.rowSelectionExists())
                    {
                        var act = new DeleteParamsAction(ParamBank.Params[_selection.getActiveParam()], new List<PARAM.Row>() { _selection.getActiveRow() });
                        EditorActionManager.ExecuteAction(act);
                        _selection.SetActiveRow(null);
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
                    _selection.setActiveParam(initcmd[1]);
                    if (initcmd.Length > 2)
                    {
                        _selection.SetActiveRow(null);
                        var p = ParamBank.Params[_selection.getActiveParam()];
                        int id;
                        var parsed = int.TryParse(initcmd[2], out id);
                        if (parsed)
                        {
                            var r = p.Rows.FirstOrDefault(r => r.ID == id);
                            if (r != null)
                            {
                                _selection.SetActiveRow(r);
                            }
                        }
                    }
                }
            }

            ImGui.Columns(3);
            ImGui.BeginChild("params");
            foreach (var param in ParamBank.Params)
            {
                if (ImGui.Selectable(param.Key, param.Key == _selection.getActiveParam()))
                {
                    _selection.setActiveParam(param.Key);
                    //_selection.SetActiveRow(null);
                }
                if (doFocus && param.Key == _selection.getActiveParam())
                {
                    ImGui.SetScrollHereY();
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();
            if (!_selection.paramSelectionExists())
            {
                ImGui.BeginChild("rowsNONE");
                ImGui.Text("Select a param to see rows");
            }
            else
            {
                ImGui.Text("id VALUE | name ROW | prop FIELD VALUE | propref FIELD ROW");
                ImGui.InputText("Search rows...", ref _selection.getCurrentSearchString(), 256);
                if(ImGui.IsItemActive())
                    _isSearchBarActive = true;
                else
                    _isSearchBarActive = false;
                ImGui.BeginChild("rows"+_selection.getActiveParam());
                IParamDecorator decorator = null;
                if (_decorators.ContainsKey(_selection.getActiveParam()))
                {
                    decorator = _decorators[_selection.getActiveParam()];
                }

                PARAM para = ParamBank.Params[_selection.getActiveParam()];
                List<PARAM.Row> p;
                Match m = new Regex(MassParamEditRegex.rowfilterRx).Match(_selection.getCurrentSearchString());
                if (!m.Success)
                    p = para.Rows;
                else
                    p = MassParamEditRegex.GetMatchingParamRows(para, m, true, true);

                foreach (var r in p)
                {
                    if (ImGui.Selectable($@"{r.ID} {r.Name}", _selection.getSelectedRows().Contains(r)))
                    {
                        if (InputTracker.GetKey(Key.LControl))
                        {
                            _selection.toggleRowInSelection(r);
                        }
                        else
                        {
                            if (InputTracker.GetKey(Key.LShift))
                            {
                                _selection.cleanSelectedRows();
                                int start = p.IndexOf(_selection.getActiveRow());
                                int end = p.IndexOf(r);
                                if (start != end)
                                {
                                    foreach (var r2 in p.GetRange(start < end ? start : end, Math.Abs(end - start)))
                                        _selection.addRowToSelection(r2);
                                }
                                _selection.addRowToSelection(r);
                            }
                            else
                                _selection.SetActiveRow(r);
                        }
                    }
                    if (decorator != null)
                    {
                        decorator.DecorateContextMenu(r);
                        decorator.DecorateParam(r);
                    }
                    if (doFocus && _selection.getActiveRow() == r)
                    {
                        ImGui.SetScrollHereY();
                    }
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();
            if (!_selection.rowSelectionExists())
            {
                ImGui.BeginChild("columnsNONE");
                ImGui.Text("Select a row to see properties");
            }
            else
            {
                ImGui.BeginChild("columns"+_selection.getActiveParam());
                _propEditor.PropEditorParamRow(_selection.getActiveRow());
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
                    if (!_selection.rowSelectionExists())
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
            _selection.cleanAllSelectionState();
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
