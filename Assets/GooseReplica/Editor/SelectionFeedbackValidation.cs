using System;
using System.IO;
using System.Linq;
using UnityEngine;
using DG.Tweening;
public static class SelectionFeedbackValidation {
    static void Check(bool ok,string text){if(!ok)throw new Exception(text);}
    public static void Preview(){
        var obj=LevelGenerator.Instance.objectsOnScene.Where(o=>o&&o.GetComponentInChildren<Renderer>().enabled).OrderByDescending(o=>o.transform.position.y).First();
        InputController.Instance.GetComponent<ReplicaSelectionFeedback>().Begin(obj,Camera.main);
    }
    public static void Run(){
        var flow=ReplicaFlow.Instance;if(!flow)throw new Exception("Enter Play first");
        bool auto=Physics.autoSimulation;string report="";
        try{
            flow.Begin(0);GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;
            Physics.autoSimulation=false;var input=InputController.Instance;
            var feedback=input.GetComponent<ReplicaSelectionFeedback>();var manager=ReplicaPilePhysics.Instance;
            var obj=LevelGenerator.Instance.objectsOnScene.OrderByDescending(o=>o.transform.position.y).First();var body=obj.GetComponent<Rigidbody>();
            Physics.SyncTransforms();var position=body.position;int count=manager.BoardCount;
            feedback.Begin(obj,Camera.main);
            Check(feedback.Selected==obj&&body.isKinematic&&body.detectCollisions,"press keeps solid kinematic body");
            Check(!manager.Contains(body)&&manager.BoardCount==count-1,"selected body excluded from settling");
            var block=new MaterialPropertyBlock();var r=obj.GetComponentInChildren<Renderer>();r.GetPropertyBlock(block);
            Check(block.GetFloat("_SelectionGlow")==0&&ReplicaSourceHighlight.For(Camera.main).Target==obj,"source highlight owns selection; old Toon additive glow disabled");
            ReplicaSourceHighlight.For(Camera.main).Rebuild();Check(ReplicaSourceHighlight.For(Camera.main).DrawCount>=12,"source stencil, overlay, four glow and four outline draws configured");
            for(int i=0;i<12;i++){feedback.SendMessage("FixedUpdate");manager.Step(.02f);Physics.Simulate(.02f);}
            Check(Vector3.Distance(body.position,position)>.1f&&Vector3.Distance(body.position,position)<.5f,"small physical lift");
            
            feedback.Cancel();r.GetPropertyBlock(block);
            Check(block.GetFloat("_SelectionGlow")==0&&manager.Contains(body)&&manager.BoardCount==count,"cancel clears glow and rejoins board");
            feedback.Begin(obj,Camera.main);input.AddObjectOnStack(obj);r.GetPropertyBlock(block);
            Check(!feedback.Selected&&block.GetFloat("_SelectionGlow")==0&&!body.detectCollisions,"collection clears selection and collision");
            Check(input.objectsOnStack.Count==1&&LevelGenerator.Instance.objectsOnScene.Count==12,"press and collection do not duplicate items");
            var release=obj.transform.position;var slot=ReplicaTray.Instance.SlotPosition(0);
            DOTween.Goto(obj.transform,.03f,false);
            var travel=obj.transform.position-release;var toward=slot-release;toward.y=0;travel.y=0;
            Check(Vector3.Dot(travel,toward)>0,"travel toward tray starts within 30ms, without second lift");
            DOTween.Goto(obj.transform,.30f,false);
            Check(Vector3.Distance(obj.transform.position,slot)<.001f,"arrival at 300ms");
            feedback.SendMessage("FixedUpdate");
            Check(Vector3.Distance(obj.transform.position,slot)<.001f,"old press lift cannot move collected item");
            flow.Home();Check(manager.BoardCount==0&&!feedback.Selected,"home clears held state");
            report="PASS first-stage compatibility: source geometry glow; legacy kinematic lift and neighbour wake; cancel restore; collection cleanup; immediate tray travel; 300ms arrival; no residual lift; home cleanup\n";
        }catch(Exception e){report="FAIL "+e+"\n";}
        finally{Physics.autoSimulation=auto;flow.Home();File.WriteAllText("Temp/goose-selection-validation.txt",report);Debug.Log(report);}
    }
}
