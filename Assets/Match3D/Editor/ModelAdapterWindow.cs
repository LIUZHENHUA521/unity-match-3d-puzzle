using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public sealed class ModelAdapterWindow : EditorWindow {
    string[] assets = Array.Empty<string>();
    int selected;
    string logicalId = string.Empty;
    string displayName = string.Empty;
    bool updateExisting;
    bool generateColliderDraft = true;
    [SerializeField] ModelAdapterResourceOptions resourceOptions = new ModelAdapterResourceOptions();
    ModelAdapterReport report;
    Vector2 scroll;
    string batchSummary;
    readonly List<string> batchDetails = new List<string>();

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
        if (assets.Length == 0) { EditorGUILayout.HelpBox("请把 Prefab/FBX/OBJ 放入 Assets/Match3D/IncomingModels。", MessageType.Warning); return; }
        var names = assets.Select(p => p.Substring(p.LastIndexOf('/') + 1)).ToArray();
        int next = EditorGUILayout.Popup("输入模型", selected, names);
        if (next != selected) { selected = next; SetDefaults(); report = null; }
        logicalId = EditorGUILayout.TextField("逻辑 ID", logicalId);
        displayName = EditorGUILayout.TextField("显示名称", displayName);
        updateExisting = EditorGUILayout.ToggleLeft("更新已有 ID（需要明确确认）", updateExisting);
        generateColliderDraft = EditorGUILayout.ToggleLeft("缺少 Pick/Body 时生成 BoxCollider 草稿", generateColliderDraft);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("模型资源优化", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        resourceOptions.optimizeTextures = EditorGUILayout.ToggleLeft("生成独立的优化贴图和材质", resourceOptions.optimizeTextures);
        using (new EditorGUI.DisabledScope(!resourceOptions.optimizeTextures)) {
            resourceOptions.maxTextureSize = EditorGUILayout.IntPopup("贴图最大尺寸", resourceOptions.maxTextureSize, new[] { "512（默认）", "1024（更清晰）" }, new[] { 512, 1024 });
            resourceOptions.compressTextures = EditorGUILayout.ToggleLeft("自动压缩贴图", resourceOptions.compressTextures);
        }
        resourceOptions.triangleWarning = Mathf.Max(1, EditorGUILayout.IntField("三角面提醒阈值", resourceOptions.triangleWarning));
        if (EditorGUI.EndChangeCheck()) report = null;
        EditorGUILayout.HelpBox("扫描只检查。生成时保留源文件，优化副本供新 Prefab 使用；不会自动减面。大小统计不是最终下载包体。", MessageType.Info);
        using (new EditorGUILayout.HorizontalScope()) {
            if (GUILayout.Button("扫描检查")) Scan();
            using (new EditorGUI.DisabledScope(report == null || report.HasErrors)) if (GUILayout.Button("生成适配 Prefab")) Generate();
            using (new EditorGUI.DisabledScope(report == null || string.IsNullOrEmpty(report.previewPath))) if (GUILayout.Button("打开浏览器预览")) ModelAdapterPreview.Open(report);
        }
        using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)) {
            if (GUILayout.Button("预览整套尺寸分配")) PreviewBatchSizes();
            if (GUILayout.Button("一键全部生成适配 Prefab")) GenerateAll();
        }
        EditorGUILayout.HelpBox("批量生成按各模型名称设置 ID，使用上方碰撞草稿和更新选项。", MessageType.Info);
        EditorGUILayout.HelpBox("自动分配小 / 中 / 大档；未知名称默认中档。生成后可在「Goose / 槽内展示配置」分别调整棋盘大小与槽内姿态。", MessageType.Info);
        if (GUILayout.Button("打开道具展示配置")) TrayDisplayWindow.Open();
        if (GUILayout.Button("编辑主题物品（添加 / 替换 / 移除）")) ThemeItemsEditorWindow.Open();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        if (!string.IsNullOrEmpty(batchSummary)) {
            EditorGUILayout.HelpBox(batchSummary, MessageType.Info);
            foreach (var detail in batchDetails) EditorGUILayout.HelpBox(detail, MessageType.None);
        }
        if (report == null) { EditorGUILayout.EndScrollView(); return; }
        EditorGUILayout.LabelField("状态", report.HasErrors ? "存在错误" : "可以生成", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("尺寸", report.visualSize.ToString("F4") + " / 归一化 " + ModelAdapterRules.Format(report.normalizedScale));
        EditorGUILayout.LabelField("建议棋盘尺寸", SizeSummary(report));
        EditorGUILayout.LabelField("Collider", report.colliderCount + "，Pick=" + report.hasPickCollider + "，Body=" + report.hasBodyCollider);
        DrawResources("源模型资源", report.sourceResources);
        DrawResources("生成后资源", report.generatedResources);
        foreach (var issue in report.issues) {
            var type = issue.severity == ModelAdapterSeverity.Error ? MessageType.Error : issue.severity == ModelAdapterSeverity.Warning ? MessageType.Warning : MessageType.Info;
            EditorGUILayout.HelpBox("[" + issue.code + "] " + issue.message + (string.IsNullOrEmpty(issue.path) ? string.Empty : "\n" + issue.path), type);
        }
        EditorGUILayout.EndScrollView();
    }

    static void DrawResources(string title, ModelAdapterResourceStats stats) {
        if (stats == null) return;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.LabelField(ModelAdapterResources.Summary(stats));
        EditorGUILayout.LabelField("引用源文件合计（去重）", EditorUtility.FormatBytes(stats.sourceFileBytes));
        EditorGUILayout.LabelField("贴图编辑器内存", EditorUtility.FormatBytes(stats.textureMemoryBytes));
        foreach (var texture in stats.textures)
            EditorGUILayout.LabelField(texture.name + " · " + texture.width + "×" + texture.height + " · " + EditorUtility.FormatBytes(texture.memoryBytes) + (texture.embedded ? " · 内嵌" : ""), EditorStyles.wordWrappedLabel);
    }

    static string SizeSummary(ModelAdapterReport report){
        if(ItemCatalog.TryGet(report.logicalId,out var existing)&&!string.IsNullOrEmpty(existing.sizeTier))
            return existing.sizeTier+"档 / "+(existing.normalizedToSource*ModelAdapterRules.SourceLongestSide).ToString("F2")+"（保留已有基准，手动倍率另存）";
        var size=ModelAdapterSizing.Suggest(report.logicalId,report.visualSize);
        return size.tier+"档 / "+size.longest.ToString("F2")+" · "+size.reason;
    }
    void PreviewBatchSizes(){
        RefreshAssets();batchDetails.Clear();
        foreach(var path in assets){
            string id=ModelAdapterRules.NormalizeId(Path.GetFileNameWithoutExtension(path));
            var scan=ModelAdapterService.Scan(path,id,id,generateColliderDraft,true,resourceOptions).report;
            batchDetails.Add(id+"："+(scan.HasErrors?"检查失败，请单独扫描":SizeSummary(scan)));
        }
        batchSummary="整套尺寸预览（世界单位，全局倍率 1；已保存的手动倍率保留）";
    }
    void GenerateAll() {
        RefreshAssets();
        var inputs = assets.ToArray();
        if (inputs.Length == 0) return;
        PreviewBatchSizes();
        if (!EditorUtility.DisplayDialog("确认批量尺寸分配", string.Join("\n", batchDetails)+"\n\n生成模型"+(updateExisting?"并更新已有 ID":"")+"；保留已保存的棋盘倍率和槽内配置。\n贴图：" + (resourceOptions.optimizeTextures ? resourceOptions.maxTextureSize + " / " + (resourceOptions.compressTextures ? "自动压缩" : "不压缩") : "沿用源设置"), "生成", "取消")) return;
        batchDetails.Clear();
        report = null;
        int succeeded = 0, failed = 0, processed = 0;
        // Reject every duplicate in this batch, including in update mode, so
        // two source models can never overwrite each other's output.
        var ids = inputs.Select(p => ModelAdapterRules.NormalizeId(Path.GetFileNameWithoutExtension(p))).ToArray();
        var duplicates = new HashSet<string>(ids.GroupBy(id => id, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1).Select(group => group.Key), StringComparer.OrdinalIgnoreCase);
        try {
            for (int i = 0; i < inputs.Length; i++) {
                if (EditorUtility.DisplayCancelableProgressBar("批量生成适配 Prefab",
                    (i + 1) + "/" + inputs.Length + " · " + ids[i], (float)i / inputs.Length)) break;
                try {
                    if (duplicates.Contains(ids[i])) {
                        failed++;
                        batchDetails.Add("跳过：" + inputs[i] + "\n本批次存在重复 ID：" + ids[i] + "，请先重命名模型。");
                    } else {
                        var result = ModelAdapterService.Generate(inputs[i], ids[i],
                            Path.GetFileNameWithoutExtension(inputs[i]), updateExisting, generateColliderDraft, resourceOptions);
                        if (result.HasErrors) {
                            failed++;
                            batchDetails.Add("失败/跳过：" + inputs[i] + "\n" + string.Join("\n", result.issues
                                .Where(issue => issue.severity == ModelAdapterSeverity.Error)
                                .Select(issue => "[" + issue.code + "] " + issue.message)));
                        } else {
                            succeeded++;
                            batchDetails.Add("成功：" + result.generatedPrefabPath + "\n" + ModelAdapterResources.Summary(result.generatedResources)
                                + "\n贴图编辑器内存：" + EditorUtility.FormatBytes(result.sourceResources.textureMemoryBytes) + " → " + EditorUtility.FormatBytes(result.generatedResources.textureMemoryBytes)
                                + "\n" + string.Join("\n", result.issues.Where(issue => issue.severity == ModelAdapterSeverity.Warning).Select(issue => issue.message)));
                        }
                    }
                } catch (Exception ex) {
                    failed++;
                    batchDetails.Add("失败：" + inputs[i] + "\n" + ex.Message);
                    Debug.LogException(ex);
                }
                processed++;
            }
        } finally {
            EditorUtility.ClearProgressBar();
            batchSummary = "批量处理" + (processed < inputs.Length ? "已取消" : "完成") +
                "：成功 " + succeeded + "，失败/跳过 " + failed + "，未处理 " + (inputs.Length - processed) + "。";
            Repaint();
        }
        EditorUtility.DisplayDialog("批量适配结果", batchSummary + "\n详细结果见窗口下方。", "确定");
    }

    void Scan() {
        if (updateExisting) { logicalId = ModelAdapterRules.NormalizeId(logicalId); }
        report = ModelAdapterService.Scan(assets[selected], logicalId, displayName, generateColliderDraft, updateExisting, resourceOptions).report;
        Repaint();
    }
    void Generate() {
        if (updateExisting && !EditorUtility.DisplayDialog("确认更新", "这会替换同逻辑 ID 的适配 prefab、尺寸、碰撞和音效映射。是否继续？", "继续", "取消")) return;
        report = ModelAdapterService.Generate(assets[selected], logicalId, displayName, updateExisting, generateColliderDraft, resourceOptions);
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
        ItemCatalog.ClearCache();
        BoardLayoutSettings.ClearCache();
        ItemColliderGeometry.ClearCache();
        GameAudioCatalog.ClearCache();
    }
}
