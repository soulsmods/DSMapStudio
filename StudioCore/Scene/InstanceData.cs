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
        public uint r1;
        public uint r2;
        public uint r3;
        public uint r4;
        public uint r5;
        public uint r6;
        public uint r7;
        public uint r8;
        public uint r9;
        public uint r10;
        public uint r11;
        public uint r12;
        public uint r13;
        public uint r14;
        public uint r15;
    }
}
