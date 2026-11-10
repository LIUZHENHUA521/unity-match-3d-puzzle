using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ItemBodyColliderBake {
    public const string Output = "ValidationOutput/compound-colliders";
    [Serializable] public sealed class Evidence {
        public string id;
        public int triangles;
        public float oldBoxVolume, fittedVolume;
        public ItemBodyFitter.Shape[] shapes;
        public Vector3[] meshTriangles;
    }
    // Explicit migration only: ordinary reimport never overwrites authored bodies.
    [MenuItem("Tools/Match 3D/Rebuild Adapter Body Colliders")]
    public static void BakeAll() {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play before rebuilding body assets.");
        Directory.CreateDirectory(Output);
        var data = JsonUtility.FromJson<ItemCatalog.Data>(File.ReadAllText(ModelAdapterService.CatalogPath));
        var paths = data.items.SelectMany(item => new[] { item.prefabPath, Path.GetDirectoryName(item.prefabPath) + "/report.json" })
            .Concat(new[] { ModelAdapterService.CatalogPath }).Where(File.Exists).Distinct().ToArray();
        var originals = paths.ToDictionary(path => path, File.ReadAllBytes);
        try { Bake(data); }
        catch {
            // A later bad mesh must not leave earlier prefabs paired with the old
            // catalog. Restore the complete asset set if any part of baking fails.
            foreach (var original in originals) File.WriteAllBytes(original.Key, original.Value);
            AssetDatabase.Refresh(); ItemColliderGeometry.ClearCache(); throw;
        }
    }
    static void Bake(ItemCatalog.Data data) {
        foreach (var item in data.items) {
            var obj = PrefabUtility.LoadPrefabContents(item.prefabPath);
            try {
                var visual = obj.transform.GetChild(0); var body = visual.Find("Body"); var geometry = visual.Find("Geometry");
                if (!body || !geometry) throw new InvalidOperationException("Missing adapter hierarchy " + item.id);
                var triangles = ItemBodyFitter.ReadTriangles(geometry, body);
                var shapes = ItemBodyFitter.Fit(triangles);
                var vertices = triangles.SelectMany(t => new[] { t.a, t.b, t.c }).ToArray();
                var bounds = new Bounds(vertices[0], Vector3.zero); foreach (var p in vertices) bounds.Encapsulate(p);
                var old = body.GetComponentsInChildren<Collider>(true);
                var material = old.FirstOrDefault()?.sharedMaterial;
                float contact = old.FirstOrDefault()?.contactOffset ?? .01f;
                foreach (var c in old) UnityEngine.Object.DestroyImmediate(c);
                // Only this generated adapter Body subtree is owned by this operation.
                for (int i = body.childCount - 1; i >= 0; i--) UnityEngine.Object.DestroyImmediate(body.GetChild(i).gameObject);
                for (int i = 0; i < shapes.Length; i++) { var c = shapes[i].AddTo(body, i); c.sharedMaterial = material; c.contactOffset = contact; }
                var cached = obj.GetComponent<ItemColliderGeometry>(); if (cached) UnityEngine.Object.DestroyImmediate(cached);
                var profile = ModelAdapterService.BuildProfile(obj.GetComponentsInChildren<Collider>(true).ToList(), visual, item.id);
                item.colliderProfile = profile;
                PrefabUtility.SaveAsPrefabAsset(obj, item.prefabPath, out bool saved);
                if (!saved) throw new InvalidOperationException("Failed to save body geometry: " + item.prefabPath);
                string reportPath = Path.GetDirectoryName(item.prefabPath) + "/report.json";
                if (File.Exists(reportPath)) {
                    var report = JsonUtility.FromJson<ModelAdapterReport>(File.ReadAllText(reportPath));
                    report.colliderCount = obj.GetComponentsInChildren<Collider>(true).Length;
                    report.hasPickCollider = report.hasBodyCollider = true;
                    report.issues.RemoveAll(i => i.code == "COLLIDER_DRAFT" || i.code == "BODY_FIT");
                    report.Add(ModelAdapterSeverity.Pass, "BODY_FIT", "已生成 " + shapes.Length + " 个贴形实体碰撞体；拾取独立");
                    File.WriteAllText(reportPath, JsonUtility.ToJson(report, true));
                }
                var evidence = new Evidence { id = item.id, triangles = triangles.Count, oldBoxVolume = bounds.size.x * bounds.size.y * bounds.size.z,
                    fittedVolume = shapes.Sum(s => s.volume), shapes = shapes, meshTriangles = vertices };
                File.WriteAllText(Output + "/" + item.id + ".json", JsonUtility.ToJson(evidence));
                Debug.Log("BODY FIT " + item.id + ": " + shapes.Length + " shapes, volume ratio " + (evidence.fittedVolume / evidence.oldBoxVolume).ToString("F3"));
            } finally { PrefabUtility.UnloadPrefabContents(obj); }
        }
        File.WriteAllText(ModelAdapterService.CatalogPath, JsonUtility.ToJson(data, true));
        AssetDatabase.ImportAsset(ModelAdapterService.CatalogPath);
        ItemColliderGeometry.ClearCache(); AssetDatabase.SaveAssets();
    }
}
