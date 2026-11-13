using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PotDailyProbeValidation {
    const string Key="pot.daily.probe.validation";
    const string Output="work/pot-daily-probe/checks.txt";
    static PotDailyProbeValidation(){EditorApplication.playModeStateChanged+=s=>{
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
        foreach(bool testTools in new[]{false,true}){
            flow.BeginQualified(0,"pot-local-fixture","bachelor","fixture",level,testTools);
            for(int stage=1;stage<=2;stage++){
                while(flow.CreationInProgress)yield return null;
                var probe=ComboProbeController.Instance;string label="testTools="+testTools+" stage="+stage;
                Check(flow.Stage==stage&&probe&&probe.Running&&probe.View.gameObject.activeInHierarchy,label+" probe visible");
                Check(flow.UnlimitedTools==testTools,label+" only tool privilege differs");
                Check(probe.State.Combo==0&&probe.State.Count==0,label+" fresh combo state");
                Check(LevelGenerator.Instance.objectsOnScene.All(o=>{var target=o.GetComponent<ComboProbeTarget>();return target&&probe.Target(target.Token)==target;}),label+" all restored items registered");
                var item=LevelGenerator.Instance.objectsOnScene[0];probe.PickAccepted(item,ProbePickSource.Manual,true);
                Check(probe.State.Combo==1&&probe.State.Stars==3,label+" manual match activates combo");
                probe.PickAccepted(item,ProbePickSource.Automatic,true);
                Check(probe.State.Combo==1&&probe.State.Stars==3,label+" magnet preserves combo rules");
                if(stage==1)flow.NextStage();
            }
            flow.RestartStage();while(flow.CreationInProgress)yield return null;
            Check(ComboProbeController.Instance.Running&&ComboProbeController.Instance.State.Combo==0,"restart resets and enables probe");
            flow.Home();Check(!ComboProbeController.Instance.Running&&!ComboProbeController.Instance.View.gameObject.activeSelf,"home hides probe");
        }
    }
}
