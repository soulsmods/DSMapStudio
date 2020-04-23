// This file is generated.

using System.Runtime.InteropServices;

namespace Vulkan
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct VkClearColorValue
    {
        [FieldOffset(0)]
        public float float32_0;
        [FieldOffset(4)]
        public float float32_1;
        [FieldOffset(8)]
        public float float32_2;
        [FieldOffset(12)]
        public float float32_3;
        [FieldOffset(0)]
        public int int32_0;
        [FieldOffset(4)]
        public int int32_1;
        [FieldOffset(8)]
        public int int32_2;
        [FieldOffset(12)]
        public int int32_3;
        [FieldOffset(0)]
        public uint uint32_0;
        [FieldOffset(4)]
        public uint uint32_1;
        [FieldOffset(8)]
        public uint uint32_2;
        [FieldOffset(12)]
        public uint uint32_3;
    }

    [StructLayout(LayoutKind.Explicit)]
    public partial struct VkClearValue
    {
        [FieldOffset(0)]
        public VkClearColorValue color;
        [FieldOffset(0)]
        public VkClearDepthStencilValue depthStencil;
    }
}
