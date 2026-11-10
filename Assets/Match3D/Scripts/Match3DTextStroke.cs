using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A round pixel-radius stroke, including cardinal directions (uGUI Outline only uses corners).
[RequireComponent(typeof(Graphic))]
public sealed class Match3DTextStroke : BaseMeshEffect {
    public Color color = new Color32(2, 109, 209, 255);
    [Range(0, 8)] public float radius = 4;
    public override void ModifyMesh(VertexHelper helper) {
        if (!IsActive() || radius <= 0) return;
        var source = new List<UIVertex>();
        helper.GetUIVertexStream(source);
        var result = new List<UIVertex>(source.Count * 13);
        for (int i = 0; i < 12; i++) {
            float angle = i * Mathf.PI / 6;
            var offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            foreach (var original in source) {
                var vertex = original;
                vertex.position += offset;
                var tint = color;
                tint.a *= original.color.a / 255f;
                vertex.color = tint;
                result.Add(vertex);
            }
        }
        result.AddRange(source);
        helper.Clear();
        helper.AddUIVertexTriangleStream(result);
    }
}
