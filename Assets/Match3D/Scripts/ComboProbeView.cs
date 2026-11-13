using Spine.Unity;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class ComboProbeView : MonoBehaviour {
    ComboProbeController owner;
    Canvas canvas;
    RectTransform root;
    Text comboText, hintText;
    readonly Image[] slotHighlights = new Image[4];
    bool highlightedFirstReward;
    int highlightedColumn = -1;
    float highlightElapsed;
    public string HintText => hintText ? hintText.text : "";
    public ComboProbeChanceFeedback Chances { get; private set; }
    public ComboProbeRewardFeedback Rewards { get; private set; }
    readonly long[,] displayedTokens = new long[4,3];
    int rewardRevision = -1;
    readonly RawImage[,] models = new RawImage[4, 3];
    const float ModelSize = 86f;
    const float TopModelSizeMultiplier = 1.5f;
    readonly ComboProbeSlot[] slots = new ComboProbeSlot[4];
    readonly ComboProbeIcons icons = new ComboProbeIcons();
    ComboProbeAssets assets;
    SkeletonGraphic ring;
    const float RingFadeDuration = .3f;
    float ringFadeRemaining;
    int revision = -1, currentColor = -2;
    public PotComboPraise Praise { get; private set; }
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
        var artwork = new GameObject("Full width probe artwork", typeof(RectTransform)).GetComponent<RectTransform>();
        artwork.gameObject.layer = 5; artwork.SetParent(root, false);
        artwork.anchoredPosition = new Vector2(4, 0); artwork.localScale = Vector3.one * GameplayBoardLayout.ProbeArtScale;
        // Scale the entire blue section together, keeping its left edge tucked under the badge.
        // The raised title is outside the slot body: align the BODY center with the red disc.
        var panel = new GameObject("Compact blue section", typeof(RectTransform)).GetComponent<RectTransform>();
        panel.gameObject.layer = 5; panel.SetParent(artwork, false);
        panel.anchoredPosition = new Vector2(-201, 17); panel.localScale = Vector3.one * .9f;
        panel.sizeDelta = new Vector2(558, 167);
        Picture("Blue four-slot panel", assets.panel, panel, new Vector2(279, 0), new Vector2(558, 167));
        // crazyCombo main.scene/newCombeNode: keep its authored local coordinates together.
        // The 134-high track is a cropped arc: its image center is ABOVE the circle center.
        var badge = new GameObject("Authored combo badge", typeof(RectTransform)).GetComponent<RectTransform>();
        badge.gameObject.layer = 5; badge.SetParent(artwork, false);
        badge.anchoredPosition = new Vector2(-220, -3); badge.sizeDelta = new Vector2(151, 151);
        badge.localScale = Vector3.one * (121f / 151);
        Picture("Counter circle", assets.circle, badge, Vector2.zero, new Vector2(151, 151));
        ring = SkeletonGraphic.NewSkeletonGraphicGameObject(assets.ring, badge, assets.ringMaterial);
        ring.name = "Authored breathing Spine ring"; ring.raycastTarget = false;
        ring.rectTransform.anchoredPosition = Vector2.zero; ring.transform.localScale = Vector3.one * 1.24f;
        Picture("Inactive track", assets.track, badge, new Vector2(.073f, 15.811f), new Vector2(160, 134) * 1.27f);
        Label("Counter label", "连消数", badge, new Vector2(0, 32.124f), new Vector2(110, 50.4f), 30, Color.white).fontStyle = FontStyle.Bold;
        comboText = Label("Counter", "0", badge, new Vector2(0, -22.303f), new Vector2(120, 88.2f), 60, new Color32(253, 251, 172, 255));
        Vector2[] starPositions = { new Vector2(-88.646f, -30.912f), new Vector2(-1.736f, 88.811f), new Vector2(87.397f, -31.31f) };
        Chances = badge.gameObject.AddComponent<ComboProbeChanceFeedback>(); Chances.Initialize(assets, starPositions);
        // Center within the raised tab itself (source texture x=253, y=23.5).
        hintText = Label("Interaction hint", "", panel, new Vector2(253 * 558f / 507, (76 - 23.5f) * 167 / 152), new Vector2(260, 28), 19, Color.white);
        hintText.fontStyle = FontStyle.Bold;
        hintText.resizeTextForBestFit = true; hintText.resizeTextMinSize = 15; hintText.resizeTextMaxSize = 19;
        hintText.horizontalOverflow = HorizontalWrapMode.Wrap; hintText.verticalOverflow = VerticalWrapMode.Truncate;
        for (int i = 0; i < 4; i++) {
            var cell = new GameObject("Probe stack " + (i + 1), typeof(RectTransform), typeof(Image), typeof(ComboProbeSlot));
            cell.layer = 5; cell.transform.SetParent(panel, false);
            // Centers measured from the supplied 507 x 152 panel texture, not guessed spacing.
            var rect = (RectTransform)cell.transform; rect.sizeDelta = new Vector2(90, 94);
            rect.anchoredPosition = new Vector2((121 + 103 * i) * 558f / 507, (76 - 95) * 167f / 152);
            var hit = cell.GetComponent<Image>(); hit.color = Color.clear; slotHighlights[i] = hit;
            // Add bottom visuals first: only the top model owns input.
            for (int layer = 0; layer < 3; layer++) {
                var visual = new GameObject("Model layer " + layer, typeof(RectTransform), typeof(RawImage));
                visual.layer = 5; visual.transform.SetParent(cell.transform, false);
                var r = (RectTransform)visual.transform; r.sizeDelta = Vector2.one * ModelSize;
                models[i, layer] = visual.GetComponent<RawImage>(); models[i, layer].raycastTarget = false;
            }
            slots[i] = cell.GetComponent<ComboProbeSlot>(); slots[i].Initialize(owner, this, i); slots[i].targetGraphic = hit;
        }
        Preview = gameObject.AddComponent<ComboProbePreview>();
        Rewards = gameObject.AddComponent<ComboProbeRewardFeedback>(); Rewards.Initialize(owner, this, assets);
        Praise = gameObject.AddComponent<PotComboPraise>(); Praise.Initialize(root);
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
        root.localScale = Vector3.one * GameplayBoardLayout.WorldPerPoint(camera);
    }
    public void Refresh() {
        if (!owner || !root) return;
        bool visible = owner.Running && Match3DFlow.Instance && (Match3DFlow.Instance.ScreenName == "game" || owner.ReplayMode);
        root.gameObject.SetActive(visible); if (!visible) { if (Preview) Preview.Clear(); return; }
        // Successful probe use is learning state, independent of the board revision.
        RefreshHint();
        if (revision == owner.State.Revision && rewardRevision == Rewards.Revision) return;
        rewardRevision = Rewards.Revision;
        revision = owner.State.Revision; comboText.text = owner.State.Combo.ToString();
        Chances.SetState(owner.State.Stars, owner.State.Combo);
        int color = owner.State.Combo == 0 ? -1 : Mathf.Min(owner.State.Combo - 1, 4);
        if (currentColor != color) {
            bool wasActive = currentColor >= 0;
            currentColor = color;
            if (color >= 0) {
                // A new combo cancels the outgoing fade before restoring its animation.
                ringFadeRemaining = 0; SetRingAlpha(1); ring.gameObject.SetActive(true);
                string[] names = { "Blue circle", "Purple circle", "Orange circle", "red circle", "colored circle" };
                ring.AnimationState.SetAnimation(0, names[color], true);
            } else if (wasActive) ringFadeRemaining = RingFadeDuration;
            else { ringFadeRemaining = 0; SetRingAlpha(0); ring.gameObject.SetActive(false); }
        }
        for (int column = 0; column < 4; column++) {
            var stack = owner.State.Column(column).Where(token => !Rewards.IsPending(token)).ToList();
            int shown = Mathf.Min(stack.Count, 3);
            for (int layer = 0; layer < 3; layer++) {
                var model = models[column, layer]; bool show = layer < shown; model.gameObject.SetActive(show);
                if (!show) continue;
                displayedTokens[column,layer] = stack[stack.Count - shown + layer];
                model.texture = icons.For(owner.Target(displayedTokens[column,layer]));
                int depth = shown - layer - 1;
                // Keep the top item readable without changing the reveal animation's scale.
                model.rectTransform.sizeDelta = Vector2.one * ModelSize * (depth == 0 ? TopModelSizeMultiplier : 1f);
                model.rectTransform.anchoredPosition = new Vector2(0, depth * 4);
                model.color = new Color(1, 1, 1, depth == 0 ? 1 : .72f);
            }
        }
    }
    void SetRingAlpha(float alpha) {
        var color = ring.color; color.a = alpha; ring.color = color;
    }
    void Update() {
        if (ringFadeRemaining <= 0 || !root || !root.gameObject.activeInHierarchy || Time.deltaTime <= 0) return;
        ringFadeRemaining = Mathf.Max(0, ringFadeRemaining - Time.deltaTime);
        float t = ringFadeRemaining / RingFadeDuration;
        SetRingAlpha(t * t * (3 - 2 * t));
        if (ringFadeRemaining == 0) ring.gameObject.SetActive(false);
    }
    void RefreshHint() {
        var state = owner.State;
        string message = state.Stars == 1 ? "最后1次取物，消除可续连" :
            state.Count > 0 ? (owner.HasUsedProbe ? "点击取物 · 长按定位" : "点击模型，取出被挡物品") :
            state.Combo == 0 ? "先消一组，点亮三星" : state.Stars + "次取物内再消除，获得探物";
        if (hintText.text != message) hintText.text = message;
        if (!highlightedFirstReward && !owner.HasUsedProbe && state.Count > 0) {
            for (int i = 0; i < 4; i++) if (state.Top(i) != 0 && !Rewards.IsPending(state.Top(i))) {
                highlightedFirstReward = true; highlightElapsed = 0; highlightedColumn = i; break;
            }
        }
    }
    void AnimateHint() {
        if (highlightedColumn < 0 || !root.gameObject.activeInHierarchy) return;
        highlightElapsed += Time.deltaTime;
        var highlight = slotHighlights[highlightedColumn];
        if (owner.HasUsedProbe || owner.State.Top(highlightedColumn) == 0 || highlightElapsed >= 2.4f) {
            highlight.color = Color.clear; highlightedColumn = -1; return;
        }
        float pulse = Mathf.Sin(highlightElapsed / 1.2f * Mathf.PI);
        highlight.color = new Color(1, .88f, .35f, .22f * pulse * pulse);
    }
    public void ResetView() {
        ringFadeRemaining = 0;
        if (Praise) Praise.ResetImmediate();
        if (ring) { SetRingAlpha(0); ring.gameObject.SetActive(false); }
        if (Rewards) Rewards.ResetImmediate(); rewardRevision = -1;
        icons.Dispose(); // A new stage/theme must capture its current scene lighting again.
        if (Chances) Chances.ResetImmediate();
        if (Preview) Preview.Clear(); foreach (var slot in slots) if (slot) slot.Cancel();
        revision = -1; currentColor = -2;
        highlightedFirstReward = false; highlightedColumn = -1; highlightElapsed = 0;
        foreach (var highlight in slotHighlights) if (highlight) highlight.color = Color.clear;
    }
    void LateUpdate() {
        if(owner&&owner.ReplayMode)return;
        Layout(); Refresh(); AnimateHint();
        if (Rewards) for (int column=0;column<4;column++) for (int layer=0;layer<3;layer++)
            if (models[column,layer] && models[column,layer].gameObject.activeSelf)
                models[column,layer].rectTransform.localScale = Vector3.one * Rewards.RevealScale(displayedTokens[column,layer]);
    }
    public void CaptureReplay(PotReplayPresentation snapshot){
        snapshot.chance=Chances.CaptureReplay();snapshot.rewards=Rewards.CaptureReplay();
        snapshot.firstHint=highlightedFirstReward;snapshot.highlightColumn=highlightedColumn;snapshot.hintAge=highlightElapsed;
        snapshot.ring=ring.AnimationState.GetCurrent(0)?.Animation.Name??"";snapshot.ringVisible=ring.gameObject.activeSelf;snapshot.ringAlpha=ring.color.a;snapshot.ringTime=ring.AnimationState.GetCurrent(0)?.TrackTime??0;
        Praise.CaptureReplay(snapshot);Preview.CaptureReplay(snapshot);
    }
    public void ApplyReplay(PotReplayPresentation snapshot,System.Collections.Generic.Dictionary<int,GameObject> objects,float offset){
        Layout();Rewards.ApplyReplay(snapshot.rewards,objects,offset);revision=-1;Refresh();
        Chances.ApplyReplay(snapshot.chance,snapshot.combo);
        ring.gameObject.SetActive(snapshot.ringVisible);SetRingAlpha(snapshot.ringAlpha);
        var track=ring.AnimationState.GetCurrent(0);if(snapshot.ring!=""&&(track==null||track.Animation.Name!=snapshot.ring))track=ring.AnimationState.SetAnimation(0,snapshot.ring,true);if(track!=null){track.TrackTime=snapshot.ringTime+offset;ring.Update(0);}
        highlightedFirstReward=snapshot.firstHint;highlightedColumn=snapshot.highlightColumn;highlightElapsed=snapshot.hintAge+offset;
        foreach(var h in slotHighlights)h.color=Color.clear;
        if(highlightedColumn>=0){float pulse=Mathf.Sin(highlightElapsed/1.2f*Mathf.PI);slotHighlights[highlightedColumn].color=new Color(1,.88f,.35f,.22f*pulse*pulse);}
        for(int column=0;column<4;column++)for(int layer=0;layer<3;layer++)if(models[column,layer].gameObject.activeSelf)models[column,layer].rectTransform.localScale=Vector3.one*Rewards.RevealScale(displayedTokens[column,layer]);
        Praise.ApplyReplay(snapshot,offset);Preview.ApplyReplay(snapshot,objects,offset);
    }
    void OnDisable() { ResetView(); }
    void OnDestroy() { icons.Dispose(); }
}
