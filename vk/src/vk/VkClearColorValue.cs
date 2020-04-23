using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Vulkan
{
    public partial struct VkClearColorValue
    {
        public VkClearColorValue(float r, float g, float b, float a = 1.0f) : this()
        {
            float32_0 = r;
            float32_1 = g;
            float32_2 = b;
            float32_3 = a;
        }

        public VkClearColorValue(int r, int g, int b, int a = 255) : this()
        {
            int32_0 = r;
            int32_1 = g;
            int32_2 = b;
            int32_3 = a;
        }

        public VkClearColorValue(uint r, uint g, uint b, uint a = 255) : this()
        {
            uint32_0 = r;
            uint32_1 = g;
            uint32_2 = b;
            uint32_3 = a;
        }
    }
}
