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
    float4 unity_LODFade;
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
    sampler samplerunity_DitherMask [[ sampler (1) ]],
    texture2d<half, access::sample > unity_DitherMask [[ texture(0) ]] ,
    texturecube<half, access::sample > unity_SpecCube0 [[ texture(1) ]] ,
    float4 mtl_FragCoord [[ position ]],
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float4 hlslcc_FragCoord = float4(mtl_FragCoord.xyz, 1.0/mtl_FragCoord.w);
    float3 u_xlat0;
    half u_xlat16_0;
    bool u_xlatb0;
    float3 u_xlat1;
    float3 u_xlat2;
    half4 u_xlat16_2;
    half3 u_xlat16_3;
    float3 u_xlat4;
    bool u_xlatb4;
    float u_xlat12;
    half u_xlat16_15;
    u_xlat0.xy = hlslcc_FragCoord.xy * float2(0.25, 0.25);
    u_xlat16_0 = unity_DitherMask.sample(samplerunity_DitherMask, u_xlat0.xy).w;
    u_xlatb4 = 0.0<FGlobals.unity_LODFade.x;
    u_xlat4.x = (u_xlatb4) ? 1.0 : -1.0;
    u_xlat0.x = fma((-float(u_xlat16_0)), u_xlat4.x, FGlobals.unity_LODFade.x);
    u_xlatb0 = u_xlat0.x<0.0;
    if(((int(u_xlatb0) * int(0xffffffffu)))!=0){discard_fragment();}
    u_xlat0.x = input.TEXCOORD5.z;
    u_xlat0.y = input.TEXCOORD6.z;
    u_xlat0.z = input.TEXCOORD7.z;
    u_xlat12 = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat12 = rsqrt(u_xlat12);
    u_xlat0.xyz = float3(u_xlat12) * u_xlat0.xyz;
    u_xlat1.x = input.TEXCOORD5.w;
    u_xlat1.y = input.TEXCOORD6.w;
    u_xlat1.z = input.TEXCOORD7.w;
    u_xlat1.xyz = (-u_xlat1.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat12 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat12 = rsqrt(u_xlat12);
    u_xlat2.xyz = float3(u_xlat12) * u_xlat1.xyz;
    u_xlat1.xyz = fma(u_xlat1.xyz, float3(u_xlat12), float3(FGlobals._WorldSpaceLightPos0.xyz));
    u_xlat16_3.x = dot((-u_xlat2.xyz), u_xlat0.xyz);
    u_xlat16_3.x = u_xlat16_3.x + u_xlat16_3.x;
    u_xlat16_3.xyz = half3(fma(u_xlat0.xyz, (-float3(u_xlat16_3.xxx)), (-u_xlat2.xyz)));
    u_xlat12 = dot(u_xlat0.xyz, u_xlat2.xyz);
    u_xlat12 = clamp(u_xlat12, 0.0f, 1.0f);
    u_xlat0.x = dot(u_xlat0.xyz, float3(FGlobals._WorldSpaceLightPos0.xyz));
    u_xlat0.x = clamp(u_xlat0.x, 0.0f, 1.0f);
    u_xlat16_15 = half((-u_xlat12) + 1.0);
    u_xlat16_15 = u_xlat16_15 * u_xlat16_15;
    u_xlat16_15 = u_xlat16_15 * u_xlat16_15;
    u_xlat16_15 = half(fma(float(u_xlat16_15), -2.98023224e-08, 0.220916301));
    u_xlat16_2 = unity_SpecCube0.sample(samplerunity_SpecCube0, float3(u_xlat16_3.xyz), level(6.0));
    u_xlat16_3.x = u_xlat16_2.w + half(-1.0);
    u_xlat16_3.x = fma(FGlobals.unity_SpecCube0_HDR.w, u_xlat16_3.x, half(1.0));
    u_xlat16_3.x = u_xlat16_3.x * FGlobals.unity_SpecCube0_HDR.x;
    u_xlat16_3.xyz = u_xlat16_2.xyz * u_xlat16_3.xxx;
    u_xlat16_3.xyz = u_xlat16_3.xyz * half3(0.720000029, 0.720000029, 0.720000029);
    u_xlat16_3.xyz = half3(u_xlat16_15) * u_xlat16_3.xyz;
    u_xlat4.x = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat4.x = max(u_xlat4.x, 0.00100000005);
    u_xlat4.x = rsqrt(u_xlat4.x);
    u_xlat4.xyz = u_xlat4.xxx * u_xlat1.xyz;
    u_xlat4.x = dot(float3(FGlobals._WorldSpaceLightPos0.xyz), u_xlat4.xyz);
    u_xlat4.x = clamp(u_xlat4.x, 0.0f, 1.0f);
    u_xlat4.x = max(u_xlat4.x, 0.319999993);
    u_xlat4.x = u_xlat4.x * 2.50002503;
    u_xlat4.x = float(1.0) / u_xlat4.x;
    u_xlat4.x = u_xlat4.x + -9.99999975e-05;
    u_xlat4.x = fma(u_xlat4.x, 0.220916301, 0.389541864);
    u_xlat4.xyz = u_xlat4.xxx * float3(FGlobals._LightColor0.xyz);
    u_xlat0.xyz = fma(u_xlat4.xyz, u_xlat0.xxx, float3(u_xlat16_3.xyz));
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
