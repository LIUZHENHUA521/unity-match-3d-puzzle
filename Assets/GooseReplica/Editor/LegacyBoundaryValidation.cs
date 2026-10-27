using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Dedicated play-mode regression: no rendering or package re-import required.
[InitializeOnLoad]
public static class LegacyBoundaryValidation {
    static LegacyBoundaryValidation(){EditorApplication.playModeStateChanged+=Changed;}
    public static void Run(){
        SessionState.SetBool("legacy-boundary-test",true);
        EditorSceneManager.OpenScene("Assets/Scenes/GooseReplica.unity");
        EditorApplication.isPlaying=true;
    }
    static void Changed(PlayModeStateChange state){
        if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("legacy-boundary-test",false))
            EditorApplication.delayCall+=Test;
    }
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static void Test(){
        string result="";int exit=0;
        try {
            var pot=GameObject.Find("Cooking Pot");
            var colliders=pot.GetComponentsInChildren<Collider>(true);
            Check(colliders.Count(c=>c.name=="Pot containment")==40,"fixture must contain 40 legacy walls");
            var original=colliders.Select(c=>c.enabled).ToArray();
            var host=new GameObject("Boundary lifecycle probe");
            var presentation=host.AddComponent<ReplicaThemePresentation>();
            var pile=host.AddComponent<ReplicaLayeredPile>();
            for(int theme=0;theme<3;theme++){
                string name=new[]{"CountrysideStove","GroceryStore","Barbecue"}[theme];
                // Preserve a disabled collider as well as enabled ones on exit.
                colliders[0].enabled=false;
                var before=colliders.Select(c=>c.enabled).ToArray();
                ReplicaSourceEnvironment.Select(name);
                presentation.Begin(name);
                Check(colliders.All(c=>!c.enabled),name+": tutorial disables legacy collision");
                var boundary=ReplicaSourceEnvironment.CreateBoundary();
                Check(boundary.GetComponentsInChildren<Collider>().Any(c=>c.enabled),"source boundary remains enabled");
                // Exact ownership transfer used by NextStage; no End on presentation.
                pile.Begin(false,presentation,boundary);
                Check(colliders.All(c=>!c.enabled),"transition cannot reactivate old walls");
                pile.Clear();
                Check(colliders.Select(c=>c.enabled).SequenceEqual(before),"exit restores mixed original collider states");
                pile.Clear();presentation.End();
                Check(colliders.Select(c=>c.enabled).SequenceEqual(before),"repeated cleanup is idempotent");
                presentation.Begin(name);presentation.Begin(name);
                Check(colliders.All(c=>!c.enabled),"restart keeps old walls disabled");
                presentation.End();
                Check(colliders.Select(c=>c.enabled).SequenceEqual(before),"restart cleanup restores states");
                result+="PASS "+name+": "+colliders.Length+" legacy colliders suppressed; source boundary active; stage transfer, restart and cleanup preserve states.\n";
            }
            for(int i=0;i<colliders.Length;i++)colliders[i].enabled=original[i];
            UnityEngine.Object.Destroy(host);
        }catch(Exception e){result+="FAIL "+e;exit=1;}
        Directory.CreateDirectory("Tools/LayeredPile");
        File.WriteAllText("Tools/LayeredPile/legacy-boundary-validation.txt",result);
        Debug.Log(result);SessionState.SetBool("legacy-boundary-test",false);EditorApplication.Exit(exit);
    }
}
