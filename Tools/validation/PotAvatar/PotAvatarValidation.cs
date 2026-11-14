using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class PotAvatarValidation {
    public static void Run() {
        var root = new GameObject("Avatar validation", typeof(RectTransform), typeof(Canvas));
        var camera = new GameObject("Camera").AddComponent<Camera>();
        var source = new Texture2D(240, 120, TextureFormat.RGBA32, false);
        var pixels = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        var previous = RenderTexture.active;
        try {
            // Wide fixture: center square is red, cropped side strips are green.
            source.SetPixels(Enumerable.Range(0, 240 * 120).Select(i => i % 240 >= 60 && i % 240 < 180 ? Color.red : Color.green).ToArray());
            source.Apply(); source.filterMode = FilterMode.Point;
            camera.enabled = false; camera.orthographic = true; camera.orthographicSize = 128;
            camera.aspect = 1; camera.transform.position = new Vector3(0, 0, -10);
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.blue; camera.allowHDR = false;
            var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.WorldSpace; canvas.worldCamera = camera;
            var avatar = new GameObject("Avatar", typeof(RectTransform), typeof(PotAvatarGraphic)).GetComponent<PotAvatarGraphic>();
            avatar.transform.SetParent(root.transform, false); avatar.rectTransform.sizeDelta = new Vector2(150, 150); avatar.texture = source;
            Color32[] baseline = null;
            foreach (int depth in new[] {0, 24}) {
                var target = new RenderTexture(256, 256, depth, RenderTextureFormat.ARGB32); target.Create();
                try {
                    camera.targetTexture = target; Canvas.ForceUpdateCanvases(); camera.Render(); RenderTexture.active = target;
                    pixels.ReadPixels(new Rect(0, 0, 256, 256), 0, 0); pixels.Apply(); var sample = pixels.GetPixels32();
                    Require(sample[128 * 256 + 128].r > 200, "avatar center visible");
                    foreach (int x in new[] {58, 198}) foreach (int y in new[] {58, 198})
                        Require(sample[y * 256 + x].b > 200 && sample[y * 256 + x].r < 10, "square corners absent");
                    Require(sample.Count(p => p.r > 200 && p.g < 10) > 17000, "circle retains expected area");
                    Require(!sample.Any(p => p.g > 200), "wide avatar center-cropped without stretching");
                    if (baseline != null) Require(sample.SequenceEqual(baseline), "identical with and without stencil");
                    baseline = sample;
                    File.WriteAllBytes("../pot-avatar-depth-" + depth + ".png", pixels.EncodeToPNG());
                } finally { camera.targetTexture = null; RenderTexture.active = previous; target.Release(); Object.DestroyImmediate(target); }
            }
            File.WriteAllText("../pot-avatar-result.txt", "PASS: circle visible, square corners absent, centered aspect crop, identical pixels with/without stencil.\n");
        } finally { RenderTexture.active = previous; Object.DestroyImmediate(root); Object.DestroyImmediate(camera.gameObject); Object.DestroyImmediate(source); Object.DestroyImmediate(pixels); }
    }
    static void Require(bool value, string message) { if (!value) throw new Exception(message); }
}
