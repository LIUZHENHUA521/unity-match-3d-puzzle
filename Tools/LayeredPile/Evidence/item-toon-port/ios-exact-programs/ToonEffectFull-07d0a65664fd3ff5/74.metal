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
    float4 u_xlat0;
    int u_xlati0;
    float4 u_xlat1;
    half3 u_xlat16_1;
    bool u_xlatb1;
    float4 u_xlat2;
    float4 u_xlat3;
    half3 u_xlat16_4;
    half3 u_xlat16_5;
    float3 u_xlat6;
    half3 u_xlat16_6;
    float3 u_xlat7;
    float3 u_xlat8;
    half3 u_xlat16_8;
    int u_xlati8;
    float3 u_xlat9;
    float u_xlat16;
    half u_xlat16_16;
    float2 u_xlat17;
    half u_xlat16_20;
    float u_xlat24;
    half u_xlat16_28;
    u_xlati0 = int(input.SV_InstanceID0) + UnityDrawCallInfo.unity_BaseInstanceID;
    u_xlati8 = u_xlati0 << 0x3;
    u_xlati0 = u_xlati0 * 0x7;
    u_xlat1 = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx * UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1].yyyy;
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1].xxxx, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1].zzzz, u_xlat1);
    u_xlat1 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[1].wwww, u_xlat1);
    u_xlat2.xyz = input.TEXCOORD2.www * UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[1].xyz;
    u_xlat2.xyz = fma(UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[0].xyz, input.TEXCOORD1.www, u_xlat2.xyz);
    u_xlat2.xyz = fma(UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[2].xyz, input.TEXCOORD3.www, u_xlat2.xyz);
    u_xlat2.xyz = u_xlat2.xyz + UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_WorldToObjectArray[3].xyz;
    u_xlat1 = u_xlat1 * u_xlat2.yyyy;
    u_xlat3 = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx * UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0].yyyy;
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0].xxxx, u_xlat3);
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0].zzzz, u_xlat3);
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[0].wwww, u_xlat3);
    u_xlat1 = fma(u_xlat3, u_xlat2.xxxx, u_xlat1);
    u_xlat3 = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx * UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2].yyyy;
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2].xxxx, u_xlat3);
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2].zzzz, u_xlat3);
    u_xlat3 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[2].wwww, u_xlat3);
    u_xlat1 = fma(u_xlat3, u_xlat2.zzzz, u_xlat1);
    u_xlat16 = u_xlat2.y * FGlobals._FadeColorScale;
    u_xlat16 = fma(u_xlat16, 0.5, FGlobals._FadeColorOffset);
    u_xlat16 = u_xlat16 + 0.5;
    u_xlat16 = clamp(u_xlat16, 0.0f, 1.0f);
    u_xlat2 = FGlobals.hlslcc_mtx4x4unity_MatrixV[1].yzzx * UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3].yyyy;
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3].xxxx, u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3].zzzz, u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[3].yzzx, UnityInstancing_PerDraw0[u_xlati8 / 8].hlslcc_mtx4x4unity_ObjectToWorldArray[3].wwww, u_xlat2);
    u_xlat1 = u_xlat1 + u_xlat2;
    u_xlat8.x = dot(u_xlat1.xzw, u_xlat1.xzw);
    u_xlat8.x = max(u_xlat8.x, 0.00100000005);
    u_xlat8.x = rsqrt(u_xlat8.x);
    u_xlat1 = u_xlat8.xxxx * u_xlat1;
    u_xlat8.xz = fma(input.TEXCOORD0.xy, FGlobals._NormalMap_ST.xy, FGlobals._NormalMap_ST.zw);
    u_xlat16_8.xz = _NormalMap.sample(sampler_NormalMap, u_xlat8.xz).yw;
    u_xlat16_4.xy = fma(u_xlat16_8.zx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_20 = dot(u_xlat16_4.xy, u_xlat16_4.xy);
    u_xlat16_5.xy = half2(float2(u_xlat16_4.xy) * float2(FGlobals._NormalScale));
    u_xlat16_4.x = min(u_xlat16_20, half(1.0));
    u_xlat16_4.x = (-u_xlat16_4.x) + half(1.0);
    u_xlat16_5.z = sqrt(u_xlat16_4.x);
    u_xlat16_4.x = dot(input.TEXCOORD1.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_4.y = dot(input.TEXCOORD2.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_4.z = dot(input.TEXCOORD3.xyz, float3(u_xlat16_5.xyz));
    u_xlat16_28 = dot(u_xlat16_4.xyz, u_xlat16_4.xyz);
    u_xlat16_28 = rsqrt(u_xlat16_28);
    u_xlat16_4.xyz = half3(u_xlat16_28) * u_xlat16_4.xyz;
    u_xlat2 = float4(u_xlat16_4.yyyy) * FGlobals.hlslcc_mtx4x4unity_MatrixV[1].zxyz;
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[0].zxyz, float4(u_xlat16_4.xxxx), u_xlat2);
    u_xlat2 = fma(FGlobals.hlslcc_mtx4x4unity_MatrixV[2].zxyz, float4(u_xlat16_4.zzzz), u_xlat2);
    u_xlat8.xz = u_xlat1.zw * u_xlat2.zw;
    u_xlat1.xy = fma(u_xlat1.xy, u_xlat2.xy, (-u_xlat8.xz));
    u_xlat1.z = (-u_xlat1.y);
    u_xlat8.x = FGlobals._ReflectScale * 0.5;
    u_xlat8.xz = fma(u_xlat1.zx, u_xlat8.xx, FGlobals._ReflectOffset.xyxx.xy);
    u_xlat8.xz = u_xlat8.xz + float2(0.5, 0.5);
    u_xlat16_1.xyz = _Matcap.sample(sampler_Matcap, u_xlat8.xz).xyz;
    u_xlat2.xyz = fma(float3(u_xlat16_1.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat2.xyz = fma(float3(u_xlat16_1.xyz), u_xlat2.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = float3(u_xlat16_1.xyz) * u_xlat2.xyz;
    u_xlat8.x = max(u_xlat1.y, u_xlat1.x);
    u_xlat8.x = max(u_xlat1.z, u_xlat8.x);
    u_xlat8.x = u_xlat8.x * FGlobals._MatcapScale;
    u_xlat24 = dot(FGlobals._WorldSpaceLightPos0.xyz, FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat24 = rsqrt(u_xlat24);
    u_xlat1.xyz = float3(u_xlat24) * float3(FGlobals._WorldSpaceLightPos0.xyz);
    u_xlat24 = dot(float3(u_xlat16_4.xyz), u_xlat1.xyz);
    u_xlat1.x = u_xlat24 + FGlobals._BaseCellOffset;
    u_xlat1.x = u_xlat1.x / FGlobals._BaseCellSharpness;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat0.x = u_xlat1.x * float(UnityInstancing_PerDraw2[u_xlati0 / 7].unity_SHCArray.w);
    u_xlatb1 = FGlobals._LightColor0.w==half(0.0);
    u_xlat0.x = (u_xlatb1) ? 0.0 : u_xlat0.x;
    u_xlat1.x = u_xlat0.x + -1.0;
    u_xlat9.xy = fma(float2(FGlobals._HighLightShadowMask, FGlobals._MetcapShadowMask), u_xlat1.xx, float2(1.0, 1.0));
    u_xlat1.x = fma(FGlobals._RimShadowScale, u_xlat1.x, 1.0);
    u_xlat8.x = u_xlat8.x * u_xlat9.y;
    u_xlat2.xyz = u_xlat8.xxx * FGlobals._MatcapColor.xyz;
    u_xlat17.xy = fma(input.TEXCOORD0.xy, FGlobals._RoughnessMap_ST.xy, FGlobals._RoughnessMap_ST.zw);
    u_xlat16_8.x = _RoughnessMap.sample(sampler_RoughnessMap, u_xlat17.xy).x;
    u_xlat8.x = fma(FGlobals._RoughnessScale, (-float(u_xlat16_8.x)), 1.0);
    u_xlat3.xyz = (-FGlobals._FadeColorF.xyz) + FGlobals._FadeColorB.xyz;
    u_xlat3.xyz = fma(float3(u_xlat16), u_xlat3.xyz, FGlobals._FadeColorF.xyz);
    u_xlat17.xy = fma(input.TEXCOORD0.xy, FGlobals._BaseColor_ST.xy, FGlobals._BaseColor_ST.zw);
    u_xlat16_6.xyz = _BaseColor.sample(sampler_BaseColor, u_xlat17.xy).xyz;
    u_xlat7.xyz = fma(float3(u_xlat16_6.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat7.xyz = fma(float3(u_xlat16_6.xyz), u_xlat7.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat6.xyz = float3(u_xlat16_6.xyz) * u_xlat7.xyz;
    u_xlat7.xyz = fma(FGlobals._BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat7.xyz = fma(FGlobals._BaseTint.xyz, u_xlat7.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat7.xyz = u_xlat7.xyz * FGlobals._BaseTint.xyz;
    u_xlat6.xyz = u_xlat6.xyz * u_xlat7.xyz;
    u_xlat17.xy = fma(input.TEXCOORD0.xy, FGlobals._AOTex_ST.xy, FGlobals._AOTex_ST.zw);
    u_xlat16_16 = _AOTex.sample(sampler_AOTex, u_xlat17.xy).x;
    u_xlat17.x = fma(float(u_xlat16_16), 0.305306017, 0.682171106);
    u_xlat17.x = fma(float(u_xlat16_16), u_xlat17.x, 0.0125228781);
    u_xlat16 = fma(float(u_xlat16_16), u_xlat17.x, -1.0);
    u_xlat16 = fma(FGlobals._AOScale, u_xlat16, 1.0);
    u_xlat6.xyz = float3(u_xlat16) * u_xlat6.xyz;
    u_xlat3.xyz = u_xlat3.xyz * u_xlat6.xyz;
    u_xlat16 = (-u_xlat24) + FGlobals._inverseLightOffset;
    u_xlat24 = u_xlat24 + FGlobals._HighLightOffset;
    u_xlat24 = u_xlat24 / FGlobals._HighLightSmooth;
    u_xlat24 = clamp(u_xlat24, 0.0f, 1.0f);
    u_xlat16 = u_xlat16 / FGlobals._inverseLightSmooth;
    u_xlat16 = clamp(u_xlat16, 0.0f, 1.0f);
    u_xlat6.xyz = fma(u_xlat0.xxx, FGlobals._LightColor.xyz, FGlobals._IndirectColor.xyz);
    u_xlat0.x = (-u_xlat0.x) + 1.0;
    u_xlat7.xyz = u_xlat0.xxx * FGlobals._ShadowAddColor.xyz;
    u_xlat6.xyz = fma(float3(u_xlat16), FGlobals._inverseColor.xyz, u_xlat6.xyz);
    u_xlat3.xyz = fma(u_xlat6.xyz, u_xlat3.xyz, u_xlat7.xyz);
    u_xlat0.x = fma(u_xlat24, u_xlat24, (-u_xlat24));
    u_xlat0.x = fma(FGlobals._HighLightPing, u_xlat0.x, u_xlat24);
    u_xlat0.xzw = u_xlat0.xxx * FGlobals._HighLightColor.xyz;
    u_xlat0.xzw = fma(u_xlat0.xzw, u_xlat9.xxx, u_xlat3.xyz);
    u_xlat0.xyz = fma(u_xlat2.xyz, u_xlat8.xxx, u_xlat0.xzw);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_4.xyz = half3(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2(float3(u_xlat16_4.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = fma(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat2.x = input.TEXCOORD1.w;
    u_xlat2.y = input.TEXCOORD2.w;
    u_xlat2.z = input.TEXCOORD3.w;
    u_xlat9.xyz = (-u_xlat2.xyz) + FGlobals._WorldSpaceCameraPos.xyzx.xyz;
    u_xlat24 = dot(u_xlat9.xyz, u_xlat9.xyz);
    u_xlat24 = rsqrt(u_xlat24);
    u_xlat2.xyz = float3(u_xlat24) * u_xlat9.xyz;
    u_xlat9.xyz = fma((-u_xlat9.xyz), float3(u_xlat24), FGlobals.hlslcc_mtx4x4unity_MatrixInvV[2].xyz);
    u_xlat9.xyz = fma(FGlobals.unity_OrthoParams.www, u_xlat9.xyz, u_xlat2.xyz);
    u_xlat2.x = input.TEXCOORD1.z;
    u_xlat2.y = input.TEXCOORD2.z;
    u_xlat2.z = input.TEXCOORD3.z;
    u_xlat24 = dot(u_xlat2.xyz, u_xlat9.xyz);
    u_xlat24 = u_xlat24 + FGlobals._RimOffset;
    u_xlat24 = clamp(u_xlat24, 0.0f, 1.0f);
    u_xlat24 = (-u_xlat24) + 1.0;
    u_xlat24 = log2(u_xlat24);
    u_xlat24 = u_xlat24 * FGlobals._RimPower;
    u_xlat24 = exp2(u_xlat24);
    u_xlat24 = u_xlat1.x * u_xlat24;
    u_xlat1.xyz = float3(u_xlat24) * FGlobals._RimColor.xyz;
    u_xlat0.xyz = fma(u_xlat1.xyz, float3(FGlobals._RimLightMul), u_xlat0.xyz);
    output.SV_Target0.xyz = half3(u_xlat0.xyz);
    output.SV_Target0.w = half(1.0);
    return output;
}
