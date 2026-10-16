using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SourceLevelPlanImporter {
    [Serializable] class Config { public Theme[] themes; }
    [Serializable] class Theme { public string environment,group; public int sourceIndex,seconds; public float limitHeight; public string[] names; public float[] weights; public Layer[] layers; }
    [Serializable] class Layer {public int count,types;public float height;}
    const string Path="Assets/GooseReplica/Resources/Replica/SourceLevelCatalog.asset";
    [MenuItem("Goose/Import Source Level Plans")]
    public static void Import(){
        var source=JsonUtility.FromJson<Config>(File.ReadAllText("Assets/GooseReplica/Resources/Replica/SourceLevelConfig.json"));
        var profiles=source.themes.Select(t=>{
            if(t.names.Length!=t.weights.Length)throw new InvalidOperationException("Source name/weight mismatch "+t.environment);
            var prefabs=t.names.Select(name=>AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GooseReplica/OriginalModels/Prefabs/"+t.group+"/"+name+".prefab")).ToArray();
            var missing=t.names.Where((name,i)=>!prefabs[i]).ToArray();
            if(missing.Length>0)throw new InvalidOperationException("Missing source prefabs: "+string.Join(", ",missing));
            return new ReplicaSourceLevelPlan.Profile{environment=t.environment,sourceIndex=t.sourceIndex,seconds=t.seconds,limitHeight=t.limitHeight,prefabs=prefabs,
                layers=t.layers.Select(l=>new ChallengeLayer(l.count,l.types,l.height){weights=(float[])t.weights.Clone()}).ToArray()};
        }).ToArray();
        var asset=AssetDatabase.LoadAssetAtPath<ReplicaSourceLevelPlan>(Path);
        if(!asset){asset=ScriptableObject.CreateInstance<ReplicaSourceLevelPlan>();AssetDatabase.CreateAsset(asset,Path);}
        asset.profiles=profiles;EditorUtility.SetDirty(asset);AssetDatabase.SaveAssets();
        Debug.Log("SOURCE_LEVEL_PLANS_IMPORTED: "+string.Join(", ",profiles.Select(x=>x.environment+"="+x.prefabs.Length)));
    }
}
