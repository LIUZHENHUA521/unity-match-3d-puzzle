using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// CPU mesh picking must also work in players, where unreadable mesh data is stripped.
public sealed class ItemPickingMeshPreparation : IPreprocessBuildWithReport
{
    public int callbackOrder => -100;
    public void OnPreprocessBuild(BuildReport report) { Prepare(); }

    [MenuItem("Goose/Prepare Item Picking Meshes")]
    public static void Prepare()
    {
        var paths = new HashSet<string>();
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" })) {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (!prefab || !prefab.CompareTag("Collectable")) continue;
            Collect(prefab, paths);
        }
        PreparePaths(paths);
        AssetDatabase.SaveAssets();
        Debug.Log("PICK_MESH_PREPARED assets=" + paths.Count);
    }
    public static void EnsureReadable(GameObject source)
    {
        var paths = new HashSet<string>(); Collect(source, paths); PreparePaths(paths);
    }
    static void Collect(GameObject root, HashSet<string> paths)
    {
        foreach (var filter in root.GetComponentsInChildren<MeshFilter>(true))
            if (filter.sharedMesh) paths.Add(AssetDatabase.GetAssetPath(filter.sharedMesh));
    }
    static void PreparePaths(HashSet<string> paths)
    {
        foreach (string path in paths) {
            if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets/", StringComparison.Ordinal)) continue;
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null) {
                if (!importer.isReadable) { importer.isReadable = true; importer.SaveAndReimport(); }
            } else {
                foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path)) if (asset is Mesh mesh && !mesh.isReadable) {
                    var serialized = new SerializedObject(mesh);
                    serialized.FindProperty("m_IsReadable").boolValue = true;
                    serialized.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(mesh);
                }
            }
        }
    }
}
