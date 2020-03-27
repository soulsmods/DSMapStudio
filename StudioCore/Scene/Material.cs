using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Numerics;

namespace StudioCore.Scene
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Material
    {
        public uint colorTex;
        public uint normalTex;
        public uint specTex;
        public uint padding;
    }
}
