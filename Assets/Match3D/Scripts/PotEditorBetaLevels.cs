#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

// Editor-only preview key lives in ignored UserSettings, never in a player build.
public sealed class PotEditorBetaLevels : MonoBehaviour {
    public const string CredentialsPath="UserSettings/PotEditorPreview.json";
    [Serializable] public sealed class Credentials {public string key;}
    [Serializable] sealed class RequestBody {public string tier;public int levelProtocol=3;public string openingVersion=Match3DDailySnapshot.ResourceVersion;}
    bool loading;
    public void Begin(string endpoint,int theme,string tier){if(!loading)StartCoroutine(Load(endpoint,theme,tier));}
    IEnumerator Load(string endpoint,int theme,string tier){
        var flow=Match3DFlow.Instance;int screen=flow.ScreenVersion;loading=true;
        try{
            string key=null;
            try{key=JsonUtility.FromJson<Credentials>(File.ReadAllText(CredentialsPath))?.key;}catch{}
            if(string.IsNullOrEmpty(key)){flow.Notice("缺少编辑器内测预览凭据，请配置 "+CredentialsPath);yield break;}
            flow.Notice("正在读取最新内测关卡…");
            using(var request=new UnityWebRequest(endpoint.TrimEnd('/')+"/pot-editor/beta-level","POST")){
                request.uploadHandler=new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(new RequestBody{tier=tier})));
                request.downloadHandler=new DownloadHandlerBuffer();request.timeout=15;
                request.SetRequestHeader("Content-Type","application/json");request.SetRequestHeader("X-Pot-Editor-Key",key);
                yield return request.SendWebRequest();
                if(!flow||flow.ScreenVersion!=screen)yield break;
                Match3DRewards.Reply reply=null;try{reply=JsonUtility.FromJson<Match3DRewards.Reply>(request.downloadHandler.text);}catch{}
                if(request.result!=UnityWebRequest.Result.Success||reply==null||!reply.ok){flow.Notice(reply!=null&&!string.IsNullOrEmpty(reply.error)?reply.error:"内测关卡加载失败，请检查网络后重试");yield break;}
                var level=reply.level;
                if(level==null||level.sourceDate!="beta"||level.tier!=tier||!Match3DModelThemes.TryGet(level.modelThemeId,flow.SourceEnvironmentForTheme(theme),out var profile)||!level.IsValid(profile.prefabs.Length)){
                    flow.Notice("内测关卡配置无效，请检查后台配置或更新编辑器");yield break;
                }
                flow.BeginPotEditorBeta(theme,level);
                Debug.Log("[Pot] 内测关卡 "+level.version+" / "+level.modelThemeId+" / "+level.initialCount+" / "+level.seconds+"s");
            }
        }finally{loading=false;}
    }
}
public partial class Match3DFlow {
    public void BeginPotEditorBeta(int theme,Match3DDailyLevel level){
        LocalPreview=true;BeginDailyPractice(theme,level);unlimitedMagnetRun=true;
        ShowTips("编辑器内测关卡 · 道具免费 · 不发放红包");
    }
}
#endif
