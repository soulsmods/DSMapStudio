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
            public int[] PickingIndices;

            public Scene.VertexIndexBufferAllocator.VertexIndexBufferHandle GeomBuffer { get; set; }

            public int VertexCount { get; set; }

            public Vector3[] PickingVertices;
            public BoundingBox Bounds { get; set; }
        }
        public HKX Hkx = null;

        public CollisionSubmesh[] GPUMeshes = null;

        public BoundingBox Bounds { get; set; }

        public FrontFace FrontFace { get; private set; }

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

            dest.VertexCount = MeshVertices.Length;
            dest.IndexCount = MeshIndices.Length;

            uint buffersize = (uint)dest.IndexCount * 4u;

            fixed (void* ptr = dest.PickingVertices)
            {
                dest.Bounds = BoundingBox.CreateFromPoints((Vector3*)ptr, dest.PickingVertices.Count(), 12, Quaternion.Identity, Vector3.Zero, Vector3.One);
            }

            uint vbuffersize = (uint)MeshVertices.Length * CollisionLayout.SizeInBytes;

            dest.GeomBuffer = Scene.Renderer.GeometryBufferAllocator.Allocate(vbuffersize, buffersize, (int)CollisionLayout.SizeInBytes, 4, (h) =>
            {
                h.FillIBuffer(MeshIndices, () =>
                {
                    MeshIndices = null;
                });
                h.FillVBuffer(MeshVertices, () =>
                {
                    MeshVertices = null;
                });
            });
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

            var MeshIndices = new int[indices.Count];
            var MeshVertices = new CollisionLayout[indices.Count];
            var factory = Scene.Renderer.Factory;

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

            dest.VertexCount = MeshVertices.Length;
            dest.IndexCount = MeshIndices.Length;

            uint buffersize = (uint)dest.IndexCount * 4u;

            fixed (void* ptr = dest.PickingVertices)
            {
                dest.Bounds = BoundingBox.CreateFromPoints((Vector3*)ptr, dest.PickingVertices.Count(), 12, Quaternion.Identity, Vector3.Zero, Vector3.One);
            }

            uint vbuffersize = (uint)MeshVertices.Length * CollisionLayout.SizeInBytes;

            dest.GeomBuffer = Scene.Renderer.GeometryBufferAllocator.Allocate(vbuffersize, buffersize, (int)CollisionLayout.SizeInBytes, 4, (h) =>
            {
                h.FillIBuffer(MeshIndices, () =>
                {
                    MeshIndices = null;
                });
                h.FillVBuffer(MeshVertices, () =>
                {
                    MeshVertices = null;
                });
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

        bool IResource._Load(byte[] bytes, AccessLevel al, GameType type)
        {
            if (type == GameType.Bloodborne)
            {
                Hkx = HKX.Read(bytes, HKX.HKXVariation.HKXBloodBorne);
            }
            else
            {
                Hkx = HKX.Read(bytes);
            }

            if (type == GameType.DarkSoulsIISOTFS || type == GameType.DarkSoulsIII || type == GameType.Bloodborne)
            {
                FrontFace = FrontFace.Clockwise;
            }
            else
            {
                FrontFace = FrontFace.CounterClockwise;
            }
                    
            return LoadInternal(al);
        }

        bool IResource._Load(string file, AccessLevel al, GameType type)
        {
            if (type == GameType.Bloodborne)
            {
                Hkx = HKX.Read(file, HKX.HKXVariation.HKXBloodBorne);
            }
            else
            {
                Hkx = HKX.Read(file);
            }

            if (type == GameType.DarkSoulsIISOTFS || type == GameType.DarkSoulsIII || type == GameType.Bloodborne)
            {
                FrontFace = FrontFace.Clockwise;
            }
            else
            {
                FrontFace = FrontFace.CounterClockwise;
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
