struct Input
{
	float2 Position : POSITION0;
	float4 Color : COLOR;
};

cbuffer UniqueConstants
{
	float4x4 Transform;
	float4 BlendColor;
	float Depth;
	float3 Padding;
};

struct Output
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR;
};

Output main(Input input)
{
	Output output;

	output.Position = mul(Transform, float4(input.Position, Depth, 1));
	output.Color = input.Color * BlendColor;

	return output;
}
