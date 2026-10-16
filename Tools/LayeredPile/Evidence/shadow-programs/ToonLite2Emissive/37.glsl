#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_WorldToObject[4];
uniform 	vec4 unity_WorldTransformParams;
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	vec4 hlslcc_mtx4x4unity_WorldToLight[4];
uniform 	vec4 _texcoord_ST;
attribute highp vec4 in_POSITION0;
attribute highp vec4 in_TANGENT0;
attribute highp vec3 in_NORMAL0;
attribute highp vec4 in_TEXCOORD0;
varying highp vec2 vs_TEXCOORD0;
varying highp vec3 vs_TEXCOORD1;
varying highp vec3 vs_TEXCOORD2;
varying highp vec3 vs_TEXCOORD3;
varying highp vec3 vs_TEXCOORD4;
varying highp vec3 vs_TEXCOORD5;
varying highp vec4 vs_TEXCOORD6;
vec4 u_xlat0;
vec4 u_xlat1;
vec4 u_xlat2;
vec3 u_xlat3;
float u_xlat13;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat1 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _texcoord_ST.xy + _texcoord_ST.zw;
    u_xlat1.y = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[0].xyz);
    u_xlat1.z = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[1].xyz);
    u_xlat1.x = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[2].xyz);
    u_xlat13 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat13 = inversesqrt(u_xlat13);
    u_xlat1.xyz = vec3(u_xlat13) * u_xlat1.xyz;
    u_xlat2.xyz = in_TANGENT0.yyy * hlslcc_mtx4x4unity_ObjectToWorld[1].yzx;
    u_xlat2.xyz = hlslcc_mtx4x4unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
    u_xlat2.xyz = hlslcc_mtx4x4unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
    u_xlat13 = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat13 = inversesqrt(u_xlat13);
    u_xlat2.xyz = vec3(u_xlat13) * u_xlat2.xyz;
    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.xyz;
    u_xlat3.xyz = u_xlat1.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
    u_xlat13 = in_TANGENT0.w * unity_WorldTransformParams.w;
    u_xlat3.xyz = vec3(u_xlat13) * u_xlat3.xyz;
    vs_TEXCOORD1.y = u_xlat3.x;
    vs_TEXCOORD1.x = u_xlat2.z;
    vs_TEXCOORD1.z = u_xlat1.y;
    vs_TEXCOORD2.x = u_xlat2.x;
    vs_TEXCOORD3.x = u_xlat2.y;
    vs_TEXCOORD2.z = u_xlat1.z;
    vs_TEXCOORD3.z = u_xlat1.x;
    vs_TEXCOORD2.y = u_xlat3.y;
    vs_TEXCOORD3.y = u_xlat3.z;
    vs_TEXCOORD4.xyz = hlslcc_mtx4x4unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
    u_xlat1.xyz = u_xlat0.yyy * hlslcc_mtx4x4unity_WorldToLight[1].xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
    vs_TEXCOORD5.xyz = hlslcc_mtx4x4unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
    vs_TEXCOORD6 = vec4(0.0, 0.0, 0.0, 0.0);
    return;
}

#endif
#ifdef FRAGMENT
#version 100
#ifdef GL_EXT_shader_texture_lod
#extension GL_EXT_shader_texture_lod : enable
#endif
#if !defined(GL_EXT_shader_texture_lod)
#define texture1DLodEXT texture1D
#define texture2DLodEXT texture2D
#define texture2DProjLodEXT texture2DProj
#define texture3DLodEXT texture3D
#define textureCubeLodEXT textureCube
#endif

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
uniform 	vec4 _Time;
uniform 	vec3 _WorldSpaceCameraPos;
uniform 	vec4 _WorldSpaceLightPos0;
uniform 	vec4 _LightPositionRange;
uniform 	vec4 _LightProjectionParams;
uniform 	mediump vec4 _LightShadowData;
uniform 	vec4 unity_ShadowFadeCenterAndType;
uniform 	vec4 hlslcc_mtx4x4unity_MatrixV[4];
uniform 	vec4 hlslcc_mtx4x4unity_WorldToLight[4];
uniform 	mediump vec4 _LightColor0;
uniform 	vec4 _EmissiveTex_ST;
uniform 	float _EmissiveScale;
uniform 	float _EmissiveFloatBase;
uniform 	float _EmissiveFloatSpeed;
uniform 	float _EmissiveFloatScale;
uniform 	vec4 _IndirectColor;
uniform 	vec4 _NormalMap_ST;
uniform 	float _NormalScale;
uniform 	float _BaseCellOffset;
uniform 	float _BaseCellSharpness;
uniform 	vec4 _LightColor;
uniform 	vec4 _BaseColor_ST;
uniform 	vec4 _BaseTint;
uniform 	vec4 _AOTex_ST;
uniform 	float _AOScale;
uniform 	vec4 _ShadowAddColor;
uniform 	float _HighLightOffset;
uniform 	float _HighLightSmooth;
uniform 	float _HighLightPing;
uniform 	vec4 _HighLightColor;
uniform 	float _MetcapShadowMask;
uniform 	float _ReflectScale;
uniform 	vec2 _ReflectOffset;
uniform 	float _MatcapScale;
uniform 	vec4 _MatcapColor;
uniform 	vec4 _RoughnessMap_ST;
uniform 	float _RoughnessScale;
uniform lowp sampler2D _EmissiveTex;
uniform lowp sampler2D _NormalMap;
uniform lowp sampler2D _BaseColor;
uniform lowp sampler2D _AOTex;
uniform lowp sampler2D _Matcap;
uniform lowp sampler2D _RoughnessMap;
uniform highp sampler2D _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec2 vs_TEXCOORD0;
varying highp vec3 vs_TEXCOORD1;
varying highp vec3 vs_TEXCOORD2;
varying highp vec3 vs_TEXCOORD3;
varying highp vec3 vs_TEXCOORD4;
#define SV_Target0 gl_FragData[0]
vec3 u_xlat0;
vec4 u_xlat1;
bvec4 u_xlatb1;
vec4 u_xlat2;
vec4 u_xlat3;
vec4 u_xlat4;
mediump vec3 u_xlat16_5;
mediump vec3 u_xlat16_6;
vec3 u_xlat8;
mediump float u_xlat16_12;
float u_xlat15;
float u_xlat21;
float u_xlat22;
mediump float u_xlat16_26;
void main()
{
    u_xlat0.xy = vs_TEXCOORD0.xy * _BaseColor_ST.xy + _BaseColor_ST.zw;
    u_xlat0.xyz = texture2D(_BaseColor, u_xlat0.xy).xyz;
    u_xlat1.xyz = u_xlat0.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat1.xyz = u_xlat0.xyz * u_xlat1.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xyz;
    u_xlat1.xyz = _BaseTint.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat1.xyz = _BaseTint.xyz * u_xlat1.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat1.xyz = u_xlat1.xyz * _BaseTint.xyz;
    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xyz;
    u_xlat1.xy = vs_TEXCOORD0.xy * _AOTex_ST.xy + _AOTex_ST.zw;
    u_xlat21 = texture2D(_AOTex, u_xlat1.xy).x;
    u_xlat1.x = u_xlat21 * 0.305306017 + 0.682171106;
    u_xlat1.x = u_xlat21 * u_xlat1.x + 0.0125228781;
    u_xlat21 = u_xlat21 * u_xlat1.x + -1.0;
    u_xlat21 = _AOScale * u_xlat21 + 1.0;
    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
    u_xlat1.xyz = vs_TEXCOORD4.xyz + (-unity_ShadowFadeCenterAndType.xyz);
    u_xlat21 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat21 = sqrt(u_xlat21);
    u_xlat1.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
    u_xlat2.x = hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat2.y = hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat2.z = hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat1.x = dot(u_xlat1.xyz, u_xlat2.xyz);
    u_xlat21 = u_xlat21 + (-u_xlat1.x);
    u_xlat21 = unity_ShadowFadeCenterAndType.w * u_xlat21 + u_xlat1.x;
    u_xlat21 = u_xlat21 * _LightShadowData.z + _LightShadowData.w;
    u_xlat21 = clamp(u_xlat21, 0.0, 1.0);
    u_xlat1.xyz = vs_TEXCOORD4.xyz + (-_LightPositionRange.xyz);
    u_xlat22 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat22 = sqrt(u_xlat22);
    u_xlat22 = u_xlat22 * _LightPositionRange.w;
    u_xlat22 = u_xlat22 * _LightProjectionParams.w;
    u_xlat2.xyz = u_xlat1.xyz + vec3(0.0078125, 0.0078125, 0.0078125);
    u_xlat2 = textureCubeLodEXT(_ShadowMapTexture, u_xlat2.xyz, 0.0);
    u_xlat2.x = dot(u_xlat2, vec4(1.0, 0.00392156886, 1.53787005e-05, 6.03086292e-08));
    u_xlat3.xyz = u_xlat1.xyz + vec3(-0.0078125, -0.0078125, 0.0078125);
    u_xlat3 = textureCubeLodEXT(_ShadowMapTexture, u_xlat3.xyz, 0.0);
    u_xlat2.y = dot(u_xlat3, vec4(1.0, 0.00392156886, 1.53787005e-05, 6.03086292e-08));
    u_xlat3.xyz = u_xlat1.xyz + vec3(-0.0078125, 0.0078125, -0.0078125);
    u_xlat1.xyz = u_xlat1.xyz + vec3(0.0078125, -0.0078125, -0.0078125);
    u_xlat4 = textureCubeLodEXT(_ShadowMapTexture, u_xlat1.xyz, 0.0);
    u_xlat2.w = dot(u_xlat4, vec4(1.0, 0.00392156886, 1.53787005e-05, 6.03086292e-08));
    u_xlat3 = textureCubeLodEXT(_ShadowMapTexture, u_xlat3.xyz, 0.0);
    u_xlat2.z = dot(u_xlat3, vec4(1.0, 0.00392156886, 1.53787005e-05, 6.03086292e-08));
    u_xlatb1 = lessThan(u_xlat2, vec4(u_xlat22));
    u_xlat1.x = (u_xlatb1.x) ? _LightShadowData.x : float(1.0);
    u_xlat1.y = (u_xlatb1.y) ? _LightShadowData.x : float(1.0);
    u_xlat1.z = (u_xlatb1.z) ? _LightShadowData.x : float(1.0);
    u_xlat1.w = (u_xlatb1.w) ? _LightShadowData.x : float(1.0);
    u_xlat16_5.x = dot(u_xlat1, vec4(0.25, 0.25, 0.25, 0.25));
    u_xlat16_12 = (-u_xlat16_5.x) + 1.0;
    u_xlat16_5.x = u_xlat21 * u_xlat16_12 + u_xlat16_5.x;
    u_xlat1.xyz = vs_TEXCOORD4.yyy * hlslcc_mtx4x4unity_WorldToLight[1].xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_WorldToLight[0].xyz * vs_TEXCOORD4.xxx + u_xlat1.xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_WorldToLight[2].xyz * vs_TEXCOORD4.zzz + u_xlat1.xyz;
    u_xlat1.xyz = u_xlat1.xyz + hlslcc_mtx4x4unity_WorldToLight[3].xyz;
    u_xlat21 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat21 = texture2D(_LightTexture0, vec2(u_xlat21)).x;
    u_xlat21 = u_xlat16_5.x * u_xlat21;
    u_xlat16_5.xyz = vec3(u_xlat21) * _LightColor0.xyz;
    u_xlat16_6.xyz = _LightColor0.xyz + vec3(9.99999997e-07, 9.99999997e-07, 9.99999997e-07);
    u_xlat16_5.xyz = u_xlat16_5.xyz / u_xlat16_6.xyz;
    u_xlat21 = max(u_xlat16_5.y, u_xlat16_5.x);
    u_xlat21 = max(u_xlat16_5.z, u_xlat21);
    u_xlat1.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceLightPos0.xyz;
    u_xlat22 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat22 = inversesqrt(u_xlat22);
    u_xlat1.xyz = vec3(u_xlat22) * u_xlat1.xyz;
    u_xlat2.xy = vs_TEXCOORD0.xy * _NormalMap_ST.xy + _NormalMap_ST.zw;
    u_xlat2.xyz = texture2D(_NormalMap, u_xlat2.xy).xyw;
    u_xlat16_5.x = dot(u_xlat2.xx, u_xlat2.zz);
    u_xlat16_5.y = u_xlat2.y + u_xlat2.y;
    u_xlat16_5.xy = u_xlat16_5.xy + vec2(-1.0, -1.0);
    u_xlat16_5.xy = u_xlat16_5.xy * vec2(_NormalScale);
    u_xlat16_26 = dot(u_xlat16_5.xy, u_xlat16_5.xy);
    u_xlat16_26 = min(u_xlat16_26, 1.0);
    u_xlat16_26 = (-u_xlat16_26) + 1.0;
    u_xlat16_5.z = sqrt(u_xlat16_26);
    u_xlat16_6.x = dot(vs_TEXCOORD1.xyz, u_xlat16_5.xyz);
    u_xlat16_6.y = dot(vs_TEXCOORD2.xyz, u_xlat16_5.xyz);
    u_xlat16_6.z = dot(vs_TEXCOORD3.xyz, u_xlat16_5.xyz);
    u_xlat16_5.x = dot(u_xlat16_6.xyz, u_xlat16_6.xyz);
    u_xlat16_5.x = inversesqrt(u_xlat16_5.x);
    u_xlat16_5.xyz = u_xlat16_5.xxx * u_xlat16_6.xyz;
    u_xlat1.x = dot(u_xlat16_5.xyz, u_xlat1.xyz);
    u_xlat8.x = u_xlat1.x + _BaseCellOffset;
    u_xlat1.x = u_xlat1.x + _HighLightOffset;
    u_xlat1.x = u_xlat1.x / _HighLightSmooth;
    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
    u_xlat8.x = u_xlat8.x / _BaseCellSharpness;
    u_xlat8.x = clamp(u_xlat8.x, 0.0, 1.0);
    u_xlat15 = u_xlat21 * u_xlat8.x;
    u_xlat2.xyz = vec3(u_xlat15) * _LightColor.xyz + _IndirectColor.xyz;
    u_xlat15 = (-u_xlat8.x) * u_xlat21 + 1.0;
    u_xlat21 = u_xlat8.x * u_xlat21 + -1.0;
    u_xlat21 = _MetcapShadowMask * u_xlat21 + 1.0;
    u_xlat8.xyz = vec3(u_xlat15) * _ShadowAddColor.xyz;
    u_xlat0.xyz = u_xlat2.xyz * u_xlat0.xyz + u_xlat8.xyz;
    u_xlat8.x = u_xlat1.x * u_xlat1.x + (-u_xlat1.x);
    u_xlat1.x = _HighLightPing * u_xlat8.x + u_xlat1.x;
    u_xlat0.xyz = u_xlat1.xxx * _HighLightColor.xyz + u_xlat0.xyz;
    u_xlat1.xy = u_xlat16_5.yy * hlslcc_mtx4x4unity_MatrixV[1].xy;
    u_xlat1.xy = hlslcc_mtx4x4unity_MatrixV[0].xy * u_xlat16_5.xx + u_xlat1.xy;
    u_xlat1.xy = hlslcc_mtx4x4unity_MatrixV[2].xy * u_xlat16_5.zz + u_xlat1.xy;
    u_xlat1.xy = u_xlat1.xy * vec2(vec2(_ReflectScale, _ReflectScale));
    u_xlat1.xy = u_xlat1.xy * vec2(0.5, 0.5) + vec2(_ReflectOffset.x, _ReflectOffset.y);
    u_xlat1.xy = u_xlat1.xy + vec2(0.5, 0.5);
    u_xlat1.xyz = texture2D(_Matcap, u_xlat1.xy).xyz;
    u_xlat2.xyz = u_xlat1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat2.xyz = u_xlat1.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xyz;
    u_xlat1.xyz = u_xlat1.xyz * vec3(_MatcapScale);
    u_xlat1.xyz = vec3(u_xlat21) * u_xlat1.xyz;
    u_xlat1.xyz = u_xlat1.xyz * _MatcapColor.xyz;
    u_xlat2.xy = vs_TEXCOORD0.xy * _RoughnessMap_ST.xy + _RoughnessMap_ST.zw;
    u_xlat21 = texture2D(_RoughnessMap, u_xlat2.xy).x;
    u_xlat21 = _RoughnessScale * (-u_xlat21) + 1.0;
    u_xlat0.xyz = u_xlat1.xyz * vec3(u_xlat21) + u_xlat0.xyz;
    u_xlat1.xy = vs_TEXCOORD0.xy * _EmissiveTex_ST.xy + _EmissiveTex_ST.zw;
    u_xlat1.xyz = texture2D(_EmissiveTex, u_xlat1.xy).xyz;
    u_xlat2.xyz = u_xlat1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat2.xyz = u_xlat1.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xyz;
    u_xlat1.xyz = u_xlat1.xyz * vec3(_EmissiveScale);
    u_xlat21 = _Time.y * _EmissiveFloatSpeed;
    u_xlat21 = sin(u_xlat21);
    u_xlat21 = u_xlat21 * _EmissiveFloatScale + _EmissiveFloatBase;
    u_xlat0.xyz = u_xlat1.xyz * vec3(u_xlat21) + u_xlat0.xyz;
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_5.xyz = max(u_xlat0.xyz, vec3(0.0, 0.0, 0.0));
    u_xlat0.xyz = log2(u_xlat16_5.xyz);
    u_xlat0.xyz = u_xlat0.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
    u_xlat0.xyz = max(u_xlat0.xyz, vec3(0.0, 0.0, 0.0));
    SV_Target0.xyz = u_xlat0.xyz;
    SV_Target0.w = 1.0;
    return;
}

#endif
