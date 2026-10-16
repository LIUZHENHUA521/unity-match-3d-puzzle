using System;
using System.IO;
using System.Linq;
using UnityEngine;
public static class SourceAlignmentValidation {
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run(){
        bool auto=Physics.autoSimulation;string report="";
        try{
            Physics.autoSimulation=false;ReplicaSourceEnvironment.Select(null);
            // Fixture values from WX saved transforms, including the parent's -7 degree tilt.
            Check(Mathf.Abs(ReplicaSourceEnvironment.GetBorder(5).scale.x-1.6201127f)<.00001f,"floor border, not interpolation");
            Check(Mathf.Abs(ReplicaSourceEnvironment.GetBorder(6).position.z+.7312162f)<.00001f,"world-space border centre");
            Check(Mathf.Abs(ReplicaSourceEnvironment.WeightedHeight(0,1,1,3,.5)-.61803399f)<.00001f,"linear-density median");
            int corners=0;var random=new System.Random(20261015);
            for(int i=0;i<10000;i++){
                var p=ReplicaSourceEnvironment.Position(1.5f,14,random);var b=ReplicaSourceEnvironment.GetBorder(p.y);
                Check(p.y>=1.5f&&p.y<=14,"sample stays inside layer height interval");
                var q=p-b.position;float x=b.scale.x*.45f,z=b.scale.z*.45f;
                Check(Mathf.Abs(q.x)<=x&&Mathf.Abs(q.z)<=z,"rectangle bounds");
                if(q.x*q.x/(x*x)+q.z*q.z/(z*z)>1)corners++;
            }
            Check(corners>1500,"rectangle corners sampled, no circular projection");
            var flow=ReplicaFlow.Instance;
            for(int theme=0;theme<3;theme++){
                flow.Begin(theme);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);var pile=ReplicaLayeredPile.Instance;var board=LevelGenerator.Instance.objectsOnScene;
                Check(board.Count==300,"all objects exist upfront");
                foreach(var obj in board){
                    var source=obj.GetComponent<ReplicaSourceColliders>();
                    if(!source)continue; // Custom hamster/fruit models have no source-prefab evidence.
                    var colliders=obj.GetComponentsInChildren<Collider>(true).Where(c=>c.enabled).ToArray();
                    Check(colliders.Any(c=>c.gameObject.layer==7),"source picking collider restored: "+obj.name);
                    Check(colliders.Any(c=>c.gameObject.layer==10&&c.gameObject.activeInHierarchy),"real collision remains active while hidden: "+obj.name);
                    Check(colliders.All(c=>!c.sharedMaterial||Mathf.Abs(c.sharedMaterial.dynamicFriction-.6f)<.00001f),"source surface restored");
                    Check(obj.transform.GetChild(0).localPosition==Vector3.zero,"source pivot restored");
                }
                Check(Physics.GetIgnoreLayerCollision(7,10)&&Physics.GetIgnoreLayerCollision(7,7),"picking shapes never simulate");
                Check(!Physics.GetIgnoreLayerCollision(10,10)&&!Physics.GetIgnoreLayerCollision(10,6),"physical shapes hit food and walls");
                var visible=board.First(o=>o.GetComponent<ReplicaSourceColliders>()&&!o.GetComponentInChildren<Renderer>().enabled);
                var body=visible.GetComponent<Rigidbody>();var fullScale=visible.transform.localScale;
                var original=new Vector3(0,pile.Threshold+.25f,0);body.position=original;pile.RefreshVisibility();
                Check(visible.transform.localScale==fullScale&&pile.CanPick(body),"ordinary source reveal retains scale and finished-create picking");
                pile.StepVisibility(.2f);body.position=new Vector3(original.x,pile.Threshold-2,original.z);pile.RefreshVisibility();
                Check(!visible.GetComponentInChildren<Renderer>().enabled,"falling below threshold hides again");
                Check(!visible.GetComponentsInChildren<Collider>().Any(c=>c.enabled&&c.gameObject.layer==7),"hidden click collider deactivated");
                pile.StepVisibility(.1f);
                Check(visible.transform.localScale==fullScale,"hidden support scale stays unchanged");
                body.position=original;pile.RefreshVisibility();
                Check(visible.GetComponentInChildren<Renderer>().enabled&&visible.transform.localScale==fullScale&&pile.CanPick(body),"repeat source reveal has no tween or extra picking delay");
                Check(board.Count==300,"visibility transition creates nothing");
                Physics.SyncTransforms();
                for(int tick=0;tick<150;tick++){
                    pile.StepVisibility(.02f);ReplicaPilePhysics.Instance.Step(.02f);Physics.Simulate(.02f);
                }
                Check(board.All(o=>o.transform.position.y>-.5f&&Mathf.Abs(o.transform.position.x)<15&&Mathf.Abs(o.transform.position.z)<15),"three-second pile stays in scene bounds");
                int picks=0;var cam=Camera.main;
                for(int sy=1;sy<20;sy++)for(int sx=1;sx<20;sx++){
                    var hit=InputController.Instance.PickAt(new Vector2(cam.pixelRect.x+cam.pixelWidth*sx/20f,cam.pixelRect.y+cam.pixelHeight*sy/20f));
                    if(hit)picks++;
                }
                Check(picks>0,"source walls do not block visible food picking");
                report+="Theme "+theme+": 3-second simulation, in-bounds population 300, selectable grid points "+picks+".\n";
                flow.Home();
            }
            report+="PASS 10,000 weighted rectangular coordinates; floor borders/world centres; all three themes retain 300 items; source picking/body layers and materials; source pivots; reversible visibility; default-false reveal gate; full-scale repeated visibility without extra picking delay.\n";
        }catch(Exception e){report+="FAIL "+e+"\n";}
        finally{ReplicaFlow.Instance.Home();Physics.autoSimulation=auto;File.WriteAllText("Temp/goose-source-alignment-validation.txt",report);Debug.Log(report);}
    }
}
