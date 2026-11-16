using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PotFoodPackPreviews {
    public static void Build() {
        Directory.CreateDirectory("work/pot-food-packs");
        foreach (string pack in PotFoodPackImporter.Packs) {
            var prefabs = Match3DModelThemes.Get("pot-" + pack, "CountrysideStove").prefabs;
            const int cell = 192, columns = 7, rows = 4;
            var sheet = new Texture2D(cell * columns, cell * rows, TextureFormat.RGB24, false);
            var blank = Enumerable.Repeat(new Color(.13f, .16f, .19f), sheet.width * sheet.height).ToArray(); sheet.SetPixels(blank);
            for (int i = 0; i < prefabs.Length; i++) {
                var utility = new PreviewRenderUtility(true);
                var model = Object.Instantiate(prefabs[i]);
                var frame = new Texture2D(cell, cell, TextureFormat.RGB24, false);
                var old = RenderTexture.active;
                try {
                    utility.AddSingleGO(model);
                    utility.ambientColor = new Color(.55f, .55f, .55f);
                    utility.lights[0].intensity = 1.1f; utility.lights[0].transform.rotation = Quaternion.Euler(50, 225, 0);
                    utility.lights[1].intensity = .6f; utility.lights[1].transform.rotation = Quaternion.Euler(30, 45, 0);
                    var renderers = model.GetComponentsInChildren<Renderer>(); var bounds = renderers[0].bounds;
                    foreach (var r in renderers.Skip(1)) bounds.Encapsulate(r.bounds);
                    float distance = Mathf.Max(bounds.size.magnitude * 1.6f, .8f);
                    utility.camera.transform.position = bounds.center + new Vector3(1, .8f, 1).normalized * distance;
                    utility.camera.transform.LookAt(bounds.center); utility.camera.nearClipPlane = .01f; utility.camera.farClipPlane = 10;
                    utility.camera.fieldOfView = 35; utility.camera.clearFlags = CameraClearFlags.Color; utility.camera.backgroundColor = new Color(.13f, .16f, .19f);
                    utility.BeginPreview(new Rect(0, 0, cell, cell), GUIStyle.none); utility.Render(true);
                    var texture = utility.EndPreview();
                    // Preview textures are scaled by Retina pixel density; resample the whole frame.
                    var scaled = RenderTexture.GetTemporary(cell, cell, 0);
                    Graphics.Blit(texture, scaled); RenderTexture.active = scaled;
                    frame.ReadPixels(new Rect(0, 0, cell, cell), 0, 0); frame.Apply();
                    sheet.SetPixels((i % columns) * cell, (rows - 1 - i / columns) * cell, cell, cell, frame.GetPixels());
                    RenderTexture.active = old; RenderTexture.ReleaseTemporary(scaled);
                } finally { RenderTexture.active = old; utility.Cleanup(); Object.DestroyImmediate(model); Object.DestroyImmediate(frame); }
            }
            sheet.Apply(); File.WriteAllBytes("work/pot-food-packs/pot-" + pack + "-models.png", sheet.EncodeToPNG()); Object.DestroyImmediate(sheet);
        }
    }
}
