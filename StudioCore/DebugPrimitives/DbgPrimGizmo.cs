using StudioCore.Scene;
using System;
using System.Numerics;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimGizmo : DbgPrim
    {
        //public override IGFXShader<DbgPrimSolidShader> Shader => GFX.DbgPrimSolidShader;

        //protected override PrimitiveType PrimType => PrimitiveType.TriangleList;

        public int TriCount => Indices.Length / 3;

        public void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color)
        {
            AddTri(a, b, c, color);
            AddTri(a, c, d, color);
        }

        public void AddTri(Vector3 a, Vector3 b, Vector3 c, Color color)
        {
            AddTri(a, b, c, color, color, color);
        }

        public void AddTri(Vector3 a, Vector3 b, Vector3 c, Color colorA, Color colorB, Color colorC)
        {
            var dir = Vector3.Cross(b - a, c - a);
            var norm = Vector3.Normalize(dir);

            var vertA = new VertexPositionColorNormal(a, colorA, norm);
            var vertB = new VertexPositionColorNormal(b, colorB, norm);
            var vertC = new VertexPositionColorNormal(c, colorC, norm);

            int vertIndexA = Array.IndexOf(Vertices, vertA);
            int vertIndexB = Array.IndexOf(Vertices, vertB);
            int vertIndexC = Array.IndexOf(Vertices, vertC);

            //If vertex A can't be recycled from an old one, make a new one.
            if (vertIndexA == -1)
            {
                AddVertex(vertA);
                vertIndexA = Vertices.Length - 1;
            }

            //If vertex B can't be recycled from an old one, make a new one.
            if (vertIndexB == -1)
            {
                AddVertex(vertB);
                vertIndexB = Vertices.Length - 1;
            }

            //If vertex C can't be recycled from an old one, make a new one.
            if (vertIndexC == -1)
            {
                AddVertex(vertC);
                vertIndexC = Vertices.Length - 1;
            }

            AddIndex((short)vertIndexC);
            AddIndex((short)vertIndexB);
            AddIndex((short)vertIndexA);

            //if (NeedToRecreateVertBuffer)
            //{
            //    VertBuffer = new VertexBuffer(GFX.Device, 
            //        typeof(VertexPositionColor), Vertices.Length, BufferUsage.WriteOnly);
            //    VertBuffer.SetData(Vertices);
            //    NeedToRecreateVertBuffer = false;
            //} 

            //if (NeedToRecreateIndexBuffer)
            //{
            //    IndexBuffer = new IndexBuffer(GFX.Device, IndexElementSize.ThirtyTwoBits, Indices.Length, BufferUsage.WriteOnly);
            //    IndexBuffer.SetData(Indices);
            //    NeedToRecreateIndexBuffer = false;
            //}
        }

        protected override void DisposeBuffers()
        {
            //VertBuffer?.Dispose();
        }

        public override DbgPrim Instantiate(string newName, Transform newLocation, Color? newNameColor = null)
        {
            var newPrim = new DbgPrimGizmo();
            newPrim.Indices = Indices;
            newPrim.GeomBuffer = GeomBuffer;
            newPrim.Vertices = Vertices;
            newPrim.NeedToRecreateGeomBuffer = NeedToRecreateGeomBuffer;

            newPrim.Transform = newLocation;

            newPrim.Name = newName;

            newPrim.NameColor = newNameColor ?? NameColor;

            return newPrim;
        }

        public override void Render(Renderer.IndirectDrawEncoder encoder, SceneRenderPipeline sp)
        {
            Draw(encoder, sp, null, new Matrix4x4());
        }

        public override void CreateDeviceObjects(Veldrid.GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            var factory = gd.ResourceFactory;
            WorldBuffer = Renderer.UniformBufferAllocator.Allocate(128, 128);
            var identity = System.Numerics.Matrix4x4.Identity;
            WorldBuffer.FillBuffer(cl, ref identity);

            ResourceLayout projViewCombinedLayout = StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            ResourceLayout worldLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding)));

            VertexLayoutDescription[] mainVertexLayouts = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Float3),
                    new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4),
                    new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Float3))
            };

            //Shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
            var res = StaticResourceCache.GetShaders(gd, gd.ResourceFactory, "DebugWire").ToTuple();
            Shaders = new Shader[] { res.Item1, res.Item2 };

            ResourceLayout mainPerObjectLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.StructuredBufferReadWrite, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.None)));

            PerObjRS = StaticResourceCache.GetResourceSet(factory, new ResourceSetDescription(mainPerObjectLayout,
                Renderer.UniformBufferAllocator._backingBuffer));

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.GreaterEqual);
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.None,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.CounterClockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: mainVertexLayouts,
                shaders: Shaders);
            pipelineDescription.ResourceLayouts = new ResourceLayout[] { StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                StaticResourceCache.SceneParamLayoutDescription), mainPerObjectLayout, Renderer.GlobalTexturePool.GetLayout(), Renderer.GlobalCubeTexturePool.GetLayout(), Renderer.MaterialBufferAllocator.GetLayout(), SamplerSet.SamplersLayout };
            pipelineDescription.Outputs = gd.SwapchainFramebuffer.OutputDescription;
            RenderPipeline = StaticResourceCache.GetPipeline(factory, ref pipelineDescription);
        }
    }
}
