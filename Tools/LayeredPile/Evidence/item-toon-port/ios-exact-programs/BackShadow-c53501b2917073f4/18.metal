#include <metal_stdlib>
#include <metal_texture>
using namespace metal;
struct VGlobals_Type
{
    float4 unity_4LightPosX0;
    float4 unity_4LightPosY0;
    float4 unity_4LightPosZ0;
    half4 unity_4LightAtten0;
    half4 unity_LightColor[8];
    half4 unity_SHBr;
    half4 unity_SHBg;
    half4 unity_SHBb;
    half4 unity_SHC;
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_WorldToObject[4];
    float4 unity_WorldTransformParams;
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 TANGENT0 [[ attribute(1) ]] ;
    float3 NORMAL0 [[ attribute(2) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 TEXCOORD0 [[ user(TEXCOORD0) ]];
    half3 TEXCOORD1 [[ user(TEXCOORD1) ]];
    float4 TEXCOORD5 [[ user(TEXCOORD5) ]];
    float4 TEXCOORD6 [[ user(TEXCOORD6) ]];
    float4 TEXCOORD7 [[ user(TEXCOORD7) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    float4 u_xlat2;
    float4 u_xlat3;
    half4 u_xlat16_3;
    float4 u_xlat4;
    float4 u_xlat5;
    float4 u_xlat6;
    half3 u_xlat16_7;
    half3 u_xlat16_8;
    float u_xlat27;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.TEXCOORD0 = float4(0.0, 0.0, 0.0, 0.0);
    u_xlat1 = (-u_xlat0.yyyy) + VGlobals.unity_4LightPosY0;
    u_xlat2 = u_xlat1 * u_xlat1;
    u_xlat3 = (-u_xlat0.xxxx) + VGlobals.unity_4LightPosX0;
    u_xlat2 = fma(u_xlat3, u_xlat3, u_xlat2);
    u_xlat4 = (-u_xlat0.zzzz) + VGlobals.unity_4LightPosZ0;
    u_xlat2 = fma(u_xlat4, u_xlat4, u_xlat2);
    u_xlat2 = max(u_xlat2, float4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat5 = fma(u_xlat2, float4(VGlobals.unity_4LightAtten0), float4(1.0, 1.0, 1.0, 1.0));
    u_xlat2 = rsqrt(u_xlat2);
    u_xlat5 = float4(1.0, 1.0, 1.0, 1.0) / u_xlat5;
    u_xlat6.x = dot(input.NORMAL0.xyz, VGlobals.hlslcc_mtx4x4unity_WorldToObject[0].xyz);
    u_xlat6.y = dot(input.NORMAL0.xyz, VGlobals.hlslcc_mtx4x4unity_WorldToObject[1].xyz);
    u_xlat6.z = dot(input.NORMAL0.xyz, VGlobals.hlslcc_mtx4x4unity_WorldToObject[2].xyz);
    u_xlat27 = dot(u_xlat6.xyz, u_xlat6.xyz);
    u_xlat27 = rsqrt(u_xlat27);
    u_xlat6 = float4(u_xlat27) * u_xlat6.xyzz;
    u_xlat1 = u_xlat1 * u_xlat6.yyyy;
    u_xlat1 = fma(u_xlat3, u_xlat6.xxxx, u_xlat1);
    u_xlat1 = fma(u_xlat4, u_xlat6.wwzw, u_xlat1);
    u_xlat1 = u_xlat2 * u_xlat1;
    u_xlat1 = max(u_xlat1, float4(0.0, 0.0, 0.0, 0.0));
    u_xlat1 = u_xlat5 * u_xlat1;
    u_xlat2.xyz = u_xlat1.yyy * float3(VGlobals.unity_LightColor[1].xyz);
    u_xlat2.xyz = fma(float3(VGlobals.unity_LightColor[0].xyz), u_xlat1.xxx, u_xlat2.xyz);
    u_xlat1.xyz = fma(float3(VGlobals.unity_LightColor[2].xyz), u_xlat1.zzz, u_xlat2.xyz);
    u_xlat1.xyz = fma(float3(VGlobals.unity_LightColor[3].xyz), u_xlat1.www, u_xlat1.xyz);
    u_xlat2.xyz = fma(u_xlat1.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat2.xyz = fma(u_xlat1.xyz, u_xlat2.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat16_7.x = half(u_xlat6.y * u_xlat6.y);
    u_xlat16_7.x = half(fma(u_xlat6.x, u_xlat6.x, (-float(u_xlat16_7.x))));
    u_xlat16_3 = half4(u_xlat6.ywzx * u_xlat6);
    u_xlat16_8.x = dot(VGlobals.unity_SHBr, u_xlat16_3);
    u_xlat16_8.y = dot(VGlobals.unity_SHBg, u_xlat16_3);
    u_xlat16_8.z = dot(VGlobals.unity_SHBb, u_xlat16_3);
    u_xlat16_7.xyz = fma(VGlobals.unity_SHC.xyz, u_xlat16_7.xxx, u_xlat16_8.xyz);
    output.TEXCOORD1.xyz = half3(fma(u_xlat1.xyz, u_xlat2.xyz, float3(u_xlat16_7.xyz)));
    output.TEXCOORD5.w = u_xlat0.x;
    u_xlat1.xyz = input.TANGENT0.yyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1].yzx;
    u_xlat1.xyz = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0].yzx, input.TANGENT0.xxx, u_xlat1.xyz);
    u_xlat1.xyz = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2].yzx, input.TANGENT0.zzz, u_xlat1.xyz);
    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat0.x = rsqrt(u_xlat0.x);
    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz;
    u_xlat16_7.xyz = half3(u_xlat1.xyz * u_xlat6.wxy);
    u_xlat16_7.xyz = half3(fma(u_xlat6.ywx, u_xlat1.yzx, (-float3(u_xlat16_7.xyz))));
    u_xlat0.x = input.TANGENT0.w * VGlobals.unity_WorldTransformParams.w;
    u_xlat16_7.xyz = half3(u_xlat0.xxx * float3(u_xlat16_7.xyz));
    output.TEXCOORD5.y = float(u_xlat16_7.x);
    output.TEXCOORD5.z = u_xlat6.x;
    output.TEXCOORD5.x = u_xlat1.z;
    output.TEXCOORD6.x = u_xlat1.x;
    output.TEXCOORD7.x = u_xlat1.y;
    output.TEXCOORD6.z = u_xlat6.y;
    output.TEXCOORD7.z = u_xlat6.w;
    output.TEXCOORD6.w = u_xlat0.y;
    output.TEXCOORD7.w = u_xlat0.z;
    output.TEXCOORD6.y = float(u_xlat16_7.y);
    output.TEXCOORD7.y = float(u_xlat16_7.z);
    return output;
}
