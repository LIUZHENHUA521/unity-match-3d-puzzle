using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class AddTimeToolValidation {
    const string Marker="Temp/run-add-time-check",Result="Temp/add-time-validation.txt";
    static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
    static AddTimeToolValidation(){EditorApplication.update+=Poll;}
    static void Poll(){
        if(!File.Exists(Marker)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
        if(!EditorApplication.isPlayingOrWillChangePlaymode&&!EditorApplication.isPlaying){AddTimeToolBuilder.Build();EditorApplication.isPlaying=true;return;}
        if(!Application.isPlaying||!Match3DFlow.Instance||!Match3DInvitations.Instance||!Match3DFlow.Instance.GetComponent<LayeredItemPile>()||!UnityEngine.Object.FindObjectOfType<Match3DHomeView>())return;
        File.Delete(Marker);new GameObject("Add-time validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
    }
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);File.AppendAllText(Result,"PASS "+message+"\n");}
    static IEnumerator Guard(){
        Match3DRewardAcquisition.Configure(new Match3DRewardAcquisition.Configuration{sharesEnabled=false});
        File.WriteAllText(Result,"");var service=Match3DInvitations.Instance;
        bool created=!service;
        if(created){service=new GameObject("Local inventory fixture").AddComponent<Match3DInvitations>();service.Initialize("http://127.0.0.1:18765");}
        var token=typeof(Match3DInvitations).GetField("token",Flags).GetValue(service);
        var endpoint=typeof(Match3DInvitations).GetField("endpoint",Flags).GetValue(service);var state=service.State;
        var steps=Run();string error=null;
        while(true){bool more=false;try{more=steps.MoveNext();}catch(Exception e){error=e.ToString();}if(error!=null||!more)break;yield return steps.Current;}
        typeof(Match3DInvitations).GetField("token",Flags).SetValue(service,token);typeof(Match3DInvitations).GetField("endpoint",Flags).SetValue(service,endpoint);typeof(Match3DInvitations).GetProperty("State").SetValue(service,state);
        Match3DRewardedVideo.EditorShowOverride=null;Match3DRewardAcquisition.Configure(null);
        File.AppendAllText(Result,error==null?"COMPLETE\n":"FAIL "+error);
        Match3DFlow.Instance.Home();
        if(created)UnityEngine.Object.Destroy(service.gameObject);
        EditorApplication.isPlaying=false;
    }
    static Button Button()=>UnityEngine.Object.FindObjectOfType<Match3DGameplayHUD>().GetComponent<Match3DUIView>().Get<Button>("AddTime");
    static void Setup(){var f=Match3DFlow.Instance;f.Home();f.Begin(0);f.NextStage();BoardCreationTestSupport.FinishPending(f);typeof(Match3DFlow).GetProperty("LocalPreview").SetValue(f,true);GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;}
    static IEnumerator Run(){
        Setup();yield return null;var f=Match3DFlow.Instance;var game=GameManager.Instance;
        Check(!f.UnlimitedTools,"fixture uses ordinary paid tools");
        var hud=UnityEngine.Object.FindObjectOfType<Match3DGameplayHUD>();Check(hud.quantities.Length==4,"four inventory counters");
        game.currentLevelTime=100;
        Check(!BoardTools.AddTime()&&game.currentLevelTime==100,"direct use cannot bypass payment");
        Check(BoardTools.RunPaid(3,()=>BoardTools.AddTime())&&game.currentLevelTime==280,"paid add-time grants exactly 180 seconds");
        Check(BoardTools.RunPaid(3,()=>BoardTools.AddTime())&&game.currentLevelTime==460,"repeated use adds another 180 seconds");
        Check(!f.TimeReviveUsed,"add-time does not consume revival");
        yield return new WaitForSecondsRealtime(.8f);Capture("toolbar");
        Action<bool,string> reply=null;Match3DRewardedVideo.EditorShowOverride=done=>reply=done;
        Button().onClick.Invoke();yield return new WaitForSecondsRealtime(.6f);
        var popup=UnityEngine.Object.FindObjectOfType<Match3DReviveView>();Check(popup&&popup.progress.text=="加时道具"&&Time.timeScale==0,"zero stock opens paused add-time offer");Capture("offer");
        float before=game.currentLevelTime;popup.revive.onClick.Invoke();reply(false,"未完整观看");Check(game.currentLevelTime==before&&popup.group.interactable,"incomplete advertisement grants no time");
        popup.revive.onClick.Invoke();var completed=reply;completed(true,null);completed(true,null);
        Check(Mathf.Abs(game.currentLevelTime-(before+180))<.001f&&BoardTools.AdUses(3)==1&&Time.timeScale==1,"duplicate ad callbacks grant exactly once: time="+game.currentLevelTime+", expected="+(before+180)+", ads="+BoardTools.AdUses(3)+", scale="+Time.timeScale);
        yield return null;
        Button().onClick.Invoke();yield return new WaitForSecondsRealtime(.6f);popup=UnityEngine.Object.FindObjectOfType<Match3DReviveView>();before=game.currentLevelTime;popup.close.onClick.Invoke();Check(game.currentLevelTime==before&&Time.timeScale==1,"close preserves time and resumes play");
        yield return null;Button().onClick.Invoke();yield return new WaitForSecondsRealtime(.6f);UnityEngine.Object.FindObjectOfType<Match3DReviveView>().revive.onClick.Invoke();var stale=reply;
        Setup();yield return null;before=game.currentLevelTime;stale(true,null);Check(game.currentLevelTime==before&&BoardTools.UsedCount(3)==0,"stale callback cannot change a new board");
        var service=Match3DInvitations.Instance;
        typeof(Match3DInvitations).GetField("token",Flags).SetValue(service,"local-add-time-test");typeof(Match3DInvitations).GetField("endpoint",Flags).SetValue(service,"http://127.0.0.1:18765");
        typeof(Match3DInvitations).GetProperty("State").SetValue(service,new Match3DInvitations.Reply{inventory=new[]{0,0,0,2},inviteCode="add-time-test"});
        int ads=0;Match3DRewardedVideo.EditorShowOverride=done=>ads++;before=game.currentLevelTime;Button().onClick.Invoke();
        float deadline=Time.realtimeSinceStartup+8;while(service.Busy&&Time.realtimeSinceStartup<deadline)yield return null;
        Check(!service.Busy&&service.Inventory(3)==1&&BoardTools.UsedCount(3)==1&&ads==0&&Mathf.Abs(game.currentLevelTime-before-180)<1,"inventory grants 180 seconds and consumes one without ads");
        // A server rejection must not grant the local effect, and its refund settles the pending request.
        before=game.currentLevelTime;Button().onClick.Invoke();deadline=Time.realtimeSinceStartup+8;while(service.Busy&&Time.realtimeSinceStartup<deadline)yield return null;
        Check(!service.Busy&&!service.HasPending&&BoardTools.UsedCount(3)==1&&game.currentLevelTime<=before&&ads==0,"failed debit grants no time and settles pending request");
        game.currentLevelTime=0;f.HandleResult(false);yield return new WaitForSecondsRealtime(.6f);Match3DRewardedVideo.EditorShowOverride=done=>done(true,null);
        UnityEngine.Object.FindObjectOfType<Match3DReviveView>().revive.onClick.Invoke();Check(f.TimeReviveUsed&&game.currentLevelTime==300,"timeout revival independently grants original 300 seconds");
        Check(BoardTools.RunPaid(3,()=>BoardTools.AddTime())&&game.currentLevelTime==480,"add-time remains usable after revival");
        var records=Match3DGameRecords.Instance;
        var record=(Match3DGameRecords.Record)typeof(Match3DGameRecords).GetField("active",Flags).GetValue(records);
        f.Home();Check(record!=null&&record.timeReviveUsed&&record.remainingSeconds==480&&record.toolUsage.Length==4&&record.toolUsage[3].inventory==2,"quit records keep extended time, fourth tool and revival before reset");
        Setup();yield return null;
        game.gameStatus=GameManager.GameStatus.FAIL;Check(!BoardTools.RunPaid(3,()=>BoardTools.AddTime()),"finished board rejects add-time");
        f.Home();f.Begin(0);BoardCreationTestSupport.FinishPending(f);game.gameStatus=GameManager.GameStatus.PLAY;
        Check(f.ToolUnavailableReason(3)!=null&&!BoardTools.RunPaid(3,()=>BoardTools.AddTime())&&BoardTools.UsedCount(3)==0,"untimed first stage never consumes add-time");
    }
    static void Capture(string name){
        var camera=Camera.main;var canvases=UnityEngine.Object.FindObjectsOfType<Canvas>().Where(c=>c.isRootCanvas&&c.gameObject.activeInHierarchy).ToArray();
        var modes=canvases.Select(c=>c.renderMode).ToArray();var cameras=canvases.Select(c=>c.worldCamera).ToArray();var distances=canvases.Select(c=>c.planeDistance).ToArray();
        var rt=new RenderTexture(720,1280,24);rt.Create();var old=camera.targetTexture;var active=RenderTexture.active;camera.targetTexture=rt;
        for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=RenderMode.ScreenSpaceCamera;canvases[i].worldCamera=camera;canvases[i].planeDistance=1;}
        Canvas.ForceUpdateCanvases();foreach(var fit in UnityEngine.Object.FindObjectsOfType<Match3DUIFitContent>())fit.Fit();Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=rt;
        var texture=new Texture2D(720,1280,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,720,1280),0,0);texture.Apply();File.WriteAllBytes("Temp/add-time-"+name+".png",texture.EncodeToPNG());camera.targetTexture=old;RenderTexture.active=active;
        for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];canvases[i].planeDistance=distances[i];}UnityEngine.Object.Destroy(texture);rt.Release();UnityEngine.Object.Destroy(rt);
    }
}
