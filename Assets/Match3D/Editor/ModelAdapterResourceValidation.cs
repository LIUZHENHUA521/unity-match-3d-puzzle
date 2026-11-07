using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ModelAdapterResourceValidation {
    const string Input = ModelAdapterService.InputRoot + "/ResourceValidationFixture";
    const string Id = "ResourceValidationFixture";
    static int checks;

    public static void Run() {
        string catalog = File.ReadAllText(ModelAdapterService.CatalogPath);
        string result = "";
        int exitCode = 0;
        bool ownsFixture = false;
        try {
            Check(!Directory.Exists(Input) && !Directory.Exists(ModelAdapterService.OutputRoot + "/" + Id), "fixture paths unused");
            ownsFixture = true;
            ModelAdapterResources.EnsureFolder(Input);
            CreateTexture(Input + "/color.png", false);
            CreateTexture(Input + "/normal.png", true);
            var material = new Material(Shader.Find("Standard"));
            material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(Input + "/color.png");
            material.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>(Input + "/normal.png"));
            material.EnableKeyword("_NORMALMAP");
            material.mainTextureScale = new Vector2(2, 3);
            AssetDatabase.CreateAsset(material, Input + "/source.mat");
            var root = new GameObject(Id);
            for (int i = 0; i < 2; i++) {
                var child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                child.transform.SetParent(root.transform);
                child.transform.localPosition = Vector3.right * i;
                child.GetComponent<Renderer>().sharedMaterial = material;
            }
            string source = Input + "/source.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, source);
            UnityEngine.Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            string colorMeta = File.ReadAllText(Input + "/color.png.meta");
            string normalMeta = File.ReadAllText(Input + "/normal.png.meta");
            string materialBytes = File.ReadAllText(Input + "/source.mat");
            var options = new ModelAdapterResourceOptions { triangleWarning = 10 };
            var scan = ModelAdapterService.Scan(source, Id, Id, true, false, options).report;
            Check(!scan.HasErrors, "scan success");
            Check(scan.sourceResources.triangles == 24, "triangle count includes both mesh instances");
            Check(scan.sourceResources.meshCount == 1 && scan.sourceResources.materialCount == 1 && scan.sourceResources.textures.Count == 2, "shared resources counted once");
            Check(scan.issues.Any(x => x.code == "TRIANGLE_BUDGET"), "high triangle warning");
            Check(!Directory.Exists(ModelAdapterService.OutputRoot + "/" + Id), "scan does not generate output");
            var generated = ModelAdapterService.Generate(source, Id, Id, false, true, options);
            ValidateOutput(generated, 512, true);
            Check(generated.generatedResources.textureMemoryBytes < generated.sourceResources.textureMemoryBytes, "imported texture memory reduced");
            string prefabGuid = AssetDatabase.AssetPathToGUID(generated.generatedPrefabPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(generated.generatedPrefabPath);
            var renderers = prefab.GetComponentsInChildren<Renderer>(true);
            Check(renderers[0].sharedMaterial == renderers[1].sharedMaterial, "material deduplication");
            Check(renderers[0].sharedMaterial.mainTextureScale == new Vector2(2, 3), "UV tiling preserved");
            var normal = renderers[0].sharedMaterial.GetTexture("_BumpMap");
            Check(((TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(normal))).textureType == TextureImporterType.NormalMap, "normal-map import preserved");
            var color = renderers[0].sharedMaterial.mainTexture;
            Check(((TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(color))).DoesSourceTextureHaveAlpha(), "alpha source retained");
            options.maxTextureSize = 1024;
            options.compressTextures = false;
            generated = ModelAdapterService.Generate(source, Id, Id, true, true, options);
            ValidateOutput(generated, 1024, false);
            Check(AssetDatabase.AssetPathToGUID(generated.generatedPrefabPath) == prefabGuid, "update retains prefab GUID");
            Check(generated.generatedResources.triangles == 24, "geometry unchanged on update");
            options.optimizeTextures = false;
            generated = ModelAdapterService.Generate(source, Id, Id, true, true, options);
            Check(!generated.HasErrors, "optimization disabled generation");
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(generated.generatedPrefabPath);
            Check(prefab.GetComponentInChildren<Renderer>().sharedMaterial == material, "disabled mode retains original material");
            Check(!Directory.Exists(ModelAdapterService.OutputRoot + "/" + Id + "/Optimized"), "update removes old optimized output");
            Check(File.ReadAllText(Input + "/color.png.meta") == colorMeta && File.ReadAllText(Input + "/normal.png.meta") == normalMeta, "source texture importers unchanged");
            Check(File.ReadAllText(Input + "/source.mat") == materialBytes, "source material unchanged");
            options.maxTextureSize = 128;
            var invalid = ModelAdapterService.Generate(source, Id, Id, true, true, options);
            Check(invalid.HasErrors && invalid.issues.Any(x => x.code == "RESOURCE_OPTIONS"), "invalid options rejected before mutation");
            AssetDatabase.DeleteAsset(ModelAdapterService.OutputRoot + "/_diagnostics/" + invalid.requestId + ".json");
            ValidateEmbedded();
            result = "PASS model adapter resources: " + checks + " checks";
        } catch (Exception ex) {
            result = "FAIL model adapter resources: " + ex;
            exitCode = 1;
            Debug.LogException(ex);
        } finally {
            File.WriteAllText(ModelAdapterService.CatalogPath, catalog);
            AssetDatabase.ImportAsset(ModelAdapterService.CatalogPath);
            if (ownsFixture) {
                AssetDatabase.DeleteAsset(Input);
                AssetDatabase.DeleteAsset(ModelAdapterService.OutputRoot + "/" + Id);
                AssetDatabase.DeleteAsset(ModelAdapterService.OutputRoot + "/" + Id + "Embedded");
            }
            ItemCatalog.ClearCache();
            AssetDatabase.Refresh();
            Directory.CreateDirectory("Logs");
            File.WriteAllText("Logs/model-adapter-resources-validation.txt", result + "\n");
            Debug.Log(result);
        }
        if (Application.isBatchMode) EditorApplication.Exit(exitCode);
    }

    static void ValidateEmbedded() {
        string fbx = Environment.GetEnvironmentVariable("MODEL_ADAPTER_TEST_FBX");
        if (string.IsNullOrEmpty(fbx)) { Debug.Log("Embedded FBX fixture not provided; skipped."); return; }
        string target = Input + "/embedded.fbx";
        File.Copy(fbx, target);
        AssetDatabase.ImportAsset(target, ImportAssetOptions.ForceSynchronousImport);
        string meta = File.ReadAllText(target + ".meta");
        var scan = ModelAdapterService.Scan(target, Id + "Embedded", "Embedded", true).report;
        Check(scan.sourceResources.textures.Count == 0 || scan.sourceResources.textures.Any(t => t.embedded), "FBX fixture needs embedded texture resolution");
        var result = ModelAdapterService.Generate(target, Id + "Embedded", "Embedded", false, true);
        ValidateOutput(result, 512, true);
        Check(result.sourceResources.triangles == result.generatedResources.triangles, "FBX geometry unchanged");
        Check(!result.generatedResources.textures.Any(t => t.embedded), "FBX embedded texture replaced by optimized external texture");
        Check(File.ReadAllText(target + ".meta") == meta, "FBX importer unchanged");
        Capture(AssetDatabase.LoadAssetAtPath<GameObject>(result.generatedPrefabPath));
        File.WriteAllText("Logs/model-adapter-embedded-result.json", JsonUtility.ToJson(result, true));
    }

    static void Capture(GameObject prefab) {
        var utility = new PreviewRenderUtility(true);
        Texture2D image = null;
        try {
            var instance = UnityEngine.Object.Instantiate(prefab);
            utility.AddSingleGO(instance);
            var renderers = instance.GetComponentsInChildren<Renderer>();
            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
            utility.camera.transform.position = bounds.center + new Vector3(1, .8f, 1).normalized * bounds.size.magnitude * 1.8f;
            utility.camera.transform.LookAt(bounds.center);
            utility.camera.nearClipPlane = .01f;
            utility.camera.farClipPlane = 10;
            utility.camera.fieldOfView = 35;
            utility.camera.clearFlags = CameraClearFlags.Color;
            utility.camera.backgroundColor = new Color(.15f, .15f, .15f, 1);
            utility.lights[0].intensity = 1.1f;
            utility.lights[0].transform.rotation = Quaternion.Euler(35, 35, 0);
            utility.lights[1].intensity = .6f;
            utility.BeginStaticPreview(new Rect(0, 0, 512, 512));
            utility.camera.Render();
            image = utility.EndStaticPreview();
            File.WriteAllBytes("Logs/model-adapter-croissant-512.png", image.EncodeToPNG());
        } finally {
            if (image) UnityEngine.Object.DestroyImmediate(image);
            utility.Cleanup();
        }
    }

    static void ValidateOutput(ModelAdapterReport report, int size, bool compressed) {
        Check(!report.HasErrors, "generate: " + string.Join("; ", report.issues.Where(x => x.severity == ModelAdapterSeverity.Error).Select(x => x.message)));
        Check(report.generatedResources.textures.Count > 0, "generated texture references exist after staging move");
        foreach (var texture in report.generatedResources.textures) {
            Check(Mathf.Max(texture.width, texture.height) == size, "texture cap " + size);
            Check(texture.path.StartsWith(ModelAdapterService.OutputRoot + "/" + report.logicalId + "/Optimized/"), "texture uses generated copy");
            var importer = (TextureImporter)AssetImporter.GetAtPath(texture.path);
            Check(importer.maxTextureSize == size && !importer.isReadable, "importer cap and read/write");
            Check(!importer.GetPlatformTextureSettings("WebGL").overridden, "WebGL inherited override cleared");
            Check(importer.textureCompression == (compressed ? TextureImporterCompression.Compressed : TextureImporterCompression.Uncompressed), "compression setting");
        }
    }

    static void CreateTexture(string path, bool normal) {
        var texture = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
        var pixels = new Color32[2048 * 2048];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = normal ? new Color32(128, 128, 255, 255) : new Color32((byte)(i % 255), 120, 40, 128);
        texture.SetPixels32(pixels);
        File.WriteAllBytes(path, texture.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 2048;
        importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings { name = "WebGL", overridden = true, maxTextureSize = 2048, format = TextureImporterFormat.RGBA32 });
        importer.SaveAndReimport();
    }

    static void Check(bool ok, string label) {
        if (!ok) throw new Exception(label);
        checks++;
        Debug.Log("MODEL_RESOURCE_CHECK " + label);
    }
}
