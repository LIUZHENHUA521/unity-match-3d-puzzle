using System.Collections.Generic;
using UnityEngine;

// Exposure only retires a probe binding; it never picks the board item or spends a chance.
public sealed class PotProbeExposure {
    public const float ConfirmSeconds = .3f, DismissSeconds = .25f;
    const float CheckInterval = .1f;
    sealed class Entry {
        public float clearSince = -1, nextCheck, dismissAge = -1;
    }
    readonly Dictionary<long, Entry> entries = new Dictionary<long, Entry>();
    readonly List<long> tokens = new List<long>();
    readonly List<long> stale = new List<long>();
    int revision = -1, cursor;
    float clock;

    public bool IsDismissing(long token) => entries.TryGetValue(token, out var entry) && entry.dismissAge >= 0;
    public float Visibility(long token) {
        if (!entries.TryGetValue(token, out var entry) || entry.dismissAge < 0) return 1;
        float t = Mathf.Clamp01(entry.dismissAge / DismissSeconds);
        return 1 - t * t * (3 - 2 * t);
    }
    public void Tick(ComboProbeController owner, float delta) {
        if (owner.ReplayMode) return;
        if (revision != owner.State.Revision) {
            revision = owner.State.Revision;
            tokens.Clear();
            for (int i = 0; i < ComboProbeState.ColumnCount; i++) tokens.AddRange(owner.State.Column(i));
            stale.Clear();
            foreach (var pair in entries) if (!owner.State.Contains(pair.Key)) stale.Add(pair.Key);
            foreach (long token in stale) entries.Remove(token);
            foreach (long token in tokens) if (!entries.ContainsKey(token)) entries.Add(token, new Entry());
        }
        if (!owner.CanInteract || (BoardCameraZoom.Instance && BoardCameraZoom.Instance.SuppressPick)) {
            ResetConfirmation();
            return;
        }
        clock += delta;
        foreach (long token in tokens) {
            var entry = entries[token];
            if (entry.dismissAge < 0) continue;
            entry.dismissAge += delta;
            if (entry.dismissAge >= DismissSeconds) owner.State.Remove(token);
        }
        // At most one five-ray geometry query per frame, and ten per second per target.
        // Round-robin includes covered lower entries, not only the four displayed tops.
        for (int i = 0; i < tokens.Count; i++) {
            cursor %= tokens.Count;
            long token = tokens[cursor++];
            var entry = entries[token];
            if (entry.dismissAge >= 0 || clock < entry.nextCheck) continue;
            entry.nextCheck = clock + CheckInterval;
            if (owner.View && owner.View.Rewards.IsPending(token)) { entry.clearSince = -1; continue; }
            var target = owner.Target(token);
            if (!target || !owner.IsExposed(target.gameObject)) entry.clearSince = -1;
            else if (entry.clearSince < 0) entry.clearSince = clock;
            else if (clock - entry.clearSince >= ConfirmSeconds) {
                entry.dismissAge = 0;
                if (owner.View && owner.View.Preview.TargetToken == token) owner.View.Preview.Clear();
                PotReplayRecorder.Event("pot_probe_exposed", PotReplayPresentation.Id(target.gameObject));
            }
            break;
        }
    }
    public PotReplayProbeDismissal[] CaptureReplay(ComboProbeController owner) {
        var result = new List<PotReplayProbeDismissal>();
        foreach (var pair in entries) if (pair.Value.dismissAge >= 0 && owner.State.Contains(pair.Key)) {
            int id = PotReplayPresentation.Id(owner.Target(pair.Key)?.gameObject);
            if (id >= 0) result.Add(new PotReplayProbeDismissal { id = id, age = pair.Value.dismissAge });
        }
        return result.ToArray();
    }
    public void ApplyReplay(PotReplayProbeDismissal[] values, Dictionary<int, GameObject> objects, float offset) {
        Reset();
        if (values == null) return; // Older recordings have no dismissal presentation.
        foreach (var value in values) if (objects.TryGetValue(value.id, out var obj)) {
            var target = obj.GetComponent<ComboProbeTarget>();
            if (target) entries[target.Token] = new Entry { dismissAge = Mathf.Max(0, value.age + offset) };
        }
    }
    public void Reset() { entries.Clear(); tokens.Clear(); stale.Clear(); revision = -1; cursor = 0; clock = 0; }
    public void ResetConfirmation() { foreach (var entry in entries.Values) entry.clearSince = -1; }
}
