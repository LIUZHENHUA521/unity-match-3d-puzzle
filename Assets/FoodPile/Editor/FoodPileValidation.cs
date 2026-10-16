using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class FoodPileValidation
{
    const string Key="FoodPileValidation.Active";
    static double deadline;
    static int stage, matched;
    static bool shaking;
    static int isolation;
    static GameObject picked;
    static System.Collections.Generic.Dictionary<GameObject,Vector3> neighbours;
    static FoodPileValidation() { EditorApplication.update += Tick; }
    public static void Run() {
        EditorSceneManager.OpenScene(FoodPileBuilder.ScenePath);
        SessionState.SetBool(Key,true);
        EditorApplication.isPlaying=true;
    }
    static void Require(bool condition,string message) { if(!condition) throw new Exception(message); }
    static void Later(double seconds) { deadline=EditorApplication.timeSinceStartup+seconds;stage++; }
    static void Tick() {
        if(!SessionState.GetBool(Key,false) || !EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        try {
            if(stage==0) { Later(6);return; }
            if(EditorApplication.timeSinceStartup<deadline) return;
            var generator=LevelGenerator.Instance; var input=InputController.Instance;
            switch(stage) {
                case 1:
                    Require(generator.objectsOnScene.Count==96,"Expected 96 spawned items");
                    Require(generator.objectsOnScene.Count(x=>x.name.StartsWith("strawberry"))==12,"Expected 12 strawberries");
                    Require(generator.objectsOnScene.GroupBy(x=>x.name).Count()==8,"Expected eight types");
                    Require(generator.objectsOnScene.GroupBy(x=>x.name).All(g=>g.Count()==12),"Expected twelve copies each");
                    Require(generator.objectsOnScene.Where(x=>!x.GetComponent<Rigidbody>().isKinematic).All(x=>Mathf.Abs(x.transform.position.x)<1.5f && x.transform.position.z > -1.6f && x.transform.position.z<1.9f && x.transform.position.y>-.25f),"Pile escaped containment: " + string.Join("; ", generator.objectsOnScene.Where(x=>Mathf.Abs(x.transform.position.x)>=1.5f || x.transform.position.z<=-1.6f || x.transform.position.z>=1.9f || x.transform.position.y<=-.25f).Select(x=>x.name+" "+x.transform.position)));
                    Require(GameManager.Instance.GameIsPlaying(),"Game didn't start after settling");
                    if (isolation == 0) {
                        var screen = Camera.main.WorldToScreenPoint(new Vector3(0,0,.15f));
                        picked = input.PickAt(screen);
                        Require(picked != null, "Pointer ray did not pick a visible food");
                        foreach(var obj in generator.objectsOnScene) obj.GetComponent<Rigidbody>().interpolation=RigidbodyInterpolation.None;
                        Physics.SyncTransforms();
                        neighbours = generator.objectsOnScene.Where(x=>x!=picked).ToDictionary(x=>x,x=>x.transform.position);
                        Physics.autoSimulation=false;
                        input.AddObjectOnStack(picked); input.AddObjectOnStack(picked);
                        Require(input.objectsOnStack.Count==1,"One selection was accepted twice");
                        Require(!picked.GetComponent<Rigidbody>().detectCollisions && picked.GetComponentsInChildren<Collider>().All(c=>!c.enabled),"Picked item still collides with pile");
                        isolation=1;deadline=EditorApplication.timeSinceStartup+.55;return;
                    }
                    if (isolation == 1) {
                        Require(neighbours.All(pair=>Vector3.Distance(pair.Key.transform.position,pair.Value)<.0001f),"Picking transported a neighbour");
                        Require(Vector3.Distance(picked.transform.position,new Vector3(-1,.3f,-1.62f))<.02f,"Picked item did not reach slot");
                        Physics.autoSimulation=true; isolation=2; generator.RestartLevel();deadline=EditorApplication.timeSinceStartup+5;return;
                    }
                    foreach(var item in generator.objectsOnScene.GroupBy(x=>x.name).First().Take(3).ToArray()) input.AddObjectOnStack(item);
                    Later(1);break;
                case 2:
                    Require(generator.objectsOnScene.Count==93 && input.objectsOnStack.Count==0,"Triple removal failed");
                    foreach(var item in generator.objectsOnScene.GroupBy(x=>x.name).Take(2).Select(g=>g.First()).ToArray()) input.AddObjectOnStack(item);
                    Later(.7);break;
                case 3:
                    Require(input.arrivedObject.Count==2,"Slot arrival failed");
                    input.UndoLast();Later(.7);break;
                case 4:
                    Require(input.objectsOnStack.Count==1,"Undo failed");
                    if (!shaking) { shaking = true; UnityEngine.Object.FindObjectOfType<FoodPileShake>().Shake(); deadline = EditorApplication.timeSinceStartup + .06; return; }
                    Require(generator.objectsOnScene.Where(x=>!x.GetComponent<Rigidbody>().isKinematic).Any(x=>x.GetComponent<Rigidbody>().velocity.y>.3f),"Shake impulse failed");
                    generator.RestartLevel();Later(5);break;
                case 5:
                    Require(generator.objectsOnScene.Count==96 && input.objectsOnStack.Count==0,"Restart failed");
                    foreach(var item in generator.objectsOnScene.GroupBy(x=>x.name).Take(7).Select(g=>g.First()).ToArray()) input.AddObjectOnStack(item);
                    // Eighth click must be rejected while the first seven are still travelling.
                    input.AddObjectOnStack(generator.objectsOnScene.GroupBy(x=>x.name).Last().First());
                    Require(input.objectsOnStack.Count==7,"Slot overflow admitted");
                    Later(1);break;
                case 6:
                    Require(GameManager.Instance.gameStatus==GameManager.GameStatus.FAIL && UIManager.Instance.OutOfBoxesPopUp.activeSelf,"Seven-slot failure missing");
                    UIManager.Instance.OnClickRestart();Later(5);break;
                case 7:
                    if(generator.objectsOnScene.Count>0) {
                        Require(GameManager.Instance.GameIsPlaying(),"Game failed during complete solve");
                        Require(!input.IsResolving && input.objectsOnStack.Count==0,"Previous match not resolved");
                        foreach(var item in generator.objectsOnScene.GroupBy(x=>x.name).First().Take(3).ToArray()) input.AddObjectOnStack(item);
                        matched++;deadline=EditorApplication.timeSinceStartup+1.05;return;
                    }
                    Require(matched==32,"Expected 32 triple matches");
                    Require(GameManager.Instance.gameStatus==GameManager.GameStatus.FINISH && UIManager.Instance.NextLevelPopUp.activeSelf,"Win popup missing");
                    SessionState.SetBool(Key,false);
                    File.WriteAllText(Path.GetFullPath("../foodpile-test-results.txt"),"PASS: 96 items; 8 x 12; containment; pointer picking; duplicate click rejection; no neighbour transport; triple matching; undo; shake; restart; overflow guard; seven-slot failure; all 32 triples; victory.\n");
                    Debug.Log("FOOD_PILE_TESTS_PASS");EditorApplication.Exit(0);break;
            }
        } catch(Exception e) {
            SessionState.SetBool(Key,false);Debug.LogException(e);
            File.WriteAllText(Path.GetFullPath("../foodpile-test-results.txt"),"FAIL: "+e);
            EditorApplication.Exit(1);
        }
    }
}
