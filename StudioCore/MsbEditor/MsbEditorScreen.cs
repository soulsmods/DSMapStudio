using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Reflection;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using StudioCore.Resource;
using Newtonsoft.Json;
using ImGuiNET;
using StudioCore.Scene;
using StudioCore.Editor;

namespace StudioCore.MsbEditor
{
    public class MsbEditorScreen : EditorScreen, SceneTreeEventHandler
    {
        public AssetLocator AssetLocator = null;
        public Scene.RenderScene RenderScene = new Scene.RenderScene();
        private Selection _selection = new Selection();
        public ActionManager EditorActionManager = new ActionManager();
        private Editor.ProjectSettings _projectSettings = null;

        private List<(string, Type)> _partsClasses = new List<(string, Type)>();
        private List<(string, Type)> _regionClasses = new List<(string, Type)>();
        private List<(string, Type)> _eventClasses = new List<(string, Type)>();

        public SceneTree SceneTree;
        public PropertyEditor PropEditor;
        public SearchProperties PropSearch;
        public DisplayGroupsEditor DispGroupEditor;
        public NavmeshEditor NavMeshEditor;

        public Universe Universe;

        private bool GCNeedsCollection = false;

        public Rectangle ModelViewerBounds;

        private const int RECENT_FILES_MAX = 32;

        public bool CtrlHeld;
        public bool ShiftHeld;
        public bool AltHeld;

        private static object _lock_PauseUpdate = new object();
        private bool _PauseUpdate;
        private bool PauseUpdate
        {
            get
            {
                lock (_lock_PauseUpdate)
                    return _PauseUpdate;
            }
            set
            {
                lock (_lock_PauseUpdate)
                    _PauseUpdate = value;
            }
        }

        public Rectangle Rect;

        private Sdl2Window Window;
        public Gui.Viewport Viewport;

        private IModal _activeModal = null;

        public MsbEditorScreen(Sdl2Window window, GraphicsDevice device, AssetLocator locator)
        {
            Rect = window.Bounds;
            AssetLocator = locator;
            ResourceManager.Locator = AssetLocator;
            Window = window;

            Viewport = new Gui.Viewport("Mapeditvp", device, RenderScene, EditorActionManager, _selection, Rect.Width, Rect.Height);
            Universe = new Universe(AssetLocator, RenderScene, _selection);

            SceneTree = new SceneTree(SceneTree.Configuration.MapEditor, this, "mapedittree", Universe, _selection, EditorActionManager, Viewport, AssetLocator);
            PropEditor = new PropertyEditor(EditorActionManager);
            DispGroupEditor = new DisplayGroupsEditor(RenderScene, _selection);
            PropSearch = new SearchProperties(Universe);
            NavMeshEditor = new NavmeshEditor(locator, RenderScene, _selection);

            EditorActionManager.AddEventHandler(SceneTree);

            RenderScene.DrawFilter = CFG.Current.LastSceneFilter;
        }

        private bool ViewportUsingKeyboard = false;

        public void Update(float dt)
        {

            if (GCNeedsCollection)
            {
                GC.Collect();
                GCNeedsCollection = false;
            }

            if (PauseUpdate)
            {
                return;
            }

            ViewportUsingKeyboard = Viewport.Update(Window, dt);
        }

        public void EditorResized(Sdl2Window window, GraphicsDevice device)
        {
            Window = window;
            Rect = window.Bounds;
            //Viewport.ResizeViewport(device, new Rectangle(0, 0, window.Width, window.Height));
        }

        public void FrameSelection()
        {
            var selected = _selection.GetFilteredSelection<Entity>();
            bool first = false;
            BoundingBox box = new BoundingBox();
            foreach (var s in selected)
            {
                if (s.RenderSceneMesh != null)
                {
                    if (!first)
                    {
                        box = s.RenderSceneMesh.GetBounds();
                        first = true;
                    }
                    else
                    {
                        box = BoundingBox.Combine(box, s.RenderSceneMesh.GetBounds());
                    }
                }
            }
            if (first)
            {
                Viewport.FrameBox(box);
            }
        }

        /// <summary>
        /// Hides all the selected objects, unless all of them are hidden in which
        /// they will be unhidden
        /// </summary>
        public void HideShowSelection()
        {
            var selected = _selection.GetFilteredSelection<Entity>();
            bool allhidden = true;
            foreach (var s in selected)
            {
                if (s.EditorVisible)
                {
                    allhidden = false;
                }
            }
            foreach (var s in selected)
            {
                s.EditorVisible = allhidden;
            }
        }

        private void AddNewEntity(Type typ, MapEntity.MapEntityType etype)
        {
            var newent = typ.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

            var map = Universe.LoadedObjectContainers.Values.First((x) => x != null); //TODO: if selection is not null, target map holding selection. first loaded map otherwise.
            var obj = new MapEntity(map, newent, etype);

            var act = new AddMapObjectsAction(Universe, (Map)map, RenderScene, new List<MapEntity> { obj }, true);
            EditorActionManager.ExecuteAction(act);
        }

        private void DummyUndummySelection(string sourceStr, string targetStr)
        {
            //Incomplete.
            //todo2: implement into new action via EditorActionManager.
            Type msbclass;
            switch (AssetLocator.Type)
            {
                case GameType.DemonsSouls:
                    msbclass = typeof(MSBD);
                    break;
                case GameType.DarkSoulsPTDE:
                case GameType.DarkSoulsRemastered:
                    msbclass = typeof(MSB1);
                    break;
                case GameType.DarkSoulsIISOTFS:
                    msbclass = typeof(MSB2);
                    break;
                case GameType.DarkSoulsIII:
                    msbclass = typeof(MSB3);
                    break;
                case GameType.Bloodborne:
                    msbclass = typeof(MSBB);
                    break;
                case GameType.Sekiro:
                    msbclass = typeof(MSBS);
                    break;
                case GameType.EldenRing:
                    msbclass = typeof(MSBE);
                    break;
                default:
                    throw new ArgumentException("type must be valid");
            }

            var sourceList = _selection.GetFilteredSelection<MapEntity>().ToList();

            var targetType = msbclass.GetNestedType("Part").GetNestedType(targetStr);
            var sourceType = msbclass.GetNestedType("Part").GetNestedType(sourceStr);

            for (var i=0;i<sourceList.Count;i++)
            {
                var ent = sourceList[i];

                if (ent.Type == MapEntity.MapEntityType.Part)
                {
                    var currentType = ent.WrappedObject.GetType();
                    if (currentType == sourceType)
                    {
                        var source = ent.WrappedObject;
                        var target = targetType.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());

                        foreach (PropertyInfo property in sourceType.GetProperties().Where(p => p.CanWrite))
                        {
                            property.SetValue(target, property.GetValue(source, null), null);
                        }
                        ent.WrappedObject = target;
                    }
                }
            }

            /*
            var action = new CloneMapObjectsAction(Universe, RenderScene, _selection.GetFilteredSelection<MapEntity>().ToList(), true);
            EditorActionManager.ExecuteAction(action);
            var action = new DeleteMapObjectsAction(Universe, RenderScene, _selection.GetFilteredSelection<MapEntity>().ToList(), true);
            EditorActionManager.ExecuteAction(action);
                //make them use ActionEvent.ObjectAddedRemoved to refresh msb map list cache
            */

        }

        public override void DrawEditorMenu()
        {
            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Undo", "CTRL+Z", false, EditorActionManager.CanUndo()))
                {
                    EditorActionManager.UndoAction();
                }
                if (ImGui.MenuItem("Redo", "Ctrl+Y", false, EditorActionManager.CanRedo()))
                {
                    EditorActionManager.RedoAction();
                }
                if (ImGui.MenuItem("Delete", "Delete", false, _selection.IsSelection()))
                {
                    var action = new DeleteMapObjectsAction(Universe, RenderScene, _selection.GetFilteredSelection<MapEntity>().ToList(), true);
                    EditorActionManager.ExecuteAction(action);
                }
                if (ImGui.MenuItem("Duplicate", "Ctrl+D", false, _selection.IsSelection()))
                {
                    var action = new CloneMapObjectsAction(Universe, RenderScene, _selection.GetFilteredSelection<MapEntity>().ToList(), true);
                    EditorActionManager.ExecuteAction(action);
                }
                if (ImGui.MenuItem("Hide/Show", "Ctrl+H", false, _selection.IsSelection()))
                {
                    HideShowSelection();
                }
                if (ImGui.MenuItem("Frame", "Ctrl-F", false, _selection.IsSelection()))
                {
                    FrameSelection();
                }

                /*
                //todo2: Finish. Add (difficult to press by accident) hotkeys for enemies.
                if (ImGui.BeginMenu("Dummify/Undummify Selection"))
                {
                    if (ImGui.MenuItem("Selected Enemies -> DummyEnemies", "", false, _selection.IsSelection()))
                    {
                        DummyUndummySelection("Enemy", "DummyEnemy");
                    }
                    if (ImGui.MenuItem("Selected DummyEnemies -> Enemies", "", false, _selection.IsSelection()))
                    {   
                        DummyUndummySelection("DummyEnemy", "Enemy");
                    }
                    if (AssetLocator.Type != GameType.EldenRing)
                    {
                        if (ImGui.MenuItem("Selected Objects -> DummyObjects", "", false, _selection.IsSelection()))
                        {
                            DummyUndummySelection("", "");
                        }
                        if (ImGui.MenuItem("Selected DummyObjects -> Objects", "", false, _selection.IsSelection()))
                        {   
                            DummyUndummySelection("", "");
                        }
                    }
                    else
                    {
                        if (ImGui.MenuItem("Assets", "", false, _selection.IsSelection()))
                        {
                            DummyUndummySelection("", "");
                        }
                        if (ImGui.MenuItem("Assets", "", false, _selection.IsSelection()))
                        {   
                            DummyUndummySelection("", "");
                        }
                    }
                    ImGui.EndMenu();
                }
                */

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Create"))
            {
                if (ImGui.BeginMenu("Parts"))
                {
                    foreach (var p in _partsClasses)
                    {
                        if (ImGui.MenuItem(p.Item1))
                        {
                            AddNewEntity(p.Item2, MapEntity.MapEntityType.Part);
                        }
                    }
                    ImGui.EndMenu();
                }
                // Some games only have one single region class
                if (_regionClasses.Count == 1)
                {
                    if (ImGui.MenuItem("Region"))
                    {
                        AddNewEntity(_regionClasses[0].Item2, MapEntity.MapEntityType.Region);
                    }
                }
                else
                {
                    if (ImGui.BeginMenu("Regions"))
                    {
                        foreach (var p in _regionClasses)
                        {
                            if (ImGui.MenuItem(p.Item1))
                            {
                                AddNewEntity(p.Item2, MapEntity.MapEntityType.Region);
                            }
                        }
                        ImGui.EndMenu();
                    }
                }
                if (ImGui.BeginMenu("Events"))
                {
                    foreach (var p in _eventClasses)
                    {
                        if (ImGui.MenuItem(p.Item1))
                        {
                            AddNewEntity(p.Item2, MapEntity.MapEntityType.Event);
                        }
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Display"))
            {
                if (ImGui.MenuItem("Grid", "", Viewport.DrawGrid))
                {
                    Viewport.DrawGrid = !Viewport.DrawGrid;
                }
                if (ImGui.BeginMenu("Object Types"))
                {
                    if (ImGui.MenuItem("Debug", "", RenderScene.DrawFilter.HasFlag(Scene.RenderFilter.Debug)))
                    {
                        RenderScene.ToggleDrawFilter(Scene.RenderFilter.Debug);
                    }
                    if (ImGui.MenuItem("Map Piece", "", RenderScene.DrawFilter.HasFlag(Scene.RenderFilter.MapPiece)))
                    {
                        RenderScene.ToggleDrawFilter(Scene.RenderFilter.MapPiece);
                    }
                    if (ImGui.MenuItem("Collision", "", RenderScene.DrawFilter.HasFlag(Scene.RenderFilter.Collision)))
                    {
                        RenderScene.ToggleDrawFilter(Scene.RenderFilter.Collision);
                    }
                    if (ImGui.MenuItem("Object", "", RenderScene.DrawFilter.HasFlag(Scene.RenderFilter.Object)))
                    {
                        RenderScene.ToggleDrawFilter(Scene.RenderFilter.Object);
                    }
                    if (ImGui.MenuItem("Character", "", RenderScene.DrawFilter.HasFlag(Scene.RenderFilter.Character)))
                    {
                        RenderScene.ToggleDrawFilter(Scene.RenderFilter.Character);
                    }
                    if (ImGui.MenuItem("Navmesh", "", RenderScene.DrawFilter.HasFlag(Scene.RenderFilter.Navmesh)))
                    {
                        RenderScene.ToggleDrawFilter(Scene.RenderFilter.Navmesh);
                    }
                    if (ImGui.MenuItem("Region", "", RenderScene.DrawFilter.HasFlag(Scene.RenderFilter.Region)))
                    {
                        RenderScene.ToggleDrawFilter(Scene.RenderFilter.Region);
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Display Presets"))
                {
                    if (ImGui.MenuItem("Map Piece/Character/Objects", "Ctrl-1"))
                    {
                        RenderScene.DrawFilter = Scene.RenderFilter.MapPiece | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region;
                    }
                    if (ImGui.MenuItem("Collision/Character/Objects", "Ctrl-2"))
                    {
                        RenderScene.DrawFilter = Scene.RenderFilter.Collision | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region;
                    }
                    if (ImGui.MenuItem("Collision/Navmesh/Character/Objects", "Ctrl-3"))
                    {
                        RenderScene.DrawFilter = Scene.RenderFilter.Collision | Scene.RenderFilter.Navmesh | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region;
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Environment Map"))
                {
                    if (ImGui.MenuItem("Default"))
                    {
                        Viewport.SetEnvMap(0);
                    }
                    foreach (var map in Universe.EnvMapTextures)
                    {
                        if (ImGui.MenuItem(map))
                        {
                            var tex = ResourceManager.GetTextureResource($@"tex/{map}".ToLower());
                            if (tex.IsLoaded && tex.Get() != null && tex.TryLock())
                            {
                                if (tex.Get().GPUTexture.Resident)
                                {
                                    Viewport.SetEnvMap(tex.Get().GPUTexture.TexHandle);
                                }
                                tex.Unlock();
                            }
                        }
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Scene Lighting"))
                {
                    Viewport.SceneParamsGui();
                    ImGui.EndMenu();
                }
                CFG.Current.LastSceneFilter = RenderScene.DrawFilter;
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Gizmos"))
            {
                if (ImGui.BeginMenu("Mode"))
                {
                    if (ImGui.MenuItem("Translate", "W", Gizmos.Mode == Gizmos.GizmosMode.Translate))
                    {
                        Gizmos.Mode = Gizmos.GizmosMode.Translate;
                    }
                    if (ImGui.MenuItem("Rotate", "E", Gizmos.Mode == Gizmos.GizmosMode.Rotate))
                    {
                        Gizmos.Mode = Gizmos.GizmosMode.Rotate;
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Space"))
                {
                    if (ImGui.MenuItem("Local", "", Gizmos.Space == Gizmos.GizmosSpace.Local))
                    {
                        Gizmos.Space = Gizmos.GizmosSpace.Local;
                    }
                    if (ImGui.MenuItem("World", "", Gizmos.Space == Gizmos.GizmosSpace.World))
                    {
                        Gizmos.Space = Gizmos.GizmosSpace.World;
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Origin"))
                {
                    if (ImGui.MenuItem("World", "", Gizmos.Origin == Gizmos.GizmosOrigin.World))
                    {
                        Gizmos.Origin = Gizmos.GizmosOrigin.World;
                    }
                    if (ImGui.MenuItem("Bounding Box", "", Gizmos.Origin == Gizmos.GizmosOrigin.BoundingBox))
                    {
                        Gizmos.Origin = Gizmos.GizmosOrigin.BoundingBox;
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }
        }

        public void OnGUI(string[] initcmd)
        {
            // Docking setup
            //var vp = ImGui.GetMainViewport();
            var wins = ImGui.GetWindowSize();
            var winp = ImGui.GetWindowPos();
            winp.Y += 20.0f;
            wins.Y -= 20.0f;
            ImGui.SetNextWindowPos(winp);
            ImGui.SetNextWindowSize(wins);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0.0f);
            ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            flags |= ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
            flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
            flags |= ImGuiWindowFlags.NoBackground;
            //ImGui.Begin("DockSpace_MapEdit", flags);
            ImGui.PopStyleVar(4);
            var dsid = ImGui.GetID("DockSpace_MapEdit");
            ImGui.DockSpace(dsid, new Vector2(0, 0));

            // Keyboard shortcuts
            if (EditorActionManager.CanUndo() && InputTracker.GetControlShortcut(Key.Z))
            {
                EditorActionManager.UndoAction();
            }
            if (EditorActionManager.CanRedo() && InputTracker.GetControlShortcut(Key.Y))
            {
                EditorActionManager.RedoAction();
            }
            if (!ViewportUsingKeyboard && !ImGui.GetIO().WantCaptureKeyboard)
            {
                if (InputTracker.GetControlShortcut(Key.D) && _selection.IsSelection())
                {
                    var action = new CloneMapObjectsAction(Universe, RenderScene, _selection.GetFilteredSelection<MapEntity>().ToList(), true);
                    EditorActionManager.ExecuteAction(action);
                }
                if (InputTracker.GetKeyDown(Key.Delete) && _selection.IsSelection())
                {
                    var action = new DeleteMapObjectsAction(Universe, RenderScene, _selection.GetFilteredSelection<MapEntity>().ToList(), true);
                    EditorActionManager.ExecuteAction(action);
                }
                if (InputTracker.GetKeyDown(Key.W))
                {
                    Gizmos.Mode = Gizmos.GizmosMode.Translate;
                }
                if (InputTracker.GetKeyDown(Key.E))
                {
                    Gizmos.Mode = Gizmos.GizmosMode.Rotate;
                }

                // Use home key to cycle between gizmos origin modes
                if (InputTracker.GetKeyDown(Key.Home))
                {
                    if (Gizmos.Origin == Gizmos.GizmosOrigin.World)
                    {
                        Gizmos.Origin = Gizmos.GizmosOrigin.BoundingBox;
                    }
                    else if (Gizmos.Origin == Gizmos.GizmosOrigin.BoundingBox)
                    {
                        Gizmos.Origin = Gizmos.GizmosOrigin.World;
                    }
                }

                // Hide/Show
                if (InputTracker.GetControlShortcut(Key.H) && _selection.IsSelection())
                {
                    HideShowSelection();
                }

                // F key frames the selection
                if (InputTracker.GetKeyDown(Key.F))
                {
                    FrameSelection();
                }

                // Render settings
                if (InputTracker.GetControlShortcut(Key.Number1))
                {
                    RenderScene.DrawFilter = Scene.RenderFilter.MapPiece | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region;
                }
                else if (InputTracker.GetControlShortcut(Key.Number2))
                {
                    RenderScene.DrawFilter = Scene.RenderFilter.Collision | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region;
                }
                else if (InputTracker.GetControlShortcut(Key.Number3))
                {
                    RenderScene.DrawFilter = Scene.RenderFilter.Collision | Scene.RenderFilter.Navmesh | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region;
                }
                CFG.Current.LastSceneFilter = RenderScene.DrawFilter;
            }

            // Parse select commands
            string propSearchKey = null;
            if (initcmd != null && initcmd[0] == "propsearch")
            {
                if (initcmd.Length > 1)
                {
                    propSearchKey = initcmd[1];
                }
            }

            ImGui.SetNextWindowSize(new Vector2(300, 500), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Vector2(20, 20), ImGuiCond.FirstUseEver);

            System.Numerics.Vector3 clear_color = new System.Numerics.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
            //ImGui.Text($@"Viewport size: {Viewport.Width}x{Viewport.Height}");
            //ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

            Viewport.OnGui();

            SceneTree.OnGui();
            if (MapStudioNew.FirstFrame)
            {
                ImGui.SetNextWindowFocus();
            }
            PropEditor.OnGui(_selection, _selection.GetSingleFilteredSelection<Entity>(), "mapeditprop", Viewport.Width, Viewport.Height);
            DispGroupEditor.OnGui(Universe._dispGroupCount);
            PropSearch.OnGui(propSearchKey);

            // Not usable yet
            if (FeatureFlags.EnableNavmeshBuilder)
            {
                NavMeshEditor.OnGui(AssetLocator.Type);
            }

            ResourceManager.OnGuiDrawTasks(Viewport.Width, Viewport.Height);
            ResourceManager.OnGuiDrawResourceList();

            if (_activeModal != null)
            {
                if (_activeModal.IsClosed)
                {
                    _activeModal.OpenModal();
                }
                _activeModal.OnGui();
                if (_activeModal.IsClosed)
                {
                    _activeModal = null;
                }
            }
        }

        public void Draw(GraphicsDevice device, CommandList cl)
        {
            Viewport.Draw(device, cl);
        }

        /// <summary>
        /// Gets all the msb types using reflection to populate editor creation menus
        /// </summary>
        /// <param name="type">The game to collect msb types for</param>
        private void PopulateClassNames(GameType type)
        {
            Type msbclass;
            switch (type)
            {
                case GameType.DemonsSouls:
                    msbclass = typeof(MSBD);
                    break;
                case GameType.DarkSoulsPTDE:
                case GameType.DarkSoulsRemastered:
                    msbclass = typeof(MSB1);
                    break;
                case GameType.DarkSoulsIISOTFS:
                    msbclass = typeof(MSB2);
                    break;
                case GameType.DarkSoulsIII:
                    msbclass = typeof(MSB3);
                    break;
                case GameType.Bloodborne:
                    msbclass = typeof(MSBB);
                    break;
                case GameType.Sekiro:
                    msbclass = typeof(MSBS);
                    break;
                case GameType.EldenRing:
                    msbclass = typeof(MSBE);
                    break;
                default:
                    throw new ArgumentException("type must be valid");
            }

            var partType = msbclass.GetNestedType("Part");
            var partSubclasses = msbclass.Assembly.GetTypes().Where(type => type.IsSubclassOf(partType) && !type.IsAbstract).ToList();
            _partsClasses = partSubclasses.Select(x => (x.Name, x)).ToList();

            var regionType = msbclass.GetNestedType("Region");
            var regionSubclasses = msbclass.Assembly.GetTypes().Where(type => type.IsSubclassOf(regionType) && !type.IsAbstract).ToList();
            _regionClasses = regionSubclasses.Select(x => (x.Name, x)).ToList();
            if (_regionClasses.Count == 0)
            {
                _regionClasses.Add(("Region", regionType));
            }

            var eventType = msbclass.GetNestedType("Event");
            var eventSubclasses = msbclass.Assembly.GetTypes().Where(type => type.IsSubclassOf(eventType) && !type.IsAbstract).ToList();
            _eventClasses = eventSubclasses.Select(x => (x.Name, x)).ToList();
        }

        public override void OnProjectChanged(Editor.ProjectSettings newSettings)
        {
            _projectSettings = newSettings;
            _selection.ClearSelection();
            EditorActionManager.Clear();
        }
        public void ReloadUniverse()
        {
            TaskManager.Run("U:LoadUniverse", true, false, true, () =>
            {
                Universe.UnloadAllMaps();
                GC.Collect();
                Universe.PopulateMapList();

                if (AssetLocator.Type != GameType.Undefined)
                {
                    PopulateClassNames(AssetLocator.Type);
                }
            });
        }

        public override void Save()
        {
            try
            {
                Universe.SaveAllMaps();
            }
            catch (SavingFailedException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Wrapped.Message, e.Message,
                     System.Windows.Forms.MessageBoxButtons.OK,
                     System.Windows.Forms.MessageBoxIcon.None);
            }
        }

        public override void SaveAll()
        {
            try
            {
                Universe.SaveAllMaps();
            }
            catch (SavingFailedException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Wrapped.Message, e.Message,
                     System.Windows.Forms.MessageBoxButtons.OK,
                     System.Windows.Forms.MessageBoxIcon.None);
            }
        }

        public void OnEntityContextMenu(Entity ent)
        {
            if (ImGui.Selectable("Create prefab"))
            {
                _activeModal = new CreatePrefabModal(Universe, ent);
            }
        }
    }
}
