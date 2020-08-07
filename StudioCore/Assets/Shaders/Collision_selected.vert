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
	// 0: material id
	// 3: entity id
	uvec4 materialID;
};

layout(set = 1, binding = 0, std140) buffer WorldBuffer
{
    readonly instanceData idata[];
};

layout(location = 0) in vec3 position;
layout(location = 1) in ivec4 normal;
layout(location = 2) in uvec4 color;
layout(location = 0) out vec3 fsin_normal;
layout(location = 1) out vec4 fsin_color;
layout(location = 2) out vec3 fsin_view;

void main()
{
	mat4 w = idata[gl_InstanceIndex].world;
	fsin_normal = normalize(mat3(w) * vec3(normal));
	fsin_view = normalize(sceneparam.eye.xyz - (w * vec4(position, 1)).xyz);
	
	vec3 ssnormal = mat3(sceneparam.projection) * mat3(sceneparam.view) * fsin_normal;

	vec4 posbase = (sceneparam.projection * sceneparam.view * w * vec4(position, 1));
    gl_Position = posbase - vec4(ssnormal, 0.0) * posbase.w * 0.005;
}