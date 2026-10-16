#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEditor;

public static class SourceShadowValidation {
 [Serializable] class Data {public Theme[] scenes;}
 [Serializable] class Theme {public string name;public Node[] nodes;}
 [Serializable] class Node {public string name,mesh,sprite;public int parent,layer,castShadows;public bool enabled,receiveShadows;}
 static void Check(bool value,string message){if(!value)throw new Exception("SOURCE_SHADOW_VALIDATION: "+message);}
 public static string Run(){
  Directory.CreateDirectory("Temp");try {var report=Execute();File.WriteAllText("Temp/goose-source-shadow-validation.txt",report);return report;}catch(Exception ex){File.WriteAllText("Temp/goose-source-shadow-validation.txt","FAIL "+ex);throw;}
 }
 static string Execute(){
  var data=JsonUtility.FromJson<Data>(File.ReadAllText("Assets/GooseReplica/SourceScenes/visuals.json"));int renderers=0;
  foreach(var t in data.scenes){var p=Resources.Load<GameObject>("Replica/Scenes/"+t.name);Check(p,"prefab "+t.name);var nodes=new Transform[t.nodes.Length];Check(p.GetComponentsInChildren<Transform>(true).Length==nodes.Length,"hierarchy count "+t.name);
   for(int i=0;i<nodes.Length;i++){var n=t.nodes[i];int sibling=0;for(int j=0;j<i;j++)if(t.nodes[j].parent==n.parent)sibling++;var tr=n.parent<0?p.transform:nodes[n.parent].GetChild(sibling);nodes[i]=tr;Check(tr.name==n.name&&tr.gameObject.layer==n.layer,"node/layer "+t.name+"/"+n.name);var r=tr.GetComponent<Renderer>();if(!r)continue;renderers++;Check((int)r.shadowCastingMode==n.castShadows&&r.receiveShadows==n.receiveShadows&&r.enabled==n.enabled,"renderer flags "+n.name);}
  }
  foreach(string name in new[]{"ToonLite1","ToonLite2","ToonLite2World","ToonLite2Emissive","ToonLite2EmissiveWorld","BackShadow"}){var shader=Shader.Find("Goose/Source/"+name);Check(shader&&shader.isSupported&&!ShaderUtil.ShaderHasError(shader),"shader "+name);}
  var oldQuality=QualitySettings.shadows;var oldDistance=QualitySettings.shadowDistance;var oldSun=RenderSettings.sun;var oldAmbient=RenderSettings.ambientSkyColor;
  var existing=UnityEngine.Object.FindObjectsOfType<Light>();var states=existing.Select(x=>x.enabled).ToArray();
  var rig=new GameObject("Source lighting validation");var lighting=rig.AddComponent<ReplicaSourceLighting>();lighting.Begin();
  try {Check(QualitySettings.shadows==ShadowQuality.HardOnly&&QualitySettings.shadowResolution==ShadowResolution.Medium&&QualitySettings.shadowDistance==40&&(QualitySettings.shadowCascades==0||QualitySettings.shadowCascades==1),"source quality actual shadows="+QualitySettings.shadows+", resolution="+QualitySettings.shadowResolution+", distance="+QualitySettings.shadowDistance+", cascades="+QualitySettings.shadowCascades+", projection="+QualitySettings.shadowProjection+", pixelLights="+QualitySettings.pixelLightCount);var l=lighting.SourceLight;Check(l&&l.cullingMask==471&&l.shadows==LightShadows.Hard&&Mathf.Abs(l.intensity-.38f)<.00001f&&Mathf.Abs(l.shadowBias-.05f)<.00001f&&Mathf.Abs(l.shadowNormalBias-.4f)<.00001f&&Mathf.Abs(l.shadowNearPlane-.2f)<.00001f,"source light");Check(Quaternion.Angle(l.transform.rotation,ReplicaSourceLighting.SavedRotation)<.001f,"light rotation");}
  finally {lighting.End();UnityEngine.Object.DestroyImmediate(rig);}
  Check(QualitySettings.shadows==oldQuality&&QualitySettings.shadowDistance==oldDistance&&RenderSettings.sun==oldSun&&RenderSettings.ambientSkyColor==oldAmbient,"global restore");for(int i=0;i<existing.Length;i++)if(existing[i])Check(existing[i].enabled==states[i],"light restore");
  int items=OriginalModelImporter.ValidateSourceShadowFlags();Check(items==119,"source item renderer count "+items);int pixels=Probe();return "PASS source shadow flags "+renderers+" environment renderers; "+items+" item renderers; 6 shaders; saved light/quality and restore; shadow-on/off changed "+pixels+" pixels; receiver off removes >=95% (isolated render probe).";
 }
 static int Probe(){
  var scene=SceneManager.CreateScene("Source shadow probe "+Guid.NewGuid());var rt=new RenderTexture(256,256,24,RenderTextureFormat.ARGB32);var image=new Texture2D(256,256,TextureFormat.RGB24,false);var material=new Material(Shader.Find("Goose/Source/ToonLite2"));
  var oldShadow=QualitySettings.shadows;var oldDistance=QualitySettings.shadowDistance;var oldPixelLights=QualitySettings.pixelLightCount;var oldSun=RenderSettings.sun;var oldActive=RenderTexture.active;
  GameObject New(string n){var o=new GameObject(n);SceneManager.MoveGameObjectToScene(o,scene);o.layer=31;return o;}
  GameObject Primitive(PrimitiveType type){var o=GameObject.CreatePrimitive(type);SceneManager.MoveGameObjectToScene(o,scene);o.layer=31;o.GetComponent<Renderer>().sharedMaterial=material;return o;}
  try{
   material.SetColor("_BaseTint",new Color(.5f,.5f,.5f,1));material.SetVector("_LightColor",new Vector4(.4f,.4f,.4f,1));material.SetVector("_IndirectColor",new Vector4(.1f,.1f,.1f,1));material.SetFloat("_MatcapScale",0);material.SetVector("_HighLightColor",Vector4.zero);material.SetFloat("_BaseCellOffset",.5f);material.SetFloat("_BaseCellSharpness",.5f);
   var floor=Primitive(PrimitiveType.Plane);floor.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;floor.GetComponent<Renderer>().receiveShadows=true;
   var cube=Primitive(PrimitiveType.Cube);cube.transform.position=new Vector3(0,1,0);cube.transform.localScale=new Vector3(1,2,1);
   var cam=New("Camera").AddComponent<Camera>();cam.cullingMask=1<<31;cam.transform.position=new Vector3(0,8,-6);cam.transform.LookAt(Vector3.zero);cam.orthographic=true;cam.orthographicSize=4;cam.nearClipPlane=.1f;cam.farClipPlane=30;cam.renderingPath=RenderingPath.Forward;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=Color.black;cam.targetTexture=rt;
   var light=New("Directional light").AddComponent<Light>();light.type=LightType.Directional;light.transform.rotation=Quaternion.Euler(45,-35,0);light.intensity=.38f;light.cullingMask=1<<31;light.shadows=LightShadows.Hard;light.shadowBias=.05f;light.shadowNormalBias=.4f;light.renderMode=LightRenderMode.ForcePixel;RenderSettings.sun=light;QualitySettings.shadows=ShadowQuality.HardOnly;QualitySettings.shadowDistance=40;QualitySettings.pixelLightCount=1;
   Color32[] Capture(){cam.Render();RenderTexture.active=rt;image.ReadPixels(new Rect(0,0,256,256),0,0);image.Apply();return image.GetPixels32();}
   var shadow=Capture();Directory.CreateDirectory("visual-captures");File.WriteAllBytes("visual-captures/shadow-probe-on.png",image.EncodeToPNG());cube.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;var plain=Capture();File.WriteAllBytes("visual-captures/shadow-probe-off.png",image.EncodeToPNG());
   cube.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.On;floor.GetComponent<Renderer>().receiveShadows=false;var noReceive=Capture();File.WriteAllBytes("visual-captures/shadow-probe-no-receive.png",image.EncodeToPNG());
   var bounds=cube.GetComponent<Renderer>().bounds;var lo=new Vector2(1,1);var hi=Vector2.zero;
   for(int k=0;k<8;k++){var point=cam.WorldToViewportPoint(bounds.center+Vector3.Scale(bounds.extents,new Vector3((k&1)==0?-1:1,(k&2)==0?-1:1,(k&4)==0?-1:1)));lo=Vector2.Min(lo,point);hi=Vector2.Max(hi,point);}
   int Delta(Color32 a,Color32 b)=>Mathf.Abs(a.r-b.r)+Mathf.Abs(a.g-b.g)+Mathf.Abs(a.b-b.b);
   int changed=0,receiverMatches=0;for(int i=0;i<shadow.Length;i++){var pixel=new Vector2((i%256)/256f,(i/256)/256f);if(pixel.x>=lo.x-.02f&&pixel.x<=hi.x+.02f&&pixel.y>=lo.y-.02f&&pixel.y<=hi.y+.02f)continue;if(Delta(shadow[i],plain[i])>12){changed++;if(Delta(noReceive[i],plain[i])<=12)receiverMatches++;}}
   Check(changed>50,"shadow render probe, changed "+changed);Check(receiverMatches>=changed*.95f,"receiver disable probe, removed "+receiverMatches+" of "+changed+" shadow pixels");return changed;
  }finally{RenderTexture.active=oldActive;RenderSettings.sun=oldSun;QualitySettings.shadows=oldShadow;QualitySettings.shadowDistance=oldDistance;QualitySettings.pixelLightCount=oldPixelLights;foreach(var o in scene.GetRootGameObjects())UnityEngine.Object.DestroyImmediate(o);UnityEngine.Object.DestroyImmediate(material);UnityEngine.Object.DestroyImmediate(image);rt.Release();UnityEngine.Object.DestroyImmediate(rt);SceneManager.UnloadSceneAsync(scene);}
 }
}
#endif
