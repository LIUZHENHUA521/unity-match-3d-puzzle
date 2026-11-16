using UnityEngine;

public partial class Match3DFlow {
    bool potReplayReturnToGlory;
    public void ShowPotGlory(){
        if(PotReplayPlayer.Active)return;
        CancelPotReplayDownloadTip();BlocksGameplay=true;
        var view=OpenModal("PotGlory");
        view.GetComponent<PotGloryView>().Connect(this,()=>{CancelPotReplayDownloadTip();CloseModal();});
    }
    public void ShowPotCollectedCertificate(PotCertificateData data){
        CancelPotReplayDownloadTip();BlocksGameplay=true;
        // A collection preview is separate from the settlement state/claim callbacks.
        var view=OpenModal("PotCertificate").GetComponent<PotCertificateView>();
        view.Configure(data,ShowPotGlory,ShowPotGlory);
        view.GuardCollection(Match3DInvitations.Instance.State.inviteCode,()=>{CloseModal();Notice("账号已变化，请重新打开我的荣耀");});
        view.status.text="已珍藏的通关奖状";view.buttonLabel.text="返回我的荣耀";
        view.home.GetComponentInChildren<UnityEngine.UI.Text>().text="返回我的荣耀";
    }
    public void OpenPotGloryReplay(string id,System.Func<bool> current,System.Action<string> failed){
        potReplayReturnToGlory=true;OpenPotReplay(id,current,failed);
    }
    public void ShowPotGloryRecordings(bool masters){
        CancelPotReplayDownloadTip();CloseModal();potReplayReturnToGlory=true;ShowPotReplays(masters);
    }
}
