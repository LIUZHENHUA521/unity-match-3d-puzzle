using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
[InitializeOnLoad]
public static class PotEditorBetaValidation {
    const string Key="pot.editor.beta.validation";
    const string Output="work/pot-editor-beta/checks.txt";
    static PotEditorBetaValidation(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))new GameObject("Pot editor beta validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());};}
    public static void Run(){Directory.CreateDirectory(Path.GetDirectoryName(Output));File.WriteAllText(Output,"");SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Check(bool ok,string label){if(!ok)throw new Exception(label);File.AppendAllText(Output,"PASS "+label+"\n");}
    static IEnumerator Guard(){var core=Core();string error=null;float end=Time.realtimeSinceStartup+120;while(true){bool next=false;object current=null;try{if(Time.realtimeSinceStartup>end)throw new Exception("timeout");next=core.MoveNext();if(next)current=core.Current;}catch(Exception e){error=e.ToString();}if(error!=null||!next)break;yield return current;}File.AppendAllText(Output,error==null?"COMPLETE\n":"FAIL "+error+"\n");SessionState.SetBool(Key,false);EditorApplication.Exit(error==null?0:1);}
    static IEnumerator Core(){
        while(!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)yield return null;
        var flow=Match3DFlow.Instance;
        typeof(Match3DFlow).GetField("endpoint",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(flow,"http://127.0.0.1:18097");
        foreach(var tier in new[]{"bachelor","elite"}){
            flow.Home();flow.BeginReward(0,tier);flow.BeginReward(0,tier);
            while(!flow.LocalPreview||flow.CreationInProgress)yield return null;
            var level=(Match3DDailyLevel)typeof(Match3DFlow).GetField("dailyLevel",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(flow);
            int expected=tier=="bachelor"?30:12;
            Check(level!=null&&level.initialCount==expected&&level.tier==tier,"fresh beta config "+tier);
            Check(flow.LocalPreview&&flow.UnlimitedTools,"preview and free tools "+tier);
            Check(flow.Stage==1&&LevelGenerator.Instance.objectsOnScene.Count==15,"tutorial preserved "+tier);
            flow.NextStage();while(flow.CreationInProgress)yield return null;
            Check(LevelGenerator.Instance.objectsOnScene.Count==expected,"second stage count "+tier);
            Check(GameManager.Instance.currentLevelTime>level.seconds-5&&GameManager.Instance.currentLevelTime<=level.seconds,"server time limit "+tier);
            flow.RestartStage();while(flow.CreationInProgress)yield return null;
            Check(LevelGenerator.Instance.objectsOnScene.Count==expected,"retry keeps loaded level "+tier);
        }
        flow.Home();File.WriteAllText(PotEditorBetaLevels.CredentialsPath,"{}");flow.BeginReward(0,"bachelor");yield return null;
        Check(!flow.LocalPreview&&flow.ScreenName=="home","missing key never falls back locally");
    }
}
