using UnityEngine;

public partial class Match3DFlow {
    public const int ReviveAddedSeconds=300;
    bool revivePending,reviveTimeUsed,reviveClearUsed;
    int reviveVersion;
    Match3DRewardedVideo reviveAds;
    public bool RevivePending=>revivePending;
    public bool TimeReviveUsed=>reviveTimeUsed;
    public bool ClearReviveUsed=>reviveClearUsed;
    public Match3DGameRecords.ReviveUse ReviveUsage {get;private set;}=new Match3DGameRecords.ReviveUse();
    void ResetRevival(){if(reviveAds)reviveAds.Cancel();reviveVersion++;revivePending=false;reviveTimeUsed=false;reviveClearUsed=false;ReviveUsage=new Match3DGameRecords.ReviveUse();}
    bool TryOfferRevival(){
        if(IsPvp||!InputController.Instance||!InputController.Instance.UsesSourceFlight)return false;
        bool timeout=Stage!=1&&GameManager.Instance.currentLevelTime<=0;
        if(timeout?reviveTimeUsed:reviveClearUsed)return false;
        if(!timeout&&InputController.Instance.objectsOnStack.Count<7)return false;
        revivePending=true;BlocksGameplay=true;GameManager.Instance.gameStatus=GameManager.GameStatus.FAIL;
        InputController.Instance.CancelSelection();Time.timeScale=0;
        int version=++reviveVersion;
        float progress=DisplayProgress();
        var view=OpenModal(timeout?"ReviveTime":"ReviveClear").GetComponent<Match3DReviveView>();
        var offer=Match3DRewardAcquisition.Next(Match3DRewardAcquisition.Entry.Revive);
        view.Configure(progress,()=>WatchRevivalVideo(timeout,version,view,offer),()=>DeclineRevival(version));
        view.DescribeReturnRevival(timeout);
        view.SetAcquisitionRoute(offer.route);
        return true;
    }
    void WatchRevivalVideo(bool timeout,int version,Match3DReviveView view,Match3DRewardAcquisition.Offer offer){
        if(!revivePending||version!=reviveVersion||!view)return;
        if(!reviveAds){var host=new GameObject("Goose Revival Ads");host.transform.SetParent(transform,false);reviveAds=host.AddComponent<Match3DRewardedVideo>();}
        view.ShowAcquisitionStatus(offer.route);
        reviveAds.Acquire(offer,timeout,(completed,error)=>{
            if(!revivePending||version!=reviveVersion||!view)return;
            if(completed&&Match3DRewardAcquisition.IsCurrent(offer))AcceptRevival(timeout,version,offer);
            else view.AllowRetry(string.IsNullOrEmpty(error)?"获取未完成，请重试":error);
        });
    }
    void AcceptRevival(bool timeout,int version,Match3DRewardAcquisition.Offer offer){
        if(!revivePending||version!=reviveVersion||ScreenName!="game")return;
        if(timeout?reviveTimeUsed:reviveClearUsed)return;
        if(!InputController.Instance.ReviveSourceTray())return;
        if(timeout){
            reviveTimeUsed=true;GameManager.Instance.currentLevelTime=Mathf.Max(0,GameManager.Instance.currentLevelTime)+ReviveAddedSeconds;
        }else{
            reviveClearUsed=true;
        }
        Match3DRewardAcquisition.Commit(offer);
        PotReplayRecorder.Event("revive",-1,timeout?1:0);
        var usage=timeout?ReviveUsage.time:ReviveUsage.clear;
        if(offer.route==Match3DRewardAcquisition.Route.Share)usage.share++;
        else if(offer.route==Match3DRewardAcquisition.Route.Ad)usage.ad++;
        else if(offer.route==Match3DRewardAcquisition.Route.Free)usage.free++;
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
