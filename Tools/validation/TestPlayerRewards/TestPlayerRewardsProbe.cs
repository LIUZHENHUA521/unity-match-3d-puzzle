using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[InitializeOnLoad] public static class TestPlayerRewardsProbe {
 const string Key="TestPlayerRewardsProbe.Pending";
 static TestPlayerRewardsProbe(){EditorApplication.playModeStateChanged+=s=>{
  if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false)){
   SessionState.SetBool(Key,false);new GameObject("Rewards probe").AddComponent<TestPlayerRewardsRunner>();
  }
 };}
 public static void Run(){SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
}
public sealed class TestPlayerRewardsRunner:MonoBehaviour {
 const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 string Result=>Path.GetFullPath("../result.txt");
 string savedQueue;
 void Start(){savedQueue=PlayerPrefs.GetString("goose.records.queue.v1","");PlayerPrefs.DeleteKey("goose.records.queue.v1");File.WriteAllText(Result,"");StartCoroutine(Guard(Check()));}
 void Finish(int code){PlayerPrefs.SetString("goose.records.queue.v1",savedQueue);PlayerPrefs.Save();EditorApplication.Exit(code);}
 IEnumerator Guard(IEnumerator routine){
  while(true){object next;try{if(!routine.MoveNext())break;next=routine.Current;}catch(Exception e){File.AppendAllText(Result,"FAIL "+e+"\n");Finish(1);yield break;}yield return next;}
  File.AppendAllText(Result,"ALL PASSED\n");Finish(0);
 }
 void Assert(bool pass,string label){if(!pass)throw new Exception(label);File.AppendAllText(Result,"PASS "+label+"\n");}
 object Get(object target,string field)=>target.GetType().GetField(field,Flags).GetValue(target);
 Match3DRewardPopup Popup=>FindObjectOfType<Match3DRewardPopup>();
 IEnumerator Wait(Func<bool> condition){float end=Time.realtimeSinceStartup+10;while(!condition()&&Time.realtimeSinceStartup<end)yield return null;}
 void Capture(Canvas canvas){
  var camera=new GameObject("Capture camera").AddComponent<Camera>();camera.transform.position=new Vector3(0,0,-10);
  camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.15f,.2f,.25f);
  var target=new RenderTexture(720,1280,24);target.Create();camera.targetTexture=target;
  canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
  Canvas.ForceUpdateCanvases();foreach(var fit in FindObjectsOfType<Match3DUIFitContent>())fit.Fit();Canvas.ForceUpdateCanvases();
  camera.Render();var old=RenderTexture.active;RenderTexture.active=target;var texture=new Texture2D(720,1280,TextureFormat.RGB24,false);
  texture.ReadPixels(new Rect(0,0,720,1280),0,0);texture.Apply();File.WriteAllBytes(Path.GetFullPath("../contact-redeem.png"),texture.EncodeToPNG());
  RenderTexture.active=old;camera.targetTexture=null;canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.worldCamera=null;
  Destroy(texture);target.Release();Destroy(target);Destroy(camera.gameObject);
 }
 IEnumerator Check(){
  var flow=new GameObject("Flow fixture").AddComponent<Match3DFlow>();Match3DFlow.Instance=flow;
  flow.canvas=new GameObject("Canvas",typeof(RectTransform),typeof(Canvas)).GetComponent<Canvas>();flow.canvas.renderMode=RenderMode.ScreenSpaceOverlay;
  var scaler=flow.canvas.gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(720,1280);
  var account=new GameObject("Account fixture").AddComponent<Match3DInvitations>();Match3DInvitations.Instance=account;
  var gm=new GameObject("Game fixture").AddComponent<GameManager>();GameManager.Instance=gm;
  string endpoint=File.ReadAllText(Path.GetFullPath("../endpoint.txt"));
  var records=new GameObject("Records").AddComponent<Match3DGameRecords>();records.Initialize(endpoint);records.StopAllCoroutines();records.enabled=false;
  var rewards=new GameObject("Rewards").AddComponent<Match3DRewards>();rewards.Initialize(endpoint);
  Match3DDefaultFont.InitializeFallback();
  rewards.Begin(0,"bachelor","daily");yield return Wait(()=>flow.QualifiedCalls==1);
  Assert(flow.QualifiedCalls==1&&flow.QualifiedId=="fixture-qualified-run"&&flow.UnlimitedMagnet,"test account enters qualified run with server test permission");
  records.Prepare("daily",1,"2026-11-11",3,0);
  var active=(Match3DGameRecords.Record)Get(records,"active");
  Assert(active.rewardTier=="bachelor"&&active.levelVersion=="fixture-level"&&active.levelSeed==123,"qualified test record keeps tier and exact level snapshot");
  records.StartRecord();records.Matched();records.Finish("win","cleared");
  var queue=(Match3DGameRecords.Queue)Get(records,"queue");
  var pending=queue.items[queue.items.Count-1];var finish=JsonUtility.FromJson<Match3DGameRecords.Record>(pending.json);
  Assert(pending.reward&&pending.action=="finish"&&finish.toolUsage[1].test==2&&finish.toolUsage[1].inventory==0,"test finish uses reward queue and records test tools separately");
  rewards.Begin(0,"elite","daily");yield return Wait(()=>flow.QualifiedCalls==2);
  Assert(flow.QualifiedCalls==2&&!flow.UnlimitedMagnet&&account.State.unlimitedMagnet,"entry follows server permission snapshot rather than live account flag");
  rewards.Begin(0,"denied","daily");yield return Wait(()=>flow.NoticeText=="fixture quota exhausted");
  Assert(flow.QualifiedCalls==2&&flow.NoticeText=="fixture quota exhausted","denied entry shows server error without starting practice");
  flow.UnlimitedMagnet=true;records.NewRun();records.Prepare("daily",1,"2026-11-11",3,0);
  Assert(((Match3DGameRecords.Record)Get(records,"active")).levelVersion==Match3DTestPrivileges.LevelVersion,"legacy practice retains non-reward test version");
  rewards.NotifyReceivedVoucher("fixture-voucher",account.State.inviteCode);yield return Wait(()=>Popup);yield return null;
  Assert(Popup&&Popup.buttonLabel.text=="联系客服兑换"&&Popup.items.GetComponentInChildren<Match3DRewardItemView>().caption.text=="2元","server voucher shows actual amount and contact-service button");
  Canvas.ForceUpdateCanvases();var label=Popup.buttonLabel;
  label.cachedTextGenerator.Populate(label.text,label.GetGenerationSettings(label.rectTransform.rect.size));
  Assert(label.cachedTextGenerator.lineCount==1&&label.cachedTextGenerator.characterCountVisible>=label.text.Length,"contact button fits actual prefab on one line");
  Capture(flow.canvas);yield return null;
  var popup=Popup;popup.confirm.onClick.Invoke();popup.confirm.onClick.Invoke();
  Assert(flow.ServiceCalls==1&&flow.NoticeText=="请在微信小游戏中联系客服","receipt click synchronously calls real customer service once");
  yield return null;rewards.NotifyReceivedVoucher("fixture-voucher",account.State.inviteCode);yield return new WaitForSecondsRealtime(.2f);
  Assert(!Popup,"duplicate voucher does not reopen receipt");
  rewards.NotifyReceivedVoucher("missing-detail",account.State.inviteCode);yield return Wait(()=>Popup);
  Assert(Popup&&Popup.buttonLabel.text=="联系客服兑换"&&Popup.items.GetComponentInChildren<Match3DRewardItemView>().caption.text=="红包","unavailable details keep contact action without invented amount");
  Popup.confirm.onClick.Invoke();yield return null;
  flow.ShowReceivedTools(new[]{1,0,0,0});yield return Wait(()=>Popup);
  Assert(Popup&&Popup.buttonLabel.text=="领取","ordinary tool receipt retains claim label");Popup.confirm.onClick.Invoke();yield return null;
  Assert(flow.ServiceCalls==2,"tool receipt never invokes customer service");
 }
}
