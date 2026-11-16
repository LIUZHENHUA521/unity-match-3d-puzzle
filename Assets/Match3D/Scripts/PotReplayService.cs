using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public sealed class PotReplayService : MonoBehaviour {
    [Serializable] public class Honor {public PotCertificateData certificate;public string replayId;public long earnedAt;}
    [Serializable] public class Summary {public string id,recordId,status,date,tier,levelVersion,resourceVersion,nickName,avatarUrl;public int stage;public float duration;public long createdAt;}
    [Serializable] public class Reply {public bool ok,hasMore;public string error,code,id,content,date,levelVersion;public int total;public bool ready;public UploadTicket upload;public DownloadTicket download;public Summary[] list;public Honor[] honors;public PotReplayData detail;}
    [Serializable] class RequestBody {public string state,reason,result,nickName,avatarUrl,phase,clientVersion,replayVersion;public long httpStatus;public int attempts;public long occurredAt;public string kind,date,tier,levelVersion,resourceVersion=PotReplayData.Resources,id,recordId,content,uploadFormat="gzip-v1";public int skip,index,count,offset;}
    [Serializable] public class UploadTicket {public string url,authorization,token,cosFileId,cloudPath,format;}
    [Serializable] public class DownloadTicket {public string url,format,sha256;}
    public const int MaxJsonBytes=64*1024*1024;
    public static PotReplayService Instance {get;private set;}
    public event Action<string> Uploaded;
    string endpoint;string queueOwner;string uploadError;public string Status {get;private set;}="";
    public static string DirectoryPath=>Path.Combine(Application.persistentDataPath,"pot-replays-v1");
    public static string PathFor(string owner,string id,string suffix){PotReplayFiles.Ensure();return Path.Combine(DirectoryPath,owner+"_"+id+suffix);}
    float memoryPauseUntil;UnityWebRequest activeUpload;
    [Serializable] class NativeRequest {public string receiver,id,path;public UploadTicket ticket;}
    [Serializable] class NativeReply {public string id,error;public bool ok;public long status;}
    string nativeId;NativeReply nativeReply;long uploadStatus;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern void PotReplayUpload(string json);
    [DllImport("__Internal")] static extern void PotReplayUploadCancel();
    void OnWechatMemory(WeChatWASM.OnMemoryWarningListenerResult value){OnLowMemory();}
#endif
    void Awake(){Application.lowMemory+=OnLowMemory;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        WeChatWASM.WX.OnMemoryWarning(OnWechatMemory);
#endif
    }
    void OnDestroy(){Application.lowMemory-=OnLowMemory;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        WeChatWASM.WX.OffMemoryWarning(OnWechatMemory);PotReplayUploadCancel();
#endif
        activeUpload?.Abort();if(Instance==this)Instance=null;
    }
    void OnLowMemory(){memoryPauseUntil=Time.realtimeSinceStartup+60;Status="设备内存紧张，录像稍后同步";activeUpload?.Abort();
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        PotReplayUploadCancel();
#endif
    }
    bool Busy()=>Time.realtimeSinceStartup<memoryPauseUntil||PotReplayRecorder.Recording||PotReplayPlayer.Active||GameManager.Instance&&GameManager.Instance.GameIsPlaying();
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');StartCoroutine(InitializeQueue());StartCoroutine(Diagnostics());}
    public void Queued(){Status="录像已保存，等待空闲时同步";}
    IEnumerator InitializeQueue(){yield return Upload();}
    IEnumerator Recover(string owner){
        PotReplayFiles.Ensure();
        foreach(var temporary in PotReplayFiles.List(owner+"_*.upload").Concat(PotReplayFiles.List(owner+"_*.packing")))try{PotReplayFiles.Delete(temporary);}catch(Exception error){Debug.LogWarning(error.Message);}
        PotReplayQueue.RestoreRejected(owner);
        foreach(var path in PotReplayFiles.List(owner+"_*.recording")){
            while(Busy())yield return new WaitForSecondsRealtime(3);
            if(PotReplayFiles.Exists(Path.ChangeExtension(path,".ready"))||PotReplayFiles.Exists(Path.ChangeExtension(path,".ready.rejected"))||PotReplayFiles.Exists(Path.ChangeExtension(path,".pending")))continue;
            var id=PotReplayQueue.Id(owner,path);var state=PotReplayLocalState.Get(owner,id);
            if(state?.state=="synced"){try{PotReplayFiles.Delete(path);}catch(Exception error){Debug.LogWarning(error.Message);}continue;}
            if(state?.state=="failed")continue;
            if(PotReplayFiles.Size(path)>MaxJsonBytes){
                if(state==null)PotReplayLocalState.Put(owner,new PotReplayLocalState.Entry{recordId=id});
                PotReplayLocalState.Set(owner,id,"failed","旧版录像超过64MiB上限");continue;
            }
            // A truncated recording cannot become a winning replay. Keep the known failure visible.
            Exception failure=null;
            yield return Match3DStartupRunner.Run(PotReplayJournal.Recover(path),e=>failure=e);
            var ready=Path.ChangeExtension(path,".ready");
            if(PotReplayFiles.Exists(ready)){
                if(state==null)PotReplayQueue.Import(owner,ready);
                PotReplayLocalState.Set(owner,id,"queued","恢复了部分录像",result:"incomplete");
            }else{
                if(state==null)PotReplayLocalState.Put(owner,new PotReplayLocalState.Entry{recordId=id});
                PotReplayLocalState.Set(owner,id,"failed",failure?.Message??"录像未完整保存");
            }
        }
        PotReplayQueue.Reconcile(owner);
    }
    [UnityEngine.Scripting.Preserve] public void OnPotReplayUpload(string json){var reply=JsonUtility.FromJson<NativeReply>(json);if(reply!=null&&reply.id==nativeId)nativeReply=reply;}
    IEnumerator UploadFile(UploadTicket ticket,string path,Action<bool> done){
        uploadStatus=0;uploadError=null;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        nativeId=Guid.NewGuid().ToString("N");nativeReply=null;
        PotReplayUpload(JsonUtility.ToJson(new NativeRequest{receiver=gameObject.name,id=nativeId,path=path,ticket=ticket}));
        float deadline=Time.realtimeSinceStartup+185;
        while(nativeReply==null&&Time.realtimeSinceStartup<deadline)yield return null;
        if(nativeReply==null)PotReplayUploadCancel();
        uploadStatus=nativeReply?.status??0;uploadError=nativeReply?.error??"录像上传超时";done(nativeReply!=null&&nativeReply.ok);nativeId=null;nativeReply=null;
#else
        using(var request=new UnityWebRequest(ticket.url,"PUT")){
            request.uploadHandler=new UploadHandlerFile(path);request.downloadHandler=new DownloadHandlerBuffer();request.timeout=180;
            request.SetRequestHeader("Content-Type","application/octet-stream");request.SetRequestHeader("Signature",ticket.authorization);request.SetRequestHeader("Authorization",ticket.authorization);
            request.SetRequestHeader("x-cos-security-token",ticket.token);request.SetRequestHeader("x-cos-meta-fileid",ticket.cosFileId);request.SetRequestHeader("key",Uri.EscapeDataString(ticket.cloudPath));
            activeUpload=request;try{yield return request.SendWebRequest();uploadError=request.error;uploadStatus=request.responseCode;done(request.result==UnityWebRequest.Result.Success);}finally{activeUpload=null;}
        }
#endif
    }
    IEnumerator Upload(){
        while(true){
            yield return new WaitForSecondsRealtime(1);
            var account=Match3DInvitations.Instance;if(!account||!account.Ready||Busy())continue;
            string owner=account.State.inviteCode;
            if(queueOwner!=owner){
                bool recovered=true;yield return Match3DStartupRunner.Run(Recover(owner),e=>{recovered=false;Status="录像本地恢复暂不可用";Debug.LogWarning(e.Message);});
                if(!recovered){yield return new WaitForSecondsRealtime(15);continue;}queueOwner=owner;
            }
            string path=null;try{path=PotReplayQueue.Next(owner);}catch(Exception e){Status="录像本地保存暂不可用";Debug.LogWarning(e.Message);}if(path==null)continue;
            string id=PotReplayQueue.Id(owner,path);
            yield return Match3DStartupRunner.Run(UploadOne(owner,path),e=>{Status="录像同步失败，已保留文件等待重试";PotReplayLocalState.Retry(owner,id,e.Message);Debug.LogWarning("Pot replay upload: "+e.Message);});
        }
    }
    IEnumerator Diagnostics(){
        while(true){yield return new WaitForSecondsRealtime(10);
            var account=Match3DInvitations.Instance;if(!account||!account.Ready||Busy())continue;
            yield return Match3DStartupRunner.Run(ReportDiagnostic(account.State.inviteCode),e=>Debug.LogWarning("Pot replay diagnostic: "+e.Message));
        }
    }
    IEnumerator ReportDiagnostic(string owner){
        var entry=PotReplayLocalState.NextDiagnostic(owner);if(entry==null)yield break;PotReplayLocalState.BeginDiagnostic(owner,entry);
        Reply reply=null;long code=0;long revision=entry.updatedAt;
        yield return Send("diagnostic",new RequestBody{recordId=entry.recordId,state=entry.state,reason=entry.reason,result=entry.result,attempts=entry.attempts,occurredAt=entry.updatedAt,phase=entry.phase??"",httpStatus=entry.httpStatus,clientVersion=entry.clientVersion??PotReplayBuild.Version,replayVersion=entry.replayVersion??PotReplayData.Version},r=>reply=r,c=>code=c,owner);
        PotReplayLocalState.CompleteDiagnostic(owner,entry.recordId,revision,reply?.ok==true,code,reply?.code);
    }
    static bool PermanentPackFailure(Exception error){
        string message=error.Message;
        return message.Contains("超过64MiB")||message.Contains("超过16MiB")||message.Contains("录像索引")||message.Contains("录像帧不完整")||message.Contains("录像初盘缺失");
    }
    void FailedUpload(string owner,string path,string reason,bool terminal){
        var id=PotReplayQueue.Id(owner,path);Status=reason;
        if(terminal){var rejected=PotReplayLocalState.Get(owner,id);if(rejected!=null){rejected.repairVersion=1;PotReplayLocalState.Put(owner,rejected);}PotReplayLocalState.Set(owner,id,"failed",reason);if(!PotReplayFiles.Exists(path+".rejected"))PotReplayFiles.Move(path,path+".rejected");}
        else PotReplayLocalState.Retry(owner,id,reason);
    }
    IEnumerator UploadOne(string owner,string path){
        var recordId=PotReplayQueue.Id(owner,path);
        PotReplayQueue.Import(owner,path);PotReplayLocalState.Set(owner,recordId,"uploading");
        Reply reply=null;long code=0;
        PotReplayLocalState.Phase(owner,recordId,"finalize-check");
        yield return Send("upload-complete",new RequestBody{recordId=recordId},r=>reply=r,c=>code=c,owner);
        PotReplayLocalState.Phase(owner,recordId,"finalize-check",code);
        if(reply?.ok==true){CompleteUpload(owner,path);yield break;}
        if(code!=404&&code!=410){FailedUpload(owner,path,reply?.error??"录像确认稍后重试",code==400||code==413);yield break;}
        PotReplayLocalState.Phase(owner,recordId,"pack");
        Exception failure=null;Status="正在分段压缩录像";
        yield return Match3DStartupRunner.Run(PotReplayCache.Prepare(path,()=>Busy()||Match3DInvitations.Instance.State.inviteCode!=owner),e=>failure=e);
        if(failure!=null){FailedUpload(owner,path,failure.Message,PermanentPackFailure(failure));yield break;}
        if(Busy()){PotReplayLocalState.Retry(owner,recordId,"录像将在空闲时继续同步");yield break;}
        PotReplayLocalState.Phase(owner,recordId,"ticket");
        yield return Send("upload-start",new RequestBody{recordId=recordId},r=>reply=r,c=>code=c,owner);
        PotReplayLocalState.Phase(owner,recordId,"ticket",code);
        if(reply==null||!reply.ok){FailedUpload(owner,path,reply?.error??"录像同步稍后重试",code==400||code==413);yield break;}
        if(reply.ready){CompleteUpload(owner,path);yield break;}
        var ticket=reply.upload;
        if(ticket==null||ticket.format!="gzip-v1"||!Uri.TryCreate(ticket.url,UriKind.Absolute,out var uri)||uri.Scheme!="https")throw new IOException("录像上传地址无效");
        PotReplayLocalState.Phase(owner,recordId,"put");bool uploaded=false;Status="录像正在上传云存储";
        yield return UploadFile(ticket,PotReplayCache.Packed(path),ok=>uploaded=ok);
        PotReplayLocalState.Phase(owner,recordId,"put",uploadStatus);
        if(!uploaded){FailedUpload(owner,path,uploadError??"云存储上传失败，已保留录像等待重试",false);yield break;}
        PotReplayLocalState.Phase(owner,recordId,"finalize");
        yield return Send("upload-complete",new RequestBody{recordId=recordId},r=>reply=r,c=>code=c,owner);
        PotReplayLocalState.Phase(owner,recordId,"finalize",code);
        if(reply?.ok==true)CompleteUpload(owner,path);
        else FailedUpload(owner,path,reply?.error??"录像等待校验重试",code==400||code==413);
    }
    void CompleteUpload(string owner,string path){
        PotReplayLocalState.Set(owner,PotReplayQueue.Id(owner,path),"synced");try{PotReplayCache.DeleteSynced(owner,PotReplayQueue.Id(owner,path));}catch(Exception error){Debug.LogWarning("Pot synced replay cleanup: "+error.Message);}Status="录像已同步";Uploaded?.Invoke(owner);
    }
    public IEnumerator List(bool masters,int skip,Match3DDailyLevel level,Action<Reply> done,string tier="bachelor"){
        if(masters){
            Match3DRewards.Reply daily=null;
            yield return Match3DRewards.Instance.Request("daily","{\"themeId\":\"daily\",\"tier\":\""+tier+"\",\"levelProtocol\":3,\"openingVersion\":\""+Match3DDailySnapshot.ResourceVersion+"\"}",r=>daily=r,true);
            if(daily==null||!daily.ok||daily.level==null){done(new Reply{error=daily?.error??"每日关卡暂不可用"});yield break;}level=daily.level;
        }
        yield return Send("list",new RequestBody{kind=masters?"masters":"mine",skip=skip,date=level?.date,tier=level?.tier,levelVersion=level?.version},done);
    }
    public void SyncPublicProfile(string owner,string name,string avatar){
        StartCoroutine(Send("profile",new RequestBody{nickName=name,avatarUrl=Match3DUserProfileView.AllowedAvatar(avatar)?avatar:""},reply=>{},expectedOwner:owner));
    }
    public IEnumerator Masters(Action<Reply> done){
        var account=Match3DInvitations.Instance;
        string owner=account&&account.Ready?account.State.inviteCode:null;
        if(string.IsNullOrEmpty(owner)||!Match3DRewards.Instance){done(new Reply{error="请先登录"});yield break;}
        Match3DRewards.Reply daily=null;
        yield return Match3DRewards.Instance.Request("daily","{\"themeId\":\"daily\",\"tier\":\"bachelor\",\"levelProtocol\":3,\"openingVersion\":\""+Match3DDailySnapshot.ResourceVersion+"\"}",r=>daily=r,true);
        if(daily==null||!daily.ok||daily.level==null){done(new Reply{error=daily?.error??"每日关卡暂不可用"});yield break;}
        yield return Send("list",new RequestBody{kind="pot-masters",date=daily.level.date,tier="bachelor",levelVersion=daily.level.version},done,expectedOwner:owner);
    }
    public IEnumerator Honors(int skip,Action<Reply> done){yield return Send("honors",new RequestBody{skip=skip},done);}
    public void RecordingFailed(string reason){Status="回放保存失败："+reason;Debug.LogWarning(Status);}
    public IEnumerator Detail(string id,Action<Reply> done){
        var account=Match3DInvitations.Instance;
        if(!account||!account.Ready){done(new Reply{error="请先登录"});yield break;}
        string owner=account.State.inviteCode;
        for(int attempt=0;attempt<2;attempt++){
            if(!account.Ready||account.State.inviteCode!=owner){done(new Reply{error="账号已切换"});yield break;}
            Reply ticket=null;yield return Send("download-start",new RequestBody{id=id},r=>ticket=r,expectedOwner:owner);
            if(ticket==null||!ticket.ok){done(ticket??new Reply{error="录像下载失败"});yield break;}
            var download=ticket.download;
            if(download==null||download.format!="gzip-v1"||!ValidDownloadUrl(download.url)||
                string.IsNullOrEmpty(download.sha256)||!System.Text.RegularExpressions.Regex.IsMatch(download.sha256,"^[a-f0-9]{64}$")){
                done(new Reply{error="录像下载地址无效"});yield break;
            }
            using(var request=UnityWebRequest.Get(download.url)){
                request.timeout=120;request.redirectLimit=0;
                var operation=request.SendWebRequest();bool tooLarge=false;
                while(!operation.isDone){
                    if(!account.Ready||account.State.inviteCode!=owner){request.Abort();done(new Reply{error="账号已切换"});yield break;}
                    if(request.downloadedBytes>PotReplayTransfer.MaxFileBytes){tooLarge=true;request.Abort();break;}
                    yield return null;
                }
                if(!account.Ready||account.State.inviteCode!=owner){done(new Reply{error="账号已切换"});yield break;}
                if(tooLarge){done(new Reply{error="录像文件过大"});yield break;}
                if(request.result!=UnityWebRequest.Result.Success){
                    // Refresh expired storage authorization once; never forward the account token to storage.
                    if(attempt==0&&(request.responseCode==0||request.responseCode==401||request.responseCode==403||request.responseCode==408||request.responseCode==429||request.responseCode>=500))continue;
                    done(new Reply{error="录像下载失败，请重试"});yield break;
                }
                Reply result;
                try{
                    var bytes=request.downloadHandler.data;
                    if(bytes==null||bytes.Length<1||bytes.Length>PotReplayTransfer.MaxFileBytes)throw new IOException("录像大小无效");
                    using(var sha=System.Security.Cryptography.SHA256.Create()){
                        string actual=BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();
                        if(actual!=download.sha256)throw new IOException("录像下载不完整，请重试");
                    }
                    var detail=JsonUtility.FromJson<PotReplayData>(PotReplayTransfer.Unpack(bytes));
                    if(detail==null||detail.frames==null||detail.frames.Count==0)throw new IOException("录像内容无效");
                    result=new Reply{ok=true,detail=detail};
                }catch(Exception e){result=new Reply{error=e.Message};}
                done(result);yield break;
            }
        }
    }
    static bool ValidDownloadUrl(string url){
        if(!Uri.TryCreate(url,UriKind.Absolute,out var uri))return false;
#if UNITY_EDITOR
        // Local HTTP fixtures exercise the real downloader; production always requires HTTPS.
        if(uri.Scheme=="http"&&uri.IsLoopback)return true;
#endif
        return uri.Scheme=="https";
    }
    IEnumerator Send(string action,RequestBody body,Action<Reply> done,Action<long> status=null,string expectedOwner=null){
        var account=Match3DInvitations.Instance;
        if(!account||!account.Ready||expectedOwner!=null&&account.State.inviteCode!=expectedOwner){done(new Reply{error="请先登录"});yield break;}
        string owner=account.State.inviteCode,token=account.RecordToken;
        using(var request=new UnityWebRequest(endpoint+"/pot-replays/"+action,"POST")){
            request.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(body)));request.downloadHandler=new DownloadHandlerBuffer();request.timeout=45;
            request.SetRequestHeader("Content-Type","application/json");request.SetRequestHeader("Authorization","Bearer "+token);
            if(action=="diagnostic")request.SetRequestHeader("X-Pot-Business-Result","1");
            yield return request.SendWebRequest();status?.Invoke(request.responseCode);
            if(!account.Ready||account.State.inviteCode!=owner){done(new Reply{error="账号已切换"});yield break;}
            Reply reply=null;try{reply=JsonUtility.FromJson<Reply>(request.downloadHandler.text);}catch{}
            if(request.responseCode==401)account.RefreshRecordLogin();done(reply??new Reply{error="网络异常，请重试"});
        }
    }
}
