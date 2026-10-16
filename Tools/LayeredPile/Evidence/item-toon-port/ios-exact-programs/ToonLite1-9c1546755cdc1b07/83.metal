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
    sampler sampler_Matcap [[ sampler (4) ]],
    sampler sampler_RoughnessMap [[ sampler (5) ]],
    texture2d<half, access::sample > _NormalMap [[ texture(0) ]] ,
    texture2d<half, access::sample > _BaseColor [[ texture(1) ]] ,
    texture2d<half, access::sample > _AOTex [[ texture(2) ]] ,
    texture2d<half, access::sample > _Matcap [[ texture(3) ]] ,
    texture2d<half, access::sample > _RoughnessMap [[ texture(4) ]] ,
    depth2d<float, access::sample > _ShadowMapTexture [[ texture(5) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float3 u_xlat0;
    half3 u_xlat16_0;
    float3 u_xlat1;
    int u_xlati1;
    half3 u_xlat16_2;
    float3 u_xlat3;
    half3 u_xlat16_3;
    bool u_xlatb3;
    float4 u_xlat4;
    half2 u_xlat16_4;
    half3 u_xlat16_5;
    half3 u_xlat16_6;
    float3 u_xlat8;
    half3 u_xlat16_9;
    float u_xlat21;
    half u_xlat16_21;
    int u_xlati21;
    float u_xlat22;
    half u_xlat16_22;
    half u_xlat16_23;
    float u_xlat24;
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
    u_xlat16_21 = _AOTex.sample(sampler_AOTex, u_xlat1.xy).x;
    u_xlat1.x = fma(float(u_xlat16_21), 0.305306017, 0.682171106);
    u_xlat1.x = fma(float(u_xlat16_21), u_xlat1.x, 0.0125228781);
    u_xlat21 = fma(float(u_xlat16_21), u_xlat1.x, -1.0);
    u_xlat21 = fma(FGlobals._AOScale, u_xlat21, 1.0);
    u_xlat0.xyz = float3(u_xlat21) * u_xlat0.xyz;
    u_xlati21 = int(input.SV_InstanceID0) + UnityDrawCallInfo.unity_BaseInstanceID;
    u_xlati1 = u_xlati21 << 0x3;
    u_xlati21 = u_xlati21 * 0x7;
    u_xlat8.x = input.TEXCOORD2.w * UnityInstancing_PerDraw0[u_xlati1 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[1].y;
    u_xlat8.x = fma(UnityInstancing_PerDraw0[u_xlati1 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[0].y, input.TEXCOORD1.w, u_xlat8.x);
    u_xlat8.x = fma(UnityInstancing_PerDraw0[u_xlati1 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[2].y, input.TEXCOORD3.w, u_xlat8.x);
    u_xlat1.x = u_xlat8.x + UnityInstancing_PerDraw0[u_xlati1 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[3].y;
    u_xlat1.x = u_xlat1.x * FGlobals._FadeColorScale;
    u_xlat1.x = fma(u_xlat1.x, 0.5, FGlobals._FadeColorOffset);
    u_xlat1.x = u_xlat1.x + 0.5;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat8.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat1.xyz = fma(u_xlat1.xxx, u_xlat8.xyz, FGlobals._FadeColorF.xyz);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xyz;
    u_xlat1.xyz = input.TEXCOORD2.www * FGlobals.hlslcc_mtx4x4unity_WorldToShadow[1].xyz;
    u_xlat1.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[0].xyz, input.TEXCOORD1.www, u_xlat1.xyz);
    u_xlat1.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[2].xyz, input.TEXCOORD3.www, u_xlat1.xyz);
    u_xlat1.xyz = u_xlat1.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToShadow[3].xyz;
    u_xlat1.x = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat1.xy, saturate(u_xlat1.z), level(0.0));
    u_xlat16_2.x = (-FGlobals._LightShadowData.x) + half(1.0);
    u_xlat16_2.x = half(fma(u_xlat1.x, float(u_xlat16_2.x), float(FGlobals._LightShadowData.x)));
    u_xlat16_9.x = (-u_xlat16_2.x) + UnityInstancing_PerDraw2[u_xlati21 / 7].unity_SHCArray.w;
    u_xlat1.x = input.TEXCOORD1.w;
    u_xlat1.y = input.TEXCOORD2.w;
    u_xlat1.z = input.TEXCOORD3.w;
    u_xlat3.xyz = u_xlat1.xyz + (-FGlobals.unity_ShadowFadeCenterAndType.xyz);
    u_xlat1.xyz = (-u_xlat1.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat21 = dot(u_xlat3.xyz, u_xlat3.xyz);
    u_xlat21 = sqrt(u_xlat21);
    u_xlat3.x = FGlobals.hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat3.y = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat3.z = FGlobals.hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat22 = dot(u_xlat1.xyz, u_xlat3.xyz);
    u_xlat21 = u_xlat21 + (-u_xlat22);
    u_xlat21 = fma(FGlobals.unity_ShadowFadeCenterAndType.w, u_xlat21, u_xlat22);
    u_xlat21 = fma(u_xlat21, float(FGlobals._LightShadowData.z), float(FGlobals._LightShadowData.w));
    u_xlat21 = clamp(u_xlat21, 0.0f, 1.0f);
    u_xlat16_2.x = half(fma(u_xlat21, float(u_xlat16_9.x), float(u_xlat16_2.x)));
    u_xlat21 = dot(FGlobals._WorldSpaceLightPos0.xyz, FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat21 = rsqrt(u_xlat21);
    u_xlat3.xyz = float3(u_xlat21) * float3(FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat4.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_4.xy = _NormalMap.sample(sampler_NormalMap, u_xlat4.xy).yw;
    u_xlat16_9.xy = fma(u_xlat16_4.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_23 = dot(u_xlat16_9.xy, u_xlat16_9.xy);
    u_xlat16_5.xy = half2(float2(u_xlat16_9.xy) * float2(FGlobals._NormalScale));
    u_xlat16_9.x = min(u_xlat16_23, half(1.0));
    u_xlat16_9.x = (-u_xlat16_9.x) + half(1.0);
    u_xlat16_5.z = sqrt(u_xlat16_9.x);
    u_xlat16_6.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_6.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_6.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_9.x = dot(u_xlat16_6.xyz, u_xlat16_6.xyz);
    u_xlat16_9.x = rsqrt(u_xlat16_9.x);
    u_xlat16_9.xyz = u_xlat16_9.xxx * u_xlat16_6.xyz;
    u_xlat21 = dot(float3(u_xlat16_9.xyz), u_xlat3.xyz);
    u_xlat22 = u_xlat21 + FGlobals._BaseCellOffset;
    u_xlat22 = u_xlat22 / FGlobals._BaseCellSharpness;
    u_xlat22 = clamp(u_xlat22, 0.0f, 1.0f);
    u_xlat22 = float(u_xlat16_2.x) * u_xlat22;
    u_xlatb3 = FGlobals._LightColor0.w==half(0.0);
    u_xlat22 = (u_xlatb3) ? 0.0 : u_xlat22;
    u_xlat3.xyz = fma(float3(u_xlat22), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat24 = (-u_xlat21) + FGlobals._inverseLightOffset;
    u_xlat21 = u_xlat21 + FGlobals._HighLightOffset;
    u_xlat21 = u_xlat21 / FGlobals._HighLightSmooth;
    u_xlat21 = clamp(u_xlat21, 0.0f, 1.0f);
    u_xlat24 = u_xlat24 / FGlobals._inverseLightSmooth;
    u_xlat24 = clamp(u_xlat24, 0.0f, 1.0f);
    u_xlat3.xyz = fma(float3(u_xlat24), FGlobals._inverseColor.xyz, u_xlat3.xyz);
    u_xlat24 = (-u_xlat22) + 1.0;
    u_xlat22 = u_xlat22 + -1.0;
    u_xlat4.xyz = float3(u_xlat24) * FGlobals._ShadowAddColor.xyz;
    u_xlat0.xyz = fma(u_xlat3.xyz, u_xlat0.xyz, u_xlat4.xyz);
    u_xlat3.x = fma(u_xlat21, u_xlat21, (-u_xlat21));
    u_xlat21 = fma(FGlobals._HighLightPing, u_xlat3.x, u_xlat21);
    u_xlat3.xyz = float3(u_xlat21) * FGlobals._HighLightColor.xyz;
    u_xlat4.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), float2(u_xlat22), float2(1.0, 1.0));
    u_xlat21 = fma(FGlobals._RimShadowScale, u_xlat22, 1.0);
    u_xlat0.xyz = fma(u_xlat3.xyz, u_xlat4.xxx, u_xlat0.xyz);
    u_xlat3.xy = float2(u_xlat16_9.yy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat3.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].xy, float2(u_xlat16_9.xx), u_xlat3.xy);
    u_xlat3.xy = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].xy, float2(u_xlat16_9.zz), u_xlat3.xy);
    u_xlat3.xy = u_xlat3.xy * float2(FGlobals._ReflectScale);
    u_xlat3.xy = fma(u_xlat3.xy, float2(0.5, 0.5), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat3.xy = u_xlat3.xy + float2(0.5, 0.5);
    u_xlat16_3.xyz = _Matcap.sample(sampler_Matcap, u_xlat3.xy).xyz;
    u_xlat4.xzw = fma(float3(u_xlat16_3.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat4.xzw = fma(float3(u_xlat16_3.xyz), u_xlat4.xzw, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xyz = float3(u_xlat16_3.xyz) * u_xlat4.xzw;
    u_xlat3.xyz = u_xlat3.xyz * float3(FGlobals._MatcapScale);
    u_xlat3.xyz = u_xlat3.xyz * u_xlat4.yyy;
    u_xlat3.xyz = u_xlat3.xyz * FGlobals._MatcapColor.xyz;
    u_xlat4.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_22 = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat4.xy).x;
    u_xlat22 = fma(FGlobals._RoughnessScale, (-float(u_xlat16_22)), 1.0);
    u_xlat0.xyz = fma(u_xlat3.xyz, float3(u_xlat22), u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_2.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_2.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat22 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat22 = rsqrt(u_xlat22);
    u_xlat3.xyz = float3(u_xlat22) * u_xlat1.xyz;
    u_xlat1.xyz = fma((-u_xlat1.xyz), float3(u_xlat22), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat1.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat1.xyz, u_xlat3.xyz);
    u_xlat3.x = input.TEXCOORD1.z;
    u_xlat3.y = input.TEXCOORD2.z;
    u_xlat3.z = input.TEXCOORD3.z;
    u_xlat1.x = dot(u_xlat3.xyz, u_xlat1.xyz);
    u_xlat1.x = u_xlat1.x + FGlobals._RimOffset;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat1.x = (-u_xlat1.x) + 1.0;
    u_xlat1.x = log2(u_xlat1.x);
    u_xlat1.x = u_xlat1.x * FGlobals._RimPower;
    u_xlat1.x = exp2(u_xlat1.x);
    u_xlat21 = u_xlat21 * u_xlat1.x;
    u_xlat1.xyz = float3(u_xlat21) * FGlobals._RimColor.xyz;
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(FGlobals._RimLightMul), u_xlat0.xyz);
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
