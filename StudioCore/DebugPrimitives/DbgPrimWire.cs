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
using StudioCore.Resource;
using Veldrid.Utilities;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWire : DbgPrim
    {
        public int LineCount => Indices.Length / 2;

        public override MeshLayoutType LayoutType => MeshLayoutType.LayoutPositionColorNormal;

        public override VertexLayoutDescription LayoutDescription => VertexPositionColorNormal.Layout;

        public override BoundingBox Bounds => new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));

        public override string ShaderName => "DebugWire";

        public override SpecializationConstant[] SpecializationConstants => new SpecializationConstant[0];

        public override FaceCullMode CullMode => FaceCullMode.None;

        public override PolygonFillMode FillMode => PolygonFillMode.Wireframe;

        public override PrimitiveTopology Topology => PrimitiveTopology.LineList;

        public override uint VertexSize => MeshLayoutUtils.GetLayoutVertexSize(MeshLayoutType.LayoutPositionColorNormal);

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
            //int startIndex = Array.IndexOf(Vertices, startVert);
            //int endIndex = Array.IndexOf(Vertices, endVert);
            int startIndex = -1;
            int endIndex = -1;

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
    }
}
