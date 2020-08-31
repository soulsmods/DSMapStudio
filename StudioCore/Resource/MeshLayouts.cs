using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Numerics;
using Veldrid;

namespace StudioCore.Resource
{
    public enum MeshLayoutType
    {
        LayoutMinimal,
        LayoutSky,
        LayoutStandard,
        LayoutUV2,
        LayoutUV3,
        LayoutUV4,
        LayoutCollision,
        LayoutNavmesh,
        LayoutPositionColorNormal
    }

    public static class MeshLayoutUtils
    {
        public static VertexLayoutDescription GetLayoutDescription(MeshLayoutType type)
        {
            switch (type)
            {
                case MeshLayoutType.LayoutMinimal:
                    return FlverLayoutMinimal.Layout;
                case MeshLayoutType.LayoutSky:
                    return FlverLayoutSky.Layout;
                case MeshLayoutType.LayoutStandard:
                    return FlverLayout.Layout;
                case MeshLayoutType.LayoutUV2:
                    return FlverLayoutUV2.Layout;
                case MeshLayoutType.LayoutUV3:
                    return FlverLayoutUV2.Layout;
                case MeshLayoutType.LayoutUV4:
                    return FlverLayoutUV2.Layout;
                case MeshLayoutType.LayoutCollision:
                    return CollisionLayout.Layout;
                case MeshLayoutType.LayoutNavmesh:
                    return NavmeshLayout.Layout;
                case MeshLayoutType.LayoutPositionColorNormal:
                    return VertexPositionColorNormal.Layout;
                default:
                    throw new ArgumentException("Invalid layout type");
            }
        }

        public static unsafe uint GetLayoutVertexSize(MeshLayoutType type)
        {
            switch (type)
            {
                case MeshLayoutType.LayoutMinimal:
                    return (uint)sizeof(FlverLayoutMinimal);
                case MeshLayoutType.LayoutSky:
                    return (uint)sizeof(FlverLayoutSky);
                case MeshLayoutType.LayoutStandard:
                    return (uint)sizeof(FlverLayout);
                case MeshLayoutType.LayoutUV2:
                    return (uint)sizeof(FlverLayoutUV2);
                case MeshLayoutType.LayoutUV3:
                    return (uint)sizeof(FlverLayoutUV2);
                case MeshLayoutType.LayoutUV4:
                    return (uint)sizeof(FlverLayoutUV2);
                case MeshLayoutType.LayoutCollision:
                    return (uint)sizeof(CollisionLayout);
                case MeshLayoutType.LayoutNavmesh:
                    return (uint)sizeof(NavmeshLayout);
                case MeshLayoutType.LayoutPositionColorNormal:
                    return (uint)sizeof(VertexPositionColorNormal);
                default:
                    throw new ArgumentException("Invalid layout type");
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FlverLayoutMinimal
    {
        public Vector3 Position;
        public fixed short Uv1[2];
        public fixed sbyte Normal[4];
        public fixed byte Color[4];

        public static VertexLayoutDescription Layout = new VertexLayoutDescription(
            new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("uv1", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2),
            new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FlverLayoutSky
    {
        public Vector3 Position;
        public fixed sbyte Normal[4];

        public static VertexLayoutDescription Layout = new VertexLayoutDescription(
            new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FlverLayout
    {
        public Vector3 Position;
        public fixed short Uv1[2];
        public fixed sbyte Normal[4];
        public fixed sbyte Binormal[4];
        public fixed sbyte Bitangent[4];
        public fixed byte Color[4];

        public static VertexLayoutDescription Layout = new VertexLayoutDescription(
            new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("uv1", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2),
            new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("binormal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("bitangent", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FlverLayoutUV2
    {
        public Vector3 Position;
        public fixed short Uv1[2];
        public fixed sbyte Normal[4];
        public fixed sbyte Binormal[4];
        public fixed sbyte Bitangent[4];
        public fixed byte Color[4];
        public fixed short Uv2[2];

        public static VertexLayoutDescription Layout = new VertexLayoutDescription(
            new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("uv1", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2),
            new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("binormal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("bitangent", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4),
            new VertexElementDescription("uv2", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FlverLayoutUV3
    {
        public Vector3 Position;
        public fixed short Uv1[2];
        public fixed sbyte Normal[4];
        public fixed sbyte Binormal[4];
        public fixed sbyte Bitangent[4];
        public fixed byte Color[4];
        public fixed short Uv2[2];
        public fixed short Uv3[2];

        public static VertexLayoutDescription Layout = new VertexLayoutDescription(
            new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("uv1", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2),
            new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("binormal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("bitangent", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4),
            new VertexElementDescription("uv2", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2),
            new VertexElementDescription("uv3", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FlverLayoutUV4
    {
        public Vector3 Position;
        public fixed short Uv1[2];
        public fixed sbyte Normal[4];
        public fixed sbyte Binormal[4];
        public fixed sbyte Bitangent[4];
        public fixed byte Color[4];
        public fixed short Uv2[2];
        public fixed short Uv3[2];
        public fixed short Uv4[2];

        public static VertexLayoutDescription Layout = new VertexLayoutDescription(
            new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("uv1", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2),
            new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("binormal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("bitangent", VertexElementSemantic.TextureCoordinate, VertexElementFormat.SByte4),
            new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4),
            new VertexElementDescription("uv2", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2),
            new VertexElementDescription("uv3", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2),
            new VertexElementDescription("uv4", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Short2));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CollisionLayout
    {
        public const uint SizeInBytes = 24;
        public Vector3 Position;
        public fixed sbyte Normal[4];
        public fixed byte Color[4];
        public fixed byte Barycentric[4];

        public static VertexLayoutDescription Layout = new VertexLayoutDescription(
             new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Float3),
             new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.SByte4),
             new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4),
             new VertexElementDescription("barycentric", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct NavmeshLayout
    {
        public const uint SizeInBytes = 24;
        public Vector3 Position;
        public fixed sbyte Normal[4];
        public fixed byte Color[4];
        public fixed byte Barycentric[4];

        public static VertexLayoutDescription Layout = new VertexLayoutDescription(
             new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Float3),
             new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.SByte4),
             new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4),
             new VertexElementDescription("barycentric", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4));
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
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

        public static VertexLayoutDescription Layout = new VertexLayoutDescription(
            new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Float3),
            new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4),
            new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Float3));
    }
}
