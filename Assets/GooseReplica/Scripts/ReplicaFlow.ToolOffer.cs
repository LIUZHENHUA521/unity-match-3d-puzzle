using System;
using UnityEngine;

public partial class ReplicaFlow {
    ReplicaRewardedVideo toolAds;
    int toolOfferVersion;
    void ResetToolOffer(){toolOfferVersion++;if(toolAds)toolAds.Cancel();}
    void ShowToolOffer(int type,Action action){
        ResetToolOffer();int version=toolOfferVersion,boardVersion=creationVersion;
        float speed=Time.timeScale;Time.timeScale=0;BlocksGameplay=true;
        InputController.Instance.CancelSelection();
        var view=OpenModal("ReviveClear").GetComponent<ReplicaReviveView>();
        bool Current()=>version==toolOfferVersion&&ScreenName=="game"&&creationVersion==boardVersion&&view&&modal==view.transform;
        void Close(){if(!Current())return;ResetToolOffer();CloseModal();Time.timeScale=speed;}
        view.ConfigureTool(type,()=>{
            if(!Current())return;
            if(!toolAds){var host=new GameObject("Goose Tool Ads");host.transform.SetParent(transform,false);toolAds=host.AddComponent<ReplicaRewardedVideo>();}
            view.ShowAdStatus("正在加载视频…");
            toolAds.ShowTool((completed,error)=>{
                if(!Current())return;
                if(!completed){view.AllowRetry(string.IsNullOrEmpty(error)?"完整观看视频后才能使用道具":error);return;}
                Close();
                string reason=ToolUnavailableReason(type);
                if(reason!=null){ShowTips(reason);return;}
                bool applied=ReplicaSourceTools.RunPaid(type,()=>{BoosterManager.Instance.CheckHintInteractibility();action();});
                if(applied)ReplicaSourceTools.RecordAdUse(type);
                else ShowTips("当前无法使用该道具");
            });
        },Close);
    }
}
