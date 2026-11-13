using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Owns only the view; never scales or moves the physics pile.
public sealed class BoardCameraZoom : MonoBehaviour {
    public const float MinZoom = 1f, MaxZoom = 1.6f;
    public static BoardCameraZoom Instance { get; private set; }
    public float Zoom { get; private set; } = 1;
    public bool SuppressPick { get; private set; }
    public bool Available => presentation && presentation.Active && Match3DFlow.Instance &&
        !Match3DFlow.Instance.IsPvp && Match3DFlow.Instance.ScreenName == "game";
    public bool CanInteract => Available && GameManager.Instance && GameManager.Instance.GameIsPlaying() &&
        !Match3DFlow.Instance.BlocksGameplay;
    Match3DThemePresentation presentation;
    Vector2 offset, previousMidpoint;
    float previousDistance;
    int firstFinger, secondFinger;
    bool tracking, rejected, restoring, singleOnUI;
    int singleFinger = -1;
    readonly List<GameObject> foreground = new List<GameObject>();

    void Awake() { Instance = this; presentation = GetComponent<Match3DThemePresentation>(); }
    void OnDestroy() { if (Instance == this) Instance = null; }
    void OnApplicationFocus(bool focus) { if (!focus) CancelGesture(); }
    void OnApplicationPause(bool paused) { if (paused) CancelGesture(); }
    public void ResetView() { Zoom = 1; offset = Vector2.zero; restoring = false; CancelGesture(); }
    public void CancelGesture() { tracking = false; rejected = true; SuppressPick = true; }

    public void CollectForeground(List<GameObject> result) {
        result.Clear();
        if (InputController.Instance) result.AddRange(InputController.Instance.objectsOnStack);
        BoardTools.AppendStoredItems(result);
    }
    public bool MotionReady {
        get {
            if (BoardTools.Busy || (InputController.Instance && InputController.Instance.IsBusy)) return false;
            CollectForeground(foreground);
            foreach (var item in foreground) if (item && DOTween.IsTweening(item.transform)) return false;
            return true;
        }
    }
    bool OnBoard(Vector2 point) {
        var camera = Camera.main;
        if (!camera || !camera.pixelRect.Contains(point)) return false;
        float unit = 720f / Mathf.Max(1, Screen.width);
        float bottom = GameplayBoardLayout.Enabled ? GameplayBoardLayout.ProbeY(camera) + GameplayBoardLayout.ProbeHeight * .5f : 310;
        return point.y * unit > bottom + GameplayBoardLayout.SafeBottom &&
            point.y < Screen.safeArea.yMax - 180 / unit;
    }
    // Called before the normal pointer path, including when a modal has blocked play.
    public bool ProcessTouches(System.Func<Vector2, bool> onUI) {
        int count = Input.touchCount;
        return ProcessContacts(count, count > 0 ? Input.GetTouch(0) : default(Touch),
            count > 1 ? Input.GetTouch(1) : default(Touch), onUI);
    }
    public bool ProcessContacts(int count, Touch a, Touch b, System.Func<Vector2, bool> onUI) {
        if (count == 0) {
            bool consumed = SuppressPick;
            SuppressPick = tracking = rejected = singleOnUI = false; singleFinger = -1;
            return consumed || restoring; // Also swallow the emulated mouse release on this frame.
        }
        if (count > 1) SuppressPick = true;
        if (!SuppressPick) {
            if (a.fingerId != singleFinger || a.phase == TouchPhase.Began) {
                singleFinger = a.fingerId; singleOnUI = !OnBoard(a.position) || onUI(a.position);
            }
            return restoring;
        }
        if (!CanInteract || count != 2) { tracking = false; rejected = true; return true; }
        if (a.phase == TouchPhase.Ended || a.phase == TouchPhase.Canceled ||
            b.phase == TouchPhase.Ended || b.phase == TouchPhase.Canceled) { tracking = false; rejected = true; return true; }
        if (rejected) return true;
        var midpoint = (a.position + b.position) * .5f;
        float distance = Vector2.Distance(a.position, b.position);
        if (!tracking) {
            if (singleOnUI || !OnBoard(a.position) || !OnBoard(b.position) || onUI(a.position) || onUI(b.position)) { rejected = true; return true; }
            firstFinger = a.fingerId; secondFinger = b.fingerId;
            tracking = true;
        } else {
            if (a.fingerId != firstFinger || b.fingerId != secondFinger) { tracking = false; rejected = true; return true; }
            if (MotionReady && previousDistance >= 12 && distance >= 12)
                Gesture(previousMidpoint, midpoint, distance / previousDistance);
        }
        previousMidpoint = midpoint; previousDistance = distance;
        return true;
    }
    public void Gesture(Vector2 previous, Vector2 current, float ratio) {
        var camera = Camera.main; if (!camera || !CanInteract || !MotionReady) return;
        float next = Mathf.Clamp(Zoom * ratio, MinZoom, MaxZoom);
        if (Zoom <= MinZoom && next <= MinZoom) {
            restoring = offset.sqrMagnitude > 0;
            return; // At the original scale, dragging must not move the composition.
        }
        float oldSize = camera.orthographicSize, newSize = oldSize * Zoom / next;
        // Preserve the world point under the moving midpoint (orthographic projection).
        var rect = camera.pixelRect;
        Vector2 before = new Vector2((previous.x - rect.x) / rect.width - .5f, (previous.y - rect.y) / rect.height - .5f);
        Vector2 after = new Vector2((current.x - rect.x) / rect.width - .5f, (current.y - rect.y) / rect.height - .5f);
        offset += new Vector2(2 * camera.aspect * (before.x * oldSize - after.x * newSize),
            2 * (before.y * oldSize - after.y * newSize));
        Zoom = next; restoring = Zoom <= MinZoom;
    }
    public void Step(float delta) {
        if (!CanInteract || !MotionReady) return;
        if (InputController.Instance) InputController.Instance.CancelSelection();
        float next = Mathf.Clamp(Mathf.Round((Zoom + delta) * 5) / 5, MinZoom, MaxZoom);
        var camera = Camera.main;
        if (camera) Gesture(camera.pixelRect.center, camera.pixelRect.center, next / Zoom);
    }
    public void Restore() {
        if (!CanInteract || !MotionReady) return;
        if (InputController.Instance) InputController.Instance.CancelSelection();
        restoring = true;
    }
    public void Apply(Camera camera, Vector3 oldPosition, float oldSize, float deltaTime) {
        if (restoring && MotionReady) {
            Zoom = Mathf.MoveTowards(Zoom, 1, deltaTime * 3);
            offset = Vector2.Lerp(offset, Vector2.zero, 1 - Mathf.Exp(-20 * deltaTime));
            if (Zoom == 1 && offset.sqrMagnitude < .000001f) { offset = Vector2.zero; restoring = false; }
        }
        float baseSize = camera.orthographicSize;
        if (!restoring) offset = ClampOffset(offset, baseSize, camera.aspect, Zoom);
        camera.orthographicSize = baseSize / Zoom;
        camera.transform.position += camera.transform.right * offset.x + camera.transform.up * offset.y;
        if ((camera.transform.position-oldPosition).sqrMagnitude < 1e-12f && Mathf.Abs(camera.orthographicSize-oldSize) < 1e-6f) return;
        CollectForeground(foreground);
        // Settled tray/storage models use world-space positions. Preserve their screen pose
        // along with the canvases; flight tweens finish before gestures can change the view.
        foreach (var item in foreground) if (item)
            Reproject(item.transform, camera, oldPosition, oldSize);
    }
    public static Vector2 ClampOffset(Vector2 value, float size, float aspect, float zoom) {
        float margin = size * (1 - 1 / Mathf.Clamp(zoom, MinZoom, MaxZoom));
        return new Vector2(Mathf.Clamp(value.x, -margin * aspect, margin * aspect), Mathf.Clamp(value.y, -margin, margin));
    }
    public static void Reproject(Transform item, Camera camera, Vector3 oldPosition, float oldSize) {
        if (oldSize <= 0) return;
        float ratio = camera.orthographicSize / oldSize;
        var delta = item.position - oldPosition;
        var depth = camera.transform.forward * Vector3.Dot(delta, camera.transform.forward);
        item.position = camera.transform.position + depth + (delta - depth) * ratio;
        item.localScale *= ratio;
    }
}
