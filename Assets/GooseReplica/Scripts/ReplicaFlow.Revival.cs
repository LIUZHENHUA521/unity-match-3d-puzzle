using UnityEngine;

public partial class ReplicaFlow {
    public const int ReviveAddedSeconds=300;
    bool revivePending,reviveTimeUsed,reviveClearUsed;
    int reviveVersion;
    ReplicaRewardedVideo reviveAds;
    public bool RevivePending=>revivePending;
    public bool TimeReviveUsed=>reviveTimeUsed;
    public bool ClearReviveUsed=>reviveClearUsed;
    void ResetRevival(){if(reviveAds)reviveAds.Cancel();reviveVersion++;revivePending=false;reviveTimeUsed=false;reviveClearUsed=false;}
    bool TryOfferRevival(){
        if(IsPvp||!InputController.Instance||!InputController.Instance.UsesSourceFlight)return false;
        bool timeout=Stage!=1&&GameManager.Instance.currentLevelTime<=0;
        if(timeout?reviveTimeUsed:reviveClearUsed)return false;
        if(!timeout&&InputController.Instance.objectsOnStack.Count<7)return false;
        revivePending=true;BlocksGameplay=true;GameManager.Instance.gameStatus=GameManager.GameStatus.FAIL;
        InputController.Instance.CancelSelection();Time.timeScale=0;
        int version=++reviveVersion;
        float progress=Mathf.Clamp01(1f-LevelGenerator.Instance.objectsOnScene.Count/(float)Mathf.Max(1,InitialCount));
        var view=OpenModal(timeout?"ReviveTime":"ReviveClear").GetComponent<ReplicaReviveView>();
        view.Configure(progress,()=>WatchRevivalVideo(timeout,version,view),()=>DeclineRevival(version));
        return true;
    }
    void WatchRevivalVideo(bool timeout,int version,ReplicaReviveView view){
        if(!revivePending||version!=reviveVersion||!view)return;
        if(!reviveAds){var host=new GameObject("Goose Revival Ads");host.transform.SetParent(transform,false);reviveAds=host.AddComponent<ReplicaRewardedVideo>();}
        view.ShowAdStatus("正在加载复活视频…");
        reviveAds.Show(timeout,(completed,error)=>{
            if(!revivePending||version!=reviveVersion||!view)return;
            if(completed)AcceptRevival(timeout,version);
            else view.AllowRetry(string.IsNullOrEmpty(error)?"完整观看视频后才能复活":error);
        });
    }
    void AcceptRevival(bool timeout,int version){
        if(!revivePending||version!=reviveVersion||ScreenName!="game")return;
        if(timeout){
            if(reviveTimeUsed)return;
            reviveTimeUsed=true;GameManager.Instance.currentLevelTime=Mathf.Max(0,GameManager.Instance.currentLevelTime)+ReviveAddedSeconds;
        }else{
            if(reviveClearUsed||!InputController.Instance.ReviveSourceTray())return;
            reviveClearUsed=true;
        }
        revivePending=false;reviveVersion++;
        CloseModal(true);GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;
        BoosterManager.Instance.BoostersInteractibility(true);BoosterManager.Instance.CheckHintInteractibility();
        // A timeout can occur with a full tray in the same frame; resolve the other cause separately.
        if(InputController.Instance.objectsOnStack.Count>=7)HandleResult(false);
    }
    void DeclineRevival(int version){
        if(!revivePending||version!=reviveVersion||ScreenName!="game")return;
        revivePending=false;reviveVersion++;Time.timeScale=1;
        FinishSoloResult(false);
    }
}
