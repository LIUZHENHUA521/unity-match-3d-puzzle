using System;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

// Exercise imported geometry at the outer container edges, including arbitrary rotations.
[InitializeOnLoad]
public static class BoardViewportFitValidation {
    const string Key="board-viewport-runtime-validation";
    static BoardViewportFitValidation(){EditorApplication.playModeStateChanged+=state=>{
        if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))
            EditorApplication.delayCall+=()=>new GameObject("Viewport runtime validation").AddComponent<BoardValidationDriver>().StartCoroutine(RuntimeChecks());
    };}
    public static void RunRuntime(){SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static IEnumerator RuntimeChecks(){
        yield return null;yield return null;
        string failure=null;var steps=RuntimeCore();
        while(true){bool more=false;try{more=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(!more||failure!=null)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();
        if(Match3DFlow.Instance)Match3DFlow.Instance.Home();
        string report=failure==null?"PASS live first/second-stage creation; settling, shuffle, pick and undo keep fixed camera; resize adapts and restores; visible board remains on screen.\n":"FAIL "+failure+"\n";
        File.WriteAllText("docs/validation/board-viewport-runtime.txt",report);Debug.Log(report);
        SessionState.SetBool(Key,false);EditorApplication.Exit(failure==null?0:1);
    }
    static IEnumerator RuntimeCore(){
        var flow=Match3DFlow.Instance;var camera=Camera.main;camera.aspect=1206f/2622;
        flow.Begin(0);BoardCreationTestSupport.FinishPending(flow);
        var presentation=flow.GetComponent<Match3DThemePresentation>();
        float size=camera.orthographicSize;var position=camera.transform.position;var rotation=camera.transform.rotation;
        void Stable(){
            presentation.Fit();Check(Mathf.Abs(camera.orthographicSize-size)<.00001f,"camera zoom changed during gameplay");
            Check(camera.transform.position==position&&camera.transform.rotation==rotation,"camera pose changed");
        }
        yield return new WaitForSeconds(2);Stable();
        flow.NextStage();BoardCreationTestSupport.FinishPending(flow);Stable();
        for(int i=0;i<120;i++){Stable();yield return null;}
        Check(BoardTools.RunPaid(2,()=>BoardTools.MessUp()),"shuffle started");
        float deadline=Time.realtimeSinceStartup+10;
        while(BoardTools.Shuffling&&Time.realtimeSinceStartup<deadline){Stable();yield return null;}
        Check(!BoardTools.Shuffling,"shuffle finished");
        var board=LevelGenerator.Instance.objectsOnScene;
        var picked=board.First(o=>LayeredItemPile.Instance.CanPick(o.GetComponent<Rigidbody>()));
        InputController.Instance.AddObjectOnStack(picked);
        yield return new WaitForSeconds(1);Stable();
        Check(InputController.Instance.objectsOnStack.Contains(picked),"pick reached tray");
        InputController.Instance.UndoLast();yield return new WaitForSeconds(1);Stable();
        Check(!InputController.Instance.objectsOnStack.Contains(picked),"undo returned item");
        foreach(float aspect in new[]{9f/16,1206f/2622,9f/20,3f/4}){
            camera.aspect=aspect;presentation.Fit();
            foreach(var obj in board)foreach(var renderer in obj.GetComponentsInChildren<Renderer>()){
                if(!renderer.enabled)continue;
                var bounds=renderer.bounds;
                for(int i=0;i<8;i++){
                    var corner=bounds.center+Vector3.Scale(bounds.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));
                    var p=camera.WorldToViewportPoint(corner);
                    Check(p.x>=0&&p.x<=1&&p.y>=0&&p.y<=1,$"runtime clipping {obj.name}: {p}, aspect={aspect}");
                }
            }
        }
        camera.aspect=1206f/2622;Stable();
    }

    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    public static void Run(){
        var cameraObject=new GameObject("Viewport fit validation camera");
        var camera=cameraObject.AddComponent<Camera>();camera.orthographic=true;
        camera.transform.SetPositionAndRotation(Match3DThemePresentation.SourcePosition,Match3DThemePresentation.SourceRotation);
        string report="";int checks=0,oldClipped=0;
        try {
            BoardEnvironment.Select("CountrysideStove");
            var prefabs=LevelLayoutConfig.Get("CountrysideStove").prefabs;
            float width=Match3DThemePresentation.RequiredHalfWidth(prefabs);
            report+=$"Models={prefabs.Length}; container half width={BoardEnvironment.HorizontalHalfExtent:F3}; fitted half width={width:F3}\n";
            var random=new System.Random(20261105);
            foreach(float aspect in new[]{9f/16,1206f/2622,9f/20,3f/4}){
                camera.aspect=aspect;float size=Match3DThemePresentation.Size(aspect,width);
                camera.orthographicSize=size;
                foreach(var prefab in prefabs){
                    var item=UnityEngine.Object.Instantiate(prefab);
                    try {
                        item.transform.localScale*=BoardLayoutSettings.ModelScale(prefab);
                        foreach(var body in item.GetComponentsInChildren<Rigidbody>())body.isKinematic=true;
                        for(int rotation=0;rotation<64;rotation++){
                            item.transform.rotation=Quaternion.Euler((float)random.NextDouble()*360,(float)random.NextDouble()*360,(float)random.NextDouble()*360);
                            foreach(int side in new[]{-1,1}){
                                item.transform.position=new Vector3(side*BoardEnvironment.HorizontalHalfExtent,23.82f,-2.925f);
                                foreach(var mesh in item.GetComponentsInChildren<MeshFilter>()){
                                    var renderer=mesh.GetComponent<Renderer>();if(!renderer||!renderer.enabled||!mesh.sharedMesh)continue;
                                    var bounds=mesh.sharedMesh.bounds;
                                    for(int i=0;i<8;i++){
                                        var p=mesh.transform.TransformPoint(bounds.center+Vector3.Scale(bounds.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1)));
                                        var projected=camera.WorldToViewportPoint(p);
                                        Check(projected.x>=Match3DThemePresentation.EdgePadding-.0001f&&projected.x<=1-Match3DThemePresentation.EdgePadding+.0001f,$"edge clipped: {prefab.name}, aspect={aspect}, x={projected.x}");
                                        Check(projected.y>0&&projected.y<1,"vertical screen clipping: "+prefab.name);
                                        if(Mathf.Abs(p.x)>Match3DThemePresentation.Size(aspect)*aspect)oldClipped++;
                                        checks++;
                                    }
                                }
                            }
                        }
                    } finally {UnityEngine.Object.DestroyImmediate(item);}
                }
                Check(size==Match3DThemePresentation.Size(aspect,width),"stable fit");
                report+=$"PASS aspect={aspect:F4}, orthographicSize={size:F3}; full mesh corners inside screen with 3% horizontal margins\n";
            }
            Check(oldClipped>0,"fixture must reproduce original clipping");
            report+=$"PASS {checks} projected corners; original fit clipped {oldClipped} corners.\n";
            Directory.CreateDirectory("docs/validation");File.WriteAllText("docs/validation/board-viewport-fit.txt",report);Debug.Log(report);
            EditorApplication.Exit(0);
        }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
        finally {UnityEngine.Object.DestroyImmediate(cameraObject);}
    }
}
