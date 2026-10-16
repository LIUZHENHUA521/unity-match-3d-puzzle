using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public static class FoodPileBuilder
{
    public const string ScenePath = "Assets/Scenes/FoodPileChallenge.unity";
    const string Root = "Assets/FoodPile";
    [Serializable] class MeshData {
        public string name, texture;
        public Vector3[] vertices, normals;
        public Vector2[] uv;
        public int[] triangles;
    }
    [MenuItem("Level/Build Food Pile Challenge")]
    public static void Build()
    {
        if (EditorApplication.isPlaying) throw new Exception("Stop Play mode before building.");
        Directory.CreateDirectory(Root+"/Generated");
        AssetDatabase.Refresh();
        string[] names = {"cheese","ice-cream","sandwich","coockie-man","toast","pancake-big","sandwich-toast","hotdog"};
        var prefabs = new GameObject[names.Length];
        for(int i=0;i<names.Length;i++) {
            var data = JsonUtility.FromJson<MeshData>(File.ReadAllText(Root+"/Editor/Source/"+names[i]+".json"));
            var mesh = new Mesh {name=data.name, vertices=data.vertices, normals=data.normals, uv=data.uv, triangles=data.triangles};
            mesh.RecalculateBounds(); mesh.RecalculateTangents();
            Save(mesh,Root+"/Generated/"+data.name+".asset");
            var texturePath = Root+"/Generated/"+data.texture;
            if (!File.Exists(texturePath)) File.Copy(Root+"/Editor/Source/"+data.texture,texturePath);
            AssetDatabase.ImportAsset(texturePath);
            var texImporter=(TextureImporter)AssetImporter.GetAtPath(texturePath);
            texImporter.maxTextureSize=1024; texImporter.textureCompression=TextureImporterCompression.Compressed; texImporter.SaveAndReimport();
            var material = new Material(Shader.Find("Standard")) {name=data.name+" food", enableInstancing=true};
            material.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            material.SetFloat("_Glossiness",.16f);
            Save(material,Root+"/Generated/"+data.name+".mat");
            var obj = new GameObject(data.name); obj.tag="Collectable";
            obj.AddComponent<MeshFilter>().sharedMesh=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/Generated/"+data.name+".asset");
            obj.AddComponent<MeshRenderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Root+"/Generated/"+data.name+".mat");
            var collider=obj.AddComponent<MeshCollider>(); collider.sharedMesh=obj.GetComponent<MeshFilter>().sharedMesh; collider.convex=true;
            var body=obj.AddComponent<Rigidbody>(); body.mass=.15f; body.drag=.6f; body.angularDrag=.8f;
            body.interpolation=RigidbodyInterpolation.Interpolate; body.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;
            body.solverIterations=10; body.solverVelocityIterations=4;
            prefabs[i]=PrefabUtility.SaveAsPrefabAsset(obj,Root+"/Generated/"+data.name+".prefab");
            UnityEngine.Object.DestroyImmediate(obj);
        }
        var settings = ScriptableObject.CreateInstance<FoodPileSettings>(); settings.prefabs=prefabs;
        Save(settings,Root+"/FoodPileSettings.asset");
        EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
        LevelGenerator.Instance.foodPile=AssetDatabase.LoadAssetAtPath<FoodPileSettings>(Root+"/FoodPileSettings.asset");
        var area=GameObject.Find("GameArea");
        foreach(Transform child in area.transform.Cast<Transform>().ToArray())
            if(child.name != "Stack") UnityEngine.Object.DestroyImmediate(child.gameObject);
        var tray=new GameObject("Cooking Pot"); tray.transform.position=new Vector3(0,0,.15f);
        var potMaterial=NewMaterial("Pot graphite enamel",new Color(.13f,.17f,.20f));
        var steel=NewMaterial("Pot steel handles",new Color(.52f,.56f,.59f));
        var potMesh=FoodPotMesh.Create(); Save(potMesh,Root+"/Generated/CookingPot.asset");
        potMesh=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/Generated/CookingPot.asset");
        tray.AddComponent<MeshFilter>().sharedMesh=potMesh;
        tray.AddComponent<MeshRenderer>().sharedMaterial=potMaterial;
        tray.AddComponent<MeshCollider>().sharedMesh=potMesh;
        // Tangential invisible walls keep the high mound inside the opening during shaking.
        for(int i=0;i<40;i++) {
            float angle=i*Mathf.PI*2/40;
            var wall=new GameObject("Pot containment");wall.layer=2;wall.transform.SetParent(tray.transform,false);
            wall.transform.localPosition=new Vector3(Mathf.Cos(angle)*1.09f,3,Mathf.Sin(angle)*1.09f);
            wall.transform.localRotation=Quaternion.Euler(0,-angle*Mathf.Rad2Deg,0);
            wall.AddComponent<BoxCollider>().size=new Vector3(.08f,6,.20f);
        }
        foreach(float side in new[]{-1f,1f}) {
            Box(tray.transform,"Handle stem",new Vector3(side*1.15f,.46f,.15f),new Vector3(.22f,.10f,.12f),steel,false);
            Box(tray.transform,"Handle grip",new Vector3(side*1.27f,.46f,.15f),new Vector3(.12f,.12f,.46f),potMaterial,false);
        }
        Camera.main.transform.position=new Vector3(0,12,0);
        Camera.main.farClipPlane=50;
        Camera.main.clearFlags=CameraClearFlags.SolidColor;
        Camera.main.backgroundColor=new Color(.075f,.12f,.14f);
        Camera.main.GetComponent<CameraFitter>().portraitViewport=true;
        RenderSettings.ambientLight=new Color(.75f,.75f,.75f);
        var shake=new GameObject("Shake Controller").AddComponent<FoodPileShake>();
        var canvas=UnityEngine.Object.FindObjectOfType<Canvas>();
        // Use the existing canvas camera and layout; new controls sit below the tray.
        var buttonObj=new GameObject("Shake Button",typeof(RectTransform),typeof(Image),typeof(Button));
        buttonObj.transform.SetParent(canvas.transform,false);
        var rect=(RectTransform)buttonObj.transform; rect.anchorMin=rect.anchorMax=new Vector2(.5f,1);
        rect.anchoredPosition=new Vector2(285,-230); rect.sizeDelta=new Vector2(330,70);
        buttonObj.GetComponent<Image>().color=new Color(.18f,.54f,.42f);
        shake.button=buttonObj.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(shake.button.onClick,shake.Shake);
        var label=Label(buttonObj.transform,"Shake label", "SHAKE  [SPACE]", 30);
        Stretch(label.rectTransform);
        var count=Label(canvas.transform,"Remaining", "96 ITEMS / 8 TYPES",30);
        count.rectTransform.anchorMin=count.rectTransform.anchorMax=new Vector2(.5f,1);
        count.rectTransform.anchoredPosition=new Vector2(-200,-230); count.rectTransform.sizeDelta=new Vector2(540,70);
        // English labels use the project's portable built-in font; keep user-facing documentation Chinese.
        shake.remainingText=count;
        var levelText=UIManager.Instance.levelText;
        levelText.fontSize=42; levelText.enableAutoSizing=false; levelText.enableWordWrapping=false;
        var titleRect=levelText.rectTransform; titleRect.anchorMin=titleRect.anchorMax=new Vector2(0,1);
        titleRect.pivot=new Vector2(0,1);titleRect.anchoredPosition=new Vector2(42,-30);titleRect.sizeDelta=new Vector2(340,80);
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);
        AssetDatabase.SaveAssets();
        if (File.Exists(Root+"/Editor/Source/strawberry.json")) StrawberryImporter.Build();
        Debug.Log("FOOD_PILE_BUILD_OK: 8 native food prefabs, 96-item settings, scene="+ScenePath);
    }
    static void Save(UnityEngine.Object obj,string path) {
        var existing=AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        if(existing) { EditorUtility.CopySerialized(obj,existing); UnityEngine.Object.DestroyImmediate(obj); }
        else AssetDatabase.CreateAsset(obj,path);
    }
    static Material NewMaterial(string name,Color color) {
        string path=Root+"/Generated/"+name+".mat";
        var mat=new Material(Shader.Find("Standard")){name=name,color=color}; mat.SetFloat("_Glossiness",.25f); Save(mat,path);
        return AssetDatabase.LoadAssetAtPath<Material>(path);
    }
    static void Box(Transform parent,string name,Vector3 pos,Vector3 scale,Material mat,bool solid) {
        var obj=GameObject.CreatePrimitive(PrimitiveType.Cube); obj.name=name; obj.transform.SetParent(parent);
        obj.transform.position=pos; obj.transform.localScale=scale; obj.GetComponent<Renderer>().sharedMaterial=mat;
        if(!solid) UnityEngine.Object.DestroyImmediate(obj.GetComponent<Collider>());
    }
    static void Wall(Transform parent,Vector3 pos,Vector3 size) {
        var obj=new GameObject("Containment");obj.transform.SetParent(parent);obj.transform.position=pos;obj.AddComponent<BoxCollider>().size=size;
    }
    static Text Label(Transform parent,string name,string value,int size) {
        var obj=new GameObject(name,typeof(RectTransform),typeof(Text));obj.transform.SetParent(parent,false);
        var text=obj.GetComponent<Text>();text.font=Resources.GetBuiltinResource<Font>("Arial.ttf");text.text=value;text.fontSize=size;text.alignment=TextAnchor.MiddleCenter;text.color=Color.white;text.raycastTarget=false;return text;
    }
    static void Stretch(RectTransform rect) { rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero; }
}
