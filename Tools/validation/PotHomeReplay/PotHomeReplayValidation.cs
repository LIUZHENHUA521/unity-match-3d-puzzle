using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object=UnityEngine.Object;

[InitializeOnLoad]
public static class PotHomeReplayValidation {
    const string Key="pot.home.replay.validation",Output="work/pot-home-replay",Owner="pot-home-validation-a",Other="pot-home-validation-b";
    const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
    static PotHomeReplayValidation(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))EditorApplication.delayCall+=()=>new GameObject("Pot home replay validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());};}
    public static void Run(){SessionState.SetBool(Key,true);File.WriteAllText(Output+"/checks.txt","");EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Check(bool ok,string text){if(!ok)throw new Exception(text);File.AppendAllText(Output+"/checks.txt","PASS "+text+"\n");}
    static IEnumerator Guard(){
        string error=null;yield return Match3DStartupRunner.Run(Core(),e=>error=e.ToString());
        foreach(var owner in new[]{Owner,Other})foreach(var tier in new[]{"bachelor","elite"})PlayerPrefs.DeleteKey("pot.home-replay.win."+owner+"."+tier);
        SessionState.SetBool(Key,false);File.AppendAllText(Output+"/checks.txt",error==null?"COMPLETE\n":"FAIL "+error+"\n");EditorApplication.Exit(error==null?0:1);
    }
    static void Mode(string value)=>File.WriteAllText(Output+"/api-mode",value);
    static void Account(string owner){var a=Match3DInvitations.Instance;typeof(Match3DInvitations).GetProperty("State").SetValue(a,new Match3DInvitations.Reply{inviteCode=owner});typeof(Match3DInvitations).GetField("token",Private).SetValue(a,"pot-local-fixture");}
    static Match3DUIView Home()=>Object.FindObjectOfType<Match3DHomeView>().GetComponent<Match3DUIView>();
    static Text Caption(string key)=>Home().Get<Button>(key).transform.Find("Pot replay caption").GetComponent<Text>();
    static string[] IDs()=> (string[])typeof(PotHomeReplayEntry).GetField("ids",Private).GetValue(Object.FindObjectOfType<PotHomeReplayEntry>());
    static IEnumerator Wait(Func<bool> condition,string message){float end=Time.realtimeSinceStartup+12;while(!condition()&&Time.realtimeSinceStartup<end)yield return null;Check(condition(),message);}
    static IEnumerator Core(){
        yield return Wait(()=>Match3DFlow.Instance&&Match3DFlow.Instance.StartupComplete,"startup completes");
        var flow=Match3DFlow.Instance;
        foreach(var service in flow.GetComponents<MonoBehaviour>())if(service!=flow){
            service.StopAllCoroutines();service.enabled=false;
            service.GetType().GetField("endpoint",Private)?.SetValue(service,File.ReadAllText(Output+"/endpoint"));
        }
        var replay=PotReplayService.Instance;
        Account(Owner);foreach(var tier in new[]{"bachelor","elite"})PlayerPrefs.DeleteKey("pot.home-replay.win."+Owner+"."+tier);
        Mode("pending");flow.Home();yield return new WaitForSecondsRealtime(1.2f);
        Check(!Caption("Bachelor").gameObject.activeSelf&&!Caption("Elite").gameObject.activeSelf,"unfinished tiers retain original reward artwork");
        foreach(var key in new[]{"Bachelor","Elite"})Check(Home().Get<Button>(key).GetComponentsInChildren<Image>().Count(x=>x.sprite&&x.sprite.name.StartsWith("PotHome"))==4,key+" background title packet and caption are separate sprites");
        Capture("pot-home-normal",720,1565);
        var win=new Match3DGameRecords.Record{stage=2,status="win",levelSeed=123,challengeTier="bachelor",levelDate=Match3DFlow.Today};
        PotHomeReplayState.MarkWin(Owner,win);flow.Home();yield return new WaitForSecondsRealtime(1.2f);
        Check(Caption("Bachelor").text=="回放待同步"&&Caption("Bachelor").gameObject.activeSelf&&!Caption("Elite").gameObject.activeSelf,"win is pending while other tier stays unchanged: "+Caption("Bachelor").text+" active="+Caption("Bachelor").gameObject.activeSelf+" elite="+Caption("Elite").gameObject.activeSelf+" account="+Match3DInvitations.Instance.State.inviteCode+" won="+PotHomeReplayState.Won(Owner,Match3DFlow.Today,"bachelor")+" entryOwner="+typeof(PotHomeReplayEntry).GetField("owner",Private).GetValue(Object.FindObjectOfType<PotHomeReplayEntry>())+" entryDate="+typeof(PotHomeReplayEntry).GetField("date",Private).GetValue(Object.FindObjectOfType<PotHomeReplayEntry>())+" ready="+Match3DInvitations.Instance.Ready);
        Check(!PotHomeReplayState.Won(Other,Match3DFlow.Today,"bachelor")&&!PotHomeReplayState.Won(Owner,"2000-01-01","bachelor"),"win markers isolate account and day");
        var row=new PotReplayService.Summary{id="a",status="win",stage=2,tier="bachelor",date=Match3DFlow.Today,resourceVersion=PotReplayData.Resources};
        Check(PotHomeReplayState.Matches(row,Match3DFlow.Today,"bachelor"),"uploaded matching win is accepted");
        row.status="fail";Check(!PotHomeReplayState.Matches(row,Match3DFlow.Today,"bachelor"),"failed replay is excluded");row.status="incomplete";Check(!PotHomeReplayState.Matches(row,Match3DFlow.Today,"bachelor"),"recovered incomplete replay is excluded");row.status="win";
        Check(!PotHomeReplayState.Matches(row,"2000-01-01","bachelor")&&!PotHomeReplayState.Matches(row,Match3DFlow.Today,"elite"),"other date or tier is excluded");row.resourceVersion="unsupported";Check(!PotHomeReplayState.Matches(row,Match3DFlow.Today,"bachelor"),"unsupported replay resources are excluded");
        Capture("pot-home-pending",720,1565);
        int notifications=0;Action<string> notified=owner=>{if(owner==Owner)notifications++;};replay.Uploaded+=notified;
        foreach(var mode in new[]{"finalized","already-ready"}){
            Mode(mode);string path=PotReplayService.PathFor(Owner,new string('e',32),".pending");PotReplayFiles.Write(path,"{}");
            yield return (IEnumerator)typeof(PotReplayService).GetMethod("UploadOne",Private).Invoke(replay,new object[]{Owner,path});
            Check(!PotReplayFiles.Exists(path),mode+" removes the completed queue entry");
        }
        Check(notifications==2,"finalization retry and already-ready upload start both notify the home");
        Mode("ready");string completed=PotReplayService.PathFor(Owner,new string('d',32),".pending");PotReplayFiles.Write(completed,"{}");
        typeof(PotReplayService).GetMethod("CompleteUpload",Private).Invoke(replay,new object[]{Owner,completed});
        yield return Wait(()=>IDs().All(x=>!string.IsNullOrEmpty(x)),"upload completion refresh finds wins beyond the first 20 records");
        Check(Caption("Bachelor").text=="查看回放"&&Caption("Elite").text=="查看回放","both completed tiers show view replay");
        Check(IDs()[0]==new string('a',64)&&IDs()[1]==new string('b',64),"each tier retains its own replay id");
        Check(!Home().Get<Button>("Bachelor").transform.Find("Pot home BachelorPacket").gameObject.activeSelf,"ready state removes red packet artwork from caption row");
        Capture("pot-home-ready",720,1565);
        Mode("download-fail");Home().Get<Button>("Bachelor").onClick.Invoke();Home().Get<Button>("Bachelor").onClick.Invoke();
        yield return new WaitForSecondsRealtime(.8f);
        Check(!PotReplayPlayer.Active&&flow.ScreenName=="home"&&!(bool)typeof(PotHomeReplayEntry).GetField("opening",Private).GetValue(Object.FindObjectOfType<PotHomeReplayEntry>()),"download failure preserves home and permits retry");
        Mode("pending");Account(Other);yield return new WaitForSecondsRealtime(1.3f);
        Check(IDs().All(string.IsNullOrEmpty)&&!Caption("Bachelor").gameObject.activeSelf,"account switch clears prior replay and win state");
        Account(Owner);Mode("slow");flow.Home();yield return new WaitForSecondsRealtime(.2f);Account(Other);Mode("pending");yield return new WaitForSecondsRealtime(3.2f);
        Check(IDs().All(string.IsNullOrEmpty),"in-flight results from previous account cannot expose its replay");
        Account(Owner);Mode("download-ok");flow.Home();yield return Wait(()=>IDs()[0]!=null,"ready state restores from cloud after reopening home");
        Home().Get<Button>("Bachelor").onClick.Invoke();yield return Wait(()=>PotReplayPlayer.Active&&flow.ScreenName=="pot-replay","home entry downloads and opens the existing replay player");
        PotReplayPlayer.Instance.Exit();yield return null;Check(!PotReplayPlayer.Active&&Time.timeScale==1,"replay exit returns cleanly to home");
        replay.Uploaded-=notified;
    }
    static void Capture(string name,int width,int height){
        var camera=Camera.main;var previous=camera.targetTexture;float aspect=camera.aspect;
        var overlays=Object.FindObjectsOfType<Canvas>().Where(c=>c.isRootCanvas&&c.renderMode==RenderMode.ScreenSpaceOverlay).ToArray();
        var oldCameras=overlays.Select(c=>c.worldCamera).ToArray();var distances=overlays.Select(c=>c.planeDistance).ToArray();
        var rt=new RenderTexture(width,height,24);rt.Create();var active=RenderTexture.active;var texture=new Texture2D(width,height,TextureFormat.RGB24,false);
        try{
            camera.targetTexture=rt;camera.aspect=width/(float)height;
            foreach(var c in overlays){c.renderMode=RenderMode.ScreenSpaceCamera;c.worldCamera=camera;c.planeDistance=.5f;}
            Canvas.ForceUpdateCanvases();Object.FindObjectOfType<Match3DHomeView>().SendMessage("Layout");Canvas.ForceUpdateCanvases();
            camera.Render();RenderTexture.active=rt;texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply();File.WriteAllBytes(Output+"/"+name+".png",texture.EncodeToPNG());
        }finally{for(int i=0;i<overlays.Length;i++){overlays[i].renderMode=RenderMode.ScreenSpaceOverlay;overlays[i].worldCamera=oldCameras[i];overlays[i].planeDistance=distances[i];}camera.targetTexture=previous;camera.aspect=aspect;RenderTexture.active=active;Object.DestroyImmediate(texture);rt.Release();Object.DestroyImmediate(rt);}
    }
}
