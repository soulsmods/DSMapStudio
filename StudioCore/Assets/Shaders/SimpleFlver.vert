#version 450

struct sceneParams
{
	mat4 projection;
	mat4 view;
	vec4 eye;
	vec4 lightDirection;
	ivec4 curserPosition;
	uint envmap;
	
	float ambientLightMult;
	float directLightMult;
	float indirectLightMult;
	float emissiveMapMult;
	float sceneBrightness;
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
layout(location = 1) in ivec4 normal;
layout(location = 0) out vec4 fsin_color;

void main()
{
	mat4 w = idata[gl_InstanceIndex].world;
	vec3 tnormal = normalize(mat3(w) * vec3(normal));
    fsin_color = vec4((vec3((vec4(tnormal, 1.0)) + 0.5) * 0.5) + 0.25, 1.0);
    gl_Position = sceneparam.projection * sceneparam.view * w * vec4(position, 1);
}