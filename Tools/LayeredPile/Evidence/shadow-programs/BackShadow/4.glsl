#ifdef VERTEX
#version 100

uniform 	mediump vec4 unity_SHBr;
uniform 	mediump vec4 unity_SHBg;
uniform 	mediump vec4 unity_SHBb;
uniform 	mediump vec4 unity_SHC;
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_WorldToObject[4];
uniform 	vec4 unity_WorldTransformParams;
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
attribute highp vec4 in_POSITION0;
attribute highp vec4 in_TANGENT0;
attribute highp vec3 in_NORMAL0;
varying highp vec4 vs_TEXCOORD0;
varying mediump vec3 vs_TEXCOORD1;
varying highp vec4 vs_TEXCOORD5;
varying highp vec4 vs_TEXCOORD6;
varying highp vec4 vs_TEXCOORD7;
vec4 u_xlat0;
vec4 u_xlat1;
mediump vec3 u_xlat16_2;
mediump vec4 u_xlat16_3;
mediump vec3 u_xlat16_4;
vec3 u_xlat5;
float u_xlat18;
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
    vs_TEXCOORD0 = vec4(0.0, 0.0, 0.0, 0.0);
    u_xlat1.x = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[0].xyz);
    u_xlat1.y = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[1].xyz);
    u_xlat1.z = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[2].xyz);
    u_xlat18 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat18 = inversesqrt(u_xlat18);
    u_xlat1 = vec4(u_xlat18) * u_xlat1.xyzz;
    u_xlat16_2.x = u_xlat1.y * u_xlat1.y;
    u_xlat16_2.x = u_xlat1.x * u_xlat1.x + (-u_xlat16_2.x);
    u_xlat16_3 = u_xlat1.ywzx * u_xlat1;
    u_xlat16_4.x = dot(unity_SHBr, u_xlat16_3);
    u_xlat16_4.y = dot(unity_SHBg, u_xlat16_3);
    u_xlat16_4.z = dot(unity_SHBb, u_xlat16_3);
    vs_TEXCOORD1.xyz = unity_SHC.xyz * u_xlat16_2.xxx + u_xlat16_4.xyz;
    vs_TEXCOORD5.w = u_xlat0.x;
    u_xlat5.xyz = in_TANGENT0.yyy * hlslcc_mtx4x4unity_ObjectToWorld[1].yzx;
    u_xlat5.xyz = hlslcc_mtx4x4unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat5.xyz;
    u_xlat5.xyz = hlslcc_mtx4x4unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat5.xyz;
    u_xlat0.x = dot(u_xlat5.xyz, u_xlat5.xyz);
    u_xlat0.x = inversesqrt(u_xlat0.x);
    u_xlat5.xyz = u_xlat0.xxx * u_xlat5.xyz;
    u_xlat16_2.xyz = u_xlat1.wxy * u_xlat5.xyz;
    u_xlat16_2.xyz = u_xlat1.ywx * u_xlat5.yzx + (-u_xlat16_2.xyz);
    u_xlat0.x = in_TANGENT0.w * unity_WorldTransformParams.w;
    u_xlat16_2.xyz = u_xlat0.xxx * u_xlat16_2.xyz;
    vs_TEXCOORD5.y = u_xlat16_2.x;
    vs_TEXCOORD5.z = u_xlat1.x;
    vs_TEXCOORD5.x = u_xlat5.z;
    vs_TEXCOORD6.x = u_xlat5.x;
    vs_TEXCOORD7.x = u_xlat5.y;
    vs_TEXCOORD6.z = u_xlat1.y;
    vs_TEXCOORD7.z = u_xlat1.w;
    vs_TEXCOORD6.w = u_xlat0.y;
    vs_TEXCOORD7.w = u_xlat0.z;
    vs_TEXCOORD6.y = u_xlat16_2.y;
    vs_TEXCOORD7.y = u_xlat16_2.z;
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
uniform 	vec3 _WorldSpaceCameraPos;
uniform 	mediump vec4 _WorldSpaceLightPos0;
uniform 	vec4 unity_SpecCube0_BoxMax;
uniform 	vec4 unity_SpecCube0_BoxMin;
uniform 	vec4 unity_SpecCube0_ProbePosition;
uniform 	mediump vec4 unity_SpecCube0_HDR;
uniform 	vec4 unity_SpecCube1_BoxMax;
uniform 	vec4 unity_SpecCube1_BoxMin;
uniform 	vec4 unity_SpecCube1_ProbePosition;
uniform 	mediump vec4 unity_SpecCube1_HDR;
uniform 	mediump vec4 _LightColor0;
uniform lowp samplerCube unity_SpecCube0;
uniform lowp samplerCube unity_SpecCube1;
varying highp vec4 vs_TEXCOORD5;
varying highp vec4 vs_TEXCOORD6;
varying highp vec4 vs_TEXCOORD7;
#define SV_Target0 gl_FragData[0]
vec3 u_xlat0;
vec3 u_xlat1;
vec3 u_xlat2;
vec3 u_xlat3;
mediump vec4 u_xlat16_4;
vec4 u_xlat5;
vec4 u_xlat6;
vec3 u_xlat7;
vec3 u_xlat8;
bvec3 u_xlatb8;
vec3 u_xlat9;
mediump vec3 u_xlat16_9;
bvec3 u_xlatb10;
mediump float u_xlat16_11;
vec3 u_xlat12;
mediump float u_xlat16_16;
float u_xlat24;
mediump float u_xlat16_28;
float u_xlat36;
float u_xlat37;
bool u_xlatb37;
mediump float u_xlat16_40;
mediump float u_xlat16_45;
void main()
{
    u_xlat0.x = vs_TEXCOORD5.w;
    u_xlat0.y = vs_TEXCOORD6.w;
    u_xlat0.z = vs_TEXCOORD7.w;
    u_xlat1.xyz = (-u_xlat0.xyz) + _WorldSpaceCameraPos.xyz;
    u_xlat36 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat36 = inversesqrt(u_xlat36);
    u_xlat2.xyz = vec3(u_xlat36) * u_xlat1.xyz;
    u_xlat3.x = vs_TEXCOORD5.z;
    u_xlat3.y = vs_TEXCOORD6.z;
    u_xlat3.z = vs_TEXCOORD7.z;
    u_xlat37 = dot(u_xlat3.xyz, u_xlat3.xyz);
    u_xlat37 = inversesqrt(u_xlat37);
    u_xlat3.xyz = vec3(u_xlat37) * u_xlat3.xyz;
    u_xlat16_4.x = dot((-u_xlat2.xyz), u_xlat3.xyz);
    u_xlat16_4.x = u_xlat16_4.x + u_xlat16_4.x;
    u_xlat16_4.xyz = u_xlat3.xyz * (-u_xlat16_4.xxx) + (-u_xlat2.xyz);
    u_xlatb37 = 0.0<unity_SpecCube0_ProbePosition.w;
    if(u_xlatb37){
        u_xlat37 = dot(u_xlat16_4.xyz, u_xlat16_4.xyz);
        u_xlat37 = inversesqrt(u_xlat37);
        u_xlat5.xyz = vec3(u_xlat37) * u_xlat16_4.xyz;
        u_xlat6.xyz = (-u_xlat0.xyz) + unity_SpecCube0_BoxMax.xyz;
        u_xlat6.xyz = u_xlat6.xyz / u_xlat5.xyz;
        u_xlat7.xyz = (-u_xlat0.xyz) + unity_SpecCube0_BoxMin.xyz;
        u_xlat7.xyz = u_xlat7.xyz / u_xlat5.xyz;
        u_xlatb8.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat5.xyzx).xyz;
        {
            vec4 hlslcc_movcTemp = u_xlat6;
            hlslcc_movcTemp.x = (u_xlatb8.x) ? u_xlat6.x : u_xlat7.x;
            hlslcc_movcTemp.y = (u_xlatb8.y) ? u_xlat6.y : u_xlat7.y;
            hlslcc_movcTemp.z = (u_xlatb8.z) ? u_xlat6.z : u_xlat7.z;
            u_xlat6 = hlslcc_movcTemp;
        }
        u_xlat37 = min(u_xlat6.y, u_xlat6.x);
        u_xlat37 = min(u_xlat6.z, u_xlat37);
        u_xlat6.xyz = u_xlat0.xyz + (-unity_SpecCube0_ProbePosition.xyz);
        u_xlat5.xyz = u_xlat5.xyz * vec3(u_xlat37) + u_xlat6.xyz;
    } else {
        u_xlat5.xyz = u_xlat16_4.xyz;
    }
    u_xlat5 = textureCubeLodEXT(unity_SpecCube0, u_xlat5.xyz, 6.0);
    u_xlat16_40 = u_xlat5.w + -1.0;
    u_xlat16_40 = unity_SpecCube0_HDR.w * u_xlat16_40 + 1.0;
    u_xlat16_40 = u_xlat16_40 * unity_SpecCube0_HDR.x;
    u_xlat16_9.xyz = u_xlat5.xyz * vec3(u_xlat16_40);
    u_xlatb37 = unity_SpecCube0_BoxMin.w<0.999989986;
    if(u_xlatb37){
        u_xlatb37 = 0.0<unity_SpecCube1_ProbePosition.w;
        if(u_xlatb37){
            u_xlat37 = dot(u_xlat16_4.xyz, u_xlat16_4.xyz);
            u_xlat37 = inversesqrt(u_xlat37);
            u_xlat6.xyz = vec3(u_xlat37) * u_xlat16_4.xyz;
            u_xlat7.xyz = (-u_xlat0.xyz) + unity_SpecCube1_BoxMax.xyz;
            u_xlat7.xyz = u_xlat7.xyz / u_xlat6.xyz;
            u_xlat8.xyz = (-u_xlat0.xyz) + unity_SpecCube1_BoxMin.xyz;
            u_xlat8.xyz = u_xlat8.xyz / u_xlat6.xyz;
            u_xlatb10.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat6.xyzx).xyz;
            {
                vec3 hlslcc_movcTemp = u_xlat7;
                hlslcc_movcTemp.x = (u_xlatb10.x) ? u_xlat7.x : u_xlat8.x;
                hlslcc_movcTemp.y = (u_xlatb10.y) ? u_xlat7.y : u_xlat8.y;
                hlslcc_movcTemp.z = (u_xlatb10.z) ? u_xlat7.z : u_xlat8.z;
                u_xlat7 = hlslcc_movcTemp;
            }
            u_xlat37 = min(u_xlat7.y, u_xlat7.x);
            u_xlat37 = min(u_xlat7.z, u_xlat37);
            u_xlat0.xyz = u_xlat0.xyz + (-unity_SpecCube1_ProbePosition.xyz);
            u_xlat0.xyz = u_xlat6.xyz * vec3(u_xlat37) + u_xlat0.xyz;
        } else {
            u_xlat0.xyz = u_xlat16_4.xyz;
        }
        u_xlat6 = textureCubeLodEXT(unity_SpecCube1, u_xlat0.xyz, 6.0);
        u_xlat16_4.x = u_xlat6.w + -1.0;
        u_xlat16_4.x = unity_SpecCube1_HDR.w * u_xlat16_4.x + 1.0;
        u_xlat16_4.x = u_xlat16_4.x * unity_SpecCube1_HDR.x;
        u_xlat16_4.xyz = u_xlat6.xyz * u_xlat16_4.xxx;
        u_xlat0.xyz = vec3(u_xlat16_40) * u_xlat5.xyz + (-u_xlat16_4.xyz);
        u_xlat9.xyz = unity_SpecCube0_BoxMin.www * u_xlat0.xyz + u_xlat16_4.xyz;
        u_xlat16_9.xyz = u_xlat9.xyz;
    }
    u_xlat0.xyz = u_xlat1.xyz * vec3(u_xlat36) + _WorldSpaceLightPos0.xyz;
    u_xlat36 = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat36 = max(u_xlat36, 0.00100000005);
    u_xlat36 = inversesqrt(u_xlat36);
    u_xlat0.xyz = vec3(u_xlat36) * u_xlat0.xyz;
    u_xlat36 = dot(u_xlat3.xyz, u_xlat2.xyz);
    u_xlat1.x = dot(u_xlat3.xyz, _WorldSpaceLightPos0.xyz);
    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
    u_xlat16_4.x = u_xlat0.x + u_xlat0.x;
    u_xlat16_4.x = u_xlat16_4.x * u_xlat0.x + -0.5;
    u_xlat16_16 = (-u_xlat1.x) + 1.0;
    u_xlat16_28 = u_xlat16_16 * u_xlat16_16;
    u_xlat16_28 = u_xlat16_28 * u_xlat16_28;
    u_xlat16_16 = u_xlat16_16 * u_xlat16_28;
    u_xlat16_16 = u_xlat16_4.x * u_xlat16_16 + 1.0;
    u_xlat16_28 = -abs(u_xlat36) + 1.0;
    u_xlat16_40 = u_xlat16_28 * u_xlat16_28;
    u_xlat16_40 = u_xlat16_40 * u_xlat16_40;
    u_xlat16_28 = u_xlat16_28 * u_xlat16_40;
    u_xlat16_4.x = u_xlat16_4.x * u_xlat16_28 + 1.0;
    u_xlat16_4.x = u_xlat16_4.x * u_xlat16_16;
    u_xlat12.x = u_xlat1.x * u_xlat16_4.x;
    u_xlat24 = abs(u_xlat36) + u_xlat1.x;
    u_xlat24 = u_xlat24 + 9.99999975e-06;
    u_xlat24 = 0.5 / u_xlat24;
    u_xlat24 = u_xlat24 * 0.999999881;
    u_xlat24 = max(u_xlat24, 9.99999975e-05);
    u_xlat24 = sqrt(u_xlat24);
    u_xlat24 = u_xlat1.x * u_xlat24;
    u_xlat16_4.xyw = u_xlat12.xxx * _LightColor0.xyz;
    u_xlat12.xyz = vec3(u_xlat24) * _LightColor0.xyz;
    u_xlat16_45 = (-u_xlat0.x) + 1.0;
    u_xlat16_11 = u_xlat16_45 * u_xlat16_45;
    u_xlat16_11 = u_xlat16_11 * u_xlat16_11;
    u_xlat16_45 = u_xlat16_45 * u_xlat16_11;
    u_xlat16_45 = u_xlat16_45 * 0.779083729 + 0.220916301;
    u_xlat0.xyz = u_xlat12.xyz * vec3(u_xlat16_45);
    u_xlat0.xyz = u_xlat16_4.xyw * vec3(0.389541864, 0.389541864, 0.389541864) + u_xlat0.xyz;
    u_xlat16_4.xyw = u_xlat16_9.xyz * vec3(0.720000029, 0.720000029, 0.720000029);
    u_xlat16_28 = u_xlat16_28 * -2.98023224e-08 + 0.220916301;
    u_xlat0.xyz = u_xlat16_4.xyw * vec3(u_xlat16_28) + u_xlat0.xyz;
    SV_Target0.xyz = u_xlat0.xyz;
    SV_Target0.w = 1.0;
    return;
}

#endif
