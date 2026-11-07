using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ThemeItemsEditorValidation {
    const string Root = "Assets/ThemeItemsEditorValidationFixture";
    static int checks;
    static void Check(bool ok, string label) {
        if (!ok) throw new InvalidOperationException("FAIL: " + label);
        checks++;
    }
    static void Reject(Action action, string label) {
        bool rejected = false;
        try { action(); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected, label);
    }
    public static void Run() {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("请先停止运行。");
        checks = 0;
        if (AssetDatabase.IsValidFolder(Root)) throw new InvalidOperationException("测试目录已存在，拒绝覆盖：" + Root);
        string originalCatalog = File.ReadAllText(ThemeItemsEditorService.CatalogPath);
        string originalSource = File.ReadAllText(ThemeItemsEditorService.SourcePath);
        Directory.CreateDirectory("Logs");
        try {
            AssetDatabase.CreateFolder("Assets", "ThemeItemsEditorValidationFixture");
            AssetDatabase.CopyAsset(ThemeItemsEditorService.CatalogPath, Root + "/Catalog.asset");
            var catalog = AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>(Root + "/Catalog.asset");
            string sourcePath = Root + "/Source.json";
            var json = JObject.Parse(originalSource);
            json["auditUnknown"] = "keep me";
            json["themes"][0]["auditThemeUnknown"] = 42;
            json["themes"][0]["layers"][0]["auditLayerUnknown"] = true;
            // A separate unrelated theme must survive edits and remain importable.
            var other = json["themes"][0].DeepClone(); other["environment"] = "UnrelatedFixture";
            ((JArray)json["themes"]).Add(other);
            File.WriteAllText(sourcePath, json.ToString()); AssetDatabase.ImportAsset(sourcePath);
            string env = catalog.profiles[0].environment;
            var draft = ThemeItemsEditorService.Load(catalog, env, sourcePath);
            Check(ThemeItemsEditorService.Validate(draft).Count == 0, "current real pool validates");
            Check(draft.rows.Count == catalog.profiles[0].prefabs.Length, "all current models loaded");
            Reject(() => ThemeItemsEditorService.Add(draft, draft.rows[0].prefab), "duplicate add rejected");
            Reject(() => ThemeItemsEditorService.Replace(draft, 0, draft.rows[1].prefab), "duplicate replacement rejected");
            var plain = new GameObject("NotAdapted");
            try { Reject(() => ThemeItemsEditorService.Add(draft, plain), "scene object rejected"); }
            finally { Object.DestroyImmediate(plain); }
            var raw = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TinyTreatsPicnic/fbx(unity)/sandwich.fbx");
            if (raw) Reject(() => ThemeItemsEditorService.Add(draft, raw), "raw FBX rejected");
            draft.rows[0].weight = 0;
            Reject(() => ThemeItemsEditorService.Save(draft), "zero weight blocks save");
            draft.rows[0].weight = float.NaN;
            Check(ThemeItemsEditorService.Validate(draft).Any(e => e.Contains("权重")), "NaN rejected");
            draft.rows[0].weight = 1;
            int target = draft.rows.FindIndex(r => r.role == ThemeItemsEditorService.FirstStageRole.Target);
            var targetRow = draft.rows[target];
            draft.rows.RemoveAt(target);
            Check(ThemeItemsEditorService.Validate(draft).Count == 0, "removing target does not block editing");
            var automatic = ThemeItemsEditorService.TutorialRows(draft);
            Check(automatic.Length >= 3 && automatic.Distinct().Count() == automatic.Length && automatic.All(draft.rows.Contains), "removed target automatically replaced from remaining pool");
            var minimal = new ThemeItemsEditorService.Draft { rows = draft.rows.Take(3).Select(r => new ThemeItemsEditorService.Row { prefab = r.prefab }).ToList() };
            Check(ThemeItemsEditorService.TutorialRows(minimal).Length == 3, "all roles absent automatically assigned");
            minimal.rows.RemoveAt(0);
            Check(ThemeItemsEditorService.TutorialRows(minimal).Length == 0, "undersized pool cannot produce invalid tutorial");
            draft.rows.Insert(target, targetRow);
            int replacementIndex = draft.rows.FindIndex(r => r.role == ThemeItemsEditorService.FirstStageRole.None);
            var replacement = draft.rows[replacementIndex].prefab;
            var removed = draft.rows[target].prefab;
            draft.rows.RemoveAt(replacementIndex);
            target = draft.rows.IndexOf(targetRow);
            draft.rows[target].weight = 2.5f;
            ThemeItemsEditorService.Replace(draft, target, replacement);
            Check(draft.rows[target].role == ThemeItemsEditorService.FirstStageRole.Target && draft.rows[target].weight == 2.5f, "replacement preserves role and weight");
            ThemeItemsEditorService.Add(draft, removed);
            Check(draft.rows.Last().weight == 1 && draft.rows.Last().role == ThemeItemsEditorService.FirstStageRole.None, "new model default weight and role");
            while (draft.rows.Count > 5) {
                int remove = draft.rows.FindLastIndex(r => r.role == ThemeItemsEditorService.FirstStageRole.None);
                draft.rows.RemoveAt(remove);
            }
            Check(ThemeItemsEditorService.BuildLayers(draft).All(l => l.types <= 5), "removal clamps layer types");
            var counts = catalog.profiles[0].layers.Select(l => l.count).ToArray();
            var ordinaryBefore = catalog.profiles[0].tutorialNames.Skip(1).ToArray();
            string backup = ThemeItemsEditorService.Save(draft);
            Check(catalog.profiles[0].tutorialNames.Skip(1).SequenceEqual(ordinaryBefore), "ordinary candidate order unchanged");
            Check(File.Exists(Path.Combine(backup, "SourceLevelCatalog.asset")), "catalog backup exists");
            Check(File.Exists(Path.Combine(backup, "SourceLevelConfig.json")), "source backup exists");
            Check(catalog.profiles[0].tutorialNames[0] == replacement.name, "tutorial reference follows replacement");
            Check(catalog.profiles[0].layers.Select(l => l.count).SequenceEqual(counts), "layer quantities unchanged");
            var saved = JObject.Parse(File.ReadAllText(sourcePath));
            Check((string)saved["auditUnknown"] == "keep me" && (int)saved["themes"][0]["auditThemeUnknown"] == 42 && (bool)saved["themes"][0]["layers"][0]["auditLayerUnknown"], "unknown fields survive save");
            Check(JToken.DeepEquals(saved["themes"][1], other), "unrelated theme preserved");
            var imported = BoardLevelPlanImporter.ReadProfiles(sourcePath)[0];
            var live = catalog.profiles[0];
            Check(imported.prefabs.SequenceEqual(live.prefabs), "actual importer reproduces pool order");
            Check(imported.tutorialNames.SequenceEqual(live.tutorialNames), "actual importer reproduces tutorial roles");
            Check(imported.layers.Zip(live.layers, (a,b) => a.types == b.types && a.count == b.count && a.height == b.height && a.weights.SequenceEqual(b.weights)).All(x => x), "actual importer reproduces all layers and weights");
            Check(imported.seconds == live.seconds && imported.limitHeight == live.limitHeight && imported.sourceIndex == live.sourceIndex, "actual importer preserves gameplay parameters");
            draft = ThemeItemsEditorService.Load(catalog, env, sourcePath);
            File.AppendAllText(sourcePath, "\n");
            Reject(() => ThemeItemsEditorService.Save(draft), "external source edit blocks save");
            draft = ThemeItemsEditorService.Load(catalog, env, sourcePath);
            catalog.profiles[0].seconds++;
            Reject(() => ThemeItemsEditorService.Save(draft), "external memory edit blocks save");
            catalog.profiles[0].seconds--;
            draft = ThemeItemsEditorService.Load(catalog, env, sourcePath);
            File.AppendAllText(Root + "/Catalog.asset", "\n");
            Reject(() => ThemeItemsEditorService.Save(draft), "external asset file edit blocks save");
            catalog.profiles[0].layers[1].weights[0] += 1;
            Reject(() => ThemeItemsEditorService.Load(catalog, env, sourcePath), "per-layer differing weights cannot silently flatten");
            Check(File.ReadAllText(ThemeItemsEditorService.CatalogPath) == originalCatalog && File.ReadAllText(ThemeItemsEditorService.SourcePath) == originalSource, "production config untouched");
            string result = "PASS theme items editor: " + checks + " checks; production config unchanged.";
            File.WriteAllText("Logs/theme-items-editor-validation.txt", result + "\n"); Debug.Log(result);
        } catch (Exception ex) {
            File.WriteAllText("Logs/theme-items-editor-validation.txt", ex.ToString()); Debug.LogException(ex); throw;
        } finally { AssetDatabase.DeleteAsset(Root); }
    }
}
