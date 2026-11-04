using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FirstStageValidation {
    static int checks;
    static void Check(bool value,string message){if(!value)throw new Exception(message);checks++;}
    sealed class RecordingRandom:IBoardRandom {
        public readonly List<Vector2> calls=new List<Vector2>();
        readonly BoardReplayRandom random=new BoardReplayRandom(42);
        public float Range(float min,float max){calls.Add(new Vector2(min,max));return random.Range(min,max);}
    }
    public static void ValidatePlans(){
        checks=0;
        var library=Resources.Load<Match3DTheme>("Match3D/ThemeLibrary");
        foreach(var theme in library.themes){
            var profile=LevelLayoutConfig.GetTutorial(theme.sourceEnvironment);
            Check(ReferenceEquals(profile.prefabs,LevelLayoutConfig.Get(theme.sourceEnvironment).prefabs),"shared theme prefab references");
            Check(profile.seconds==0&&profile.limitHeight==4,"tutorial time and height retained");
            var selections=new HashSet<string>();
            for(int seed=0;seed<1000;seed++){
                var plan=LevelLayoutConfig.GenerateFirstStage(profile,new BoardReplayRandom(seed));
                Check(plan.Count==15&&plan.Select(t=>t.id).Distinct().Count()==15,"15 unique objects");
                Check(plan.GroupBy(t=>t.type).Select(g=>g.Count()).OrderBy(n=>n).SequenceEqual(new[]{3,6,6}),"three types, complete triples");
                Check(plan.All(t=>t.type>=0&&t.type<profile.prefabs.Length&&profile.prefabs[t.type]&&t.layer==0&&t.height==6f),"valid theme models and tutorial positions");
                Check(plan.Count(t=>profile.prefabs[t.type].name==profile.tutorialNames[0])==3,"three fixed target items");
                Check(plan.All(t=>profile.tutorialNames.Contains(profile.prefabs[t.type].name)),"only curated tutorial models");
                selections.Add(string.Join(",",plan.Select(t=>t.type).Distinct().OrderBy(t=>t)));
            }
            Check(selections.Count>1,"selection varies across seeds");
        }
        Debug.Log("PASS first-stage theme plans: "+checks+" assertions");
    }
    public static void Start(){var driver=new GameObject("First stage validation").AddComponent<BoardValidationDriver>();driver.StartCoroutine(Run(driver));}
    static IEnumerator Run(BoardValidationDriver driver){
        checks=0;string failure=null;var flow=Match3DFlow.Instance;float oldScale=Time.timeScale;double oldBudget=flow.CreationBudgetOverride;
        var steps=Core();
        while(true){bool next=false;try{next=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(failure!=null||!next)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();flow.Home();Time.timeScale=oldScale;flow.CreationBudgetOverride=oldBudget;
        string report=(failure==null?"PASS":"FAIL")+" first stage: "+checks+" assertions. "+failure;
        File.WriteAllText("Temp/goose-first-stage-validation.txt",report);Debug.Log(report);UnityEngine.Object.Destroy(driver.gameObject);
    }
    static IEnumerator Core(){
        var flow=Match3DFlow.Instance;flow.Home();Time.timeScale=1;var board=LevelGenerator.Instance.objectsOnScene;
        for(int theme=0;theme<flow.library.themes.Length;theme++){
            string environment=flow.library.themes[theme].sourceEnvironment;
            var profile=LevelLayoutConfig.GetTutorial(environment);var random=new RecordingRandom();
            var plan=LevelLayoutConfig.GenerateFirstStage(profile,random);
            Check(ReferenceEquals(profile.prefabs,LevelLayoutConfig.Get(environment).prefabs),"first stage uses exact second-stage prefabs");
            Check(plan.Count==15&&plan.GroupBy(t=>t.type).Select(g=>g.Count()).OrderBy(n=>n).SequenceEqual(new[]{3,6,6}),"three random types with complete triples");
            Check(plan.All(t=>t.type>=0&&t.type<profile.prefabs.Length),"all types belong to theme");
            flow.Begin(theme);
            Check(flow.Stage==1&&flow.InitialCount==15&&board.Count==1&&flow.CreationInProgress,"entry emits first batch only");
            Check(InputController.Instance.UsesSourceFlight&&BoardTools.Enabled,"first stage uses common input and source tools");
            Check(Mathf.Approximately(LayeredItemPile.Instance.Threshold,-2),"initial visible threshold -2");
            var preload=flow.GetComponent<Match3DStagePreloader>();Check(preload.Running,"preload starts during first creation");
            float deadline=Time.realtimeSinceStartup+60;
            while((flow.CreationInProgress||preload.Running)&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(GameManager.Instance.GameIsPlaying()&&board.Count==15,"first stage ready");
            Check(preload.Ready&&preload.PreparedCount==LevelLayoutConfig.Get(environment).prefabs.Length,"preload visits every second-stage type");
            Check(board.All(o=>GameAudioCatalog.TryBoundProfile(o,out var _)),"source audio bound for first stage");
            Check(board.All(o=>o.GetComponentsInChildren<Renderer>(true).Any(r=>r.enabled&&r.gameObject.activeInHierarchy)),"all fifteen visible");
            var visual=GameObject.Find(environment+" source visuals");var shell=GameObject.Find(environment+" source collision reference");
            int visualId=visual.GetInstanceID(),shellId=shell.GetInstanceID();
            // Clear every triple; a partial clear must not advance the stage.
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
            flow.Home();yield return null;Check(board.Count==0&&BoardItemPool.PooledCount==0,"home clears board and warm pool");
        }
        flow.Begin(0);yield return null;flow.Home();yield return null;yield return null;
        Check(board.Count==0&&BoardItemPool.PooledCount==0&&!flow.GetComponent<Match3DStagePreloader>().Running,"cancel mid-preload has no late objects");
        flow.Begin(0);flow.NextStage();Check(flow.Stage==2&&board.Count>0&&board.Count<=Match3DFlow.HiddenCreationSliceLimit,"early transition falls back to normal source creation");flow.Home();
        // Warm-up is inert, and original / adapted models with the same name cannot collide.
        var banana=Resources.Load<LevelLayoutConfig>("Match3D/FirstStageCatalog").profiles[0].prefabs[0];
        var adapted=LevelLayoutConfig.Get("CountrysideStove").prefabs.First(p=>p.name=="Banana");
        UnityEngine.Random.InitState(197);var before=UnityEngine.Random.state;
        BoardItemPool.Prepare(banana);BoardItemPool.Prepare(adapted);
        Check(JsonUtility.ToJson(before)==JsonUtility.ToJson(UnityEngine.Random.state),"inactive warm clones do not consume gameplay RNG");
        var first=BoardItemPool.Get(banana,"Banana");var second=BoardItemPool.Get(adapted,"Banana");
        Check(first.GetComponent<BoardItemPoolToken>().sourcePrefab==banana&&second.GetComponent<BoardItemPoolToken>().sourcePrefab==adapted,"pool keyed by prefab identity");
        BoardItemPool.Recycle(first);BoardItemPool.Recycle(second);flow.Home();
    }
}
