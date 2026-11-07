using System;
using UnityEditor;
using UnityEngine;

// Isolated preview scene: use the board's actual material, ambient and directional light.
public sealed class ThemeItemGamePreview : IDisposable {
    readonly PreviewRenderUtility utility = new PreviewRenderUtility(true);
    GameObject instance, prefab;
    Bounds bounds;
    public ThemeItemGamePreview() {
        utility.ambientColor = BoardLighting.ItemAmbientColor;
        utility.lights[0].type = LightType.Directional;
        utility.lights[0].color = Color.white;
        utility.lights[0].intensity = BoardLighting.ItemLightIntensity;
        utility.lights[0].transform.rotation = BoardLighting.SavedRotation;
        utility.lights[1].intensity = 0;
        utility.camera.orthographic = true;
        utility.camera.allowHDR = false;
        utility.camera.clearFlags = CameraClearFlags.SolidColor;
        utility.camera.backgroundColor = new Color(.22f, .22f, .22f, 1);
    }
    public Texture Render(GameObject source, Rect rect, Vector2 orbit, float zoom) {
        if (!source) return null;
        if (prefab != source || !instance) {
            if (instance) UnityEngine.Object.DestroyImmediate(instance);
            prefab = source;
            instance = UnityEngine.Object.Instantiate(source);
            instance.hideFlags = HideFlags.HideAndDontSave;
            utility.AddSingleGO(instance);
            foreach (var body in instance.GetComponentsInChildren<Rigidbody>(true)) { body.isKinematic = true; body.detectCollisions = false; }
            foreach (var collider in instance.GetComponentsInChildren<Collider>(true)) collider.enabled = false;
            var renderers = instance.GetComponentsInChildren<Renderer>();
            bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds(Vector3.zero, Vector3.one);
            foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
        }
        var rotation = Quaternion.Euler(orbit.y, orbit.x, 0);
        float radius = Mathf.Max(.01f, bounds.extents.magnitude);
        utility.camera.transform.SetPositionAndRotation(bounds.center - rotation * Vector3.forward * radius * 4, rotation);
        utility.camera.orthographicSize = radius * 1.12f / Mathf.Min(1, rect.width / rect.height) / zoom;
        utility.camera.nearClipPlane = radius * .01f;
        utility.camera.farClipPlane = radius * 8;
        utility.BeginPreview(rect, GUIStyle.none);
        utility.Render();
        return utility.EndPreview();
    }
    public Texture2D Thumbnail(GameObject source) {
        var texture = Render(source, new Rect(0, 0, 128, 128), new Vector2(30, 25), 1);
        var previous = RenderTexture.active;
        var target = RenderTexture.GetTemporary(128, 128, 0);
        try {
            Graphics.Blit(texture, target);
            RenderTexture.active = target;
            var image = new Texture2D(128, 128, TextureFormat.RGB24, false) { hideFlags = HideFlags.HideAndDontSave };
            image.ReadPixels(new Rect(0, 0, 128, 128), 0, 0);
            image.Apply();
            return image;
        } finally { RenderTexture.active = previous; RenderTexture.ReleaseTemporary(target); }
    }
    public void Dispose() { utility.Cleanup(); }
}

public sealed class ThemeItemPreviewWindow : EditorWindow {
    [SerializeField] GameObject prefab;
    ThemeItemGamePreview preview;
    Vector2 orbit = new Vector2(30, 25);
    float zoom = 1;
    public static void Open(GameObject model) {
        if (!model) return;
        var window = GetWindow<ThemeItemPreviewWindow>("物品预览");
        window.minSize = new Vector2(440, 420);
        window.prefab = model;
        window.orbit = new Vector2(30, 25); window.zoom = 1;
        window.titleContent = new GUIContent("预览 · " + model.name);
        window.Release(); window.Show(); window.Repaint();
    }
    void OnEnable() { EditorApplication.projectChanged += Invalidate; }
    void OnDisable() { EditorApplication.projectChanged -= Invalidate; Release(); }
    void Release() { preview?.Dispose(); preview = null; }
    void Invalidate() { Release(); Repaint(); }
    void OnGUI() {
        if (!prefab) { EditorGUILayout.HelpBox("模型不存在，请重新选择预览物品。", MessageType.Info); return; }
        EditorGUILayout.LabelField(prefab.name, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("游戏内灯光 · 拖动旋转 · 滚轮缩放", EditorStyles.miniLabel);
        var rect = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        var evt = Event.current;
        int control = GUIUtility.GetControlID(FocusType.Passive);
        if (evt.type == EventType.MouseDown && evt.button == 0 && rect.Contains(evt.mousePosition)) { GUIUtility.hotControl = control; evt.Use(); }
        if (evt.type == EventType.MouseDrag && GUIUtility.hotControl == control) { orbit += new Vector2(-evt.delta.x, -evt.delta.y) * .6f; evt.Use(); Repaint(); }
        if (evt.type == EventType.MouseUp && GUIUtility.hotControl == control) { GUIUtility.hotControl = 0; evt.Use(); }
        if (evt.type == EventType.ScrollWheel && rect.Contains(evt.mousePosition)) { zoom = Mathf.Clamp(zoom * Mathf.Exp(-evt.delta.y * .08f), .4f, 3); evt.Use(); Repaint(); }
        if (evt.type == EventType.Repaint) {
            if (preview == null) preview = new ThemeItemGamePreview();
            GUI.DrawTexture(rect, preview.Render(prefab, rect, orbit, zoom), ScaleMode.StretchToFill, false);
        }
        if (GUILayout.Button("在项目中定位")) EditorGUIUtility.PingObject(prefab);
    }
}
