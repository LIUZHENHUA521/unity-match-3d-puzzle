using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFitter : MonoBehaviour
{
    public bool portraitViewport;
    private Camera view;
    private Renderer pot;
    private void Awake() { view = GetComponent<Camera>(); Fit(); }
    private void LateUpdate() { Fit(); }
    private void Fit()
    {
        if (portraitViewport) {
            // Fit the pot body to the actual screen, without a 9:16 pillarbox.
            if (!pot) pot = GameObject.Find("Cooking Pot")?.GetComponent<Renderer>();
            view.rect = new Rect(0, 0, 1, 1);
            float width = pot ? pot.bounds.size.x : 2.36f;
            view.orthographicSize = width / (2 * view.aspect);
        } else {
            view.orthographicSize = 2.4f * Mathf.Max(1, (1080f / 1920f) / view.aspect);
        }
    }
}
