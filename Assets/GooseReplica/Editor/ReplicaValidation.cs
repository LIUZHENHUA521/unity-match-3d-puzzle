using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class ReplicaValidation {
    static int step;static double due;static string result="";static int previousCollection;static bool finishing;static bool oldOptionsEnabled;static EnterPlayModeOptions oldOptions;static string oldCompleted;static double deadline;
    static void Require(bool yes,string text){if(!yes)throw new Exception(text);result+="PASS "+text+"\n";}
    public static void Run(){
        result="";CheckOriginalModels();
        var layers=new[]{new ChallengeLayer(60,8,.65f),new ChallengeLayer(36,6,2.9f),new ChallengeLayer(24,4,4.3f),new ChallengeLayer(24,3,5.3f)};
        layers[0].weights=new float[0]; // Unity serializes an unset array as empty.
        var a=ChallengePlan.Generate(123,8,layers);var b=ChallengePlan.Generate(123,8,layers);
        Require(a.Count==144,"144 hard-stage items");Require(a.Select(x=>x.type).SequenceEqual(b.Select(x=>x.type)),"seed repeatability");
        Require(a.GroupBy(x=>x.type).All(g=>g.Count()%3==0),"global triples");
        Require(a.GroupBy(x=>x.layer).Select(g=>g.Count()).SequenceEqual(new[]{60,36,24,24}),"layer capacity preserved");
        bool rejected=false;try{ChallengePlan.Generate(1,8,new[]{new ChallengeLayer(13,2,1)});}catch(ArgumentException){rejected=true;}Require(rejected,"reject invalid layer count");
        var boundary=new DateTime(2026,10,12,16,0,0,DateTimeKind.Utc);Require(ChallengePlan.DailySeed("daily",1,boundary)!=ChallengePlan.DailySeed("daily",1,boundary.AddSeconds(-1)),"Shanghai midnight changes seed");
        if(EditorApplication.isPlaying)throw new Exception("Stop play before validation");
        oldOptionsEnabled=EditorSettings.enterPlayModeOptionsEnabled;oldOptions=EditorSettings.enterPlayModeOptions;EditorSettings.enterPlayModeOptionsEnabled=true;EditorSettings.enterPlayModeOptions=EnterPlayModeOptions.DisableDomainReload;oldCompleted=PlayerPrefs.GetString("goose.completed.daily","");deadline=EditorApplication.timeSinceStartup+180;previousCollection=PlayerPrefs.GetInt("goose.collection.daily",0);step=0;finishing=false;EditorSceneManager.OpenScene(ReplicaBuilder.Scene);EditorApplication.isPlaying=true;due=EditorApplication.timeSinceStartup+5;EditorApplication.update+=Tick;
    }
    static void CheckOriginalModels(){
        var library=AssetDatabase.LoadAssetAtPath<ReplicaTheme>("Assets/GooseReplica/Resources/Replica/ThemeLibrary.asset");
        Require(library.themes.Select(t=>t.prefabs.Length).SequenceEqual(new[]{21,18,25}),"original model pools 21/18/25");
        foreach(var prefab in library.themes.SelectMany(t=>t.prefabs)){
            Require(prefab&&prefab.CompareTag("Collectable")&&prefab.GetComponent<Rigidbody>(),"collectable body: "+prefab.name);
            Require(prefab.GetComponentsInChildren<Collider>().Length>0,"colliders: "+prefab.name);
            var rs=prefab.GetComponentsInChildren<MeshRenderer>().Where(r=>r.enabled).ToArray();
            Require(rs.Length>0&&rs.All(r=>r.sharedMaterials.Length>0&&r.sharedMaterials.All(m=>m&&m.shader&&m.shader.isSupported)),"meshes/materials: "+prefab.name);
        }
    }
    static void Later(double seconds){due=EditorApplication.timeSinceStartup+seconds;step++;}
    static void Tick(){
        if(EditorApplication.timeSinceStartup<due||!EditorApplication.isPlaying)return;
        try{
            if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Validation timed out at step "+step);var flow=ReplicaFlow.Instance;var generator=LevelGenerator.Instance;var input=InputController.Instance;
            if(step==0&&(!flow||string.IsNullOrEmpty(flow.ScreenName)))return;
            switch(step){
                case 0:Require(flow&&flow.ScreenName=="home","home launch");flow.ShowThemes();Require(flow.ScreenName=="themes","theme navigation");flow.Home();flow.Begin(0);Require(generator.objectsOnScene.Count==12&&!GameManager.Instance.GameIsPlaying(),"whole source tutorial board spawns before play");Require(GameManager.Instance.currentLevelTime==0,"source tutorial has no countdown");Require(ReplicaSourceEnvironment.CurrentName=="CountrysideStove","source tutorial selects first-level scene");Later(4);break;
                case 1:Require(generator.objectsOnScene.Count==12,"tutorial has 12 items");Require(generator.objectsOnScene.All(o=>!o.GetComponent<GooseSpawnMotion>()&&o.GetComponent<Rigidbody>().useGravity),"ordinary gravity restored without centre attraction");Require(generator.objectsOnScene.GroupBy(x=>x.name).Count()==2,"tutorial has two types");Require(GameManager.Instance.GameIsPlaying(),"tutorial starts after settling");Require(generator.objectsOnScene.All(o=>o.transform.position.y>-.5f),"tutorial bodies stay above floor");Time.timeScale=8;Later(.1);break;
                case 2:
                    if(generator.objectsOnScene.Count>0){if(!input.IsBusy){foreach(var obj in generator.objectsOnScene.GroupBy(x=>x.name).First().Take(3).ToArray())input.AddObjectOnStack(obj);}due=EditorApplication.timeSinceStartup+.15;return;}
                    Time.timeScale=1;Later(.5);break;
                case 3:Require(flow.LastResult=="第一关通过！","tutorial result");flow.NextStage();Later(4);break;
                case 4:Require(flow.Stage==2&&generator.objectsOnScene.Count==300,"second-stage transition");Require(generator.objectsOnScene.All(o=>o.transform.position.y>-.5f),"hard-stage bodies stay above floor");var groups=generator.objectsOnScene.GroupBy(x=>x.name).Take(2).ToArray();input.AddObjectOnStack(groups[0].First());input.AddObjectOnStack(groups[1].First());input.AddObjectOnStack(groups[0].Skip(1).First());Later(1);break;
                case 5:input.UndoLast();Later(.7);break;
                case 6:Require(input.objectsOnStack.Count==2&&input.objectsOnStack.Select(x=>x.name).Distinct().Count()==2,"chronological undo after visual grouping");flow.RestartStage();Later(4);break;
                case 7:foreach(var obj in generator.objectsOnScene.GroupBy(x=>x.name).Take(7).Select(g=>g.First()).ToArray())input.AddObjectOnStack(obj);Later(1);break;
                case 8:Require(GameManager.Instance.gameStatus==GameManager.GameStatus.FAIL,"seven different items fail");flow.RestartStage();Later(4);break;
                case 9:Time.timeScale=8;Later(.1);break;
                case 10:
                    if(generator.objectsOnScene.Count>0){if(!input.IsBusy){foreach(var obj in generator.objectsOnScene.GroupBy(x=>x.name).First().Take(3).ToArray())input.AddObjectOnStack(obj);}due=EditorApplication.timeSinceStartup+.15;return;}
                    Time.timeScale=1;Later(.5);break;
                case 11:Require(flow.HasGoose("daily"),"hard-stage collection unlock");flow.Home();flow.ShowCollection();Require(flow.ScreenName=="collection","collection navigation");flow.Begin(1);flow.Home();Later(4);break;
                case 12:Require(generator.objectsOnScene.Count==0&&!GameManager.Instance.GameIsPlaying(),"home cancels pending start");Finish(null);break;
            }
        }catch(Exception e){Finish(e);}
    }
    static void Finish(Exception e){if(finishing)return;finishing=true;EditorApplication.update-=Tick;Time.timeScale=1;PlayerPrefs.SetInt("goose.collection.daily",previousCollection);PlayerPrefs.SetString("goose.completed.daily",oldCompleted);PlayerPrefs.Save();EditorSettings.enterPlayModeOptionsEnabled=oldOptionsEnabled;EditorSettings.enterPlayModeOptions=oldOptions;result+=e==null?"ALL CHECKS PASSED\n":"FAIL "+e+"\n";File.WriteAllText("Temp/goose-validation.txt",result);Debug.Log(result);EditorApplication.isPlaying=false;}
}
