#version 330 core

in vec3 f_position;

layout(location = 0) out vec3 out_position;

void main() {
	out_position = f_position;
}
