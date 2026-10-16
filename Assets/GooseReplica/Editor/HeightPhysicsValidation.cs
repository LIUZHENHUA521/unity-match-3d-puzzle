using System;
using System.IO;
using System.Linq;
using UnityEngine;
public static class HeightPhysicsValidation {
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    public static void Run(){
        bool auto=Physics.autoSimulation;string report="";
        try {
            Physics.autoSimulation=false;
            var flow=ReplicaFlow.Instance;flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
            var pile=ReplicaLayeredPile.Instance;var physics=ReplicaPilePhysics.Instance;
            var board=LevelGenerator.Instance.objectsOnScene;
            pile.RefreshVisibility();float oldThreshold=pile.PhysicsThreshold;
            var revealed=board.First(o=>!o.GetComponentInChildren<Renderer>().enabled);
            var revealedBody=revealed.GetComponent<Rigidbody>();revealedBody.position=new Vector3(0,pile.Threshold+.25f,0);pile.RefreshVisibility();
            var deep=board.Where(o=>o!=revealed).Select(o=>o.GetComponent<Rigidbody>()).Where(b=>b.position.y<pile.Threshold).ToArray();
            Check(deep.Length>0,"hidden deep population exists");
            var startScale=revealed.transform.localScale;
            var collider=revealed.GetComponentsInChildren<Collider>().First(c=>c.enabled&&c.gameObject.layer==ReplicaSourceColliders.BodyLayer);Physics.SyncTransforms();var startBounds=collider.bounds.size.magnitude;
            pile.StepVisibility(.375f);Physics.SyncTransforms();
            Check(revealed.transform.localScale==startScale&&startScale.sqrMagnitude>0,"ordinary reveal keeps full transform scale");
            Check(Mathf.Abs(collider.bounds.size.magnitude-startBounds)<.0001f&&startBounds>0,"support collider does not collapse or regrow on reveal");
            Check(pile.CanPick(revealedBody),"ordinary reveal does not clear finished-create state");
            double simulation=0,management=0;
            for(int i=0;i<12;i++){
                physics.Step(.02f);management+=physics.LastStepMilliseconds;
                var timer=System.Diagnostics.Stopwatch.StartNew();Physics.Simulate(.02f);simulation+=timer.Elapsed.TotalMilliseconds;
                Check(deep.All(b=>b.isKinematic),"hidden deep bodies stay frozen");
            }
            Check(deep.All(b=>b.detectCollisions),"deep support collision retained");
            foreach(var obj in board.Where(o=>o.transform.position.y>=oldThreshold).ToArray()){
                var body=obj.GetComponent<Rigidbody>();pile.Detach(body);physics.RemoveForPick(body);body.detectCollisions=false;obj.SetActive(false);
            }
            for(int i=0;i<120;i++)pile.StepVisibility(.02f);
            Check(pile.PhysicsThreshold<oldThreshold,"threshold follows exposed pile");physics.Step(.1f);
            Check(board.Where(o=>o.activeSelf).Any(o=>!o.GetComponent<Rigidbody>().isKinematic),"exposed band remains dynamic");
            Check(board.Count==300,"no additional objects created");
            report=$"PASS hidden depth gating; ordinary reveal preserves transform/collider size; exposed band activation; 300 objects retained.\nEditor 12 ticks: manager mean={management/12:F3} ms; Physics.Simulate mean={simulation/12:F3} ms. Not device FPS.\n";
            flow.Home();
            // Isolated rigid bodies verify removal of the count/time policies without relying on sleep timing.
            var probes=new System.Collections.Generic.List<GameObject>();
            try {
                for(int i=0;i<24;i++){var obj=new GameObject("Physics policy probe");probes.Add(obj);obj.transform.position=new Vector3(i*3,50,0);var body=obj.AddComponent<Rigidbody>();physics.Register(body);body.useGravity=false;body.velocity=Vector3.right;}
                Check(physics.DynamicCount==24,"more than 16 bodies can be dynamic");
                for(int i=0;i<151;i++)physics.Step(.02f);
                Check(physics.DynamicCount==24,"no two-second forced freeze");
                var first=probes[0].GetComponent<Rigidbody>();first.position=new Vector3(100,50,0);physics.Step(.1f);
                Check(first.position.x==100,"manager never clamps body position");
                report+="PASS 24 dynamic bodies; no 2-second freeze; no manager position clamp.\n";
            }finally{physics.Clear();foreach(var obj in probes)UnityEngine.Object.DestroyImmediate(obj);}
        }catch(Exception e){report+="FAIL "+e;}
        finally{ReplicaFlow.Instance.Home();Physics.autoSimulation=auto;File.WriteAllText("Temp/goose-height-physics-validation.txt",report);Debug.Log(report);}
    }
}
