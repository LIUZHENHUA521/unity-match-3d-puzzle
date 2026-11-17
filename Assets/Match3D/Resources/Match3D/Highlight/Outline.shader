// Pot implementation: screen-space normal expansion plus a caller-supplied offset.
// Property names and stencil mask are the existing ItemHighlight integration contract.
Shader "Pot/Highlight/Outline" {
    Properties {
        _OutlineColor ("Outline color", Color) = (0,0,0,1)
        _OutlineWidth ("Viewport width", Float) = 0.002
        _OutlineDirection ("Viewport offset", Vector) = (0,0,0,0)
    }
    SubShader {
        Tags { "Queue"="Transparent+121" "RenderType"="Transparent" }
        Pass {
            Cull Back ZWrite Off ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha, Zero One
            Stencil { Ref 2 ReadMask 2 WriteMask 0 Comp NotEqual Pass Keep }
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"
            float _OutlineWidth;
            float4 _OutlineDirection;
            fixed4 _OutlineColor;
            struct Input { float4 position : POSITION; float3 normal : NORMAL; };
            float4 Vert(Input input) : SV_POSITION {
                float4 clip = UnityObjectToClipPos(input.position);
                float3 worldNormal = UnityObjectToWorldNormal(input.normal);
                float2 viewNormal = mul((float3x3)UNITY_MATRIX_V, worldNormal).xy;
                // The length guard also handles faces pointing directly at the camera.
                viewNormal /= max(length(viewNormal), 0.001);
                float2 aspect = float2(1, _ScreenParams.x / max(_ScreenParams.y, 1));
                float2 offset = viewNormal * aspect * max(_OutlineWidth, 0) + _OutlineDirection.xy;
                clip.xy += offset * max(clip.w, 0);
                return clip;
            }
            fixed4 Frag() : SV_Target { return _OutlineColor; }
            ENDCG
        }
    }
    Fallback Off
}
