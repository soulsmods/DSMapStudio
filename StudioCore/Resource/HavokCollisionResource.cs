using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using SoulsFormats;
using System.IO;
using System.Threading.Tasks.Dataflow;
using HKX2;
using Vortice.Vulkan;

namespace StudioCore.Resource
{
    public class HavokCollisionResource : IResource, IDisposable
    {
        public class CollisionSubmesh
        {
            public int IndexCount;
            public int[] PickingIndices;

            public Scene.VertexIndexBufferAllocator.VertexIndexBufferHandle GeomBuffer { get; set; }

            public int VertexCount { get; set; }

            public Vector3[] PickingVertices;
            public BoundingBox Bounds { get; set; }
        }
        public HKX Hkx = null;
        public HKX2.hkRootLevelContainer Hkx2 = null;

        public CollisionSubmesh[] GPUMeshes = null;

        public BoundingBox Bounds { get; set; }

        public VkFrontFace FrontFace { get; private set; }

        unsafe private void ProcessMesh(HKX.HKPStorageExtendedMeshShapeMeshSubpartStorage mesh, CollisionSubmesh dest)
        {
            var verts = mesh.Vertices.GetArrayData().Elements;
            dynamic indices;
            if(mesh.Indices8?.Capacity > 0)
            {
                indices = mesh.Indices8.GetArrayData().Elements;
            }
            else if(mesh.Indices16?.Capacity > 0)
            {
                indices = mesh.Indices16.GetArrayData().Elements;
            }
            else //Indices32 have to be there if those aren't
            {
                indices = mesh.Indices32.GetArrayData().Elements;
            }
            dest.VertexCount = (indices.Count / 4) * 3;
            dest.IndexCount = (indices.Count / 4) * 3;
            uint buffersize = (uint)dest.IndexCount * 4u;
            uint vbuffersize = (uint)dest.VertexCount * CollisionLayout.SizeInBytes;
            dest.GeomBuffer = Scene.Renderer.GeometryBufferAllocator.Allocate(vbuffersize, buffersize, (int)CollisionLayout.SizeInBytes, 4);
            var MeshIndices = new Span<int>(dest.GeomBuffer.MapIBuffer().ToPointer(), dest.IndexCount);
            var MeshVertices = new Span<CollisionLayout>(dest.GeomBuffer.MapVBuffer().ToPointer(), dest.VertexCount);
            dest.PickingVertices = new Vector3[(indices.Count / 4) * 3];
            dest.PickingIndices = new int[(indices.Count / 4) * 3];
            
            for (int id = 0; id < indices.Count; id += 4)
            {
                int i = (id / 4) * 3;
                var vert1 = mesh.Vertices[(int)indices[id].data].Vector;
                var vert2 = mesh.Vertices[(int)indices[id+1].data].Vector;
                var vert3 = mesh.Vertices[(int)indices[id+2].data].Vector;

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
                MeshVertices[i].Barycentric[0] = 0;
                MeshVertices[i].Barycentric[1] = 0;
                MeshVertices[i+1].Barycentric[0] = 1;
                MeshVertices[i+1].Barycentric[1] = 0;
                MeshVertices[i+2].Barycentric[0] = 0;
                MeshVertices[i+2].Barycentric[1] = 1;

                MeshIndices[i] = i;
                MeshIndices[i + 1] = i + 1;
                MeshIndices[i + 2] = i + 2;
                dest.PickingIndices[i] = i;
                dest.PickingIndices[i + 1] = i + 1;
                dest.PickingIndices[i + 2] = i + 2;
            }
            
            dest.GeomBuffer.UnmapIBuffer();
            dest.GeomBuffer.UnmapVBuffer();

            fixed (void* ptr = dest.PickingVertices)
            {
                dest.Bounds = BoundingBox.CreateFromPoints((Vector3*)ptr, dest.PickingVertices.Count(), 12, Quaternion.Identity, Vector3.Zero, Vector3.One);
            }
        }

        internal static Vector3 TransformVert(System.Numerics.Vector3 vert, HKX.HKNPBodyCInfo body)
        {
            var newVert = new Vector3(vert.X, vert.Y, vert.Z);
            if (body == null)
            {
                return newVert;
            }

            Vector3 trans = new Vector3(body.Position.Vector.X, body.Position.Vector.Y, body.Position.Vector.Z);
            Quaternion quat = new Quaternion(body.Orientation.Vector.X, body.Orientation.Vector.Y, body.Orientation.Vector.Z, body.Orientation.Vector.W);
            return Vector3.Transform(newVert, quat) + trans;
        }

        internal static Vector3 TransformVert(System.Numerics.Vector3 vert, HKX2.hknpBodyCinfo body)
        {
            var newVert = new Vector3(vert.X, vert.Y, vert.Z);
            if (body == null)
            {
                return newVert;
            }

            Vector3 trans = new Vector3(body.m_position.X, body.m_position.Y, body.m_position.Z);
            return Vector3.Transform(newVert, body.m_orientation) + trans;
        }

        unsafe private void ProcessMesh(HKX.FSNPCustomParamCompressedMeshShape mesh, HKX.HKNPBodyCInfo bodyinfo, CollisionSubmesh dest)
        {
            var verts = new List<Vector3>();
            var indices = new List<int>();

            var coldata = mesh.GetMeshShapeData();
            foreach (var section in coldata.sections.GetArrayData().Elements)
            {
                for (int i = 0; i < section.primitivesLength; i++)
                {
                    var tri = coldata.primitives.GetArrayData().Elements[i + section.primitivesIndex];
                    //if (tri.Idx2 == tri.Idx3 && tri.Idx1 != tri.Idx2)
                    //{

                    if (tri.Idx0 == 0xDE && tri.Idx1 == 0xAD && tri.Idx2 == 0xDE && tri.Idx3 == 0xAD)
                    {
                        continue; // Don't know what to do with this shape yet
                    }

                    if (tri.Idx0 < section.sharedVerticesLength)
                    {
                        ushort index = (ushort)((uint)tri.Idx0 + section.firstPackedVertex);
                        indices.Add(verts.Count);

                        var vert = coldata.packedVertices.GetArrayData().Elements[index].Decompress(section.SmallVertexScale, section.SmallVertexOffset);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }
                    else
                    {
                        ushort index = (ushort)(coldata.sharedVerticesIndex.GetArrayData().Elements[tri.Idx0 + section.sharedVerticesIndex - section.sharedVerticesLength].data);
                        indices.Add(verts.Count);

                        var vert = coldata.sharedVertices.GetArrayData().Elements[index].Decompress(coldata.BoundingBoxMin, coldata.BoundingBoxMax);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }

                    if (tri.Idx1 < section.sharedVerticesLength)
                    {
                        ushort index = (ushort)((uint)tri.Idx1 + section.firstPackedVertex);
                        indices.Add(verts.Count);

                        var vert = coldata.packedVertices.GetArrayData().Elements[index].Decompress(section.SmallVertexScale, section.SmallVertexOffset);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }
                    else
                    {
                        ushort index = (ushort)(coldata.sharedVerticesIndex.GetArrayData().Elements[tri.Idx1 + section.sharedVerticesIndex - section.sharedVerticesLength].data);
                        indices.Add(verts.Count);

                        var vert = coldata.sharedVertices.GetArrayData().Elements[index].Decompress(coldata.BoundingBoxMin, coldata.BoundingBoxMax);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }

                    if (tri.Idx2 < section.sharedVerticesLength)
                    {
                        ushort index = (ushort)((uint)tri.Idx2 + section.firstPackedVertex);
                        indices.Add(verts.Count);

                        var vert = coldata.packedVertices.GetArrayData().Elements[index].Decompress(section.SmallVertexScale, section.SmallVertexOffset);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }
                    else
                    {
                        ushort index = (ushort)(coldata.sharedVerticesIndex.GetArrayData().Elements[tri.Idx2 + section.sharedVerticesIndex - section.sharedVerticesLength].data);
                        indices.Add(verts.Count);

                        var vert = coldata.sharedVertices.GetArrayData().Elements[index].Decompress(coldata.BoundingBoxMin, coldata.BoundingBoxMax);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }

                    if (tri.Idx2 != tri.Idx3)
                    {
                        indices.Add(verts.Count);
                        verts.Add(verts[verts.Count - 3]);
                        indices.Add(verts.Count);
                        verts.Add(verts[verts.Count - 2]);
                        if (tri.Idx3 < section.sharedVerticesLength)
                        {
                            ushort index = (ushort)((uint)tri.Idx3 + section.firstPackedVertex);
                            indices.Add(verts.Count);

                            var vert = coldata.packedVertices.GetArrayData().Elements[index].Decompress(section.SmallVertexScale, section.SmallVertexOffset);
                            verts.Add(TransformVert(vert, bodyinfo));
                        }
                        else
                        {
                            ushort index = (ushort)(coldata.sharedVerticesIndex.GetArrayData().Elements[tri.Idx3 + section.sharedVerticesIndex - section.sharedVerticesLength].data);
                            indices.Add(verts.Count);

                            var vert = coldata.sharedVertices.GetArrayData().Elements[index].Decompress(coldata.BoundingBoxMin, coldata.BoundingBoxMax);
                            verts.Add(TransformVert(vert, bodyinfo));
                        }
                    }
                }
            }

            dest.PickingIndices = indices.ToArray();
            dest.PickingVertices = verts.ToArray();

            dest.VertexCount = indices.Count;
            dest.IndexCount = indices.Count;
            uint buffersize = (uint)dest.IndexCount * 4u;
            uint vbuffersize = (uint)dest.VertexCount * CollisionLayout.SizeInBytes;
            dest.GeomBuffer = Scene.Renderer.GeometryBufferAllocator.Allocate(vbuffersize, buffersize, (int)CollisionLayout.SizeInBytes, 4);
            var MeshIndices = new Span<int>(dest.GeomBuffer.MapIBuffer().ToPointer(), dest.IndexCount);
            var MeshVertices = new Span<CollisionLayout>(dest.GeomBuffer.MapVBuffer().ToPointer(), dest.VertexCount);

            for (int i = 0; i < indices.Count; i += 3)
            {
                var vert1 = verts[indices[i]];
                var vert2 = verts[indices[i + 1]];
                var vert3 = verts[indices[i + 2]];

                MeshVertices[i] = new CollisionLayout();
                MeshVertices[i + 1] = new CollisionLayout();
                MeshVertices[i + 2] = new CollisionLayout();

                MeshVertices[i].Position = vert1;
                MeshVertices[i + 1].Position = vert2;
                MeshVertices[i + 2].Position = vert3;
                var n = Vector3.Normalize(Vector3.Cross(MeshVertices[i + 2].Position - MeshVertices[i].Position, MeshVertices[i + 1].Position - MeshVertices[i].Position));
                MeshVertices[i].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i].Normal[2] = (sbyte)(n.Z * 127.0f);
                MeshVertices[i + 1].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i + 1].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i + 1].Normal[2] = (sbyte)(n.Z * 127.0f);
                MeshVertices[i + 2].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i + 2].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i + 2].Normal[2] = (sbyte)(n.Z * 127.0f);

                MeshVertices[i].Color[0] = (byte)(53);
                MeshVertices[i].Color[1] = (byte)(157);
                MeshVertices[i].Color[2] = (byte)(255);
                MeshVertices[i].Color[3] = (byte)(255);
                MeshVertices[i + 1].Color[0] = (byte)(53);
                MeshVertices[i + 1].Color[1] = (byte)(157);
                MeshVertices[i + 1].Color[2] = (byte)(255);
                MeshVertices[i + 1].Color[3] = (byte)(255);
                MeshVertices[i + 2].Color[0] = (byte)(53);
                MeshVertices[i + 2].Color[1] = (byte)(157);
                MeshVertices[i + 2].Color[2] = (byte)(255);
                MeshVertices[i + 2].Color[3] = (byte)(255);
                MeshVertices[i].Barycentric[0] = 0;
                MeshVertices[i].Barycentric[1] = 0;
                MeshVertices[i + 1].Barycentric[0] = 1;
                MeshVertices[i + 1].Barycentric[1] = 0;
                MeshVertices[i + 2].Barycentric[0] = 0;
                MeshVertices[i + 2].Barycentric[1] = 1;

                MeshIndices[i] = i;
                MeshIndices[i + 1] = i + 1;
                MeshIndices[i + 2] = i + 2;
            }
            
            dest.GeomBuffer.UnmapVBuffer();
            dest.GeomBuffer.UnmapIBuffer();

            fixed (void* ptr = dest.PickingVertices)
            {
                dest.Bounds = BoundingBox.CreateFromPoints((Vector3*)ptr, dest.PickingVertices.Count(), 12, Quaternion.Identity, Vector3.Zero, Vector3.One);
            }
        }

        unsafe private void ProcessMesh(HKX2.fsnpCustomParamCompressedMeshShape mesh, HKX2.hknpBodyCinfo bodyinfo, CollisionSubmesh dest)
        {
            var verts = new List<Vector3>();
            var indices = new List<int>();

            var coldata = mesh.m_data;
            foreach (var section in coldata.m_meshTree.m_sections)
            {
                for (int i = 0; i < (section.m_primitives.m_data & 0xFF); i++)
                {
                    var tri = coldata.m_meshTree.m_primitives[i + (int)(section.m_primitives.m_data >> 8)];
                    //if (tri.Idx2 == tri.Idx3 && tri.Idx1 != tri.Idx2)
                    //{

                    if (tri.m_indices_0 == 0xDE && tri.m_indices_1 == 0xAD && tri.m_indices_2 == 0xDE && tri.m_indices_3 == 0xAD)
                    {
                        continue; // Don't know what to do with this shape yet
                    }

                    uint sharedVerticesLength = section.m_sharedVertices.m_data & 0xFF;
                    uint sharedVerticesIndex = section.m_sharedVertices.m_data >> 8;
                    var smallVertexOffset = new Vector3(section.m_codecParms_0, section.m_codecParms_1, section.m_codecParms_2);
                    var smallVertexScale = new Vector3(section.m_codecParms_3, section.m_codecParms_4, section.m_codecParms_5);
                    if (tri.m_indices_0 < sharedVerticesLength)
                    {
                        ushort index = (ushort)((uint)tri.m_indices_0 + section.m_firstPackedVertex);
                        indices.Add(verts.Count);

                        var vert = coldata.DecompressPackedVertex(coldata.m_meshTree.m_packedVertices[index], smallVertexScale, smallVertexOffset);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }
                    else
                    {
                        ushort index = (ushort)(coldata.m_meshTree.m_sharedVerticesIndex[(int)(tri.m_indices_0 + sharedVerticesIndex - sharedVerticesLength)]);
                        indices.Add(verts.Count);

                        var vert = coldata.DecompressSharedVertex(coldata.m_meshTree.m_sharedVertices[index], coldata.m_meshTree.m_domain.m_min, coldata.m_meshTree.m_domain.m_max);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }

                    if (tri.m_indices_1 < sharedVerticesLength)
                    {
                        ushort index = (ushort)((uint)tri.m_indices_1 + section.m_firstPackedVertex);
                        indices.Add(verts.Count);

                        var vert = coldata.DecompressPackedVertex(coldata.m_meshTree.m_packedVertices[index], smallVertexScale, smallVertexOffset);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }
                    else
                    {
                        ushort index = (ushort)(coldata.m_meshTree.m_sharedVerticesIndex[(int)(tri.m_indices_1 + sharedVerticesIndex - sharedVerticesLength)]);
                        indices.Add(verts.Count);

                        var vert = coldata.DecompressSharedVertex(coldata.m_meshTree.m_sharedVertices[index], coldata.m_meshTree.m_domain.m_min, coldata.m_meshTree.m_domain.m_max);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }

                    if (tri.m_indices_2 < sharedVerticesLength)
                    {
                        ushort index = (ushort)((uint)tri.m_indices_2 + section.m_firstPackedVertex);
                        indices.Add(verts.Count);

                        var vert = coldata.DecompressPackedVertex(coldata.m_meshTree.m_packedVertices[index], smallVertexScale, smallVertexOffset);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }
                    else
                    {
                        ushort index = (ushort)(coldata.m_meshTree.m_sharedVerticesIndex[(int)(tri.m_indices_2 + sharedVerticesIndex - sharedVerticesLength)]);
                        indices.Add(verts.Count);

                        var vert = coldata.DecompressSharedVertex(coldata.m_meshTree.m_sharedVertices[index], coldata.m_meshTree.m_domain.m_min, coldata.m_meshTree.m_domain.m_max);
                        verts.Add(TransformVert(vert, bodyinfo));
                    }

                    if (tri.m_indices_2 != tri.m_indices_3)
                    {
                        indices.Add(verts.Count);
                        verts.Add(verts[verts.Count - 3]);
                        indices.Add(verts.Count);
                        verts.Add(verts[verts.Count - 2]);
                        if (tri.m_indices_3 < sharedVerticesLength)
                        {
                            ushort index = (ushort)((uint)tri.m_indices_3 + section.m_firstPackedVertex);
                            indices.Add(verts.Count);

                            var vert = coldata.DecompressPackedVertex(coldata.m_meshTree.m_packedVertices[index], smallVertexScale, smallVertexOffset);
                            verts.Add(TransformVert(vert, bodyinfo));
                        }
                        else
                        {
                            ushort index = (ushort)(coldata.m_meshTree.m_sharedVerticesIndex[(int)(tri.m_indices_3 + sharedVerticesIndex - sharedVerticesLength)]);
                            indices.Add(verts.Count);

                            var vert = coldata.DecompressSharedVertex(coldata.m_meshTree.m_sharedVertices[index], coldata.m_meshTree.m_domain.m_min, coldata.m_meshTree.m_domain.m_max);
                            verts.Add(TransformVert(vert, bodyinfo));
                        }
                    }
                }
            }

            dest.PickingIndices = indices.ToArray();
            dest.PickingVertices = verts.ToArray();

            dest.VertexCount = indices.Count;
            dest.IndexCount = indices.Count;
            uint buffersize = (uint)dest.IndexCount * 4u;
            uint vbuffersize = (uint)dest.VertexCount * CollisionLayout.SizeInBytes;
            dest.GeomBuffer = Scene.Renderer.GeometryBufferAllocator.Allocate(vbuffersize, buffersize, (int)CollisionLayout.SizeInBytes, 4);
            var MeshIndices = new Span<int>(dest.GeomBuffer.MapIBuffer().ToPointer(), dest.IndexCount);
            var MeshVertices = new Span<CollisionLayout>(dest.GeomBuffer.MapVBuffer().ToPointer(), dest.VertexCount);

            for (int i = 0; i < indices.Count; i += 3)
            {
                var vert1 = verts[indices[i]];
                var vert2 = verts[indices[i + 1]];
                var vert3 = verts[indices[i + 2]];

                MeshVertices[i] = new CollisionLayout();
                MeshVertices[i + 1] = new CollisionLayout();
                MeshVertices[i + 2] = new CollisionLayout();

                MeshVertices[i].Position = vert1;
                MeshVertices[i + 1].Position = vert2;
                MeshVertices[i + 2].Position = vert3;
                var n = Vector3.Normalize(Vector3.Cross(MeshVertices[i + 2].Position - MeshVertices[i].Position, MeshVertices[i + 1].Position - MeshVertices[i].Position));
                MeshVertices[i].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i].Normal[2] = (sbyte)(n.Z * 127.0f);
                MeshVertices[i + 1].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i + 1].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i + 1].Normal[2] = (sbyte)(n.Z * 127.0f);
                MeshVertices[i + 2].Normal[0] = (sbyte)(n.X * 127.0f);
                MeshVertices[i + 2].Normal[1] = (sbyte)(n.Y * 127.0f);
                MeshVertices[i + 2].Normal[2] = (sbyte)(n.Z * 127.0f);

                MeshVertices[i].Color[0] = (byte)(53);
                MeshVertices[i].Color[1] = (byte)(157);
                MeshVertices[i].Color[2] = (byte)(255);
                MeshVertices[i].Color[3] = (byte)(255);
                MeshVertices[i + 1].Color[0] = (byte)(53);
                MeshVertices[i + 1].Color[1] = (byte)(157);
                MeshVertices[i + 1].Color[2] = (byte)(255);
                MeshVertices[i + 1].Color[3] = (byte)(255);
                MeshVertices[i + 2].Color[0] = (byte)(53);
                MeshVertices[i + 2].Color[1] = (byte)(157);
                MeshVertices[i + 2].Color[2] = (byte)(255);
                MeshVertices[i + 2].Color[3] = (byte)(255);
                MeshVertices[i].Barycentric[0] = 0;
                MeshVertices[i].Barycentric[1] = 0;
                MeshVertices[i + 1].Barycentric[0] = 1;
                MeshVertices[i + 1].Barycentric[1] = 0;
                MeshVertices[i + 2].Barycentric[0] = 0;
                MeshVertices[i + 2].Barycentric[1] = 1;

                MeshIndices[i] = i;
                MeshIndices[i + 1] = i + 1;
                MeshIndices[i + 2] = i + 2;
            }
            
            dest.GeomBuffer.UnmapIBuffer();
            dest.GeomBuffer.UnmapVBuffer();
            
            fixed (void* ptr = dest.PickingVertices)
            {
                dest.Bounds = BoundingBox.CreateFromPoints((Vector3*)ptr, dest.PickingVertices.Count(), 12, Quaternion.Identity, Vector3.Zero, Vector3.One);
            }
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
                    if (obj is HKX.FSNPCustomParamCompressedMeshShape ncol)
                    {
                        // Find a body data for this
                        HKX.HKNPBodyCInfo bodyInfo = null;
                        foreach (var scene in Hkx.DataSection.Objects)
                        {
                            if (scene is HKX.HKNPPhysicsSystemData)
                            {
                                var sys = (HKX.HKNPPhysicsSystemData)scene;
                                foreach (HKX.HKNPBodyCInfo info in sys.Bodies.GetArrayData().Elements)
                                {
                                    if (info.ShapeReference.DestObject == ncol)
                                    {
                                        bodyInfo = info;
                                        break;
                                    }
                                }
                                break;
                            }

                            try
                            {
                                var mesh = new CollisionSubmesh();
                                ProcessMesh(ncol, bodyInfo, mesh);
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
                            catch (Exception e)
                            {
                                // Debug failing cases later
                            }
                        }
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

        private bool LoadInternalNew(AccessLevel al)
        {
            if (al == AccessLevel.AccessFull || al == AccessLevel.AccessGPUOptimizedOnly)
            {
                Bounds = new BoundingBox();
                var submeshes = new List<CollisionSubmesh>();
                bool first = true;
                if (Hkx2.m_namedVariants.Count == 0)
                {
                    // Yes this happens for some cols wtf From???
                    return false;
                }
                var physicsscene = (hknpPhysicsSceneData)Hkx2.m_namedVariants[0].m_variant;

                foreach (var bodyInfo in physicsscene.m_systemDatas[0].m_bodyCinfos)
                {
                    if (bodyInfo.m_shape is not HKX2.fsnpCustomParamCompressedMeshShape ncol)
                        continue;
                    try
                    {
                        var mesh = new CollisionSubmesh();
                        ProcessMesh(ncol, bodyInfo, mesh);
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
                    catch (Exception e)
                    {
                        // Debug failing cases later
                    }
                }

                GPUMeshes = submeshes.ToArray();
            }

            if (al == AccessLevel.AccessGPUOptimizedOnly)
            {
                Hkx = null;
            }
            return true;
        }

        public bool _Load(byte[] bytes, AccessLevel al, GameType type)
        {
            
            if (type == GameType.Bloodborne)
            {
                Hkx = HKX.Read(bytes, HKX.HKXVariation.HKXBloodBorne);
            }
            else if (type == GameType.DarkSoulsIII)
            {
                DCX.Type t;
                var decomp = DCX.Decompress(bytes, out t);
                var br = new BinaryReaderEx(false, decomp);
                var des = new HKX2.PackFileDeserializer();
                Hkx2 = (hkRootLevelContainer)des.Deserialize(br);
            }
            else
            {
                Hkx = HKX.Read(bytes);
            }

            if (type == GameType.DarkSoulsIISOTFS || type == GameType.DarkSoulsIII || type == GameType.Bloodborne)
            {
                FrontFace = VkFrontFace.Clockwise;
            }
            else
            {
                FrontFace = VkFrontFace.CounterClockwise;
            }

            if (type == GameType.DarkSoulsIII)
            {
                return LoadInternalNew(al);
            }
            return LoadInternal(al);
        }

        public bool _Load(string file, AccessLevel al, GameType type)
        {
            if (type == GameType.Bloodborne)
            {
                Hkx = HKX.Read(file, HKX.HKXVariation.HKXBloodBorne);
            }
            else if (type == GameType.DarkSoulsIII)
            {
                DCX.Type t;
                var decomp = DCX.Decompress(file, out t);
                var br = new BinaryReaderEx(false, decomp);
                var des = new HKX2.PackFileDeserializer();
                Hkx2 = (hkRootLevelContainer)des.Deserialize(br);
            }
            else
            {
                Hkx = HKX.Read(file);
            }

            if (type == GameType.DarkSoulsIISOTFS || type == GameType.DarkSoulsIII || type == GameType.Bloodborne)
            {
                FrontFace = VkFrontFace.Clockwise;
            }
            else
            {
                FrontFace = VkFrontFace.CounterClockwise;
            }

            if (type == GameType.DarkSoulsIII)
            {
                return LoadInternalNew(al);
            }
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
                }

                if (GPUMeshes != null)
                {
                    foreach (var m in GPUMeshes)
                    {
                        m.GeomBuffer.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        ~HavokCollisionResource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
