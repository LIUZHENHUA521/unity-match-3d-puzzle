using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[InitializeOnLoad]
public static class UnlimitedMagnetValidation {
 static UnlimitedMagnetValidation(){EditorApplication.update+=Poll;}
 static void Poll(){
  if(!File.Exists("Temp/run-unlimited-magnet")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  if(!EditorApplication.isPlayingOrWillChangePlaymode){EditorApplication.isPlaying=true;return;}
  if(!Application.isPlaying||!ReplicaFlow.Instance||ReplicaFlow.Instance.ScreenName!="home")return;
  File.Delete("Temp/run-unlimited-magnet");new GameObject("Unlimited Magnet Validation").AddComponent<SourceValidationDriver>().StartCoroutine(Run());
 }
 static IEnumerator Run(){
  var records=ReplicaGameRecords.Instance;records.StopAllCoroutines();records.enabled=false;typeof(ReplicaGameRecords).GetProperty("Instance").SetValue(null,null);
  var it=Checks();string result="PASS zero-inventory repeated magnet; no ads/modal; isolated counters; empty tray rejected; other tools ordinary; reset and ordinary account";
  while(true){bool more=false;object next=null;try{more=it.MoveNext();if(more)next=it.Current;}catch(Exception e){result="FAIL "+e;break;}if(!more)break;yield return next;}
  ReplicaTestPrivileges.ForceForValidation=false;ReplicaFlow.Instance.Home();typeof(ReplicaGameRecords).GetProperty("Instance").SetValue(null,records);
  File.WriteAllText("Temp/unlimited-magnet-validation.txt",result);EditorApplication.isPlaying=false;
 }
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 static IEnumerator Checks(){
  var flow=ReplicaFlow.Instance;ReplicaTestPrivileges.ForceForValidation=true;flow.Begin(0);SourceCreationTestSupport.FinishPending(flow);yield return null;yield return null;
  Check(flow.UnlimitedMagnet,"flag not latched");Check(!ReplicaSourceTools.CanUse(0)&&!ReplicaSourceTools.CanUse(2),"other tools became free");
  var hud=UnityEngine.Object.FindObjectOfType<ReplicaGameplayHUD>();var button=hud.GetComponent<ReplicaUIView>().Get<Button>("Hint");
  Check(hud.quantities[1].text=="∞","missing infinity badge");
  button.onClick.Invoke();Check(ReplicaSourceTools.UsedCount(1)==0,"empty tray spent use");
  for(int i=0;i<4;i++){
   var input=InputController.Instance;
   var obj=LevelGenerator.Instance.objectsOnScene.First(x=>x&&x.activeInHierarchy&&ReplicaLayeredPile.Instance.CanPick(x.GetComponent<Rigidbody>()));
   input.AddObjectOnStack(obj);int before=LevelGenerator.Instance.objectsOnScene.Count;button.onClick.Invoke();
   Check(!flow.BlocksGameplay,"advertisement/modal opened");Check(ReplicaSourceTools.UsedCount(1)==i+1&&ReplicaSourceTools.TestUses(1)==i+1,"magnet did not run");
   Check(ReplicaSourceTools.Remaining(1)==0&&ReplicaSourceTools.AdUses(1)==0,"inventory/ad side effect");
   float deadline=Time.realtimeSinceStartup+8;while(input.IsBusy){Check(Time.realtimeSinceStartup<deadline,"animation stuck");yield return null;}
   Check(LevelGenerator.Instance.objectsOnScene.Count==before-3,"did not match exactly three");Check(input.objectsOnStack.Count==0,"tray not empty");
  }
  flow.NextStage();SourceCreationTestSupport.FinishPending(flow);yield return null;Check(flow.UnlimitedMagnet&&ReplicaSourceTools.TestUses(1)==0,"stage reset/privilege incorrect");
  flow.RestartStage();SourceCreationTestSupport.FinishPending(flow);yield return null;Check(flow.UnlimitedMagnet,"restart lost privilege");
  typeof(ReplicaFlow).GetProperty("IsPvp").SetValue(flow,true);Check(!flow.UnlimitedMagnet&&!ReplicaSourceTools.CanUse(1),"PVP privilege leaked");typeof(ReplicaFlow).GetProperty("IsPvp").SetValue(flow,false);
  flow.Home();ReplicaTestPrivileges.ForceForValidation=false;flow.Begin(0);SourceCreationTestSupport.FinishPending(flow);yield return null;Check(!flow.UnlimitedMagnet&&!ReplicaSourceTools.CanUse(1),"ordinary account became free");
 }
}
