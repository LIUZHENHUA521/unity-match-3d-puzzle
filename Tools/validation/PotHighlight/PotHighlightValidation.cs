using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Render the production highlight component against current food assets.
// Run only in a disposable project; no network, main scene or asset mutations.
public static class PotHighlightValidation {
    static readonly string Output=Environment.GetEnvironmentVariable("POT_HIGHLIGHT_OUTPUT")??"Temp/pot-highlight";
    static readonly System.Text.StringBuilder Report=new System.Text.StringBuilder();
    static void Check(bool value,string message){if(!value)throw new Exception(message);Report.AppendLine("PASS "+message);}
    static int Delta(Color32 a,Color32 b)=>Math.Abs(a.r-b.r)+Math.Abs(a.g-b.g)+Math.Abs(a.b-b.b);
    static int Changed(Color32[] a,Color32[] b)=>a.Where((c,i)=>Delta(c,b[i])>8).Count();
    public static void Run(){
        Directory.CreateDirectory(Output);
        try {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            foreach(var name in new[]{"Outline","Glow","Overlay","ScreenHalo"}){
                var shader=Resources.Load<Shader>("Match3D/Highlight/"+name);
                Check(shader&&shader.isSupported&&!ShaderUtil.ShaderHasError(shader),"shader supported: "+name);
            }
            Report.AppendLine("GPU "+SystemInfo.graphicsDeviceType);
            foreach(bool perspective in new[]{false,true})
            foreach(bool stencil in new[]{false,true})Probe(stencil,perspective);
            File.WriteAllText(Path.Combine(Output,"checks.txt"),Report+"COMPLETE\n");
            EditorApplication.Exit(0);
        } catch(Exception e){File.WriteAllText(Path.Combine(Output,"checks.txt"),Report+"FAIL "+e);Debug.LogException(e);EditorApplication.Exit(1);}
    }
    static void Probe(bool stencil,bool perspective){
        var cameraObject=new GameObject("Pot highlight preview camera");
        var camera=cameraObject.AddComponent<Camera>();camera.enabled=false;
        camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.18f,.21f,.26f);
        camera.orthographic=!perspective;camera.fieldOfView=24;camera.orthographicSize=2.1f;camera.nearClipPlane=.3f;camera.farClipPlane=40;
        camera.allowHDR=false;camera.allowMSAA=false;camera.cullingMask=1<<30;
        camera.transform.position=new Vector3(0,0,-10);
        var lightObject=new GameObject("Pot preview light");var light=lightObject.AddComponent<Light>();
        light.type=LightType.Directional;light.intensity=1;light.transform.rotation=Quaternion.Euler(35,-35,0);
        RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.65f,.65f,.65f);
        var descriptor=new RenderTextureDescriptor(480,640,RenderTextureFormat.ARGB32,0);
        descriptor.depthStencilFormat=stencil?GraphicsFormat.D24_UNorm_S8_UInt:GraphicsFormat.D16_UNorm;
        var rt=new RenderTexture(descriptor);rt.Create();camera.targetTexture=rt;
        var prefabs=LevelLayoutConfig.Get("CountrysideStove").prefabs;
        var items=new GameObject[3];
        // Same real models, camera and light for all three preview frames.
        for(int i=0;i<items.Length;i++){
            var prefab=prefabs.FirstOrDefault(p=>p&&p.name.IndexOf(new[]{"Donut","Burger","Pizza"}[i],StringComparison.OrdinalIgnoreCase)>=0)??prefabs[i];
            var item=items[i]=UnityEngine.Object.Instantiate(prefab);item.name= prefab.name;
            foreach(var t in item.GetComponentsInChildren<Transform>(true))t.gameObject.layer=30;
            foreach(var body in item.GetComponentsInChildren<Rigidbody>(true)){body.isKinematic=true;body.detectCollisions=false;}
            item.transform.rotation=Quaternion.Euler(-35,20+i*20,0);
            var renderers=item.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;
            foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);
            item.transform.localScale*=1.6f/Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
            bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);
            item.transform.position+=new Vector3(i==0?0:i==1?-.65f:.65f,i==0?.95f:-.85f,0)-bounds.center;
            Report.AppendLine("MODEL "+prefab.name);
        }
        var fx=ItemHighlight.For(camera);fx.UseStencilOutlines=stencil;
        string mode=(stencil?"stencil":"wechat-compatible")+(perspective?"-perspective":"");
        Color32[] Capture(string name){
            fx.Rebuild();camera.Render();var previous=RenderTexture.active;RenderTexture.active=rt;
            var texture=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);
            try{texture.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);texture.Apply();
                if(name!=null)File.WriteAllBytes(Path.Combine(Output,mode+"-"+name+".png"),texture.EncodeToPNG());
                return texture.GetPixels32();
            }finally{RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(texture);}
        }
        var plain=Capture("plain");fx.Select(items[0]);var selected=Capture("selected");
        Check(Changed(plain,selected)>100,mode+" selection visible");
        int black=0,pink=0,blue=0,halo=0;
        for(int i=0;i<plain.Length;i++){
            if(Math.Max(plain[i].r,Math.Max(plain[i].g,plain[i].b))>80&&Math.Max(selected[i].r,Math.Max(selected[i].g,selected[i].b))<12)black++;
            if(selected[i].r>240&&selected[i].b>240&&selected[i].g<15)pink++;
            if(selected[i].b>plain[i].b+8&&selected[i].b>selected[i].r+15)blue++;
            if(Delta(plain[i],plain[0])<4&&selected[i].b>plain[i].b+15&&selected[i].b>selected[i].r+15)halo++;
        }
        // Stencil outline may intentionally darken a narrow edge, but not the surface.
        Check(black<(stencil?selected.Length/100:1),mode+" no black surface: "+black);
        Check(pink==0&&blue>100,mode+" blue feedback, no error pink: "+blue);
        Check(halo>15,mode+" external halo: "+halo);
        fx.Clear();Check(Changed(plain,Capture("cleared"))<20,mode+" cancel restores pixels");
        fx.ShowAutoFind(new[]{items[1],items[2]},10);var auto=Capture("autofind");
        Check(fx.ToolTargetCount==2&&Changed(plain,auto)>200,mode+" two absorption targets");
        fx.Select(items[0]);fx.Clear();Check(fx.ToolTargetCount==2,mode+" selection cancellation preserves tool targets");
        var tick=typeof(ItemHighlight).GetMethod("TickToolLifetime",BindingFlags.Instance|BindingFlags.NonPublic);
        var deadline=(float)typeof(ItemHighlight).GetField("toolsExpireAt",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(fx);
        tick.Invoke(fx,new object[]{deadline+.01f});
        Check(fx.ToolTargetCount==0&&Changed(plain,Capture("expired"))<20,mode+" tool expiration restores pixels");
        fx.ShowAutoFind(new[]{items[1],items[2]});fx.ClearAll();Check(!fx.HasCommands,mode+" full cleanup");
        // A foreground opaque slab must not receive the selected object's surface tint.
        var cover=GameObject.CreatePrimitive(PrimitiveType.Cube);cover.layer=30;
        var coveredBounds=items[1].GetComponentsInChildren<Renderer>()[0].bounds;
        foreach(var renderer in items[1].GetComponentsInChildren<Renderer>())coveredBounds.Encapsulate(renderer.bounds);
        cover.transform.position=coveredBounds.center+new Vector3(0,0,-2);cover.transform.localScale=new Vector3(.5f,.5f,.2f);
        var covered=Capture(null);fx.Select(items[1]);var coveredSelected=Capture("occluded");
        var center=camera.WorldToScreenPoint(cover.transform.position);int coveredChanges=0;
        for(int y=(int)center.y-8;y<=(int)center.y+8;y++)for(int x=(int)center.x-8;x<=(int)center.x+8;x++)
            if(Delta(covered[y*rt.width+x],coveredSelected[y*rt.width+x])>8)coveredChanges++;
        Check(coveredChanges==0,mode+" opaque occluder stays unchanged: "+coveredChanges);
        // Export a directly rendered triptych: plain / selected / absorption.
        var strip=new Texture2D(rt.width*3,rt.height,TextureFormat.RGB24,false);
        strip.SetPixels32(0,0,rt.width,rt.height,plain);strip.SetPixels32(rt.width,0,rt.width,rt.height,selected);strip.SetPixels32(rt.width*2,0,rt.width,rt.height,auto);strip.Apply();
        File.WriteAllBytes(Path.Combine(Output,mode+"-comparison.png"),strip.EncodeToPNG());UnityEngine.Object.DestroyImmediate(strip);
        fx.ClearAll();UnityEngine.Object.DestroyImmediate(cameraObject);UnityEngine.Object.DestroyImmediate(lightObject);
        foreach(var item in items)UnityEngine.Object.DestroyImmediate(item);UnityEngine.Object.DestroyImmediate(cover);
        rt.Release();UnityEngine.Object.DestroyImmediate(rt);
    }
}
