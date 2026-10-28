using System;
using System.IO;
using UnityEngine;

// Deterministic lifecycle probes, independent of random level composition.
public static class PileOptimizationValidation {
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    public static void Run(){
        var flow=ReplicaFlow.Instance;
        bool auto=Physics.autoSimulation;
        GameObject high=null,low=null;
        string report;
        try{
            Physics.autoSimulation=false;flow.Home();ReplicaSourceEnvironment.Select("CountrysideStove");
            var pile=ReplicaLayeredPile.Instance;pile.Begin();
            high=GameObject.CreatePrimitive(PrimitiveType.Cube);
            var top=high.AddComponent<Rigidbody>();top.position=Vector3.up*30;
            pile.Register(top);
            low=GameObject.CreatePrimitive(PrimitiveType.Cube);
            var body=low.AddComponent<Rigidbody>();body.position=Vector3.zero;
            var renderer=low.GetComponent<Renderer>();
            pile.Register(body);pile.Register(body);
            Check(!renderer.enabled&&!pile.CanPick(body)&&pile.HiddenCount==1,"first hidden state and duplicate registration");
            Check(!pile.NeedsPhysics(body)&&pile.NeedsPhysics(top),"height lookup");
            body.position=Vector3.up*30;pile.RefreshVisibility();
            Check(renderer.enabled&&pile.CanPick(body)&&pile.NeedsPhysics(body),"reveal restores picking and physics eligibility");
            // An unchanged visibility tick must not write renderer state again.
            renderer.enabled=false;pile.RefreshVisibility();
            Check(!renderer.enabled,"unchanged visibility avoids redundant renderer writes");
            renderer.enabled=true;
            body.position=Vector3.zero;pile.RefreshVisibility();
            Check(!renderer.enabled&&!pile.CanPick(body),"hide transition");
            body.position=Vector3.up*30;
            pile.StepVisibility(.5f);Check(!renderer.enabled,"no scan before one second");
            pile.StepVisibility(.5f);Check(renderer.enabled,"scan at one second");
            pile.Detach(body);body.position=Vector3.zero;pile.Register(body,true);
            Check(!renderer.enabled,"detach then register initializes visibility again");
            body.position=Vector3.up*30;pile.RefreshVisibility();
            Check(low.transform.localScale==Vector3.zero&&!pile.CanPick(body),"opt-in reveal growth retained");
            pile.StepVisibility(.75f);Check(pile.CanPick(body),"growth completes");
            UnityEngine.Object.DestroyImmediate(low);low=null;pile.RefreshVisibility();
            Check(pile.HiddenCount==0&&pile.CanPick(top),"destroyed entry cleanup");
            pile.Clear();pile.Begin(true);pile.Register(top);
            Check(!pile.CanPick(top),"clear and restart reset creation state");
            pile.FinishCreation();Check(pile.CanPick(top),"creation completes after restart");
            report="PASS initial hidden/visible state; duplicate registration; physics lookup; transition-only renderer updates; 1s cadence; detach/re-register; optional reveal growth; destroyed entry cleanup; clear/restart and creation completion. Not an iOS FPS measurement.\n";
        }catch(Exception e){report="FAIL "+e;}
        finally{
            flow.Home();if(high)UnityEngine.Object.DestroyImmediate(high);if(low)UnityEngine.Object.DestroyImmediate(low);
            Physics.autoSimulation=auto;
        }
        File.WriteAllText("Temp/goose-pile-optimization-validation.txt",report);Debug.Log(report);
    }
}
