using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Persists telemetry and qualified reward settlement separately; only the server issues vouchers.
public sealed class Match3DGameRecords : MonoBehaviour {
    [Serializable] public class ToolUse { public int free,inventory,rewarded,test,share,ad; }
    [Serializable] public class AcquisitionUse { public int free,share,ad; }
    [Serializable] public class ReviveUse { public AcquisitionUse clear=new AcquisitionUse(),time=new AcquisitionUse(); }
    [Serializable] public class Record {
        public int schemaVersion=1,stage,initialCount,timeLimitSeconds,clearedCount;
        public string rewardTier,ruleVersion,challengeTier,levelGeneratorVersion;public int levelSeed;
        public string[] toolReceipts=new string[0];
        public string recordId,runId,mode="solo",themeId,levelDate,levelVersion="2026-10-31-v1",clientVersion,platform,status,endReason;
        public long startedAt,endedAt,activeDurationMs;
        public float remainingSeconds;
        public bool timeReviveUsed;
        public int acquisitionVersion;
        public ReviveUse reviveUsage;
        public ToolUse[] toolUsage=new[]{new ToolUse(),new ToolUse(),new ToolUse(),new ToolUse()};
    }
    [Serializable] public class Pending {
        public string owner,action,json;public bool reward,rejected,rejectionNotified;
        public long rejectedAt,rejectionHttpStatus;public string rejectionReason;
    }
    [Serializable] public class Queue { public List<Pending> items=new List<Pending>(); }
    [Serializable] class Receipt {public bool ok;public string recordId,voucherId,rewardError,error;}
    public static Match3DGameRecords Instance {get;private set;}
    const string QueueKey="goose.records.queue.v1";
    Match3DDailyLevel dailyLevel;
    Queue queue;Record active;string owner,endpoint,runId,rewardTier,ruleVersion;
    readonly List<string> toolReceipts=new List<string>();bool started,hidden,notified;double activeSeconds;int cleared;
    readonly List<Pending> unbound=new List<Pending>();
    readonly HashSet<string> rejectionNotifiedOwners=new HashSet<string>();
    public string SyncError {get;private set;}
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');
        try{queue=JsonUtility.FromJson<Queue>(PlayerPrefs.GetString(QueueKey,""));}catch{}
        if(queue==null||queue.items==null)queue=new Queue();
        // Old clients retained rejected payloads without a reason or acknowledgement.
        // Keep those payloads for diagnosis, but do not replay their startup warning.
        bool migrated=false;
        foreach(var item in queue.items){if(item.rejected&&item.rejectedAt==0&&!item.rejectionNotified){item.rejectionNotified=true;migrated=true;}}
        if(migrated)Save();
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        WeChatWASM.WX.OnHide(OnWechatHide);WeChatWASM.WX.OnShow(OnWechatShow);
#endif
        StartCoroutine(Upload());
    }
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    void OnWechatHide(WeChatWASM.GeneralCallbackResult data){hidden=true;}
    void OnWechatShow(WeChatWASM.OnShowListenerResult data){hidden=false;}
#endif
    void OnApplicationPause(bool paused){hidden=paused;}
    void OnDestroy(){
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        WeChatWASM.WX.OffHide(OnWechatHide);WeChatWASM.WX.OffShow(OnWechatShow);
#endif
        if(Instance==this)Instance=null;
    }
    public void NewRun(){dailyLevel=null;runId=Guid.NewGuid().ToString("N");rewardTier=null;ruleVersion=null;}
    public void NewDailyRun(Match3DDailyLevel level){NewRun();dailyLevel=level;}
    public void QualifiedRun(string id,string tier,string version,Match3DDailyLevel level=null){runId=id;rewardTier=tier;ruleVersion=version;dailyLevel=level;}
    public void InventoryReceipt(string id){if(active!=null&&started&&!string.IsNullOrEmpty(id))toolReceipts.Add(id);}
    public void Prepare(string theme,int stage,string date,int count,int seconds){
        if(string.IsNullOrEmpty(runId))NewRun();
        toolReceipts.Clear();
        active=new Record{rewardTier=rewardTier,ruleVersion=ruleVersion,recordId=Guid.NewGuid().ToString("N"),runId=runId,themeId=theme,stage=stage,levelDate=date,initialCount=count,timeLimitSeconds=seconds,clientVersion=Application.version,platform=Application.platform.ToString()};
        if(dailyLevel!=null){active.levelVersion=dailyLevel.version;active.levelSeed=dailyLevel.seed;active.levelGeneratorVersion=dailyLevel.generatorVersion;active.challengeTier=dailyLevel.tier;}
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        try{active.platform=WeChatWASM.WX.GetSystemInfoSync().platform;}catch{}
#endif
        // Qualified test accounts keep the server's level snapshot; test tool usage is recorded separately.
        if(string.IsNullOrEmpty(rewardTier)&&Match3DFlow.Instance&&Match3DFlow.Instance.UnlimitedMagnet)active.levelVersion=Match3DTestPrivileges.LevelVersion;
        owner=null;started=false;activeSeconds=0;cleared=0;
    }
    public void StartRecord(){if(active==null||started)return;started=true;active.startedAt=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var account=Match3DInvitations.Instance;owner=account&&account.Ready?account.State.inviteCode:null;
        Add("start",active,owner);
    }
    void Update(){
        var flow=Match3DFlow.Instance;
        if(active!=null&&flow&&!flow.IsPvp&&GameManager.Instance&&GameManager.Instance.GameIsPlaying()&&!hidden&&Time.timeScale>0&&!flow.BlocksGameplay){StartRecord();activeSeconds+=Time.unscaledDeltaTime;}
        var account=Match3DInvitations.Instance;
        if(account&&account.Ready){
            if(active!=null&&started&&string.IsNullOrEmpty(owner))owner=account.State.inviteCode;
            if(unbound.Count>0){foreach(var item in unbound){item.owner=account.State.inviteCode;queue.items.Add(item);}unbound.Clear();Save();}
        }
        if(SyncError=="queue_full"){
            if(!notified&&flow){notified=true;flow.Notice("记录缓存已满，暂无法保存新记录");}
            return;
        }
        string currentOwner=account&&account.Ready?account.State.inviteCode:null;
        SyncError=!string.IsNullOrEmpty(currentOwner)&&queue.items.Exists(x=>x.rejected&&x.owner==currentOwner)?"rejected":null;
        if(!flow||string.IsNullOrEmpty(currentOwner)||!queue.items.Exists(x=>x.rejected&&!x.rejectionNotified&&x.owner==currentOwner))return;
        // Persist acknowledgement so a restart cannot turn one failure into repeated warnings.
        foreach(var item in queue.items){if(item.rejected&&item.owner==currentOwner)item.rejectionNotified=true;}
        Save();
        if(rejectionNotifiedOwners.Add(currentOwner))flow.Notice("有闯关记录未能同步，已保留，请稍后联系客服");
    }
    public void Matched(){if(active!=null&&started)cleared=Mathf.Min(active.initialCount,cleared+3);}
    public void Finish(string status,string reason){
        if(active==null)return;
        if(!started){active=null;return;}
        active.status=status;active.endReason=reason;active.endedAt=Math.Max(active.startedAt,DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());active.activeDurationMs=Math.Min((long)(activeSeconds*1000),active.endedAt-active.startedAt);
        active.clearedCount=cleared;active.remainingSeconds=Mathf.Max(0,GameManager.Instance.currentLevelTime);active.timeReviveUsed=Match3DFlow.Instance&&Match3DFlow.Instance.TimeReviveUsed;
        active.acquisitionVersion=1;active.reviveUsage=Match3DFlow.Instance?Match3DFlow.Instance.ReviveUsage:new ReviveUse();
        for(int i=0;i<BoardTools.ToolCount;i++){int used=BoardTools.UsedCount(i);active.toolUsage[i].free=BoardTools.FreeUses(i);active.toolUsage[i].share=BoardTools.ShareUses(i);active.toolUsage[i].ad=BoardTools.AdUses(i);active.toolUsage[i].rewarded=active.toolUsage[i].ad+active.toolUsage[i].share;active.toolUsage[i].test=BoardTools.TestUses(i);active.toolUsage[i].inventory=Math.Max(0,used-active.toolUsage[i].free-active.toolUsage[i].rewarded-active.toolUsage[i].test);}
        active.toolReceipts=toolReceipts.ToArray();Add("finish",active,owner);active=null;started=false;
    }
    void Add(string action,Record record,string account){
        if(Match3DFlow.Instance&&Match3DFlow.Instance.LocalPreview)return;
        // Each live record reserves its terminal slot at start; never drop a queued terminal.
        if(action=="start"&&queue.items.Count+unbound.Count>=498){SyncError="queue_full";active=null;return;}
        var item=new Pending{reward=!string.IsNullOrEmpty(record.rewardTier),owner=account,action=action,json=JsonUtility.ToJson(record)};
        if(string.IsNullOrEmpty(account))unbound.Add(item);else{queue.items.Add(item);Save();}
    }
    static string RewardErrorMessage(string error){
        switch(error){
            case "daily_quota_exhausted":return "通关已记录，今日红包名额已领完，本局未获得红包";
            case "hourly_quota_exhausted":return "通关已记录，当前时段红包名额已领完，本局未获得红包";
            case "daily_quota_claimed":return "通关已记录，今日已领取过该档位红包";
            case "rewards_closed":return "通关已记录，红包挑战已关闭，本局未获得红包";
            case "qualification_expired":return "通关已记录，本局红包资格已过期";
            case "test_tools_not_allowed":return "通关已记录，本局未开通测试道具权限，请重新开始挑战";
            default:return "通关已记录，未获红包资格："+error;
        }
    }
    void Save(){PlayerPrefs.SetString(QueueKey,JsonUtility.ToJson(queue));PlayerPrefs.Save();}
    IEnumerator Upload(){
        float delay=1;
        while(true){
            yield return new WaitForSecondsRealtime(delay);
            var account=Match3DInvitations.Instance;if(!account||!account.Ready)continue;
            var item=queue.items.Find(x=>!x.rejected&&x.owner==account.State.inviteCode);if(item==null)continue;
            using(var req=new UnityWebRequest(endpoint+(item.reward?"/rewards/":"/records/")+item.action,"POST")){
                req.uploadHandler=new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(item.json));req.downloadHandler=new DownloadHandlerBuffer();req.timeout=10;
                req.SetRequestHeader("Content-Type","application/json");req.SetRequestHeader("Authorization","Bearer "+account.RecordToken);
                yield return req.SendWebRequest();
                Receipt receipt=null;try{receipt=JsonUtility.FromJson<Receipt>(req.downloadHandler.text);}catch{}
                if(req.result==UnityWebRequest.Result.Success&&receipt!=null&&receipt.ok){if(item.reward&&item.action=="finish"&&Match3DFlow.Instance&&account.Ready&&account.State.inviteCode==item.owner){if(!string.IsNullOrEmpty(receipt.voucherId)&&Match3DRewards.Instance)Match3DRewards.Instance.NotifyReceivedVoucher(receipt.voucherId,item.owner);else if(!string.IsNullOrEmpty(receipt.rewardError))Match3DFlow.Instance.Notice(RewardErrorMessage(receipt.rewardError));}
                    queue.items.Remove(item);Save();delay=1;continue;}
                if(req.responseCode==401){if(account.Ready&&account.State.inviteCode==item.owner)account.RefreshRecordLogin();}
                else if(req.responseCode==400||req.responseCode==409){
                    item.rejected=true;item.rejectionNotified=false;item.rejectedAt=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();item.rejectionHttpStatus=req.responseCode;
                    string reason=receipt!=null?receipt.error:null;
                    item.rejectionReason=string.IsNullOrEmpty(reason)?"HTTP "+req.responseCode:reason.Substring(0,Math.Min(512,reason.Length));
                    Save();
                }
                delay=Mathf.Min(60,delay*2);
            }
        }
    }
}
