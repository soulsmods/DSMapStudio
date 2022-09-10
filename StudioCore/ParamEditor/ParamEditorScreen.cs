using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FSParam;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using ImGuiNET;
using SoulsFormats;
using StudioCore;
using StudioCore.TextEditor;
using StudioCore.Editor;
using StudioCore.MsbEditor;
using ActionManager = StudioCore.Editor.ActionManager;
using AddParamsAction = StudioCore.Editor.AddParamsAction;
using CompoundAction = StudioCore.Editor.CompoundAction;
using DeleteParamsAction = StudioCore.Editor.DeleteParamsAction;
using EditorScreen = StudioCore.Editor.EditorScreen;

namespace StudioCore.ParamEditor
{
    /// <summary>
    /// Interface for decorating param rows with additional information (such as english
    /// strings sourced from FMG files)
    /// </summary>
    public interface IParamDecorator
    {
        public void DecorateParam(Param.Row row);

        public void DecorateContextMenuItems(Param.Row row);

        public void ClearDecoratorCache();
    }

    public class FMGItemParamDecorator : IParamDecorator
    {
        private static Vector4 FMGLINKCOLOUR = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);

        private FMGBank.FmgEntryCategory _category = FMGBank.FmgEntryCategory.None;

        private Dictionary<int, FMG.Entry> _entryCache = new Dictionary<int, FMG.Entry>();

        public FMGItemParamDecorator(FMGBank.FmgEntryCategory cat)
        {
            _category = cat;
        }

        private void PopulateDecorator()
        {
            if (_entryCache.Count == 0 && FMGBank.IsLoaded)
            {
                var fmgEntries = FMGBank.GetFmgEntriesByType(_category, FMGBank.FmgEntryTextType.Title, false);
                foreach (var fmgEntry in fmgEntries)
                {
                    _entryCache[fmgEntry.ID] = fmgEntry;
                }
            }
        }

        public void ClearDecoratorCache()
        {
            _entryCache.Clear();
        }

        public void DecorateParam(Param.Row row)
        {
            PopulateDecorator();
            FMG.Entry entry = null;
            _entryCache.TryGetValue(row.ID, out entry);

            if (entry != null)
            {
                ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Text, FMGLINKCOLOUR);
                ImGui.TextUnformatted($@" <{entry.Text}>");
                ImGui.PopStyleColor();
            }
        }

        public void DecorateContextMenuItems(Param.Row row)
        {
            PopulateDecorator();
            if (!_entryCache.ContainsKey((int)row.ID))
            {
                return;
            }
            if (ImGui.Selectable($@"Goto {_category.ToString()} Text"))
            {
                EditorCommandQueue.AddCommand($@"text/select/{_category.ToString()}/{row.ID}");
            }
        }
    }

    public class ParamEditorScreen : EditorScreen
    {
        public ActionManager EditorActionManager = new ActionManager();

        internal List<ParamEditorView> _views;
        internal ParamEditorView _activeView;

        // Clipboard vars
        private string _clipboardParam = null;
        private List<Param.Row> _clipboardRows = new List<Param.Row>();
        private long _clipboardBaseRow = 0;
        private bool _ctrlVuseIndex = false;
        private string _currentCtrlVValue = "0";
        private string _currentCtrlVOffset = "0";

        // MassEdit Popup vars
        private string _currentMEditRegexInput = "";
        private string _lastMEditRegexInput = "";
        private string _mEditRegexResult = "";
        private string _currentMEditSingleCSVField = "";
        private string _currentMEditCSVInput = "";
        private string _currentMEditCSVOutput = "";
        private string _mEditCSVResult = "";
        private bool _mEditCSVAppendOnly = false;
        private bool _mEditCSVReplaceRows = false;

        public static bool EditorMode = false;

        internal bool _isSearchBarActive = false;
        private bool _isMEditPopupOpen = false;
        private bool _isShortcutPopupOpen = false;

        internal Dictionary<string, IParamDecorator> _decorators = new Dictionary<string, IParamDecorator>();

        internal ProjectSettings _projectSettings = null;
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
            _decorators.Add("EquipParamAccessory", new FMGItemParamDecorator(FMGBank.FmgEntryCategory.Rings));
            _decorators.Add("EquipParamGoods", new FMGItemParamDecorator(FMGBank.FmgEntryCategory.Goods));
            _decorators.Add("EquipParamProtector", new FMGItemParamDecorator(FMGBank.FmgEntryCategory.Armor));
            _decorators.Add("EquipParamWeapon", new FMGItemParamDecorator(FMGBank.FmgEntryCategory.Weapons));
            _decorators.Add("EquipParamGem", new FMGItemParamDecorator(FMGBank.FmgEntryCategory.Gem));
            _decorators.Add("SwordArtsParam", new FMGItemParamDecorator(FMGBank.FmgEntryCategory.SwordArts));
        }
        
        public void UpgradeRegulation(string oldRegulation)
        {
            var conflicts = new Dictionary<string, HashSet<int>>();
            var result = ParamBank.UpgradeRegulation(oldRegulation, conflicts);

            if (result == ParamBank.ParamUpgradeResult.OldRegulationNotFound)
            {
                System.Windows.Forms.MessageBox.Show(
                    $@"Unable to load old vanilla regulation.", 
                    "Loading error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
            if (result == ParamBank.ParamUpgradeResult.OldRegulationVersionMismatch)
            {
                System.Windows.Forms.MessageBox.Show(
                    $@"The version of the vanilla regulation you selected does not match the version of your mod.", 
                    "Version mismatch",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            if (result == ParamBank.ParamUpgradeResult.RowConflictsFound)
            {
                // If there's row conflicts write a conflict log
                var logPath = $@"{ParamBank.AssetLocator.GameModDirectory}\regulationUpgradeLog.txt";
                if (File.Exists(logPath))
                {
                    File.Delete(logPath);
                }

                using StreamWriter logWriter = new StreamWriter(logPath);
                logWriter.WriteLine("The following rows have conflicts (i.e. both you and the game update added these rows).");
                logWriter.WriteLine("The conflicting rows have been overwritten with your modded version, but it is recommended");
                logWriter.WriteLine("that you review these rows and potentially move them to new IDs and try merging again");
                logWriter.WriteLine("instead of saving your upgraded regulation right away.");
                logWriter.WriteLine();
                foreach (var c in conflicts)
                {
                    logWriter.WriteLine($@"{c.Key}:");
                    foreach (var r in c.Value)
                    {
                        logWriter.WriteLine($@"    {r}");
                    }
                    logWriter.WriteLine();
                }
                logWriter.Flush();
                
                var msgRes = System.Windows.Forms.MessageBox.Show(
                    $@"Conflicts were found while upgrading params. This is usually caused by a game updating adding " +
                    "a new row that has the same ID as the one that you added in your mod. It is recommended that you " +
                    "review these conflicts and handle them before saving. You can revert to your original params by " +
                    "reloading your project by saving and move the conflicting rows to new IDs, or you can chance it by " +
                    "trying to fix the current post-merge result. Currently your mod added rows will have overwritten" +
                    "the added rows in the vanilla regulation.\n\nThe list of conflicts can be found in regulationUpgradeLog.txt" +
                    "in your mod project directory. Would you like to open them now?",
                    "Row conflicts found",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Warning);
                if (msgRes == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer",
                        Arguments = "\"" + logPath + "\""
                    });
                }
            }

            if (result == ParamBank.ParamUpgradeResult.Success)
            {
                var msgRes = System.Windows.Forms.MessageBox.Show(
                    "Upgrade successful",
                    "Success",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);
            }
            
            EditorActionManager.Clear();
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
                if (ImGui.MenuItem("Copy", "Ctrl+C", false, _activeView._selection.rowSelectionExists())) //TODO: hotkey
                {
                    CopySelectionToClipboard();
                }
                if (ImGui.MenuItem("Paste", "Ctrl+V", false, _clipboardRows.Any())) //TODO: hotkey
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
                    DelimiterInputText();

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
                    if (ImGui.BeginMenu("To File...", _activeView._selection.rowSelectionExists()))
                    {
                        if (ImGui.MenuItem("All"))
                        {
                            _activeView._selection.sortSelection();
                            var rbrowseDlg = new System.Windows.Forms.SaveFileDialog()
                            {
                                Filter = "CSV file (*.CSV)|*.CSV|Text file (*.txt) |*.TXT|All Files|*.*",
                                ValidateNames = true,
                                CheckPathExists = true
                            };
                            if (rbrowseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                File.WriteAllText(rbrowseDlg.FileName, MassParamEditCSV.GenerateCSV(_activeView._selection.getSelectedRows(), ParamBank.Params[_activeView._selection.getActiveParam()], CFG.Current.Param_Export_Delimiter[0]));
                        }
                        if (ImGui.MenuItem("Name"))
                        {
                            _activeView._selection.sortSelection();
                            var rbrowseDlg = new System.Windows.Forms.SaveFileDialog()
                            {
                                Filter = "CSV file (*.CSV)|*.CSV|Text file (*.txt) |*.TXT|All Files|*.*",
                                ValidateNames = true,
                                CheckPathExists = true
                            };
                            if (rbrowseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                File.WriteAllText(rbrowseDlg.FileName, MassParamEditCSV.GenerateSingleCSV(_activeView._selection.getSelectedRows(), ParamBank.Params[_activeView._selection.getActiveParam()], "Name", CFG.Current.Param_Export_Delimiter[0]));
                        }
                        if (ImGui.BeginMenu("Field"))
                        {
                            foreach (PARAMDEF.Field field in ParamBank.Params[_activeView._selection.getActiveParam()].AppliedParamdef.Fields)
                            {
                                if (ImGui.MenuItem(field.InternalName))
                                {
                                    _activeView._selection.sortSelection();
                                    var rbrowseDlg = new System.Windows.Forms.SaveFileDialog()
                                    {
                                        Filter = "CSV file (*.CSV)|*.CSV|Text file (*.txt) |*.TXT|All Files|*.*",
                                        ValidateNames = true,
                                        CheckPathExists = true
                                    };
                                    if (rbrowseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                        File.WriteAllText(rbrowseDlg.FileName, MassParamEditCSV.GenerateSingleCSV(_activeView._selection.getSelectedRows(), ParamBank.Params[_activeView._selection.getActiveParam()], field.InternalName, CFG.Current.Param_Export_Delimiter[0]));
                                }
                            }
                            ImGui.EndMenu();
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Import CSV", _activeView._selection.paramSelectionExists()))
                {
                    DelimiterInputText();
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
                    if (ImGui.BeginMenu("From file...", _activeView._selection.paramSelectionExists()))
                    {
                        if (ImGui.MenuItem("All"))
                        {
                            var rbrowseDlg = new System.Windows.Forms.OpenFileDialog()
                            {
                                Filter = "CSV file (*.CSV)|*.CSV|Text file (*.txt) |*.TXT|All Files|*.*",
                                ValidateNames = true,
                                CheckFileExists = true
                            };
                            if (rbrowseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                MassEditResult r = MassParamEditCSV.PerformMassEdit(File.ReadAllText(rbrowseDlg.FileName), EditorActionManager, _activeView._selection.getActiveParam(), false, false, CFG.Current.Param_Export_Delimiter[0]);
                                if (r.Type == MassEditResultType.SUCCESS)
                                    TaskManager.Run("PB:RefreshDirtyCache", false, true, true, () => ParamBank.refreshParamDirtyCache());
                                else
                                    System.Windows.Forms.MessageBox.Show(r.Information, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.None);
                            }
                        }
                        if (ImGui.MenuItem("Name"))
                        {
                            var rbrowseDlg = new System.Windows.Forms.OpenFileDialog()
                            {
                                Filter = "CSV file (*.CSV)|*.CSV|Text file (*.txt) |*.TXT|All Files|*.*",
                                ValidateNames = true,
                                CheckFileExists = true
                            };
                            if (rbrowseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                (MassEditResult r, CompoundAction a) = MassParamEditCSV.PerformSingleMassEdit(File.ReadAllText(rbrowseDlg.FileName), _activeView._selection.getActiveParam(), "Name", CFG.Current.Param_Export_Delimiter[0], false);
                                if (r.Type == MassEditResultType.SUCCESS && a != null)
                                    EditorActionManager.ExecuteAction(a);
                                else
                                    System.Windows.Forms.MessageBox.Show(r.Information, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.None);
                                TaskManager.Run("PB:RefreshDirtyCache", false, true, true, () => ParamBank.refreshParamDirtyCache());
                            }
                        }
                        if (ImGui.BeginMenu("Field"))
                        {
                            foreach (PARAMDEF.Field field in ParamBank.Params[_activeView._selection.getActiveParam()].AppliedParamdef.Fields)
                            {
                                if (ImGui.MenuItem(field.InternalName))
                                {
                                    var rbrowseDlg = new System.Windows.Forms.OpenFileDialog()
                                    {
                                        Filter = "CSV file (*.CSV)|*.CSV|Text file (*.txt) |*.TXT|All Files|*.*",
                                        ValidateNames = true,
                                        CheckFileExists = true
                                    };
                                    if (rbrowseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                    {
                                        (MassEditResult r, CompoundAction a) = MassParamEditCSV.PerformSingleMassEdit(File.ReadAllText(rbrowseDlg.FileName), _activeView._selection.getActiveParam(), field.InternalName, CFG.Current.Param_Export_Delimiter[0], false);
                                        if (r.Type == MassEditResultType.SUCCESS && a != null)
                                            EditorActionManager.ExecuteAction(a);
                                        else
                                            System.Windows.Forms.MessageBox.Show(r.Information, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.None);
                                        TaskManager.Run("PB:RefreshDirtyCache", false, true, true, () => ParamBank.refreshParamDirtyCache());
                                    }
                                }
                            }
                            ImGui.EndMenu();
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.MenuItem("Sort rows by ID", _activeView._selection.paramSelectionExists()))
                {
                    EditorActionManager.ExecuteAction(MassParamEditOther.SortRows(_activeView._selection.getActiveParam()));
                }
                if (ImGui.MenuItem("Load Default Row Names", "", false, ParamBank.Params != null))
                {
                    try {
                        EditorActionManager.ExecuteAction(ParamBank.LoadParamDefaultNames());
                    } catch {
                    }
                }
                if (ImGui.MenuItem("Trim hidden newlines in names", "", false, ParamBank.Params != null))
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
                if (ImGui.MenuItem("Show alternate field names", null, CFG.Current.Param_ShowAltNames))
                    CFG.Current.Param_ShowAltNames = !CFG.Current.Param_ShowAltNames;
                if (ImGui.MenuItem("Always show original field names", null, CFG.Current.Param_AlwaysShowOriginalName))
                    CFG.Current.Param_AlwaysShowOriginalName = !CFG.Current.Param_AlwaysShowOriginalName;
                if (ImGui.MenuItem("Hide field references", null, CFG.Current.Param_HideReferenceRows))
                    CFG.Current.Param_HideReferenceRows = !CFG.Current.Param_HideReferenceRows;
                if (ImGui.MenuItem("Hide field enums", null, CFG.Current.Param_HideEnums))
                    CFG.Current.Param_HideEnums = !CFG.Current.Param_HideEnums;
                if (ImGui.MenuItem("Allow field reordering", null, CFG.Current.Param_AllowFieldReorder))
                    CFG.Current.Param_AllowFieldReorder = !CFG.Current.Param_AllowFieldReorder;
                if (ImGui.MenuItem("Sort Params Alphabetically", null, CFG.Current.Param_AlphabeticalParams))
                {
                    CFG.Current.Param_AlphabeticalParams = !CFG.Current.Param_AlphabeticalParams;
                    CacheBank.ClearCaches();
                }
                if (ImGui.MenuItem("Show Vanilla Params", null, CFG.Current.Param_ShowVanillaParams))
                    CFG.Current.Param_ShowVanillaParams = !CFG.Current.Param_ShowVanillaParams;
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
                    bool canHotReload = ParamReloader.CanReloadMemoryParams(_projectSettings);
                    if (ImGui.MenuItem("Current Param", "F5", false, canHotReload))
                    {
                        ParamReloader.ReloadMemoryParams(ParamBank.AssetLocator, new string[]{_activeView._selection.getActiveParam()});
                    }
                    if (ImGui.MenuItem("All Params", "Shift+F5", false, canHotReload))
                    {
                        ParamReloader.ReloadMemoryParams(ParamBank.AssetLocator, ParamBank.Params.Keys.ToArray());
                    }
                    foreach (string param in ParamReloader.GetReloadableParams(ParamBank.AssetLocator))
                    {
                        if (ImGui.MenuItem(param, "", false, canHotReload))
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
            if (ImGui.BeginMenu("Help"))
            {
                if (ImGui.BeginMenu("Search and MassEdit"))
                {
                    if (ImGui.BeginMenu("Search"))
                    {
                        ImGui.TextUnformatted(UIHints.SearchBarHint);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("MassEdit"))
                    {
                        ImGui.TextUnformatted(UIHints.MassEditHint);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Examples"))
                    {
                        ImGui.TextUnformatted(UIHints.MassEditExamples);
                        ImGui.Separator();
                        ImGui.TextUnformatted(UIHints.SearchExamples);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Regex"))
                    {
                        ImGui.TextUnformatted(UIHints.RegexCheatSheet);
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }
            
            // Param upgrading for Elden Ring
            if (ParamBank.AssetLocator.Type == GameType.EldenRing &&
                ParamBank.IsDefsLoaded && ParamBank.Params != null && ParamBank.VanillaParams != null &&
                !ParamBank.IsLoadingParams && !ParamBank.IsLoadingVParams &&
                ParamBank.ParamVersion < ParamBank.VanillaParamVersion)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.0f, 1f, 0f, 1.0f));
                if (ImGui.Button("Upgrade Params"))
                {
                    var message = System.Windows.Forms.MessageBox.Show(
                        $@"Your mod is currently on regulation version {ParamBank.ParamVersion} while the game is on param version " +
                        $"{ParamBank.VanillaParamVersion}.\n\nWould you like to attempt to upgrade your mod's params to be based on the " +
                        "latest game version? Params will be upgraded by copying all rows that you modified to the new regulation, " +
                        "overwriting exiting rows if needed.\n\nIf both you and the game update added a row with the same ID, the merge " +
                        "will fail and there will be a log saying what rows you will need to manually change the ID of before trying " +
                        "to merge again.\n\nIn order to perform this operation, you must specify the original regulation on the version " +
                        $"that your current mod is based on (version {ParamBank.ParamVersion}.\n\n Once done, the upgraded params will appear" +
                        "in the param editor where you can view and save them, but this operation is not undoable. " +
                        "Would you like to continue?", "Regulation upgrade",
                        System.Windows.Forms.MessageBoxButtons.OKCancel,
                        System.Windows.Forms.MessageBoxIcon.Question);
                    if (message == System.Windows.Forms.DialogResult.OK)
                    {
                        var rbrowseDlg = new System.Windows.Forms.OpenFileDialog()
                        {
                            Filter = AssetLocator.ParamFilter,
                            ValidateNames = true,
                            CheckFileExists = true,
                            CheckPathExists = true,
                        };
                            
                        if (rbrowseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            var path = rbrowseDlg.FileName;
                            UpgradeRegulation(path);
                        }
                    }
                }
                ImGui.PopStyleColor();
            }
        }

        public void CopySelectionToClipboard()
        {
            _clipboardParam = _activeView._selection.getActiveParam();
            _clipboardRows.Clear();
            long baseValue = long.MaxValue;
            _activeView._selection.sortSelection();
            foreach (Param.Row r in _activeView._selection.getSelectedRows())
            {
                _clipboardRows.Add(new Param.Row(r));// make a clone
                if (r.ID < baseValue)
                    baseValue = r.ID;
            }
            _clipboardBaseRow = baseValue;
            _currentCtrlVValue = _clipboardBaseRow.ToString();
        }

        private static void DelimiterInputText()
        {
            string displayDelimiter = CFG.Current.Param_Export_Delimiter;
            if (displayDelimiter == "\t")
                displayDelimiter = "\\t";

            if (ImGui.InputText("Delimiter", ref displayDelimiter, 2))
            {
                if (displayDelimiter == "\\t")
                    displayDelimiter = "\t";
                CFG.Current.Param_Export_Delimiter = displayDelimiter;
            }
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
                ImGui.InputTextMultiline("##MEditRegexInput", ref _currentMEditRegexInput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4));

                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    _activeView._selection.sortSelection();
                    (MassEditResult r, ActionManager child) = MassParamEditRegex.PerformMassEdit(_currentMEditRegexInput, _activeView._selection);
                    if (child != null)
                        EditorActionManager.PushSubManager(child);
                    if (r.Type == MassEditResultType.SUCCESS)
                    {
                        _lastMEditRegexInput = _currentMEditRegexInput;
                        _currentMEditRegexInput = "";
                        TaskManager.Run("PB:RefreshDirtyCache", false, true, true, () => ParamBank.refreshParamDirtyCache());
                    }
                    _mEditRegexResult = r.Information;
                }
                ImGui.Text(_mEditRegexResult);
                ImGui.InputTextMultiline("##MEditRegexOutput", ref _lastMEditRegexInput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4), ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuCSVExport"))
            {
                ImGui.InputTextMultiline("##MEditOutput", ref _currentMEditCSVOutput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4), ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuSingleCSVExport"))
            {
                ImGui.Text(_currentMEditSingleCSVField);
                ImGui.InputTextMultiline("##MEditOutput", ref _currentMEditCSVOutput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4), ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuCSVImport"))
            {
                ImGui.InputTextMultiline("##MEditRegexInput", ref _currentMEditCSVInput, 256 * 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4));
                ImGui.Checkbox("Append new rows instead of ID based insertion (this will create out-of-order IDs)", ref _mEditCSVAppendOnly);
                if (_mEditCSVAppendOnly)
                    ImGui.Checkbox("Replace existing rows instead of updating them (they will be moved to the end)", ref _mEditCSVReplaceRows);
                DelimiterInputText();
                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    MassEditResult r = MassParamEditCSV.PerformMassEdit(_currentMEditCSVInput, EditorActionManager, _activeView._selection.getActiveParam(), _mEditCSVAppendOnly, _mEditCSVAppendOnly && _mEditCSVReplaceRows, CFG.Current.Param_Export_Delimiter[0]);
                    if (r.Type == MassEditResultType.SUCCESS)
                    {
                        TaskManager.Run("PB:RefreshDirtyCache", false, true, true, () => ParamBank.refreshParamDirtyCache());
                    }
                    _mEditCSVResult = r.Information;
                }
                ImGui.Text(_mEditCSVResult);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuSingleCSVImport"))
            {
                ImGui.Text(_currentMEditSingleCSVField);
                ImGui.InputTextMultiline("##MEditRegexInput", ref _currentMEditCSVInput, 256 * 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4));
                DelimiterInputText();
                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    (MassEditResult r, CompoundAction a) = MassParamEditCSV.PerformSingleMassEdit(_currentMEditCSVInput, _activeView._selection.getActiveParam(), _currentMEditSingleCSVField, CFG.Current.Param_Export_Delimiter[0], false);
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
                    TaskManager.Run("PB:RefreshDirtyCache", false, true, true, () => ParamBank.refreshParamDirtyCache());
                }
                if (EditorActionManager.CanRedo() && InputTracker.GetControlShortcut(Key.Y))
                {
                    EditorActionManager.RedoAction();
                    TaskManager.Run("PB:RefreshDirtyCache", false, true, true, () => ParamBank.refreshParamDirtyCache());
                }
                if (!ImGui.IsAnyItemActive() && _activeView._selection.paramSelectionExists() && InputTracker.GetControlShortcut(Key.A))
                {
                    _clipboardParam = _activeView._selection.getActiveParam();
                    foreach (Param.Row row in CacheBank.GetCached(this, (_activeView._viewIndex, _activeView._selection.getActiveParam()), () =>RowSearchEngine.rse.Search(ParamBank.Params[_activeView._selection.getActiveParam()], _activeView._selection.getCurrentRowSearchString(), true, true)))

                        _activeView._selection.addRowToSelection(row);
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

            if (ParamBank.Params == null)
            {
                if (ParamBank.IsLoadingParams)
                {
                    ImGui.Text("Loading...");
                }
                return;
            }

            //Hot Reload shortcut keys
            if (InputTracker.GetKey(Key.F5) && ParamReloader.CanReloadMemoryParams(_projectSettings))
            {
                if (InputTracker.GetKey(Key.ShiftLeft) || InputTracker.GetKey(Key.ShiftRight))
                    ParamReloader.ReloadMemoryParams(ParamBank.AssetLocator, ParamBank.Params.Keys.ToArray());
                else
                    ParamReloader.ReloadMemoryParams(ParamBank.AssetLocator, new string[] { _activeView._selection.getActiveParam() });
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
                            ParamBank.refreshParamRowDirtyCache(_activeView._selection.getActiveRow(), 
                                ParamBank.VanillaParams[_activeView._selection.getActiveParam()],
                                ParamBank.DirtyParamCache[_activeView._selection.getActiveParam()]);

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
                            ParamBank.refreshParamRowDirtyCache(_activeView._selection.getActiveRow(),
                                ParamBank.VanillaParams[_activeView._selection.getActiveParam()],
                                ParamBank.DirtyParamCache[_activeView._selection.getActiveParam()]);

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
                            _currentMEditCSVOutput = MassParamEditCSV.GenerateCSV(_activeView._selection.getSelectedRows(), ParamBank.Params[_activeView._selection.getActiveParam()], CFG.Current.Param_Export_Delimiter[0]);
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
                            _currentMEditCSVOutput = MassParamEditCSV.GenerateSingleCSV(_activeView._selection.getSelectedRows(), ParamBank.Params[_activeView._selection.getActiveParam()], _currentMEditSingleCSVField, CFG.Current.Param_Export_Delimiter[0]);
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
                _activeView.ParamView(doFocus, true);
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
                    view.ParamView(doFocus && view == _activeView, view == _activeView);
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
                        List<Param.Row> rows = _activeView._selection.getSelectedRows();
                        Param param = ParamBank.Params[_activeView._selection.getActiveParam()];
                        foreach (Param.Row r in rows)
                        {
                            max = param.IndexOfRow(r) > max ? param.IndexOfRow(r) : max;
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
                        List<Param.Row> rowsToInsert = new List<Param.Row>();
                        foreach (Param.Row r in _clipboardRows)
                        {
                            Param.Row newrow = new Param.Row(r);// more cloning
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
            foreach (var dec in _decorators)
            {
                dec.Value.ClearDecoratorCache();
            }
        }

        public override void Save()
        {
            try
            {
                if (_projectSettings != null)
                {
                    ParamBank.SaveParams(_projectSettings.UseLooseParams, _projectSettings.PartialParams);
                }
            }
            catch (SavingFailedException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Wrapped.Message, e.Message,
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.None);
            }
        }

        public override void SaveAll()
        {
            try
            {
                if (_projectSettings != null)
                {
                    ParamBank.SaveParams(_projectSettings.UseLooseParams, _projectSettings.PartialParams);
                }
            }
            catch (SavingFailedException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Wrapped.Message, e.Message,
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.None);
            }
        }
    }

    public class ParamEditorSelectionState
    {
        internal string currentParamSearchString = "";
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
        public Param.Row getActiveRow()
        {
            if (_activeParam == null)
                return null;
            return _paramStates[_activeParam].activeRow;
        }
        public void SetActiveRow(Param.Row row, bool clearSelection)
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                s.activeRow = row;
                s.selectionRows.Clear();
                s.selectionRows.Add(row);
            }
        }
        public void toggleRowInSelection(Param.Row row)
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
        public void addRowToSelection(Param.Row row)
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                if (!s.selectionRows.Contains(row))
                    s.selectionRows.Add(row);
            }
        }
        public void removeRowFromSelection(Param.Row row)
        {
            if (_activeParam != null)
                _paramStates[_activeParam].selectionRows.Remove(row);
        }
        public List<Param.Row> getSelectedRows()
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
                Param p = ParamBank.Params[_activeParam];
                s.selectionRows.Sort((Param.Row a, Param.Row b) => {return p.IndexOfRow(a) - p.IndexOfRow(b);});
            }
        }
    }

    internal class ParamEditorParamSelectionState
    {
        internal string currentRowSearchString = "";
        internal string currentPropSearchString = "";
        internal Param.Row activeRow = null;
        internal List<Param.Row> selectionRows = new List<Param.Row>();
    }

    public class ParamEditorView
    {
        private static Vector4 DIRTYCOLOUR = new Vector4(0.7f,1,0.7f,1);
        private static Vector4 CLEANCOLOUR = new Vector4(0.9f,0.9f,0.9f,1);

        private ParamEditorScreen _paramEditor;
        internal int _viewIndex;
        private int _gotoParamRow = -1;
        private bool _arrowKeyPressed = false;
        private bool _focusRows = false;

        internal ParamEditorSelectionState _selection = new ParamEditorSelectionState();

        private PropertyEditor _propEditor = null;

        private string lastParamSearch = "";
        private Dictionary<string, string> lastRowSearch = new Dictionary<string, string>();

        public ParamEditorView(ParamEditorScreen parent, int index)
        {
            _paramEditor = parent;
            _viewIndex = index;
            _propEditor = new PropertyEditor(parent.EditorActionManager, _paramEditor);
        }

        public void ParamView(bool doFocus, bool isActiveView)
        {
            ImGui.Columns(3);
            ImGui.BeginChild("params");
            if (isActiveView && InputTracker.GetControlShortcut(Key.P))
                ImGui.SetKeyboardFocusHere();
            ImGui.InputText("Search <Ctrl+P>", ref _selection.currentParamSearchString, 256);
            if (!_selection.currentParamSearchString.Equals(lastParamSearch))
            {
                CacheBank.ClearCaches();
                lastParamSearch = _selection.currentParamSearchString;
            }

            List<string> pinnedParamKeyList = new List<string>(_paramEditor._projectSettings.PinnedParams);

            if (pinnedParamKeyList.Count > 0)
            {
                //ImGui.Text("        Pinned Params");
                foreach (var paramKey in pinnedParamKeyList)
                {
                    if (ImGui.Selectable(paramKey, paramKey == _selection.getActiveParam()))
                    {
                        EditorCommandQueue.AddCommand($@"param/view/{_viewIndex}/{paramKey}");
                    }
                    if (ImGui.BeginPopupContextItem())
                    {
                        if (ImGui.Selectable("Unpin " + paramKey))
                            _paramEditor._projectSettings.PinnedParams.Remove(paramKey);
                        ImGui.EndPopup();
                    }
                }
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
            }

            ImGui.BeginChild("paramTypes");
            float scrollTo = 0f;

            List<string> paramKeyList = CacheBank.GetCached(this._paramEditor, _viewIndex, () => {
                var list = ParamSearchEngine.pse.Search(true, _selection.currentParamSearchString, true, true);
                var keyList = list.Select((param) => ParamBank.GetKeyForParam(param)).ToList();
                if (CFG.Current.Param_AlphabeticalParams)
                    keyList.Sort();
                return keyList;
            });

            foreach (var paramKey in paramKeyList)
            {
                if (ImGui.Selectable(paramKey, paramKey == _selection.getActiveParam()))
                {
                    //_selection.setActiveParam(param.Key);
                    EditorCommandQueue.AddCommand($@"param/view/{_viewIndex}/{paramKey}");
                }
                if (doFocus && paramKey == _selection.getActiveParam())
                    scrollTo = ImGui.GetCursorPosY();
                if (ImGui.BeginPopupContextItem())
                {
                    if (ImGui.Selectable("Pin "+paramKey) && !_paramEditor._projectSettings.PinnedParams.Contains(paramKey))
                        _paramEditor._projectSettings.PinnedParams.Add(paramKey);
                    ImGui.EndPopup();
                }
            }

            if (doFocus)
                ImGui.SetScrollFromPosY(scrollTo - ImGui.GetScrollY());
            ImGui.EndChild();
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
                Param para = ParamBank.Params[activeParam];
                HashSet<int> dirtyCache = ParamBank.DirtyParamCache[activeParam];
                IParamDecorator decorator = null;
                if (_paramEditor._decorators.ContainsKey(activeParam))
                {
                    decorator = _paramEditor._decorators[activeParam];
                }
                scrollTo = 0;

                //Goto ID
                if (ImGui.Button("Goto ID <Ctrl+G>") || (isActiveView && InputTracker.GetControlShortcut(Key.G)))
                {
                    ImGui.OpenPopup("gotoParamRow");
                }
                if (ImGui.BeginPopup("gotoParamRow"))
                {
                    int gotorow = 0;
                    ImGui.SetKeyboardFocusHere();
                    ImGui.InputInt("Goto Row ID", ref gotorow);
                    if (ImGui.IsItemDeactivated())
                    {
                        _gotoParamRow = gotorow;
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.EndPopup();
                }

                //Row ID/name search
                if (isActiveView && InputTracker.GetControlShortcut(Key.F))
                    ImGui.SetKeyboardFocusHere();

                ImGui.InputText("Search <Ctrl+F>", ref _selection.getCurrentRowSearchString(), 256);
                if (!lastRowSearch.ContainsKey(_selection.getActiveParam()) || !lastRowSearch[_selection.getActiveParam()].Equals(_selection.getCurrentRowSearchString()))
                {
                    CacheBank.ClearCaches();
                    lastRowSearch[_selection.getActiveParam()] = _selection.getCurrentRowSearchString();
                }

                if (ImGui.IsItemActive())
                    _paramEditor._isSearchBarActive = true;
                else
                    _paramEditor._isSearchBarActive = false;
                UIHints.AddImGuiHintButton("MassEditHint", ref UIHints.SearchBarHint);

                ImGui.BeginChild("pinnedRows");

                List<int> pinnedRowList = new List<int>(_paramEditor._projectSettings.PinnedRows.GetValueOrDefault(activeParam, new List<int>()));
                if (pinnedRowList.Count != 0)
                {

                    foreach (int rowID in pinnedRowList)
                    {
                        Param.Row row = para[rowID];
                        if (row == null)
                        {
                            _paramEditor._projectSettings.PinnedRows.GetValueOrDefault(activeParam, new List<int>()).Remove(rowID);
                            continue;
                        }
                        RowColumnEntry(activeParam, null, para[rowID], dirtyCache, decorator, ref scrollTo, false, true);
                    }

                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();
                }

                // Up/Down arrow key input
                if ((InputTracker.GetKey(Key.Up) || InputTracker.GetKey(Key.Down))
                    && !ImGui.IsAnyItemActive())
                {
                    _arrowKeyPressed = true;
                }
                if (_focusRows)
                {
                    ImGui.SetNextWindowFocus();
                    _arrowKeyPressed = false;
                    _focusRows = false;
                }

                ImGui.BeginChild("rows" + activeParam);
                List<Param.Row> rows = CacheBank.GetCached(this._paramEditor, (_viewIndex, activeParam), () => RowSearchEngine.rse.Search(para, _selection.getCurrentRowSearchString(), true, true));

                // Rows
                foreach (var r in rows)
                {
                    RowColumnEntry(activeParam, rows, r, dirtyCache, decorator, ref scrollTo, doFocus, false);
                }
                if (doFocus)
                    ImGui.SetScrollFromPosY(scrollTo - ImGui.GetScrollY());
                ImGui.EndChild();
            }
            Param.Row activeRow = _selection.getActiveRow();
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
                _propEditor.PropEditorParamRow(activeRow, ParamBank.VanillaParams != null ? ParamBank.VanillaParams[activeParam][activeRow.ID] : null, ref _selection.getCurrentPropSearchString(), activeParam, isActiveView);
            }
            ImGui.EndChild();
        }

        private void RowColumnEntry(string activeParam, List<Param.Row> p, Param.Row r, HashSet<int> dirtyCache, IParamDecorator decorator, ref float scrollTo, bool doFocus, bool isPinned)
        {
            if (dirtyCache != null && dirtyCache.Contains(r.ID))
                ImGui.PushStyleColor(ImGuiCol.Text, DIRTYCOLOUR);
            else
                ImGui.PushStyleColor(ImGuiCol.Text, CLEANCOLOUR);

            var selected = _selection.getSelectedRows().Contains(r);
            if (_gotoParamRow != -1)
            {
                // Goto row was activated. As soon as a corresponding ID is found, change selection to it.
                if (r.ID == _gotoParamRow)
                {
                    selected = true;
                    _selection.SetActiveRow(r, true);
                    _gotoParamRow = -1;
                    ImGui.SetScrollHereY();
                }
            }

            if (ImGui.Selectable($@"{r.ID} {Utils.ImGuiEscape(r.Name, "")}", selected))
            {
                _focusRows = true;
                if (InputTracker.GetKey(Key.LControl))
                {
                    _selection.toggleRowInSelection(r);
                }
                else
                {
                    if (p != null && InputTracker.GetKey(Key.LShift) && _selection.getActiveRow() != null)
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
            if (_arrowKeyPressed && ImGui.IsItemFocused()
                && (r != _selection.getActiveRow()))
            {
                if (InputTracker.GetKey(Key.ControlLeft) || InputTracker.GetKey(Key.ControlRight))
                {
                    // Add to selection
                    _selection.addRowToSelection(r);
                }
                else
                {
                    // Exclusive selection
                    _selection.SetActiveRow(r, true);
                }
                _arrowKeyPressed = false;
            }
            ImGui.PopStyleColor();

            if (ImGui.BeginPopupContextItem(r.ID.ToString()))
            {
                if (decorator != null)
                    decorator.DecorateContextMenuItems(r);
                if (ImGui.Selectable((isPinned ? "Unpin ":"Pin ")+r.ID))
                {
                    if (!_paramEditor._projectSettings.PinnedRows.ContainsKey(activeParam))
                        _paramEditor._projectSettings.PinnedRows.Add(activeParam, new List<int>());
                    List<int> pinned = _paramEditor._projectSettings.PinnedRows[activeParam];
                    if (isPinned)
                        pinned.Remove(r.ID);
                    else if (!pinned.Contains(r.ID))
                       pinned.Add(r.ID);
                }
                ImGui.EndPopup();
            }
            if (decorator != null)
                decorator.DecorateParam(r);
            if (doFocus && _selection.getActiveRow() == r)
                scrollTo = ImGui.GetCursorPosY();
        }
    }
}
