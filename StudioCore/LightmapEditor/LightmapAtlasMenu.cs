using ImGuiNET;
using SoulsFormats;
using StudioCore.Editor;
using StudioCore.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static SoulsFormats.NVA;

namespace StudioCore.LightmapEditor
{
    public class LightmapAtlasMenu
    {
        public bool MenuOpenState = false;

        private AssetLocator _locator;

        public MsbEditor.MsbEditorScreen MsbEditor;

        private string _selected = null;

        private BTAB _currentLightmapAtlas;
        private BTAB _currentLightmapAtlasCache;

        private List<string> mapList;
        List<AssetDescription> currentAtlasAssets;
        private Dictionary<string, AssetDescription> perMapLightmapAtlasDict = new Dictionary<string, AssetDescription>();

        private string _searchStrFilename = "";
        private string _searchStrFilenameCache = "";

        private string _searchStrEntries = "";
        private string _searchStrEntriesCache = "";
        private AssetDescription _selectedLightmapAtlasFile;

        private List<int> _atlasEntryDeleteIndexList = new List<int>();

        private string wikiEntry_PartName = "The name of the target part defined in an MSB file.";
        private string wikiEntry_MaterialName = "The name of the target material in the part's FLVER model.";
        private string wikiEntry_AtlasID = "The ID of the atlas texture to use.";
        private string wikiEntry_UVOffset = "Offsets the lightmap UVs.";
        private string wikiEntry_UVScale = "Scales the lightmap UVs.";

        public LightmapAtlasMenu(AssetLocator locator) 
        {
            _locator = locator;
        }

        public void UpdateLightmapAtlasMenu()
        {
            mapList = _locator.GetFullMapList();
            perMapLightmapAtlasDict.Clear();

            List<AssetDescription> btabList = new List<AssetDescription>();

            foreach (string mapName in mapList)
            {
                currentAtlasAssets = _locator.GetMapBTABs(mapName);

                foreach (AssetDescription d in currentAtlasAssets)
                {
                    perMapLightmapAtlasDict.Add(d.AssetName, d);
                }
            }
        }

        public void Display()
        {
            float scale = MapStudioNew.GetUIScale();

            if (!MenuOpenState)
                return;

            ImGui.SetNextWindowSize(new Vector2(700.0f, 800.0f) * scale, ImGuiCond.FirstUseEver);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0f, 0f, 0f, 0.98f));
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, new Vector4(0.25f, 0.25f, 0.25f, 1.0f));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10.0f, 10.0f) * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(20.0f, 10.0f) * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 20.0f * scale);

            if (ImGui.Begin("Lightmap Atlas##Popup", ref MenuOpenState, ImGuiWindowFlags.NoDocking))
            {
                ImGui.Columns(1);

                // Filename Search
                ImGui.InputText($"File Search", ref _searchStrFilename, 255);

                if (_searchStrFilename != _searchStrFilenameCache)
                {
                    _searchStrFilenameCache = _searchStrFilename;
                }

                // Filename List
                ImGui.BeginChild("LightmapList", new Vector2(630, 100));

                foreach (var mapName in perMapLightmapAtlasDict.Keys)
                {
                    if (MatchSearchInput(_searchStrFilename, mapName))
                    {
                        if (ImGui.Selectable(mapName, _selected == mapName))
                        {
                            _selected = mapName;
                        }
                        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                        {
                            _selectedLightmapAtlasFile = perMapLightmapAtlasDict[mapName];
                            LoadLightmapAtlasFile();
                        }
                    }
                }

                ImGui.EndChild();

                ImGui.Separator();

                // Entry Search
                if (_selectedLightmapAtlasFile != null)
                    ImGui.InputText($"Entry Search", ref _searchStrEntries, 255);

                ImGui.Separator();

                // Entry Actions
                if (_selectedLightmapAtlasFile != null && ImGui.Button("Save Changes"))
                {
                    SaveLightmapAtlasFile(_selectedLightmapAtlasFile, _currentLightmapAtlas);
                }

                ImGui.SameLine();

                if (_selectedLightmapAtlasFile != null && ImGui.Button("Add New Entry"))
                {
                    AddLightmapAtlasEntry();
                }

                ImGui.Separator();

                if(_selectedLightmapAtlasFile != null)
                    ImGui.Text(_selectedLightmapAtlasFile.AssetName);
                else
                    ImGui.Text("None");

                ImGui.Separator();

                // Entry Sections
                if (_selectedLightmapAtlasFile != null)
                {
                    int index = 0;

                    if (_searchStrEntries != _searchStrEntriesCache)
                    {
                        _searchStrEntriesCache = _searchStrEntries;

                        _atlasEntryDeleteIndexList.Clear();
                    }

                    foreach (BTAB.Entry entry in _currentLightmapAtlas.Entries)
                    {
                        var PartName = _currentLightmapAtlasCache.Entries[index].PartName;
                        var MaterialName = _currentLightmapAtlasCache.Entries[index].MaterialName;
                        var AtlasID = _currentLightmapAtlasCache.Entries[index].AtlasID;
                        var UVOffset = _currentLightmapAtlasCache.Entries[index].UVOffset;
                        var UVScale = _currentLightmapAtlasCache.Entries[index].UVScale;

                        if (_searchStrEntries == "" || MatchSearchInput(_searchStrEntries, PartName))
                        {
                            EditorDecorations.HelpIcon($"##wiki_part_{entry.PartName}{index}", ref wikiEntry_PartName, false);
                            ImGui.SameLine();
                            ImGui.Text("Part Name    ");
                            ImGui.SameLine();
                            if (ImGui.InputText($"##part_{entry.PartName}{index}", ref PartName, 255))
                            {
                                _currentLightmapAtlasCache.Entries[index].PartName = PartName;
                            }

                            ImGui.SameLine();
                            if (ImGui.Button($"Delete Entry##delete_{index}"))
                            {
                                _atlasEntryDeleteIndexList.Add(index);
                            }

                            EditorDecorations.HelpIcon($"##wiki_part_{entry.MaterialName}{index}", ref wikiEntry_MaterialName, false);
                            ImGui.SameLine();
                            ImGui.Text("Material Name");
                            ImGui.SameLine();
                            if (ImGui.InputText($"##mat_{entry.MaterialName}{index}", ref MaterialName, 255)) ;
                            {
                                _currentLightmapAtlasCache.Entries[index].MaterialName = MaterialName;
                            }

                            EditorDecorations.HelpIcon($"##wiki_part_{entry.AtlasID}{index}", ref wikiEntry_AtlasID, false);
                            ImGui.SameLine();
                            ImGui.Text("Atlas ID     ");
                            ImGui.SameLine();
                            if (ImGui.InputInt($"##atlas_{entry.AtlasID}{index}", ref AtlasID))
                            {
                                _currentLightmapAtlasCache.Entries[index].AtlasID = AtlasID;
                            }

                            EditorDecorations.HelpIcon($"##wiki_part_{entry.UVOffset}{index}", ref wikiEntry_UVOffset, false);
                            ImGui.SameLine();
                            ImGui.Text("UV Offset    ");
                            ImGui.SameLine();
                            if (ImGui.InputFloat2($"##uv_offset_{entry.UVOffset}{index}", ref UVOffset))
                            {
                                _currentLightmapAtlasCache.Entries[index].UVOffset = UVOffset;
                            }

                            EditorDecorations.HelpIcon($"##wiki_part_{entry.UVScale}{index}", ref wikiEntry_UVScale, false);
                            ImGui.SameLine();
                            ImGui.Text("UV Scale     ");
                            ImGui.SameLine();
                            if (ImGui.InputFloat2($"##uv_scale_{entry.UVScale}{index}", ref UVScale))
                            {
                                _currentLightmapAtlasCache.Entries[index].UVScale = UVScale;
                            }

                            ImGui.Separator();
                        }

                        index = index + 1;
                    }

                    if(_atlasEntryDeleteIndexList.Count > 0)
                        DeleteMarkedLightmapAtlasEntries();
                }

                ImGui.End();

                ImGui.PopStyleVar(3);
                ImGui.PopStyleColor(2);
            }
        }

        public void AddLightmapAtlasEntry()
        {
            BTAB.Entry newEntry = new BTAB.Entry();
            BTAB.Entry newCachedEntry = new BTAB.Entry();
            _currentLightmapAtlas.Entries.Add(newEntry);
            _currentLightmapAtlasCache.Entries.Add(newCachedEntry);
        }

        public void DeleteMarkedLightmapAtlasEntries()
        {
            foreach(int index in _atlasEntryDeleteIndexList)
            {
                _currentLightmapAtlas.Entries.Remove(_currentLightmapAtlas.Entries[index]);
                _currentLightmapAtlasCache.Entries.Remove(_currentLightmapAtlasCache.Entries[index]);
            }

            _atlasEntryDeleteIndexList.Clear();
        }

        public void LoadLightmapAtlasFile()
        {
            try
            {
                _currentLightmapAtlas = BTAB.Read($"{_selectedLightmapAtlasFile.AssetPath}");

                // Make a new BTAB to store edits
                _currentLightmapAtlasCache = new BTAB();
                _currentLightmapAtlasCache.BigEndian = _currentLightmapAtlas.BigEndian;
                _currentLightmapAtlasCache.LongFormat = _currentLightmapAtlas.LongFormat;
                _currentLightmapAtlasCache.Entries = new List<BTAB.Entry>();

                foreach (BTAB.Entry entry in _currentLightmapAtlas.Entries)
                {
                    BTAB.Entry newEntry = new BTAB.Entry();
                    newEntry.PartName = entry.PartName;
                    newEntry.MaterialName = entry.MaterialName;
                    newEntry.AtlasID = entry.AtlasID;
                    newEntry.UVOffset = entry.UVOffset;
                    newEntry.UVScale = entry.UVScale;

                    _currentLightmapAtlasCache.Entries.Add(newEntry);
                }
            }
            catch (Exception e) when (e is DirectoryNotFoundException or FileNotFoundException)
            {
            }
        }

        public void SaveLightmapAtlasFile(AssetDescription currentAssetDescription, BTAB modifiedFile)
        {
            int index = 0;

            // Update real Lightmap Atlas data from edit Cache
            foreach (BTAB.Entry entry in _currentLightmapAtlas.Entries)
            {
                BTAB.Entry editCacheEntry = _currentLightmapAtlasCache.Entries[index];

                if (entry.PartName != editCacheEntry.PartName)
                    entry.PartName = editCacheEntry.PartName;

                if (entry.MaterialName != editCacheEntry.MaterialName)
                    entry.MaterialName = editCacheEntry.MaterialName;

                if (entry.AtlasID != editCacheEntry.AtlasID)
                    entry.AtlasID = editCacheEntry.AtlasID;

                if (entry.UVOffset != editCacheEntry.UVOffset)
                    entry.UVOffset = editCacheEntry.UVOffset;

                if (entry.UVScale != editCacheEntry.UVScale)
                    entry.UVScale = editCacheEntry.UVScale;

                index = index + 1;
            }

            // If file was loaded from game root, make new copy in mod root
            if (currentAssetDescription.AssetPath.Contains(_locator.GameRootDirectory))
            {
                currentAssetDescription.AssetPath = currentAssetDescription.AssetPath.Replace(_locator.GameRootDirectory, _locator.GameModDirectory);
            }

            try
            {
                modifiedFile.Write(currentAssetDescription.AssetPath);
                PlatformUtils.Instance.MessageBox($"{_selectedLightmapAtlasFile.AssetName} saved.", "Lightmap Atlas", MessageBoxButtons.OK);
            }
            catch (Exception e)
            {
            }
        }

        public static bool MatchSearchInput(string inputStr, string matchStr)
        {
            bool match = false;

            string curInput = inputStr.Trim().ToLower();

            matchStr = matchStr.ToLower();

            if (curInput.Equals(""))
            {
                match = true; // If input is empty, show all
                return match;
            }

            // Match String
            if (curInput == matchStr)
                match = true;

            // Matc String Segments
            string[] segments = matchStr.Split("_");
            foreach (string segment in segments)
            {
                if (curInput == segment.Trim())
                    match = true;
            }

            return match;
        }
    }
}
