using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Newtonsoft.Json.Linq;

// Native module JSON is recovered from iOS particleprefab part1; store board from WX main scene.
public static class SourceToolVisualImporter {
    const string Root="Assets/GooseReplica/SourceToolVisuals",Out="Assets/GooseReplica/Resources/SourceTools";
    [Serializable] class Manifest { public PrefabData[] prefabs;public MeshData[] meshes;public MatData[] materials;public TexData[] textures;public Background background; }
    [Serializable] class PrefabData {public string name;public Node[] nodes;}
    [Serializable] class Node {public string name,particle,renderer;public bool active;public int parent,layer;public Vector3 position,scale;public Quaternion rotation;public string[] materials,meshes;public int[] streams;}
    [Serializable] class MeshData {public string id,name;public int builtin;public Vector3[] vertices,normals;public Vector2[] uv;public Sub[] submeshes;}
    [Serializable] class Sub {public int[] indices;}
    [Serializable] class MatData {public string id,name,shader;public int renderQueue;public FloatProp[] floats;public ColorProp[] colors;public TexProp[] textures;}
    [Serializable] class FloatProp {public string name;public float value;}
    [Serializable] class ColorProp {public string name;public Color value;}
    [Serializable] class TexProp {public string name,texture;public Vector2 scale,offset;}
    [Serializable] class TexData {public string id,file,name;public int width,height,filterMode,wrapU,wrapV,wrapW,colorSpace;public bool mipmaps;}
    [Serializable] class Background {public string name,material,mesh;public Vector3 position,scale;public Quaternion rotation;}
    // EditorJsonUtility wraps built-in native components in their class name. UnityPy
    // typetrees have only the payload; retain Unity's serializedVersion fields at every depth.
    static void OverwriteNative(string source,UnityEngine.Object target){
        var original=JObject.Parse(EditorJsonUtility.ToJson(target));var body=JObject.Parse(source);
        var wrapper=original.Property(target.GetType().Name);
        if(wrapper!=null&&wrapper.Value is JObject)wrapper.Value=MergeNative(wrapper.Value,body);
        else original=(JObject)MergeNative(original,body);
        EditorJsonUtility.FromJsonOverwrite(original.ToString(Newtonsoft.Json.Formatting.None),target);
        var readback=JObject.Parse(EditorJsonUtility.ToJson(target));var result=readback[target.GetType().Name]??readback;
        CompareNative(body,result,target.GetType().Name);
    }
    static void CompareNative(JToken expected,JToken actual,string path){
        if(expected is JObject obj){if(obj.Property("instanceID")!=null)return;foreach(var p in obj.Properties()){
            // Runtime asset-only baked-lightmap bookkeeping is absent from native editor particle schema.
            if(path=="ParticleSystemRenderer"&&(p.Name=="m_LightmapIndex"||p.Name=="m_LightmapIndexDynamic"||p.Name=="m_LightmapTilingOffset"||p.Name=="m_LightmapTilingOffsetDynamic"))continue;
            if(!(actual is JObject current)||current[p.Name]==null)throw new Exception("Native particle missing "+path+"."+p.Name);CompareNative(p.Value,current[p.Name],path+"."+p.Name);
        }return;}
        if(expected is JArray arr){if(!(actual is JArray next)||next.Count!=arr.Count)throw new Exception("Native particle array length "+path);for(int i=0;i<arr.Count;i++)CompareNative(arr[i],next[i],path+"["+i+"]");return;}
        bool numeric=expected.Type==JTokenType.Float||expected.Type==JTokenType.Integer;
        if(numeric&&actual.Type==JTokenType.Boolean&&((double)expected==0||(double)expected==1)){if(((double)expected!=0)!=(bool)actual)throw new Exception("Native particle boolean "+path);return;}
        if(expected.Type==JTokenType.Boolean&&(actual.Type==JTokenType.Integer)){if((bool)expected!=((int)actual!=0))throw new Exception("Native particle boolean "+path);return;}
        if(numeric&&(actual.Type==JTokenType.Float||actual.Type==JTokenType.Integer)){double a=(double)expected,b=(double)actual;if(Math.Abs(a-b)>Math.Max(1e-6,Math.Abs(a)*1e-6))throw new Exception("Native particle value "+path+" "+a+" != "+b);}
        else if(!JToken.DeepEquals(expected,actual))throw new Exception("Native particle value "+path+" "+expected+" != "+actual);
    }
    static JToken MergeNative(JToken schema,JToken source){
        if(source is JObject src){var result=schema is JObject obj?(JObject)obj.DeepClone():new JObject();foreach(var p in src.Properties())result[p.Name]=MergeNative(result[p.Name],p.Value);return result;}
        if(source is JArray array){var result=new JArray();var old=schema as JArray;for(int i=0;i<array.Count;i++)result.Add(MergeNative(old!=null&&old.Count>0?old[Mathf.Min(i,old.Count-1)]:null,array[i]));return result;}
        return source.DeepClone();
    }
    [MenuItem("Goose/Import source tool visuals")]
    public static void Build(){
        Directory.CreateDirectory(Out);Directory.CreateDirectory(Out+"/Materials");Directory.CreateDirectory(Out+"/Meshes");
        AssetDatabase.Refresh();var data=JsonUtility.FromJson<Manifest>(File.ReadAllText(Root+"/manifest.json"));
        var textures=new Dictionary<string,Texture2D>();
        foreach(var d in data.textures){var path=Root+"/Textures/"+d.file;var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.sRGBTexture=d.colorSpace==1;importer.alphaIsTransparency=false;importer.mipmapEnabled=d.mipmaps;importer.filterMode=(FilterMode)d.filterMode;importer.wrapModeU=(TextureWrapMode)d.wrapU;importer.wrapModeV=(TextureWrapMode)d.wrapV;importer.wrapModeW=(TextureWrapMode)d.wrapW;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=Mathf.NextPowerOfTwo(Mathf.Max(d.width,d.height));importer.SaveAndReimport();textures[d.id]=AssetDatabase.LoadAssetAtPath<Texture2D>(path);}
        var materials=new Dictionary<string,Material>();
        foreach(var d in data.materials){var shader=Shader.Find(d.shader=="Unlit/NiuQu_UV_Alpha"?"Goose/Source/ToolDistortion":d.shader);if(!shader)throw new Exception("Missing source tool shader "+d.shader);var path=Out+"/Materials/"+d.id+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(!mat){mat=new Material(shader);AssetDatabase.CreateAsset(mat,path);}else mat.shader=shader;mat.name=d.name;
            foreach(var p in d.floats)if(mat.HasProperty(p.name))mat.SetFloat(p.name,p.value);
            foreach(var p in d.colors)if(mat.HasProperty(p.name))mat.SetColor(p.name,p.value);
            foreach(var p in d.textures)if(mat.HasProperty(p.name)){mat.SetTexture(p.name,string.IsNullOrEmpty(p.texture)?null:textures[p.texture]);mat.SetTextureScale(p.name,p.scale);mat.SetTextureOffset(p.name,p.offset);}mat.renderQueue=d.renderQueue;EditorUtility.SetDirty(mat);materials[d.id]=mat;}
        var meshes=new Dictionary<string,Mesh>();
        foreach(var d in data.meshes){if(d.builtin!=0){var primitive=GameObject.CreatePrimitive(d.builtin==10210?PrimitiveType.Quad:d.builtin==10209?PrimitiveType.Plane:PrimitiveType.Cube);var mesh=primitive.GetComponent<MeshFilter>().sharedMesh;AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mesh,out string guid,out long id);if(id!=d.builtin)throw new Exception("Source builtin mesh mismatch "+d.builtin+" != "+id);meshes[d.id]=mesh;UnityEngine.Object.DestroyImmediate(primitive);continue;}
            var path=Out+"/Meshes/"+d.id+".asset";var m=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(!m){m=new Mesh();AssetDatabase.CreateAsset(m,path);}else m.Clear();m.name=d.name;m.vertices=d.vertices;if(d.normals.Length==d.vertices.Length)m.normals=d.normals;if(d.uv.Length==d.vertices.Length)m.uv=d.uv;m.subMeshCount=d.submeshes.Length;for(int i=0;i<d.submeshes.Length;i++)m.SetTriangles(d.submeshes[i].indices,i);m.RecalculateBounds();EditorUtility.SetDirty(m);meshes[d.id]=m;}
        Directory.CreateDirectory("visual-captures/source-tool-module-roundtrip");int particleCount=0;
        foreach(var p in data.prefabs){var objects=new List<GameObject>();
            try{for(int i=0;i<p.nodes.Length;i++){var d=p.nodes[i];var go=new GameObject(d.name);go.SetActive(false);objects.Add(go);go.layer=d.layer;if(d.parent>=0)go.transform.SetParent(objects[d.parent].transform,false);go.transform.localPosition=d.position;go.transform.localRotation=d.rotation;go.transform.localScale=d.scale;
                    if(!string.IsNullOrEmpty(d.particle)){var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var emissionSchema=ps.emission;emissionSchema.SetBursts(new[]{new ParticleSystem.Burst(0,1)});OverwriteNative(File.ReadAllText(Root+"/Modules/"+d.particle),ps);File.WriteAllText("visual-captures/source-tool-module-roundtrip/"+d.particle,EditorJsonUtility.ToJson(ps));particleCount++;}
                    if(!string.IsNullOrEmpty(d.renderer)){var r=go.GetComponent<ParticleSystemRenderer>();OverwriteNative(File.ReadAllText(Root+"/Modules/"+d.renderer),r);File.WriteAllText("visual-captures/source-tool-module-roundtrip/"+d.renderer,EditorJsonUtility.ToJson(r));r.sharedMaterials=d.materials.Select(k=>string.IsNullOrEmpty(k)?null:materials[k]).ToArray();var m=d.meshes.Where(k=>!string.IsNullOrEmpty(k)).Select(k=>meshes[k]).ToArray();if(m.Length>0)r.SetMeshes(m);r.SetActiveVertexStreams(d.streams.Select(x=>(ParticleSystemVertexStream)x).ToList());}
                }
                for(int i=1;i<objects.Count;i++)objects[i].SetActive(p.nodes[i].active);objects[0].SetActive(true);PrefabUtility.SaveAsPrefabAsset(objects[0],Out+"/"+p.name+".prefab");
            }finally{if(objects.Count>0)UnityEngine.Object.DestroyImmediate(objects[0]);}}
        var bg=new GameObject("StoreContentBG");try{bg.transform.localPosition=data.background.position;bg.transform.localRotation=data.background.rotation;bg.transform.localScale=data.background.scale;bg.AddComponent<MeshFilter>().sharedMesh=meshes[data.background.mesh];var mr=bg.AddComponent<MeshRenderer>();mr.sharedMaterial=materials[data.background.material];mr.shadowCastingMode=ShadowCastingMode.Off;mr.receiveShadows=false;PrefabUtility.SaveAsPrefabAsset(bg,Out+"/StoreContentBG.prefab");}finally{UnityEngine.Object.DestroyImmediate(bg);}
        AssetDatabase.SaveAssets();Debug.Log("SOURCE_TOOL_VISUAL_IMPORT PASS: "+data.prefabs.Length+" original prefabs, "+particleCount+" native particle modules, "+data.materials.Length+" materials, "+data.textures.Length+" textures; verified builtin store mesh 10210.");
    }
}
