using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Utilities;

namespace StudioCore.Scene
{
    public class Region : IDrawable, IDisposable
    {
        public enum RegionShape
        {
            Point,
            Circle,
            Sphere,
            Cylinder,
            Rect,
            Box,
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
        public ISelectable Selectable { get; set; } = null;
        public RenderFilter DrawFilter { get; set; } = RenderFilter.Region;
        public bool Highlighted { get; set; } = false;
        public bool AutoRegister { get; set; } = true;

        private Scene.RenderScene RenderScene;
        private bool Registered = false;

        public BoundingBox Bounds { get; private set; } = new BoundingBox();

        private DebugPrimitives.DbgPrimWire RegionMesh = null;

        private Region(RenderScene scene)
        {
            RenderScene = scene;
        }

        private void OnWorldMatrixChanged()
        {
            if (RegionMesh != null)
            {
                RegionMesh.Transform = new Transform(_WorldMatrix);
            }
            RenderScene.ObjectMoved(this);
        }

        private void RegisterMesh()
        {
            //Bounds = RegionMesh.GetBounds();
            OnWorldMatrixChanged();
            Renderer.AddBackgroundUploadTask((d, cl) =>
            {
                RegionMesh.CreateDeviceObjects(d, cl, null);
                if (AutoRegister)
                {
                    RegisterWithScene(RenderScene);
                }
            });
        }

        public static Region GetBoxRegion(RenderScene scene, float width, float height, float depth)
        {
            var r = new Region(scene);
            Vector3 min = new Vector3(-width / 2.0f, 0.0f, -depth / 2.0f);
            Vector3 max = new Vector3(width / 2.0f, height, depth / 2.0f);
            r.RegionMesh = new DebugPrimitives.DbgPrimWireBox(Transform.Default, min, max, Color.Blue);
            r.Bounds = new BoundingBox(min, max);
            r.RegisterMesh();
            return r;
        }

        public BoundingBox GetBounds()
        {
            return BoundingBox.Transform(Bounds, WorldMatrix);
        }

        public bool RayCast(Ray ray, out float dist)
        {
            if (RegionMesh != null)
            {
                //return RegionMesh.RayCast(ray, WorldMatrix, out dist);
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

        public void SubmitRenderObjects(Renderer.RenderQueue queue)
        {
            if (RegionMesh != null)
            {
                queue.Add(RegionMesh, new RenderKey(0));
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Region()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
