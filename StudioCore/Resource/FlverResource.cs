using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Buffers;
//using Microsoft.Xna.Framework.Graphics;
using Veldrid;
using Veldrid.Utilities;
using SoulsFormats;

namespace StudioCore.Resource
{
    public class FlverResource : IResource, IDisposable
    {
        private static Stack<FlverCache> FlverCaches = new Stack<FlverCache>();
        private static object CacheLock = new object();

        private static ArrayPool<MapFlverLayout> VerticesPool = ArrayPool<MapFlverLayout>.Create();

        private FlverCache GetCache()
        {
            lock (CacheLock)
            {
                if (FlverCaches.Count > 0)
                {
                    return FlverCaches.Pop();
                }
            }
            return new FlverCache();
        }

        private void ReleaseCache(FlverCache cache)
        {
            if (cache != null)
            {
                cache.ResetUsage();
                lock (CacheLock)
                {
                    FlverCaches.Push(cache);
                }
            }
        }

        public class FlverSubmesh
        {
            public struct FlverSubmeshFaceSet
            {
                public int IndexCount;
                //public IndexBuffer IndexBuffer;
                //public DeviceBuffer IndexBuffer;
                //public Scene.GPUBufferAllocator.GPUBufferHandle IndexBuffer;
                public int IndexOffset;
                public int[] PickingIndices;
                public bool BackfaceCulling;
                public bool IsTriangleStrip;
                public byte LOD;
                public bool IsMotionBlur;
                public bool Is32Bit;
            }

            public List<FlverSubmeshFaceSet> MeshFacesets { get; set; } = new List<FlverSubmeshFaceSet>();

            public Scene.VertexIndexBufferAllocator.VertexIndexBufferHandle GeomBuffer { get; set; }

            public int VertexCount { get; set; }

            public Vector3[] PickingVertices;
            public BoundingBox Bounds { get; set; }

            public int DefaultBoneIndex { get; set; } = -1;
        }

        /// <summary>
        /// Low level access to the flver struct. Use only in modification mode.
        /// </summary>
        public FLVER2 Flver = null;

        public FlverSubmesh[] GPUMeshes = null;

        public BoundingBox Bounds { get; set; }

        public List<FLVER.Bone> Bones { get; private set; } = null;

        unsafe private void ProcessMesh(FLVER2.Mesh mesh, FlverSubmesh dest)
        {
            bool hasLightmap = false;
            bool useSecondUV = false;

            var factory = Scene.Renderer.Factory;

            //var MeshVertices = new MapFlverLayout[mesh.VertexCount];
            var MeshVertices = VerticesPool.Rent(mesh.VertexCount);
            dest.PickingVertices = new Vector3[mesh.VertexCount];
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var vert = mesh.Vertices[i];

                var ORIG_BONE_WEIGHTS = vert.BoneWeights;
                var ORIG_BONE_INDICES = vert.BoneIndices;

                MeshVertices[i] = new MapFlverLayout();

                MeshVertices[i].Position = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);
                dest.PickingVertices[i] = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);

                var n = Vector3.Normalize(new Vector3(vert.Normal.X, vert.Normal.Y, vert.Normal.Z));
                MeshVertices[i].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i].Normal[2] = (sbyte)(n.Z * 127.0f);


                if (vert.UVCount > 0)
                {
                    if (useSecondUV && vert.UVCount > 1)
                    {
                        //MeshVertices[i].TextureCoordinate = new Vector2(vert.UVs[1].X, vert.UVs[1].Y);
                        var uv = vert.GetUV(1);
                        MeshVertices[i].Uv1[0] = (short)(uv.X * 2048.0f);
                        MeshVertices[i].Uv1[1] = (short)(uv.Y * 2048.0f);
                    }
                    else
                    {
                        //MeshVertices[i].TextureCoordinate = new Vector2(vert.UVs[0].X, vert.UVs[0].Y);
                        var uv = vert.GetUV(0);
                        MeshVertices[i].Uv1[0] = (short)(uv.X * 2048.0f);
                        MeshVertices[i].Uv1[1] = (short)(uv.Y * 2048.0f);
                    }

                    if (vert.UVCount >= 2)
                    {
                        //MeshVertices[i].TextureCoordinate2 = new Vector2(vert.UVs[1].X, vert.UVs[1].Y);
                    }
                }
                else
                {
                    //MeshVertices[i].TextureCoordinate = Vector2.Zero;
                    //MeshVertices[i].TextureCoordinate2 = Vector2.Zero;
                    MeshVertices[i].Uv1[0] = 0;
                    MeshVertices[i].Uv1[1] = 0;
                }
            }

            //debug_sortedByZ = debug_sortedByZ.OrderBy(v => v.Position.Z).ToList();

            dest.VertexCount = MeshVertices.Length;

            dest.MeshFacesets = new List<FlverSubmesh.FlverSubmeshFaceSet>();
            var facesets = mesh.FaceSets;
            var fsUploadsPending = facesets.Count();

            bool is32bit = Flver.Header.Version > 0x20005 && mesh.VertexCount > 65535;
            int indicesTotal = 0;
            ushort[] fs16 = null;
            int[] fs32 = null;
            foreach (var faceset in facesets)
            {
                indicesTotal += faceset.Indices.Length;
            }
            if (is32bit)
            {
                fs32 = new int[indicesTotal];
            }
            else
            {
                fs16 = new ushort[indicesTotal];
            }

            int idxoffset = 0;
            foreach (var faceset in facesets)
            {
                if (faceset.Indices.Length == 0)
                    continue;

                //At this point they use 32-bit faceset vertex indices

                uint buffersize = (uint)faceset.IndicesCount * (is32bit ? 4u : 2u);
                var newFaceSet = new FlverSubmesh.FlverSubmeshFaceSet()
                {
                    BackfaceCulling = faceset.CullBackfaces,
                    IsTriangleStrip = faceset.TriangleStrip,
                    //IndexBuffer = factory.CreateBuffer(new BufferDescription(buffersize, BufferUsage.IndexBuffer)),
                    IndexOffset = idxoffset,

                    IndexCount = faceset.IndicesCount,
                    Is32Bit = is32bit,
                    PickingIndices = faceset.TriangleStrip ? faceset.Triangulate(true).ToArray() : faceset.Indices.ToArray(),
                };
                

                if ((faceset.Flags & FLVER2.FaceSet.FSFlags.LodLevel1) > 0)
                {
                    newFaceSet.LOD = 1;
                    //HasNoLODs = false;
                    newFaceSet.IsMotionBlur = false;
                }
                else if ((faceset.Flags & FLVER2.FaceSet.FSFlags.LodLevel2) > 0)
                {
                    newFaceSet.LOD = 2;
                    //HasNoLODs = false;
                    newFaceSet.IsMotionBlur = false;
                }

                if ((faceset.Flags & FLVER2.FaceSet.FSFlags.MotionBlur) > 0)
                {
                    newFaceSet.IsMotionBlur = true;
                }

                if (is32bit)
                {
                    /*newFaceSet.IndexBuffer.FillBuffer(
                        faceset.Indices.Select(x => (x == 0xFFFF && x > mesh.Vertices.Length) ? -1 : x).Take(faceset.IndicesCount).ToArray(),
                        () =>
                        {
                            fsUploadsPending--;
                            if (fsUploadsPending <= 0)
                            {
                                facesets = null;
                            }
                        }
                    );*/
                    for (int i = 0; i < faceset.Indices.Length; i++)
                    {
                        if (faceset.Indices[i] == 0xFFFF && faceset.Indices[i] > mesh.Vertices.Length)
                        {
                            fs32[newFaceSet.IndexOffset + i] = -1;
                        }
                        else
                        {
                            fs32[newFaceSet.IndexOffset + i] = faceset.Indices[i];
                        }
                    }
                }
                else
                {
                    /*newFaceSet.IndexBuffer.FillBuffer(
                        faceset.Indices.Select<int, ushort>(x => (ushort)((x == 0xFFFF && x > mesh.Vertices.Length) ? 0xFFFF : (ushort)x)).Take(faceset.IndicesCount).ToArray(),
                        () =>
                        {
                            fsUploadsPending--;
                            if (fsUploadsPending <= 0)
                            {
                                facesets = null;
                            }
                        }
                    );*/
                    for (int i = 0; i < faceset.Indices.Length; i++)
                    {
                        if (faceset.Indices[i] == 0xFFFF && faceset.Indices[i] > mesh.Vertices.Length)
                        {
                            fs16[newFaceSet.IndexOffset + i] = 0xFFFF;
                        }
                        else
                        {
                            fs16[newFaceSet.IndexOffset + i] = (ushort)faceset.Indices[i];
                        }
                    }
                }

                dest.MeshFacesets.Add(newFaceSet);

            }

            //dest.Bounds = BoundingBox.CreateFromPoints(MeshVertices.Select(x => x.Position));
            //dest.Bounds = new BoundingBox(mesh.BoundingBox.Min, mesh.BoundingBox.Max);
            fixed (void* ptr = dest.PickingVertices)
            {
                dest.Bounds = BoundingBox.CreateFromPoints((Vector3*)ptr, dest.PickingVertices.Count(), 12, Quaternion.Identity, Vector3.Zero, Vector3.One);
            }

            //dest.VertBuffer = new VertexBuffer(GFX.Device,
            //    typeof(FlverShaderVertInput), MeshVertices.Length, BufferUsage.WriteOnly);
            //dest.VertBuffer.SetData(MeshVertices);

            uint vbuffersize = (uint)mesh.VertexCount * MapFlverLayout.SizeInBytes;
            //dest.VertBuffer = factory.CreateBuffer(new BufferDescription(vbuffersize, BufferUsage.VertexBuffer));
            //dest.VertBuffer = Scene.Renderer.VertexBufferAllocator.Allocate(vbuffersize, (int)MapFlverLayout.SizeInBytes);
            dest.GeomBuffer = Scene.Renderer.GeometryBufferAllocator.Allocate(vbuffersize, (uint)indicesTotal * (is32bit ? 4u : 2u), (int)MapFlverLayout.SizeInBytes, 4, (h) =>
            {
                h.FillVBuffer(MeshVertices, () =>
                {
                    VerticesPool.Return(MeshVertices);
                    MeshVertices = null;
                });
                if (is32bit)
                {
                    h.FillIBuffer(fs32);
                }
                else
                {
                    h.FillIBuffer(fs16);
                }
            });
            facesets = null;
        }

        private bool LoadInternal(AccessLevel al)
        {
            if (al == AccessLevel.AccessFull || al == AccessLevel.AccessGPUOptimizedOnly)
            {
                GPUMeshes = new FlverSubmesh[Flver.Meshes.Count()];
                Bounds = new BoundingBox();

                for (int i = 0; i < Flver.Meshes.Count(); i++)
                {
                    GPUMeshes[i] = new FlverSubmesh();
                    ProcessMesh(Flver.Meshes[i], GPUMeshes[i]);
                    if (i == 0)
                    {
                        Bounds = GPUMeshes[i].Bounds;
                    }
                    else
                    {
                        Bounds = BoundingBox.Combine(Bounds, GPUMeshes[i].Bounds);
                    }
                    //Bounds = BoundingBox.CreateMerged(Bounds, GPUMeshes[i].Bounds);
                }

                Bones = Flver.Bones;
            }

            if (al == AccessLevel.AccessGPUOptimizedOnly)
            {
                Flver = null;
            }
            return true;
        }

        bool IResource._Load(byte[] bytes, AccessLevel al, GameType type)
        {
            var cache = (al == AccessLevel.AccessGPUOptimizedOnly) ? GetCache() : null;
            Flver = FLVER2.Read(bytes, cache);
            bool ret = LoadInternal(al);
            ReleaseCache(cache);
            return ret;
        }

        bool IResource._Load(string file, AccessLevel al, GameType type)
        {
            var cache = (al == AccessLevel.AccessGPUOptimizedOnly) ? GetCache() : null;
            Flver = FLVER2.Read(file, cache);
            bool ret = LoadInternal(al);
            ReleaseCache(cache);
            return ret;
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
                var fc = mesh.MeshFacesets[0];
                if (Utils.RayMeshIntersection(tray, mesh.PickingVertices, fc.PickingIndices, cull, out locdist))
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

                if (GPUMeshes != null)
                {
                    foreach (var m in GPUMeshes)
                    {
                        /*m.VertBuffer.Dispose();
                        foreach (var fs in m.MeshFacesets)
                        {
                            fs.IndexBuffer.Dispose();
                        }*/
                    }
                }

                disposedValue = true;
            }
        }

        ~FlverResource()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
