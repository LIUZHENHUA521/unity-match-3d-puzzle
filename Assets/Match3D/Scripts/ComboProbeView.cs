using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public sealed class ComboProbeView : MonoBehaviour {
    ComboProbeController owner;
    Canvas canvas;
    RectTransform root;
    Text comboText;
    public ComboProbeChanceFeedback Chances { get; private set; }
    readonly RawImage[,] models = new RawImage[4, 3];
    readonly ComboProbeSlot[] slots = new ComboProbeSlot[4];
    readonly ComboProbeIcons icons = new ComboProbeIcons();
    ComboProbeAssets assets;
    SkeletonGraphic ring;
    int revision = -1, currentColor = -2;
    public ComboProbePreview Preview { get; private set; }
    public RectTransform Root => root;
    public SkeletonGraphic Ring => ring;
    public ComboProbeSlot Slot(int index) => slots[index];
    public float PanelWorldDepth => 1.4f;

    public void Initialize(ComboProbeController controller) {
        owner = controller; assets = ComboProbeAssets.Load();
        if (!assets || !assets.ring) throw new System.InvalidOperationException("Combo probe assets must be built before playing");
        var go = new GameObject("Four probe stacks", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
        go.layer = 5; go.transform.SetParent(transform, false); root = (RectTransform)go.transform;
        root.sizeDelta = new Vector2(GameplayBoardLayout.ProbeWidth, GameplayBoardLayout.ProbeHeight);
        canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.WorldSpace; canvas.sortingOrder = 25; canvas.worldCamera = Camera.main;
        var block = go.AddComponent<Image>(); block.color = Color.clear; block.raycastTarget = true;
        var blocker = go.AddComponent<Button>(); blocker.transition = Selectable.Transition.None; blocker.targetGraphic = block;
        // Scale the entire blue section together, keeping its left edge tucked under the badge.
        // The raised title is outside the slot body: align the BODY center with the red disc.
        var panel = new GameObject("Compact blue section", typeof(RectTransform)).GetComponent<RectTransform>();
        panel.gameObject.layer = 5; panel.SetParent(root, false);
        panel.anchoredPosition = new Vector2(-201, 17); panel.localScale = Vector3.one * .9f;
        panel.sizeDelta = new Vector2(558, 167);
        Picture("Blue four-slot panel", assets.panel, panel, new Vector2(279, 0), new Vector2(558, 167));
        // crazyCombo main.scene/newCombeNode: keep its authored local coordinates together.
        // The 134-high track is a cropped arc: its image center is ABOVE the circle center.
        var badge = new GameObject("Authored combo badge", typeof(RectTransform)).GetComponent<RectTransform>();
        badge.gameObject.layer = 5; badge.SetParent(root, false);
        badge.anchoredPosition = new Vector2(-220, -3); badge.sizeDelta = new Vector2(151, 151);
        badge.localScale = Vector3.one * (121f / 151);
        Picture("Counter circle", assets.circle, badge, Vector2.zero, new Vector2(151, 151));
        ring = SkeletonGraphic.NewSkeletonGraphicGameObject(assets.ring, badge, assets.ringMaterial);
        ring.name = "Authored breathing Spine ring"; ring.raycastTarget = false;
        ring.rectTransform.anchoredPosition = Vector2.zero; ring.transform.localScale = Vector3.one * 1.24f;
        Picture("Inactive track", assets.track, badge, new Vector2(.073f, 15.811f), new Vector2(160, 134) * 1.27f);
        Label("Counter label", "连消数", badge, new Vector2(4.137f, 32.124f), new Vector2(110, 50.4f), 30, Color.white);
        comboText = Label("Counter", "0", badge, new Vector2(1.233f, -22.303f), new Vector2(120, 88.2f), 60, new Color32(253, 251, 172, 255));
        Vector2[] starPositions = { new Vector2(-88.646f, -30.912f), new Vector2(-1.736f, 88.811f), new Vector2(87.397f, -31.31f) };
        Chances = badge.gameObject.AddComponent<ComboProbeChanceFeedback>(); Chances.Initialize(assets, starPositions);
        Label("Interaction hint", "短按取物 · 长按查看", panel, new Vector2(299, 63), new Vector2(340, 28), 19, Color.white);
        for (int i = 0; i < 4; i++) {
            var cell = new GameObject("Probe stack " + (i + 1), typeof(RectTransform), typeof(Image), typeof(ComboProbeSlot));
            cell.layer = 5; cell.transform.SetParent(panel, false);
            // Centers measured from the supplied 507 x 152 panel texture, not guessed spacing.
            var rect = (RectTransform)cell.transform; rect.sizeDelta = new Vector2(90, 94);
            rect.anchoredPosition = new Vector2((121 + 103 * i) * 558f / 507, (76 - 95) * 167f / 152);
            var hit = cell.GetComponent<Image>(); hit.color = Color.clear;
            // Add bottom visuals first: only the top model owns input.
            for (int layer = 0; layer < 3; layer++) {
                var visual = new GameObject("Model layer " + layer, typeof(RectTransform), typeof(RawImage));
                visual.layer = 5; visual.transform.SetParent(cell.transform, false);
                var r = (RectTransform)visual.transform; r.sizeDelta = new Vector2(86, 86);
                models[i, layer] = visual.GetComponent<RawImage>(); models[i, layer].raycastTarget = false;
            }
            slots[i] = cell.GetComponent<ComboProbeSlot>(); slots[i].Initialize(owner, this, i); slots[i].targetGraphic = hit;
        }
        Preview = gameObject.AddComponent<ComboProbePreview>();
        Layout(); Refresh();
    }
    static Image Picture(string name, Sprite sprite, Transform parent, Vector2 position, Vector2 size) {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image)); go.layer = 5; go.transform.SetParent(parent, false);
        var rect = (RectTransform)go.transform; rect.anchoredPosition = position; rect.sizeDelta = size;
        var image = go.GetComponent<Image>(); image.sprite = sprite; image.raycastTarget = false; return image;
    }
    static Text Label(string name, string value, Transform parent, Vector2 position, Vector2 size, int fontSize, Color color) {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text)); go.layer = 5; go.transform.SetParent(parent, false);
        var rect = (RectTransform)go.transform; rect.anchoredPosition = position; rect.sizeDelta = size;
        var text = go.GetComponent<Text>(); text.font = Match3DDefaultFont.Current; text.text = value; text.fontSize = fontSize;
        text.color = color; text.alignment = TextAnchor.MiddleCenter; text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Overflow; text.verticalOverflow = VerticalWrapMode.Overflow; return text;
    }
    void Layout() {
        var camera = Camera.main; if (!root || !camera) return;
        canvas.worldCamera = camera;
        root.SetPositionAndRotation(GameplayBoardLayout.Point(camera, 0, GameplayBoardLayout.ProbeY(camera), PanelWorldDepth), camera.transform.rotation);
        root.localScale = Vector3.one * (GameplayBoardLayout.WorldPerPoint(camera) * GameplayBoardLayout.RegionScale(camera));
    }
    public void Refresh() {
        if (!owner || !root) return;
        bool visible = owner.Running && Match3DFlow.Instance && Match3DFlow.Instance.ScreenName == "game";
        root.gameObject.SetActive(visible); if (!visible) { if (Preview) Preview.Clear(); return; }
        if (revision == owner.State.Revision) return;
        revision = owner.State.Revision; comboText.text = owner.State.Combo.ToString();
        Chances.SetState(owner.State.Stars, owner.State.Combo);
        int color = owner.State.Combo == 0 ? -1 : Mathf.Min(owner.State.Combo - 1, 4);
        if (currentColor != color) {
            currentColor = color; ring.gameObject.SetActive(color >= 0);
            if (color >= 0) { string[] names = { "Blue circle", "Purple circle", "Orange circle", "red circle", "colored circle" }; ring.AnimationState.SetAnimation(0, names[color], true); }
        }
        for (int column = 0; column < 4; column++) {
            var stack = owner.State.Column(column);
            int shown = Mathf.Min(stack.Count, 3);
            for (int layer = 0; layer < 3; layer++) {
                var model = models[column, layer]; bool show = layer < shown; model.gameObject.SetActive(show);
                if (!show) continue;
                model.texture = icons.For(owner.Target(stack[stack.Count - shown + layer]));
                int depth = shown - layer - 1;
                model.rectTransform.anchoredPosition = new Vector2(0, depth * 4);
                model.color = new Color(1, 1, 1, depth == 0 ? 1 : .72f);
            }
        }
    }
    public void ResetView() {
        if (Chances) Chances.ResetImmediate();
        if (Preview) Preview.Clear(); foreach (var slot in slots) if (slot) slot.Cancel();
        revision = -1; currentColor = -2;
    }
    void LateUpdate() { Layout(); Refresh(); }
    void OnDisable() { ResetView(); }
    void OnDestroy() { icons.Dispose(); }
}
