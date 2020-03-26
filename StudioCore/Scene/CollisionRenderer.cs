using StudioCore.DebugPrimitives;
//using MeowDSIO.DataTypes.FLVER;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using StudioCore.Scene;

namespace StudioCore.Scene
{
    public class CollisionRenderer : RenderObject, IDisposable
    {
        public BoundingBox Bounds;

        private bool HasNoLODs = true;

        private Resource.ResourceHandle<Resource.HavokCollisionResource> ColResource;
        private int ColMeshIndex;

        private bool WindClockwise = false;

        private int bufferIndexCached = -1;
        public int BufferIndex
        {
            get
            {
                if (bufferIndexCached != -1)
                {
                    return bufferIndexCached;
                }
                if (ColResource != null && ColResource.IsLoaded && ColResource.Get() != null)
                {
                    if (ColResource.Get().GPUMeshes[ColMeshIndex].GeomBuffer.AllocStatus == VertexIndexBufferAllocator.VertexIndexBufferHandle.Status.Resident)
                    {
                        bufferIndexCached = ColResource.Get().GPUMeshes[ColMeshIndex].GeomBuffer.BufferIndex;
                        return bufferIndexCached;
                    }
                }
                return 0;
            }
        }

        bool WorldDirty = false;
        private Matrix4x4 _World = Matrix4x4.Identity;
        public Matrix4x4 WorldTransform
        {
            get
            {
                return _World;
            }
            set
            {
                _World = value;
                WorldDirty = true;
            }
        }

        protected Pipeline RenderPipeline;
        protected Shader[] Shaders;
        protected GPUBufferAllocator.GPUBufferHandle WorldBuffer;
        protected ResourceSet PerObjRS;

        public int VertexCount { get; private set; }

        public readonly CollisionMesh Parent;

        public bool IsVisible { get; set; } = true;

        public CollisionRenderer(CollisionMesh parent, Resource.ResourceHandle<Resource.HavokCollisionResource> resourceHandle, int meshIndex, bool windCW)
        {
            ColResource = resourceHandle;
            ColMeshIndex = meshIndex;
            WindClockwise = windCW;
        }

        public override void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            var factory = gd.ResourceFactory;
            WorldBuffer = Renderer.UniformBufferAllocator.Allocate(128, 128);
            WorldBuffer.FillBuffer(cl, ref _World);

            ResourceLayout projViewCombinedLayout = StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            ResourceLayout worldLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding)));

            VertexLayoutDescription[] mainVertexLayouts = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Float3),
                    new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.SByte4),
                    new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4))
            };

            var res = StaticResourceCache.GetShaders(gd, gd.ResourceFactory, "Collision").ToTuple();
            Shaders = new Shader[] { res.Item1, res.Item2 };

            ResourceLayout projViewLayout = StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                StaticResourceCache.ProjViewLayoutDescription);

            ResourceLayout mainPerObjectLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.StructuredBufferReadWrite, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)));

            PerObjRS = StaticResourceCache.GetResourceSet(factory, new ResourceSetDescription(mainPerObjectLayout,
                Renderer.UniformBufferAllocator._backingBuffer));

            bool isTriStrip = false;
            var fres = ColResource.Get();
            if (fres != null)
            {
                var mesh = fres.GPUMeshes[ColMeshIndex];
            }

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = DepthStencilStateDescription.DepthOnlyGreaterEqual;
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: WindClockwise ? FrontFace.Clockwise : FrontFace.CounterClockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = isTriStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: mainVertexLayouts,
                shaders: Shaders);
            pipelineDescription.ResourceLayouts = new ResourceLayout[] { projViewLayout, mainPerObjectLayout, Renderer.GlobalTexturePool.GetLayout() };
            pipelineDescription.Outputs = gd.SwapchainFramebuffer.OutputDescription;
            RenderPipeline = StaticResourceCache.GetPipeline(factory, ref pipelineDescription);
        }

        public override void DestroyDeviceObjects()
        {
            //throw new NotImplementedException();
        }

        public override void UpdatePerFrameResources(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (WorldDirty)
            {
                WorldBuffer.FillBuffer(cl, ref _World);
                WorldDirty = false;
            }
        }

        unsafe public override void Render(Renderer.IndirectDrawEncoder encoder, SceneRenderPipeline sp)
        {
            if (!IsVisible)
                return;

            if (ColResource == null || !ColResource.IsLoaded || ColResource.Get() == null)
                return;

            if (ColResource.TryLock())
            {
                var resource = ColResource.Get();
                var mesh = resource.GPUMeshes[ColMeshIndex];
                var geombuffer = mesh.GeomBuffer;

                if (geombuffer.AllocStatus != VertexIndexBufferAllocator.VertexIndexBufferHandle.Status.Resident)
                {
                    ColResource.Unlock();
                    return;
                }

                uint indexStart = geombuffer.IAllocationStart / 4u;
                var args = new Renderer.IndirectDrawIndexedArgumentsPacked();
                args.FirstInstance = WorldBuffer.AllocationStart / (uint)sizeof(InstanceData);
                args.VertexOffset = (int)(geombuffer.VAllocationStart / Resource.CollisionLayout.SizeInBytes);
                args.InstanceCount = 1;
                args.FirstIndex = indexStart;
                args.IndexCount = geombuffer.IAllocationSize / 4u;
                encoder.AddDraw(ref args, geombuffer.BufferIndex, RenderPipeline, PerObjRS, IndexFormat.UInt32);
                ColResource.Unlock();
            }
        }

        public override Pipeline GetPipeline()
        {
            return RenderPipeline;
        }

        public void Dispose()
        {

            //VertBuffer.Dispose();

            // Just leave the texture data as-is, since 
            // TexturePool handles memory cleanup
        }
    }
}
