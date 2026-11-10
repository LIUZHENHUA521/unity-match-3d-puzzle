using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public sealed class Match3DRewards : MonoBehaviour {
    [Serializable] public class Voucher {public string id,type,runId,recordId,rewardTier,status;public int money;public long createTime,expiresAt;}
    [Serializable] public class Reply {public bool ok;public string error,runId,rewardTier,ruleVersion;public Match3DDailyLevel level;public Voucher[] vouchers;public int nextCursor=-1;}
    [Serializable] class StartBody {public string requestId,tier,themeId;public int levelProtocol=3;public string openingVersion=Match3DDailySnapshot.ResourceVersion;}
    public static Match3DRewards Instance {get;private set;}
    string endpoint;bool starting;
    readonly System.Collections.Generic.HashSet<string> presented=new System.Collections.Generic.HashSet<string>();
    public void NotifyReceivedVoucher(string id,string owner){
        if(string.IsNullOrEmpty(id)||string.IsNullOrEmpty(owner)||!presented.Add(owner+":"+id))return;
        StartCoroutine(PresentVoucher(id,owner));
    }
    IEnumerator PresentVoucher(string id,string owner){
        int cursor=0;Voucher found=null;
        while(cursor>=0){
            var account=Match3DInvitations.Instance;
            if(!account||!account.Ready||account.State.inviteCode!=owner)yield break;
            Reply reply=null;yield return Request("list","{\"cursor\":"+cursor+"}",r=>reply=r);
            if(reply==null||!reply.ok)break;
            found=Array.Find(reply.vouchers??new Voucher[0],v=>v.id==id);
            if(found!=null||reply.nextCursor<=cursor)break;
            cursor=reply.nextCursor;
        }
        var current=Match3DInvitations.Instance;
        if(!current||!current.Ready||current.State.inviteCode!=owner||!Match3DFlow.Instance)yield break;
        // The settlement already confirmed this voucher. Never guess an amount if detail lookup fails.
        decimal? money=found!=null&&found.money>0?(decimal?)found.money:null;
        Match3DFlow.Instance.ShowReceivedRewards(new[]{Match3DRewardItem.RedPacket(money)},receivedAt:found!=null?found.createTime:0);
    }
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');}
    void OnDestroy(){if(Instance==this)Instance=null;}
    public void Begin(int theme,string tier,string themeId){if(!starting)StartCoroutine(StartChallenge(theme,tier,themeId));}
    IEnumerator StartChallenge(int theme,string tier,string themeId){
        var flow=Match3DFlow.Instance;int screen=flow.ScreenVersion;starting=true;
        try{
            var account=Match3DInvitations.Instance;
            if(account&&!account.Ready){
                flow.Notice("正在登录微信，请稍候…");bool ready=false;
                yield return account.EnsureReady(ok=>ready=ok,()=>!flow||flow.ScreenVersion!=screen);
                if(!flow||flow.ScreenVersion!=screen)yield break;
                if(!ready){flow.Notice(string.IsNullOrEmpty(account.Status)?"登录失败，请重试":account.Status);yield break;}
            }
            bool testing=Match3DTestPrivileges.AccountEnabled;flow.Notice("正在确认红包挑战资格…");Reply result=null;
            yield return Request(testing?"daily":"runs",JsonUtility.ToJson(new StartBody{requestId=Guid.NewGuid().ToString("N"),tier=tier,themeId=themeId}),r=>result=r,testing);
            if(!flow||flow.ScreenVersion!=screen)yield break;
            if(result!=null&&result.ok){
                if(result.level==null||!Match3DModelThemes.TryGet(result.level.modelThemeId,flow.SourceEnvironmentForTheme(theme),out var profile)||result.level.tier!=tier||!result.level.IsValid(profile.prefabs.Length)){flow.Notice("每日关卡配置无效，请更新游戏或稍后重试");yield break;}
                if(testing)Match3DFlow.Instance.ChooseTestTheme(theme,result.level);
                else Match3DFlow.Instance.BeginQualified(theme,result.runId,result.rewardTier,result.ruleVersion,result.level);
                yield break;
            }
            Match3DFlow.Instance.Notice(result!=null&&!string.IsNullOrEmpty(result.error)?result.error:"网络暂不可用，请重试");
        }finally{starting=false;}
    }
    public IEnumerator Request(string action,string json,Action<Reply> completed,bool daily=false){
        var account=Match3DInvitations.Instance;
        if(!account){completed(new Reply{error="账号服务尚未就绪，请稍后重试"});yield break;}
        bool ready=false;yield return account.EnsureReady(ok=>ready=ok);
        if(!ready){completed(new Reply{error=string.IsNullOrEmpty(account.Status)?"登录失败，请重试":account.Status});yield break;}
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
