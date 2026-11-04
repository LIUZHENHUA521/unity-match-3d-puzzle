#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

// The native editor transport is in Match3DNetwork.cs. Web builds use wx's
// request/SocketTask APIs, with browser APIs available for WebGL smoke tests.
public class Match3DNetwork : MonoBehaviour {
    public Action<Match3DMessage> Received;
    public string PlayerId { get; private set; }
    public bool Connected { get; private set; }
    int generation;
    [Serializable] class TransportEvent { public int generation; public string kind, data; }
    [DllImport("__Internal")] static extern void GooseConnect(string target, string endpoint, string intent, string room, int generation);
    [DllImport("__Internal")] static extern void GooseSend(string target, string message);
    [DllImport("__Internal")] static extern void GooseDisconnect(string target);

    public void Connect(string endpoint, string intent, string room="") {
        Disconnect();
        GooseConnect(gameObject.name, endpoint.TrimEnd('/'), intent, room, generation);
    }
    public void Send(Match3DMessage message) {
        if(Connected) GooseSend(gameObject.name, JsonUtility.ToJson(message));
    }
    [Preserve] public void OnGooseTransportMessage(string json) {
        var e=JsonUtility.FromJson<TransportEvent>(json);
        if(e.generation!=generation)return;
        if(e.kind=="auth") { PlayerId=e.data; return; }
        if(e.kind=="open") { Connected=true; return; }
        if(e.kind=="message") {
            try { Received?.Invoke(JsonUtility.FromJson<Match3DMessage>(e.data)); }
            catch(Exception) { Connected=false; Received?.Invoke(new Match3DMessage{type="error",message="对战消息格式错误"}); }
            return;
        }
        Connected=false;
        Received?.Invoke(new Match3DMessage{type=e.kind=="error"?"error":"disconnected",message=e.data});
    }
    public void Disconnect() {
        generation++; Connected=false; PlayerId=null;
        GooseDisconnect(gameObject.name);
    }
    void OnDestroy() { Disconnect(); }
}
#endif
