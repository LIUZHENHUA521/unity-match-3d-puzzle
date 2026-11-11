using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// The server, not PlayerPrefs or the native page callback, confirms authorization.
public sealed class Match3DMerchantAuthorization : MonoBehaviour {
    [Serializable] public class Reply {public bool ok,authorized;public string state,appid,mchid,packageInfo,error,requestId;}
    [Serializable] class Bridge {public string requestId,kind;}
    public static Match3DMerchantAuthorization Instance {get;private set;}
    string endpoint,owner,bridgeRequest,bridgeResult;
    bool attempted,busy;
    int screen;
    Match3DCommonDialog dialog;
#if UNITY_EDITOR
    public static bool EditorEnabled;
    public static Action<Reply,Action<string>> EditorOpenOverride;
#endif
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern int GooseMerchantAuthorizationSupported();
    [DllImport("__Internal")] static extern void GooseMerchantAuthorizationOpen(string target,string json);
#endif
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');}
    void OnDestroy(){if(Instance==this)Instance=null;}
    public void OnHome(){
#if UNITY_EDITOR
        if(!EditorEnabled)return;
#elif !UNITY_WEBGL || DAILY_WEB_PREVIEW
        return;
#endif
        if(!attempted){attempted=true;StartCoroutine(CheckStartup());}
    }
    bool SameOwner(){var account=Match3DInvitations.Instance;return account&&account.Ready&&account.State.inviteCode==owner;}
    bool CurrentScreen(){var flow=Match3DFlow.Instance;return SameOwner()&&flow&&flow.ScreenName=="home"&&flow.ScreenVersion==screen;}
    bool CurrentDialog()=>CurrentScreen()&&dialog&&dialog.gameObject.activeInHierarchy;
    IEnumerator CheckStartup(){
        var account=Match3DInvitations.Instance;var flow=Match3DFlow.Instance;
        if(!account||!account.Ready||!flow)yield break;
        owner=account.State.inviteCode;screen=flow.ScreenVersion;
        Reply reply=null;yield return Request("status",r=>reply=r);
        if(!CurrentScreen()||reply==null||!reply.ok||reply.authorized)yield break;
        // Never interrupt another dialog or a game that started during the query.
        while(CurrentScreen()&&!flow.CanPromptMerchantAuthorization)yield return null;
        if(CurrentScreen())dialog=flow.ShowMerchantAuthorization(Skip,Authorize);
    }
    void Skip(){Close();}
    void Close(){bridgeRequest=null;bridgeResult=null;if(Match3DFlow.Instance)Match3DFlow.Instance.CloseMerchantAuthorization(dialog);dialog=null;}
    void Authorize(){if(busy||!CurrentDialog())return;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        if(GooseMerchantAuthorizationSupported()==0){dialog.status.text="当前微信版本不支持，请更新微信后重试";return;}
#endif
        busy=true;dialog.SetBusy(true);dialog.left.interactable=dialog.close.interactable=true;
        dialog.status.text="正在准备微信授权…";StartCoroutine(AuthorizeRoutine());
    }
    IEnumerator AuthorizeRoutine(){
        try{
            Reply reply=null;yield return Request("prepare",r=>reply=r);
            if(!CurrentDialog())yield break;
            if(reply==null||!reply.ok){Message(reply?.error??"网络暂不可用，请稍后重试");yield break;}
            if(reply.authorized){Success();yield break;}
            if(string.IsNullOrEmpty(reply.packageInfo)||string.IsNullOrEmpty(reply.appid)||string.IsNullOrEmpty(reply.mchid)){Message("收款授权暂不可用，请稍后重试");yield break;}
            bridgeRequest=reply.requestId=Guid.NewGuid().ToString("N");bridgeResult=null;
            dialog.status.text="请在微信页面确认收款授权";
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
            GooseMerchantAuthorizationOpen(gameObject.name,JsonUtility.ToJson(reply));
#elif UNITY_EDITOR
            if(EditorOpenOverride!=null){string id=bridgeRequest;EditorOpenOverride(reply,kind=>OnMerchantAuthorizationBridge(JsonUtility.ToJson(new Bridge{requestId=id,kind=kind})));}
            else bridgeResult="unsupported";
#else
            bridgeResult="unsupported";
#endif
            float deadline=Time.realtimeSinceStartup+90;
            while(CurrentDialog()&&bridgeResult==null&&Time.realtimeSinceStartup<deadline)yield return null;
            if(!CurrentDialog())yield break;
            string result=bridgeResult;bridgeRequest=null;
            if(result=="cancelled"){Close();yield break;}
            if(result=="unsupported"){Message("请在支持收款授权的微信版本中操作");yield break;}
            if(result=="failed"){Message("未能打开微信授权，请稍后重试");yield break;}
            // Returning from the native page (or a lost callback) is not proof of consent.
            dialog.status.text="正在确认授权结果…";
            for(int i=0;i<5&&CurrentDialog();i++){
                Reply status=null;yield return Request("status",r=>status=r);
                if(!CurrentDialog())yield break;
                if(status!=null&&status.ok&&status.authorized){Success();yield break;}
                if(status==null||!status.ok){Message(status?.error??"授权状态查询失败，请稍后重试");yield break;}
                if(i<4)yield return new WaitForSecondsRealtime(1);
            }
            Message("尚未确认授权，可稍后重试或继续游戏");
        }finally{busy=false;bridgeRequest=null;if(dialog&&(!CurrentScreen()))Close();}
    }
    void Message(string text){if(CurrentDialog()){dialog.SetBusy(false);dialog.status.text=text;}}
    void Success(){Close();if(Match3DFlow.Instance)Match3DFlow.Instance.ShowTips("微信收款授权已开通");}
    [UnityEngine.Scripting.Preserve]
    public void OnMerchantAuthorizationBridge(string json){
        Bridge result=null;try{result=JsonUtility.FromJson<Bridge>(json);}catch{}
        if(result!=null&&!string.IsNullOrEmpty(bridgeRequest)&&result.requestId==bridgeRequest&&bridgeResult==null)bridgeResult=result.kind;
    }
    IEnumerator Request(string action,Action<Reply> done){
        if(!SameOwner()){done(null);yield break;}
        using(var req=new UnityWebRequest(endpoint+"/rewards/authorization/"+action,"POST")){
            req.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes("{}"));req.downloadHandler=new DownloadHandlerBuffer();req.timeout=35;
            req.SetRequestHeader("Content-Type","application/json");req.SetRequestHeader("Authorization","Bearer "+Match3DInvitations.Instance.RecordToken);
            yield return req.SendWebRequest();Reply reply=null;
            try{reply=JsonUtility.FromJson<Reply>(req.downloadHandler.text);}catch{}
            if(req.responseCode!=200&&reply!=null)reply.ok=false;
            if(req.responseCode==401&&Match3DInvitations.Instance)Match3DInvitations.Instance.RefreshRecordLogin();
            done(SameOwner()?reply:null);
        }
    }
}
