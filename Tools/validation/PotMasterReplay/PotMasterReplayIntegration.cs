using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object=UnityEngine.Object;

[InitializeOnLoad]
public static class PotMasterReplayIntegration {
 const string Key="pot.master.integration",Out="work/pot-master-replay/",Owner="pot-master-fixture";
 const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static Match3DFlow flow;static int ads;static Action<bool,string> pending;
 static void Set(object o,string k,object v)=>o.GetType().GetField(k,Flags).SetValue(o,v);
 static object Get(object o,string k)=>o.GetType().GetField(k,Flags).GetValue(o);
 static PotMasterReplayIntegration(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))EditorApplication.delayCall+=()=>new GameObject("Pot validation driver").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());};}
 public static void Run(){SessionState.SetBool(Key,true);File.WriteAllText(Out+"integration.txt","");EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
 static void Check(bool ok,string message){if(!ok)throw new Exception(message);File.AppendAllText(Out+"integration.txt","PASS "+message+"\n");}
 static IEnumerator Wait(Func<bool> condition,string message,float seconds=15){float until=Time.realtimeSinceStartup+seconds;while(!condition()&&Time.realtimeSinceStartup<until)yield return null;Check(condition(),message);}
 static void Mode(string value)=>File.WriteAllText(Out+"mode",value);
 static PotMasterReplayView View()=>Object.FindObjectOfType<PotMasterReplayView>();
 static Button Watch(int row=0)=>View().content.GetChild(row).GetComponentInChildren<Button>();
 static PotMasterReplayAccess Access()=> (PotMasterReplayAccess)Get(View(),"access");
 static IEnumerator Guard(){string error=null;yield return Match3DStartupRunner.Run(Core(),e=>error=e.ToString());Match3DRewardedVideo.EditorShowOverride=null;SessionState.SetBool(Key,false);File.AppendAllText(Out+"integration.txt",error==null?"COMPLETE\n":"FAIL "+error+"\n");EditorApplication.Exit(error==null?0:1);}
 static IEnumerator Core(){
  yield return Wait(()=>Match3DFlow.Instance&&Match3DFlow.Instance.StartupComplete,"startup",60);flow=Match3DFlow.Instance;
  foreach(var s in flow.GetComponents<MonoBehaviour>())if(s!=flow){s.StopAllCoroutines();s.enabled=false;s.GetType().GetField("endpoint",Flags)?.SetValue(s,File.ReadAllText(Out+"endpoint"));}
  var account=Match3DInvitations.Instance;Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode=Owner});Set(account,"token","fixture");
  Match3DRewardedVideo.EditorShowOverride=done=>{ads++;pending=done;};
  Mode("five");flow.Home();yield return new WaitForSecondsRealtime(1);
  Check(!Object.FindObjectOfType<PotMasterReplayEntry>(),"home has no master replay entry");Capture("home",720,1564);
  Set(flow,"<Stage>k__BackingField",2);Set(flow,"dailyLevel",new Match3DDailyLevel{tier="elite",date=Match3DFlow.Today});
  typeof(Match3DFlow).GetMethod("Result",Flags).Invoke(flow,new object[]{false,"失败",""});
  Check(!Object.FindObjectOfType<PotMasterReplayEntry>(),"elite failure has no master replay entry");
  Set(flow,"dailyLevel",new Match3DDailyLevel{tier="bachelor",date=Match3DFlow.Today});
  typeof(Match3DFlow).GetMethod("Result",Flags).Invoke(flow,new object[]{false,"失败",""});yield return new WaitForSecondsRealtime(2);
  var entry=Object.FindObjectOfType<PotMasterReplayEntry>();Check(entry,"bachelor failure master entry exists");
  var button=(Button)Get(entry,"button");Check(!button.gameObject.activeSelf,"five replays hide entry");
  Mode("ready");Set(entry,"next",0f);yield return Wait(()=>button.gameObject.activeSelf,"six or more show entry");yield return new WaitForSecondsRealtime(.8f);
  var result=entry.GetComponent<Match3DResultView>();var entryRect=(RectTransform)button.transform;var retryRect=(RectTransform)result.primary.transform;
  Check(entryRect.sizeDelta==retryRect.sizeDelta&&entryRect.localScale==retryRect.localScale&&entryRect.anchorMin==retryRect.anchorMin&&entryRect.anchorMax==retryRect.anchorMax&&entryRect.pivot==retryRect.pivot,"entry preserves retry button layout");
  Check(button.transition==result.primary.transition&&button.colors.Equals(result.primary.colors)&&button.navigation.Equals(result.primary.navigation),"entry preserves retry interaction styling");
  Check(button.GetComponentsInChildren<Outline>(true).Length==result.primary.GetComponentsInChildren<Outline>(true).Length,"entry adds no offset text shadow");
  Check(entryRect.anchoredPosition.y-entryRect.rect.height*.5f>retryRect.anchoredPosition.y+retryRect.rect.height*.5f,"entry is above retry without overlap");Capture("failure",720,1564);Capture("failure-short",720,1280);
  button.onClick.Invoke();yield return Wait(()=>View()&&View().content.childCount==10,"server list renders top ten");yield return new WaitForSecondsRealtime(.5f);
  Capture("list-long",720,1564);Capture("list-short",720,1280);Capture("list-tablet",768,1024);
  Watch().onClick.Invoke();Watch().onClick.Invoke();Check(ads==1,"duplicate click starts one ad");pending(false,"视频未看完");Check(!Access().Busy&&!PotReplayPlayer.Active,"cancelled ad does not play");
  Watch().onClick.Invoke();Check(ads==2,"cancel permits new ad");pending(true,null);yield return Wait(()=>!Access().Busy,"download failure releases guard");Check(Access().Paid(new string('a',64)),"download failure retains paid selection");
  Watch().onClick.Invoke();Check(ads==2,"same replay retries without another ad");yield return Wait(()=>!Access().Busy,"second download failure releases guard");
  Watch(1).onClick.Invoke();Check(ads==3,"different replay requires ad");var late=pending;View().close.onClick.Invoke();late(true,null);yield return null;Check(!View()&&!PotReplayPlayer.Active,"closed modal ignores late ad success");Check(result&&result.gameObject.activeInHierarchy,"closing list restores original failure dialog");
  flow.ShowPotMasterReplays();yield return Wait(()=>View()&&View().content.childCount==10,"reopen list");Watch().onClick.Invoke();pending(true,null);View().close.onClick.Invoke();yield return new WaitForSecondsRealtime(1.5f);Check(!View()&&!PotReplayPlayer.Active,"closed modal ignores late download");
  flow.ShowPotMasterReplays();yield return Wait(()=>View()&&View().content.childCount==10,"account guard list");Watch().onClick.Invoke();late=pending;Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode="other"});yield return null;late(true,null);yield return null;Check(!View()&&!PotReplayPlayer.Active,"account switch ignores ad callback");
  Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode=Owner});Mode("success");flow.ShowPotMasterReplays();yield return Wait(()=>View()&&View().content.childCount==10,"playable fixture list");Watch().onClick.Invoke();pending(true,null);
  yield return Wait(()=>PotReplayPlayer.Active&&PotReplayPlayer.Instance.IsPlaying,"paid replay actually starts",45);Check(flow.ScreenName=="pot-replay","replay screen active");PotReplayPlayer.Instance.Exit();yield return Wait(()=>View()&&View().content.childCount==10,"exit returns to masters list");
  int before=ads;Watch().onClick.Invoke();Check(ads==before+1,"watch again requires new ad");pending(false,"取消");
  Mode("version");Watch().onClick.Invoke();pending(true,null);yield return Wait(()=>View().status.text.Contains("已更新"),"daily version changed during ad cannot play stale replay");Check(!Access().Paid(new string('a',64)),"version change clears retry qualification");
  Mode("error");View().retry.onClick.Invoke();yield return Wait(()=>View().retry.gameObject.activeSelf,"network error exposes retry");
 }
 static void Capture(string name,int w,int h){
  var canvas=(Canvas)Get(flow,"canvas");var camera=new GameObject("Pot capture").AddComponent<Camera>();camera.transform.position=new Vector3(0,0,-10);camera.clearFlags=CameraClearFlags.SolidColor;
  var rt=new RenderTexture(w,h,24);rt.Create();camera.targetTexture=rt;var old=RenderTexture.active;var mode=canvas.renderMode;
  canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;Canvas.ForceUpdateCanvases();var home=Object.FindObjectOfType<Match3DHomeView>();if(home)typeof(Match3DHomeView).GetMethod("Layout",Flags).Invoke(home,null);foreach(var fit in Object.FindObjectsOfType<Match3DUIFitContent>())fit.Fit();Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=rt;
  var texture=new Texture2D(w,h,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,w,h),0,0);texture.Apply();File.WriteAllBytes(Out+name+".png",texture.EncodeToPNG());canvas.renderMode=mode;canvas.worldCamera=null;RenderTexture.active=old;camera.targetTexture=null;Object.DestroyImmediate(texture);Object.DestroyImmediate(rt);Object.DestroyImmediate(camera.gameObject);
 }
}
