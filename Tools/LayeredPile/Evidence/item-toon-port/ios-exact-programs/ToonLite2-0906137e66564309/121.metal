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
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_WorldToObject[4];
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
    half3 u_xlat16_0;
    float4 u_xlat1;
    float4 u_xlat2;
    float4 u_xlat3;
    float4 u_xlat4;
    float3 u_xlat5;
    float4 u_xlat6;
    half3 u_xlat16_6;
    bool u_xlatb6;
    float3 u_xlat7;
    half3 u_xlat16_8;
    half3 u_xlat16_9;
    float3 u_xlat10;
    float3 u_xlat11;
    float3 u_xlat16;
    half2 u_xlat16_16;
    float3 u_xlat17;
    half u_xlat16_17;
    bool u_xlatb17;
    half u_xlat16_20;
    float2 u_xlat24;
    float u_xlat28;
    float u_xlat29;
    half u_xlat16_32;
    float u_xlat36;
    half u_xlat16_36;
    float u_xlat37;
    float u_xlat40;
    float u_xlat42;
    half u_xlat16_42;
    float u_xlat43;
    half u_xlat16_44;
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
    u_xlat4 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToLight[1];
    u_xlat4 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[0], input.TEXCOORD4.xxxx, u_xlat4);
    u_xlat4 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToLight[2], input.TEXCOORD4.zzzz, u_xlat4);
    u_xlat4 = u_xlat4 + FGlobals.hlslcc_mtx4x4unity_WorldToLight[3];
    u_xlat5.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat6.x = FGlobals.hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat6.y = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat6.z = FGlobals.hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat5.x = dot(u_xlat5.xyz, u_xlat6.xyz);
    u_xlat17.xyz = input.TEXCOORD4.xyz + (-FGlobals.unity_ShadowFadeCenterAndType.xyz);
    u_xlat17.x = dot(u_xlat17.xyz, u_xlat17.xyz);
    u_xlat17.x = sqrt(u_xlat17.x);
    u_xlat17.x = (-u_xlat5.x) + u_xlat17.x;
    u_xlat5.x = fma(FGlobals.unity_ShadowFadeCenterAndType.w, u_xlat17.x, u_xlat5.x);
    u_xlat5.x = fma(u_xlat5.x, float(FGlobals._LightShadowData.z), float(FGlobals._LightShadowData.w));
    u_xlat5.x = clamp(u_xlat5.x, 0.0f, 1.0f);
    u_xlatb17 = u_xlat5.x<0.99000001;
    if(u_xlatb17){
        u_xlat6 = input.TEXCOORD4.yyyy * FGlobals.hlslcc_mtx4x4unity_WorldToShadow[1];
        u_xlat6 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[0], input.TEXCOORD4.xxxx, u_xlat6);
        u_xlat6 = fma(FGlobals.hlslcc_mtx4x4unity_WorldToShadow[2], input.TEXCOORD4.zzzz, u_xlat6);
        u_xlat6 = u_xlat6 + FGlobals.hlslcc_mtx4x4unity_WorldToShadow[3];
        u_xlat17.xyz = u_xlat6.xyz / u_xlat6.www;
        u_xlat6.xyz = u_xlat17.xyz + FGlobals._ShadowOffsets[0].xyz;
        u_xlat6.x = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat6.xy, saturate(u_xlat6.z), level(0.0));
        u_xlat7.xyz = u_xlat17.xyz + FGlobals._ShadowOffsets[1].xyz;
        u_xlat6.y = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat7.xy, saturate(u_xlat7.z), level(0.0));
        u_xlat7.xyz = u_xlat17.xyz + FGlobals._ShadowOffsets[2].xyz;
        u_xlat6.z = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat7.xy, saturate(u_xlat7.z), level(0.0));
        u_xlat17.xyz = u_xlat17.xyz + FGlobals._ShadowOffsets[3].xyz;
        u_xlat6.w = _ShadowMapTexture.sample_compare(_mtl_xl_shadow_sampler, u_xlat17.xy, saturate(u_xlat17.z), level(0.0));
        u_xlat17.x = dot(u_xlat6, float4(0.25, 0.25, 0.25, 0.25));
        u_xlat29 = (-float(FGlobals._LightShadowData.x)) + 1.0;
        u_xlat17.x = fma(u_xlat17.x, u_xlat29, float(FGlobals._LightShadowData.x));
        u_xlat16_17 = half(u_xlat17.x);
    } else {
        u_xlat16_17 = half(1.0);
    }
    u_xlat16_8.x = (-u_xlat16_17) + half(1.0);
    u_xlat16_8.x = half(fma(u_xlat5.x, float(u_xlat16_8.x), float(u_xlat16_17)));
    u_xlatb6 = 0.0<u_xlat4.z;
    u_xlat16_20 = (u_xlatb6) ? half(1.0) : half(0.0);
    u_xlat6.xy = u_xlat4.xy / u_xlat4.ww;
    u_xlat6.xy = u_xlat6.xy + float2(0.5, 0.5);
    u_xlat40 = _LightTexture0.sample(sampler_LightTexture0, u_xlat6.xy).w;
    u_xlat16_20 = half(u_xlat40 * float(u_xlat16_20));
    u_xlat4.x = dot(u_xlat4.xyz, u_xlat4.xyz);
    u_xlat4.x = _LightTextureB0.sample(sampler_LightTextureB0, u_xlat4.xx).x;
    u_xlat16_20 = half(u_xlat4.x * float(u_xlat16_20));
    u_xlat16_8.x = u_xlat16_8.x * u_xlat16_20;
    u_xlat16_8.xyz = u_xlat16_8.xxx * FGlobals._LightColor0.xyz;
    u_xlat16_9.xyz = half3(float3(FGlobals._LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_8.xyz = u_xlat16_8.xyz / u_xlat16_9.xyz;
    u_xlat4.x = max(float(u_xlat16_8.y), float(u_xlat16_8.x));
    u_xlat4.x = max(float(u_xlat16_8.z), u_xlat4.x);
    u_xlat16.xy = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_16.xy = _NormalMap.sample(sampler_NormalMap, u_xlat16.xy).yw;
    u_xlat16_8.xy = fma(u_xlat16_16.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_32 = dot(u_xlat16_8.xy, u_xlat16_8.xy);
    u_xlat16_32 = min(u_xlat16_32, half(1.0));
    u_xlat16_32 = (-u_xlat16_32) + half(1.0);
    u_xlat16_9.z = sqrt(u_xlat16_32);
    u_xlat16_9.xy = half2(float2(u_xlat16_8.xy) * float2(FGlobals._NormalScale));
    u_xlat16_8.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_9.xyz));
    u_xlat16_8.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_9.xyz));
    u_xlat16_8.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_9.xyz));
    u_xlat16_44 = dot(u_xlat16_8.xyz, u_xlat16_8.xyz);
    u_xlat16_44 = rsqrt(u_xlat16_44);
    u_xlat16_8.xyz = half3(u_xlat16_44) * u_xlat16_8.xyz;
    u_xlat16.xyz = (-input.TEXCOORD4.xyz) + FGlobals._WorldSpaceLightPos0.xyz;
    u_xlat6.x = dot(u_xlat16.xyz, u_xlat16.xyz);
    u_xlat6.x = rsqrt(u_xlat6.x);
    u_xlat16.xyz = u_xlat16.xyz * u_xlat6.xxx;
    u_xlat16.x = dot(float3(u_xlat16_8.xyz), u_xlat16.xyz);
    u_xlat28 = u_xlat16.x + FGlobals._BaseCellOffset;
    u_xlat28 = u_xlat28 / FGlobals._BaseCellSharpness;
    u_xlat28 = clamp(u_xlat28, 0.0f, 1.0f);
    u_xlat40 = u_xlat4.x * u_xlat28;
    u_xlat6.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_6.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat6.xy).xyz;
    u_xlat7.xyz = fma(float3(u_xlat16_6.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat7.xyz = fma(float3(u_xlat16_6.xyz), u_xlat7.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat6.xyz = float3(u_xlat16_6.xyz) * u_xlat7.xyz;
    u_xlat7.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat7.xyz = fma(FGlobals._BaseTint.xyz, u_xlat7.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat7.xyz = u_xlat7.xyz * FGlobals._BaseTint.xyz;
    u_xlat10.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_42 = _AOTex.sample(sampler_AOTex, u_xlat10.xy).x;
    u_xlat43 = fma(float(u_xlat16_42), 0.305306017, 0.682171106);
    u_xlat43 = fma(float(u_xlat16_42), u_xlat43, 0.0125228781);
    u_xlat42 = fma(float(u_xlat16_42), u_xlat43, -1.0);
    u_xlat42 = fma(FGlobals._AOScale, u_xlat42, 1.0);
    u_xlat6.xyz = u_xlat6.xyz * u_xlat7.xyz;
    u_xlat6.xyz = float3(u_xlat42) * u_xlat6.xyz;
    u_xlat16.x = u_xlat16.x + FGlobals._HighLightOffset;
    u_xlat16.x = u_xlat16.x / FGlobals._HighLightSmooth;
    u_xlat16.x = clamp(u_xlat16.x, 0.0f, 1.0f);
    u_xlat42 = fma(u_xlat16.x, u_xlat16.x, (-u_xlat16.x));
    u_xlat16.x = fma(FGlobals._HighLightPing, u_xlat42, u_xlat16.x);
    u_xlat42 = fma(u_xlat28, u_xlat4.x, -1.0);
    u_xlat7.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), float2(u_xlat42), float2(1.0, 1.0));
    u_xlat10.xyz = u_xlat16.xxx * FGlobals._HighLightColor.xyz;
    u_xlat11.xyz = input.TEXCOORD4.yyy * FGlobals.hlslcc_mtx4x4unity_WorldToObject[1].xyz;
    u_xlat11.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[0].xyz, input.TEXCOORD4.xxx, u_xlat11.xyz);
    u_xlat11.xyz = fma(FGlobals.hlslcc_mtx4x4unity_WorldToObject[2].xyz, input.TEXCOORD4.zzz, u_xlat11.xyz);
    u_xlat11.xyz = u_xlat11.xyz + FGlobals.hlslcc_mtx4x4unity_WorldToObject[3].xyz;
    u_xlat1 = u_xlat1 * u_xlat11.yyyy;
    u_xlat0 = fma(u_xlat0, u_xlat11.xxxx, u_xlat1);
    u_xlat0 = fma(u_xlat2, u_xlat11.zzzz, u_xlat0);
    u_xlat0 = u_xlat3 + u_xlat0;
    u_xlat1.x = dot(u_xlat0.xzw, u_xlat0.xzw);
    u_xlat1.x = max(u_xlat1.x, 0.00100000005);
    u_xlat1.x = rsqrt(u_xlat1.x);
    u_xlat0 = u_xlat0 * u_xlat1.xxxx;
    u_xlat1 = float4(u_xlat16_8.yyyy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].zxyz;
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].zxyz, float4(u_xlat16_8.xxxx), u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].zxyz, float4(u_xlat16_8.zzzz), u_xlat1);
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
    u_xlat37 = fma((-u_xlat28), u_xlat4.x, 1.0);
    u_xlat2.xyz = float3(u_xlat37) * FGlobals._ShadowAddColor.xyz;
    u_xlat1.xyz = fma(u_xlat1.xyz, u_xlat6.xyz, u_xlat2.xyz);
    u_xlat1.xyz = fma(u_xlat10.xyz, u_xlat7.xxx, u_xlat1.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(u_xlat36), u_xlat1.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_8.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_8.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
