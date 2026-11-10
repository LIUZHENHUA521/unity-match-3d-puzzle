using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ItemBodyColliderValidation {
    static readonly List<string> results = new List<string>();
    static void Check(bool ok, string message) { if (!ok) throw new Exception(message); results.Add("PASS " + message); }
    static ItemCatalog.Data Catalog() => JsonUtility.FromJson<ItemCatalog.Data>(File.ReadAllText(ModelAdapterService.CatalogPath));
    public static void PickingAndRollback() {
        const string probePath = "Assets/__BodyRollbackProbe.prefab";
        string catalogText = File.ReadAllText(ModelAdapterService.CatalogPath);
        try {
            var first = Catalog().items[0];
            var probe = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(first.prefabPath));
            try { UnityEngine.Object.DestroyImmediate(probe.transform.GetChild(0).Find("Geometry").gameObject); PrefabUtility.SaveAsPrefabAsset(probe, probePath); }
            finally { UnityEngine.Object.DestroyImmediate(probe); }
            var originalPrefab = File.ReadAllBytes(first.prefabPath);
            string report = Path.GetDirectoryName(first.prefabPath) + "/report.json"; var originalReport = File.ReadAllBytes(report);
            var testCatalog = new ItemCatalog.Data { version = ItemCatalog.CurrentVersion, items = new[] { first, new ItemCatalog.Item { id = "FailureProbe", prefabPath = probePath } } };
            string testText = JsonUtility.ToJson(testCatalog, true); File.WriteAllText(ModelAdapterService.CatalogPath, testText);
            bool rejected = false;
            try { ItemBodyColliderBake.BakeAll(); } catch (InvalidOperationException e) { rejected = e.Message.Contains("Missing adapter hierarchy"); }
            Check(rejected && File.ReadAllBytes(first.prefabPath).SequenceEqual(originalPrefab) && File.ReadAllBytes(report).SequenceEqual(originalReport) && File.ReadAllText(ModelAdapterService.CatalogPath) == testText, "failed later model restores prior prefab/report/catalog byte-for-byte");
            File.WriteAllText(ItemBodyColliderBake.Output + "/rollback.txt", results.Last());
        } catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); return; }
        finally { File.WriteAllText(ModelAdapterService.CatalogPath, catalogText); AssetDatabase.DeleteAsset(probePath); AssetDatabase.Refresh(); ItemColliderGeometry.ClearCache(); }
        ItemMeshPickingValidation.Run();
    }
    public static void Run() {
        try {
            Directory.CreateDirectory(ItemBodyColliderBake.Output);
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Physics.simulationMode = SimulationMode.Script;
            ItemColliderGeometry.ConfigureLayers(); BoardPhysicsSettings.Begin();
            ItemPickingMeshPreparation.Prepare();
            var data = Catalog();
            Pile(data, "before");
            ItemBodyColliderBake.BakeAll();
            ValidateAssets(Catalog());
            Drops(Catalog());
            Pile(Catalog(), "after");
            File.WriteAllLines(ItemBodyColliderBake.Output + "/validation.txt", results);
            Debug.Log(string.Join("\n", results)); BoardPhysicsSettings.End(); EditorApplication.Exit(0);
        } catch (Exception e) {
            Directory.CreateDirectory(ItemBodyColliderBake.Output); File.WriteAllText(ItemBodyColliderBake.Output + "/validation.txt", string.Join("\n", results) + "\nFAIL " + e);
            Debug.LogException(e); BoardPhysicsSettings.End(); EditorApplication.Exit(1);
        }
    }
    public static void Verify() {
        try {
            results.Clear(); EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); Physics.simulationMode = SimulationMode.Script; ItemColliderGeometry.ConfigureLayers(); BoardPhysicsSettings.Begin();
            ValidateAssets(Catalog()); Drops(Catalog()); Pile(Catalog(), "after");
            File.WriteAllLines(ItemBodyColliderBake.Output + "/verification.txt", results);
            BoardPhysicsSettings.End(); EditorApplication.Exit(0);
        } catch (Exception e) { File.WriteAllText(ItemBodyColliderBake.Output + "/verification.txt", string.Join("\n", results) + "\nFAIL " + e); Debug.LogException(e); BoardPhysicsSettings.End(); EditorApplication.Exit(1); }
    }
    static void ValidateAssets(ItemCatalog.Data data) {
        int total = 0;
        var poolObject = new GameObject("Validation pool"); var pool = poolObject.AddComponent<BoardItemPool>();
        typeof(BoardItemPool).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(pool, null);
        foreach (var item in data.items) {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item.prefabPath);
            var obj = UnityEngine.Object.Instantiate(prefab);
            try {
                var bodyRoot = obj.transform.GetChild(0).Find("Body");
                var colliders = bodyRoot.GetComponentsInChildren<Collider>(true);
                var initialProfile = JsonUtility.ToJson(ModelAdapterService.BuildProfile(obj.GetComponentsInChildren<Collider>(true).ToList(), obj.transform.GetChild(0), item.id));
                Check(initialProfile == JsonUtility.ToJson(item.colliderProfile), item.id + " prefab matches profile including Body group path");
                Check(colliders.Length > 0 && colliders.Length <= ItemBodyFitter.MaxShapes && colliders.All(c => c.enabled && !c.isTrigger && c.gameObject.layer == ItemColliderGeometry.BodyLayer), item.id + " valid enabled bodies (" + colliders.Length + ")");
                Check(colliders.All(c => c is BoxCollider box ? box.size.x > 0 && box.size.y > 0 && box.size.z > 0 : c is SphereCollider sphere ? sphere.radius > 0 : c is CapsuleCollider cap && cap.radius > 0 && cap.height >= 2 * cap.radius), item.id + " positive primitive dimensions");
                total += colliders.Length;
                var matrices = colliders.Select(c => c.transform.localToWorldMatrix).ToArray();
                ItemColliderGeometry.Apply(obj, item.id, true, true); Physics.SyncTransforms();
                Check(colliders.Select((c, i) => c && c.transform.localToWorldMatrix == matrices[i]).All(v => v), item.id + " forced profile apply preserves oriented children");
                foreach (var c in colliders) { c.enabled = false; c.isTrigger = true; }
                obj.GetComponent<ItemColliderGeometry>().RestoreConfiguredColliderState();
                Check(colliders.All(c => c.enabled && !c.isTrigger), item.id + " restore re-enables all physical children");
                // Independent surface sampling using actual Unity collider distance,
                // including face interiors where vertex-only decomposition can fail.
                Physics.SyncTransforms();
                var triangles = ItemBodyFitter.ReadTriangles(obj.transform.GetChild(0).Find("Geometry"), bodyRoot);
                foreach (var t in triangles) foreach (var p in new[] { t.a, t.b, t.c, (t.a+t.b+t.c)/3, (t.a+t.b)/2, (t.a+t.c)/2, (t.b+t.c)/2 }) {
                    var world = bodyRoot.TransformPoint(p);
                    if (!colliders.Any(c => (c.ClosestPoint(world) - world).sqrMagnitude < 1e-8f)) throw new Exception(item.id + " uncovered mesh sample " + p);
                }
                Check(true, item.id + " actual colliders cover vertices, edges and face centers");
                if (item.id == "Party_Donut") {
                    var vertices = triangles.SelectMany(t => new[] { t.a, t.b, t.c }).ToArray(); var bounds = new Bounds(vertices[0], Vector3.zero); foreach (var p in vertices) bounds.Encapsulate(p);
                    var center = bodyRoot.TransformPoint(bounds.center);
                    Check(colliders.All(c => (c.ClosestPoint(center) - center).sqrMagnitude > .0001f), "donut physical hole remains open");
                }
                if (item.id == "ToyBear" || item.id == "ToyBasketBall" || item.id == "ToyPlane") {
                    var rebuilt = new GameObject("Regenerated body"); rebuilt.transform.SetParent(obj.transform.GetChild(0), false); rebuilt.name = "Body reimport probe";
                    var generated = ItemBodyFitter.Generate(obj.transform.GetChild(0).Find("Geometry"), rebuilt.transform);
                    Check(generated.Count == colliders.Length && generated.Select((c, i) => c.GetType() == colliders[i].GetType() && c.transform.localPosition == colliders[i].transform.localPosition && Quaternion.Angle(c.transform.localRotation, colliders[i].transform.localRotation) < .01f).All(v => v), item.id + " regeneration reproduces baked primitive types/poses");
                    UnityEngine.Object.DestroyImmediate(rebuilt);
                }
                var pooled = BoardItemPool.Get(prefab, item.id);
                foreach (var c in pooled.GetComponentsInChildren<Collider>(true)) c.enabled = false;
                BoardItemPool.Recycle(pooled);
                var reused = BoardItemPool.Get(prefab, item.id);
                Check(reused == pooled && reused.GetComponentsInChildren<Collider>(true).Count(c => c.enabled && c.gameObject.layer == ItemColliderGeometry.BodyLayer) == colliders.Length, item.id + " pool round trip restores every primitive");
                UnityEngine.Object.DestroyImmediate(reused);
            } finally { UnityEngine.Object.DestroyImmediate(obj); }
        }
        // Destroy the test hierarchy immediately in edit mode (templates are
        // children); production Clear intentionally uses deferred Destroy.
        UnityEngine.Object.DestroyImmediate(poolObject);
        results.Add("INFO " + data.items.Length + " items / " + total + " physical primitives");
    }
    static GameObject Spawn(ItemCatalog.Item item, Scene scene, Vector3 position, Quaternion rotation) {
        var obj = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(item.prefabPath));
        SceneManager.MoveGameObjectToScene(obj, scene);
        obj.transform.SetPositionAndRotation(position, rotation); obj.transform.localScale *= item.normalizedToSource;
        ItemColliderGeometry.Apply(obj, item.id, true);
        var rb = obj.GetComponent<Rigidbody>(); rb.isKinematic = false; BoardPhysicsSettings.Apply(rb);
        return obj;
    }
    static void Box(Scene scene, Vector3 position, Vector3 size) {
        var o = new GameObject("Validation boundary"); SceneManager.MoveGameObjectToScene(o, scene); o.layer = ItemColliderGeometry.WallLayer;
        o.transform.position = position; o.AddComponent<BoxCollider>().size = size;
    }
    static void Drops(ItemCatalog.Data data) {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var objects = new List<GameObject>();
        try {
            Box(scene, new Vector3(0, -.25f, 0), new Vector3(400, .5f, 400));
            int index = 0;
            foreach (var item in data.items) for (int rotation = 0; rotation < 4; rotation++) {
                var p = new Vector3((index % 20 - 10) * 8, 4 + rotation * 2, (index / 20 - 7) * 8);
                objects.Add(Spawn(item, scene, p, Quaternion.Euler(rotation * 83, rotation * 47, rotation * 31))); index++;
            }
            Physics.SyncTransforms(); var physics = scene.GetPhysicsScene();
            for (int i = 0; i < 600; i++) physics.Simulate(.02f);
            foreach (var obj in objects) {
                var rb = obj.GetComponent<Rigidbody>();
                Check(ModelAdapterRules.IsFinite(obj.transform.position) && obj.transform.position.y > -1 && obj.transform.position.y < 4 && rb.velocity.magnitude < .3f, obj.name + " rotated drop stable above floor");
            }
            results.Add("INFO " + objects.Count + " drops, 12 simulated seconds each");
        } finally { foreach (var root in scene.GetRootGameObjects()) UnityEngine.Object.DestroyImmediate(root); }
    }
    static void Pile(ItemCatalog.Data data, string label) {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var objects = new List<GameObject>(); var timings = new List<double>();
        try {
            Box(scene, new Vector3(0, -.25f, 0), new Vector3(16, .5f, 16));
            Box(scene, new Vector3(-8, 20, 0), new Vector3(.5f, 40, 17)); Box(scene, new Vector3(8, 20, 0), new Vector3(.5f, 40, 17));
            Box(scene, new Vector3(0, 20, -8), new Vector3(17, 40, .5f)); Box(scene, new Vector3(0, 20, 8), new Vector3(17, 40, .5f));
            var random = new System.Random(42681);
            for (int i = 0; i < 300; i++) {
                var item = data.items[random.Next(data.items.Length)];
                objects.Add(Spawn(item, scene, new Vector3((i % 6 - 2.5f) * 2.25f, 3 + (i / 36) * 2.5f, ((i / 6) % 6 - 2.5f) * 2.25f), Quaternion.Euler((float)random.NextDouble()*360, (float)random.NextDouble()*360, (float)random.NextDouble()*360)));
            }
            Physics.SyncTransforms(); var physics = scene.GetPhysicsScene(); var timer = new System.Diagnostics.Stopwatch();
            for (int i = 0; i < 750; i++) { timer.Restart(); physics.Simulate(.02f); timer.Stop(); if (i >= 10) timings.Add(timer.Elapsed.TotalMilliseconds); }
            Check(objects.All(o => ModelAdapterRules.IsFinite(o.transform.position) && o.transform.position.y > -1 && Mathf.Abs(o.transform.position.x) < 9 && Mathf.Abs(o.transform.position.z) < 9), label + " seeded 300-item pile finite and contained");
            timings.Sort();
            var heights = objects.Select(o => o.GetComponentsInChildren<Renderer>().Max(r => r.bounds.max.y)).OrderBy(h => h).ToArray();
            int primitives = objects.Sum(o => o.GetComponentsInChildren<Collider>().Count(c => c.gameObject.layer == ItemColliderGeometry.BodyLayer));
            results.Add("INFO " + label + " 300-item/15s pile: " + primitives + " primitives; step median " + timings[timings.Count/2].ToString("F3") + " ms, p95 " + timings[(int)(timings.Count*.95)].ToString("F3") + " ms; top median/p95 " + heights[150].ToString("F3") + "/" + heights[285].ToString("F3") + "; sleeping " + objects.Count(o=>o.GetComponent<Rigidbody>().IsSleeping()));
        } finally { foreach (var root in scene.GetRootGameObjects()) UnityEngine.Object.DestroyImmediate(root); }
    }
}
