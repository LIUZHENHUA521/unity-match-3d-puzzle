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
    sampler sampler_NormalMap [[ sampler (1) ]],
    sampler sampler_BaseColor [[ sampler (2) ]],
    sampler sampler_AOTex [[ sampler (3) ]],
    sampler sampler_AlphaTex [[ sampler (4) ]],
    sampler sampler_Matcap [[ sampler (5) ]],
    sampler sampler_RoughnessMap [[ sampler (6) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _AlphaTex [[ texture(3) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(4) ]] ,
    texture2d<half, access::sample > _RoughnessMap [[ texture(5) ]] ,
    texture2d<float, access::sample > _LightTexture0 [[ texture(6) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float4 u_xlat0;
    half u_xlat16_0;
    float4 u_xlat1;
    float3 u_xlat2;
    half3 u_xlat16_2;
    half3 u_xlat16_3;
    half3 u_xlat16_4;
    half3 u_xlat16_5;
    float3 u_xlat6;
    float3 u_xlat7;
    float2 u_xlat8;
    half3 u_xlat16_8;
    float u_xlat9;
    float u_xlat17;
    half u_xlat16_19;
    float u_xlat24;
    half u_xlat16_24;
    half u_xlat16_27;
    u_xlat0.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_0 = _AOTex.sample(sampler_AOTex, u_xlat0.xy).x;
    u_xlat8.x = fma(float(u_xlat16_0), 0.305306017, 0.682171106);
    u_xlat8.x = fma(float(u_xlat16_0), u_xlat8.x, 0.0125228781);
    u_xlat0.x = fma(float(u_xlat16_0), u_xlat8.x, -1.0);
    u_xlat0.x = fma(FGlobals._AOScale, u_xlat0.x, 1.0);
    u_xlat8.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_8.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat8.xy).xyz;
    u_xlat1.xyz = fma(float3(u_xlat16_8.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat1.xyz = fma(float3(u_xlat16_8.xyz), u_xlat1.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = float3(u_xlat16_8.xyz) * u_xlat1.xyz;
    u_xlat1.w = 1.0;
    u_xlat1 = u_xlat1 * FGlobals._BaseTint;
    u_xlat0.xyz = u_xlat0.xxx * u_xlat1.xyz;
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat24 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat24 = rsqrt(u_xlat24);
    u_xlat1.xyz = float3(u_xlat24) * u_xlat1.xyz;
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_2.xy = _NormalMap.sample(sampler_NormalMap, u_xlat2.xy).yw;
    u_xlat16_3.xy = fma(u_xlat16_2.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_19 = dot(u_xlat16_3.xy, u_xlat16_3.xy);
    u_xlat16_4.xy = half2(float2(u_xlat16_3.xy) * float2(FGlobals._NormalScale));
    u_xlat16_3.x = min(u_xlat16_19, half(1.0));
    u_xlat16_3.x = (-u_xlat16_3.x) + half(1.0);
    u_xlat16_4.z = sqrt(u_xlat16_3.x);
    u_xlat16_3.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_3.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_3.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_27 = dot(u_xlat16_3.xyz, u_xlat16_3.xyz);
    u_xlat16_27 = rsqrt(u_xlat16_27);
    u_xlat16_3.xyz = half3(u_xlat16_27) * u_xlat16_3.xyz;
    u_xlat24 = dot(float3(u_xlat16_3.xyz), u_xlat1.xyz);
    u_xlat1.x = u_xlat24 + FGlobals._BaseCellOffset;
    u_xlat24 = u_xlat24 + FGlobals._HighLightOffset;
    u_xlat24 = u_xlat24 / FGlobals._HighLightSmooth;
    u_xlat24 = clamp(u_xlat24, 0.0f, 1.0f);
    u_xlat1.x = u_xlat1.x / FGlobals._BaseCellSharpness;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat2.xyz = input.TEXCOORD4.yyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1].xyz;
    u_xlat2.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0].xyz, input.TEXCOORD4.xxx, u_xlat2.xyz);
    u_xlat2.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2].xyz, input.TEXCOORD4.zzz, u_xlat2.xyz);
    u_xlat2.xyz = u_xlat2.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3].xyz;
    u_xlat9 = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat9 = _LightTexture0.sample(sampler_LightTexture0, float2(u_xlat9)).x;
    u_xlat16_4.xyz = half3(float3(u_xlat9) * float3(FGlobals._LightColor0.xyz));
    u_xlat16_5.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_4.xyz = u_xlat16_4.xyz / u_xlat16_5.xyz;
    u_xlat9 = max(float(u_xlat16_4.y), float(u_xlat16_4.x));
    u_xlat9 = max(float(u_xlat16_4.z), u_xlat9);
    u_xlat17 = u_xlat9 * u_xlat1.x;
    u_xlat2.xyz = fma(float3(u_xlat17), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat17 = fma((-u_xlat1.x), u_xlat9, 1.0);
    u_xlat1.x = fma(u_xlat1.x, u_xlat9, -1.0);
    u_xlat9 = u_xlat9 + -1.0;
    u_xlat9 = fma(FGlobals._RimShadowScale, u_xlat9, 1.0);
    u_xlat1.x = fma(FGlobals._MetcapShadowMask, u_xlat1.x, 1.0);
    u_xlat6.xyz = float3(u_xlat17) * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xyz = fma(u_xlat2.xyz, u_xlat0.xyz, u_xlat6.xyz);
    u_xlat17 = fma(u_xlat24, u_xlat24, (-u_xlat24));
    u_xlat24 = fma(FGlobals._HighLightPing, u_xlat17, u_xlat24);
    u_xlat0.xyz = fma(float3(u_xlat24), FGlobals._HighLightColor.xyz, u_xlat0.xyz);
    u_xlat2.xy = float2(u_xlat16_3.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat2.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_3.xx), u_xlat2.xy);
    u_xlat2.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_3.zz), u_xlat2.xy);
    u_xlat2.xy = u_xlat2.xy * float2(FGlobals._ReflectScale);
    u_xlat2.xy = fma(u_xlat2.xy, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat2.xy = u_xlat2.xy + float2(0.5, 0.5);
    u_xlat16_2.xyz = _Matcap.sample(sampler_Matcap, u_xlat2.xy).xyz;
    u_xlat6.xyz = fma(float3(u_xlat16_2.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat6.xyz = fma(float3(u_xlat16_2.xyz), u_xlat6.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat2.xyz = float3(u_xlat16_2.xyz) * u_xlat6.xyz;
    u_xlat2.xyz = u_xlat2.xyz * float3(FGlobals._MatcapScale);
    u_xlat2.xyz = u_xlat1.xxx * u_xlat2.xyz;
    u_xlat2.xyz = u_xlat2.xyz * FGlobals._MatcapColor.xyz;
    u_xlat1.xz = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_24 = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat1.xz).x;
    u_xlat24 = fma(FGlobals._RoughnessScale, (-float(u_xlat16_24)), 1.0);
    u_xlat0.xyz = fma(u_xlat2.xyz, float3(u_xlat24), u_xlat0.xyz);
    u_xlat2.xyz = float3(u_xlat24) * u_xlat2.xyz;
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_3.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_3.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat6.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat24 = dot(u_xlat6.xyz, u_xlat6.xyz);
    u_xlat24 = rsqrt(u_xlat24);
    u_xlat7.xyz = float3(u_xlat24) * u_xlat6.xyz;
    u_xlat6.xyz = fma((-u_xlat6.xyz), float3(u_xlat24), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat6.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat6.xyz, u_xlat7.xyz);
    u_xlat7.x = input.TEXCOORD1.z;
    u_xlat7.y = input.TEXCOORD2.z;
    u_xlat7.z = input.TEXCOORD3.z;
    u_xlat24 = dot(u_xlat7.xyz, u_xlat6.xyz);
    u_xlat24 = u_xlat24 + FGlobals._RimOffset;
    u_xlat24 = clamp(u_xlat24, 0.0f, 1.0f);
    u_xlat24 = (-u_xlat24) + 1.0;
    u_xlat24 = log2(u_xlat24);
    u_xlat24 = u_xlat24 * FGlobals._RimPower;
    u_xlat24 = exp2(u_xlat24);
    u_xlat24 = u_xlat9 * u_xlat24;
    u_xlat1.xyz = float3(u_xlat24) * FGlobals._RimColor.xyz;
    u_xlat1.xyz = u_xlat1.xyz * float3(FGlobals._RimLightMul);
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(FGlobals._RimColorMul), u_xlat0.xyz);
    u_xlat1.x = max(u_xlat1.y, u_xlat1.x);
    u_xlat1.x = max(u_xlat1.z, u_xlat1.x);
    u_xlat9 = max(u_xlat2.y, u_xlat2.x);
    u_xlat9 = max(u_xlat2.z, u_xlat9);
    u_xlat9 = clamp(u_xlat9, 0.0f, 1.0f);
    u_xlat9 = u_xlat9 * FGlobals._MatcapAlphaScale;
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._AlphaTex_ST.xy, FGlobals._AlphaTex_ST.zw);
    u_xlat16_2.xy = _AlphaTex.sample(sampler_AlphaTex, u_xlat2.xy).xw;
    u_xlat17 = fma(float(u_xlat16_2.x), 0.305306017, 0.682171106);
    u_xlat17 = fma(float(u_xlat16_2.x), u_xlat17, 0.0125228781);
    u_xlat17 = u_xlat17 * float(u_xlat16_2.x);
    u_xlat17 = u_xlat1.w * u_xlat17;
    u_xlat9 = fma(u_xlat17, float(u_xlat16_2.y), u_xlat9);
    u_xlat0.w = fma(u_xlat1.x, FGlobals._RimAlphaMul, u_xlat9);
    output.SV_Target0 = half4(u_xlat0);
    return output;
}
