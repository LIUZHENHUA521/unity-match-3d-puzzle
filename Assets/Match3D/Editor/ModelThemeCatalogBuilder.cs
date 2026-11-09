using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ModelThemeCatalogBuilder {
    static ModelThemeCatalogBuilder(){EditorApplication.update+=Poll;}
    static void Poll(){
        const string command="Temp/model-themes-command";
        if(EditorApplication.isCompiling||EditorApplication.isUpdating||!File.Exists(command))return;
        var action=File.ReadAllText(command).Trim();File.Delete(command);
        try{if(action=="build")Build();else if(action=="validate")ModelThemeValidation.Start();else throw new Exception("Unknown action");File.WriteAllText("Temp/model-themes-result","OK "+action);}
        catch(Exception e){File.WriteAllText("Temp/model-themes-result",e.ToString());Debug.LogException(e);}
    }
    public static void Build(){
        if(EditorApplication.isPlaying)throw new Exception("Stop Play first");
        const string path="Assets/Match3D/Resources/Match3D/ModelThemes.asset";
        var asset=AssetDatabase.LoadAssetAtPath<Match3DModelThemes>(path);
        if(!asset)asset=ScriptableObject.CreateInstance<Match3DModelThemes>();
        var ids=new[]{"playful-toys","tasty-kitchen","foodie-party"};
        var titles=new[]{"萌趣玩具","美味厨房","吃货派对"};
        var prefixes=new[]{"Toy","Kitchen_","Party_"};
        // Reconstruct each original ordered pool and tutorial choices from the saved import snapshot.
        var configs=new[]{"work/tasty-kitchen/before/SourceLevelConfig.json","work/foodie-party/before/SourceLevelConfig.json","Assets/Match3D/Resources/Match3D/SourceLevelConfig.json"};
        asset.themes=ids.Select((id,i)=>{
            var json=JObject.Parse(File.ReadAllText(configs[i]))["themes"].Single(t=>(string)t["environment"]=="CountrysideStove");
            var template=AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>(ThemeItemsEditorService.CatalogPath).profiles.Single(p=>p.environment=="CountrysideStove");
            var profile=JsonUtility.FromJson<LevelLayoutConfig.Profile>(JsonUtility.ToJson(template));
            profile.prefabs=json["prefabPaths"].Select(v=>AssetDatabase.LoadAssetAtPath<GameObject>((string)v)).ToArray();
            profile.tutorialNames=json["tutorialNames"].Select(v=>(string)v).ToArray();
            if(profile.prefabs.Length!=23||profile.prefabs.Any(p=>!p||!p.name.StartsWith(prefixes[i])||ThemeItemsEditorService.CheckPrefab(p)!=null))throw new Exception("Invalid models: "+id);
            LevelLayoutConfig.GetTutorial(profile);
            return new Match3DModelThemes.Entry{id=id,title=titles[i],profile=profile,ambientMultiplier=i==0?.65f:i==1?.75f:1f,lightMultiplier=i==0?.8f:i==1?.85f:1f};
        }).ToArray();
        if(!AssetDatabase.Contains(asset))AssetDatabase.CreateAsset(asset,path);
        EditorUtility.SetDirty(asset);AssetDatabase.SaveAssetIfDirty(asset);
    }
}
