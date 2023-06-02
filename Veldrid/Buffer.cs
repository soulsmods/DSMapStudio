﻿using System;
using Vortice.Vulkan;
using static Veldrid.VulkanUtil;

namespace Veldrid
{
    /// <summary>
    /// A device resource used to store arbitrary graphics data in various formats.
    /// The size of a <see cref="DeviceBuffer"/> is fixed upon creation, and resizing is not possible.
    /// See <see cref="BufferDescription"/>.
    /// </summary>
    public unsafe class DeviceBuffer : DeviceResource, BindableResource, MappableResource, IDisposable
    {
        private readonly GraphicsDevice _gd;
        private readonly Vortice.Vulkan.VkBuffer _deviceBuffer;
        private readonly VmaAllocation _allocation;
        private readonly VmaAllocationInfo _allocationInfo;
        private readonly VkMemoryRequirements _bufferMemoryRequirements;
        
        internal ResourceRefCount RefCount { get; }
        private bool _destroyed;
        private string _name;
        
        /// <summary>
        /// The total capacity, in bytes, of the buffer. This value is fixed upon creation.
        /// </summary>
        public uint SizeInBytes { get; }

        /// <summary>
        /// A bitmask indicating how this instance is permitted to be used.
        /// </summary>
        public BufferUsage Usage { get; }

        /// <summary>
        /// A string identifying this instance. Can be used to differentiate between objects in graphics debuggers and other
        /// tools.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                _gd.SetResourceName(this, value);
            }
        }
        
        internal VkBuffer Buffer => _deviceBuffer;
        internal VmaAllocation Allocation => _allocation;
        internal VmaAllocationInfo AllocationInfo => _allocationInfo;
        internal VkMemoryRequirements BufferMemoryRequirements => _bufferMemoryRequirements;

        internal DeviceBuffer(GraphicsDevice gd, uint sizeInBytes, BufferUsage usage, string callerMember = null)
        {
            _gd = gd;
            SizeInBytes = sizeInBytes;
            Usage = usage;

            VkBufferUsageFlags vkUsage = VkBufferUsageFlags.TransferSrc | VkBufferUsageFlags.TransferDst;
            if ((usage & BufferUsage.VertexBuffer) == BufferUsage.VertexBuffer)
            {
                vkUsage |= VkBufferUsageFlags.VertexBuffer;
            }
            if ((usage & BufferUsage.IndexBuffer) == BufferUsage.IndexBuffer)
            {
                vkUsage |= VkBufferUsageFlags.IndexBuffer;
            }
            if ((usage & BufferUsage.UniformBuffer) == BufferUsage.UniformBuffer)
            {
                vkUsage |= VkBufferUsageFlags.UniformBuffer;
            }
            if ((usage & BufferUsage.StructuredBufferReadWrite) == BufferUsage.StructuredBufferReadWrite
                || (usage & BufferUsage.StructuredBufferReadOnly) == BufferUsage.StructuredBufferReadOnly)
            {
                vkUsage |= VkBufferUsageFlags.StorageBuffer;
            }
            if ((usage & BufferUsage.IndirectBuffer) == BufferUsage.IndirectBuffer)
            {
                vkUsage |= VkBufferUsageFlags.IndirectBuffer;
            }

            bool hostVisible = (usage & BufferUsage.Dynamic) == BufferUsage.Dynamic
                               || (usage & BufferUsage.Staging) == BufferUsage.Staging;
            
            var bufferCI = new VkBufferCreateInfo
            {
                sType = VkStructureType.BufferCreateInfo,
                size = sizeInBytes,
                usage = vkUsage
            };

            var allocationCI = new VmaAllocationCreateInfo
            {
                flags = hostVisible ? VmaAllocationCreateFlags.HostAccessRandom | VmaAllocationCreateFlags.Mapped : 0,
                usage = hostVisible ? VmaMemoryUsage.AutoPreferHost : VmaMemoryUsage.AutoPreferDevice
            };

            VmaAllocationInfo allocationInfo;
            VkResult result = Vma.vmaCreateBuffer(gd.Allocator, &bufferCI, &allocationCI, out _deviceBuffer,
                out _allocation, &allocationInfo);
            CheckResult(result);
            _allocationInfo = allocationInfo;

            RefCount = new ResourceRefCount(DisposeCore);
        }
        
        /// <summary>
        /// Frees unmanaged device resources controlled by this instance.
        /// </summary>
        public void Dispose()
        {
            RefCount.Decrement();
        }

        private void DisposeCore()
        {
            if (!_destroyed)
            {
                _destroyed = true;
                Vma.vmaDestroyBuffer(_gd.Allocator, _deviceBuffer, _allocation);
            }
        }
    }
}
