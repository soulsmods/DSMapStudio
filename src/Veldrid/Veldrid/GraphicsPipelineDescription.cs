﻿using System;
using Vortice.Vulkan;

namespace Veldrid
{
    /// <summary>
    /// Describes a graphics <see cref="Pipeline"/>, for creation using a <see cref="ResourceFactory"/>.
    /// </summary>
    public struct GraphicsPipelineDescription : IEquatable<GraphicsPipelineDescription>
    {
        /// <summary>
        /// A description of the blend state, which controls how color values are blended into each color target.
        /// </summary>
        public BlendStateDescription BlendState;
        /// <summary>
        /// A description of the depth stencil state, which controls depth tests, writing, and comparisons.
        /// </summary>
        public DepthStencilStateDescription DepthStencilState;
        /// <summary>
        /// A description of the rasterizer state, which controls culling, clipping, scissor, and polygon-fill behavior.
        /// </summary>
        public RasterizerStateDescription RasterizerState;
        /// <summary>
        /// The <see cref="PrimitiveTopology"/> to use, which controls how a series of input vertices is interpreted by the
        /// <see cref="Pipeline"/>.
        /// </summary>
        public VkPrimitiveTopology PrimitiveTopology;
        /// <summary>
        /// A description of the shader set to be used.
        /// </summary>
        public ShaderSetDescription ShaderSet;
        /// <summary>
        /// An array of <see cref="ResourceLayout"/>, which controls the layout of shader resoruces in the <see cref="Pipeline"/>.
        /// </summary>
        public ResourceLayout[] ResourceLayouts;
        /// <summary>
        /// A description of the output attachments used by the <see cref="Pipeline"/>.
        /// </summary>
        public OutputDescription Outputs;

        /// <summary>
        /// Constructs a new <see cref="GraphicsPipelineDescription"/>.
        /// </summary>
        /// <param name="blendState">A description of the blend state, which controls how color values are blended into each
        /// color target.</param>
        /// <param name="depthStencilStateDescription">A description of the depth stencil state, which controls depth tests,
        /// writing, and comparisons.</param>
        /// <param name="rasterizerState">A description of the rasterizer state, which controls culling, clipping, scissor, and
        /// polygon-fill behavior.</param>
        /// <param name="primitiveTopology">The <see cref="PrimitiveTopology"/> to use, which controls how a series of input
        /// vertices is interpreted by the <see cref="Pipeline"/>.</param>
        /// <param name="shaderSet">A description of the shader set to be used.</param>
        /// <param name="resourceLayouts">An array of <see cref="ResourceLayout"/>, which controls the layout of shader resoruces
        /// in the <see cref="Pipeline"/>.</param>
        /// <param name="outputs">A description of the output attachments used by the <see cref="Pipeline"/>.</param>
        public GraphicsPipelineDescription(
            BlendStateDescription blendState,
            DepthStencilStateDescription depthStencilStateDescription,
            RasterizerStateDescription rasterizerState,
            VkPrimitiveTopology primitiveTopology,
            ShaderSetDescription shaderSet,
            ResourceLayout[] resourceLayouts,
            OutputDescription outputs)
        {
            BlendState = blendState;
            DepthStencilState = depthStencilStateDescription;
            RasterizerState = rasterizerState;
            PrimitiveTopology = primitiveTopology;
            ShaderSet = shaderSet;
            ResourceLayouts = resourceLayouts;
            Outputs = outputs;
        }

        /// <summary>
        /// Constructs a new <see cref="GraphicsPipelineDescription"/>.
        /// </summary>
        /// <param name="blendState">A description of the blend state, which controls how color values are blended into each
        /// color target.</param>
        /// <param name="depthStencilStateDescription">A description of the depth stencil state, which controls depth tests,
        /// writing, and comparisons.</param>
        /// <param name="rasterizerState">A description of the rasterizer state, which controls culling, clipping, scissor, and
        /// polygon-fill behavior.</param>
        /// <param name="primitiveTopology">The <see cref="PrimitiveTopology"/> to use, which controls how a series of input
        /// vertices is interpreted by the <see cref="Pipeline"/>.</param>
        /// <param name="shaderSet">A description of the shader set to be used.</param>
        /// <param name="resourceLayout">A <see cref="ResourceLayout"/>, which controls the layout of shader resoruces in the
        /// <see cref="Pipeline"/>.</param>
        /// <param name="outputs">A description of the output attachments used by the <see cref="Pipeline"/>.</param>
        public GraphicsPipelineDescription(
            BlendStateDescription blendState,
            DepthStencilStateDescription depthStencilStateDescription,
            RasterizerStateDescription rasterizerState,
            VkPrimitiveTopology primitiveTopology,
            ShaderSetDescription shaderSet,
            ResourceLayout resourceLayout,
            OutputDescription outputs)
        {
            BlendState = blendState;
            DepthStencilState = depthStencilStateDescription;
            RasterizerState = rasterizerState;
            PrimitiveTopology = primitiveTopology;
            ShaderSet = shaderSet;
            ResourceLayouts = new[] { resourceLayout };
            Outputs = outputs;
        }

        /// <summary>
        /// Element-wise equality.
        /// </summary>
        /// <param name="other">The instance to compare to.</param>
        /// <returns>True if all elements and all array elements are equal; false otherswise.</returns>
        public bool Equals(GraphicsPipelineDescription other)
        {
            return BlendState.Equals(other.BlendState)
                && DepthStencilState.Equals(other.DepthStencilState)
                && RasterizerState.Equals(other.RasterizerState)
                && PrimitiveTopology == other.PrimitiveTopology
                && ShaderSet.Equals(other.ShaderSet)
                && Util.ArrayEquals(ResourceLayouts, other.ResourceLayouts)
                && Outputs.Equals(other.Outputs);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return HashHelper.Combine(
                BlendState.GetHashCode(),
                DepthStencilState.GetHashCode(),
                RasterizerState.GetHashCode(),
                (int)PrimitiveTopology,
                ShaderSet.GetHashCode(),
                HashHelper.Array(ResourceLayouts),
                Outputs.GetHashCode());
        }
    }
}
