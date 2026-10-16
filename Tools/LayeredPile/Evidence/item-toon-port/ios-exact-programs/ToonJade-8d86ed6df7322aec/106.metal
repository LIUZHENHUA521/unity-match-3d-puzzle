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
    float4 hlslcc_mtx4x4unity_WorldToShadow[16];
    half4 _LightShadowData;
    float4 unity_ShadowFadeCenterAndType;
    float4 hlslcc_mtx4x4unity_MatrixV[4];
    float4 hlslcc_mtx4x4unity_MatrixInvV[4];
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

struct UnityDrawCallInfo_Type
{
    int unity_BaseInstanceID;
    int unity_InstanceCount;
};

struct unity_Builtins0Array_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorldArray[4];
    float4 hlslcc_mtx4x4unity_WorldToObjectArray[4];
};

struct UnityInstancing_PerDraw0_Type
{
    unity_Builtins0Array_Type unity_Builtins0Array[2];
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

constexpr sampler _mtl_xl_shadow_sampler(address::clamp_to_edge, filter::linear, compare_func::greater_equal);
fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    constant UnityDrawCallInfo_Type& UnityDrawCallInfo [[ buffer(1) ]],
    const constant unity_Builtins0Array_Type* UnityInstancing_PerDraw0 [[ buffer(2) ]],
    const constant unity_Builtins2Array_Type* UnityInstancing_PerDraw2 [[ buffer(3) ]],
    sampler sampler_ShadowMapTexture [[ sampler (0) ]],
    sampler sampler_NormalMap [[ sampler (1) ]],
    sampler sampler_BaseColor [[ sampler (2) ]],
    sampler sampler_AOTex [[ sampler (3) ]],
    sampler sampler_GradientNoise [[ sampler (4) ]],
    sampler sampler_ColorNoise1 [[ sampler (5) ]],
    sampler sampler_ColorNoise2 [[ sampler (6) ]],
    sampler sampler_Matcap [[ sampler (7) ]],
    sampler sampler_RimNoise [[ sampler (8) ]],
    sampler sampler_RefractionNoise [[ sampler (9) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _GradientNoise [[ texture(3) ]] ,
    texture2d<half, access::sample > _ColorNoise1 [[ texture(4) ]] ,
    texture2d<half, access::sample > _ColorNoise2 [[ texture(5) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(6) ]] ,
    texture2d<half, access::sample > _RimNoise [[ texture(7) ]] ,
    texture2d<half, access::sample > _RefractionNoise [[ texture(8) ]] ,
    depth2d<float, access::sample > _ShadowMapTexture [[ texture(9) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float3 u_xlat0;
    half3 u_xlat16_0;
    float4 u_xlat1;
    half3 u_xlat16_1;
    float4 u_xlat2;
    half3 u_xlat16_2;
    float3 u_xlat3;
    half3 u_xlat16_3;
    float3 u_xlat4;
    half u_xlat16_4;
    float4 u_xlat5;
    half u_xlat16_5;
    int u_xlati5;
    float3 u_xlat6;
    half u_xlat16_6;
    float3 u_xlat7;
    float4 u_xlat8;
    int3 u_xlati8;
    int3 u_xlati9;
    half3 u_xlat16_10;
    half u_xlat16_12;
    float3 u_xlat15;
    half u_xlat16_15;
    float3 u_xlat16;
    half u_xlat16_16;
    int u_xlati16;
    float u_xlat17;
    half u_xlat16_23;
    float u_xlat26;
    half u_xlat16_26;
    float2 u_xlat28;
    float u_xlat33;
    half u_xlat16_33;
    half u_xlat16_34;
    float u_xlat36;
    float u_xlat37;
    half u_xlat16_37;
    bool u_xlatb37;
    u_xlat0.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_0.xy = _NormalMap.sample(sampler_NormalMap, u_xlat0.xy).yw;
    u_xlat16_1.xy = fma(u_xlat16_0.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_23 = dot(u_xlat16_1.xy, u_xlat16_1.xy);
    u_xlat16_2.xy = half2(float2(u_xlat16_1.xy) * float2(FGlobals._NormalScale));
    u_xlat16_1.x = min(u_xlat16_23, half(1.0));
    u_xlat16_1.x = (-u_xlat16_1.x) + half(1.0);
    u_xlat16_2.z = sqrt(u_xlat16_1.x);
    u_xlat16_1.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_1.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_1.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_34 = dot(u_xlat16_1.xyz, u_xlat16_1.xyz);
    u_xlat16_34 = rsqrt(u_xlat16_34);
    u_xlat16_1.xyz = half3(u_xlat16_34) * u_xlat16_1.xyz;
    u_xlat0.xy = float2(u_xlat16_1.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat0.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_1.xx), u_xlat0.xy);
    u_xlat0.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_1.zz), u_xlat0.xy);
    u_xlat0.xy = u_xlat0.xy * float2(FGlobals._ReflectScale);
    u_xlat0.xy = fma(u_xlat0.xy, float2(0.5, 0.5), float2(0.5, 0.5));
    u_xlat16_3.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat0.xy).xyz;
    u_xlat0.xy = u_xlat0.xy + FGlobals._ReflectOffset.xyxx.xy;
    u_xlat16_0.xyz = _Matcap.sample(sampler_Matcap, u_xlat0.xy).xyz;
    u_xlat4.xyz = fma(float3(u_xlat16_3.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat4.xyz = fma(float3(u_xlat16_3.xyz), u_xlat4.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xyz = float3(u_xlat16_3.xyz) * u_xlat4.xyz;
    u_xlat4.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat4.xyz = fma(FGlobals._BaseTint.xyz, u_xlat4.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat4.xyz = u_xlat4.xyz * FGlobals._BaseTint.xyz;
    u_xlat3.xyz = u_xlat3.xyz * u_xlat4.xyz;
    u_xlat4.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_33 = _AOTex.sample(sampler_AOTex, u_xlat4.xy).x;
    u_xlat36 = fma(float(u_xlat16_33), 0.305306017, 0.682171106);
    u_xlat36 = fma(float(u_xlat16_33), u_xlat36, 0.0125228781);
    u_xlat33 = fma(float(u_xlat16_33), u_xlat36, -1.0);
    u_xlat33 = fma(FGlobals._AOScale, u_xlat33, 1.0);
    u_xlat3.xyz = float3(u_xlat33) * u_xlat3.xyz;
    u_xlat33 = dot(FGlobals._WorldSpaceLightPos0.xyz, FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat33 = rsqrt(u_xlat33);
    u_xlat4.xyz = float3(u_xlat33) * float3(FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat33 = dot(float3(u_xlat16_1.xyz), u_xlat4.xyz);
    u_xlat36 = u_xlat33 + FGlobals._BaseCellOffset;
    u_xlat33 = u_xlat33 + FGlobals._HighLightOffset;
    u_xlat33 = u_xlat33 / FGlobals._HighLightSmooth;
    u_xlat33 = clamp(u_xlat33, 0.0f, 1.0f);
    u_xlat36 = u_xlat36 / FGlobals._BaseCellSharpness;
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlat4.xyz = input.TEXCOORD2.www * FGlobals.hlslcc_mtx4x4unity_WorldToShadow[1].xyz;
    u_xlat4.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[0].xyz, input.TEXCOORD1.www, u_xlat4.xyz);
    u_xlat4.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[2].xyz, input.TEXCOORD3.www, u_xlat4.xyz);
    u_xlat4.xyz = u_xlat4.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToShadow[3].xyz;
    u_xlat4.x = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat4.xy, saturate(u_xlat4.z), level(0.0));
    u_xlat16_1.x = (-FGlobals._LightShadowData.x) + half(1.0);
    u_xlat16_1.x = half(fma(u_xlat4.x, float(u_xlat16_1.x), float(FGlobals._LightShadowData.x)));
    u_xlat4.x = input.TEXCOORD1.w;
    u_xlat4.y = input.TEXCOORD2.w;
    u_xlat4.z = input.TEXCOORD3.w;
    u_xlat5.xyz = u_xlat4.xyz + (-FGlobals.unity_ShadowFadeCenterAndType.xyz);
    u_xlat4.xyz = (-u_xlat4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat37 = dot(u_xlat5.xyz, u_xlat5.xyz);
    u_xlat37 = sqrt(u_xlat37);
    u_xlat5.x = FGlobals.hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat5.y = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat5.z = FGlobals.hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat5.x = dot(u_xlat4.xyz, u_xlat5.xyz);
    u_xlat37 = u_xlat37 + (-u_xlat5.x);
    u_xlat37 = fma(FGlobals.unity_ShadowFadeCenterAndType.w, u_xlat37, u_xlat5.x);
    u_xlat37 = fma(u_xlat37, float(FGlobals._LightShadowData.z), float(FGlobals._LightShadowData.w));
    u_xlat37 = clamp(u_xlat37, 0.0f, 1.0f);
    u_xlati5 = int(input.SV_InstanceID0) + UnityDrawCallInfo.unity_BaseInstanceID;
    u_xlati16 = u_xlati5 * 0x7;
    u_xlati5 = u_xlati5 << 0x3;
    u_xlat16_12 = (-u_xlat16_1.x) + UnityInstancing_PerDraw2[u_xlati16 / 7].unity_SHCArray.w;
    u_xlat16_1.x = half(fma(u_xlat37, float(u_xlat16_12), float(u_xlat16_1.x)));
    u_xlatb37 = FGlobals._LightColor0.w==half(0.0);
    u_xlat37 = (u_xlatb37) ? 0.0 : float(u_xlat16_1.x);
    u_xlat16.x = u_xlat36 * u_xlat37;
    u_xlat16.xyz = fma(u_xlat16.xxx, FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat6.x = fma((-u_xlat36), u_xlat37, 1.0);
    u_xlat36 = fma(u_xlat36, u_xlat37, -1.0);
    u_xlat6.xyz = u_xlat6.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat3.xyz = fma(u_xlat16.xyz, u_xlat3.xyz, u_xlat6.xyz);
    u_xlat37 = fma(u_xlat33, u_xlat33, (-u_xlat33));
    u_xlat33 = fma(FGlobals._HighLightPing, u_xlat37, u_xlat33);
    u_xlat16.xyz = float3(u_xlat33) * FGlobals._HighLightColor.xyz;
    u_xlat33 = fma(FGlobals._HighLightShadowMask, u_xlat36, 1.0);
    u_xlat3.xyz = fma(u_xlat16.xyz, float3(u_xlat33), u_xlat3.xyz);
    u_xlat3.xyz = u_xlat3.xyz + u_xlat3.xyz;
    u_xlat16.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat1 = input.TEXCOORD2.wwww * UnityInstancing_PerDraw0[u_xlati5 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[1].zyxz;
    u_xlat1 = fma(UnityInstancing_PerDraw0[u_xlati5 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[0].zyxz, input.TEXCOORD1.wwww, u_xlat1);
    u_xlat1 = fma(UnityInstancing_PerDraw0[u_xlati5 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[2].zyxz, input.TEXCOORD3.wwww, u_xlat1);
    u_xlat1 = u_xlat1 + UnityInstancing_PerDraw0[u_xlati5 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[3].zyxz;
    u_xlat33 = u_xlat1.y * FGlobals._FadeColorScale;
    u_xlat33 = fma(u_xlat33, 0.5, FGlobals._FadeColorOffset);
    u_xlat2 = u_xlat1.wyzw + float4(FGlobals._GradientNoiseOffset);
    u_xlat6.xy = u_xlat2.zy * float2(FGlobals._GradientNoiseScale);
    u_xlat2 = u_xlat2 * float4(FGlobals._GradientNoiseScale);
    u_xlat7.xyz = input.TEXCOORD2.zzz * UnityInstancing_PerDraw0[u_xlati5 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[1].xyz;
    u_xlat7.xyz = fma(UnityInstancing_PerDraw0[u_xlati5 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[0].xyz, input.TEXCOORD1.zzz, u_xlat7.xyz);
    u_xlat7.xyz = fma(UnityInstancing_PerDraw0[u_xlati5 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[2].xyz, input.TEXCOORD3.zzz, u_xlat7.xyz);
    u_xlat37 = dot(u_xlat7.xyz, u_xlat7.xyz);
    u_xlat37 = rsqrt(u_xlat37);
    u_xlat7.xyz = float3(u_xlat37) * u_xlat7.xyz;
    u_xlati8.xyz = int3(uint3((float3(0.0, 0.0, 0.0)<u_xlat7.xyz)) * 0xFFFFFFFFu);
    u_xlati9.xyz = int3(uint3((u_xlat7.xyz<float3(0.0, 0.0, 0.0))) * 0xFFFFFFFFu);
    u_xlati8.xyz = (-u_xlati8.xyz) + u_xlati9.xyz;
    u_xlat8.xyz = float3(u_xlati8.xyz);
    u_xlat8.w = 1.0;
    u_xlat28.xy = u_xlat8.zw * float2(-1.0, 1.0);
    u_xlat6.xy = u_xlat28.xy * u_xlat6.xy;
    u_xlat16_37 = _GradientNoise.sample(sampler_GradientNoise, u_xlat6.xy).x;
    u_xlat2 = u_xlat2 * u_xlat8.xwyw;
    u_xlat16_5 = _GradientNoise.sample(sampler_GradientNoise, u_xlat2.xy).x;
    u_xlat16_6 = _GradientNoise.sample(sampler_GradientNoise, u_xlat2.zw).x;
    u_xlat17 = abs(u_xlat7.y) + abs(u_xlat7.x);
    u_xlat17 = abs(u_xlat7.z) + u_xlat17;
    u_xlat17 = u_xlat17 + 9.99999975e-06;
    u_xlat7.xyz = abs(u_xlat7.xyz) / float3(u_xlat17);
    u_xlat6.x = float(u_xlat16_6) * u_xlat7.y;
    u_xlat5.x = fma(float(u_xlat16_5), u_xlat7.x, u_xlat6.x);
    u_xlat37 = fma(float(u_xlat16_37), u_xlat7.z, u_xlat5.x);
    u_xlat33 = u_xlat33 + u_xlat37;
    u_xlat33 = u_xlat33 + 0.5;
    u_xlat33 = clamp(u_xlat33, 0.0f, 1.0f);
    u_xlat5.xyz = fma(float3(u_xlat33), u_xlat16.xyz, FGlobals._FadeColorF.xyz);
    u_xlat3.xyz = u_xlat3.xyz * u_xlat5.xyz;
    u_xlat16_10.xyz = half3(max(u_xlat3.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat3.xyz = log2(float3(u_xlat16_10.xyz));
    u_xlat3.xyz = u_xlat3.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat3.xyz = exp2(u_xlat3.xyz);
    u_xlat3.xyz = fma(u_xlat3.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat3.xyz = max(u_xlat3.xyz, float3(0.0, 0.0, 0.0));
    u_xlat33 = FGlobals._Noise1Draker + 0.00999999978;
    u_xlat37 = (-FGlobals._Noise1Lighter) + 1.0;
    u_xlat33 = max(u_xlat33, u_xlat37);
    u_xlat33 = u_xlat33 + (-FGlobals._Noise1Draker);
    u_xlat2 = u_xlat1.wyzw + FGlobals._Noise1Offset.xxyz.wzyw;
    u_xlat5 = u_xlat2 * float4(FGlobals._Noise1Scale);
    u_xlat6.xy = u_xlat2.zy * float2(FGlobals._Noise1Scale);
    u_xlat6.xy = u_xlat28.xy * u_xlat6.xy;
    u_xlat16_37 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat6.xy).x;
    u_xlat2 = u_xlat8.xwyw * u_xlat5;
    u_xlat16_5 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat2.xy).x;
    u_xlat16_16 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat2.zw).x;
    u_xlat16.x = u_xlat7.y * float(u_xlat16_16);
    u_xlat5.x = fma(float(u_xlat16_5), u_xlat7.x, u_xlat16.x);
    u_xlat37 = fma(float(u_xlat16_37), u_xlat7.z, u_xlat5.x);
    u_xlat5.x = fma(u_xlat37, 0.305306017, 0.682171106);
    u_xlat5.x = fma(u_xlat37, u_xlat5.x, 0.0125228781);
    u_xlat37 = fma(u_xlat37, u_xlat5.x, (-FGlobals._Noise1Draker));
    u_xlat33 = u_xlat37 / u_xlat33;
    u_xlat33 = clamp(u_xlat33, 0.0f, 1.0f);
    u_xlat5.xyz = (-FGlobals._ColorNoise1Dark.xyz) + FGlobals._ColorNoise1Light.xyz;
    u_xlat5.xyz = fma(float3(u_xlat33), u_xlat5.xyz, FGlobals._ColorNoise1Dark.xyz);
    u_xlat3.xyz = u_xlat3.xyz * u_xlat5.xyz;
    u_xlat5.xyz = fma(float3(u_xlat16_0.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat5.xyz = fma(float3(u_xlat16_0.xyz), u_xlat5.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat0.xyz = float3(u_xlat16_0.xyz) * u_xlat5.xyz;
    u_xlat0.xyz = u_xlat0.xyz * float3(FGlobals._MatcapScale);
    u_xlat33 = fma(FGlobals._MatcapShadowMask, u_xlat36, 1.0);
    u_xlat0.xyz = u_xlat0.xyz * float3(u_xlat33);
    u_xlat0.xyz = u_xlat0.xyz * FGlobals._MatcapColor.xyz;
    u_xlat33 = FGlobals._Noise2Draker + 0.00999999978;
    u_xlat37 = (-FGlobals._Noise2Lighter) + 1.0;
    u_xlat33 = max(u_xlat33, u_xlat37);
    u_xlat33 = u_xlat33 + (-FGlobals._Noise2Draker);
    u_xlat2 = u_xlat1.wyzw + FGlobals._Noise2Offset.xxyz.wzyw;
    u_xlat5 = u_xlat2 * float4(FGlobals._Noise2Scale);
    u_xlat6.xy = u_xlat2.zy * float2(FGlobals._Noise2Scale);
    u_xlat6.xy = u_xlat28.xy * u_xlat6.xy;
    u_xlat16_37 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat6.xy).y;
    u_xlat2 = u_xlat8.xwyw * u_xlat5;
    u_xlat16_5 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat2.xy).y;
    u_xlat16_16 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat2.zw).y;
    u_xlat16.x = u_xlat7.y * float(u_xlat16_16);
    u_xlat5.x = fma(float(u_xlat16_5), u_xlat7.x, u_xlat16.x);
    u_xlat37 = fma(float(u_xlat16_37), u_xlat7.z, u_xlat5.x);
    u_xlat5.x = fma(u_xlat37, 0.305306017, 0.682171106);
    u_xlat5.x = fma(u_xlat37, u_xlat5.x, 0.0125228781);
    u_xlat37 = fma(u_xlat37, u_xlat5.x, (-FGlobals._Noise2Draker));
    u_xlat33 = u_xlat37 / u_xlat33;
    u_xlat33 = clamp(u_xlat33, 0.0f, 1.0f);
    u_xlat5.xyz = (-FGlobals._ColorNoise2Dark.xyz) + FGlobals._ColorNoise2Light.xyz;
    u_xlat5.xyz = fma(float3(u_xlat33), u_xlat5.xyz, FGlobals._ColorNoise2Dark.xyz);
    u_xlat0.xyz = fma(u_xlat3.xyz, u_xlat5.xyz, u_xlat0.xyz);
    u_xlat16_10.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_10.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat33 = dot(u_xlat4.xyz, u_xlat4.xyz);
    u_xlat33 = rsqrt(u_xlat33);
    u_xlat3.xyz = float3(u_xlat33) * u_xlat4.xyz;
    u_xlat4.xyz = fma((-u_xlat4.xyz), float3(u_xlat33), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat3.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat4.xyz, u_xlat3.xyz);
    u_xlat4.x = input.TEXCOORD1.z;
    u_xlat4.y = input.TEXCOORD2.z;
    u_xlat4.z = input.TEXCOORD3.z;
    u_xlat33 = dot(u_xlat4.xyz, u_xlat3.xyz);
    u_xlat3.x = u_xlat33 + FGlobals._RimOffset2;
    u_xlat3.x = clamp(u_xlat3.x, 0.0f, 1.0f);
    u_xlat3.x = (-u_xlat3.x) + 1.0;
    u_xlat3.x = log2(u_xlat3.x);
    u_xlat3.x = u_xlat3.x * FGlobals._RimPower2;
    u_xlat3.x = exp2(u_xlat3.x);
    u_xlat3.xyz = u_xlat3.xxx * FGlobals._RimColor2.xyz;
    u_xlat4.x = fma(FGlobals._RimShadowScale, u_xlat36, 1.0);
    u_xlat36 = fma(FGlobals._RefractionShadowMask, u_xlat36, 1.0);
    u_xlat15.x = u_xlat33 + FGlobals._RimOffset;
    u_xlat15.x = clamp(u_xlat15.x, 0.0f, 1.0f);
    u_xlat15.x = (-u_xlat15.x) + 1.0;
    u_xlat15.x = log2(u_xlat15.x);
    u_xlat15.x = u_xlat15.x * FGlobals._RimPower;
    u_xlat15.x = exp2(u_xlat15.x);
    u_xlat4.x = u_xlat4.x * u_xlat15.x;
    u_xlat4.xyz = u_xlat4.xxx * FGlobals._RimColor.xyz;
    u_xlat4.xyz = u_xlat4.xyz * float3(FGlobals._RimLightMul);
    u_xlat2 = u_xlat1.wyzw + float4(FGlobals._RimNoiseOffset);
    u_xlat1 = u_xlat1 + float4(FGlobals._RefractionNoiseOffset);
    u_xlat5 = u_xlat2 * float4(FGlobals._RimNoiseScale);
    u_xlat6.xy = u_xlat2.zy * float2(FGlobals._RimNoiseScale);
    u_xlat6.xy = u_xlat28.xy * u_xlat6.xy;
    u_xlat16_37 = _RimNoise.sample(sampler_RimNoise, u_xlat6.xy).x;
    u_xlat2 = u_xlat8.xwyw * u_xlat5;
    u_xlat16_5 = _RimNoise.sample(sampler_RimNoise, u_xlat2.xy).x;
    u_xlat16_16 = _RimNoise.sample(sampler_RimNoise, u_xlat2.zw).x;
    u_xlat16.x = u_xlat7.y * float(u_xlat16_16);
    u_xlat5.x = fma(float(u_xlat16_5), u_xlat7.x, u_xlat16.x);
    u_xlat37 = fma(float(u_xlat16_37), u_xlat7.z, u_xlat5.x);
    u_xlat4.xyz = float3(u_xlat37) * u_xlat4.xyz;
    u_xlat3.xyz = fma(u_xlat3.xyz, float3(FGlobals._RimLightMul2), u_xlat4.xyz);
    u_xlat2 = u_xlat1 * float4(FGlobals._RefractionNoiseScale);
    u_xlat4.xy = u_xlat1.zy * float2(FGlobals._RefractionNoiseScale);
    u_xlat4.xy = u_xlat28.xy * u_xlat4.xy;
    u_xlat16_4 = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat4.xy).x;
    u_xlat1 = u_xlat8.xwyw * u_xlat2;
    u_xlat16_15 = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat1.xy).x;
    u_xlat16_26 = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat1.zw).x;
    u_xlat26 = u_xlat7.y * float(u_xlat16_26);
    u_xlat15.x = fma(float(u_xlat16_15), u_xlat7.x, u_xlat26);
    u_xlat4.x = fma(float(u_xlat16_4), u_xlat7.z, u_xlat15.x);
    u_xlat15.xyz = (-FGlobals._RefractionColorDark.xyz) + FGlobals._RefractionColorLight.xyz;
    u_xlat4.xyz = fma(u_xlat4.xxx, u_xlat15.xyz, FGlobals._RefractionColorDark.xyz);
    u_xlat4.xyz = float3(u_xlat33) * u_xlat4.xyz;
    u_xlat3.xyz = fma(u_xlat4.xyz, float3(u_xlat36), u_xlat3.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat3.xyz;
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
