using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
//using Microsoft.Xna.Framework.Graphics;
using Veldrid;
using Veldrid.Utilities;
using SoulsFormats;

namespace StudioCore.Resource
{
    public class FlverResource : IResource, IDisposable
    {
        public class FlverSubmesh
        {
            public struct FlverSubmeshFaceSet
            {
                public int IndexCount;
                //public IndexBuffer IndexBuffer;
                public DeviceBuffer IndexBuffer;
                public int[] PickingIndices;
                public bool BackfaceCulling;
                public bool IsTriangleStrip;
                public byte LOD;
                public bool IsMotionBlur;
                public bool Is32Bit;
            }

            public List<FlverSubmeshFaceSet> MeshFacesets { get; set; } = new List<FlverSubmeshFaceSet>();

            public DeviceBuffer VertBuffer { get; set; }

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
            dest.DefaultBoneIndex = mesh.DefaultBoneIndex;

            bool hasLightmap = false;
            bool useSecondUV = false;

            Dictionary<int, int> finalBoneRemapper = null;

            /*if (boneIndexRemap != null)
            {
                finalBoneRemapper = new Dictionary<int, int>();
                for (int i = 0; i < flvr.Bones.Count; i++)
                {
                    if (boneIndexRemap.ContainsKey(flvr.Bones[i].Name))
                    {
                        finalBoneRemapper.Add(i, boneIndexRemap[flvr.Bones[i].Name]);
                    }
                }
            }*/

            // MTD lookup
            MTD mtd = null; //InterrootLoader.GetMTD(flvr.Materials[mesh.MaterialIndex].MTD);

            //var debug_LowestBoneWeight = float.MaxValue;
            //var debug_HighestBoneWeight = float.MinValue;

            //var debug_sortedByZ = new List<FLVER.Vertex>();

            var factory = Scene.Renderer.Factory;

            Matrix4x4 GetBoneMatrix(SoulsFormats.FLVER.Bone b)
            {
                SoulsFormats.FLVER.Bone parentBone = b;

                var result = Matrix4x4.Identity;

                do
                {
                    result *= Matrix4x4.CreateScale(parentBone.Scale.X, parentBone.Scale.Y, parentBone.Scale.Z);
                    result *= Matrix4x4.CreateRotationX(parentBone.Rotation.X);
                    result *= Matrix4x4.CreateRotationZ(parentBone.Rotation.Z);
                    result *= Matrix4x4.CreateRotationY(parentBone.Rotation.Y);
                    result *= Matrix4x4.CreateTranslation(parentBone.Translation.X, parentBone.Translation.Y, parentBone.Translation.Z);

                    if (parentBone.ParentIndex >= 0)
                        parentBone = Flver.Bones[parentBone.ParentIndex];
                    else
                        parentBone = null;
                }
                while (parentBone != null);

                return result;
            }

            var MeshVertices = new MapFlverLayout[mesh.Vertices.Count];
            dest.PickingVertices = new Vector3[mesh.Vertices.Count];
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {

                var vert = mesh.Vertices[i];

                var ORIG_BONE_WEIGHTS = vert.BoneWeights;
                var ORIG_BONE_INDICES = vert.BoneIndices;

                MeshVertices[i] = new MapFlverLayout();

                if (vert.BoneWeights[0] == 0 && vert.BoneWeights[1] == 0 && vert.BoneWeights[2] == 0 && vert.BoneWeights[3] == 0)
                {
                    vert.BoneWeights[0] = 1;
                }

                // Apply normal W channel bone index (for some weapons etc)
                if (!vert.UsesBoneIndices)
                {
                    int boneIndex = vert.NormalW;

                    //if (boneIndex == 0 && mesh.DefaultBoneIndex != 0)
                    //    boneIndex = mesh.DefaultBoneIndex;

                    vert.BoneIndices[0] = boneIndex;
                    //vert.BoneIndices[1] = 0;
                    //vert.BoneIndices[2] = 0;
                    //vert.BoneIndices[3] = 0;

                    vert.BoneWeights[0] = 1;
                    //vert.BoneWeights[1] = 0;
                    //vert.BoneWeights[2] = 0;
                    //vert.BoneWeights[3] = 0;
                }

                // Apply bind pose of bone to actual vert if !mesh.Dynamic
                if (mesh.Dynamic == 0)
                {
                    //ApplySkin(vert, flvr.Bones.Select(b => GetBoneMatrix(b)).ToList(), mesh.BoneIndices, (flvr.Header.Version <= 0x2000D));
                }

                //MeshVertices[i].BoneWeights = new Vector4(vert.BoneWeights[0], vert.BoneWeights[1], vert.BoneWeights[2], vert.BoneWeights[3]);

                // Apply per-mesh bone indices for DS1 and older
                /*if (flvr.Header.Version <= 0x2000D)
                {
                    // Hotfix for my own bad models imported with DSFBX / FBX2FLVER lol im sorry i learned now that
                    // they don't use -1
                    if (vert.BoneIndices[0] < 0)
                        vert.BoneIndices[0] = 0;
                    if (vert.BoneIndices[1] < 0)
                        vert.BoneIndices[1] = 0;
                    if (vert.BoneIndices[2] < 0)
                        vert.BoneIndices[2] = 0;
                    if (vert.BoneIndices[3] < 0)
                        vert.BoneIndices[3] = 0;

                    if (vert.BoneIndices[0] >= mesh.BoneIndices.Count)
                        vert.BoneIndices[0] = 0;
                    if (vert.BoneIndices[1] >= mesh.BoneIndices.Count)
                        vert.BoneIndices[1] = 0;
                    if (vert.BoneIndices[2] >= mesh.BoneIndices.Count)
                        vert.BoneIndices[2] = 0;
                    if (vert.BoneIndices[3] >= mesh.BoneIndices.Count)
                        vert.BoneIndices[3] = 0;

                    vert.BoneIndices[0] = mesh.BoneIndices[vert.BoneIndices[0]];
                    vert.BoneIndices[1] = mesh.BoneIndices[vert.BoneIndices[1]];
                    vert.BoneIndices[2] = mesh.BoneIndices[vert.BoneIndices[2]];
                    vert.BoneIndices[3] = mesh.BoneIndices[vert.BoneIndices[3]];
                }

                if (finalBoneRemapper != null)
                {
                    if (finalBoneRemapper.ContainsKey(vert.BoneIndices[0]))
                        vert.BoneIndices[0] = finalBoneRemapper[vert.BoneIndices[0]];

                    if (finalBoneRemapper.ContainsKey(vert.BoneIndices[1]))
                        vert.BoneIndices[1] = finalBoneRemapper[vert.BoneIndices[1]];

                    if (finalBoneRemapper.ContainsKey(vert.BoneIndices[2]))
                        vert.BoneIndices[2] = finalBoneRemapper[vert.BoneIndices[2]];

                    if (finalBoneRemapper.ContainsKey(vert.BoneIndices[3]))
                        vert.BoneIndices[3] = finalBoneRemapper[vert.BoneIndices[3]];
                }*/

                /*MeshVertices[i].BoneIndices = new Vector4(
                    (int)(vert.BoneIndices[0] >= 0 ? vert.BoneIndices[0] % FlverShader.MaxBonePerMatrixArray : -1),
                    (int)(vert.BoneIndices[1] >= 0 ? vert.BoneIndices[1] % FlverShader.MaxBonePerMatrixArray : -1),
                    (int)(vert.BoneIndices[2] >= 0 ? vert.BoneIndices[2] % FlverShader.MaxBonePerMatrixArray : -1),
                    (int)(vert.BoneIndices[3] >= 0 ? vert.BoneIndices[3] % FlverShader.MaxBonePerMatrixArray : -1));

                MeshVertices[i].BoneIndicesBank = new Vector4(
                   (float)(vert.BoneIndices[0] >= 0 ? Math.Floor(1.0f * vert.BoneIndices[0] / FlverShader.MaxBonePerMatrixArray) : -1.0),
                   (float)(vert.BoneIndices[1] >= 0 ? Math.Floor(1.0f * vert.BoneIndices[1] / FlverShader.MaxBonePerMatrixArray) : -1.0),
                   (float)(vert.BoneIndices[2] >= 0 ? Math.Floor(1.0f * vert.BoneIndices[2] / FlverShader.MaxBonePerMatrixArray) : -1.0),
                   (float)(vert.BoneIndices[3] >= 0 ? Math.Floor(1.0f * vert.BoneIndices[3] / FlverShader.MaxBonePerMatrixArray) : -1.0));

                if (vert.BoneIndices[0] < 0)
                    MeshVertices[i].BoneWeights.X = 0;

                if (vert.BoneIndices[1] < 0)
                    MeshVertices[i].BoneWeights.Y = 0;

                if (vert.BoneIndices[2] < 0)
                    MeshVertices[i].BoneWeights.Z = 0;

                if (vert.BoneIndices[3] < 0)
                    MeshVertices[i].BoneWeights.W = 0;

                vert.BoneWeights = ORIG_BONE_WEIGHTS;
                vert.BoneIndices = ORIG_BONE_INDICES;*/

                MeshVertices[i].Position = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);
                dest.PickingVertices[i] = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);

                var n = Vector3.Normalize(new Vector3(vert.Normal.X, vert.Normal.Y, vert.Normal.Z));
                MeshVertices[i].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i].Normal[2] = (sbyte)(n.Z * 127.0f);

                if (vert.Colors.Count >= 1)
                {
                    //MeshVertices[i].Color = new Vector4(vert.Colors[0].R, vert.Colors[0].G, vert.Colors[0].B, vert.Colors[0].A);
                    MeshVertices[i].Color[0] = (byte)(vert.Colors[0].R * 255.0f);
                    MeshVertices[i].Color[1] = (byte)(vert.Colors[0].G * 255.0f);
                    MeshVertices[i].Color[2] = (byte)(vert.Colors[0].B * 255.0f);
                    MeshVertices[i].Color[3] = (byte)(vert.Colors[0].A * 255.0f);
                }

                if (vert.Tangents.Count > 0)
                {
                    var bt = new Vector4(vert.Tangents[0].X, vert.Tangents[0].Y, vert.Tangents[0].Z, vert.Tangents[0].W);
                    var bn = Vector3.Cross(Vector3.Normalize(n), Vector3.Normalize(new Vector3(bt.X, bt.Y, bt.Z))) * vert.Tangents[0].W;
                    MeshVertices[i].Bitangent[0] = (sbyte)(bt.X * 127.0f);
                    MeshVertices[i].Bitangent[1] = (sbyte)(bt.Y * 127.0f);
                    MeshVertices[i].Bitangent[2] = (sbyte)(bt.Z * 127.0f);
                    MeshVertices[i].Bitangent[3] = (sbyte)(bt.W * 127.0f);
                    MeshVertices[i].Binormal[0] = (sbyte)(bn.X * 127.0f);
                    MeshVertices[i].Binormal[1] = (sbyte)(bn.Y * 127.0f);
                    MeshVertices[i].Binormal[2] = (sbyte)(bn.Z * 127.0f);
                }


                if (vert.UVs.Count > 0)
                {
                    if (useSecondUV && vert.UVs.Count > 1)
                    {
                        //MeshVertices[i].TextureCoordinate = new Vector2(vert.UVs[1].X, vert.UVs[1].Y);
                        MeshVertices[i].Uv1[0] = (short)(vert.UVs[1].X * 2048.0f);
                        MeshVertices[i].Uv1[1] = (short)(vert.UVs[1].Y * 2048.0f);
                    }
                    else
                    {
                        //MeshVertices[i].TextureCoordinate = new Vector2(vert.UVs[0].X, vert.UVs[0].Y);
                        MeshVertices[i].Uv1[0] = (short)(vert.UVs[0].X * 2048.0f);
                        MeshVertices[i].Uv1[1] = (short)(vert.UVs[0].Y * 2048.0f);
                    }

                    if (vert.UVs.Count >= 2)
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

            foreach (var faceset in facesets)
            {
                if (faceset.Indices.Count == 0)
                    continue;

                //At this point they use 32-bit faceset vertex indices
                bool is32bit = Flver.Header.Version > 0x20005 && mesh.Vertices.Count() > 65535;

                uint buffersize = (uint)faceset.Indices.Count * (is32bit ? 4u : 2u);
                var newFaceSet = new FlverSubmesh.FlverSubmeshFaceSet()
                {
                    BackfaceCulling = faceset.CullBackfaces,
                    IsTriangleStrip = faceset.TriangleStrip,
                    IndexBuffer = factory.CreateBuffer(new BufferDescription(buffersize, BufferUsage.IndexBuffer)),

                    IndexCount = faceset.Indices.Count,
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

                Scene.Renderer.AddBackgroundUploadTask((device, cl) =>
                {
                    if (is32bit)
                    {
                        cl.UpdateBuffer(newFaceSet.IndexBuffer, 0, faceset.Indices.Select(x => (x == 0xFFFF && x > mesh.Vertices.Count) ? -1 : x).ToArray());
                        //newFaceSet.IndexBuffer.SetData(faceset.Indices.Select(x => (x == 0xFFFF && x > mesh.Vertices.Count) ? -1 : x).ToArray());
                    }
                    else
                    {
                        cl.UpdateBuffer(newFaceSet.IndexBuffer, 0, faceset.Indices.Select<int, ushort>(x => (ushort)((x == 0xFFFF && x > mesh.Vertices.Count) ? 0xFFFF : (ushort)x)).ToArray());
                        //newFaceSet.IndexBuffer.SetData(faceset.Indices.Select(x => (x == 0xFFFF && x > mesh.Vertices.Count) ? -1 : (ushort)x).ToArray());
                    }
                    fsUploadsPending--;
                    if (fsUploadsPending <= 0)
                    {
                        facesets = null;
                    }
                });

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

            uint vbuffersize = (uint)MeshVertices.Length * MapFlverLayout.SizeInBytes;
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

        bool IResource._Load(byte[] bytes, AccessLevel al)
        {
            Flver = FLVER2.Read(bytes);
            return LoadInternal(al);
        }

        bool IResource._Load(string file, AccessLevel al)
        {
            Flver = FLVER2.Read(file);
            return LoadInternal(al);
        }

        public bool RayCast(Ray ray, Matrix4x4 transform, out float dist)
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
                var fc = mesh.MeshFacesets[0];
                for (int index = 0; index < fc.PickingIndices.Count(); index += 3)
                {
                    //var a = Vector3.Transform(mesh.PickingVertices[fc.PickingIndices[index]], transform);
                    //var b = Vector3.Transform(mesh.PickingVertices[fc.PickingIndices[index + 1]], transform);
                    //var c = Vector3.Transform(mesh.PickingVertices[fc.PickingIndices[index + 2]], transform);
                    //var a = mesh.PickingVertices[fc.PickingIndices[index]];
                    //var b = mesh.PickingVertices[fc.PickingIndices[index + 1]];
                    //var c = mesh.PickingVertices[fc.PickingIndices[index + 2]];
                    float locdist;
                    if (tray.Intersects(ref mesh.PickingVertices[fc.PickingIndices[index]],
                        ref mesh.PickingVertices[fc.PickingIndices[index + 1]],
                        ref mesh.PickingVertices[fc.PickingIndices[index + 2]],
                        out locdist))
                    {
                        hit = true;
                        if (locdist < mindist)
                        {
                            mindist = locdist;
                        }
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
                        m.VertBuffer.Dispose();
                        foreach (var fs in m.MeshFacesets)
                        {
                            fs.IndexBuffer.Dispose();
                        }
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
