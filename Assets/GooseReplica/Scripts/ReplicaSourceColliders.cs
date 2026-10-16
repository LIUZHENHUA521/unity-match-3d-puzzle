using System;
using System.Collections.Generic;
using UnityEngine;

// Geometry/layers from the existing iOS source prefabs; layer matrix cross-checked in WX2.1.12.
public sealed class ReplicaSourceColliders : MonoBehaviour {
    public const int PickLayer=7,BodyLayer=10,WallLayer=6;
    [Serializable] class Data {public Profile[] items;}
    [Serializable] class Profile {public string name;public int[] col,realCol;public Node[] nodes;}
    [Serializable] class Node {public int[] indices;public int layer;public Shape[] colliders;}
    [Serializable] class Shape {public string type;public bool enabled,trigger,hasMaterial;public Vector3 center,size;public float radius,height;public int direction;public Surface material;}
    [Serializable] class Surface {public string m_Name;public float dynamicFriction,staticFriction,bounciness;public int frictionCombine,bounceCombine;}
    static Dictionary<string,Profile> profiles;
    static readonly Dictionary<string,PhysicMaterial> materials=new Dictionary<string,PhysicMaterial>();
    GameObject pickObject;
    public bool HasPickObject=>pickObject;
    public void SetVisible(bool visible){if(pickObject)pickObject.SetActive(visible);}
    static Transform Resolve(Transform root,int[] path){if(path==null)return null;foreach(int i in path){if(i>=root.childCount)return null;root=root.GetChild(i);}return root;}
    public static void ConfigureLayers(){
        for(int i=0;i<32;i++){
            Physics.IgnoreLayerCollision(PickLayer,i,true);
            Physics.IgnoreLayerCollision(BodyLayer,i,i!=0&&i!=WallLayer&&i!=BodyLayer);
        }
    }
    public static void Apply(GameObject obj,string name,bool restorePivot=false){
        if(profiles==null){profiles=new Dictionary<string,Profile>();var data=Resources.Load<TextAsset>("Replica/SourceColliders");if(data)foreach(var p in JsonUtility.FromJson<Data>(data.text).items)profiles[p.name]=p;}
        if(!profiles.TryGetValue(name,out var profile)||obj.transform.childCount==0)return;
        var visual=obj.transform.GetChild(0);
        // Source Instantiate sets the object pivot and Euler rotation; importer had recentered its visual.
        if(restorePivot){visual.localPosition=Vector3.zero;visual.localRotation=Quaternion.identity;}
        foreach(var node in profile.nodes){
            var target=Resolve(visual,node.indices);if(!target){Debug.LogError("Source collider hierarchy mismatch: "+name);continue;}
            target.gameObject.layer=node.layer;
            foreach(var old in target.GetComponents<Collider>()){old.enabled=false;Destroy(old);}
            foreach(var shape in node.colliders){
                Collider collider;
                if(shape.type=="BoxCollider"){var c=target.gameObject.AddComponent<BoxCollider>();c.center=shape.center;c.size=shape.size;collider=c;}
                else if(shape.type=="SphereCollider"){var c=target.gameObject.AddComponent<SphereCollider>();c.center=shape.center;c.radius=shape.radius;collider=c;}
                else{var c=target.gameObject.AddComponent<CapsuleCollider>();c.center=shape.center;c.radius=shape.radius;c.height=shape.height;c.direction=shape.direction;collider=c;}
                collider.enabled=shape.enabled;collider.isTrigger=shape.trigger;
                if(shape.hasMaterial&&shape.material!=null){var m=shape.material;string key=JsonUtility.ToJson(m);if(!materials.TryGetValue(key,out var material)){material=new PhysicMaterial(m.m_Name??"Source surface"){dynamicFriction=m.dynamicFriction,staticFriction=m.staticFriction,bounciness=m.bounciness,frictionCombine=(PhysicMaterialCombine)m.frictionCombine,bounceCombine=(PhysicMaterialCombine)m.bounceCombine};materials.Add(key,material);}collider.sharedMaterial=material;}
            }
        }
        // A LevelObjectPool.Get may return an ObjectUnit that already has this
        // runtime component. Rebind it instead of adding a second registry entry.
        var component=obj.GetComponent<ReplicaSourceColliders>() ?? obj.AddComponent<ReplicaSourceColliders>();
        component.pickObject=Resolve(visual,profile.col)?.gameObject;
    }
}
