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

fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    constant UnityDrawCallInfo_Type& UnityDrawCallInfo [[ buffer(1) ]],
    const constant unity_Builtins0Array_Type* UnityInstancing_PerDraw0 [[ buffer(2) ]],
    const constant unity_Builtins2Array_Type* UnityInstancing_PerDraw2 [[ buffer(3) ]],
    sampler sampler_NormalMap [[ sampler (0) ]],
    sampler sampler_BaseColor [[ sampler (1) ]],
    sampler sampler_AOTex [[ sampler (2) ]],
    sampler sampler_Matcap [[ sampler (3) ]],
    sampler sampler_RoughnessMap [[ sampler (4) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(3) ]] ,
    texture2d<half, access::sample > _RoughnessMap [[ texture(4) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float3 u_xlat0;
    half3 u_xlat16_0;
    float3 u_xlat1;
    half3 u_xlat16_1;
    int u_xlati1;
    float4 u_xlat2;
    half2 u_xlat16_2;
    half3 u_xlat16_3;
    half3 u_xlat16_4;
    float3 u_xlat6;
    bool u_xlatb6;
    half u_xlat16_13;
    float u_xlat15;
    half u_xlat16_15;
    int u_xlati15;
    float u_xlat16;
    half u_xlat16_16;
    half u_xlat16_18;
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
    u_xlat16_15 = _AOTex.sample(sampler_AOTex, u_xlat1.xy).x;
    u_xlat1.x = fma(float(u_xlat16_15), 0.305306017, 0.682171106);
    u_xlat1.x = fma(float(u_xlat16_15), u_xlat1.x, 0.0125228781);
    u_xlat15 = fma(float(u_xlat16_15), u_xlat1.x, -1.0);
    u_xlat15 = fma(FGlobals._AOScale, u_xlat15, 1.0);
    u_xlat0.xyz = float3(u_xlat15) * u_xlat0.xyz;
    u_xlati15 = int(input.SV_InstanceID0) + UnityDrawCallInfo.unity_BaseInstanceID;
    u_xlati1 = u_xlati15 << 0x3;
    u_xlati15 = u_xlati15 * 0x7;
    u_xlat6.x = input.TEXCOORD2.w * UnityInstancing_PerDraw0[u_xlati1 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[1].y;
    u_xlat6.x = fma(UnityInstancing_PerDraw0[u_xlati1 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[0].y, input.TEXCOORD1.w, u_xlat6.x);
    u_xlat6.x = fma(UnityInstancing_PerDraw0[u_xlati1 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[2].y, input.TEXCOORD3.w, u_xlat6.x);
    u_xlat1.x = u_xlat6.x + UnityInstancing_PerDraw0[u_xlati1 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[3].y;
    u_xlat1.x = u_xlat1.x * FGlobals._FadeColorScale;
    u_xlat1.x = fma(u_xlat1.x, 0.5, FGlobals._FadeColorOffset);
    u_xlat1.x = u_xlat1.x + 0.5;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat6.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat1.xyz = fma(u_xlat1.xxx, u_xlat6.xyz, FGlobals._FadeColorF.xyz);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xyz;
    u_xlat1.x = dot(FGlobals._WorldSpaceLightPos0.xyz, FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat1.x = rsqrt(u_xlat1.x);
    u_xlat1.xyz = u_xlat1.xxx * float3(FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat2.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_2.xy = _NormalMap.sample(sampler_NormalMap, u_xlat2.xy).yw;
    u_xlat16_3.xy = fma(u_xlat16_2.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_13 = dot(u_xlat16_3.xy, u_xlat16_3.xy);
    u_xlat16_4.xy = half2(float2(u_xlat16_3.xy) * float2(FGlobals._NormalScale));
    u_xlat16_3.x = min(u_xlat16_13, half(1.0));
    u_xlat16_3.x = (-u_xlat16_3.x) + half(1.0);
    u_xlat16_4.z = sqrt(u_xlat16_3.x);
    u_xlat16_3.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_3.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_3.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_4.xyz));
    u_xlat16_18 = dot(u_xlat16_3.xyz, u_xlat16_3.xyz);
    u_xlat16_18 = rsqrt(u_xlat16_18);
    u_xlat16_3.xyz = half3(u_xlat16_18) * u_xlat16_3.xyz;
    u_xlat1.x = dot(float3(u_xlat16_3.xyz), u_xlat1.xyz);
    u_xlat6.x = u_xlat1.x + FGlobals._BaseCellOffset;
    u_xlat6.x = u_xlat6.x / FGlobals._BaseCellSharpness;
    u_xlat6.x = clamp(u_xlat6.x, 0.0f, 1.0f);
    u_xlat15 = u_xlat6.x * float(UnityInstancing_PerDraw2[u_xlati15 / 7].unity_SHCArray.w);
    u_xlatb6 = FGlobals._LightColor0.w==half(0.0);
    u_xlat15 = (u_xlatb6) ? 0.0 : u_xlat15;
    u_xlat6.xyz = fma(float3(u_xlat15), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat2.x = (-u_xlat1.x) + FGlobals._inverseLightOffset;
    u_xlat1.x = u_xlat1.x + FGlobals._HighLightOffset;
    u_xlat1.x = u_xlat1.x / FGlobals._HighLightSmooth;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat2.x = u_xlat2.x / FGlobals._inverseLightSmooth;
    u_xlat2.x = clamp(u_xlat2.x, 0.0f, 1.0f);
    u_xlat6.xyz = fma(u_xlat2.xxx, FGlobals._inverseColor.xyz, u_xlat6.xyz);
    u_xlat2.x = (-u_xlat15) + 1.0;
    u_xlat15 = u_xlat15 + -1.0;
    u_xlat2.xyz = u_xlat2.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xyz = fma(u_xlat6.xyz, u_xlat0.xyz, u_xlat2.xyz);
    u_xlat6.x = fma(u_xlat1.x, u_xlat1.x, (-u_xlat1.x));
    u_xlat1.x = fma(FGlobals._HighLightPing, u_xlat6.x, u_xlat1.x);
    u_xlat1.xyz = u_xlat1.xxx * FGlobals._HighLightColor.xyz;
    u_xlat2.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), float2(u_xlat15), float2(1.0, 1.0));
    u_xlat15 = fma(FGlobals._RimShadowScale, u_xlat15, 1.0);
    u_xlat0.xyz = fma(u_xlat1.xyz, u_xlat2.xxx, u_xlat0.xyz);
    u_xlat1.xy = float2(u_xlat16_3.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat1.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_3.xx), u_xlat1.xy);
    u_xlat1.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_3.zz), u_xlat1.xy);
    u_xlat1.xy = u_xlat1.xy * float2(FGlobals._ReflectScale);
    u_xlat1.xy = fma(u_xlat1.xy, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
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
    u_xlat16_16 = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat2.xy).x;
    u_xlat16 = fma(FGlobals._RoughnessScale, (-float(u_xlat16_16)), 1.0);
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(u_xlat16), u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_3.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_3.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat1.x = input.TEXCOORD1.w;
    u_xlat1.y = input.TEXCOORD2.w;
    u_xlat1.z = input.TEXCOORD3.w;
    u_xlat1.xyz = (-u_xlat1.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat16 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat16 = rsqrt(u_xlat16);
    u_xlat2.xyz = float3(u_xlat16) * u_xlat1.xyz;
    u_xlat1.xyz = fma((-u_xlat1.xyz), float3(u_xlat16), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat1.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat1.xyz, u_xlat2.xyz);
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
    u_xlat15 = u_xlat15 * u_xlat1.x;
    u_xlat1.xyz = float3(u_xlat15) * FGlobals._RimColor.xyz;
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(FGlobals._RimLightMul), u_xlat0.xyz);
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
