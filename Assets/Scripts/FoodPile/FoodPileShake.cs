using UnityEngine;
using UnityEngine.UI;

public class FoodPileShake : MonoBehaviour
{
    public Button button;
    public Text remainingText;
    private float nextShake;
    public bool CanShakeGameplay=>GameManager.Instance&&GameManager.Instance.GameIsPlaying()&&Time.timeScale>0&&!BoardTools.Shuffling&&
        (!Match3DFlow.Instance||(Match3DFlow.Instance.ScreenName=="game"&&!Match3DFlow.Instance.BlocksGameplay&&!Match3DFlow.Instance.CreationInProgress));
    public void Shake(){TryShake();}
    public bool TryShake()
    {
        if (!CanShakeGameplay || Time.unscaledTime < nextShake) return false;
        nextShake = Time.unscaledTime + 2f; // Legacy button/Space toss only; phone input has no cooldown.
        if(InputController.Instance)InputController.Instance.CancelSelection();
        if(ItemPhysicsController.Instance){ItemPhysicsController.Instance.ShakeAll();return true;}
        foreach (var obj in LevelGenerator.Instance.objectsOnScene)
        {
            if (!obj || !obj.activeInHierarchy || BoardTools.IsStored(obj) || (InputController.Instance&&InputController.Instance.objectsOnStack.Contains(obj))) continue;
            var body = obj.GetComponent<Rigidbody>();
            if (!body || body.isKinematic) continue;
            // Impulse toward the tray's centre keeps the pile contained.
            var inward = new Vector3(-obj.transform.position.x, 0, .15f-obj.transform.position.z);
            body.velocity = Vector3.ClampMagnitude(body.velocity, 1);
            body.AddForce(inward * .3f + Vector3.up * 1.9f, ForceMode.VelocityChange);
            body.AddTorque(Random.insideUnitSphere * 2, ForceMode.VelocityChange);
        }
        return true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Shake();
        if (button) button.interactable = CanShakeGameplay && Time.unscaledTime >= nextShake;
        if (remainingText) remainingText.text = "ITEMS LEFT  " + LevelGenerator.Instance.objectsOnScene.Count + " / " + (LevelGenerator.Instance.foodPile.prefabs.Length * LevelGenerator.Instance.foodPile.copiesPerType);
    }
}
