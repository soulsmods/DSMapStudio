using Vortice.Vulkan;

namespace Veldrid
{
    internal static partial class VkFormats
    {
        internal static VkImageUsageFlags VdToVkTextureUsage(TextureUsage vdUsage)
        {
            VkImageUsageFlags vkUsage = VkImageUsageFlags.None;

            vkUsage = VkImageUsageFlags.TransferDst | VkImageUsageFlags.TransferSrc;
            bool isDepthStencil = (vdUsage & TextureUsage.DepthStencil) == TextureUsage.DepthStencil;
            if ((vdUsage & TextureUsage.Sampled) == TextureUsage.Sampled)
            {
                vkUsage |= VkImageUsageFlags.Sampled;
            }
            if (isDepthStencil)
            {
                vkUsage |= VkImageUsageFlags.DepthStencilAttachment;
            }
            if ((vdUsage & TextureUsage.RenderTarget) == TextureUsage.RenderTarget)
            {
                vkUsage |= VkImageUsageFlags.ColorAttachment;
            }
            if ((vdUsage & TextureUsage.Storage) == TextureUsage.Storage)
            {
                vkUsage |= VkImageUsageFlags.Storage;
            }

            return vkUsage;
        }

        internal static VkImageType VdToVkTextureType(TextureType type)
        {
            switch (type)
            {
                case TextureType.Texture1D:
                    return VkImageType.Image1D;
                case TextureType.Texture2D:
                    return VkImageType.Image2D;
                case TextureType.Texture3D:
                    return VkImageType.Image3D;
                default:
                    throw Illegal.Value<TextureType>();
            }
        }

        internal static VkDescriptorType VdToVkDescriptorType(ResourceKind kind, ResourceLayoutElementOptions options)
        {
            bool dynamicBinding = (options & ResourceLayoutElementOptions.DynamicBinding) != 0;
            switch (kind)
            {
                case ResourceKind.UniformBuffer:
                    return dynamicBinding ? VkDescriptorType.UniformBufferDynamic : VkDescriptorType.UniformBuffer;
                case ResourceKind.StructuredBufferReadWrite:
                case ResourceKind.StructuredBufferReadOnly:
                    return dynamicBinding ? VkDescriptorType.StorageBufferDynamic : VkDescriptorType.StorageBuffer;
                case ResourceKind.TextureReadOnly:
                    return VkDescriptorType.SampledImage;
                case ResourceKind.TextureReadWrite:
                    return VkDescriptorType.StorageImage;
                case ResourceKind.Sampler:
                    return VkDescriptorType.Sampler;
                default:
                    throw Illegal.Value<ResourceKind>();
            }
        }

        internal static VkSampleCountFlags VdToVkSampleCount(TextureSampleCount sampleCount)
        {
            switch (sampleCount)
            {
                case TextureSampleCount.Count1:
                    return VkSampleCountFlags.Count1;
                case TextureSampleCount.Count2:
                    return VkSampleCountFlags.Count2;
                case TextureSampleCount.Count4:
                    return VkSampleCountFlags.Count4;
                case TextureSampleCount.Count8:
                    return VkSampleCountFlags.Count8;
                case TextureSampleCount.Count16:
                    return VkSampleCountFlags.Count16;
                case TextureSampleCount.Count32:
                    return VkSampleCountFlags.Count32;
                default:
                    throw Illegal.Value<TextureSampleCount>();
            }
        }

        internal static VkStencilOp VdToVkStencilOp(StencilOperation op)
        {
            switch (op)
            {
                case StencilOperation.Keep:
                    return VkStencilOp.Keep;
                case StencilOperation.Zero:
                    return VkStencilOp.Zero;
                case StencilOperation.Replace:
                    return VkStencilOp.Replace;
                case StencilOperation.IncrementAndClamp:
                    return VkStencilOp.IncrementAndClamp;
                case StencilOperation.DecrementAndClamp:
                    return VkStencilOp.DecrementAndClamp;
                case StencilOperation.Invert:
                    return VkStencilOp.Invert;
                case StencilOperation.IncrementAndWrap:
                    return VkStencilOp.IncrementAndWrap;
                case StencilOperation.DecrementAndWrap:
                    return VkStencilOp.DecrementAndWrap;
                default:
                    throw Illegal.Value<StencilOperation>();
            }
        }

        internal static VkPolygonMode VdToVkPolygonMode(PolygonFillMode fillMode)
        {
            switch (fillMode)
            {
                case PolygonFillMode.Solid:
                    return VkPolygonMode.Fill;
                case PolygonFillMode.Wireframe:
                    return VkPolygonMode.Line;
                default:
                    throw Illegal.Value<PolygonFillMode>();
            }
        }

        internal static VkCullModeFlags VdToVkCullMode(FaceCullMode cullMode)
        {
            switch (cullMode)
            {
                case FaceCullMode.Back:
                    return VkCullModeFlags.Back;
                case FaceCullMode.Front:
                    return VkCullModeFlags.Front;
                case FaceCullMode.None:
                    return VkCullModeFlags.None;
                default:
                    throw Illegal.Value<FaceCullMode>();
            }
        }

        internal static VkBlendOp VdToVkBlendOp(BlendFunction func)
        {
            switch (func)
            {
                case BlendFunction.Add:
                    return VkBlendOp.Add;
                case BlendFunction.Subtract:
                    return VkBlendOp.Subtract;
                case BlendFunction.ReverseSubtract:
                    return VkBlendOp.ReverseSubtract;
                case BlendFunction.Minimum:
                    return VkBlendOp.Min;
                case BlendFunction.Maximum:
                    return VkBlendOp.Max;
                default:
                    throw Illegal.Value<BlendFunction>();
            }
        }

        internal static VkPrimitiveTopology VdToVkPrimitiveTopology(PrimitiveTopology topology)
        {
            switch (topology)
            {
                case PrimitiveTopology.TriangleList:
                    return VkPrimitiveTopology.TriangleList;
                case PrimitiveTopology.TriangleStrip:
                    return VkPrimitiveTopology.TriangleStrip;
                case PrimitiveTopology.LineList:
                    return VkPrimitiveTopology.LineList;
                case PrimitiveTopology.LineStrip:
                    return VkPrimitiveTopology.LineStrip;
                case PrimitiveTopology.PointList:
                    return VkPrimitiveTopology.PointList;
                default:
                    throw Illegal.Value<PrimitiveTopology>();
            }
        }

        internal static uint GetSpecializationConstantSize(ShaderConstantType type)
        {
            switch (type)
            {
                case ShaderConstantType.Bool:
                    return 4;
                case ShaderConstantType.UInt16:
                    return 2;
                case ShaderConstantType.Int16:
                    return 2;
                case ShaderConstantType.UInt32:
                    return 4;
                case ShaderConstantType.Int32:
                    return 4;
                case ShaderConstantType.UInt64:
                    return 8;
                case ShaderConstantType.Int64:
                    return 8;
                case ShaderConstantType.Float:
                    return 4;
                case ShaderConstantType.Double:
                    return 8;
                default:
                    throw Illegal.Value<ShaderConstantType>();
            }
        }

        internal static VkBlendFactor VdToVkBlendFactor(BlendFactor factor)
        {
            switch (factor)
            {
                case BlendFactor.Zero:
                    return VkBlendFactor.Zero;
                case BlendFactor.One:
                    return VkBlendFactor.One;
                case BlendFactor.SourceAlpha:
                    return VkBlendFactor.SrcAlpha;
                case BlendFactor.InverseSourceAlpha:
                    return VkBlendFactor.OneMinusSrcAlpha;
                case BlendFactor.DestinationAlpha:
                    return VkBlendFactor.DstAlpha;
                case BlendFactor.InverseDestinationAlpha:
                    return VkBlendFactor.OneMinusDstAlpha;
                case BlendFactor.SourceColor:
                    return VkBlendFactor.SrcColor;
                case BlendFactor.InverseSourceColor:
                    return VkBlendFactor.OneMinusSrcColor;
                case BlendFactor.DestinationColor:
                    return VkBlendFactor.DstColor;
                case BlendFactor.InverseDestinationColor:
                    return VkBlendFactor.OneMinusDstColor;
                case BlendFactor.BlendFactor:
                    return VkBlendFactor.ConstantColor;
                case BlendFactor.InverseBlendFactor:
                    return VkBlendFactor.OneMinusConstantColor;
                default:
                    throw Illegal.Value<BlendFactor>();
            }
        }

        internal static VkFormat VdToVkVertexElementFormat(VertexElementFormat format)
        {
            switch (format)
            {
                case VertexElementFormat.Float1:
                    return VkFormat.R32Sfloat;
                case VertexElementFormat.Float2:
                    return VkFormat.R32G32Sfloat;
                case VertexElementFormat.Float3:
                    return VkFormat.R32G32B32Sfloat;
                case VertexElementFormat.Float4:
                    return VkFormat.R32G32B32A32Sfloat;
                case VertexElementFormat.Byte2_Norm:
                    return VkFormat.R8G8Unorm;
                case VertexElementFormat.Byte2:
                    return VkFormat.R8G8Uint;
                case VertexElementFormat.Byte4_Norm:
                    return VkFormat.R8G8B8A8Unorm;
                case VertexElementFormat.Byte4:
                    return VkFormat.R8G8B8A8Uint;
                case VertexElementFormat.SByte2_Norm:
                    return VkFormat.R8G8Snorm;
                case VertexElementFormat.SByte2:
                    return VkFormat.R8G8Sint;
                case VertexElementFormat.SByte4_Norm:
                    return VkFormat.R8G8B8A8Snorm;
                case VertexElementFormat.SByte4:
                    return VkFormat.R8G8B8A8Sint;
                case VertexElementFormat.UShort2_Norm:
                    return VkFormat.R16G16Unorm;
                case VertexElementFormat.UShort2:
                    return VkFormat.R16G16Uint;
                case VertexElementFormat.UShort4_Norm:
                    return VkFormat.R16G16B16A16Unorm;
                case VertexElementFormat.UShort4:
                    return VkFormat.R16G16B16A16Uint;
                case VertexElementFormat.Short2_Norm:
                    return VkFormat.R16G16Snorm;
                case VertexElementFormat.Short2:
                    return VkFormat.R16G16Sint;
                case VertexElementFormat.Short4_Norm:
                    return VkFormat.R16G16B16A16Snorm;
                case VertexElementFormat.Short4:
                    return VkFormat.R16G16B16A16Sint;
                case VertexElementFormat.UInt1:
                    return VkFormat.R32Uint;
                case VertexElementFormat.UInt2:
                    return VkFormat.R32G32Uint;
                case VertexElementFormat.UInt3:
                    return VkFormat.R32G32B32Uint;
                case VertexElementFormat.UInt4:
                    return VkFormat.R32G32B32A32Uint;
                case VertexElementFormat.Int1:
                    return VkFormat.R32Sint;
                case VertexElementFormat.Int2:
                    return VkFormat.R32G32Sint;
                case VertexElementFormat.Int3:
                    return VkFormat.R32G32B32Sint;
                case VertexElementFormat.Int4:
                    return VkFormat.R32G32B32A32Sint;
                case VertexElementFormat.Half1:
                    return VkFormat.R16Sfloat;
                case VertexElementFormat.Half2:
                    return VkFormat.R16G16Sfloat;
                case VertexElementFormat.Half4:
                    return VkFormat.R16G16B16A16Sfloat;
                default:
                    throw Illegal.Value<VertexElementFormat>();
            }
        }

        internal static VkShaderStageFlags VdToVkShaderStages(ShaderStages stage)
        {
            VkShaderStageFlags ret = VkShaderStageFlags.None;

            if ((stage & ShaderStages.Vertex) == ShaderStages.Vertex)
                ret |= VkShaderStageFlags.Vertex;

            if ((stage & ShaderStages.Geometry) == ShaderStages.Geometry)
                ret |= VkShaderStageFlags.Geometry;

            if ((stage & ShaderStages.TessellationControl) == ShaderStages.TessellationControl)
                ret |= VkShaderStageFlags.TessellationControl;

            if ((stage & ShaderStages.TessellationEvaluation) == ShaderStages.TessellationEvaluation)
                ret |= VkShaderStageFlags.TessellationEvaluation;

            if ((stage & ShaderStages.Fragment) == ShaderStages.Fragment)
                ret |= VkShaderStageFlags.Fragment;

            if ((stage & ShaderStages.Compute) == ShaderStages.Compute)
                ret |= VkShaderStageFlags.Compute;

            return ret;
        }

        internal static VkIndexType VdToVkIndexFormat(IndexFormat format)
        {
            switch (format)
            {
                case IndexFormat.UInt16:
                    return VkIndexType.Uint16;
                case IndexFormat.UInt32:
                    return VkIndexType.Uint32;
                default:
                    throw Illegal.Value<IndexFormat>();
            }
        }

        internal static VkCompareOp VdToVkCompareOp(ComparisonKind comparisonKind)
        {
            switch (comparisonKind)
            {
                case ComparisonKind.Never:
                    return VkCompareOp.Never;
                case ComparisonKind.Less:
                    return VkCompareOp.Less;
                case ComparisonKind.Equal:
                    return VkCompareOp.Equal;
                case ComparisonKind.LessEqual:
                    return VkCompareOp.LessOrEqual;
                case ComparisonKind.Greater:
                    return VkCompareOp.Greater;
                case ComparisonKind.NotEqual:
                    return VkCompareOp.NotEqual;
                case ComparisonKind.GreaterEqual:
                    return VkCompareOp.GreaterOrEqual;
                case ComparisonKind.Always:
                    return VkCompareOp.Always;
                default:
                    throw Illegal.Value<ComparisonKind>();
            }
        }
    }
}
