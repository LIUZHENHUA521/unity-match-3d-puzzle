using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

// Checks the imported hierarchy against source records, plus runtime lifecycle / projection.
public static class SourceVisualValidation {
    [Serializable] class Data {public Scene[] scenes;}
    [Serializable] class Scene {public string name;public Node[] nodes;}
    [Serializable] class Node {public string name;public int parent;public Vector3 position,scale;public Quaternion rotation;}
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static string NodePath(Node[] nodes,int index){var n=nodes[index];return n.parent<0?"":NodePath(nodes,n.parent)+"/"+n.name;}
    public static void Run(){
        string report="";var flow=ReplicaFlow.Instance;var camera=Camera.main;float aspect=camera.aspect;
        var oldPosition=camera.transform.position;var oldRotation=camera.transform.rotation;float size=camera.orthographicSize;bool ortho=camera.orthographic;
        var fitter=camera.GetComponent<CameraFitter>();bool fitEnabled=fitter&&fitter.enabled;
        var oldRenderers=GameObject.Find("Cooking Pot").GetComponentsInChildren<Renderer>(true);var oldEnabled=oldRenderers.Select(r=>r.enabled).ToArray();
        try{
            var data=JsonUtility.FromJson<Data>(File.ReadAllText("Assets/GooseReplica/SourceScenes/visuals.json"));
            for(int theme=0;theme<3;theme++){
                var scene=data.scenes[theme];var prefab=Resources.Load<GameObject>("Replica/Scenes/"+scene.name);
                Check(prefab&&prefab.GetComponentsInChildren<Collider>(true).Length==0&&prefab.GetComponentsInChildren<Rigidbody>(true).Length==0,"visuals must not duplicate source collision bodies");
                Check(prefab.GetComponentsInChildren<Transform>(true).Length==scene.nodes.Length,"complete visual hierarchy");
                foreach(var r in prefab.GetComponentsInChildren<Renderer>(true))foreach(var m in r.sharedMaterials)Check(m&&m.shader&&m.shader.isSupported&&!ShaderUtil.ShaderHasError(m.shader),"supported material: "+r.name);
                var resolved=new Transform[scene.nodes.Length];
                for(int j=0;j<scene.nodes.Length;j++){
                    var n=scene.nodes[j];var path=NodePath(scene.nodes,j).TrimStart('/');var t=j==0?prefab.transform:resolved[n.parent].GetChild(scene.nodes.Take(j).Count(x=>x.parent==n.parent));resolved[j]=t;
                    Check(t&&Vector3.Distance(t.localPosition,n.position)<.00001f&&Vector3.Distance(t.localScale,n.scale)<.00001f&&Mathf.Abs(Quaternion.Dot(t.localRotation.normalized,n.rotation.normalized))>1-.000001f,"source visual transform: "+path+" actual="+(t? t.localPosition.ToString("F6")+" "+t.localScale.ToString("F6")+" "+t.localRotation.ToString("F6"):"missing")+" expected="+n.position.ToString("F6")+" "+n.scale.ToString("F6")+" "+n.rotation.ToString("F6"));
                }
                flow.Begin(theme);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);var presentation=flow.GetComponent<ReplicaThemePresentation>();
                Check(presentation&&presentation.Active,"theme presentation active");
                Check(Vector3.Distance(camera.transform.position,ReplicaThemePresentation.SourcePosition)<.0001f&&Quaternion.Angle(camera.transform.rotation,ReplicaThemePresentation.SourceRotation)<.001f,"source camera world pose");
                Check(oldRenderers.All(r=>!r.enabled),"legacy pot hidden");
                foreach(float a in new[]{9f/16,9f/19.5f,3f/4}){
                    camera.aspect=a;presentation.Fit();float expected=a<9f/16?6.5f*(9f/16)/a:6.5f;
                    Check(Mathf.Abs(camera.orthographicSize-expected)<.0001f,"screen aspect adaptation");
                    // A world point at this camera's horizontal extent must project to the right edge.
                    var center=new Vector3(0,0,.5f);var edge=center+camera.transform.right*(expected*a);
                    Check(Mathf.Abs(camera.WorldToViewportPoint(edge).x-1)<.0001f,"projected horizontal extent");
                }
                // Common source ancestors must occupy the same world space in visual and collision trees.
                var visual=GameObject.Find(scene.name+" source visuals").transform;
                var boundary=GameObject.Find(scene.name+" source collision reference").transform;
                int shared=0;
                foreach(var t in boundary.GetComponentsInChildren<Transform>(true)){
                    string path=AnimationUtility.CalculateTransformPath(t,boundary);var v=path.Length==0?visual:visual.Find(path);if(!v)continue;
                    var a=t.localToWorldMatrix;var b=v.localToWorldMatrix;
                    for(int k=0;k<16;k++)Check(Mathf.Abs(a[k]-b[k])<.0001f,"visual/collision ancestor alignment "+path);
                    shared++;
                }
                Check(shared>=2,"source geometry shared ancestors");flow.Home();
                Check(Vector3.Distance(camera.transform.position,oldPosition)<.0001f&&Quaternion.Angle(camera.transform.rotation,oldRotation)<.001f&&Mathf.Abs(camera.orthographicSize-size)<.0001f&&camera.orthographic==ortho,"camera restored");
                Check(!fitter||fitter.enabled==fitEnabled,"fitter restored");
                for(int j=0;j<oldRenderers.Length;j++)Check(oldRenderers[j].enabled==oldEnabled[j],"legacy renderer restored");
                report+="PASS "+scene.name+": "+scene.nodes.Length+" source transforms; "+shared+" shared visual/collision ancestors; supported materials; 9:16, 9:19.5 and 3:4 projection; home cleanup.\n";
            }
        }catch(Exception e){report+="FAIL "+e+"\n";}
        finally{flow.Home();camera.aspect=aspect;File.WriteAllText("Temp/goose-source-visual-validation.txt",report);Debug.Log(report);}
    }
}
