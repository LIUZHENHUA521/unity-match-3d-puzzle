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
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_WorldToObject[4];
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
    half3 u_xlat16_0;
    float4 u_xlat1;
    half2 u_xlat16_1;
    float4 u_xlat2;
    float4 u_xlat3;
    half3 u_xlat16_3;
    half3 u_xlat16_4;
    float3 u_xlat5;
    half3 u_xlat16_5;
    float3 u_xlat6;
    float2 u_xlat7;
    half2 u_xlat16_7;
    float2 u_xlat8;
    float2 u_xlat14;
    float u_xlat15;
    bool u_xlatb15;
    half u_xlat16_17;
    float u_xlat21;
    half u_xlat16_21;
    float u_xlat22;
    float u_xlat23;
    half u_xlat16_23;
    half u_xlat16_24;
    u_xlat0 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat0 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].xxxx, u_xlat0);
    u_xlat0 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].zzzz, u_xlat0);
    u_xlat0 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].wwww, u_xlat0);
    u_xlat1 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].xxxx, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].zzzz, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].wwww, u_xlat1);
    u_xlat2.xyz = input.TEXCOORD4.yyy * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].xyz;
    u_xlat2.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].xyz, input.TEXCOORD4.xxx, u_xlat2.xyz);
    u_xlat2.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].xyz, input.TEXCOORD4.zzz, u_xlat2.xyz);
    u_xlat2.xyz = u_xlat2.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToObject[3].xyz;
    u_xlat1 = u_xlat1 * u_xlat2.yyyy;
    u_xlat0 = fma(u_xlat0, u_xlat2.xxxx, u_xlat1);
    u_xlat1 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].xxxx, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].zzzz, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].wwww, u_xlat1);
    u_xlat0 = fma(u_xlat1, u_xlat2.zzzz, u_xlat0);
    u_xlat1 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].xxxx, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].zzzz, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].wwww, u_xlat1);
    u_xlat0 = u_xlat0 + u_xlat1;
    u_xlat1.x = dot(u_xlat0.xzw, u_xlat0.xzw);
    u_xlat1.x = max(u_xlat1.x, 0.00100000005);
    u_xlat1.x = rsqrt(u_xlat1.x);
    u_xlat0 = u_xlat0 * u_xlat1.xxxx;
    u_xlat1.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_1.xy = _NormalMap.sample(sampler_NormalMap, u_xlat1.xy).yw;
    u_xlat16_3.xy = fma(u_xlat16_1.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
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
    u_xlat1 = float4(u_xlat16_3.yyyy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].zxyz;
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].zxyz, float4(u_xlat16_3.xxxx), u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].zxyz, float4(u_xlat16_3.zzzz), u_xlat1);
    u_xlat14.xy = u_xlat0.zw * u_xlat1.zw;
    u_xlat0.xy = fma(u_xlat0.xy, u_xlat1.xy, (-u_xlat14.xy));
    u_xlat0.z = (-u_xlat0.y);
    u_xlat7.x = FGlobals._ReflectScale * 0.5;
    u_xlat0.xy = fma(u_xlat0.zx, u_xlat7.xx, FGlobals._ReflectOffset.xyxx.xy);
    u_xlat0.xy = u_xlat0.xy + float2(0.5, 0.5);
    u_xlat16_0.xyz = _Matcap.sample(sampler_Matcap, u_xlat0.xy).xyz;
    u_xlat1.xyz = fma(float3(u_xlat16_0.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat1.xyz = fma(float3(u_xlat16_0.xyz), u_xlat1.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat0.xyz = float3(u_xlat16_0.xyz) * u_xlat1.xyz;
    u_xlat0.xyz = u_xlat0.xyz * float3(FGlobals._MatcapScale);
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat21 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat21 = rsqrt(u_xlat21);
    u_xlat1.xyz = float3(u_xlat21) * u_xlat1.xyz;
    u_xlat21 = dot(float3(u_xlat16_3.xyz), u_xlat1.xyz);
    u_xlat1.x = u_xlat21 + FGlobals._BaseCellOffset;
    u_xlat21 = u_xlat21 + FGlobals._HighLightOffset;
    u_xlat21 = u_xlat21 / FGlobals._HighLightSmooth;
    u_xlat21 = clamp(u_xlat21, 0.0f, 1.0f);
    u_xlat1.x = u_xlat1.x / FGlobals._BaseCellSharpness;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat2 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1];
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0], input.TEXCOORD4.xxxx, u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2], input.TEXCOORD4.zzzz, u_xlat2);
    u_xlat2 = u_xlat2 + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3];
    u_xlat8.xy = u_xlat2.xy / u_xlat2.ww;
    u_xlat8.xy = u_xlat8.xy + float2(0.5, 0.5);
    u_xlat8.x = _LightTexture0.sample(sampler_LightTexture0, u_xlat8.xy).w;
    u_xlatb15 = 0.0<u_xlat2.z;
    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat22 = _LightTextureB0.sample(sampler_LightTextureB0, float2(u_xlat22)).x;
    u_xlat16_3.x = (u_xlatb15) ? half(1.0) : half(0.0);
    u_xlat16_3.x = half(u_xlat8.x * float(u_xlat16_3.x));
    u_xlat16_3.x = half(u_xlat22 * float(u_xlat16_3.x));
    u_xlat16_3.xyz = u_xlat16_3.xxx * FGlobals._LightColor0.xyz;
    u_xlat16_4.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_3.xyz = u_xlat16_3.xyz / u_xlat16_4.xyz;
    u_xlat8.x = max(float(u_xlat16_3.y), float(u_xlat16_3.x));
    u_xlat8.x = max(float(u_xlat16_3.z), u_xlat8.x);
    u_xlat15 = fma(u_xlat1.x, u_xlat8.x, -1.0);
    u_xlat15 = fma(FGlobals._MetcapShadowMask, u_xlat15, 1.0);
    u_xlat0.xyz = u_xlat0.xyz * float3(u_xlat15);
    u_xlat0.xyz = u_xlat0.xyz * FGlobals._MatcapColor.xyz;
    u_xlat15 = fma(u_xlat21, u_xlat21, (-u_xlat21));
    u_xlat21 = fma(FGlobals._HighLightPing, u_xlat15, u_xlat21);
    u_xlat15 = u_xlat8.x * u_xlat1.x;
    u_xlat1.x = fma((-u_xlat1.x), u_xlat8.x, 1.0);
    u_xlat8.x = u_xlat8.x + -1.0;
    u_xlat8.x = fma(FGlobals._RimShadowScale, u_xlat8.x, 1.0);
    u_xlat2.xyz = u_xlat1.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat1.xzw = fma(float3(u_xlat15), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat5.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_23 = _AOTex.sample(sampler_AOTex, u_xlat5.xy).x;
    u_xlat5.x = fma(float(u_xlat16_23), 0.305306017, 0.682171106);
    u_xlat5.x = fma(float(u_xlat16_23), u_xlat5.x, 0.0125228781);
    u_xlat23 = fma(float(u_xlat16_23), u_xlat5.x, -1.0);
    u_xlat23 = fma(FGlobals._AOScale, u_xlat23, 1.0);
    u_xlat5.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_5.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat5.xy).xyz;
    u_xlat6.xyz = fma(float3(u_xlat16_5.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat6.xyz = fma(float3(u_xlat16_5.xyz), u_xlat6.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xyz = float3(u_xlat16_5.xyz) * u_xlat6.xyz;
    u_xlat3.w = 1.0;
    u_xlat3 = u_xlat3 * FGlobals._BaseTint;
    u_xlat5.xyz = float3(u_xlat23) * u_xlat3.xyz;
    u_xlat1.xzw = fma(u_xlat1.xzw, u_xlat5.xyz, u_xlat2.xyz);
    u_xlat1.xzw = fma(float3(u_xlat21), FGlobals._HighLightColor.xyz, u_xlat1.xzw);
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_21 = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat2.xy).x;
    u_xlat21 = fma(FGlobals._RoughnessScale, (-float(u_xlat16_21)), 1.0);
    u_xlat1.xzw = fma(u_xlat0.xyz, float3(u_xlat21), u_xlat1.xzw);
    u_xlat0.xyz = float3(u_xlat21) * u_xlat0.xyz;
    u_xlat1.xzw = u_xlat1.xzw + u_xlat1.xzw;
    u_xlat16_4.xyz = half3(max(u_xlat1.xzw, float3(0.0, 0.0, 0.0)));
    u_xlat1.xzw = log2(float3(u_xlat16_4.xyz));
    u_xlat1.xzw = u_xlat1.xzw * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat1.xzw = exp2(u_xlat1.xzw);
    u_xlat1.xzw = fma(u_xlat1.xzw, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat1.xzw = max(u_xlat1.xzw, float3(0.0, 0.0, 0.0));
    u_xlat2.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat21 = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat21 = rsqrt(u_xlat21);
    u_xlat5.xyz = float3(u_xlat21) * u_xlat2.xyz;
    u_xlat2.xyz = fma((-u_xlat2.xyz), float3(u_xlat21), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat2.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat2.xyz, u_xlat5.xyz);
    u_xlat5.x = input.TEXCOORD1.z;
    u_xlat5.y = input.TEXCOORD2.z;
    u_xlat5.z = input.TEXCOORD3.z;
    u_xlat21 = dot(u_xlat5.xyz, u_xlat2.xyz);
    u_xlat21 = u_xlat21 + FGlobals._RimOffset;
    u_xlat21 = clamp(u_xlat21, 0.0f, 1.0f);
    u_xlat21 = (-u_xlat21) + 1.0;
    u_xlat21 = log2(u_xlat21);
    u_xlat21 = u_xlat21 * FGlobals._RimPower;
    u_xlat21 = exp2(u_xlat21);
    u_xlat21 = u_xlat8.x * u_xlat21;
    u_xlat2.xyz = float3(u_xlat21) * FGlobals._RimColor.xyz;
    u_xlat2.xyz = u_xlat2.xyz * float3(FGlobals._RimLightMul);
    u_xlat1.xyz = fma(u_xlat2.xyz, float3(FGlobals._RimColorMul), u_xlat1.xzw);
    u_xlat21 = max(u_xlat2.y, u_xlat2.x);
    u_xlat21 = max(u_xlat2.z, u_xlat21);
    u_xlat0.x = max(u_xlat0.y, u_xlat0.x);
    u_xlat0.x = max(u_xlat0.z, u_xlat0.x);
    u_xlat0.x = clamp(u_xlat0.x, 0.0f, 1.0f);
    u_xlat0.x = u_xlat0.x * FGlobals._MatcapAlphaScale;
    u_xlat7.xy = fma(input.TEXCOORD0.xy, FGlobals._AlphaTex_ST.xy, FGlobals._AlphaTex_ST.zw);
    u_xlat16_7.xy = _AlphaTex.sample(sampler_AlphaTex, u_xlat7.xy).xw;
    u_xlat2.x = fma(float(u_xlat16_7.x), 0.305306017, 0.682171106);
    u_xlat2.x = fma(float(u_xlat16_7.x), u_xlat2.x, 0.0125228781);
    u_xlat7.x = float(u_xlat16_7.x) * u_xlat2.x;
    u_xlat7.x = u_xlat3.w * u_xlat7.x;
    u_xlat0.x = fma(u_xlat7.x, float(u_xlat16_7.y), u_xlat0.x);
    u_xlat1.w = fma(u_xlat21, FGlobals._RimAlphaMul, u_xlat0.x);
    output.SV_Target0 = half4(u_xlat1);
    return output;
}
