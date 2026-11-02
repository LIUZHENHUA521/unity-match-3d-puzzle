using UnityEngine;
using UnityEngine.UI;
// Geometry-only stencil for the downloaded avatar; no extra mask texture required.
[RequireComponent(typeof(CanvasRenderer))]
public sealed class ReplicaCircleGraphic : MaskableGraphic {
 protected override void OnPopulateMesh(VertexHelper mesh){
  mesh.Clear();var rect=GetPixelAdjustedRect();var center=rect.center;var vertex=UIVertex.simpleVert;vertex.color=color;vertex.position=center;vertex.uv0=new Vector2(.5f,.5f);mesh.AddVert(vertex);
  const int segments=64;
  for(int i=0;i<=segments;i++){float angle=i*Mathf.PI*2/segments;var direction=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));vertex.position=center+Vector2.Scale(direction,rect.size*.5f);vertex.uv0=direction*.5f+Vector2.one*.5f;mesh.AddVert(vertex);if(i>0)mesh.AddTriangle(0,i+1,i);}
 }
}
