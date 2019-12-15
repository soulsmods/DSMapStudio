using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Numerics;

namespace StudioCore.Resource
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MapFlverLayout
    {
        public const uint SizeInBytes = 32;
        public Vector3 Position;
        public fixed short Uv1[2];
        public fixed sbyte Normal[4];
        public fixed sbyte Binormal[4];
        public fixed sbyte Bitangent[4];
        public fixed byte Color[4];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CollisionLayout
    {
        public const uint SizeInBytes = 20;
        public Vector3 Position;
        public fixed sbyte Normal[4];
        public fixed byte Color[4];
    }
}
