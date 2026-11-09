using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Checks actual consecutive frames through the fifth triple and second-stage
// creation. Matching only the pan's center misses a changed camera orientation.
[InitializeOnLoad]
public static class StageTransitionViewValidation {
    const string Key="stage-transition-view-validation",Output="../results/stable-view";
    [Serializable] class Report {public int frames,secondStageFrames,pixelDifferences,maxPixelDelta;public float viewDelta,projectionDelta,panDelta;public string error;}
    static Report report;
    static Camera camera;
    static Transform visual,pan;
    static Matrix4x4 initialView,initialProjection,initialPan;
    static Renderer[] panRenderers;
    static bool[] rendererStates;
    static RenderTexture target;
    static Color32[] baseline;
    static StageTransitionViewValidation(){EditorApplication.playModeStateChanged+=state=>{if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))EditorApplication.delayCall+=()=>new GameObject("Stage view regression").AddComponent<BoardValidationDriver>().StartCoroutine(Execute());};}
    public static void Run(){SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static float Delta(Matrix4x4 a,Matrix4x4 b){float result=0;for(int i=0;i<16;i++)result=Mathf.Max(result,Mathf.Abs(a[i]-b[i]));return result;}
    static IEnumerator Execute(){
        yield return null;yield return null;Directory.CreateDirectory(Output);report=new Report();
        camera=Camera.main;var previousTarget=camera.targetTexture;bool auto=Physics.autoSimulation,inputEnabled=InputController.Instance.enabled;
        Physics.autoSimulation=false;InputController.Instance.enabled=false;
        target=new RenderTexture(720,1280,24);camera.targetTexture=target;
        var steps=Core();while(true){bool next=false;try{next=steps.MoveNext();}catch(Exception e){report.error=e.ToString();Debug.LogException(e);}if(!next||report.error!=null)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();camera.targetTexture=previousTarget;Match3DFlow.Instance.Home();Physics.autoSimulation=auto;InputController.Instance.enabled=inputEnabled;UnityEngine.Object.Destroy(target);
        File.WriteAllText(Output+"/validation.json",JsonUtility.ToJson(report,true));SessionState.SetBool(Key,false);EditorApplication.Exit(report.error==null?0:1);
    }
    static IEnumerator Core(){
        var flow=Match3DFlow.Instance;var profile=LevelLayoutConfig.Get("CountrysideStove");
        var daily=new Match3DDailyLevel{date="2026-11-10",tier="bachelor",version="stable-view",generatorVersion="daily-v1",modelThemeId="foodie-party",seed=47231,seconds=600,initialCount=300,limitHeight=profile.limitHeight,layers=profile.layers};
        flow.BeginDailyPractice(0,daily);BoardCreationTestSupport.FinishPending(flow);yield return null;yield return null;
        visual=GameObject.Find("CountrysideStove source visuals").transform;pan=visual.Find("Grab3D Frying Pan");
        initialView=camera.worldToCameraMatrix;initialProjection=camera.projectionMatrix;initialPan=pan.localToWorldMatrix;
        panRenderers=pan.GetComponentsInChildren<Renderer>();rendererStates=panRenderers.Select(r=>r.enabled).ToArray();
        baseline=RenderPan("first-stage.png");
        var triples=LevelGenerator.Instance.objectsOnScene.GroupBy(o=>o.name).SelectMany(g=>g.Select((o,i)=>new{o,i}).GroupBy(x=>x.i/3).Select(g2=>g2.Select(x=>x.o).ToArray())).ToArray();
        foreach(var triple in triples){
            foreach(var obj in triple)InputController.Instance.AddObjectOnStack(obj,false);
            float until=Time.time+1.1f;while(Time.time<until){Observe();yield return null;}
        }
        Check(flow.Stage==2,"five triples must naturally enter stage 2");
        float deadline=Time.realtimeSinceStartup+45;
        while(flow.CreationInProgress&&Time.realtimeSinceStartup<deadline){Observe();yield return null;}
        Check(!flow.CreationInProgress&&LevelGenerator.Instance.objectsOnScene.Count==300,"second-stage creation completes");
        for(int i=0;i<12;i++){Observe();yield return null;}
        Compare(RenderPan("second-stage-ready.png"));
        var shell=UnityEngine.Object.FindObjectOfType<SoftTransitionContainer>();Check(shell&&Vector3.Distance(shell.transform.up,Vector3.up)<.00001f,"second-stage collider stays upright");
        Check(report.secondStageFrames>12,"observed transition plus creation frames");
        flow.RestartStage();BoardCreationTestSupport.FinishPending(flow);yield return null;
        Check(Delta(initialView,camera.worldToCameraMatrix)<.00001f&&Delta(initialProjection,camera.projectionMatrix)<.00001f,"restart retains first-stage view");
    }
    static void Observe(){
        Check(visual&&visual.gameObject.activeInHierarchy&&pan&&pan.gameObject.activeInHierarchy,"pan visual never deactivates during transition");
        for(int i=0;i<panRenderers.Length;i++)Check(panRenderers[i]&&panRenderers[i].enabled==rendererStates[i],"pan renderer never flickers off");
        report.frames++;report.viewDelta=Mathf.Max(report.viewDelta,Delta(initialView,camera.worldToCameraMatrix));report.projectionDelta=Mathf.Max(report.projectionDelta,Delta(initialProjection,camera.projectionMatrix));report.panDelta=Mathf.Max(report.panDelta,Delta(initialPan,pan.localToWorldMatrix));
        Check(report.viewDelta<.00001f&&report.projectionDelta<.00001f&&report.panDelta<.00001f,"complete camera/pan projection changed between frames");
        camera.Render();
        if(Match3DFlow.Instance.Stage==2&&report.secondStageFrames++==0)Compare(RenderPan("first-second-stage-frame.png"));
    }
    static Color32[] RenderPan(string file){
        // Remove item occlusion only for this diagnostic render; preserve all
        // renderer states immediately so the real creation/merge loop continues.
        var other=UnityEngine.Object.FindObjectsOfType<Renderer>().Where(r=>r.enabled&&!r.transform.IsChildOf(visual)).ToArray();
        foreach(var r in other)r.enabled=false;
        var previous=RenderTexture.active;var texture=new Texture2D(720,1280,TextureFormat.RGB24,false);
        try{camera.Render();RenderTexture.active=target;texture.ReadPixels(new Rect(0,0,720,1280),0,0);texture.Apply();File.WriteAllBytes(Output+"/"+file,texture.EncodeToPNG());return texture.GetPixels32();}
        finally{foreach(var r in other)if(r)r.enabled=true;RenderTexture.active=previous;UnityEngine.Object.Destroy(texture);}
    }
    static void Compare(Color32[] pixels){
        Check(pixels.Length==baseline.Length,"same render dimensions");
        for(int i=0;i<pixels.Length;i++){
            int d=Math.Max(Math.Abs(pixels[i].r-baseline[i].r),Math.Max(Math.Abs(pixels[i].g-baseline[i].g),Math.Abs(pixels[i].b-baseline[i].b)));
            if(d>0)report.pixelDifferences++;report.maxPixelDelta=Math.Max(report.maxPixelDelta,d);
        }
        Check(report.maxPixelDelta<=1,"isolated pan/backdrop image changes at stage switch");
    }
}
