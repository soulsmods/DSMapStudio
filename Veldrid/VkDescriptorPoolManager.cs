﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Veldrid
{
    internal class VkDescriptorPoolManager
    {
        private readonly GraphicsDevice _gd;
        private readonly List<PoolInfo> _pools = new List<PoolInfo>();
        private readonly object _lock = new object();

        public VkDescriptorPoolManager(GraphicsDevice gd)
        {
            _gd = gd;
            _pools.Add(CreateNewPool());
        }

        public unsafe DescriptorAllocationToken Allocate(DescriptorResourceCounts counts,
            VkDescriptorSetLayout setLayout, uint variableCount)
        {
            VkDescriptorPool pool = GetPool(counts);
            var variableCountAI = new VkDescriptorSetVariableDescriptorCountAllocateInfo
            {
                sType = VkStructureType.DescriptorSetVariableDescriptorCountAllocateInfo,
                descriptorSetCount = 1,
                pDescriptorCounts = &variableCount,
            };
            VkDescriptorSetAllocateInfo dsAI = new VkDescriptorSetAllocateInfo
            {
                sType = VkStructureType.DescriptorSetAllocateInfo,
                pNext = &variableCountAI,
                descriptorSetCount = 1,
                pSetLayouts = &setLayout,
                descriptorPool = pool
            };
            VkDescriptorSet set = new VkDescriptorSet();
            VkResult result = vkAllocateDescriptorSets(_gd.Device, &dsAI, &set);
            VulkanUtil.CheckResult(result);

            return new DescriptorAllocationToken(set, pool);
        }

        public void Free(DescriptorAllocationToken token, DescriptorResourceCounts counts)
        {
            lock (_lock)
            {
                foreach (PoolInfo poolInfo in _pools)
                {
                    if (poolInfo.Pool == token.Pool)
                    {
                        poolInfo.Free(_gd.Device, token, counts);
                    }
                }
            }
        }

        private VkDescriptorPool GetPool(DescriptorResourceCounts counts)
        {
            lock (_lock)
            {
                foreach (PoolInfo poolInfo in _pools)
                {
                    if (poolInfo.Allocate(counts))
                    {
                        return poolInfo.Pool;
                    }
                }

                PoolInfo newPool = CreateNewPool(counts);
                _pools.Add(newPool);
                bool result = newPool.Allocate(counts);
                Debug.Assert(result);
                return newPool.Pool;
            }
        }

        private unsafe PoolInfo CreateNewPool(DescriptorResourceCounts counts)
        {
            uint totalSets = 1000;
            uint descriptorCount = 100;
            uint poolSizeCount = 7;
            VkDescriptorPoolSize* sizes = stackalloc VkDescriptorPoolSize[(int)poolSizeCount];
            sizes[0].type = VkDescriptorType.UniformBuffer;
            sizes[0].descriptorCount = counts.UniformBufferCount < descriptorCount ? descriptorCount : counts.UniformBufferCount;
            sizes[1].type = VkDescriptorType.SampledImage;
            sizes[1].descriptorCount = counts.SampledImageCount < descriptorCount ? descriptorCount : counts.SampledImageCount;
            sizes[2].type = VkDescriptorType.Sampler;
            sizes[2].descriptorCount = counts.SamplerCount < descriptorCount ? descriptorCount : counts.SamplerCount;
            sizes[3].type = VkDescriptorType.StorageBuffer;
            sizes[3].descriptorCount = counts.StorageBufferCount < descriptorCount ? descriptorCount : counts.StorageBufferCount;
            sizes[4].type = VkDescriptorType.StorageImage;
            sizes[4].descriptorCount = counts.StorageImageCount < descriptorCount ? descriptorCount : counts.StorageImageCount;
            sizes[5].type = VkDescriptorType.UniformBufferDynamic;
            sizes[5].descriptorCount = descriptorCount;
            sizes[6].type = VkDescriptorType.StorageBufferDynamic;
            sizes[6].descriptorCount = descriptorCount;

            var poolCI = new VkDescriptorPoolCreateInfo
            {
                sType = VkStructureType.DescriptorPoolCreateInfo,
                flags = VkDescriptorPoolCreateFlags.FreeDescriptorSet | VkDescriptorPoolCreateFlags.UpdateAfterBind,
                maxSets = totalSets,
                pPoolSizes = sizes,
                poolSizeCount = poolSizeCount
            };

            VkResult result = vkCreateDescriptorPool(_gd.Device, &poolCI, null, out VkDescriptorPool descriptorPool);
            VulkanUtil.CheckResult(result);

            DescriptorResourceCounts cts = new DescriptorResourceCounts(
                counts.UniformBufferCount < descriptorCount ? descriptorCount : counts.UniformBufferCount,
                counts.SampledImageCount < descriptorCount ? descriptorCount : counts.SampledImageCount,
                counts.SamplerCount < descriptorCount ? descriptorCount : counts.SamplerCount,
                counts.StorageBufferCount < descriptorCount ? descriptorCount : counts.StorageBufferCount,
                counts.StorageImageCount < descriptorCount ? descriptorCount : counts.StorageImageCount);

            return new PoolInfo(descriptorPool, totalSets, cts);
        }

        private unsafe PoolInfo CreateNewPool()
        {
            uint totalSets = 1000;
            uint descriptorCount = 100;
            uint poolSizeCount = 7;
            VkDescriptorPoolSize* sizes = stackalloc VkDescriptorPoolSize[(int)poolSizeCount];
            sizes[0].type = VkDescriptorType.UniformBuffer;
            sizes[0].descriptorCount = descriptorCount;
            sizes[1].type = VkDescriptorType.SampledImage;
            sizes[1].descriptorCount = descriptorCount;
            sizes[2].type = VkDescriptorType.Sampler;
            sizes[2].descriptorCount = descriptorCount;
            sizes[3].type = VkDescriptorType.StorageBuffer;
            sizes[3].descriptorCount = descriptorCount;
            sizes[4].type = VkDescriptorType.StorageImage;
            sizes[4].descriptorCount = descriptorCount;
            sizes[5].type = VkDescriptorType.UniformBufferDynamic;
            sizes[5].descriptorCount = descriptorCount;
            sizes[6].type = VkDescriptorType.StorageBufferDynamic;
            sizes[6].descriptorCount = descriptorCount;

            var poolCI = new VkDescriptorPoolCreateInfo
            {
                sType = VkStructureType.DescriptorPoolCreateInfo,
                flags = VkDescriptorPoolCreateFlags.FreeDescriptorSet | VkDescriptorPoolCreateFlags.UpdateAfterBind,
                maxSets = totalSets,
                pPoolSizes = sizes,
                poolSizeCount = poolSizeCount
            };

            VkResult result = vkCreateDescriptorPool(_gd.Device, &poolCI, null, out VkDescriptorPool descriptorPool);
            VulkanUtil.CheckResult(result);

            return new PoolInfo(descriptorPool, totalSets, descriptorCount);
        }

        internal unsafe void DestroyAll()
        {
            foreach (PoolInfo poolInfo in _pools)
            {
                vkDestroyDescriptorPool(_gd.Device, poolInfo.Pool, null);
            }
        }

        private class PoolInfo
        {
            public readonly VkDescriptorPool Pool;

            public uint RemainingSets;

            public uint UniformBufferCount;
            public uint SampledImageCount;
            public uint SamplerCount;
            public uint StorageBufferCount;
            public uint StorageImageCount;

            public PoolInfo(VkDescriptorPool pool, uint totalSets, uint descriptorCount)
            {
                Pool = pool;
                RemainingSets = totalSets;
                UniformBufferCount = descriptorCount;
                SampledImageCount = descriptorCount;
                SamplerCount = descriptorCount;
                StorageBufferCount = descriptorCount;
                StorageImageCount = descriptorCount;
            }

            public PoolInfo(VkDescriptorPool pool, uint totalSets, DescriptorResourceCounts counts)
            {
                Pool = pool;
                RemainingSets = totalSets;
                UniformBufferCount = counts.UniformBufferCount;
                SampledImageCount = counts.SampledImageCount;
                SamplerCount = counts.SamplerCount;
                StorageBufferCount = counts.StorageBufferCount;
                StorageImageCount = counts.StorageImageCount;
            }

            internal bool Allocate(DescriptorResourceCounts counts)
            {
                if (RemainingSets > 0
                    && UniformBufferCount >= counts.UniformBufferCount
                    && SampledImageCount >= counts.SampledImageCount
                    && SamplerCount >= counts.SamplerCount
                    && StorageBufferCount >= counts.SamplerCount
                    && StorageImageCount >= counts.StorageImageCount)
                {
                    RemainingSets -= 1;
                    UniformBufferCount -= counts.UniformBufferCount;
                    SampledImageCount -= counts.SampledImageCount;
                    SamplerCount -= counts.SamplerCount;
                    StorageBufferCount -= counts.StorageBufferCount;
                    StorageImageCount -= counts.StorageImageCount;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            unsafe internal void Free(VkDevice device, DescriptorAllocationToken token, DescriptorResourceCounts counts)
            {
                VkDescriptorSet set = token.Set;
                vkFreeDescriptorSets(device, Pool, 1, &set);

                RemainingSets += 1;

                UniformBufferCount += counts.UniformBufferCount;
                SampledImageCount += counts.SampledImageCount;
                SamplerCount += counts.SamplerCount;
                StorageBufferCount += counts.StorageBufferCount;
                StorageImageCount += counts.StorageImageCount;
            }
        }
    }

    internal struct DescriptorAllocationToken
    {
        public readonly VkDescriptorSet Set;
        public readonly VkDescriptorPool Pool;

        public DescriptorAllocationToken(VkDescriptorSet set, VkDescriptorPool pool)
        {
            Set = set;
            Pool = pool;
        }
    }
}
