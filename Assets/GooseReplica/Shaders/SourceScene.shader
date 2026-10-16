Shader "Goose/Source Scene" {
 Properties{_MainTex("Base",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) _AO("AO",2D)="white"{} _HasAO("Has AO",Float)=0 _AOScale("AO scale",Float)=1 _Emission("Emission",2D)="black"{} _HasEmission("Has emission",Float)=0 _Cull("Cull",Float)=2}
 SubShader{Tags{"RenderType"="Opaque"} Pass{Cull [_Cull]
CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex,_AO,_Emission;float4 _MainTex_ST,_Color;float _HasAO,_AOScale,_HasEmission;
 struct a{float4 vertex:POSITION;float2 uv:TEXCOORD0;};struct v{float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float2 raw:TEXCOORD1;};
 v vert(a i){v o;o.pos=UnityObjectToClipPos(i.vertex);o.uv=TRANSFORM_TEX(i.uv,_MainTex);o.raw=i.uv;return o;}
 fixed4 frag(v i):SV_Target{fixed4 c=tex2D(_MainTex,i.uv)*_Color;c.rgb*=lerp(1,lerp(1,tex2D(_AO,i.raw).r,_AOScale),_HasAO);c.rgb+=tex2D(_Emission,i.raw).rgb*_HasEmission;c.a=1;return c;}
 ENDCG}}
}
