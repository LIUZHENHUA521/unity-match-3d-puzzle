#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_WorldToObject[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	vec4 _MainTex_ST;
attribute highp vec4 in_POSITION0;
attribute highp vec2 in_TEXCOORD0;
attribute highp vec3 in_NORMAL0;
varying highp vec2 vs_TEXCOORD0;
varying highp vec3 vs_TEXCOORD1;
varying highp vec3 vs_TEXCOORD2;
vec4 u_xlat0;
vec4 u_xlat1;
float u_xlat6;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat1 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    vs_TEXCOORD1.xyz = hlslcc_mtx4x4unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
    u_xlat0 = u_xlat1.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
    gl_Position = u_xlat0 + vec4(0.0, 0.0, -9.99999975e-05, 0.0);
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    u_xlat0.x = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[0].xyz);
    u_xlat0.y = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[1].xyz);
    u_xlat0.z = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[2].xyz);
    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat6 = inversesqrt(u_xlat6);
    vs_TEXCOORD2.xyz = vec3(u_xlat6) * u_xlat0.xyz;
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
uniform 	vec4 _Time;
uniform 	mediump vec4 _OverlayColor;
uniform 	mediump vec4 _OverlayBackColor;
uniform 	mediump vec4 _OverlayData;
uniform 	vec4 _OverlayHitPosData;
uniform 	float _OverlayHitStartTime;
uniform 	mediump float _CutOff;
uniform lowp sampler2D _MainTex;
uniform lowp sampler2D _OverlayTexture;
varying highp vec2 vs_TEXCOORD0;
varying highp vec3 vs_TEXCOORD1;
varying highp vec3 vs_TEXCOORD2;
#define SV_Target0 gl_FragData[0]
vec4 u_xlat0;
mediump vec4 u_xlat16_0;
mediump vec4 u_xlat16_1;
vec4 u_xlat2;
bool u_xlatb2;
vec4 u_xlat3;
mediump vec4 u_xlat16_3;
vec4 u_xlat4;
float u_xlat7;
bool u_xlatb7;
void main()
{
    u_xlat0 = texture2D(_MainTex, vs_TEXCOORD0.xy);
    u_xlat16_1.x = u_xlat0.w + (-_CutOff);
    u_xlat16_0 = u_xlat0 * _OverlayBackColor;
    u_xlat16_0 = u_xlat16_0 * _OverlayColor + (-_OverlayColor);
    u_xlat16_0 = _OverlayData.zzzz * u_xlat16_0 + _OverlayColor;
    u_xlatb2 = u_xlat16_1.x<0.0;
    if(u_xlatb2){discard;}
    u_xlat2.x = _Time.y * 1000.0;
    u_xlatb2 = u_xlat2.x>=(-u_xlat2.x);
    u_xlat2.xy = (bool(u_xlatb2)) ? vec2(1000.0, 0.00100000005) : vec2(-1000.0, -0.00100000005);
    u_xlat7 = u_xlat2.y * _Time.y;
    u_xlat7 = fract(u_xlat7);
    u_xlat2.x = u_xlat7 * u_xlat2.x;
    u_xlat2.x = u_xlat2.x * _OverlayData.x;
    u_xlat2.x = fract(u_xlat2.x);
    u_xlat2.x = (-u_xlat2.x) + 0.5;
    u_xlat16_1.x = (-_OverlayData.y) + 1.0;
    u_xlat16_1.x = u_xlat16_1.x + u_xlat16_1.x;
    u_xlat2.x = u_xlat16_1.x * abs(u_xlat2.x) + _OverlayData.y;
    u_xlat16_1.x = u_xlat16_0.w * u_xlat2.x;
    u_xlat2.xyz = vs_TEXCOORD1.xyz + (-_OverlayHitPosData.xyz);
    u_xlat2.x = dot(u_xlat2.xyz, u_xlat2.xyz);
    u_xlat2.x = sqrt(u_xlat2.x);
    u_xlat7 = _Time.y + (-_OverlayHitStartTime);
    u_xlat7 = min(u_xlat7, _OverlayHitPosData.w);
    u_xlat2.x = u_xlat7 / u_xlat2.x;
    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
    u_xlat2.x = u_xlat16_1.x * u_xlat2.x;
    u_xlatb7 = 0.0<_OverlayHitPosData.w;
    u_xlat16_0.w = (u_xlatb7) ? u_xlat2.x : u_xlat16_1.x;
    u_xlat2.xyz = vs_TEXCOORD2.xyz * vs_TEXCOORD2.xyz;
    u_xlat2.xyz = u_xlat2.xyz * u_xlat2.xyz;
    u_xlat2.xyz = min(u_xlat2.xyz, vec3(1.0, 1.0, 1.0));
    u_xlat16_1.x = dot(u_xlat2.xyz, vec3(1.0, 1.0, 1.0));
    u_xlat16_1.x = max(u_xlat16_1.x, 9.99999975e-05);
    u_xlat16_1.xyz = u_xlat2.xyz / u_xlat16_1.xxx;
    u_xlat2.xyz = vs_TEXCOORD1.xyz * _OverlayData.www;
    u_xlat3 = texture2D(_OverlayTexture, u_xlat2.xz);
    u_xlat16_3 = u_xlat16_1.yyyy * u_xlat3;
    u_xlat4 = texture2D(_OverlayTexture, u_xlat2.zy);
    u_xlat2 = texture2D(_OverlayTexture, u_xlat2.xy);
    u_xlat16_3 = u_xlat4 * u_xlat16_1.xxxx + u_xlat16_3;
    u_xlat16_1 = u_xlat2 * u_xlat16_1.zzzz + u_xlat16_3;
    SV_Target0 = u_xlat16_0 * u_xlat16_1;
    return;
}

#endif
