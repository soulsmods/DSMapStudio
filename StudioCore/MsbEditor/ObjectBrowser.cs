using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using HKX2;
using ImGuiNET;
using SoulsFormats;
using SoulsFormats.KF4;
using StudioCore.Assetdex;
using StudioCore.Scene;
using Veldrid;

namespace StudioCore.MsbEditor
{
    public class ObjectBrowser
    {
        private string _id;

        private List<string> _modelNameCache = new List<string>();
        private Dictionary<string, List<string>> _mapModelNameCache = new Dictionary<string, List<string>>();

        private List<string> _searchFilterCache = new();

        private AssetLocator _assetLocator;

        private string _selectedAssetType = null;
        private string _selectedAssetTypeCache = null;

        private string _searchStrInput = "";
        private string _searchStrInputCache = "";

        public bool MenuOpenState = false;

        public MsbEditorScreen MsbEditor;

        private StudioCore.Assetdex.Assetdex _assetdex;

        public ObjectBrowser(string id, AssetLocator locator, Assetdex.Assetdex assetdex)
        {
            _id = id;
            _assetLocator = locator;
            _assetdex = assetdex;
        }

        public void OnProjectChanged()
        {
            if (_assetLocator.Type != GameType.Undefined)
            {
                AssetdexUtil.UpdateAssetReferences(_assetdex.resourceDict[_assetLocator.Type].GameReference[0]);
                _modelNameCache = new List<string>();
                _mapModelNameCache = new Dictionary<string, List<string>>();
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

                if (ImGui.Checkbox("Show Tags", ref CFG.Current.ObjectBrowser_ShowTagsInBrowser))
                {
                }

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

                DisplayAssetSelectionList("Chr", AssetdexUtil.assetReferenceDict_Obj);
                DisplayAssetSelectionList("Obj", AssetdexUtil.assetReferenceDict_Obj);

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
                            ChangeObjectModel(name, assetType);
                        }
                    }
                }
            }
        }

        public void ChangeObjectModel(string modelName, string modelType)
        {
            MsbEditor.SetObjectModelForSelection(modelName, modelType);
        }
    }
}
