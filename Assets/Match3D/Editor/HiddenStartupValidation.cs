using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class HiddenStartupValidation {
    static int checks;static string observations;
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);checks++;}
    [Serializable] class State {public string type;public int id;public Vector3 position;public Quaternion rotation;public bool visible,kinematic,pickable;public string colliders;}
    sealed class Replay {public readonly List<string> states=new List<string>();public float hiddenWait;public string random;}
    public static void Start(){var driver=new GameObject("Hidden startup validation").AddComponent<BoardValidationDriver>();driver.StartCoroutine(Run(driver));}
    static IEnumerator Run(BoardValidationDriver driver){
        var flow=Match3DFlow.Instance;var random=UnityEngine.Random.state;bool auto=Physics.autoSimulation,legacy=flow.UseLegacyCreationScheduleForValidation;
        double budget=flow.CreationBudgetOverride;float scale=Time.timeScale;checks=0;observations="";string failure=null;var steps=Core();
        while(true){bool more=false;try{more=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(!more||failure!=null)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();flow.Home();Physics.autoSimulation=auto;Time.timeScale=scale;UnityEngine.Random.state=random;flow.CreationBudgetOverride=budget;flow.UseLegacyCreationScheduleForValidation=legacy;
        string report=(failure==null?"PASS ":"FAIL ")+"hidden startup: "+checks+" assertions. "+failure+"\n"+observations;
        File.WriteAllText("Temp/goose-hidden-startup-validation.txt",report);Debug.Log(report);UnityEngine.Object.Destroy(driver.gameObject);
    }
    static Replay Capture(int theme,bool legacy){
        var flow=Match3DFlow.Instance;flow.Home();flow.UseLegacyCreationScheduleForValidation=legacy;flow.CreationBudgetOverride=double.PositiveInfinity;
        UnityEngine.Random.InitState(100+theme);flow.Begin(theme);UnityEngine.Random.InitState(937+theme);flow.NextStage();
        var pile=LayeredItemPile.Instance;float threshold=pile.Threshold;
        Check(!pile.IsInitiallyVisible(threshold-.001f)&&pile.IsInitiallyVisible(threshold)&&pile.IsInitiallyVisible(threshold+.001f),"same boundary comparison, equality visible");
        var board=LevelGenerator.Instance.objectsOnScene;var iterator=flow.TakeOverSourceCreationForValidation();var result=new Replay();int previous=0,tail=0;bool seenVisible=false;
        do{
            Check(board.Count-previous<=(legacy?3:Match3DFlow.HiddenCreationSliceLimit),"bounded creation slice");
            foreach(var obj in board.Skip(previous)){
                var body=obj.GetComponent<Rigidbody>();bool visible=pile.IsInitiallyVisible(obj.transform.position.y);seenVisible|=visible;
                Check(obj.GetComponentsInChildren<Renderer>(true).Any(r=>r.enabled&&r.gameObject.activeInHierarchy)==visible,"initial hidden render state preserved");
                if(!visible){Check(body.isKinematic&&!pile.CanPick(body),"hidden supports frozen and unpickable");Check(obj.GetComponentsInChildren<Collider>(true).Any(c=>c.enabled&&!c.isTrigger&&c.gameObject.activeInHierarchy),"hidden support collider retained");}
                result.states.Add(JsonUtility.ToJson(new State{type=obj.name,id=obj.GetComponent<Match3DTile>().id,position=obj.transform.position,rotation=obj.transform.rotation,visible=visible,kinematic=body.isKinematic,pickable=pile.CanPick(body),colliders=string.Join("|",obj.GetComponentsInChildren<Collider>(true).Select(c=>c.GetType().Name+":"+c.enabled+":"+c.isTrigger+":"+c.gameObject.activeInHierarchy))}));
            }
            float wait=(float)iterator.Current;if(!seenVisible)result.hiddenWait+=wait;
            if(wait>=.2f&&pile.Creating){float[] waits={.5f,.2f,.3f};Check(tail<3&&Mathf.Approximately(wait,waits[tail++]),"completion waits preserved");}
            previous=board.Count;
            BoardCreationTestSupport.AdvanceGrowth(wait);
        }while(iterator.MoveNext());
        Check(board.Count==300&&!flow.CreationInProgress&&GameManager.Instance.GameIsPlaying()&&tail==3,"300 unique complete board and completion gate");
        result.random=JsonUtility.ToJson(UnityEngine.Random.state);flow.Home();return result;
    }
    static IEnumerator Core(){
        var flow=Match3DFlow.Instance;Time.timeScale=1;Physics.autoSimulation=false;
        for(int theme=0;theme<3;theme++){
            var before=Capture(theme,true);var after=Capture(theme,false);
            int mismatch=Enumerable.Range(0,Math.Min(before.states.Count,after.states.Count)).FirstOrDefault(i=>before.states[i]!=after.states[i]);
            Check(before.states.SequenceEqual(after.states),"same layout/order/rotation/render/pick/rigid/collider states, theme "+theme+" index="+mismatch+" before="+before.states[mismatch]+" after="+after.states[mismatch]+" randomEqual="+(before.random==after.random));
            Check(before.random==after.random,"same random consumption, theme "+theme);
            Check(before.hiddenWait>=.99f&&after.hiddenWait==0,"fixed waits removed only before initially visible objects");
            observations+=$"Theme {theme}: hidden scheduled waits {before.hiddenWait:F3}s -> {after.hiddenWait:F3}s; 300 creation snapshots identical.\n";
            yield return null;
        }
        Physics.autoSimulation=true;flow.CreationBudgetOverride=-1;flow.UseLegacyCreationScheduleForValidation=false;
        // Same seeded stage two layout; fully warmed cache in each run. Observe only new objects and the first growing visible item.
        for(int mode=0;mode<2;mode++){
            flow.Begin(0);var preload=flow.GetComponent<Match3DStagePreloader>();float deadline=Time.realtimeSinceStartup+90;
            while(!preload.Ready&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(preload.Ready,"expanded preload completes");var profile=LevelLayoutConfig.Get("CountrysideStove");
            Check(profile.prefabs.All(p=>BoardItemPool.CachedCount(p)>=Match3DStagePreloader.CopiesPerType),"configured cached copies per type");
            var warmed=flow.GetComponentsInChildren<BoardItemPoolToken>(true).Where(t=>t.sourcePrefab&&profile.prefabs.Contains(t.sourcePrefab)).ToArray();
            Check(warmed.Length>=profile.prefabs.Length*Match3DStagePreloader.CopiesPerType&&warmed.All(t=>!t.gameObject.activeInHierarchy),"reserve is inactive and isolated");
            var ids=new HashSet<int>(warmed.Select(t=>t.gameObject.GetInstanceID()));
            flow.UseLegacyCreationScheduleForValidation=mode==0;UnityEngine.Random.InitState(481);
            float start=Time.realtimeSinceStartup;flow.NextStage();var board=LevelGenerator.Instance.objectsOnScene;var pile=LayeredItemPile.Instance;
            int inspected=0,usedCache=0;GameObject visible=null;var hidden=new List<GameObject>();deadline=start+30;
            while(Time.realtimeSinceStartup<deadline){
                for(;inspected<board.Count;inspected++){
                    var obj=board[inspected];if(ids.Contains(obj.GetInstanceID()))usedCache++;
                    if(!pile.IsInitiallyVisible(obj.transform.position.y))hidden.Add(obj);else if(!visible)visible=obj;
                }
                if(visible&&visible.transform.localScale.sqrMagnitude>0)break;
                yield return null;
            }
            Check(visible&&visible.transform.localScale.sqrMagnitude>0,"first visible item grows");
            Check(hidden.All(o=>!o.GetComponentsInChildren<Renderer>(true).Any(r=>r.enabled&&r.gameObject.activeInHierarchy)&&!pile.CanPick(o.GetComponent<Rigidbody>())),"initial hidden base still invisible/unpickable at first reveal");
            Check(usedCache>23,"expanded cache actually reused beyond one per type");
            observations+=$"Warm editor handoff {(mode==0?"legacy":"optimized")}: first visible {Time.realtimeSinceStartup-start:F3}s, created {board.Count}, cached used {usedCache}. Not phone timing.\n";
            flow.Home();yield return null;
        }
        flow.UseLegacyCreationScheduleForValidation=false;flow.Begin(0);
        var worker=flow.GetComponent<Match3DStagePreloader>();yield return null;Time.timeScale=0;yield return null;
        int paused=BoardItemPool.PooledCount;yield return new WaitForSecondsRealtime(.1f);
        Check(BoardItemPool.PooledCount==paused,"paused preload cannot stock");flow.Home();yield return null;
        Check(!worker.Running&&BoardItemPool.PooledCount==0,"cancelled preload leaves no reserve");
    }
}
