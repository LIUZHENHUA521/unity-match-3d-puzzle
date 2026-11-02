using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class TailStutterValidation {
    const string Key="goose-tail-validation";
    static TailStutterValidation(){EditorApplication.playModeStateChanged+=Changed;}
    public static void RunTimingBatch(){SessionState.SetBool(Key+"-timing",true);RunBatch();}
    public static void RunSelectionBatch(){SessionState.SetBool(Key+"-selection",true);RunBatch();}
    public static void RunBatch(){
        SessionState.SetBool(Key,true);
        EditorSceneManager.OpenScene("Assets/Scenes/GooseReplica.unity");
        EditorApplication.isPlaying=true;
    }
    static void Changed(PlayModeStateChange state){
        if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))
            EditorApplication.delayCall+=()=>new GameObject("Tail validation").AddComponent<SourceValidationDriver>().StartCoroutine(Run());
    }
    static void Check(bool value,string why){if(!value)throw new Exception(why);}
    static string report;
    static void Read(string path){Check(File.Exists(path),"missing report "+path);string text=File.ReadAllText(path);Check(text.StartsWith("PASS"),text);report+=text+"\n";}
    static IEnumerator Run(){
        yield return null;
        report="";string error=null;var steps=Core();
        while(true){bool more=false;try{more=steps.MoveNext();}catch(Exception e){error=e.ToString();}if(!more||error!=null)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();
        SessionState.SetBool(Key,false);
        Directory.CreateDirectory("Builds");File.WriteAllText("Builds/tail-stutter-validation.txt",(error==null?"PASS\n":"FAIL "+error+"\n")+report);
        Debug.Log((error==null?"TAIL_VALIDATION_PASS":"TAIL_VALIDATION_FAIL "+error)+"\n"+report);
        EditorApplication.Exit(error==null?0:1);
    }
    static IEnumerator Core(){
        var flow=ReplicaFlow.Instance;Check(flow,"flow initialized");flow.Home();
        if(SessionState.GetBool(Key+"-timing",false)){
            SessionState.SetBool(Key+"-timing",false);TimedGrowthChecks();yield return CreationTimingFrames();yield break;
        }
        if(SessionState.GetBool(Key+"-selection",false)){
            SessionState.SetBool(Key+"-selection",false);
            yield return SourceSelectionPhysicsFramesValidation.Run();Read("Temp/goose-source-selection-physics-frames-validation.txt");yield break;
        }
        foreach(var path in Directory.GetFiles("Assets/GooseReplica/Shaders/SourceItems","*.shader")){
            var shader=AssetDatabase.LoadAssetAtPath<Shader>(path);Check(shader&&!ShaderUtil.ShaderHasError(shader),"shader compiles "+path);
            var material=new Material(shader);Check(material.FindPass("SELECTION_OUTLINE")==-1,"old pass removed "+path);UnityEngine.Object.DestroyImmediate(material);
        }
        Check(ReplicaFlow.CreationSliceLimit(true,1f/60)==12&&ReplicaFlow.CreationSliceLimit(false,1f/60)==3,"healthy frame limits");
        Check(ReplicaFlow.CreationSliceLimit(true,.05f)==1&&ReplicaFlow.CreationSliceLimit(false,.05f)==1,"slow frame limits");
        var prefab=ReplicaSourceLevelPlan.Get("CountrysideStove").prefabs[0];
        Check(ReplicaSourceObjectPool.Prepare(prefab,2),"prewarm clone");
        Check(ReplicaSourceObjectPool.Prepare(prefab,2),"second prewarm clone");
        ReplicaSourceObjectPool.ResetMetrics();
        var a=ReplicaSourceObjectPool.Get(prefab,prefab.name);var b=ReplicaSourceObjectPool.Get(prefab,prefab.name);
        var ta=a.GetComponent<ReplicaSourcePoolToken>();var tb=b.GetComponent<ReplicaSourcePoolToken>();
        Check(ta.Transforms.All(t=>t==a.transform||t.IsChildOf(a.transform))&&tb.Transforms.All(t=>t==b.transform||t.IsChildOf(b.transform)),"cache references belong to each instance");
        Check(!ReferenceEquals(ta.Transforms,tb.Transforms)&&ReplicaSourceObjectPool.CacheHits==2&&ReplicaSourceObjectPool.CacheMisses==0,"separate clone arrays and warm hits");
        var transforms=ta.Transforms;var renderers=ta.Renderers;var colliders=ta.Colliders;
        ReplicaSourceObjectPool.Recycle(a);var reused=ReplicaSourceObjectPool.Get(prefab,prefab.name);
        Check(reused==a&&ReferenceEquals(transforms,ta.Transforms)&&ReferenceEquals(renderers,ta.Renderers)&&ReferenceEquals(colliders,ta.Colliders),"reuse retains component arrays");
        var renderer=ta.Renderers[0];renderer.enabled=false;Check(!ta.EnabledRenderers().Contains(renderer),"registration preserves disabled renderers");renderer.enabled=true;
        ReplicaSourceColliders.Apply(a,prefab.name,true,true);Check(ta.Transforms==null,"forced collider rebuild invalidates cache");ta.CacheComponents();
        Check(ta.Colliders.SequenceEqual(a.GetComponentsInChildren<Collider>(true)),"rebuilt colliders cached");
        ReplicaSourceObjectPool.Recycle(a);ReplicaSourceObjectPool.Recycle(b);ReplicaSourceObjectPool.Clear();
        report+="PASS shader pass removal, frame-pressure limits, inactive prewarm cache ownership, cache identity reuse, disabled renderer filtering, forced collider rebuild.\n";
        yield return null;
        SourceObjectPoolValidation.Run();Read("Temp/goose-source-object-pool-validation.txt");
        SourceCreationValidation.Run();Read("Temp/goose-source-creation-validation.txt");
        TimedGrowthChecks();
        yield return CreationTimingFrames();
        SourceHighlightValidation.Run();Read("Temp/goose-source-highlight-validation.txt");
        yield return SourceCreationFramesValidation.Run();Read("Temp/goose-source-creation-frames-validation.txt");
        string budgetPath="Temp/goose-creation-budget-validation.txt";if(File.Exists(budgetPath))File.Delete(budgetPath);
        CreationBudgetValidation.Start();float deadline=Time.realtimeSinceStartup+150;
        while(!File.Exists(budgetPath)&&Time.realtimeSinceStartup<deadline)yield return null;Read(budgetPath);
        string hiddenPath="Temp/goose-hidden-startup-validation.txt";if(File.Exists(hiddenPath))File.Delete(hiddenPath);
        HiddenStartupValidation.Start();deadline=Time.realtimeSinceStartup+240;
        while(!File.Exists(hiddenPath)&&Time.realtimeSinceStartup<deadline)yield return null;Read(hiddenPath);
        yield return RenderWarmupFrames();
        yield return PressureFrames();
        string trayPath="Temp/goose-tray-display-validation.txt";if(File.Exists(trayPath))File.Delete(trayPath);
        TrayDisplayValidation.Start();deadline=Time.realtimeSinceStartup+120;
        while(!File.Exists(trayPath)&&Time.realtimeSinceStartup<deadline)yield return null;Read(trayPath);
        yield return SourceSelectionPhysicsFramesValidation.Run();Read("Temp/goose-source-selection-physics-frames-validation.txt");
    }
    static void TimedGrowthChecks(){
        var flow=ReplicaFlow.Instance;flow.Home();ReplicaSourceEnvironment.Select(flow.library.themes[0].sourceEnvironment);var pile=ReplicaLayeredPile.Instance;var physics=ReplicaPilePhysics.Instance;
        bool auto=Physics.autoSimulation;float maximum=Time.maximumDeltaTime;Physics.autoSimulation=false;
        var bodies=new System.Collections.Generic.List<Rigidbody>();
        Action clear=()=>{pile.Clear();physics.Clear();foreach(var body in bodies)if(body)UnityEngine.Object.DestroyImmediate(body.gameObject);bodies.Clear();};
        Func<Rigidbody> add=()=>{
            var obj=new GameObject("Timed growth fixture");obj.transform.position=new Vector3(0,20+bodies.Count,0);
            var body=obj.AddComponent<Rigidbody>();body.detectCollisions=true;
            physics.Register(body,true);pile.Register(body,initialHeight:20+bodies.Count);bodies.Add(body);return body;
        };
        Action<int> reset=count=>{clear();pile.Begin(true,limitHeight:2);for(int i=0;i<count;i++)add();};
        try{
            reset(1);pile.StopCreationGrowth();
            Check(Time.maximumDeltaTime<=Time.fixedDeltaTime*2+.00001f,"growth stop retains loading timing scope");
            pile.FinishCreation();Check(Mathf.Approximately(Time.maximumDeltaTime,maximum),"tail finish restores timing scope");
            try{
                Time.maximumDeltaTime=Time.fixedDeltaTime*1.5f;float tighter=Time.maximumDeltaTime;
                reset(1);Check(Mathf.Approximately(Time.maximumDeltaTime,tighter),"creation does not relax a tighter existing cap");
                pile.Clear();Check(Mathf.Approximately(Time.maximumDeltaTime,tighter),"clear restores tighter original cap");
            }finally{pile.Clear();Time.maximumDeltaTime=maximum;}
            reset(1);Check(Time.maximumDeltaTime<=Time.fixedDeltaTime*2+.00001f,"creation limits physics catch-up");pile.StepVisibility(.219f);
            Check(bodies[0].transform.localScale.x<1&&bodies[0].isKinematic&&pile.CreationGrowingCount==1,".99 progress is not completion");
            pile.StepVisibility(.002f);
            Check(bodies[0].transform.localScale==Vector3.one&&bodies[0].isKinematic,"exact full size before next-frame release");
            pile.StepVisibility(.001f);Check(!bodies[0].isKinematic&&pile.CreationGrowingCount==0,"full size releases next frame");
            pile.FinishCreation();Check(Mathf.Approximately(Time.maximumDeltaTime,maximum),"finish restores original frame timing");
            reset(1);pile.StepVisibility(.1f);add();pile.StepVisibility(.01f);
            Check(Mathf.Abs(bodies[0].transform.localScale.x-.11f/ReplicaLayeredPile.CreationGrowSeconds)<.0001f&&Mathf.Abs(bodies[1].transform.localScale.x-.01f/ReplicaLayeredPile.CreationGrowSeconds)<.0001f,"independent registration start times");
            foreach(float dt in new[]{1f/60,1f/30}){
                reset(24);float elapsed=0;int frames=0;var lastWrite=new int[24];
                while(pile.CreationGrowingCount>0&&frames<60){
                    var before=bodies.Select(b=>b.transform.localScale).ToArray();elapsed+=dt;frames++;
                    pile.StepVisibility(dt);
                    Check(pile.LastCreationGrowthWrites<=12&&pile.LastCreationReleases<=3,"12 writes and 3 releases per rendered frame");
                    for(int i=0;i<bodies.Count;i++){
                        float size=bodies[i].transform.localScale.x;
                        if(bodies[i].transform.localScale!=before[i]){
                            Check(Mathf.Abs(size-Mathf.Clamp01(elapsed/ReplicaLayeredPile.CreationGrowSeconds))<.0001f,"slot samples accumulated time");
                            Check(frames-lastWrite[i]<=2,"24 entries update at least every second frame");lastWrite[i]=frames;
                        }
                        Check(bodies[i].isKinematic||size==1,"no early physics release");
                        Check(bodies[i].detectCollisions,"original collision participation preserved");
                    }
                    if(frames==1){
                        int pending=pile.CreationGrowingCount;var paused=bodies.Select(b=>b.transform.localScale).ToArray();
                        pile.StepVisibility(0);
                        Check(pile.CreationGrowingCount==pending&&pile.LastCreationReleases==0&&pile.LastCreationGrowthWrites==0&&bodies.Select(b=>b.transform.localScale).SequenceEqual(paused),"pause preserves queue, sizes and releases");
                    }
                }
                Check(pile.CreationGrowingCount==0&&elapsed<.6f&&bodies.All(b=>b.transform.localScale==Vector3.one),"bounded full-size completion at 30/60Hz");
            }
            reset(24);pile.StepVisibility(.5f);
            Check(pile.LastCreationGrowthWrites==12&&pile.LastCreationReleases==0&&bodies.Count(b=>b.transform.localScale==Vector3.one)==12,"long frame does not replay missed steps");
            foreach(float dt in new[]{1f/60,.07f,1f/30,.002f,.05f,.03f,.01f,.01f,.01f,.01f}){
                pile.StepVisibility(dt);Check(pile.LastCreationGrowthWrites<=12&&pile.LastCreationReleases<=3,"mixed frame budgets");
            }
            Check(pile.CreationGrowingCount==0,"mixed frames drain without starvation");
            report+="PASS timed growth: independent starts; 30/60Hz slot progress and <=2-frame gaps; exact full size before release; zero-delta pause; mixed/long frames bounded to 12 writes/3 releases; original collision participation.\n";
        }finally{clear();Physics.autoSimulation=auto;flow.Home();Check(Mathf.Approximately(Time.maximumDeltaTime,maximum),"cancel restores original frame timing");}
    }
    static IEnumerator CreationTimingFrames(){
        var flow=ReplicaFlow.Instance;flow.Home();float maximum=Time.maximumDeltaTime;
        flow.Begin(0);flow.NextStage();
        var probe=new GameObject("Creation fixed step probe").AddComponent<CreationFixedStepProbe>();
        try{
            yield return null;
            System.Threading.Thread.Sleep(120); // Deliberate test-only loading stall.
            yield return null;yield return null;
            Check(flow.CreationInProgress&&probe.MaximumSteps>0&&probe.MaximumSteps<=2,"loading stall cannot cascade into many physics steps");
            flow.Home();Check(Mathf.Approximately(Time.maximumDeltaTime,maximum),"home restores frame timing after a stall");
            flow.Begin(0);flow.NextStage();flow.RestartStage();
            Check(Time.maximumDeltaTime<=Time.fixedDeltaTime*2+.00001f,"restart owns a fresh loading timing scope");
            flow.Home();Check(Mathf.Approximately(Time.maximumDeltaTime,maximum),"restart cancellation restores original timing");
            report+=$"PASS actual 120ms loading stall: at most {probe.MaximumSteps} FixedUpdates per frame; home/restart restore original maximumDeltaTime.\n";
        }finally{flow.Home();UnityEngine.Object.Destroy(probe.gameObject);}
    }
    static IEnumerator RenderWarmupFrames(){
        var flow=ReplicaFlow.Instance;flow.Home();yield return null;
        var main=Camera.main;int bodies=UnityEngine.Object.FindObjectsOfType<Rigidbody>().Length;
        int cameras=UnityEngine.Object.FindObjectsOfType<Camera>().Length;
        int prepared=0;
        using(var warmup=new ReplicaSourceRenderWarmup()){
            foreach(var theme in flow.library.themes.Take(3))foreach(var prefab in ReplicaSourceLevelPlan.Get(theme.sourceEnvironment).prefabs){
                string random=JsonUtility.ToJson(UnityEngine.Random.state);var active=RenderTexture.active;
                bool first=warmup.Prepare(prefab);if(first)prepared++;
                Check(!warmup.Prepare(prefab),"same type renders only once per preloader lifetime");
                Check(JsonUtility.ToJson(UnityEngine.Random.state)==random,"warmup preserves layout RNG");
                Check(RenderTexture.active==active&&Camera.main==main,"warmup preserves main render target/camera");
                Check(UnityEngine.Object.FindObjectsOfType<Rigidbody>().Length==bodies,"no physics instances during warmup");
                yield return null;
            }
            Check(warmup.PreparedCount==prepared&&prepared>50,"all three themes render warmup");
        }
        yield return null;
        Check(UnityEngine.Object.FindObjectsOfType<Camera>().Length==cameras,"warmup camera disposed");
        report+=$"PASS offscreen rendering for {prepared} unique prefabs: no RNG/physics/main camera mutation, duplicate calls skipped, camera cleanup.\n";
    }
    static IEnumerator PressureFrames(){
        var flow=ReplicaFlow.Instance;flow.Home();flow.CreationFrameSecondsOverride=.05f;
        try{
            flow.Begin(0);flow.NextStage();var board=LevelGenerator.Instance.objectsOnScene;
            Check(board.Count==1,"slow initial frame creates only one item");
            int previous=board.Count;float deadline=Time.realtimeSinceStartup+90;
            while(flow.CreationInProgress&&Time.realtimeSinceStartup<deadline){
                yield return null;Check(board.Count-previous<=1,"actual slow-frame path bounds every frame");previous=board.Count;
            }
            Check(!flow.CreationInProgress&&board.Count==300&&GameManager.Instance.GameIsPlaying(),"slow frames still complete");
            Check(flow.SlowFrameCreationYieldCount>0,"adaptive path actually used");
            Check(ReplicaSourceObjectPool.CacheHits+ReplicaSourceObjectPool.CacheMisses==300,"cache metrics cover all stage items");
            report+=$"PASS forced 50ms frame feedback: 300 items complete, at most one per frame; adaptive yields {flow.SlowFrameCreationYieldCount}; pool hit/miss {ReplicaSourceObjectPool.CacheHits}/{ReplicaSourceObjectPool.CacheMisses}.\n";
        }finally{flow.Home();flow.CreationFrameSecondsOverride=-1;}
        flow.Begin(0);var preloader=flow.GetComponent<ReplicaStagePreloader>();
        Check(preloader.PreparedInstanceCount==0,"no preload clones during tutorial creation");
        float end=Time.realtimeSinceStartup+30;
        while(preloader.PreparedInstanceCount==0&&Time.realtimeSinceStartup<end)yield return null;
        Check(preloader.PreparedInstanceCount>0&&!flow.CreationInProgress,"preload begins after tutorial creation");
        Time.timeScale=0;yield return null;int count=preloader.PreparedInstanceCount;
        yield return new WaitForSecondsRealtime(.1f);Check(preloader.PreparedInstanceCount==count,"active preloader pauses");Time.timeScale=1;
        flow.NextStage();count=preloader.PreparedInstanceCount;yield return null;
        Check(!preloader.Running&&preloader.PreparedInstanceCount==count,"fast handoff cancels preloader");flow.Home();
        report+="PASS active preload pause and fast-handoff cancellation.\n";
    }

}

public sealed class CreationFixedStepProbe : MonoBehaviour {
    int steps;public int MaximumSteps {get;private set;}
    void FixedUpdate(){steps++;}
    void LateUpdate(){MaximumSteps=Mathf.Max(MaximumSteps,steps);steps=0;}
}
