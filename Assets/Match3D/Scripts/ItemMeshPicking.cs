using System;
using System.Collections.Generic;
using UnityEngine;

// Pick colliders are only a broad phase. Render triangles define the clickable
// surface, without introducing concave MeshColliders on dynamic rigidbodies.
public sealed class ItemMeshPicking : MonoBehaviour
{
    static readonly Dictionary<Mesh, Surface> surfaces = new Dictionary<Mesh, Surface>();
    MeshFilter[] filters;
    MeshRenderer[] meshRenderers;
    SkinnedMeshRenderer[] skins;
    Mesh baked;
    [SerializeField] BoxCollider broadPhase;
    bool animated;
    public bool HasRenderGeometry { get { if (filters == null) CacheRenderers(); return filters.Length > 0 || skins.Length > 0; } }

    public static void Prepare(GameObject item, GameObject pickRoot)
    {
        if (!pickRoot) return;
        var geometry = item.GetComponent<ItemMeshPicking>() ?? item.AddComponent<ItemMeshPicking>();
        geometry.CacheRenderers();
        if (!geometry.HasRenderGeometry) return;
        if (!geometry.broadPhase) {
            var child = new GameObject("Mesh pick bounds");
            child.layer = ItemColliderGeometry.PickLayer;
            child.transform.SetParent(pickRoot.transform, false);
            geometry.broadPhase = child.AddComponent<BoxCollider>();
            item.GetComponent<BoardItemPoolToken>()?.InvalidateComponents();
        }
        geometry.RestoreBroadPhase();
        geometry.FitBroadPhase();
    }

    public void RestoreBroadPhase() { if (broadPhase) broadPhase.enabled = true; }

    void LateUpdate() { if (animated && broadPhase && broadPhase.enabled) FitBroadPhase(); }
    void FitBroadPhase()
    {
        bool any = false; var bounds = new Bounds();
        foreach (var filter in filters) if (filter && filter.sharedMesh)
            Encapsulate(ref bounds, ref any, filter.transform, filter.sharedMesh.bounds);
        foreach (var skin in skins) if (skin && skin.sharedMesh)
            Encapsulate(ref bounds, ref any, skin.transform, skin.localBounds);
        if (any) { broadPhase.center = bounds.center; broadPhase.size = bounds.size + Vector3.one * .002f; }
    }
    void Encapsulate(ref Bounds bounds, ref bool any, Transform source, Bounds local)
    {
        var matrix = broadPhase.transform.worldToLocalMatrix * source.localToWorldMatrix;
        for (int i = 0; i < 8; i++) {
            var point = matrix.MultiplyPoint3x4(local.center + Vector3.Scale(local.extents,
                new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1)));
            if (!any) { bounds = new Bounds(point, Vector3.zero); any = true; } else bounds.Encapsulate(point);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetCache() { surfaces.Clear(); }

    void Awake() { CacheRenderers(); }
    void CacheRenderers()
    {
        filters = GetComponentsInChildren<MeshFilter>(true);
        meshRenderers = new MeshRenderer[filters.Length];
        for (int i = 0; i < filters.Length; i++) meshRenderers[i] = filters[i].GetComponent<MeshRenderer>();
        skins = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        animated = skins.Length > 0 || GetComponentInChildren<Animator>(true) || GetComponentInChildren<Animation>(true);
    }
    void OnDestroy() { if (baked) Destroy(baked); }

    public static bool Raycast(GameObject item, Ray ray, float limit, float colliderDistance, out float distance)
    {
        var geometry = item.GetComponent<ItemMeshPicking>() ?? item.AddComponent<ItemMeshPicking>();
        return geometry.Intersect(ray, limit, colliderDistance, out distance);
    }

    bool Intersect(Ray ray, float limit, float colliderDistance, out float distance)
    {
        if (filters == null) CacheRenderers();
        distance = limit;
        bool hit = false, hasGeometry = false;
        for (int i = 0; i < filters.Length; i++)
        {
            var filter = filters[i]; var renderer = meshRenderers[i];
            if (!filter || !renderer || !filter.sharedMesh) continue;
            hasGeometry = true;
            if (!renderer.enabled || !renderer.gameObject.activeInHierarchy || !renderer.bounds.IntersectRay(ray)) continue;
            var mesh = filter.sharedMesh;
            if (!mesh.isReadable) continue; // Build validation rejects unreadable item meshes.
            if (!surfaces.TryGetValue(mesh, out var surface)) surfaces.Add(mesh, surface = new Surface(mesh));
            if (IntersectWorld(surface, filter.transform, ray, ref distance)) hit = true;
        }
        foreach (var skin in skins)
        {
            if (!skin || !skin.sharedMesh) continue;
            hasGeometry = true;
            if (!skin.enabled || !skin.gameObject.activeInHierarchy || !skin.bounds.IntersectRay(ray)) continue;
            if (!baked) baked = new Mesh { name = "Picking pose", hideFlags = HideFlags.HideAndDontSave };
            skin.BakeMesh(baked);
            if (IntersectWorld(new Surface(baked), skin.transform, ray, ref distance)) hit = true;
        }
        // Explicit compatibility for collider-only fixtures/legacy items. Hidden
        // renderers are NOT collider-only items and must never use this fallback.
        if (!hasGeometry && colliderDistance <= limit) { distance = colliderDistance; return true; }
        return hit;
    }

    static bool IntersectWorld(Surface surface, Transform transform, Ray ray, ref float distance)
    {
        // Unity normalizes Ray.direction. Convert the distance limit into local
        // ray units and back, including under nonuniform/negative scale.
        var direction = transform.InverseTransformVector(ray.direction);
        float scale = direction.magnitude;
        if (scale <= 1e-10f) return false;
        var localRay = new Ray(transform.InverseTransformPoint(ray.origin), direction / scale);
        float localDistance = distance * scale;
        if (!surface.Intersect(localRay, ref localDistance)) return false;
        distance = localDistance / scale;
        return true;
    }

    sealed class Surface
    {
        readonly Vector3[] vertices;
        readonly int[] triangles;
        readonly int[] order;
        readonly Bounds[] triangleBounds;
        readonly Node root;
        sealed class Node { public Bounds bounds; public Node left, right; public int start, count; }
        public Surface(Mesh mesh)
        {
            vertices = mesh.vertices; triangles = mesh.triangles;
            int count = triangles.Length / 3;
            order = new int[count]; triangleBounds = new Bounds[count];
            for (int i = 0; i < count; i++)
            {
                order[i] = i;
                var bounds = new Bounds(vertices[triangles[i * 3]], Vector3.zero);
                bounds.Encapsulate(vertices[triangles[i * 3 + 1]]); bounds.Encapsulate(vertices[triangles[i * 3 + 2]]);
                triangleBounds[i] = bounds;
            }
            if (count > 0) root = Build(0, count);
        }
        Node Build(int start, int count)
        {
            var node = new Node { start = start, count = count, bounds = triangleBounds[order[start]] };
            for (int i = start + 1; i < start + count; i++) node.bounds.Encapsulate(triangleBounds[order[i]]);
            if (count <= 12) return node;
            var size = node.bounds.size; int axis = size.x >= size.y && size.x >= size.z ? 0 : size.y >= size.z ? 1 : 2;
            Array.Sort(order, start, count, Comparer<int>.Create((a, b) => triangleBounds[a].center[axis].CompareTo(triangleBounds[b].center[axis])));
            int half = count / 2; node.left = Build(start, half); node.right = Build(start + half, count - half);
            return node;
        }
        public bool Intersect(Ray ray, ref float distance) { return root != null && Visit(root, ray, ref distance); }
        bool Visit(Node node, Ray ray, ref float distance)
        {
            if (!node.bounds.IntersectRay(ray, out float entry) || entry > distance) return false;
            if (node.left != null)
            {
                bool left = Visit(node.left, ray, ref distance);
                bool right = Visit(node.right, ray, ref distance);
                return left || right;
            }
            bool hit = false;
            for (int i = node.start; i < node.start + node.count; i++)
            {
                int index = order[i] * 3;
                var a = vertices[triangles[index]];
                var e1 = vertices[triangles[index + 1]] - a; var e2 = vertices[triangles[index + 2]] - a;
                var p = Vector3.Cross(ray.direction, e2); float determinant = Vector3.Dot(e1, p);
                if (Mathf.Abs(determinant) < 1e-10f) continue;
                float inverse = 1f / determinant;
                var offset = ray.origin - a; float u = Vector3.Dot(offset, p) * inverse;
                if (u < 0 || u > 1) continue;
                var q = Vector3.Cross(offset, e1); float v = Vector3.Dot(ray.direction, q) * inverse;
                if (v < 0 || u + v > 1) continue;
                float t = Vector3.Dot(e2, q) * inverse;
                if (t < 0 || t > distance) continue;
                distance = t; hit = true;
            }
            return hit;
        }
    }
}
