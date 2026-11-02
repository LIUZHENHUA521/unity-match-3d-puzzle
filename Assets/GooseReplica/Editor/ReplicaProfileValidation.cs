using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
public static class ReplicaProfileValidation {
 const string Output="work/profile-validation";
 public static void Run(){if(!Application.isPlaying||!ReplicaFlow.Instance)throw new Exception("Start Play Mode first");Directory.CreateDirectory(Output);File.WriteAllText(Output+"/result.txt","");new GameObject("Profile validation driver").AddComponent<SourceValidationDriver>().StartCoroutine(Guard(Check()));}
 static IEnumerator Guard(IEnumerator routine){while(true){object current;try{if(!routine.MoveNext())break;current=routine.Current;}catch(Exception e){File.AppendAllText(Output+"/result.txt","FAIL "+e);Debug.LogException(e);yield break;}yield return current;}File.AppendAllText(Output+"/result.txt","ALL PASSED\n");}
 static void Assert(bool value,string name){if(!value)throw new Exception(name);File.AppendAllText(Output+"/result.txt","PASS "+name+"\n");}
 static IEnumerator Idle(ReplicaInvitations service){float until=Time.realtimeSinceStartup+20;while(service.Busy&&Time.realtimeSinceStartup<until)yield return null;Assert(!service.Busy,"network settled");}
 static IEnumerator Check(){
  var flow=ReplicaFlow.Instance;var service=ReplicaInvitations.Instance;flow.Home();service.Initialize("http://127.0.0.1:8098");service.OnInvitationBridge("{\"code\":\"profile-fixture-user\"}");yield return Idle(service);Assert(service.Ready&&service.State.createdAt>0,"server account and registration timestamp");
  var home=UnityEngine.Object.FindObjectOfType<ReplicaHomeView>();home.GetComponent<ReplicaUIView>().Get<Button>("Settings").onClick.Invoke();yield return Idle(service);var menu=UnityEngine.Object.FindObjectOfType<ReplicaHomeMenu>();menu.GetComponent<ReplicaUIView>().Get<Button>("Card").onClick.Invoke();yield return null;
  var view=UnityEngine.Object.FindObjectOfType<ReplicaUserProfileView>();Assert(view&&flow.BlocksGameplay,"personal card entry opens profile");Assert(view.gender.text=="未提供"&&view.location.text=="未提供"&&view.nickname.text=="微信玩家","missing authorization defaults");Assert(view.GetComponentsInChildren<InputField>().Length==0,"read-only profile has no edit fields");Assert(view.GetComponentsInChildren<Button>().Length==3,"only close, sync, copy controls; no camera");
  view.OnWechatProfile("{\"state\":\"ready\",\"nickName\":\"微信玩家小鹅\",\"gender\":1,\"province\":\"浙江省\",\"city\":\"杭州市\"}");Assert(view.nickname.text=="微信玩家小鹅"&&view.gender.text=="男"&&view.location.text=="浙江省 / 杭州市","authorized data binding");Assert(view.registered.text.Contains(DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd")),"server date displayed");
  string clipboard=GUIUtility.systemCopyBuffer;try{view.copy.onClick.Invoke();Assert(GUIUtility.systemCopyBuffer==service.State.inviteCode,"copy full user ID despite abbreviated display");}finally{GUIUtility.systemCopyBuffer=clipboard;}
  Assert(ReplicaUserProfileView.AllowedAvatar("https://thirdwx.qlogo.cn/mmopen/a/132")&&!ReplicaUserProfileView.AllowedAvatar("https://thirdwx.qlogo.cn.evil.example/a")&&!ReplicaUserProfileView.AllowedAvatar("http://thirdwx.qlogo.cn/a"),"avatar host policy");
  view.status.text="已同步微信资料";yield return null;Capture("authorized");
  view.OnWechatProfile("{\"state\":\"ready\",\"nickName\":\"昵称较长也会完整适配显示\",\"gender\":0}");Assert(view.gender.text=="未提供"&&view.location.text=="未提供","missing gender and location not fabricated");
  view.OnWechatProfile("{\"state\":\"denied\",\"message\":\"未获取微信资料，可点击下方按钮重试\"}");Assert(view.nickname.text=="微信玩家"&&view.avatar.texture==view.defaultAvatar,"denied resets display only");Assert(service.Ready,"denying profile preserves game login");Capture("unauthorized");
  view.GetComponent<ReplicaUIView>().Get<Button>("Close").onClick.Invoke();view.OnWechatProfile("{\"state\":\"ready\",\"nickName\":\"late\"}");Assert(view.nickname.text=="微信玩家"&&!flow.BlocksGameplay,"closed view ignores late callback and restores input");yield return null;
 }
 static void Capture(string name){
  var camera=Camera.main;var canvases=UnityEngine.Object.FindObjectsOfType<Canvas>().Where(c=>c.isRootCanvas&&c.gameObject.activeInHierarchy).ToArray();var modes=canvases.Select(c=>c.renderMode).ToArray();var cameras=canvases.Select(c=>c.worldCamera).ToArray();var distances=canvases.Select(c=>c.planeDistance).ToArray();var target=new RenderTexture(720,1280,24);var old=camera.targetTexture;var active=RenderTexture.active;target.Create();camera.targetTexture=target;
  for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=RenderMode.ScreenSpaceCamera;canvases[i].worldCamera=camera;canvases[i].planeDistance=1;}
  Canvas.ForceUpdateCanvases();foreach(var home in UnityEngine.Object.FindObjectsOfType<ReplicaHomeView>())home.SendMessage("Layout");foreach(var fit in UnityEngine.Object.FindObjectsOfType<ReplicaUIFitContent>())fit.Fit();Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;var tex=new Texture2D(720,1280,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,720,1280),0,0);tex.Apply();File.WriteAllBytes(Output+"/"+name+"-720x1280.png",tex.EncodeToPNG());camera.targetTexture=old;RenderTexture.active=active;
  for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];canvases[i].planeDistance=distances[i];}UnityEngine.Object.Destroy(tex);target.Release();UnityEngine.Object.Destroy(target);
 }
}
