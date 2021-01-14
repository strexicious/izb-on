#version 330 core

in vec3 f_position;

out vec3 world_pos;

void main() {
	world_pos = f_position;
}
