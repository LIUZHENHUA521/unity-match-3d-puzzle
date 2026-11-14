using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PotDailyOpeningValidation {
    const string Key="pot.daily.opening.validation";
    const string Output="work/pot-daily-opening/checks.txt";
    static PotDailyOpeningValidation(){EditorApplication.playModeStateChanged+=s=>{
        if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))
            new GameObject("Pot daily probe validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
    };}
    public static void Run(){Directory.CreateDirectory(Path.GetDirectoryName(Output));File.WriteAllText(Output,"");SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);File.AppendAllText(Output,"PASS "+message+"\n");}
    static IEnumerator Guard(){
        var test=Core();string error=null;float deadline=Time.realtimeSinceStartup+90;
        while(true){object current=null;bool next=false;try{if(Time.realtimeSinceStartup>deadline)throw new Exception("timeout");next=test.MoveNext();if(next)current=test.Current;}catch(Exception e){error=e.ToString();}
            if(error!=null||!next)break;yield return current;
        }
        File.AppendAllText(Output,error==null?"COMPLETE\n":"FAIL "+error+"\n");SessionState.SetBool(Key,false);EditorApplication.Exit(error==null?0:1);
    }
    static IEnumerator Core(){
        while(!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)yield return null;
        foreach(var service in new MonoBehaviour[]{Match3DGameRecords.Instance,Match3DInvitations.Instance,PotReplayService.Instance})if(service){service.StopAllCoroutines();service.enabled=false;}
        var flow=Match3DFlow.Instance;flow.Begin(0);
        while(flow.CreationInProgress)yield return null;
        var first=flow.CaptureDailyOpening();var second=JsonUtility.FromJson<Match3DDailyStage>(JsonUtility.ToJson(first));second.stage=2;
        var level=new Match3DDailyLevel{date="2026-11-13",tier="bachelor",version="pot-probe-fixture",generatorVersion="shared-v2",seed=123,seconds=300,initialCount=15,limitHeight=3.5f,layers=new[]{new ChallengeLayer(15,5,4)},opening=new Match3DDailySnapshot{stages=new[]{first,second}}};
        first.items[0].visible=false;second.items[0].visible=false;
        foreach(bool testTools in new[]{false,true}){
            flow.BeginQualified(0,"pot-local-fixture","bachelor","fixture",level,testTools);
            for(int stage=1;stage<=2;stage++){
                var expected=level.opening.stages[stage-1];bool sawIntermediate=false;int animationFrames=0;
                float seconds=GameManager.Instance.currentLevelTime;
                while(flow.CreationInProgress){
                    animationFrames++;
                    Check(GameManager.Instance.gameStatus==GameManager.GameStatus.PREPARING,"input remains locked during opening");
                    Check(ItemPhysicsController.Instance.DailyOpeningHeld&&ItemPhysicsController.Instance.DynamicCount==0,"physics held during opening");
                    Check(GameManager.Instance.currentLevelTime==seconds,"timer held during opening");
                    foreach(var obj in LevelGenerator.Instance.objectsOnScene){
                        int id=obj.GetComponent<Match3DTile>().id;var saved=expected.items.First(i=>i.id==id);
                        float fraction=obj.transform.localScale.x/saved.scale.x;
                        if(saved.visible&&fraction>0&&fraction<1)sawIntermediate=true;
                        if(!saved.visible)Check(obj.transform.localScale==saved.scale&&!LayeredItemPile.Instance.DailyVisible(obj.GetComponent<Rigidbody>()),"hidden item preserved");
                    }
                    yield return null;
                }
                Check(sawIntermediate&&animationFrames>3,"visible growth across multiple frames, stage="+stage+" testTools="+testTools);
                var actual=flow.CaptureDailyOpening();
                Check(actual.threshold==expected.threshold&&actual.physicsThreshold==expected.physicsThreshold&&actual.touchHeight==expected.touchHeight&&actual.maxHeight==expected.maxHeight,"snapshot height thresholds preserved");
                foreach(var item in actual.items){
                    var saved=expected.items.First(i=>i.id==item.id);
                    Check(item.position==saved.position&&Quaternion.Angle(item.rotation,saved.rotation)<.01f&&item.scale==saved.scale&&item.visible==saved.visible,"exact final pose/visibility "+item.id);
                }
                Check(ItemPhysicsController.Instance.DailyOpeningHeld,"opening remains held until interaction");
                ItemPhysicsController.Instance.ReleaseDailyOpening();
                foreach(var obj in LevelGenerator.Instance.objectsOnScene){var saved=expected.items.First(i=>i.id==obj.GetComponent<Match3DTile>().id);Check(!obj.GetComponent<Rigidbody>().isKinematic==saved.dynamic,"saved dynamic state restored");}
                if(stage==1)flow.NextStage();
            }
            flow.Home();
        }
        flow.BeginQualified(0,"pot-local-fixture","bachelor","fixture",level,false);
        while(LevelGenerator.Instance.objectsOnScene.Count<15)yield return null;
        for(int i=0;i<4;i++)yield return null;
        Time.timeScale=0;
        var scales=LevelGenerator.Instance.objectsOnScene.Select(o=>o.transform.localScale).ToArray();
        for(int i=0;i<5;i++)yield return null;
        Check(flow.CreationInProgress&&LevelGenerator.Instance.objectsOnScene.Select(o=>o.transform.localScale).SequenceEqual(scales),"pause freezes opening animation");
        Time.timeScale=1;flow.Home();
        for(int i=0;i<5;i++)yield return null;
        Check(!flow.CreationInProgress&&flow.ScreenName=="home"&&LevelGenerator.Instance.objectsOnScene.Count==0,"home cancels opening without stale completion");
        flow.BeginQualified(0,"pot-local-fixture","bachelor","fixture",level,false);
        yield return null;flow.RestartStage();
        while(flow.CreationInProgress)yield return null;
        Check(LevelGenerator.Instance.objectsOnScene.Count==15,"restart during opening replaces old board");
        flow.BeginPotReplay(level,1,first);
        bool replayShrank=false;
        while(flow.CreationInProgress){
            foreach(var obj in LevelGenerator.Instance.objectsOnScene){var saved=first.items.First(i=>i.id==obj.GetComponent<Match3DTile>().id);if(obj.transform.localScale!=saved.scale)replayShrank=true;}
            yield return null;
        }
        Check(!replayShrank,"replay restores full scale without entrance animation");flow.Home();
    }
}
