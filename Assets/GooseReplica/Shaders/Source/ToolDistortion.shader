// Mechanical translation of Unlit/NiuQu_UV_Alpha Metal vertex/fragment and saved render state.
Shader "Goose/Source/ToolDistortion" {
Properties {
_V("Distortion",Float)=.2
_Niuqu("Noise",2D)="white"{}
_Niuqu_U("Noise U",Float)=1
_Niuqu_V("Noise V",Float)=1
_Tex("Texture",2D)="white"{}
_Tex_Color("Texture color",Color)=(1,1,1,1)
_Tex_Color_Scale("Color scale",Float)=0
_Tex_U("Texture U",Float)=1
_Tex_V("Texture V",Float)=1
_Mask("Mask",2D)="white"{}
}
SubShader { Tags {"RenderType"="Opaque"} LOD 100
Pass { Name "Unlit" Tags {"LightMode"="ForwardBase"} Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off ZTest LEqual
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
sampler2D _Niuqu,_Tex,_Mask;half4 _Niuqu_ST,_Tex_ST,_Mask_ST,_Tex_Color;float _Niuqu_U,_Niuqu_V,_Tex_U,_Tex_V;half _V,_Tex_Color_Scale;
struct appdata {float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};
struct v2f {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float4 color:COLOR;};
v2f vert(appdata v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
half4 frag(v2f i):SV_Target {
 float2 noiseUV=i.uv*_Niuqu_ST.xy+_Niuqu_ST.zw;
 half2 animatedNoise=(float2(_Niuqu_U,_Niuqu_V)*_Time.yy+noiseUV);
 half n=tex2D(_Niuqu,animatedNoise).x;
 float2 texUV=i.uv*_Tex_ST.xy+_Tex_ST.zw;
 half2 animatedTex=float2(_Tex_U,_Tex_V)*_Time.yy+texUV;
 half4 tex=tex2D(_Tex,n.xx*_V+animatedTex);
 half4 mask=tex2D(_Mask,i.uv*_Mask_ST.xy+_Mask_ST.zw);
 return (float4(tex)*float4(_Tex_Color)*float4(mask)*i.color*float4(_Tex_Color_Scale,_Tex_Color_Scale,_Tex_Color_Scale,_Tex_Color_Scale));
}
ENDCG
}
}
}
