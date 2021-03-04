#version 450 core

layout(location=0) out vec4 out_color;

layout(r32ui, binding = 3) uniform uimage2D shadow_map;
layout(rgba32f, binding = 4) uniform image2D color_image;

void main() {
	float shadow_val = float(imageLoad(shadow_map, ivec2(gl_FragCoord.xy)).x)/255.0; 
	out_color = imageLoad(color_image, ivec2(gl_FragCoord.xy));
	if (shadow_val != 0) {
		out_color *= shadow_val;
	}
}
