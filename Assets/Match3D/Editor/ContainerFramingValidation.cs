using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Actual-item framing check; run only in a disposable project snapshot.
[InitializeOnLoad]
public static class ContainerFramingValidation {
    const string Key="container-framing-validation",Output="../results/container-framing";
    [Serializable] class Sample {public string name;public int remaining,visible;public Vector2 min,max,center,panCenter;}
    [Serializable] class Report {public List<Sample> samples=new List<Sample>();public int triples,pickHits;public string error;}
    static Report report;static Camera camera;static RenderTexture target;static Vector3 firstCamera;static Quaternion firstRotation;static Matrix4x4 firstPan;
    static ContainerFramingValidation(){EditorApplication.playModeStateChanged+=state=>{if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))EditorApplication.delayCall+=()=>new GameObject("Framing checks").AddComponent<BoardValidationDriver>().StartCoroutine(Execute());};}
    public static void Run(){SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Check(bool pass,string message){if(!pass)throw new Exception(message);}
    static IEnumerator Execute(){
        yield return null;yield return null;Directory.CreateDirectory(Output);report=new Report();
        camera=Camera.main;var previousTarget=camera.targetTexture;bool auto=Physics.autoSimulation,input=InputController.Instance.enabled;float size=BoardLayoutSettings.ModelSizeMultiplier;
        Physics.autoSimulation=false;InputController.Instance.enabled=false;BoardLayoutSettings.ModelSizeMultiplier=1;
        target=new RenderTexture(720,1560,24);camera.targetTexture=target;
        var canvases=UnityEngine.Object.FindObjectsOfType<Canvas>().Where(c=>c.isRootCanvas&&c.renderMode==RenderMode.ScreenSpaceOverlay).ToArray();
        foreach(var canvas in canvases){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=.3f;}
        var steps=Core();while(true){bool more=false;try{more=steps.MoveNext();}catch(Exception e){report.error=e.ToString();Debug.LogException(e);}if(!more||report.error!=null)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();foreach(var canvas in canvases)if(canvas)canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        camera.targetTexture=previousTarget;Match3DFlow.Instance.Home();Physics.autoSimulation=auto;InputController.Instance.enabled=input;BoardLayoutSettings.ModelSizeMultiplier=size;UnityEngine.Object.Destroy(target);
        File.WriteAllText(Output+"/validation.json",JsonUtility.ToJson(report,true));SessionState.SetBool(Key,false);EditorApplication.Exit(report.error==null?0:1);
    }
    static void Simulate(int steps){Physics.SyncTransforms();for(int i=0;i<steps;i++){LayeredItemPile.Instance.StepVisibility(.02f);Physics.Simulate(.02f);}}
    static IEnumerator Core(){
        var flow=Match3DFlow.Instance;var profile=LevelLayoutConfig.Get("CountrysideStove");
        var daily=new Match3DDailyLevel{date="2026-11-10",tier="bachelor",version="framing",generatorVersion="daily-v1",modelThemeId="foodie-party",seed=47231,seconds=600,initialCount=300,limitHeight=profile.limitHeight,layers=profile.layers};
        flow.BeginDailyPractice(0,daily);BoardCreationTestSupport.FinishPending(flow);Simulate(150);yield return null;
        firstCamera=camera.transform.position;firstRotation=camera.transform.rotation;firstPan=Pan().localToWorldMatrix;Capture("first-stage");
        flow.NextStage();BoardCreationTestSupport.FinishPending(flow);Simulate(500);yield return new WaitForSeconds(2);
        AssertPose();
        // Diagnostic before/after on precisely the same settled board. Revert the
        // fixed origin only for one render; no physics or gameplay runs in between.
        var objects=LevelGenerator.Instance.objectsOnScene.ToArray();var positions=objects.Select(o=>o.transform.position).ToArray();
        for(int i=0;i<objects.Length;i++)objects[i].transform.position=positions[i]-SoftTransitionContainer.Origin;
        Capture("before-offset");for(int i=0;i<objects.Length;i++)objects[i].transform.position=positions[i];Physics.SyncTransforms();
        Capture("remaining-300");CheckView();
        Check(Mathf.Abs(report.samples.Last().center.y-report.samples.Last().panCenter.y)<.025f,"opening pile is centered within 2.5% of viewport height");
        // Exercise the real ray picker at visible mesh centers and a viewport grid.
        for(int y=20;y<=80;y+=3)for(int x=5;x<96;x+=3){if(InputController.Instance.PickAt(new Vector2(camera.pixelWidth*x/100f,camera.pixelHeight*y/100f)))report.pickHits++;}
        Check(report.pickHits>20,"rendered pile remains pickable through the unchanged camera");
        while(LevelGenerator.Instance.objectsOnScene.Count>3){
            var triple=LevelGenerator.Instance.objectsOnScene.GroupBy(o=>o.name).Where(g=>g.Count()>=3).Select(g=>g.OrderByDescending(o=>o.transform.position.y).Take(3).ToArray()).OrderByDescending(g=>g.Average(o=>o.transform.position.y)).First();
            foreach(var obj in triple)InputController.Instance.AddObjectOnStack(obj,false);
            yield return new WaitForSeconds(1.1f);Check(InputController.Instance.objectsOnStack.Count==0,"triple clears tray");report.triples++;Simulate(100);
            int count=LevelGenerator.Instance.objectsOnScene.Count;
            if(count==150||count==30||count==3){Simulate(400);yield return null;AssertPose();Capture("remaining-"+count);CheckView();}
        }
        Check(report.triples==99,"depleted via 99 actual triples");
        Check(LevelGenerator.Instance.objectsOnScene.All(o=>SoftTransitionContainer.Contains(o.transform.position,-.1f)),"last items stay in translated shell");
    }
    static Transform Pan()=>GameObject.Find("CountrysideStove source visuals").transform.Find("Grab3D Frying Pan");
    static void AssertPose(){
        Check(Vector3.Distance(firstCamera,camera.transform.position)<.00001f&&Quaternion.Angle(firstRotation,camera.transform.rotation)<.001f,"camera remains unchanged");
        var matrix=Pan().localToWorldMatrix;for(int i=0;i<16;i++)Check(Mathf.Abs(matrix[i]-firstPan[i])<.00001f,"pan remains unchanged");
        var shell=UnityEngine.Object.FindObjectOfType<SoftTransitionContainer>();Check(shell&&Vector3.Distance(shell.transform.up,Vector3.up)<.00001f,"shell stays upright");
    }
    static void CheckView(){var sample=report.samples.Last();Check(sample.min.y>.19f&&sample.max.y<.90f,"visible pile overlaps top HUD or bottom tray: "+sample.name);}
    static void Capture(string name){
        Canvas.ForceUpdateCanvases();camera.Render();var previous=RenderTexture.active;RenderTexture.active=target;var texture=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);
        texture.ReadPixels(new Rect(0,0,target.width,target.height),0,0);texture.Apply();File.WriteAllBytes(Output+"/"+name+".png",texture.EncodeToPNG());RenderTexture.active=previous;UnityEngine.Object.Destroy(texture);
        var objects=LevelGenerator.Instance.objectsOnScene;var sample=new Sample{name=name,remaining=objects.Count,min=Vector2.one*float.PositiveInfinity,max=Vector2.one*float.NegativeInfinity};
        foreach(var obj in objects){var renderers=obj.GetComponentsInChildren<MeshRenderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy).ToArray();if(renderers.Length==0)continue;sample.visible++;
            foreach(var renderer in renderers){var b=renderer.bounds;for(int i=0;i<8;i++){var p=b.center+Vector3.Scale(b.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));Vector2 screen=camera.WorldToViewportPoint(p);sample.min=Vector2.Min(sample.min,screen);sample.max=Vector2.Max(sample.max,screen);}}
        }
        sample.center=(sample.min+sample.max)*.5f;sample.panCenter=camera.WorldToViewportPoint(Match3DFlow.Instance.GetComponents<Match3DThemePresentation>().Single(p=>p.Active).PanBowlCenter);report.samples.Add(sample);
    }
}
