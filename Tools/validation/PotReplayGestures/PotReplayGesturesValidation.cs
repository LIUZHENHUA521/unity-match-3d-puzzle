using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Object=UnityEngine.Object;

[InitializeOnLoad]
public static class PotReplayGesturesValidation {
    const string Key="pot.replay.gestures.validation",Output="work/pot-replay-gestures";
    const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
    static PotReplayGesturesValidation(){EditorApplication.playModeStateChanged+=state=>{
        if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))
            EditorApplication.delayCall+=()=>new GameObject("Pot replay validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
    };}
    public static void Run(){Directory.CreateDirectory(Output);File.WriteAllText(Output+"/checks.txt","");SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Check(bool value,string message){if(!value)throw new Exception(message);File.AppendAllText(Output+"/checks.txt","PASS "+message+"\n");}
    static IEnumerator Guard(){
        float deadline=Time.realtimeSinceStartup+90;
        while((!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)&&Time.realtimeSinceStartup<deadline)yield return null;
        string error=null;var core=Match3DStartupRunner.Run(Core(),e=>error=e.ToString());
        while(true){bool more=false;try{more=core.MoveNext();}catch(Exception e){error=e.ToString();}if(!more||error!=null)break;yield return core.Current;}
        Time.timeScale=1;SessionState.SetBool(Key,false);File.AppendAllText(Output+"/checks.txt",error==null?"COMPLETE\n":"FAIL "+error+"\n");EditorApplication.Exit(error==null?0:1);
    }
    static IEnumerator Core(){
        foreach(var service in new MonoBehaviour[]{Match3DGameRecords.Instance,Match3DInvitations.Instance,PotReplayService.Instance})if(service){service.StopAllCoroutines();service.enabled=false;}
        var data=JsonUtility.FromJson<PotReplayData>(PotReplayTransfer.Unpack(File.ReadAllBytes(Output+"/fixture.json.gz")));
        var player=new GameObject("Pot replay player").AddComponent<PotReplayPlayer>();player.Load(data);
        float deadline=Time.realtimeSinceStartup+90;
        while(!(bool)typeof(PotReplayPlayer).GetField("ready",Private).GetValue(player)&&Time.realtimeSinceStartup<deadline)yield return null;
        Check((bool)typeof(PotReplayPlayer).GetField("ready",Private).GetValue(player),"actual uploaded v2 replay loads");player.Pause();yield return null;
        var hud=Object.FindObjectOfType<Match3DGameplayHUD>();
        Check(hud&&hud.footer.gameObject.activeInHierarchy&&hud.header.gameObject.activeSelf,"replay header and footer both visible");
        Check(!hud.footer.GetComponent<CanvasGroup>().blocksRaycasts&&!hud.footer.GetComponent<CanvasGroup>().interactable,"replay footer cannot trigger live actions");
        Check(hud.quantities.All(q=>!q.transform.parent.gameObject.activeSelf),"replay never displays current account inventory");
        Check(Enumerable.Range(0,4).All(i=>hud.ReplayToolTarget(i).gameObject.activeInHierarchy),"all four tool icons stay visible");
        var tray=(RectTransform)typeof(Match3DTray).GetField("tray",Private).GetValue(Match3DTray.Instance);
        Check(tray.gameObject.activeInHierarchy,"seven-slot tray background visible in replay");
        var timeline=(PotReplayGestures)typeof(PotReplayPlayer).GetField("gestures",Private).GetValue(player);
        Check(timeline.cues.Count(c=>c.target==PotReplayGestures.Target.Item)==data.events.Count(e=>e.type=="pick"&&e.value==0),"manual picks shown; magnet and probe pickups never duplicate clicks");
        var manual=timeline.cues.First(c=>c.target==PotReplayGestures.Target.Item);player.Seek(manual.time+.06f);yield return null;
        var hand=(RectTransform)typeof(PotReplayGestures).GetField("hand",Private).GetValue(timeline);
        var circle=hand.Find("Pot replay circle").GetComponent<Image>();
        Check(circle.sprite&&circle.rectTransform.sizeDelta==new Vector2(81,84)&&circle.transform.GetSiblingIndex()==0&&!circle.raycastTarget,"original circle is rendered behind the hand without intercepting input");
        Check(hand.gameObject.activeSelf&&Mathf.Abs(hand.localScale.x-.92f)<.001f,"hand uses source 92 percent press scale");
        Vector3 position=hand.position,scale=hand.localScale,haloScale=circle.transform.localScale;yield return null;yield return null;
        Check(hand.position==position&&hand.localScale==scale,"paused replay freezes hand animation");
        Check(circle.transform.localScale==haloScale,"paused replay freezes circle pulse");
        player.SetSpeed(3);player.Seek(manual.time+.06f);Check(hand.position==position&&hand.localScale==scale,"speed changes do not alter an absolute-time gesture");
        player.Seek(Mathf.Min(data.duration,manual.time+5));player.Seek(manual.time+.06f);
        Check(hand.position==position&&hand.localScale==scale,"backward seek reconstructs identical hand state");
        Capture(player,manual.time+.06f,"pot-replay-item",720,1565);
        var tool=timeline.cues.FirstOrDefault(c=>c.target==PotReplayGestures.Target.Tool);
        Check(tool!=null,"real recording contains a tool use");Capture(player,tool.time+.06f,"pot-replay-tool",720,1565);
        Check(hand.localRotation==Quaternion.identity&&hand.localScale.x>0,"tool hand keeps the original upright orientation without mirroring");
        var probe=timeline.cues.FirstOrDefault(c=>c.target==PotReplayGestures.Target.Probe);
        if(probe!=null)Capture(player,probe.time+.06f,"pot-replay-probe",720,1565);
        else{
            var frame=data.frames.First(f=>f.presentation!=null&&f.presentation.columns.Any(c=>c.ids.Length>0));
            int column=Array.FindIndex(frame.presentation.columns,c=>c.ids.Length>0);
            var cue=new PotReplayGestures.Cue{target=PotReplayGestures.Target.Probe,index=column,time=frame.t,end=frame.t};
            timeline.cues.Clear();timeline.cues.Add(cue);Capture(player,frame.t+.06f,"pot-replay-probe",720,1565);
            Check(hand.gameObject.activeSelf,"probe hand resolves the visible recorded probe column");
        }
        TimelineChecks((RectTransform)hud.transform.parent);
        yield return ControlsChecks(player,hud,data,hand);
        Match3DFlow.Instance.ShowPotReplaySettings();
        Object.FindObjectOfType<Match3DSettingsSkin>().support.onClick.Invoke();yield return null;Check(!PotReplayPlayer.Active&&Time.timeScale==1,"exit restores game time and removes replay state");
        Check(GameObject.Find("Pot replay gestures")==null,"exit disposes hand overlay");
    }
    static IEnumerator ControlsChecks(PotReplayPlayer player,Match3DGameplayHUD hud,PotReplayData data,RectTransform hand){
        var view=hud.GetComponent<Match3DUIView>();var slider=hud.header.GetComponentInChildren<PotReplayProgress>();
        var speed=hud.header.Find("Pot replay speed");var minus=speed.Find("Zoom out").GetComponent<Button>();var plus=speed.Find("Zoom in").GetComponent<Button>();
        Check(slider&&slider.fillRect&&slider.handleRect&&view.Get<Button>("Pause").IsInteractable(),"original pause, speed controls and artist progress bar are available");
        Check(GameObject.Find("Pot replay overlay")==null&&!hud.GetComponentsInChildren<Text>().Any(t=>t.text=="退出回放"||t.text=="暂停 / 继续"),"standalone replay buttons and pale panel removed");
        player.SetSpeed(.5f);yield return null;Check(!minus.interactable&&plus.interactable,"minimum speed disables minus only");
        plus.onClick.Invoke();Check(Mathf.Abs(player.Speed-1)<.001f,"plus adds half speed");
        minus.onClick.Invoke();Check(Mathf.Abs(player.Speed-.5f)<.001f,"minus subtracts half speed");
        player.SetSpeed(3);yield return null;Check(minus.interactable&&!plus.interactable,"maximum speed disables plus only");
        player.SetSpeed(1);player.Seek(data.duration*.25f);player.Pause();yield return null;
        Check(player.IsPlaying,"replay runs before opening settings");
        view.Get<Button>("Pause").onClick.Invoke();yield return null;
        float stopped=player.CurrentTime;Vector3 handPosition=hand.position;yield return null;yield return null;
        Check(player.SettingsOpen&&!player.IsPlaying&&player.CurrentTime==stopped&&hand.position==handPosition,"original pause button opens settings and freezes replay and gesture");
        player.SetSpeed(3);slider.onValueChanged.Invoke(.9f);yield return null;
        Check(player.Speed==1&&player.CurrentTime==stopped&&!slider.IsInteractable()&&!plus.IsInteractable()&&!minus.IsInteractable(),"settings blocks speed and progress changes");
        Object.FindObjectOfType<Match3DSettingsSkin>().GetComponent<Match3DUIView>().Get<Button>("Close").onClick.Invoke();yield return null;
        Check(!player.SettingsOpen&&player.IsPlaying&&player.CurrentTime>stopped&&Time.timeScale==0&&Match3DFlow.Instance.BlocksGameplay,"closing settings resumes only replay while live simulation stays locked");
        Canvas.ForceUpdateCanvases();var rect=(RectTransform)slider.transform;
        var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(new Vector3(-120,-40,0)))};
        slider.OnPointerDown(pointer);float dragTime=player.CurrentTime;yield return null;yield return null;
        Check(!player.IsPlaying&&player.CurrentTime==dragTime,"pressing progress pauses playback during scrub");
        slider.value=.4f;slider.OnPointerUp(pointer);yield return null;
        Check(player.IsPlaying&&player.CurrentTime>=data.duration*.4f,"releasing progress resumes from selected position");
        player.Seek(data.duration);yield return null;
        Check(!player.IsPlaying&&player.CurrentTime==data.duration,"replay ends on final frame without restarting");
        slider.OnPointerDown(pointer);slider.value=.2f;slider.OnPointerUp(pointer);yield return null;
        Check(player.IsPlaying&&player.CurrentTime<data.duration*.3f,"scrubbing backward from the end resumes playback");
        player.Pause();
        var sample=data.frames.First(f=>f.remaining>0);player.Seek(sample.t);
        int seconds=Mathf.CeilToInt(sample.remaining);
        Check(view.Get<Text>("Timer").text==$"{seconds/60:00}:{seconds%60:00}","game timer displays recorded remaining time");
        var added=data.frames.Zip(data.frames.Skip(1),(a,b)=>new{a,b}).FirstOrDefault(x=>x.b.remaining>x.a.remaining+.5f);
        if(added!=null){player.Seek(added.b.t);seconds=Mathf.CeilToInt(added.b.remaining);Check(view.Get<Text>("Timer").text==$"{seconds/60:00}:{seconds%60:00}","recorded add-time updates game timer");}
        // Exercise an add-time jump even when this uploaded fixture did not use that tool.
        int jumpIndex=data.frames.Count/2;var jump=data.frames[jumpIndex];float savedRemaining=jump.remaining;
        jump.remaining=data.frames[jumpIndex-1].remaining+60;player.Seek(jump.t);seconds=Mathf.CeilToInt(jump.remaining);
        Check(view.Get<Text>("Timer").text==$"{seconds/60:00}:{seconds%60:00}","remaining timer follows recorded add-time jumps rather than duration minus elapsed");
        jump.remaining=savedRemaining;player.Seek(data.frames[jumpIndex-1].t);seconds=Mathf.CeilToInt(data.frames[jumpIndex-1].remaining);
        Check(view.Get<Text>("Timer").text==$"{seconds/60:00}:{seconds%60:00}","backward seek restores the earlier remaining time");
        Canvas.ForceUpdateCanvases();slider.value=.5f;
        Check(Mathf.Abs(slider.fillRect.rect.width-630)<.1f&&Mathf.Abs(slider.handleRect.anchoredPosition.x)<.1f&&Mathf.Abs(slider.handleRect.rect.height-81)<.1f,"progress fill and thumb use correct track geometry");
        Capture(player,data.duration*.4f,"pot-replay-controls",720,1565);
    }
    static void TimelineChecks(RectTransform parent){
        var pos=Camera.main.ViewportToWorldPoint(new Vector3(.4f,.6f,8));var data=new PotReplayData{duration=10,opening=new Match3DDailyStage{items=new[]{new Match3DDailyItem{id=1,position=pos}}}};
        var p=new PotReplayPresentation{columns=new[]{new PotReplayProbeColumn{ids=new[]{1}},new PotReplayProbeColumn(),new PotReplayProbeColumn(),new PotReplayProbeColumn()}};
        data.frames.Add(new PotReplayFrame{t=0,presentation=p,items=new List<PotReplayPose>{new PotReplayPose{id=1,p=pos}}});
        data.frames.Add(new PotReplayFrame{t=.5f,presentation=p});
        data.events=new List<PotReplayEvent>{new PotReplayEvent{t=1,type="pick",id=1},new PotReplayEvent{t=2,type="pick",id=1,value=1},new PotReplayEvent{t=3,type="probe_preview",id=1,value=1},new PotReplayEvent{t=4,type="probe_preview",id=1,value=0},new PotReplayEvent{t=5,type="probe_preview",id=1,value=1},new PotReplayEvent{t=5.1f,type="probe_preview",id=1,value=0},new PotReplayEvent{t=5.1f,type="pick",id=1,value=2},new PotReplayEvent{t=5.1f,type="probe_use",id=1,value=0}};
        var timeline=new PotReplayGestures(data,parent);
        Check(timeline.cues.Count==3&&timeline.cues[0].position==pos,"delta frames retain pre-pick positions and synthetic pickups are filtered");
        Check(timeline.cues[1].preview&&!timeline.cues[2].preview,"cancelled preview stays a preview; successful probe use merges into one hand action");
        Check(Mathf.Abs(PotReplayGestures.Scale(timeline.cues[1],3.8f)-.92f)<.001f,"long preview holds press scale until recorded preview end");
        Check(timeline.CueAt(2.5f)==0&&timeline.CueAt(10)==2,"idle gaps and replay end retain the last operation");
        var hand=(RectTransform)typeof(PotReplayGestures).GetField("hand",Private).GetValue(timeline);
        timeline.Apply(1.2f);Vector2 held=hand.anchoredPosition;timeline.Apply(2.5f);
        Check(hand.gameObject.activeSelf&&hand.anchoredPosition==held,"hand remains visible at the previous click while waiting");
        timeline.Apply(3);Vector2 target=hand.anchoredPosition;timeline.Apply(2.93f);Vector2 mid=hand.anchoredPosition;
        Check(hand.gameObject.activeSelf&&Vector2.Distance(mid,held)>.01f&&Vector2.Distance(mid,target)>.01f&&Vector2.Distance(mid,target)<Vector2.Distance(held,target),"hand moves continuously between operation targets");
        timeline.Apply(10);Vector2 end=hand.anchoredPosition;timeline.Apply(2.5f);timeline.Apply(10);
        Check(hand.gameObject.activeSelf&&hand.anchoredPosition==end,"seeking reconstructs the retained cursor position");
        Check(Mathf.Abs(PotReplayGestures.CircleScale(0)-1)<.001f&&Mathf.Abs(PotReplayGestures.CircleScale(2f/3)-1.05f)<.001f&&Mathf.Abs(PotReplayGestures.CircleScale(4f/3)-1.1f)<.001f&&Mathf.Abs(PotReplayGestures.CircleScale(8f/3)-1)<.001f,"circle pulse matches source animation timing and keyframes");
        timeline.Dispose();
    }
    static void Capture(PotReplayPlayer player,float time,string name,int width,int height){
        var camera=Camera.main;var previous=camera.targetTexture;float aspect=camera.aspect;
        var overlays=Object.FindObjectsOfType<Canvas>().Where(c=>c.isRootCanvas&&c.renderMode==RenderMode.ScreenSpaceOverlay).ToArray();
        var oldCameras=overlays.Select(c=>c.worldCamera).ToArray();var distances=overlays.Select(c=>c.planeDistance).ToArray();
        var rt=new RenderTexture(width,height,24);rt.Create();var active=RenderTexture.active;var texture=new Texture2D(width,height,TextureFormat.RGB24,false);
        try{
            camera.targetTexture=rt;camera.aspect=width/(float)height;
            foreach(var c in overlays){c.renderMode=RenderMode.ScreenSpaceCamera;c.worldCamera=camera;c.planeDistance=.5f;}
            Canvas.ForceUpdateCanvases();player.Seek(time);Match3DTray.Instance.SendMessage("LateUpdate");Canvas.ForceUpdateCanvases();player.Seek(time);
            camera.Render();RenderTexture.active=rt;texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply();File.WriteAllBytes(Output+"/"+name+".png",texture.EncodeToPNG());
        }finally{for(int i=0;i<overlays.Length;i++){overlays[i].renderMode=RenderMode.ScreenSpaceOverlay;overlays[i].worldCamera=oldCameras[i];overlays[i].planeDistance=distances[i];}camera.targetTexture=previous;camera.aspect=aspect;RenderTexture.active=active;Object.DestroyImmediate(texture);rt.Release();Object.DestroyImmediate(rt);}
    }
}
