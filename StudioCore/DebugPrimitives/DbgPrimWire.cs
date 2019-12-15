using Veldrid;
using Veldrid.SPIRV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using StudioCore.Scene;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWire : DbgPrim
    {
        //public override IGFXShader<DbgPrimWireShader> Shader => GFX.DbgPrimWireShader;

        //protected override PrimitiveType PrimType => PrimitiveType.LineList;

        public int LineCount => Indices.Length / 2;

        //private static Pipeline 

        public void AddLine(Vector3 start, Vector3 end)
        {
            AddLine(start, end, Color.White);
        }

        public void AddLine(Vector3 start, Vector3 end, Color color)
        {
            AddLine(start, end, color, color);
        }

        public void AddLine(Vector3 start, Vector3 end, Color startColor, Color endColor)
        {
            var startVert = new VertexPositionColorNormal(start, startColor, Vector3.Zero);
            var endVert = new VertexPositionColorNormal(end, endColor, Vector3.Zero);
            int startIndex = Array.IndexOf(Vertices, startVert);
            int endIndex = Array.IndexOf(Vertices, endVert);

            //If start vertex can't be recycled from an old one, make a new one.
            if (startIndex == -1)
            {
                AddVertex(startVert);
                startIndex = Vertices.Length - 1;
            }

            //If end vertex can't be recycled from an old one, make a new one.
            if (endIndex == -1)
            {
                AddVertex(endVert);
                endIndex = Vertices.Length - 1;
            }

            for (int i = 0; i < Indices.Length; i += 2)
            {
                int lineStart = Indices[i];
                if ((i + 1) < Indices.Length)
                {
                    int lineEnd = Indices[i + 1];

                    if (lineStart == startIndex && lineEnd == endIndex)
                    {
                        // Line literally already exists lmao
                        return;
                    }
                }
            }

            AddIndex((short)startIndex);
            AddIndex((short)endIndex);
        }

        protected override void DisposeBuffers()
        {
            //VertBuffer?.Dispose();
        }

        public override DbgPrim Instantiate(string newName, Transform newLocation, Color? newNameColor = null)
        {
            var newPrim = new DbgPrimWire();
            newPrim.Indices = Indices;
            newPrim.VertBuffer = VertBuffer;
            newPrim.IndexBuffer = IndexBuffer;
            newPrim.Vertices = Vertices;
            newPrim.NeedToRecreateVertBuffer = NeedToRecreateVertBuffer;
            newPrim.NeedToRecreateIndexBuffer = NeedToRecreateIndexBuffer;

            newPrim.Transform = newLocation;

            newPrim.Name = newName;

            newPrim.NameColor = newNameColor ?? NameColor;

            return newPrim;
        }

        public override void Render(Veldrid.GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            Draw(gd, cl, sp, null, new Matrix4x4());
        }

        public override void CreateDeviceObjects(Veldrid.GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            var factory = gd.ResourceFactory;
            WorldBuffer = factory.CreateBuffer(new BufferDescription(64, Veldrid.BufferUsage.UniformBuffer | Veldrid.BufferUsage.Dynamic));
            var identity = System.Numerics.Matrix4x4.Identity;
            gd.UpdateBuffer(WorldBuffer, 0, ref identity, 64);

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
                new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.DynamicBinding)));


            PerObjRS = factory.CreateResourceSet(new ResourceSetDescription(mainPerObjectLayout,
                WorldBuffer));

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.None,
                fillMode: PolygonFillMode.Wireframe,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.LineList;
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: mainVertexLayouts,
                shaders: Shaders);
            pipelineDescription.ResourceLayouts = new ResourceLayout[] { StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                StaticResourceCache.ProjViewLayoutDescription), mainPerObjectLayout };
            pipelineDescription.Outputs = gd.SwapchainFramebuffer.OutputDescription;
            RenderPipeline = factory.CreateGraphicsPipeline(pipelineDescription);
        }

        public override void DestroyDeviceObjects()
        {
            throw new NotImplementedException();
        }
    }
}
