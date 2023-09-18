using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImGuiNET;
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
        private List<string> _searchStrList = new List<string>();

        public AssetBrowser(AssetBrowserEventHandler handler, string id, AssetLocator locator)
        {
            _id = id;
            _locator = locator;
            _handler = handler;
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
                        string referenceName = "";
                        string tagList = "";
                        List<string> tags = new List<string>();

                        foreach (ChrReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).chrReferences)
                        {
                            if (chr == entry.fileName)
                            {
                                referenceName = entry.referenceName;
                                tagList = "{ ";
                                foreach (string tagEntry in entry.tags)
                                {
                                    tags.Add(tagEntry);
                                    tagList = tagList + tagEntry + " ";
                                }
                                tagList = tagList + "}";
                            }
                        }

                        if (MatchInput(chr, referenceName, tags))
                        {
                            string fullName = $"{chr}";
                            if (referenceName != "")
                                fullName = fullName + $" <{referenceName}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                fullName = fullName + " " + tagList;
                            }

                            if (ImGui.Selectable(fullName))
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
                        string referenceName = "";
                        string tagList = "";
                        List<string> tags = new List<string>();

                        foreach (ObjReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).objReferences)
                        {
                            if (obj == entry.fileName)
                            {
                                referenceName = entry.referenceName;
                                tagList = "{ ";
                                foreach (string tagEntry in entry.tags)
                                {
                                    tags.Add(tagEntry);
                                    tagList = tagList + tagEntry + " ";
                                }
                                tagList = tagList + "}";
                            }
                        }

                        if (MatchInput(obj, referenceName, tags))
                        {
                            string fullName = $"{obj}";
                            if (referenceName != "")
                                fullName = fullName + $" <{referenceName}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                fullName = fullName + " " + tagList;
                            }

                            if (ImGui.Selectable(fullName))
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
                        string referenceName = "";
                        string tagList = "";
                        List<string> tags = new List<string>();

                        foreach (PartReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).partReferences)
                        {
                            if (part == entry.fileName)
                            {
                                referenceName = entry.referenceName;
                                tagList = "{ ";
                                foreach (String tagEntry in entry.tags)
                                {
                                    tags.Add(tagEntry);
                                    tagList = tagList + tagEntry + " ";
                                }
                                tagList = tagList + "}";
                            }
                        }

                        if (MatchInput(part, referenceName, tags))
                        {
                            string fullName = $"{part}";
                            if (referenceName != "")
                                fullName = fullName + $" <{referenceName}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                fullName = fullName + " " + tagList;
                            }

                            if (ImGui.Selectable(fullName))
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
                            string referenceName = "";
                            string tagList = "";
                            List<string> tags = new List<string>();

                            foreach (MapPieceReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).mapPieceReferences)
                            {
                                if (model == entry.fileName)
                                {
                                    referenceName = entry.referenceName;
                                    tagList = "{ ";
                                    foreach (string tagEntry in entry.tags)
                                    {
                                        tags.Add(tagEntry);
                                        tagList = tagList + tagEntry + " ";
                                    }
                                    tagList = tagList + "}";
                                }
                            }

                            if (MatchInput(model, referenceName, tags))
                            {
                                string fullName = $"{model}";
                                if (referenceName != "")
                                    fullName = fullName + $" <{referenceName}>";

                                if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                                {
                                    fullName = fullName + " " + tagList;
                                }

                                if (ImGui.Selectable(fullName))
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

        public bool MatchInput(string fileName, string referenceName, List<string> tags)
        {
            // Force input to lower so it matches more readily.
            _searchStrInput = _searchStrInput.ToLower();

            // Remove braces in referenceName, and force lower to match more readily.
            referenceName = referenceName.Replace("(", "").Replace(")", "").ToLower();

            bool match = false;

            // Match input can be split via the ; delimiter
            if (_searchStrInput.Contains(";"))
                _searchStrList = _searchStrInput.Split(";").ToList();
            else
                _searchStrList = new List<string> { _searchStrInput };

            match = MatchInputSegment(tags, _searchStrList);

            // If referenceName has multiple word segments, break it up and check if input matches any of the segments
            if (referenceName.Contains(" "))
            {
                List<string> refereceNameSegement = referenceName.Split(" ").ToList();

                match = MatchInputSegment(refereceNameSegement, _searchStrList);
            }

            if (_searchStrList.Contains(fileName) || _searchStrList.Contains(referenceName))
            {
                match = true;
            }

            if (_searchStrInput == "")
                match = true;

            return match;
        }
        public bool MatchInputSegment(List<string> stringList, List<string> inputStringList)
        {
            bool match = false;

            foreach (string str in stringList)
            {
                foreach (string entry in inputStringList)
                {
                    if (entry.Contains(str))
                        match = true;
                }
            }

            return match;
        }
    }
}
