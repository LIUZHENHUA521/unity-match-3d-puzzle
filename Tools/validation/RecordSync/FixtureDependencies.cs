using UnityEngine;
public sealed class Match3DInvitations:MonoBehaviour {
 public static Match3DInvitations Instance;public bool Ready=true;public string RecordToken="fixture";public int Refreshes;
 public StateData State=new StateData();public class StateData {public string inviteCode="owner-a";}
 public void RefreshRecordLogin(){Refreshes++;}
}
public sealed class Match3DFlow:MonoBehaviour {
 public static Match3DFlow Instance;public bool IsPvp,BlocksGameplay,UnlimitedMagnet,LocalPreview,TimeReviveUsed;
 public int Notices;public string LastNotice;public void Notice(string text){Notices++;LastNotice=text;}
}
public sealed class Match3DRewards:MonoBehaviour {
 public static Match3DRewards Instance;public void NotifyReceivedVoucher(string id,string owner){}
}
public sealed class Match3DDailyLevel {public string version,generatorVersion,tier;public int seed;}
public static class Match3DTestPrivileges {public const string LevelVersion="test";}
public static class BoardTools {public const int ToolCount=4;public static int UsedCount(int i)=>0;public static int AdUses(int i)=>0;public static int TestUses(int i)=>0;}
public sealed class GameManager:MonoBehaviour {public static GameManager Instance;public float currentLevelTime;public bool GameIsPlaying()=>false;}
