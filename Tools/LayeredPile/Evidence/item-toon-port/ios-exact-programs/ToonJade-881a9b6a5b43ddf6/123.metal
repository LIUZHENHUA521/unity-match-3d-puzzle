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
    float _ReflectScale;
    float4 _BaseTint;
    float4 _AOTex_ST;
    float _AOScale;
    float4 _ShadowAddColor;
    float _HighLightOffset;
    float _HighLightSmooth;
    float _HighLightPing;
    float4 _HighLightColor;
    float _HighLightShadowMask;
    float4 _FadeColorF;
    float4 _FadeColorB;
    float _FadeColorScale;
    float _FadeColorOffset;
    float _GradientNoiseScale;
    float _GradientNoiseOffset;
    float4 _ColorNoise1Dark;
    float4 _ColorNoise1Light;
    float _Noise1Scale;
    float3 _Noise1Offset;
    float _Noise1Draker;
    float _Noise1Lighter;
    float4 _ColorNoise2Dark;
    float4 _ColorNoise2Light;
    float _Noise2Scale;
    float3 _Noise2Offset;
    float _Noise2Draker;
    float _Noise2Lighter;
    float _MatcapShadowMask;
    float2 _ReflectOffset;
    float _MatcapScale;
    float4 _MatcapColor;
    float _RimOffset2;
    float _RimPower2;
    float4 _RimColor2;
    float _RimLightMul2;
    float _RimOffset;
    float _RimPower;
    float _RimShadowScale;
    float4 _RimColor;
    float _RimLightMul;
    float _RimNoiseScale;
    float _RimNoiseOffset;
    float4 _RefractionColorDark;
    float4 _RefractionColorLight;
    float _RefractionNoiseScale;
    float _RefractionNoiseOffset;
    float _RefractionShadowMask;
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
    sampler sampler_GradientNoise [[ sampler (6) ]],
    sampler sampler_ColorNoise1 [[ sampler (7) ]],
    sampler sampler_ColorNoise2 [[ sampler (8) ]],
    sampler sampler_Matcap [[ sampler (9) ]],
    sampler sampler_RimNoise [[ sampler (10) ]],
    sampler sampler_RefractionNoise [[ sampler (11) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _GradientNoise [[ texture(3) ]] ,
    texture2d<half, access::sample > _ColorNoise1 [[ texture(4) ]] ,
    texture2d<half, access::sample > _ColorNoise2 [[ texture(5) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(6) ]] ,
    texture2d<float, access::sample > _LightTextureB0 [[ texture(7) ]] ,
    texturecube<float, access::sample > _LightTexture0 [[ texture(8) ]] ,
    texture2d<half, access::sample > _RimNoise [[ texture(9) ]] ,
    texture2d<half, access::sample > _RefractionNoise [[ texture(10) ]] ,
    depthcube<float, access::sample > _ShadowMapTexture [[ texture(11) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float3 u_xlat0;
    float4 u_xlat1;
    float4 u_xlat2;
    half u_xlat16_2;
    float4 u_xlat3;
    half3 u_xlat16_3;
    float4 u_xlat4;
    half3 u_xlat16_4;
    int3 u_xlati4;
    float4 u_xlat5;
    half3 u_xlat16_5;
    float4 u_xlat6;
    half3 u_xlat16_6;
    float3 u_xlat7;
    float3 u_xlat8;
    int3 u_xlati8;
    float3 u_xlat9;
    half3 u_xlat16_10;
    float3 u_xlat11;
    float3 u_xlat12;
    half2 u_xlat16_12;
    float u_xlat13;
    half u_xlat16_13;
    float2 u_xlat14;
    half u_xlat16_14;
    half u_xlat16_17;
    float u_xlat24;
    float u_xlat25;
    half u_xlat16_25;
    float2 u_xlat27;
    half u_xlat16_27;
    half u_xlat16_29;
    float2 u_xlat32;
    float u_xlat36;
    half u_xlat16_36;
    float u_xlat37;
    half u_xlat16_37;
    bool u_xlatb37;
    float u_xlat39;
    half u_xlat16_39;
    half u_xlat16_41;
    u_xlat0.xyz = input.TEXCOORD4.yyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1].xyz;
    u_xlat0.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0].xyz, input.TEXCOORD4.xxx, u_xlat0.xyz);
    u_xlat0.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2].xyz, input.TEXCOORD4.zzz, u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3].xyz;
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat2.x = FGlobals.hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat2.y = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat2.z = FGlobals.hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat36 = dot(u_xlat1.xyz, u_xlat2.xyz);
    u_xlat2.xyz = input.TEXCOORD4.xyz + (-FGlobals.unity_ShadowFadeCenterAndType.xyz);
    u_xlat37 = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat37 = sqrt(u_xlat37);
    u_xlat37 = (-u_xlat36) + u_xlat37;
    u_xlat36 = fma(FGlobals.unity_ShadowFadeCenterAndType.w, u_xlat37, u_xlat36);
    u_xlat36 = fma(u_xlat36, float(FGlobals._LightShadowData.z), float(FGlobals._LightShadowData.w));
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlatb37 = u_xlat36<0.99000001;
    if(u_xlatb37){
        u_xlat2.xyz = input.TEXCOORD4.xyz + (-FGlobals._LightPositionRange.xyz);
        u_xlat37 = max(abs(u_xlat2.y), abs(u_xlat2.x));
        u_xlat37 = max(abs(u_xlat2.z), u_xlat37);
        u_xlat37 = u_xlat37 + (-FGlobals._LightProjectionParams.z);
        u_xlat37 = max(u_xlat37, 9.99999975e-06);
        u_xlat37 = u_xlat37 * FGlobals._LightProjectionParams.w;
        u_xlat37 = FGlobals._LightProjectionParams.y / u_xlat37;
        u_xlat37 = u_xlat37 + (-FGlobals._LightProjectionParams.x);
        u_xlat37 = (-u_xlat37) + 1.0;
        u_xlat3.xyz = u_xlat2.xyz + float3(0.0078125, 0.0078125, 0.0078125);
        u_xlat3.x = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat3.xyz, saturate(u_xlat37), level(0.0));
        u_xlat4.xyz = u_xlat2.xyz + float3(-0.0078125, -0.0078125, 0.0078125);
        u_xlat3.y = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat4.xyz, saturate(u_xlat37), level(0.0));
        u_xlat4.xyz = u_xlat2.xyz + float3(-0.0078125, 0.0078125, -0.0078125);
        u_xlat3.z = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat4.xyz, saturate(u_xlat37), level(0.0));
        u_xlat2.xyz = u_xlat2.xyz + float3(0.0078125, -0.0078125, -0.0078125);
        u_xlat3.w = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat2.xyz, saturate(u_xlat37), level(0.0));
        u_xlat37 = dot(u_xlat3, float4(0.25, 0.25, 0.25, 0.25));
        u_xlat16_5.x = (-FGlobals._LightShadowData.x) + half(1.0);
        u_xlat16_5.x = half(fma(u_xlat37, float(u_xlat16_5.x), float(FGlobals._LightShadowData.x)));
    } else {
        u_xlat16_5.x = half(1.0);
    }
    u_xlat16_17 = (-u_xlat16_5.x) + half(1.0);
    u_xlat16_5.x = half(fma(u_xlat36, float(u_xlat16_17), float(u_xlat16_5.x)));
    u_xlat36 = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat36 = _LightTextureB0.sample(sampler_LightTextureB0, float2(u_xlat36)).x;
    u_xlat0.x = _LightTexture0.sample(sampler_LightTexture0, u_xlat0.xyz).w;
    u_xlat0.x = u_xlat0.x * u_xlat36;
    u_xlat0.x = float(u_xlat16_5.x) * u_xlat0.x;
    u_xlat16_5.xyz = half3(u_xlat0.xxx * float3(FGlobals._LightColor0.xyz));
    u_xlat16_6.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_5.xyz = u_xlat16_5.xyz / u_xlat16_6.xyz;
    u_xlat0.x = max(float(u_xlat16_5.y), float(u_xlat16_5.x));
    u_xlat0.x = max(float(u_xlat16_5.z), u_xlat0.x);
    u_xlat12.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_12.xy = _NormalMap.sample(sampler_NormalMap, u_xlat12.xy).yw;
    u_xlat16_5.xy = fma(u_xlat16_12.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_29 = dot(u_xlat16_5.xy, u_xlat16_5.xy);
    u_xlat16_29 = min(u_xlat16_29, half(1.0));
    u_xlat16_29 = (-u_xlat16_29) + half(1.0);
    u_xlat16_6.z = sqrt(u_xlat16_29);
    u_xlat16_6.xy = half2(float2(u_xlat16_5.xy) * float2(FGlobals._NormalScale));
    u_xlat16_5.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_6.xyz));
    u_xlat16_5.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_6.xyz));
    u_xlat16_5.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_6.xyz));
    u_xlat16_41 = dot(u_xlat16_5.xyz, u_xlat16_5.xyz);
    u_xlat16_41 = rsqrt(u_xlat16_41);
    u_xlat16_5.xyz = half3(u_xlat16_41) * u_xlat16_5.xyz;
    u_xlat12.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat37 = dot(u_xlat12.xyz, u_xlat12.xyz);
    u_xlat37 = rsqrt(u_xlat37);
    u_xlat12.xyz = u_xlat12.xyz * float3(u_xlat37);
    u_xlat12.x = dot(float3(u_xlat16_5.xyz), u_xlat12.xyz);
    u_xlat24 = u_xlat12.x + FGlobals._BaseCellOffset;
    u_xlat24 = u_xlat24 / FGlobals._BaseCellSharpness;
    u_xlat24 = clamp(u_xlat24, 0.0f, 1.0f);
    u_xlat36 = u_xlat0.x * u_xlat24;
    u_xlat2 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].zyxz;
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].zyxz, input.TEXCOORD4.xxxx, u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].zyxz, input.TEXCOORD4.zzzz, u_xlat2);
    u_xlat2 = u_xlat2 + FGlobals.hlslcc_mtx4x4unity_WorldToObject[3].zyxz;
    u_xlat3.xy = float2(u_xlat16_5.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat3.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_5.xx), u_xlat3.xy);
    u_xlat3.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_5.zz), u_xlat3.xy);
    u_xlat3.xy = u_xlat3.xy * float2(FGlobals._ReflectScale);
    u_xlat3.xy = fma(u_xlat3.xy, float2(0.5, 0.5), float2(0.5, 0.5));
    u_xlat16_4.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat3.xy).xyz;
    u_xlat7.xyz = fma(float3(u_xlat16_4.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat7.xyz = fma(float3(u_xlat16_4.xyz), u_xlat7.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat4.xyz = float3(u_xlat16_4.xyz) * u_xlat7.xyz;
    u_xlat7.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat7.xyz = fma(FGlobals._BaseTint.xyz, u_xlat7.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat7.xyz = u_xlat7.xyz * FGlobals._BaseTint.xyz;
    u_xlat27.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_37 = _AOTex.sample(sampler_AOTex, u_xlat27.xy).x;
    u_xlat27.x = fma(float(u_xlat16_37), 0.305306017, 0.682171106);
    u_xlat27.x = fma(float(u_xlat16_37), u_xlat27.x, 0.0125228781);
    u_xlat37 = fma(float(u_xlat16_37), u_xlat27.x, -1.0);
    u_xlat37 = fma(FGlobals._AOScale, u_xlat37, 1.0);
    u_xlat4.xyz = u_xlat4.xyz * u_xlat7.xyz;
    u_xlat4.xyz = float3(u_xlat37) * u_xlat4.xyz;
    u_xlat12.x = u_xlat12.x + FGlobals._HighLightOffset;
    u_xlat12.x = u_xlat12.x / FGlobals._HighLightSmooth;
    u_xlat12.x = clamp(u_xlat12.x, 0.0f, 1.0f);
    u_xlat37 = fma(u_xlat12.x, u_xlat12.x, (-u_xlat12.x));
    u_xlat12.x = fma(FGlobals._HighLightPing, u_xlat37, u_xlat12.x);
    u_xlat37 = fma(u_xlat24, u_xlat0.x, -1.0);
    u_xlat27.x = fma(FGlobals._HighLightShadowMask, u_xlat37, 1.0);
    u_xlat7.xyz = u_xlat12.xxx * FGlobals._HighLightColor.xyz;
    u_xlat8.xyz = fma(float3(u_xlat36), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat0.x = fma((-u_xlat24), u_xlat0.x, 1.0);
    u_xlat0.xyz = u_xlat0.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xyz = fma(u_xlat8.xyz, u_xlat4.xyz, u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat7.xyz, u_xlat27.xxx, u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat4.xyz = input.TEXCOORD2.zzz * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].xyz;
    u_xlat4.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].xyz, input.TEXCOORD1.zzz, u_xlat4.xyz);
    u_xlat4.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].xyz, input.TEXCOORD3.zzz, u_xlat4.xyz);
    u_xlat36 = dot(u_xlat4.xyz, u_xlat4.xyz);
    u_xlat36 = rsqrt(u_xlat36);
    u_xlat4.xyz = float3(u_xlat36) * u_xlat4.xyz;
    u_xlat5 = u_xlat2.wyzw + float4(FGlobals._GradientNoiseOffset);
    u_xlat36 = abs(u_xlat4.y) + abs(u_xlat4.x);
    u_xlat36 = abs(u_xlat4.z) + u_xlat36;
    u_xlat36 = u_xlat36 + 9.99999975e-06;
    u_xlat7.xyz = abs(u_xlat4.xyz) / float3(u_xlat36);
    u_xlati8.xyz = int3(uint3((float3(0.0, 0.0, 0.0)<u_xlat4.xyz)) * 0xFFFFFFFFu);
    u_xlati4.xyz = int3(uint3((u_xlat4.xyz<float3(0.0, 0.0, 0.0))) * 0xFFFFFFFFu);
    u_xlati4.xyz = (-u_xlati8.xyz) + u_xlati4.xyz;
    u_xlat4.xyz = float3(u_xlati4.xyz);
    u_xlat6 = u_xlat5 * float4(FGlobals._GradientNoiseScale);
    u_xlat4.w = 1.0;
    u_xlat6 = u_xlat4.xwyw * u_xlat6;
    u_xlat16_36 = _GradientNoise.sample(sampler_GradientNoise, u_xlat6.xy).x;
    u_xlat16_27 = _GradientNoise.sample(sampler_GradientNoise, u_xlat6.zw).x;
    u_xlat8.xy = u_xlat5.zy * float2(FGlobals._GradientNoiseScale);
    u_xlat32.xy = u_xlat4.zw * float2(-1.0, 1.0);
    u_xlat8.xy = u_xlat32.xy * u_xlat8.xy;
    u_xlat16_39 = _GradientNoise.sample(sampler_GradientNoise, u_xlat8.xy).x;
    u_xlat27.x = u_xlat7.y * float(u_xlat16_27);
    u_xlat36 = fma(float(u_xlat16_36), u_xlat7.x, u_xlat27.x);
    u_xlat36 = fma(float(u_xlat16_39), u_xlat7.z, u_xlat36);
    u_xlat27.x = u_xlat2.y * FGlobals._FadeColorScale;
    u_xlat27.x = fma(u_xlat27.x, 0.5, FGlobals._FadeColorOffset);
    u_xlat36 = u_xlat36 + u_xlat27.x;
    u_xlat36 = u_xlat36 + 0.5;
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlat9.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat9.xyz = fma(float3(u_xlat36), u_xlat9.xyz, FGlobals._FadeColorF.xyz);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat9.xyz;
    u_xlat16_10.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_10.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat5 = u_xlat2.wyzw + FGlobals._Noise1Offset.xxyz.wzyw;
    u_xlat6 = u_xlat5 * float4(FGlobals._Noise1Scale);
    u_xlat6 = u_xlat4.xwyw * u_xlat6;
    u_xlat16_36 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat6.xy).x;
    u_xlat16_27 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat6.zw).x;
    u_xlat8.xy = u_xlat5.zy * float2(FGlobals._Noise1Scale);
    u_xlat8.xy = u_xlat32.xy * u_xlat8.xy;
    u_xlat16_39 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat8.xy).x;
    u_xlat27.x = u_xlat7.y * float(u_xlat16_27);
    u_xlat36 = fma(float(u_xlat16_36), u_xlat7.x, u_xlat27.x);
    u_xlat36 = fma(float(u_xlat16_39), u_xlat7.z, u_xlat36);
    u_xlat27.x = fma(u_xlat36, 0.305306017, 0.682171106);
    u_xlat27.x = fma(u_xlat36, u_xlat27.x, 0.0125228781);
    u_xlat36 = fma(u_xlat36, u_xlat27.x, (-FGlobals._Noise1Draker));
    u_xlat27.x = FGlobals._Noise1Draker + 0.00999999978;
    u_xlat39 = (-FGlobals._Noise1Lighter) + 1.0;
    u_xlat27.x = max(u_xlat39, u_xlat27.x);
    u_xlat27.x = u_xlat27.x + (-FGlobals._Noise1Draker);
    u_xlat36 = u_xlat36 / u_xlat27.x;
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlat9.xyz = (-FGlobals._ColorNoise1Dark.xyz) + FGlobals._ColorNoise1Light.xyz;
    u_xlat9.xyz = fma(float3(u_xlat36), u_xlat9.xyz, FGlobals._ColorNoise1Dark.xyz);
    u_xlat5 = u_xlat2.wyzw + FGlobals._Noise2Offset.xxyz.wzyw;
    u_xlat6 = u_xlat5 * float4(FGlobals._Noise2Scale);
    u_xlat6 = u_xlat4.xwyw * u_xlat6;
    u_xlat16_36 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat6.xy).y;
    u_xlat16_27 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat6.zw).y;
    u_xlat8.xy = u_xlat5.zy * float2(FGlobals._Noise2Scale);
    u_xlat8.xy = u_xlat32.xy * u_xlat8.xy;
    u_xlat16_39 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat8.xy).y;
    u_xlat27.x = u_xlat7.y * float(u_xlat16_27);
    u_xlat36 = fma(float(u_xlat16_36), u_xlat7.x, u_xlat27.x);
    u_xlat36 = fma(float(u_xlat16_39), u_xlat7.z, u_xlat36);
    u_xlat27.x = fma(u_xlat36, 0.305306017, 0.682171106);
    u_xlat27.x = fma(u_xlat36, u_xlat27.x, 0.0125228781);
    u_xlat36 = fma(u_xlat36, u_xlat27.x, (-FGlobals._Noise2Draker));
    u_xlat27.x = FGlobals._Noise2Draker + 0.00999999978;
    u_xlat39 = (-FGlobals._Noise2Lighter) + 1.0;
    u_xlat27.x = max(u_xlat39, u_xlat27.x);
    u_xlat27.x = u_xlat27.x + (-FGlobals._Noise2Draker);
    u_xlat36 = u_xlat36 / u_xlat27.x;
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlat11.xyz = (-FGlobals._ColorNoise2Dark.xyz) + FGlobals._ColorNoise2Light.xyz;
    u_xlat11.xyz = fma(float3(u_xlat36), u_xlat11.xyz, FGlobals._ColorNoise2Dark.xyz);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat9.xyz;
    u_xlat36 = fma(FGlobals._MatcapShadowMask, u_xlat37, 1.0);
    u_xlat3.xy = u_xlat3.xy + FGlobals._ReflectOffset.xyxx.xy;
    u_xlat16_3.xyz = _Matcap.sample(sampler_Matcap, u_xlat3.xy).xyz;
    u_xlat9.xyz = fma(float3(u_xlat16_3.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat9.xyz = fma(float3(u_xlat16_3.xyz), u_xlat9.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xyz = float3(u_xlat16_3.xyz) * u_xlat9.xyz;
    u_xlat3.xyz = u_xlat3.xyz * float3(FGlobals._MatcapScale);
    u_xlat3.xyz = float3(u_xlat36) * u_xlat3.xyz;
    u_xlat3.xyz = u_xlat3.xyz * FGlobals._MatcapColor.xyz;
    u_xlat0.xyz = fma(u_xlat0.xyz, u_xlat11.xyz, u_xlat3.xyz);
    u_xlat16_10.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_10.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat36 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat36 = rsqrt(u_xlat36);
    u_xlat3.xyz = float3(u_xlat36) * u_xlat1.xyz;
    u_xlat1.xyz = fma((-u_xlat1.xyz), float3(u_xlat36), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat1.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat1.xyz, u_xlat3.xyz);
    u_xlat3.x = input.TEXCOORD1.z;
    u_xlat3.y = input.TEXCOORD2.z;
    u_xlat3.z = input.TEXCOORD3.z;
    u_xlat36 = dot(u_xlat3.xyz, u_xlat1.xyz);
    u_xlat1.x = fma(FGlobals._RimShadowScale, u_xlat37, 1.0);
    u_xlat3 = u_xlat2.wyzw + float4(FGlobals._RimNoiseOffset);
    u_xlat5 = u_xlat3 * float4(FGlobals._RimNoiseScale);
    u_xlat5 = u_xlat4.xwyw * u_xlat5;
    u_xlat16_13 = _RimNoise.sample(sampler_RimNoise, u_xlat5.xy).x;
    u_xlat16_25 = _RimNoise.sample(sampler_RimNoise, u_xlat5.zw).x;
    u_xlat3.xy = u_xlat3.zy * float2(FGlobals._RimNoiseScale);
    u_xlat3.xy = u_xlat32.xy * u_xlat3.xy;
    u_xlat16_3.x = _RimNoise.sample(sampler_RimNoise, u_xlat3.xy).x;
    u_xlat25 = u_xlat7.y * float(u_xlat16_25);
    u_xlat13 = fma(float(u_xlat16_13), u_xlat7.x, u_xlat25);
    u_xlat13 = fma(float(u_xlat16_3.x), u_xlat7.z, u_xlat13);
    u_xlat2 = u_xlat2 + float4(FGlobals._RefractionNoiseOffset);
    u_xlat3 = u_xlat2 * float4(FGlobals._RefractionNoiseScale);
    u_xlat3 = u_xlat4.xwyw * u_xlat3;
    u_xlat16_25 = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat3.xy).x;
    u_xlat16_2 = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat3.zw).x;
    u_xlat14.xy = u_xlat2.zy * float2(FGlobals._RefractionNoiseScale);
    u_xlat14.xy = u_xlat32.xy * u_xlat14.xy;
    u_xlat16_14 = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat14.xy).x;
    u_xlat2.x = u_xlat7.y * float(u_xlat16_2);
    u_xlat25 = fma(float(u_xlat16_25), u_xlat7.x, u_xlat2.x);
    u_xlat25 = fma(float(u_xlat16_14), u_xlat7.z, u_xlat25);
    u_xlat2.xyz = (-FGlobals._RefractionColorDark.xyz) + FGlobals._RefractionColorLight.xyz;
    u_xlat2.xyz = fma(float3(u_xlat25), u_xlat2.xyz, FGlobals._RefractionColorDark.xyz);
    u_xlat25 = fma(FGlobals._RefractionShadowMask, u_xlat37, 1.0);
    u_xlat37 = u_xlat36 + FGlobals._RimOffset2;
    u_xlat37 = clamp(u_xlat37, 0.0f, 1.0f);
    u_xlat37 = (-u_xlat37) + 1.0;
    u_xlat37 = log2(u_xlat37);
    u_xlat37 = u_xlat37 * FGlobals._RimPower2;
    u_xlat37 = exp2(u_xlat37);
    u_xlat3.xyz = float3(u_xlat37) * FGlobals._RimColor2.xyz;
    u_xlat37 = u_xlat36 + FGlobals._RimOffset;
    u_xlat37 = clamp(u_xlat37, 0.0f, 1.0f);
    u_xlat37 = (-u_xlat37) + 1.0;
    u_xlat37 = log2(u_xlat37);
    u_xlat37 = u_xlat37 * FGlobals._RimPower;
    u_xlat37 = exp2(u_xlat37);
    u_xlat1.x = u_xlat1.x * u_xlat37;
    u_xlat4.xyz = u_xlat1.xxx * FGlobals._RimColor.xyz;
    u_xlat4.xyz = u_xlat4.xyz * float3(FGlobals._RimLightMul);
    u_xlat1.xyw = float3(u_xlat13) * u_xlat4.xyz;
    u_xlat1.xyw = fma(u_xlat3.xyz, float3(FGlobals._RimLightMul2), u_xlat1.xyw);
    u_xlat2.xyz = float3(u_xlat36) * u_xlat2.xyz;
    u_xlat1.xyz = fma(u_xlat2.xyz, float3(u_xlat25), u_xlat1.xyw);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat1.xyz;
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
