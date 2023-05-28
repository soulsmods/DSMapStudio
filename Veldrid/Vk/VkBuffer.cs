using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Veldrid.Vk.VulkanUtil;

namespace Veldrid.Vk
{
    internal unsafe class VkBuffer : DeviceBuffer
    {
        private readonly VkGraphicsDevice _gd;
        private readonly Vortice.Vulkan.VkBuffer _deviceBuffer;
        private readonly VmaAllocation _allocation;
        private readonly VmaAllocationInfo _allocationInfo;
        private readonly VkMemoryRequirements _bufferMemoryRequirements;
        public ResourceRefCount RefCount { get; }
        private bool _destroyed;
        private string _name;

        public override uint SizeInBytes { get; }
        public override BufferUsage Usage { get; }

        public Vortice.Vulkan.VkBuffer DeviceBuffer => _deviceBuffer;
        public VmaAllocation Allocation => _allocation;
        public VmaAllocationInfo AllocationInfo => _allocationInfo;

        public VkMemoryRequirements BufferMemoryRequirements => _bufferMemoryRequirements;

        public VkBuffer(VkGraphicsDevice gd, uint sizeInBytes, BufferUsage usage, string callerMember = null)
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

        public override string Name
        {
            get => _name;
            set
            {
                _name = value;
                _gd.SetResourceName(this, value);
            }
        }

        public override void Dispose()
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
