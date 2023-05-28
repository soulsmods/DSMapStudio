// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Vulkan;

public static unsafe class Vma
{
    private static readonly IntPtr s_NativeLibrary = LoadNativeLibrary();

    private static readonly delegate* unmanaged[Cdecl]<VmaAllocatorCreateInfo*, out VmaAllocator, VkResult> vmaCreateAllocator_ptr;
    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, void> vmaDestroyAllocator_ptr;
    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, VkBufferCreateInfo*, VmaAllocationCreateInfo*, out VkBuffer, out VmaAllocation, VmaAllocationInfo*, VkResult> vmaCreateBuffer_ptr;
    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, VkBuffer, VmaAllocation, void> vmaDestroyBuffer_ptr;
    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, VkImageCreateInfo*, VmaAllocationCreateInfo*, out VkImage, out VmaAllocation, VmaAllocationInfo*, VkResult> vmaCreateImage_ptr;
    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, VmaAllocation, VkImageCreateInfo*, out VkImage, VkResult> vmaCreateAliasingImage_ptr;
    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, VkImage, VmaAllocation, void> vmaDestroyImage_ptr;

    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, VmaAllocation, sbyte*, void> vmaSetAllocationName_ptr;

    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, VmaAllocation, void*, VkResult> vmaMapMemory_ptr;
    private static readonly delegate* unmanaged[Cdecl]<VmaAllocator, VmaAllocation, void> vmaUnmapMemory_ptr;
    
    static Vma()
    {
        vmaCreateAllocator_ptr = (delegate* unmanaged[Cdecl]<VmaAllocatorCreateInfo*, out VmaAllocator, VkResult>)LoadFunction(nameof(vmaCreateAllocator));
        vmaDestroyAllocator_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, void>)LoadFunction(nameof(vmaDestroyAllocator));
        vmaCreateBuffer_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, VkBufferCreateInfo*, VmaAllocationCreateInfo*, out VkBuffer, out VmaAllocation, VmaAllocationInfo*, VkResult>)LoadFunction(nameof(vmaCreateBuffer));
        vmaDestroyBuffer_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, VkBuffer, VmaAllocation, void>)LoadFunction(nameof(vmaDestroyBuffer));

        vmaCreateImage_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, VkImageCreateInfo*, VmaAllocationCreateInfo*, out VkImage, out VmaAllocation, VmaAllocationInfo*, VkResult>)LoadFunction(nameof(vmaCreateImage));
        vmaCreateAliasingImage_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, VmaAllocation, VkImageCreateInfo*, out VkImage, VkResult>)LoadFunction(nameof(vmaCreateAliasingImage));
        vmaDestroyImage_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, VkImage, VmaAllocation, void>)LoadFunction(nameof(vmaDestroyImage));

        vmaMapMemory_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, VmaAllocation, void*, VkResult>)LoadFunction(nameof(vmaMapMemory));
        vmaUnmapMemory_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, VmaAllocation, void>)LoadFunction(nameof(vmaUnmapMemory));
        
        vmaSetAllocationName_ptr = (delegate* unmanaged[Cdecl]<VmaAllocator, VmaAllocation, sbyte*, void>)LoadFunction(nameof(vmaSetAllocationName));
    }

    public static VkResult vmaCreateAllocator(VmaAllocatorCreateInfo* allocateInfo, out VmaAllocator allocator)
    {
        VmaVulkanFunctions functions = default;
        functions.vkGetInstanceProcAddr = vkGetInstanceProcAddr_ptr;
        functions.vkGetDeviceProcAddr = vkGetDeviceProcAddr_ptr;
        allocateInfo->pVulkanFunctions = &functions;
        return vmaCreateAllocator_ptr(allocateInfo, out allocator);
    }

    public static void vmaDestroyAllocator(VmaAllocator allocator)
    {
        vmaDestroyAllocator_ptr(allocator);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateBuffer_ptr(allocator, pBufferCreateInfo, &allocationInfo, out buffer, out allocation, null);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateBuffer_ptr(allocator, pBufferCreateInfo, &allocationInfo, out buffer, out allocation, pAllocationInfo);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation)
    {
        return vmaCreateBuffer_ptr(allocator, pBufferCreateInfo, pAllocationCreateInfo, out buffer, out allocation, null);
    }

    public static VkResult vmaCreateBuffer(
        VmaAllocator allocator,
        VkBufferCreateInfo* pBufferCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkBuffer buffer,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo)
    {
        return vmaCreateBuffer_ptr(allocator, pBufferCreateInfo, pAllocationCreateInfo, out buffer, out allocation, pAllocationInfo);
    }

    public static void vmaDestroyBuffer(VmaAllocator allocator, VkBuffer buffer, VmaAllocation allocation)
    {
        vmaDestroyBuffer_ptr(allocator, buffer, allocation);
    }

    public static VkResult vmaCreateImage(
        VmaAllocator allocator,
        VkImageCreateInfo* pImageCreateInfo,
        out VkImage image,
        out VmaAllocation allocation)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateImage_ptr(allocator, pImageCreateInfo, &allocationInfo, out image, out allocation, null);
    }

    public static VkResult vmaCreateImage(
        VmaAllocator allocator,
        VkImageCreateInfo* pImageCreateInfo,
        out VkImage image,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo)
    {
        VmaAllocationCreateInfo allocationInfo = new()
        {
            usage = VmaMemoryUsage.Auto
        };
        return vmaCreateImage_ptr(allocator, pImageCreateInfo, &allocationInfo, out image, out allocation, pAllocationInfo);
    }

    public static VkResult vmaCreateImage(
        VmaAllocator allocator,
        VkImageCreateInfo* pImageCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkImage image,
        out VmaAllocation allocation)
    {
        return vmaCreateImage_ptr(allocator, pImageCreateInfo, pAllocationCreateInfo, out image, out allocation, null);
    }

    public static VkResult vmaCreateImage(
        VmaAllocator allocator,
        VkImageCreateInfo* pImageCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo,
        out VkImage image,
        out VmaAllocation allocation,
        VmaAllocationInfo* pAllocationInfo)
    {
        return vmaCreateImage_ptr(allocator, pImageCreateInfo, pAllocationCreateInfo, out image, out allocation, pAllocationInfo);
    }

    public static VkResult vmaCreateAliasingImage(
        VmaAllocator allocator,
        VmaAllocation allocation,
        VkImageCreateInfo* pImageCreateInfo,
        out VkImage image)
    {
        return vmaCreateAliasingImage_ptr(allocator, allocation, pImageCreateInfo, out image);
    }

    public static void vmaDestroyImage(VmaAllocator allocator, VkImage image, VmaAllocation allocation)
    {
        vmaDestroyImage_ptr(allocator, image, allocation);
    }

    public static void vmaSetAllocationName(VmaAllocator allocator, VmaAllocation allocation, string name)
    {
        ReadOnlySpan<sbyte> data = Interop.GetUtf8Span(name);
        fixed (sbyte* dataPtr = data)
        {
            vmaSetAllocationName_ptr(allocator, allocation, dataPtr);
        }
    }

    public static VkResult vmaMapMemory(VmaAllocator allocator, VmaAllocation allocation, void* ppData)
    {
        return vmaMapMemory_ptr(allocator, allocation, ppData);
    }

    public static void vmaUnmapMemory(VmaAllocator allocator, VmaAllocation allocation)
    {
        vmaUnmapMemory_ptr(allocator, allocation);
    }

    private static IntPtr LoadNativeLibrary()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return LibraryLoader.LoadLocalLibrary("vma.dll");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return LibraryLoader.LoadLocalLibrary("libvma.so");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return LibraryLoader.LoadLocalLibrary("libvma.dylib");
        }

        throw new PlatformNotSupportedException("VMA is not supported");
    }

    private static IntPtr LoadFunction(string name) => LibraryLoader.GetSymbol(s_NativeLibrary, name);
}
