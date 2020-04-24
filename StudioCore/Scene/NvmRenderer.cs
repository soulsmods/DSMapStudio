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
    public class NvmRenderer : RenderObject, IDisposable
    {
        public BoundingBox Bounds;

        private bool HasNoLODs = true;

        private Resource.ResourceHandle<Resource.NVMNavmeshResource> NvmResource;

        
        private DeviceBuffer IndexBuffer = null;
        private DeviceBuffer VertBuffer = null;

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
        protected Pipeline RenderWirePipeline;
        protected Shader[] Shaders;
        protected Shader[] ShadersWire;
        protected GPUBufferAllocator.GPUBufferHandle WorldBuffer;
        protected ResourceSet PerObjRS;

        public int VertexCount { get; private set; }

        public readonly CollisionMesh Parent;

        public bool IsVisible { get; set; } = true;

        private int bufferIndexCached = -1;
        public int BufferIndex
        {
            get
            {
                if (bufferIndexCached != -1)
                {
                    return bufferIndexCached;
                }
                if (NvmResource != null && NvmResource.IsLoaded && NvmResource.Get() != null)
                {
                    if (NvmResource.Get().GeomBuffer.AllocStatus == VertexIndexBufferAllocator.VertexIndexBufferHandle.Status.Resident)
                    {
                        bufferIndexCached = NvmResource.Get().GeomBuffer.BufferIndex;
                        return bufferIndexCached;
                    }
                }
                return 0;
            }
        }

        public NvmRenderer(NvmMesh parent, Resource.ResourceHandle<Resource.NVMNavmeshResource> resourceHandle)
        {
            NvmResource = resourceHandle;
        }

        /// <summary>
        /// Construct navmesh renderer from output of recast
        /// </summary>
        unsafe public NvmRenderer(Vector3 bbmin, Vector3 bbmax, float cs, float ch, ushort[] verts, ushort[] indices)
        {
            Bounds = new BoundingBox(bbmin, bbmax);

            var MeshIndices = new int[indices.Length / 2];
            var MeshVertices = new Resource.CollisionLayout[indices.Length / 2];

            var factory = Scene.Renderer.Factory;

            for (int i = 0; i < indices.Length / 2; i += 3)
            {
                var v1x = verts[indices[i*2]*3];
                var v1y = verts[indices[i*2]*3 + 1];
                var v1z = verts[indices[i*2]*3 + 2];
                var v2x = verts[indices[i*2+1]*3];
                var v2y = verts[indices[i*2+1]*3 + 1];
                var v2z = verts[indices[i*2+1]*3 + 2];
                var v3x = verts[indices[i*2+2]*3];
                var v3y = verts[indices[i*2+2]*3 + 1];
                var v3z = verts[indices[i*2+2]*3 + 2];

                var vert1 = new Vector3(bbmin.X + (float)v1x * cs,
                                        bbmin.Y + (float)v1y * ch,
                                        bbmin.Z + (float)v1z * cs);
                var vert2 = new Vector3(bbmin.X + (float)v2x * cs,
                                        bbmin.Y + (float)v2y * ch,
                                        bbmin.Z + (float)v2z * cs);
                var vert3 = new Vector3(bbmin.X + (float)v3x * cs,
                                        bbmin.Y + (float)v3y * ch,
                                        bbmin.Z + (float)v3z * cs);

                MeshVertices[i] = new Resource.CollisionLayout();
                MeshVertices[i + 1] = new Resource.CollisionLayout();
                MeshVertices[i + 2] = new Resource.CollisionLayout();

                MeshVertices[i].Position = new Vector3(vert1.X, vert1.Y, vert1.Z);
                MeshVertices[i + 1].Position = new Vector3(vert2.X, vert2.Y, vert2.Z);
                MeshVertices[i + 2].Position = new Vector3(vert3.X, vert3.Y, vert3.Z);
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

                MeshVertices[i].Color[0] = (byte)(157);
                MeshVertices[i].Color[1] = (byte)(53);
                MeshVertices[i].Color[2] = (byte)(255);
                MeshVertices[i].Color[3] = (byte)(255);
                MeshVertices[i + 1].Color[0] = (byte)(157);
                MeshVertices[i + 1].Color[1] = (byte)(53);
                MeshVertices[i + 1].Color[2] = (byte)(255);
                MeshVertices[i + 1].Color[3] = (byte)(255);
                MeshVertices[i + 2].Color[0] = (byte)(157);
                MeshVertices[i + 2].Color[1] = (byte)(53);
                MeshVertices[i + 2].Color[2] = (byte)(255);
                MeshVertices[i + 2].Color[3] = (byte)(255);

                MeshIndices[i] = i;
                MeshIndices[i + 1] = i + 1;
                MeshIndices[i + 2] = i + 2;
            }

            var icount = MeshIndices.Length;

            uint buffersize = (uint)icount * 4u;
            IndexBuffer = factory.CreateBuffer(new BufferDescription(buffersize, BufferUsage.IndexBuffer));
            Scene.Renderer.AddBackgroundUploadTask((device, cl) =>
            {
                cl.UpdateBuffer(IndexBuffer, 0, MeshIndices);
            });

            uint vbuffersize = (uint)MeshVertices.Length * Resource.CollisionLayout.SizeInBytes;
            VertBuffer = factory.CreateBuffer(new BufferDescription(vbuffersize, BufferUsage.VertexBuffer));

            Scene.Renderer.AddBackgroundUploadTask((d, cl) =>
            {
                cl.UpdateBuffer(VertBuffer, 0, MeshVertices);
                MeshVertices = null;
            });
        }

        public override void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            var factory = gd.ResourceFactory;
            WorldBuffer = Renderer.UniformBufferAllocator.Allocate(128, 128);
            WorldBuffer.FillBuffer(cl, ref _World);

            VertexLayoutDescription[] mainVertexLayouts = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Float3),
                    new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.SByte4),
                    new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4))
            };

            var res = StaticResourceCache.GetShaders(gd, gd.ResourceFactory, "NavSolid").ToTuple();
            Shaders = new Shader[] { res.Item1, res.Item2 };

            var res2 = StaticResourceCache.GetShaders(gd, gd.ResourceFactory, "NavWire").ToTuple();
            ShadersWire = new Shader[] { res2.Item1, res2.Item2 };

            ResourceLayout projViewLayout = StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                StaticResourceCache.SceneParamLayoutDescription);

            ResourceLayout mainPerObjectLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.StructuredBufferReadWrite, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)));

            PerObjRS = StaticResourceCache.GetResourceSet(factory, new ResourceSetDescription(mainPerObjectLayout,
                Renderer.UniformBufferAllocator._backingBuffer));

            bool isTriStrip = false;

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = DepthStencilStateDescription.DepthOnlyGreaterEqual;
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.None,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.CounterClockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = isTriStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: mainVertexLayouts,
                shaders: Shaders);
            pipelineDescription.ResourceLayouts = new ResourceLayout[] { projViewLayout, mainPerObjectLayout, Renderer.GlobalTexturePool.GetLayout(), Renderer.GlobalCubeTexturePool.GetLayout(), Renderer.MaterialBufferAllocator.GetLayout(), SamplerSet.SamplersLayout };
            pipelineDescription.Outputs = gd.SwapchainFramebuffer.OutputDescription;
            RenderPipeline = StaticResourceCache.GetPipeline(factory, ref pipelineDescription);

            pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = DepthStencilStateDescription.DepthOnlyGreaterEqual;
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.None,
                fillMode: PolygonFillMode.Wireframe,
                frontFace: FrontFace.CounterClockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = isTriStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: mainVertexLayouts,
                shaders: ShadersWire);
            pipelineDescription.ResourceLayouts = new ResourceLayout[] { projViewLayout, mainPerObjectLayout, Renderer.GlobalTexturePool.GetLayout(), Renderer.MaterialBufferAllocator.GetLayout(), SamplerSet.SamplersLayout };
            pipelineDescription.Outputs = gd.SwapchainFramebuffer.OutputDescription;
            RenderWirePipeline = StaticResourceCache.GetPipeline(factory, ref pipelineDescription);
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

            if (NvmResource == null || !NvmResource.IsLoaded || NvmResource.Get() == null)
                return;

            if (NvmResource.TryLock())
            {
                var resource = NvmResource.Get();
                var geombuffer = resource.GeomBuffer;

                if (geombuffer.AllocStatus != VertexIndexBufferAllocator.VertexIndexBufferHandle.Status.Resident)
                {
                    NvmResource.Unlock();
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
                NvmResource.Unlock();
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
