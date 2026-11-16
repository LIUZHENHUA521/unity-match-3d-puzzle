using System;

// One completed video buys one playback. Only a failed download may reuse it.
public sealed class PotMasterReplayAccess {
    public bool Busy {get;private set;}
    string selected,paid;int revision;
    public int Begin(string id){
        if(Busy||string.IsNullOrEmpty(id))return -1;
        if(selected!=id)paid=null;
        selected=id;Busy=true;return ++revision;
    }
    public bool Current(int request)=>Busy&&revision==request;
    public bool Paid(string id)=>paid==id&&!string.IsNullOrEmpty(id);
    public bool CompleteAd(int request,bool completed){if(!Current(request))return false;if(completed)paid=selected;else Busy=false;return completed;}
    public void Failed(int request){if(Current(request))Busy=false;}
    public void Consumed(){paid=null;Busy=false;++revision;}
    public void Cancel(){selected=paid=null;Busy=false;++revision;}
    public static bool Visible(PotReplayService.Reply reply,string date)=>reply!=null&&reply.ok&&reply.total>5&&reply.date==date&&!string.IsNullOrEmpty(reply.levelVersion)&&reply.list!=null&&reply.list.Length>0;
    public static string Account(){var a=Match3DInvitations.Instance;return a&&a.Ready?a.State.inviteCode:null;}
}
