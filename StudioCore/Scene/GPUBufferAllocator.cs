using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;

namespace StudioCore.Scene
{
    public class GPUBufferAllocator
    {
        private long _bufferSize = 0;
        private long _allocatedbytes = 0;

        public long BufferSize { get => _bufferSize; }
        public long AllocatedSize { get => _allocatedbytes; }

        private object _allocationLock = new object();

        private List<GPUBufferHandle> _allocations = new List<GPUBufferHandle>();

        private DeviceBuffer _stagingBuffer = null;
        public DeviceBuffer _backingBuffer { get; private set; } = null;

        public GPUBufferAllocator(uint initialSize, BufferUsage usage)
        {
            //Renderer.AddBackgroundUploadTask((device, cl) =>
            //{
                BufferDescription desc = new BufferDescription(
                    initialSize, usage);
                _backingBuffer = Renderer.Factory.CreateBuffer(desc);
            //});
            _bufferSize = initialSize;
        }

        public GPUBufferAllocator(uint initialSize, BufferUsage usage, uint stride)
        {
            BufferDescription desc = new BufferDescription(
                initialSize, usage, stride);
            _backingBuffer = Renderer.Factory.CreateBuffer(desc);
            _bufferSize = initialSize;
        }

        public GPUBufferHandle Allocate(uint size, int alignment)
        {
            GPUBufferHandle handle;
            lock (_allocationLock)
            {
                if ((_allocatedbytes % alignment) != 0)
                {
                    _allocatedbytes += (alignment - (_allocatedbytes % alignment));
                }
                handle = new GPUBufferHandle(this, (uint)_allocatedbytes, size);
                _allocatedbytes += size;
                if (_allocatedbytes > _bufferSize)
                {
                    throw new Exception("Download more RAM 4head");
                }
                _allocations.Add(handle);
            }
            return handle;
        }

        public void BindAsVertexBuffer(CommandList cl)
        {
            cl.SetVertexBuffer(0, _backingBuffer);
        }

        public void BindAsIndexBuffer(CommandList cl, IndexFormat indexformat)
        {
            cl.SetIndexBuffer(_backingBuffer, indexformat);
        }

        public class GPUBufferHandle
        {
            private GPUBufferAllocator _allocator;

            public uint AllocationStart { get; private set; }
            public uint AllocationSize { get; private set; }

            public GPUBufferHandle(GPUBufferAllocator alloc, uint start, uint size)
            {
                _allocator = alloc;
                AllocationStart = start;
                AllocationSize = size;
            }

            public void FillBuffer<T>(T[] data, Action completionHandler=null) where T : struct
            {
                Renderer.AddBackgroundUploadTask((device, cl) =>
                {
                    cl.UpdateBuffer(_allocator._backingBuffer, AllocationStart, data);
                    if (completionHandler != null)
                    {
                        completionHandler.Invoke();
                    }
                });
            }

            public void FillBuffer<T>(CommandList cl, T[] data) where T : struct
            {
                cl.UpdateBuffer(_allocator._backingBuffer, AllocationStart, data);
            }

            public void FillBuffer<T>(T data, Action completionHandler = null) where T : struct
            {
                Renderer.AddBackgroundUploadTask((device, cl) =>
                {
                    cl.UpdateBuffer(_allocator._backingBuffer, AllocationStart, ref data);
                    if (completionHandler != null)
                    {
                        completionHandler.Invoke();
                    }
                });
            }

            public void FillBuffer<T>(CommandList cl, ref T data) where T : struct
            {
                cl.UpdateBuffer(_allocator._backingBuffer, AllocationStart, ref data);
            }

            public void FillBuffer(IntPtr data, uint size, Action completionHandler)
            {
                Renderer.AddBackgroundUploadTask((device, cl) =>
                {
                    cl.UpdateBuffer(_allocator._backingBuffer, AllocationStart, data, size);
                    completionHandler.Invoke();
                });
            }
        }
    }

    /// <summary>
    /// Allocator for vertex/index buffers. Maintains a set of smaller megabuffers
    /// and tries to batch allocations together behind the scenes.
    /// </summary>
    public class VertexIndexBufferAllocator
    {
        private object _allocationLock = new object();

        private List<VertexIndexBufferHandle> _allocations = new List<VertexIndexBufferHandle>();
        private List<VertexIndexBufferHandle> _stagingAllocations = new List<VertexIndexBufferHandle>();
        private List<VertexIndexBufferHandle> _pendingAllocations = new List<VertexIndexBufferHandle>();
        private List<VertexIndexBuffer> _buffers = new List<VertexIndexBuffer>();

        private uint _maxVertsSize;
        private uint _maxIndicesSize;

        private DeviceBuffer _stagingBufferVerts = null;
        private DeviceBuffer _stagingBufferIndices = null;
        private long _stagingVertsSize;
        private long _stagingIndicesSize;

        private bool _stagingLocked = false;

        public VertexIndexBufferAllocator(uint maxVertsSize, uint maxIndicesSize)
        {
            BufferDescription desc = new BufferDescription(
                maxVertsSize, BufferUsage.Staging);
            _stagingBufferVerts = Renderer.Factory.CreateBuffer(desc);
            desc = new BufferDescription(
                maxIndicesSize, BufferUsage.Staging);
            _stagingBufferIndices = Renderer.Factory.CreateBuffer(desc);
            _maxVertsSize = maxVertsSize;
            _maxIndicesSize = maxIndicesSize;
        }

        public VertexIndexBufferHandle Allocate(uint vsize, uint isize, int valignment, int ialignment, Action<VertexIndexBufferHandle> onStaging=null)
        {
            VertexIndexBufferHandle handle;
            bool needsFlush = false;
            lock (_allocationLock)
            {
                long avsize = vsize;
                long aisize = isize;
                if ((_stagingVertsSize % valignment) != 0)
                {
                    _stagingVertsSize += (valignment - (_stagingVertsSize % valignment));
                }
                if ((_stagingIndicesSize % ialignment) != 0)
                {
                    _stagingIndicesSize += (ialignment - (_stagingIndicesSize % ialignment));
                }
                
                if (_stagingLocked || (_stagingVertsSize + avsize) > _maxVertsSize || (_stagingIndicesSize + aisize) > _maxIndicesSize)
                {
                    // Buffer won't fit in current megabuffer. Make it pending
                    handle = new VertexIndexBufferHandle(this);
                    handle.VAllocationSize = vsize;
                    handle.IAllocationSize = isize;
                    handle._valign = valignment;
                    handle._ialign = ialignment;
                    handle._onStagedAction = onStaging;
                    _pendingAllocations.Add(handle);
                    needsFlush = true;
                    _stagingLocked = true;
                }
                else
                {
                    // Add to currently staging megabuffer
                    handle = new VertexIndexBufferHandle(this, (uint)_stagingVertsSize, (uint)avsize, (uint)_stagingIndicesSize, (uint)aisize);
                    _stagingVertsSize += avsize;
                    _stagingIndicesSize += aisize;
                    _stagingAllocations.Add(handle);
                    if (onStaging != null)
                    {
                        onStaging.Invoke(handle);
                    }
                }
            }

            if (needsFlush)
            {
                FlushStaging();
            }
            return handle;
        }

        /// <summary>
        /// Fills the current staging buffer with any pending allocations up until it's full
        /// </summary>
        private void FlushPending()
        {
            bool full = false;
            while (!full && _pendingAllocations.Count > 0)
            {
                var pend = _pendingAllocations[_pendingAllocations.Count - 1];

                long avsize = pend.VAllocationSize;
                long aisize = pend.IAllocationSize;
                if ((_stagingVertsSize % pend._valign) != 0)
                {
                    _stagingVertsSize += (pend._valign - (_stagingVertsSize % pend._valign));
                }
                if ((_stagingIndicesSize % pend._ialign) != 0)
                {
                    _stagingIndicesSize += (pend._ialign - (_stagingIndicesSize % pend._ialign));
                }

                if ((_stagingVertsSize + avsize) > _maxVertsSize || (_stagingIndicesSize + aisize) > _maxIndicesSize)
                {
                    full = true;
                    break;
                }

                pend.VAllocationStart = (uint)_stagingVertsSize;
                pend.IAllocationStart = (uint)_stagingIndicesSize;
                pend.AllocStatus = VertexIndexBufferHandle.Status.Staging;
                if (pend._onStagedAction != null)
                {
                    pend._onStagedAction.Invoke(pend);
                }
                _stagingVertsSize += avsize;
                _stagingIndicesSize += aisize;
                _stagingAllocations.Add(pend);
                _pendingAllocations.RemoveAt(_pendingAllocations.Count - 1);
            }
        }

        public bool HasStagingOrPending()
        {
            if (_stagingVertsSize > 0 || _pendingAllocations.Count > 0)
            {
                return true;
            }
            return false;
        }

        public void FlushStaging(bool full = false)
        {
            lock (_allocationLock)
            {
                _stagingLocked = true;
            }
            Renderer.AddBackgroundUploadTask((d, cl) =>
            {
                while (_stagingVertsSize > 0)
                {
                    if (_stagingVertsSize > _maxVertsSize)
                    {
                        _stagingVertsSize = _maxVertsSize;
                    }
                    if (_stagingIndicesSize > _maxIndicesSize)
                    {
                        _stagingIndicesSize = _maxIndicesSize;
                    }

                    var buffer = new VertexIndexBuffer();
                    buffer.BufferIndex = _buffers.Count;
                    _buffers.Add(buffer);
                    buffer._bufferSizeVert = _stagingVertsSize;
                    buffer._bufferSizeIndex = _stagingIndicesSize;
                    var vd = new BufferDescription((uint)_stagingVertsSize, BufferUsage.VertexBuffer);
                    var id = new BufferDescription((uint)_stagingIndicesSize, BufferUsage.IndexBuffer);
                    buffer._backingVertBuffer = d.ResourceFactory.CreateBuffer(ref vd);
                    buffer._backingIndexBuffer = d.ResourceFactory.CreateBuffer(ref id);
                    cl.CopyBuffer(_stagingBufferVerts, 0, buffer._backingVertBuffer, 0, (uint)_stagingVertsSize);
                    cl.CopyBuffer(_stagingBufferIndices, 0, buffer._backingIndexBuffer, 0, (uint)_stagingIndicesSize);

                    foreach (var alloc in _stagingAllocations)
                    {
                        alloc.AllocStatus = VertexIndexBufferHandle.Status.Resident;
                        alloc._buffer = buffer;
                        _allocations.Add(alloc);
                    }
                    _stagingAllocations.Clear();

                    _stagingVertsSize = 0;
                    _stagingIndicesSize = 0;

                    lock (_allocationLock)
                    {
                        FlushPending();
                    }

                    if (!full)
                    {
                        break;
                    }
                }
                lock (_allocationLock)
                {
                    _stagingLocked = false;
                }
            });
        }

        public void BindAsVertexBuffer(CommandList cl, int index)
        {
            cl.SetVertexBuffer(0, _buffers[index]._backingVertBuffer);
        }

        public void BindAsIndexBuffer(CommandList cl, int index, IndexFormat indexformat)
        {
            cl.SetIndexBuffer(_buffers[index]._backingIndexBuffer, indexformat);
        }

        internal class VertexIndexBuffer
        {
            public List<VertexIndexBufferHandle> _allocations = new List<VertexIndexBufferHandle>();

            public int BufferIndex { get; internal set; } = -1;
            public long _bufferSizeVert = 0;
            public long _bufferSizeIndex = 0;

            public DeviceBuffer _backingVertBuffer { get; internal set; } = null;
            public DeviceBuffer _backingIndexBuffer { get; internal set; } = null;
        }

        public class VertexIndexBufferHandle
        {
            private VertexIndexBufferAllocator _allocator;
            internal VertexIndexBuffer _buffer = null;

            internal Action<VertexIndexBufferHandle> _onStagedAction = null;

            public enum Status
            {
                /// <summary>
                /// The allocation has not been allocated yet, and references to the data
                /// will be held
                /// </summary>
                Pending,

                /// <summary>
                /// The allocation has been reserved in a staging buffer, and data will be
                /// copied into the staging buffer
                /// </summary>
                Staging,

                /// <summary>
                /// The allocation is resident in GPU memory, and data will have been copied
                /// to it. The buffer is effectively usable
                /// </summary>
                Resident,
            }

            public Status AllocStatus { get; internal set; }

            public uint VAllocationStart { get; internal set; }
            public uint VAllocationSize { get; internal set; }
            public uint IAllocationStart { get; internal set; }
            public uint IAllocationSize { get; internal set; }

            internal int _valign;
            internal int _ialign;

            public int BufferIndex {
                get
                {
                    return (_buffer != null) ? _buffer.BufferIndex : -1;
                }
            }

            public VertexIndexBufferHandle(VertexIndexBufferAllocator alloc)
            {
                _allocator = alloc;
                AllocStatus = Status.Pending;
            }

            public VertexIndexBufferHandle(VertexIndexBufferAllocator alloc, uint vstart, uint vsize, uint istart, uint isize)
            {
                _allocator = alloc;
                VAllocationStart = vstart;
                VAllocationSize = vsize;
                IAllocationStart = istart;
                IAllocationSize = isize;
                AllocStatus = Status.Staging;
            }

            unsafe public void FillVBuffer<T>(T[] vdata, Action completionHandler = null) where T : struct
            {
                Renderer.AddBackgroundUploadTask((device, cl) =>
                {
                    if (AllocStatus == Status.Staging)
                    {
                        cl.UpdateBuffer(_allocator._stagingBufferVerts, VAllocationStart, vdata);
                    }
                    else if (AllocStatus == Status.Resident)
                    {
                        cl.UpdateBuffer(_buffer._backingVertBuffer, VAllocationStart, vdata);
                    }
                    if (completionHandler != null)
                    {
                        completionHandler.Invoke();
                    }
                });
            }

            public void FillVBuffer<T>(CommandList cl, T[] vdata) where T : struct
            {
                if (AllocStatus == Status.Staging)
                {
                    cl.UpdateBuffer(_allocator._stagingBufferVerts, VAllocationStart, vdata);
                }
                else if (AllocStatus == Status.Resident)
                {
                    cl.UpdateBuffer(_buffer._backingVertBuffer, VAllocationStart, vdata);
                }
            }

            public void FillIBuffer<T>(T[] idata, Action completionHandler = null) where T : struct
            {
                Renderer.AddBackgroundUploadTask((device, cl) =>
                {
                    if (AllocStatus == Status.Staging)
                    {
                        cl.UpdateBuffer(_allocator._stagingBufferIndices, IAllocationStart, idata);
                    }
                    else if (AllocStatus == Status.Resident)
                    {
                        cl.UpdateBuffer(_buffer._backingIndexBuffer, IAllocationStart, idata);
                    }
                    if (completionHandler != null)
                    {
                        completionHandler.Invoke();
                    }
                });
            }

            public void FillIBuffer<T>(CommandList cl, T[] idata) where T : struct
            {
                if (AllocStatus == Status.Staging)
                {
                    cl.UpdateBuffer(_allocator._stagingBufferIndices, IAllocationStart, idata);
                }
                else if (AllocStatus == Status.Resident)
                {
                    cl.UpdateBuffer(_buffer._backingIndexBuffer, IAllocationStart, idata);
                }
            }
        }
    }
}
