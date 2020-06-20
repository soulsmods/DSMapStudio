#version 450

struct sceneParams
{
	mat4 projection;
	mat4 view;
	vec4 eye;
	vec4 lightDirection;
	uint envmap;
};

layout(set = 0, binding = 0) uniform SceneParamBuffer
{
    sceneParams sceneparam;
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
layout(location = 1) in uvec4 color;
layout(location = 2) in vec3 normal;
layout(location = 0) out vec4 fsin_color;
layout(location = 1) out uint fsin_mat;

void main()
{
	mat4 w = idata[gl_InstanceIndex].world;
    fsin_color = vec4(color) / 255.0;
	fsin_mat = idata[gl_InstanceIndex].materialID[0];
    gl_Position = sceneparam.projection * sceneparam.view * w * vec4(position, 1);
}