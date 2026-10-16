using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Validate the 12 newly recovered special-goose profiles against their serialized source shapes.
// Run in the disposable Play Mode validation project after importing the original item prefabs.
public static class SourceGooseColliderValidation {
    [Serializable] class Data {public Profile[] items;}
    [Serializable] class Profile {public string name,source;public int[] col,realCol;public Node[] nodes;}
    [Serializable] class Node {public int[] indices;public int layer;public Shape[] colliders;}
    [Serializable] class Shape {public string type;public bool enabled,trigger,hasMaterial;public Vector3 center,size;public float radius,height;public int direction;public Surface material;}
    [Serializable] class Surface {public float dynamicFriction,staticFriction,bounciness;public int frictionCombine,bounceCombine;}
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static Transform At(Transform root,int[] indices){foreach(int i in indices)root=root.GetChild(i);return root;}
    public static void Run(){
        string report="";GameObject instance=null;bool hadPhysics=ReplicaSourcePhysics.Active;
        try{
            ReplicaSourcePhysics.Begin();
            var profiles=JsonUtility.FromJson<Data>(Resources.Load<TextAsset>("Replica/SourceColliders").text).items.Where(p=>p.name.StartsWith("Goose_",StringComparison.Ordinal)).ToArray();
            Check(profiles.Length==12,"all 12 source Goose collider profiles present");int count=0;
            foreach(var p in profiles){
                string group=Path.GetFileName(Path.GetDirectoryName(p.source));
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GooseReplica/OriginalModels/Prefabs/"+group+"/"+p.name+".prefab");
                Check(prefab,"missing imported source Goose "+p.name);
                instance=UnityEngine.Object.Instantiate(prefab,new Vector3(100,100,100),Quaternion.identity);
                ReplicaSourceColliders.Apply(instance,p.name,true);var body=instance.GetComponent<Rigidbody>();ReplicaSourcePhysics.Apply(body);
                Check(body.mass==.05f&&body.drag==0&&body.angularDrag==.05f&&body.collisionDetectionMode==CollisionDetectionMode.Discrete,p.name+" source body settings");
                var visual=instance.transform.GetChild(0);
                var skins=instance.GetComponentsInChildren<SkinnedMeshRenderer>(true);Check(skins.Length>0,p.name+" body mesh is present, not only accessories");
                foreach(var skin in skins){var mesh=skin.sharedMesh;Check(mesh&&mesh.vertexCount>0&&mesh.boneWeights.Length==mesh.vertexCount,p.name+" skin vertex/weights");
                    Check(skin.bones.Length>0&&skin.bones.All(b=>b)&&skin.rootBone&&mesh.bindposes.Length==skin.bones.Length,p.name+" complete original bone bindings");
                    Check(mesh.boneWeights.All(w=>Mathf.Abs(w.weight0+w.weight1+w.weight2+w.weight3-1)<.002f),p.name+" normalized source bone weights");
                    var baked=new Mesh();try{skin.BakeMesh(baked);Check(baked.vertexCount==mesh.vertexCount&&baked.bounds.size.sqrMagnitude>.01f,p.name+" body renders in recovered static pose");}finally{UnityEngine.Object.DestroyImmediate(baked);}
                }

                foreach(var node in p.nodes){var target=At(visual,node.indices);Check(target.gameObject.layer==node.layer,p.name+" source layer");
                    var all=target.GetComponents<Collider>();var actual=all.Skip(all.Length-node.colliders.Length).ToArray();Check(actual.Length==node.colliders.Length,p.name+" shape count");
                    for(int i=0;i<actual.Length;i++){var a=actual[i];var s=node.colliders[i];count++;
                        Check(a.GetType().Name==s.type&&a.enabled==s.enabled&&a.isTrigger==s.trigger,p.name+" shape type/state");
                        if(a is BoxCollider box)Check(box.center==s.center&&box.size==s.size,p.name+" box geometry");
                        else if(a is SphereCollider sphere)Check(sphere.center==s.center&&sphere.radius==s.radius,p.name+" sphere geometry");
                        else if(a is CapsuleCollider capsule)Check(capsule.center==s.center&&capsule.radius==s.radius&&capsule.height==s.height&&capsule.direction==s.direction,p.name+" capsule geometry");
                        var material=a.sharedMaterial;Check(material&&a.contactOffset==ReplicaSourcePhysics.Contact,p.name+" contact material");
                        if(s.hasMaterial){var m=s.material;Check(material.dynamicFriction==m.dynamicFriction&&material.staticFriction==m.staticFriction&&material.bounciness==m.bounciness&&(int)material.frictionCombine==m.frictionCombine&&(int)material.bounceCombine==m.bounceCombine,p.name+" source friction");}
                    }
                }
                var component=instance.GetComponent<ReplicaSourceColliders>();Check(component&&component.HasPickObject,p.name+" source click object");
                var pick=At(visual,p.col);var real=At(visual,p.realCol);bool realActive=real.gameObject.activeSelf;
                component.SetVisible(false);Check(!pick.gameObject.activeSelf&&real.gameObject.activeSelf==realActive,p.name+" hide pick without hiding support");
                component.SetVisible(true);Check(pick.gameObject.activeSelf,p.name+" show pick");
                UnityEngine.Object.DestroyImmediate(instance);instance=null;
            }
            report="PASS 12 recovered Goose profiles; "+count+" source colliders; exact geometry/layers/state/material; source Rigidbody parameters; complete skinned body/bones/weights and nonempty baked pose; click visibility retains physical support.\n";
        }catch(Exception e){report="FAIL "+e+"\n";}
        finally{if(instance)UnityEngine.Object.DestroyImmediate(instance);if(!hadPhysics)ReplicaSourcePhysics.End();File.WriteAllText("Temp/goose-source-goose-collider-validation.txt",report);Debug.Log(report);}
    }
}
