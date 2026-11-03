using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[InitializeOnLoad]
public static class ToolAdOfferValidation {
 static ToolAdOfferValidation(){EditorApplication.update+=Poll;}
 static void Poll(){
  if(!File.Exists("Temp/run-tool-ad-check")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  if(!EditorApplication.isPlayingOrWillChangePlaymode){EditorApplication.isPlaying=true;return;}
  if(!Application.isPlaying||!ReplicaFlow.Instance||!UnityEngine.Object.FindObjectOfType<ReplicaHomeView>())return;
  File.Delete("Temp/run-tool-ad-check");new GameObject("Tool ad validation").AddComponent<SourceValidationDriver>().StartCoroutine(Guard());
 }
 static object oldToken,oldEndpoint;static ReplicaInvitations.Reply oldState;static bool mocked;
 static IEnumerator Guard(){var steps=Run();string failure=null;while(true){object current=null;bool next=false;try{next=steps.MoveNext();if(next)current=steps.Current;}catch(Exception e){failure=e.ToString();}if(!next||failure!=null)break;yield return current;}
  if(mocked){var service=ReplicaInvitations.Instance;var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;typeof(ReplicaInvitations).GetField("token",flags).SetValue(service,oldToken);typeof(ReplicaInvitations).GetField("endpoint",flags).SetValue(service,oldEndpoint);typeof(ReplicaInvitations).GetProperty("State").SetValue(service,oldState);mocked=false;}
  ReplicaRewardedVideo.EditorShowOverride=null;ReplicaFlow.Instance.Home();File.WriteAllText("Temp/tool-ad-validation.txt",failure==null?"PASS all three descriptions; no per-round free uses; empty inventory prompts video; incomplete video no grant; retry; completed video exactly one effect; close resumes timer; stale callback ignored; inventory consumed through local mock HTTP without showing an ad.":"FAIL "+failure);EditorApplication.isPlaying=false;
 }
 static void Assert(bool value,string message){if(!value)throw new Exception(message);}
 static void Setup(){var flow=ReplicaFlow.Instance;flow.Begin(0);SourceCreationTestSupport.FinishPending(flow);GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;}
 static Button Tool(int type)=>UnityEngine.Object.FindObjectOfType<ReplicaGameplayHUD>().GetComponent<ReplicaUIView>().Get<Button>(new[]{"Undo","Hint","Shuffle"}[type]);
 static IEnumerator Run(){
  Action<bool,string> reply=null;ReplicaRewardedVideo.EditorShowOverride=done=>reply=done;
  for(int type=0;type<3;type++){
   Setup();yield return null;
   Assert(ReplicaSourceTools.Remaining(type)==0&&!ReplicaSourceTools.TrySpend(type),"free allowance remains");
   if(type!=2){InputController.Instance.AddObjectOnStack(LevelGenerator.Instance.objectsOnScene.First(x=>x&&x.activeInHierarchy));yield return new WaitForSecondsRealtime(.4f);}
   if(type==2)Assert(!ReplicaSourceTools.MessUp(),"direct shuffle bypassed payment");
   Tool(type).onClick.Invoke();yield return new WaitForSecondsRealtime(.5f);
   var view=UnityEngine.Object.FindObjectOfType<ReplicaReviveView>();Assert(view&&view.progress.text==new[]{"移出道具","吸附道具","洗牌道具"}[type],"wrong tool offer");
   Capture(type);
   Assert(!view.progress.gameObject.activeSelf&&view.animatedContent.Find("Tool title").GetComponent<Image>().sprite,"missing artwork title");
   var artwork=view.animatedContent.Find("Tool description").GetComponent<Image>();Assert(artwork&&artwork.sprite&&artwork.rectTransform.sizeDelta==artwork.sprite.rect.size,"text artwork stretched");
   Assert(Time.timeScale==0&&ReplicaSourceTools.UsedCount(type)==0,"offer consumed effect or timer running");
   view.revive.onClick.Invoke();var cancelled=reply;reply(false,"");Assert(view.group.interactable&&ReplicaSourceTools.UsedCount(type)==0,"incomplete ad granted effect");
   view.revive.onClick.Invoke();var success=reply;reply(true,"");success(true,"");
   Assert(ReplicaSourceTools.UsedCount(type)==1&&ReplicaSourceTools.AdUses(type)==1&&Time.timeScale==1,"completed ad not exactly once");
   yield return new WaitForSecondsRealtime(1.3f);
  }
  Setup();yield return null;Tool(2).onClick.Invoke();yield return new WaitForSecondsRealtime(.5f);
  var close=UnityEngine.Object.FindObjectOfType<ReplicaReviveView>();close.close.onClick.Invoke();Assert(Time.timeScale==1&&ReplicaSourceTools.UsedCount(2)==0,"closing grants effect or leaves pause");
  Tool(2).onClick.Invoke();yield return new WaitForSecondsRealtime(.5f);UnityEngine.Object.FindObjectOfType<ReplicaReviveView>().revive.onClick.Invoke();var stale=reply;
  ReplicaFlow.Instance.Home();Setup();yield return null;stale(true,"");Assert(ReplicaSourceTools.UsedCount(2)==0&&!ReplicaSourceTools.Shuffling,"stale ad affects new round");
  Setup();yield return null;
  var service=ReplicaInvitations.Instance;var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
  oldToken=typeof(ReplicaInvitations).GetField("token",flags).GetValue(service);oldEndpoint=typeof(ReplicaInvitations).GetField("endpoint",flags).GetValue(service);oldState=service.State;mocked=true;
  typeof(ReplicaInvitations).GetField("token",flags).SetValue(service,"local-validation-token");typeof(ReplicaInvitations).GetField("endpoint",flags).SetValue(service,"http://127.0.0.1:18764");
  typeof(ReplicaInvitations).GetProperty("State").SetValue(service,new ReplicaInvitations.Reply{inventory=new[]{0,0,2},inviteCode="tool-test"});
  int ads=0;ReplicaRewardedVideo.EditorShowOverride=done=>ads++;
  Tool(2).onClick.Invoke();float deadline=Time.realtimeSinceStartup+5;
  while((service.Busy||ReplicaSourceTools.Shuffling)&&Time.realtimeSinceStartup<deadline)yield return null;
  Assert(ads==0&&service.State!=null&&service.State.inventory[2]==1&&ReplicaSourceTools.UsedCount(2)==1&&ReplicaSourceTools.AdUses(2)==0,"inventory did not debit exactly once without ads: ads="+ads+", inventory="+service.Inventory(2)+", used="+ReplicaSourceTools.UsedCount(2)+", rewarded="+ReplicaSourceTools.AdUses(2)+", status="+service.Status);

 }
    static void Capture(int type){
        var c=Camera.main;var canvases=UnityEngine.Object.FindObjectsOfType<Canvas>().Where(x=>x.isRootCanvas&&x.gameObject.activeInHierarchy).ToArray();var modes=canvases.Select(x=>x.renderMode).ToArray();var cameras=canvases.Select(x=>x.worldCamera).ToArray();var distances=canvases.Select(x=>x.planeDistance).ToArray();var rt=new RenderTexture(720,1280,24);rt.Create();var old=c.targetTexture;var active=RenderTexture.active;c.targetTexture=rt;
        for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=RenderMode.ScreenSpaceCamera;canvases[i].worldCamera=c;canvases[i].planeDistance=1;}
        Canvas.ForceUpdateCanvases();foreach(var fit in UnityEngine.Object.FindObjectsOfType<ReplicaUIFitContent>())fit.Fit();Canvas.ForceUpdateCanvases();c.Render();RenderTexture.active=rt;var tex=new Texture2D(720,1280,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,720,1280),0,0);tex.Apply();File.WriteAllBytes("Temp/tool-offer-artwork-"+type+".png",tex.EncodeToPNG());c.targetTexture=old;RenderTexture.active=active;
        for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];canvases[i].planeDistance=distances[i];}UnityEngine.Object.Destroy(tex);rt.Release();UnityEngine.Object.Destroy(rt);
    }
}
