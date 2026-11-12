using UnityEngine;

// A pool checkout always receives a new token. Model/type IDs cannot identify a physical item.
public sealed class ComboProbeTarget : MonoBehaviour {
    static long sequence;
    public long Token { get; private set; }
    public int Generation { get; private set; }
    public GameObject Prefab { get; private set; }
    public void Assign(int generation, GameObject prefab) {
        Token = ++sequence; Generation = generation; Prefab = prefab;
    }
}
