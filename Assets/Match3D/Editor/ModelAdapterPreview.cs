using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ModelAdapterPreview {
    public static void Open(ModelAdapterReport report) {
        if (report == null || string.IsNullOrEmpty(report.generatedPrefabPath)) return;
        string dir = Path.GetDirectoryName(report.generatedPrefabPath).Replace('\\', '/');
        Generate(report, dir);
        Application.OpenURL(new Uri(Path.GetFullPath(Path.Combine(Directory.GetParent(Application.dataPath).FullName, dir + "/index.html"))).AbsoluteUri);
    }

    public static void Generate(ModelAdapterReport report, string directory) {
        Directory.CreateDirectory(ToAbsolute(directory));
        string prefabPath = report.generatedPrefabPath;
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        string[] views = { "perspective", "top", "front", "side" };
        foreach (var view in views) {
            var png = Render(prefab, view);
            if (png != null) File.WriteAllBytes(ToAbsolute(directory + "/" + view + ".png"), png.EncodeToPNG());
        }
        string svg = BuildOverlay(report);
        File.WriteAllText(ToAbsolute(directory + "/collision-overlay.svg"), svg);
        File.WriteAllText(ToAbsolute(directory + "/report.json"), JsonUtility.ToJson(report, true));
        File.WriteAllText(ToAbsolute(directory + "/index.html"), Html(report, views));
        AssetDatabase.Refresh();
        report.previewPath = directory + "/index.html";
    }

    static Texture2D Render(GameObject prefab, string view) {
        if (!prefab) return null;
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.hideFlags = HideFlags.HideAndDontSave;
        var utility = new PreviewRenderUtility(true);
        try {
            utility.AddSingleGO(instance);
            utility.lights[0].intensity = 1.1f; utility.lights[0].transform.rotation = Quaternion.Euler(35, 35, 0);
            utility.lights[1].intensity = .6f; utility.lights[1].transform.rotation = Quaternion.Euler(340, 218, 177);
            var bounds = BoundsOf(instance);
            Vector3 direction = view == "top" ? Vector3.up : view == "front" ? Vector3.forward : view == "side" ? Vector3.right : new Vector3(1, .8f, 1);
            float distance = Mathf.Max(bounds.size.magnitude * 1.8f, .8f);
            utility.camera.transform.position = bounds.center + direction.normalized * distance;
            utility.camera.transform.LookAt(bounds.center);
            utility.camera.nearClipPlane = .01f; utility.camera.farClipPlane = distance * 4f; utility.camera.fieldOfView = 35f;
            utility.camera.clearFlags = CameraClearFlags.Color; utility.camera.backgroundColor = new Color(.08f, .08f, .08f, 1);
            utility.BeginPreview(new Rect(0, 0, 640, 640), GUIStyle.none); utility.camera.Render(); return utility.EndPreview() as Texture2D;
        } finally { utility.Cleanup(); UnityEngine.Object.DestroyImmediate(instance); }
    }

    static Bounds BoundsOf(GameObject obj) { var renderers = obj.GetComponentsInChildren<Renderer>(true); if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.one); var bounds = renderers[0].bounds; for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds); return bounds; }
    static string BuildOverlay(ModelAdapterReport report) { float w = 640, h = 420, max = Mathf.Max(report.visualSize.x, Mathf.Max(report.visualSize.y, report.visualSize.z)); if (max <= 0) max = 1; float sx = report.visualSize.x / max * 220, sy = report.visualSize.y / max * 220; return "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 640 420'><rect width='640' height='420' fill='#151515'/><rect x='" + ((w - sx) / 2).ToString("F1") + "' y='" + ((h - sy) / 2).ToString("F1") + "' width='" + sx.ToString("F1") + "' height='" + sy.ToString("F1") + "' fill='none' stroke='#75d6ff' stroke-width='3'/><line x1='320' y1='40' x2='320' y2='380' stroke='#f6d365' stroke-dasharray='5,5'/><line x1='150' y1='210' x2='490' y2='210' stroke='#f6d365' stroke-dasharray='5,5'/><text x='18' y='28' fill='#fff' font-size='18'>Visual bounds / origin</text><text x='18' y='400' fill='#75d6ff' font-size='16'>size " + ModelAdapterRules.Format(report.visualSize.x) + " × " + ModelAdapterRules.Format(report.visualSize.y) + " × " + ModelAdapterRules.Format(report.visualSize.z) + "</text></svg>"; }
    static string Html(ModelAdapterReport report, string[] views) { string cards = string.Empty; foreach (var view in views) cards += "<figure><img src='" + view + ".png'><figcaption>" + view + "</figcaption></figure>"; string issues = string.Empty; foreach (var issue in report.issues) issues += "<li class='" + issue.severity.ToString().ToLowerInvariant() + "'><b>" + issue.code + "</b> " + issue.message + "</li>"; return "<!doctype html><meta charset='utf-8'><title>Model Adapter - " + report.logicalId + "</title><style>body{font:15px system-ui;background:#111;color:#eee;margin:24px}h1{font-size:22px}section{background:#1c1c1c;border:1px solid #444;border-radius:10px;padding:16px;margin:12px 0}.grid{display:grid;grid-template-columns:repeat(2,minmax(240px,1fr));gap:12px}figure{margin:0;background:#090909;padding:8px;border-radius:8px}img{width:100%;height:260px;object-fit:contain}figcaption{color:#aaa;text-align:center}.pass{color:#8ee28e}.warning{color:#ffd479}.error{color:#ff8181}dt{color:#aaa}dd{margin:2px 0 10px 0}</style><h1>模型适配预览：" + report.logicalId + "</h1><section><dl><dt>显示名称</dt><dd>" + report.displayName + "</dd><dt>尺寸</dt><dd>" + report.visualSize + "，归一化 " + ModelAdapterRules.Format(report.normalizedScale) + "</dd><dt>Collider</dt><dd>" + report.colliderCount + "，Pick=" + report.hasPickCollider + "，Body=" + report.hasBodyCollider + "</dd></dl></section><section class='grid'>" + cards + "</section><section><h2>碰撞/原点叠加</h2><img src='collision-overlay.svg' style='width:100%;max-width:640px'></section><section><h2>检查结果</h2><ul>" + issues + "</ul></section>"; }
    static string ToAbsolute(string projectPath) => Path.GetFullPath(Path.Combine(Directory.GetParent(Application.dataPath).FullName, projectPath));
}
