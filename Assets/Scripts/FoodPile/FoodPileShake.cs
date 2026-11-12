using UnityEngine;
using UnityEngine.UI;

// Retained for serialized legacy scenes: only the remaining-item label is active.
public class FoodPileShake : MonoBehaviour
{
    public Button button;
    public Text remainingText;
    void Awake(){if(button)button.gameObject.SetActive(false);}
    // Existing scene UnityEvents may still reference Shake; tossing is removed.
    public void Shake(){}
    private void Update()
    {
        if (remainingText) remainingText.text = "ITEMS LEFT  " + LevelGenerator.Instance.objectsOnScene.Count + " / " + (LevelGenerator.Instance.foodPile.prefabs.Length * LevelGenerator.Instance.foodPile.copiesPerType);
    }
}
