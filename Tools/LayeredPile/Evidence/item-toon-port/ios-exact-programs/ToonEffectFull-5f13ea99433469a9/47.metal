#include <metal_stdlib>
#include <metal_texture>
using namespace metal;
struct VGlobals_Type
{
    float4 unity_WorldTransformParams;
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
    float4 _texcoord_ST;
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

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 TANGENT0 [[ attribute(1) ]] ;
    float3 NORMAL0 [[ attribute(2) ]] ;
    float4 TEXCOORD0 [[ attribute(3) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float2 TEXCOORD0 [[ user(TEXCOORD0) ]];
    float4 TEXCOORD1 [[ user(TEXCOORD1) ]];
    float4 TEXCOORD2 [[ user(TEXCOORD2) ]];
    float4 TEXCOORD3 [[ user(TEXCOORD3) ]];
    half3 TEXCOORD4 [[ user(TEXCOORD4) ]];
    float4 TEXCOORD6 [[ user(TEXCOORD6) ]];
    float4 TEXCOORD7 [[ user(TEXCOORD7) ]];
    uint SV_InstanceID0 [[ user(SV_InstanceID0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    constant UnityDrawCallInfo_Type& UnityDrawCallInfo [[ buffer(1) ]],
    const constant unity_Builtins0Array_Type* UnityInstancing_PerDraw0 [[ buffer(2) ]],
    const constant unity_Builtins2Array_Type* UnityInstancing_PerDraw2 [[ buffer(3) ]],
    uint mtl_InstanceID [[ instance_id ]],
    uint mtl_BaseInstance [[ base_instance ]],
    Mtl_VertexIn input [[ stage_in ]])
{
#if !UNITY_SUPPORT_INDIRECT_BUFFERS
    mtl_BaseInstance = 0;
#endif
    mtl_InstanceID = mtl_InstanceID - mtl_BaseInstance;
    Mtl_VertexOut output;
    int u_xlati0;
    float4 u_xlat1;
    half4 u_xlat16_1;
    float4 u_xlat2;
    float4 u_xlat3;
    half u_xlat16_4;
    half3 u_xlat16_5;
    float3 u_xlat6;
    int u_xlati6;
    float u_xlat12;
    float u_xlat19;
    u_xlati0 = int(mtl_InstanceID) + UnityDrawCallInfo.unity_BaseInstanceID;
    u_xlati6 = u_xlati0 << 0x3;
    u_xlati0 = u_xlati0 * 0x7;
    u_xlat1 = input.POSITION0.yyyy * UnityInstancing_PerDraw0[u_xlati6 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1];
    u_xlat1 = fma(UnityInstancing_PerDraw0[u_xlati6 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0], input.POSITION0.xxxx, u_xlat1);
    u_xlat1 = fma(UnityInstancing_PerDraw0[u_xlati6 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2], input.POSITION0.zzzz, u_xlat1);
    u_xlat1 = u_xlat1 + UnityInstancing_PerDraw0[u_xlati6 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3];
    u_xlat2 = u_xlat1.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat2 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat1.xxxx, u_xlat2);
    u_xlat2 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat1.zzzz, u_xlat2);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat1.wwww, u_xlat2);
    output.TEXCOORD0.xy = fma(input.TEXCOORD0.xy, VGlobals._texcoord_ST.xy, VGlobals._texcoord_ST.zw);
    u_xlat2.xyz = input.TANGENT0.yyy * UnityInstancing_PerDraw0[u_xlati6 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1].yzx;
    u_xlat2.xyz = fma(UnityInstancing_PerDraw0[u_xlati6 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0].yzx, input.TANGENT0.xxx, u_xlat2.xyz);
    u_xlat2.xyz = fma(UnityInstancing_PerDraw0[u_xlati6 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2].yzx, input.TANGENT0.zzz, u_xlat2.xyz);
    u_xlat12 = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat12 = rsqrt(u_xlat12);
    u_xlat2.xyz = float3(u_xlat12) * u_xlat2.xyz;
    u_xlat3.x = dot(input.NORMAL0.xyz, UnityInstancing_PerDraw0[u_xlati6 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[0].xyz);
    u_xlat3.y = dot(input.NORMAL0.xyz, UnityInstancing_PerDraw0[u_xlati6 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[1].xyz);
    u_xlat3.z = dot(input.NORMAL0.xyz, UnityInstancing_PerDraw0[u_xlati6 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[2].xyz);
    u_xlat6.x = dot(u_xlat3.xyz, u_xlat3.xyz);
    u_xlat6.x = rsqrt(u_xlat6.x);
    u_xlat3 = u_xlat6.xxxx * u_xlat3.xyzz;
    u_xlat6.xyz = u_xlat2.xyz * u_xlat3.wxy;
    u_xlat6.xyz = fma(u_xlat3.ywx, u_xlat2.yzx, (-u_xlat6.xyz));
    u_xlat19 = input.TANGENT0.w * VGlobals.unity_WorldTransformParams.w;
    u_xlat6.xyz = u_xlat6.xyz * float3(u_xlat19);
    output.TEXCOORD1.y = u_xlat6.x;
    output.TEXCOORD1.x = u_xlat2.z;
    output.TEXCOORD1.z = u_xlat3.x;
    output.TEXCOORD1.w = u_xlat1.x;
    output.TEXCOORD2.x = u_xlat2.x;
    output.TEXCOORD3.x = u_xlat2.y;
    output.TEXCOORD2.w = u_xlat1.y;
    output.TEXCOORD3.w = u_xlat1.z;
    output.TEXCOORD2.y = u_xlat6.y;
    output.TEXCOORD3.y = u_xlat6.z;
    output.TEXCOORD2.z = u_xlat3.y;
    output.TEXCOORD3.z = u_xlat3.w;
    u_xlat16_4 = half(u_xlat3.y * u_xlat3.y);
    u_xlat16_4 = half(fma(u_xlat3.x, u_xlat3.x, (-float(u_xlat16_4))));
    u_xlat16_1 = half4(u_xlat3.ywzx * u_xlat3);
    u_xlat16_5.x = dot(UnityInstancing_PerDraw2[u_xlati0 / 7].unity_SHBrArray, u_xlat16_1);
    u_xlat16_5.y = dot(UnityInstancing_PerDraw2[u_xlati0 / 7].unity_SHBgArray, u_xlat16_1);
    u_xlat16_5.z = dot(UnityInstancing_PerDraw2[u_xlati0 / 7].unity_SHBbArray, u_xlat16_1);
    output.TEXCOORD4.xyz = fma(UnityInstancing_PerDraw2[u_xlati0 / 7].unity_SHCArray.xyz, half3(u_xlat16_4), u_xlat16_5.xyz);
    output.TEXCOORD6 = float4(0.0, 0.0, 0.0, 0.0);
    output.TEXCOORD7 = float4(0.0, 0.0, 0.0, 0.0);
    output.SV_InstanceID0 = mtl_InstanceID;
    return output;
}
