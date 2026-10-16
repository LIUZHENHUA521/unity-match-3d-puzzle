using System;
using System.IO;
using System.Linq;
using UnityEngine;
public static class PhysicsThemeValidation {
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run(){
        bool auto=Physics.autoSimulation;string report="";GameObject probe=null;
        Vector3 gravity=Physics.gravity;float sleep=Physics.sleepThreshold,contact=Physics.defaultContactOffset,bounce=Physics.bounceThreshold,depenetration=Physics.defaultMaxDepenetrationVelocity;
        try{
            Physics.autoSimulation=false;
            var flow=ReplicaFlow.Instance;string[] names={"CountrysideStove","GroceryStore","Barbecue"};float[] widths={5.851400852f,7.105272770f,7.523229599f};
            for(int theme=0;theme<3;theme++){
                flow.Begin(theme);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
                Check(ReplicaSourceEnvironment.CurrentName==names[theme],"theme selects its own source scene");
                Check(Mathf.Abs(ReplicaSourceEnvironment.GetBorder(24).scale.x-widths[theme])<.00001f,"theme-specific spawn box width");
                Check(Physics.gravity==new Vector3(0,-19.81f,0)&&Physics.sleepThreshold==.2f&&Physics.defaultContactOffset==.02f,"source global gravity/sleep/contact");
                Check(Physics.bounceThreshold==100&&Physics.defaultMaxDepenetrationVelocity==1,"source bounce/depenetration");
                foreach(var obj in LevelGenerator.Instance.objectsOnScene){
                    var b=obj.GetComponent<Rigidbody>();
                    Check(b.mass==.05f&&b.drag==0&&b.angularDrag==.05f,"source mass and drag");
                    Check(b.interpolation==RigidbodyInterpolation.None&&b.collisionDetectionMode==CollisionDetectionMode.Discrete,"source discrete mode and interpolation");
                    Check(b.sleepThreshold==.2f&&b.maxDepenetrationVelocity==1,"per-body sleep/depenetration");
                    Check(obj.GetComponentsInChildren<Collider>(true).Where(c=>c.enabled).All(c=>c.contactOffset==.02f),"per-collider contact offset");
                }
                probe=new GameObject("Source gravity probe");probe.transform.position=new Vector3(100,100,100);var body=probe.AddComponent<Rigidbody>();ReplicaSourcePhysics.Apply(body);Physics.SyncTransforms();Physics.Simulate(.02f);
                Check(Mathf.Abs(body.velocity.y-(-19.81f*.02f))<.0001f,"integrated acceleration uses original gravity without linear damping");
                UnityEngine.Object.DestroyImmediate(probe);probe=null;
                flow.Home();
                Check(!ReplicaSourcePhysics.Active&&Physics.gravity==gravity&&Physics.sleepThreshold==sleep&&Physics.defaultContactOffset==contact&&Physics.bounceThreshold==bounce&&Physics.defaultMaxDepenetrationVelocity==depenetration,"leaving solo restores all changed global settings");
            }
            Check((int)ReplicaSourcePhysics.DefaultSurface.frictionCombine==1,"source default material combine enum");
            report="PASS three distinct source environments; source gravity/mass/drag/solver-related settings; discrete collision; contact offsets; measured free-fall acceleration; global settings restored after every theme.\n";
        }catch(Exception e){report="FAIL "+e+"\n";}
        finally{if(probe)UnityEngine.Object.DestroyImmediate(probe);ReplicaFlow.Instance.Home();Physics.autoSimulation=auto;File.WriteAllText("Temp/goose-physics-theme-validation.txt",report);Debug.Log(report);}
    }
}
