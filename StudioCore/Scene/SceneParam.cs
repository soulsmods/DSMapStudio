using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Numerics;

namespace StudioCore.Scene
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SceneParam
    {
        public Matrix4x4 Projection;
        public Matrix4x4 View;
        public Vector4 EyePosition;
        public Vector4 LightDirection;
        public fixed int CursorPosition[4];
        public uint EnvMap;
        public float AmbientLightMult;
        public float DirectLightMult;
        public float IndirectLightMult;
        public float EmissiveMapMult;
        public float SceneBrightness;
        public fixed uint padding[2];
    }
}
