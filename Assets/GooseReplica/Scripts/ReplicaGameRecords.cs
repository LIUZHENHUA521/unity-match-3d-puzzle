using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Persists telemetry and qualified reward settlement separately; only the server issues vouchers.
public sealed class ReplicaGameRecords : MonoBehaviour {
    [Serializable] public class ToolUse { public int free,inventory,rewarded,test; }
    [Serializable] public class Record {
        public int schemaVersion=1,stage,initialCount,timeLimitSeconds,clearedCount;
        public string rewardTier,ruleVersion;
        public string[] toolReceipts=new string[0];
        public string recordId,runId,mode="solo",themeId,levelDate,levelVersion="2026-10-31-v1",clientVersion,platform,status,endReason;
        public long startedAt,endedAt,activeDurationMs;
        public float remainingSeconds;
        public ToolUse[] toolUsage=new[]{new ToolUse(),new ToolUse(),new ToolUse()};
    }
    [Serializable] public class Pending { public string owner,action,json;public bool reward; public bool rejected; }
    [Serializable] public class Queue { public List<Pending> items=new List<Pending>(); }
    [Serializable] class Receipt {public bool ok;public string recordId,voucherId,rewardError;}
    public static ReplicaGameRecords Instance {get;private set;}
    const string QueueKey="goose.records.queue.v1";
    Queue queue;Record active;string owner,endpoint,runId,rewardTier,ruleVersion;
    readonly List<string> toolReceipts=new List<string>();bool started,hidden,notified;double activeSeconds;int cleared;
    readonly List<Pending> unbound=new List<Pending>();
    public string SyncError {get;private set;}
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');
        try{queue=JsonUtility.FromJson<Queue>(PlayerPrefs.GetString(QueueKey,""));}catch{}
        if(queue==null||queue.items==null)queue=new Queue();
        if(queue.items.Exists(x=>x.rejected))SyncError="rejected";
#if UNITY_WEBGL && !UNITY_EDITOR
        WeChatWASM.WX.OnHide(OnWechatHide);WeChatWASM.WX.OnShow(OnWechatShow);
#endif
        StartCoroutine(Upload());
    }
#if UNITY_WEBGL && !UNITY_EDITOR
    void OnWechatHide(WeChatWASM.GeneralCallbackResult data){hidden=true;}
    void OnWechatShow(WeChatWASM.OnShowListenerResult data){hidden=false;}
#endif
    void OnApplicationPause(bool paused){hidden=paused;}
    void OnDestroy(){
#if UNITY_WEBGL && !UNITY_EDITOR
        WeChatWASM.WX.OffHide(OnWechatHide);WeChatWASM.WX.OffShow(OnWechatShow);
#endif
        if(Instance==this)Instance=null;
    }
    public void NewRun(){runId=Guid.NewGuid().ToString("N");rewardTier=null;ruleVersion=null;}
    public void QualifiedRun(string id,string tier,string version){runId=id;rewardTier=tier;ruleVersion=version;}
    public void InventoryReceipt(string id){if(active!=null&&started&&!string.IsNullOrEmpty(id))toolReceipts.Add(id);}
    public void Prepare(string theme,int stage,string date,int count,int seconds){
        if(string.IsNullOrEmpty(runId))NewRun();
        toolReceipts.Clear();
        active=new Record{rewardTier=rewardTier,ruleVersion=ruleVersion,recordId=Guid.NewGuid().ToString("N"),runId=runId,themeId=theme,stage=stage,levelDate=date,initialCount=count,timeLimitSeconds=seconds,clientVersion=Application.version,platform=Application.platform.ToString()};
#if UNITY_WEBGL && !UNITY_EDITOR
        try{active.platform=WeChatWASM.WX.GetSystemInfoSync().platform;}catch{}
#endif
        if(ReplicaFlow.Instance&&ReplicaFlow.Instance.UnlimitedMagnet)active.levelVersion=ReplicaTestPrivileges.LevelVersion;
        owner=null;started=false;activeSeconds=0;cleared=0;
    }
    public void StartRecord(){if(active==null||started)return;started=true;active.startedAt=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var account=ReplicaInvitations.Instance;owner=account&&account.Ready?account.State.inviteCode:null;
        Add("start",active,owner);
    }
    void Update(){
        var flow=ReplicaFlow.Instance;
        if(active!=null&&flow&&!flow.IsPvp&&GameManager.Instance&&GameManager.Instance.GameIsPlaying()&&!hidden&&Time.timeScale>0&&!flow.BlocksGameplay){StartRecord();activeSeconds+=Time.unscaledDeltaTime;}
        var account=ReplicaInvitations.Instance;
        if(account&&account.Ready){
            if(active!=null&&started&&string.IsNullOrEmpty(owner))owner=account.State.inviteCode;
            if(unbound.Count>0){foreach(var item in unbound){item.owner=account.State.inviteCode;queue.items.Add(item);}unbound.Clear();Save();}
        }
        if(!string.IsNullOrEmpty(SyncError)&&!notified&&flow){notified=true;flow.Notice(SyncError=="queue_full"?"记录缓存已满，暂无法保存新记录":"闯关记录同步异常，记录已保留");}
    }
    public void Matched(){if(active!=null&&started)cleared=Mathf.Min(active.initialCount,cleared+3);}
    public void Finish(string status,string reason){
        if(active==null)return;
        if(!started){active=null;return;}
        active.status=status;active.endReason=reason;active.endedAt=Math.Max(active.startedAt,DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());active.activeDurationMs=Math.Min((long)(activeSeconds*1000),active.endedAt-active.startedAt);
        active.clearedCount=cleared;active.remainingSeconds=Mathf.Clamp(GameManager.Instance.currentLevelTime,0,active.timeLimitSeconds);
        for(int i=0;i<3;i++){int used=ReplicaSourceTools.UsedCount(i);active.toolUsage[i].free=0;active.toolUsage[i].rewarded=ReplicaSourceTools.AdUses(i);active.toolUsage[i].test=ReplicaSourceTools.TestUses(i);active.toolUsage[i].inventory=Math.Max(0,used-active.toolUsage[i].rewarded-active.toolUsage[i].test);}
        active.toolReceipts=toolReceipts.ToArray();Add("finish",active,owner);active=null;started=false;
    }
    void Add(string action,Record record,string account){
        // Each live record reserves its terminal slot at start; never drop a queued terminal.
        if(action=="start"&&queue.items.Count+unbound.Count>=498){SyncError="queue_full";active=null;return;}
        var item=new Pending{reward=!string.IsNullOrEmpty(record.rewardTier),owner=account,action=action,json=JsonUtility.ToJson(record)};
        if(string.IsNullOrEmpty(account))unbound.Add(item);else{queue.items.Add(item);Save();}
    }
    void Save(){PlayerPrefs.SetString(QueueKey,JsonUtility.ToJson(queue));PlayerPrefs.Save();}
    IEnumerator Upload(){
        float delay=1;
        while(true){
            yield return new WaitForSecondsRealtime(delay);
            var account=ReplicaInvitations.Instance;if(!account||!account.Ready)continue;
            var item=queue.items.Find(x=>!x.rejected&&x.owner==account.State.inviteCode);if(item==null)continue;
            using(var req=new UnityWebRequest(endpoint+(item.reward?"/rewards/":"/records/")+item.action,"POST")){
                req.uploadHandler=new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(item.json));req.downloadHandler=new DownloadHandlerBuffer();req.timeout=10;
                req.SetRequestHeader("Content-Type","application/json");req.SetRequestHeader("Authorization","Bearer "+account.RecordToken);
                yield return req.SendWebRequest();
                Receipt receipt=null;if(req.result==UnityWebRequest.Result.Success){try{receipt=JsonUtility.FromJson<Receipt>(req.downloadHandler.text);}catch{}}
                if(receipt!=null&&receipt.ok){if(item.reward&&item.action=="finish"&&ReplicaFlow.Instance){if(!string.IsNullOrEmpty(receipt.voucherId))ReplicaFlow.Instance.Notice("已获得红包凭证，可在我的红包查看");else if(!string.IsNullOrEmpty(receipt.rewardError))ReplicaFlow.Instance.Notice("通关已记录，未获红包资格："+receipt.rewardError);}
                    queue.items.Remove(item);Save();delay=1;continue;}
                if(req.responseCode==401)account.RefreshRecordLogin();
                else if(req.responseCode==400||req.responseCode==409){item.rejected=true;SyncError="rejected";Save();}
                delay=Mathf.Min(60,delay*2);
            }
        }
    }
}
