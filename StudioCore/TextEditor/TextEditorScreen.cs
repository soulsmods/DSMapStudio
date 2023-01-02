﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using ImGuiNET;
using SoulsFormats;
using StudioCore.Editor;

namespace StudioCore.TextEditor
{
    public class TextEditorScreen : EditorScreen
    {
        public ActionManager EditorActionManager = new ActionManager();
        private readonly PropertyEditor _propEditor = null;
        private ProjectSettings _projectSettings;

        private FMGBank.EntryGroup _activeEntryGroup;
        private FMGBank.FMGInfo _activeFmgInfo;

        private List<FMG.Entry> _entryLabelCache;
        private List<FMG.Entry> _EntryLabelCacheFiltered;
        private int _activeIDCache = -1;

        private string _searchFilter = "";
        private string _searchFilterCached = "";

        private bool _clearEntryGroup = false;
        private bool _arrowKeyPressed = false;

        public TextEditorScreen(Sdl2Window window, GraphicsDevice device)
        {
            _propEditor = new PropertyEditor(EditorActionManager);
        }

        private void ClearTextEditorCache()
        {
            CacheBank.ClearCaches();
            _entryLabelCache = null;
            _EntryLabelCacheFiltered = null;
            _activeFmgInfo = null;
            _activeEntryGroup = null;
            _activeIDCache = -1;
            _searchFilter = "";
            _searchFilterCached = "";
        }

        private void ResetActionManager()
        {
            EditorActionManager.Clear();
        }

        /// <summary>
        /// Duplicates all Entries in active EntryGroup from their FMGs
        /// </summary>
        private void DuplicateFMGEntries(FMGBank.EntryGroup entry)
        {
            _activeIDCache = entry.GetNextUnusedID();
            var action = new DuplicateFMGEntryAction(entry);
            EditorActionManager.ExecuteAction(action);

            // Lazy method to refresh search filter
            // TODO: _searchFilterCached should be cleared whenever CacheBank is cleared.
            _searchFilterCached = "";
        }

        /// <summary>
        /// Deletes all Entries within active EntryGroup from their FMGs
        /// </summary>
        private void DeleteFMGEntries(FMGBank.EntryGroup entry)
        {
            var action = new DeleteFMGEntryAction(entry);
            EditorActionManager.ExecuteAction(action);
            _activeEntryGroup = null;
            _activeIDCache = -1;
        }

        public override void DrawEditorMenu()
        {
            if (ImGui.BeginMenu("Edit", FMGBank.IsLoaded))
            {
                if (ImGui.MenuItem("Undo", KeyBindings.Current.Core_Undo.HintText, false, EditorActionManager.CanUndo()))
                {
                    EditorActionManager.UndoAction();
                }
                if (ImGui.MenuItem("Redo", KeyBindings.Current.Core_Redo.HintText, false, EditorActionManager.CanRedo()))
                {
                    EditorActionManager.RedoAction();
                }
                if (ImGui.MenuItem("Delete Entry", KeyBindings.Current.Core_Delete.HintText, false, _activeEntryGroup != null))
                {
                    DeleteFMGEntries(_activeEntryGroup);
                }
                if (ImGui.MenuItem("Duplicate Entry", KeyBindings.Current.Core_Duplicate.HintText, false, _activeEntryGroup != null))
                {
                    DuplicateFMGEntries(_activeEntryGroup);
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Text Language", !FMGBank.IsLoading))
            {
                var folders = FMGBank.AssetLocator.GetMsgLanguages();
                if (folders.Count == 0)
                {
                    ImGui.TextColored(new Vector4(1.0f, 0.0f, 0.0f, 1.0f), "Cannot find language folders.");
                }
                else
                {
                    foreach (var path in folders)
                    {
                        if (ImGui.MenuItem(path.Key, "", FMGBank.LanguageFolder == path.Key))
                        {
                            ChangeLanguage(path.Key);
                        }
                    }
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Import/Export", FMGBank.IsLoaded))
            {
                if (ImGui.MenuItem("Import Files", KeyBindings.Current.TextFMG_Import.HintText))
                {
                    if (FMGBank.ImportFMGs())
                    {
                        ClearTextEditorCache();
                        ResetActionManager();
                    }
                }
                if (ImGui.MenuItem("Export All Text", KeyBindings.Current.TextFMG_ExportAll.HintText))
                {
                    FMGBank.ExportFMGs();
                }
                ImGui.EndMenu();
            }
        }

        private void FMGSearchLogic(ref bool doFocus)
        {
            if (_entryLabelCache != null)
            {
                if (_searchFilter != _searchFilterCached)
                {
                    List<FMG.Entry> matches = new();
                    _EntryLabelCacheFiltered = _entryLabelCache;

                    List<FMG.Entry> mainEntries;
                    if (_searchFilter.Length > _searchFilterCached.Length)
                        mainEntries = _EntryLabelCacheFiltered;
                    else
                        mainEntries = _entryLabelCache;

                    // Title/Textbody
                    foreach (var entry in mainEntries)
                    {
                        if (entry.ID.ToString().Contains(_searchFilter, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // ID search
                            matches.Add(entry);
                        }
                        else if (entry.Text != null)
                        {
                            // Text search
                            if (entry.Text.Contains(_searchFilter, StringComparison.CurrentCultureIgnoreCase))
                                matches.Add(entry);
                        }
                    }

                    // Descriptions
                    foreach (var entry in FMGBank.GetFmgEntriesByCategoryAndTextType(_activeFmgInfo.EntryCategory, FMGBank.FmgEntryTextType.Description, false))
                    {
                        if (entry.Text != null)
                        {
                            if (entry.Text.Contains(_searchFilter, StringComparison.CurrentCultureIgnoreCase))
                            {
                                var search = _entryLabelCache.Find(e => e.ID == entry.ID && !matches.Contains(e));
                                if (search != null)
                                    matches.Add(search);
                            }
                        }
                    }

                    // Summaries
                    foreach (var entry in FMGBank.GetFmgEntriesByCategoryAndTextType(_activeFmgInfo.EntryCategory, FMGBank.FmgEntryTextType.Summary, false))
                    {
                        if (entry.Text != null)
                        {
                            if (entry.Text.Contains(_searchFilter, StringComparison.CurrentCultureIgnoreCase))
                            {
                                var search = _entryLabelCache.Find(e => e.ID == entry.ID && !matches.Contains(e));
                                if (search != null)
                                    matches.Add(search);
                            }
                        }
                    }
                   
                    _EntryLabelCacheFiltered = matches;
                    _searchFilterCached = _searchFilter;
                    doFocus = true;
                }
                else if (_entryLabelCache != _EntryLabelCacheFiltered && _searchFilter == "")
                {
                    _EntryLabelCacheFiltered = _entryLabelCache;
                }
            }
        }

        private void CategoryListUI(FMGBank.FmgUICategory uiType, bool doFocus)
        {
            foreach (var info in FMGBank.FmgInfoBank)
            {
                if (info.PatchParent == null 
                    && info.UICategory == uiType 
                    && info.EntryType is FMGBank.FmgEntryTextType.Title or FMGBank.FmgEntryTextType.TextBody)
                {
                    string displayName = "";
                    if (CFG.Current.FMG_ShowOriginalNames)
                    {
                        displayName = info.FileName;
                    }
                    else
                    {
                        if (!CFG.Current.FMG_NoGroupedFmgEntries)
                            displayName = info.Name.Replace("Title", "");
                        else
                            displayName = info.Name;

                        displayName = displayName.Replace("Modern_", "");
                    }
                    if (ImGui.Selectable($@" {displayName}", info == _activeFmgInfo))
                    {
                        ClearTextEditorCache();
                        _activeFmgInfo = info;
                    }

                    if (doFocus && info == _activeFmgInfo)
                    {
                        ImGui.SetScrollHereY();
                    }
                }
            }
        }

        private void EditorGUI(bool doFocus)
        {
            if (!FMGBank.IsLoaded)
            {
                if (FMGBank.IsLoading)
                {
                    ImGui.Text("Loading...");
                }
                else
                {
                    ImGui.Text("This Editor requires game to be unpacked");
                    // Ascii fatcat (IMPORTANT)
                    ImGui.TextUnformatted("\nPPPPP5555PGGGGGGGBBBBBBGPYJ?????????JJYYYYYY5PPPGGPPPGPPPPPPPPPPPPGGGGGGGGGPPPPPPPPPPPPPPPPPPPPPPPPP\r\nPPPPP5555PGGGGGBBBBBBGGGPYJJ??????JJYY55555Y555PPPPGGGGGGGGGGGGGGGGGPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP\r\nPPPPP555PPGGGGGBBBBBBGGGP5JJ?????JJY55555555YYY555PPPPGGGGGGGPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP\r\nPPPPP555PPGGGGGBBBBBBGBGPYJ?????JJY555555555555555555PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPGPPP\r\nPP55555PPGGGGGGGBBBBBBGGPYJ????JJY555555555555555555555555PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP\r\n555Y55PPGGGGGGGGGGGBBGGG5Y?????JYY5555555P5555P555555555555555PPPPPPPPPPPPPPPPPPGGGGGGGGGPPPPPPPPPPP\r\n555555PPPPPPPPGGGGGBBBBGPJ???JJYY55555555PPPP5555555555555555555PPPPPPPPPPPPPPPPPPPPPPPPPPPPPGGGGGGG\r\nPPPPPPPPPPPGGGGGGGGGBBBBG5YJJJYYYY555555PPP55555555555555555555555PPPPPPPPPGGPPPPPGGGGPPPPPPPPPPPPPP\r\nPPPPPPPGGGGGGGGGGGGBBBBBGP555YYYYYYYYYY55555555555YYYYYYYYY55555555555PPPPPPPPPPPPPPPPPPPPPPPGPPPPPP\r\nPPPPPGGGGGPPPPPPPPGGGGGGPPPP5555YYYYYYYYYY555555YYYYYYYYYY55555YYYYYY5555555555555555555555555555PPP\r\nPPPPPPPPPPPP555555PPPPPPPPPP5555555555YYYYYYYYYYYYYYYYYYYY55YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY\r\n55PPPPPP555555555555555PP55555555555555555YYYYYYYYYYYYYYYYYYYYYYYJJJJJ??JJJJJJ?JJJJJJJJJJJJJJJJJJJJJ\r\nPPPPPPP55555555PP5555555P55555555555555555YYYYYYYYYYYYYYYYJJJJJJJJJ??????????77?????????????????????\r\nPPPPPP5PPPPPPPPPP555555555555555555555555YYYYYYYYYYYYYYYJJ??????????7777777777777?????????7?????????\r\nPPPPPPPPPPPPPPPP55555555555555555555YYYYYYYYYYYYYYYYJJJ???7777777777??7777777777777777????77777?????\r\nGGGGBBBBBBGGGGGGGGGPPPPPPPPP5555555YYYYYYYJJJJJJJJ??????77777777?77??????777777777777777?777777?????\r\nGGGGGGGBBBBBBBBBBBBBBGGGGGPPPPPP5555YYYYJJJJJJ????????7777777?????????????????????7777777777777?????\r\nPPPPPPPPPGGGGGGGGGGBBBBBBBGGPPPPPP555YYYYJJJJJ?????????77777????JJYYYJJJJJ?????????????????777777777\r\nPPP5PPPPPPPPPPPPPPPPPPPGGBBGGGPPPPPP55YYYYYYJJJ??7777????777??JJYY5555YYYYYYYYYYYYYJJJJYJJJJ????????\r\nPPPPPPPPPPPPP5PPPPPP555PPGGGGGPPPPPPP5555YYYJJ???7777?77777???JY555PPPPPPPPPGGGGGGGGGPPPPPPPP555YYYY\r\nPPPPPPPPPPPPP55PPP5555555PPPPPPPPPPPPP555YYJJ????????????777??J5PGGGGGGGGGGGGGGGGBBBBBBBBBBBBBBBBBBB\r\nPPPPPPPPPPPP55555555555PPPPPPPPPPPPPP55YYYYJJJ?????????????77?Y5P555YJJ??JJJJJ??JJJJJJJYYY5555555555\r\nPPP55PPPP5555555555555PPPPPPPPPPPGGPP5YYYYYJJJ????????????777??JJJJ???777777777777777777????????????\r\n555555P55555P55555555PPPPPPPPPPPGGGP55YYYYYJJJJJJJ??JJJ?????????????77777777777777777777777777777777\r\n5555555555PPPP55555PPPPPPPPPPPPPGGPP5YYYYYYJJJJJJJJJJJJJ????????????77777777777777777777777777777777\r\nP5555555555P5555555PPPPPPPPGGGGGGGPP5YYYYYYYYJJJJJYYYYYJJJ??????????77777777777777777777777777777777\r\n555P5555555555555555PPPPPPPGGGGPPPP55YYYYYYYYYYYYYY555YYJJJ????????777777777777777777777777777777777\r\n55555555555555PPP555PPPPPPGGGGGGPPP55YYYYYYYYYYYYYYY55YYJJJ???????7777777777777777777777777777777777\r\nPPPP55PP5555PPPPPPPPPPPPPPPGGGGGGPPP555YYYYYYYYYYY55PP5YJJ???????????7777777777777777777777777777777\r\nPPP555555PPPP55PPPPPPPPPPPGGGGBGGPPPPP555555555PPPGGPP5YJJJJ??????????777777777777777777777777777777\r\nPPPPPPPPPPPPPPPPPPPPPPPPPPGGGGGGGPPPPPPPPPPPPPPPGGGGP55YYYYJJ?????????777777777777777777777777777777\r\nPPPPPPPGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGPPGPPP55YYYYYJJJJ?????????77777777777777777777777777\r\nPPPPPPPPGGGBBBBBBBBBGGGBBBBBBBBBBBBBBBBBBBBGGBBGGGGGGPPP5555YYYYYYYJJJJJJ??????777777777777777777777\r\nPPPPPPPPPPPPPGGBBBGGPPGGBBBBBBBBBBBBBBBBBBBBBBBBBBBGGGGGGGPPPPPPPPP55555YYYYJJJ????77777777777777777\r\nPPPPPPPPPPPPPPPPPPPPPPPPGBBBBBBBBBBBBBBBBBBBBBB##BGGPPGGBBBBGGGGGGGGPPPPPPPP5555YYJJ??????7777777777\r\nPPPPPPPPPPPPPPPPPPP55555PPGGGBBBBBBBBBBBBBBBBBBBGGPPPPGGGGGPPPPPPPPPPP5555555555555YYJJJ????????????\r\nPPPPPPPPPPPPPPPP5PPPPP555PPPPPPPPPPPPPPPPGPPPPPPP555555555555555YYYJJJJJJJJJJJJJJYYYYJJJJJJJJJ??????\r\nPPPPPPPPPPPPPPP555PPPP555555555555555555555555YYYYYYYYYYYJJJJJJJ???777777777777???JJJJJJJJJJJJJJ????\r\nPPP55PPPPP555PP5555555555555555YYYYYYYYYYYYYYJJJJJJJJJJ??????777777777777777777???JJJJJJJJJJJJJJJ???\r\nPPPPPPPP5555555555555555YYYYYYYJJJJJJ????????????????????????????777777777777???JJJJJJJJJJJJJJJJJ???\r\nPPPP5P555555555555555555555YYYYYJJJJJJJJ??????????????????????????????7?????JJJJJJJJJJJJJJJJJJJJJJJJ\r\nPPP55PP55555555555555555555YYYYYYYJJJJJJJJJJJJJJJJ?????????JJJJJJJJ???JJJJJJJJJJJJJJJJJJJJJJJJJJJJJJ\r\n555555555555555555555555YYYYYYYYYYYYJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJ\r\n55555555555555555555555555555555YYYYYYYYYYYYYYYYYYYYYYYYYYYYYJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJ\r\n5555555555555555555555555555555555555555555555555555555YYYYYYYJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJ?\r\n55555555555555555555555555555555555555555555555555555555YYYYYYYYYYYYJJJJJJJJJJJJJJJJJJJJJJJJJJJ?????\r\n555555555555555555555555555555555555555555555555555555YYYYYYYYYYYYYYYJJJJJJJJJJJJJJJJJJJJJ??????????");
                }
                return;
            }
            var dsid = ImGui.GetID("DockSpace_TextEntries");
            ImGui.DockSpace(dsid, new Vector2(0, 0), ImGuiDockNodeFlags.None);

            ImGui.Begin("Text Categories");
            foreach (var v in FMGBank.ActiveUITypes)
            {
                if (v.Value)
                {
                    ImGui.Separator();
                    ImGui.Text($"  {v.Key} Text");
                    ImGui.Separator();
                    // Categories
                    CategoryListUI(v.Key, doFocus);
                    ImGui.Spacing();
                }
            }
            if (_activeFmgInfo != null)
            {
                _entryLabelCache = CacheBank.GetCached(this, "FMGEntryCache", () =>
                {
                    return _activeFmgInfo.GetPatchedEntries();
                });
            }

            // Needed to ensure EntryGroup is still valid after undo/redo actions while also maintaining highlight-duped-row functionality.
            // It's a bit dumb and probably overthinking things.
            _clearEntryGroup = CacheBank.GetCached(this, "FMGClearEntryGroup", () =>
            {
                if (_clearEntryGroup)
                    return false;
                else
                    return true;
            });
            if (_clearEntryGroup)
            {
                if (!doFocus)
                {
                    _activeEntryGroup = null;
                }
                CacheBank.RemoveCache(this, "FMGClearEntryGroup");
            }

            ImGui.End();

            ImGui.Begin("Text Entries");
            if (ImGui.Button("Clear Text"))
                _searchFilter = "";
            ImGui.SameLine();

            // Search
            if (InputTracker.GetKeyDown(KeyBindings.Current.TextFMG_Search))
                ImGui.SetKeyboardFocusHere();
            ImGui.InputText($"Search <{KeyBindings.Current.TextFMG_Search.HintText}>", ref _searchFilter, 255);

            FMGSearchLogic(ref doFocus);

            ImGui.BeginChild("Text Entry List");
            if (_activeFmgInfo == null)
            {
                ImGui.Text("Select a category to see entries");
            }
            else if (_EntryLabelCacheFiltered == null)
            {
                ImGui.Text("No entries match search filter");
            }
            else
            {
                // Up/Down arrow key input
                if (InputTracker.GetKey(Key.Up) || InputTracker.GetKey(Key.Down))
                {
                    _arrowKeyPressed = true;
                }

                // Entries
                foreach (var r in _EntryLabelCacheFiltered)
                {
                    var text = (r.Text == null) ? "%null%" : r.Text.Replace("\n", "\n".PadRight(r.ID.ToString().Length+2)); 
                    if (ImGui.Selectable($@"{r.ID} {text}", _activeIDCache == r.ID))
                    {
                        _activeEntryGroup = FMGBank.GenerateEntryGroup(r.ID, _activeFmgInfo);
                    }
                    else if (_activeIDCache == r.ID && _activeEntryGroup == null)
                    {
                        _activeEntryGroup = FMGBank.GenerateEntryGroup(r.ID, _activeFmgInfo);
                        _searchFilterCached = "";
                    }

                    if (_arrowKeyPressed && ImGui.IsItemFocused()
                        && _activeEntryGroup?.ID != r.ID)
                    {
                        // Up/Down arrow key selection
                        _activeEntryGroup = FMGBank.GenerateEntryGroup(r.ID, _activeFmgInfo);
                        _arrowKeyPressed = false;
                    }

                    if (ImGui.BeginPopupContextItem())
                    {
                        if (ImGui.Selectable("Duplicate Entry"))
                        {
                            _activeEntryGroup = FMGBank.GenerateEntryGroup(r.ID, _activeFmgInfo);
                            DuplicateFMGEntries(_activeEntryGroup);
                        }
                        if (ImGui.Selectable("Delete Entry"))
                        {
                            _activeEntryGroup = FMGBank.GenerateEntryGroup(r.ID, _activeFmgInfo);
                            DeleteFMGEntries(_activeEntryGroup);
                        }
                        ImGui.EndPopup();
                    }
                    if (doFocus && _activeEntryGroup?.ID == r.ID)
                    {
                        ImGui.SetScrollHereY();
                    }
                }
            }
            ImGui.EndChild();
            ImGui.End();

            ImGui.Begin("Text");
            if (_activeEntryGroup == null)
            {
                ImGui.Text("Select an item to edit text");
            }
            else
            {
                ImGui.Columns(2);
                ImGui.SetColumnWidth(0, 100);
                ImGui.Text("ID");
                ImGui.NextColumn();

                _propEditor.PropIDFMG(_activeEntryGroup, _entryLabelCache);
                _activeIDCache = _activeEntryGroup.ID;

                ImGui.NextColumn();

                _propEditor.PropEditorFMGBegin();
                if (_activeEntryGroup.TextBody != null)
                {
                    _propEditor.PropEditorFMG(_activeEntryGroup.TextBody, "Text", 160.0f);
                }
                if (_activeEntryGroup.Title != null)
                {
                    _propEditor.PropEditorFMG(_activeEntryGroup.Title, "Title", -1.0f);
                }
                if (_activeEntryGroup.Summary != null)
                {
                    _propEditor.PropEditorFMG(_activeEntryGroup.Summary, "Summary", 80.0f);
                }
                if (_activeEntryGroup.Description != null)
                {
                    _propEditor.PropEditorFMG(_activeEntryGroup.Description, "Description", 160.0f);
                }

                _propEditor.PropEditorFMGEnd();
            }
            ImGui.End();
        }

        public void OnGUI(string[] initcmd)
        {
            if (FMGBank.AssetLocator == null)
            {
                return;
            }

            var scale = ImGuiRenderer.GetUIScale();

            // Docking setup
            var wins = ImGui.GetWindowSize();
            var winp = ImGui.GetWindowPos();
            winp.Y += 20.0f * scale;
            wins.Y -= 20.0f * scale;
            ImGui.SetNextWindowPos(winp);
            ImGui.SetNextWindowSize(wins);

            if (!ImGui.IsAnyItemActive() && FMGBank.IsLoaded)
            {
                // Only allow key shortcuts when an item [text box] is not currently activated
                if (EditorActionManager.CanUndo() && InputTracker.GetKeyDown(KeyBindings.Current.Core_Undo))
                {
                    EditorActionManager.UndoAction();
                }
                if (EditorActionManager.CanRedo() && InputTracker.GetKeyDown(KeyBindings.Current.Core_Redo))
                {
                    EditorActionManager.RedoAction();
                }
                if (InputTracker.GetKeyDown(KeyBindings.Current.Core_Delete) && _activeEntryGroup != null)
                {
                    DeleteFMGEntries(_activeEntryGroup);
                }
                if (InputTracker.GetKeyDown(KeyBindings.Current.Core_Duplicate) && _activeEntryGroup != null)
                {
                    DuplicateFMGEntries(_activeEntryGroup);
                }
                if (InputTracker.GetKeyDown(KeyBindings.Current.TextFMG_Import))
                {
                    if (FMGBank.ImportFMGs())
                    {
                        ClearTextEditorCache();
                        ResetActionManager();
                    }
                }
                if (InputTracker.GetKeyDown(KeyBindings.Current.TextFMG_ExportAll))
                {
                    FMGBank.ExportFMGs();
                }
            }
            bool doFocus = false;
            // Parse select commands
            if (initcmd != null && initcmd[0] == "select")
            {
                if (initcmd.Length > 1)
                {
                    // Select FMG
                    doFocus = true;
                    // Use three possible keys: entry category is for param references,
                    // binder id and FMG name are for soapstone references.
                    // This can be revisited as more high-level categories get added.
                    int? searchId = null;
                    FMGBank.FmgEntryCategory? searchCategory = null;
                    string searchName = null;
                    if (int.TryParse(initcmd[1], out int intId) && intId >= 0)
                    {
                        searchId = intId;
                    }
                    // Enum.TryParse allows arbitrary ints (thanks C#), so checking definition is required
                    else if (Enum.TryParse(initcmd[1], out FMGBank.FmgEntryCategory cat)
                        && Enum.IsDefined(typeof(FMGBank.FmgEntryCategory), cat))
                    {
                        searchCategory = cat;
                    }
                    else
                    {
                        searchName = initcmd[1];
                    }
                    foreach (var info in FMGBank.FmgInfoBank)
                    {
                        bool match = false;
                        // This matches top-level item FMGs
                        if (info.EntryCategory.Equals(searchCategory) && info.PatchParent == null
                            && info.EntryType is FMGBank.FmgEntryTextType.Title or FMGBank.FmgEntryTextType.TextBody)
                        {
                            match = true;
                        }
                        else if (searchId is int binderId && binderId == (int)info.FmgID)
                        {
                            match = true;
                        }
                        else if (info.Name == searchName)
                        {
                            match = true;
                        }
                        if (match)
                        {
                            _activeFmgInfo = info;
                            break;
                        }
                    }

                    if (initcmd.Length > 2 && _activeFmgInfo != null)
                    {
                        // Select Entry
                        var parsed = int.TryParse(initcmd[2], out int id);
                        if (parsed)
                        {
                            _activeEntryGroup = FMGBank.GenerateEntryGroup(id, _activeFmgInfo);
                        }
                    }
                }
            }
            EditorGUI(doFocus);
        }

        private void ChangeLanguage(string path)
        {
            _projectSettings.LastFmgLanguageUsed = path;
            ClearTextEditorCache();
            ResetActionManager();
            FMGBank.ReloadFMGs(path);
        }

        public override void OnProjectChanged(ProjectSettings newSettings)
        {
            _projectSettings = newSettings;
            ClearTextEditorCache();
            ResetActionManager();
            FMGBank.ReloadFMGs(_projectSettings.LastFmgLanguageUsed);
        }

        public override void Save()
        {
            FMGBank.SaveFMGs();
        }

        public override void SaveAll()
        {
            FMGBank.SaveFMGs();
        }
    }
}
