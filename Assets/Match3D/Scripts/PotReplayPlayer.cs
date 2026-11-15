using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(15000)]
public sealed class PotReplayPlayer : MonoBehaviour {
    public static PotReplayPlayer Instance {get;private set;}
    public static bool Active=>Instance&&Instance.data!=null;
    PotReplayData data;float time,speed=1;bool playing,ready;bool seeking,settingsOpen,scrubbing,resumeAfterScrub;float presentationTime=float.NaN;int currentFrame=-1;
    Dictionary<int,PotReplayPose> state;readonly Dictionary<int,GameObject> objects=new Dictionary<int,GameObject>();
    readonly Dictionary<int,Renderer[]> renderers=new Dictionary<int,Renderer[]>();
    Dictionary<int,PotReplayPose> next;
    readonly List<Behaviour> disabled=new List<Behaviour>();PotReplayProgress slider;
    PotReplayGestures gestures;
    PotReplayAudio audio;float audioTail;
    public float Duration=>data?.duration??0;
    public float CurrentTime=>time;
    public float Speed=>speed;
    public bool SettingsOpen=>settingsOpen;
    public bool ControlsEnabled=>ready&&!settingsOpen;
    public bool IsPlaying=>ready&&playing&&!settingsOpen&&!scrubbing;
    public void Load(PotReplayData value){
        if(Active)throw new InvalidOperationException("请先关闭当前回放");
        if(value==null||!value.Supported||value.frames==null||value.frames.Count==0||value.level==null||!Match3DModelThemes.TryGet(value.level.modelThemeId,Match3DFlow.Instance.SourceEnvironmentForTheme(0),out var profile)||value.opening==null||!value.opening.IsValid(value.stage==1?15:value.level.initialCount,profile.prefabs.Length))throw new InvalidOperationException("录像版本或资源不兼容，请更新游戏");
        Instance=this;data=value;time=audioTail=0;speed=1;ready=playing=settingsOpen=scrubbing=false;StartCoroutine(Prepare());
    }
    IEnumerator Prepare(){
        Exception failure=null;yield return Match3DStartupRunner.Run(PrepareCore(),e=>failure=e);
        if(failure!=null){Debug.LogWarning("Pot replay load: "+failure.Message);Exit(false);Match3DFlow.Instance.Notice("回放加载失败："+failure.Message);
#if DAILY_WEB_PREVIEW
            if(DailyWebPreview.Instance)DailyWebPreview.Instance.PotReplayError(failure.Message);
#endif
        }
    }
    IEnumerator PrepareCore(){
        var flow=Match3DFlow.Instance;flow.BeginPotReplay(data.level,data.stage,data.opening);
        float deadline=Time.realtimeSinceStartup+90;
        while(flow.CreationInProgress){if(Time.realtimeSinceStartup>deadline){Exit();flow.Notice("回放加载超时");yield break;}yield return null;}
        flow.LockPotReplay();
        Disable(InputController.Instance);Disable(LayeredItemPile.Instance);Disable(ItemPhysicsController.Instance);Disable(GameManager.Instance);
        foreach(var obj in LevelGenerator.Instance.objectsOnScene){
            if(!obj)continue;int id=obj.GetComponent<Match3DTile>().id;objects.Add(id,obj);renderers.Add(id,obj.GetComponentsInChildren<Renderer>(true));
            foreach(var body in obj.GetComponentsInChildren<Rigidbody>()){body.isKinematic=true;body.detectCollisions=false;}
            foreach(var behaviour in obj.GetComponentsInChildren<MonoBehaviour>())Disable(behaviour);
            foreach(var animator in obj.GetComponentsInChildren<Animator>())Disable(animator);
        }
        audio=new PotReplayAudio(data,objects);
        Time.timeScale=0;ready=true;playing=true;Controls();Apply(0);flow.CompletePotReplayLoading();
#if DAILY_WEB_PREVIEW
        if(DailyWebPreview.Instance)DailyWebPreview.Instance.PotReplayReady();
#endif
    }
    void Disable(Behaviour b){if(b&&b.enabled){b.enabled=false;disabled.Add(b);}}
    void LateUpdate(){if(!ready)return;if(IsPlaying)time=Mathf.Min(Duration,time+Time.unscaledDeltaTime*speed);else if(time>=Duration&&!settingsOpen&&!scrubbing)audioTail+=Time.unscaledDeltaTime*speed;if(time>=Duration&&playing){playing=false;presentationTime=float.NaN;}Apply(time);}
    public void Seek(float value){seeking=true;presentationTime=float.NaN;time=Mathf.Clamp(value,0,Duration);audioTail=0;audio?.Seek(time);currentFrame=-1;if(ready)Apply(time);seeking=false;}
    public void SetSpeed(float value){if(ControlsEnabled)speed=Mathf.Clamp(value,.5f,3);}
    public void SetSettingsOpen(bool value){
        if(!ready)return;
        if(value&&scrubbing)EndScrub();
        settingsOpen=value;presentationTime=float.NaN;
        if(slider)slider.interactable=!value;
        Apply(time);
    }
    public void BeginScrub(){
        if(!ControlsEnabled||scrubbing)return;
        resumeAfterScrub=playing||time>=Duration;scrubbing=true;presentationTime=float.NaN;Apply(time);
    }
    public void EndScrub(){
        if(!scrubbing)return;
        scrubbing=false;playing=resumeAfterScrub&&time<Duration;presentationTime=float.NaN;
        if(ready)Apply(time);
    }
    public void Pause(){if(!ControlsEnabled)return;playing=!playing;presentationTime=float.NaN;if(playing&&time>=Duration)Seek(0);if(ready)Apply(time);}
    void Apply(float value){
        int frame=0;for(int i=1;i<data.frames.Count&&data.frames[i].t<=value;i++)frame=i;
        bool changed=frame!=currentFrame;
        if(changed){state=PotReplayState.At(data,value,out currentFrame);next=data.frames[Mathf.Min(frame+1,data.frames.Count-1)].items.ToDictionary(x=>x.id);}
        var a=data.frames[frame];var b=data.frames[Mathf.Min(frame+1,data.frames.Count-1)];float blend=b.t>a.t?Mathf.Clamp01((value-a.t)/(b.t-a.t)):0;
        
        foreach(var pair in changed?state:next){
            if(!objects.TryGetValue(pair.Key,out var obj)||!obj)continue;var p=state[pair.Key];
            bool interpolate=next.TryGetValue(pair.Key,out var q)&&q.visible&&p.visible&&p.zone==q.zone;
            obj.transform.SetPositionAndRotation(interpolate?Vector3.Lerp(p.p,q.p,blend):p.p,interpolate?Quaternion.Slerp(p.r,q.r,blend):p.r);
            obj.transform.localScale=interpolate?Vector3.Lerp(p.s,q.s,blend):p.s;
            if(changed){var list=renderers[pair.Key];for(int i=0;i<list.Length;i++)if(list[i])list[i].enabled=p.visible&&(p.render==null||p.render.Length==0||i<p.render.Length&&p.render[i]);}
        }
        var camera=Camera.main;camera.transform.SetPositionAndRotation(Vector3.Lerp(a.cameraPosition,b.cameraPosition,blend),Quaternion.Slerp(a.cameraRotation,b.cameraRotation,blend));camera.fieldOfView=Mathf.Lerp(a.fov,b.fov,blend);camera.orthographic=a.orthographic;camera.orthographicSize=Mathf.Max(.01f,Mathf.Lerp(a.cameraSize,b.cameraSize,blend));
        if(a.presentation!=null&&presentationTime!=value){
            float offset=Mathf.Max(0,value-a.t);var probe=ComboProbeController.Instance;
            BoardToolVisual.ApplyReplay(a.presentation,offset);ItemHighlight.For(camera).ApplyReplay(a.presentation,objects,offset);
            if(probe)probe.ApplyReplay(a.presentation,objects,offset);
            PotProbeActivation.ApplyReplay(a.presentation.activations,objects,offset,IsPlaying&&!seeking,speed);
            if(probe&&probe.View&&a.presentation.probe&&b.presentation!=null&&b.presentation.probe&&a.presentation.combo==b.presentation.combo&&a.presentation.stars==b.presentation.stars){
                var chances=new float[13];for(int i=0;i<13;i++)chances[i]=Mathf.Lerp(a.presentation.chance[i],b.presentation.chance[i],blend);probe.View.Chances.ApplyReplay(chances,a.presentation.combo);
            }
            presentationTime=value;
        }
        GameManager.Instance.currentLevelTime=a.remaining;
        Match3DFlow.Instance.UpdatePotReplayTime(a.remaining);
        if(slider)slider.SetValueWithoutNotify(Duration>0?value/Duration:0);
        gestures?.Apply(value);
        // The final frame stays still while the short completion sound finishes.
        audio?.Apply(value+audioTail,!seeking&&!settingsOpen&&!scrubbing&&(IsPlaying||value>=Duration),speed);
    }
    void Controls(){
        var flow=Match3DFlow.Instance;
        slider=PotReplayUI.Progress(flow.PotReplayHeader,new Vector2(0,-192));
        slider.Connect(this);slider.onValueChanged.AddListener(v=>{if(ControlsEnabled)Seek(v*Duration);});
        gestures=new PotReplayGestures(data,(RectTransform)flow.PotReplayHeader.parent.parent);
    }
    public void Exit(){Exit(true);}
    void Exit(bool notify){
        gestures?.Dispose();gestures=null;
        audio?.Dispose();audio=null;
        if(Camera.main)ItemHighlight.For(Camera.main).ClearAll();PotProbeActivation.Clear();BoardToolVisual.Reset();ComboProbeController.End();presentationTime=float.NaN;
        StopAllCoroutines();ready=playing=settingsOpen=scrubbing=false;foreach(var b in disabled)if(b)b.enabled=true;disabled.Clear();objects.Clear();renderers.Clear();state=null;next=null;data=null;currentFrame=-1;Time.timeScale=1;
        Match3DFlow.Instance.EndPotReplay();
#if DAILY_WEB_PREVIEW
        if(notify&&DailyWebPreview.Instance)DailyWebPreview.Instance.PotReplayClosed();
#endif
    }
    void OnDestroy(){if(Instance==this)Instance=null;}
}
