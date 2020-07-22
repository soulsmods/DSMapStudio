using StudioCore.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Veldrid;
using Veldrid.Utilities;

namespace StudioCore.Scene
{
    public interface IMeshProviderEventListener
    {
        public void OnProviderAvailable();

        public void OnProviderUnavailable();
    }

    /// <summary>
    /// Common interface to a family of providers that can be used to supply mesh
    /// data to renderable proxies and renderables. For instance, many individual
    /// resources are capable of supplying render meshes, and they may or may not be
    /// loaded
    /// </summary>
    public abstract class MeshProvider
    {
        protected List<WeakReference<IMeshProviderEventListener>> _listeners = new List<WeakReference<IMeshProviderEventListener>>();

        public void AddEventListener(IMeshProviderEventListener listener)
        {
            if (IsAvailable())
            {
                listener.OnProviderAvailable();
            }
            else
            {
                listener.OnProviderUnavailable();
            }
            _listeners.Add(new WeakReference<IMeshProviderEventListener>(listener));
        }

        protected void NotifyAvailable()
        {
            foreach (var listener in _listeners)
            {
                IMeshProviderEventListener l;
                bool succ = listener.TryGetTarget(out l);
                if (succ)
                {
                    l.OnProviderAvailable();
                }
            }
        }

        protected void NotifyUnavailable()
        {
            foreach (var listener in _listeners)
            {
                IMeshProviderEventListener l;
                bool succ = listener.TryGetTarget(out l);
                if (succ)
                {
                    l.OnProviderUnavailable();
                }
            }
        }

        /// <summary>
        /// Attempts to lock the underlying resource such that it can't be unloaded
        /// while the lock is active
        /// </summary>
        /// <returns>If the resource was loaded and locked</returns>
        public virtual bool TryLock() { return true; }

        /// <summary>
        /// Unlocks the underlying resource allowing it to be locked by another thread
        /// or unloaded
        /// </summary>
        public virtual void Unlock() { }

        /// <summary>
        /// This mesh provider is capable of supplying mesh data at the moment.
        /// For example, this may return true if the underlying resource is loaded,
        /// and false if it is not.
        /// </summary>
        /// <returns>If the resource is available</returns>
        public virtual bool IsAvailable() { return true; }

        /// <summary>
        /// This mesh provider is atomic with respect to the underlying resource.
        /// This means that this represents an entire mesh that is able to be loaded and
        /// unloaded at once. If this is not atomic, this means that it may be a submesh
        /// of a larger resource, and this provider may not be valid if the resource is
        /// unloaded because its existence depends on the parent resource
        /// </summary>
        /// <returns></returns>
        public virtual bool IsAtomic() { return true; }

        /// <summary>
        /// This mesh provider has mesh data. If it does not have mesh data, child
        /// providers probably do have mesh data.
        /// </summary>
        /// <returns></returns>
        public virtual bool HasMeshData() { return false; }

        /// <summary>
        /// This mesh provider has child mesh providers. For example, a FLVER mesh
        /// provider will have submesh providers that provide the actual mesh data
        /// </summary>
        public virtual int ChildCount { get => 0; }

        /// <summary>
        /// Get the child provider at the supplied index
        /// </summary>
        /// <param name="index">index to the child provider</param>
        /// <returns>The child provider</returns>
        public virtual MeshProvider GetChildProvider(int index) { return null; }

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

        /// <summary>
        /// Get handle to the material data
        /// </summary>
        public abstract Scene.GPUBufferAllocator.GPUBufferHandle MaterialBuffer { get; }

        public virtual uint MaterialIndex { get => 0; }

        // Pipeline state
        public abstract string ShaderName { get; }

        public abstract SpecializationConstant[] SpecializationConstants { get; }

        public virtual FaceCullMode CullMode { get => FaceCullMode.None; }

        public virtual PolygonFillMode FillMode { get => PolygonFillMode.Solid; }

        public virtual FrontFace FrontFace { get => FrontFace.CounterClockwise; }

        public virtual PrimitiveTopology Topology { get => PrimitiveTopology.TriangleList; }

        // Mesh data
        public virtual bool Is32Bit { get => false; }

        public virtual int IndexOffset { get => 0; }

        public virtual int IndexCount { get => 0; }

        public virtual uint VertexSize { get => 0; }
    }

    public class FlverMeshProvider : MeshProvider, Resource.IResourceEventListener
    {
        private Resource.ResourceHandle<Resource.FlverResource> _resource;

        private List<FlverSubmeshProvider> _submeshes = new List<FlverSubmeshProvider>();

        public FlverMeshProvider(ResourceHandle<Resource.FlverResource> res)
        {
            _resource = res;
            _resource.Acquire();
            _resource.AddResourceEventListener(this);
        }

        ~FlverMeshProvider()
        {
            if (_resource != null)
            {
                _resource.Release();
            }
        }

        public override bool TryLock()
        {
            return _resource.TryLock();
        }

        public override void Unlock()
        {
            _resource.Unlock();
        }

        public override bool IsAvailable()
        {
            return _resource.IsLoaded && 
                (_resource.AccessLevel == AccessLevel.AccessGPUOptimizedOnly ||
                 _resource.AccessLevel == AccessLevel.AccessFull);
        }

        public override bool IsAtomic()
        {
            return true;
        }

        public override bool HasMeshData()
        {
            return false;
        }

        private BoundingBox _bounds;

        public override BoundingBox Bounds => _bounds;

        public override MeshLayoutType LayoutType => throw new NotImplementedException();

        public override VertexLayoutDescription LayoutDescription => throw new NotImplementedException();

        public override VertexIndexBufferAllocator.VertexIndexBufferHandle GeometryBuffer => throw new NotImplementedException();

        public override GPUBufferAllocator.GPUBufferHandle MaterialBuffer => throw new NotImplementedException();

        public override string ShaderName => throw new NotImplementedException();

        public override SpecializationConstant[] SpecializationConstants => throw new NotImplementedException();

        private void CreateSubmeshes()
        {
            _submeshes = new List<FlverSubmeshProvider>();
            var res = _resource.Get();
            _bounds = res.Bounds;
            if (res.GPUMeshes != null)
            {
                for (int i = 0; i < res.GPUMeshes.Length; i++)
                {
                    var sm = new FlverSubmeshProvider(_resource, i);
                    _submeshes.Add(sm);
                }
            }
        }

        public void OnResourceLoaded(IResourceHandle handle)
        {
            if (_resource != null && _resource.TryLock())
            {
                CreateSubmeshes();
                _resource.Unlock();
                NotifyAvailable();
            }
        }

        public void OnResourceUnloaded(IResourceHandle handle)
        {
            foreach (var submesh in _submeshes)
            {
                submesh.Invalidate();
            }
            _submeshes.Clear();
            NotifyUnavailable();
        }
    }

    public unsafe class FlverSubmeshProvider : MeshProvider
    {
        private bool _isValid = true;

        private Resource.ResourceHandle<Resource.FlverResource> _resource;
        private int _meshIndex;

        public FlverSubmeshProvider(ResourceHandle<Resource.FlverResource> handle, int idx)
        {
            _resource = handle;
            _meshIndex = idx;
        }

        public override bool TryLock()
        {
            return _resource.TryLock();
        }

        public override void Unlock()
        {
            _resource.Unlock();
        }

        internal void Invalidate()
        {
            _isValid = false;
        }

        public override bool IsAvailable()
        {
            return _isValid;
        }

        public override bool IsAtomic()
        {
            return false;
        }

        public override bool HasMeshData()
        {
            if (_resource.Get().GPUMeshes[_meshIndex].MeshFacesets.Count == 0)
            {
                return false;
            }
            return true;
        }

        public override BoundingBox Bounds => _resource.Get().GPUMeshes[_meshIndex].Bounds;

        public override MeshLayoutType LayoutType => _resource.Get().GPUMeshes[_meshIndex].Material.LayoutType;

        public override VertexLayoutDescription LayoutDescription => _resource.Get().GPUMeshes[_meshIndex].Material.VertexLayout;

        public override VertexIndexBufferAllocator.VertexIndexBufferHandle GeometryBuffer => _resource.Get().GPUMeshes[_meshIndex].GeomBuffer;

        public override GPUBufferAllocator.GPUBufferHandle MaterialBuffer => _resource.Get().GPUMeshes[_meshIndex].Material.MaterialBuffer;

        public override uint MaterialIndex => MaterialBuffer.AllocationStart / (uint)sizeof(Material);

        public override string ShaderName => _resource.Get().GPUMeshes[_meshIndex].Material.ShaderName;

        public override SpecializationConstant[] SpecializationConstants => _resource.Get().GPUMeshes[_meshIndex].Material.SpecializationConstants;

        public override FaceCullMode CullMode => _resource.Get().GPUMeshes[_meshIndex].MeshFacesets[0].BackfaceCulling ? FaceCullMode.Back : FaceCullMode.None;

        public override FrontFace FrontFace => FrontFace.CounterClockwise;

        public override PrimitiveTopology Topology => _resource.Get().GPUMeshes[_meshIndex].MeshFacesets[0].IsTriangleStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;

        public override bool Is32Bit => _resource.Get().GPUMeshes[_meshIndex].MeshFacesets[0].Is32Bit;

        public override int IndexOffset => _resource.Get().GPUMeshes[_meshIndex].MeshFacesets[0].IndexOffset;

        public override int IndexCount => _resource.Get().GPUMeshes[_meshIndex].MeshFacesets[0].IndexCount;

        public override uint VertexSize => _resource.Get().GPUMeshes[_meshIndex].Material.VertexSize;
    }
}
