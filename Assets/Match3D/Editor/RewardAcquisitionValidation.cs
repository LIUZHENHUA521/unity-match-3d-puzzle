using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Policy=Match3DRewardAcquisition;

public static class RewardAcquisitionValidation {
    const string Output="docs/validation/reward-acquisition/result.txt";
    const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
    [InitializeOnLoadMethod] static void Init(){EditorApplication.playModeStateChanged+=s=>{
        if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("reward-acquisition-check",false)){
            SessionState.SetBool("reward-acquisition-check",false);
            new GameObject("Acquisition validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
        }
    };}
    public static void Run(){Directory.CreateDirectory(Path.GetDirectoryName(Output));File.WriteAllText(Output,"");EditorSceneManager.OpenScene(Match3DBuilder.Scene);SessionState.SetBool("reward-acquisition-check",true);EditorApplication.isPlaying=true;}
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);File.AppendAllText(Output,"PASS "+message+"\n");}
    static IEnumerator Guard(){
        var steps=RunChecks();Exception failure=null;
        while(true){bool next=false;object value=null;try{next=steps.MoveNext();if(next)value=steps.Current;}catch(Exception e){failure=e;}
            if(!next||failure!=null)break;yield return value;
        }
        (steps as IDisposable)?.Dispose();
        File.AppendAllText(Output,failure==null?"ALL PASSED\n":"FAIL "+failure+"\n");EditorApplication.Exit(failure==null?0:1);
    }
    static Match3DReviveView View=>UnityEngine.Object.FindObjectOfType<Match3DReviveView>();
    static Button Tool(string name)=>UnityEngine.Object.FindObjectOfType<Match3DGameplayHUD>().GetComponent<Match3DUIView>().Get<Button>(name);
    static string Label=>View.revive.GetComponentInChildren<Text>().text;
    static void Setup(){var f=Match3DFlow.Instance;f.Begin(0);BoardCreationTestSupport.FinishPending(f);f.NextStage();BoardCreationTestSupport.FinishPending(f);typeof(Match3DFlow).GetProperty("LocalPreview").SetValue(f,true);typeof(Match3DFlow).GetField("unlimitedMagnetRun",Flags).SetValue(f,false);GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;}
    static IEnumerator RunChecks(){
        var config=new Policy.Configuration();
        for(int i=0;i<8;i++)Check(Policy.Resolve(config,Policy.Entry.Tool,i,0)==(i%2==0?Policy.Route.Share:Policy.Route.Ad),"configured sequence step "+i);
        config.tool.freeUsesPerRound=1;Check(Policy.Resolve(config,Policy.Entry.Tool,1,0)==Policy.Route.Free,"free precedes sequence");
        Check(Policy.Resolve(config,Policy.Entry.Tool,1,1)==Policy.Route.Ad,"free does not shift paid sequence");
        config.adsEnabled=false;Check(Policy.Resolve(config,Policy.Entry.Tool,1,1)==Policy.Route.Share,"ad switch falls back to share");
        config.sharesEnabled=false;Check(Policy.Resolve(config,Policy.Entry.Tool,1,1)==Policy.Route.Unavailable,"both disabled never grant free");
        config.adsEnabled=true;Check(Policy.Resolve(config,Policy.Entry.Tool,0,1)==Policy.Route.Ad,"share switch falls back to ad");
        config=new Policy.Configuration();Policy.Configure(config);
        float deadline=Time.realtimeSinceStartup+45;
        while((!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)&&Time.realtimeSinceStartup<deadline)yield return null;
        var flow=Match3DFlow.Instance;Check(flow&&flow.StartupComplete,"scene startup complete");
        var account=Match3DInvitations.Instance;account.StopAllCoroutines();account.enabled=false;Match3DGameRecords.Instance.StopAllCoroutines();
        var customer=UnityEngine.Object.FindObjectOfType<Match3DCustomerService>();customer.StopAllCoroutines();customer.enabled=false;
        var oldState=account.State;object oldToken=typeof(Match3DInvitations).GetField("token",Flags).GetValue(account);
        string owner="acquisition-validation-"+Guid.NewGuid().ToString("N");
        typeof(Match3DInvitations).GetProperty("State").SetValue(account,new Match3DInvitations.Reply{inviteCode=owner,inventory=new int[4]});
        typeof(Match3DInvitations).GetField("token",Flags).SetValue(account,"local-validation");
        string toolKey=Policy.CounterKey(Policy.Entry.Tool,owner),reviveKey=Policy.CounterKey(Policy.Entry.Revive,owner);
        Action<bool,string> share=null,ad=null;int shares=0,ads=0;
        Match3DRewardedVideo.EditorShareOverride=done=>{shares++;share=done;};
        Match3DRewardedVideo.EditorShowOverride=done=>{ads++;ad=done;};
        try{
            Setup();yield return null;
            Tool("Shuffle").onClick.Invoke();yield return new WaitUntil(()=>View&&View.group.interactable);
            Check(Label=="分享并使用"&&Time.timeScale==0,"first tool offers share and pauses");Capture("tool-share");
            View.revive.onClick.Invoke();View.revive.onClick.Invoke();Check(shares==1&&ads==0&&BoardTools.UsedCount(2)==0,"pending share locks duplicate clicks and grants nothing: shares="+shares+", ads="+ads+", used="+BoardTools.UsedCount(2)+", interactive="+View.group.interactable+", status="+View.status.text+", account="+account.Ready+", tips="+string.Join("|",UnityEngine.Object.FindObjectsOfType<Match3DTips>().SelectMany(x=>x.GetComponentsInChildren<Text>()).Select(x=>x.text)));
            share(false,"返回过快");Check(View.group.interactable&&Policy.Next(Policy.Entry.Tool).route==Policy.Route.Share,"failed share preserves sequence and allows retry");
            View.revive.onClick.Invoke();share(true,null);share(true,null);
            Check(BoardTools.UsedCount(2)==1&&BoardTools.ShareUses(2)==1&&PlayerPrefs.GetInt(toolKey)==1&&Time.timeScale==1,"share applies exactly once and advances once");
            yield return new WaitForSecondsRealtime(1.3f);
            Tool("AddTime").onClick.Invoke();yield return new WaitUntil(()=>View&&View.group.interactable);
            Check(Label=="看广告并使用","different tool shares same acquisition counter");Capture("tool-ad");
            float before=GameManager.Instance.currentLevelTime;View.revive.onClick.Invoke();ad(false,"未看完");
            Check(View.group.interactable&&GameManager.Instance.currentLevelTime==before&&PlayerPrefs.GetInt(toolKey)==1,"incomplete ad changes neither time nor sequence");
            View.revive.onClick.Invoke();ad(true,null);ad(true,null);
            Check(GameManager.Instance.currentLevelTime==before+180&&BoardTools.AdUses(3)==1&&PlayerPrefs.GetInt(toolKey)==2,"ad grants exactly one add-time effect");
            yield return null;Tool("AddTime").onClick.Invoke();yield return new WaitUntil(()=>View&&View.group.interactable);
            Check(Label=="分享并使用","sequence cycles back to share");View.close.onClick.Invoke();
            Check(Time.timeScale==1&&PlayerPrefs.GetInt(toolKey)==2,"closing offer preserves sequence and resumes");
            yield return null;Tool("AddTime").onClick.Invoke();yield return new WaitUntil(()=>View&&View.group.interactable);View.revive.onClick.Invoke();var stale=share;
            Setup();yield return null;stale(true,null);Check(BoardTools.UsedCount(3)==0&&PlayerPrefs.GetInt(toolKey)==2,"stale callback cannot affect a new board");
            GameManager.Instance.currentLevelTime=0;flow.HandleResult(false);yield return new WaitUntil(()=>View&&View.group.interactable);
            Check(Label=="分享复活"&&PlayerPrefs.GetInt(reviveKey)==0,"revive starts independent share sequence");Capture("revive-share");
            View.revive.onClick.Invoke();share(true,null);share(true,null);
            Check(flow.TimeReviveUsed&&GameManager.Instance.currentLevelTime==300&&PlayerPrefs.GetInt(reviveKey)==1&&PlayerPrefs.GetInt(toolKey)==2,"share revival once, tool counter untouched");
            Check(flow.ReviveUsage.time.share==1&&flow.ReviveUsage.time.ad==0&&flow.ReviveUsage.clear.share==0,"time revival records share exactly once");
            var input=InputController.Instance;
            foreach(var obj in LevelGenerator.Instance.objectsOnScene.GroupBy(x=>x.name).SelectMany(g=>g.Take(2)).Take(7).ToArray())input.AddObjectOnStack(obj,false);
            yield return new WaitUntil(()=>View&&View.group.interactable);Check(Label=="看广告复活","clear and time revival share revival sequence");
            View.revive.onClick.Invoke();ad(true,null);ad(true,null);
            Check(flow.ClearReviveUsed&&input.objectsOnStack.Count==0&&PlayerPrefs.GetInt(reviveKey)==2,"ad clears tray and advances revival once");
            Check(flow.ReviveUsage.clear.ad==1&&flow.ReviveUsage.clear.share==0&&flow.ReviveUsage.time.share==1,"clear revival records ad exactly once independently of time revival");
            config.tool.freeUsesPerRound=1;account.State.inventory[3]=2;Setup();yield return null;
            Check(flow.ReviveUsage.clear.ad==0&&flow.ReviveUsage.time.share==0&&BoardTools.ShareUses(2)==0&&BoardTools.AdUses(3)==0,"new board resets all acquisition telemetry");
            Tool("AddTime").onClick.Invoke();yield return new WaitUntil(()=>View&&View.group.interactable);Check(Label=="免费使用","free offer takes priority over owned inventory");
            before=GameManager.Instance.currentLevelTime;int calls=shares+ads;View.revive.onClick.Invoke();
            Check(BoardTools.FreeUses(3)==1&&GameManager.Instance.currentLevelTime==before+180&&account.Inventory(3)==2&&shares+ads==calls&&PlayerPrefs.GetInt(toolKey)==2,"free grants once without SDK, inventory or sequence debit");
            account.State.inventory[3]=0;Check(Policy.Next(Policy.Entry.Tool).route==Policy.Route.Share,"exhausted free quota returns to saved sequence");
            config.adsEnabled=false;PlayerPrefs.SetInt(toolKey,3);yield return null;Tool("AddTime").onClick.Invoke();yield return new WaitUntil(()=>View&&View.group.interactable);
            Check(Label=="分享并使用","disabled ad route updates actual button to share");View.close.onClick.Invoke();
            Policy.Configure(null);Check(Policy.Next(Policy.Entry.Tool).route==Policy.Route.Ad,"configuration reload preserves counter");
        }finally{
            flow.Home();Match3DRewardedVideo.EditorShareOverride=null;Match3DRewardedVideo.EditorShowOverride=null;Policy.Configure(null);
            customer.enabled=true;account.enabled=true;typeof(Match3DInvitations).GetProperty("State").SetValue(account,oldState);typeof(Match3DInvitations).GetField("token",Flags).SetValue(account,oldToken);
            PlayerPrefs.DeleteKey(toolKey);PlayerPrefs.DeleteKey(reviveKey);PlayerPrefs.Save();
        }
    }
    static void Capture(string name){
        var camera=Camera.main;var canvases=UnityEngine.Object.FindObjectsOfType<Canvas>().Where(x=>x.isRootCanvas&&x.gameObject.activeInHierarchy).ToArray();
        var modes=canvases.Select(x=>x.renderMode).ToArray();var cameras=canvases.Select(x=>x.worldCamera).ToArray();var distances=canvases.Select(x=>x.planeDistance).ToArray();
        var rt=new RenderTexture(720,1280,24);rt.Create();var old=camera.targetTexture;var active=RenderTexture.active;camera.targetTexture=rt;
        for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=RenderMode.ScreenSpaceCamera;canvases[i].worldCamera=camera;canvases[i].planeDistance=1;}
        Canvas.ForceUpdateCanvases();foreach(var fit in UnityEngine.Object.FindObjectsOfType<Match3DUIFitContent>())fit.Fit();Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=rt;
        var texture=new Texture2D(720,1280,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,720,1280),0,0);texture.Apply();File.WriteAllBytes(Path.GetDirectoryName(Output)+"/"+name+".png",texture.EncodeToPNG());
        camera.targetTexture=old;RenderTexture.active=active;for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];canvases[i].planeDistance=distances[i];}
        UnityEngine.Object.Destroy(texture);rt.Release();UnityEngine.Object.Destroy(rt);
    }
}
