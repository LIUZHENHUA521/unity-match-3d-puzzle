using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class PotGloryValidation {
 const string Out="work/pot-glory-validation/";const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static Match3DFlow flow;static Match3DInvitations account;static PotGloryView view;static int step;static double due;static string report="";static bool oldEnabled;static EnterPlayModeOptions oldOptions;
 static void Set(object o,string n,object v)=>o.GetType().GetField(n,Flags).SetValue(o,v);
 static object Get(object o,string n)=>o.GetType().GetField(n,Flags).GetValue(o);
 static void Check(bool ok,string text){if(!ok)throw new Exception(text);report+="PASS "+text+"\n";}
 public static void Run(){
  PotGloryBuilder.Build();Directory.CreateDirectory(Out);oldEnabled=EditorSettings.enterPlayModeOptionsEnabled;oldOptions=EditorSettings.enterPlayModeOptions;
  EditorSettings.enterPlayModeOptionsEnabled=true;EditorSettings.enterPlayModeOptions=EnterPlayModeOptions.DisableDomainReload;
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);EditorApplication.update+=Tick;due=EditorApplication.timeSinceStartup+3;EditorApplication.isPlaying=true;
 }
 static PotGloryView Current()=>((RectTransform)Get(flow,"modal")).GetComponent<PotGloryView>();
 static PotGloryRow[] Rows()=>view.content.GetComponentsInChildren<PotGloryRow>();
 static void Tick(){if(!EditorApplication.isPlaying||EditorApplication.timeSinceStartup<due)return;try{
 switch(step++){
 case 0:
  flow=new GameObject("Pot glory fixture").AddComponent<Match3DFlow>();flow.enabled=false;flow.library=Resources.Load<Match3DTheme>("Match3D/ThemeLibrary");
  ((CanvasGroup)Get(flow,"startupInput")).interactable=true;Set(flow,"effects",flow.gameObject.AddComponent<AudioSource>());
  Set(flow,"<ScreenName>k__BackingField","home");var page=(RectTransform)Get(flow,"page");foreach(Transform t in page)t.gameObject.SetActive(false);
  var home=Match3DUIView.Load("Home",page);Check(home.GetComponent<Match3DHomeView>().glory,"home entry is serialized");
  account=new GameObject("Account fixture").AddComponent<Match3DInvitations>();account.enabled=false;
  typeof(Match3DInvitations).GetField("<Instance>k__BackingField",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,account);
  Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode="pot-glory-fixture-alice"});Set(account,"token","fixture");
  var service=new GameObject("Glory transport fixture").AddComponent<PotReplayService>();Set(service,"endpoint",File.ReadAllText(Out+"endpoint").Trim());
  typeof(PotReplayService).GetField("<Instance>k__BackingField",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,service);
  PlayerPrefs.DeleteKey("pot.glory.seen.pot-glory-fixture-alice");flow.ShowPotGlory();view=Current();break;
 case 1:
  Check(Rows().Length==10,"first page fetched through real HTTP transport");Check(view.count.text.Contains("12"),"total count uses server total");
  Check(Rows()[0].tier.text=="清北局"&&Rows()[1].tier.text=="本科局","both tier cards mapped");Check(Rows()[0].duration.text=="03:28","duration formatted");
  Check(Rows()[0].badge.activeSelf,"newly acquired badge visible");Check(Rows()[2].replayState.transform.parent.gameObject.activeSelf,"pending replay remains visible");
  Check(view.more.gameObject.activeSelf,"pagination offered");
  Capture("glory-long",720,1564);Capture("glory-short",720,1280);Capture("glory-tablet",768,1024);
  view.more.onClick.Invoke();break;
 case 2:
  Check(Rows().Length==12&&!view.more.gameObject.activeSelf,"pagination appends without duplicates");
  Rows()[0].certificate.onClick.Invoke();break;
 case 3:
  var certificate=((RectTransform)Get(flow,"modal")).GetComponent<PotCertificateView>();
  Check(certificate&&certificate.number.text=="POT-fixture0","row opens matching issued certificate");Check(certificate.buttonLabel.text=="返回我的荣耀","history has no claim action");
  certificate.Skip();certificate.claim.onClick.Invoke();break;
 case 4:
  view=Current();Check(Rows().Length==10,"certificate returns to collection");Check(!Rows()[0].badge.activeSelf,"viewed newest certificate marked seen");
  Rows()[0].replay.onClick.Invoke();Rows()[0].replay.onClick.Invoke();break;
 case 5:
  Check(!(bool)Get(view,"opening"),"failed download releases repeated-click guard");
  File.WriteAllText(Out+"mode","error");view.close.onClick.Invoke();flow.ShowPotGlory();break;
 case 6:
  view=Current();Check(view.retry.gameObject.activeSelf&&Rows().Length==0,"API failure has retry and no fabricated certificates");
  File.WriteAllText(Out+"mode","empty");view.retry.onClick.Invoke();break;
 case 7:
  Check(view.status.text.Contains("还没有")&&Rows().Length==0,"empty collection state");Capture("glory-empty",720,1564);
  File.WriteAllText(Out+"mode","slow");view.retry.gameObject.SetActive(true);view.retry.onClick.Invoke();Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode="bob"});break;
 case 8:
  Check(!view||!view.gameObject.activeInHierarchy,"account switch dismisses collection and ignores late response");
  Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode="pot-glory-fixture-alice"});
  flow.ShowPotCollectedCertificate(new PotCertificateData{recordId="guard",rewardTier="elite",certificateNo="POT-guard"});
  Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode="bob"});break;
 case 9:
  Check(Get(flow,"modal")==null||!(RectTransform)Get(flow,"modal"),"account switch also dismisses history certificate");Finish(null);return;
 }
 due=EditorApplication.timeSinceStartup+2;
 }catch(Exception e){Finish(e);}}
 static void Capture(string name,int w,int h){
  var canvas=(Canvas)Get(flow,"canvas");var camera=new GameObject("Capture").AddComponent<Camera>();camera.transform.position=new Vector3(0,0,-10);camera.clearFlags=CameraClearFlags.SolidColor;
  var rt=new RenderTexture(w,h,24);rt.Create();camera.targetTexture=rt;var old=RenderTexture.active;var mode=canvas.renderMode;
  canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;Canvas.ForceUpdateCanvases();view.GetComponent<Match3DUIFitContent>().Fit();Canvas.ForceUpdateCanvases();
  camera.Render();foreach(var label in view.GetComponentsInChildren<Text>()){if(!string.IsNullOrEmpty(label.text)&&label.transform.IsChildOf(view.content)&&label.rectTransform.position.y>view.scroll.viewport.position.y-view.scroll.viewport.rect.height)Check(label.cachedTextGenerator.vertexCount>0,"visible row text produces glyphs: "+label.name);}RenderTexture.active=rt;var t=new Texture2D(w,h,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,w,h),0,0);t.Apply();File.WriteAllBytes(Out+name+".png",t.EncodeToPNG());
  canvas.renderMode=mode;canvas.worldCamera=null;RenderTexture.active=old;camera.targetTexture=null;UnityEngine.Object.DestroyImmediate(t);UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(camera.gameObject);
 }
 static void Finish(Exception e){EditorApplication.update-=Tick;EditorSettings.enterPlayModeOptionsEnabled=oldEnabled;EditorSettings.enterPlayModeOptions=oldOptions;report+=e==null?"COMPLETE\n":"FAIL "+e+"\n";File.WriteAllText(Out+"checks.txt",report);Debug.Log(report);EditorApplication.Exit(e==null?0:1);}
}
