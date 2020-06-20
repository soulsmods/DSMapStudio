#version 450
#extension GL_EXT_shader_16bit_storage : enable
#extension GL_EXT_shader_explicit_arithmetic_types : enable
layout(location = 0) in vec4 color;
layout(location = 1) flat in uint fsin_mat;
layout(location = 0) out vec4 fsout_color;

struct materialData
{
	uint32_t color;
};

layout(set = 4, binding = 0, std140) buffer materials
{
    readonly materialData matdata[];
};

void main()
{
    uint32_t col = matdata[fsin_mat].color;
    fsout_color = vec4(col & 255, (col >> 8) & 255, (col >> 16) & 255, (col >> 24) & 255);
    //fsout_color = vec4(1.0, 1.0, 1.0, 1.0);
	fsout_color = fsout_color / 255.0f;
}