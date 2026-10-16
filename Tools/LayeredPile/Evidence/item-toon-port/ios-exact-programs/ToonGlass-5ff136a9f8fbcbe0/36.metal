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

struct UnityDrawCallInfo_Type
{
    int unity_BaseInstanceID;
    int unity_InstanceCount;
};

struct unity_Builtins2Array_Type
{
    half4 unity_SHArArray;
    half4 unity_SHAgArray;
    half4 unity_SHAbArray;
    half4 unity_SHBrArray;
    half4 unity_SHBgArray;
    half4 unity_SHBbArray;
    half4 unity_SHCArray;
};

struct UnityInstancing_PerDraw2_Type
{
    unity_Builtins2Array_Type unity_Builtins2Array[2];
};

struct Mtl_FragmentIn
{
    float2 TEXCOORD0 [[ user(TEXCOORD0) ]] ;
    float4 TEXCOORD1 [[ user(TEXCOORD1) ]] ;
    float4 TEXCOORD2 [[ user(TEXCOORD2) ]] ;
    float4 TEXCOORD3 [[ user(TEXCOORD3) ]] ;
    uint SV_InstanceID0 [[ user(SV_InstanceID0) ]]  [[ flat ]];
};

struct Mtl_FragmentOut
{
    half4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    constant UnityDrawCallInfo_Type& UnityDrawCallInfo [[ buffer(1) ]],
    const constant unity_Builtins2Array_Type* UnityInstancing_PerDraw2 [[ buffer(2) ]],
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
    float4 u_xlat0;
    half u_xlat16_0;
    float4 u_xlat1;
    float3 u_xlat2;
    half3 u_xlat16_2;
    half3 u_xlat16_3;
    half3 u_xlat16_4;
    float3 u_xlat5;
    float3 u_xlat6;
    float2 u_xlat7;
    half3 u_xlat16_7;
    float u_xlat8;
    int u_xlati8;
    float u_xlat15;
    bool u_xlatb15;
    half u_xlat16_17;
    float u_xlat21;
    half u_xlat16_21;
    half u_xlat16_24;
    u_xlat0.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_0 = _AOTex.sample(sampler_AOTex, u_xlat0.xy).x;
    u_xlat7.x = fma(float(u_xlat16_0), 0.305306017, 0.682171106);
    u_xlat7.x = fma(float(u_xlat16_0), u_xlat7.x, 0.0125228781);
    u_xlat0.x = fma(float(u_xlat16_0), u_xlat7.x, -1.0);
    u_xlat0.x = fma(FGlobals._AOScale, u_xlat0.x, 1.0);
    u_xlat7.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_7.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat7.xy).xyz;
    u_xlat1.xyz = fma(float3(u_xlat16_7.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat1.xyz = fma(float3(u_xlat16_7.xyz), u_xlat1.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = float3(u_xlat16_7.xyz) * u_xlat1.xyz;
    u_xlat1.w = 1.0;
    u_xlat1 = u_xlat1 * FGlobals._BaseTint;
    u_xlat0.xyz = u_xlat0.xxx * u_xlat1.xyz;
    u_xlat21 = dot(FGlobals._WorldSpaceLightPos0.xyz, FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat21 = rsqrt(u_xlat21);
    u_xlat1.xyz = float3(u_xlat21) * float3(FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_2.xy = _NormalMap.sample(sampler_NormalMap, u_xlat2.xy).yw;
    u_xlat16_3.xy = fma(u_xlat16_2.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_17 = dot(u_xlat16_3.xy, u_xlat16_3.xy);
    u_xlat16_4.xy = half2(float2(u_xlat16_3.xy) * float2(FGlobals._NormalScale));
    u_xlat16_3.x = min(u_xlat16_17, half(1.0));
    u_xlat16_3.x = (-u_xlat16_3.x) + half(1.0);
    u_xlat16_4.z = sqrt(u_xlat16_3.x);
    u_xlat16_3.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_3.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_3.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_24 = dot(u_xlat16_3.xyz, u_xlat16_3.xyz);
    u_xlat16_24 = rsqrt(u_xlat16_24);
    u_xlat16_3.xyz = half3(u_xlat16_24) * u_xlat16_3.xyz;
    u_xlat21 = dot(float3(u_xlat16_3.xyz), u_xlat1.xyz);
    u_xlat1.x = u_xlat21 + FGlobals._BaseCellOffset;
    u_xlat21 = u_xlat21 + FGlobals._HighLightOffset;
    u_xlat21 = u_xlat21 / FGlobals._HighLightSmooth;
    u_xlat21 = clamp(u_xlat21, 0.0f, 1.0f);
    u_xlat1.x = u_xlat1.x / FGlobals._BaseCellSharpness;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlati8 = int(input.SV_InstanceID0) + UnityDrawCallInfo.unity_BaseInstanceID;
    u_xlati8 = u_xlati8 * 0x7;
    u_xlatb15 = FGlobals._LightColor0.w==half(0.0);
    u_xlat8 = (u_xlatb15) ? 0.0 : float(UnityInstancing_PerDraw2[u_xlati8 / 7].unity_SHCArray.w);
    u_xlat15 = u_xlat8 * u_xlat1.x;
    u_xlat2.xyz = fma(float3(u_xlat15), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat15 = fma((-u_xlat1.x), u_xlat8, 1.0);
    u_xlat1.x = fma(u_xlat1.x, u_xlat8, -1.0);
    u_xlat8 = u_xlat8 + -1.0;
    u_xlat8 = fma(FGlobals._RimShadowScale, u_xlat8, 1.0);
    u_xlat1.x = fma(FGlobals._MetcapShadowMask, u_xlat1.x, 1.0);
    u_xlat5.xyz = float3(u_xlat15) * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xyz = fma(u_xlat2.xyz, u_xlat0.xyz, u_xlat5.xyz);
    u_xlat15 = fma(u_xlat21, u_xlat21, (-u_xlat21));
    u_xlat21 = fma(FGlobals._HighLightPing, u_xlat15, u_xlat21);
    u_xlat0.xyz = fma(float3(u_xlat21), FGlobals._HighLightColor.xyz, u_xlat0.xyz);
    u_xlat2.xy = float2(u_xlat16_3.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat2.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_3.xx), u_xlat2.xy);
    u_xlat2.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_3.zz), u_xlat2.xy);
    u_xlat2.xy = u_xlat2.xy * float2(FGlobals._ReflectScale);
    u_xlat2.xy = fma(u_xlat2.xy, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat2.xy = u_xlat2.xy + float2(0.5, 0.5);
    u_xlat16_2.xyz = _Matcap.sample(sampler_Matcap, u_xlat2.xy).xyz;
    u_xlat5.xyz = fma(float3(u_xlat16_2.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat5.xyz = fma(float3(u_xlat16_2.xyz), u_xlat5.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat2.xyz = float3(u_xlat16_2.xyz) * u_xlat5.xyz;
    u_xlat21 = max(u_xlat2.y, u_xlat2.x);
    u_xlat21 = max(u_xlat2.z, u_xlat21);
    u_xlat21 = u_xlat21 * FGlobals._MatcapScale;
    u_xlat21 = u_xlat21 * u_xlat1.x;
    u_xlat2.xyz = float3(u_xlat21) * FGlobals._MatcapColor.xyz;
    u_xlat1.xz = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_21 = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat1.xz).x;
    u_xlat21 = fma(FGlobals._RoughnessScale, (-float(u_xlat16_21)), 1.0);
    u_xlat0.xyz = fma(u_xlat2.xyz, float3(u_xlat21), u_xlat0.xyz);
    u_xlat2.xyz = float3(u_xlat21) * u_xlat2.xyz;
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_3.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_3.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat5.x = input.TEXCOORD1.w;
    u_xlat5.y = input.TEXCOORD2.w;
    u_xlat5.z = input.TEXCOORD3.w;
    u_xlat5.xyz = (-u_xlat5.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat21 = dot(u_xlat5.xyz, u_xlat5.xyz);
    u_xlat21 = rsqrt(u_xlat21);
    u_xlat6.xyz = float3(u_xlat21) * u_xlat5.xyz;
    u_xlat5.xyz = fma((-u_xlat5.xyz), float3(u_xlat21), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat5.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat5.xyz, u_xlat6.xyz);
    u_xlat6.x = input.TEXCOORD1.z;
    u_xlat6.y = input.TEXCOORD2.z;
    u_xlat6.z = input.TEXCOORD3.z;
    u_xlat21 = dot(u_xlat6.xyz, u_xlat5.xyz);
    u_xlat21 = u_xlat21 + FGlobals._RimOffset;
    u_xlat21 = clamp(u_xlat21, 0.0f, 1.0f);
    u_xlat21 = (-u_xlat21) + 1.0;
    u_xlat21 = log2(u_xlat21);
    u_xlat21 = u_xlat21 * FGlobals._RimPower;
    u_xlat21 = exp2(u_xlat21);
    u_xlat21 = u_xlat8 * u_xlat21;
    u_xlat1.xyz = float3(u_xlat21) * FGlobals._RimColor.xyz;
    u_xlat1.xyz = u_xlat1.xyz * float3(FGlobals._RimLightMul);
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(FGlobals._RimColorMul), u_xlat0.xyz);
    u_xlat1.x = max(u_xlat1.y, u_xlat1.x);
    u_xlat1.x = max(u_xlat1.z, u_xlat1.x);
    u_xlat8 = max(u_xlat2.y, u_xlat2.x);
    u_xlat8 = max(u_xlat2.z, u_xlat8);
    u_xlat8 = clamp(u_xlat8, 0.0f, 1.0f);
    u_xlat8 = u_xlat8 * FGlobals._MatcapAlphaScale;
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._AlphaTex_ST.xy, FGlobals._AlphaTex_ST.zw);
    u_xlat16_2.xy = _AlphaTex.sample(sampler_AlphaTex, u_xlat2.xy).xw;
    u_xlat15 = fma(float(u_xlat16_2.x), 0.305306017, 0.682171106);
    u_xlat15 = fma(float(u_xlat16_2.x), u_xlat15, 0.0125228781);
    u_xlat15 = u_xlat15 * float(u_xlat16_2.x);
    u_xlat15 = u_xlat1.w * u_xlat15;
    u_xlat8 = fma(u_xlat15, float(u_xlat16_2.y), u_xlat8);
    u_xlat0.w = fma(u_xlat1.x, FGlobals._RimAlphaMul, u_xlat8);
    output.SV_Target0 = half4(u_xlat0);
    return output;
}
