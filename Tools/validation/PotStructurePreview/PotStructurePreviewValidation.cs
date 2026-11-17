using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PotStructurePreviewValidation {
    const string Key="pot.structure.validation";
    const string Output="work/pot-structure-preview";
    static PotStructurePreviewValidation(){EditorApplication.playModeStateChanged+=s=>{
        if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))
            new GameObject("Pot structure validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
    };}
    public static void Run(){Directory.CreateDirectory(Output);File.WriteAllText(Output+"/checks.txt","");SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);File.AppendAllText(Output+"/checks.txt","PASS "+message+"\n");}
    static IEnumerator Guard(){
        var test=Core();string error=null;float deadline=Time.realtimeSinceStartup+180;
        while(true){object current=null;bool next=false;try{if(Time.realtimeSinceStartup>deadline)throw new Exception("timeout");next=test.MoveNext();if(next)current=test.Current;}catch(Exception e){error=e.ToString();}
            if(error!=null||!next)break;yield return current;
        }
        File.AppendAllText(Output+"/checks.txt",error==null?"COMPLETE\n":"FAIL "+error+"\n");SessionState.SetBool(Key,false);EditorApplication.Exit(error==null?0:1);
    }
    static IEnumerator Core(){
        while(!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)yield return null;
        foreach(var service in new MonoBehaviour[]{Match3DGameRecords.Instance,Match3DInvitations.Instance,PotReplayService.Instance})if(service){service.StopAllCoroutines();service.enabled=false;}
        var flow=Match3DFlow.Instance;
        foreach(int count in new[]{120,180,240}){
            var level=new Match3DDailyLevel{date="2026-11-17",tier="bachelor",version="pot-structure-validation",candidateId="candidate-0123456789abcdef01234567",generatorVersion="daily-v1",modelThemeId="foodie-party",seed=47231,seconds=600,initialCount=count+150,limitHeight=25.5f,layers=new[]{new ChallengeLayer(count,7,14),new ChallengeLayer(75,5,20),new ChallengeLayer(75,5,26)}};
            Check(level.IsValid(23),"valid generation fixture "+count);
            flow.PreviewDailyOpening(level,1);while(flow.CreationInProgress)yield return null;
            yield return new WaitForSeconds(5);var first=flow.CaptureDailyOpening();
            flow.PreviewDailyOpening(level,2);while(flow.CreationInProgress)yield return null;
            yield return new WaitForSeconds(5);
            // Freeze the generated opening exactly as the daily preview does.
            var snapshot=flow.CaptureDailyOpening();
            level.opening=new Match3DDailySnapshot{stages=new[]{first,snapshot}};level.generatorVersion="shared-v2";
            Check(level.IsValid(23),"valid restored fixture "+count);
            flow.PreviewDailyOpening(level,2);while(flow.CreationInProgress)yield return null;
            var renderers=LayeredItemPile.Instance.PotStructureRenderers.ToArray();
            Check(renderers.Any(r=>!r.enabled),"fixture has height-hidden items "+count);
            Check(renderers.All(r=>r.gameObject.activeInHierarchy),"hidden item objects remain active "+count);
            var enabled=renderers.Select(r=>r.enabled).ToArray();var layers=renderers.Select(r=>r.gameObject.layer).ToArray();
            var before=JsonUtility.ToJson(flow.CaptureDailyOpening());var camera=Camera.main;var position=camera.transform.position;var rotation=camera.transform.rotation;float size=camera.orthographicSize;
            var image=PotStructurePreview.Capture();var repeat=PotStructurePreview.Capture();
            Check(before==JsonUtility.ToJson(flow.CaptureDailyOpening()),"capture preserves opening poses, visibility and thresholds "+count);
            Check(renderers.Select(r=>r.enabled).SequenceEqual(enabled)&&renderers.Select(r=>r.gameObject.layer).SequenceEqual(layers),"capture restores every visual flag and layer "+count);
            Check(camera.transform.position==position&&camera.transform.rotation==rotation&&camera.orthographicSize==size,"game camera untouched "+count);
            Check(!UnityEngine.Object.FindObjectsOfType<Camera>().Any(c=>c.name=="Pot structure capture"),"capture camera released immediately "+count);
            Check(image.Length<=180000&&repeat.Length<=180000&&image.StartsWith("data:image/jpeg;base64,/9j/"),"JPEG satisfies backend contract "+count);
            var bytes=Convert.FromBase64String(image.Substring(image.IndexOf(',')+1));File.WriteAllBytes(Output+"/pot-structure-"+count+".jpg",bytes);
            var texture=new Texture2D(2,2);texture.LoadImage(bytes);
            Check(texture.width==540&&texture.height==960,"portrait capture dimensions "+count);
            Check(texture.GetPixels32().Count(c=>c.g>c.r*1.3f&&c.g>c.b*1.3f)>1500,"green physical outlines rendered "+count);UnityEngine.Object.Destroy(texture);
        }
        var obj=new GameObject("Pot collider math fixture");obj.transform.position=new Vector3(2,3,4);obj.transform.rotation=Quaternion.Euler(13,27,41);obj.transform.localScale=new Vector3(2,3,4);
        var points=new System.Collections.Generic.List<Vector3>();var box=obj.AddComponent<BoxCollider>();box.center=Vector3.one;box.size=new Vector3(2,4,6);PotStructurePreview.AddCollider(points,box);
        Check(points.Count==24&&points.Contains(obj.transform.TransformPoint(box.center+box.size*.5f)),"box center, rotation and nonuniform scale preserved");
        var sphere=obj.AddComponent<SphereCollider>();sphere.radius=.5f;sphere.center=Vector3.one;points.Clear();PotStructurePreview.AddCollider(points,sphere);
        Check(points.All(p=>Mathf.Abs(Vector3.Distance(p,obj.transform.TransformPoint(sphere.center))-2)<.001f),"sphere uses maximum world scale");
        foreach(int axis in new[]{0,1,2}){var capsule=obj.AddComponent<CapsuleCollider>();capsule.direction=axis;capsule.radius=.4f;capsule.height=3;points.Clear();PotStructurePreview.AddCollider(points,capsule);Check(points.Count>200&&points.All(p=>!float.IsNaN(p.x)),"capsule axis "+axis);UnityEngine.Object.DestroyImmediate(capsule);}
        var fit=new GameObject("Pot fit test").AddComponent<Camera>();fit.enabled=false;fit.orthographic=true;fit.aspect=540f/960;
        var bounds=new Bounds(new Vector3(3,20,-4),new Vector3(15,50,12));PotStructurePreview.Fit(fit,bounds);
        for(int i=0;i<8;i++){var p=fit.WorldToViewportPoint(bounds.center+Vector3.Scale(bounds.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1)));Check(p.x>0&&p.x<1&&p.y>0&&p.y<1&&p.z>fit.nearClipPlane&&p.z<fit.farClipPlane,"full-height frame corner "+i);}
        UnityEngine.Object.DestroyImmediate(fit.gameObject);UnityEngine.Object.DestroyImmediate(obj);flow.Home();
    }
}
