// Only dependencies outside the tested reward/record/UI components are fixtures.
using System;
using System.Collections;
using UnityEngine;
public sealed class Match3DInvitations:MonoBehaviour {
 public static Match3DInvitations Instance; public bool Ready=true; public string RecordToken="fixture",Status;
 public StateData State=new StateData(); public class StateData {public string inviteCode="fixture-owner";public bool unlimitedMagnet=true;}
 public IEnumerator EnsureReady(Action<bool> complete,Func<bool> cancel=null){complete(Ready);yield break;}
 public void RefreshRecordLogin(){}
}
public partial class Match3DFlow:MonoBehaviour {
 public static Match3DFlow Instance; public Canvas canvas; public string ScreenName="home",NoticeText;
 public bool resultShown,IsPvp,LocalPreview,TimeReviveUsed,BlocksGameplay,UnlimitedMagnet;
 public bool UnlimitedTools=>UnlimitedMagnet;public int ScreenVersion,QualifiedCalls,ServiceCalls;
 public string QualifiedId; public void Notice(string text){NoticeText=text;}
 public void ShowHomeFeatureNotice(string text){ServiceCalls++;NoticeText=text;}
 public void HomeClick(){} public string SourceEnvironmentForTheme(int theme)=>"fixture";
 public void BeginQualified(int theme,string id,string tier,string version,Match3DDailyLevel level=null,bool testToolsEnabled=false){
  QualifiedCalls++;QualifiedId=id;UnlimitedMagnet=testToolsEnabled;
  Match3DGameRecords.Instance.QualifiedRun(id,tier,version,level);
 }
 public void ChooseTestTheme(int theme,Match3DDailyLevel level){throw new Exception("Test account diverted into practice");}
}
[Serializable] public sealed class Match3DDailyLevel {public string date,version,tier,generatorVersion,modelThemeId;public int seed;public bool IsValid(int pool)=>true;}
public static class Match3DDailySnapshot {public const string ResourceVersion="fixture-opening";}
public static class Match3DModelThemes {
 public class Profile {public GameObject[] prefabs=new GameObject[1];}
 public static bool TryGet(string id,string environment,out Profile profile){profile=new Profile();return true;}
}
public static class BoardTools {
 public const int ToolCount=4;public static int UsedCount(int i)=>2;public static int TestUses(int i)=>2;public static int AdUses(int i)=>0;
}
public sealed class GameManager:MonoBehaviour {public static GameManager Instance;public float currentLevelTime;public bool GameIsPlaying()=>false;}
public static class Match3DModalMotion {public static void Play(GameObject root){}}
