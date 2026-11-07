using UnityEditor;
using UnityEngine;

// Unity's prefab inspector preview renders in isolation and supports orbit/zoom.
public sealed class ThemeItemPreviewWindow : EditorWindow {
    [SerializeField] GameObject prefab;
    Editor previewEditor;

    public static void Open(GameObject model) {
        if (!model) return;
        var window = GetWindow<ThemeItemPreviewWindow>("物品预览");
        window.minSize = new Vector2(440, 420);
        window.SetModel(model);
        window.Show();
        window.Repaint();
    }
    void OnEnable() { if (prefab) SetModel(prefab); }
    void OnDisable() { ReleaseEditor(); }
    void ReleaseEditor() {
        if (previewEditor) DestroyImmediate(previewEditor);
        previewEditor = null;
    }
    void SetModel(GameObject model) {
        ReleaseEditor();
        prefab = model;
        if (prefab) previewEditor = Editor.CreateEditor(prefab);
        titleContent = new GUIContent(prefab ? "预览 · " + prefab.name : "物品预览");
    }
    void OnGUI() {
        if (!prefab) { EditorGUILayout.HelpBox("模型不存在，请重新选择预览物品。", MessageType.Info); return; }
        if (!previewEditor) SetModel(prefab);
        EditorGUILayout.LabelField(prefab.name, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("拖动旋转 · 滚轮缩放", EditorStyles.miniLabel);
        if (previewEditor && previewEditor.HasPreviewGUI()) {
            var rect = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            previewEditor.OnInteractivePreviewGUI(rect, EditorStyles.helpBox);
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) previewEditor.OnPreviewSettings();
        } else {
            EditorGUILayout.HelpBox("该模型暂不支持交互预览。", MessageType.Info);
            var texture = AssetPreview.GetAssetPreview(prefab);
            if (texture) GUILayout.Label(texture, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }
        if (GUILayout.Button("在项目中定位")) EditorGUIUtility.PingObject(prefab);
    }
}
