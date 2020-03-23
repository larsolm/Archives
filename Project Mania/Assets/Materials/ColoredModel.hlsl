cbuffer Constants : register(b0)
{
	float4x4 Projection;
	float4 Camera;
}

cbuffer Constants : register(b1)
{
	float4x4 Transform;
}

struct VertexInput
{
	float3 Position : POSITION;
	float4 Color : COLOR;
};

struct PixelInput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR;
};

PixelInput VertexMain(VertexInput input)
{
	PixelInput output;

	float4 position = mul(Transform, float4(input.Position, 1));

	output.Position = mul(Projection, position);
	output.Color = input.Color;

	return output;
}

float4 FragmentMain(PixelInput input) : SV_TARGET
{
	return input.Color;
}
