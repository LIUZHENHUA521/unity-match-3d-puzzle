using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

// Clip the authored cropped arc in geometry, so a host framebuffer without stencil
// still draws exactly the remaining chances (and draws no triangles at zero).
[RequireComponent(typeof(CanvasRenderer))]
public sealed class ComboProbeArcGraphic : MaskableGraphic {
    public Sprite Sprite { get; private set; }
    float amount;
    public float fillAmount {
        get => amount;
        set { float next = Mathf.Clamp01(value); if (next == amount) return; amount = next; SetVerticesDirty(); }
    }
    public override Texture mainTexture => Sprite ? Sprite.texture : Texture2D.whiteTexture;
    public void Initialize(Sprite sprite) { Sprite = sprite; raycastTarget = false; SetAllDirty(); }
    protected override void OnPopulateMesh(VertexHelper mesh) {
        mesh.Clear(); if (!Sprite || amount <= 0) return;
        Rect rect = rectTransform.rect;
        Vector2 center = rect.min + Vector2.Scale(rect.size, new Vector2(.5f, .4f));
        const float start = -136.8f;
        float sweep = amount * 360;
        // The rectangle corners are the only places where the clipping edge changes.
        var angles = new List<float> { 0, sweep };
        for (int i = 0; i < 4; i++) {
            Vector2 corner = new Vector2((i & 1) == 0 ? rect.xMin : rect.xMax, (i & 2) == 0 ? rect.yMin : rect.yMax) - center;
            float angle = Mathf.Repeat(start - Mathf.Atan2(corner.y, corner.x) * Mathf.Rad2Deg, 360);
            if (angle > 0 && angle < sweep) angles.Add(angle);
        }
        angles.Sort(); Vector4 uv = DataUtility.GetOuterUV(Sprite);
        AddVertex(mesh, center, rect, uv);
        for (int i = 0; i < angles.Count; i++) {
            float angle = (start - angles[i]) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            float x = Mathf.Abs(direction.x) < .000001f ? float.PositiveInfinity : ((direction.x > 0 ? rect.xMax : rect.xMin) - center.x) / direction.x;
            float y = Mathf.Abs(direction.y) < .000001f ? float.PositiveInfinity : ((direction.y > 0 ? rect.yMax : rect.yMin) - center.y) / direction.y;
            AddVertex(mesh, center + direction * Mathf.Min(x, y), rect, uv);
            if (i > 0) mesh.AddTriangle(0, i, i + 1);
        }
    }
    void AddVertex(VertexHelper mesh, Vector2 point, Rect rect, Vector4 uv) {
        var vertex = UIVertex.simpleVert; vertex.color = color; vertex.position = point;
        vertex.uv0 = new Vector2(Mathf.Lerp(uv.x, uv.z, (point.x - rect.xMin) / rect.width), Mathf.Lerp(uv.y, uv.w, (point.y - rect.yMin) / rect.height));
        mesh.AddVert(vertex);
    }
}
