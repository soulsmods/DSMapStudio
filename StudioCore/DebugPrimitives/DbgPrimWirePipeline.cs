using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace StudioCore.DebugPrimitives
{
    public static class DbgPrimWirePipeline
    {
        private static Shader[] _shaders;
        private static Pipeline _pipeline;

        private static DeviceBuffer ProjectionBuffer;
        private static DeviceBuffer ViewBuffer;
        private static DeviceBuffer WorldBuffer;

        private const string VertexShader = @"
#version 450

layout(set = 0, binding = 0) uniform ProjectionBuffer
{
    mat4 projection;
};
layout(set = 0, binding = 1) uniform ViewBuffer
{
    mat4 view;
};
layout(set = 0, binding = 2) uniform WorldBuffer
{
    mat4 world;
};

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 color;
layout(location = 2) in vec3 normal;

void main()
{
    gl_Position = projection * view * world * vec4(position, 1);
}";

        private const string FragmentShader = @"
#version 450
layout(location = 0) out vec4 fsout_color;
void main()
{
    fsout_color = vec4(1.0, 1.0, 1.0, 1.0);
}";

        public static void Init(GraphicsDevice device)
        {
            var factory = device.ResourceFactory;
        }
    }
}
