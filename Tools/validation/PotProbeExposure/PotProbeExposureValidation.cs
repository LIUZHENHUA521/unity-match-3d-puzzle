using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class PotProbeExposureValidation {
    const string Key = "pot.probe.exposure.validation", Output = "work/pot-probe-exposure";
    static readonly BindingFlags Flags = BindingFlags.Static | BindingFlags.NonPublic;
    static PotProbeExposureValidation() {
        EditorApplication.playModeStateChanged += state => {
            if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Key, false))
                EditorApplication.delayCall += () => new GameObject("Pot exposure validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
        };
    }
    public static void Run() {
        Directory.CreateDirectory(Output); File.WriteAllText(Output + "/checks.txt", "");
        SessionState.SetBool(Key, true); EditorSceneManager.OpenScene(Match3DBuilder.Scene); EditorApplication.isPlaying = true;
    }
    static void Check(bool value, string message) {
        if (!value) throw new Exception(message);
        File.AppendAllText(Output + "/checks.txt", "PASS " + message + "\n");
    }
    static object Fixture(string method, params object[] args) => typeof(ComboProbeValidation).GetMethod(method, Flags).Invoke(null, args);
    static GameObject Cube(int id, float depth, float size) {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube); obj.name = "Pot exposure fixture " + id;
        obj.tag = "Collectable"; obj.layer = ItemColliderGeometry.PickLayer;
        obj.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(.5f, .6f, depth));
        obj.transform.rotation = Camera.main.transform.rotation; obj.transform.localScale = Vector3.one * size;
        var body = obj.AddComponent<Rigidbody>(); body.useGravity = false;
        obj.AddComponent<Match3DTile>().id = id;
        ItemPhysicsController.Instance.Register(body); body.isKinematic = true;
        LevelGenerator.Instance.objectsOnScene.Add(obj); ComboProbeController.Register(obj, obj);
        return obj;
    }
    static long Token(GameObject obj) => obj.GetComponent<ComboProbeTarget>().Token;
    static void Tick(ComboProbeController owner, float delta) { Physics.SyncTransforms(); owner.Exposure.Tick(owner, delta); owner.View.SendMessage("LateUpdate"); }
    static void Capture(string name) {
        Directory.CreateDirectory(ComboProbeValidation.Output);
        float speed = Time.timeScale; Time.timeScale = 0;
        try { Fixture("Capture", name, 720, 1565); } finally { Time.timeScale = speed; }
        File.Copy(ComboProbeValidation.Output + "/" + name + ".png", Output + "/" + name + ".png", true);
    }
    static IEnumerator Guard() {
        float deadline = Time.realtimeSinceStartup + 90;
        while ((!Match3DFlow.Instance || !Match3DFlow.Instance.StartupComplete) && Time.realtimeSinceStartup < deadline) yield return null;
        string error = null; var core = Core();
        while (true) { bool more = false; try { more = core.MoveNext(); } catch (Exception e) { error = e.ToString(); } if (!more || error != null) break; yield return core.Current; }
        Time.timeScale = 1;
        File.AppendAllText(Output + "/checks.txt", error == null ? "COMPLETE\n" : "FAIL " + error + "\n");
        SessionState.SetBool(Key, false); EditorApplication.Exit(error == null ? 0 : 1);
    }
    static IEnumerator Core() {
        foreach (var service in new MonoBehaviour[] { Match3DGameRecords.Instance, Match3DInvitations.Instance, PotReplayService.Instance })
            if (service) { service.StopAllCoroutines(); service.enabled = false; }
        var rt = new RenderTexture(720, 1565, 24); rt.Create(); Camera.main.targetTexture = rt; Camera.main.aspect = 720f / 1565;
        Fixture("Start", 2); Physics.autoSimulation = false; Fixture("ClearFixture");
        var owner = ComboProbeController.Instance; owner.enabled = false;
        yield return null;
        var bottom = Cube(29001, 11, .8f); var top = Cube(29002, 10, 1f); var cover = Cube(29003, 8, 2f);
        long token = Token(top), lower = Token(bottom);
        owner.State.RestoreReplay(2, 3, 1, new IEnumerable<long>[] { new[] { lower, token }, new long[0], new long[0], new long[0] });
        Tick(owner, .1f);
        Check(owner.IsOccluded(top) && !owner.IsExposed(top), "real mesh raycasts identify a covered target");
        for (int i = 0; i < 12; i++) Tick(owner, .1f);
        Check(!owner.Exposure.IsDismissing(token) && owner.State.Count == 2, "covered bindings survive well beyond the confirmation window");
        var coverPosition = cover.transform.position; cover.transform.position += Camera.main.transform.right * 10;
        Tick(owner, .1f); Tick(owner, .1f);
        Check(owner.IsExposed(top) && !owner.Exposure.IsDismissing(token), "exposed target is pickable but does not disappear immediately");
        cover.transform.position = coverPosition; Tick(owner, .1f); Tick(owner, .1f);
        cover.transform.position += Camera.main.transform.right * 10; Tick(owner, .1f); Tick(owner, .1f);
        Check(!owner.Exposure.IsDismissing(token), "brief re-occlusion resets the confirmation timer");
        Time.timeScale = 0; Tick(owner, .5f); Time.timeScale = 1;
        Tick(owner, .1f); Tick(owner, .1f);
        Check(!owner.Exposure.IsDismissing(token), "pause invalidates partial exposure confirmation");
        owner.View.Preview.Show(owner, token, owner.View.Slot(0).transform);
        for (int i = 0; i < 5 && !owner.Exposure.IsDismissing(token); i++) Tick(owner, .1f);
        Check(owner.Exposure.IsDismissing(token) && owner.State.Contains(token), "confirmed exposure starts animation before binding removal");
        Check(!owner.View.Preview.Active && !owner.UseTop(0, token), "dismissal clears the connection and rejects stale probe clicks");
        Capture("pot-exposure-before");
        Tick(owner, .125f);
        var image = owner.View.Slot(0).GetComponentsInChildren<RawImage>().Last();
        Check(image.color.a > .1f && image.color.a < .9f && image.rectTransform.localScale.x > .1f && image.rectTransform.localScale.x < .9f,
            "production model fades and shrinks halfway through dismissal");
        Capture("pot-exposure-half");
        var snapshot = PotReplayPresentation.Capture();
        var objects = new Dictionary<int, GameObject> { { 29001, bottom }, { 29002, top }, { 29003, cover } };
        float half = owner.Exposure.Visibility(token);
        Time.timeScale = 0; Tick(owner, .5f);
        Check(Mathf.Abs(owner.Exposure.Visibility(token) - half) < .0001f, "pause freezes the retirement animation");
        Time.timeScale = 1; Tick(owner, .13f);
        Check(!owner.State.Contains(token) && owner.State.Top(0) == lower, "completion removes only the exposed entry and promotes the next model");
        Check(owner.State.Combo == 2 && owner.State.Stars == 3 && owner.State.NextColumn == 1, "automatic cleanup preserves combo, chances and reward distribution");
        Check(owner.Target(token) && top.activeInHierarchy && !InputController.Instance.objectsOnStack.Contains(top), "actual board item remains in the pot");
        Check(!owner.Exposure.IsDismissing(lower), "still-covered lower model stays bound");
        Capture("pot-exposure-after");
        owner.ApplyReplay(JsonUtility.FromJson<PotReplayPresentation>(JsonUtility.ToJson(snapshot)), objects, 0);
        Check(Mathf.Abs(owner.Exposure.Visibility(token) - half) < .0001f && owner.State.Top(0) == token, "replay seek restores dismissal age and stack before promotion");
        owner.ApplyReplay(snapshot, objects, .05f);
        Check(owner.Exposure.Visibility(token) < half, "replay time offset advances the fade");
        snapshot.probeDismissals = null; owner.ApplyReplay(snapshot, objects, 0);
        Check(owner.Exposure.Visibility(token) == 1, "old recordings without dismissal data retain their original presentation");
        ComboProbeController.BeginStage(true, 123); owner.enabled = false;
        // Re-register because stage changes invalidate pool identities.
        ComboProbeController.Register(top, top); token = Token(top); owner.State.Add(token);
        top.transform.position += Camera.main.transform.right * 100;
        for (int i = 0; i < 10; i++) Tick(owner, .1f);
        Check(!owner.IsExposed(top) && owner.State.Contains(token), "off-screen target is not treated as exposed");
        top.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(.5f, .6f, 10));
        owner.View.Rewards.Play(0, token);
        for (int i = 0; i < 10; i++) Tick(owner, .1f);
        Check(owner.View.Rewards.IsPending(token) && !owner.Exposure.IsDismissing(token), "in-flight reward waits to land before exposure confirmation");
        owner.View.Rewards.ResetImmediate(); owner.enabled = true;
        yield return new WaitForSeconds(.9f);
        Check(!owner.State.Contains(token) && owner.Target(token), "normal controller update automatically retires an exposed binding");
        ComboProbeController.End(); Check(owner.Exposure.Visibility(token) == 1 && owner.State.Count == 0, "stage end clears retirement state");
    }
}
