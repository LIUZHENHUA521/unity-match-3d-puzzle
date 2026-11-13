using Spine.Unity;
using UnityEditor;
using UnityEngine;

public static class PotComboPraiseAssetBuilder {
    const string Root = "Assets/Match3D/Resources/Match3D/PotComboPraise/";
    [MenuItem("Match3D/Build Pot Combo Praise Assets")]
    public static void Build() {
        var importer = (TextureImporter)AssetImporter.GetAtPath(Root + "glow_ji.png");
        importer.textureType = TextureImporterType.Default;
        importer.alphaIsTransparency = false; importer.sRGBTexture = false;
        importer.mipmapEnabled = false; importer.npotScale = TextureImporterNPOTScale.None;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 2048; importer.SaveAndReimport();
        var material = AssetDatabase.LoadAssetAtPath<Material>(Root + "PotPraiseMaterial.mat");
        if (!material) { material = new Material(Shader.Find("Spine/SkeletonGraphic")); AssetDatabase.CreateAsset(material, Root + "PotPraiseMaterial.mat"); }
        material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "glow_ji.png");
        material.DisableKeyword("_STRAIGHT_ALPHA_INPUT"); material.SetFloat("_StraightAlphaInput", 0);
        var atlas = AssetDatabase.LoadAssetAtPath<SpineAtlasAsset>(Root + "PotPraiseAtlas.asset");
        if (!atlas) { atlas = ScriptableObject.CreateInstance<SpineAtlasAsset>(); AssetDatabase.CreateAsset(atlas, Root + "PotPraiseAtlas.asset"); }
        atlas.atlasFile = AssetDatabase.LoadAssetAtPath<TextAsset>(Root + "glow_ji.atlas.txt"); atlas.materials = new[] { material }; atlas.Clear();
        var data = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(Root + "PotPraiseSkeleton.asset");
        if (!data) { data = ScriptableObject.CreateInstance<SkeletonDataAsset>(); AssetDatabase.CreateAsset(data, Root + "PotPraiseSkeleton.asset"); }
        data.skeletonJSON = AssetDatabase.LoadAssetAtPath<TextAsset>(Root + "glow_ji.json");
        data.atlasAssets = new AtlasAssetBase[] { atlas }; data.scale = .01f; data.Clear();
        var skeleton = data.GetSkeletonData(false);
        if (skeleton == null || skeleton.Animations.Count != 8) throw new System.Exception("Pot praise Spine animations failed to import");
        foreach (var asset in new Object[] { material, atlas, data }) EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
    }
}
