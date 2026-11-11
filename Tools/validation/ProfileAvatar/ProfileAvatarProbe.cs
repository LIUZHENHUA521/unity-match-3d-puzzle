// Copy into an isolated Unity project's Assets/Editor alongside the actual profile view.
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[InitializeOnLoad]
public static class ProfileAvatarProbe {
 const string Key="ProfileAvatarProbe.Pending";
 static ProfileAvatarProbe(){EditorApplication.playModeStateChanged+=state=>{
  if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false)){
   SessionState.SetBool(Key,false);new GameObject("Avatar regression").AddComponent<ProfileAvatarProbeRunner>();
  }
 };}
 public static void Run(){SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
}
public sealed class ProfileAvatarProbeRunner:MonoBehaviour {
 const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 Match3DUserProfileView view;
 string Result=>Path.GetFullPath("../result.txt");
 void Start(){File.WriteAllText(Result,"");StartCoroutine(Guard(Check()));}
 IEnumerator Guard(IEnumerator routine){
  while(true){object next;try{if(!routine.MoveNext())break;next=routine.Current;}catch(Exception e){File.AppendAllText(Result,"FAIL "+e+"\n");EditorApplication.Exit(1);yield break;}yield return next;}
  File.AppendAllText(Result,"ALL PASSED\n");EditorApplication.Exit(0);
 }
 void Assert(bool ok,string label){if(!ok)throw new Exception(label);File.AppendAllText(Result,"PASS "+label+"\n");}
 object Call(string name,params object[] args)=>typeof(Match3DUserProfileView).GetMethod(name,Flags).Invoke(view,args);
 void Set(string name,object value)=>typeof(Match3DUserProfileView).GetField(name,Flags).SetValue(view,value);
 object Get(string name)=>typeof(Match3DUserProfileView).GetField(name,Flags).GetValue(view);
 string Png(int size){var texture=new Texture2D(size,size,TextureFormat.RGBA32,false);var path=Path.GetFullPath("../avatar-"+size+".png");File.WriteAllBytes(path,texture.EncodeToPNG());Destroy(texture);return new Uri(path).AbsoluteUri;}
 IEnumerator Check(){
  view=new GameObject("Profile").AddComponent<Match3DUserProfileView>();
  view.avatar=new GameObject("Avatar",typeof(RectTransform)).AddComponent<RawImage>();
  view.status=new GameObject("Status",typeof(RectTransform)).AddComponent<Text>();
  view.defaultAvatar=new Texture2D(2,2);view.avatar.texture=view.defaultAvatar;
  Assert(Match3DUserProfileView.AllowedAvatar("https://thirdwx.qlogo.cn/mmopen/test/132"),"accept HTTPS WeChat avatar");
  Assert(!Match3DUserProfileView.AllowedAvatar("https://qlogo.cn.evil.test/avatar")&&!Match3DUserProfileView.AllowedAvatar("http://thirdwx.qlogo.cn/avatar"),"retain host and HTTPS checks");
  foreach(int size in new[]{132,640}){
   Call("StopAvatar");var routine=(IEnumerator)Call("DownloadAvatar",Png(size),(int)Get("avatarVersion"));
   while(routine.MoveNext())yield return routine.Current;
   Assert(view.avatar.texture&&view.avatar.texture.width==size&&Get("ownedAvatar")!=null,"display downloaded "+size+"px avatar");
  }
  Call("StopAvatar");var large=(IEnumerator)Call("DownloadAvatar",Png(641),(int)Get("avatarVersion"));while(large.MoveNext())yield return large.Current;
  Assert(Get("ownedAvatar")==null&&view.status.text.Contains("头像图片暂不可用"),"oversized image has actionable feedback");
  var missing=new Uri(Path.GetFullPath("../missing.png")).AbsoluteUri;
  Set("avatarUrl",missing);view.status.gameObject.SetActive(false);
  var failure=(IEnumerator)Call("DownloadAvatar",missing,(int)Get("avatarVersion"));while(failure.MoveNext())yield return failure.Current;
  Assert(Get("avatarUrl")==null&&Get("avatarRequest")==null&&view.status.gameObject.activeSelf&&view.status.text.Contains("头像加载失败"),"failed download clears cache and shows failure");
  // A previous failed URL must pass through loading/validation again, never return as if cached.
  Set("avatarUrl",missing);view.status.text="sentinel";Call("LoadAvatar",missing);
  Assert(view.status.text.Contains("头像地址暂不支持")&&Get("avatarUrl")==null,"same URL without texture or request is retried");
  string allowed="https://thirdwx.qlogo.cn/mmopen/test/132";Set("avatarUrl",allowed);var pending=UnityWebRequest.Get(allowed);Set("avatarRequest",pending);
  Call("LoadAvatar",allowed);Assert(ReferenceEquals(Get("avatarRequest"),pending),"duplicate tap preserves in-flight request");
  Call("StopAvatar");Assert(Get("avatarRequest")==null&&Get("avatarUrl")==null,"close/cancel releases request and cached URL");
  view.status.gameObject.SetActive(false);Call("LoadAvatar",new object[]{null});Assert(view.status.text.Contains("微信未返回头像")&&view.avatar.texture==view.defaultAvatar,"missing avatar retains default and reports absence");
  var stale=(IEnumerator)Call("DownloadAvatar",Png(132),(int)Get("avatarVersion")-1);while(stale.MoveNext())yield return stale.Current;
  Assert(Get("ownedAvatar")==null&&view.avatar.texture==view.defaultAvatar,"stale image cannot replace current avatar");
  Set("avatarRequest",null);Destroy(view.gameObject);Destroy(view.avatar.gameObject);Destroy(view.status.gameObject);
 }
}
