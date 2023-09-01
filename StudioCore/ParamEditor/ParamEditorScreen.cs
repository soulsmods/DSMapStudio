using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
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
using StudioCore.Platform;
using ActionManager = StudioCore.Editor.ActionManager;
using AddParamsAction = StudioCore.Editor.AddParamsAction;
using CompoundAction = StudioCore.Editor.CompoundAction;
using DeleteParamsAction = StudioCore.Editor.DeleteParamsAction;
using EditorScreen = StudioCore.Editor.EditorScreen;
using Key = Veldrid.Key;

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

        private FmgEntryCategory _category = FmgEntryCategory.None;

        private Dictionary<int, FMG.Entry> _entryCache = new Dictionary<int, FMG.Entry>();

        public FMGItemParamDecorator(FmgEntryCategory cat)
        {
            _category = cat;
        }

        private void PopulateDecorator()
        {
            if (_entryCache.Count == 0 && FMGBank.IsLoaded)
            {
                var fmgEntries = FMGBank.GetFmgEntriesByCategory(_category, false);
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
        public string EditorName => "Param Editor";
        public string CommandEndpoint => "param";
        public string SaveType => "Params";

        public readonly AssetLocator AssetLocator;
        public ActionManager EditorActionManager = new ActionManager();

        internal List<ParamEditorView> _views;
        internal ParamEditorView _activeView;

        // Clipboard vars
        private long _clipboardBaseRow = 0;
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

        private IEnumerable<(object, int)> _distributionOutput = null;
        private string _statisticPopupOutput = "";
        private string _statisticPopupParameter = "";
        private string[] _autoFillArgsParse = Enumerable.Repeat("", ParamAndRowSearchEngine.parse.AllCommands().Sum((x) => x.Item2.Length)).ToArray();
        private string[] _autoFillArgsPse = Enumerable.Repeat("", ParamSearchEngine.pse.AllCommands().Sum((x) => x.Item2.Length)).ToArray();
        private string[] _autoFillArgsRse = Enumerable.Repeat("", RowSearchEngine.rse.AllCommands().Sum((x) => x.Item2.Length)).ToArray();
        private string[] _autoFillArgsCse = Enumerable.Repeat("", CellSearchEngine.cse.AllCommands().Sum((x) => x.Item2.Length)).ToArray();
        private string[] _autoFillArgsRop = Enumerable.Repeat("", MERowOperation.rowOps.AvailableCommands().Sum((x) => x.Item2.Length)).ToArray();
        private string[] _autoFillArgsCop = Enumerable.Repeat("", MEValueOperation.valueOps.AvailableCommands().Sum((x) => x.Item2.Length)).ToArray();
        private string[] _autoFillArgsOa = Enumerable.Repeat("", MEOperationArgument.arg.AllArguments().Sum((x) => x.Item2.Length)).ToArray();

        public static bool EditorMode = false;

        public bool GotoSelectedRow = false;
        internal bool _isSearchBarActive = false;
        private bool _isShortcutPopupOpen = false;
        private bool _isMEditPopupOpen = false;
        private bool _isStatisticPopupOpen = false;

        internal Dictionary<string, IParamDecorator> _decorators = new Dictionary<string, IParamDecorator>();

        internal ProjectSettings _projectSettings = null;
        public ParamEditorScreen(Sdl2Window window, GraphicsDevice device, AssetLocator locator)
        {
            AssetLocator = locator;
            _views = new List<ParamEditorView>();
            _views.Add(new ParamEditorView(this, 0));
            _activeView = _views[0];
            ResetFMGDecorators();
        }

        public void ResetFMGDecorators()
        {
            _decorators.Clear();
            _decorators.Add("EquipParamAccessory", new FMGItemParamDecorator(FmgEntryCategory.Rings));
            _decorators.Add("EquipParamGoods", new FMGItemParamDecorator(FmgEntryCategory.Goods));
            _decorators.Add("EquipParamProtector", new FMGItemParamDecorator(FmgEntryCategory.Armor));
            _decorators.Add("EquipParamWeapon", new FMGItemParamDecorator(FmgEntryCategory.Weapons));
            _decorators.Add("EquipParamGem", new FMGItemParamDecorator(FmgEntryCategory.Gem));
            _decorators.Add("SwordArtsParam", new FMGItemParamDecorator(FmgEntryCategory.SwordArts));
            //_decorators.Add("CharacterText", new FMGItemParamDecorator(FmgEntryCategory.Characters)); // TODO: Decorators need to be updated to support text references.
        }

        /// <summary>
        /// Whitelist of games and maximum param version to allow param upgrading.
        /// Used to restrict upgrading before DSMS properly supports it.
        /// </summary>
        public readonly Dictionary<GameType, ulong> ParamUpgrade_Whitelist = new()
        {
            {GameType.EldenRing, 1_10_9_9999L},
            {GameType.ArmoredCoreVI, 0_00_0_9999L},
        };

        private void ParamUpgradeDisplay()
        {
            // Param upgrading for Elden Ring
            if (ParamBank.IsDefsLoaded
                && ParamBank.PrimaryBank.Params != null
                && ParamBank.VanillaBank.Params != null
                && !ParamBank.PrimaryBank.IsLoadingParams
                && !ParamBank.VanillaBank.IsLoadingParams
                && ParamBank.PrimaryBank.ParamVersion < ParamBank.VanillaBank.ParamVersion)
            {
                if (!ParamUpgrade_Whitelist.TryGetValue(ParamBank.PrimaryBank.AssetLocator.Type, out ulong versionThreshold))
                {
                    // Unsupported game type.
                    return;
                }

                if (ParamBank.VanillaBank.ParamVersion <= versionThreshold)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.0f, 1f, 0f, 1.0f));
                    if (ImGui.Button("Upgrade Params"))
                    {
                        var message = PlatformUtils.Instance.MessageBox(
                            $@"Your mod is currently on regulation version {ParamBank.PrimaryBank.ParamVersion} while the game is on param version " +
                            $"{ParamBank.VanillaBank.ParamVersion}.\n\nWould you like to attempt to upgrade your mod's params to be based on the " +
                            "latest game version? Params will be upgraded by copying all rows that you modified to the new regulation, " +
                            "overwriting exiting rows if needed.\n\nIf both you and the game update added a row with the same ID, the merge " +
                            "will fail and there will be a log saying what rows you will need to manually change the ID of before trying " +
                            "to merge again.\n\nIn order to perform this operation, you must specify the original regulation on the version " +
                            $"that your current mod is based on (version {ParamBank.PrimaryBank.ParamVersion}).\n\nOnce done, the upgraded params will appear " +
                            "in the param editor where you can view and save them. This operation is not undoable, but you can reload the project without " +
                            "saving to revert to the un-upgraded params.\n\n" +
                            "Would you like to continue?", "Regulation upgrade",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Information);
                        if (message == DialogResult.OK)
                        {
                            var fileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.RegulationBinFilter);
                            if (fileDialog.IsOk)
                            {
                                var path = fileDialog.Path;
                                UpgradeRegulation(ParamBank.PrimaryBank, ParamBank.VanillaBank, path);
                            }
                        }
                    }
                    ImGui.PopStyleColor();
                }
                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.3f, 0.3f, 1.0f));
                    if (ImGui.BeginMenu("Upgrade Params"))
                    {
                        ImGui.PopStyleColor();
                        ImGui.Text("Param version unsupported, DSMapStudio must be updated first.\nDownload update if available, wait for update otherwise.");
                        ImGui.EndMenu();
                    }
                    else
                    {
                        ImGui.PopStyleColor();
                    }
                }
            }
        }

        public void UpgradeRegulation(ParamBank bank, ParamBank vanillaBank, string oldRegulation)
        {
            var oldVersion = bank.ParamVersion;
            var newVersion = vanillaBank.ParamVersion;

            var conflicts = new Dictionary<string, HashSet<int>>();
            var result = bank.UpgradeRegulation(vanillaBank, oldRegulation, conflicts);

            if (result == ParamBank.ParamUpgradeResult.OldRegulationNotFound)
            {
                PlatformUtils.Instance.MessageBox(
                    $@"Unable to load old vanilla regulation.", 
                    "Loading error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            if (result == ParamBank.ParamUpgradeResult.OldRegulationVersionMismatch)
            {
                PlatformUtils.Instance.MessageBox(
                    $@"The version of the vanilla regulation you selected does not match the version of your mod.", 
                    "Version mismatch",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (result == ParamBank.ParamUpgradeResult.RowConflictsFound)
            {
                // If there's row conflicts write a conflict log
                var logPath = $@"{bank.AssetLocator.GameModDirectory}\regulationUpgradeLog.txt";
                if (File.Exists(logPath))
                {
                    File.Delete(logPath);
                }

                using StreamWriter logWriter = new StreamWriter(logPath);
                logWriter.WriteLine("The following rows have conflicts (i.e. both you and the game update added these rows).");
                logWriter.WriteLine("The conflicting rows have been overwritten with your modded version, but it is recommended");
                logWriter.WriteLine("that you review these rows and move them to new IDs and try merging again");
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
                
                var msgRes = PlatformUtils.Instance.MessageBox(
                    $@"Conflicts were found while upgrading params. This is usually caused by a game update adding " +
                    "a new row that has the same ID as the one that you added in your mod. It is highly recommended that you " +
                    "review these conflicts and handle them before saving. You can revert to your original params by " +
                    "reloading your project without saving. Then you can move the conflicting rows to new IDs. " +
                    "Currently the added rows from your mod will have overwritten " +
                    "the added rows in the vanilla regulation.\n\nThe list of conflicts can be found in regulationUpgradeLog.txt " +
                    "in your mod project directory. Would you like to open them now?",
                    "Row conflicts found",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
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
                var msgUpgradeEdits = PlatformUtils.Instance.MessageBox(
                    $@"MapStudio can automatically perform several edits to keep your params consistent with updates to vanilla params. " +
                    "Would you like to perform these edits?", "Regulation upgrade edits",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);
                if (msgUpgradeEdits == DialogResult.Yes)
                {
                    var (success, fail) = bank.RunUpgradeEdits(oldVersion, newVersion);
                    if (success.Count > 0 || fail.Count > 0)
                        PlatformUtils.Instance.MessageBox(
                            (success.Count > 0 ? "Successfully performed the following edits:\n" + String.Join('\n', success) : "") +
                            (success.Count > 0 && fail.Count > 0 ? "\n" : "") + 
                            (fail.Count > 0 ? "Unable to perform the following edits:\n" + String.Join('\n', fail) : ""),
                            "Regulation upgrade edits",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    CacheBank.ClearCaches();
                    bank.RefreshParamDiffCaches();
                }


                var msgRes = PlatformUtils.Instance.MessageBox(
                    "Upgrade successful",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            
            EditorActionManager.Clear();
        }

        private void ParamUndo()
        {
            EditorActionManager.UndoAction();
            TaskManager.Run(new("Param - Check Differences",
                TaskManager.RequeueType.Repeat, true,
                TaskLogs.LogPriority.Low,
                () => ParamBank.PrimaryBank.RefreshParamDiffCaches()));
        }
        private void ParamRedo()
        {
            EditorActionManager.RedoAction();
            TaskManager.Run(new("Param - Check Differences",
                TaskManager.RequeueType.Repeat, true,
                TaskLogs.LogPriority.Low,
                () => ParamBank.PrimaryBank.RefreshParamDiffCaches()));
        }

        private IReadOnlyList<Param.Row> CsvExportGetRows(ParamBank.RowGetType rowType)
        {
            IReadOnlyList<Param.Row> rows;

            var activeParam = _activeView._selection.getActiveParam();
            if (rowType == ParamBank.RowGetType.AllRows)
            {
                // All rows
                rows = ParamBank.PrimaryBank.Params[activeParam].Rows;
            }
            else if (rowType == ParamBank.RowGetType.ModifiedRows)
            {
                // Modified rows
                HashSet<int> vanillaDiffCache = ParamBank.PrimaryBank.GetVanillaDiffRows(activeParam);
                rows = ParamBank.PrimaryBank.Params[activeParam].Rows.Where(p => vanillaDiffCache.Contains(p.ID)).ToList();
            }
            else if (rowType == ParamBank.RowGetType.SelectedRows)
            {
                // Selected rows
                rows = _activeView._selection.getSelectedRows();
            }
            else
            {
                throw new NotSupportedException();
            }
            return rows;
        }

        /// <summary>
        /// CSV Export DIsplay
        /// </summary>
        private void CsvExportDisplay(ParamBank.RowGetType rowType)
        {
            if (ImGui.BeginMenu("Export to window..."))
            {
                if (ImGui.MenuItem("Export all fields", KeyBindings.Current.Param_ExportCSV.HintText))
                    EditorCommandQueue.AddCommand($@"param/menu/massEditCSVExport/{rowType}");
                if (ImGui.BeginMenu("Export specific field"))
                {
                    if (ImGui.MenuItem("Row name"))
                        EditorCommandQueue.AddCommand($@"param/menu/massEditSingleCSVExport/Name/{rowType}");
                    foreach (PARAMDEF.Field field in ParamBank.PrimaryBank
                                 .Params[_activeView._selection.getActiveParam()].AppliedParamdef.Fields)
                    {
                        if (ImGui.MenuItem(field.InternalName))
                            EditorCommandQueue.AddCommand(
                                $@"param/menu/massEditSingleCSVExport/{field.InternalName}/{rowType}");
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Export to file..."))
            {
                if (ImGui.MenuItem("Export all fields"))
                {
                    var fileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.CombineFilters(AssetLocator.CsvFilter, AssetLocator.TxtFilter));
                    if (fileDialog.IsOk)
                    {
                        var rows = CsvExportGetRows(rowType);
                        TryWriteFile(fileDialog.Path,
                            ParamIO.GenerateCSV(rows,
                                ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()],
                                CFG.Current.Param_Export_Delimiter[0]));
                    }
                }

                if (ImGui.BeginMenu("Export specific field"))
                {
                    if (ImGui.MenuItem("Row name"))
                    {
                        var fileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.CombineFilters(AssetLocator.CsvFilter, AssetLocator.TxtFilter));
                        if (fileDialog.IsOk)
                        {
                            var rows = CsvExportGetRows(rowType);
                            TryWriteFile(fileDialog.Path,
                                    ParamIO.GenerateSingleCSV(rows,
                                        ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()],
                                        "Name",
                                        CFG.Current.Param_Export_Delimiter[0]));
                        }
                    }
                    foreach (PARAMDEF.Field field in ParamBank.PrimaryBank
                                 .Params[_activeView._selection.getActiveParam()].AppliedParamdef.Fields)
                    {
                        if (ImGui.MenuItem(field.InternalName))
                        {
                            var fileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.CombineFilters(AssetLocator.CsvFilter, AssetLocator.TxtFilter));
                            if (fileDialog.IsOk)
                            {
                                var rows = CsvExportGetRows(rowType);
                                TryWriteFile(fileDialog.Path,
                                    ParamIO.GenerateSingleCSV(rows,
                                        ParamBank.PrimaryBank.Params[
                                            _activeView._selection.getActiveParam()],
                                        field.InternalName, CFG.Current.Param_Export_Delimiter[0]));
                            }
                        }
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }
        }

        public void DrawEditorMenu()
        {
            // Menu Options
            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Undo", KeyBindings.Current.Core_Undo.HintText, false, EditorActionManager.CanUndo()))
                {
                    ParamUndo();
                }
                if (ImGui.MenuItem("Redo", KeyBindings.Current.Core_Redo.HintText, false, EditorActionManager.CanRedo()))
                {
                    ParamRedo();
                }
                if (ImGui.MenuItem("Copy", KeyBindings.Current.Param_Copy.HintText, false, _activeView._selection.rowSelectionExists()))
                {
                    CopySelectionToClipboard();
                }
                if (ImGui.MenuItem("Paste", KeyBindings.Current.Param_Paste.HintText, false, ParamBank.ClipboardRows.Any()))
                {
                    EditorCommandQueue.AddCommand($@"param/menu/ctrlVPopup");
                }
                if (ImGui.MenuItem("Delete", KeyBindings.Current.Core_Delete.HintText, false, _activeView._selection.rowSelectionExists()))
                {
                    DeleteSelection();
                }
                if (ImGui.MenuItem("Duplicate", KeyBindings.Current.Core_Duplicate.HintText, false, _activeView._selection.rowSelectionExists()))
                {
                    DuplicateSelection();
                }
                if (ImGui.MenuItem("Goto selected row", KeyBindings.Current.Param_GotoSelectedRow.HintText, false, _activeView._selection.rowSelectionExists()))
                {
                    GotoSelectedRow = true;
                }
                ImGui.Separator();
                if (ImGui.MenuItem($"Mass Edit", KeyBindings.Current.Param_MassEdit.HintText))
                {
                    EditorCommandQueue.AddCommand($@"param/menu/massEditRegex");
                }
                if (ImGui.BeginMenu($"Mass Edit Script"))
                {
                    if (ImGui.Selectable("Open Scripts Folder"))
                    {
                        Process.Start("explorer.exe", AssetLocator.GetScriptAssetsDir());
                    }
                    if (ImGui.Selectable("Reload Scripts"))
                    {
                        MassEditScript.ReloadScripts();
                    }
                    ImGui.Separator();
                    MassEditScript.EditorScreenMenuItems(ref _currentMEditRegexInput);
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Export CSV", _activeView._selection.activeParamExists()))
                {
                    DelimiterInputText();

                    if (ImGui.BeginMenu("Quick action"))
                    {
                        if (ImGui.MenuItem("Export entire param to window", KeyBindings.Current.Param_ExportCSV.HintText))
                            EditorCommandQueue.AddCommand($@"param/menu/massEditCSVExport/0");
                        if (ImGui.MenuItem("Export entire param to file"))
                        {
                            var fileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.CombineFilters(AssetLocator.CsvFilter, AssetLocator.TxtFilter));
                            if (fileDialog.IsOk)
                            {
                                var rows = ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()].Rows;
                                TryWriteFile(fileDialog.Path,
                                    ParamIO.GenerateCSV(rows,
                                        ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()],
                                        CFG.Current.Param_Export_Delimiter[0]));
                            }
                        }
                        ImGui.EndMenu();
                    }

                    ImGui.Separator();
                    if (ImGui.BeginMenu("All rows"))
                    {
                        CsvExportDisplay(ParamBank.RowGetType.AllRows);
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Modified rows", ParamBank.PrimaryBank.GetVanillaDiffRows(_activeView._selection.getActiveParam()).Any()))
                    {
                        CsvExportDisplay(ParamBank.RowGetType.ModifiedRows);
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Selected rows", _activeView._selection.rowSelectionExists()))
                    {
                        CsvExportDisplay(ParamBank.RowGetType.SelectedRows);
                        ImGui.EndMenu();
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Import CSV", _activeView._selection.activeParamExists()))
                {
                    DelimiterInputText();
                    if (ImGui.MenuItem("All fields", KeyBindings.Current.Param_ImportCSV.HintText))
                        EditorCommandQueue.AddCommand($@"param/menu/massEditCSVImport");
                    if (ImGui.MenuItem("Row Name"))
                        EditorCommandQueue.AddCommand($@"param/menu/massEditSingleCSVImport/Name");
                    if (ImGui.BeginMenu("Specific Field"))
                    {
                        foreach (PARAMDEF.Field field in ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()].AppliedParamdef.Fields)
                        {
                            if (ImGui.MenuItem(field.InternalName))
                                EditorCommandQueue.AddCommand($@"param/menu/massEditSingleCSVImport/{field.InternalName}");
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("From file...", _activeView._selection.activeParamExists()))
                    {
                        if (ImGui.MenuItem("All fields"))
                        {
                            var fileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.CombineFilters(AssetLocator.CsvFilter, AssetLocator.TxtFilter));
                            if (fileDialog.IsOk)
                            {
                                string csv = TryReadFile(fileDialog.Path);
                                if (csv != null)
                                {
                                    (string result, CompoundAction action) = ParamIO.ApplyCSV(ParamBank.PrimaryBank, csv, _activeView._selection.getActiveParam(), false, false, CFG.Current.Param_Export_Delimiter[0]);
                                    if (action != null)
                                    {
                                        if (action.HasActions)
                                            EditorActionManager.ExecuteAction(action);
                                        TaskManager.Run(new("Param - Check Differences",
                                            TaskManager.RequeueType.Repeat, true,
                                            TaskLogs.LogPriority.Low,
                                            () => ParamBank.PrimaryBank.RefreshParamDiffCaches()));
                                    }
                                    else
                                        PlatformUtils.Instance.MessageBox(result, "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
                                }
                            }
                        }
                        if (ImGui.MenuItem("Row Name"))
                        {
                            var fileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.CombineFilters(AssetLocator.CsvFilter, AssetLocator.TxtFilter));
                            if (fileDialog.IsOk)
                            {
                                string csv = TryReadFile(fileDialog.Path);
                                if (csv != null)
                                {
                                    (string result, CompoundAction action) = ParamIO.ApplySingleCSV(ParamBank.PrimaryBank, csv, _activeView._selection.getActiveParam(), "Name", CFG.Current.Param_Export_Delimiter[0], false);
                                    if (action != null)
                                        EditorActionManager.ExecuteAction(action);
                                    else
                                        PlatformUtils.Instance.MessageBox(result, "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
                                    TaskManager.Run(new("Param - Check Differences",
                                        TaskManager.RequeueType.Repeat,
                                        true, TaskLogs.LogPriority.Low,
                                        () => ParamBank.PrimaryBank.RefreshParamDiffCaches()));
                                }
                            }
                        }
                        if (ImGui.BeginMenu("Specific Field"))
                        {
                            foreach (PARAMDEF.Field field in ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()].AppliedParamdef.Fields)
                            {
                                if (ImGui.MenuItem(field.InternalName))
                                {
                                    var fileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.CombineFilters(AssetLocator.CsvFilter, AssetLocator.TxtFilter));
                                    if (fileDialog.IsOk)
                                    {
                                        string csv = TryReadFile(fileDialog.Path);
                                        if (csv != null)
                                        {
                                            (string result, CompoundAction action) = ParamIO.ApplySingleCSV(ParamBank.PrimaryBank, csv, _activeView._selection.getActiveParam(), field.InternalName, CFG.Current.Param_Export_Delimiter[0], false);
                                            if (action != null)
                                                EditorActionManager.ExecuteAction(action);
                                            else
                                                PlatformUtils.Instance.MessageBox(result, "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
                                            TaskManager.Run(new("Param - Check Differences",
                                                TaskManager.RequeueType.Repeat,
                                                true, TaskLogs.LogPriority.Low,
                                                () => ParamBank.PrimaryBank.RefreshParamDiffCaches()));
                                        }
                                    }
                                }
                            }
                            ImGui.EndMenu();
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.MenuItem("Sort rows by ID", _activeView._selection.activeParamExists()))
                {
                    EditorActionManager.ExecuteAction(MassParamEditOther.SortRows(ParamBank.PrimaryBank, _activeView._selection.getActiveParam()));
                }
                if (ImGui.BeginMenu("Import row names"))
                {
                    void ImportRowNames(bool currentParamOnly, string title)
                    {
                        const string importRowQuestion = $"Would you like to replace row names with default names defined within DSMapStudio?\n\nSelect \"Yes\" to replace all names, \"No\" to only replace empty names, \"Cancel\" to abort.";
                        string currentParam = currentParamOnly ? _activeView._selection.getActiveParam() : null;
                        DialogResult question = PlatformUtils.Instance.MessageBox(importRowQuestion, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                        switch (question)
                        {
                            case DialogResult.Yes:
                                EditorActionManager.ExecuteAction(ParamBank.PrimaryBank.LoadParamDefaultNames(currentParam));
                                break;
                            case DialogResult.No:
                                EditorActionManager.ExecuteAction(ParamBank.PrimaryBank.LoadParamDefaultNames(currentParam, true));
                                break;
                        }
                    }

                    if (ImGui.MenuItem("All", "", false, ParamBank.PrimaryBank.Params != null))
                    {
                        try {
                            ImportRowNames(false, "Replace all row names?");
                        } catch {
                        }
                    }
                    if (ImGui.MenuItem("Current Param", "", false, ParamBank.PrimaryBank.Params != null && _activeView._selection.activeParamExists()))
                    {
                        try {
                            ImportRowNames(true, $"Replace all row names in {_activeView._selection.getActiveParam()}?");
                        } catch {
                        }
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("Trim hidden newlines in names", "", false, ParamBank.PrimaryBank.Params != null))
                {
                    try {
                        EditorActionManager.PushSubManager(ParamBank.PrimaryBank.TrimNewlineChrsFromNames());
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
                if (ImGui.MenuItem("Go back...", KeyBindings.Current.Param_GotoBack.HintText, false, _activeView._selection.hasHistory()))
                {
                    EditorCommandQueue.AddCommand($@"param/back");
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Check all params for edits", null, false, !ParamBank.PrimaryBank.IsLoadingParams && !ParamBank.VanillaBank.IsLoadingParams))
                {
                    ParamBank.PrimaryBank.RefreshParamDiffCaches();
                }
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

                    ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), "WARNING: Hot Reloader only works for existing row entries.\nGame must be restarted for new rows and modified row IDs.");
                    ImGui.Separator();

                    bool canHotReload = ParamReloader.CanReloadMemoryParams(ParamBank.PrimaryBank, _projectSettings);

                    if (ImGui.MenuItem("Current Param", KeyBindings.Current.Param_HotReload.HintText, false, canHotReload))
                    {
                        ParamReloader.ReloadMemoryParams(ParamBank.PrimaryBank, ParamBank.PrimaryBank.AssetLocator, new string[] { _activeView._selection.getActiveParam() });
                    }
                    if (ImGui.MenuItem("All Params", KeyBindings.Current.Param_HotReloadAll.HintText, false, canHotReload))
                    {
                        ParamReloader.ReloadMemoryParams(ParamBank.PrimaryBank, ParamBank.PrimaryBank.AssetLocator, ParamBank.PrimaryBank.Params.Keys.ToArray());
                    }
                    foreach (string param in ParamReloader.GetReloadableParams(ParamBank.PrimaryBank.AssetLocator))
                    {
                        if (ImGui.MenuItem(param, "", false, canHotReload))
                        {
                            ParamReloader.ReloadMemoryParams(ParamBank.PrimaryBank, ParamBank.PrimaryBank.AssetLocator, new string[] { param });
                        }
                    }
                    ImGui.EndMenu();
                }
                string activeParam = _activeView._selection.getActiveParam();
                if (activeParam != null && _projectSettings.GameType == GameType.DarkSoulsIII)
                {
                    ParamReloader.GiveItemMenu(ParamBank.PrimaryBank.AssetLocator, _activeView._selection.getSelectedRows(), _activeView._selection.getActiveParam());
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Compare"))
            {
                if (ImGui.MenuItem("Show Vanilla Params", null, CFG.Current.Param_ShowVanillaParams))
                {
                    CFG.Current.Param_ShowVanillaParams = !CFG.Current.Param_ShowVanillaParams;
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Clear current row comparison", null, false, _activeView != null && _activeView._selection.getCompareRow() != null))
                {
                    _activeView._selection.SetCompareRow(null);
                }
                if (ImGui.MenuItem("Clear current field comparison", null, false, _activeView != null && _activeView._selection.getCompareCol() != null))
                {
                    _activeView._selection.SetCompareCol(null);
                }
                ImGui.Separator();
                // Only support ER for now
                if (ImGui.MenuItem("Load Params for comparison...", null, false))
                {
                    try
                    {
                        if (_projectSettings.GameType != GameType.DarkSoulsIISOTFS)
                        {
                            var fileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.CombineFilters(AssetLocator.RegulationBinFilter,
                                AssetLocator.Data0Filter,
                                AssetLocator.ParamBndDcxFilter,
                                AssetLocator.ParamBndFilter,
                                AssetLocator.EncRegulationFilter));
                            if (fileDialog.IsOk)
                            {
                                ParamBank.LoadAuxBank(fileDialog.Path, null, null, _projectSettings);
                            }
                        }
                        else
                        {
                            var folderDialog = NativeFileDialogSharp.Dialog.FolderPicker();
                            if (folderDialog.IsOk)
                            {
                                var fileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.CombineFilters(AssetLocator.RegulationBinFilter,
                                    AssetLocator.Data0Filter,
                                    AssetLocator.ParamBndDcxFilter,
                                    AssetLocator.ParamBndFilter,
                                    AssetLocator.EncRegulationFilter));
                                if (fileDialog.IsOk)
                                {
                                    var enemyFileDialog = NativeFileDialogSharp.Dialog.FileOpen(AssetLocator.CombineFilters(AssetLocator.ParamLooseFilter));
                                    if (enemyFileDialog.IsOk)
                                    {
                                        ParamBank.LoadAuxBank(fileDialog.Path, folderDialog.Path,
                                            enemyFileDialog.Path, _projectSettings);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        PlatformUtils.Instance.MessageBox(
                        $@"Unable to load regulation.\n" + e.Message,
                        "Loading error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }
                }
                if (ImGui.BeginMenu("Clear param comparison...", ParamBank.AuxBanks.Count > 0))
                {
                    for (int i = 0; i < ParamBank.AuxBanks.Count; i++)
                    {
                        var pb = ParamBank.AuxBanks.ElementAt(i);
                        if (ImGui.MenuItem(pb.Key))
                        {
                            ParamBank.AuxBanks.Remove(pb.Key);
                            break;
                        }
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.MenuItem("Clear all param comparisons", null, false, ParamBank.AuxBanks.Count > 0))
                {
                    ParamBank.AuxBanks = new Dictionary<string, ParamBank>();
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

            ParamUpgradeDisplay();
        }

        public void CopySelectionToClipboard()
        {
            ParamBank.ClipboardParam = _activeView._selection.getActiveParam();
            ParamBank.ClipboardRows.Clear();
            long baseValue = long.MaxValue;
            _activeView._selection.sortSelection();
            foreach (Param.Row r in _activeView._selection.getSelectedRows())
            {
                ParamBank.ClipboardRows.Add(new Param.Row(r));// make a clone
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

        public void DeleteSelection()
        {
            List<Param.Row> toRemove = new List<Param.Row>(_activeView._selection.getSelectedRows());
            var act = new DeleteParamsAction(ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()], toRemove);
            EditorActionManager.ExecuteAction(act);
            _views.ForEach((view) => {
                if (view != null)
                    toRemove.ForEach((row) => view._selection.removeRowFromAllSelections(row));
            });
        }

        public void DuplicateSelection()
        {
            Param param = ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()];
            List<Param.Row> rows = _activeView._selection.getSelectedRows();
            if (rows.Count == 0)
                return;

            List<Param.Row> rowsToInsert = new();
            foreach (Param.Row r in rows)
            {
                Param.Row newrow = new(r);
                rowsToInsert.Add(newrow);
            }
            EditorActionManager.ExecuteAction(new AddParamsAction(param, "legacystring", rowsToInsert, false, false));
        }

        public void OpenMassEditPopup(string popup)
        {
            ImGui.OpenPopup(popup);
            _isMEditPopupOpen = true;
        }
        public void OpenStatisticPopup(string popup)
        {
            ImGui.OpenPopup(popup);
            _isStatisticPopupOpen = true;
        }

        public void MassEditPopups()
        {
            float scale = MapStudioNew.GetUIScale();
            
            // Popup size relies on magic numbers. Multiline maxlength is also arbitrary.
            if (ImGui.BeginPopup("massEditMenuRegex"))
            {
                ImGui.Text("param PARAM: id VALUE: FIELD: = VALUE;");
                UIHints.AddImGuiHintButton("MassEditHint", ref UIHints.MassEditHint);
                ImGui.InputTextMultiline("##MEditRegexInput", ref _currentMEditRegexInput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4) * scale);

                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    _activeView._selection.sortSelection();
                    (MassEditResult r, ActionManager child) = MassParamEditRegex.PerformMassEdit(ParamBank.PrimaryBank, _currentMEditRegexInput, _activeView._selection);
                    if (child != null)
                        EditorActionManager.PushSubManager(child);
                    if (r.Type == MassEditResultType.SUCCESS)
                    {
                        _lastMEditRegexInput = _currentMEditRegexInput;
                        _currentMEditRegexInput = "";
                        TaskManager.Run(new("Param - Check Differences",
                            TaskManager.RequeueType.Repeat,
                            true, TaskLogs.LogPriority.Low,
                            () => ParamBank.PrimaryBank.RefreshParamDiffCaches()));
                    }
                    _mEditRegexResult = r.Information;
                }
                ImGui.Text(_mEditRegexResult);
                ImGui.InputTextMultiline("##MEditRegexOutput", ref _lastMEditRegexInput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4) * scale, ImGuiInputTextFlags.ReadOnly);
                ImGui.TextUnformatted("Remember to handle clipboard state between edits with the 'clear' command");
                string result = AutoFill.MassEditCompleteAutoFill();
                if (result != null)
                {
                    if (string.IsNullOrWhiteSpace(_currentMEditRegexInput))
                        _currentMEditRegexInput = result;
                    else
                        _currentMEditRegexInput += "\n" + result;
                }
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuCSVExport"))
            {
                ImGui.InputTextMultiline("##MEditOutput", ref _currentMEditCSVOutput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4) * scale, ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuSingleCSVExport"))
            {
                ImGui.Text(_currentMEditSingleCSVField);
                ImGui.InputTextMultiline("##MEditOutput", ref _currentMEditCSVOutput, 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4) * scale, ImGuiInputTextFlags.ReadOnly);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuCSVImport"))
            {
                ImGui.InputTextMultiline("##MEditRegexInput", ref _currentMEditCSVInput, 256 * 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4) * scale);
                ImGui.Checkbox("Append new rows instead of ID based insertion (this will create out-of-order IDs)", ref _mEditCSVAppendOnly);
                if (_mEditCSVAppendOnly)
                    ImGui.Checkbox("Replace existing rows instead of updating them (they will be moved to the end)", ref _mEditCSVReplaceRows);
                DelimiterInputText();
                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    (string result, CompoundAction action) = ParamIO.ApplyCSV(ParamBank.PrimaryBank, _currentMEditCSVInput, _activeView._selection.getActiveParam(), _mEditCSVAppendOnly, _mEditCSVAppendOnly && _mEditCSVReplaceRows, CFG.Current.Param_Export_Delimiter[0]);
                    if (action != null)
                    {
                        if (action.HasActions)
                            EditorActionManager.ExecuteAction(action);
                        TaskManager.Run(new("Param - Check Differences",
                            TaskManager.RequeueType.Repeat, true,
                            TaskLogs.LogPriority.Low,
                            () => ParamBank.PrimaryBank.RefreshParamDiffCaches()));
                    }
                    _mEditCSVResult = result;
                }
                ImGui.Text(_mEditCSVResult);
                ImGui.EndPopup();
            }
            else if (ImGui.BeginPopup("massEditMenuSingleCSVImport"))
            {
                ImGui.Text(_currentMEditSingleCSVField);
                ImGui.InputTextMultiline("##MEditRegexInput", ref _currentMEditCSVInput, 256 * 65536, new Vector2(1024, ImGui.GetTextLineHeightWithSpacing() * 4) * scale);
                DelimiterInputText();
                if (ImGui.Selectable("Submit", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    (string result, CompoundAction action) = ParamIO.ApplySingleCSV(ParamBank.PrimaryBank, _currentMEditCSVInput, _activeView._selection.getActiveParam(), _currentMEditSingleCSVField, CFG.Current.Param_Export_Delimiter[0], false);
                    if (action != null)
                        EditorActionManager.ExecuteAction(action);
                    _mEditCSVResult = result;
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
        public void StatisticPopups()
        {
            if (ImGui.BeginPopup("distributionPopup"))
            {
                ImGui.Text("Occurences in "+_statisticPopupParameter);
                ImGui.Text("Count".PadLeft(9)+" Value");
                ImGui.SameLine();
                try
                {
                    if (ImGui.Button("Sort by count"))
                    {
                        _distributionOutput = _distributionOutput.OrderByDescending((g) => g.Item2);
                        _statisticPopupOutput = String.Join('\n', _distributionOutput.Select((e) => e.Item2.ToString().PadLeft(9) + " " + e.Item1.ToParamEditorString()));
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Sort by value"))
                    {
                        _distributionOutput = _distributionOutput.OrderBy((g) => g.Item1);
                        _statisticPopupOutput = String.Join('\n', _distributionOutput.Select((e) => e.Item2.ToString().PadLeft(9) + " " + e.Item1.ToParamEditorString()));
                    }
                }
                catch (Exception e)
                {
                    // Happily ignore exceptions. This is non-mutating code with no critical use.
                }
                ImGui.Separator();
                ImGui.Text(_statisticPopupOutput);
                ImGui.EndPopup();
            }
            else
            {
                _isStatisticPopupOpen = false;
                _statisticPopupOutput = "";
                _statisticPopupParameter = "";
                _distributionOutput = null;
            }
        }

        public void OnGUI(string[] initcmd)
        {
            float scale = MapStudioNew.GetUIScale();

            if (!_isShortcutPopupOpen && !_isMEditPopupOpen && !_isStatisticPopupOpen && !_isSearchBarActive)
            {
                // Keyboard shortcuts
                if (EditorActionManager.CanUndo() && InputTracker.GetKeyDown(KeyBindings.Current.Core_Undo))
                {
                    ParamUndo();
                }
                if (EditorActionManager.CanRedo() && InputTracker.GetKeyDown(KeyBindings.Current.Core_Redo))
                {
                    ParamRedo();
                }
                if (_activeView._selection.hasHistory() && InputTracker.GetKeyDown(KeyBindings.Current.Param_GotoBack))
                {
                    EditorCommandQueue.AddCommand($@"param/back");
                }
                if (!ImGui.IsAnyItemActive() && _activeView._selection.activeParamExists() && InputTracker.GetKeyDown(KeyBindings.Current.Param_SelectAll))
                {
                    ParamBank.ClipboardParam = _activeView._selection.getActiveParam();
                    foreach (Param.Row row in CacheBank.GetCached(this, (_activeView._viewIndex, _activeView._selection.getActiveParam()), () =>RowSearchEngine.rse.Search((ParamBank.PrimaryBank, ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()]), _activeView._selection.getCurrentRowSearchString(), true, true)))

                        _activeView._selection.addRowToSelection(row);
                }
                if (!ImGui.IsAnyItemActive() && _activeView._selection.rowSelectionExists() && InputTracker.GetKeyDown(KeyBindings.Current.Param_Copy))
                {
                    CopySelectionToClipboard();
                }
                if (ParamBank.ClipboardRows.Count > 00 && ParamBank.ClipboardParam == _activeView._selection.getActiveParam() && !ImGui.IsAnyItemActive() && InputTracker.GetKeyDown(KeyBindings.Current.Param_Paste))
                {
                    ImGui.OpenPopup("ctrlVPopup");
                }
                if (!ImGui.IsAnyItemActive() && _activeView._selection.rowSelectionExists() && InputTracker.GetKeyDown(KeyBindings.Current.Core_Duplicate))
                {
                    DuplicateSelection();
                }
                if (!ImGui.IsAnyItemActive() && _activeView._selection.rowSelectionExists() && InputTracker.GetKeyDown(KeyBindings.Current.Core_Delete))
                {
                    DeleteSelection();
                }
                if (!ImGui.IsAnyItemActive() && _activeView._selection.rowSelectionExists() && InputTracker.GetKeyDown(KeyBindings.Current.Param_GotoSelectedRow))
                {
                    GotoSelectedRow = true;
                }
            }

            if (_projectSettings == null)
            {
                ImGui.Text("No project loaded. File -> New Project");
                return;
            }
            if (ParamBank.PrimaryBank.IsLoadingParams)
            {
                ImGui.Text("Loading Params...");
                return;
            }
            if (ParamBank.PrimaryBank.Params == null)
            {
                ImGui.Text("No params loaded");
                return;
            }
            if (!ParamBank.IsMetaLoaded)
            {
                ImGui.Text("Loading Meta...");
                return;
            }

            //Hot Reload shortcut keys
            if (ParamReloader.CanReloadMemoryParams(ParamBank.PrimaryBank, _projectSettings))
            {
                if (InputTracker.GetKeyDown(KeyBindings.Current.Param_HotReloadAll))
                    ParamReloader.ReloadMemoryParams(ParamBank.PrimaryBank, ParamBank.PrimaryBank.AssetLocator, ParamBank.PrimaryBank.Params.Keys.ToArray());
                else if (InputTracker.GetKeyDown(KeyBindings.Current.Param_HotReload))
                    ParamReloader.ReloadMemoryParams(ParamBank.PrimaryBank, ParamBank.PrimaryBank.AssetLocator, new string[] { _activeView._selection.getActiveParam() });
            }

            if (InputTracker.GetKeyDown(KeyBindings.Current.Param_MassEdit))
                EditorCommandQueue.AddCommand($@"param/menu/massEditRegex");
            if (InputTracker.GetKeyDown(KeyBindings.Current.Param_ImportCSV))
                EditorCommandQueue.AddCommand($@"param/menu/massEditCSVImport");
            if (InputTracker.GetKeyDown(KeyBindings.Current.Param_ExportCSV))
                EditorCommandQueue.AddCommand($@"param/menu/massEditCSVExport");

            // Parse commands
            bool doFocus = false;
            // Parse select commands
            if (initcmd != null)
            {
                if (initcmd[0] == "select" || initcmd[0] == "view")
                {
                    if (initcmd.Length > 2 && ParamBank.PrimaryBank.Params.ContainsKey(initcmd[2]))
                    {
                        doFocus = initcmd[0] == "select";
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
                            var p = ParamBank.PrimaryBank.Params[viewToMofidy._selection.getActiveParam()];
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
                    }
                }
                else if (initcmd[0] == "back")
                {
                    _activeView._selection.popHistory();
                }
                else if (initcmd[0] == "search")
                {
                    if (initcmd.Length > 1)
                    {
                        _activeView._selection.setCurrentRowSearchString(initcmd[1]);
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
                        var rows = CsvExportGetRows(Enum.Parse<ParamBank.RowGetType>(initcmd[2]));
                        _currentMEditCSVOutput = ParamIO.GenerateCSV(rows,
                            ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()],
                            CFG.Current.Param_Export_Delimiter[0]);
                        OpenMassEditPopup("massEditMenuCSVExport");
                    }
                    else if (initcmd[1] == "massEditCSVImport")
                    {
                        OpenMassEditPopup("massEditMenuCSVImport");
                    }
                    else if (initcmd[1] == "massEditSingleCSVExport")
                    {
                        _currentMEditSingleCSVField = initcmd[2];
                        var rows = CsvExportGetRows(Enum.Parse<ParamBank.RowGetType>(initcmd[3]));
                        _currentMEditCSVOutput = ParamIO.GenerateSingleCSV(rows,
                            ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()],
                            _currentMEditSingleCSVField,
                            CFG.Current.Param_Export_Delimiter[0]);
                        OpenMassEditPopup("massEditMenuSingleCSVExport");
                    }
                    else if (initcmd[1] == "massEditSingleCSVImport" && initcmd.Length > 2)
                    {
                        _currentMEditSingleCSVField = initcmd[2];
                        OpenMassEditPopup("massEditMenuSingleCSVImport");
                    }
                    else if (initcmd[1] == "distributionPopup" && initcmd.Length > 2)
                    {
                        Param p = ParamBank.PrimaryBank.GetParamFromName(_activeView._selection.getActiveParam());
                        var col = ParamUtils.GetCol(p, initcmd[2]);
                        _distributionOutput = ParamUtils.GetParamValueDistribution(_activeView._selection.getSelectedRows(), col);
                        _statisticPopupOutput = String.Join('\n', _distributionOutput.Select((e) => e.Item2.ToString().PadLeft(9) + " " + e.Item1.ToParamEditorString()));
                        _statisticPopupParameter = initcmd[2];
                        OpenStatisticPopup("distributionPopup");
                    }
                }
            }

            ShortcutPopups();
            MassEditPopups();
            StatisticPopups();

            if (CFG.Current.UI_CompactParams)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1.0f, 1.0f) * scale);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(5.0f, 1.0f) * scale);
                ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(5.0f, 1.0f) * scale);
            }
            else
            {
                var style = ImGui.GetStyle();
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, style.ItemSpacing * scale - (new Vector2(3.5f, 0f) * scale));
            }
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
                    ImGui.SetNextWindowSize(new Vector2(1280.0f, 720.0f) * scale, ImGuiCond.Once);
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
            if (CFG.Current.UI_CompactParams)
            {
                ImGui.PopStyleVar(3);
            }
            else
            {
                ImGui.PopStyleVar();
            }
        }

        public void ShortcutPopups()
        {
            if (ImGui.BeginPopup("ctrlVPopup"))
            {
                _isShortcutPopupOpen = true;
                try
                {
                    long offset = 0;
                    ImGui.Checkbox("Paste after selection", ref CFG.Current.Param_PasteAfterSelection);
                    var insertIndex = -1;
                    if (CFG.Current.Param_PasteAfterSelection)
                    {
                        //ImGui.Text("Note: Allows out-of-order rows, which may confuse later ID-based row additions.");
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
                    bool disableSubmit = CFG.Current.Param_PasteAfterSelection && !_activeView._selection.rowSelectionExists();
                    if (disableSubmit)
                    {
                        ImGui.TextUnformatted("No selection exists");
                    }
                    else if (ImGui.Button("Submit"))
                    {
                        List<Param.Row> rowsToInsert = new List<Param.Row>();
                        if (!CFG.Current.Param_PasteAfterSelection)
                        {
                            foreach (Param.Row r in ParamBank.ClipboardRows)
                            {
                                Param.Row newrow = new Param.Row(r);// more cloning
                                newrow.ID = (int)(r.ID + offset);
                                rowsToInsert.Add(newrow);
                            }
                        }
                        else
                        {
                            List<Param.Row> rows = _activeView._selection.getSelectedRows();
                            Param param = ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()];
                            insertIndex = param.IndexOfRow(rows.Last()) + 1;
                            foreach (Param.Row r in ParamBank.ClipboardRows)
                            {
                                // Determine new ID based on paste target. Increment ID until a free ID is found.
                                Param.Row newrow = new Param.Row(r);
                                newrow.ID = _activeView._selection.getSelectedRows().Last().ID;
                                do
                                {
                                    newrow.ID++;
                                }
                                while (ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()][newrow.ID] != null || rowsToInsert.Exists(e => e.ID == newrow.ID));
                                rowsToInsert.Add(newrow);
                            }
                            // Do a clever thing by reversing order, making ID order incremental and resulting in row insertion being in the correct order because of the static index.
                            rowsToInsert.Reverse();
                        }
                        EditorActionManager.ExecuteAction(new AddParamsAction(ParamBank.PrimaryBank.Params[ParamBank.ClipboardParam], "legacystring", rowsToInsert, false, false, insertIndex));
                        ImGui.CloseCurrentPopup();
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

        public (string, List<Param.Row>) GetActiveSelection()
        {
            return (_activeView?._selection?.getActiveParam(), _activeView?._selection?.getSelectedRows());
        }

        public void OnProjectChanged(ProjectSettings newSettings)
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
            MassEditScript.ReloadScripts();
        }

        public void Save()
        {
            try
            {
                if (_projectSettings != null)
                {
                    ParamBank.PrimaryBank.SaveParams(_projectSettings.UseLooseParams, _projectSettings.PartialParams);
                    TaskLogs.AddLog("Saved params");
                }
            }
            catch (SavingFailedException e)
            {
                TaskLogs.AddLog(e.Message,
                    Microsoft.Extensions.Logging.LogLevel.Error, TaskLogs.LogPriority.High, e.Wrapped);
            }
            catch (Exception e)
            {
                TaskLogs.AddLog(e.Message,
                    Microsoft.Extensions.Logging.LogLevel.Error, TaskLogs.LogPriority.High, e);
            }
        }

        public void SaveAll()
        {
            try
            {
                if (_projectSettings != null)
                {
                    ParamBank.PrimaryBank.SaveParams(_projectSettings.UseLooseParams, _projectSettings.PartialParams);
                    TaskLogs.AddLog("Saved params");
                }
            }
            catch (SavingFailedException e)
            {
                TaskLogs.AddLog($"{e.Message}",
                    Microsoft.Extensions.Logging.LogLevel.Error, TaskLogs.LogPriority.High, e.Wrapped);
            }
            catch (Exception e)
            {
                TaskLogs.AddLog($"{e.Message}",
                    Microsoft.Extensions.Logging.LogLevel.Error, TaskLogs.LogPriority.High, e);
            }
        }

        private static void TryWriteFile(string path, string text)
        {
            try
            {
                File.WriteAllText(path, text);
            }
            catch (Exception e)
            {
                PlatformUtils.Instance.MessageBox("Unable to write to "+path, "Write Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static string TryReadFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                PlatformUtils.Instance.MessageBox("Unable to read from "+path, "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }

    public class ParamEditorSelectionState
    {
        ParamEditorScreen _scr;
        internal string currentParamSearchString = "";
        private static string _globalRowSearchString = "";
        private static string _globalPropSearchString = "";
        private string _activeParam = null;

        List<(string, Param.Row)> pastStack = new List<(string, Param.Row)>();
        private Dictionary<string, ParamEditorParamSelectionState> _paramStates = new Dictionary<string, ParamEditorParamSelectionState>();

        public ParamEditorSelectionState(ParamEditorScreen paramEditor)
        {
            _scr = paramEditor;
        }

        private void pushHistory(string newParam, Param.Row newRow)
        {
            if (pastStack.Count > 0)
            {
                var prev = pastStack[pastStack.Count - 1];
                if (prev.Item1 == newParam && prev.Item2 == null)
                    pastStack[pastStack.Count - 1] = (prev.Item1, newRow);
                prev = pastStack[pastStack.Count - 1];
                if (prev.Item1 == newParam && prev.Item2 == newRow)
                    return;
            }
            if (_activeParam != null)
                pastStack.Add((_activeParam, _paramStates[_activeParam].activeRow));
            if (pastStack.Count >= 6)
                pastStack.RemoveAt(0);
        }
        public void popHistory()
        {
            if (pastStack.Count > 0)
            {
                var past = pastStack[pastStack.Count - 1];
                pastStack.RemoveAt(pastStack.Count - 1);
                if (past.Item2 == null && pastStack.Count > 0)
                {
                    past = pastStack[pastStack.Count - 1];
                    pastStack.RemoveAt(pastStack.Count - 1);
                }
                setActiveParam(past.Item1, true);
                SetActiveRow(past.Item2, true, true);
            }
        }
        public bool hasHistory()
        {
            return pastStack.Count > 0;
        }

        public bool activeParamExists()
        {
            return _activeParam != null;
        }
        public string getActiveParam()
        {
            return _activeParam;
        }
        public void setActiveParam(string param, bool isHistory = false)
        {
            if (!isHistory)
                pushHistory(param, null);
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
        public void setCurrentRowSearchString(string s)
        {
            if (_activeParam == null)
                return;
            _paramStates[_activeParam].currentRowSearchString = s;
            _paramStates[_activeParam].selectionCacheDirty = true;
        }
        public void setCurrentPropSearchString(string s)
        {
            if (_activeParam == null)
                return;
            _paramStates[_activeParam].currentPropSearchString = s;
        }
        public bool rowSelectionExists()
        {
            return _activeParam != null && _paramStates[_activeParam].selectionRows.Count > 0;
        }
        public Param.Row getActiveRow()
        {
            if (_activeParam == null)
                return null;
            return _paramStates[_activeParam].activeRow;
        }
        public Param.Row getCompareRow()
        {
            if (_activeParam == null)
                return null;
            return _paramStates[_activeParam].compareRow;
        }
        public Param.Column getCompareCol()
        {
            if (_activeParam == null)
                return null;
            return _paramStates[_activeParam].compareCol;
        }
        public void SetActiveRow(Param.Row row, bool clearSelection, bool isHistory = false)
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                if (s.activeRow != null && !ParamBank.VanillaBank.IsLoadingParams)
                    ParamBank.PrimaryBank.RefreshParamRowVanillaDiff(s.activeRow, _activeParam);
                if (!isHistory)
                    pushHistory(_activeParam, s.activeRow);
                s.activeRow = row;
                s.selectionRows.Clear();
                s.selectionRows.Add(row);
                if (s.activeRow != null && !ParamBank.VanillaBank.IsLoadingParams)
                    ParamBank.PrimaryBank.RefreshParamRowVanillaDiff(s.activeRow, _activeParam);
                s.selectionCacheDirty = true;
            }
        }
        public void SetCompareRow(Param.Row row)
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                s.compareRow = row;
            }
        }
        public void SetCompareCol(Param.Column col)
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                s.compareCol = col;
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
                s.selectionCacheDirty = true;
            }
            //Do not perform vanilla diff here, will be very slow when making large selections
        }
        public void addRowToSelection(Param.Row row)
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                if (!s.selectionRows.Contains(row))
                {
                    s.selectionRows.Add(row);
                    s.selectionCacheDirty = true;
                }
            }
            //Do not perform vanilla diff here, will be very slow when making large selections
        }
        public void removeRowFromSelection(Param.Row row)
        {
            if (_activeParam != null)
            {
                _paramStates[_activeParam].selectionRows.Remove(row);
                _paramStates[_activeParam].selectionCacheDirty = true;
            }
        }
        public void removeRowFromAllSelections(Param.Row row)
        {
            foreach (ParamEditorParamSelectionState state in _paramStates.Values)
            {
                state.selectionRows.Remove(row);
                if (state.activeRow == row)
                    state.activeRow = null;
                state.selectionCacheDirty = true;
            }
        }
        public List<Param.Row> getSelectedRows()
        {
            if (_activeParam == null)
                return null;
            return _paramStates[_activeParam].selectionRows;
        }
        public bool[] getSelectionCache(List<Param.Row> rows, string cacheVer)
        {
            if (_activeParam == null)
                return null;
            ParamEditorParamSelectionState s = _paramStates[_activeParam];
            // We maintain this flag as clearing the cache properly is slow for the number of times we modify selection
            if (s.selectionCacheDirty)
                CacheBank.RemoveCache(_scr, s);
            return CacheBank.GetCached(_scr, s, "selectionCache"+cacheVer, () =>
            {
                s.selectionCacheDirty = false;
                return rows.Select((x) => getSelectedRows().Contains(x)).ToArray();
            });
        }
        public void cleanSelectedRows()
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                s.selectionRows.Clear();
                if (s.activeRow != null)
                    s.selectionRows.Add(s.activeRow);
                s.selectionCacheDirty = true;
            }
        }
        public void cleanAllSelectionState()
        {
            foreach (ParamEditorParamSelectionState s in _paramStates.Values)
                s.selectionCacheDirty = true;
            _activeParam = null;
            _paramStates.Clear();
        }
        public void sortSelection()
        {
            if (_activeParam != null)
            {
                ParamEditorParamSelectionState s = _paramStates[_activeParam];
                Param p = ParamBank.PrimaryBank.Params[_activeParam];
                s.selectionRows.Sort((Param.Row a, Param.Row b) => {return p.IndexOfRow(a) - p.IndexOfRow(b);});
            }
        }
    }

    internal class ParamEditorParamSelectionState
    {
        internal string currentRowSearchString = "";
        internal string currentPropSearchString = "";
        internal Param.Row activeRow = null;
        internal Param.Row compareRow = null;
        internal Param.Column compareCol = null;

        internal List<Param.Row> selectionRows = new List<Param.Row>();
        internal bool selectionCacheDirty = true;
    }

    public class ParamEditorView
    {
        private static Vector4 AUXCONFLICTCOLOUR = new Vector4(1,0.7f,0.7f,1);
        private static Vector4 AUXADDEDCOLOUR = new Vector4(0.7f,0.7f,1,1);
        private static Vector4 PRIMARYCHANGEDCOLOUR = new Vector4(0.7f,1,0.7f,1);
        private static Vector4 ALLVANILLACOLOUR = new Vector4(0.9f,0.9f,0.9f,1);

        internal ParamEditorScreen _paramEditor;
        internal int _viewIndex;
        private int _gotoParamRow = -1;
        private bool _arrowKeyPressed = false;
        private bool _focusRows = false;
        private bool _mapParamView = false;

        internal ParamEditorSelectionState _selection;

        private PropertyEditor _propEditor = null;

        private string lastParamSearch = "";
        private Dictionary<string, string> lastRowSearch = new Dictionary<string, string>();

        public ParamEditorView(ParamEditorScreen parent, int index)
        {
            _paramEditor = parent;
            _viewIndex = index;
            _propEditor = new PropertyEditor(parent.EditorActionManager, _paramEditor);
            _selection = new ParamEditorSelectionState(_paramEditor);
        }

        public void ParamView(bool doFocus, bool isActiveView)
        {
            float scale = MapStudioNew.GetUIScale();

            if (EditorDecorations.ImGuiTableStdColumns("paramsT", 3, true))
            {
                ImGui.TableSetupColumn("paramsCol", ImGuiTableColumnFlags.None, 0.5f);
                ImGui.TableSetupColumn("paramsCol2", ImGuiTableColumnFlags.None, 0.5f);
                ImGui.TableNextColumn();

                if (isActiveView && InputTracker.GetKeyDown(KeyBindings.Current.Param_SearchParam))
                    ImGui.SetKeyboardFocusHere();
                ImGui.InputText($"Search <{KeyBindings.Current.Param_SearchParam.HintText}>", ref _selection.currentParamSearchString, 256);
                string resAutoParam = AutoFill.ParamSearchBarAutoFill();
                if (resAutoParam != null)
                    _selection.setCurrentRowSearchString(resAutoParam);
                if (!_selection.currentParamSearchString.Equals(lastParamSearch))
                {
                    CacheBank.ClearCaches();
                    lastParamSearch = _selection.currentParamSearchString;
                }

                if (ParamBank.PrimaryBank.AssetLocator.Type is GameType.DemonsSouls
                    or GameType.DarkSoulsPTDE
                    or GameType.DarkSoulsRemastered)
                {
                    // This game has DrawParams, add UI element to toggle viewing DrawParam and GameParams.
                    if (ImGui.Checkbox("Edit Drawparams", ref _mapParamView))
                        CacheBank.ClearCaches();
                    ImGui.Separator();
                }
                else if (ParamBank.PrimaryBank.AssetLocator.Type is GameType.DarkSoulsIISOTFS)
                {
                    // DS2 has map params, add UI element to toggle viewing map params and GameParams.
                    if (ImGui.Checkbox("Edit Map Params", ref _mapParamView))
                        CacheBank.ClearCaches();
                    ImGui.Separator();
                }

                List<string> pinnedParamKeyList = new List<string>(_paramEditor._projectSettings.PinnedParams);

                if (pinnedParamKeyList.Count > 0)
                {
                    //ImGui.Text("        Pinned Params");
                    foreach (var paramKey in pinnedParamKeyList)
                    {
                        var primary = ParamBank.PrimaryBank.VanillaDiffCache.GetValueOrDefault(paramKey, null);
                        Param p = ParamBank.PrimaryBank.Params[paramKey];
                        if (p != null)
                        {
                            ParamMetaData meta = ParamMetaData.Get(p.AppliedParamdef);
                            string Wiki = meta?.Wiki;
                            if (Wiki != null)
                            {
                                if (EditorDecorations.HelpIcon(paramKey + "wiki", ref Wiki, true))
                                {
                                    meta.Wiki = Wiki;
                                }
                            }
                        }
                        ImGui.Indent(15.0f * scale);
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
                        ImGui.Unindent(15.0f * scale);
                    }
                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();
                }
                float scrollTo = 0f;
                if (ImGui.BeginChild("paramTypes"))
                {
                    List<string> paramKeyList = CacheBank.GetCached(this._paramEditor, _viewIndex, () => {
                        var list = ParamSearchEngine.pse.Search(true, _selection.currentParamSearchString, true, true);
                        var keyList = list.Where((param) => param.Item1 == ParamBank.PrimaryBank).Select((param) => ParamBank.PrimaryBank.GetKeyForParam(param.Item2)).ToList();

                        if (ParamBank.PrimaryBank.AssetLocator.Type is GameType.DemonsSouls
                            or GameType.DarkSoulsPTDE
                            or GameType.DarkSoulsRemastered)
                        {
                            if (_mapParamView)
                                keyList = keyList.FindAll(p => p.EndsWith("Bank"));
                            else
                                keyList = keyList.FindAll(p => !p.EndsWith("Bank"));
                        }
                        else if (ParamBank.PrimaryBank.AssetLocator.Type is GameType.DarkSoulsIISOTFS)
                        {
                            if (_mapParamView)
                                keyList = keyList.FindAll(p => ParamBank.DS2MapParamlist.Contains(p.Split('_')[0]));
                            else
                                keyList = keyList.FindAll(p => !ParamBank.DS2MapParamlist.Contains(p.Split('_')[0]));
                        }

                        if (CFG.Current.Param_AlphabeticalParams)
                            keyList.Sort();
                        return keyList;
                    });

                    foreach (var paramKey in paramKeyList)
                    {
                        var primary = ParamBank.PrimaryBank.VanillaDiffCache.GetValueOrDefault(paramKey, null);
                        Param p = ParamBank.PrimaryBank.Params[paramKey];
                        if (p != null)
                        {
                            ParamMetaData meta = ParamMetaData.Get(p.AppliedParamdef);
                            string Wiki = meta?.Wiki;
                            if (Wiki != null)
                            {
                                if (EditorDecorations.HelpIcon(paramKey + "wiki", ref Wiki, true))
                                    meta.Wiki = Wiki;
                            }
                        }

                        ImGui.Indent(15.0f * scale);
                        if (primary != null ? primary.Any() : false)
                            ImGui.PushStyleColor(ImGuiCol.Text, PRIMARYCHANGEDCOLOUR);
                        else
                            ImGui.PushStyleColor(ImGuiCol.Text, ALLVANILLACOLOUR);
                        if (ImGui.Selectable($"{paramKey}", paramKey == _selection.getActiveParam()))
                        {
                            //_selection.setActiveParam(param.Key);
                            EditorCommandQueue.AddCommand($@"param/view/{_viewIndex}/{paramKey}");
                        }
                        ImGui.PopStyleColor();

                        if (doFocus && paramKey == _selection.getActiveParam())
                            scrollTo = ImGui.GetCursorPosY();
                        if (ImGui.BeginPopupContextItem())
                        {
                            if (ImGui.Selectable("Pin "+paramKey) && !_paramEditor._projectSettings.PinnedParams.Contains(paramKey))
                                _paramEditor._projectSettings.PinnedParams.Add(paramKey);
                            if (ParamEditorScreen.EditorMode && p != null)
                            {
                                ParamMetaData meta = ParamMetaData.Get(p.AppliedParamdef);
                                if (meta != null && meta.Wiki == null && ImGui.MenuItem("Add wiki..."))
                                    meta.Wiki = "Empty wiki...";
                                if (meta?.Wiki != null && ImGui.MenuItem("Remove wiki"))
                                    meta.Wiki = null;
                            }
                            ImGui.EndPopup();
                        }
                        ImGui.Unindent(15.0f * scale);
                    }

                    if (doFocus)
                        ImGui.SetScrollFromPosY(scrollTo - ImGui.GetScrollY());
                    ImGui.EndChild();
                }
                ImGui.TableNextColumn();
                string activeParam = _selection.getActiveParam();
                if (!_selection.activeParamExists())
                {
                    ImGui.BeginChild("rowsNONE");
                    ImGui.Text("Select a param to see rows");
                    ImGui.EndChild();
                }
                else
                {
                    Param para = ParamBank.PrimaryBank.Params[activeParam];
                    HashSet<int> vanillaDiffCache = ParamBank.PrimaryBank.GetVanillaDiffRows(activeParam);
                    List<(HashSet<int>, HashSet<int>)> auxDiffCaches = ParamBank.AuxBanks.Select((bank, i) => (bank.Value.GetVanillaDiffRows(activeParam), bank.Value.GetPrimaryDiffRows(activeParam))).ToList();
                    IParamDecorator decorator = null;
                    if (_paramEditor._decorators.ContainsKey(activeParam))
                    {
                        decorator = _paramEditor._decorators[activeParam];
                    }
                    scrollTo = 0;

                    //Goto ID
                    if (ImGui.Button($"Goto ID <{KeyBindings.Current.Param_GotoRowID.HintText}>") || (isActiveView && InputTracker.GetKeyDown(KeyBindings.Current.Param_GotoRowID)))
                    {
                        ImGui.OpenPopup("gotoParamRow");
                    }
                    if (ImGui.BeginPopup("gotoParamRow"))
                    {
                        int gotorow = 0;
                        ImGui.SetKeyboardFocusHere();
                        ImGui.InputInt("Goto Row ID", ref gotorow);
                        if (ImGui.IsItemDeactivatedAfterEdit())
                        {
                            _gotoParamRow = gotorow;
                            ImGui.CloseCurrentPopup();
                        }
                        ImGui.EndPopup();
                    }

                    //Row ID/name search
                    if (isActiveView && InputTracker.GetKeyDown(KeyBindings.Current.Param_SearchRow))
                        ImGui.SetKeyboardFocusHere();

                    ImGui.InputText($"Search <{KeyBindings.Current.Param_SearchRow.HintText}>", ref _selection.getCurrentRowSearchString(), 256);
                    string resAutoRow = AutoFill.RowSearchBarAutoFill();
                    if (resAutoRow != null)
                        _selection.setCurrentRowSearchString(resAutoRow);
                    if (!lastRowSearch.ContainsKey(_selection.getActiveParam()) || !lastRowSearch[_selection.getActiveParam()].Equals(_selection.getCurrentRowSearchString()))
                    {
                        CacheBank.ClearCaches();
                        lastRowSearch[_selection.getActiveParam()] = _selection.getCurrentRowSearchString();
                        doFocus = true;
                    }

                    if (ImGui.IsItemActive())
                        _paramEditor._isSearchBarActive = true;
                    else
                        _paramEditor._isSearchBarActive = false;
                    UIHints.AddImGuiHintButton("MassEditHint", ref UIHints.SearchBarHint);
                    
                    Param.Column compareCol = _selection.getCompareCol();
                    ImGui.BeginChild("rows" + activeParam);
                    if (EditorDecorations.ImGuiTableStdColumns("rowList", compareCol == null ? 1 : 2, false))
                    {
                        ImGui.TableSetupColumn("rowCol", ImGuiTableColumnFlags.None, 1f);
                        if (compareCol != null)
                            ImGui.TableSetupColumn("rowCol2", ImGuiTableColumnFlags.None, 0.4f);
                        ImGui.PushID("pinned");

                        List<Param.Row> pinnedRowList = _paramEditor._projectSettings.PinnedRows.GetValueOrDefault(activeParam, new List<int>()).Select((id) => para[id]).ToList();
                        bool[] selectionCachePins = _selection.getSelectionCache(pinnedRowList, "pinned");
                        if (pinnedRowList.Count != 0)
                        {
                            for (int i=0; i<pinnedRowList.Count(); i++)
                            {
                                Param.Row row = pinnedRowList[i];
                                if (row == null)
                                {
                                    continue;
                                }
                                RowColumnEntry(selectionCachePins, i, activeParam, null, row, vanillaDiffCache, auxDiffCaches, decorator, ref scrollTo, false, true, compareCol);
                            }

                            ImGui.Spacing();
                            EditorDecorations.ImguiTableSeparator();
                            ImGui.Spacing();
                        }
                        ImGui.PopID();

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

                        List<Param.Row> rows = CacheBank.GetCached(this._paramEditor, (_viewIndex, activeParam), () => RowSearchEngine.rse.Search((ParamBank.PrimaryBank, para), _selection.getCurrentRowSearchString(), true, true));

                        bool enableGrouping = !CFG.Current.Param_DisableRowGrouping && ParamMetaData.Get(ParamBank.PrimaryBank.Params[activeParam].AppliedParamdef).ConsecutiveIDs;

                        // Rows
                        bool[] selectionCache = _selection.getSelectionCache(rows, "regular");
                        for (int i = 0; i < rows.Count; i++)
                        {
                            Param.Row currentRow = rows[i];
                            if (enableGrouping)
                            {
                                Param.Row prev = i - 1 > 0 ? rows[i - 1] : null;
                                Param.Row next = i + 1 < rows.Count ? rows[i + 1] : null;
                                if (prev != null && next != null && prev.ID + 1 != currentRow.ID && currentRow.ID + 1 == next.ID)
                                    EditorDecorations.ImguiTableSeparator();
                                RowColumnEntry(selectionCache, i, activeParam, rows, currentRow, vanillaDiffCache, auxDiffCaches, decorator, ref scrollTo, doFocus, false, compareCol);
                                if (prev != null && next != null && prev.ID + 1 == currentRow.ID && currentRow.ID + 1 != next.ID)
                                    EditorDecorations.ImguiTableSeparator();
                            }
                            else
                            {
                                RowColumnEntry(selectionCache, i, activeParam, rows, currentRow, vanillaDiffCache, auxDiffCaches, decorator, ref scrollTo, doFocus, false, compareCol);
                            }
                        }
                        if (doFocus)
                            ImGui.SetScrollFromPosY(scrollTo - ImGui.GetScrollY());
                        ImGui.EndTable();
                    }
                    ImGui.EndChild();
                }
                Param.Row activeRow = _selection.getActiveRow();
                ImGui.TableNextColumn();
                if (activeRow == null && ImGui.BeginChild("columnsNONE"))
                {
                    ImGui.Text("Select a row to see properties");
                    ImGui.EndChild();
                }
                else if (ImGui.BeginChild("columns" + activeParam))
                {
                    Param vanillaParam = ParamBank.VanillaBank.Params?.GetValueOrDefault(activeParam);
                    _propEditor.PropEditorParamRow(
                        ParamBank.PrimaryBank,
                        activeRow,
                        vanillaParam?[activeRow.ID],
                        ParamBank.AuxBanks.Select((bank, i) => (bank.Key, bank.Value.Params?.GetValueOrDefault(activeParam)?[activeRow.ID])).ToList(),
                        _selection.getCompareRow(),
                        ref _selection.getCurrentPropSearchString(),
                        activeParam,
                        isActiveView,
                        _selection);
                    ImGui.EndChild();
                }
                ImGui.EndTable();
            }
        }

        private void RowColumnEntry(bool[] selectionCache, int selectionCacheIndex, string activeParam, List<Param.Row> p, Param.Row r, HashSet<int> vanillaDiffCache, List<(HashSet<int>, HashSet<int>)> auxDiffCaches, IParamDecorator decorator, ref float scrollTo, bool doFocus, bool isPinned, Param.Column compareCol)
        {
            float scale = MapStudioNew.GetUIScale();

            if (CFG.Current.UI_CompactParams)
            {
                // ItemSpacing only affects clickable area for selectables in tables. Add additional height to prevent gaps between selectables.
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(5.0f, 2.0f) * scale);
            }
            ImGui.TableNextColumn();
            bool diffVanilla = vanillaDiffCache.Contains(r.ID);
            bool auxDiffVanilla = auxDiffCaches.Where((cache) => cache.Item1.Contains(r.ID)).Count() > 0;
            if (diffVanilla)
            {
                // If the auxes are changed bu
                bool auxDiffPrimaryAndVanilla = auxDiffCaches.Where((cache) => cache.Item1.Contains(r.ID) && cache.Item2.Contains(r.ID)).Count() > 0;
                if (auxDiffVanilla && auxDiffPrimaryAndVanilla)
                    ImGui.PushStyleColor(ImGuiCol.Text, AUXCONFLICTCOLOUR);
                else
                    ImGui.PushStyleColor(ImGuiCol.Text, PRIMARYCHANGEDCOLOUR);
            }
            else
            {
                if (auxDiffVanilla)
                    ImGui.PushStyleColor(ImGuiCol.Text, AUXADDEDCOLOUR);
                else
                    ImGui.PushStyleColor(ImGuiCol.Text, ALLVANILLACOLOUR);
            }

            var selected = selectionCache != null && selectionCacheIndex < selectionCache.Length ? selectionCache[selectionCacheIndex] : false;
            if (_gotoParamRow != -1 && !isPinned)
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
            if (_paramEditor.GotoSelectedRow && !isPinned)
            {
                if (_selection.getActiveRow().ID == r.ID)
                {
                    ImGui.SetScrollHereY();
                    _paramEditor.GotoSelectedRow = false;
                }
            }

            string label = $@"{r.ID} {Utils.ImGuiEscape(r.Name, "")}";
            label = Utils.ImGui_WordWrapString(label, ImGui.GetColumnWidth());
            if (ImGui.Selectable(label, selected))
            {
                _focusRows = true;
                if (InputTracker.GetKey(Key.LControl))
                {
                    _selection.toggleRowInSelection(r);
                }
                else if (p != null && InputTracker.GetKey(Key.LShift) && _selection.getActiveRow() != null)
                {
                    _selection.cleanSelectedRows();
                    int start = p.IndexOf(_selection.getActiveRow());
                    int end = p.IndexOf(r);
                    if (start != end && start != -1 && end != -1)
                    {
                        foreach (var r2 in p.GetRange(start < end ? start : end, Math.Abs(end - start)))
                            _selection.addRowToSelection(r2);
                    }
                    _selection.addRowToSelection(r);
                }
                else
                {
                    _selection.SetActiveRow(r, true);
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
                if (ImGui.Selectable("Compare..."))
                {
                    _selection.SetCompareRow(r);
                }
                ParamRefReverseLookupSelectables(ParamBank.PrimaryBank, activeParam, r.ID);
                if (ImGui.Selectable("Copy ID to clipboard"))
                {
                    PlatformUtils.Instance.SetClipboardText($"{r.ID}");
                }
                ImGui.EndPopup();
            }
            if (decorator != null)
                decorator.DecorateParam(r);
            if (doFocus && _selection.getActiveRow() == r)
                scrollTo = ImGui.GetCursorPosY();

            if (compareCol != null)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(r[compareCol].Value.ToParamEditorString());
            }

            if (CFG.Current.UI_CompactParams)
            {
                ImGui.PopStyleVar();
            }
        }

        private void ParamRefReverseLookupSelectables(ParamBank bank, string currentParam, int currentID)
        {
            if (ImGui.BeginMenu("Search for references..."))
            {
                Dictionary<string, List<(string, ParamRef)>> items = CacheBank.GetCached(_paramEditor, (bank, currentParam), () => ParamRefReverseLookupFieldItems(bank, currentParam));
                foreach (KeyValuePair<string, List<(string, ParamRef)>> paramitems in items)
                {
                    if (ImGui.BeginMenu($@"in {paramitems.Key}..."))
                    {
                        foreach ((string fieldName, ParamRef pref) in paramitems.Value)
                        {
                            if (ImGui.BeginMenu($@"in {fieldName}"))
                            {
                                List<Param.Row> rows = CacheBank.GetCached(_paramEditor, (bank, currentParam, currentID, pref), () => ParamRefReverseLookupRowItems(bank, paramitems.Key, fieldName, currentID, pref));
                                foreach (Param.Row row in rows)
                                {
                                    string nameToPrint = string.IsNullOrEmpty(row.Name) ? "Unnamed Row" : row.Name;
                                    if (ImGui.Selectable($@"{row.ID} {nameToPrint}"))
                                    {
                                        EditorCommandQueue.AddCommand($@"param/select/-1/{paramitems.Key}/{row.ID}");
                                    }
                                }
                                if (rows.Count == 0)
                                    ImGui.TextUnformatted("No rows found");
                                ImGui.EndMenu();
                            }

                        }
                        ImGui.EndMenu();
                    }
                }
                if (items.Count == 0)
                    ImGui.TextUnformatted("This param is not referenced");
                ImGui.EndMenu();
            }
        }

        private static Dictionary<string, List<(string, ParamRef)>> ParamRefReverseLookupFieldItems(ParamBank bank, string currentParam)
        {
            Dictionary<string, List<(string, ParamRef)>> items = new Dictionary<string, List<(string, ParamRef)>>();
            foreach (var param in bank.Params)
            {
                List<(string, ParamRef)> paramitems = new List<(string, ParamRef)>();
                //get field
                foreach (PARAMDEF.Field f in param.Value.AppliedParamdef.Fields)
                {
                    var meta = FieldMetaData.Get(f); 
                    if (meta.RefTypes == null)
                        continue;
                    // get hilariously deep in loops
                    foreach (ParamRef pref in meta.RefTypes)
                    {
                        if (!pref.param.Equals(currentParam))
                            continue;
                        paramitems.Add((f.InternalName, pref));
                    }
                }
                if (paramitems.Count > 0)
                    items[param.Key] = paramitems;
            }
            return items;
        }

        private static List<Param.Row> ParamRefReverseLookupRowItems(ParamBank bank, string paramName, string fieldName, int currentID, ParamRef pref)
        {
            string searchTerm = pref.conditionField != null ? $@"prop {fieldName} ^{currentID}$ && prop {pref.conditionField} ^{pref.conditionValue}$" : $@"prop {fieldName} ^{currentID}$";
            return RowSearchEngine.rse.Search((bank, bank.Params[paramName]), searchTerm, false, false);
        }
    }
}
