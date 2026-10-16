#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEditor;

// Renders every imported prefab to exercise otherwise-unseen special materials; no physics changes.
public static class SourceItemRenderValidation {
 [Serializable] class Data {public Model[] models;}
 [Serializable] class Model {public string name,group;}
 static void Check(bool ok,string message){if(!ok)throw new Exception("ITEM_RENDER: "+message);}
 public static void Run(){
  string report="";var scene=SceneManager.CreateScene("Item material render validation");
  var rig=new GameObject("Source material lighting");SceneManager.MoveGameObjectToScene(rig,scene);var lighting=rig.AddComponent<ReplicaSourceLighting>();lighting.Begin();lighting.SourceLight.cullingMask=1<<30;
  var data=SourceModelJson.Read<Data>(File.ReadAllText("Assets/GooseReplica/OriginalModels/models.json"));int rows=Mathf.CeilToInt(data.models.Length/8f),height=rows*120;
  var rt=new RenderTexture(960,height,24,RenderTextureFormat.ARGB32);var image=new Texture2D(960,height,TextureFormat.RGB24,false);var oldActive=RenderTexture.active;
  var objects=new System.Collections.Generic.List<GameObject>();
  try {
   Directory.CreateDirectory("visual-captures");
   var camObj=new GameObject("All item materials camera");SceneManager.MoveGameObjectToScene(camObj,scene);var camera=camObj.AddComponent<Camera>();camera.transform.position=new Vector3(0,0,-30);camera.orthographic=true;camera.orthographicSize=rows;camera.aspect=8f/rows;camera.nearClipPlane=.1f;camera.farClipPlane=100;camera.cullingMask=1<<30;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.22f,.25f,.28f,1);camera.targetTexture=rt;
   string mapping="Index,source prefab (left to right, top to bottom)\n";
   for(int i=0;i<data.models.Length;i++){
    var m=data.models[i];var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GooseReplica/OriginalModels/Prefabs/"+m.group+"/"+m.name+".prefab");var item=UnityEngine.Object.Instantiate(prefab);SceneManager.MoveGameObjectToScene(item,scene);objects.Add(item);
    foreach(var tr in item.GetComponentsInChildren<Transform>(true))tr.gameObject.layer=30;
    foreach(var body in item.GetComponentsInChildren<Rigidbody>(true)){body.isKinematic=true;body.detectCollisions=false;}
    item.transform.rotation=Quaternion.Euler(20,-25,8);
    var rs=item.GetComponentsInChildren<Renderer>().Where(r=>r.enabled&&r.shadowCastingMode!=ShadowCastingMode.ShadowsOnly).ToArray();Check(rs.Length>0,"visible item "+m.name);var bounds=rs[0].bounds;foreach(var r in rs)bounds.Encapsulate(r.bounds);
    item.transform.localScale*=1.65f/Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);bounds=rs[0].bounds;foreach(var r in rs)bounds.Encapsulate(r.bounds);
    item.transform.position+=new Vector3((i%8)*2-7,rows-1-(i/8)*2,0)-bounds.center;mapping+=i+","+m.group+"/"+m.name+"\n";
   }
   Color32[] Capture(string file){camera.Render();RenderTexture.active=rt;image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();File.WriteAllBytes("visual-captures/"+file,image.EncodeToPNG());return image.GetPixels32();}
   var plain=Capture("all-items-toon.png");File.WriteAllText("visual-captures/all-items-toon-index.csv",mapping);
   foreach(var item in objects)foreach(var renderer in item.GetComponentsInChildren<Renderer>())foreach(var material in renderer.sharedMaterials)Check(material.shader.isSupported&&!ShaderUtil.ShaderHasError(material.shader),"compiled shader "+material.shader.name);
   var block=new MaterialPropertyBlock();foreach(var renderer in objects.SelectMany(o=>o.GetComponentsInChildren<Renderer>())){renderer.GetPropertyBlock(block);block.SetFloat("_SelectionGlow",1);block.SetFloat("_SelectionPulse",.5f);renderer.SetPropertyBlock(block);}
   var selected=Capture("all-items-selected.png");int changed=0;for(int i=0;i<plain.Length;i++)if(Mathf.Abs(plain[i].r-selected[i].r)+Mathf.Abs(plain[i].g-selected[i].g)+Mathf.Abs(plain[i].b-selected[i].b)>6)changed++;Check(changed>300,"selection visible");
   foreach(var renderer in objects.SelectMany(o=>o.GetComponentsInChildren<Renderer>()))renderer.SetPropertyBlock(null);
   var restored=Capture("all-items-selection-cleared.png");int residual=0;for(int i=0;i<plain.Length;i++)if(Mathf.Abs(plain[i].r-restored[i].r)+Mathf.Abs(plain[i].g-restored[i].g)+Mathf.Abs(plain[i].b-restored[i].b)>6)residual++;Check(residual<20,"selection clearing residual "+residual);
   report="PASS rendered all "+objects.Count+" source prefabs, including transparent/special shaders; no shader errors; selection changed "+changed+" pixels and clearing left "+residual+" changed pixels. Contact sheet is a diagnostic layout, not a source-device screenshot.\n";
  }catch(Exception e){report="FAIL "+e+"\n";}
  finally{lighting.End();RenderTexture.active=oldActive;foreach(var o in scene.GetRootGameObjects())UnityEngine.Object.DestroyImmediate(o);SceneManager.UnloadSceneAsync(scene);rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(image);Directory.CreateDirectory("Temp");File.WriteAllText("Temp/goose-source-item-render-validation.txt",report);Debug.Log(report);}
 }
}
#endif
