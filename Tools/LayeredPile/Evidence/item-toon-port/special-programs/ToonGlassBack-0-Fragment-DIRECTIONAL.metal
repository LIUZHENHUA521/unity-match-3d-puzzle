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
    float4 hlslcc_mtx4x4unity_MatrixV[4];
    float4 hlslcc_mtx4x4unity_MatrixInvV[4];
    half4 _LightColor0;
    float4 _IndirectColor;
    float4 _NormalMap_ST;
    float _NormalScale;
    float _BaseCellOffset;
    float _BaseCellSharpness;
    float4 _LightColor;
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
    float4 TEXCOORD1 [[ user(TEXCOORD1) ]] ;
    float4 TEXCOORD2 [[ user(TEXCOORD2) ]] ;
    float4 TEXCOORD3 [[ user(TEXCOORD3) ]] ;
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
    sampler sampler_AlphaTex [[ sampler (3) ]],
    sampler sampler_Matcap [[ sampler (4) ]],
    sampler sampler_RoughnessMap [[ sampler (5) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _AlphaTex [[ texture(3) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(4) ]] ,
    texture2d<half, access::sample > _RoughnessMap [[ texture(5) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float3 u_xlat0;
    half3 u_xlat16_0;
    float4 u_xlat1;
    half4 u_xlat16_1;
    float4 u_xlat2;
    half3 u_xlat16_2;
    float4 u_xlat3;
    float3 u_xlat4;
    float3 u_xlat5;
    float2 u_xlat6;
    half2 u_xlat16_6;
    float u_xlat9;
    bool u_xlatb9;
    half u_xlat16_13;
    float u_xlat15;
    float u_xlat18;
    half u_xlat16_18;
    half u_xlat16_19;
    float u_xlat22;
    half u_xlat16_22;
    u_xlat0.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_0.xy = _NormalMap.sample(sampler_NormalMap, u_xlat0.xy).yw;
    u_xlat16_1.xy = fma(u_xlat16_0.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_13 = dot(u_xlat16_1.xy, u_xlat16_1.xy);
    u_xlat16_2.xy = half2(float2(u_xlat16_1.xy) * float2(FGlobals._NormalScale));
    u_xlat16_1.x = min(u_xlat16_13, half(1.0));
    u_xlat16_1.x = (-u_xlat16_1.x) + half(1.0);
    u_xlat16_2.z = sqrt(u_xlat16_1.x);
    u_xlat16_1.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_1.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_1.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_19 = dot(u_xlat16_1.xyz, u_xlat16_1.xyz);
    u_xlat16_19 = rsqrt(u_xlat16_19);
    u_xlat16_1.xyz = half3(u_xlat16_19) * u_xlat16_1.xyz;
    u_xlat0.xy = float2(u_xlat16_1.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat0.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_1.xx), u_xlat0.xy);
    u_xlat0.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_1.zz), u_xlat0.xy);
    u_xlat0.xy = u_xlat0.xy * float2(FGlobals._ReflectScale);
    u_xlat0.xy = fma(u_xlat0.xy, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat0.xy = u_xlat0.xy + float2(0.5, 0.5);
    u_xlat16_0.xyz = _Matcap.sample(sampler_Matcap, u_xlat0.xy).xyz;
    u_xlat3.xyz = fma(float3(u_xlat16_0.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat3.xyz = fma(float3(u_xlat16_0.xyz), u_xlat3.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat0.xyz = float3(u_xlat16_0.xyz) * u_xlat3.xyz;
    u_xlat0.xyz = u_xlat0.xyz * float3(FGlobals._MatcapScale);
    u_xlat18 = dot(FGlobals._WorldSpaceLightPos0.xyz, FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat18 = rsqrt(u_xlat18);
    u_xlat3.xyz = float3(u_xlat18) * float3(FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat18 = dot(float3(u_xlat16_1.xyz), u_xlat3.xyz);
    u_xlat3.x = u_xlat18 + FGlobals._BaseCellOffset;
    u_xlat18 = u_xlat18 + FGlobals._HighLightOffset;
    u_xlat18 = u_xlat18 / FGlobals._HighLightSmooth;
    u_xlat18 = clamp(u_xlat18, 0.0f, 1.0f);
    u_xlat3.x = u_xlat3.x / FGlobals._BaseCellSharpness;
    u_xlat3.x = clamp(u_xlat3.x, 0.0f, 1.0f);
    u_xlatb9 = FGlobals._LightColor0.w==half(0.0);
    u_xlat3.x = (u_xlatb9) ? 0.0 : u_xlat3.x;
    u_xlat9 = u_xlatb9 ? -1.0 : float(0.0);
    u_xlat9 = fma(FGlobals._RimShadowScale, u_xlat9, 1.0);
    u_xlat15 = u_xlat3.x + -1.0;
    u_xlat15 = fma(FGlobals._MetcapShadowMask, u_xlat15, 1.0);
    u_xlat0.xyz = u_xlat0.xyz * float3(u_xlat15);
    u_xlat0.xyz = u_xlat0.xyz * FGlobals._MatcapColor.xyz;
    u_xlat15 = fma(u_xlat18, u_xlat18, (-u_xlat18));
    u_xlat18 = fma(FGlobals._HighLightPing, u_xlat15, u_xlat18);
    u_xlat15 = (-u_xlat3.x) + 1.0;
    u_xlat4.xyz = fma(u_xlat3.xxx, FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat3.xzw = float3(u_xlat15) * FGlobals._ShadowAddColor.xyz;
    u_xlat5.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_22 = _AOTex.sample(sampler_AOTex, u_xlat5.xy).x;
    u_xlat5.x = fma(float(u_xlat16_22), 0.305306017, 0.682171106);
    u_xlat5.x = fma(float(u_xlat16_22), u_xlat5.x, 0.0125228781);
    u_xlat22 = fma(float(u_xlat16_22), u_xlat5.x, -1.0);
    u_xlat22 = fma(FGlobals._AOScale, u_xlat22, 1.0);
    u_xlat5.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_1 = _BaseColor.sample(sampler_BaseColor, u_xlat5.xy);
    u_xlat1 = float4(u_xlat16_1) * FGlobals._BaseTint;
    u_xlat5.xyz = float3(u_xlat22) * u_xlat1.xyz;
    u_xlat3.xzw = fma(u_xlat4.xyz, u_xlat5.xyz, u_xlat3.xzw);
    u_xlat3.xzw = fma(float3(u_xlat18), FGlobals._HighLightColor.xyz, u_xlat3.xzw);
    u_xlat4.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_18 = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat4.xy).x;
    u_xlat18 = fma(FGlobals._RoughnessScale, (-float(u_xlat16_18)), 1.0);
    u_xlat3.xzw = fma(u_xlat0.xyz, float3(u_xlat18), u_xlat3.xzw);
    u_xlat0.xyz = float3(u_xlat18) * u_xlat0.xyz;
    u_xlat3.xzw = u_xlat3.xzw + u_xlat3.xzw;
    u_xlat16_2.xyz = half3(max(u_xlat3.xzw, float3(0.0, 0.0, 0.0)));
    u_xlat3.xzw = log2(float3(u_xlat16_2.xyz));
    u_xlat3.xzw = u_xlat3.xzw * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat3.xzw = exp2(u_xlat3.xzw);
    u_xlat3.xzw = fma(u_xlat3.xzw, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat3.xzw = max(u_xlat3.xzw, float3(0.0, 0.0, 0.0));
    u_xlat4.x = input.TEXCOORD1.w;
    u_xlat4.y = input.TEXCOORD2.w;
    u_xlat4.z = input.TEXCOORD3.w;
    u_xlat4.xyz = (-u_xlat4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat18 = dot(u_xlat4.xyz, u_xlat4.xyz);
    u_xlat18 = rsqrt(u_xlat18);
    u_xlat5.xyz = float3(u_xlat18) * u_xlat4.xyz;
    u_xlat4.xyz = fma((-u_xlat4.xyz), float3(u_xlat18), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat4.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat4.xyz, u_xlat5.xyz);
    u_xlat5.x = input.TEXCOORD1.z;
    u_xlat5.y = input.TEXCOORD2.z;
    u_xlat5.z = input.TEXCOORD3.z;
    u_xlat18 = dot(u_xlat5.xyz, u_xlat4.xyz);
    u_xlat18 = u_xlat18 + FGlobals._RimOffset;
    u_xlat18 = clamp(u_xlat18, 0.0f, 1.0f);
    u_xlat18 = (-u_xlat18) + 1.0;
    u_xlat18 = log2(u_xlat18);
    u_xlat18 = u_xlat18 * FGlobals._RimPower;
    u_xlat18 = exp2(u_xlat18);
    u_xlat18 = u_xlat9 * u_xlat18;
    u_xlat4.xyz = float3(u_xlat18) * FGlobals._RimColor.xyz;
    u_xlat4.xyz = u_xlat4.xyz * float3(FGlobals._RimLightMul);
    u_xlat2.xyz = fma(u_xlat4.xyz, float3(FGlobals._RimColorMul), u_xlat3.xzw);
    u_xlat0.x = max(u_xlat0.y, u_xlat0.x);
    u_xlat0.x = max(u_xlat0.z, u_xlat0.x);
    u_xlat0.x = clamp(u_xlat0.x, 0.0f, 1.0f);
    u_xlat0.x = u_xlat0.x * FGlobals._MatcapAlphaScale;
    u_xlat6.xy = fma(input.TEXCOORD0.xy, FGlobals._AlphaTex_ST.xy, FGlobals._AlphaTex_ST.zw);
    u_xlat16_6.xy = _AlphaTex.sample(sampler_AlphaTex, u_xlat6.xy).xw;
    u_xlat6.x = u_xlat1.w * float(u_xlat16_6.x);
    u_xlat0.x = fma(u_xlat6.x, float(u_xlat16_6.y), u_xlat0.x);
    u_xlat6.x = max(u_xlat4.y, u_xlat4.x);
    u_xlat6.x = max(u_xlat4.z, u_xlat6.x);
    u_xlat2.w = fma(u_xlat6.x, FGlobals._RimAlphaMul, u_xlat0.x);
    output.SV_Target0 = half4(u_xlat2);
    return output;
}
