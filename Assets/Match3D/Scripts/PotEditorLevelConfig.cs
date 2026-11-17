#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

// Editor-only configuration reader. Never creates player identities or reward runs.
public sealed class PotEditorLevelConfig : MonoBehaviour {
    [Serializable] public sealed class Layer { public int count,types;public float height; }
    [Serializable] public sealed class Config { public Layer[] layers; }
    [Serializable] sealed class Reply { public bool ok;public string error;public Config config;public int revision; }
    bool loading;
    public void Begin(string endpoint,int theme,string tier){if(!loading)StartCoroutine(Load(endpoint,theme,tier));}
    IEnumerator Load(string endpoint,int theme,string tier){
        var flow=Match3DFlow.Instance;int screen=flow.ScreenVersion;loading=true;
        string selected=UnityEditor.EditorPrefs.GetString(Match3DFlow.EditorThemePreference,"");
        try{
            string key=null;
            try{key=JsonUtility.FromJson<PotEditorBetaLevels.Credentials>(File.ReadAllText(PotEditorBetaLevels.CredentialsPath))?.key;}catch{}
            if(string.IsNullOrEmpty(key)){flow.Notice("缺少编辑器预览凭据，请配置 "+PotEditorBetaLevels.CredentialsPath);yield break;}
            flow.Notice("正在读取编辑器关卡配置…");
            using(var request=new UnityWebRequest(endpoint.TrimEnd('/')+"/pot-editor/config","POST")){
                request.uploadHandler=new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes("{\"configProtocol\":1}"));
                request.downloadHandler=new DownloadHandlerBuffer();request.timeout=15;
                request.SetRequestHeader("Content-Type","application/json");request.SetRequestHeader("X-Pot-Editor-Key",key);
                yield return request.SendWebRequest();
                if(!flow||flow.ScreenVersion!=screen)yield break;
                Reply reply=null;try{reply=JsonUtility.FromJson<Reply>(request.downloadHandler.text);}catch{}
                if(request.result!=UnityWebRequest.Result.Success||reply==null||!reply.ok){flow.Notice(!string.IsNullOrEmpty(reply?.error)?reply.error:"编辑器配置读取失败，请检查网络后重试");yield break;}
                LevelLayoutConfig.Profile profile=null;string error=null;
                try{profile=BuildProfile(Match3DModelThemes.Get(selected,flow.SourceEnvironmentForTheme(theme)),reply.config);}
                catch(Exception e){error=e.Message;}
                if(error!=null){flow.Notice("编辑器配置无效："+error);yield break;}
                flow.BeginPotEditorConfig(theme,tier,selected,profile);
                Debug.Log("[Pot] 编辑器配置版本 "+reply.revision+" / "+selected);
            }
        }finally{loading=false;}
    }
    public static LevelLayoutConfig.Profile BuildProfile(LevelLayoutConfig.Profile source,Config config){
        if(config?.layers==null||config.layers.Length==0||config.layers.Length>100)throw new ArgumentException("请配置 1 至 100 层");
        var originals=source.layers??Array.Empty<ChallengeLayer>();
        float step=originals.Length>1?originals[originals.Length-1].height-originals[originals.Length-2].height:2;
        if(step<=0||float.IsNaN(step)||float.IsInfinity(step))step=2;
        var layers=new ChallengeLayer[config.layers.Length];
        for(int i=0;i<layers.Length;i++){
            var input=config.layers[i];
            if(input==null||input.count<3||input.count>30000||input.count%3!=0||input.types<1||input.types>input.count/3||input.types>source.prefabs.Length)throw new ArgumentException("第 "+(i+1)+" 层数量或类型数无效");
            var original=i<originals.Length?originals[i]:null;
            float height=original!=null?original.height:i==0?14:layers[i-1].height+step;
            if(input.height!=0){
                if(float.IsNaN(input.height)||float.IsInfinity(input.height)||input.height<=1.5f||input.height>30)throw new ArgumentException("第 "+(i+1)+" 层高度无效");
                height=input.height;
            }
            layers[i]=new ChallengeLayer(input.count,input.types,height){weights=original?.weights==null?Array.Empty<float>():(float[])original.weights.Clone()};
        }
        return new LevelLayoutConfig.Profile{environment=source.environment,sourceIndex=source.sourceIndex,seconds=source.seconds,limitHeight=source.limitHeight,prefabs=(GameObject[])source.prefabs.Clone(),tutorialNames=source.tutorialNames,ordinaryTypes=source.ordinaryTypes,layers=layers};
    }
}
public partial class Match3DFlow {
    public const string PotEditorSourcePreference="pot.editor.configurationSource";
    // Existing explicit selections retain their meaning; new editors use the independent config.
    public static int PotEditorSource => UnityEditor.EditorPrefs.HasKey(PotEditorSourcePreference)?UnityEditor.EditorPrefs.GetInt(PotEditorSourcePreference,2):UnityEditor.EditorPrefs.HasKey(PotEditorConfigurationPreference)?(PotUseEditorConfiguration?1:0):2;
    public void BeginPotEditorConfig(int theme,string tier,string modelTheme,LevelLayoutConfig.Profile profile){
        BeginLocal(theme,tier,true,profile,modelTheme);
        ShowTips("编辑器玩家 · 道具免费 · 不发放红包");
    }
}
#endif
