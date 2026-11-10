Shader "Goose/SourceHighlight/ScreenHalo" {
    Properties { _MainTex("Silhouette",2D)="black"{} }
    SubShader { Pass {
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha, Zero One
        CGPROGRAM
        #pragma target 3.0
        #pragma vertex vert_img
        #pragma fragment frag
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        float4 _MainTex_TexelSize, _HaloPixels;
        fixed4 _HaloColor, _EdgeColor;
        float Ring(float2 uv,float radius) {
            float2 d=abs(_MainTex_TexelSize.xy)*radius;
            float a=tex2D(_MainTex,uv+float2(d.x,0)).a;
            a=max(a,tex2D(_MainTex,uv-float2(d.x,0)).a);
            a=max(a,tex2D(_MainTex,uv+float2(0,d.y)).a);
            a=max(a,tex2D(_MainTex,uv-float2(0,d.y)).a);
            d*=.70710678;
            a=max(a,tex2D(_MainTex,uv+d).a);
            a=max(a,tex2D(_MainTex,uv-d).a);
            a=max(a,tex2D(_MainTex,uv+float2(d.x,-d.y)).a);
            return max(a,tex2D(_MainTex,uv+float2(-d.x,d.y)).a);
        }
        fixed4 frag(v2f_img i) : SV_Target {
            float inside=tex2D(_MainTex,i.uv).a;
            float edge=Ring(i.uv,_HaloPixels.x);
            float mid=Ring(i.uv,lerp(_HaloPixels.x,_HaloPixels.y,.5));
            float outer=Ring(i.uv,_HaloPixels.y);
            float halo=max(edge,max(mid,outer));
            float alpha=(1-inside)*saturate(edge*.9+mid*.28*_HaloPixels.z+halo*.15*_HaloPixels.z);
            return fixed4(lerp(_HaloColor.rgb,_EdgeColor.rgb,edge),alpha);
        }
        ENDCG
    } }
}
