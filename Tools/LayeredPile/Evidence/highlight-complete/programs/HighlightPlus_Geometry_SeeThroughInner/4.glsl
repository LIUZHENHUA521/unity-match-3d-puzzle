#ifdef VERTEX
#version 100

uniform 	vec4 _ProjectionParams;
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_WorldToObject[4];
uniform 	vec4 hlslcc_mtx4x4glstate_matrix_projection[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixV[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixInvV[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	vec4 _MainTex_ST;
uniform 	float _SeeThroughBorderWidth;
uniform 	float _SeeThroughBorderConstantWidth;
uniform 	int _SeeThroughOrdered;
attribute highp vec4 in_POSITION0;
attribute highp vec3 in_NORMAL0;
attribute highp vec2 in_TEXCOORD0;
varying highp vec2 vs_TEXCOORD0;
varying highp float vs_TEXCOORD2;
varying highp vec4 vs_TEXCOORD1;
vec4 u_xlat0;
vec4 u_xlat1;
vec4 u_xlat2;
float u_xlat6;
void main()
{
    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].yyy;
    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].xxx + u_xlat0.xyz;
    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].zzz + u_xlat0.xyz;
    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].www + u_xlat0.xyz;
    u_xlat0.x = dot(u_xlat0.xyz, in_NORMAL0.xyz);
    u_xlat0.xy = u_xlat0.xx * hlslcc_mtx4x4glstate_matrix_projection[1].xy;
    u_xlat1.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].yyy;
    u_xlat1.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].xxx + u_xlat1.xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].zzz + u_xlat1.xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].www + u_xlat1.xyz;
    u_xlat6 = dot(u_xlat1.xyz, in_NORMAL0.xyz);
    u_xlat0.xy = hlslcc_mtx4x4glstate_matrix_projection[0].xy * vec2(u_xlat6) + u_xlat0.xy;
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
    u_xlat6 = (-u_xlat2.z) + 2.0;
    u_xlat6 = hlslcc_mtx4x4glstate_matrix_projection[3].w * u_xlat6 + u_xlat2.z;
    u_xlat6 = u_xlat6 + -2.0;
    u_xlat6 = _SeeThroughBorderConstantWidth * u_xlat6 + 2.0;
    u_xlat0.xy = vec2(u_xlat6) * u_xlat0.xy;
    gl_Position.xy = (-u_xlat0.xy) * vec2(_SeeThroughBorderWidth) + u_xlat2.xy;
    u_xlat0.x = u_xlat2.w + u_xlat2.z;
    u_xlat0.x = u_xlat0.x * 0.00100000005 + (-u_xlat2.w);
    gl_Position.z = (_SeeThroughOrdered != 0) ? u_xlat0.x : u_xlat2.z;
    gl_Position.w = u_xlat2.w;
    u_xlat0.x = u_xlat1.y * hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat0.x = hlslcc_mtx4x4unity_MatrixV[0].z * u_xlat1.x + u_xlat0.x;
    u_xlat0.x = hlslcc_mtx4x4unity_MatrixV[2].z * u_xlat1.z + u_xlat0.x;
    u_xlat0.x = hlslcc_mtx4x4unity_MatrixV[3].z * u_xlat1.w + u_xlat0.x;
    vs_TEXCOORD2 = (-u_xlat0.x);
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    u_xlat0.x = u_xlat2.y * _ProjectionParams.x;
    u_xlat0.w = u_xlat0.x * 0.5;
    u_xlat0.xz = u_xlat2.xw * vec2(0.5, 0.5);
    vs_TEXCOORD1.zw = u_xlat2.zw;
    vs_TEXCOORD1.xy = u_xlat0.zz + u_xlat0.xw;
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
uniform 	vec4 _ProjectionParams;
uniform 	vec4 _ZBufferParams;
uniform 	vec4 unity_OrthoParams;
uniform 	mediump float _SeeThrough;
uniform 	mediump vec4 _SeeThroughTintColor;
uniform 	mediump float _CutOff;
uniform 	mediump float _SeeThroughNoise;
uniform 	float _SeeThroughDepthOffset;
uniform 	float _SeeThroughMaxDepth;
uniform highp sampler2D _CameraDepthTexture;
uniform lowp sampler2D _MainTex;
varying highp vec2 vs_TEXCOORD0;
varying highp float vs_TEXCOORD2;
varying highp vec4 vs_TEXCOORD1;
#define SV_Target0 gl_FragData[0]
vec4 u_xlat0;
bool u_xlatb0;
mediump vec3 u_xlat16_1;
vec2 u_xlat2;
bool u_xlatb2;
float u_xlat3;
bool u_xlatb3;
float u_xlat5;
float u_xlat6;
bool u_xlatb9;
void main()
{
vec4 hlslcc_FragCoord = vec4(gl_FragCoord.xyz, 1.0/gl_FragCoord.w);
    u_xlat0.xy = vs_TEXCOORD1.xy / vs_TEXCOORD1.ww;
    u_xlat0.x = texture2D(_CameraDepthTexture, u_xlat0.xy).x;
    u_xlat3 = _ZBufferParams.z * u_xlat0.x + _ZBufferParams.w;
    u_xlat3 = float(1.0) / u_xlat3;
    u_xlat6 = (-_ProjectionParams.y) + _ProjectionParams.z;
    u_xlat0.x = (-u_xlat0.x) + 1.0;
    u_xlat0.x = u_xlat6 * u_xlat0.x + _ProjectionParams.y;
    u_xlat0.x = (-u_xlat3) + u_xlat0.x;
    u_xlat0.x = unity_OrthoParams.w * u_xlat0.x + u_xlat3;
    u_xlat0.x = (-u_xlat0.x) + vs_TEXCOORD2;
    u_xlat3 = u_xlat0.x + (-_SeeThroughDepthOffset);
    u_xlatb3 = u_xlat3<0.0;
    u_xlatb0 = _SeeThroughMaxDepth<u_xlat0.x;
    u_xlatb0 = u_xlatb0 || u_xlatb3;
    if(u_xlatb0){discard;}
    u_xlat0 = texture2D(_MainTex, vs_TEXCOORD0.xy);
    u_xlat16_1.x = u_xlat0.w + (-_CutOff);
    u_xlatb9 = u_xlat16_1.x<0.0;
    if(u_xlatb9){discard;}
    u_xlat16_1.xyz = (-u_xlat0.xyz) + _SeeThroughTintColor.xyz;
    u_xlat16_1.xyz = _SeeThroughTintColor.www * u_xlat16_1.xyz + u_xlat0.xyz;
    u_xlatb0 = _Time.w>=(-_Time.w);
    u_xlat0.x = (u_xlatb0) ? 1.0 : -1.0;
    u_xlat3 = u_xlat0.x * _Time.w;
    u_xlat3 = fract(u_xlat3);
    u_xlat0.x = u_xlat3 * u_xlat0.x;
    u_xlat0.x = u_xlat0.x * hlslcc_FragCoord.y;
    u_xlat0.x = fract(u_xlat0.x);
    u_xlat0.x = u_xlat0.x * 0.100000001;
    u_xlat0.xyz = vec3(vec3(_SeeThroughNoise, _SeeThroughNoise, _SeeThroughNoise)) * u_xlat0.xxx + u_xlat16_1.xyz;
    u_xlat2.x = hlslcc_FragCoord.y + hlslcc_FragCoord.y;
    u_xlatb2 = u_xlat2.x>=(-u_xlat2.x);
    u_xlat2.xy = (bool(u_xlatb2)) ? vec2(2.0, 0.5) : vec2(-2.0, -0.5);
    u_xlat5 = u_xlat2.y * hlslcc_FragCoord.y;
    u_xlat5 = fract(u_xlat5);
    u_xlat2.x = u_xlat2.x * u_xlat5 + -1.0;
    u_xlat2.x = _SeeThrough * u_xlat2.x + (-_SeeThrough);
    u_xlat0.w = _SeeThroughNoise * u_xlat2.x + _SeeThrough;
    SV_Target0 = u_xlat0;
    return;
}

#endif
