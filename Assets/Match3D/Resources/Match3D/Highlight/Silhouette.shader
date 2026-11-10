Shader "Goose/SourceHighlight/Silhouette" {
    SubShader { Pass {
        Cull Back ZWrite Off ZTest Always
        CGPROGRAM
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
        float4 vert(float4 vertex : POSITION) : SV_POSITION { return UnityObjectToClipPos(vertex); }
        fixed4 frag() : SV_Target { return 1; }
        ENDCG
    } }
}
