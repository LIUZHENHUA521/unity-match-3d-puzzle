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
    float4 hlslcc_mtx4x4unity_WorldToObject[4];
    float4 hlslcc_mtx4x4unity_MatrixV[4];
    float4 hlslcc_mtx4x4unity_MatrixInvV[4];
    float4 _ShadowOffsets[4];
    float4 hlslcc_mtx4x4unity_WorldToLight[4];
    half4 _LightColor0;
    float4 _IndirectColor;
    float4 _NormalMap_ST;
    float _NormalScale;
    float _BaseCellOffset;
    float _BaseCellSharpness;
    float4 _LightColor;
    float3 _CustomInverseDir;
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
    float4 u_xlat0;
    float3 u_xlat1;
    float4 u_xlat2;
    half u_xlat16_2;
    bool u_xlatb2;
    float4 u_xlat3;
    half3 u_xlat16_3;
    float3 u_xlat4;
    half3 u_xlat16_5;
    half3 u_xlat16_6;
    float3 u_xlat7;
    float3 u_xlat8;
    half3 u_xlat16_8;
    float3 u_xlat9;
    float3 u_xlat10;
    half2 u_xlat16_10;
    float u_xlat12;
    float3 u_xlat14;
    half u_xlat16_15;
    float u_xlat20;
    float2 u_xlat24;
    half u_xlat16_25;
    float u_xlat30;
    float u_xlat31;
    bool u_xlatb31;
    float u_xlat33;
    half u_xlat16_33;
    float u_xlat34;
    half u_xlat16_35;
    u_xlat0 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1];
    u_xlat0 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0], input.TEXCOORD4.xxxx, u_xlat0);
    u_xlat0 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2], input.TEXCOORD4.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3];
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat2.x = FGlobals.hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat2.y = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat2.z = FGlobals.hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat31 = dot(u_xlat1.xyz, u_xlat2.xyz);
    u_xlat2.xyz = input.TEXCOORD4.xyz + (-FGlobals.unity_ShadowFadeCenterAndType.xyz);
    u_xlat2.x = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat2.x = sqrt(u_xlat2.x);
    u_xlat2.x = (-u_xlat31) + u_xlat2.x;
    u_xlat31 = fma(FGlobals.unity_ShadowFadeCenterAndType.w, u_xlat2.x, u_xlat31);
    u_xlat31 = fma(u_xlat31, float(FGlobals._LightShadowData.z), float(FGlobals._LightShadowData.w));
    u_xlat31 = clamp(u_xlat31, 0.0f, 1.0f);
    u_xlatb2 = u_xlat31<0.99000001;
    if(u_xlatb2){
        u_xlat2 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToShadow[1];
        u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[0], input.TEXCOORD4.xxxx, u_xlat2);
        u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[2], input.TEXCOORD4.zzzz, u_xlat2);
        u_xlat2 = u_xlat2 + FGlobals.hlslcc_mtx4x4unity_WorldToShadow[3];
        u_xlat2.xyz = u_xlat2.xyz / u_xlat2.www;
        u_xlat3.xyz = u_xlat2.xyz + FGlobals._ShadowOffsets[0].xyz;
        u_xlat3.x = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat3.xy, saturate(u_xlat3.z), level(0.0));
        u_xlat4.xyz = u_xlat2.xyz + FGlobals._ShadowOffsets[1].xyz;
        u_xlat3.y = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat4.xy, saturate(u_xlat4.z), level(0.0));
        u_xlat4.xyz = u_xlat2.xyz + FGlobals._ShadowOffsets[2].xyz;
        u_xlat3.z = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat4.xy, saturate(u_xlat4.z), level(0.0));
        u_xlat2.xyz = u_xlat2.xyz + FGlobals._ShadowOffsets[3].xyz;
        u_xlat3.w = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat2.xy, saturate(u_xlat2.z), level(0.0));
        u_xlat2.x = dot(u_xlat3, float4(0.25, 0.25, 0.25, 0.25));
        u_xlat12 = (-float(FGlobals._LightShadowData.x)) + 1.0;
        u_xlat2.x = fma(u_xlat2.x, u_xlat12, float(FGlobals._LightShadowData.x));
        u_xlat16_2 = half(u_xlat2.x);
    } else {
        u_xlat16_2 = half(1.0);
    }
    u_xlat16_5.x = (-u_xlat16_2) + half(1.0);
    u_xlat16_5.x = half(fma(u_xlat31, float(u_xlat16_5.x), float(u_xlat16_2)));
    u_xlatb31 = 0.0<u_xlat0.z;
    u_xlat16_15 = (u_xlatb31) ? half(1.0) : half(0.0);
    u_xlat3.xy = u_xlat0.xy / u_xlat0.ww;
    u_xlat3.xy = u_xlat3.xy + float2(0.5, 0.5);
    u_xlat30 = _LightTexture0.sample(sampler_LightTexture0, u_xlat3.xy).w;
    u_xlat16_15 = half(u_xlat30 * float(u_xlat16_15));
    u_xlat0.x = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat0.x = _LightTextureB0.sample(sampler_LightTextureB0, u_xlat0.xx).x;
    u_xlat16_15 = half(u_xlat0.x * float(u_xlat16_15));
    u_xlat16_5.x = u_xlat16_5.x * u_xlat16_15;
    u_xlat16_5.xyz = u_xlat16_5.xxx * FGlobals._LightColor0.xyz;
    u_xlat16_6.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_5.xyz = u_xlat16_5.xyz / u_xlat16_6.xyz;
    u_xlat0.x = max(float(u_xlat16_5.y), float(u_xlat16_5.x));
    u_xlat0.x = max(float(u_xlat16_5.z), u_xlat0.x);
    u_xlat10.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_10.xy = _NormalMap.sample(sampler_NormalMap, u_xlat10.xy).yw;
    u_xlat16_5.xy = fma(u_xlat16_10.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_25 = dot(u_xlat16_5.xy, u_xlat16_5.xy);
    u_xlat16_25 = min(u_xlat16_25, half(1.0));
    u_xlat16_25 = (-u_xlat16_25) + half(1.0);
    u_xlat16_6.z = sqrt(u_xlat16_25);
    u_xlat16_6.xy = half2(float2(u_xlat16_5.xy) * float2(FGlobals._NormalScale));
    u_xlat16_5.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_6.xyz));
    u_xlat16_5.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_6.xyz));
    u_xlat16_5.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_6.xyz));
    u_xlat16_35 = dot(u_xlat16_5.xyz, u_xlat16_5.xyz);
    u_xlat16_35 = rsqrt(u_xlat16_35);
    u_xlat16_5.xyz = half3(u_xlat16_35) * u_xlat16_5.xyz;
    u_xlat10.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat31 = dot(u_xlat10.xyz, u_xlat10.xyz);
    u_xlat31 = rsqrt(u_xlat31);
    u_xlat10.xyz = u_xlat10.xyz * float3(u_xlat31);
    u_xlat10.x = dot(float3(u_xlat16_5.xyz), u_xlat10.xyz);
    u_xlat20 = u_xlat10.x + FGlobals._BaseCellOffset;
    u_xlat20 = u_xlat20 / FGlobals._BaseCellSharpness;
    u_xlat20 = clamp(u_xlat20, 0.0f, 1.0f);
    u_xlat30 = u_xlat0.x * u_xlat20;
    u_xlat31 = dot(FGlobals._CustomInverseDir.xyzx.xyz, FGlobals._CustomInverseDir.xyzx.xyz);
    u_xlat31 = max(u_xlat31, 0.00100000005);
    u_xlat31 = rsqrt(u_xlat31);
    u_xlat3.xyz = float3(u_xlat31) * FGlobals._CustomInverseDir.xyzx.xyz;
    u_xlat31 = dot(float3(u_xlat16_5.xyz), u_xlat3.xyz);
    u_xlat31 = u_xlat31 + FGlobals._inverseLightOffset;
    u_xlat31 = u_xlat31 / FGlobals._inverseLightSmooth;
    u_xlat31 = clamp(u_xlat31, 0.0f, 1.0f);
    u_xlat3.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_3.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat3.xy).xyz;
    u_xlat4.xyz = fma(float3(u_xlat16_3.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat4.xyz = fma(float3(u_xlat16_3.xyz), u_xlat4.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xyz = float3(u_xlat16_3.xyz) * u_xlat4.xyz;
    u_xlat4.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat4.xyz = fma(FGlobals._BaseTint.xyz, u_xlat4.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat4.xyz = u_xlat4.xyz * FGlobals._BaseTint.xyz;
    u_xlat7.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_33 = _AOTex.sample(sampler_AOTex, u_xlat7.xy).x;
    u_xlat34 = fma(float(u_xlat16_33), 0.305306017, 0.682171106);
    u_xlat34 = fma(float(u_xlat16_33), u_xlat34, 0.0125228781);
    u_xlat33 = fma(float(u_xlat16_33), u_xlat34, -1.0);
    u_xlat33 = fma(FGlobals._AOScale, u_xlat33, 1.0);
    u_xlat34 = input.TEXCOORD4.y * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].y;
    u_xlat34 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].y, input.TEXCOORD4.x, u_xlat34);
    u_xlat34 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].y, input.TEXCOORD4.z, u_xlat34);
    u_xlat34 = u_xlat34 + FGlobals.hlslcc_mtx4x4unity_WorldToObject[3].y;
    u_xlat34 = u_xlat34 * FGlobals._FadeColorScale;
    u_xlat34 = fma(u_xlat34, 0.5, FGlobals._FadeColorOffset);
    u_xlat34 = u_xlat34 + 0.5;
    u_xlat34 = clamp(u_xlat34, 0.0f, 1.0f);
    u_xlat7.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat7.xyz = fma(float3(u_xlat34), u_xlat7.xyz, FGlobals._FadeColorF.xyz);
    u_xlat3.xyz = u_xlat3.xyz * u_xlat4.xyz;
    u_xlat3.xyz = float3(u_xlat33) * u_xlat3.xyz;
    u_xlat3.xyz = u_xlat7.xyz * u_xlat3.xyz;
    u_xlat10.x = u_xlat10.x + FGlobals._HighLightOffset;
    u_xlat10.x = u_xlat10.x / FGlobals._HighLightSmooth;
    u_xlat10.x = clamp(u_xlat10.x, 0.0f, 1.0f);
    u_xlat33 = fma(u_xlat10.x, u_xlat10.x, (-u_xlat10.x));
    u_xlat10.x = fma(FGlobals._HighLightPing, u_xlat33, u_xlat10.x);
    u_xlat33 = fma(u_xlat20, u_xlat0.x, -1.0);
    u_xlat4.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), float2(u_xlat33), float2(1.0, 1.0));
    u_xlat7.xyz = u_xlat10.xxx * FGlobals._HighLightColor.xyz;
    u_xlat24.xy = float2(u_xlat16_5.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat24.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_5.xx), u_xlat24.xy);
    u_xlat24.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_5.zz), u_xlat24.xy);
    u_xlat24.xy = u_xlat24.xy * float2(FGlobals._ReflectScale);
    u_xlat24.xy = fma(u_xlat24.xy, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat24.xy = u_xlat24.xy + float2(0.5, 0.5);
    u_xlat16_8.xyz = _Matcap.sample(sampler_Matcap, u_xlat24.xy).xyz;
    u_xlat9.xyz = fma(float3(u_xlat16_8.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat9.xyz = fma(float3(u_xlat16_8.xyz), u_xlat9.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat8.xyz = float3(u_xlat16_8.xyz) * u_xlat9.xyz;
    u_xlat24.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_10.x = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat24.xy).x;
    u_xlat10.x = fma(FGlobals._RoughnessScale, (-float(u_xlat16_10.x)), 1.0);
    u_xlat8.xyz = u_xlat8.xyz * float3(FGlobals._MatcapScale);
    u_xlat14.xyz = u_xlat4.yyy * u_xlat8.xyz;
    u_xlat14.xyz = u_xlat14.xyz * FGlobals._MatcapColor.xyz;
    u_xlat8.xyz = fma(float3(u_xlat30), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat8.xyz = fma(float3(u_xlat31), FGlobals._inverseColor.xyz, u_xlat8.xyz);
    u_xlat0.x = fma((-u_xlat20), u_xlat0.x, 1.0);
    u_xlat0.xzw = u_xlat0.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xzw = fma(u_xlat8.xyz, u_xlat3.xyz, u_xlat0.xzw);
    u_xlat0.xzw = fma(u_xlat7.xyz, u_xlat4.xxx, u_xlat0.xzw);
    u_xlat0.xyz = fma(u_xlat14.xyz, u_xlat10.xxx, u_xlat0.xzw);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_5.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_5.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat30 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat30 = rsqrt(u_xlat30);
    u_xlat3.xyz = float3(u_xlat30) * u_xlat1.xyz;
    u_xlat1.xyz = fma((-u_xlat1.xyz), float3(u_xlat30), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat1.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat1.xyz, u_xlat3.xyz);
    u_xlat3.x = input.TEXCOORD1.z;
    u_xlat3.y = input.TEXCOORD2.z;
    u_xlat3.z = input.TEXCOORD3.z;
    u_xlat30 = dot(u_xlat3.xyz, u_xlat1.xyz);
    u_xlat1.x = fma(FGlobals._RimShadowScale, u_xlat33, 1.0);
    u_xlat30 = u_xlat30 + FGlobals._RimOffset;
    u_xlat30 = clamp(u_xlat30, 0.0f, 1.0f);
    u_xlat30 = (-u_xlat30) + 1.0;
    u_xlat30 = log2(u_xlat30);
    u_xlat30 = u_xlat30 * FGlobals._RimPower;
    u_xlat30 = exp2(u_xlat30);
    u_xlat30 = u_xlat1.x * u_xlat30;
    u_xlat1.xyz = float3(u_xlat30) * FGlobals._RimColor.xyz;
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(FGlobals._RimLightMul), u_xlat0.xyz);
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
