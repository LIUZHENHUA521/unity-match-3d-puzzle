using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class PicnicBackdropBuilder {
    const string Prefab = "Assets/GooseReplica/Resources/Replica/Scenes/CountrysideStove.prefab";
    const string Image = "Assets/GooseReplica/Resources/sprite/img_picnic_bg.jpg";
    public static void Build() {
        if (EditorApplication.isPlaying) throw new Exception("Stop play mode first");
        var importer = (TextureImporter)AssetImporter.GetAtPath(Image);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        importer.mipmapEnabled = false;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.maxTextureSize = 2048;
        importer.SaveAndReimport();
        var root = PrefabUtility.LoadPrefabContents(Prefab);
        try {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name == "Table" || t.name.StartsWith("tableCoffeeSquare", StringComparison.OrdinalIgnoreCase)) t.gameObject.SetActive(false);
            var old = root.transform.Find("PicnicBackground");
            if (old) UnityEngine.Object.DestroyImmediate(old.gameObject);
            var g = new GameObject("PicnicBackground");g.transform.SetParent(root.transform, false);
            var renderer = g.AddComponent<SpriteRenderer>();renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Image);
            renderer.shadowCastingMode = ShadowCastingMode.Off;renderer.receiveShadows = false;
            g.AddComponent<ReplicaCameraBackdrop>();
            var cameraObject = new GameObject("Backdrop preview camera");
            try {
                var camera = cameraObject.AddComponent<Camera>();camera.enabled = false;
                camera.transform.SetPositionAndRotation(ReplicaThemePresentation.SourcePosition, ReplicaThemePresentation.SourceRotation);
                const float aspect = 720f/1560;
                float height = ReplicaThemePresentation.Size(aspect)*2;
                float scale = Mathf.Max(height*aspect/renderer.sprite.bounds.size.x,height/renderer.sprite.bounds.size.y);
                g.transform.SetPositionAndRotation(camera.transform.position+camera.transform.forward*60,camera.transform.rotation);
                g.transform.localScale = Vector3.one*scale;
            } finally { UnityEngine.Object.DestroyImmediate(cameraObject); }
            PrefabUtility.SaveAsPrefabAsset(root, Prefab);
            File.WriteAllLines("Temp/picnic-build.txt",root.GetComponentsInChildren<Renderer>().Select(r=>r.name+" "+r.GetType().Name));
        } finally { PrefabUtility.UnloadPrefabContents(root); }
    }
    public static void Capture() {
        var camera = Camera.main;var bg = UnityEngine.Object.FindObjectOfType<ReplicaCameraBackdrop>();
        if (!camera || !bg) throw new Exception("Start countryside gameplay first");
        var savedTarget = camera.targetTexture;float size = camera.orthographicSize;float aspect = camera.aspect;
        var savedActive = RenderTexture.active;
        try {
            foreach (var dimensions in new[]{new Vector2Int(720,1560),new Vector2Int(720,1280),new Vector2Int(768,1024)}) {
                var target = new RenderTexture(dimensions.x,dimensions.y,24);target.Create();
                var texture = new Texture2D(dimensions.x,dimensions.y,TextureFormat.RGB24,false);
                try {
                    camera.targetTexture=target;camera.aspect=(float)dimensions.x/dimensions.y;
                    camera.orthographicSize=ReplicaThemePresentation.Size(camera.aspect);bg.Fit(camera);
                    var s=bg.GetComponent<SpriteRenderer>().sprite.bounds.size;
                    var scale=bg.transform.lossyScale;
                    if(s.x*scale.x+0.001f<2*camera.orthographicSize*camera.aspect || s.y*scale.y+0.001f<2*camera.orthographicSize) throw new Exception("Background coverage failed");
                    camera.Render();RenderTexture.active=target;
                    texture.ReadPixels(new Rect(0,0,dimensions.x,dimensions.y),0,0);texture.Apply();
                    File.WriteAllBytes("Temp/picnic-"+dimensions.x+"x"+dimensions.y+".png",texture.EncodeToPNG());
                } finally {camera.targetTexture=savedTarget;RenderTexture.active=savedActive;UnityEngine.Object.DestroyImmediate(texture);UnityEngine.Object.DestroyImmediate(target);}
            }
            File.WriteAllText("Temp/picnic-validation.txt","PASS viewport coverage: 720x1560, 720x1280, 768x1024");
        } finally {camera.targetTexture=savedTarget;camera.aspect=aspect;camera.orthographicSize=size;bg.Fit(camera);RenderTexture.active=savedActive;}
    }
}
