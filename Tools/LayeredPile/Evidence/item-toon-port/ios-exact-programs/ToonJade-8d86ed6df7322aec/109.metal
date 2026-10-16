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
    const constant unity_Builtins0Array_Type* UnityInstancing_PerDraw0 [[ buffer(2) ]],
    sampler sampler_NormalMap [[ sampler (0) ]],
    sampler sampler_BaseColor [[ sampler (1) ]],
    sampler sampler_AOTex [[ sampler (2) ]],
    sampler sampler_GradientNoise [[ sampler (3) ]],
    sampler sampler_ColorNoise1 [[ sampler (4) ]],
    sampler sampler_ColorNoise2 [[ sampler (5) ]],
    sampler sampler_Matcap [[ sampler (6) ]],
    sampler sampler_RimNoise [[ sampler (7) ]],
    sampler sampler_RefractionNoise [[ sampler (8) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _GradientNoise [[ texture(3) ]] ,
    texture2d<half, access::sample > _ColorNoise1 [[ texture(4) ]] ,
    texture2d<half, access::sample > _ColorNoise2 [[ texture(5) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(6) ]] ,
    texture2d<half, access::sample > _RimNoise [[ texture(7) ]] ,
    texture2d<half, access::sample > _RefractionNoise [[ texture(8) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float4 u_xlat0;
    half u_xlat16_0;
    float4 u_xlat1;
    half3 u_xlat16_1;
    float4 u_xlat2;
    half3 u_xlat16_2;
    float4 u_xlat3;
    half3 u_xlat16_3;
    float4 u_xlat4;
    float4 u_xlat5;
    half3 u_xlat16_5;
    float4 u_xlat6;
    half u_xlat16_6;
    float3 u_xlat7;
    float3 u_xlat8;
    float3 u_xlat9;
    int3 u_xlati9;
    float3 u_xlat10;
    int3 u_xlati10;
    half3 u_xlat16_11;
    float2 u_xlat12;
    half2 u_xlat16_12;
    int u_xlati12;
    bool u_xlatb12;
    float u_xlat15;
    float u_xlat18;
    half u_xlat16_18;
    float2 u_xlat24;
    half u_xlat16_25;
    float u_xlat27;
    half u_xlat16_27;
    bool u_xlatb27;
    float2 u_xlat31;
    float u_xlat36;
    half u_xlat16_36;
    bool u_xlatb36;
    half u_xlat16_37;
    float u_xlat39;
    half u_xlat16_39;
    float u_xlat41;
    half u_xlat16_41;
    bool u_xlatb41;
    float u_xlat42;
    half u_xlat16_42;
    u_xlat0.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_0 = _AOTex.sample(sampler_AOTex, u_xlat0.xy).x;
    u_xlat12.x = fma(float(u_xlat16_0), 0.305306017, 0.682171106);
    u_xlat12.x = fma(float(u_xlat16_0), u_xlat12.x, 0.0125228781);
    u_xlat0.x = fma(float(u_xlat16_0), u_xlat12.x, -1.0);
    u_xlat0.x = fma(FGlobals._AOScale, u_xlat0.x, 1.0);
    u_xlat12.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_12.xy = _NormalMap.sample(sampler_NormalMap, u_xlat12.xy).yw;
    u_xlat16_1.xy = fma(u_xlat16_12.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_25 = dot(u_xlat16_1.xy, u_xlat16_1.xy);
    u_xlat16_2.xy = half2(float2(u_xlat16_1.xy) * float2(FGlobals._NormalScale));
    u_xlat16_1.x = min(u_xlat16_25, half(1.0));
    u_xlat16_1.x = (-u_xlat16_1.x) + half(1.0);
    u_xlat16_2.z = sqrt(u_xlat16_1.x);
    u_xlat16_1.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_1.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_1.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_2.xyz));
    u_xlat16_37 = dot(u_xlat16_1.xyz, u_xlat16_1.xyz);
    u_xlat16_37 = rsqrt(u_xlat16_37);
    u_xlat16_1.xyz = half3(u_xlat16_37) * u_xlat16_1.xyz;
    u_xlat2 = float4(u_xlat16_1.yyyy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].zxyz;
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].zxyz, float4(u_xlat16_1.xxxx), u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].zxyz, float4(u_xlat16_1.zzzz), u_xlat2);
    u_xlati12 = int(input.SV_InstanceID0) + UnityDrawCallInfo.unity_BaseInstanceID;
    u_xlati12 = u_xlati12 << 0x3;
    u_xlat3 = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx * UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1].yyyy;
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1].xxxx, u_xlat3);
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1].zzzz, u_xlat3);
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1].wwww, u_xlat3);
    u_xlat4 = input.TEXCOORD2.wwww * UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[1].zyxz;
    u_xlat4 = fma(UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[0].zyxz, input.TEXCOORD1.wwww, u_xlat4);
    u_xlat4 = fma(UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[2].zyxz, input.TEXCOORD3.wwww, u_xlat4);
    u_xlat4 = u_xlat4 + UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[3].zyxz;
    u_xlat3 = u_xlat3 * u_xlat4.yyyy;
    u_xlat5 = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx * UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0].yyyy;
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0].xxxx, u_xlat5);
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0].zzzz, u_xlat5);
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0].wwww, u_xlat5);
    u_xlat3 = fma(u_xlat5, u_xlat4.zzzz, u_xlat3);
    u_xlat5 = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx * UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2].yyyy;
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2].xxxx, u_xlat5);
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2].zzzz, u_xlat5);
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2].wwww, u_xlat5);
    u_xlat3 = fma(u_xlat5, u_xlat4.xwww, u_xlat3);
    u_xlat5 = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx * UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3].yyyy;
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3].xxxx, u_xlat5);
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3].zzzz, u_xlat5);
    u_xlat5 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3].wwww, u_xlat5);
    u_xlat3 = u_xlat3 + u_xlat5;
    u_xlat24.x = dot(u_xlat3.xzw, u_xlat3.xzw);
    u_xlat24.x = max(u_xlat24.x, 0.00100000005);
    u_xlat24.x = rsqrt(u_xlat24.x);
    u_xlat3 = u_xlat24.xxxx * u_xlat3;
    u_xlat24.xy = u_xlat2.zw * u_xlat3.zw;
    u_xlat3.xy = fma(u_xlat3.xy, u_xlat2.xy, (-u_xlat24.xy));
    u_xlat3.z = (-u_xlat3.y);
    u_xlat24.x = FGlobals._ReflectScale * 0.5;
    u_xlat24.xy = fma(u_xlat3.zx, u_xlat24.xx, float2(0.5, 0.5));
    u_xlat16_3.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat24.xy).xyz;
    u_xlat24.xy = u_xlat24.xy + FGlobals._ReflectOffset.xyxx.xy;
    u_xlat16_5.xyz = _Matcap.sample(sampler_Matcap, u_xlat24.xy).xyz;
    u_xlat6.xyz = fma(float3(u_xlat16_3.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat6.xyz = fma(float3(u_xlat16_3.xyz), u_xlat6.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xyz = float3(u_xlat16_3.xyz) * u_xlat6.xyz;
    u_xlat6.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat6.xyz = fma(FGlobals._BaseTint.xyz, u_xlat6.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat6.xyz = u_xlat6.xyz * FGlobals._BaseTint.xyz;
    u_xlat3.xyz = u_xlat3.xyz * u_xlat6.xyz;
    u_xlat0.xzw = u_xlat0.xxx * u_xlat3.xyz;
    u_xlat3.x = dot(FGlobals._WorldSpaceLightPos0.xyz, FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat3.x = rsqrt(u_xlat3.x);
    u_xlat3.xyz = u_xlat3.xxx * float3(FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat3.x = dot(float3(u_xlat16_1.xyz), u_xlat3.xyz);
    u_xlat15 = u_xlat3.x + FGlobals._BaseCellOffset;
    u_xlat3.x = u_xlat3.x + FGlobals._HighLightOffset;
    u_xlat3.x = u_xlat3.x / FGlobals._HighLightSmooth;
    u_xlat3.x = clamp(u_xlat3.x, 0.0f, 1.0f);
    u_xlat15 = u_xlat15 / FGlobals._BaseCellSharpness;
    u_xlat15 = clamp(u_xlat15, 0.0f, 1.0f);
    u_xlatb27 = FGlobals._LightColor0.w==half(0.0);
    u_xlat27 = (u_xlatb27) ? 0.0 : 1.0;
    u_xlat39 = u_xlat27 * u_xlat15;
    u_xlat6.xyz = fma(float3(u_xlat39), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat39 = fma((-u_xlat15), u_xlat27, 1.0);
    u_xlat15 = fma(u_xlat15, u_xlat27, -1.0);
    u_xlat7.xyz = float3(u_xlat39) * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xzw = fma(u_xlat6.xyz, u_xlat0.xzw, u_xlat7.xyz);
    u_xlat27 = fma(u_xlat3.x, u_xlat3.x, (-u_xlat3.x));
    u_xlat3.x = fma(FGlobals._HighLightPing, u_xlat27, u_xlat3.x);
    u_xlat3.xzw = u_xlat3.xxx * FGlobals._HighLightColor.xyz;
    u_xlat41 = fma(FGlobals._HighLightShadowMask, u_xlat15, 1.0);
    u_xlat0.xzw = fma(u_xlat3.xzw, float3(u_xlat41), u_xlat0.xzw);
    u_xlat3.xzw = fma((-u_xlat0.xzw), float3(2.0, 2.0, 2.0), float3(1.0, 1.0, 1.0));
    u_xlat0.xzw = u_xlat0.xzw + u_xlat0.xzw;
    u_xlat6.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat1 = u_xlat4.wyzw + float4(FGlobals._GradientNoiseOffset);
    u_xlat7.xy = u_xlat1.zy * float2(FGlobals._GradientNoiseScale);
    u_xlat1 = u_xlat1 * float4(FGlobals._GradientNoiseScale);
    u_xlat8.xyz = input.TEXCOORD2.zzz * UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[1].xyz;
    u_xlat8.xyz = fma(UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[0].xyz, input.TEXCOORD1.zzz, u_xlat8.xyz);
    u_xlat8.xyz = fma(UnityInstancing_PerDraw0[u_xlati12 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[2].xyz, input.TEXCOORD3.zzz, u_xlat8.xyz);
    u_xlat12.x = dot(u_xlat8.xyz, u_xlat8.xyz);
    u_xlat12.x = rsqrt(u_xlat12.x);
    u_xlat8.xyz = u_xlat12.xxx * u_xlat8.xyz;
    u_xlati9.xyz = int3(uint3((float3(0.0, 0.0, 0.0)<u_xlat8.xyz)) * 0xFFFFFFFFu);
    u_xlati10.xyz = int3(uint3((u_xlat8.xyz<float3(0.0, 0.0, 0.0))) * 0xFFFFFFFFu);
    u_xlati9.xyz = (-u_xlati9.xyz) + u_xlati10.xyz;
    u_xlat2.xyz = float3(u_xlati9.xyz);
    u_xlat2.w = 1.0;
    u_xlat31.xy = u_xlat2.zw * float2(-1.0, 1.0);
    u_xlat7.xy = u_xlat31.xy * u_xlat7.xy;
    u_xlat16_12.x = _GradientNoise.sample(sampler_GradientNoise, u_xlat7.xy).x;
    u_xlat1 = u_xlat1 * u_xlat2.xwyw;
    u_xlat16_41 = _GradientNoise.sample(sampler_GradientNoise, u_xlat1.xy).x;
    u_xlat16_42 = _GradientNoise.sample(sampler_GradientNoise, u_xlat1.zw).x;
    u_xlat7.x = abs(u_xlat8.y) + abs(u_xlat8.x);
    u_xlat7.x = abs(u_xlat8.z) + u_xlat7.x;
    u_xlat7.x = u_xlat7.x + 9.99999975e-06;
    u_xlat8.xyz = abs(u_xlat8.xyz) / u_xlat7.xxx;
    u_xlat42 = float(u_xlat16_42) * u_xlat8.y;
    u_xlat41 = fma(float(u_xlat16_41), u_xlat8.x, u_xlat42);
    u_xlat12.x = fma(float(u_xlat16_12.x), u_xlat8.z, u_xlat41);
    u_xlat41 = u_xlat4.w * FGlobals._FadeColorScale;
    u_xlat41 = fma(u_xlat41, 0.5, FGlobals._FadeColorOffset);
    u_xlat12.x = u_xlat12.x + u_xlat41;
    u_xlat12.x = u_xlat12.x + 0.5;
    u_xlat12.x = clamp(u_xlat12.x, 0.0f, 1.0f);
    u_xlat6.xyz = fma(u_xlat12.xxx, u_xlat6.xyz, FGlobals._FadeColorF.xyz);
    u_xlat9.xyz = fma(u_xlat6.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat9.xyz = fma(u_xlat6.xyz, u_xlat9.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat10.xyz = fma((-u_xlat6.xyz), u_xlat9.xyz, float3(1.0, 1.0, 1.0));
    u_xlat6.xyz = u_xlat6.xyz * u_xlat9.xyz;
    u_xlat3.xzw = u_xlat3.xzw * u_xlat10.xyz;
    u_xlat3.xzw = fma((-u_xlat3.xzw), float3(2.0, 2.0, 2.0), float3(1.0, 1.0, 1.0));
    u_xlat12.x = max(u_xlat6.y, u_xlat6.x);
    u_xlat12.x = max(u_xlat6.z, u_xlat12.x);
    u_xlat0.xzw = u_xlat0.xzw * u_xlat6.xyz;
    u_xlat0.xzw = u_xlat0.xzw + u_xlat0.xzw;
    u_xlatb41 = u_xlat12.x>=0.5;
    u_xlatb12 = 0.5>=u_xlat12.x;
    u_xlat12.x = u_xlatb12 ? 1.0 : float(0.0);
    u_xlat41 = u_xlatb41 ? 1.0 : float(0.0);
    u_xlat3.xzw = u_xlat3.xzw * float3(u_xlat41);
    u_xlat0.xyz = fma(u_xlat0.xzw, u_xlat12.xxx, u_xlat3.xzw);
    u_xlat16_11.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_11.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat3.xzw = (-u_xlat0.xyz) + float3(1.0, 1.0, 1.0);
    u_xlat36 = FGlobals._Noise1Draker + 0.00999999978;
    u_xlat41 = (-FGlobals._Noise1Lighter) + 1.0;
    u_xlat36 = max(u_xlat36, u_xlat41);
    u_xlat36 = u_xlat36 + (-FGlobals._Noise1Draker);
    u_xlat1 = u_xlat4.wyzw + FGlobals._Noise1Offset.xxyz.wzyw;
    u_xlat6 = u_xlat1 * float4(FGlobals._Noise1Scale);
    u_xlat7.xy = u_xlat1.zy * float2(FGlobals._Noise1Scale);
    u_xlat7.xy = u_xlat31.xy * u_xlat7.xy;
    u_xlat16_41 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat7.xy).x;
    u_xlat1 = u_xlat2.xwyw * u_xlat6;
    u_xlat16_6 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat1.xy).x;
    u_xlat16_18 = _ColorNoise1.sample(sampler_ColorNoise1, u_xlat1.zw).x;
    u_xlat18 = u_xlat8.y * float(u_xlat16_18);
    u_xlat6.x = fma(float(u_xlat16_6), u_xlat8.x, u_xlat18);
    u_xlat41 = fma(float(u_xlat16_41), u_xlat8.z, u_xlat6.x);
    u_xlat6.x = fma(u_xlat41, 0.305306017, 0.682171106);
    u_xlat6.x = fma(u_xlat41, u_xlat6.x, 0.0125228781);
    u_xlat41 = fma(u_xlat41, u_xlat6.x, (-FGlobals._Noise1Draker));
    u_xlat36 = u_xlat41 / u_xlat36;
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlat6.xyz = (-FGlobals._ColorNoise1Dark.xyz) + FGlobals._ColorNoise1Light.xyz;
    u_xlat6.xyz = fma(float3(u_xlat36), u_xlat6.xyz, FGlobals._ColorNoise1Dark.xyz);
    u_xlat9.xyz = fma(u_xlat6.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat9.xyz = fma(u_xlat6.xyz, u_xlat9.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat10.xyz = fma((-u_xlat6.xyz), u_xlat9.xyz, float3(1.0, 1.0, 1.0));
    u_xlat6.xyz = u_xlat6.xyz * u_xlat9.xyz;
    u_xlat3.xzw = u_xlat3.xzw * u_xlat10.xyz;
    u_xlat3.xzw = fma((-u_xlat3.xzw), float3(2.0, 2.0, 2.0), float3(1.0, 1.0, 1.0));
    u_xlat36 = max(u_xlat6.y, u_xlat6.x);
    u_xlat36 = max(u_xlat6.z, u_xlat36);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat6.xyz;
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlatb41 = u_xlat36>=0.5;
    u_xlatb36 = 0.5>=u_xlat36;
    u_xlat36 = u_xlatb36 ? 1.0 : float(0.0);
    u_xlat41 = u_xlatb41 ? 1.0 : float(0.0);
    u_xlat3.xzw = u_xlat3.xzw * float3(u_xlat41);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(u_xlat36), u_xlat3.xzw);
    u_xlat3.xzw = (-u_xlat0.xyz) + float3(1.0, 1.0, 1.0);
    u_xlat36 = FGlobals._Noise2Draker + 0.00999999978;
    u_xlat41 = (-FGlobals._Noise2Lighter) + 1.0;
    u_xlat36 = max(u_xlat36, u_xlat41);
    u_xlat36 = u_xlat36 + (-FGlobals._Noise2Draker);
    u_xlat1 = u_xlat4.wyzw + FGlobals._Noise2Offset.xxyz.wzyw;
    u_xlat6 = u_xlat1 * float4(FGlobals._Noise2Scale);
    u_xlat7.xy = u_xlat1.zy * float2(FGlobals._Noise2Scale);
    u_xlat7.xy = u_xlat31.xy * u_xlat7.xy;
    u_xlat16_41 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat7.xy).y;
    u_xlat1 = u_xlat2.xwyw * u_xlat6;
    u_xlat16_6 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat1.xy).y;
    u_xlat16_18 = _ColorNoise2.sample(sampler_ColorNoise2, u_xlat1.zw).y;
    u_xlat18 = u_xlat8.y * float(u_xlat16_18);
    u_xlat6.x = fma(float(u_xlat16_6), u_xlat8.x, u_xlat18);
    u_xlat41 = fma(float(u_xlat16_41), u_xlat8.z, u_xlat6.x);
    u_xlat6.x = fma(u_xlat41, 0.305306017, 0.682171106);
    u_xlat6.x = fma(u_xlat41, u_xlat6.x, 0.0125228781);
    u_xlat41 = fma(u_xlat41, u_xlat6.x, (-FGlobals._Noise2Draker));
    u_xlat36 = u_xlat41 / u_xlat36;
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlat6.xyz = (-FGlobals._ColorNoise2Dark.xyz) + FGlobals._ColorNoise2Light.xyz;
    u_xlat6.xyz = fma(float3(u_xlat36), u_xlat6.xyz, FGlobals._ColorNoise2Dark.xyz);
    u_xlat9.xyz = fma(u_xlat6.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat9.xyz = fma(u_xlat6.xyz, u_xlat9.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat10.xyz = fma((-u_xlat6.xyz), u_xlat9.xyz, float3(1.0, 1.0, 1.0));
    u_xlat6.xyz = u_xlat6.xyz * u_xlat9.xyz;
    u_xlat3.xzw = u_xlat3.xzw * u_xlat10.xyz;
    u_xlat3.xzw = fma((-u_xlat3.xzw), float3(2.0, 2.0, 2.0), float3(1.0, 1.0, 1.0));
    u_xlat36 = max(u_xlat6.y, u_xlat6.x);
    u_xlat36 = max(u_xlat6.z, u_xlat36);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat6.xyz;
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlatb41 = u_xlat36>=0.5;
    u_xlatb36 = 0.5>=u_xlat36;
    u_xlat36 = u_xlatb36 ? 1.0 : float(0.0);
    u_xlat41 = u_xlatb41 ? 1.0 : float(0.0);
    u_xlat3.xzw = u_xlat3.xzw * float3(u_xlat41);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(u_xlat36), u_xlat3.xzw);
    u_xlat3.xzw = fma(float3(u_xlat16_5.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat3.xzw = fma(float3(u_xlat16_5.xyz), u_xlat3.xzw, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xzw = u_xlat3.xzw * float3(u_xlat16_5.xyz);
    u_xlat3.xzw = u_xlat3.xzw * float3(FGlobals._MatcapScale);
    u_xlat36 = fma(FGlobals._MatcapShadowMask, u_xlat15, 1.0);
    u_xlat3.xzw = u_xlat3.xzw * float3(u_xlat36);
    u_xlat0.xyz = fma(u_xlat3.xzw, FGlobals._MatcapColor.xyz, u_xlat0.xyz);
    u_xlat16_11.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_11.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat1 = u_xlat4.wyzw + float4(FGlobals._RimNoiseOffset);
    u_xlat4 = u_xlat4 + float4(FGlobals._RefractionNoiseOffset);
    u_xlat5 = u_xlat1 * float4(FGlobals._RimNoiseScale);
    u_xlat3.xz = u_xlat1.zy * float2(FGlobals._RimNoiseScale);
    u_xlat3.xz = u_xlat31.xy * u_xlat3.xz;
    u_xlat16_36 = _RimNoise.sample(sampler_RimNoise, u_xlat3.xz).x;
    u_xlat1 = u_xlat2.xwyw * u_xlat5;
    u_xlat16_3.x = _RimNoise.sample(sampler_RimNoise, u_xlat1.xy).x;
    u_xlat16_27 = _RimNoise.sample(sampler_RimNoise, u_xlat1.zw).x;
    u_xlat27 = u_xlat8.y * float(u_xlat16_27);
    u_xlat3.x = fma(float(u_xlat16_3.x), u_xlat8.x, u_xlat27);
    u_xlat36 = fma(float(u_xlat16_36), u_xlat8.z, u_xlat3.x);
    u_xlat3.x = fma(FGlobals._RimShadowScale, u_xlat15, 1.0);
    u_xlat15 = fma(FGlobals._RefractionShadowMask, u_xlat15, 1.0);
    u_xlat5.x = input.TEXCOORD1.w;
    u_xlat5.y = input.TEXCOORD2.w;
    u_xlat5.z = input.TEXCOORD3.w;
    u_xlat5.xyz = (-u_xlat5.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat27 = dot(u_xlat5.xyz, u_xlat5.xyz);
    u_xlat27 = rsqrt(u_xlat27);
    u_xlat6.xyz = float3(u_xlat27) * u_xlat5.xyz;
    u_xlat5.xyz = fma((-u_xlat5.xyz), float3(u_xlat27), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat5.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat5.xyz, u_xlat6.xyz);
    u_xlat6.x = input.TEXCOORD1.z;
    u_xlat6.y = input.TEXCOORD2.z;
    u_xlat6.z = input.TEXCOORD3.z;
    u_xlat27 = dot(u_xlat6.xyz, u_xlat5.xyz);
    u_xlat39 = u_xlat27 + FGlobals._RimOffset;
    u_xlat39 = clamp(u_xlat39, 0.0f, 1.0f);
    u_xlat39 = (-u_xlat39) + 1.0;
    u_xlat39 = log2(u_xlat39);
    u_xlat39 = u_xlat39 * FGlobals._RimPower;
    u_xlat39 = exp2(u_xlat39);
    u_xlat3.x = u_xlat3.x * u_xlat39;
    u_xlat5.xyz = u_xlat3.xxx * FGlobals._RimColor.xyz;
    u_xlat5.xyz = u_xlat5.xyz * float3(FGlobals._RimLightMul);
    u_xlat5.xyz = float3(u_xlat36) * u_xlat5.xyz;
    u_xlat36 = u_xlat27 + FGlobals._RimOffset2;
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlat36 = (-u_xlat36) + 1.0;
    u_xlat36 = log2(u_xlat36);
    u_xlat36 = u_xlat36 * FGlobals._RimPower2;
    u_xlat36 = exp2(u_xlat36);
    u_xlat6.xyz = float3(u_xlat36) * FGlobals._RimColor2.xyz;
    u_xlat5.xyz = fma(u_xlat6.xyz, float3(FGlobals._RimLightMul2), u_xlat5.xyz);
    u_xlat1 = u_xlat4 * float4(FGlobals._RefractionNoiseScale);
    u_xlat3.xw = u_xlat4.zy * float2(FGlobals._RefractionNoiseScale);
    u_xlat3.xw = u_xlat31.xy * u_xlat3.xw;
    u_xlat16_36 = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat3.xw).x;
    u_xlat1 = u_xlat2.xwyw * u_xlat1;
    u_xlat16_3.x = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat1.xy).x;
    u_xlat16_39 = _RefractionNoise.sample(sampler_RefractionNoise, u_xlat1.zw).x;
    u_xlat39 = u_xlat8.y * float(u_xlat16_39);
    u_xlat3.x = fma(float(u_xlat16_3.x), u_xlat8.x, u_xlat39);
    u_xlat36 = fma(float(u_xlat16_36), u_xlat8.z, u_xlat3.x);
    u_xlat4.xyz = (-FGlobals._RefractionColorDark.xyz) + FGlobals._RefractionColorLight.xyz;
    u_xlat4.xyz = fma(float3(u_xlat36), u_xlat4.xyz, FGlobals._RefractionColorDark.xyz);
    u_xlat3.xzw = float3(u_xlat27) * u_xlat4.xyz;
    u_xlat3.xyz = fma(u_xlat3.xzw, float3(u_xlat15), u_xlat5.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat3.xyz;
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
