using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Veldrid.Vk
{
    internal unsafe class VkFence : Fence
    {
        private readonly VkGraphicsDevice _gd;
        private Vortice.Vulkan.VkFence _fence;
        private string _name;
        private bool _destroyed;

        public Vortice.Vulkan.VkFence DeviceFence => _fence;

        public VkFence(VkGraphicsDevice gd, bool signaled)
        {
            _gd = gd;
            var fenceCI = new VkFenceCreateInfo
            {
                sType = VkStructureType.FenceCreateInfo,
                flags = signaled ? VkFenceCreateFlags.Signaled : VkFenceCreateFlags.None
            };
            VkResult result = vkCreateFence(_gd.Device, &fenceCI, null, out _fence);
            VulkanUtil.CheckResult(result);
        }

        public override void Reset()
        {
            _gd.ResetFence(this);
        }

        public override bool Signaled => vkGetFenceStatus(_gd.Device, _fence) == VkResult.Success;

        public override string Name
        {
            get => _name;
            set
            {
                _name = value; _gd.SetResourceName(this, value);
            }
        }

        public override void Dispose()
        {
            if (!_destroyed)
            {
                vkDestroyFence(_gd.Device, _fence, null);
                _destroyed = true;
            }
        }
    }
}
