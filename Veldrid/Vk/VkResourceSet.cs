using System.Collections.Generic;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Veldrid.Vk.VulkanUtil;

namespace Veldrid.Vk
{
    internal unsafe class VkResourceSet : ResourceSet
    {
        private readonly VkGraphicsDevice _gd;
        private readonly DescriptorResourceCounts _descriptorCounts;
        private readonly DescriptorAllocationToken _descriptorAllocationToken;
        private readonly List<ResourceRefCount> _refCounts = new List<ResourceRefCount>();
        private bool _destroyed;
        private string _name;

        public VkDescriptorSet DescriptorSet => _descriptorAllocationToken.Set;

        private readonly List<VkTexture> _sampledTextures = new List<VkTexture>();
        public IReadOnlyList<VkTexture> SampledTextures => _sampledTextures;
        private readonly List<VkTexture> _storageImages = new List<VkTexture>();
        public IReadOnlyList<VkTexture> StorageTextures => _storageImages;

        public ResourceRefCount RefCount { get; }
        public IReadOnlyList<ResourceRefCount> RefCounts => _refCounts;

        public VkResourceSet(VkGraphicsDevice gd, ref ResourceSetDescription description)
            : base(ref description)
        {
            // TODO: There's a lot of hacks done in here to "support" unbounded arrays/descriptor indexing. It
            // needs to be reworked eventually to get rid of them.
            _gd = gd;
            RefCount = new ResourceRefCount(DisposeCore);
            VkResourceLayout vkLayout = Util.AssertSubtype<ResourceLayout, VkResourceLayout>(description.Layout);

            BindableResource[] boundResources = description.BoundResources;
            int descriptorWriteCount = vkLayout.Description.Elements.Length;

            uint desccount = 0;
            foreach (var e in vkLayout.Description.Elements)
            {
                desccount += e.DescriptorCount;
            }
            
            bool variableCount =
                (vkLayout.Description.Elements[^1].Options & ResourceLayoutElementOptions.VariableCount) != 0;
            VkDescriptorSetLayout dsl = vkLayout.DescriptorSetLayout;
            _descriptorCounts = vkLayout.DescriptorResourceCounts;
            _descriptorAllocationToken =
                _gd.DescriptorPoolManager.Allocate(_descriptorCounts, dsl, variableCount ? desccount : 0);

            VkWriteDescriptorSet* descriptorWrites = stackalloc VkWriteDescriptorSet[(int)descriptorWriteCount];
            VkDescriptorBufferInfo* bufferInfos = stackalloc VkDescriptorBufferInfo[(int)descriptorWriteCount];
            VkDescriptorImageInfo* imageInfos = stackalloc VkDescriptorImageInfo[(int)desccount];

            int boundr = 0;
            for (int i = 0; i < descriptorWriteCount; i++)
            {
                VkDescriptorType type = vkLayout.DescriptorTypes[i];
                descriptorWrites[i] = new VkWriteDescriptorSet
                {
                    sType = VkStructureType.WriteDescriptorSet,
                    descriptorCount = 1,
                    descriptorType = type,
                    dstBinding = (uint)i,
                    dstSet = _descriptorAllocationToken.Set
                };

                if (type == VkDescriptorType.UniformBuffer || type == VkDescriptorType.UniformBufferDynamic
                                                           || type == VkDescriptorType.StorageBuffer || type == VkDescriptorType.StorageBufferDynamic)
                {
                    DeviceBufferRange range = Util.GetBufferRange(boundResources[boundr], 0);
                    VkBuffer rangedVkBuffer = Util.AssertSubtype<DeviceBuffer, VkBuffer>(range.Buffer);
                    bufferInfos[i] = new VkDescriptorBufferInfo
                    {
                        buffer = rangedVkBuffer.DeviceBuffer,
                        offset = range.Offset,
                        range = range.SizeInBytes
                    };
                    descriptorWrites[i].pBufferInfo = &bufferInfos[i];
                    _refCounts.Add(rangedVkBuffer.RefCount);
                }
                else if (type == VkDescriptorType.SampledImage)
                {
                    descriptorWrites[i].pImageInfo = &imageInfos[boundr];
                    descriptorWrites[i].descriptorCount = vkLayout.Description.Elements[i].DescriptorCount;
                    for (int j = 0; j < vkLayout.Description.Elements[i].DescriptorCount; j++)
                    {
                        TextureView texView = Util.GetTextureView(_gd, boundResources[boundr]);
                        VkTextureView vkTexView = Util.AssertSubtype<TextureView, VkTextureView>(texView);
                        imageInfos[boundr].imageView = vkTexView.ImageView;
                        imageInfos[boundr].imageLayout = VkImageLayout.ShaderReadOnlyOptimal;
                        _sampledTextures.Add(Util.AssertSubtype<Texture, VkTexture>(texView.Target));
                        _refCounts.Add(vkTexView.RefCount);
                        boundr++;
                    }
                }
                else if (type == VkDescriptorType.StorageImage)
                {
                    TextureView texView = Util.GetTextureView(_gd, boundResources[boundr]);
                    VkTextureView vkTexView = Util.AssertSubtype<TextureView, VkTextureView>(texView);
                    imageInfos[i].imageView = vkTexView.ImageView;
                    imageInfos[i].imageLayout = VkImageLayout.General;
                    descriptorWrites[i].pImageInfo = &imageInfos[i];
                    _storageImages.Add(Util.AssertSubtype<Texture, VkTexture>(texView.Target));
                    _refCounts.Add(vkTexView.RefCount);
                }
                else if (type == VkDescriptorType.Sampler)
                {
                    VkSampler sampler = Util.AssertSubtype<BindableResource, VkSampler>(boundResources[boundr]);
                    imageInfos[i].sampler = sampler.DeviceSampler;
                    descriptorWrites[i].pImageInfo = &imageInfos[i];
                    _refCounts.Add(sampler.RefCount);
                }
                boundr++;
            }

            vkUpdateDescriptorSets(_gd.Device, descriptorWriteCount, descriptorWrites, 0, null);
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
            RefCount.Decrement();
        }

        private void DisposeCore()
        {
            if (!_destroyed)
            {
                _destroyed = true;
                _gd.DescriptorPoolManager.Free(_descriptorAllocationToken, _descriptorCounts);
            }
        }
    }
}
