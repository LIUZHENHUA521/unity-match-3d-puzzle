using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class HomeSkinValidation {
    static int step;static double due,deadline;static string report="";
    static void Check(bool value,string message){if(!value)throw new Exception(message);report+="PASS "+message+"\n";}
    public static void Run(){
        HomeSkinAssets.Ensure();Directory.CreateDirectory("Builds/HomePreview");
        EditorSettings.enterPlayModeOptionsEnabled=true;EditorSettings.enterPlayModeOptions=EnterPlayModeOptions.DisableDomainReload;
        EditorSceneManager.OpenScene("Assets/Scenes/GooseReplica.unity");
        EditorApplication.isPlaying=true;step=0;due=EditorApplication.timeSinceStartup+5;deadline=due+90;EditorApplication.update+=Tick;
    }
    static Button Button(string name){return UnityEngine.Object.FindObjectsOfType<Button>().First(b=>b.name==name);}
    static void Tick(){
        if(EditorApplication.timeSinceStartup<due||!EditorApplication.isPlaying)return;
        try{
            if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Timed out");
            var flow=ReplicaFlow.Instance;if(!flow||string.IsNullOrEmpty(flow.ScreenName))return;
            switch(step++){
                case 0:
                    Check(flow.ScreenName=="home","launch home");
                    Check(UnityEngine.Object.FindObjectsOfType<Button>().Count(b=>b.name.StartsWith("Tab "))==3,"exactly three home tabs");
                    Check(UnityEngine.Object.FindObjectsOfType<Image>().Where(i=>i.transform.IsChildOf(UnityEngine.Object.FindObjectOfType<ReplicaHomeView>().transform)&&i.name!="Footer background"&&i.name!="Rewards").All(i=>i.sprite!=null),"home sprites resolve");
                    Capture(720,1560,"phone-long");Capture(720,1280,"phone-standard");Capture(768,1024,"tablet");
                    Button("Tab Collection").onClick.Invoke();break;
                case 1:Check(flow.ScreenName=="collection-tab","collection placeholder");Button("Tab Pets").onClick.Invoke();break;
                case 2:Check(flow.ScreenName=="pets-tab","pets placeholder");Button("Tab Home").onClick.Invoke();break;
                case 3:Button("Settings").onClick.Invoke();Check(flow.BlocksGameplay,"settings opens");break;
                case 4:UnityEngine.Object.FindObjectsOfType<Button>().First(b=>b.GetComponentInChildren<Text>()?.text=="完成").onClick.Invoke();Check(!flow.BlocksGameplay,"settings closes");Button("Rewards").onClick.Invoke();break;
                case 5:Check(flow.BlocksGameplay,"reward explanation opens");UnityEngine.Object.FindObjectsOfType<Button>().First(b=>b.GetComponentInChildren<Text>()?.text=="知道了").onClick.Invoke();Check(!flow.BlocksGameplay,"reward explanation closes");Button("Elite").onClick.Invoke();Check(flow.ScreenName=="themes","elite opens theme selection");flow.Home();break;
                case 6:Button("Bachelor").onClick.Invoke();Check(flow.ScreenName=="game"&&flow.ThemeIndex==0&&flow.Stage==1,"bachelor starts ordinary tutorial");Check(LevelGenerator.Instance.objectsOnScene.Count==12,"ordinary gameplay unchanged");flow.Home();break;
                case 7:Check(flow.ScreenName=="home"&&LevelGenerator.Instance.objectsOnScene.Count==0,"return clears board");Finish(null);break;
            }
            due=EditorApplication.timeSinceStartup+.5;
        }catch(Exception e){Finish(e);}
    }
    static void Capture(int width,int height,string name){
        var canvas=GameObject.Find("Goose UI").GetComponent<Canvas>();var cam=Camera.main;
        var target=new RenderTexture(width,height,24);target.Create();
        cam.targetTexture=target;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=1;
        Canvas.ForceUpdateCanvases();
        var view=UnityEngine.Object.FindObjectOfType<ReplicaHomeView>();view.SendMessage("Layout");Canvas.ForceUpdateCanvases();
        foreach(string button in new[]{"Bachelor","Elite","Tab Home","Tab Collection","Tab Pets"}){
            var rect=Button(button).GetComponent<RectTransform>();var corners=new Vector3[4];rect.GetWorldCorners(corners);
            var points=corners.Select(p=>cam.WorldToViewportPoint(p)).ToArray();
            Check(points.All(p=>p.x>=-.01f&&p.x<=1.01f&&p.y>=-.01f&&p.y<=1.01f),name+" bounds "+button);
        }
        cam.Render();var previous=RenderTexture.active;RenderTexture.active=target;
        var texture=new Texture2D(width,height,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply();
        File.WriteAllBytes("Builds/HomePreview/"+name+".png",texture.EncodeToPNG());
        RenderTexture.active=previous;cam.targetTexture=null;canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        UnityEngine.Object.DestroyImmediate(texture);UnityEngine.Object.DestroyImmediate(target);
    }
    static void Finish(Exception error){
        EditorApplication.update-=Tick;
        report+=error==null?"ALL CHECKS PASSED\n":"FAIL "+error+"\n";
        File.WriteAllText("Builds/HomePreview/validation.txt",report);Debug.Log(report);EditorApplication.Exit(error==null?0:1);
    }
}
