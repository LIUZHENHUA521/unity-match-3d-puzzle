// Copy into an isolated Unity project's Assets folder for a real player test.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
public sealed class PanPlayerProbe : MonoBehaviour {
    [Serializable] public class ItemFixture { public GameObject prefab; public Vector3[] vertices; }
    public GameObject panPrefab;
    public ItemFixture[] items;
    void Start() {
        var lines = new List<string>();
        int failures = 0;
        try {
            Physics.autoSimulation = false;
            BoardPhysicsSettings.Begin();
            ItemColliderGeometry.ConfigureLayers();
            var pan = Instantiate(panPrefab);
            Match3DThemePresentation.AddPanCollision(pan);
            var mesh = pan.transform.Find("Grab3D Frying Pan").GetComponentInChildren<MeshFilter>();
            var collider = mesh.GetComponent<MeshCollider>();
            Physics.SyncTransforms();
            bool hit = collider.Raycast(new Ray(new Vector3(0, 20, 0), Vector3.down), out var floor, 40);
            lines.Add("mesh readable=" + mesh.sharedMesh.isReadable + "; floor hit=" + hit + "; floor=" + floor.point.ToString("F4"));
            if (!hit || floor.point.y < .45f) throw new Exception("Visible pan has no valid player collision surface");
            FirstStageContainer.CreateBoundary();
            int cases = 0;
            foreach (var fixture in items)
            foreach (float size in new[] { .5f, 1f, 1.25f })
            for (int index = 0; index < 12; index++) {
                var prefab = fixture.prefab;
                var item = Instantiate(prefab);
                item.transform.localScale *= BoardLayoutSettings.ModelScale(prefab) / BoardLayoutSettings.ModelSizeMultiplier * size;
                // Stay below the first-stage safety lid; a 22-unit drop starts above it.
                item.transform.position = new Vector3((index % 3 - 1) * .7f, index < 6 ? 3 : 12, index % 2 == 0 ? .7f : -.7f);
                item.transform.rotation = Quaternion.Euler(index * 23, index * 37, index * 13);
                var body = item.GetComponent<Rigidbody>();
                body.isKinematic = false;
                BoardPhysicsSettings.Apply(body);
                Physics.SyncTransforms();
                for (int step = 0; step < 600; step++) Physics.Simulate(.02f);
                // Bounds corners are empty space for tilted bottles. Use the
                // actual rendered vertices, serialized by the editor fixture.
                float bottom = fixture.vertices.Min(v => item.transform.TransformPoint(v).y);
                bool supported = bottom >= floor.point.y - .04f && body.position.y < 5;
                if (!supported) failures++;
                lines.Add((supported ? "PASS " : "FAIL ") + prefab.name + " size=" + size + " drop=" + index + " bottom=" + bottom.ToString("F4") + " mode=" + body.collisionDetectionMode);
                item.SetActive(false);
                Destroy(item);
                cases++;
            }
            lines.Add("cases=" + cases + "; failures=" + failures);
        } catch (Exception error) {
            failures++;
            lines.Add(error.ToString());
        }
        string output = Environment.GetCommandLineArgs().SkipWhile(x => x != "--report").Skip(1).First();
        File.WriteAllLines(output, lines);
        Debug.Log(string.Join("\n", lines));
        Application.Quit(failures == 0 ? 0 : 1);
    }
}
