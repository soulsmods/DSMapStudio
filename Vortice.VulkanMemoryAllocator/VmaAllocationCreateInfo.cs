// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

public unsafe struct VmaAllocationCreateInfo
{
    public VmaAllocationCreateFlags flags;
    public VmaMemoryUsage usage;
    public VkMemoryPropertyFlags requiredFlags;
    public VkMemoryPropertyFlags preferredFlags;
    public uint memoryTypeBits;
    public VmaPool pool;
    public void* pUserData;
    public float priority;
}
