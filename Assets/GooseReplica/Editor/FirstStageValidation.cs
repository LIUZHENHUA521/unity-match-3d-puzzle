using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FirstStageValidation {
    static int checks;
    static void Check(bool value,string message){if(!value)throw new Exception(message);checks++;}
    sealed class RecordingRandom:IReplicaSourceRandom {
        public readonly List<Vector2> calls=new List<Vector2>();
        readonly ReplicaSourceRandom random=new ReplicaSourceRandom(42);
        public float Range(float min,float max){calls.Add(new Vector2(min,max));return random.Range(min,max);}
    }
    public static void Start(){var driver=new GameObject("First stage validation").AddComponent<SourceValidationDriver>();driver.StartCoroutine(Run(driver));}
    static IEnumerator Run(SourceValidationDriver driver){
        checks=0;string failure=null;var flow=ReplicaFlow.Instance;float oldScale=Time.timeScale;double oldBudget=flow.CreationBudgetOverride;
        var steps=Core();
        while(true){bool next=false;try{next=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(failure!=null||!next)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();flow.Home();Time.timeScale=oldScale;flow.CreationBudgetOverride=oldBudget;
        string report=(failure==null?"PASS":"FAIL")+" first stage: "+checks+" assertions. "+failure;
        File.WriteAllText("Temp/goose-first-stage-validation.txt",report);Debug.Log(report);UnityEngine.Object.Destroy(driver.gameObject);
    }
    static IEnumerator Core(){
        var flow=ReplicaFlow.Instance;flow.Home();Time.timeScale=1;var board=LevelGenerator.Instance.objectsOnScene;
        for(int theme=0;theme<3;theme++){
            string environment=flow.library.themes[theme].sourceEnvironment;
            var profile=ReplicaSourceLevelPlan.GetTutorial(environment);var random=new RecordingRandom();
            var plan=ReplicaSourceLevelPlan.GenerateFirstStage(profile,random);
            Check(plan.Count==15&&plan.Count(t=>t.type==0)==3,"15 objects with three targets");
            Check(plan.Where(t=>t.type!=0).GroupBy(t=>t.type).Select(g=>g.Count()).OrderBy(n=>n).SequenceEqual(new[]{6,6}),"two ordinary types each six");
            Check(random.calls.Take(3).All(c=>c==new Vector2(0,1))&&random.calls[3].y==profile.ordinaryTypes.Length,"target draws precede ordinary selection");
            Check(plan.Skip(12).All(t=>t.type==0),"targets appended before common position sort");
            flow.Begin(theme);
            Check(flow.Stage==1&&flow.InitialCount==15&&board.Count==1&&flow.CreationInProgress,"entry emits first batch only");
            Check(InputController.Instance.UsesSourceFlight&&ReplicaSourceTools.Enabled,"first stage uses common input and source tools");
            Check(Mathf.Approximately(ReplicaLayeredPile.Instance.Threshold,-2),"initial visible threshold -2");
            var preload=flow.GetComponent<ReplicaStagePreloader>();Check(preload.Running,"preload starts during first creation");
            float deadline=Time.realtimeSinceStartup+60;
            while((flow.CreationInProgress||preload.Running)&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(GameManager.Instance.GameIsPlaying()&&board.Count==15,"first stage ready");
            Check(preload.Ready&&preload.PreparedCount==ReplicaSourceLevelPlan.Get(environment).prefabs.Length,"preload visits every second-stage type");
            Check(board.All(o=>ReplicaSourceAudio.TryBoundProfile(o,out var _)),"source audio bound for first stage");
            Check(board.All(o=>o.GetComponentsInChildren<Renderer>(true).Any(r=>r.enabled&&r.gameObject.activeInHierarchy)),"all fifteen visible");
            var visual=GameObject.Find(environment+" source visuals");var shell=GameObject.Find(environment+" source collision reference");
            int visualId=visual.GetInstanceID(),shellId=shell.GetInstanceID();
            // Actual source picks/triples, starting with targets: they alone cannot win.
            var triples=board.GroupBy(o=>o.name).OrderBy(g=>g.Key=="type_0"?0:1).SelectMany(g=>g.Select((o,i)=>new{o,i}).GroupBy(x=>x.i/3).Select(gr=>gr.Select(x=>x.o).ToArray())).ToArray();
            for(int i=0;i<triples.Length;i++){
                foreach(var obj in triples[i])InputController.Instance.AddObjectOnStack(obj);
                if(i==triples.Length-1)Check(flow.Stage==1&&!InputController.Instance.CanEvaluateWin,"last triple waits for animation checkpoint");
                yield return new WaitForSeconds(.85f);
                if(i<triples.Length-1)Check(flow.Stage==1&&InputController.Instance.objectsOnStack.Count==0,"partial clear cannot finish first stage");
            }
            Check(flow.Stage==2&&flow.InitialCount==300&&flow.CreationInProgress,"automatic second stage without result modal");
            Check(GameObject.Find(environment+" source visuals").GetInstanceID()==visualId&&GameObject.Find(environment+" source collision reference").GetInstanceID()==shellId,"scene and boundary retained");
            Check(!preload.Running,"handoff cancels preloader");
            flow.Home();yield return null;Check(board.Count==0&&ReplicaSourceObjectPool.PooledCount==0,"home clears board and warm pool");
        }
        flow.Begin(0);yield return null;flow.Home();yield return null;yield return null;
        Check(board.Count==0&&ReplicaSourceObjectPool.PooledCount==0&&!flow.GetComponent<ReplicaStagePreloader>().Running,"cancel mid-preload has no late objects");
        flow.Begin(0);flow.NextStage();Check(flow.Stage==2&&board.Count>0&&board.Count<=ReplicaFlow.HiddenCreationSliceLimit,"early transition falls back to normal source creation");flow.Home();
        // Warm-up is inert, and original / adapted models with the same name cannot collide.
        var banana=ReplicaSourceLevelPlan.GetTutorial("GroceryStore").prefabs.First(p=>p.name=="Banana");
        var adapted=ReplicaSourceLevelPlan.Get("CountrysideStove").prefabs.First(p=>p.name=="Banana");
        UnityEngine.Random.InitState(197);var before=UnityEngine.Random.state;
        ReplicaSourceObjectPool.Prepare(banana);ReplicaSourceObjectPool.Prepare(adapted);
        Check(JsonUtility.ToJson(before)==JsonUtility.ToJson(UnityEngine.Random.state),"inactive warm clones do not consume gameplay RNG");
        var first=ReplicaSourceObjectPool.Get(banana,"Banana");var second=ReplicaSourceObjectPool.Get(adapted,"Banana");
        Check(first.GetComponent<ReplicaSourcePoolToken>().sourcePrefab==banana&&second.GetComponent<ReplicaSourcePoolToken>().sourcePrefab==adapted,"pool keyed by prefab identity");
        ReplicaSourceObjectPool.Recycle(first);ReplicaSourceObjectPool.Recycle(second);flow.Home();
    }
}
