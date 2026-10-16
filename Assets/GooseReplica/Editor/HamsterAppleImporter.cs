using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public static class HamsterAppleImporter {
    const string Root="Assets/GooseReplica/Generated/HamsterApple/";
    const string Source="Assets/GooseReplica/Editor/MeshySource/";
    [Serializable] class Data {public Vector3[] vertices,normals;public Vector2[] uv;public int[] triangles;}
    [MenuItem("Goose/Import Meshy Apple")]
    public static void Build(){
        if(EditorApplication.isPlaying)throw new Exception("Stop Play mode before importing");
        Directory.CreateDirectory(Root);AssetDatabase.Refresh();
        var d=JsonUtility.FromJson<Data>(File.ReadAllText(Source+"hamster_apple.json"));
        var mesh=new Mesh{name="Hamster Apple",vertices=d.vertices,normals=d.normals,uv=d.uv,triangles=d.triangles};mesh.RecalculateBounds();mesh.RecalculateTangents();Save(mesh,Root+"apple.asset");
        File.Copy(Source+"hamster_apple.png",Root+"apple.png",true);AssetDatabase.ImportAsset(Root+"apple.png");
        var ti=(TextureImporter)AssetImporter.GetAtPath(Root+"apple.png");ti.maxTextureSize=1024;ti.mipmapEnabled=false;ti.textureCompression=TextureImporterCompression.Compressed;ti.SaveAndReimport();
        var mat=new Material(Shader.Find("Goose/Original Item")){name="Hamster Apple",enableInstancing=true};mat.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"apple.png");mat.SetColor("_Color",Color.white);mat.SetFloat("_HasAO",0);Save(mat,Root+"apple.mat");
        var g=new GameObject("hamster_apple");g.tag="Collectable";g.AddComponent<MeshFilter>().sharedMesh=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"apple.asset");g.AddComponent<MeshRenderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Root+"apple.mat");
        var c=g.AddComponent<SphereCollider>();c.radius=.145f;c.center=new Vector3(0,-.038f,0);
        var b=g.AddComponent<Rigidbody>();b.mass=.15f;b.drag=.6f;b.angularDrag=.8f;b.interpolation=RigidbodyInterpolation.Interpolate;b.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;b.solverIterations=10;
        var prefab=PrefabUtility.SaveAsPrefabAsset(g,Root+"hamster_apple.prefab");UnityEngine.Object.DestroyImmediate(g);
        var library=AssetDatabase.LoadAssetAtPath<ReplicaTheme>("Assets/GooseReplica/Resources/Replica/ThemeLibrary.asset");
        int index=Array.FindIndex(library.themes[0].prefabs,p=>p.name=="Apple"||p.name=="hamster_apple"||p.name=="apple");if(index<0)throw new Exception("Original apple slot not found");library.themes[0].prefabs[index]=prefab;
        EditorUtility.SetDirty(library);AssetDatabase.SaveAssets();File.WriteAllText("Temp/hamster-apple-import.txt","PASS imported "+d.triangles.Length/3+" triangles; 1024 texture; fruit-body collider; daily pool apple slot="+index);Debug.Log("HAMSTER_APPLE_IMPORT_OK");
    }
    static void Save(UnityEngine.Object value,string path){var old=AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);if(old){EditorUtility.CopySerialized(value,old);UnityEngine.Object.DestroyImmediate(value);}else AssetDatabase.CreateAsset(value,path);}
    [MenuItem("Goose/Preview Meshy Apple In Game")]
    public static void Preview(){
        if(!EditorApplication.isPlaying||!ReplicaFlow.Instance)throw new Exception("Play GooseReplica scene first");
        var flow=ReplicaFlow.Instance;flow.Begin(0);var pool=flow.library.themes[0].prefabs;int apple=Array.FindIndex(pool,p=>p.name=="hamster_apple"),corn=Array.FindIndex(pool,p=>p.name=="Corn");if(apple<0||corn<0)throw new Exception("Missing sample prefabs");
        Action<string,object[]> invoke=(name,args)=>typeof(ReplicaFlow).GetMethod(name,BindingFlags.NonPublic|BindingFlags.Instance).Invoke(flow,args);
        invoke("StopBoard",null);var tiles=new List<ChallengeTile>();for(int i=0;i<12;i++)tiles.Add(new ChallengeTile{id=i,type=i%2==0?apple:corn,layer=0,height=.65f});invoke("Spawn",new object[]{tiles,12345,120});invoke("HUD",null);
    }
}
