using System;
using System.Collections.Generic;
using UnityEngine;

// Runtime bridge for editor-generated model adapters. Legacy source JSON remains
// authoritative unless an adapter entry with the same logical ID exists.
public static class ReplicaSourceCatalog {
    public const int CurrentVersion = 1;

    [Serializable] public sealed class Data { public int version; public Item[] items; }
    [Serializable] public sealed class Item {
        public string id;
        public string displayName;
        public string prefabPath;
        public float normalizedToSource;
        public ColliderProfile colliderProfile;
        public AudioProfile audioProfile;
    }
    [Serializable] public sealed class ColliderProfile {
        public string name;
        public int[] col;
        public int[] realCol;
        public ColliderNode[] nodes;
    }
    [Serializable] public sealed class ColliderNode {
        public int[] indices;
        public int layer;
        public ColliderShape[] colliders;
    }
    [Serializable] public sealed class ColliderShape {
        public string type;
        public bool enabled;
        public bool trigger;
        public bool hasMaterial;
        public Vector3 center;
        public Vector3 size;
        public float radius;
        public float height;
        public int direction;
        public Surface material;
    }
    [Serializable] public sealed class Surface {
        public string m_Name;
        public float dynamicFriction;
        public float staticFriction;
        public float bounciness;
        public int frictionCombine;
        public int bounceCombine;
    }
    [Serializable] public sealed class AudioProfile {
        public string name;
        public int select;
        public int combine;
        public bool usePitch;
        public bool combineDefault;
        public bool goose;
    }

    static Data data;
    static Dictionary<string, Item> items;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnSubsystem() => ClearCache();

    public static void ClearCache() { data = null; items = null; }

    static void Load() {
        if (items != null) return;
        items = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
        var text = Resources.Load<TextAsset>("Replica/ModelAdapterCatalog");
        if (!text) return;
        try {
            data = JsonUtility.FromJson<Data>(text.text);
            if (data == null || data.version != CurrentVersion) {
                Debug.LogWarning("ModelAdapterCatalog version mismatch; ignoring catalog.");
                return;
            }
            foreach (var item in data.items ?? Array.Empty<Item>()) {
                if (item == null || string.IsNullOrWhiteSpace(item.id)) continue;
                if (items.ContainsKey(item.id)) Debug.LogWarning("Duplicate model adapter ID; later entry wins: " + item.id);
                items[item.id] = item;
            }
        } catch (Exception ex) {
            Debug.LogError("Failed to load ModelAdapterCatalog: " + ex.Message);
        }
    }

    public static bool TryGet(string id, out Item item) {
        Load();
        item = null;
        return !string.IsNullOrEmpty(id) && items.TryGetValue(id, out item);
    }
    public static bool TryGetSize(string id, out float normalizedToSource) {
        normalizedToSource = 0;
        if (!TryGet(id, out var item) || item.normalizedToSource <= 0) return false;
        normalizedToSource = item.normalizedToSource;
        return true;
    }
    public static bool TryGetColliderProfile(string id, out ColliderProfile profile) {
        profile = null;
        return TryGet(id, out var item) && (profile = item.colliderProfile) != null;
    }
    public static bool TryGetAudioProfile(string id, out AudioProfile profile) {
        profile = null;
        return TryGet(id, out var item) && (profile = item.audioProfile) != null;
    }
    public static string[] GetIds() {
        Load();
        var ids = new string[items.Count];
        items.Keys.CopyTo(ids, 0);
        return ids;
    }
}
