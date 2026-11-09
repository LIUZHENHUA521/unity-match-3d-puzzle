using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

// One-time asset migration; never modifies source toy or food prefabs.
[InitializeOnLoad]
public static class ToyThemeReplacement {
    const string Inputs = "Assets/Match3D/IncomingModels/ToyTheme";
    const string Report = "work/toy-theme-20261109";
    static readonly string[] Toys = { "Bear", "Dinosaur", "WoodToy", "Doll", "Bucket", "WaterGun", "Bunny", "Boat", "RollerSkate", "Robot", "Horse", "Minibus", "Car", "Sheep", "Duck", "Fish", "Bee", "BasketBall", "RugbyBall", "Rocket", "Truck", "Plane", "Helicopter" };
    static readonly string[] Labels = { "小熊", "恐龙", "木制玩具", "娃娃", "小桶", "水枪", "兔子", "小船", "轮滑鞋", "机器人", "小马", "小巴士", "小汽车", "小羊", "小鸭", "小鱼", "蜜蜂", "篮球", "橄榄球", "火箭", "卡车", "飞机", "直升机" };
    static ToyThemeReplacement() { EditorApplication.update += Poll; }
    static void Poll() {
        const string request = "Temp/toy-theme-command";
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(request)) return;
        var command = File.ReadAllText(request).Trim(); File.Delete(request);
        try {
            if (command == "build") Build();
            else if (command == "validate") ToyThemeValidation.Start();
            else throw new ArgumentException("Unknown toy command: " + command);
            File.WriteAllText("Temp/toy-theme-result", "OK " + command);
        } catch (Exception ex) { File.WriteAllText("Temp/toy-theme-result", "FAIL " + ex); Debug.LogException(ex); }
    }
    public static void Build() {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play before replacing models.");
        var catalog = AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>(ThemeItemsEditorService.CatalogPath);
        var draft = ThemeItemsEditorService.Load(catalog, "CountrysideStove");
        if (draft.rows.Count != Toys.Length || draft.rows.Any(r => r.prefab.name.StartsWith("Toy")))
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
            string sourcePath = "Assets/Resources/CollectableObjects/" + Toys[i] + ".prefab";
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (!source) throw new InvalidOperationException("Missing " + sourcePath);
            var instance = UnityEngine.Object.Instantiate(source);
            string inputPath = Inputs + "/" + Toys[i] + ".prefab";
            try {
                instance.name = Toys[i];
                foreach (var c in instance.GetComponentsInChildren<Collider>(true)) UnityEngine.Object.DestroyImmediate(c);
                foreach (var b in instance.GetComponentsInChildren<Rigidbody>(true)) UnityEngine.Object.DestroyImmediate(b);
                PrefabUtility.SaveAsPrefabAsset(instance, inputPath);
            } finally { UnityEngine.Object.DestroyImmediate(instance); }
            var report = ModelAdapterService.Generate(inputPath, "Toy" + Toys[i], Labels[i], true, true,
                new ModelAdapterResourceOptions { optimizeTextures = false });
            if (report.HasErrors || report.status != "Generated") throw new InvalidOperationException(JsonUtility.ToJson(report, true));
            generated[i] = AssetDatabase.LoadAssetAtPath<GameObject>(report.generatedPrefabPath);
            map.Add(new JObject { ["old"] = oldItems[i].id, ["toy"] = generated[i].name, ["label"] = Labels[i], ["source"] = sourcePath });
        }
        // Retain per-slot authored board size and audio rather than applying name-based toy defaults.
        var data = JsonUtility.FromJson<ItemCatalog.Data>(File.ReadAllText(ModelAdapterService.CatalogPath));
        for (int i = 0; i < Toys.Length; i++) {
            var item = data.items.Single(x => x.id == generated[i].name);
            var old = oldItems[i];
            item.normalizedToSource = old.normalizedToSource;
            item.trayBaseScale = old.trayBaseScale;
            item.sizeTier = old.sizeTier;
            item.sizeReason = "萌趣玩具：沿用 " + old.id + " 槽位大小";
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
        File.WriteAllText(Report + "/mapping.json", map.ToString());
        File.WriteAllText(Report + "/build.txt", "PASS: 23 toy adapters; all theme references valid; non-model JSON parameters unchanged.\nTheme backup: " + backup);
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
    }
}
