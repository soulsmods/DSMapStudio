﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using ImGuiNET;
using SoulsFormats.KF4;
using StudioCore.Aliases;
using StudioCore.Editor;
using StudioCore.Platform;
using StudioCore.Utilities;
using Veldrid;

namespace StudioCore.MsbEditor
{
    public interface AssetBrowserEventHandler
    {
        public void OnInstantiateChr(string chrid);
        public void OnInstantiateObj(string objid);
        public void OnInstantiateParts(string objid);
        public void OnInstantiateMapPiece(string mapid, string modelid);
    }

    public class ModelAssetBrowser
    {
        private string _id;

        private AssetBrowserEventHandler _handler;

        private List<string> _modelNameCache = new List<string>();
        private Dictionary<string, List<string>> _mapModelNameCache = new Dictionary<string, List<string>>();

        private AssetLocator _assetLocator;

        private string _selectedAssetType = null;
        private string _selectedAssetTypeCache = null;

        private string _selectedAssetMapId = "";
        private string _selectedAssetMapIdCache = null;

        private string _searchStrInput = "";
        private string _searchStrInputCache = "";

        private string _refUpdateId = "";
        private string _refUpdateName = "";
        private string _refUpdateTags = "";

        private string _selectedName;

        private bool reloadModelAlias = false;

        public ModelAssetBrowser(AssetBrowserEventHandler handler, string id, AssetLocator locator)
        {
            _id = id;
            _assetLocator = locator;
            _handler = handler;

            _selectedName = null;
        }

        public void OnProjectChanged()
        {
            if (_assetLocator.Type != GameType.Undefined)
            {
                _modelNameCache = new List<string>();
                _mapModelNameCache = new Dictionary<string, List<string>>();
                _selectedAssetMapId = "";
                _selectedAssetMapIdCache = null;

                List<string> mapList = _assetLocator.GetFullMapList();

                foreach (string mapId in mapList)
                {
                    var assetMapId = _assetLocator.GetAssetMapID(mapId);

                    if (!_mapModelNameCache.ContainsKey(assetMapId))
                    {
                        _mapModelNameCache.Add(assetMapId, null);
                    }
                }
            }
        }

        public void Display()
        {
            if (_assetLocator.Type == GameType.Undefined)
                return;

            if (ModelAliasBank.IsLoadingAliases)
                return;

            if (ImGui.Begin($@"Asset Browser##{_id}"))
            {
                if (ImGui.Button("Help"))
                    ImGui.OpenPopup("##AssetBrowserHelp");
                if (ImGui.BeginPopup("##AssetBrowserHelp"))
                {
                    ImGui.Text(
                        "The Asset Browser allows you to browse through all of the available characters, assets and objects and map pieces.\n" +
                        "The search will filter the browser list by filename, reference name and tags.\n" +
                        "\n" +
                        "If a Enemy is selected within the MSB view, \n" +
                        "you can click on an entry within the Chr list to change the enemy to that type.\n" +
                        "\n" +
                        "If a Asset or Obj is selected within the MSB view, \n" +
                        "you can click on an entry within the AEG or Obj list to change the object to that type.\n" +
                        "\n" +
                        "If a Map Piece is selected within the MSB view, \n" +
                        "you can click on an entry within the Map Piece list to change the object to that type.\n" +
                        "Note, you cannot apply a Map Piece asset from a different map to that of the selected Map Piece."
                        );
                    ImGui.EndPopup();
                }

                ImGui.SameLine();
                ImGui.Checkbox("Show Tags", ref CFG.Current.AssetBrowser_ShowTagsInBrowser);

                ImGui.Columns(2);

                // Asset Type List
                ImGui.BeginChild("AssetTypeList");

                DisplayAssetTypeSelectionList();

                ImGui.EndChild();

                // Asset List
                ImGui.NextColumn();

                ImGui.BeginChild("AssetListSearch");
                ImGui.InputText($"Search", ref _searchStrInput, 255);

                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();

                ImGui.BeginChild("AssetList");

                DisplayAssetSelectionList("Chr", ModelAliasBank._loadedAliasBank.GetChrEntries());
                DisplayAssetSelectionList("Obj", ModelAliasBank._loadedAliasBank.GetObjEntries());
                DisplayAssetSelectionList("Part", ModelAliasBank._loadedAliasBank.GetPartEntries());
                DisplayMapAssetSelectionList("MapPiece", ModelAliasBank._loadedAliasBank.GetMapPieceEntries());

                ImGui.EndChild();
                ImGui.EndChild();
            }
            ImGui.End();

            if (reloadModelAlias)
            {
                reloadModelAlias = false;
                ModelAliasBank.ReloadAliasBank();
            }
        }
        private void DisplayAssetTypeSelectionList()
        {
            string objLabel = "Obj";

            if (_assetLocator.Type is GameType.EldenRing or GameType.ArmoredCoreVI)
                objLabel = "AEG";

            if (ImGui.Selectable("Chr", _selectedAssetType == "Chr"))
            {
                _modelNameCache = _assetLocator.GetChrModels();
                _selectedAssetType = "Chr";
                _selectedAssetMapId = "";
            }
            if (ImGui.Selectable(objLabel, _selectedAssetType == "Obj"))
            {
                _modelNameCache = _assetLocator.GetObjModels();
                _selectedAssetType = "Obj";
                _selectedAssetMapId = "";
            }
            if (ImGui.Selectable("Part", _selectedAssetType == "Part"))
            {
                _modelNameCache = _assetLocator.GetPartsModels();
                _selectedAssetType = "Part";
                _selectedAssetMapId = "";
            }

            foreach (var mapId in _mapModelNameCache.Keys)
            {
                string labelName = mapId;

                if (AliasBank.MapNames.ContainsKey(mapId))
                {
                    labelName = labelName + $" <{AliasBank.MapNames[mapId]}>";
                }

                if (ImGui.Selectable(labelName, _selectedAssetMapId == mapId))
                {
                    if (_mapModelNameCache[mapId] == null)
                    {
                        var modelList = _assetLocator.GetMapModels(mapId);
                        var cache = new List<string>();
                        foreach (var model in modelList)
                        {
                            cache.Add(model.AssetName);
                        }
                        _mapModelNameCache[mapId] = cache;
                    }

                    _selectedAssetMapId = mapId;
                    _selectedAssetType = "MapPiece";
                }
            }
        }

        /// <summary>
        /// Display the asset selection list for Chr, Obj/AEG and Parts.
        /// </summary>
        private void DisplayAssetSelectionList(string assetType, List<ModelAliasReference> referenceList)
        {
            Dictionary<string, ModelAliasReference> referenceDict = new Dictionary<string, ModelAliasReference>();

            foreach (ModelAliasReference v in referenceList)
            {
                if (!referenceDict.ContainsKey(v.id))
                    referenceDict.Add(v.id, v);
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
                            string tagString = string.Join(" ", referenceDict[lowerName].tags);
                            displayedName = $"{displayedName} {{ {tagString} }}";
                        }

                        refID = referenceDict[lowerName].id;
                        refName = referenceDict[lowerName].name;
                        refTagList = referenceDict[lowerName].tags;
                    }

                    if (Utils.IsSearchFilterMatch(_searchStrInput, lowerName, refName, refTagList))
                    {
                        if (ImGui.Selectable(displayedName))
                        {
                            _selectedName = refID;

                            _refUpdateId = refID;
                            _refUpdateName = refName;

                            if (refTagList.Count > 0)
                            {
                                string tagStr = refTagList[0];
                                foreach (string entry in refTagList.Skip(1))
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
                                    ModelAliasBank.AddToLocalAliasBank(assetType, _refUpdateId, _refUpdateName, _refUpdateTags);
                                    ImGui.CloseCurrentPopup();
                                    reloadModelAlias = true;
                                }

                                ImGui.SameLine();
                                if (ImGui.Button("Restore Default"))
                                {
                                    ModelAliasBank.RemoveFromLocalAliasBank(assetType, _refUpdateId);
                                    ImGui.CloseCurrentPopup();
                                    reloadModelAlias = true;
                                }

                                ImGui.EndPopup();
                            }
                        }

                        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                        {
                            if (_selectedAssetType == "Chr")
                            {
                                _handler.OnInstantiateChr(name);
                            }
                            if (_selectedAssetType == "Obj")
                            {
                                _handler.OnInstantiateObj(name);
                            }
                            if (_selectedAssetType == "Part")
                            {
                                _handler.OnInstantiateParts(name);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Display the asset selection list for Map Pieces.
        /// </summary>
        private void DisplayMapAssetSelectionList(string assetType, List<ModelAliasReference> referenceList)
        {
            Dictionary<string, ModelAliasReference> referenceDict = new Dictionary<string, ModelAliasReference>();

            foreach (ModelAliasReference v in referenceList)
            {
                if (!referenceDict.ContainsKey(v.id))
                    referenceDict.Add(v.id, v);
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
                    foreach (string name in _mapModelNameCache[_selectedAssetMapId])
                    {
                        string modelName = name.Replace($"{_selectedAssetMapId}_", "m");

                        var displayedName = $"{modelName}";
                        var lowerName = name.ToLower();

                        var refID = $"{name}";
                        var refName = "";
                        var refTagList = new List<string>();

                        // Adjust the name to remove the A{mapId} section.
                        if (_assetLocator.Type == GameType.DarkSoulsPTDE || _assetLocator.Type == GameType.DarkSoulsRemastered)
                        {
                            displayedName = displayedName.Replace($"A{_selectedAssetMapId.Substring(1, 2)}", "");
                        }

                        if (referenceDict.ContainsKey(lowerName))
                        {
                            displayedName = displayedName + $" <{referenceDict[lowerName].name}>";

                            if (CFG.Current.AssetBrowser_ShowTagsInBrowser)
                            {
                                string tagString = string.Join(" ", referenceDict[lowerName].tags);
                                displayedName = $"{displayedName} {{ {tagString} }}";
                            }

                            refID = referenceDict[lowerName].id;
                            refName = referenceDict[lowerName].name;
                            refTagList = referenceDict[lowerName].tags;
                        }

                        if (Utils.IsSearchFilterMatch(_searchStrInput, lowerName, refName, refTagList))
                        {
                            if (ImGui.Selectable(displayedName))
                            {
                                _selectedName = refID;

                                _refUpdateId = refID;
                                _refUpdateName = refName;

                                if (refTagList.Count > 0)
                                {
                                    string tagStr = refTagList[0];
                                    foreach (string entry in refTagList.Skip(1))
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
                                        ModelAliasBank.AddToLocalAliasBank(assetType, _refUpdateId, _refUpdateName, _refUpdateTags);
                                        ImGui.CloseCurrentPopup();
                                        reloadModelAlias = true;
                                    }

                                    ImGui.SameLine();
                                    if (ImGui.Button("Restore Default"))
                                    {
                                        ModelAliasBank.RemoveFromLocalAliasBank(assetType, _refUpdateId);
                                        ImGui.CloseCurrentPopup();
                                        reloadModelAlias = true;
                                    }

                                    ImGui.EndPopup();
                                }
                            }

                            if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
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
