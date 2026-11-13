using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad] public static class ABProbe {
 static ABProbe(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("ABProbe.Pending",false)){SessionState.SetBool("ABProbe.Pending",false);new GameObject("Probe").AddComponent<ABRunner>();}};}
 public static void Run(){SessionState.SetBool("ABProbe.Pending",true);EditorApplication.EnterPlaymode();}
}
public class ABRunner:MonoBehaviour {
 const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 string Result=>Path.GetFullPath("../result.txt");
 Match3DABExperiments ab;Match3DInvitations account;
 void Start(){File.WriteAllText(Result,"");StartCoroutine(Guard(Check()));}
 IEnumerator Guard(IEnumerator check){while(true){object next;try{if(!check.MoveNext())break;next=check.Current;}catch(Exception e){File.AppendAllText(Result,"FAIL "+e);EditorApplication.Exit(1);yield break;}yield return next;}File.AppendAllText(Result,"ALL PASSED\n");EditorApplication.Exit(0);}
 void Assert(bool value,string name){if(!value)throw new Exception(name);File.AppendAllText(Result,"PASS "+name+"\n");}
 IEnumerator Wait(Func<bool> condition){float deadline=Time.realtimeSinceStartup+25;while(!condition()&&Time.realtimeSinceStartup<deadline)yield return null;}
 IEnumerator Check(){
  Assert(Match3DABExperiments.GetABGroup("demo","A")=="A","missing service falls back");
  account=new GameObject("Account").AddComponent<Match3DInvitations>();Match3DInvitations.Instance=account;account.Ready=false;
  ab=new GameObject("AB").AddComponent<Match3DABExperiments>();ab.Initialize(File.ReadAllText(Path.GetFullPath("../endpoint.txt")));
  yield return null;Assert(!ab.Ready&&Match3DABExperiments.GetABGroup("demo")==null,"no login has no group");
  account.Ready=true;yield return Wait(()=>ab.Ready);
  Assert(ab.Ready&&Match3DABExperiments.GetABGroup("DEMO")=="B","authenticated assignments load and keys are case insensitive");
  Assert(Match3DABExperiments.GetABGroup("missing","A")=="A","unassigned test keeps default");
  account.State.inviteCode="owner-b";account.RecordToken="token-b";
  Assert(Match3DABExperiments.GetABGroup("demo")==null,"account switch immediately hides previous group");
  yield return Wait(()=>ab.Ready);Assert(ab.Ready&&Match3DABExperiments.GetABGroup("demo")=="C","second account gets independent assignment");
  account.RecordToken="empty";ab.Refresh();yield return Wait(()=>Match3DABExperiments.GetABGroup("demo")==null);
  Assert(ab.Ready&&Match3DABExperiments.GetABGroup("demo")==null,"paused or removed test clears old cache");
  account.RecordToken="token-b";ab.Refresh();yield return Wait(()=>Match3DABExperiments.GetABGroup("demo")=="C");
  account.RecordToken="failure";typeof(Match3DABExperiments).GetField("expires",Flags).SetValue(ab,Time.realtimeSinceStartup-.1f);ab.Refresh();yield return null;
  Assert(!ab.Ready&&Match3DABExperiments.GetABGroup("demo","A")=="A","expired cache falls back even during retry");
  yield return Wait(()=>!(bool)typeof(Match3DABExperiments).GetField("busy",Flags).GetValue(ab));Assert(!ab.Ready,"service failure never invents a group");
  account.RecordToken="slow";ab.Refresh();yield return Wait(()=>File.Exists(Path.GetFullPath("../slow-started")));
  account.State.inviteCode="owner-c";account.RecordToken="token-c";
  yield return Wait(()=>ab.Ready);Assert(ab.Ready&&Match3DABExperiments.GetABGroup("demo")=="D","late previous-account response is discarded");
  account.RecordToken="unauthorized";ab.Refresh();yield return Wait(()=>account.Refreshes>0);
  Assert(account.Refreshes==1&&Match3DABExperiments.GetABGroup("demo")==null,"401 refreshes login and hides cached groups");
 }
}
