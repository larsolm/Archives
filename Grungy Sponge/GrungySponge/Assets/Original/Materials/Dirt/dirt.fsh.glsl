#version 330

in vec2 Mask;
in vec2 Red;
in vec2 Green;
in vec2 Blue;

out vec4 fragColor

uniform sampler2D MaskObject;
uniform sampler2D RedTexture;
uniform sampler2D GreenTexture;
uniform sampler2D BlueTexture;

void main()
{
	vec4 outColor;
	vec4 mask = texture(MaskObject, Mask);

	if (mask.r > 0.5)
		outColor = texture(RedTexture, Red);

	if (mask.g > 0.5)
		outColor = texture(GreenTexture, Green);

	if (mask.b > 0.5)
		outColor =  texture(BlueTexture, Blue);

	fragColor = outColor;
}
