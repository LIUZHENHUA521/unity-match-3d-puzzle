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
    float4 _LightPositionRange;
    float4 _LightProjectionParams;
    half4 _LightShadowData;
    float4 unity_ShadowFadeCenterAndType;
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
    texture2d<float, access::sample > _LightTextureB0 [[ texture(5) ]] ,
    texturecube<float, access::sample > _LightTexture0 [[ texture(6) ]] ,
    depthcube<float, access::sample > _ShadowMapTexture [[ texture(7) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float4 u_xlat0;
    float3 u_xlat1;
    float3 u_xlat2;
    half3 u_xlat16_2;
    float4 u_xlat3;
    float3 u_xlat4;
    half3 u_xlat16_5;
    half3 u_xlat16_6;
    float3 u_xlat7;
    half3 u_xlat16_7;
    float3 u_xlat8;
    float3 u_xlat9;
    half2 u_xlat16_9;
    float3 u_xlat12;
    half u_xlat16_14;
    float u_xlat18;
    float2 u_xlat21;
    half u_xlat16_23;
    float u_xlat27;
    float u_xlat28;
    bool u_xlatb28;
    float u_xlat29;
    half u_xlat16_29;
    float u_xlat30;
    half u_xlat16_32;
    u_xlat0.xyz = input.TEXCOORD4.yyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1].xyz;
    u_xlat0.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0].xyz, input.TEXCOORD4.xxx, u_xlat0.xyz);
    u_xlat0.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2].xyz, input.TEXCOORD4.zzz, u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3].xyz;
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat2.x = FGlobals.hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat2.y = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat2.z = FGlobals.hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat27 = dot(u_xlat1.xyz, u_xlat2.xyz);
    u_xlat2.xyz = input.TEXCOORD4.xyz + (-FGlobals.unity_ShadowFadeCenterAndType.xyz);
    u_xlat28 = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat28 = sqrt(u_xlat28);
    u_xlat28 = (-u_xlat27) + u_xlat28;
    u_xlat27 = fma(FGlobals.unity_ShadowFadeCenterAndType.w, u_xlat28, u_xlat27);
    u_xlat27 = fma(u_xlat27, float(FGlobals._LightShadowData.z), float(FGlobals._LightShadowData.w));
    u_xlat27 = clamp(u_xlat27, 0.0f, 1.0f);
    u_xlatb28 = u_xlat27<0.99000001;
    if(u_xlatb28){
        u_xlat2.xyz = input.TEXCOORD4.xyz + (-FGlobals._LightPositionRange.xyz);
        u_xlat28 = max(abs(u_xlat2.y), abs(u_xlat2.x));
        u_xlat28 = max(abs(u_xlat2.z), u_xlat28);
        u_xlat28 = u_xlat28 + (-FGlobals._LightProjectionParams.z);
        u_xlat28 = max(u_xlat28, 9.99999975e-06);
        u_xlat28 = u_xlat28 * FGlobals._LightProjectionParams.w;
        u_xlat28 = FGlobals._LightProjectionParams.y / u_xlat28;
        u_xlat28 = u_xlat28 + (-FGlobals._LightProjectionParams.x);
        u_xlat28 = (-u_xlat28) + 1.0;
        u_xlat3.xyz = u_xlat2.xyz + float3(0.0078125, 0.0078125, 0.0078125);
        u_xlat3.x = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat3.xyz, saturate(u_xlat28), level(0.0));
        u_xlat4.xyz = u_xlat2.xyz + float3(-0.0078125, -0.0078125, 0.0078125);
        u_xlat3.y = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat4.xyz, saturate(u_xlat28), level(0.0));
        u_xlat4.xyz = u_xlat2.xyz + float3(-0.0078125, 0.0078125, -0.0078125);
        u_xlat3.z = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat4.xyz, saturate(u_xlat28), level(0.0));
        u_xlat2.xyz = u_xlat2.xyz + float3(0.0078125, -0.0078125, -0.0078125);
        u_xlat3.w = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat2.xyz, saturate(u_xlat28), level(0.0));
        u_xlat28 = dot(u_xlat3, float4(0.25, 0.25, 0.25, 0.25));
        u_xlat16_5.x = (-FGlobals._LightShadowData.x) + half(1.0);
        u_xlat16_5.x = half(fma(u_xlat28, float(u_xlat16_5.x), float(FGlobals._LightShadowData.x)));
    } else {
        u_xlat16_5.x = half(1.0);
    }
    u_xlat16_14 = (-u_xlat16_5.x) + half(1.0);
    u_xlat16_5.x = half(fma(u_xlat27, float(u_xlat16_14), float(u_xlat16_5.x)));
    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat27 = _LightTextureB0.sample(sampler_LightTextureB0, float2(u_xlat27)).x;
    u_xlat0.x = _LightTexture0.sample(sampler_LightTexture0, u_xlat0.xyz).w;
    u_xlat0.x = u_xlat0.x * u_xlat27;
    u_xlat0.x = float(u_xlat16_5.x) * u_xlat0.x;
    u_xlat16_5.xyz = half3(u_xlat0.xxx * float3(FGlobals._LightColor0.xyz));
    u_xlat16_6.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_5.xyz = u_xlat16_5.xyz / u_xlat16_6.xyz;
    u_xlat0.x = max(float(u_xlat16_5.y), float(u_xlat16_5.x));
    u_xlat0.x = max(float(u_xlat16_5.z), u_xlat0.x);
    u_xlat9.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_9.xy = _NormalMap.sample(sampler_NormalMap, u_xlat9.xy).yw;
    u_xlat16_5.xy = fma(u_xlat16_9.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_23 = dot(u_xlat16_5.xy, u_xlat16_5.xy);
    u_xlat16_23 = min(u_xlat16_23, half(1.0));
    u_xlat16_23 = (-u_xlat16_23) + half(1.0);
    u_xlat16_6.z = sqrt(u_xlat16_23);
    u_xlat16_6.xy = half2(float2(u_xlat16_5.xy) * float2(FGlobals._NormalScale));
    u_xlat16_5.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_6.xyz));
    u_xlat16_5.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_6.xyz));
    u_xlat16_5.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_6.xyz));
    u_xlat16_32 = dot(u_xlat16_5.xyz, u_xlat16_5.xyz);
    u_xlat16_32 = rsqrt(u_xlat16_32);
    u_xlat16_5.xyz = half3(u_xlat16_32) * u_xlat16_5.xyz;
    u_xlat9.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat28 = dot(u_xlat9.xyz, u_xlat9.xyz);
    u_xlat28 = rsqrt(u_xlat28);
    u_xlat9.xyz = u_xlat9.xyz * float3(u_xlat28);
    u_xlat9.x = dot(float3(u_xlat16_5.xyz), u_xlat9.xyz);
    u_xlat18 = u_xlat9.x + FGlobals._BaseCellOffset;
    u_xlat18 = u_xlat18 / FGlobals._BaseCellSharpness;
    u_xlat18 = clamp(u_xlat18, 0.0f, 1.0f);
    u_xlat27 = u_xlat0.x * u_xlat18;
    u_xlat28 = dot(FGlobals._CustomInverseDir.xyzx.xyz, FGlobals._CustomInverseDir.xyzx.xyz);
    u_xlat28 = max(u_xlat28, 0.00100000005);
    u_xlat28 = rsqrt(u_xlat28);
    u_xlat2.xyz = float3(u_xlat28) * FGlobals._CustomInverseDir.xyzx.xyz;
    u_xlat28 = dot(float3(u_xlat16_5.xyz), u_xlat2.xyz);
    u_xlat28 = u_xlat28 + FGlobals._inverseLightOffset;
    u_xlat28 = u_xlat28 / FGlobals._inverseLightSmooth;
    u_xlat28 = clamp(u_xlat28, 0.0f, 1.0f);
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_2.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat2.xy).xyz;
    u_xlat3.xyz = fma(float3(u_xlat16_2.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat3.xyz = fma(float3(u_xlat16_2.xyz), u_xlat3.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat2.xyz = float3(u_xlat16_2.xyz) * u_xlat3.xyz;
    u_xlat3.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat3.xyz = fma(FGlobals._BaseTint.xyz, u_xlat3.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xyz = u_xlat3.xyz * FGlobals._BaseTint.xyz;
    u_xlat4.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_29 = _AOTex.sample(sampler_AOTex, u_xlat4.xy).x;
    u_xlat30 = fma(float(u_xlat16_29), 0.305306017, 0.682171106);
    u_xlat30 = fma(float(u_xlat16_29), u_xlat30, 0.0125228781);
    u_xlat29 = fma(float(u_xlat16_29), u_xlat30, -1.0);
    u_xlat29 = fma(FGlobals._AOScale, u_xlat29, 1.0);
    u_xlat30 = input.TEXCOORD4.y * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].y;
    u_xlat30 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].y, input.TEXCOORD4.x, u_xlat30);
    u_xlat30 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].y, input.TEXCOORD4.z, u_xlat30);
    u_xlat30 = u_xlat30 + FGlobals.hlslcc_mtx4x4unity_WorldToObject[3].y;
    u_xlat30 = u_xlat30 * FGlobals._FadeColorScale;
    u_xlat30 = fma(u_xlat30, 0.5, FGlobals._FadeColorOffset);
    u_xlat30 = u_xlat30 + 0.5;
    u_xlat30 = clamp(u_xlat30, 0.0f, 1.0f);
    u_xlat4.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat4.xyz = fma(float3(u_xlat30), u_xlat4.xyz, FGlobals._FadeColorF.xyz);
    u_xlat2.xyz = u_xlat2.xyz * u_xlat3.xyz;
    u_xlat2.xyz = float3(u_xlat29) * u_xlat2.xyz;
    u_xlat2.xyz = u_xlat4.xyz * u_xlat2.xyz;
    u_xlat9.x = u_xlat9.x + FGlobals._HighLightOffset;
    u_xlat9.x = u_xlat9.x / FGlobals._HighLightSmooth;
    u_xlat9.x = clamp(u_xlat9.x, 0.0f, 1.0f);
    u_xlat29 = fma(u_xlat9.x, u_xlat9.x, (-u_xlat9.x));
    u_xlat9.x = fma(FGlobals._HighLightPing, u_xlat29, u_xlat9.x);
    u_xlat29 = fma(u_xlat18, u_xlat0.x, -1.0);
    u_xlat3.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), float2(u_xlat29), float2(1.0, 1.0));
    u_xlat4.xyz = u_xlat9.xxx * FGlobals._HighLightColor.xyz;
    u_xlat21.xy = float2(u_xlat16_5.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat21.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_5.xx), u_xlat21.xy);
    u_xlat21.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_5.zz), u_xlat21.xy);
    u_xlat21.xy = u_xlat21.xy * float2(FGlobals._ReflectScale);
    u_xlat21.xy = fma(u_xlat21.xy, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat21.xy = u_xlat21.xy + float2(0.5, 0.5);
    u_xlat16_7.xyz = _Matcap.sample(sampler_Matcap, u_xlat21.xy).xyz;
    u_xlat8.xyz = fma(float3(u_xlat16_7.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat8.xyz = fma(float3(u_xlat16_7.xyz), u_xlat8.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat7.xyz = float3(u_xlat16_7.xyz) * u_xlat8.xyz;
    u_xlat21.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_9.x = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat21.xy).x;
    u_xlat9.x = fma(FGlobals._RoughnessScale, (-float(u_xlat16_9.x)), 1.0);
    u_xlat7.xyz = u_xlat7.xyz * float3(FGlobals._MatcapScale);
    u_xlat12.xyz = u_xlat3.yyy * u_xlat7.xyz;
    u_xlat12.xyz = u_xlat12.xyz * FGlobals._MatcapColor.xyz;
    u_xlat7.xyz = fma(float3(u_xlat27), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat7.xyz = fma(float3(u_xlat28), FGlobals._inverseColor.xyz, u_xlat7.xyz);
    u_xlat0.x = fma((-u_xlat18), u_xlat0.x, 1.0);
    u_xlat0.xzw = u_xlat0.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xzw = fma(u_xlat7.xyz, u_xlat2.xyz, u_xlat0.xzw);
    u_xlat0.xzw = fma(u_xlat4.xyz, u_xlat3.xxx, u_xlat0.xzw);
    u_xlat0.xyz = fma(u_xlat12.xyz, u_xlat9.xxx, u_xlat0.xzw);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_5.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_5.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat27 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat27 = rsqrt(u_xlat27);
    u_xlat2.xyz = float3(u_xlat27) * u_xlat1.xyz;
    u_xlat1.xyz = fma((-u_xlat1.xyz), float3(u_xlat27), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat1.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat1.xyz, u_xlat2.xyz);
    u_xlat2.x = input.TEXCOORD1.z;
    u_xlat2.y = input.TEXCOORD2.z;
    u_xlat2.z = input.TEXCOORD3.z;
    u_xlat27 = dot(u_xlat2.xyz, u_xlat1.xyz);
    u_xlat1.x = fma(FGlobals._RimShadowScale, u_xlat29, 1.0);
    u_xlat27 = u_xlat27 + FGlobals._RimOffset;
    u_xlat27 = clamp(u_xlat27, 0.0f, 1.0f);
    u_xlat27 = (-u_xlat27) + 1.0;
    u_xlat27 = log2(u_xlat27);
    u_xlat27 = u_xlat27 * FGlobals._RimPower;
    u_xlat27 = exp2(u_xlat27);
    u_xlat27 = u_xlat1.x * u_xlat27;
    u_xlat1.xyz = float3(u_xlat27) * FGlobals._RimColor.xyz;
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(FGlobals._RimLightMul), u_xlat0.xyz);
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
