using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BoardLevelPlanImporter {
    [Serializable] class Config { public Theme[] themes; }
    [Serializable] class Theme { public string environment,group; public int sourceIndex,seconds; public float limitHeight; public string[] names,prefabPaths,tutorialNames; public float[] weights; public Layer[] layers; }
    [Serializable] class Layer {public int count,types;public float height;}
    const string Path="Assets/Match3D/Resources/Match3D/SourceLevelCatalog.asset";
    [MenuItem("Goose/Import Source Level Plans")]
    public static void Import(){
        var source=JsonUtility.FromJson<Config>(File.ReadAllText("Assets/Match3D/Resources/Match3D/SourceLevelConfig.json"));
        var profiles=source.themes.Select(t=>{
            if(t.names.Length!=t.weights.Length)throw new InvalidOperationException("Source name/weight mismatch "+t.environment);
            bool customPaths=t.prefabPaths!=null && t.prefabPaths.Length>0;
            if(customPaths && t.prefabPaths.Length!=t.names.Length)throw new InvalidOperationException("Prefab path/name mismatch "+t.environment);
            var prefabs=t.names.Select((name,i)=>AssetDatabase.LoadAssetAtPath<GameObject>(customPaths?t.prefabPaths[i]:"Assets/Match3D/OriginalModels/Prefabs/"+t.group+"/"+name+".prefab")).ToArray();
            if(t.tutorialNames!=null && t.tutorialNames.Any(name=>!prefabs.Any(p=>p && p.name==name)))throw new InvalidOperationException("Missing tutorial prefab "+t.environment);
            var missing=t.names.Where((name,i)=>!prefabs[i]).ToArray();
            if(missing.Length>0)throw new InvalidOperationException("Missing source prefabs: "+string.Join(", ",missing));
            return new LevelLayoutConfig.Profile{environment=t.environment,sourceIndex=t.sourceIndex,seconds=t.seconds,limitHeight=t.limitHeight,prefabs=prefabs,tutorialNames=t.tutorialNames,
                layers=t.layers.Select(l=>new ChallengeLayer(l.count,l.types,l.height){weights=(float[])t.weights.Clone()}).ToArray()};
        }).ToArray();
        var asset=AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>(Path);
        if(!asset){asset=ScriptableObject.CreateInstance<LevelLayoutConfig>();AssetDatabase.CreateAsset(asset,Path);}
        asset.profiles=profiles;EditorUtility.SetDirty(asset);AssetDatabase.SaveAssets();
        Debug.Log("SOURCE_LEVEL_PLANS_IMPORTED: "+string.Join(", ",profiles.Select(x=>x.environment+"="+x.prefabs.Length)));
    }
    [MenuItem("Goose/Import First Stage Plans")]
    public static void ImportFirstStage(){
        string[] environments={"CountrysideStove"};
        string[] groups={"Countryside"};
        string[][] names={new[]{"Goose_Tomato","Potato","Orange","SaltedEgg"}};
        const string path="Assets/Match3D/Resources/Match3D/FirstStageCatalog.asset";
        var asset=AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>(path);
        if(!asset){asset=ScriptableObject.CreateInstance<LevelLayoutConfig>();AssetDatabase.CreateAsset(asset,path);}
        asset.profiles=Enumerable.Range(0,environments.Length).Select(i=>{
            var prefabs=names[i].Select(n=>AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/OriginalModels/Prefabs/"+groups[i]+"/"+n+".prefab")).ToArray();
            if(prefabs.Any(p=>!p))throw new InvalidOperationException("Missing first-stage original prefab "+groups[i]);
            return new LevelLayoutConfig.Profile{environment=environments[i],sourceIndex=new[]{1,4,5}[i],seconds=0,limitHeight=4,prefabs=prefabs,
                ordinaryTypes=Enumerable.Range(1,prefabs.Length-1).ToArray(),layers=new[]{new ChallengeLayer(12,2,6f){weights=Enumerable.Repeat(1f,prefabs.Length-1).ToArray()}}};
        }).ToArray();
        EditorUtility.SetDirty(asset);AssetDatabase.SaveAssets();Debug.Log("FIRST_STAGE_PLANS_IMPORTED");
    }
}
