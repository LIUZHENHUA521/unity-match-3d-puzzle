using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// WeChat profile is display-only and stays in memory for this popup. Account identity stays server-owned.
public sealed class Match3DUserProfileView : MonoBehaviour {
 [Serializable] public class WechatData {public string state,message,nickName,avatarUrl,country,province,city;public int gender;}
 public Text nickname,userId,gender,location,registered,status,syncLabel;
 public RawImage avatar;public Texture defaultAvatar;public Button sync,copy;
 Match3DInvitations service;string account,avatarUrl;bool bridgeOpen;float deadline;Rect lastRect;Vector2 lastScreen;Texture2D ownedAvatar;UnityWebRequest avatarRequest;int avatarVersion;
#if UNITY_WEBGL && !UNITY_EDITOR
 [DllImport("__Internal")] static extern void GooseProfileOpen(string target);
 [DllImport("__Internal")] static extern void GooseProfileBounds(float x,float y,float w,float h,float screenW,float screenH);
 [DllImport("__Internal")] static extern void GooseProfileClose();
 [DllImport("__Internal")] static extern void GooseProfileCopy(string value);
#endif
 public void Connect(Match3DInvitations value){
  gameObject.name="UserProfile-"+Guid.NewGuid().ToString("N");service=value;service.Changed+=RefreshAccount;sync.onClick.AddListener(Sync);copy.onClick.AddListener(CopyId);ResetProfile();RefreshAccount();
 }
 void ResetProfile(){nickname.text="微信玩家";gender.text=location.text="未提供";avatar.texture=defaultAvatar;avatarUrl=null;StopAvatar();}
 void RefreshAccount(){
  bool ready=service&&service.Ready;copy.interactable=ready;sync.interactable=!service||!service.Busy;
  string next=ready?service.State.inviteCode:null;
  if(next!=account){CloseBridge();ResetProfile();account=next;}
  userId.text=ready?ShortId(account):"未登录";
  registered.text=ready&&service.State.createdAt>0?DateTimeOffset.FromUnixTimeMilliseconds(service.State.createdAt).ToOffset(TimeSpan.FromHours(8)).ToString("yyyy-MM-dd HH:mm:ss"):"未提供";
  if(!ready){syncLabel.text="登录后同步资料";status.text=service!=null?service.Status:"账号服务尚未连接";return;}
  syncLabel.text="同步微信资料";if(!bridgeOpen)OpenBridge();
 }
 public static string ShortId(string id)=>string.IsNullOrEmpty(id)?"未登录":id.Length>18?id.Substring(0,16)+"…":id;
 void OpenBridge(){
  bridgeOpen=true;lastRect=Rect.zero;lastScreen=Vector2.zero;deadline=Time.realtimeSinceStartup+15;status.text="正在读取微信资料…";
#if UNITY_WEBGL && !UNITY_EDITOR
  GooseProfileOpen(gameObject.name);
#else
  deadline=0;status.text="微信授权需在小游戏中进行";
#endif
 }
 void Sync(){Match3DFlow.Instance.HomeClick();if(!service||!service.Ready){service?.Refresh();return;}
#if !UNITY_WEBGL || UNITY_EDITOR
  status.text="微信授权需在小游戏中进行";
#else
  // Normally the native transparent user-info button handles the physical tap.
  if(!bridgeOpen)OpenBridge();
#endif
 }
 void CopyId(){if(!service||!service.Ready||string.IsNullOrEmpty(account))return;Match3DFlow.Instance.HomeClick();
#if UNITY_WEBGL && !UNITY_EDITOR
  GooseProfileCopy(account);
#else
  GUIUtility.systemCopyBuffer=account;status.text="用户 ID 已复制";
#endif
 }
 void LateUpdate(){
  if(deadline>0&&Time.realtimeSinceStartup>deadline){deadline=0;status.text="读取超时，可点击同步微信资料重试";}
#if UNITY_WEBGL && !UNITY_EDITOR
  if(!bridgeOpen||!sync||!sync.interactable)return;
  var corners=new Vector3[4];((RectTransform)sync.transform).GetWorldCorners(corners);var canvas=GetComponentInParent<Canvas>();var camera=canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera;
  Vector2 a=RectTransformUtility.WorldToScreenPoint(camera,corners[0]),b=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
  var rect=new Rect(a.x,Screen.height-b.y,b.x-a.x,b.y-a.y);var size=new Vector2(Screen.width,Screen.height);
  if(rect!=lastRect||size!=lastScreen){lastRect=rect;lastScreen=size;GooseProfileBounds(rect.x,rect.y,rect.width,rect.height,size.x,size.y);}
#endif
 }
 [UnityEngine.Scripting.Preserve] public void OnWechatProfile(string json){
  if(!isActiveAndEnabled||!bridgeOpen||!service||!service.Ready||service.State.inviteCode!=account)return;
  WechatData data;try{data=JsonUtility.FromJson<WechatData>(json);}catch{return;}if(data==null)return;deadline=0;
  if(data.state=="ready"){
   nickname.text=string.IsNullOrWhiteSpace(data.nickName)?"微信玩家":Limit(data.nickName,64);gender.text=data.gender==1?"男":data.gender==2?"女":"未提供";
   var parts=new List<string>();foreach(var part in new[]{data.country,data.province,data.city})if(!string.IsNullOrWhiteSpace(part)&&!parts.Contains(part))parts.Add(Limit(part,48));location.text=parts.Count==0?"未提供":string.Join(" / ",parts);
   status.text="已同步微信资料";LoadAvatar(data.avatarUrl);
  }else {if(data.state=="denied"||data.state=="authorize")ResetProfile();status.text=string.IsNullOrEmpty(data.message)?"微信暂未提供资料":Limit(data.message,120);}
 }
 static string Limit(string s,int count)=>s.Length>count?s.Substring(0,count):s;
 public static bool AllowedAvatar(string url){if(!Uri.TryCreate(url,UriKind.Absolute,out var uri))return false;return uri.Scheme=="https"&&string.IsNullOrEmpty(uri.UserInfo)&&uri.IsDefaultPort&&(uri.Host=="qlogo.cn"||uri.Host.EndsWith(".qlogo.cn",StringComparison.OrdinalIgnoreCase));}
 void LoadAvatar(string url){if(url==avatarUrl)return;StopAvatar();avatar.texture=defaultAvatar;avatarUrl=url;if(AllowedAvatar(url))StartCoroutine(DownloadAvatar(url,avatarVersion));}
 IEnumerator DownloadAvatar(string url,int version){
  using(var request=UnityWebRequestTexture.GetTexture(url,true)){avatarRequest=request;request.timeout=10;request.redirectLimit=0;yield return request.SendWebRequest();
   if(version!=avatarVersion)yield break;avatarRequest=null;
   if(request.result==UnityWebRequest.Result.Success){var texture=DownloadHandlerTexture.GetContent(request);if(texture&&texture.width<=512&&texture.height<=512){ownedAvatar=texture;avatar.texture=texture;}else if(texture)Destroy(texture);}
  }
 }
 void StopAvatar(){avatarVersion++;if(avatarRequest!=null){avatarRequest.Abort();avatarRequest.Dispose();avatarRequest=null;}StopAllCoroutines();if(ownedAvatar){Destroy(ownedAvatar);ownedAvatar=null;}}
 void CloseBridge(){if(!bridgeOpen)return;bridgeOpen=false;deadline=0;
#if UNITY_WEBGL && !UNITY_EDITOR
  GooseProfileClose();
#endif
 }
 void OnDisable(){CloseBridge();StopAvatar();}
 void OnDestroy(){if(service)service.Changed-=RefreshAccount;}
}
