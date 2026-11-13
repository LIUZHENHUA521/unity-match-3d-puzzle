using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class ComboProbePreview : MonoBehaviour {
    struct Part { public Transform transform; public Mesh mesh; public SkinnedMeshRenderer skin; }
    readonly List<Part> parts = new List<Part>();
    readonly ParticleSystem.Particle[] particles = new ParticleSystem.Particle[72];
    ComboProbeController owner;
    Transform origin;
    long token;float replayTime;bool replay;Transform replayOrigin;
    Camera camera;
    CommandBuffer outline;
    Material silhouette, particleMaterial;
    ParticleSystem line;
    public bool Active => token != 0;
    public int ParticleCount => line ? line.particleCount : 0;
    public long TargetToken => token;
    public void Show(ComboProbeController controller, long id, Transform source) {
        Clear(); owner = controller;replay=owner.ReplayMode; var target = owner.Target(id);
        if (!target || (!owner.CanInteract&&!owner.ReplayMode)) return;
        token = id; origin = source; camera = Camera.main;
        if (!silhouette) silhouette = new Material(Resources.Load<Shader>("Match3D/ComboProbe/ProbeOutline"));
        foreach (var renderer in target.GetComponentsInChildren<Renderer>(true)) {
            var skin = renderer as SkinnedMeshRenderer;
            var mesh = skin ? new Mesh() : renderer.GetComponent<MeshFilter>()?.sharedMesh;
            if (mesh) parts.Add(new Part { transform = renderer.transform, mesh = mesh, skin = skin });
        }
        outline = new CommandBuffer { name = "Probe target through-pile silhouette" };
        camera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, outline);
        if (!line) {
            var go = new GameObject("Probe particle connection") { layer = 5 }; go.transform.SetParent(transform, false);
            line = go.AddComponent<ParticleSystem>(); line.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = line.main; main.playOnAwake = false; main.loop = false; main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = particles.Length; main.startSpeed = 0; main.startLifetime = 1000;
            var emission = line.emission; emission.enabled = false; var shape = line.shape; shape.enabled = false;
            particleMaterial = new Material(Resources.Load<Shader>("Match3D/ComboProbe/ProbeParticle"));
            line.GetComponent<ParticleSystemRenderer>().sharedMaterial = particleMaterial;
        }
        line.gameObject.SetActive(true); line.Play(); Tick();if(!owner.ReplayMode)PotReplayRecorder.Event("probe_preview",PotReplayPresentation.Id(target.gameObject),1);
    }
    void LateUpdate() { if (Active&&!replay) Tick(); }
    void Tick() {
        var target = owner ? owner.Target(token) : null;
        if (!target || (!owner.CanInteract&&!owner.ReplayMode) || !origin || !camera) { Clear(); return; }
        outline.Clear();
        foreach (var part in parts) {
            if (!part.transform || !part.mesh) continue;
            if (part.skin) part.skin.BakeMesh(part.mesh);
            for (int i = 0; i < part.mesh.subMeshCount; i++) outline.DrawMesh(part.mesh, part.transform.localToWorldMatrix, silhouette, i);
        }
        // Draw in a plane facing the camera: the particle ribbon remains visible through the pile.
        Vector3 start = camera.WorldToScreenPoint(origin.position);
        Vector3 end = camera.WorldToScreenPoint(target.transform.position);
        float unit = camera.pixelWidth / 720f;
        for (int i = 0; i < particles.Length; i++) {
            float t = i / (float)(particles.Length - 1);
            Vector3 point = PotProbeActivation.ConnectionPoint(start, end, t, unit, (replay?replayTime:Time.unscaledTime));
            point.z = 1.1f;
            particles[i].position = camera.ScreenToWorldPoint(point);
            particles[i].startSize = GameplayBoardLayout.WorldPerPoint(camera) * (3 + Mathf.Sin(t * 25 - (replay?replayTime:Time.unscaledTime) * 4));
            particles[i].startColor = Color.Lerp(new Color32(100, 214, 255, 240), Color.white, .5f + .5f * Mathf.Sin(t * 20 - (replay?replayTime:Time.unscaledTime) * 5));
            particles[i].startLifetime = particles[i].remainingLifetime = 1000;
        }
        line.SetParticles(particles, particles.Length);
    }
    public void CaptureReplay(PotReplayPresentation snapshot){
        if(!Active)return;snapshot.previewId=PotReplayPresentation.Id(owner.Target(token)?.gameObject);snapshot.previewOrigin=origin.position;snapshot.previewTime=Time.unscaledTime;
    }
    public void ApplyReplay(PotReplayPresentation snapshot,Dictionary<int,GameObject> objects,float offset){
        replay=true;replayTime=snapshot.previewTime+offset;
        if(snapshot.previewId<0||!objects.TryGetValue(snapshot.previewId,out var obj)){Clear();return;}
        if(!replayOrigin){replayOrigin=new GameObject("Pot replay preview origin").transform;replayOrigin.SetParent(transform);}
        replayOrigin.position=snapshot.previewOrigin;var target=obj.GetComponent<ComboProbeTarget>();
        if(token!=target.Token)Show(ComboProbeController.Instance,target.Token,replayOrigin);Tick();
    }
    public void Clear() {
        if(token!=0&&owner&&!owner.ReplayMode)PotReplayRecorder.Event("probe_preview",PotReplayPresentation.Id(owner.Target(token)?.gameObject),0);
        token = 0; origin = null;replay=false;
        if (outline != null) { if (camera) camera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, outline); outline.Release(); outline = null; }
        foreach (var part in parts) if (part.skin && part.mesh) Destroy(part.mesh); parts.Clear();
        if (line) { line.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); line.gameObject.SetActive(false); }
    }
    void OnDisable() { Clear(); }
    void OnApplicationFocus(bool focused) { if (!focused) Clear(); }
    void OnApplicationPause(bool paused) { if (paused) Clear(); }
    void OnDestroy() { Clear(); if (silhouette) Destroy(silhouette); if (particleMaterial) Destroy(particleMaterial); }
}
