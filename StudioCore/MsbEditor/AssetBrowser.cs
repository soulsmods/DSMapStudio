using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImGuiNET;
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

        private List<string> _chrCache = new List<string>();
        private List<string> _objCache = new List<string>();
        private List<string> _partsCache = new List<string>();
        private Dictionary<string, List<string>> _mapModelCache = new Dictionary<string, List<string>>();

        private List<string> _cacheFiltered = new();

        private AssetLocator _locator;

        private AssetBrowserEventHandler _handler;

        private string _selected = null;
        private string _selectedCache = null;

        private string _searchStrInput = "";
        private string _searchStrInputCache = "";

        private StudioCore.Assetdex.Assetdex _assetdex;

        public AssetBrowser(AssetBrowserEventHandler handler, string id, AssetLocator locator, Assetdex.Assetdex assetdex)
        {
            _id = id;
            _locator = locator;
            _handler = handler;
            _assetdex = assetdex;
        }

        public void ClearCaches()
        {
            _chrCache = new List<string>();
            _objCache = new List<string>();
            _mapModelCache = new Dictionary<string, List<string>>();
            var mapList = _locator.GetFullMapList();
            foreach (var m in mapList)
            {
                var adjm = _locator.GetAssetMapID(m);
                if (!_mapModelCache.ContainsKey(adjm))
                {
                    _mapModelCache.Add(adjm, null);
                }
            }
        }

        public void OnGui()
        {
            if (ImGui.Begin($@"Asset Browser##{_id}"))
            {
                ImGui.Columns(2);
                ImGui.BeginChild("AssetTypeList");
                if (ImGui.Selectable("Chr", _selected == "Chr"))
                {
                    _chrCache = _locator.GetChrModels();
                    _selected = "Chr";
                }
                string objLabel = "Obj";
                if (_locator.Type is GameType.EldenRing or GameType.ArmoredCoreVI)
                {
                    objLabel = "Aeg";
                }
                if (ImGui.Selectable(objLabel, _selected == "Obj"))
                {
                    _objCache = _locator.GetObjModels();
                    _selected = "Obj";
                }
                if (ImGui.Selectable("Parts", _selected == "Parts"))
                {
                    _partsCache = _locator.GetPartsModels();
                    _selected = "Parts";
                }
                foreach (var m in _mapModelCache.Keys)
                {
                    if (ImGui.Selectable(m, _selected == m))
                    {
                        if (_mapModelCache[m] == null)
                        {
                            var modelList = _locator.GetMapModels(m);
                            var cache = new List<string>();
                            foreach (var model in modelList)
                            {
                                cache.Add(model.AssetName);
                            }
                            _mapModelCache[m] = cache;
                        }
                        _selected = m;
                    }
                }
                ImGui.EndChild();
                ImGui.NextColumn();
                ImGui.BeginChild("AssetListSearch");

                if (InputTracker.GetKeyDown(KeyBindings.Current.Map_PropSearch))
                    ImGui.SetKeyboardFocusHere();
                ImGui.InputText($"Search <{KeyBindings.Current.Map_PropSearch.HintText}>", ref _searchStrInput, 255);
                
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();

                ImGui.BeginChild("AssetList");

                if (_selected == "Chr")
                {
                    if (_searchStrInput != _searchStrInputCache || _selected != _selectedCache)
                    {
                        _cacheFiltered = _chrCache;
                        _searchStrInputCache = _searchStrInput;
                        _selectedCache = _selected;
                    }
                    foreach (var chr in _cacheFiltered)
                    {
                        string fileName = $"{chr}";
                        string referenceName = "";
                        List<string> tagList = new List<string>();

                        if (AssetdexUtil.assetReferenceDict_Chr.ContainsKey(chr))
                        {
                            referenceName = AssetdexUtil.assetReferenceDict_Chr[chr].referenceName;
                            tagList = AssetdexUtil.assetReferenceDict_Chr[chr].tagList;
                            fileName = fileName + $" <{referenceName}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                string tagString = String.Join(" ", tagList);
                                fileName = $"{fileName} {{ {tagString} }}";
                            }
                        }

                        if (Utils.IsSearchFilterMatch(_searchStrInput, chr, referenceName, tagList))
                        {
                            if (ImGui.Selectable(fileName))
                            {
                            }
                            if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                            {
                                _handler.OnInstantiateChr(chr);
                            }
                        }
                    }
                }
                else if (_selected == "Obj")
                {
                    if (_searchStrInput != _searchStrInputCache || _selected != _selectedCache)
                    {
                        _cacheFiltered = _objCache;
                        _searchStrInputCache = _searchStrInput;
                        _selectedCache = _selected;
                    }
                    foreach (var obj in _cacheFiltered)
                    {
                        string fileName = $"{obj}";
                        string referenceName = "";
                        List<string> tagList = new List<string>();

                        if (AssetdexUtil.assetReferenceDict_Obj.ContainsKey(obj))
                        {
                            referenceName = AssetdexUtil.assetReferenceDict_Obj[obj].referenceName;
                            tagList = AssetdexUtil.assetReferenceDict_Obj[obj].tagList;
                            fileName = fileName + $" <{referenceName}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                string tagString = String.Join(" ", tagList);
                                fileName = $"{fileName} {{ {tagString} }}";
                            }
                        }

                        if (Utils.IsSearchFilterMatch(_searchStrInput, obj, referenceName, tagList))
                        {
                            if (ImGui.Selectable(fileName))
                            {
                            }
                            if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                            {
                                _handler.OnInstantiateObj(obj);
                            }
                        }
                    }
                }
                else if (_selected == "Parts")
                {
                    if (_searchStrInput != _searchStrInputCache || _selected != _selectedCache)
                    {
                        _cacheFiltered = _partsCache;
                        _searchStrInputCache = _searchStrInput;
                        _selectedCache = _selected;
                    }

                    foreach (var part in _cacheFiltered)
                    {
                        string fileName = $"{part}";
                        string referenceName = "";
                        List<string> tagList = new List<string>();

                        if (AssetdexUtil.assetReferenceDict_Part.ContainsKey(part))
                        { 
                            referenceName = AssetdexUtil.assetReferenceDict_Part[part].referenceName;
                            tagList = AssetdexUtil.assetReferenceDict_Part[part].tagList;
                            fileName = fileName + $" <{referenceName}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                string tagString = String.Join(" ", tagList);
                                fileName = $"{fileName} {{ {tagString} }}";
                            }
                        }

                        if (Utils.IsSearchFilterMatch(_searchStrInput, part, referenceName, tagList))
                        {
                            if (ImGui.Selectable(fileName))
                            {
                            }
                            if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                            {
                                _handler.OnInstantiateParts(part);
                            }
                        }
                    }
                }
                else if (_selected != null && _selected.StartsWith("m"))
                {
                    if (_mapModelCache.ContainsKey(_selected))
                    {
                        if (_searchStrInput != _searchStrInputCache || _selected != _selectedCache)
                        {
                            _cacheFiltered = _mapModelCache[_selected];
                            _searchStrInputCache = _searchStrInput;
                            _selectedCache = _selected;
                        }
                        foreach (var model in _cacheFiltered)
                        {
                            string fileName = $"{model}";
                            string referenceName = "";
                            List<string> tagList = new List<string>();

                            if (AssetdexUtil.assetReferenceDict_MapPiece.ContainsKey(model))
                            {
                                referenceName = AssetdexUtil.assetReferenceDict_MapPiece[model].referenceName;
                                tagList = AssetdexUtil.assetReferenceDict_MapPiece[model].tagList;
                                fileName = fileName + $" <{referenceName}>";

                                if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                                {
                                    string tagString = String.Join(" ", tagList);
                                    fileName = $"{fileName} {{ {tagString} }}";
                                }
                            }

                            if (Utils.IsSearchFilterMatch(_searchStrInput, model, referenceName, tagList))
                            {
                                if (ImGui.Selectable(fileName))
                                {
                                }
                                if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                                {
                                    _handler.OnInstantiateMapPiece(_selected, model);
                                }
                            }
                        }
                    }
                }
                ImGui.EndChild();
                ImGui.EndChild();
            }
            ImGui.End();
        }
    }
}
