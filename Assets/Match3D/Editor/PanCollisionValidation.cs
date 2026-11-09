using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
[InitializeOnLoad] public static class PanCollisionValidation {
 static PanCollisionValidation(){EditorApplication.update+=Poll;}
 static void Poll(){if(EditorApplication.isCompiling||EditorApplication.isUpdating||!File.Exists("Temp/pan-check"))return;var action=File.ReadAllText("Temp/pan-check").Trim();File.Delete("Temp/pan-check");try{if(action=="test")Test();else Inspect();}catch(System.Exception e){File.WriteAllText("Temp/pan-check-result",e.ToString());}}
 static void Test(){
 var scene=UnityEngine.SceneManagement.SceneManager.CreateScene("Pan collision regression",new UnityEngine.SceneManagement.CreateSceneParameters(UnityEngine.SceneManagement.LocalPhysicsMode.Physics3D));
 var physics=scene.GetPhysicsScene();var lines=new List<string>();bool prior=BoardPhysicsSettings.Active;
 try{
 BoardPhysicsSettings.Begin();ItemColliderGeometry.ConfigureLayers();
 var pan=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/Resources/Match3D/Scenes/CountrysideStove.prefab"));
 UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(pan,scene);Match3DThemePresentation.AddPanCollision(pan);
 var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/Generated/ModelAdapter/Party_ChocolateBar/Party_ChocolateBar.prefab");
 for(int n=0;n<12;n++){
 var item=Object.Instantiate(prefab);UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(item,scene);
 item.transform.localScale*=BoardLayoutSettings.ModelScale(prefab);item.transform.position=new Vector3((n%3-1)*1.2f,n<6?3:22,(n%2==0?1:-1)*.7f);item.transform.rotation=Quaternion.Euler(n*23,n*37,n*13);
 var body=item.GetComponent<Rigidbody>();body.isKinematic=false;BoardPhysicsSettings.Apply(body);
 for(int step=0;step<600;step++)physics.Simulate(.02f);
 var bounds=item.GetComponentsInChildren<Renderer>().First().bounds;
 if(bounds.center.y<.48f||body.position.y<0)throw new System.Exception("Chocolate below pan: "+n+" "+bounds);
 var ray=new Ray(bounds.center+Vector3.up*8,Vector3.down);
 if(!physics.Raycast(ray.origin,ray.direction,out var hit,20,1<<ItemColliderGeometry.PickLayer,QueryTriggerInteraction.Ignore)||hit.rigidbody!=body)throw new System.Exception("Chocolate not pickable: "+n);
 lines.Add("PASS drop "+n+" center="+bounds.center+" detection="+body.collisionDetectionMode);Object.DestroyImmediate(item);
 }
 }finally{if(!prior)BoardPhysicsSettings.End();if(Application.isPlaying)SceneManager.UnloadSceneAsync(scene);else UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene,true);}
 File.WriteAllLines("Temp/pan-check-result",lines);
 }
 static void Inspect(){
 var lines=new List<string>();
 var live=Object.FindObjectsOfType<BoardItemIdentity>();foreach(var i in live.Where(i=>i.name.Contains("Chocolate")))lines.Add("live "+i.name+" "+i.transform.position);
 var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/Resources/Match3D/Scenes/CountrysideStove.prefab"));
 foreach(var mf in root.GetComponentsInChildren<MeshFilter>().Where(m=>m.transform.IsChildOf(root.transform.Find("Grab3D Frying Pan")))){
 var vertices=mf.sharedMesh.vertices.Select(v=>mf.transform.TransformPoint(v)).ToArray();
 lines.Add("pan center vertices "+string.Join(",",vertices.Where(v=>new Vector2(v.x,v.z).magnitude<1).Select(v=>v.y.ToString("F4")).Distinct()));
 }
 BoardEnvironment.Select("CountrysideStove");var shell=BoardEnvironment.CreateBoundary();Physics.SyncTransforms();
 foreach(var c in shell.GetComponentsInChildren<Collider>()){if(c.Raycast(new Ray(new Vector3(0,10,0),Vector3.down),out var hit,30))lines.Add("floor "+c.name+" top="+hit.point+" bounds="+c.bounds);}
 Object.DestroyImmediate(root);Object.DestroyImmediate(shell);File.WriteAllLines("Temp/pan-check-result",lines);
 }
}
