using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using DG.Tweening;

public static class ItemMeshPickingValidation
{
    static readonly List<string> results = new List<string>();
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); results.Add("PASS " + message); }
    static GameObject Cube(string name, Vector3 position, Vector3 scale, bool item = true)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube); obj.name = name;
        obj.transform.position = position; obj.transform.localScale = scale;
        if (item) { obj.tag = "Collectable"; obj.layer = ItemColliderGeometry.PickLayer; obj.AddComponent<Rigidbody>().useGravity = false; }
        return obj;
    }
    static Vector2 Screen(Camera camera, float x = 0, float y = 0) { return camera.WorldToScreenPoint(new Vector3(x, y, 0)); }
    static void Invoke(InputController input, string method, Vector2 position)
    {
        typeof(InputController).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(input, new object[] { position });
    }
    public static void Run()
    {
        try {
            ItemPickingMeshPreparation.Prepare();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("Picking camera").AddComponent<Camera>(); camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0, 0, -10); camera.orthographic = true; camera.orthographicSize = 3;
            var input = new GameObject("Picking input").AddComponent<InputController>(); typeof(InputController).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(input, null);
            typeof(Match3DSelectionFeedback).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(input.GetComponent<Match3DSelectionFeedback>(), null);
            var front = Cube("front", Vector3.zero, Vector3.one);
            var rear = Cube("rear", new Vector3(0, 0, 2), Vector3.one);
            Physics.SyncTransforms();
            Check(input.PickAt(Screen(camera)) == front, "front visible surface blocks rear");
            front.GetComponent<BoxCollider>().size = Vector3.one * 4;
            rear.transform.position = new Vector3(.8f, 0, 2); Physics.SyncTransforms();
            Check(input.PickAt(Screen(camera, .8f)) == rear, "oversized front box no longer blocks exposed rear mesh");
            var decoration = Cube("non-pick occluder", new Vector3(.8f, 0, -2), Vector3.one, false);
            Physics.SyncTransforms(); Check(input.PickAt(Screen(camera, .8f)) == rear, "default-layer scene collider ignored");
            decoration.layer = ItemColliderGeometry.WallLayer; Physics.SyncTransforms();
            Check(input.PickAt(Screen(camera, .8f)) == rear, "wall collider ignored");
            decoration.layer = ItemColliderGeometry.BodyLayer; Physics.SyncTransforms();
            Check(input.PickAt(Screen(camera, .8f)) == rear, "body collider ignored");
            UnityEngine.Object.DestroyImmediate(decoration);
            rear.transform.position = new Vector3(0, 0, 2);
            rear.GetComponent<BoxCollider>().size = new Vector3(1, 1, 10); Physics.SyncTransforms();
            Check(input.PickAt(Screen(camera)) == front, "nearest mesh wins even when rear bounding box is closer");
            front.GetComponent<Rigidbody>().isKinematic = true;
            Check(input.PickAt(Screen(camera)) == null, "ineligible visible front does not permit rear penetration");
            front.GetComponent<Rigidbody>().isKinematic = false;
            front.GetComponent<MeshRenderer>().enabled = false;
            Check(input.PickAt(Screen(camera)) == rear, "hidden render mesh does not intercept picking");
            front.GetComponent<MeshRenderer>().enabled = true; front.GetComponent<BoxCollider>().enabled = false;
            Physics.SyncTransforms(); Check(input.PickAt(Screen(camera)) == rear, "disabled pick collider excluded");
            UnityEngine.Object.DestroyImmediate(front); UnityEngine.Object.DestroyImmediate(rear);

            var frame = new GameObject("open frame"); frame.tag = "Collectable"; frame.layer = ItemColliderGeometry.PickLayer;
            frame.AddComponent<Rigidbody>().useGravity = false; frame.AddComponent<BoxCollider>().size = new Vector3(2, 2, .2f);
            foreach (var p in new[] { new Vector3(-.8f, 0, 0), new Vector3(.8f, 0, 0), new Vector3(0, -.8f, 0), new Vector3(0, .8f, 0) }) {
                var part = Cube("frame part", p, p.x == 0 ? new Vector3(2, .4f, .2f) : new Vector3(.4f, 2, .2f), false);
                UnityEngine.Object.DestroyImmediate(part.GetComponent<Collider>()); part.transform.SetParent(frame.transform, true);
            }
            rear = Cube("through hole", new Vector3(0, 0, 2), Vector3.one * .5f); Physics.SyncTransforms();
            Check(input.PickAt(Screen(camera)) == rear, "real hole between multiple meshes exposes rear");
            Check(input.PickAt(Screen(camera, .8f)) == frame, "frame solid edge remains selectable");
            frame.transform.rotation = Quaternion.Euler(20, 25, 35); frame.transform.localScale = new Vector3(-1.5f, .7f, 1.2f);
            Physics.SyncTransforms();
            var edgePoint = frame.transform.TransformPoint(new Vector3(.8f, 0, 0));
            Check(input.PickAt(camera.WorldToScreenPoint(edgePoint)) == frame, "rotated negative nonuniform scale supported");
            frame.GetComponent<BoxCollider>().size = Vector3.one * .01f;
            ItemMeshPicking.Prepare(frame, frame); Physics.SyncTransforms();
            Check(input.PickAt(camera.WorldToScreenPoint(edgePoint)) == frame, "conservative pick bounds cover visible mesh outside authored collider");
            foreach (var collider in frame.GetComponentsInChildren<Collider>()) collider.enabled = false;
            ItemMeshPicking.Prepare(frame, frame); Physics.SyncTransforms();
            Check(input.PickAt(camera.WorldToScreenPoint(edgePoint)) == frame, "unprofiled item reuse restores added pick bounds");
            UnityEngine.Object.DestroyImmediate(frame); UnityEngine.Object.DestroyImmediate(rear);

            front = Cube("slide A", new Vector3(-1, 0, 0), Vector3.one * .4f);
            rear = Cube("slide B", new Vector3(1, 0, 0), Vector3.one * .4f); Physics.SyncTransforms();
            Invoke(input, "BeginPointer", Screen(camera, -1));
            var feedback = input.GetComponent<Match3DSelectionFeedback>();
            Check(feedback.Selected == front, "press selects visible mesh");
            Invoke(input, "UpdatePointer", Screen(camera, 1));
            Check(feedback.Selected == rear, "held slide switches to second visible mesh");
            Invoke(input, "UpdatePointer", Screen(camera, 0));
            Check(!feedback.Selected, "slide into empty space cancels selection");
            input.CancelSelection();
            UnityEngine.Object.DestroyImmediate(front); UnityEngine.Object.DestroyImmediate(rear);

            var dense = new List<GameObject>();
            for (int i = 0; i < 70; i++) {
                var obj = Cube("dense " + i, new Vector3(i == 69 ? 0 : 1, 0, 1 + i * .05f), Vector3.one * .1f);
                obj.GetComponent<BoxCollider>().size = Vector3.one * 40; dense.Add(obj);
            }
            Physics.SyncTransforms(); Check(input.PickAt(Screen(camera)) == dense[69], "70 candidate hits grow buffer without dropping exposed target");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++) input.PickAt(Screen(camera));
            watch.Stop(); results.Add("INFO dense 70-candidate 1000 queries: " + watch.ElapsedMilliseconds + " ms (editor)");
            foreach (var obj in dense) UnityEngine.Object.DestroyImmediate(obj);
            ValidateAssets();
            Directory.CreateDirectory("ValidationOutput"); File.WriteAllLines("ValidationOutput/item-mesh-picking-validation.txt", results);
            Debug.Log(string.Join("\n", results)); EditorApplication.Exit(0);
        } catch (Exception e) { Debug.LogException(e); Directory.CreateDirectory("ValidationOutput"); File.WriteAllText("ValidationOutput/item-mesh-picking-validation.txt", string.Join("\n", results) + "\nFAIL " + e); EditorApplication.Exit(1); }
    }
    static void ValidateSkinned(GameObject prefab)
    {
        var instance = UnityEngine.Object.Instantiate(prefab);
        try {
            Physics.SyncTransforms();
            var renderers = instance.GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToArray();
            var bounds = renderers[0].bounds;
            foreach (var r in renderers) bounds.Encapsulate(r.bounds);
            bool hit = false;
            for (int x = 1; x < 8 && !hit; x++) for (int y = 1; y < 8 && !hit; y++) {
                var origin = new Vector3(Mathf.Lerp(bounds.min.x, bounds.max.x, x / 8f),
                    Mathf.Lerp(bounds.min.y, bounds.max.y, y / 8f), bounds.min.z - 5);
                hit = ItemMeshPicking.Raycast(instance, new Ray(origin, Vector3.forward), 100, 0, out _);
            }
            Check(hit, "current skinned pose selectable: " + prefab.name);
        } finally { UnityEngine.Object.DestroyImmediate(instance); }
    }
    static void ValidateAssets()
    {
        int prefabs = 0, meshes = 0, skinned = 0;
        var checkedMeshes = new HashSet<Mesh>();
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" })) {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (!prefab || !prefab.CompareTag("Collectable")) continue;
            prefabs++;
            foreach (var filter in prefab.GetComponentsInChildren<MeshFilter>(true)) {
                if (!filter.sharedMesh || !checkedMeshes.Add(filter.sharedMesh)) continue;
                if (!filter.sharedMesh.isReadable) throw new Exception("Unreadable player mesh " + AssetDatabase.GetAssetPath(filter.sharedMesh));
                meshes++;
            }
            int skinCount = prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length;
            skinned += skinCount;
            if (skinCount > 0) ValidateSkinned(prefab);
        }
        Check(prefabs > 0 && meshes > 0, "all item mesh assets readable: " + prefabs + " prefabs, " + meshes + " distinct static meshes, " + skinned + " skinned renderers");
        var data = JsonUtility.FromJson<ItemCatalog.Data>(Resources.Load<TextAsset>("Match3D/ModelAdapterCatalog").text);
        int count = 0;
        foreach (var item in data.items) {
            var instance = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(item.prefabPath));
            try {
                ItemColliderGeometry.Apply(instance, item.id, true);
                if (!instance.GetComponent<ItemMeshPicking>()) throw new Exception("Missing picking geometry " + item.id);
                if (!instance.GetComponentsInChildren<BoxCollider>(true).Any(c => c.name == "Mesh pick bounds" && c.gameObject.layer == ItemColliderGeometry.PickLayer)) throw new Exception("Missing conservative pick bounds " + item.id);
                var broad = instance.GetComponentsInChildren<BoxCollider>(true).First(c => c.name == "Mesh pick bounds");
                broad.enabled = false;
                instance.GetComponent<ItemColliderGeometry>().RestoreConfiguredColliderState();
                if (!broad.enabled) throw new Exception("Pool restore lost broad phase " + item.id);
                count++;
            } finally { UnityEngine.Object.DestroyImmediate(instance); }
        }
        Check(count == data.items.Length, "runtime preparation covers all " + count + " adapted catalog items");
    }
}
