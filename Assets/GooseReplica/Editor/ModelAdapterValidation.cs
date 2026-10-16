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
            var reportPath = Path.GetDirectoryName(item.prefabPath) + "/report.json";
            Check(AssetDatabase.LoadAssetAtPath<TextAsset>(reportPath), "report " + item.id, ref failures);
        }
        string output = failures == 0 ? "PASS model adapter validation: " + checkedItems + " catalog items" : "FAIL model adapter validation: " + failures + " failures / " + checkedItems + " items";
        Directory.CreateDirectory("Temp"); File.WriteAllText("Temp/model-adapter-validation.txt", output + "\n");
        return output;
    }
    static void Check(bool value, string label, ref int failures) { if (!value) { failures++; Debug.LogError("MODEL_ADAPTER " + label); } }
}
