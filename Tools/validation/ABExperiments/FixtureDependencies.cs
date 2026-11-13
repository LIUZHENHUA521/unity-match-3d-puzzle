using UnityEngine;
public class Match3DInvitations:MonoBehaviour {
 public static Match3DInvitations Instance;
 public class Reply {public string inviteCode="owner-a";}
 public Reply State=new Reply();public bool Ready=true;public string RecordToken="token-a";public int Refreshes;
 public void RefreshRecordLogin(){Refreshes++;Ready=false;}
}
