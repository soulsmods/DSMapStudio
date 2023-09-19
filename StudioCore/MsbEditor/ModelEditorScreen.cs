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
using StudioCore.Editor;
using ImGuiNET;
using StudioCore.Gui;

namespace StudioCore.MsbEditor
{
    public class ModelEditorScreen : EditorScreen, AssetBrowserEventHandler, SceneTreeEventHandler, IResourceEventListener
    {
        public string EditorName => "Model Editor";
        public string CommandEndpoint => "model";
        public string SaveType => "Models";
        
        public AssetLocator AssetLocator = null;
        public Scene.RenderScene RenderScene;
        public ActionManager EditorActionManager = new ActionManager();
        private Selection _selection = new Selection();
        private Sdl2Window Window;
        public Gui.IViewport Viewport;
        public Rectangle Rect;

        private Universe _universe;

        private SceneTree _sceneTree;
        private PropertyEditor _propEditor;
        private AssetBrowser _assetBrowser;

        private ResourceHandle<FlverResource> _flverhandle = null;
        private string _currentModel = null;
        private MeshRenderableProxy _renderMesh = null;

        private Task _loadingTask = null;

        public ModelEditorScreen(Sdl2Window window, GraphicsDevice device, AssetLocator locator)
        {
            Rect = window.Bounds;
            AssetLocator = locator;
            ResourceManager.Locator = AssetLocator;
            Window = window;

            if (device != null)
            {
                RenderScene = new RenderScene();
                Viewport = new Gui.Viewport("Modeleditvp", device, RenderScene, EditorActionManager, _selection,
                    Rect.Width, Rect.Height);
            }
            else
            {
                Viewport = new NullViewport("Modeleditvp", EditorActionManager, _selection, Rect.Width, Rect.Height);
            }

            _universe = new Universe(AssetLocator, RenderScene, _selection);

            _sceneTree = new SceneTree(SceneTree.Configuration.ModelEditor, this, "modeledittree", _universe, _selection, EditorActionManager, Viewport, AssetLocator);
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
            if (Viewport != null)
                Viewport.Draw(device, cl);
        }

        public void DrawEditorMenu()
        {
            
        }

        public void LoadModel(string modelid, ModelEditorModelType modelType, string mapid = null)
        {
            AssetDescription asset;
            AssetDescription assettex;
            Scene.RenderFilter filt = Scene.RenderFilter.All;
            var job = ResourceManager.CreateNewJob($@"Loading mesh");
            switch (modelType)
            {
                case ModelEditorModelType.Character:
                    asset = AssetLocator.GetChrModel(modelid);
                    assettex = AssetLocator.GetChrTextures(modelid);
                    break;
                case ModelEditorModelType.Object:
                    asset = AssetLocator.GetObjModel(modelid);
                    assettex = AssetLocator.GetObjTexture(modelid);
                    break;
                case ModelEditorModelType.Parts:
                    asset = AssetLocator.GetPartsModel(modelid);
                    assettex = AssetLocator.GetPartTextures(modelid);
                    break;
                case ModelEditorModelType.MapPiece:
                    asset = AssetLocator.GetMapModel(mapid, modelid);
                    assettex = AssetLocator.GetNullAsset();
                    break;
                default:
                    //Uh oh
                    asset = AssetLocator.GetNullAsset();
                    assettex = AssetLocator.GetNullAsset();
                    break;
            }
            
            if (_renderMesh != null)
            {
                //RenderScene.RemoveObject(_renderMesh);
            }
            _renderMesh = MeshRenderableProxy.MeshRenderableFromFlverResource(
                RenderScene, asset.AssetVirtualPath, ModelMarkerType.None);
            //_renderMesh.DrawFilter = filt;
            _renderMesh.World = Matrix4x4.Identity;
            _currentModel = modelid;
            if (!ResourceManager.IsResourceLoadedOrInFlight(asset.AssetVirtualPath, AccessLevel.AccessFull))
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
                _loadingTask = job.Complete();
            }
            ResourceManager.AddResourceListener<FlverResource>(asset.AssetVirtualPath, this, AccessLevel.AccessFull);
        }

        public void OnInstantiateChr(string chrid)
        {
            LoadModel(chrid, ModelEditorModelType.Character);
        }

        public void OnInstantiateObj(string objid)
        {
            LoadModel(objid, ModelEditorModelType.Object);
        }
        public void OnInstantiateParts(string partsid)
        {
            LoadModel(partsid, ModelEditorModelType.Parts);
        }

        public void OnInstantiateMapPiece(string mapid, string modelid)
        {
            LoadModel(modelid, ModelEditorModelType.MapPiece, mapid);
        }

        public void OnGUI(string[] commands)
        {
            float scale = MapStudioNew.GetUIScale();
            // Docking setup
            //var vp = ImGui.GetMainViewport();
            var wins = ImGui.GetWindowSize();
            var winp = ImGui.GetWindowPos();
            winp.Y += 20.0f * scale;
            wins.Y -= 20.0f * scale;
            ImGui.SetNextWindowPos(winp);
            ImGui.SetNextWindowSize(wins);
            var dsid = ImGui.GetID("DockSpace_ModelEdit");
            ImGui.DockSpace(dsid, new Vector2(0, 0));

            // Keyboard shortcuts
            if (EditorActionManager.CanUndo() && InputTracker.GetKeyDown(KeyBindings.Current.Core_Undo))
            {
                EditorActionManager.UndoAction();
            }
            if (EditorActionManager.CanRedo() && InputTracker.GetKeyDown(KeyBindings.Current.Core_Redo))
            {
                EditorActionManager.RedoAction();
            }
            if (!ViewportUsingKeyboard && !ImGui.GetIO().WantCaptureKeyboard)
            {
                if (InputTracker.GetKeyDown(KeyBindings.Current.Viewport_TranslateMode))
                {
                    Gizmos.Mode = Gizmos.GizmosMode.Translate;
                }
                if (InputTracker.GetKeyDown(KeyBindings.Current.Viewport_RotationMode))
                {
                    Gizmos.Mode = Gizmos.GizmosMode.Rotate;
                }

                // Use home key to cycle between gizmos origin modes
                if (InputTracker.GetKeyDown(KeyBindings.Current.Viewport_ToggleGizmoOrigin))
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
                if (InputTracker.GetKeyDown(KeyBindings.Current.Viewport_FrameSelection))
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

            ImGui.SetNextWindowSize(new Vector2(300, 500) * scale, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Vector2(20, 20) * scale, ImGuiCond.FirstUseEver);

            System.Numerics.Vector3 clear_color = new System.Numerics.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
            //ImGui.Text($@"Viewport size: {Viewport.Width}x{Viewport.Height}");
            //ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

            Viewport.OnGui();
            _assetBrowser.OnGui();
            _sceneTree.OnGui();
            _propEditor.OnGui(_selection, "modeleditprop", Viewport.Width, Viewport.Height);
            ResourceManager.OnGuiDrawTasks(Viewport.Width, Viewport.Height);
        }
        
        public bool InputCaptured()
        {
            return Viewport.ViewportSelected;
        }

        public void OnProjectChanged(Editor.ProjectSettings newSettings)
        {
            ReloadAssetBrowser();
        }

        public void ReloadAssetBrowser()
        {
            if (AssetLocator.Type != GameType.Undefined)
            {
                _assetBrowser.ClearCaches();
                _assetBrowser.UpdateReferenceDicts();
            }
        }

        public void Save()
        {

        }

        public void SaveAll()
        {

        }

        public void OnEntityContextMenu(Entity ent)
        {

        }

        public void OnResourceLoaded(IResourceHandle handle, int tag)
        {
            _flverhandle = (ResourceHandle<FlverResource>)handle;
            _flverhandle.Acquire();

            if (_renderMesh != null)
            {
                var box = _renderMesh.GetBounds();
                Viewport.FrameBox(box);

                var dim = box.GetDimensions();
                var mindim = Math.Min(dim.X, Math.Min(dim.Y, dim.Z));
                var maxdim = Math.Max(dim.X, Math.Max(dim.Y, dim.Z));

                var minSpeed = 1.0f;
                var basespeed = Math.Max(minSpeed, (float)Math.Sqrt(mindim / 3.0f));
                Viewport.WorldView.CameraMoveSpeed_Normal = basespeed;
                Viewport.WorldView.CameraMoveSpeed_Slow = basespeed / 10.0f;
                Viewport.WorldView.CameraMoveSpeed_Fast = basespeed * 10.0f;

                Viewport.NearClip = Math.Max(0.001f, maxdim / 10000.0f);
            }

            if (_flverhandle.IsLoaded && _flverhandle.Get() != null)
            {
                var r = _flverhandle.Get();
                if (r.Flver != null)
                {
                    _universe.UnloadAll(true);
                    _universe.LoadFlver(r.Flver, _renderMesh, _currentModel);
                }
            }
        }

        public void OnResourceUnloaded(IResourceHandle handle, int tag)
        {
            _flverhandle = null;
        }
    }
}
