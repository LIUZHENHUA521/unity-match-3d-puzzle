// Pot implementation: a pulsing expanded shell, softly weighted toward grazing faces.
// _Glow=(opacity,width,unused,unused); _Glow2.xy=(outline width,animation speed).
Shader "Pot/Highlight/Glow" {
    Properties {
        _GlowColor ("Glow color", Color) = (0.4,0.8,1,1)
        _Glow ("Opacity and viewport width", Vector) = (0.93,0.0025,0,0)
        _Glow2 ("Animation controls", Vector) = (0,1,0,0)
        _GlowDirection ("Shell offset direction", Vector) = (0,0,0,0)
    }
    SubShader {
        Tags { "Queue"="Transparent+121" "RenderType"="Transparent" }
        Pass {
            Cull Back ZWrite Off ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha, Zero One
            Stencil { Ref 2 ReadMask 2 WriteMask 0 Comp NotEqual Pass Keep }
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"
            float4 _Glow, _Glow2, _GlowDirection;
            fixed4 _GlowColor;
            struct Input { float4 position : POSITION; float3 normal : NORMAL; };
            struct Interpolators {
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 toCamera : TEXCOORD1;
            };
            Interpolators Vert(Input input) {
                Interpolators output;
                float3 worldPosition = mul(unity_ObjectToWorld, input.position).xyz;
                output.normal = UnityObjectToWorldNormal(input.normal);
                // Orthographic and perspective cameras need different view directions.
                float3 parallelView = UNITY_MATRIX_V[2].xyz;
                output.toCamera = lerp(_WorldSpaceCameraPos.xyz - worldPosition, parallelView, unity_OrthoParams.w);
                float2 viewNormal = mul((float3x3)UNITY_MATRIX_V, output.normal).xy;
                viewNormal /= max(length(viewNormal), 0.001);
                float2 aspect = float2(1, _ScreenParams.x / max(_ScreenParams.y, 1));
                float breath = 1 + 0.25 * sin(_Time.y * _Glow2.y * 3);
                // Keep the soft shell outside the narrower black outline at its pulse minimum.
                float shellWidth = max(_Glow.y * breath * 2.5, _Glow2.x * 2.5);
                float2 offset = (viewNormal * aspect + _GlowDirection.xy) * max(shellWidth, 0);
                output.position = UnityObjectToClipPos(input.position);
                output.position.xy += offset * max(output.position.w, 0);
                return output;
            }
            fixed4 Frag(Interpolators input) : SV_Target {
                float facing = abs(dot(normalize(input.normal), normalize(input.toCamera)));
                float softness = lerp(0.2, 0.65, 1 - saturate(facing));
                return fixed4(_GlowColor.rgb, saturate(_Glow.x * _GlowColor.a * softness));
            }
            ENDCG
        }
    }
    Fallback Off
}
