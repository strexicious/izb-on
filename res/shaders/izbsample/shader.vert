#version 330 core

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;

out vec3 f_position;

uniform mat4 view;
uniform mat4 proj;

void main() {
	f_position = position;
	gl_Position = proj * view * vec4(position, 1.0);
}
