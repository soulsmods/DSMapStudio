using System;
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
    class TextEditorScreen : EditorScreen
    {
        public ActionManager EditorActionManager = new ActionManager();
        private readonly PropertyEditor _propEditor = null;

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
            _EntryLabelCacheFiltered = null;
            _activeFmgInfo = null;
            _activeEntryGroup = null;
            _activeIDCache = -1;
            _searchFilter = "";
            _searchFilterCached = "";
        }

        /// <summary>
        /// Duplicates all FMG Entries in the EntryGroup
        /// </summary>
        private void DuplicateFMGEntries(FMGBank.EntryGroup entry)
        {
            _activeIDCache = entry.GetNextUnusedID();
            var action = new DuplicateFMGEntryAction(entry);
            EditorActionManager.ExecuteAction(action);
        }

        /// <summary>
        /// Deletes all FMG Entries in the EntryGroup
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
                if (ImGui.MenuItem("Undo", "Ctrl+Z", false, EditorActionManager.CanUndo()))
                {
                    EditorActionManager.UndoAction();
                }
                if (ImGui.MenuItem("Redo", "Ctrl+Y", false, EditorActionManager.CanRedo()))
                {
                    EditorActionManager.RedoAction();
                }
                if (ImGui.MenuItem("Delete Entry", "Delete", false, false || _activeEntryGroup != null))
                {
                    DeleteFMGEntries(_activeEntryGroup);
                }
                if (ImGui.MenuItem("Duplicate Entry", "Ctrl+D", false, _activeEntryGroup != null))
                {
                    DuplicateFMGEntries(_activeEntryGroup);
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Text Language", FMGBank.IsLoaded))
            {
                var folders = FMGBank.AssetLocator.GetMsgLanguages();
                foreach (var path in folders)
                {
                    if (ImGui.MenuItem(path.Key, true))
                    {
                        ClearTextEditorCache();
                        FMGBank.ReloadFMGs(path.Key);
                    }
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Import/Export", FMGBank.IsLoaded))
            {
                if (ImGui.MenuItem("Import Files"))
                {
                    if (FMGBank.ImportFMGs())
                    {
                        ClearTextEditorCache();
                    }
                }
                if (ImGui.MenuItem("Export All Text"))
                {
                    FMGBank.ExportFMGs();
                }
                ImGui.EndMenu();
            }
        }

        private void FMGSearchLogic()
        {
            // Todo: This could be cleaned up.
            if (_entryLabelCache != null)
            {
                if (_searchFilter != _searchFilterCached)
                {
                    _EntryLabelCacheFiltered = _entryLabelCache;
                    List<FMG.Entry> matches = new();

                    if (_activeFmgInfo.EntryCategory != FMGBank.FmgEntryCategory.None)
                    {
                        // Gouped entries
                        List<FMG.Entry> searchEntries;
                        if (_searchFilter.Length > _searchFilterCached.Length)
                            searchEntries = _EntryLabelCacheFiltered;
                        else
                            searchEntries = _entryLabelCache;

                        foreach (var entry in searchEntries)
                        {
                            // Titles
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
                        foreach (var entry in FMGBank.GetFmgEntriesByType(_activeFmgInfo.EntryCategory, FMGBank.FmgEntryTextType.Description, false))
                        {
                            // Descriptions
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
                        foreach (var entry in FMGBank.GetFmgEntriesByType(_activeFmgInfo.EntryCategory, FMGBank.FmgEntryTextType.Summary, false))
                        {
                            // Summaries
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
                    }
                    else
                    {
                        // Non-grouped entries
                        List<FMG.Entry> searchEntries;
                        if (_searchFilter.Length > _searchFilterCached.Length)
                            searchEntries = _EntryLabelCacheFiltered;
                        else
                            searchEntries = _entryLabelCache;

                        foreach (var entry in searchEntries)
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
                                {
                                    var search = _entryLabelCache.Find(e => e.ID == entry.ID && !matches.Contains(e));
                                    if (search != null)
                                        matches.Add(search);
                                }
                            }
                        }
                    }

                    _EntryLabelCacheFiltered = matches;
                    _searchFilterCached = _searchFilter;
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
                    if (ImGui.Selectable($@" {info.Name.Replace("Title","")}", info == _activeFmgInfo))
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
                    ImGui.Text("This editor requires a project with the game unpacked to be loaded.");
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
                    CategoryListUI(v.Key, doFocus);
                    ImGui.Spacing();
                }
            }
            if (_activeFmgInfo != null)
            {
                _entryLabelCache = CacheBank.GetCached(this, "FMGEntryCache", () =>
                {
                    return _activeFmgInfo.PatchedEntries();
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
                _activeEntryGroup = null;
                CacheBank.RemoveCache(this, "FMGClearEntryGroup");
            }

            ImGui.End();

            ImGui.Begin("Text Entries");
            if (ImGui.Button("Clear Text"))
                _searchFilter = "";
            ImGui.SameLine();

            if (InputTracker.GetControlShortcut(Key.F))
                ImGui.SetKeyboardFocusHere();
            ImGui.InputText("Search <Ctrl+F>", ref _searchFilter, 255);

            FMGSearchLogic();

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
                if (InputTracker.GetKey(Key.Up) || InputTracker.GetKey(Key.Down))
                {
                    _arrowKeyPressed = true;
                }
                foreach (var r in _EntryLabelCacheFiltered)
                {
                    var text = (r.Text == null) ? "%null%" : r.Text; 
                    if (ImGui.Selectable($@"{r.ID} {text}", _activeIDCache == r.ID))
                    {
                        _activeEntryGroup = FMGBank.GenerateEntryGroup(r.ID, _activeFmgInfo);
                    }
                    else if (_activeIDCache == r.ID && _activeEntryGroup == null)
                    {
                        _activeEntryGroup = FMGBank.GenerateEntryGroup(r.ID, _activeFmgInfo);
                        doFocus = true;
                    }
                    if (_arrowKeyPressed && ImGui.IsItemFocused() 
                        && _activeEntryGroup.ID != r.ID)
                    {
                        // Up/Down arrow selection
                        _activeEntryGroup = FMGBank.GenerateEntryGroup(r.ID, _activeFmgInfo);
                        ImGui.SetKeyboardFocusHere(-1);
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
                    if (doFocus && _activeEntryGroup.ID == r.ID)
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

            // Docking setup
            var wins = ImGui.GetWindowSize();
            var winp = ImGui.GetWindowPos();
            winp.Y += 20.0f;
            wins.Y -= 20.0f;
            ImGui.SetNextWindowPos(winp);
            ImGui.SetNextWindowSize(wins);

            if (!ImGui.IsAnyItemActive())
            {
                // Only allow key shortcuts when an item [text box] is not currently activated
                if (EditorActionManager.CanUndo() && InputTracker.GetControlShortcut(Key.Z))
                {
                    EditorActionManager.UndoAction();
                }
                if (EditorActionManager.CanRedo() && InputTracker.GetControlShortcut(Key.Y))
                {
                    EditorActionManager.RedoAction();
                }
                if (InputTracker.GetKeyDown(Key.Delete) && _activeEntryGroup != null)
                {
                    DeleteFMGEntries(_activeEntryGroup);
                }
                if (InputTracker.GetControlShortcut(Key.D) && _activeEntryGroup != null)
                {
                    DuplicateFMGEntries(_activeEntryGroup);
                }
            }

            bool doFocus = false;

            if (initcmd != null && initcmd[0] == "select")
            {
                var debug = true;
            }
            /*
            // Parse select commands
            if (initcmd != null && initcmd[0] == "select")
            {
                if (initcmd.Length > 1)
                {
                    doFocus = true;
                    if (_activeFmgInfo == FMGBank.FMGTypes.Item)
                    {
                        foreach (var cat in _displayCategories)
                        {
                            if (cat.ToString() == initcmd[1])
                            {
                                _activeItemCategory = cat;
                                _cacheFiltered = FMGBank.GetItemFMGEntriesByType(cat, FMGBank.ItemType.Title);
                                _entryCache = _cacheFiltered;
                                break;
                            }
                        }
                    }
                    else //FMGBank.FMGTypes.Menu
                    {
                        foreach (var cat in FMGBank.GetMenuFMGs())
                        {
                            if (cat.ToString() == initcmd[1])
                            {
                                _activeItemCategory = FMGBank.ItemCategory.None;
                                _activeMenuCategoryPair = cat;
                                _cacheFiltered = FMGBank.GetMenuFMGEntries(cat.Value);
                                _entryCache = _cacheFiltered;
                                break;
                            }
                        }
                    }

                    if (initcmd.Length > 2)
                    {
                        int id;
                        var parsed = int.TryParse(initcmd[2], out id);
                        if (parsed)
                        {
                            var r = _cacheFiltered.FirstOrDefault(r => r.ID == id);
                            if (r != null)
                            {
                                _activeEntryGroup = r;
                                if (_activeFmgInfo == FMGBank.FMGTypes.Item)
                                {
                                    FMGBank.LookupItemID(r.ID, _activeItemCategory, out _cachedTitle, out _cachedSummary, out _cachedDescription);
                                }
                                else
                                {
                                    _cachedTitle = r;
                                }
                            }
                        }
                    }
                }
            }
            */
            EditorGUI(doFocus);
        }

        public override void OnProjectChanged(ProjectSettings newSettings)
        {
            ClearTextEditorCache();
            FMGBank.ReloadFMGs();
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
