#include <metal_stdlib>
#include <metal_texture>
using namespace metal;
#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct FGlobals_Type
{
    float3 _WorldSpaceCameraPos;
    half4 _WorldSpaceLightPos0;
    half4 unity_SpecCube0_HDR;
    half4 _LightColor0;
};

struct Mtl_FragmentIn
{
    float4 TEXCOORD5 [[ user(TEXCOORD5) ]] ;
    float4 TEXCOORD6 [[ user(TEXCOORD6) ]] ;
    float4 TEXCOORD7 [[ user(TEXCOORD7) ]] ;
};

struct Mtl_FragmentOut
{
    half4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    sampler samplerunity_SpecCube0 [[ sampler (0) ]],
    sampler samplerunity_NHxRoughness [[ sampler (1) ]],
    texture2d<float, access::sample > unity_NHxRoughness [[ texture(0) ]] ,
    texturecube<half, access::sample > unity_SpecCube0 [[ texture(1) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float3 u_xlat0;
    float3 u_xlat1;
    half4 u_xlat16_2;
    half3 u_xlat16_3;
    half3 u_xlat16_4;
    float u_xlat10;
    float u_xlat15;
    float u_xlat16;
    half u_xlat16_18;
    u_xlat0.x = input.TEXCOORD5.w;
    u_xlat0.y = input.TEXCOORD6.w;
    u_xlat0.z = input.TEXCOORD7.w;
    u_xlat0.xyz = (-u_xlat0.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat15 = rsqrt(u_xlat15);
    u_xlat0.xyz = float3(u_xlat15) * u_xlat0.xyz;
    u_xlat1.x = input.TEXCOORD5.z;
    u_xlat1.y = input.TEXCOORD6.z;
    u_xlat1.z = input.TEXCOORD7.z;
    u_xlat15 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat15 = rsqrt(u_xlat15);
    u_xlat1.xyz = float3(u_xlat15) * u_xlat1.xyz;
    u_xlat16_2.x = dot((-u_xlat0.xyz), u_xlat1.xyz);
    u_xlat16_2.x = u_xlat16_2.x + u_xlat16_2.x;
    u_xlat16_2.xyz = half3(fma(u_xlat1.xyz, (-float3(u_xlat16_2.xxx)), (-u_xlat0.xyz)));
    u_xlat16_2 = unity_SpecCube0.sample(samplerunity_SpecCube0, float3(u_xlat16_2.xyz), level(6.0));
    u_xlat16_3.x = u_xlat16_2.w + half(-1.0);
    u_xlat16_3.x = fma(FGlobals.unity_SpecCube0_HDR.w, u_xlat16_3.x, half(1.0));
    u_xlat16_3.x = u_xlat16_3.x * FGlobals.unity_SpecCube0_HDR.x;
    u_xlat16_3.xyz = u_xlat16_2.xyz * u_xlat16_3.xxx;
    u_xlat15 = dot(u_xlat0.xyz, u_xlat1.xyz);
    u_xlat16 = u_xlat15;
    u_xlat16 = clamp(u_xlat16, 0.0f, 1.0f);
    u_xlat15 = u_xlat15 + u_xlat15;
    u_xlat0.xyz = fma(u_xlat1.xyz, (-float3(u_xlat15)), u_xlat0.xyz);
    u_xlat15 = dot(u_xlat1.xyz, float3(FGlobals._WorldSpaceLightPos0.xyz));
    u_xlat15 = clamp(u_xlat15, 0.0f, 1.0f);
    u_xlat16_4.xyz = half3(float3(u_xlat15) * float3(FGlobals._LightColor0.xyz));
    u_xlat0.x = dot(u_xlat0.xyz, float3(FGlobals._WorldSpaceLightPos0.xyz));
    u_xlat0.x = u_xlat0.x * u_xlat0.x;
    u_xlat0.x = u_xlat0.x * u_xlat0.x;
    u_xlat16_18 = half((-u_xlat16) + 1.0);
    u_xlat10 = float(u_xlat16_18) * float(u_xlat16_18);
    u_xlat10 = float(u_xlat16_18) * u_xlat10;
    u_xlat10 = float(u_xlat16_18) * u_xlat10;
    u_xlat16_18 = half(fma(u_xlat10, -2.98023224e-08, 0.220916301));
    u_xlat16_3.xyz = half3(u_xlat16_18) * u_xlat16_3.xyz;
    u_xlat0.y = 1.0;
    u_xlat0.x = unity_NHxRoughness.sample(samplerunity_NHxRoughness, u_xlat0.xy).x;
    u_xlat16_18 = half(fma(u_xlat0.x, 3.53466082, 0.389541864));
    output.SV_Target0.xyz = fma(half3(u_xlat16_18), u_xlat16_4.xyz, u_xlat16_3.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
