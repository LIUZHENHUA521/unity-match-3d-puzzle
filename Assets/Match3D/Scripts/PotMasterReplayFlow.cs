using System;
using UnityEngine;

public partial class Match3DFlow {
    bool potReplayReturnToMasters;
    public void ShowPotMasterReplays(Match3DResultView failure=null){
        if(PotReplayPlayer.Active)return;
        CancelPotReplayDownloadTip();
        string owner=PotMasterReplayAccess.Account();
        if(failure){failure.gameObject.SetActive(false);modal=null;}else CloseModal();
        BlocksGameplay=true;
        var view=PotMasterReplayView.Create(page);modal=(RectTransform)view.transform;
        view.Connect(this,()=>{
            CancelPotReplayDownloadTip();CloseModal();
            if(failure&&owner==PotMasterReplayAccess.Account()){failure.gameObject.SetActive(true);modal=(RectTransform)failure.transform;BlocksGameplay=true;}
            else if(failure){Destroy(failure.gameObject);Home();}
        });
        Match3DModalMotion.Play(view.gameObject);
    }
    public void OpenPotMasterReplay(string id,string version,Func<bool> current,Action<string> failed,Action started){
        string date=Today;
        OpenPotReplay(id,current,failed,()=>{potReplayReturnToGlory=false;potReplayReturnToMasters=true;started?.Invoke();},
            data=>data!=null&&data.status=="win"&&data.stage==2&&data.level!=null&&data.level.tier=="bachelor"&&data.level.version==version&&data.level.date==date);
    }
}
