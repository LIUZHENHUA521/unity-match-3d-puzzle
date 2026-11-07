using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public static class MagnetMotionValidation {
 static MagnetMotionValidation(){EditorApplication.update+=Watch;}
 public static void RunBatch(){
  UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Match3D.unity");
  Directory.CreateDirectory("Temp");File.WriteAllText("Temp/run-magnet-validation","");
 }
 static void Watch(){
  if(!File.Exists("Temp/run-magnet-validation")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  if(!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetBool("magnet-test-play",true);EditorApplication.isPlaying=true;return;}
  if(!Application.isPlaying||!Match3DFlow.Instance||Match3DFlow.Instance.ScreenName!="home"||!Camera.main)return;
  File.Delete("Temp/run-magnet-validation");
  new GameObject("Magnet validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
 }
 static IEnumerator Guard(){var steps=Check();string failure=null;while(true){object current=null;bool next=false;try{next=steps.MoveNext();if(next)current=steps.Current;}catch(Exception e){failure=e.ToString();}if(!next||failure!=null)break;yield return current;}
 Match3DFlow.Instance.Home();Directory.CreateDirectory("Logs");File.WriteAllText("Logs/magnet-validation-result.txt",failure==null?"PASS magnetic pickup: target highlight, shake/lift/arc/landing; full triple removal; home cancels pending animation.":"FAIL "+failure);
 Match3DTestPrivileges.ForceForValidation=false;
 if(Application.isBatchMode){EditorApplication.Exit(failure==null?0:1);yield break;}
 if(SessionState.GetBool("magnet-test-play",false)){SessionState.SetBool("magnet-test-play",false);EditorApplication.isPlaying=false;}}
 static void Assert(bool ok,string message){if(!ok)throw new Exception(message);}
 static void Setup(){Match3DTestPrivileges.ForceForValidation=true;Match3DFlow.Instance.Begin(0);Match3DFlow.Instance.NextStage();BoardCreationTestSupport.FinishPending(Match3DFlow.Instance);GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;}
 static IEnumerator Check(){
  var origin=Vector3.zero;var slot=new Vector3(2,-3,1);
  Assert(ItemFlightMotion.MagnetPosition(origin,slot,Vector3.up,Vector3.right,1,-.1f)==origin,"delay moved target");
  Assert(Vector3.Distance(ItemFlightMotion.MagnetPosition(origin,slot,Vector3.up,Vector3.right,1,ItemFlightMotion.MagnetSeconds),slot)<.0001f,"landing misses slot");
  Assert(ItemFlightMotion.MagnetPosition(origin,slot,Vector3.up,Vector3.right,1,.23f).y>0,"no lift before pull");
  Assert(Vector3.Distance(ItemFlightMotion.MagnetScale(Vector3.one,Vector3.one*.5f,ItemFlightMotion.MagnetSeconds),Vector3.one*.5f)<.0001f,"landing scale incorrect");
  Setup();yield return null;
  var input=InputController.Instance;var board=LevelGenerator.Instance.objectsOnScene;
  var triple=board.Where(x=>x&&x.activeInHierarchy).GroupBy(x=>x.name).First(g=>g.Count()>=3).Take(3).ToArray();
  input.AddObjectOnStack(triple[0]);yield return new WaitForSecondsRealtime(.4f);
  var targets=BoardTools.AutoFindCandidates(input.objectsOnStack,board);var start=targets.Select(x=>x.transform.position).ToArray();
  BoardTools.RunPaid(1,()=>BoosterManager.Instance.HintButton());yield return new WaitForSecondsRealtime(.08f);
  Assert(BoardTools.UsedCount(1)==1,"tool not consumed");
  Assert(ItemHighlight.For(Camera.main).ToolTargetCount==targets.Count,"target highlight missing");
  Assert(targets.Any(x=>x&&x.activeInHierarchy),"targets removed before arrival");
  Assert(targets.Select((x,i)=>x&&Vector3.Distance(start[i],x.transform.position)>.001f).Any(x=>x),"no anticipation motion");
  yield return new WaitForSecondsRealtime(1.2f);
  Assert(input.objectsOnStack.Count==0&&!input.IsBusy,"triple failed to settle and clear");
  Setup();yield return null;
  var first=board.First(x=>x&&x.activeInHierarchy);input.AddObjectOnStack(first);yield return new WaitForSecondsRealtime(.4f);BoardTools.RunPaid(1,()=>BoosterManager.Instance.HintButton());
  yield return new WaitForSecondsRealtime(.06f);Match3DFlow.Instance.Home();yield return new WaitForSecondsRealtime(.7f);
  Assert(!input.IsBusy&&board.Count==0&&BoardToolVisual.ActiveEffectCount==0,"home left pending objects");
 }
}
