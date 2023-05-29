using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Veldrid.Vk.VulkanUtil;

namespace Veldrid.Vk
{
    internal unsafe class VkResourceLayout : ResourceLayout
    {
        private readonly VkGraphicsDevice _gd;
        private readonly VkDescriptorSetLayout _dsl;
        private readonly VkDescriptorType[] _descriptorTypes;
        private bool _disposed;
        private string _name;

        public VkDescriptorSetLayout DescriptorSetLayout => _dsl;
        public VkDescriptorType[] DescriptorTypes => _descriptorTypes;
        public DescriptorResourceCounts DescriptorResourceCounts { get; }
        public new int DynamicBufferCount { get; }

        public VkResourceLayout(VkGraphicsDevice gd, ref ResourceLayoutDescription description)
            : base(ref description)
        {
            _gd = gd;
            ResourceLayoutElementDescription[] elements = description.Elements;
            _descriptorTypes = new VkDescriptorType[elements.Length];
            VkDescriptorSetLayoutBinding* bindings = stackalloc VkDescriptorSetLayoutBinding[elements.Length];
            VkDescriptorBindingFlags* flags = stackalloc VkDescriptorBindingFlags[elements.Length];

            uint uniformBufferCount = 0;
            uint sampledImageCount = 0;
            uint samplerCount = 0;
            uint storageBufferCount = 0;
            uint storageImageCount = 0;

            for (uint i = 0; i < elements.Length; i++)
            {
                VkDescriptorType descriptorType = VkFormats.VdToVkDescriptorType(elements[i].Kind, elements[i].Options);
                bindings[i] = new VkDescriptorSetLayoutBinding
                {
                    binding = i,
                    descriptorCount = elements[i].DescriptorCount,
                    descriptorType = descriptorType,
                    stageFlags = VkFormats.VdToVkShaderStages(elements[i].Stages)
                };
                if ((elements[i].Options & ResourceLayoutElementOptions.DynamicBinding) != 0)
                {
                    DynamicBufferCount += 1;
                }

                _descriptorTypes[i] = descriptorType;

                switch (descriptorType)
                {
                    case VkDescriptorType.Sampler:
                        samplerCount += elements[i].DescriptorCount;
                        break;
                    case VkDescriptorType.SampledImage:
                        sampledImageCount += elements[i].DescriptorCount;
                        break;
                    case VkDescriptorType.StorageImage:
                        storageImageCount += elements[i].DescriptorCount;
                        break;
                    case VkDescriptorType.UniformBuffer:
                        uniformBufferCount += elements[i].DescriptorCount;
                        break;
                    case VkDescriptorType.StorageBuffer:
                        storageBufferCount += elements[i].DescriptorCount;
                        break;
                }

                flags[i] = new VkDescriptorBindingFlags();
                if ((elements[i].Options & ResourceLayoutElementOptions.VariableCount) != 0)
                {
                    flags[i] = VkDescriptorBindingFlags.VariableDescriptorCount;
                    // UpdateAfterBind is needed for larger texture pools on Intel for some reason
                    if (descriptorType == VkDescriptorType.SampledImage)
                        flags[i] |= VkDescriptorBindingFlags.UpdateAfterBind;
                }
            }

            DescriptorResourceCounts = new DescriptorResourceCounts(
                uniformBufferCount,
                sampledImageCount,
                samplerCount,
                storageBufferCount,
                storageImageCount);

            var bindingFlagsCI = new VkDescriptorSetLayoutBindingFlagsCreateInfo
            {
                sType = VkStructureType.DescriptorSetLayoutBindingFlagsCreateInfo,
                bindingCount = (uint)elements.Length,
                pBindingFlags = flags
            };
            
            var dslCI = new VkDescriptorSetLayoutCreateInfo
            {
                sType = VkStructureType.DescriptorSetLayoutCreateInfo,
                pNext = &bindingFlagsCI,
                flags = VkDescriptorSetLayoutCreateFlags.UpdateAfterBindPool,
                bindingCount = (uint)elements.Length,
                pBindings = bindings
            };
            VkResult result = vkCreateDescriptorSetLayout(_gd.Device, &dslCI, null, out _dsl);
            CheckResult(result);
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
            if (!_disposed)
            {
                _disposed = true;
                vkDestroyDescriptorSetLayout(_gd.Device, _dsl, null);
            }
        }
    }
}
