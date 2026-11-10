using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Editor-only conservative primitive decomposition. Triangles crossing a split
// are clipped into both regions; fitting vertex clusters alone leaves face gaps.
public static class ItemBodyFitter {
    public const int MaxShapes = 8;
    const float Padding = .0008f; // normalized longest side is .43
    public struct Triangle {
        public Vector3 a, b, c;
        public Triangle(Vector3 a, Vector3 b, Vector3 c) { this.a = a; this.b = b; this.c = c; }
    }
    [Serializable] public sealed class Shape {
        public string type;
        public Vector3 center, size;
        public Quaternion rotation = Quaternion.identity;
        public float radius, height, volume;
        public int direction;
        public bool Contains(Vector3 point, float tolerance = .00001f) {
            var p = Quaternion.Inverse(rotation) * (point - center);
            if (type == "BoxCollider") return Mathf.Abs(p.x) <= size.x * .5f + tolerance && Mathf.Abs(p.y) <= size.y * .5f + tolerance && Mathf.Abs(p.z) <= size.z * .5f + tolerance;
            if (type == "CapsuleCollider") p[direction] -= Mathf.Clamp(p[direction], -Mathf.Max(0, height * .5f - radius), Mathf.Max(0, height * .5f - radius));
            return p.sqrMagnitude <= (radius + tolerance) * (radius + tolerance);
        }
        public Collider AddTo(Transform parent, int index) {
            var node = new GameObject("Shape " + index + " " + type.Replace("Collider", ""));
            node.layer = ItemColliderGeometry.BodyLayer; node.transform.SetParent(parent, false);
            node.transform.localPosition = center; node.transform.localRotation = rotation;
            if (type == "BoxCollider") { var c = node.AddComponent<BoxCollider>(); c.size = size; return c; }
            if (type == "SphereCollider") { var c = node.AddComponent<SphereCollider>(); c.radius = radius; return c; }
            var capsule = node.AddComponent<CapsuleCollider>(); capsule.radius = radius; capsule.height = height; capsule.direction = direction; return capsule;
        }
    }
    sealed class Region {
        public List<Triangle> triangles;
        public Shape shape;
        public Region left, right;
        public float gain;
        public bool searched;
    }
    public static List<Triangle> ReadTriangles(Transform geometry, Transform space) {
        var output = new List<Triangle>();
        foreach (var renderer in geometry.GetComponentsInChildren<Renderer>(true)) {
            if (!renderer.enabled || !renderer.gameObject.activeInHierarchy || renderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly) continue;
            Mesh mesh = null; bool baked = false;
            if (renderer is SkinnedMeshRenderer skin) { mesh = new Mesh(); skin.BakeMesh(mesh); baked = true; }
            else if (renderer is MeshRenderer) mesh = renderer.GetComponent<MeshFilter>()?.sharedMesh;
            if (!mesh) continue;
            try {
                if (!mesh.isReadable) throw new InvalidOperationException("Unreadable collision input: " + mesh.name);
                var matrix = space.worldToLocalMatrix * renderer.transform.localToWorldMatrix;
                var vertices = mesh.vertices.Select(v => matrix.MultiplyPoint3x4(v)).ToArray();
                var indices = mesh.triangles;
                for (int i = 0; i < indices.Length; i += 3) {
                    var t = new Triangle(vertices[indices[i]], vertices[indices[i + 1]], vertices[indices[i + 2]]);
                    if (!ModelAdapterRules.IsFinite(t.a) || !ModelAdapterRules.IsFinite(t.b) || !ModelAdapterRules.IsFinite(t.c)) throw new InvalidOperationException("Non-finite mesh vertex");
                    if (Vector3.Cross(t.b - t.a, t.c - t.a).sqrMagnitude > 1e-16f) output.Add(t);
                }
            } finally { if (baked) UnityEngine.Object.DestroyImmediate(mesh); }
        }
        if (output.Count == 0) throw new InvalidOperationException("No visible triangles to fit body collision");
        return output;
    }
    public static Shape[] Fit(List<Triangle> triangles) {
        var first = MakeRegion(triangles);
        float initialVolume = first.shape.volume;
        var regions = new List<Region> { first };
        while (regions.Count < MaxShapes) {
            Region chosen = null; Region[] replacement = null; float bestBenefit = 0;
            foreach (var region in regions) {
                var plan = SplitPlan(region, MaxShapes - regions.Count, initialVolume);
                if (plan == null) continue;
                float benefit = (region.shape.volume - plan.Sum(r => r.shape.volume)) / (plan.Length - 1);
                if (benefit > bestBenefit) { bestBenefit = benefit; chosen = region; replacement = plan; }
            }
            if (chosen == null) break;
            regions.Remove(chosen); regions.AddRange(replacement);
        }
        // Flat rings need angular sectors: rectangular bisection can retain the
        // central hole inside every candidate box and never find an improvement.
        var radial = FitRing(triangles);
        if (radial != null) {
            var bounds = new Bounds(triangles[0].a, Vector3.zero);
            foreach (var t in triangles) { bounds.Encapsulate(t.a); bounds.Encapsulate(t.b); bounds.Encapsulate(t.c); }
            float volume = radial.Sum(r => r.shape.volume);
            bool betterVolume = volume + initialVolume * .025f * (radial.Count - 1) < regions.Sum(r => r.shape.volume) + initialVolume * .025f * (regions.Count - 1);
            // Sector boxes overlap. Their volume sum can exceed the old box even
            // though their union frees the hole; retain that meaningful opening.
            bool opensHole = regions.Any(r => r.shape.Contains(bounds.center)) && radial.All(r => !r.shape.Contains(bounds.center)) && volume <= initialVolume * 1.15f;
            if (betterVolume || opensHole) regions = radial;
        }
        foreach (var region in regions) foreach (var t in region.triangles)
            if (!region.shape.Contains(t.a) || !region.shape.Contains(t.b) || !region.shape.Contains(t.c)) throw new InvalidOperationException("Primitive does not cover its clipped triangle");
        double sourceArea = triangles.Sum(t => (double)Vector3.Cross(t.b - t.a, t.c - t.a).magnitude);
        double clippedArea = regions.Sum(r => r.triangles.Sum(t => (double)Vector3.Cross(t.b - t.a, t.c - t.a).magnitude));
        if (Math.Abs(sourceArea - clippedArea) > Math.Max(1e-8, sourceArea * .0001)) throw new InvalidOperationException("Triangle clipping lost or duplicated surface area");
        // Flat rectangular artwork needs no seam padding when it remains a
        // single box. Retain the original tight bounds instead of making it bigger.
        if (regions.Count == 1 && regions[0].shape.type == "BoxCollider") {
            var bounds = new Bounds(triangles[0].a, Vector3.zero);
            foreach (var t in triangles) { bounds.Encapsulate(t.a); bounds.Encapsulate(t.b); bounds.Encapsulate(t.c); }
            var size = Vector3.Max(bounds.size, Vector3.one * .001f); float volume = size.x * size.y * size.z;
            if (volume <= regions[0].shape.volume) regions[0].shape = new Shape { type = "BoxCollider", center = bounds.center, size = size, volume = volume };
        }
        return regions.Select(r => r.shape).ToArray();
    }
    static List<Region> FitRing(List<Triangle> triangles) {
        var bounds = new Bounds(triangles[0].a, Vector3.zero);
        foreach (var t in triangles) { bounds.Encapsulate(t.a); bounds.Encapsulate(t.b); bounds.Encapsulate(t.c); }
        var axes = new[] { 0, 1, 2 }.OrderByDescending(i => bounds.size[i]).ToArray();
        int x = axes[0], y = axes[1]; var center = bounds.center;
        if (bounds.size[axes[2]] > bounds.size[y] * .65f) return null;
        // Test the projected center against actual triangles, not just vertices.
        // A plain square's four corner vertices must not be mistaken for a ring.
        foreach (var t in triangles) {
            var a = new Vector2(t.a[x]-center[x],t.a[y]-center[y]); var b = new Vector2(t.b[x]-center[x],t.b[y]-center[y]); var c = new Vector2(t.c[x]-center[x],t.c[y]-center[y]);
            float Cross(Vector2 u, Vector2 v) => u.x*v.y-u.y*v.x;
            if (Mathf.Abs(Cross(b-a,c-a)) < 1e-10f) continue;
            float ab = Cross(a,b), bc = Cross(b,c), ca = Cross(c,a);
            if ((ab >= -1e-9f && bc >= -1e-9f && ca >= -1e-9f) || (ab <= 1e-9f && bc <= 1e-9f && ca <= 1e-9f)) return null;
        }
        List<Region> best = null; float bestVolume = float.PositiveInfinity;
        foreach (float offset in new[] { 0f, Mathf.PI / 8 }) {
            var parts = new List<Region>();
            for (int i = 0; i < 8; i++) {
                float angle = offset + i * Mathf.PI / 4, end = angle + Mathf.PI / 4;
                var n1 = Vector3.zero; n1[x] = Mathf.Sin(angle); n1[y] = -Mathf.Cos(angle);
                var n2 = Vector3.zero; n2[x] = -Mathf.Sin(end); n2[y] = Mathf.Cos(end);
                var first = new List<Triangle>(); var second = new List<Triangle>();
                foreach (var t in triangles) Clip(t,n1,Vector3.Dot(n1,center),first);
                foreach (var t in first) Clip(t,n2,Vector3.Dot(n2,center),second);
                if (second.Count > 0) parts.Add(MakeRegion(second));
            }
            float volume = parts.Sum(p=>p.shape.volume);
            if (parts.Count > 0 && volume < bestVolume) { best = parts; bestVolume = volume; }
        }
        return best;
    }
    static Region[] SplitPlan(Region region, int budget, float initialVolume) {
        SearchSplit(region);
        if (region.left == null) return null;
        Region[] best = null; float bestBenefit = 0;
        void Consider(params Region[] plan) {
            int cost = plan.Length - 1; if (cost > budget) return;
            float gain = region.shape.volume - plan.Sum(r => r.shape.volume);
            if (gain < initialVolume * .025f * cost || gain < region.shape.volume * .12f) return;
            if (gain / cost > bestBenefit) { best = plan; bestBenefit = gain / cost; }
        }
        Consider(region.left, region.right);
        // A symmetric shape can require two cuts before empty corners disappear
        // (e.g. a teddy's head/body). Permit one bounded lookahead through a split
        // that is not useful alone, instead of leaving the full enclosing box.
        if (best == null && budget >= 2) {
            SearchSplit(region.left); SearchSplit(region.right);
            if (region.left.left != null) Consider(region.left.left, region.left.right, region.right);
            if (region.right.left != null) Consider(region.left, region.right.left, region.right.right);
            if (region.left.left != null && region.right.left != null) Consider(region.left.left, region.left.right, region.right.left, region.right.right);
        }
        return best;
    }
    public static List<Collider> Generate(Transform geometry, Transform body) {
        var shapes = Fit(ReadTriangles(geometry, body));
        var result = new List<Collider>();
        for (int i = 0; i < shapes.Length; i++) result.Add(shapes[i].AddTo(body, i));
        return result;
    }
    static Region MakeRegion(List<Triangle> triangles) {
        var points = new HashSet<Vector3>();
        foreach (var t in triangles) { points.Add(t.a); points.Add(t.b); points.Add(t.c); }
        return new Region { triangles = triangles, shape = FitPrimitive(points.ToArray()) };
    }
    static Shape FitPrimitive(Vector3[] points) {
        Shape best = null;
        foreach (var rotation in new[] { Quaternion.identity, PrincipalRotation(points) }) {
            var inverse = Quaternion.Inverse(rotation);
            var local = points.Select(p => inverse * p).ToArray();
            var bounds = new Bounds(local[0], Vector3.zero); foreach (var p in local) bounds.Encapsulate(p);
            var center = bounds.center;
            var size = Vector3.Max(bounds.size + Vector3.one * Padding * 2, Vector3.one * .002f);
            Choose(ref best, new Shape { type = "BoxCollider", center = rotation * center, rotation = rotation, size = size, volume = size.x * size.y * size.z });
            float radius = Mathf.Sqrt(local.Max(p => (p - center).sqrMagnitude)) + Padding;
            Choose(ref best, new Shape { type = "SphereCollider", center = rotation * center, radius = radius, volume = 4f / 3f * Mathf.PI * radius * radius * radius });
            for (int axis = 0; axis < 3; axis++) {
                float radial = 0;
                foreach (var p0 in local) { var p = p0 - center; p[axis] = 0; radial = Mathf.Max(radial, p.sqrMagnitude); }
                radial = Mathf.Sqrt(radial);
                foreach (float multiplier in new[] { 1.001f, 1.15f, 1.35f }) {
                    float r = Mathf.Max(.001f, radial * multiplier), start = float.PositiveInfinity, end = float.NegativeInfinity;
                    foreach (var p0 in local) {
                        var p = p0 - center; float along = p[axis]; p[axis] = 0;
                        float reach = Mathf.Sqrt(Mathf.Max(0, r * r - p.sqrMagnitude));
                        start = Mathf.Min(start, along + reach); end = Mathf.Max(end, along - reach);
                    }
                    float length = Mathf.Max(0, end - start); var c = center; c[axis] += (start + end) * .5f;
                    r += Padding;
                    Choose(ref best, new Shape { type = "CapsuleCollider", center = rotation * c, rotation = rotation, direction = axis,
                        radius = r, height = length + 2 * r, volume = Mathf.PI * r * r * (length + 4f / 3f * r) });
                }
            }
        }
        return best;
    }
    static void Choose(ref Shape best, Shape candidate) { if (best == null || candidate.volume < best.volume * .999f) best = candidate; }
    static Quaternion PrincipalRotation(Vector3[] points) {
        Vector3 mean = Vector3.zero; foreach (var p in points) mean += p; mean /= points.Length;
        var covariance = new float[3, 3];
        foreach (var p0 in points) { var p = p0 - mean; for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++) covariance[i, j] += p[i] * p[j]; }
        Vector3 Multiply(Vector3 v) => new Vector3(covariance[0,0]*v.x + covariance[0,1]*v.y + covariance[0,2]*v.z, covariance[1,0]*v.x + covariance[1,1]*v.y + covariance[1,2]*v.z, covariance[2,0]*v.x + covariance[2,1]*v.y + covariance[2,2]*v.z);
        var x = new Vector3(1, .71f, .39f).normalized;
        for (int k = 0; k < 24; k++) { var v = Multiply(x); if (v.sqrMagnitude < 1e-18f) return Quaternion.identity; x = v.normalized; }
        var y = Vector3.Cross(x, Mathf.Abs(x.y) < .8f ? Vector3.up : Vector3.right).normalized;
        for (int k = 0; k < 24; k++) { var v = Multiply(y); v -= x * Vector3.Dot(x, v); if (v.sqrMagnitude < 1e-18f) break; y = v.normalized; }
        var z = Vector3.Cross(x, y).normalized; y = Vector3.Cross(z, x).normalized;
        return Quaternion.LookRotation(z, y);
    }
    static void SearchSplit(Region region) {
        if (region.searched) return;
        region.searched = true; region.gain = float.NegativeInfinity;
        foreach (var rotation in new[] { Quaternion.identity, region.shape.rotation }) for (int axis = 0; axis < 3; axis++) {
            Vector3 normal = rotation * (axis == 0 ? Vector3.right : axis == 1 ? Vector3.up : Vector3.forward);
            float min = float.PositiveInfinity, max = float.NegativeInfinity;
            foreach (var t in region.triangles) foreach (var p in new[] { t.a, t.b, t.c }) { float d = Vector3.Dot(normal, p); min = Mathf.Min(min, d); max = Mathf.Max(max, d); }
            if (max - min < .012f) continue;
            foreach (float fraction in new[] { .3f, .5f, .7f }) {
                float cut = Mathf.Lerp(min, max, fraction);
                var left = new List<Triangle>(); var right = new List<Triangle>();
                foreach (var t in region.triangles) {
                    // A face exactly on the cut belongs to one region only.
                    if (Vector3.Dot(normal, t.a) == cut && Vector3.Dot(normal, t.b) == cut && Vector3.Dot(normal, t.c) == cut) left.Add(t);
                    else { Clip(t, normal, cut, left); Clip(t, -normal, -cut, right); }
                }
                if (left.Count == 0 || right.Count == 0) continue;
                var a = MakeRegion(left); var b = MakeRegion(right);
                float gain = region.shape.volume - a.shape.volume - b.shape.volume;
                if (gain > region.gain) { region.gain = gain; region.left = a; region.right = b; }
            }
        }
    }
    static void Clip(Triangle triangle, Vector3 normal, float cut, List<Triangle> output) {
        var input = new[] { triangle.a, triangle.b, triangle.c }; var polygon = new List<Vector3>(4);
        for (int i = 0; i < 3; i++) {
            var a = input[i]; var b = input[(i + 1) % 3]; float da = Vector3.Dot(normal, a) - cut, db = Vector3.Dot(normal, b) - cut;
            if (da <= 0) polygon.Add(a);
            if ((da < 0 && db > 0) || (da > 0 && db < 0)) polygon.Add(Vector3.LerpUnclamped(a, b, da / (da - db)));
        }
        for (int i = 1; i + 1 < polygon.Count; i++) {
            var t = new Triangle(polygon[0], polygon[i], polygon[i + 1]);
            if (Vector3.Cross(t.b - t.a, t.c - t.a).sqrMagnitude > 1e-16f) output.Add(t);
        }
    }
}
