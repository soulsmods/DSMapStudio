using Veldrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using StudioCore.Scene;
using System.Runtime.InteropServices;
using Veldrid.Utilities;
using StudioCore.Resource;

namespace StudioCore.DebugPrimitives
{
    public enum DbgPrimCategory
    {
        HkxBone,
        FlverBone,
        FlverBoneBoundingBox,
        DummyPoly,
        WeaponDummyPoly,
        DummyPolyHelper,
        Skybox,
        DummyPolySpawnArrow,
        Other,
    }

    public class DbgLabel
    {
        public Matrix4x4 World = Matrix4x4.Identity;
        public float Height = 1;
        public string Text = "?LabelText?";
        public Color Color;

        public DbgLabel(Matrix4x4 world, float height, string text, Color color)
        {
            World = world;
            Height = height;
            Text = text;
            Color = color;
        }
    }

    public abstract class DbgPrim : IDbgPrim, IDisposable
    {
        public string Name { get; set; }
        public Color NameColor { get; set; } = Color.Yellow;

        private bool _updateColors = true;
        private Color _baseColor = Color.Gray;
        public Color BaseColor
        {
            get => _baseColor;
            set
            {
                _baseColor = value;
                _updateColors = true;
            }
        }
        private Color _highlightedColor = Color.Gray;
        public Color HighlightedColor
        {
            get => _highlightedColor;
            set
            {
                _highlightedColor = value;
                _updateColors = true;
            }
        }
        private bool _highlighted = false;
        public bool Highlighted
        {
            get => _highlighted;
            set
            {
                _highlighted = value;
                _updateColors = true;
            }
        }

        public DbgPrimCategory Category { get; set; } = DbgPrimCategory.Other;


        protected short[] Indices = new short[0];
        protected VertexPositionColorNormal[] Vertices = new VertexPositionColorNormal[0];
        protected Shader[] Shaders;
        protected GPUBufferAllocator.GPUBufferHandle WorldBuffer;
        protected GPUBufferAllocator.GPUBufferHandle _materialBuffer;
        protected ResourceSet PerObjRS;
        protected bool NeedToRecreateGeomBuffer = true;

        public abstract MeshLayoutType LayoutType { get; }
        public abstract VertexLayoutDescription LayoutDescription { get; }
        public abstract BoundingBox Bounds { get; }
        public VertexIndexBufferAllocator.VertexIndexBufferHandle GeometryBuffer { get; protected set; }
        public abstract string ShaderName { get; }
        public abstract SpecializationConstant[] SpecializationConstants { get; }

        public abstract FaceCullMode CullMode { get; }
        public abstract PolygonFillMode FillMode { get; }
        public abstract PrimitiveTopology Topology { get; }
        public abstract uint VertexSize { get; }

        public int IndexCount => Indices.Length;

        private int bufferIndexCached = -1;
        private bool disposedValue;

        public int BufferIndex
        {
            get
            {
                if (bufferIndexCached != -1)
                {
                    return bufferIndexCached;
                }

                if (GeometryBuffer != null && GeometryBuffer.AllocStatus == VertexIndexBufferAllocator.VertexIndexBuffer.Status.Resident)
                {
                    bufferIndexCached = GeometryBuffer.BufferIndex;
                    return bufferIndexCached;
                }
                return 0;
            }
        }

        protected void SetBuffers(VertexIndexBufferAllocator.VertexIndexBufferHandle geomBuffer)
        {
            GeometryBuffer = geomBuffer;
        }

        public unsafe void UpdatePerFrameResources(Veldrid.GraphicsDevice device, CommandList cl, Scene.SceneRenderPipeline sp)
        {
            if (NeedToRecreateGeomBuffer)
            {
                //VertBuffer?.Dispose();
                GeometryBuffer = null;
                if (Vertices.Length > 0 && Indices.Length > 0)
                {
                    GeometryBuffer = Renderer.GeometryBufferAllocator.Allocate(28 * (uint)Vertices.Length, 2 * (uint)Indices.Length, 28, 2, (h) =>
                    {
                        h.FillVBuffer(Vertices);
                        h.FillIBuffer(Indices);
                    });
                }
                else
                {
                    throw new Exception("WTF");
                }
                NeedToRecreateGeomBuffer = false;
            }
        }

        public void DestroyDeviceObjects()
        {
            if (GeometryBuffer != null)
            {
                GeometryBuffer.Dispose();
            }
        }

        protected void AddVertex(Vector3 pos, Color color, Vector3? normal = null)
        {
            Array.Resize(ref Vertices, Vertices.Length + 1);
            Vertices[Vertices.Length - 1].Position = pos;
            Vertices[Vertices.Length - 1].Color = color;
            Vertices[Vertices.Length - 1].Normal = normal ?? Vector3.UnitX;

            NeedToRecreateGeomBuffer = true;
        }

        protected void AddVertex(VertexPositionColorNormal vert)
        {
            Array.Resize(ref Vertices, Vertices.Length + 1);
            Vertices[Vertices.Length - 1] = vert;

            NeedToRecreateGeomBuffer = true;
        }

        protected void AddIndex(short index)
        {
            Array.Resize(ref Indices, Indices.Length + 1);
            Indices[Indices.Length - 1] = index;
            NeedToRecreateGeomBuffer = true;
        }

        unsafe private void DrawPrimitive(Renderer.IndirectDrawEncoder encoder, Scene.SceneRenderPipeline sp)
        {
            if (GeometryBuffer == null || GeometryBuffer.VAllocationSize == 0 || GeometryBuffer.IAllocationSize == 0)
            {
                //If it's NOT a parent thing, then it shouldn't have empty geometry.
                // Some mistake was made.
                throw new Exception("DbgPrim geometry is empty and it had no children. Something went wrong...");
            }

            if (GeometryBuffer.AllocStatus != VertexIndexBufferAllocator.VertexIndexBuffer.Status.Resident)
            {
                return;
            }

            var args = new Renderer.IndirectDrawIndexedArgumentsPacked();
            args.FirstInstance = WorldBuffer.AllocationStart / (uint)sizeof(InstanceData);
            args.VertexOffset = (int)(GeometryBuffer.VAllocationStart / 28);
            args.InstanceCount = 1;
            args.FirstIndex = GeometryBuffer.IAllocationStart / 2;
            args.IndexCount = GeometryBuffer.IAllocationSize / 2;
            //encoder.AddDraw(ref args, GeomBuffer.BufferIndex, RenderPipeline, PerObjRS, IndexFormat.UInt16);
        }

        public void Draw(Renderer.IndirectDrawEncoder encoder, Scene.SceneRenderPipeline sp, IDbgPrim parentPrim, Matrix4x4 world)
        {
            DrawPrimitive(encoder, sp);
        }

        protected abstract void DisposeBuffers();

        public void SubmitRenderObjects(Renderer.RenderQueue queue)
        {
            //ulong code = RenderPipeline != null ? (ulong)RenderPipeline.GetHashCode() : 0;
            //queue.Add(this, RenderKey.Create((int)(code & 0xFFFFFFFF), (uint)BufferIndex));
        }

        public virtual BoundingBox GetBounds()
        {
            return new BoundingBox();
        }

        public virtual bool RayCast(Ray ray, out float dist)
        {
            dist = float.MaxValue;
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DbgPrim()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
