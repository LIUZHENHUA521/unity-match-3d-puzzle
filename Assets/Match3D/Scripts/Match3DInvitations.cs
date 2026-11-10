using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// The server is authoritative. PlayerPrefs stores only an unfinished request, never rewards.
public sealed class Match3DInvitations : MonoBehaviour {
    [Serializable] public class Reply {public string token,inviteCode,error,requestId;public int invited;public long createdAt;public int[] inventory;public bool refunded,unlimitedMagnet;public Match3DMail[] mails;public int nextCursor;public bool attention;}
    [Serializable] class Bridge {public string code,inviter,error;public bool refresh;}
    [Serializable] class LoginBody {public string code,inviter;}
    [Serializable] class UseBody {public string requestId;public int type;}
    public static Match3DInvitations Instance {get;private set;}
    public Reply State {get;private set;}
    public string Status {get;private set;}="正在连接账号服务…";
    public bool Busy {get;private set;}
    public string RecordToken=>token;
    public void RefreshRecordLogin(){if(!Busy&&!logging){token=null;Refresh();}}
    public bool Ready=>State!=null&&!string.IsNullOrEmpty(token);
    // Share the startup login instead of treating an in-flight login as a failure.
    public IEnumerator EnsureReady(Action<bool> completed,Func<bool> cancelled=null){
        if(!Ready&&!Busy&&!logging)Refresh();
        float deadline=Time.realtimeSinceStartup+35f;
        while(!Ready&&(Busy||logging)&&Time.realtimeSinceStartup<deadline){
            if(cancelled!=null&&cancelled()){completed(false);yield break;}
            yield return null;
        }
        if(cancelled!=null&&cancelled()){completed(false);yield break;}
        if(!Ready&&(Busy||logging))SetStatus("登录超时，请重试");
        completed(Ready);
    }
    public event Action Changed;
    public event Action MailChanged;
    public readonly System.Collections.Generic.List<Match3DMail> Mails=new System.Collections.Generic.List<Match3DMail>();
    public bool MailAttention {get;private set;}
    public int MailNextCursor {get;private set;}=-1;
    public bool MailLoaded {get;private set;}
    [Serializable] class MailBody {public string id;public int cursor;}
    string endpoint,token;bool logging;float loginDeadline;
    string PendingKey=>"goose.invites.pending."+(State!=null?State.inviteCode:"");
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern void GooseInvitesInit(string target);
    [DllImport("__Internal")] static extern void GooseInvitesLogin();
    [DllImport("__Internal")] static extern void GooseInvitesShare(string code);
#endif
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');
#if UNITY_WEBGL && !UNITY_EDITOR
        GooseInvitesInit(gameObject.name);
#endif
        Refresh();
    }
    void OnDestroy(){if(Instance==this)Instance=null;}
    void Update(){if(logging&&Time.realtimeSinceStartup>loginDeadline){logging=false;SetStatus("登录超时，请重试");}}
    public int Inventory(int type)=>Ready&&State.inventory!=null&&type>=0&&type<State.inventory.Length?State.inventory[type]:0;
    void SetStatus(string value){Status=value;Changed?.Invoke();}
    public void Refresh(){if(Busy||logging)return;
#if UNITY_WEBGL && !UNITY_EDITOR
        logging=true;loginDeadline=Time.realtimeSinceStartup+25;SetStatus("正在登录微信…");GooseInvitesLogin();
#else
        SetStatus("请在微信小游戏中登录（编辑器仅预览）");
#endif
    }
    [UnityEngine.Scripting.Preserve]
    public void OnInvitationBridge(string json){var data=JsonUtility.FromJson<Bridge>(json);if(data.refresh){Refresh();return;}if(!string.IsNullOrEmpty(data.error)){logging=false;SetStatus(data.error);return;}if(!string.IsNullOrEmpty(data.code)){logging=false;if(!Busy)StartCoroutine(Login(data));}}
    IEnumerator Login(Bridge data){Busy=true;Reply reply=null;yield return Request("login",JsonUtility.ToJson(new LoginBody{code=data.code,inviter=data.inviter}),r=>reply=r,false);
        if(reply!=null){if(State!=null&&State.inviteCode!=reply.inviteCode){Mails.Clear();MailLoaded=false;MailAttention=false;MailNextCursor=-1;MailChanged?.Invoke();}token=reply.token;Apply(reply);yield return Recover();yield return FetchMail("list",null,0);}Busy=false;Changed?.Invoke();
    }
    void Apply(Reply reply){State=reply;SetStatus("");}
    IEnumerator Request(string route,string body,Action<Reply> callback,bool retry=true){
        for(int attempt=0;attempt<(retry?2:1);attempt++){
            using(var req=new UnityWebRequest(endpoint+"/invites/"+route,"POST")){
                req.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));req.downloadHandler=new DownloadHandlerBuffer();req.timeout=8;
                req.SetRequestHeader("Content-Type","application/json");req.SetRequestHeader("X-Goose-Tool-Count",BoardTools.ToolCount.ToString());if(!string.IsNullOrEmpty(token))req.SetRequestHeader("Authorization","Bearer "+token);
                yield return req.SendWebRequest();
                if(req.result==UnityWebRequest.Result.Success){Reply result=null;try{result=JsonUtility.FromJson<Reply>(req.downloadHandler.text);}catch{}if(result!=null&&result.inventory!=null&&(result.inventory.Length==3||result.inventory.Length==BoardTools.ToolCount)){callback(result);yield break;}}
                if(req.responseCode>=400&&req.responseCode<500){if(req.responseCode==401){token=null;State=null;Mails.Clear();MailLoaded=false;MailAttention=false;MailChanged?.Invoke();}SetStatus(req.responseCode==401?"登录已失效，请刷新重新登录":req.responseCode==410?"邮件已过期":req.responseCode==409?"道具不足或请求已失效，请刷新后重试":"请求失败，请稍后重试");yield break;}
            }
        }
        SetStatus("网络暂不可用，请稍后重试");
    }
    // An interrupted debit is settled before permitting another debit, including after restart.
    IEnumerator Recover(){var raw=PlayerPrefs.GetString(PendingKey,"");if(string.IsNullOrEmpty(raw))yield break;
        UseBody pending=null;try{pending=JsonUtility.FromJson<UseBody>(raw);}catch{}
        if(pending==null)yield break;
        Reply refund=null;yield return Request("refund",raw,r=>refund=r);
        if(refund!=null){Apply(refund);ClearPending();}
    }
    void ClearPending(){PlayerPrefs.DeleteKey(PendingKey);PlayerPrefs.Save();}
    public bool HasPending=>PlayerPrefs.HasKey(PendingKey);
    public IEnumerator Use(int type,Func<bool> effect){
        if(Busy||logging||!Ready||HasPending||Inventory(type)<1)yield break;
        Busy=true;var body=JsonUtility.ToJson(new UseBody{type=type,requestId=Guid.NewGuid().ToString("N")});
        PlayerPrefs.SetString(PendingKey,body);PlayerPrefs.Save();Reply reply=null;
        yield return Request("consume",body,r=>reply=r);
        bool applied=false;
        if(reply!=null){Apply(reply);if(!reply.refunded){try{applied=effect();}catch(Exception e){Debug.LogException(e);}}else ClearPending();}
        if(applied){if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.InventoryReceipt(reply.requestId);ClearPending();}
        else if(reply==null||!reply.refunded){Reply refund=null;yield return Request("refund",body,r=>refund=r);if(refund!=null){Apply(refund);ClearPending();}}
        Busy=false;Changed?.Invoke();
    }
    public void RefreshMail(bool more=false){if(Busy||logging)return;if(!Ready){Refresh();return;}StartCoroutine(MailOperation("list",null,more?MailNextCursor:0));}
    public void ReadMail(string id){if(!Busy&&Ready)StartCoroutine(MailOperation("read",id,0));}
    public void ClaimMail(string id,Action<bool> completed){if(Busy||!Ready){completed(false);return;}StartCoroutine(MailOperation("claim",id,0,completed));}
    IEnumerator MailOperation(string action,string id,int cursor,Action<bool> completed=null){
        var claiming=action=="claim"?Mails.Find(mail=>mail.id==id&&!mail.claimed&&!mail.expired):null;
        var received=claiming!=null&&claiming.rewards!=null?(int[])claiming.rewards.Clone():null;
        string claimant=State!=null?State.inviteCode:null;
        if(cursor<0)yield break;Busy=true;Changed?.Invoke();bool success=false;
        yield return FetchMail(action,id,cursor,ok=>success=ok);
        Busy=false;Changed?.Invoke();MailChanged?.Invoke();completed?.Invoke(success);
        if(success&&received!=null&&State!=null&&State.inviteCode==claimant&&Match3DFlow.Instance)
            Match3DFlow.Instance.ShowReceivedTools(received);
    }
    IEnumerator FetchMail(string action,string id,int cursor,Action<bool> completed=null){
        Reply reply=null;yield return Request("mail/"+action,JsonUtility.ToJson(new MailBody{id=id,cursor=cursor}),r=>reply=r);
        if(reply!=null&&reply.mails!=null){
            Apply(reply);if(cursor==0)Mails.Clear();
            foreach(var mail in reply.mails){int index=Mails.FindIndex(m=>m.id==mail.id);if(index>=0)Mails[index]=mail;else Mails.Add(mail);}
            MailNextCursor=reply.nextCursor;MailAttention=reply.attention;MailLoaded=true;MailChanged?.Invoke();
        }
        completed?.Invoke(reply!=null&&reply.mails!=null);
    }
    public void Share(){if(!Ready||HasPending){Refresh();return;}
#if UNITY_WEBGL && !UNITY_EDITOR
        GooseInvitesShare(State.inviteCode);SetStatus("好友首次进入游戏后，奖励会自动到账");
#else
        SetStatus("请在微信小游戏中邀请好友");
#endif
    }
}
