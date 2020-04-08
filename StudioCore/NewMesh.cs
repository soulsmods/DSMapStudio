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

namespace StudioCore
{
    public class NewMesh : Scene.IDrawable, Resource.IResourceEventListener, IDisposable
    {
        public List<FlverSubmeshRenderer> Submeshes = new List<FlverSubmeshRenderer>();

        private Resource.ResourceHandle<Resource.FlverResource> Resource;
        private bool Created = false;

        private object _lock_submeshes = new object();

        public bool AutoRegister { get; set; } = true;
        private Scene.RenderScene RenderScene;
        private bool Registered = false;
        private DebugPrimitives.DbgPrimWireBox DebugBoundingBox = null;

        public WeakReference<ISelectable> Selectable { get; set; }

        public bool TextureReloadQueued = false;

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
        public RenderFilter DrawFilter { get; set; } = RenderFilter.MapPiece;

        public DrawGroup DrawGroups { get; set; } = new DrawGroup();

        public bool IsVisible { get; set; }

        public NewMesh(Scene.RenderScene scene, Resource.ResourceHandle<Resource.FlverResource> res, bool useSecondUV, Dictionary<string, int> boneIndexRemap = null,
            bool ignoreStaticTransforms = false)
        {
            RenderScene = scene;
            Resource = res;
            Resource.Acquire();
            res.AddResourceEventListener(this);
        }

        public NewMesh(NewMesh mesh)
        {
            RenderScene = mesh.RenderScene;
            DrawFilter = mesh.DrawFilter;
            Resource = mesh.Resource;
            Resource.Acquire();
            Resource.AddResourceEventListener(this);
        }

        ~NewMesh()
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
                    if (Submeshes != null && Resource.TryLock())
                    {
                        foreach (var sm in Submeshes)
                        {
                            sm.CreateDeviceObjects(d, cl, null);
                        }
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
                Submeshes = null;
            }
        }

        private void OnWorldMatrixChanged()
        {
            var res = Resource.Get();
            if (res != null)
            {
                foreach (var sm in Submeshes)
                {
                    sm.WorldTransform = _WorldMatrix;
                }
            }
            if (DebugBoundingBox != null)
            {
                DebugBoundingBox.Transform = new Transform(_WorldMatrix);
            }
            RenderScene.ObjectMoved(this);
        }

        private void CreateSubmeshes()
        {
            lock (_lock_submeshes)
            {
                Submeshes = new List<FlverSubmeshRenderer>();
            }
            var res = Resource.Get();
            Bounds = res.Bounds;
            if (res.GPUMeshes != null)
            {
                for (int i = 0; i < res.GPUMeshes.Length; i++)
                {
                    lock (_lock_submeshes)
                    {
                        var sm = new FlverSubmeshRenderer(this, Resource, i, false);
                        Submeshes.Add(sm);
                    }
                }
            }
        }

        public List<FlverSubmeshRenderer> GetLoadedSubmeshes()
        {
            if (Submeshes.Count() == 0 && Resource.IsLoaded && Resource.Get().GPUMeshes != null)
            {
                CreateSubmeshes();
                return Submeshes;
            }
            return new List<FlverSubmeshRenderer>();
        }

        public void Dispose()
        {
            if (Submeshes != null)
            {
                for (int i = 0; i < Submeshes.Count; i++)
                {
                    if (Submeshes[i] != null)
                        Submeshes[i].Dispose();
                }

                Submeshes = null;
            }
        }

        public void SubmitRenderObjects(Renderer.RenderQueue queue)
        {
            if (Submeshes != null && Created)
            {
                foreach (var sm in Submeshes)
                {
                    queue.Add(sm, sm.GetRenderKey(Vector3.Distance(queue.Pipeline.Eye, sm.Center)));
                }
                if (DebugBoundingBox != null)
                {
                    DebugBoundingBox.SubmitRenderObjects(queue);
                }
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
                return res.RayCast(ray, WorldMatrix, Utils.RayCastCull.CullBack, out dist);
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
            Submeshes = null;
        }
    }
}
