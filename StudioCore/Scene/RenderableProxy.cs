using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using StudioCore.Resource;

namespace StudioCore.Scene
{
    /// <summary>
    /// Proxy class that represents a connection between the logical scene
    /// heirarchy and the actual underlying renderable representation. Used to handle
    /// things like renderable construction, selection, hiding/showing, etc
    /// </summary>
    public abstract class RenderableProxy : Renderer.IRendererUpdatable
    {
        public abstract void ConstructRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);
        public abstract void DestroyRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);
        public abstract void UpdateRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);

        protected void ScheduleRenderableConstruction()
        {
            Renderer.AddBackgroundUploadTask((gd, cl) =>
            {
                ConstructRenderables(gd, cl, null);
            });
        }

        protected void ScheduleRenderableUpdate()
        {
            Renderer.AddBackgroundUploadTask((gd, cl) =>
            {
                UpdateRenderables(gd, cl, null);
            });
        }
    }

    /// <summary>
    /// Render proxy for a single static mesh
    /// </summary>
    public class MeshRenderableProxy : RenderableProxy, IMeshProviderEventListener
    {
        private MeshProvider _meshProvider;
        private MeshRenderables _renderablesSet;

        private List<MeshRenderableProxy> _submeshes = new List<MeshRenderableProxy>();

        private int _renderable = -1;

        protected Pipeline _pipeline;
        protected Shader[] _shaders;
        protected GPUBufferAllocator.GPUBufferHandle _worldBuffer;
        protected ResourceSet _perObjectResourceSet;

        private Matrix4x4 _world = Matrix4x4.Identity;

        public Matrix4x4 World
        {
            get
            {
                return _world;
            }
            set
            {
                _world = value;
                ScheduleRenderableUpdate();
                foreach (var sm in _submeshes)
                {
                    sm.World = _world;
                }
            }
        }

        public MeshRenderableProxy(MeshRenderables renderables, MeshProvider provider)
        {
            _renderablesSet = renderables;
            _meshProvider = provider;
            _meshProvider.AddEventListener(this);
            ScheduleRenderableConstruction();
        }

        public unsafe override void ConstructRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (_renderable != -1)
            {
                _renderablesSet.RemoveRenderable(_renderable);
                _renderable = -1;
            }
            if (_meshProvider.TryLock())
            {
                if (!_meshProvider.IsAvailable() || !_meshProvider.HasMeshData())
                {
                    _meshProvider.Unlock();
                    return;
                }

                if (_meshProvider.GeometryBuffer.AllocStatus != VertexIndexBufferAllocator.VertexIndexBufferHandle.Status.Resident)
                {
                    ScheduleRenderableConstruction();
                    _meshProvider.Unlock();
                    return;
                }

                var factory = gd.ResourceFactory;
                if (_worldBuffer == null)
                {
                    _worldBuffer = Renderer.UniformBufferAllocator.Allocate(128, 128);
                }
                InstanceData dat = new InstanceData();
                dat.WorldMatrix = _world;
                dat.MaterialID = _meshProvider.MaterialIndex;
                _worldBuffer.FillBuffer(cl, ref dat);

                // Construct pipeline
                ResourceLayout projViewCombinedLayout = StaticResourceCache.GetResourceLayout(
                    gd.ResourceFactory,
                    new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

                ResourceLayout worldLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding)));


                VertexLayoutDescription[] mainVertexLayouts = new VertexLayoutDescription[]
                {
                    _meshProvider.LayoutDescription
                };

                var res = StaticResourceCache.GetShaders(gd, gd.ResourceFactory, _meshProvider.ShaderName).ToTuple();
                _shaders = new Shader[] { res.Item1, res.Item2 };

                ResourceLayout projViewLayout = StaticResourceCache.GetResourceLayout(
                    gd.ResourceFactory,
                    StaticResourceCache.SceneParamLayoutDescription);

                ResourceLayout mainPerObjectLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.StructuredBufferReadWrite, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)));

                ResourceLayout texLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("globalTextures", ResourceKind.TextureReadOnly, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)));

                _perObjectResourceSet = StaticResourceCache.GetResourceSet(factory, new ResourceSetDescription(mainPerObjectLayout,
                    Renderer.UniformBufferAllocator._backingBuffer));

                GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
                pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
                pipelineDescription.DepthStencilState = DepthStencilStateDescription.DepthOnlyGreaterEqual;
                pipelineDescription.RasterizerState = new RasterizerStateDescription(
                    cullMode: _meshProvider.CullMode,
                    fillMode: _meshProvider.FillMode,
                    frontFace: _meshProvider.FrontFace,
                    depthClipEnabled: true,
                    scissorTestEnabled: false);
                pipelineDescription.PrimitiveTopology = _meshProvider.Topology;
                pipelineDescription.ShaderSet = new ShaderSetDescription(
                    vertexLayouts: mainVertexLayouts,
                    shaders: _shaders, _meshProvider.SpecializationConstants);
                pipelineDescription.ResourceLayouts = new ResourceLayout[] { projViewLayout, mainPerObjectLayout, Renderer.GlobalTexturePool.GetLayout(), Renderer.GlobalCubeTexturePool.GetLayout(), Renderer.MaterialBufferAllocator.GetLayout(), SamplerSet.SamplersLayout };
                pipelineDescription.Outputs = gd.SwapchainFramebuffer.OutputDescription;
                _pipeline = StaticResourceCache.GetPipeline(factory, ref pipelineDescription);

                // Create draw call arguments
                var meshcomp = new MeshDrawParametersComponent();
                var geombuffer = _meshProvider.GeometryBuffer;
                uint indexStart = geombuffer.IAllocationStart / (_meshProvider.Is32Bit ? 4u : 2u) + (uint)_meshProvider.IndexOffset;
                meshcomp._indirectArgs.FirstInstance = _worldBuffer.AllocationStart / (uint)sizeof(InstanceData);
                meshcomp._indirectArgs.VertexOffset = (int)(geombuffer.VAllocationStart / _meshProvider.VertexSize);
                meshcomp._indirectArgs.InstanceCount = 1;
                meshcomp._indirectArgs.FirstIndex = indexStart;
                meshcomp._indirectArgs.IndexCount = (uint)_meshProvider.IndexCount;

                // Rest of draw parameters
                meshcomp._indexFormat = _meshProvider.Is32Bit ? IndexFormat.UInt32 : IndexFormat.UInt16;
                meshcomp._pipeline = _pipeline;
                meshcomp._objectResourceSet = _perObjectResourceSet;
                meshcomp._bufferIndex = geombuffer.BufferIndex;

                // Instantiate renderable
                var bounds = BoundingBox.Transform(_meshProvider.Bounds, _world);
                _renderable = _renderablesSet.CreateMesh(ref bounds, ref meshcomp);
                _renderablesSet.cRenderKeys[_renderable] = GetRenderKey(0.0f);

                _meshProvider.Unlock();
            }
        }

        public override void UpdateRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (_meshProvider.TryLock())
            {
                if (!_meshProvider.IsAvailable() || !_meshProvider.HasMeshData())
                {
                    _meshProvider.Unlock();
                    return;
                }
                InstanceData dat = new InstanceData();
                dat.WorldMatrix = _world;
                dat.MaterialID = _meshProvider.MaterialIndex;
                if (_worldBuffer == null)
                {
                    _worldBuffer = Renderer.UniformBufferAllocator.Allocate(128, 128);
                }
                _worldBuffer.FillBuffer(cl, ref dat);
            }
        }

        public override void DestroyRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderAvailable()
        {
            if (_meshProvider.TryLock())
            {
                for (int i = 0; i < _meshProvider.ChildCount; i++)
                {
                    var child = new MeshRenderableProxy(_renderablesSet, _meshProvider.GetChildProvider(i));
                    child.World = _world;
                    _submeshes.Add(child);
                }
                _meshProvider.Unlock();
            }
        }

        public void OnProviderUnavailable()
        {
            if (_meshProvider.IsAtomic())
            {
                _submeshes.Clear();
            }
        }

        public RenderKey GetRenderKey(float distance)
        {
            ulong code = _pipeline != null ? (ulong)_pipeline.GetHashCode() : 0;
            ulong index = 0;

            uint cameraDistanceInt = (uint)Math.Min(uint.MaxValue, (distance * 1000f));

            if (_meshProvider.IsAvailable())
            {
                if (_meshProvider.TryLock())
                {
                    index = _meshProvider.Is32Bit ? 1u : 0;
                    _meshProvider.Unlock();
                }
            }

            return new RenderKey((code << 41) | (index << 40) | ((ulong)(_renderablesSet.cDrawParameters[_renderable]._bufferIndex & 0xFF) << 32) + cameraDistanceInt);
        }

        public static MeshRenderableProxy MeshRenderableFromFlverResource(RenderScene scene, ResourceHandle<FlverResource> handle)
        {
            var renderable = new MeshRenderableProxy(scene.OpaqueRenderables, MeshProviderCache.GetFlverMeshProvider(handle));
            return renderable;
        }
    }
}
