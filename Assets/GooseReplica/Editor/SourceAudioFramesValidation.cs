using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SourceAudioFramesValidation {
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static IEnumerator Run(){
  float scale=Time.timeScale;bool physics=Physics.autoSimulation;bool had=PlayerPrefs.HasKey("goose.effects");int sound=PlayerPrefs.GetInt("goose.effects",1);
  string failure=null;var core=Core();
  while(true){bool next=false;try{next=core.MoveNext();}catch(Exception e){failure=e.ToString();}if(!next||failure!=null)break;yield return core.Current;}
  (core as IDisposable)?.Dispose();ReplicaFlow.Instance.Home();Time.timeScale=scale;Physics.autoSimulation=physics;
  if(had)PlayerPrefs.SetInt("goose.effects",sound);else PlayerPrefs.DeleteKey("goose.effects");
  string report=failure==null?"PASS source audio live: real model pickup routes original clip and 1/1.15/1.3 pitches; third match plays original combine at its completion callback, no early combine; mute suppresses playback; Home clears bindings. Editor AudioSource requests, not speaker recording.\n":"FAIL source audio live: "+failure+"\n";
  Directory.CreateDirectory("Temp");File.WriteAllText("Temp/goose-source-audio-frames-validation.txt",report);Debug.Log(report);
 }
 static IEnumerator Core(){
  Time.timeScale=1;Physics.autoSimulation=false;PlayerPrefs.SetInt("goose.effects",1);
  var flow=ReplicaFlow.Instance;flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
  var board=LevelGenerator.Instance.objectsOnScene;foreach(var item in board)item.SetActive(false);
  var chosen=board.GroupBy(x=>x.name).First(g=>g.Count()>=3&&ReplicaSourceAudio.TryBoundProfile(g.First(),out var p)&&p.select>0&&p.usePitch&&!p.goose).Take(3).ToArray();
  ReplicaSourceAudio.TryBoundProfile(chosen[0],out var profile);
  foreach(var item in chosen)item.SetActive(true);
  yield return null;
  var input=InputController.Instance;
  input.AddObjectOnStack(chosen[0]);var audio=flow.GetComponent<ReplicaSourceAudio>();
  Check(audio&&audio.LastSound==profile.select&&audio.LastPitch==1,"first original material clip/pitch");int count=audio.PlayedCount;
  yield return new WaitForSeconds(.08f);input.AddObjectOnStack(chosen[1]);
  Check(audio.PlayedCount==count+1&&audio.LastSound==profile.select&&Mathf.Abs(audio.LastPitch-1.15f)<.0001f,"second actual pickup routes pitch source 2");
  yield return new WaitForSeconds(.08f);input.AddObjectOnStack(chosen[2]);
  Check(audio.PlayedCount==count+2&&audio.LastSound==profile.select&&Mathf.Abs(audio.LastPitch-1.3f)<.0001f,"third actual pickup routes pitch source 3");
  yield return new WaitForSeconds(.4f);Check(audio.PlayedCount==count+2,"combine not played before EndCombine");
  yield return new WaitForSeconds(.45f);Check(audio.PlayedCount==count+3&&audio.LastSound==23&&audio.LastPitch==1,"original combine only at completion");
  PlayerPrefs.SetInt("goose.effects",0);count=audio.PlayedCount;ReplicaSourceAudio.Tool(24);Check(audio.PlayedCount==count,"muted tool does not emit audio");
  var remaining=board.First();flow.Home();Check(!ReplicaSourceAudio.TryBoundProfile(remaining,out _),"Home clears profile references");
 }
}
