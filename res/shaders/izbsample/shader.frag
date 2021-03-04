#version 330 core

in vec3 f_position;

layout(location=0) out vec3 world_pos;
layout(location=1) out vec4 out_color;

void main() {
	world_pos = f_position;
	out_color = vec4(0.7, 0, 0.8, 1.0);
}
