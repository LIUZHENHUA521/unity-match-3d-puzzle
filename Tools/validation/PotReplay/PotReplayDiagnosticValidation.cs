using System;
using UnityEditor;
using UnityEngine;
public static class PotReplayDiagnosticValidation {
 static void Check(bool b,string m){if(!b)throw new Exception(m);}
 public static void Run(){const string owner="pot-diagnostic-fixture",id="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";try{
  PlayerPrefs.DeleteKey("pot.replay.state.v1."+owner);
  PotReplayLocalState.Put(owner,new PotReplayLocalState.Entry{recordId=id,state="failed",updatedAt=1});
  for(int n=0;n<8;n++){
   var e=PotReplayLocalState.Get(owner,id);e.diagnosticNextAttempt=0;PotReplayLocalState.Put(owner,e);
   Check(PotReplayLocalState.NextDiagnostic(owner)!=null,"eligible attempt");PotReplayLocalState.BeginDiagnostic(owner,e);
   e=PotReplayLocalState.Get(owner,id);Check(e.diagnosticAttempts==n+1&&e.diagnosticNextAttempt-PotReplayLocalState.Now<=3600000,"persisted bounded backoff");
   Check(PotReplayLocalState.NextDiagnostic(owner)==null,"no immediate retry after restart/read");
   PotReplayLocalState.CompleteDiagnostic(owner,id,e.updatedAt,false,409,null);
   PotReplayLocalState.Phase(owner,id,"retry",409);Check(PotReplayLocalState.NextDiagnostic(owner)==null,"upload phase does not bypass backoff");
  }
  var stopped=PotReplayLocalState.Get(owner,id);stopped.diagnosticNextAttempt=0;PotReplayLocalState.Put(owner,stopped);
  Check(PotReplayLocalState.NextDiagnostic(owner)==null,"eight failed sends suspend instead of looping forever");
  long old=stopped.updatedAt;PotReplayLocalState.SettlementSynced(owner,id);Check(PotReplayLocalState.NextDiagnostic(owner)!=null,"settlement acknowledgement wakes pending report");
  PotReplayLocalState.CompleteDiagnostic(owner,id,old,true,200,null);Check(!PotReplayLocalState.Get(owner,id).reported,"late acknowledgement cannot consume newer revision");
  var current=PotReplayLocalState.Get(owner,id);PotReplayLocalState.CompleteDiagnostic(owner,id,current.updatedAt,false,200,"record_not_daily");Check(PotReplayLocalState.NextDiagnostic(owner)==null,"permanent business failure stops");
  PotReplayLocalState.SettlementSynced(owner,id);current=PotReplayLocalState.Get(owner,id);PotReplayLocalState.CompleteDiagnostic(owner,id,current.updatedAt,true,200,null);Check(PotReplayLocalState.Get(owner,id).reported&&PotReplayLocalState.NextDiagnostic(owner)==null,"accepted report completes");
  PotReplayLocalState.Set(owner,id,"synced");Check(PotReplayLocalState.NextDiagnostic(owner)!=null&&PotReplayLocalState.Get(owner,id).diagnosticAttempts==0,"new state after success has fresh failure budget");
  Check(PotReplayLocalState.Get("other-owner",id)==null,"account isolation");
  Debug.Log("POT_DIAGNOSTIC_RETRY_OK");EditorApplication.Exit(0);
 }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}finally{PlayerPrefs.DeleteKey("pot.replay.state.v1."+owner);PlayerPrefs.Save();}}
}
