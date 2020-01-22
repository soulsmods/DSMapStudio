using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
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
        public SearchProperties PropSearch;
        public DisplayGroupsEditor DispGroupEditor;

        public Universe Universe;

        private bool GCNeedsCollection = false;

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
            Universe = new Universe(AssetLocator, ResourceMan, RenderScene);

            PropEditor = new PropertyEditor(EditorActionManager);
            DispGroupEditor = new DisplayGroupsEditor(RenderScene);
            PropSearch = new SearchProperties(Universe);

            ParamBank.LoadParams(AssetLocator);
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

                        if (browseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            if (!AssetLocator.SetGameRootDirectoryByExePath(browseDlg.FileName))
                            {
                                System.Windows.Forms.MessageBox.Show("The game you selected could not be detected as a valid supported game", "Error",
                                    System.Windows.Forms.MessageBoxButtons.OK,
                                    System.Windows.Forms.MessageBoxIcon.None);
                            }
                            else
                            {
                                ParamBank.ReloadParams();
                            }
                        }
                    }
                    if (ImGui.MenuItem("Set Mod Project Directory..", ""))
                    {
                        var browseDlg = new System.Windows.Forms.FolderBrowserDialog()
                        {
                            SelectedPath = AssetLocator.GameRootDirectory
                        };

                        if (browseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            AssetLocator.SetModProjectDirectory(browseDlg.SelectedPath);
                            ParamBank.ReloadParams();
                        }
                    }
                    if (ImGui.BeginMenu("Open map", AssetLocator.Type != GameType.Undefined))
                    {
                        foreach (var map in AssetLocator.GetFullMapList())
                        {
                            if (ImGui.MenuItem(map)) 
                            {
                                Universe.LoadMap(map);
                            }
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.MenuItem("Unload All Open Maps"))
                    {
                        Selection.ClearSelection();
                        EditorActionManager.Clear();
                        Universe.UnloadAllMaps();
                        GC.Collect();
                    }
                    if (ImGui.MenuItem("Save All Open Maps", "CTRL+S") || InputTracker.GetControlShortcut(Key.S))
                    {
                        Universe.SaveAllMaps();
                    }
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
                        var action = new DeleteMapObjectsAction(Universe, RenderScene, Selection.GetFilteredSelection<MapObject>().ToList(), true);
                        EditorActionManager.ExecuteAction(action);
                    }
                    if (ImGui.MenuItem("Duplicate", "Ctrl+D", false, Selection.IsSelection()))
                    {
                        var action = new CloneMapObjectsAction(Universe, RenderScene, Selection.GetFilteredSelection<MapObject>().ToList(), true);
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
                        if (ImGui.MenuItem("Map Piece/Character/Objects"))
                        {
                            RenderScene.DrawFilter = Scene.RenderFilter.MapPiece | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region;
                        }
                        if (ImGui.MenuItem("Collision/Character/Objects"))
                        {
                            RenderScene.DrawFilter = Scene.RenderFilter.Collision | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region;
                        }
                        if (ImGui.MenuItem("Collision/Navmesh/Character/Objects"))
                        {
                            RenderScene.DrawFilter = Scene.RenderFilter.Collision | Scene.RenderFilter.Navmesh | Scene.RenderFilter.Object | Scene.RenderFilter.Character | Scene.RenderFilter.Region;
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
                if (InputTracker.GetControlShortcut(Key.D) && Selection.IsSelection())
                {
                    var action = new CloneMapObjectsAction(Universe, RenderScene, Selection.GetFilteredSelection<MapObject>().ToList(), true);
                    EditorActionManager.ExecuteAction(action);
                }
                if (InputTracker.GetKeyDown(Key.Delete) && Selection.IsSelection())
                {
                    var action = new DeleteMapObjectsAction(Universe, RenderScene, Selection.GetFilteredSelection<MapObject>().ToList(), true);
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

                // F key frames the selection
                if (InputTracker.GetKeyDown(Key.F))
                {
                    var selected = Selection.GetFilteredSelection<MapObject>();
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
            }

            ImGui.SetNextWindowSize(new Vector2(300, 500), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Vector2(20, 20), ImGuiCond.FirstUseEver);

            System.Numerics.Vector3 clear_color = new System.Numerics.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
            //ImGui.Text($@"Viewport size: {Viewport.Width}x{Viewport.Height}");
            //ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

            Viewport.OnGui();

            if (ImGui.Begin("Map Object List", ref MapObjectListOpen))
            {
                Map pendingUnload = null;
                foreach (var map in Universe.LoadedMaps)
                {
                    var treeflags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;
                    if (Selection.GetSelection().Contains(map.RootObject))
                    {
                        treeflags |= ImGuiTreeNodeFlags.Selected;
                    }
                    var nodeopen = ImGui.TreeNodeEx($@"{ForkAwesome.Cube} {map.MapId}", treeflags);
                    // Right click context menu
                    if (ImGui.BeginPopupContextItem($@"mapcontext_{map.MapId}"))
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
                        ImGui.EndPopup();
                    }
                    if (ImGui.IsItemClicked())
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
                        foreach (var obj in map.MapObjects)
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
                            bool visible = obj.EditorVisible;
                            ImGui.SameLine(ImGui.GetWindowContentRegionWidth() - 12.0f);
                            ImGui.PushStyleColor(ImGuiCol.Text, visible ? new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                                : new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
                            ImGui.TextWrapped(visible ? ForkAwesome.Eye : ForkAwesome.EyeSlash);
                            ImGui.PopStyleColor();
                            if (ImGui.IsItemClicked(0))
                            {
                                obj.EditorVisible = !obj.EditorVisible;
                                doSelect = false;
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
                        ImGui.TreePop();
                    }
                }
                ImGui.End();

                if (pendingUnload != null)
                {
                    Universe.UnloadMap(pendingUnload);
                    GC.Collect();
                    GCNeedsCollection = true;
                }
            }

            PropEditor.OnGui(Selection.GetSingleFilteredSelection<MapObject>(), Viewport.Width, Viewport.Height);
            DispGroupEditor.OnGui(AssetLocator.Type);
            PropSearch.OnGui();

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
