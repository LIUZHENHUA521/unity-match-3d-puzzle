using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Presentation derived from recorded actions, never an input source or gameplay driver.
public sealed class PotReplayGestures {
    public enum Target { Item, Tool, Probe }
    public sealed class Cue {
        public Target target;public int id,index,seq;public float time,end;public bool preview;
        public Vector3 position;
    }
    public const float MoveSeconds=.14f,DownSeconds=.06f,UpSeconds=.08f,TapScale=.92f;
    public readonly List<Cue> cues=new List<Cue>();
    readonly RectTransform layer,hand,circle;
    readonly Canvas canvas;
    readonly Camera camera;

    public PotReplayGestures(PotReplayData data,RectTransform parent){
        Build(data);
        layer=new GameObject("Pot replay gestures",typeof(RectTransform)).GetComponent<RectTransform>();
        layer.SetParent(parent,false);layer.anchorMin=Vector2.zero;layer.anchorMax=Vector2.one;layer.offsetMin=layer.offsetMax=Vector2.zero;
        canvas=parent.GetComponentInParent<Canvas>();camera=Camera.main;
        hand=new GameObject("Pot replay hand",typeof(RectTransform)).GetComponent<RectTransform>();hand.SetParent(layer,false);
        hand.anchorMin=hand.anchorMax=hand.pivot=new Vector2(.5f,.5f);hand.sizeDelta=Vector2.zero;
        var halo=new GameObject("Pot replay circle",typeof(RectTransform),typeof(Image)).GetComponent<Image>();
        halo.transform.SetParent(hand,false);halo.sprite=Resources.Load<Sprite>("Match3D/PotReplay/PotReplayCircle");
        if(!halo.sprite)throw new InvalidOperationException("Missing pot replay circle art");
        halo.raycastTarget=false;circle=halo.rectTransform;circle.sizeDelta=new Vector2(81,84);
        var icon=new GameObject("Pot replay hand icon",typeof(RectTransform),typeof(Image)).GetComponent<Image>();
        icon.transform.SetParent(hand,false);icon.sprite=Resources.Load<Sprite>("Match3D/PotReplay/PotReplayHand");
        if(!icon.sprite)throw new InvalidOperationException("Missing pot replay hand art");
        icon.raycastTarget=false;
        // Exact size, offset and scale from crazyCombo's movefinger/Node scene binding.
        icon.rectTransform.sizeDelta=new Vector2(143,152);icon.rectTransform.anchoredPosition=new Vector2(34.941f,-45.246f);icon.rectTransform.localScale=Vector3.one*.8f;
        hand.gameObject.SetActive(false);
    }

    void Build(PotReplayData data){
        var poses=new Dictionary<int,Vector3>();foreach(var item in data.opening.items)poses[item.id]=item.position;
        int frame=0;PotReplayPresentation presentation=null;Cue preview=null;
        foreach(var e in data.events.OrderBy(e=>e.t).ThenBy(e=>e.seq)){
            // Replay frames are deltas. Retain all earlier item poses, not just the last frame's changes.
            while(frame<data.frames.Count&&data.frames[frame].t<=e.t){
                var f=data.frames[frame++];foreach(var item in f.items)poses[item.id]=item.p;presentation=f.presentation;
            }
            if(e.type=="probe_preview"){
                if(e.value==0){if(preview!=null){preview.end=Mathf.Max(preview.time,e.t);preview=null;}continue;}
                int column=Column(presentation,e.id);
                if(column<0)continue;
                if(preview!=null)preview.end=Mathf.Max(preview.time,e.t);
                preview=new Cue{target=Target.Probe,id=e.id,index=column,time=e.t,end=data.duration,seq=e.seq,preview=true};cues.Add(preview);continue;
            }
            if(e.type=="probe_use"){
                var last=cues.LastOrDefault();
                // Preview end can mean cancellation. Only an actual use confirms a successful tap.
                if(last!=null&&last.preview&&last.id==e.id&&Mathf.Abs(last.end-e.t)<.05f){last.preview=false;last.end=e.t;continue;}
                if(e.value>=0&&e.value<4)cues.Add(new Cue{target=Target.Probe,id=e.id,index=e.value,time=e.t,end=e.t,seq=e.seq});
            }else if(e.type=="tool"&&e.value>=0&&e.value<BoardTools.ToolCount){
                cues.Add(new Cue{target=Target.Tool,index=e.value,time=e.t,end=e.t,seq=e.seq});
            }else if(e.type=="pick"&&e.value==0&&poses.TryGetValue(e.id,out var position)){
                cues.Add(new Cue{target=Target.Item,id=e.id,position=position,time=e.t,end=e.t,seq=e.seq});
            }
        }
    }
    static int Column(PotReplayPresentation p,int id){
        if(p?.columns==null)return -1;
        for(int i=0;i<p.columns.Length;i++)if(p.columns[i].ids.Contains(id))return i;
        return -1;
    }
    static float Ease(float t){t=Mathf.Clamp01(t);return 1-(1-t)*(1-t);}
    public static float Scale(Cue cue,float time){
        float elapsed=time-cue.time;
        if(elapsed<0)return 1;
        if(elapsed<DownSeconds)return Mathf.Lerp(1,TapScale,Ease(elapsed/DownSeconds));
        float release=Mathf.Max(cue.time+DownSeconds,cue.end);
        return time<release?TapScale:Mathf.Lerp(TapScale,1,Ease((time-release)/UpSeconds));
    }
    float Start(int i)=>Mathf.Min(cues[i].time,Mathf.Max(0,cues[i].time-MoveSeconds,i>0?cues[i-1].time+DownSeconds:0));
    public int CueAt(float time){
        int lo=0,hi=cues.Count-1,result=-1;
        while(lo<=hi){int mid=(lo+hi)/2;if(Start(mid)<=time){result=mid;lo=mid+1;}else hi=mid-1;}
        // Keep the hand at the last operation during idle gaps and after the final operation.
        return cues.Count==0?-1:Mathf.Max(0,result);
    }
    // crazyCombo bubble.anim: 4/3 clip seconds at speed .5, linear 1 -> 1.1 -> 1.
    public static float CircleScale(float time)=>1+.1f*Mathf.PingPong(Mathf.Max(0,time)*.75f,1);
    bool Position(Cue cue,out Vector2 position){
        position=default;Vector2 screen;
        if(cue.target==Target.Item){
            var projected=camera.WorldToScreenPoint(cue.position);if(projected.z<=0)return false;screen=projected;
        }else{
            RectTransform target=null;
            if(cue.target==Target.Tool)target=Match3DFlow.Instance.PotReplayToolTarget(cue.index);
            else if(ComboProbeController.Instance?.View)target=(RectTransform)ComboProbeController.Instance.View.Slot(cue.index).transform;
            if(!target||!target.gameObject.activeInHierarchy)return false;
            var targetCanvas=target.GetComponentInParent<Canvas>();
            screen=RectTransformUtility.WorldToScreenPoint(targetCanvas&&targetCanvas.renderMode!=RenderMode.ScreenSpaceOverlay?targetCanvas.worldCamera:null,target.TransformPoint(target.rect.center));
        }
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(layer,screen,canvas&&canvas.renderMode!=RenderMode.ScreenSpaceOverlay?canvas.worldCamera:null,out position);
    }
    public void Apply(float time){
        int i=CueAt(time);Vector2 point=default;
        // A temporarily unavailable target must not blink the cursor out of existence.
        while(i>=0&&!Position(cues[i],out point))i--;
        if(i<0){if(cues.Count==0)hand.gameObject.SetActive(false);return;}
        var cue=cues[i];float start=Start(i);
        if(time<cue.time&&i>0&&Position(cues[i-1],out var previous))point=Vector2.Lerp(previous,point,Ease((time-start)/Mathf.Max(.001f,cue.time-start)));
        hand.localRotation=Quaternion.identity;
        hand.gameObject.SetActive(true);hand.anchoredPosition=point;hand.localScale=Vector3.one*Scale(cue,time);
        circle.localScale=new Vector3(CircleScale(time),CircleScale(time),1);
    }
    public void Dispose(){if(layer)UnityEngine.Object.Destroy(layer.gameObject);}
}
