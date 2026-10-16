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
    half4 _WorldSpaceLightPos0;
    float4 hlslcc_mtx4x4unity_WorldToObject[4];
    float4 hlslcc_mtx4x4unity_MatrixV[4];
    float4 hlslcc_mtx4x4unity_MatrixInvV[4];
    half4 _LightColor0;
    float4 _IndirectColor;
    float4 _NormalMap_ST;
    float _NormalScale;
    float _BaseCellOffset;
    float _BaseCellSharpness;
    float4 _LightColor;
    float _ReflectScale;
    float4 _BaseTint;
    float4 _AOTex_ST;
    float _AOScale;
    float4 _ShadowAddColor;
    float _HighLightOffset;
    float _HighLightSmooth;
    float _HighLightPing;
    float4 _HighLightColor;
    float _HighLightShadowMask;
    float4 _FadeColorF;
    float4 _FadeColorB;
    float _FadeColorScale;
    float _FadeColorOffset;
    float _GradientNoiseScale;
    float _GradientNoiseOffset;
    float4 _ColorNoise1Dark;
    float4 _ColorNoise1Light;
    float _Noise1Scale;
    float3 _Noise1Offset;
    float _Noise1Draker;
    float _Noise1Lighter;
    float4 _ColorNoise2Dark;
    float4 _ColorNoise2Light;
    float _Noise2Scale;
    float3 _Noise2Offset;
    float _Noise2Draker;
    float _Noise2Lighter;
    float _MatcapShadowMask;
    float2 _ReflectOffset;
    float _MatcapScale;
    float4 _MatcapColor;
    float _RimOffset2;
    float _RimPower2;
    float4 _RimColor2;
    float _RimLightMul2;
    float _RimOffset;
    float _RimPower;
    float _RimShadowScale;
    float4 _RimColor;
    float _RimLightMul;
    float _RimNoiseScale;
    float _RimNoiseOffset;
    float4 _RefractionColorDark;
    float4 _RefractionColorLight;
    float _RefractionNoiseScale;
    float _RefractionNoiseOffset;
    float _RefractionShadowMask;
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
    sampler sampler_NormalMap [[ sampler (0) ]],
    sampler sampler_BaseColor [[ sampler (1) ]],
    sampler sampler_AOTex [[ sampler (2) ]],
    sampler sampler_GradientNoise [[ sampler (3) ]],
    sampler sampler_ColorNoise1 [[ sampler (4) ]],
    sampler sampler_ColorNoise2 [[ sampler (5) ]],
    sampler sampler_Matcap [[ sampler (6) ]],
    sampler sampler_RimNoise [[ sampler (7) ]],
    sampler sampler_RefractionNoise [[ sampler (8) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _GradientNoise [[ texture(3) ]] ,
    texture2d<half, access::sample > _ColorNoise1 [[ texture(4) ]] ,
    texture2d<half, access::sample > _ColorNoise2 [[ texture(5) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(6) ]] ,
    texture2d<half, access::sample > _RimNoise [[ texture(7) ]] ,
    texture2d<half, access::sample > _RefractionNoise [[ texture(8) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float3 u_xlat0;
    half3 u_xlat16_0;
    float4 u_xlat1;
    half3 u_xlat16_1;
    float4 u_xlat2;
    half3 u_xlat16_2;
    float4 u_xlat3;
    half3 u_xlat16_3;
    float4 u_xlat4;
    half u_xlat16_4;
    float4 u_xlat5;
    half u_xlat16_5;
    float2 u_xlat6;
    float4 u_xlat7;
    int3 u_xlati8;
    int3 u_xlati9;
    half3 u_xlat16_10;
    float u_xlat14;
    half u_xlat16_14;
    float3 u_xlat15;
    half u_xlat16_15;
    float3 u_xlat16;
    half u_xlat16_23;
    float u_xlat25;
    float u_xlat26;
    half u_xlat16_26;
    float2 u_xlat28;
    float u_xlat33;
    half u_xlat16_33;
    half u_xlat16_34;
    float u_xlat36;
    half u_xlat16_36;
    float u_xlat37;
    half u_xlat16_37;
    u_xlat0.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_0.xy = _NormalMap.sample(sampler_NormalMap, u_xlat0.xy).yw;
    u_xlat16_1.xy = fma(u_xlat16_0.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_23 = dot(u_xlat16_1.xy, u_xlat16_1.xy);
    u_xlat16_2.xy = half2(float2(u_xlat16_1.xy) * float2(FGlobals._NormalScale));
    u_xlat16_1.x = min(u_xlat16_23, half(1.0));
    u_xlat16_1.x = (-u_xlat16_1.x) + half(1.0);
    u_xlat16_2.z = sqrt(u_xlat16_1.x);
    u_xlat16_1.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_1.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_1.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_34 = dot(u_xlat16_1.xyz, u_xlat16_1.xyz);
    u_xlat16_34 = rsqrt(u_xlat16_34);
    u_xlat16_1.xyz = half3(u_xlat16_34) * u_xlat16_1.xyz;
    u_xlat0.xy = float2(u_xlat16_1.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat0.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_1.xx), u_xlat0.xy);
    u_xlat0.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_1.zz), u_xlat0.xy);
    u_xlat0.xy = u_xlat0.xy * float2(FGlobals._ReflectScale);
    u_xlat0.xy = fma(u_xlat0.xy, float2(0.5, 0.5), float2(0.5, 0.5));
    u_xlat16_3.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat0.xy).xyz;
    u_xlat0.xy = u_xlat0.xy + FGlobals._ReflectOffset.xyxx.xy;
    u_xlat16_0.xyz = _Matcap.sample(sampler_Matcap, u_xlat0.xy).xyz;
    u_xlat4.xyz = fma(float3(u_xlat16_3.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat4.xyz = fma(float3(u_xlat16_3.xyz), u_xlat4.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xyz = float3(u_xlat16_3.xyz) * u_xlat4.xyz;
    u_xlat4.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat4.xyz = fma(FGlobals._BaseTint.xyz, u_xlat4.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat4.xyz = u_xlat4.xyz * FGlobals._BaseTint.xyz;
    u_xlat3.xyz = u_xlat3.xyz * u_xlat4.xyz;
    u_xlat4.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_33 = _AOTex.sample(sampler_AOTex, u_xlat4.xy).x;
    u_xlat36 = fma(float(u_xlat16_33), 0.305306017, 0.682171106);
    u_xlat36 = fma(float(u_xlat16_33), u_xlat36, 0.0125228781);
    u_xlat33 = fma(float(u_xlat16_33), u_xlat36, -1.0);
    u_xlat33 = fma(FGlobals._AOScale, u_xlat33, 1.0);
    u_xlat3.xyz = float3(u_xlat33) * u_xlat3.xyz;
    u_xlat33 = dot(FGlobals._WorldSpaceLightPos0.xyz, FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat33 = rsqrt(u_xlat33);
    u_xlat4.xyz = float3(u_xlat33) * float3(FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat33 = dot(float3(u_xlat16_1.xyz), u_xlat4.xyz);
    u_xlat36 = u_xlat33 + FGlobals._BaseCellOffset;
    u_xlat33 = u_xlat33 + FGlobals._HighLightOffset;
    u_xlat33 = u_xlat33 / FGlobals._HighLightSmooth;
    u_xlat33 = clamp(u_xlat33, 0.0f, 1.0f);
    u_xlat36 = u_xlat36 / FGlobals._BaseCellSharpness;
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlat16_1.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_1.xyz = FGlobals._LightColor0.xyz / u_xlat16_1.xyz;
    u_xlat4.x = max(float(u_xlat16_1.y), float(u_xlat16_1.x));
    u_xlat4.x = max(float(u_xlat16_1.z), u_xlat4.x);
    u_xlat15.x = u_xlat36 * u_xlat4.x;
    u_xlat15.xyz = fma(u_xlat15.xxx, FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat5.x = fma((-u_xlat36), u_xlat4.x, 1.0);
    u_xlat36 = fma(u_xlat36, u_xlat4.x, -1.0);
    u_xlat5.xyz = u_xlat5.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat3.xyz = fma(u_xlat15.xyz, u_xlat3.xyz, u_xlat5.xyz);
    u_xlat4.x = fma(u_xlat33, u_xlat33, (-u_xlat33));
    u_xlat33 = fma(FGlobals._HighLightPing, u_xlat4.x, u_xlat33);
    u_xlat4.xyz = float3(u_xlat33) * FGlobals._HighLightColor.xyz;
    u_xlat33 = fma(FGlobals._HighLightShadowMask, u_xlat36, 1.0);
    u_xlat3.xyz = fma(u_xlat4.xyz, float3(u_xlat33), u_xlat3.xyz);
    u_xlat3.xyz = u_xlat3.xyz + u_xlat3.xyz;
    u_xlat4.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat1 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].zyxz;
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].zyxz, input.TEXCOORD4.xxxx, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].zyxz, input.TEXCOORD4.zzzz, u_xlat1);
    u_xlat1 = u_xlat1 + FGlobals.hlslcc_mtx4x4unity_WorldToObject[3].zyxz;
    u_xlat33 = u_xlat1.y * FGlobals._FadeColorScale;
    u_xlat33 = fma(u_xlat33, 0.5, FGlobals._FadeColorOffset);
    u_xlat2 = u_xlat1.wyzw + float4(FGlobals._GradientNoiseOffset);
    u_xlat5 = u_xlat2 * float4(FGlobals._GradientNoiseScale);
    u_xlat6.xy = u_xlat2.zy * float2(FGlobals._GradientNoiseScale);
    u_xlat7.xyz = input.TEXCOORD2.zzz * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].xyz;
    u_xlat7.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].xyz, input.TEXCOORD1.zzz, u_xlat7.xyz);
    u_xlat7.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].xyz, input.TEXCOORD3.zzz, u_xlat7.xyz);
    u_xlat37 = dot(u_xlat7.xyz, u_xlat7.xyz);
    u_xlat37 = rsqrt(u_xlat37);
    u_xlat7.xyz = float3(u_xlat37) * u_xlat7.xyz;
    u_xlati8.xyz = int3(uint3((float3(0.0, 0.0, 0.0)<u_xlat7.xyz)) * 0xFFFFFFFFu);
    u_xlati9.xyz = int3(uint3((u_xlat7.xyz<float3(0.0, 0.0, 0.0))) * 0xFFFFFFFFu);
    u_xlati8.xyz = (-u_xlati8.xyz) + u_xlati9.xyz;
    u_xlat2.xyz = float3(u_xlati8.xyz);
    u_xlat2.w = 1.0;
    u_xlat5 = u_xlat2.xwyw * u_xlat5;
    u_xlat16_37 = _GradientNoise.sample(sampler_GradientNoise, u_xlat5.xy).x;
    u_xlat16_5 = _GradientNoise.sample(sampler_GradientNoise, u_xlat5.zw).x;
    u_xlat16.x = abs(u_xlat7.y) + abs(u_xlat7.x);
    u_xlat16.x = abs(u_xlat7.z) + u_xlat16.x;
    u_xlat16.x = u_xlat16.x + 9.99999975e-06;
    u_xlat16.xyz = abs(u_xlat7.xyz) / u_xlat16.xxx;
    u_xlat5.x = u_xlat16.y * float(u_xlat16_5);
    u_xlat37 = fma(float(u_xlat16_37), u_xlat16.x, u_xlat5.x);
    u_xlat28.xy = u_xlat2.zw * float2(-1.0, 1.0);
    u_xlat6.xy = u_xlat28.xy * u_xlat6.xy;
    u_xlat16_5 = _GradientNoise.sample(sampler_GradientNoise, u_xlat6.xy).x;
    u_xlat37 = fma(float(u_xlat16_5), u_xlat16.z, u_xlat37);
    u_xlat33 = u_xlat33 + u_xlat37;
    u_xlat33 = u_xlat33 + 0.5;
    u_xlat33 = clamp(u_xlat33, 0.0f, 1.0f);
    u_xlat4.xyz = fma(float3(u_xlat33), u_xlat4.xyz, FGlobals._FadeColorF.xyz);
    u_xlat3.xyz = u_xlat3.xyz * u_xlat4.xyz;
    u_xlat16_10.xyz = half3(max(u_xlat3.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat3.xyz = log2(float3(u_xlat16_10.xyz));
    u_xlat3.xyz = u_xlat3.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat3.xyz = exp2(u_xlat3.xyz);
    u_xlat3.xyz = fma(u_xlat3.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat3.xyz = max(u_xlat3.xyz, float3(0.0, 0.0, 0.0));
    u_xlat33 = FGlobals._Noise1Draker + 0.00999999978;
    u_xlat4.x = (-FGlobals._Noise1Lighter) + 1.0;
    u_xlat33 = max(u_xlat33, u_xlat4.x);
    u_xlat33 = u_xlat33 + (-FGlobals._Noise1Draker);
    u_xlat4 = u_xlat1.wyzw + FGlobals._Noise1Offset.xxyz.wzyw;
    u_xlat7 = u_xlat4 * float4(FGlobals._Noise1Scale);
    u_xlat4.xy = u_xlat4.zy * float2(FGlobals._Noise1Scale);
    u_xlat4.xy = u_xlat28.xy * u_xlat4.xy;
    u_xlat16_4 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat4.xy).x;
    u_xlat7 = u_xlat2.xwyw * u_xlat7;
    u_xlat16_15 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat7.xy).x;
    u_xlat16_26 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat7.zw).x;
    u_xlat26 = u_xlat16.y * float(u_xlat16_26);
    u_xlat15.x = fma(float(u_xlat16_15), u_xlat16.x, u_xlat26);
    u_xlat4.x = fma(float(u_xlat16_4), u_xlat16.z, u_xlat15.x);
    u_xlat15.x = fma(u_xlat4.x, 0.305306017, 0.682171106);
    u_xlat15.x = fma(u_xlat4.x, u_xlat15.x, 0.0125228781);
    u_xlat4.x = fma(u_xlat4.x, u_xlat15.x, (-FGlobals._Noise1Draker));
    u_xlat33 = u_xlat4.x / u_xlat33;
    u_xlat33 = clamp(u_xlat33, 0.0f, 1.0f);
    u_xlat4.xyz = (-FGlobals._ColorNoise1Dark.xyz) + FGlobals._ColorNoise1Light.xyz;
    u_xlat4.xyz = fma(float3(u_xlat33), u_xlat4.xyz, FGlobals._ColorNoise1Dark.xyz);
    u_xlat3.xyz = u_xlat3.xyz * u_xlat4.xyz;
    u_xlat4.xyz = fma(float3(u_xlat16_0.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat4.xyz = fma(float3(u_xlat16_0.xyz), u_xlat4.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat0.xyz = float3(u_xlat16_0.xyz) * u_xlat4.xyz;
    u_xlat0.xyz = u_xlat0.xyz * float3(FGlobals._MatcapScale);
    u_xlat33 = fma(FGlobals._MatcapShadowMask, u_xlat36, 1.0);
    u_xlat0.xyz = u_xlat0.xyz * float3(u_xlat33);
    u_xlat0.xyz = u_xlat0.xyz * FGlobals._MatcapColor.xyz;
    u_xlat33 = FGlobals._Noise2Draker + 0.00999999978;
    u_xlat4.x = (-FGlobals._Noise2Lighter) + 1.0;
    u_xlat33 = max(u_xlat33, u_xlat4.x);
    u_xlat33 = u_xlat33 + (-FGlobals._Noise2Draker);
    u_xlat4 = u_xlat1.wyzw + FGlobals._Noise2Offset.xxyz.wzyw;
    u_xlat7 = u_xlat4 * float4(FGlobals._Noise2Scale);
    u_xlat4.xy = u_xlat4.zy * float2(FGlobals._Noise2Scale);
    u_xlat4.xy = u_xlat28.xy * u_xlat4.xy;
    u_xlat16_4 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat4.xy).y;
    u_xlat7 = u_xlat2.xwyw * u_xlat7;
    u_xlat16_15 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat7.xy).y;
    u_xlat16_26 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat7.zw).y;
    u_xlat26 = u_xlat16.y * float(u_xlat16_26);
    u_xlat15.x = fma(float(u_xlat16_15), u_xlat16.x, u_xlat26);
    u_xlat4.x = fma(float(u_xlat16_4), u_xlat16.z, u_xlat15.x);
    u_xlat15.x = fma(u_xlat4.x, 0.305306017, 0.682171106);
    u_xlat15.x = fma(u_xlat4.x, u_xlat15.x, 0.0125228781);
    u_xlat4.x = fma(u_xlat4.x, u_xlat15.x, (-FGlobals._Noise2Draker));
    u_xlat33 = u_xlat4.x / u_xlat33;
    u_xlat33 = clamp(u_xlat33, 0.0f, 1.0f);
    u_xlat4.xyz = (-FGlobals._ColorNoise2Dark.xyz) + FGlobals._ColorNoise2Light.xyz;
    u_xlat4.xyz = fma(float3(u_xlat33), u_xlat4.xyz, FGlobals._ColorNoise2Dark.xyz);
    u_xlat0.xyz = fma(u_xlat3.xyz, u_xlat4.xyz, u_xlat0.xyz);
    u_xlat16_10.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_10.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat4 = u_xlat1.wyzw + float4(FGlobals._RimNoiseOffset);
    u_xlat1 = u_xlat1 + float4(FGlobals._RefractionNoiseOffset);
    u_xlat7 = u_xlat4 * float4(FGlobals._RimNoiseScale);
    u_xlat3.xy = u_xlat4.zy * float2(FGlobals._RimNoiseScale);
    u_xlat3.xy = u_xlat28.xy * u_xlat3.xy;
    u_xlat16_33 = _RimNoise.sample(sampler_RimNoise, u_xlat3.xy).x;
    u_xlat4 = u_xlat2.xwyw * u_xlat7;
    u_xlat16_3.x = _RimNoise.sample(sampler_RimNoise, u_xlat4.xy).x;
    u_xlat16_14 = _RimNoise.sample(sampler_RimNoise, u_xlat4.zw).x;
    u_xlat14 = u_xlat16.y * float(u_xlat16_14);
    u_xlat3.x = fma(float(u_xlat16_3.x), u_xlat16.x, u_xlat14);
    u_xlat33 = fma(float(u_xlat16_33), u_xlat16.z, u_xlat3.x);
    u_xlat3.x = fma(FGlobals._RimShadowScale, u_xlat36, 1.0);
    u_xlat14 = fma(FGlobals._RefractionShadowMask, u_xlat36, 1.0);
    u_xlat4.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat25 = dot(u_xlat4.xyz, u_xlat4.xyz);
    u_xlat25 = rsqrt(u_xlat25);
    u_xlat7.xyz = float3(u_xlat25) * u_xlat4.xyz;
    u_xlat4.xyz = fma((-u_xlat4.xyz), float3(u_xlat25), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat4.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat4.xyz, u_xlat7.xyz);
    u_xlat7.x = input.TEXCOORD1.z;
    u_xlat7.y = input.TEXCOORD2.z;
    u_xlat7.z = input.TEXCOORD3.z;
    u_xlat25 = dot(u_xlat7.xyz, u_xlat4.xyz);
    u_xlat36 = u_xlat25 + FGlobals._RimOffset;
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlat36 = (-u_xlat36) + 1.0;
    u_xlat36 = log2(u_xlat36);
    u_xlat36 = u_xlat36 * FGlobals._RimPower;
    u_xlat36 = exp2(u_xlat36);
    u_xlat3.x = u_xlat3.x * u_xlat36;
    u_xlat4.xyz = u_xlat3.xxx * FGlobals._RimColor.xyz;
    u_xlat4.xyz = u_xlat4.xyz * float3(FGlobals._RimLightMul);
    u_xlat4.xyz = float3(u_xlat33) * u_xlat4.xyz;
    u_xlat33 = u_xlat25 + FGlobals._RimOffset2;
    u_xlat33 = clamp(u_xlat33, 0.0f, 1.0f);
    u_xlat33 = (-u_xlat33) + 1.0;
    u_xlat33 = log2(u_xlat33);
    u_xlat33 = u_xlat33 * FGlobals._RimPower2;
    u_xlat33 = exp2(u_xlat33);
    u_xlat7.xyz = float3(u_xlat33) * FGlobals._RimColor2.xyz;
    u_xlat4.xyz = fma(u_xlat7.xyz, float3(FGlobals._RimLightMul2), u_xlat4.xyz);
    u_xlat7 = u_xlat1 * float4(FGlobals._RefractionNoiseScale);
    u_xlat3.xw = u_xlat1.zy * float2(FGlobals._RefractionNoiseScale);
    u_xlat3.xw = u_xlat28.xy * u_xlat3.xw;
    u_xlat16_33 = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat3.xw).x;
    u_xlat1 = u_xlat2.xwyw * u_xlat7;
    u_xlat16_3.x = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat1.xy).x;
    u_xlat16_36 = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat1.zw).x;
    u_xlat36 = u_xlat16.y * float(u_xlat16_36);
    u_xlat3.x = fma(float(u_xlat16_3.x), u_xlat16.x, u_xlat36);
    u_xlat33 = fma(float(u_xlat16_33), u_xlat16.z, u_xlat3.x);
    u_xlat5.xyz = (-FGlobals._RefractionColorDark.xyz) + FGlobals._RefractionColorLight.xyz;
    u_xlat5.xyz = fma(float3(u_xlat33), u_xlat5.xyz, FGlobals._RefractionColorDark.xyz);
    u_xlat3.xzw = float3(u_xlat25) * u_xlat5.xyz;
    u_xlat3.xyz = fma(u_xlat3.xzw, float3(u_xlat14), u_xlat4.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat3.xyz;
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
