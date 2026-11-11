using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class Match3DStartupBuilder {
    const string AssetsRoot="Assets/Match3D/Resources/Match3D/";
    public const string CoverPath="Assets/Match3D/Art/Startup/WechatCover.png";
    public const string ShortCoverPath="Assets/Match3D/Art/Startup/WechatCoverShort.png";
    public const string ArtworkPath="Assets/Match3D/Art/Startup/ToyLoadingLong.png";
    public const string ArtworkDataPath="Assets/Match3D/Art/Startup/ToyLoadingLong.bytes";
    static Match3DStartupBuilder(){EditorApplication.update+=Poll;}
    static void Poll(){
        if(!File.Exists("Temp/build-startup-loading")||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
        File.Delete("Temp/build-startup-loading");
        try{Build();File.WriteAllText("Temp/startup-loading-build.txt","PASS prefab and WeChat cover exported");}
        catch(Exception e){File.WriteAllText("Temp/startup-loading-build.txt","FAIL "+e);Debug.LogException(e);}
    }
    [MenuItem("Goose/UI/Build startup loading")]
    public static void Build(){
        // Ship a full-resolution JPEG instead of a downscaled, block-compressed GPU texture.
        var source=new Texture2D(2,2,TextureFormat.RGB24,false);
        try{
            if(!ImageConversion.LoadImage(source,File.ReadAllBytes(ArtworkPath)))throw new Exception("Cannot decode approved artwork");
            File.WriteAllBytes(ArtworkDataPath,source.EncodeToJPG(95));
        }finally{UnityEngine.Object.DestroyImmediate(source);}
        AssetDatabase.ImportAsset(ArtworkDataPath);
        foreach(string name in new[]{"Age8"}){
            string path=AssetsRoot+"Startup/"+name+".png";
            AssetDatabase.ImportAsset(path);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
            importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.maxTextureSize=2048;
            importer.textureCompression=TextureImporterCompression.CompressedHQ;importer.SaveAndReimport();
        }
        var root=Rect("StartupLoading",null,Vector2.zero,new Vector2(720,1280));
        try{
            root.gameObject.AddComponent<Image>().color=new Color32(168,218,246,255);
            var view=root.gameObject.AddComponent<Match3DStartupView>();
            var bindings=root.gameObject.AddComponent<Match3DUIView>();bindings.bindings=new Match3DUIView.Binding[0];
            view.artwork=Rect("Artwork",root,Vector2.zero,new Vector2(720,1600)).gameObject.AddComponent<Image>();
            view.artwork.raycastTarget=false;
            view.artworkData=AssetDatabase.LoadAssetAtPath<TextAsset>(ArtworkDataPath);
            var content=Rect("Content",root,Vector2.zero,new Vector2(720,1280));view.content=content;
            Picture("Age8",content,new Vector2(92,129),new Vector2(78,104),"Age8");
            Label("HealthTitle",content,new Vector2(360,200),new Vector2(440,30),"健康游戏忠告",23,FontStyle.Bold);
            Label("HealthAdvice",content,new Vector2(360,130),new Vector2(440,114),Match3DStartupView.HealthAdvice,21,FontStyle.Normal);
            var track=Rect("ProgressTrack",content,new Vector2(0,-396),new Vector2(450,12));
            track.gameObject.AddComponent<Image>().color=new Color32(98,150,179,150);
            var fill=Rect("Progress",track,Vector2.zero,new Vector2(450,12));
            var image=fill.gameObject.AddComponent<Image>();image.color=new Color32(38,131,183,255);
            fill.anchorMin=Vector2.zero;fill.anchorMax=new Vector2(0,1);fill.offsetMin=fill.offsetMax=Vector2.zero;
            image.fillAmount=0;
            view.progress=image;
            view.status=Label("Status",content,new Vector2(360,274),new Vector2(650,34),"正在准备游戏…",22,FontStyle.Normal);
            var retry=Rect("Retry",content,new Vector2(0,-396),new Vector2(190,40));
            var retryImage=retry.gameObject.AddComponent<Image>();retryImage.color=new Color32(46,137,192,255);
            view.retry=retry.gameObject.AddComponent<Button>();view.retry.targetGraphic=retryImage;
            var retryLabel=Label("RetryLabel",retry,new Vector2(360,640),new Vector2(180,42),"重试",24,FontStyle.Bold);retryLabel.color=Color.white;
            retry.gameObject.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root.gameObject,AssetsRoot+"UI/StartupLoading.prefab");
            Render(root.gameObject,CoverPath,841,1870,true);
            Render(root.gameObject,ShortCoverPath,841,1495,true);
            AssetDatabase.SaveAssets();AssetDatabase.ImportAsset(CoverPath);AssetDatabase.ImportAsset(ShortCoverPath);
        }finally{UnityEngine.Object.DestroyImmediate(root.gameObject);}
    }
    static RectTransform Rect(string name,Transform parent,Vector2 pos,Vector2 size){
        var rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();rect.SetParent(parent,false);
        rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);rect.sizeDelta=size;rect.anchoredPosition=pos;return rect;
    }
    static RectTransform Picture(string name,Transform parent,Vector2 center,Vector2 size,string asset){
        var image=Rect(name,parent,center-new Vector2(360,640),size).gameObject.AddComponent<Image>();
        image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(AssetsRoot+"Startup/"+asset+".png");image.raycastTarget=false;
        return image.rectTransform;
    }
    static Text Label(string name,Transform parent,Vector2 center,Vector2 size,string value,int fontSize,FontStyle style){
        var text=Rect(name,parent,center-new Vector2(360,640),size).gameObject.AddComponent<Text>();
        text.font=AssetDatabase.LoadAssetAtPath<Font>(AssetsRoot+"Fonts/DefaultChinese.otf");text.fontSize=fontSize;text.fontStyle=style;
        text.text=value;text.color=new Color32(39,74,95,255);text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;
        text.horizontalOverflow=HorizontalWrapMode.Overflow;text.verticalOverflow=VerticalWrapMode.Overflow;return text;
    }
    // Render authored UI, preserving the source badge pixels and real font glyphs.
    // The SDK can show this before C# starts; no AI redraw of the source notice/icon.
    public static void Render(GameObject root,string path,int width,int height,bool cover){
        var scene=EditorSceneManager.NewPreviewScene();
        var canvasRoot=new GameObject("Startup cover canvas",typeof(RectTransform),typeof(Canvas));
        var cameraRoot=new GameObject("Startup cover camera",typeof(Camera));
        var rt=new RenderTexture(width,height,24);Texture2D capture=null;var previous=RenderTexture.active;
        try{
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(canvasRoot,scene);
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraRoot,scene);
            var camera=cameraRoot.GetComponent<Camera>();camera.orthographic=true;camera.orthographicSize=640;camera.nearClipPlane=.1f;camera.farClipPlane=100;
            camera.transform.position=new Vector3(0,0,-10);camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color32(168,218,246,255);camera.targetTexture=rt;camera.scene=scene;
            var canvas=canvasRoot.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
            var copy=UnityEngine.Object.Instantiate(root,canvasRoot.transform,false);var rect=(RectTransform)copy.transform;
            rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
            var view=copy.GetComponent<Match3DStartupView>();
            if(cover){view.status.gameObject.SetActive(false);view.progress.transform.parent.gameObject.SetActive(false);}
            else view.SetProgress(.65f,"正在同步账号数据…");
            Canvas.ForceUpdateCanvases();view.Fit();Canvas.ForceUpdateCanvases();camera.Render();
            RenderTexture.active=rt;capture=new Texture2D(width,height,TextureFormat.RGB24,false);capture.ReadPixels(new Rect(0,0,width,height),0,0);capture.Apply();
            Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllBytes(path,capture.EncodeToPNG());
        }finally{RenderTexture.active=previous;if(capture)UnityEngine.Object.DestroyImmediate(capture);rt.Release();UnityEngine.Object.DestroyImmediate(rt);EditorSceneManager.ClosePreviewScene(scene);}
    }
}
