using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using System.IO;using System;using System.Linq;
[InitializeOnLoad]public static class VisualSceneBatch{
 static VisualSceneBatch(){EditorApplication.playModeStateChanged+=Changed;}
 public static void Run(){OriginalModelImporter.ImportMissingSourceModels();SourceModelSizeImporter.Import();SourceLevelPlanImporter.Import();SourceSceneImporter.Import();OriginalModelImporter.ApplySourceShadowFlags();OriginalModelImporter.ApplySourceMaterials();SourceGooseAnimationImporter.Import();SourceToolVisualImporter.Build();SessionState.SetBool("visual-batch",true);EditorSceneManager.OpenScene(ReplicaBuilder.Scene);EditorApplication.isPlaying=true;}
 static void Changed(PlayModeStateChange s){if(!SessionState.GetBool("visual-batch",false)||s!=PlayModeStateChange.EnteredPlayMode)return;EditorApplication.delayCall+=StartFrameChecks;}
 static void StartFrameChecks(){new GameObject("Source validation driver").AddComponent<SourceValidationDriver>().StartCoroutine(FrameChecks());}
 static System.Collections.IEnumerator FrameChecks(){yield return SourceCreationFramesValidation.Run();yield return SourceFlightTimingValidation.Run();yield return SourceSelectionPhysicsFramesValidation.Run();yield return SourceToolsFramesValidation.Run();yield return SourceAudioFramesValidation.Run();yield return SourceToolVisualValidation.RunFrames();Test();}
 static void Test(){
  string report=File.Exists("Temp/goose-source-creation-frames-validation.txt")?File.ReadAllText("Temp/goose-source-creation-frames-validation.txt"):"FAIL missing live creation report\n";bool auto=Physics.autoSimulation;
  report+=File.Exists("Temp/goose-source-flight-timing-validation.txt")?File.ReadAllText("Temp/goose-source-flight-timing-validation.txt"):"FAIL missing live flight report\n";
  report+=File.Exists("Temp/goose-source-tools-frames-validation.txt")?File.ReadAllText("Temp/goose-source-tools-frames-validation.txt"):"FAIL missing live tools report\n";
  report+=File.Exists("Temp/goose-source-audio-frames-validation.txt")?File.ReadAllText("Temp/goose-source-audio-frames-validation.txt"):"FAIL missing live audio report\n";
  report+=File.Exists("Temp/goose-source-tool-visual-frames-validation.txt")?File.ReadAllText("Temp/goose-source-tool-visual-frames-validation.txt"):"FAIL missing live tool visuals report\n";
  report+=File.Exists("Temp/goose-source-selection-physics-frames-validation.txt")?File.ReadAllText("Temp/goose-source-selection-physics-frames-validation.txt"):"FAIL missing source selection physics report\n";
  try{
   Physics.autoSimulation=false;var flow=ReplicaFlow.Instance;var camera=Camera.main;Vector3 oldPosition=camera.transform.position;Quaternion oldRotation=camera.transform.rotation;
   Directory.CreateDirectory("visual-captures");
   for(int i=0;i<3;i++){
    flow.Begin(i);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);camera.aspect=9f/16;flow.GetComponent<ReplicaLayeredPile>()?.GetComponent<ReplicaThemePresentation>()?.Fit();
    if(Vector3.Distance(camera.transform.position,ReplicaThemePresentation.SourcePosition)>.0001f)throw new Exception("camera source pose");
    for(int tick=0;tick<150;tick++){ReplicaLayeredPile.Instance.StepVisibility(.02f);ReplicaPilePhysics.Instance.Step(.02f);Physics.Simulate(.02f);}
    Capture(camera,"visual-captures/theme-"+i+"-pile.png");
    foreach(var obj in LevelGenerator.Instance.objectsOnScene)obj.SetActive(false);
    Capture(camera,"visual-captures/theme-"+i+"-empty.png");
    report+="PASS source visual/theme "+i+"; source camera pose; rendered pile and empty scene.\n";flow.Home();
    if(Vector3.Distance(camera.transform.position,oldPosition)>.0001f||Quaternion.Angle(camera.transform.rotation,oldRotation)>.001f)throw new Exception("camera restore");
   }
   void RunCheck(string name,Action check){
    string path="Temp/goose-"+name+"-validation.txt";if(File.Exists(path))File.Delete(path);
    try{check();report+=File.Exists(path)?File.ReadAllText(path)+"\n":"FAIL missing validation report "+name+"\n";}
    catch(Exception e){report+="FAIL "+name+": "+e+"\n";}
   }
   RunCheck("source-tool-visual",()=>{SourceToolVisualValidation.Run();});
   RunCheck("source-tools",()=>{SourceToolsValidation.Run();});
   RunCheck("source-highlight",()=>{SourceHighlightValidation.Run();});
   RunCheck("source-animation",()=>{SourceGooseAnimationValidation.Run();});
   RunCheck("source-audio",()=>{SourceAudioValidation.Run();});
   RunCheck("source-creation",()=>{SourceCreationValidation.Run();});
   RunCheck("source-random",()=>{SourceRandomValidation.Run();});
   RunCheck("source-generation",()=>{SourceGenerationValidation.Run();});
   RunCheck("source-interaction",()=>{SourceInteractionValidation.Run();});
   RunCheck("source-goose-collider",()=>{SourceGooseColliderValidation.Run();});
   RunCheck("source-item-materials",()=>{SourceItemMaterialsValidation.Run();});
   RunCheck("source-item-render",()=>{SourceItemRenderValidation.Run();});
   RunCheck("source-ambient",()=>{SourceAmbientValidation.Run();});
   RunCheck("camera-motion",()=>{SourceCameraMotionValidation.Run();});
   RunCheck("source-shadow",()=>{SourceShadowValidation.Run();});
   RunCheck("source-visual",()=>{SourceVisualValidation.Run();});
   RunCheck("physics-theme",()=>{PhysicsThemeValidation.Run();});
   RunCheck("source-alignment",()=>{SourceAlignmentValidation.Run();});
   RunCheck("reveal-timing",()=>{RevealTimingValidation.Run();});
   RunCheck("height-physics",()=>{HeightPhysicsValidation.Run();});
   RunCheck("selection",()=>{SelectionFeedbackValidation.Run();});
   RunCheck("source-object-pool",()=>{SourceObjectPoolValidation.Run();});

  }catch(Exception e){report+="FAIL "+e;}
  finally{ReplicaFlow.Instance.Home();Physics.autoSimulation=auto;File.WriteAllText("visual-batch-result.txt",report);Debug.Log(report);SessionState.SetBool("visual-batch",false);EditorApplication.Exit(report.Contains("FAIL")?1:0);}
 }
 static void Capture(Camera c,string path){var rt=new RenderTexture(720,1280,24);var old=c.targetTexture;var active=RenderTexture.active;c.targetTexture=rt;rt.Create();c.Render();RenderTexture.active=rt;var tex=new Texture2D(720,1280,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,720,1280),0,0);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());c.targetTexture=old;RenderTexture.active=active;UnityEngine.Object.DestroyImmediate(tex);rt.Release();UnityEngine.Object.DestroyImmediate(rt);}
}
