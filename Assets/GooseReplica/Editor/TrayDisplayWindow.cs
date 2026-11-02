using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class TrayDisplayWindow : EditorWindow {
    public const string SettingsPath="Assets/GooseReplica/Resources/Replica/TrayDisplaySettings.asset";
    GameObject[] models=Array.Empty<GameObject>();string[] labels=Array.Empty<string>();int selected;
    ReplicaTrayDisplaySettings.Pose draft=new ReplicaTrayDisplaySettings.Pose();
    TrayDisplayPreview preview;BoardSizePreview boardPreview;Vector2 scroll;bool closeup,resetTray;float boardMultiplier=1;
    [MenuItem("Goose/槽内展示配置")]
    public static void Open(){GetWindow<TrayDisplayWindow>("道具展示配置");}
    public static ReplicaTrayDisplaySettings EnsureSettings(){
        var settings=AssetDatabase.LoadAssetAtPath<ReplicaTrayDisplaySettings>(SettingsPath);
        if(!settings){settings=CreateInstance<ReplicaTrayDisplaySettings>();AssetDatabase.CreateAsset(settings,SettingsPath);AssetDatabase.SaveAssets();}
        return settings;
    }
    public static void SavePose(GameObject model,ReplicaTrayDisplaySettings.Pose pose){
        var settings=EnsureSettings();Undo.RecordObject(settings,"保存槽内展示");settings.Set(model,pose);EditorUtility.SetDirty(settings);AssetDatabase.SaveAssets();
    }
    public static void ResetPose(GameObject model){var settings=EnsureSettings();Undo.RecordObject(settings,"重置槽内展示");settings.Remove(model);EditorUtility.SetDirty(settings);AssetDatabase.SaveAssets();}
    void OnEnable(){minSize=new Vector2(600,560);RefreshModels();Undo.undoRedoPerformed+=Reload;saveChangesMessage="道具展示配置尚未保存。";}
    void OnDisable(){Undo.undoRedoPerformed-=Reload;preview?.Dispose();preview=null;boardPreview?.Dispose();boardPreview=null;}
    void RefreshModels(){
        var previous=models.Length>0?models[Mathf.Clamp(selected,0,models.Length-1)]:null;measured=null;
        var catalog=Resources.Load<ReplicaSourceLevelPlan>("Replica/SourceLevelCatalog");
        if(!catalog)return;
        var imported=AssetDatabase.FindAssets("t:Prefab",new[]{ModelAdapterService.OutputRoot}).Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<GameObject>).Where(m=>m).Select(m=>new{model=m,label="已导入 / "+m.name});
        var rows=catalog.profiles.SelectMany(p=>p.prefabs.Where(m=>m).Select(m=>new{model=m,label=ThemeLabel(p.environment)+" / "+m.name})).Concat(imported).GroupBy(x=>x.model).Select(g=>g.First()).ToArray();
        models=rows.Select(x=>x.model).ToArray();labels=rows.Select(x=>x.label).ToArray();selected=previous?Array.IndexOf(models,previous):0;selected=Mathf.Clamp(selected,0,Mathf.Max(0,models.Length-1));Reload();
    }
    static string ThemeLabel(string value)=>value=="CountrysideStove"?"乡村":value=="GroceryStore"?"小卖部":value=="Barbecue"?"烧烤摊":value;
    void Reload(){
        draft=new ReplicaTrayDisplaySettings.Pose();boardMultiplier=1;resetTray=false;var settings=AssetDatabase.LoadAssetAtPath<ReplicaTrayDisplaySettings>(SettingsPath);
        if(models.Length>0&&settings){settings.Invalidate();boardMultiplier=settings.BoardMultiplier(models[selected]);if(settings.TryGet(models[selected],out var stored))draft=stored.Copy();}hasUnsavedChanges=false;Repaint();
    }
    public override void SaveChanges(){
        if(models.Length==0||!draft.IsValid||!ReplicaTrayDisplaySettings.ValidBoardMultiplier(boardMultiplier))return;
        var settings=EnsureSettings();Undo.RecordObject(settings,"保存道具展示配置");
        if(resetTray)settings.Remove(models[selected]);else settings.Set(models[selected],draft);
        settings.SetBoardMultiplier(models[selected],boardMultiplier);
        EditorUtility.SetDirty(settings);AssetDatabase.SaveAssets();hasUnsavedChanges=false;base.SaveChanges();
    }
    public override void DiscardChanges(){Reload();base.DiscardChanges();}
    bool CanSwitch(){
        if(!hasUnsavedChanges)return true;
        int choice=EditorUtility.DisplayDialogComplex("未保存的展示配置","切换前保存当前物品的调整？","保存","取消","放弃");
        if(choice==1)return false;if(choice==0){if(!draft.IsValid||!ReplicaTrayDisplaySettings.ValidBoardMultiplier(boardMultiplier))return false;SaveChanges();}else DiscardChanges();return true;
    }
    GameObject measured;float measuredSize;
    float NormalizedLongest(GameObject model){
        if(measured==model)return measuredSize;
        measured=model;var clone=Instantiate(model);
        try{clone.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);var rs=clone.GetComponentsInChildren<Renderer>().Where(r=>r.enabled).ToArray();if(rs.Length==0)return measuredSize=0;
            var bounds=rs[0].bounds;foreach(var r in rs)bounds.Encapsulate(r.bounds);return measuredSize=Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
        }finally{DestroyImmediate(clone);}
    }
    void OnGUI(){
        if(models.Length==0){EditorGUILayout.HelpBox("没有找到关卡模型。",MessageType.Info);if(GUILayout.Button("刷新模型"))RefreshModels();return;}
        scroll=EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.LabelField("道具展示配置",EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("棋盘大小与槽内姿态独立保存。棋盘大小同步缩放模型和碰撞体；保存后重新开始关卡生效，重新导入保留手动倍率。",MessageType.Info);
        int next=EditorGUILayout.Popup("物品",selected,labels);if(next!=selected&&CanSwitch()){selected=next;Reload();}
        using(new EditorGUILayout.HorizontalScope()){
            if(GUILayout.Button("上一件")&&CanSwitch()){selected=(selected+models.Length-1)%models.Length;Reload();}
            if(GUILayout.Button("下一件")&&CanSwitch()){selected=(selected+1)%models.Length;Reload();}
            if(GUILayout.Button("定位预制体"))EditorGUIUtility.PingObject(models[selected]);
            if(GUILayout.Button("刷新列表")&&CanSwitch()){measured=null;RefreshModels();}
        }
        EditorGUILayout.LabelField("棋盘展示",EditorStyles.boldLabel);
        var model=models[selected];string adapterId=ReplicaTrayDisplaySettings.AdapterId(model);
        if(!string.IsNullOrEmpty(adapterId)&&ReplicaSourceCatalog.TryGet(adapterId,out var catalogItem)){
            EditorGUILayout.LabelField("自动档位",string.IsNullOrEmpty(catalogItem.sizeTier)?"旧版统一尺寸":catalogItem.sizeTier+"档");
            EditorGUILayout.LabelField("分配依据",catalogItem.sizeReason??"尚未初始化大小层次",EditorStyles.wordWrappedMiniLabel);
        }
        float baseSize=NormalizedLongest(model)*ReplicaSourceProfile.BaseModelScale(model)/ReplicaSourceProfile.ModelSizeMultiplier;
        EditorGUILayout.LabelField("基准最长边",baseSize.ToString("F2")+" 世界单位");
        EditorGUI.BeginChangeCheck();
        boardMultiplier=EditorGUILayout.Slider("棋盘大小倍率",boardMultiplier,.25f,2);
        if(EditorGUI.EndChangeCheck())hasUnsavedChanges=true;
        EditorGUILayout.LabelField("最终最长边",(baseSize*boardMultiplier*ReplicaSourceProfile.ModelSizeMultiplier).ToString("F2")+"（含当前全局倍率 "+ReplicaSourceProfile.ModelSizeMultiplier.ToString("F2")+"×）");
        if(GUILayout.Button("恢复棋盘默认大小")){boardMultiplier=1;hasUnsavedChanges=true;}
        var boardRect=GUILayoutUtility.GetRect(560,260,GUILayout.ExpandWidth(true));
        if(Event.current.type==EventType.Repaint&&ReplicaTrayDisplaySettings.ValidBoardMultiplier(boardMultiplier)){
            if(boardPreview==null)boardPreview=new BoardSizePreview();
            GUI.DrawTexture(boardRect,boardPreview.Render(boardRect,model,boardMultiplier),ScaleMode.StretchToFill,false);
        }
        EditorGUILayout.LabelField("棋盘预览固定为 9:16 游戏视野宽度；网格间距 1 世界单位，斜放展示；不改变游戏内随机朝向。",EditorStyles.wordWrappedMiniLabel);
        EditorGUILayout.Space();EditorGUILayout.LabelField("槽内展示",EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        draft.rotation=EditorGUILayout.Vector3Field("角度（相对相机）",draft.rotation);
        draft.scale=EditorGUILayout.Slider("槽内大小倍率",draft.scale,.05f,4);
        draft.offset=EditorGUILayout.Vector3Field("位置偏移（格间距）",draft.offset);
        if(EditorGUI.EndChangeCheck()){hasUnsavedChanges=true;resetTray=false;}
        EditorGUILayout.LabelField("X 向右 · Y 向上 · Z 靠近镜头；角度直接替换默认姿态（−90, 0, 0）。",EditorStyles.wordWrappedMiniLabel);
        if(!draft.IsValid)EditorGUILayout.HelpBox("请输入有限数值，大小倍率需为 0.05–4。",MessageType.Error);
        using(new EditorGUILayout.HorizontalScope()){
            using(new EditorGUI.DisabledScope(!draft.IsValid||!ReplicaTrayDisplaySettings.ValidBoardMultiplier(boardMultiplier)))if(GUILayout.Button(hasUnsavedChanges?"保存配置 *":"保存配置"))SaveChanges();
            if(GUILayout.Button("恢复槽内默认姿态")){draft=new ReplicaTrayDisplaySettings.Pose();resetTray=true;hasUnsavedChanges=true;}
            if(GUILayout.Button("重新读取")&&CanSwitch())Reload();
        }
        closeup=EditorGUILayout.ToggleLeft("放大查看中央格（完整七格预览用于检查邻格遮挡）",closeup);
        var rect=GUILayoutUtility.GetRect(560,closeup?310:220,GUILayout.ExpandWidth(true));
        if(Event.current.type==EventType.Repaint&&draft.IsValid){
            if(preview==null)preview=new TrayDisplayPreview();preview.Select(models[selected]);
            GUI.DrawTexture(rect,preview.Render(rect,draft,3,closeup),ScaleMode.StretchToFill,false);
        }
        EditorGUILayout.LabelField("配置资产",SettingsPath,EditorStyles.wordWrappedMiniLabel);
        EditorGUILayout.EndScrollView();
    }
}
