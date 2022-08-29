using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using StudioCore.Resource;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using StudioCore.DebugPrimitives;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace StudioCore.Scene
{
    /// <summary>
    /// Proxy class that represents a connection between the logical scene
    /// heirarchy and the actual underlying renderable representation. Used to handle
    /// things like renderable construction, selection, hiding/showing, etc
    /// </summary>
    public abstract class RenderableProxy : Renderer.IRendererUpdatable, IDisposable
    {
        private bool disposedValue;
        
        public abstract void ConstructRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);
        public abstract void DestroyRenderables();
        public abstract void UpdateRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);

        public abstract void SetSelectable(ISelectable sel);

        public abstract bool RenderSelectionOutline { set; get; }

        public abstract Matrix4x4 World { get; set; }

        public abstract bool Visible { get; set; }

        public abstract RenderFilter DrawFilter { get; set; }
        public abstract DrawGroup DrawGroups { get; set; }

        protected bool _autoregister = true;
        public virtual bool AutoRegister { get => _autoregister; set => _autoregister = value; }

        protected bool _registered = false;

        public abstract BoundingBox GetBounds();

        public abstract BoundingBox GetLocalBounds();

        internal void ScheduleRenderableConstruction()
        {
            Renderer.AddLowPriorityBackgroundUploadTask((gd, cl) =>
            {
                var ctx = Tracy.TracyCZoneN(1, $@"Renderable construction");
                ConstructRenderables(gd, cl, null);
                Tracy.TracyCZoneEnd(ctx);
            });
        }

        internal void ScheduleRenderableUpdate()
        {
            Renderer.AddLowPriorityBackgroundUploadTask((gd, cl) =>
            {
                var ctx = Tracy.TracyCZoneN(1, $@"Renderable update");
                UpdateRenderables(gd, cl, null);
                Tracy.TracyCZoneEnd(ctx);
            });
        }

        public virtual void Register()
        {
            if (_registered)
            {
                return;
            }
            _registered = true;
            ScheduleRenderableConstruction();
        }

        public virtual void UnregisterWithScene()
        {
            if (_registered)
            {
                _registered = false;
                DestroyRenderables();
            }
        }

        public virtual void UnregisterAndRelease()
        {
            if (_registered)
            {
                UnregisterWithScene();
            }
            /*if (Resource != null)
            {
                Resource.Release();
            }
            Resource = null;
            Created = false;
            Submeshes = null;*/
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                UnregisterAndRelease();
                disposedValue = true;
            }
        }

        protected uint GetPackedEntityID(int system, int index)
        {
            return (((uint)system) << 30) | ((uint)index) & 0x3FFFFFFF;
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~RenderableProxy()
        {
             // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
             Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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

        public IReadOnlyList<MeshRenderableProxy> Submeshes { get => _submeshes; }

        private int _renderable = -1;
        private int _selectionOutlineRenderable = -1;

        protected Pipeline _pipeline;
        protected Pipeline _pickingPipeline;
        protected Pipeline _selectedPipeline;
        protected Shader[] _shaders;
        protected GPUBufferAllocator.GPUBufferHandle _worldBuffer;
        protected ResourceSet _perObjectResourceSet;

        private Matrix4x4 _world = Matrix4x4.Identity;

        private WeakReference<ISelectable> _selectable = null;

        public IResourceHandle ResourceHandle
        {
            get
            {
                return _meshProvider.ResourceHandle;
            }
        }

        public override bool AutoRegister
        {
            get => _autoregister; set
            {
                _autoregister = value;
                foreach (var c in _submeshes)
                {
                    c.AutoRegister = value;
                }
            }
        }

        public override Matrix4x4 World
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

        private bool _visible = true;
        public override bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
                if (_renderable != -1)
                {
                    _renderablesSet.cVisible[_renderable]._visible = value;
                    if (!_meshProvider.SelectedRenderBaseMesh && _renderOutline)
                    {
                        _renderablesSet.cVisible[_renderable]._visible = false;
                    }
                }
                foreach (var sm in _submeshes)
                {
                    sm.Visible = _visible;
                }
            }
        }

        private RenderFilter _drawfilter = RenderFilter.All;
        public override RenderFilter DrawFilter
        {
            get
            {
                return _drawfilter;
            }
            set
            {
                _drawfilter = value;
                if (_renderable != -1)
                {
                    _renderablesSet.cSceneVis[_renderable]._renderFilter = value;
                }
                foreach (var sm in _submeshes)
                {
                    sm.DrawFilter = _drawfilter;
                }
            }
        }

        private DrawGroup _drawgroups = new DrawGroup();
        public override DrawGroup DrawGroups
        {
            get
            {
                return _drawgroups;
            }
            set
            {
                _drawgroups = value;
                if (_renderable != -1)
                {
                    _renderablesSet.cSceneVis[_renderable]._drawGroup = value;
                }
                foreach (var sm in _submeshes)
                {
                    sm.DrawGroups = _drawgroups;
                }
            }
        }

        private bool _renderOutline = false;

        public override bool RenderSelectionOutline
        {
            get => _renderOutline;
            set
            {
                _renderOutline = value;
                if (_registered)
                {
                    ScheduleRenderableConstruction();
                }
                foreach (var child in _submeshes)
                {
                    child.RenderSelectionOutline = value;
                }
            }
        }

        public override BoundingBox GetBounds()
        {
            return BoundingBox.Transform(GetLocalBounds(), _world);
        }

        public override BoundingBox GetLocalBounds()
        {
            if (_meshProvider.IsAvailable() && _meshProvider.HasMeshData() && _meshProvider.TryLock())
            {
                _meshProvider.Unlock();
                return BoundingBox.Transform(_meshProvider.Bounds, _meshProvider.ObjectTransform);
            }
            BoundingBox b = _submeshes.Count > 0 ? _submeshes[0].GetLocalBounds() : new BoundingBox();
            foreach (var c in _submeshes)
            {
                b = BoundingBox.Combine(b, c.GetLocalBounds());
            }
            return b;
        }

        public MeshRenderableProxy(MeshRenderables renderables, MeshProvider provider, bool autoregister=true)
        {
            AutoRegister = autoregister;
            _renderablesSet = renderables;
            _meshProvider = provider;
            _meshProvider.AddEventListener(this);
            _meshProvider.Acquire();
            if (autoregister)
            {
                _registered = true;
                ScheduleRenderableConstruction();
            }
        }

        public MeshRenderableProxy(MeshRenderableProxy clone) : this(clone._renderablesSet, clone._meshProvider)
        {
            DrawFilter = clone._drawfilter;
        }

        public override void Register()
        {
            if (_registered)
            {
                return;
            }
            foreach (var c in _submeshes)
            {
                c.Register();
            }
            _registered = true;
            ScheduleRenderableConstruction();
        }

        public override void UnregisterWithScene()
        {
            if (_registered)
            {
                _registered = false;
                DestroyRenderables();
            }
            foreach (var c in _submeshes)
            {
                c.UnregisterWithScene();
            }
        }

        public override void UnregisterAndRelease()
        {
            if (_registered)
            {
                UnregisterWithScene();
            }
            foreach (var c in _submeshes)
            {
                c.UnregisterAndRelease();
            }
            if (_meshProvider != null)
            {
                _meshProvider.Release();
            }
            if (_worldBuffer != null)
            {
                _worldBuffer.Dispose();
                _worldBuffer = null;
            }
        }

        public unsafe override void ConstructRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            foreach (var c in _submeshes)
            {
                if (c._registered)
                {
                    c.ScheduleRenderableConstruction();
                }
            }
            if (_renderable != -1)
            {
                _renderablesSet.RemoveRenderable(_renderable);
                _renderable = -1;
            }
            if (_selectionOutlineRenderable != -1)
            {
                _renderablesSet.RemoveRenderable(_selectionOutlineRenderable);
                _selectionOutlineRenderable = -1;
            }
            if (_meshProvider.TryLock())
            {
                if (!_meshProvider.IsAvailable() || !_meshProvider.HasMeshData())
                {
                    _meshProvider.Unlock();
                    return;
                }

                if (_meshProvider.GeometryBuffer == null)
                {
                    _meshProvider.Unlock();
                    return;
                }

                if (_meshProvider.GeometryBuffer.AllocStatus != VertexIndexBufferAllocator.VertexIndexBuffer.Status.Resident)
                {
                    ScheduleRenderableConstruction();
                    _meshProvider.Unlock();
                    return;
                }

                var factory = gd.ResourceFactory;
                if (_worldBuffer == null)
                {
                    _worldBuffer = Renderer.UniformBufferAllocator.Allocate((uint)sizeof(InstanceData), sizeof(InstanceData));
                }

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

                ResourceLayout pickingResultLayout = StaticResourceCache.GetResourceLayout(
                    gd.ResourceFactory,
                    StaticResourceCache.PickingResultDescription);

                ResourceLayout mainPerObjectLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.StructuredBufferReadWrite, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)));

                ResourceLayout texLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("globalTextures", ResourceKind.TextureReadOnly, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)));

                _perObjectResourceSet = StaticResourceCache.GetResourceSet(factory, new ResourceSetDescription(mainPerObjectLayout,
                    Renderer.UniformBufferAllocator._backingBuffer));

                // Build default pipeline
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
                pipelineDescription.ResourceLayouts = new ResourceLayout[]
                { 
                    projViewLayout,
                    mainPerObjectLayout,
                    Renderer.GlobalTexturePool.GetLayout(),
                    Renderer.GlobalCubeTexturePool.GetLayout(),
                    Renderer.MaterialBufferAllocator.GetLayout(),
                    SamplerSet.SamplersLayout,
                    pickingResultLayout,
                    Renderer.BoneBufferAllocator.GetLayout(),
                };
                pipelineDescription.Outputs = gd.SwapchainFramebuffer.OutputDescription;
                _pipeline = StaticResourceCache.GetPipeline(factory, ref pipelineDescription);

                // Build picking pipeline
                var pickingSpecializationConstants = new SpecializationConstant[_meshProvider.SpecializationConstants.Length + 1];
                Array.Copy(_meshProvider.SpecializationConstants, pickingSpecializationConstants, _meshProvider.SpecializationConstants.Length);
                pickingSpecializationConstants[pickingSpecializationConstants.Length - 1] = new SpecializationConstant(99, true);
                pipelineDescription.ShaderSet = new ShaderSetDescription(
                    vertexLayouts: mainVertexLayouts,
                    shaders: _shaders, pickingSpecializationConstants);
                _pickingPipeline = StaticResourceCache.GetPipeline(factory, ref pipelineDescription);

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
                meshcomp._objectResourceSet = _perObjectResourceSet;
                meshcomp._bufferIndex = geombuffer.BufferIndex;

                // Instantiate renderable
                var bounds = BoundingBox.Transform(_meshProvider.Bounds, _meshProvider.ObjectTransform * _world);
                _renderable = _renderablesSet.CreateMesh(ref bounds, ref meshcomp);
                _renderablesSet.cRenderKeys[_renderable] = GetRenderKey(0.0f);

                // Pipelines
                _renderablesSet.cPipelines[_renderable] = _pipeline;
                _renderablesSet.cSelectionPipelines[_renderable] = _pickingPipeline;

                // Update instance data
                InstanceData dat = new InstanceData();
                dat.WorldMatrix = _meshProvider.ObjectTransform * _world;
                dat.MaterialID = _meshProvider.MaterialIndex;
                if (_meshProvider.BoneBuffer != null)
                {
                    dat.BoneStartIndex = _meshProvider.BoneBuffer.AllocationStart / 64;
                }
                else
                {
                    dat.BoneStartIndex = 0;
                }
                dat.EntityID = GetPackedEntityID(_renderablesSet.RenderableSystemIndex, _renderable);
                _worldBuffer.FillBuffer(gd, cl, ref dat);

                // Selectable
                _renderablesSet.cSelectables[_renderable] = _selectable;

                // Visible
                _renderablesSet.cVisible[_renderable]._visible = _visible;
                if (!_meshProvider.SelectedRenderBaseMesh && _renderOutline)
                {
                    _renderablesSet.cVisible[_renderable]._visible = false;
                }
                _renderablesSet.cSceneVis[_renderable]._renderFilter = _drawfilter;
                _renderablesSet.cSceneVis[_renderable]._drawGroup = _drawgroups;

                // Build mesh for selection outline
                if (_renderOutline)
                {
                    pipelineDescription.RasterizerState = new RasterizerStateDescription(
                        cullMode: _meshProvider.SelectedUseBackface ?
                            ((_meshProvider.CullMode == FaceCullMode.Front) ? FaceCullMode.Back : FaceCullMode.Front) :
                            _meshProvider.CullMode,
                        fillMode: _meshProvider.FillMode,
                        frontFace: _meshProvider.FrontFace,
                        depthClipEnabled: true,
                        scissorTestEnabled: false);

                    var s = StaticResourceCache.GetShaders(gd, gd.ResourceFactory, _meshProvider.ShaderName + (_meshProvider.UseSelectedShader ? "_selected" : "")).ToTuple();
                    _shaders = new Shader[] { s.Item1, s.Item2 };
                    pipelineDescription.ShaderSet = new ShaderSetDescription(
                        vertexLayouts: mainVertexLayouts,
                        shaders: _shaders, _meshProvider.SpecializationConstants);
                    _selectedPipeline = StaticResourceCache.GetPipeline(factory, ref pipelineDescription);

                    _selectionOutlineRenderable = _renderablesSet.CreateMesh(ref bounds, ref meshcomp);
                    _renderablesSet.cRenderKeys[_selectionOutlineRenderable] = GetRenderKey(0.0f);

                    // Pipelines
                    _renderablesSet.cPipelines[_selectionOutlineRenderable] = _selectedPipeline;
                    _renderablesSet.cSelectionPipelines[_selectionOutlineRenderable] = _selectedPipeline;

                    // Selectable
                    _renderablesSet.cSelectables[_selectionOutlineRenderable] = _selectable;
                }

                _meshProvider.Unlock();
            }
        }

        public unsafe override void UpdateRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (_meshProvider.TryLock())
            {
                if (!_meshProvider.IsAvailable() || !_meshProvider.HasMeshData())
                {
                    _meshProvider.Unlock();
                    return;
                }
                InstanceData dat = new InstanceData();
                dat.WorldMatrix = _meshProvider.ObjectTransform * _world;
                dat.MaterialID = _meshProvider.MaterialIndex;
                if (_meshProvider.BoneBuffer != null)
                {
                    dat.BoneStartIndex = _meshProvider.BoneBuffer.AllocationStart / 64;
                }
                else
                {
                    dat.BoneStartIndex = 0;
                }
                dat.EntityID = GetPackedEntityID(_renderablesSet.RenderableSystemIndex, _renderable);
                if (_worldBuffer == null)
                {
                    _worldBuffer = Renderer.UniformBufferAllocator.Allocate((uint)sizeof(InstanceData), sizeof(InstanceData));
                }
                _worldBuffer.FillBuffer(gd, cl, ref dat);

                if (_renderable != -1)
                {
                    _renderablesSet.cBounds[_renderable] = BoundingBox.Transform(_meshProvider.Bounds, _meshProvider.ObjectTransform * _world);
                }
                if (_selectionOutlineRenderable != -1)
                {
                    _renderablesSet.cBounds[_selectionOutlineRenderable] = BoundingBox.Transform(_meshProvider.Bounds, _meshProvider.ObjectTransform * _world);
                }
                _meshProvider.Unlock();
            }
        }

        public override void DestroyRenderables()
        {
            if (_renderable != -1)
            {
                _renderablesSet.RemoveRenderable(_renderable);
                _renderable = -1;
            }
            if (_selectionOutlineRenderable != -1)
            {
                _renderablesSet.RemoveRenderable(_selectionOutlineRenderable);
                _selectionOutlineRenderable = -1;
            }
            foreach (var c in _submeshes)
            {
                c.DestroyRenderables();
            }
        }

        public override void SetSelectable(ISelectable sel)
        {
            _selectable = new WeakReference<ISelectable>(sel);
            if (_renderable != -1)
            {
                _renderablesSet.cSelectables[_renderable] = _selectable;
            }
            foreach (var child in _submeshes)
            {
                child.SetSelectable(sel);
            }
        }

        public int MeshIndexCount = 0;

        public void OnProviderAvailable()
        {
            if (_meshProvider.TryLock())
            {
                if (_meshProvider.HasMeshData())
                {
                    ScheduleRenderableConstruction();
                }
                for (int i = 0; i < _meshProvider.ChildCount; i++)
                {
                    var child = new MeshRenderableProxy(_renderablesSet, _meshProvider.GetChildProvider(i), AutoRegister);
                    child.World = _world;
                    child.Visible = _visible;
                    child.DrawFilter = _drawfilter;
                    child.DrawGroups = _drawgroups;
                    _submeshes.Add(child);
                    ISelectable sel = null;
                    if (_selectable != null)
                    {
                        _selectable.TryGetTarget(out sel);
                        if (sel != null)
                        {
                            child.SetSelectable(sel);
                        }
                    }
                }
                
                // Used for model markers for old Navmeshes.
                // Really stupid hack that's likely in a very stupid location. Can be removed when Model Marker code is improved.
                if (_meshProvider.HasMeshData())
                    MeshIndexCount = _meshProvider.IndexCount;
                else
                    MeshIndexCount = 0;
                
                _meshProvider.Unlock();
            }
        }

        public void OnProviderUnavailable()
        {
            if (_meshProvider.IsAtomic())
            {
                foreach (var c in _submeshes)
                {
                    c.UnregisterAndRelease();
                }
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

        public static MeshRenderableProxy MeshRenderableFromCollisionResource(RenderScene scene, ResourceHandle<HavokCollisionResource> handle)
        {
            var renderable = new MeshRenderableProxy(scene.OpaqueRenderables, MeshProviderCache.GetCollisionMeshProvider(handle));
            return renderable;
        }

        public static MeshRenderableProxy MeshRenderableFromNVMResource(RenderScene scene, ResourceHandle<NVMNavmeshResource> handle)
        {
            var renderable = new MeshRenderableProxy(scene.OpaqueRenderables, MeshProviderCache.GetNVMMeshProvider(handle));
            return renderable;
        }

        public static MeshRenderableProxy MeshRenderableFromHavokNavmeshResource(RenderScene scene, ResourceHandle<HavokNavmeshResource> handle, bool temp=false)
        {
            var renderable = new MeshRenderableProxy(scene.OpaqueRenderables, MeshProviderCache.GetHavokNavMeshProvider(handle, temp));
            return renderable;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DbgMaterial
    {
        private struct _color
        {
            public byte r;
            public byte g;
            public byte b;
            public byte a;
        }

        private _color _Color;
        public fixed int pad[3];

        public Color Color
        {
            set
            {
                _Color.r = value.R;
                _Color.g = value.G;
                _Color.b = value.B;
                _Color.a = value.A;
            }
        }
    }

    public class DebugPrimitiveRenderableProxy : RenderableProxy
    {
        private MeshRenderables _renderablesSet;

        private IDbgPrim _debugPrimitive;

        private int _renderable = -1;

        protected Pipeline _pipeline;
        protected Pipeline _pickingPipeline;
        protected Shader[] _shaders;
        protected GPUBufferAllocator.GPUBufferHandle _worldBuffer;
        protected GPUBufferAllocator.GPUBufferHandle _materialBuffer;
        protected ResourceSet _perObjectResourceSet;

        private Matrix4x4 _world = Matrix4x4.Identity;
        private WeakReference<ISelectable> _selectable = null;

        private Color _baseColor = Color.Gray;
        public Color BaseColor
        {
            get => _baseColor;
            set
            {
                _baseColor = value;
                ScheduleRenderableUpdate();
            }
        }
        private Color _highlightedColor = Color.Gray;
        public Color HighlightedColor
        {
            get => _highlightedColor;
            set
            {
                _highlightedColor = value;
                ScheduleRenderableUpdate();
            }
        }

        public override Matrix4x4 World
        {
            get
            {
                return _world;
            }
            set
            {
                _world = value;
                ScheduleRenderableUpdate();
            }
        }

        private bool _visible = true;
        public override bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
                if (_renderable != -1)
                {
                    _renderablesSet.cVisible[_renderable]._visible = value;
                }
            }
        }

        private RenderFilter _drawfilter = RenderFilter.All;
        public override RenderFilter DrawFilter
        {
            get
            {
                return _drawfilter;
            }
            set
            {
                _drawfilter = value;
                if (_renderable != -1)
                {
                    _renderablesSet.cSceneVis[_renderable]._renderFilter = value;
                }
            }
        }

        private DrawGroup _drawgroups = new DrawGroup();
        public override DrawGroup DrawGroups
        {
            get
            {
                return _drawgroups;
            }
            set
            {
                _drawgroups = value;
                if (_renderable != -1)
                {
                    _renderablesSet.cSceneVis[_renderable]._drawGroup = value;
                }
            }
        }

        private bool _renderOutline = false;

        public override bool RenderSelectionOutline
        {
            get => _renderOutline;
            set
            {
                bool old = _renderOutline;
                _renderOutline = value;
                if (_registered && old != _renderOutline)
                {
                    ScheduleRenderableUpdate();
                }
            }
        }

        private bool _overdraw = false;
        public bool RenderOverlay
        {
            get => _overdraw;
            set
            {
                bool old = _overdraw;
                _overdraw = true;
                if (_registered && _overdraw != old)
                {
                    ScheduleRenderableConstruction();
                }
            }
        }

        public override BoundingBox GetBounds()
        {
            return BoundingBox.Transform(_debugPrimitive.Bounds, _world);
        }

        public override BoundingBox GetLocalBounds()
        {
            return _debugPrimitive.Bounds;
        }

        public DebugPrimitiveRenderableProxy(MeshRenderables renderables, IDbgPrim prim)
        {
            _renderablesSet = renderables;
            _debugPrimitive = prim;
            ScheduleRenderableConstruction();
            AutoRegister = true;
            _registered = true;
        }

        public DebugPrimitiveRenderableProxy(DebugPrimitiveRenderableProxy clone) : this(clone._renderablesSet, clone._debugPrimitive)
        {
            _drawfilter = clone.DrawFilter;
            _baseColor = clone._baseColor;
            _highlightedColor = clone._highlightedColor;
        }

        public override void UnregisterAndRelease()
        {
            if (_registered)
            {
                UnregisterWithScene();
            }
            if (_worldBuffer != null)
            {
                _worldBuffer.Dispose();
                _worldBuffer = null;
            }
            if (_materialBuffer != null)
            {
                _materialBuffer.Dispose();
                _materialBuffer = null;
            }
        }

        public unsafe override void ConstructRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (_renderable != -1)
            {
                _renderablesSet.RemoveRenderable(_renderable);
                _renderable = -1;
            }
            if (_debugPrimitive.GeometryBuffer.AllocStatus != VertexIndexBufferAllocator.VertexIndexBuffer.Status.Resident)
            {
                ScheduleRenderableConstruction();
                return;
            }

            var factory = gd.ResourceFactory;
            if (_worldBuffer == null)
            {
                _worldBuffer = Renderer.UniformBufferAllocator.Allocate((uint)sizeof(InstanceData), sizeof(InstanceData));
            }
            if (_materialBuffer == null)
            {
                _materialBuffer = Renderer.MaterialBufferAllocator.Allocate((uint)sizeof(DbgMaterial), sizeof(DbgMaterial));
            }

            // Construct pipeline
            ResourceLayout projViewCombinedLayout = StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            ResourceLayout worldLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding)));


            VertexLayoutDescription[] mainVertexLayouts = new VertexLayoutDescription[]
            {
                _debugPrimitive.LayoutDescription
            };

            var res = StaticResourceCache.GetShaders(gd, gd.ResourceFactory, _debugPrimitive.ShaderName).ToTuple();
            _shaders = new Shader[] { res.Item1, res.Item2 };

            ResourceLayout projViewLayout = StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                StaticResourceCache.SceneParamLayoutDescription);

            ResourceLayout pickingResultLayout = StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                StaticResourceCache.PickingResultDescription);

            ResourceLayout mainPerObjectLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.StructuredBufferReadWrite, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)));

            ResourceLayout texLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("globalTextures", ResourceKind.TextureReadOnly, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)));

            _perObjectResourceSet = StaticResourceCache.GetResourceSet(factory, new ResourceSetDescription(mainPerObjectLayout,
                Renderer.UniformBufferAllocator._backingBuffer));

            // Build default pipeline
            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = DepthStencilStateDescription.DepthOnlyGreaterEqual;
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: _debugPrimitive.CullMode,
                fillMode: _debugPrimitive.FillMode,
                frontFace: _debugPrimitive.FrontFace,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = _debugPrimitive.Topology;
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: mainVertexLayouts,
                shaders: _shaders, _debugPrimitive.SpecializationConstants);
            pipelineDescription.ResourceLayouts = new ResourceLayout[]
            {
                projViewLayout,
                mainPerObjectLayout,
                Renderer.GlobalTexturePool.GetLayout(),
                Renderer.GlobalCubeTexturePool.GetLayout(),
                Renderer.MaterialBufferAllocator.GetLayout(),
                SamplerSet.SamplersLayout,
                pickingResultLayout,
                Renderer.BoneBufferAllocator.GetLayout(),
            };
            pipelineDescription.Outputs = gd.SwapchainFramebuffer.OutputDescription;
            _pipeline = StaticResourceCache.GetPipeline(factory, ref pipelineDescription);

            // Build picking pipeline
            var pickingSpecializationConstants = new SpecializationConstant[_debugPrimitive.SpecializationConstants.Length + 1];
            Array.Copy(_debugPrimitive.SpecializationConstants, pickingSpecializationConstants, _debugPrimitive.SpecializationConstants.Length);
            pickingSpecializationConstants[pickingSpecializationConstants.Length - 1] = new SpecializationConstant(99, true);
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: mainVertexLayouts,
                shaders: _shaders, pickingSpecializationConstants);
            _pickingPipeline = StaticResourceCache.GetPipeline(factory, ref pipelineDescription);

            // Create draw call arguments
            var meshcomp = new MeshDrawParametersComponent();
            var geombuffer = _debugPrimitive.GeometryBuffer;
            uint indexStart = geombuffer.IAllocationStart / (_debugPrimitive.Is32Bit ? 4u : 2u) + (uint)_debugPrimitive.IndexOffset;
            meshcomp._indirectArgs.FirstInstance = _worldBuffer.AllocationStart / (uint)sizeof(InstanceData);
            meshcomp._indirectArgs.VertexOffset = (int)(geombuffer.VAllocationStart / _debugPrimitive.VertexSize);
            meshcomp._indirectArgs.InstanceCount = 1;
            meshcomp._indirectArgs.FirstIndex = indexStart;
            meshcomp._indirectArgs.IndexCount = geombuffer.IAllocationSize / (_debugPrimitive.Is32Bit ? 4u : 2u);

            // Rest of draw parameters
            meshcomp._indexFormat = _debugPrimitive.Is32Bit ? IndexFormat.UInt32 : IndexFormat.UInt16;
            meshcomp._objectResourceSet = _perObjectResourceSet;
            meshcomp._bufferIndex = geombuffer.BufferIndex;

            // Instantiate renderable
            var bounds = BoundingBox.Transform(_debugPrimitive.Bounds, _world);
            _renderable = _renderablesSet.CreateMesh(ref bounds, ref meshcomp);
            _renderablesSet.cRenderKeys[_renderable] = GetRenderKey(0.0f);

            // Pipelines
            _renderablesSet.cPipelines[_renderable] = _pipeline;
            _renderablesSet.cSelectionPipelines[_renderable] = _pickingPipeline;

            // Update instance data
            InstanceData dat = new InstanceData();
            dat.WorldMatrix = _world;
            dat.MaterialID = _materialBuffer.AllocationStart / (uint)sizeof(DbgMaterial);
            dat.EntityID = GetPackedEntityID(_renderablesSet.RenderableSystemIndex, _renderable);
            _worldBuffer.FillBuffer(gd, cl, ref dat);

            // Update material data
            var colmat = new DbgMaterial();
            colmat.Color = (_renderOutline ? HighlightedColor : BaseColor);
            _materialBuffer.FillBuffer(gd, cl, ref colmat);

            // Selectable
            _renderablesSet.cSelectables[_renderable] = _selectable;

            // Visible
            _renderablesSet.cVisible[_renderable]._visible = _visible;
            _renderablesSet.cSceneVis[_renderable]._renderFilter = _drawfilter;
            _renderablesSet.cSceneVis[_renderable]._drawGroup = _drawgroups;
        }

        public unsafe override void UpdateRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (_materialBuffer == null)
            {
                _materialBuffer = Renderer.MaterialBufferAllocator.Allocate((uint)sizeof(DbgMaterial), sizeof(DbgMaterial));
            }

            InstanceData dat = new InstanceData();
            dat.WorldMatrix = _world;
            dat.MaterialID = _materialBuffer.AllocationStart / (uint)sizeof(DbgMaterial);
            dat.EntityID = GetPackedEntityID(_renderablesSet.RenderableSystemIndex, _renderable);
            if (_worldBuffer == null)
            {
                _worldBuffer = Renderer.UniformBufferAllocator.Allocate((uint)sizeof(InstanceData), sizeof(InstanceData));
            }
            _worldBuffer.FillBuffer(gd, cl, ref dat);

            var colmat = new DbgMaterial();
            colmat.Color = (_renderOutline ? HighlightedColor : BaseColor);
            _materialBuffer.FillBuffer(gd, cl, ref colmat);

            if (_renderable != -1)
            {
                _renderablesSet.cBounds[_renderable] = BoundingBox.Transform(_debugPrimitive.Bounds, _world);
            }
        }

        public override void DestroyRenderables()
        {
            if (_renderable != -1)
            {
                _renderablesSet.RemoveRenderable(_renderable);
                _renderable = -1;
            }
        }

        public override void SetSelectable(ISelectable sel)
        {
            _selectable = new WeakReference<ISelectable>(sel);
            if (_renderable != -1)
            {
                _renderablesSet.cSelectables[_renderable] = _selectable;
            }
        }

        public RenderKey GetRenderKey(float distance)
        {
            // Overlays are always rendered last
            if (_overdraw)
            {
                return new RenderKey(ulong.MaxValue);
            }

            ulong code = _pipeline != null ? (ulong)_pipeline.GetHashCode() : 0;

            uint cameraDistanceInt = (uint)Math.Min(uint.MaxValue, (distance * 1000f));
            ulong index = _debugPrimitive.Is32Bit ? 1u : 0;

            return new RenderKey((code << 41) | (index << 40) | ((ulong)(_renderablesSet.cDrawParameters[_renderable]._bufferIndex & 0xFF) << 32) + cameraDistanceInt);
        }

        private static DbgPrimWireBox _regionBox = new DbgPrimWireBox(Transform.Default, new Vector3(-0.5f, 0.0f, -0.5f), new Vector3(0.5f, 1.0f, 0.5f), Color.Blue);
        private static DbgPrimWireCylinder _regionCylinder = new DbgPrimWireCylinder(Transform.Default, 1.0f, 1.0f, 12, Color.Blue);
        private static DbgPrimWireSphere _regionSphere = new DbgPrimWireSphere(Transform.Default, 1.0f, Color.Blue);
        private static DbgPrimWireSphere _regionPoint = new DbgPrimWireSphere(Transform.Default, 1.0f, Color.Yellow, 1, 4);
        private static DbgPrimWireSphere _dmyPoint = new DbgPrimWireSphere(Transform.Default, 0.05f, Color.Yellow, 1, 4);
        private static DbgPrimWireSphere _jointSphere = new DbgPrimWireSphere(Transform.Default, 0.05f, Color.Blue, 6, 6);

        public static DebugPrimitiveRenderableProxy GetBoxRegionProxy(RenderScene scene)
        {
            var r = new DebugPrimitiveRenderableProxy(scene.OpaqueRenderables, _regionBox);
            r.BaseColor = Color.Blue;
            r.HighlightedColor = Color.DarkViolet;
            return r;
        }

        public static DebugPrimitiveRenderableProxy GetCylinderRegionProxy(RenderScene scene)
        {
            var r = new DebugPrimitiveRenderableProxy(scene.OpaqueRenderables, _regionCylinder);
            r.BaseColor = Color.Blue;
            r.HighlightedColor = Color.DarkViolet;
            return r;
        }

        public static DebugPrimitiveRenderableProxy GetSphereRegionProxy(RenderScene scene)
        {
            var r = new DebugPrimitiveRenderableProxy(scene.OpaqueRenderables, _regionSphere);
            r.BaseColor = Color.Blue;
            r.HighlightedColor = Color.DarkViolet;
            return r;
        }

        public static DebugPrimitiveRenderableProxy GetPointRegionProxy(RenderScene scene)
        {
            var r = new DebugPrimitiveRenderableProxy(scene.OpaqueRenderables, _regionPoint);
            r.BaseColor = Color.Yellow;
            r.HighlightedColor = Color.DarkViolet;
            return r;
        }

        public static DebugPrimitiveRenderableProxy GetDummyPolyRegionProxy(RenderScene scene)
        {
            var r = new DebugPrimitiveRenderableProxy(scene.OverlayRenderables, _dmyPoint);
            r.BaseColor = Color.Yellow;
            r.HighlightedColor = Color.DarkViolet;
            return r;
        }

        public static DebugPrimitiveRenderableProxy GetBonePointProxy(RenderScene scene)
        {
            var r = new DebugPrimitiveRenderableProxy(scene.OverlayRenderables, _jointSphere);
            r.BaseColor = Color.Blue;
            r.HighlightedColor = Color.DarkViolet;
            return r;
        }

        // BTL Light Primitives
        private static DbgPrimWireSphere _pointLight = new DbgPrimWireSphere(Transform.Default, 1.0f, Color.Yellow, 6, 6);
        private static DbgPrimWireSpotLight _spotLight = new DbgPrimWireSpotLight(Transform.Default, 1.0f, 1.0f, Color.Yellow);
        private static DbgPrimWireSpheroidWithArrow _directionalLight = new DbgPrimWireSpheroidWithArrow(Transform.Default, 5.0f, Color.Yellow, 4, 2);
        public static DebugPrimitiveRenderableProxy GetPointLightProxy(RenderScene scene)
        {
            var r = new DebugPrimitiveRenderableProxy(scene.OpaqueRenderables, _pointLight);
            r.BaseColor = Color.YellowGreen;
            r.HighlightedColor = Color.Yellow;
            return r;
        }
        public static DebugPrimitiveRenderableProxy GetSpotLightProxy(RenderScene scene)
        {
            var r = new DebugPrimitiveRenderableProxy(scene.OpaqueRenderables, _spotLight);
            r.BaseColor = Color.Goldenrod;
            r.HighlightedColor = Color.Violet;
            return r;
        }
        public static DebugPrimitiveRenderableProxy GetDirectionalLightProxy(RenderScene scene)
        {
            var r = new DebugPrimitiveRenderableProxy(scene.OpaqueRenderables, _directionalLight);
            r.BaseColor = Color.Cyan;
            r.HighlightedColor = Color.AliceBlue;
            return r;
        }

        private static DbgPrimWireSpheroidWithArrow _modelMarkerChr = new DbgPrimWireSpheroidWithArrow(Transform.Default, .9f, Color.Firebrick, 4, 10, true);
        private static DbgPrimWireWallBox _modelMarkerObj = new DbgPrimWireWallBox(Transform.Default, new Vector3(-1.5f, 0.0f, -0.75f), new Vector3(1.5f, 2.5f, 0.75f), Color.Firebrick);
        private static DbgPrimWireSpheroidWithArrow _modelMarkerPlayer = new DbgPrimWireSpheroidWithArrow(Transform.Default, 0.75f, Color.Firebrick, 1, 6, true);
        private static DbgPrimWireWallBox _modelMarkerOther = new DbgPrimWireWallBox(Transform.Default, new Vector3(-0.3f, 0.0f, -0.3f), new Vector3(0.3f, 1.8f, 0.3f), Color.Firebrick);
        public static DebugPrimitiveRenderableProxy GetModelMarkerProxy(RenderScene scene, string partType)
        {
            //Model Markers for meshses that don't render normally (like c0000)

            IDbgPrim prim;
            Color baseColor;
            Color selectColor;

            switch (partType)
            {
                case "Enemy":
                case "DummyEnemy":
                    prim = _modelMarkerChr;
                    baseColor = Color.Firebrick;
                    selectColor = Color.Tomato;
                    break;
                case "Asset":
                case "DummyAsset":
                case "Object":
                case "DummyObject":
                    prim = _modelMarkerObj;
                    baseColor = Color.MediumVioletRed;
                    selectColor = Color.DeepPink;
                    break;
                case "Player":
                    prim = _modelMarkerPlayer;
                    baseColor = Color.DarkOliveGreen;
                    selectColor = Color.OliveDrab;
                    break;
                case "MapPiece":
                case "Collision":
                case "Navmesh":
                case "Region":
                default:
                    prim = _modelMarkerOther;
                    baseColor = Color.Wheat;
                    selectColor = Color.AntiqueWhite;
                    break;
            }

            var r = new DebugPrimitiveRenderableProxy(scene.OpaqueRenderables, prim);
            r.BaseColor = baseColor;
            r.HighlightedColor = selectColor;

            return r;
        }
    }

    public class SkeletonBoneRenderableProxy : RenderableProxy
    {
        /// <summary>
        /// Renderable for the actual bone
        /// </summary>
        private DebugPrimitiveRenderableProxy _bonePointRenderable = null;

        /// <summary>
        /// Renderables for the bones to child joints
        /// </summary>
        private List<DebugPrimitiveRenderableProxy> _boneRenderables = new List<DebugPrimitiveRenderableProxy>();

        /// <summary>
        /// Child renderables that this bone is connected to
        /// </summary>
        private List<SkeletonBoneRenderableProxy> _childBones = new List<SkeletonBoneRenderableProxy>();

        public SkeletonBoneRenderableProxy(RenderScene scene)
        {
            _bonePointRenderable = DebugPrimitiveRenderableProxy.GetBonePointProxy(scene);
            ScheduleRenderableConstruction();
            AutoRegister = true;
            _registered = true;
        }

        public override bool AutoRegister
        {
            get => _autoregister; set
            {
                _autoregister = value;
                _bonePointRenderable.AutoRegister = value;
                foreach (var c in _boneRenderables)
                {
                    c.AutoRegister = value;
                }
            }
        }

        private bool _renderOutline = false;
        public override bool RenderSelectionOutline
        {
            get => _renderOutline;
            set
            {
                _renderOutline = value;
                _bonePointRenderable.RenderSelectionOutline = _renderOutline;
                foreach (var c in _boneRenderables)
                {
                    c.RenderSelectionOutline = value;
                }
            }
        }

        private Matrix4x4 _world = Matrix4x4.Identity;
        public override Matrix4x4 World
        {
            get
            {
                return _world;
            }
            set
            {
                _world = value;
                ScheduleRenderableUpdate();
                _bonePointRenderable.World = _world;
                foreach (var c in _boneRenderables)
                {
                    c.World = _world;
                }
            }
        }
        private bool _visible = true;
        public override bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
                _bonePointRenderable.Visible = value;
                foreach (var c in _boneRenderables)
                {
                    c.Visible = _visible;
                }
            }
        }

        private RenderFilter _drawfilter = RenderFilter.All;
        public override RenderFilter DrawFilter
        {
            get
            {
                return _drawfilter;
            }
            set
            {
                _drawfilter = value;
                _bonePointRenderable.DrawFilter = value;
                foreach (var c in _boneRenderables)
                {
                    c.DrawFilter = _drawfilter;
                }
            }
        }

        private DrawGroup _drawgroups = new DrawGroup();
        public override DrawGroup DrawGroups
        {
            get
            {
                return _drawgroups;
            }
            set
            {
                _drawgroups = value;
                _bonePointRenderable.DrawGroups = _drawgroups;
                foreach (var c in _boneRenderables)
                {
                    c.DrawGroups = _drawgroups;
                }
            }
        }

        public unsafe override void ConstructRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            _bonePointRenderable.ScheduleRenderableConstruction();
            foreach (var c in _boneRenderables)
            {
                c.ScheduleRenderableConstruction();
            }
        }

        public override void DestroyRenderables()
        {
            _bonePointRenderable.DestroyRenderables();
            foreach (var c in _boneRenderables)
            {
                c.DestroyRenderables();
            }
        }

        public override BoundingBox GetBounds()
        {
            return BoundingBox.Transform(GetLocalBounds(), _world);
        }

        public override BoundingBox GetLocalBounds()
        {
            BoundingBox b = _bonePointRenderable.GetLocalBounds();
            foreach (var c in _boneRenderables)
            {
                b = BoundingBox.Combine(b, c.GetLocalBounds());
            }
            return b;
        }

        private WeakReference<ISelectable> _selectable = null;
        public override void SetSelectable(ISelectable sel)
        {
            _selectable = new WeakReference<ISelectable>(sel);
            _bonePointRenderable.SetSelectable(sel);
            foreach (var c in _boneRenderables)
            {
                c.SetSelectable(sel);
            }
        }

        public override void UpdateRenderables(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
        }
    }
}
