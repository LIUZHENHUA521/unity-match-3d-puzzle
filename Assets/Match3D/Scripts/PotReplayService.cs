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
    [Serializable] public class Reply {public bool ok,hasMore;public string error,id,content;public int total;public bool ready;public UploadTicket upload;public Summary[] list;public PotReplayData detail;}
    [Serializable] class RequestBody {public string kind,date,tier,levelVersion,id,recordId,content;public int skip,index,count,offset;}
    [Serializable] public class UploadTicket {public string url,authorization,token,cosFileId,cloudPath,key,iv;}
    public const int MaxJsonBytes=64*1024*1024;
    public static PotReplayService Instance {get;private set;}
    string endpoint;int nextUpload;public string Status {get;private set;}="";
    public static string DirectoryPath=>Path.Combine(Application.persistentDataPath,"pot-replays-v1");
    public static string PathFor(string owner,string id,string suffix){PotReplayFiles.Ensure();return Path.Combine(DirectoryPath,owner+"_"+id+suffix);}
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');try{Recover();}catch(Exception e){Status="录像本地保存暂不可用";Debug.LogWarning(e.Message);}StartCoroutine(Upload());}
    public void Enqueue(string owner,PotReplayData data){
        if(data.stage!=2)return;
        string target=PathFor(owner,data.recordId,".pending");var json=JsonUtility.ToJson(data);
        int bytes=Encoding.UTF8.GetByteCount(json);if(bytes>MaxJsonBytes)throw new Exception("录像展开大小超过64MiB");
        if(PotReplayFiles.List().Sum(x=>PotReplayFiles.Size(x))+bytes>128L*1024*1024)throw new Exception("录像本地缓存已满，请联网同步后重试");
        PotReplayFiles.Write(target+".tmp",json);PotReplayFiles.Move(target+".tmp",target);Status="录像等待同步";
    }
    void Recover(){
        PotReplayFiles.Ensure();
        foreach(var path in PotReplayFiles.List("*.recording"))try{
            if(PotReplayFiles.Exists(Path.ChangeExtension(path,".pending"))){PotReplayFiles.Delete(path);continue;}
            var lines=PotReplayFiles.Read(path).Split(new[]{'\n'},StringSplitOptions.RemoveEmptyEntries);var data=JsonUtility.FromJson<PotReplayData>(lines[0]);
            if(data.stage!=2){PotReplayFiles.Delete(path);continue;}
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
            string path=files[nextUpload++%files.Length];
            yield return Match3DStartupRunner.Run(UploadOne(owner,path),e=>{Status="录像同步失败，已保留文件等待重试";Debug.LogWarning("Pot replay upload: "+e.Message);});
        }
    }
    IEnumerator UploadOne(string owner,string path){
        string text=PotReplayFiles.Read(path);
            var recordId=Path.GetFileNameWithoutExtension(path).Substring(owner.Length+1);
            // Old queued first-stage files are no longer uploaded; existing cloud replays remain available.
            Summary header=null;try{header=JsonUtility.FromJson<Summary>(text);}catch{}
            if(header==null||header.stage!=2){PotReplayFiles.Move(path,path+".skipped");yield break;}
            header=null;
            Reply reply=null;long code=0;
            yield return Send("upload-complete",new RequestBody{recordId=recordId},r=>reply=r,c=>code=c,owner);
            if(reply!=null&&reply.ok){PotReplayFiles.Delete(path);Status="录像已同步";yield break;}
            // A previous PUT may already be complete. Retry finalization first, without re-uploading.
            if(code!=404&&code!=410){Status=reply?.error??"录像同步稍后重试";if(code==400||code==413){PotReplayFiles.Move(path,path+".rejected");RecordingFailed(Status);}yield return new WaitForSecondsRealtime(30);yield break;}
            yield return Send("upload-start",new RequestBody{recordId=recordId},r=>reply=r,c=>code=c,owner);
            if(reply==null||!reply.ok){Status=reply?.error??"录像同步稍后重试";yield return new WaitForSecondsRealtime(30);yield break;}
            if(reply.ready){PotReplayFiles.Delete(path);Status="录像已同步";yield break;}
            var ticket=reply.upload;byte[] packed=null;
            try{if(ticket==null||!Uri.TryCreate(ticket.url,UriKind.Absolute,out var uri)||uri.Scheme!="https")throw new Exception("录像上传地址无效");packed=PotReplayTransfer.Pack(text,ticket.key,ticket.iv);}
            catch(Exception e){RecordingFailed(e.Message);PotReplayFiles.Move(path,path+".rejected");}
            if(packed==null)yield break;
            Status="录像正在上传云存储";bool uploaded=false;
            using(var request=UnityWebRequest.Put(ticket.url,packed)){
                request.timeout=180;request.SetRequestHeader("Content-Type","application/octet-stream");
                request.SetRequestHeader("Signature",ticket.authorization);request.SetRequestHeader("Authorization",ticket.authorization);
                request.SetRequestHeader("x-cos-security-token",ticket.token);request.SetRequestHeader("x-cos-meta-fileid",ticket.cosFileId);
                request.SetRequestHeader("key",Uri.EscapeDataString(ticket.cloudPath));
                yield return request.SendWebRequest();uploaded=request.result==UnityWebRequest.Result.Success;
                if(!uploaded)Status="云存储上传失败，已保留录像等待重试";
            }
            if(uploaded){yield return Send("upload-complete",new RequestBody{recordId=recordId},r=>reply=r,c=>code=c,owner);
                if(reply!=null&&reply.ok){PotReplayFiles.Delete(path);Status="录像已同步";}
                else {Status=reply?.error??"录像等待校验重试";if(code==400||code==413){PotReplayFiles.Move(path,path+".rejected");RecordingFailed(Status);}}
            }
            yield return new WaitForSecondsRealtime(30);
    }
    public IEnumerator List(bool masters,int skip,Match3DDailyLevel level,Action<Reply> done,string tier="bachelor"){
        if(masters){
            Match3DRewards.Reply daily=null;
            yield return Match3DRewards.Instance.Request("daily","{\"themeId\":\"daily\",\"tier\":\""+tier+"\",\"levelProtocol\":3,\"openingVersion\":\""+Match3DDailySnapshot.ResourceVersion+"\"}",r=>daily=r,true);
            if(daily==null||!daily.ok||daily.level==null){done(new Reply{error=daily?.error??"每日关卡暂不可用"});yield break;}level=daily.level;
        }
        yield return Send("list",new RequestBody{kind=masters?"masters":"mine",skip=skip,date=level?.date,tier=level?.tier,levelVersion=level?.version},done);
    }
    public void RecordingFailed(string reason){Status="回放保存失败："+reason;Debug.LogWarning(Status);}
    public IEnumerator Detail(string id,Action<Reply> done){
        using(var bytes=new MemoryStream()){
            int total=1;
            while(bytes.Length<total){
                Reply part=null;yield return Send("download",new RequestBody{id=id,offset=(int)bytes.Length},r=>part=r);
                if(part==null||!part.ok){done(part??new Reply{error="录像下载失败"});yield break;}
                string error=null;
                try{if(part.total<1||part.total>PotReplayTransfer.MaxFileBytes||bytes.Length>0&&part.total!=total)throw new IOException("录像大小无效");total=part.total;
                    var chunk=Convert.FromBase64String(part.content);if(chunk.Length!=Math.Min(196608,total-bytes.Length))throw new IOException("录像下载不完整");bytes.Write(chunk,0,chunk.Length);
                }catch(Exception e){error=e.Message;}
                if(error!=null){done(new Reply{error=error});yield break;}
            }
            Reply result;try{result=new Reply{ok=true,detail=JsonUtility.FromJson<PotReplayData>(PotReplayTransfer.Unpack(bytes.ToArray()))};}catch(Exception e){result=new Reply{error=e.Message};}done(result);
        }
    }
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
