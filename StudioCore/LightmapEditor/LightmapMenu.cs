using ImGuiNET;
using SoulsFormats;
using StudioCore.Platform;
using System;
using System.Collections.Generic;
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

        private BTAB currentBTAB;
        private AssetDescription _selectedBtabAsset;

        private List<string> mapList;
        List<AssetDescription> currentMapBtabList;
        private Dictionary<string, AssetDescription> btabMapDict = new Dictionary<string, AssetDescription>();

        private string _searchStr = "";
        private string _searchStrCache = "";

        private List<BTAB.Entry> filteredEntries = new List<BTAB.Entry>();

        public LightmapMenu(AssetLocator locator) 
        {
            _locator = locator;
        }

        public void UpdateBTABList()
        {
            mapList = _locator.GetFullMapList();
            btabMapDict.Clear();

            List<AssetDescription> btabList = new List<AssetDescription>();

            foreach (string mapName in mapList)
            {
                currentMapBtabList = _locator.GetMapBTABs(mapName);

                foreach (AssetDescription d in currentMapBtabList)
                {
                    btabMapDict.Add(d.AssetName, d);
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
                ImGui.BeginChild("LightmapList", new Vector2(700, 100));

                foreach (var m in btabMapDict.Keys)
                {
                    if (ImGui.Selectable(m, _selected == m))
                    {
                        _selected = m;
                    }
                    if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                    {
                        _selectedBtabAsset = btabMapDict[m];
                        currentBTAB = BTAB.Read($"{_selectedBtabAsset.AssetPath}");
                    }
                }

                ImGui.EndChild();

                ImGui.SameLine();
                if (ImGui.Button("Save"))
                {
                    SaveBTAB(_selectedBtabAsset, currentBTAB);
                    PlatformUtils.Instance.MessageBox($"{_selectedBtabAsset.AssetName} saved.", "Lightmap Menu", MessageBoxButtons.OK);
                }

                ImGui.InputText($"Search", ref _searchStr, 255);

                ImGui.Separator();

                if (ImGui.Button("Add New Entry"))
                {
                    BTAB.Entry newEntry = new BTAB.Entry();
                    currentBTAB.Entries.Add(newEntry);
                }

                ImGui.Separator();

                // BTAB Edit Panel
                if (_selectedBtabAsset != null)
                {
                    int index = 0;

                    if (_searchStr != _searchStrCache)
                    {
                        _searchStrCache = _searchStr;

                        filteredEntries.Clear(); // Clear this when the search term changes
                    }

                    List<BTAB.Entry> entriesforDelete = new List<BTAB.Entry>();

                    foreach (BTAB.Entry entry in currentBTAB.Entries)
                    {
                        bool match = false;

                        if (Regex.IsMatch(entry.PartName, _searchStr) || Regex.IsMatch(entry.MaterialName, _searchStr))
                        {
                            match = true;
                        }

                        if (_searchStr == "")
                        {
                            match = true;
                        }

                        if (match)
                        {
                            if(!filteredEntries.Contains(entry))
                                filteredEntries.Add(entry);

                            var PartName = entry.PartName;
                            var MaterialName = entry.MaterialName;
                            var AtlasID = entry.AtlasID;
                            var UVOffset = entry.UVOffset;
                            var UVScale = entry.UVScale;

                            ImGui.Text("Part Name    ");
                            ImGui.SameLine(); ImGui.InputText($"##part_{entry.PartName}{index}", ref PartName, 255);

                            ImGui.SameLine();
                            if (ImGui.Button("Edit"))
                            {
                                
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("Delete Entry"))
                            {
                                entriesforDelete.Add(entry);
                            }

                            ImGui.Text("Material Name");
                            ImGui.SameLine(); ImGui.InputText($"##mat_{entry.MaterialName}{index}", ref MaterialName, 255);

                            ImGui.SameLine();
                            if (ImGui.Button("Edit"))
                            {

                            }

                            ImGui.Text("Atlas ID     ");
                            ImGui.SameLine(); ImGui.InputInt($"##atlas_{entry.AtlasID}{index}", ref AtlasID, 255);

                            ImGui.SameLine();
                            if (ImGui.Button("Edit"))
                            {

                            }

                            ImGui.Text("UV Offset    ");
                            ImGui.SameLine(); ImGui.InputFloat2($"##uv_offset_{entry.UVOffset}{index}", ref UVOffset);

                            ImGui.SameLine();
                            if (ImGui.Button("Edit"))
                            {

                            }

                            ImGui.Text("UV Scale     ");
                            ImGui.SameLine(); ImGui.InputFloat2($"##uv_scale_{entry.UVScale}{index}", ref UVScale);

                            ImGui.SameLine();
                            if (ImGui.Button("Edit"))
                            {

                            }

                            ImGui.Separator();

                            index = index + 1;
                        }
                    }

                    if (currentBTAB.Entries.Count > 0 && entriesforDelete.Count > 0)
                    {
                        for (int i = currentBTAB.Entries.Count - 1; i > -1; i--)
                        {
                            for (int k = entriesforDelete.Count - 1; k > -1; k--)
                            {
                                if (currentBTAB.Entries[i] == entriesforDelete[k])
                                {
                                    currentBTAB.Entries.Remove(currentBTAB.Entries[i]);
                                }
                            }
                        }
                    }
                }

                ImGui.End();

                ImGui.PopStyleVar(3);
                ImGui.PopStyleColor(2);
            }
        }

        public void SaveBTAB(AssetDescription currentAssetDescription, BTAB modifiedFile)
        {
            // If file was loaded from game root, make new copy in mod root
            if(currentAssetDescription.AssetPath.Contains(_locator.GameRootDirectory))
            {
                currentAssetDescription.AssetPath = currentAssetDescription.AssetPath.Replace(_locator.GameRootDirectory, _locator.GameModDirectory);
            }

            modifiedFile.Write(currentAssetDescription.AssetPath);
        }
    }
}
