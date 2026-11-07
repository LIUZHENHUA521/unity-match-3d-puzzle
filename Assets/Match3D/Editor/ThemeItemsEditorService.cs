using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

// Editor-only drafts keep the live catalog untouched until both persisted files can be saved.
public static class ThemeItemsEditorService {
    public const string CatalogPath = "Assets/Match3D/Resources/Match3D/SourceLevelCatalog.asset";
    public const string SourcePath = "Assets/Match3D/Resources/Match3D/SourceLevelConfig.json";
    public enum FirstStageRole { None, Target, Ordinary }
    [Serializable] public sealed class Row {
        public GameObject prefab;
        public float weight = 1;
        public FirstStageRole role;
        public int firstStageOrder = -1;
    }
    [Serializable] public sealed class Draft {
        public LevelLayoutConfig catalog;
        public string sourcePath, environment, sourceSnapshot, catalogSnapshot, catalogFileSnapshot;
        public List<Row> rows = new List<Row>();
        public ChallengeLayer[] layers;
    }

    public static Draft Load(LevelLayoutConfig catalog, string environment, string sourcePath = SourcePath) {
        if (!catalog) throw new InvalidOperationException("缺少实际关卡配置。");
        var profile = FindProfile(catalog, environment);
        var source = File.ReadAllText(sourcePath);
        FindTheme(JObject.Parse(source), environment);
        var weights = SharedWeights(profile);
        var draft = new Draft {
            catalog = catalog, environment = environment, sourcePath = sourcePath, sourceSnapshot = source,
            catalogSnapshot = EditorJsonUtility.ToJson(catalog),
            catalogFileSnapshot = File.ReadAllText(AssetDatabase.GetAssetPath(catalog)),
            layers = profile.layers.Select(CloneLayer).ToArray()
        };
        foreach (var pair in profile.prefabs.Select((p, i) => new { p, i })) {
            int first = Array.IndexOf(profile.tutorialNames ?? Array.Empty<string>(), pair.p ? pair.p.name : "");
            draft.rows.Add(new Row { prefab = pair.p, weight = weights[pair.i], firstStageOrder = first,
                role = first == 0 ? FirstStageRole.Target : first > 0 ? FirstStageRole.Ordinary : FirstStageRole.None });
        }
        return draft;
    }

    static float[] SharedWeights(LevelLayoutConfig.Profile profile) {
        if (profile.prefabs == null || profile.layers == null || profile.layers.Length == 0)
            throw new InvalidOperationException("物品池或层配置缺失。");
        float[] shared = null;
        foreach (var layer in profile.layers) {
            if (layer == null) throw new InvalidOperationException("层配置缺失。");
            var weights = layer.weights == null || layer.weights.Length == 0
                ? Enumerable.Repeat(1f, profile.prefabs.Length).ToArray() : layer.weights;
            if (weights.Length != profile.prefabs.Length) throw new InvalidOperationException("权重数量与物品池不一致，请先修复原配置。");
            if (shared != null && !shared.SequenceEqual(weights))
                throw new InvalidOperationException("此主题各层使用不同权重，当前窗口只支持全层共享权重，已阻止编辑以保护原配置。");
            shared = weights;
        }
        var tutorial = profile.tutorialNames ?? Array.Empty<string>();
        if (tutorial.Distinct().Count() != tutorial.Length || tutorial.Any(n => !profile.prefabs.Any(p => p && p.name == n)))
            throw new InvalidOperationException("原配置的第一关引用重复或缺失，请先修复。");
        return shared;
    }

    public static void Add(Draft draft, GameObject prefab) {
        string error = CheckPrefab(prefab);
        if (error != null) throw new InvalidOperationException(error);
        if (draft.rows.Any(r => r.prefab && string.Equals(r.prefab.name, prefab.name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("主题中已存在：" + prefab.name);
        draft.rows.Add(new Row { prefab = prefab });
    }
    public static void Replace(Draft draft, int index, GameObject prefab) {
        string error = CheckPrefab(prefab);
        if (error != null) throw new InvalidOperationException(error);
        if (draft.rows.Where((r, i) => i != index).Any(r => r.prefab && string.Equals(r.prefab.name, prefab.name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("主题中已存在同名物品：" + prefab.name);
        draft.rows[index].prefab = prefab; // Role and weight deliberately travel with the replacement.
    }
    // Preserve existing internal choices where possible, filling gaps from pool order.
    // This is derived from the draft, so removing an item never leaves a dangling reference.
    public static Row[] TutorialRows(Draft draft) {
        var rows = draft.rows.Where(r => r.prefab).ToList();
        if (rows.Count < 3) return Array.Empty<Row>();
        var target = rows.FirstOrDefault(r => r.role == FirstStageRole.Target)
            ?? rows.FirstOrDefault(r => r.role == FirstStageRole.None) ?? rows[0];
        var ordinary = rows.Where(r => r != target && r.role == FirstStageRole.Ordinary)
            .OrderBy(r => r.firstStageOrder).ToList();
        foreach (var row in rows.Where(r => r != target && !ordinary.Contains(r))) {
            if (ordinary.Count >= 2) break;
            ordinary.Add(row);
        }
        return new[] { target }.Concat(ordinary).ToArray();
    }
    public static ChallengeLayer[] BuildLayers(Draft draft) => draft.layers.Select(layer => {
        var copy = CloneLayer(layer);
        copy.types = Math.Min(layer.types, draft.rows.Count);
        copy.weights = draft.rows.Select(r => r.weight).ToArray();
        return copy;
    }).ToArray();
    static ChallengeLayer CloneLayer(ChallengeLayer layer) => new ChallengeLayer(layer.count, layer.types, layer.height) {
        weights = layer.weights == null ? null : (float[])layer.weights.Clone()
    };

    public static List<string> Validate(Draft draft) {
        var errors = new List<string>();
        if (draft == null || !draft.catalog) { errors.Add("关卡配置不存在。"); return errors; }
        ItemCatalog.ClearCache();
        if (draft.rows.Count < 3) errors.Add("主题至少保留 3 种物品。");
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in draft.rows) {
            string error = CheckPrefab(row.prefab);
            if (error != null) errors.Add(error);
            if (row.prefab && !names.Add(row.prefab.name)) errors.Add("重复名称：" + row.prefab.name);
            if (!ModelAdapterRules.IsFinite(row.weight) || row.weight <= 0) errors.Add((row.prefab ? row.prefab.name : "空物品") + "：权重必须是大于 0 的有限数值。");
        }
        if (draft.layers == null || draft.layers.Length == 0) errors.Add("主题没有层配置。");
        else foreach (var layer in draft.layers) {
            if (layer == null || layer.count <= 0 || layer.count % 3 != 0 || layer.types < 1 ||
                layer.count / 3 < Math.Min(layer.types, draft.rows.Count) || !ModelAdapterRules.IsFinite(layer.height))
                errors.Add("层配置必须满足三消数量预算、有效类型数及有限高度。");
        }
        return errors.Distinct().ToList();
    }

    public static string CheckPrefab(GameObject prefab) {
        if (!prefab) return "物品引用为空。";
        string path = AssetDatabase.GetAssetPath(prefab);
        if (!path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) || AssetDatabase.LoadAssetAtPath<GameObject>(path) != prefab)
            return prefab.name + "：请使用适配后的 Prefab 根对象，不能直接使用 FBX 或场景对象。";
        if (!prefab.activeSelf || !prefab.CompareTag("Collectable") || !prefab.GetComponent<Rigidbody>() ||
            !prefab.GetComponentsInChildren<Renderer>().Any(r => r.enabled))
            return prefab.name + "：缺少可见模型、Collectable 标记或 Rigidbody，请先运行模型适配。";
        var identity = prefab.GetComponent<BoardItemIdentity>();
        JObject collision;
        if (identity) {
            if (identity.LogicalId != prefab.name || !ItemCatalog.TryGet(identity.LogicalId, out var item) ||
                item.prefabPath != path || item.colliderProfile == null || item.audioProfile == null ||
                !ModelAdapterRules.IsFinite(item.normalizedToSource) || item.normalizedToSource <= 0)
                return prefab.name + "：模型名称、ID 或适配目录登记不匹配，请重新生成适配 Prefab。";
            collision = JObject.Parse(JsonUtility.ToJson(item.colliderProfile));
        } else {
            var text = Resources.Load<TextAsset>("Match3D/SourceColliders");
            collision = text ? (JObject)JObject.Parse(text.text)["items"].FirstOrDefault(t => (string)t["name"] == prefab.name) : null;
        }
        if (collision == null || prefab.transform.childCount == 0) return prefab.name + "：没有可用的运行时碰撞配置。";
        Transform visual = prefab.transform.GetChild(0);
        if (!Resolve(visual, collision["col"]) || !Resolve(visual, collision["realCol"]))
            return prefab.name + "：拾取/实体碰撞路径无效。";
        var nodes = collision["nodes"] as JArray;
        if (nodes == null || nodes.Count == 0 || nodes.Any(n => !Resolve(visual, n["indices"])))
            return prefab.name + "：碰撞节点与模型层级不匹配。";
        return null;
    }
    static Transform Resolve(Transform root, JToken indices) {
        if (!(indices is JArray values)) return null;
        foreach (var value in values) {
            int index = (int)value;
            if (index < 0 || index >= root.childCount) return null;
            root = root.GetChild(index);
        }
        return root;
    }

    public static string Save(Draft draft) {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            throw new InvalidOperationException("请退出运行模式并等待编译完成后保存。");
        var errors = Validate(draft);
        if (errors.Count > 0) throw new InvalidOperationException(string.Join("\n", errors));
        string catalogPath = AssetDatabase.GetAssetPath(draft.catalog);
        if (File.ReadAllText(draft.sourcePath) != draft.sourceSnapshot ||
            EditorJsonUtility.ToJson(draft.catalog) != draft.catalogSnapshot ||
            File.ReadAllText(catalogPath) != draft.catalogFileSnapshot)
            throw new InvalidOperationException("配置已在窗口外修改。请放弃草稿并重新加载，避免覆盖他人的修改。");
        var profile = FindProfile(draft.catalog, draft.environment);
        var json = JObject.Parse(draft.sourceSnapshot);
        var theme = FindTheme(json, draft.environment);
        var layers = BuildLayers(draft);
        var tutorial = TutorialRows(draft).Select(r => r.prefab.name).ToArray();
        theme["names"] = new JArray(draft.rows.Select(r => r.prefab.name));
        theme["prefabPaths"] = new JArray(draft.rows.Select(r => AssetDatabase.GetAssetPath(r.prefab)));
        theme["weights"] = new JArray(draft.rows.Select(r => r.weight));
        theme["tutorialNames"] = new JArray(tutorial);
        // The live catalog is authoritative for unchanged gameplay parameters too.
        theme["sourceIndex"] = profile.sourceIndex;
        theme["seconds"] = profile.seconds;
        theme["limitHeight"] = profile.limitHeight;
        var previousLayers = theme["layers"] as JArray;
        var jsonLayers = new JArray();
        for (int i = 0; i < layers.Length; i++) {
            var layer = previousLayers != null && i < previousLayers.Count ? (JObject)previousLayers[i].DeepClone() : new JObject();
            layer["count"] = layers[i].count; layer["types"] = layers[i].types; layer["height"] = layers[i].height;
            jsonLayers.Add(layer);
        }
        theme["layers"] = jsonLayers;
        string backup = Path.GetFullPath("work/theme-items-backups/" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + "-" + Guid.NewGuid().ToString("N").Substring(0, 6));
        Directory.CreateDirectory(backup);
        File.WriteAllText(Path.Combine(backup, "SourceLevelConfig.json"), draft.sourceSnapshot);
        File.WriteAllText(Path.Combine(backup, "SourceLevelCatalog.asset"), draft.catalogFileSnapshot);
        File.WriteAllText(Path.Combine(backup, "catalog-memory.json"), draft.catalogSnapshot);
        File.WriteAllText(Path.Combine(backup, "paths.txt"), draft.sourcePath + "\n" + catalogPath);
        try {
            File.WriteAllText(draft.sourcePath, json.ToString(Formatting.Indented) + "\n");
            profile.prefabs = draft.rows.Select(r => r.prefab).ToArray();
            profile.tutorialNames = tutorial;
            profile.layers = layers;
            EditorUtility.SetDirty(draft.catalog);
            AssetDatabase.SaveAssetIfDirty(draft.catalog);
            AssetDatabase.ImportAsset(draft.sourcePath);
        } catch {
            File.WriteAllText(draft.sourcePath, draft.sourceSnapshot);
            File.WriteAllText(catalogPath, draft.catalogFileSnapshot);
            EditorJsonUtility.FromJsonOverwrite(draft.catalogSnapshot, draft.catalog);
            AssetDatabase.ImportAsset(draft.sourcePath);
            throw;
        }
        draft.sourceSnapshot = File.ReadAllText(draft.sourcePath);
        draft.catalogSnapshot = EditorJsonUtility.ToJson(draft.catalog);
        draft.catalogFileSnapshot = File.ReadAllText(catalogPath);
        draft.layers = layers.Select(CloneLayer).ToArray();
        foreach (var row in draft.rows) {
            int index = Array.IndexOf(tutorial, row.prefab.name);
            row.firstStageOrder = index;
            row.role = index == 0 ? FirstStageRole.Target : index > 0 ? FirstStageRole.Ordinary : FirstStageRole.None;
        }
        return backup;
    }
    static LevelLayoutConfig.Profile FindProfile(LevelLayoutConfig catalog, string environment) {
        var matches = (catalog.profiles ?? Array.Empty<LevelLayoutConfig.Profile>()).Where(p => p != null && p.environment == environment).ToArray();
        if (matches.Length != 1) throw new InvalidOperationException("主题实际关卡配置缺失或重复：" + environment);
        return matches[0];
    }
    static JObject FindTheme(JObject root, string environment) {
        var matches = (root["themes"] as JArray ?? new JArray()).OfType<JObject>().Where(t => (string)t["environment"] == environment).ToArray();
        if (matches.Length != 1) throw new InvalidOperationException("源配置中的主题缺失或重复：" + environment);
        return matches[0];
    }
}
