using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public sealed class ReplicaRewards : MonoBehaviour {
    [Serializable] public class Voucher {public string id,type,runId,recordId,rewardTier,status;public int money;public long createTime,expiresAt;}
    [Serializable] public class Reply {public bool ok;public string error,runId,rewardTier,ruleVersion;public Voucher[] vouchers;public int nextCursor=-1;}
    [Serializable] class StartBody {public string requestId,tier,themeId;}
    public static ReplicaRewards Instance {get;private set;}
    string endpoint;bool starting;
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');}
    void OnDestroy(){if(Instance==this)Instance=null;}
    public void Begin(int theme,string tier,string themeId){if(!starting)StartCoroutine(StartChallenge(theme,tier,themeId));}
    IEnumerator StartChallenge(int theme,string tier,string themeId){
        var screen=ReplicaFlow.Instance.ScreenName;starting=true;ReplicaFlow.Instance.Notice("正在确认红包挑战资格…");Reply result=null;
        yield return Request("runs",JsonUtility.ToJson(new StartBody{requestId=Guid.NewGuid().ToString("N"),tier=tier,themeId=themeId}),r=>result=r);
        starting=false;if(ReplicaFlow.Instance.ScreenName!=screen)yield break;
        if(result!=null&&result.ok){ReplicaFlow.Instance.BeginQualified(theme,result.runId,result.rewardTier,result.ruleVersion);yield break;}
        ReplicaFlow.Instance.Notice((result!=null?result.error:"网络不可用")+"；本局未取得红包资格，进入普通挑战");
        yield return new WaitForSecondsRealtime(2);
        if(ReplicaFlow.Instance.ScreenName==screen)ReplicaFlow.Instance.Begin(theme);
    }
    public IEnumerator Request(string action,string json,Action<Reply> completed){
        var account=ReplicaInvitations.Instance;
        if(!account||!account.Ready){completed(new Reply{error="请先完成微信登录"});yield break;}
        using(var req=new UnityWebRequest(endpoint+"/rewards/"+action,"POST")){
            req.uploadHandler=new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));req.downloadHandler=new DownloadHandlerBuffer();req.timeout=10;
            req.SetRequestHeader("Content-Type","application/json");req.SetRequestHeader("Authorization","Bearer "+account.RecordToken);
            yield return req.SendWebRequest();Reply result=null;
            try{result=JsonUtility.FromJson<Reply>(req.downloadHandler.text);}catch{}
            if(req.responseCode==401)account.RefreshRecordLogin();
            completed(result??new Reply{error="网络暂不可用，请重试"});
        }
    }
}
