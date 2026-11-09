using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

// One-time asset migration; never modifies source toy or food prefabs.
[InitializeOnLoad]
public static class TastyKitchenImport {
    const string Inputs = "Assets/Match3D/IncomingModels/TastyKitchen";
    const string Report = "work/tasty-kitchen";
    static readonly string[] Toys = { "bag-flat", "bottle-oil", "bowl-broth", "bread", "burger-cheese", "cake", "carton-small", "cauliflower", "chocolate", "cookie-chocolate", "cookie", "donut-chocolate", "fries", "meat-cooked", "paprika", "pizza", "popsicle", "pumpkin", "skewer-vegetables", "soda-can", "soda-glass", "sundae", "whole-ham" };
    static readonly string[] Labels = { "食品袋", "食用油", "汤碗", "面包", "芝士汉堡", "蛋糕", "饮品纸盒", "菜花", "巧克力", "巧克力饼干", "曲奇", "巧克力甜甜圈", "薯条", "熟肉", "彩椒", "披萨", "冰棒", "南瓜", "蔬菜串", "罐装汽水", "杯装汽水", "圣代", "火腿" };
    static TastyKitchenImport() { EditorApplication.update += Poll; }
    static void Poll() {
        const string request = "Temp/tasty-kitchen-command";
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(request)) return;
        var command = File.ReadAllText(request).Trim(); File.Delete(request);
        try {
            if (command == "build") Build();
            else if (command == "validate") TastyKitchenValidation.Start();
            else throw new ArgumentException("Unknown toy command: " + command);
            File.WriteAllText("Temp/tasty-kitchen-result", "OK " + command);
        } catch (Exception ex) { File.WriteAllText("Temp/tasty-kitchen-result", "FAIL " + ex); Debug.LogException(ex); }
    }
    public static void Build() {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play before replacing models.");
        var catalog = AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>(ThemeItemsEditorService.CatalogPath);
        var draft = ThemeItemsEditorService.Load(catalog, "CountrysideStove");
        if (draft.rows.Count != Toys.Length || draft.rows.Any(r => !r.prefab.name.StartsWith("Toy")))
            throw new InvalidOperationException("Expected the original 23 food slots; migration already applied or pool changed.");
        var oldData = JsonUtility.FromJson<ItemCatalog.Data>(File.ReadAllText(ModelAdapterService.CatalogPath));
        var oldItems = draft.rows.Select(r => oldData.items.Single(i => i.id == r.prefab.name)).ToArray();
        var oldPrefabs = draft.rows.Select(r => r.prefab).ToArray();
        var tray = Match3DTrayDisplaySettings.Current;
        var configBefore = JObject.Parse(File.ReadAllText(ThemeItemsEditorService.SourcePath));
        Directory.CreateDirectory(Inputs); Directory.CreateDirectory(Report); AssetDatabase.Refresh();
        var generated = new GameObject[Toys.Length];
        var map = new JArray();
        for (int i = 0; i < Toys.Length; i++) {
            File.WriteAllText(Report + "/progress.txt", (i + 1) + "/23 " + Toys[i]);
            string sourcePath = Directory.GetFiles(Inputs, Toys[i] + ".obj", SearchOption.AllDirectories).Single();
            var report = ModelAdapterService.Generate(sourcePath, "Kitchen_" + Toys[i], Labels[i], true, true,
                new ModelAdapterResourceOptions { optimizeTextures = false });
            if (report.HasErrors || report.status != "Generated") throw new InvalidOperationException(JsonUtility.ToJson(report, true));
            generated[i] = AssetDatabase.LoadAssetAtPath<GameObject>(report.generatedPrefabPath);
            map.Add(new JObject { ["old"] = oldItems[i].id, ["model"] = generated[i].name, ["label"] = Labels[i], ["source"] = sourcePath });
        }
        // Retain per-slot authored board size and audio rather than applying name-based toy defaults.
        var data = JsonUtility.FromJson<ItemCatalog.Data>(File.ReadAllText(ModelAdapterService.CatalogPath));
        for (int i = 0; i < Toys.Length; i++) {
            var item = data.items.Single(x => x.id == generated[i].name);
            var old = oldItems[i];
            item.normalizedToSource = old.normalizedToSource;
            item.trayBaseScale = old.trayBaseScale;
            item.sizeTier = old.sizeTier;
            item.sizeReason = "美味厨房：沿用 " + old.id + " 槽位大小";
            item.audioProfile = JsonUtility.FromJson<ItemCatalog.AudioProfile>(JsonUtility.ToJson(old.audioProfile));
            item.audioProfile.name = item.id;
        }
        File.WriteAllText(ModelAdapterService.CatalogPath, JsonUtility.ToJson(data, true));
        AssetDatabase.ImportAsset(ModelAdapterService.CatalogPath);
        BoardLayoutSettings.ClearCache(); ItemColliderGeometry.ClearCache(); GameAudioCatalog.ClearCache();
        for (int i = 0; i < Toys.Length; i++) ThemeItemsEditorService.Replace(draft, i, generated[i]);
        var errors = ThemeItemsEditorService.Validate(draft);
        if (errors.Count > 0) throw new InvalidOperationException(string.Join("\n", errors));
        // Toy-specific tray pose is model presentation; old entries stay available for restoration.
        for (int i = 0; i < Toys.Length; i++) {
            tray.SetBoardMultiplier(generated[i], tray.BoardMultiplier(oldPrefabs[i]));
            tray.Set(generated[i], new Match3DTrayDisplaySettings.Pose { rotation = new Vector3(-90, 0, 0), scale = 1 });
        }
        EditorUtility.SetDirty(tray); AssetDatabase.SaveAssetIfDirty(tray);
        string backup = ThemeItemsEditorService.Save(draft);
        var after = JObject.Parse(File.ReadAllText(ThemeItemsEditorService.SourcePath));
        foreach (var root in new[] { configBefore, after }) foreach (var theme in root["themes"].OfType<JObject>()) {
            theme.Remove("names"); theme.Remove("prefabPaths"); theme.Remove("tutorialNames");
        }
        if (!JToken.DeepEquals(configBefore, after)) throw new InvalidOperationException("Unexpected non-model level parameter change.");
        var library = AssetDatabase.LoadAssetAtPath<Match3DTheme>("Assets/Match3D/Resources/Match3D/ThemeLibrary.asset");
        var themeEntry = library.themes.Single(t => t.sourceEnvironment == "CountrysideStove");
        themeEntry.title = "美味厨房";
        themeEntry.subtitle = "缤纷美食，每天一道挑战";
        EditorUtility.SetDirty(library); AssetDatabase.SaveAssetIfDirty(library);
        File.WriteAllText(Report + "/mapping.json", map.ToString());
        File.WriteAllText(Report + "/build.txt", "PASS: 23 kitchen adapters; all theme references valid; non-model JSON parameters unchanged.\nTheme backup: " + backup);
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
    }
}
