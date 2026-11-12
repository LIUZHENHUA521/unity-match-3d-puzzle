using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad] public static class OpeningSeparationValidation {
    const string Pending="opening-separation.validation",Output="ValidationOutput/opening-separation";
    [Serializable] sealed class Export {public Match3DDailySnapshot opening;}
    static readonly List<string> results=new List<string>();
    static OpeningSeparationValidation(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Pending,false)){SessionState.SetBool(Pending,false);EditorApplication.delayCall+=()=>new GameObject("Opening regression").AddComponent<BoardValidationDriver>().StartCoroutine(Run());}};}
    public static void RunBatch(){SessionState.SetBool(Pending,true);Physics.simulationMode=SimulationMode.FixedUpdate;EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    public static void RunInteractionBatch(){SessionState.SetBool("opening-interaction-only",true);RunBatch();}
    public static void RunFreshBatch(){SessionState.SetBool("opening-fresh-only",true);RunBatch();}
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);results.Add("PASS "+message);File.WriteAllLines(Output+"/report.txt",results);}
    public static Match3DDailyLevel LegacyLevel()=>new Match3DDailyLevel{date="2026-11-13",tier="bachelor",candidateId="candidate-0123456789abcdef01234567",version="regression",generatorVersion="shared-v2",modelThemeId="foodie-party",seed=47231,seconds=600,initialCount=300,limitHeight=21.5f,layers=new[]{new ChallengeLayer(150,23,14),new ChallengeLayer(60,10,18),new ChallengeLayer(45,8,20),new ChallengeLayer(45,7,22)},opening=JsonUtility.FromJson<Export>(File.ReadAllText("Tools/validation/OpeningSeparation/legacy-opening.json")).opening};
    static IEnumerator Wait(){
        float deadline=Time.realtimeSinceStartup+120;
        while(Match3DFlow.Instance.CreationInProgress){if(Time.realtimeSinceStartup>deadline)throw new Exception("Opening timeout");yield return null;}
        Check(string.IsNullOrEmpty(Match3DFlow.Instance.CreationFailure),"opening completed without failure");yield return null;
    }
    static float Depth(Collider[] a,Collider[] b){float max=0;foreach(var x in a)foreach(var y in b)if(x.bounds.Intersects(y.bounds)&&Physics.ComputePenetration(x,x.transform.position,x.transform.rotation,y,y.transform.position,y.transform.rotation,out _,out var depth))max=Mathf.Max(max,depth);return max;}
    static float Inspect(string label,bool requireClean=true){
        Physics.SyncTransforms();var objects=LevelGenerator.Instance.objectsOnScene.Where(o=>o&&o.activeInHierarchy).ToArray();
        var solids=objects.Select(o=>o.GetComponentsInChildren<Collider>().Where(c=>c.enabled&&!c.isTrigger&&c.gameObject.layer==ItemColliderGeometry.BodyLayer).ToArray()).ToArray();
        var visible=objects.Select(o=>LayeredItemPile.Instance.DailyVisible(o.GetComponent<Rigidbody>())).ToArray();
        float max=0,visibleMax=0;int pairs=0;
        for(int i=0;i<objects.Length;i++)for(int j=0;j<i;j++)if(visible[i]||visible[j]){float depth=Depth(solids[i],solids[j]);max=Mathf.Max(max,depth);if(visible[i]&&visible[j])visibleMax=Mathf.Max(visibleMax,depth);if(depth>.011f)pairs++;}
        var walls=UnityEngine.Object.FindObjectsOfType<Collider>().Where(c=>c.enabled&&!c.isTrigger&&!c.attachedRigidbody&&(c.gameObject.layer==0||c.gameObject.layer==ItemColliderGeometry.WallLayer)).ToArray();
        float wallDepth=0;string worst="";for(int i=0;i<objects.Length;i++)if(visible[i])foreach(var wall in walls){float d=Depth(solids[i],new[]{wall});if(d>wallDepth){wallDepth=d;var body=objects[i].GetComponent<Rigidbody>();worst=objects[i].name+"/"+objects[i].GetComponent<Match3DTile>().id+" at "+body.position+" vs "+wall.name+", kinematic="+body.isKinematic+", sleeping="+body.IsSleeping();}}
        if(requireClean&&Match3DFlow.Instance.Stage==2){
            Check(solids.Where((s,i)=>visible[i]).SelectMany(s=>s).All(c=>SoftTransitionContainer.Contains(c.bounds.center)),label+" every primitive center inside shell");
            Check(solids.Where((s,i)=>visible[i]).SelectMany(s=>s).All(c=>c.bounds.max.y<=SoftTransitionContainer.Height+.011f),label+" full bodies below lid and camera near plane");
        }
        results.Add($"INFO {label}: objects={objects.Length}, visible={visible.Count(v=>v)}, penetratingPairs={pairs}, maxDepth={max:F6}, wallDepth={wallDepth:F6}, visiblePairMax={visibleMax:F6}, worstWall={worst}");
        if(requireClean){Check(pairs==0,label+" visible-to-any bodies have no penetration > .011");Check(wallDepth<=.011f,label+" visible bodies clear actual container and pan");}
        return Mathf.Max(max,wallDepth);
    }
    static void Same(Match3DDailyStage expected,string label){
        var state=Match3DFlow.Instance.CaptureDailyOpening();
        Check(state.items.Length==expected.items.Length,label+" count");
        Check(state.items.Zip(expected.items,(a,b)=>a.id==b.id&&a.type==b.type&&a.visible==b.visible&&Vector3.Distance(a.position,b.position)<.00001f&&Quaternion.Angle(a.rotation,b.rotation)<.05f&&Vector3.Distance(a.scale,b.scale)<.00001f).All(v=>v),label+" exact poses, types, visibility and sizes");
    }
    static IEnumerator Core(){
        yield return null;yield return null;Physics.simulationMode=SimulationMode.FixedUpdate;Time.timeScale=1;
        var flow=Match3DFlow.Instance;
        if(SessionState.GetBool("opening-interaction-only",false)){
            SessionState.SetBool("opening-interaction-only",false);
            yield return BoardSelectionPhysicsFramesValidation.Run();
            Check(File.ReadAllText("Temp/goose-source-selection-physics-frames-validation.txt").StartsWith("PASS"),"manual selection/contact frame regression");
            yield return BoardFlightTimingValidation.Run();
            Check(File.ReadAllText("Temp/goose-source-flight-timing-validation.txt").StartsWith("PASS"),"pickup/match/pause/undo frame regression");
            yield break;
        }
        if(!SessionState.GetBool("opening-fresh-only",false)){
        var original=LegacyLevel();flow.PreviewDailyOpening(original,2);yield return Wait();
        Check(ItemPhysicsController.Instance.DailyOpeningHeld,"legacy opening stays held before first input");Inspect("legacy repaired");var repaired=flow.CaptureDailyOpening();CaptureImage();
        var originalItems=LegacyLevel().opening.stages[1].items;
        Check(repaired.items.Zip(originalItems,(a,b)=>a.type==b.type&&a.id==b.id&&a.scale==b.scale&&a.visible==b.visible).All(v=>v),"legacy repair preserves 300 IDs, types, sizes and visibility");
        Check(repaired.items.Where(i=>i.visible).All(i=>i.dynamic),"legacy visible/static flags corrected for release");
        yield return new WaitForSeconds(1);Same(repaired,"idle");
        flow.RestartStage();yield return Wait();Same(repaired,"cached retry");Inspect("cached retry");
        flow.PreviewDailyOpening(LegacyLevel(),2);yield return Wait();Same(repaired,"independent repair repeat");
        var target=LevelGenerator.Instance.objectsOnScene.First(o=>LayeredItemPile.Instance.CanPick(o.GetComponent<Rigidbody>()));
        InputController.Instance.AddObjectOnStack(target);Check(!ItemPhysicsController.Instance.DailyOpeningHeld,"first pick releases opening");
        Check(LevelGenerator.Instance.objectsOnScene.Where(o=>o!=target&&LayeredItemPile.Instance.DailyVisible(o.GetComponent<Rigidbody>())).All(o=>!o.GetComponent<Rigidbody>().isKinematic),"all remaining visible bodies dynamic after pick");
        yield return new WaitForSeconds(2);float falling=Inspect("after first pick",false);yield return new WaitForSeconds(8);float settled=Inspect("after first pick 10s",false);Check(settled<.1f,"first-pick settled contact penetration below .1");
        flow.PreviewDailyOpening(LegacyLevel(),1);yield return Wait();Inspect("first stage");Check(LevelGenerator.Instance.objectsOnScene.Count==15,"first stage retains 15 items");
        }
        SessionState.SetBool("opening-fresh-only",false);
        foreach(int seed in new[]{47231,42}){
            var level=LegacyLevel();level.generatorVersion="daily-v1";level.opening=null;level.seed=seed;
            flow.PreviewDailyOpening(level,2);yield return Wait();Inspect("fresh seed "+seed);Check(LevelGenerator.Instance.objectsOnScene.Count==300,"fresh count remains 300");
            var state=flow.CaptureDailyOpening();yield return new WaitForSeconds(.5f);Same(state,"fresh idle");
        }
        var pending=LegacyLevel();pending.generatorVersion="daily-v1";pending.opening=null;flow.PreviewDailyOpening(pending,2);yield return null;flow.Home();yield return null;yield return null;
        Check(!flow.CreationInProgress&&!ItemPhysicsController.Instance.DailyOpeningHeld&&LevelGenerator.Instance.objectsOnScene.Count==0,"Home cancels creation and clears held state");
        VerifyFailureAndRandom();
    }
    static void CaptureImage(){
        if(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Null)return;
        var camera=Camera.main;if(!camera)return;
        var previous=camera.targetTexture;var active=RenderTexture.active;
        var target=new RenderTexture(600,900,24);var image=new Texture2D(600,900,TextureFormat.RGB24,false);
        try{camera.targetTexture=target;Match3DFlow.Instance.GetComponent<Match3DThemePresentation>().Fit();Match3DTray.Instance.SendMessage("Layout");camera.Render();RenderTexture.active=target;image.ReadPixels(new Rect(0,0,600,900),0,0);image.Apply();File.WriteAllBytes(Output+"/repaired-opening.png",image.EncodeToPNG());}
        finally{camera.targetTexture=previous;Match3DFlow.Instance.GetComponent<Match3DThemePresentation>().Fit();Match3DTray.Instance.SendMessage("Layout");RenderTexture.active=active;UnityEngine.Object.Destroy(target);UnityEngine.Object.Destroy(image);}
    }
    static void VerifyFailureAndRandom(){
        var shell=SoftTransitionContainer.Create();var cube=new GameObject("Opening failure fixture");cube.layer=ItemColliderGeometry.BodyLayer;
        var body=cube.AddComponent<Rigidbody>();body.isKinematic=true;var shape=cube.AddComponent<BoxCollider>();shape.size=Vector3.one*100;
        cube.transform.position=new Vector3(0,25,0);var before=cube.transform.position;var rotation=cube.transform.rotation;var random=UnityEngine.Random.state;
        bool failed=false;
        try{var repair=new BoardOpeningSeparation(new[]{cube},_=>true,false,21);for(int i=0;i<514&&!repair.Complete;i++)repair.Step();repair.Apply();}
        catch(InvalidOperationException){failed=true;}
        Check(failed,"oversized fixture fails bounded search");
        Check(cube.transform.position==before&&cube.transform.rotation==rotation,"failed repair leaves original transform untouched");
        Check(JsonUtility.ToJson(random)==JsonUtility.ToJson(UnityEngine.Random.state),"repair preserves Unity gameplay random state");
        UnityEngine.Object.DestroyImmediate(cube);UnityEngine.Object.DestroyImmediate(shell);
    }
    static IEnumerator Run(){
        Directory.CreateDirectory(Output);var stack=new Stack<IEnumerator>();stack.Push(Core());string failure=null;
        while(stack.Count>0){bool next=false;object current=null;try{next=stack.Peek().MoveNext();if(next)current=stack.Peek().Current;}catch(Exception e){failure=e.ToString();break;}
            if(!next){(stack.Pop() as IDisposable)?.Dispose();continue;}if(current is IEnumerator nested)stack.Push(nested);else yield return current;
        }
        while(stack.Count>0)(stack.Pop() as IDisposable)?.Dispose();
        if(failure!=null)results.Add("FAIL "+failure);File.WriteAllLines(Output+"/report.txt",results);Debug.Log(string.Join("\n",results));EditorApplication.Exit(failure==null?0:1);
    }
}
