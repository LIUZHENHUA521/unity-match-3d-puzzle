using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using P=Match3DRewardAcquisition;
public static class PotAcquisitionValidation {
 static int checks;
 static void Check(bool ok,string name){if(!ok)throw new Exception(name);checks++;Debug.Log("PASS "+name);}
 public static void Run(){
  try{Tests();Directory.CreateDirectory("work");File.WriteAllText("work/pot-acquisition-checks.txt","PASS "+checks+" cadence checks");EditorApplication.Exit(0);}
  catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
 }
 static void Tests(){
  P.SetEnvironment("pot-validation-"+Guid.NewGuid().ToString("N"));
  var host=new GameObject("Pot acquisition validation");var account=host.AddComponent<Match3DInvitations>();
  typeof(Match3DInvitations).GetProperty("Instance").SetValue(null,account);
  typeof(Match3DInvitations).GetProperty("State").SetValue(account,new Match3DInvitations.Reply{inviteCode="pot-test",inventory=new int[4]});
  typeof(Match3DInvitations).GetField("token",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(account,"test");
  string key=P.CounterKey(P.Entry.Tool,"pot-test");
  Check(key==P.CounterKey(P.Entry.Revive,"pot-test"),"shared counter");
  foreach(int n in new[]{1,3,100}){
   P.Configure(new P.Configuration{adsBeforeShare=n});PlayerPrefs.DeleteKey(key);
   for(int i=0;i<n;i++){var a=P.Next(i%2==0?P.Entry.Tool:P.Entry.Revive);Check(a.route==P.Route.Ad,"ad before threshold");P.Commit(a);P.Commit(a);Check(PlayerPrefs.GetInt(key)==i+1,"single commit");}
   Check(P.Next(P.Entry.Tool).route==P.Route.Share,"share at threshold");
   for(int i=0;i<3;i++){var a=P.Next(P.Entry.Revive);Check(a.route==P.Route.Ad,"revival never share");P.Commit(a);}
   P.ResetRound();Check(P.Next(P.Entry.Tool).route==P.Route.Share,"pending survives revival and round");
   var share=P.Next(P.Entry.Tool);P.Commit(share);P.Commit(share);Check(PlayerPrefs.GetInt(key)==0&&P.Next(P.Entry.Tool).route==P.Route.Ad,"share resets without stacking");
  }
  P.Configure(new P.Configuration{adsBeforeShare=3});
  var stale=P.Next(P.Entry.Tool);P.ApplyRemote(new P.PotRemoteConfiguration{adsBeforeShare=4});Check(!P.IsCurrent(stale),"idle offer invalidated");
  var playing=P.Next(P.Entry.Tool);Check(P.TryBegin(playing),"begin");Check(!P.TryBegin(P.Next(P.Entry.Revive)),"global lock");
  P.ApplyRemote(new P.PotRemoteConfiguration{adsBeforeShare=1});Check(P.IsCurrent(playing),"active offer keeps snapshot");P.Commit(playing);P.End(playing);
  Check(P.Next(P.Entry.Tool).route==P.Route.Share,"new config used next");
  P.ApplyRemote(new P.PotRemoteConfiguration{adsBeforeShare=0});Check(P.Config.adsBeforeShare==1,"invalid config ignored");
  P.Configure(new P.Configuration{adsEnabled=false,revive=new P.Rules{freeUsesPerRound=99}});
  Check(P.Next(P.Entry.Revive).route==P.Route.Unavailable,"revive cannot fallback to free or share");
  PlayerPrefs.SetInt(key,0);Check(P.Next(P.Entry.Tool).route==P.Route.Unavailable,"no early share without ads");
  PlayerPrefs.SetInt(key,3);Check(P.Next(P.Entry.Tool).route==P.Route.Share,"earned share with ads disabled");
  P.Configure(new P.Configuration{sharesEnabled=false});Check(P.Next(P.Entry.Tool).route==P.Route.Ad,"disabled share keeps ads");
  P.Configure(new P.Configuration());PlayerPrefs.SetInt(key,0);
  var video=host.AddComponent<Match3DRewardedVideo>();Action<bool,string> complete=null;
  Match3DRewardedVideo.EditorShowOverride=done=>complete=done;
  var failed=P.Next(P.Entry.Tool);video.Acquire(failed,false,(ok,e)=>{if(ok&&P.IsCurrent(failed))P.Commit(failed);});complete(false,"cancelled");Check(PlayerPrefs.GetInt(key)==0,"failure no progress");
  video.Acquire(failed,false,(ok,e)=>{if(ok&&P.IsCurrent(failed))P.Commit(failed);});complete(true,null);complete(true,null);Check(PlayerPrefs.GetInt(key)==1,"retry exactly once");
  var switched=P.Next(P.Entry.Tool);video.Acquire(switched,false,(ok,e)=>{if(ok&&P.IsCurrent(switched))P.Commit(switched);});account.State.inviteCode="pot-other";complete(true,null);Check(PlayerPrefs.GetInt(key)==1&&PlayerPrefs.GetInt(P.CounterKey(P.Entry.Tool,"pot-other"),0)==0,"switch identity no cross-account grant");
  account.State.inviteCode="pot-test";var cancel=P.Next(P.Entry.Tool);video.Acquire(cancel,false,(ok,e)=>{if(ok)P.Commit(cancel);});video.Cancel();complete(true,null);Check(PlayerPrefs.GetInt(key)==1,"cancel ignores callback");
  P.ApplyRemote(new P.PotRemoteConfiguration{adsBeforeShare=7});P.SetEnvironment("pot-other-environment");Check(P.Config.adsBeforeShare==3,"environment isolated");
  Match3DRewardedVideo.EditorShowOverride=null;UnityEngine.Object.DestroyImmediate(host);
 }
}
