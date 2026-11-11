using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad] public static class RecordSyncProbe {
 const string Key="RecordSyncProbe.Pending";
 static RecordSyncProbe(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false)){SessionState.SetBool(Key,false);new GameObject("Probe").AddComponent<RecordSyncRunner>();}};}
 public static void Run(){SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
}
public sealed class RecordSyncRunner:MonoBehaviour {
 const string QueueKey="goose.records.queue.v1";
 const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 string Result=>Path.GetFullPath("../result.txt");
 string saved,endpoint;bool hadQueue;
 Match3DFlow flow;Match3DInvitations account;Match3DGameRecords records;
 Match3DGameRecords.Queue Queue=>(Match3DGameRecords.Queue)typeof(Match3DGameRecords).GetField("queue",Flags).GetValue(records);
 void Start(){hadQueue=PlayerPrefs.HasKey(QueueKey);saved=PlayerPrefs.GetString(QueueKey,"");endpoint=File.ReadAllText(Path.GetFullPath("../endpoint.txt"));File.WriteAllText(Result,"");StartCoroutine(Guard(Check()));}
 void End(int code){if(records)records.StopAllCoroutines();if(hadQueue)PlayerPrefs.SetString(QueueKey,saved);else PlayerPrefs.DeleteKey(QueueKey);PlayerPrefs.Save();EditorApplication.Exit(code);}
 IEnumerator Guard(IEnumerator check){while(true){object next;try{if(!check.MoveNext())break;next=check.Current;}catch(Exception e){File.AppendAllText(Result,"FAIL "+e);End(1);yield break;}yield return next;}File.AppendAllText(Result,"ALL PASSED\n");End(0);}
 void Assert(bool value,string name){if(!value)throw new Exception(name);File.AppendAllText(Result,"PASS "+name+"\n");}
 IEnumerator Wait(Func<bool> condition){float until=Time.realtimeSinceStartup+45;while(!condition()&&Time.realtimeSinceStartup<until)yield return null;}
 void Reload(){if(records)DestroyImmediate(records.gameObject);records=new GameObject("Records").AddComponent<Match3DGameRecords>();records.Initialize(endpoint);}
 Match3DGameRecords.Pending Add(string action,string owner="owner-a"){
  var item=new Match3DGameRecords.Pending{owner=owner,action=action,json="{\"recordId\":\"fixture-record\"}"};Queue.items.Add(item);return item;
 }
 Match3DGameRecords.Queue Saved()=>JsonUtility.FromJson<Match3DGameRecords.Queue>(PlayerPrefs.GetString(QueueKey));
 IEnumerator Check(){
  flow=new GameObject("Flow").AddComponent<Match3DFlow>();Match3DFlow.Instance=flow;
  account=new GameObject("Account").AddComponent<Match3DInvitations>();Match3DInvitations.Instance=account;
  // Literal legacy schema: no diagnostic or acknowledgement fields.
  PlayerPrefs.SetString(QueueKey,"{\"items\":[{\"owner\":\"owner-a\",\"action\":\"finish\",\"json\":\"legacy-payload\",\"rejected\":true},{\"owner\":\"owner-b\",\"action\":\"finish\",\"json\":\"other-payload\",\"rejected\":true}]}");
  Reload();yield return null;
  Assert(flow.Notices==0&&Queue.items.Count==2&&Queue.items[0].json=="legacy-payload","legacy rejection retained without startup popup");
  Assert(Saved().items.TrueForAll(x=>x.rejectionNotified),"legacy migration persists acknowledgement");
  account.State.inviteCode="owner-c";yield return null;
  Assert(records.SyncError==null&&flow.Notices==0,"other account legacy rejection does not set current error");
  account.State.inviteCode="owner-a";
  var bad=Add("bad");yield return Wait(()=>bad.rejected&&bad.rejectionNotified);
  Assert(bad.rejected&&bad.rejectionNotified&&flow.Notices==1,"fresh HTTP 400 is retained and warned once");
  Assert(bad.rejectionHttpStatus==400&&bad.rejectionReason=="关卡时间或数量无效"&&bad.rejectedAt>0,"structured server reason and rejection time captured");
  Assert(Saved().items.Exists(x=>x.action=="bad"&&x.rejectionNotified&&x.rejectionReason==bad.rejectionReason),"diagnostics and acknowledgement survive serialization");
  Reload();yield return null;
  Assert(flow.Notices==1,"acknowledged rejection does not warn after restart");
  var conflict=Add("conflict");yield return Wait(()=>conflict.rejectionNotified);
  Assert(conflict.rejected&&conflict.rejectionHttpStatus==409&&conflict.rejectionReason=="开局信息冲突"&&flow.Notices==2,"new 409 still warns after an earlier acknowledged failure");
  var second=Add("bad");yield return Wait(()=>second.rejectionNotified);
  Assert(second.rejected&&flow.Notices==2,"multiple fresh rejections warn only once per account per session");
  var foreign=Add("slow","owner-b");account.State.inviteCode="owner-b";
  yield return Wait(()=>File.Exists(Path.GetFullPath("../slow-started")));
  Assert(File.Exists(Path.GetFullPath("../slow-started")),"account B upload reached server");
  account.State.inviteCode="owner-a";
  yield return Wait(()=>foreign.rejected);yield return null;
  Assert(foreign.rejected&&!foreign.rejectionNotified&&flow.Notices==2,"in-flight rejection does not notify switched account");
  account.State.inviteCode="owner-b";yield return null;
  Assert(foreign.rejectionNotified&&flow.Notices==3,"owner receives its pending rejection after returning");
  account.State.inviteCode="owner-c";yield return null;
  Assert(records.SyncError==null,"switch to clean account clears sync error");
  account.State.inviteCode="owner-a";
  var transient=Add("retry");yield return Wait(()=>!Queue.items.Contains(transient));
  Assert(!Queue.items.Contains(transient)&&!transient.rejected&&flow.Notices==3,"HTTP 503 retries and successful receipt drains pending record");
  var unauthorized=Add("auth");yield return Wait(()=>!Queue.items.Contains(unauthorized));
  Assert(account.Refreshes==1&&!unauthorized.rejected&&!Queue.items.Contains(unauthorized),"HTTP 401 refreshes login and retries without rejection");
  var malformed=Add("malformed");yield return Wait(()=>malformed.rejectionNotified);
  Assert(malformed.rejected&&malformed.rejectionReason=="HTTP 400","malformed error response retains HTTP fallback");
  records.StopAllCoroutines();
  while(Queue.items.Count<498)Queue.items.Add(new Match3DGameRecords.Pending{owner="owner-z",rejected=true,rejectionNotified=true});
  records.Prepare("daily",1,"2026-11-11",3,0);records.StartRecord();yield return null;
  Assert(records.SyncError=="queue_full"&&flow.LastNotice.Contains("缓存已满"),"queue capacity still warns and protects existing payloads");
 }
}
