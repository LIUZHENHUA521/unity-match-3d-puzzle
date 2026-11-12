using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Regression for removal of phone, keyboard and legacy-button tossing.
[InitializeOnLoad]
public static class PhoneShakeValidation {
    static PhoneShakeValidation(){EditorApplication.update+=Poll;}
    static void Poll(){
        if(!File.Exists("Temp/run-phone-shake-validation")||EditorApplication.isCompiling||EditorApplication.isUpdating||!EditorApplication.isPlaying||!Match3DFlow.Instance||Match3DFlow.Instance.ScreenName!="home")return;
        File.Delete("Temp/run-phone-shake-validation");
        new GameObject("Pan toss removal validation").AddComponent<BoardValidationDriver>().StartCoroutine(Run());
    }
    static void Check(bool value,string reason){if(!value)throw new Exception(reason);}
    static IEnumerator Run(){
        bool auto=Physics.autoSimulation;float scale=Time.timeScale;
        string failure=null;var steps=Frames();
        while(true){bool more=false;try{more=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(!more||failure!=null)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();Physics.autoSimulation=auto;Time.timeScale=scale;
        Match3DFlow.Instance.Home();
        Directory.CreateDirectory("Logs");
        var report=failure==null?"PASS pan toss removal: sensor classes and force APIs absent; legacy button hidden and callback inert; pickup reaches tray; independent shuffle completes and preserves board identities and tray item.\n":"FAIL pan toss removal: "+failure;
        File.WriteAllText("Logs/phone-shake-validation.txt",report);Debug.Log(report);
    }
    static IEnumerator Frames(){
        var assembly=typeof(Match3DFlow).Assembly;
        Check(assembly.GetType("Match3DPhoneShake")==null&&assembly.GetType("PhoneShakeDetector")==null,"phone shake code remains");
        Check(typeof(ItemPhysicsController).GetMethod("ShakeAll")==null&&typeof(ItemPhysicsController).GetMethod("ApplyPhoneShake")==null,"shake force APIs remain");
        Time.timeScale=1;Physics.autoSimulation=false;
        var flow=Match3DFlow.Instance;flow.Begin(0);flow.NextStage();BoardCreationTestSupport.FinishPending(flow);
        GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;yield return null;
        var board=LevelGenerator.Instance.objectsOnScene;var ids=board.Select(o=>o.GetInstanceID()).ToArray();
        var bodies=board.Select(o=>o.GetComponent<Rigidbody>()).ToArray();
        var positions=bodies.Select(b=>b.position).ToArray();var velocities=bodies.Select(b=>b.velocity).ToArray();var spins=bodies.Select(b=>b.angularVelocity).ToArray();
        var legacy=UnityEngine.Object.FindObjectOfType<FoodPileShake>();Check(legacy,"legacy scene component missing");
        Check(!legacy.button||!legacy.button.gameObject.activeSelf,"legacy button visible");legacy.Shake();
        for(int i=0;i<bodies.Length;i++)Check(bodies[i].position==positions[i]&&bodies[i].velocity==velocities[i]&&bodies[i].angularVelocity==spins[i],"legacy callback moved an item");
        var input=InputController.Instance;var item=board.First(o=>o&&o.activeInHierarchy&&o.GetComponentsInChildren<Renderer>().Any(r=>r.enabled));
        input.AddObjectOnStack(item,false);yield return new WaitForSeconds(.7f);
        Check(input.arrivedObject.Contains(item),"pickup did not reach tray");var trayPosition=item.transform.position;
        Check(BoardTools.RunPaid(2,()=>BoardTools.MessUp()),"independent shuffle failed to start");
        yield return new WaitForSeconds(2);
        Check(!BoardTools.Shuffling,"shuffle did not complete");
        Check(ids.SequenceEqual(board.Select(o=>o.GetInstanceID())),"shuffle changed board identities");
        Check(input.objectsOnStack.Contains(item)&&Vector3.Distance(item.transform.position,trayPosition)<.001f,"shuffle disturbed tray item");
    }
}
