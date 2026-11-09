using System;
using System.Collections;
using System.Reflection;
using System.IO;
using UnityEngine;

public static class LocalPreviewValidation {
    public static void Start(){var driver=new GameObject("Local preview validation").AddComponent<BoardValidationDriver>();driver.StartCoroutine(Run(driver));}
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    static IEnumerator Run(BoardValidationDriver driver){
        var steps=Core();string failure=null;
        while(true){bool next=false;try{next=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(failure!=null||!next)break;yield return steps.Current;}
        File.WriteAllText("Temp/goose-local-preview-validation.txt",failure==null?"PASS: both home entries, playable boards, second stage, retry, free tools, simulated revival, no queued records.":"FAIL: "+failure);
        Match3DFlow.Instance.Home();UnityEngine.Object.Destroy(driver.gameObject);
    }
    static IEnumerator Core(){
        var flow=Match3DFlow.Instance;
        Check(!Match3DInvitations.Instance.Ready,"fixture must be logged out");
        foreach(var tier in new[]{"bachelor","elite"}){
            flow.Home();flow.BeginReward(0,tier);
            Check(flow.LocalPreview&&flow.UnlimitedTools&&flow.ScreenName=="game","home entry starts local preview");
            float deadline=Time.realtimeSinceStartup+45;
            while(flow.CreationInProgress&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(!flow.CreationInProgress&&GameManager.Instance.GameIsPlaying()&&flow.InitialCount>0,"first stage playable");
            Check(BoardTools.CanUse(0)&&BoardTools.CanUse(1)&&BoardTools.CanUse(2),"tools available without inventory");
            flow.NextStage();deadline=Time.realtimeSinceStartup+60;
            while(flow.CreationInProgress&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(flow.Stage==2&&GameManager.Instance.GameIsPlaying(),"second stage playable");
            flow.RestartStage();deadline=Time.realtimeSinceStartup+60;
            while(flow.CreationInProgress&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(flow.LocalPreview&&GameManager.Instance.GameIsPlaying(),"retry preserves preview");
            var ad=flow.gameObject.AddComponent<Match3DRewardedVideo>();bool completed=false;ad.Show(false,(ok,error)=>completed=ok);Check(completed,"preview revival completes");UnityEngine.Object.Destroy(ad);
            flow.Home();
            var unbound=(IList)typeof(Match3DGameRecords).GetField("unbound",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(Match3DGameRecords.Instance);
            Check(unbound.Count==0,"preview records must not await account binding");
        }
    }
}
