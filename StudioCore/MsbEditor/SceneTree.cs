using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using Veldrid;
using System.Windows.Forms;
using SoulsFormats;

namespace StudioCore.MsbEditor
{

    public struct DragDropPayload
    {
        public Entity Entity;
    }

    public struct DragDropPayloadReference
    {
        public int Index;
    }

    public interface SceneTreeEventHandler
    {
        public void OnEntityContextMenu(Entity ent);
    }

    public class SceneTree : IActionEventHandler
    {
        private Universe _universe;
        private ActionManager _editorActionManager;
        private Gui.Viewport _viewport;
        private AssetLocator _assetLocator;
        private Selection _selection;

        private string _id;

        private SceneTreeEventHandler _handler;

        private string _chaliceMapID = "m29_";
        private bool _chaliceLoadError = false;

        private bool _GCNeedsCollection = false;

        private Dictionary<string, Dictionary<MapEntity.MapEntityType, Dictionary<Type, List<MapEntity>>>> _cachedTypeView = null;

        private bool _initiatedDragDrop = false;
        private bool _pendingDragDrop = false;
        private Dictionary<int, DragDropPayload> _dragDropPayloads = new Dictionary<int, DragDropPayload>();
        private int _dragDropPayloadCounter = 0;

        private List<Entity> _dragDropSources = new List<Entity>();
        private List<int> _dragDropDests = new List<int>();
        private List<Entity> _dragDropDestObjects = new List<Entity>();

        // Keep track of open tree nodes for selection management purposes
        private HashSet<Entity> _treeOpenEntities = new HashSet<Entity>();

        private Scene.ISelectable _pendingClick = null;

        private bool _setNextFocus = false;

        public enum ViewMode
        {
            Hierarchy,
            Flat,
            ObjectType,
        }

        private string[] _viewModeStrings =
        {
            "Hierarchy View",
            "Flat View",
            "Type View",
        };

        private ViewMode _viewMode = ViewMode.ObjectType;

        public enum Configuration
        {
            MapEditor,
            ModelEditor,
        }

        private Configuration _configuration;

        public SceneTree(Configuration configuration, SceneTreeEventHandler handler, string id, Universe universe, Selection sel, ActionManager aman, Gui.Viewport vp, AssetLocator al)
        {
            _handler = handler;
            _id = id;
            _universe = universe;
            _selection = sel;
            _editorActionManager = aman;
            _viewport = vp;
            _assetLocator = al;
            _configuration = configuration;
            if (_configuration == Configuration.ModelEditor)
            {
                _viewMode = ViewMode.Hierarchy;
            }
        }

        private void RebuildTypeViewCache(Map map)
        {
            if (_cachedTypeView == null)
            {
                _cachedTypeView = new Dictionary<string, Dictionary<MapEntity.MapEntityType, Dictionary<Type, List<MapEntity>>>>();
            }

            var mapcache = new Dictionary<MapEntity.MapEntityType, Dictionary<Type, List<MapEntity>>>();
            mapcache.Add(MapEntity.MapEntityType.Part, new Dictionary<Type, List<MapEntity>>());
            mapcache.Add(MapEntity.MapEntityType.Region, new Dictionary<Type, List<MapEntity>>());
            mapcache.Add(MapEntity.MapEntityType.Event, new Dictionary<Type, List<MapEntity>>());
            if (_assetLocator.Type is GameType.Bloodborne or GameType.DarkSoulsIII or GameType.Sekiro or GameType.EldenRing)
            {
                mapcache.Add(MapEntity.MapEntityType.Light, new Dictionary<Type, List<MapEntity>>());
            }
            else if (_assetLocator.Type is GameType.DarkSoulsIISOTFS)
            {
                mapcache.Add(MapEntity.MapEntityType.Light, new Dictionary<Type, List<MapEntity>>());
                mapcache.Add(MapEntity.MapEntityType.DS2Event, new Dictionary<Type, List<MapEntity>>());
                mapcache.Add(MapEntity.MapEntityType.DS2EventLocation, new Dictionary<Type, List<MapEntity>>());
                mapcache.Add(MapEntity.MapEntityType.DS2Generator, new Dictionary<Type, List<MapEntity>>());
                mapcache.Add(MapEntity.MapEntityType.DS2GeneratorRegist, new Dictionary<Type, List<MapEntity>>());
            }

            foreach (var obj in map.Objects)
            {
                if (obj is MapEntity e && mapcache.ContainsKey(e.Type))
                {
                    var typ = e.WrappedObject.GetType();
                    if (!mapcache[e.Type].ContainsKey(typ))
                    {
                        mapcache[e.Type].Add(typ, new List<MapEntity>());
                    }
                    mapcache[e.Type][typ].Add(e);
                }
            }

            if (!_cachedTypeView.ContainsKey(map.Name))
            {
                _cachedTypeView.Add(map.Name, mapcache);
            }
            else
            {
                _cachedTypeView[map.Name] = mapcache;
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
                    if (!_universe.LoadMap(_chaliceMapID))
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

        private ulong _mapEnt_ImGuiID = 0; // Needed to avoid issue with identical IDs during keyboard navigation. May be unecessary when ImGUI is updated.
        unsafe private void MapObjectSelectable(Entity e, bool visicon, bool hierarchial=false)
        {
            float scale = ImGuiRenderer.GetUIScale();
            
            // Main selectable
            if (e is MapEntity me)
            {
                ImGui.PushID(me.Type.ToString() + e.Name);
            }
            else
            {
                ImGui.PushID(e.Name);
            }
            bool doSelect = false;
            if (_setNextFocus)
            {
                ImGui.SetItemDefaultFocus();
                _setNextFocus = false;
                doSelect = true;
            }
            bool nodeopen = false;
            string padding = hierarchial ? "   " : "    ";
            if (hierarchial && e.Children.Count > 0)
            {
                var treeflags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;
                if ( _selection.GetSelection().Contains(e))
                {
                    treeflags |= ImGuiTreeNodeFlags.Selected;
                }
                nodeopen = ImGui.TreeNodeEx(e.PrettyName, treeflags);
                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(0))
                {
                    if (e.RenderSceneMesh != null)
                    {
                        _viewport.FrameBox(e.RenderSceneMesh.GetBounds());
                    }
                }
            }
            else
            {
                _mapEnt_ImGuiID++;
                if (ImGui.Selectable(padding + e.PrettyName+"##"+ _mapEnt_ImGuiID, _selection.GetSelection().Contains(e), ImGuiSelectableFlags.AllowDoubleClick | ImGuiSelectableFlags.AllowItemOverlap))
                {
                    // If double clicked frame the selection in the viewport
                    if (ImGui.IsMouseDoubleClicked(0))
                    {
                        if (e.RenderSceneMesh != null)
                        {
                            _viewport.FrameBox(e.RenderSceneMesh.GetBounds());
                        }
                    }
                }
            }
            if (ImGui.IsItemClicked(0))
            {
                _pendingClick = e;
            }

            if (_pendingClick == e && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                if (ImGui.IsItemHovered())
                {
                    doSelect = true;
                }
                _pendingClick = null;
            }

            // Up/Down arrow mass selection
            bool arrowKeySelect = false;
            if (ImGui.IsItemFocused()
                && (InputTracker.GetKey(Key.Up) || InputTracker.GetKey(Key.Down)))
            {
                doSelect = true;
                arrowKeySelect = true;
            }

            if (hierarchial && doSelect)
            {
                if ((nodeopen && !_treeOpenEntities.Contains(e)) ||
                    (!nodeopen && _treeOpenEntities.Contains(e)))
                {
                    doSelect = false;
                }

                if (nodeopen && !_treeOpenEntities.Contains(e))
                {
                    _treeOpenEntities.Add(e);
                }
                else if (!nodeopen && _treeOpenEntities.Contains(e))
                {
                    _treeOpenEntities.Remove(e);
                }
            }

            if (_selection.ShouldGoto(e))
            {
                // By default, this places the item at 50% in the frame. Use 0 to place it on top.
                ImGui.SetScrollHereY();
                _selection.ClearGotoTarget();
            }

            if (ImGui.BeginPopupContextItem())
            {
                _handler.OnEntityContextMenu(e);
                ImGui.EndPopup();
            }

            if (ImGui.BeginDragDropSource())
            {
                ImGui.Text(e.PrettyName);
                // Kinda meme
                DragDropPayload p = new DragDropPayload();
                p.Entity = e;
                _dragDropPayloads.Add(_dragDropPayloadCounter, p);
                DragDropPayloadReference r = new DragDropPayloadReference();
                r.Index = _dragDropPayloadCounter;
                _dragDropPayloadCounter++;
                GCHandle handle = GCHandle.Alloc(r, GCHandleType.Pinned);
                ImGui.SetDragDropPayload("entity", handle.AddrOfPinnedObject(), (uint)sizeof(DragDropPayloadReference));
                ImGui.EndDragDropSource();
                handle.Free();
                _initiatedDragDrop = true;
            }
            if (hierarchial && ImGui.BeginDragDropTarget())
            {
                var payload = ImGui.AcceptDragDropPayload("entity");
                if (payload.NativePtr != null)
                {
                    DragDropPayloadReference* h = (DragDropPayloadReference*)payload.Data;
                    var pload = _dragDropPayloads[h->Index];
                    _dragDropPayloads.Remove(h->Index);
                    _dragDropSources.Add(pload.Entity);
                    _dragDropDestObjects.Add(e);
                    _dragDropDests.Add(e.Children.Count);
                }
                ImGui.EndDragDropTarget();
            }

            // Visibility icon
            if (visicon)
            {
                ImGui.SetItemAllowOverlap();
                bool visible = e.EditorVisible;
                ImGui.SameLine(ImGui.GetContentRegionAvail().X - 18.0f);
                ImGui.PushStyleColor(ImGuiCol.Text, visible ? new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                    : new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
                ImGui.TextWrapped(visible ? ForkAwesome.Eye : ForkAwesome.EyeSlash);
                ImGui.PopStyleColor();
                if (ImGui.IsItemClicked(0))
                {
                    e.EditorVisible = !e.EditorVisible;
                    doSelect = false;
                }
            }

            // If the visibility icon wasn't clicked actually perform the selection
            if (doSelect)
            {
                if (arrowKeySelect)
                {
                    if (InputTracker.GetKey(Key.ControlLeft) || InputTracker.GetKey(Key.ControlRight)
                        || InputTracker.GetKey(Key.ShiftLeft) || InputTracker.GetKey(Key.ShiftRight))
                    {
                        _selection.AddSelection(e);
                    }
                    else
                    {
                        _selection.ClearSelection();
                        _selection.AddSelection(e);
                    }
                }
                else if (InputTracker.GetKey(Key.ControlLeft) || InputTracker.GetKey(Key.ControlRight))
                {
                    // Toggle Selection
                    if (_selection.GetSelection().Contains(e))
                    {
                        _selection.RemoveSelection(e);
                    }
                    else
                    {
                        _selection.AddSelection(e);
                    }
                }
                else if (_selection.GetSelection().Count > 0 
                    && (InputTracker.GetKey(Key.ShiftLeft) || InputTracker.GetKey(Key.ShiftRight)))
                {
                    // Select Range
                    var entList = e.Container.Objects;
                    var i1 = entList.IndexOf((MapEntity)_selection.GetSelection().FirstOrDefault(fe => ((MapEntity)fe).Container == e.Container && fe != e.Container.RootObject));
                    var i2 = entList.IndexOf((MapEntity)e);

                    if (i1 != -1 && i2 != -1)
                    {
                        var iStart = i1;
                        var iEnd = i2;
                        if (i2 < i1)
                        {
                            iStart = i2;
                            iEnd = i1;
                        }
                        for (var i = iStart; i <= iEnd; i++)
                        {
                            _selection.AddSelection(entList[i]);
                        }
                    }
                    else
                    {
                        _selection.AddSelection(e);
                    }
                }
                else
                {
                    // Exclusive Selection
                    _selection.ClearSelection();
                    _selection.AddSelection(e);
                }
            }


            // Invisible item to be a drag drop target between nodes
            if (_pendingDragDrop)
            {
                if (e is MapEntity me2)
                {
                    ImGui.SetItemAllowOverlap();
                    ImGui.InvisibleButton(me2.Type.ToString() + e.Name, new Vector2(-1, 3.0f) * scale);
                }
                else
                {
                    ImGui.SetItemAllowOverlap();
                    ImGui.InvisibleButton(e.Name, new Vector2(-1, 3.0f) * scale);
                }
                if (ImGui.IsItemFocused())
                {
                    _setNextFocus = true;
                }
                if (ImGui.BeginDragDropTarget())
                {
                    var payload = ImGui.AcceptDragDropPayload("entity");
                    if (payload.NativePtr != null) //todo: never passes
                    {
                        DragDropPayloadReference* h = (DragDropPayloadReference*)payload.Data;
                        var pload = _dragDropPayloads[h->Index];
                        _dragDropPayloads.Remove(h->Index);
                        if (hierarchial)
                        {
                            _dragDropSources.Add(pload.Entity);
                            _dragDropDestObjects.Add(e.Parent);
                            _dragDropDests.Add(e.Parent.ChildIndex(e) + 1);
                        }
                        else
                        {
                            _dragDropSources.Add(pload.Entity);
                            _dragDropDests.Add(pload.Entity.Container.Objects.IndexOf(e) + 1);
                        }

                    }
                    ImGui.EndDragDropTarget();
                }
            }

            // If there's children then draw them
            if (nodeopen)
            {
                HierarchyView(e);
                ImGui.TreePop();
            }
            ImGui.PopID();
        }

        private void HierarchyView(Entity entity)
        {
            foreach (var obj in entity.Children)
            {
                if (obj is Entity e)
                {
                    MapObjectSelectable(e, true, true);
                }
            }
        }

        private void FlatView(Map map)
        {
            foreach (var obj in map.Objects)
            {
                if (obj is MapEntity e)
                {
                    MapObjectSelectable(e, true);
                }
            }
        }

        private void TypeView(Map map)
        {
            if (_cachedTypeView == null || !_cachedTypeView.ContainsKey(map.Name))
            {
                RebuildTypeViewCache(map);
            }

            foreach (var cats in _cachedTypeView[map.Name].OrderBy(q => q.Key.ToString()))
            {
                if (cats.Value.Count > 0)
                {
                    if (ImGui.TreeNodeEx(cats.Key.ToString(), ImGuiTreeNodeFlags.OpenOnArrow))
                    {
                        foreach (var typ in cats.Value.OrderBy(q => q.Key.Name))
                        {
                            if (typ.Value.Count > 0)
                            {
                                // Regions don't have multiple types in certain games
                                if (cats.Key == MapEntity.MapEntityType.Region &&
                                    (_assetLocator.Type is GameType.DemonsSouls
                                    or GameType.DarkSoulsPTDE
                                    or GameType.DarkSoulsRemastered
                                    or GameType.Bloodborne))
                                {
                                    foreach (var obj in typ.Value)
                                    {
                                        MapObjectSelectable(obj, true);
                                    }
                                }
                                else if (cats.Key == MapEntity.MapEntityType.Light)
                                {
                                    foreach (var parent in map.BTLParents)
                                    {
                                        AssetDescription parentAD = (AssetDescription)parent.WrappedObject;
                                        if (ImGui.TreeNodeEx($"{typ.Key.Name} {parentAD.AssetName}", ImGuiTreeNodeFlags.OpenOnArrow))
                                        {
                                            ImGui.SetItemAllowOverlap();
                                            bool visible = parent.EditorVisible;
                                            ImGui.SameLine(ImGui.GetContentRegionAvail().X - 18.0f);
                                            ImGui.PushStyleColor(ImGuiCol.Text, visible ? new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                                                : new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
                                            ImGui.TextWrapped(visible ? ForkAwesome.Eye : ForkAwesome.EyeSlash);
                                            ImGui.PopStyleColor();
                                            if (ImGui.IsItemClicked(0))
                                            {
                                                // Hide/Unhide all lights within this BTL.
                                                parent.EditorVisible = !parent.EditorVisible;
                                            }
                                            foreach (var obj in parent.Children)
                                            {
                                                MapObjectSelectable(obj, true);
                                            }
                                            ImGui.TreePop();
                                        }
                                        else
                                        {
                                            ImGui.SetItemAllowOverlap();
                                            bool visible = parent.EditorVisible;
                                            ImGui.SameLine(ImGui.GetContentRegionAvail().X - 39.0f);
                                            ImGui.PushStyleColor(ImGuiCol.Text, visible ? new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                                                : new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
                                            ImGui.TextWrapped(visible ? ForkAwesome.Eye : ForkAwesome.EyeSlash);
                                            ImGui.PopStyleColor();
                                            if (ImGui.IsItemClicked(0))
                                            {
                                                // Hide/Unhide all lights within this BTL.
                                                parent.EditorVisible = !parent.EditorVisible;
                                            }
                                        }
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

        private string  _mapNameSearchStr = "";

        public void OnGui()
        {
            float scale = ImGuiRenderer.GetUIScale();

            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.145f, 0.145f, 0.149f, 1.0f));
            if (_configuration == Configuration.MapEditor)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            }
            else
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 2.0f) * scale);
            }
            string titleString = _configuration == Configuration.MapEditor ? $@"Map Object List##{_id}" : $@"Model Hierarchy##{_id}";
            if (ImGui.Begin(titleString))
            {
                if (_initiatedDragDrop)
                {
                    _initiatedDragDrop = false;
                    _pendingDragDrop = true;
                }
                if (_pendingDragDrop && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    _pendingDragDrop = false;
                }

                ImGui.PopStyleVar();
                if (_configuration == Configuration.MapEditor)
                {
                    ImGui.Spacing();
                    ImGui.Indent(30 * scale);
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("List Sorting Style:");
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1);
                    
                    int mode = (int)_viewMode;
                    if (ImGui.Combo("##typecombo", ref mode, _viewModeStrings, _viewModeStrings.Length))
                    {
                        _viewMode = (ViewMode)mode;
                    }
                    
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Map ID Search:");
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1);
                    ImGui.InputText("##treeSearch", ref _mapNameSearchStr, 99);

                    ImGui.Unindent(30 * scale);
                }

                ImGui.BeginChild("listtree");
                Map pendingUnload = null;
                if (_configuration == Configuration.MapEditor && _universe.LoadedObjectContainers.Count == 0)
                    ImGui.Text("This Editor requires game to be unpacked");

                var orderedMaps = _universe.LoadedObjectContainers.OrderBy(k => k.Key);

                _mapEnt_ImGuiID = 0;
                foreach (var lm in orderedMaps)
                {
                    string metaName = "";
                    var map = lm.Value;
                    var mapid = lm.Key;
                    if (mapid == null)
                        continue;

                    if (Editor.AliasBank.MapNames != null && Editor.AliasBank.MapNames.ContainsKey(mapid))
                    {
                        metaName = Editor.AliasBank.MapNames[mapid];
                    }

                    // Map name search filter
                    if (_mapNameSearchStr != ""
                        && (!CFG.Current.Map_AlwaysListLoadedMaps || map == null)
                        && !lm.Key.Contains(_mapNameSearchStr, StringComparison.CurrentCultureIgnoreCase)
                        && !metaName.Contains(_mapNameSearchStr, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }

                    Entity mapRoot = map?.RootObject;
                    ObjectContainerReference mapRef = new ObjectContainerReference(mapid, _universe);
                    Scene.ISelectable selectTarget = (Scene.ISelectable)mapRoot ?? mapRef;

                    var treeflags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;
                    bool selected = _selection.GetSelection().Contains(mapRoot) || _selection.GetSelection().Contains(mapRef);
                    if (selected)
                    {
                        treeflags |= ImGuiTreeNodeFlags.Selected;
                    }
                    bool nodeopen = false;
                    string unsaved = (map != null && map.HasUnsavedChanges) ? "*" : "";
                    ImGui.BeginGroup();
                    if (map != null)
                    {
                        nodeopen = ImGui.TreeNodeEx($@"{ForkAwesome.Cube} {mapid}", treeflags, $@"{ForkAwesome.Cube} {mapid}{unsaved}");
                    }
                    else
                    {
                        ImGui.Selectable($@"   {ForkAwesome.Cube} {mapid}", selected);
                    }
                    if (metaName != "")
                    {
                        ImGui.SameLine();
                        if (metaName.StartsWith("--")) //marked as normally unused (use red text)
                            ImGui.TextColored(new Vector4(1.0f, 0.0f, 0.0f, 1.0f), @$"<{metaName.Replace("--","")}>");
                        else
                            ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), @$"<{metaName}>");
                    }
                    ImGui.EndGroup();
                    if (_selection.ShouldGoto(mapRoot) || _selection.ShouldGoto(mapRef))
                    {
                        ImGui.SetScrollHereY();
                        _selection.ClearGotoTarget();
                    }

                    if (nodeopen)
                        ImGui.Indent(); //TreeNodeEx fails to indent as it is inside a group / indentation is reset
                    // Right click context menu
                    if (ImGui.BeginPopupContextItem($@"mapcontext_{mapid}"))
                    {
                        if (map == null)
                        {
                            if (ImGui.Selectable("Load Map"))
                            {
                                if (selected)
                                {
                                    _selection.ClearSelection();
                                }
                                _universe.LoadMap(mapid, selected);
                            }
                        }
                        else if (map is Map m)
                        {
                            if (ImGui.Selectable("Save Map"))
                            {
                                try
                                {
                                    _universe.SaveMap(m);
                                }
                                catch (SavingFailedException e)
                                {
                                    System.Windows.Forms.MessageBox.Show(e.Wrapped.Message, e.Message,
                                         System.Windows.Forms.MessageBoxButtons.OK,
                                         System.Windows.Forms.MessageBoxIcon.None);
                                }
                            }
                            if (ImGui.Selectable("Unload Map"))
                            {
                                _selection.ClearSelection();
                                _editorActionManager.Clear();
                                pendingUnload = m;
                            }
                        }
                        ImGui.EndPopup();
                    }
                    if (ImGui.IsItemClicked())
                    {
                        _pendingClick = selectTarget;
                    }
                    if (ImGui.IsMouseDoubleClicked(0) && _pendingClick != null && mapRoot == _pendingClick)
                    {
                        _viewport.FramePosition(mapRoot.GetLocalTransform().Position, 10f);
                    }
                    if ((_pendingClick == mapRoot || mapRef.Equals(_pendingClick)) && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                    {
                        if (ImGui.IsItemHovered())
                        {
                            // Only select if a node is not currently being opened/closed
                            if (mapRoot == null ||
                                (nodeopen && _treeOpenEntities.Contains(mapRoot)) ||
                                (!nodeopen && !_treeOpenEntities.Contains(mapRoot)))
                            {
                                if (InputTracker.GetKey(Key.ControlLeft) || InputTracker.GetKey(Key.ControlRight))
                                {
                                    // Toggle Selection
                                    if (_selection.GetSelection().Contains(selectTarget))
                                    {
                                        _selection.RemoveSelection(selectTarget);
                                    }
                                    else
                                    {
                                        _selection.AddSelection(selectTarget);
                                    }
                                }
                                else
                                {
                                    _selection.ClearSelection();
                                    _selection.AddSelection(selectTarget);
                                }
                            }

                            // Update the open/closed state
                            if (mapRoot != null)
                            {
                                if (nodeopen && !_treeOpenEntities.Contains(mapRoot))
                                {
                                    _treeOpenEntities.Add(mapRoot);
                                }
                                else if (!nodeopen && _treeOpenEntities.Contains(mapRoot))
                                {
                                    _treeOpenEntities.Remove(mapRoot);
                                }
                            }
                        }
                        _pendingClick = null;
                    }
                    if (nodeopen)
                    {
                        if (_pendingDragDrop)
                        {
                            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8.0f, 0.0f) * scale);
                        }
                        else
                        {
                            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8.0f, 3.0f) * scale);
                        }
                        if (_viewMode == ViewMode.Hierarchy)
                        {
                            HierarchyView(map.RootObject);
                        }
                        else if (_viewMode == ViewMode.Flat)
                        {
                            FlatView((Map)map);
                        }
                        else if (_viewMode == ViewMode.ObjectType)
                        {
                            TypeView((Map)map);
                        }
                        ImGui.PopStyleVar();
                        ImGui.TreePop();
                    }

                    // Update type cache when a map is no longer loaded
                    if (_cachedTypeView != null && map == null && _cachedTypeView.ContainsKey(mapid))
                    {
                        _cachedTypeView.Remove(mapid);
                    }
                }
                if (_assetLocator.Type == GameType.Bloodborne && _configuration == Configuration.MapEditor)
                {
                    ChaliceDungeonImportButton();
                }
                ImGui.EndChild();

                if (_dragDropSources.Count > 0)
                {
                    if (_dragDropDestObjects.Count > 0)
                    {
                        var action = new ChangeEntityHierarchyAction(_universe, _dragDropSources, _dragDropDestObjects, _dragDropDests, false);
                        _editorActionManager.ExecuteAction(action);
                        _dragDropSources.Clear();
                        _dragDropDests.Clear();
                        _dragDropDestObjects.Clear();
                    }
                    else
                    {
                        var action = new ReorderContainerObjectsAction(_universe, _dragDropSources, _dragDropDests, false);
                        _editorActionManager.ExecuteAction(action);
                        _dragDropSources.Clear();
                        _dragDropDests.Clear();
                    }
                }

                if (pendingUnload != null)
                {
                    _universe.UnloadMap(pendingUnload);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    _GCNeedsCollection = true;
                }
            }
            else
            {
                ImGui.PopStyleVar();
            }
            ImGui.End();
            ImGui.PopStyleColor();
            _selection.ClearGotoTarget();
        }

        public void OnActionEvent(ActionEvent evt)
        {
            if (evt.HasFlag(ActionEvent.ObjectAddedRemoved))
            {
                _cachedTypeView = null;
            }
        }
    }
}
