using System.Runtime.InteropServices;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Shared by the home menu and home settings. Call directly from the user's tap.
public sealed class Match3DCustomerService : MonoBehaviour {
    [Serializable] class Reply {public string url,error;public long expiresAt;}
    static Match3DCustomerService instance;
    string endpoint,url,owner;float refreshAt;bool preparing;
    public void Initialize(string value){instance=this;endpoint=value.TrimEnd('/');}
    void OnDestroy(){if(instance==this)instance=null;}
    void Update(){
        var account=Match3DInvitations.Instance;
        if(!account||!account.Ready){url=null;owner=null;return;}
        if(owner!=account.State.inviteCode){url=null;refreshAt=0;}
        if(!preparing&&Time.realtimeSinceStartup>=refreshAt)StartCoroutine(Prepare());
    }
    IEnumerator Prepare(){
        preparing=true;refreshAt=Time.realtimeSinceStartup+15f;
        var account=Match3DInvitations.Instance;
        if(!account||!account.Ready){preparing=false;yield break;}
        string requestedOwner=account.State.inviteCode;
        owner=requestedOwner;
        using(var req=new UnityWebRequest(endpoint+"/invites/customer-service","POST")){
            req.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes("{}"));req.downloadHandler=new DownloadHandlerBuffer();req.timeout=8;
            req.SetRequestHeader("Content-Type","application/json");req.SetRequestHeader("Authorization","Bearer "+account.RecordToken);
            yield return req.SendWebRequest();Reply reply=null;
            if(req.result==UnityWebRequest.Result.Success){try{reply=JsonUtility.FromJson<Reply>(req.downloadHandler.text);}catch{}}
            if(account&&account.Ready&&account.State.inviteCode==requestedOwner&&reply!=null&&reply.url!=null&&(reply.url.StartsWith("https://work.weixin.qq.com/kf/",StringComparison.Ordinal)||reply.url.StartsWith("https://work.weixin.qq.com/kfid/",StringComparison.Ordinal))&&reply.expiresAt>DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()+60000){
                url=reply.url;refreshAt=Time.realtimeSinceStartup+Mathf.Min(1800f,(reply.expiresAt-DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())/1000f-60f);
            }else{url=null;if(req.responseCode==401&&account)account.RefreshRecordLogin();}
        }
        preparing=false;
    }
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern void GooseCustomerServiceOpen(string url);
#endif
    public static void Open() {
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        var account=Match3DInvitations.Instance;
        if(instance&&account&&account.Ready&&instance.owner==account.State.inviteCode&&!string.IsNullOrEmpty(instance.url)&&Time.realtimeSinceStartup<instance.refreshAt){GooseCustomerServiceOpen(instance.url);return;}
        if(instance){instance.refreshAt=0;if(!instance.preparing)instance.StartCoroutine(instance.Prepare());}
        if(Match3DFlow.Instance)Match3DFlow.Instance.ShowHomeFeatureNotice("正在关联红包凭证，请稍后再点联系客服");
#else
        if (Match3DFlow.Instance) Match3DFlow.Instance.ShowHomeFeatureNotice("请在微信小游戏中联系客服");
#endif
    }
}
