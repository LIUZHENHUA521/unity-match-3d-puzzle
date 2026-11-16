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
        var config=new Policy.Configuration{adsBeforeShare=3};Policy.Configure(config);
        float deadline=Time.realtimeSinceStartup+45;
        while((!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)&&Time.realtimeSinceStartup<deadline)yield return null;
        var flow=Match3DFlow.Instance;Check(flow&&flow.StartupComplete,"scene startup complete");
        var account=Match3DInvitations.Instance;account.StopAllCoroutines();account.enabled=false;var records=Match3DGameRecords.Instance;records.StopAllCoroutines();typeof(Match3DGameRecords).GetProperty("Instance").SetValue(null,null);
        var customer=UnityEngine.Object.FindObjectOfType<Match3DCustomerService>();customer.StopAllCoroutines();customer.enabled=false;
        var oldState=account.State;object oldToken=typeof(Match3DInvitations).GetField("token",Flags).GetValue(account);
        Policy.SetEnvironment("pot-ui-validation-"+Guid.NewGuid().ToString("N"));
        string owner="local";
        typeof(Match3DInvitations).GetProperty("State").SetValue(account,new Match3DInvitations.Reply{inviteCode=owner,inventory=new int[4]});
        typeof(Match3DInvitations).GetField("token",Flags).SetValue(account,null);
        string toolKey=Policy.CounterKey(Policy.Entry.Tool,owner),reviveKey=Policy.CounterKey(Policy.Entry.Revive,owner);
        Action<bool,string> share=null,ad=null;int shares=0,ads=0;
        Match3DRewardedVideo.EditorShareOverride=done=>{shares++;share=done;};
        Match3DRewardedVideo.EditorShowOverride=done=>{ads++;ad=done;};
        try{
            PlayerPrefs.DeleteKey(toolKey);Setup();yield return new WaitForSecondsRealtime(.7f);
            Tool("AddTime").onClick.Invoke();yield return new WaitUntil(()=>View&&View.group.interactable);
            Check(Label=="看广告并使用"&&Time.timeScale==0,"first tool offers ad and pauses");Capture("pot-tool-ad");
            float before=GameManager.Instance.currentLevelTime;
            View.revive.onClick.Invoke();View.revive.onClick.Invoke();Check(ads==1&&shares==0,"duplicate clicks start one ad: ads="+ads+", shares="+shares+", status="+View.status.text+", ready="+account.Ready+", screen="+flow.ScreenName+", modal="+typeof(Match3DFlow).GetField("modal",Flags).GetValue(flow)+", view="+View.transform);
            ad(false,"未看完");Check(View.group.interactable&&PlayerPrefs.GetInt(toolKey)==0&&GameManager.Instance.currentLevelTime==before,"failed ad preserves state");
            View.revive.onClick.Invoke();ad(true,null);ad(true,null);
            Check(PlayerPrefs.GetInt(toolKey)==1&&GameManager.Instance.currentLevelTime==before+180&&BoardTools.AdUses(3)==1,"completed ad grants once");
            Setup();yield return new WaitForSecondsRealtime(.7f);
            var input=InputController.Instance;
            foreach(var obj in LevelGenerator.Instance.objectsOnScene.GroupBy(x=>x.name).SelectMany(g=>g.Take(2)).Take(7).ToArray())input.AddObjectOnStack(obj,false);
            yield return new WaitUntil(()=>View&&View.group.interactable);
            Check(Label=="看广告复活","revival is ad");View.revive.onClick.Invoke();ad(true,null);ad(true,null);
            Check(flow.ClearReviveUsed&&input.objectsOnStack.Count==0&&PlayerPrefs.GetInt(toolKey)==2&&flow.ReviveUsage.clear.ad==1,"revive contributes to shared counter once");
            Setup();yield return new WaitForSecondsRealtime(.7f);
            Tool("AddTime").onClick.Invoke();yield return new WaitUntil(()=>View&&View.group.interactable);View.revive.onClick.Invoke();ad(true,null);
            Check(PlayerPrefs.GetInt(toolKey)==3&&Policy.Next(Policy.Entry.Tool).route==Policy.Route.Share,"third ad unlocks share");
            Setup();yield return new WaitForSecondsRealtime(.7f);
            foreach(var obj in LevelGenerator.Instance.objectsOnScene.GroupBy(x=>x.name).SelectMany(g=>g.Take(2)).Take(7).ToArray())input.AddObjectOnStack(obj,false);
            yield return new WaitUntil(()=>View&&View.group.interactable);
            Check(Label=="看广告复活","pending share never replaces revival ad");Capture("pot-revive-ad");View.revive.onClick.Invoke();ad(true,null);
            Check(PlayerPrefs.GetInt(toolKey)==4&&Policy.Next(Policy.Entry.Tool).route==Policy.Route.Share,"revival preserves share opportunity");
            Setup();yield return new WaitForSecondsRealtime(.7f);
            Tool("Shuffle").onClick.Invoke();yield return new WaitUntil(()=>View&&View.group.interactable);
            Check(Label=="分享并使用","next tool offers share");Capture("pot-tool-share");
            View.revive.onClick.Invoke();share(false,"返回过快");Check(PlayerPrefs.GetInt(toolKey)==4,"failed share keeps opportunity");
            View.revive.onClick.Invoke();share(true,null);share(true,null);
            Check(PlayerPrefs.GetInt(toolKey)==0&&BoardTools.ShareUses(2)==1&&Time.timeScale==1,"share consumes opportunity once");
            Setup();yield return new WaitForSecondsRealtime(.7f);
            Tool("AddTime").onClick.Invoke();yield return new WaitUntil(()=>View&&View.group.interactable);View.revive.onClick.Invoke();var stale=ad;
            Setup();yield return new WaitForSecondsRealtime(.7f);stale(true,null);Check(PlayerPrefs.GetInt(toolKey)==0&&BoardTools.UsedCount(3)==0,"stale callback cannot grant on new board");
        }finally{
            flow.Home();Match3DRewardedVideo.EditorShareOverride=null;Match3DRewardedVideo.EditorShowOverride=null;Policy.Configure(null);
            typeof(Match3DGameRecords).GetProperty("Instance").SetValue(null,records);customer.enabled=true;account.enabled=true;typeof(Match3DInvitations).GetProperty("State").SetValue(account,oldState);typeof(Match3DInvitations).GetField("token",Flags).SetValue(account,oldToken);
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
