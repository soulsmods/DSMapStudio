using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using ImGuiNET;
using SoulsFormats.KF4;
using StudioCore.MsbEditor;
using Veldrid;

namespace StudioCore
{
    public class ObjectBrowser
    {
        private string _id;

        private List<string> _chrCache = new List<string>();
        private List<string> _objCache = new List<string>();
        private Dictionary<string, List<string>> _mapModelCache = new Dictionary<string, List<string>>();

        private List<string> _cacheFiltered = new();

        private AssetLocator _locator;

        private string _selected = null;
        private string _selectedCache = null;

        private string _searchStr = "";
        private string _searchStrCache = "";

        public bool MenuOpenState = false;

        public MsbEditor.MsbEditorScreen MsbEditor;

        public ObjectBrowser(string id, AssetLocator locator)
        {
            _id = id;
            _locator = locator;
        }

        public void ClearCaches()
        {
            _chrCache = new List<string>();
            _objCache = new List<string>();
        }

        public void OnGui()
        {
            float scale = MapStudioNew.GetUIScale();

            if (!MenuOpenState)
                return;

            ImGui.SetNextWindowSize(new Vector2(300.0f, 200.0f) * scale, ImGuiCond.FirstUseEver);

            if (ImGui.Begin($@"Object Browser##{_id}"))
            {
                ImGui.Checkbox("Update Name and Instance ID", ref CFG.Current.ObjectBrowser_UpdateNameAndInstanceID);

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
                        string referenceName = "";
                        List<string> tags = new List<string>();

                        foreach (ChrReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).chrReferences)
                        {
                            if (chr == entry.fileName)
                            {
                                referenceName = entry.referenceName;
                                foreach (Tag tagEntry in entry.tags)
                                {
                                    tags.Add(tagEntry.tag);
                                }
                            }
                        }

                        if (chr.Contains(_searchStr) || referenceName.Contains(_searchStr) || tags.Contains(_searchStr))
                        {
                            string fullName = $"{chr}";
                            if (referenceName != "")
                                fullName = fullName + $" <{referenceName}>";

                            if (ImGui.Selectable(fullName))
                            {
                            }
                            if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                            {
                                ChangeObjectModel(chr, "Chr");
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
                        string referenceName = "";
                        List<string> tags = new List<string>();

                        foreach (ObjReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).objReferences)
                        {
                            if (obj == entry.fileName)
                            {
                                referenceName = entry.referenceName;
                                foreach (Tag tagEntry in entry.tags)
                                {
                                    tags.Add(tagEntry.tag);
                                }
                            }
                        }

                        if (obj.Contains(_searchStr) || referenceName.Contains(_searchStr) || tags.Contains(_searchStr))
                        {
                            string fullName = $"{obj}";
                            if (referenceName != "")
                                fullName = fullName + $" <{referenceName}>";

                            if (ImGui.Selectable(fullName))
                            {
                            }
                            if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                            {
                                ChangeObjectModel(obj, "Obj");
                            }
                        }
                    }
                }

                ImGui.EndChild();
                ImGui.EndChild();
            }
            ImGui.End();
        }

        public void ChangeObjectModel(string modelName, string modelType)
        {
            MsbEditor.SetObjectModelForSelection(modelName, modelType);
        }
    }
}
