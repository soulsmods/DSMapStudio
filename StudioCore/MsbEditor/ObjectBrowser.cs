using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using HKX2;
using ImGuiNET;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SoulsFormats;
using SoulsFormats.KF4;
using StudioCore.Assetdex;
using StudioCore.Platform;
using StudioCore.Scene;
using Veldrid;
using static SoulsFormats.ACB;

namespace StudioCore.MsbEditor
{
    public class ObjectBrowser
    {
        private string _id;

        private List<string> _modelNameCache = new List<string>();
        private Dictionary<string, List<string>> _mapModelNameCache = new Dictionary<string, List<string>>();

        private AssetLocator _assetLocator;

        private string _selectedAssetType = null;
        private string _selectedAssetTypeCache = null;

        private string _selectedAssetMapId = null;
        private string _selectedAssetMapIdCache = null;

        private string _searchStrInput = "";
        private string _searchStrInputCache = "";

        public bool MenuOpenState = false;

        public MsbEditorScreen MsbEditor;

        private StudioCore.Assetdex.Assetdex _assetdex;

        private List<string> _loadedMaps = new List<string>();

        public ObjectBrowser(string id, AssetLocator locator, Assetdex.Assetdex assetdex)
        {
            _id = id;
            _assetLocator = locator;
            _assetdex = assetdex;

            _selectedAssetMapId = "";
        }

        public void OnProjectChanged()
        {
            if (_assetLocator.Type != GameType.Undefined)
            {
                AssetdexUtil.UpdateAssetReferences(_assetdex.resourceDict[_assetLocator.Type].GameReference[0]);

                _modelNameCache = new List<string>();
                _mapModelNameCache = new Dictionary<string, List<string>>();

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
            float scale = MapStudioNew.GetUIScale();

            if (!MenuOpenState)
                return;

            if (_assetLocator.Type == GameType.Undefined)
                return;

            ImGui.SetNextWindowSize(new Vector2(300.0f, 200.0f) * scale, ImGuiCond.FirstUseEver);

            if (ImGui.Begin($@"Object Browser##{_id}"))
            {
                if (ImGui.Button("Help"))
                {
                    ImGui.OpenPopup("##ObjectBrowserHelp");
                }
                if (ImGui.BeginPopup("##ObjectBrowserHelp"))
                {
                    ImGui.Text(
                        "OVERVIEW\n" +
                        "The Object Browser allows you to browse through all of the available characters, assets and objects.\n" +
                        "The search will filter the browser list by filename, reference name and tags.\n" +
                        "\n" +
                        "USAGE\n" +
                        "If a Enemy object is selected within the MSB view, \n" +
                        "you can click on an entry within the Chr list to change the enemy to that type.\n" +
                        "\n" +
                        "If a Asset or Obj object is selected within the MSB view, \n" +
                        "you can click on an entry within the AEG or Obj list to change the object to that type.\n"
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
                ImGui.InputText($"Search", ref _searchStrInput, 255);

                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();

                ImGui.BeginChild("AssetList");

                DisplayAssetSelectionList("Chr", AssetdexUtil.assetReferenceDict_Chr);
                DisplayAssetSelectionList("Obj", AssetdexUtil.assetReferenceDict_Obj);
                DisplayMapAssetSelectionList("MapPiece", AssetdexUtil.assetReferenceDict_MapPiece);

                ImGui.EndChild();
                ImGui.EndChild();
            }
            ImGui.End();
        }

        public void DisplayAssetTypeSelectionList()
        {
            string objLabel = "Obj";

            if (_assetLocator.Type is GameType.EldenRing or GameType.ArmoredCoreVI)
                objLabel = "AEG";

            if (ImGui.Selectable("Chr", _selectedAssetType == "Chr"))
            {
                _modelNameCache = _assetLocator.GetChrModels();
                _selectedAssetType = "Chr";
            }
            if (ImGui.Selectable(objLabel, _selectedAssetType == "Obj"))
            {
                _modelNameCache = _assetLocator.GetObjModels();
                _selectedAssetType = "Obj";
            }

            _loadedMaps.Clear();

            // Map-specific MapPieces
            foreach (string mapId in _mapModelNameCache.Keys)
            {
                foreach(var obj in MsbEditor.Universe.LoadedObjectContainers)
                {
                    if(obj.Value != null)
                    {
                        _loadedMaps.Add(obj.Key);
                    }
                }

                if (_loadedMaps.Contains(mapId))
                {
                    if (ImGui.Selectable(mapId, _selectedAssetType == mapId))
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

        public void DisplayAssetSelectionList(string assetType, Dictionary<string, AssetReference> assetDict)
        {
            if (_selectedAssetType == assetType)
            {
                if (_searchStrInput != _searchStrInputCache || _selectedAssetType != _selectedAssetTypeCache)
                {
                    _searchStrInputCache = _searchStrInput;
                    _selectedAssetTypeCache = _selectedAssetType;
                }
                foreach (string name in _modelNameCache)
                {
                    string displayName = $"{name}";

                    string referenceName = "";
                    List<string> tagList = new List<string>();

                    if (assetDict.ContainsKey(name))
                    {
                        displayName = displayName + $" <{assetDict[name].referenceName}>";

                        if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                        {
                            string tagString = string.Join(" ", assetDict[name].tagList);
                            displayName = $"{displayName} {{ {tagString} }}";
                        }

                        referenceName = assetDict[name].referenceName;
                        tagList = assetDict[name].tagList;
                    }

                    if (Utils.IsSearchFilterMatch(_searchStrInput, name, referenceName, tagList))
                    {
                        if (ImGui.Selectable(displayName))
                        {
                        }
                        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                        {
                            string modelName = name;

                            if (modelName.Contains("aeg"))
                                modelName = modelName.Replace("aeg", "AEG");

                            MsbEditor.SetObjectModelForSelection(modelName, assetType, "");
                        }
                    }
                }
            }
        }
        public void DisplayMapAssetSelectionList(string assetType, Dictionary<string, AssetReference> assetDict)
        {
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
                        string displayName = $"{modelName}";

                        string referenceName = "";
                        List<string> tagList = new List<string>();

                        if (assetDict.ContainsKey(name))
                        {
                            displayName = displayName + $" <{assetDict[name].referenceName}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                string tagString = String.Join(" ", assetDict[name].tagList);
                                displayName = $"{displayName} {{ {tagString} }}";
                            }

                            referenceName = assetDict[name].referenceName;
                            tagList = assetDict[name].tagList;
                        }

                        if (Utils.IsSearchFilterMatch(_searchStrInput, name, referenceName, tagList))
                        {
                            if (ImGui.Selectable(displayName))
                            {
                            }
                            if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                            {
                                MsbEditor.SetObjectModelForSelection(modelName, assetType, _selectedAssetMapId);
                            }
                        }
                    }
                }
            }
        }
    }
}
