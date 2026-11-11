using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

// Release-only transformations: never change artwork in the working project.
public static class WechatReleaseAssets {
    public const string StartupCoverPath="Assets/Match3D/Art/Startup/WechatCover.jpg";
    public const string StartupShortCoverPath="Assets/Match3D/Art/Startup/WechatCoverShort.jpg";
    public static void Prepare(string scenePath) {
        if (!File.Exists(".goose-build-stage"))
            throw new InvalidOperationException("Release asset optimization requires the disposable build stage.");
        const string root = "Assets/Match3D/Resources/Match3D/";
        // Keep the editable source art intact; the WeChat main package only needs JPEG.
        EncodeCover(Match3DStartupBuilder.CoverPath,StartupCoverPath);
        EncodeCover(Match3DStartupBuilder.ShortCoverPath,StartupShortCoverPath);
        Atlas("WechatInvite", AssetDatabase.FindAssets("t:Texture2D", new[]{root+"InviteSkinPS"})
            .Select(AssetDatabase.GUIDToAssetPath).ToArray());
        // Home already synchronously replaces this sprite from the original JPEG
        // before its first frame. Keep only a tiny named placeholder in its atlas.
        if(!File.Exists(root+"HomeBackgroundOriginal.bytes"))throw new InvalidOperationException("Missing full-resolution home background");
        var homeImporter=(TextureImporter)AssetImporter.GetAtPath(root+"HomeSkinPS/img_Homepage_bg2.jpg");
        var homeSettings=homeImporter.GetPlatformTextureSettings("WebGL");
        homeSettings.overridden=true;homeSettings.maxTextureSize=32;homeSettings.format=TextureImporterFormat.DXT1;
        homeImporter.SetPlatformTextureSettings(homeSettings);homeImporter.SaveAndReimport();
        Atlas("WechatBackgrounds", new[]{root+"HomeSkinPS/img_Homepage_bg2.jpg"},TextureImporterFormat.DXT1,64);
        // Opaque gameplay backdrops need no alpha channel. Limit only release
        // sprites to 1024px so their decoded textures fit the mini-game package.
        var backgrounds = new[]{root+"ThemeBackgrounds/playful-toys.jpg", root+"ThemeBackgrounds/tasty-kitchen.jpg", root+"ThemeBackgrounds/foodie-party.jpg"};
        foreach (var path in backgrounds) {
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            var settings=importer.GetPlatformTextureSettings("WebGL");
            settings.overridden=true; settings.maxTextureSize=1024;
            settings.format=TextureImporterFormat.DXT1;
            settings.textureCompression=TextureImporterCompression.CompressedHQ;
            importer.SetPlatformTextureSettings(settings); importer.SaveAndReimport();
        }
        Atlas("WechatThemeBackgrounds", backgrounds, TextureImporterFormat.DXT1);

        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        var renderers = scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Renderer>(true)).ToArray();
        if (!renderers.Any(x=>x.lightmapIndex>=0 && x.lightmapIndex<65534)) {
            Lightmapping.lightingDataAsset = null;
            LightmapSettings.lightmaps = new LightmapData[0];
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("WECHAT_SIZE: removed unused baked lighting from release scene");
        } else Debug.Log("WECHAT_SIZE: keeping baked lighting referenced by scene renderers");
        // Include-in-build atlases may contain sprites used by legacy UI. Keep
        // UIAtlas unless dependency analysis proves those sprites unreachable.
        AssetDatabase.SaveAssets();
        SpriteAtlasUtility.PackAllAtlases(BuildTarget.WebGL);
    }

    static void EncodeCover(string source,string target){
        var cover=new Texture2D(2,2,TextureFormat.RGB24,false);
        try{
            if(!ImageConversion.LoadImage(cover,File.ReadAllBytes(source)))
                throw new InvalidOperationException("Cannot decode startup cover");
            File.WriteAllBytes(target,cover.EncodeToJPG(95));
        }finally{UnityEngine.Object.DestroyImmediate(cover);}
        AssetDatabase.ImportAsset(target);
    }

    static void Atlas(string name, string[] paths, TextureImporterFormat format=TextureImporterFormat.DXT5,int maxSize=2048) {
        var sprites = paths.Select(p=>AssetDatabase.LoadAssetAtPath<Sprite>(p)).ToArray();
        if (sprites.Length==0 || sprites.Any(x=>x==null))
            throw new InvalidOperationException("Missing release atlas sprites: "+name);
        string path="Assets/Match3D/"+name+".spriteatlas";
        var atlas=AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
        if (!atlas) { atlas=new SpriteAtlas(); AssetDatabase.CreateAsset(atlas,path); }
        atlas.Remove(atlas.GetPackables());
        atlas.SetPackingSettings(new SpriteAtlasPackingSettings{enableRotation=false,enableTightPacking=false,padding=4});
        atlas.SetTextureSettings(new SpriteAtlasTextureSettings{readable=false,generateMipMaps=false,sRGB=true,filterMode=FilterMode.Bilinear});
        var platform=atlas.GetPlatformSettings("WebGL");
        platform.overridden=true; platform.maxTextureSize=maxSize;
        platform.format=format; platform.textureCompression=TextureImporterCompression.CompressedHQ;
        platform.compressionQuality=100; atlas.SetPlatformSettings(platform);
        atlas.Add(sprites); atlas.SetIncludeInBuild(true); EditorUtility.SetDirty(atlas);
        Debug.Log("WECHAT_SIZE: packed "+sprites.Length+" sprites into "+name);
    }
}
