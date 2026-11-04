using UnityEngine;
using UnityEngine.UI;
public sealed class Match3DInviteView : MonoBehaviour {
    public Text count,status;public Button invite;
    Match3DInvitations service;
    public void Connect(Match3DInvitations value){service=value;service.Changed+=Refresh;Refresh();}
    void Refresh(){if(!service)return;count.text=service.Ready?"当前邀请："+service.State.invited+"个好友":"当前邀请：--";status.text=service.Status;invite.interactable=!service.Busy;}
    void OnDestroy(){if(service)service.Changed-=Refresh;}
}
