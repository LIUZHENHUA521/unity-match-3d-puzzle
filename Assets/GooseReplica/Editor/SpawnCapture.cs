using UnityEditor;
using UnityEngine;
using System.IO;
public static class SpawnCapture {
    static readonly float[] Times={.05f,.3f,.8f,1.5f,3.8f};
    static int frame;static float start;static string folder;
    public static void Run(){
        folder="/Users/liuzhenhua/Documents/Codex/2026-10-13/r/outputs/goose-replica/spawn-fix";
        Directory.CreateDirectory(folder);frame=0;
        ReplicaFlow.Instance.Begin(0);ReplicaFlow.Instance.NextStage();start=Time.time;
        EditorApplication.update-=Tick;EditorApplication.update+=Tick;
    }
    static void Tick(){
        if(!EditorApplication.isPlaying){EditorApplication.update-=Tick;return;}
        if(Time.time-start<Times[frame])return;
        ScreenCapture.CaptureScreenshot(folder+"/frame-"+frame+".png");frame++;
        if(frame==Times.Length)EditorApplication.update-=Tick;
    }
}
