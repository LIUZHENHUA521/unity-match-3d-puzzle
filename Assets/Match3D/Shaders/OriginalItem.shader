Shader "Goose/Original Item" {
 Properties {_MainTex("Original base color",2D)="white"{} _AO("Original occlusion",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) _HasAO("Use AO",Float)=0 _SelectionGlow("Selection glow",Range(0,1))=0 _SelectionPulse("Selection pulse",Range(0,1))=1 _OutlinePixels("Selected outline pixels",Float)=1.2}
 SubShader {
  Tags {"RenderType"="Opaque"}
  // Inverted hull: stable black silhouette, depth-tested against the pile.
  Pass {
   Name "SELECTION_OUTLINE"
   Tags {"LightMode"="Always"}
   Cull Front
   ZWrite On
   ZTest LEqual
   CGPROGRAM
   #pragma vertex outlineVert
   #pragma fragment outlineFrag
   #include "UnityCG.cginc"
   float _SelectionGlow, _OutlinePixels;
   struct outlineInput {float4 vertex:POSITION; float3 normal:NORMAL;};
   struct outlineOutput {float4 pos:SV_POSITION;};
   outlineOutput outlineVert(outlineInput v) {
    outlineOutput o;
    o.pos=UnityObjectToClipPos(v.vertex);
    float3 normal=mul((float3x3)UNITY_MATRIX_V,UnityObjectToWorldNormal(v.normal));
    float2 direction=mul((float2x2)UNITY_MATRIX_P,normal.xy)*_ScreenParams.xy;
    direction/=max(length(direction),.0001);
    o.pos.xy+=direction*(2*_OutlinePixels/_ScreenParams.xy)*o.pos.w;
    return o;
   }
   fixed4 outlineFrag(outlineOutput i):SV_Target {clip(_SelectionGlow-.5);return fixed4(.015,.012,.01,1);}
   ENDCG
  }
  CGPROGRAM
  #pragma surface surf Lambert fullforwardshadows addshadow
  sampler2D _MainTex,_AO;fixed4 _Color;float _HasAO;float _SelectionGlow;float _SelectionPulse;
  struct Input {float2 uv_MainTex;float3 viewDir;};
  void surf(Input IN,inout SurfaceOutput o){fixed3 base=tex2D(_MainTex,IN.uv_MainTex).rgb*_Color.rgb;float ao=lerp(1,tex2D(_AO,IN.uv_MainTex).r,_HasAO);o.Albedo=base*.72;float rim=pow(1-saturate(dot(normalize(IN.viewDir),float3(0,0,1))),2);o.Emission=base*.28*ao+_SelectionGlow*lerp(.35,1,_SelectionPulse)*fixed3(1,.93,.78)*(.025+rim*.12);o.Alpha=1;}
  ENDCG
 }
 Fallback "Diffuse"
}
