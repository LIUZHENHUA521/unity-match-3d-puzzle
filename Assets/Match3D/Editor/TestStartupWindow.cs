using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class TestStartupWindow:EditorWindow {
    [MenuItem("Goose/测试启动配置")]
    public static void Open(){GetWindow<TestStartupWindow>("测试启动配置");}
    void OnGUI(){
        EditorGUILayout.LabelField("Unity 内测关卡试玩",EditorStyles.boldLabel);
        var catalog=Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
        var entries=catalog&&catalog.themes!=null?catalog.themes.Where(t=>t!=null).ToArray():Array.Empty<Match3DModelThemes.Entry>();
        var ids=new[]{""}.Concat(entries.Select(t=>t.id)).ToArray();var titles=new[]{"跟随默认"}.Concat(entries.Select(t=>t.title)).ToArray();
        int current=Math.Max(0,Array.IndexOf(ids,EditorPrefs.GetString(Match3DFlow.EditorThemePreference,"")));
        int next=EditorGUILayout.Popup("本地工具主题",current,titles);
        if(next!=current)EditorPrefs.SetString(Match3DFlow.EditorThemePreference,ids[next]);
        EditorGUILayout.HelpBox("进入 Play 后点击本科局或清北局，每次新开局读取后台最新内测配置，主题也跟随后台。上方选择仅用于本地关卡工具，不覆盖内测关卡。道具免费，不发放红包。",MessageType.Info);
    }
}
