﻿using System.Collections.Generic;
using System.Linq;
using Octokit;
using StudioCore.Banks;
using StudioCore.Banks.AliasBank;
using StudioCore.Utilities;
using static Andre.Native.ImGuiBindings;

namespace StudioCore.MsbEditor;

public enum SelectedCategoryType
{
    None,
    Character,
    Object,
    Part,
    MapPiece
}

public class ModelAssetBrowser
{
    private string _id;

    private AssetBrowserEventHandler _handler;

    private List<string> _modelNameCache = new List<string>();
    private Dictionary<string, List<string>> _mapModelNameCache = new Dictionary<string, List<string>>();

    private SelectedCategoryType _selectedAssetType = SelectedCategoryType.None;
    private SelectedCategoryType _selectedAssetTypeCache = SelectedCategoryType.None;

    private string _selectedAssetMapId = "";
    private string _selectedAssetMapIdCache = null;

    private string _searchStrInput = "";
    private string _searchStrInputCache = "";

    private string _refUpdateId = "";
    private string _refUpdateName = "";
    private string _refUpdateTags = "";

    private string _selectedName;

    public ModelAssetBrowser(AssetBrowserEventHandler handler, string id)
    {
        _id = id;
        _handler = handler;

        _selectedName = null;
    }

    public string GetSelectedCategoryNameForAliasBank()
    {
        string category = "None";

        switch (_selectedAssetType)
        {
            case SelectedCategoryType.Character:
                category = "Chr";
                break;
            case SelectedCategoryType.Object:
                category = "Obj";
                break;
            case SelectedCategoryType.Part:
                category = "Part";
                break;
            case SelectedCategoryType.MapPiece:
                category = "MapPiece";
                break;
        }

        return category;
    }

    public void OnProjectChanged()
    {
        if (Locator.AssetLocator.Type != GameType.Undefined)
        {
            _modelNameCache = new List<string>();
            _mapModelNameCache = new Dictionary<string, List<string>>();
            _selectedAssetMapId = "";
            _selectedAssetMapIdCache = null;

            List<string> mapList = Locator.AssetLocator.GetFullMapList();

            foreach (string mapId in mapList)
            {
                var assetMapId = Locator.AssetLocator.GetAssetMapID(mapId);

                if (!_mapModelNameCache.ContainsKey(assetMapId))
                {
                    _mapModelNameCache.Add(assetMapId, null);
                }
            }
        }
    }

    public void Display()
    {
        if (Locator.AssetLocator.Type == GameType.Undefined)
        {
            return;
        }

        if (ModelAliasBank.Bank.IsLoadingAliases)
        {
            return;
        }

        if (ImGui.Begin($@"Asset Browser##{_id}"))
        {
            ImGui.Columns(2);

            // Asset Type List
            ImGui.BeginChild("AssetTypeList");

            DisplayAssetTypeSelectionList();

            ImGui.EndChild();

            // Asset List
            ImGui.NextColumn();

            ImGui.BeginChild("AssetListSearch");
            ImGui.InputText($"Search", ref _searchStrInput, 255);

            ImGui.SameLine();
            ImguiUtils.ShowHelpMarker("Separate terms are split via the + character.");

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Checkbox("Show tags", ref CFG.Current.AssetBrowser_ShowTagsInBrowser);
            ImguiUtils.ShowHelpMarker("Show the tags for each entry within the browser list as part of their displayed name.");

            if (_selectedAssetType == SelectedCategoryType.Part)
            {
                ImGui.Checkbox("Show low detail models", ref CFG.Current.AssetBrowser_ShowLowDetailParts);
                ImguiUtils.ShowHelpMarker("Show the low detail part models in this list.");
            }

            ImGui.BeginChild("AssetList");

            DisplayAssetSelectionList(SelectedCategoryType.Character, ModelAliasBank.Bank.AliasNames.GetEntries("Characters"));
            DisplayAssetSelectionList(SelectedCategoryType.Object, ModelAliasBank.Bank.AliasNames.GetEntries("Objects"));
            DisplayAssetSelectionList(SelectedCategoryType.Part, ModelAliasBank.Bank.AliasNames.GetEntries("Parts"));
            DisplayMapAssetSelectionList(SelectedCategoryType.MapPiece, ModelAliasBank.Bank.AliasNames.GetEntries("MapPieces"));

            ImGui.EndChild();
            ImGui.EndChild();
        }
        ImGui.End();

        if (ModelAliasBank.Bank.mayReloadAliasBank)
        {
            ModelAliasBank.Bank.mayReloadAliasBank = false;
            ModelAliasBank.Bank.ReloadAliasBank();
        }
    }
    private void DisplayAssetTypeSelectionList()
    {
        var objLabel = "Obj";

        if (Locator.AssetLocator.Type is GameType.EldenRing or GameType.ArmoredCoreVI)
        {
            objLabel = "AEG";
        }

        if (ImGui.Selectable("Chr", _selectedAssetType == SelectedCategoryType.Character))
        {
            _modelNameCache = Locator.AssetLocator.GetChrModels();
            _selectedAssetType = SelectedCategoryType.Character;
            _selectedAssetMapId = "";
        }
        if (ImGui.Selectable(objLabel, _selectedAssetType == SelectedCategoryType.Object))
        {
            _modelNameCache = Locator.AssetLocator.GetObjModels();
            _selectedAssetType = SelectedCategoryType.Object;
            _selectedAssetMapId = "";
        }
        if (ImGui.Selectable("Part", _selectedAssetType == SelectedCategoryType.Part))
        {
            _modelNameCache = Locator.AssetLocator.GetPartsModels();
            _selectedAssetType = SelectedCategoryType.Part;
            _selectedAssetMapId = "";
        }

        foreach (var mapId in _mapModelNameCache.Keys)
        {
            var labelName = mapId;

            if (MapAliasBank.Bank.MapNames != null)
            {
                if (MapAliasBank.Bank.MapNames.ContainsKey(mapId))
                {
                    labelName = labelName + $" <{MapAliasBank.Bank.MapNames[mapId]}>";
                }

                if (ImGui.Selectable(labelName, _selectedAssetMapId == mapId))
                {
                    if (_mapModelNameCache[mapId] == null)
                    {
                        var modelList = Locator.AssetLocator.GetMapModels(mapId);
                        var cache = new List<string>();
                        foreach (var model in modelList)
                        {
                            cache.Add(model.AssetName);
                        }

                        _mapModelNameCache[mapId] = cache;
                    }

                    _selectedAssetMapId = mapId;
                    _selectedAssetType = SelectedCategoryType.MapPiece;
                }
            }
        }
    }

    /// <summary>
    /// Display the asset selection list for Chr, Obj/AEG and Parts.
    /// </summary>
    private void DisplayAssetSelectionList(SelectedCategoryType assetType, List<AliasReference> referenceList)
    {
        var referenceDict = new Dictionary<string, AliasReference>();

        foreach (AliasReference v in referenceList)
        {
            if (!referenceDict.ContainsKey(v.id))
            {
                referenceDict.Add(v.id, v);
            }
        }

        if (_selectedAssetType == assetType)
        {
            if (_searchStrInput != _searchStrInputCache || _selectedAssetType != _selectedAssetTypeCache)
            {
                _searchStrInputCache = _searchStrInput;
                _selectedAssetTypeCache = _selectedAssetType;
            }
            foreach (var name in _modelNameCache)
            {
                var displayedName = $"{name}";
                var lowerName = name.ToLower();

                var refID = $"{name}";
                var refName = "";
                var refTagList = new List<string>();

                if (referenceDict.ContainsKey(lowerName))
                {
                    displayedName = displayedName + $" <{referenceDict[lowerName].name}>";

                    if (CFG.Current.AssetBrowser_ShowTagsInBrowser)
                    {
                        var tagString = string.Join(" ", referenceDict[lowerName].tags);
                        displayedName = $"{displayedName} {{ {tagString} }}";
                    }

                    refID = referenceDict[lowerName].id;
                    refName = referenceDict[lowerName].name;
                    refTagList = referenceDict[lowerName].tags;
                }

                if (!CFG.Current.AssetBrowser_ShowLowDetailParts)
                {
                    if (_selectedAssetType == SelectedCategoryType.Part)
                    {
                        if (name.Substring(name.Length - 2) == "_l")
                        {
                            continue; // Skip this entry if it is a low detail entry
                        }
                    }
                }

                if (SearchFilters.IsSearchMatch(_searchStrInput, lowerName, refName, refTagList, true, false, true))
                {
                    if (ImGui.Selectable(displayedName))
                    {
                        _selectedName = refID;

                        _refUpdateId = refID;
                        _refUpdateName = refName;

                        if (refTagList.Count > 0)
                        {
                            var tagStr = refTagList[0];
                            foreach (var entry in refTagList.Skip(1))
                            {
                                tagStr = $"{tagStr},{entry}";
                            }
                            _refUpdateTags = tagStr;
                        }
                        else
                        {
                            _refUpdateTags = "";
                        }
                    }

                    if (_selectedName == refID)
                    {
                        if (ImGui.BeginPopupContextItem($"{refID}##context"))
                        {
                            if (ImGui.InputText($"Name", ref _refUpdateName, 255))
                            {

                            }

                            if (ImGui.InputText($"Tags", ref _refUpdateTags, 255))
                            {

                            }

                            if (ImGui.Button("Update"))
                            {
                                ModelAliasBank.Bank.AddToLocalAliasBank(GetSelectedCategoryNameForAliasBank(), _refUpdateId, _refUpdateName, _refUpdateTags);
                                ImGui.CloseCurrentPopup();
                                ModelAliasBank.Bank.mayReloadAliasBank = true;
                            }

                            ImGui.SameLine();
                            if (ImGui.Button("Restore Default"))
                            {
                                ModelAliasBank.Bank.RemoveFromLocalAliasBank(GetSelectedCategoryNameForAliasBank(), _refUpdateId);
                                ImGui.CloseCurrentPopup();
                                ModelAliasBank.Bank.mayReloadAliasBank = true;
                            }

                            ImGui.EndPopup();
                        }
                    }

                    if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClickedNil(0))
                    {
                        // TODO: fix issue with DS2 loading
                        if (Locator.AssetLocator.Type != GameType.DarkSoulsIISOTFS)
                        {
                            if (_selectedAssetType == SelectedCategoryType.Character)
                            {
                                _handler.OnInstantiateChr(name);
                            }

                            if (_selectedAssetType == SelectedCategoryType.Object)
                            {
                                _handler.OnInstantiateObj(name);
                            }

                            if (_selectedAssetType == SelectedCategoryType.Part)
                            {
                                _handler.OnInstantiateParts(name);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Display the asset selection list for Map Pieces.
    /// </summary>
    private void DisplayMapAssetSelectionList(SelectedCategoryType assetType, List<AliasReference> referenceList)
    {
        var referenceDict = new Dictionary<string, AliasReference>();

        foreach (AliasReference v in referenceList)
        {
            if (!referenceDict.ContainsKey(v.id))
            {
                referenceDict.Add(v.id, v);
            }
        }

        if (_selectedAssetType == assetType)
        {
            if (_mapModelNameCache.ContainsKey(_selectedAssetMapId))
            {
                if (_searchStrInput != _searchStrInputCache || _selectedAssetType != _selectedAssetTypeCache || _selectedAssetMapId != _selectedAssetMapIdCache)
                {
                    _searchStrInputCache = _searchStrInput;
                    _selectedAssetTypeCache = _selectedAssetType;
                    _selectedAssetMapIdCache = _selectedAssetMapId;
                }
                foreach (var name in _mapModelNameCache[_selectedAssetMapId])
                {
                    var modelName = name.Replace($"{_selectedAssetMapId}_", "m");

                    var displayedName = $"{modelName}";
                    var lowerName = name.ToLower();

                    var refID = $"{name}";
                    var refName = "";
                    var refTagList = new List<string>();

                    // Adjust the name to remove the A{mapId} section.
                    if (Locator.AssetLocator.Type == GameType.DarkSoulsPTDE || Locator.AssetLocator.Type == GameType.DarkSoulsRemastered)
                    {
                        displayedName = displayedName.Replace($"A{_selectedAssetMapId.Substring(1, 2)}", "");
                    }

                    if (referenceDict.ContainsKey(lowerName))
                    {
                        displayedName = displayedName + $" <{referenceDict[lowerName].name}>";

                        if (CFG.Current.AssetBrowser_ShowTagsInBrowser)
                        {
                            var tagString = string.Join(" ", referenceDict[lowerName].tags);
                            displayedName = $"{displayedName} {{ {tagString} }}";
                        }

                        refID = referenceDict[lowerName].id;
                        refName = referenceDict[lowerName].name;
                        refTagList = referenceDict[lowerName].tags;
                    }

                    if (SearchFilters.IsSearchMatch(_searchStrInput, lowerName, refName, refTagList, true, false, true))
                    {
                        if (ImGui.Selectable(displayedName))
                        {
                            _selectedName = refID;

                            _refUpdateId = refID;
                            _refUpdateName = refName;

                            if (refTagList.Count > 0)
                            {
                                var tagStr = refTagList[0];
                                foreach (var entry in refTagList.Skip(1))
                                {
                                    tagStr = $"{tagStr},{entry}";
                                }

                                _refUpdateTags = tagStr;
                            }
                            else
                            {
                                _refUpdateTags = "";
                            }
                        }

                        if (_selectedName == refID)
                        {
                            if (ImGui.BeginPopupContextItem($"{refID}##context"))
                            {
                                if (ImGui.InputText($"Name", ref _refUpdateName, 255))
                                {

                                }

                                if (ImGui.InputText($"Tags", ref _refUpdateTags, 255))
                                {

                                }

                                if (ImGui.Button("Update"))
                                {
                                    ModelAliasBank.Bank.AddToLocalAliasBank(GetSelectedCategoryNameForAliasBank(), _refUpdateId, _refUpdateName, _refUpdateTags);
                                    ImGui.CloseCurrentPopup();
                                    ModelAliasBank.Bank.mayReloadAliasBank = true;
                                }

                                ImGui.SameLine();
                                if (ImGui.Button("Restore Default"))
                                {
                                    ModelAliasBank.Bank.RemoveFromLocalAliasBank(GetSelectedCategoryNameForAliasBank(), _refUpdateId);
                                    ImGui.CloseCurrentPopup();
                                    ModelAliasBank.Bank.mayReloadAliasBank = true;
                                }

                                ImGui.EndPopup();
                            }
                        }

                        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClickedNil(0))
                        {
                            // TODO: fix issue with DS2 loading
                            if (Locator.AssetLocator.Type != GameType.DarkSoulsIISOTFS)
                            {
                                _handler.OnInstantiateMapPiece(_selectedAssetMapId, name);
                            }
                        }
                    }
                }
            }
        }
    }
}
