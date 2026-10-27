using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class WechatPublishWindow : EditorWindow {
    [Serializable] class ReleaseConfig { public string appid, scene; }
    static Process worker;
    static string status = "准备发布";
    string version;
    string description = "Unity 小游戏更新";
    Vector2 scroll;
    static string Root => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
    static string LogPath => Path.Combine(Root, "Builds/Wechat/menu-publish.log");
    static string PrefKey => "WechatPublish.Version." + Root;

    // Unity localizes the built-in "Tools" menu to "工具" in Chinese.
    // Register against its canonical name, not the translated display label.
    [MenuItem("Tools/微信小游戏/发布")]
    public static void Open() {
        GetWindow<WechatPublishWindow>("微信小游戏发布").minSize = new Vector2(520, 380);
    }

    void OnEnable() {
        version = EditorPrefs.GetString(PrefKey, "0.1.12");
    }

    void OnInspectorUpdate() { Repaint(); }

    void OnGUI() {
        EditorGUILayout.LabelField("微信小游戏发布", EditorStyles.boldLabel);
        try {
            var config = JsonUtility.FromJson<ReleaseConfig>(File.ReadAllText(Path.Combine(Root, "Tools/wechat/release.config.json")));
            EditorGUILayout.LabelField("AppID", config.appid);
            EditorGUILayout.LabelField("发布场景", config.scene);
        } catch (Exception e) { EditorGUILayout.HelpBox(e.Message, MessageType.Error); }
        EditorGUILayout.HelpBox("上传开发版本后，请到微信公众平台设为体验版。请先登录微信开发者工具，并在设置 → 安全设置中开启服务端口。", MessageType.Info);
        using (new EditorGUI.DisabledScope(worker != null)) {
            version = EditorGUILayout.TextField("版本号", version);
            description = EditorGUILayout.TextField("更新说明", description);
            using (new EditorGUI.DisabledScope(EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)) {
                if (GUILayout.Button("保存、构建并上传开发版本", GUILayout.Height(32))) StartPublish();
            }
        }
        EditorGUILayout.LabelField(status, EditorStyles.wordWrappedLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("查看发布日志") && File.Exists(LogPath)) EditorUtility.RevealInFinder(LogPath);
        if (GUILayout.Button("查看 Unity 构建日志")) {
            var path = Path.Combine(Root, "Builds/wechat-build.log");
            if (File.Exists(path)) EditorUtility.RevealInFinder(path);
        }
        if (GUILayout.Button("微信公众平台")) Application.OpenURL("https://mp.weixin.qq.com/");
        EditorGUILayout.EndHorizontal();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        try {
            if (File.Exists(LogPath)) {
                var log = File.ReadAllText(LogPath);
                if (log.Length > 16000) log = log.Substring(log.Length - 16000);
                EditorGUILayout.TextArea(log, GUILayout.ExpandHeight(true));
            }
        } catch (IOException) { GUILayout.Label("正在更新日志…"); }
        EditorGUILayout.EndScrollView();
    }

    // ProcessStartInfo arguments are parsed by Mono, never by a shell.
    static string Quote(string value) { return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""; }

    void StartPublish() {
        if (Application.platform != RuntimePlatform.OSXEditor) {
            EditorUtility.DisplayDialog("发布", "当前发布工具支持 macOS。", "知道了");
            return;
        }
        if (!Regex.IsMatch(version ?? "", @"^\d+\.\d+\.\d+$") || string.IsNullOrWhiteSpace(description)) {
            EditorUtility.DisplayDialog("发布", "请输入三段版本号（例如 0.1.12）和更新说明。", "知道了");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        // Selecting Don't Save must not silently publish stale scene contents.
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++) {
            if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isDirty) {
                EditorUtility.DisplayDialog("发布", "请保存场景后再发布。", "知道了");
                return;
            }
        }
        AssetDatabase.SaveAssets();
        EditorPrefs.SetString(PrefKey, version);
        EditorApplication.LockReloadAssemblies();
        try {
            worker = Process.Start(new ProcessStartInfo {
                FileName = "/usr/bin/python3",
                Arguments = Quote(Path.Combine(Root, "Tools/wechat/publish.py")) + " --version " + Quote(version) + " --description " + Quote(description),
                WorkingDirectory = Root,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            status = "正在构建并上传，请勿退出 Unity；可以关闭此窗口，重新打开查看进度。";
            EditorApplication.update += Poll;
            EditorApplication.wantsToQuit += PreventQuit;
        } catch (Exception e) {
            worker = null;
            EditorApplication.UnlockReloadAssemblies();
            status = "启动失败：" + e.Message;
        }
    }

    static bool PreventQuit() {
        EditorUtility.DisplayDialog("正在发布", "请等待发布结束再退出 Unity。", "知道了");
        return false;
    }

    static void Poll() {
        if (worker == null || !worker.HasExited) return;
        status = worker.ExitCode == 0
            ? "开发版本上传成功，请到公众平台设为体验版。未提交审核。"
            : "发布失败，请查看发布日志及 Unity 构建日志。";
        worker.Dispose();
        worker = null;
        EditorApplication.update -= Poll;
        EditorApplication.wantsToQuit -= PreventQuit;
        EditorApplication.UnlockReloadAssemblies();
        UnityEngine.Debug.Log("[微信发布] " + status);
    }
}
