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
    public static void Prepare(string scenePath) {
        if (!File.Exists(".goose-build-stage"))
            throw new InvalidOperationException("Release asset optimization requires the disposable build stage.");
        const string root = "Assets/Match3D/Resources/Match3D/";
        Atlas("WechatInvite", AssetDatabase.FindAssets("t:Texture2D", new[]{root+"InviteSkinPS"})
            .Select(AssetDatabase.GUIDToAssetPath).ToArray());
        Atlas("WechatBackgrounds", new[]{root+"HomeSkinPS/img_Homepage_bg2.jpg", root+"GameplaySkin/img_picnic_bg.jpg"});

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

    static void Atlas(string name, string[] paths) {
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
        platform.overridden=true; platform.maxTextureSize=2048;
        platform.format=TextureImporterFormat.DXT5; platform.textureCompression=TextureImporterCompression.CompressedHQ;
        platform.compressionQuality=100; atlas.SetPlatformSettings(platform);
        atlas.Add(sprites); atlas.SetIncludeInBuild(true); EditorUtility.SetDirty(atlas);
        Debug.Log("WECHAT_SIZE: packed "+sprites.Length+" sprites into "+name);
    }
}
