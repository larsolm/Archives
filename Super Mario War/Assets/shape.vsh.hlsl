struct Input
{
	float2 Position : POSITION;
	float2 Texture : TEXCOORD;
	float4 Color : COLOR;
};

struct Output
{
	float4 Position : SV_POSITION;
	float2 Texture : TEXCOORD;
	float4 Color : COLOR;
};

cbuffer Constants : register(b0)
{
	float4x4 projection;
	float4x4 transform;
	float4x4 textureTransform;
	float4 color;
	float depth;
};

Output vertex_main(Input input)
{
	float4 position = mul(transform, float4(input.Position, depth, 1));
	float4 coordinate = mul(textureTransform, float4(input.Texture, 0, 1));

	Output output;
	output.Position = mul(projection, position);
	output.Texture = coordinate.xy;
	output.Color = input.Color * color;

	return output;
}
