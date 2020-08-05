using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Veldrid.Utilities;

namespace StudioCore.Scene
{
    /// <summary>
    /// A simple mesh that will render a drawable and doesn't depend on a resource
    /// </summary>
    class Mesh : Scene.IDrawable
    {
        public RenderObject RenderMesh;

        private bool Created = false;

        private object _lock_submeshes = new object();

        //public BoundingBox Bounds;

        public bool AutoRegister { get; set; } = true;
        private Scene.RenderScene RenderScene;
        private bool Registered = false;
        private DebugPrimitives.DbgPrimWireBox DebugBoundingBox = null;

        public WeakReference<ISelectable> Selectable { get; set; }

        public RenderFilter DrawFilter { get; set; } = RenderFilter.All;

        private bool _DebugDrawBounds = false;

        public bool IsVisible { get; set; } = true;

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
                        //DebugBoundingBox.EnableDraw = true;
                    }
                }
                else
                {
                    //DebugBoundingBox.EnableDraw = false;
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

        public Mesh(Scene.RenderScene scene, BoundingBox bounds, RenderObject mesh)
        {
            RenderScene = scene;
            RenderMesh = mesh;
            Bounds = bounds;

            OnWorldMatrixChanged();
            Renderer.AddBackgroundUploadTask((d, cl) =>
            {
                if (RenderMesh != null)
                {
                    RenderMesh.CreateDeviceObjects(d, cl, null);
                    if (AutoRegister)
                    {
                        RegisterWithScene(RenderScene);
                    }
                    Created = true;
                }
            });
        }

        public Mesh(Mesh mesh)
        {
            RenderScene = mesh.RenderScene;
        }

        ~Mesh()
        {
            if (Registered)
            {
                UnregisterWithScene();
            }
        }

        private void OnWorldMatrixChanged()
        {
            if (RenderMesh != null)
            {

                //RenderMesh.WorldTransform = _WorldMatrix;

            }
            if (DebugBoundingBox != null)
            {
                //DebugBoundingBox.Transform = new Transform(_WorldMatrix);
            }
        }

        public void Draw(int lod = 0, bool motionBlur = false, bool forceNoBackfaceCulling = false, bool isSkyboxLol = false)
        {
            if (RenderMesh == null)
            {
            }
        }

        public void Dispose()
        {
            if (RenderMesh != null)
            {
                //RenderMesh.Dispose();
                RenderMesh = null;
            }
        }

        public void SubmitRenderObjects(Renderer.RenderQueue queue)
        {
            if (RenderMesh != null && Created)
            {
                //TODO:queue.Add(RenderMesh, new RenderKey(0));
            }
            if (DebugBoundingBox != null)
            {
                //TODO:queue.Add(DebugBoundingBox, new RenderKey(0));
            }
        }

        public BoundingBox GetBounds()
        {
            return BoundingBox.Transform(Bounds, WorldMatrix);
        }

        public bool RayCast(Ray ray, out float dist)
        {
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
            if (RenderMesh == null)
            {
                return;
            }
            Registered = true;
        }

        public void UnregisterWithScene()
        {
            if (Registered)
            {
                Registered = false;
            }
        }

        public void UnregisterAndRelease()
        {
            if (Registered)
            {
                UnregisterWithScene();
            }
            Created = false;
            RenderMesh = null;
        }
    }
}
