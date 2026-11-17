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
    static IEnumerator Guard(){bool hadMode=EditorPrefs.HasKey(Match3DFlow.PotEditorSourcePreference);int oldMode=Match3DFlow.PotEditorSource;bool oldSource=EditorPrefs.GetBool(Match3DFlow.PotEditorConfigurationPreference,false);string oldTheme=EditorPrefs.GetString(Match3DFlow.EditorThemePreference,"");var core=Core();string error=null;float end=Time.realtimeSinceStartup+900;while(true){bool next=false;object current=null;try{if(Time.realtimeSinceStartup>end)throw new Exception("timeout");next=core.MoveNext();if(next)current=core.Current;}catch(Exception e){error=e.ToString();}if(error!=null||!next)break;yield return current;}if(hadMode)EditorPrefs.SetInt(Match3DFlow.PotEditorSourcePreference,oldMode);else EditorPrefs.DeleteKey(Match3DFlow.PotEditorSourcePreference);EditorPrefs.SetBool(Match3DFlow.PotEditorConfigurationPreference,oldSource);EditorPrefs.SetString(Match3DFlow.EditorThemePreference,oldTheme);File.AppendAllText(Output,error==null?"COMPLETE\n":"FAIL "+error+"\n");SessionState.SetBool(Key,false);EditorApplication.Exit(error==null?0:1);}
    static IEnumerator Core(){
        while(!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)yield return null;
        var flow=Match3DFlow.Instance;
        EditorPrefs.DeleteKey(Match3DFlow.PotEditorConfigurationPreference);
        EditorPrefs.DeleteKey(Match3DFlow.PotEditorSourcePreference);
        Check(Match3DFlow.PotEditorSource==2,"new editors default to independent config");
        EditorPrefs.SetBool(Match3DFlow.PotEditorConfigurationPreference,true);
        Check(Match3DFlow.PotEditorSource==2,"legacy local flag migrates to backend editor config");
        EditorPrefs.SetBool(Match3DFlow.PotEditorConfigurationPreference,false);
        Check(Match3DFlow.PotEditorSource==2,"legacy beta flag migrates to backend editor config");
        EditorPrefs.SetInt(Match3DFlow.PotEditorSourcePreference,1);
        Check(Match3DFlow.PotEditorSource==1,"explicit current local selection is preserved");
        EditorPrefs.SetInt(Match3DFlow.PotEditorSourcePreference,0);
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
        EditorPrefs.SetInt(Match3DFlow.PotEditorSourcePreference,1);
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
        EditorPrefs.SetInt(Match3DFlow.PotEditorSourcePreference,2);
        File.WriteAllText(PotEditorBetaLevels.CredentialsPath,"{\"key\":\"fixture-preview-key\"}");
        int reads=0;
        foreach(var entry in Resources.Load<Match3DModelThemes>("Match3D/ModelThemes").themes){
          foreach(var tier in new[]{"bachelor","elite"}){
            string label="independent "+entry.id+" / "+tier;
            EditorPrefs.SetString(Match3DFlow.EditorThemePreference,entry.id);
            flow.Home();flow.BeginReward(0,tier);flow.BeginReward(0,tier);
            while(!flow.LocalPreview||flow.CreationInProgress)yield return null;
            int expected=++reads%2==1?42:48;
            var profile=Match3DModelThemes.Get(entry.id,flow.SourceEnvironmentForTheme(0));
            var snapshot=(LevelLayoutConfig.Profile)typeof(Match3DFlow).GetField("potEditorRunProfile",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(flow);
            Check(snapshot!=null&&snapshot.layers.Sum(l=>l.count)==expected,"fresh config and duplicate click guard "+label);
            Check(snapshot.layers[0].height==14.5f&&snapshot.layers[1].height==19.5f,"backend heights override local heights "+label);
            Check(snapshot!=profile&&snapshot.layers!=profile.layers&&snapshot.seconds==profile.seconds,"local profile preserved "+label);
            Check(flow.ActiveModelTheme==entry.id&&LevelGenerator.Instance.objectsOnScene.Count==15,"local theme and tutorial "+label);
            Check(typeof(Match3DFlow).GetField("dailyLevel",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(flow)==null,"no daily snapshot "+label);
            EditorPrefs.SetString(Match3DFlow.EditorThemePreference,entry.id=="playful-toys"?"tasty-kitchen":"playful-toys");
            flow.NextStage();while(flow.CreationInProgress)yield return null;
            Check(flow.ActiveModelTheme==entry.id&&LevelGenerator.Instance.objectsOnScene.Count==expected,"second stage freezes theme and counts "+label);
            Check(GameManager.Instance.currentLevelTime>profile.seconds-5&&GameManager.Instance.currentLevelTime<=profile.seconds,"local timer "+label);
            flow.RestartStage();while(flow.CreationInProgress)yield return null;
            Check(flow.ActiveModelTheme==entry.id&&LevelGenerator.Instance.objectsOnScene.Count==expected&&flow.UnlimitedTools,"retry preserves config "+label);
            Check(snapshot.layers[0].height==14.5f&&snapshot.layers[1].height==19.5f,"retry preserves backend heights "+label);
            var extra=PotEditorLevelConfig.BuildProfile(profile,new PotEditorLevelConfig.Config{layers=Enumerable.Range(0,profile.layers.Length+2).Select(_=>new PotEditorLevelConfig.Layer{count=12,types=2}).ToArray()});
            Check(extra.layers.Take(profile.layers.Length).Select(l=>l.height).SequenceEqual(profile.layers.Select(l=>l.height))&&extra.layers.Last().height>extra.layers[extra.layers.Length-2].height,"additional layer heights "+label);
            Check(extra.layers.Last().weights.Length==0&&extra.layers.Take(profile.layers.Length).Select((l,i)=>l.weights.SequenceEqual(profile.layers[i].weights??Array.Empty<float>())).All(x=>x),"preserve local weights and new equal weights "+label);
          }
        }
        flow.Home();File.WriteAllText(PotEditorBetaLevels.CredentialsPath,"{}");flow.BeginReward(0,"bachelor");yield return null;
        Check(!flow.LocalPreview&&flow.ScreenName=="home","independent missing key never falls back");
        EditorPrefs.SetInt(Match3DFlow.PotEditorSourcePreference,0);
        EditorPrefs.SetBool(Match3DFlow.PotEditorConfigurationPreference,false);
        flow.Home();File.WriteAllText(PotEditorBetaLevels.CredentialsPath,"{}");flow.BeginReward(0,"bachelor");yield return null;
        Check(!flow.LocalPreview&&flow.ScreenName=="home","missing key never falls back locally");
    }
}
