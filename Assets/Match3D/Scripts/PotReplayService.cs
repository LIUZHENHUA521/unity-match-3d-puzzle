using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public sealed class PotReplayService : MonoBehaviour {
    [Serializable] public class Summary {public string id,recordId,status,date,tier,levelVersion,resourceVersion;public int stage;public float duration;}
    [Serializable] public class Reply {public bool ok,hasMore;public string error;public Summary[] list;public PotReplayData detail;}
    [Serializable] class RequestBody {public string kind,date,tier,levelVersion,id,recordId,content;public int skip,index,count;}
    public static PotReplayService Instance {get;private set;}
    string endpoint;int nextUpload;public string Status {get;private set;}="";
    public static string DirectoryPath=>Path.Combine(Application.persistentDataPath,"pot-replays-v1");
    public static string PathFor(string owner,string id,string suffix){PotReplayFiles.Ensure();return Path.Combine(DirectoryPath,owner+"_"+id+suffix);}
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');try{Recover();}catch(Exception e){Status="录像本地保存暂不可用";Debug.LogWarning(e.Message);}StartCoroutine(Upload());}
    public void Enqueue(string owner,PotReplayData data){
        string target=PathFor(owner,data.recordId,".pending");var json=JsonUtility.ToJson(data);
        if(Encoding.UTF8.GetByteCount(json)>16*1024*1024)throw new Exception("录像超出保存上限");
        PotReplayFiles.Write(target+".tmp",json);PotReplayFiles.Move(target+".tmp",target);Status="录像等待同步";
    }
    void Recover(){
        PotReplayFiles.Ensure();
        foreach(var path in PotReplayFiles.List("*.recording"))try{
            if(PotReplayFiles.Exists(Path.ChangeExtension(path,".pending"))){PotReplayFiles.Delete(path);continue;}
            var lines=PotReplayFiles.Read(path).Split(new[]{'\n'},StringSplitOptions.RemoveEmptyEntries);var data=JsonUtility.FromJson<PotReplayData>(lines[0]);
            data.frames.Clear();for(int i=1;i<lines.Length;i++){try{var frame=JsonUtility.FromJson<PotReplayFrame>(lines[i]);if(frame?.items!=null)data.frames.Add(frame);}catch{break;}}
            if(data.frames.Count==0)continue;
            // A crash leaves a partial replay. Reconstruct a final checkpoint, never call it a win.
            var final=data.frames[data.frames.Count-1];var state=PotReplayState.At(data,final.t,out _);
            final.full=true;final.items=state.Values.OrderBy(x=>x.id).ToList();data.duration=final.t;data.status="incomplete";
            data.events.Clear();data.events.Add(new PotReplayEvent{seq=0,t=final.t,type="finish"});
            string owner=Path.GetFileName(path).Split('_')[0];Enqueue(owner,data);PotReplayFiles.Delete(path);
        }catch(Exception e){Debug.LogWarning("Pot replay recovery: "+e.Message);}
    }
    IEnumerator Upload(){
        while(true){
            yield return new WaitForSecondsRealtime(3);
            var account=Match3DInvitations.Instance;if(!account||!account.Ready)continue;
            string owner=account.State.inviteCode;
            string[] files;try{files=PotReplayFiles.List(owner+"_*.pending");}catch{Status="录像本地保存暂不可用";continue;}if(files.Length==0)continue;
            string path=files[nextUpload++%files.Length],text=null;try{text=PotReplayFiles.Read(path);}catch{continue;}
            var recordId=Path.GetFileNameWithoutExtension(path).Substring(owner.Length+1);
            // Bound UTF-8 bytes and preserve surrogate pairs at chunk boundaries.
            const int chars=32768;var chunks=new List<string>();for(int start=0;start<text.Length;){int length=Math.Min(chars,text.Length-start);if(start+length<text.Length&&char.IsHighSurrogate(text[start+length-1]))length--;chunks.Add(text.Substring(start,length));start+=length;}int count=chunks.Count;bool failed=false;long failureCode=0;
            for(int i=0;i<count;i++){
                Reply reply=null;long code=0;
                yield return Send("chunk",new RequestBody{recordId=recordId,index=i,content=chunks[i]},r=>reply=r,c=>code=c,owner);
                if(reply==null||!reply.ok){failed=true;failureCode=code;Status=reply?.error??"录像同步稍后重试";break;}
                yield return new WaitForSecondsRealtime(1);
            }
            if(!failed){Reply reply=null;yield return Send("finish",new RequestBody{recordId=recordId,count=count},r=>reply=r,c=>failureCode=c,owner);failed=reply==null||!reply.ok;Status=failed?reply?.error??"录像同步稍后重试":"录像已同步";}
            try{if(!failed)PotReplayFiles.Delete(path);else if(failureCode==400||failureCode==413)PotReplayFiles.Move(path,path+".rejected");}catch(Exception e){Debug.LogWarning("Replay queue: "+e.Message);}
            if(failed)yield return new WaitForSecondsRealtime(30);
        }
    }
    public IEnumerator List(bool masters,int skip,Match3DDailyLevel level,Action<Reply> done,string tier="bachelor"){
        if(masters){
            Match3DRewards.Reply daily=null;
            yield return Match3DRewards.Instance.Request("daily","{\"themeId\":\"daily\",\"tier\":\""+tier+"\",\"levelProtocol\":3,\"openingVersion\":\""+Match3DDailySnapshot.ResourceVersion+"\"}",r=>daily=r,true);
            if(daily==null||!daily.ok||daily.level==null){done(new Reply{error=daily?.error??"每日关卡暂不可用"});yield break;}level=daily.level;
        }
        yield return Send("list",new RequestBody{kind=masters?"masters":"mine",skip=skip,date=level?.date,tier=level?.tier,levelVersion=level?.version},done);
    }
    public IEnumerator Detail(string id,Action<Reply> done){yield return Send("detail",new RequestBody{id=id},done);}
    IEnumerator Send(string action,RequestBody body,Action<Reply> done,Action<long> status=null,string expectedOwner=null){
        var account=Match3DInvitations.Instance;
        if(!account||!account.Ready||expectedOwner!=null&&account.State.inviteCode!=expectedOwner){done(new Reply{error="请先登录"});yield break;}
        string owner=account.State.inviteCode,token=account.RecordToken;
        using(var request=new UnityWebRequest(endpoint+"/pot-replays/"+action,"POST")){
            request.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(body)));request.downloadHandler=new DownloadHandlerBuffer();request.timeout=45;
            request.SetRequestHeader("Content-Type","application/json");request.SetRequestHeader("Authorization","Bearer "+token);
            yield return request.SendWebRequest();status?.Invoke(request.responseCode);
            if(!account.Ready||account.State.inviteCode!=owner){done(new Reply{error="账号已切换"});yield break;}
            Reply reply=null;try{reply=JsonUtility.FromJson<Reply>(request.downloadHandler.text);}catch{}
            if(request.responseCode==401)account.RefreshRecordLogin();done(reply??new Reply{error="网络异常，请重试"});
        }
    }
}
