using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Author candidates with the same models, colliders and gameplay as the shipped client.
public sealed class DailyOpeningWorkshop : EditorWindow {
    const string Pending="goose.dailyWorkshop.pending",Validation="goose.dailyWorkshop.validate";
    static Match3DDailyLevel candidate;
    static string previewImage,status="打开后台下载的候选试玩文件。";
    static bool busy;
    [Serializable] sealed class Export {public string date,tier,candidateId,previewImage;public int seed;public Match3DDailySnapshot opening;}
    [MenuItem("工具/每日同题/候选试玩")]
    public static void Open()=>GetWindow<DailyOpeningWorkshop>("每日同题试玩");
    [InitializeOnLoadMethod] static void Initialize(){EditorApplication.playModeStateChanged-=Changed;EditorApplication.playModeStateChanged+=Changed;}
    void OnGUI(){
        EditorGUILayout.HelpBox(status,MessageType.Info);
        GUI.enabled=!busy;
        if(GUILayout.Button("打开候选试玩文件")){
            var path=EditorUtility.OpenFilePanel("选择后台下载的候选","","json");
            if(!string.IsNullOrEmpty(path))Load(path);
        }
        GUI.enabled=!busy&&candidate!=null&&EditorApplication.isPlaying;
        if(GUILayout.Button("生成两关固定开局"))Start(false);
        GUI.enabled=!busy&&candidate?.opening!=null&&EditorApplication.isPlaying;
        if(GUILayout.Button("试玩第一关"))Match3DFlow.Instance.PreviewDailyOpening(candidate,1);
        if(GUILayout.Button("试玩第二关"))Match3DFlow.Instance.PreviewDailyOpening(candidate,2);
        if(GUILayout.Button("导出布局和预览图")){
            var path=EditorUtility.SaveFilePanel("回传后台", "",candidate.date+"-"+candidate.tier+"-opening.json","json");
            if(!string.IsNullOrEmpty(path))WriteExport(path);
        }
        GUI.enabled=true;
        if(candidate!=null)EditorGUILayout.LabelField(candidate.date+" / "+candidate.tier+" / "+candidate.initialCount+" 个");
    }
    static void Load(string path){
        try{
            var level=JsonUtility.FromJson<Match3DDailyLevel>(File.ReadAllText(path));
            if(level==null||string.IsNullOrEmpty(level.candidateId)||level.seed<=0)throw new Exception("不是候选关卡文件");
            var copy=JsonUtility.FromJson<Match3DDailyLevel>(JsonUtility.ToJson(level));copy.generatorVersion="daily-v1";
            if(!copy.IsValid(23))throw new Exception("候选参数无效");
            if(level.opening!=null&&!level.opening.IsValid(level.initialCount,23))throw new Exception("布局版本不兼容，请重新生成");
            if(!EditorApplication.isPlaying){
                if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
                SessionState.SetString(Pending,path);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;
            }else {candidate=level;previewImage=null;status="候选已打开，生成固定开局后可试玩。";}
        }catch(Exception e){status=e.Message;Debug.LogException(e);}
    }
    static void Changed(PlayModeStateChange state){
        if(state==PlayModeStateChange.EnteredPlayMode){
            string path=SessionState.GetString(Pending,"");
            if(path!=""){SessionState.EraseString(Pending);candidate=JsonUtility.FromJson<Match3DDailyLevel>(File.ReadAllText(path));previewImage=null;status="候选已打开，生成固定开局后可试玩。";}
            if(SessionState.GetBool(Validation,false))EditorApplication.delayCall+=()=>Start(true);
        }
        if(state==PlayModeStateChange.ExitingPlayMode){busy=false;candidate=null;previewImage=null;}
    }
    static void Start(bool validate){
        busy=true;status="正在使用真实游戏生成两关开局…";
        new GameObject("Daily opening authoring").AddComponent<BoardValidationDriver>().StartCoroutine(Run(validate));
    }
    static IEnumerator Run(bool validate){
        yield return null;yield return null;
        string failure=null;var steps=Generate(validate);
        while(true){bool next=false;try{next=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(failure!=null||!next)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();busy=false;
        status=failure??"两关固定开局已生成。可以试玩；导出文件回传后台后确认试玩并排期。";
        if(failure!=null)Debug.LogError(failure);
        if(validate){
            Directory.CreateDirectory("Builds/DailyOpening");File.WriteAllText("Builds/DailyOpening/validation.txt",failure??"PASS: real-model capture; two-stage exact pose/visibility restore; idle stability; retry; first-pick release; no preview telemetry; resource validation; date/tier sharing.");
            SessionState.SetBool(Validation,false);EditorApplication.Exit(failure==null?0:1);
        }
    }
    static IEnumerator Generate(bool validate){
        var flow=Match3DFlow.Instance;
        if(validate)candidate=new Match3DDailyLevel{date="2026-11-12",tier="bachelor",candidateId="candidate-0123456789abcdef01234567",version="validation",generatorVersion="shared-v2",modelThemeId="foodie-party",seed=47231,seconds=600,initialCount=300,limitHeight=21.5f,layers=new[]{new ChallengeLayer(150,23,14),new ChallengeLayer(60,10,18),new ChallengeLayer(45,8,20),new ChallengeLayer(45,7,22)}};
        var source=JsonUtility.FromJson<Match3DDailyLevel>(JsonUtility.ToJson(candidate));source.generatorVersion="daily-v1";source.opening=null;
        var opening=new Match3DDailySnapshot{stages=new Match3DDailyStage[2]};
        for(int stage=1;stage<=2;stage++){
            flow.PreviewDailyOpening(source,stage);
            float deadline=Time.realtimeSinceStartup+90;
            while(flow.CreationInProgress&&Time.realtimeSinceStartup<deadline)yield return null;
            if(flow.CreationInProgress)throw new Exception("关卡生成超时");
            yield return new WaitForSeconds(5);
            opening.stages[stage-1]=flow.CaptureDailyOpening();
        }
        if(!opening.IsValid(candidate.initialCount,23))throw new Exception("生成的布局不完整，请更换候选");
        candidate.opening=opening;candidate.generatorVersion="shared-v2";
        for(int stage=1;stage<=2;stage++){
            flow.PreviewDailyOpening(candidate,stage);
            while(flow.CreationInProgress)yield return null;
            yield return null;yield return null;
            Verify(opening.stages[stage-1]);
            yield return new WaitForSeconds(.2f);Verify(opening.stages[stage-1]);
            if(validate){flow.RestartStage();while(flow.CreationInProgress)yield return null;yield return null;Verify(opening.stages[stage-1]);}
        }
        previewImage=CaptureImage();
        if(validate){
            WriteExport("Builds/DailyOpening/opening.json");
            if(!flow.LocalPreview)throw new Exception("试玩产生了正式记录");
            if(!flow.DailyShareTitle(true).Contains("2026-11-12 本科局"))throw new Exception("分享缺少日期档位");
            var picked=LevelGenerator.Instance.objectsOnScene.First(o=>LayeredItemPile.Instance.CanPick(o.GetComponent<Rigidbody>()));
            InputController.Instance.AddObjectOnStack(picked);
            if(ItemPhysicsController.Instance.DailyOpeningHeld)throw new Exception("首次消除未恢复物理");
        }
    }
    static void Verify(Match3DDailyStage expected){
        var objects=LevelGenerator.Instance.objectsOnScene;
        if(objects.Count!=expected.items.Length)throw new Exception("还原数量不一致");
        foreach(var item in expected.items){
            var obj=objects.Single(o=>o.GetComponent<Match3DTile>().id==item.id);
            if(obj.name!="type_"+item.type||Vector3.Distance(obj.transform.position,item.position)>.00001f||Quaternion.Angle(obj.transform.rotation,item.rotation)>.05f||Vector3.Distance(obj.transform.localScale,item.scale)>.00001f||LayeredItemPile.Instance.DailyVisible(obj.GetComponent<Rigidbody>())!=item.visible)throw new Exception("开局还原不一致："+item.id);
        }
    }
    static void WriteExport(string path){
        if(string.IsNullOrEmpty(previewImage))throw new Exception("请先生成两关开局与预览图");
        var parent=Path.GetDirectoryName(path);if(!string.IsNullOrEmpty(parent))Directory.CreateDirectory(parent);
        File.WriteAllText(path,JsonUtility.ToJson(new Export{date=candidate.date,tier=candidate.tier,candidateId=candidate.candidateId,seed=candidate.seed,opening=candidate.opening,previewImage=previewImage}));
        status="已导出，请在后台回传并确认试玩。";
    }
    static string CaptureImage(){
        var camera=Camera.main;var rt=new RenderTexture(360,640,24);var old=camera.targetTexture;var active=RenderTexture.active;var tex=new Texture2D(360,640,TextureFormat.RGB24,false);
        try{camera.targetTexture=rt;rt.Create();Match3DFlow.Instance.GetComponent<Match3DThemePresentation>().Fit();Match3DTray.Instance.SendMessage("Layout");camera.Render();RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,360,640),0,0);tex.Apply();return "data:image/jpeg;base64,"+Convert.ToBase64String(tex.EncodeToJPG(65));}
        finally{camera.targetTexture=old;Match3DFlow.Instance.GetComponent<Match3DThemePresentation>().Fit();Match3DTray.Instance.SendMessage("Layout");RenderTexture.active=active;DestroyImmediate(tex);rt.Release();DestroyImmediate(rt);}
    }
    public static void RunValidation(){EditorApplication.delayCall+=()=>{SessionState.SetBool(Validation,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;};}
}
