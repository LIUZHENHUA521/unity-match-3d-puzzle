using System;
using System.IO;
using UnityEditor;
using UnityEngine;
public static class StrawberryImporter {
    [Serializable] class Data { public Vector3[] vertices,normals; public Vector2[] uv; public int[] triangles; }
    [MenuItem("Level/Import Meshy Strawberry")]
    public static void Build() {
        const string root="Assets/FoodPile/";
        AssetDatabase.Refresh();
        var d=JsonUtility.FromJson<Data>(File.ReadAllText(root+"Editor/Source/strawberry.json"));
        var mesh=new Mesh{name="strawberry",vertices=d.vertices,normals=d.normals,uv=d.uv,triangles=d.triangles};
        mesh.RecalculateBounds();mesh.RecalculateTangents();
        Save(mesh,root+"Generated/strawberry.asset");
        File.Copy(root+"Editor/Source/strawberry.png",root+"Generated/strawberry.png",true);
        AssetDatabase.ImportAsset(root+"Generated/strawberry.png");
        var ti=(TextureImporter)AssetImporter.GetAtPath(root+"Generated/strawberry.png");
        ti.maxTextureSize=512;ti.textureCompression=TextureImporterCompression.Compressed;ti.mipmapEnabled=true;ti.SaveAndReimport();
        var mat=new Material(Shader.Find("Standard")){name="strawberry food",enableInstancing=true};
        mat.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(root+"Generated/strawberry.png");
        mat.SetFloat("_Glossiness",.12f);mat.SetFloat("_Metallic",0);
        Save(mat,root+"Generated/strawberry.mat");
        var obj=new GameObject("strawberry");obj.tag="Collectable";
        obj.AddComponent<MeshFilter>().sharedMesh=AssetDatabase.LoadAssetAtPath<Mesh>(root+"Generated/strawberry.asset");
        obj.AddComponent<MeshRenderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(root+"Generated/strawberry.mat");
        // Smooth fruit-shaped collider avoids fragile leaf geometry in the physics pile.
        var c=obj.AddComponent<CapsuleCollider>();c.radius=.16f;c.height=.37f;c.center=new Vector3(0,-.04f,0);
        var b=obj.AddComponent<Rigidbody>();b.mass=.15f;b.drag=.6f;b.angularDrag=.8f;b.interpolation=RigidbodyInterpolation.Interpolate;
        b.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;b.solverIterations=10;b.solverVelocityIterations=4;
        var prefab=PrefabUtility.SaveAsPrefabAsset(obj,root+"Generated/strawberry.prefab");UnityEngine.Object.DestroyImmediate(obj);
        var settings=AssetDatabase.LoadAssetAtPath<FoodPileSettings>(root+"FoodPileSettings.asset");
        settings.prefabs[4]=prefab;EditorUtility.SetDirty(settings);AssetDatabase.SaveAssets();
        Debug.Log("STRAWBERRY_IMPORT_OK: "+d.triangles.Length/3+" triangles; 512 texture; replaces toast, 12 strawberries / 96 items.");
    }
    static void Save(UnityEngine.Object obj,string path) {
        var old=AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        if(old){EditorUtility.CopySerialized(obj,old);UnityEngine.Object.DestroyImmediate(obj);}else AssetDatabase.CreateAsset(obj,path);
    }
}
