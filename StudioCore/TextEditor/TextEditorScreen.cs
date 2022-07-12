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
        private FMG _activeDS2FMG = null;
        private List<FMG.Entry> _cachedEntriesFiltered = null;
        private List<FMG.Entry> _cachedEntries = null;
        private FMG.Entry _activeEntry = null;
        private int _cachedID = 0;


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
            _activeCategoryDS2 = null;
            _activeDS2FMG = null;
            _activeEntry = null;
            _cachedID = 0;
            _FMGsearchStr = "";
            _FMGsearchStrCache = "";

            _cachedTitle = null;
            _cachedSummary = null;
            _cachedDescription = null;
        }
        private void RefreshFMGCache()
        {
            _FMGsearchStr = "";
            _FMGsearchStrCache = "";
            if (_activeFmgType == FMGBank.FMGTypes.Item)
            {
                _cachedEntriesFiltered = FMGBank.GetItemFMGEntriesByType(_activeItemCategory, FMGBank.ItemType.Title);
                _cachedEntries = _cachedEntriesFiltered;
            }
            else if (_activeFmgType == FMGBank.FMGTypes.Menu)
            {
                _activeItemCategory = FMGBank.ItemCategory.None;
                _cachedEntriesFiltered = FMGBank.GetMenuFMGEntries(_activeMenuCategoryPair.Value);
                _cachedEntries = _cachedEntriesFiltered;
            }
        }

        //not an action ATM because that would likely require a full FMG system rewrite
        private void TempActionDeleteEntry()
        {
            //todo: replace me with action
            if (_activeFmgType == FMGBank.FMGTypes.Item)
            {
                //item
                FMG.Entry title;
                FMG.Entry summary;
                FMG.Entry desc;

                FMGBank.LookupItemID(_activeEntry.ID, _activeItemCategory, out title, out summary, out desc);

                if (title != null)
                {
                    var fmg = FMGBank.FindFMGForEntry_Item(title); //Very dumb.
                    DeleteFMGEntry(fmg, title);
                }
                if (summary != null)
                {
                    var fmg = FMGBank.FindFMGForEntry_Item(summary); //Very dumb.
                    DeleteFMGEntry(fmg, summary);
                }
                if (desc != null)
                {
                    var fmg = FMGBank.FindFMGForEntry_Item(desc); //Very dumb.
                    DeleteFMGEntry(fmg, desc);
                }
            }
            else if (_activeFmgType == FMGBank.FMGTypes.DS2)
            {
                //ds2
                FMG.Entry entry = _activeEntry;
                DeleteFMGEntry(_activeDS2FMG, entry);

            }
            else
            {
                //menu
                FMG.Entry entry = _activeEntry;
                DeleteFMGEntry(_activeMenuCategoryPair.Value, entry);

            }
            
            /*
            var index = _cachedEntries.IndexOf(_activeEntry);
            if (_cachedEntries.Count > index + 1)
                _activeEntry = _cachedEntries[index + 1];
            else
                _activeEntry = _cachedEntries[index - 1];
            */
            _activeEntry = null;
            _cachedTitle = null;
            _cachedSummary = null;
            _cachedDescription = null;
        }
        private void DeleteFMGEntry(FMG fmg, FMG.Entry entry)
        {
            fmg.Entries.Remove(entry);
            _cachedEntries.Remove(entry);
            return;
        }


        //not an action ATM because that would likely require a full FMG system rewrite
        private void TempActionDupeEntry()
        {
            //todo: replace me with action
            if (_activeFmgType == FMGBank.FMGTypes.Item)
            {
                //items
                FMGBank.LookupItemID(_activeEntry.ID, _activeItemCategory, out FMG.Entry title, out FMG.Entry summary, out FMG.Entry desc);

                if (title != null)
                {
                    var fmg = FMGBank.FindFMGForEntry_Item(title); //Very dumb.
                    var newEntry = DuplicateFMGEntry(fmg, title);
                    //_cachedEntries.Insert(_cachedEntries.IndexOf(title) + 1, newEntry);
                    _cachedEntries.Insert(_cachedEntries.FindIndex(e => e.ID == newEntry.ID - 1) + 1, newEntry);
                    _activeEntry = newEntry;
                    _cachedTitle = newEntry;
                }
                else
                {
                    throw new Exception("Error: FMG duplicate could not find 'title'");
                }
                if (summary != null)
                {
                    var fmg = FMGBank.FindFMGForEntry_Item(summary); //Very dumb.
                    DuplicateFMGEntry(fmg, summary);
                    _cachedSummary = summary;
                }
                if (desc != null)
                {
                    var fmg = FMGBank.FindFMGForEntry_Item(desc); //Very dumb.
                    DuplicateFMGEntry(fmg, desc);
                    _cachedDescription = desc;
                }
            }
            else if (_activeFmgType == FMGBank.FMGTypes.DS2)
            {
                //ds2
                FMG.Entry text = _activeEntry;
                var newEntry = DuplicateFMGEntry(_activeDS2FMG, text);
                //_cachedEntries.Insert(_cachedEntries.IndexOf(text) + 1, newEntry);
                //_cachedEntries.Insert(_cachedEntries.FindIndex(e => e.ID == newEntry.ID - 1) + 1, newEntry);
                _activeEntry = newEntry;

            }
            else
            {
                //menu
                FMG.Entry text = _activeEntry;
                var newEntry = DuplicateFMGEntry(_activeMenuCategoryPair.Value, text);
                //_cachedEntries.Insert(_cachedEntries.IndexOf(text) + 1, newEntry);
                _cachedEntries.Insert(_cachedEntries.FindIndex(e => e.ID == newEntry.ID - 1) + 1, newEntry);
                _activeEntry = newEntry;

            }
        }
        private FMG.Entry DuplicateFMGEntry(FMG fmg, FMG.Entry entry)
        {
            FMG.Entry newentry = new(entry.ID, entry.Text);

            do
            {
                newentry.ID++; //get an unused ID
            }
            while (fmg.Entries.Find(e => e.ID == newentry.ID) != null);

            //fmg.Entries.Insert(fmg.Entries.IndexOf(entry) + 1, newentry);
            fmg.Entries.Insert(fmg.Entries.FindIndex(e => e.ID == newentry.ID-1) + 1, newentry);

            //RefreshFMGCache();
            return newentry;
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
                if (ImGui.MenuItem("Delete Entry", "Ctrl+Delete", false, false || _activeEntry != null))
                {
                    TempActionDeleteEntry(); //todo2: replace with action (GOOD LUCK)
                }
                if (ImGui.MenuItem("Duplicate Entry", "Ctrl+D", false, _activeEntry != null))
                {
                    TempActionDupeEntry(); //todo2: replace with action
                }
                ImGui.EndMenu();
            }
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
            if (ImGui.BeginMenu("Import/Export", FMGBank.IsLoaded))
            {
                if (ImGui.MenuItem("Import Files"))
                {
                    if (FMGBank.ImportFMGs())
                    {
                        ClearFMGCache();
                    }
                }
                if (ImGui.MenuItem("Export All Text"))
                {
                    FMGBank.ExportFMGs();
                }
                ImGui.EndMenu();
            }
        }
        
        private string _FMGsearchStr = "";
        private string _FMGsearchStrCache = "";

        private void FMGSearchLogic()
        {

            // Do we really need regex here? Eh.
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

                    //TaskManager.Run("SearchFMGs", false, false, true, () =>
                    if (_activeFmgType == FMGBank.FMGTypes.Item)
                    {
                        //item
                        List<FMG.Entry> searchEntries;
                        if (_FMGsearchStr.Length > _FMGsearchStrCache.Length)
                            searchEntries = _cachedEntriesFiltered;
                        else
                            searchEntries = _cachedEntries;//FMGBank.GetItemFMGEntriesByType(_activeItemCategory, FMGBank.ItemType.Title).ToList()

                        foreach (var entry in searchEntries)
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
                        List<FMG.Entry> searchEntries;
                        if (_FMGsearchStr.Length > _FMGsearchStrCache.Length)
                            searchEntries = _cachedEntriesFiltered;
                        else
                            searchEntries = _cachedEntries;// FMGBank.GetMenuFMGEntries(_activeMenuCategoryPair.Value).ToList();

                        foreach (var entry in searchEntries)
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
                        _cachedEntriesFiltered = _cachedEntries;
                    else
                        _cachedEntriesFiltered = _cachedEntries;
                }
            }
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
                        _FMGsearchStr = "";
                        _FMGsearchStrCache = "";
                }

                if (doFocus && cat == _activeItemCategory)
                {
                    ImGui.SetScrollHereY();
                }
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Text("  Menu Text");
            ImGui.Separator();

            foreach (var cat in FMGBank.GetMenuFMGs())
            {
                if (ImGui.Selectable($@" {FMGBank.MenuEnumString(cat.Key)}", cat.Key == _activeMenuCategoryPair.Key))
                {
                    _activeItemCategory = FMGBank.ItemCategory.None;
                    _activeMenuCategoryPair = cat;
                    _cachedEntriesFiltered = FMGBank.GetMenuFMGEntries(cat.Value);
                    _cachedEntries = _cachedEntriesFiltered;
                    _activeFmgType = FMGBank.FMGTypes.Menu;
                    _activeEntry = null;
                    _FMGsearchStr = "";
                    _FMGsearchStrCache = "";
                }
                if (doFocus && cat.Key == _activeMenuCategoryPair.Key)
                {
                    ImGui.SetScrollHereY();
                }
            }
            ImGui.End();

            ImGui.Begin("Text Entries");
            //text entry search
            if (ImGui.Button("Clear Text"))
                _FMGsearchStr = "";
            ImGui.SameLine();

            if (InputTracker.GetControlShortcut(Key.F))
                ImGui.SetKeyboardFocusHere();
            ImGui.InputText("Search <Ctrl+F>", ref _FMGsearchStr, 255);

            FMGSearchLogic();

            ImGui.BeginChild("Text Entry List");
            //actual text entries

            if (_activeItemCategory == FMGBank.ItemCategory.None && _activeMenuCategoryPair.Key == FMGBank.MenuFMGTypes.None)
            {
                ImGui.Text("Select a category to see items");
            }
            else
            {
                foreach (var r in _cachedEntriesFiltered.ToList())
                {
                    var text = (r.Text == null) ? "%null%" : r.Text;
                    if (ImGui.Selectable($@"{r.ID} {text}", _activeEntry == r))
                    {
                        _activeEntry = r;
                        if (_activeFmgType == FMGBank.FMGTypes.Item)
                        {
                            //TODO2: do this in a faster way?
                            FMGBank.LookupItemID(r.ID, _activeItemCategory, out _cachedTitle, out _cachedSummary, out _cachedDescription);
                        }
                        else
                        {
                            _cachedTitle = r;
                        }
                    }
                    if (ImGui.BeginPopupContextItem())
                    {
                        _activeEntry = r;
                        if (ImGui.Selectable("Duplicate Entry"))
                        {
                            if (_activeFmgType == FMGBank.FMGTypes.Item)
                            {
                                //TODO2: do this in a faster way?
                                FMGBank.LookupItemID(r.ID, _activeItemCategory, out _cachedTitle, out _cachedSummary, out _cachedDescription);
                            }
                            else
                            {
                                _cachedTitle = r;
                            }
                            TempActionDupeEntry();
                        }
                        ImGui.EndPopup();
                        //todo: put delete entry in here once they are implemented via aciton (currently not in because doing it by mistake would be bad)
                    }
                    if (doFocus && _activeEntry == r)
                    {
                        ImGui.SetScrollHereY();
                    }
                }
            }
            ImGui.EndChild();
            ImGui.End();

            ImGui.Begin("Text");
            if (_activeEntry == null)
            {
                ImGui.Text("Select an item to edit text");
            }
            else
            {
                ImGui.Columns(2);
                ImGui.SetColumnWidth(0, 100);
                ImGui.Text("ID");
                ImGui.NextColumn();
                //int id = _activeEntry.ID;
                ImGui.InputInt("##id", ref _activeEntry.ID);

                if (_cachedID != _activeEntry.ID)
                {
                    //ID was changed, make sure it's not a dupe.
                    if (_cachedEntries.Count(e => e.ID == _activeEntry.ID) > 1)
                    {
                        //ID is a dupe, go pick an unused one instead.
                        do
                        {
                            _activeEntry.ID++;
                        } while (_cachedEntries.Count(e => e.ID == _activeEntry.ID) > 1);
                    }
                    _cachedID = _activeEntry.ID;
                }

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
        }

        private void EditorGUIDS2(bool doFocus)
        {
            

            /*
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

            FMGSearchLogic();//todo2: update imgui structure (needs children), actually test

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
                //ImGui.Text("Text");
                //ImGui.NextColumn();
                //string text = (_activeEntry.Text != null) ? _activeEntry.Text : "";
                //ImGui.InputTextMultiline("##description", ref text, 1000, new Vector2(-1, 160.0f));
                //ImGui.NextColumn();
                _propEditor.PropEditorFMG(_activeEntry, "Text", 160.0f);
                _propEditor.PropEditorFMGEnd();
            }
            ImGui.EndChild();
            */


            if (!FMGBank.IsLoaded)
            {
                if (FMGBank.IsLoading)
                {
                    ImGui.Text("Loading...");
                }
                return;
            }
            if (FMGBank.DS2Fmgs == null)
            {
                return;
            }

            var dsid = ImGui.GetID("DockSpace_TextEntries");
            ImGui.DockSpace(dsid, new Vector2(0, 0), ImGuiDockNodeFlags.None);

            ImGui.Begin("Text Categories");
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Text("  Text (-bloodmes, -talk)");
            ImGui.Separator();

            foreach (var cat in FMGBank.DS2Fmgs.Keys)
            {
                if (ImGui.Selectable($@" {cat}", cat == _activeCategoryDS2))
                {
                    _activeCategoryDS2 = cat;
                    _cachedEntriesFiltered = FMGBank.DS2Fmgs[cat].Entries;
                    _cachedEntries = _cachedEntriesFiltered;
                    _activeEntry = null;
                    _FMGsearchStr = "";
                    _FMGsearchStrCache = "";
                    _activeDS2FMG = FMGBank.DS2Fmgs[cat];
                    _activeFmgType = FMGBank.FMGTypes.DS2;
                }
                if (doFocus && cat == _activeCategoryDS2)
                {
                    ImGui.SetScrollHereY();
                }
            }

            ImGui.End();

            ImGui.Begin("Text Entries");
            //text entry search
            if (ImGui.Button("Clear Text"))
                _FMGsearchStr = "";
            ImGui.SameLine();

            if (InputTracker.GetControlShortcut(Key.F))
                ImGui.SetKeyboardFocusHere();
            ImGui.InputText("Search <Ctrl+F>", ref _FMGsearchStr, 255);

            FMGSearchLogic();

            ImGui.BeginChild("Text Entry List");
            //actual text entries

            if (_activeCategoryDS2 == null)
            {
                ImGui.Text("Select a category to see items");
            }
            else
            {
                foreach (var r in _cachedEntriesFiltered.ToList())
                {
                    var text = (r.Text == null) ? "%null%" : r.Text;
                    if (ImGui.Selectable($@"{r.ID} {text}", _activeEntry == r))
                    {
                        _activeEntry = r;
                        _cachedTitle = r;
                    }
                    if (ImGui.BeginPopupContextItem())
                    {
                        _activeEntry = r;
                        if (ImGui.Selectable("Duplicate Entry"))
                        {
                            _cachedTitle = r;
                            TempActionDupeEntry();
                        }
                        ImGui.EndPopup();
                        //todo: put delete entry in here once they are implemented via aciton (currently not in because doing it by mistake would be bad)
                    }
                    if (doFocus && _activeEntry == r)
                    {
                        ImGui.SetScrollHereY();
                    }
                }
            }
            ImGui.EndChild();
            ImGui.End();

            ImGui.Begin("Text");
            if (_activeEntry == null)
            {
                ImGui.Text("Select an item to edit text");
            }
            else
            {
                ImGui.Columns(2);
                ImGui.SetColumnWidth(0, 100);
                ImGui.Text("ID");
                ImGui.NextColumn();
                //int id = _activeEntry.ID;
                ImGui.InputInt("##id", ref _activeEntry.ID);

                if (_cachedID != _activeEntry.ID)
                {
                    //ID was changed, make sure it's not a dupe.
                    if (_cachedEntries.Count(e => e.ID == _activeEntry.ID) > 1)
                    {
                        //ID is a dupe, go pick an unused one instead.
                        do
                        {
                            _activeEntry.ID++;
                        } while (_cachedEntries.Count(e => e.ID == _activeEntry.ID) > 1);
                    }
                    _cachedID = _activeEntry.ID;
                }

                ImGui.NextColumn();

                _propEditor.PropEditorFMGBegin();
                _propEditor.PropEditorFMG(_activeEntry, "Text", 160.0f);
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
            if (InputTracker.GetControlShortcut(Key.Delete) && _activeEntry != null)
            {
                TempActionDeleteEntry(); //todo2: turn to action
            }
            if (InputTracker.GetControlShortcut(Key.D) && _activeEntry != null)
            {
                TempActionDupeEntry(); //todo2: turn to action
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
