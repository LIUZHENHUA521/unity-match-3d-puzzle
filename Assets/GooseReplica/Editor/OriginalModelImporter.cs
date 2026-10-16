using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
public static class OriginalModelImporter {
    const string Root="Assets/GooseReplica/OriginalModels";
    [Serializable] class Export {public Model[] models;public MeshData[] meshes;public MaterialData[] materials;public TextureInfo[] textureInfos;}
    [Serializable] class Model {public string name,group,source,bundle;public bool legacy;public Node root;}
    [Serializable] class Node {public string name,mesh,transformId,rootBoneId;public string[] boneIds;public bool skinned,updateWhenOffscreen,skinnedMotionVectors;public int skinQuality;public Vector3 boundsCenter,boundsExtents;public bool active,rendererEnabled,receiveShadows;public int castShadows;public Vector3 position,scale;public Quaternion rotation;public string[] materials;public Collision[] colliders;public Node[] children;}
    [Serializable] class Collision {public string type;public Vector3 center,size;public float radius,height;public int direction;}
    [Serializable] class MeshData {public string id,name;public Vector3[] vertices,normals;public Vector4[] tangents;public Vector2[] uv;public Submesh[] submeshes;public SkinWeight[] boneWeights;public BindPose[] bindposes;}
    [Serializable] class SkinWeight {public float weight0,weight1,weight2,weight3;public int boneIndex0,boneIndex1,boneIndex2,boneIndex3;}
    [Serializable] class BindPose {public float[] values;}
    [Serializable] class Submesh {public int[] indices;}
    [Serializable] class MaterialData {public string id,name,texture,ao,portedShader;public Color tint;public Vector2 scale,offset;public int renderQueue;public FloatValue[] floats,ints;public VectorValue[] vectors;public TextureValue[] textures;public int lightmapFlags;public bool enableInstancingVariants,doubleSidedGI;}
    [Serializable] class FloatValue {public string name;public float value;}
    [Serializable] class VectorValue {public string name;public Color value;}
    [Serializable] class TextureValue {public string name,texture;public Vector2 scale,offset;}
    [Serializable] class TextureInfo {public string file;public bool srgb;public int width,height,mipCount,filterMode,aniso,wrapU,wrapV,wrapW;public float mipBias;}
    static Dictionary<string,Mesh> meshes;static Dictionary<string,Material> materials;
    [MenuItem("Goose/Import Original 3D Items")]
    public static void Import(){
        if(EditorApplication.isPlaying)throw new Exception("Stop Play mode before importing models");
        var data=SourceModelJson.Read<Export>(File.ReadAllText(Root+"/models.json"));
        foreach(var dir in new[]{"Meshes","Materials","Prefabs"})Directory.CreateDirectory(Root+"/"+dir);
        AssetDatabase.Refresh();ConfigureTextures(data);meshes=new Dictionary<string,Mesh>();materials=new Dictionary<string,Material>();
        foreach(var d in data.meshes){
            var m=new Mesh{name=d.name,indexFormat=d.vertices.Length>65535?IndexFormat.UInt32:IndexFormat.UInt16};m.vertices=d.vertices;m.uv=d.uv;
            if(d.normals.Length==d.vertices.Length)m.normals=d.normals;m.subMeshCount=d.submeshes.Length;
            for(int i=0;i<d.submeshes.Length;i++)m.SetTriangles(d.submeshes[i].indices,i);
            if(d.normals.Length!=d.vertices.Length)m.RecalculateNormals();m.tangents=d.tangents;ApplyMeshSkin(m,d);m.RecalculateBounds();meshes[d.id]=Save(m,Root+"/Meshes/"+d.id+".asset");
        }
        foreach(var d in data.materials){
            materials[d.id]=Save(SourceMaterial(d),Root+"/Materials/"+d.id+".mat");
        }
        var groups=new Dictionary<string,List<GameObject>>();
        foreach(var model in data.models){
            var wrapper=new GameObject(model.name);wrapper.tag="Collectable";var visual=Build(model.root,wrapper.transform);BindSkins(model.root,visual.transform);visual.transform.localPosition=Vector3.zero;
            var renderers=wrapper.GetComponentsInChildren<Renderer>().Where(x=>x.enabled).ToArray();if(renderers.Length==0)throw new Exception("No renderer: "+model.name);
            var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);float max=Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
            if(max<=0||float.IsNaN(max))throw new Exception("Invalid bounds: "+model.name);
            float scale=.43f/max;visual.transform.localScale*=scale;visual.transform.localPosition=-bounds.center*scale;
            if(wrapper.GetComponentsInChildren<Collider>().Length==0){var box=wrapper.AddComponent<BoxCollider>();box.size=bounds.size*scale;}
            var body=wrapper.AddComponent<Rigidbody>();body.mass=.15f;body.drag=.6f;body.angularDrag=.8f;body.interpolation=RigidbodyInterpolation.Interpolate;body.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;
            string folder=Root+"/Prefabs/"+model.group;Directory.CreateDirectory(folder);var prefab=PrefabUtility.SaveAsPrefabAsset(wrapper,folder+"/"+model.name+".prefab");UnityEngine.Object.DestroyImmediate(wrapper);
            if(!model.legacy){if(!groups.ContainsKey(model.group))groups[model.group]=new List<GameObject>();groups[model.group].Add(prefab);}
        }
        var library=AssetDatabase.LoadAssetAtPath<ReplicaTheme>("Assets/GooseReplica/Resources/Replica/ThemeLibrary.asset");
        string[] categories={"Countryside","GroceryStore","Barbecue"},titles={"乡村每日挑战","小卖部","烧烤摊"},subtitles={"原版乡村物品，每天一道挑战","零食日用品，清空小卖部","找齐烤串，挑战烧烤摊"};
        for(int i=0;i<3;i++){library.themes[i].prefabs=groups[categories[i]].ToArray();library.themes[i].title=titles[i];library.themes[i].subtitle=subtitles[i];}
        EditorUtility.SetDirty(library);AssetDatabase.SaveAssets();AssetDatabase.Refresh();
        Debug.Log("ORIGINAL_MODELS_IMPORTED "+data.models.Length+" prefabs, "+data.meshes.Length+" meshes, "+data.materials.Length+" materials. Pools: "+string.Join(",",categories.Select(x=>x+"="+groups[x].Count)));
    }
    // Add newly recovered prefabs without rebuilding the existing 71 or changing tutorial/PVP pools.
    public static void ImportMissingSourceModels(){
        if(EditorApplication.isPlaying)throw new Exception("Stop Play mode before importing models");
        var data=SourceModelJson.Read<Export>(File.ReadAllText(Root+"/models.json"));
        foreach(var dir in new[]{"Meshes","Materials","Prefabs"})Directory.CreateDirectory(Root+"/"+dir);
        AssetDatabase.Refresh();ConfigureTextures(data);meshes=new Dictionary<string,Mesh>();materials=new Dictionary<string,Material>();
        foreach(var d in data.meshes){
            string path=Root+"/Meshes/"+d.id+".asset";var m=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(!m){m=new Mesh{name=d.name,indexFormat=d.vertices.Length>65535?IndexFormat.UInt32:IndexFormat.UInt16};m.vertices=d.vertices;m.uv=d.uv;
                if(d.normals.Length==d.vertices.Length)m.normals=d.normals;m.subMeshCount=d.submeshes.Length;
                for(int i=0;i<d.submeshes.Length;i++)m.SetTriangles(d.submeshes[i].indices,i);
                if(d.normals.Length!=d.vertices.Length)m.RecalculateNormals();m.tangents=d.tangents;ApplyMeshSkin(m,d);m.RecalculateBounds();m=Save(m,path);}
            if(d.boneWeights!=null&&d.boneWeights.Length>0){ApplyMeshSkin(m,d);EditorUtility.SetDirty(m);}
            meshes[d.id]=m;
        }
        foreach(var d in data.materials){string path=Root+"/Materials/"+d.id+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);materials[d.id]=m?m:Save(SourceMaterial(d),path);}
        int count=0;
        foreach(var model in data.models){
            string folder=Root+"/Prefabs/"+model.group,path=folder+"/"+model.name+".prefab";
            var existing=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(existing&&(!HasSkin(model.root)||existing.GetComponentInChildren<SkinnedMeshRenderer>(true)))continue;
            var wrapper=new GameObject(model.name);wrapper.tag="Collectable";
            try{
                var visual=Build(model.root,wrapper.transform);BindSkins(model.root,visual.transform);visual.transform.localPosition=Vector3.zero;
                var renderers=wrapper.GetComponentsInChildren<Renderer>().Where(x=>x.enabled).ToArray();if(renderers.Length==0)throw new Exception("No renderer: "+model.name);
                var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);float max=Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
                if(max<=0||float.IsNaN(max))throw new Exception("Invalid bounds: "+model.name);
                float scale=.43f/max;visual.transform.localScale*=scale;visual.transform.localPosition=-bounds.center*scale;
                if(wrapper.GetComponentsInChildren<Collider>().Length==0){var box=wrapper.AddComponent<BoxCollider>();box.size=bounds.size*scale;}
                var body=wrapper.AddComponent<Rigidbody>();body.mass=.15f;body.drag=.6f;body.angularDrag=.8f;body.interpolation=RigidbodyInterpolation.Interpolate;body.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;
                Directory.CreateDirectory(folder);PrefabUtility.SaveAsPrefabAsset(wrapper,path);count++;
            }finally{UnityEngine.Object.DestroyImmediate(wrapper);}
        }
        AssetDatabase.SaveAssets();AssetDatabase.Refresh();Debug.Log("MISSING_SOURCE_MODELS_IMPORTED "+count);
    }
    // Updates stable material/mesh asset GUIDs; leaves prefab geometry, transforms and physics intact.
    public static void ApplySourceMaterials(){
        var data=SourceModelJson.Read<Export>(File.ReadAllText(Root+"/models.json"));
        AssetDatabase.Refresh();ConfigureTextures(data);
        foreach(var d in data.meshes){var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/Meshes/"+d.id+".asset");
            if(!mesh||mesh.vertexCount!=d.tangents.Length)throw new Exception("Missing source tangent correspondence "+d.id);
            mesh.tangents=d.tangents;EditorUtility.SetDirty(mesh);
        }
        foreach(var d in data.materials)Save(SourceMaterial(d),Root+"/Materials/"+d.id+".mat");
        AssetDatabase.SaveAssets();Debug.Log("SOURCE_ITEM_MATERIALS_APPLIED "+data.materials.Length);
    }
    static void ConfigureTextures(Export data){
        foreach(var d in data.textureInfos){var path=Root+"/Textures/"+d.file;var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            if(!importer)throw new Exception("Missing source texture "+d.file);
            // Source normal images pack X in alpha, Y in green. Reimport as ordinary RGBA,
            // not NormalMap; that would destroy the recovered channel encoding.
            importer.textureType=TextureImporterType.Default;importer.sRGBTexture=d.srgb;importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.alphaIsTransparency=false;
            importer.mipmapEnabled=d.mipCount>1;importer.filterMode=(FilterMode)d.filterMode;importer.anisoLevel=d.aniso;importer.mipMapBias=d.mipBias;
            importer.wrapModeU=(TextureWrapMode)d.wrapU;importer.wrapModeV=(TextureWrapMode)d.wrapV;importer.wrapModeW=(TextureWrapMode)d.wrapW;
            importer.maxTextureSize=Mathf.Max(32,Mathf.NextPowerOfTwo(Mathf.Max(d.width,d.height)));importer.npotScale=TextureImporterNPOTScale.None;
            importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
        }
    }
    static Material SourceMaterial(MaterialData d){
        var shader=Shader.Find(d.portedShader);if(!shader)throw new Exception("Missing recovered shader "+d.portedShader);
        var mat=new Material(shader){name=d.name,renderQueue=d.renderQueue,enableInstancing=d.enableInstancingVariants,doubleSidedGI=d.doubleSidedGI,globalIlluminationFlags=(MaterialGlobalIlluminationFlags)d.lightmapFlags};
        foreach(var p in d.floats??new FloatValue[0])if(mat.HasProperty(p.name))mat.SetFloat(p.name,p.value);
        foreach(var p in d.ints??new FloatValue[0])if(mat.HasProperty(p.name))mat.SetInt(p.name,(int)p.value);
        foreach(var p in d.vectors??new VectorValue[0])if(mat.HasProperty(p.name))mat.SetVector(p.name,(Vector4)p.value);
        foreach(var p in d.textures??new TextureValue[0])if(mat.HasProperty(p.name)){mat.SetTexture(p.name,Texture(p.texture));mat.SetTextureScale(p.name,p.scale);mat.SetTextureOffset(p.name,p.offset);}
        return mat;
    }
    // Patch renderer state only; do not regenerate normalized model geometry or physics.
    public static int ApplySourceShadowFlags(){
        var data=SourceModelJson.Read<Export>(File.ReadAllText(Root+"/models.json"));int count=0;
        foreach(var model in data.models){string path=Root+"/Prefabs/"+model.group+"/"+model.name+".prefab";var root=PrefabUtility.LoadPrefabContents(path);
            try{count+=ApplyShadowNode(model.root,root.transform.GetChild(0),true);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
        }AssetDatabase.SaveAssets();return count;
    }
    public static int ValidateSourceShadowFlags(){
        var data=SourceModelJson.Read<Export>(File.ReadAllText(Root+"/models.json"));int count=0;
        foreach(var model in data.models){var root=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/"+model.group+"/"+model.name+".prefab");count+=ApplyShadowNode(model.root,root.transform.GetChild(0),false);}return count;
    }
    static int ApplyShadowNode(Node n,Transform t,bool write){
        if(t.name!=n.name)throw new Exception("Source shadow hierarchy mismatch "+n.name+" vs "+t.name);int count=0;
        if(!string.IsNullOrEmpty(n.mesh)){var r=t.GetComponent<Renderer>();if(!r)throw new Exception("Missing renderer "+n.name);
            if(write){r.shadowCastingMode=(ShadowCastingMode)n.castShadows;r.receiveShadows=n.receiveShadows;}
            else if((int)r.shadowCastingMode!=n.castShadows||r.receiveShadows!=n.receiveShadows)throw new Exception("Source shadow mismatch "+n.name);count++;
        }
        for(int i=0;i<n.children.Length;i++)count+=ApplyShadowNode(n.children[i],t.GetChild(i),write);return count;
    }
    static void ApplyMeshSkin(Mesh mesh,MeshData source){
        if(source.boneWeights==null||source.boneWeights.Length==0)return;
        if(source.boneWeights.Length!=mesh.vertexCount)throw new Exception("Skin vertex count mismatch "+source.name);
        mesh.boneWeights=source.boneWeights.Select(w=>new BoneWeight{weight0=w.weight0,weight1=w.weight1,weight2=w.weight2,weight3=w.weight3,boneIndex0=w.boneIndex0,boneIndex1=w.boneIndex1,boneIndex2=w.boneIndex2,boneIndex3=w.boneIndex3}).ToArray();
        mesh.bindposes=source.bindposes.Select(b=>{if(b.values.Length!=16)throw new Exception("Invalid source bind pose");var matrix=new Matrix4x4();for(int r=0;r<4;r++)for(int c=0;c<4;c++)matrix[r,c]=b.values[r*4+c];return matrix;}).ToArray();
    }
    static bool HasSkin(Node n)=>n.skinned||n.children.Any(HasSkin);
    static void BindSkins(Node root,Transform visual){
        var transforms=new Dictionary<string,Transform>();
        Action<Node,Transform> collect=null;collect=(n,t)=>{if(!string.IsNullOrEmpty(n.transformId))transforms.Add(n.transformId,t);for(int i=0;i<n.children.Length;i++)collect(n.children[i],t.GetChild(i));};collect(root,visual);
        Action<Node,Transform> bind=null;bind=(n,t)=>{if(n.skinned){var skin=t.GetComponent<SkinnedMeshRenderer>();skin.bones=n.boneIds.Select(id=>transforms[id]).ToArray();skin.rootBone=string.IsNullOrEmpty(n.rootBoneId)||n.rootBoneId=="0"?null:transforms[n.rootBoneId];if(skin.bones.Length!=skin.sharedMesh.bindposes.Length)throw new Exception("Source bone/bind pose mismatch "+n.name);}for(int i=0;i<n.children.Length;i++)bind(n.children[i],t.GetChild(i));};bind(root,visual);
    }
    static Texture2D Texture(string name){return string.IsNullOrEmpty(name)?null:AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/"+name);}
    static T Save<T>(T value,string path)where T:UnityEngine.Object {var old=AssetDatabase.LoadAssetAtPath<T>(path);if(old){EditorUtility.CopySerialized(value,old);UnityEngine.Object.DestroyImmediate(value);return old;}AssetDatabase.CreateAsset(value,path);return value;}
    static GameObject Build(Node n,Transform parent){
        var g=new GameObject(n.name);g.transform.SetParent(parent,false);g.transform.localPosition=n.position;g.transform.localRotation=n.rotation;g.transform.localScale=n.scale;g.SetActive(n.active);
        if(!string.IsNullOrEmpty(n.mesh)){Renderer r;if(n.skinned){var skin=g.AddComponent<SkinnedMeshRenderer>();skin.sharedMesh=meshes[n.mesh];skin.localBounds=new Bounds(n.boundsCenter,n.boundsExtents*2);skin.quality=(SkinQuality)n.skinQuality;skin.updateWhenOffscreen=n.updateWhenOffscreen;skin.skinnedMotionVectors=n.skinnedMotionVectors;r=skin;}else{g.AddComponent<MeshFilter>().sharedMesh=meshes[n.mesh];r=g.AddComponent<MeshRenderer>();}r.sharedMaterials=n.materials.Select(x=>materials[x]).ToArray();r.enabled=n.rendererEnabled;r.shadowCastingMode=(ShadowCastingMode)n.castShadows;r.receiveShadows=n.receiveShadows;}
        foreach(var c in n.colliders){
            if(c.type=="BoxCollider"){var v=g.AddComponent<BoxCollider>();v.center=c.center;v.size=c.size;}
            else if(c.type=="SphereCollider"){var v=g.AddComponent<SphereCollider>();v.center=c.center;v.radius=c.radius;}
            else {var v=g.AddComponent<CapsuleCollider>();v.center=c.center;v.radius=c.radius;v.height=c.height;v.direction=c.direction;}
        }
        foreach(var c in n.children)Build(c,g.transform);return g;
    }
}
