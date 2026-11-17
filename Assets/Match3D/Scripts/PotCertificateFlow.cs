using System;
using UnityEngine;

public partial class Match3DFlow {
    public void PlayPotCertificateSound(AudioClip clip){if(SettingsEffects&&clip)effects.PlayOneShot(clip);}
    PotCertificateView potCertificate;
    string potCertificateOwner,potCertificateRun,potCertificateRecord,potVoucher,potSettlementError;
    bool potSettled,potRewardsReleased,potCertificatePreview,potUploading,potCertificateVerified;
    float potUploadStarted,potUploadNextTick;
    int potUploadScreen,potUploadDots;
    PotCertificateData potPendingCertificate;
    internal bool PotCertificateBlocksRewards=>(potUploading||PotCertificateShowing)&&!potRewardsReleased;
    internal bool PotCertificateShowing=>potCertificate&&potCertificate.gameObject.activeInHierarchy;
    bool ShowPotCertificate(){
        var records=Match3DGameRecords.Instance;var record=records?records.LastFinished:null;
        if(record==null||record.stage!=2||record.status!="win")return false;
        potCertificatePreview=LocalPreview&&dailyLevel!=null;
        if(string.IsNullOrEmpty(record.rewardTier)&&!potCertificatePreview)return false;
        potCertificateOwner=records.LastFinishedOwner;potCertificateRun=record.runId;potCertificateRecord=record.recordId;
        potSettled=false;potCertificateVerified=false;potRewardsReleased=false;potVoucher=null;potSettlementError=null;
        potCertificate=OpenModal("PotCertificate").GetComponent<PotCertificateView>();
        var data=new PotCertificateData{runId=record.runId,recordId=record.recordId,rewardTier=potCertificatePreview?dailyLevel.tier:record.rewardTier,
            dateKey=DateTimeOffset.FromUnixTimeMilliseconds(record.endedAt).ToOffset(TimeSpan.FromHours(8)).ToString("yyyy-MM-dd"),activeDurationMs=record.activeDurationMs};
        potPendingCertificate=data;
        potUploading=!potCertificatePreview;
        potUploadStarted=Time.realtimeSinceStartup;potUploadNextTick=0;potUploadDots=0;potUploadScreen=ScreenVersion;
        if(potUploading)potCertificate.gameObject.SetActive(false);
        else potCertificate.Configure(data,ContinuePotCertificate);
        if(potCertificatePreview){potCertificate.title.text="一锅消不完 · 奖状预览";potCertificate.number.text="预览";potCertificate.Message("编辑器预览，不发放红包",true);}
        return true;
    }
    void CheckPotCertificateOwner(){
        var account=Match3DInvitations.Instance;
        if(potUploading){
            if(ScreenVersion!=potUploadScreen||!potCertificate){potUploading=false;if(tips)tips.Hide();return;}
            if(account&&account.Ready&&account.State.inviteCode!=potCertificateOwner){potUploading=false;Home();Notice("账号已变化，请重新挑战");return;}
            var replay=PotReplayLocalState.Get(potCertificateOwner,potCertificateRecord);
            if(potSettled&&replay!=null&&replay.state=="synced"){
                potUploading=false;if(tips)tips.Hide();potCertificate.gameObject.SetActive(true);
                potCertificate.Configure(potPendingCertificate,ContinuePotCertificate);
                ApplyPotSettlement();
            }else if((replay!=null&&replay.state=="failed")||Time.realtimeSinceStartup-potUploadStarted>180||potSettlementError=="settlement_rejected"){
                potUploading=false;if(tips)tips.Hide();Home();Notice("同步尚未完成，请稍后在我的荣耀和红包记录中查看");
            }else if(Time.realtimeSinceStartup>=potUploadNextTick){
                potUploadNextTick=Time.realtimeSinceStartup+.45f;potUploadDots=potUploadDots%3+1;
                if(!tips)tips=Match3DTips.Create(canvas.transform);tips.Show("上传中"+new string('。',potUploadDots),true);
            }
        }
        if(PotCertificateShowing&&!potCertificatePreview&&account&&account.State!=null&&account.State.inviteCode!=potCertificateOwner){CloseModal();potCertificate=null;Notice("账号已变化，请返回首页重新挑战");}
    }
    bool PotOwnerMatches(string owner){var account=Match3DInvitations.Instance;return account&&account.Ready&&account.State.inviteCode==owner;}
    public bool ReceivePotSettlement(string owner,string run,string record,PotCertificateData certificate,string voucher,string error){
        if(potCertificatePreview||(!potUploading&&!PotCertificateShowing)||owner!=potCertificateOwner||run!=potCertificateRun||record!=potCertificateRecord||!PotOwnerMatches(owner))return false;
        potSettled=true;potVoucher=voucher;potSettlementError=error;
        potCertificateVerified=certificate!=null&&!string.IsNullOrEmpty(certificate.certificateNo)&&certificate.runId==run&&certificate.recordId==record;
        if(potCertificateVerified)potPendingCertificate=certificate;
        if(!potUploading)ApplyPotSettlement();return true;
    }
    void ApplyPotSettlement(){
        if(potPendingCertificate!=null)potCertificate.Apply(potPendingCertificate);
        if(!potCertificateVerified)potCertificate.number.text=string.IsNullOrEmpty(potVoucher)&&!string.IsNullOrEmpty(potSettlementError)?"未通过验证":"通关已记录";
        potCertificate.Message(!string.IsNullOrEmpty(potVoucher)?"奖状已就绪，点击领取红包":!string.IsNullOrEmpty(potSettlementError)?Match3DGameRecords.RewardErrorMessage(potSettlementError):"通关已记录，本局暂无红包");
        if(potCertificate.Confirmed)ContinuePotCertificate();
    }
    void ContinuePotCertificate(){
        if(!PotCertificateShowing)return;
        if(!PotOwnerMatches(potCertificateOwner)){potCertificate.Message("账号已变化，请返回首页",true);return;}
        if(!potSettled){potCertificate.Message("正在同步通关记录，可返回首页稍后查看红包");return;}
        if(string.IsNullOrEmpty(potVoucher)){potCertificate.Message(string.IsNullOrEmpty(potSettlementError)?"通关已记录，本局暂无红包":Match3DGameRecords.RewardErrorMessage(potSettlementError),true);return;}
        if(potRewardsReleased)return;
        potRewardsReleased=true;potCertificate.Message("红包已到账，可联系客服兑换",true);
        string voucher=potVoucher,owner=potCertificateOwner;
        if(Match3DRewards.Instance)Match3DRewards.Instance.NotifyReceivedVoucher(voucher,owner);
    }
}
