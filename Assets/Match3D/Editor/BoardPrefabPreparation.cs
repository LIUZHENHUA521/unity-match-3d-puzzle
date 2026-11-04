using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Bake the runtime profile once in the editor, rather than while items appear.
public static class BoardPrefabPreparation {
    const string Materials="Assets/Match3D/SourcePhysicsMaterials";
    [MenuItem("Goose/Prepare Creation Prefabs")]
    public static void Bake(){
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play mode before preparing prefabs");
        var plan=AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>("Assets/Match3D/Resources/Match3D/SourceLevelCatalog.asset");
        if(!plan)throw new InvalidOperationException("Missing SourceLevelCatalog");
        Directory.CreateDirectory(Materials);AssetDatabase.Refresh();ItemColliderGeometry.ClearCache();
        var visited=new HashSet<string>();int count=0;
        var first=AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>("Assets/Match3D/Resources/Match3D/FirstStageCatalog.asset");
        foreach(var profile in plan.profiles.Concat(first?first.profiles:Array.Empty<LevelLayoutConfig.Profile>()))foreach(var prefab in profile.prefabs){
            string path=AssetDatabase.GetAssetPath(prefab);
            if(string.IsNullOrEmpty(path))throw new InvalidOperationException("Missing prefab in "+profile.environment);
            if(!visited.Add(path))continue;
            var root=PrefabUtility.LoadPrefabContents(path);
            try{
                // Keep authored visual pivot: runtime Apply(restorePivot=true)
                // normalizes only the source board, not legacy/PVP instances.
                ItemColliderGeometry.Apply(root,prefab.name,false,true);
                var cache=root.GetComponent<ItemColliderGeometry>();
                if(!cache||!cache.IsConfiguredFor(prefab.name))throw new InvalidOperationException("Missing collider profile "+path);
                foreach(var collider in root.GetComponentsInChildren<Collider>(true)){
                    var material=collider.sharedMaterial;
                    if(!material||AssetDatabase.Contains(material))continue;
                    string key=material.dynamicFriction.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+"/"+
                        material.staticFriction.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+"/"+
                        material.bounciness.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+"/"+(int)material.frictionCombine+"/"+(int)material.bounceCombine;
                    string target=Materials+"/"+Hash128.Compute(key)+".physicMaterial";
                    var saved=AssetDatabase.LoadAssetAtPath<PhysicMaterial>(target);
                    if(!saved){saved=UnityEngine.Object.Instantiate(material);saved.name=material.name;AssetDatabase.CreateAsset(saved,target);}
                    collider.sharedMaterial=saved;
                }
                PrefabUtility.SaveAsPrefabAsset(root,path);count++;
            }finally{PrefabUtility.UnloadPrefabContents(root);}
        }
        AssetDatabase.SaveAssets();
        Debug.Log("SOURCE_CREATION_PREFABS_PREPARED "+count);
    }
}
