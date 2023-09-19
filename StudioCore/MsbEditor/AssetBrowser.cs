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

        public Dictionary<string, ChrReference> chrReferenceDict = new Dictionary<string, ChrReference>();
        public Dictionary<string, ObjReference> objReferenceDict = new Dictionary<string, ObjReference>();
        public Dictionary<string, PartReference> partReferenceDict = new Dictionary<string, PartReference>();
        public Dictionary<string, MapPieceReference> mapPieceReferenceDict = new Dictionary<string, MapPieceReference>();

        public AssetBrowser(AssetBrowserEventHandler handler, string id, AssetLocator locator)
        {
            _id = id;
            _locator = locator;
            _handler = handler;
        }

        public void UpdateReferenceDicts()
        {
            chrReferenceDict.Clear();
            objReferenceDict.Clear();
            partReferenceDict.Clear();
            mapPieceReferenceDict.Clear();

            foreach (ChrReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).chrReferences)
            {
                if (!chrReferenceDict.ContainsKey(entry.fileName))
                    chrReferenceDict.Add(entry.fileName, entry);
            }
            foreach (ObjReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).objReferences)
            {
                if (!objReferenceDict.ContainsKey(entry.fileName))
                    objReferenceDict.Add(entry.fileName, entry);
            }
            foreach (PartReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).partReferences)
            {
                if (!partReferenceDict.ContainsKey(entry.fileName))
                    partReferenceDict.Add(entry.fileName, entry);
            }
            foreach (MapPieceReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).mapPieceReferences)
            {
                if (!mapPieceReferenceDict.ContainsKey(entry.fileName))
                    mapPieceReferenceDict.Add(entry.fileName, entry);
            }
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

                        if (chrReferenceDict.ContainsKey(chr))
                        {
                            referenceName = chrReferenceDict[chr].referenceName;
                            tagList = chrReferenceDict[chr].tags;
                            fileName = fileName + $" <{referenceName}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                string tagString = String.Join(" ", tagList);
                                fileName = $"{fileName} {{ {tagString} }}";
                            }
                        }

                        if (MatchInput(chr, referenceName, tagList))
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

                        if (objReferenceDict.ContainsKey(obj))
                        {
                            referenceName = objReferenceDict[obj].referenceName;
                            tagList = objReferenceDict[obj].tags;
                            fileName = fileName + $" <{referenceName}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                string tagString = String.Join(" ", tagList);
                                fileName = $"{fileName} {{ {tagString} }}";
                            }
                        }

                        if (MatchInput(obj, referenceName, tagList))
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

                        if (partReferenceDict.ContainsKey(part))
                        { 
                            referenceName = partReferenceDict[part].referenceName;
                            tagList = partReferenceDict[part].tags;
                            fileName = fileName + $" <{referenceName}>";

                            if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                            {
                                string tagString = String.Join(" ", tagList);
                                fileName = $"{fileName} {{ {tagString} }}";
                            }
                        }

                        if (MatchInput(part, referenceName, tagList))
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

                            if (mapPieceReferenceDict.ContainsKey(model))
                            {
                                referenceName = mapPieceReferenceDict[model].referenceName;
                                tagList = mapPieceReferenceDict[model].tags;
                                fileName = fileName + $" <{referenceName}>";

                                if (CFG.Current.ObjectBrowser_ShowTagsInBrowser)
                                {
                                    string tagString = String.Join(" ", tagList);
                                    fileName = $"{fileName} {{ {tagString} }}";
                                }
                            }

                            if (MatchInput(model, referenceName, tagList))
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
