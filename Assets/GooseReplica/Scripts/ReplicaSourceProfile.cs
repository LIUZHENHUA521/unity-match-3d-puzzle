using System;
using System.Collections.Generic;
using UnityEngine;

// Legacy breakfast radius helper for historical diagnostics only. Runtime coordinates and walls now
// come from ReplicaSourceEnvironment per theme. Shared visibility and model-size values remain here.
public static class ReplicaSourceProfile {
    public const float VisibleMin=4,VisibleMax=5,MinVisibleY=21,MaxHeightStep=.5f,GrowSeconds=.75f,OptimizationInterval=1f;
    public static readonly float[] Heights={0,3,6,9,12,15,18,21,24,30};
    public static readonly float[] Widths={1.399999976f,1.620112658f,1.840225339f,2.060338020f,2.280450583f,3.173188448f,4.065925598f,4.958663464f,5.851400852f,5.851400852f};
    public static float HalfExtent(float height){
        for(int i=1;i<Heights.Length;i++)if(height<=Heights[i])return Mathf.Lerp(Widths[i-1],Widths[i],Mathf.InverseLerp(Heights[i-1],Heights[i],height))*.45f;
        return Widths[Widths.Length-1]*.45f;
    }
    [Serializable] class Sizes {public Size[] items;}
    [Serializable] class Size {public string name;public float normalizedToSource;}
    static Dictionary<string,float> sizes;
    public const string ModelSizeKey="goose.modelSizeMultiplier";
    public static float ModelSizeMultiplier {
        get => Mathf.Clamp(PlayerPrefs.GetFloat(ModelSizeKey,1f),.5f,1.25f);
        set {PlayerPrefs.SetFloat(ModelSizeKey,Mathf.Round(Mathf.Clamp(value,.5f,1.25f)*100f)/100f);PlayerPrefs.Save();}
    }
    public static float ModelScale(string name){
        if(ReplicaSourceCatalog.TryGetSize(name,out float catalogValue))return catalogValue*ModelSizeMultiplier;
        if(sizes==null){sizes=new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase);var data=Resources.Load<TextAsset>("Replica/SourceSizes");if(data)foreach(var item in JsonUtility.FromJson<Sizes>(data.text).items)sizes[item.name]=item.normalizedToSource;}
        return (sizes.TryGetValue(name,out float value)?value:ReplicaFlow.BoardItemScale)*ModelSizeMultiplier;
    }
    public static void ClearCache(){sizes=null;ReplicaSourceCatalog.ClearCache();}
}
