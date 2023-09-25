using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using ImGuiNET;
using SoulsFormats.KF4;
using StudioCore.Assetdex;
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

    public class AssetBrowser
    {
        private string _id;

        private AssetBrowserEventHandler _handler;

        private List<string> _modelNameCache = new List<string>();
        private Dictionary<string, List<string>> _mapModelNameCache = new Dictionary<string, List<string>>();

        private AssetLocator _assetLocator;

        private string _selectedAssetType = null;
        private string _selectedAssetTypeCache = null;

        private string _searchStrInput = "";
        private string _searchStrInputCache = "";

        private StudioCore.Assetdex.Assetdex _assetdex;

        public AssetBrowser(AssetBrowserEventHandler handler, string id, AssetLocator locator, Assetdex.Assetdex assetdex)
        {
            _id = id;
            _assetLocator = locator;
            _handler = handler;
            _assetdex = assetdex;
        }

        public void OnProjectChanged()
        {
            if (_assetLocator.Type != GameType.Undefined)
            {
                AssetdexUtil.UpdateAssetReferences(_assetdex.resourceDict[_assetLocator.Type].GameReference[0]);

                _modelNameCache = new List<string>();
                _mapModelNameCache = new Dictionary<string, List<string>>();
                var mapList = _assetLocator.GetFullMapList();
                foreach (var m in mapList)
                {
                    var adjm = _assetLocator.GetAssetMapID(m);
                    if (!_mapModelNameCache.ContainsKey(adjm))
                    {
                        _mapModelNameCache.Add(adjm, null);
                    }
                }
            }
        }

        public void Display()
        {
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
                
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();

                ImGui.BeginChild("AssetList");

                DisplayAssetSelectionList("Chr", AssetdexUtil.assetReferenceDict_Chr);
                DisplayAssetSelectionList("Obj", AssetdexUtil.assetReferenceDict_Obj);
                DisplayAssetSelectionList("Parts", AssetdexUtil.assetReferenceDict_Part);
                DisplayAssetSelectionList("MapPiece", AssetdexUtil.assetReferenceDict_MapPiece);

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
            if (ImGui.Selectable("Parts", _selectedAssetType == "Parts"))
            {
                _modelNameCache = _assetLocator.GetPartsModels();
                _selectedAssetType = "Parts";
            }

            foreach (var m in _mapModelNameCache.Keys)
            {
                if (ImGui.Selectable(m, _selectedAssetType == m))
                {
                    if (_mapModelNameCache[m] == null)
                    {
                        var modelList = _assetLocator.GetMapModels(m);
                        var cache = new List<string>();
                        foreach (var model in modelList)
                        {
                            cache.Add(model.AssetName);
                        }
                        _mapModelNameCache[m] = cache;
                    }
                    _selectedAssetType = m;
                }
            }
        }

        public void DisplayAssetSelectionList(string assetType, Dictionary<string, AssetReference> assetDict)
        {
            // Chr, Obj, Parts
            if (_selectedAssetType == assetType)
            {
                if (_searchStrInput != _searchStrInputCache || _selectedAssetType != _selectedAssetTypeCache)
                {
                    _searchStrInputCache = _searchStrInput;
                    _selectedAssetTypeCache = _selectedAssetType;
                }
                foreach (var name in _modelNameCache)
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
                            if (_selectedAssetType == "Chr")
                            {
                                _handler.OnInstantiateChr(name);
                            }
                            if (_selectedAssetType == "Obj")
                            {
                                _handler.OnInstantiateObj(name);
                            }
                            if (_selectedAssetType == "Parts")
                            {
                                _handler.OnInstantiateParts(name);
                            }
                        }
                    }
                }
            }
            // MapPiece
            else if (_selectedAssetType != null && _selectedAssetType.StartsWith("m"))
            {
                if (_mapModelNameCache.ContainsKey(_selectedAssetType))
                {
                    if (_searchStrInput != _searchStrInputCache || _selectedAssetType != _selectedAssetTypeCache)
                    {
                        _searchStrInputCache = _searchStrInput;
                        _selectedAssetTypeCache = _selectedAssetType;
                    }
                    foreach (var name in _mapModelNameCache[_selectedAssetType])
                    {
                        string displayName = $"{name}";

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
                                _handler.OnInstantiateMapPiece(_selectedAssetType, name);
                            }
                        }
                    }
                }
            }
        }
    }
}
