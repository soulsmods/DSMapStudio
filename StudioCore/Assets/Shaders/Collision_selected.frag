#version 450
layout(location = 0) in vec3 normal;
layout(location = 1) in vec4 color;
layout(location = 2) in vec3 view;
layout(location = 0) out vec4 fsout_color;

void main()
{
	fsout_color = vec4(1.0, 0.5, 0.0, 1.0);
}