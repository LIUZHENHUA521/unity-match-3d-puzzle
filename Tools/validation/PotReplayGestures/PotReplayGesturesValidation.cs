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
        string error=null;var core=Core();
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
        Check(hud&&hud.footer.gameObject.activeInHierarchy&&!hud.header.gameObject.activeSelf,"replay footer visible and gameplay header hidden");
        Check(!hud.GetComponent<CanvasGroup>().blocksRaycasts&&!hud.GetComponent<CanvasGroup>().interactable,"replay HUD cannot trigger live actions");
        Check(hud.quantities.All(q=>!q.transform.parent.gameObject.activeSelf),"replay never displays current account inventory");
        Check(Enumerable.Range(0,4).All(i=>hud.ReplayToolTarget(i).gameObject.activeInHierarchy),"all four tool icons stay visible");
        var tray=(RectTransform)typeof(Match3DTray).GetField("tray",Private).GetValue(Match3DTray.Instance);
        Check(tray.gameObject.activeInHierarchy,"seven-slot tray background visible in replay");
        var timeline=(PotReplayGestures)typeof(PotReplayPlayer).GetField("gestures",Private).GetValue(player);
        Check(timeline.cues.Count(c=>c.target==PotReplayGestures.Target.Item)==data.events.Count(e=>e.type=="pick"&&e.value==0),"manual picks shown; magnet and probe pickups never duplicate clicks");
        var manual=timeline.cues.First(c=>c.target==PotReplayGestures.Target.Item);player.Seek(manual.time+.06f);yield return null;
        var hand=(RectTransform)typeof(PotReplayGestures).GetField("hand",Private).GetValue(timeline);
        Check(hand.gameObject.activeSelf&&Mathf.Abs(hand.localScale.x-.92f)<.001f,"hand uses source 92 percent press scale");
        Vector3 position=hand.position,scale=hand.localScale;yield return null;yield return null;
        Check(hand.position==position&&hand.localScale==scale,"paused replay freezes hand animation");
        player.SetSpeed(3);player.Seek(manual.time+.06f);Check(hand.position==position&&hand.localScale==scale,"speed changes do not alter an absolute-time gesture");
        player.Seek(Mathf.Min(data.duration,manual.time+5));player.Seek(manual.time+.06f);
        Check(hand.position==position&&hand.localScale==scale,"backward seek reconstructs identical hand state");
        Capture(player,manual.time+.06f,"pot-replay-item",720,1565);
        var tool=timeline.cues.FirstOrDefault(c=>c.target==PotReplayGestures.Target.Tool);
        Check(tool!=null,"real recording contains a tool use");Capture(player,tool.time+.06f,"pot-replay-tool",720,1565);
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
        player.Exit();yield return null;Check(!PotReplayPlayer.Active&&Time.timeScale==1,"exit restores game time and removes replay state");
        Check(GameObject.Find("Pot replay gestures")==null,"exit disposes hand overlay");
    }
    static void TimelineChecks(RectTransform parent){
        var pos=new Vector3(1,2,3);var data=new PotReplayData{duration=10,opening=new Match3DDailyStage{items=new[]{new Match3DDailyItem{id=1,position=pos}}}};
        var p=new PotReplayPresentation{columns=new[]{new PotReplayProbeColumn{ids=new[]{1}},new PotReplayProbeColumn(),new PotReplayProbeColumn(),new PotReplayProbeColumn()}};
        data.frames.Add(new PotReplayFrame{t=0,presentation=p,items=new List<PotReplayPose>{new PotReplayPose{id=1,p=pos}}});
        data.frames.Add(new PotReplayFrame{t=.5f,presentation=p});
        data.events=new List<PotReplayEvent>{new PotReplayEvent{t=1,type="pick",id=1},new PotReplayEvent{t=2,type="pick",id=1,value=1},new PotReplayEvent{t=3,type="probe_preview",id=1,value=1},new PotReplayEvent{t=4,type="probe_preview",id=1,value=0},new PotReplayEvent{t=5,type="probe_preview",id=1,value=1},new PotReplayEvent{t=5.1f,type="probe_preview",id=1,value=0},new PotReplayEvent{t=5.1f,type="pick",id=1,value=2},new PotReplayEvent{t=5.1f,type="probe_use",id=1,value=0}};
        var timeline=new PotReplayGestures(data,parent);
        Check(timeline.cues.Count==3&&timeline.cues[0].position==pos,"delta frames retain pre-pick positions and synthetic pickups are filtered");
        Check(timeline.cues[1].preview&&!timeline.cues[2].preview,"cancelled preview stays a preview; successful probe use merges into one hand action");
        Check(Mathf.Abs(PotReplayGestures.Scale(timeline.cues[1],3.8f)-.92f)<.001f,"long preview holds press scale until recorded preview end");
        Check(timeline.CueAt(2.5f)==-1,"hand hides during idle gaps");timeline.Dispose();
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
