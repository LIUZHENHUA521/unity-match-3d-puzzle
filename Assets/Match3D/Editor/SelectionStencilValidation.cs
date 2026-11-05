using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class SelectionStencilValidation {
    static int Delta(Color32 a,Color32 b)=>Math.Abs(a.r-b.r)+Math.Abs(a.g-b.g)+Math.Abs(a.b-b.b);
    public static void Run() {
        var folder="docs/validation/selection-stencil";Directory.CreateDirectory(folder);
        var root=new GameObject("Selection stencil probe");
        GameObject item=null;RenderTexture rt=null;
        var previousAmbient=RenderSettings.ambientLight;
        try {
            var camera=root.AddComponent<Camera>();camera.enabled=false;camera.cullingMask=1<<30;
            camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.2f,.2f,.2f);
            camera.allowHDR=false;camera.allowMSAA=false;camera.orthographic=true;camera.orthographicSize=1.4f;
            camera.nearClipPlane=.3f;camera.farClipPlane=1000;
            camera.transform.rotation=Match3DThemePresentation.SourceRotation;
            camera.transform.position=camera.transform.rotation*new Vector3(0,0,-30);
            var light=new GameObject("Probe light");light.transform.SetParent(root.transform);var sun=light.AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1;light.transform.rotation=Quaternion.Euler(50,-30,0);RenderSettings.ambientLight=Color.white;
            item=UnityEngine.Object.Instantiate(LevelLayoutConfig.Get("CountrysideStove").prefabs.First(p=>p.name=="Donut"));
            foreach(var t in item.GetComponentsInChildren<Transform>(true))t.gameObject.layer=30;
            var rs=item.GetComponentsInChildren<Renderer>();var bounds=rs[0].bounds;foreach(var r in rs)bounds.Encapsulate(r.bounds);
            item.transform.localScale*=2/bounds.size.magnitude;bounds=rs[0].bounds;foreach(var r in rs)bounds.Encapsulate(r.bounds);item.transform.position-=bounds.center;
            var fx=ItemHighlight.For(camera);string report="GPU: "+SystemInfo.graphicsDeviceType+"\n";
            foreach(var format in new[]{GraphicsFormat.D24_UNorm_S8_UInt,GraphicsFormat.D16_UNorm}) {
                var descriptor=new RenderTextureDescriptor(512,512,RenderTextureFormat.ARGB32,0);descriptor.depthStencilFormat=format;
                rt=new RenderTexture(descriptor);rt.Create();camera.targetTexture=rt;
                fx.UseStencilOutlines=true;fx.Clear();var before=Capture("off");fx.Select(item);var after=Capture("on");
                fx.UseStencilOutlines=false;var compatible=Capture("compatible");
                fx.Clear();var cleared=Capture("cleared");
                int compatBlack=0,changed=0,residual=0;
                for(int i=0;i<before.Length;i++){
                    if(Math.Max(before[i].r,Math.Max(before[i].g,before[i].b))>80&&Math.Max(compatible[i].r,Math.Max(compatible[i].g,compatible[i].b))<12)compatBlack++;
                    if(Delta(before[i],compatible[i])>6)changed++;
                    if(Delta(before[i],cleared[i])>6)residual++;
                }
                if(compatBlack!=0||changed<100||residual>20)throw new Exception($"Surface feedback failed: black={compatBlack}, changed={changed}, residual={residual}");
                fx.ShowAutoFind(new[]{item});var auto=Capture("autofind");
                int autoChanged=0;for(int i=0;i<auto.Length;i++)if(Delta(before[i],auto[i])>6)autoChanged++;
                if(autoChanged<100)throw new Exception("AutoFind surface feedback missing");fx.ClearAll();
                report+=$"PASS compatible feedback: black={compatBlack}, changed={changed}, residual={residual}, autoChanged={autoChanged}\n";
                int visible=0,black=0;for(int i=0;i<before.Length;i++)if(Math.Max(before[i].r,Math.Max(before[i].g,before[i].b))>80){visible++;if(Math.Max(after[i].r,Math.Max(after[i].g,after[i].b))<12)black++;}
                report+=$"{format} actual={rt.depthStencilFormat}: visible={visible}, newlyBlack={black}\n";
                Color32[] Capture(string suffix){fx.Rebuild();camera.Render();var old=RenderTexture.active;RenderTexture.active=rt;var tex=new Texture2D(512,512,TextureFormat.RGB24,false);try{tex.ReadPixels(new Rect(0,0,512,512),0,0);tex.Apply();File.WriteAllBytes(folder+"/"+format+"-"+suffix+".png",tex.EncodeToPNG());return tex.GetPixels32();}finally{RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(tex);}}
                camera.targetTexture=null;rt.Release();UnityEngine.Object.DestroyImmediate(rt);rt=null;
            }
            File.WriteAllText(folder+"/report.txt",report);Debug.Log(report);
        } finally {RenderSettings.ambientLight=previousAmbient;if(rt){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}if(item)UnityEngine.Object.DestroyImmediate(item);UnityEngine.Object.DestroyImmediate(root);}
    }
}
