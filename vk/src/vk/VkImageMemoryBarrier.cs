namespace Vulkan
{
    public unsafe partial struct VkImageMemoryBarrier
    {
        public VkImageMemoryBarrier(
            VkImage image, 
            VkImageSubresourceRange subresourceRange,
            VkAccessFlags srcAccessMask,
            VkAccessFlags dstAccessMask, 
            VkImageLayout oldLayout,
            VkImageLayout newLayout,
            uint srcQueueFamilyIndex = VulkanNative.QueueFamilyIgnored, 
            uint dstQueueFamilyIndex = VulkanNative.QueueFamilyIgnored)
        {
            sType = VkStructureType.ImageMemoryBarrier;
            pNext = null;
            this.srcAccessMask = srcAccessMask;
            this.dstAccessMask = dstAccessMask;
            this.oldLayout = oldLayout;
            this.newLayout = newLayout;
            this.srcQueueFamilyIndex = srcQueueFamilyIndex;
            this.dstQueueFamilyIndex = dstQueueFamilyIndex;
            this.image = image;
            this.subresourceRange = subresourceRange;
        }
    }
}
