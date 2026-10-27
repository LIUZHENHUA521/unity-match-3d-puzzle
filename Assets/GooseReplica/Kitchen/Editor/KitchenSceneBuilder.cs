using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class KitchenSceneBuilder {
    const string Root="Assets/GooseReplica/Kitchen";
    public static void Inspect() {
        var report=new StringBuilder();
        var paths=AssetDatabase.FindAssets("t:Model",new[]{Root+"/Art"}).Select(AssetDatabase.GUIDToAssetPath)
            .Concat(new[]{"Assets/GooseReplica/Resources/Replica/Scenes/CountrysideStove.prefab"});
        foreach(var path in paths){
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(!prefab){report.AppendLine("MISSING "+path);continue;}
            var obj=UnityEngine.Object.Instantiate(prefab);
            try{
                report.AppendLine(path);
                foreach(var r in obj.GetComponentsInChildren<Renderer>(true))
                    report.AppendLine("  "+r.name+" center="+r.bounds.center.ToString("F3")+" size="+r.bounds.size.ToString("F3")+" mats="+string.Join(",",r.sharedMaterials.Select(m=>m?m.name+":"+(m.HasProperty("_Color")?m.color.ToString():"no color"):"null")));
                if(path.Contains("Polytus")) foreach(var mf in obj.GetComponentsInChildren<MeshFilter>()) {
                    var m=mf.sharedMesh;
                    for(int k=0;k<m.subMeshCount;k++){
                        var indices=m.GetIndices(k);var vertices=m.vertices;var b=new Bounds(mf.transform.TransformPoint(vertices[indices[0]]),Vector3.zero);
                        foreach(int index in indices)b.Encapsulate(mf.transform.TransformPoint(vertices[index]));
                        report.AppendLine("   submesh "+k+" center="+b.center.ToString("F4")+" size="+b.size.ToString("F4"));
                    }
                }
            }finally{UnityEngine.Object.DestroyImmediate(obj);}
        }
        File.WriteAllText("Temp/kitchen-inspect.txt",report.ToString());
    }

    public const string PrefabPath="Assets/GooseReplica/Resources/Replica/Scenes/Kitchen.prefab";
    [MenuItem("Goose/Kitchen/Rebuild Kitchen Background")]
    public static void Build() {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play before rebuilding kitchen");
        Directory.CreateDirectory(Root+"/Materials");AssetDatabase.Refresh();
        var root=new GameObject("Kitchen");
        try {
            var cream=Material("Countertop",new Color(.88f,.84f,.72f));
            var green=Material("SageCabinet",new Color(.32f,.49f,.40f));
            var darkGreen=Material("SageTrim",new Color(.20f,.34f,.28f));
            var tile=Material("IvoryTile",new Color(.82f,.88f,.83f));
            var grout=Material("Grout",new Color(.46f,.56f,.50f));
            var dark=Material("Cooktop",new Color(.095f,.115f,.12f));
            var steel=Material("BrushedSteel",new Color(.49f,.55f,.54f),.25f);
            var brass=Material("WarmMetal",new Color(.70f,.52f,.27f),.35f);
            var wood=Material("CuttingBoard",new Color(.65f,.40f,.20f));
            // All decoration is visual-only. The existing source boundary remains
            // responsible for food physics; no new solid collider spans the pan.
            Box(root,"Counter base",new Vector3(0,-4.6f,0),new Vector3(24,1.2f,28),green);
            Box(root,"Cream work surface",new Vector3(0,-3.9f,0),new Vector3(24,.32f,28),cream);
            Box(root,"Backsplash grout",new Vector3(0,-3.65f,6.3f),new Vector3(24,.12f,5),grout);
            for(int x=-9;x<=9;x++)for(int z=0;z<4;z++)
                Box(root,"Ceramic tile",new Vector3(x*1.25f+(z%2)*.625f,-3.56f,4.5f+z*1.2f),new Vector3(1.21f,.08f,1.16f),tile);
            Box(root,"Stove surround",new Vector3(0,-3.54f,-.1f),new Vector3(9.4f,.38f,9.2f),steel);
            Box(root,"Dark stove inset",new Vector3(0,-3.31f,-.1f),new Vector3(9.0f,.10f,8.8f),dark);
            for(int i=-1;i<=1;i++){
                var knob=GameObject.CreatePrimitive(PrimitiveType.Cylinder);knob.name="Stove control";knob.transform.SetParent(root.transform,false);
                knob.transform.localPosition=new Vector3(i*.75f,-3.15f,-4.05f);knob.transform.localScale=new Vector3(.30f,.10f,.30f);
                knob.GetComponent<Renderer>().sharedMaterial=brass;UnityEngine.Object.DestroyImmediate(knob.GetComponent<Collider>());
            }
            var pan=Model(root,"Art/Polytus/wok.fbx","Polytus black wok");
            // The FBX is exported with a world-space offset. Recenter on the
            // circular bowl (not the handle-inclusive overall bounds).
            pan.transform.localScale=Vector3.one*(8.0f/.3531f);
            pan.transform.localRotation=Quaternion.Euler(0,-55,0);
            pan.transform.localPosition=new Vector3(0,-3.18f,0)-pan.transform.localRotation*(new Vector3(1.75f,0,0)*pan.transform.localScale.x);
            foreach(var r in pan.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>{
                if(m.name.Contains("handle"))return wood;
                if(m.name.Contains("steel"))return steel;
                return Material("BlackWok",new Color(.075f,.083f,.087f),.18f);
            }).ToArray();
            PlaceFurniture(root,"kitchenSink",new Vector3(-3.15f,-5.75f,6.15f),5.1f,0,green,darkGreen,steel);
            PlaceFurniture(root,"kitchenCabinetDrawer",new Vector3(-.95f,-5.75f,6.15f),5.1f,0,green,darkGreen,steel);
            PlaceFurniture(root,"kitchenCabinet",new Vector3(1.25f,-5.75f,6.15f),5.1f,0,green,darkGreen,steel);
            PlaceFurniture(root,"kitchenStove",new Vector3(3.45f,-5.75f,6.15f),5.1f,0,green,darkGreen,steel);
            PlaceFurniture(root,"kitchenFridge",new Vector3(-5.5f,-5.75f,6.2f),5.1f,0,green,darkGreen,steel);
            PlaceFurniture(root,"kitchenCoffeeMachine",new Vector3(1.20f,-3.40f,6.10f),4.7f,0,green,darkGreen,steel);
            PlaceFurniture(root,"plantSmall1",new Vector3(-.95f,-3.40f,6.25f),6.2f,0,green,darkGreen,steel);
            PlaceFurniture(root,"kitchenBlender",new Vector3(3.6f,-3.40f,6.3f),4.8f,0,green,darkGreen,steel);
            Box(root,"Chopping board",new Vector3(4.9f,-3.63f,2.65f),new Vector3(1.55f,.16f,2.7f),wood);
            PlaceFurniture(root,"plantSmall1",new Vector3(-5.0f,-3.7f,2.1f),8,0,green,darkGreen,steel);
            foreach(var t in root.GetComponentsInChildren<Transform>(true))t.gameObject.layer=0;
            PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);AssetDatabase.SaveAssets();
            Debug.Log("KITCHEN_BUILT "+PrefabPath);
        }finally{UnityEngine.Object.DestroyImmediate(root);}
    }
    static Material Material(string name,Color color,float metallic=0) {
        string path=Root+"/Materials/"+name+".mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!mat){mat=new Material(Shader.Find("Standard"));AssetDatabase.CreateAsset(mat,path);}
        mat.color=color;mat.SetFloat("_Metallic",metallic);mat.SetFloat("_Glossiness",.23f);EditorUtility.SetDirty(mat);return mat;
    }
    static GameObject Model(GameObject root,string path,string name){
        var source=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/"+path);
        if(!source)throw new InvalidOperationException("Missing kitchen model: "+path);
        var obj=(GameObject)PrefabUtility.InstantiatePrefab(source);obj.name=name;obj.transform.SetParent(root.transform,false);return obj;
    }
    static void Box(GameObject root,string name,Vector3 position,Vector3 size,Material mat){
        var box=GameObject.CreatePrimitive(PrimitiveType.Cube);box.name=name;box.transform.SetParent(root.transform,false);
        box.transform.localPosition=position;box.transform.localScale=size;box.GetComponent<Renderer>().sharedMaterial=mat;
        UnityEngine.Object.DestroyImmediate(box.GetComponent<Collider>());
    }
    static void PlaceFurniture(GameObject root,string name,Vector3 position,float scale,float yaw,Material green,Material trim,Material steel){
        var obj=Model(root,"Art/Kenney/"+name+".obj",name);
        var bounds=new Bounds();bool first=true;
        foreach(var r in obj.GetComponentsInChildren<Renderer>()){if(first){bounds=r.bounds;first=false;}else bounds.Encapsulate(r.bounds);}
        obj.transform.localScale=Vector3.one*scale;obj.transform.localRotation=Quaternion.Euler(0,yaw,0);
        obj.transform.localPosition=position-obj.transform.localRotation*(new Vector3(bounds.center.x,bounds.min.y,bounds.center.z)*scale);
        foreach(var r in obj.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>{
            if(m.name=="wood")return green;if(m.name=="woodDark")return trim;
            if(m.name=="metal")return steel;
            return Material("Kenney_"+m.name,m.HasProperty("_Color")?m.color:Color.white);
        }).ToArray();
    }
}
