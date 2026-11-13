using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(15000)]
public sealed class PotReplayPlayer : MonoBehaviour {
    public static PotReplayPlayer Instance {get;private set;}
    public static bool Active=>Instance&&Instance.data!=null;
    PotReplayData data;float time,speed=1;bool playing,ready;int currentFrame=-1;
    Dictionary<int,PotReplayPose> state;readonly Dictionary<int,GameObject> objects=new Dictionary<int,GameObject>();
    readonly Dictionary<int,Renderer[]> renderers=new Dictionary<int,Renderer[]>();
    Dictionary<int,PotReplayPose> next;
    readonly List<Behaviour> disabled=new List<Behaviour>();Text readout;Slider slider;
    public float Duration=>data?.duration??0;
    public void Load(PotReplayData value){
        if(Active)throw new InvalidOperationException("请先关闭当前回放");
        if(value==null||!value.Supported||value.frames==null||value.frames.Count==0||value.level==null||!Match3DModelThemes.TryGet(value.level.modelThemeId,Match3DFlow.Instance.SourceEnvironmentForTheme(0),out var profile)||value.opening==null||!value.opening.IsValid(value.stage==1?15:value.level.initialCount,profile.prefabs.Length))throw new InvalidOperationException("录像版本或资源不兼容，请更新游戏");
        Instance=this;data=value;time=0;speed=1;ready=false;StartCoroutine(Prepare());
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
        while(flow.CreationInProgress){if(Time.realtimeSinceStartup>deadline){flow.Notice("回放加载超时");Exit();yield break;}yield return null;}
        flow.LockPotReplay();
        Disable(InputController.Instance);Disable(LayeredItemPile.Instance);Disable(ItemPhysicsController.Instance);Disable(GameManager.Instance);
        foreach(var obj in LevelGenerator.Instance.objectsOnScene){
            if(!obj)continue;int id=obj.GetComponent<Match3DTile>().id;objects.Add(id,obj);renderers.Add(id,obj.GetComponentsInChildren<Renderer>(true));
            foreach(var body in obj.GetComponentsInChildren<Rigidbody>()){body.isKinematic=true;body.detectCollisions=false;}
            foreach(var behaviour in obj.GetComponentsInChildren<MonoBehaviour>())Disable(behaviour);
            foreach(var animator in obj.GetComponentsInChildren<Animator>())Disable(animator);
        }
        Time.timeScale=0;ready=true;playing=true;Controls();Apply(0);
#if DAILY_WEB_PREVIEW
        if(DailyWebPreview.Instance)DailyWebPreview.Instance.PotReplayReady();
#endif
    }
    void Disable(Behaviour b){if(b&&b.enabled){b.enabled=false;disabled.Add(b);}}
    void LateUpdate(){if(!ready)return;if(playing)time=Mathf.Min(Duration,time+Time.unscaledDeltaTime*speed);Apply(time);if(time>=Duration)playing=false;}
    public void Seek(float value){time=Mathf.Clamp(value,0,Duration);currentFrame=-1;if(ready)Apply(time);}
    public void SetSpeed(float value){speed=Mathf.Clamp(value,.5f,3);}
    public void Pause(){playing=!playing;if(playing&&time>=Duration)Seek(0);}
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
        GameManager.Instance.currentLevelTime=a.remaining;
        if(readout)readout.text=$"{(int)value / 60:00}:{(int)value % 60:00} / {(int)Duration / 60:00}:{(int)Duration % 60:00}    {speed:0.#}×\n剩余 {Mathf.CeilToInt(a.remaining)} 秒 · 槽位 {a.tray.Length}/7";
        if(slider)slider.SetValueWithoutNotify(Duration>0?value/Duration:0);
    }
    void Controls(){
        var root=Match3DFlow.Instance.PotReplayOverlay(false);
        readout=PotReplayUI.Label(root,"每日挑战回放",new Vector2(0,-40),new Vector2(650,90));
        PotReplayUI.Button(root,"暂停 / 继续",new Vector2(-190,-145),()=>Pause());
        PotReplayUI.Button(root,"倍速",new Vector2(0,-145),()=>SetSpeed(speed>=3?.5f:speed+.5f));
        PotReplayUI.Button(root,"退出回放",new Vector2(190,-145),Exit);
        slider=PotReplayUI.Progress(root,new Vector2(0,-215));slider.onValueChanged.AddListener(v=>Seek(v*Duration));
    }
    public void Exit(){Exit(true);}
    void Exit(bool notify){
        StopAllCoroutines();ready=playing=false;foreach(var b in disabled)if(b)b.enabled=true;disabled.Clear();objects.Clear();renderers.Clear();state=null;next=null;data=null;currentFrame=-1;Time.timeScale=1;
        Match3DFlow.Instance.EndPotReplay();
#if DAILY_WEB_PREVIEW
        if(notify&&DailyWebPreview.Instance)DailyWebPreview.Instance.PotReplayClosed();
#endif
    }
    void OnDestroy(){if(Instance==this)Instance=null;}
}
