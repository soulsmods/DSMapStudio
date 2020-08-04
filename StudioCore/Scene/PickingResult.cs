using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Numerics;

namespace StudioCore.Scene
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct PickingResult
    {
        public uint depth;
        uint padding;
        public ulong entityID;
    }
}
