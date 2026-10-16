#ifdef VERTEX
#version 100

uniform 	vec4 _Time;
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	vec4 _MainTex_TexelSize;
uniform 	float _BlurScale;
uniform 	float _Speed;
attribute highp vec4 in_POSITION0;
attribute highp vec2 in_TEXCOORD0;
varying highp vec2 vs_TEXCOORD0;
highp  vec4 phase0_Output0_1;
varying highp vec2 vs_TEXCOORD1;
varying highp vec2 vs_TEXCOORD2;
varying highp vec2 vs_TEXCOORD3;
varying highp vec2 vs_TEXCOORD4;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    u_xlat0.x = _Time.w * _Speed;
    u_xlat0.x = sin(u_xlat0.x);
    u_xlat0.x = u_xlat0.x * 0.25 + 1.0;
    u_xlat0.x = u_xlat0.x * _BlurScale;
    u_xlat0.x = u_xlat0.x * _MainTex_TexelSize.x;
    phase0_Output0_1.z = (-u_xlat0.x) * 1.38461542 + in_TEXCOORD0.x;
    u_xlat0.xz = u_xlat0.xx * vec2(1.38461542, 3.23076916);
    phase0_Output0_1.xyw = in_TEXCOORD0.xyy;
    u_xlat0.w = 0.0;
    vs_TEXCOORD2.xy = u_xlat0.xw + in_TEXCOORD0.xy;
    vs_TEXCOORD3.xy = (-u_xlat0.zw) + in_TEXCOORD0.xy;
    vs_TEXCOORD4.xy = u_xlat0.zw + in_TEXCOORD0.xy;
vs_TEXCOORD0 = phase0_Output0_1.xy;
vs_TEXCOORD1 = phase0_Output0_1.zw;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
uniform highp sampler2D _MainTex;
varying highp vec2 vs_TEXCOORD0;
varying highp vec2 vs_TEXCOORD1;
varying highp vec2 vs_TEXCOORD2;
varying highp vec2 vs_TEXCOORD3;
varying highp vec2 vs_TEXCOORD4;
#define SV_Target0 gl_FragData[0]
vec4 u_xlat0;
vec4 u_xlat1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = texture2D(_MainTex, vs_TEXCOORD1.xy);
    u_xlat1 = texture2D(_MainTex, vs_TEXCOORD2.xy);
    u_xlat0 = u_xlat0 + u_xlat1;
    u_xlat0 = u_xlat0 * vec4(0.31621623, 0.31621623, 0.31621623, 0.31621623);
    u_xlat1 = texture2D(_MainTex, vs_TEXCOORD0.xy);
    u_xlat0 = u_xlat1 * vec4(0.227027029, 0.227027029, 0.227027029, 0.227027029) + u_xlat0;
    u_xlat1 = texture2D(_MainTex, vs_TEXCOORD3.xy);
    u_xlat2 = texture2D(_MainTex, vs_TEXCOORD4.xy);
    u_xlat1 = u_xlat1 + u_xlat2;
    SV_Target0 = u_xlat1 * vec4(0.0702702701, 0.0702702701, 0.0702702701, 0.0702702701) + u_xlat0;
    return;
}

#endif
