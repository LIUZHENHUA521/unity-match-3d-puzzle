// Mechanical port of matching iOS 1.11.03 item Metal; selection overlay is project compatibility.
Shader "Goose/SourceItems/ToonLite1SingleWorld" { Properties {
_ground1("_ground1",Float)=0.0
_BaseColor("_BaseColor",2D)="white"{}
_BaseTint("_BaseTint",Vector)=(1.0,1.0,1.0,0.0)
_NormalMap("_NormalMap",2D)="bump"{}
_NormalScale("_NormalScale",Float)=1.0
_AOTex("_AOTex",2D)="white"{}
_AOScale("_AOScale",Float)=1.0
_ground2("_ground2",Float)=0.0
_LightColor("_LightColor",Vector)=(0.0,0.0,0.0,0.0)
_BaseCellSharpness("_BaseCellSharpness",Float)=0.009999999776482582
_BaseCellOffset("_BaseCellOffset",Float)=0.0
_ShadowAddColor("_ShadowAddColor",Vector)=(0.0,0.0,0.0,0.0)
_IndirectColor("_IndirectColor",Vector)=(0.0,0.0,0.0,0.0)
_ground7("_ground7",Float)=0.0
_HighLightColor("_HighLightColor",Vector)=(0.0,0.0,0.0,0.0)
_HighLightSmooth("_HighLightSmooth",Float)=0.0
_HighLightOffset("_HighLightOffset",Float)=0.0
_HighLightPing("_HighLightPing",Float)=0.0
_HighLightShadowMask("_HighLightShadowMask",Float)=0.0
_ground5("_ground5",Float)=0.0
_IsUseNormalMap("_IsUseNormalMap",Float)=0.0
_RimColor("_RimColor",Vector)=(1.0,1.0,1.0,0.0)
_RimPower("_RimPower",Float)=0.4000000059604645
_RimOffset("_RimOffset",Float)=0.6000000238418579
_RimShadowScale("_RimShadowScale",Float)=0.0
_RimLightMul("_RimLightMul",Float)=1.0
_ground6("_ground6",Float)=0.0
_Matcap("_Matcap",2D)="white"{}
_MatcapScale("_MatcapScale",Float)=0.0
_MatcapColor("_MatcapColor",Vector)=(0.0,0.0,0.0,0.0)
_ReflectScale("_ReflectScale",Float)=1.0
_ReflectOffset("_ReflectOffset",Vector)=(0.0,0.0,0.0,0.0)
_MetcapShadowMask("_MetcapShadowMask",Float)=0.0
_RoughnessMap("_RoughnessMap",2D)="black"{}
_RoughnessScale("_RoughnessScale",Float)=0.0
_IsUseWorldMatcap("_IsUseWorldMatcap",Float)=0.0
_IsSingleColor("_IsSingleColor",Float)=0.0
_ground3("_ground3",Float)=0.0
_inverseLightSmooth("_inverseLightSmooth",Float)=1.0
_inverseColor("_inverseColor",Vector)=(0.0,0.0,0.0,0.0)
_inverseLightOffset("_inverseLightOffset",Float)=0.0
_CustomInvertLightDir("_CustomInvertLightDir",Float)=0.0
_CustomInverseDir("_CustomInverseDir",Vector)=(1.0,0.0,0.0,0.0)
_ground4("_ground4",Float)=0.0
_FadeColorDirY("_FadeColorDirY",Float)=0.0
_FadeColorF("_FadeColorF",Vector)=(1.0,1.0,1.0,1.0)
_FadeColorB("_FadeColorB",Vector)=(1.0,1.0,1.0,1.0)
_FadeColorOffset("_FadeColorOffset",Float)=0.0
_FadeColorScale("_FadeColorScale",Float)=0.0
_texcoord("_texcoord",2D)="white"{}
__dirty("__dirty",Float)=1.0
_SelectionGlow("Project selection glow",Float)=0
_SelectionPulse("Project selection pulse",Float)=1
_OutlinePixels("Project selection outline",Float)=1.2
}
SubShader { Tags {"RenderType"="Opaque" "Queue"="Geometry"}
UsePass "Goose/Original Item/SELECTION_OUTLINE"
Pass { Name "FORWARD_BASE" Tags {"LightMode"="ForwardBase"}
Cull Back ZWrite On ZTest LEqual Blend One Zero ColorMask RGBA
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
float4 _texcoord_ST;
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
sampler2D _NormalMap;
sampler2D _BaseColor;
sampler2D _AOTex;
sampler2D _Matcap;
sampler2D _RoughnessMap;
float _SelectionGlow, _SelectionPulse;
struct appdata {float4 POSITION0:POSITION;float4 TANGENT0:TANGENT;float3 NORMAL0:NORMAL;float4 TEXCOORD0:TEXCOORD0;};
struct v2f {float4 mtl_Position:SV_POSITION;float2 TEXCOORD0:TEXCOORD0;float4 TEXCOORD1:TEXCOORD1;float4 TEXCOORD2:TEXCOORD2;float4 TEXCOORD3:TEXCOORD3;float4 TEXCOORD6:TEXCOORD6;float4 TEXCOORD7:TEXCOORD7; SHADOW_COORDS(8) };
v2f vert(appdata input){
    v2f output;
    float4 u_xlat0;
    float4 u_xlat1;
    float4 u_xlat2;
    float3 u_xlat3;
    u_xlat0 = input.POSITION0.yyyy * transpose(unity_ObjectToWorld)[1];
    u_xlat0 = mad(transpose(unity_ObjectToWorld)[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = mad(transpose(unity_ObjectToWorld)[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat1 = u_xlat0 + transpose(unity_ObjectToWorld)[3];
    u_xlat0.xyz = mad(transpose(unity_ObjectToWorld)[3].xyz, input.POSITION0.www, u_xlat0.xyz);
    u_xlat2 = u_xlat1.yyyy * transpose(UNITY_MATRIX_VP)[1];
    u_xlat2 = mad(transpose(UNITY_MATRIX_VP)[0], u_xlat1.xxxx, u_xlat2);
    u_xlat2 = mad(transpose(UNITY_MATRIX_VP)[2], u_xlat1.zzzz, u_xlat2);
    output.mtl_Position = mad(transpose(UNITY_MATRIX_VP)[3], u_xlat1.wwww, u_xlat2);
    output.TEXCOORD0.xy = mad(input.TEXCOORD0.xy, _texcoord_ST.xy, _texcoord_ST.zw);
    output.TEXCOORD1.w = u_xlat0.x;
    u_xlat1.y = dot(input.NORMAL0.xyz, transpose(unity_WorldToObject)[0].xyz);
    u_xlat1.z = dot(input.NORMAL0.xyz, transpose(unity_WorldToObject)[1].xyz);
    u_xlat1.x = dot(input.NORMAL0.xyz, transpose(unity_WorldToObject)[2].xyz);
    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat0.x = rsqrt(u_xlat0.x);
    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz;
    u_xlat2.xyz = input.TANGENT0.yyy * transpose(unity_ObjectToWorld)[1].yzx;
    u_xlat2.xyz = mad(transpose(unity_ObjectToWorld)[0].yzx, input.TANGENT0.xxx, u_xlat2.xyz);
    u_xlat2.xyz = mad(transpose(unity_ObjectToWorld)[2].yzx, input.TANGENT0.zzz, u_xlat2.xyz);
    u_xlat0.x = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat0.x = rsqrt(u_xlat0.x);
    u_xlat2.xyz = u_xlat0.xxx * u_xlat2.xyz;
    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.xyz;
    u_xlat3.xyz = mad(u_xlat1.zxy, u_xlat2.yzx, (-u_xlat3.xyz));
    u_xlat0.x = input.TANGENT0.w * unity_WorldTransformParams.w;
    u_xlat3.xyz = u_xlat0.xxx * u_xlat3.xyz;
    output.TEXCOORD1.y = u_xlat3.x;
    output.TEXCOORD1.x = u_xlat2.z;
    output.TEXCOORD1.z = u_xlat1.y;
    output.TEXCOORD2.x = u_xlat2.x;
    output.TEXCOORD3.x = u_xlat2.y;
    output.TEXCOORD2.z = u_xlat1.z;
    output.TEXCOORD3.z = u_xlat1.x;
    output.TEXCOORD2.w = u_xlat0.y;
    output.TEXCOORD3.w = u_xlat0.z;
    output.TEXCOORD2.y = u_xlat3.y;
    output.TEXCOORD3.y = u_xlat3.z;
    output.TEXCOORD6 = float4(0.0, 0.0, 0.0, 0.0);
    output.TEXCOORD7 = float4(0.0, 0.0, 0.0, 0.0);
    
#if defined(SHADOWS_SCREEN)
#if defined(UNITY_NO_SCREENSPACE_SHADOWS)
output._ShadowCoord=mul(unity_WorldToShadow[0],mul(unity_ObjectToWorld,input.POSITION0));
#else
output._ShadowCoord=ComputeScreenPos(output.mtl_Position);
#endif
#endif
return output;
}
float4 frag(v2f input):SV_Target {UNITY_LIGHT_ATTENUATION(sourceAttenuation,input,float3(input.TEXCOORD1.w,input.TEXCOORD2.w,input.TEXCOORD3.w));

    float4 result;
    float4 u_xlat0;
    half3 u_xlat16_0;
    float4 u_xlat1;
    half u_xlat16_1;
    float4 u_xlat2;
    float3 u_xlat3;
    half3 u_xlat16_3;
    half3 u_xlat16_4;
    half3 u_xlat16_5;
    float3 u_xlat6;
    float3 u_xlat7;
    float3 u_xlat8;
    half2 u_xlat16_8;
    float2 u_xlat14;
    half u_xlat16_18;
    float u_xlat21;
    bool u_xlatb21;
    float u_xlat22;
    float u_xlat23;
    half u_xlat16_25;
    u_xlat0 = transpose(unity_ObjectToWorld)[2].yyyy * transpose(UNITY_MATRIX_V)[1].yzzx;
    u_xlat0 = mad(transpose(UNITY_MATRIX_V)[0].yzzx, transpose(unity_ObjectToWorld)[2].xxxx, u_xlat0);
    u_xlat0 = mad(transpose(UNITY_MATRIX_V)[2].yzzx, transpose(unity_ObjectToWorld)[2].zzzz, u_xlat0);
    u_xlat0 = mad(transpose(UNITY_MATRIX_V)[3].yzzx, transpose(unity_ObjectToWorld)[2].wwww, u_xlat0);
    u_xlat1 = transpose(unity_ObjectToWorld)[0].yyyy * transpose(UNITY_MATRIX_V)[1].yzzx;
    u_xlat1 = mad(transpose(UNITY_MATRIX_V)[0].yzzx, transpose(unity_ObjectToWorld)[0].xxxx, u_xlat1);
    u_xlat1 = mad(transpose(UNITY_MATRIX_V)[2].yzzx, transpose(unity_ObjectToWorld)[0].zzzz, u_xlat1);
    u_xlat1 = mad(transpose(UNITY_MATRIX_V)[3].yzzx, transpose(unity_ObjectToWorld)[0].wwww, u_xlat1);
    u_xlat2 = transpose(unity_ObjectToWorld)[1].yyyy * transpose(UNITY_MATRIX_V)[1].yzzx;
    u_xlat2 = mad(transpose(UNITY_MATRIX_V)[0].yzzx, transpose(unity_ObjectToWorld)[1].xxxx, u_xlat2);
    u_xlat2 = mad(transpose(UNITY_MATRIX_V)[2].yzzx, transpose(unity_ObjectToWorld)[1].zzzz, u_xlat2);
    u_xlat2 = mad(transpose(UNITY_MATRIX_V)[3].yzzx, transpose(unity_ObjectToWorld)[1].wwww, u_xlat2);
    u_xlat3.xyz = input.TEXCOORD2.www * transpose(unity_WorldToObject)[1].xyz;
    u_xlat3.xyz = mad(transpose(unity_WorldToObject)[0].xyz, input.TEXCOORD1.www, u_xlat3.xyz);
    u_xlat3.xyz = mad(transpose(unity_WorldToObject)[2].xyz, input.TEXCOORD3.www, u_xlat3.xyz);
    u_xlat3.xyz = u_xlat3.xyz + transpose(unity_WorldToObject)[3].xyz;
    u_xlat2 = u_xlat2 * u_xlat3.yyyy;
    u_xlat1 = mad(u_xlat1, u_xlat3.xxxx, u_xlat2);
    u_xlat0 = mad(u_xlat0, u_xlat3.zzzz, u_xlat1);
    u_xlat1.x = u_xlat3.y * _FadeColorScale;
    u_xlat1.x = mad(u_xlat1.x, 0.5, _FadeColorOffset);
    u_xlat1.x = u_xlat1.x + 0.5;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat2 = transpose(unity_ObjectToWorld)[3].yyyy * transpose(UNITY_MATRIX_V)[1].yzzx;
    u_xlat2 = mad(transpose(UNITY_MATRIX_V)[0].yzzx, transpose(unity_ObjectToWorld)[3].xxxx, u_xlat2);
    u_xlat2 = mad(transpose(UNITY_MATRIX_V)[2].yzzx, transpose(unity_ObjectToWorld)[3].zzzz, u_xlat2);
    u_xlat2 = mad(transpose(UNITY_MATRIX_V)[3].yzzx, transpose(unity_ObjectToWorld)[3].wwww, u_xlat2);
    u_xlat0 = u_xlat0 + u_xlat2;
    u_xlat8.x = dot(u_xlat0.xzw, u_xlat0.xzw);
    u_xlat8.x = max(u_xlat8.x, 0.00100000005);
    u_xlat8.x = rsqrt(u_xlat8.x);
    u_xlat0 = u_xlat0 * u_xlat8.xxxx;
    u_xlat8.xy = mad(input.TEXCOORD0.xy, _NormalMap_ST.xy, _NormalMap_ST.zw);
    u_xlat16_8.xy = tex2D(_NormalMap, u_xlat8.xy).yw;
    u_xlat16_4.xy = mad(u_xlat16_8.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_18 = dot(u_xlat16_4.xy, u_xlat16_4.xy);
    u_xlat16_5.xy = (half2)((float2)(u_xlat16_4.xy) * (float2)(_NormalScale));
    u_xlat16_4.x = min(u_xlat16_18, half(1.0));
    u_xlat16_4.x = (-u_xlat16_4.x) + half(1.0);
    u_xlat16_5.z = sqrt(u_xlat16_4.x);
    u_xlat16_4.x = dot(input.TEXCOORD1.xyz, (float3)(u_xlat16_5.xyz));
    u_xlat16_4.y = dot(input.TEXCOORD2.xyz, (float3)(u_xlat16_5.xyz));
    u_xlat16_4.z = dot(input.TEXCOORD3.xyz, (float3)(u_xlat16_5.xyz));
    u_xlat16_25 = dot(u_xlat16_4.xyz, u_xlat16_4.xyz);
    u_xlat16_25 = rsqrt(u_xlat16_25);
    u_xlat16_4.xyz = (half3)(u_xlat16_25) * u_xlat16_4.xyz;
    u_xlat2 = (float4)(u_xlat16_4.yyyy) * transpose(UNITY_MATRIX_V)[1].zxyz;
    u_xlat2 = mad(transpose(UNITY_MATRIX_V)[0].zxyz, (float4)(u_xlat16_4.xxxx), u_xlat2);
    u_xlat2 = mad(transpose(UNITY_MATRIX_V)[2].zxyz, (float4)(u_xlat16_4.zzzz), u_xlat2);
    u_xlat14.xy = u_xlat0.zw * u_xlat2.zw;
    u_xlat0.xy = mad(u_xlat0.xy, u_xlat2.xy, (-u_xlat14.xy));
    u_xlat0.z = (-u_xlat0.y);
    u_xlat7.x = _ReflectScale * 0.5;
    u_xlat0.xy = mad(u_xlat0.zx, u_xlat7.xx, _ReflectOffset.xyxx.xy);
    u_xlat0.xy = u_xlat0.xy + float2(0.5, 0.5);
    u_xlat16_0.xyz = tex2D(_Matcap, u_xlat0.xy).xyz;
    u_xlat8.xyz = mad((float3)(u_xlat16_0.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat8.xyz = mad((float3)(u_xlat16_0.xyz), u_xlat8.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat0.xyz = (float3)(u_xlat16_0.xyz) * u_xlat8.xyz;
    u_xlat0.x = max(u_xlat0.y, u_xlat0.x);
    u_xlat0.x = max(u_xlat0.z, u_xlat0.x);
    u_xlat0.x = u_xlat0.x * _MatcapScale;
    u_xlat7.x = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
    u_xlat7.x = rsqrt(u_xlat7.x);
    u_xlat7.xyz = u_xlat7.xxx * (float3)(_WorldSpaceLightPos0.xyz);
    u_xlat7.x = dot((float3)(u_xlat16_4.xyz), u_xlat7.xyz);
    u_xlat14.x = u_xlat7.x + _BaseCellOffset;
    u_xlat14.x = u_xlat14.x / _BaseCellSharpness;
    u_xlat14.x = clamp(u_xlat14.x, 0.0f, 1.0f);
    u_xlatb21 = _LightColor0.w==half(0.0);
    u_xlat14.x = (u_xlatb21) ? 0.0 : u_xlat14.x;
u_xlat14.x *= sourceAttenuation;
    u_xlat21 = u_xlat14.x + -1.0;
    u_xlat8.xy = mad(float2(_HighLightShadowMask, _MetcapShadowMask), (float2)(u_xlat21), float2(1.0, 1.0));
    u_xlat21 = mad(_RimShadowScale, u_xlat21, 1.0);
    u_xlat0.x = u_xlat0.x * u_xlat8.y;
    u_xlat2.xyz = u_xlat0.xxx * _MatcapColor.xyz;
    u_xlat3.xyz = (-_FadeColorF.xyz) + _FadeColorB.xyz;
    u_xlat1.xzw = mad(u_xlat1.xxx, u_xlat3.xyz, _FadeColorF.xyz);
    u_xlat3.xy = mad(input.TEXCOORD0.xy, _BaseColor_ST.xy, _BaseColor_ST.zw);
    u_xlat16_3.xyz = tex2D(_BaseColor, u_xlat3.xy).xyz;
    u_xlat6.xyz = mad((float3)(u_xlat16_3.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat6.xyz = mad((float3)(u_xlat16_3.xyz), u_xlat6.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat3.xyz = (float3)(u_xlat16_3.xyz) * u_xlat6.xyz;
    u_xlat6.xyz = mad(_BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat6.xyz = mad(_BaseTint.xyz, u_xlat6.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat6.xyz = u_xlat6.xyz * _BaseTint.xyz;
    u_xlat3.xyz = u_xlat3.xyz * u_xlat6.xyz;
    u_xlat6.xy = mad(input.TEXCOORD0.xy, _AOTex_ST.xy, _AOTex_ST.zw);
    u_xlat16_0.x = tex2D(_AOTex, u_xlat6.xy).x;
    u_xlat23 = mad(float(u_xlat16_0.x), 0.305306017, 0.682171106);
    u_xlat23 = mad(float(u_xlat16_0.x), u_xlat23, 0.0125228781);
    u_xlat0.x = mad(float(u_xlat16_0.x), u_xlat23, -1.0);
    u_xlat0.x = mad(_AOScale, u_xlat0.x, 1.0);
    u_xlat3.xyz = u_xlat0.xxx * u_xlat3.xyz;
    u_xlat1.xzw = u_xlat1.xzw * u_xlat3.xyz;
    u_xlat0.x = (-u_xlat7.x) + _inverseLightOffset;
    u_xlat7.x = u_xlat7.x + _HighLightOffset;
    u_xlat7.x = u_xlat7.x / _HighLightSmooth;
    u_xlat7.x = clamp(u_xlat7.x, 0.0f, 1.0f);
    u_xlat0.x = u_xlat0.x / _inverseLightSmooth;
    u_xlat0.x = clamp(u_xlat0.x, 0.0f, 1.0f);
    u_xlat3.xyz = mad(u_xlat14.xxx, _LightColor.xyz, _IndirectColor.xyz);
    u_xlat14.x = (-u_xlat14.x) + 1.0;
    u_xlat6.xyz = u_xlat14.xxx * _ShadowAddColor.xyz;
    u_xlat3.xyz = mad(u_xlat0.xxx, _inverseColor.xyz, u_xlat3.xyz);
    u_xlat1.xzw = mad(u_xlat3.xyz, u_xlat1.xzw, u_xlat6.xyz);
    u_xlat0.x = mad(u_xlat7.x, u_xlat7.x, (-u_xlat7.x));
    u_xlat0.x = mad(_HighLightPing, u_xlat0.x, u_xlat7.x);
    u_xlat0.xyz = u_xlat0.xxx * _HighLightColor.xyz;
    u_xlat0.xyz = mad(u_xlat0.xyz, u_xlat8.xxx, u_xlat1.xzw);
    u_xlat1.xy = mad(input.TEXCOORD0.xy, _RoughnessMap_ST.xy, _RoughnessMap_ST.zw);
    u_xlat16_1 = tex2D(_RoughnessMap, u_xlat1.xy).x;
    u_xlat1.x = mad(_RoughnessScale, (-float(u_xlat16_1)), 1.0);
    u_xlat0.xyz = mad(u_xlat2.xyz, u_xlat1.xxx, u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_4.xyz = (half3)(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2((float3)(u_xlat16_4.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = mad(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat1.x = input.TEXCOORD1.w;
    u_xlat1.y = input.TEXCOORD2.w;
    u_xlat1.z = input.TEXCOORD3.w;
    u_xlat1.xyz = (-u_xlat1.xyz) + _WorldSpaceCameraPos.xyzx.xyz;
    u_xlat22 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat22 = rsqrt(u_xlat22);
    u_xlat2.xyz = (float3)(u_xlat22) * u_xlat1.xyz;
    u_xlat1.xyz = mad((-u_xlat1.xyz), (float3)(u_xlat22), transpose(UNITY_MATRIX_I_V)[2].xyz);
    u_xlat1.xyz = mad(unity_OrthoParams.www, u_xlat1.xyz, u_xlat2.xyz);
    u_xlat2.x = input.TEXCOORD1.z;
    u_xlat2.y = input.TEXCOORD2.z;
    u_xlat2.z = input.TEXCOORD3.z;
    u_xlat1.x = dot(u_xlat2.xyz, u_xlat1.xyz);
    u_xlat1.x = u_xlat1.x + _RimOffset;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat1.x = (-u_xlat1.x) + 1.0;
    u_xlat1.x = log2(u_xlat1.x);
    u_xlat1.x = u_xlat1.x * _RimPower;
    u_xlat1.x = exp2(u_xlat1.x);
    u_xlat21 = u_xlat21 * u_xlat1.x;
    u_xlat1.xyz = (float3)(u_xlat21) * _RimColor.xyz;
    u_xlat0.xyz = mad(u_xlat1.xyz, (float3)(_RimLightMul), u_xlat0.xyz);
    result.xyz = (half3)(u_xlat0.xyz);
    result.w = half(1.0);
    // Preserve the project's subtle press feedback; separate from source material math.
 float3 sn=normalize(float3(input.TEXCOORD1.z,input.TEXCOORD2.z,input.TEXCOORD3.z));
 float3 sv=normalize(_WorldSpaceCameraPos-float3(input.TEXCOORD1.w,input.TEXCOORD2.w,input.TEXCOORD3.w));
 float sr=pow(1-saturate(dot(sn,sv)),2);
 result.rgb+=_SelectionGlow*lerp(.35,1,_SelectionPulse)*float3(1,.93,.78)*(.025+sr*.12);
 return result;
}
ENDCG
}
Pass { Name "FORWARD_ADD" Tags {"LightMode"="ForwardAdd"}
Cull Back ZWrite Off ZTest LEqual Blend One One ColorMask RGBA
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile DIRECTIONAL
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
float4 _texcoord_ST;
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
sampler2D _NormalMap;
sampler2D _BaseColor;
sampler2D _AOTex;
sampler2D _Matcap;
sampler2D _RoughnessMap;
struct appdata {float4 POSITION0:POSITION;float4 TANGENT0:TANGENT;float3 NORMAL0:NORMAL;float4 TEXCOORD0:TEXCOORD0;};
struct v2f {float4 mtl_Position:SV_POSITION;float2 TEXCOORD0:TEXCOORD0;float3 TEXCOORD1:TEXCOORD1;float3 TEXCOORD2:TEXCOORD2;float3 TEXCOORD3:TEXCOORD3;float3 TEXCOORD4:TEXCOORD4;float4 TEXCOORD6:TEXCOORD6;};
v2f vert(appdata input){
    v2f output;
    float4 u_xlat0;
    float4 u_xlat1;
    float3 u_xlat2;
    float u_xlat9;
    u_xlat0 = input.POSITION0.yyyy * transpose(unity_ObjectToWorld)[1];
    u_xlat0 = mad(transpose(unity_ObjectToWorld)[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = mad(transpose(unity_ObjectToWorld)[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat1 = u_xlat0 + transpose(unity_ObjectToWorld)[3];
    output.TEXCOORD4.xyz = mad(transpose(unity_ObjectToWorld)[3].xyz, input.POSITION0.www, u_xlat0.xyz);
    u_xlat0 = u_xlat1.yyyy * transpose(UNITY_MATRIX_VP)[1];
    u_xlat0 = mad(transpose(UNITY_MATRIX_VP)[0], u_xlat1.xxxx, u_xlat0);
    u_xlat0 = mad(transpose(UNITY_MATRIX_VP)[2], u_xlat1.zzzz, u_xlat0);
    output.mtl_Position = mad(transpose(UNITY_MATRIX_VP)[3], u_xlat1.wwww, u_xlat0);
    output.TEXCOORD0.xy = mad(input.TEXCOORD0.xy, _texcoord_ST.xy, _texcoord_ST.zw);
    u_xlat0.y = dot(input.NORMAL0.xyz, transpose(unity_WorldToObject)[0].xyz);
    u_xlat0.z = dot(input.NORMAL0.xyz, transpose(unity_WorldToObject)[1].xyz);
    u_xlat0.x = dot(input.NORMAL0.xyz, transpose(unity_WorldToObject)[2].xyz);
    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat9 = rsqrt(u_xlat9);
    u_xlat0.xyz = (float3)(u_xlat9) * u_xlat0.xyz;
    u_xlat1.xyz = input.TANGENT0.yyy * transpose(unity_ObjectToWorld)[1].yzx;
    u_xlat1.xyz = mad(transpose(unity_ObjectToWorld)[0].yzx, input.TANGENT0.xxx, u_xlat1.xyz);
    u_xlat1.xyz = mad(transpose(unity_ObjectToWorld)[2].yzx, input.TANGENT0.zzz, u_xlat1.xyz);
    u_xlat9 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat9 = rsqrt(u_xlat9);
    u_xlat1.xyz = (float3)(u_xlat9) * u_xlat1.xyz;
    u_xlat2.xyz = u_xlat0.xyz * u_xlat1.xyz;
    u_xlat2.xyz = mad(u_xlat0.zxy, u_xlat1.yzx, (-u_xlat2.xyz));
    u_xlat9 = input.TANGENT0.w * unity_WorldTransformParams.w;
    u_xlat2.xyz = (float3)(u_xlat9) * u_xlat2.xyz;
    output.TEXCOORD1.y = u_xlat2.x;
    output.TEXCOORD1.x = u_xlat1.z;
    output.TEXCOORD1.z = u_xlat0.y;
    output.TEXCOORD2.x = u_xlat1.x;
    output.TEXCOORD3.x = u_xlat1.y;
    output.TEXCOORD2.z = u_xlat0.z;
    output.TEXCOORD3.z = u_xlat0.x;
    output.TEXCOORD2.y = u_xlat2.y;
    output.TEXCOORD3.y = u_xlat2.z;
    output.TEXCOORD6 = float4(0.0, 0.0, 0.0, 0.0);
    return output;
}
float4 frag(v2f input):SV_Target {
    float4 result;
    float3 u_xlat0;
    half3 u_xlat16_0;
    float4 u_xlat1;
    half3 u_xlat16_1;
    float4 u_xlat2;
    float3 u_xlat3;
    half2 u_xlat16_3;
    float4 u_xlat4;
    half3 u_xlat16_4;
    float4 u_xlat5;
    half3 u_xlat16_5;
    half3 u_xlat16_6;
    float u_xlat8;
    float3 u_xlat9;
    float2 u_xlat15;
    float u_xlat16;
    half u_xlat16_18;
    float u_xlat21;
    half u_xlat16_21;
    float u_xlat22;
    half u_xlat16_22;
    half u_xlat16_25;
    u_xlat0.xy = mad(input.TEXCOORD0.xy, _BaseColor_ST.xy, _BaseColor_ST.zw);
    u_xlat16_0.xyz = tex2D(_BaseColor, u_xlat0.xy).xyz;
    u_xlat1.xyz = mad((float3)(u_xlat16_0.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat1.xyz = mad((float3)(u_xlat16_0.xyz), u_xlat1.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat0.xyz = (float3)(u_xlat16_0.xyz) * u_xlat1.xyz;
    u_xlat1.xyz = mad(_BaseTint.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat1.xyz = mad(_BaseTint.xyz, u_xlat1.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = u_xlat1.xyz * _BaseTint.xyz;
    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xyz;
    u_xlat1.xy = mad(input.TEXCOORD0.xy, _AOTex_ST.xy, _AOTex_ST.zw);
    u_xlat16_21 = tex2D(_AOTex, u_xlat1.xy).x;
    u_xlat1.x = mad(float(u_xlat16_21), 0.305306017, 0.682171106);
    u_xlat1.x = mad(float(u_xlat16_21), u_xlat1.x, 0.0125228781);
    u_xlat21 = mad(float(u_xlat16_21), u_xlat1.x, -1.0);
    u_xlat21 = mad(_AOScale, u_xlat21, 1.0);
    u_xlat0.xyz = (float3)(u_xlat21) * u_xlat0.xyz;
    u_xlat1.xyz = input.TEXCOORD4.yyy * transpose(unity_WorldToObject)[1].xyz;
    u_xlat1.xyz = mad(transpose(unity_WorldToObject)[0].xyz, input.TEXCOORD4.xxx, u_xlat1.xyz);
    u_xlat1.xyz = mad(transpose(unity_WorldToObject)[2].xyz, input.TEXCOORD4.zzz, u_xlat1.xyz);
    u_xlat1.xyz = u_xlat1.xyz + transpose(unity_WorldToObject)[3].xyz;
    u_xlat21 = u_xlat1.y * _FadeColorScale;
    u_xlat21 = mad(u_xlat21, 0.5, _FadeColorOffset);
    u_xlat21 = u_xlat21 + 0.5;
    u_xlat21 = clamp(u_xlat21, 0.0f, 1.0f);
    u_xlat2.xyz = (-_FadeColorF.xyz) + _FadeColorB.xyz;
    u_xlat2.xyz = mad((float3)(u_xlat21), u_xlat2.xyz, _FadeColorF.xyz);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat2.xyz;
    u_xlat21 = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
    u_xlat21 = rsqrt(u_xlat21);
    u_xlat2.xyz = (float3)(u_xlat21) * (float3)(_WorldSpaceLightPos0.xyz);
    u_xlat3.xy = mad(input.TEXCOORD0.xy, _NormalMap_ST.xy, _NormalMap_ST.zw);
    u_xlat16_3.xy = tex2D(_NormalMap, u_xlat3.xy).yw;
    u_xlat16_4.xy = mad(u_xlat16_3.yx, half2(2.0, 2.0), half2(-1.0, -1.0));
    u_xlat16_18 = dot(u_xlat16_4.xy, u_xlat16_4.xy);
    u_xlat16_5.xy = (half2)((float2)(u_xlat16_4.xy) * (float2)(_NormalScale));
    u_xlat16_4.x = min(u_xlat16_18, half(1.0));
    u_xlat16_4.x = (-u_xlat16_4.x) + half(1.0);
    u_xlat16_5.z = sqrt(u_xlat16_4.x);
    u_xlat16_4.x = dot(input.TEXCOORD1.xyz, (float3)(u_xlat16_5.xyz));
    u_xlat16_4.y = dot(input.TEXCOORD2.xyz, (float3)(u_xlat16_5.xyz));
    u_xlat16_4.z = dot(input.TEXCOORD3.xyz, (float3)(u_xlat16_5.xyz));
    u_xlat16_25 = dot(u_xlat16_4.xyz, u_xlat16_4.xyz);
    u_xlat16_25 = rsqrt(u_xlat16_25);
    u_xlat16_4.xyz = (half3)(u_xlat16_25) * u_xlat16_4.xyz;
    u_xlat21 = dot((float3)(u_xlat16_4.xyz), u_xlat2.xyz);
    u_xlat22 = (-u_xlat21) + _inverseLightOffset;
    u_xlat22 = u_xlat22 / _inverseLightSmooth;
    u_xlat22 = clamp(u_xlat22, 0.0f, 1.0f);
    u_xlat2.x = u_xlat21 + _BaseCellOffset;
    u_xlat21 = u_xlat21 + _HighLightOffset;
    u_xlat21 = u_xlat21 / _HighLightSmooth;
    u_xlat21 = clamp(u_xlat21, 0.0f, 1.0f);
    u_xlat2.x = u_xlat2.x / _BaseCellSharpness;
    u_xlat2.x = clamp(u_xlat2.x, 0.0f, 1.0f);
    u_xlat16_5.xyz = (half3)((float3)(_LightColor0.xyz) + float3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
    u_xlat16_5.xyz = _LightColor0.xyz / u_xlat16_5.xyz;
    u_xlat9.x = max(float(u_xlat16_5.y), float(u_xlat16_5.x));
    u_xlat9.x = max(float(u_xlat16_5.z), u_xlat9.x);
    u_xlat16 = u_xlat9.x * u_xlat2.x;
    u_xlat3.xyz = mad((float3)(u_xlat16), _LightColor.xyz, _IndirectColor.xyz);
    u_xlat3.xyz = mad((float3)(u_xlat22), _inverseColor.xyz, u_xlat3.xyz);
    u_xlat22 = mad((-u_xlat2.x), u_xlat9.x, 1.0);
    u_xlat2.x = mad(u_xlat2.x, u_xlat9.x, -1.0);
    u_xlat9.xyz = (float3)(u_xlat22) * _ShadowAddColor.xyz;
    u_xlat0.xyz = mad(u_xlat3.xyz, u_xlat0.xyz, u_xlat9.xyz);
    u_xlat22 = mad(u_xlat21, u_xlat21, (-u_xlat21));
    u_xlat21 = mad(_HighLightPing, u_xlat22, u_xlat21);
    u_xlat9.xyz = (float3)(u_xlat21) * _HighLightColor.xyz;
    u_xlat3.xy = mad(float2(_HighLightShadowMask, _MetcapShadowMask), u_xlat2.xx, float2(1.0, 1.0));
    u_xlat21 = mad(_RimShadowScale, u_xlat2.x, 1.0);
    u_xlat0.xyz = mad(u_xlat9.xyz, u_xlat3.xxx, u_xlat0.xyz);
    u_xlat2 = (float4)(u_xlat16_4.yyyy) * transpose(UNITY_MATRIX_V)[1].zxyz;
    u_xlat2 = mad(transpose(UNITY_MATRIX_V)[0].zxyz, (float4)(u_xlat16_4.xxxx), u_xlat2);
    u_xlat2 = mad(transpose(UNITY_MATRIX_V)[2].zxyz, (float4)(u_xlat16_4.zzzz), u_xlat2);
    u_xlat4 = transpose(unity_ObjectToWorld)[0].yyyy * transpose(UNITY_MATRIX_V)[1].yzzx;
    u_xlat4 = mad(transpose(UNITY_MATRIX_V)[0].yzzx, transpose(unity_ObjectToWorld)[0].xxxx, u_xlat4);
    u_xlat4 = mad(transpose(UNITY_MATRIX_V)[2].yzzx, transpose(unity_ObjectToWorld)[0].zzzz, u_xlat4);
    u_xlat4 = mad(transpose(UNITY_MATRIX_V)[3].yzzx, transpose(unity_ObjectToWorld)[0].wwww, u_xlat4);
    u_xlat5 = transpose(unity_ObjectToWorld)[1].yyyy * transpose(UNITY_MATRIX_V)[1].yzzx;
    u_xlat5 = mad(transpose(UNITY_MATRIX_V)[0].yzzx, transpose(unity_ObjectToWorld)[1].xxxx, u_xlat5);
    u_xlat5 = mad(transpose(UNITY_MATRIX_V)[2].yzzx, transpose(unity_ObjectToWorld)[1].zzzz, u_xlat5);
    u_xlat5 = mad(transpose(UNITY_MATRIX_V)[3].yzzx, transpose(unity_ObjectToWorld)[1].wwww, u_xlat5);
    u_xlat5 = u_xlat1.yyyy * u_xlat5;
    u_xlat4 = mad(u_xlat4, u_xlat1.xxxx, u_xlat5);
    u_xlat5 = transpose(unity_ObjectToWorld)[2].yyyy * transpose(UNITY_MATRIX_V)[1].yzzx;
    u_xlat5 = mad(transpose(UNITY_MATRIX_V)[0].yzzx, transpose(unity_ObjectToWorld)[2].xxxx, u_xlat5);
    u_xlat5 = mad(transpose(UNITY_MATRIX_V)[2].yzzx, transpose(unity_ObjectToWorld)[2].zzzz, u_xlat5);
    u_xlat5 = mad(transpose(UNITY_MATRIX_V)[3].yzzx, transpose(unity_ObjectToWorld)[2].wwww, u_xlat5);
    u_xlat1 = mad(u_xlat5, u_xlat1.zzzz, u_xlat4);
    u_xlat4 = transpose(unity_ObjectToWorld)[3].yyyy * transpose(UNITY_MATRIX_V)[1].yzzx;
    u_xlat4 = mad(transpose(UNITY_MATRIX_V)[0].yzzx, transpose(unity_ObjectToWorld)[3].xxxx, u_xlat4);
    u_xlat4 = mad(transpose(UNITY_MATRIX_V)[2].yzzx, transpose(unity_ObjectToWorld)[3].zzzz, u_xlat4);
    u_xlat4 = mad(transpose(UNITY_MATRIX_V)[3].yzzx, transpose(unity_ObjectToWorld)[3].wwww, u_xlat4);
    u_xlat1 = u_xlat1 + u_xlat4;
    u_xlat3.x = dot(u_xlat1.xzw, u_xlat1.xzw);
    u_xlat3.x = max(u_xlat3.x, 0.00100000005);
    u_xlat3.x = rsqrt(u_xlat3.x);
    u_xlat1 = u_xlat1 * u_xlat3.xxxx;
    u_xlat15.xy = u_xlat2.zw * u_xlat1.zw;
    u_xlat1.xy = mad(u_xlat1.xy, u_xlat2.xy, (-u_xlat15.xy));
    u_xlat1.z = (-u_xlat1.y);
    u_xlat8 = _ReflectScale * 0.5;
    u_xlat1.xy = mad(u_xlat1.zx, (float2)(u_xlat8), _ReflectOffset.xyxx.xy);
    u_xlat1.xy = u_xlat1.xy + float2(0.5, 0.5);
    u_xlat16_1.xyz = tex2D(_Matcap, u_xlat1.xy).xyz;
    u_xlat2.xyz = mad((float3)(u_xlat16_1.xyz), float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat2.xyz = mad((float3)(u_xlat16_1.xyz), u_xlat2.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = (float3)(u_xlat16_1.xyz) * u_xlat2.xyz;
    u_xlat1.x = max(u_xlat1.y, u_xlat1.x);
    u_xlat1.x = max(u_xlat1.z, u_xlat1.x);
    u_xlat1.x = u_xlat1.x * _MatcapScale;
    u_xlat1.x = u_xlat1.x * u_xlat3.y;
    u_xlat1.xyz = u_xlat1.xxx * _MatcapColor.xyz;
    u_xlat2.xy = mad(input.TEXCOORD0.xy, _RoughnessMap_ST.xy, _RoughnessMap_ST.zw);
    u_xlat16_22 = tex2D(_RoughnessMap, u_xlat2.xy).x;
    u_xlat22 = mad(_RoughnessScale, (-float(u_xlat16_22)), 1.0);
    u_xlat0.xyz = mad(u_xlat1.xyz, (float3)(u_xlat22), u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_6.xyz = (half3)(max(u_xlat0.xyz, float3(0.0, 0.0, 0.0)));
    u_xlat0.xyz = log2((float3)(u_xlat16_6.xyz));
    u_xlat0.xyz = u_xlat0.xyz * float3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = mad(u_xlat0.xyz, float3(1.05499995, 1.05499995, 1.05499995), float3(-0.0549999997, -0.0549999997, -0.0549999997));
    u_xlat0.xyz = max(u_xlat0.xyz, float3(0.0, 0.0, 0.0));
    u_xlat1.xyz = (-input.TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyzx.xyz;
    u_xlat22 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat22 = rsqrt(u_xlat22);
    u_xlat2.xyz = (float3)(u_xlat22) * u_xlat1.xyz;
    u_xlat1.xyz = mad((-u_xlat1.xyz), (float3)(u_xlat22), transpose(UNITY_MATRIX_I_V)[2].xyz);
    u_xlat1.xyz = mad(unity_OrthoParams.www, u_xlat1.xyz, u_xlat2.xyz);
    u_xlat2.x = input.TEXCOORD1.z;
    u_xlat2.y = input.TEXCOORD2.z;
    u_xlat2.z = input.TEXCOORD3.z;
    u_xlat1.x = dot(u_xlat2.xyz, u_xlat1.xyz);
    u_xlat1.x = u_xlat1.x + _RimOffset;
    u_xlat1.x = clamp(u_xlat1.x, 0.0f, 1.0f);
    u_xlat1.x = (-u_xlat1.x) + 1.0;
    u_xlat1.x = log2(u_xlat1.x);
    u_xlat1.x = u_xlat1.x * _RimPower;
    u_xlat1.x = exp2(u_xlat1.x);
    u_xlat21 = u_xlat21 * u_xlat1.x;
    u_xlat1.xyz = (float3)(u_xlat21) * _RimColor.xyz;
    u_xlat0.xyz = mad(u_xlat1.xyz, (float3)(_RimLightMul), u_xlat0.xyz);
    result.xyz = (half3)(u_xlat0.xyz);
    result.w = half(1.0);
    return result;
}
ENDCG
}
UsePass "Goose/Source/ToonLite2/ShadowCaster"
}
}
