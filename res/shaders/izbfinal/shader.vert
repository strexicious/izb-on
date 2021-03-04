#version 450 core

const vec4[6] QUAD_POSITIONS = {
	vec4(-1.0,  1.0, 0.5, 1.0),
	vec4( 1.0,  1.0, 0.5, 1.0),
	vec4( 1.0, -1.0, 0.5, 1.0),
	vec4(-1.0,  1.0, 0.5, 1.0),
	vec4( 1.0, -1.0, 0.5, 1.0),
	vec4(-1.0, -1.0, 0.5, 1.0),
};

void main() {
	gl_Position = QUAD_POSITIONS[gl_VertexID];
}
