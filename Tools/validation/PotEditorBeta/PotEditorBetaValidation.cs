using System;
using System.Collections;
using System.IO;
using System.Linq;
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
    static IEnumerator Guard(){bool oldSource=EditorPrefs.GetBool(Match3DFlow.PotEditorConfigurationPreference,false);string oldTheme=EditorPrefs.GetString(Match3DFlow.EditorThemePreference,"");var core=Core();string error=null;float end=Time.realtimeSinceStartup+240;while(true){bool next=false;object current=null;try{if(Time.realtimeSinceStartup>end)throw new Exception("timeout");next=core.MoveNext();if(next)current=core.Current;}catch(Exception e){error=e.ToString();}if(error!=null||!next)break;yield return current;}EditorPrefs.SetBool(Match3DFlow.PotEditorConfigurationPreference,oldSource);EditorPrefs.SetString(Match3DFlow.EditorThemePreference,oldTheme);File.AppendAllText(Output,error==null?"COMPLETE\n":"FAIL "+error+"\n");SessionState.SetBool(Key,false);EditorApplication.Exit(error==null?0:1);}
    static IEnumerator Core(){
        while(!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)yield return null;
        var flow=Match3DFlow.Instance;
        EditorPrefs.DeleteKey(Match3DFlow.PotEditorConfigurationPreference);
        Check(!Match3DFlow.PotUseEditorConfiguration,"default remains beta configuration");
        EditorPrefs.SetString(Match3DFlow.EditorThemePreference,"playful-toys");
        typeof(Match3DFlow).GetField("endpoint",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(flow,"http://127.0.0.1:18097");
        foreach(var tier in new[]{"bachelor","elite"}){
            flow.Home();flow.BeginReward(0,tier);flow.BeginReward(0,tier);
            while(!flow.LocalPreview||flow.CreationInProgress)yield return null;
            var level=(Match3DDailyLevel)typeof(Match3DFlow).GetField("dailyLevel",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(flow);
            int expected=tier=="bachelor"?30:12;
            Check(level!=null&&level.initialCount==expected&&level.tier==tier,"fresh beta config "+tier);
            Check(flow.ActiveModelTheme==level.modelThemeId&&flow.ActiveModelTheme!="playful-toys","beta ignores editor theme "+tier);
            Check(flow.LocalPreview&&flow.UnlimitedTools,"preview and free tools "+tier);
            Check(flow.Stage==1&&LevelGenerator.Instance.objectsOnScene.Count==15,"tutorial preserved "+tier);
            flow.NextStage();while(flow.CreationInProgress)yield return null;
            Check(LevelGenerator.Instance.objectsOnScene.Count==expected,"second stage count "+tier);
            Check(GameManager.Instance.currentLevelTime>level.seconds-5&&GameManager.Instance.currentLevelTime<=level.seconds,"server time limit "+tier);
            flow.RestartStage();while(flow.CreationInProgress)yield return null;
            Check(LevelGenerator.Instance.objectsOnScene.Count==expected,"retry keeps loaded level "+tier);
        }
        EditorPrefs.SetBool(Match3DFlow.PotEditorConfigurationPreference,true);
        File.WriteAllText(PotEditorBetaLevels.CredentialsPath,"{}");
        foreach(var entry in Resources.Load<Match3DModelThemes>("Match3D/ModelThemes").themes){
          foreach(var tier in new[]{"bachelor","elite"}){
            string id=entry.id;
            string label=id+" / "+tier;
            EditorPrefs.SetString(Match3DFlow.EditorThemePreference,id);
            flow.Home();flow.BeginReward(0,tier);
            while(flow.CreationInProgress)yield return null;
            var profile=Match3DModelThemes.Get(id,flow.SourceEnvironmentForTheme(0));
            Check(profile.layers.All(l=>l.weights==null||l.weights.Length==0||(l.weights.Length==profile.prefabs.Length&&l.weights.All(w=>w>0&&!float.IsNaN(w)&&!float.IsInfinity(w)))),"weights match model pool "+label);
            var names=profile.prefabs.Select(p=>p.name).ToArray();
            Check(flow.LocalPreview&&flow.UnlimitedTools&&flow.ActiveModelTheme==id,"local configuration without credentials "+label);
            Check(typeof(Match3DFlow).GetField("dailyLevel",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(flow)==null,"local configuration clears beta snapshot "+label);
            Check(flow.Stage==1&&LevelGenerator.Instance.objectsOnScene.Count==15&&LevelGenerator.Instance.objectsOnScene.All(o=>names.Contains(o.GetComponent<BoardItemIdentity>().LogicalId)),"local tutorial theme "+label);
            flow.NextStage();while(flow.CreationInProgress)yield return null;
            Check(LevelGenerator.Instance.objectsOnScene.Count>0&&LevelGenerator.Instance.objectsOnScene.All(o=>names.Contains(o.GetComponent<BoardItemIdentity>().LogicalId)),"local second-stage theme "+label);
            Check(GameManager.Instance.currentLevelTime>profile.seconds-5&&GameManager.Instance.currentLevelTime<=profile.seconds,"local time limit "+label);
            flow.RestartStage();while(flow.CreationInProgress)yield return null;
            Check(flow.ActiveModelTheme==id&&flow.LocalPreview&&flow.UnlimitedTools,"local retry preserves configuration "+label);
          }
        }
        EditorPrefs.SetBool(Match3DFlow.PotEditorConfigurationPreference,false);
        flow.Home();File.WriteAllText(PotEditorBetaLevels.CredentialsPath,"{}");flow.BeginReward(0,"bachelor");yield return null;
        Check(!flow.LocalPreview&&flow.ScreenName=="home","missing key never falls back locally");
    }
}
