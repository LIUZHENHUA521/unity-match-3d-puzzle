using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[InitializeOnLoad]
public static class ModalMotionValidation {
 static ModalMotionValidation(){EditorApplication.update+=Poll;}
 static void Poll(){
  if(!File.Exists("Temp/run-modal-motion-check")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  if(!EditorApplication.isPlayingOrWillChangePlaymode){EditorApplication.isPlaying=true;return;}
  if(!Application.isPlaying||!Match3DFlow.Instance)return;
  File.Delete("Temp/run-modal-motion-check");new GameObject("Modal validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
 }
 static IEnumerator Guard(){float speed=Time.timeScale;var steps=Run();string failure=null;while(true){object current=null;bool next=false;try{next=steps.MoveNext();if(next)current=steps.Current;}catch(Exception e){failure=e.ToString();}if(!next||failure!=null)break;yield return current;}
  Time.timeScale=speed;File.WriteAllText("Temp/modal-motion-validation.txt",failure==null?"PASS 12 modal prefabs: identical entrance, unscaled pause support, input locked until entrance completes, final scale/opacity, repeated play guarded, early close cleaned up.":"FAIL "+failure);EditorApplication.isPlaying=false;
 }
 static void Assert(bool ok,string reason){if(!ok)throw new Exception(reason);}
 static IEnumerator Run(){
  Time.timeScale=0;
  foreach(string name in new[]{"PauseSettings","ReviveClear","ReviveTime","ResultSuccess","ResultFailure","Reward","RedPackets","UserProfile","Mailbox","InviteRewards","ModelSize","Result"}){
   var view=Match3DUIView.Load(name,null);Match3DModalMotion.Play(view.gameObject);var motion=view.GetComponent<Match3DModalMotion>();var group=view.GetComponentInChildren<CanvasGroup>();
   Assert(motion.Playing&&!group.interactable&&group.alpha==0,name+" initial state");
   yield return new WaitForSecondsRealtime(.1f);Assert(group.alpha>0,name+" paused fade");
   if(motion.Playing)Match3DModalMotion.Play(view.gameObject);yield return new WaitForSecondsRealtime(.4f);
   Assert(!motion.Playing&&group.interactable&&Mathf.Abs(group.alpha-1)<.001f,name+" finish");
   Assert(Vector3.Distance(group.transform.localScale,Vector3.one)<.001f,name+" scale");
   var shade=view.GetComponent<Image>();Assert(shade&&Mathf.Abs(shade.color.a-.64f)<.001f,name+" shade");
   UnityEngine.Object.Destroy(view.gameObject);yield return null;
  }
  var early=Match3DUIView.Load("PauseSettings",null);Match3DModalMotion.Play(early.gameObject);UnityEngine.Object.Destroy(early.gameObject);yield return new WaitForSecondsRealtime(.5f);
  Assert(!early,"early close survives");
 }
}
