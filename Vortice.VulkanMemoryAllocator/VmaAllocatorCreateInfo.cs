// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

public unsafe struct VmaAllocatorCreateInfo
{
    /// <summary>
    /// Flags for created allocator.
    /// </summary>
    public VmaAllocatorCreateFlags flags;
    /// <summary>
    /// Vulkan physical device.
    /// </summary>
    /// <remarks>
    /// It must be valid throughout whole lifetime of created allocator.
    /// </remarks>
    public VkPhysicalDevice PhysicalDevice;
    /// <summary>
    /// Vulkan device.
    /// </summary>
    /// <remarks>
    /// It must be valid throughout whole lifetime of created allocator.
    /// </remarks>
    public VkDevice Device;

    /// Preferred size of a single `VkDeviceMemory` block to be allocated from large heaps > 1 GiB. Optional.
    /** Set to 0 to use default, which is currently 256 MiB. */
    public ulong preferredLargeHeapBlockSize;
    /// <summary>
    /// Optional custom CPU memory allocation callbacks. 
    /// </summary>
    public VkAllocationCallbacks* pAllocationCallbacks;
    /// <summary>
    /// Informative callbacks for `vkAllocateMemory`, `vkFreeMemory`. Optional.
    /// </summary>
    public /*VmaDeviceMemoryCallbacks*/void* pDeviceMemoryCallbacks;
    /** \brief Either null or a pointer to an array of limits on maximum number of bytes that can be allocated out of particular Vulkan memory heap.

    If not NULL, it must be a pointer to an array of
    `VkPhysicalDeviceMemoryProperties::memoryHeapCount` elements, defining limit on
    maximum number of bytes that can be allocated out of particular Vulkan memory
    heap.

    Any of the elements may be equal to `VK_WHOLE_SIZE`, which means no limit on that
    heap. This is also the default in case of `pHeapSizeLimit` = NULL.

    If there is a limit defined for a heap:

    - If user tries to allocate more memory from that heap using this allocator,
      the allocation fails with `VK_ERROR_OUT_OF_DEVICE_MEMORY`.
    - If the limit is smaller than heap size reported in `VkMemoryHeap::size`, the
      value of this limit will be reported instead when using vmaGetMemoryProperties().

    Warning! Using this feature may not be equivalent to installing a GPU with
    smaller amount of memory, because graphics driver doesn't necessary fail new
    allocations with `VK_ERROR_OUT_OF_DEVICE_MEMORY` result when memory capacity is
    exceeded. It may return success and just silently migrate some device memory
    blocks to system RAM. This driver behavior can also be controlled using
    VK_AMD_memory_overallocation_behavior extension.
    */
    public ulong* pHeapSizeLimit;
    /// <summary>
    /// Pointers to Vulkan functions. Can be null.
    /// </summary>
    internal VmaVulkanFunctions* pVulkanFunctions;
    /// <summary>
    /// Handle to Vulkan instance object.
    /// </summary>
    public VkInstance Instance;
    /** \brief Optional. The highest version of Vulkan that the application is designed to use.

    It must be a value in the format as created by macro `VK_MAKE_VERSION` or a constant like: `VK_API_VERSION_1_1`, `VK_API_VERSION_1_0`.
    The patch version number specified is ignored. Only the major and minor versions are considered.
    It must be less or equal (preferably equal) to value as passed to `vkCreateInstance` as `VkApplicationInfo::apiVersion`.
    Only versions 1.0, 1.1, 1.2, 1.3 are supported by the current implementation.
    Leaving it initialized to zero is equivalent to `VK_API_VERSION_1_0`.
    */
    public VkVersion VulkanApiVersion;
    /** \brief Either null or a pointer to an array of external memory handle types for each Vulkan memory type.

    If not NULL, it must be a pointer to an array of `VkPhysicalDeviceMemoryProperties::memoryTypeCount`
    elements, defining external memory handle types of particular Vulkan memory type,
    to be passed using `VkExportMemoryAllocateInfoKHR`.

    Any of the elements may be equal to 0, which means not to use `VkExportMemoryAllocateInfoKHR` on this memory type.
    This is also the default in case of `pTypeExternalMemoryHandleTypes` = NULL.
    */
    public VkExternalMemoryHandleTypeFlagsKHR* pTypeExternalMemoryHandleTypes;
}
