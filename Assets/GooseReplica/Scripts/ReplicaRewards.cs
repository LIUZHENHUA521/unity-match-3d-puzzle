using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public sealed class ReplicaRewards : MonoBehaviour {
    [Serializable] public class Voucher {public string id,type,runId,recordId,rewardTier,status;public int money;public long createTime,expiresAt;}
    [Serializable] public class Reply {public bool ok;public string error,runId,rewardTier,ruleVersion;public ReplicaDailyLevel level;public Voucher[] vouchers;public int nextCursor=-1;}
    [Serializable] class StartBody {public string requestId,tier,themeId;public int levelProtocol=1;}
    public static ReplicaRewards Instance {get;private set;}
    string endpoint;bool starting;
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');}
    void OnDestroy(){if(Instance==this)Instance=null;}
    public void Begin(int theme,string tier,string themeId){if(!starting)StartCoroutine(StartChallenge(theme,tier,themeId));}
    IEnumerator StartChallenge(int theme,string tier,string themeId){
        var screen=ReplicaFlow.Instance.ScreenName;bool testing=ReplicaTestPrivileges.AccountEnabled;starting=true;ReplicaFlow.Instance.Notice("正在确认红包挑战资格…");Reply result=null;
        yield return Request(testing?"daily":"runs",JsonUtility.ToJson(new StartBody{requestId=Guid.NewGuid().ToString("N"),tier=tier,themeId=themeId}),r=>result=r,testing);
        starting=false;if(ReplicaFlow.Instance.ScreenName!=screen)yield break;
        if(result!=null&&result.ok){
            var pool=ReplicaSourceLevelPlan.Get(ReplicaFlow.Instance.SourceEnvironmentForTheme(theme)).prefabs.Length;
            if(result.level==null||result.level.tier!=tier||!result.level.IsValid(pool)){ReplicaFlow.Instance.Notice("每日关卡配置无效，请更新游戏或稍后重试");yield break;}
            if(testing)ReplicaFlow.Instance.BeginDailyPractice(theme,result.level);
            else ReplicaFlow.Instance.BeginQualified(theme,result.runId,result.rewardTier,result.ruleVersion,result.level);
            yield break;
        }
        ReplicaFlow.Instance.Notice(result!=null&&!string.IsNullOrEmpty(result.error)?result.error:"网络暂不可用，请重试");
    }
    public IEnumerator Request(string action,string json,Action<Reply> completed,bool daily=false){
        var account=ReplicaInvitations.Instance;
        if(!account||!account.Ready){completed(new Reply{error="请先完成微信登录"});yield break;}
        using(var req=new UnityWebRequest(endpoint+(daily?"/levels/":"/rewards/")+action,"POST")){
            req.uploadHandler=new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));req.downloadHandler=new DownloadHandlerBuffer();req.timeout=10;
            req.SetRequestHeader("Content-Type","application/json");req.SetRequestHeader("Authorization","Bearer "+account.RecordToken);
            yield return req.SendWebRequest();Reply result=null;
            try{result=JsonUtility.FromJson<Reply>(req.downloadHandler.text);}catch{}
            if(req.responseCode==401)account.RefreshRecordLogin();
            completed(result??new Reply{error="网络暂不可用，请重试"});
        }
    }
}
