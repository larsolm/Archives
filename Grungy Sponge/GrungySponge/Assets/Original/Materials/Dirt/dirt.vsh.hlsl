struct Input
{
    float2 Position : POSITION;
    float2 Mask : TEXCOORD0;
    float2 Red : TEXCOORD1;
    float2 Green : TEXCOORD2;
    float2 Blue : TEXCOORD3;
};

cbuffer UniqueConstants
{
    float4x4 Transform;
    float TextureScale;
    float3 Padding;
};

struct Output
{
    float4 Position : SV_POSITION;
    float2 Mask : TEXCOORD0;
    float2 Red : TEXCOORD1;
    float2 Green : TEXCOORD2;
    float2 Blue : TEXCOORD3;
};

Output main(Input input)
{
    Output output;

    output.Position = mul(Transform, float4(input.Position.x, input.Position.y, 1, 1));
    output.Mask = input.Mask;
    output.Red = float2(input.Red.x * TextureScale, input.Red.y);
    output.Green = float2(input.Green.x * TextureScale, input.Green.y);
    output.Blue = float2(input.Blue.x * TextureScale, input.Blue.y);

    return output;
}
