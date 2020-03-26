#version 450

layout(set = 0, binding = 0) uniform ProjectionBuffer
{
    mat4 projection;
};
layout(set = 0, binding = 1) uniform ViewBuffer
{
    mat4 view;
};
layout(set = 0, binding = 2) uniform EyePositionBuffer
{
    vec3 eye;
};

struct instanceData
{
	mat4 world;
	uint materialID[4];
};

layout(set = 1, binding = 0, std140) buffer WorldBuffer
{
    readonly instanceData idata[];
};

layout(location = 0) in vec3 position;
layout(location = 1) in uvec2 uv;
layout(location = 2) in ivec4 normal;
layout(location = 0) out vec4 fsin_color;

void main()
{
	mat4 w = idata[gl_InstanceIndex].world;
	vec3 tnormal = mat3(w) * vec3(normal);
    fsin_color = vec4(vec3((vec4(tnormal, 1.0) / 255.0) + 0.5), 1.0);
    gl_Position = projection * view * w * vec4(position, 1);
}