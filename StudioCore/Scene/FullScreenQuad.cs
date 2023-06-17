using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using Vortice.Vulkan;

namespace StudioCore.Scene
{
    public class FullScreenQuad
    {
        private DisposeCollector _disposeCollector;
        private Pipeline _pipeline;
        private DeviceBuffer _ib;
        private DeviceBuffer _vb;

        public void CreateDeviceObjects(GraphicsDevice gd, CommandList cl)
        {
            DisposeCollectorResourceFactory factory = new DisposeCollectorResourceFactory(gd.ResourceFactory);
            _disposeCollector = factory.DisposeCollector;

            //ResourceLayout resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription());

            (Shader vs, Shader fs) = StaticResourceCache.GetShaders(gd, gd.ResourceFactory, "FullScreenQuad");

            GraphicsPipelineDescription pd = new GraphicsPipelineDescription(
                new BlendStateDescription(
                    RgbaFloat.Black,
                    BlendAttachmentDescription.OverrideBlend),
                new DepthStencilStateDescription(true, true, VkCompareOp.Always),
                new RasterizerStateDescription(VkCullModeFlags.Back, VkPolygonMode.Fill, VkFrontFace.Clockwise, true, false),
                VkPrimitiveTopology.TriangleList,
                new ShaderSetDescription(
                    new[]
                    {
                        new VertexLayoutDescription(
                            new VertexElementDescription("Position", VkFormat.R32G32Sfloat),
                            new VertexElementDescription("TexCoords", VkFormat.R32G32Sfloat))
                    },
                    new[] { vs, fs },
                    ShaderHelper.GetSpecializations(gd)),
                new ResourceLayout[] { },
                gd.SwapchainFramebuffer.OutputDescription);
            _pipeline = factory.CreateGraphicsPipeline(ref pd);

            float[] verts = Utils.GetFullScreenQuadVerts(gd);

            _vb = factory.CreateBuffer(
                new BufferDescription(
                    (uint)verts.Length * sizeof(float),
                    VkBufferUsageFlags.VertexBuffer,
                    VmaMemoryUsage.AutoPreferDevice,
                    0));
            cl.UpdateBuffer(_vb, 0, verts);

            _ib = factory.CreateBuffer(
                new BufferDescription(
                    (uint)s_quadIndices.Length * sizeof(ushort),
                    VkBufferUsageFlags.IndexBuffer,
                    VmaMemoryUsage.AutoPreferDevice,
                    0));
            cl.UpdateBuffer(_ib, 0, s_quadIndices);
            
            cl.Barrier(VkPipelineStageFlags2.Transfer,
                VkAccessFlags2.TransferWrite,
                VkPipelineStageFlags2.VertexInput | VkPipelineStageFlags2.IndexInput,
                VkAccessFlags2.VertexAttributeRead);
        }

        public void DestroyDeviceObjects()
        {
            _disposeCollector.DisposeAll();
        }

        public void Render(GraphicsDevice gd, CommandList cl)
        {
            cl.SetPipeline(_pipeline);
            cl.SetVertexBuffer(0, _vb);
            cl.SetIndexBuffer(_ib, VkIndexType.Uint16);
            cl.DrawIndexed(6, 1, 0, 0, 0);
        }

        private static ushort[] s_quadIndices = new ushort[] { 0, 1, 2, 0, 2, 3 };
    }
}
