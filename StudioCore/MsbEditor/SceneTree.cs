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
                        HierarchyView(map);
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
