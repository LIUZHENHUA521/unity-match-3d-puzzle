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
    float4 hlslcc_mtx4x4unity_MatrixV[4];
    float4 hlslcc_mtx4x4unity_MatrixInvV[4];
    half4 _LightColor0;
    float4 hlslcc_mtx4x4unity_WorldToLight[4];
    float4 _NormalMap_ST;
    float _NormalScale;
    float _BaseCellOffset;
    float _BaseCellSharpness;
    float4 _LightColor;
    float4 _IndirectColor;
    float4 _BaseColor_ST;
    float4 _BaseTint;
    float4 _AOTex_ST;
    float _AOScale;
    float4 _AlphaTex_ST;
    float4 _ShadowAddColor;
    float _HighLightOffset;
    float _HighLightSmooth;
    float _HighLightPing;
    float4 _HighLightColor;
    float _MetcapShadowMask;
    float _ReflectScale;
    float2 _ReflectOffset;
    float _MatcapScale;
    float4 _MatcapColor;
    float4 _RoughnessMap_ST;
    float _RoughnessScale;
    float _MatcapAlphaScale;
    float _RimOffset;
    float _RimPower;
    float _RimShadowScale;
    float4 _RimColor;
    float _RimLightMul;
    float _RimAlphaMul;
    float _RimColorMul;
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
    sampler sampler_LightTextureB0 [[ sampler (1) ]],
    sampler sampler_NormalMap [[ sampler (2) ]],
    sampler sampler_BaseColor [[ sampler (3) ]],
    sampler sampler_AOTex [[ sampler (4) ]],
    sampler sampler_AlphaTex [[ sampler (5) ]],
    sampler sampler_Matcap [[ sampler (6) ]],
    sampler sampler_RoughnessMap [[ sampler (7) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _AlphaTex [[ texture(3) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(4) ]] ,
    texture2d<half, access::sample > _RoughnessMap [[ texture(5) ]] ,
    texture2d<float, access::sample > _LightTexture0 [[ texture(6) ]] ,
    texture2d<float, access::sample > _LightTextureB0 [[ texture(7) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float4 u_xlat0;
    half u_xlat16_0;
    float4 u_xlat1;
    bool u_xlatb1;
    float4 u_xlat2;
    half3 u_xlat16_2;
    half3 u_xlat16_3;
    half3 u_xlat16_4;
    float3 u_xlat5;
    float2 u_xlat6;
    half3 u_xlat16_6;
    float2 u_xlat7;
    half2 u_xlat16_7;
    float u_xlat13;
    half u_xlat16_15;
    float u_xlat18;
    float u_xlat20;
    half u_xlat16_21;
    u_xlat0.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_0 = _AOTex.sample(sampler_AOTex, u_xlat0.xy).x;
    u_xlat6.x = fma(float(u_xlat16_0), 0.305306017, 0.682171106);
    u_xlat6.x = fma(float(u_xlat16_0), u_xlat6.x, 0.0125228781);
    u_xlat0.x = fma(float(u_xlat16_0), u_xlat6.x, -1.0);
    u_xlat0.x = fma(FGlobals._AOScale, u_xlat0.x, 1.0);
    u_xlat6.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_6.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat6.xy).xyz;
    u_xlat1.xyz = fma(float3(u_xlat16_6.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat1.xyz = fma(float3(u_xlat16_6.xyz), u_xlat1.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = float3(u_xlat16_6.xyz) * u_xlat1.xyz;
    u_xlat1.w = 1.0;
    u_xlat1 = u_xlat1 * FGlobals._BaseTint;
    u_xlat0.xyz = u_xlat0.xxx * u_xlat1.xyz;
    u_xlat2 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1];
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0], input.TEXCOORD4.xxxx, u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2], input.TEXCOORD4.zzzz, u_xlat2);
    u_xlat2 = u_xlat2 + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3];
    u_xlat1.xy = u_xlat2.xy / u_xlat2.ww;
    u_xlat1.xy = u_xlat1.xy + float2(0.5, 0.5);
    u_xlat18 = _LightTexture0.sample(sampler_LightTexture0, u_xlat1.xy).w;
    u_xlatb1 = 0.0<u_xlat2.z;
    u_xlat7.x = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat7.x = _LightTextureB0.sample(sampler_LightTextureB0, u_xlat7.xx).x;
    u_xlat16_3.x = (u_xlatb1) ? half(1.0) : half(0.0);
    u_xlat16_3.x = half(u_xlat18 * float(u_xlat16_3.x));
    u_xlat16_3.x = half(u_xlat7.x * float(u_xlat16_3.x));
    u_xlat16_3.xyz = u_xlat16_3.xxx * FGlobals._LightColor0.xyz;
    u_xlat16_4.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_3.xyz = u_xlat16_3.xyz / u_xlat16_4.xyz;
    u_xlat18 = max(float(u_xlat16_3.y), float(u_xlat16_3.x));
    u_xlat18 = max(float(u_xlat16_3.z), u_xlat18);
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat2.x = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat2.x = rsqrt(u_xlat2.x);
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xxx;
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
    u_xlat1.x = dot(float3(u_xlat16_3.xyz), u_xlat1.xyz);
    u_xlat7.x = u_xlat1.x + FGlobals._BaseCellOffset;
    u_xlat1.x = u_xlat1.x + FGlobals._HighLightOffset;
    u_xlat1.x = u_xlat1.x / FGlobals._HighLightSmooth;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat7.x = u_xlat7.x / FGlobals._BaseCellSharpness;
    u_xlat7.x = clamp(u_xlat7.x, 0.0f, 1.0f);
    u_xlat13 = u_xlat18 * u_xlat7.x;
    u_xlat2.xyz = fma(float3(u_xlat13), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat13 = fma((-u_xlat7.x), u_xlat18, 1.0);
    u_xlat7.x = fma(u_xlat7.x, u_xlat18, -1.0);
    u_xlat18 = u_xlat18 + -1.0;
    u_xlat18 = fma(FGlobals._RimShadowScale, u_xlat18, 1.0);
    u_xlat7.x = fma(FGlobals._MetcapShadowMask, u_xlat7.x, 1.0);
    u_xlat5.xyz = float3(u_xlat13) * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xyz = fma(u_xlat2.xyz, u_xlat0.xyz, u_xlat5.xyz);
    u_xlat13 = fma(u_xlat1.x, u_xlat1.x, (-u_xlat1.x));
    u_xlat1.x = fma(FGlobals._HighLightPing, u_xlat13, u_xlat1.x);
    u_xlat0.xyz = fma(u_xlat1.xxx, FGlobals._HighLightColor.xyz, u_xlat0.xyz);
    u_xlat1.xz = float2(u_xlat16_3.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat1.xz = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_3.xx), u_xlat1.xz);
    u_xlat1.xz = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_3.zz), u_xlat1.xz);
    u_xlat1.xz = u_xlat1.xz * float2(FGlobals._ReflectScale);
    u_xlat1.xz = fma(u_xlat1.xz, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat1.xz = u_xlat1.xz + float2(0.5, 0.5);
    u_xlat16_2.xyz = _Matcap.sample(sampler_Matcap, u_xlat1.xz).xyz;
    u_xlat5.xyz = fma(float3(u_xlat16_2.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat5.xyz = fma(float3(u_xlat16_2.xyz), u_xlat5.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat2.xyz = float3(u_xlat16_2.xyz) * u_xlat5.xyz;
    u_xlat1.x = max(u_xlat2.y, u_xlat2.x);
    u_xlat1.x = max(u_xlat2.z, u_xlat1.x);
    u_xlat1.x = u_xlat1.x * FGlobals._MatcapScale;
    u_xlat1.x = u_xlat1.x * u_xlat7.x;
    u_xlat1.xyz = u_xlat1.xxx * FGlobals._MatcapColor.xyz;
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_2.x = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat2.xy).x;
    u_xlat2.x = fma(FGlobals._RoughnessScale, (-float(u_xlat16_2.x)), 1.0);
    u_xlat0.xyz = fma(u_xlat1.xyz, u_xlat2.xxx, u_xlat0.xyz);
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xxx;
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_3.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_3.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat2.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat20 = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat20 = rsqrt(u_xlat20);
    u_xlat5.xyz = float3(u_xlat20) * u_xlat2.xyz;
    u_xlat2.xyz = fma((-u_xlat2.xyz), float3(u_xlat20), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat2.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat2.xyz, u_xlat5.xyz);
    u_xlat5.x = input.TEXCOORD1.z;
    u_xlat5.y = input.TEXCOORD2.z;
    u_xlat5.z = input.TEXCOORD3.z;
    u_xlat2.x = dot(u_xlat5.xyz, u_xlat2.xyz);
    u_xlat2.x = u_xlat2.x + FGlobals._RimOffset;
    u_xlat2.x = clamp(u_xlat2.x, 0.0f, 1.0f);
    u_xlat2.x = (-u_xlat2.x) + 1.0;
    u_xlat2.x = log2(u_xlat2.x);
    u_xlat2.x = u_xlat2.x * FGlobals._RimPower;
    u_xlat2.x = exp2(u_xlat2.x);
    u_xlat18 = u_xlat18 * u_xlat2.x;
    u_xlat2.xyz = float3(u_xlat18) * FGlobals._RimColor.xyz;
    u_xlat2.xyz = u_xlat2.xyz * float3(FGlobals._RimLightMul);
    u_xlat0.xyz = fma(u_xlat2.xyz, float3(FGlobals._RimColorMul), u_xlat0.xyz);
    u_xlat1.x = max(u_xlat1.y, u_xlat1.x);
    u_xlat1.x = max(u_xlat1.z, u_xlat1.x);
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat1.x = u_xlat1.x * FGlobals._MatcapAlphaScale;
    u_xlat7.xy = fma(input.TEXCOORD0.xy, FGlobals._AlphaTex_ST.xy, FGlobals._AlphaTex_ST.zw);
    u_xlat16_7.xy = _AlphaTex.sample(sampler_AlphaTex, u_xlat7.xy).xw;
    u_xlat20 = fma(float(u_xlat16_7.x), 0.305306017, 0.682171106);
    u_xlat20 = fma(float(u_xlat16_7.x), u_xlat20, 0.0125228781);
    u_xlat7.x = float(u_xlat16_7.x) * u_xlat20;
    u_xlat7.x = u_xlat1.w * u_xlat7.x;
    u_xlat1.x = fma(u_xlat7.x, float(u_xlat16_7.y), u_xlat1.x);
    u_xlat7.x = max(u_xlat2.y, u_xlat2.x);
    u_xlat7.x = max(u_xlat2.z, u_xlat7.x);
    u_xlat0.w = fma(u_xlat7.x, FGlobals._RimAlphaMul, u_xlat1.x);
    output.SV_Target0 = half4(u_xlat0);
    return output;
}
