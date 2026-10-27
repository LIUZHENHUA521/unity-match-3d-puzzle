Shader "GooseReplica/Soft Contact Shadow"
{
    Properties
    {
        _Color ("Shadow Color / Opacity", Color) = (0.12,0.08,0.04,0.35)
        _Softness ("Edge Softness", Range(0.01,1)) = 0.35
    }
    SubShader
    {
        Tags { "Queue"="Transparent+1" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; };
            fixed4 _Color;
            float _Softness;
            v2f vert(appdata v) { v2f o; o.vertex=UnityObjectToClipPos(v.vertex); o.uv=v.uv; return o; }
            fixed4 frag(v2f i):SV_Target
            {
                float radius=length(i.uv*2-1);
                fixed4 color=_Color;
                color.a*=1-smoothstep(1-_Softness,1,radius);
                return color;
            }
            ENDCG
        }
    }
}
