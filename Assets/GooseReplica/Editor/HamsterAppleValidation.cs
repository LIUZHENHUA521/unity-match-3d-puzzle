using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class HamsterAppleValidation {
    static double deadline,due;static int step;static string result="";static string output;
    static void Check(bool ok,string label){if(!ok)throw new Exception(label);result+="PASS "+label+"\n";}
    public static void Run(){
        output=Environment.GetEnvironmentVariable("APPLE_VALIDATION_OUTPUT");
        try{
            HamsterAppleImporter.Build();var p=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GooseReplica/Generated/HamsterApple/apple.prefab");
            var mesh=p.GetComponent<MeshFilter>().sharedMesh;Check(mesh.triangles.Length/3<=3000,"optimized mesh <= 3000 triangles");Check(p.GetComponent<MeshRenderer>().sharedMaterial.mainTexture!=null,"texture assigned");Check(p.GetComponent<Rigidbody>()&&p.GetComponent<SphereCollider>()&&p.CompareTag("Collectable"),"physics and pick tag assigned");
            Check(Mathf.Abs(mesh.bounds.size.magnitude)<1&&mesh.bounds.size.y>.1f,"model scale fits game");Render(p);
            EditorSettings.enterPlayModeOptionsEnabled=true;EditorSettings.enterPlayModeOptions=EnterPlayModeOptions.DisableDomainReload;
            EditorSceneManager.OpenScene(ReplicaBuilder.Scene);EditorApplication.update+=Tick;EditorApplication.isPlaying=true;step=0;due=EditorApplication.timeSinceStartup+5;deadline=due+90;
        }catch(Exception e){Finish(e);}
    }
    static void Render(GameObject prefab){
        RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.6f,.6f,.6f);
        var obj=UnityEngine.Object.Instantiate(prefab,new Vector3(100,100,100),Quaternion.Euler(0,25,0));UnityEngine.Object.DestroyImmediate(obj.GetComponent<Rigidbody>());
        var camera=new GameObject("Apple review camera").AddComponent<Camera>();camera.transform.position=obj.transform.position+new Vector3(.55f,.30f,.80f);camera.transform.LookAt(obj.transform.position);camera.orthographic=true;camera.orthographicSize=.25f;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.93f,.90f,.83f);camera.nearClipPlane=.01f;camera.farClipPlane=3;
        var light=new GameObject("Apple review light").AddComponent<Light>();light.type=LightType.Directional;light.intensity=.8f;light.transform.rotation=Quaternion.Euler(40,150,0);
        var rt=new RenderTexture(640,640,24);camera.targetTexture=rt;camera.Render();var old=RenderTexture.active;RenderTexture.active=rt;var png=new Texture2D(640,640,TextureFormat.RGB24,false);png.ReadPixels(new Rect(0,0,640,640),0,0);png.Apply();File.WriteAllBytes(Path.Combine(output,"apple-unity-render.png"),png.EncodeToPNG());RenderTexture.active=old;camera.targetTexture=null;rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(png);UnityEngine.Object.DestroyImmediate(camera.gameObject);UnityEngine.Object.DestroyImmediate(light.gameObject);UnityEngine.Object.DestroyImmediate(obj);
    }
    static void Tick(){
        if(EditorApplication.timeSinceStartup<due||!EditorApplication.isPlaying)return;
        try{
            if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Validation timed out at step "+step);
            var gen=LevelGenerator.Instance;var input=InputController.Instance;
            if(step==0){HamsterAppleImporter.Preview();step=1;due=EditorApplication.timeSinceStartup+5;}
            else if(step==1){Check(gen.objectsOnScene.Count==12,"12 sample items spawn");Check(gen.objectsOnScene.All(x=>x.transform.position.y>-.5f),"items stay above pot floor");Check(GameManager.Instance.GameIsPlaying(),"sample becomes playable");var apples=gen.objectsOnScene.Where(x=>x.GetComponent<MeshFilter>()&&x.GetComponent<MeshFilter>().sharedMesh.name=="Hamster Apple").ToArray();Check(apples.Length==6,"six generated apples in live game");foreach(var a in apples.Take(3))input.AddObjectOnStack(a);step=2;due=EditorApplication.timeSinceStartup+2;}
            else if(step==2){Check(gen.objectsOnScene.Count==9&&input.objectsOnStack.Count==0,"three generated apples match and disappear");foreach(var a in gen.objectsOnScene.GroupBy(x=>x.name).First().Take(3).ToArray())input.AddObjectOnStack(a);step=3;due=EditorApplication.timeSinceStartup+2;}
            else if(step==3){Check(gen.objectsOnScene.Count==6,"next triple also resolves");Finish(null);}
        }catch(Exception e){Finish(e);}
    }
    static void Finish(Exception e){EditorApplication.update-=Tick;result+=e==null?"ALL CHECKS PASSED\n":"FAIL "+e+"\n";if(!string.IsNullOrEmpty(output))File.WriteAllText(Path.Combine(output,"Unity验证.txt"),result);Debug.Log(result);EditorApplication.Exit(e==null?0:1);}
}
