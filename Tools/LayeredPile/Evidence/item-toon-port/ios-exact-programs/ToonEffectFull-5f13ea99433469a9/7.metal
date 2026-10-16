#include <metal_stdlib>
#include <metal_texture>
using namespace metal;
struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
    float _OutlineW;
    float GlobalOutlineWidth;
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

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float3 NORMAL0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float3 TEXCOORD0 [[ user(TEXCOORD0) ]];
    float3 TEXCOORD1 [[ user(TEXCOORD1) ]];
    uint SV_InstanceID0 [[ user(SV_InstanceID0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    constant UnityDrawCallInfo_Type& UnityDrawCallInfo [[ buffer(1) ]],
    const constant unity_Builtins0Array_Type* UnityInstancing_PerDraw0 [[ buffer(2) ]],
    uint mtl_InstanceID [[ instance_id ]],
    uint mtl_BaseInstance [[ base_instance ]],
    Mtl_VertexIn input [[ stage_in ]])
{
#if !UNITY_SUPPORT_INDIRECT_BUFFERS
    mtl_BaseInstance = 0;
#endif
    mtl_InstanceID = mtl_InstanceID - mtl_BaseInstance;
    Mtl_VertexOut output;
    float3 u_xlat0;
    float4 u_xlat1;
    float4 u_xlat2;
    float u_xlat9;
    int u_xlati9;
    u_xlat0.x = VGlobals.GlobalOutlineWidth * VGlobals._OutlineW;
    u_xlat0.xyz = fma(input.NORMAL0.xyz, u_xlat0.xxx, input.POSITION0.xyz);
    u_xlati9 = int(mtl_InstanceID) + UnityDrawCallInfo.unity_BaseInstanceID;
    u_xlati9 = u_xlati9 << 0x3;
    u_xlat1 = u_xlat0.yyyy * UnityInstancing_PerDraw0[u_xlati9 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1];
    u_xlat1 = fma(UnityInstancing_PerDraw0[u_xlati9 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(UnityInstancing_PerDraw0[u_xlati9 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2], u_xlat0.zzzz, u_xlat1);
    u_xlat2 = u_xlat1 + UnityInstancing_PerDraw0[u_xlati9 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3];
    output.TEXCOORD1.xyz = fma(UnityInstancing_PerDraw0[u_xlati9 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3].xyz, input.POSITION0.www, u_xlat1.xyz);
    u_xlat1 = u_xlat2.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat2.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat2.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat2.wwww, u_xlat1);
    u_xlat0.x = dot(input.NORMAL0.xyz, UnityInstancing_PerDraw0[u_xlati9 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[0].xyz);
    u_xlat0.y = dot(input.NORMAL0.xyz, UnityInstancing_PerDraw0[u_xlati9 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[1].xyz);
    u_xlat0.z = dot(input.NORMAL0.xyz, UnityInstancing_PerDraw0[u_xlati9 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[2].xyz);
    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat9 = rsqrt(u_xlat9);
    output.TEXCOORD0.xyz = float3(u_xlat9) * u_xlat0.xyz;
    output.SV_InstanceID0 = mtl_InstanceID;
    return output;
}
