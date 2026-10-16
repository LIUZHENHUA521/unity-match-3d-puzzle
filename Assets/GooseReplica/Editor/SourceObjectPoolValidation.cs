using System;
using System.IO;
using UnityEngine;

// Evidence-backed lifecycle check for LevelObjectPool.Get/Recycle. This deliberately
// checks identity reuse, reset state and Clear without starting another level.
public static class SourceObjectPoolValidation
{
    static void Check(bool value,string message){if(!value)throw new Exception(message);}

    public static void Run()
    {
        string report="";
        try
        {
            var plan=ReplicaSourceLevelPlan.Get("CountrysideStove");
            Check(plan!=null&&plan.prefabs!=null&&plan.prefabs.Length>0,"source pool prefab plan");
            var prefab=plan.prefabs[0];
            var first=ReplicaSourceObjectPool.Get(prefab,prefab.name);
            Check(first,"pool Get creates object");
            var id=first.GetInstanceID();
            var body=first.GetComponent<Rigidbody>();
            if(body){body.isKinematic=false;body.detectCollisions=true;body.velocity=Vector3.one;}
            ReplicaSourceObjectPool.Recycle(first,prefab.name);
            Check(!first.activeSelf,"Recycle deactivates object");
            var second=ReplicaSourceObjectPool.Get(prefab,prefab.name);
            Check(second&&second.GetInstanceID()==id,"Get reuses same ObjectUnit instance");
            Check(second.transform.localScale==second.GetComponent<ReplicaSourcePoolToken>().prefabScale,"Recycle resets scale");
            if(body=second.GetComponent<Rigidbody>())Check(body.isKinematic&&body.detectCollisions&&body.velocity==Vector3.zero,"Get reactivates rigid body");
            ReplicaSourceObjectPool.Recycle(second,prefab.name);ReplicaSourceObjectPool.Clear();
            report="PASS source object pool: Get/Recycle reuses ObjectUnit identity; deactivates and resets transform/rigid body; Clear releases pooled instances.\n";
        }
        catch(Exception e){report="FAIL source object pool: "+e+"\n";throw;}
        finally{Directory.CreateDirectory("Temp");File.WriteAllText("Temp/goose-source-object-pool-validation.txt",report);Debug.Log(report);}
    }
}
