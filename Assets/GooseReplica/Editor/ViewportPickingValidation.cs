using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ViewportPickingValidation
{
    static void Check(bool value, string message) { if (!value) throw new Exception(message); Debug.Log("PICK_CHECK " + message); }
    public static void Run()
    {
        try {
            EditorSceneManager.OpenScene("Assets/Scenes/GooseReplica.unity");
            var camera = Camera.main;
            var fitter = camera.GetComponent<CameraFitter>();
            var pot = GameObject.Find("Cooking Pot").GetComponent<Renderer>();
            foreach (float aspect in new[] { 9f/16, 9f/19.5f, 3f/4 }) {
                camera.aspect = aspect;
                fitter.SendMessage("Awake");
                var left = camera.WorldToViewportPoint(new Vector3(pot.bounds.min.x, 0, 0));
                var right = camera.WorldToViewportPoint(new Vector3(pot.bounds.max.x, 0, 0));
                Check(Mathf.Abs(left.x) < .001f && Mathf.Abs(right.x-1) < .001f, "pot fills width at aspect " + aspect);
                Check(camera.rect == new Rect(0,0,1,1), "no pillarbox");
            }
            foreach (var collider in UnityEngine.Object.FindObjectsOfType<Collider>())
                if (collider.name == "Pot containment") Check(collider.gameObject.layer == 2, "invisible wall excluded from picking");
            // A clean patch of space isolates pointer selection from the real board.
            camera.transform.position = new Vector3(100, 12, 0);
            camera.aspect = 9f/16;
            fitter.SendMessage("Awake");
            var input = UnityEngine.Object.FindObjectOfType<InputController>();
            var low = Item("lower", new Vector3(100, 0, 0));
            var high = Item("upper", new Vector3(100, 1, 0));
            Physics.SyncTransforms();
            Vector2 centre = camera.WorldToScreenPoint(high.transform.position);
            Check(input.PickAt(centre) == high, "front item wins over hidden item");
            high.SetActive(false); Physics.SyncTransforms();
            Check(input.PickAt(centre) == low, "uncovered item selectable");
            var edge = (Vector2)camera.WorldToScreenPoint(new Vector3(100.1f,0,0));
            float scale = Screen.width / 720f;
            Check(input.PickAt(edge + Vector2.right * 10f * scale) == low, "small edge miss selects visible item");
            Check(input.PickAt(edge + Vector2.right * 60f * scale) == null, "distant empty space does not select");
            var cover = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cover.transform.position = new Vector3(100,2,0); cover.transform.localScale = Vector3.one;
            Physics.SyncTransforms();
            Check(input.PickAt(centre) == null, "solid occluder blocks selection");
            File.WriteAllText("Temp/viewport-picking-validation.txt", "PASS: pot width 9:16, 9:19.5, 3:4; first-hit occlusion; edge tolerance; far miss; invisible walls\n");
            EditorApplication.Exit(0);
        } catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
    }
    static GameObject Item(string name, Vector3 position)
    {
        var item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name = name; item.tag = "Collectable";
        item.transform.position = position; item.transform.localScale = Vector3.one * .2f;
        item.AddComponent<Rigidbody>().useGravity = false;
        return item;
    }
}
