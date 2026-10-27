using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class ReplicaUIPrefabValidation {
    static int step;static double due;static string report;static float pausedTime;
    static ReplicaFlow Flow=>ReplicaFlow.Instance;
    static ReplicaUIView View(string name)=>UnityEngine.Object.FindObjectsOfType<ReplicaUIView>().First(v=>v.name==name+"(Clone)");
    static void Check(bool success,string message){if(!success)throw new Exception(message);report+="PASS "+message+"\n";}
    static void Click(string view,string key){View(view).Get<Button>(key).onClick.Invoke();}
    static void Private(string name,params object[] args){typeof(ReplicaFlow).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(Flow,args);}
    [MenuItem("Goose/UI/Validate prefab UI in Play Mode")]
    public static void Run(){
        if(!Application.isPlaying||!Flow)throw new Exception("Start GooseReplica first");
        report="";step=0;due=0;Directory.CreateDirectory("Temp/ui-migration");EditorApplication.update+=Tick;
    }
    static void Tick(){
        if(EditorApplication.timeSinceStartup<due)return;
        due=EditorApplication.timeSinceStartup+.3;
        try{
            switch(step++){
            case 0:
                foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/GooseReplica/Resources/Replica/UI"})){
                    var asset=AssetDatabase.LoadAssetAtPath<ReplicaUIView>(AssetDatabase.GUIDToAssetPath(guid));
                    Check(asset && asset.bindings.All(b=>b.target) && asset.bindings.Select(b=>b.key).Distinct().Count()==asset.bindings.Length,"references "+asset.name);
                    Check(asset.GetComponentsInChildren<Transform>(true).All(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0),"no missing scripts "+asset.name);
                }
                Flow.Home();break;
            case 1:Check(Flow.ScreenName=="home","home opens from prefab");Check(UnityEngine.Object.FindObjectsOfType<Button>().Length==7,"home button count");Click("Home","Tab Collection");break;
            case 2:Check(Flow.ScreenName=="collection-tab","collection tab binding");Click("CollectionTab","Tab Pets");break;
            case 3:Check(Flow.ScreenName=="pets-tab","pets tab binding");Click("PetsTab","Tab Home");break;
            case 4:Click("Home","Rewards");break;
            case 5:Check(Flow.BlocksGameplay,"reward blocks input");Click("Reward","Close");Click("Home","Settings");break;
            case 6:
                Check(Time.timeScale==1 && !View("PauseSettings").Get<Transform>("Home").gameObject.activeSelf,"home settings does not pause / hides return-home");
                bool prior=Flow.SettingsEffects;Click("PauseSettings","Effects");Check(Flow.SettingsEffects!=prior,"effects toggle updates preference");Click("PauseSettings","Effects");
                Click("PauseSettings","Continue");Click("Home","Elite");break;
            case 7:Check(Flow.ScreenName=="themes","theme page opens");Click("Themes","Back");break;
            case 8:Click("Home","Bachelor");break;
            case 9:
                Check(Flow.ScreenName=="game"&&Flow.Stage==1,"begin gameplay binding");
                Check(LevelGenerator.Instance.objectsOnScene.Count==12,"tutorial spawn unchanged");
                var hud=View("GameHUD");var authored=Resources.Load<ReplicaUIView>("Replica/UI/GameHUD");
                Check(hud.Get<RectTransform>("Pause").anchoredPosition==authored.Get<RectTransform>("Pause").anchoredPosition,"authored pause position preserved");
                Check(hud.Get<Image>("Pause").sprite==authored.Get<Image>("Pause").sprite,"authored pause sprite preserved");
                Check(!hud.Get<Transform>("Freeze").gameObject.activeSelf,"art HUD omits legacy freeze button");Click("GameHUD","Pause");break;
            case 10:
                Check(Time.timeScale==0 && Flow.BlocksGameplay,"pause freezes game and blocks picking");pausedTime=GameManager.Instance.currentLevelTime;
                ScreenCapture.CaptureScreenshot("Temp/ui-migration/pause.png");due=EditorApplication.timeSinceStartup+.8;break;
            case 11:
                Check(Mathf.Approximately(pausedTime,GameManager.Instance.currentLevelTime),"timer frozen while paused");
                Click("PauseSettings","Continue");Check(Time.timeScale==1&&!Flow.BlocksGameplay,"continue resumes game");
                Private("Result",true,"第一关通过！","测试结算按钮");break;
            case 12:Check(View("Result").Get<Text>("Title").text=="第一关通过！","result dynamic title");Click("Result","Continue");break;
            case 13:Check(Flow.Stage==2,"result continue starts second stage");Check(!View("GameHUD").Get<Transform>("Freeze").gameObject.activeSelf,"stage2 hides freeze");Flow.ShowModelSize();break;
            case 14:
                var slider=View("ModelSize").Get<Slider>("Slider");float value=slider.value;Click("ModelSize","Minus");Check(slider.value==value-1,"model slider minus binding");Click("ModelSize","Plus");Click("ModelSize","Close");
                Flow.ShowPvp();break;
            case 15:
                Check(Flow.ScreenName=="pvp","pvp lobby opens");Check(View("PvpLobby").Get<Transform>("Join").gameObject.activeSelf&&!View("PvpLobby").Get<Transform>("Ready").gameObject.activeSelf,"disconnected lobby state");
                Flow.ShowSettings(true);Check(Time.timeScale==1,"pvp settings never pauses simulation");Click("PauseSettings","Continue");Flow.ShowCollection();break;
            case 16:Check(Flow.ScreenName=="collection","collection page opens");Click("Collection","Back");break;
            case 17:ScreenCapture.CaptureScreenshot("Temp/ui-migration/home-final.png");Flow.Begin(0);break;
            case 18:Click("GameHUD","Pause");break;
            case 19:Click("PauseSettings","Home");break;
            case 20:
                Check(Flow.ScreenName=="home"&&Time.timeScale==1&&!Flow.BlocksGameplay,"pause return-home restores time and input");
                Check(LevelGenerator.Instance.objectsOnScene.Count==0,"return-home clears gameplay");Finish(null);break;
            }
        }catch(Exception e){Finish(e);}
    }
    static void Finish(Exception e){EditorApplication.update-=Tick;report+=e==null?"ALL CHECKS PASSED\n":"FAIL "+e+"\n";File.WriteAllText("Temp/ui-migration/validation.txt",report);Debug.Log(report);}
}
