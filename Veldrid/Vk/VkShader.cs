using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Veldrid.Vk.VulkanUtil;
using System;

namespace Veldrid.Vk
{
    internal unsafe class VkShader : Shader
    {
        private readonly VkGraphicsDevice _gd;
        private readonly VkShaderModule _shaderModule;
        private bool _disposed;
        private string _name;

        public VkShaderModule ShaderModule => _shaderModule;

        public VkShader(VkGraphicsDevice gd, ref ShaderDescription description)
            : base(description.Stage, description.EntryPoint)
        {
            _gd = gd;
            
            fixed (byte* codePtr = description.ShaderBytes)
            {
                var shaderModuleCI = new VkShaderModuleCreateInfo
                {
                    sType = VkStructureType.ShaderModuleCreateInfo,
                    codeSize = (UIntPtr)description.ShaderBytes.Length,
                    pCode = (uint*)codePtr
                };
                VkResult result = vkCreateShaderModule(gd.Device, &shaderModuleCI, null, out _shaderModule);
                CheckResult(result);
            }
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
                vkDestroyShaderModule(_gd.Device, ShaderModule, null);
            }
        }
    }
}
