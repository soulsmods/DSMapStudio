namespace Vulkan
{
    public unsafe partial struct VkImageSubresourceRange
    {
        public VkImageSubresourceRange(
            VkImageAspectFlags aspectMask,
            uint baseMipLevel = 0, uint levelCount = 1,
            uint baseArrayLayer = 0, uint layerCount = 1)
        {
            this.aspectMask = aspectMask;
            this.baseMipLevel = baseMipLevel;
            this.levelCount = levelCount;
            this.baseArrayLayer = baseArrayLayer;
            this.layerCount = layerCount;
        }
    }
}
