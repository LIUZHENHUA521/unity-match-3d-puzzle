using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Assignments always come from the shared AB service. Missing/expired groups use the caller's default.
public sealed class Match3DABExperiments : MonoBehaviour {
    [Serializable] public class Assignment { public string testKey,group; }
    [Serializable] class Reply { public bool ok; public string owner; public Assignment[] experiments; public long expiresAt; }
    public static Match3DABExperiments Instance {get;private set;}
    public bool Ready => CurrentOwner()==owner && !string.IsNullOrEmpty(owner) && Time.realtimeSinceStartup<expires;
    public event Action Changed;
    readonly Dictionary<string,string> groups=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
    string endpoint,owner;float expires,nextAttempt;bool busy;int generation;
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');}
    void OnDestroy(){generation++;if(Instance==this)Instance=null;}
    string CurrentOwner(){var account=Match3DInvitations.Instance;return account&&account.Ready?account.State.inviteCode:null;}
    public static string GetABGroup(string testKey,string fallback=null){
        var service=Instance;
        if(!service||!service.Ready||string.IsNullOrWhiteSpace(testKey))return fallback;
        return service.groups.TryGetValue(testKey.Trim(),out var group)?group:fallback;
    }
    public void Refresh(){nextAttempt=0;}
    void Update(){
        var current=CurrentOwner();
        if(current!=owner){owner=current;generation++;groups.Clear();expires=0;nextAttempt=0;Changed?.Invoke();}
        if(expires>0&&Time.realtimeSinceStartup>=expires){groups.Clear();expires=0;Changed?.Invoke();}
        if(!busy&&!string.IsNullOrEmpty(owner)&&!string.IsNullOrEmpty(endpoint)&&Time.realtimeSinceStartup>=nextAttempt)
            StartCoroutine(Fetch(owner,generation,Match3DInvitations.Instance.RecordToken));
    }
    IEnumerator Fetch(string expectedOwner,int version,string token){
        busy=true;nextAttempt=Time.realtimeSinceStartup+30;
        using(var request=new UnityWebRequest(endpoint+"/invites/ab","POST")){
            request.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes("{}"));
            request.downloadHandler=new DownloadHandlerBuffer();request.timeout=15;
            request.SetRequestHeader("X-Goose-Progress-Display","1");
            request.SetRequestHeader("X-Pot-Combo-Praise","1");
            request.SetRequestHeader("Content-Type","application/json");request.SetRequestHeader("Authorization","Bearer "+token);
            yield return request.SendWebRequest();
            if(version==generation&&CurrentOwner()==expectedOwner){
                Reply reply=null;
                if(request.result==UnityWebRequest.Result.Success){try{reply=JsonUtility.FromJson<Reply>(request.downloadHandler.text);}catch{}}
                if(reply!=null&&reply.ok&&reply.owner==expectedOwner&&reply.experiments!=null){
                    groups.Clear();
                    foreach(var item in reply.experiments)if(item!=null&&!string.IsNullOrWhiteSpace(item.testKey)&&!string.IsNullOrWhiteSpace(item.group))groups[item.testKey.Trim()]=item.group;
                    // Monotonic session cache; no persisted assignment can leak between accounts/environments.
                    expires=Time.realtimeSinceStartup+3600;nextAttempt=expires;Changed?.Invoke();
                }else if(request.responseCode==401){
                    var account=Match3DInvitations.Instance;if(account&&account.RecordToken==token)account.RefreshRecordLogin();
                }
            }
        }
        busy=false;
    }
}
