#version 450 core

layout(binding = 0) readonly buffer VertexData
{
	float[] attributes;
};
layout(binding = 1) readonly buffer IndexData
{
	uint[] indices;
};
layout(rgba32f, binding = 0) uniform image2D pos_samples;
layout(r32ui, binding = 1) uniform uimage2D lists;
layout(r32ui, binding = 2) uniform uimage2D lists_heads;
layout(r32ui, binding = 3) uniform uimage2D shadow_map;
uniform mat4 lightTransform;

const uint LISTS_BUFFER_WIDTH = 1600;

ivec2 linearToDoubleIdx(uint idx) {
	return ivec2(idx % LISTS_BUFFER_WIDTH, idx / LISTS_BUFFER_WIDTH);
}

int orientationTest(vec2 from, vec2 to, vec2 point) {
	float expr = (to.x-from.x)*(point.y-from.y)-(point.x-from.x)*(to.y-from.y);
	return int(sign(expr));
}

vec3 triNormal(vec3 v1, vec3 v2, vec3 v3) {
	return normalize(cross(v2 - v1, v3 - v1));
}

void main() {
	uint triangle_idx = gl_PrimitiveID;
	uint vi1 = indices[triangle_idx * 3 + 0];
	uint vi2 = indices[triangle_idx * 3 + 1];
	uint vi3 = indices[triangle_idx * 3 + 2];
	vec3 v1p = vec3(attributes[vi1 * 6 + 0], attributes[vi1 * 6 + 1], attributes[vi1 * 6 + 2]);
	vec3 v2p = vec3(attributes[vi2 * 6 + 0], attributes[vi2 * 6 + 1], attributes[vi2 * 6 + 2]);
	vec3 v3p = vec3(attributes[vi3 * 6 + 0], attributes[vi3 * 6 + 1], attributes[vi3 * 6 + 2]);
	vec4 v1 = lightTransform * vec4(v1p, 1);
	vec4 v2 = lightTransform * vec4(v2p, 1);
	vec4 v3 = lightTransform * vec4(v3p, 1);

	// get triangle plane parameters
	vec3 t_normal = triNormal(v1.xyz, v2.xyz, v3.xyz);
	float D = -dot(t_normal, v1.xyz);
	
	if (abs(dot(t_normal, vec3(0.0, 0.0, 1.0))) < 0.000001) return;

	ivec2 bin_idx = ivec2(gl_FragCoord.x, gl_FragCoord.y);
	uint list_idx = imageLoad(lists_heads, bin_idx).x;
	while (list_idx != 0) {
		ivec2 coords = linearToDoubleIdx(list_idx - 1);
		list_idx = imageLoad(lists, coords).x;

		vec3 pos_sample = imageLoad(pos_samples, coords).xyz;

		int o1 = orientationTest(v1.xy, v2.xy, pos_sample.xy);
		int o2 = orientationTest(v2.xy, v3.xy, pos_sample.xy);
		int o3 = orientationTest(v3.xy, v1.xy, pos_sample.xy);

		bool op = (o1 > 0) && (o2 > 0) && (o3 > 0);
		bool on = (o1 < 0) && (o2 < 0) && (o3 < 0);

		// distance to triangle
		float d1 = -(dot(t_normal, vec3(pos_sample.xy, 0.0)) + D) / dot(t_normal, vec3(0.0, 0.0, sign(pos_sample.z)));
		
		// since the origin is seen as the source, and not the
		// sun which is veeeeeeeeeeeery far way, we manually have to
		// correct the "direction of comparison". (Yes this vague but trust)
		if (sign(pos_sample.z) > 0) {
			d1 = -d1;
		}

		// distance to pos_sample
		float d2 = -pos_sample.z;
		
		/**
		 * If o1 == o2 == o3, or not (op || on), then sample lies on
		 * triangle. in that case it is self-testing and we don't shadow
		 **/
		if ((op || on) && (d2 - d1 > 0.000001)) {
			imageStore(shadow_map, coords, uvec4(175, 0, 0, 0));
		}
	}
}
