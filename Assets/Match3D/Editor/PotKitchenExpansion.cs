using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PotKitchenExpansion {
    static PotKitchenExpansion() { EditorApplication.update += Poll; }
    static void Poll() {
        const string request = "Temp/pot-kitchen-expansion";
        if (!File.Exists(request) || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        File.Delete(request);
        try { Build(); File.WriteAllText(request + "-result", "PASS: 26 models; adapters, materials and weights validated."); }
        catch (Exception e) { File.WriteAllText(request + "-result", e.ToString()); Debug.LogException(e); }
    }
    public static void Build() {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play first.");
        var asset = AssetDatabase.LoadAssetAtPath<Match3DModelThemes>("Assets/Match3D/Resources/Match3D/ModelThemes.asset");
        var profile = asset.themes.Single(t => t.id == "tasty-kitchen").profile;
        if (profile.prefabs.Length != 23) throw new InvalidOperationException("Expected 23 existing kitchen models.");
        string[] names = { "corn", "fish", "turkey" }, labels = { "玉米", "鱼", "火鸡" };
        var additions = new GameObject[3];
        var tray = Match3DTrayDisplaySettings.Current;
        for (int i = 0; i < names.Length; i++) {
            var source = Directory.GetFiles("Assets/Match3D/IncomingModels/PotKitchenExpansion", names[i] + ".obj", SearchOption.AllDirectories).Single();
            var report = ModelAdapterService.Generate(source, "PotKitchen_" + names[i], labels[i], true, true, new ModelAdapterResourceOptions { optimizeTextures = false });
            if (report.HasErrors || report.status != "Generated") throw new Exception(JsonUtility.ToJson(report));
            var prefab = additions[i] = AssetDatabase.LoadAssetAtPath<GameObject>(report.generatedPrefabPath);
            var error = ThemeItemsEditorService.CheckPrefab(prefab);
            if (error != null) throw new Exception(error);
            if (prefab.GetComponentsInChildren<Renderer>(true).Any(r => r.sharedMaterials.Any(m => !m || !m.shader || !m.shader.isSupported))) throw new Exception("Invalid material: " + names[i]);
            tray.Set(prefab, new Match3DTrayDisplaySettings.Pose { rotation = new Vector3(-90, 0, 0), scale = 1 });
        }
        foreach (var layer in profile.layers) {
            if (layer.weights != null && layer.weights.Length > 0) {
                if (layer.weights.Length != 23) throw new Exception("Unexpected weights.");
                layer.weights = layer.weights.Concat(new[] { 1f, 1f, 1f }).ToArray();
            }
        }
        profile.prefabs = profile.prefabs.Concat(additions).ToArray();
        EditorUtility.SetDirty(asset); EditorUtility.SetDirty(tray);
        AssetDatabase.SaveAssets();
    }
}
