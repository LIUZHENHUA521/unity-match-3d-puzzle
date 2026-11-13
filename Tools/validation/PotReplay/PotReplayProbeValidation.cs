using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PotReplayProbeValidation {
 const string Key="pot.replay.probe.validation";static readonly BindingFlags flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static PotReplayProbeValidation(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))EditorApplication.delayCall+=()=>new GameObject("Pot replay validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());};}
 public static void Run(){SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
 static void Check(bool value,string text){if(!value)throw new Exception(text);Debug.Log("POT_PROBE_CHECK "+text);}
 static IEnumerator Guard(){
  var core=Core();string failure=null;while(true){bool more=false;try{more=core.MoveNext();}catch(Exception e){failure=e.ToString();}if(!more||failure!=null)break;yield return core.Current;}
  SessionState.SetBool(Key,false);if(failure!=null)Debug.LogError(failure);else Debug.Log("POT_REPLAY_PROBE_OK generation + preview + concurrent use + tools + seek + pause + speed + exit");EditorApplication.Exit(failure==null?0:1);
 }
 static IEnumerator Core(){
  float deadline=Time.realtimeSinceStartup+90;while(!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete){if(Time.realtimeSinceStartup>deadline)throw new Exception("startup timeout");yield return null;}
  var flow=Match3DFlow.Instance;var level=new Match3DDailyLevel{date="2026-11-13",tier="bachelor",version="probe-runtime",generatorVersion="daily-v1",seed=123,seconds=300,initialCount=90,limitHeight=8,layers=new[]{new ChallengeLayer(90,6,5)}};
  flow.PreviewDailyOpening(level,2);while(flow.CreationInProgress)yield return null;yield return new WaitForSeconds(1);
  foreach(var b in new MonoBehaviour[]{Match3DGameRecords.Instance,PotReplayService.Instance,Match3DInvitations.Instance})if(b){b.StopAllCoroutines();b.enabled=false;}
  var account=Match3DInvitations.Instance;typeof(Match3DInvitations).GetField("<State>k__BackingField",flags).SetValue(account,new Match3DInvitations.Reply{inviteCode=new string('a',64)});typeof(Match3DInvitations).GetField("token",flags).SetValue(account,"fixture-no-network");
  typeof(Match3DFlow).GetField("<LocalPreview>k__BackingField",flags).SetValue(flow,false);
  foreach(var suffix in new[]{".pending",".recording",".pending.tmp"})PotReplayFiles.Delete(PotReplayService.PathFor(new string('a',64),new string('e',32),suffix));
  PotReplayRecorder.Instance.Begin(new Match3DGameRecords.Record{recordId=new string('e',32),runId=new string('f',32),stage=2,initialCount=90},flow.PotDailyLevel);
  var camera=Camera.main;var board=LevelGenerator.Instance.objectsOnScene;
  foreach(var obj in board){LayeredItemPile.Instance.Detach(obj.GetComponent<Rigidbody>());foreach(var body in obj.GetComponentsInChildren<Rigidbody>())body.isKinematic=true;obj.transform.position=camera.ViewportToWorldPoint(new Vector3(.5f,.65f,10));}
  board.Last().transform.position=camera.ViewportToWorldPoint(new Vector3(.5f,.65f,8));board.Last().transform.localScale*=3;
  Physics.SyncTransforms();GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;

  var probe=ComboProbeController.Instance;Check(probe&&probe.Running,"probe active in daily stage");probe.State.Pick(1,ProbePickSource.Manual,true);probe.State.Pick(2,ProbePickSource.Manual,true);
  Check(probe.GrantReward()&&probe.GrantReward(),"actual random generation binds two occluded targets");probe.View.Refresh();board.Last().transform.localScale/=3;
  yield return new WaitForSeconds(.8f);
  probe.View.Preview.Show(probe,probe.State.Top(0),probe.View.Slot(0).transform);yield return new WaitForSeconds(.2f);probe.View.Preview.Clear();
  Check(probe.UseTop(0,probe.State.Top(0)),"first real probe accepted");yield return new WaitForSeconds(.12f);
  Check(probe.UseTop(1,probe.State.Top(1)),"second real probe accepted");yield return new WaitForSeconds(.12f);
  Check(PotProbeActivation.ActiveCount==2,"two concurrent activation effects recorded");
  yield return new WaitForSeconds(1.4f);
  Check(BoardTools.RunPaid(0,()=>InputController.Instance.MoveSourceItemsToStore()),"move tool used");yield return new WaitForSeconds(.6f);
  Check(BoardTools.RunPaid(2,()=>BoardTools.MessUp()),"shuffle used");yield return new WaitForSeconds(2);
  Check(BoardTools.RunPaid(3,()=>BoardTools.AddTime()),"add time used");
  deadline=Time.realtimeSinceStartup+10;while(BoardTools.Busy){if(Time.realtimeSinceStartup>=deadline)throw new Exception("shuffle timeout");yield return null;}
  for(int n=0;n<2;n++){
   var item=LevelGenerator.Instance.objectsOnScene.FirstOrDefault(o=>o&&!BoardTools.IsStored(o)&&o.activeSelf);
   if(item)InputController.Instance.AddObjectOnStack(item);yield return new WaitForSeconds(.6f);
   Debug.Log("POT_MAGNET_DIAG stack="+InputController.Instance.objectsOnStack.Count+" candidates="+BoardTools.AutoFindCandidates(InputController.Instance.objectsOnStack,LevelGenerator.Instance.objectsOnScene).Count+" status="+GameManager.Instance.gameStatus+" resolving="+InputController.Instance.IsResolving+" board="+LevelGenerator.Instance.objectsOnScene.Count);
   Check(BoardTools.RunPaid(1,()=>{BoosterManager.Instance.CheckHintInteractibility();BoosterManager.Instance.HintButton();}),"consecutive magnet "+n+" tray="+InputController.Instance.objectsOnStack.Count+" busy="+BoardTools.Busy+" resolving="+InputController.Instance.IsResolving);yield return new WaitForSeconds(1.5f);
  }
  PotReplayRecorder.Instance.Finish("quit");var path=PotReplayService.PathFor(new string('a',64),new string('e',32),".pending");Check(File.Exists(path),"v2 recorded to queue");var json=File.ReadAllText(path);File.WriteAllText("/tmp/pot-replay-probe-detail.json",json);File.Delete(path);var data=JsonUtility.FromJson<PotReplayData>(json);
  Check(data.frames.All(f=>f.presentation!=null),"all frames carry full presentation");Check(data.events.Count(e=>e.type=="probe_generate")==2&&data.events.Count(e=>e.type=="probe_use")==2,"generation and use events distinguished");
  var reward=data.frames.First(f=>f.presentation.rewards.Length>0);var preview=data.frames.First(f=>f.presentation.previewId>=0);var activation=data.frames.First(f=>f.presentation.activations.Length==2);var stored=data.frames.First(f=>f.presentation.storeVisible);var end=data.frames.Last();
  typeof(Match3DFlow).GetField("<LocalPreview>k__BackingField",flags).SetValue(flow,true);bool learned=probe.HasUsedProbe;var player=flow.gameObject.AddComponent<PotReplayPlayer>();player.Load(data);
  deadline=Time.realtimeSinceStartup+90;while(flow.ScreenName!="pot-replay"){if(Time.realtimeSinceStartup>deadline)throw new Exception("playback timeout");yield return null;}player.Pause();
  foreach(var f in new[]{end,reward,preview,activation,stored,reward,activation,end}){
   player.Seek(f.t);yield return null;yield return null;probe=ComboProbeController.Instance;
   Check(!probe.CanInteract&&probe.ReplayMode,"replay remains read only");Check(probe.State.Combo==f.presentation.combo&&probe.State.Stars==f.presentation.stars,"combo stars seek correctly");
   for(int i=0;i<4;i++)Check(probe.State.Column(i).Select(t=>PotReplayPresentation.Id(probe.Target(t)?.gameObject)).SequenceEqual(f.presentation.columns[i].ids),"column "+i+" restores IDs");
   Check(PotProbeActivation.ActiveCount==f.presentation.activations.Length,"concurrent activation seek count");Check(probe.View.Preview.Active==(f.presentation.previewId>=0),"preview seek visibility");Check(BoardToolVisual.HasBackground==f.presentation.storeVisible,"stored background restored");
  }
  player.Seek(activation.t);var effect=UnityEngine.Object.FindObjectOfType<PotProbeActivation>();float age=effect.Elapsed;yield return new WaitForSecondsRealtime(.2f);Check(Mathf.Abs(effect.Elapsed-age)<.001f,"pause freezes activation clock");Check(!effect.GetComponent<AudioSource>().isPlaying,"paused seek is silent");
  player.SetSpeed(2);player.Pause();yield return new WaitForSecondsRealtime(.06f);Check(effect&&effect.Elapsed>age+.07f,"2x advances presentation clock");player.Pause();
  end.presentation.activations=activation.presentation.activations;end.presentation.ringVisible=true;end.presentation.ring="red circle";end.presentation.ringAlpha=.5f;end.presentation.combo=0;end.presentation.stars=0;
  player.Seek(data.duration-.02f);player.SetSpeed(3);player.Pause();yield return new WaitForSecondsRealtime(.08f);
  Check(UnityEngine.Object.FindObjectsOfType<PotProbeActivation>().All(e=>!e.GetComponent<AudioSource>().isPlaying),"terminal frame freezes active audio");
  Check(probe.View.Ring.AnimationState.GetCurrent(0).Animation.Name=="red circle"&&Mathf.Abs(probe.View.Ring.color.a-.5f)<.001f,"seek restores exact fading ring animation");
  player.Seek(0);Check(probe.State.Count==0&&PotProbeActivation.ActiveCount==0,"rewind clears future rewards and effects");player.Exit();yield return null;Check(!PotReplayPlayer.Active&&!probe.ReplayMode&&PotProbeActivation.ActiveCount==0&&Time.timeScale==1,"exit resets replay state");Check(probe.HasUsedProbe==learned,"replay preserves personal teaching state");
 }
}
