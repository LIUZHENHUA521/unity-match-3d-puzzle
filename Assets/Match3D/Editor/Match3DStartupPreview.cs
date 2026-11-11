using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Serialized editor-only artwork makes opening the scene match its runtime home.
// Runtime creates its interactive home in Awake; this preview is stripped from builds.
[InitializeOnLoad]
public static class Match3DStartupPreview {
    const string PreviewName="Startup Home Preview";
    static Match3DStartupPreview(){EditorApplication.update+=Poll;}
    static void Poll(){
        if(!File.Exists("Temp/setup-startup-home")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
        File.Delete("Temp/setup-startup-home");
        try{Apply();File.WriteAllText("Temp/startup-home-setup.txt","PASS saved home preview");}
        catch(Exception e){File.WriteAllText("Temp/startup-home-setup.txt","FAIL "+e);Debug.LogException(e);}
    }
    static Match3DUIView InstantiatePreview(string name,Transform parent){
        var prefab=Resources.Load<Match3DUIView>("Match3D/UI/"+name);
        var root=(GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject);
        if(parent){
            var rect=(RectTransform)root.transform;rect.SetParent(parent,false);
            rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;
            rect.offsetMin=rect.offsetMax=Vector2.zero;rect.localScale=Vector3.one;
            PrefabUtility.RecordPrefabInstancePropertyModifications(rect);
        }
        return root.GetComponent<Match3DUIView>();
    }
    [MenuItem("Goose/UI/Refresh startup home preview")]
    public static void Apply(){
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(EditorApplication.isPlayingOrWillChangePlaymode||scene.path!=Match3DBuilder.Scene||scene.isDirty)
            throw new InvalidOperationException("Open and save Match3D in Edit Mode before refreshing its preview.");
        foreach(var root in scene.GetRootGameObjects())if(root.name==PreviewName)UnityEngine.Object.DestroyImmediate(root);
        Match3DFlow.HideLegacyPresentation();
        Match3DDefaultFont.InitializeFallback();
        var ui=InstantiatePreview("UIRoot",null);ui.name=PreviewName;ui.tag="EditorOnly";
        var home=InstantiatePreview("Home",ui.Get<RectTransform>("Page"));
        var controller=home.GetComponent<Match3DHomeView>();
        if(controller.signIn)controller.signIn.gameObject.SetActive(true);
        ui.gameObject.AddComponent<CanvasGroup>().blocksRaycasts=false;
        PrefabUtility.RecordPrefabInstancePropertyModifications(ui.gameObject);
        if(controller.signIn)PrefabUtility.RecordPrefabInstancePropertyModifications(controller.signIn.gameObject);
        Canvas.ForceUpdateCanvases();
        EditorSceneManager.SaveScene(scene);
    }
}
