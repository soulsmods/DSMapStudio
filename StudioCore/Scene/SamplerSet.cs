using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;

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
                new ResourceLayoutElementDescription("linearSampler", ResourceKind.Sampler, ShaderStages.Fragment, ResourceLayoutElementOptions.None),
                new ResourceLayoutElementDescription("anisoLinearSampler", ResourceKind.Sampler, ShaderStages.Fragment, ResourceLayoutElementOptions.None));
            SamplersLayout = d.ResourceFactory.CreateResourceLayout(layoutdesc);

            _linearSampler = d.ResourceFactory.CreateSampler(new SamplerDescription(
                SamplerAddressMode.Wrap, SamplerAddressMode.Wrap, SamplerAddressMode.Wrap,
                SamplerFilter.MinLinear_MagLinear_MipLinear,
                null, 0, 0, 15, 0, SamplerBorderColor.OpaqueBlack
                ));
            _anisoLinearSampler = d.ResourceFactory.CreateSampler(new SamplerDescription(
                SamplerAddressMode.Wrap, SamplerAddressMode.Wrap, SamplerAddressMode.Wrap,
                SamplerFilter.Anisotropic,
                null, 16, 0, 15, 0, SamplerBorderColor.OpaqueBlack
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
