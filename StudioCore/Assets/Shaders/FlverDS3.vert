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

layout(location = 0) out vec2 fsin_texcoord;
layout(location = 1) out vec3 fsin_view;
layout(location = 2) out mat3 fsin_worldToTangent;
layout(location = 5) out vec3 fsin_normal;
layout(location = 6) out vec4 fsin_bitangent;
layout(location = 7) out vec4 fsin_color;

void main()
{
	mat4 w = idata[gl_InstanceIndex].world;
	fsin_texcoord = uv;
	fsin_normal = mat3(w) * vec3(normal);
	fsin_view = normalize(eye - (w * vec4(position, 1)).xyz);
    gl_Position = projection * view * w * vec4(position, 1);
}