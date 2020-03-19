using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Veldrid;

namespace StudioCore.MsbEditor
{
    public class SceneTree
    {
        private Universe Universe;
        private ActionManager EditorActionManager;
        private Gui.Viewport Viewport;
        private AssetLocator AssetLocator;
        private Resource.ResourceManager ResourceMan;

        private string _chaliceMapID = "m29_";
        private bool _chaliceLoadError = false;

        private bool _GCNeedsCollection = false;

        private Dictionary<string, Dictionary<MapObject.ObjectType, Dictionary<Type, List<MapObject>>>> _cachedTypeView = null;

        public enum ViewMode
        {
            Hierarchy,
            ObjectType,
        }

        private string[] _viewModeStrings =
        {
            "Hierarchy View",
            "Type View",
        };

        private ViewMode _viewMode = ViewMode.Hierarchy;

        public SceneTree(Universe universe, ActionManager aman, Gui.Viewport vp, AssetLocator al, Resource.ResourceManager rm)
        {
            Universe = universe;
            EditorActionManager = aman;
            Viewport = vp;
            AssetLocator = al;
            ResourceMan = rm;
        }

        private void RebuildTypeViewCache(Map map)
        {
            if (_cachedTypeView == null)
            {
                _cachedTypeView = new Dictionary<string, Dictionary<MapObject.ObjectType, Dictionary<Type, List<MapObject>>>>();
            }

            var mapcache = new Dictionary<MapObject.ObjectType, Dictionary<Type, List<MapObject>>>();
            mapcache.Add(MapObject.ObjectType.Part, new Dictionary<Type, List<MapObject>>());
            mapcache.Add(MapObject.ObjectType.Region, new Dictionary<Type, List<MapObject>>());
            mapcache.Add(MapObject.ObjectType.Event, new Dictionary<Type, List<MapObject>>());
            if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                mapcache.Add(MapObject.ObjectType.DS2Event, new Dictionary<Type, List<MapObject>>());
                mapcache.Add(MapObject.ObjectType.DS2EventLocation, new Dictionary<Type, List<MapObject>>());
                mapcache.Add(MapObject.ObjectType.DS2Generator, new Dictionary<Type, List<MapObject>>());
                mapcache.Add(MapObject.ObjectType.DS2GeneratorRegist, new Dictionary<Type, List<MapObject>>());
            }

            foreach (var obj in map.MapObjects)
            {
                if (mapcache.ContainsKey(obj.Type))
                {
                    var typ = obj.MsbObject.GetType();
                    if (!mapcache[obj.Type].ContainsKey(typ))
                    {
                        mapcache[obj.Type].Add(typ, new List<MapObject>());
                    }
                    mapcache[obj.Type][typ].Add(obj);
                }
            }

            if (!_cachedTypeView.ContainsKey(map.MapId))
            {
                _cachedTypeView.Add(map.MapId, mapcache);
            }
            else
            {
                _cachedTypeView[map.MapId] = mapcache;
            }
        }

        private void ChaliceDungeonImportButton()
        {
            ImGui.Selectable($@"   {ForkAwesome.PlusCircle} Load Chalice Dungeon...", false);
            if (ImGui.BeginPopupContextItem("chalice", 0))
            {
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Chalice ID (m29_xx_xx_xx): ");
                ImGui.SameLine();
                var pname = _chaliceMapID;
                ImGui.SetNextItemWidth(100);
                if (_chaliceLoadError)
                {
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.8f, 0.2f, 0.2f, 1.0f));
                }
                if (ImGui.InputText("##chalicename", ref pname, 12))
                {
                    _chaliceMapID = pname;
                }
                if (_chaliceLoadError)
                {
                    ImGui.PopStyleColor();
                }
                ImGui.SameLine();
                if (ImGui.Button("Load"))
                {
                    if (!Universe.LoadMap(_chaliceMapID))
                    {
                        _chaliceLoadError = true;
                    }
                    else
                    {
                        ImGui.CloseCurrentPopup();
                        _chaliceLoadError = false;
                        _chaliceMapID = "m29_";
                    }
                }
                ImGui.EndPopup();
            }
        }

        private void MapObjectSelectable(MapObject obj, bool visicon)
        {
            // Main selectable
            ImGui.PushID(obj.Type.ToString() + obj.Name);
            bool doSelect = false;
            if (ImGui.Selectable(obj.PrettyName, Selection.GetSelection().Contains(obj), ImGuiSelectableFlags.AllowDoubleClick | ImGuiSelectableFlags.AllowItemOverlap))
            {
                // If double clicked frame the selection in the viewport
                if (ImGui.IsMouseDoubleClicked(0))
                {
                    if (obj.RenderSceneMesh != null)
                    {
                        Viewport.FrameBox(obj.RenderSceneMesh.GetBounds());
                    }
                }
            }
            if (ImGui.IsItemClicked(0))
            {
                doSelect = true;
            }

            if (ImGui.IsItemFocused() && !Selection.IsSelected(obj))
            {
                doSelect = true;
            }

            // Visibility icon
            if (visicon)
            {
                bool visible = obj.EditorVisible;
                ImGui.SameLine(ImGui.GetWindowContentRegionWidth() - 18.0f);
                ImGui.PushStyleColor(ImGuiCol.Text, visible ? new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                    : new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
                ImGui.TextWrapped(visible ? ForkAwesome.Eye : ForkAwesome.EyeSlash);
                ImGui.PopStyleColor();
                if (ImGui.IsItemClicked(0))
                {
                    obj.EditorVisible = !obj.EditorVisible;
                    doSelect = false;
                }
            }

            // If the visibility icon wasn't clicked actually perform the selection
            if (doSelect)
            {
                if (InputTracker.GetKey(Key.ControlLeft) || InputTracker.GetKey(Key.ControlRight))
                {
                    Selection.AddSelection(obj);
                }
                else
                {
                    Selection.ClearSelection();
                    Selection.AddSelection(obj);
                }
            }

            ImGui.PopID();
        }

        private void HierarchyView(Map map)
        {
            foreach (var obj in map.MapObjects)
            {
                MapObjectSelectable(obj, true);
            }
        }

        private void TypeView(Map map)
        {
            if (_cachedTypeView == null || !_cachedTypeView.ContainsKey(map.MapId))
            {
                RebuildTypeViewCache(map);
            }

            foreach (var cats in _cachedTypeView[map.MapId].OrderBy(q => q.Key.ToString()))
            {
                if (cats.Value.Count > 0)
                {
                    if (ImGui.TreeNodeEx(cats.Key.ToString(), ImGuiTreeNodeFlags.OpenOnArrow))
                    {
                        foreach (var typ in cats.Value.OrderBy(q => q.Key.Name))
                        {
                            if (typ.Value.Count > 0)
                            {
                                // Regions don't have multiple types in games before DS3
                                if (cats.Key == MapObject.ObjectType.Region &&
                                    AssetLocator.Type != GameType.DarkSoulsIII && AssetLocator.Type != GameType.Sekiro)
                                {
                                    foreach (var obj in typ.Value)
                                    {
                                        MapObjectSelectable(obj, true);
                                    }
                                }
                                else if (ImGui.TreeNodeEx(typ.Key.Name, ImGuiTreeNodeFlags.OpenOnArrow))
                                {
                                    foreach (var obj in typ.Value)
                                    {
                                        MapObjectSelectable(obj, true);
                                    }
                                    ImGui.TreePop();
                                }
                            }
                            else
                            {
                                ImGui.Text($@"   {typ.Key.ToString()}");
                            }
                        }
                        ImGui.TreePop();
                    }
                }
                else
                {
                    ImGui.Text($@"   {cats.Key.ToString()}");
                }

            }
        }

        public void OnGui()
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.145f, 0.145f, 0.149f, 1.0f));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            if (ImGui.Begin("Map Object List"))
            {
                ImGui.PopStyleVar();
                int mode = (int)_viewMode;
                ImGui.SetNextItemWidth(-1);
                if (ImGui.Combo("##typecombo", ref mode, _viewModeStrings, _viewModeStrings.Length))
                {
                    _viewMode = (ViewMode)mode;
                }

                ImGui.BeginChild("listtree");
                Map pendingUnload = null;
                foreach (var lm in Universe.LoadedMaps.OrderBy((k) => k.Key))
                {
                    var map = lm.Value;
                    var mapid = lm.Key;
                    var treeflags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;
                    if (map != null && Selection.GetSelection().Contains(map.RootObject))
                    {
                        treeflags |= ImGuiTreeNodeFlags.Selected;
                    }
                    bool nodeopen = false;
                    if (map != null)
                    {
                        nodeopen = ImGui.TreeNodeEx($@"{ForkAwesome.Cube} {mapid}", treeflags);
                    }
                    else
                    {
                        ImGui.Selectable($@"   {ForkAwesome.Cube} {mapid}", false);
                    }
                    // Right click context menu
                    if (ImGui.BeginPopupContextItem($@"mapcontext_{mapid}"))
                    {
                        if (map == null)
                        {
                            if (ImGui.Selectable("Load Map"))
                            {
                                Universe.LoadMap(mapid);
                            }
                        }
                        else
                        {
                            if (ImGui.Selectable("Save Map"))
                            {
                                Universe.SaveMap(map);
                            }
                            if (ImGui.Selectable("Unload Map"))
                            {
                                Selection.ClearSelection();
                                EditorActionManager.Clear();
                                pendingUnload = map;
                            }
                        }
                        ImGui.EndPopup();
                    }
                    if (ImGui.IsItemClicked() && map != null)
                    {
                        if (InputTracker.GetKey(Key.ShiftLeft) || InputTracker.GetKey(Key.ShiftRight))
                        {
                            Selection.AddSelection(map.RootObject);
                        }
                        else
                        {
                            Selection.ClearSelection();
                            Selection.AddSelection(map.RootObject);
                        }
                    }
                    if (nodeopen)
                    {
                        if (_viewMode == ViewMode.Hierarchy)
                        {
                            HierarchyView(map);
                        }
                        else if (_viewMode == ViewMode.ObjectType)
                        {
                            TypeView(map);
                        }
                        ImGui.TreePop();
                    }
                }
                if (AssetLocator.Type == GameType.Bloodborne)
                {
                    ChaliceDungeonImportButton();
                }
                ImGui.EndChild();
                ImGui.End();

                if (pendingUnload != null)
                {
                    Universe.UnloadMap(pendingUnload);
                    GC.Collect();
                    _GCNeedsCollection = true;
                    ResourceMan.UnloadUnusedResources();
                    GC.Collect();
                }
            }
            else
            {
                ImGui.PopStyleVar();
            }
            ImGui.PopStyleColor();
        }
    }
}
