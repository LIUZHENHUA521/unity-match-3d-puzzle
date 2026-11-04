using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Reuse layered-board items after their match animation, resetting state on checkout.
// Buckets are keyed by prefab. Legacy authored levels retain their Destroy path.
// Provenance: docs/refactoring/2026-11-04-naming.md.
public sealed class BoardItemPool : MonoBehaviour
{
    sealed class Bucket
    {
        public readonly Stack<GameObject> items = new Stack<GameObject>();
    }

    static BoardItemPool instance;
    readonly Dictionary<GameObject, Bucket> buckets = new Dictionary<GameObject, Bucket>();
    readonly Dictionary<GameObject, GameObject> preparedTemplates = new Dictionary<GameObject, GameObject>();
    readonly HashSet<GameObject> pooled = new HashSet<GameObject>();

    public static int PooledCount => instance ? instance.pooled.Count : 0;
    public static bool Active => instance != null;
    public static int CacheHits {get;private set;}
    public static int CacheMisses {get;private set;}
    public static void ResetMetrics(){CacheHits=0;CacheMisses=0;}

    static BoardItemPool Ensure()
    {
        if (!instance && Match3DFlow.Instance)
            instance = Match3DFlow.Instance.gameObject.AddComponent<BoardItemPool>();
        return instance;
    }

    Transform inactiveRoot;
    Transform InactiveRoot {
        get {if(!inactiveRoot){var root=new GameObject("Prepared stage objects");root.SetActive(false);root.transform.SetParent(transform,false);inactiveRoot=root.transform;}return inactiveRoot;}
    }
    void Awake() { instance = this; }
    // Instantiate below an inactive parent: no gameplay Awake/OnEnable, random
    // draws, animation or collision registration occurs during warm-up.
    public static int CachedCount(GameObject prefab)=>instance&&prefab&&instance.buckets.TryGetValue(prefab,out var bucket)?bucket.items.Count:0;
    public static bool Prepare(GameObject prefab,int targetCount=1){
        if(!prefab)return false;var owner=Ensure();if(!owner)return false;
        if(targetCount<=0||CachedCount(prefab)>=targetCount)return false;
        var obj=Instantiate(owner.PreparedTemplate(prefab,prefab.name),owner.InactiveRoot,false);
        obj.SetActive(false);
        var token=obj.GetComponent<BoardItemPoolToken>()??obj.AddComponent<BoardItemPoolToken>();
        token.CacheComponents();
        token.sourcePrefab=prefab;token.sourceName=prefab.name;token.prefabScale=obj.transform.localScale;token.hasPrefabScale=true;
        if(!owner.buckets.TryGetValue(prefab,out var bucket))owner.buckets.Add(prefab,bucket=new Bucket());
        bucket.items.Push(obj);owner.pooled.Add(obj);return true;
    }


    GameObject PreparedTemplate(GameObject prefab, string sourceName)
    {
        // Editor-prepared prefabs already contain the native shapes and cloneable
        // reference cache. Avoid creating a second, runtime template for them.
        var prepared = prefab.GetComponent<ItemColliderGeometry>();
        if (prepared && prepared.IsConfiguredFor(sourceName)) return prefab;
        if (preparedTemplates.TryGetValue(prefab, out var existing) && existing)
            return existing;

        // Source ObjectUnit prefabs carry their colliders. The replica starts
        // from the recovered geometry JSON, so prepare one hidden template per
        // type and let Instantiate clone the already-built native collider
        // hierarchy. Only the first copy of a type pays the JSON/AddComponent
        // cost; every later copy avoids rebuilding it on the gameplay frame.
        var template = Instantiate(prefab,InactiveRoot,false);
        template.SetActive(false);
        template.name = "__SourceColliderTemplate_" + sourceName;
        ItemColliderGeometry.Apply(template, sourceName, true);
        template.transform.SetParent(InactiveRoot, false);
        template.transform.localPosition = new Vector3(0, -1000, 0);
        template.transform.localRotation = Quaternion.identity;
        foreach (var r in template.GetComponentsInChildren<Renderer>(true)) r.enabled = false;
        var body = template.GetComponent<Rigidbody>();
        if (body)
        {
            body.isKinematic = true;
            body.useGravity = false;
            body.detectCollisions = false;
        }
        preparedTemplates[prefab] = template;
        return template;
    }

    public static GameObject Get(GameObject prefab, string sourceName)
    {
        if (!prefab) return null;
        var owner = Ensure();
        GameObject obj = null;
        if (owner && !string.IsNullOrEmpty(sourceName) && owner.buckets.TryGetValue(prefab, out var bucket))
        {
            while (bucket.items.Count > 0 && !obj)
            {
                var candidate = bucket.items.Pop();
                owner.pooled.Remove(candidate);
                if (candidate) {obj = candidate;CacheHits++;}
            }
        }

        if (!obj)
        {
            CacheMisses++;
            var template = owner && !string.IsNullOrEmpty(sourceName) ? owner.PreparedTemplate(prefab, sourceName) : null;
            obj = template ? Instantiate(template) : Instantiate(prefab);
        }
        var token = obj.GetComponent<BoardItemPoolToken>() ?? obj.AddComponent<BoardItemPoolToken>();
        token.CacheComponents();
        token.sourceName = sourceName;
        token.sourcePrefab = prefab;
        if (!token.hasPrefabScale)
        {
            token.prefabScale = obj.transform.localScale;
            token.hasPrefabScale = true;
        }
        obj.transform.SetParent(null, false);
        obj.transform.localScale = token.prefabScale;
        // Re-enable the complete hierarchy hidden by Recycle. Source visibility
        // is reapplied by LayeredItemPile after the object is registered.
        foreach (var child in token.Transforms) if(child&&!child.gameObject.activeSelf)child.gameObject.SetActive(true);
        foreach (var r in token.Renderers) if(r&&!r.enabled)r.enabled = true;
        obj.SetActive(true);
        obj.GetComponent<ItemColliderGeometry>()?.RestoreConfiguredColliderState();
        var body = token.Body;
        if (body) body.detectCollisions = true;
        return obj;
    }

    public static void Recycle(GameObject obj, string sourceName = null)
    {
        if (!obj) return;
        var owner = Ensure();
        if (!owner) { Destroy(obj); return; }

        var token = obj.GetComponent<BoardItemPoolToken>() ?? obj.AddComponent<BoardItemPoolToken>();
        token.CacheComponents();
        if (!string.IsNullOrEmpty(sourceName)) token.sourceName = sourceName;
        var key = token.sourcePrefab;
        if (!key) { Destroy(obj); return; }
        if (owner.pooled.Contains(obj)) return;

        obj.transform.DOKill(false);
        ItemFlightMotion.Forget(obj);
        GameAudioCatalog.Forget(obj);
        var body = token.Body;
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
        foreach (var c in token.Colliders) if(c&&c.enabled)c.enabled = false;
        foreach (var r in token.Renderers) if(r&&r.enabled)r.enabled = false;
        var anim = obj.GetComponent<SpecialItemAnimation>();
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
        foreach (var template in owner.preparedTemplates.Values)
            if (template) Destroy(template);
        owner.buckets.Clear();
        owner.preparedTemplates.Clear();
        owner.pooled.Clear();
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}

// Kept on the pooled instance so changing the gameplay name to type_N does not
// lose the source prefab key required by LevelObjectPool.Get/Recycle.
public sealed class BoardItemPoolToken : MonoBehaviour
{
    [HideInInspector] public GameObject sourcePrefab;
    [HideInInspector] public string sourceName;
    [HideInInspector] public Vector3 prefabScale;
    [HideInInspector] public bool hasPrefabScale;
    // Never serialize these: Instantiate must not copy references into a template.
    [System.NonSerialized] public Transform[] Transforms;
    [System.NonSerialized] public Renderer[] Renderers;
    [System.NonSerialized] public Collider[] Colliders;
    [System.NonSerialized] public Rigidbody Body;
    public void CacheComponents(){
        if(Transforms!=null)return;
        Transforms=GetComponentsInChildren<Transform>(true);
        Renderers=GetComponentsInChildren<Renderer>(true);
        Colliders=GetComponentsInChildren<Collider>(true);
        Body=GetComponent<Rigidbody>();
    }
    public Renderer[] EnabledRenderers(){
        CacheComponents();
        foreach(var r in Renderers)if(!r||!r.enabled)return System.Array.FindAll(Renderers,r=>r&&r.enabled);
        return Renderers;
    }
    public void InvalidateComponents(){Transforms=null;Renderers=null;Colliders=null;Body=null;}
}
