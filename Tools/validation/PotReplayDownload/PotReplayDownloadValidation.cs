using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PotReplayDownloadValidation {
    const string Key="pot.replay.download.validation",Output="work/pot-replay-download",Owner="pot-download-fixture";
    const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
    [Serializable] class Timings {public float[] baseline=new float[3],direct=new float[3];}
    static PotReplayDownloadValidation(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))EditorApplication.delayCall+=()=>new GameObject("Pot download validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());};}
    public static void Run(){SessionState.SetBool(Key,true);File.WriteAllText(Output+"/checks.txt","");EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Check(bool ok,string text){if(!ok)throw new Exception(text);File.AppendAllText(Output+"/checks.txt","PASS "+text+"\n");}
    static IEnumerator Guard(){string error=null;yield return Match3DStartupRunner.Run(Core(),e=>error=e.ToString());SessionState.SetBool(Key,false);File.AppendAllText(Output+"/checks.txt",error==null?"COMPLETE\n":"FAIL "+error+"\n");EditorApplication.Exit(error==null?0:1);}
    static void Mode(string value)=>File.WriteAllText(Output+"/mode",value);
    static void Account(string owner){var a=Match3DInvitations.Instance;typeof(Match3DInvitations).GetProperty("State").SetValue(a,new Match3DInvitations.Reply{inviteCode=owner});typeof(Match3DInvitations).GetField("token",Private).SetValue(a,"pot-local-fixture");}
    static IEnumerator SwitchAccount(){yield return new WaitForSecondsRealtime(.3f);Account("pot-download-other");}
    static IEnumerator Core(){
        float deadline=Time.realtimeSinceStartup+20;
        while(!(Match3DFlow.Instance&&Match3DFlow.Instance.StartupComplete)&&Time.realtimeSinceStartup<deadline)yield return null;
        Check(Match3DFlow.Instance&&Match3DFlow.Instance.StartupComplete,"startup completes");
        var flow=Match3DFlow.Instance;
        foreach(var service in flow.GetComponents<MonoBehaviour>())if(service!=flow){service.StopAllCoroutines();service.enabled=false;service.GetType().GetField("endpoint",Private)?.SetValue(service,File.ReadAllText(Output+"/endpoint"));}
        Account(Owner);var replay=PotReplayService.Instance;var timings=new Timings();int frames=int.Parse(File.ReadAllText(Output+"/expected"));
        string expected=null;
        for(int i=0;i<3;i++){
            Mode("baseline-"+i);PotReplayService.Reply old=null;float start=Time.realtimeSinceStartup;
            yield return PotReplayDownloadBaseline.Detail(replay,new string('a',64),r=>old=r);timings.baseline[i]=Time.realtimeSinceStartup-start;
            Check(old!=null&&old.ok&&old.detail.frames.Count==frames,"baseline downloads full fixture "+i);
            expected=JsonUtility.ToJson(old.detail);old=null;
            Mode("direct-"+i);PotReplayService.Reply current=null;start=Time.realtimeSinceStartup;
            yield return replay.Detail(new string('a',64),r=>current=r);timings.direct[i]=Time.realtimeSinceStartup-start;
            Check(current!=null&&current.ok&&JsonUtility.ToJson(current.detail)==expected,"direct replay equals baseline including all frames "+i);
        }
        File.WriteAllText(Output+"/timings.json",JsonUtility.ToJson(timings));
        foreach(var mode in new[]{"retry","denied","bad-url","corrupt","oversize","unavailable","switch"}){
            Mode(mode);Account(Owner);int callbacks=0;PotReplayService.Reply result=null;
            if(mode=="switch")flow.StartCoroutine(SwitchAccount());
            yield return replay.Detail(new string('a',64),r=>{callbacks++;result=r;});
            Check(callbacks==1&&result!=null,"exactly one completion for "+mode);
            if(mode=="retry")Check(result.ok&&JsonUtility.ToJson(result.detail)==expected,"expired URL refresh succeeds");
            else Check(!result.ok&&!string.IsNullOrEmpty(result.error),"rejects "+mode+": "+result.error);
        }
    }
}
