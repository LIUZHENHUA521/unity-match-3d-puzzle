using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Compare imported assets with independent source records; no prefab regeneration.
public static class SourceItemMaterialsValidation {
    const string Root="Assets/GooseReplica/OriginalModels";
    [Serializable] class Data { public MaterialData[] materials;public MeshData[] meshes;public TextureInfo[] textureInfos;public Model[] models; }
    [Serializable] class MaterialData {public string id,name,portedShader;public int renderQueue,lightmapFlags;public bool enableInstancingVariants,doubleSidedGI;public FloatValue[] floats,ints;public VectorValue[] vectors;public TextureValue[] textures;}
    [Serializable] class FloatValue {public string name;public float value;}
    [Serializable] class VectorValue {public string name;public Color value;}
    [Serializable] class TextureValue {public string name,texture;public Vector2 scale,offset;}
    [Serializable] class MeshData {public string id;public Vector3[] vertices;public Vector4[] tangents;public SkinWeight[] boneWeights;public BindPose[] bindposes;}
    [Serializable] class SkinWeight {public float weight0,weight1,weight2,weight3;public int boneIndex0,boneIndex1,boneIndex2,boneIndex3;}
    [Serializable] class BindPose {public float[] values;}
    [Serializable] class TextureInfo {public string file;public bool srgb;public int mipCount,filterMode,aniso,wrapU,wrapV,wrapW;public float mipBias;}
    [Serializable] class Model {public string name,group;public Node root;}
    [Serializable] class Node {public string name,mesh,transformId,rootBoneId;public string[] boneIds;public bool skinned,updateWhenOffscreen,skinnedMotionVectors;public int skinQuality;public Vector3 boundsCenter,boundsExtents;public string[] materials;public Node[] children;}
    static void Check(bool condition,string message){if(!condition)throw new Exception(message);}
    static bool Near(float a,float b){return Mathf.Abs(a-b)<=.00001f*Mathf.Max(1,Mathf.Abs(b));}
    static bool Near(Vector4 a,Vector4 b){return Near(a.x,b.x)&&Near(a.y,b.y)&&Near(a.z,b.z)&&Near(a.w,b.w);}
    static int CheckNode(Node n,Transform transform,Dictionary<string,Transform> bones){
        Check(transform.name==n.name,"item hierarchy "+n.name);int count=0;
        if(!string.IsNullOrEmpty(n.mesh)){
            var renderer=transform.GetComponent<Renderer>();Check(renderer,"renderer missing "+n.name);
            Check(renderer.sharedMaterials.Length==n.materials.Length,"material slot count "+n.name);
            for(int i=0;i<n.materials.Length;i++)Check(AssetDatabase.GetAssetPath(renderer.sharedMaterials[i])==Root+"/Materials/"+n.materials[i]+".mat","material slot mapping "+n.name+" #"+i);
            Mesh mesh;
            if(n.skinned){var skin=renderer as SkinnedMeshRenderer;Check(skin,"expected skinned renderer "+n.name);mesh=skin.sharedMesh;
                Check(skin.bones.Length==n.boneIds.Length&&skin.bones.Select((b,i)=>b==bones[n.boneIds[i]]).All(x=>x),"source skin bones "+n.name);
                Check(skin.rootBone==(string.IsNullOrEmpty(n.rootBoneId)||n.rootBoneId=="0"?null:bones[n.rootBoneId]),"source root bone "+n.name);
                Check(skin.localBounds.center==n.boundsCenter&&skin.localBounds.extents==n.boundsExtents,"source skin bounds "+n.name);
                Check((int)skin.quality==n.skinQuality&&skin.updateWhenOffscreen==n.updateWhenOffscreen&&skin.skinnedMotionVectors==n.skinnedMotionVectors,"source skin flags "+n.name);
            }else{var filter=transform.GetComponent<MeshFilter>();Check(filter,"mesh filter "+n.name);mesh=filter.sharedMesh;}
            Check(AssetDatabase.GetAssetPath(mesh)==Root+"/Meshes/"+n.mesh+".asset","mesh mapping "+n.name);count++;
        }
        Check(transform.childCount==n.children.Length,"item child count "+n.name);
        for(int i=0;i<n.children.Length;i++)count+=CheckNode(n.children[i],transform.GetChild(i),bones);return count;
    }
    public static void Run(){
        string report="";
        try{
            var data=SourceModelJson.Read<Data>(File.ReadAllText(Root+"/models.json"));
            int floats=0,vectors=0,textures=0,legacy=0;
            foreach(var source in data.materials){
                var material=AssetDatabase.LoadAssetAtPath<Material>(Root+"/Materials/"+source.id+".mat");
                Check(material&&material.shader,"material missing "+source.name);
                Check(material.enableInstancing==source.enableInstancingVariants&&material.doubleSidedGI==source.doubleSidedGI&&(int)material.globalIlluminationFlags==source.lightmapFlags,"source material flags "+source.name);
                Check(material.shader.name==source.portedShader,"source shader "+source.name+" got "+material.shader.name+" expected "+source.portedShader);
                Check(material.shader.isSupported&&!ShaderUtil.ShaderHasError(material.shader),"unsupported source shader "+material.shader.name);
                var serialized=new SerializedObject(material);var queue=serialized.FindProperty("m_CustomRenderQueue");
                Check(queue!=null&&queue.intValue==source.renderQueue,"source render queue "+source.name);
                foreach(var p in (source.floats??new FloatValue[0]).Concat(source.ints??new FloatValue[0])){
                    if(!material.HasProperty(p.name)){legacy++;continue;}
                    Check(Near(material.GetFloat(p.name),p.value),"float "+source.name+" "+p.name);floats++;
                }
                foreach(var p in source.vectors??new VectorValue[0]){
                    if(!material.HasProperty(p.name)){legacy++;continue;}
                    Check(Near(material.GetVector(p.name),(Vector4)p.value),"vector "+source.name+" "+p.name);vectors++;
                }
                foreach(var p in source.textures??new TextureValue[0]){
                    if(!material.HasProperty(p.name)){legacy++;continue;}
                    var expected=string.IsNullOrEmpty(p.texture)?null:AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/"+p.texture);
                    Check(material.GetTexture(p.name)==expected,"texture mapping "+source.name+" "+p.name);
                    Check(Vector2.Distance(material.GetTextureScale(p.name),p.scale)<.00001f&&Vector2.Distance(material.GetTextureOffset(p.name),p.offset)<.00001f,"texture ST "+source.name+" "+p.name);textures++;
                }
            }
            foreach(var source in data.textureInfos){
                var importer=AssetImporter.GetAtPath(Root+"/Textures/"+source.file) as TextureImporter;
                Check(importer,"texture importer "+source.file);
                Check(importer.textureType==TextureImporterType.Default,"do not reencode packed normal "+source.file);
                Check(importer.sRGBTexture==source.srgb,"texture color space "+source.file);
                Check(importer.mipmapEnabled==(source.mipCount>1),"texture mip setting "+source.file);
                Check((int)importer.filterMode==source.filterMode&&importer.anisoLevel==source.aniso&&Near(importer.mipMapBias,source.mipBias),"texture filter "+source.file);
                Check((int)importer.wrapModeU==source.wrapU&&(int)importer.wrapModeV==source.wrapV&&(int)importer.wrapModeW==source.wrapW,"texture wrap "+source.file);
            }
            int tangentCount=0;
            foreach(var source in data.meshes){
                var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/Meshes/"+source.id+".asset");
                Check(mesh&&mesh.vertexCount==source.vertices.Length,"mesh vertex count "+source.id);
                var tangents=mesh.tangents;Check(tangents.Length==source.tangents.Length&&tangents.Length==mesh.vertexCount,"source tangent count "+source.id);
                for(int i=0;i<tangents.Length;i++)Check(Near(tangents[i],source.tangents[i]),"source tangent value "+source.id+" #"+i);tangentCount+=tangents.Length;
                if(source.boneWeights!=null&&source.boneWeights.Length>0){var weights=mesh.boneWeights;var bind=mesh.bindposes;Check(weights.Length==source.boneWeights.Length&&bind.Length==source.bindposes.Length,"source skin counts "+source.id);
                    for(int i=0;i<weights.Length;i++){var a=weights[i];var b=source.boneWeights[i];Check(Near(new Vector4(a.weight0,a.weight1,a.weight2,a.weight3),new Vector4(b.weight0,b.weight1,b.weight2,b.weight3))&&a.boneIndex0==b.boneIndex0&&a.boneIndex1==b.boneIndex1&&a.boneIndex2==b.boneIndex2&&a.boneIndex3==b.boneIndex3,"source bone weights "+source.id+" #"+i);}
                    for(int i=0;i<bind.Length;i++)for(int r=0;r<4;r++)for(int c=0;c<4;c++)Check(Near(bind[i][r,c],source.bindposes[i].values[r*4+c]),"source bind pose "+source.id+" #"+i);
                }
            }
            int renderers=0;
            foreach(var model in data.models){var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/"+model.group+"/"+model.name+".prefab");Check(prefab,"item prefab "+model.name);var bones=new Dictionary<string,Transform>();Action<Node,Transform> collect=null;collect=(n,t)=>{if(!string.IsNullOrEmpty(n.transformId))bones.Add(n.transformId,t);for(int i=0;i<n.children.Length;i++)collect(n.children[i],t.GetChild(i));};collect(model.root,prefab.transform.GetChild(0));renderers+=CheckNode(model.root,prefab.transform.GetChild(0),bones);}
            report="PASS source item materials: "+data.materials.Length+" shaders/queues; "+floats+" declared numeric parameters, "+vectors+" vectors, "+textures+" texture slots/ST; "+legacy+" undeclared legacy saved fields excluded.\n"+
                "PASS "+data.textureInfos.Length+" texture import settings (packed RGBA retained); "+data.meshes.Length+" meshes / "+tangentCount+" original tangents; "+data.models.Length+" prefabs / "+renderers+" renderer material/mesh mappings, including original skin bone references/weights/bind poses.\n";
        }catch(Exception e){report+="FAIL "+e+"\n";}
        Directory.CreateDirectory("Temp");File.WriteAllText("Temp/goose-source-item-materials-validation.txt",report);Debug.Log(report);
    }
}
