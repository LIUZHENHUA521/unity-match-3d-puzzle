using UnityEngine;

// Keep the artwork camera-facing and cover the viewport without stretching it.
[RequireComponent(typeof(SpriteRenderer))]
public sealed class ReplicaCameraBackdrop : MonoBehaviour {
    public float distance = 60f;
    void OnEnable() { Camera.onPreCull += Fit; }
    void OnDisable() { Camera.onPreCull -= Fit; }
    public void Fit(Camera camera) {
        if (!camera || camera != Camera.main || !camera.orthographic) return;
        var sprite = GetComponent<SpriteRenderer>().sprite;
        if (!sprite) return;
        float height = camera.orthographicSize * 2;
        float scale = Mathf.Max(height * camera.aspect / sprite.bounds.size.x, height / sprite.bounds.size.y);
        transform.SetPositionAndRotation(camera.transform.position + camera.transform.forward * distance, camera.transform.rotation);
        transform.localScale = Vector3.one;
        var parentScale = transform.lossyScale;
        transform.localScale = new Vector3(scale / parentScale.x, scale / parentScale.y, scale / parentScale.z);
    }
}
