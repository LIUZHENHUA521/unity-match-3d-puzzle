using UnityEngine;
using UnityEngine.UI;

// Draw the circle itself: WeChat's host framebuffer may not provide stencil.
// RawImage inheritance preserves the existing downloaded/default texture binding.
public sealed class PotAvatarGraphic : RawImage {
    protected override void OnPopulateMesh(VertexHelper mesh) {
        mesh.Clear();
        Rect rect = GetPixelAdjustedRect();
        float radius = Mathf.Min(rect.width, rect.height) * .5f;
        if (radius <= 0) return;
        Vector2 uvSize = uvRect.size;
        if (texture) {
            float width = texture.width * Mathf.Abs(uvSize.x);
            float height = texture.height * Mathf.Abs(uvSize.y);
            if (width > height && width > 0) uvSize.x *= height / width;
            else if (height > width && height > 0) uvSize.y *= width / height;
        }
        AddVertex(mesh, rect.center, uvRect.center);
        const int segments = 96;
        for (int i = 0; i <= segments; i++) {
            float angle = i * Mathf.PI * 2 / segments;
            var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            AddVertex(mesh, rect.center + direction * radius,
                uvRect.center + Vector2.Scale(direction, uvSize) * .5f);
            if (i > 0) mesh.AddTriangle(0, i + 1, i);
        }
    }

    void AddVertex(VertexHelper mesh, Vector2 position, Vector2 uv) {
        var vertex = UIVertex.simpleVert;
        vertex.color = color;
        vertex.position = position;
        vertex.uv0 = uv;
        mesh.AddVert(vertex);
    }
}
