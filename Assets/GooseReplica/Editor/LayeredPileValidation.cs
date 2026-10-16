using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Run via Temp/goose-command.txt: layercheck, while playing at the home screen.
public static class LayeredPileValidation {
    public static void Run(){
        if(!EditorApplication.isPlaying||!ReplicaFlow.Instance)throw new Exception("Enter play first");
        bool auto=Physics.autoSimulation;string report="";
        try{
            Physics.autoSimulation=false;
            var flow=ReplicaFlow.Instance;
            for(int theme=0;theme<flow.library.themes.Length;theme++){
                flow.Begin(theme);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
                var board=LevelGenerator.Instance.objectsOnScene;var pile=ReplicaLayeredPile.Instance;var physics=ReplicaPilePhysics.Instance;
                Check(board.Count==300,"300 objects created at start");
                Check(pile.HiddenCount>0&&pile.HiddenCount<300,"initial visible and hidden populations");
                Check(board.Where(o=>!o.GetComponentsInChildren<Renderer>().Any(r=>r.enabled)).All(o=>!pile.CanPick(o.GetComponent<Rigidbody>())),"hidden renderers cannot be picked");
                Check(board.Where(o=>o.GetComponentsInChildren<Renderer>().Any(r=>r.enabled)).All(o=>pile.CanPick(o.GetComponent<Rigidbody>())),"finished visible source objects have no default reveal-growth picking delay");
                var ids=board.Select(o=>o.GetInstanceID()).ToArray();int ticks=Settle(physics);for(int i=0;i<120;i++)pile.StepVisibility(.02f);
                Check(board.All(o=>o.transform.position.y>-.5f&&o.transform.position.y<Camera.main.transform.position.y-1),"settled bodies stay above floor and below camera");
                Check(board.All(o=>new Vector2(o.transform.position.x,o.transform.position.z-.15f).magnitude<ReplicaLayeredPile.Radius(o.transform.position.y)+.35f),"settled bodies stay inside tapered boundary");
                var input=InputController.Instance;
                Check(board.Any(o=>input.PickAt(Camera.main.WorldToScreenPoint(o.transform.position))!=null),"visible settled objects can be ray-picked");
                int beforeHidden=pile.HiddenCount;
                // Remove the exposed band as if collected, then verify existing objects become visible.
                foreach(var obj in board.Where(o=>pile.CanPick(o.GetComponent<Rigidbody>())).ToArray()){
                    var body=obj.GetComponent<Rigidbody>();pile.Detach(body);physics.RemoveForPick(body);body.detectCollisions=false;obj.SetActive(false);
                }
                for(int i=0;i<120;i++)pile.StepVisibility(.02f);Check(pile.HiddenCount<beforeHidden,"lower band revealed without creation");
                Check(board.Select(o=>o.GetInstanceID()).SequenceEqual(ids),"reveal preserves every object ID and count");
                flow.RestartStage();physics.ShakeAll();Settle(physics);
                Check(LevelGenerator.Instance.objectsOnScene.All(o=>o.transform.position.y>-.5f),"shake containment");
                report+=$"PASS {flow.library.themes[theme].id}: 300 objects; initial hidden={beforeHidden}; settle ticks={ticks}; pick/reveal/identity/shake\n";
            }
            flow.Home();Check(!ReplicaLayeredPile.Instance.Running&&LevelGenerator.Instance.objectsOnScene.Count==0,"home cleans layered state");
            report+="ALL LAYER CHECKS PASSED\n";
        }catch(Exception e){report+="FAIL "+e+"\n";Debug.LogException(e);}
        finally{Physics.autoSimulation=auto;File.WriteAllText("Temp/goose-layer-validation.txt",report);Debug.Log(report);ReplicaFlow.Instance.Home();}
    }
    static int Settle(ReplicaPilePhysics physics){int ticks=0;Physics.SyncTransforms();while(physics.IsSettling&&ticks<2400){physics.Step(.02f);Physics.Simulate(.02f);ticks++;}Check(!physics.IsSettling,"settle timeout");return ticks;}
    static void Check(bool success,string message){if(!success)throw new Exception(message);}
}
