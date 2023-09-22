using ImGuiNET;
using SoulsFormats;
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

namespace StudioCore.LightmapEditor
{
    public class LightmapMenu
    {
        public bool MenuOpenState = false;

        private AssetLocator _locator;

        public MsbEditor.MsbEditorScreen MsbEditor;

        private string _selected = null;

        private BTAB _currentLightmapData;

        private List<string> mapList;
        List<AssetDescription> currentMapBtabList;
        private Dictionary<string, AssetDescription> perMapLightmapDict = new Dictionary<string, AssetDescription>();

        private string _searchStrFilename = "";
        private string _searchStrFilenameCache = "";

        private string _searchStrEntries = "";
        private string _searchStrEntriesCache = "";
        private AssetDescription _selectedLightmapFile;

        private List<BTAB.Entry> _lightmapEntriesForDeletion = new List<BTAB.Entry>();

        public LightmapMenu(AssetLocator locator) 
        {
            _locator = locator;
        }

        public void UpdateLightmapMenu()
        {
            mapList = _locator.GetFullMapList();
            perMapLightmapDict.Clear();

            List<AssetDescription> btabList = new List<AssetDescription>();

            foreach (string mapName in mapList)
            {
                currentMapBtabList = _locator.GetMapBTABs(mapName);

                foreach (AssetDescription d in currentMapBtabList)
                {
                    perMapLightmapDict.Add(d.AssetName, d);
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

            if (ImGui.Begin("Lightmap Menu##Popup", ref MenuOpenState, ImGuiWindowFlags.NoDocking))
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

                foreach (var mapName in perMapLightmapDict.Keys)
                {
                    if (_searchStrFilename == "" || Regex.IsMatch(mapName, _searchStrFilename) || Regex.IsMatch(mapName, _searchStrFilename))
                    {
                        if (ImGui.Selectable(mapName, _selected == mapName))
                        {
                            _selected = mapName;
                        }
                        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                        {
                            _selectedLightmapFile = perMapLightmapDict[mapName];
                            LoadLightmapFile();
                        }
                    }
                }

                ImGui.EndChild();

                ImGui.Separator();

                // Entry Search
                ImGui.InputText($"Entry Search", ref _searchStrEntries, 255);

                ImGui.Separator();

                // Entry Actions
                if (ImGui.Button("Save Changes"))
                {
                    SaveLightmapFile(_selectedLightmapFile, _currentLightmapData);
                }

                ImGui.SameLine();

                if (ImGui.Button("Add New Entry"))
                {
                    AddLightmapEntry();
                }

                ImGui.Separator();

                // Entry Sections
                if (_selectedLightmapFile != null)
                {
                    int index = 0;

                    if (_searchStrEntries != _searchStrEntriesCache)
                    {
                        _searchStrEntriesCache = _searchStrEntries;

                        _lightmapEntriesForDeletion.Clear(); // Clear this when the search term changes
                    }

                    foreach (BTAB.Entry entry in _currentLightmapData.Entries)
                    {
                        if (_searchStrEntries == "" || Regex.IsMatch(entry.PartName, _searchStrEntries) || Regex.IsMatch(entry.MaterialName, _searchStrEntries))
                        {
                            var PartName = entry.PartName;
                            var MaterialName = entry.MaterialName;
                            var AtlasID = entry.AtlasID;
                            var UVOffset = entry.UVOffset;
                            var UVScale = entry.UVScale;

                            ImGui.Text("Part Name    ");
                            ImGui.SameLine(); 
                            if(ImGui.InputText($"##part_{entry.PartName}{index}", ref PartName, 255))
                            {
                                entry.PartName = PartName;
                            }

                            ImGui.SameLine();
                            if (ImGui.Button("Delete Entry"))
                            {
                                if (!_lightmapEntriesForDeletion.Contains(entry))
                                    _lightmapEntriesForDeletion.Add(entry);
                            }

                            ImGui.Text("Material Name");
                            ImGui.SameLine(); 
                            if(ImGui.InputText($"##mat_{entry.MaterialName}{index}", ref MaterialName, 255));
                            {
                                entry.MaterialName = MaterialName;
                            }

                            ImGui.Text("Atlas ID     ");
                            ImGui.SameLine(); 
                            if(ImGui.InputInt($"##atlas_{entry.AtlasID}{index}", ref AtlasID, 255))
                            {
                                entry.AtlasID = AtlasID;
                            }

                            ImGui.Text("UV Offset    ");
                            ImGui.SameLine(); 
                            if(ImGui.InputFloat2($"##uv_offset_{entry.UVOffset}{index}", ref UVOffset))
                            {
                                entry.UVOffset = UVOffset;
                            }

                            ImGui.Text("UV Scale     ");
                            ImGui.SameLine(); 
                            if(ImGui.InputFloat2($"##uv_scale_{entry.UVScale}{index}", ref UVScale))
                            {
                                entry.UVScale = UVScale;
                            }

                            ImGui.Separator();

                            index = index + 1;
                        }
                    }

                    DeleteMarkedLightmapEntries();
                }

                ImGui.End();

                ImGui.PopStyleVar(3);
                ImGui.PopStyleColor(2);
            }
        }

        public void AddLightmapEntry()
        {
            BTAB.Entry newEntry = new BTAB.Entry();
            _currentLightmapData.Entries.Add(newEntry);
        }

        public void DeleteMarkedLightmapEntries()
        {
            if (_currentLightmapData.Entries.Count > 0 && _lightmapEntriesForDeletion.Count > 0)
            {
                for (int i = _currentLightmapData.Entries.Count - 1; i > -1; i--)
                {
                    for (int k = _lightmapEntriesForDeletion.Count - 1; k > -1; k--)
                    {
                        if (_currentLightmapData.Entries[i] == _lightmapEntriesForDeletion[k])
                        {
                            _currentLightmapData.Entries.Remove(_currentLightmapData.Entries[i]);
                        }
                    }
                }
            }
        }

        public void LoadLightmapFile()
        {
            try
            {
                _currentLightmapData = BTAB.Read($"{_selectedLightmapFile.AssetPath}");
            }
            catch (Exception e) when (e is DirectoryNotFoundException or FileNotFoundException)
            {
            }
        }

        public void SaveLightmapFile(AssetDescription currentAssetDescription, BTAB modifiedFile)
        {
            // If file was loaded from game root, make new copy in mod root
            if(currentAssetDescription.AssetPath.Contains(_locator.GameRootDirectory))
            {
                currentAssetDescription.AssetPath = currentAssetDescription.AssetPath.Replace(_locator.GameRootDirectory, _locator.GameModDirectory);
            }

            try
            {
                modifiedFile.Write(currentAssetDescription.AssetPath);
                PlatformUtils.Instance.MessageBox($"{_selectedLightmapFile.AssetName} saved.", "Lightmap Menu", MessageBoxButtons.OK);
            }
            catch (Exception e)
            {
            }
        }
    }
}
