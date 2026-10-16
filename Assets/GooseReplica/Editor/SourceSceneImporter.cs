using System;using System.IO;using System.Linq;using System.Collections.Generic;using UnityEngine;using UnityEditor;using UnityEngine.Rendering;
public static class SourceSceneImporter {
 const string Root="Assets/GooseReplica/SourceScenes";
 [Serializable] class Data{public Scene[] scenes;public MeshData[] meshes;public Mat[] materials;public Spr[] sprites;}
 [Serializable] class Scene{public string name;public Node[] nodes;}
 [Serializable] class Node{public string name,mesh,sprite;public string[] materials;public bool active,enabled;public int parent,sortingOrder,layer,castShadows;public bool receiveShadows;public Vector3 position,scale;public Quaternion rotation;public Color color;}
 [Serializable] class MeshData{public string id,name;public int builtin;public Vector3[] vertices,normals;public Vector4[] tangents;public Vector2[] uv;public Sub[] submeshes;}
 [Serializable] class Sub{public int[] indices;}
 [Serializable] class Scalar{public string name;public float value;}
 [Serializable] class Vector{public string name;public Color value;}
 [Serializable] class Tex{public string name,texture;public Vector2 scale,offset;}
 [Serializable] class Mat{public string shader;public Scalar[] floats;public Vector[] vectors;public Tex[] textures;public string id,name,texture,ao,emission;public Color tint;public Vector2 scale,offset;public float aoScale;}
 [Serializable] class Spr{public string id,texture;public float ppu;public Vector2 pivot;}
 public static void Import(){
  var data=JsonUtility.FromJson<Data>(File.ReadAllText(Root+"/visuals.json"));
  Directory.CreateDirectory(Root+"/Generated");Directory.CreateDirectory("Assets/GooseReplica/Resources/Replica/Scenes");AssetDatabase.Refresh();
  foreach(var path in Directory.GetFiles(Root+"/Textures","*.png")){var imp=(TextureImporter)AssetImporter.GetAtPath(path);imp.maxTextureSize=2048;imp.alphaIsTransparency=true;imp.SaveAndReimport();}
  var meshes=new Dictionary<string,Mesh>();var mats=new Dictionary<string,Material>();var sprites=new Dictionary<string,Sprite>();
  foreach(var d in data.meshes){
   Mesh m;
   if(d.builtin!=0){var p=GameObject.CreatePrimitive(d.builtin==10202?PrimitiveType.Cube:d.builtin==10209?PrimitiveType.Plane:PrimitiveType.Quad);m=UnityEngine.Object.Instantiate(p.GetComponent<MeshFilter>().sharedMesh);UnityEngine.Object.DestroyImmediate(p);}
   else{m=new Mesh{indexFormat=IndexFormat.UInt32,vertices=d.vertices,uv=d.uv};if(d.normals.Length==d.vertices.Length)m.normals=d.normals;m.subMeshCount=d.submeshes.Length;for(int i=0;i<d.submeshes.Length;i++)m.SetTriangles(d.submeshes[i].indices,i);if(d.normals.Length!=d.vertices.Length)m.RecalculateNormals();}
   if(d.tangents!=null&&d.tangents.Length==m.vertexCount)m.tangents=d.tangents;else if(d.builtin==0)m.RecalculateTangents();
   m.name=d.name??d.id;m.RecalculateBounds();meshes[d.id]=Save(m,Root+"/Generated/"+d.id+".asset");
  }
  foreach(var d in data.materials){
   var shader=Shader.Find("Goose/Source/"+d.shader);bool ported=shader;var m=new Material(shader?shader:Shader.Find("Goose/Source Scene")){name=d.name};
   if(ported){
    foreach(var x in d.floats)if(m.HasProperty(x.name))m.SetFloat(x.name,x.value);
    foreach(var x in d.vectors)if(m.HasProperty(x.name))m.SetVector(x.name,(Vector4)x.value);
    foreach(var x in d.textures)if(m.HasProperty(x.name)){if(!string.IsNullOrEmpty(x.texture))m.SetTexture(x.name,Texture(x.texture));m.SetTextureScale(x.name,x.scale);m.SetTextureOffset(x.name,x.offset);}
    var direction=-(new Quaternion(-.07652297616f,-.82288718224f,.55780386925f,.07652299106f)*Vector3.forward);m.SetVector("_SourceLightDirection",direction);
   }else{
    m.mainTexture=Texture(d.texture);m.mainTextureScale=d.scale;m.mainTextureOffset=d.offset;m.SetColor("_Color",d.tint);m.SetTexture("_AO",Texture(d.ao));m.SetFloat("_HasAO",string.IsNullOrEmpty(d.ao)?0:1);m.SetFloat("_AOScale",d.aoScale);m.SetTexture("_Emission",Texture(d.emission));m.SetFloat("_HasEmission",string.IsNullOrEmpty(d.emission)?0:1);
   }
   if(d.shader=="BackShadow" && m.HasProperty("_Cull"))m.SetFloat("_Cull",1); // source ForwardBase and ShadowCaster both Cull Front
   mats[d.id]=Save(m,Root+"/Generated/"+d.id+".mat");
  }
  foreach(var d in data.sprites){var t=Texture(d.texture);var s=Sprite.Create(t,new Rect(0,0,t.width,t.height),d.pivot,d.ppu);s.name=d.id;sprites[d.id]=Save(s,Root+"/Generated/"+d.id+".asset");}
  foreach(var scene in data.scenes){var nodes=new GameObject[scene.nodes.Length];for(int i=0;i<nodes.Length;i++){var n=scene.nodes[i];var g=new GameObject(n.name);nodes[i]=g;g.layer=n.layer;if(n.parent>=0)g.transform.SetParent(nodes[n.parent].transform,false);g.transform.localPosition=n.position;g.transform.localRotation=n.rotation;g.transform.localScale=n.scale;
   if(!string.IsNullOrEmpty(n.mesh)){g.AddComponent<MeshFilter>().sharedMesh=meshes[n.mesh];var r=g.AddComponent<MeshRenderer>();r.sharedMaterials=n.materials.Select(k=>mats[k]).ToArray();r.enabled=n.enabled;r.shadowCastingMode=(ShadowCastingMode)n.castShadows;r.receiveShadows=n.receiveShadows;}
   if(!string.IsNullOrEmpty(n.sprite)){var r=g.AddComponent<SpriteRenderer>();r.sprite=sprites[n.sprite];r.color=n.color;r.sortingOrder=n.sortingOrder;r.enabled=n.enabled;r.shadowCastingMode=(ShadowCastingMode)n.castShadows;r.receiveShadows=n.receiveShadows;}
   g.SetActive(n.active);
  }PrefabUtility.SaveAsPrefabAsset(nodes[0],"Assets/GooseReplica/Resources/Replica/Scenes/"+scene.name+".prefab");UnityEngine.Object.DestroyImmediate(nodes[0]);}
  AssetDatabase.SaveAssets();Debug.Log("SOURCE_SCENES_IMPORTED "+data.scenes.Length);
 }
 static Texture2D Texture(string name)=>string.IsNullOrEmpty(name)?null:AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/"+name);
 static T Save<T>(T value,string path)where T:UnityEngine.Object{var old=AssetDatabase.LoadAssetAtPath<T>(path);if(old){EditorUtility.CopySerialized(value,old);UnityEngine.Object.DestroyImmediate(value);return old;}AssetDatabase.CreateAsset(value,path);return value;}
}
