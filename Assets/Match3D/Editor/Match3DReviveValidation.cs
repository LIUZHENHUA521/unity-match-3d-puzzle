using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class Match3DReviveValidation {
    const string Output="work/revive-ui";
    static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
    [InitializeOnLoadMethod]static void Init(){EditorApplication.playModeStateChanged+=Changed;}
    public static void Run(){Match3DReviveBuilder.Build();Directory.CreateDirectory(Output);File.WriteAllText(Output+"/validation.txt","");SessionState.SetBool("revive-validation",true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Changed(PlayModeStateChange state){if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("revive-validation",false))EditorApplication.delayCall+=()=>new GameObject("Revive tests").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());}
    static void Assert(bool condition,string message){if(!condition)throw new Exception(message);File.AppendAllText(Output+"/validation.txt","PASS "+message+"\n");}
    static object Field(object target,string name)=>target.GetType().GetField(name,Flags).GetValue(target);
    static IEnumerator Guard(){var steps=Check();while(true){object current;try{if(!steps.MoveNext())break;current=steps.Current;}catch(Exception e){File.AppendAllText(Output+"/validation.txt","FAIL "+e);Debug.LogException(e);(steps as IDisposable)?.Dispose();SessionState.SetBool("revive-validation",false);EditorApplication.Exit(1);yield break;}yield return current;}SessionState.SetBool("revive-validation",false);EditorApplication.Exit(0);}
    static IEnumerator Check(){
        Match3DRewardAcquisition.Configure(new Match3DRewardAcquisition.Configuration{sharesEnabled=false});
        yield return null;var flow=Match3DFlow.Instance;var records=Match3DGameRecords.Instance;
        records.StopAllCoroutines();Match3DInvitations.Instance.StopAllCoroutines();
        string[] keys={"goose.result.date","goose.result.attempts","goose.records.queue.v1","goose.result.best."+flow.library.themes[0].id+".2"};
        string date=PlayerPrefs.GetString(keys[0],"");int attempts=PlayerPrefs.GetInt(keys[1],0);string queue=PlayerPrefs.GetString(keys[2],"");float best=PlayerPrefs.GetFloat(keys[3],0);
        Match3DRewardedVideo.EditorShowOverride=done=>done(true,null);
        try{
            flow.Begin(0);BoardCreationTestSupport.FinishPending(flow);flow.NextStage();BoardCreationTestSupport.FinishPending(flow);yield return null;
            var input=InputController.Instance;var board=LevelGenerator.Instance.objectsOnScene;
            var record=Field(records,"active");int attempt=PlayerPrefs.GetInt(keys[1]);
            var seven=board.GroupBy(x=>x.name).SelectMany(g=>g.Take(2)).Take(7).ToArray();Assert(seven.Length==7,"fixture has seven nonmatching objects");
            GameManager.Instance.currentLevelTime=123;
            foreach(var obj in seven)input.AddObjectOnStack(obj,false);
            Assert(flow.RevivePending&&flow.BlocksGameplay&&Time.timeScale==0,"full tray opens revive and pauses gameplay");
            Assert(ReferenceEquals(record,Field(records,"active")),"offer does not finish the active game record");
            flow.HandleResult(false);var popup=UnityEngine.Object.FindObjectOfType<Match3DReviveView>();Assert(UnityEngine.Object.FindObjectsOfType<Match3DReviveView>().Length==1,"duplicate failure does not create duplicate popup");
            yield return new WaitForSecondsRealtime(.6f);Capture("clear");float time=GameManager.Instance.currentLevelTime;
            Action<bool,string> adReply=null;int adRequests=0;
            Match3DRewardedVideo.EditorShowOverride=done=>{adRequests++;adReply=done;};
            popup.revive.onClick.Invoke();popup.revive.onClick.Invoke();
            Assert(adRequests==1&&flow.RevivePending&&!flow.ClearReviveUsed&&input.objectsOnStack.Count==7,"ad pending blocks duplicate clicks and grants nothing");
            adReply(false,"未完整观看");Assert(popup.group.interactable&&flow.RevivePending&&!flow.ClearReviveUsed,"early close preserves allowance and allows retry");
            popup.revive.onClick.Invoke();adReply(false,"广告加载失败");Assert(popup.group.interactable&&!flow.ClearReviveUsed,"ad failure grants nothing and allows retry");
            popup.revive.onClick.Invoke();adReply(true,null);adReply(true,null);
            Match3DRewardedVideo.EditorShowOverride=done=>done(true,null);
            Assert(!flow.RevivePending&&!flow.BlocksGameplay&&flow.ClearReviveUsed&&GameManager.Instance.GameIsPlaying(),"clear revival resumes same board once");
            Assert(input.objectsOnStack.Count==0&&BoardTools.StoredCount==7&&seven.All(x=>x&&board.Contains(x)&&BoardTools.IsStored(x)),"all seven objects remain collectible in storage");
            Assert(BoardTools.UsedCount(0)==0&&Mathf.Abs(time-GameManager.Instance.currentLevelTime)<.01f,"clear revival does not spend move tool or change timer");
            Assert(ReferenceEquals(record,Field(records,"active"))&&PlayerPrefs.GetInt(keys[1])==attempt,"revival preserves record and challenge count");
            yield return new WaitForSeconds(.5f);
            string kind=seven[0].name;var triple=board.Where(x=>x.name==kind).Take(3).ToArray();int before=board.Count;
            foreach(var obj in triple)input.AddObjectOnStack(obj,false);yield return new WaitForSeconds(1);
            Assert(board.Count==before-3,"stored objects can be picked and matched after revival");
            GameManager.Instance.currentLevelTime=0;flow.HandleResult(false);Assert(flow.RevivePending,"time revive remains available after clear revive");yield return new WaitForSecondsRealtime(.6f);Capture("time");
            popup=UnityEngine.Object.FindObjectOfType<Match3DReviveView>();popup.revive.onClick.Invoke();popup.revive.onClick.Invoke();
            Assert(flow.TimeReviveUsed&&GameManager.Instance.currentLevelTime==300&&GameManager.Instance.GameIsPlaying(),"time revive adds exactly five minutes once");
            GameManager.Instance.currentLevelTime=0;flow.HandleResult(false);
            Assert(!flow.RevivePending&&GameManager.Instance.gameStatus==GameManager.GameStatus.FAIL&&UnityEngine.Object.FindObjectOfType<Match3DResultView>(),"second timeout finishes in failure screen");Assert(Field(records,"active")==null,"terminal failure finishes record");
            UnityEngine.Object.FindObjectOfType<Match3DResultView>().secondary.onClick.Invoke();BoardCreationTestSupport.FinishPending(flow);yield return null;
            Assert(!flow.ClearReviveUsed&&!flow.TimeReviveUsed,"retry resets both revive allowances");
            board=LevelGenerator.Instance.objectsOnScene;
            foreach(var obj in board.GroupBy(x=>x.name).SelectMany(g=>g.Take(2)).Take(7).ToArray())input.AddObjectOnStack(obj,false);
            yield return new WaitForSecondsRealtime(.5f);UnityEngine.Object.FindObjectOfType<Match3DReviveView>().revive.onClick.Invoke();yield return new WaitForSeconds(.5f);
            foreach(var obj in board.Where(x=>!BoardTools.IsStored(x)).GroupBy(x=>x.name).SelectMany(g=>g.Take(2)).Take(7).ToArray())input.AddObjectOnStack(obj,false);
            Assert(!flow.RevivePending&&GameManager.Instance.gameStatus==GameManager.GameStatus.FAIL,"second full tray cannot reuse clear revival");
            flow.RestartStage();BoardCreationTestSupport.FinishPending(flow);yield return null;
            GameManager.Instance.currentLevelTime=0;flow.HandleResult(false);yield return new WaitForSecondsRealtime(.5f);
            popup=UnityEngine.Object.FindObjectOfType<Match3DReviveView>();popup.close.onClick.Invoke();
            Assert(!flow.RevivePending&&GameManager.Instance.gameStatus==GameManager.GameStatus.FAIL&&Field(records,"active")==null,"closing revive settles failure without restart");
        }finally{Match3DRewardedVideo.EditorShowOverride=null;Match3DRewardAcquisition.Configure(null);flow.Home();PlayerPrefs.SetString(keys[0],date);PlayerPrefs.SetInt(keys[1],attempts);PlayerPrefs.SetString(keys[2],queue);PlayerPrefs.SetFloat(keys[3],best);PlayerPrefs.Save();}
    }
    static void Capture(string name){
        var c=Camera.main;var canvases=UnityEngine.Object.FindObjectsOfType<Canvas>().Where(x=>x.isRootCanvas&&x.gameObject.activeInHierarchy).ToArray();var modes=canvases.Select(x=>x.renderMode).ToArray();var cameras=canvases.Select(x=>x.worldCamera).ToArray();var distances=canvases.Select(x=>x.planeDistance).ToArray();var rt=new RenderTexture(720,1280,24);rt.Create();var old=c.targetTexture;var active=RenderTexture.active;c.targetTexture=rt;
        for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=RenderMode.ScreenSpaceCamera;canvases[i].worldCamera=c;canvases[i].planeDistance=1;}
        Canvas.ForceUpdateCanvases();foreach(var fit in UnityEngine.Object.FindObjectsOfType<Match3DUIFitContent>())fit.Fit();Canvas.ForceUpdateCanvases();c.Render();RenderTexture.active=rt;var tex=new Texture2D(720,1280,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,720,1280),0,0);tex.Apply();File.WriteAllBytes(Output+"/"+name+"-720x1280.png",tex.EncodeToPNG());c.targetTexture=old;RenderTexture.active=active;
        for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];canvases[i].planeDistance=distances[i];}UnityEngine.Object.Destroy(tex);rt.Release();UnityEngine.Object.Destroy(rt);
    }
}
