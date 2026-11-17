// Pot implementation: depth-tested tint with texture detail and a triangle pulse.
// This pass needs no stencil and also serves the WeChat screen-halo path.
Shader "Pot/Highlight/Overlay" {
    Properties {
        _MainTex ("Item texture", 2D) = "white" {}
        _OverlayBackColor ("Item base color", Color) = (1,1,1,1)
        _OverlayColor ("Highlight tint and opacity", Color) = (0.2,0.6,1,0.5)
        _OverlayData ("Pulse speed and minimum", Vector) = (1,0.5,0,0)
    }
    SubShader {
        Tags { "Queue"="Transparent+121" "RenderType"="Transparent" }
        Pass {
            Cull Back ZWrite Off ZTest LEqual
            Offset -1, -1
            Blend SrcAlpha OneMinusSrcAlpha, Zero One
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float4 _MainTex_ST, _OverlayData;
            fixed4 _OverlayBackColor, _OverlayColor;
            struct Input { float4 position : POSITION; float2 uv : TEXCOORD0; };
            struct Interpolators { float4 position : SV_POSITION; float2 uv : TEXCOORD0; };
            Interpolators Vert(Input input) {
                Interpolators output;
                output.position = UnityObjectToClipPos(input.position);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }
            fixed4 Frag(Interpolators input) : SV_Target {
                fixed4 surface = tex2D(_MainTex, input.uv) * _OverlayBackColor;
                float phase = frac(_Time.y * max(_OverlayData.x, 0));
                float pulse = lerp(saturate(_OverlayData.y), 1, abs(phase * 2 - 1));
                // Retain fine texture differences without flattening dark items to black.
                float detail = lerp(0.6, 1, saturate(dot(surface.rgb, float3(0.299,0.587,0.114))));
                float3 tint = lerp(surface.rgb, _OverlayColor.rgb * detail, 0.85);
                return fixed4(tint, saturate(surface.a * _OverlayColor.a * pulse));
            }
            ENDCG
        }
    }
    Fallback Off
}
