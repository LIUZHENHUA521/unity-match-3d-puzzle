using UnityEngine;

// Stable identity for generated models. Display text can change without changing
// the key used by level, size, collider, and audio catalogs.
public sealed class ReplicaSourceIdentity : MonoBehaviour {
    [SerializeField] string logicalId;
    [SerializeField] string displayName;
    [SerializeField] int catalogVersion = ReplicaSourceCatalog.CurrentVersion;

    public string LogicalId => logicalId;
    public string DisplayName => displayName;
    public int CatalogVersion => catalogVersion;

    public void Configure(string id, string label, int version) {
        logicalId = id;
        displayName = label;
        catalogVersion = version;
    }
}
