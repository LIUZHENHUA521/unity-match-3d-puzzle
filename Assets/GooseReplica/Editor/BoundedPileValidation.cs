using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug=UnityEngine.Debug;

public static class BoundedPileValidation {
    public static void RunGameplay(){
        string result="Temp/goose-validation.txt";
        if(File.Exists(result))File.Delete(result);
        ReplicaValidation.Run();
        EditorApplication.update+=()=>{
            if(!File.Exists(result))return;
            string report=File.ReadAllText(result);
            Directory.CreateDirectory("Builds");File.WriteAllText("Builds/bounded-gameplay-validation.txt",report);
            EditorApplication.Exit(report.Contains("ALL CHECKS PASSED")?0:1);
        };
    }
    static void Check(bool test,string message){if(!test)throw new Exception(message);}
    public static void Run(){
        try{
            Physics.autoSimulation=false;
            EditorSceneManager.OpenScene("Assets/Scenes/GooseReplica.unity");
            var library=AssetDatabase.LoadAssetAtPath<ReplicaTheme>("Assets/GooseReplica/Resources/Replica/ThemeLibrary.asset");
            var manager=new GameObject("Physics validation").AddComponent<ReplicaPilePhysics>();manager.SendMessage("Awake");
            string report="Bounded physics validation; desktop CPU measurements, not iOS FPS.\n";
            foreach(var theme in library.themes){
                var random=new System.Random(123);var bodies=new List<Rigidbody>();int last=-1,local=0;
                foreach(var tile in ChallengePlan.Generate(123,theme.prefabs.Length,theme.layers)){
                    if(tile.layer!=last){last=tile.layer;local=0;}
                    var pos=new Vector3((local%3-1)*.48f,tile.height+(local/9)*.32f,((local/3)%3-1)*.48f+.15f);local++;
                    var obj=UnityEngine.Object.Instantiate(theme.prefabs[tile.type],pos,Quaternion.Euler(random.Next(-10,11),random.Next(360),random.Next(-10,11)));
                    obj.transform.localScale*=ReplicaFlow.BoardItemScale;
                    var body=obj.GetComponent<Rigidbody>();bodies.Add(body);manager.Register(body);
                }
                Physics.SyncTransforms();int peak=0;var watch=Stopwatch.StartNew();int ticks=Settle(manager,ref peak);watch.Stop();
                Check(bodies.Count==144,"count changed");Check(bodies.All(b=>b.isKinematic||b.IsSleeping()),"idle body remains dynamic");
                Check(bodies.All(b=>b.position.y>-.5f&&new Vector2(b.position.x,b.position.z-.15f).magnitude<1.65f),"body escaped initial pile");
                var input=UnityEngine.Object.FindObjectOfType<InputController>();
                Check(bodies.Any(b=>input.PickAt(Camera.main.WorldToScreenPoint(b.position))!=null),"frozen pile cannot be picked");
                var selected=bodies.OrderBy(b=>b.position.y).ElementAt(bodies.Count/2);
                manager.RemoveForPick(selected);selected.detectCollisions=false;
                Check(!manager.Contains(selected)&&manager.BoardCount==143,"picked item still managed");
                Check(manager.IsSettling,"removal did not wake neighbours");Settle(manager,ref peak);
                selected.position=new Vector3(0,4,.15f);selected.detectCollisions=true;manager.Register(selected);Physics.SyncTransforms();Settle(manager,ref peak);
                Check(manager.BoardCount==144&&(selected.isKinematic||selected.IsSleeping()),"undo failed to rejoin pile");
                manager.ShakeAll();Check(manager.IsSettling,"shake did not wake pile");Settle(manager,ref peak);
                Check(bodies.All(b=>b.position.y>-.5f),"body escaped after shake");
                report+=$"{theme.id}: count=144 peakActive={peak} settleSeconds={ticks*.02f:F2} CPUms={watch.ElapsedMilliseconds}; pick/wake/undo/shake PASS\n";
                manager.Clear();foreach(var body in bodies)UnityEngine.Object.DestroyImmediate(body.gameObject);
            }
            Directory.CreateDirectory("Builds");File.WriteAllText("Builds/bounded-pile-validation.txt",report);Debug.Log(report);EditorApplication.Exit(0);
        }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
    }
    static int Settle(ReplicaPilePhysics manager,ref int peak){
        int ticks=0;
        while(manager.IsSettling&&ticks<1600){manager.Step(.02f);peak=Math.Max(peak,manager.ActiveCount);Physics.Simulate(.02f);ticks++;}
        Check(!manager.IsSettling,"pile never settled");return ticks;
    }
}
