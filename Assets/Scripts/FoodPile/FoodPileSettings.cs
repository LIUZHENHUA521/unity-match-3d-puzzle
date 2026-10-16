using UnityEngine;

[CreateAssetMenu(menuName = "Food Pile/Challenge Settings")]
public class FoodPileSettings : ScriptableObject
{
    public GameObject[] prefabs;
    [Min(3)] public int copiesPerType = 12;
    [Min(30)] public float timeLimit = 600;
    public int seed = 20261013;
    public bool IsValid => prefabs != null && prefabs.Length == 8 &&
        System.Array.TrueForAll(prefabs, p => p != null) && copiesPerType > 0 && copiesPerType % 3 == 0;
}
