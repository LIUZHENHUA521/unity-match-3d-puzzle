using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class FoodPilePreview
{
    static double ready;
    static bool picked;
    static FoodPilePreview() { EditorApplication.update += Update; }
    public static void Run() {
        EditorSceneManager.OpenScene(FoodPileBuilder.ScenePath);
        SessionState.SetBool("FoodPilePreview",true);
        EditorApplication.isPlaying=true;
    }
    static void Update() {
        if (!SessionState.GetBool("FoodPilePreview",false) || !EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        if (ready==0) { ready=EditorApplication.timeSinceStartup+7; return; }
        if (EditorApplication.timeSinceStartup<ready) return;
        if (!picked) {
            picked=true;
            var berry=LevelGenerator.Instance.objectsOnScene.FirstOrDefault(x=>x.name.StartsWith("strawberry"));
            if(berry) InputController.Instance.AddObjectOnStack(berry);
            ready=EditorApplication.timeSinceStartup+.7;
            return;
        }
        SessionState.SetBool("FoodPilePreview",false);
        var camera=Camera.main;
        camera.GetComponent<CameraFitter>().enabled=false;
        camera.rect=new Rect(0,0,1,1);
        var rt=new RenderTexture(720,1280,24);
        camera.targetTexture=rt;camera.aspect=720f/1280;
        Canvas.ForceUpdateCanvases();camera.Render();
        RenderTexture.active=rt;
        var texture=new Texture2D(720,1280,TextureFormat.RGB24,false);
        texture.ReadPixels(new Rect(0,0,720,1280),0,0);texture.Apply();
        File.WriteAllBytes(Path.GetFullPath("../foodpile-preview.png"),texture.EncodeToPNG());
        camera.targetTexture=null; RenderTexture.active=null;
        Object.DestroyImmediate(texture);Object.DestroyImmediate(rt);
        Debug.Log("FOOD_PILE_PREVIEW_SAVED");EditorApplication.Exit(0);
    }
}
