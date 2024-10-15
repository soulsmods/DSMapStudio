using Microsoft.Extensions.Logging;
using SoulsFormats;
using StudioCore.Banks;
using StudioCore.Banks.AliasBank;
using StudioCore.Editor;
using StudioCore.Renderer.Gui;
using StudioCore.Editor;
using StudioCore.MsbEditor;
using StudioCore.Platform;
using StudioCore.Renderer.Scene;
using StudioCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using static Andre.Native.ImGuiBindings;
using ActionManager = StudioCore.Editor.ActionManager;
using CompoundAction = StudioCore.Editor.CompoundAction;
namespace StudioCore.Editors.AssetBrowser;

public enum AssetBrowserSource
{
    MapEditor,
    ModelEditor
}

public enum AssetCategoryType
{
    None,
    Character,
    Asset,
    Part,
    MapPiece
}

public class AssetBrowserScreen
{
    private AssetCategoryType _selectedAssetType = AssetCategoryType.None;
    private AssetCategoryType _selectedAssetTypeCache = AssetCategoryType.None;

    private AssetBrowserSource SourceType;
    private MsbEditorScreen MapEditor;
    private ModelEditorScreen ModelEditor;
    private ActionManager _actionManager;
    private RenderScene _scene;
    private Selection _selection;
    private IViewport _viewport;
    private Universe _universe;

    private List<string> _characterNameCache = new List<string>();
    private List<string> _objectNameCache = new List<string>();
    private List<string> _partNameCache = new List<string>();
    private Dictionary<string, List<string>> _mapPieceNameCache = new Dictionary<string, List<string>>();

    private List<string> _loadedMaps = new List<string>();
    private Dictionary<string, List<string>> _mapModelNameCache = new Dictionary<string, List<string>>();

    private string _selectedAssetMapId = "";
    private string _selectedAssetMapIdCache = null;

    private string _searchInput = "";
    private string _searchInputCache = "";

    private string _refUpdateId = "";
    private string _refUpdateName = "";
    private string _refUpdateTags = "";

    private string _selectedName;

    private bool updateScrollPosition = false;
    private float _currentScrollY;

    Dictionary<string, AliasReference> chrReferenceDict = new Dictionary<string, AliasReference>();
    Dictionary<string, AliasReference> assetReferenceDict = new Dictionary<string, AliasReference>();
    Dictionary<string, AliasReference> partReferenceDict = new Dictionary<string, AliasReference>();
    Dictionary<string, AliasReference> mapPieceReferenceDict = new Dictionary<string, AliasReference>();


    public AssetBrowserScreen(AssetBrowserSource sourceType, Universe universe, RenderScene scene, Selection sel, ActionManager manager, EditorScreen editor, IViewport viewport)
    {
        SourceType = sourceType;

        MapEditor = null;
        ModelEditor = null;

        if (sourceType == AssetBrowserSource.MapEditor)
        {
            MapEditor = (MsbEditorScreen)editor;
        }
        if (sourceType == AssetBrowserSource.ModelEditor)
        {
            ModelEditor = (ModelEditorScreen)editor;
        }

        _scene = scene;
        _selection = sel;
        _actionManager = manager;
        _universe = universe;

        _viewport = viewport;

        _selectedName = null;
    }

    public void OnProjectChanged()
    {
        if (Locator.AssetLocator.Type != GameType.Undefined)
        {
            _characterNameCache = Locator.AssetLocator.GetChrModels();
            _objectNameCache = Locator.AssetLocator.GetObjModels();
            _partNameCache = Locator.AssetLocator.GetPartsModels();
            _mapPieceNameCache = new Dictionary<string, List<string>>();

            _selectedAssetMapId = "";
            _selectedAssetMapIdCache = null;
            _selectedAssetType = AssetCategoryType.None;
            _selectedAssetTypeCache = AssetCategoryType.None;

            while (ModelAliasBank.Bank.IsLoadingAliases)
            {
                Thread.Sleep(50);
            }
            if (ModelAliasBank.Bank.AliasNames == null)
            {
                TaskLogs.AddLog("Unable to initialise ModelAliasBank", LogLevel.Error);
                return;
            }

            foreach (AliasReference v in ModelAliasBank.Bank.AliasNames.GetEntries("Characters"))
            {
                if (!chrReferenceDict.ContainsKey(v.id))
                {
                    chrReferenceDict.Add(v.id, v);
                }
            }
            foreach (AliasReference v in ModelAliasBank.Bank.AliasNames.GetEntries("Objects"))
            {
                if (!assetReferenceDict.ContainsKey(v.id))
                {
                    assetReferenceDict.Add(v.id, v);
                }
            }
            foreach (AliasReference v in ModelAliasBank.Bank.AliasNames.GetEntries("Parts"))
            {
                if (!partReferenceDict.ContainsKey(v.id))
                {
                    partReferenceDict.Add(v.id, v);
                }
            }
            foreach (AliasReference v in ModelAliasBank.Bank.AliasNames.GetEntries("MapPieces"))
            {
                if (!mapPieceReferenceDict.ContainsKey(v.id))
                {
                    mapPieceReferenceDict.Add(v.id, v);
                }
            }

            List<string> mapList = Locator.ActiveProject.MSBBank.GetFullMapList();

            foreach (var mapId in mapList)
            {
                var assetMapId = Locator.AssetLocator.GetAssetMapID(mapId);

                List<AssetDescription> modelList = new List<AssetDescription>();

                if (Locator.AssetLocator.Type is GameType.DarkSoulsIISOTFS)
                {
                    modelList = Locator.AssetLocator.GetMapModelsFromBXF(mapId);
                }
                else
                {
                    modelList = Locator.AssetLocator.GetMapModels(mapId);
                }

                var cache = new List<string>();
                foreach (var model in modelList)
                {
                    cache.Add(model.AssetName);
                }

                if (!_mapPieceNameCache.ContainsKey(assetMapId))
                {
                    _mapPieceNameCache.Add(assetMapId, cache);
                }
            }
        }
    }

    public void OnGui()
    {
        var scale = MapStudioNew.GetUIScale();

        if (Locator.AssetLocator.Type == GameType.Undefined)
            return;

        // View toggles
        /*
        if (SourceType == AssetBrowserSource.MapEditor)
        {
            if (!CFG.Current.Interface_MapEditor_AssetBrowser)
                return;
        }
        if (SourceType == AssetBrowserSource.ModelEditor)
        {
            if (!CFG.Current.Interface_ModelEditor_AssetBrowser)
                return;
        }
        */

        if (ModelAliasBank.Bank.IsLoadingAliases)
            return;

        ImGui.PushStyleColorVec4(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
        ImGui.SetNextWindowSize(new Vector2(300.0f, 200.0f) * scale, ImGuiCond.FirstUseEver);

        if (ImGui.Begin($@"Asset Browser: Category##{SourceType}AssetBrowser_CategoryList"))
        {
            ImGui.Separator();
            Editor.EditorDecorations.WrappedText("Categories:");
            ImGui.Separator();

            DisplayCategoryList();
        }

        ImGui.End();
        ImGui.PopStyleColor(1);

        ImGui.PushStyleColorVec4(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
        ImGui.SetNextWindowSize(new Vector2(300.0f, 200.0f) * scale, ImGuiCond.FirstUseEver);

        if (ImGui.Begin($@"Asset Browser: Contents##{SourceType}AssetBrowser_ContentList"))
        {
            DisplayTopSection();

            ImGui.Separator();
            Editor.EditorDecorations.WrappedText("Assets:");
            ImGui.Separator();

            DisplayBrowserList(AssetCategoryType.Character, _characterNameCache, chrReferenceDict);
            DisplayBrowserList(AssetCategoryType.Asset, _objectNameCache, assetReferenceDict);
            DisplayBrowserList(AssetCategoryType.Part, _partNameCache, partReferenceDict);
            DisplayBrowserList_MapPiece(AssetCategoryType.MapPiece, mapPieceReferenceDict);
        }

        ImGui.End();
        ImGui.PopStyleColor(1);

        ImGui.PushStyleColorVec4(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
        ImGui.SetNextWindowSize(new Vector2(300.0f, 200.0f) * scale, ImGuiCond.FirstUseEver);

        if (ImGui.Begin($@"Asset Browser: Actions##{SourceType}AssetBrowser_CategoryList"))
        {
            ImGui.Indent(10.0f);

            ImGui.Separator();
            Editor.EditorDecorations.WrappedText("Actions:");
            ImGui.Separator();

            if (SourceType == AssetBrowserSource.MapEditor)
            {
                DisplayActions_MapEditor();
            }
            if (SourceType == AssetBrowserSource.ModelEditor)
            {
                DisplayActions_ModelEditor();
            }

        }

        ImGui.End();
        ImGui.PopStyleColor(1);

        if (ModelAliasBank.Bank.mayReloadAliasBank)
        {
            ModelAliasBank.Bank.mayReloadAliasBank = false;
            ModelAliasBank.Bank.ReloadAliasBank();
        }
    }

    private void DisplayTopSection()
    {
        ImGui.Separator();
        ImGui.InputText($"Search", ref _searchInput, 255);
        Editor.EditorDecorations.ShowHoverTooltip("Separate terms are split via the + character.");
    }

    private void DisplayCategoryList()
    {
        var assetLabel = "Objects";

        if (Locator.AssetLocator.Type is GameType.EldenRing or GameType.ArmoredCoreVI)
        {
            assetLabel = "Assets";
        }

        if (ImGui.Selectable("Characters", _selectedAssetType == AssetCategoryType.Character))
        {
            _selectedAssetType = AssetCategoryType.Character;
            _selectedAssetMapId = "";
        }
        if (ImGui.Selectable(assetLabel, _selectedAssetType == AssetCategoryType.Asset))
        {
            _selectedAssetType = AssetCategoryType.Asset;
            _selectedAssetMapId = "";
        }

        // Only display in Model Editor since Parts aren't used anywhere in Map Editor.
        if (SourceType == AssetBrowserSource.ModelEditor)
        {
            if (ImGui.Selectable("Parts", _selectedAssetType == AssetCategoryType.Part))
            {
                _selectedAssetType = AssetCategoryType.Part;
                _selectedAssetMapId = "";
            }
        }

        foreach (var mapId in _mapPieceNameCache.Keys)
        {
            if (ImGui.Selectable($"MapPieces: {mapId}", _selectedAssetMapId == mapId))
            {
                _selectedAssetMapId = mapId;
                _selectedAssetType = AssetCategoryType.MapPiece;
            }

            if (CFG.Current.AssetBrowser_ShowAliasesInBrowser)
            {
                var labelName = AliasUtils.GetMapNameAlias(mapId);
                EditorDecorations.DisplayAlias(labelName);
            }
        }
    }

    private void DisplayBrowserList(AssetCategoryType assetType, List<string> nameCache, Dictionary<string, AliasReference> referenceDict)
    {
        if (updateScrollPosition)
        {
            updateScrollPosition = false;
            ImGui.SetScrollYFloat(_currentScrollY);
        }

        if (_selectedAssetType == assetType)
        {
            if (_searchInput != _searchInputCache || _selectedAssetType != _selectedAssetTypeCache)
            {
                _searchInputCache = _searchInput;
                _selectedAssetTypeCache = _selectedAssetType;
            }

            foreach (var name in nameCache)
            {
                var displayedName = $"{name}";
                var lowerName = name.ToLower();

                var refID = $"{name}";
                var refName = "";
                var refTagList = new List<string>();

                if (referenceDict.ContainsKey(lowerName))
                {
                    refID = referenceDict[lowerName].id;
                    refName = referenceDict[lowerName].name;
                    refTagList = referenceDict[lowerName].tags;
                }

                if (!CFG.Current.AssetBrowser_ShowLowDetailParts)
                {
                    if (_selectedAssetType == AssetCategoryType.Part)
                    {
                        if (name.Substring(name.Length - 2) == "_l")
                        {
                            continue; // Skip this entry if it is a low detail entry
                        }
                    }
                }

                if (SearchFilters.IsAssetBrowserSearchMatch(_searchInput, lowerName, refName, refTagList))
                {
                    if (ImGui.Selectable(displayedName, _selectedName == name))
                    {
                        _currentScrollY = ImGui.GetScrollY();

                        _selectedName = refID;
                        _refUpdateId = refID;
                        _refUpdateName = refName;
                        _refUpdateTags = AliasUtils.GetTagListString(refTagList);
                    }

                    // Alias
                    if (referenceDict.ContainsKey(lowerName))
                    {
                        if (CFG.Current.AssetBrowser_ShowAliasesInBrowser)
                        {
                            var aliasName = referenceDict[lowerName].name;
                            EditorDecorations.DisplayAlias(aliasName);
                        }

                        // Tags
                        if (CFG.Current.AssetBrowser_ShowTagsInBrowser)
                        {
                            var tagString = string.Join(" ", referenceDict[lowerName].tags);
                            EditorDecorations.DisplayTagAlias(tagString);
                        }
                    }
                }
            }
        }
    }

    private void DisplayBrowserList_MapPiece(AssetCategoryType assetType, Dictionary<string, AliasReference> referenceDict)
    {
        if (_selectedAssetType == assetType)
        {
            if (_mapPieceNameCache.ContainsKey(_selectedAssetMapId))
            {
                if (_searchInput != _searchInputCache || _selectedAssetType != _selectedAssetTypeCache || _selectedAssetMapId != _selectedAssetMapIdCache)
                {
                    _searchInputCache = _searchInput;
                    _selectedAssetTypeCache = _selectedAssetType;
                    _selectedAssetMapIdCache = _selectedAssetMapId;
                }

                foreach (var name in _mapPieceNameCache[_selectedAssetMapId])
                {
                    var modelName = name.Replace($"{_selectedAssetMapId}_", "m");

                    var displayedName = $"{modelName}";
                    var lowerName = name.ToLower();

                    var refID = $"{name}";
                    var refName = "";
                    var refTagList = new List<string>();

                    // Adjust the name to remove the A{mapId} section.
                    if (Locator.AssetLocator.Type is GameType.DarkSoulsPTDE or GameType.DarkSoulsRemastered)
                    {
                        displayedName = displayedName.Replace($"A{_selectedAssetMapId.Substring(1, 2)}", "");
                    }

                    if (referenceDict.ContainsKey(lowerName))
                    {
                        refID = referenceDict[lowerName].id;
                        refName = referenceDict[lowerName].name;
                        refTagList = referenceDict[lowerName].tags;
                    }

                    if (SearchFilters.IsSearchMatch(_searchInput, lowerName, refName, refTagList, true))
                    {
                        if (ImGui.Selectable(displayedName, _selectedName == name))
                        {
                            _selectedName = refID;

                            _refUpdateId = refID;
                            _refUpdateName = refName;
                            _refUpdateTags = AliasUtils.GetTagListString(refTagList);
                        }

                        // Alias
                        if (referenceDict.ContainsKey(lowerName))
                        {
                            if (CFG.Current.AssetBrowser_ShowAliasesInBrowser)
                            {
                                var aliasName = referenceDict[lowerName].name;
                                EditorDecorations.DisplayAlias(aliasName);
                            }

                            // Tags
                            if (CFG.Current.AssetBrowser_ShowTagsInBrowser)
                            {
                                var tagString = string.Join(" ", referenceDict[lowerName].tags);
                                EditorDecorations.DisplayTagAlias(tagString);
                            }
                        }
                    }
                }
            }
        }
    }

    private void DisplayActions_MapEditor()
    {
        if (_selectedName == null || _selectedName == "")
            return;

        Editor.EditorDecorations.WrappedText("Apply the selected asset attributes to your current object selection.");
        Editor.EditorDecorations.WrappedText("");

        ImGui.Checkbox("Update Name of Selected Object", ref CFG.Current.AssetBrowser_UpdateName);
        Editor.EditorDecorations.ShowHoverTooltip("Update the Name property of the selected entity when it is changed to a selected asset.");

        if (Locator.AssetLocator.Type is GameType.EldenRing or GameType.ArmoredCoreVI)
        {
            ImGui.Checkbox("Update Instance ID of Selected Object", ref CFG.Current.AssetBrowser_UpdateInstanceID);
            Editor.EditorDecorations.ShowHoverTooltip("Update the Name property of the selected entity when it is changed to a selected asset.");
            Editor.EditorDecorations.WrappedText("");
        }

        if (ImGui.Button("Apply##action_Asset_Apply", new Vector2(200, 32)))
        {
            ApplyMapAssetSelection();
        }
        Editor.EditorDecorations.WrappedText("");

        ImGui.Separator();
        Editor.EditorDecorations.WrappedText("Alias:");
        ImGui.Separator();

        Editor.EditorDecorations.WrappedText("Update the stored name and tag list for the selected asset here.");
        Editor.EditorDecorations.WrappedText("");

        Editor.EditorDecorations.WrappedText("Name:");
        ImGui.InputText($"##Name", ref _refUpdateName, 255);
        Editor.EditorDecorations.ShowHoverTooltip("Alias name given to this asset.");
        Editor.EditorDecorations.WrappedText("");

        Editor.EditorDecorations.WrappedText("Tags:");
        ImGui.InputText($"##Tags", ref _refUpdateTags, 255);
        Editor.EditorDecorations.ShowHoverTooltip("Tags associated with this asset. Tags are separated with the , character.");
        Editor.EditorDecorations.WrappedText("");

        if (ImGui.Button("Update##action_AssetAlias_Update", new Vector2(200, 32)))
        {
            UpdateAssetAlias();
        }
        ImGui.SameLine();
        if (ImGui.Button("Restore Default##action_AssetAlias_Restore", new Vector2(200, 32)))
        {
            RestoreAssetAlias();
        }
    }

    private void DisplayActions_ModelEditor()
    {
        if (_selectedName == null || _selectedName == "")
            return;

        Editor.EditorDecorations.WrappedText("Load the selected asset.");
        Editor.EditorDecorations.WrappedText("");

        if (ImGui.Button("Load##action_Asset_Load", new Vector2(200, 32)))
        {
            LoadModelAssetSelection();
        }
        Editor.EditorDecorations.WrappedText("");

        ImGui.Separator();
        Editor.EditorDecorations.WrappedText("Alias:");
        ImGui.Separator();

        Editor.EditorDecorations.WrappedText("Update the stored name and tag list for the selected asset here.");
        Editor.EditorDecorations.WrappedText("");

        Editor.EditorDecorations.WrappedText("Name:");
        ImGui.InputText($"##Name", ref _refUpdateName, 255);
        Editor.EditorDecorations.ShowHoverTooltip("Alias name given to this asset.");
        Editor.EditorDecorations.WrappedText("");

        Editor.EditorDecorations.WrappedText("Tags:");
        ImGui.InputText($"##Tags", ref _refUpdateTags, 255);
        Editor.EditorDecorations.ShowHoverTooltip("Tags associated with this asset. Tags are separated with the , character.");
        Editor.EditorDecorations.WrappedText("");

        if (ImGui.Button("Update##action_AssetAlias_Update", new Vector2(200, 32)))
        {
            UpdateAssetAlias();
        }
        ImGui.SameLine();
        if (ImGui.Button("Restore Default##action_AssetAlias_Restore", new Vector2(200, 32)))
        {
            RestoreAssetAlias();
        }
    }

    private void UpdateAssetAlias()
    {
        ModelAliasBank.Bank.AddToLocalAliasBank(GetSelectedAssetTypeString(), _refUpdateId, _refUpdateName, _refUpdateTags);
        ImGui.CloseCurrentPopup();
        ModelAliasBank.Bank.mayReloadAliasBank = true;
    }

    private void RestoreAssetAlias()
    {
        ModelAliasBank.Bank.RemoveFromLocalAliasBank(GetSelectedAssetTypeString(), _refUpdateId);
        ModelAliasBank.Bank.mayReloadAliasBank = true;
    }

    private string GetSelectedAssetTypeString()
    {
        var assetType = "";
        switch (_selectedAssetType)
        {
            case AssetCategoryType.Character:
                assetType = "Chr";
                break;
            case AssetCategoryType.Asset:
                assetType = "Obj";
                break;
            case AssetCategoryType.Part:
                assetType = "Part";
                break;
            case AssetCategoryType.MapPiece:
                assetType = "MapPiece";
                break;
            default:
                break;
        }

        return assetType;
    }

    public void ApplyMapAssetSelection()
    {
        var modelName = _selectedName;
        var assetType = _selectedAssetType;

        if (modelName.Contains("aeg"))
        {
            modelName = modelName.Replace("aeg", "AEG");
        }

        if (assetType == AssetCategoryType.MapPiece)
        {
            SetObjectModelForSelection(modelName, assetType, _selectedAssetMapId);
        }
        else
        {
            SetObjectModelForSelection(modelName, assetType, "");
        }
    }

    public void LoadModelAssetSelection()
    {
        var modelName = _selectedName;
        var assetType = _selectedAssetType;

        if (assetType == AssetCategoryType.Character)
        {
            ModelEditor.OnInstantiateChr(modelName);
        }

        if (assetType == AssetCategoryType.Asset)
        {
            ModelEditor.OnInstantiateObj(modelName);
        }

        if (assetType == AssetCategoryType.Part)
        {
            ModelEditor.OnInstantiateParts(modelName);
        }

        if (assetType == AssetCategoryType.MapPiece)
        {
            ModelEditor.OnInstantiateMapPiece(_selectedAssetMapId, modelName);
        }
    }

    public void SetObjectModelForSelection(string modelName, AssetCategoryType assetType, string assetMapId)
    {
        var actlist = new List<EditorAction>();

        var selected = _selection.GetFilteredSelection<Entity>();

        foreach (var s in selected)
        {
            var isValidObjectType = false;

            if (assetType == AssetCategoryType.Character)
            {
                switch (Locator.AssetLocator.Type)
                {
                    case GameType.DemonsSouls:
                        if (s.WrappedObject is MSBD.Part.Enemy)
                            isValidObjectType = true;
                        break;
                    case GameType.DarkSoulsPTDE:
                    case GameType.DarkSoulsRemastered:
                        if (s.WrappedObject is MSB1.Part.Enemy)
                            isValidObjectType = true;
                        break;
                    case GameType.DarkSoulsIISOTFS:
                        break;
                    case GameType.DarkSoulsIII:
                        if (s.WrappedObject is MSB3.Part.Enemy)
                            isValidObjectType = true;
                        break;
                    case GameType.Bloodborne:
                        if (s.WrappedObject is MSBB.Part.Enemy)
                            isValidObjectType = true;
                        break;
                    case GameType.Sekiro:
                        if (s.WrappedObject is MSBS.Part.Enemy)
                            isValidObjectType = true;
                        break;
                    case GameType.EldenRing:
                        if (s.WrappedObject is MSBE.Part.Enemy)
                            isValidObjectType = true;
                        break;
                    case GameType.ArmoredCoreVI:
                        if (s.WrappedObject is MSB_AC6.Part.Enemy)
                            isValidObjectType = true;
                        break;
                    default:
                        throw new ArgumentException("Selected entity type must be Enemy");
                }
            }
            if (assetType == AssetCategoryType.Asset)
            {
                switch (Locator.AssetLocator.Type)
                {
                    case GameType.DemonsSouls:
                        if (s.WrappedObject is MSBD.Part.Object)
                            isValidObjectType = true;
                        break;
                    case GameType.DarkSoulsPTDE:
                    case GameType.DarkSoulsRemastered:
                        if (s.WrappedObject is MSB1.Part.Object)
                            isValidObjectType = true;
                        break;
                    case GameType.DarkSoulsIISOTFS:
                        if (s.WrappedObject is MSB2.Part.Object)
                            isValidObjectType = true;
                        break;
                    case GameType.DarkSoulsIII:
                        if (s.WrappedObject is MSB3.Part.Object)
                            isValidObjectType = true;
                        break;
                    case GameType.Bloodborne:
                        if (s.WrappedObject is MSBB.Part.Object)
                            isValidObjectType = true;
                        break;
                    case GameType.Sekiro:
                        if (s.WrappedObject is MSBS.Part.Object)
                            isValidObjectType = true;
                        break;
                    case GameType.EldenRing:
                        if (s.WrappedObject is MSBE.Part.Asset)
                            isValidObjectType = true;
                        break;
                    case GameType.ArmoredCoreVI:
                        if (s.WrappedObject is MSB_AC6.Part.Asset)
                            isValidObjectType = true;
                        break;
                    default:
                        throw new ArgumentException("Selected entity type must be Object/Asset");
                }
            }
            if (assetType == AssetCategoryType.MapPiece)
            {
                switch (Locator.AssetLocator.Type)
                {
                    case GameType.DemonsSouls:
                        if (s.WrappedObject is MSBD.Part.MapPiece)
                            isValidObjectType = true;
                        break;
                    case GameType.DarkSoulsPTDE:
                    case GameType.DarkSoulsRemastered:
                        if (s.WrappedObject is MSB1.Part.MapPiece)
                            isValidObjectType = true;
                        break;
                    case GameType.DarkSoulsIISOTFS:
                        if (s.WrappedObject is MSB2.Part.MapPiece)
                            isValidObjectType = true;
                        break;
                    case GameType.DarkSoulsIII:
                        if (s.WrappedObject is MSB3.Part.MapPiece)
                            isValidObjectType = true;
                        break;
                    case GameType.Bloodborne:
                        if (s.WrappedObject is MSBB.Part.MapPiece)
                            isValidObjectType = true;
                        break;
                    case GameType.Sekiro:
                        if (s.WrappedObject is MSBS.Part.MapPiece)
                            isValidObjectType = true;
                        break;
                    case GameType.EldenRing:
                        if (s.WrappedObject is MSBE.Part.MapPiece)
                            isValidObjectType = true;
                        break;
                    case GameType.ArmoredCoreVI:
                        if (s.WrappedObject is MSB_AC6.Part.MapPiece)
                            isValidObjectType = true;
                        break;
                    default:
                        throw new ArgumentException("Selected entity type must be MapPiece");
                }
            }

            if (assetType == AssetCategoryType.MapPiece)
            {
                if (s.Parent != null)
                {
                    var mapName = s.Parent.Name;
                    if (mapName != assetMapId)
                    {
                        PlatformUtils.Instance.MessageBox($"Map Pieces are specific to each map.\nYou cannot change a Map Piece in {mapName} to a Map Piece from {assetMapId}.", "Object Browser", MessageBoxButtons.OK);

                        isValidObjectType = false;
                    }
                }
                else
                {
                    isValidObjectType = false;
                }
            }

            if (isValidObjectType)
            {
                if (assetType == AssetCategoryType.MapPiece)
                {
                    // Adjust modelName for mappieces, since by default they are mXX_YY_ZZ_AA_<id>
                    string newName = modelName.Replace($"{_selectedAssetMapId}_", "m");
                    modelName = newName;
                }

                // ModelName
                actlist.Add(s.ChangeObjectProperty("ModelName", modelName));

                // Name
                if (CFG.Current.AssetBrowser_UpdateName)
                {
                    var name = GetUniqueNameString(modelName);
                    s.Name = name;
                    actlist.Add(s.ChangeObjectProperty("Name", name));
                }

                if (CFG.Current.AssetBrowser_UpdateInstanceID)
                {
                    // Name
                    if (s.WrappedObject is MSBE.Part)
                    {
                        SetUniqueInstanceID((MapEntity)s, modelName);
                    }
                }
            }
        }

        if (actlist.Any())
        {
            var action = new CompoundAction(actlist);
            _actionManager.ExecuteAction(action);
        }
    }

    public string GetUniqueNameString(string modelName)
    {
        var postfix = 0;
        var baseName = $"{modelName}_0000";

        var names = new List<string>();

        // Collect names
        foreach (var o in _universe.LoadedObjectContainers.Values)
        {
            if (o == null)
            {
                continue;
            }

            if (o is ObjectContainer m)
            {
                foreach (var ob in m.Objects)
                {
                    if (ob is MapEntity e)
                    {
                        names.Add(ob.Name);
                    }
                }
            }
        }

        var validName = false;
        while (!validName)
        {
            var matchesName = false;

            foreach (var name in names)
            {
                // Name already exists
                if (name == baseName)
                {
                    // Increment postfix number by 1
                    var old_value = postfix;
                    postfix = postfix + 1;

                    // Replace baseName postfix number
                    baseName = baseName.Replace($"{PadNameString(old_value)}", $"{PadNameString(postfix)}");

                    matchesName = true;
                }
            }

            // If it does not match any name during 1 full iteration, then it must be valid
            if (!matchesName)
            {
                validName = true;
            }
        }

        return baseName;
    }

    public void SetUniqueInstanceID(MapEntity selected, string modelName)
    {
        ObjectContainer m;
        m = _universe.GetLoadedMap(selected.MapID);

        Dictionary<ObjectContainer, HashSet<MapEntity>> mapPartEntities = new();

        if (selected.WrappedObject is MSBE.Part msbePart)
        {
            if (mapPartEntities.TryAdd(m, new HashSet<MapEntity>()))
            {
                foreach (Entity ent in m.Objects)
                {
                    if (ent.WrappedObject != null && ent.WrappedObject is MSBE.Part)
                    {
                        mapPartEntities[m].Add((MapEntity)ent);
                    }
                }
            }

            var newInstanceID = 9000; // Default start value

            while (mapPartEntities[m].FirstOrDefault(e =>
                       ((MSBE.Part)e.WrappedObject).ModelName == msbePart.ModelName
                       && ((MSBE.Part)e.WrappedObject).InstanceID == newInstanceID
                       && msbePart != (MSBE.Part)e.WrappedObject) != null)
            {
                newInstanceID++;
            }

            msbePart.InstanceID = newInstanceID;
        }
    }

    public string PadNameString(int value)
    {
        if (value < 10)
        {
            return $"000{value}";
        }

        if (value >= 10 && value < 100)
        {
            return $"00{value}";
        }

        if (value >= 100 && value < 1000)
        {
            return $"0{value}";
        }

        return $"{value}";
    }
}
