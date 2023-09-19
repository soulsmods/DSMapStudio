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
using StudioCore.MsbEditor;
using StudioCore.Scene;
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

        private string _searchStrInput = "";
        private string _searchStrInputCache = "";
        private List<string> _searchStrList = new List<string>();

        public bool MenuOpenState = false;

        public MsbEditor.MsbEditorScreen MsbEditor;

        public Dictionary<string, ChrReference> chrReferenceDict = new Dictionary<string, ChrReference>();
        public Dictionary<string, ObjReference> objReferenceDict = new Dictionary<string, ObjReference>();

        public ObjectBrowser(string id, AssetLocator locator)
        {
            _id = id;
            _locator = locator;
        }

        public void UpdateReferenceDicts()
        {
            chrReferenceDict.Clear();
            objReferenceDict.Clear();

            foreach (ChrReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).chrReferences)
            {
                if(!chrReferenceDict.ContainsKey(entry.fileName))
                    chrReferenceDict.Add(entry.fileName, entry);
            }
            foreach (ObjReference entry in AssetdexUtils.GetCurrentGameAssetdex(_locator.Type).objReferences)
            {
                if (!objReferenceDict.ContainsKey(entry.fileName))
                    objReferenceDict.Add(entry.fileName, entry);
            }
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
                if (ImGui.Button("Help"))
                {
                    ImGui.OpenPopup("##ObjectBrowserHelp");
                }
                if (ImGui.BeginPopup("##ObjectBrowserHelp"))
                {
                    ImGui.Text(
                        "OVERVIEW\n" +
                        "The Object Browser allows you to browse through all of the characters and objects/assets available.\n" +
                        "The search is fuzzy, and includes pre-defined tags.\n" +
                        "For example, typing in \"grass\" will show all objects tagged as \"grass\".\n" +
                        "\n" +
                        "Multiple filters may be chained together by adding a semi-colon between each statement.\n" +
                        "This will acts as an OR statement.\n" +
                        "\n" +
                        "USAGE\n" +
                        "If a Enemy object is selected within the MSB view, \n" +
                        "you can click on an entry within the Chr list to change the enemy to that type.\n" +
                        "\n" +
                        "If a Asset or Obj object is selected within the MSB view, \n" +
                        "you can click on an entry within the AEG or Obj list to change the enemy to that type.\n"
                        );
                    ImGui.EndPopup();
                }

                if (ImGui.Checkbox("Show Tags", ref CFG.Current.ObjectBrowser_ShowTagsInBrowser))
                {
                }

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
                ImGui.InputText($"Search", ref _searchStrInput, 255);

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
                                ChangeObjectModel(chr, "Chr");
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

        public void ChangeObjectModel(string modelName, string modelType)
        {
            MsbEditor.SetObjectModelForSelection(modelName, modelType);
        }
    }
}
