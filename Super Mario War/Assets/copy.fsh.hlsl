struct Input
{
	float4 Position : SV_POSITION;
	float2 Texture : TEXCOORD;
};

Texture2D _texture : register(t0);
SamplerState _sampler : register(s0);

float4 fragment_main(Input input) : SV_TARGET
{
	return _texture.Sample(_sampler, input.Texture);
}
