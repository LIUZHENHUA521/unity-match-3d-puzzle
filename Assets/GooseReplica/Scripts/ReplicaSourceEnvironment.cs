using System;
using UnityEngine;

// Per-theme WX2.1.12 saved geometry; not a runtime capture.
public static class ReplicaSourceEnvironment {
    [Serializable] public class Border {public Vector3 position,scale;}
    [Serializable] class Data {public Node[] nodes;public Border[] borders;public float[] changeAngleHeight;}
    [Serializable] class Node {public string name;public bool active;public int layer,parent;public Vector3 position,scale;public Quaternion rotation;public Box[] boxes;}
    [Serializable] class Box {public Vector3 center,size;public bool enabled,trigger,hasMaterial;public Surface material;}
    [Serializable] class Surface {public string m_Name;public float dynamicFriction,staticFriction,bounciness;public int frictionCombine,bounceCombine;}
    static Data data;
    public static string CurrentName {get;private set;}="Breakfast";
    public static void Select(string name){
        string path=string.IsNullOrEmpty(name)?"Replica/SourceEnvironment":"Replica/Environments/"+name;
        var asset=Resources.Load<TextAsset>(path);
        if(!asset)throw new InvalidOperationException("Missing source environment: "+path);
        data=JsonUtility.FromJson<Data>(asset.text);CurrentName=string.IsNullOrEmpty(name)?"Breakfast":name;
    }
    static Data Source {get {if(data==null)data=JsonUtility.FromJson<Data>(Resources.Load<TextAsset>("Replica/SourceEnvironment").text);return data;}}
    public static Border GetBorder(float height){
        var borders=Source.borders;
        for(int i=borders.Length-1;i>=0;i--)if(borders[i].position.y<=height)return borders[i];
        return borders[0];
    }
    // Inverse integral of a linear density, equivalent to GetWeightRandom for these increasing weights.
    public static float WeightedHeight(float min,float max,float w0,float w1,double unit){
        if(Mathf.Approximately(w0,w1))return Mathf.Lerp(min,max,(float)unit);
        double t=unit*(w0+w1)/(Math.Sqrt(w0*w0+unit*(w1*w1-w0*w0))+w0);
        return Mathf.Lerp(min,max,(float)t);
    }
    public static Vector3 Position(float min,float max,System.Random random){
        float w0=GetBorder(min).scale.x,w1=GetBorder(max).scale.x;
        float low=min,high=max;
        // LoopCreateObj uses the first angle change strictly inside this height interval.
        foreach(float split in Source.changeAngleHeight)if(split>min&&split<max){
            float middle=GetBorder(split+.5f).scale.x;
            double left=(w0+middle)*(split-min),right=(middle+w1)*(max-split);
            if(random.NextDouble()*(left+right)<left){high=split;w1=middle;}
            else{low=split;w0=middle;}break;
        }
        float y=WeightedHeight(low,high,w0,w1,random.NextDouble());var border=GetBorder(y);
        return new Vector3(border.position.x+((float)random.NextDouble()*2-1)*border.scale.x*.45f,
                           y,border.position.z+((float)random.NextDouble()*2-1)*border.scale.z*.45f);
    }
    public static Quaternion Rotation(System.Random random)=>Quaternion.Euler((float)random.NextDouble()*360,(float)random.NextDouble()*360,(float)random.NextDouble()*360);
    // j16590, preserving the original float32 evaluation order and direct Range bounds.
    public static float WeightedHeight(float min,float max,float w0,float w1,IReplicaSourceRandom random){
        if(Mathf.Approximately(w0,w1))return random.Range(min,max);
        float sample=random.Range(0f,(w0+w1)*1f*.5f);
        float a=(sample+sample)/(w1-w0),b=(w0+w0)/(w1-w0);
        return min+(max-min)*(Mathf.Sqrt(a+b*.25f*b)-b*.5f);
    }
    public static Vector3 Position(float min,float max,IReplicaSourceRandom random){
        float w0=GetBorder(min).scale.x,w1=GetBorder(max).scale.x;
        float low=min,high=max;
        foreach(float split in Source.changeAngleHeight)if(split>min&&split<max){
            float middle=GetBorder(split+.5f).scale.x;
            // j29517 creates a new KFakeRandom, rolls once, then j16590 samples that interval.
            float left=(w0+middle)*(split-min),right=(middle+w1)*(max-split);
            if(random.Range(0f,left+right)<left){high=split;w1=middle;}
            else{low=split;w0=middle;}break;
        }
        float y=WeightedHeight(low,high,w0,w1,random);var border=GetBorder(y);
        // j13228 uses Range(-1,1), then scale * .45, X before Z.
        return new Vector3(border.position.x+random.Range(-1f,1f)*border.scale.x*.45f,
                           y,border.position.z+random.Range(-1f,1f)*border.scale.z*.45f);
    }
    public static Quaternion Rotation(IReplicaSourceRandom random)=>Quaternion.Euler(random.Range(0f,1f)*360f,random.Range(0f,1f)*360f,random.Range(0f,1f)*360f);
    public static GameObject CreateBoundary(){
        var nodes=Source.nodes;var objects=new GameObject[nodes.Length];
        for(int i=0;i<nodes.Length;i++)objects[i]=Build(nodes[i],nodes[i].parent<0?null:objects[nodes[i].parent].transform);
        return objects[0];
    }
    static GameObject Build(Node n,Transform parent){
        var obj=new GameObject(n.name);obj.layer=ReplicaSourceColliders.WallLayer;obj.transform.SetParent(parent,false);
        obj.transform.localPosition=n.position;obj.transform.localRotation=n.rotation;obj.transform.localScale=n.scale;
        foreach(var shape in n.boxes){var box=obj.AddComponent<BoxCollider>();box.center=shape.center;box.size=shape.size;box.enabled=shape.enabled;box.isTrigger=shape.trigger;
            box.contactOffset=ReplicaSourcePhysics.Active?ReplicaSourcePhysics.Contact:Physics.defaultContactOffset;
            if(shape.hasMaterial&&shape.material!=null){var m=shape.material;box.sharedMaterial=Material(m);}else if(ReplicaSourcePhysics.Active)box.sharedMaterial=ReplicaSourcePhysics.DefaultSurface;}
        obj.SetActive(n.active);return obj;
    }
    static readonly System.Collections.Generic.Dictionary<string,PhysicMaterial> materials=new System.Collections.Generic.Dictionary<string,PhysicMaterial>();
    static PhysicMaterial Material(Surface m){string key=JsonUtility.ToJson(m);if(!materials.TryGetValue(key,out var value)){value=new PhysicMaterial(m.m_Name??"Source wall surface"){dynamicFriction=m.dynamicFriction,staticFriction=m.staticFriction,bounciness=m.bounciness,frictionCombine=(PhysicMaterialCombine)m.frictionCombine,bounceCombine=(PhysicMaterialCombine)m.bounceCombine};materials.Add(key,value);}return value;}
}
