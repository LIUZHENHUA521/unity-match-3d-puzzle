using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class ReplicaGameplaySkinValidation {
    static int step;static double due,deadline;static string report;
    static ReplicaFlow Flow=>ReplicaFlow.Instance;
    static ReplicaUIView Hud=>UnityEngine.Object.FindObjectOfType<ReplicaGameplayHUD>().GetComponent<ReplicaUIView>();
    static void Check(bool condition,string message){if(!condition)throw new Exception(message);report+="PASS "+message+"\n";}
    public static void FinalizeAssets(){
        const string path="Assets/GooseReplica/Resources/Replica/UI/CollectionTray.prefab";
        var root=PrefabUtility.LoadPrefabContents(path);
        try{root.GetComponent<ReplicaTraySkin>().footerBackground=(RectTransform)root.transform.Find("Toolbar background");PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
        const string pvp="Assets/GooseReplica/Resources/Replica/UI/PvpGameHUD.prefab";
        root=PrefabUtility.LoadPrefabContents(pvp);try{root.name="PvpGameHUD";PrefabUtility.SaveAsPrefabAsset(root,pvp);}finally{PrefabUtility.UnloadPrefabContents(root);}
    }
    [MenuItem("Goose/UI/Validate gameplay art in Play Mode")]
    public static void Run(){
        if(!Application.isPlaying||!Flow)throw new Exception("Start game first");
        step=0;due=0;deadline=EditorApplication.timeSinceStartup+90;report="";EditorApplication.update+=Tick;
    }
    static void Tick(){
        if(EditorApplication.timeSinceStartup<due)return;
        due=EditorApplication.timeSinceStartup+.5;
        try{
            if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Validation timeout");
            if(step==5 && Flow.CreationInProgress)return;
            switch(step++){
            case 0:Flow.Begin(0);due=EditorApplication.timeSinceStartup+4;break;
            case 1:
                Check(Hud.Get<Text>("Timer").text=="∞","unlimited tutorial timer");
                Check(Hud.Get<RectTransform>("Settings").sizeDelta==new Vector2(70,70),"authored settings control");
                Hud.Get<Button>("Settings").onClick.Invoke();Check(Flow.BlocksGameplay&&Time.timeScale==0,"new settings button pauses gameplay");break;
            case 2:
                UnityEngine.Object.FindObjectsOfType<ReplicaUIView>().First(v=>v.name=="PauseSettings(Clone)").Get<Button>("Continue").onClick.Invoke();
                InputController.Instance.AddObjectOnStack(LevelGenerator.Instance.objectsOnScene[0]);due=EditorApplication.timeSinceStartup+1;break;
            case 3:
                Check(InputController.Instance.objectsOnStack.Count==1,"tutorial item collected");Check(Vector3.Distance(InputController.Instance.objectsOnStack[0].transform.position,ReplicaTray.Instance.SlotPosition(0))<.01f,"tutorial item lands at blue slot");
                Hud.Get<Button>("Undo").onClick.Invoke();due=EditorApplication.timeSinceStartup+1;break;
            case 4:Check(InputController.Instance.objectsOnStack.Count==0,"remove button returns tutorial item");Flow.NextStage();break;
            case 5:
                Check(LevelGenerator.Instance.objectsOnScene.Count==300,"challenge generation unchanged");
                Check(Hud.Get<Text>("Timer").text!="∞"&&Hud.Get<Text>("Timer").text.Contains(":"),"challenge uses live timer");
                Check(Hud.GetComponent<ReplicaGameplayHUD>().quantities.All(t=>t.text=="1"&&t.transform.parent.gameObject.activeSelf),"all three badges show real initial uses");
                InputController.Instance.AddObjectOnStack(LevelGenerator.Instance.objectsOnScene[0]);due=EditorApplication.timeSinceStartup+1;break;
            case 6:
                Check(Vector3.Distance(InputController.Instance.objectsOnStack[0].transform.position,ReplicaTray.Instance.SlotPosition(0))<.01f,"challenge item lands at blue slot");
                Hud.Get<Button>("Undo").onClick.Invoke();due=EditorApplication.timeSinceStartup+1;break;
            case 7:
                Check(ReplicaSourceTools.UsedCount(0)==1&&InputController.Instance.objectsOnStack.Count==0,"remove button consumes real use and empties tray");
                Check(Hud.GetComponent<ReplicaGameplayHUD>().quantities[0].text=="0","remaining badge updates after use");
                InputController.Instance.AddObjectOnStack(LevelGenerator.Instance.objectsOnScene.First(o=>!ReplicaSourceTools.IsStored(o)));due=EditorApplication.timeSinceStartup+1;break;
            case 8:Hud.Get<Button>("Hint").onClick.Invoke();due=EditorApplication.timeSinceStartup+2;break;
            case 9:Check(ReplicaSourceTools.UsedCount(1)==1,"adsorb button invokes auto-find");Hud.Get<Button>("Shuffle").onClick.Invoke();due=EditorApplication.timeSinceStartup+2;break;
            case 10:Check(ReplicaSourceTools.UsedCount(2)==1&&!ReplicaSourceTools.Shuffling,"shuffle button invokes and finishes shuffle");Flow.Home();Flow.Begin(0);due=EditorApplication.timeSinceStartup+4;break;
            case 11:ScreenCapture.CaptureScreenshot("Temp/gameplay-skin/final-tutorial.png");Flow.NextStage();break;
            case 12:if(Flow.CreationInProgress){step--;return;}ScreenCapture.CaptureScreenshot("Temp/gameplay-skin/final-challenge.png");Finish(null);break;
            }
        }catch(Exception e){Finish(e);}
    }
    static void Finish(Exception error){EditorApplication.update-=Tick;report+=error==null?"ALL GAMEPLAY SKIN CHECKS PASSED\n":"FAIL "+error+"\n";File.WriteAllText("Temp/gameplay-skin/validation.txt",report);Debug.Log(report);}
}
