// Procedural soft disc on a Unity quad; no image or extracted shader dependency.
Shader "Pot/Contact Shadow" {
    Properties {
        _Color ("Shadow tint", Color) = (0.12, 0.08, 0.04, 0.35)
        _Softness ("Edge softness", Range(0.01, 1)) = 0.35
    }
    SubShader {
        Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "UnityCG.cginc"
            struct Input { float4 position : POSITION; float2 uv : TEXCOORD0; };
            struct Interpolated { float4 position : SV_POSITION; float2 uv : TEXCOORD0; };
            fixed4 _Color;
            float _Softness;
            Interpolated Vertex(Input input) {
                Interpolated output;
                output.position = UnityObjectToClipPos(input.position);
                output.uv = input.uv;
                return output;
            }
            fixed4 Fragment(Interpolated input) : SV_Target {
                float distanceFromCenter = length(input.uv * 2.0 - 1.0);
                fixed4 shadow = _Color;
                shadow.a *= 1.0 - smoothstep(1.0 - max(_Softness, 0.01), 1.0, distanceFromCenter);
                return shadow;
            }
            ENDCG
        }
    }
}
