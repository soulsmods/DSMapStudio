using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Text.RegularExpressions;
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

        private FMGBank.ItemCategory[] _displayCategories {
            get {
                if (FMGBank.AssetLocator.Type == GameType.EldenRing)
                {
                    return new FMGBank.ItemCategory[]
                    {
                        FMGBank.ItemCategory.Armor,
                        FMGBank.ItemCategory.Characters,
                        FMGBank.ItemCategory.Goods,
                        FMGBank.ItemCategory.Locations,
                        FMGBank.ItemCategory.Rings,
                        FMGBank.ItemCategory.Spells,
                        FMGBank.ItemCategory.Weapons,
                        FMGBank.ItemCategory.Gem,
                        FMGBank.ItemCategory.SwordArts,
                        FMGBank.ItemCategory.Effect,
                        FMGBank.ItemCategory.Message,
                        FMGBank.ItemCategory.Misc,
                    };
                }
                else
                {
                    return new FMGBank.ItemCategory[]
                    {
                        FMGBank.ItemCategory.Armor,
                        FMGBank.ItemCategory.Characters,
                        FMGBank.ItemCategory.Goods,
                        FMGBank.ItemCategory.Locations,
                        FMGBank.ItemCategory.Rings,
                        FMGBank.ItemCategory.Spells,
                        FMGBank.ItemCategory.Weapons
                    };
                }
            }
        }

        private FMGBank.FMGTypes _activeFmgType = FMGBank.FMGTypes.Item;
        private FMGBank.ItemCategory _activeItemCategory = FMGBank.ItemCategory.None;
        private KeyValuePair<FMGBank.MenuFMGTypes, FMG> _activeMenuCategoryPair = new(FMGBank.MenuFMGTypes.None, null);
        private string _activeCategoryDS2 = null;
        private List<FMG.Entry> _cachedEntriesFiltered = null;
        private List<FMG.Entry> _cachedEntries = null;
        private FMG.Entry _activeEntry = null;

        private FMG.Entry _cachedTitle = null;
        private FMG.Entry _cachedSummary = null;
        private FMG.Entry _cachedDescription = null;

        private PropertyEditor _propEditor = null;

        public TextEditorScreen(Sdl2Window window, GraphicsDevice device)
        {
            _propEditor = new PropertyEditor(EditorActionManager);
        }

        private void ClearFMGCache()
        {
            _activeItemCategory = FMGBank.ItemCategory.None;
            _activeMenuCategoryPair = new(FMGBank.MenuFMGTypes.None, null);
            _cachedEntries = null;
            _cachedEntriesFiltered = null;
            _activeEntry = null;
            _FMGsearchStr = "";
            _FMGsearchStrCache = "";
        }
        public override void DrawEditorMenu()
        {
            if (ImGui.BeginMenu("Edit", FMGBank.IsLoaded))
            {
                if (ImGui.MenuItem("Undo", "CTRL+Z", false, EditorActionManager.CanUndo()))
                {
                    EditorActionManager.UndoAction();
                }
                if (ImGui.MenuItem("Redo", "Ctrl+Y", false, EditorActionManager.CanRedo()))
                {
                    EditorActionManager.RedoAction();
                }
                if (ImGui.MenuItem("Delete", "Delete", false, true))
                {
                    //TODO2: delete
                }
                if (ImGui.MenuItem("Duplicate", "Ctrl+D", false, true))
                {
                    //TODO2: dupe row
                }
                ImGui.EndMenu();
            }
            if (FMGBank.AssetLocator.Type != GameType.DarkSoulsIISOTFS)
            {
                //TODO2 FMG: handle however ds2 does it
                if (ImGui.BeginMenu("Text Language", FMGBank.IsLoaded))
                {
                    var folderList = FMGBank.AssetLocator.GetMsgLanguages();
                    foreach (var fullpath in folderList)
                    {
                        var foldername = fullpath.Split("\\").Last();
                        if (ImGui.MenuItem(foldername, true))
                        {
                            FMGBank.ReloadFMGs(foldername); //load specified language
                            //ImGui.Columns(1);
                            ClearFMGCache();
                        }
                    }
                    ImGui.EndMenu();
                }
            }
            if (ImGui.BeginMenu("Import/Export", FMGBank.IsLoaded))
            {
                if (ImGui.MenuItem("Import Files", true))
                {
                    if (FMGBank.ImportFMGs())
                    {
                        ClearFMGCache();
                    }
                }
                if (ImGui.MenuItem("Export All Text", true))
                {
                    FMGBank.ExportFMGs();
                }
                ImGui.EndMenu();
            }
        }
        
        private string _FMGsearchStr = "";
        private string _FMGsearchStrCache = "";

        private void FMGSearchGUI()
        {

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
            ImGui.PopStyleColor();
            ImGui.Begin("Search Text", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.Modal);
            ImGui.Columns(2);
            if (ImGui.Button("Clear Text"))
                _FMGsearchStr = "";
            ImGui.NextColumn();
            ImGui.InputText("Search...", ref _FMGsearchStr, 255);

            //TODO2: regex crap
            /*
            Regex propSearchRx = null;
            try
            {
                propSearchRx = new Regex(_FMGsearchStr.ToLower());
            }
            catch
            {
            }
            */

            if (_cachedEntries != null)
            {
                if (_FMGsearchStr != _FMGsearchStrCache)
                {
                    _cachedEntriesFiltered = _cachedEntries;
                    List<FMG.Entry> matches = new();

                    if (_activeFmgType == FMGBank.FMGTypes.Item)
                    {
                        //item
                        foreach (var entry in FMGBank.GetItemFMGEntriesByType(_activeItemCategory, FMGBank.ItemType.Title).ToList())
                        {
                            if (entry.ID.ToString().Contains(_FMGsearchStr, StringComparison.CurrentCultureIgnoreCase))
                                matches.Add(entry);
                            else if (entry.Text != null)
                            {
                                if (entry.Text.Contains(_FMGsearchStr, StringComparison.CurrentCultureIgnoreCase))
                                    matches.Add(entry);
                            }
                        }
                        foreach (var entry in FMGBank.GetItemFMGEntriesByType(_activeItemCategory, FMGBank.ItemType.Description).ToList())
                        {
                            if (entry.Text != null)
                            {
                                if (entry.Text.Contains(_FMGsearchStr, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    var search = _cachedEntries.Find(e => e.ID == entry.ID && !matches.Contains(e));
                                    if (search != null)
                                        matches.Add(search);
                                }
                            }
                        }
                        foreach (var entry in FMGBank.GetItemFMGEntriesByType(_activeItemCategory, FMGBank.ItemType.Summary).ToList())
                        {
                            if (entry.Text != null)
                            {
                                if (entry.Text.Contains(_FMGsearchStr, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    var search = _cachedEntries.Find(e => e.ID == entry.ID && !matches.Contains(e));
                                    if (search != null)
                                        matches.Add(search);
                                }
                            }
                        }
                    }
                    else
                    {
                        //menu
                        foreach (var entry in FMGBank.GetMenuFMGEntries(_activeMenuCategoryPair.Value).ToList())
                        {
                            if (entry.Text != null)
                            {
                                if (entry.Text.Contains(_FMGsearchStr, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    var search = _cachedEntries.Find(e => e.ID == entry.ID && !matches.Contains(e));
                                    if (search != null)
                                        matches.Add(search);
                                }
                            }
                        }
                        
                    }

                    _cachedEntriesFiltered = matches;
                    _FMGsearchStrCache = _FMGsearchStr;
                }
                else if (_cachedEntries != _cachedEntriesFiltered && _FMGsearchStr == "")
                {
                    if (_activeFmgType == FMGBank.FMGTypes.Item)
                        _cachedEntriesFiltered = _cachedEntries;//FMGBank.GetItemFMGEntriesByType(_activeItemCategory, FMGBank.ItemType.Title);
                    else
                        _cachedEntriesFiltered = _cachedEntries;//FMGBank.GetMenuFMGEntries(_active);
                }
            }
            ImGui.End();
            //ImGui.Separator();
        }

        private void EditorGUI(bool doFocus)
        {
            //TODO2: hide mostly-useless FMGs option

            if (!FMGBank.IsLoaded)
            {
                if (FMGBank.IsLoading)
                {
                    ImGui.Text("Loading...");
                }
                return;
            }
            var dsid = ImGui.GetID("DockSpace_TextEntries");
            ImGui.DockSpace(dsid, new Vector2(0, 0), ImGuiDockNodeFlags.None);

            FMGSearchGUI();//

            ImGui.Columns(2);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(8.0f, 8.0f));
            ImGui.Begin("Text Categories");
            ImGui.Separator();
            ImGui.Text("  Item Text");
            ImGui.Separator();

            foreach (var cat in _displayCategories)
            {
                //if (Enum.IsDefined(typeof(FMGBank.ItemFMGTypes)) && cat != _activeItemCategory) //check if this fmg should be hidden
                
                    if (ImGui.Selectable($@" {cat.ToString()}", cat == _activeItemCategory))
                    {
                        _activeItemCategory = cat;
                        _activeMenuCategoryPair = new(FMGBank.MenuFMGTypes.None, null);
                        _cachedEntriesFiltered = FMGBank.GetItemFMGEntriesByType(cat, FMGBank.ItemType.Title);
                        _cachedEntries = _cachedEntriesFiltered;
                        _activeFmgType = FMGBank.FMGTypes.Item;
                        _activeEntry = null;
                    }
                
                if (doFocus && cat == _activeItemCategory)
                {
                    ImGui.SetScrollHereY();
                }
            }
            //ImGui.EndChild();

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Text("  Menu Text");
            ImGui.Separator();

            foreach (var cat in FMGBank.GetMenuFMGs())
            {
                if (ImGui.Selectable($@" {cat.Key.ToString()}", cat.Key == _activeMenuCategoryPair.Key))
                {
                    _activeItemCategory = FMGBank.ItemCategory.None;
                    _activeMenuCategoryPair = cat;
                    _cachedEntriesFiltered = FMGBank.GetMenuFMGEntries(cat.Value);
                    _cachedEntries = _cachedEntriesFiltered;
                    _activeFmgType = FMGBank.FMGTypes.Menu;
                    _activeEntry = null;
                }
                if (doFocus && cat.Key == _activeMenuCategoryPair.Key)
                {
                    ImGui.SetScrollHereY();
                }
            }
            ImGui.End();

            ImGui.PopStyleVar();
            ImGui.NextColumn();

            //ImGui.BeginChild("TextRows");
            ImGui.Begin("Text Entries");

            if (_activeItemCategory == FMGBank.ItemCategory.None && _activeMenuCategoryPair.Key == FMGBank.MenuFMGTypes.None)
            {
                ImGui.Text("Select a category to see items");
            }
            else
            {
                foreach (var r in _cachedEntriesFiltered)
                {
                    var text = (r.Text == null) ? "%null%" : r.Text;
                    if (ImGui.Selectable($@"{r.ID} {text}", _activeEntry == r))
                    {
                        _activeEntry = r;
                        if (_activeFmgType == FMGBank.FMGTypes.Item)
                        {
                            FMGBank.LookupItemID(r.ID, _activeItemCategory, out _cachedTitle, out _cachedSummary, out _cachedDescription);
                        }
                        else
                        {
                            _cachedTitle = r;
                        }
                    }
                    if (doFocus && _activeEntry == r)
                    {
                        ImGui.SetScrollHereY();
                    }
                }
            }
            ImGui.End();
            //ImGui.NextColumn();
            //ImGui.BeginMenu("text");

            ImGui.Begin("Text");
            if (_activeEntry == null)
            {
                ImGui.Text("Select an item to edit text");
            }
            else
            {
                //_propEditor.PropEditorParamRow(_activeRow);
                ImGui.Columns(2);
                ImGui.SetColumnWidth(0, 100);
                ImGui.Text("ID");
                ImGui.NextColumn();
                int id = _activeEntry.ID;
                ImGui.InputInt("##id", ref id);
                ImGui.NextColumn();

                _propEditor.PropEditorFMGBegin();
                if (_activeFmgType == FMGBank.FMGTypes.Item)
                {
                    if (_cachedTitle != null)
                    {
                        _propEditor.PropEditorFMG(_cachedTitle, "Title", -1.0f);
                    }

                    if (_cachedSummary != null)
                    {
                        _propEditor.PropEditorFMG(_cachedSummary, "Summary", 80.0f);
                    }

                    if (_cachedDescription != null)
                    {
                        _propEditor.PropEditorFMG(_cachedDescription, "Description", 160.0f);
                    }
                }
                else
                {
                    _propEditor.PropEditorFMG(_activeEntry, "Text", 160.0f);
                }
                _propEditor.PropEditorFMGEnd();
            }
            ImGui.End();
            //ImGui.EndChild();
        }

        private void EditorGUIDS2(bool doFocus)
        {
            if (FMGBank.DS2Fmgs == null)
            {
                return;
            }

            ImGui.Columns(3);
            ImGui.BeginChild("categories");
            foreach (var cat in FMGBank.DS2Fmgs.Keys)
            {
                if (ImGui.Selectable($@" {cat}", cat == _activeCategoryDS2))
                {
                    _activeCategoryDS2 = cat;
                    _cachedEntriesFiltered = FMGBank.DS2Fmgs[cat].Entries;
                }
                if (doFocus && cat == _activeCategoryDS2)
                {
                    ImGui.SetScrollHereY();
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            FMGSearchGUI();//

            ImGui.BeginChild("rows");
            if (_activeCategoryDS2 == null)
            {
                ImGui.Text("Select a category to see items");
            }
            else
            {
                foreach (var r in _cachedEntriesFiltered)
                {
                    var text = (r.Text == null) ? "%null%" : r.Text;
                    if (ImGui.Selectable($@"{r.ID} {text}", _activeEntry == r))
                    {
                        _activeEntry = r;
                    }
                    if (doFocus && _activeEntry == r)
                    {
                        ImGui.SetScrollHereY();
                    }
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();
            ImGui.BeginChild("text");
            if (_activeEntry == null)
            {
                ImGui.Text("Select an item to edit text");
            }
            else
            {
                //_propEditor.PropEditorParamRow(_activeRow);
                ImGui.Columns(2);
                ImGui.Text("ID");
                ImGui.NextColumn();
                int id = _activeEntry.ID;
                ImGui.InputInt("##id", ref id);
                ImGui.NextColumn();


                _propEditor.PropEditorFMGBegin();
                /*ImGui.Text("Text");
                ImGui.NextColumn();
                string text = (_activeEntry.Text != null) ? _activeEntry.Text : "";
                ImGui.InputTextMultiline("##description", ref text, 1000, new Vector2(-1, 160.0f));
                ImGui.NextColumn();*/
                _propEditor.PropEditorFMG(_activeEntry, "Text", 160.0f);
                _propEditor.PropEditorFMGEnd();
            }
            ImGui.EndChild();
        }

        public void OnGUI(string[] initcmd)
        {
            if (FMGBank.AssetLocator == null)
            {
                return;
            }

            // Docking setup
            //var vp = ImGui.GetMainViewport();
            var wins = ImGui.GetWindowSize();
            var winp = ImGui.GetWindowPos();
            winp.Y += 20.0f;
            wins.Y -= 20.0f;
            ImGui.SetNextWindowPos(winp);
            ImGui.SetNextWindowSize(wins);
            ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            flags |= ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
            flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
            flags |= ImGuiWindowFlags.NoBackground;
            //ImGui.Begin("DockSpace_MapEdit", flags);
            //var dsid = ImGui.GetID("DockSpace_ParamEdit");
            //ImGui.DockSpace(dsid, new Vector2(0, 0));

            // Keyboard shortcuts
            if (EditorActionManager.CanUndo() && InputTracker.GetControlShortcut(Key.Z))
            {
                EditorActionManager.UndoAction();
            }
            if (EditorActionManager.CanRedo() && InputTracker.GetControlShortcut(Key.Y))
            {
                EditorActionManager.RedoAction();
            }

            bool doFocus = false;
            // Parse select commands
            if (initcmd != null && initcmd[0] == "select")
            {
                if (initcmd.Length > 1)
                {
                    doFocus = true;
                    if (_activeFmgType == FMGBank.FMGTypes.Item)
                    {
                        foreach (var cat in _displayCategories)
                        {
                            if (cat.ToString() == initcmd[1])
                            {
                                _activeItemCategory = cat;
                                _activeMenuCategoryPair = new(FMGBank.MenuFMGTypes.None, null);
                                _cachedEntriesFiltered = FMGBank.GetItemFMGEntriesByType(cat, FMGBank.ItemType.Title);
                                _cachedEntries = _cachedEntriesFiltered;
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
                                _cachedEntriesFiltered = FMGBank.GetMenuFMGEntries(cat.Value);
                                _cachedEntries = _cachedEntriesFiltered;
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
                            var r = _cachedEntriesFiltered.FirstOrDefault(r => r.ID == id);
                            if (r != null)
                            {
                                _activeEntry = r;
                                if (_activeFmgType == FMGBank.FMGTypes.Item)
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

            if (FMGBank.AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                EditorGUIDS2(doFocus);
            }
            else
            {
                EditorGUI(doFocus);
            }
        }

        public override void OnProjectChanged(ProjectSettings newSettings)
        {
            ClearFMGCache();
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
