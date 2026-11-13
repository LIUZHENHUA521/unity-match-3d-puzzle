using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class PotProbeActivationValidation {
    const string Key = "pot-probe-activation-validation", Output = "work/pot-probe-results";
    static readonly BindingFlags Flags = BindingFlags.Static | BindingFlags.NonPublic;
    static PotProbeActivationValidation() {
        EditorApplication.playModeStateChanged += s => {
            if (s == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Key, false))
                EditorApplication.delayCall += () => new GameObject("Pot probe validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
        };
    }
    public static void Run() {
        Directory.CreateDirectory(Output); File.WriteAllText(Output + "/checks.txt", "");
        SessionState.SetBool(Key, true); EditorSceneManager.OpenScene(Match3DBuilder.Scene); EditorApplication.isPlaying = true;
    }
    static void Check(bool condition, string message) {
        if (!condition) throw new Exception(message);
        File.AppendAllText(Output + "/checks.txt", "PASS " + message + "\n");
    }
    static object Fixture(string method, params object[] args) => typeof(ComboProbeValidation).GetMethod(method, Flags).Invoke(null, args);
    static GameObject Spawn(int type, float x) => (GameObject)Fixture("Spawn", type, new Vector3(x, 2, 0), null);
    static void Fresh() { Fixture("Start", 2); Physics.autoSimulation = false; Fixture("ClearFixture"); }
    static void Bind(GameObject item) { ComboProbeController.Instance.State.Add(item.GetComponent<ComboProbeTarget>().Token); ComboProbeController.Instance.View.Refresh(); }
    static bool Use(int column) => ComboProbeController.Instance.UseTop(column, ComboProbeController.Instance.State.Top(column));
    static void Capture(string name) {
        Directory.CreateDirectory(ComboProbeValidation.Output);
        Fixture("Capture", name, 720, 1565);
        File.Copy(ComboProbeValidation.Output + "/" + name + ".png", Output + "/" + name + ".png", true);
    }
    static IEnumerator Guard() {
        float deadline = Time.realtimeSinceStartup + 60;
        while ((!Match3DFlow.Instance || !Match3DFlow.Instance.StartupComplete) && Time.realtimeSinceStartup < deadline) yield return null;
        var rt = new RenderTexture(720, 1565, 24); rt.Create(); Camera.main.targetTexture = rt; Camera.main.aspect = 720f / 1565;
        string error = null; var steps = Core();
        while (true) {
            bool more = false; try { more = steps.MoveNext(); } catch (Exception e) { error = e.ToString(); }
            if (!more || error != null) break; yield return steps.Current;
        }
        Time.timeScale = 1; Match3DFlow.Instance.Home();
        File.AppendAllText(Output + "/checks.txt", error == null ? "COMPLETE\n" : "FAIL " + error + "\n");
        SessionState.SetBool(Key, false); EditorApplication.Exit(error == null ? 0 : 1);
    }
    static IEnumerator Core() {
        Fresh(); yield return null;
        var owner = ComboProbeController.Instance; var input = InputController.Instance;
        var a = Spawn(2100, -1); var b = Spawn(2101, 1); Bind(a); Bind(b);
        var aStart = a.transform.position; var bStart = b.transform.position;
        Check(Use(0), "first transmission accepted");
        yield return new WaitForSeconds(.12f);
        Check(Use(1), "second transmission accepted independently");
        input.AddObjectOnStack(Spawn(2102, 2));
        Check(PotProbeActivation.ActiveCount == 2 && Vector3.Distance(a.transform.position, aStart) < .001f, "normal pick repositions slots without cancelling two transmissions");
        yield return new WaitForSeconds(.48f);
        Check(a.transform.position.y > aStart.y && Vector3.Distance(b.transform.position, bStart) < .001f,
            "reordering retains original per-pick clocks: first lifts while second activates");
        Check(ProbeLiftContact.ActiveCount == 1, "only first pickup owns an emergence contact");
        yield return new WaitForSeconds(.9f);
        Check(input.arrivedObject.Contains(a) && input.arrivedObject.Contains(b) && !input.IsBusy && PotProbeActivation.ActiveCount == 0, "concurrent pickups arrive and release effects");

        Fresh(); yield return null;
        var first = Spawn(2200, -1); var second = Spawn(2200, 0); var third = Spawn(2200, 1);
        input.AddObjectOnStack(first); input.AddObjectOnStack(second); yield return new WaitForSeconds(.35f);
        Bind(third); var thirdStart = third.transform.position;
        Check(Use(0), "probe completes triple");
        Check(!owner.Target(third.GetComponent<ComboProbeTarget>().Token) && PotProbeActivation.ActiveCount == 1, "effect survives immediate removal of matched target from logical board");
        yield return new WaitForSeconds(.18f);
        Check(PotProbeActivation.ActiveCount == 1 && Vector3.Distance(third.transform.position, thirdStart) < .001f && input.HasPendingVisuals,
            "triple waits for transmitted light before moving");
        yield return new WaitForSeconds(.62f);
        Check(first.activeSelf && second.activeSelf && third.activeSelf && input.HasPendingVisuals, "triple does not recycle any object before probe arrival");
        yield return new WaitForSeconds(.9f);
        Check(!input.IsBusy && PotProbeActivation.ActiveCount == 0 && ProbeLiftContact.ActiveCount == 0, "probe triple completes without stale visuals");

        Fresh(); yield return null;
        var cancelled = Spawn(2300, 0); Bind(cancelled); Check(Use(0), "cancellation fixture starts");
        yield return new WaitForSeconds(.1f);
        Check(BoardTools.RunPaid(0, () => input.MoveSourceItemsToStore()), "move tool accepts during transmission");
        Check(PotProbeActivation.ActiveCount == 0 && ProbeLiftContact.ActiveCount == 0, "moving to storage immediately clears transmission");
        yield return new WaitForSeconds(1.1f);
        Check(BoardTools.IsStored(cancelled) && !input.HasPendingVisuals, "cancelled transmission never resumes lift from storage");

        Fresh(); yield return null;
        var recycled = Spawn(2400, 0); Bind(recycled); Check(Use(0), "pooled identity fixture starts");
        recycled.GetComponent<ComboProbeTarget>().Assign(owner.Generation, recycled);
        yield return null; yield return null;
        Check(PotProbeActivation.ActiveCount == 0, "pool token reassignment cancels old effect");
        Fresh(); yield return null;
        var restarting = Spawn(2401, 0); Bind(restarting); Check(Use(0), "restart fixture starts");
        Fixture("Start", 2);
        Check(PotProbeActivation.ActiveCount == 0 && ProbeLiftContact.ActiveCount == 0, "restart immediately clears activation resources");
        yield return new WaitForSeconds(1.2f);
        Check(input.objectsOnStack.Count == 0 && !input.HasPendingVisuals, "restart prevents delayed pickups in new stage");

        // Full toy pile captures using the real reward selection and source card.
        Physics.autoSimulation = true; Fixture("Start", 2); yield return new WaitForSeconds(3);
        Physics.autoSimulation = false;
        Check(owner.GrantReward(), "real pile supplies an occluded probe target");
        yield return new WaitForSeconds(.8f);
        int column = Enumerable.Range(0, 4).First(i => owner.State.Top(i) != 0);
        Capture("pot-probe-before"); Check(Use(column), "real pile probe accepted");
        yield return new WaitForSeconds(.21f); Capture("pot-probe-light");
        yield return new WaitForSeconds(.22f); Capture("pot-probe-awaken");
        yield return new WaitForSeconds(.18f); Capture("pot-probe-lift");
        yield return new WaitForSeconds(.62f); Capture("pot-probe-landed");
        Check(!input.IsBusy && PotProbeActivation.ActiveCount == 0, "real pile activation lands normally");
    }
}
