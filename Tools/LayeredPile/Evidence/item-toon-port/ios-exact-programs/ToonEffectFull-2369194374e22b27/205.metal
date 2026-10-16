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
    half3 u_xlat16_0;
    float4 u_xlat1;
    float4 u_xlat2;
    float4 u_xlat3;
    float3 u_xlat4;
    float3 u_xlat5;
    float3 u_xlat6;
    half3 u_xlat16_6;
    float4 u_xlat7;
    float3 u_xlat8;
    half3 u_xlat16_9;
    half3 u_xlat16_10;
    float3 u_xlat11;
    float3 u_xlat16;
    half2 u_xlat16_16;
    half u_xlat16_21;
    float2 u_xlat24;
    float u_xlat28;
    half u_xlat16_33;
    float u_xlat36;
    half u_xlat16_36;
    float u_xlat37;
    float u_xlat40;
    float u_xlat41;
    bool u_xlatb41;
    float u_xlat42;
    half u_xlat16_42;
    float u_xlat43;
    half u_xlat16_45;
    u_xlat0 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat0 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].xxxx, u_xlat0);
    u_xlat0 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].zzzz, u_xlat0);
    u_xlat0 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].wwww, u_xlat0);
    u_xlat1 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].xxxx, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].zzzz, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].wwww, u_xlat1);
    u_xlat2 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].xxxx, u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].zzzz, u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].wwww, u_xlat2);
    u_xlat3 = FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].yyyy * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx;
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].xxxx, u_xlat3);
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].zzzz, u_xlat3);
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, FGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3].wwww, u_xlat3);
    u_xlat4.xyz = input.TEXCOORD4.yyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1].xyz;
    u_xlat4.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0].xyz, input.TEXCOORD4.xxx, u_xlat4.xyz);
    u_xlat4.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2].xyz, input.TEXCOORD4.zzz, u_xlat4.xyz);
    u_xlat4.xyz = u_xlat4.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3].xyz;
    u_xlat5.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat6.x = FGlobals.hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat6.y = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat6.z = FGlobals.hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat40 = dot(u_xlat5.xyz, u_xlat6.xyz);
    u_xlat6.xyz = input.TEXCOORD4.xyz + (-FGlobals.unity_ShadowFadeCenterAndType.xyz);
    u_xlat41 = dot(u_xlat6.xyz, u_xlat6.xyz);
    u_xlat41 = sqrt(u_xlat41);
    u_xlat41 = (-u_xlat40) + u_xlat41;
    u_xlat40 = fma(FGlobals.unity_ShadowFadeCenterAndType.w, u_xlat41, u_xlat40);
    u_xlat40 = fma(u_xlat40, float(FGlobals._LightShadowData.z), float(FGlobals._LightShadowData.w));
    u_xlat40 = clamp(u_xlat40, 0.0f, 1.0f);
    u_xlatb41 = u_xlat40<0.99000001;
    if(u_xlatb41){
        u_xlat6.xyz = input.TEXCOORD4.xyz + (-FGlobals._LightPositionRange.xyz);
        u_xlat41 = max(abs(u_xlat6.y), abs(u_xlat6.x));
        u_xlat41 = max(abs(u_xlat6.z), u_xlat41);
        u_xlat41 = u_xlat41 + (-FGlobals._LightProjectionParams.z);
        u_xlat41 = max(u_xlat41, 9.99999975e-06);
        u_xlat41 = u_xlat41 * FGlobals._LightProjectionParams.w;
        u_xlat41 = FGlobals._LightProjectionParams.y / u_xlat41;
        u_xlat41 = u_xlat41 + (-FGlobals._LightProjectionParams.x);
        u_xlat41 = (-u_xlat41) + 1.0;
        u_xlat7.xyz = u_xlat6.xyz + float3(0.0078125, 0.0078125, 0.0078125);
        u_xlat7.x = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat7.xyz, saturate(u_xlat41), level(0.0));
        u_xlat8.xyz = u_xlat6.xyz + float3(-0.0078125, -0.0078125, 0.0078125);
        u_xlat7.y = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat8.xyz, saturate(u_xlat41), level(0.0));
        u_xlat8.xyz = u_xlat6.xyz + float3(-0.0078125, 0.0078125, -0.0078125);
        u_xlat7.z = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat8.xyz, saturate(u_xlat41), level(0.0));
        u_xlat6.xyz = u_xlat6.xyz + float3(0.0078125, -0.0078125, -0.0078125);
        u_xlat7.w = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat6.xyz, saturate(u_xlat41), level(0.0));
        u_xlat41 = dot(u_xlat7, float4(0.25, 0.25, 0.25, 0.25));
        u_xlat16_9.x = (-FGlobals._LightShadowData.x) + half(1.0);
        u_xlat16_9.x = half(fma(u_xlat41, float(u_xlat16_9.x), float(FGlobals._LightShadowData.x)));
    } else {
        u_xlat16_9.x = half(1.0);
    }
    u_xlat16_21 = (-u_xlat16_9.x) + half(1.0);
    u_xlat16_9.x = half(fma(u_xlat40, float(u_xlat16_21), float(u_xlat16_9.x)));
    u_xlat4.x = dot(u_xlat4.xyz, u_xlat4.xyz);
    u_xlat4.x = _LightTexture0.sample(sampler_LightTexture0, u_xlat4.xx).x;
    u_xlat4.x = float(u_xlat16_9.x) * u_xlat4.x;
    u_xlat16_9.xyz = half3(u_xlat4.xxx * float3(FGlobals._LightColor0.xyz));
    u_xlat16_10.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_9.xyz = u_xlat16_9.xyz / u_xlat16_10.xyz;
    u_xlat4.x = max(float(u_xlat16_9.y), float(u_xlat16_9.x));
    u_xlat4.x = max(float(u_xlat16_9.z), u_xlat4.x);
    u_xlat16.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_16.xy = _NormalMap.sample(sampler_NormalMap, u_xlat16.xy).yw;
    u_xlat16_9.xy = fma(u_xlat16_16.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_33 = dot(u_xlat16_9.xy, u_xlat16_9.xy);
    u_xlat16_33 = min(u_xlat16_33, half(1.0));
    u_xlat16_33 = (-u_xlat16_33) + half(1.0);
    u_xlat16_10.z = sqrt(u_xlat16_33);
    u_xlat16_10.xy = half2(float2(u_xlat16_9.xy) * float2(FGlobals._NormalScale));
    u_xlat16_9.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_10.xyz));
    u_xlat16_9.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_10.xyz));
    u_xlat16_9.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_10.xyz));
    u_xlat16_45 = dot(u_xlat16_9.xyz, u_xlat16_9.xyz);
    u_xlat16_45 = rsqrt(u_xlat16_45);
    u_xlat16_9.xyz = half3(u_xlat16_45) * u_xlat16_9.xyz;
    u_xlat16.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat41 = dot(u_xlat16.xyz, u_xlat16.xyz);
    u_xlat41 = rsqrt(u_xlat41);
    u_xlat16.xyz = u_xlat16.xyz * float3(u_xlat41);
    u_xlat16.x = dot(float3(u_xlat16_9.xyz), u_xlat16.xyz);
    u_xlat28 = u_xlat16.x + FGlobals._BaseCellOffset;
    u_xlat28 = u_xlat28 / FGlobals._BaseCellSharpness;
    u_xlat28 = clamp(u_xlat28, 0.0f, 1.0f);
    u_xlat40 = u_xlat4.x * u_xlat28;
    u_xlat41 = (-u_xlat16.x) + FGlobals._inverseLightOffset;
    u_xlat41 = u_xlat41 / FGlobals._inverseLightSmooth;
    u_xlat41 = clamp(u_xlat41, 0.0f, 1.0f);
    u_xlat6.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_6.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat6.xy).xyz;
    u_xlat7.xyz = fma(float3(u_xlat16_6.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat7.xyz = fma(float3(u_xlat16_6.xyz), u_xlat7.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat6.xyz = float3(u_xlat16_6.xyz) * u_xlat7.xyz;
    u_xlat7.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat7.xyz = fma(FGlobals._BaseTint.xyz, u_xlat7.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat7.xyz = u_xlat7.xyz * FGlobals._BaseTint.xyz;
    u_xlat8.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_42 = _AOTex.sample(sampler_AOTex, u_xlat8.xy).x;
    u_xlat43 = fma(float(u_xlat16_42), 0.305306017, 0.682171106);
    u_xlat43 = fma(float(u_xlat16_42), u_xlat43, 0.0125228781);
    u_xlat42 = fma(float(u_xlat16_42), u_xlat43, -1.0);
    u_xlat42 = fma(FGlobals._AOScale, u_xlat42, 1.0);
    u_xlat8.xyz = input.TEXCOORD4.yyy * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].xyz;
    u_xlat8.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].xyz, input.TEXCOORD4.xxx, u_xlat8.xyz);
    u_xlat8.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].xyz, input.TEXCOORD4.zzz, u_xlat8.xyz);
    u_xlat8.xyz = u_xlat8.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToObject[3].xyz;
    u_xlat43 = u_xlat8.y * FGlobals._FadeColorScale;
    u_xlat43 = fma(u_xlat43, 0.5, FGlobals._FadeColorOffset);
    u_xlat43 = u_xlat43 + 0.5;
    u_xlat43 = clamp(u_xlat43, 0.0f, 1.0f);
    u_xlat11.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat11.xyz = fma(float3(u_xlat43), u_xlat11.xyz, FGlobals._FadeColorF.xyz);
    u_xlat6.xyz = u_xlat6.xyz * u_xlat7.xyz;
    u_xlat6.xyz = float3(u_xlat42) * u_xlat6.xyz;
    u_xlat6.xyz = u_xlat11.xyz * u_xlat6.xyz;
    u_xlat16.x = u_xlat16.x + FGlobals._HighLightOffset;
    u_xlat16.x = u_xlat16.x / FGlobals._HighLightSmooth;
    u_xlat16.x = clamp(u_xlat16.x, 0.0f, 1.0f);
    u_xlat42 = fma(u_xlat16.x, u_xlat16.x, (-u_xlat16.x));
    u_xlat16.x = fma(FGlobals._HighLightPing, u_xlat42, u_xlat16.x);
    u_xlat42 = fma(u_xlat28, u_xlat4.x, -1.0);
    u_xlat7.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), float2(u_xlat42), float2(1.0, 1.0));
    u_xlat11.xyz = u_xlat16.xxx * FGlobals._HighLightColor.xyz;
    u_xlat1 = u_xlat1 * u_xlat8.yyyy;
    u_xlat0 = fma(u_xlat0, u_xlat8.xxxx, u_xlat1);
    u_xlat0 = fma(u_xlat2, u_xlat8.zzzz, u_xlat0);
    u_xlat0 = u_xlat3 + u_xlat0;
    u_xlat1.x = dot(u_xlat0.xzw, u_xlat0.xzw);
    u_xlat1.x = max(u_xlat1.x, 0.00100000005);
    u_xlat1.x = rsqrt(u_xlat1.x);
    u_xlat0 = u_xlat0 * u_xlat1.xxxx;
    u_xlat1 = float4(u_xlat16_9.yyyy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].zxyz;
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].zxyz, float4(u_xlat16_9.xxxx), u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].zxyz, float4(u_xlat16_9.zzzz), u_xlat1);
    u_xlat24.xy = u_xlat0.zw * u_xlat1.zw;
    u_xlat0.xy = fma(u_xlat0.xy, u_xlat1.xy, (-u_xlat24.xy));
    u_xlat36 = FGlobals._ReflectScale * 0.5;
    u_xlat0.z = (-u_xlat0.y);
    u_xlat0.xy = fma(u_xlat0.zx, float2(u_xlat36), FGlobals._ReflectOffset.xyxx.xy);
    u_xlat0.xy = u_xlat0.xy + float2(0.5, 0.5);
    u_xlat16_0.xyz = _Matcap.sample(sampler_Matcap, u_xlat0.xy).xyz;
    u_xlat1.xyz = fma(float3(u_xlat16_0.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat1.xyz = fma(float3(u_xlat16_0.xyz), u_xlat1.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat0.xyz = float3(u_xlat16_0.xyz) * u_xlat1.xyz;
    u_xlat1.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_36 = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat1.xy).x;
    u_xlat36 = fma(FGlobals._RoughnessScale, (-float(u_xlat16_36)), 1.0);
    u_xlat0.xyz = u_xlat0.xyz * float3(FGlobals._MatcapScale);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat7.yyy;
    u_xlat0.xyz = u_xlat0.xyz * FGlobals._MatcapColor.xyz;
    u_xlat1.xyz = fma(float3(u_xlat40), FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat1.xyz = fma(float3(u_xlat41), FGlobals._inverseColor.xyz, u_xlat1.xyz);
    u_xlat37 = fma((-u_xlat28), u_xlat4.x, 1.0);
    u_xlat2.xyz = float3(u_xlat37) * FGlobals._ShadowAddColor.xyz;
    u_xlat1.xyz = fma(u_xlat1.xyz, u_xlat6.xyz, u_xlat2.xyz);
    u_xlat1.xyz = fma(u_xlat11.xyz, u_xlat7.xxx, u_xlat1.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(u_xlat36), u_xlat1.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_9.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_9.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat36 = dot(u_xlat5.xyz, u_xlat5.xyz);
    u_xlat36 = rsqrt(u_xlat36);
    u_xlat1.xyz = float3(u_xlat36) * u_xlat5.xyz;
    u_xlat2.xyz = fma((-u_xlat5.xyz), float3(u_xlat36), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat1.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat2.xyz, u_xlat1.xyz);
    u_xlat2.x = input.TEXCOORD1.z;
    u_xlat2.y = input.TEXCOORD2.z;
    u_xlat2.z = input.TEXCOORD3.z;
    u_xlat36 = dot(u_xlat2.xyz, u_xlat1.xyz);
    u_xlat1.x = fma(FGlobals._RimShadowScale, u_xlat42, 1.0);
    u_xlat36 = u_xlat36 + FGlobals._RimOffset;
    u_xlat36 = clamp(u_xlat36, 0.0f, 1.0f);
    u_xlat36 = (-u_xlat36) + 1.0;
    u_xlat36 = log2(u_xlat36);
    u_xlat36 = u_xlat36 * FGlobals._RimPower;
    u_xlat36 = exp2(u_xlat36);
    u_xlat36 = u_xlat1.x * u_xlat36;
    u_xlat1.xyz = float3(u_xlat36) * FGlobals._RimColor.xyz;
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(FGlobals._RimLightMul), u_xlat0.xyz);
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
