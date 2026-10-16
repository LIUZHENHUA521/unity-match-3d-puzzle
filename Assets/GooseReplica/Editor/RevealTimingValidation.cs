using System;
using System.IO;
using System.Linq;
using UnityEngine;
public static class RevealTimingValidation {
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    public static void Run(){
        bool auto=Physics.autoSimulation;string report="";
        try{
            Physics.autoSimulation=false;
            var flow=ReplicaFlow.Instance;flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
            var pile=ReplicaLayeredPile.Instance;var board=LevelGenerator.Instance.objectsOnScene;
            var initial=board.First(o=>o.GetComponentInChildren<Renderer>().enabled);
            Check(initial.transform.localScale.sqrMagnitude>0&&pile.CanPick(initial.GetComponent<Rigidbody>()),"initial visible objects are already full-size");
            var obj=board.First(o=>!o.GetComponentInChildren<Renderer>().enabled);
            var body=obj.GetComponent<Rigidbody>();var savedScale=obj.transform.localScale;
            body.position=new Vector3(0,pile.Threshold+.25f,0);pile.RefreshVisibility();
            Check(obj.transform.localScale==savedScale&&pile.CanPick(body),"default-false source gate preserves full size and immediate picking on reveal");
            body.position=Vector3.zero;pile.RefreshVisibility();
            Check(!obj.GetComponentInChildren<Renderer>().enabled&&!pile.CanPick(body),"hidden source objects remain unpickable");
            body.position=new Vector3(0,pile.Threshold+.25f,0);pile.RefreshVisibility();
            Check(obj.transform.localScale==savedScale&&pile.CanPick(body),"ordinary repeated reveal does not add a tween");

            // Explicitly opt in only to test the dormant j19265 branch. No gameplay caller does this.
            pile.Detach(body);body.position=Vector3.zero;pile.Register(body,sourceChangeObjScale:true);
            body.position=new Vector3(0,pile.Threshold+.25f,0);pile.RefreshVisibility();
            Check(obj.transform.localScale==Vector3.zero&&!pile.CanPick(body),"enabled source gate starts growth at exact zero");
            pile.StepVisibility(.1875f);var quarter=obj.transform.localScale.magnitude;
            // Hiding and re-showing an unfinished dormant tween must not restart it.
            body.position=Vector3.zero;pile.RefreshVisibility();
            pile.StepVisibility(.1875f);var half=obj.transform.localScale.magnitude;
            body.position=new Vector3(0,pile.Threshold+.25f,0);pile.RefreshVisibility();
            Check(Mathf.Abs(obj.transform.localScale.magnitude-half)<.0001f&&!pile.CanPick(body),"enabled unfinished tween continues through hiding without restart");
            pile.StepVisibility(.375f);var full=obj.transform.localScale.magnitude;
            Check(full>0&&Mathf.Abs(quarter/full-.4375f)<.0001f,"dormant OutQuad quarter point");
            Check(Mathf.Abs(half/full-.75f)<.0001f,"dormant OutQuad midpoint");
            Check(pile.CanPick(body),"enabled growth completes at .75 seconds");
            body.position=Vector3.zero;pile.RefreshVisibility();
            body.position=new Vector3(0,pile.Threshold+.25f,0);pile.RefreshVisibility();
            Check(obj.transform.localScale==Vector3.zero&&!pile.CanPick(body),"only explicitly enabled completed objects replay growth on a later reveal");
            // Restore the ordinary source gate before checking the optimization clock.
            pile.Detach(body);pile.Register(body);
            float threshold=pile.Threshold;
            foreach(var item in board){var itemBody=item.GetComponent<Rigidbody>();itemBody.position+=Vector3.down*2;}
            Physics.SyncTransforms();
            pile.StepVisibility(.24f);
            Check(pile.Threshold==threshold,"no visibility check before 1 second");
            pile.StepVisibility(.011f);
            Check(pile.Threshold<threshold,"visibility check after 1 second");
            float after=pile.Threshold;pile.StepVisibility(0);
            Check(pile.Threshold==after,"paused scaled time does not advance check");
            pile.StepVisibility(3);
            Check(after-pile.Threshold<.6f,"slow frame performs only one height step");
            Check(board.Count==300,"revealing does not create new objects");
            report="PASS ordinary reveal keeps full scale and picking readiness; explicit-only dormant .75s OutQuad branch and repeated transitions; 1s scaled check; pause; no catch-up burst; 300 objects retained.\n";
        }catch(Exception e){report="FAIL "+e;}
        finally{ReplicaFlow.Instance.Home();Physics.autoSimulation=auto;File.WriteAllText("Temp/goose-reveal-timing-validation.txt",report);Debug.Log(report);}
    }
}
