using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Run in a disposable project: enters Play mode and exits the batch editor.
[InitializeOnLoad]
public static class SoftContainerValidation {
    const string Key="soft-container-validation";
    const string Output="../results";
    [Serializable] class Sample { public string theme;public int seed,count,visible,escaped;public float size,height,minY,maxY; }
    [Serializable] class Report {public int assertions,fitCases;public List<Sample> boards=new List<Sample>();public string error;}
    static Report report;
    static SoftContainerValidation(){EditorApplication.playModeStateChanged+=Changed;}
    public static void Run(){SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Changed(PlayModeStateChange state){if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))EditorApplication.delayCall+=()=>new GameObject("Soft container checks").AddComponent<BoardValidationDriver>().StartCoroutine(Execute());}
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);report.assertions++;}
    static IEnumerator Execute(){
        yield return null;yield return null;report=new Report();Directory.CreateDirectory(Output);
        bool auto=Physics.autoSimulation,input=InputController.Instance.enabled;float size=BoardLayoutSettings.ModelSizeMultiplier;
        Physics.autoSimulation=false;InputController.Instance.enabled=false;
        var checks=Core();while(true){bool more=false;try{more=checks.MoveNext();}catch(Exception e){report.error=e.ToString();Debug.LogException(e);}if(!more||report.error!=null)break;yield return checks.Current;}
        (checks as IDisposable)?.Dispose();Match3DFlow.Instance.Home();Physics.autoSimulation=auto;InputController.Instance.enabled=input;BoardLayoutSettings.ModelSizeMultiplier=size;
        File.WriteAllText(Output+"/validation.json",JsonUtility.ToJson(report,true));SessionState.SetBool(Key,false);EditorApplication.Exit(report.error==null?0:1);
    }
    static void CheckClear(GameObject obj,Collider[] walls){
        foreach(var p in SoftTransitionContainer.SolidPoints(obj))Check(SoftTransitionContainer.Contains(p,.038f),"full solid bounds outside: "+obj.name+" "+p);
        foreach(var c in obj.GetComponentsInChildren<Collider>(true).Where(c=>!c.isTrigger&&c.gameObject.layer==ItemColliderGeometry.BodyLayer))foreach(var wall in walls){
            bool hit=Physics.ComputePenetration(c,c.transform.position,c.transform.rotation,wall,wall.transform.position,wall.transform.rotation,out var direction,out float distance);
            Check(!hit||distance<.001f,"solid/wall overlap: "+obj.name+" "+wall.name+" "+distance);
        }
    }
    static void Simulate(int steps){Physics.SyncTransforms();for(int i=0;i<steps;i++){LayeredItemPile.Instance.StepVisibility(.02f);Physics.Simulate(.02f);}}
    static IEnumerator Core(){
        Match3DFlow.Instance.Home();ItemColliderGeometry.ConfigureLayers();
        Check(!Resources.Load<TextAsset>("Match3D/Environments/CountrysideStove")&&!Resources.Load<TextAsset>("Match3D/SourceEnvironment"),"old container resources absent");
        var shell=SoftTransitionContainer.Create();var container=shell.GetComponent<SoftTransitionContainer>();var walls=shell.GetComponentsInChildren<Collider>();
        Check(Vector3.Distance(container.transform.up,Vector3.up)<.00001f,"stage 2 shell is upright");
        Check(Vector3.Distance(SoftTransitionContainer.ToWorld(Vector3.up*26),Vector3.up*26)<.00001f,"container axis has no sideways drift");
        Physics.SyncTransforms();var mesh=container.WallMesh;var triangles=mesh.triangles;var edges=new Dictionary<string,int>();
        for(int i=0;i<triangles.Length;i+=3)for(int j=0;j<3;j++){int a=triangles[i+j],b=triangles[i+(j+1)%3];string key=Math.Min(a,b)+":"+Math.Max(a,b);edges[key]=edges.TryGetValue(key,out int n)?n+1:1;}
        Check(edges.Values.All(n=>n==2),"wall closed manifold with annular rims");Check(!shell.GetComponentInChildren<MeshCollider>().convex,"hollow shell is not convex");
        foreach(float h in new[]{1f,10f,11f,12f,13f,14f,25f})for(int i=0;i<16;i++){
            float angle=i*Mathf.PI/8;var normal=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));
            var ray=new Ray(SoftTransitionContainer.ToWorld(Vector3.up*h),SoftTransitionContainer.ToWorld(normal));
            Check(walls[0].Raycast(ray,out var hit,10)&&Mathf.Abs(hit.distance-SoftTransitionContainer.Apothem(h))<.002f,"interior ray hits proper wall face at "+h+"/"+i);
        }
        Check(!walls[0].Raycast(new Ray(SoftTransitionContainer.ToWorld(Vector3.up),SoftTransitionContainer.ToWorld(Vector3.up)),out _,24),"no disk across cavity");
        Check(walls.Single(c=>c.name=="Container floor").Raycast(new Ray(SoftTransitionContainer.ToWorld(Vector3.up),SoftTransitionContainer.ToWorld(Vector3.down)),out var floor,2)&&Mathf.Abs(floor.distance-1)<.001f,"floor surface is local zero");
        Check(walls.Single(c=>c.name=="Container ceiling").Raycast(new Ray(SoftTransitionContainer.ToWorld(Vector3.up*25),SoftTransitionContainer.ToWorld(Vector3.up)),out var lid,2)&&Mathf.Abs(lid.distance-1)<.001f,"safety lid keeps interior height 26");
        var catalog=Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");Check(catalog&&catalog.themes.Length>=3,"three current model themes");
        var prefabs=catalog.themes.SelectMany(t=>t.profile.prefabs).Distinct().ToArray();var random=new System.Random(72131);
        foreach(float scale in new[]{.5f,1f,1.25f}){
            BoardLayoutSettings.ModelSizeMultiplier=scale;
            foreach(var prefab in prefabs){
                var obj=UnityEngine.Object.Instantiate(prefab);obj.transform.localScale*=BoardLayoutSettings.ModelScale(prefab);ItemColliderGeometry.Apply(obj,prefab.name,true);
                for(int i=0;i<8;i++){
                    obj.transform.rotation=BoardEnvironment.Rotation(random);obj.transform.position=SoftTransitionContainer.ToWorld(new Vector3(i%2==0?5:-5,new[]{.1f,10f,12f,13.5f,22f,25.9f,30f,36f}[i],3));
                    SoftTransitionContainer.Fit(obj);CheckClear(obj,walls);var fitted=obj.transform.position;SoftTransitionContainer.Fit(obj);
                    Check(Vector3.Distance(fitted,obj.transform.position)<.0001f,"fit is idempotent "+prefab.name);report.fitCases++;
                }
                UnityEngine.Object.DestroyImmediate(obj);
            }
        }
        UnityEngine.Object.DestroyImmediate(shell);BoardLayoutSettings.ModelSizeMultiplier=1;
        var flow=Match3DFlow.Instance;
        foreach(var theme in catalog.themes)foreach(float height in new[]{22f,30f}){
            var daily=new Match3DDailyLevel{date="2026-11-10",tier="bachelor",version="container-local",generatorVersion="daily-v1",modelThemeId=theme.id,seed=47231,seconds=600,initialCount=300,limitHeight=height-.5f,layers=theme.profile.layers.Select(l=>new ChallengeLayer(l.count,l.types,BoardEnvironment.MapHeight(l.height,22)*(height/22))).ToArray()};
            // Keep counts/type distribution; explicitly cover the accepted height-30 envelope.
            float oldTop=daily.layers.Max(l=>l.height);foreach(var layer in daily.layers)layer.height=1.5f+(layer.height-1.5f)*(height-1.5f)/(oldTop-1.5f);
            Check(daily.IsValid(theme.profile.prefabs.Length),"valid daily test "+theme.id);
            BoardLayoutSettings.ModelSizeMultiplier=height==30?1.25f:1;
            flow.BeginDailyPractice(0,daily);BoardCreationTestSupport.FinishPending(flow);Physics.SyncTransforms();
            Check(LevelGenerator.Instance.objectsOnScene.Count==15,"tutorial count");
            Check(BoardEnvironment.IsFirstStage&&!UnityEngine.Object.FindObjectOfType<SoftTransitionContainer>(),"stage 1 excludes new shell");
            var original=GameObject.Find(BoardEnvironment.CurrentName+" first stage container");
            Check(original&&original.GetComponentsInChildren<BoxCollider>(true).Length==25&&original.GetComponentsInChildren<Transform>(true).Any(t=>t.name=="Lv1Col"),"original first-stage collision hierarchy");
            Check(LevelGenerator.Instance.objectsOnScene.All(o=>o.transform.position.y>=1.5f&&o.transform.position.y<=6f),"unchanged tutorial spawn heights");
            var tutorial=Match3DModelThemes.Get(theme.id,theme.profile.environment,true);var replay=daily.RandomForStage(1);
            var tiles=LevelLayoutConfig.GenerateFirstStage(tutorial,replay);var expected=new Dictionary<int,Vector3>();float previousHeight=1.5f;
            foreach(var group in tiles.GroupBy(t=>t.layer).OrderBy(g=>g.Key)){
                foreach(var tile in group)expected[tile.id]=FirstStageContainer.Position(previousHeight,tile.height,replay);
                previousHeight=group.First().height;
            }
            foreach(var obj in LevelGenerator.Instance.objectsOnScene)Check(Vector3.Distance(obj.transform.position,expected[obj.GetComponent<Match3DTile>().id])<.00001f,"original tutorial spawn retained without new-container fitting");
            var presentation=flow.GetComponents<Match3DThemePresentation>().Single(p=>p.Active);
            Check(Vector3.Distance(presentation.PanBowlCenter,new Vector3(.05f,.36f,-.04f))<.001f,"first-stage authored pan position");
            Check(Quaternion.Angle(Camera.main.transform.rotation,Match3DThemePresentation.SourceRotation)<.001f,"first-stage authored camera pose");
            var firstPanWorld=presentation.PanBowlCenter;
            var firstPanViewport=Camera.main.WorldToViewportPoint(firstPanWorld);
            var oldColliders=original.GetComponentsInChildren<Collider>(true);
            flow.NextStage();BoardCreationTestSupport.FinishPending(flow);Physics.SyncTransforms();
            var boundary=UnityEngine.Object.FindObjectOfType<SoftTransitionContainer>();
            Check(boundary&&!BoardEnvironment.IsFirstStage,"stage 2 creates new shell");
            Check(oldColliders.All(c=>!c||!c.gameObject.activeInHierarchy),"first-stage walls disabled before simulation");
            Check(boundary.transform.parent==original.transform,"boundary owner reused across stages");
            var camera=Camera.main;
            Check(Vector3.Distance(camera.transform.forward,Vector3.down)<.00001f,"second-stage camera faces down the container axis");
            Check(Vector3.Distance(presentation.PanBowlCenter,firstPanWorld)<.00001f,"stage 2 preserves first-stage pan world position");
            var projectedPan=camera.WorldToViewportPoint(presentation.PanBowlCenter);
            Check(Vector2.Distance(projectedPan,firstPanViewport)<.00001f,"pan screen position remains unchanged across stages");
            var aligned= presentation.PanBowlCenter;presentation.SetStage(false);
            Check(Vector3.Distance(aligned,presentation.PanBowlCenter)<.0001f,"pan alignment does not accumulate");
            foreach(var c in GameObject.Find(BoardEnvironment.CurrentName+" source visuals").transform.Find("Grab3D Frying Pan").GetComponentsInChildren<Collider>())Check(c is MeshCollider&&c.enabled,"pan visual keeps attached solid mesh");
            var board=LevelGenerator.Instance.objectsOnScene;Check(board.Count==300,"second stage count");
            walls=boundary.GetComponentsInChildren<Collider>();foreach(var obj in board)CheckClear(obj,walls);
            Check(board.Count(o=>o.transform.position.y>25)<10,"no mass ceiling clamp on creation");Simulate(500);
            var sample=new Sample{theme=theme.id,seed=daily.seed,count=board.Count,height=height,size=BoardLayoutSettings.ModelSizeMultiplier,minY=board.Min(o=>o.transform.position.y),maxY=board.Max(o=>o.transform.position.y),visible=board.Count(o=>o.GetComponentsInChildren<Renderer>().Any(r=>r.enabled)),escaped=board.Count(o=>!SoftTransitionContainer.Contains(o.transform.position,-.1f))};report.boards.Add(sample);
            if(sample.escaped>0)File.WriteAllLines(Output+"/escaped.txt",board.Where(o=>!SoftTransitionContainer.Contains(o.transform.position,-.1f)).Select(o=>o.name+" "+o.GetComponent<BoardItemIdentity>().LogicalId+" pivot="+SoftTransitionContainer.ToLocal(o.transform.position)+" points="+string.Join(";",SoftTransitionContainer.SolidPoints(o).Select(p=>SoftTransitionContainer.ToLocal(p).ToString("F4")+":"+SoftTransitionContainer.Contains(p,-.1f)))));
            Check(sample.escaped==0,"no item center escapes after settling "+theme.id);Check(sample.visible>0&&sample.visible<300,"visible pile and hidden reserve");
            if(theme.id=="foodie-party"&&height==22&&SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null){
                ScreenCapture.CaptureScreenshot(Path.GetFullPath(Output+"/second-stage.png"));yield return null;yield return null;
            }
            flow.Home();yield return null;Check(!UnityEngine.Object.FindObjectOfType<SoftTransitionContainer>(),"home destroys shell");
        }
        // Actual coroutine/tween handoffs, including the first stage's five triples.
        BoardLayoutSettings.ModelSizeMultiplier=1;
        var first=catalog.themes[0];var live=new Match3DDailyLevel{date="2026-11-10",tier="bachelor",version="container-local",generatorVersion="daily-v1",modelThemeId=first.id,seed=17331,seconds=600,initialCount=300,limitHeight=first.profile.limitHeight,layers=first.profile.layers};
        flow.BeginDailyPractice(0,live);BoardCreationTestSupport.FinishPending(flow);Simulate(100);
        if(SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null){ScreenCapture.CaptureScreenshot(Path.GetFullPath(Output+"/first-stage.png"));yield return null;yield return null;}
        var input=InputController.Instance;var triples=LevelGenerator.Instance.objectsOnScene.GroupBy(o=>o.name).SelectMany(g=>g.Select((o,i)=>new{o,i}).GroupBy(v=>v.i/3).Select(g2=>g2.Select(v=>v.o).ToArray())).ToArray();
        foreach(var triple in triples){foreach(var obj in triple)input.AddObjectOnStack(obj,false);yield return new WaitForSeconds(1.1f);Check(input.objectsOnStack.Count==0,"tutorial triple clears");}
        float deadline=Time.realtimeSinceStartup+10;while(flow.Stage!=2&&Time.realtimeSinceStartup<deadline)yield return null;
        Check(flow.Stage==2,"tutorial completes naturally into stage 2");BoardCreationTestSupport.FinishPending(flow);Simulate(300);
        var item=LevelGenerator.Instance.objectsOnScene.OrderByDescending(o=>o.transform.position.y).First();var boardScale=item.transform.localScale;
        input.AddObjectOnStack(item,false);yield return new WaitForSeconds(.6f);Check(input.arrivedObject.Contains(item),"pickup arrives in tray");
        input.UndoLast();yield return new WaitForSeconds(.5f);Check(!input.objectsOnStack.Contains(item)&&Vector3.Distance(boardScale,item.transform.localScale)<.001f,"undo restores size and leaves tray");
        CheckClear(item,UnityEngine.Object.FindObjectOfType<SoftTransitionContainer>().GetComponentsInChildren<Collider>());
        Check(BoardTools.RunPaid(2,()=>BoardTools.MessUp()),"shuffle starts");yield return new WaitForSeconds(1.3f);Check(!BoardTools.Shuffling,"shuffle completes");
        // Every moved candidate is refitted before physical handoff. Unmoved resting
        // objects can touch walls, so inspect penetration rather than spawn margin.
        Simulate(300);Check(LevelGenerator.Instance.objectsOnScene.All(o=>SoftTransitionContainer.Contains(o.transform.position,-.1f)),"shuffle retains items inside");
        var phone=flow.GetComponent<Match3DPhoneShake>();Check(phone,"phone shake listener");
        int affected=0;
        for(int step=0;step<100;step++){
            affected+=phone.ProcessAcceleration(Vector3.right*(step%20<10?3:-3));Physics.Simulate(.02f);LayeredItemPile.Instance.StepVisibility(.02f);
        }
        Check(affected>0,"shake applies physical force to exposed items");Simulate(300);
        Check(LevelGenerator.Instance.objectsOnScene.All(o=>SoftTransitionContainer.Contains(o.transform.position,-.1f)),"shake retains items inside");
        flow.RestartStage();BoardCreationTestSupport.FinishPending(flow);Check(LevelGenerator.Instance.objectsOnScene.Count==300,"restart count");
        flow.Home();yield return null;Check(!UnityEngine.Object.FindObjectOfType<SoftTransitionContainer>(),"final cleanup");
    }
}
