using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using Veldrid;

namespace StudioCore.MsbEditor
{
    public interface AssetBrowserEventHandler
    {
        public void OnInstantiateChr(string chrid);
        public void OnInstantiateObj(string objid);
        public void OnInstantiateMapPiece(string mapid, string modelid);
    }

    public class AssetBrowser
    {
        private string _id;

        private List<string> _chrCache = new List<string>();
        private List<string> _objCache = new List<string>();
        private Dictionary<string, List<string>> _mapModelCache = new Dictionary<string, List<string>>();

        private List<string> _cacheFiltered = new();

        private AssetLocator _locator;

        private AssetBrowserEventHandler _handler;

        private string _selected = null;
        private string _selectedCache = null;

        private string _searchStr = "";
        private string _searchStrCache = "";

        public AssetBrowser(AssetBrowserEventHandler handler, string id, AssetLocator locator)
        {
            _id = id;
            _locator = locator;
            _handler = handler;
        }

        public void RebuildCaches()
        {
            _chrCache = new List<string>();
            _objCache = new List<string>();
            _mapModelCache = new Dictionary<string, List<string>>();
            _chrCache = _locator.GetChrModels();
            _objCache = _locator.GetObjModels();
            var mapList = _locator.GetFullMapList();
            foreach (var m in mapList)
            {
                var adjm = _locator.GetAssetMapID(m);
                if (!_mapModelCache.ContainsKey(adjm))
                {
                    var modelList = _locator.GetMapModels(adjm);
                    var cache = new List<string>();
                    foreach (var model in modelList)
                    {
                        cache.Add(model.AssetName);
                    }
                    _mapModelCache.Add(adjm, cache);
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
                    _selected = "Chr";
                }
                if (ImGui.Selectable("Obj", _selected == "Obj"))
                {
                    _selected = "Obj";
                }
                foreach (var m in _mapModelCache.Keys)
                {
                    if (ImGui.Selectable(m, _selected == m))
                    {
                        _selected = m;
                    }
                }
                ImGui.EndChild();
                ImGui.NextColumn();
                ImGui.BeginChild("AssetListSearch");

                if (InputTracker.GetKeyDown(KeyBindings.Current.Map_PropSearch))
                    ImGui.SetKeyboardFocusHere();
                ImGui.InputText($"Search <{KeyBindings.Current.Map_PropSearch.HintText}>", ref _searchStr, 255);
                
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();

                ImGui.BeginChild("AssetList");

                if (_selected == "Chr")
                {
                    if (_searchStr != _searchStrCache || _selected != _selectedCache)
                    {
                        _cacheFiltered = _chrCache;
                        _searchStrCache = _searchStr;
                        _selectedCache = _selected;
                    }
                    foreach (var chr in _cacheFiltered)
                    {
                        if (chr.Contains(_searchStr))
                        {
                            if (ImGui.Selectable(chr))
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
                    if (_searchStr != _searchStrCache || _selected != _selectedCache)
                    {
                        _cacheFiltered = _objCache;
                        _searchStrCache = _searchStr;
                        _selectedCache = _selected;
                    }
                    foreach (var obj in _cacheFiltered)
                    {
                        if (obj.Contains(_searchStr))
                        {
                            if (ImGui.Selectable(obj))
                            {
                            }
                            if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                            {
                                _handler.OnInstantiateObj(obj);
                            }
                        }
                    }
                }
                else if (_selected != null && _selected.StartsWith("m"))
                {
                    if (_mapModelCache.ContainsKey(_selected))
                    {
                        if (_searchStr != _searchStrCache || _selected != _selectedCache)
                        {
                            _cacheFiltered = _mapModelCache[_selected];
                            _searchStrCache = _searchStr;
                            _selectedCache = _selected;
                        }
                        foreach (var model in _cacheFiltered)
                        {
                            if (model.Contains(_searchStr))
                            {
                                if (ImGui.Selectable(model))
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
