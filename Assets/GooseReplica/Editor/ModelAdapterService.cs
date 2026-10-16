using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ModelAdapterService {
    public const string InputRoot = "Assets/GooseReplica/IncomingModels";
    public const string OutputRoot = "Assets/GooseReplica/Generated/ModelAdapter";
    public const string CatalogPath = "Assets/GooseReplica/Resources/Replica/ModelAdapterCatalog.json";

    public sealed class ScanResult {
        public ModelAdapterReport report;
        public GameObject source;
        public Vector3 boundsSize;
        public Vector3 boundsCenter;
        public List<Collider> colliders = new List<Collider>();
    }

    public static string[] InputAssets() {
        if (!AssetDatabase.IsValidFolder(InputRoot)) return Array.Empty<string>();
        var guids = AssetDatabase.FindAssets("t:Prefab", new[] { InputRoot }).Concat(AssetDatabase.FindAssets("t:Model", new[] { InputRoot }));
        return guids.Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => !p.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase) || AssetDatabase.LoadAssetAtPath<GameObject>(p))
            .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(p => p).ToArray();
    }

    public static ScanResult Scan(string inputPath, string requestedId, string displayName, bool generateColliderDraft, bool allowExisting = false) {
        var report = new ModelAdapterReport { requestId = Guid.NewGuid().ToString("N"), inputPath = inputPath, displayName = displayName };
        var result = new ScanResult { report = report };
        var source = AssetDatabase.LoadAssetAtPath<GameObject>(inputPath);
        if (!source) { report.Add(ModelAdapterSeverity.Error, "INPUT_LOAD", "无法加载模型或 Prefab", inputPath); return result; }
        result.source = source;
        var id = ModelAdapterRules.NormalizeId(string.IsNullOrEmpty(requestedId) ? source.name : requestedId);
        report.logicalId = id;
        if (!ModelAdapterRules.IsValidId(id)) report.Add(ModelAdapterSeverity.Error, "ID_INVALID", "逻辑 ID 只能包含字母、数字、下划线和短横线", id);
        if (!inputPath.StartsWith(InputRoot + "/", StringComparison.OrdinalIgnoreCase)) report.Add(ModelAdapterSeverity.Warning, "INPUT_ROOT", "模型不在 IncomingModels 目录内", inputPath);
        if (!allowExisting && CatalogHasId(id)) report.Add(ModelAdapterSeverity.Error, "ID_CONFLICT", "逻辑 ID 已存在，请选择更新模式", id);

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
        if (!instance) instance = UnityEngine.Object.Instantiate(source);
        instance.hideFlags = HideFlags.HideAndDontSave;
        try {
            var renderers = instance.GetComponentsInChildren<Renderer>(true).Where(r => r.enabled && r.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly).ToArray();
            Bounds bounds = default(Bounds);
            bool hasBounds = false;
            foreach (var renderer in renderers) {
                if (renderer is SkinnedMeshRenderer skin) {
                    var baked = new Mesh();
                    skin.BakeMesh(baked);
                    if (baked.vertexCount > 0) { var local = baked.bounds; var world = TransformBounds(skin.transform, local); if (!hasBounds) { bounds = world; hasBounds = true; } else bounds.Encapsulate(world); }
                    UnityEngine.Object.DestroyImmediate(baked);
                } else if (!hasBounds) { bounds = renderer.bounds; hasBounds = true; } else bounds.Encapsulate(renderer.bounds);
            }
            if (!hasBounds || !ModelAdapterRules.IsFinite(bounds.center) || !ModelAdapterRules.IsFinite(bounds.size) || bounds.size == Vector3.zero) {
                report.Add(ModelAdapterSeverity.Error, "BOUNDS_INVALID", "无法计算有效的可见模型包围盒", inputPath);
            } else {
                result.boundsSize = bounds.size; result.boundsCenter = bounds.center;
                report.visualSize = bounds.size; report.visualCenter = bounds.center; report.bottom = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z); report.longestSide = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z)); report.normalizedScale = ModelAdapterRules.SourceLongestSide / report.longestSide;
                if (report.longestSide <= 0 || !ModelAdapterRules.IsFinite(report.normalizedScale)) report.Add(ModelAdapterSeverity.Error, "SCALE_INVALID", "模型最长边或归一化缩放非法", inputPath);
                else report.Add(ModelAdapterSeverity.Pass, "BOUNDS_OK", "包围盒和归一化缩放有效");
            }
            if (!ModelAdapterRules.IsUniform(instance.transform.localScale)) report.Add(ModelAdapterSeverity.Error, "ROOT_SCALE", "根节点必须使用统一缩放", inputPath);
            if (!ModelAdapterRules.IsFinite(instance.transform.localScale)) report.Add(ModelAdapterSeverity.Error, "ROOT_SCALE_FINITE", "根节点缩放包含 NaN/Infinity", inputPath);
            var rootBody = instance.GetComponent<Rigidbody>();
            report.hasRigidbody = rootBody != null;
            if (!rootBody) report.Add(ModelAdapterSeverity.Warning, "RIGIDBODY_MISSING", "生成时会补齐标准根 Rigidbody"); else report.Add(ModelAdapterSeverity.Pass, "RIGIDBODY_OK", "根 Rigidbody 已存在");
            if (!string.Equals(instance.tag, "Collectable", StringComparison.Ordinal)) report.Add(ModelAdapterSeverity.Warning, "TAG_REPAIR", "生成时会设置 Collectable tag");
            var materialMissing = renderers.Any(r => r.sharedMaterials == null || r.sharedMaterials.Any(m => !m || !m.shader));
            if (renderers.Length == 0) report.Add(ModelAdapterSeverity.Error, "RENDERER_MISSING", "没有启用的 Renderer"); else if (materialMissing) report.Add(ModelAdapterSeverity.Error, "MATERIAL_MISSING", "存在空材质或无效 Shader"); else report.Add(ModelAdapterSeverity.Pass, "RENDERER_OK", "Renderer 和材质有效");
            result.colliders.AddRange(instance.GetComponentsInChildren<Collider>(true));
            report.colliderCount = result.colliders.Count;
            bool pick = false, body = false;
            foreach (var collider in result.colliders) {
                if (!collider || !collider.enabled) continue;
                if (collider is MeshCollider) report.Add(ModelAdapterSeverity.Error, "MESH_COLLIDER", "MeshCollider 不进入正式适配，请转换为 Box/Sphere/Capsule", collider.name);
                if (collider.gameObject.layer == ReplicaSourceColliders.PickLayer || HasAncestor(collider.transform, "Pick")) { pick = true; if (collider.isTrigger) report.Add(ModelAdapterSeverity.Error, "PICK_TRIGGER", "拾取 Collider 不能是 Trigger", collider.name); }
                if (collider.gameObject.layer == ReplicaSourceColliders.BodyLayer || HasAncestor(collider.transform, "Body")) body = true;
                if (collider.bounds.size == Vector3.zero || !ModelAdapterRules.IsFinite(collider.bounds.center) || !ModelAdapterRules.IsFinite(collider.bounds.size)) report.Add(ModelAdapterSeverity.Error, "COLLIDER_INVALID", "Collider 尺寸或位置非法", collider.name);
            }
            report.hasPickCollider = pick; report.hasBodyCollider = body;
            if (result.colliders.Count == 0) {
                if (generateColliderDraft) report.Add(ModelAdapterSeverity.Warning, "COLLIDER_DRAFT", "将根据视觉包围盒生成 Pick/Body BoxCollider"); else report.Add(ModelAdapterSeverity.Error, "COLLIDER_REQUIRED", "没有 Collider；勾选生成碰撞草稿后再生成");
            } else { if (!pick) report.Add(ModelAdapterSeverity.Warning, "PICK_GROUP", "没有识别到 Pick 子树或 PickLayer=7"); if (!body) report.Add(ModelAdapterSeverity.Warning, "BODY_GROUP", "没有识别到 Body 子树或 BodyLayer=10"); }
            report.status = report.HasErrors ? "Error" : "Ready";
        } finally { UnityEngine.Object.DestroyImmediate(instance); }
        return result;
    }

    public static ModelAdapterReport Generate(string inputPath, string logicalId, string displayName, bool updateExisting, bool generateColliderDraft) {
        var scan = Scan(inputPath, logicalId, displayName, generateColliderDraft, updateExisting);
        if (!updateExisting && CatalogHasId(scan.report.logicalId)) scan.report.Add(ModelAdapterSeverity.Error, "ID_CONFLICT", "逻辑 ID 已存在，请选择更新模式", scan.report.logicalId);
        if (scan.report.HasErrors) { scan.report.status = "Error"; WriteDiagnostic(scan.report); return scan.report; }
        string request = scan.report.requestId;
        string staging = OutputRoot + "/.staging/" + request;
        string finalDir = OutputRoot + "/" + scan.report.logicalId;
        Directory.CreateDirectory(ToAbsolute(staging));
        try {
            var source = scan.source;
            var wrapper = new GameObject(scan.report.logicalId) { tag = "Collectable" };
            var identity = wrapper.AddComponent<ReplicaSourceIdentity>(); identity.Configure(scan.report.logicalId, displayName, ReplicaSourceCatalog.CurrentVersion);
            var body = wrapper.AddComponent<Rigidbody>(); body.mass = .15f; body.drag = .6f; body.angularDrag = .8f; body.interpolation = RigidbodyInterpolation.Interpolate; body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; body.isKinematic = false; body.detectCollisions = true;
            var visual = new GameObject("Visual"); visual.transform.SetParent(wrapper.transform, false);
            var geometry = new GameObject("Geometry"); geometry.transform.SetParent(visual.transform, false);
            var sourceInstance = (GameObject)PrefabUtility.InstantiatePrefab(source);
            if (!sourceInstance) sourceInstance = UnityEngine.Object.Instantiate(source);
            sourceInstance.name = source.name; sourceInstance.transform.SetParent(geometry.transform, false);
            foreach (var rb in sourceInstance.GetComponentsInChildren<Rigidbody>(true)) UnityEngine.Object.DestroyImmediate(rb);
            float scale = scan.report.normalizedScale;
            geometry.transform.localScale = Vector3.one * scale;
            geometry.transform.localPosition = -scan.report.visualCenter * scale;
            var generatedColliders = new List<Collider>(sourceInstance.GetComponentsInChildren<Collider>(true));
            bool generatedPick = generatedColliders.Any(IsPick);
            bool generatedBody = generatedColliders.Any(c => c && !IsPick(c.transform));
            if (generateColliderDraft && (!generatedPick || !generatedBody)) {
                if (!generatedPick) { var pick = new GameObject("Pick"); pick.layer = ReplicaSourceColliders.PickLayer; pick.transform.SetParent(visual.transform, false); var pc = pick.AddComponent<BoxCollider>(); pc.size = scan.report.visualSize * scale; pc.isTrigger = false; generatedColliders.Add(pc); }
                if (!generatedBody) { var real = new GameObject("Body"); real.layer = ReplicaSourceColliders.BodyLayer; real.transform.SetParent(visual.transform, false); var rc = real.AddComponent<BoxCollider>(); rc.size = scan.report.visualSize * scale; generatedColliders.Add(rc); }
            }
            foreach (var collider in generatedColliders) {
                if (collider is MeshCollider) throw new InvalidOperationException("MeshCollider is not supported: " + collider.name);
                collider.gameObject.layer = IsPick(collider.transform) ? ReplicaSourceColliders.PickLayer : ReplicaSourceColliders.BodyLayer;
            }
            var colliderProfile = BuildProfile(generatedColliders, visual.transform, scan.report.logicalId);
            if (colliderProfile.col == null || colliderProfile.realCol == null) throw new InvalidOperationException("Generated model needs both Pick and Body collider groups.");
            string prefabPath = staging + "/" + scan.report.logicalId + ".prefab";
            Directory.CreateDirectory(ToAbsolute(staging));
            PrefabUtility.SaveAsPrefabAsset(wrapper, prefabPath);
            UnityEngine.Object.DestroyImmediate(wrapper);
            scan.report.generatedPrefabPath = prefabPath;
            scan.report.previewPath = staging + "/index.html";
            WriteReport(scan.report, staging + "/report.json");
            ReplaceStaging(staging, finalDir);
            scan.report.generatedPrefabPath = finalDir + "/" + scan.report.logicalId + ".prefab";
            scan.report.status = "Generated";
            UpsertCatalog(scan.report, colliderProfile, updateExisting);
            scan.report.previewPath = finalDir + "/index.html";
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ModelAdapterPreview.Generate(scan.report, finalDir);
            WriteReport(scan.report, finalDir + "/report.json");
            return scan.report;
        } catch (Exception ex) { scan.report.Add(ModelAdapterSeverity.Error, "GENERATE_FAILED", ex.Message); WriteDiagnostic(scan.report); if (Directory.Exists(ToAbsolute(staging))) Directory.Delete(ToAbsolute(staging), true); return scan.report; }
    }

    static ReplicaSourceCatalog.ColliderProfile BuildProfile(List<Collider> colliders, Transform visual, string id) {
        var profile = new ReplicaSourceCatalog.ColliderProfile { name = id, nodes = new List<ReplicaSourceCatalog.ColliderNode>().ToArray() };
        var grouped = colliders.Where(c => c).GroupBy(c => c.transform).ToArray();
        var nodes = new List<ReplicaSourceCatalog.ColliderNode>();
        foreach (var group in grouped) {
            var node = new ReplicaSourceCatalog.ColliderNode { indices = PathIndices(visual, group.Key), layer = group.Key.gameObject.layer, colliders = group.Select(ToShape).ToArray() };
            nodes.Add(node);
            if (node.layer == ReplicaSourceColliders.PickLayer && profile.col == null) profile.col = node.indices;
            if (node.layer == ReplicaSourceColliders.BodyLayer && profile.realCol == null) profile.realCol = node.indices;
        }
        profile.nodes = nodes.ToArray(); return profile;
    }
    static ReplicaSourceCatalog.ColliderShape ToShape(Collider collider) {
        var shape = new ReplicaSourceCatalog.ColliderShape { type = collider.GetType().Name, enabled = collider.enabled, trigger = collider.isTrigger, hasMaterial = collider.sharedMaterial != null };
        if (collider is BoxCollider box) { shape.center = box.center; shape.size = box.size; }
        else if (collider is SphereCollider sphere) { shape.center = sphere.center; shape.radius = sphere.radius; }
        else if (collider is CapsuleCollider capsule) { shape.center = capsule.center; shape.radius = capsule.radius; shape.height = capsule.height; shape.direction = capsule.direction; }
        if (collider.sharedMaterial) { var m = collider.sharedMaterial; shape.material = new ReplicaSourceCatalog.Surface { m_Name = m.name, dynamicFriction = m.dynamicFriction, staticFriction = m.staticFriction, bounciness = m.bounciness, frictionCombine = (int)m.frictionCombine, bounceCombine = (int)m.bounceCombine }; }
        return shape;
    }
    static int[] PathIndices(Transform root, Transform target) { var list = new List<int>(); var current = target; while (current && current != root) { list.Add(current.GetSiblingIndex()); current = current.parent; } list.Reverse(); return list.ToArray(); }
    static bool IsPick(Transform t) => t.gameObject.layer == ReplicaSourceColliders.PickLayer || HasAncestor(t, "Pick");
    static bool HasAncestor(Transform t, string name) { while (t) { if (string.Equals(t.name, name, StringComparison.OrdinalIgnoreCase)) return true; t = t.parent; } return false; }
    static Bounds TransformBounds(Transform t, Bounds local) { var points = new[] { new Vector3(local.min.x, local.min.y, local.min.z), new Vector3(local.min.x, local.min.y, local.max.z), new Vector3(local.min.x, local.max.y, local.min.z), new Vector3(local.min.x, local.max.y, local.max.z), new Vector3(local.max.x, local.min.y, local.min.z), new Vector3(local.max.x, local.min.y, local.max.z), new Vector3(local.max.x, local.max.y, local.min.z), new Vector3(local.max.x, local.max.y, local.max.z) }; var output = new Bounds(t.TransformPoint(points[0]), Vector3.zero); for (int i = 1; i < points.Length; i++) output.Encapsulate(t.TransformPoint(points[i])); return output; }
    static bool CatalogHasId(string id) { var text = AssetDatabase.LoadAssetAtPath<TextAsset>(CatalogPath); if (!text) return false; var data = JsonUtility.FromJson<ReplicaSourceCatalog.Data>(text.text); return (data?.items ?? Array.Empty<ReplicaSourceCatalog.Item>()).Any(x => x != null && string.Equals(x.id, id, StringComparison.OrdinalIgnoreCase)); }
    static void UpsertCatalog(ModelAdapterReport report, ReplicaSourceCatalog.ColliderProfile profile, bool update) { var data = new ReplicaSourceCatalog.Data { version = ReplicaSourceCatalog.CurrentVersion, items = Array.Empty<ReplicaSourceCatalog.Item>() }; var existing = AssetDatabase.LoadAssetAtPath<TextAsset>(CatalogPath); if (existing) data = JsonUtility.FromJson<ReplicaSourceCatalog.Data>(existing.text) ?? data; var list = (data.items ?? Array.Empty<ReplicaSourceCatalog.Item>()).ToList(); var index = list.FindIndex(x => x != null && string.Equals(x.id, report.logicalId, StringComparison.OrdinalIgnoreCase)); var item = new ReplicaSourceCatalog.Item { id = report.logicalId, displayName = report.displayName, prefabPath = report.generatedPrefabPath, normalizedToSource = report.normalizedScale, colliderProfile = profile, audioProfile = new ReplicaSourceCatalog.AudioProfile { name = report.logicalId } }; if (index >= 0) { if (!update) throw new InvalidOperationException("Existing catalog entry requires Update mode."); list[index] = item; } else list.Add(item); data.items = list.ToArray(); Directory.CreateDirectory(ToAbsolute(Path.GetDirectoryName(CatalogPath))); File.WriteAllText(ToAbsolute(CatalogPath), JsonUtility.ToJson(data, true)); AssetDatabase.ImportAsset(CatalogPath); }
    static void ReplaceStaging(string staging, string finalDir) { var stagingAbsolute = ToAbsolute(staging); var finalAbsolute = ToAbsolute(finalDir); if (Directory.Exists(finalAbsolute)) Directory.Delete(finalAbsolute, true); Directory.Move(stagingAbsolute, finalAbsolute); }
    static void WriteReport(ModelAdapterReport report, string path) { Directory.CreateDirectory(Path.GetDirectoryName(ToAbsolute(path))); File.WriteAllText(ToAbsolute(path), JsonUtility.ToJson(report, true)); AssetDatabase.ImportAsset(path); }
    static void WriteDiagnostic(ModelAdapterReport report) { string path = OutputRoot + "/_diagnostics/" + report.requestId + ".json"; Directory.CreateDirectory(ToAbsolute(Path.GetDirectoryName(path))); File.WriteAllText(ToAbsolute(path), JsonUtility.ToJson(report, true)); AssetDatabase.Refresh(); }
    static string ToAbsolute(string projectPath) => Path.GetFullPath(Path.Combine(Directory.GetParent(Application.dataPath).FullName, projectPath));
}
