using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ComboProbeController : MonoBehaviour {
    public static ComboProbeController Instance { get; private set; }
    public readonly ComboProbeState State = new ComboProbeState();
    readonly Dictionary<long, ComboProbeTarget> targets = new Dictionary<long, ComboProbeTarget>();
    readonly List<long> stale = new List<long>();
    System.Random rewardRandom;
    long operation;
    int generation;
    ComboProbeView view;
    public bool Running { get; private set; }
    public int Generation => generation;
    public ComboProbeView View => view;
    public bool CanInteract => Running && Match3DFlow.Instance && Match3DFlow.Instance.ScreenName == "game" &&
        !Match3DFlow.Instance.BlocksGameplay && !Match3DFlow.Instance.CreationInProgress && GameManager.Instance &&
        GameManager.Instance.GameIsPlaying() && !BoardTools.Busy && Time.timeScale > 0 &&
        InputController.Instance && InputController.Instance.objectsOnStack.Count < 7;

    void Awake() { Instance = this; }
    public static ComboProbeController Ensure() {
        if (!Instance && Match3DFlow.Instance) Match3DFlow.Instance.gameObject.AddComponent<ComboProbeController>();
        return Instance;
    }
    public static void BeginStage(bool enabled, int seed) {
        var owner = Ensure(); if (!owner) return;
        owner.EndStage(); owner.generation++; owner.Running = enabled;
        owner.rewardRandom = new System.Random(unchecked(seed ^ 0x2c59b371));
        if (enabled && !owner.view) {
            owner.view = new GameObject("Combo probe region").AddComponent<ComboProbeView>();
            owner.view.Initialize(owner);
        }
        if (owner.view) owner.view.gameObject.SetActive(enabled);
    }
    public static void End() { if (Instance) Instance.EndStage(); }
    void EndStage() {
        Running = false; targets.Clear(); State.Reset(); operation = 0;
        if (view) { view.ResetView(); view.gameObject.SetActive(false); }
    }
    public static void Register(GameObject obj, GameObject prefab) {
        if (!Instance || !Instance.Running || !obj) return;
        var identity = obj.GetComponent<ComboProbeTarget>() ?? obj.AddComponent<ComboProbeTarget>();
        identity.Assign(Instance.generation, prefab);
        Instance.targets[identity.Token] = identity;
    }
    public ComboProbeTarget Target(long token) {
        if (!targets.TryGetValue(token, out var target) || !target || target.Token != token ||
            target.Generation != generation || !target.gameObject.activeInHierarchy ||
            !LevelGenerator.Instance || !InputController.Instance || !LevelGenerator.Instance.objectsOnScene.Contains(target.gameObject) ||
            InputController.Instance.objectsOnStack.Contains(target.gameObject) || BoardTools.IsStored(target.gameObject)) return null;
        return target;
    }
    public void PickAccepted(GameObject obj, ProbePickSource source, bool matched) {
        if (!Running) return;
        var target = obj.GetComponent<ComboProbeTarget>();
        if (target) { State.Remove(target.Token); }
        bool reward = State.Pick(++operation, source, matched);
        if (reward) GrantReward();
        if (view) view.Refresh();
    }
    public bool GrantReward() {
        if (!Running || !Camera.main) return false;
        // Randomize the cheap eligibility list, then stop at the first occluded target.
        // Dense piles normally need one geometry query, not five raycasts for every item.
        var candidates = new List<ComboProbeTarget>();
        foreach (var entry in targets) {
            var target = Target(entry.Key);
            if (target && !State.Contains(entry.Key)) candidates.Add(target);
        }
        while (candidates.Count > 0) {
            int index = rewardRandom.Next(candidates.Count); var chosen = candidates[index];
            candidates[index] = candidates[candidates.Count - 1]; candidates.RemoveAt(candidates.Count - 1);
            if (IsOccluded(chosen.gameObject)) return State.Add(chosen.Token);
        }
        return false;
    }
    public bool IsOccluded(GameObject obj) {
        var camera = Camera.main;
        if (!camera || !obj) return false;
        var renderers = obj.GetComponentsInChildren<Renderer>(true);
        bool first = true; Bounds bounds = default;
        foreach (var r in renderers) if (r is MeshRenderer || r is SkinnedMeshRenderer) {
            if (first) { bounds = r.bounds; first = false; } else bounds.Encapsulate(r.bounds);
        }
        if (first) return false;
        var center = camera.WorldToViewportPoint(bounds.center);
        if (center.z <= 0 || center.x < 0 || center.x > 1 || center.y < 0 || center.y > 1) return false;
        // Multiple interior samples admit partially covered models; only other live board items occlude.
        Vector3[] samples = { bounds.center, bounds.center + camera.transform.right * bounds.extents.x * .5f,
            bounds.center - camera.transform.right * bounds.extents.x * .5f,
            bounds.center + camera.transform.up * bounds.extents.z * .5f,
            bounds.center - camera.transform.up * bounds.extents.z * .5f };
        foreach (var sample in samples) {
            var ray = camera.ViewportPointToRay(camera.WorldToViewportPoint(sample));
            float targetDistance = Vector3.Dot(sample - ray.origin, ray.direction);
            // The first exact visible mesh can be in front of this target, even for renderer-culled deep layers.
            var hit = InputController.Instance.PickAt(camera.WorldToScreenPoint(sample));
            if (hit && hit != obj && !BoardTools.IsStored(hit) && !InputController.Instance.objectsOnStack.Contains(hit) &&
                LevelGenerator.Instance.objectsOnScene.Contains(hit) &&
                Vector3.Dot(hit.transform.position - ray.origin, ray.direction) < targetDistance) return true;
        }
        return false;
    }
    public bool UseTop(int column, long expected) {
        if (!CanInteract || State.Top(column) != expected) return false;
        var target = Target(expected); if (!target) { RemoveInvalid(); return false; }
        int before = InputController.Instance.AcceptedPicks;
        InputController.Instance.AddObjectOnStack(target.gameObject, source: ProbePickSource.Probe);
        return InputController.Instance.AcceptedPicks > before;
    }
    void RemoveInvalid() {
        stale.Clear();
        for (int i = 0; i < 4; i++) foreach (long token in State.Column(i)) if (!Target(token)) stale.Add(token);
        foreach (long token in stale) { State.Remove(token); targets.Remove(token); }
    }
    void LateUpdate() {
        if (!Running) return;
        RemoveInvalid(); if (view) view.Refresh();
    }
    void OnDestroy() { EndStage(); if (view) Destroy(view.gameObject); if (Instance == this) Instance = null; }
}
