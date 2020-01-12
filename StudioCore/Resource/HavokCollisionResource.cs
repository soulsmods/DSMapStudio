using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using SoulsFormats;

namespace StudioCore.Resource
{
    public class HavokCollisionResource : IResource, IDisposable
    {
        public class CollisionSubmesh
        {
            public int IndexCount;
            public DeviceBuffer IndexBuffer;
            public int[] PickingIndices;

            public DeviceBuffer VertBuffer { get; set; }

            public int VertexCount { get; set; }

            public Vector3[] PickingVertices;
            public BoundingBox Bounds { get; set; }
        }
        public HKX Hkx = null;

        public CollisionSubmesh[] GPUMeshes = null;

        public BoundingBox Bounds { get; set; }

        unsafe private void ProcessMesh(HKX.HKPStorageExtendedMeshShapeMeshSubpartStorage mesh, CollisionSubmesh dest)
        {
            var verts = mesh.Vertices.GetArrayData().Elements;
            var indices = mesh.Indices16.GetArrayData().Elements;
            var MeshIndices = new int[(indices.Count / 4) * 3];
            var MeshVertices = new CollisionLayout[(indices.Count / 4) * 3];
            dest.PickingVertices = new Vector3[(indices.Count / 4) * 3];
            dest.PickingIndices = new int[(indices.Count / 4) * 3];

            var factory = Scene.Renderer.Factory;

            for (int id = 0; id < indices.Count; id += 4)
            {
                int i = (id / 4) * 3;
                var vert1 = mesh.Vertices[mesh.Indices16[id].data].Vector;
                var vert2 = mesh.Vertices[mesh.Indices16[id+1].data].Vector;
                var vert3 = mesh.Vertices[mesh.Indices16[id+2].data].Vector;

                MeshVertices[i] = new CollisionLayout();
                MeshVertices[i+1] = new CollisionLayout();
                MeshVertices[i+2] = new CollisionLayout();

                MeshVertices[i].Position = new Vector3(vert1.X, vert1.Y, vert1.Z);
                MeshVertices[i+1].Position = new Vector3(vert2.X, vert2.Y, vert2.Z);
                MeshVertices[i+2].Position = new Vector3(vert3.X, vert3.Y, vert3.Z);
                dest.PickingVertices[i] = new Vector3(vert1.X, vert1.Y, vert1.Z);
                dest.PickingVertices[i+1] = new Vector3(vert2.X, vert2.Y, vert2.Z);
                dest.PickingVertices[i+2] = new Vector3(vert3.X, vert3.Y, vert3.Z);
                var n = Vector3.Normalize(Vector3.Cross(MeshVertices[i + 2].Position - MeshVertices[i].Position, MeshVertices[i + 1].Position - MeshVertices[i].Position));
                MeshVertices[i].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i].Normal[2] = (sbyte)(n.Z * 127.0f);
                MeshVertices[i+1].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i+1].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i+1].Normal[2] = (sbyte)(n.Z * 127.0f);
                MeshVertices[i+2].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i+2].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i+2].Normal[2] = (sbyte)(n.Z * 127.0f);

                MeshVertices[i].Color[0] = (byte)(53);
                MeshVertices[i].Color[1] = (byte)(157);
                MeshVertices[i].Color[2] = (byte)(255);
                MeshVertices[i].Color[3] = (byte)(255);
                MeshVertices[i+1].Color[0] = (byte)(53);
                MeshVertices[i+1].Color[1] = (byte)(157);
                MeshVertices[i+1].Color[2] = (byte)(255);
                MeshVertices[i+1].Color[3] = (byte)(255);
                MeshVertices[i+2].Color[0] = (byte)(53);
                MeshVertices[i+2].Color[1] = (byte)(157);
                MeshVertices[i+2].Color[2] = (byte)(255);
                MeshVertices[i+2].Color[3] = (byte)(255);

                MeshIndices[i] = i;
                MeshIndices[i + 1] = i + 1;
                MeshIndices[i + 2] = i + 2;
                dest.PickingIndices[i] = i;
                dest.PickingIndices[i + 1] = i + 1;
                dest.PickingIndices[i + 2] = i + 2;
            }

            dest.VertexCount = MeshVertices.Length;
            dest.IndexCount = MeshIndices.Length;

            uint buffersize = (uint)dest.IndexCount * 4u;
            dest.IndexBuffer = factory.CreateBuffer(new BufferDescription(buffersize, BufferUsage.IndexBuffer));
            Scene.Renderer.AddBackgroundUploadTask((device, cl) =>
            {
                cl.UpdateBuffer(dest.IndexBuffer, 0, MeshIndices);
            });

            fixed (void* ptr = dest.PickingVertices)
            {
                dest.Bounds = BoundingBox.CreateFromPoints((Vector3*)ptr, dest.PickingVertices.Count(), 12, Quaternion.Identity, Vector3.Zero, Vector3.One);
            }

            uint vbuffersize = (uint)MeshVertices.Length * CollisionLayout.SizeInBytes;
            dest.VertBuffer = factory.CreateBuffer(new BufferDescription(vbuffersize, BufferUsage.VertexBuffer));

            Scene.Renderer.AddBackgroundUploadTask((d, cl) =>
            {
                cl.UpdateBuffer(dest.VertBuffer, 0, MeshVertices);
                MeshVertices = null;
            });
        }

        private bool LoadInternal(AccessLevel al)
        {
            if (al == AccessLevel.AccessFull || al == AccessLevel.AccessGPUOptimizedOnly)
            {
                Bounds = new BoundingBox();
                var submeshes = new List<CollisionSubmesh>();
                bool first = true;
                foreach (var obj in Hkx.DataSection.Objects)
                {
                    if (obj is HKX.HKPStorageExtendedMeshShapeMeshSubpartStorage col)
                    {
                        var mesh = new CollisionSubmesh();
                        ProcessMesh(col, mesh);
                        if (first)
                        {
                            Bounds = mesh.Bounds;
                            first = false;
                        }
                        else
                        {
                            Bounds = BoundingBox.Combine(Bounds, mesh.Bounds);
                        }
                        submeshes.Add(mesh);
                    }
                    //Bounds = BoundingBox.CreateMerged(Bounds, GPUMeshes[i].Bounds);
                }
                GPUMeshes = submeshes.ToArray();
            }

            if (al == AccessLevel.AccessGPUOptimizedOnly)
            {
                Hkx = null;
            }
            return true;
        }

        bool IResource._Load(byte[] bytes, AccessLevel al)
        {
            Hkx = HKX.Read(bytes);
            return LoadInternal(al);
        }

        bool IResource._Load(string file, AccessLevel al)
        {
            Hkx = HKX.Read(file);
            return LoadInternal(al);
        }

        public bool RayCast(Ray ray, Matrix4x4 transform, Utils.RayCastCull cull, out float dist)
        {
            bool hit = false;
            float mindist = float.MaxValue;
            var invw = transform.Inverse();
            var newo = Vector3.Transform(ray.Origin, invw);
            var newd = Vector3.TransformNormal(ray.Direction, invw);
            var tray = new Ray(newo, newd);
            foreach (var mesh in GPUMeshes)
            {
                if (!tray.Intersects(mesh.Bounds))
                {
                    continue;
                }
                float locdist;
                if (Utils.RayMeshIntersection(tray, mesh.PickingVertices, mesh.PickingIndices, cull, out locdist))
                {
                    hit = true;
                    if (locdist < mindist)
                    {
                        mindist = locdist;
                    }
                }
            }
            dist = mindist;
            return hit;
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
        // ~HavokCollisionResource()
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
