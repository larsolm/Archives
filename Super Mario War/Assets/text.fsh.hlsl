struct Input
{
	float4 Position : SV_POSITION;
	float2 Texture : TEXCOORD;
};

cbuffer Constants : register(b1)
{
	float4 fillColor;
	float4 strokeColor;
	float strokeRadius;
};

Texture2D _texture : register(t0);
SamplerState _sampler : register(s0);

float FilterWidth(float2 value)
{
  float2 width = max(abs(ddx(value)), abs(ddy(value)));
  return max(width.x, width.y);
}

float4 fragment_main(Input input) : SV_TARGET
{
	float4 stroke = strokeRadius == 0 ? fillColor : strokeColor;

	if (strokeRadius == 0)
		strokeColor = fillColor;

	float distance = _texture.Sample(_sampler, input.Texture).a;
	float width = FilterWidth(distance);

	float outerMin = 0.5 - strokeRadius - width;
	float outerMax = 0.5 - strokeRadius + width;
	float innerMin = 0.5 + strokeRadius - width;
	float innerMax = 0.5 + strokeRadius + width;
	
	if (distance < outerMin)
		discard;

	if (distance < outerMax)
		return float4(strokeColor.rgb, smoothstep(outerMin, outerMax, distance));

	if (distance < 0.6 - width)
		return strokeColor;

	if (distance < 0.6 + width)
		return lerp(strokeColor, fillColor, smoothstep(0.6 - width, 0.6 + width, distance));

	return fillColor;
}
