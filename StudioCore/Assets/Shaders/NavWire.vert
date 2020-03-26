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

layout(set = 1, binding = 0) buffer WorldBuffer
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
	fsin_normal = mat3(w) * vec3((vec3(normal) / 255.0));
	fsin_color = vec4(color / 255.0);
    //fsin_color = vec4(vec3((vec4(tnormal, 1.0) / 255.0) + 0.5), 1.0);
	fsin_view = normalize(eye - vec3(w * vec4(position, 1)));
    vec4 p = view * w * vec4(position, 1);
	p.z -= 0.003;
    gl_Position = projection * p;
	// Apply a bias
	//gl_Position.z -= 0.01;
}