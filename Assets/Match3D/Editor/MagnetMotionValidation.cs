using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public static class MagnetMotionValidation {
 static MagnetMotionValidation(){EditorApplication.update+=Watch;}
 static void Watch(){
  if(!File.Exists("Temp/run-magnet-validation")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  if(!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetBool("magnet-test-play",true);EditorApplication.isPlaying=true;return;}
  if(!Application.isPlaying||!Match3DFlow.Instance||!Camera.main)return;
  File.Delete("Temp/run-magnet-validation");
  new GameObject("Magnet validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
 }
 static IEnumerator Guard(){var steps=Check();string failure=null;while(true){object current=null;bool next=false;try{next=steps.MoveNext();if(next)current=steps.Current;}catch(Exception e){failure=e.ToString();}if(!next||failure!=null)break;yield return current;}
 Match3DFlow.Instance.Home();File.WriteAllText("Temp/magnet-validation.txt",failure==null?"PASS magnetic pickup: no beam/highlight; anticipatory lift; full triple removal; home cancels pending animation.":"FAIL "+failure);
 if(SessionState.GetBool("magnet-test-play",false)){SessionState.SetBool("magnet-test-play",false);EditorApplication.isPlaying=false;}}
 static void Assert(bool ok,string message){if(!ok)throw new Exception(message);}
 static void Setup(){Match3DFlow.Instance.Begin(0);Match3DFlow.Instance.NextStage();BoardCreationTestSupport.FinishPending(Match3DFlow.Instance);GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;}
 static IEnumerator Check(){
  Setup();yield return null;
  var input=InputController.Instance;var board=LevelGenerator.Instance.objectsOnScene;
  var triple=board.Where(x=>x&&x.activeInHierarchy).GroupBy(x=>x.name).First(g=>g.Count()>=3).Take(3).ToArray();
  input.AddObjectOnStack(triple[0]);yield return new WaitForSecondsRealtime(.4f);
  var targets=BoardTools.AutoFindCandidates(input.objectsOnStack,board);var start=targets.Select(x=>x.transform.position).ToArray();
  BoosterManager.Instance.HintButton();yield return new WaitForSecondsRealtime(.08f);
  Assert(BoardTools.UsedCount(1)==1,"tool not consumed");
  Assert(BoardToolVisual.ActiveEffectCount==0&&ItemHighlight.For(Camera.main).ToolTargetCount==0,"old effects present");
  Assert(targets.Any(x=>x&&x.activeInHierarchy),"targets removed before arrival");
  Assert(targets.Select((x,i)=>x&&Vector3.Distance(start[i],x.transform.position)>.001f).Any(x=>x),"no anticipation motion");
  yield return new WaitForSecondsRealtime(1.2f);
  Assert(input.objectsOnStack.Count==0&&!input.IsBusy,"triple failed to settle and clear");
  Setup();yield return null;
  var first=board.First(x=>x&&x.activeInHierarchy);input.AddObjectOnStack(first);yield return new WaitForSecondsRealtime(.4f);BoosterManager.Instance.HintButton();
  yield return new WaitForSecondsRealtime(.06f);Match3DFlow.Instance.Home();yield return new WaitForSecondsRealtime(.7f);
  Assert(!input.IsBusy&&board.Count==0&&BoardToolVisual.ActiveEffectCount==0,"home left pending objects");
 }
}
