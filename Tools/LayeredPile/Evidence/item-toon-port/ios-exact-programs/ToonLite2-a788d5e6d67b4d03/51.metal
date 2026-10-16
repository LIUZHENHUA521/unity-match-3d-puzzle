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
    float4 _LightPositionRange;
    float4 _LightProjectionParams;
    half4 _LightShadowData;
    float4 unity_ShadowFadeCenterAndType;
    float4 hlslcc_mtx4x4unity_MatrixV[4];
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
    sampler sampler_NormalMap [[ sampler (2) ]],
    sampler sampler_BaseColor [[ sampler (3) ]],
    sampler sampler_AOTex [[ sampler (4) ]],
    sampler sampler_Matcap [[ sampler (5) ]],
    sampler sampler_RoughnessMap [[ sampler (6) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(3) ]] ,
    texture2d<half, access::sample > _RoughnessMap [[ texture(4) ]] ,
    texture2d<float, access::sample > _LightTexture0 [[ texture(5) ]] ,
    depthcube<float, access::sample > _ShadowMapTexture [[ texture(6) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float4 u_xlat0;
    float3 u_xlat1;
    half3 u_xlat16_1;
    bool u_xlatb1;
    float4 u_xlat2;
    float3 u_xlat3;
    half3 u_xlat16_4;
    half3 u_xlat16_5;
    float3 u_xlat6;
    half3 u_xlat16_6;
    float3 u_xlat7;
    float3 u_xlat8;
    half2 u_xlat16_8;
    float3 u_xlat10;
    half u_xlat16_12;
    float u_xlat16;
    float2 u_xlat18;
    half u_xlat16_20;
    float u_xlat24;
    float u_xlat25;
    half u_xlat16_25;
    float u_xlat26;
    half u_xlat16_28;
    u_xlat0.xyz = input.TEXCOORD4.yyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1].xyz;
    u_xlat0.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0].xyz, input.TEXCOORD4.xxx, u_xlat0.xyz);
    u_xlat0.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2].xyz, input.TEXCOORD4.zzz, u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3].xyz;
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat2.x = FGlobals.hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat2.y = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat2.z = FGlobals.hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat24 = dot(u_xlat1.xyz, u_xlat2.xyz);
    u_xlat1.xyz = input.TEXCOORD4.xyz + (-FGlobals.unity_ShadowFadeCenterAndType.xyz);
    u_xlat1.x = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat1.x = sqrt(u_xlat1.x);
    u_xlat1.x = (-u_xlat24) + u_xlat1.x;
    u_xlat24 = fma(FGlobals.unity_ShadowFadeCenterAndType.w, u_xlat1.x, u_xlat24);
    u_xlat24 = fma(u_xlat24, float(FGlobals._LightShadowData.z), float(FGlobals._LightShadowData.w));
    u_xlat24 = clamp(u_xlat24, 0.0f, 1.0f);
    u_xlatb1 = u_xlat24<0.99000001;
    if(u_xlatb1){
        u_xlat1.xyz = input.TEXCOORD4.xyz + (-FGlobals._LightPositionRange.xyz);
        u_xlat25 = max(abs(u_xlat1.y), abs(u_xlat1.x));
        u_xlat25 = max(abs(u_xlat1.z), u_xlat25);
        u_xlat25 = u_xlat25 + (-FGlobals._LightProjectionParams.z);
        u_xlat25 = max(u_xlat25, 9.99999975e-06);
        u_xlat25 = u_xlat25 * FGlobals._LightProjectionParams.w;
        u_xlat25 = FGlobals._LightProjectionParams.y / u_xlat25;
        u_xlat25 = u_xlat25 + (-FGlobals._LightProjectionParams.x);
        u_xlat25 = (-u_xlat25) + 1.0;
        u_xlat2.xyz = u_xlat1.xyz + float3(0.0078125, 0.0078125, 0.0078125);
        u_xlat2.x = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat2.xyz, saturate(u_xlat25), level(0.0));
        u_xlat3.xyz = u_xlat1.xyz + float3(-0.0078125, -0.0078125, 0.0078125);
        u_xlat2.y = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat3.xyz, saturate(u_xlat25), level(0.0));
        u_xlat3.xyz = u_xlat1.xyz + float3(-0.0078125, 0.0078125, -0.0078125);
        u_xlat2.z = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat3.xyz, saturate(u_xlat25), level(0.0));
        u_xlat1.xyz = u_xlat1.xyz + float3(0.0078125, -0.0078125, -0.0078125);
        u_xlat2.w = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat1.xyz, saturate(u_xlat25), level(0.0));
        u_xlat1.x = dot(u_xlat2, float4(0.25, 0.25, 0.25, 0.25));
        u_xlat16_4.x = (-FGlobals._LightShadowData.x) + half(1.0);
        u_xlat16_4.x = half(fma(u_xlat1.x, float(u_xlat16_4.x), float(FGlobals._LightShadowData.x)));
    } else {
        u_xlat16_4.x = half(1.0);
    }
    u_xlat16_12 = (-u_xlat16_4.x) + half(1.0);
    u_xlat16_4.x = half(fma(u_xlat24, float(u_xlat16_12), float(u_xlat16_4.x)));
    u_xlat0.x = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat0.x = _LightTexture0.sample(sampler_LightTexture0, u_xlat0.xx).x;
    u_xlat0.x = float(u_xlat16_4.x) * u_xlat0.x;
    u_xlat16_4.xyz = half3(u_xlat0.xxx * float3(FGlobals._LightColor0.xyz));
    u_xlat16_5.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_4.xyz = u_xlat16_4.xyz / u_xlat16_5.xyz;
    u_xlat0.x = max(float(u_xlat16_4.y), float(u_xlat16_4.x));
    u_xlat0.x = max(float(u_xlat16_4.z), u_xlat0.x);
    u_xlat8.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_8.xy = _NormalMap.sample(sampler_NormalMap, u_xlat8.xy).yw;
    u_xlat16_4.xy = fma(u_xlat16_8.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_20 = dot(u_xlat16_4.xy, u_xlat16_4.xy);
    u_xlat16_20 = min(u_xlat16_20, half(1.0));
    u_xlat16_20 = (-u_xlat16_20) + half(1.0);
    u_xlat16_5.z = sqrt(u_xlat16_20);
    u_xlat16_5.xy = half2(float2(u_xlat16_4.xy) * float2(FGlobals._NormalScale));
    u_xlat16_4.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_4.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_4.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_28 = dot(u_xlat16_4.xyz, u_xlat16_4.xyz);
    u_xlat16_28 = rsqrt(u_xlat16_28);
    u_xlat16_4.xyz = half3(u_xlat16_28) * u_xlat16_4.xyz;
    u_xlat8.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat1.x = dot(u_xlat8.xyz, u_xlat8.xyz);
    u_xlat1.x = rsqrt(u_xlat1.x);
    u_xlat8.xyz = u_xlat8.xyz * u_xlat1.xxx;
    u_xlat8.x = dot(float3(u_xlat16_4.xyz), u_xlat8.xyz);
    u_xlat16 = u_xlat8.x + FGlobals._BaseCellOffset;
    u_xlat16 = u_xlat16 / FGlobals._BaseCellSharpness;
    u_xlat16 = clamp(u_xlat16, 0.0f, 1.0f);
    u_xlat24 = u_xlat0.x * u_xlat16;
    u_xlat1.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_1.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat1.xy).xyz;
    u_xlat2.xyz = fma(float3(u_xlat16_1.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat2.xyz = fma(float3(u_xlat16_1.xyz), u_xlat2.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = float3(u_xlat16_1.xyz) * u_xlat2.xyz;
    u_xlat2.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat2.xyz = fma(FGlobals._BaseTint.xyz, u_xlat2.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat2.xyz = u_xlat2.xyz * FGlobals._BaseTint.xyz;
    u_xlat3.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_25 = _AOTex.sample(sampler_AOTex, u_xlat3.xy).x;
    u_xlat26 = fma(float(u_xlat16_25), 0.305306017, 0.682171106);
    u_xlat26 = fma(float(u_xlat16_25), u_xlat26, 0.0125228781);
    u_xlat25 = fma(float(u_xlat16_25), u_xlat26, -1.0);
    u_xlat25 = fma(FGlobals._AOScale, u_xlat25, 1.0);
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xyz;
    u_xlat1.xyz = float3(u_xlat25) * u_xlat1.xyz;
    u_xlat8.x = u_xlat8.x + FGlobals._HighLightOffset;
    u_xlat8.x = u_xlat8.x / FGlobals._HighLightSmooth;
    u_xlat8.x = clamp(u_xlat8.x, 0.0f, 1.0f);
    u_xlat25 = fma(u_xlat8.x, u_xlat8.x, (-u_xlat8.x));
    u_xlat8.x = fma(FGlobals._HighLightPing, u_xlat25, u_xlat8.x);
    u_xlat25 = fma(u_xlat16, u_xlat0.x, -1.0);
    u_xlat2.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), float2(u_xlat25), float2(1.0, 1.0));
    u_xlat3.xyz = u_xlat8.xxx * FGlobals._HighLightColor.xyz;
    u_xlat18.xy = float2(u_xlat16_4.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat18.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_4.xx), u_xlat18.xy);
    u_xlat18.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_4.zz), u_xlat18.xy);
    u_xlat18.xy = u_xlat18.xy * float2(FGlobals._ReflectScale);
    u_xlat18.xy = fma(u_xlat18.xy, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat18.xy = u_xlat18.xy + float2(0.5, 0.5);
    u_xlat16_6.xyz = _Matcap.sample(sampler_Matcap, u_xlat18.xy).xyz;
    u_xlat7.xyz = fma(float3(u_xlat16_6.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat7.xyz = fma(float3(u_xlat16_6.xyz), u_xlat7.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat6.xyz = float3(u_xlat16_6.xyz) * u_xlat7.xyz;
    u_xlat18.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_8.x = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat18.xy).x;
    u_xlat8.x = fma(FGlobals._RoughnessScale, (-float(u_xlat16_8.x)), 1.0);
    u_xlat6.xyz = u_xlat6.xyz * float3(FGlobals._MatcapScale);
    u_xlat10.xyz = u_xlat2.yyy * u_xlat6.xyz;
    u_xlat10.xyz = u_xlat10.xyz * FGlobals._MatcapColor.xyz;
    u_xlat6.xyz = fma(float3(u_xlat24), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat0.x = fma((-u_xlat16), u_xlat0.x, 1.0);
    u_xlat0.xzw = u_xlat0.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xzw = fma(u_xlat6.xyz, u_xlat1.xyz, u_xlat0.xzw);
    u_xlat0.xzw = fma(u_xlat3.xyz, u_xlat2.xxx, u_xlat0.xzw);
    u_xlat0.xyz = fma(u_xlat10.xyz, u_xlat8.xxx, u_xlat0.xzw);
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
