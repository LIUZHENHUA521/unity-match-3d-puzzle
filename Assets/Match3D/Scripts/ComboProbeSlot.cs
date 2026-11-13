using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class ComboProbeSlot : Selectable, IPointerClickHandler, IDragHandler {
    public const float HoldSeconds = .25f;
    ComboProbeController owner;
    ComboProbeView view;
    int column, pointer;
    long token;
    float began;
    Vector2 origin;
    bool held, previewed, cancelled;
    public void Initialize(ComboProbeController controller, ComboProbeView display, int index) {
        owner = controller; view = display; column = index; transition = Transition.None;
        navigation = new Navigation { mode = Navigation.Mode.None };
    }
    public override void OnPointerDown(PointerEventData e) {
        base.OnPointerDown(e); Cancel();
        if (Input.touchCount > 1 || (BoardCameraZoom.Instance && BoardCameraZoom.Instance.SuppressPick) || !owner.CanInteract || e.button != PointerEventData.InputButton.Left) return;
        token = owner.State.Top(column); if (token == 0 || view.Rewards.IsPending(token)) return;
        held = true; pointer = e.pointerId; began = Time.unscaledTime; origin = e.position;
    }
    public override void OnPointerUp(PointerEventData e) {
        base.OnPointerUp(e); if (!held || e.pointerId != pointer) return;
        bool use = Input.touchCount <= 1 && !(BoardCameraZoom.Instance && BoardCameraZoom.Instance.SuppressPick) && !cancelled && !previewed && Time.unscaledTime - began < HoldSeconds && Valid() &&
            RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, e.position, e.pressEventCamera);
        long expected = token; Cancel(); if (use) owner.UseTop(column, expected);
    }
    public void OnPointerClick(PointerEventData e) { /* release above owns the one and only use */ }
    public void OnDrag(PointerEventData e) {
        if (!held || e.pointerId != pointer) return;
        float logicalDistance = (e.position - origin).magnitude * 720 / Mathf.Max(1, Screen.width);
        if (logicalDistance > 18) { cancelled = true; if (view.Preview) view.Preview.Clear(); }
    }
    public override void OnPointerExit(PointerEventData e) { base.OnPointerExit(e); if (held) { cancelled = true; view.Preview.Clear(); } }
    bool Valid() => owner && owner.CanInteract && owner.State.Top(column) == token && !view.Rewards.IsPending(token) && owner.Target(token);
    void Update() {
        if (!held) return;
        if (Input.touchCount > 1 || (BoardCameraZoom.Instance && BoardCameraZoom.Instance.SuppressPick) || !Valid()) { Cancel(); return; }
        if (!cancelled && !previewed && Time.unscaledTime - began >= HoldSeconds) {
            previewed = true; view.Preview.Show(owner, token, transform);
        }
    }
    public void Cancel() { held = previewed = cancelled = false; token = 0; if (view && view.Preview) view.Preview.Clear(); }
    protected override void OnDisable() { Cancel(); base.OnDisable(); }
    void OnApplicationFocus(bool focused) { if (!focused) Cancel(); }
    void OnApplicationPause(bool paused) { if (paused) Cancel(); }
}
