﻿using System;
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
                    return VkFormat.R8Unorm;
                case PixelFormat.R8_SNorm:
                    return VkFormat.R8Snorm;
                case PixelFormat.R8_UInt:
                    return VkFormat.R8Uint;
                case PixelFormat.R8_SInt:
                    return VkFormat.R8Sint;

                case PixelFormat.R16_UNorm:
                    return toDepthFormat ? VkFormat.D16Unorm : VkFormat.R16Unorm;
                case PixelFormat.R16_SNorm:
                    return VkFormat.R16Snorm;
                case PixelFormat.R16_UInt:
                    return VkFormat.R16Uint;
                case PixelFormat.R16_SInt:
                    return VkFormat.R16Sint;
                case PixelFormat.R16_Float:
                    return VkFormat.R16Sfloat;

                case PixelFormat.R32_UInt:
                    return VkFormat.R32Uint;
                case PixelFormat.R32_SInt:
                    return VkFormat.R32Sint;
                case PixelFormat.R32_Float:
                    return toDepthFormat ? VkFormat.D32Sfloat : VkFormat.R32Sfloat;

                case PixelFormat.R8_G8_UNorm:
                    return VkFormat.R8G8Unorm;
                case PixelFormat.R8_G8_SNorm:
                    return VkFormat.R8G8Snorm;
                case PixelFormat.R8_G8_UInt:
                    return VkFormat.R8G8Uint;
                case PixelFormat.R8_G8_SInt:
                    return VkFormat.R8G8Sint;

                case PixelFormat.R16_G16_UNorm:
                    return VkFormat.R16G16Unorm;
                case PixelFormat.R16_G16_SNorm:
                    return VkFormat.R16G16Snorm;
                case PixelFormat.R16_G16_UInt:
                    return VkFormat.R16G16Uint;
                case PixelFormat.R16_G16_SInt:
                    return VkFormat.R16G16Sint;
                case PixelFormat.R16_G16_Float:
                    return VkFormat.R16G16B16A16Sfloat;

                case PixelFormat.R32_G32_UInt:
                    return VkFormat.R32G32Uint;
                case PixelFormat.R32_G32_SInt:
                    return VkFormat.R32G32Sint;
                case PixelFormat.R32_G32_Float:
                    return VkFormat.R32G32B32A32Sfloat;

                case PixelFormat.B5_G5_R5_A1_UNorm:
                    return VkFormat.B5G5R5A1UnormPack16;
                case PixelFormat.R8_G8_B8_A8_UNorm:
                    return VkFormat.R8G8B8A8Unorm;
                case PixelFormat.R8_G8_B8_A8_UNorm_SRgb:
                    return VkFormat.R8G8B8A8Srgb;
                case PixelFormat.B8_G8_R8_A8_UNorm:
                    return VkFormat.B8G8R8A8Unorm;
                case PixelFormat.B8_G8_R8_A8_UNorm_SRgb:
                    return VkFormat.B8G8R8A8Srgb;
                case PixelFormat.R8_G8_B8_A8_SNorm:
                    return VkFormat.R8G8B8A8Snorm;
                case PixelFormat.R8_G8_B8_A8_UInt:
                    return VkFormat.R8G8B8A8Uint;
                case PixelFormat.R8_G8_B8_A8_SInt:
                    return VkFormat.R8G8B8A8Sint;

                case PixelFormat.R16_G16_B16_A16_UNorm:
                    return VkFormat.R16G16B16A16Unorm;
                case PixelFormat.R16_G16_B16_A16_SNorm:
                    return VkFormat.R16G16B16A16Snorm;
                case PixelFormat.R16_G16_B16_A16_UInt:
                    return VkFormat.R16G16B16A16Uint;
                case PixelFormat.R16_G16_B16_A16_SInt:
                    return VkFormat.R16G16B16A16Sint;
                case PixelFormat.R16_G16_B16_A16_Float:
                    return VkFormat.R16G16B16A16Sfloat;

                case PixelFormat.R32_G32_B32_A32_UInt:
                    return VkFormat.R32G32B32A32Uint;
                case PixelFormat.R32_G32_B32_A32_SInt:
                    return VkFormat.R32G32B32A32Sint;
                case PixelFormat.R32_G32_B32_A32_Float:
                    return VkFormat.R32G32B32A32Sfloat;

                case PixelFormat.BC1_Rgb_UNorm:
                    return VkFormat.Bc1RgbUnormBlock;
                case PixelFormat.BC1_Rgb_UNorm_SRgb:
                    return VkFormat.Bc1RgbSrgbBlock;
                case PixelFormat.BC1_Rgba_UNorm:
                    return VkFormat.Bc1RgbaUnormBlock;
                case PixelFormat.BC1_Rgba_UNorm_SRgb:
                    return VkFormat.Bc1RgbaSrgbBlock;
                case PixelFormat.BC2_UNorm:
                    return VkFormat.Bc2UnormBlock;
                case PixelFormat.BC2_UNorm_SRgb:
                    return VkFormat.Bc2SrgbBlock;
                case PixelFormat.BC3_UNorm:
                    return VkFormat.Bc3UnormBlock;
                case PixelFormat.BC3_UNorm_SRgb:
                    return VkFormat.Bc3SrgbBlock;
                case PixelFormat.BC4_UNorm:
                    return VkFormat.Bc4UnormBlock;
                case PixelFormat.BC4_SNorm:
                    return VkFormat.Bc4SnormBlock;
                case PixelFormat.BC5_UNorm:
                    return VkFormat.Bc5UnormBlock;
                case PixelFormat.BC5_SNorm:
                    return VkFormat.Bc5SnormBlock;
                case PixelFormat.BC6H_UFloat:
                    return VkFormat.Bc6hUfloatBlock;
                case PixelFormat.BC6H_SFloat:
                    return VkFormat.Bc6hSfloatBlock;
                case PixelFormat.BC7_UNorm:
                    return VkFormat.Bc7UnormBlock;
                case PixelFormat.BC7_UNorm_SRgb:
                    return VkFormat.Bc7SrgbBlock;

                case PixelFormat.ETC2_R8_G8_B8_UNorm:
                    return VkFormat.Etc2R8G8B8UnormBlock;
                case PixelFormat.ETC2_R8_G8_B8_A1_UNorm:
                    return VkFormat.Etc2R8G8B8A1UnormBlock;
                case PixelFormat.ETC2_R8_G8_B8_A8_UNorm:
                    return VkFormat.Etc2R8G8B8A8UnormBlock;

                case PixelFormat.D32_Float_S8_UInt:
                    return VkFormat.D32SfloatS8Uint;
                case PixelFormat.D24_UNorm_S8_UInt:
                    return VkFormat.D24UnormS8Uint;

                case PixelFormat.R10_G10_B10_A2_UNorm:
                    return VkFormat.A2B10G10R10UnormPack32;
                case PixelFormat.R10_G10_B10_A2_UInt:
                    return VkFormat.A2B10G10R10UintPack32;
                case PixelFormat.R11_G11_B10_Float:
                    return VkFormat.B10G11R11UfloatPack32;

                default:
                    throw new VeldridException($"Invalid {nameof(PixelFormat)}: {format}");
            }
        }
    }
}
