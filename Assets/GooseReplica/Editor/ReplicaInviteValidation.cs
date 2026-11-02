using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
public static class ReplicaInviteValidation {
 public static void Run(){ReplicaInvitations.Instance.StartCoroutine(Check());}
 static IEnumerator Check(){
  var service=ReplicaInvitations.Instance;service.Initialize("http://127.0.0.1:8098");string user="test-"+Guid.NewGuid().ToString("N");
  service.OnInvitationBridge("{\"code\":\""+user+"\"}");while(service.Busy)yield return null;
  Assert(service.Ready,"login");
  using(var req=UnityWebRequest.Put("http://127.0.0.1:8098/invites/login",System.Text.Encoding.UTF8.GetBytes("{\"code\":\"friend-"+user+"\",\"inviter\":\""+service.State.inviteCode+"\"}"))){req.method="POST";req.SetRequestHeader("Content-Type","application/json");yield return req.SendWebRequest();Assert(req.result==UnityWebRequest.Result.Success,"friend login");}
  service.OnInvitationBridge("{\"code\":\""+user+"\"}");while(service.Busy)yield return null;
  Assert(service.State.invited==1&&service.Inventory(0)==1,"reward");
  ReplicaSourceTools.Reset();Assert(ReplicaSourceTools.TrySpend(0),"free use");Assert(!ReplicaSourceTools.TrySpend(0),"cannot bypass server debit");
  int calls=0;yield return service.Use(0,()=>ReplicaSourceTools.RunPaid(0,()=>{if(ReplicaSourceTools.TrySpend(0))calls++;}));
  Assert(calls==1&&service.Inventory(0)==0&&!service.HasPending,"paid once");
  yield return service.Use(1,()=>false);Assert(service.Inventory(1)==1&&!service.HasPending,"invalid effect refunded");
  // Simulate shutdown with an unresolved debit, then authenticate again and recover.
  string pending="{\"type\":2,\"requestId\":\""+Guid.NewGuid().ToString("N")+"\"}";
  UnityEngine.PlayerPrefs.SetString("goose.invites.pending."+service.State.inviteCode,pending);UnityEngine.PlayerPrefs.Save();
  service.OnInvitationBridge("{\"code\":\""+user+"\"}");while(service.Busy)yield return null;
  Assert(service.Inventory(2)==1&&!service.HasPending,"unfinished request canceled");
  var popup=UnityEngine.Object.FindObjectOfType<ReplicaInviteView>();if(popup)popup.GetComponent<ReplicaUIView>().Get<UnityEngine.UI.Button>("Close").onClick.Invoke();
  Assert(!ReplicaFlow.Instance.BlocksGameplay,"close unblocks");
  File.WriteAllText("Temp/invitation-tests/unity-result.txt","PASS login; new-user reward; free use; bypass denied; paid once; invalid effect refund; pending recovery; close modal");
 }
 static void Assert(bool pass,string name){if(!pass)throw new Exception("Invitation validation failed: "+name);}
}
