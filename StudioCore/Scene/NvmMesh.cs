using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using StudioCore.Scene;
using StudioCore.Resource;

namespace StudioCore.Scene
{
    public class NvmMesh : Scene.IDrawable, Resource.IResourceEventListener, IDisposable
    {
        public NvmRenderer RenderMesh;

        private Resource.ResourceHandle<Resource.NVMNavmeshResource> Resource;
        private bool Created = false;

        private object _lock_submeshes = new object();

        //public BoundingBox Bounds;

        public bool AutoRegister { get; set; } = true;
        private Scene.RenderScene RenderScene;
        private bool Registered = false;
        private DebugPrimitives.DbgPrimWireBox DebugBoundingBox = null;

        public WeakReference<ISelectable> Selectable { get; set; }

        public RenderFilter DrawFilter { get; set; } = RenderFilter.Navmesh;

        private bool _DebugDrawBounds = false;
        public bool Highlighted
        {
            set
            {
                _DebugDrawBounds = value;
                if (_DebugDrawBounds)
                {
                    if (DebugBoundingBox == null)
                    {
                        DebugBoundingBox = new DebugPrimitives.DbgPrimWireBox(new Transform(_WorldMatrix), Bounds.Min, Bounds.Max, System.Drawing.Color.Red);
                        //RenderScene.AddObject(DebugBoundingBox);
                        Scene.Renderer.AddBackgroundUploadTask((device, cl) =>
                        {
                            DebugBoundingBox.CreateDeviceObjects(device, cl, null);
                        });
                    }
                    else
                    {
                        DebugBoundingBox.EnableDraw = true;
                    }
                }
                else
                {
                    DebugBoundingBox.EnableDraw = false;
                }
            }
            get
            {
                return _DebugDrawBounds;
            }
        }

        private Matrix4x4 _WorldMatrix = Matrix4x4.Identity;
        public Matrix4x4 WorldMatrix
        {
            get
            {
                return _WorldMatrix;
            }
            set
            {
                _WorldMatrix = value;
                OnWorldMatrixChanged();
            }
        }

        public BoundingBox Bounds { get; private set; }
        public DrawGroup DrawGroups { get; set; } = new DrawGroup();

        public NvmMesh(Scene.RenderScene scene, Resource.ResourceHandle<Resource.NVMNavmeshResource> res, bool useSecondUV, Dictionary<string, int> boneIndexRemap = null,
            bool ignoreStaticTransforms = false)
        {
            RenderScene = scene;
            Resource = res;
            Resource.Acquire();
            res.AddResourceEventListener(this);
        }

        public NvmMesh(NvmMesh mesh)
        {
            RenderScene = mesh.RenderScene;
            Resource = mesh.Resource;
            Resource.Acquire();
            Resource.AddResourceEventListener(this);
        }

        ~NvmMesh()
        {
            if (Registered)
            {
                UnregisterWithScene();
            }
            if (Resource != null)
            {
                Resource.Release();
            }
        }

        public void OnResourceLoaded(IResourceHandle handle)
        {
            if (Resource != null && Resource.TryLock())
            {
                CreateSubmeshes();
                OnWorldMatrixChanged();
                Renderer.AddBackgroundUploadTask((d, cl) =>
                {
                    if (RenderMesh != null && Resource.TryLock())
                    {
                        RenderMesh.CreateDeviceObjects(d, cl, null);
                        if (AutoRegister)
                        {
                            RegisterWithScene(RenderScene);
                        }
                        Created = true;
                        Resource.Unlock();
                    }
                });
                Resource.Unlock();
            }
        }

        public void OnResourceUnloaded(IResourceHandle handle)
        {
            if (Resource != null)
            {
                Created = false;
                UnregisterWithScene();
                RenderMesh = null;
            }
        }

        private void OnWorldMatrixChanged()
        {
            var res = Resource.Get();
            if (res != null)
            {

                RenderMesh.WorldTransform = _WorldMatrix;
                
            }
            if (DebugBoundingBox != null)
            {
                DebugBoundingBox.Transform = new Transform(_WorldMatrix);
            }
            RenderScene.ObjectMoved(this);
        }

        private void CreateSubmeshes()
        {
            var res = Resource.Get();
            Bounds = res.Bounds;
            RenderMesh = new NvmRenderer(this, Resource);
            //Bounds = res.Bounds;
        }

        /*public List<CollisionRenderer> GetLoadedSubmeshes()
        {
            if (Submeshes.Count() == 0 && Resource.IsLoaded && Resource.Get().GPUMeshes != null)
            {
                CreateSubmeshes();
                return Submeshes;
            }
            return new List<CollisionRenderer>();
        }*/

        public void Draw(int lod = 0, bool motionBlur = false, bool forceNoBackfaceCulling = false, bool isSkyboxLol = false)
        {
            if (RenderMesh == null && Resource.IsLoaded)
            {
                CreateSubmeshes();
            }
        }

        public void Dispose()
        {
            if (RenderMesh != null)
            {
                RenderMesh.Dispose();
                RenderMesh = null;
            }
        }

        public void SubmitRenderObjects(Renderer.RenderQueue queue)
        {
            if (RenderMesh != null && Created)
            {
                queue.Add(RenderMesh, new RenderKey(0));
            }
            if (DebugBoundingBox != null)
            {
                queue.Add(DebugBoundingBox, new RenderKey(0));
            }
        }

        public BoundingBox GetBounds()
        {
            return BoundingBox.Transform(Bounds, WorldMatrix);
        }

        public bool RayCast(Ray ray, out float dist)
        {
            var res = Resource.Get();
            if (res != null)
            {
                return res.RayCast(ray, WorldMatrix, out dist);
            }
            dist = float.MaxValue;
            return false;
        }

        public void RegisterWithScene(RenderScene scene)
        {
            if (RenderScene == scene && Registered)
            {
                return;
            }
            else if (RenderScene != scene && Registered)
            {
                UnregisterWithScene();
                RenderScene = scene;
            }
            if (!Resource.IsLoaded)
            {
                return;
            }
            RenderScene.AddObject(this);
            RenderScene.AddOctreeCullable(this);
            Registered = true;
        }

        public void UnregisterWithScene()
        {
            if (Registered)
            {
                RenderScene.RemoveObject(this);
                Registered = false;
            }
        }

        public void UnregisterAndRelease()
        {
            if (Registered)
            {
                UnregisterWithScene();
            }
            if (Resource != null)
            {
                Resource.Release();
            }
            Resource = null;
            Created = false;
            RenderMesh = null;
        }
    }
}
