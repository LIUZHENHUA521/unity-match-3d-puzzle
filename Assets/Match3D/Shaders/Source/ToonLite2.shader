// Generated from the saved WX GLES directional fragment; see Tools/LayeredPile/visual-camera-alignment.md.
Shader "Goose/Source/ToonLite2" { Properties { _ground1("_ground1",Float)=0.0
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
_texcoord("_texcoord",2D)="white"{}
__dirty("__dirty",Float)=1.0
_SourceLightDirection("Saved light direction",Vector)=(0,1,0,0) }
SubShader { Tags { "RenderType"="Opaque" } Pass { Tags { "LightMode"="ForwardBase" } CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
float4 _SourceLightDirection;



float4 _IndirectColor;
float4 _NormalMap_ST;
float _NormalScale;
float _BaseCellOffset;
float _BaseCellSharpness;
float4 _LightColor;
float4 _BaseColor_ST;
float4 _BaseTint;
float4 _AOTex_ST;
float _AOScale;
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
sampler2D _NormalMap;
sampler2D _BaseColor;
sampler2D _AOTex;
sampler2D _Matcap;
sampler2D _RoughnessMap;





float2 V2(float a){return a.xx;} float2 V2(float2 a){return a;} float2 V2(float a,float b){return float2(a,b);}
float3 V3(float a){return a.xxx;} float3 V3(float3 a){return a;} float3 V3(float a,float b,float c){return float3(a,b,c);}
float4 V4(float a){return a.xxxx;} float4 V4(float4 a){return a;} float4 V4(float a,float b,float c,float d){return float4(a,b,c,d);}
struct appdata {float4 vertex:POSITION;float3 normal:NORMAL;float4 tangent:TANGENT;float2 uv:TEXCOORD0;};
struct v2f {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float4 t0:TEXCOORD1;float4 t1:TEXCOORD2;float4 t2:TEXCOORD3;SHADOW_COORDS(4)};
float4 _texcoord_ST;
v2f vert(appdata v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv*_texcoord_ST.xy+_texcoord_ST.zw;
float3 p=mul(unity_ObjectToWorld,v.vertex).xyz,n=UnityObjectToWorldNormal(v.normal),t=UnityObjectToWorldDir(v.tangent.xyz),b=cross(n,t)*v.tangent.w*unity_WorldTransformParams.w;
o.t0=float4(t.x,b.x,n.x,p.x);o.t1=float4(t.y,b.y,n.y,p.y);o.t2=float4(t.z,b.z,n.z,p.z);TRANSFER_SHADOW(o);return o;}
float4 frag(v2f i):SV_Target {
float2 vs_TEXCOORD0=i.uv;float4 vs_TEXCOORD1=i.t0,vs_TEXCOORD2=i.t1,vs_TEXCOORD3=i.t2,SV_Target0;

float3 u_xlat0;
float3 u_xlat1;
float3 u_xlat16_2;
float3 u_xlat16_3;
float3 u_xlat4;
float2 u_xlat5;
float3 u_xlat7;
bool u_xlatb7;
float u_xlat18;
float u_xlat16_20;

UNITY_LIGHT_ATTENUATION(sourceAttenuation,i,float3(i.t0.w,i.t1.w,i.t2.w));

    u_xlat0.xy = vs_TEXCOORD0.xy * _BaseColor_ST.xy + _BaseColor_ST.zw;
    u_xlat0.xyz = tex2D(_BaseColor, u_xlat0.xy).xyz;
    u_xlat1.xyz = u_xlat0.xyz * V3(0.305306017, 0.305306017, 0.305306017) + V3(0.682171106, 0.682171106, 0.682171106);
    u_xlat1.xyz = u_xlat0.xyz * u_xlat1.xyz + V3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xyz;
    u_xlat1.xyz = _BaseTint.xyz * V3(0.305306017, 0.305306017, 0.305306017) + V3(0.682171106, 0.682171106, 0.682171106);
    u_xlat1.xyz = _BaseTint.xyz * u_xlat1.xyz + V3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat1.xyz = u_xlat1.xyz * _BaseTint.xyz;
    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xyz;
    u_xlat1.xy = vs_TEXCOORD0.xy * _AOTex_ST.xy + _AOTex_ST.zw;
    u_xlat18 = tex2D(_AOTex, u_xlat1.xy).x;
    u_xlat1.x = u_xlat18 * 0.305306017 + 0.682171106;
    u_xlat1.x = u_xlat18 * u_xlat1.x + 0.0125228781;
    u_xlat18 = u_xlat18 * u_xlat1.x + -1.0;
    u_xlat18 = _AOScale * u_xlat18 + 1.0;
    u_xlat0.xyz = V3(u_xlat18) * u_xlat0.xyz;
    u_xlat1.xy = vs_TEXCOORD0.xy * _NormalMap_ST.xy + _NormalMap_ST.zw;
    u_xlat1.xyz = tex2D(_NormalMap, u_xlat1.xy).xyw;
    u_xlat16_2.x = dot(u_xlat1.xx, u_xlat1.zz);
    u_xlat16_2.y = u_xlat1.y + u_xlat1.y;
    u_xlat16_2.xy = u_xlat16_2.xy + V2(-1.0, -1.0);
    u_xlat16_2.xy = u_xlat16_2.xy * V2(_NormalScale);
    u_xlat16_20 = dot(u_xlat16_2.xy, u_xlat16_2.xy);
    u_xlat16_20 = min(u_xlat16_20, 1.0);
    u_xlat16_20 = (-u_xlat16_20) + 1.0;
    u_xlat16_2.z = sqrt(u_xlat16_20);
    u_xlat16_3.x = dot(vs_TEXCOORD1.xyz, u_xlat16_2.xyz);
    u_xlat16_3.y = dot(vs_TEXCOORD2.xyz, u_xlat16_2.xyz);
    u_xlat16_3.z = dot(vs_TEXCOORD3.xyz, u_xlat16_2.xyz);
    u_xlat16_2.x = dot(u_xlat16_3.xyz, u_xlat16_3.xyz);
    u_xlat16_2.x = rsqrt(u_xlat16_2.x);
    u_xlat16_2.xyz = u_xlat16_2.xxx * u_xlat16_3.xyz;
    u_xlat18 = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
    u_xlat18 = rsqrt(u_xlat18);
    u_xlat1.xyz = V3(u_xlat18) * _WorldSpaceLightPos0.xyz;
    u_xlat18 = dot(u_xlat16_2.xyz, u_xlat1.xyz);
    u_xlat1.x = u_xlat18 + _BaseCellOffset;
    u_xlat18 = u_xlat18 + _HighLightOffset;
    u_xlat18 = u_xlat18 / _HighLightSmooth;
    u_xlat18 = clamp(u_xlat18, 0.0, 1.0);
    u_xlat1.x = u_xlat1.x / _BaseCellSharpness;
    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
    u_xlatb7 = _LightColor0.w==0.0;
    u_xlat1.x = (u_xlatb7) ? 0.0 : u_xlat1.x;
    u_xlat1.x *= sourceAttenuation;
    u_xlat7.x = (-u_xlat1.x) + 1.0;
    u_xlat7.xyz = u_xlat7.xxx * _ShadowAddColor.xyz;
    u_xlat4.xyz = u_xlat1.xxx * _LightColor.xyz + _IndirectColor.xyz;
    u_xlat1.x = u_xlat1.x + -1.0;
    u_xlat5.xy = V2(_HighLightShadowMask, _MetcapShadowMask) * u_xlat1.xx + V2(1.0, 1.0);
    u_xlat0.xyz = u_xlat4.xyz * u_xlat0.xyz + u_xlat7.xyz;
    u_xlat1.x = u_xlat18 * u_xlat18 + (-u_xlat18);
    u_xlat18 = _HighLightPing * u_xlat1.x + u_xlat18;
    u_xlat1.xyz = V3(u_xlat18) * _HighLightColor.xyz;
    u_xlat0.xyz = u_xlat1.xyz * u_xlat5.xxx + u_xlat0.xyz;
    u_xlat1.xy = u_xlat16_2.yy * transpose(UNITY_MATRIX_V)[1].xy;
    u_xlat1.xy = transpose(UNITY_MATRIX_V)[0].xy * u_xlat16_2.xx + u_xlat1.xy;
    u_xlat1.xy = transpose(UNITY_MATRIX_V)[2].xy * u_xlat16_2.zz + u_xlat1.xy;
    u_xlat1.xy = u_xlat1.xy * V2(V2(_ReflectScale, _ReflectScale));
    u_xlat1.xy = u_xlat1.xy * V2(0.5, 0.5) + _ReflectOffset.xy;
    u_xlat1.xy = u_xlat1.xy + V2(0.5, 0.5);
    u_xlat1.xyz = tex2D(_Matcap, u_xlat1.xy).xyz;
    u_xlat4.xyz = u_xlat1.xyz * V3(0.305306017, 0.305306017, 0.305306017) + V3(0.682171106, 0.682171106, 0.682171106);
    u_xlat4.xyz = u_xlat1.xyz * u_xlat4.xyz + V3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat1.xyz = u_xlat1.xyz * u_xlat4.xyz;
    u_xlat1.xyz = u_xlat1.xyz * V3(V3(_MatcapScale, _MatcapScale, _MatcapScale));
    u_xlat1.xyz = u_xlat1.xyz * u_xlat5.yyy;
    u_xlat1.xyz = u_xlat1.xyz * _MatcapColor.xyz;
    u_xlat4.xy = vs_TEXCOORD0.xy * _RoughnessMap_ST.xy + _RoughnessMap_ST.zw;
    u_xlat18 = tex2D(_RoughnessMap, u_xlat4.xy).x;
    u_xlat18 = _RoughnessScale * (-u_xlat18) + 1.0;
    u_xlat0.xyz = u_xlat1.xyz * V3(u_xlat18) + u_xlat0.xyz;
    u_xlat0.xyz = u_xlat0.xyz + u_xlat0.xyz;
    u_xlat16_2.xyz = max(u_xlat0.xyz, V3(0.0, 0.0, 0.0));
    u_xlat0.xyz = log2(u_xlat16_2.xyz);
    u_xlat0.xyz = u_xlat0.xyz * V3(0.416666657, 0.416666657, 0.416666657);
    u_xlat0.xyz = exp2(u_xlat0.xyz);
    u_xlat0.xyz = u_xlat0.xyz * V3(1.05499995, 1.05499995, 1.05499995) + V3(-0.0549999997, -0.0549999997, -0.0549999997);
    u_xlat0.xyz = max(u_xlat0.xyz, V3(0.0, 0.0, 0.0));
    SV_Target0.xyz = u_xlat0.xyz;
    SV_Target0.w = 1.0;
    return SV_Target0;

}
ENDCG }
Pass { Name "ShadowCaster" Tags { "LightMode"="ShadowCaster" } ZWrite On ZTest LEqual Cull Back
CGPROGRAM
#pragma vertex shadowVert
#pragma fragment shadowFrag
#pragma multi_compile_shadowcaster
#include "UnityCG.cginc"
struct shadowV2f { V2F_SHADOW_CASTER; };
shadowV2f shadowVert(appdata_base v) { shadowV2f o; TRANSFER_SHADOW_CASTER_NORMALOFFSET(o) return o; }
float4 shadowFrag(shadowV2f i):SV_Target { SHADOW_CASTER_FRAGMENT(i) }
ENDCG
} }
}
