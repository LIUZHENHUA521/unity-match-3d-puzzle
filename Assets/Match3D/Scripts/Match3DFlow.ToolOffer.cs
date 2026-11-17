using System;
using UnityEngine;

public partial class Match3DFlow {
    Match3DRewardedVideo toolAds;
    int toolOfferVersion;
    void ResetToolOffer(){toolOfferVersion++;if(toolAds)toolAds.Cancel();}
    public bool PotZoomUnlocked { get; private set; }
    public void ShowPotZoomOffer(){
        if(PotZoomUnlocked||BlocksGameplay||IsPvp||ScreenName!="game"||!GameManager.Instance.GameIsPlaying())return;
        ResetToolOffer();int version=toolOfferVersion,boardVersion=creationVersion;
        float speed=Time.timeScale;Time.timeScale=0;BlocksGameplay=true;
        if(InputController.Instance)InputController.Instance.CancelSelection();
        var view=OpenModal("ReviveClear").GetComponent<Match3DReviveView>();
        bool Current()=>version==toolOfferVersion&&ScreenName=="game"&&creationVersion==boardVersion&&view&&modal==view.transform;
        void Close(){if(!Current())return;ResetToolOffer();CloseModal();Time.timeScale=speed;}
        view.ConfigurePotZoom(()=>{
            if(!Current())return;
            if(!toolAds){var host=new GameObject("Pot Zoom Ads");host.transform.SetParent(transform,false);toolAds=host.AddComponent<Match3DRewardedVideo>();}
            view.ShowAcquisitionStatus(Match3DRewardAcquisition.Route.Ad);
            toolAds.ShowPotZoom((completed,error)=>{
                if(!Current())return;
                if(!completed){view.AllowRetry(string.IsNullOrEmpty(error)?"请完整观看视频后解锁":error);return;}
                PotZoomUnlocked=true;
                Close();
                ShowTips("本局已解锁自由缩放");
            });
        },Close);
    }
    void ShowToolOffer(int type,Action action){
        ResetToolOffer();int version=toolOfferVersion,boardVersion=creationVersion;
        float speed=Time.timeScale;Time.timeScale=0;BlocksGameplay=true;
        InputController.Instance.CancelSelection();
        var view=OpenModal("ReviveClear").GetComponent<Match3DReviveView>();
        var offer=Match3DRewardAcquisition.Next(Match3DRewardAcquisition.Entry.Tool);
        bool Current()=>version==toolOfferVersion&&ScreenName=="game"&&creationVersion==boardVersion&&view&&modal==view.transform;
        void Close(){if(!Current())return;ResetToolOffer();CloseModal();Time.timeScale=speed;}
        view.ConfigureTool(type,()=>{
            if(!Current())return;
            if(!toolAds){var host=new GameObject("Goose Tool Ads");host.transform.SetParent(transform,false);toolAds=host.AddComponent<Match3DRewardedVideo>();}
            view.ShowAcquisitionStatus(offer.route);
            toolAds.Acquire(offer,false,(completed,error)=>{
                if(!Current())return;
                if(!completed||!Match3DRewardAcquisition.IsCurrent(offer)){view.AllowRetry(string.IsNullOrEmpty(error)?"获取未完成，请重试":error);return;}
                Close();
                string reason=ToolUnavailableReason(type);
                if(reason!=null){ShowTips(reason);return;}
                bool applied=BoardTools.RunPaid(type,()=>{BoosterManager.Instance.CheckHintInteractibility();action();});
                if(applied){Match3DRewardAcquisition.Commit(offer);BoardTools.RecordAcquisition(type,offer.route);}
                else ShowTips("当前无法使用该道具");
            },type);
        },Close);
        view.SetAcquisitionRoute(offer.route);
    }
}
