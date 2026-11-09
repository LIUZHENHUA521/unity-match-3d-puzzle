using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Exercises the actual production item flight/merge and all kitchen model adapters in Play mode.
public static class TastyKitchenValidation {
    static int checks;
    static bool previousInputEnabled;
    const string Output = "work/tasty-kitchen";
    static void Check(bool ok, string why) { if (!ok) throw new Exception(why); checks++; }
    public static void Start() {
        if (!Application.isPlaying) throw new InvalidOperationException("Enter Play mode first.");
        new GameObject("Kitchen theme validation").AddComponent<BoardValidationDriver>().StartCoroutine(Run());
    }
    static IEnumerator Run() {
        checks = 0;
        previousInputEnabled = InputController.Instance.enabled;
        InputController.Instance.enabled = false;
        var steps = Core(); string error = null;
        while (true) {
            bool next = false;
            try { next = steps.MoveNext(); } catch (Exception e) { error = e.ToString(); }
            if (!next || error != null) break;
            yield return steps.Current;
        }
        (steps as IDisposable)?.Dispose();
        InputController.Instance.enabled = previousInputEnabled;
        File.WriteAllText(Output + "/validation.txt", (error == null ? "PASS" : "FAIL") + ": " + checks + " assertions; " + (error ?? "23 kitchen model models, materials, physical/pick colliders, first-stage triple clears, second-stage 300-item pile, all 23 tray pickups and undo/reselect.") + "\n");
        UnityEngine.Object.Destroy(GameObject.Find("Kitchen theme validation"));
    }
    static IEnumerator Core() {
        float startupDeadline = Time.realtimeSinceStartup + 60;
        while ((!Match3DInvitations.Instance || !Match3DTray.Instance) && Time.realtimeSinceStartup < startupDeadline) yield return null;
        Check(Match3DInvitations.Instance && Match3DTray.Instance, "startup initialized");
        var flow = Match3DFlow.Instance;
        flow.Home(); flow.BeginReward(0, "bachelor");
        float deadline = Time.realtimeSinceStartup + 60;
        while (flow.CreationInProgress && Time.realtimeSinceStartup < deadline) yield return null;
        Check(!flow.CreationInProgress && flow.Stage == 1, "first stage ready");
        var profile = LevelLayoutConfig.Get("CountrysideStove");
        Check(profile.prefabs.Length == 23 && profile.prefabs.All(p => p.name.StartsWith("Kitchen_")), "exactly 23 kitchen models");
        foreach (var prefab in profile.prefabs) {
            Check(ThemeItemsEditorService.CheckPrefab(prefab) == null, "valid adapter " + prefab.name);
            Check(prefab.GetComponentsInChildren<Renderer>(true).All(r => r.sharedMaterials.All(m => m && m.shader && m.shader.isSupported)), "supported materials " + prefab.name);
            Check(!prefab.GetComponentsInChildren<MeshCollider>(true).Any(), "no legacy mesh collider " + prefab.name);
        }
        yield return new WaitForSeconds(3);
        ScreenCapture.CaptureScreenshot(Path.GetFullPath(Output + "/first-stage.png"));
        yield return null;
        var board = LevelGenerator.Instance.objectsOnScene;
        Check(board.Count == 15, "15 first-stage objects; actual=" + board.Count);
        var triples = board.GroupBy(o => o.name).SelectMany(g => g.Select((o, i) => new { o, i }).GroupBy(x => x.i / 3).Select(g2 => g2.Select(x => x.o).ToArray())).ToArray();
        var input = InputController.Instance;
        foreach (var triple in triples) {
            foreach (var item in triple) input.AddObjectOnStack(item);
            yield return new WaitForSeconds(1);
            Check(input.objectsOnStack.Count == 0, "triple clears");
        }
        deadline = Time.realtimeSinceStartup + 60;
        while (flow.CreationInProgress && Time.realtimeSinceStartup < deadline) yield return null;
        Check(!flow.CreationInProgress && flow.Stage == 2 && flow.InitialCount == 300, "300-item second stage");
        yield return new WaitForSeconds(6);
        ScreenCapture.CaptureScreenshot(Path.GetFullPath(Output + "/second-stage.png"));
        yield return null;
        Check(board.All(o => o.GetComponent<BoardItemIdentity>() && o.GetComponent<BoardItemIdentity>().LogicalId.StartsWith("Kitchen_")), "second stage contains only kitchen models");
        foreach (var item in board.ToArray()) item.SetActive(false);
        var spawn = typeof(Match3DFlow).GetMethod("CreateBoardItem", BindingFlags.Instance | BindingFlags.NonPublic);
        for (int i = 0; i < profile.prefabs.Length; i++) {
            spawn.Invoke(flow, new object[] { new ChallengeTile { id = 9000 + i, type = 1000 + i }, profile.prefabs[i], new Vector3(0, 25, 0), Quaternion.Euler(32, 41, 18), true, false });
            var item = board.Last();
            Check(item.GetComponentsInChildren<Collider>().Any(c => c.enabled && c.gameObject.layer == ItemColliderGeometry.PickLayer), "pick collider " + profile.prefabs[i].name);
            Check(item.GetComponentsInChildren<Collider>().Any(c => c.enabled && c.gameObject.layer == ItemColliderGeometry.BodyLayer), "body collider " + profile.prefabs[i].name);
            var boardScale = item.transform.localScale;
            input.AddObjectOnStack(item, false);
            yield return new WaitForSeconds(.5f);
            Check(input.objectsOnStack.Contains(item), "pickup " + profile.prefabs[i].name);
            var binding = Match3DTrayDisplayBinding.For(item);
            Check(binding && (item.transform.position - Match3DTray.Instance.SlotPosition(0)).magnitude < .02f, "tray placement " + profile.prefabs[i].name);
            Check(item.transform.localScale.x > 0 && item.GetComponentsInChildren<Renderer>().Any(r => r.enabled), "visible tray kitchen model");
            if (i == 0) {
                ScreenCapture.CaptureScreenshot(Path.GetFullPath(Output + "/tray.png"));
                yield return null;
                input.UndoLast(); yield return new WaitForSeconds(.5f);
                Check(Vector3.Distance(item.transform.localScale, boardScale) < .002f, "undo board scale");
                input.AddObjectOnStack(item, false); yield return new WaitForSeconds(.5f);
                Check(input.objectsOnStack.Contains(item), "undo reselect");
            }
            input.ResetStack(); board.Remove(item); BoardItemPool.Recycle(item);
        }
        flow.RestartStage();
        deadline = Time.realtimeSinceStartup + 60;
        while (flow.CreationInProgress && Time.realtimeSinceStartup < deadline) yield return null;
        Check(!flow.CreationInProgress && flow.Stage == 2, "retry ready");
        yield return new WaitForSeconds(5);
        ScreenCapture.CaptureScreenshot(Path.GetFullPath(Output + "/second-stage.png"));
        yield return null;
    }
}
