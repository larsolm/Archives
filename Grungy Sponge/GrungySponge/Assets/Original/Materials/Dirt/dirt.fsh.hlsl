struct Input
{
    float4 Position : SV_POSITION;
    float2 Mask : TEXCOORD0;
    float2 Red : TEXCOORD1;
    float2 Green : TEXCOORD2;
    float2 Blue : TEXCOORD3;
};

Texture2D MaskObject;
SamplerState MaskSampler;

Texture2D RedTexture;
SamplerState RedSampler;

Texture2D GreenTexture;
SamplerState GreenSampler;

Texture2D BlueTexture;
SamplerState BlueSampler;

float4 main(Input input) : SV_TARGET
{
	float4 mask = MaskObject.Sample(MaskSampler, input.Mask);
	if (mask.r > 0.5)
		return RedTexture.Sample(RedSampler, input.Red);

	if (mask.g > 0.5)
		return GreenTexture.Sample(GreenSampler, input.Green);

	if (mask.b > 0.5)
		return BlueTexture.Sample(BlueSampler, input.Blue);

	discard;
	return float4(0, 0, 0, 0);
}
