using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ComboProbeController : MonoBehaviour {
    public static ComboProbeController Instance { get; private set; }
    public readonly ComboProbeState State = new ComboProbeState();
    public readonly PotProbeExposure Exposure = new PotProbeExposure();
    readonly Dictionary<long, ComboProbeTarget> targets = new Dictionary<long, ComboProbeTarget>();
    readonly List<long> stale = new List<long>();
    System.Random rewardRandom;
    long operation;
    int generation;bool beforeReplayUsed;
    ComboProbeView view;
    public bool Running { get; private set; }
    // Remember successful use for this session, including across stage restarts.
    public bool HasUsedProbe { get; private set; }
    public int Generation => generation;
    public ComboProbeView View => view;
    public bool ReplayMode { get; private set; }
    public bool CanInteract => !ReplayMode && Running && Match3DFlow.Instance && Match3DFlow.Instance.ScreenName == "game" &&
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
        if (owner.view) {
            owner.view.gameObject.SetActive(enabled);
            owner.view.Praise.BeginStage(enabled ? Match3DABExperiments.GetABGroup(PotComboPraise.TestKey) : null);
        }
    }
    public static void End() { if (Instance) Instance.EndStage(); }
    void EndStage() {
        PotProbeActivation.Clear(); Exposure.Reset();
        if(ReplayMode)HasUsedProbe=beforeReplayUsed;ReplayMode=false;Running = false; targets.Clear(); State.Reset(); operation = 0;
        if (view) { view.ResetView(); view.gameObject.SetActive(false); }
    }
    public static void Register(GameObject obj, GameObject prefab) {
        if (!Instance || !Instance.Running || !obj) return;
        var identity = obj.GetComponent<ComboProbeTarget>() ?? obj.AddComponent<ComboProbeTarget>();
        identity.Assign(Instance.generation, prefab);
        Instance.targets[identity.Token] = identity;
    }
    public ComboProbeTarget Target(long token) {
        if(ReplayMode)return targets.TryGetValue(token,out var recorded)?recorded:null;
        if (!targets.TryGetValue(token, out var target) || !target || target.Token != token ||
            target.Generation != generation || !target.gameObject.activeInHierarchy ||
            !LevelGenerator.Instance || !InputController.Instance || !LevelGenerator.Instance.objectsOnScene.Contains(target.gameObject) ||
            InputController.Instance.objectsOnStack.Contains(target.gameObject) || BoardTools.IsStored(target.gameObject)) return null;
        return target;
    }
    public void PickAccepted(GameObject obj, ProbePickSource source, bool matched) {
        if (!Running || ReplayMode) return;
        var target = obj.GetComponent<ComboProbeTarget>();
        if (target) { State.Remove(target.Token); }
        int previousCombo = State.Combo;
        bool reward = State.Pick(++operation, source, matched);
        if (reward) GrantReward();
        if (view) {
            view.Refresh();
            if (State.Combo > previousCombo) view.Praise.ShowCombo(State.Combo);
            else if (State.Combo == 0) view.Praise.ResetImmediate();
        }
    }
    public bool GrantReward() {
        if (!Running || ReplayMode || !Camera.main) return false;
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
            if (IsOccluded(chosen.gameObject)) {
                int column = State.NextColumn;
                bool added = State.Add(chosen.Token);
                if(added)PotReplayRecorder.Event("probe_generate",PotReplayPresentation.Id(chosen.gameObject),column);
                if (added && view) view.Rewards.Play(column, chosen.Token);
                return added;
            }
        }
        return false;
    }
    public bool IsOccluded(GameObject obj) => CheckOcclusion(obj, out _);
    public bool IsExposed(GameObject obj) => !CheckOcclusion(obj, out bool pickable) && pickable;
    bool CheckOcclusion(GameObject obj, out bool pickable) {
        pickable = false;
        var camera = Camera.main;
        if (!camera || !obj || !InputController.Instance || !LevelGenerator.Instance) return false;
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
            if (hit == obj) pickable = true;
            if (hit && hit != obj && !BoardTools.IsStored(hit) && !InputController.Instance.objectsOnStack.Contains(hit) &&
                LevelGenerator.Instance.objectsOnScene.Contains(hit) &&
                Vector3.Dot(hit.transform.position - ray.origin, ray.direction) < targetDistance) return true;
        }
        return false;
    }
    public bool UseTop(int column, long expected) {
        if (!CanInteract || Exposure.IsDismissing(expected) || State.Top(column) != expected || (view && view.Rewards.IsPending(expected))) return false;
        var target = Target(expected); if (!target) { RemoveInvalid(); return false; }
        int before = InputController.Instance.AcceptedPicks;
        InputController.Instance.AddObjectOnStack(target.gameObject, source: ProbePickSource.Probe,
            potProbeOrigin: view ? (Vector3?)view.Slot(column).transform.position : null);
        bool accepted = InputController.Instance.AcceptedPicks > before;
        if (accepted) { PotReplayRecorder.Event("probe_use",PotReplayPresentation.Id(target.gameObject),column);HasUsedProbe = true; if (view) view.Refresh(); }
        return accepted;
    }
    public void ApplyReplay(PotReplayPresentation snapshot,Dictionary<int,GameObject> objects,float offset){
        if(!ReplayMode)beforeReplayUsed=HasUsedProbe;ReplayMode=true;Running=snapshot!=null&&snapshot.probe;
        if(view)view.gameObject.SetActive(Running);if(!Running)return;
        targets.Clear();foreach(var pair in objects){var target=pair.Value.GetComponent<ComboProbeTarget>();if(target)targets[target.Token]=target;}
        State.RestoreReplay(snapshot.combo,snapshot.stars,snapshot.nextColumn,snapshot.columns.Select(c=>(IEnumerable<long>)c.ids.Where(objects.ContainsKey).Select(id=>objects[id].GetComponent<ComboProbeTarget>().Token).ToArray()).ToArray());
        Exposure.ApplyReplay(snapshot.probeDismissals,objects,offset);
        HasUsedProbe=snapshot.used;if(view)view.ApplyReplay(snapshot,objects,offset);
    }
    void RemoveInvalid() {
        stale.Clear();
        for (int i = 0; i < 4; i++) foreach (long token in State.Column(i)) if (!Target(token)) stale.Add(token);
        foreach (long token in stale) { State.Remove(token); targets.Remove(token); }
    }
    void LateUpdate() {
        if (!Running || ReplayMode) return;
        RemoveInvalid(); Exposure.Tick(this, Time.deltaTime); if (view) view.Refresh();
    }
    void OnDestroy() { EndStage(); if (view) Destroy(view.gameObject); if (Instance == this) Instance = null; }
    void OnApplicationFocus(bool focused) { if (!focused) Exposure.ResetConfirmation(); }
    void OnApplicationPause(bool paused) { if (paused) Exposure.ResetConfirmation(); }
}
