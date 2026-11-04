// Mechanically ported saved WX GLES Outline program 1; active Medium branch.
Shader "Goose/SourceHighlight/Outline" {Properties{_MainTex("Texture",2D)="white"{} _Color("Color",Color)=(1,1,1,1)
 _OutlineColor("Outline",Color)=(0,0,0,1) _OutlineWidth("Width",Float)=.002 _OutlineVertexWidth("Vertex width",Float)=0 _ConstantWidth("Constant width",Float)=1 _OutlineDirection("Direction",Vector)=(0,0,0,0)
 _GlowColor("Glow color",Color)=(1,.964705944,.13333334,1) _Glow("Glow",Vector)=(.93,.0025,.75,.5) _Glow2("Glow2",Vector)=(.002,1,1,0) _GlowDirection("Direction",Vector)=(0,0,0,0)
 _OverlayColor("Overlay",Color)=(1,.92156863,.015686275,.5) _OverlayBackColor("Back",Color)=(1,1,1,1) _OverlayData("Data",Vector)=(1,.5,1,1) _OverlayHitPosData("Hit",Vector)=(0,0,0,0) _OverlayHitStartTime("Hit start",Float)=0}
SubShader{Tags {"Queue"="Transparent+121"} Pass{Cull Back ZWrite Off ZTest LEqual Blend SrcAlpha OneMinusSrcAlpha
Stencil {Ref 2 ReadMask 2 Comp NotEqual Pass Keep}
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
float2 V2(float a){return a.xx;} float2 V2(float2 a){return a;} float2 V2(float a,float b){return float2(a,b);}
float3 V3(float a){return a.xxx;} float3 V3(float3 a){return a;} float3 V3(float a,float b,float c){return float3(a,b,c);}
float4 V4(float a){return a.xxxx;} float4 V4(float4 a){return a;} float4 V4(float a,float b,float c,float d){return float4(a,b,c,d);}
// GLES clip Z is [-w,w]. Preserve its width equation on Metal/D3D reversed-Z targets.
float SourceClipZ(float4 p){
#if defined(UNITY_REVERSED_Z)
 return p.w-2*p.z;
#elif defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
 return p.z;
#else
 return 2*p.z-p.w;
#endif
}
float _OutlineWidth;
float _ConstantWidth;
float _OutlineVertexWidth;
float4 _OutlineDirection;
float4 _OutlineColor;
struct appdata {float4 vertex:POSITION;float3 normal:NORMAL;float2 uv:TEXCOORD0;};
struct v2f{float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float3 world:TEXCOORD1;float3 normal:TEXCOORD2;float4 color:COLOR0;};
v2f vert(appdata v){v2f o=(v2f)0;











float3 u_xlat0;
float4 u_xlat1;
float4 u_xlat2;
float2 u_xlat3;
float2 u_xlat6;

    u_xlat0.xyz = transpose(unity_WorldToObject)[1].xyz * transpose(UNITY_MATRIX_I_V)[0].yyy;
    u_xlat0.xyz = transpose(unity_WorldToObject)[0].xyz * transpose(UNITY_MATRIX_I_V)[0].xxx + u_xlat0.xyz;
    u_xlat0.xyz = transpose(unity_WorldToObject)[2].xyz * transpose(UNITY_MATRIX_I_V)[0].zzz + u_xlat0.xyz;
    u_xlat0.xyz = transpose(unity_WorldToObject)[3].xyz * transpose(UNITY_MATRIX_I_V)[0].www + u_xlat0.xyz;
    u_xlat0.x = dot(u_xlat0.xyz, v.normal.xyz);
    u_xlat1.xyz = transpose(unity_WorldToObject)[1].xyz * transpose(UNITY_MATRIX_I_V)[1].yyy;
    u_xlat1.xyz = transpose(unity_WorldToObject)[0].xyz * transpose(UNITY_MATRIX_I_V)[1].xxx + u_xlat1.xyz;
    u_xlat1.xyz = transpose(unity_WorldToObject)[2].xyz * transpose(UNITY_MATRIX_I_V)[1].zzz + u_xlat1.xyz;
    u_xlat1.xyz = transpose(unity_WorldToObject)[3].xyz * transpose(UNITY_MATRIX_I_V)[1].www + u_xlat1.xyz;
    u_xlat0.y = dot(u_xlat1.xyz, v.normal.xyz);
    u_xlat6.x = dot(u_xlat0.xy, u_xlat0.xy);
    u_xlat6.x = rsqrt(u_xlat6.x);
    u_xlat0.xy = u_xlat6.xx * u_xlat0.xy;
    u_xlat3.xy = u_xlat0.yy * transpose(UNITY_MATRIX_P)[1].xy;
    u_xlat0.xy = transpose(UNITY_MATRIX_P)[0].xy * u_xlat0.xx + u_xlat3.xy;
    u_xlat1.xyz = v.normal.xyz * V3(V3(_OutlineVertexWidth, _OutlineVertexWidth, _OutlineVertexWidth)) + v.vertex.xyz;
    u_xlat2 = u_xlat1.yyyy * transpose(unity_ObjectToWorld)[1];
    u_xlat2 = transpose(unity_ObjectToWorld)[0] * u_xlat1.xxxx + u_xlat2;
    u_xlat1 = transpose(unity_ObjectToWorld)[2] * u_xlat1.zzzz + u_xlat2;
    u_xlat1 = u_xlat1 + transpose(unity_ObjectToWorld)[3];
    u_xlat2 = u_xlat1.yyyy * transpose(UNITY_MATRIX_VP)[1];
    u_xlat2 = transpose(UNITY_MATRIX_VP)[0] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = transpose(UNITY_MATRIX_VP)[2] * u_xlat1.zzzz + u_xlat2;
    u_xlat1 = transpose(UNITY_MATRIX_VP)[3] * u_xlat1.wwww + u_xlat2;
    u_xlat6.x = (-SourceClipZ(u_xlat1)) + 2.0;
    u_xlat6.x = transpose(UNITY_MATRIX_P)[3].w * u_xlat6.x + SourceClipZ(u_xlat1);
    u_xlat6.x = u_xlat6.x + -2.0;
    u_xlat6.x = _ConstantWidth * u_xlat6.x + 2.0;
    u_xlat0.xy = u_xlat6.xx * u_xlat0.xy;
    u_xlat6.xy = u_xlat6.xx * _OutlineDirection.xy;
    u_xlat0.xy = u_xlat0.xy * V2(_OutlineWidth) + u_xlat6.xy;
    o.pos.xy = u_xlat0.xy + u_xlat1.xy;
    o.pos.zw = u_xlat1.zw;
    return o;
}
float4 frag(v2f i):SV_Target {float4 result;



    result = _OutlineColor;
    return result;
}
ENDCG
}}}
