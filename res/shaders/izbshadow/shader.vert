#version 450 core

layout(location = 0) in vec3 position;

uniform mat4 lightTransform;

void main() {
	gl_Position = lightTransform * vec4(position, 1.0);
}
