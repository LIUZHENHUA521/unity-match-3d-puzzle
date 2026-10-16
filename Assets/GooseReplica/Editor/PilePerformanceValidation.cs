using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug=UnityEngine.Debug;

public static class PilePerformanceValidation {
    public static void CheckFrostLifecycle() {
        try {
            EditorSceneManager.OpenScene("Assets/Scenes/GooseReplica.unity");
            var effect=UnityEngine.Object.FindObjectOfType<FrostEffect>();
            if(!effect)throw new Exception("Frost effect missing");
            effect.SendMessage("Awake");
            FrostEffect.FrostAmount=0;
            if(effect.enabled)throw new Exception("Idle post effect remains enabled");
            FrostEffect.FrostAmount=.5f;
            if(!effect.enabled)throw new Exception("Freeze cannot enable effect");
            FrostEffect.FrostAmount=0;
            if(effect.enabled)throw new Exception("Reset did not disable effect");
            Directory.CreateDirectory("Builds");
            File.WriteAllText("Builds/frost-validation.txt","PASS: idle disabled; freeze enables effect; reset disables effect\n");
            EditorApplication.Exit(0);
        }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
    }
    public static void Run() {
        bool auto=Physics.autoSimulation;
        try {
            Physics.autoSimulation=false;
            EditorSceneManager.OpenScene("Assets/Scenes/GooseReplica.unity");
            var library=AssetDatabase.LoadAssetAtPath<ReplicaTheme>("Assets/GooseReplica/Resources/Replica/ThemeLibrary.asset");
            string report="Editor CPU comparison of a REJECTED 30 Hz physics candidate; shipping physics remains unchanged. Not an iOS FPS measurement.\n";
            foreach(var theme in library.themes)foreach(bool optimized in new[]{false,true}) {
                var bodies=new List<Rigidbody>();var random=new System.Random(123);
                var tiles=ChallengePlan.Generate(123,theme.prefabs.Length,theme.layers);
                int last=-1,local=0;
                foreach(var tile in tiles){
                    if(tile.layer!=last){local=0;last=tile.layer;}
                    float x=(local%3-1)*.48f,z=((local/3)%3-1)*.48f+.15f;
                    float y=tile.height+(local/9)*.32f;local++;
                    var obj=UnityEngine.Object.Instantiate(theme.prefabs[tile.type],new Vector3(x,y,z),Quaternion.Euler(random.Next(-10,11),random.Next(360),random.Next(-10,11)));
                    obj.transform.localScale*=ReplicaFlow.BoardItemScale;
                    var body=obj.GetComponent<Rigidbody>();body.useGravity=true;
                    if(optimized){body.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;body.sleepThreshold=.02f;}
                    bodies.Add(body);
                }
                Physics.SyncTransforms();float dt=optimized?1f/30:1f/50;int steps=Mathf.RoundToInt(20/dt);
                var watch=Stopwatch.StartNew();for(int i=0;i<steps;i++)Physics.Simulate(dt);watch.Stop();
                int escaped=bodies.Count(b=>b.position.y<-.5f||new Vector2(b.position.x,b.position.z-.15f).magnitude>1.65f);
                report+=theme.id+" rejectedCandidate="+optimized+" count="+bodies.Count+" simulateMs="+watch.ElapsedMilliseconds+" awake="+bodies.Count(b=>!b.IsSleeping())+" escaped="+escaped+"\n";
                if(optimized&&escaped>0)throw new Exception(report+"Containment regression");
                if(optimized){
                    foreach(var b in bodies){b.AddForce(Vector3.up*1.9f,ForceMode.VelocityChange);b.AddTorque(Vector3.one,ForceMode.VelocityChange);}
                    if(!bodies.Any(b=>!b.IsSleeping()))throw new Exception("Shake did not wake pile");
                    for(int i=0;i<300;i++)Physics.Simulate(dt);
                    if(bodies.Any(b=>b.position.y<-.5f))throw new Exception("Shake containment failed");
                }
                foreach(var b in bodies)UnityEngine.Object.DestroyImmediate(b.gameObject);
            }
            Directory.CreateDirectory("Builds");File.WriteAllText("Builds/pile-performance-validation.txt",report+"PASS containment and shake wake-up\n");Debug.Log(report);Physics.autoSimulation=auto;EditorApplication.Exit(0);
        }catch(Exception e){Physics.autoSimulation=auto;Debug.LogException(e);EditorApplication.Exit(1);}
    }
}
