using System;using System.IO;using System.Linq;using UnityEngine;using UnityEditor;
public static class SourceModelSizeImporter {
 [Serializable] class Data {public Model[] models;}
 [Serializable] class Model {public string name,group;public Node root;}
 [Serializable] class Node {public Vector3 scale;}
 [Serializable] class Sizes {public Size[] items;}
 [Serializable] class Size {public string name;public float normalizedToSource;}
 public static void Import(){
  const string root="Assets/GooseReplica/OriginalModels";
  var data=SourceModelJson.Read<Data>(File.ReadAllText(root+"/models.json"));
  var sizes=new Sizes{items=data.models.Select(m=>{
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(root+"/Prefabs/"+m.group+"/"+m.name+".prefab");
   if(!prefab||prefab.transform.childCount==0)throw new InvalidOperationException("Missing source prefab "+m.name);
   return new Size{name=m.name,normalizedToSource=m.root.scale.x/prefab.transform.GetChild(0).localScale.x};
  }).ToArray()};
  const string path="Assets/GooseReplica/Resources/Replica/SourceSizes.json";
  File.WriteAllText(path,JsonUtility.ToJson(sizes,true));AssetDatabase.ImportAsset(path);
 }
}
