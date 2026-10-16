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
    float4 hlslcc_mtx4x4unity_WorldToShadow[16];
    half4 _LightShadowData;
    float4 unity_ShadowFadeCenterAndType;
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
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

constexpr sampler _mtl_xl_shadow_sampler(address::clamp_to_edge, filter::linear, compare_func::greater_equal);
fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    sampler sampler_ShadowMapTexture [[ sampler (0) ]],
    sampler sampler_LightTexture0 [[ sampler (1) ]],
    sampler sampler_LightTextureB0 [[ sampler (2) ]],
    sampler sampler_NormalMap [[ sampler (3) ]],
    sampler sampler_BaseColor [[ sampler (4) ]],
    sampler sampler_AOTex [[ sampler (5) ]],
    sampler sampler_Matcap [[ sampler (6) ]],
    sampler sampler_RoughnessMap [[ sampler (7) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(3) ]] ,
    texture2d<half, access::sample > _RoughnessMap [[ texture(4) ]] ,
    texture2d<float, access::sample > _LightTexture0 [[ texture(5) ]] ,
    texture2d<float, access::sample > _LightTextureB0 [[ texture(6) ]] ,
    depth2d<float, access::sample > _ShadowMapTexture [[ texture(7) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float3 u_xlat0;
    half3 u_xlat16_0;
    float4 u_xlat1;
    half3 u_xlat16_1;
    float4 u_xlat2;
    float3 u_xlat3;
    float4 u_xlat4;
    half3 u_xlat16_4;
    float4 u_xlat5;
    half3 u_xlat16_5;
    float4 u_xlat6;
    half2 u_xlat16_6;
    half3 u_xlat16_7;
    float3 u_xlat9;
    float3 u_xlat10;
    half u_xlat16_12;
    float2 u_xlat17;
    float u_xlat18;
    half u_xlat16_20;
    float u_xlat24;
    half u_xlat16_24;
    float u_xlat25;
    half u_xlat16_25;
    bool u_xlatb25;
    float u_xlat27;
    half u_xlat16_28;
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
    u_xlat16_24 = _AOTex.sample(sampler_AOTex, u_xlat1.xy).x;
    u_xlat1.x = fma(float(u_xlat16_24), 0.305306017, 0.682171106);
    u_xlat1.x = fma(float(u_xlat16_24), u_xlat1.x, 0.0125228781);
    u_xlat24 = fma(float(u_xlat16_24), u_xlat1.x, -1.0);
    u_xlat24 = fma(FGlobals._AOScale, u_xlat24, 1.0);
    u_xlat0.xyz = float3(u_xlat24) * u_xlat0.xyz;
    u_xlat1.xyz = input.TEXCOORD4.yyy * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].xyz;
    u_xlat1.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].xyz, input.TEXCOORD4.xxx, u_xlat1.xyz);
    u_xlat1.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].xyz, input.TEXCOORD4.zzz, u_xlat1.xyz);
    u_xlat1.xyz = u_xlat1.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToObject[3].xyz;
    u_xlat24 = u_xlat1.y * FGlobals._FadeColorScale;
    u_xlat24 = fma(u_xlat24, 0.5, FGlobals._FadeColorOffset);
    u_xlat24 = u_xlat24 + 0.5;
    u_xlat24 = clamp(u_xlat24, 0.0f, 1.0f);
    u_xlat2.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat2.xyz = fma(float3(u_xlat24), u_xlat2.xyz, FGlobals._FadeColorF.xyz);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat2.xyz;
    u_xlat2 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1];
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0], input.TEXCOORD4.xxxx, u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2], input.TEXCOORD4.zzzz, u_xlat2);
    u_xlat2 = u_xlat2 + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3];
    u_xlat3.xy = u_xlat2.xy / u_xlat2.ww;
    u_xlat3.xy = u_xlat3.xy + float2(0.5, 0.5);
    u_xlat24 = _LightTexture0.sample(sampler_LightTexture0, u_xlat3.xy).w;
    u_xlatb25 = 0.0<u_xlat2.z;
    u_xlat2.x = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat2.x = _LightTextureB0.sample(sampler_LightTextureB0, u_xlat2.xx).x;
    u_xlat16_4.x = (u_xlatb25) ? half(1.0) : half(0.0);
    u_xlat16_4.x = half(u_xlat24 * float(u_xlat16_4.x));
    u_xlat16_4.x = half(u_xlat2.x * float(u_xlat16_4.x));
    u_xlat2 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToShadow[1];
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[0], input.TEXCOORD4.xxxx, u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[2], input.TEXCOORD4.zzzz, u_xlat2);
    u_xlat2 = u_xlat2 + FGlobals.hlslcc_mtx4x4unity_WorldToShadow[3];
    u_xlat2.xyz = u_xlat2.xyz / u_xlat2.www;
    u_xlat24 = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat2.xy, saturate(u_xlat2.z), level(0.0));
    u_xlat25 = (-float(FGlobals._LightShadowData.x)) + 1.0;
    u_xlat24 = fma(u_xlat24, u_xlat25, float(FGlobals._LightShadowData.x));
    u_xlat16_12 = half((-u_xlat24) + 1.0);
    u_xlat2.xyz = input.TEXCOORD4.xyz + (-FGlobals.unity_ShadowFadeCenterAndType.xyz);
    u_xlat25 = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat25 = sqrt(u_xlat25);
    u_xlat2.x = FGlobals.hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat2.y = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat2.z = FGlobals.hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat3.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat2.x = dot(u_xlat3.xyz, u_xlat2.xyz);
    u_xlat25 = u_xlat25 + (-u_xlat2.x);
    u_xlat25 = fma(FGlobals.unity_ShadowFadeCenterAndType.w, u_xlat25, u_xlat2.x);
    u_xlat25 = fma(u_xlat25, float(FGlobals._LightShadowData.z), float(FGlobals._LightShadowData.w));
    u_xlat25 = clamp(u_xlat25, 0.0f, 1.0f);
    u_xlat16_12 = half(fma(u_xlat25, float(u_xlat16_12), u_xlat24));
    u_xlat16_4.x = u_xlat16_12 * u_xlat16_4.x;
    u_xlat16_4.xyz = u_xlat16_4.xxx * FGlobals._LightColor0.xyz;
    u_xlat16_5.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_4.xyz = u_xlat16_4.xyz / u_xlat16_5.xyz;
    u_xlat24 = max(float(u_xlat16_4.y), float(u_xlat16_4.x));
    u_xlat24 = max(float(u_xlat16_4.z), u_xlat24);
    u_xlat2.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat25 = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat25 = rsqrt(u_xlat25);
    u_xlat2.xyz = float3(u_xlat25) * u_xlat2.xyz;
    u_xlat6.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_6.xy = _NormalMap.sample(sampler_NormalMap, u_xlat6.xy).yw;
    u_xlat16_4.xy = fma(u_xlat16_6.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_20 = dot(u_xlat16_4.xy, u_xlat16_4.xy);
    u_xlat16_5.xy = half2(float2(u_xlat16_4.xy) * float2(FGlobals._NormalScale));
    u_xlat16_4.x = min(u_xlat16_20, half(1.0));
    u_xlat16_4.x = (-u_xlat16_4.x) + half(1.0);
    u_xlat16_5.z = sqrt(u_xlat16_4.x);
    u_xlat16_4.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_4.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_4.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_28 = dot(u_xlat16_4.xyz, u_xlat16_4.xyz);
    u_xlat16_28 = rsqrt(u_xlat16_28);
    u_xlat16_4.xyz = half3(u_xlat16_28) * u_xlat16_4.xyz;
    u_xlat25 = dot(float3(u_xlat16_4.xyz), u_xlat2.xyz);
    u_xlat2.x = u_xlat25 + FGlobals._BaseCellOffset;
    u_xlat2.x = u_xlat2.x / FGlobals._BaseCellSharpness;
    u_xlat2.x = clamp(u_xlat2.x, 0.0f, 1.0f);
    u_xlat10.x = u_xlat24 * u_xlat2.x;
    u_xlat10.xyz = fma(u_xlat10.xxx, FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat27 = (-u_xlat25) + FGlobals._inverseLightOffset;
    u_xlat25 = u_xlat25 + FGlobals._HighLightOffset;
    u_xlat25 = u_xlat25 / FGlobals._HighLightSmooth;
    u_xlat25 = clamp(u_xlat25, 0.0f, 1.0f);
    u_xlat27 = u_xlat27 / FGlobals._inverseLightSmooth;
    u_xlat27 = clamp(u_xlat27, 0.0f, 1.0f);
    u_xlat10.xyz = fma(float3(u_xlat27), FGlobals._inverseColor.xyz, u_xlat10.xyz);
    u_xlat27 = fma((-u_xlat2.x), u_xlat24, 1.0);
    u_xlat24 = fma(u_xlat2.x, u_xlat24, -1.0);
    u_xlat6.xyz = float3(u_xlat27) * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xyz = fma(u_xlat10.xyz, u_xlat0.xyz, u_xlat6.xyz);
    u_xlat2.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), float2(u_xlat24), float2(1.0, 1.0));
    u_xlat24 = fma(FGlobals._RimShadowScale, u_xlat24, 1.0);
    u_xlat18 = fma(u_xlat25, u_xlat25, (-u_xlat25));
    u_xlat25 = fma(FGlobals._HighLightPing, u_xlat18, u_xlat25);
    u_xlat6.xyz = float3(u_xlat25) * FGlobals._HighLightColor.xyz;
    u_xlat0.xyz = fma(u_xlat6.xyz, u_xlat2.xxx, u_xlat0.xyz);
    u_xlat5 = float4(u_xlat16_4.yyyy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].zxyz;
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].zxyz, float4(u_xlat16_4.xxxx), u_xlat5);
    u_xlat4 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].zxyz, float4(u_xlat16_4.zzzz), u_xlat5);
    u_xlat5 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].xxxx, u_xlat5);
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].zzzz, u_xlat5);
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].wwww, u_xlat5);
    u_xlat6 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat6 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].xxxx, u_xlat6);
    u_xlat6 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].zzzz, u_xlat6);
    u_xlat6 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].wwww, u_xlat6);
    u_xlat6 = u_xlat1.yyyy * u_xlat6;
    u_xlat5 = fma(u_xlat5, u_xlat1.xxxx, u_xlat6);
    u_xlat6 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat6 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].xxxx, u_xlat6);
    u_xlat6 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].zzzz, u_xlat6);
    u_xlat6 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].wwww, u_xlat6);
    u_xlat1 = fma(u_xlat6, u_xlat1.zzzz, u_xlat5);
    u_xlat5 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].xxxx, u_xlat5);
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].zzzz, u_xlat5);
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].wwww, u_xlat5);
    u_xlat1 = u_xlat1 + u_xlat5;
    u_xlat2.x = dot(u_xlat1.xzw, u_xlat1.xzw);
    u_xlat2.x = max(u_xlat2.x, 0.00100000005);
    u_xlat2.x = rsqrt(u_xlat2.x);
    u_xlat1 = u_xlat1 * u_xlat2.xxxx;
    u_xlat17.xy = u_xlat4.zw * u_xlat1.zw;
    u_xlat1.xy = fma(u_xlat1.xy, u_xlat4.xy, (-u_xlat17.xy));
    u_xlat1.z = (-u_xlat1.y);
    u_xlat9.x = FGlobals._ReflectScale * 0.5;
    u_xlat1.xy = fma(u_xlat1.zx, u_xlat9.xx, FGlobals._ReflectOffset.xyxx.xy);
    u_xlat1.xy = u_xlat1.xy + float2(0.5, 0.5);
    u_xlat16_1.xyz = _Matcap.sample(sampler_Matcap, u_xlat1.xy).xyz;
    u_xlat2.xzw = fma(float3(u_xlat16_1.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat2.xzw = fma(float3(u_xlat16_1.xyz), u_xlat2.xzw, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = float3(u_xlat16_1.xyz) * u_xlat2.xzw;
    u_xlat1.x = max(u_xlat1.y, u_xlat1.x);
    u_xlat1.x = max(u_xlat1.z, u_xlat1.x);
    u_xlat1.x = u_xlat1.x * FGlobals._MatcapScale;
    u_xlat1.x = u_xlat1.x * u_xlat2.y;
    u_xlat1.xyz = u_xlat1.xxx * FGlobals._MatcapColor.xyz;
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_25 = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat2.xy).x;
    u_xlat25 = fma(FGlobals._RoughnessScale, (-float(u_xlat16_25)), 1.0);
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(u_xlat25), u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_7.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_7.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat1.x = dot(u_xlat3.xyz, u_xlat3.xyz);
    u_xlat1.x = rsqrt(u_xlat1.x);
    u_xlat9.xyz = u_xlat1.xxx * u_xlat3.xyz;
    u_xlat2.xyz = fma((-u_xlat3.xyz), u_xlat1.xxx, FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat1.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat2.xyz, u_xlat9.xyz);
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
    u_xlat24 = u_xlat24 * u_xlat1.x;
    u_xlat1.xyz = float3(u_xlat24) * FGlobals._RimColor.xyz;
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(FGlobals._RimLightMul), u_xlat0.xyz);
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
