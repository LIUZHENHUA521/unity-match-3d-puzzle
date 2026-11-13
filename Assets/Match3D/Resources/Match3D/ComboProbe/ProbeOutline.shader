Shader "Match3D/ProbeOutline" {
    Properties { _PotOpacity ("Opacity", Float) = 1 }
    SubShader {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Pass {
            ZTest Always ZWrite Off Cull Back Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float _PotOpacity;
            struct appdata { float4 vertex:POSITION; float3 normal:NORMAL; };
            struct v2f { float4 position:SV_POSITION; float3 normal:TEXCOORD0; float3 view:TEXCOORD1; };
            v2f vert(appdata v) {
                v2f o; o.position=UnityObjectToClipPos(v.vertex);
                o.normal=UnityObjectToWorldNormal(v.normal);
                o.view=UnityWorldSpaceViewDir(mul(unity_ObjectToWorld,v.vertex).xyz); return o;
            }
            fixed4 frag(v2f i):SV_Target {
                float rim=pow(1-saturate(abs(dot(normalize(i.normal),normalize(i.view)))),2);
                return fixed4(.25+rim*.6,.8+rim*.2,1,(.12+rim*.8)*_PotOpacity);
            }
            ENDCG
        }
    }
}
