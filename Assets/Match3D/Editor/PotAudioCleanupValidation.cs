using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PotAudioCleanupValidation {
    static double deadline;
    static bool started;
    const string Output="work/pot-audio-cleanup/";
    public static void Run(){
        Directory.CreateDirectory(Output);
        if(File.Exists("Temp/goose-source-audio-frames-validation.txt"))File.Delete("Temp/goose-source-audio-frames-validation.txt");
        EditorSettings.enterPlayModeOptionsEnabled=true;
        EditorSettings.enterPlayModeOptions=EnterPlayModeOptions.DisableDomainReload;
        EditorSceneManager.OpenScene("Assets/Scenes/Match3D.unity");
        deadline=EditorApplication.timeSinceStartup+150;
        EditorApplication.update+=Tick;EditorApplication.isPlaying=true;
    }
    static void Tick(){
        if(EditorApplication.timeSinceStartup>deadline){Finish("FAIL audio validation timed out");return;}
        if(started||!EditorApplication.isPlaying||!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)return;
        try{
            File.WriteAllText(Output+"assets.txt",BoardAudioValidation.Run());
            started=true;new GameObject("Pot audio validation runner").AddComponent<PotAudioValidationRunner>().StartCoroutine(Validate());
        }catch(Exception error){Finish("FAIL "+error);}
    }
    static IEnumerator Validate(){
        yield return BoardAudioFramesValidation.Run();
        // Read the validation report on the editor update after cleanup completes.
    }
    [InitializeOnLoadMethod] static void RegisterCompletion(){EditorApplication.update+=Completion;}
    static void Completion(){
        if(!started)return;
        const string result="Temp/goose-source-audio-frames-validation.txt";
        if(File.Exists(result))Finish(File.ReadAllText(result));
    }
    static void Finish(string report){
        started=false;EditorApplication.update-=Tick;EditorApplication.update-=Completion;
        File.WriteAllText(Output+"runtime.txt",report);Debug.Log(report);EditorApplication.Exit(report.StartsWith("PASS")?0:1);
    }
}

public sealed class PotAudioValidationRunner : MonoBehaviour {}
