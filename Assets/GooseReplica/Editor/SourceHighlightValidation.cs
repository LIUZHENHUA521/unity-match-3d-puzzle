using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using System.Linq;

public static class SourceHighlightValidation {
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    static void Close(float a,float b,string message){Check(Mathf.Abs(a-b)<.00001f,message+" "+a+" != "+b);}
    public static void Run(){
        string report="";GameObject cameraObject=null,first=null,second=null,real=null,country=null;RenderTexture rt=null;Mesh gpuOnly=null;
        try{
            Close(ReplicaSourceHighlight.OverlayPulse(0),1,"source triangle wave starts at maximum");
            Close(ReplicaSourceHighlight.OverlayPulse(.25f),.75f,"quarter period");
            Close(ReplicaSourceHighlight.OverlayPulse(.5f),.5f,"half period minimum");
            Close(ReplicaSourceHighlight.OverlayPulse(1),1,"one second period");
            Close(ReplicaSourceHighlight.GlowPulse(Mathf.PI/6),1.25f,"original _Time.w multiplier gives max width");
            cameraObject=new GameObject("Source highlight validation camera");var camera=cameraObject.AddComponent<Camera>();camera.enabled=false;camera.transform.position=new Vector3(0,0,-5);camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.1f,.1f,.1f);camera.cullingMask=1<<30;
            first=GameObject.CreatePrimitive(PrimitiveType.Sphere);first.layer=30;first.transform.position=Vector3.zero;
            second=GameObject.CreatePrimitive(PrimitiveType.Cube);second.layer=30;second.transform.position=new Vector3(1.5f,0,0);
            var mesh=first.GetComponent<MeshFilter>().sharedMesh;var originalNormals=mesh.normals;var originalMaterial=first.GetComponent<Renderer>().sharedMaterial;
            var fx=ReplicaSourceHighlight.For(camera);fx.Select(first);fx.Rebuild();
            Check(fx.Target==first&&fx.RendererCount==1&&fx.DrawCount==12,"one renderer has stencil clear/mask/overlay/four glow/four outline/clear");
            Check(camera.GetCommandBuffers(CameraEvent.AfterForwardAlpha).Length==1,"one camera effect buffer");
            Check(first.GetComponent<MeshFilter>().sharedMesh==mesh&&first.GetComponent<Renderer>().sharedMaterial==originalMaterial,"no shared mesh/material replacement");
            Check(mesh.normals.Length==originalNormals.Length&&mesh.normals[0]==originalNormals[0],"source mesh normals remain untouched");
            foreach(var name in new[]{"Mask","Overlay","Glow","Outline"}){var shader=Resources.Load<Shader>("Replica/Highlight/"+name);Check(shader&&shader.isSupported&&!ShaderUtil.ShaderHasError(shader),"shader supported without compile errors "+name);}
            rt=new RenderTexture(512,512,24);camera.targetTexture=rt;camera.Render();
            Directory.CreateDirectory("visual-captures");var active=RenderTexture.active;RenderTexture.active=rt;var tex=new Texture2D(512,512,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,512,512),0,0);tex.Apply();File.WriteAllBytes("visual-captures/source-selection-highlight.png",tex.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tex);RenderTexture.active=active;
            fx.Select(second);fx.Rebuild();Check(fx.Target==second&&fx.RendererCount==1,"switch releases old selection geometry");
            second.SetActive(false);fx.Rebuild();Check(!fx.Target&&!fx.HasCommands,"inactive object immediately clears commands");
            first.GetComponent<Renderer>().enabled=false;fx.Select(first);fx.Rebuild();Check(fx.DrawCount==2,"hidden renderer not outlined");
            fx.Clear();Check(!fx.HasCommands&&fx.RendererCount==0&&!fx.Target,"cancel has no residual draws");
            for(int i=0;i<8;i++){fx.Select(first);fx.Clear();}Check(camera.GetCommandBuffers(CameraEvent.AfterForwardAlpha).Length==1,"repeated selection does not leak camera buffers");
            // Imported country meshes disable Read/Write. Exercise a guaranteed
            // GPU-only mesh as well, so Editor CPU access cannot hide a regression.
            second.SetActive(false);first.GetComponent<Renderer>().enabled=true;
            gpuOnly=UnityEngine.Object.Instantiate(mesh);gpuOnly.UploadMeshData(true);
            first.GetComponent<MeshFilter>().sharedMesh=gpuOnly;Check(!gpuOnly.isReadable,"GPU-only probe");
            var gpuPlain=Capture(null);fx.Select(first);var gpuLit=Capture(null);
            Check(fx.RendererCount==1&&fx.DrawCount==12,"GPU-only mesh participates in every highlight pass");
            Check(Changed(gpuPlain,gpuLit)>100,"GPU-only mesh highlight changes rendered pixels");
            fx.Clear();Check(Changed(gpuPlain,Capture(null))<20,"GPU-only selection clears");
            Check(first.GetComponent<MeshFilter>().sharedMesh==gpuOnly&&!gpuOnly.isReadable,"highlight preserves borrowed mesh and readability");
            first.GetComponent<MeshFilter>().sharedMesh=mesh;UnityEngine.Object.DestroyImmediate(gpuOnly);gpuOnly=null;
            first.SetActive(false);
            int countryCount=0,unreadableCount=0;string countryEvidence="";
            var feedback=cameraObject.AddComponent<ReplicaSelectionFeedback>();
            foreach(var countryPrefab in ReplicaSourceLevelPlan.Get("CountrysideStove").prefabs){
                country=UnityEngine.Object.Instantiate(countryPrefab);
                foreach(var tr in country.GetComponentsInChildren<Transform>(true))tr.gameObject.layer=30;
                foreach(var body in country.GetComponentsInChildren<Rigidbody>(true)){body.isKinematic=true;body.detectCollisions=false;}
                var countryRenderers=country.GetComponentsInChildren<Renderer>().Where(r=>r.enabled&&r.shadowCastingMode!=ShadowCastingMode.ShadowsOnly).ToArray();
                Check(countryRenderers.Length>0,"country renderer "+countryPrefab.name);
                var cb=countryRenderers[0].bounds;foreach(var r in countryRenderers)cb.Encapsulate(r.bounds);
                country.transform.localScale*=2/Mathf.Max(cb.size.x,cb.size.y,cb.size.z);
                cb=countryRenderers[0].bounds;foreach(var r in countryRenderers)cb.Encapsulate(r.bounds);country.transform.position-=cb.center;
                unreadableCount+=country.GetComponentsInChildren<MeshFilter>().Count(f=>f.sharedMesh&&!f.sharedMesh.isReadable);
                var before=Capture(null);feedback.Begin(country,camera,false);var lit=Capture(countryCount==0?"country-highlight-on.png":null);
                Check(feedback.Selected==country&&fx.RendererCount>=countryRenderers.Length,"country press registers renderer "+countryPrefab.name);
                int changedPixels=Changed(before,lit);Check(changedPixels>100,"country press renders highlight "+countryPrefab.name);
                int yellow=0;for(int i=0;i<lit.Length;i++)if(lit[i].r>before[i].r+6&&lit[i].g>before[i].g+6&&lit[i].r>lit[i].b+20&&lit[i].g>lit[i].b+20)yellow++;
                Check(yellow>10,"country press produces yellow pixels "+countryPrefab.name);
                feedback.Cancel();Check(!fx.Target&&!fx.HasCommands&&Changed(before,Capture(null))<20,"country cancel clears highlight "+countryPrefab.name);
                countryEvidence+=$" {countryPrefab.name}:{changedPixels}/{yellow}";countryCount++;
                UnityEngine.Object.DestroyImmediate(country);country=null;
            }
            Check(unreadableCount>0,"current countryside includes GPU-only imported meshes");
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GooseReplica/OriginalModels/Prefabs/Barbecue/Goose_Pancake.prefab");Check(prefab,"source goose prefab");
            real=UnityEngine.Object.Instantiate(prefab);first.SetActive(false);second.SetActive(false);
            foreach(var tr in real.GetComponentsInChildren<Transform>(true))tr.gameObject.layer=30;
            foreach(var body in real.GetComponentsInChildren<Rigidbody>(true)){body.isKinematic=true;body.detectCollisions=false;}
            real.transform.rotation=Quaternion.Euler(0,150,0);
            var rs=real.GetComponentsInChildren<Renderer>().Where(r=>r.enabled&&r.shadowCastingMode!=ShadowCastingMode.ShadowsOnly).ToArray();Check(rs.Length>0,"source goose renderers");
            var bounds=rs[0].bounds;foreach(var r in rs)bounds.Encapsulate(r.bounds);real.transform.localScale*=2/Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);bounds=rs[0].bounds;foreach(var r in rs)bounds.Encapsulate(r.bounds);real.transform.position-=bounds.center;
            Color32[] Capture(string filename){camera.Render();var old=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);try{image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();if(filename!=null)File.WriteAllBytes("visual-captures/"+filename,image.EncodeToPNG());return image.GetPixels32();}finally{RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(image);}}
            int Changed(Color32[] a,Color32[] b){int count=0;for(int i=0;i<a.Length;i++)if(Mathf.Abs(a[i].r-b[i].r)+Mathf.Abs(a[i].g-b[i].g)+Mathf.Abs(a[i].b-b[i].b)>6)count++;return count;}
            var plain=Capture("source-goose-highlight-off.png");fx.Select(real);var selected=Capture("source-goose-highlight-on.png");Check(fx.RendererCount==rs.Length,"all real goose renderer parts selected");
            int changed=0;for(int i=0;i<plain.Length;i++)if(Mathf.Abs(plain[i].r-selected[i].r)+Mathf.Abs(plain[i].g-selected[i].g)+Mathf.Abs(plain[i].b-selected[i].b)>6)changed++;Check(changed>100,"real source skinned selection produces pixels");
            fx.Clear();var cleared=Capture("source-goose-highlight-cleared.png");int residual=0;for(int i=0;i<plain.Length;i++)if(Mathf.Abs(plain[i].r-cleared[i].r)+Mathf.Abs(plain[i].g-cleared[i].g)+Mathf.Abs(plain[i].b-cleared[i].b)>6)residual++;Check(residual<20,"real source selection clears without residual "+residual);
            int skinCount=0;string skinEvidence="";
            // Isolate every source skin: a bright static pancake must not hide a missing goose body effect.
            foreach(var skin in rs.OfType<SkinnedMeshRenderer>()){
                foreach(var r in rs)r.enabled=r==skin;
                var sb=skin.bounds;camera.orthographic=true;camera.orthographicSize=Mathf.Max(sb.size.x,sb.size.y)*.65f+.05f;camera.transform.position=sb.center+new Vector3(0,0,-5);
                var beforeSkin=Capture("source-goose-skin-"+skinCount+"-off.png");fx.Select(real);var afterSkin=Capture("source-goose-skin-"+skinCount+"-on.png");
                int skinChanged=0;for(int i=0;i<beforeSkin.Length;i++)if(Mathf.Abs(beforeSkin[i].r-afterSkin[i].r)+Mathf.Abs(beforeSkin[i].g-afterSkin[i].g)+Mathf.Abs(beforeSkin[i].b-afterSkin[i].b)>6)skinChanged++;
                Check(skinChanged>100,"isolated source skin "+skin.name+" must itself receive highlight, changed="+skinChanged);
                skinEvidence+=" "+skin.name+":"+skinChanged;skinCount++;fx.Clear();
            }
            Check(skinCount>0,"real goose has isolated skin validation");foreach(var r in rs)r.enabled=true;
            camera.orthographic=false;camera.transform.position=new Vector3(0,0,-5);
            var toolPlain=Capture("source-autofind-highlight-off.png");fx.ShowAutoFind(new[]{real});var toolLit=Capture("source-autofind-highlight-on.png");
            int blueShift=0;for(int i=0;i<toolPlain.Length;i++)if(toolLit[i].b-toolPlain[i].b>6&&toolLit[i].r<toolPlain[i].r)blueShift++;
            Check(blueShift>100,"Lighting profile creates blue overlay on the actual goose "+blueShift);
            fx.Select(real);fx.Clear();Check(fx.ToolTargetCount==1,"ordinary selection cancellation retains AutoFind during flight");
            second.SetActive(true);fx.ShowAutoFind(new[]{real,second});fx.Rebuild();Check(fx.ToolTargetCount==2&&fx.DrawCount>=24,"two source tool effect slots coexist");
            var deadlineField=typeof(ReplicaSourceHighlight).GetField("toolsExpireAt",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
            var tickLifetime=typeof(ReplicaSourceHighlight).GetMethod("TickToolLifetime",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
            float oldTimeScale=Time.timeScale;
            try {
                Time.timeScale=0;fx.ShowAutoFind(new[]{real,second});fx.Rebuild();
                float deadline=(float)deadlineField.GetValue(fx);Close(deadline,Time.unscaledTime+.3f,"AutoFind deadline uses unscaled clock while paused");
                // Exercise the same lifetime transition as Update; EditMode does not advance PlayerLoop clocks inside Run.
                tickLifetime.Invoke(fx,new object[]{deadline-.001f});Check(fx.ToolTargetCount==2&&fx.HasCommands,"paused AutoFind still present at .299 real seconds");
                tickLifetime.Invoke(fx,new object[]{deadline+.001f});Check(fx.ToolTargetCount==0&&!fx.HasCommands,"paused AutoFind clears at .301 real seconds");
                fx.ShowAutoFind(new[]{real,second});deadlineField.SetValue(fx,Time.unscaledTime-1);
                fx.SendMessage("Update");Check(fx.ToolTargetCount==0&&!fx.HasCommands,"Update clears expired AutoFind even with timeScale=0");
            } finally {Time.timeScale=oldTimeScale;}
            fx.ShowAutoFind(new[]{real,second});fx.ClearAll();Check(fx.ToolTargetCount==0&&!fx.HasCommands,"restart cleanup clears ordinary and tool groups");
            // Source RenderMask uses Always when glow is AlwaysOnTop. A covered interior must not turn cyan.
            real.SetActive(false);first.SetActive(true);first.GetComponent<Renderer>().enabled=true;first.transform.position=Vector3.zero;
            second.SetActive(true);second.transform.position=new Vector3(0,0,-.7f);second.transform.localScale=new Vector3(.6f,.6f,.2f);
            int coveredDelta=0;
            foreach(float distance in new[]{3f,5f,9f}){
                camera.transform.position=new Vector3(0,0,-distance);fx.ClearAll();
                var coveredPlain=Capture("source-autofind-occluded-"+distance+"-off.png");fx.ShowAutoFind(new[]{first});var coveredLit=Capture("source-autofind-occluded-"+distance+"-on.png");
                int centerDelta=0,edgeDelta=0;
                for(int y=0;y<rt.height;y++)for(int x=0;x<rt.width;x++){
                    int i=y*rt.width+x;int delta=Mathf.Abs(coveredPlain[i].r-coveredLit[i].r)+Mathf.Abs(coveredPlain[i].g-coveredLit[i].g)+Mathf.Abs(coveredPlain[i].b-coveredLit[i].b);
                    if(delta>6){edgeDelta++;if(Mathf.Abs(x-rt.width/2)<8&&Mathf.Abs(y-rt.height/2)<8)centerDelta++;}
                }
                Check(centerDelta==0,"AlwaysOnTop glow must not fill an occluded interior at distance "+distance+" changed="+centerDelta);
                Check(edgeDelta>10,"occlusion mask must retain actual highlight around visible geometry at distance "+distance);
                coveredDelta+=centerDelta;fx.ClearAll();
            }
            report="PASS source profile; triangle overlay and width sine; stencil/4-direction geometry; shader support; selection switching; hidden/destroyed cancellation; immutable originals; camera buffer lifetime; real goose mesh render changed "+changed+" pixels and clearing left "+residual+"; isolated skins "+skinEvidence+"; AutoFind blue pixels "+blueShift+"; two tool slots and unscaled timeout during pause; 3-distance covered interior changed "+coveredDelta+" pixels; rendered capture\n";
            report+=$"PASS GPU-only mesh; {countryCount} countryside prefabs ({unreadableCount} unreadable meshes): press produces yellow highlight, cancel clears. Changed/yellow pixels:"+countryEvidence+"\n";
        }catch(Exception e){report="FAIL "+e+"\n";}
        finally{
            if(cameraObject)UnityEngine.Object.DestroyImmediate(cameraObject);if(first)UnityEngine.Object.DestroyImmediate(first);if(second)UnityEngine.Object.DestroyImmediate(second);if(real)UnityEngine.Object.DestroyImmediate(real);if(country)UnityEngine.Object.DestroyImmediate(country);if(gpuOnly)UnityEngine.Object.DestroyImmediate(gpuOnly);if(rt){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}
            File.WriteAllText("Temp/goose-source-highlight-validation.txt",report);Debug.Log(report);
        }
    }
}
