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
}
