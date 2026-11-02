using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[InitializeOnLoad]
public static class ToolButtonStateValidation {
 static ToolButtonStateValidation(){EditorApplication.update+=Poll;}
 static void Poll(){
  if(!File.Exists("Temp/run-tool-button-check")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  if(!EditorApplication.isPlayingOrWillChangePlaymode){EditorApplication.isPlaying=true;return;}
  if(!Application.isPlaying||!ReplicaFlow.Instance||!UnityEngine.Object.FindObjectOfType<ReplicaHomeView>())return;
  File.Delete("Temp/run-tool-button-check");
  new GameObject("Tool button check").AddComponent<SourceValidationDriver>().StartCoroutine(Run());
 }
 static System.Collections.IEnumerator Run(){
  var flow=ReplicaFlow.Instance;flow.Begin(0);SourceCreationTestSupport.FinishPending(flow);yield return null;yield return null;
  string report;
  try{
   var hud=UnityEngine.Object.FindObjectOfType<ReplicaGameplayHUD>();var view=hud.GetComponent<ReplicaUIView>();
   var move=view.Get<Button>("Undo");var find=view.Get<Button>("Hint");
   if(!move.interactable||!find.interactable)throw new Exception("Unavailable buttons are not clickable");
   if(string.IsNullOrEmpty(flow.ToolUnavailableReason(0))||string.IsNullOrEmpty(flow.ToolUnavailableReason(1)))throw new Exception("Missing empty tray reason");
   move.onClick.Invoke();find.onClick.Invoke();
   if(ReplicaSourceTools.UsedCount(0)!=0||ReplicaSourceTools.UsedCount(1)!=0)throw new Exception("Invalid click consumed tool");
   report="PASS unavailable buttons remain clickable; empty tray reasons; rejected clicks do not spend tools.";
  }catch(Exception e){report="FAIL "+e;}
  var skin=UnityEngine.Object.FindObjectOfType<ReplicaGameplayHUD>();var v=skin.GetComponent<ReplicaUIView>();var before=v.Get<Image>("Undo").sprite;var icon=v.Get<Button>("Undo").transform.Find("Icon").GetComponent<Image>();var color=icon.color;
  ReplicaSourceTools.MarkUsed(0);yield return null;yield return null;
  if(v.Get<Image>("Undo").sprite==before||icon.color!=color)report+=" FAIL used background/icon";else report+=" PASS used background changes without icon tint.";
  flow.Home();File.WriteAllText("Temp/tool-button-validation.txt",report);EditorApplication.isPlaying=false;
 }
}
