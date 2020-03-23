struct Input
{
	float3 Position : POSITION;
	float2 Texture : TEXCOORD;
};

struct Output
{
	float4 Position : SV_POSITION;
	float2 Texture : TEXCOORD;
};

Output vertex_main(Input input)
{
	Output output;
	output.Position = float4(input.Position, 1);
	output.Texture = input.Texture;

	return output;
}
