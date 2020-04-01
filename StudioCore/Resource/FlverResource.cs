using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using Veldrid;
using Veldrid.Utilities;
using SoulsFormats;

namespace StudioCore.Resource
{
    public class FlverResource : IResource, IDisposable
    {
        private static Stack<FlverCache> FlverCaches = new Stack<FlverCache>();
        public static int CacheCount { get; private set; } = 0;
        public static long CacheFootprint
        {
            get
            {
                long total = 0;
                lock (CacheLock)
                {
                    foreach (var c in FlverCaches)
                    {
                        total += c.MemoryUsage;
                    }
                }
                return total;
            }
        }
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
                CacheCount++;
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

        public static void PurgeCaches()
        {
            FlverCaches.Clear();
            VerticesPool = ArrayPool<MapFlverLayout>.Create();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public class FlverMaterial : IResourceEventListener
        {
            public Scene.GPUBufferAllocator.GPUBufferHandle MaterialBuffer;
            public Scene.Material MaterialData;

            public TextureResourceHande AlbedoTextureResource = null;
            public TextureResourceHande NormalTextureResource = null;
            public TextureResourceHande SpecularTextureResource = null;
            public TextureResourceHande ShininessTextureResource = null;

            private void SetMaterialTexture(TextureResourceHande handle, ref ushort matTex, ushort defaultTex)
            {
                if (handle != null && handle.IsLoaded && handle.TryLock())
                {
                    var res = handle.Get();
                    if (res != null && res.GPUTexture != null)
                    {
                        matTex = (ushort)handle.Get().GPUTexture.TexHandle;
                    }
                    else
                    {
                        matTex = defaultTex;
                    }
                    handle.Unlock();
                }
                else
                {
                    matTex = defaultTex;
                }
            }

            public void UpdateMaterial()
            {
                SetMaterialTexture(AlbedoTextureResource, ref MaterialData.colorTex, 0);
                SetMaterialTexture(NormalTextureResource, ref MaterialData.normalTex, 1);
                SetMaterialTexture(SpecularTextureResource, ref MaterialData.specTex, 2);
                SetMaterialTexture(ShininessTextureResource, ref MaterialData.shininessTex, 2);

                Scene.Renderer.AddBackgroundUploadTask((d, cl) =>
                {
                    MaterialBuffer.FillBuffer(cl, ref MaterialData);
                });
            }

            public void OnResourceLoaded(IResourceHandle handle)
            {
                UpdateMaterial();
            }

            public void OnResourceUnloaded(IResourceHandle handle)
            {
                UpdateMaterial();
            }
        }

        public unsafe class FlverSubmesh
        {
            public struct FlverSubmeshFaceSet
            {
                public int IndexCount;
                public int IndexOffset;
                public int PickingIndicesCount;
                public IntPtr PickingIndices;
                public bool BackfaceCulling;
                public bool IsTriangleStrip;
                public byte LOD;
                public bool IsMotionBlur;
                public bool Is32Bit;
            }

            public List<FlverSubmeshFaceSet> MeshFacesets { get; set; } = new List<FlverSubmeshFaceSet>();

            public Scene.VertexIndexBufferAllocator.VertexIndexBufferHandle GeomBuffer { get; set; }

            public int VertexCount { get; set; }
            // This is native because using managed arrays causes a weird memory leak
            public IntPtr PickingVertices = IntPtr.Zero;

            //public Vector3[] PickingVertices;
            public BoundingBox Bounds { get; set; }

            public int DefaultBoneIndex { get; set; } = -1;

            public FlverMaterial Material { get; set; } = null;
        }

        /// <summary>
        /// Low level access to the flver struct. Use only in modification mode.
        /// </summary>
        public FLVER2 Flver = null;

        public FlverSubmesh[] GPUMeshes = null;
        public FlverMaterial[] GPUMaterials = null;

        public BoundingBox Bounds { get; set; }

        public List<FLVER.Bone> Bones { get; private set; } = null;

        unsafe private void ProcessMaterial(FLVER2.Material mat, FlverMaterial dest)
        {
            dest.MaterialBuffer = Scene.Renderer.MaterialBufferAllocator.Allocate((uint)sizeof(Scene.Material), sizeof(Scene.Material));
            dest.MaterialData = new Scene.Material();

            foreach (var matparam in mat.Textures)
            {
                var paramNameCheck = matparam.Type.ToUpper();
                if (paramNameCheck == "G_DIFFUSETEXTURE" || paramNameCheck == "G_DIFFUSE" || paramNameCheck.Contains("ALBEDO"))
                {
                    if (matparam.Path == "")
                    {
                        // TODO Sekiro handling
                    }
                    else
                    {
                        dest.AlbedoTextureResource = ResourceManager.GetTextureResource($@"tex/{Path.GetFileNameWithoutExtension(matparam.Path)}");
                        dest.AlbedoTextureResource.Acquire();
                        dest.AlbedoTextureResource.AddResourceEventListener(dest);
                    }
                }
                else if (paramNameCheck == "G_BUMPMAPTEXTURE" || paramNameCheck == "G_BUMPMAP" || paramNameCheck.Contains("NORMAL"))
                {
                    if (matparam.Path == "")
                    {
                        // TODO Sekiro handling
                    }
                    else
                    {
                        dest.NormalTextureResource = ResourceManager.GetTextureResource($@"tex/{Path.GetFileNameWithoutExtension(matparam.Path)}");
                        dest.NormalTextureResource.Acquire();
                        dest.NormalTextureResource.AddResourceEventListener(dest);
                    }
                }
                else if (paramNameCheck == "G_SPECULARTEXTURE" || paramNameCheck == "G_SPECULAR" || paramNameCheck.Contains("SPECULAR"))
                {
                    if (matparam.Path == "")
                    {
                        // TODO Sekiro handling
                    }
                    else
                    {
                        dest.SpecularTextureResource = ResourceManager.GetTextureResource($@"tex/{Path.GetFileNameWithoutExtension(matparam.Path)}");
                        dest.SpecularTextureResource.Acquire();
                        dest.SpecularTextureResource.AddResourceEventListener(dest);
                    }
                }
                else if (paramNameCheck.Contains("SHININESS"))
                {
                    if (matparam.Path == "")
                    {
                        // TODO Sekiro handling
                    }
                    else
                    {
                        dest.ShininessTextureResource = ResourceManager.GetTextureResource($@"tex/{Path.GetFileNameWithoutExtension(matparam.Path)}");
                        dest.ShininessTextureResource.Acquire();
                        dest.ShininessTextureResource.AddResourceEventListener(dest);
                    }
                }
            }

            dest.UpdateMaterial();
        }

        unsafe private void ProcessMesh(FLVER2.Mesh mesh, FlverSubmesh dest)
        {
            bool hasLightmap = false;
            bool useSecondUV = false;

            var factory = Scene.Renderer.Factory;

            //var MeshVertices = new MapFlverLayout[mesh.VertexCount];
            var MeshVertices = VerticesPool.Rent(mesh.VertexCount);
            //dest.PickingVertices = new Vector3[mesh.VertexCount];
            dest.PickingVertices = Marshal.AllocHGlobal(mesh.VertexCount * sizeof(Vector3));
            var pvhandle = new Span<Vector3>(dest.PickingVertices.ToPointer(), mesh.VertexCount);
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var vert = mesh.Vertices[i];

                var ORIG_BONE_WEIGHTS = vert.BoneWeights;
                var ORIG_BONE_INDICES = vert.BoneIndices;

                MeshVertices[i] = new MapFlverLayout();

                MeshVertices[i].Position = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);
                pvhandle[i] = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);

                var n = Vector3.Normalize(new Vector3(vert.Normal.X, vert.Normal.Y, vert.Normal.Z));
                MeshVertices[i].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i].Normal[2] = (sbyte)(n.Z * 127.0f);

                //var bt = Vector3.Normalize(new Vector3(vert.Bitangent.X, vert.Bitangent.Y, vert.Bitangent.Z));

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

                if (vert.TangentCount > 0)
                {
                    var tan = vert.GetTangent(0);
                    var t = Vector3.Normalize(new Vector3(tan.X, tan.Y, tan.Z));
                    MeshVertices[i].Bitangent[0] = (sbyte)(t.X * 127.0f);
                    MeshVertices[i].Bitangent[1] = (sbyte)(t.Y * 127.0f);
                    MeshVertices[i].Bitangent[2] = (sbyte)(t.Z * 127.0f);
                    MeshVertices[i].Bitangent[3] = (sbyte)(tan.W * 127.0f);

                    var bn = Vector3.Cross(n, Vector3.Normalize(new Vector3(t.X, t.Y, t.Z))) * tan.W;
                    MeshVertices[i].Binormal[0] = (sbyte)(bn.X * 127.0f);
                    MeshVertices[i].Binormal[1] = (sbyte)(bn.Y * 127.0f);
                    MeshVertices[i].Binormal[2] = (sbyte)(bn.Z * 127.0f);
                }
                else
                {
                    MeshVertices[i].Bitangent[0] = 0;
                    MeshVertices[i].Bitangent[1] = 0;
                    MeshVertices[i].Bitangent[2] = 0;
                    MeshVertices[i].Bitangent[3] = 127;
                }
            }

            //debug_sortedByZ = debug_sortedByZ.OrderBy(v => v.Position.Z).ToList();

            dest.VertexCount = mesh.VertexCount;

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
                var indices = faceset.TriangleStrip ? faceset.Triangulate(true).ToArray() : faceset.Indices.ToArray();
                var newFaceSet = new FlverSubmesh.FlverSubmeshFaceSet()
                {
                    BackfaceCulling = faceset.CullBackfaces,
                    IsTriangleStrip = faceset.TriangleStrip,
                    //IndexBuffer = factory.CreateBuffer(new BufferDescription(buffersize, BufferUsage.IndexBuffer)),
                    IndexOffset = idxoffset,

                    IndexCount = faceset.IndicesCount,
                    Is32Bit = is32bit,
                    PickingIndicesCount = indices.Length,
                    PickingIndices = Marshal.AllocHGlobal(indices.Length * 4),
                };
                fixed (void* iptr = indices)
                {
                    Unsafe.CopyBlock(newFaceSet.PickingIndices.ToPointer(), iptr, (uint)indices.Length * 4);
                }
                

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
                idxoffset += faceset.Indices.Length;
            }

            //dest.Bounds = BoundingBox.CreateFromPoints(MeshVertices.Select(x => x.Position));
            //dest.Bounds = new BoundingBox(mesh.BoundingBox.Min, mesh.BoundingBox.Max);
            dest.Bounds = BoundingBox.CreateFromPoints((Vector3*)dest.PickingVertices.ToPointer(), dest.VertexCount, 12, Quaternion.Identity, Vector3.Zero, Vector3.One);
            //dest.Bounds = new BoundingBox();

            //dest.VertBuffer = new VertexBuffer(GFX.Device,
            //    typeof(FlverShaderVertInput), MeshVertices.Length, BufferUsage.WriteOnly);
            //dest.VertBuffer.SetData(MeshVertices);

            uint vbuffersize = (uint)mesh.VertexCount * MapFlverLayout.SizeInBytes;
            //dest.VertBuffer = factory.CreateBuffer(new BufferDescription(vbuffersize, BufferUsage.VertexBuffer));
            //dest.VertBuffer = Scene.Renderer.VertexBufferAllocator.Allocate(vbuffersize, (int)MapFlverLayout.SizeInBytes);
            dest.GeomBuffer = Scene.Renderer.GeometryBufferAllocator.Allocate(vbuffersize, (uint)indicesTotal * (is32bit ? 4u : 2u), (int)MapFlverLayout.SizeInBytes, 4, (h) =>
            {
                h.FillVBuffer(MeshVertices, mesh.VertexCount, () =>
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

            dest.Material = GPUMaterials[mesh.MaterialIndex];
        }

        private bool LoadInternal(AccessLevel al)
        {
            if (al == AccessLevel.AccessFull || al == AccessLevel.AccessGPUOptimizedOnly)
            {
                GPUMeshes = new FlverSubmesh[Flver.Meshes.Count()];
                GPUMaterials = new FlverMaterial[Flver.Materials.Count()];
                Bounds = new BoundingBox();

                for (int i = 0; i < Flver.Materials.Count(); i++)
                {
                    GPUMaterials[i] = new FlverMaterial();
                    ProcessMaterial(Flver.Materials[i], GPUMaterials[i]);
                }

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
            //return false;
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

        public unsafe bool RayCast(Ray ray, Matrix4x4 transform, Utils.RayCastCull cull, out float dist)
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
                if (Utils.RayMeshIntersection(tray, new Span<Vector3>(mesh.PickingVertices.ToPointer(), mesh.VertexCount),
                    new Span<int>(fc.PickingIndices.ToPointer(), fc.PickingIndicesCount), cull, out locdist))
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
                        m.GeomBuffer.Dispose();
                        Marshal.FreeHGlobal(m.PickingVertices);
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
