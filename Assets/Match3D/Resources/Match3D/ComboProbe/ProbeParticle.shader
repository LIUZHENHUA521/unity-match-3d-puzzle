Shader "Match3D/ProbeParticle" {
    SubShader {
        Tags { "Queue"="Overlay+1" "RenderType"="Transparent" }
        Pass {
            ZTest Always ZWrite Off Cull Off Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            struct v2f { float4 position:SV_POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            v2f vert(appdata v) { v2f o; o.position=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o; }
            fixed4 frag(v2f i):SV_Target { float d=length(i.uv*2-1);i.color.a*=1-smoothstep(.25,1,d);return i.color; }
            ENDCG
        }
    }
}
