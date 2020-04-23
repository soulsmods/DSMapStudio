// This file is generated.

using System;

namespace Vulkan
{
    public unsafe partial struct VkOffset2D
    {
        public int x;
        public int y;
    }

    public unsafe partial struct VkOffset3D
    {
        public int x;
        public int y;
        public int z;
    }

    public unsafe partial struct VkExtent2D
    {
        public uint width;
        public uint height;
    }

    public unsafe partial struct VkExtent3D
    {
        public uint width;
        public uint height;
        public uint depth;
    }

    public unsafe partial struct VkViewport
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public float minDepth;
        public float maxDepth;
    }

    public unsafe partial struct VkRect2D
    {
        public VkOffset2D offset;
        public VkExtent2D extent;
    }

    public unsafe partial struct VkClearRect
    {
        public VkRect2D rect;
        public uint baseArrayLayer;
        public uint layerCount;
    }

    public unsafe partial struct VkComponentMapping
    {
        public VkComponentSwizzle r;
        public VkComponentSwizzle g;
        public VkComponentSwizzle b;
        public VkComponentSwizzle a;
    }

    public unsafe partial struct VkPhysicalDeviceProperties
    {
        public uint apiVersion;
        public uint driverVersion;
        public uint vendorID;
        public uint deviceID;
        public VkPhysicalDeviceType deviceType;
        public fixed byte deviceName[(int)VulkanNative.MaxPhysicalDeviceNameSize];
        public fixed byte pipelineCacheUUID[(int)VulkanNative.UuidSize];
        public VkPhysicalDeviceLimits limits;
        public VkPhysicalDeviceSparseProperties sparseProperties;
    }

    public unsafe partial struct VkExtensionProperties
    {
        public fixed byte extensionName[(int)VulkanNative.MaxExtensionNameSize];
        public uint specVersion;
    }

    public unsafe partial struct VkLayerProperties
    {
        public fixed byte layerName[(int)VulkanNative.MaxExtensionNameSize];
        public uint specVersion;
        public uint implementationVersion;
        public fixed byte description[(int)VulkanNative.MaxDescriptionSize];
    }

    public unsafe partial struct VkApplicationInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public byte* pApplicationName;
        public uint applicationVersion;
        public byte* pEngineName;
        public uint engineVersion;
        public uint apiVersion;
        public static VkApplicationInfo New()
        {
            VkApplicationInfo ret = new VkApplicationInfo();
            ret.sType = VkStructureType.ApplicationInfo;
            return ret;
        }
    }

    public unsafe partial struct VkAllocationCallbacks
    {
        public void* pUserData;
        public IntPtr pfnAllocation;
        public IntPtr pfnReallocation;
        public IntPtr pfnFree;
        public IntPtr pfnInternalAllocation;
        public IntPtr pfnInternalFree;
    }

    public unsafe partial struct VkDeviceQueueCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint queueFamilyIndex;
        public uint queueCount;
        public float* pQueuePriorities;
        public static VkDeviceQueueCreateInfo New()
        {
            VkDeviceQueueCreateInfo ret = new VkDeviceQueueCreateInfo();
            ret.sType = VkStructureType.DeviceQueueCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint queueCreateInfoCount;
        public VkDeviceQueueCreateInfo* pQueueCreateInfos;
        public uint enabledLayerCount;
        public byte** ppEnabledLayerNames;
        public uint enabledExtensionCount;
        public byte** ppEnabledExtensionNames;
        public VkPhysicalDeviceFeatures* pEnabledFeatures;
        public static VkDeviceCreateInfo New()
        {
            VkDeviceCreateInfo ret = new VkDeviceCreateInfo();
            ret.sType = VkStructureType.DeviceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkInstanceCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkApplicationInfo* pApplicationInfo;
        public uint enabledLayerCount;
        public byte** ppEnabledLayerNames;
        public uint enabledExtensionCount;
        public byte** ppEnabledExtensionNames;
        public static VkInstanceCreateInfo New()
        {
            VkInstanceCreateInfo ret = new VkInstanceCreateInfo();
            ret.sType = VkStructureType.InstanceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkQueueFamilyProperties
    {
        public VkQueueFlags queueFlags;
        public uint queueCount;
        public uint timestampValidBits;
        public VkExtent3D minImageTransferGranularity;
    }

    public unsafe partial struct VkPhysicalDeviceMemoryProperties
    {
        public uint memoryTypeCount;
        public VkMemoryType memoryTypes_0;
        public VkMemoryType memoryTypes_1;
        public VkMemoryType memoryTypes_2;
        public VkMemoryType memoryTypes_3;
        public VkMemoryType memoryTypes_4;
        public VkMemoryType memoryTypes_5;
        public VkMemoryType memoryTypes_6;
        public VkMemoryType memoryTypes_7;
        public VkMemoryType memoryTypes_8;
        public VkMemoryType memoryTypes_9;
        public VkMemoryType memoryTypes_10;
        public VkMemoryType memoryTypes_11;
        public VkMemoryType memoryTypes_12;
        public VkMemoryType memoryTypes_13;
        public VkMemoryType memoryTypes_14;
        public VkMemoryType memoryTypes_15;
        public VkMemoryType memoryTypes_16;
        public VkMemoryType memoryTypes_17;
        public VkMemoryType memoryTypes_18;
        public VkMemoryType memoryTypes_19;
        public VkMemoryType memoryTypes_20;
        public VkMemoryType memoryTypes_21;
        public VkMemoryType memoryTypes_22;
        public VkMemoryType memoryTypes_23;
        public VkMemoryType memoryTypes_24;
        public VkMemoryType memoryTypes_25;
        public VkMemoryType memoryTypes_26;
        public VkMemoryType memoryTypes_27;
        public VkMemoryType memoryTypes_28;
        public VkMemoryType memoryTypes_29;
        public VkMemoryType memoryTypes_30;
        public VkMemoryType memoryTypes_31;
        public uint memoryHeapCount;
        public VkMemoryHeap memoryHeaps_0;
        public VkMemoryHeap memoryHeaps_1;
        public VkMemoryHeap memoryHeaps_2;
        public VkMemoryHeap memoryHeaps_3;
        public VkMemoryHeap memoryHeaps_4;
        public VkMemoryHeap memoryHeaps_5;
        public VkMemoryHeap memoryHeaps_6;
        public VkMemoryHeap memoryHeaps_7;
        public VkMemoryHeap memoryHeaps_8;
        public VkMemoryHeap memoryHeaps_9;
        public VkMemoryHeap memoryHeaps_10;
        public VkMemoryHeap memoryHeaps_11;
        public VkMemoryHeap memoryHeaps_12;
        public VkMemoryHeap memoryHeaps_13;
        public VkMemoryHeap memoryHeaps_14;
        public VkMemoryHeap memoryHeaps_15;
    }

    public unsafe partial struct VkMemoryAllocateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public ulong allocationSize;
        public uint memoryTypeIndex;
        public static VkMemoryAllocateInfo New()
        {
            VkMemoryAllocateInfo ret = new VkMemoryAllocateInfo();
            ret.sType = VkStructureType.MemoryAllocateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryRequirements
    {
        public ulong size;
        public ulong alignment;
        public uint memoryTypeBits;
    }

    public unsafe partial struct VkSparseImageFormatProperties
    {
        public VkImageAspectFlags aspectMask;
        public VkExtent3D imageGranularity;
        public VkSparseImageFormatFlags flags;
    }

    public unsafe partial struct VkSparseImageMemoryRequirements
    {
        public VkSparseImageFormatProperties formatProperties;
        public uint imageMipTailFirstLod;
        public ulong imageMipTailSize;
        public ulong imageMipTailOffset;
        public ulong imageMipTailStride;
    }

    public unsafe partial struct VkMemoryType
    {
        public VkMemoryPropertyFlags propertyFlags;
        public uint heapIndex;
    }

    public unsafe partial struct VkMemoryHeap
    {
        public ulong size;
        public VkMemoryHeapFlags flags;
    }

    public unsafe partial struct VkMappedMemoryRange
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceMemory memory;
        public ulong offset;
        public ulong size;
        public static VkMappedMemoryRange New()
        {
            VkMappedMemoryRange ret = new VkMappedMemoryRange();
            ret.sType = VkStructureType.MappedMemoryRange;
            return ret;
        }
    }

    public unsafe partial struct VkFormatProperties
    {
        public VkFormatFeatureFlags linearTilingFeatures;
        public VkFormatFeatureFlags optimalTilingFeatures;
        public VkFormatFeatureFlags bufferFeatures;
    }

    public unsafe partial struct VkImageFormatProperties
    {
        public VkExtent3D maxExtent;
        public uint maxMipLevels;
        public uint maxArrayLayers;
        public VkSampleCountFlags sampleCounts;
        public ulong maxResourceSize;
    }

    public unsafe partial struct VkDescriptorBufferInfo
    {
        public VkBuffer buffer;
        public ulong offset;
        public ulong range;
    }

    public unsafe partial struct VkDescriptorImageInfo
    {
        public VkSampler sampler;
        public VkImageView imageView;
        public VkImageLayout imageLayout;
    }

    public unsafe partial struct VkWriteDescriptorSet
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorSet dstSet;
        public uint dstBinding;
        public uint dstArrayElement;
        public uint descriptorCount;
        public VkDescriptorType descriptorType;
        public VkDescriptorImageInfo* pImageInfo;
        public VkDescriptorBufferInfo* pBufferInfo;
        public VkBufferView* pTexelBufferView;
        public static VkWriteDescriptorSet New()
        {
            VkWriteDescriptorSet ret = new VkWriteDescriptorSet();
            ret.sType = VkStructureType.WriteDescriptorSet;
            return ret;
        }
    }

    public unsafe partial struct VkCopyDescriptorSet
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorSet srcSet;
        public uint srcBinding;
        public uint srcArrayElement;
        public VkDescriptorSet dstSet;
        public uint dstBinding;
        public uint dstArrayElement;
        public uint descriptorCount;
        public static VkCopyDescriptorSet New()
        {
            VkCopyDescriptorSet ret = new VkCopyDescriptorSet();
            ret.sType = VkStructureType.CopyDescriptorSet;
            return ret;
        }
    }

    public unsafe partial struct VkBufferCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBufferCreateFlags flags;
        public ulong size;
        public VkBufferUsageFlags usage;
        public VkSharingMode sharingMode;
        public uint queueFamilyIndexCount;
        public uint* pQueueFamilyIndices;
        public static VkBufferCreateInfo New()
        {
            VkBufferCreateInfo ret = new VkBufferCreateInfo();
            ret.sType = VkStructureType.BufferCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkBufferViewCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkBuffer buffer;
        public VkFormat format;
        public ulong offset;
        public ulong range;
        public static VkBufferViewCreateInfo New()
        {
            VkBufferViewCreateInfo ret = new VkBufferViewCreateInfo();
            ret.sType = VkStructureType.BufferViewCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkImageSubresource
    {
        public VkImageAspectFlags aspectMask;
        public uint mipLevel;
        public uint arrayLayer;
    }

    public unsafe partial struct VkImageSubresourceLayers
    {
        public VkImageAspectFlags aspectMask;
        public uint mipLevel;
        public uint baseArrayLayer;
        public uint layerCount;
    }

    public unsafe partial struct VkImageSubresourceRange
    {
        public VkImageAspectFlags aspectMask;
        public uint baseMipLevel;
        public uint levelCount;
        public uint baseArrayLayer;
        public uint layerCount;
    }

    public unsafe partial struct VkMemoryBarrier
    {
        public VkStructureType sType;
        public void* pNext;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public static VkMemoryBarrier New()
        {
            VkMemoryBarrier ret = new VkMemoryBarrier();
            ret.sType = VkStructureType.MemoryBarrier;
            return ret;
        }
    }

    public unsafe partial struct VkBufferMemoryBarrier
    {
        public VkStructureType sType;
        public void* pNext;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public uint srcQueueFamilyIndex;
        public uint dstQueueFamilyIndex;
        public VkBuffer buffer;
        public ulong offset;
        public ulong size;
        public static VkBufferMemoryBarrier New()
        {
            VkBufferMemoryBarrier ret = new VkBufferMemoryBarrier();
            ret.sType = VkStructureType.BufferMemoryBarrier;
            return ret;
        }
    }

    public unsafe partial struct VkImageMemoryBarrier
    {
        public VkStructureType sType;
        public void* pNext;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public VkImageLayout oldLayout;
        public VkImageLayout newLayout;
        public uint srcQueueFamilyIndex;
        public uint dstQueueFamilyIndex;
        public VkImage image;
        public VkImageSubresourceRange subresourceRange;
        public static VkImageMemoryBarrier New()
        {
            VkImageMemoryBarrier ret = new VkImageMemoryBarrier();
            ret.sType = VkStructureType.ImageMemoryBarrier;
            return ret;
        }
    }

    public unsafe partial struct VkImageCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImageCreateFlags flags;
        public VkImageType imageType;
        public VkFormat format;
        public VkExtent3D extent;
        public uint mipLevels;
        public uint arrayLayers;
        public VkSampleCountFlags samples;
        public VkImageTiling tiling;
        public VkImageUsageFlags usage;
        public VkSharingMode sharingMode;
        public uint queueFamilyIndexCount;
        public uint* pQueueFamilyIndices;
        public VkImageLayout initialLayout;
        public static VkImageCreateInfo New()
        {
            VkImageCreateInfo ret = new VkImageCreateInfo();
            ret.sType = VkStructureType.ImageCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkSubresourceLayout
    {
        public ulong offset;
        public ulong size;
        public ulong rowPitch;
        public ulong arrayPitch;
        public ulong depthPitch;
    }

    public unsafe partial struct VkImageViewCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkImage image;
        public VkImageViewType viewType;
        public VkFormat format;
        public VkComponentMapping components;
        public VkImageSubresourceRange subresourceRange;
        public static VkImageViewCreateInfo New()
        {
            VkImageViewCreateInfo ret = new VkImageViewCreateInfo();
            ret.sType = VkStructureType.ImageViewCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkBufferCopy
    {
        public ulong srcOffset;
        public ulong dstOffset;
        public ulong size;
    }

    public unsafe partial struct VkSparseMemoryBind
    {
        public ulong resourceOffset;
        public ulong size;
        public VkDeviceMemory memory;
        public ulong memoryOffset;
        public VkSparseMemoryBindFlags flags;
    }

    public unsafe partial struct VkSparseImageMemoryBind
    {
        public VkImageSubresource subresource;
        public VkOffset3D offset;
        public VkExtent3D extent;
        public VkDeviceMemory memory;
        public ulong memoryOffset;
        public VkSparseMemoryBindFlags flags;
    }

    public unsafe partial struct VkSparseBufferMemoryBindInfo
    {
        public VkBuffer buffer;
        public uint bindCount;
        public VkSparseMemoryBind* pBinds;
    }

    public unsafe partial struct VkSparseImageOpaqueMemoryBindInfo
    {
        public VkImage image;
        public uint bindCount;
        public VkSparseMemoryBind* pBinds;
    }

    public unsafe partial struct VkSparseImageMemoryBindInfo
    {
        public VkImage image;
        public uint bindCount;
        public VkSparseImageMemoryBind* pBinds;
    }

    public unsafe partial struct VkBindSparseInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint waitSemaphoreCount;
        public VkSemaphore* pWaitSemaphores;
        public uint bufferBindCount;
        public VkSparseBufferMemoryBindInfo* pBufferBinds;
        public uint imageOpaqueBindCount;
        public VkSparseImageOpaqueMemoryBindInfo* pImageOpaqueBinds;
        public uint imageBindCount;
        public VkSparseImageMemoryBindInfo* pImageBinds;
        public uint signalSemaphoreCount;
        public VkSemaphore* pSignalSemaphores;
        public static VkBindSparseInfo New()
        {
            VkBindSparseInfo ret = new VkBindSparseInfo();
            ret.sType = VkStructureType.BindSparseInfo;
            return ret;
        }
    }

    public unsafe partial struct VkImageCopy
    {
        public VkImageSubresourceLayers srcSubresource;
        public VkOffset3D srcOffset;
        public VkImageSubresourceLayers dstSubresource;
        public VkOffset3D dstOffset;
        public VkExtent3D extent;
    }

    public unsafe partial struct VkImageBlit
    {
        public VkImageSubresourceLayers srcSubresource;
        public VkOffset3D srcOffsets_0;
        public VkOffset3D srcOffsets_1;
        public VkImageSubresourceLayers dstSubresource;
        public VkOffset3D dstOffsets_0;
        public VkOffset3D dstOffsets_1;
    }

    public unsafe partial struct VkBufferImageCopy
    {
        public ulong bufferOffset;
        public uint bufferRowLength;
        public uint bufferImageHeight;
        public VkImageSubresourceLayers imageSubresource;
        public VkOffset3D imageOffset;
        public VkExtent3D imageExtent;
    }

    public unsafe partial struct VkImageResolve
    {
        public VkImageSubresourceLayers srcSubresource;
        public VkOffset3D srcOffset;
        public VkImageSubresourceLayers dstSubresource;
        public VkOffset3D dstOffset;
        public VkExtent3D extent;
    }

    public unsafe partial struct VkShaderModuleCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public UIntPtr codeSize;
        public uint* pCode;
        public static VkShaderModuleCreateInfo New()
        {
            VkShaderModuleCreateInfo ret = new VkShaderModuleCreateInfo();
            ret.sType = VkStructureType.ShaderModuleCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDescriptorSetLayoutBinding
    {
        public uint binding;
        public VkDescriptorType descriptorType;
        public uint descriptorCount;
        public VkShaderStageFlags stageFlags;
        public VkSampler* pImmutableSamplers;
    }

    public unsafe partial struct VkDescriptorSetLayoutCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorSetLayoutCreateFlags flags;
        public uint bindingCount;
        public VkDescriptorSetLayoutBinding* pBindings;
        public static VkDescriptorSetLayoutCreateInfo New()
        {
            VkDescriptorSetLayoutCreateInfo ret = new VkDescriptorSetLayoutCreateInfo();
            ret.sType = VkStructureType.DescriptorSetLayoutCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDescriptorPoolSize
    {
        public VkDescriptorType type;
        public uint descriptorCount;
    }

    public unsafe partial struct VkDescriptorPoolCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorPoolCreateFlags flags;
        public uint maxSets;
        public uint poolSizeCount;
        public VkDescriptorPoolSize* pPoolSizes;
        public static VkDescriptorPoolCreateInfo New()
        {
            VkDescriptorPoolCreateInfo ret = new VkDescriptorPoolCreateInfo();
            ret.sType = VkStructureType.DescriptorPoolCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDescriptorSetAllocateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorPool descriptorPool;
        public uint descriptorSetCount;
        public VkDescriptorSetLayout* pSetLayouts;
        public static VkDescriptorSetAllocateInfo New()
        {
            VkDescriptorSetAllocateInfo ret = new VkDescriptorSetAllocateInfo();
            ret.sType = VkStructureType.DescriptorSetAllocateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkSpecializationMapEntry
    {
        public uint constantID;
        public uint offset;
        public UIntPtr size;
    }

    public unsafe partial struct VkSpecializationInfo
    {
        public uint mapEntryCount;
        public VkSpecializationMapEntry* pMapEntries;
        public UIntPtr dataSize;
        public void* pData;
    }

    public unsafe partial struct VkPipelineShaderStageCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkShaderStageFlags stage;
        public VkShaderModule module;
        public byte* pName;
        public VkSpecializationInfo* pSpecializationInfo;
        public static VkPipelineShaderStageCreateInfo New()
        {
            VkPipelineShaderStageCreateInfo ret = new VkPipelineShaderStageCreateInfo();
            ret.sType = VkStructureType.PipelineShaderStageCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkComputePipelineCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineCreateFlags flags;
        public VkPipelineShaderStageCreateInfo stage;
        public VkPipelineLayout layout;
        public VkPipeline basePipelineHandle;
        public int basePipelineIndex;
        public static VkComputePipelineCreateInfo New()
        {
            VkComputePipelineCreateInfo ret = new VkComputePipelineCreateInfo();
            ret.sType = VkStructureType.ComputePipelineCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkVertexInputBindingDescription
    {
        public uint binding;
        public uint stride;
        public VkVertexInputRate inputRate;
    }

    public unsafe partial struct VkVertexInputAttributeDescription
    {
        public uint location;
        public uint binding;
        public VkFormat format;
        public uint offset;
    }

    public unsafe partial struct VkPipelineVertexInputStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint vertexBindingDescriptionCount;
        public VkVertexInputBindingDescription* pVertexBindingDescriptions;
        public uint vertexAttributeDescriptionCount;
        public VkVertexInputAttributeDescription* pVertexAttributeDescriptions;
        public static VkPipelineVertexInputStateCreateInfo New()
        {
            VkPipelineVertexInputStateCreateInfo ret = new VkPipelineVertexInputStateCreateInfo();
            ret.sType = VkStructureType.PipelineVertexInputStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineInputAssemblyStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkPrimitiveTopology topology;
        public VkBool32 primitiveRestartEnable;
        public static VkPipelineInputAssemblyStateCreateInfo New()
        {
            VkPipelineInputAssemblyStateCreateInfo ret = new VkPipelineInputAssemblyStateCreateInfo();
            ret.sType = VkStructureType.PipelineInputAssemblyStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineTessellationStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint patchControlPoints;
        public static VkPipelineTessellationStateCreateInfo New()
        {
            VkPipelineTessellationStateCreateInfo ret = new VkPipelineTessellationStateCreateInfo();
            ret.sType = VkStructureType.PipelineTessellationStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineViewportStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint viewportCount;
        public VkViewport* pViewports;
        public uint scissorCount;
        public VkRect2D* pScissors;
        public static VkPipelineViewportStateCreateInfo New()
        {
            VkPipelineViewportStateCreateInfo ret = new VkPipelineViewportStateCreateInfo();
            ret.sType = VkStructureType.PipelineViewportStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineRasterizationStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkBool32 depthClampEnable;
        public VkBool32 rasterizerDiscardEnable;
        public VkPolygonMode polygonMode;
        public VkCullModeFlags cullMode;
        public VkFrontFace frontFace;
        public VkBool32 depthBiasEnable;
        public float depthBiasConstantFactor;
        public float depthBiasClamp;
        public float depthBiasSlopeFactor;
        public float lineWidth;
        public static VkPipelineRasterizationStateCreateInfo New()
        {
            VkPipelineRasterizationStateCreateInfo ret = new VkPipelineRasterizationStateCreateInfo();
            ret.sType = VkStructureType.PipelineRasterizationStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineMultisampleStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkSampleCountFlags rasterizationSamples;
        public VkBool32 sampleShadingEnable;
        public float minSampleShading;
        public uint* pSampleMask;
        public VkBool32 alphaToCoverageEnable;
        public VkBool32 alphaToOneEnable;
        public static VkPipelineMultisampleStateCreateInfo New()
        {
            VkPipelineMultisampleStateCreateInfo ret = new VkPipelineMultisampleStateCreateInfo();
            ret.sType = VkStructureType.PipelineMultisampleStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineColorBlendAttachmentState
    {
        public VkBool32 blendEnable;
        public VkBlendFactor srcColorBlendFactor;
        public VkBlendFactor dstColorBlendFactor;
        public VkBlendOp colorBlendOp;
        public VkBlendFactor srcAlphaBlendFactor;
        public VkBlendFactor dstAlphaBlendFactor;
        public VkBlendOp alphaBlendOp;
        public VkColorComponentFlags colorWriteMask;
    }

    public unsafe partial struct VkPipelineColorBlendStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkBool32 logicOpEnable;
        public VkLogicOp logicOp;
        public uint attachmentCount;
        public VkPipelineColorBlendAttachmentState* pAttachments;
        public float blendConstants_0;
        public float blendConstants_1;
        public float blendConstants_2;
        public float blendConstants_3;
        public static VkPipelineColorBlendStateCreateInfo New()
        {
            VkPipelineColorBlendStateCreateInfo ret = new VkPipelineColorBlendStateCreateInfo();
            ret.sType = VkStructureType.PipelineColorBlendStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineDynamicStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint dynamicStateCount;
        public VkDynamicState* pDynamicStates;
        public static VkPipelineDynamicStateCreateInfo New()
        {
            VkPipelineDynamicStateCreateInfo ret = new VkPipelineDynamicStateCreateInfo();
            ret.sType = VkStructureType.PipelineDynamicStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkStencilOpState
    {
        public VkStencilOp failOp;
        public VkStencilOp passOp;
        public VkStencilOp depthFailOp;
        public VkCompareOp compareOp;
        public uint compareMask;
        public uint writeMask;
        public uint reference;
    }

    public unsafe partial struct VkPipelineDepthStencilStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkBool32 depthTestEnable;
        public VkBool32 depthWriteEnable;
        public VkCompareOp depthCompareOp;
        public VkBool32 depthBoundsTestEnable;
        public VkBool32 stencilTestEnable;
        public VkStencilOpState front;
        public VkStencilOpState back;
        public float minDepthBounds;
        public float maxDepthBounds;
        public static VkPipelineDepthStencilStateCreateInfo New()
        {
            VkPipelineDepthStencilStateCreateInfo ret = new VkPipelineDepthStencilStateCreateInfo();
            ret.sType = VkStructureType.PipelineDepthStencilStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkGraphicsPipelineCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineCreateFlags flags;
        public uint stageCount;
        public VkPipelineShaderStageCreateInfo* pStages;
        public VkPipelineVertexInputStateCreateInfo* pVertexInputState;
        public VkPipelineInputAssemblyStateCreateInfo* pInputAssemblyState;
        public VkPipelineTessellationStateCreateInfo* pTessellationState;
        public VkPipelineViewportStateCreateInfo* pViewportState;
        public VkPipelineRasterizationStateCreateInfo* pRasterizationState;
        public VkPipelineMultisampleStateCreateInfo* pMultisampleState;
        public VkPipelineDepthStencilStateCreateInfo* pDepthStencilState;
        public VkPipelineColorBlendStateCreateInfo* pColorBlendState;
        public VkPipelineDynamicStateCreateInfo* pDynamicState;
        public VkPipelineLayout layout;
        public VkRenderPass renderPass;
        public uint subpass;
        public VkPipeline basePipelineHandle;
        public int basePipelineIndex;
        public static VkGraphicsPipelineCreateInfo New()
        {
            VkGraphicsPipelineCreateInfo ret = new VkGraphicsPipelineCreateInfo();
            ret.sType = VkStructureType.GraphicsPipelineCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineCacheCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public UIntPtr initialDataSize;
        public void* pInitialData;
        public static VkPipelineCacheCreateInfo New()
        {
            VkPipelineCacheCreateInfo ret = new VkPipelineCacheCreateInfo();
            ret.sType = VkStructureType.PipelineCacheCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPushConstantRange
    {
        public VkShaderStageFlags stageFlags;
        public uint offset;
        public uint size;
    }

    public unsafe partial struct VkPipelineLayoutCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint setLayoutCount;
        public VkDescriptorSetLayout* pSetLayouts;
        public uint pushConstantRangeCount;
        public VkPushConstantRange* pPushConstantRanges;
        public static VkPipelineLayoutCreateInfo New()
        {
            VkPipelineLayoutCreateInfo ret = new VkPipelineLayoutCreateInfo();
            ret.sType = VkStructureType.PipelineLayoutCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkSamplerCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkFilter magFilter;
        public VkFilter minFilter;
        public VkSamplerMipmapMode mipmapMode;
        public VkSamplerAddressMode addressModeU;
        public VkSamplerAddressMode addressModeV;
        public VkSamplerAddressMode addressModeW;
        public float mipLodBias;
        public VkBool32 anisotropyEnable;
        public float maxAnisotropy;
        public VkBool32 compareEnable;
        public VkCompareOp compareOp;
        public float minLod;
        public float maxLod;
        public VkBorderColor borderColor;
        public VkBool32 unnormalizedCoordinates;
        public static VkSamplerCreateInfo New()
        {
            VkSamplerCreateInfo ret = new VkSamplerCreateInfo();
            ret.sType = VkStructureType.SamplerCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkCommandPoolCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkCommandPoolCreateFlags flags;
        public uint queueFamilyIndex;
        public static VkCommandPoolCreateInfo New()
        {
            VkCommandPoolCreateInfo ret = new VkCommandPoolCreateInfo();
            ret.sType = VkStructureType.CommandPoolCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkCommandBufferAllocateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkCommandPool commandPool;
        public VkCommandBufferLevel level;
        public uint commandBufferCount;
        public static VkCommandBufferAllocateInfo New()
        {
            VkCommandBufferAllocateInfo ret = new VkCommandBufferAllocateInfo();
            ret.sType = VkStructureType.CommandBufferAllocateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkCommandBufferInheritanceInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRenderPass renderPass;
        public uint subpass;
        public VkFramebuffer framebuffer;
        public VkBool32 occlusionQueryEnable;
        public VkQueryControlFlags queryFlags;
        public VkQueryPipelineStatisticFlags pipelineStatistics;
        public static VkCommandBufferInheritanceInfo New()
        {
            VkCommandBufferInheritanceInfo ret = new VkCommandBufferInheritanceInfo();
            ret.sType = VkStructureType.CommandBufferInheritanceInfo;
            return ret;
        }
    }

    public unsafe partial struct VkCommandBufferBeginInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkCommandBufferUsageFlags flags;
        public VkCommandBufferInheritanceInfo* pInheritanceInfo;
        public static VkCommandBufferBeginInfo New()
        {
            VkCommandBufferBeginInfo ret = new VkCommandBufferBeginInfo();
            ret.sType = VkStructureType.CommandBufferBeginInfo;
            return ret;
        }
    }

    public unsafe partial struct VkRenderPassBeginInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRenderPass renderPass;
        public VkFramebuffer framebuffer;
        public VkRect2D renderArea;
        public uint clearValueCount;
        public VkClearValue* pClearValues;
        public static VkRenderPassBeginInfo New()
        {
            VkRenderPassBeginInfo ret = new VkRenderPassBeginInfo();
            ret.sType = VkStructureType.RenderPassBeginInfo;
            return ret;
        }
    }

    public unsafe partial struct VkClearDepthStencilValue
    {
        public float depth;
        public uint stencil;
    }

    public unsafe partial struct VkClearAttachment
    {
        public VkImageAspectFlags aspectMask;
        public uint colorAttachment;
        public VkClearValue clearValue;
    }

    public unsafe partial struct VkAttachmentDescription
    {
        public VkAttachmentDescriptionFlags flags;
        public VkFormat format;
        public VkSampleCountFlags samples;
        public VkAttachmentLoadOp loadOp;
        public VkAttachmentStoreOp storeOp;
        public VkAttachmentLoadOp stencilLoadOp;
        public VkAttachmentStoreOp stencilStoreOp;
        public VkImageLayout initialLayout;
        public VkImageLayout finalLayout;
    }

    public unsafe partial struct VkAttachmentReference
    {
        public uint attachment;
        public VkImageLayout layout;
    }

    public unsafe partial struct VkSubpassDescription
    {
        public VkSubpassDescriptionFlags flags;
        public VkPipelineBindPoint pipelineBindPoint;
        public uint inputAttachmentCount;
        public VkAttachmentReference* pInputAttachments;
        public uint colorAttachmentCount;
        public VkAttachmentReference* pColorAttachments;
        public VkAttachmentReference* pResolveAttachments;
        public VkAttachmentReference* pDepthStencilAttachment;
        public uint preserveAttachmentCount;
        public uint* pPreserveAttachments;
    }

    public unsafe partial struct VkSubpassDependency
    {
        public uint srcSubpass;
        public uint dstSubpass;
        public VkPipelineStageFlags srcStageMask;
        public VkPipelineStageFlags dstStageMask;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public VkDependencyFlags dependencyFlags;
    }

    public unsafe partial struct VkRenderPassCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint attachmentCount;
        public VkAttachmentDescription* pAttachments;
        public uint subpassCount;
        public VkSubpassDescription* pSubpasses;
        public uint dependencyCount;
        public VkSubpassDependency* pDependencies;
        public static VkRenderPassCreateInfo New()
        {
            VkRenderPassCreateInfo ret = new VkRenderPassCreateInfo();
            ret.sType = VkStructureType.RenderPassCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkEventCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public static VkEventCreateInfo New()
        {
            VkEventCreateInfo ret = new VkEventCreateInfo();
            ret.sType = VkStructureType.EventCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkFenceCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFenceCreateFlags flags;
        public static VkFenceCreateInfo New()
        {
            VkFenceCreateInfo ret = new VkFenceCreateInfo();
            ret.sType = VkStructureType.FenceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceFeatures
    {
        public VkBool32 robustBufferAccess;
        public VkBool32 fullDrawIndexUint32;
        public VkBool32 imageCubeArray;
        public VkBool32 independentBlend;
        public VkBool32 geometryShader;
        public VkBool32 tessellationShader;
        public VkBool32 sampleRateShading;
        public VkBool32 dualSrcBlend;
        public VkBool32 logicOp;
        public VkBool32 multiDrawIndirect;
        public VkBool32 drawIndirectFirstInstance;
        public VkBool32 depthClamp;
        public VkBool32 depthBiasClamp;
        public VkBool32 fillModeNonSolid;
        public VkBool32 depthBounds;
        public VkBool32 wideLines;
        public VkBool32 largePoints;
        public VkBool32 alphaToOne;
        public VkBool32 multiViewport;
        public VkBool32 samplerAnisotropy;
        public VkBool32 textureCompressionETC2;
        public VkBool32 textureCompressionASTC_LDR;
        public VkBool32 textureCompressionBC;
        public VkBool32 occlusionQueryPrecise;
        public VkBool32 pipelineStatisticsQuery;
        public VkBool32 vertexPipelineStoresAndAtomics;
        public VkBool32 fragmentStoresAndAtomics;
        public VkBool32 shaderTessellationAndGeometryPointSize;
        public VkBool32 shaderImageGatherExtended;
        public VkBool32 shaderStorageImageExtendedFormats;
        public VkBool32 shaderStorageImageMultisample;
        public VkBool32 shaderStorageImageReadWithoutFormat;
        public VkBool32 shaderStorageImageWriteWithoutFormat;
        public VkBool32 shaderUniformBufferArrayDynamicIndexing;
        public VkBool32 shaderSampledImageArrayDynamicIndexing;
        public VkBool32 shaderStorageBufferArrayDynamicIndexing;
        public VkBool32 shaderStorageImageArrayDynamicIndexing;
        public VkBool32 shaderClipDistance;
        public VkBool32 shaderCullDistance;
        public VkBool32 shaderFloat64;
        public VkBool32 shaderInt64;
        public VkBool32 shaderInt16;
        public VkBool32 shaderResourceResidency;
        public VkBool32 shaderResourceMinLod;
        public VkBool32 sparseBinding;
        public VkBool32 sparseResidencyBuffer;
        public VkBool32 sparseResidencyImage2D;
        public VkBool32 sparseResidencyImage3D;
        public VkBool32 sparseResidency2Samples;
        public VkBool32 sparseResidency4Samples;
        public VkBool32 sparseResidency8Samples;
        public VkBool32 sparseResidency16Samples;
        public VkBool32 sparseResidencyAliased;
        public VkBool32 variableMultisampleRate;
        public VkBool32 inheritedQueries;
    }

    public unsafe partial struct VkPhysicalDeviceSparseProperties
    {
        public VkBool32 residencyStandard2DBlockShape;
        public VkBool32 residencyStandard2DMultisampleBlockShape;
        public VkBool32 residencyStandard3DBlockShape;
        public VkBool32 residencyAlignedMipSize;
        public VkBool32 residencyNonResidentStrict;
    }

    public unsafe partial struct VkPhysicalDeviceLimits
    {
        public uint maxImageDimension1D;
        public uint maxImageDimension2D;
        public uint maxImageDimension3D;
        public uint maxImageDimensionCube;
        public uint maxImageArrayLayers;
        public uint maxTexelBufferElements;
        public uint maxUniformBufferRange;
        public uint maxStorageBufferRange;
        public uint maxPushConstantsSize;
        public uint maxMemoryAllocationCount;
        public uint maxSamplerAllocationCount;
        public ulong bufferImageGranularity;
        public ulong sparseAddressSpaceSize;
        public uint maxBoundDescriptorSets;
        public uint maxPerStageDescriptorSamplers;
        public uint maxPerStageDescriptorUniformBuffers;
        public uint maxPerStageDescriptorStorageBuffers;
        public uint maxPerStageDescriptorSampledImages;
        public uint maxPerStageDescriptorStorageImages;
        public uint maxPerStageDescriptorInputAttachments;
        public uint maxPerStageResources;
        public uint maxDescriptorSetSamplers;
        public uint maxDescriptorSetUniformBuffers;
        public uint maxDescriptorSetUniformBuffersDynamic;
        public uint maxDescriptorSetStorageBuffers;
        public uint maxDescriptorSetStorageBuffersDynamic;
        public uint maxDescriptorSetSampledImages;
        public uint maxDescriptorSetStorageImages;
        public uint maxDescriptorSetInputAttachments;
        public uint maxVertexInputAttributes;
        public uint maxVertexInputBindings;
        public uint maxVertexInputAttributeOffset;
        public uint maxVertexInputBindingStride;
        public uint maxVertexOutputComponents;
        public uint maxTessellationGenerationLevel;
        public uint maxTessellationPatchSize;
        public uint maxTessellationControlPerVertexInputComponents;
        public uint maxTessellationControlPerVertexOutputComponents;
        public uint maxTessellationControlPerPatchOutputComponents;
        public uint maxTessellationControlTotalOutputComponents;
        public uint maxTessellationEvaluationInputComponents;
        public uint maxTessellationEvaluationOutputComponents;
        public uint maxGeometryShaderInvocations;
        public uint maxGeometryInputComponents;
        public uint maxGeometryOutputComponents;
        public uint maxGeometryOutputVertices;
        public uint maxGeometryTotalOutputComponents;
        public uint maxFragmentInputComponents;
        public uint maxFragmentOutputAttachments;
        public uint maxFragmentDualSrcAttachments;
        public uint maxFragmentCombinedOutputResources;
        public uint maxComputeSharedMemorySize;
        public uint maxComputeWorkGroupCount_0;
        public uint maxComputeWorkGroupCount_1;
        public uint maxComputeWorkGroupCount_2;
        public uint maxComputeWorkGroupInvocations;
        public uint maxComputeWorkGroupSize_0;
        public uint maxComputeWorkGroupSize_1;
        public uint maxComputeWorkGroupSize_2;
        public uint subPixelPrecisionBits;
        public uint subTexelPrecisionBits;
        public uint mipmapPrecisionBits;
        public uint maxDrawIndexedIndexValue;
        public uint maxDrawIndirectCount;
        public float maxSamplerLodBias;
        public float maxSamplerAnisotropy;
        public uint maxViewports;
        public uint maxViewportDimensions_0;
        public uint maxViewportDimensions_1;
        public float viewportBoundsRange_0;
        public float viewportBoundsRange_1;
        public uint viewportSubPixelBits;
        public UIntPtr minMemoryMapAlignment;
        public ulong minTexelBufferOffsetAlignment;
        public ulong minUniformBufferOffsetAlignment;
        public ulong minStorageBufferOffsetAlignment;
        public int minTexelOffset;
        public uint maxTexelOffset;
        public int minTexelGatherOffset;
        public uint maxTexelGatherOffset;
        public float minInterpolationOffset;
        public float maxInterpolationOffset;
        public uint subPixelInterpolationOffsetBits;
        public uint maxFramebufferWidth;
        public uint maxFramebufferHeight;
        public uint maxFramebufferLayers;
        public VkSampleCountFlags framebufferColorSampleCounts;
        public VkSampleCountFlags framebufferDepthSampleCounts;
        public VkSampleCountFlags framebufferStencilSampleCounts;
        public VkSampleCountFlags framebufferNoAttachmentsSampleCounts;
        public uint maxColorAttachments;
        public VkSampleCountFlags sampledImageColorSampleCounts;
        public VkSampleCountFlags sampledImageIntegerSampleCounts;
        public VkSampleCountFlags sampledImageDepthSampleCounts;
        public VkSampleCountFlags sampledImageStencilSampleCounts;
        public VkSampleCountFlags storageImageSampleCounts;
        public uint maxSampleMaskWords;
        public VkBool32 timestampComputeAndGraphics;
        public float timestampPeriod;
        public uint maxClipDistances;
        public uint maxCullDistances;
        public uint maxCombinedClipAndCullDistances;
        public uint discreteQueuePriorities;
        public float pointSizeRange_0;
        public float pointSizeRange_1;
        public float lineWidthRange_0;
        public float lineWidthRange_1;
        public float pointSizeGranularity;
        public float lineWidthGranularity;
        public VkBool32 strictLines;
        public VkBool32 standardSampleLocations;
        public ulong optimalBufferCopyOffsetAlignment;
        public ulong optimalBufferCopyRowPitchAlignment;
        public ulong nonCoherentAtomSize;
    }

    public unsafe partial struct VkSemaphoreCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public static VkSemaphoreCreateInfo New()
        {
            VkSemaphoreCreateInfo ret = new VkSemaphoreCreateInfo();
            ret.sType = VkStructureType.SemaphoreCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkQueryPoolCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkQueryType queryType;
        public uint queryCount;
        public VkQueryPipelineStatisticFlags pipelineStatistics;
        public static VkQueryPoolCreateInfo New()
        {
            VkQueryPoolCreateInfo ret = new VkQueryPoolCreateInfo();
            ret.sType = VkStructureType.QueryPoolCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkFramebufferCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkRenderPass renderPass;
        public uint attachmentCount;
        public VkImageView* pAttachments;
        public uint width;
        public uint height;
        public uint layers;
        public static VkFramebufferCreateInfo New()
        {
            VkFramebufferCreateInfo ret = new VkFramebufferCreateInfo();
            ret.sType = VkStructureType.FramebufferCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDrawIndirectCommand
    {
        public uint vertexCount;
        public uint instanceCount;
        public uint firstVertex;
        public uint firstInstance;
    }

    public unsafe partial struct VkDrawIndexedIndirectCommand
    {
        public uint indexCount;
        public uint instanceCount;
        public uint firstIndex;
        public int vertexOffset;
        public uint firstInstance;
    }

    public unsafe partial struct VkDispatchIndirectCommand
    {
        public uint x;
        public uint y;
        public uint z;
    }

    public unsafe partial struct VkSubmitInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint waitSemaphoreCount;
        public VkSemaphore* pWaitSemaphores;
        public VkPipelineStageFlags* pWaitDstStageMask;
        public uint commandBufferCount;
        public VkCommandBuffer* pCommandBuffers;
        public uint signalSemaphoreCount;
        public VkSemaphore* pSignalSemaphores;
        public static VkSubmitInfo New()
        {
            VkSubmitInfo ret = new VkSubmitInfo();
            ret.sType = VkStructureType.SubmitInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDisplayPropertiesKHR
    {
        public VkDisplayKHR display;
        public byte* displayName;
        public VkExtent2D physicalDimensions;
        public VkExtent2D physicalResolution;
        public VkSurfaceTransformFlagsKHR supportedTransforms;
        public VkBool32 planeReorderPossible;
        public VkBool32 persistentContent;
    }

    public unsafe partial struct VkDisplayPlanePropertiesKHR
    {
        public VkDisplayKHR currentDisplay;
        public uint currentStackIndex;
    }

    public unsafe partial struct VkDisplayModeParametersKHR
    {
        public VkExtent2D visibleRegion;
        public uint refreshRate;
    }

    public unsafe partial struct VkDisplayModePropertiesKHR
    {
        public VkDisplayModeKHR displayMode;
        public VkDisplayModeParametersKHR parameters;
    }

    public unsafe partial struct VkDisplayModeCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkDisplayModeParametersKHR parameters;
        public static VkDisplayModeCreateInfoKHR New()
        {
            VkDisplayModeCreateInfoKHR ret = new VkDisplayModeCreateInfoKHR();
            ret.sType = VkStructureType.DisplayModeCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkDisplayPlaneCapabilitiesKHR
    {
        public VkDisplayPlaneAlphaFlagsKHR supportedAlpha;
        public VkOffset2D minSrcPosition;
        public VkOffset2D maxSrcPosition;
        public VkExtent2D minSrcExtent;
        public VkExtent2D maxSrcExtent;
        public VkOffset2D minDstPosition;
        public VkOffset2D maxDstPosition;
        public VkExtent2D minDstExtent;
        public VkExtent2D maxDstExtent;
    }

    public unsafe partial struct VkDisplaySurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkDisplayModeKHR displayMode;
        public uint planeIndex;
        public uint planeStackIndex;
        public VkSurfaceTransformFlagsKHR transform;
        public float globalAlpha;
        public VkDisplayPlaneAlphaFlagsKHR alphaMode;
        public VkExtent2D imageExtent;
        public static VkDisplaySurfaceCreateInfoKHR New()
        {
            VkDisplaySurfaceCreateInfoKHR ret = new VkDisplaySurfaceCreateInfoKHR();
            ret.sType = VkStructureType.DisplaySurfaceCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkDisplayPresentInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRect2D srcRect;
        public VkRect2D dstRect;
        public VkBool32 persistent;
        public static VkDisplayPresentInfoKHR New()
        {
            VkDisplayPresentInfoKHR ret = new VkDisplayPresentInfoKHR();
            ret.sType = VkStructureType.DisplayPresentInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkSurfaceCapabilitiesKHR
    {
        public uint minImageCount;
        public uint maxImageCount;
        public VkExtent2D currentExtent;
        public VkExtent2D minImageExtent;
        public VkExtent2D maxImageExtent;
        public uint maxImageArrayLayers;
        public VkSurfaceTransformFlagsKHR supportedTransforms;
        public VkSurfaceTransformFlagsKHR currentTransform;
        public VkCompositeAlphaFlagsKHR supportedCompositeAlpha;
        public VkImageUsageFlags supportedUsageFlags;
    }

    public unsafe partial struct VkAndroidSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Android.ANativeWindow* window;
        public static VkAndroidSurfaceCreateInfoKHR New()
        {
            VkAndroidSurfaceCreateInfoKHR ret = new VkAndroidSurfaceCreateInfoKHR();
            ret.sType = VkStructureType.AndroidSurfaceCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkMirSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Mir.MirConnection* connection;
        public Mir.MirSurface* mirSurface;
        public static VkMirSurfaceCreateInfoKHR New()
        {
            VkMirSurfaceCreateInfoKHR ret = new VkMirSurfaceCreateInfoKHR();
            ret.sType = VkStructureType.MirSurfaceCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkViSurfaceCreateInfoNN
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public void* window;
        public static VkViSurfaceCreateInfoNN New()
        {
            VkViSurfaceCreateInfoNN ret = new VkViSurfaceCreateInfoNN();
            ret.sType = VkStructureType.ViSurfaceCreateInfoNn;
            return ret;
        }
    }

    public unsafe partial struct VkWaylandSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Wayland.wl_display* display;
        public Wayland.wl_surface* surface;
        public static VkWaylandSurfaceCreateInfoKHR New()
        {
            VkWaylandSurfaceCreateInfoKHR ret = new VkWaylandSurfaceCreateInfoKHR();
            ret.sType = VkStructureType.WaylandSurfaceCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkWin32SurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Win32.HINSTANCE hinstance;
        public Win32.HWND hwnd;
        public static VkWin32SurfaceCreateInfoKHR New()
        {
            VkWin32SurfaceCreateInfoKHR ret = new VkWin32SurfaceCreateInfoKHR();
            ret.sType = VkStructureType.Win32SurfaceCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkXlibSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Xlib.Display* dpy;
        public Xlib.Window window;
        public static VkXlibSurfaceCreateInfoKHR New()
        {
            VkXlibSurfaceCreateInfoKHR ret = new VkXlibSurfaceCreateInfoKHR();
            ret.sType = VkStructureType.XlibSurfaceCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkXcbSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Xcb.xcb_connection_t* connection;
        public Xcb.xcb_window_t window;
        public static VkXcbSurfaceCreateInfoKHR New()
        {
            VkXcbSurfaceCreateInfoKHR ret = new VkXcbSurfaceCreateInfoKHR();
            ret.sType = VkStructureType.XcbSurfaceCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkSurfaceFormatKHR
    {
        public VkFormat format;
        public VkColorSpaceKHR colorSpace;
    }

    public unsafe partial struct VkSwapchainCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSwapchainCreateFlagsKHR flags;
        public VkSurfaceKHR surface;
        public uint minImageCount;
        public VkFormat imageFormat;
        public VkColorSpaceKHR imageColorSpace;
        public VkExtent2D imageExtent;
        public uint imageArrayLayers;
        public VkImageUsageFlags imageUsage;
        public VkSharingMode imageSharingMode;
        public uint queueFamilyIndexCount;
        public uint* pQueueFamilyIndices;
        public VkSurfaceTransformFlagsKHR preTransform;
        public VkCompositeAlphaFlagsKHR compositeAlpha;
        public VkPresentModeKHR presentMode;
        public VkBool32 clipped;
        public VkSwapchainKHR oldSwapchain;
        public static VkSwapchainCreateInfoKHR New()
        {
            VkSwapchainCreateInfoKHR ret = new VkSwapchainCreateInfoKHR();
            ret.sType = VkStructureType.SwapchainCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPresentInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint waitSemaphoreCount;
        public VkSemaphore* pWaitSemaphores;
        public uint swapchainCount;
        public VkSwapchainKHR* pSwapchains;
        public uint* pImageIndices;
        public VkResult* pResults;
        public static VkPresentInfoKHR New()
        {
            VkPresentInfoKHR ret = new VkPresentInfoKHR();
            ret.sType = VkStructureType.PresentInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkDebugReportCallbackCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugReportFlagsEXT flags;
        public IntPtr pfnCallback;
        public void* pUserData;
        public static VkDebugReportCallbackCreateInfoEXT New()
        {
            VkDebugReportCallbackCreateInfoEXT ret = new VkDebugReportCallbackCreateInfoEXT();
            ret.sType = VkStructureType.DebugReportCallbackCreateInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkValidationFlagsEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint disabledValidationCheckCount;
        public VkValidationCheckEXT* pDisabledValidationChecks;
        public static VkValidationFlagsEXT New()
        {
            VkValidationFlagsEXT ret = new VkValidationFlagsEXT();
            ret.sType = VkStructureType.ValidationEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineRasterizationStateRasterizationOrderAMD
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRasterizationOrderAMD rasterizationOrder;
        public static VkPipelineRasterizationStateRasterizationOrderAMD New()
        {
            VkPipelineRasterizationStateRasterizationOrderAMD ret = new VkPipelineRasterizationStateRasterizationOrderAMD();
            ret.sType = VkStructureType.PipelineRasterizationStateRasterizationOrderAMD;
            return ret;
        }
    }

    public unsafe partial struct VkDebugMarkerObjectNameInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugReportObjectTypeEXT objectType;
        public ulong @object;
        public byte* pObjectName;
        public static VkDebugMarkerObjectNameInfoEXT New()
        {
            VkDebugMarkerObjectNameInfoEXT ret = new VkDebugMarkerObjectNameInfoEXT();
            ret.sType = VkStructureType.DebugMarkerObjectNameInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkDebugMarkerObjectTagInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugReportObjectTypeEXT objectType;
        public ulong @object;
        public ulong tagName;
        public UIntPtr tagSize;
        public void* pTag;
        public static VkDebugMarkerObjectTagInfoEXT New()
        {
            VkDebugMarkerObjectTagInfoEXT ret = new VkDebugMarkerObjectTagInfoEXT();
            ret.sType = VkStructureType.DebugMarkerObjectTagInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkDebugMarkerMarkerInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public byte* pMarkerName;
        public float color_0;
        public float color_1;
        public float color_2;
        public float color_3;
        public static VkDebugMarkerMarkerInfoEXT New()
        {
            VkDebugMarkerMarkerInfoEXT ret = new VkDebugMarkerMarkerInfoEXT();
            ret.sType = VkStructureType.DebugMarkerMarkerInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkDedicatedAllocationImageCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 dedicatedAllocation;
        public static VkDedicatedAllocationImageCreateInfoNV New()
        {
            VkDedicatedAllocationImageCreateInfoNV ret = new VkDedicatedAllocationImageCreateInfoNV();
            ret.sType = VkStructureType.DedicatedAllocationImageCreateInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkDedicatedAllocationBufferCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 dedicatedAllocation;
        public static VkDedicatedAllocationBufferCreateInfoNV New()
        {
            VkDedicatedAllocationBufferCreateInfoNV ret = new VkDedicatedAllocationBufferCreateInfoNV();
            ret.sType = VkStructureType.DedicatedAllocationBufferCreateInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkDedicatedAllocationMemoryAllocateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImage image;
        public VkBuffer buffer;
        public static VkDedicatedAllocationMemoryAllocateInfoNV New()
        {
            VkDedicatedAllocationMemoryAllocateInfoNV ret = new VkDedicatedAllocationMemoryAllocateInfoNV();
            ret.sType = VkStructureType.DedicatedAllocationMemoryAllocateInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkExternalImageFormatPropertiesNV
    {
        public VkImageFormatProperties imageFormatProperties;
        public VkExternalMemoryFeatureFlagsNV externalMemoryFeatures;
        public VkExternalMemoryHandleTypeFlagsNV exportFromImportedHandleTypes;
        public VkExternalMemoryHandleTypeFlagsNV compatibleHandleTypes;
    }

    public unsafe partial struct VkExternalMemoryImageCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsNV handleTypes;
        public static VkExternalMemoryImageCreateInfoNV New()
        {
            VkExternalMemoryImageCreateInfoNV ret = new VkExternalMemoryImageCreateInfoNV();
            ret.sType = VkStructureType.ExternalMemoryImageCreateInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkExportMemoryAllocateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsNV handleTypes;
        public static VkExportMemoryAllocateInfoNV New()
        {
            VkExportMemoryAllocateInfoNV ret = new VkExportMemoryAllocateInfoNV();
            ret.sType = VkStructureType.ExportMemoryAllocateInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkImportMemoryWin32HandleInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsNV handleType;
        public Win32.HANDLE handle;
        public static VkImportMemoryWin32HandleInfoNV New()
        {
            VkImportMemoryWin32HandleInfoNV ret = new VkImportMemoryWin32HandleInfoNV();
            ret.sType = VkStructureType.ImportMemoryWin32HandleInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkExportMemoryWin32HandleInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public Win32.SECURITY_ATTRIBUTES* pAttributes;
        public uint dwAccess;
        public static VkExportMemoryWin32HandleInfoNV New()
        {
            VkExportMemoryWin32HandleInfoNV ret = new VkExportMemoryWin32HandleInfoNV();
            ret.sType = VkStructureType.ExportMemoryWin32HandleInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkWin32KeyedMutexAcquireReleaseInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public uint acquireCount;
        public VkDeviceMemory* pAcquireSyncs;
        public ulong* pAcquireKeys;
        public uint* pAcquireTimeoutMilliseconds;
        public uint releaseCount;
        public VkDeviceMemory* pReleaseSyncs;
        public ulong* pReleaseKeys;
        public static VkWin32KeyedMutexAcquireReleaseInfoNV New()
        {
            VkWin32KeyedMutexAcquireReleaseInfoNV ret = new VkWin32KeyedMutexAcquireReleaseInfoNV();
            ret.sType = VkStructureType.Win32KeyedMutexAcquireReleaseInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGeneratedCommandsFeaturesNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 computeBindingPointSupport;
        public static VkDeviceGeneratedCommandsFeaturesNVX New()
        {
            VkDeviceGeneratedCommandsFeaturesNVX ret = new VkDeviceGeneratedCommandsFeaturesNVX();
            ret.sType = VkStructureType.DeviceGeneratedCommandsFeaturesNVX;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGeneratedCommandsLimitsNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint maxIndirectCommandsLayoutTokenCount;
        public uint maxObjectEntryCounts;
        public uint minSequenceCountBufferOffsetAlignment;
        public uint minSequenceIndexBufferOffsetAlignment;
        public uint minCommandsTokenBufferOffsetAlignment;
        public static VkDeviceGeneratedCommandsLimitsNVX New()
        {
            VkDeviceGeneratedCommandsLimitsNVX ret = new VkDeviceGeneratedCommandsLimitsNVX();
            ret.sType = VkStructureType.DeviceGeneratedCommandsLimitsNVX;
            return ret;
        }
    }

    public unsafe partial struct VkIndirectCommandsTokenNVX
    {
        public VkIndirectCommandsTokenTypeNVX tokenType;
        public VkBuffer buffer;
        public ulong offset;
    }

    public unsafe partial struct VkIndirectCommandsLayoutTokenNVX
    {
        public VkIndirectCommandsTokenTypeNVX tokenType;
        public uint bindingUnit;
        public uint dynamicCount;
        public uint divisor;
    }

    public unsafe partial struct VkIndirectCommandsLayoutCreateInfoNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineBindPoint pipelineBindPoint;
        public VkIndirectCommandsLayoutUsageFlagsNVX flags;
        public uint tokenCount;
        public VkIndirectCommandsLayoutTokenNVX* pTokens;
        public static VkIndirectCommandsLayoutCreateInfoNVX New()
        {
            VkIndirectCommandsLayoutCreateInfoNVX ret = new VkIndirectCommandsLayoutCreateInfoNVX();
            ret.sType = VkStructureType.IndirectCommandsLayoutCreateInfoNVX;
            return ret;
        }
    }

    public unsafe partial struct VkCmdProcessCommandsInfoNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkObjectTableNVX objectTable;
        public VkIndirectCommandsLayoutNVX indirectCommandsLayout;
        public uint indirectCommandsTokenCount;
        public VkIndirectCommandsTokenNVX* pIndirectCommandsTokens;
        public uint maxSequencesCount;
        public VkCommandBuffer targetCommandBuffer;
        public VkBuffer sequencesCountBuffer;
        public ulong sequencesCountOffset;
        public VkBuffer sequencesIndexBuffer;
        public ulong sequencesIndexOffset;
        public static VkCmdProcessCommandsInfoNVX New()
        {
            VkCmdProcessCommandsInfoNVX ret = new VkCmdProcessCommandsInfoNVX();
            ret.sType = VkStructureType.CmdProcessCommandsInfoNVX;
            return ret;
        }
    }

    public unsafe partial struct VkCmdReserveSpaceForCommandsInfoNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkObjectTableNVX objectTable;
        public VkIndirectCommandsLayoutNVX indirectCommandsLayout;
        public uint maxSequencesCount;
        public static VkCmdReserveSpaceForCommandsInfoNVX New()
        {
            VkCmdReserveSpaceForCommandsInfoNVX ret = new VkCmdReserveSpaceForCommandsInfoNVX();
            ret.sType = VkStructureType.CmdReserveSpaceForCommandsInfoNVX;
            return ret;
        }
    }

    public unsafe partial struct VkObjectTableCreateInfoNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint objectCount;
        public VkObjectEntryTypeNVX* pObjectEntryTypes;
        public uint* pObjectEntryCounts;
        public VkObjectEntryUsageFlagsNVX* pObjectEntryUsageFlags;
        public uint maxUniformBuffersPerDescriptor;
        public uint maxStorageBuffersPerDescriptor;
        public uint maxStorageImagesPerDescriptor;
        public uint maxSampledImagesPerDescriptor;
        public uint maxPipelineLayouts;
        public static VkObjectTableCreateInfoNVX New()
        {
            VkObjectTableCreateInfoNVX ret = new VkObjectTableCreateInfoNVX();
            ret.sType = VkStructureType.ObjectTableCreateInfoNVX;
            return ret;
        }
    }

    public unsafe partial struct VkObjectTableEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
    }

    public unsafe partial struct VkObjectTablePipelineEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
        public VkPipeline pipeline;
    }

    public unsafe partial struct VkObjectTableDescriptorSetEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
        public VkPipelineLayout pipelineLayout;
        public VkDescriptorSet descriptorSet;
    }

    public unsafe partial struct VkObjectTableVertexBufferEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
        public VkBuffer buffer;
    }

    public unsafe partial struct VkObjectTableIndexBufferEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
        public VkBuffer buffer;
        public VkIndexType indexType;
    }

    public unsafe partial struct VkObjectTablePushConstantEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
        public VkPipelineLayout pipelineLayout;
        public VkShaderStageFlags stageFlags;
    }

    public unsafe partial struct VkPhysicalDeviceFeatures2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPhysicalDeviceFeatures features;
        public static VkPhysicalDeviceFeatures2KHR New()
        {
            VkPhysicalDeviceFeatures2KHR ret = new VkPhysicalDeviceFeatures2KHR();
            ret.sType = VkStructureType.PhysicalDeviceFeatures2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceProperties2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPhysicalDeviceProperties properties;
        public static VkPhysicalDeviceProperties2KHR New()
        {
            VkPhysicalDeviceProperties2KHR ret = new VkPhysicalDeviceProperties2KHR();
            ret.sType = VkStructureType.PhysicalDeviceProperties2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkFormatProperties2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFormatProperties formatProperties;
        public static VkFormatProperties2KHR New()
        {
            VkFormatProperties2KHR ret = new VkFormatProperties2KHR();
            ret.sType = VkStructureType.FormatProperties2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkImageFormatProperties2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImageFormatProperties imageFormatProperties;
        public static VkImageFormatProperties2KHR New()
        {
            VkImageFormatProperties2KHR ret = new VkImageFormatProperties2KHR();
            ret.sType = VkStructureType.ImageFormatProperties2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceImageFormatInfo2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFormat format;
        public VkImageType type;
        public VkImageTiling tiling;
        public VkImageUsageFlags usage;
        public VkImageCreateFlags flags;
        public static VkPhysicalDeviceImageFormatInfo2KHR New()
        {
            VkPhysicalDeviceImageFormatInfo2KHR ret = new VkPhysicalDeviceImageFormatInfo2KHR();
            ret.sType = VkStructureType.PhysicalDeviceImageFormatInfo2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkQueueFamilyProperties2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkQueueFamilyProperties queueFamilyProperties;
        public static VkQueueFamilyProperties2KHR New()
        {
            VkQueueFamilyProperties2KHR ret = new VkQueueFamilyProperties2KHR();
            ret.sType = VkStructureType.QueueFamilyProperties2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceMemoryProperties2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPhysicalDeviceMemoryProperties memoryProperties;
        public static VkPhysicalDeviceMemoryProperties2KHR New()
        {
            VkPhysicalDeviceMemoryProperties2KHR ret = new VkPhysicalDeviceMemoryProperties2KHR();
            ret.sType = VkStructureType.PhysicalDeviceMemoryProperties2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkSparseImageFormatProperties2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSparseImageFormatProperties properties;
        public static VkSparseImageFormatProperties2KHR New()
        {
            VkSparseImageFormatProperties2KHR ret = new VkSparseImageFormatProperties2KHR();
            ret.sType = VkStructureType.SparseImageFormatProperties2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceSparseImageFormatInfo2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFormat format;
        public VkImageType type;
        public VkSampleCountFlags samples;
        public VkImageUsageFlags usage;
        public VkImageTiling tiling;
        public static VkPhysicalDeviceSparseImageFormatInfo2KHR New()
        {
            VkPhysicalDeviceSparseImageFormatInfo2KHR ret = new VkPhysicalDeviceSparseImageFormatInfo2KHR();
            ret.sType = VkStructureType.PhysicalDeviceSparseImageFormatInfo2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDevicePushDescriptorPropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint maxPushDescriptors;
        public static VkPhysicalDevicePushDescriptorPropertiesKHR New()
        {
            VkPhysicalDevicePushDescriptorPropertiesKHR ret = new VkPhysicalDevicePushDescriptorPropertiesKHR();
            ret.sType = VkStructureType.PhysicalDevicePushDescriptorPropertiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPresentRegionsKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint swapchainCount;
        public VkPresentRegionKHR* pRegions;
        public static VkPresentRegionsKHR New()
        {
            VkPresentRegionsKHR ret = new VkPresentRegionsKHR();
            ret.sType = VkStructureType.PresentRegionsKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPresentRegionKHR
    {
        public uint rectangleCount;
        public VkRectLayerKHR* pRectangles;
    }

    public unsafe partial struct VkRectLayerKHR
    {
        public VkOffset2D offset;
        public VkExtent2D extent;
        public uint layer;
    }

    public unsafe partial struct VkPhysicalDeviceVariablePointerFeaturesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 variablePointersStorageBuffer;
        public VkBool32 variablePointers;
        public static VkPhysicalDeviceVariablePointerFeaturesKHR New()
        {
            VkPhysicalDeviceVariablePointerFeaturesKHR ret = new VkPhysicalDeviceVariablePointerFeaturesKHR();
            ret.sType = VkStructureType.PhysicalDeviceVariablePointerFeaturesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExternalMemoryPropertiesKHR
    {
        public VkExternalMemoryFeatureFlagsKHR externalMemoryFeatures;
        public VkExternalMemoryHandleTypeFlagsKHR exportFromImportedHandleTypes;
        public VkExternalMemoryHandleTypeFlagsKHR compatibleHandleTypes;
    }

    public unsafe partial struct VkPhysicalDeviceExternalImageFormatInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsKHR handleType;
        public static VkPhysicalDeviceExternalImageFormatInfoKHR New()
        {
            VkPhysicalDeviceExternalImageFormatInfoKHR ret = new VkPhysicalDeviceExternalImageFormatInfoKHR();
            ret.sType = VkStructureType.PhysicalDeviceExternalImageFormatInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExternalImageFormatPropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryPropertiesKHR externalMemoryProperties;
        public static VkExternalImageFormatPropertiesKHR New()
        {
            VkExternalImageFormatPropertiesKHR ret = new VkExternalImageFormatPropertiesKHR();
            ret.sType = VkStructureType.ExternalImageFormatPropertiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceExternalBufferInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBufferCreateFlags flags;
        public VkBufferUsageFlags usage;
        public VkExternalMemoryHandleTypeFlagsKHR handleType;
        public static VkPhysicalDeviceExternalBufferInfoKHR New()
        {
            VkPhysicalDeviceExternalBufferInfoKHR ret = new VkPhysicalDeviceExternalBufferInfoKHR();
            ret.sType = VkStructureType.PhysicalDeviceExternalBufferInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExternalBufferPropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryPropertiesKHR externalMemoryProperties;
        public static VkExternalBufferPropertiesKHR New()
        {
            VkExternalBufferPropertiesKHR ret = new VkExternalBufferPropertiesKHR();
            ret.sType = VkStructureType.ExternalBufferPropertiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceIDPropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public fixed byte deviceUUID[(int)VulkanNative.UuidSize];
        public fixed byte driverUUID[(int)VulkanNative.UuidSize];
        public fixed byte deviceLUID[(int)VulkanNative.LuidSizeKHR];
        public uint deviceNodeMask;
        public VkBool32 deviceLUIDValid;
        public static VkPhysicalDeviceIDPropertiesKHR New()
        {
            VkPhysicalDeviceIDPropertiesKHR ret = new VkPhysicalDeviceIDPropertiesKHR();
            ret.sType = VkStructureType.PhysicalDeviceIdPropertiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExternalMemoryImageCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsKHR handleTypes;
        public static VkExternalMemoryImageCreateInfoKHR New()
        {
            VkExternalMemoryImageCreateInfoKHR ret = new VkExternalMemoryImageCreateInfoKHR();
            ret.sType = VkStructureType.ExternalMemoryImageCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExternalMemoryBufferCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsKHR handleTypes;
        public static VkExternalMemoryBufferCreateInfoKHR New()
        {
            VkExternalMemoryBufferCreateInfoKHR ret = new VkExternalMemoryBufferCreateInfoKHR();
            ret.sType = VkStructureType.ExternalMemoryBufferCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExportMemoryAllocateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsKHR handleTypes;
        public static VkExportMemoryAllocateInfoKHR New()
        {
            VkExportMemoryAllocateInfoKHR ret = new VkExportMemoryAllocateInfoKHR();
            ret.sType = VkStructureType.ExportMemoryAllocateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkImportMemoryWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsKHR handleType;
        public Win32.HANDLE handle;
        public IntPtr name;
        public static VkImportMemoryWin32HandleInfoKHR New()
        {
            VkImportMemoryWin32HandleInfoKHR ret = new VkImportMemoryWin32HandleInfoKHR();
            ret.sType = VkStructureType.ImportMemoryWin32HandleInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExportMemoryWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public Win32.SECURITY_ATTRIBUTES* pAttributes;
        public uint dwAccess;
        public IntPtr name;
        public static VkExportMemoryWin32HandleInfoKHR New()
        {
            VkExportMemoryWin32HandleInfoKHR ret = new VkExportMemoryWin32HandleInfoKHR();
            ret.sType = VkStructureType.ExportMemoryWin32HandleInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryWin32HandlePropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint memoryTypeBits;
        public static VkMemoryWin32HandlePropertiesKHR New()
        {
            VkMemoryWin32HandlePropertiesKHR ret = new VkMemoryWin32HandlePropertiesKHR();
            ret.sType = VkStructureType.MemoryWin32HandlePropertiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryGetWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceMemory memory;
        public VkExternalMemoryHandleTypeFlagsKHR handleType;
        public static VkMemoryGetWin32HandleInfoKHR New()
        {
            VkMemoryGetWin32HandleInfoKHR ret = new VkMemoryGetWin32HandleInfoKHR();
            ret.sType = VkStructureType.MemoryGetWin32HandleInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkImportMemoryFdInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsKHR handleType;
        public int fd;
        public static VkImportMemoryFdInfoKHR New()
        {
            VkImportMemoryFdInfoKHR ret = new VkImportMemoryFdInfoKHR();
            ret.sType = VkStructureType.ImportMemoryFdInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryFdPropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint memoryTypeBits;
        public static VkMemoryFdPropertiesKHR New()
        {
            VkMemoryFdPropertiesKHR ret = new VkMemoryFdPropertiesKHR();
            ret.sType = VkStructureType.MemoryFdPropertiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryGetFdInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceMemory memory;
        public VkExternalMemoryHandleTypeFlagsKHR handleType;
        public static VkMemoryGetFdInfoKHR New()
        {
            VkMemoryGetFdInfoKHR ret = new VkMemoryGetFdInfoKHR();
            ret.sType = VkStructureType.MemoryGetFdInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkWin32KeyedMutexAcquireReleaseInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint acquireCount;
        public VkDeviceMemory* pAcquireSyncs;
        public ulong* pAcquireKeys;
        public uint* pAcquireTimeouts;
        public uint releaseCount;
        public VkDeviceMemory* pReleaseSyncs;
        public ulong* pReleaseKeys;
        public static VkWin32KeyedMutexAcquireReleaseInfoKHR New()
        {
            VkWin32KeyedMutexAcquireReleaseInfoKHR ret = new VkWin32KeyedMutexAcquireReleaseInfoKHR();
            ret.sType = VkStructureType.Win32KeyedMutexAcquireReleaseInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceExternalSemaphoreInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalSemaphoreHandleTypeFlagsKHR handleType;
        public static VkPhysicalDeviceExternalSemaphoreInfoKHR New()
        {
            VkPhysicalDeviceExternalSemaphoreInfoKHR ret = new VkPhysicalDeviceExternalSemaphoreInfoKHR();
            ret.sType = VkStructureType.PhysicalDeviceExternalSemaphoreInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExternalSemaphorePropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalSemaphoreHandleTypeFlagsKHR exportFromImportedHandleTypes;
        public VkExternalSemaphoreHandleTypeFlagsKHR compatibleHandleTypes;
        public VkExternalSemaphoreFeatureFlagsKHR externalSemaphoreFeatures;
        public static VkExternalSemaphorePropertiesKHR New()
        {
            VkExternalSemaphorePropertiesKHR ret = new VkExternalSemaphorePropertiesKHR();
            ret.sType = VkStructureType.ExternalSemaphorePropertiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExportSemaphoreCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalSemaphoreHandleTypeFlagsKHR handleTypes;
        public static VkExportSemaphoreCreateInfoKHR New()
        {
            VkExportSemaphoreCreateInfoKHR ret = new VkExportSemaphoreCreateInfoKHR();
            ret.sType = VkStructureType.ExportSemaphoreCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkImportSemaphoreWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSemaphore semaphore;
        public VkSemaphoreImportFlagsKHR flags;
        public VkExternalSemaphoreHandleTypeFlagsKHR handleType;
        public Win32.HANDLE handle;
        public IntPtr name;
        public static VkImportSemaphoreWin32HandleInfoKHR New()
        {
            VkImportSemaphoreWin32HandleInfoKHR ret = new VkImportSemaphoreWin32HandleInfoKHR();
            ret.sType = VkStructureType.ImportSemaphoreWin32HandleInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExportSemaphoreWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public Win32.SECURITY_ATTRIBUTES* pAttributes;
        public uint dwAccess;
        public IntPtr name;
        public static VkExportSemaphoreWin32HandleInfoKHR New()
        {
            VkExportSemaphoreWin32HandleInfoKHR ret = new VkExportSemaphoreWin32HandleInfoKHR();
            ret.sType = VkStructureType.ExportSemaphoreWin32HandleInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkD3D12FenceSubmitInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint waitSemaphoreValuesCount;
        public ulong* pWaitSemaphoreValues;
        public uint signalSemaphoreValuesCount;
        public ulong* pSignalSemaphoreValues;
        public static VkD3D12FenceSubmitInfoKHR New()
        {
            VkD3D12FenceSubmitInfoKHR ret = new VkD3D12FenceSubmitInfoKHR();
            ret.sType = VkStructureType.D3d12FenceSubmitInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkSemaphoreGetWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSemaphore semaphore;
        public VkExternalSemaphoreHandleTypeFlagsKHR handleType;
        public static VkSemaphoreGetWin32HandleInfoKHR New()
        {
            VkSemaphoreGetWin32HandleInfoKHR ret = new VkSemaphoreGetWin32HandleInfoKHR();
            ret.sType = VkStructureType.SemaphoreGetWin32HandleInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkImportSemaphoreFdInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSemaphore semaphore;
        public VkSemaphoreImportFlagsKHR flags;
        public VkExternalSemaphoreHandleTypeFlagsKHR handleType;
        public int fd;
        public static VkImportSemaphoreFdInfoKHR New()
        {
            VkImportSemaphoreFdInfoKHR ret = new VkImportSemaphoreFdInfoKHR();
            ret.sType = VkStructureType.ImportSemaphoreFdInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkSemaphoreGetFdInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSemaphore semaphore;
        public VkExternalSemaphoreHandleTypeFlagsKHR handleType;
        public static VkSemaphoreGetFdInfoKHR New()
        {
            VkSemaphoreGetFdInfoKHR ret = new VkSemaphoreGetFdInfoKHR();
            ret.sType = VkStructureType.SemaphoreGetFdInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceExternalFenceInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalFenceHandleTypeFlagsKHR handleType;
        public static VkPhysicalDeviceExternalFenceInfoKHR New()
        {
            VkPhysicalDeviceExternalFenceInfoKHR ret = new VkPhysicalDeviceExternalFenceInfoKHR();
            ret.sType = VkStructureType.PhysicalDeviceExternalFenceInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExternalFencePropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalFenceHandleTypeFlagsKHR exportFromImportedHandleTypes;
        public VkExternalFenceHandleTypeFlagsKHR compatibleHandleTypes;
        public VkExternalFenceFeatureFlagsKHR externalFenceFeatures;
        public static VkExternalFencePropertiesKHR New()
        {
            VkExternalFencePropertiesKHR ret = new VkExternalFencePropertiesKHR();
            ret.sType = VkStructureType.ExternalFencePropertiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExportFenceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalFenceHandleTypeFlagsKHR handleTypes;
        public static VkExportFenceCreateInfoKHR New()
        {
            VkExportFenceCreateInfoKHR ret = new VkExportFenceCreateInfoKHR();
            ret.sType = VkStructureType.ExportFenceCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkImportFenceWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFence fence;
        public VkFenceImportFlagsKHR flags;
        public VkExternalFenceHandleTypeFlagsKHR handleType;
        public Win32.HANDLE handle;
        public IntPtr name;
        public static VkImportFenceWin32HandleInfoKHR New()
        {
            VkImportFenceWin32HandleInfoKHR ret = new VkImportFenceWin32HandleInfoKHR();
            ret.sType = VkStructureType.ImportFenceWin32HandleInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkExportFenceWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public Win32.SECURITY_ATTRIBUTES* pAttributes;
        public uint dwAccess;
        public IntPtr name;
        public static VkExportFenceWin32HandleInfoKHR New()
        {
            VkExportFenceWin32HandleInfoKHR ret = new VkExportFenceWin32HandleInfoKHR();
            ret.sType = VkStructureType.ExportFenceWin32HandleInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkFenceGetWin32HandleInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFence fence;
        public VkExternalFenceHandleTypeFlagsKHR handleType;
        public static VkFenceGetWin32HandleInfoKHR New()
        {
            VkFenceGetWin32HandleInfoKHR ret = new VkFenceGetWin32HandleInfoKHR();
            ret.sType = VkStructureType.FenceGetWin32HandleInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkImportFenceFdInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFence fence;
        public VkFenceImportFlagsKHR flags;
        public VkExternalFenceHandleTypeFlagsKHR handleType;
        public int fd;
        public static VkImportFenceFdInfoKHR New()
        {
            VkImportFenceFdInfoKHR ret = new VkImportFenceFdInfoKHR();
            ret.sType = VkStructureType.ImportFenceFdInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkFenceGetFdInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFence fence;
        public VkExternalFenceHandleTypeFlagsKHR handleType;
        public static VkFenceGetFdInfoKHR New()
        {
            VkFenceGetFdInfoKHR ret = new VkFenceGetFdInfoKHR();
            ret.sType = VkStructureType.FenceGetFdInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceMultiviewFeaturesKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 multiview;
        public VkBool32 multiviewGeometryShader;
        public VkBool32 multiviewTessellationShader;
        public static VkPhysicalDeviceMultiviewFeaturesKHX New()
        {
            VkPhysicalDeviceMultiviewFeaturesKHX ret = new VkPhysicalDeviceMultiviewFeaturesKHX();
            ret.sType = VkStructureType.PhysicalDeviceMultiviewFeaturesKHX;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceMultiviewPropertiesKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint maxMultiviewViewCount;
        public uint maxMultiviewInstanceIndex;
        public static VkPhysicalDeviceMultiviewPropertiesKHX New()
        {
            VkPhysicalDeviceMultiviewPropertiesKHX ret = new VkPhysicalDeviceMultiviewPropertiesKHX();
            ret.sType = VkStructureType.PhysicalDeviceMultiviewPropertiesKHX;
            return ret;
        }
    }

    public unsafe partial struct VkRenderPassMultiviewCreateInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint subpassCount;
        public uint* pViewMasks;
        public uint dependencyCount;
        public int* pViewOffsets;
        public uint correlationMaskCount;
        public uint* pCorrelationMasks;
        public static VkRenderPassMultiviewCreateInfoKHX New()
        {
            VkRenderPassMultiviewCreateInfoKHX ret = new VkRenderPassMultiviewCreateInfoKHX();
            ret.sType = VkStructureType.RenderPassMultiviewCreateInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkSurfaceCapabilities2EXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint minImageCount;
        public uint maxImageCount;
        public VkExtent2D currentExtent;
        public VkExtent2D minImageExtent;
        public VkExtent2D maxImageExtent;
        public uint maxImageArrayLayers;
        public VkSurfaceTransformFlagsKHR supportedTransforms;
        public VkSurfaceTransformFlagsKHR currentTransform;
        public VkCompositeAlphaFlagsKHR supportedCompositeAlpha;
        public VkImageUsageFlags supportedUsageFlags;
        public VkSurfaceCounterFlagsEXT supportedSurfaceCounters;
        public static VkSurfaceCapabilities2EXT New()
        {
            VkSurfaceCapabilities2EXT ret = new VkSurfaceCapabilities2EXT();
            ret.sType = VkStructureType.SurfaceCapabilities2EXT;
            return ret;
        }
    }

    public unsafe partial struct VkDisplayPowerInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDisplayPowerStateEXT powerState;
        public static VkDisplayPowerInfoEXT New()
        {
            VkDisplayPowerInfoEXT ret = new VkDisplayPowerInfoEXT();
            ret.sType = VkStructureType.DisplayPowerInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceEventInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceEventTypeEXT deviceEvent;
        public static VkDeviceEventInfoEXT New()
        {
            VkDeviceEventInfoEXT ret = new VkDeviceEventInfoEXT();
            ret.sType = VkStructureType.DeviceEventInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkDisplayEventInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDisplayEventTypeEXT displayEvent;
        public static VkDisplayEventInfoEXT New()
        {
            VkDisplayEventInfoEXT ret = new VkDisplayEventInfoEXT();
            ret.sType = VkStructureType.DisplayEventInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkSwapchainCounterCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSurfaceCounterFlagsEXT surfaceCounters;
        public static VkSwapchainCounterCreateInfoEXT New()
        {
            VkSwapchainCounterCreateInfoEXT ret = new VkSwapchainCounterCreateInfoEXT();
            ret.sType = VkStructureType.SwapchainCounterCreateInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceGroupPropertiesKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint physicalDeviceCount;
        public VkPhysicalDevice physicalDevices_0;
        public VkPhysicalDevice physicalDevices_1;
        public VkPhysicalDevice physicalDevices_2;
        public VkPhysicalDevice physicalDevices_3;
        public VkPhysicalDevice physicalDevices_4;
        public VkPhysicalDevice physicalDevices_5;
        public VkPhysicalDevice physicalDevices_6;
        public VkPhysicalDevice physicalDevices_7;
        public VkPhysicalDevice physicalDevices_8;
        public VkPhysicalDevice physicalDevices_9;
        public VkPhysicalDevice physicalDevices_10;
        public VkPhysicalDevice physicalDevices_11;
        public VkPhysicalDevice physicalDevices_12;
        public VkPhysicalDevice physicalDevices_13;
        public VkPhysicalDevice physicalDevices_14;
        public VkPhysicalDevice physicalDevices_15;
        public VkPhysicalDevice physicalDevices_16;
        public VkPhysicalDevice physicalDevices_17;
        public VkPhysicalDevice physicalDevices_18;
        public VkPhysicalDevice physicalDevices_19;
        public VkPhysicalDevice physicalDevices_20;
        public VkPhysicalDevice physicalDevices_21;
        public VkPhysicalDevice physicalDevices_22;
        public VkPhysicalDevice physicalDevices_23;
        public VkPhysicalDevice physicalDevices_24;
        public VkPhysicalDevice physicalDevices_25;
        public VkPhysicalDevice physicalDevices_26;
        public VkPhysicalDevice physicalDevices_27;
        public VkPhysicalDevice physicalDevices_28;
        public VkPhysicalDevice physicalDevices_29;
        public VkPhysicalDevice physicalDevices_30;
        public VkPhysicalDevice physicalDevices_31;
        public VkBool32 subsetAllocation;
        public static VkPhysicalDeviceGroupPropertiesKHX New()
        {
            VkPhysicalDeviceGroupPropertiesKHX ret = new VkPhysicalDeviceGroupPropertiesKHX();
            ret.sType = VkStructureType.PhysicalDeviceGroupPropertiesKHX;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryAllocateFlagsInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkMemoryAllocateFlagsKHX flags;
        public uint deviceMask;
        public static VkMemoryAllocateFlagsInfoKHX New()
        {
            VkMemoryAllocateFlagsInfoKHX ret = new VkMemoryAllocateFlagsInfoKHX();
            ret.sType = VkStructureType.MemoryAllocateInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkBindBufferMemoryInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBuffer buffer;
        public VkDeviceMemory memory;
        public ulong memoryOffset;
        public static VkBindBufferMemoryInfoKHR New()
        {
            VkBindBufferMemoryInfoKHR ret = new VkBindBufferMemoryInfoKHR();
            ret.sType = VkStructureType.BindBufferMemoryInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkBindBufferMemoryDeviceGroupInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint deviceIndexCount;
        public uint* pDeviceIndices;
        public static VkBindBufferMemoryDeviceGroupInfoKHX New()
        {
            VkBindBufferMemoryDeviceGroupInfoKHX ret = new VkBindBufferMemoryDeviceGroupInfoKHX();
            ret.sType = VkStructureType.BindBufferMemoryDeviceGroupInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkBindImageMemoryInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImage image;
        public VkDeviceMemory memory;
        public ulong memoryOffset;
        public static VkBindImageMemoryInfoKHR New()
        {
            VkBindImageMemoryInfoKHR ret = new VkBindImageMemoryInfoKHR();
            ret.sType = VkStructureType.BindImageMemoryInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkBindImageMemoryDeviceGroupInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint deviceIndexCount;
        public uint* pDeviceIndices;
        public uint SFRRectCount;
        public VkRect2D* pSFRRects;
        public static VkBindImageMemoryDeviceGroupInfoKHX New()
        {
            VkBindImageMemoryDeviceGroupInfoKHX ret = new VkBindImageMemoryDeviceGroupInfoKHX();
            ret.sType = VkStructureType.BindImageMemoryDeviceGroupInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGroupRenderPassBeginInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint deviceMask;
        public uint deviceRenderAreaCount;
        public VkRect2D* pDeviceRenderAreas;
        public static VkDeviceGroupRenderPassBeginInfoKHX New()
        {
            VkDeviceGroupRenderPassBeginInfoKHX ret = new VkDeviceGroupRenderPassBeginInfoKHX();
            ret.sType = VkStructureType.DeviceGroupRenderPassBeginInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGroupCommandBufferBeginInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint deviceMask;
        public static VkDeviceGroupCommandBufferBeginInfoKHX New()
        {
            VkDeviceGroupCommandBufferBeginInfoKHX ret = new VkDeviceGroupCommandBufferBeginInfoKHX();
            ret.sType = VkStructureType.DeviceGroupCommandBufferBeginInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGroupSubmitInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint waitSemaphoreCount;
        public uint* pWaitSemaphoreDeviceIndices;
        public uint commandBufferCount;
        public uint* pCommandBufferDeviceMasks;
        public uint signalSemaphoreCount;
        public uint* pSignalSemaphoreDeviceIndices;
        public static VkDeviceGroupSubmitInfoKHX New()
        {
            VkDeviceGroupSubmitInfoKHX ret = new VkDeviceGroupSubmitInfoKHX();
            ret.sType = VkStructureType.DeviceGroupSubmitInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGroupBindSparseInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint resourceDeviceIndex;
        public uint memoryDeviceIndex;
        public static VkDeviceGroupBindSparseInfoKHX New()
        {
            VkDeviceGroupBindSparseInfoKHX ret = new VkDeviceGroupBindSparseInfoKHX();
            ret.sType = VkStructureType.DeviceGroupBindSparseInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGroupPresentCapabilitiesKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public fixed uint presentMask[(int)VulkanNative.MaxDeviceGroupSizeKHX];
        public VkDeviceGroupPresentModeFlagsKHX modes;
        public static VkDeviceGroupPresentCapabilitiesKHX New()
        {
            VkDeviceGroupPresentCapabilitiesKHX ret = new VkDeviceGroupPresentCapabilitiesKHX();
            ret.sType = VkStructureType.DeviceGroupPresentCapabilitiesKHX;
            return ret;
        }
    }

    public unsafe partial struct VkImageSwapchainCreateInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSwapchainKHR swapchain;
        public static VkImageSwapchainCreateInfoKHX New()
        {
            VkImageSwapchainCreateInfoKHX ret = new VkImageSwapchainCreateInfoKHX();
            ret.sType = VkStructureType.ImageSwapchainCreateInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkBindImageMemorySwapchainInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSwapchainKHR swapchain;
        public uint imageIndex;
        public static VkBindImageMemorySwapchainInfoKHX New()
        {
            VkBindImageMemorySwapchainInfoKHX ret = new VkBindImageMemorySwapchainInfoKHX();
            ret.sType = VkStructureType.BindImageMemorySwapchainInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkAcquireNextImageInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSwapchainKHR swapchain;
        public ulong timeout;
        public VkSemaphore semaphore;
        public VkFence fence;
        public uint deviceMask;
        public static VkAcquireNextImageInfoKHX New()
        {
            VkAcquireNextImageInfoKHX ret = new VkAcquireNextImageInfoKHX();
            ret.sType = VkStructureType.AcquireNextImageInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGroupPresentInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint swapchainCount;
        public uint* pDeviceMasks;
        public VkDeviceGroupPresentModeFlagsKHX mode;
        public static VkDeviceGroupPresentInfoKHX New()
        {
            VkDeviceGroupPresentInfoKHX ret = new VkDeviceGroupPresentInfoKHX();
            ret.sType = VkStructureType.DeviceGroupPresentInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGroupDeviceCreateInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint physicalDeviceCount;
        public VkPhysicalDevice* pPhysicalDevices;
        public static VkDeviceGroupDeviceCreateInfoKHX New()
        {
            VkDeviceGroupDeviceCreateInfoKHX ret = new VkDeviceGroupDeviceCreateInfoKHX();
            ret.sType = VkStructureType.DeviceGroupDeviceCreateInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGroupSwapchainCreateInfoKHX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceGroupPresentModeFlagsKHX modes;
        public static VkDeviceGroupSwapchainCreateInfoKHX New()
        {
            VkDeviceGroupSwapchainCreateInfoKHX ret = new VkDeviceGroupSwapchainCreateInfoKHX();
            ret.sType = VkStructureType.DeviceGroupSwapchainCreateInfoKHX;
            return ret;
        }
    }

    public unsafe partial struct VkDescriptorUpdateTemplateEntryKHR
    {
        public uint dstBinding;
        public uint dstArrayElement;
        public uint descriptorCount;
        public VkDescriptorType descriptorType;
        public UIntPtr offset;
        public UIntPtr stride;
    }

    public unsafe partial struct VkDescriptorUpdateTemplateCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint descriptorUpdateEntryCount;
        public VkDescriptorUpdateTemplateEntryKHR* pDescriptorUpdateEntries;
        public VkDescriptorUpdateTemplateTypeKHR templateType;
        public VkDescriptorSetLayout descriptorSetLayout;
        public VkPipelineBindPoint pipelineBindPoint;
        public VkPipelineLayout pipelineLayout;
        public uint set;
        public static VkDescriptorUpdateTemplateCreateInfoKHR New()
        {
            VkDescriptorUpdateTemplateCreateInfoKHR ret = new VkDescriptorUpdateTemplateCreateInfoKHR();
            ret.sType = VkStructureType.DescriptorUpdateTemplateCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkXYColorEXT
    {
        public float x;
        public float y;
    }

    public unsafe partial struct VkHdrMetadataEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkXYColorEXT displayPrimaryRed;
        public VkXYColorEXT displayPrimaryGreen;
        public VkXYColorEXT displayPrimaryBlue;
        public VkXYColorEXT whitePoint;
        public float maxLuminance;
        public float minLuminance;
        public float maxContentLightLevel;
        public float maxFrameAverageLightLevel;
        public static VkHdrMetadataEXT New()
        {
            VkHdrMetadataEXT ret = new VkHdrMetadataEXT();
            ret.sType = VkStructureType.HdrMetadataEXT;
            return ret;
        }
    }

    public unsafe partial struct VkRefreshCycleDurationGOOGLE
    {
        public ulong refreshDuration;
    }

    public unsafe partial struct VkPastPresentationTimingGOOGLE
    {
        public uint presentID;
        public ulong desiredPresentTime;
        public ulong actualPresentTime;
        public ulong earliestPresentTime;
        public ulong presentMargin;
    }

    public unsafe partial struct VkPresentTimesInfoGOOGLE
    {
        public VkStructureType sType;
        public void* pNext;
        public uint swapchainCount;
        public VkPresentTimeGOOGLE* pTimes;
        public static VkPresentTimesInfoGOOGLE New()
        {
            VkPresentTimesInfoGOOGLE ret = new VkPresentTimesInfoGOOGLE();
            ret.sType = VkStructureType.PresentTimesInfoGoogle;
            return ret;
        }
    }

    public unsafe partial struct VkPresentTimeGOOGLE
    {
        public uint presentID;
        public ulong desiredPresentTime;
    }

    public unsafe partial struct VkIOSSurfaceCreateInfoMVK
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public void* pView;
        public static VkIOSSurfaceCreateInfoMVK New()
        {
            VkIOSSurfaceCreateInfoMVK ret = new VkIOSSurfaceCreateInfoMVK();
            ret.sType = VkStructureType.IosSurfaceCreateInfoMvk;
            return ret;
        }
    }

    public unsafe partial struct VkMacOSSurfaceCreateInfoMVK
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public void* pView;
        public static VkMacOSSurfaceCreateInfoMVK New()
        {
            VkMacOSSurfaceCreateInfoMVK ret = new VkMacOSSurfaceCreateInfoMVK();
            ret.sType = VkStructureType.MacosSurfaceCreateInfoMvk;
            return ret;
        }
    }

    public unsafe partial struct VkViewportWScalingNV
    {
        public float xcoeff;
        public float ycoeff;
    }

    public unsafe partial struct VkPipelineViewportWScalingStateCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 viewportWScalingEnable;
        public uint viewportCount;
        public VkViewportWScalingNV* pViewportWScalings;
        public static VkPipelineViewportWScalingStateCreateInfoNV New()
        {
            VkPipelineViewportWScalingStateCreateInfoNV ret = new VkPipelineViewportWScalingStateCreateInfoNV();
            ret.sType = VkStructureType.PipelineViewportWScalingStateCreateInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkViewportSwizzleNV
    {
        public VkViewportCoordinateSwizzleNV x;
        public VkViewportCoordinateSwizzleNV y;
        public VkViewportCoordinateSwizzleNV z;
        public VkViewportCoordinateSwizzleNV w;
    }

    public unsafe partial struct VkPipelineViewportSwizzleStateCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint viewportCount;
        public VkViewportSwizzleNV* pViewportSwizzles;
        public static VkPipelineViewportSwizzleStateCreateInfoNV New()
        {
            VkPipelineViewportSwizzleStateCreateInfoNV ret = new VkPipelineViewportSwizzleStateCreateInfoNV();
            ret.sType = VkStructureType.PipelineViewportSwizzleStateCreateInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceDiscardRectanglePropertiesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint maxDiscardRectangles;
        public static VkPhysicalDeviceDiscardRectanglePropertiesEXT New()
        {
            VkPhysicalDeviceDiscardRectanglePropertiesEXT ret = new VkPhysicalDeviceDiscardRectanglePropertiesEXT();
            ret.sType = VkStructureType.PhysicalDeviceDiscardRectanglePropertiesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineDiscardRectangleStateCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkDiscardRectangleModeEXT discardRectangleMode;
        public uint discardRectangleCount;
        public VkRect2D* pDiscardRectangles;
        public static VkPipelineDiscardRectangleStateCreateInfoEXT New()
        {
            VkPipelineDiscardRectangleStateCreateInfoEXT ret = new VkPipelineDiscardRectangleStateCreateInfoEXT();
            ret.sType = VkStructureType.PipelineDiscardRectangleStateCreateInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceMultiviewPerViewAttributesPropertiesNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 perViewPositionAllComponents;
        public static VkPhysicalDeviceMultiviewPerViewAttributesPropertiesNVX New()
        {
            VkPhysicalDeviceMultiviewPerViewAttributesPropertiesNVX ret = new VkPhysicalDeviceMultiviewPerViewAttributesPropertiesNVX();
            ret.sType = VkStructureType.PhysicalDeviceMultiviewPerViewAttributesPropertiesNVX;
            return ret;
        }
    }

    public unsafe partial struct VkInputAttachmentAspectReferenceKHR
    {
        public uint subpass;
        public uint inputAttachmentIndex;
        public VkImageAspectFlags aspectMask;
    }

    public unsafe partial struct VkRenderPassInputAttachmentAspectCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint aspectReferenceCount;
        public VkInputAttachmentAspectReferenceKHR* pAspectReferences;
        public static VkRenderPassInputAttachmentAspectCreateInfoKHR New()
        {
            VkRenderPassInputAttachmentAspectCreateInfoKHR ret = new VkRenderPassInputAttachmentAspectCreateInfoKHR();
            ret.sType = VkStructureType.RenderPassInputAttachmentAspectCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceSurfaceInfo2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSurfaceKHR surface;
        public static VkPhysicalDeviceSurfaceInfo2KHR New()
        {
            VkPhysicalDeviceSurfaceInfo2KHR ret = new VkPhysicalDeviceSurfaceInfo2KHR();
            ret.sType = VkStructureType.PhysicalDeviceSurfaceInfo2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkSurfaceCapabilities2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSurfaceCapabilitiesKHR surfaceCapabilities;
        public static VkSurfaceCapabilities2KHR New()
        {
            VkSurfaceCapabilities2KHR ret = new VkSurfaceCapabilities2KHR();
            ret.sType = VkStructureType.SurfaceCapabilities2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkSurfaceFormat2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSurfaceFormatKHR surfaceFormat;
        public static VkSurfaceFormat2KHR New()
        {
            VkSurfaceFormat2KHR ret = new VkSurfaceFormat2KHR();
            ret.sType = VkStructureType.SurfaceFormat2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkSharedPresentSurfaceCapabilitiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImageUsageFlags sharedPresentSupportedUsageFlags;
        public static VkSharedPresentSurfaceCapabilitiesKHR New()
        {
            VkSharedPresentSurfaceCapabilitiesKHR ret = new VkSharedPresentSurfaceCapabilitiesKHR();
            ret.sType = VkStructureType.SharedPresentSurfaceCapabilitiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDevice16BitStorageFeaturesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 storageBuffer16BitAccess;
        public VkBool32 uniformAndStorageBuffer16BitAccess;
        public VkBool32 storagePushConstant16;
        public VkBool32 storageInputOutput16;
        public static VkPhysicalDevice16BitStorageFeaturesKHR New()
        {
            VkPhysicalDevice16BitStorageFeaturesKHR ret = new VkPhysicalDevice16BitStorageFeaturesKHR();
            ret.sType = VkStructureType.PhysicalDevice16bitStorageFeaturesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkBufferMemoryRequirementsInfo2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBuffer buffer;
        public static VkBufferMemoryRequirementsInfo2KHR New()
        {
            VkBufferMemoryRequirementsInfo2KHR ret = new VkBufferMemoryRequirementsInfo2KHR();
            ret.sType = VkStructureType.BufferMemoryRequirementsInfo2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkImageMemoryRequirementsInfo2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImage image;
        public static VkImageMemoryRequirementsInfo2KHR New()
        {
            VkImageMemoryRequirementsInfo2KHR ret = new VkImageMemoryRequirementsInfo2KHR();
            ret.sType = VkStructureType.ImageMemoryRequirementsInfo2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkImageSparseMemoryRequirementsInfo2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImage image;
        public static VkImageSparseMemoryRequirementsInfo2KHR New()
        {
            VkImageSparseMemoryRequirementsInfo2KHR ret = new VkImageSparseMemoryRequirementsInfo2KHR();
            ret.sType = VkStructureType.ImageSparseMemoryRequirementsInfo2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryRequirements2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkMemoryRequirements memoryRequirements;
        public static VkMemoryRequirements2KHR New()
        {
            VkMemoryRequirements2KHR ret = new VkMemoryRequirements2KHR();
            ret.sType = VkStructureType.MemoryRequirements2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkSparseImageMemoryRequirements2KHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSparseImageMemoryRequirements memoryRequirements;
        public static VkSparseImageMemoryRequirements2KHR New()
        {
            VkSparseImageMemoryRequirements2KHR ret = new VkSparseImageMemoryRequirements2KHR();
            ret.sType = VkStructureType.SparseImageMemoryRequirements2KHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDevicePointClippingPropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPointClippingBehaviorKHR pointClippingBehavior;
        public static VkPhysicalDevicePointClippingPropertiesKHR New()
        {
            VkPhysicalDevicePointClippingPropertiesKHR ret = new VkPhysicalDevicePointClippingPropertiesKHR();
            ret.sType = VkStructureType.PhysicalDevicePointClippingPropertiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryDedicatedRequirementsKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 prefersDedicatedAllocation;
        public VkBool32 requiresDedicatedAllocation;
        public static VkMemoryDedicatedRequirementsKHR New()
        {
            VkMemoryDedicatedRequirementsKHR ret = new VkMemoryDedicatedRequirementsKHR();
            ret.sType = VkStructureType.MemoryDedicatedRequirementsKHR;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryDedicatedAllocateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImage image;
        public VkBuffer buffer;
        public static VkMemoryDedicatedAllocateInfoKHR New()
        {
            VkMemoryDedicatedAllocateInfoKHR ret = new VkMemoryDedicatedAllocateInfoKHR();
            ret.sType = VkStructureType.MemoryDedicatedAllocateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkImageViewUsageCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImageUsageFlags usage;
        public static VkImageViewUsageCreateInfoKHR New()
        {
            VkImageViewUsageCreateInfoKHR ret = new VkImageViewUsageCreateInfoKHR();
            ret.sType = VkStructureType.ImageViewUsageCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineTessellationDomainOriginStateCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkTessellationDomainOriginKHR domainOrigin;
        public static VkPipelineTessellationDomainOriginStateCreateInfoKHR New()
        {
            VkPipelineTessellationDomainOriginStateCreateInfoKHR ret = new VkPipelineTessellationDomainOriginStateCreateInfoKHR();
            ret.sType = VkStructureType.PipelineTessellationDomainOriginStateCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkSamplerYcbcrConversionInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSamplerYcbcrConversionKHR conversion;
        public static VkSamplerYcbcrConversionInfoKHR New()
        {
            VkSamplerYcbcrConversionInfoKHR ret = new VkSamplerYcbcrConversionInfoKHR();
            ret.sType = VkStructureType.SamplerYcbcrConversionInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkSamplerYcbcrConversionCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFormat format;
        public VkSamplerYcbcrModelConversionKHR ycbcrModel;
        public VkSamplerYcbcrRangeKHR ycbcrRange;
        public VkComponentMapping components;
        public VkChromaLocationKHR xChromaOffset;
        public VkChromaLocationKHR yChromaOffset;
        public VkFilter chromaFilter;
        public VkBool32 forceExplicitReconstruction;
        public static VkSamplerYcbcrConversionCreateInfoKHR New()
        {
            VkSamplerYcbcrConversionCreateInfoKHR ret = new VkSamplerYcbcrConversionCreateInfoKHR();
            ret.sType = VkStructureType.SamplerYcbcrConversionCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkBindImagePlaneMemoryInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImageAspectFlags planeAspect;
        public static VkBindImagePlaneMemoryInfoKHR New()
        {
            VkBindImagePlaneMemoryInfoKHR ret = new VkBindImagePlaneMemoryInfoKHR();
            ret.sType = VkStructureType.BindImagePlaneMemoryInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkImagePlaneMemoryRequirementsInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImageAspectFlags planeAspect;
        public static VkImagePlaneMemoryRequirementsInfoKHR New()
        {
            VkImagePlaneMemoryRequirementsInfoKHR ret = new VkImagePlaneMemoryRequirementsInfoKHR();
            ret.sType = VkStructureType.ImagePlaneMemoryRequirementsInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceSamplerYcbcrConversionFeaturesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 samplerYcbcrConversion;
        public static VkPhysicalDeviceSamplerYcbcrConversionFeaturesKHR New()
        {
            VkPhysicalDeviceSamplerYcbcrConversionFeaturesKHR ret = new VkPhysicalDeviceSamplerYcbcrConversionFeaturesKHR();
            ret.sType = VkStructureType.PhysicalDeviceSamplerYcbcrConversionFeaturesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkSamplerYcbcrConversionImageFormatPropertiesKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint combinedImageSamplerDescriptorCount;
        public static VkSamplerYcbcrConversionImageFormatPropertiesKHR New()
        {
            VkSamplerYcbcrConversionImageFormatPropertiesKHR ret = new VkSamplerYcbcrConversionImageFormatPropertiesKHR();
            ret.sType = VkStructureType.SamplerYcbcrConversionImageFormatPropertiesKHR;
            return ret;
        }
    }

    public unsafe partial struct VkTextureLODGatherFormatPropertiesAMD
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 supportsTextureGatherLODBiasAMD;
        public static VkTextureLODGatherFormatPropertiesAMD New()
        {
            VkTextureLODGatherFormatPropertiesAMD ret = new VkTextureLODGatherFormatPropertiesAMD();
            ret.sType = VkStructureType.TextureLodGatherFormatPropertiesAMD;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineCoverageToColorStateCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkBool32 coverageToColorEnable;
        public uint coverageToColorLocation;
        public static VkPipelineCoverageToColorStateCreateInfoNV New()
        {
            VkPipelineCoverageToColorStateCreateInfoNV ret = new VkPipelineCoverageToColorStateCreateInfoNV();
            ret.sType = VkStructureType.PipelineCoverageToColorStateCreateInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceSamplerFilterMinmaxPropertiesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 filterMinmaxSingleComponentFormats;
        public VkBool32 filterMinmaxImageComponentMapping;
        public static VkPhysicalDeviceSamplerFilterMinmaxPropertiesEXT New()
        {
            VkPhysicalDeviceSamplerFilterMinmaxPropertiesEXT ret = new VkPhysicalDeviceSamplerFilterMinmaxPropertiesEXT();
            ret.sType = VkStructureType.PhysicalDeviceSamplerFilterMinmaxPropertiesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkSampleLocationEXT
    {
        public float x;
        public float y;
    }

    public unsafe partial struct VkSampleLocationsInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSampleCountFlags sampleLocationsPerPixel;
        public VkExtent2D sampleLocationGridSize;
        public uint sampleLocationsCount;
        public VkSampleLocationEXT* pSampleLocations;
        public static VkSampleLocationsInfoEXT New()
        {
            VkSampleLocationsInfoEXT ret = new VkSampleLocationsInfoEXT();
            ret.sType = VkStructureType.SampleLocationsInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkAttachmentSampleLocationsEXT
    {
        public uint attachmentIndex;
        public VkSampleLocationsInfoEXT sampleLocationsInfo;
    }

    public unsafe partial struct VkSubpassSampleLocationsEXT
    {
        public uint subpassIndex;
        public VkSampleLocationsInfoEXT sampleLocationsInfo;
    }

    public unsafe partial struct VkRenderPassSampleLocationsBeginInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint attachmentInitialSampleLocationsCount;
        public VkAttachmentSampleLocationsEXT* pAttachmentInitialSampleLocations;
        public uint postSubpassSampleLocationsCount;
        public VkSubpassSampleLocationsEXT* pPostSubpassSampleLocations;
        public static VkRenderPassSampleLocationsBeginInfoEXT New()
        {
            VkRenderPassSampleLocationsBeginInfoEXT ret = new VkRenderPassSampleLocationsBeginInfoEXT();
            ret.sType = VkStructureType.RenderPassSampleLocationsBeginInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineSampleLocationsStateCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 sampleLocationsEnable;
        public VkSampleLocationsInfoEXT sampleLocationsInfo;
        public static VkPipelineSampleLocationsStateCreateInfoEXT New()
        {
            VkPipelineSampleLocationsStateCreateInfoEXT ret = new VkPipelineSampleLocationsStateCreateInfoEXT();
            ret.sType = VkStructureType.PipelineSampleLocationsStateCreateInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceSampleLocationsPropertiesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSampleCountFlags sampleLocationSampleCounts;
        public VkExtent2D maxSampleLocationGridSize;
        public float sampleLocationCoordinateRange_0;
        public float sampleLocationCoordinateRange_1;
        public uint sampleLocationSubPixelBits;
        public VkBool32 variableSampleLocations;
        public static VkPhysicalDeviceSampleLocationsPropertiesEXT New()
        {
            VkPhysicalDeviceSampleLocationsPropertiesEXT ret = new VkPhysicalDeviceSampleLocationsPropertiesEXT();
            ret.sType = VkStructureType.PhysicalDeviceSampleLocationsPropertiesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkMultisamplePropertiesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExtent2D maxSampleLocationGridSize;
        public static VkMultisamplePropertiesEXT New()
        {
            VkMultisamplePropertiesEXT ret = new VkMultisamplePropertiesEXT();
            ret.sType = VkStructureType.MultisamplePropertiesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkSamplerReductionModeCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSamplerReductionModeEXT reductionMode;
        public static VkSamplerReductionModeCreateInfoEXT New()
        {
            VkSamplerReductionModeCreateInfoEXT ret = new VkSamplerReductionModeCreateInfoEXT();
            ret.sType = VkStructureType.SamplerReductionModeCreateInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceBlendOperationAdvancedFeaturesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 advancedBlendCoherentOperations;
        public static VkPhysicalDeviceBlendOperationAdvancedFeaturesEXT New()
        {
            VkPhysicalDeviceBlendOperationAdvancedFeaturesEXT ret = new VkPhysicalDeviceBlendOperationAdvancedFeaturesEXT();
            ret.sType = VkStructureType.PhysicalDeviceBlendOperationAdvancedFeaturesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceBlendOperationAdvancedPropertiesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint advancedBlendMaxColorAttachments;
        public VkBool32 advancedBlendIndependentBlend;
        public VkBool32 advancedBlendNonPremultipliedSrcColor;
        public VkBool32 advancedBlendNonPremultipliedDstColor;
        public VkBool32 advancedBlendCorrelatedOverlap;
        public VkBool32 advancedBlendAllOperations;
        public static VkPhysicalDeviceBlendOperationAdvancedPropertiesEXT New()
        {
            VkPhysicalDeviceBlendOperationAdvancedPropertiesEXT ret = new VkPhysicalDeviceBlendOperationAdvancedPropertiesEXT();
            ret.sType = VkStructureType.PhysicalDeviceBlendOperationAdvancedPropertiesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineColorBlendAdvancedStateCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 srcPremultiplied;
        public VkBool32 dstPremultiplied;
        public VkBlendOverlapEXT blendOverlap;
        public static VkPipelineColorBlendAdvancedStateCreateInfoEXT New()
        {
            VkPipelineColorBlendAdvancedStateCreateInfoEXT ret = new VkPipelineColorBlendAdvancedStateCreateInfoEXT();
            ret.sType = VkStructureType.PipelineColorBlendAdvancedStateCreateInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineCoverageModulationStateCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkCoverageModulationModeNV coverageModulationMode;
        public VkBool32 coverageModulationTableEnable;
        public uint coverageModulationTableCount;
        public float* pCoverageModulationTable;
        public static VkPipelineCoverageModulationStateCreateInfoNV New()
        {
            VkPipelineCoverageModulationStateCreateInfoNV ret = new VkPipelineCoverageModulationStateCreateInfoNV();
            ret.sType = VkStructureType.PipelineCoverageModulationStateCreateInfoNV;
            return ret;
        }
    }

    public unsafe partial struct VkImageFormatListCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint viewFormatCount;
        public VkFormat* pViewFormats;
        public static VkImageFormatListCreateInfoKHR New()
        {
            VkImageFormatListCreateInfoKHR ret = new VkImageFormatListCreateInfoKHR();
            ret.sType = VkStructureType.ImageFormatListCreateInfoKHR;
            return ret;
        }
    }

    public unsafe partial struct VkValidationCacheCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public UIntPtr initialDataSize;
        public void* pInitialData;
        public static VkValidationCacheCreateInfoEXT New()
        {
            VkValidationCacheCreateInfoEXT ret = new VkValidationCacheCreateInfoEXT();
            ret.sType = VkStructureType.ValidationCacheCreateInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkShaderModuleValidationCacheCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkValidationCacheEXT validationCache;
        public static VkShaderModuleValidationCacheCreateInfoEXT New()
        {
            VkShaderModuleValidationCacheCreateInfoEXT ret = new VkShaderModuleValidationCacheCreateInfoEXT();
            ret.sType = VkStructureType.ShaderModuleValidationCacheCreateInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceDescriptorIndexingFeaturesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 shaderInputAttachmentArrayDynamicIndexing;
        public VkBool32 shaderUniformTexelBufferArrayDynamicIndexing;
        public VkBool32 shaderStorageTexelBufferArrayDynamicIndexing;
        public VkBool32 shaderUniformBufferArrayNonUniformIndexing;
        public VkBool32 shaderSampledImageArrayNonUniformIndexing;
        public VkBool32 shaderStorageBufferArrayNonUniformIndexing;
        public VkBool32 shaderStorageImageArrayNonUniformIndexing;
        public VkBool32 shaderInputAttachmentArrayNonUniformIndexing;
        public VkBool32 shaderUniformTexelBufferArrayNonUniformIndexing;
        public VkBool32 shaderStorageTexelBufferArrayNonUniformIndexing;
        public VkBool32 descriptorBindingUniformBufferUpdateAfterBind;
        public VkBool32 descriptorBindingSampledImageUpdateAfterBind;
        public VkBool32 descriptorBindingStorageImageUpdateAfterBind;
        public VkBool32 descriptorBindingStorageBufferUpdateAfterBind;
        public VkBool32 descriptorBindingUniformTexelBufferUpdateAfterBind;
        public VkBool32 descriptorBindingStorageTexelBufferUpdateAfterBind;
        public VkBool32 descriptorBindingUpdateUnusedWhilePending;
        public VkBool32 descriptorBindingPartiallyBound;
        public VkBool32 descriptorBindingVariableDescriptorCount;
        public VkBool32 runtimeDescriptorArray;

        public static VkPhysicalDeviceDescriptorIndexingFeaturesEXT New()
        {
            VkPhysicalDeviceDescriptorIndexingFeaturesEXT ret = new VkPhysicalDeviceDescriptorIndexingFeaturesEXT();
            ret.sType = VkStructureType.PhysicalDeviceDescriptorIndexingFeaturesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceDescriptorIndexingPropertiesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint maxUpdateAfterBindDescriptorsInAllPools;
        public VkBool32 shaderUniformBufferArrayNonUniformIndexingNative;
        public VkBool32 shaderSampledImageArrayNonUniformIndexingNative;
        public VkBool32 shaderStorageBufferArrayNonUniformIndexingNative;
        public VkBool32 shaderStorageImageArrayNonUniformIndexingNative;
        public VkBool32 shaderInputAttachmentArrayNonUniformIndexingNative;
        public VkBool32 robustBufferAccessUpdateAfterBind;
        public VkBool32 quadDivergentImplicitLod;
        public uint maxPerStageDescriptorUpdateAfterBindSamplers;
        public uint maxPerStageDescriptorUpdateAfterBindUniformBuffers;
        public uint maxPerStageDescriptorUpdateAfterBindStorageBuffers;
        public uint maxPerStageDescriptorUpdateAfterBindSampledImages;
        public uint maxPerStageDescriptorUpdateAfterBindStorageImages;
        public uint maxPerStageDescriptorUpdateAfterBindInputAttachments;
        public uint maxPerStageUpdateAfterBindResources;
        public uint maxDescriptorSetUpdateAfterBindSamplers;
        public uint maxDescriptorSetUpdateAfterBindUniformBuffers;
        public uint maxDescriptorSetUpdateAfterBindUniformBuffersDynamic;
        public uint maxDescriptorSetUpdateAfterBindStorageBuffers;
        public uint maxDescriptorSetUpdateAfterBindStorageBuffersDynamic;
        public uint maxDescriptorSetUpdateAfterBindSampledImages;
        public uint maxDescriptorSetUpdateAfterBindStorageImages;
        public uint maxDescriptorSetUpdateAfterBindInputAttachments;

        public static VkPhysicalDeviceDescriptorIndexingPropertiesEXT New()
        {
            VkPhysicalDeviceDescriptorIndexingPropertiesEXT ret = new VkPhysicalDeviceDescriptorIndexingPropertiesEXT();
            ret.sType = VkStructureType.PhysicalDeviceDescriptorIndexingPropertiesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkDescriptorSetVariableDescriptorCountAllocateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint descriptorSetCount;
        public uint* pDescriptorCounts;

        public static VkDescriptorSetVariableDescriptorCountAllocateInfoEXT New()
        {
            VkDescriptorSetVariableDescriptorCountAllocateInfoEXT ret = new VkDescriptorSetVariableDescriptorCountAllocateInfoEXT();
            ret.sType = VkStructureType.DescriptorSetVariableDescriptorCountAllocateInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkDescriptorSetVariableDescriptorCountLayoutSupportEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint maxVariableDescriptorCount;

        public static VkDescriptorSetVariableDescriptorCountLayoutSupportEXT New()
        {
            VkDescriptorSetVariableDescriptorCountLayoutSupportEXT ret = new VkDescriptorSetVariableDescriptorCountLayoutSupportEXT();
            ret.sType = VkStructureType.DescriptorSetVariableDescriptorCountLayoutSupportEXT;
            return ret;
        }
    }

    public unsafe partial struct VkNativeBufferANDROID
    {
        public VkStructureType sType;
        public void* pNext;
        public void* handle;
        public int stride;
        public int format;
        public int usage;
        public static VkNativeBufferANDROID New()
        {
            VkNativeBufferANDROID ret = new VkNativeBufferANDROID();
            ret.sType = VkStructureType.NativeBufferAndroid;
            return ret;
        }
    }

    public unsafe partial struct VkShaderResourceUsageAMD
    {
        public uint numUsedVgprs;
        public uint numUsedSgprs;
        public uint ldsSizePerLocalWorkGroup;
        public UIntPtr ldsUsageSizeInBytes;
        public UIntPtr scratchMemUsageInBytes;
    }

    public unsafe partial struct VkShaderStatisticsInfoAMD
    {
        public VkShaderStageFlags shaderStageMask;
        public VkShaderResourceUsageAMD resourceUsage;
        public uint numPhysicalVgprs;
        public uint numPhysicalSgprs;
        public uint numAvailableVgprs;
        public uint numAvailableSgprs;
        public uint computeWorkGroupSize_0;
        public uint computeWorkGroupSize_1;
        public uint computeWorkGroupSize_2;
    }

    public unsafe partial struct VkDeviceQueueGlobalPriorityCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkQueueGlobalPriorityEXT globalPriority;
        public static VkDeviceQueueGlobalPriorityCreateInfoEXT New()
        {
            VkDeviceQueueGlobalPriorityCreateInfoEXT ret = new VkDeviceQueueGlobalPriorityCreateInfoEXT();
            ret.sType = VkStructureType.DeviceQueueGlobalPriorityCreateInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkImportMemoryHostPointerInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsKHR handleType;
        public void* pHostPointer;
        public static VkImportMemoryHostPointerInfoEXT New()
        {
            VkImportMemoryHostPointerInfoEXT ret = new VkImportMemoryHostPointerInfoEXT();
            ret.sType = VkStructureType.ImportMemoryHostPointerInfoEXT;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryHostPointerPropertiesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint memoryTypeBits;
        public static VkMemoryHostPointerPropertiesEXT New()
        {
            VkMemoryHostPointerPropertiesEXT ret = new VkMemoryHostPointerPropertiesEXT();
            ret.sType = VkStructureType.MemoryHostPointerPropertiesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceExternalMemoryHostPropertiesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public ulong minImportedHostPointerAlignment;
        public static VkPhysicalDeviceExternalMemoryHostPropertiesEXT New()
        {
            VkPhysicalDeviceExternalMemoryHostPropertiesEXT ret = new VkPhysicalDeviceExternalMemoryHostPropertiesEXT();
            ret.sType = VkStructureType.PhysicalDeviceExternalMemoryHostPropertiesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceConservativeRasterizationPropertiesEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public float primitiveOverestimationSize;
        public float maxExtraPrimitiveOverestimationSize;
        public float extraPrimitiveOverestimationSizeGranularity;
        public VkBool32 primitiveUnderestimation;
        public VkBool32 conservativePointAndLineRasterization;
        public VkBool32 degenerateTrianglesRasterized;
        public VkBool32 degenerateLinesRasterized;
        public VkBool32 fullyCoveredFragmentShaderInputVariable;
        public VkBool32 conservativeRasterizationPostDepthCoverage;
        public static VkPhysicalDeviceConservativeRasterizationPropertiesEXT New()
        {
            VkPhysicalDeviceConservativeRasterizationPropertiesEXT ret = new VkPhysicalDeviceConservativeRasterizationPropertiesEXT();
            ret.sType = VkStructureType.PhysicalDeviceConservativeRasterizationPropertiesEXT;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineRasterizationConservativeStateCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkConservativeRasterizationModeEXT conservativeRasterizationMode;
        public float extraPrimitiveOverestimationSize;
        public static VkPipelineRasterizationConservativeStateCreateInfoEXT New()
        {
            VkPipelineRasterizationConservativeStateCreateInfoEXT ret = new VkPipelineRasterizationConservativeStateCreateInfoEXT();
            ret.sType = VkStructureType.PipelineRasterizationConservativeStateCreateInfoEXT;
            return ret;
        }
    }
}
