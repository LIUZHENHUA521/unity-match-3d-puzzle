using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class PotResourceCleanupValidation {
    static int step;
    static double due, deadline;
    static string report="";
    const string Output="work/pot-resource-cleanup/";
    static void Check(bool value,string message){if(!value)throw new Exception(message);report+="PASS "+message+"\n";}
    public static void Run(){
        Directory.CreateDirectory(Output);
        Check(Match3DThemePresentation.SceneAssetName("CountrysideStove")=="Match3DBoard","legacy environment resolves to renamed board");
        var board=Resources.Load<GameObject>("Match3D/Scenes/Match3DBoard");
        Check(board&&board.name=="Match3DBoard","renamed board root loads");
        Check(!Resources.Load<GameObject>("Match3D/Scenes/CountrysideStove"),"old board resource path removed");
        Check(!Directory.Exists("Assets/Match3D/Resources/GooseArt"),"extracted art directory removed");
        foreach(var name in new[]{"PvpLobby","PvpGameHUD","Result","Themes"})
            Check(!Resources.Load<GameObject>("Match3D/UI/"+name),"retired page absent: "+name);
        foreach(var name in new[]{"GameHUD","ModelSize","Diagnostics","Collection","Reward"}){
            var prefab=Resources.Load<Match3DUIView>("Match3D/UI/"+name);
            Check(prefab&&prefab.bindings.All(b=>b.target),"UI bindings intact: "+name);
            Check(prefab.GetComponentsInChildren<Transform>(true).All(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0),"no missing scripts: "+name);
        }
        var library=Resources.Load<Match3DTheme>("Match3D/ThemeLibrary");
        Check(library.homeMusic&&library.gameMusic&&library.clickSound&&library.selectSound&&library.matchSound,"retained music and interaction sounds assigned");
        Check(!library.winSound&&!library.loseSound,"removed result sounds have no dangling references");
        EditorSettings.enterPlayModeOptionsEnabled=true;
        EditorSettings.enterPlayModeOptions=EnterPlayModeOptions.DisableDomainReload;
        EditorSceneManager.OpenScene("Assets/Scenes/Match3D.unity");
        due=EditorApplication.timeSinceStartup+4;deadline=due+150;
        EditorApplication.update+=Tick;EditorApplication.isPlaying=true;
    }
    static void Tick(){
        if(EditorApplication.timeSinceStartup<due)return;
        try{
            if(EditorApplication.timeSinceStartup>deadline)throw new Exception("cleanup runtime validation timed out");
            if(!EditorApplication.isPlaying||!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)return;
            var flow=Match3DFlow.Instance;
            switch(step){
                case 0:
                    Check(flow.ScreenName=="home","home startup succeeds without PVP transport");
                    Check(flow.GetComponents<Component>().All(c=>c&&c.GetType().Name!="Match3DNetwork"),"no PVP network service");
                    flow.ShowCollection();break;
                case 1:
                    var collection=UnityEngine.Object.FindObjectsOfType<Match3DUIView>().First(v=>v.name.StartsWith("Collection("));
                    Check(collection.Get<Image>("Goose0").sprite,"collection uses current theme artwork");
                    flow.Home();flow.ShowRewardInfo();break;
                case 2:
                    Check(flow.BlocksGameplay,"reward dialog opens");flow.Home();flow.ShowSettings(false);break;
                case 3:
                    Check(flow.BlocksGameplay,"settings opens");flow.Home();flow.Begin(0);break;
                case 4:
                    if(flow.CreationInProgress)return;
                    Check(flow.ScreenName=="game"&&flow.Stage==1&&!flow.IsPvp,"single-player first stage starts");
                    Check(LevelGenerator.Instance.objectsOnScene.Count>0,"first-stage items created");
                    flow.NextStage();break;
                case 5:
                    if(flow.CreationInProgress)return;
                    Check(flow.Stage==2&&LevelGenerator.Instance.objectsOnScene.Count>0,"second-stage items created");
                    flow.ShowModelSize();break;
                case 6:
                    Check(flow.BlocksGameplay&&Time.timeScale==0,"model-size dialog opens and pauses");
                    typeof(Match3DFlow).GetMethod("CloseModal",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(flow,new object[]{true});
                    typeof(Match3DFlow).GetMethod("Result",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(flow,new object[]{false,"test","test"});break;
                case 7:
                    Check(UnityEngine.Object.FindObjectOfType<Match3DResultView>(),"single-player result opens without retired page or sound");
                    flow.Home();Check(Time.timeScale==1&&!flow.BlocksGameplay,"home restores time and input");Finish(null);return;
            }
            step++;due=EditorApplication.timeSinceStartup+1;
        }catch(Exception e){Finish(e);}
    }
    static void Finish(Exception error){
        EditorApplication.update-=Tick;
        report+=error==null?"ALL CHECKS PASSED\n":"FAIL "+error+"\n";
        File.WriteAllText(Output+"result.txt",report);Debug.Log(report);EditorApplication.Exit(error==null?0:1);
    }
}
