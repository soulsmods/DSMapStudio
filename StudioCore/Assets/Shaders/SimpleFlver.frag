#version 450
layout(location = 0) in vec4 color;
layout(location = 0) out vec4 fsout_color;
void main()
{
    //fsout_color = vec4(1.0, 1.0, 1.0, 1.0);
	fsout_color = color;
}