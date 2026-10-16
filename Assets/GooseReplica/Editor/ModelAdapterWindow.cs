using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class ModelAdapterWindow : EditorWindow {
    string[] assets = Array.Empty<string>();
    int selected;
    string logicalId = string.Empty;
    string displayName = string.Empty;
    bool updateExisting;
    bool generateColliderDraft = true;
    ModelAdapterReport report;
    Vector2 scroll;

    [MenuItem("Goose/Model Adapter")]
    public static void Open() { GetWindow<ModelAdapterWindow>("Model Adapter"); }

    void OnEnable() { RefreshAssets(); }

    void RefreshAssets() {
        assets = ModelAdapterService.InputAssets();
        if (selected >= assets.Length) selected = Mathf.Max(0, assets.Length - 1);
        if (assets.Length > 0 && string.IsNullOrEmpty(logicalId)) SetDefaults();
    }
    void SetDefaults() {
        var path = assets[selected];
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        logicalId = ModelAdapterRules.NormalizeId(prefab ? prefab.name : "new_item");
        displayName = prefab ? prefab.name : logicalId;
    }

    void OnGUI() {
        EditorGUILayout.HelpBox("从 IncomingModels 扫描模型，生成标准 Collectable prefab、运行时 catalog 和浏览器预览。", MessageType.Info);
        using (new EditorGUILayout.HorizontalScope()) {
            if (GUILayout.Button("刷新模型列表", GUILayout.Width(120))) RefreshAssets();
            GUILayout.Label(assets.Length == 0 ? "没有找到模型" : assets.Length + " 个模型");
        }
        if (assets.Length == 0) { EditorGUILayout.HelpBox("请把 Prefab/FBX/OBJ 放入 Assets/GooseReplica/IncomingModels。", MessageType.Warning); return; }
        var names = assets.Select(p => p.Substring(p.LastIndexOf('/') + 1)).ToArray();
        int next = EditorGUILayout.Popup("输入模型", selected, names);
        if (next != selected) { selected = next; SetDefaults(); report = null; }
        logicalId = EditorGUILayout.TextField("逻辑 ID", logicalId);
        displayName = EditorGUILayout.TextField("显示名称", displayName);
        updateExisting = EditorGUILayout.ToggleLeft("更新已有 ID（需要明确确认）", updateExisting);
        generateColliderDraft = EditorGUILayout.ToggleLeft("缺少 Pick/Body 时生成 BoxCollider 草稿", generateColliderDraft);
        using (new EditorGUILayout.HorizontalScope()) {
            if (GUILayout.Button("扫描检查")) Scan();
            using (new EditorGUI.DisabledScope(report == null || report.HasErrors)) if (GUILayout.Button("生成适配 Prefab")) Generate();
            using (new EditorGUI.DisabledScope(report == null || string.IsNullOrEmpty(report.previewPath))) if (GUILayout.Button("打开浏览器预览")) ModelAdapterPreview.Open(report);
        }
        if (report == null) return;
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.LabelField("状态", report.HasErrors ? "存在错误" : "可以生成", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("尺寸", report.visualSize.ToString("F4") + " / 归一化 " + ModelAdapterRules.Format(report.normalizedScale));
        EditorGUILayout.LabelField("Collider", report.colliderCount + "，Pick=" + report.hasPickCollider + "，Body=" + report.hasBodyCollider);
        foreach (var issue in report.issues) {
            var type = issue.severity == ModelAdapterSeverity.Error ? MessageType.Error : issue.severity == ModelAdapterSeverity.Warning ? MessageType.Warning : MessageType.Info;
            EditorGUILayout.HelpBox("[" + issue.code + "] " + issue.message + (string.IsNullOrEmpty(issue.path) ? string.Empty : "\n" + issue.path), type);
        }
        EditorGUILayout.EndScrollView();
    }

    void Scan() {
        if (updateExisting) { logicalId = ModelAdapterRules.NormalizeId(logicalId); }
        report = ModelAdapterService.Scan(assets[selected], logicalId, displayName, generateColliderDraft, updateExisting).report;
        Repaint();
    }
    void Generate() {
        if (updateExisting && !EditorUtility.DisplayDialog("确认更新", "这会替换同逻辑 ID 的适配 prefab、尺寸、碰撞和音效映射。是否继续？", "继续", "取消")) return;
        report = ModelAdapterService.Generate(assets[selected], logicalId, displayName, updateExisting, generateColliderDraft);
        if (report.HasErrors) EditorUtility.DisplayDialog("适配失败", "请查看诊断报告：" + report.requestId, "确定"); else EditorUtility.DisplayDialog("适配完成", report.generatedPrefabPath, "确定");
        Repaint();
    }
}

[InitializeOnLoad]
static class ModelAdapterEditorHooks {
    static ModelAdapterEditorHooks() {
        AssemblyReloadEvents.afterAssemblyReload += ClearRuntimeCatalogCaches;
        EditorApplication.playModeStateChanged += state => { if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode) ClearRuntimeCatalogCaches(); };
    }
    static void ClearRuntimeCatalogCaches() {
        ReplicaSourceCatalog.ClearCache();
        ReplicaSourceProfile.ClearCache();
        ReplicaSourceColliders.ClearCache();
        ReplicaSourceAudio.ClearCache();
    }
}
