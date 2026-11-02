using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
public static class ReplicaRecordsValidation {
 const string Output="work/game-records/unity-result.txt";
 static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static object Get(object o,string name)=>o.GetType().GetField(name,Flags).GetValue(o);
 static void Set(object o,string name,object value)=>o.GetType().GetField(name,Flags).SetValue(o,value);
 static void Assert(bool value,string name){if(!value)throw new Exception(name);File.AppendAllText(Output,"PASS "+name+"\n");}
 [MenuItem("Goose Replica/Validation/Game Records")]
 public static void Run(){if(!Application.isPlaying||!ReplicaFlow.Instance)throw new Exception("Enter Play Mode first");Directory.CreateDirectory("work/game-records");File.WriteAllText(Output,"");new GameObject("Records validation driver").AddComponent<SourceValidationDriver>().StartCoroutine(Guard(Check()));}
 static IEnumerator Guard(IEnumerator check){while(true){object current;try{if(!check.MoveNext())break;current=check.Current;}catch(Exception e){File.AppendAllText(Output,"FAIL "+e);Debug.LogException(e);yield break;}yield return current;}File.AppendAllText(Output,"ALL PASSED\n");}
 static IEnumerator Check(){
  var flow=ReplicaFlow.Instance;var records=ReplicaGameRecords.Instance;var account=ReplicaInvitations.Instance;
  Assert(records!=null,"record component initialized");
  var saved=PlayerPrefs.GetString("goose.records.queue.v1","");
  try{
   Set(records,"queue",new ReplicaGameRecords.Queue());Set(records,"endpoint","http://127.0.0.1:1");
   flow.Begin(0);float until=Time.realtimeSinceStartup+30;while(!GameManager.Instance.GameIsPlaying()&&Time.realtimeSinceStartup<until)yield return null;
   Assert(GameManager.Instance.GameIsPlaying(),"first stage reached playable state");
   yield return new WaitForSecondsRealtime(.2f);
   var first=(ReplicaGameRecords.Record)Get(records,"active");Assert(first!=null&&first.startedAt>0,"start captured before first input");
   double before=(double)Get(records,"activeSeconds");Time.timeScale=0;yield return new WaitForSecondsRealtime(.2f);Assert(Math.Abs((double)Get(records,"activeSeconds")-before)<.01,"pause excluded from active duration");Time.timeScale=1;
   Set(records,"hidden",true);before=(double)Get(records,"activeSeconds");yield return new WaitForSecondsRealtime(.2f);Assert(Math.Abs((double)Get(records,"activeSeconds")-before)<.01,"background excluded from duration");Set(records,"hidden",false);
   for(int i=0;i<first.initialCount/3;i++)records.Matched();flow.HandleResult(true);
   var second=(ReplicaGameRecords.Record)Get(records,"active");Assert(second!=null&&second.stage==2&&second.runId==first.runId&&second.recordId!=first.recordId,"automatic transition records first stage and links second");
   until=Time.realtimeSinceStartup+30;while(!GameManager.Instance.GameIsPlaying()&&Time.realtimeSinceStartup<until)yield return null;
   records.Matched();flow.RestartStage();
   var retry=(ReplicaGameRecords.Record)Get(records,"active");Assert(retry.recordId!=second.recordId&&retry.runId==second.runId,"restart creates a distinct record in same run");
   until=Time.realtimeSinceStartup+30;while(!GameManager.Instance.GameIsPlaying()&&Time.realtimeSinceStartup<until)yield return null;
   flow.Home();
   var unbound=(System.Collections.Generic.List<ReplicaGameRecords.Pending>)Get(records,"unbound");Assert(unbound.Count==6,"offline start and terminal captured for three attempts");
   var ended=JsonUtility.FromJson<ReplicaGameRecords.Record>(unbound[1].json);Assert(ended.status=="win"&&ended.clearedCount==ended.initialCount,"completion progress captured before board cleanup");
   Assert(JsonUtility.FromJson<ReplicaGameRecords.Record>(unbound[3].json).endReason=="restart","restart reason preserved");Assert(JsonUtility.FromJson<ReplicaGameRecords.Record>(unbound[5].json).endReason=="home","home exit captured");
   account.Initialize("http://127.0.0.1:8099");account.OnInvitationBridge("{\"code\":\"records-editor-test\"}");until=Time.realtimeSinceStartup+15;while(!account.Ready&&Time.realtimeSinceStartup<until)yield return null;
   Assert(account.Ready,"local fixture login");yield return null;
   var queue=(ReplicaGameRecords.Queue)Get(records,"queue");Assert(queue.items.Count==6&&queue.items.TrueForAll(x=>x.owner==account.State.inviteCode),"unbound in-process events attach to authenticated account");
   var persisted=JsonUtility.FromJson<ReplicaGameRecords.Queue>(PlayerPrefs.GetString("goose.records.queue.v1"));Assert(persisted.items.Count==6,"pending records survive serialization for restart");
   // A different account's pending item must not upload with this account's token.
   queue.items.Add(new ReplicaGameRecords.Pending{owner=new string('f',64),action="start",json=queue.items[0].json});
   Set(records,"endpoint","http://127.0.0.1:8099");until=Time.realtimeSinceStartup+30;while(queue.items.Count>1&&Time.realtimeSinceStartup<until)yield return null;
   Assert(queue.items.Count==1&&queue.items[0].owner==new string('f',64),"network recovery drains own events and preserves other account queue");
  }finally{flow.Home();records.StopAllCoroutines();PlayerPrefs.SetString("goose.records.queue.v1",saved);PlayerPrefs.Save();}
 }
}
