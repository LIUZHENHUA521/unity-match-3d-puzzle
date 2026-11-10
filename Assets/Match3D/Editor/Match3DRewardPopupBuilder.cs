using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class Match3DRewardPopupBuilder {
    const string Root = "Assets/Match3D/Resources/Match3D/";
    const string Trigger = "Temp/build-reward-popup";
    static Match3DRewardPopupBuilder() { EditorApplication.update += Poll; }
    static void Poll() {
        if (!File.Exists(Trigger) || EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode) return;
        File.Delete(Trigger);
        try { Build(); File.WriteAllText("Temp/reward-popup-build.txt", "PASS"); }
        catch (Exception e) { File.WriteAllText("Temp/reward-popup-build.txt", e.ToString()); Debug.LogException(e); }
    }
    static RectTransform Rect(string name, Transform parent, float x, float y, float w, float h) =>
        Match3DMailBuilder.Rect(name, parent, x, y, w, h);
    static Image Picture(string name, string asset, Transform parent, float x, float y) {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "RewardSkinPS/" + asset + ".png");
        if (!sprite) throw new InvalidOperationException("Missing reward artwork: " + asset);
        var image = Rect(name, parent, x, y, sprite.rect.width, sprite.rect.height).gameObject.AddComponent<Image>();
        image.sprite = sprite; image.raycastTarget = false;
        return image;
    }
    static Text Label(string name, Transform parent, float x, float y, float w, float h, int size, Color? stroke = null) {
        var text = Rect(name, parent, x, y, w, h).gameObject.AddComponent<Text>();
        text.font = AssetDatabase.LoadAssetAtPath<Font>(Root + "Fonts/DefaultChinese.otf");
        text.fontSize = size; text.fontStyle = FontStyle.Bold;
        text.color = Color.white; text.alignment = TextAnchor.MiddleCenter;
        text.supportRichText = false; text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        if (stroke.HasValue) text.gameObject.AddComponent<Match3DTextStroke>().color = stroke.Value;
        return text;
    }
    [MenuItem("Goose Match3D/UI/Build Received Reward Popup")]
    public static void Build() {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Build in Edit Mode");
        AssetDatabase.Refresh();
        foreach (string path in Directory.GetFiles(Root + "RewardSkinPS", "*.png")) {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true; importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None; importer.wrapMode = TextureWrapMode.Clamp;
            importer.spritePixelsPerUnit = 100; importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
        var root = Rect("RewardReceived", null, 0, 0, 720, 1280);
        try {
            var shade = root.gameObject.AddComponent<Image>(); shade.color = new Color(0, 0, 0, .64f);
            root.gameObject.AddComponent<Match3DUIView>().bindings = new Match3DUIView.Binding[0];
            var popup = root.gameObject.AddComponent<Match3DRewardPopup>();
            var content = Rect("Content - edit UI here", root, 0, 0, 720, 1280);
            root.gameObject.AddComponent<Match3DUIFitContent>().content = content;
            Picture("Congratulations", "title", content, 4, 401);
            var viewport = Rect("Rewards viewport", content, 0, 29, 640, 480);
            var target = viewport.gameObject.AddComponent<Image>(); target.color = Color.clear;
            viewport.gameObject.AddComponent<RectMask2D>();
            popup.scroll = viewport.gameObject.AddComponent<ScrollRect>();
            popup.scroll.viewport = viewport; popup.scroll.horizontal = false;
            popup.scroll.movementType = ScrollRect.MovementType.Clamped; popup.scroll.scrollSensitivity = 35;
            popup.items = Rect("Items", viewport, 0, 0, 0, 480);
            popup.items.anchorMin = new Vector2(0, 1); popup.items.anchorMax = Vector2.one;
            popup.items.pivot = new Vector2(.5f, 1); popup.scroll.content = popup.items;
            var cell = Rect("Reward item template", popup.items, 0, -240, 334, 334);
            popup.itemTemplate = cell.gameObject.AddComponent<Match3DRewardItemView>();
            popup.itemTemplate.glow = Picture("Glow", "light-(2)", cell, 0, 0).rectTransform;
            popup.itemTemplate.icon = Picture("Icon", "image_-redPacket_big_Lingt", cell, 0, -4);
            popup.redPacketIcon = popup.itemTemplate.icon.sprite;
            popup.itemTemplate.caption = Label("Amount", cell, 0, -94, 300, 65, 40, new Color32(242, 101, 128, 255));
            popup.itemTemplate.caption.text = "10元";
            popup.itemTemplate.caption.resizeTextForBestFit = true;
            popup.itemTemplate.caption.resizeTextMinSize = 24; popup.itemTemplate.caption.resizeTextMaxSize = 40;
            cell.gameObject.SetActive(false);
            string[] icons = { "image_btn_remove", "image_btn_adsorpt", "image_btn_shuffle", "img_btn_clock" };
            popup.toolIcons = new Sprite[icons.Length];
            for (int i = 0; i < icons.Length; i++) {
                popup.toolIcons[i] = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "GameplaySkin/" + icons[i] + ".png");
                if (!popup.toolIcons[i]) throw new InvalidOperationException("Missing tool icon " + icons[i]);
            }
            popup.timestamp = Label("Received at", content, 0, -248, 600, 50, 32);
            popup.timestamp.gameObject.SetActive(false);
            var button = Picture("Claim", "image_btn_blue02", content, 7, -348);
            button.raycastTarget = true;
            popup.confirm = button.gameObject.AddComponent<Button>(); popup.confirm.targetGraphic = button;
            // PS: 18.4171295 pt × 2.49767415 transform = 46 px, Bold, 4 px outer stroke.
            popup.buttonLabel = Label("Label", button.transform, -7, 8, 240, 76, 46, new Color32(2, 109, 209, 255));
            popup.buttonLabel.text = "领取";
            popup.buttonLabel.resizeTextForBestFit = true;
            popup.buttonLabel.resizeTextMinSize = 18; popup.buttonLabel.resizeTextMaxSize = 46;
            PrefabUtility.SaveAsPrefabAsset(root.gameObject, Root + "UI/RewardReceived.prefab");
            AssetDatabase.SaveAssets();
        } finally { UnityEngine.Object.DestroyImmediate(root.gameObject); }
    }
}
