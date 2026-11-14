using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public static class PotCapacityValidation {
 public static void Run(){int code=0;var lines=new List<string>();try{
  foreach(int layers in new[]{4,5,6})foreach(int count in new[]{3,150,300}){
   BoardEnvironment.Select("CountrysideStove");
   float[] heights={14,18,20,22,26,30};
   var tiles=Enumerable.Range(0,layers).SelectMany(l=>Enumerable.Range(0,count).Select(i=>new ChallengeTile{id=l*count+i,type=0,layer=l,height=heights[l]})).ToList();
   var plan=PotCapacityPlan.Build(tiles);plan.Apply();
   if(plan.Ceiling!=(layers==4?26:heights[layers-1]+2))throw new Exception("Configured ceiling mismatch");
   for(int i=0;i<layers;i++)if(plan.layerTops[i]!=heights[i])throw new Exception("Authored layer changed");
   if(SoftTransitionContainer.CylinderStart!=(layers==4?26:22))throw new Exception("Cylinder start mismatch");
   var shell=SoftTransitionContainer.Create();Physics.SyncTransforms();var wall=shell.GetComponentInChildren<MeshCollider>();
   foreach(float h in new[]{21f,23f,plan.Ceiling-.1f}){
    float expected=2.1f+.225f*(Mathf.Min(h,layers==4?26:22)-14);
    if(Mathf.Abs(SoftTransitionContainer.Apothem(h)-expected)>.001f)throw new Exception("Radius mismatch");
    if(!wall.Raycast(new Ray(SoftTransitionContainer.ToWorld(Vector3.up*h),Vector3.right),out var hit,10)||Mathf.Abs(hit.distance-expected)>.002f)throw new Exception("Wall mismatch");
   }
   if(wall.Raycast(new Ray(SoftTransitionContainer.ToWorld(Vector3.up*21),Vector3.up),out _,plan.Ceiling-21))throw new Exception("Internal disk");
   UnityEngine.Object.DestroyImmediate(shell);
   lines.Add($"layers={layers} countPerLayer={count} ceiling={plan.Ceiling} cylinderStart={plan.CylinderStart}");
  }
  BoardEnvironment.Select("CountrysideStove");if(SoftTransitionContainer.Height!=26||SoftTransitionContainer.CylinderStart!=26)throw new Exception("Reset failed");
  File.WriteAllText("../result.txt","PASS configured layer geometry checks\n"+string.Join("\n",lines));
 }catch(Exception e){code=1;File.WriteAllText("../result.txt",e+"\n"+string.Join("\n",lines));}finally{EditorApplication.Exit(code);}}
}
