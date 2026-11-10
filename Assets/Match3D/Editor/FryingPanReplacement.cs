using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FryingPanReplacement {
    const string Model = "Assets/Match3D/Kitchen/Art/Grab3D/frying-pan.fbx";
    const string Scene = "Assets/Match3D/Resources/Match3D/Scenes/CountrysideStove.prefab";
    static FryingPanReplacement(){EditorApplication.update+=Poll;}
    static void Poll(){
        const string request="Temp/frying-pan-command";
        if(EditorApplication.isCompiling||EditorApplication.isUpdating||!File.Exists(request))return;
        var command=File.ReadAllText(request).Trim();File.Delete(request);
        try{if(command=="inspect")Inspect();else if(command=="build")Build();File.WriteAllText("Temp/frying-pan-result","OK "+command);}
        catch(Exception e){File.WriteAllText("Temp/frying-pan-result",e.ToString());Debug.LogException(e);}
    }
    [MenuItem("Goose/Art/Replace countryside frying pan")]
    public static void Build(){
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop play mode before replacing the prefab.");
        const string art="Assets/Match3D/Kitchen/Art/Grab3D/";
        var importer=(ModelImporter)AssetImporter.GetAtPath(Model);
        // AddPanCollision cooks this mesh at runtime. Editor-only CPU mesh data
        // hides a missing Read/Write flag until a player build is run.
        if(!importer.isReadable){importer.isReadable=true;importer.SaveAndReimport();}
        importer.ExtractTextures(art+"Textures");AssetDatabase.Refresh();
        var texturePaths=Directory.GetFiles(art+"Textures","*.jpg");
        foreach(var path in texturePaths){
            var ti=(TextureImporter)AssetImporter.GetAtPath(path);
            ti.maxTextureSize=1024;
            if(path.Contains("normal"))ti.textureType=TextureImporterType.NormalMap;
            else ti.sRGBTexture=path.Contains("basecolor");
            ti.SaveAndReimport();
        }
        var material=AssetDatabase.LoadAssetAtPath<Material>(art+"FryingPan.mat");
        if(!material){material=new Material(Shader.Find("Standard"));AssetDatabase.CreateAsset(material,art+"FryingPan.mat");}
        material.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(texturePaths.Single(p=>p.Contains("basecolor")));
        material.color=Color.white;
        material.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(texturePaths.Single(p=>p.Contains("normal"))));
        material.EnableKeyword("_NORMALMAP");material.SetFloat("_BumpScale",.5f);
        material.SetFloat("_Metallic",.25f);material.SetFloat("_Glossiness",.3f);
        EditorUtility.SetDirty(material);
        var root=PrefabUtility.LoadPrefabContents(Scene);
        try{
            var old=root.GetComponentsInChildren<Transform>(true).FirstOrDefault(t=>t.name=="skillet");
            if(old)old.gameObject.SetActive(false);
            var previous=root.transform.Find("Grab3D Frying Pan");if(previous)UnityEngine.Object.DestroyImmediate(previous.gameObject);
            var wrapper=new GameObject("Grab3D Frying Pan");wrapper.transform.SetParent(root.transform,false);
            var model=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Model),wrapper.transform);
            // The FBX includes its Blender-to-Unity axis correction. Preserve it.
            wrapper.transform.localScale=Vector3.one*(8.1f/.28f);
            wrapper.transform.localRotation=Quaternion.Euler(0,125,0);
            // Bowl centre is x=0.105m; the full FBX bounds also include the handle.
            wrapper.transform.localPosition=new Vector3(.05f,.36f,-.04f)-wrapper.transform.TransformVector(new Vector3(.105f,0,0));
            foreach(var r in model.GetComponentsInChildren<Renderer>()){r.sharedMaterial=material;r.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;r.receiveShadows=true;}
            PrefabUtility.SaveAsPrefabAsset(root,Scene);
            AssetDatabase.SaveAssets();
        }finally{PrefabUtility.UnloadPrefabContents(root);}
    }
    static void Inspect(){
        Directory.CreateDirectory("Assets/Match3D/Kitchen/Art/Grab3D/Textures");
        ((ModelImporter)AssetImporter.GetAtPath(Model)).ExtractTextures("Assets/Match3D/Kitchen/Art/Grab3D/Textures");
        AssetDatabase.Refresh();
        var lines=new System.Collections.Generic.List<string>();
        foreach(var path in new[]{Model,Scene}){
            var g=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));
            lines.Add(path);
            foreach(var t in g.GetComponentsInChildren<Transform>(true))if(path==Model||t.name=="skillet")lines.Add("transform "+t.name+" parent="+(t.parent?t.parent.name:"none")+" pos="+t.position+" rotation="+t.eulerAngles+" scale="+t.lossyScale);
            if(path==Model){var mf=g.GetComponentInChildren<MeshFilter>();File.WriteAllLines("Temp/frying-pan-vertices.csv",mf.sharedMesh.vertices.Select(v=>mf.transform.TransformPoint(v)).Select(v=>v.x+","+v.y+","+v.z));}
            foreach(var r in g.GetComponentsInChildren<Renderer>(true))lines.Add(r.name+" active="+r.gameObject.activeInHierarchy+" enabled="+r.enabled+" bounds="+r.bounds+" materials="+string.Join(",",r.sharedMaterials.Select(m=>m?m.name+"/"+m.shader.name+"/tex="+(m.mainTexture?m.mainTexture.name:"none"):"null")));
            foreach(var m in g.GetComponentsInChildren<MeshFilter>(true))lines.Add("mesh "+m.name+" "+m.sharedMesh.bounds+" transform="+m.transform.localEulerAngles+" scale="+m.transform.lossyScale);
            UnityEngine.Object.DestroyImmediate(g);
        }
        lines.Add("playing="+EditorApplication.isPlaying);
        File.WriteAllLines("Temp/frying-pan-inspect.txt",lines);
    }
}
