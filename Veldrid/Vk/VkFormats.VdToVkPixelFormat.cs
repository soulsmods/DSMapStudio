using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Veldrid.Vk
{
    internal static partial class VkFormats
    {
        internal static VkFormat VdToVkPixelFormat(PixelFormat format, bool toDepthFormat = false)
        {
            switch (format)
            {
                case PixelFormat.R8_UNorm:
                    return VkFormat.R8UNorm;
                case PixelFormat.R8_SNorm:
                    return VkFormat.R8SNorm;
                case PixelFormat.R8_UInt:
                    return VkFormat.R8UInt;
                case PixelFormat.R8_SInt:
                    return VkFormat.R8SInt;

                case PixelFormat.R16_UNorm:
                    return toDepthFormat ? VkFormat.D16UNorm : VkFormat.R16UNorm;
                case PixelFormat.R16_SNorm:
                    return VkFormat.R16SNorm;
                case PixelFormat.R16_UInt:
                    return VkFormat.R16UInt;
                case PixelFormat.R16_SInt:
                    return VkFormat.R16SInt;
                case PixelFormat.R16_Float:
                    return VkFormat.R16SFloat;

                case PixelFormat.R32_UInt:
                    return VkFormat.R32UInt;
                case PixelFormat.R32_SInt:
                    return VkFormat.R32SInt;
                case PixelFormat.R32_Float:
                    return toDepthFormat ? VkFormat.D32SFloat : VkFormat.R32SFloat;

                case PixelFormat.R8_G8_UNorm:
                    return VkFormat.R8G8UNorm;
                case PixelFormat.R8_G8_SNorm:
                    return VkFormat.R8G8SNorm;
                case PixelFormat.R8_G8_UInt:
                    return VkFormat.R8G8UInt;
                case PixelFormat.R8_G8_SInt:
                    return VkFormat.R8G8SInt;

                case PixelFormat.R16_G16_UNorm:
                    return VkFormat.R16G16UNorm;
                case PixelFormat.R16_G16_SNorm:
                    return VkFormat.R16G16SNorm;
                case PixelFormat.R16_G16_UInt:
                    return VkFormat.R16G16UInt;
                case PixelFormat.R16_G16_SInt:
                    return VkFormat.R16G16SInt;
                case PixelFormat.R16_G16_Float:
                    return VkFormat.R16G16B16A16SFloat;

                case PixelFormat.R32_G32_UInt:
                    return VkFormat.R32G32UInt;
                case PixelFormat.R32_G32_SInt:
                    return VkFormat.R32G32SInt;
                case PixelFormat.R32_G32_Float:
                    return VkFormat.R32G32B32A32SFloat;

                case PixelFormat.B5_G5_R5_A1_UNorm:
                    return VkFormat.B5G5R5A1UNormPack16;
                case PixelFormat.R8_G8_B8_A8_UNorm:
                    return VkFormat.R8G8B8A8UNorm;
                case PixelFormat.R8_G8_B8_A8_UNorm_SRgb:
                    return VkFormat.R8G8B8A8SRgb;
                case PixelFormat.B8_G8_R8_A8_UNorm:
                    return VkFormat.B8G8R8A8UNorm;
                case PixelFormat.B8_G8_R8_A8_UNorm_SRgb:
                    return VkFormat.B8G8R8A8SRgb;
                case PixelFormat.R8_G8_B8_A8_SNorm:
                    return VkFormat.R8G8B8A8SNorm;
                case PixelFormat.R8_G8_B8_A8_UInt:
                    return VkFormat.R8G8B8A8UInt;
                case PixelFormat.R8_G8_B8_A8_SInt:
                    return VkFormat.R8G8B8A8SInt;

                case PixelFormat.R16_G16_B16_A16_UNorm:
                    return VkFormat.R16G16B16A16UNorm;
                case PixelFormat.R16_G16_B16_A16_SNorm:
                    return VkFormat.R16G16B16A16SNorm;
                case PixelFormat.R16_G16_B16_A16_UInt:
                    return VkFormat.R16G16B16A16UInt;
                case PixelFormat.R16_G16_B16_A16_SInt:
                    return VkFormat.R16G16B16A16SInt;
                case PixelFormat.R16_G16_B16_A16_Float:
                    return VkFormat.R16G16B16A16SFloat;

                case PixelFormat.R32_G32_B32_A32_UInt:
                    return VkFormat.R32G32B32A32UInt;
                case PixelFormat.R32_G32_B32_A32_SInt:
                    return VkFormat.R32G32B32A32SInt;
                case PixelFormat.R32_G32_B32_A32_Float:
                    return VkFormat.R32G32B32A32SFloat;

                case PixelFormat.BC1_Rgb_UNorm:
                    return VkFormat.BC1RGBUNormBlock;
                case PixelFormat.BC1_Rgb_UNorm_SRgb:
                    return VkFormat.BC1RGBSRgbBlock;
                case PixelFormat.BC1_Rgba_UNorm:
                    return VkFormat.BC1RGBAUNormBlock;
                case PixelFormat.BC1_Rgba_UNorm_SRgb:
                    return VkFormat.BC1RGBASRgbBlock;
                case PixelFormat.BC2_UNorm:
                    return VkFormat.BC2UNormBlock;
                case PixelFormat.BC2_UNorm_SRgb:
                    return VkFormat.BC2SRgbBlock;
                case PixelFormat.BC3_UNorm:
                    return VkFormat.BC3UNormBlock;
                case PixelFormat.BC3_UNorm_SRgb:
                    return VkFormat.BC3SRgbBlock;
                case PixelFormat.BC4_UNorm:
                    return VkFormat.BC4UNormBlock;
                case PixelFormat.BC4_SNorm:
                    return VkFormat.BC4SNormBlock;
                case PixelFormat.BC5_UNorm:
                    return VkFormat.BC5UNormBlock;
                case PixelFormat.BC5_SNorm:
                    return VkFormat.BC5SNormBlock;
                case PixelFormat.BC6H_UFloat:
                    return VkFormat.BC6HUFloatBlock;
                case PixelFormat.BC6H_SFloat:
                    return VkFormat.BC6HSFloatBlock;
                case PixelFormat.BC7_UNorm:
                    return VkFormat.BC7UNormBlock;
                case PixelFormat.BC7_UNorm_SRgb:
                    return VkFormat.BC7SRgbBlock;

                case PixelFormat.ETC2_R8_G8_B8_UNorm:
                    return VkFormat.ETC2R8G8B8UNormBlock;
                case PixelFormat.ETC2_R8_G8_B8_A1_UNorm:
                    return VkFormat.ETC2R8G8B8A1UNormBlock;
                case PixelFormat.ETC2_R8_G8_B8_A8_UNorm:
                    return VkFormat.ETC2R8G8B8A8UNormBlock;

                case PixelFormat.D32_Float_S8_UInt:
                    return VkFormat.D32SFloatS8UInt;
                case PixelFormat.D24_UNorm_S8_UInt:
                    return VkFormat.D24UNormS8UInt;

                case PixelFormat.R10_G10_B10_A2_UNorm:
                    return VkFormat.A2B10G10R10UNormPack32;
                case PixelFormat.R10_G10_B10_A2_UInt:
                    return VkFormat.A2B10G10R10UIntPack32;
                case PixelFormat.R11_G11_B10_Float:
                    return VkFormat.B10G11R11UFloatPack32;

                default:
                    throw new VeldridException($"Invalid {nameof(PixelFormat)}: {format}");
            }
        }
    }
}
