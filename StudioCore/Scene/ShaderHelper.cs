using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Veldrid;
using Veldrid.SPIRV;
using Vortice.Vulkan;

namespace StudioCore.Scene
{
    public static class ShaderHelper
    {
        private static readonly string s_assetRoot = Path.Combine(AppContext.BaseDirectory, "Assets");

        internal static string GetPath(string assetPath)
        {
            return Path.Combine(s_assetRoot, assetPath);
        }

        public static (Shader vs, Shader fs) LoadSPIRV(
            GraphicsDevice gd,
            ResourceFactory factory,
            string setName)
        {
            byte[] vsBytes = LoadBytecode(setName, VkShaderStageFlags.Vertex);
            byte[] fsBytes = LoadBytecode(setName, VkShaderStageFlags.Fragment);
            bool debug = false;
#if DEBUG
            debug = true;
#endif
            Shader[] shaders = factory.CreateFromSpirv(
                new ShaderDescription(VkShaderStageFlags.Vertex, vsBytes, "main", debug),
                new ShaderDescription(VkShaderStageFlags.Fragment, fsBytes, "main", debug),
                GetOptions(gd));

            Shader vs = shaders[0];
            Shader fs = shaders[1];

            vs.Name = setName + "-Vertex";
            fs.Name = setName + "-Fragment";

            return (vs, fs);
        }

        private static CrossCompileOptions GetOptions(GraphicsDevice gd)
        {
            SpecializationConstant[] specializations = GetSpecializations(gd);

            bool fixClipZ = !gd.IsDepthRangeZeroToOne;
            bool invertY = false;

            return new CrossCompileOptions(fixClipZ, invertY, specializations);
        }

        public static SpecializationConstant[] GetSpecializations(GraphicsDevice gd)
        {
            List<SpecializationConstant> specializations = new List<SpecializationConstant>();
            specializations.Add(new SpecializationConstant(100, gd.IsClipSpaceYInverted));
            specializations.Add(new SpecializationConstant(101, false)); // TextureCoordinatesInvertedY
            specializations.Add(new SpecializationConstant(102, gd.IsDepthRangeZeroToOne));

            var swapchainFormat = gd.MainSwapchain.Framebuffer.OutputDescription.ColorAttachments[0].Format;
            bool swapchainIsSrgb = swapchainFormat == VkFormat.R8G8B8A8Unorm
                || swapchainFormat == VkFormat.R8G8B8A8Srgb;
            specializations.Add(new SpecializationConstant(103, swapchainIsSrgb));

            return specializations.ToArray();
        }

        public static byte[] LoadBytecode(string setName, VkShaderStageFlags stage)
        {
            string stageExt = stage == VkShaderStageFlags.Vertex ? "vert" : "frag";
            string name = setName + "." + stageExt;

            string bytecodeExtension = GetBytecodeExtension();
            string bytecodePath = GetPath(Path.Combine("Shaders", name + bytecodeExtension));
            if (File.Exists(bytecodePath))
            {
                return File.ReadAllBytes(bytecodePath);
            }

            string extension = GetSourceExtension();
            string path = GetPath(Path.Combine("Shaders.Generated", name + extension));
            return File.ReadAllBytes(path);
        }

        private static string GetBytecodeExtension()
        {
            return ".spv";
        }

        private static string GetSourceExtension()
        {
            return ".450.glsl";
        }
    }
}
