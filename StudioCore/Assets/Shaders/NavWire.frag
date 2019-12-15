#version 450
layout(location = 0) in vec3 normal;
layout(location = 1) in vec4 color;
layout(location = 2) in vec3 view;
layout(location = 0) out vec4 fsout_color;
void main()
{
    //fsout_color = vec4(1.0, 1.0, 1.0, 1.0);
	vec3 diffuse = vec3(clamp(abs(dot(-view, normal)), 0.0, 1.0));
	vec3 ambient = vec3(1.0, 1.0, 1.0);//color.xyz * vec3(0.3);
	//fsout_color = vec4(diffuse, 1.0);
	fsout_color = vec4(diffuse * color.xyz * 0.8 + ambient * color.xyz * 0.2, 1.0) * vec4(0.5);
}