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
    float4 unity_OrthoParams;
    float4 _WorldSpaceLightPos0;
    float4 hlslcc_mtx4x4unity_WorldToObject[4];
    float4 hlslcc_mtx4x4unity_MatrixV[4];
    float4 hlslcc_mtx4x4unity_MatrixInvV[4];
    float4 hlslcc_mtx4x4unity_WorldToLight[4];
    half4 _LightColor0;
    float4 _IndirectColor;
    float4 _NormalMap_ST;
    float _NormalScale;
    float _BaseCellOffset;
    float _BaseCellSharpness;
    float4 _LightColor;
    float _inverseLightOffset;
    float _inverseLightSmooth;
    float4 _inverseColor;
    float4 _BaseColor_ST;
    float4 _BaseTint;
    float4 _AOTex_ST;
    float _AOScale;
    float4 _FadeColorF;
    float4 _FadeColorB;
    float _FadeColorScale;
    float _FadeColorOffset;
    float4 _ShadowAddColor;
    float _HighLightOffset;
    float _HighLightSmooth;
    float _HighLightPing;
    float4 _HighLightColor;
    float _HighLightShadowMask;
    float _MetcapShadowMask;
    float _ReflectScale;
    float2 _ReflectOffset;
    float _MatcapScale;
    float4 _MatcapColor;
    float4 _RoughnessMap_ST;
    float _RoughnessScale;
    float _RimOffset;
    float _RimPower;
    float _RimShadowScale;
    float4 _RimColor;
    float _RimLightMul;
};

struct Mtl_FragmentIn
{
    float2 TEXCOORD0 [[ user(TEXCOORD0) ]] ;
    float3 TEXCOORD1 [[ user(TEXCOORD1) ]] ;
    float3 TEXCOORD2 [[ user(TEXCOORD2) ]] ;
    float3 TEXCOORD3 [[ user(TEXCOORD3) ]] ;
    float3 TEXCOORD4 [[ user(TEXCOORD4) ]] ;
};

struct Mtl_FragmentOut
{
    half4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    sampler sampler_LightTexture0 [[ sampler (0) ]],
    sampler sampler_NormalMap [[ sampler (1) ]],
    sampler sampler_BaseColor [[ sampler (2) ]],
    sampler sampler_AOTex [[ sampler (3) ]],
    sampler sampler_Matcap [[ sampler (4) ]],
    sampler sampler_RoughnessMap [[ sampler (5) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(3) ]] ,
    texture2d<half, access::sample > _RoughnessMap [[ texture(4) ]] ,
    texture2d<float, access::sample > _LightTexture0 [[ texture(5) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float3 u_xlat0;
    half3 u_xlat16_0;
    float4 u_xlat1;
    half3 u_xlat16_1;
    float4 u_xlat2;
    half2 u_xlat16_2;
    half3 u_xlat16_3;
    half3 u_xlat16_4;
    half3 u_xlat16_5;
    float3 u_xlat7;
    float u_xlat13;
    half u_xlat16_15;
    float u_xlat18;
    half u_xlat16_18;
    float u_xlat19;
    half u_xlat16_19;
    half u_xlat16_21;
    u_xlat0.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_0.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat0.xy).xyz;
    u_xlat1.xyz = fma(float3(u_xlat16_0.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat1.xyz = fma(float3(u_xlat16_0.xyz), u_xlat1.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat0.xyz = float3(u_xlat16_0.xyz) * u_xlat1.xyz;
    u_xlat1.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat1.xyz = fma(FGlobals._BaseTint.xyz, u_xlat1.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = u_xlat1.xyz * FGlobals._BaseTint.xyz;
    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xyz;
    u_xlat1.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_18 = _AOTex.sample(sampler_AOTex, u_xlat1.xy).x;
    u_xlat1.x = fma(float(u_xlat16_18), 0.305306017, 0.682171106);
    u_xlat1.x = fma(float(u_xlat16_18), u_xlat1.x, 0.0125228781);
    u_xlat18 = fma(float(u_xlat16_18), u_xlat1.x, -1.0);
    u_xlat18 = fma(FGlobals._AOScale, u_xlat18, 1.0);
    u_xlat0.xyz = float3(u_xlat18) * u_xlat0.xyz;
    u_xlat18 = input.TEXCOORD4.y * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].y;
    u_xlat18 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].y, input.TEXCOORD4.x, u_xlat18);
    u_xlat18 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].y, input.TEXCOORD4.z, u_xlat18);
    u_xlat18 = u_xlat18 + FGlobals.hlslcc_mtx4x4unity_WorldToObject[3].y;
    u_xlat18 = u_xlat18 * FGlobals._FadeColorScale;
    u_xlat18 = fma(u_xlat18, 0.5, FGlobals._FadeColorOffset);
    u_xlat18 = u_xlat18 + 0.5;
    u_xlat18 = clamp(u_xlat18, 0.0f, 1.0f);
    u_xlat1.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat1.xyz = fma(float3(u_xlat18), u_xlat1.xyz, FGlobals._FadeColorF.xyz);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xyz;
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat18 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat18 = rsqrt(u_xlat18);
    u_xlat1.xyz = float3(u_xlat18) * u_xlat1.xyz;
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_2.xy = _NormalMap.sample(sampler_NormalMap, u_xlat2.xy).yw;
    u_xlat16_3.xy = fma(u_xlat16_2.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_15 = dot(u_xlat16_3.xy, u_xlat16_3.xy);
    u_xlat16_4.xy = half2(float2(u_xlat16_3.xy) * float2(FGlobals._NormalScale));
    u_xlat16_3.x = min(u_xlat16_15, half(1.0));
    u_xlat16_3.x = (-u_xlat16_3.x) + half(1.0);
    u_xlat16_4.z = sqrt(u_xlat16_3.x);
    u_xlat16_3.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_3.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_3.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_21 = dot(u_xlat16_3.xyz, u_xlat16_3.xyz);
    u_xlat16_21 = rsqrt(u_xlat16_21);
    u_xlat16_3.xyz = half3(u_xlat16_21) * u_xlat16_3.xyz;
    u_xlat18 = dot(float3(u_xlat16_3.xyz), u_xlat1.xyz);
    u_xlat1.x = (-u_xlat18) + FGlobals._inverseLightOffset;
    u_xlat1.x = u_xlat1.x / FGlobals._inverseLightSmooth;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat7.xyz = input.TEXCOORD4.yyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1].xyz;
    u_xlat7.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0].xyz, input.TEXCOORD4.xxx, u_xlat7.xyz);
    u_xlat7.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2].xyz, input.TEXCOORD4.zzz, u_xlat7.xyz);
    u_xlat7.xyz = u_xlat7.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3].xyz;
    u_xlat7.x = dot(u_xlat7.xyz, u_xlat7.xyz);
    u_xlat7.x = _LightTexture0.sample(sampler_LightTexture0, u_xlat7.xx).x;
    u_xlat16_4.xyz = half3(u_xlat7.xxx * float3(FGlobals._LightColor0.xyz));
    u_xlat16_5.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_4.xyz = u_xlat16_4.xyz / u_xlat16_5.xyz;
    u_xlat7.x = max(float(u_xlat16_4.y), float(u_xlat16_4.x));
    u_xlat7.x = max(float(u_xlat16_4.z), u_xlat7.x);
    u_xlat13 = u_xlat18 + FGlobals._BaseCellOffset;
    u_xlat18 = u_xlat18 + FGlobals._HighLightOffset;
    u_xlat18 = u_xlat18 / FGlobals._HighLightSmooth;
    u_xlat18 = clamp(u_xlat18, 0.0f, 1.0f);
    u_xlat13 = u_xlat13 / FGlobals._BaseCellSharpness;
    u_xlat13 = clamp(u_xlat13, 0.0f, 1.0f);
    u_xlat19 = u_xlat7.x * u_xlat13;
    u_xlat2.xyz = fma(float3(u_xlat19), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat2.xyz = fma(u_xlat1.xxx, FGlobals._inverseColor.xyz, u_xlat2.xyz);
    u_xlat1.x = fma((-u_xlat13), u_xlat7.x, 1.0);
    u_xlat7.x = fma(u_xlat13, u_xlat7.x, -1.0);
    u_xlat1.xzw = u_xlat1.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xyz = fma(u_xlat2.xyz, u_xlat0.xyz, u_xlat1.xzw);
    u_xlat1.x = fma(u_xlat18, u_xlat18, (-u_xlat18));
    u_xlat18 = fma(FGlobals._HighLightPing, u_xlat1.x, u_xlat18);
    u_xlat1.xzw = float3(u_xlat18) * FGlobals._HighLightColor.xyz;
    u_xlat2.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), u_xlat7.xx, float2(1.0, 1.0));
    u_xlat18 = fma(FGlobals._RimShadowScale, u_xlat7.x, 1.0);
    u_xlat0.xyz = fma(u_xlat1.xzw, u_xlat2.xxx, u_xlat0.xyz);
    u_xlat1.xy = float2(u_xlat16_3.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat1.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_3.xx), u_xlat1.xy);
    u_xlat1.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_3.zz), u_xlat1.xy);
    u_xlat1.xy = u_xlat1.xy * float2(FGlobals._ReflectScale);
    u_xlat1.xy = fma(u_xlat1.xy, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat1.xy = u_xlat1.xy + float2(0.5, 0.5);
    u_xlat16_1.xyz = _Matcap.sample(sampler_Matcap, u_xlat1.xy).xyz;
    u_xlat2.xzw = fma(float3(u_xlat16_1.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat2.xzw = fma(float3(u_xlat16_1.xyz), u_xlat2.xzw, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = float3(u_xlat16_1.xyz) * u_xlat2.xzw;
    u_xlat1.xyz = u_xlat1.xyz * float3(FGlobals._MatcapScale);
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.yyy;
    u_xlat1.xyz = u_xlat1.xyz * FGlobals._MatcapColor.xyz;
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_19 = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat2.xy).x;
    u_xlat19 = fma(FGlobals._RoughnessScale, (-float(u_xlat16_19)), 1.0);
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(u_xlat19), u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_3.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_3.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat19 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat19 = rsqrt(u_xlat19);
    u_xlat2.xyz = float3(u_xlat19) * u_xlat1.xyz;
    u_xlat1.xyz = fma((-u_xlat1.xyz), float3(u_xlat19), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat1.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat1.xyz, u_xlat2.xyz);
    u_xlat2.x = input.TEXCOORD1.z;
    u_xlat2.y = input.TEXCOORD2.z;
    u_xlat2.z = input.TEXCOORD3.z;
    u_xlat1.x = dot(u_xlat2.xyz, u_xlat1.xyz);
    u_xlat1.x = u_xlat1.x + FGlobals._RimOffset;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat1.x = (-u_xlat1.x) + 1.0;
    u_xlat1.x = log2(u_xlat1.x);
    u_xlat1.x = u_xlat1.x * FGlobals._RimPower;
    u_xlat1.x = exp2(u_xlat1.x);
    u_xlat18 = u_xlat18 * u_xlat1.x;
    u_xlat1.xyz = float3(u_xlat18) * FGlobals._RimColor.xyz;
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(FGlobals._RimLightMul), u_xlat0.xyz);
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
