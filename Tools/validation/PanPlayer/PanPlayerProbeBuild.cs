// Copy into Assets/Editor beside the player fixture in an isolated project.
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
public static class PanPlayerProbeBuild {
    public static void CheckReleaseGuard() {
        var importer = (ModelImporter)AssetImporter.GetAtPath("Assets/Match3D/Kitchen/Art/Grab3D/frying-pan.fbx");
        bool readable = importer.isReadable, rejected = false;
        try {
            importer.isReadable = false;
            importer.SaveAndReimport();
            try { PanCollisionValidation.ValidatePlayerMeshes(); }
            catch (System.InvalidOperationException) { rejected = true; }
        } finally {
            importer.isReadable = readable;
            importer.SaveAndReimport();
        }
        PanCollisionValidation.ValidatePlayerMeshes();
        Debug.Log("PAN_RELEASE_GUARD_REJECTED_UNREADABLE=" + rejected);
        EditorApplication.Exit(rejected ? 0 : 1);
    }
    public static void Run() {
        PanCollisionValidation.ValidatePlayerMeshes();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var probe = new GameObject("Pan player regression").AddComponent<PanPlayerProbe>();
        probe.panPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/Resources/Match3D/Scenes/CountrysideStove.prefab");
        var names = new[] { "Party_ChocolateBar", "Party_BreadSlice", "Party_Bottle" };
        probe.items = new PanPlayerProbe.ItemFixture[names.Length];
        for (int i = 0; i < names.Length; i++) {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/Generated/ModelAdapter/" + names[i] + "/" + names[i] + ".prefab");
            probe.items[i] = new PanPlayerProbe.ItemFixture {
                prefab = prefab,
                vertices = prefab.GetComponentsInChildren<MeshFilter>().SelectMany(m => m.sharedMesh.vertices.Select(v => prefab.transform.InverseTransformPoint(m.transform.TransformPoint(v)))).ToArray()
            };
        }
        EditorSceneManager.SaveScene(scene, "Assets/PanProbe.unity");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { "Assets/PanProbe.unity" },
            locationPathName = "../pan-investigation/regression.app",
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.Development
        });
        EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
    }
}
