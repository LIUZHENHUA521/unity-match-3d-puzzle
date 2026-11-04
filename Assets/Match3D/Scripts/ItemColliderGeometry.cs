using System;
using System.Collections.Generic;
using UnityEngine;

// Geometry/layers from the existing iOS source prefabs; layer matrix cross-checked in WX2.1.12.
public sealed class ItemColliderGeometry : MonoBehaviour {
    public const int PickLayer=7,BodyLayer=10,WallLayer=6;
    [Serializable] class Data {public Profile[] items;}
    [Serializable] class Profile {public string name;public int[] col,realCol;public Node[] nodes;}
    [Serializable] class Node {public int[] indices;public int layer;public Shape[] colliders;}
    [Serializable] class Shape {public string type;public bool enabled,trigger,hasMaterial;public Vector3 center,size;public float radius,height;public int direction;public Surface material;}
    [Serializable] class Surface {public string m_Name;public float dynamicFriction,staticFriction,bounciness;public int frictionCombine,bounceCombine;}
    static Dictionary<string,Profile> profiles;
    static Dictionary<string,Profile> adapterProfiles;
    static readonly Dictionary<string,PhysicMaterial> materials=new Dictionary<string,PhysicMaterial>();
    [SerializeField] GameObject pickObject;
    [Serializable] struct ColliderState {public Collider collider;public bool enabled,trigger;}
    [SerializeField] ColliderState[] colliderStates;
    public bool HasPickObject=>pickObject;
    [SerializeField] string configuredSourceName;
    // The original ObjectUnit prefab already owns its collider hierarchy. This
    // marker lets pooled/template clones skip the JSON walk and AddComponent
    // pass while retaining the exact baked shape/layer/material state.
    public bool IsConfiguredFor(string name)=>pickObject&&colliderStates!=null&&string.Equals(configuredSourceName,name,StringComparison.OrdinalIgnoreCase);
    public void SetVisible(bool visible){if(pickObject)pickObject.SetActive(visible);}
    public void RestoreConfiguredColliderState(){
        if(colliderStates==null)return;
        foreach(var state in colliderStates)if(state.collider){
            if(state.collider.enabled!=state.enabled)state.collider.enabled=state.enabled;
            if(state.collider.isTrigger!=state.trigger)state.collider.isTrigger=state.trigger;
        }
    }
    static Transform Resolve(Transform root,int[] path){if(path==null)return null;foreach(int i in path){if(i>=root.childCount)return null;root=root.GetChild(i);}return root;}
    public static void ConfigureLayers(){
        for(int i=0;i<32;i++){
            Physics.IgnoreLayerCollision(PickLayer,i,true);
            Physics.IgnoreLayerCollision(BodyLayer,i,i!=0&&i!=WallLayer&&i!=BodyLayer);
        }
    }
    public static void Apply(GameObject obj,string name,bool restorePivot=false,bool force=false){
        var component=obj?obj.GetComponent<ItemColliderGeometry>():null;
        var visual=obj&&obj.transform.childCount>0?obj.transform.GetChild(0):null;
        // A recovered prefab may already carry the exact source collider
        // hierarchy. Keep the imported pivot normalization even when the
        // runtime marker lets us skip the expensive profile walk.
        if(restorePivot&&visual){visual.localPosition=Vector3.zero;visual.localRotation=Quaternion.identity;}
        if(!force&&component&&component.IsConfiguredFor(name))return;
        LoadProfiles();
        // Imported replacements can share names with another theme's original
        // models (e.g. Banana). Their hierarchy paths are not interchangeable.
        var selectedProfiles=obj.GetComponent<BoardItemIdentity>()?adapterProfiles:profiles;
        if(!selectedProfiles.TryGetValue(name,out var profile)||obj.transform.childCount==0)return;
        // Source Instantiate sets the object pivot and Euler rotation; importer had recentered its visual.
        foreach(var node in profile.nodes){
            var target=Resolve(visual,node.indices);if(!target){Debug.LogError("Source collider hierarchy mismatch: "+name);continue;}
            target.gameObject.layer=node.layer;
            var existing=target.GetComponents<Collider>();
            bool reuse=existing.Length==node.colliders.Length;
            if(reuse)for(int i=0;i<existing.Length;i++){
                string type=existing[i] is BoxCollider?"BoxCollider":existing[i] is SphereCollider?"SphereCollider":existing[i] is CapsuleCollider?"CapsuleCollider":"";
                if(type!=node.colliders[i].type){reuse=false;break;}
            }
            if(!reuse){
                // Fallback is retained for malformed/legacy prefabs; normal
                // source prefabs have equal collider counts and stay on the
                // allocation-free path above.
                foreach(var old in existing){old.enabled=false;if(Application.isPlaying)Destroy(old);else DestroyImmediate(old);}
                existing=new Collider[node.colliders.Length];
                for(int i=0;i<node.colliders.Length;i++){
                    var shape=node.colliders[i];
                    if(shape.type=="BoxCollider")existing[i]=target.gameObject.AddComponent<BoxCollider>();
                    else if(shape.type=="SphereCollider")existing[i]=target.gameObject.AddComponent<SphereCollider>();
                    else existing[i]=target.gameObject.AddComponent<CapsuleCollider>();
                }
            }
            for(int i=0;i<node.colliders.Length;i++){
                var shape=node.colliders[i];var collider=existing[i];
                if(collider is BoxCollider){var c=(BoxCollider)collider;c.center=shape.center;c.size=shape.size;}
                else if(collider is SphereCollider){var c=(SphereCollider)collider;c.center=shape.center;c.radius=shape.radius;}
                else if(collider is CapsuleCollider){var c=(CapsuleCollider)collider;c.center=shape.center;c.radius=shape.radius;c.height=shape.height;c.direction=shape.direction;}
                collider.enabled=shape.enabled;collider.isTrigger=shape.trigger;
                if(shape.hasMaterial&&shape.material!=null){var m=shape.material;string key=JsonUtility.ToJson(m);if(!materials.TryGetValue(key,out var material)){material=new PhysicMaterial(m.m_Name??"Source surface"){dynamicFriction=m.dynamicFriction,staticFriction=m.staticFriction,bounciness=m.bounciness,frictionCombine=(PhysicMaterialCombine)m.frictionCombine,bounceCombine=(PhysicMaterialCombine)m.bounceCombine};materials.Add(key,material);}collider.sharedMaterial=material;}
            }
        }
        // A LevelObjectPool.Get may return an ObjectUnit that already has this
        // runtime component. Rebind it instead of adding a second registry entry.
        component=component??obj.GetComponent<ItemColliderGeometry>() ?? obj.AddComponent<ItemColliderGeometry>();
        component.pickObject=Resolve(visual,profile.col)?.gameObject;
        component.configuredSourceName=name;
        var states=new List<ColliderState>();
        foreach(var node in profile.nodes){
            var target=Resolve(visual,node.indices);if(!target)continue;
            foreach(var collider in target.GetComponents<Collider>())states.Add(new ColliderState{collider=collider,enabled=collider.enabled,trigger=collider.isTrigger});
        }
        component.colliderStates=states.ToArray();
        obj.GetComponent<BoardItemPoolToken>()?.InvalidateComponents();
    }

    static void LoadProfiles(){
        if(profiles!=null)return;
        profiles=new Dictionary<string,Profile>(StringComparer.OrdinalIgnoreCase);
        adapterProfiles=new Dictionary<string,Profile>(StringComparer.OrdinalIgnoreCase);
        var data=Resources.Load<TextAsset>("Match3D/SourceColliders");
        if(data){var parsed=JsonUtility.FromJson<Data>(data.text);foreach(var p in parsed?.items??Array.Empty<Profile>())if(p!=null&&!string.IsNullOrEmpty(p.name))profiles[p.name]=p;}
        foreach(var id in AdapterIds())if(ItemCatalog.TryGetColliderProfile(id,out var source))adapterProfiles[id]=FromAdapter(source,id);
    }
    static IEnumerable<string> AdapterIds(){return ItemCatalog.GetIds();}
    static Profile FromAdapter(ItemCatalog.ColliderProfile source,string id){
        var p=new Profile{name=string.IsNullOrEmpty(source.name)?id:source.name,col=source.col,realCol=source.realCol};
        var nodes=source.nodes??Array.Empty<ItemCatalog.ColliderNode>();p.nodes=new Node[nodes.Length];
        for(int i=0;i<nodes.Length;i++){var n=nodes[i];var shapes=n.colliders??Array.Empty<ItemCatalog.ColliderShape>();p.nodes[i]=new Node{indices=n.indices,layer=n.layer,colliders=new Shape[shapes.Length]};for(int j=0;j<shapes.Length;j++){var s=shapes[j];p.nodes[i].colliders[j]=new Shape{type=s.type,enabled=s.enabled,trigger=s.trigger,hasMaterial=s.hasMaterial,center=s.center,size=s.size,radius=s.radius,height=s.height,direction=s.direction,material=s.material==null?null:new Surface{m_Name=s.material.m_Name,dynamicFriction=s.material.dynamicFriction,staticFriction=s.material.staticFriction,bounciness=s.material.bounciness,frictionCombine=s.material.frictionCombine,bounceCombine=s.material.bounceCombine}};}}
        return p;
    }
    public static void ClearCache(){profiles=null;ItemCatalog.ClearCache();}
}
