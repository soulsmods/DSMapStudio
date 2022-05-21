using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using ImGuiNET;
using SoulsFormats;
using StudioCore;
using StudioCore.TextEditor;
using StudioCore.Editor;

namespace StudioCore.ParamEditor
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
        private static Vector4 FMGLINKCOLOUR = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);

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
                ImGui.PushStyleColor(ImGuiCol.Text, FMGLINKCOLOUR);
                ImGui.TextUnformatted($@" <{entry.Text}>");
                ImGui.PopStyleColor();
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

        private List<ParamEditorView> _views;
        private ParamEditorView _activeView;

        // Clipboard vars
        private string _clipboardParam = null;
        private List<PARAM.Row> _clipboardRows = new List<PARAM.Row>();
        private long _clipboardBaseRow = 0;
        private bool _ctrlVuseIndex = false;
        private string _currentCtrlVValue = "0";
        private string _currentCtrlVOffset = "0";

        // MassEdit Popup vars
        private string _currentMEditRegexInput = "";
        private string _lastMEditRegexInput = "";
        private string _mEditRegexResult = "";
        private bool _currentMEditSingleCSVSpaces = false;
        private string _currentMEditSingleCSVField = "";
        private string _currentMEditCSVInput = "";
        private string _currentMEditCSVOutput = "";
        private string _mEditCSVResult = "";
        private bool _mEditCSVAppendOnly = false;
        private bool _mEditCSVReplaceRows = false;

        public static bool ShowAltNamesPreference = true;
        public static bool AlwaysShowOriginalNamePreference = false;
        public static bool HideReferenceRowsPreference = false;
        public static bool HideEnumsPreference = false;
        public static bool AllowFieldReorderPreference = true;
        public static bool AlphabeticalParamsPreference = true;
        public static bool ShowVanillaParamsPreference = false;
        public static bool EditorMode = false;

        internal bool _isSearchBarActive = false;
        private bool _isMEditPopupOpen = false;
        private bool _isShortcutPopupOpen = false;

        internal Dictionary<string, IParamDecorator> _decorators = new Dictionary<string, IParamDecorator>();

        private ProjectSettings _projectSettings = null;
        public ParamEditorScreen(Sdl2Window window, GraphicsDevice device)
        {
            _views = new List<ParamEditorView>();
            _views.Add(new ParamEditorView(this, 0));
            _activeView = _views[0];
            ResetFMGDecorators();
        }

        public void ResetFMGDecorators()
        {
            _decorators.Clear();
            _decorators.Add("EquipParamAccessory", new FMGItemParamDecorator(FMGBank.ItemCategory.Rings));
            _decorators.Add("EquipParamGoods", new FMGItemParamDecorator(FMGBank.ItemCategory.Goods));
            _decorators.Add("EquipParamProtector", new FMGItemParamDecorator(FMGBank.ItemCategory.Armor));
            _decorators.Add("EquipParamWeapon", new FMGItemParamDecorator(FMGBank.ItemCategory.Weapons));
            _decorators.Add("EquipParamGem", new FMGItemParamDecorator(FMGBank.ItemCategory.Gem));
            _decorators.Add("SwordArtsParam", new FMGItemParamDecorator(FMGBank.ItemCategory.SwordArts));
        }

        public override void DrawEditorMenu()
        {
            // Menu Options
            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Undo", "Ctrl+Z", false, EditorActionManager.CanUndo()))
                {
                    EditorActionManager.UndoAction();
                }
                if (ImGui.MenuItem("Redo", "Ctrl+Y", false, EditorActionManager.CanRedo()))
                {
                    EditorActionManager.RedoAction();
                }
                if (ImGui.MenuItem("Copy", "Ctrl+C", false, _activeView._selection.rowSelectionExists()))
                {
                    CopySelectionToClipboard();
                }
                if (ImGui.MenuItem("Paste", "Ctrl+V", false, _clipboardRows.Any()))
                {
                    EditorCommandQueue.AddCommand($@"param/menu/ctrlVPopup");
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Mass Edit"))
                {
                    EditorCommandQueue.AddCommand($@"param/menu/massEditRegex");
                }
                if (ImGui.BeginMenu("Export CSV", _activeView._selection.rowSelectionExists()))
                {
                    if (ImGui.MenuItem("All"))
                        EditorCommandQueue.AddCommand($@"param/menu/massEditCSVExport");
                    if (ImGui.MenuItem("Name"))
                        EditorCommandQueue.AddCommand($@"param/menu/massEditSingleCSVExport/Name");
                    if (ImGui.BeginMenu("Field"))
                    {
                        foreach (PARAMDEF.Field field in ParamBank.Params[_activeView._selection.getActiveParam()].AppliedParamdef.Fields)
                        {
                            if (ImGui.MenuItem(field.InternalName))
                                EditorCommandQueue.AddCommand($@"param/menu/massEditSingleCSVExport/{field.InternalName}");
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Import CSV", _activeView._selection.paramSelectionExists()))
                {
                    if (ImGui.MenuItem("All"))
                        EditorCommandQueue.AddCommand($@"param/menu/massEditCSVImport");
                    if (ImGui.MenuItem("Name"))
                        EditorCommandQueue.AddCommand($@"param/menu/massEditSingleCSVImport/Name");
                    if (ImGui.BeginMenu("Field"))
                    {
                        foreach (PARAMDEF.Field field in ParamBank.Params[_activeView._selection.getActiveParam()].AppliedParamdef.Fields)
                        {
                            if (ImGui.MenuItem(field.InternalName))
                                EditorCommandQueue.AddCommand($@"param/menu/massEditSingleCSVImport/{field.InternalName}");
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.MenuItem("Sort rows by ID", _activeView._selection.paramSelectionExists()))
                {
                    EditorActionManager.ExecuteAction(MassParamEditOther.SortRows(_activeView._selection.getActiveParam()));
                }
                if (ImGui.MenuItem("Load Default Row Names"))
                {
                    try {
                        EditorActionManager.ExecuteAction(ParamBank.LoadParamDefaultNames());
                    } catch {
                    }
                }
                if (ImGui.MenuItem("Trim hidden newlines in names"))
                {
                    try {
                        EditorActionManager.PushSubManager(ParamBank.TrimNewlineChrsFromNames());
                    } catch {
                    }
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("View"))
            {
                if (ImGui.MenuItem("New View"))
                {
                    AddView();
                }
                if (ImGui.MenuItem("Close View", null, false, CountViews() > 1))
                {
                    RemoveView(_activeView);
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Check all params for edits (Slow!)", null, false, !ParamBank.IsLoadingVParams))
                {
                    ParamBank.refreshParamDirtyCache();
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Show alternate field names", null, ShowAltNamesPreference))
                    ShowAltNamesPreference = !ShowAltNamesPreference;
                if (ImGui.MenuItem("Always show original field names", null, AlwaysShowOriginalNamePreference))
                    AlwaysShowOriginalNamePreference = !AlwaysShowOriginalNamePreference;
                if (ImGui.MenuItem("Hide field references", null, HideReferenceRowsPreference))
                    HideReferenceRowsPreference = !HideReferenceRowsPreference;
                if (ImGui.MenuItem("Hide field enums", null, HideEnumsPreference))
                    HideEnumsPreference = !HideEnumsPreference;
                if (ImGui.MenuItem("Allow field reordering", null, AllowFieldReorderPreference))
                    AllowFieldReorderPreference = !AllowFieldReorderPreference;
                if (ImGui.MenuItem("Sort Params Alphabetically", null, AlphabeticalParamsPreference))
                    AlphabeticalParamsPreference = !AlphabeticalParamsPreference;
                if (ImGui.MenuItem("Show Vanilla Params", null, ShowVanillaParamsPreference))
                    ShowVanillaParamsPreference = !ShowVanillaParamsPreference;
                ImGui.Separator();
                if (!EditorMode && ImGui.MenuItem("Editor Mode", null, EditorMode))
                    EditorMode = true;
                if (EditorMode && ImGui.BeginMenu("Editor Mode"))
                {
                    if (ImGui.MenuItem("Save Changes"))
                    {
                        ParamMetaData.SaveAll();
                        EditorMode = false;
                    }
                    if (ImGui.MenuItem("Discard Changes"))
                        EditorMode = false;
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Game"))
            {
                if (ImGui.BeginMenu("Hot Reload Params"))
                {
                    if (ImGui.MenuItem("Current Param", "F5", false, _projectSettings != null && (_projectSettings.GameType == GameType.DarkSoulsIII || _projectSettings.GameType == GameType.EldenRing) && ParamBank.IsLoadingParams == false))
                    {
                        ParamReloader.ReloadMemoryParams(ParamBank.AssetLocator, new string[]{_activeView._selection.getActiveParam()});
                    }
                    if (ImGui.MenuItem("All Params", "Shift-F5", false, _projectSettings != null && (_projectSettings.GameType == GameType.DarkSoulsIII || _projectSettings.GameType == GameType.EldenRing) && ParamBank.IsLoadingParams == false))
                    {
                        ParamReloader.ReloadMemoryParams(ParamBank.AssetLocator, ParamBank.Params.Keys.ToArray());
                    }
                    foreach (string param in ParamReloader.GetReloadableParams(ParamBank.AssetLocator))
                    {
                        if (ImGui.MenuItem(param, "", false, _projectSettings != null && (_projectSettings.GameType == GameType.DarkSoulsIII || _projectSettings.GameType == GameType.EldenRing) && ParamBank.IsLoadingParams == false))
                        {
                            ParamReloader.ReloadMemoryParams(ParamBank.AssetLocator, new string[]{param});
                        }
                    }
                    ImGui.EndMenu();
                }
                string activeParam = _activeView._selection.getActiveParam();
                if (activeParam != null && _projectSettings.GameType == GameType.DarkSoulsIII)
                {
                    ParamReloader.GiveItemMenu(ParamBank.AssetLocator, _activeView._selection.getSelectedRows(), _activeView._selection.getActiveParam());
                }
                ImGui.EndMenu();
            }
        }

        public void CopySelectionToClipboard()
        {
            _clipboardParam = _activeView._selection.getActiveParam();
            _clipboardRows.Clear();
            long baseValue = long.MaxValue;
            _activeView._selection.sortSelection();
            foreach (PARAM.Row r in _activeView._selection.getSelectedRows())
            {
                _clipboardRows.Add(new PARAM.Row(r));// make a clone
                if (r.ID < baseValue)
                    baseValue = r.ID;
            }
            _clipboardBaseRow = baseValue;
            _currentCtrlVValue = _clipboardBaseRow.ToString();
        }

        public void OpenMassEditPopup(string popup)
        {
            ImGui.OpenPopup(popup);
            _isMEditPopupOpen = true;
        }

        public void MassEditPopups()
        {
            // Popup size relies on magic numbers. Multiline maxlength is also arbitrary.
            if (ImGui.BeginPopup("massEditMenuRegex"))
            {
                ImGui.Text("param PARAM: id VALUE: FIELD: = VALUE;");
                UIHints.AddImGuiHintButton("MassEditHint", ref UIHints.MassEditHint);
                ImGui.InputTextMultiline("MEditRegexInput", ref _currentMEditRegexInput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4));
                if (ImGui.BeginCombo("###", ""))
                {
                    string target = _currentMEditRegexInput.Split('\n').LastOrDefault();
                    foreach (string option in MassParamEditRegex.GetRegexAutocomplete(target, _activeView._selection.getActiveParam()))
                    {
                        if (ImGui.Selectable(target + option))
                        {
                            _currentMEditRegexInput = _currentMEditRegexInput + option;
                        }
                    }
                    ImGui.EndCombo();
                }

                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    _activeView._selection.sortSelection();
                    (MassEditResult r, ActionManager child) = MassParamEditRegex.PerformMassEdit(_currentMEditRegexInput, _activeView._selection.getActiveParam(), _activeView._selection.getSelectedRows());
                    if (child != null)
                        EditorActionManager.PushSubManager(child);
                    if (r.Type == MassEditResultType.SUCCESS)
                    {
                        _lastMEditRegexInput = _currentMEditRegexInput;
                        _currentMEditRegexInput = "";
                        TaskManager.Run("PB:RefreshDirtyCache", false, true, () => ParamBank.refreshParamDirtyCache());
                    }
                    _mEditRegexResult = r.Information;
                }
                ImGui.Text(_mEditRegexResult);
                ImGui.InputTextMultiline("MEditRegexOutput", ref _lastMEditRegexInput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4), ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuCSVExport"))
            {
                ImGui.InputTextMultiline("MEditOutput", ref _currentMEditCSVOutput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4), ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuSingleCSVExport"))
            {
                ImGui.Text(_currentMEditSingleCSVField);
                ImGui.InputTextMultiline("MEditOutput", ref _currentMEditCSVOutput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4), ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuCSVImport"))
            {
                ImGui.InputTextMultiline("MEditRegexInput", ref _currentMEditCSVInput, 256 * 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4));
                ImGui.Checkbox("Append new rows instead of ID based insertion (this will create out-of-order IDs)", ref _mEditCSVAppendOnly);
                if (_mEditCSVAppendOnly)
                    ImGui.Checkbox("Replace existing rows instead of updating them (they will be moved to the end)", ref _mEditCSVReplaceRows);
                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    MassEditResult r = MassParamEditCSV.PerformMassEdit(_currentMEditCSVInput, EditorActionManager, _activeView._selection.getActiveParam(), _mEditCSVAppendOnly, _mEditCSVAppendOnly && _mEditCSVReplaceRows);
                    if (r.Type == MassEditResultType.SUCCESS)
                    {
                        TaskManager.Run("PB:RefreshDirtyCache", false, true, () => ParamBank.refreshParamDirtyCache());
                    }
                    _mEditCSVResult = r.Information;
                }
                ImGui.Text(_mEditCSVResult);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuSingleCSVImport"))
            {
                ImGui.Text(_currentMEditSingleCSVField);
                ImGui.Checkbox("Space separator", ref _currentMEditSingleCSVSpaces);
                ImGui.InputTextMultiline("MEditRegexInput", ref _currentMEditCSVInput, 256 * 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4));
                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    (MassEditResult r, CompoundAction a) = MassParamEditCSV.PerformSingleMassEdit(_currentMEditCSVInput, _activeView._selection.getActiveParam(), _currentMEditSingleCSVField, _currentMEditSingleCSVSpaces);
                    if (a != null)
                        EditorActionManager.ExecuteAction(a);
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
                    TaskManager.Run("PB:RefreshDirtyCache", false, true, () => ParamBank.refreshParamDirtyCache());
                }
                if (EditorActionManager.CanRedo() && InputTracker.GetControlShortcut(Key.Y))
                {
                    EditorActionManager.RedoAction();
                    TaskManager.Run("PB:RefreshDirtyCache", false, true, () => ParamBank.refreshParamDirtyCache());
                }
                if (!ImGui.IsAnyItemActive() && _activeView._selection.paramSelectionExists() && InputTracker.GetControlShortcut(Key.A))
                {
                    _clipboardParam = _activeView._selection.getActiveParam();
                    Match m = new Regex(MassParamEditRegex.rowfilterRx).Match(_activeView._selection.getCurrentRowSearchString());
                    if (!m.Success)
                    {
                        foreach (PARAM.Row row in ParamBank.Params[_activeView._selection.getActiveParam()].Rows)
                            _activeView._selection.addRowToSelection(row);
                    }
                    else
                    {
                        foreach (PARAM.Row row in MassParamEditRegex.GetMatchingParamRows(ParamBank.Params[_activeView._selection.getActiveParam()], m, true, true))
                            _activeView._selection.addRowToSelection(row);
                    }
                }
                if (!ImGui.IsAnyItemActive() && _activeView._selection.rowSelectionExists() && InputTracker.GetControlShortcut(Key.C))
                {
                    CopySelectionToClipboard();
                }
                if (_clipboardRows.Count > 00 && _clipboardParam == _activeView._selection.getActiveParam() && !ImGui.IsAnyItemActive() && InputTracker.GetControlShortcut(Key.V))
                {
                    ImGui.OpenPopup("ctrlVPopup");
                }
                if (!ImGui.IsAnyItemActive() && InputTracker.GetKeyDown(Key.Delete))
                {
                    if (_activeView._selection.rowSelectionExists())
                    {
                        var act = new DeleteParamsAction(ParamBank.Params[_activeView._selection.getActiveParam()], _activeView._selection.getSelectedRows());
                        EditorActionManager.ExecuteAction(act);
                        _activeView._selection.SetActiveRow(null, true);
                    }
                }
            }

            if (InputTracker.GetKey(Key.F5) && _projectSettings != null && (_projectSettings.GameType == GameType.DarkSoulsIII || _projectSettings.GameType == GameType.EldenRing) && ParamBank.IsLoadingParams == false)
            {
                if (InputTracker.GetKey(Key.ShiftLeft) || InputTracker.GetKey(Key.ShiftRight))
                    ParamReloader.ReloadMemoryParams(ParamBank.AssetLocator, ParamBank.Params.Keys.ToArray());
                else
                    ParamReloader.ReloadMemoryParams(ParamBank.AssetLocator, new string[]{_activeView._selection.getActiveParam()});
            }

            if (ParamBank.Params == null)
            {
                if (ParamBank.IsLoadingParams)
                {
                    ImGui.Text("Loading...");
                }
                return;
            }
            
            // Parse commands
            bool doFocus = false;
            // Parse select commands
            if (initcmd != null)
            {
                if (initcmd[0] == "select" || initcmd[0] == "view")
                {
                    if (initcmd.Length > 2 && ParamBank.Params.ContainsKey(initcmd[2]))
                    {
                        doFocus = initcmd[0] == "select";
                        if (_activeView._selection.getActiveRow() != null && !ParamBank.IsLoadingVParams)
                            ParamBank.refreshParamRowDirtyCache(_activeView._selection.getActiveRow(), ParamBank.VanillaParams[_activeView._selection.getActiveParam()], ParamBank.DirtyParamCache[_activeView._selection.getActiveParam()]);

                        ParamEditorView viewToMofidy = _activeView;
                        if (initcmd[1].Equals("new"))
                            viewToMofidy = AddView();
                        else
                        {
                            int cmdIndex = -1;
                            bool parsable = int.TryParse(initcmd[1], out cmdIndex);
                            if (parsable && cmdIndex >= 0 && cmdIndex < _views.Count)
                                viewToMofidy = _views[cmdIndex];
                        }
                        _activeView = viewToMofidy;
                        
                        viewToMofidy._selection.setActiveParam(initcmd[2]);
                        if (initcmd.Length > 3)
                        {
                            viewToMofidy._selection.SetActiveRow(null, doFocus);
                            var p = ParamBank.Params[viewToMofidy._selection.getActiveParam()];
                            int id;
                            var parsed = int.TryParse(initcmd[3], out id);
                            if (parsed)
                            {
                                var r = p.Rows.FirstOrDefault(r => r.ID == id);
                                if (r != null)
                                {
                                    viewToMofidy._selection.SetActiveRow(r, doFocus);
                                }
                            }
                        }
                        if (_activeView._selection.getActiveRow() != null && !ParamBank.IsLoadingVParams)
                            ParamBank.refreshParamRowDirtyCache(_activeView._selection.getActiveRow(), ParamBank.VanillaParams[_activeView._selection.getActiveParam()], ParamBank.DirtyParamCache[_activeView._selection.getActiveParam()]);

                    }
                }
                else if (initcmd[0] == "search")
                {
                    if (initcmd.Length > 1)
                    {
                        _activeView._selection.getCurrentRowSearchString() = initcmd[1];
                    }
                }
                else if (initcmd[0] == "menu" && initcmd.Length > 1)
                {
                    if (initcmd[1] == "ctrlVPopup")
                    {
                        ImGui.OpenPopup("ctrlVPopup");
                    }
                    else if (initcmd[1] == "massEditRegex")
                    {
                        _currentMEditRegexInput = initcmd.Length > 2 ? initcmd[2] : _currentMEditRegexInput;
                        OpenMassEditPopup("massEditMenuRegex");
                    }
                    else if (initcmd[1] == "massEditCSVExport")
                    {
                        _activeView._selection.sortSelection();
                        if (_activeView._selection.rowSelectionExists())
                            _currentMEditCSVOutput = MassParamEditCSV.GenerateCSV(_activeView._selection.getSelectedRows());
                        OpenMassEditPopup("massEditMenuCSVExport");
                    }
                    else if (initcmd[1] == "massEditCSVImport")
                    {
                        OpenMassEditPopup("massEditMenuCSVImport");
                    }
                    else if (initcmd[1] == "massEditSingleCSVExport" && initcmd.Length > 2)
                    {
                        _activeView._selection.sortSelection();
                        _currentMEditSingleCSVField = initcmd[2];
                        if (_activeView._selection.rowSelectionExists())
                            _currentMEditCSVOutput = MassParamEditCSV.GenerateSingleCSV(_activeView._selection.getSelectedRows(), _currentMEditSingleCSVField);
                        OpenMassEditPopup("massEditMenuSingleCSVExport");
                    }
                    else if (initcmd[1] == "massEditSingleCSVImport" && initcmd.Length > 2)
                    {
                        _currentMEditSingleCSVField = initcmd[2];
                        OpenMassEditPopup("massEditMenuSingleCSVImport");
                    }
                }
            }

            ShortcutPopups();
            MassEditPopups();

            if (CountViews() == 1)
            {
                _activeView.ParamView(doFocus);
            }
            else
            {
                ImGui.DockSpace(ImGui.GetID("DockSpace_ParamEditorViews"));
                foreach (ParamEditorView view in _views)
                {
                    if (view == null)
                        continue;
                    string name = view._selection.getActiveRow() != null ? view._selection.getActiveRow().Name : null;
                    string toDisplay = (view == _activeView ? "**" : "") + (name == null || name.Trim().Equals("") ? "Param Editor View" : Utils.ImGuiEscape(name, "null")) + (view == _activeView ? "**" : "");
                    ImGui.SetNextWindowSize(new Vector2(1280.0f, 720.0f), ImGuiCond.Once);
                    ImGui.SetNextWindowDockID(ImGui.GetID("DockSpace_ParamEditorViews"), ImGuiCond.Once);
                    ImGui.Begin($@"{toDisplay}###ParamEditorView##{view._viewIndex}");
                    if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                        _activeView = view;
                    if (ImGui.BeginPopupContextItem())
                    {
                        if (ImGui.MenuItem("Close View"))
                        {
                            RemoveView(view);
                            ImGui.EndMenu();
                            break; //avoid concurrent modification
                        }
                        ImGui.EndMenu();
                    }
                    view.ParamView(doFocus && view == _activeView);
                    ImGui.End();
                }
            }
        }

        public void ShortcutPopups()
        {
            if (ImGui.BeginPopup("ctrlVPopup"))
            {
                _isShortcutPopupOpen = true;
                try
                {
                    int max = -1;
                    long offset = 0;
                    ImGui.Checkbox("Paste after selection", ref _ctrlVuseIndex);
                    if (_ctrlVuseIndex)
                    {
                        ImGui.Text("Note: You may produce out-of-order or duplicate rows. These may confuse later ID-based row additions.");
                        List<PARAM.Row> rows = _activeView._selection.getSelectedRows();
                        PARAM param = ParamBank.Params[_activeView._selection.getActiveParam()];
                        foreach (PARAM.Row r in rows)
                        {
                            max = param.Rows.IndexOf(r) > max ? param.Rows.IndexOf(r) : max;
                        }
                    }
                    else
                    {
                        ImGui.InputText("Row", ref _currentCtrlVValue, 20);
                        if (ImGui.IsItemEdited())
                        {
                            offset = long.Parse(_currentCtrlVValue) - _clipboardBaseRow;
                            _currentCtrlVOffset = offset.ToString();
                        }
                        ImGui.InputText("Offset", ref _currentCtrlVOffset, 20);
                        if (ImGui.IsItemEdited())
                        {
                            offset = long.Parse(_currentCtrlVOffset);
                            _currentCtrlVValue = (_clipboardBaseRow + offset).ToString();
                        }
                        // Recheck that this is valid
                        offset = long.Parse(_currentCtrlVValue);
                        offset = long.Parse(_currentCtrlVOffset);
                    }
                    int index = 1;
                    if (ImGui.Selectable("Submit"))
                    {
                        List<PARAM.Row> rowsToInsert = new List<PARAM.Row>();
                        foreach (PARAM.Row r in _clipboardRows)
                        {
                            PARAM.Row newrow = new PARAM.Row(r);// more cloning
                            if (_ctrlVuseIndex)
                                newrow.ID = (int) (max+index);
                            else
                                newrow.ID = (int) (r.ID + offset);
                            rowsToInsert.Add(newrow);
                            index++;
                        }
                        EditorActionManager.ExecuteAction(new AddParamsAction(ParamBank.Params[_clipboardParam], "legacystring", rowsToInsert, false, false, _ctrlVuseIndex));
                    }
                }
                catch
                {
                    ImGui.EndPopup();
                    return;
                }
                ImGui.EndPopup();
            }
            else
            {
                _isShortcutPopupOpen = false;
            }

        }

        public ParamEditorView AddView()
        {
            int index = 0;
            while (index < _views.Count)
            {
                if (_views[index] == null)
                    break;
                index++;
            }
            ParamEditorView view = new ParamEditorView(this, index);
            if (index < _views.Count)
                _views[index] = view;
            else
                _views.Add(view);
            _activeView = view;
            return view;
        }

        public bool RemoveView(ParamEditorView view)
        {
            if (!_views.Contains(view))
                return false;
            _views[view._viewIndex] = null;
            if (view == _activeView || _activeView == null)
            {
                _activeView = _views.FindLast(e => e != null);
            }
            return true;
        }

        public int CountViews()
        {
            return _views.Where(e => e != null).Count();
        }

        public override void OnProjectChanged(ProjectSettings newSettings)
        {
            _projectSettings = newSettings;
            foreach (ParamEditorView view in _views)
            {
                if (view != null)
                    view._selection.cleanAllSelectionState();
            }
        }

        public override void Save()
        {
            if (_projectSettings != null)
            {
                ParamBank.SaveParams(_projectSettings.UseLooseParams, _projectSettings.PartialParams);
            }
        }

        public override void SaveAll()
        {
            if (_projectSettings != null)
            {
                ParamBank.SaveParams(_projectSettings.UseLooseParams, _projectSettings.PartialParams);
            }
        }
    }

    internal class ParamEditorSelectionState
    {
        private static string _globalRowSearchString = "";
        private static string _globalPropSearchString = "";
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
            if (!_paramStates.ContainsKey(_activeParam))
                _paramStates.Add(_activeParam, new ParamEditorParamSelectionState());
        }
        public ref string getCurrentRowSearchString()
        {
            if (_activeParam == null)
                return ref _globalRowSearchString;
            return ref _paramStates[_activeParam].currentRowSearchString;
        }
        public ref string getCurrentPropSearchString()
        {
            if (_activeParam == null)
                return ref _globalPropSearchString;
            return ref _paramStates[_activeParam].currentPropSearchString;
        }
        public bool rowSelectionExists()
        {
            return _activeParam != null && _paramStates[_activeParam].activeRow != null;
        }
        public PARAM.Row getActiveRow()
        {
            if (_activeParam == null)
                return null;
            return _paramStates[_activeParam].activeRow;
        }
        public void SetActiveRow(PARAM.Row row, bool clearSelection)
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                s.activeRow = row;
                s.selectionRows.Clear();
                s.selectionRows.Add(row);
            }
        }
        public void toggleRowInSelection(PARAM.Row row)
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                if (s.selectionRows.Contains(row))
                    s.selectionRows.Remove(row);
                else
                    s.selectionRows.Add(row);
            }
        }
        public void addRowToSelection(PARAM.Row row)
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                if (!s.selectionRows.Contains(row))
                    s.selectionRows.Add(row);
            }
        }
        public void removeRowFromSelection(PARAM.Row row)
        {
            if (_activeParam != null)
                _paramStates[_activeParam].selectionRows.Remove(row);
        }
        public List<PARAM.Row> getSelectedRows()
        {
            if (_activeParam == null)
                return null;
            return _paramStates[_activeParam].selectionRows;
        }
        public void cleanSelectedRows()
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                s.selectionRows.Clear();
                if (s.activeRow != null)
                    s.selectionRows.Add(s.activeRow);
            }
        }
        public void cleanAllSelectionState()
        {
            _activeParam = null;
            _paramStates.Clear();
        }
        public void sortSelection()
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                PARAM p = ParamBank.Params[_activeParam];
                s.selectionRows.Sort((PARAM.Row a, PARAM.Row b) => {return p.Rows.IndexOf(a) - p.Rows.IndexOf(b);});
            }
        }
    }

    internal class ParamEditorParamSelectionState
    {
        internal string currentRowSearchString = "";
        internal string currentPropSearchString = "";
        internal PARAM.Row activeRow = null;
        internal List<PARAM.Row> selectionRows = new List<PARAM.Row>();
    }

    public class ParamEditorView
    {
        private static Vector4 DIRTYCOLOUR = new Vector4(0.7f,1,0.7f,1);
        private static Vector4 CLEANCOLOUR = new Vector4(0.9f,0.9f,0.9f,1);
        private static Regex ROWFILTERMATCHER = new Regex(MassParamEditRegex.rowfilterRx);

        private ParamEditorScreen _paramEditor;
        internal int _viewIndex;

        internal ParamEditorSelectionState _selection = new ParamEditorSelectionState();

        private PropertyEditor _propEditor = null;

        public ParamEditorView(ParamEditorScreen parent, int index)
        {
            _paramEditor = parent;
            _viewIndex = index;
            _propEditor = new PropertyEditor(parent.EditorActionManager);
        }

        public void ParamView(bool doFocus)
        {
            ImGui.Columns(3);
            ImGui.BeginChild("params");
            float scrollTo = 0f;
            List<string> paramKeyList = ParamBank.Params.Keys.ToList();
            if (ParamEditorScreen.AlphabeticalParamsPreference)
                paramKeyList.Sort();
            foreach (var paramKey in paramKeyList)
            {
                if (ImGui.Selectable(paramKey, paramKey == _selection.getActiveParam()))
                {
                    //_selection.setActiveParam(param.Key);
                    EditorCommandQueue.AddCommand($@"param/view/{_viewIndex}/{paramKey}");
                }
                if (doFocus && paramKey == _selection.getActiveParam())
                    scrollTo = ImGui.GetCursorPosY();
            }
            if (doFocus)
                ImGui.SetScrollFromPosY(scrollTo - ImGui.GetScrollY());
            ImGui.EndChild();
            ImGui.NextColumn();
            string activeParam = _selection.getActiveParam();
            if (!_selection.paramSelectionExists())
            {
                ImGui.BeginChild("rowsNONE");
                ImGui.Text("Select a param to see rows");
            }
            else
            {
                PARAM para = ParamBank.Params[activeParam];
                HashSet<int> dirtyCache = ParamBank.DirtyParamCache[activeParam];
                ImGui.Text("id VALUE | name ROW | prop FIELD VALUE | propref FIELD ROW | original | modified");
                UIHints.AddImGuiHintButton("MassEditHint", ref UIHints.SearchBarHint);
                ImGui.InputText("Search rows...", ref _selection.getCurrentRowSearchString(), 256);
                if (ImGui.IsItemActive())
                    _paramEditor._isSearchBarActive = true;
                else
                    _paramEditor._isSearchBarActive = false;
                ImGui.BeginChild("rows" + activeParam);
                IParamDecorator decorator = null;
                if (_paramEditor._decorators.ContainsKey(activeParam))
                {
                    decorator = _paramEditor._decorators[activeParam];
                }
                List<PARAM.Row> p;
                Match m = ROWFILTERMATCHER.Match(_selection.getCurrentRowSearchString());
                if (!m.Success)
                {
                    p = para.Rows;
                }
                else
                {
                    p = MassParamEditRegex.GetMatchingParamRows(para, m, true, true);
                }

                scrollTo = 0;
                foreach (var r in p)
                {
                    if (dirtyCache != null && dirtyCache.Contains(r.ID))
                        ImGui.PushStyleColor(ImGuiCol.Text, DIRTYCOLOUR);
                    else
                        ImGui.PushStyleColor(ImGuiCol.Text, CLEANCOLOUR);
                    if (ImGui.Selectable($@"{r.ID} {Utils.ImGuiEscape(r.Name, "")}", _selection.getSelectedRows().Contains(r)))
                    {
                        if (InputTracker.GetKey(Key.LControl))
                        {
                            _selection.toggleRowInSelection(r);
                        }
                        else
                        {
                            if (InputTracker.GetKey(Key.LShift) && _selection.getActiveRow() != null)
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
                                //_selection.SetActiveRow(r);
                                EditorCommandQueue.AddCommand($@"param/view/{_viewIndex}/{activeParam}/{r.ID}");
                        }
                    }
                    ImGui.PopStyleColor();
                    if (decorator != null)
                    {
                        decorator.DecorateContextMenu(r);
                        decorator.DecorateParam(r);
                    }
                    if (doFocus && _selection.getActiveRow() == r)
                        scrollTo = ImGui.GetCursorPosY();
                }
                if (doFocus)
                    ImGui.SetScrollFromPosY(scrollTo - ImGui.GetScrollY());
            }
            PARAM.Row activeRow = _selection.getActiveRow();
            ImGui.EndChild();
            ImGui.NextColumn();
            if (activeRow == null)
            {
                ImGui.BeginChild("columnsNONE");
                ImGui.Text("Select a row to see properties");
            }
            else
            {
                ImGui.BeginChild("columns" + activeParam);
                _propEditor.PropEditorParamRow(activeRow, ParamBank.VanillaParams != null ? ParamBank.VanillaParams[activeParam][activeRow.ID] : null, ref _selection.getCurrentPropSearchString());
            }
            ImGui.EndChild();
        }
    }
}
