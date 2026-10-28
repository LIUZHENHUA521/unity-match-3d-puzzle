using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SourceCreationFramesValidation {
 static string observations;
 public static void Start(){
  var driver=new GameObject("Creation frame validation").AddComponent<SourceValidationDriver>();
  driver.StartCoroutine(RunAndDispose(driver));
 }
 static IEnumerator RunAndDispose(SourceValidationDriver driver){
  yield return Run();UnityEngine.Object.Destroy(driver.gameObject);
 }
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static IEnumerator Run(){
  var flow=ReplicaFlow.Instance;float oldScale=Time.timeScale;bool oldSimulation=Physics.autoSimulation;
  observations="";string failure=null;var steps=Core();
  while(true){
   bool next=false;try{next=steps.MoveNext();}catch(Exception e){failure=e.ToString();}
   if(failure!=null||!next)break;
   yield return steps.Current;
  }
  (steps as IDisposable)?.Dispose();flow.Home();Time.timeScale=oldScale;Physics.autoSimulation=oldSimulation;
  Directory.CreateDirectory("Temp");
  string report=(failure==null?"PASS live creation coroutine: first batch only; progress across frames; scaled-time pause; home cancels pending work; all three themes progressively reveal/grow before creation completes; restart creates 300 distinct items and enables play only after creation. This is editor timing, not device frame equivalence.\n":"FAIL live creation: "+failure+"\n")+observations;
  File.WriteAllText("Temp/goose-source-creation-frames-validation.txt",report);Debug.Log(report);
 }
 static IEnumerator Core(){
  Physics.autoSimulation=true;Time.timeScale=1;var flow=ReplicaFlow.Instance;var board=LevelGenerator.Instance.objectsOnScene;
  flow.Begin(0);
  Check(flow.CreationInProgress&&board.Count==1,"only source first batch exists on entry");
  Check(!GameManager.Instance.GameIsPlaying(),"no play before creation completes");
  int observed=board.Count;float limit=Time.realtimeSinceStartup+5;
  while(board.Count==observed&&Time.realtimeSinceStartup<limit)yield return null;
  Check(board.Count>observed&&board.Count<300,"creation continues in later frames");
  Time.timeScale=0;observed=board.Count;
  // A WaitForSeconds already due in this frame can resume after this coroutine.
  // Allow that one scheduled batch, then verify a sustained pause across real time.
  yield return null;
  Check(board.Count<=observed+3,"pause cannot advance more than the already-due batch");
  observed=board.Count;yield return new WaitForSecondsRealtime(.12f);
  Check(board.Count==observed&&flow.CreationInProgress,"paused game time does not spawn more items");
  flow.Home();Check(!flow.CreationInProgress&&board.Count==0,"home cancels active creation");
  yield return new WaitForSecondsRealtime(.08f);
  Check(board.Count==0&&!GameManager.Instance.GameIsPlaying(),"cancelled creation cannot resume or enable play");
  for(int theme=0;theme<3;theme++){
   float started=Time.realtimeSinceStartup,firstVisible=-1;
   int countAtFirstVisible=0,previousCount=0;bool sawGrowth=false;
   flow.Begin(theme);limit=Time.realtimeSinceStartup+20;
   var profile=ReplicaSourceLevelPlan.Get(flow.library.themes[theme].sourceEnvironment);
   while(flow.CreationInProgress&&Time.realtimeSinceStartup<limit){
    Check(!GameManager.Instance.GameIsPlaying(),"input remains locked while growing");
    Check(board.Count-previousCount<=3,"at most one source batch per rendered frame");previousCount=board.Count;
    foreach(var obj in board){
     if(!obj.GetComponentsInChildren<Renderer>(true).Any(r=>r.enabled&&r.gameObject.activeInHierarchy))continue;
     float size=obj.transform.localScale.magnitude;if(size<=0)continue;
     if(firstVisible<0){firstVisible=Time.realtimeSinceStartup-started;countAtFirstVisible=board.Count;}
     int type=int.Parse(obj.name.Substring(5));var prefab=profile.prefabs[type];
     float full=prefab.transform.localScale.magnitude*ReplicaSourceProfile.ModelScale(prefab.name);
     if(size<full*.99f)sawGrowth=true;
    }
    yield return null;
   }
   Check(!flow.CreationInProgress&&board.Count==300,"new creation completes exactly 300 objects");
   Check(firstVisible>=0&&countAtFirstVisible<300&&sawGrowth,"visible objects progressively grow before all 300 exist");
   Check(board.Select(x=>x.GetComponent<ReplicaTile>().id).Distinct().Count()==300,"no duplicates from cancelled runner");
   Check(GameManager.Instance.GameIsPlaying(),"play begins only after completed creation");
   observations+=$"Theme {theme}: first visible {firstVisible:F3}s at {countAtFirstVisible} objects; finished {Time.realtimeSinceStartup-started:F3}s; progressive growth observed. Editor only.\n";
   flow.Home();yield return null;
  }
 }
}
