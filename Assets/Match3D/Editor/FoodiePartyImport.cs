using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

// One-time asset migration; never modifies source toy or food prefabs.
[InitializeOnLoad]
public static class FoodiePartyImport {
    const string Inputs = "Assets/Match3D/IncomingModels/FoodieParty";
    const string Report = "work/foodie-party";
    static readonly string[] Toys = {"Bottle", "BreadSlice", "Burger", "ChocolateBar", "Cupcake", "Donut", "Egg", "Eggplant", "Fries", "Hotdog", "IceCream", "KetchupBottle", "Lettuce", "PancakesStack", "PepperGreen", "Pizza", "Popsicle", "Pumpkin", "Soda", "Steak", "Tentacle", "Tomato", "Waffle"};
    static readonly string[] Labels = {"饮料瓶", "吐司片", "汉堡", "巧克力棒", "纸杯蛋糕", "甜甜圈", "鸡蛋", "茄子", "薯条", "热狗", "冰淇淋", "番茄酱", "生菜", "松饼", "青椒", "披萨", "冰棒", "南瓜", "汽水", "牛排", "章鱼触手", "番茄", "华夫饼"};
    static FoodiePartyImport() { EditorApplication.update += Poll; }
    static void Poll() {
        const string request = "Temp/foodie-party-command";
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(request)) return;
        var command = File.ReadAllText(request).Trim(); File.Delete(request);
        try {
            if (command == "build") Build();
            else if (command == "validate") FoodiePartyValidation.Start();
            else throw new ArgumentException("Unknown toy command: " + command);
            File.WriteAllText("Temp/foodie-party-result", "OK " + command);
        } catch (Exception ex) { File.WriteAllText("Temp/foodie-party-result", "FAIL " + ex); Debug.LogException(ex); }
    }
    public static void Build() {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play before replacing models.");
        var catalog = AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>(ThemeItemsEditorService.CatalogPath);
        var draft = ThemeItemsEditorService.Load(catalog, "CountrysideStove");
        if (draft.rows.Count != Toys.Length || draft.rows.Any(r => !r.prefab.name.StartsWith("Kitchen_")))
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
            string sourcePath = Directory.GetFiles(Directory.GetDirectories(Inputs, (i + 1).ToString("000") + "_*").Single(), "*.obj").Single();
            var report = ModelAdapterService.Generate(sourcePath, "Party_" + Toys[i], Labels[i], true, true,
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
            item.sizeReason = "吃货派对：沿用 " + old.id + " 槽位大小";
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
        themeEntry.title = "吃货派对";
        themeEntry.subtitle = "吃货集合，每天一道挑战";
        EditorUtility.SetDirty(library); AssetDatabase.SaveAssetIfDirty(library);
        File.WriteAllText(Report + "/mapping.json", map.ToString());
        File.WriteAllText(Report + "/build.txt", "PASS: 23 foodie party adapters; all theme references valid; non-model JSON parameters unchanged.\nTheme backup: " + backup);
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
    }
}
