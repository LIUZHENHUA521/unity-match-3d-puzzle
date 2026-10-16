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
    float4 _WorldSpaceLightPos0;
    float4 hlslcc_mtx4x4unity_WorldToShadow[16];
    half4 _LightShadowData;
    float4 unity_ShadowFadeCenterAndType;
    float4 hlslcc_mtx4x4unity_MatrixV[4];
    float4 _ShadowOffsets[4];
    float4 hlslcc_mtx4x4unity_WorldToLight[4];
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
    half3 u_xlat16_2;
    bool u_xlatb2;
    float3 u_xlat3;
    half3 u_xlat16_4;
    half3 u_xlat16_5;
    float3 u_xlat6;
    float3 u_xlat7;
    half3 u_xlat16_7;
    float3 u_xlat8;
    float3 u_xlat9;
    half2 u_xlat16_9;
    float3 u_xlat10;
    half u_xlat16_10;
    bool u_xlatb10;
    float3 u_xlat12;
    half u_xlat16_13;
    float u_xlat18;
    float u_xlat19;
    float2 u_xlat21;
    half u_xlat16_22;
    float u_xlat27;
    float u_xlat29;
    half u_xlat16_29;
    float u_xlat30;
    half u_xlat16_31;
    u_xlat0 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1];
    u_xlat0 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0], input.TEXCOORD4.xxxx, u_xlat0);
    u_xlat0 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2], input.TEXCOORD4.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3];
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat2.x = FGlobals.hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat2.y = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat2.z = FGlobals.hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat1.x = dot(u_xlat1.xyz, u_xlat2.xyz);
    u_xlat10.xyz = input.TEXCOORD4.xyz + (-FGlobals.unity_ShadowFadeCenterAndType.xyz);
    u_xlat10.x = dot(u_xlat10.xyz, u_xlat10.xyz);
    u_xlat10.x = sqrt(u_xlat10.x);
    u_xlat10.x = (-u_xlat1.x) + u_xlat10.x;
    u_xlat1.x = fma(FGlobals.unity_ShadowFadeCenterAndType.w, u_xlat10.x, u_xlat1.x);
    u_xlat1.x = fma(u_xlat1.x, float(FGlobals._LightShadowData.z), float(FGlobals._LightShadowData.w));
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlatb10 = u_xlat1.x<0.99000001;
    if(u_xlatb10){
        u_xlat2 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToShadow[1];
        u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[0], input.TEXCOORD4.xxxx, u_xlat2);
        u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[2], input.TEXCOORD4.zzzz, u_xlat2);
        u_xlat2 = u_xlat2 + FGlobals.hlslcc_mtx4x4unity_WorldToShadow[3];
        u_xlat10.xyz = u_xlat2.xyz / u_xlat2.www;
        u_xlat2.xyz = u_xlat10.xyz + FGlobals._ShadowOffsets[0].xyz;
        u_xlat2.x = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat2.xy, saturate(u_xlat2.z), level(0.0));
        u_xlat3.xyz = u_xlat10.xyz + FGlobals._ShadowOffsets[1].xyz;
        u_xlat2.y = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat3.xy, saturate(u_xlat3.z), level(0.0));
        u_xlat3.xyz = u_xlat10.xyz + FGlobals._ShadowOffsets[2].xyz;
        u_xlat2.z = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat3.xy, saturate(u_xlat3.z), level(0.0));
        u_xlat10.xyz = u_xlat10.xyz + FGlobals._ShadowOffsets[3].xyz;
        u_xlat2.w = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat10.xy, saturate(u_xlat10.z), level(0.0));
        u_xlat10.x = dot(u_xlat2, float4(0.25, 0.25, 0.25, 0.25));
        u_xlat19 = (-float(FGlobals._LightShadowData.x)) + 1.0;
        u_xlat10.x = fma(u_xlat10.x, u_xlat19, float(FGlobals._LightShadowData.x));
        u_xlat16_10 = half(u_xlat10.x);
    } else {
        u_xlat16_10 = half(1.0);
    }
    u_xlat16_4.x = (-u_xlat16_10) + half(1.0);
    u_xlat16_4.x = half(fma(u_xlat1.x, float(u_xlat16_4.x), float(u_xlat16_10)));
    u_xlatb2 = 0.0<u_xlat0.z;
    u_xlat16_13 = (u_xlatb2) ? half(1.0) : half(0.0);
    u_xlat2.xy = u_xlat0.xy / u_xlat0.ww;
    u_xlat2.xy = u_xlat2.xy + float2(0.5, 0.5);
    u_xlat27 = _LightTexture0.sample(sampler_LightTexture0, u_xlat2.xy).w;
    u_xlat16_13 = half(u_xlat27 * float(u_xlat16_13));
    u_xlat0.x = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat0.x = _LightTextureB0.sample(sampler_LightTextureB0, u_xlat0.xx).x;
    u_xlat16_13 = half(u_xlat0.x * float(u_xlat16_13));
    u_xlat16_4.x = u_xlat16_4.x * u_xlat16_13;
    u_xlat16_4.xyz = u_xlat16_4.xxx * FGlobals._LightColor0.xyz;
    u_xlat16_5.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_4.xyz = u_xlat16_4.xyz / u_xlat16_5.xyz;
    u_xlat0.x = max(float(u_xlat16_4.y), float(u_xlat16_4.x));
    u_xlat0.x = max(float(u_xlat16_4.z), u_xlat0.x);
    u_xlat9.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_9.xy = _NormalMap.sample(sampler_NormalMap, u_xlat9.xy).yw;
    u_xlat16_4.xy = fma(u_xlat16_9.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_22 = dot(u_xlat16_4.xy, u_xlat16_4.xy);
    u_xlat16_22 = min(u_xlat16_22, half(1.0));
    u_xlat16_22 = (-u_xlat16_22) + half(1.0);
    u_xlat16_5.z = sqrt(u_xlat16_22);
    u_xlat16_5.xy = half2(float2(u_xlat16_4.xy) * float2(FGlobals._NormalScale));
    u_xlat16_4.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_4.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_4.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_31 = dot(u_xlat16_4.xyz, u_xlat16_4.xyz);
    u_xlat16_31 = rsqrt(u_xlat16_31);
    u_xlat16_4.xyz = half3(u_xlat16_31) * u_xlat16_4.xyz;
    u_xlat9.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat2.x = dot(u_xlat9.xyz, u_xlat9.xyz);
    u_xlat2.x = rsqrt(u_xlat2.x);
    u_xlat9.xyz = u_xlat9.xyz * u_xlat2.xxx;
    u_xlat9.x = dot(float3(u_xlat16_4.xyz), u_xlat9.xyz);
    u_xlat18 = u_xlat9.x + FGlobals._BaseCellOffset;
    u_xlat18 = u_xlat18 / FGlobals._BaseCellSharpness;
    u_xlat18 = clamp(u_xlat18, 0.0f, 1.0f);
    u_xlat27 = u_xlat0.x * u_xlat18;
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_2.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat2.xy).xyz;
    u_xlat3.xyz = fma(float3(u_xlat16_2.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat3.xyz = fma(float3(u_xlat16_2.xyz), u_xlat3.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat2.xyz = float3(u_xlat16_2.xyz) * u_xlat3.xyz;
    u_xlat3.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat3.xyz = fma(FGlobals._BaseTint.xyz, u_xlat3.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xyz = u_xlat3.xyz * FGlobals._BaseTint.xyz;
    u_xlat6.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_29 = _AOTex.sample(sampler_AOTex, u_xlat6.xy).x;
    u_xlat30 = fma(float(u_xlat16_29), 0.305306017, 0.682171106);
    u_xlat30 = fma(float(u_xlat16_29), u_xlat30, 0.0125228781);
    u_xlat29 = fma(float(u_xlat16_29), u_xlat30, -1.0);
    u_xlat29 = fma(FGlobals._AOScale, u_xlat29, 1.0);
    u_xlat2.xyz = u_xlat2.xyz * u_xlat3.xyz;
    u_xlat2.xyz = float3(u_xlat29) * u_xlat2.xyz;
    u_xlat9.x = u_xlat9.x + FGlobals._HighLightOffset;
    u_xlat9.x = u_xlat9.x / FGlobals._HighLightSmooth;
    u_xlat9.x = clamp(u_xlat9.x, 0.0f, 1.0f);
    u_xlat29 = fma(u_xlat9.x, u_xlat9.x, (-u_xlat9.x));
    u_xlat9.x = fma(FGlobals._HighLightPing, u_xlat29, u_xlat9.x);
    u_xlat29 = fma(u_xlat18, u_xlat0.x, -1.0);
    u_xlat3.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), float2(u_xlat29), float2(1.0, 1.0));
    u_xlat6.xyz = u_xlat9.xxx * FGlobals._HighLightColor.xyz;
    u_xlat21.xy = float2(u_xlat16_4.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat21.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_4.xx), u_xlat21.xy);
    u_xlat21.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_4.zz), u_xlat21.xy);
    u_xlat21.xy = u_xlat21.xy * float2(FGlobals._ReflectScale);
    u_xlat21.xy = fma(u_xlat21.xy, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat21.xy = u_xlat21.xy + float2(0.5, 0.5);
    u_xlat16_7.xyz = _Matcap.sample(sampler_Matcap, u_xlat21.xy).xyz;
    u_xlat8.xyz = fma(float3(u_xlat16_7.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat8.xyz = fma(float3(u_xlat16_7.xyz), u_xlat8.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat7.xyz = float3(u_xlat16_7.xyz) * u_xlat8.xyz;
    u_xlat9.x = max(u_xlat7.y, u_xlat7.x);
    u_xlat9.x = max(u_xlat7.z, u_xlat9.x);
    u_xlat21.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_29 = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat21.xy).x;
    u_xlat29 = fma(FGlobals._RoughnessScale, (-float(u_xlat16_29)), 1.0);
    u_xlat9.x = u_xlat9.x * FGlobals._MatcapScale;
    u_xlat9.x = u_xlat9.x * u_xlat3.y;
    u_xlat12.xyz = u_xlat9.xxx * FGlobals._MatcapColor.xyz;
    u_xlat7.xyz = fma(float3(u_xlat27), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat0.x = fma((-u_xlat18), u_xlat0.x, 1.0);
    u_xlat0.xyz = u_xlat0.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xyz = fma(u_xlat7.xyz, u_xlat2.xyz, u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat6.xyz, u_xlat3.xxx, u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat12.xyz, float3(u_xlat29), u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_4.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_4.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
