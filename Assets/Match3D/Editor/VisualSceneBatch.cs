using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using System.IO;using System;using System.Linq;
[InitializeOnLoad]public static class VisualSceneBatch{
 static VisualSceneBatch(){EditorApplication.playModeStateChanged+=Changed;}
 public static void Run(){OriginalModelImporter.ImportMissingSourceModels();BoardModelSizeImporter.Import();BoardLevelPlanImporter.Import();BoardSceneImporter.Import();OriginalModelImporter.ApplySourceShadowFlags();OriginalModelImporter.ApplySourceMaterials();BoardGooseAnimationImporter.Import();BoardToolVisualImporter.Build();SessionState.SetBool("visual-batch",true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
 static void Changed(PlayModeStateChange s){if(!SessionState.GetBool("visual-batch",false)||s!=PlayModeStateChange.EnteredPlayMode)return;EditorApplication.delayCall+=StartFrameChecks;}
 static void StartFrameChecks(){new GameObject("Source validation driver").AddComponent<BoardValidationDriver>().StartCoroutine(FrameChecks());}
 static System.Collections.IEnumerator FrameChecks(){yield return BoardCreationFramesValidation.Run();yield return BoardFlightTimingValidation.Run();yield return BoardSelectionPhysicsFramesValidation.Run();yield return BoardToolsFramesValidation.Run();yield return BoardAudioFramesValidation.Run();yield return BoardToolVisualValidation.RunFrames();Test();}
 static void Test(){
  string report=File.Exists("Temp/goose-source-creation-frames-validation.txt")?File.ReadAllText("Temp/goose-source-creation-frames-validation.txt"):"FAIL missing live creation report\n";bool auto=Physics.autoSimulation;
  report+=File.Exists("Temp/goose-source-flight-timing-validation.txt")?File.ReadAllText("Temp/goose-source-flight-timing-validation.txt"):"FAIL missing live flight report\n";
  report+=File.Exists("Temp/goose-source-tools-frames-validation.txt")?File.ReadAllText("Temp/goose-source-tools-frames-validation.txt"):"FAIL missing live tools report\n";
  report+=File.Exists("Temp/goose-source-audio-frames-validation.txt")?File.ReadAllText("Temp/goose-source-audio-frames-validation.txt"):"FAIL missing live audio report\n";
  report+=File.Exists("Temp/goose-source-tool-visual-frames-validation.txt")?File.ReadAllText("Temp/goose-source-tool-visual-frames-validation.txt"):"FAIL missing live tool visuals report\n";
  report+=File.Exists("Temp/goose-source-selection-physics-frames-validation.txt")?File.ReadAllText("Temp/goose-source-selection-physics-frames-validation.txt"):"FAIL missing source selection physics report\n";
  try{
   Physics.autoSimulation=false;var flow=Match3DFlow.Instance;var camera=Camera.main;Vector3 oldPosition=camera.transform.position;Quaternion oldRotation=camera.transform.rotation;
   Directory.CreateDirectory("visual-captures");
   for(int i=0;i<3;i++){
    flow.Begin(i);flow.NextStage();BoardCreationTestSupport.FinishPending(flow);camera.aspect=9f/16;flow.GetComponent<LayeredItemPile>()?.GetComponent<Match3DThemePresentation>()?.Fit();
    if(Vector3.Distance(camera.transform.position,Match3DThemePresentation.SourcePosition)>.0001f)throw new Exception("camera source pose");
    for(int tick=0;tick<150;tick++){LayeredItemPile.Instance.StepVisibility(.02f);ItemPhysicsController.Instance.Step(.02f);Physics.Simulate(.02f);}
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
   RunCheck("source-tool-visual",()=>{BoardToolVisualValidation.Run();});
   RunCheck("source-tools",()=>{BoardToolsValidation.Run();});
   RunCheck("source-highlight",()=>{BoardHighlightValidation.Run();});
   RunCheck("source-animation",()=>{BoardGooseAnimationValidation.Run();});
   RunCheck("source-audio",()=>{BoardAudioValidation.Run();});
   RunCheck("source-creation",()=>{BoardCreationValidation.Run();});
   RunCheck("source-random",()=>{BoardRandomValidation.Run();});
   RunCheck("source-generation",()=>{BoardGenerationValidation.Run();});
   RunCheck("source-interaction",()=>{BoardInteractionValidation.Run();});
   RunCheck("source-goose-collider",()=>{BoardGooseColliderValidation.Run();});
   RunCheck("source-item-materials",()=>{BoardItemMaterialsValidation.Run();});
   RunCheck("source-item-render",()=>{BoardItemRenderValidation.Run();});
   RunCheck("source-ambient",()=>{BoardAmbientValidation.Run();});
   RunCheck("camera-motion",()=>{BoardCameraMotionValidation.Run();});
   RunCheck("source-shadow",()=>{BoardShadowValidation.Run();});
   RunCheck("source-visual",()=>{BoardVisualValidation.Run();});
   RunCheck("physics-theme",()=>{PhysicsThemeValidation.Run();});
   RunCheck("source-alignment",()=>{BoardAlignmentValidation.Run();});
   RunCheck("reveal-timing",()=>{RevealTimingValidation.Run();});
   RunCheck("height-physics",()=>{HeightPhysicsValidation.Run();});
   RunCheck("selection",()=>{SelectionFeedbackValidation.Run();});
   RunCheck("source-object-pool",()=>{BoardObjectPoolValidation.Run();});

  }catch(Exception e){report+="FAIL "+e;}
  finally{Match3DFlow.Instance.Home();Physics.autoSimulation=auto;File.WriteAllText("visual-batch-result.txt",report);Debug.Log(report);SessionState.SetBool("visual-batch",false);EditorApplication.Exit(report.Contains("FAIL")?1:0);}
 }
 static void Capture(Camera c,string path){var rt=new RenderTexture(720,1280,24);var old=c.targetTexture;var active=RenderTexture.active;c.targetTexture=rt;rt.Create();c.Render();RenderTexture.active=rt;var tex=new Texture2D(720,1280,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,720,1280),0,0);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());c.targetTexture=old;RenderTexture.active=active;UnityEngine.Object.DestroyImmediate(tex);rt.Release();UnityEngine.Object.DestroyImmediate(rt);}
}
