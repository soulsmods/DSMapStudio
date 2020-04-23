using System;

namespace Vulkan
{
    public unsafe delegate void* PFN_vkAllocationFunction(
         void* pUserData,
         UIntPtr size,
         UIntPtr alignment,
         VkSystemAllocationScope allocationScope);

    public unsafe delegate void* PFN_vkReallocationFunction(
         void* pUserData,
         void* pOriginal,
         UIntPtr size,
         UIntPtr alignment,
         VkSystemAllocationScope allocationScope);

    public unsafe delegate void PFN_vkFreeFunction(
         void* pUserData,
         void* pMemory);

    public unsafe delegate void PFN_vkInternalAllocationNotification(
         void* pUserData,
         UIntPtr size,
         VkInternalAllocationType allocationType,
         VkSystemAllocationScope allocationScope);

    public unsafe delegate void PFN_vkInternalFreeNotification(
         void* pUserData,
         UIntPtr size,
         VkInternalAllocationType allocationType,
         VkSystemAllocationScope allocationScope);

    public unsafe delegate void PFN_vkVoidFunction();

    public unsafe delegate uint PFN_vkDebugReportCallbackEXT(
        uint flags,
        VkDebugReportObjectTypeEXT objectType,
        ulong @object,
        UIntPtr location,
        int messageCode,
        byte* pLayerPrefix,
        byte* pMessage,
        void* pUserData);
}
