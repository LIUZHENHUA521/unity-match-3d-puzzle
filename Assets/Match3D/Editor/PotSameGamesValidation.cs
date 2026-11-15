using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class PotSameGamesValidation {
    const string Output="work/pot-same-games-validation/";
    static int step;static double due,deadline;static string report="";
    static bool oldOptionsEnabled;static EnterPlayModeOptions oldOptions;
    static void Check(bool ok,string description){if(!ok)throw new Exception(description);report+="PASS "+description+"\n";}
    public static void Run(){
        PotSameGamesBuilder.Build();Directory.CreateDirectory(Output);
        oldOptionsEnabled=EditorSettings.enterPlayModeOptionsEnabled;oldOptions=EditorSettings.enterPlayModeOptions;
        EditorSettings.enterPlayModeOptionsEnabled=true;EditorSettings.enterPlayModeOptions=EnterPlayModeOptions.DisableDomainReload;
        EditorSceneManager.OpenScene("Assets/Scenes/Match3D.unity");
        step=0;due=EditorApplication.timeSinceStartup+5;deadline=due+100;
        EditorApplication.update+=Tick;EditorApplication.isPlaying=true;
    }
    static Match3DUIView Modal()=>UnityEngine.Object.FindObjectsOfType<Match3DUIView>().First(v=>v.name.StartsWith("PotSameGames"));
    static void Tick(){
        if(EditorApplication.timeSinceStartup<due)return;
        try{
            if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Home startup timed out");
            if(!EditorApplication.isPlaying)return;
            var flow=Match3DFlow.Instance;if(!flow||flow.ScreenName!="home")return;
            switch(step++){
                case 0:
                    var home=UnityEngine.Object.FindObjectOfType<Match3DHomeView>();
                    Check(home.sameGames&&home.sameGames.gameObject.activeInHierarchy,"home entry wired and visible");
                    Capture(720,1560,"home");home.sameGames.onClick.Invoke();break;
                case 1:
                    Check(flow.BlocksGameplay,"modal blocks background interaction");
                    Check(Modal().GetComponentsInChildren<Button>().Length==3,"only close and two game buttons");
                    Check(Modal().GetComponentsInChildren<Text>().Any(t=>t.text=="尽情期待"),"coming-soon footer");
                    Capture(720,1560,"popup-long");Capture(720,1280,"popup-standard");Capture(768,1024,"popup-tablet");
                    Modal().Get<Button>("CurrentGame").onClick.Invoke();
                    Check(UnityEngine.Object.FindObjectsOfType<Text>().Any(t=>t.text==PotSameGames.CurrentGameNotice),"current-game button displays exact notice");
                    Check(Modal().gameObject.activeInHierarchy,"current-game notice keeps popup open");
                    Capture(720,1560,"current-game-notice");break;
                case 2:
                    Modal().Get<Button>("CrazyCombo").onClick.Invoke();
                    Check(UnityEngine.Object.FindObjectsOfType<Text>().Any(t=>t.text=="请在微信小游戏中打开疯狂动物消"),"editor navigation fallback");
                    Modal().Get<Button>("Close").onClick.Invoke();Check(!flow.BlocksGameplay,"close restores home input");break;
                case 3:
                    UnityEngine.Object.FindObjectOfType<Match3DHomeView>().sameGames.onClick.Invoke();break;
                case 4:
                    Check(UnityEngine.Object.FindObjectsOfType<Match3DUIView>().Count(v=>v.name.StartsWith("PotSameGames"))==1,"reopen creates one modal");
                    Modal().Get<Button>("Close").onClick.Invoke();Finish(null);return;
            }
            due=EditorApplication.timeSinceStartup+.8;
        }catch(Exception e){Finish(e);}
    }
    static void Capture(int width,int height,string name){
        var canvas=GameObject.Find("Goose UI").GetComponent<Canvas>();var camera=Camera.main;
        var target=new RenderTexture(width,height,24);target.Create();
        var previous=RenderTexture.active;var oldTarget=camera.targetTexture;
        var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;
        Texture2D texture=null;
        try{
            camera.targetTexture=target;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
            Canvas.ForceUpdateCanvases();
            UnityEngine.Object.FindObjectOfType<Match3DHomeView>().SendMessage("Layout");
            foreach(var fit in UnityEngine.Object.FindObjectsOfType<Match3DUIFitContent>())fit.Fit();
            Canvas.ForceUpdateCanvases();
            var modal=UnityEngine.Object.FindObjectsOfType<Match3DUIView>().FirstOrDefault(v=>v.name.StartsWith("PotSameGames"));
            var controls=modal?modal.GetComponentsInChildren<Button>():new[]{UnityEngine.Object.FindObjectOfType<Match3DHomeView>().sameGames};
            foreach(var button in controls){
                var corners=new Vector3[4];((RectTransform)button.transform).GetWorldCorners(corners);
                Check(corners.Select(camera.WorldToViewportPoint).All(p=>p.x>=0&&p.x<=1&&p.y>=0&&p.y<=1),name+" visible button "+button.name);
            }
            camera.Render();RenderTexture.active=target;texture=new Texture2D(width,height,TextureFormat.RGB24,false);
            texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply();File.WriteAllBytes(Output+name+".png",texture.EncodeToPNG());
        }finally{
            RenderTexture.active=previous;camera.targetTexture=oldTarget;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;
            if(texture)UnityEngine.Object.DestroyImmediate(texture);UnityEngine.Object.DestroyImmediate(target);
        }
    }
    static void Finish(Exception error){
        EditorApplication.update-=Tick;EditorSettings.enterPlayModeOptionsEnabled=oldOptionsEnabled;EditorSettings.enterPlayModeOptions=oldOptions;
        report+=error==null?"ALL CHECKS PASSED\n":"FAIL "+error+"\n";
        File.WriteAllText(Output+"result.txt",report);Debug.Log(report);EditorApplication.Exit(error==null?0:1);
    }
}
