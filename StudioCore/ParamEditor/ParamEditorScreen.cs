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
            foreach ((string paramName, FmgEntryCategory category) in ParamBank.ParamToFmgCategoryList)
            {
                _decorators.Add(paramName, new FMGItemParamDecorator(category));
            }
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
                            if (PlatformUtils.Instance.OpenFileDialog(
                                $"Select regulation.bin for game version {ParamBank.PrimaryBank.ParamVersion}...",
                                new[] { AssetLocator.RegulationBinFilter },
                                out string path))
                            {
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
                    if (SaveCsvDialog(out string path))
                    {
                        var rows = CsvExportGetRows(rowType);
                        TryWriteFile(
                            path,
                            ParamIO.GenerateCSV(rows,
                                ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()],
                                CFG.Current.Param_Export_Delimiter[0]));
                    }
                }

                if (ImGui.BeginMenu("Export specific field"))
                {
                    if (ImGui.MenuItem("Row name"))
                    {
                        if (SaveCsvDialog(out string path))
                        {
                            var rows = CsvExportGetRows(rowType);
                            TryWriteFile(
                                path,
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
                            if (SaveCsvDialog(out string path))
                            {
                                var rows = CsvExportGetRows(rowType);
                                TryWriteFile(
                                    path,
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
                            if (SaveCsvDialog(out string path))
                            {
                                var rows = ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()].Rows;
                                TryWriteFile(
                                    path,
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
                            if (ReadCsvDialog(out string csv))
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
                        if (ImGui.MenuItem("Row Name"))
                        {
                            if (ReadCsvDialog(out string csv))
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
                        if (ImGui.BeginMenu("Specific Field"))
                        {
                            foreach (PARAMDEF.Field field in ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()].AppliedParamdef.Fields)
                            {
                                if (ImGui.MenuItem(field.InternalName))
                                {
                                    if (ReadCsvDialog(out string csv))
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
                        try
                        {
                            ImportRowNames(false, "Replace all row names?");
                        }
                        catch
                        {
                        }
                    }
                    if (ImGui.MenuItem("Current Param", "", false, ParamBank.PrimaryBank.Params != null && _activeView._selection.activeParamExists()))
                    {
                        try
                        {
                            ImportRowNames(true, $"Replace all row names in {_activeView._selection.getActiveParam()}?");
                        }
                        catch
                        {
                        }
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("Trim hidden newlines in names", "", false, ParamBank.PrimaryBank.Params != null))
                {
                    try
                    {
                        EditorActionManager.PushSubManager(ParamBank.PrimaryBank.TrimNewlineChrsFromNames());
                    }
                    catch
                    {
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
                    if (!ParamReloader.GameIsSupported(_projectSettings.GameType))
                    {
                        ImGui.TextColored(new Vector4(1.0f, 0.75f, 0.25f, 1.0f),
                            "Param hot reloading is not supported for this game at the moment.");
                    }
                    else
                    {
                        ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                            "WARNING: Hot Reloader only works for existing row entries.\nGame must be restarted for new rows and modified row IDs.");
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
                    string[] allParamTypes = new[]
                    {
                        AssetLocator.RegulationBinFilter,
                        AssetLocator.Data0Filter,
                        AssetLocator.ParamBndDcxFilter,
                        AssetLocator.ParamBndFilter,
                        AssetLocator.EncRegulationFilter
                    };
                    try
                    {
                        if (_projectSettings.GameType != GameType.DarkSoulsIISOTFS)
                        {
                            if (PlatformUtils.Instance.OpenFileDialog("Select file containing params", allParamTypes, out string path))
                            {
                                ParamBank.LoadAuxBank(path, null, null, _projectSettings);
                            }
                        }
                        else
                        {
                            // NativeFileDialog doesn't show the title currently, so manual dialogs are required for now.
                            PlatformUtils.Instance.MessageBox(
                                "To compare DS2 params, select the file locations of alternative params, including\n" +
                                "the loose params folder, the non-loose parambnd or regulation, and the loose enemy param.\n\n" +
                                "First, select the loose params folder.",
                                "Select loose params",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            if (PlatformUtils.Instance.OpenFolderDialog("Select folder for looseparams", out string folder))
                            {
                                PlatformUtils.Instance.MessageBox(
                                    "Second, select the non-loose parambnd or regulation",
                                    "Select regulation",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                if (PlatformUtils.Instance.OpenFileDialog(
                                    "Select file containing remaining, non-loose params", allParamTypes, out string path))
                                {
                                    PlatformUtils.Instance.MessageBox(
                                        "Finally, select the file containing enemyparam",
                                        "Select enemyparam",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                                    if (PlatformUtils.Instance.OpenFileDialog(
                                        "Select file containing enemyparam", new[] { AssetLocator.ParamLooseFilter }, out string enemyPath))
                                    {
                                        ParamBank.LoadAuxBank(path, folder, enemyPath, _projectSettings);
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
            _views.ForEach((view) =>
            {
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
                ImGui.Text("Occurences in " + _statisticPopupParameter);
                ImGui.Text("Count".PadLeft(9) + " Value");
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
                    foreach (Param.Row row in CacheBank.GetCached(this, (_activeView._viewIndex, _activeView._selection.getActiveParam()), () => RowSearchEngine.rse.Search((ParamBank.PrimaryBank, ParamBank.PrimaryBank.Params[_activeView._selection.getActiveParam()]), _activeView._selection.getCurrentRowSearchString(), true, true)))

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

        private static bool SaveCsvDialog(out string path)
        {
            return PlatformUtils.Instance.SaveFileDialog(
                "Choose CSV file", new[] { AssetLocator.CsvFilter, AssetLocator.TxtFilter }, out path);
        }

        private static bool OpenCsvDialog(out string path)
        {
            return PlatformUtils.Instance.OpenFileDialog(
                "Choose CSV file", new[] { AssetLocator.CsvFilter, AssetLocator.TxtFilter }, out path);
        }

        private static bool ReadCsvDialog(out string csv)
        {
            csv = null;
            if (OpenCsvDialog(out string path))
            {
                csv = TryReadFile(path);
            }
            return csv != null;
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
}
