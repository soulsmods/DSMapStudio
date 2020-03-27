#version 450
#extension GL_EXT_nonuniform_qualifier : require

layout(location = 0) in vec2 fsin_texcoord;
layout(location = 1) in vec3 fsin_view;
layout(location = 2) in mat3 fsin_worldToTangent;
layout(location = 5) in vec3 fsin_normal;
layout(location = 6) in vec4 fsin_bitangent;
layout(location = 7) in vec4 fsin_color;
layout(location = 8) flat in uint fsin_mat;
layout(location = 0) out vec4 fsout_color;

layout(set = 2, binding = 0) uniform sampler2D globalTextures[];

struct materialData
{
	uint colorTex;
	uint normalTex;
	uint specTex;
};

layout(set = 3, binding = 0, std140) buffer materials
{
    readonly materialData matdata[];
};

float LdotNPower = 0.1;

void main()
{
    //fsout_color = vec4(1.0, 1.0, 1.0, 1.0);
	vec3 lightdir = normalize(vec3(1.0, -0.5, 0.0));
	
	vec3 viewVec = normalize(fsin_view);
	vec3 diffuseColor = texture(globalTextures[nonuniformEXT(matdata[fsin_mat].colorTex)], fsin_texcoord.xy).xyz; //vec3(0.5) * vec3(0.5);
	vec3 normalColor = texture(globalTextures[nonuniformEXT(matdata[fsin_mat].normalTex)], fsin_texcoord.xy).xyz;
	vec3 L = -lightdir;
	vec3 H = normalize(L + viewVec);
	vec3 F0 = vec3(0.5, 0.5, 0.5);
	F0 *= F0;
	
	float roughness = 1.0 - normalColor.z;
	
	vec3 normalMap;
	normalMap.xy = normalColor.xy * 2.0 - 1.0;
	normalMap.z = sqrt(1.0 - min(dot(normalMap.xy, normalMap.xy), 1.0));
	normalMap = normalize(normalMap);
	normalMap = normalize(fsin_worldToTangent * normalMap);
	
	vec3 N = (gl_FrontFacing ? normalMap : -normalMap);
	
	float LdotN = clamp(dot(N, L), 0.0, 1.0);
	float NdotV = abs(clamp(dot(viewVec, N), 0.0, 1.0));
	float NdotH = abs(clamp(dot(H, N), 0.0, 1.0));
	float VdotH = clamp(dot(H, viewVec), 0.0, 1.0);
	
	float alpha = roughness * roughness;
	float alphasquare = alpha * alpha;
	
	vec3 finalDiffuse = diffuseColor * LdotN;
	
	vec3 F = pow(1.0 - VdotH, 5) * (1.0 - F0) + F0;
	float denom = NdotH * NdotH * (alphasquare - 1.0) + 1.0;
	
	float specPower = exp2((1 - roughness) * 13.0);
    specPower = max(1.0, specPower / (specPower * 0.01 + 1.0)) * 1;
    float D = pow(NdotH, specPower) * (specPower * 0.125 + 0.25);
	
	vec3 specular = D * F * pow(LdotN, LdotNPower);
	
	vec3 diffuse = finalDiffuse * (1 - F0);
	
	vec3 direct = diffuse + specular;
	
	fsout_color = vec4(direct + diffuseColor * 0.2, 1.0);
}