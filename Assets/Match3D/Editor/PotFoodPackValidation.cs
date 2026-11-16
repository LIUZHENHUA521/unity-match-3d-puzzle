using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PotFoodPackValidation {
    const string Output = "work/pot-food-packs";
    static bool started;
    static double deadline;
    static int checks;
    static void Check(bool value, string message) { if (!value) throw new Exception(message); checks++; }
    public static void Run() {
        Directory.CreateDirectory(Output); PotFoodPackImporter.ValidateAssets(); PotFoodPackPreviews.Build();
        EditorSettings.enterPlayModeOptionsEnabled = true;
        EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
        EditorSceneManager.OpenScene("Assets/Scenes/Match3D.unity");
        deadline = EditorApplication.timeSinceStartup + 600;
        EditorApplication.update += Tick; EditorApplication.isPlaying = true;
    }
    static void Tick() {
        if (EditorApplication.timeSinceStartup > deadline) { Finish("FAIL runtime timeout"); return; }
        if (started || !EditorApplication.isPlaying || !Match3DFlow.Instance || !Match3DFlow.Instance.StartupComplete) return;
        started = true;
        new GameObject("Pot food pack validation").AddComponent<BoardValidationDriver>().StartCoroutine(RunChecks());
    }
    static IEnumerator RunChecks() {
        var steps = Core(); string error = null;
        while (true) {
            bool more = false;
            try { more = steps.MoveNext(); } catch (Exception e) { error = e.ToString(); }
            if (!more || error != null) break;
            yield return steps.Current;
        }
        Finish(error == null ? "PASS: " + checks + " assertions; all 5 themes, 130 pickups, triple removal, 300-item piles, shared opening replay and legacy pools." : "FAIL " + error);
    }
    static void Finish(string result) {
        EditorApplication.update -= Tick; File.WriteAllText(Output + "/runtime.txt", result); Debug.Log(result);
        EditorApplication.Exit(result.StartsWith("PASS") ? 0 : 1);
    }
    static IEnumerator Ready() {
        float until = Time.realtimeSinceStartup + 60;
        while (Match3DFlow.Instance.CreationInProgress && Time.realtimeSinceStartup < until) yield return null;
        Check(!Match3DFlow.Instance.CreationInProgress, "creation timed out");
    }
    static IEnumerator Core() {
        var flow = Match3DFlow.Instance; var input = InputController.Instance; input.enabled = false;
        var catalog = Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
        Check(catalog.themes.Length == 8, "8 model themes");
        foreach (var id in new[] { "playful-toys", "tasty-kitchen", "foodie-party" })
            Check(Match3DModelThemes.Get(id, "CountrysideStove").prefabs.Length == 23, "legacy pool unchanged " + id);
        foreach (string pack in PotFoodPackImporter.Packs) {
            string id = "pot-" + pack;
            File.WriteAllText(Output + "/runtime-progress.txt", id);
            var profile = Match3DModelThemes.Get(id, "CountrysideStove");
            var names = profile.prefabs.Select(p => p.name).ToArray();
            var level = new Match3DDailyLevel { date = "2026-11-17", tier = "bachelor", sourceDate = "2026-11-17", version = "pot-food-validation", generatorVersion = "daily-v1", modelThemeId = id, seed = 15937, seconds = 600, initialCount = 300, limitHeight = 21.5f,
                layers = new[] { new ChallengeLayer(150, 26, 14), new ChallengeLayer(60, 10, 18), new ChallengeLayer(45, 8, 20), new ChallengeLayer(45, 7, 22) } };
            Check(level.IsValid(26), "26-model daily config");
            flow.BeginDailyPractice(0, level); yield return Ready();
            var board = LevelGenerator.Instance.objectsOnScene;
            Check(flow.Stage == 1 && board.Count == 15 && board.All(o => names.Contains(o.GetComponent<BoardItemIdentity>().LogicalId)), id + " tutorial");
            var first = flow.CaptureDailyOpening();
            foreach (var item in board.GroupBy(o => o.name).First().Take(3).ToArray()) input.AddObjectOnStack(item);
            yield return new WaitForSeconds(1);
            Check(input.objectsOnStack.Count == 0 && board.Count == 12, id + " triple clears");
            flow.NextStage(); yield return Ready();
            Check(board.Count == 300 && board.All(o => names.Contains(o.GetComponent<BoardItemIdentity>().LogicalId)), id + " challenge pool");
            Check(board.Select(o => o.GetComponent<BoardItemIdentity>().LogicalId).Distinct().Count() == 26, id + " all 26 appear");
            var second = flow.CaptureDailyOpening();
            level.opening = new Match3DDailySnapshot { stages = new[] { first, second } }; level.generatorVersion = "shared-v2";
            Check(level.IsValid(26), id + " valid shared snapshot");
            File.WriteAllText(Output + "/" + id + "-opening.json", JsonUtility.ToJson(level, true));
            yield return new WaitForSeconds(2);
            Capture(id); yield return new WaitForSeconds(.2f);
            // Exercise actual tray flight for every model, including the new indices 23–25.
            foreach (var item in board.ToArray()) item.SetActive(false);
            var spawn = typeof(Match3DFlow).GetMethod("CreateBoardItem", BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < 26; i++) {
                spawn.Invoke(flow, new object[] { new ChallengeTile { id = 9000 + i, type = 1000 + i }, profile.prefabs[i], new Vector3(0, 25, 0), Quaternion.identity, true, false, null });
                var item = board.Last();
                Check(item.GetComponentsInChildren<Collider>().Any(c => c.enabled && c.gameObject.layer == ItemColliderGeometry.PickLayer), item.name + " pick");
                Check(item.GetComponentsInChildren<Collider>().Any(c => c.enabled && c.gameObject.layer == ItemColliderGeometry.BodyLayer), item.name + " body");
                input.AddObjectOnStack(item, false); yield return new WaitForSeconds(.8f);
                Check(input.objectsOnStack.Contains(item), names[i] + " pickup");
                Check((item.transform.position - Match3DTray.Instance.SlotPosition(0)).magnitude < .02f, names[i] + " tray position: actual=" + item.transform.position + " expected=" + Match3DTray.Instance.SlotPosition(0) + " pending=" + input.HasPendingVisuals);
                Check(item.GetComponentsInChildren<Renderer>().Any(r => r.enabled && r.sharedMaterial.shader.isSupported), names[i] + " visible supported material");
                if (i == 0) { input.UndoLast(); yield return new WaitForSeconds(.8f); input.AddObjectOnStack(item, false); yield return new WaitForSeconds(.8f); Check(input.objectsOnStack.Contains(item), id + " undo/reselect"); }
                input.ResetStack(); board.Remove(item); BoardItemPool.Recycle(item);
            }
            flow.BeginDailyPractice(0, level); yield return Ready();
            flow.NextStage(); yield return Ready();
            var restored = flow.CaptureDailyOpening();
            Check(restored.items.Length == second.items.Length && restored.items.Zip(second.items, (a, b) => a.type == b.type && Vector3.Distance(a.position, b.position) < .001f).All(v => v), id + " shared opening restore");
            flow.Home();
        }
    }
    static void Capture(string id) {
        ScreenCapture.CaptureScreenshot(Path.GetFullPath(Output + "/" + id + ".png"));
    }
}
