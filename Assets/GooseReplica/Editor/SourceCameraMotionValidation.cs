using System;
using System.IO;
using System.Linq;
using UnityEngine;

// Source's shipped default is flatTopView. A falling stack must not introduce a camera tween.
public static class SourceCameraMotionValidation {
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run(){
        string report="";var flow=ReplicaFlow.Instance;bool automatic=Physics.autoSimulation;
        try{
            Physics.autoSimulation=false;
            Check(ReplicaThemePresentation.SourceControlMode==2,"shipped flatTopView mode");
            for(int theme=0;theme<3;theme++){
                flow.Begin(theme);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);var camera=Camera.main;var p=camera.transform.position;var r=camera.transform.rotation;var size=camera.orthographicSize;
                var pile=ReplicaLayeredPile.Instance;float previous=pile.Threshold;
                foreach(var obj in LevelGenerator.Instance.objectsOnScene){var body=obj.GetComponent<Rigidbody>();body.position-=Vector3.up*8;}
                for(int i=0;i<12;i++){
                    pile.StepVisibility(1);flow.GetComponent<ReplicaThemePresentation>().Fit();
                    Check(Vector3.Distance(p,camera.transform.position)<.00001f&&Mathf.Abs(Quaternion.Dot(r,camera.transform.rotation))>1-.000001f,"stack height must not move/orbit flatTopView camera");
                    Check(Mathf.Abs(size-camera.orthographicSize)<.00001f,"no stack-follow zoom");
                }
                Check(pile.Threshold<previous,"height/visibility actually changed during probe");
                Check(LevelGenerator.Instance.objectsOnScene.Count==300,"camera probe retains board count");
                flow.Home();
                report+="PASS theme "+theme+": lowered all 300 bodies by 8 units; 12 visibility checks; default camera position, rotation and zoom unchanged; no inferred follow-height tween.\n";
            }
        }catch(Exception e){report+="FAIL "+e+"\n";}
        finally{flow.Home();Physics.autoSimulation=automatic;File.WriteAllText("Temp/goose-camera-motion-validation.txt",report);Debug.Log(report);}
    }
}
