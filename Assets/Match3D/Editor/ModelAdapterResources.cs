using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

[Serializable]
public sealed class ModelAdapterResourceOptions {
    public bool optimizeTextures = true;
    public int maxTextureSize = 512;
    public bool compressTextures = true;
    public int triangleWarning = 3000;

    public void Validate() {
        if (maxTextureSize != 512 && maxTextureSize != 1024)
            throw new ArgumentException("贴图尺寸必须为 512 或 1024");
        if (triangleWarning < 1) throw new ArgumentException("面数提醒阈值必须大于 0");
    }
}

[Serializable]
public sealed class ModelAdapterResourceStats {
    public long triangles;
    public int meshCount;
    public int materialCount;
    public long sourceFileBytes;
    public long textureMemoryBytes;
    public List<ModelAdapterTextureStats> textures = new List<ModelAdapterTextureStats>();
}

[Serializable]
public sealed class ModelAdapterTextureStats {
    public string name, path, format;
    public int width, height;
    public long memoryBytes;
    public bool embedded;
}

// Optimized resources belong to the generated prefab. Never change shared source importers.
public static class ModelAdapterResources {
    public static ModelAdapterResourceStats Inspect(GameObject root, string inputPath) {
        var stats = new ModelAdapterResourceStats();
        var meshes = new HashSet<Mesh>();
        var materials = new HashSet<Material>();
        var textures = new HashSet<Texture>();
        var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AddFile(files, inputPath);
        foreach (var renderer in root.GetComponentsInChildren<Renderer>(true)) {
            var filter = renderer.GetComponent<MeshFilter>();
            var mesh = renderer is SkinnedMeshRenderer skin ? skin.sharedMesh : filter ? filter.sharedMesh : null;
            if (mesh) {
                meshes.Add(mesh);
                AddFile(files, AssetDatabase.GetAssetPath(mesh));
                for (int i = 0; i < mesh.subMeshCount; i++) {
                    if (mesh.GetTopology(i) == MeshTopology.Triangles) stats.triangles += mesh.GetIndexCount(i) / 3;
                    else if (mesh.GetTopology(i) == MeshTopology.Quads) stats.triangles += mesh.GetIndexCount(i) / 4 * 2;
                }
            }
            foreach (var material in renderer.sharedMaterials) {
                if (!material || !materials.Add(material)) continue;
                AddFile(files, AssetDatabase.GetAssetPath(material));
                foreach (string property in material.GetTexturePropertyNames()) {
                    var texture = material.GetTexture(property);
                    if (!texture || !textures.Add(texture)) continue;
                    string path = AssetDatabase.GetAssetPath(texture);
                    AddFile(files, path);
                    long memory = Profiler.GetRuntimeMemorySizeLong(texture);
                    stats.textureMemoryBytes += memory;
                    stats.textures.Add(new ModelAdapterTextureStats {
                        name = texture.name, path = path, width = texture.width, height = texture.height,
                        format = texture.graphicsFormat.ToString(), memoryBytes = memory,
                        embedded = AssetDatabase.IsSubAsset(texture)
                    });
                }
            }
        }
        stats.meshCount = meshes.Count;
        stats.materialCount = materials.Count;
        stats.sourceFileBytes = files.Sum(p => new FileInfo(p).Length);
        return stats;
    }

    static void AddFile(HashSet<string> files, string path) {
        if (!string.IsNullOrEmpty(path) && File.Exists(path)) files.Add(path);
    }

    public static void AddScanIssues(ModelAdapterReport report, ModelAdapterResourceOptions options) {
        var stats = report.sourceResources;
        report.Add(ModelAdapterSeverity.Pass, "RESOURCE_STATS", Summary(stats));
        if (stats.triangles > options.triangleWarning)
            report.Add(ModelAdapterSeverity.Warning, "TRIANGLE_BUDGET", "超过 " + options.triangleWarning + " 三角面；仅提醒，不自动减面。数量包含所有子物体和 LOD。");
        foreach (var texture in stats.textures) {
            if (Mathf.Max(texture.width, texture.height) > options.maxTextureSize)
                report.Add(ModelAdapterSeverity.Warning, "TEXTURE_SIZE", texture.name + " 当前 " + texture.width + "×" + texture.height + "，目标上限 " + options.maxTextureSize, texture.path);
        }
        if (stats.textures.Count == 0 && AssetImporter.GetAtPath(report.inputPath) is ModelImporter)
            report.Add(ModelAdapterSeverity.Warning, "MODEL_TEXTURE_UNRESOLVED", "当前材质未引用贴图；启用优化后会尝试从模型副本提取内嵌贴图。纯色材质可忽略此提示。");
    }

    public static string Summary(ModelAdapterResourceStats stats) {
        return stats.triangles + " 三角面 / " + stats.meshCount + " 网格 / " + stats.materialCount + " 材质 / " + stats.textures.Count + " 贴图";
    }

    public static void Optimize(GameObject root, string directory, ModelAdapterResourceOptions options, ModelAdapterReport report) {
        if (!options.optimizeTextures) return;
        options.Validate();
        EnsureFolder(directory);
        string resolvedFolder = directory + "/ResolvedSources";
        if (ResolveModelMaterials(root, resolvedFolder)) {
            long sourceBytes = report.sourceResources.sourceFileBytes;
            report.sourceResources = Inspect(root, report.inputPath);
            report.sourceResources.sourceFileBytes = sourceBytes;
            foreach (var texture in report.sourceResources.textures) {
                if (!texture.path.StartsWith(resolvedFolder + "/", StringComparison.Ordinal)) continue;
                texture.path = report.inputPath + "#" + texture.name;
                texture.embedded = true;
            }
            report.issues.RemoveAll(issue => issue.code == "RESOURCE_STATS" || issue.code == "TEXTURE_SIZE" || issue.code == "MODEL_TEXTURE_UNRESOLVED" || issue.code == "TRIANGLE_BUDGET");
            AddScanIssues(report, options);
        }
        var materials = new Dictionary<Material, Material>();
        var textures = new Dictionary<Texture, Texture>();
        var extracted = new Dictionary<string, string>();
        foreach (var renderer in root.GetComponentsInChildren<Renderer>(true)) {
            var slots = renderer.sharedMaterials;
            for (int i = 0; i < slots.Length; i++) {
                var source = slots[i];
                if (!source) continue;
                if (!materials.TryGetValue(source, out var copy)) {
                    copy = new Material(source);
                    AssetDatabase.CreateAsset(copy, directory + "/material_" + materials.Count + ".mat");
                    materials.Add(source, copy);
                    foreach (string property in source.GetTexturePropertyNames()) {
                        var texture = source.GetTexture(property);
                        if (!texture) continue;
                        if (!textures.TryGetValue(texture, out var optimized)) {
                            optimized = CopyTexture(texture, directory, textures.Count, options, extracted, report);
                            textures.Add(texture, optimized);
                        }
                        copy.SetTexture(property, optimized);
                    }
                    EditorUtility.SetDirty(copy);
                }
                slots[i] = copy;
            }
            renderer.sharedMaterials = slots;
        }
        foreach (var folder in extracted.Values) AssetDatabase.DeleteAsset(folder);
        AssetDatabase.SaveAssets();
        AssetDatabase.DeleteAsset(resolvedFolder);
    }

    // Unity can import FBX materials with empty texture slots until ExtractTextures runs.
    // Resolve those on a throwaway FBX copy, then transfer only its materials to the instance.
    static bool ResolveModelMaterials(GameObject root, string directory) {
        bool changed = false;
        var modelMaterials = new Dictionary<Material, Material>();
        var sources = root.GetComponentsInChildren<Renderer>(true).SelectMany(r => r.sharedMaterials)
            .Where(m => m).Distinct().GroupBy(AssetDatabase.GetAssetPath)
            .Where(g => AssetImporter.GetAtPath(g.Key) is ModelImporter).ToArray();
        for (int index = 0; index < sources.Length; index++) {
            var group = sources[index];
            string folder = directory + "/model_" + index;
            EnsureFolder(folder);
            string copy = folder + "/model" + Path.GetExtension(group.Key);
            if (!AssetDatabase.CopyAsset(group.Key, copy)) throw new IOException("无法复制模型：" + group.Key);
            var importer = (ModelImporter)AssetImporter.GetAtPath(copy);
            if (!importer.ExtractTextures(folder)) continue; // Model has no embedded textures.
            AssetDatabase.Refresh();
            importer.SaveAndReimport();
            var copies = new Dictionary<long, Material>();
            foreach (var mat in AssetDatabase.LoadAllAssetsAtPath(copy).OfType<Material>()) {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mat, out string guid, out long id)) copies[id] = mat;
            }
            foreach (var material in group) {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(material, out string guid, out long id) && copies.TryGetValue(id, out var resolved)) {
                    modelMaterials[material] = resolved;
                    changed = true;
                }
            }
        }
        foreach (var renderer in root.GetComponentsInChildren<Renderer>(true)) {
            var slots = renderer.sharedMaterials;
            for (int i = 0; i < slots.Length; i++)
                if (slots[i] && modelMaterials.TryGetValue(slots[i], out var resolved)) slots[i] = resolved;
            renderer.sharedMaterials = slots;
        }
        return changed;
    }

    static Texture CopyTexture(Texture source, string directory, int index, ModelAdapterResourceOptions options,
        Dictionary<string, string> extracted, ModelAdapterReport report) {
        string path = AssetDatabase.GetAssetPath(source);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (!(source is Texture2D) || (importer == null && !(AssetImporter.GetAtPath(path) is ModelImporter))) {
            report.Add(ModelAdapterSeverity.Warning, "TEXTURE_SKIPPED", "保留无法通过 TextureImporter 处理的贴图：" + source.name, path);
            return source;
        }
        if (importer == null) {
            // Extract from a temporary MODEL COPY, so FBX source bytes and remaps stay untouched.
            if (!extracted.TryGetValue(path, out var folder)) {
                folder = directory + "/extract_" + extracted.Count;
                EnsureFolder(folder);
                string modelCopy = folder + "/source" + Path.GetExtension(path);
                if (!AssetDatabase.CopyAsset(path, modelCopy)) throw new IOException("无法复制内嵌贴图模型：" + path);
                var modelImporter = (ModelImporter)AssetImporter.GetAtPath(modelCopy);
                if (!modelImporter.ExtractTextures(folder)) throw new IOException("无法提取 FBX 内嵌贴图：" + path);
                AssetDatabase.Refresh();
                extracted.Add(path, folder);
            }
            var matches = AssetDatabase.FindAssets("t:Texture2D", new[] { folder })
                .Select(AssetDatabase.GUIDToAssetPath).Distinct()
                .Where(p => AssetImporter.GetAtPath(p) is TextureImporter)
                .Where(p => string.Equals(Path.GetFileNameWithoutExtension(p), Path.GetFileNameWithoutExtension(source.name), StringComparison.OrdinalIgnoreCase)).ToArray();
            if (matches.Length != 1) throw new IOException("无法唯一匹配内嵌贴图：" + source.name);
            path = matches[0];
        }
        string target = directory + "/texture_" + index + Path.GetExtension(path);
        if (!AssetDatabase.CopyAsset(path, target)) throw new IOException("无法复制贴图：" + path);
        var settings = (TextureImporter)AssetImporter.GetAtPath(target);
        // Clear inherited platform overrides: an old 2048 override must not defeat the chosen cap.
        foreach (var platform in ReadPlatformNames(settings)) settings.ClearPlatformTextureSettings(platform);
        settings.maxTextureSize = options.maxTextureSize;
        settings.isReadable = false;
        settings.textureCompression = options.compressTextures ? TextureImporterCompression.Compressed : TextureImporterCompression.Uncompressed;
        settings.crunchedCompression = false;
        var defaults = settings.GetDefaultPlatformTextureSettings();
        defaults.maxTextureSize = options.maxTextureSize;
        defaults.format = TextureImporterFormat.Automatic;
        defaults.textureCompression = settings.textureCompression;
        defaults.crunchedCompression = false;
        settings.SetPlatformTextureSettings(defaults);
        settings.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(target);
    }

    static string[] ReadPlatformNames(TextureImporter importer) {
        var names = new List<string>();
        var data = new SerializedObject(importer);
        var platforms = data.FindProperty("m_PlatformSettings");
        if (platforms == null || !platforms.isArray) throw new InvalidOperationException("无法读取贴图平台设置");
        for (int i = 0; i < platforms.arraySize; i++) {
            string name = platforms.GetArrayElementAtIndex(i).FindPropertyRelative("m_BuildTarget").stringValue;
            if (name != "DefaultTexturePlatform") names.Add(name);
        }
        return names.ToArray();
    }

    public static void EnsureFolder(string path) {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
    }
}
