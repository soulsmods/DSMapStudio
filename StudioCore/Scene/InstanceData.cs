using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Numerics;

namespace StudioCore.Scene
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct InstanceData
    {
        public Matrix4x4 WorldMatrix;
        public uint MaterialID;
        public uint BoneStartIndex;
        public uint r2;
        public uint EntityID;
    }
}
