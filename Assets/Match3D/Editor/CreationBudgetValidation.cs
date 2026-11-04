using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public static class CreationBudgetValidation {
    static void Check(bool ok,string reason){if(!ok)throw new Exception(reason);}
    public static void Start(){var driver=new GameObject("Creation budget validation").AddComponent<BoardValidationDriver>();driver.StartCoroutine(Run(driver));}
    static IEnumerator Run(BoardValidationDriver driver){
        var flow=Match3DFlow.Instance;bool oldAuto=Physics.autoSimulation;float oldScale=Time.timeScale;double oldBudget=flow.CreationBudgetOverride;
        string failure=null;var steps=Core();
        while(true){bool next=false;try{next=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(!next||failure!=null)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();flow.Home();flow.CreationBudgetOverride=oldBudget;Physics.autoSimulation=oldAuto;Time.timeScale=oldScale;
        string report=failure==null?"PASS forced budget: one item per frame, pause at null yield, cancellation/restart, 300 distinct items and readiness; all prefab caches clone/reset with equivalent collider profiles; growing queue drains and falling full-size bodies still park. Editor only.\n":"FAIL "+failure;
        File.WriteAllText("Temp/goose-creation-budget-validation.txt",report);Debug.Log(report);UnityEngine.Object.Destroy(driver.gameObject);
    }
    static string Shape(Collider c){
        string shape=c is BoxCollider b?b.center+"/"+b.size:c is SphereCollider s?s.center+"/"+s.radius:c is CapsuleCollider k?k.center+"/"+k.radius+"/"+k.height+"/"+k.direction:c.GetType().Name;
        var m=c.sharedMaterial;return c.GetType().Name+"/"+c.gameObject.layer+"/"+c.enabled+"/"+c.isTrigger+"/"+shape+"/"+(m?m.dynamicFriction+"/"+m.staticFriction+"/"+m.bounciness+"/"+(int)m.frictionCombine+"/"+(int)m.bounceCombine:"");
    }
    static IEnumerator Core(){
        var flow=Match3DFlow.Instance;flow.Home();Physics.autoSimulation=false;Time.timeScale=1;
        foreach(var theme in flow.library.themes.Take(3))foreach(var prefab in LevelLayoutConfig.Get(theme.sourceEnvironment).prefabs){
            var obj=BoardItemPool.Get(prefab,prefab.name);
            try{
                var cache=obj.GetComponent<ItemColliderGeometry>();Check(cache&&cache.IsConfiguredFor(prefab.name),"clone keeps configured references: "+prefab.name);
                Check(prefab.GetComponent<ItemColliderGeometry>().IsConfiguredFor(prefab.name),"source prefab was baked");
                var colliders=obj.GetComponentsInChildren<Collider>(true);var before=colliders.Select(Shape).ToArray();
                var prefabStates=prefab.GetComponentsInChildren<Transform>(true).Select(t=>t.gameObject.activeSelf).ToArray();
                cache.SetVisible(false);Check(obj.GetComponentsInChildren<Transform>(true).Any(t=>!t.gameObject.activeSelf),"pick reference belongs to clone");
                Check(prefabStates.SequenceEqual(prefab.GetComponentsInChildren<Transform>(true).Select(t=>t.gameObject.activeSelf)),"clone cannot hide prefab children");cache.SetVisible(true);
                ItemColliderGeometry.Apply(obj,prefab.name,true,true);
                Check(before.SequenceEqual(obj.GetComponentsInChildren<Collider>(true).Select(Shape)),"bake equals freshly applied profile: "+prefab.name);
                int identity=obj.GetInstanceID();BoardItemPool.Recycle(obj,prefab.name);obj=BoardItemPool.Get(prefab,prefab.name);
                Check(obj.GetInstanceID()==identity,"pool identity retained");
                Check(before.SequenceEqual(obj.GetComponentsInChildren<Collider>(true).Select(Shape)),"recycle restores cached enabled/trigger states");
            }finally{UnityEngine.Object.DestroyImmediate(obj);}
        }
        flow.Home();flow.CreationBudgetOverride=0;Physics.autoSimulation=true;
        flow.Begin(0);flow.NextStage();var board=LevelGenerator.Instance.objectsOnScene;
        float deadline=Time.realtimeSinceStartup+10;
        while(flow.CreationBudgetYieldCount==0&&Time.realtimeSinceStartup<deadline)yield return null;
        Check(flow.CreationBudgetYieldCount>0,"budget path exercised");Time.timeScale=0;
        int paused=board.Count;yield return null;yield return new WaitForSecondsRealtime(.1f);
        Check(board.Count==paused&&flow.CreationInProgress,"null budget yield respects pause");
        flow.Home();yield return null;Check(board.Count==0&&!flow.CreationInProgress,"budget work cancelled");
        flow.Begin(0);flow.NextStage();int previous=board.Count;deadline=Time.realtimeSinceStartup+90; // 300 forced single-frame slices can exceed 25s in an unfocused editor.
        while(flow.CreationInProgress&&Time.realtimeSinceStartup<deadline){
            yield return null;Check(board.Count-previous<=1,"zero budget emits at most one object per frame");previous=board.Count;
            if(flow.CreationInProgress)Check(!GameManager.Instance.GameIsPlaying(),"input gated until finish");
        }
        Check(!flow.CreationInProgress&&board.Count==300&&board.Select(o=>o.GetComponent<Match3DTile>().id).Distinct().Count()==300,"restart completes unique board: count="+board.Count+", creating="+flow.CreationInProgress+", unique="+board.Select(o=>o.GetComponent<Match3DTile>().id).Distinct().Count());
        Check(GameManager.Instance.GameIsPlaying(),"readiness restored");
        flow.Home();Physics.autoSimulation=false;flow.CreationBudgetOverride=-1;flow.Begin(0);flow.NextStage();
        var creation=flow.TakeOverSourceCreationForValidation();while(board.Count<300){BoardCreationTestSupport.AdvanceGrowth((float)creation.Current);Check(creation.MoveNext(),"creation remains before finish");}
        Physics.SyncTransforms();var pile=LayeredItemPile.Instance;
        for(int i=0;i<500&&pile.CreationGrowingCount>0;i++){
            pile.StepVisibility(.02f);Physics.SyncTransforms();
            Check(pile.LastCreationGrowthWrites<=LayeredItemPile.CreationGrowthWriteLimit,"bounded collider scale writes");
            Check(pile.LastCreationReleases<=LayeredItemPile.CreationReleaseLimit,"bounded rigid releases");
        }
        Check(pile.CreationGrowingCount==0,"full-grown items exit growth queue");
        Check(pile.CreationPhysicsCount==ItemPhysicsController.Instance.DynamicCount,"only moving candidates retain creation height checks");
        var falling=board.Select(o=>o.GetComponent<Rigidbody>()).First(b=>!b.isKinematic);
        falling.position=new Vector3(0,pile.PhysicsThreshold-1,0);falling.velocity=Vector3.zero;pile.StepVisibility(.02f);
        Check(falling.isKinematic&&falling.detectCollisions,"full-grown falling item still parks without losing support");
        int total=pile.CreationPhysicsCount;var moving=board.Select(o=>o.GetComponent<Rigidbody>()).First(b=>!b.isKinematic);
        pile.Detach(moving);Check(pile.CreationPhysicsCount==total-1,"detach removes queued work");
        (creation as IDisposable)?.Dispose();flow.Home();Check(pile.CreationGrowingCount==0&&pile.CreationPhysicsCount==0,"clear removes queued work");
    }
}
