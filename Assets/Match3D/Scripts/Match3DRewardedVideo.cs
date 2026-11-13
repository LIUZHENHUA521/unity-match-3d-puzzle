using System;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class Match3DRewardedVideo : MonoBehaviour {
    [Serializable] public class Configuration {public string clearAdUnitId,timeAdUnitId,toolAdUnitId;}
    [Serializable] class Reply {public string requestId,error;public bool completed;}
    Action<bool,string> callback;string requestId;
#if UNITY_EDITOR
    public static Action<Action<bool,string>> EditorShowOverride;
    public static Action<Action<bool,string>> EditorShareOverride;
#endif
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern void GooseReviveAdShow(string target,string id,string request,string context);
    [DllImport("__Internal")] static extern void GooseReviveAdCancel(string request);
    [DllImport("__Internal")] static extern void GooseRewardShare(string target,string request,float minAwaySeconds);
    [DllImport("__Internal")] static extern void GooseRewardShareCancel(string request);
#endif
    public void Acquire(Match3DRewardAcquisition.Offer offer,bool timeout,Action<bool,string> done,int toolType=0){
        if(!Match3DRewardAcquisition.IsCurrent(offer)){done(false,"获取状态已变化，请关闭后重试");return;}
        switch(offer.route){
            case Match3DRewardAcquisition.Route.Free:done(true,null);return;
            case Match3DRewardAcquisition.Route.Share:ShowShare(done);return;
            case Match3DRewardAcquisition.Route.Ad:ShowVideo(timeout,offer.entry==Match3DRewardAcquisition.Entry.Tool,done,toolType);return;
            default:done(false,"暂时无法获取，请稍后再试");return;
        }
    }
    void ShowShare(Action<bool,string> done){
        if(callback!=null){done(false,"正在获取中，请稍候");return;}
        requestId=Guid.NewGuid().ToString("N");callback=done;
#if UNITY_EDITOR
        if(EditorShareOverride!=null){string request=requestId;EditorShareOverride((ok,error)=>Complete(request,ok,error));return;}
#endif
#if UNITY_EDITOR || DAILY_WEB_PREVIEW
        if(Match3DFlow.Instance&&Match3DFlow.Instance.LocalPreview){Complete(requestId,true,null);return;}
#endif
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        GooseRewardShare(gameObject.name,requestId,Mathf.Max(0,Match3DRewardAcquisition.Config.minShareAwaySeconds));
#else
        Complete(requestId,false,"请在微信小游戏中分享");
#endif
    }
    public void Show(bool timeout,Action<bool,string> done){ShowVideo(timeout,false,done);}
    public void ShowTool(Action<bool,string> done){ShowVideo(false,true,done);}
    void ShowVideo(bool timeout,bool tool,Action<bool,string> done,int toolType=0){
        if(callback!=null){done(false,"广告正在播放，请稍候");return;}
        requestId=Guid.NewGuid().ToString("N");callback=done;
#if UNITY_EDITOR
        if(EditorShowOverride!=null){string request=requestId;EditorShowOverride((ok,error)=>Complete(request,ok,error));return;}
#endif
#if UNITY_EDITOR || DAILY_WEB_PREVIEW
        if(Match3DFlow.Instance&&Match3DFlow.Instance.LocalPreview){Complete(requestId,true,null);return;}
#endif
        var asset=Resources.Load<TextAsset>("Match3D/RewardedVideoConfig");
        var config=asset?JsonUtility.FromJson<Configuration>(asset.text):null;
        string id=config==null?null:tool?config.toolAdUnitId:timeout?config.timeAdUnitId:config.clearAdUnitId;
        if(string.IsNullOrWhiteSpace(id)||!id.StartsWith("adunit-",StringComparison.Ordinal)){Complete(requestId,false,"激励广告尚未配置，请稍后再试");return;}
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        if(tool&&(toolType<0||toolType>3)){Complete(requestId,false,"广告用途无效，请重试");return;}
        string placement=tool?new[]{"tool_remove","tool_magnet","tool_shuffle","tool_time"}[toolType]:timeout?"revive_time":"revive_clear";
        var telemetry=Match3DAdTelemetry.Instance;
        string context=telemetry?telemetry.Capture(requestId,id,placement):null;
        if(string.IsNullOrEmpty(context)){Complete(requestId,false,"账号或关卡尚未准备好，请稍后重试");return;}
        GooseReviveAdShow(gameObject.name,id,requestId,context);
#else
        Complete(requestId,false,"请在微信小游戏中观看视频");
#endif
    }
    [UnityEngine.Scripting.Preserve] public void OnReviveAdReply(string json){
        Reply reply;try{reply=JsonUtility.FromJson<Reply>(json);}catch{return;}
        if(reply!=null)Complete(reply.requestId,reply.completed,reply.error);
    }
    void Complete(string request,bool completed,string error){
        if(callback==null||request!=requestId)return;
        var done=callback;callback=null;requestId=null;done(completed,error);
    }
    public void Cancel(){
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        if(requestId!=null){GooseReviveAdCancel(requestId);GooseRewardShareCancel(requestId);}
#endif
        callback=null;requestId=null;
    }
    void OnDestroy(){Cancel();}
}
