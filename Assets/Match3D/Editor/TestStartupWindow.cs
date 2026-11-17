using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class TestStartupWindow:EditorWindow {
    [MenuItem("Goose/测试启动配置")]
    public static void Open(){GetWindow<TestStartupWindow>("测试启动配置");}
    void OnGUI(){
        EditorGUILayout.LabelField("Unity 测试关卡试玩",EditorStyles.boldLabel);
        int source=Match3DFlow.PotEditorSource;
        int nextSource=EditorGUILayout.Popup("配置来源",source,new[]{"内测每日关卡","纯本地配置","后台编辑器配置"});
        if(nextSource!=source)EditorPrefs.SetInt(Match3DFlow.PotEditorSourcePreference,nextSource);
        var catalog=Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
        var entries=catalog&&catalog.themes!=null?catalog.themes.Where(t=>t!=null).ToArray():Array.Empty<Match3DModelThemes.Entry>();
        var ids=new[]{""}.Concat(entries.Select(t=>t.id)).ToArray();var titles=new[]{"跟随默认"}.Concat(entries.Select(t=>t.title)).ToArray();
        int current=Math.Max(0,Array.IndexOf(ids,EditorPrefs.GetString(Match3DFlow.EditorThemePreference,"")));
        EditorGUI.BeginDisabledGroup(nextSource==0);
        int next=EditorGUILayout.Popup("编辑器主题",current,titles);
        if(next!=current)EditorPrefs.SetString(Match3DFlow.EditorThemePreference,ids[next]);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.HelpBox(nextSource==2
            ?"点击本科局或清北局，读取后台「编辑器玩家」的层数、物品数量、类型数量。主题使用上方选择，其余参数沿用本地配置。保存后下次新开局生效，重试保持本局配置。"
            :nextSource==0
            ?"点击本科局或清北局，每次新开局读取后台最新内测配置，主题、数量和限时跟随后台。道具免费，不发放红包。"
            :"点击本科局或清北局，使用上方主题及其本地关卡配置，不读取后台内测配置。两档均使用本地配置的数量和限时。修改在下次新开局生效。道具免费，不发放红包。",MessageType.Info);
    }
}
