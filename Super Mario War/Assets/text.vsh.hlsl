struct Input
{
	float2 Position : POSITION;
	float2 Texture : TEXCOORD;
};

struct Output
{
	float4 Position : SV_POSITION;
	float2 Texture : TEXCOORD;
};

cbuffer Constants : register(b0)
{
	float4x4 projection;
	float4x4 transform;
	float depth;
};

Output vertex_main(Input input)
{
	float4 position = mul(transform, float4(input.Position, depth, 1));

	Output output;
	output.Position = mul(projection, position);
	output.Texture = input.Texture;

	return output;
}
