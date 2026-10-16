using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

[InitializeOnLoad]
public static class HomeSkinAssets {
    const string Root="Assets/GooseReplica/Resources/Replica/HomeSkin";
    static HomeSkinAssets(){EditorApplication.delayCall+=Ensure;}
    public static void Ensure(){
        if(EditorApplication.isPlayingOrWillChangePlaymode||!AssetDatabase.IsValidFolder(Root+"/Icons"))return;
        const string path=Root+"/HomeUI.spriteatlas";
        if(AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path))return;
        var atlas=new SpriteAtlas();
        atlas.SetPackingSettings(new SpriteAtlasPackingSettings{enableRotation=false,enableTightPacking=false,padding=4});
        atlas.SetTextureSettings(new SpriteAtlasTextureSettings{readable=false,generateMipMaps=false,sRGB=true,filterMode=FilterMode.Bilinear});
        var platform=atlas.GetPlatformSettings("DefaultTexturePlatform");platform.maxTextureSize=1024;atlas.SetPlatformSettings(platform);
        atlas.Add(new[]{AssetDatabase.LoadAssetAtPath<Object>(Root+"/Icons")});atlas.SetIncludeInBuild(true);
        AssetDatabase.CreateAsset(atlas,path);AssetDatabase.SaveAssets();
    }
}
