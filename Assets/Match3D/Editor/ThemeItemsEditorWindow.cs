using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class ThemeItemsEditorWindow : EditorWindow {
    [SerializeField] ThemeItemsEditorService.Draft draft;
    [SerializeField] int themeIndex;
    [SerializeField] string search = "";
    [SerializeField] bool showCandidates;
    Match3DTheme library;
    GameObject[] candidates = Array.Empty<GameObject>();
    List<string> errors = new List<string>();
    Vector2 itemsScroll, candidatesScroll;
    string message, loadError;
    bool EditingLocked => EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling;

    [MenuItem("Goose/主题物品编辑器")]
    public static void Open() {
        var window = GetWindow<ThemeItemsEditorWindow>("主题物品编辑器");
        window.minSize = new Vector2(780, 560);
        window.Show();
    }
    void OnEnable() {
        saveChangesMessage = "主题物品有未保存修改，是否保存到关卡配置？";
        library = AssetDatabase.LoadAssetAtPath<Match3DTheme>("Assets/Match3D/Resources/Match3D/ThemeLibrary.asset");
        RefreshCandidates();
        if (draft == null || !draft.catalog) LoadTheme(); else errors = ThemeItemsEditorService.Validate(draft);
    }
    void RefreshCandidates() {
        ItemCatalog.ClearCache();
        candidates = AssetDatabase.IsValidFolder(ModelAdapterService.OutputRoot)
            ? AssetDatabase.FindAssets("t:Prefab", new[] { ModelAdapterService.OutputRoot })
                .Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(g)))
                .Where(p => p && p.GetComponent<BoardItemIdentity>()).OrderBy(p => p.name).ToArray()
            : Array.Empty<GameObject>();
    }
    void LoadTheme() {
        draft = null; loadError = null; errors.Clear();
        try {
            if (!library || library.themes == null || library.themes.Length == 0) throw new InvalidOperationException("没有主题配置。");
            themeIndex = Mathf.Clamp(themeIndex, 0, library.themes.Length - 1);
            var catalog = library.sourceLevelCatalog;
            if (!catalog) catalog = AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>(ThemeItemsEditorService.CatalogPath);
            if (AssetDatabase.GetAssetPath(catalog) != ThemeItemsEditorService.CatalogPath)
                throw new InvalidOperationException("此主题引用非默认关卡配置；当前导入器只支持默认 SourceLevelCatalog，暂不允许编辑以免保存到不同位置。");
            draft = ThemeItemsEditorService.Load(catalog, library.themes[themeIndex].sourceEnvironment);
            errors = ThemeItemsEditorService.Validate(draft);
            hasUnsavedChanges = false;
        } catch (Exception ex) { loadError = ex.Message; }
        Repaint();
    }
    void Changed() {
        hasUnsavedChanges = true; message = null;
        errors = ThemeItemsEditorService.Validate(draft);
        Repaint();
    }
    public override void SaveChanges() {
        try {
            message = "已保存，下次开局生效。备份：" + ThemeItemsEditorService.Save(draft);
            hasUnsavedChanges = false;
            errors = ThemeItemsEditorService.Validate(draft);
            base.SaveChanges();
        } catch (Exception ex) {
            message = "保存失败：" + ex.Message;
            EditorUtility.DisplayDialog("主题物品未保存", ex.Message, "返回编辑");
            throw; // Unity must abort closing when its unsaved-changes save fails.
        }
    }
    bool TrySave() {
        try { SaveChanges(); return !hasUnsavedChanges; }
        catch { return false; } // SaveChanges already displayed the actionable error.
    }
    public override void DiscardChanges() {
        LoadTheme();
        base.DiscardChanges();
    }
    bool FinishDraft() {
        if (!hasUnsavedChanges) return true;
        int choice = EditorUtility.DisplayDialogComplex("未保存的主题物品", "当前修改尚未保存。", "保存", "取消", "放弃修改");
        if (choice == 1) return false;
        if (choice == 0) return TrySave();
        DiscardChanges(); return true;
    }
    void OnGUI() {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
            GUILayout.Label("主题", GUILayout.Width(32));
            using (new EditorGUI.DisabledScope(EditingLocked)) {
                if (library && library.themes != null && library.themes.Length > 0) {
                    int next = EditorGUILayout.Popup(themeIndex, library.themes.Select(t => t.title + " · " + t.sourceEnvironment).ToArray());
                    if (next != themeIndex && FinishDraft()) { themeIndex = next; message = null; LoadTheme(); GUIUtility.ExitGUI(); }
                }
                if (GUILayout.Button("重新加载", EditorStyles.toolbarButton, GUILayout.Width(70)) && FinishDraft()) {
                    RefreshCandidates(); LoadTheme(); GUIUtility.ExitGUI();
                }
            }
            if (GUILayout.Button("模型导入工具", EditorStyles.toolbarButton, GUILayout.Width(100))) ModelAdapterWindow.Open();
            if (GUILayout.Button("大小与槽内姿态", EditorStyles.toolbarButton, GUILayout.Width(110))) TrayDisplayWindow.Open();
        }
        if (!string.IsNullOrEmpty(loadError)) { EditorGUILayout.HelpBox(loadError, MessageType.Error); return; }
        if (draft == null) return;
        if (EditingLocked) EditorGUILayout.HelpBox("运行或编译中仅可查看，请停止运行并等待编译结束后编辑。", MessageType.Info);
        EditorGUILayout.HelpBox("这里编辑实际关卡物品池。替换：把适配 Prefab 拖到物品框。本科局和清北局共用这套主题物品。\n权重是该类型被选中后的数量预算比例，不是抽中概率；默认 1 表示等权。", MessageType.Info);
        GUILayout.Label("当前物品（" + draft.rows.Count + "）" + (hasUnsavedChanges ? "  · 有未保存修改" : ""), EditorStyles.boldLabel);
        using (new EditorGUI.DisabledScope(EditingLocked)) {
            itemsScroll = EditorGUILayout.BeginScrollView(itemsScroll);
            int remove = -1;
            for (int i = 0; i < draft.rows.Count; i++) {
                var row = draft.rows[i];
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.MinHeight(58))) {
                    DrawPreview(row.prefab, 52);
                    using (new EditorGUILayout.VerticalScope()) {
                        EditorGUILayout.LabelField(row.prefab ? DisplayName(row.prefab) : "缺失物品", EditorStyles.boldLabel);
                        var replacement = (GameObject)EditorGUILayout.ObjectField(row.prefab, typeof(GameObject), false, GUILayout.MinWidth(160));
                        if (replacement != row.prefab) {
                            try { ThemeItemsEditorService.Replace(draft, i, replacement); Changed(); }
                            catch (Exception ex) { message = ex.Message; }
                        }
                    }
                    using (new EditorGUILayout.VerticalScope(GUILayout.Width(76))) {
                        GUILayout.Label("数量权重", EditorStyles.miniLabel);
                        float weight = EditorGUILayout.FloatField(row.weight);
                        if (!weight.Equals(row.weight)) { row.weight = weight; Changed(); }
                    }
                    if (GUILayout.Button("移除", GUILayout.Width(48), GUILayout.Height(40))) remove = i;
                }
            }
            if (remove >= 0) { draft.rows.RemoveAt(remove); Changed(); }
            EditorGUILayout.EndScrollView();
            Rect drop = GUILayoutUtility.GetRect(0, 36, GUILayout.ExpandWidth(true));
            GUI.Box(drop, "拖入适配后的 Prefab 添加物品（移除只改变主题，不删除模型文件）", EditorStyles.helpBox);
            HandleDrop(drop);
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.Label("添加物品", GUILayout.Width(65));
                var add = (GameObject)EditorGUILayout.ObjectField(null, typeof(GameObject), false);
                if (add) Add(add);
            }
            showCandidates = EditorGUILayout.Foldout(showCandidates, "从已适配模型中选择（" + candidates.Length + "）", true);
            if (showCandidates) DrawCandidates();
        }
        int clamped = draft.layers.Count(l => l.types > draft.rows.Count);
        if (clamped > 0) EditorGUILayout.HelpBox("保存时将下调 " + clamped + " 层的类型数至 " + draft.rows.Count + "，物品总数、时间和高度保持不变。", MessageType.Info);
        if (errors.Count > 0) EditorGUILayout.HelpBox(string.Join("\n", errors.Take(4)) + (errors.Count > 4 ? "\n另有 " + (errors.Count - 4) + " 项问题。" : ""), MessageType.Error);
        if (!string.IsNullOrEmpty(message)) EditorGUILayout.HelpBox(message, MessageType.Info);
        using (new EditorGUILayout.HorizontalScope()) {
            using (new EditorGUI.DisabledScope(EditingLocked || !hasUnsavedChanges || errors.Count > 0))
                if (GUILayout.Button("保存主题物品", GUILayout.Height(32))) TrySave();
            using (new EditorGUI.DisabledScope(EditingLocked || !hasUnsavedChanges))
                if (GUILayout.Button("放弃修改", GUILayout.Height(32), GUILayout.Width(100))) {
                    if (EditorUtility.DisplayDialog("放弃本次修改", "恢复到当前已保存的主题配置？", "放弃修改", "返回编辑")) { message = null; DiscardChanges(); }
                }
        }
    }
    static string DisplayName(GameObject prefab) {
        var identity = prefab.GetComponent<BoardItemIdentity>();
        return identity && !string.IsNullOrEmpty(identity.DisplayName) ? identity.DisplayName : prefab.name;
    }
    void DrawPreview(GameObject prefab, int size) {
        Texture texture = prefab ? AssetPreview.GetAssetPreview(prefab) ?? AssetPreview.GetMiniThumbnail(prefab) : null;
        GUILayout.Label(texture, GUILayout.Width(size), GUILayout.Height(size));
        if (prefab && AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID())) Repaint();
    }
    void DrawCandidates() {
        using (new EditorGUILayout.HorizontalScope()) {
            search = EditorGUILayout.TextField("搜索模型", search);
            if (GUILayout.Button("刷新候选", GUILayout.Width(80))) RefreshCandidates();
        }
        candidatesScroll = EditorGUILayout.BeginScrollView(candidatesScroll, GUILayout.Height(150));
        foreach (var candidate in candidates.Where(p => p && (p.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || DisplayName(p).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0))) {
            using (new EditorGUILayout.HorizontalScope()) {
                DrawPreview(candidate, 36);
                GUILayout.Label(DisplayName(candidate));
                if (GUILayout.Button("预览", GUILayout.Width(60))) ThemeItemPreviewWindow.Open(candidate);
                bool included = draft.rows.Any(r => r.prefab && string.Equals(r.prefab.name, candidate.name, StringComparison.OrdinalIgnoreCase));
                using (new EditorGUI.DisabledScope(included)) if (GUILayout.Button(included ? "已在主题中" : "添加", GUILayout.Width(95))) Add(candidate);
                if (GUILayout.Button("定位", GUILayout.Width(45))) EditorGUIUtility.PingObject(candidate);
            }
        }
        EditorGUILayout.EndScrollView();
    }
    void Add(GameObject prefab) {
        try { ThemeItemsEditorService.Add(draft, prefab); Changed(); }
        catch (Exception ex) { message = ex.Message; }
    }
    void HandleDrop(Rect area) {
        var evt = Event.current;
        if (EditingLocked || !area.Contains(evt.mousePosition) || (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform)) return;
        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        if (evt.type == EventType.DragPerform) {
            DragAndDrop.AcceptDrag();
            var failures = new List<string>();
            foreach (Object obj in DragAndDrop.objectReferences) {
                try { ThemeItemsEditorService.Add(draft, obj as GameObject); Changed(); }
                catch (Exception ex) { failures.Add(ex.Message); }
            }
            if (failures.Count > 0) message = string.Join("\n", failures);
        }
        evt.Use();
    }
}
