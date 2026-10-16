using UnityEngine;
using UnityEngine.UI;

public class FoodPileShake : MonoBehaviour
{
    public Button button;
    public Text remainingText;
    private float nextShake;
    public void Shake()
    {
        if (!GameManager.Instance.GameIsPlaying() || Time.time < nextShake || (ReplicaFlow.Instance && ReplicaFlow.Instance.BlocksGameplay)) return;
        nextShake = Time.time + 2;
        if(ReplicaPilePhysics.Instance){ReplicaPilePhysics.Instance.ShakeAll();return;}
        foreach (var obj in LevelGenerator.Instance.objectsOnScene)
        {
            if (!obj) continue;
            var body = obj.GetComponent<Rigidbody>();
            if (!body || body.isKinematic) continue;
            // Impulse toward the tray's centre keeps the pile contained.
            var inward = new Vector3(-obj.transform.position.x, 0, .15f-obj.transform.position.z);
            body.velocity = Vector3.ClampMagnitude(body.velocity, 1);
            body.AddForce(inward * .3f + Vector3.up * 1.9f, ForceMode.VelocityChange);
            body.AddTorque(Random.insideUnitSphere * 2, ForceMode.VelocityChange);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Shake();
        if (button) button.interactable = GameManager.Instance.GameIsPlaying() && Time.time >= nextShake;
        if (remainingText) remainingText.text = "ITEMS LEFT  " + LevelGenerator.Instance.objectsOnScene.Count + " / " + (LevelGenerator.Instance.foodPile.prefabs.Length * LevelGenerator.Instance.foodPile.copiesPerType);
    }
}
