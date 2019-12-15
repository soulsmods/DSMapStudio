using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Drawing;
using System.Threading.Tasks;

namespace StudioCore.DebugPrimitives
{
    public struct VertexPositionColorNormal
    {
        private struct _color
        {
            public byte r;
            public byte g;
            public byte b;
            public byte a;
        }

        public Vector3 Position;
        private _color _Color;
        public Vector3 Normal;

        public Color Color
        {
            set
            {
                _Color.r = value.R;
                _Color.g = value.G;
                _Color.b = value.B;
                _Color.a = value.A;
            }
        }

        public VertexPositionColorNormal(Vector3 position, Color color, Vector3 normal)
        {
            Position = position;
            _Color = new _color();
            _Color.r = color.R;
            _Color.g = color.G;
            _Color.b = color.B;
            _Color.a = color.A;
            Normal = normal;
        }
    }
}
