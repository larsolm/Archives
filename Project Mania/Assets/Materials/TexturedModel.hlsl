cbuffer SceneConstants : register(b0)
{
	float4x4 Projection;
	float4 Camera;
}

cbuffer ModelConstants : register(b1)
{
	float4x4 Transform;
}

struct VertexInput
{
	float3 Position : POSITION;
	float3 Normal : CUSTOM0;
	float3 Tangent : CUSTOM1;
	float3 Binormal : CUSTOM2;
	float2 Coordinate : TEXCOORD;
};

struct PixelInput
{
	float4 Position : SV_POSITION;
	float3 Normal : POSITION1;
	float3 World : TEXCOORD0;
	float2 Coordinate : TEXCOORD1;
};

SamplerState _sampler : register(s0);
Texture2D _diffuseTexture : register(t0);
Texture2D _normalTexture : register(t1);
Texture2D _occlusionTexture : register(t2);
Texture2D _roughnessTexture : register(t3);
//Texture2D _metallicTexture : register(t4);

PixelInput VertexMain(VertexInput input)
{
	PixelInput output;

	float4 position = mul(Transform, float4(input.Position, 1));
	float3 normal = normalize(mul(Transform, float4(input.Normal, 0)).xyz);
	float3 tangent = normalize(mul(Transform, float4(input.Tangent, 0)).xyz);
	float3 binormal = normalize(mul(Transform, float4(input.Binormal, 0)).xyz);
	float3x3 tangentSpace = transpose(float3x3(binormal, tangent, normal));

	output.Position = mul(Projection, position);
	output.Normal = normal;
	output.World = position.xyz;
	output.Coordinate = input.Coordinate;

	return output;
}

static const float PI = 3.14159265359;

float DistributionGGX(float3 N, float3 H, float roughness)
{
    float a = roughness*roughness;
    float a2 = a*a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;

    float nom   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return nom / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float nom   = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return nom / denom;
}

float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

float3 fresnelSchlick(float cosTheta, float3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

float4 FragmentMain(PixelInput input) : SV_TARGET
{
	float3 albedo     = pow(_diffuseTexture.Sample(_sampler, input.Coordinate).rgb, float3(2.2, 2.2, 2.2));
    float metallic  = 0;//_metallicTexture.Sample(_sampler, input.Coordinate).r;
    float roughness = _roughnessTexture.Sample(_sampler, input.Coordinate).r;
    float ao        = _occlusionTexture.Sample(_sampler, input.Coordinate).r;

    float3 N;
	{
		float3 tangentNormal = _normalTexture.Sample(_sampler, input.Coordinate).rgb * 2.0 - 1.0;

		float3 Q1  = ddx(input.World);
		float3 Q2  = ddy(input.World);
		float2 st1 = ddx(input.Coordinate);
		float2 st2 = ddy(input.Coordinate);

		float3 normal   = normalize(input.Normal);
		float3 T  = normalize(Q1*st2.y - Q2*st1.y);
		float3 B  = normalize(cross(normal, T));
		float3x3 TBN = float3x3(T, B, normal);

		N = normalize(mul(TBN, tangentNormal));
	}

	//return float4(N * 0.5 + 0.5, 1);

    float3 V = normalize(Camera.xyz - input.World);

    // calculate reflectance at normal incidence; if dia-electric (like plastic) use F0 
    // of 0.04 and if it's a metal, use the albedo color as F0 (metallic workflow)    
    float3 F0 = float3(0.04, 0.04, 0.04); 
    F0 = lerp(F0, albedo, float3(metallic, metallic, metallic));

    // reflectance equation
    float3 Lo = float3(0.0, 0.0, 0.0);
	float3 Light = float3(-8.0, -8.0, -8.0);
    //for(int i = 0; i < 4; ++i) 
    {
        // calculate per-light radiance
        float3 L = normalize(Light.xyz - input.World);
        float3 H = normalize(V + L);
        float distance = length(Light.xyz - input.World);
        float attenuation = 1.0 / (distance * distance);
        float3 radiance = float3(150, 150, 150) * float3(attenuation, attenuation, attenuation);

        // Cook-Torrance BRDF
        float NDF = DistributionGGX(N, H, roughness);   
        float G   = GeometrySmith(N, V, L, roughness);      
        float3 F    = fresnelSchlick(max(dot(H, V), 0.0), F0);
           
        float3 nominator    = NDF * G * F; 
        float denominator = 4 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001; // 0.001 to prevent divide by zero.
        float3 specular = nominator / denominator;
        
        // kS is equal to Fresnel
        float3 kS = F;
        // for energy conservation, the diffuse and specular light can't
        // be above 1.0 (unless the surface emits light); to preserve this
        // relationship the diffuse component (kD) should equal 1.0 - kS.
        float3 kD = float3(1.0, 1.0, 1.0) - kS;
        // multiply kD by the inverse metalness such that only non-metals 
        // have diffuse lighting, or a linear blend if partly metal (pure metals
        // have no diffuse light).
        kD *= 1.0 - metallic;	  

        // scale light by NdotL
        float NdotL = max(dot(N, L), 0.0);        

        // add to outgoing radiance Lo
        Lo += (kD * albedo / PI + specular) * radiance * NdotL;  // note that we already multiplied the BRDF by the Fresnel (kS) so we won't multiply by kS again
    }   
    
    // ambient lighting (note that the next IBL tutorial will replace 
    // this ambient lighting with environment lighting).
    float3 ambient = float3(0.03, 0.03, 0.03) * albedo * ao;
    
    float3 color = ambient + Lo;

    // HDR tonemapping
    color = color / (color + float3(1.0, 1.0, 1.0));
    // gamma correct
	float gamma = 1.0/2.2;
    color = pow(color, float3(gamma, gamma, gamma)); 

    return float4(color, 1.0);
}
