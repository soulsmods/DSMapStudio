using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using SoulsFormats;
using HKX2;
using System.IO;
using System.Threading.Tasks.Dataflow;

namespace StudioCore.Resource
{
    public class HavokNavmeshResource : IResource, IDisposable
    {
        public int IndexCount;
        public int GraphIndexCount;
        public int[] PickingIndices;

        public Scene.VertexIndexBufferAllocator.VertexIndexBufferHandle GeomBuffer { get; set; }

        public Scene.VertexIndexBufferAllocator.VertexIndexBufferHandle CostGraphGeomBuffer { get; set; }


        public int VertexCount { get; set; }
        public int GraphVertexCount { get; set; }

        public Vector3[] PickingVertices;

        public hkRootLevelContainer HkxRoot = null;


        public BoundingBox Bounds { get; set; }

        unsafe private void ProcessMesh(hkaiNavMesh mesh)
        {
            var verts = mesh.m_vertices;
            int indexCount = 0;
            foreach (var f in mesh.m_faces)
            {
                // Simple formula for indices count for a triangulation of a poly
                indexCount += (f.m_numEdges - 2) * 3;
            }

            VertexCount = indexCount * 3;
            IndexCount = indexCount * 3;
            uint buffersize = (uint)IndexCount * 4u;
            uint vbuffersize = (uint)VertexCount * NavmeshLayout.SizeInBytes;
            GeomBuffer =
                Scene.Renderer.GeometryBufferAllocator.Allocate(
                    vbuffersize, buffersize, (int)NavmeshLayout.SizeInBytes, 4);
            var MeshIndices = new Span<int>(GeomBuffer.MapIBuffer().ToPointer(), IndexCount);
            var MeshVertices = new Span<NavmeshLayout>(GeomBuffer.MapVBuffer().ToPointer(), VertexCount);
            PickingVertices = new Vector3[VertexCount];
            PickingIndices = new int[IndexCount];

            var factory = Scene.Renderer.Factory;

            int idx = 0;

            int maxcluster = 0;

            for (int id = 0; id < mesh.m_faces.Count; id++)
            {
                if (mesh.m_faces[id].m_clusterIndex > maxcluster)
                {
                    maxcluster = mesh.m_faces[id].m_clusterIndex;
                }

                var sedge = mesh.m_faces[id].m_startEdgeIndex;
                var ecount = mesh.m_faces[id].m_numEdges;

                // Use simple algorithm for convex polygon trianglization
                for (int t = 0; t < ecount - 2; t++)
                {
                    if (ecount > 3)
                    {
                        //ecount = ecount;
                    }
                    var end = (t + 2 >= ecount) ? sedge : sedge + t + 2;
                    var vert1 = mesh.m_vertices[mesh.m_edges[sedge].m_a];
                    var vert2 = mesh.m_vertices[mesh.m_edges[sedge + t + 1].m_a];
                    var vert3 = mesh.m_vertices[mesh.m_edges[end].m_a];

                    MeshVertices[idx] = new NavmeshLayout();
                    MeshVertices[idx + 1] = new NavmeshLayout();
                    MeshVertices[idx + 2] = new NavmeshLayout();

                    MeshVertices[idx].Position = new Vector3(vert1.X, vert1.Y, vert1.Z);
                    MeshVertices[idx + 1].Position = new Vector3(vert2.X, vert2.Y, vert2.Z);
                    MeshVertices[idx + 2].Position = new Vector3(vert3.X, vert3.Y, vert3.Z);
                    PickingVertices[idx] = new Vector3(vert1.X, vert1.Y, vert1.Z);
                    PickingVertices[idx + 1] = new Vector3(vert2.X, vert2.Y, vert2.Z);
                    PickingVertices[idx + 2] = new Vector3(vert3.X, vert3.Y, vert3.Z);
                    var n = Vector3.Normalize(Vector3.Cross(MeshVertices[idx + 2].Position - MeshVertices[idx].Position, MeshVertices[idx + 1].Position - MeshVertices[idx].Position));
                    MeshVertices[idx].Normal[0] = (sbyte)(n.X * 127.0f);
                    MeshVertices[idx].Normal[1] = (sbyte)(n.Y * 127.0f);
                    MeshVertices[idx].Normal[2] = (sbyte)(n.Z * 127.0f);
                    MeshVertices[idx + 1].Normal[0] = (sbyte)(n.X * 127.0f);
                    MeshVertices[idx + 1].Normal[1] = (sbyte)(n.Y * 127.0f);
                    MeshVertices[idx + 1].Normal[2] = (sbyte)(n.Z * 127.0f);
                    MeshVertices[idx + 2].Normal[0] = (sbyte)(n.X * 127.0f);
                    MeshVertices[idx + 2].Normal[1] = (sbyte)(n.Y * 127.0f);
                    MeshVertices[idx + 2].Normal[2] = (sbyte)(n.Z * 127.0f);

                    MeshVertices[idx].Color[0] = (byte)(157);
                    MeshVertices[idx].Color[1] = (byte)(53);
                    MeshVertices[idx].Color[2] = (byte)(255);
                    MeshVertices[idx].Color[3] = (byte)(255);
                    MeshVertices[idx + 1].Color[0] = (byte)(157);
                    MeshVertices[idx + 1].Color[1] = (byte)(53);
                    MeshVertices[idx + 1].Color[2] = (byte)(255);
                    MeshVertices[idx + 1].Color[3] = (byte)(255);
                    MeshVertices[idx + 2].Color[0] = (byte)(157);
                    MeshVertices[idx + 2].Color[1] = (byte)(53);
                    MeshVertices[idx + 2].Color[2] = (byte)(255);
                    MeshVertices[idx + 2].Color[3] = (byte)(255);

                    MeshVertices[idx].Barycentric[0] = (byte)(0);
                    MeshVertices[idx].Barycentric[1] = (byte)(0);
                    MeshVertices[idx + 1].Barycentric[0] = (byte)(1);
                    MeshVertices[idx + 1].Barycentric[1] = (byte)(0);
                    MeshVertices[idx + 2].Barycentric[0] = (byte)(0);
                    MeshVertices[idx + 2].Barycentric[1] = (byte)(1);

                    MeshIndices[idx] = idx;
                    MeshIndices[idx + 1] = idx + 1;
                    MeshIndices[idx + 2] = idx + 2;
                    PickingIndices[idx] = idx;
                    PickingIndices[idx + 1] = idx + 1;
                    PickingIndices[idx + 2] = idx + 2;

                    idx += 3;
                }
            }
            
            GeomBuffer.UnmapIBuffer();
            GeomBuffer.UnmapVBuffer();

            if (VertexCount > 0)
            {
                fixed (void* ptr = PickingVertices)
                {
                    Bounds = BoundingBox.CreateFromPoints((Vector3*)ptr, PickingVertices.Count(), 12, Quaternion.Identity, Vector3.Zero, Vector3.One);
                }
            }
            else
            {
                Bounds = new BoundingBox();
            }
        }

        unsafe private void ProcessGraph(hkaiDirectedGraphExplicitCost graph)
        {
            var verts = graph.m_positions;
            int indexCount = 0;
            foreach (var g in graph.m_nodes)
            {
                // Simple formula for indices count for a triangulation of a poly
                indexCount += g.m_numEdges;
            }

            GraphVertexCount = indexCount * 2;
            GraphIndexCount = indexCount * 2;
            uint buffersize = (uint)GraphIndexCount * 4u;
            var lsize = MeshLayoutUtils.GetLayoutVertexSize(MeshLayoutType.LayoutPositionColor);
            uint vbuffersize = (uint)GraphVertexCount * lsize;

            CostGraphGeomBuffer = Scene.Renderer.GeometryBufferAllocator.Allocate(vbuffersize, buffersize, (int)lsize, 4);
            var MeshIndices = new Span<int>(CostGraphGeomBuffer.MapIBuffer().ToPointer(), GraphIndexCount);
            var MeshVertices = new Span<PositionColor>(CostGraphGeomBuffer.MapVBuffer().ToPointer(), GraphVertexCount);
            var vertPos = new Vector3[indexCount * 2];
            
            int idx = 0;

            for (int id = 0; id < graph.m_nodes.Count; id++)
            {
                var sedge = graph.m_nodes[id].m_startEdgeIndex;
                var ecount = graph.m_nodes[id].m_numEdges;

                for (int e = 0; e < ecount; e++)
                {
                    var vert1 = graph.m_positions[id];
                    var vert2 = graph.m_positions[(int)graph.m_edges[graph.m_nodes[id].m_startEdgeIndex + e].m_target];

                    MeshVertices[idx] = new PositionColor();
                    MeshVertices[idx + 1] = new PositionColor();

                    MeshVertices[idx].Position = new Vector3(vert1.X, vert1.Y, vert1.Z);
                    MeshVertices[idx + 1].Position = new Vector3(vert2.X, vert2.Y, vert2.Z);
                    vertPos[idx] = new Vector3(vert1.X, vert1.Y, vert1.Z);
                    vertPos[idx + 1] = new Vector3(vert2.X, vert2.Y, vert2.Z);

                    MeshVertices[idx].Color[0] = (byte)(235);
                    MeshVertices[idx].Color[1] = (byte)(200);
                    MeshVertices[idx].Color[2] = (byte)(255);
                    MeshVertices[idx].Color[3] = (byte)(255);
                    MeshVertices[idx + 1].Color[0] = (byte)(235);
                    MeshVertices[idx + 1].Color[1] = (byte)(200);
                    MeshVertices[idx + 1].Color[2] = (byte)(255);
                    MeshVertices[idx + 1].Color[3] = (byte)(255);

                    MeshIndices[idx] = idx;
                    MeshIndices[idx + 1] = idx + 1;

                    idx += 2;
                }
            }
            
            CostGraphGeomBuffer.UnmapIBuffer();
            CostGraphGeomBuffer.UnmapVBuffer();

            if (GraphVertexCount > 0)
            {
                fixed (void* ptr = vertPos)
                {
                    Bounds = BoundingBox.CreateFromPoints((Vector3*)ptr, vertPos.Count(), 12, Quaternion.Identity, Vector3.Zero, Vector3.One);
                }
            }
            else
            {
                Bounds = new BoundingBox();
            }
        }

        private bool LoadInternal(AccessLevel al)
        {
            if (al == AccessLevel.AccessFull || al == AccessLevel.AccessGPUOptimizedOnly)
            {
                Bounds = new BoundingBox();
                var mesh = HkxRoot.FindVariant<hkaiNavMesh>();
                if (mesh != null)
                {
                    ProcessMesh(mesh);
                }
                var graph = HkxRoot.FindVariant<hkaiDirectedGraphExplicitCost>();
                if (graph != null)
                {
                    ProcessGraph(graph);
                }
            }

            if (al == AccessLevel.AccessGPUOptimizedOnly)
            {
                HkxRoot = null;
            }
            return true;
        }

        public bool _Load(byte[] bytes, AccessLevel al, GameType type)
        {
            BinaryReaderEx br = new BinaryReaderEx(false, bytes);
            var des = new HKX2.PackFileDeserializer();
            HkxRoot = (hkRootLevelContainer)des.Deserialize(br);
            return LoadInternal(al);
        }

        public bool _Load(string file, AccessLevel al, GameType type)
        {
            using (var s = File.OpenRead(file))
            {
                var des = new HKX2.PackFileDeserializer();
                HkxRoot = (hkRootLevelContainer)des.Deserialize(new BinaryReaderEx(false, s));
            }
            return LoadInternal(al);
        }

        public static HavokNavmeshResource ResourceFromNavmeshRoot(hkRootLevelContainer root)
        {
            var ret = new HavokNavmeshResource();
            ret.HkxRoot = root;
            ret.LoadInternal(AccessLevel.AccessFull);
            return ret;
        }

        public bool RayCast(Ray ray, Matrix4x4 transform, out float dist)
        {
            bool hit = false;
            float mindist = float.MaxValue;
            var invw = transform.Inverse();
            var newo = Vector3.Transform(ray.Origin, invw);
            var newd = Vector3.TransformNormal(ray.Direction, invw);
            var tray = new Ray(newo, newd);
            if (!tray.Intersects(Bounds))
            {
                dist = float.MaxValue;
                return false;
            }
            for (int index = 0; index < PickingIndices.Count(); index += 3)
            {
                float locdist;
                if (tray.Intersects(ref PickingVertices[PickingIndices[index]],
                    ref PickingVertices[PickingIndices[index + 1]],
                    ref PickingVertices[PickingIndices[index + 2]],
                    out locdist))
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

                GeomBuffer.Dispose();
                CostGraphGeomBuffer.Dispose();

                disposedValue = true;
            }
        }

        ~HavokNavmeshResource()
        {
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
