using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Runtime counterpart of the shipped LevelObjectPool.  The source keeps ObjectUnit
// instances in a per-name pool (Get/Recycle); it does not destroy a matched unit at
// the end of the combine animation.  This pool is intentionally limited to the
// layered source board.  Legacy authored levels keep their original Destroy path.
public sealed class ReplicaSourceObjectPool : MonoBehaviour
{
    sealed class Bucket
    {
        public readonly Stack<GameObject> items = new Stack<GameObject>();
    }

    static ReplicaSourceObjectPool instance;
    readonly Dictionary<string, Bucket> buckets = new Dictionary<string, Bucket>();
    readonly HashSet<GameObject> pooled = new HashSet<GameObject>();

    public static int PooledCount => instance ? instance.pooled.Count : 0;
    public static bool Active => instance != null;

    static ReplicaSourceObjectPool Ensure()
    {
        if (!instance && ReplicaFlow.Instance)
            instance = ReplicaFlow.Instance.gameObject.AddComponent<ReplicaSourceObjectPool>();
        return instance;
    }

    void Awake() { instance = this; }

    public static GameObject Get(GameObject prefab, string sourceName)
    {
        if (!prefab) return null;
        var owner = Ensure();
        GameObject obj = null;
        if (owner && !string.IsNullOrEmpty(sourceName) && owner.buckets.TryGetValue(sourceName, out var bucket))
        {
            while (bucket.items.Count > 0 && !obj)
            {
                var candidate = bucket.items.Pop();
                owner.pooled.Remove(candidate);
                if (candidate) obj = candidate;
            }
        }

        if (!obj) obj = Instantiate(prefab);
        var token = obj.GetComponent<ReplicaSourcePoolToken>() ?? obj.AddComponent<ReplicaSourcePoolToken>();
        token.sourceName = sourceName;
        if (!token.hasPrefabScale)
        {
            token.prefabScale = obj.transform.localScale;
            token.hasPrefabScale = true;
        }
        obj.transform.SetParent(null, false);
        obj.transform.localScale = token.prefabScale;
        // Re-enable the complete hierarchy hidden by Recycle. Source visibility
        // is reapplied by ReplicaLayeredPile after the object is registered.
        foreach (var child in obj.GetComponentsInChildren<Transform>(true)) child.gameObject.SetActive(true);
        foreach (var r in obj.GetComponentsInChildren<Renderer>(true)) r.enabled = true;
        obj.SetActive(true);
        var body = obj.GetComponent<Rigidbody>();
        if (body) body.detectCollisions = true;
        return obj;
    }

    public static void Recycle(GameObject obj, string sourceName = null)
    {
        if (!obj) return;
        var owner = Ensure();
        if (!owner) { Destroy(obj); return; }

        var token = obj.GetComponent<ReplicaSourcePoolToken>() ?? obj.AddComponent<ReplicaSourcePoolToken>();
        if (!string.IsNullOrEmpty(sourceName)) token.sourceName = sourceName;
        string key = token.sourceName;
        if (string.IsNullOrEmpty(key)) { Destroy(obj); return; }
        if (owner.pooled.Contains(obj)) return;

        obj.transform.DOKill(false);
        ReplicaSourceFlight.Forget(obj);
        ReplicaSourceAudio.Forget(obj);
        var body = obj.GetComponent<Rigidbody>();
        if (body)
        {
            // Unity rejects velocity writes while a body is kinematic. Clear
            // dynamic state when available, then store the inert state.
            if (!body.isKinematic)
            {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
            body.isKinematic = true;
            body.detectCollisions = false;
            body.useGravity = false;
        }
        foreach (var c in obj.GetComponentsInChildren<Collider>(true)) c.enabled = false;
        foreach (var r in obj.GetComponentsInChildren<Renderer>(true)) r.enabled = false;
        var anim = obj.GetComponent<ReplicaSourceGooseAnimation>();
        if (anim) anim.SetSourceVisible(false);
        if (token.hasPrefabScale) obj.transform.localScale = token.prefabScale;
        obj.transform.SetParent(owner.transform, false);
        obj.SetActive(false);

        if (!owner.buckets.TryGetValue(key, out var bucket)) owner.buckets.Add(key, bucket = new Bucket());
        bucket.items.Push(obj);
        owner.pooled.Add(obj);
    }

    // LevelObjectPool.Clear is called when a board memory scope is discarded.  It
    // destroys only pooled objects; active board objects are owned by the level list.
    public static void Clear()
    {
        var owner = instance;
        if (!owner) return;
        foreach (var bucket in owner.buckets.Values)
            while (bucket.items.Count > 0)
            {
                var obj = bucket.items.Pop();
                if (obj) Destroy(obj);
            }
        owner.buckets.Clear();
        owner.pooled.Clear();
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}

// Kept on the pooled instance so changing the gameplay name to type_N does not
// lose the source prefab key required by LevelObjectPool.Get/Recycle.
public sealed class ReplicaSourcePoolToken : MonoBehaviour
{
    [HideInInspector] public string sourceName;
    [HideInInspector] public Vector3 prefabScale;
    [HideInInspector] public bool hasPrefabScale;
}
