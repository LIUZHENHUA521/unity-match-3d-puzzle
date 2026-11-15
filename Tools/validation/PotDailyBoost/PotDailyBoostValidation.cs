using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PotDailyBoostValidation {
    const string Key="pot.daily.boost.validation",Output="work/pot-daily-boost/checks.txt";
    static PotDailyBoostValidation(){EditorApplication.playModeStateChanged+=s=>{
        if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))
            new GameObject("Pot daily boost validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
    };}
    public static void Run(){Directory.CreateDirectory(Path.GetDirectoryName(Output));File.WriteAllText(Output,"");SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);File.AppendAllText(Output,"PASS "+message+"\n");}
    static bool SameOpening(Match3DDailyStage actual,Match3DDailyStage expected){
        if(actual.items.Length!=expected.items.Length)return false;
        foreach(var a in actual.items){var e=expected.items.Single(i=>i.id==a.id);
            if(a.type!=e.type||Vector3.Distance(a.position,e.position)>.00001f||Quaternion.Angle(a.rotation,e.rotation)>.05f||Vector3.Distance(a.scale,e.scale)>.00001f||a.visible!=e.visible)return false;
        }
        return Math.Abs(actual.threshold-expected.threshold)<.001f&&Math.Abs(actual.physicsThreshold-expected.physicsThreshold)<.001f;
    }
    static IEnumerator Guard(){
        var test=Core();string error=null;float deadline=Time.realtimeSinceStartup+180;
        while(true){object current=null;bool next=false;try{if(Time.realtimeSinceStartup>deadline)throw new Exception("timeout");next=test.MoveNext();if(next)current=test.Current;}catch(Exception e){error=e.ToString();}
            if(error!=null||!next)break;yield return current;
        }
        File.AppendAllText(Output,error==null?"COMPLETE\n":"FAIL "+error+"\n");SessionState.SetBool(Key,false);EditorApplication.Exit(error==null?0:1);
    }
    static IEnumerator Core(){
        while(!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)yield return null;
        foreach(var service in new MonoBehaviour[]{Match3DGameRecords.Instance,Match3DInvitations.Instance,PotReplayService.Instance})if(service){service.StopAllCoroutines();service.enabled=false;}
        var flow=Match3DFlow.Instance;Match3DDailyStage first=null;
        foreach(int n in new[]{0,60,120}){
            var level=new Match3DDailyLevel{date="2026-11-15",tier="bachelor",version="pot-boost-validation",candidateId="candidate-0123456789abcdef01234567",generatorVersion="daily-v1",modelThemeId="foodie-party",seed=47231+n,seconds=600,initialCount=60+n,limitHeight=13.5f,layers=new[]{new ChallengeLayer(30+n,5,10),new ChallengeLayer(30,5,14)},potVariant=n==0?"normal":"boosted",potBoostCount=n};
            Check(level.IsValid(23),"valid dynamic count "+n);
            var json=JsonUtility.ToJson(level);var copy=JsonUtility.FromJson<Match3DDailyLevel>(json);
            Check(copy.potBoostCount==n&&copy.potVariant==level.potVariant,"variant survives serialization "+n);
            var tiles=Match3DDailyLevel.Generate(23,level.layers,level.RandomForStage(2));
            Check(tiles.Count(t=>t.layer==0)==30+n&&tiles.Count(t=>t.layer==1)==30,"only first layer gains n "+n);
            var opening=new Match3DDailySnapshot{stages=new Match3DDailyStage[2]};
            for(int stage=1;stage<=2;stage++){
                if(stage==1&&first!=null){opening.stages[0]=first;continue;}
                flow.PreviewDailyOpening(level,stage);while(flow.CreationInProgress)yield return null;
                yield return new WaitForSeconds(5);
                opening.stages[stage-1]=flow.CaptureDailyOpening();
                if(stage==1)first=opening.stages[0];
            }
            level.opening=opening;level.generatorVersion="shared-v2";
            Check(level.IsValid(23)&&opening.stages[0].items.Length==15,"paired snapshot valid, tutorial unchanged "+n);
            foreach(int stage in new[]{1,2}){
                flow.PreviewDailyOpening(level,stage);while(flow.CreationInProgress)yield return null;
                var expected=opening.stages[stage-1];
                Check(SameOpening(flow.CaptureDailyOpening(),expected),"exact snapshot restore stage="+stage+" n="+n);
                flow.RestartStage();while(flow.CreationInProgress)yield return null;
                Check(SameOpening(flow.CaptureDailyOpening(),expected),"restart preserves board stage="+stage+" n="+n);
            }
            Check(flow.LocalPreview,"validation does not upload game records");
            File.WriteAllText("work/pot-daily-boost/level-"+n+".json",JsonUtility.ToJson(level));
        }
        flow.Home();
    }
}
