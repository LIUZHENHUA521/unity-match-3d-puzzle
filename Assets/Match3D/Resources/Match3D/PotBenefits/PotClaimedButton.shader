Shader "Pot/UI/ClaimedButton"
{
    Properties { [PerRendererData] _MainTex ("Button", 2D) = "white" {} }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            float4 _ClipRect;
            struct ButtonVertex { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            struct ButtonPixel { float4 position : SV_POSITION; float2 local : TEXCOORD1; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            sampler2D _MainTex;
            ButtonPixel vert(ButtonVertex v)
            {
                ButtonPixel o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.local = v.vertex.xy;
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            fixed4 frag(ButtonPixel i) : SV_Target
            {
                float2 uv = i.uv;
                // These standalone button sprites have baked lettering. Extend the
                // empty left part of each scanline across the label, keeping the rim.
                if (uv.x > 0.18 && uv.x < 0.82 && uv.y > 0.19 && uv.y < 0.83)
                    uv.x = 0.17;
                fixed4 button = tex2D(_MainTex, uv);
                #ifdef UNITY_UI_CLIP_RECT
                button.a *= UnityGet2DClipping(i.local, _ClipRect);
                #endif
                return fixed4(Luminance(button.rgb).xxx, button.a) * i.color;
            }
            ENDCG
        }
    }
}
