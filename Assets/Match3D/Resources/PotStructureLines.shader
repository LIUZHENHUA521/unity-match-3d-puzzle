Shader "Pot/StructureLines" {
    SubShader {
        Pass {
            Cull Off ZWrite Off ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct v2f { float4 vertex : SV_POSITION; };
            v2f vert(float4 vertex : POSITION) { v2f o; o.vertex=UnityObjectToClipPos(vertex); return o; }
            fixed4 frag(v2f i) : SV_Target { return fixed4(.35,1,.28,.52); }
            ENDCG
        }
    }
}
