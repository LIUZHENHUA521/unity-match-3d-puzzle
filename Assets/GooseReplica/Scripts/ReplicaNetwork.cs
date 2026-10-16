using System;
using System.Collections;
using System.Collections.Concurrent;
#if !UNITY_WEBGL || UNITY_EDITOR
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endif
using UnityEngine;
using UnityEngine.Networking;

[Serializable] public class ReplicaPlayer { public string id;public bool ready; }
[Serializable] public class ReplicaMessage {
    public string type,token,playerId,roomId,phase,message,action,winner,reason;
    public int tileId,seed,seconds,cleared,undoLeft;
    public long deadline;
    public int[] deck,slots;
    public ReplicaPlayer[] players;
}
#if !UNITY_WEBGL || UNITY_EDITOR
public class ReplicaNetwork : MonoBehaviour {
    public Action<ReplicaMessage> Received;
    public string PlayerId { get;private set; }
    public bool Connected { get {return socket!=null&&socket.State==WebSocketState.Open;} }
    ClientWebSocket socket;CancellationTokenSource lifetime;readonly SemaphoreSlim gate=new SemaphoreSlim(1,1);
    readonly ConcurrentQueue<ReplicaMessage> incoming=new ConcurrentQueue<ReplicaMessage>();int generation;
    public void Connect(string endpoint,string intent,string room="") {Disconnect();StartCoroutine(Login(endpoint,intent,room,generation));}
    IEnumerator Login(string endpoint,string intent,string room,int version) {
        using(var req=new UnityWebRequest(endpoint.TrimEnd('/')+"/session","POST")) {
            req.downloadHandler=new DownloadHandlerBuffer();req.timeout=10;yield return req.SendWebRequest();
            if(version!=generation)yield break;
            if(req.result!=UnityWebRequest.Result.Success){Received?.Invoke(new ReplicaMessage {type="error",message="无法连接对战服务："+req.error});yield break;}
            var auth=JsonUtility.FromJson<ReplicaMessage>(req.downloadHandler.text);PlayerId=auth.playerId;
            var task=Open(endpoint,auth.token,intent,room,version);
            while(!task.IsCompleted)yield return null;
        }
    }
    async Task Open(string endpoint,string token,string intent,string room,int version) {
        var ws=new ClientWebSocket();var cancel=new CancellationTokenSource();socket=ws;lifetime=cancel;
        try {
            await ws.ConnectAsync(new Uri(endpoint.Replace("https://","wss://").Replace("http://","ws://").TrimEnd('/')+"/ws?token="+Uri.EscapeDataString(token)),cancel.Token);
            if(version!=generation)return;
            Send(new ReplicaMessage {type=intent,roomId=room});
            var buffer=new byte[8192];var text=new StringBuilder();
            while(ws.State==WebSocketState.Open&&!cancel.IsCancellationRequested) {
                var r=await ws.ReceiveAsync(new ArraySegment<byte>(buffer),cancel.Token);
                if(r.MessageType==WebSocketMessageType.Close)break;
                text.Append(Encoding.UTF8.GetString(buffer,0,r.Count));if(text.Length>65536)throw new Exception("消息过大");
                if(r.EndOfMessage){var m=JsonUtility.FromJson<ReplicaMessage>(text.ToString());if(version==generation)incoming.Enqueue(m);text.Clear();}
            }
            if(version==generation)incoming.Enqueue(new ReplicaMessage {type="disconnected",message="对战连接已断开"});
        }catch(Exception e){if(version==generation&&!cancel.IsCancellationRequested)incoming.Enqueue(new ReplicaMessage {type="disconnected",message="对战连接失败："+e.Message});}
        finally {ws.Dispose();}
    }
    public async void Send(ReplicaMessage msg){
        var ws=socket;var cancel=lifetime;if(ws==null||ws.State!=WebSocketState.Open||cancel==null)return;
        bool held=false;
        try {await gate.WaitAsync(cancel.Token);held=true;var bytes=Encoding.UTF8.GetBytes(JsonUtility.ToJson(msg));await ws.SendAsync(new ArraySegment<byte>(bytes),WebSocketMessageType.Text,true,cancel.Token);}
        catch(Exception){if(!cancel.IsCancellationRequested)incoming.Enqueue(new ReplicaMessage {type="disconnected",message="发送失败，请重新匹配"});}
        finally {if(held)gate.Release();}
    }
    void Update(){while(incoming.TryDequeue(out var msg))Received?.Invoke(msg);}
    public void Disconnect(){generation++;StopAllCoroutines();lifetime?.Cancel();socket?.Abort();socket=null;while(incoming.TryDequeue(out _)){} }
    void OnDestroy(){Disconnect();}
}

#endif
