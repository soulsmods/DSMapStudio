using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using ImGuiNET;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StudioCore.Assetdex;
using StudioCore.MsbEditor;

namespace StudioCore.AssetBrowser
{
    /// <summary>
    /// Class <c>AssetBrowser</c> displays a dynamic selection list of all Chr, Obj/AEG, Part and MapPiece objects for the loaded <c>GameType</c>.
    /// </summary>
    public class AssetBrowser
    {
        private string _id;
        private AssetLocator _assetLocator;
        private AssetdexCore _assetdex;
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

        private bool isMenuVisible = false;

        public AssetBrowser(string id, AssetLocator locator, AssetdexCore assetdex, MsbEditorScreen msbEditor)
        {
            _id = id;
            _assetLocator = locator;
            _assetdex = assetdex;
            _msbEditor = msbEditor;
        }

        /// <summary>
        /// Toggle the visibility of the Asset Browser.
        /// </summary>
        public void ToggleMenuVisibility()
        {
            isMenuVisible = !isMenuVisible;
        }

        /// <summary>
        /// Hidethe Asset Browser.
        /// </summary>
        public void CloseMenu()
        {
            isMenuVisible = false;
        }

        /// <summary>
        /// Update <c>_modelNameCache</c> and <c>_mapModelNameCache</c> when the project has changed.
        /// </summary>
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

        /// <summary>
        /// Display the Asset Browser window.
        /// </summary>
        public void Display()
        {
            float scale = MapStudioNew.GetUIScale();

            if (!isMenuVisible)
                return;

            if (_assetLocator.Type == GameType.Undefined)
                return;

            ImGui.SetNextWindowSize(new Vector2(300.0f, 200.0f) * scale, ImGuiCond.FirstUseEver);

            if (ImGui.Begin($@"Asset Browser##{_id}"))
            {
                if (ImGui.Button("Help"))
                {
                    ImGui.OpenPopup("##AssetBrowserHelp");
                }
                if (ImGui.BeginPopup("##AssetBrowserHelp"))
                {
                    ImGui.Text(
                        "--- OVERVIEW ---\n" +
                        "The Asset Browser allows you to browse through all of the available characters, assets and objects and map pieces.\n" +
                        "The search will filter the browser list by filename, reference name and tags.\n" +
                        "\n" +
                        "--- USAGE ---\n" +
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
                ImGui.Checkbox("Show Tags", ref CFG.Current.ObjectBrowser_ShowTagsInBrowser);

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

                DisplayAssetSelectionList("Chr", _assetdex.GetChrEntriesForGametype(_assetLocator.Type));
                DisplayAssetSelectionList("Obj", _assetdex.GetObjEntriesForGametype(_assetLocator.Type));
                DisplayMapAssetSelectionList("MapPiece", _assetdex.GetMapPieceEntriesForGametype(_assetLocator.Type));

                ImGui.EndChild();
                ImGui.EndChild();
            }
            ImGui.End();
        }

        /// <summary>
        /// Display the asset category type selection list: Chr, Obj/AEG, Part and each map id for Map Pieces.
        /// </summary>
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

            _loadedMaps.Clear();

            // Map-specific MapPieces
            foreach (string mapId in _mapModelNameCache.Keys)
            {
                foreach (var obj in _msbEditor.Universe.LoadedObjectContainers)
                {
                    if (obj.Value != null)
                    {
                        _loadedMaps.Add(obj.Key);
                    }
                }

                if (_loadedMaps.Contains(mapId))
                {
                    string labelName = mapId;

                    if (Editor.AliasBank.MapNames.ContainsKey(mapId))
                    {
                        labelName = labelName + $" <{Editor.AliasBank.MapNames[mapId]}>";
                    }

                    if (ImGui.Selectable(labelName, _selectedAssetMapId == mapId))
                    {
                        if (_mapModelNameCache[mapId] == null)
                        {
                            List<AssetDescription> modelList = _assetLocator.GetMapModels(mapId);
                            List<string> cache = new List<string>();

                            foreach (AssetDescription model in modelList)
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
        }

        /// <summary>
        /// Display the asset selection list for Chr, Obj/AEG and Parts.
        /// </summary>
        private void DisplayAssetSelectionList(string assetType, Dictionary<string, AssetReference> assetDict)
        {
            if (_selectedAssetType == assetType)
            {
                if (_searchInput != _searchInputCache || _selectedAssetType != _selectedAssetTypeCache)
                {
                    _searchInputCache = _searchInput;
                    _selectedAssetTypeCache = _selectedAssetType;
                }
                foreach (string name in _modelNameCache)
                {
                    string displayName = $"{name}";

                    string referenceName = "";
                    List<string> tagList = new List<string>();

                    string lowercaseName = name.ToLower();

                    if (assetDict.ContainsKey(lowercaseName))
                    {
                        displayName = displayName + $" <{assetDict[lowercaseName].name}>";

                        if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                        {
                            string tagString = string.Join(" ", assetDict[lowercaseName].tags);
                            displayName = $"{displayName} {{ {tagString} }}";
                        }

                        referenceName = assetDict[lowercaseName].name;
                        tagList = assetDict[lowercaseName].tags;
                    }

                    if (Utils.IsSearchFilterMatch(_searchInput, lowercaseName, referenceName, tagList))
                    {
                        if (ImGui.Selectable(displayName))
                        {
                        }
                        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                        {
                            string modelName = name;

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
        private void DisplayMapAssetSelectionList(string assetType, Dictionary<string, AssetReference> assetDict)
        {
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
                    foreach (string name in _mapModelNameCache[_selectedAssetMapId])
                    {
                        string modelName = name.Replace($"{_selectedAssetMapId}_", "m");

                        // Adjust the name to remove the A{mapId} section.
                        if (_assetLocator.Type == GameType.DarkSoulsPTDE || _assetLocator.Type == GameType.DarkSoulsRemastered)
                        {
                            modelName = modelName.Replace($"A{_selectedAssetMapId.Substring(1, 2)}", "");
                        }

                        string displayName = $"{modelName}";

                        string referenceName = "";
                        List<string> tagList = new List<string>();

                        string lowercaseName = name.ToLower();

                        if (assetDict.ContainsKey(lowercaseName))
                        {
                            displayName = displayName + $" <{assetDict[lowercaseName].name}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                string tagString = string.Join(" ", assetDict[lowercaseName].tags);
                                displayName = $"{displayName} {{ {tagString} }}";
                            }

                            referenceName = assetDict[lowercaseName].name;
                            tagList = assetDict[lowercaseName].tags;
                        }

                        if (Utils.IsSearchFilterMatch(_searchInput, name, referenceName, tagList))
                        {
                            if (ImGui.Selectable(displayName))
                            {
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
