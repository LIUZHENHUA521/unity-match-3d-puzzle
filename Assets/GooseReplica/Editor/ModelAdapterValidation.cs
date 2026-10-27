using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ModelAdapterValidation {
    [MenuItem("Goose/Validate Model Adapter Catalog")]
    public static void RunMenu() { var report = Run(); Debug.Log(report); }

    public static string Run() {
        int checkedItems = 0, failures = 0;
        ReplicaSourceProfile.ClearCache();
        ReplicaSourceColliders.ClearCache();
        var text = AssetDatabase.LoadAssetAtPath<TextAsset>(ModelAdapterService.CatalogPath);
        if (!text) return "MODEL_ADAPTER_VALIDATION FAIL: missing catalog";
        var data = JsonUtility.FromJson<ReplicaSourceCatalog.Data>(text.text);
        foreach (var item in data?.items ?? Array.Empty<ReplicaSourceCatalog.Item>()) {
            if (item == null || string.IsNullOrEmpty(item.id)) { failures++; continue; }
            checkedItems++;
            Check(AssetDatabase.LoadAssetAtPath<GameObject>(item.prefabPath), "prefab " + item.id, ref failures);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item.prefabPath);
            if (!prefab) continue;
            Check(prefab.CompareTag("Collectable"), "Collectable tag " + item.id, ref failures);
            Check(prefab.GetComponent<Rigidbody>() != null, "Rigidbody " + item.id, ref failures);
            Check(prefab.GetComponent<ReplicaSourceIdentity>()?.LogicalId == item.id, "identity " + item.id, ref failures);
            Check(item.colliderProfile != null && item.colliderProfile.col != null && item.colliderProfile.realCol != null, "Pick/Body profile " + item.id, ref failures);
            Check(ReplicaSourceCatalog.TryGetSize(item.id, out _), "runtime size lookup " + item.id, ref failures);
            Check(ReplicaSourceCatalog.TryGetColliderProfile(item.id, out _), "runtime collider lookup " + item.id, ref failures);
            Check(ReplicaSourceCatalog.TryGetAudioProfile(item.id, out _), "runtime audio lookup " + item.id, ref failures);
            ValidateGameplaySize(prefab, item, ref failures);
            var reportPath = Path.GetDirectoryName(item.prefabPath) + "/report.json";
            Check(AssetDatabase.LoadAssetAtPath<TextAsset>(reportPath), "report " + item.id, ref failures);
        }
        string output = failures == 0 ? "PASS model adapter validation: " + checkedItems + " catalog items" : "FAIL model adapter validation: " + failures + " failures / " + checkedItems + " items";
        Directory.CreateDirectory("Temp"); File.WriteAllText("Temp/model-adapter-validation.txt", output + "\n");
        return output;
    }
    // Exercise the same root-scale and collider restore used by CreateBoardItem.
    // This catches a second application of the import normalization, even when
    // both the catalog and the loader agree on that incorrect value.
    static void ValidateGameplaySize(GameObject prefab, ReplicaSourceCatalog.Item item, ref int failures) {
        var instance = UnityEngine.Object.Instantiate(prefab);
        instance.hideFlags = HideFlags.HideAndDontSave;
        try {
            var renderers = instance.GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToArray();
            Check(renderers.Length > 0, "renderers " + item.id, ref failures);
            if (renderers.Length == 0) return;
            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
            float normalizedLongest = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
            Check(Mathf.Abs(normalizedLongest - ModelAdapterRules.SourceLongestSide) < .005f,
                "normalized Prefab size " + item.id, ref failures);
            Check(Mathf.Abs(item.normalizedToSource - ModelAdapterRules.DefaultGameplayScale) < .001f,
                "gameplay scale must not reuse import scale " + item.id, ref failures);
            instance.transform.localScale *= ReplicaSourceProfile.ModelScale(item.id);
            ReplicaSourceColliders.Apply(instance, item.id, true);
            Physics.SyncTransforms();
            bounds = renderers[0].bounds;
            foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
            float longest = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
            float expected = ModelAdapterRules.DefaultGameplayLongestSide * ReplicaSourceProfile.ModelSizeMultiplier;
            Check(Mathf.Abs(longest - expected) < .01f, "gameplay rendered size " + item.id, ref failures);
            foreach (int layer in new[]{ReplicaSourceColliders.PickLayer, ReplicaSourceColliders.BodyLayer}) {
                var colliders = instance.GetComponentsInChildren<Collider>()
                    .Where(c => c.enabled && c.gameObject.layer == layer).ToArray();
                Check(colliders.Length > 0, "gameplay colliders " + item.id + "/" + layer, ref failures);
                foreach (var collider in colliders) {
                    float side = Mathf.Max(collider.bounds.size.x, Mathf.Max(collider.bounds.size.y, collider.bounds.size.z));
                    Check(ModelAdapterRules.IsFinite(side) && side > .01f && side <= expected * 1.2f,
                        "gameplay collider size " + item.id + "/" + layer, ref failures);
                }
            }
        } finally { UnityEngine.Object.DestroyImmediate(instance); }
    }

    static void Check(bool value, string label, ref int failures) { if (!value) { failures++; Debug.LogError("MODEL_ADAPTER " + label); } }
}
