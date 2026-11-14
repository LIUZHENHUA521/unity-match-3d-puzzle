using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public static class PotFlatItemValidation {
 const string Output="../results";
 public static void Run(){
  Directory.CreateDirectory(Output);var lines=new List<string>();
  try{
   EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
   Physics.autoSimulation=false;BoardPhysicsSettings.Begin();ItemColliderGeometry.ConfigureLayers();
   var pan=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/Resources/Match3D/Scenes/CountrysideStove.prefab"));
   Match3DThemePresentation.AddPanCollision(pan);SoftTransitionContainer.ConfigureHeight(26);SoftTransitionContainer.Create();
   var cam=new GameObject("Test camera").AddComponent<Camera>();cam.transform.SetPositionAndRotation(new Vector3(0,30,0),Quaternion.Euler(90,0,0));cam.orthographic=true;cam.orthographicSize=5;cam.nearClipPlane=.1f;cam.farClipPlane=100;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=Color.gray;
   RenderSettings.ambientLight=Color.white;var light=new GameObject("Test light").AddComponent<Light>();light.type=LightType.Directional;light.intensity=1;light.transform.rotation=Quaternion.Euler(60,-30,0);
   Physics.SyncTransforms();var pc=pan.transform.Find("Grab3D Frying Pan").GetComponentInChildren<MeshCollider>();pc.Raycast(new Ray(new Vector3(.05f,10,-.04f),Vector3.down),out var hit,20);lines.Add("pan floor="+hit.point.ToString("F5"));
   foreach(string id in new[]{"Party_Waffle","Party_Pizza","Party_PancakesStack"}){
    var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/Generated/ModelAdapter/"+id+"/"+id+".prefab");
    foreach(float scale in new[]{.5f,1f,1.25f})foreach(string placement in new[]{"drop","flat_on_pan","fit_old_floor"}){
     var obj=UnityEngine.Object.Instantiate(prefab);obj.transform.localScale*=BoardLayoutSettings.ModelScale(prefab)/BoardLayoutSettings.ModelSizeMultiplier*scale;
     obj.transform.rotation=Quaternion.identity;obj.transform.position=new Vector3(.05f,0,-.04f);
     var vertices=obj.GetComponentsInChildren<MeshFilter>().SelectMany(m=>m.sharedMesh.vertices.Select(v=>obj.transform.InverseTransformPoint(m.transform.TransformPoint(v)))).ToArray();
     Func<float> bottom=()=>vertices.Min(v=>obj.transform.TransformPoint(v).y);Func<float> top=()=>vertices.Max(v=>obj.transform.TransformPoint(v).y);
     float thickness=top()-bottom();
     obj.transform.position+=Vector3.up*((placement=="drop"?3:placement=="flat_on_pan"?hit.point.y+.025f:.04f)-bottom());
     if(placement=="fit_old_floor")SoftTransitionContainer.Fit(obj);
     var rb=obj.GetComponent<Rigidbody>();rb.isKinematic=false;BoardPhysicsSettings.Apply(rb);float initialBottom=bottom();
     Physics.SyncTransforms();for(int i=0;i<600;i++)Physics.Simulate(.02f);
     Physics.SyncTransforms();bool pick=Physics.Raycast(new Vector3(rb.position.x,20,rb.position.z),Vector3.down,out var itemHit,30,1<<ItemColliderGeometry.PickLayer)&&itemHit.rigidbody==rb;
     lines.Add(id+" scale="+scale+" placement="+placement+" thickness="+thickness.ToString("F5")+" initialBottom="+initialBottom.ToString("F5")+" finalBottom="+bottom().ToString("F5")+" finalTop="+top().ToString("F5")+" position="+rb.position.ToString("F5")+" mode="+rb.collisionDetectionMode+" pick="+pick);
     {var rt=new RenderTexture(800,800,24);cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;var tex=new Texture2D(800,800,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,800,800),0,0);tex.Apply();File.WriteAllBytes(Output+"/"+id+"-"+scale.ToString("F2")+"-"+placement+".png",tex.EncodeToPNG());cam.targetTexture=null;RenderTexture.active=null;UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(tex);}
     UnityEngine.Object.DestroyImmediate(obj);
    }
   }
   File.WriteAllLines(Output+"/report.txt",lines);EditorApplication.Exit(0);
  }catch(Exception ex){lines.Add(ex.ToString());File.WriteAllLines(Output+"/report.txt",lines);EditorApplication.Exit(1);}
 }
}
