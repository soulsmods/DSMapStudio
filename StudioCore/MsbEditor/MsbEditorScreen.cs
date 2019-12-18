using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using ImGuiNET;

namespace StudioCore.MsbEditor
{
    public class MsbEditorScreen
    {
        //private ContentManager DebugReloadContentManager = null;

        public AssetLocator AssetLocator = new AssetLocator();
        public Resource.ResourceManager ResourceMan = new Resource.ResourceManager();
        public Scene.RenderScene RenderScene = new Scene.RenderScene();
        public ActionManager EditorActionManager = new ActionManager();

        public PropertyEditor PropEditor;

        public Map CurrentMap;

        public enum ScreenMouseHoverKind
        {
            None,
            AnimList,
            EventGraph,
            Inspector,
            ModelViewer,
            DividerBetweenCenterAndLeftPane,
            DividerBetweenCenterAndRightPane,
            DividerRightPaneHorizontal,
            ShaderAdjuster
        }

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

        public MsbEditorScreen(Sdl2Window window, GraphicsDevice device)
        {
            Rect = window.Bounds;
            ResourceMan.Locator = AssetLocator;
            Window = window;

            Viewport = new Gui.Viewport(device, RenderScene, EditorActionManager, Rect.Width, Rect.Height);
            CurrentMap = new Map();

            PropEditor = new PropertyEditor(EditorActionManager);
        }

        private bool ViewportUsingKeyboard = false;

        public void Update(float dt)
        {

            // Always update playback regardless of GUI memes.
            // Still only allow hitting spacebar to play/pause
            // if the window is in focus.
            // Also for Interactor

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

        bool MapObjectListOpen = true;

        //public void Draw(GraphicsDevice gd, SpriteBatch sb, Texture2D boxTex,
        //    SpriteFont font, GameTime gameTime, SpriteFont smallFont, Texture2D scrollbarArrowTex, ImGuiSystem.ImGuiRenderer guiRenderer)
        public void OnGUI()
        {
            // Docking setup
            var vp = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(vp.Pos);
            ImGui.SetNextWindowSize(vp.Size);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            flags |= ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
            flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
            flags |= ImGuiWindowFlags.NoBackground;
            ImGui.Begin("DockSpace_W", flags);
            ImGui.PopStyleVar(3);
            var dsid = ImGui.GetID("DockSpace");
            ImGui.DockSpace(dsid, new Vector2(0, 0));

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Set Interroot..", "CTRL+I") || InputTracker.GetControlShortcut(Key.I))
                    {
                        var browseDlg = new System.Windows.Forms.OpenFileDialog()
                        {
                            Filter = AssetLocator.GameExecutatbleFilter,
                            ValidateNames = true,
                            CheckFileExists = true,
                            CheckPathExists = true,
                            //ShowReadOnly = true,
                        };

                        /*if (System.IO.File.Exists(FileContainerName))
                        {
                            browseDlg.InitialDirectory = System.IO.Path.GetDirectoryName(FileContainerName);
                            browseDlg.FileName = System.IO.Path.GetFileName(FileContainerName);
                        }*/

                        if (browseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            if (!AssetLocator.SetGameRootDirectoryByExePath(browseDlg.FileName))
                            {
                                System.Windows.Forms.MessageBox.Show("The game you selected could not be detected as a valid supported game", "Error",
                                    System.Windows.Forms.MessageBoxButtons.OK,
                                    System.Windows.Forms.MessageBoxIcon.None);
                            }
                        }
                    }
                    if (ImGui.BeginMenu("Open map", AssetLocator.Type != GameType.Undefined))
                    {
                        foreach (var map in AssetLocator.GetFullMapList())
                        {
                            if (ImGui.MenuItem(map)) 
                            {
                                var chrsToLoad = new HashSet<AssetDescription>();
                                var objsToLoad = new HashSet<AssetDescription>();
                                var colsToLoad = new HashSet<AssetDescription>();
                                var navsToLoad = new HashSet<AssetDescription>();
                                //try
                                //{
                                var ad = AssetLocator.GetMapMSB(map);
                                IMsb msb;
                                if (AssetLocator.Type == GameType.DarkSoulsIII)
                                {
                                    msb = MSB3.Read(ad.AssetPath);
                                }
                                else if (AssetLocator.Type == GameType.Sekiro)
                                {
                                    msb = MSBS.Read(ad.AssetPath);
                                }
                                else
                                {
                                    msb = MSB1.Read(ad.AssetPath);
                                }
                                CurrentMap.LoadMSB(msb);

                                // Temporary garbage
                                foreach (var obj in CurrentMap.MapObjects)
                                {
                                    if (obj.MsbObject is IMsbPart mp && mp.ModelName != null && mp.ModelName != "")
                                    {
                                        AssetDescription asset;
                                        bool loadcol = false;
                                        bool loadnav = false;
                                        Scene.RenderFilter filt = Scene.RenderFilter.All;
                                        if (mp.ModelName.StartsWith("m"))
                                        {
                                            asset = AssetLocator.GetMapModel(map, AssetLocator.MapModelNameToAssetName(map, mp.ModelName));
                                            filt = Scene.RenderFilter.MapPiece;
                                        }
                                        else if (mp.ModelName.StartsWith("c"))
                                        {
                                            asset = AssetLocator.GetChrModel(mp.ModelName);
                                            filt = Scene.RenderFilter.Character;
                                            chrsToLoad.Add(asset);
                                        }
                                        else if (mp.ModelName.StartsWith("o"))
                                        {
                                            asset = AssetLocator.GetObjModel(mp.ModelName);
                                            filt = Scene.RenderFilter.Object;
                                            objsToLoad.Add(asset);
                                        }
                                        else if (mp.ModelName.StartsWith("h"))
                                        {
                                            loadcol = true;
                                            asset = AssetLocator.GetMapCollisionModel(map, AssetLocator.MapModelNameToAssetName(map, mp.ModelName));
                                            filt = Scene.RenderFilter.Collision;
                                            colsToLoad.Add(asset);
                                        }
                                        else if (mp.ModelName.StartsWith("n"))
                                        {
                                            loadnav = true;
                                            asset = AssetLocator.GetMapNVMModel(map, AssetLocator.MapModelNameToAssetName(map, mp.ModelName));
                                            filt = Scene.RenderFilter.Navmesh;
                                            navsToLoad.Add(asset);
                                        }
                                        else
                                        {
                                            asset = AssetLocator.GetNullAsset();
                                        }

                                        if (loadcol)
                                        {
                                            var res = ResourceMan.GetResource<Resource.HavokCollisionResource>(asset.AssetVirtualPath);
                                            var mesh = new Scene.CollisionMesh(RenderScene, res, false);
                                            mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                                            obj.RenderSceneMesh = mesh;
                                            mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                                        }
                                        else if (loadnav)
                                        {
                                            var res = ResourceMan.GetResource<Resource.NVMNavmeshResource>(asset.AssetVirtualPath);
                                            var mesh = new Scene.NvmMesh(RenderScene, res, false);
                                            mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                                            obj.RenderSceneMesh = mesh;
                                            mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                                        }
                                        else
                                        {
                                            var res = ResourceMan.GetResource<Resource.FlverResource>(asset.AssetVirtualPath);
                                            var model = new NewMesh(RenderScene, res, false);
                                            model.DrawFilter = filt;
                                            model.WorldMatrix = obj.GetTransform().WorldMatrix;
                                            obj.RenderSceneMesh = model;
                                            model.Selectable = new WeakReference<Scene.ISelectable>(obj);
                                        }
                                    }
                                    if (obj.MsbObject is MSB1.Region r && r.Shape is MSB1.Shape.Box b)
                                    {
                                        var mesh = Scene.Region.GetBoxRegion(RenderScene, b.Width, b.Height, b.Depth);
                                        mesh.WorldMatrix = obj.GetTransform().WorldMatrix;
                                        obj.RenderSceneMesh = mesh;
                                        mesh.Selectable = new WeakReference<Scene.ISelectable>(obj);
                                    }
                                }
                                //}
                                /*catch (Exception e)
                                {

                                }*/
                                var job = ResourceMan.CreateNewJob($@"Loading {map} geometry");
                                foreach (var mappiece in AssetLocator.GetMapModels(map))
                                {
                                    if (mappiece.AssetArchiveVirtualPath != null)
                                    {
                                        job.AddLoadArchiveTask(mappiece.AssetArchiveVirtualPath, false);
                                    }
                                    else if (mappiece.AssetVirtualPath != null)
                                    {
                                        job.AddLoadFileTask(mappiece.AssetVirtualPath);
                                    }
                                }
                                job.StartJobAsync();
                                job = ResourceMan.CreateNewJob($@"Loading {map} collisions");
                                foreach (var col in colsToLoad)
                                {
                                    if (col.AssetArchiveVirtualPath != null)
                                    {
                                        job.AddLoadArchiveTask(col.AssetArchiveVirtualPath, false);
                                    }
                                    else if (col.AssetVirtualPath != null)
                                    {
                                        job.AddLoadFileTask(col.AssetVirtualPath);
                                    }
                                }
                                job.StartJobAsync();
                                job = ResourceMan.CreateNewJob($@"Loading chrs");
                                foreach (var chr in chrsToLoad)
                                {
                                    if (chr.AssetArchiveVirtualPath != null)
                                    {
                                        job.AddLoadArchiveTask(chr.AssetArchiveVirtualPath, false);
                                    }
                                    else if (chr.AssetVirtualPath != null)
                                    {
                                        job.AddLoadFileTask(chr.AssetVirtualPath);
                                    }
                                }
                                job.StartJobAsync();
                                job = ResourceMan.CreateNewJob($@"Loading objs");
                                foreach (var obj in objsToLoad)
                                {
                                    if (obj.AssetArchiveVirtualPath != null)
                                    {
                                        job.AddLoadArchiveTask(obj.AssetArchiveVirtualPath, false);
                                    }
                                    else if (obj.AssetVirtualPath != null)
                                    {
                                        job.AddLoadFileTask(obj.AssetVirtualPath);
                                    }
                                }
                                job.StartJobAsync();

                                job = ResourceMan.CreateNewJob($@"Loading Navmeshes");
                                foreach (var nav in navsToLoad)
                                {
                                    if (nav.AssetArchiveVirtualPath != null)
                                    {
                                        job.AddLoadArchiveTask(nav.AssetArchiveVirtualPath, false);
                                    }
                                    else if (nav.AssetVirtualPath != null)
                                    {
                                        job.AddLoadFileTask(nav.AssetVirtualPath);
                                    }
                                }
                                job.StartJobAsync();
                            }
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.MenuItem("Close Map"))
                    {
                        Selection.ClearSelection();
                        EditorActionManager.Clear();
                        CurrentMap.Clear();
                        GC.Collect();
                    }
                    if (ImGui.MenuItem("Save MSB", "CTRL+S") || InputTracker.GetControlShortcut(Key.S)) { }
                    ImGui.EndMenu();
                }
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
                    if (ImGui.MenuItem("Delete", "Delete", false, Selection.IsSelection()))
                    {
                        var action = new DeleteMapObjectsAction(CurrentMap, RenderScene, Selection.GetFilteredSelection<MapObject>().ToList(), true);
                        EditorActionManager.ExecuteAction(action);
                    }
                    if (ImGui.MenuItem("Duplicate", "Ctrl+D", false, Selection.IsSelection()))
                    {
                        var action = new CloneMapObjectsAction(CurrentMap, RenderScene, Selection.GetFilteredSelection<MapObject>().ToList(), true);
                        EditorActionManager.ExecuteAction(action);
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Display"))
                {
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
                ImGui.EndMainMenuBar();
            }

            // Keyboard shortcuts
            if (!ViewportUsingKeyboard && !ImGui.GetIO().WantCaptureKeyboard)
            {
                if (EditorActionManager.CanUndo() && InputTracker.GetControlShortcut(Key.Z))
                {
                    EditorActionManager.UndoAction();
                }
                if (EditorActionManager.CanRedo() && InputTracker.GetControlShortcut(Key.Y))
                {
                    EditorActionManager.RedoAction();
                }
                if (InputTracker.GetControlShortcut(Key.D) && Selection.IsSelection())
                {
                    var action = new CloneMapObjectsAction(CurrentMap, RenderScene, Selection.GetFilteredSelection<MapObject>().ToList(), true);
                    EditorActionManager.ExecuteAction(action);
                }
                if (InputTracker.GetKeyDown(Key.Delete) && Selection.IsSelection())
                {
                    var action = new DeleteMapObjectsAction(CurrentMap, RenderScene, Selection.GetFilteredSelection<MapObject>().ToList(), true);
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
            }

            ImGui.SetNextWindowSize(new Vector2(300, 500), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Vector2(20, 20), ImGuiCond.FirstUseEver);

            System.Numerics.Vector3 clear_color = new System.Numerics.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
            //ImGui.Text($@"Viewport size: {Viewport.Width}x{Viewport.Height}");
            //ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

            Viewport.OnGui();

            if (ImGui.Begin("Map Object List", ref MapObjectListOpen))
            {
                foreach (var obj in CurrentMap.MapObjects)
                {
                    if (ImGui.Selectable(obj.Name, Selection.GetSelection().Contains(obj)))
                    {
                        if (InputTracker.GetKey(Key.ShiftLeft) || InputTracker.GetKey(Key.ShiftRight))
                        {
                            Selection.AddSelection(obj);
                        }
                        else
                        {
                            Selection.ClearSelection();
                            Selection.AddSelection(obj);
                        }
                    }
                }
                ImGui.End();
            }

            PropEditor.OnGui(Selection.GetSingleFilteredSelection<MapObject>(), Viewport.Width, Viewport.Height);

            ResourceMan.OnGuiDrawTasks(Viewport.Width, Viewport.Height);

            //guiRenderer.AfterLayout();

            ImGui.End();
        }

        public void Draw(GraphicsDevice device, CommandList cl)
        {
            Viewport.Draw(device, cl);
        }
    }
}
