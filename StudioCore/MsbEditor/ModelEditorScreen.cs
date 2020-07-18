using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using StudioCore.Resource;
using StudioCore.Scene;
using ImGuiNET;

namespace StudioCore.MsbEditor
{
    public class ModelEditorScreen : EditorScreen, AssetBrowserEventHandler, SceneTreeEventHandler
    {
        public AssetLocator AssetLocator = null;
        public Scene.RenderScene RenderScene = new Scene.RenderScene();
        public ActionManager EditorActionManager = new ActionManager();
        private Selection _selection = new Selection();
        private Sdl2Window Window;
        public Gui.Viewport Viewport;
        public Rectangle Rect;

        private Universe _universe;

        private SceneTree _sceneTree;
        private PropertyEditor _propEditor;
        private AssetBrowser _assetBrowser;

        private ResourceHandle<FlverResource> _flverhandle = null;
        private string _currentModel = null;
        private IDrawable _renderMesh = null;

        private Task _loadingTask = null;

        public ModelEditorScreen(Sdl2Window window, GraphicsDevice device, AssetLocator locator)
        {
            Rect = window.Bounds;
            AssetLocator = locator;
            ResourceManager.Locator = AssetLocator;
            Window = window;

            Viewport = new Gui.Viewport("Modeleditvp", device, RenderScene, EditorActionManager, _selection, Rect.Width, Rect.Height);
            _universe = new Universe(AssetLocator, RenderScene, _selection);

            _sceneTree = new SceneTree(this, "modeledittree", _universe, _selection, EditorActionManager, Viewport, AssetLocator);
            _propEditor = new PropertyEditor(EditorActionManager);
            _assetBrowser = new AssetBrowser(this, "modelEditorBrowser", AssetLocator);
        }

        private bool ViewportUsingKeyboard = false;

        public void Update(float dt)
        {
            ViewportUsingKeyboard = Viewport.Update(Window, dt);

            if (_loadingTask != null && _loadingTask.IsCompleted)
            {
                _loadingTask = null;
                if (_renderMesh != null)
                {
                    var box = _renderMesh.GetBounds();
                    Viewport.FrameBox(box);

                    var dim = box.GetDimensions();
                    var mindim = Math.Min(dim.X, Math.Min(dim.Y, dim.Z));
                    var maxdim = Math.Max(dim.X, Math.Max(dim.Y, dim.Z));
                    var basespeed = (float)Math.Sqrt(mindim / 3.0f);
                    Viewport._worldView.CameraMoveSpeed = basespeed;
                    Viewport._worldView.CameraMoveSpeedSlow = basespeed / 10.0f;
                    Viewport._worldView.CameraMoveSpeedFast = basespeed * 10.0f;

                    Viewport.FarClip = Math.Max(10.0f, maxdim * 10.0f);
                    Viewport.NearClip = Math.Max(0.001f, maxdim / 10000.0f);
                }

                if (_flverhandle.IsLoaded && _flverhandle.Get() != null)
                {
                    if (_flverhandle.TryLock())
                    {
                        var r = _flverhandle.Get();
                        if (r.Flver != null)
                        {
                            _universe.LoadFlver(r.Flver, _currentModel);
                        }
                        _flverhandle.Unlock();
                    }
                }
            }
        }

        public void EditorResized(Sdl2Window window, GraphicsDevice device)
        {
            Window = window;
            Rect = window.Bounds;
            //Viewport.ResizeViewport(device, new Rectangle(0, 0, window.Width, window.Height));
        }

        public void Draw(GraphicsDevice device, CommandList cl)
        {
            Viewport.Draw(device, cl);
        }

        public override void DrawEditorMenu()
        {
            
        }

        public void LoadModel(string modelid)
        {
            AssetDescription asset;
            AssetDescription assettex;
            Scene.RenderFilter filt = Scene.RenderFilter.All;
            var job = ResourceManager.CreateNewJob($@"Loading mesh");
            if (modelid.StartsWith("c"))
            {
                asset = AssetLocator.GetChrModel(modelid);
                assettex = AssetLocator.GetChrTextures(modelid);
            }
            else if (modelid.StartsWith("o"))
            {
                asset = AssetLocator.GetObjModel(modelid);
                assettex = AssetLocator.GetNullAsset();
            }
            else
            {
                asset = AssetLocator.GetNullAsset();
                assettex = AssetLocator.GetNullAsset();
            }

            var res = ResourceManager.GetResource<Resource.FlverResource>(asset.AssetVirtualPath);
            if (_renderMesh != null)
            {
                RenderScene.RemoveObject(_renderMesh);
            }
            _renderMesh = new NewMesh(RenderScene, res);
            _renderMesh.DrawFilter = filt;
            _renderMesh.WorldMatrix = Matrix4x4.Identity;
            _flverhandle = res;
            _currentModel = modelid;
            if (!res.IsLoaded || res.AccessLevel != AccessLevel.AccessFull)
            {
                if (asset.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(asset.AssetArchiveVirtualPath, AccessLevel.AccessFull, false, Resource.ResourceManager.ResourceType.Flver);
                }
                else if (asset.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(asset.AssetVirtualPath, AccessLevel.AccessFull);
                }
                if (assettex.AssetArchiveVirtualPath != null)
                {
                    job.AddLoadArchiveTask(assettex.AssetArchiveVirtualPath, AccessLevel.AccessGPUOptimizedOnly, false, Resource.ResourceManager.ResourceType.Texture);
                }
                else if (assettex.AssetVirtualPath != null)
                {
                    job.AddLoadFileTask(assettex.AssetVirtualPath, AccessLevel.AccessGPUOptimizedOnly);
                }
                _loadingTask = job.StartJobAsync();
            }
        }

        public void OnInstantiateChr(string chrid)
        {
            LoadModel(chrid);
        }

        public void OnInstantiateObj(string objid)
        {
            LoadModel(objid);
        }

        public void OnGUI()
        {
            // Docking setup
            //var vp = ImGui.GetMainViewport();
            var wins = ImGui.GetWindowSize();
            var winp = ImGui.GetWindowPos();
            winp.Y += 20.0f;
            wins.Y -= 20.0f;
            ImGui.SetNextWindowPos(winp);
            ImGui.SetNextWindowSize(wins);
            var dsid = ImGui.GetID("DockSpace_ModelEdit");
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
            }

            ImGui.SetNextWindowSize(new Vector2(300, 500), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Vector2(20, 20), ImGuiCond.FirstUseEver);

            System.Numerics.Vector3 clear_color = new System.Numerics.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
            //ImGui.Text($@"Viewport size: {Viewport.Width}x{Viewport.Height}");
            //ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

            Viewport.OnGui();
            _assetBrowser.OnGui();
            _sceneTree.OnGui();
            _propEditor.OnGui(_selection.GetSingleFilteredSelection<Entity>(), "modeleditprop", Viewport.Width, Viewport.Height);
            ResourceManager.OnGuiDrawTasks(Viewport.Width, Viewport.Height);
        }

        public override void OnProjectChanged(ProjectSettings newSettings)
        {
            _assetBrowser.RebuildCaches();
        }

        public override void Save()
        {
            
        }

        public override void SaveAll()
        {
            
        }

        public void OnEntityContextMenu(Entity ent)
        {

        }
    }
}
