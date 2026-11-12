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
            // A hierarchy-free visual copy keeps renderer transforms, but not body scale/growth or scripts.
            foreach (var source in target.GetComponentsInChildren<Renderer>(true)) {
                Mesh mesh = null;
                if (source is SkinnedMeshRenderer skin) { mesh = new Mesh(); skin.BakeMesh(mesh); meshes.Add(mesh); }
                else if (source is MeshRenderer) mesh = source.GetComponent<MeshFilter>()?.sharedMesh;
                if (!mesh) continue;
                var part = new GameObject("Geometry") { layer = 30 }; part.transform.SetParent(root.transform, false);
                part.transform.position = source.transform.position;
                part.transform.rotation = source.transform.rotation;
                part.transform.localScale = source.transform.lossyScale;
                part.AddComponent<MeshFilter>().sharedMesh = mesh;
                var renderer = part.AddComponent<MeshRenderer>(); renderer.sharedMaterials = source.sharedMaterials;
                renderer.shadowCastingMode = ShadowCastingMode.Off; renderer.receiveShadows = false;
            }
            var renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return null;
            var bounds = renderers[0].bounds; foreach (var r in renderers) bounds.Encapsulate(r.bounds);
            var destination = new Vector3(1000, 1000, 1000);
            root.transform.position = destination - bounds.center;
            bounds.center = destination;
            camera = new GameObject("Probe thumbnail camera").AddComponent<Camera>();
            camera.enabled = false; camera.orthographic = true; camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear; camera.cullingMask = 1 << 30; camera.allowHDR = false;
            camera.useOcclusionCulling = false;
            // Preserve the top-down board pose so the icon and its source are readily recognizable.
            camera.transform.rotation = Camera.main ? Camera.main.transform.rotation : Quaternion.Euler(90, 0, 0);
            float radius = Mathf.Max(.01f, Mathf.Max(bounds.extents.x, bounds.extents.z));
            camera.orthographicSize = radius * 1.2f; camera.nearClipPlane = .01f; camera.farClipPlane = bounds.size.magnitude * 4 + 10;
            camera.transform.position = bounds.center - camera.transform.forward * (bounds.size.magnitude + 2);
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
    public void Dispose() { foreach (var icon in cache.Values) if (icon) Object.Destroy(icon); cache.Clear(); }
}
