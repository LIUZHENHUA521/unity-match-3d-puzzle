// Saved HighlightPlus/Geometry/Mask and ClearStencil state, only used bit 2.
Shader "Goose/SourceHighlight/Mask" {
 SubShader {
  // j20234 -> j39438: active glowVisibility=AlwaysOnTop requires an unconditional silhouette mask.
  Pass {Name "MASK" Cull Back ZWrite Off ZTest Always ColorMask 0
   Stencil {Ref 2 ReadMask 2 WriteMask 2 Comp Always Pass Replace}
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   float4 vert(float4 p:POSITION):SV_POSITION{
    float4 clip=UnityObjectToClipPos(p);
    // Original GLES Mask program applies the same -0.0001 clip-Z bias as Overlay.
#if defined(UNITY_REVERSED_Z)
    clip.z+=0.0001;
#else
    clip.z-=0.0001;
#endif
    return clip;
   }
   float4 frag():SV_Target{return 0;}
   ENDCG
  }
  Pass {Name "CLEAR" Cull Off ZWrite Off ZTest Always ColorMask 0
   Stencil {Ref 0 ReadMask 2 WriteMask 2 Comp Always Pass Replace}
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   float4 vert(float4 p:POSITION):SV_POSITION{return float4(p.xy,0,1);}
   float4 frag():SV_Target{return 0;}
   ENDCG
  }
 }
}
