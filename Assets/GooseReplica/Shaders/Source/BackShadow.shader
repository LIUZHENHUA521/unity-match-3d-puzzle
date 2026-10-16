// WX BackShadow: ForwardBase and ShadowCaster both Cull Front.
// Recovered constant Standard BRDF: albedo .5, metallic 0, smoothness 0, alpha 1.
// Built-in surface compiler supplies platform reflection probes / SH / shadow sampling.
Shader "Goose/Source/BackShadow" {
 SubShader { Tags { "RenderType"="Opaque" } Cull Front
 CGPROGRAM
 #pragma target 3.0
 #pragma surface surf Standard fullforwardshadows addshadow noforwardadd exclude_path:deferred
 struct Input { float3 worldPos; };
 void surf(Input i,inout SurfaceOutputStandard o) {
   o.Albedo=.5; o.Metallic=0; o.Smoothness=0; o.Alpha=1; o.Occlusion=1;
 }
 ENDCG
 }
}
