using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Deterministic native asset import. Run prepare_pot_food_packs.py first.
public static class PotFoodPackImporter {
    public const string Root = "Assets/Match3D/IncomingModels/PotFoodPacks";
    public static readonly string[] Packs = { "vegetable", "meat", "grocery", "fruit", "sweets" };
    [Serializable] public sealed class Pack { public string id, pack, title; public MaterialData[] materials; public Model[] models; }
    [Serializable] public sealed class MaterialData { public string name, texture; }
    [Serializable] public sealed class Model { public string name, label; public int material; public Vector3[] vertices, normals; public Vector2[] uv; public int[] triangles; }

    [MenuItem("Pot/Import Purchased Food Themes")]
    public static void Build() {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play before importing themes");
        Directory.CreateDirectory("work/pot-food-packs");
        foreach (var folder in new[] { "Meshes", "Materials", "Prefabs" }) ModelAdapterResources.EnsureFolder(Root + "/" + folder);
        AssetDatabase.Refresh();
        var catalog = AssetDatabase.LoadAssetAtPath<Match3DModelThemes>("Assets/Match3D/Resources/Match3D/ModelThemes.asset");
        var baseline = catalog.themes.Single(t => t.id == "foodie-party");
        var entries = catalog.themes.ToList();
        var tray = Match3DTrayDisplaySettings.Current;
        foreach (var packName in Packs) {
            var pack = JsonUtility.FromJson<Pack>(File.ReadAllText(Root + "/Editor/pot-" + packName + ".json"));
            if (pack.models.Length != 26 || pack.models.Select(m => m.name).Distinct().Count() != 26) throw new Exception("Expected 26 distinct models: " + packName);
            var materials = pack.materials.Select(m => {
                string texturePath = Root + "/Textures/" + m.texture;
                var importer = (TextureImporter)AssetImporter.GetAtPath(texturePath);
                importer.textureType = TextureImporterType.Default;
                importer.sRGBTexture = true; importer.alphaSource = TextureImporterAlphaSource.None;
                importer.maxTextureSize = 1024; importer.mipmapEnabled = true;
                importer.textureCompression = TextureImporterCompression.Compressed; importer.SaveAndReimport();
                var material = new Material(Shader.Find("Standard")) { name = m.name, enableInstancing = true };
                material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                material.SetFloat("_Metallic", 0); material.SetFloat("_Glossiness", .3f);
                return Save(material, Root + "/Materials/" + m.name + ".mat");
            }).ToArray();
            var prefabs = pack.models.Select((model, index) => {
                string id = "Pot_" + pack.pack + "_" + model.name;
                File.WriteAllText("work/pot-food-packs/progress.txt", pack.title + " " + (index + 1) + "/26 " + model.name);
                var mesh = new Mesh { name = id, vertices = model.vertices, normals = model.normals, uv = model.uv, triangles = model.triangles };
                mesh.RecalculateBounds(); mesh.RecalculateTangents();
                mesh = Save(mesh, Root + "/Meshes/" + id + ".asset");
                string inputPath = Root + "/Prefabs/" + id + ".prefab";
                var source = new GameObject(id);
                try {
                    source.AddComponent<MeshFilter>().sharedMesh = mesh;
                    source.AddComponent<MeshRenderer>().sharedMaterial = materials[model.material];
                    PrefabUtility.SaveAsPrefabAsset(source, inputPath);
                } finally { UnityEngine.Object.DestroyImmediate(source); }
                var report = ModelAdapterService.Generate(inputPath, id, model.label, true, true, new ModelAdapterResourceOptions { optimizeTextures = false });
                if (report.HasErrors || report.status != "Generated") throw new Exception(JsonUtility.ToJson(report, true));
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(report.generatedPrefabPath);
                // Upright source meshes are presented at a readable three-quarter angle in the tray.
                tray.Set(prefab, new Match3DTrayDisplaySettings.Pose { rotation = new Vector3(-20, 25, 0), scale = 1 });
                return prefab;
            }).ToArray();
            var profile = JsonUtility.FromJson<LevelLayoutConfig.Profile>(JsonUtility.ToJson(baseline.profile));
            profile.prefabs = prefabs;
            profile.tutorialNames = prefabs.Take(5).Select(p => p.name).ToArray();
            // All 26 models participate in the first challenge layer. Other gameplay parameters remain authored defaults.
            profile.layers[0].types = 26;
            LevelLayoutConfig.GetTutorial(profile);
            var entry = new Match3DModelThemes.Entry { id = pack.id, title = pack.title, profile = profile,
                background = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Match3D/Resources/Match3D/ThemeBackgrounds/" + pack.id + ".jpg") ?? baseline.background,
                ambientMultiplier = .75f, lightMultiplier = .85f };
            int existing = entries.FindIndex(t => t.id == pack.id);
            if (existing >= 0) entries[existing] = entry; else entries.Add(entry);
        }
        catalog.themes = entries.ToArray();
        EditorUtility.SetDirty(catalog); EditorUtility.SetDirty(tray);
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        ValidateAssets();
    }

    static T Save<T>(T value, string path) where T : UnityEngine.Object {
        var old = AssetDatabase.LoadAssetAtPath<T>(path);
        if (!old) { AssetDatabase.CreateAsset(value, path); return value; }
        EditorUtility.CopySerialized(value, old); EditorUtility.SetDirty(old); UnityEngine.Object.DestroyImmediate(value); return old;
    }

    public static void ValidateAssets() {
        var catalog = Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
        var data = JsonUtility.FromJson<ItemCatalog.Data>(File.ReadAllText(ModelAdapterService.CatalogPath));
        foreach (string pack in Packs) {
            var entry = catalog.themes.Single(t => t.id == "pot-" + pack);
            if (entry.profile.prefabs.Length != 26) throw new Exception("Invalid model count " + pack);
            foreach (var prefab in entry.profile.prefabs) {
                string error = ThemeItemsEditorService.CheckPrefab(prefab);
                if (error != null) throw new Exception(prefab.name + ": " + error);
                var item = data.items.Single(i => i.id == prefab.name);
                if (item.colliderProfile.col == null || item.colliderProfile.realCol == null) throw new Exception("Missing colliders " + prefab.name);
                foreach (var f in prefab.GetComponentsInChildren<MeshFilter>(true))
                    if (!f.sharedMesh || !f.sharedMesh.isReadable || f.sharedMesh.vertexCount == 0) throw new Exception("Unreadable mesh " + prefab.name);
                foreach (var r in prefab.GetComponentsInChildren<Renderer>(true))
                    if (r.sharedMaterials.Any(m => !m || !m.shader || !m.mainTexture)) throw new Exception("Missing textured material " + prefab.name);
            }
            var tutorial = LevelLayoutConfig.GetTutorial(entry.profile);
            if (tutorial.ordinaryTypes.Length < 2) throw new Exception("Invalid tutorial " + pack);
        }
        File.WriteAllText("work/pot-food-packs/assets.txt", "PASS: 5 themes, 130 unique native prefabs, textured readable meshes, Pick/Body colliders, tutorial pools.\n");
    }
}
