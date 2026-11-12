using System.IO;
using Spine.Unity;
using UnityEditor;
using UnityEngine;

public static class ComboProbeAssetBuilder {
    public const string Root = "Assets/Match3D/Resources/Match3D/ComboProbe/";
    [MenuItem("Match3D/Build Combo Probe Assets")]
    public static void Build() {
        foreach (string path in Directory.GetFiles(Root, "*.png")) {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = path.Contains("colorfulCircles") ? TextureImporterType.Default : TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single; importer.spritePixelsPerUnit = 100;
            importer.alphaIsTransparency = true; importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed; importer.maxTextureSize = 2048;
            importer.npotScale = TextureImporterNPOTScale.None; importer.SaveAndReimport();
        }
        var settings = AssetDatabase.LoadAssetAtPath<ComboProbeAssets>(Root + "Assets.asset");
        if (!settings) { settings = ScriptableObject.CreateInstance<ComboProbeAssets>(); AssetDatabase.CreateAsset(settings, Root + "Assets.asset"); }
        settings.panel = Sprite("image_blindBox_blue"); settings.circle = Sprite("image_red_circle");
        settings.darkStar = Sprite("image_Star_black"); settings.lightStar = Sprite("image_Star");
        settings.sunshine = Sprite("sunshine"); settings.track = Sprite("image_combo_load1");
        var material = AssetDatabase.LoadAssetAtPath<Material>(Root + "ProbeRing.mat");
        if (!material) { material = new Material(Shader.Find("Spine/SkeletonGraphic")); AssetDatabase.CreateAsset(material, Root + "ProbeRing.mat"); }
        material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "colorfulCircles_spine.png");
        // This Cocos-exported atlas uses straight alpha, unlike Spine's default PMA workflow.
        material.EnableKeyword("_STRAIGHT_ALPHA_INPUT"); material.SetFloat("_StraightAlphaInput", 1);
        var atlas = AssetDatabase.LoadAssetAtPath<SpineAtlasAsset>(Root + "ProbeRingAtlas.asset");
        if (!atlas) { atlas = ScriptableObject.CreateInstance<SpineAtlasAsset>(); AssetDatabase.CreateAsset(atlas, Root + "ProbeRingAtlas.asset"); }
        atlas.atlasFile = AssetDatabase.LoadAssetAtPath<TextAsset>(Root + "colorfulCircles_spine.atlas.txt");
        atlas.materials = new[] { material }; atlas.Clear();
        var data = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(Root + "ProbeRingSkeleton.asset");
        if (!data) { data = ScriptableObject.CreateInstance<SkeletonDataAsset>(); AssetDatabase.CreateAsset(data, Root + "ProbeRingSkeleton.asset"); }
        data.skeletonJSON = AssetDatabase.LoadAssetAtPath<TextAsset>(Root + "colorfulCircles_spine.json");
        data.atlasAssets = new AtlasAssetBase[] { atlas }; data.scale = .01f; data.Clear();
        if (data.GetSkeletonData(false) == null) throw new System.IO.InvalidDataException("Failed to read the authored Spine 3.8 ring");
        settings.ring = data; settings.ringMaterial = material;
        foreach (var asset in new Object[] { settings, material, atlas, data }) EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets(); Debug.Log("Combo probe art and all five authored Spine animations built");
    }
    static Sprite Sprite(string name) => AssetDatabase.LoadAssetAtPath<Sprite>(Root + name + ".png");
}
