using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SourceCreationFramesValidation {
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static IEnumerator Run(){
  var flow=ReplicaFlow.Instance;float oldScale=Time.timeScale;bool oldSimulation=Physics.autoSimulation;
  string failure=null;var steps=Core();
  while(true){
   bool next=false;try{next=steps.MoveNext();}catch(Exception e){failure=e.ToString();}
   if(failure!=null||!next)break;
   yield return steps.Current;
  }
  (steps as IDisposable)?.Dispose();flow.Home();Time.timeScale=oldScale;Physics.autoSimulation=oldSimulation;
  Directory.CreateDirectory("Temp");
  string report=failure==null?"PASS live creation coroutine: first batch only; progress across frames; scaled-time pause; home cancels pending work; clean restart completes 300 distinct items and enables play after creation. This is editor timing, not device frame equivalence.\n":"FAIL live creation: "+failure+"\n";
  File.WriteAllText("Temp/goose-source-creation-frames-validation.txt",report);Debug.Log(report);
 }
 static IEnumerator Core(){
  Physics.autoSimulation=true;Time.timeScale=1;var flow=ReplicaFlow.Instance;var board=LevelGenerator.Instance.objectsOnScene;
  flow.Begin(0);flow.NextStage();
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
  flow.Begin(0);flow.NextStage();limit=Time.realtimeSinceStartup+15;
  while(flow.CreationInProgress&&Time.realtimeSinceStartup<limit)yield return null;
  Check(!flow.CreationInProgress&&board.Count==300,"new creation completes exactly 300 objects");
  Check(board.Select(x=>x.GetComponent<ReplicaTile>().id).Distinct().Count()==300,"no duplicates from cancelled runner");
  Check(GameManager.Instance.GameIsPlaying(),"play begins only after completed creation");
 }
}
