using System;
using System.Runtime.InteropServices;
using UnityEngine;

// The native JS outbox owns persistence; this component only supplies current auth/context.
public sealed class Match3DAdTelemetry : MonoBehaviour {
    [Serializable] public class Context {
        public int schemaVersion=1,stage;
        public string eventType="completed",owner,impressionId,recordId,runId,placement,adUnitId,clientVersion,platform;
        public long occurredAt;
    }
    public static Match3DAdTelemetry Instance {get;private set;}
    string endpoint,lastOwner,lastToken;
    bool bound;
    string lastWarning;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern void GooseAdTelemetryBind(string target,string url,string owner,string token);
#endif
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');SyncSession();}
    void Update(){SyncSession();}
    void OnDestroy(){if(Instance==this)Instance=null;}
    public void SyncSession(){
        var account=Match3DInvitations.Instance;
        string owner=account&&account.Ready?account.State.inviteCode:"",token=account&&account.Ready?account.RecordToken:"";
        if(bound&&owner==lastOwner&&token==lastToken)return;
        lastOwner=owner;lastToken=token;bound=true;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        GooseAdTelemetryBind(gameObject.name,endpoint,owner,token);
#endif
    }
    public string Capture(string request,string adUnit,string placement){
        SyncSession();
        var records=Match3DGameRecords.Instance;
        if(string.IsNullOrEmpty(lastOwner)||string.IsNullOrEmpty(lastToken)||!records)return null;
        var context=records.AdContext(lastOwner);
        if(context==null)return null;
        context.impressionId=request;context.adUnitId=adUnit;context.placement=placement;
        return JsonUtility.ToJson(context);
    }
    [UnityEngine.Scripting.Preserve] public void OnAdTelemetryAuthExpired(string owner){
        var account=Match3DInvitations.Instance;
        if(account&&account.Ready&&account.State.inviteCode==owner){account.RefreshRecordLogin();bound=false;SyncSession();}
    }
    [UnityEngine.Scripting.Preserve] public void OnAdTelemetryWarning(string warning){
        if(warning==lastWarning)return;lastWarning=warning;Debug.LogWarning(warning);
        if(Match3DFlow.Instance)Match3DFlow.Instance.Notice(warning);
    }
}
