using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad] public static class PotToolLimitValidation {
    const string Key="pot-tool-limit-validation";
    static int checks;
    static PotToolLimitValidation(){EditorApplication.playModeStateChanged+=Changed;}
    public static void Run(){SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Changed(PlayModeStateChange state){if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))EditorApplication.delayCall+=()=>new GameObject("Pot tool limit validation").AddComponent<BoardValidationDriver>().StartCoroutine(Execute());}
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);checks++;}
    static IEnumerator Execute(){
        yield return null;yield return null;
        string error=null;var core=Core();
        while(true){bool more=false;try{more=core.MoveNext();}catch(Exception e){error=e.ToString();}if(!more||error!=null)break;yield return core.Current;}
        Match3DTestPrivileges.ForceForValidation=false;Match3DRewardAcquisition.Configure(null);
        Match3DFlow.Instance.Home();
        File.WriteAllText("../result.txt",error==null?"PASS "+checks+" tool-limit runtime checks":error);
        SessionState.SetBool(Key,false);EditorApplication.Exit(error==null?0:1);
    }
    static IEnumerator Core(){
        var flow=Match3DFlow.Instance;
        Match3DTestPrivileges.ForceForValidation=true;flow.Home();flow.Begin(0);BoardCreationTestSupport.FinishPending(flow);yield return null;yield return null;
        var input=InputController.Instance;
        Check(!BoardTools.RunPaid(0,()=>input.MoveSourceItemsToStore())&&BoardTools.UsedCount(0)==0,"empty tray consumes no use");
        Check(!BoardTools.RunPaid(1,()=>BoosterManager.Instance.HintButton())&&BoardTools.UsedCount(1)==0,"empty magnet consumes no use");
        Check(!BoardTools.RunPaid(3,()=>BoardTools.AddTime())&&BoardTools.UsedCount(3)==0,"untimed stage consumes no use");
        Check(!BoardTools.RunPaid(-1,()=>{throw new Exception("invalid callback");}),"invalid type rejected");
        var hud=UnityEngine.Object.FindObjectOfType<Match3DGameplayHUD>();var view=hud.GetComponent<Match3DUIView>();
        string[] keys={"Undo","Hint","Shuffle","AddTime"};
        for(int type=0;type<4;type++){
            int t=type;var background=view.Get<Image>(keys[t]).sprite;
            Check(BoardTools.HasUseRemaining(t),"initial quota");
            Check(!BoardTools.TrySpend(t),"direct spend rejected");
            Check(BoardTools.RunPaid(t,()=>{Check(BoardTools.TrySpend(t),"first use succeeds");Check(!BoardTools.TrySpend(t),"duplicate spend rejected");}),"paid use succeeds");
            bool invoked=false;Check(!BoardTools.RunPaid(t,()=>invoked=true)&&!invoked,"repeat callback never runs");
            Check(!BoardTools.CanUse(t)&&BoardTools.UsedCount(t)==1,"test privilege cannot bypass quota");
            Check(flow.ToolUnavailableReason(t).Contains("每关限用一次"),"limit reason");
            view.Get<Button>(keys[t]).onClick.Invoke();Check(!flow.BlocksGameplay,"repeat click opens no acquisition");
            yield return null;yield return null;
            Check(hud.quantities[t].text=="0"&&view.Get<Image>(keys[t]).sprite!=background,"used badge and background");
        }
        Check(input.ReviveSourceTray(),"empty tray revival succeeds");
        for(int t=0;t<4;t++)Check(BoardTools.UsedCount(t)==1,"revival preserves spent quota");
        flow.NextStage();BoardCreationTestSupport.FinishPending(flow);yield return null;
        for(int t=0;t<4;t++)Check(BoardTools.HasUseRemaining(t),"next stage resets every tool");
        var game=GameManager.Instance;game.currentLevelTime=100;
        Check(BoardTools.RunPaid(3,()=>BoardTools.AddTime())&&game.currentLevelTime==280,"add time grants 180 seconds");
        Check(!BoardTools.RunPaid(3,()=>BoardTools.AddTime())&&game.currentLevelTime==280,"second add time rejected");
        flow.RestartStage();BoardCreationTestSupport.FinishPending(flow);yield return null;
        for(int t=0;t<4;t++)Check(BoardTools.HasUseRemaining(t),"restart resets every tool");
        // Ordinary account uses the same paid gate as inventory and ad/share acquisition.
        flow.Home();Match3DTestPrivileges.ForceForValidation=false;flow.Begin(0);BoardCreationTestSupport.FinishPending(flow);yield return null;
        for(int t=0;t<4;t++){
            int type=t;Check(BoardTools.RunPaid(type,()=>BoardTools.TrySpend(type)),"ordinary first authorized use");
            Check(!BoardTools.RunPaid(type,()=>BoardTools.TrySpend(type)),"ordinary repeat rejected");
        }
    }
}
