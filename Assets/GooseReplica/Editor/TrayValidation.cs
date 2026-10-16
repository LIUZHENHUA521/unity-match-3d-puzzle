using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class TrayValidation {
    static int step;static double due,deadline;static string report="";static RenderTexture target;
    static void Check(bool value,string message){if(!value)throw new Exception(message);report+="PASS "+message+"\n";}
    public static void Run(){
        Directory.CreateDirectory("Builds/TrayPreview");EditorSettings.enterPlayModeOptionsEnabled=true;EditorSettings.enterPlayModeOptions=EnterPlayModeOptions.DisableDomainReload;
        EditorSceneManager.OpenScene("Assets/Scenes/GooseReplica.unity");EditorApplication.isPlaying=true;
        step=0;due=EditorApplication.timeSinceStartup+5;deadline=due+90;EditorApplication.update+=Tick;
    }
    static void Tick(){
        if(EditorApplication.timeSinceStartup<due||!EditorApplication.isPlaying)return;
        try{
            if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Timeout");
            var flow=ReplicaFlow.Instance;if(!flow||string.IsNullOrEmpty(flow.ScreenName))return;
            var input=InputController.Instance;var board=LevelGenerator.Instance.objectsOnScene;
            switch(step++){
                case 0:
                    target=new RenderTexture(720,1560,24);target.Create();Camera.main.targetTexture=target;
                    var canvas=GameObject.Find("Goose UI").GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=Camera.main;canvas.planeDistance=1;
                    Camera.main.GetComponent<CameraFitter>().SendMessage("Fit");Canvas.ForceUpdateCanvases();flow.Begin(0);due=EditorApplication.timeSinceStartup+4;return;
                case 1:
                    Check(!GameObject.Find("Stack").GetComponent<Renderer>().enabled,"old thick seven-cell texture hidden");
                    var images=GameObject.Find("Collection Tray").GetComponentsInChildren<Image>();Check(images.Length==8&&images.All(i=>i.sprite),"one frame and seven separate cell sprites");
                    Check(images.All(i=>!i.raycastTarget),"tray cannot intercept gameplay input");
                    Capture("empty");input.AddObjectOnStack(board.First());break;
                case 2:
                    Check(input.objectsOnStack.Count==1,"pick reaches tray");
                    Check(Vector3.Distance(input.objectsOnStack[0].transform.position,ReplicaTray.Instance.SlotPosition(0))<.01f,"3D object aligned to first cell");
                    Capture("one-item");input.UndoLast();break;
                case 3:
                    Check(input.objectsOnStack.Count==0&&board.Count==12,"undo returns object to board");
                    foreach(var obj in board.GroupBy(o=>o.name).First().Take(3).ToArray())input.AddObjectOnStack(obj);break;
                case 4:
                    Check(input.objectsOnStack.Count==0&&board.Count==9,"triple match removes three");flow.NextStage();due=EditorApplication.timeSinceStartup+4;return;
                case 5:
                    foreach(var obj in board.GroupBy(o=>o.name).Take(7).Select(g=>g.First()).ToArray())input.AddObjectOnStack(obj);break;
                case 6:
                    Check(input.objectsOnStack.Count==7&&GameManager.Instance.gameStatus==GameManager.GameStatus.FAIL,"seven different items still fail");
                    for(int i=0;i<7;i++)Check(Vector3.Distance(input.objectsOnStack[i].transform.position,ReplicaTray.Instance.SlotPosition(i))<.01f,"slot alignment "+i);
                    flow.Home();break;
                case 7:Check(!GameObject.Find("Collection Tray"),"tray hidden on homepage");Check(board.Count==0,"home clears board");Finish(null);break;
            }
            due=EditorApplication.timeSinceStartup+1;
        }catch(Exception e){Finish(e);}
    }
    static void Capture(string name){
        Canvas.ForceUpdateCanvases();Camera.main.Render();var old=RenderTexture.active;RenderTexture.active=target;
        var image=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,target.width,target.height),0,0);image.Apply();
        File.WriteAllBytes("Builds/TrayPreview/"+name+".png",image.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(image);
    }
    static void Finish(Exception error){
        EditorApplication.update-=Tick;report+=error==null?"ALL CHECKS PASSED\n":"FAIL "+error+"\n";
        File.WriteAllText("Builds/TrayPreview/validation.txt",report);Debug.Log(report);EditorApplication.Exit(error==null?0:1);
    }
}
