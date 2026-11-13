using System;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PotReplayRuntimeValidation {
 static double deadline;static int step;static float baseline;static int attempts;static PotReplayData data;
 static PotReplayRuntimeValidation(){if(SessionState.GetBool("pot.replay.runtime",false)){deadline=EditorApplication.timeSinceStartup+180;EditorApplication.update+=Tick;}}
 public static void Run(){SessionState.SetBool("pot.replay.runtime",true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
 static void Tick(){
  if(!EditorApplication.isPlaying)return;
  try{
   if(EditorApplication.timeSinceStartup>deadline)throw new Exception("runtime replay timeout at "+step);
   var flow=Match3DFlow.Instance;if(!flow||!flow.StartupComplete)return;
   if(step==0){var level=new Match3DDailyLevel{date="2026-11-13",tier="bachelor",version="runtime-fixture",generatorVersion="daily-v1",seed=123,seconds=300,initialCount=30,limitHeight=8,layers=new[]{new ChallengeLayer(30,3,4)}};flow.PreviewDailyOpening(level,1);step=1;return;}
   if(step==1){if(flow.CreationInProgress)return;baseline=Time.realtimeSinceStartup;step=2;return;}
   if(step==2){if(Time.realtimeSinceStartup-baseline<1)return;
    foreach(var b in new MonoBehaviour[]{Match3DGameRecords.Instance,PotReplayService.Instance,Match3DInvitations.Instance})if(b){b.StopAllCoroutines();b.enabled=false;}
    var account=Match3DInvitations.Instance;typeof(Match3DInvitations).GetField("<State>k__BackingField",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(account,new Match3DInvitations.Reply{inviteCode=new string('a',64)});typeof(Match3DInvitations).GetField("token",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(account,"fixture-no-network");
    typeof(Match3DFlow).GetField("<LocalPreview>k__BackingField",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(flow,false);
    PotReplayRecorder.Instance.Begin(new Match3DGameRecords.Record{recordId=new string('c',32),runId=new string('d',32),stage=1,initialCount=15},flow.PotDailyLevel);
    baseline=Time.realtimeSinceStartup;step=20;return;
   }
   if(step==20){if(Time.realtimeSinceStartup-baseline<1)return;PotReplayRecorder.Instance.Finish("quit");var path=PotReplayService.PathFor(new string('a',64),new string('c',32),".pending");Check(File.Exists(path),"recorder persisted a real daily recording");File.Copy(path,"/tmp/pot-replay-runtime-detail.json",true);File.Delete(path);
    typeof(Match3DFlow).GetField("<LocalPreview>k__BackingField",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(flow,true);
    var opening=flow.CaptureDailyOpening();data=new PotReplayData{level=flow.PotDailyLevel,opening=opening,stage=1,duration=2,status="quit",recordId=new string('a',32),runId=new string('b',32)};
    for(int i=0;i<3;i++){var frame=new PotReplayFrame{t=i,full=true,remaining=300-i,tray=i==1?new[]{opening.items[0].id}:new int[0],cameraPosition=Camera.main.transform.position,cameraRotation=Camera.main.transform.rotation,fov=Camera.main.fieldOfView,cameraSize=Camera.main.orthographicSize,orthographic=Camera.main.orthographic};foreach(var item in opening.items)frame.items.Add(new PotReplayPose{id=item.id,p=item.position+Vector3.right*i*.1f,r=item.rotation,s=item.scale,visible=i!=2,zone=i==1?1:0});data.frames.Add(frame);}
    attempts=PlayerPrefs.GetInt("goose.result.attempts",0);flow.gameObject.AddComponent<PotReplayPlayer>().Load(data);step=3;return;
   }
   if(step==3){if(flow.ScreenName!="pot-replay")return;PotReplayPlayer.Instance.Pause();PotReplayPlayer.Instance.Seek(2);baseline=Time.realtimeSinceStartup;step=4;return;}
   if(step==4){if(Time.realtimeSinceStartup-baseline<.2f)return;Check(PlayerPrefs.GetInt("goose.result.attempts",0)==attempts,"playback must not increment attempts");Check(!InputController.Instance.enabled,"input disabled");Check(!GameManager.Instance.enabled,"settlement loop disabled");Check(Time.timeScale==0,"game clock frozen");Check(LevelGenerator.Instance.objectsOnScene.All(o=>!o.GetComponentsInChildren<Renderer>().Any(r=>r.enabled)),"seek end visibility");PotReplayPlayer.Instance.Seek(0);baseline=Time.realtimeSinceStartup;step=5;return;}
   if(step==5){if(Time.realtimeSinceStartup-baseline<.2f)return;foreach(var obj in LevelGenerator.Instance.objectsOnScene){int id=obj.GetComponent<Match3DTile>().id;var expected=data.opening.items.First(x=>x.id==id);Check(Vector3.Distance(obj.transform.position,expected.position)<.001f,"backward seek restores position "+id);}PotReplayPlayer.Instance.Exit();Check(!PotReplayPlayer.Active&&Time.timeScale==1&&InputController.Instance.enabled&&GameManager.Instance.enabled,"exit restores services");Debug.Log("POT_REPLAY_RUNTIME_OK readonly + seek + exit");Finish(0);}
  }catch(Exception e){Debug.LogException(e);Finish(1);}
 }
 static void Check(bool value,string message){if(!value)throw new Exception(message);}
 static void Finish(int code){SessionState.SetBool("pot.replay.runtime",false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
}
