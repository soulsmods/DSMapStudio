using StudioCore.Resource;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Utilities;
using Vortice.Vulkan;

namespace StudioCore.DebugPrimitives
{
    public interface IDbgPrim : IDisposable
    {
        string Name { get; set; }
        Color NameColor { get; set; }

        DbgPrimCategory Category { get; set; }

        /// <summary>
        /// Underlying layout type of the mesh data
        /// </summary>
        public abstract MeshLayoutType LayoutType { get; }

        public abstract VertexLayoutDescription LayoutDescription { get; }

        public abstract BoundingBox Bounds { get; }

        /// <summary>
        /// Get handle to the GPU allocated geometry
        /// </summary>
        public abstract Scene.VertexIndexBufferAllocator.VertexIndexBufferHandle GeometryBuffer { get; }

        // Pipeline state
        public abstract string ShaderName { get; }

        public abstract SpecializationConstant[] SpecializationConstants { get; }

        public abstract VkCullModeFlags CullMode { get; }

        public abstract VkPolygonMode FillMode { get; }

        public virtual VkFrontFace FrontFace { get => VkFrontFace.CounterClockwise; }

        public abstract VkPrimitiveTopology Topology { get; }

        // Mesh data
        public virtual bool Is32Bit { get => false; }

        public virtual int IndexOffset { get => 0; }

        public abstract int IndexCount { get; }

        public abstract uint VertexSize { get; }
    }
}
