using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

[DefaultExecutionOrder(10000)]
public sealed class PotReplayRecorder : MonoBehaviour {
    public static PotReplayRecorder Instance {get;private set;}
    PotReplayData data;long writtenBytes;string owner,journal;float elapsed,nextSample,lastCheckpoint;int journalEvents,frameCount;bool hidden,eventDirty;
    readonly StringBuilder pendingFrames=new StringBuilder();
    readonly Dictionary<int,GameObject> objects=new Dictionary<int,GameObject>();
    readonly Dictionary<int,Renderer[]> renderers=new Dictionary<int,Renderer[]>();
    readonly Dictionary<int,PotReplayPose> previous=new Dictionary<int,PotReplayPose>();
    public static bool Recording=>Instance&&Instance.data!=null;
    public static void Event(string type,int id=-1,int value=0){var self=Instance;if(self?.data==null)return;if(self.data.events.Count>=49999&&type!="finish"){self.Abort(new Exception("录像操作事件超过上限"));return;}self.eventDirty=true;self.data.events.Add(new PotReplayEvent{seq=self.data.events.Count,t=self.elapsed,type=type,id=id,value=value});}
    void Awake(){Instance=this;Application.lowMemory+=OnLowMemory;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        WeChatWASM.WX.OnMemoryWarning(OnWechatMemory);WeChatWASM.WX.OnHide(OnHide);WeChatWASM.WX.OnShow(OnShow);
#endif
    }
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    void OnWechatMemory(WeChatWASM.OnMemoryWarningListenerResult value){OnLowMemory();}
    void OnHide(WeChatWASM.GeneralCallbackResult data){OnApplicationPause(true);}
    void OnShow(WeChatWASM.OnShowListenerResult data){OnApplicationPause(false);}
#endif
    public void Begin(Match3DGameRecords.Record record,Match3DDailyLevel level){
        if(record==null||record.stage!=2||data!=null||level==null||Match3DFlow.Instance.LocalPreview||PotReplayPlayer.Active)return;
        var account=Match3DInvitations.Instance;if(!account||!account.Ready)return;
        try{
            if(PotReplayFiles.List().Sum(x=>PotReplayFiles.Size(x))>128*1024*1024)throw new Exception("录像本地缓存已满，请联网同步");
            owner=account.State.inviteCode;pendingFrames.Clear();
            data=new PotReplayData{opening=Match3DFlow.Instance.CaptureDailyOpening(),recordId=record.recordId,runId=record.runId,stage=record.stage,level=JsonUtility.FromJson<Match3DDailyLevel>(JsonUtility.ToJson(level))};
            objects.Clear();renderers.Clear();previous.Clear();
            foreach(var obj in LevelGenerator.Instance.objectsOnScene){if(!obj)continue;var tile=obj.GetComponent<Match3DTile>();objects.Add(tile.id,obj);renderers.Add(tile.id,obj.GetComponentsInChildren<Renderer>(true));}
            if(objects.Count!=record.initialCount)throw new Exception("initial object count mismatch");
            journalEvents=frameCount=0;elapsed=nextSample=lastCheckpoint=0;hidden=false;
            journal=PotReplayService.PathFor(owner,record.recordId,".recording");
            string header=JsonUtility.ToJson(data)+"\n";PotReplayFiles.Write(journal,header);writtenBytes=Encoding.UTF8.GetByteCount(header);Capture(true);
        }catch(Exception e){Abort(e);}
    }
    void LateUpdate(){
        if(data==null||hidden)return;
        var flow=Match3DFlow.Instance;
        if(!flow||flow.CreationInProgress||flow.BlocksGameplay||Time.timeScale<=0)return;
        elapsed+=Time.unscaledDeltaTime;
        if(elapsed>7200||frameCount>=72000){Abort(new Exception("录像超过时长或帧数上限"));return;}
        if(elapsed<nextSample&&!eventDirty)return;eventDirty=false;nextSample=elapsed+.1f;
        try{Capture(elapsed-lastCheckpoint>=5);}catch(Exception e){Abort(e);}
    }
    void Capture(bool full){
        var camera=Camera.main;var input=InputController.Instance;
        var frame=new PotReplayFrame{t=elapsed,full=full,remaining=Mathf.Max(0,GameManager.Instance.currentLevelTime),cameraPosition=camera.transform.position,cameraRotation=camera.transform.rotation,fov=camera.fieldOfView,cameraSize=camera.orthographicSize,orthographic=camera.orthographic,tray=input.objectsOnStack.Where(x=>x).Select(x=>x.GetComponent<Match3DTile>().id).ToArray()};
        foreach(var pair in objects){
            var obj=pair.Value;var old=previous.TryGetValue(pair.Key,out var cached)?cached:null;
            var pose=obj?new PotReplayPose{id=pair.Key,render=renderers[pair.Key].Select(r=>r&&r.enabled&&r.gameObject.activeInHierarchy).ToArray(),p=obj.transform.position,r=obj.transform.rotation,s=obj.transform.localScale,visible=obj.activeInHierarchy&&renderers[pair.Key].Any(r=>r&&r.enabled&&r.gameObject.activeInHierarchy),zone=input.objectsOnStack.Contains(obj)?1:BoardTools.IsStored(obj)?2:LevelGenerator.Instance.objectsOnScene.Contains(obj)?0:3}:new PotReplayPose{id=pair.Key,render=new bool[renderers[pair.Key].Length],p=old?.p??Vector3.zero,r=old?.r??Quaternion.identity,s=old?.s??Vector3.one,zone=3,visible=false};
            // Pooled objects remain allocated after being removed; disappearance is captured by visibility.
            if(full||!pose.Same(old))frame.items.Add(pose);previous[pair.Key]=pose;
        }
        frame.presentation=PotReplayPresentation.Capture();frame.events=data.events.Skip(journalEvents).ToArray();journalEvents=data.events.Count;
        frameCount++;
        pendingFrames.AppendLine(JsonUtility.ToJson(frame));
        if(full||pendingFrames.Length>=32768)Flush();if(full)lastCheckpoint=elapsed;
        if(writtenBytes+pendingFrames.Length>PotReplayService.MaxJsonBytes)throw new Exception("录像超过64MiB保存上限");
    }
    public void Finish(string status){
        if(data==null)return;
        try{Event("finish");Capture(true);data.duration=elapsed;data.status=status;PotReplayJournal.Ready(journal,data,frameCount);PotReplayService.Instance.Queued();data=null;objects.Clear();renderers.Clear();previous.Clear();pendingFrames.Clear();}
        catch(Exception e){Abort(e);}
    }
    void OnLowMemory(){if(data!=null)Abort(new Exception("设备内存紧张，已停止录制以保护对局"));}
    void Abort(Exception error){Debug.LogWarning("Pot replay unavailable: "+error.Message);PotReplayService.Instance?.RecordingFailed(error.Message);data=null;objects.Clear();renderers.Clear();previous.Clear();pendingFrames.Clear();}
    void Flush(){if(pendingFrames.Length==0||string.IsNullOrEmpty(journal))return;string text=pendingFrames.ToString();PotReplayFiles.Append(journal,text);writtenBytes+=Encoding.UTF8.GetByteCount(text);pendingFrames.Clear();}
    void OnApplicationPause(bool paused){hidden=paused;if(paused&&data!=null)try{Flush();}catch(Exception e){Abort(e);}}
    void OnApplicationFocus(bool focus){OnApplicationPause(!focus);}
    void OnDestroy(){
        Application.lowMemory-=OnLowMemory;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        WeChatWASM.WX.OffMemoryWarning(OnWechatMemory);WeChatWASM.WX.OffHide(OnHide);WeChatWASM.WX.OffShow(OnShow);
#endif
        if(Instance==this)Instance=null;
    }
}
