// Mechanically ported saved WX GLES Overlay program 1; active Medium branch.
Shader "Goose/SourceHighlight/Overlay" {Properties{_MainTex("Texture",2D)="white"{} _Color("Color",Color)=(1,1,1,1)
 _OutlineColor("Outline",Color)=(0,0,0,1) _OutlineWidth("Width",Float)=.002 _OutlineVertexWidth("Vertex width",Float)=0 _ConstantWidth("Constant width",Float)=1 _OutlineDirection("Direction",Vector)=(0,0,0,0)
 _GlowColor("Glow color",Color)=(1,.964705944,.13333334,1) _Glow("Glow",Vector)=(.93,.0025,.75,.5) _Glow2("Glow2",Vector)=(.002,1,1,0) _GlowDirection("Direction",Vector)=(0,0,0,0)
 _OverlayColor("Overlay",Color)=(1,.92156863,.015686275,.5) _OverlayBackColor("Back",Color)=(1,1,1,1) _OverlayData("Data",Vector)=(1,.5,1,1) _OverlayHitPosData("Hit",Vector)=(0,0,0,0) _OverlayHitStartTime("Hit start",Float)=0}
SubShader{Tags {"Queue"="Transparent+121"} Pass{Cull Back ZWrite Off ZTest LEqual Blend SrcAlpha OneMinusSrcAlpha
Stencil {Ref 4 ReadMask 4 Comp NotEqual Pass Keep}
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
float4 _MainTex_ST;
float4 _OverlayColor;
float4 _OverlayBackColor;
float4 _OverlayData;
float4 _OverlayHitPosData;
float _OverlayHitStartTime;
sampler2D _MainTex;
struct appdata {float4 vertex:POSITION;float3 normal:NORMAL;float2 uv:TEXCOORD0;};
struct v2f{float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float3 world:TEXCOORD1;float3 normal:TEXCOORD2;float4 color:COLOR0;};
v2f vert(appdata v){v2f o=(v2f)0;










float4 u_xlat0;
float4 u_xlat1;
float u_xlat6;

    u_xlat0 = v.vertex.yyyy * transpose(unity_ObjectToWorld)[1];
    u_xlat0 = transpose(unity_ObjectToWorld)[0] * v.vertex.xxxx + u_xlat0;
    u_xlat0 = transpose(unity_ObjectToWorld)[2] * v.vertex.zzzz + u_xlat0;
    u_xlat1 = u_xlat0 + transpose(unity_ObjectToWorld)[3];
    o.world.xyz = transpose(unity_ObjectToWorld)[3].xyz * v.vertex.www + u_xlat0.xyz;
    u_xlat0 = u_xlat1.yyyy * transpose(UNITY_MATRIX_VP)[1];
    u_xlat0 = transpose(UNITY_MATRIX_VP)[0] * u_xlat1.xxxx + u_xlat0;
    u_xlat0 = transpose(UNITY_MATRIX_VP)[2] * u_xlat1.zzzz + u_xlat0;
    u_xlat0 = transpose(UNITY_MATRIX_VP)[3] * u_xlat1.wwww + u_xlat0;
    o.pos = u_xlat0;
#if defined(UNITY_REVERSED_Z)
 o.pos.z+=0.0001;
#else
 o.pos.z-=0.0001;
#endif
    o.uv.xy = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    u_xlat0.x = dot(v.normal.xyz, transpose(unity_WorldToObject)[0].xyz);
    u_xlat0.y = dot(v.normal.xyz, transpose(unity_WorldToObject)[1].xyz);
    u_xlat0.z = dot(v.normal.xyz, transpose(unity_WorldToObject)[2].xyz);
    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat6 = rsqrt(u_xlat6);
    o.normal.xyz = V3(u_xlat6) * u_xlat0.xyz;
    return o;
}
float4 frag(v2f i):SV_Target {float4 result;










float3 u_xlat0;
float4 u_xlat1;
float4 u_xlat16_1;
float u_xlat16_2;
float2 u_xlat3;
bool u_xlatb3;
float u_xlat6;

    u_xlat0.xyz = i.world.xyz + (-_OverlayHitPosData.xyz);
    u_xlat0.x = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat0.x = sqrt(u_xlat0.x);
    u_xlat3.x = _Time.y + (-_OverlayHitStartTime);
    u_xlat3.x = min(u_xlat3.x, _OverlayHitPosData.w);
    u_xlat0.x = u_xlat3.x / u_xlat0.x;
    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
    u_xlat3.x = _Time.y * 1000.0;
    u_xlatb3 = u_xlat3.x>=(-u_xlat3.x);
    u_xlat3.xy = (bool(u_xlatb3)) ? V2(1000.0, 0.00100000005) : V2(-1000.0, -0.00100000005);
    u_xlat6 = u_xlat3.y * _Time.y;
    u_xlat6 = frac(u_xlat6);
    u_xlat3.x = u_xlat6 * u_xlat3.x;
    u_xlat3.x = u_xlat3.x * _OverlayData.x;
    u_xlat3.x = frac(u_xlat3.x);
    u_xlat3.x = (-u_xlat3.x) + 0.5;
    u_xlat16_1.x = (-_OverlayData.y) + 1.0;
    u_xlat16_1.x = u_xlat16_1.x + u_xlat16_1.x;
    u_xlat3.x = u_xlat16_1.x * abs(u_xlat3.x) + _OverlayData.y;
    u_xlat1 = tex2D(_MainTex, i.uv.xy);
    u_xlat16_1 = u_xlat1 * _OverlayBackColor;
    u_xlat16_1 = u_xlat16_1 * _OverlayColor + (-_OverlayColor);
    u_xlat16_1 = _OverlayData.zzzz * u_xlat16_1 + _OverlayColor;
    u_xlat16_2 = u_xlat3.x * u_xlat16_1.w;
    result.xyz = u_xlat16_1.xyz;
    u_xlat0.x = u_xlat0.x * u_xlat16_2;
    u_xlatb3 = 0.0<_OverlayHitPosData.w;
    result.w = (u_xlatb3) ? u_xlat0.x : u_xlat16_2;
    return result;
}
ENDCG
}}}
