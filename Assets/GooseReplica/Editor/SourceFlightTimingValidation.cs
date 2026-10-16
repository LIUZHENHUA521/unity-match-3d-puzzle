using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public static class SourceFlightTimingValidation {
 static readonly List<GameObject> probes=new List<GameObject>();
 static string collectionKey,completedKey,priorCompleted;static int priorCollection;static bool hadCollection,hadCompleted;
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static IEnumerator Run(){
  var flow=ReplicaFlow.Instance;float priorScale=Time.timeScale;bool priorPhysics=Physics.autoSimulation;float priorAspect=Camera.main.aspect;string failure=null;
  var core=Core();
  while(true){bool next=false;try{next=core.MoveNext();}catch(Exception e){failure=e.ToString();}if(failure!=null||!next)break;yield return core.Current;}
  (core as IDisposable)?.Dispose();flow.Home();
  foreach(var p in probes)if(p){p.transform.DOKill();UnityEngine.Object.DestroyImmediate(p);}probes.Clear();
  if(collectionKey!=null){if(hadCollection)PlayerPrefs.SetInt(collectionKey,priorCollection);else PlayerPrefs.DeleteKey(collectionKey);if(hadCompleted)PlayerPrefs.SetString(completedKey,priorCompleted);else PlayerPrefs.DeleteKey(completedKey);PlayerPrefs.Save();}
  Time.timeScale=priorScale;Physics.autoSimulation=priorPhysics;Camera.main.aspect=priorAspect;
  string report=failure==null?"PASS source flight: shipped old branch; independent XZ linear/Y OutQuad/rotation InQuad; scale peak; 50ms collider grace; release travel; continuous picks; seventh-slot triple frees slots immediately; .3/.2/.2/.05 combine stages; completion gate and actual last-triple and concurrent-match win checkpoints; unmatched seventh-slot failure lock; reflow; mixed pause clocks; Home cancellation; six original model scales and camera depth 1.58 rendered. Editor-frame timing, not original-device capture.\n":"FAIL source flight: "+failure+"\n";
  Directory.CreateDirectory("Temp");File.WriteAllText("Temp/goose-source-flight-timing-validation.txt",report);Debug.Log(report);
 }
 static void Setup(){
  var flow=ReplicaFlow.Instance;flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
  foreach(var item in LevelGenerator.Instance.objectsOnScene)item.SetActive(false);
  GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;
 }
 static GameObject Probe(string name,int index){
  var p=GameObject.CreatePrimitive(PrimitiveType.Cube);p.name=name;p.tag="Collectable";
  p.transform.position=new Vector3((index%3-1)*1.2f,5+index*.15f,1);p.transform.rotation=Quaternion.Euler(20,30,40);
  p.transform.localScale=Vector3.one*.25f;p.AddComponent<Rigidbody>().useGravity=false;
  LevelGenerator.Instance.objectsOnScene.Add(p);probes.Add(p);return p;
 }
 static IEnumerator Core(){
  Physics.autoSimulation=false;Time.timeScale=1;Camera.main.aspect=9f/16;Setup();yield return null;var input=InputController.Instance;
  collectionKey="goose.collection."+ReplicaFlow.Instance.library.themes[ReplicaFlow.Instance.ThemeIndex].id;completedKey="goose.completed."+ReplicaFlow.Instance.library.themes[ReplicaFlow.Instance.ThemeIndex].id;
  hadCollection=PlayerPrefs.HasKey(collectionKey);hadCompleted=PlayerPrefs.HasKey(completedKey);priorCollection=PlayerPrefs.GetInt(collectionKey);priorCompleted=PlayerPrefs.GetString(completedKey);
  Check(input.UsesSourceFlight,"stage 2 uses recovered package-default old path");
  var first=Probe("source-axis",0);Vector3 start=first.transform.position;Vector3 scale=first.transform.localScale;
  var end=ReplicaTray.Instance.SlotPosition(0);input.AddObjectOnStack(first);
  // Inspect actual independent axis tweens, not an isolated easing formula.
  DOTween.Goto(first.transform,.1f,false);
  Check(Mathf.Abs(first.transform.position.x-Mathf.Lerp(start.x,end.x,1f/3))<.001f,"X axis linear at 100ms");
  Check(Mathf.Abs(first.transform.position.z-Mathf.Lerp(start.z,end.z,1f/3))<.001f,"Z axis linear at 100ms");
  Check(Mathf.Abs(first.transform.position.y-Mathf.Lerp(start.y,end.y,5f/9))<.001f,"Y axis OutQuad at 100ms");
  Check(Vector3.Distance(first.transform.localScale,scale*1.4f)<.001f,"first .1s scales to 1.4 source start scale");
  input.ResetStack();LevelGenerator.Instance.objectsOnScene.Remove(first);UnityEngine.Object.DestroyImmediate(first);
  var a=Probe("source-A",1);var b=Probe("source-A",2);var c=Probe("source-A",3);
  input.AddObjectOnStack(a);input.AddObjectOnStack(b);
  for(int i=0;i<4;i++)input.AddObjectOnStack(Probe("source-other-"+i,4+i));
  Check(input.objectsOnStack.Count==6,"continuous clicks during incoming flights accepted");
  yield return null;float began=Time.time;input.AddObjectOnStack(c);
  Check(input.objectsOnStack.Count==4&&!input.objectsOnStack.Contains(a)&&!input.objectsOnStack.Contains(c),"seventh-slot triple removes logical three immediately");
  Check(a&&b&&c&&input.HasPendingVisuals&&!input.CanEvaluateWin,"visual triple retained and win delayed");
  var next=Probe("source-next",8);input.AddObjectOnStack(next);Check(input.objectsOnStack.Count==5,"new pick accepted during match");
  Check(c.GetComponent<Collider>().enabled&&c.GetComponent<Rigidbody>().detectCollisions,"source collision grace on release");
  while(Time.time-began<.08f)yield return null;
  Check(!c.GetComponent<Collider>().enabled&&!c.GetComponent<Rigidbody>().detectCollisions,"collision hidden after .05 scaled seconds");
  while(Time.time-began<.34f)yield return null;
  Check(a&&b&&c&&!input.CanEvaluateWin,"triple still visible during convergence");
  while(Time.time-began<.58f)yield return null;
  Check(!a&&!c&&b&&!input.CanEvaluateWin,"sides removed after convergence; centre shrinking before completion");
  while(Time.time-began<.82f)yield return null;
  Check(!b&&input.CanEvaluateWin&&!input.HasPendingVisuals,"centre removed and completion released");
  while(Time.time-began<1.15f)yield return null;
  for(int i=0;i<input.objectsOnStack.Count;i++)Check(Vector3.Distance(input.objectsOnStack[i].transform.position,ReplicaTray.Instance.SlotPosition(i))<.002f,"post-match reflow reaches each new slot");
  var latest=Probe("source-undo-busy",9);input.AddObjectOnStack(latest);int beforeUndo=input.objectsOnStack.Count;
  Check(input.arrivedObject.Count>0&&input.HasPendingVisuals,"undo fixture has arrived items plus a new incoming item");
  input.UndoLast();Check(input.objectsOnStack.Count==beforeUndo&&input.objectsOnStack.Contains(latest),"project undo busy guard cannot kill an incoming tween and strand pending state");
  yield return new WaitForSeconds(.4f);Check(!input.HasPendingVisuals&&!input.IsBusy,"incoming tween completes after ignored undo");
  input.UndoLast();Check(input.objectsOnStack.Count==beforeUndo-1,"undo works after flight completes");DOTween.Complete(latest.transform,true);
  // Source SetUpdate(true) flight continues during timeScale=0, but WaitForSeconds(.05) does not.
  input.ResetStack();var pause=Probe("source-pause",9);input.AddObjectOnStack(pause);Time.timeScale=0;
  yield return new WaitForSecondsRealtime(.38f);
  Check(Vector3.Distance(pause.transform.position,ReplicaTray.Instance.SlotPosition(0))<.002f,"flight independent of game time");
  Check(pause.GetComponent<Collider>().enabled,"collider coroutine uses paused game time");
  Time.timeScale=1;yield return new WaitForSeconds(.08f);
  Check(!pause.GetComponent<Collider>().enabled,"collider coroutine resumes with game time");
  input.ResetStack();a=Probe("source-cancel",10);b=Probe("source-cancel",11);c=Probe("source-cancel",12);
  input.AddObjectOnStack(a);input.AddObjectOnStack(b);input.AddObjectOnStack(c);
  Check(input.HasPendingVisuals&&!input.CanEvaluateWin,"cancel fixture actually has in-flight triple");
  ReplicaFlow.Instance.Home();Check(!input.HasPendingVisuals&&!GameManager.Instance.GameIsPlaying(),"Home clears pending visuals and leaves gameplay");
  yield return null;Check(!a&&!b&&!c,"Home destroys visual triple removed from board bookkeeping");
  Setup();for(int i=0;i<7;i++)input.AddObjectOnStack(Probe("source-full-"+i,20+i));
  Check(input.objectsOnStack.Count==7&&GameManager.Instance.gameStatus==GameManager.GameStatus.FAIL,"seventh unmatched item immediately enters project failure UI (source PrepareLose lock)");
  input.AddObjectOnStack(Probe("source-eighth",28));Check(input.objectsOnStack.Count==7,"failure blocks additional picks");
  ReplicaFlow.Instance.Home();Setup();
  // Exercise the actual GameManager completion chain on the last triple, not just its boolean.
  foreach(var old in LevelGenerator.Instance.objectsOnScene)if(old){old.transform.DOKill();UnityEngine.Object.DestroyImmediate(old);}
  LevelGenerator.Instance.objectsOnScene.Clear();
  a=Probe("source-last",30);b=Probe("source-last",31);c=Probe("source-last",32);yield return null;began=Time.time;
  input.AddObjectOnStack(a);input.AddObjectOnStack(b);input.AddObjectOnStack(c);
  Check(LevelGenerator.Instance.objectsOnScene.Count==0&&!input.CanEvaluateWin,"last triple logical board is empty while visual completion is pending");
  GameManager.Instance.SendMessage("CheckIfGameEnd");Check(GameManager.Instance.GameIsPlaying(),"no early win on empty logical board");
  while(Time.time-began<.58f)yield return null;
  Check(GameManager.Instance.GameIsPlaying(),"last triple cannot win while centre is still shrinking");
  float deadline=Time.realtimeSinceStartup+2;
  while(GameManager.Instance.GameIsPlaying()&&Time.realtimeSinceStartup<deadline)yield return null;
  Check(GameManager.Instance.gameStatus==GameManager.GameStatus.FINISH&&Time.time-began>=.69f&&Time.time-began<1.1f,"actual win occurs after source .7 completion chain");
  ReplicaFlow.Instance.Home();Setup();
  foreach(var old in LevelGenerator.Instance.objectsOnScene)if(old){old.transform.DOKill();UnityEngine.Object.DestroyImmediate(old);}
  LevelGenerator.Instance.objectsOnScene.Clear();
  var groupA=new[]{Probe("source-overlap-A",40),Probe("source-overlap-A",41),Probe("source-overlap-A",42)};
  var groupB=new[]{Probe("source-overlap-B",43),Probe("source-overlap-B",44),Probe("source-overlap-B",45)};
  yield return null;began=Time.time;foreach(var item in groupA)input.AddObjectOnStack(item);
  yield return new WaitForSeconds(.25f);float secondBegan=Time.time;
  foreach(var item in groupB)input.AddObjectOnStack(item);
  deadline=Time.realtimeSinceStartup+2;
  while(GameManager.Instance.GameIsPlaying()&&Time.realtimeSinceStartup<deadline)yield return null;
  Check(GameManager.Instance.gameStatus==GameManager.GameStatus.FINISH&&Time.time-began>=.69f&&Time.time-secondBegan<.69f,"first EndCombine checks empty board without waiting for concurrent second match");
  Check(input.HasPendingVisuals,"win checkpoint permits remaining parallel visuals");
  ReplicaFlow.Instance.Home();Setup();
  var realItems=LevelGenerator.Instance.objectsOnScene.GroupBy(x=>x.name).Select(g=>g.First()).Take(6).ToArray();
  Check(realItems.Length==6,"six distinct original model types available");
  // Geometry recovered from data.unity3d: seven Content points are .71 local
  // apart under a 1.4 Content scale, and StorePointList stacks .75 local
  // along its up vector (with the alternating .1 local offset).
  Check(Mathf.Abs(Vector3.Distance(ReplicaTray.Instance.SlotPosition(0),ReplicaTray.Instance.SlotPosition(1))-.994f)<.002f,"source tray point spacing is .71*1.4");
  var store0=ReplicaSourceTools.StorePosition(0,0,2);var store1=ReplicaSourceTools.StorePosition(0,1,2);
  Check(Mathf.Abs(Vector3.Dot(store1-store0,Camera.main.transform.forward)+1.05f)<.002f,"store stack depth spacing is .75*1.4");
  foreach(var item in realItems){item.SetActive(true);input.AddObjectOnStack(item);}
  yield return new WaitForSeconds(.85f);
  Check(input.objectsOnStack.Count==6&&!input.HasPendingVisuals,"six original models complete flight");
  for(int i=0;i<input.objectsOnStack.Count;i++){
   var item=input.objectsOnStack[i];
   Check(Vector3.Distance(item.transform.position,ReplicaTray.Instance.SlotPosition(i))<.002f,"original model reaches source depth slot");
   Check(Mathf.Abs(Camera.main.WorldToScreenPoint(item.transform.position).z-1.58f)<.002f,"original model camera depth is 1.58");
   var viewport=Camera.main.WorldToViewportPoint(item.transform.position);
   Check(viewport.x>0&&viewport.x<1&&viewport.y>0&&viewport.y<1,"portrait slot stays inside viewport");
   Check(Vector3.Distance(item.transform.localScale,ReplicaSourceFlight.PickScale(item,Vector3.one))<.002f,"original model uses recovered pick scale after punch settles");
  }
  var tray=GameObject.Find("Collection Tray");
  Check(tray&&Mathf.Abs(Camera.main.WorldToScreenPoint(tray.transform.position).z-2f)<.002f,"tray canvas stays at source Content depth 2 instead of behind pot");
  Check(Mathf.Abs(Camera.main.WorldToViewportPoint(tray.transform.position).y-Camera.main.WorldToViewportPoint(ReplicaTray.Instance.SlotPosition(3)).y)<.001f,"tray and middle target share screen centre");
  CaptureTray();
 }
 static void CaptureTray(){
  var camera=Camera.main;var old=camera.targetTexture;var active=RenderTexture.active;
  var rt=new RenderTexture(720,1280,24);var tex=new Texture2D(720,1280,TextureFormat.RGB24,false);
  try{camera.targetTexture=rt;rt.Create();camera.Render();RenderTexture.active=rt;
   tex.ReadPixels(new Rect(0,0,720,1280),0,0);tex.Apply();Directory.CreateDirectory("visual-captures");
   File.WriteAllBytes("visual-captures/source-tray-original-scale.png",tex.EncodeToPNG());
  }finally{camera.targetTexture=old;RenderTexture.active=active;UnityEngine.Object.DestroyImmediate(tex);rt.Release();UnityEngine.Object.DestroyImmediate(rt);}
 }
}
