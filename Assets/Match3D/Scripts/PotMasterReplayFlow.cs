using System;
using UnityEngine;

public partial class Match3DFlow {
    bool potReplayReturnToMasters;
    public void ShowPotMasterReplays(){
        if(PotReplayPlayer.Active)return;
        CancelPotReplayDownloadTip();CloseModal();BlocksGameplay=true;
        var view=PotMasterReplayView.Create(page);modal=(RectTransform)view.transform;
        view.Connect(this,()=>{CancelPotReplayDownloadTip();CloseModal();});
        Match3DModalMotion.Play(view.gameObject);
    }
    public void OpenPotMasterReplay(string id,string version,Func<bool> current,Action<string> failed,Action started){
        string date=Today;
        OpenPotReplay(id,current,failed,()=>{potReplayReturnToGlory=false;potReplayReturnToMasters=true;started?.Invoke();},
            data=>data!=null&&data.status=="win"&&data.stage==2&&data.level!=null&&data.level.tier=="bachelor"&&data.level.version==version&&data.level.date==date);
    }
}
