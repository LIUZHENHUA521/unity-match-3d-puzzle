using System;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class Match3DRewardedVideo : MonoBehaviour {
    [Serializable] public class Configuration {public string clearAdUnitId,timeAdUnitId,toolAdUnitId;}
    [Serializable] class Reply {public string requestId,error;public bool completed;}
    Action<bool,string> callback;string requestId;
#if UNITY_EDITOR
    public static Action<Action<bool,string>> EditorShowOverride;
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern void GooseReviveAdShow(string target,string id,string request);
    [DllImport("__Internal")] static extern void GooseReviveAdCancel(string request);
#endif
    public void Show(bool timeout,Action<bool,string> done){ShowVideo(timeout,false,done);}
    public void ShowTool(Action<bool,string> done){ShowVideo(false,true,done);}
    void ShowVideo(bool timeout,bool tool,Action<bool,string> done){
        if(callback!=null){done(false,"广告正在播放，请稍候");return;}
        requestId=Guid.NewGuid().ToString("N");callback=done;
#if UNITY_EDITOR
        if(EditorShowOverride!=null){string request=requestId;EditorShowOverride((ok,error)=>Complete(request,ok,error));return;}
        if(Match3DFlow.Instance&&Match3DFlow.Instance.LocalPreview){Complete(requestId,true,null);return;}
#endif
        var asset=Resources.Load<TextAsset>("Match3D/RewardedVideoConfig");
        var config=asset?JsonUtility.FromJson<Configuration>(asset.text):null;
        string id=config==null?null:tool?config.toolAdUnitId:timeout?config.timeAdUnitId:config.clearAdUnitId;
        if(string.IsNullOrWhiteSpace(id)||!id.StartsWith("adunit-",StringComparison.Ordinal)){Complete(requestId,false,"激励广告尚未配置，请稍后再试");return;}
#if UNITY_WEBGL && !UNITY_EDITOR
        GooseReviveAdShow(gameObject.name,id,requestId);
#else
        Complete(requestId,false,"请在微信小游戏中观看视频");
#endif
    }
    public void OnReviveAdReply(string json){
        Reply reply;try{reply=JsonUtility.FromJson<Reply>(json);}catch{return;}
        if(reply!=null)Complete(reply.requestId,reply.completed,reply.error);
    }
    void Complete(string request,bool completed,string error){
        if(callback==null||request!=requestId)return;
        var done=callback;callback=null;requestId=null;done(completed,error);
    }
    public void Cancel(){
#if UNITY_WEBGL && !UNITY_EDITOR
        if(requestId!=null)GooseReviveAdCancel(requestId);
#endif
        callback=null;requestId=null;
    }
    void OnDestroy(){Cancel();}
}
