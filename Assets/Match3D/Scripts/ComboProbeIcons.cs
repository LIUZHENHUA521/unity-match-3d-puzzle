using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Render actual model geometry once per prefab. No collider or gameplay component is cloned.
public sealed class ComboProbeIcons {
    readonly Dictionary<GameObject, Texture2D> cache = new Dictionary<GameObject, Texture2D>();
    public Texture For(ComboProbeTarget target) {
        if (!target) return null;
        var key = target.Prefab ? target.Prefab : target.gameObject;
        if (cache.TryGetValue(key, out var cached)) return cached;
        var root = new GameObject("Probe model thumbnail") { layer = 30 };
        var meshes = new List<Mesh>();
        var previous = RenderTexture.active;
        Camera camera = null; GameObject lightObject = null;
        var texture = new RenderTexture(144, 144, 24, RenderTextureFormat.ARGB32) { name = "Probe " + key.name, antiAliasing = 1 };
        try {
            // Copy the prefab's visual hierarchy only. A board instance can be upside down,
            // scaled by gameplay or moving; none of that belongs in a tray-style thumbnail.
            var transforms = new Dictionary<Transform, Transform> { { key.transform, root.transform } };
            foreach (var source in key.GetComponentsInChildren<Renderer>(true)) {
                Mesh mesh = null;
                if (source is SkinnedMeshRenderer skin) { mesh = new Mesh(); skin.BakeMesh(mesh); meshes.Add(mesh); }
                else if (source is MeshRenderer) mesh = source.GetComponent<MeshFilter>()?.sharedMesh;
                if (!mesh) continue;
                var part = CopyTransform(source.transform, transforms);
                part.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
                var renderer = part.gameObject.AddComponent<MeshRenderer>(); renderer.sharedMaterials = source.sharedMaterials;
                renderer.shadowCastingMode = ShadowCastingMode.Off; renderer.receiveShadows = false;
            }
            // Match ItemColliderGeometry.Apply(restorePivot:true) and TrayDisplayPreview:
            // recovered prefabs carry an import-time visual pivot that gameplay resets.
            if (key.transform.childCount > 0 && transforms.TryGetValue(key.transform.GetChild(0), out var visual)) {
                visual.localPosition = Vector3.zero; visual.localRotation = Quaternion.identity;
            }
            var renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return null;
            var destination = new Vector3(1000, 1000, 1000);
            var cameraRotation = Camera.main ? Camera.main.transform.rotation : Quaternion.identity;
            var pose = Match3DTrayDisplayBinding.For(target.gameObject)?.pose;
            if (pose == null && Match3DTrayDisplaySettings.Current)
                Match3DTrayDisplaySettings.Current.TryGet(key, out pose);
            root.transform.SetPositionAndRotation(destination, Match3DTrayDisplaySettings.Rotation(pose, cameraRotation));
            root.transform.localScale = key.transform.localScale;
            // Frame the mesh bounds in camera space, including height. Model pivots need not
            // be centered, and world X/Z bounds are incorrect for the configured tray pose.
            var viewBounds = new Bounds(); bool hasBounds = false;
            foreach (var renderer in renderers) {
                var b = renderer.GetComponent<MeshFilter>().sharedMesh.bounds;
                for (int i = 0; i < 8; i++) {
                    var corner = b.center + Vector3.Scale(b.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                    var p = Quaternion.Inverse(cameraRotation) * (renderer.transform.TransformPoint(corner) - destination);
                    if (!hasBounds) { viewBounds = new Bounds(p, Vector3.zero); hasBounds = true; } else viewBounds.Encapsulate(p);
                }
            }
            camera = new GameObject("Probe thumbnail camera").AddComponent<Camera>();
            camera.enabled = false; camera.orthographic = true; camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear; camera.cullingMask = 1 << 30; camera.allowHDR = false;
            camera.useOcclusionCulling = false;
            camera.transform.rotation = cameraRotation;
            float radius = Mathf.Max(.0001f, Mathf.Max(viewBounds.extents.x, viewBounds.extents.y));
            camera.orthographicSize = radius * 1.12f; camera.nearClipPlane = .01f; camera.farClipPlane = viewBounds.size.magnitude * 4 + 10;
            camera.transform.position = destination + cameraRotation * viewBounds.center - camera.transform.forward * (viewBounds.size.magnitude + 2);
            lightObject = new GameObject("Probe thumbnail light"); var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional; light.intensity = .8f; light.cullingMask = 1 << 30;
            light.transform.rotation = Quaternion.Euler(60, -25, 0);
            texture.Create(); camera.targetTexture = texture;
            camera.Render();
            // Copy the completed capture into an owned texture before disposing its camera.
            RenderTexture.active = texture;
            var icon = new Texture2D(144, 144, TextureFormat.RGBA32, false) { name = "Probe " + key.name };
            icon.ReadPixels(new Rect(0, 0, 144, 144), 0, 0); icon.Apply(false, true);
            cache.Add(key, icon); return icon;
        } finally {
            RenderTexture.active = previous;
            // Destroy immediately: a second capture in this frame must not see the previous thumbnail.
            if (camera) Object.DestroyImmediate(camera.gameObject);
            if (lightObject) Object.DestroyImmediate(lightObject);
            Object.DestroyImmediate(root); foreach (var mesh in meshes) Object.DestroyImmediate(mesh);
            texture.Release(); Object.Destroy(texture);
        }
    }
    static Transform CopyTransform(Transform source, Dictionary<Transform, Transform> copies) {
        if (copies.TryGetValue(source, out var existing)) return existing;
        var parent = CopyTransform(source.parent, copies);
        var copy = new GameObject(source.name) { layer = 30 }.transform;
        copy.SetParent(parent, false); copy.localPosition = source.localPosition;
        copy.localRotation = source.localRotation; copy.localScale = source.localScale;
        copies.Add(source, copy); return copy;
    }
    public void Dispose() { foreach (var icon in cache.Values) if (icon) Object.Destroy(icon); cache.Clear(); }
}
