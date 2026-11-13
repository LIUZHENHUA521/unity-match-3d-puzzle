using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// One independent effect per accepted pickup. It must survive removal from the
// board/tray lists when that pickup completes a triple.
public sealed class PotProbeActivation : MonoBehaviour {
    public const float SourceSeconds = .08f, TravelSeconds = .28f, ActivateSeconds = .20f;
    public const float HitSeconds = SourceSeconds + TravelSeconds;
    public const float Duration = HitSeconds + ActivateSeconds;
    static readonly Dictionary<GameObject, PotProbeActivation> active = new Dictionary<GameObject, PotProbeActivation>();
    struct Part { public Transform transform; public Mesh mesh; public SkinnedMeshRenderer skin; }
    readonly List<Part> parts = new List<Part>();
    readonly ParticleSystem.Particle[] particles = new ParticleSystem.Particle[128];
    GameObject item;
    ComboProbeTarget identity;
    long token;
    int generation;
    float started;
    Vector3 origin;
    Camera camera;
    CommandBuffer outline;
    Material silhouette, particleMaterial;
    ParticleSystem ribbon;
    int count;
    public static int ActiveCount => active.Count;
    public float Elapsed => Mathf.Max(0, Time.time - started);

    public static Vector3 ConnectionPoint(Vector3 start, Vector3 end, float t, float unit, float seconds) {
        var delta = end - start;
        var side = new Vector3(-delta.y, delta.x, 0).normalized;
        return Vector3.Lerp(start, end, t) + side * (Mathf.Sin(t * Mathf.PI) * 25 * unit +
            Mathf.Sin(t * 15 - seconds * 3) * 2 * unit * Mathf.Sin(t * Mathf.PI));
    }
    public static float ActivationScale(float seconds) => 1 + .14f * Mathf.Sin(Mathf.Clamp01((seconds - HitSeconds) / ActivateSeconds) * Mathf.PI);

    public static void Begin(GameObject target, Vector3 source) {
        Stop(target);
        if (!target || !Camera.main) return;
        var outlineShader = Resources.Load<Shader>("Match3D/ComboProbe/ProbeOutline");
        var particleShader = Resources.Load<Shader>("Match3D/ComboProbe/ProbeParticle");
        if (!outlineShader || !particleShader) return;
        var effect = new GameObject("Pot probe activation").AddComponent<PotProbeActivation>();
        effect.item = target; effect.origin = source; effect.started = Time.time; effect.camera = Camera.main;
        effect.identity = target.GetComponent<ComboProbeTarget>();
        if (effect.identity) { effect.token = effect.identity.Token; effect.generation = effect.identity.Generation; }
        effect.silhouette = new Material(outlineShader);
        effect.particleMaterial = new Material(particleShader);
        foreach (var renderer in target.GetComponentsInChildren<Renderer>()) {
            if (!renderer.enabled) continue;
            var skin = renderer as SkinnedMeshRenderer;
            var mesh = skin ? new Mesh() : renderer.GetComponent<MeshFilter>()?.sharedMesh;
            if (mesh) effect.parts.Add(new Part { transform = renderer.transform, mesh = mesh, skin = skin });
        }
        effect.outline = new CommandBuffer { name = "Pot probe activation silhouette" };
        effect.camera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, effect.outline);
        effect.gameObject.layer = 5;
        effect.ribbon = effect.gameObject.AddComponent<ParticleSystem>();
        effect.ribbon.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = effect.ribbon.main;
        main.playOnAwake = false; main.loop = false; main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = effect.particles.Length; main.startSpeed = 0; main.startLifetime = 1000;
        var emission = effect.ribbon.emission; emission.enabled = false;
        var shape = effect.ribbon.shape; shape.enabled = false;
        effect.ribbon.GetComponent<ParticleSystemRenderer>().sharedMaterial = effect.particleMaterial;
        effect.ribbon.Play(); active[target] = effect; effect.Tick();
    }
    void LateUpdate() { Tick(); }
    void Dot(Vector3 screen, float size, float alpha, bool white = false) {
        if (count >= particles.Length) return;
        screen.z = 1.1f;
        particles[count++] = new ParticleSystem.Particle {
            position = camera.ScreenToWorldPoint(screen),
            startSize = GameplayBoardLayout.WorldPerPoint(camera) * size,
            startColor = white ? new Color(1, 1, 1, alpha) : new Color(.3f, .85f, 1, alpha),
            startLifetime = 1000, remainingLifetime = 1000
        };
    }
    void Tick() {
        if (!item || !item.activeInHierarchy || !camera ||
            (identity && (identity.Token != token || identity.Generation != generation)) ||
            (ComboProbeController.Instance && (!ComboProbeController.Instance.Running ||
                (identity && ComboProbeController.Instance.Generation != generation)))) { Finish(); return; }
        float elapsed = Elapsed;
        // Keep a brief fading outline during emergence, then leave the normal mesh.
        if (elapsed >= Duration + .12f) { Finish(); return; }
        float activation = Mathf.Clamp01((elapsed - HitSeconds) / ActivateSeconds);
        float fade = 1 - Mathf.Clamp01((elapsed - Duration) / .12f);
        silhouette.SetFloat("_PotOpacity", (elapsed < HitSeconds ? .15f : .7f + .3f * Mathf.Sin(activation * Mathf.PI)) * fade);
        outline.Clear();
        foreach (var part in parts) {
            if (!part.transform || !part.mesh) continue;
            if (part.skin) part.skin.BakeMesh(part.mesh);
            for (int i = 0; i < part.mesh.subMeshCount; i++) outline.DrawMesh(part.mesh, part.transform.localToWorldMatrix, silhouette, i);
        }
        var start = camera.WorldToScreenPoint(origin);
        var end = camera.WorldToScreenPoint(item.transform.position);
        float unit = camera.pixelWidth / 720f;
        count = 0;
        if (elapsed < Duration) {
            float lineAlpha = elapsed < HitSeconds ? .42f : .42f * (1 - activation);
            for (int i = 0; i < 48; i++) Dot(ConnectionPoint(start, end, i / 47f, unit, elapsed), 3, lineAlpha);
        }
        if (elapsed < SourceSeconds) {
            float pulse = Mathf.Sin(elapsed / SourceSeconds * Mathf.PI);
            Dot(start, 34 + pulse * 22, .65f); Dot(start, 12, 1, true);
        } else if (elapsed < HitSeconds) {
            float progress = (elapsed - SourceSeconds) / TravelSeconds;
            for (int i = 0; i < 18; i++) {
                float tail = i / 17f;
                Dot(ConnectionPoint(start, end, Mathf.Max(0, progress - .18f * (1 - tail)), unit, elapsed), 4 + tail * 9, tail * .85f);
            }
            var head = ConnectionPoint(start, end, progress, unit, elapsed);
            Dot(head, 24, .45f); Dot(head, 10, 1, true);
        } else if (elapsed < Duration) {
            for (int i = 0; i < 28; i++) {
                float angle = i * Mathf.PI * 2 / 28;
                Dot(end + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * (20 + activation * 42) * unit,
                    6 * (1 - activation) + 2, .9f * (1 - activation));
            }
            Dot(end, 28, .6f * (1 - activation), true);
        }
        ribbon.SetParticles(particles, count);
    }
    void Finish() { Release(); Destroy(gameObject); }
    void Release() {
        if (!ReferenceEquals(item, null) && active.TryGetValue(item, out var current) && current == this) active.Remove(item);
        if (outline != null) { if (camera) camera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, outline); outline.Release(); outline = null; }
        if (ribbon) { ribbon.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); ribbon.gameObject.SetActive(false); }
        foreach (var part in parts) if (part.skin && part.mesh) Destroy(part.mesh);
        parts.Clear();
        if (silhouette) Destroy(silhouette); silhouette = null;
        if (particleMaterial) Destroy(particleMaterial); particleMaterial = null;
    }
    public static void Stop(GameObject target) {
        if (!ReferenceEquals(target, null) && active.TryGetValue(target, out var effect) && effect) effect.Finish();
    }
    public static void Clear() { foreach (var effect in new List<PotProbeActivation>(active.Values)) if (effect) effect.Finish(); active.Clear(); }
    void OnDestroy() { Release(); }
}
