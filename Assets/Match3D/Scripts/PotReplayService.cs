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
    [Serializable] public class Summary {public string id,recordId,status,date,tier,levelVersion,resourceVersion;public int stage;public float duration;public long createdAt;}
    [Serializable] public class Reply {public bool ok,hasMore;public string error,id,content;public int total;public bool ready;public UploadTicket upload;public DownloadTicket download;public Summary[] list;public PotReplayData detail;}
    [Serializable] class RequestBody {public string kind,date,tier,levelVersion,resourceVersion=PotReplayData.Resources,id,recordId,content,uploadFormat="gzip-v1";public int skip,index,count,offset;}
    [Serializable] public class UploadTicket {public string url,authorization,token,cosFileId,cloudPath,format;}
    [Serializable] public class DownloadTicket {public string url,format,sha256;}
    public const int MaxJsonBytes=64*1024*1024;
    public static PotReplayService Instance {get;private set;}
    public event Action<string> Uploaded;
    string endpoint;int nextUpload;public string Status {get;private set;}="";
    public static string DirectoryPath=>Path.Combine(Application.persistentDataPath,"pot-replays-v1");
    public static string PathFor(string owner,string id,string suffix){PotReplayFiles.Ensure();return Path.Combine(DirectoryPath,owner+"_"+id+suffix);}
    float memoryPauseUntil;UnityWebRequest activeUpload;
    [Serializable] class NativeRequest {public string receiver,id,path;public UploadTicket ticket;}
    [Serializable] class NativeReply {public string id,error;public bool ok;}
    string nativeId;NativeReply nativeReply;
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
    public void Initialize(string url){Instance=this;endpoint=url.TrimEnd('/');StartCoroutine(InitializeQueue());}
    public void Queued(){Status="录像已保存，等待空闲时同步";}
    IEnumerator InitializeQueue(){
        yield return Match3DStartupRunner.Run(Recover(),e=>{Status="录像本地恢复暂不可用";Debug.LogWarning(e.Message);});
        yield return Upload();
    }
    IEnumerator Recover(){
        PotReplayFiles.Ensure();
        foreach(var path in PotReplayFiles.List("*.recording")){
            while(Busy())yield return new WaitForSecondsRealtime(3);
            if(PotReplayFiles.Exists(Path.ChangeExtension(path,".ready"))||PotReplayFiles.Exists(Path.ChangeExtension(path,".ready.rejected"))||PotReplayFiles.Exists(Path.ChangeExtension(path,".pending")))continue;
            yield return Match3DStartupRunner.Run(PotReplayJournal.Recover(path),e=>Debug.LogWarning("Pot replay recovery: "+e.Message));
        }
    }
    [UnityEngine.Scripting.Preserve] public void OnPotReplayUpload(string json){var reply=JsonUtility.FromJson<NativeReply>(json);if(reply!=null&&reply.id==nativeId)nativeReply=reply;}
    IEnumerator UploadFile(UploadTicket ticket,string path,Action<bool> done){
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        nativeId=Guid.NewGuid().ToString("N");nativeReply=null;
        PotReplayUpload(JsonUtility.ToJson(new NativeRequest{receiver=gameObject.name,id=nativeId,path=path,ticket=ticket}));
        float deadline=Time.realtimeSinceStartup+185;
        while(nativeReply==null&&Time.realtimeSinceStartup<deadline)yield return null;
        if(nativeReply==null)PotReplayUploadCancel();
        done(nativeReply!=null&&nativeReply.ok);nativeId=null;nativeReply=null;
#else
        using(var request=new UnityWebRequest(ticket.url,"PUT")){
            request.uploadHandler=new UploadHandlerFile(path);request.downloadHandler=new DownloadHandlerBuffer();request.timeout=180;
            request.SetRequestHeader("Content-Type","application/octet-stream");request.SetRequestHeader("Signature",ticket.authorization);request.SetRequestHeader("Authorization",ticket.authorization);
            request.SetRequestHeader("x-cos-security-token",ticket.token);request.SetRequestHeader("x-cos-meta-fileid",ticket.cosFileId);request.SetRequestHeader("key",Uri.EscapeDataString(ticket.cloudPath));
            activeUpload=request;try{yield return request.SendWebRequest();done(request.result==UnityWebRequest.Result.Success);}finally{activeUpload=null;}
        }
#endif
    }
    IEnumerator Upload(){
        while(true){
            yield return new WaitForSecondsRealtime(3);
            var account=Match3DInvitations.Instance;if(!account||!account.Ready||Busy())continue;
            string owner=account.State.inviteCode;
            string[] files;try{files=PotReplayFiles.List(owner+"_*.ready").Concat(PotReplayFiles.List(owner+"_*.pending")).ToArray();}catch{Status="录像本地保存暂不可用";continue;}if(files.Length==0)continue;
            string path=files[nextUpload++%files.Length];
            yield return Match3DStartupRunner.Run(UploadOne(owner,path),e=>{Status="录像同步失败，已保留文件等待重试";Debug.LogWarning("Pot replay upload: "+e.Message);});
            yield return new WaitForSecondsRealtime(15);
        }
    }
    IEnumerator UploadOne(string owner,string path){
        var recordId=Path.GetFileNameWithoutExtension(path).Substring(owner.Length+1);
            Reply reply=null;long code=0;
            yield return Send("upload-complete",new RequestBody{recordId=recordId},r=>reply=r,c=>code=c,owner);
            if(reply!=null&&reply.ok){CompleteUpload(owner,path);yield break;}
            // A previous PUT may already be complete. Retry finalization first, without re-uploading.
            if(code!=404&&code!=410){Status=reply?.error??"录像同步稍后重试";if(code==400||code==413){PotReplayFiles.Move(path,path+".rejected");RecordingFailed(Status);}yield return new WaitForSecondsRealtime(30);yield break;}
            yield return Send("upload-start",new RequestBody{recordId=recordId},r=>reply=r,c=>code=c,owner);
            if(reply==null||!reply.ok){Status=reply?.error??"录像同步稍后重试";if(code==400||code==413)PotReplayFiles.Move(path,path+".rejected");yield return new WaitForSecondsRealtime(30);yield break;}
            if(reply.ready){CompleteUpload(owner,path);yield break;}
            var ticket=reply.upload;
            if(ticket==null||ticket.format!="gzip-v1"||!Uri.TryCreate(ticket.url,UriKind.Absolute,out var uri)||uri.Scheme!="https")throw new IOException("录像上传地址无效");
            string packed=Path.ChangeExtension(path,".upload");bool uploaded=false;
            try{
                Exception failure=null;Status="正在分段压缩录像";
                yield return Match3DStartupRunner.Run(PotReplayTransfer.PackFile(path,packed,Busy),e=>failure=e);
                if(failure!=null){Status=failure.Message;yield return new WaitForSecondsRealtime(30);yield break;}
                if(Busy())yield break;
                Status="录像正在上传云存储";
                yield return UploadFile(ticket,packed,ok=>uploaded=ok);
                if(!uploaded)Status="云存储上传失败，已保留录像等待重试";
            }finally{if(PotReplayFiles.Exists(packed))PotReplayFiles.Delete(packed);}
            if(uploaded){yield return Send("upload-complete",new RequestBody{recordId=recordId},r=>reply=r,c=>code=c,owner);
                if(reply!=null&&reply.ok){CompleteUpload(owner,path);}
                else {Status=reply?.error??"录像等待校验重试";if(code==400||code==413){PotReplayFiles.Move(path,path+".rejected");RecordingFailed(Status);}}
            }
            yield return new WaitForSecondsRealtime(30);
    }
    void CompleteUpload(string owner,string path){
        PotReplayJournal.DeleteCompleted(path);Status="录像已同步";Uploaded?.Invoke(owner);
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
            yield return request.SendWebRequest();status?.Invoke(request.responseCode);
            if(!account.Ready||account.State.inviteCode!=owner){done(new Reply{error="账号已切换"});yield break;}
            Reply reply=null;try{reply=JsonUtility.FromJson<Reply>(request.downloadHandler.text);}catch{}
            if(request.responseCode==401)account.RefreshRecordLogin();done(reply??new Reply{error="网络异常，请重试"});
        }
    }
}
