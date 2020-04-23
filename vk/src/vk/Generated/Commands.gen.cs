// This file is generated.

using System;
using System.Runtime.InteropServices;

namespace Vulkan
{
    public static unsafe partial class VulkanNative
    {
        private static IntPtr vkAcquireImageANDROID_ptr;
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireImageANDROID(VkDevice device, VkImage image, int nativeFenceFd, VkSemaphore semaphore, VkFence fence)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkAcquireNextImage2KHX_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImage2KHX(VkDevice device, VkAcquireNextImageInfoKHX* pAcquireInfo, uint* pImageIndex)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImage2KHX(VkDevice device, VkAcquireNextImageInfoKHX* pAcquireInfo, ref uint pImageIndex)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImage2KHX(VkDevice device, VkAcquireNextImageInfoKHX* pAcquireInfo, IntPtr pImageIndex)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImage2KHX(VkDevice device, ref VkAcquireNextImageInfoKHX pAcquireInfo, uint* pImageIndex)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImage2KHX(VkDevice device, ref VkAcquireNextImageInfoKHX pAcquireInfo, ref uint pImageIndex)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImage2KHX(VkDevice device, ref VkAcquireNextImageInfoKHX pAcquireInfo, IntPtr pImageIndex)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImage2KHX(VkDevice device, IntPtr pAcquireInfo, uint* pImageIndex)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImage2KHX(VkDevice device, IntPtr pAcquireInfo, ref uint pImageIndex)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImage2KHX(VkDevice device, IntPtr pAcquireInfo, IntPtr pImageIndex)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkAcquireNextImageKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImageKHR(VkDevice device, VkSwapchainKHR swapchain, ulong timeout, VkSemaphore semaphore, VkFence fence, uint* pImageIndex)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImageKHR(VkDevice device, VkSwapchainKHR swapchain, ulong timeout, VkSemaphore semaphore, VkFence fence, ref uint pImageIndex)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT, VK_NOT_READY, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireNextImageKHR(VkDevice device, VkSwapchainKHR swapchain, ulong timeout, VkSemaphore semaphore, VkFence fence, IntPtr pImageIndex)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkAcquireXlibDisplayEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireXlibDisplayEXT(VkPhysicalDevice physicalDevice, Xlib.Display* dpy, VkDisplayKHR display)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireXlibDisplayEXT(VkPhysicalDevice physicalDevice, ref Xlib.Display dpy, VkDisplayKHR display)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAcquireXlibDisplayEXT(VkPhysicalDevice physicalDevice, IntPtr dpy, VkDisplayKHR display)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkAllocateCommandBuffers_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateCommandBuffers(VkDevice device, VkCommandBufferAllocateInfo* pAllocateInfo, VkCommandBuffer* pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateCommandBuffers(VkDevice device, VkCommandBufferAllocateInfo* pAllocateInfo, out VkCommandBuffer pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateCommandBuffers(VkDevice device, ref VkCommandBufferAllocateInfo pAllocateInfo, VkCommandBuffer* pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateCommandBuffers(VkDevice device, ref VkCommandBufferAllocateInfo pAllocateInfo, out VkCommandBuffer pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateCommandBuffers(VkDevice device, IntPtr pAllocateInfo, VkCommandBuffer* pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateCommandBuffers(VkDevice device, IntPtr pAllocateInfo, out VkCommandBuffer pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkAllocateDescriptorSets_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FRAGMENTED_POOL, VK_ERROR_OUT_OF_POOL_MEMORY_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateDescriptorSets(VkDevice device, VkDescriptorSetAllocateInfo* pAllocateInfo, VkDescriptorSet* pDescriptorSets)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FRAGMENTED_POOL, VK_ERROR_OUT_OF_POOL_MEMORY_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateDescriptorSets(VkDevice device, VkDescriptorSetAllocateInfo* pAllocateInfo, out VkDescriptorSet pDescriptorSets)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FRAGMENTED_POOL, VK_ERROR_OUT_OF_POOL_MEMORY_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateDescriptorSets(VkDevice device, ref VkDescriptorSetAllocateInfo pAllocateInfo, VkDescriptorSet* pDescriptorSets)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FRAGMENTED_POOL, VK_ERROR_OUT_OF_POOL_MEMORY_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateDescriptorSets(VkDevice device, ref VkDescriptorSetAllocateInfo pAllocateInfo, out VkDescriptorSet pDescriptorSets)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FRAGMENTED_POOL, VK_ERROR_OUT_OF_POOL_MEMORY_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateDescriptorSets(VkDevice device, IntPtr pAllocateInfo, VkDescriptorSet* pDescriptorSets)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FRAGMENTED_POOL, VK_ERROR_OUT_OF_POOL_MEMORY_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateDescriptorSets(VkDevice device, IntPtr pAllocateInfo, out VkDescriptorSet pDescriptorSets)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkAllocateMemory_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, VkMemoryAllocateInfo* pAllocateInfo, VkAllocationCallbacks* pAllocator, VkDeviceMemory* pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, VkMemoryAllocateInfo* pAllocateInfo, VkAllocationCallbacks* pAllocator, out VkDeviceMemory pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, VkMemoryAllocateInfo* pAllocateInfo, ref VkAllocationCallbacks pAllocator, VkDeviceMemory* pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, VkMemoryAllocateInfo* pAllocateInfo, ref VkAllocationCallbacks pAllocator, out VkDeviceMemory pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, VkMemoryAllocateInfo* pAllocateInfo, IntPtr pAllocator, VkDeviceMemory* pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, VkMemoryAllocateInfo* pAllocateInfo, IntPtr pAllocator, out VkDeviceMemory pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, ref VkMemoryAllocateInfo pAllocateInfo, VkAllocationCallbacks* pAllocator, VkDeviceMemory* pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, ref VkMemoryAllocateInfo pAllocateInfo, VkAllocationCallbacks* pAllocator, out VkDeviceMemory pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, ref VkMemoryAllocateInfo pAllocateInfo, ref VkAllocationCallbacks pAllocator, VkDeviceMemory* pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, ref VkMemoryAllocateInfo pAllocateInfo, ref VkAllocationCallbacks pAllocator, out VkDeviceMemory pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, ref VkMemoryAllocateInfo pAllocateInfo, IntPtr pAllocator, VkDeviceMemory* pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, ref VkMemoryAllocateInfo pAllocateInfo, IntPtr pAllocator, out VkDeviceMemory pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, IntPtr pAllocateInfo, VkAllocationCallbacks* pAllocator, VkDeviceMemory* pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, IntPtr pAllocateInfo, VkAllocationCallbacks* pAllocator, out VkDeviceMemory pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, IntPtr pAllocateInfo, ref VkAllocationCallbacks pAllocator, VkDeviceMemory* pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, IntPtr pAllocateInfo, ref VkAllocationCallbacks pAllocator, out VkDeviceMemory pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, IntPtr pAllocateInfo, IntPtr pAllocator, VkDeviceMemory* pMemory)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkAllocateMemory(VkDevice device, IntPtr pAllocateInfo, IntPtr pAllocator, out VkDeviceMemory pMemory)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkBeginCommandBuffer_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBeginCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferBeginInfo* pBeginInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBeginCommandBuffer(VkCommandBuffer commandBuffer, ref VkCommandBufferBeginInfo pBeginInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBeginCommandBuffer(VkCommandBuffer commandBuffer, IntPtr pBeginInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkBindBufferMemory_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBindBufferMemory(VkDevice device, VkBuffer buffer, VkDeviceMemory memory, ulong memoryOffset)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkBindBufferMemory2KHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBindBufferMemory2KHR(VkDevice device, uint bindInfoCount, VkBindBufferMemoryInfoKHR* pBindInfos)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBindBufferMemory2KHR(VkDevice device, uint bindInfoCount, ref VkBindBufferMemoryInfoKHR pBindInfos)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBindBufferMemory2KHR(VkDevice device, uint bindInfoCount, IntPtr pBindInfos)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBindBufferMemory2KHR(VkDevice device, uint bindInfoCount, VkBindBufferMemoryInfoKHR[] pBindInfos)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkBindImageMemory_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBindImageMemory(VkDevice device, VkImage image, VkDeviceMemory memory, ulong memoryOffset)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkBindImageMemory2KHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBindImageMemory2KHR(VkDevice device, uint bindInfoCount, VkBindImageMemoryInfoKHR* pBindInfos)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBindImageMemory2KHR(VkDevice device, uint bindInfoCount, ref VkBindImageMemoryInfoKHR pBindInfos)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBindImageMemory2KHR(VkDevice device, uint bindInfoCount, IntPtr pBindInfos)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkBindImageMemory2KHR(VkDevice device, uint bindInfoCount, VkBindImageMemoryInfoKHR[] pBindInfos)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdBeginQuery_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdBeginQuery(VkCommandBuffer commandBuffer, VkQueryPool queryPool, uint query, VkQueryControlFlags flags)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdBeginRenderPass_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdBeginRenderPass(VkCommandBuffer commandBuffer, VkRenderPassBeginInfo* pRenderPassBegin, VkSubpassContents contents)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBeginRenderPass(VkCommandBuffer commandBuffer, ref VkRenderPassBeginInfo pRenderPassBegin, VkSubpassContents contents)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBeginRenderPass(VkCommandBuffer commandBuffer, IntPtr pRenderPassBegin, VkSubpassContents contents)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdBindDescriptorSets_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, VkDescriptorSet* pDescriptorSets, uint dynamicOffsetCount, uint* pDynamicOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, VkDescriptorSet* pDescriptorSets, uint dynamicOffsetCount, ref uint pDynamicOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, VkDescriptorSet* pDescriptorSets, uint dynamicOffsetCount, IntPtr pDynamicOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, ref VkDescriptorSet pDescriptorSets, uint dynamicOffsetCount, uint* pDynamicOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, ref VkDescriptorSet pDescriptorSets, uint dynamicOffsetCount, ref uint pDynamicOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, ref VkDescriptorSet pDescriptorSets, uint dynamicOffsetCount, IntPtr pDynamicOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, IntPtr pDescriptorSets, uint dynamicOffsetCount, uint* pDynamicOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, IntPtr pDescriptorSets, uint dynamicOffsetCount, ref uint pDynamicOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, IntPtr pDescriptorSets, uint dynamicOffsetCount, IntPtr pDynamicOffsets)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdBindIndexBuffer_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindIndexBuffer(VkCommandBuffer commandBuffer, VkBuffer buffer, ulong offset, VkIndexType indexType)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdBindPipeline_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindPipeline(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipeline pipeline)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdBindVertexBuffers_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, VkBuffer* pBuffers, ulong* pOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, VkBuffer* pBuffers, ref ulong pOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, VkBuffer* pBuffers, IntPtr pOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, ref VkBuffer pBuffers, ulong* pOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, ref VkBuffer pBuffers, ref ulong pOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, ref VkBuffer pBuffers, IntPtr pOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, IntPtr pBuffers, ulong* pOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, IntPtr pBuffers, ref ulong pOffsets)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, IntPtr pBuffers, IntPtr pOffsets)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdBlitImage_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdBlitImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, VkImageBlit* pRegions, VkFilter filter)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBlitImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, ref VkImageBlit pRegions, VkFilter filter)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdBlitImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, IntPtr pRegions, VkFilter filter)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdClearAttachments_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearAttachments(VkCommandBuffer commandBuffer, uint attachmentCount, VkClearAttachment* pAttachments, uint rectCount, VkClearRect* pRects)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearAttachments(VkCommandBuffer commandBuffer, uint attachmentCount, VkClearAttachment* pAttachments, uint rectCount, ref VkClearRect pRects)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearAttachments(VkCommandBuffer commandBuffer, uint attachmentCount, VkClearAttachment* pAttachments, uint rectCount, IntPtr pRects)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearAttachments(VkCommandBuffer commandBuffer, uint attachmentCount, ref VkClearAttachment pAttachments, uint rectCount, VkClearRect* pRects)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearAttachments(VkCommandBuffer commandBuffer, uint attachmentCount, ref VkClearAttachment pAttachments, uint rectCount, ref VkClearRect pRects)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearAttachments(VkCommandBuffer commandBuffer, uint attachmentCount, ref VkClearAttachment pAttachments, uint rectCount, IntPtr pRects)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearAttachments(VkCommandBuffer commandBuffer, uint attachmentCount, IntPtr pAttachments, uint rectCount, VkClearRect* pRects)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearAttachments(VkCommandBuffer commandBuffer, uint attachmentCount, IntPtr pAttachments, uint rectCount, ref VkClearRect pRects)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearAttachments(VkCommandBuffer commandBuffer, uint attachmentCount, IntPtr pAttachments, uint rectCount, IntPtr pRects)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdClearColorImage_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearColorValue* pColor, uint rangeCount, VkImageSubresourceRange* pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearColorValue* pColor, uint rangeCount, ref VkImageSubresourceRange pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearColorValue* pColor, uint rangeCount, IntPtr pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, ref VkClearColorValue pColor, uint rangeCount, VkImageSubresourceRange* pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, ref VkClearColorValue pColor, uint rangeCount, ref VkImageSubresourceRange pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, ref VkClearColorValue pColor, uint rangeCount, IntPtr pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, IntPtr pColor, uint rangeCount, VkImageSubresourceRange* pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, IntPtr pColor, uint rangeCount, ref VkImageSubresourceRange pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, IntPtr pColor, uint rangeCount, IntPtr pRanges)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdClearDepthStencilImage_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearDepthStencilImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearDepthStencilValue* pDepthStencil, uint rangeCount, VkImageSubresourceRange* pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearDepthStencilImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearDepthStencilValue* pDepthStencil, uint rangeCount, ref VkImageSubresourceRange pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearDepthStencilImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearDepthStencilValue* pDepthStencil, uint rangeCount, IntPtr pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearDepthStencilImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, ref VkClearDepthStencilValue pDepthStencil, uint rangeCount, VkImageSubresourceRange* pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearDepthStencilImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, ref VkClearDepthStencilValue pDepthStencil, uint rangeCount, ref VkImageSubresourceRange pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearDepthStencilImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, ref VkClearDepthStencilValue pDepthStencil, uint rangeCount, IntPtr pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearDepthStencilImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, IntPtr pDepthStencil, uint rangeCount, VkImageSubresourceRange* pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearDepthStencilImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, IntPtr pDepthStencil, uint rangeCount, ref VkImageSubresourceRange pRanges)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdClearDepthStencilImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, IntPtr pDepthStencil, uint rangeCount, IntPtr pRanges)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdCopyBuffer_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyBuffer(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkBuffer dstBuffer, uint regionCount, VkBufferCopy* pRegions)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyBuffer(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkBuffer dstBuffer, uint regionCount, ref VkBufferCopy pRegions)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyBuffer(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkBuffer dstBuffer, uint regionCount, IntPtr pRegions)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdCopyBufferToImage_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, VkBufferImageCopy* pRegions)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, ref VkBufferImageCopy pRegions)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, IntPtr pRegions)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdCopyImage_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, VkImageCopy* pRegions)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, ref VkImageCopy pRegions)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, IntPtr pRegions)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdCopyImageToBuffer_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyImageToBuffer(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkBuffer dstBuffer, uint regionCount, VkBufferImageCopy* pRegions)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyImageToBuffer(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkBuffer dstBuffer, uint regionCount, ref VkBufferImageCopy pRegions)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyImageToBuffer(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkBuffer dstBuffer, uint regionCount, IntPtr pRegions)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdCopyQueryPoolResults_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdCopyQueryPoolResults(VkCommandBuffer commandBuffer, VkQueryPool queryPool, uint firstQuery, uint queryCount, VkBuffer dstBuffer, ulong dstOffset, ulong stride, VkQueryResultFlags flags)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDebugMarkerBeginEXT_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDebugMarkerBeginEXT(VkCommandBuffer commandBuffer, VkDebugMarkerMarkerInfoEXT* pMarkerInfo)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdDebugMarkerBeginEXT(VkCommandBuffer commandBuffer, ref VkDebugMarkerMarkerInfoEXT pMarkerInfo)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdDebugMarkerBeginEXT(VkCommandBuffer commandBuffer, IntPtr pMarkerInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDebugMarkerEndEXT_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDebugMarkerEndEXT(VkCommandBuffer commandBuffer)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDebugMarkerInsertEXT_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDebugMarkerInsertEXT(VkCommandBuffer commandBuffer, VkDebugMarkerMarkerInfoEXT* pMarkerInfo)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdDebugMarkerInsertEXT(VkCommandBuffer commandBuffer, ref VkDebugMarkerMarkerInfoEXT pMarkerInfo)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdDebugMarkerInsertEXT(VkCommandBuffer commandBuffer, IntPtr pMarkerInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDispatch_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDispatch(VkCommandBuffer commandBuffer, uint groupCountX, uint groupCountY, uint groupCountZ)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDispatchBaseKHX_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDispatchBaseKHX(VkCommandBuffer commandBuffer, uint baseGroupX, uint baseGroupY, uint baseGroupZ, uint groupCountX, uint groupCountY, uint groupCountZ)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDispatchIndirect_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDispatchIndirect(VkCommandBuffer commandBuffer, VkBuffer buffer, ulong offset)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDraw_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDraw(VkCommandBuffer commandBuffer, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDrawIndexed_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDrawIndexed(VkCommandBuffer commandBuffer, uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDrawIndexedIndirect_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDrawIndexedIndirect(VkCommandBuffer commandBuffer, VkBuffer buffer, ulong offset, uint drawCount, uint stride)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDrawIndexedIndirectCountAMD_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDrawIndexedIndirectCountAMD(VkCommandBuffer commandBuffer, VkBuffer buffer, ulong offset, VkBuffer countBuffer, ulong countBufferOffset, uint maxDrawCount, uint stride)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDrawIndirect_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDrawIndirect(VkCommandBuffer commandBuffer, VkBuffer buffer, ulong offset, uint drawCount, uint stride)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdDrawIndirectCountAMD_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdDrawIndirectCountAMD(VkCommandBuffer commandBuffer, VkBuffer buffer, ulong offset, VkBuffer countBuffer, ulong countBufferOffset, uint maxDrawCount, uint stride)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdEndQuery_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdEndQuery(VkCommandBuffer commandBuffer, VkQueryPool queryPool, uint query)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdEndRenderPass_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdEndRenderPass(VkCommandBuffer commandBuffer)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdExecuteCommands_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, uint commandBufferCount, VkCommandBuffer* pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, uint commandBufferCount, ref VkCommandBuffer pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, uint commandBufferCount, IntPtr pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdFillBuffer_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdFillBuffer(VkCommandBuffer commandBuffer, VkBuffer dstBuffer, ulong dstOffset, ulong size, uint data)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdNextSubpass_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdNextSubpass(VkCommandBuffer commandBuffer, VkSubpassContents contents)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdPipelineBarrier_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdProcessCommandsNVX_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdProcessCommandsNVX(VkCommandBuffer commandBuffer, VkCmdProcessCommandsInfoNVX* pProcessCommandsInfo)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdProcessCommandsNVX(VkCommandBuffer commandBuffer, ref VkCmdProcessCommandsInfoNVX pProcessCommandsInfo)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdProcessCommandsNVX(VkCommandBuffer commandBuffer, IntPtr pProcessCommandsInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdPushConstants_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdPushConstants(VkCommandBuffer commandBuffer, VkPipelineLayout layout, VkShaderStageFlags stageFlags, uint offset, uint size, void* pValues)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdPushDescriptorSetKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdPushDescriptorSetKHR(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint set, uint descriptorWriteCount, VkWriteDescriptorSet* pDescriptorWrites)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPushDescriptorSetKHR(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint set, uint descriptorWriteCount, ref VkWriteDescriptorSet pDescriptorWrites)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdPushDescriptorSetKHR(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint set, uint descriptorWriteCount, IntPtr pDescriptorWrites)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdPushDescriptorSetWithTemplateKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdPushDescriptorSetWithTemplateKHR(VkCommandBuffer commandBuffer, VkDescriptorUpdateTemplateKHR descriptorUpdateTemplate, VkPipelineLayout layout, uint set, void* pData)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdReserveSpaceForCommandsNVX_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdReserveSpaceForCommandsNVX(VkCommandBuffer commandBuffer, VkCmdReserveSpaceForCommandsInfoNVX* pReserveSpaceInfo)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdReserveSpaceForCommandsNVX(VkCommandBuffer commandBuffer, ref VkCmdReserveSpaceForCommandsInfoNVX pReserveSpaceInfo)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdReserveSpaceForCommandsNVX(VkCommandBuffer commandBuffer, IntPtr pReserveSpaceInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdResetEvent_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdResetEvent(VkCommandBuffer commandBuffer, VkEvent @event, VkPipelineStageFlags stageMask)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdResetQueryPool_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdResetQueryPool(VkCommandBuffer commandBuffer, VkQueryPool queryPool, uint firstQuery, uint queryCount)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdResolveImage_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdResolveImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, VkImageResolve* pRegions)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdResolveImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, ref VkImageResolve pRegions)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdResolveImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, IntPtr pRegions)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetBlendConstants_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetBlendConstants(VkCommandBuffer commandBuffer, float blendConstants)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetDepthBias_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetDepthBias(VkCommandBuffer commandBuffer, float depthBiasConstantFactor, float depthBiasClamp, float depthBiasSlopeFactor)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetDepthBounds_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetDepthBounds(VkCommandBuffer commandBuffer, float minDepthBounds, float maxDepthBounds)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetDeviceMaskKHX_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetDeviceMaskKHX(VkCommandBuffer commandBuffer, uint deviceMask)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetDiscardRectangleEXT_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetDiscardRectangleEXT(VkCommandBuffer commandBuffer, uint firstDiscardRectangle, uint discardRectangleCount, VkRect2D* pDiscardRectangles)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetDiscardRectangleEXT(VkCommandBuffer commandBuffer, uint firstDiscardRectangle, uint discardRectangleCount, ref VkRect2D pDiscardRectangles)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetDiscardRectangleEXT(VkCommandBuffer commandBuffer, uint firstDiscardRectangle, uint discardRectangleCount, IntPtr pDiscardRectangles)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetEvent_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetEvent(VkCommandBuffer commandBuffer, VkEvent @event, VkPipelineStageFlags stageMask)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetLineWidth_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetLineWidth(VkCommandBuffer commandBuffer, float lineWidth)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetSampleLocationsEXT_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetSampleLocationsEXT(VkCommandBuffer commandBuffer, VkSampleLocationsInfoEXT* pSampleLocationsInfo)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetSampleLocationsEXT(VkCommandBuffer commandBuffer, ref VkSampleLocationsInfoEXT pSampleLocationsInfo)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetSampleLocationsEXT(VkCommandBuffer commandBuffer, IntPtr pSampleLocationsInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetScissor_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, uint scissorCount, VkRect2D* pScissors)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, uint scissorCount, ref VkRect2D pScissors)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, uint scissorCount, IntPtr pScissors)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetStencilCompareMask_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetStencilCompareMask(VkCommandBuffer commandBuffer, VkStencilFaceFlags faceMask, uint compareMask)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetStencilReference_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetStencilReference(VkCommandBuffer commandBuffer, VkStencilFaceFlags faceMask, uint reference)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetStencilWriteMask_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetStencilWriteMask(VkCommandBuffer commandBuffer, VkStencilFaceFlags faceMask, uint writeMask)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetViewport_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, VkViewport* pViewports)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, ref VkViewport pViewports)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, IntPtr pViewports)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdSetViewportWScalingNV_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetViewportWScalingNV(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, VkViewportWScalingNV* pViewportWScalings)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetViewportWScalingNV(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, ref VkViewportWScalingNV pViewportWScalings)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdSetViewportWScalingNV(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, IntPtr pViewportWScalings)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdUpdateBuffer_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdUpdateBuffer(VkCommandBuffer commandBuffer, VkBuffer dstBuffer, ulong dstOffset, ulong dataSize, void* pData)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdWaitEvents_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, ref VkEvent pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, ref VkMemoryBarrier pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, ref VkBufferMemoryBarrier pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, ref VkImageMemoryBarrier pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkCmdWaitEvents(VkCommandBuffer commandBuffer, uint eventCount, IntPtr pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, uint memoryBarrierCount, IntPtr pMemoryBarriers, uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers, uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCmdWriteTimestamp_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkCmdWriteTimestamp(VkCommandBuffer commandBuffer, VkPipelineStageFlags pipelineStage, VkQueryPool queryPool, uint query)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateAndroidSurfaceKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, VkAndroidSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, VkAndroidSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, VkAndroidSurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, VkAndroidSurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, VkAndroidSurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, VkAndroidSurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, ref VkAndroidSurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, ref VkAndroidSurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, ref VkAndroidSurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, ref VkAndroidSurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, ref VkAndroidSurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, ref VkAndroidSurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateAndroidSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateBuffer_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, VkBufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkBuffer* pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, VkBufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, VkBufferCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkBuffer* pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, VkBufferCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, VkBufferCreateInfo* pCreateInfo, IntPtr pAllocator, VkBuffer* pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, VkBufferCreateInfo* pCreateInfo, IntPtr pAllocator, out VkBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, ref VkBufferCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkBuffer* pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, ref VkBufferCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, ref VkBufferCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkBuffer* pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, ref VkBufferCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, ref VkBufferCreateInfo pCreateInfo, IntPtr pAllocator, VkBuffer* pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, ref VkBufferCreateInfo pCreateInfo, IntPtr pAllocator, out VkBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkBuffer* pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkBuffer* pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkBuffer* pBuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBuffer(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateBufferView_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, VkBufferViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkBufferView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, VkBufferViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkBufferView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, VkBufferViewCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkBufferView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, VkBufferViewCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkBufferView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, VkBufferViewCreateInfo* pCreateInfo, IntPtr pAllocator, VkBufferView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, VkBufferViewCreateInfo* pCreateInfo, IntPtr pAllocator, out VkBufferView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, ref VkBufferViewCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkBufferView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, ref VkBufferViewCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkBufferView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, ref VkBufferViewCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkBufferView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, ref VkBufferViewCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkBufferView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, ref VkBufferViewCreateInfo pCreateInfo, IntPtr pAllocator, VkBufferView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, ref VkBufferViewCreateInfo pCreateInfo, IntPtr pAllocator, out VkBufferView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkBufferView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkBufferView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkBufferView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkBufferView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkBufferView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateBufferView(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkBufferView pView)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateCommandPool_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkCommandPool* pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkCommandPool pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkCommandPool* pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkCommandPool pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, IntPtr pAllocator, VkCommandPool* pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, IntPtr pAllocator, out VkCommandPool pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, ref VkCommandPoolCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkCommandPool* pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, ref VkCommandPoolCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkCommandPool pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, ref VkCommandPoolCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkCommandPool* pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, ref VkCommandPoolCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkCommandPool pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, ref VkCommandPoolCreateInfo pCreateInfo, IntPtr pAllocator, VkCommandPool* pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, ref VkCommandPoolCreateInfo pCreateInfo, IntPtr pAllocator, out VkCommandPool pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkCommandPool* pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkCommandPool pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkCommandPool* pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkCommandPool pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkCommandPool* pCommandPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateCommandPool(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkCommandPool pCommandPool)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateComputePipelines_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo* pCreateInfos, ref VkAllocationCallbacks pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo* pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo* pCreateInfos, IntPtr pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo* pCreateInfos, IntPtr pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkComputePipelineCreateInfo pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkComputePipelineCreateInfo pCreateInfos, VkAllocationCallbacks* pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkComputePipelineCreateInfo pCreateInfos, ref VkAllocationCallbacks pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkComputePipelineCreateInfo pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkComputePipelineCreateInfo pCreateInfos, IntPtr pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkComputePipelineCreateInfo pCreateInfos, IntPtr pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, VkAllocationCallbacks* pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, ref VkAllocationCallbacks pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, IntPtr pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, IntPtr pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo[] pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo[] pCreateInfos, VkAllocationCallbacks* pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo[] pCreateInfos, ref VkAllocationCallbacks pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo[] pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo[] pCreateInfos, IntPtr pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkComputePipelineCreateInfo[] pCreateInfos, IntPtr pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateDebugReportCallbackEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, VkDebugReportCallbackCreateInfoEXT* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDebugReportCallbackEXT* pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, VkDebugReportCallbackCreateInfoEXT* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDebugReportCallbackEXT pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, VkDebugReportCallbackCreateInfoEXT* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDebugReportCallbackEXT* pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, VkDebugReportCallbackCreateInfoEXT* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDebugReportCallbackEXT pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, VkDebugReportCallbackCreateInfoEXT* pCreateInfo, IntPtr pAllocator, VkDebugReportCallbackEXT* pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, VkDebugReportCallbackCreateInfoEXT* pCreateInfo, IntPtr pAllocator, out VkDebugReportCallbackEXT pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, ref VkDebugReportCallbackCreateInfoEXT pCreateInfo, VkAllocationCallbacks* pAllocator, VkDebugReportCallbackEXT* pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, ref VkDebugReportCallbackCreateInfoEXT pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDebugReportCallbackEXT pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, ref VkDebugReportCallbackCreateInfoEXT pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDebugReportCallbackEXT* pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, ref VkDebugReportCallbackCreateInfoEXT pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDebugReportCallbackEXT pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, ref VkDebugReportCallbackCreateInfoEXT pCreateInfo, IntPtr pAllocator, VkDebugReportCallbackEXT* pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, ref VkDebugReportCallbackCreateInfoEXT pCreateInfo, IntPtr pAllocator, out VkDebugReportCallbackEXT pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkDebugReportCallbackEXT* pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDebugReportCallbackEXT pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDebugReportCallbackEXT* pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDebugReportCallbackEXT pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkDebugReportCallbackEXT* pCallback)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDebugReportCallbackEXT(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkDebugReportCallbackEXT pCallback)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateDescriptorPool_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, VkDescriptorPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorPool* pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, VkDescriptorPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDescriptorPool pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, VkDescriptorPoolCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDescriptorPool* pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, VkDescriptorPoolCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDescriptorPool pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, VkDescriptorPoolCreateInfo* pCreateInfo, IntPtr pAllocator, VkDescriptorPool* pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, VkDescriptorPoolCreateInfo* pCreateInfo, IntPtr pAllocator, out VkDescriptorPool pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, ref VkDescriptorPoolCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorPool* pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, ref VkDescriptorPoolCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDescriptorPool pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, ref VkDescriptorPoolCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDescriptorPool* pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, ref VkDescriptorPoolCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDescriptorPool pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, ref VkDescriptorPoolCreateInfo pCreateInfo, IntPtr pAllocator, VkDescriptorPool* pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, ref VkDescriptorPoolCreateInfo pCreateInfo, IntPtr pAllocator, out VkDescriptorPool pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorPool* pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDescriptorPool pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDescriptorPool* pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDescriptorPool pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkDescriptorPool* pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorPool(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkDescriptorPool pDescriptorPool)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateDescriptorSetLayout_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorSetLayout* pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDescriptorSetLayout pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDescriptorSetLayout* pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDescriptorSetLayout pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, IntPtr pAllocator, VkDescriptorSetLayout* pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, IntPtr pAllocator, out VkDescriptorSetLayout pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, ref VkDescriptorSetLayoutCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorSetLayout* pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, ref VkDescriptorSetLayoutCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDescriptorSetLayout pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, ref VkDescriptorSetLayoutCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDescriptorSetLayout* pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, ref VkDescriptorSetLayoutCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDescriptorSetLayout pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, ref VkDescriptorSetLayoutCreateInfo pCreateInfo, IntPtr pAllocator, VkDescriptorSetLayout* pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, ref VkDescriptorSetLayoutCreateInfo pCreateInfo, IntPtr pAllocator, out VkDescriptorSetLayout pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorSetLayout* pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDescriptorSetLayout pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDescriptorSetLayout* pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDescriptorSetLayout pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkDescriptorSetLayout* pSetLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorSetLayout(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkDescriptorSetLayout pSetLayout)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateDescriptorUpdateTemplateKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, VkDescriptorUpdateTemplateCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorUpdateTemplateKHR* pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, VkDescriptorUpdateTemplateCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDescriptorUpdateTemplateKHR pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, VkDescriptorUpdateTemplateCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDescriptorUpdateTemplateKHR* pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, VkDescriptorUpdateTemplateCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDescriptorUpdateTemplateKHR pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, VkDescriptorUpdateTemplateCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkDescriptorUpdateTemplateKHR* pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, VkDescriptorUpdateTemplateCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkDescriptorUpdateTemplateKHR pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, ref VkDescriptorUpdateTemplateCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorUpdateTemplateKHR* pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, ref VkDescriptorUpdateTemplateCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDescriptorUpdateTemplateKHR pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, ref VkDescriptorUpdateTemplateCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDescriptorUpdateTemplateKHR* pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, ref VkDescriptorUpdateTemplateCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDescriptorUpdateTemplateKHR pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, ref VkDescriptorUpdateTemplateCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkDescriptorUpdateTemplateKHR* pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, ref VkDescriptorUpdateTemplateCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkDescriptorUpdateTemplateKHR pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorUpdateTemplateKHR* pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDescriptorUpdateTemplateKHR pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDescriptorUpdateTemplateKHR* pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDescriptorUpdateTemplateKHR pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkDescriptorUpdateTemplateKHR* pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDescriptorUpdateTemplateKHR(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkDescriptorUpdateTemplateKHR pDescriptorUpdateTemplate)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateDevice_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDevice* pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDevice pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDevice* pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDevice pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, IntPtr pAllocator, VkDevice* pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, IntPtr pAllocator, out VkDevice pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, ref VkDeviceCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkDevice* pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, ref VkDeviceCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDevice pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, ref VkDeviceCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDevice* pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, ref VkDeviceCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDevice pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, ref VkDeviceCreateInfo pCreateInfo, IntPtr pAllocator, VkDevice* pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, ref VkDeviceCreateInfo pCreateInfo, IntPtr pAllocator, out VkDevice pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkDevice* pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDevice pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDevice* pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDevice pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, IntPtr pCreateInfo, IntPtr pAllocator, VkDevice* pDevice)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, IntPtr pCreateInfo, IntPtr pAllocator, out VkDevice pDevice)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateDisplayModeKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, VkDisplayModeCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDisplayModeKHR* pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, VkDisplayModeCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDisplayModeKHR pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, VkDisplayModeCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDisplayModeKHR* pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, VkDisplayModeCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDisplayModeKHR pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, VkDisplayModeCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkDisplayModeKHR* pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, VkDisplayModeCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkDisplayModeKHR pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, ref VkDisplayModeCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkDisplayModeKHR* pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, ref VkDisplayModeCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDisplayModeKHR pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, ref VkDisplayModeCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDisplayModeKHR* pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, ref VkDisplayModeCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDisplayModeKHR pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, ref VkDisplayModeCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkDisplayModeKHR* pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, ref VkDisplayModeCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkDisplayModeKHR pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkDisplayModeKHR* pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkDisplayModeKHR pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkDisplayModeKHR* pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkDisplayModeKHR pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, IntPtr pCreateInfo, IntPtr pAllocator, VkDisplayModeKHR* pMode)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayModeKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, IntPtr pCreateInfo, IntPtr pAllocator, out VkDisplayModeKHR pMode)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateDisplayPlaneSurfaceKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, VkDisplaySurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, VkDisplaySurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, VkDisplaySurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, VkDisplaySurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, VkDisplaySurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, VkDisplaySurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, ref VkDisplaySurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, ref VkDisplaySurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, ref VkDisplaySurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, ref VkDisplaySurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, ref VkDisplaySurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, ref VkDisplaySurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateDisplayPlaneSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateEvent_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, VkEventCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkEvent* pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, VkEventCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkEvent pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, VkEventCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkEvent* pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, VkEventCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkEvent pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, VkEventCreateInfo* pCreateInfo, IntPtr pAllocator, VkEvent* pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, VkEventCreateInfo* pCreateInfo, IntPtr pAllocator, out VkEvent pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, ref VkEventCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkEvent* pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, ref VkEventCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkEvent pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, ref VkEventCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkEvent* pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, ref VkEventCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkEvent pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, ref VkEventCreateInfo pCreateInfo, IntPtr pAllocator, VkEvent* pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, ref VkEventCreateInfo pCreateInfo, IntPtr pAllocator, out VkEvent pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkEvent* pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkEvent pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkEvent* pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkEvent pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkEvent* pEvent)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateEvent(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkEvent pEvent)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateFence_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, IntPtr pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, IntPtr pAllocator, out VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, ref VkFenceCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, ref VkFenceCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, ref VkFenceCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, ref VkFenceCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, ref VkFenceCreateInfo pCreateInfo, IntPtr pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, ref VkFenceCreateInfo pCreateInfo, IntPtr pAllocator, out VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFence(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkFence pFence)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateFramebuffer_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFramebuffer* pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkFramebuffer pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkFramebuffer* pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkFramebuffer pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, IntPtr pAllocator, VkFramebuffer* pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, IntPtr pAllocator, out VkFramebuffer pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, ref VkFramebufferCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkFramebuffer* pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, ref VkFramebufferCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkFramebuffer pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, ref VkFramebufferCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkFramebuffer* pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, ref VkFramebufferCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkFramebuffer pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, ref VkFramebufferCreateInfo pCreateInfo, IntPtr pAllocator, VkFramebuffer* pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, ref VkFramebufferCreateInfo pCreateInfo, IntPtr pAllocator, out VkFramebuffer pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkFramebuffer* pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkFramebuffer pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkFramebuffer* pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkFramebuffer pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkFramebuffer* pFramebuffer)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateFramebuffer(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkFramebuffer pFramebuffer)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateGraphicsPipelines_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, ref VkAllocationCallbacks pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, IntPtr pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, IntPtr pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkGraphicsPipelineCreateInfo pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkGraphicsPipelineCreateInfo pCreateInfos, VkAllocationCallbacks* pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkGraphicsPipelineCreateInfo pCreateInfos, ref VkAllocationCallbacks pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkGraphicsPipelineCreateInfo pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkGraphicsPipelineCreateInfo pCreateInfos, IntPtr pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, ref VkGraphicsPipelineCreateInfo pCreateInfos, IntPtr pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, VkAllocationCallbacks* pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, ref VkAllocationCallbacks pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, IntPtr pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, IntPtr pCreateInfos, IntPtr pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo[] pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo[] pCreateInfos, VkAllocationCallbacks* pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo[] pCreateInfos, ref VkAllocationCallbacks pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo[] pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo[] pCreateInfos, IntPtr pAllocator, VkPipeline* pPipelines)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo[] pCreateInfos, IntPtr pAllocator, out VkPipeline pPipelines)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateImage_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, VkImageCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImage* pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, VkImageCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkImage pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, VkImageCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkImage* pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, VkImageCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkImage pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, VkImageCreateInfo* pCreateInfo, IntPtr pAllocator, VkImage* pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, VkImageCreateInfo* pCreateInfo, IntPtr pAllocator, out VkImage pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, ref VkImageCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkImage* pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, ref VkImageCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkImage pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, ref VkImageCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkImage* pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, ref VkImageCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkImage pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, ref VkImageCreateInfo pCreateInfo, IntPtr pAllocator, VkImage* pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, ref VkImageCreateInfo pCreateInfo, IntPtr pAllocator, out VkImage pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkImage* pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkImage pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkImage* pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkImage pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkImage* pImage)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImage(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkImage pImage)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateImageView_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImageView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkImageView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkImageView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkImageView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, IntPtr pAllocator, VkImageView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, IntPtr pAllocator, out VkImageView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, ref VkImageViewCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkImageView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, ref VkImageViewCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkImageView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, ref VkImageViewCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkImageView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, ref VkImageViewCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkImageView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, ref VkImageViewCreateInfo pCreateInfo, IntPtr pAllocator, VkImageView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, ref VkImageViewCreateInfo pCreateInfo, IntPtr pAllocator, out VkImageView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkImageView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkImageView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkImageView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkImageView pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkImageView* pView)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateImageView(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkImageView pView)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateIndirectCommandsLayoutNVX_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, VkIndirectCommandsLayoutCreateInfoNVX* pCreateInfo, VkAllocationCallbacks* pAllocator, VkIndirectCommandsLayoutNVX* pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, VkIndirectCommandsLayoutCreateInfoNVX* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkIndirectCommandsLayoutNVX pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, VkIndirectCommandsLayoutCreateInfoNVX* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkIndirectCommandsLayoutNVX* pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, VkIndirectCommandsLayoutCreateInfoNVX* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkIndirectCommandsLayoutNVX pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, VkIndirectCommandsLayoutCreateInfoNVX* pCreateInfo, IntPtr pAllocator, VkIndirectCommandsLayoutNVX* pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, VkIndirectCommandsLayoutCreateInfoNVX* pCreateInfo, IntPtr pAllocator, out VkIndirectCommandsLayoutNVX pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, ref VkIndirectCommandsLayoutCreateInfoNVX pCreateInfo, VkAllocationCallbacks* pAllocator, VkIndirectCommandsLayoutNVX* pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, ref VkIndirectCommandsLayoutCreateInfoNVX pCreateInfo, VkAllocationCallbacks* pAllocator, out VkIndirectCommandsLayoutNVX pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, ref VkIndirectCommandsLayoutCreateInfoNVX pCreateInfo, ref VkAllocationCallbacks pAllocator, VkIndirectCommandsLayoutNVX* pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, ref VkIndirectCommandsLayoutCreateInfoNVX pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkIndirectCommandsLayoutNVX pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, ref VkIndirectCommandsLayoutCreateInfoNVX pCreateInfo, IntPtr pAllocator, VkIndirectCommandsLayoutNVX* pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, ref VkIndirectCommandsLayoutCreateInfoNVX pCreateInfo, IntPtr pAllocator, out VkIndirectCommandsLayoutNVX pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkIndirectCommandsLayoutNVX* pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkIndirectCommandsLayoutNVX pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkIndirectCommandsLayoutNVX* pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkIndirectCommandsLayoutNVX pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkIndirectCommandsLayoutNVX* pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIndirectCommandsLayoutNVX(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkIndirectCommandsLayoutNVX pIndirectCommandsLayout)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateInstance_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkInstance pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(VkInstanceCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkInstance* pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(VkInstanceCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkInstance pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(VkInstanceCreateInfo* pCreateInfo, IntPtr pAllocator, VkInstance* pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(VkInstanceCreateInfo* pCreateInfo, IntPtr pAllocator, out VkInstance pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(ref VkInstanceCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(ref VkInstanceCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkInstance pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(ref VkInstanceCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkInstance* pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(ref VkInstanceCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkInstance pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(ref VkInstanceCreateInfo pCreateInfo, IntPtr pAllocator, VkInstance* pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(ref VkInstanceCreateInfo pCreateInfo, IntPtr pAllocator, out VkInstance pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkInstance pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkInstance* pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkInstance pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(IntPtr pCreateInfo, IntPtr pAllocator, VkInstance* pInstance)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED, VK_ERROR_LAYER_NOT_PRESENT, VK_ERROR_EXTENSION_NOT_PRESENT, VK_ERROR_INCOMPATIBLE_DRIVER</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateInstance(IntPtr pCreateInfo, IntPtr pAllocator, out VkInstance pInstance)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateIOSSurfaceMVK_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, VkIOSSurfaceCreateInfoMVK* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, VkIOSSurfaceCreateInfoMVK* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, VkIOSSurfaceCreateInfoMVK* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, VkIOSSurfaceCreateInfoMVK* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, VkIOSSurfaceCreateInfoMVK* pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, VkIOSSurfaceCreateInfoMVK* pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, ref VkIOSSurfaceCreateInfoMVK pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, ref VkIOSSurfaceCreateInfoMVK pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, ref VkIOSSurfaceCreateInfoMVK pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, ref VkIOSSurfaceCreateInfoMVK pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, ref VkIOSSurfaceCreateInfoMVK pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, ref VkIOSSurfaceCreateInfoMVK pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateIOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateMacOSSurfaceMVK_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, VkMacOSSurfaceCreateInfoMVK* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, VkMacOSSurfaceCreateInfoMVK* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, VkMacOSSurfaceCreateInfoMVK* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, VkMacOSSurfaceCreateInfoMVK* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, VkMacOSSurfaceCreateInfoMVK* pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, VkMacOSSurfaceCreateInfoMVK* pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, ref VkMacOSSurfaceCreateInfoMVK pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, ref VkMacOSSurfaceCreateInfoMVK pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, ref VkMacOSSurfaceCreateInfoMVK pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, ref VkMacOSSurfaceCreateInfoMVK pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, ref VkMacOSSurfaceCreateInfoMVK pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, ref VkMacOSSurfaceCreateInfoMVK pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMacOSSurfaceMVK(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateMirSurfaceKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, VkMirSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, VkMirSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, VkMirSurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, VkMirSurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, VkMirSurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, VkMirSurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, ref VkMirSurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, ref VkMirSurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, ref VkMirSurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, ref VkMirSurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, ref VkMirSurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, ref VkMirSurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateMirSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateObjectTableNVX_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, VkObjectTableCreateInfoNVX* pCreateInfo, VkAllocationCallbacks* pAllocator, VkObjectTableNVX* pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, VkObjectTableCreateInfoNVX* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkObjectTableNVX pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, VkObjectTableCreateInfoNVX* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkObjectTableNVX* pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, VkObjectTableCreateInfoNVX* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkObjectTableNVX pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, VkObjectTableCreateInfoNVX* pCreateInfo, IntPtr pAllocator, VkObjectTableNVX* pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, VkObjectTableCreateInfoNVX* pCreateInfo, IntPtr pAllocator, out VkObjectTableNVX pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, ref VkObjectTableCreateInfoNVX pCreateInfo, VkAllocationCallbacks* pAllocator, VkObjectTableNVX* pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, ref VkObjectTableCreateInfoNVX pCreateInfo, VkAllocationCallbacks* pAllocator, out VkObjectTableNVX pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, ref VkObjectTableCreateInfoNVX pCreateInfo, ref VkAllocationCallbacks pAllocator, VkObjectTableNVX* pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, ref VkObjectTableCreateInfoNVX pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkObjectTableNVX pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, ref VkObjectTableCreateInfoNVX pCreateInfo, IntPtr pAllocator, VkObjectTableNVX* pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, ref VkObjectTableCreateInfoNVX pCreateInfo, IntPtr pAllocator, out VkObjectTableNVX pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkObjectTableNVX* pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkObjectTableNVX pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkObjectTableNVX* pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkObjectTableNVX pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkObjectTableNVX* pObjectTable)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateObjectTableNVX(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkObjectTableNVX pObjectTable)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreatePipelineCache_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, VkPipelineCacheCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineCache* pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, VkPipelineCacheCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkPipelineCache pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, VkPipelineCacheCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkPipelineCache* pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, VkPipelineCacheCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkPipelineCache pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, VkPipelineCacheCreateInfo* pCreateInfo, IntPtr pAllocator, VkPipelineCache* pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, VkPipelineCacheCreateInfo* pCreateInfo, IntPtr pAllocator, out VkPipelineCache pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, ref VkPipelineCacheCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineCache* pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, ref VkPipelineCacheCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkPipelineCache pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, ref VkPipelineCacheCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkPipelineCache* pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, ref VkPipelineCacheCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkPipelineCache pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, ref VkPipelineCacheCreateInfo pCreateInfo, IntPtr pAllocator, VkPipelineCache* pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, ref VkPipelineCacheCreateInfo pCreateInfo, IntPtr pAllocator, out VkPipelineCache pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineCache* pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkPipelineCache pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkPipelineCache* pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkPipelineCache pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkPipelineCache* pPipelineCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineCache(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkPipelineCache pPipelineCache)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreatePipelineLayout_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineLayout* pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkPipelineLayout pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkPipelineLayout* pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkPipelineLayout pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, IntPtr pAllocator, VkPipelineLayout* pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, IntPtr pAllocator, out VkPipelineLayout pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, ref VkPipelineLayoutCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineLayout* pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, ref VkPipelineLayoutCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkPipelineLayout pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, ref VkPipelineLayoutCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkPipelineLayout* pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, ref VkPipelineLayoutCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkPipelineLayout pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, ref VkPipelineLayoutCreateInfo pCreateInfo, IntPtr pAllocator, VkPipelineLayout* pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, ref VkPipelineLayoutCreateInfo pCreateInfo, IntPtr pAllocator, out VkPipelineLayout pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineLayout* pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkPipelineLayout pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkPipelineLayout* pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkPipelineLayout pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkPipelineLayout* pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreatePipelineLayout(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkPipelineLayout pPipelineLayout)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateQueryPool_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, VkQueryPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkQueryPool* pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, VkQueryPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkQueryPool pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, VkQueryPoolCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkQueryPool* pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, VkQueryPoolCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkQueryPool pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, VkQueryPoolCreateInfo* pCreateInfo, IntPtr pAllocator, VkQueryPool* pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, VkQueryPoolCreateInfo* pCreateInfo, IntPtr pAllocator, out VkQueryPool pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, ref VkQueryPoolCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkQueryPool* pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, ref VkQueryPoolCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkQueryPool pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, ref VkQueryPoolCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkQueryPool* pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, ref VkQueryPoolCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkQueryPool pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, ref VkQueryPoolCreateInfo pCreateInfo, IntPtr pAllocator, VkQueryPool* pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, ref VkQueryPoolCreateInfo pCreateInfo, IntPtr pAllocator, out VkQueryPool pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkQueryPool* pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkQueryPool pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkQueryPool* pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkQueryPool pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkQueryPool* pQueryPool)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateQueryPool(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkQueryPool pQueryPool)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateRenderPass_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkRenderPass* pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkRenderPass pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkRenderPass* pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkRenderPass pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, IntPtr pAllocator, VkRenderPass* pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, IntPtr pAllocator, out VkRenderPass pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, ref VkRenderPassCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkRenderPass* pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, ref VkRenderPassCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkRenderPass pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, ref VkRenderPassCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkRenderPass* pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, ref VkRenderPassCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkRenderPass pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, ref VkRenderPassCreateInfo pCreateInfo, IntPtr pAllocator, VkRenderPass* pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, ref VkRenderPassCreateInfo pCreateInfo, IntPtr pAllocator, out VkRenderPass pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkRenderPass* pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkRenderPass pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkRenderPass* pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkRenderPass pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkRenderPass* pRenderPass)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateRenderPass(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkRenderPass pRenderPass)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateSampler_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, VkSamplerCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSampler* pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, VkSamplerCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSampler pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, VkSamplerCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSampler* pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, VkSamplerCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSampler pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, VkSamplerCreateInfo* pCreateInfo, IntPtr pAllocator, VkSampler* pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, VkSamplerCreateInfo* pCreateInfo, IntPtr pAllocator, out VkSampler pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, ref VkSamplerCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkSampler* pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, ref VkSamplerCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSampler pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, ref VkSamplerCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSampler* pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, ref VkSamplerCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSampler pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, ref VkSamplerCreateInfo pCreateInfo, IntPtr pAllocator, VkSampler* pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, ref VkSamplerCreateInfo pCreateInfo, IntPtr pAllocator, out VkSampler pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSampler* pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSampler pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSampler* pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSampler pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkSampler* pSampler)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_TOO_MANY_OBJECTS</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSampler(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkSampler pSampler)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateSamplerYcbcrConversionKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, VkSamplerYcbcrConversionCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSamplerYcbcrConversionKHR* pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, VkSamplerYcbcrConversionCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSamplerYcbcrConversionKHR pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, VkSamplerYcbcrConversionCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSamplerYcbcrConversionKHR* pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, VkSamplerYcbcrConversionCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSamplerYcbcrConversionKHR pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, VkSamplerYcbcrConversionCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkSamplerYcbcrConversionKHR* pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, VkSamplerYcbcrConversionCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkSamplerYcbcrConversionKHR pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, ref VkSamplerYcbcrConversionCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkSamplerYcbcrConversionKHR* pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, ref VkSamplerYcbcrConversionCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSamplerYcbcrConversionKHR pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, ref VkSamplerYcbcrConversionCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSamplerYcbcrConversionKHR* pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, ref VkSamplerYcbcrConversionCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSamplerYcbcrConversionKHR pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, ref VkSamplerYcbcrConversionCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkSamplerYcbcrConversionKHR* pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, ref VkSamplerYcbcrConversionCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkSamplerYcbcrConversionKHR pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSamplerYcbcrConversionKHR* pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSamplerYcbcrConversionKHR pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSamplerYcbcrConversionKHR* pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSamplerYcbcrConversionKHR pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkSamplerYcbcrConversionKHR* pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSamplerYcbcrConversionKHR(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkSamplerYcbcrConversionKHR pYcbcrConversion)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateSemaphore_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSemaphore* pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSemaphore pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSemaphore* pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSemaphore pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, IntPtr pAllocator, VkSemaphore* pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, IntPtr pAllocator, out VkSemaphore pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, ref VkSemaphoreCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkSemaphore* pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, ref VkSemaphoreCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSemaphore pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, ref VkSemaphoreCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSemaphore* pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, ref VkSemaphoreCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSemaphore pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, ref VkSemaphoreCreateInfo pCreateInfo, IntPtr pAllocator, VkSemaphore* pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, ref VkSemaphoreCreateInfo pCreateInfo, IntPtr pAllocator, out VkSemaphore pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSemaphore* pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSemaphore pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSemaphore* pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSemaphore pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkSemaphore* pSemaphore)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSemaphore(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkSemaphore pSemaphore)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateShaderModule_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkShaderModule* pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkShaderModule pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkShaderModule* pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkShaderModule pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, IntPtr pAllocator, VkShaderModule* pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, IntPtr pAllocator, out VkShaderModule pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, ref VkShaderModuleCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, VkShaderModule* pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, ref VkShaderModuleCreateInfo pCreateInfo, VkAllocationCallbacks* pAllocator, out VkShaderModule pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, ref VkShaderModuleCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, VkShaderModule* pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, ref VkShaderModuleCreateInfo pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkShaderModule pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, ref VkShaderModuleCreateInfo pCreateInfo, IntPtr pAllocator, VkShaderModule* pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, ref VkShaderModuleCreateInfo pCreateInfo, IntPtr pAllocator, out VkShaderModule pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkShaderModule* pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkShaderModule pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkShaderModule* pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkShaderModule pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkShaderModule* pShaderModule)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INVALID_SHADER_NV</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateShaderModule(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkShaderModule pShaderModule)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateSharedSwapchainsKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR* pCreateInfos, VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR* pCreateInfos, VkAllocationCallbacks* pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR* pCreateInfos, ref VkAllocationCallbacks pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR* pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR* pCreateInfos, IntPtr pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR* pCreateInfos, IntPtr pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, ref VkSwapchainCreateInfoKHR pCreateInfos, VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, ref VkSwapchainCreateInfoKHR pCreateInfos, VkAllocationCallbacks* pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, ref VkSwapchainCreateInfoKHR pCreateInfos, ref VkAllocationCallbacks pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, ref VkSwapchainCreateInfoKHR pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, ref VkSwapchainCreateInfoKHR pCreateInfos, IntPtr pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, ref VkSwapchainCreateInfoKHR pCreateInfos, IntPtr pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, IntPtr pCreateInfos, VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, IntPtr pCreateInfos, VkAllocationCallbacks* pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, IntPtr pCreateInfos, ref VkAllocationCallbacks pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, IntPtr pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, IntPtr pCreateInfos, IntPtr pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, IntPtr pCreateInfos, IntPtr pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR[] pCreateInfos, VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR[] pCreateInfos, VkAllocationCallbacks* pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR[] pCreateInfos, ref VkAllocationCallbacks pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR[] pCreateInfos, ref VkAllocationCallbacks pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR[] pCreateInfos, IntPtr pAllocator, VkSwapchainKHR* pSwapchains)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INCOMPATIBLE_DISPLAY_KHR, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSharedSwapchainsKHR(VkDevice device, uint swapchainCount, VkSwapchainCreateInfoKHR[] pCreateInfos, IntPtr pAllocator, out VkSwapchainKHR pSwapchains)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateSwapchainKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, VkSwapchainCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, VkSwapchainCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSwapchainKHR pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, VkSwapchainCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSwapchainKHR* pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, VkSwapchainCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSwapchainKHR pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, VkSwapchainCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkSwapchainKHR* pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, VkSwapchainCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkSwapchainKHR pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, ref VkSwapchainCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, ref VkSwapchainCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSwapchainKHR pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, ref VkSwapchainCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSwapchainKHR* pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, ref VkSwapchainCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSwapchainKHR pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, ref VkSwapchainCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkSwapchainKHR* pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, ref VkSwapchainCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkSwapchainKHR pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSwapchainKHR pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSwapchainKHR* pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSwapchainKHR pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkSwapchainKHR* pSwapchain)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateSwapchainKHR(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkSwapchainKHR pSwapchain)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateValidationCacheEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, VkValidationCacheCreateInfoEXT* pCreateInfo, VkAllocationCallbacks* pAllocator, VkValidationCacheEXT* pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, VkValidationCacheCreateInfoEXT* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkValidationCacheEXT pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, VkValidationCacheCreateInfoEXT* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkValidationCacheEXT* pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, VkValidationCacheCreateInfoEXT* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkValidationCacheEXT pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, VkValidationCacheCreateInfoEXT* pCreateInfo, IntPtr pAllocator, VkValidationCacheEXT* pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, VkValidationCacheCreateInfoEXT* pCreateInfo, IntPtr pAllocator, out VkValidationCacheEXT pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, ref VkValidationCacheCreateInfoEXT pCreateInfo, VkAllocationCallbacks* pAllocator, VkValidationCacheEXT* pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, ref VkValidationCacheCreateInfoEXT pCreateInfo, VkAllocationCallbacks* pAllocator, out VkValidationCacheEXT pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, ref VkValidationCacheCreateInfoEXT pCreateInfo, ref VkAllocationCallbacks pAllocator, VkValidationCacheEXT* pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, ref VkValidationCacheCreateInfoEXT pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkValidationCacheEXT pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, ref VkValidationCacheCreateInfoEXT pCreateInfo, IntPtr pAllocator, VkValidationCacheEXT* pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, ref VkValidationCacheCreateInfoEXT pCreateInfo, IntPtr pAllocator, out VkValidationCacheEXT pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkValidationCacheEXT* pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkValidationCacheEXT pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkValidationCacheEXT* pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkValidationCacheEXT pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, VkValidationCacheEXT* pValidationCache)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateValidationCacheEXT(VkDevice device, IntPtr pCreateInfo, IntPtr pAllocator, out VkValidationCacheEXT pValidationCache)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateViSurfaceNN_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, VkViSurfaceCreateInfoNN* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, VkViSurfaceCreateInfoNN* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, VkViSurfaceCreateInfoNN* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, VkViSurfaceCreateInfoNN* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, VkViSurfaceCreateInfoNN* pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, VkViSurfaceCreateInfoNN* pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, ref VkViSurfaceCreateInfoNN pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, ref VkViSurfaceCreateInfoNN pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, ref VkViSurfaceCreateInfoNN pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, ref VkViSurfaceCreateInfoNN pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, ref VkViSurfaceCreateInfoNN pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, ref VkViSurfaceCreateInfoNN pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_NATIVE_WINDOW_IN_USE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateViSurfaceNN(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateWaylandSurfaceKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, VkWaylandSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, VkWaylandSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, VkWaylandSurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, VkWaylandSurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, VkWaylandSurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, VkWaylandSurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, ref VkWaylandSurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, ref VkWaylandSurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, ref VkWaylandSurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, ref VkWaylandSurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, ref VkWaylandSurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, ref VkWaylandSurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateWin32SurfaceKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, ref VkWin32SurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, ref VkWin32SurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, ref VkWin32SurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, ref VkWin32SurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, ref VkWin32SurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, ref VkWin32SurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateWin32SurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateXcbSurfaceKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, VkXcbSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, VkXcbSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, VkXcbSurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, VkXcbSurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, VkXcbSurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, VkXcbSurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, ref VkXcbSurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, ref VkXcbSurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, ref VkXcbSurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, ref VkXcbSurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, ref VkXcbSurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, ref VkXcbSurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXcbSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkCreateXlibSurfaceKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, VkXlibSurfaceCreateInfoKHR* pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, ref VkXlibSurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, ref VkXlibSurfaceCreateInfoKHR pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, ref VkXlibSurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, ref VkXlibSurfaceCreateInfoKHR pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, ref VkXlibSurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, ref VkXlibSurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, VkAllocationCallbacks* pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, ref VkAllocationCallbacks pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, VkSurfaceKHR* pSurface)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkCreateXlibSurfaceKHR(VkInstance instance, IntPtr pCreateInfo, IntPtr pAllocator, out VkSurfaceKHR pSurface)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDebugMarkerSetObjectNameEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkDebugMarkerSetObjectNameEXT(VkDevice device, VkDebugMarkerObjectNameInfoEXT* pNameInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkDebugMarkerSetObjectNameEXT(VkDevice device, ref VkDebugMarkerObjectNameInfoEXT pNameInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkDebugMarkerSetObjectNameEXT(VkDevice device, IntPtr pNameInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDebugMarkerSetObjectTagEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkDebugMarkerSetObjectTagEXT(VkDevice device, VkDebugMarkerObjectTagInfoEXT* pTagInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkDebugMarkerSetObjectTagEXT(VkDevice device, ref VkDebugMarkerObjectTagInfoEXT pTagInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkDebugMarkerSetObjectTagEXT(VkDevice device, IntPtr pTagInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDebugReportMessageEXT_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDebugReportMessageEXT(VkInstance instance, VkDebugReportFlagsEXT flags, VkDebugReportObjectTypeEXT objectType, ulong @object, UIntPtr location, int messageCode, byte* pLayerPrefix, byte* pMessage)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDebugReportMessageEXT(VkInstance instance, VkDebugReportFlagsEXT flags, VkDebugReportObjectTypeEXT objectType, ulong @object, UIntPtr location, int messageCode, byte* pLayerPrefix, string pMessage)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDebugReportMessageEXT(VkInstance instance, VkDebugReportFlagsEXT flags, VkDebugReportObjectTypeEXT objectType, ulong @object, UIntPtr location, int messageCode, string pLayerPrefix, byte* pMessage)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDebugReportMessageEXT(VkInstance instance, VkDebugReportFlagsEXT flags, VkDebugReportObjectTypeEXT objectType, ulong @object, UIntPtr location, int messageCode, string pLayerPrefix, string pMessage)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyBuffer_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyBuffer(VkDevice device, VkBuffer buffer, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyBuffer(VkDevice device, VkBuffer buffer, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyBuffer(VkDevice device, VkBuffer buffer, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyBufferView_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyBufferView(VkDevice device, VkBufferView bufferView, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyBufferView(VkDevice device, VkBufferView bufferView, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyBufferView(VkDevice device, VkBufferView bufferView, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyCommandPool_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyCommandPool(VkDevice device, VkCommandPool commandPool, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyCommandPool(VkDevice device, VkCommandPool commandPool, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyCommandPool(VkDevice device, VkCommandPool commandPool, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyDebugReportCallbackEXT_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDebugReportCallbackEXT(VkInstance instance, VkDebugReportCallbackEXT callback, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDebugReportCallbackEXT(VkInstance instance, VkDebugReportCallbackEXT callback, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDebugReportCallbackEXT(VkInstance instance, VkDebugReportCallbackEXT callback, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyDescriptorPool_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDescriptorPool(VkDevice device, VkDescriptorPool descriptorPool, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDescriptorPool(VkDevice device, VkDescriptorPool descriptorPool, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDescriptorPool(VkDevice device, VkDescriptorPool descriptorPool, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyDescriptorSetLayout_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDescriptorSetLayout(VkDevice device, VkDescriptorSetLayout descriptorSetLayout, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDescriptorSetLayout(VkDevice device, VkDescriptorSetLayout descriptorSetLayout, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDescriptorSetLayout(VkDevice device, VkDescriptorSetLayout descriptorSetLayout, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyDescriptorUpdateTemplateKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDescriptorUpdateTemplateKHR(VkDevice device, VkDescriptorUpdateTemplateKHR descriptorUpdateTemplate, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDescriptorUpdateTemplateKHR(VkDevice device, VkDescriptorUpdateTemplateKHR descriptorUpdateTemplate, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDescriptorUpdateTemplateKHR(VkDevice device, VkDescriptorUpdateTemplateKHR descriptorUpdateTemplate, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyDevice_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDevice(VkDevice device, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDevice(VkDevice device, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyDevice(VkDevice device, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyEvent_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyEvent(VkDevice device, VkEvent @event, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyEvent(VkDevice device, VkEvent @event, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyEvent(VkDevice device, VkEvent @event, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyFence_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyFence(VkDevice device, VkFence fence, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyFence(VkDevice device, VkFence fence, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyFence(VkDevice device, VkFence fence, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyFramebuffer_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyFramebuffer(VkDevice device, VkFramebuffer framebuffer, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyFramebuffer(VkDevice device, VkFramebuffer framebuffer, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyFramebuffer(VkDevice device, VkFramebuffer framebuffer, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyImage_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyImage(VkDevice device, VkImage image, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyImage(VkDevice device, VkImage image, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyImage(VkDevice device, VkImage image, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyImageView_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyImageView(VkDevice device, VkImageView imageView, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyImageView(VkDevice device, VkImageView imageView, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyImageView(VkDevice device, VkImageView imageView, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyIndirectCommandsLayoutNVX_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyIndirectCommandsLayoutNVX(VkDevice device, VkIndirectCommandsLayoutNVX indirectCommandsLayout, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyIndirectCommandsLayoutNVX(VkDevice device, VkIndirectCommandsLayoutNVX indirectCommandsLayout, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyIndirectCommandsLayoutNVX(VkDevice device, VkIndirectCommandsLayoutNVX indirectCommandsLayout, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyInstance_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyInstance(VkInstance instance, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyInstance(VkInstance instance, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyInstance(VkInstance instance, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyObjectTableNVX_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyObjectTableNVX(VkDevice device, VkObjectTableNVX objectTable, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyObjectTableNVX(VkDevice device, VkObjectTableNVX objectTable, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyObjectTableNVX(VkDevice device, VkObjectTableNVX objectTable, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyPipeline_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyPipeline(VkDevice device, VkPipeline pipeline, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyPipeline(VkDevice device, VkPipeline pipeline, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyPipeline(VkDevice device, VkPipeline pipeline, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyPipelineCache_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyPipelineCache(VkDevice device, VkPipelineCache pipelineCache, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyPipelineCache(VkDevice device, VkPipelineCache pipelineCache, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyPipelineCache(VkDevice device, VkPipelineCache pipelineCache, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyPipelineLayout_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyPipelineLayout(VkDevice device, VkPipelineLayout pipelineLayout, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyPipelineLayout(VkDevice device, VkPipelineLayout pipelineLayout, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyPipelineLayout(VkDevice device, VkPipelineLayout pipelineLayout, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyQueryPool_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyQueryPool(VkDevice device, VkQueryPool queryPool, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyQueryPool(VkDevice device, VkQueryPool queryPool, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyQueryPool(VkDevice device, VkQueryPool queryPool, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyRenderPass_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyRenderPass(VkDevice device, VkRenderPass renderPass, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyRenderPass(VkDevice device, VkRenderPass renderPass, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyRenderPass(VkDevice device, VkRenderPass renderPass, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroySampler_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroySampler(VkDevice device, VkSampler sampler, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroySampler(VkDevice device, VkSampler sampler, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroySampler(VkDevice device, VkSampler sampler, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroySamplerYcbcrConversionKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroySamplerYcbcrConversionKHR(VkDevice device, VkSamplerYcbcrConversionKHR ycbcrConversion, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroySamplerYcbcrConversionKHR(VkDevice device, VkSamplerYcbcrConversionKHR ycbcrConversion, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroySamplerYcbcrConversionKHR(VkDevice device, VkSamplerYcbcrConversionKHR ycbcrConversion, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroySemaphore_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroySemaphore(VkDevice device, VkSemaphore semaphore, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroySemaphore(VkDevice device, VkSemaphore semaphore, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroySemaphore(VkDevice device, VkSemaphore semaphore, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyShaderModule_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyShaderModule(VkDevice device, VkShaderModule shaderModule, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyShaderModule(VkDevice device, VkShaderModule shaderModule, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyShaderModule(VkDevice device, VkShaderModule shaderModule, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroySurfaceKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroySurfaceKHR(VkInstance instance, VkSurfaceKHR surface, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroySurfaceKHR(VkInstance instance, VkSurfaceKHR surface, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroySurfaceKHR(VkInstance instance, VkSurfaceKHR surface, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroySwapchainKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroySwapchainKHR(VkDevice device, VkSwapchainKHR swapchain, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroySwapchainKHR(VkDevice device, VkSwapchainKHR swapchain, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroySwapchainKHR(VkDevice device, VkSwapchainKHR swapchain, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDestroyValidationCacheEXT_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkDestroyValidationCacheEXT(VkDevice device, VkValidationCacheEXT validationCache, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyValidationCacheEXT(VkDevice device, VkValidationCacheEXT validationCache, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkDestroyValidationCacheEXT(VkDevice device, VkValidationCacheEXT validationCache, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDeviceWaitIdle_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkDeviceWaitIdle(VkDevice device)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkDisplayPowerControlEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkDisplayPowerControlEXT(VkDevice device, VkDisplayKHR display, VkDisplayPowerInfoEXT* pDisplayPowerInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkDisplayPowerControlEXT(VkDevice device, VkDisplayKHR display, ref VkDisplayPowerInfoEXT pDisplayPowerInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkDisplayPowerControlEXT(VkDevice device, VkDisplayKHR display, IntPtr pDisplayPowerInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkEndCommandBuffer_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEndCommandBuffer(VkCommandBuffer commandBuffer)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkEnumerateDeviceExtensionProperties_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, uint* pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, uint* pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, ref uint pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, ref uint pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, ref uint pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, IntPtr pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, IntPtr pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, IntPtr pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string pLayerName, uint* pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string pLayerName, uint* pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string pLayerName, ref uint pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string pLayerName, ref uint pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string pLayerName, ref uint pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string pLayerName, IntPtr pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string pLayerName, IntPtr pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string pLayerName, IntPtr pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkEnumerateDeviceLayerProperties_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceLayerProperties(VkPhysicalDevice physicalDevice, uint* pPropertyCount, VkLayerProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceLayerProperties(VkPhysicalDevice physicalDevice, uint* pPropertyCount, ref VkLayerProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceLayerProperties(VkPhysicalDevice physicalDevice, uint* pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceLayerProperties(VkPhysicalDevice physicalDevice, ref uint pPropertyCount, VkLayerProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceLayerProperties(VkPhysicalDevice physicalDevice, ref uint pPropertyCount, ref VkLayerProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceLayerProperties(VkPhysicalDevice physicalDevice, ref uint pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceLayerProperties(VkPhysicalDevice physicalDevice, IntPtr pPropertyCount, VkLayerProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceLayerProperties(VkPhysicalDevice physicalDevice, IntPtr pPropertyCount, ref VkLayerProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateDeviceLayerProperties(VkPhysicalDevice physicalDevice, IntPtr pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkEnumerateInstanceExtensionProperties_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(byte* pLayerName, ref uint pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(byte* pLayerName, ref uint pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(byte* pLayerName, ref uint pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(byte* pLayerName, IntPtr pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(byte* pLayerName, IntPtr pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(byte* pLayerName, IntPtr pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(string pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(string pLayerName, uint* pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(string pLayerName, uint* pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(string pLayerName, ref uint pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(string pLayerName, ref uint pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(string pLayerName, ref uint pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(string pLayerName, IntPtr pPropertyCount, VkExtensionProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(string pLayerName, IntPtr pPropertyCount, ref VkExtensionProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_LAYER_NOT_PRESENT</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceExtensionProperties(string pLayerName, IntPtr pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkEnumerateInstanceLayerProperties_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceLayerProperties(uint* pPropertyCount, VkLayerProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceLayerProperties(uint* pPropertyCount, ref VkLayerProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceLayerProperties(uint* pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceLayerProperties(ref uint pPropertyCount, VkLayerProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceLayerProperties(ref uint pPropertyCount, ref VkLayerProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceLayerProperties(ref uint pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceLayerProperties(IntPtr pPropertyCount, VkLayerProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceLayerProperties(IntPtr pPropertyCount, ref VkLayerProperties pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumerateInstanceLayerProperties(IntPtr pPropertyCount, IntPtr pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkEnumeratePhysicalDeviceGroupsKHX_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDeviceGroupsKHX(VkInstance instance, uint* pPhysicalDeviceGroupCount, VkPhysicalDeviceGroupPropertiesKHX* pPhysicalDeviceGroupProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDeviceGroupsKHX(VkInstance instance, uint* pPhysicalDeviceGroupCount, ref VkPhysicalDeviceGroupPropertiesKHX pPhysicalDeviceGroupProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDeviceGroupsKHX(VkInstance instance, uint* pPhysicalDeviceGroupCount, IntPtr pPhysicalDeviceGroupProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDeviceGroupsKHX(VkInstance instance, ref uint pPhysicalDeviceGroupCount, VkPhysicalDeviceGroupPropertiesKHX* pPhysicalDeviceGroupProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDeviceGroupsKHX(VkInstance instance, ref uint pPhysicalDeviceGroupCount, ref VkPhysicalDeviceGroupPropertiesKHX pPhysicalDeviceGroupProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDeviceGroupsKHX(VkInstance instance, ref uint pPhysicalDeviceGroupCount, IntPtr pPhysicalDeviceGroupProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDeviceGroupsKHX(VkInstance instance, IntPtr pPhysicalDeviceGroupCount, VkPhysicalDeviceGroupPropertiesKHX* pPhysicalDeviceGroupProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDeviceGroupsKHX(VkInstance instance, IntPtr pPhysicalDeviceGroupCount, ref VkPhysicalDeviceGroupPropertiesKHX pPhysicalDeviceGroupProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDeviceGroupsKHX(VkInstance instance, IntPtr pPhysicalDeviceGroupCount, IntPtr pPhysicalDeviceGroupProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkEnumeratePhysicalDevices_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDevices(VkInstance instance, uint* pPhysicalDeviceCount, VkPhysicalDevice* pPhysicalDevices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDevices(VkInstance instance, uint* pPhysicalDeviceCount, ref VkPhysicalDevice pPhysicalDevices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDevices(VkInstance instance, uint* pPhysicalDeviceCount, IntPtr pPhysicalDevices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDevices(VkInstance instance, ref uint pPhysicalDeviceCount, VkPhysicalDevice* pPhysicalDevices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDevices(VkInstance instance, ref uint pPhysicalDeviceCount, ref VkPhysicalDevice pPhysicalDevices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDevices(VkInstance instance, ref uint pPhysicalDeviceCount, IntPtr pPhysicalDevices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDevices(VkInstance instance, IntPtr pPhysicalDeviceCount, VkPhysicalDevice* pPhysicalDevices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDevices(VkInstance instance, IntPtr pPhysicalDeviceCount, ref VkPhysicalDevice pPhysicalDevices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_INITIALIZATION_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkEnumeratePhysicalDevices(VkInstance instance, IntPtr pPhysicalDeviceCount, IntPtr pPhysicalDevices)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkFlushMappedMemoryRanges_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkFlushMappedMemoryRanges(VkDevice device, uint memoryRangeCount, VkMappedMemoryRange* pMemoryRanges)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkFlushMappedMemoryRanges(VkDevice device, uint memoryRangeCount, ref VkMappedMemoryRange pMemoryRanges)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkFlushMappedMemoryRanges(VkDevice device, uint memoryRangeCount, IntPtr pMemoryRanges)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkFreeCommandBuffers_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkFreeCommandBuffers(VkDevice device, VkCommandPool commandPool, uint commandBufferCount, VkCommandBuffer* pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkFreeCommandBuffers(VkDevice device, VkCommandPool commandPool, uint commandBufferCount, ref VkCommandBuffer pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkFreeCommandBuffers(VkDevice device, VkCommandPool commandPool, uint commandBufferCount, IntPtr pCommandBuffers)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkFreeDescriptorSets_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkFreeDescriptorSets(VkDevice device, VkDescriptorPool descriptorPool, uint descriptorSetCount, VkDescriptorSet* pDescriptorSets)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkFreeDescriptorSets(VkDevice device, VkDescriptorPool descriptorPool, uint descriptorSetCount, ref VkDescriptorSet pDescriptorSets)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkFreeDescriptorSets(VkDevice device, VkDescriptorPool descriptorPool, uint descriptorSetCount, IntPtr pDescriptorSets)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkFreeMemory_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkFreeMemory(VkDevice device, VkDeviceMemory memory, VkAllocationCallbacks* pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkFreeMemory(VkDevice device, VkDeviceMemory memory, ref VkAllocationCallbacks pAllocator)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkFreeMemory(VkDevice device, VkDeviceMemory memory, IntPtr pAllocator)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetBufferMemoryRequirements_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetBufferMemoryRequirements(VkDevice device, VkBuffer buffer, VkMemoryRequirements* pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetBufferMemoryRequirements(VkDevice device, VkBuffer buffer, out VkMemoryRequirements pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetBufferMemoryRequirements2KHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetBufferMemoryRequirements2KHR(VkDevice device, VkBufferMemoryRequirementsInfo2KHR* pInfo, VkMemoryRequirements2KHR* pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetBufferMemoryRequirements2KHR(VkDevice device, VkBufferMemoryRequirementsInfo2KHR* pInfo, out VkMemoryRequirements2KHR pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetBufferMemoryRequirements2KHR(VkDevice device, ref VkBufferMemoryRequirementsInfo2KHR pInfo, VkMemoryRequirements2KHR* pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetBufferMemoryRequirements2KHR(VkDevice device, ref VkBufferMemoryRequirementsInfo2KHR pInfo, out VkMemoryRequirements2KHR pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetBufferMemoryRequirements2KHR(VkDevice device, IntPtr pInfo, VkMemoryRequirements2KHR* pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetBufferMemoryRequirements2KHR(VkDevice device, IntPtr pInfo, out VkMemoryRequirements2KHR pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetDeviceGroupPeerMemoryFeaturesKHX_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetDeviceGroupPeerMemoryFeaturesKHX(VkDevice device, uint heapIndex, uint localDeviceIndex, uint remoteDeviceIndex, VkPeerMemoryFeatureFlagsKHX* pPeerMemoryFeatures)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetDeviceGroupPeerMemoryFeaturesKHX(VkDevice device, uint heapIndex, uint localDeviceIndex, uint remoteDeviceIndex, out VkPeerMemoryFeatureFlagsKHX pPeerMemoryFeatures)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetDeviceGroupPresentCapabilitiesKHX_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDeviceGroupPresentCapabilitiesKHX(VkDevice device, VkDeviceGroupPresentCapabilitiesKHX* pDeviceGroupPresentCapabilities)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDeviceGroupPresentCapabilitiesKHX(VkDevice device, out VkDeviceGroupPresentCapabilitiesKHX pDeviceGroupPresentCapabilities)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetDeviceGroupSurfacePresentModesKHX_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDeviceGroupSurfacePresentModesKHX(VkDevice device, VkSurfaceKHR surface, VkDeviceGroupPresentModeFlagsKHX* pModes)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDeviceGroupSurfacePresentModesKHX(VkDevice device, VkSurfaceKHR surface, out VkDeviceGroupPresentModeFlagsKHX pModes)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetDeviceMemoryCommitment_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetDeviceMemoryCommitment(VkDevice device, VkDeviceMemory memory, ulong* pCommittedMemoryInBytes)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetDeviceMemoryCommitment(VkDevice device, VkDeviceMemory memory, out ulong pCommittedMemoryInBytes)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetDeviceProcAddr_ptr;
        [Generator.CalliRewrite]
        public static unsafe IntPtr vkGetDeviceProcAddr(VkDevice device, byte* pName)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe IntPtr vkGetDeviceProcAddr(VkDevice device, out byte pName)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetDeviceQueue_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetDeviceQueue(VkDevice device, uint queueFamilyIndex, uint queueIndex, VkQueue* pQueue)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetDeviceQueue(VkDevice device, uint queueFamilyIndex, uint queueIndex, out VkQueue pQueue)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetDisplayModePropertiesKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayModePropertiesKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, uint* pPropertyCount, VkDisplayModePropertiesKHR* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayModePropertiesKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, uint* pPropertyCount, out VkDisplayModePropertiesKHR pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayModePropertiesKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, ref uint pPropertyCount, VkDisplayModePropertiesKHR* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayModePropertiesKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, ref uint pPropertyCount, out VkDisplayModePropertiesKHR pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayModePropertiesKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, IntPtr pPropertyCount, VkDisplayModePropertiesKHR* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayModePropertiesKHR(VkPhysicalDevice physicalDevice, VkDisplayKHR display, IntPtr pPropertyCount, out VkDisplayModePropertiesKHR pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetDisplayPlaneCapabilitiesKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayPlaneCapabilitiesKHR(VkPhysicalDevice physicalDevice, VkDisplayModeKHR mode, uint planeIndex, VkDisplayPlaneCapabilitiesKHR* pCapabilities)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayPlaneCapabilitiesKHR(VkPhysicalDevice physicalDevice, VkDisplayModeKHR mode, uint planeIndex, out VkDisplayPlaneCapabilitiesKHR pCapabilities)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetDisplayPlaneSupportedDisplaysKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayPlaneSupportedDisplaysKHR(VkPhysicalDevice physicalDevice, uint planeIndex, uint* pDisplayCount, VkDisplayKHR* pDisplays)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayPlaneSupportedDisplaysKHR(VkPhysicalDevice physicalDevice, uint planeIndex, uint* pDisplayCount, out VkDisplayKHR pDisplays)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayPlaneSupportedDisplaysKHR(VkPhysicalDevice physicalDevice, uint planeIndex, ref uint pDisplayCount, VkDisplayKHR* pDisplays)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayPlaneSupportedDisplaysKHR(VkPhysicalDevice physicalDevice, uint planeIndex, ref uint pDisplayCount, out VkDisplayKHR pDisplays)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayPlaneSupportedDisplaysKHR(VkPhysicalDevice physicalDevice, uint planeIndex, IntPtr pDisplayCount, VkDisplayKHR* pDisplays)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetDisplayPlaneSupportedDisplaysKHR(VkPhysicalDevice physicalDevice, uint planeIndex, IntPtr pDisplayCount, out VkDisplayKHR pDisplays)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetEventStatus_ptr;
        ///<remarks>Success codes:VK_EVENT_SET, VK_EVENT_RESET. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetEventStatus(VkDevice device, VkEvent @event)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetFenceFdKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceFdKHR(VkDevice device, VkFenceGetFdInfoKHR* pGetFdInfo, int* pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceFdKHR(VkDevice device, VkFenceGetFdInfoKHR* pGetFdInfo, out int pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceFdKHR(VkDevice device, ref VkFenceGetFdInfoKHR pGetFdInfo, int* pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceFdKHR(VkDevice device, ref VkFenceGetFdInfoKHR pGetFdInfo, out int pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceFdKHR(VkDevice device, IntPtr pGetFdInfo, int* pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceFdKHR(VkDevice device, IntPtr pGetFdInfo, out int pFd)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetFenceStatus_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_NOT_READY. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceStatus(VkDevice device, VkFence fence)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetFenceWin32HandleKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceWin32HandleKHR(VkDevice device, VkFenceGetWin32HandleInfoKHR* pGetWin32HandleInfo, Win32.HANDLE* pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceWin32HandleKHR(VkDevice device, VkFenceGetWin32HandleInfoKHR* pGetWin32HandleInfo, out Win32.HANDLE pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceWin32HandleKHR(VkDevice device, ref VkFenceGetWin32HandleInfoKHR pGetWin32HandleInfo, Win32.HANDLE* pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceWin32HandleKHR(VkDevice device, ref VkFenceGetWin32HandleInfoKHR pGetWin32HandleInfo, out Win32.HANDLE pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceWin32HandleKHR(VkDevice device, IntPtr pGetWin32HandleInfo, Win32.HANDLE* pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetFenceWin32HandleKHR(VkDevice device, IntPtr pGetWin32HandleInfo, out Win32.HANDLE pHandle)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetImageMemoryRequirements_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetImageMemoryRequirements(VkDevice device, VkImage image, VkMemoryRequirements* pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageMemoryRequirements(VkDevice device, VkImage image, out VkMemoryRequirements pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetImageMemoryRequirements2KHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetImageMemoryRequirements2KHR(VkDevice device, VkImageMemoryRequirementsInfo2KHR* pInfo, VkMemoryRequirements2KHR* pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageMemoryRequirements2KHR(VkDevice device, VkImageMemoryRequirementsInfo2KHR* pInfo, out VkMemoryRequirements2KHR pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageMemoryRequirements2KHR(VkDevice device, ref VkImageMemoryRequirementsInfo2KHR pInfo, VkMemoryRequirements2KHR* pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageMemoryRequirements2KHR(VkDevice device, ref VkImageMemoryRequirementsInfo2KHR pInfo, out VkMemoryRequirements2KHR pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageMemoryRequirements2KHR(VkDevice device, IntPtr pInfo, VkMemoryRequirements2KHR* pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageMemoryRequirements2KHR(VkDevice device, IntPtr pInfo, out VkMemoryRequirements2KHR pMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetImageSparseMemoryRequirements_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements(VkDevice device, VkImage image, uint* pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements(VkDevice device, VkImage image, uint* pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements(VkDevice device, VkImage image, ref uint pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements(VkDevice device, VkImage image, ref uint pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements(VkDevice device, VkImage image, IntPtr pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements(VkDevice device, VkImage image, IntPtr pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetImageSparseMemoryRequirements2KHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, VkImageSparseMemoryRequirementsInfo2KHR* pInfo, uint* pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements2KHR* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, VkImageSparseMemoryRequirementsInfo2KHR* pInfo, uint* pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements2KHR pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, VkImageSparseMemoryRequirementsInfo2KHR* pInfo, ref uint pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements2KHR* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, VkImageSparseMemoryRequirementsInfo2KHR* pInfo, ref uint pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements2KHR pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, VkImageSparseMemoryRequirementsInfo2KHR* pInfo, IntPtr pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements2KHR* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, VkImageSparseMemoryRequirementsInfo2KHR* pInfo, IntPtr pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements2KHR pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, ref VkImageSparseMemoryRequirementsInfo2KHR pInfo, uint* pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements2KHR* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, ref VkImageSparseMemoryRequirementsInfo2KHR pInfo, uint* pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements2KHR pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, ref VkImageSparseMemoryRequirementsInfo2KHR pInfo, ref uint pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements2KHR* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, ref VkImageSparseMemoryRequirementsInfo2KHR pInfo, ref uint pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements2KHR pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, ref VkImageSparseMemoryRequirementsInfo2KHR pInfo, IntPtr pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements2KHR* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, ref VkImageSparseMemoryRequirementsInfo2KHR pInfo, IntPtr pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements2KHR pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, IntPtr pInfo, uint* pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements2KHR* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, IntPtr pInfo, uint* pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements2KHR pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, IntPtr pInfo, ref uint pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements2KHR* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, IntPtr pInfo, ref uint pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements2KHR pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, IntPtr pInfo, IntPtr pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements2KHR* pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSparseMemoryRequirements2KHR(VkDevice device, IntPtr pInfo, IntPtr pSparseMemoryRequirementCount, out VkSparseImageMemoryRequirements2KHR pSparseMemoryRequirements)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetImageSubresourceLayout_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSubresourceLayout(VkDevice device, VkImage image, VkImageSubresource* pSubresource, VkSubresourceLayout* pLayout)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSubresourceLayout(VkDevice device, VkImage image, VkImageSubresource* pSubresource, out VkSubresourceLayout pLayout)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSubresourceLayout(VkDevice device, VkImage image, ref VkImageSubresource pSubresource, VkSubresourceLayout* pLayout)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSubresourceLayout(VkDevice device, VkImage image, ref VkImageSubresource pSubresource, out VkSubresourceLayout pLayout)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSubresourceLayout(VkDevice device, VkImage image, IntPtr pSubresource, VkSubresourceLayout* pLayout)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetImageSubresourceLayout(VkDevice device, VkImage image, IntPtr pSubresource, out VkSubresourceLayout pLayout)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetInstanceProcAddr_ptr;
        [Generator.CalliRewrite]
        public static unsafe IntPtr vkGetInstanceProcAddr(VkInstance instance, byte* pName)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe IntPtr vkGetInstanceProcAddr(VkInstance instance, out byte pName)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetMemoryFdKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryFdKHR(VkDevice device, VkMemoryGetFdInfoKHR* pGetFdInfo, int* pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryFdKHR(VkDevice device, VkMemoryGetFdInfoKHR* pGetFdInfo, out int pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryFdKHR(VkDevice device, ref VkMemoryGetFdInfoKHR pGetFdInfo, int* pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryFdKHR(VkDevice device, ref VkMemoryGetFdInfoKHR pGetFdInfo, out int pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryFdKHR(VkDevice device, IntPtr pGetFdInfo, int* pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryFdKHR(VkDevice device, IntPtr pGetFdInfo, out int pFd)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetMemoryFdPropertiesKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryFdPropertiesKHR(VkDevice device, VkExternalMemoryHandleTypeFlagsKHR handleType, int fd, VkMemoryFdPropertiesKHR* pMemoryFdProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryFdPropertiesKHR(VkDevice device, VkExternalMemoryHandleTypeFlagsKHR handleType, int fd, out VkMemoryFdPropertiesKHR pMemoryFdProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetMemoryHostPointerPropertiesEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryHostPointerPropertiesEXT(VkDevice device, VkExternalMemoryHandleTypeFlagsKHR handleType, void* pHostPointer, VkMemoryHostPointerPropertiesEXT* pMemoryHostPointerProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryHostPointerPropertiesEXT(VkDevice device, VkExternalMemoryHandleTypeFlagsKHR handleType, void* pHostPointer, out VkMemoryHostPointerPropertiesEXT pMemoryHostPointerProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetMemoryWin32HandleKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryWin32HandleKHR(VkDevice device, VkMemoryGetWin32HandleInfoKHR* pGetWin32HandleInfo, Win32.HANDLE* pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryWin32HandleKHR(VkDevice device, VkMemoryGetWin32HandleInfoKHR* pGetWin32HandleInfo, out Win32.HANDLE pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryWin32HandleKHR(VkDevice device, ref VkMemoryGetWin32HandleInfoKHR pGetWin32HandleInfo, Win32.HANDLE* pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryWin32HandleKHR(VkDevice device, ref VkMemoryGetWin32HandleInfoKHR pGetWin32HandleInfo, out Win32.HANDLE pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryWin32HandleKHR(VkDevice device, IntPtr pGetWin32HandleInfo, Win32.HANDLE* pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryWin32HandleKHR(VkDevice device, IntPtr pGetWin32HandleInfo, out Win32.HANDLE pHandle)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetMemoryWin32HandleNV_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryWin32HandleNV(VkDevice device, VkDeviceMemory memory, VkExternalMemoryHandleTypeFlagsNV handleType, Win32.HANDLE* pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryWin32HandleNV(VkDevice device, VkDeviceMemory memory, VkExternalMemoryHandleTypeFlagsNV handleType, out Win32.HANDLE pHandle)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetMemoryWin32HandlePropertiesKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryWin32HandlePropertiesKHR(VkDevice device, VkExternalMemoryHandleTypeFlagsKHR handleType, Win32.HANDLE handle, VkMemoryWin32HandlePropertiesKHR* pMemoryWin32HandleProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetMemoryWin32HandlePropertiesKHR(VkDevice device, VkExternalMemoryHandleTypeFlagsKHR handleType, Win32.HANDLE handle, out VkMemoryWin32HandlePropertiesKHR pMemoryWin32HandleProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPastPresentationTimingGOOGLE_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPastPresentationTimingGOOGLE(VkDevice device, VkSwapchainKHR swapchain, uint* pPresentationTimingCount, VkPastPresentationTimingGOOGLE* pPresentationTimings)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPastPresentationTimingGOOGLE(VkDevice device, VkSwapchainKHR swapchain, uint* pPresentationTimingCount, out VkPastPresentationTimingGOOGLE pPresentationTimings)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPastPresentationTimingGOOGLE(VkDevice device, VkSwapchainKHR swapchain, ref uint pPresentationTimingCount, VkPastPresentationTimingGOOGLE* pPresentationTimings)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPastPresentationTimingGOOGLE(VkDevice device, VkSwapchainKHR swapchain, ref uint pPresentationTimingCount, out VkPastPresentationTimingGOOGLE pPresentationTimings)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPastPresentationTimingGOOGLE(VkDevice device, VkSwapchainKHR swapchain, IntPtr pPresentationTimingCount, VkPastPresentationTimingGOOGLE* pPresentationTimings)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPastPresentationTimingGOOGLE(VkDevice device, VkSwapchainKHR swapchain, IntPtr pPresentationTimingCount, out VkPastPresentationTimingGOOGLE pPresentationTimings)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceDisplayPlanePropertiesKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPlanePropertiesKHR(VkPhysicalDevice physicalDevice, uint* pPropertyCount, VkDisplayPlanePropertiesKHR* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPlanePropertiesKHR(VkPhysicalDevice physicalDevice, uint* pPropertyCount, out VkDisplayPlanePropertiesKHR pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPlanePropertiesKHR(VkPhysicalDevice physicalDevice, ref uint pPropertyCount, VkDisplayPlanePropertiesKHR* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPlanePropertiesKHR(VkPhysicalDevice physicalDevice, ref uint pPropertyCount, out VkDisplayPlanePropertiesKHR pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPlanePropertiesKHR(VkPhysicalDevice physicalDevice, IntPtr pPropertyCount, VkDisplayPlanePropertiesKHR* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPlanePropertiesKHR(VkPhysicalDevice physicalDevice, IntPtr pPropertyCount, out VkDisplayPlanePropertiesKHR pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceDisplayPropertiesKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPropertiesKHR(VkPhysicalDevice physicalDevice, uint* pPropertyCount, VkDisplayPropertiesKHR* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPropertiesKHR(VkPhysicalDevice physicalDevice, uint* pPropertyCount, out VkDisplayPropertiesKHR pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPropertiesKHR(VkPhysicalDevice physicalDevice, ref uint pPropertyCount, VkDisplayPropertiesKHR* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPropertiesKHR(VkPhysicalDevice physicalDevice, ref uint pPropertyCount, out VkDisplayPropertiesKHR pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPropertiesKHR(VkPhysicalDevice physicalDevice, IntPtr pPropertyCount, VkDisplayPropertiesKHR* pProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceDisplayPropertiesKHR(VkPhysicalDevice physicalDevice, IntPtr pPropertyCount, out VkDisplayPropertiesKHR pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceExternalBufferPropertiesKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalBufferPropertiesKHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceExternalBufferInfoKHR* pExternalBufferInfo, VkExternalBufferPropertiesKHR* pExternalBufferProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalBufferPropertiesKHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceExternalBufferInfoKHR* pExternalBufferInfo, out VkExternalBufferPropertiesKHR pExternalBufferProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalBufferPropertiesKHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceExternalBufferInfoKHR pExternalBufferInfo, VkExternalBufferPropertiesKHR* pExternalBufferProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalBufferPropertiesKHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceExternalBufferInfoKHR pExternalBufferInfo, out VkExternalBufferPropertiesKHR pExternalBufferProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalBufferPropertiesKHR(VkPhysicalDevice physicalDevice, IntPtr pExternalBufferInfo, VkExternalBufferPropertiesKHR* pExternalBufferProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalBufferPropertiesKHR(VkPhysicalDevice physicalDevice, IntPtr pExternalBufferInfo, out VkExternalBufferPropertiesKHR pExternalBufferProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceExternalFencePropertiesKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalFencePropertiesKHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceExternalFenceInfoKHR* pExternalFenceInfo, VkExternalFencePropertiesKHR* pExternalFenceProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalFencePropertiesKHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceExternalFenceInfoKHR* pExternalFenceInfo, out VkExternalFencePropertiesKHR pExternalFenceProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalFencePropertiesKHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceExternalFenceInfoKHR pExternalFenceInfo, VkExternalFencePropertiesKHR* pExternalFenceProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalFencePropertiesKHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceExternalFenceInfoKHR pExternalFenceInfo, out VkExternalFencePropertiesKHR pExternalFenceProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalFencePropertiesKHR(VkPhysicalDevice physicalDevice, IntPtr pExternalFenceInfo, VkExternalFencePropertiesKHR* pExternalFenceProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalFencePropertiesKHR(VkPhysicalDevice physicalDevice, IntPtr pExternalFenceInfo, out VkExternalFencePropertiesKHR pExternalFenceProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceExternalImageFormatPropertiesNV_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FORMAT_NOT_SUPPORTED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceExternalImageFormatPropertiesNV(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkImageTiling tiling, VkImageUsageFlags usage, VkImageCreateFlags flags, VkExternalMemoryHandleTypeFlagsNV externalHandleType, VkExternalImageFormatPropertiesNV* pExternalImageFormatProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FORMAT_NOT_SUPPORTED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceExternalImageFormatPropertiesNV(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkImageTiling tiling, VkImageUsageFlags usage, VkImageCreateFlags flags, VkExternalMemoryHandleTypeFlagsNV externalHandleType, out VkExternalImageFormatPropertiesNV pExternalImageFormatProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceExternalSemaphorePropertiesKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalSemaphorePropertiesKHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceExternalSemaphoreInfoKHR* pExternalSemaphoreInfo, VkExternalSemaphorePropertiesKHR* pExternalSemaphoreProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalSemaphorePropertiesKHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceExternalSemaphoreInfoKHR* pExternalSemaphoreInfo, out VkExternalSemaphorePropertiesKHR pExternalSemaphoreProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalSemaphorePropertiesKHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceExternalSemaphoreInfoKHR pExternalSemaphoreInfo, VkExternalSemaphorePropertiesKHR* pExternalSemaphoreProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalSemaphorePropertiesKHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceExternalSemaphoreInfoKHR pExternalSemaphoreInfo, out VkExternalSemaphorePropertiesKHR pExternalSemaphoreProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalSemaphorePropertiesKHR(VkPhysicalDevice physicalDevice, IntPtr pExternalSemaphoreInfo, VkExternalSemaphorePropertiesKHR* pExternalSemaphoreProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceExternalSemaphorePropertiesKHR(VkPhysicalDevice physicalDevice, IntPtr pExternalSemaphoreInfo, out VkExternalSemaphorePropertiesKHR pExternalSemaphoreProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceFeatures_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceFeatures(VkPhysicalDevice physicalDevice, VkPhysicalDeviceFeatures* pFeatures)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceFeatures(VkPhysicalDevice physicalDevice, out VkPhysicalDeviceFeatures pFeatures)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceFeatures2KHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceFeatures2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceFeatures2KHR* pFeatures)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceFeatures2KHR(VkPhysicalDevice physicalDevice, out VkPhysicalDeviceFeatures2KHR pFeatures)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceFormatProperties_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkFormatProperties* pFormatProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, out VkFormatProperties pFormatProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceFormatProperties2KHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceFormatProperties2KHR(VkPhysicalDevice physicalDevice, VkFormat format, VkFormatProperties2KHR* pFormatProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceFormatProperties2KHR(VkPhysicalDevice physicalDevice, VkFormat format, out VkFormatProperties2KHR pFormatProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceGeneratedCommandsPropertiesNVX_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceGeneratedCommandsPropertiesNVX(VkPhysicalDevice physicalDevice, VkDeviceGeneratedCommandsFeaturesNVX* pFeatures, VkDeviceGeneratedCommandsLimitsNVX* pLimits)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceGeneratedCommandsPropertiesNVX(VkPhysicalDevice physicalDevice, VkDeviceGeneratedCommandsFeaturesNVX* pFeatures, out VkDeviceGeneratedCommandsLimitsNVX pLimits)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceGeneratedCommandsPropertiesNVX(VkPhysicalDevice physicalDevice, ref VkDeviceGeneratedCommandsFeaturesNVX pFeatures, VkDeviceGeneratedCommandsLimitsNVX* pLimits)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceGeneratedCommandsPropertiesNVX(VkPhysicalDevice physicalDevice, ref VkDeviceGeneratedCommandsFeaturesNVX pFeatures, out VkDeviceGeneratedCommandsLimitsNVX pLimits)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceGeneratedCommandsPropertiesNVX(VkPhysicalDevice physicalDevice, IntPtr pFeatures, VkDeviceGeneratedCommandsLimitsNVX* pLimits)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceGeneratedCommandsPropertiesNVX(VkPhysicalDevice physicalDevice, IntPtr pFeatures, out VkDeviceGeneratedCommandsLimitsNVX pLimits)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceImageFormatProperties_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FORMAT_NOT_SUPPORTED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceImageFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkImageTiling tiling, VkImageUsageFlags usage, VkImageCreateFlags flags, VkImageFormatProperties* pImageFormatProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FORMAT_NOT_SUPPORTED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceImageFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkImageTiling tiling, VkImageUsageFlags usage, VkImageCreateFlags flags, out VkImageFormatProperties pImageFormatProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceImageFormatProperties2KHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FORMAT_NOT_SUPPORTED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceImageFormatInfo2KHR* pImageFormatInfo, VkImageFormatProperties2KHR* pImageFormatProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FORMAT_NOT_SUPPORTED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceImageFormatInfo2KHR* pImageFormatInfo, out VkImageFormatProperties2KHR pImageFormatProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FORMAT_NOT_SUPPORTED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceImageFormatInfo2KHR pImageFormatInfo, VkImageFormatProperties2KHR* pImageFormatProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FORMAT_NOT_SUPPORTED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceImageFormatInfo2KHR pImageFormatInfo, out VkImageFormatProperties2KHR pImageFormatProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FORMAT_NOT_SUPPORTED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, IntPtr pImageFormatInfo, VkImageFormatProperties2KHR* pImageFormatProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_FORMAT_NOT_SUPPORTED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, IntPtr pImageFormatInfo, out VkImageFormatProperties2KHR pImageFormatProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceMemoryProperties_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceMemoryProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceMemoryProperties* pMemoryProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceMemoryProperties(VkPhysicalDevice physicalDevice, out VkPhysicalDeviceMemoryProperties pMemoryProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceMemoryProperties2KHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceMemoryProperties2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceMemoryProperties2KHR* pMemoryProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceMemoryProperties2KHR(VkPhysicalDevice physicalDevice, out VkPhysicalDeviceMemoryProperties2KHR pMemoryProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceMirPresentationSupportKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceMirPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, Mir.MirConnection* connection)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceMirPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, out Mir.MirConnection connection)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceMultisamplePropertiesEXT_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceMultisamplePropertiesEXT(VkPhysicalDevice physicalDevice, VkSampleCountFlags samples, VkMultisamplePropertiesEXT* pMultisampleProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceMultisamplePropertiesEXT(VkPhysicalDevice physicalDevice, VkSampleCountFlags samples, out VkMultisamplePropertiesEXT pMultisampleProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDevicePresentRectanglesKHX_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDevicePresentRectanglesKHX(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pRectCount, VkRect2D* pRects)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDevicePresentRectanglesKHX(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pRectCount, out VkRect2D pRects)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDevicePresentRectanglesKHX(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, ref uint pRectCount, VkRect2D* pRects)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDevicePresentRectanglesKHX(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, ref uint pRectCount, out VkRect2D pRects)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDevicePresentRectanglesKHX(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, IntPtr pRectCount, VkRect2D* pRects)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDevicePresentRectanglesKHX(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, IntPtr pRectCount, out VkRect2D pRects)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceProperties_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceProperties(VkPhysicalDevice physicalDevice, out VkPhysicalDeviceProperties pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceProperties2KHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceProperties2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceProperties2KHR* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceProperties2KHR(VkPhysicalDevice physicalDevice, out VkPhysicalDeviceProperties2KHR pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceQueueFamilyProperties_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, uint* pQueueFamilyPropertyCount, VkQueueFamilyProperties* pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, uint* pQueueFamilyPropertyCount, out VkQueueFamilyProperties pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, ref uint pQueueFamilyPropertyCount, VkQueueFamilyProperties* pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, ref uint pQueueFamilyPropertyCount, out VkQueueFamilyProperties pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, IntPtr pQueueFamilyPropertyCount, VkQueueFamilyProperties* pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, IntPtr pQueueFamilyPropertyCount, out VkQueueFamilyProperties pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceQueueFamilyProperties2KHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties2KHR(VkPhysicalDevice physicalDevice, uint* pQueueFamilyPropertyCount, VkQueueFamilyProperties2KHR* pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties2KHR(VkPhysicalDevice physicalDevice, uint* pQueueFamilyPropertyCount, out VkQueueFamilyProperties2KHR pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties2KHR(VkPhysicalDevice physicalDevice, ref uint pQueueFamilyPropertyCount, VkQueueFamilyProperties2KHR* pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties2KHR(VkPhysicalDevice physicalDevice, ref uint pQueueFamilyPropertyCount, out VkQueueFamilyProperties2KHR pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties2KHR(VkPhysicalDevice physicalDevice, IntPtr pQueueFamilyPropertyCount, VkQueueFamilyProperties2KHR* pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceQueueFamilyProperties2KHR(VkPhysicalDevice physicalDevice, IntPtr pQueueFamilyPropertyCount, out VkQueueFamilyProperties2KHR pQueueFamilyProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceSparseImageFormatProperties_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkSampleCountFlags samples, VkImageUsageFlags usage, VkImageTiling tiling, uint* pPropertyCount, VkSparseImageFormatProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkSampleCountFlags samples, VkImageUsageFlags usage, VkImageTiling tiling, uint* pPropertyCount, out VkSparseImageFormatProperties pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkSampleCountFlags samples, VkImageUsageFlags usage, VkImageTiling tiling, ref uint pPropertyCount, VkSparseImageFormatProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkSampleCountFlags samples, VkImageUsageFlags usage, VkImageTiling tiling, ref uint pPropertyCount, out VkSparseImageFormatProperties pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkSampleCountFlags samples, VkImageUsageFlags usage, VkImageTiling tiling, IntPtr pPropertyCount, VkSparseImageFormatProperties* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkSampleCountFlags samples, VkImageUsageFlags usage, VkImageTiling tiling, IntPtr pPropertyCount, out VkSparseImageFormatProperties pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceSparseImageFormatProperties2KHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSparseImageFormatInfo2KHR* pFormatInfo, uint* pPropertyCount, VkSparseImageFormatProperties2KHR* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSparseImageFormatInfo2KHR* pFormatInfo, uint* pPropertyCount, out VkSparseImageFormatProperties2KHR pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSparseImageFormatInfo2KHR* pFormatInfo, ref uint pPropertyCount, VkSparseImageFormatProperties2KHR* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSparseImageFormatInfo2KHR* pFormatInfo, ref uint pPropertyCount, out VkSparseImageFormatProperties2KHR pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSparseImageFormatInfo2KHR* pFormatInfo, IntPtr pPropertyCount, VkSparseImageFormatProperties2KHR* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSparseImageFormatInfo2KHR* pFormatInfo, IntPtr pPropertyCount, out VkSparseImageFormatProperties2KHR pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSparseImageFormatInfo2KHR pFormatInfo, uint* pPropertyCount, VkSparseImageFormatProperties2KHR* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSparseImageFormatInfo2KHR pFormatInfo, uint* pPropertyCount, out VkSparseImageFormatProperties2KHR pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSparseImageFormatInfo2KHR pFormatInfo, ref uint pPropertyCount, VkSparseImageFormatProperties2KHR* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSparseImageFormatInfo2KHR pFormatInfo, ref uint pPropertyCount, out VkSparseImageFormatProperties2KHR pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSparseImageFormatInfo2KHR pFormatInfo, IntPtr pPropertyCount, VkSparseImageFormatProperties2KHR* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSparseImageFormatInfo2KHR pFormatInfo, IntPtr pPropertyCount, out VkSparseImageFormatProperties2KHR pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, IntPtr pFormatInfo, uint* pPropertyCount, VkSparseImageFormatProperties2KHR* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, IntPtr pFormatInfo, uint* pPropertyCount, out VkSparseImageFormatProperties2KHR pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, IntPtr pFormatInfo, ref uint pPropertyCount, VkSparseImageFormatProperties2KHR* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, IntPtr pFormatInfo, ref uint pPropertyCount, out VkSparseImageFormatProperties2KHR pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, IntPtr pFormatInfo, IntPtr pPropertyCount, VkSparseImageFormatProperties2KHR* pProperties)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetPhysicalDeviceSparseImageFormatProperties2KHR(VkPhysicalDevice physicalDevice, IntPtr pFormatInfo, IntPtr pPropertyCount, out VkSparseImageFormatProperties2KHR pProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceSurfaceCapabilities2EXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceCapabilities2EXT(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, VkSurfaceCapabilities2EXT* pSurfaceCapabilities)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceCapabilities2EXT(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, out VkSurfaceCapabilities2EXT pSurfaceCapabilities)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceSurfaceCapabilities2KHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceCapabilities2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSurfaceInfo2KHR* pSurfaceInfo, VkSurfaceCapabilities2KHR* pSurfaceCapabilities)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceCapabilities2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSurfaceInfo2KHR* pSurfaceInfo, out VkSurfaceCapabilities2KHR pSurfaceCapabilities)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceCapabilities2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSurfaceInfo2KHR pSurfaceInfo, VkSurfaceCapabilities2KHR* pSurfaceCapabilities)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceCapabilities2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSurfaceInfo2KHR pSurfaceInfo, out VkSurfaceCapabilities2KHR pSurfaceCapabilities)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceCapabilities2KHR(VkPhysicalDevice physicalDevice, IntPtr pSurfaceInfo, VkSurfaceCapabilities2KHR* pSurfaceCapabilities)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceCapabilities2KHR(VkPhysicalDevice physicalDevice, IntPtr pSurfaceInfo, out VkSurfaceCapabilities2KHR pSurfaceCapabilities)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceSurfaceCapabilitiesKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceCapabilitiesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, VkSurfaceCapabilitiesKHR* pSurfaceCapabilities)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceCapabilitiesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, out VkSurfaceCapabilitiesKHR pSurfaceCapabilities)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceSurfaceFormats2KHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSurfaceInfo2KHR* pSurfaceInfo, uint* pSurfaceFormatCount, VkSurfaceFormat2KHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSurfaceInfo2KHR* pSurfaceInfo, uint* pSurfaceFormatCount, out VkSurfaceFormat2KHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSurfaceInfo2KHR* pSurfaceInfo, ref uint pSurfaceFormatCount, VkSurfaceFormat2KHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSurfaceInfo2KHR* pSurfaceInfo, ref uint pSurfaceFormatCount, out VkSurfaceFormat2KHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSurfaceInfo2KHR* pSurfaceInfo, IntPtr pSurfaceFormatCount, VkSurfaceFormat2KHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, VkPhysicalDeviceSurfaceInfo2KHR* pSurfaceInfo, IntPtr pSurfaceFormatCount, out VkSurfaceFormat2KHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSurfaceInfo2KHR pSurfaceInfo, uint* pSurfaceFormatCount, VkSurfaceFormat2KHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSurfaceInfo2KHR pSurfaceInfo, uint* pSurfaceFormatCount, out VkSurfaceFormat2KHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSurfaceInfo2KHR pSurfaceInfo, ref uint pSurfaceFormatCount, VkSurfaceFormat2KHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSurfaceInfo2KHR pSurfaceInfo, ref uint pSurfaceFormatCount, out VkSurfaceFormat2KHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSurfaceInfo2KHR pSurfaceInfo, IntPtr pSurfaceFormatCount, VkSurfaceFormat2KHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, ref VkPhysicalDeviceSurfaceInfo2KHR pSurfaceInfo, IntPtr pSurfaceFormatCount, out VkSurfaceFormat2KHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, IntPtr pSurfaceInfo, uint* pSurfaceFormatCount, VkSurfaceFormat2KHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, IntPtr pSurfaceInfo, uint* pSurfaceFormatCount, out VkSurfaceFormat2KHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, IntPtr pSurfaceInfo, ref uint pSurfaceFormatCount, VkSurfaceFormat2KHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, IntPtr pSurfaceInfo, ref uint pSurfaceFormatCount, out VkSurfaceFormat2KHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, IntPtr pSurfaceInfo, IntPtr pSurfaceFormatCount, VkSurfaceFormat2KHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormats2KHR(VkPhysicalDevice physicalDevice, IntPtr pSurfaceInfo, IntPtr pSurfaceFormatCount, out VkSurfaceFormat2KHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceSurfaceFormatsKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormatsKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pSurfaceFormatCount, VkSurfaceFormatKHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormatsKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pSurfaceFormatCount, out VkSurfaceFormatKHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormatsKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, ref uint pSurfaceFormatCount, VkSurfaceFormatKHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormatsKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, ref uint pSurfaceFormatCount, out VkSurfaceFormatKHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormatsKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, IntPtr pSurfaceFormatCount, VkSurfaceFormatKHR* pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceFormatsKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, IntPtr pSurfaceFormatCount, out VkSurfaceFormatKHR pSurfaceFormats)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceSurfacePresentModesKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfacePresentModesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pPresentModeCount, VkPresentModeKHR* pPresentModes)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfacePresentModesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, uint* pPresentModeCount, out VkPresentModeKHR pPresentModes)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfacePresentModesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, ref uint pPresentModeCount, VkPresentModeKHR* pPresentModes)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfacePresentModesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, ref uint pPresentModeCount, out VkPresentModeKHR pPresentModes)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfacePresentModesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, IntPtr pPresentModeCount, VkPresentModeKHR* pPresentModes)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfacePresentModesKHR(VkPhysicalDevice physicalDevice, VkSurfaceKHR surface, IntPtr pPresentModeCount, out VkPresentModeKHR pPresentModes)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceSurfaceSupportKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, VkSurfaceKHR surface, VkBool32* pSupported)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPhysicalDeviceSurfaceSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, VkSurfaceKHR surface, out VkBool32 pSupported)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceWaylandPresentationSupportKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceWaylandPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, Wayland.wl_display* display)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceWaylandPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, out Wayland.wl_display display)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceWin32PresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceXcbPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, Xcb.xcb_connection_t* connection, Xcb.xcb_visualid_t visual_id)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceXcbPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, ref Xcb.xcb_connection_t connection, Xcb.xcb_visualid_t visual_id)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceXcbPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr connection, Xcb.xcb_visualid_t visual_id)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceXlibPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, Xlib.Display* dpy, Xlib.VisualID visualID)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceXlibPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, ref Xlib.Display dpy, Xlib.VisualID visualID)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkBool32 vkGetPhysicalDeviceXlibPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr dpy, Xlib.VisualID visualID)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetPipelineCacheData_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPipelineCacheData(VkDevice device, VkPipelineCache pipelineCache, UIntPtr* pDataSize, void* pData)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPipelineCacheData(VkDevice device, VkPipelineCache pipelineCache, ref UIntPtr pDataSize, void* pData)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetPipelineCacheData(VkDevice device, VkPipelineCache pipelineCache, IntPtr pDataSize, void* pData)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetQueryPoolResults_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_NOT_READY. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetQueryPoolResults(VkDevice device, VkQueryPool queryPool, uint firstQuery, uint queryCount, UIntPtr dataSize, void* pData, ulong stride, VkQueryResultFlags flags)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetRandROutputDisplayEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetRandROutputDisplayEXT(VkPhysicalDevice physicalDevice, Xlib.Display* dpy, IntPtr rrOutput, VkDisplayKHR* pDisplay)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetRandROutputDisplayEXT(VkPhysicalDevice physicalDevice, Xlib.Display* dpy, IntPtr rrOutput, out VkDisplayKHR pDisplay)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetRandROutputDisplayEXT(VkPhysicalDevice physicalDevice, ref Xlib.Display dpy, IntPtr rrOutput, VkDisplayKHR* pDisplay)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetRandROutputDisplayEXT(VkPhysicalDevice physicalDevice, ref Xlib.Display dpy, IntPtr rrOutput, out VkDisplayKHR pDisplay)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetRandROutputDisplayEXT(VkPhysicalDevice physicalDevice, IntPtr dpy, IntPtr rrOutput, VkDisplayKHR* pDisplay)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetRandROutputDisplayEXT(VkPhysicalDevice physicalDevice, IntPtr dpy, IntPtr rrOutput, out VkDisplayKHR pDisplay)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetRefreshCycleDurationGOOGLE_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetRefreshCycleDurationGOOGLE(VkDevice device, VkSwapchainKHR swapchain, VkRefreshCycleDurationGOOGLE* pDisplayTimingProperties)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_DEVICE_LOST, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetRefreshCycleDurationGOOGLE(VkDevice device, VkSwapchainKHR swapchain, out VkRefreshCycleDurationGOOGLE pDisplayTimingProperties)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetRenderAreaGranularity_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkGetRenderAreaGranularity(VkDevice device, VkRenderPass renderPass, VkExtent2D* pGranularity)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkGetRenderAreaGranularity(VkDevice device, VkRenderPass renderPass, out VkExtent2D pGranularity)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetSemaphoreFdKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreFdKHR(VkDevice device, VkSemaphoreGetFdInfoKHR* pGetFdInfo, int* pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreFdKHR(VkDevice device, VkSemaphoreGetFdInfoKHR* pGetFdInfo, out int pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreFdKHR(VkDevice device, ref VkSemaphoreGetFdInfoKHR pGetFdInfo, int* pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreFdKHR(VkDevice device, ref VkSemaphoreGetFdInfoKHR pGetFdInfo, out int pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreFdKHR(VkDevice device, IntPtr pGetFdInfo, int* pFd)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreFdKHR(VkDevice device, IntPtr pGetFdInfo, out int pFd)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetSemaphoreWin32HandleKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreWin32HandleKHR(VkDevice device, VkSemaphoreGetWin32HandleInfoKHR* pGetWin32HandleInfo, Win32.HANDLE* pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreWin32HandleKHR(VkDevice device, VkSemaphoreGetWin32HandleInfoKHR* pGetWin32HandleInfo, out Win32.HANDLE pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreWin32HandleKHR(VkDevice device, ref VkSemaphoreGetWin32HandleInfoKHR pGetWin32HandleInfo, Win32.HANDLE* pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreWin32HandleKHR(VkDevice device, ref VkSemaphoreGetWin32HandleInfoKHR pGetWin32HandleInfo, out Win32.HANDLE pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreWin32HandleKHR(VkDevice device, IntPtr pGetWin32HandleInfo, Win32.HANDLE* pHandle)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_TOO_MANY_OBJECTS, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSemaphoreWin32HandleKHR(VkDevice device, IntPtr pGetWin32HandleInfo, out Win32.HANDLE pHandle)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetShaderInfoAMD_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetShaderInfoAMD(VkDevice device, VkPipeline pipeline, VkShaderStageFlags shaderStage, VkShaderInfoTypeAMD infoType, UIntPtr* pInfoSize, void* pInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetShaderInfoAMD(VkDevice device, VkPipeline pipeline, VkShaderStageFlags shaderStage, VkShaderInfoTypeAMD infoType, ref UIntPtr pInfoSize, void* pInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_FEATURE_NOT_PRESENT, VK_ERROR_OUT_OF_HOST_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetShaderInfoAMD(VkDevice device, VkPipeline pipeline, VkShaderStageFlags shaderStage, VkShaderInfoTypeAMD infoType, IntPtr pInfoSize, void* pInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetSwapchainCounterEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainCounterEXT(VkDevice device, VkSwapchainKHR swapchain, VkSurfaceCounterFlagsEXT counter, ulong* pCounterValue)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainCounterEXT(VkDevice device, VkSwapchainKHR swapchain, VkSurfaceCounterFlagsEXT counter, out ulong pCounterValue)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetSwapchainGrallocUsageANDROID_ptr;
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainGrallocUsageANDROID(VkDevice device, VkFormat format, VkImageUsageFlags imageUsage, int* grallocUsage)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainGrallocUsageANDROID(VkDevice device, VkFormat format, VkImageUsageFlags imageUsage, out int grallocUsage)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetSwapchainImagesKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainImagesKHR(VkDevice device, VkSwapchainKHR swapchain, uint* pSwapchainImageCount, VkImage* pSwapchainImages)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainImagesKHR(VkDevice device, VkSwapchainKHR swapchain, uint* pSwapchainImageCount, out VkImage pSwapchainImages)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainImagesKHR(VkDevice device, VkSwapchainKHR swapchain, ref uint pSwapchainImageCount, VkImage* pSwapchainImages)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainImagesKHR(VkDevice device, VkSwapchainKHR swapchain, ref uint pSwapchainImageCount, out VkImage pSwapchainImages)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainImagesKHR(VkDevice device, VkSwapchainKHR swapchain, IntPtr pSwapchainImageCount, VkImage* pSwapchainImages)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainImagesKHR(VkDevice device, VkSwapchainKHR swapchain, IntPtr pSwapchainImageCount, out VkImage pSwapchainImages)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetSwapchainStatusKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetSwapchainStatusKHR(VkDevice device, VkSwapchainKHR swapchain)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkGetValidationCacheDataEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetValidationCacheDataEXT(VkDevice device, VkValidationCacheEXT validationCache, UIntPtr* pDataSize, void* pData)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetValidationCacheDataEXT(VkDevice device, VkValidationCacheEXT validationCache, ref UIntPtr pDataSize, void* pData)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_INCOMPLETE. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkGetValidationCacheDataEXT(VkDevice device, VkValidationCacheEXT validationCache, IntPtr pDataSize, void* pData)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkImportFenceFdKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportFenceFdKHR(VkDevice device, VkImportFenceFdInfoKHR* pImportFenceFdInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportFenceFdKHR(VkDevice device, ref VkImportFenceFdInfoKHR pImportFenceFdInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportFenceFdKHR(VkDevice device, IntPtr pImportFenceFdInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkImportFenceWin32HandleKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportFenceWin32HandleKHR(VkDevice device, VkImportFenceWin32HandleInfoKHR* pImportFenceWin32HandleInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportFenceWin32HandleKHR(VkDevice device, ref VkImportFenceWin32HandleInfoKHR pImportFenceWin32HandleInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportFenceWin32HandleKHR(VkDevice device, IntPtr pImportFenceWin32HandleInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkImportSemaphoreFdKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportSemaphoreFdKHR(VkDevice device, VkImportSemaphoreFdInfoKHR* pImportSemaphoreFdInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportSemaphoreFdKHR(VkDevice device, ref VkImportSemaphoreFdInfoKHR pImportSemaphoreFdInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportSemaphoreFdKHR(VkDevice device, IntPtr pImportSemaphoreFdInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkImportSemaphoreWin32HandleKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportSemaphoreWin32HandleKHR(VkDevice device, VkImportSemaphoreWin32HandleInfoKHR* pImportSemaphoreWin32HandleInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportSemaphoreWin32HandleKHR(VkDevice device, ref VkImportSemaphoreWin32HandleInfoKHR pImportSemaphoreWin32HandleInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkImportSemaphoreWin32HandleKHR(VkDevice device, IntPtr pImportSemaphoreWin32HandleInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkInvalidateMappedMemoryRanges_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkInvalidateMappedMemoryRanges(VkDevice device, uint memoryRangeCount, VkMappedMemoryRange* pMemoryRanges)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkInvalidateMappedMemoryRanges(VkDevice device, uint memoryRangeCount, ref VkMappedMemoryRange pMemoryRanges)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkInvalidateMappedMemoryRanges(VkDevice device, uint memoryRangeCount, IntPtr pMemoryRanges)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkMapMemory_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_MEMORY_MAP_FAILED</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkMapMemory(VkDevice device, VkDeviceMemory memory, ulong offset, ulong size, uint flags, void** ppData)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkMergePipelineCaches_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkMergePipelineCaches(VkDevice device, VkPipelineCache dstCache, uint srcCacheCount, VkPipelineCache* pSrcCaches)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkMergePipelineCaches(VkDevice device, VkPipelineCache dstCache, uint srcCacheCount, ref VkPipelineCache pSrcCaches)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkMergePipelineCaches(VkDevice device, VkPipelineCache dstCache, uint srcCacheCount, IntPtr pSrcCaches)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkMergeValidationCachesEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkMergeValidationCachesEXT(VkDevice device, VkValidationCacheEXT dstCache, uint srcCacheCount, VkValidationCacheEXT* pSrcCaches)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkMergeValidationCachesEXT(VkDevice device, VkValidationCacheEXT dstCache, uint srcCacheCount, ref VkValidationCacheEXT pSrcCaches)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkMergeValidationCachesEXT(VkDevice device, VkValidationCacheEXT dstCache, uint srcCacheCount, IntPtr pSrcCaches)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkQueueBindSparse_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueBindSparse(VkQueue queue, uint bindInfoCount, VkBindSparseInfo* pBindInfo, VkFence fence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueBindSparse(VkQueue queue, uint bindInfoCount, ref VkBindSparseInfo pBindInfo, VkFence fence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueBindSparse(VkQueue queue, uint bindInfoCount, IntPtr pBindInfo, VkFence fence)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkQueuePresentKHR_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueuePresentKHR(VkQueue queue, VkPresentInfoKHR* pPresentInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueuePresentKHR(VkQueue queue, ref VkPresentInfoKHR pPresentInfo)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_SUBOPTIMAL_KHR. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST, VK_ERROR_OUT_OF_DATE_KHR, VK_ERROR_SURFACE_LOST_KHR</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueuePresentKHR(VkQueue queue, IntPtr pPresentInfo)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkQueueSignalReleaseImageANDROID_ptr;
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSignalReleaseImageANDROID(VkQueue queue, uint waitSemaphoreCount, VkSemaphore* pWaitSemaphores, VkImage image, int* pNativeFenceFd)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSignalReleaseImageANDROID(VkQueue queue, uint waitSemaphoreCount, VkSemaphore* pWaitSemaphores, VkImage image, ref int pNativeFenceFd)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSignalReleaseImageANDROID(VkQueue queue, uint waitSemaphoreCount, VkSemaphore* pWaitSemaphores, VkImage image, IntPtr pNativeFenceFd)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSignalReleaseImageANDROID(VkQueue queue, uint waitSemaphoreCount, ref VkSemaphore pWaitSemaphores, VkImage image, int* pNativeFenceFd)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSignalReleaseImageANDROID(VkQueue queue, uint waitSemaphoreCount, ref VkSemaphore pWaitSemaphores, VkImage image, ref int pNativeFenceFd)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSignalReleaseImageANDROID(VkQueue queue, uint waitSemaphoreCount, ref VkSemaphore pWaitSemaphores, VkImage image, IntPtr pNativeFenceFd)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSignalReleaseImageANDROID(VkQueue queue, uint waitSemaphoreCount, IntPtr pWaitSemaphores, VkImage image, int* pNativeFenceFd)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSignalReleaseImageANDROID(VkQueue queue, uint waitSemaphoreCount, IntPtr pWaitSemaphores, VkImage image, ref int pNativeFenceFd)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSignalReleaseImageANDROID(VkQueue queue, uint waitSemaphoreCount, IntPtr pWaitSemaphores, VkImage image, IntPtr pNativeFenceFd)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkQueueSubmit_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSubmit(VkQueue queue, uint submitCount, VkSubmitInfo* pSubmits, VkFence fence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSubmit(VkQueue queue, uint submitCount, ref VkSubmitInfo pSubmits, VkFence fence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueSubmit(VkQueue queue, uint submitCount, IntPtr pSubmits, VkFence fence)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkQueueWaitIdle_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkQueueWaitIdle(VkQueue queue)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkRegisterDeviceEventEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, VkDeviceEventInfoEXT* pDeviceEventInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, VkDeviceEventInfoEXT* pDeviceEventInfo, VkAllocationCallbacks* pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, VkDeviceEventInfoEXT* pDeviceEventInfo, VkAllocationCallbacks* pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, VkDeviceEventInfoEXT* pDeviceEventInfo, ref VkAllocationCallbacks pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, VkDeviceEventInfoEXT* pDeviceEventInfo, ref VkAllocationCallbacks pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, VkDeviceEventInfoEXT* pDeviceEventInfo, ref VkAllocationCallbacks pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, VkDeviceEventInfoEXT* pDeviceEventInfo, IntPtr pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, VkDeviceEventInfoEXT* pDeviceEventInfo, IntPtr pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, VkDeviceEventInfoEXT* pDeviceEventInfo, IntPtr pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, ref VkDeviceEventInfoEXT pDeviceEventInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, ref VkDeviceEventInfoEXT pDeviceEventInfo, VkAllocationCallbacks* pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, ref VkDeviceEventInfoEXT pDeviceEventInfo, VkAllocationCallbacks* pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, ref VkDeviceEventInfoEXT pDeviceEventInfo, ref VkAllocationCallbacks pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, ref VkDeviceEventInfoEXT pDeviceEventInfo, ref VkAllocationCallbacks pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, ref VkDeviceEventInfoEXT pDeviceEventInfo, ref VkAllocationCallbacks pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, ref VkDeviceEventInfoEXT pDeviceEventInfo, IntPtr pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, ref VkDeviceEventInfoEXT pDeviceEventInfo, IntPtr pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, ref VkDeviceEventInfoEXT pDeviceEventInfo, IntPtr pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, IntPtr pDeviceEventInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, IntPtr pDeviceEventInfo, VkAllocationCallbacks* pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, IntPtr pDeviceEventInfo, VkAllocationCallbacks* pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, IntPtr pDeviceEventInfo, ref VkAllocationCallbacks pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, IntPtr pDeviceEventInfo, ref VkAllocationCallbacks pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, IntPtr pDeviceEventInfo, ref VkAllocationCallbacks pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, IntPtr pDeviceEventInfo, IntPtr pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, IntPtr pDeviceEventInfo, IntPtr pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDeviceEventEXT(VkDevice device, IntPtr pDeviceEventInfo, IntPtr pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkRegisterDisplayEventEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, VkDisplayEventInfoEXT* pDisplayEventInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, VkDisplayEventInfoEXT* pDisplayEventInfo, VkAllocationCallbacks* pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, VkDisplayEventInfoEXT* pDisplayEventInfo, VkAllocationCallbacks* pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, VkDisplayEventInfoEXT* pDisplayEventInfo, ref VkAllocationCallbacks pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, VkDisplayEventInfoEXT* pDisplayEventInfo, ref VkAllocationCallbacks pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, VkDisplayEventInfoEXT* pDisplayEventInfo, ref VkAllocationCallbacks pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, VkDisplayEventInfoEXT* pDisplayEventInfo, IntPtr pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, VkDisplayEventInfoEXT* pDisplayEventInfo, IntPtr pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, VkDisplayEventInfoEXT* pDisplayEventInfo, IntPtr pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, ref VkDisplayEventInfoEXT pDisplayEventInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, ref VkDisplayEventInfoEXT pDisplayEventInfo, VkAllocationCallbacks* pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, ref VkDisplayEventInfoEXT pDisplayEventInfo, VkAllocationCallbacks* pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, ref VkDisplayEventInfoEXT pDisplayEventInfo, ref VkAllocationCallbacks pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, ref VkDisplayEventInfoEXT pDisplayEventInfo, ref VkAllocationCallbacks pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, ref VkDisplayEventInfoEXT pDisplayEventInfo, ref VkAllocationCallbacks pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, ref VkDisplayEventInfoEXT pDisplayEventInfo, IntPtr pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, ref VkDisplayEventInfoEXT pDisplayEventInfo, IntPtr pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, ref VkDisplayEventInfoEXT pDisplayEventInfo, IntPtr pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, IntPtr pDisplayEventInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, IntPtr pDisplayEventInfo, VkAllocationCallbacks* pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, IntPtr pDisplayEventInfo, VkAllocationCallbacks* pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, IntPtr pDisplayEventInfo, ref VkAllocationCallbacks pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, IntPtr pDisplayEventInfo, ref VkAllocationCallbacks pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, IntPtr pDisplayEventInfo, ref VkAllocationCallbacks pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, IntPtr pDisplayEventInfo, IntPtr pAllocator, VkFence* pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, IntPtr pDisplayEventInfo, IntPtr pAllocator, ref VkFence pFence)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterDisplayEventEXT(VkDevice device, VkDisplayKHR display, IntPtr pDisplayEventInfo, IntPtr pAllocator, IntPtr pFence)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkRegisterObjectsNVX_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, VkObjectTableEntryNVX** ppObjectTableEntries, uint* pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, VkObjectTableEntryNVX** ppObjectTableEntries, ref uint pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, VkObjectTableEntryNVX** ppObjectTableEntries, IntPtr pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, ref VkObjectTableEntryNVX* ppObjectTableEntries, uint* pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, ref VkObjectTableEntryNVX* ppObjectTableEntries, ref uint pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, ref VkObjectTableEntryNVX* ppObjectTableEntries, IntPtr pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, IntPtr ppObjectTableEntries, uint* pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, IntPtr ppObjectTableEntries, ref uint pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkRegisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, IntPtr ppObjectTableEntries, IntPtr pObjectIndices)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkReleaseDisplayEXT_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkReleaseDisplayEXT(VkPhysicalDevice physicalDevice, VkDisplayKHR display)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkResetCommandBuffer_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkResetCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferResetFlags flags)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkResetCommandPool_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkResetCommandPool(VkDevice device, VkCommandPool commandPool, VkCommandPoolResetFlags flags)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkResetDescriptorPool_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkResetDescriptorPool(VkDevice device, VkDescriptorPool descriptorPool, uint flags)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkResetEvent_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkResetEvent(VkDevice device, VkEvent @event)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkResetFences_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkResetFences(VkDevice device, uint fenceCount, VkFence* pFences)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkResetFences(VkDevice device, uint fenceCount, ref VkFence pFences)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkResetFences(VkDevice device, uint fenceCount, IntPtr pFences)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkSetEvent_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkSetEvent(VkDevice device, VkEvent @event)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkSetHdrMetadataEXT_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkSetHdrMetadataEXT(VkDevice device, uint swapchainCount, VkSwapchainKHR* pSwapchains, VkHdrMetadataEXT* pMetadata)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkSetHdrMetadataEXT(VkDevice device, uint swapchainCount, VkSwapchainKHR* pSwapchains, ref VkHdrMetadataEXT pMetadata)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkSetHdrMetadataEXT(VkDevice device, uint swapchainCount, VkSwapchainKHR* pSwapchains, IntPtr pMetadata)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkSetHdrMetadataEXT(VkDevice device, uint swapchainCount, ref VkSwapchainKHR pSwapchains, VkHdrMetadataEXT* pMetadata)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkSetHdrMetadataEXT(VkDevice device, uint swapchainCount, ref VkSwapchainKHR pSwapchains, ref VkHdrMetadataEXT pMetadata)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkSetHdrMetadataEXT(VkDevice device, uint swapchainCount, ref VkSwapchainKHR pSwapchains, IntPtr pMetadata)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkSetHdrMetadataEXT(VkDevice device, uint swapchainCount, IntPtr pSwapchains, VkHdrMetadataEXT* pMetadata)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkSetHdrMetadataEXT(VkDevice device, uint swapchainCount, IntPtr pSwapchains, ref VkHdrMetadataEXT pMetadata)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkSetHdrMetadataEXT(VkDevice device, uint swapchainCount, IntPtr pSwapchains, IntPtr pMetadata)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkTrimCommandPoolKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkTrimCommandPoolKHR(VkDevice device, VkCommandPool commandPool, uint flags)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkUnmapMemory_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkUnmapMemory(VkDevice device, VkDeviceMemory memory)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkUnregisterObjectsNVX_ptr;
        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkUnregisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, VkObjectEntryTypeNVX* pObjectEntryTypes, uint* pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkUnregisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, VkObjectEntryTypeNVX* pObjectEntryTypes, ref uint pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkUnregisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, VkObjectEntryTypeNVX* pObjectEntryTypes, IntPtr pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkUnregisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, ref VkObjectEntryTypeNVX pObjectEntryTypes, uint* pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkUnregisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, ref VkObjectEntryTypeNVX pObjectEntryTypes, ref uint pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkUnregisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, ref VkObjectEntryTypeNVX pObjectEntryTypes, IntPtr pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkUnregisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, IntPtr pObjectEntryTypes, uint* pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkUnregisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, IntPtr pObjectEntryTypes, ref uint pObjectIndices)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkUnregisterObjectsNVX(VkDevice device, VkObjectTableNVX objectTable, uint objectCount, IntPtr pObjectEntryTypes, IntPtr pObjectIndices)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkUpdateDescriptorSets_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, VkWriteDescriptorSet* pDescriptorWrites, uint descriptorCopyCount, VkCopyDescriptorSet* pDescriptorCopies)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, VkWriteDescriptorSet* pDescriptorWrites, uint descriptorCopyCount, ref VkCopyDescriptorSet pDescriptorCopies)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, VkWriteDescriptorSet* pDescriptorWrites, uint descriptorCopyCount, IntPtr pDescriptorCopies)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, ref VkWriteDescriptorSet pDescriptorWrites, uint descriptorCopyCount, VkCopyDescriptorSet* pDescriptorCopies)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, ref VkWriteDescriptorSet pDescriptorWrites, uint descriptorCopyCount, ref VkCopyDescriptorSet pDescriptorCopies)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, ref VkWriteDescriptorSet pDescriptorWrites, uint descriptorCopyCount, IntPtr pDescriptorCopies)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, IntPtr pDescriptorWrites, uint descriptorCopyCount, VkCopyDescriptorSet* pDescriptorCopies)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, IntPtr pDescriptorWrites, uint descriptorCopyCount, ref VkCopyDescriptorSet pDescriptorCopies)
        {
            throw new NotImplementedException();
        }

        [Generator.CalliRewrite]
        public static unsafe void vkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, IntPtr pDescriptorWrites, uint descriptorCopyCount, IntPtr pDescriptorCopies)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkUpdateDescriptorSetWithTemplateKHR_ptr;
        [Generator.CalliRewrite]
        public static unsafe void vkUpdateDescriptorSetWithTemplateKHR(VkDevice device, VkDescriptorSet descriptorSet, VkDescriptorUpdateTemplateKHR descriptorUpdateTemplate, void* pData)
        {
            throw new NotImplementedException();
        }

        private static IntPtr vkWaitForFences_ptr;
        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkWaitForFences(VkDevice device, uint fenceCount, VkFence* pFences, VkBool32 waitAll, ulong timeout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkWaitForFences(VkDevice device, uint fenceCount, ref VkFence pFences, VkBool32 waitAll, ulong timeout)
        {
            throw new NotImplementedException();
        }

        ///<remarks>Success codes:VK_SUCCESS, VK_TIMEOUT. Error codes:VK_ERROR_OUT_OF_HOST_MEMORY, VK_ERROR_OUT_OF_DEVICE_MEMORY, VK_ERROR_DEVICE_LOST</remarks>
        [Generator.CalliRewrite]
        public static unsafe VkResult vkWaitForFences(VkDevice device, uint fenceCount, IntPtr pFences, VkBool32 waitAll, ulong timeout)
        {
            throw new NotImplementedException();
        }

        private static void LoadFunctionPointers()
        {
            vkCreateInstance_ptr = s_nativeLib.LoadFunctionPointer("vkCreateInstance");
            vkDestroyInstance_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyInstance");
            vkEnumeratePhysicalDevices_ptr = s_nativeLib.LoadFunctionPointer("vkEnumeratePhysicalDevices");
            vkGetDeviceProcAddr_ptr = s_nativeLib.LoadFunctionPointer("vkGetDeviceProcAddr");
            vkGetInstanceProcAddr_ptr = s_nativeLib.LoadFunctionPointer("vkGetInstanceProcAddr");
            vkGetPhysicalDeviceProperties_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceProperties");
            vkGetPhysicalDeviceQueueFamilyProperties_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceQueueFamilyProperties");
            vkGetPhysicalDeviceMemoryProperties_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceMemoryProperties");
            vkGetPhysicalDeviceFeatures_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceFeatures");
            vkGetPhysicalDeviceFormatProperties_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceFormatProperties");
            vkGetPhysicalDeviceImageFormatProperties_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceImageFormatProperties");
            vkCreateDevice_ptr = s_nativeLib.LoadFunctionPointer("vkCreateDevice");
            vkDestroyDevice_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyDevice");
            vkEnumerateInstanceLayerProperties_ptr = s_nativeLib.LoadFunctionPointer("vkEnumerateInstanceLayerProperties");
            vkEnumerateInstanceExtensionProperties_ptr = s_nativeLib.LoadFunctionPointer("vkEnumerateInstanceExtensionProperties");
            vkEnumerateDeviceLayerProperties_ptr = s_nativeLib.LoadFunctionPointer("vkEnumerateDeviceLayerProperties");
            vkEnumerateDeviceExtensionProperties_ptr = s_nativeLib.LoadFunctionPointer("vkEnumerateDeviceExtensionProperties");
            vkGetDeviceQueue_ptr = s_nativeLib.LoadFunctionPointer("vkGetDeviceQueue");
            vkQueueSubmit_ptr = s_nativeLib.LoadFunctionPointer("vkQueueSubmit");
            vkQueueWaitIdle_ptr = s_nativeLib.LoadFunctionPointer("vkQueueWaitIdle");
            vkDeviceWaitIdle_ptr = s_nativeLib.LoadFunctionPointer("vkDeviceWaitIdle");
            vkAllocateMemory_ptr = s_nativeLib.LoadFunctionPointer("vkAllocateMemory");
            vkFreeMemory_ptr = s_nativeLib.LoadFunctionPointer("vkFreeMemory");
            vkMapMemory_ptr = s_nativeLib.LoadFunctionPointer("vkMapMemory");
            vkUnmapMemory_ptr = s_nativeLib.LoadFunctionPointer("vkUnmapMemory");
            vkFlushMappedMemoryRanges_ptr = s_nativeLib.LoadFunctionPointer("vkFlushMappedMemoryRanges");
            vkInvalidateMappedMemoryRanges_ptr = s_nativeLib.LoadFunctionPointer("vkInvalidateMappedMemoryRanges");
            vkGetDeviceMemoryCommitment_ptr = s_nativeLib.LoadFunctionPointer("vkGetDeviceMemoryCommitment");
            vkGetBufferMemoryRequirements_ptr = s_nativeLib.LoadFunctionPointer("vkGetBufferMemoryRequirements");
            vkBindBufferMemory_ptr = s_nativeLib.LoadFunctionPointer("vkBindBufferMemory");
            vkGetImageMemoryRequirements_ptr = s_nativeLib.LoadFunctionPointer("vkGetImageMemoryRequirements");
            vkBindImageMemory_ptr = s_nativeLib.LoadFunctionPointer("vkBindImageMemory");
            vkGetImageSparseMemoryRequirements_ptr = s_nativeLib.LoadFunctionPointer("vkGetImageSparseMemoryRequirements");
            vkGetPhysicalDeviceSparseImageFormatProperties_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceSparseImageFormatProperties");
            vkQueueBindSparse_ptr = s_nativeLib.LoadFunctionPointer("vkQueueBindSparse");
            vkCreateFence_ptr = s_nativeLib.LoadFunctionPointer("vkCreateFence");
            vkDestroyFence_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyFence");
            vkResetFences_ptr = s_nativeLib.LoadFunctionPointer("vkResetFences");
            vkGetFenceStatus_ptr = s_nativeLib.LoadFunctionPointer("vkGetFenceStatus");
            vkWaitForFences_ptr = s_nativeLib.LoadFunctionPointer("vkWaitForFences");
            vkCreateSemaphore_ptr = s_nativeLib.LoadFunctionPointer("vkCreateSemaphore");
            vkDestroySemaphore_ptr = s_nativeLib.LoadFunctionPointer("vkDestroySemaphore");
            vkCreateEvent_ptr = s_nativeLib.LoadFunctionPointer("vkCreateEvent");
            vkDestroyEvent_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyEvent");
            vkGetEventStatus_ptr = s_nativeLib.LoadFunctionPointer("vkGetEventStatus");
            vkSetEvent_ptr = s_nativeLib.LoadFunctionPointer("vkSetEvent");
            vkResetEvent_ptr = s_nativeLib.LoadFunctionPointer("vkResetEvent");
            vkCreateQueryPool_ptr = s_nativeLib.LoadFunctionPointer("vkCreateQueryPool");
            vkDestroyQueryPool_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyQueryPool");
            vkGetQueryPoolResults_ptr = s_nativeLib.LoadFunctionPointer("vkGetQueryPoolResults");
            vkCreateBuffer_ptr = s_nativeLib.LoadFunctionPointer("vkCreateBuffer");
            vkDestroyBuffer_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyBuffer");
            vkCreateBufferView_ptr = s_nativeLib.LoadFunctionPointer("vkCreateBufferView");
            vkDestroyBufferView_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyBufferView");
            vkCreateImage_ptr = s_nativeLib.LoadFunctionPointer("vkCreateImage");
            vkDestroyImage_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyImage");
            vkGetImageSubresourceLayout_ptr = s_nativeLib.LoadFunctionPointer("vkGetImageSubresourceLayout");
            vkCreateImageView_ptr = s_nativeLib.LoadFunctionPointer("vkCreateImageView");
            vkDestroyImageView_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyImageView");
            vkCreateShaderModule_ptr = s_nativeLib.LoadFunctionPointer("vkCreateShaderModule");
            vkDestroyShaderModule_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyShaderModule");
            vkCreatePipelineCache_ptr = s_nativeLib.LoadFunctionPointer("vkCreatePipelineCache");
            vkDestroyPipelineCache_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyPipelineCache");
            vkGetPipelineCacheData_ptr = s_nativeLib.LoadFunctionPointer("vkGetPipelineCacheData");
            vkMergePipelineCaches_ptr = s_nativeLib.LoadFunctionPointer("vkMergePipelineCaches");
            vkCreateGraphicsPipelines_ptr = s_nativeLib.LoadFunctionPointer("vkCreateGraphicsPipelines");
            vkCreateComputePipelines_ptr = s_nativeLib.LoadFunctionPointer("vkCreateComputePipelines");
            vkDestroyPipeline_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyPipeline");
            vkCreatePipelineLayout_ptr = s_nativeLib.LoadFunctionPointer("vkCreatePipelineLayout");
            vkDestroyPipelineLayout_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyPipelineLayout");
            vkCreateSampler_ptr = s_nativeLib.LoadFunctionPointer("vkCreateSampler");
            vkDestroySampler_ptr = s_nativeLib.LoadFunctionPointer("vkDestroySampler");
            vkCreateDescriptorSetLayout_ptr = s_nativeLib.LoadFunctionPointer("vkCreateDescriptorSetLayout");
            vkDestroyDescriptorSetLayout_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyDescriptorSetLayout");
            vkCreateDescriptorPool_ptr = s_nativeLib.LoadFunctionPointer("vkCreateDescriptorPool");
            vkDestroyDescriptorPool_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyDescriptorPool");
            vkResetDescriptorPool_ptr = s_nativeLib.LoadFunctionPointer("vkResetDescriptorPool");
            vkAllocateDescriptorSets_ptr = s_nativeLib.LoadFunctionPointer("vkAllocateDescriptorSets");
            vkFreeDescriptorSets_ptr = s_nativeLib.LoadFunctionPointer("vkFreeDescriptorSets");
            vkUpdateDescriptorSets_ptr = s_nativeLib.LoadFunctionPointer("vkUpdateDescriptorSets");
            vkCreateFramebuffer_ptr = s_nativeLib.LoadFunctionPointer("vkCreateFramebuffer");
            vkDestroyFramebuffer_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyFramebuffer");
            vkCreateRenderPass_ptr = s_nativeLib.LoadFunctionPointer("vkCreateRenderPass");
            vkDestroyRenderPass_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyRenderPass");
            vkGetRenderAreaGranularity_ptr = s_nativeLib.LoadFunctionPointer("vkGetRenderAreaGranularity");
            vkCreateCommandPool_ptr = s_nativeLib.LoadFunctionPointer("vkCreateCommandPool");
            vkDestroyCommandPool_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyCommandPool");
            vkResetCommandPool_ptr = s_nativeLib.LoadFunctionPointer("vkResetCommandPool");
            vkAllocateCommandBuffers_ptr = s_nativeLib.LoadFunctionPointer("vkAllocateCommandBuffers");
            vkFreeCommandBuffers_ptr = s_nativeLib.LoadFunctionPointer("vkFreeCommandBuffers");
            vkBeginCommandBuffer_ptr = s_nativeLib.LoadFunctionPointer("vkBeginCommandBuffer");
            vkEndCommandBuffer_ptr = s_nativeLib.LoadFunctionPointer("vkEndCommandBuffer");
            vkResetCommandBuffer_ptr = s_nativeLib.LoadFunctionPointer("vkResetCommandBuffer");
            vkCmdBindPipeline_ptr = s_nativeLib.LoadFunctionPointer("vkCmdBindPipeline");
            vkCmdSetViewport_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetViewport");
            vkCmdSetScissor_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetScissor");
            vkCmdSetLineWidth_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetLineWidth");
            vkCmdSetDepthBias_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetDepthBias");
            vkCmdSetBlendConstants_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetBlendConstants");
            vkCmdSetDepthBounds_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetDepthBounds");
            vkCmdSetStencilCompareMask_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetStencilCompareMask");
            vkCmdSetStencilWriteMask_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetStencilWriteMask");
            vkCmdSetStencilReference_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetStencilReference");
            vkCmdBindDescriptorSets_ptr = s_nativeLib.LoadFunctionPointer("vkCmdBindDescriptorSets");
            vkCmdBindIndexBuffer_ptr = s_nativeLib.LoadFunctionPointer("vkCmdBindIndexBuffer");
            vkCmdBindVertexBuffers_ptr = s_nativeLib.LoadFunctionPointer("vkCmdBindVertexBuffers");
            vkCmdDraw_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDraw");
            vkCmdDrawIndexed_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDrawIndexed");
            vkCmdDrawIndirect_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDrawIndirect");
            vkCmdDrawIndexedIndirect_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDrawIndexedIndirect");
            vkCmdDispatch_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDispatch");
            vkCmdDispatchIndirect_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDispatchIndirect");
            vkCmdCopyBuffer_ptr = s_nativeLib.LoadFunctionPointer("vkCmdCopyBuffer");
            vkCmdCopyImage_ptr = s_nativeLib.LoadFunctionPointer("vkCmdCopyImage");
            vkCmdBlitImage_ptr = s_nativeLib.LoadFunctionPointer("vkCmdBlitImage");
            vkCmdCopyBufferToImage_ptr = s_nativeLib.LoadFunctionPointer("vkCmdCopyBufferToImage");
            vkCmdCopyImageToBuffer_ptr = s_nativeLib.LoadFunctionPointer("vkCmdCopyImageToBuffer");
            vkCmdUpdateBuffer_ptr = s_nativeLib.LoadFunctionPointer("vkCmdUpdateBuffer");
            vkCmdFillBuffer_ptr = s_nativeLib.LoadFunctionPointer("vkCmdFillBuffer");
            vkCmdClearColorImage_ptr = s_nativeLib.LoadFunctionPointer("vkCmdClearColorImage");
            vkCmdClearDepthStencilImage_ptr = s_nativeLib.LoadFunctionPointer("vkCmdClearDepthStencilImage");
            vkCmdClearAttachments_ptr = s_nativeLib.LoadFunctionPointer("vkCmdClearAttachments");
            vkCmdResolveImage_ptr = s_nativeLib.LoadFunctionPointer("vkCmdResolveImage");
            vkCmdSetEvent_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetEvent");
            vkCmdResetEvent_ptr = s_nativeLib.LoadFunctionPointer("vkCmdResetEvent");
            vkCmdWaitEvents_ptr = s_nativeLib.LoadFunctionPointer("vkCmdWaitEvents");
            vkCmdPipelineBarrier_ptr = s_nativeLib.LoadFunctionPointer("vkCmdPipelineBarrier");
            vkCmdBeginQuery_ptr = s_nativeLib.LoadFunctionPointer("vkCmdBeginQuery");
            vkCmdEndQuery_ptr = s_nativeLib.LoadFunctionPointer("vkCmdEndQuery");
            vkCmdResetQueryPool_ptr = s_nativeLib.LoadFunctionPointer("vkCmdResetQueryPool");
            vkCmdWriteTimestamp_ptr = s_nativeLib.LoadFunctionPointer("vkCmdWriteTimestamp");
            vkCmdCopyQueryPoolResults_ptr = s_nativeLib.LoadFunctionPointer("vkCmdCopyQueryPoolResults");
            vkCmdPushConstants_ptr = s_nativeLib.LoadFunctionPointer("vkCmdPushConstants");
            vkCmdBeginRenderPass_ptr = s_nativeLib.LoadFunctionPointer("vkCmdBeginRenderPass");
            vkCmdNextSubpass_ptr = s_nativeLib.LoadFunctionPointer("vkCmdNextSubpass");
            vkCmdEndRenderPass_ptr = s_nativeLib.LoadFunctionPointer("vkCmdEndRenderPass");
            vkCmdExecuteCommands_ptr = s_nativeLib.LoadFunctionPointer("vkCmdExecuteCommands");
            vkCreateAndroidSurfaceKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateAndroidSurfaceKHR");
            vkGetPhysicalDeviceDisplayPropertiesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceDisplayPropertiesKHR");
            vkGetPhysicalDeviceDisplayPlanePropertiesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceDisplayPlanePropertiesKHR");
            vkGetDisplayPlaneSupportedDisplaysKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetDisplayPlaneSupportedDisplaysKHR");
            vkGetDisplayModePropertiesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetDisplayModePropertiesKHR");
            vkCreateDisplayModeKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateDisplayModeKHR");
            vkGetDisplayPlaneCapabilitiesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetDisplayPlaneCapabilitiesKHR");
            vkCreateDisplayPlaneSurfaceKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateDisplayPlaneSurfaceKHR");
            vkCreateSharedSwapchainsKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateSharedSwapchainsKHR");
            vkCreateMirSurfaceKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateMirSurfaceKHR");
            vkGetPhysicalDeviceMirPresentationSupportKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceMirPresentationSupportKHR");
            vkDestroySurfaceKHR_ptr = s_nativeLib.LoadFunctionPointer("vkDestroySurfaceKHR");
            vkGetPhysicalDeviceSurfaceSupportKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceSurfaceSupportKHR");
            vkGetPhysicalDeviceSurfaceCapabilitiesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
            vkGetPhysicalDeviceSurfaceFormatsKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceSurfaceFormatsKHR");
            vkGetPhysicalDeviceSurfacePresentModesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceSurfacePresentModesKHR");
            vkCreateSwapchainKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateSwapchainKHR");
            vkDestroySwapchainKHR_ptr = s_nativeLib.LoadFunctionPointer("vkDestroySwapchainKHR");
            vkGetSwapchainImagesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetSwapchainImagesKHR");
            vkAcquireNextImageKHR_ptr = s_nativeLib.LoadFunctionPointer("vkAcquireNextImageKHR");
            vkQueuePresentKHR_ptr = s_nativeLib.LoadFunctionPointer("vkQueuePresentKHR");
            vkCreateViSurfaceNN_ptr = s_nativeLib.LoadFunctionPointer("vkCreateViSurfaceNN");
            vkCreateWaylandSurfaceKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateWaylandSurfaceKHR");
            vkGetPhysicalDeviceWaylandPresentationSupportKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceWaylandPresentationSupportKHR");
            vkCreateWin32SurfaceKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateWin32SurfaceKHR");
            vkGetPhysicalDeviceWin32PresentationSupportKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceWin32PresentationSupportKHR");
            vkCreateXlibSurfaceKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateXlibSurfaceKHR");
            vkGetPhysicalDeviceXlibPresentationSupportKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceXlibPresentationSupportKHR");
            vkCreateXcbSurfaceKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateXcbSurfaceKHR");
            vkGetPhysicalDeviceXcbPresentationSupportKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceXcbPresentationSupportKHR");
            vkCreateDebugReportCallbackEXT_ptr = s_nativeLib.LoadFunctionPointer("vkCreateDebugReportCallbackEXT");
            vkDestroyDebugReportCallbackEXT_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyDebugReportCallbackEXT");
            vkDebugReportMessageEXT_ptr = s_nativeLib.LoadFunctionPointer("vkDebugReportMessageEXT");
            vkDebugMarkerSetObjectNameEXT_ptr = s_nativeLib.LoadFunctionPointer("vkDebugMarkerSetObjectNameEXT");
            vkDebugMarkerSetObjectTagEXT_ptr = s_nativeLib.LoadFunctionPointer("vkDebugMarkerSetObjectTagEXT");
            vkCmdDebugMarkerBeginEXT_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDebugMarkerBeginEXT");
            vkCmdDebugMarkerEndEXT_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDebugMarkerEndEXT");
            vkCmdDebugMarkerInsertEXT_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDebugMarkerInsertEXT");
            vkGetPhysicalDeviceExternalImageFormatPropertiesNV_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceExternalImageFormatPropertiesNV");
            vkGetMemoryWin32HandleNV_ptr = s_nativeLib.LoadFunctionPointer("vkGetMemoryWin32HandleNV");
            vkCmdDrawIndirectCountAMD_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDrawIndirectCountAMD");
            vkCmdDrawIndexedIndirectCountAMD_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDrawIndexedIndirectCountAMD");
            vkCmdProcessCommandsNVX_ptr = s_nativeLib.LoadFunctionPointer("vkCmdProcessCommandsNVX");
            vkCmdReserveSpaceForCommandsNVX_ptr = s_nativeLib.LoadFunctionPointer("vkCmdReserveSpaceForCommandsNVX");
            vkCreateIndirectCommandsLayoutNVX_ptr = s_nativeLib.LoadFunctionPointer("vkCreateIndirectCommandsLayoutNVX");
            vkDestroyIndirectCommandsLayoutNVX_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyIndirectCommandsLayoutNVX");
            vkCreateObjectTableNVX_ptr = s_nativeLib.LoadFunctionPointer("vkCreateObjectTableNVX");
            vkDestroyObjectTableNVX_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyObjectTableNVX");
            vkRegisterObjectsNVX_ptr = s_nativeLib.LoadFunctionPointer("vkRegisterObjectsNVX");
            vkUnregisterObjectsNVX_ptr = s_nativeLib.LoadFunctionPointer("vkUnregisterObjectsNVX");
            vkGetPhysicalDeviceGeneratedCommandsPropertiesNVX_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceGeneratedCommandsPropertiesNVX");
            vkGetPhysicalDeviceFeatures2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceFeatures2KHR");
            vkGetPhysicalDeviceProperties2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceProperties2KHR");
            vkGetPhysicalDeviceFormatProperties2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceFormatProperties2KHR");
            vkGetPhysicalDeviceImageFormatProperties2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceImageFormatProperties2KHR");
            vkGetPhysicalDeviceQueueFamilyProperties2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceQueueFamilyProperties2KHR");
            vkGetPhysicalDeviceMemoryProperties2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceMemoryProperties2KHR");
            vkGetPhysicalDeviceSparseImageFormatProperties2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceSparseImageFormatProperties2KHR");
            vkCmdPushDescriptorSetKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCmdPushDescriptorSetKHR");
            vkTrimCommandPoolKHR_ptr = s_nativeLib.LoadFunctionPointer("vkTrimCommandPoolKHR");
            vkGetPhysicalDeviceExternalBufferPropertiesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceExternalBufferPropertiesKHR");
            vkGetMemoryWin32HandleKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetMemoryWin32HandleKHR");
            vkGetMemoryWin32HandlePropertiesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetMemoryWin32HandlePropertiesKHR");
            vkGetMemoryFdKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetMemoryFdKHR");
            vkGetMemoryFdPropertiesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetMemoryFdPropertiesKHR");
            vkGetPhysicalDeviceExternalSemaphorePropertiesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceExternalSemaphorePropertiesKHR");
            vkGetSemaphoreWin32HandleKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetSemaphoreWin32HandleKHR");
            vkImportSemaphoreWin32HandleKHR_ptr = s_nativeLib.LoadFunctionPointer("vkImportSemaphoreWin32HandleKHR");
            vkGetSemaphoreFdKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetSemaphoreFdKHR");
            vkImportSemaphoreFdKHR_ptr = s_nativeLib.LoadFunctionPointer("vkImportSemaphoreFdKHR");
            vkGetPhysicalDeviceExternalFencePropertiesKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceExternalFencePropertiesKHR");
            vkGetFenceWin32HandleKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetFenceWin32HandleKHR");
            vkImportFenceWin32HandleKHR_ptr = s_nativeLib.LoadFunctionPointer("vkImportFenceWin32HandleKHR");
            vkGetFenceFdKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetFenceFdKHR");
            vkImportFenceFdKHR_ptr = s_nativeLib.LoadFunctionPointer("vkImportFenceFdKHR");
            vkReleaseDisplayEXT_ptr = s_nativeLib.LoadFunctionPointer("vkReleaseDisplayEXT");
            vkAcquireXlibDisplayEXT_ptr = s_nativeLib.LoadFunctionPointer("vkAcquireXlibDisplayEXT");
            vkGetRandROutputDisplayEXT_ptr = s_nativeLib.LoadFunctionPointer("vkGetRandROutputDisplayEXT");
            vkDisplayPowerControlEXT_ptr = s_nativeLib.LoadFunctionPointer("vkDisplayPowerControlEXT");
            vkRegisterDeviceEventEXT_ptr = s_nativeLib.LoadFunctionPointer("vkRegisterDeviceEventEXT");
            vkRegisterDisplayEventEXT_ptr = s_nativeLib.LoadFunctionPointer("vkRegisterDisplayEventEXT");
            vkGetSwapchainCounterEXT_ptr = s_nativeLib.LoadFunctionPointer("vkGetSwapchainCounterEXT");
            vkGetPhysicalDeviceSurfaceCapabilities2EXT_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceSurfaceCapabilities2EXT");
            vkEnumeratePhysicalDeviceGroupsKHX_ptr = s_nativeLib.LoadFunctionPointer("vkEnumeratePhysicalDeviceGroupsKHX");
            vkGetDeviceGroupPeerMemoryFeaturesKHX_ptr = s_nativeLib.LoadFunctionPointer("vkGetDeviceGroupPeerMemoryFeaturesKHX");
            vkBindBufferMemory2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkBindBufferMemory2KHR");
            vkBindImageMemory2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkBindImageMemory2KHR");
            vkCmdSetDeviceMaskKHX_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetDeviceMaskKHX");
            vkGetDeviceGroupPresentCapabilitiesKHX_ptr = s_nativeLib.LoadFunctionPointer("vkGetDeviceGroupPresentCapabilitiesKHX");
            vkGetDeviceGroupSurfacePresentModesKHX_ptr = s_nativeLib.LoadFunctionPointer("vkGetDeviceGroupSurfacePresentModesKHX");
            vkAcquireNextImage2KHX_ptr = s_nativeLib.LoadFunctionPointer("vkAcquireNextImage2KHX");
            vkCmdDispatchBaseKHX_ptr = s_nativeLib.LoadFunctionPointer("vkCmdDispatchBaseKHX");
            vkGetPhysicalDevicePresentRectanglesKHX_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDevicePresentRectanglesKHX");
            vkCreateDescriptorUpdateTemplateKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateDescriptorUpdateTemplateKHR");
            vkDestroyDescriptorUpdateTemplateKHR_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyDescriptorUpdateTemplateKHR");
            vkUpdateDescriptorSetWithTemplateKHR_ptr = s_nativeLib.LoadFunctionPointer("vkUpdateDescriptorSetWithTemplateKHR");
            vkCmdPushDescriptorSetWithTemplateKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCmdPushDescriptorSetWithTemplateKHR");
            vkSetHdrMetadataEXT_ptr = s_nativeLib.LoadFunctionPointer("vkSetHdrMetadataEXT");
            vkGetSwapchainStatusKHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetSwapchainStatusKHR");
            vkGetRefreshCycleDurationGOOGLE_ptr = s_nativeLib.LoadFunctionPointer("vkGetRefreshCycleDurationGOOGLE");
            vkGetPastPresentationTimingGOOGLE_ptr = s_nativeLib.LoadFunctionPointer("vkGetPastPresentationTimingGOOGLE");
            vkCreateIOSSurfaceMVK_ptr = s_nativeLib.LoadFunctionPointer("vkCreateIOSSurfaceMVK");
            vkCreateMacOSSurfaceMVK_ptr = s_nativeLib.LoadFunctionPointer("vkCreateMacOSSurfaceMVK");
            vkCmdSetViewportWScalingNV_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetViewportWScalingNV");
            vkCmdSetDiscardRectangleEXT_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetDiscardRectangleEXT");
            vkCmdSetSampleLocationsEXT_ptr = s_nativeLib.LoadFunctionPointer("vkCmdSetSampleLocationsEXT");
            vkGetPhysicalDeviceMultisamplePropertiesEXT_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceMultisamplePropertiesEXT");
            vkGetPhysicalDeviceSurfaceCapabilities2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceSurfaceCapabilities2KHR");
            vkGetPhysicalDeviceSurfaceFormats2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetPhysicalDeviceSurfaceFormats2KHR");
            vkGetBufferMemoryRequirements2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetBufferMemoryRequirements2KHR");
            vkGetImageMemoryRequirements2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetImageMemoryRequirements2KHR");
            vkGetImageSparseMemoryRequirements2KHR_ptr = s_nativeLib.LoadFunctionPointer("vkGetImageSparseMemoryRequirements2KHR");
            vkCreateSamplerYcbcrConversionKHR_ptr = s_nativeLib.LoadFunctionPointer("vkCreateSamplerYcbcrConversionKHR");
            vkDestroySamplerYcbcrConversionKHR_ptr = s_nativeLib.LoadFunctionPointer("vkDestroySamplerYcbcrConversionKHR");
            vkCreateValidationCacheEXT_ptr = s_nativeLib.LoadFunctionPointer("vkCreateValidationCacheEXT");
            vkDestroyValidationCacheEXT_ptr = s_nativeLib.LoadFunctionPointer("vkDestroyValidationCacheEXT");
            vkGetValidationCacheDataEXT_ptr = s_nativeLib.LoadFunctionPointer("vkGetValidationCacheDataEXT");
            vkMergeValidationCachesEXT_ptr = s_nativeLib.LoadFunctionPointer("vkMergeValidationCachesEXT");
            vkGetSwapchainGrallocUsageANDROID_ptr = s_nativeLib.LoadFunctionPointer("vkGetSwapchainGrallocUsageANDROID");
            vkAcquireImageANDROID_ptr = s_nativeLib.LoadFunctionPointer("vkAcquireImageANDROID");
            vkQueueSignalReleaseImageANDROID_ptr = s_nativeLib.LoadFunctionPointer("vkQueueSignalReleaseImageANDROID");
            vkGetShaderInfoAMD_ptr = s_nativeLib.LoadFunctionPointer("vkGetShaderInfoAMD");
            vkGetMemoryHostPointerPropertiesEXT_ptr = s_nativeLib.LoadFunctionPointer("vkGetMemoryHostPointerPropertiesEXT");
        }
    }
}
