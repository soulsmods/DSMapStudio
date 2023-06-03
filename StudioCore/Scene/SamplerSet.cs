using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;
using Vortice.Vulkan;

namespace StudioCore.Scene
{
    /// <summary>
    /// Helper class that contains descriptor sets and layouts for samplers
    /// </summary>
    public static class SamplerSet
    {
        private static Sampler _linearSampler = null;
        private static Sampler _anisoLinearSampler = null;
        public static ResourceSet SamplersSet = null;
        public static ResourceLayout SamplersLayout = null;

        public static void Initialize(GraphicsDevice d)
        {
            var layoutdesc = new ResourceLayoutDescription(
                new ResourceLayoutElementDescription(
                    "linearSampler", 
                    VkDescriptorType.Sampler, 
                    VkShaderStageFlags.Fragment,
                    VkDescriptorBindingFlags.None),
                new ResourceLayoutElementDescription(
                    "anisoLinearSampler", 
                    VkDescriptorType.Sampler, 
                    VkShaderStageFlags.Fragment,
                    VkDescriptorBindingFlags.None));
            SamplersLayout = d.ResourceFactory.CreateResourceLayout(layoutdesc);

            _linearSampler = d.ResourceFactory.CreateSampler(new SamplerDescription(
                VkSamplerAddressMode.Repeat, VkSamplerAddressMode.Repeat, VkSamplerAddressMode.Repeat,
                VkFilter.Linear, VkFilter.Linear, VkSamplerMipmapMode.Linear,
                null, 0, 0, 15, 0, VkBorderColor.FloatOpaqueBlack
                ));
            _anisoLinearSampler = d.ResourceFactory.CreateSampler(new SamplerDescription(
                VkSamplerAddressMode.Repeat, VkSamplerAddressMode.Repeat, VkSamplerAddressMode.Repeat,
                VkFilter.Linear, VkFilter.Linear, VkSamplerMipmapMode.Linear,
                null, 16, 0, 15, 0, VkBorderColor.FloatOpaqueBlack
            ));

            var setdesc = new ResourceSetDescription(SamplersLayout, new [] { _linearSampler, _anisoLinearSampler });
            SamplersSet = d.ResourceFactory.CreateResourceSet(setdesc);
        }

        public static void Destory()
        {
            SamplersSet.Dispose();
            SamplersLayout.Dispose();
            _linearSampler.Dispose();
        }
    }
}
