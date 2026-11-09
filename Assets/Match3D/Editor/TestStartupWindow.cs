using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class TestStartupWindow:EditorWindow {
    [MenuItem("Goose/测试启动配置")]
    public static void Open(){GetWindow<TestStartupWindow>("测试启动配置");}
    void OnGUI(){
        EditorGUILayout.LabelField("Unity 本地试玩",EditorStyles.boldLabel);
        var catalog=Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
        var entries=catalog&&catalog.themes!=null?catalog.themes.Where(t=>t!=null).ToArray():Array.Empty<Match3DModelThemes.Entry>();
        var ids=new[]{""}.Concat(entries.Select(t=>t.id)).ToArray();var titles=new[]{"跟随默认"}.Concat(entries.Select(t=>t.title)).ToArray();
        int current=Math.Max(0,Array.IndexOf(ids,EditorPrefs.GetString(Match3DFlow.EditorThemePreference,"")));
        int next=EditorGUILayout.Popup("启动主题",current,titles);
        if(next!=current)EditorPrefs.SetString(Match3DFlow.EditorThemePreference,ids[next]);
        EditorGUILayout.HelpBox("自动保存。进入 Play 后点击本科局或清北局，使用所选主题；下一次新开局生效。仅用于本机编辑器，不影响游戏发布或普通玩家。",MessageType.Info);
    }
}
