using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class StartupHomeValidation {
    static bool watching,firstCanvas;static double deadline;static int step;
    static StartupHomeValidation(){EditorApplication.update+=Tick;if(SessionState.GetBool("startup-home-check",false)){watching=true;deadline=EditorApplication.timeSinceStartup+45;Canvas.willRenderCanvases+=BeforeRender;}}
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    static void BeforeRender(){
        if(firstCanvas||!Application.isPlaying)return;
        firstCanvas=true;
        try{Check(UnityEngine.Object.FindObjectOfType<Match3DHomeView>(),"First canvas render has no home");}
        catch(Exception e){Finish(e);}
    }
    static void Tick(){
        if(!watching){
            if(!File.Exists("Temp/check-startup-home")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            File.Delete("Temp/check-startup-home");SessionState.SetBool("startup-home-check",true);watching=true;firstCanvas=false;step=0;
            deadline=EditorApplication.timeSinceStartup+45;
            Canvas.willRenderCanvases+=BeforeRender;
            EditorApplication.isPaused=false;EditorApplication.isPlaying=true;return;
        }
        try{
            if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Startup timed out");
            if(!Application.isPlaying)return;
            var flow=Match3DFlow.Instance;
            if(!flow||flow.ScreenName!="home")return;
            if(step++==0)return; // Allow the first rendered frame and deferred destruction.
            Check(firstCanvas,"No canvas rendered");
            Check(UnityEngine.Object.FindObjectsOfType<Match3DHomeView>().Length==1,"Duplicate home");
            var canvas=GameObject.Find("Goose UI");
            Check(canvas&&canvas.GetComponent<CanvasGroup>().interactable,"Home remains blocked");
            Check(!GameObject.Find("Startup Home Preview"),"Editor preview remains active in Play Mode");
            flow.Begin(0);BoardCreationTestSupport.FinishPending(flow);
            Check(flow.ScreenName=="game"&&LevelGenerator.Instance.objectsOnScene.Count>0,"Gameplay did not start");
            flow.Home();Check(flow.ScreenName=="home","Return home failed");
            Finish(null);
        }catch(Exception e){Finish(e);}
    }
    static void Finish(Exception error){
        watching=false;SessionState.SetBool("startup-home-check",false);Canvas.willRenderCanvases-=BeforeRender;
        File.WriteAllText("Temp/startup-home-validation.txt",error==null?"PASS first rendered canvas contains home; one active home; input unlocked; preview hidden; tutorial starts; returns home.":"FAIL "+error);
        EditorApplication.isPlaying=false;
    }
}
