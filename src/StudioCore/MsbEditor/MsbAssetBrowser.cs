﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using ImGuiNET;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.HighPerformance;
using StudioCore.Aliases;
using StudioCore.Editor;
using StudioCore.Gui;
using StudioCore.ParamEditor;
using StudioCore.Scene;
using StudioCore.Utilities;
using Veldrid;
using System.IO;
using System;
using System.Threading;
using static SoulsFormats.ACB;

namespace StudioCore.MsbEditor
{
    public class MsbAssetBrowser
    {
        private readonly RenderScene _scene;

        private AssetLocator _assetLocator;
        private MsbEditorScreen _msbEditor;

        private List<string> _loadedMaps = new List<string>();
        private List<string> _modelNameCache = new List<string>();
        private Dictionary<string, List<string>> _mapModelNameCache = new Dictionary<string, List<string>>();

        private string _selectedAssetType = null;
        private string _selectedAssetTypeCache = null;

        private string _selectedAssetMapId = "";
        private string _selectedAssetMapIdCache = null;

        private string _searchInput = "";
        private string _searchInputCache = "";

        private string _refUpdateId = "";
        private string _refUpdateName = "";
        private string _refUpdateTags = "";

        private string _selectedName;

        private bool reloadModelAlias = false;

        public MsbAssetBrowser(RenderScene scene, AssetLocator locator, MsbEditorScreen editor)
        {
            _scene = scene;

            _assetLocator = locator;
            _msbEditor = editor;

            _selectedName = null;
        }

        /// <summary>
        /// Update <c>_modelNameCache</c> and <c>_mapModelNameCache</c> when the project has changed.
        /// </summary>
        public void UpdateBrowserCache()
        {
            if (_assetLocator.Type != GameType.Undefined)
            {
                _modelNameCache = new List<string>();
                _mapModelNameCache = new Dictionary<string, List<string>>();
                _selectedAssetMapId = "";
                _selectedAssetMapIdCache = null;

                List<string> mapList = _assetLocator.GetFullMapList();

                foreach (var mapId in mapList)
                {
                    var assetMapId = _assetLocator.GetAssetMapID(mapId);

                    if (!_mapModelNameCache.ContainsKey(assetMapId))
                        _mapModelNameCache.Add(assetMapId, null);
                }
            }
        }

        /// <summary>
        /// Display the Asset Browser window.
        /// </summary>
        public void OnGui()
        {
            var scale = MapStudioNew.GetUIScale();

            if (_assetLocator.Type == GameType.Undefined)
                return;

            if (ModelAliasBank.IsLoadingAliases)
                return;

            ImGui.SetNextWindowSize(new Vector2(300.0f, 200.0f) * scale, ImGuiCond.FirstUseEver);

            if (ImGui.Begin($@"Asset Browser##MsbAssetBrowser"))
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
                ImGui.InputText($"Search", ref _searchInput, 255);

                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();

                ImGui.BeginChild("AssetList");

                DisplayAssetSelectionList("Chr", ModelAliasBank._loadedAliasBank.GetChrEntries());
                DisplayAssetSelectionList("Obj", ModelAliasBank._loadedAliasBank.GetObjEntries());
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

        /// <summary>
        /// Display the asset category type selection list: Chr, Obj/AEG, Part and each map id for Map Pieces.
        /// </summary>
        private void DisplayAssetTypeSelectionList()
        {
            var objLabel = "Obj";

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

            _loadedMaps.Clear();

            // Map-specific MapPieces
            foreach (var mapId in _mapModelNameCache.Keys)
            {
                foreach (var obj in _msbEditor.Universe.LoadedObjectContainers)
                    if (obj.Value != null)
                        _loadedMaps.Add(obj.Key);

                if (_loadedMaps.Contains(mapId))
                {
                    var labelName = mapId;

                    if (AliasBank.MapNames.ContainsKey(mapId))
                        labelName = labelName + $" <{AliasBank.MapNames[mapId]}>";

                    if (ImGui.Selectable(labelName, _selectedAssetMapId == mapId))
                    {
                        if (_mapModelNameCache[mapId] == null)
                        {
                            List<AssetDescription> modelList = _assetLocator.GetMapModels(mapId);
                            var cache = new List<string>();

                            foreach (AssetDescription model in modelList)
                                cache.Add(model.AssetName);
                            _mapModelNameCache[mapId] = cache;
                        }

                        _selectedAssetMapId = mapId;
                        _selectedAssetType = "MapPiece";
                    }
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
                if (_searchInput != _searchInputCache || _selectedAssetType != _selectedAssetTypeCache)
                {
                    _searchInputCache = _searchInput;
                    _selectedAssetTypeCache = _selectedAssetType;
                }

                foreach (var name in _modelNameCache)
                {
                    var displayedName = $"{name}";
                    var lowerName = name.ToLower();

                    var refID = $"{name}";
                    var refName = "";
                    var refTagList = new List<string>();

                    // Alias contains name
                    if (referenceDict.ContainsKey(lowerName))
                    {
                        displayedName = displayedName + $" <{referenceDict[lowerName].name}>";

                        // Append tags to to displayed name
                        if (CFG.Current.AssetBrowser_ShowTagsInBrowser)
                        {
                            var tagString = string.Join(" ", referenceDict[lowerName].tags);
                            displayedName = $"{displayedName} {{ {tagString} }}";
                        }

                        refID = referenceDict[lowerName].id;
                        refName = referenceDict[lowerName].name;
                        refTagList = referenceDict[lowerName].tags;
                    }

                    if (Utils.IsSearchFilterMatch(_searchInput, lowerName, refName, refTagList))
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
                            var modelName = name;

                            if (modelName.Contains("aeg"))
                                modelName = modelName.Replace("aeg", "AEG");

                            _msbEditor.SetObjectModelForSelection(modelName, assetType, "");
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
                    if (_searchInput != _searchInputCache || _selectedAssetType != _selectedAssetTypeCache || _selectedAssetMapId != _selectedAssetMapIdCache)
                    {
                        _searchInputCache = _searchInput;
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
                        if (_assetLocator.Type == GameType.DarkSoulsPTDE || _assetLocator.Type == GameType.DarkSoulsRemastered)
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

                        if (Utils.IsSearchFilterMatch(_searchInput, lowerName, refName, refTagList))
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
                                _msbEditor.SetObjectModelForSelection(modelName, assetType, _selectedAssetMapId);
                            }
                        }
                    }
                }
            }
        }
    }
}
