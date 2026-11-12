using UnityEngine;
using UnityEngine.UI;

// Visual feedback only: the accepted-pick state remains authoritative during every animation.
public sealed class ComboProbeChanceFeedback : MonoBehaviour {
    readonly Image[] lit = new Image[3], flashes = new Image[3];
    readonly float[] elapsed = new float[3], fromAlpha = new float[3], fromScale = new float[3];
    readonly bool[] targets = new bool[3];
    ComboProbeArcGraphic fill;
    int stars = -1, combo = -1;
    float progressFrom, progressTo, progressElapsed, progressDuration;
    public float Progress => fill ? fill.fillAmount : 0;
    public float StarAlpha(int i) => lit[i].color.a;
    public float StarScale(int i) => lit[i].rectTransform.localScale.x;
    public float FlashAlpha(int i) => flashes[i].color.a;

    public void Initialize(ComboProbeAssets assets, Vector2[] positions) {
        var arc = new GameObject("Golden chance progress", typeof(RectTransform), typeof(ComboProbeArcGraphic));
        arc.layer = 5; arc.transform.SetParent(transform, false);
        fill = arc.GetComponent<ComboProbeArcGraphic>(); fill.Initialize(assets.sunshine);
        fill.rectTransform.anchoredPosition = new Vector2(.073f, 15.811f);
        fill.rectTransform.sizeDelta = new Vector2(154, 128) * 1.27f;
        for (int i = 0; i < 3; i++) {
            var p = positions[i] + new Vector2(1.47f, 0);
            Picture("Chance dark " + i, assets.darkStar, transform, p, new Vector2(45.8f, 45));
            flashes[i] = Picture("Chance flash " + i, assets.starFlash, transform, p, new Vector2(76, 75));
            lit[i] = Picture("Chance lit " + i, assets.lightStar, transform, p, new Vector2(45.8f, 45));
        }
        ResetImmediate();
    }
    static Image Picture(string name, Sprite sprite, Transform parent, Vector2 position, Vector2 size) {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image)); go.layer = 5; go.transform.SetParent(parent, false);
        var rect = (RectTransform)go.transform; rect.anchoredPosition = position; rect.sizeDelta = size;
        var image = go.GetComponent<Image>(); image.sprite = sprite; image.raycastTarget = false; return image;
    }
    static float FillFor(int count) => count == 3 ? .75f : count == 2 ? .38f : count == 1 ? .08f : 0;
    public void SetState(int count, int nextCombo) {
        count = Mathf.Clamp(count, 0, 3);
        if (count == stars && nextCombo == combo) return;
        bool immediate = stars < 0;
        bool refill = nextCombo > combo && count == 3 && !immediate;
        progressFrom = Progress; progressTo = FillFor(count); progressElapsed = 0;
        progressDuration = immediate ? 0 : progressTo > progressFrom ? .34f : .22f;
        if (immediate) fill.fillAmount = progressTo;
        for (int i = 0; i < 3; i++) {
            bool on = i < count;
            if (!immediate && on == targets[i] && !(on && refill)) continue;
            fromAlpha[i] = lit[i].color.a; fromScale[i] = lit[i].rectTransform.localScale.x;
            targets[i] = on; elapsed[i] = immediate ? 1 : 0;
            ApplyStar(i);
        }
        stars = count; combo = nextCombo;
    }
    void Update() {
        if (Time.deltaTime <= 0) return;
        if (progressElapsed < progressDuration) {
            progressElapsed = Mathf.Min(progressDuration, progressElapsed + Time.deltaTime);
            float t = progressElapsed / progressDuration;
            fill.fillAmount = Mathf.Lerp(progressFrom, progressTo, t * t * (3 - 2 * t));
        }
        for (int i = 0; i < 3; i++) if (elapsed[i] < .34f) { elapsed[i] += Time.deltaTime; ApplyStar(i); }
    }
    void ApplyStar(int i) {
        bool on = targets[i]; float t = Mathf.Clamp01(elapsed[i] / (on ? .34f : .2f));
        float ease = t * t * (3 - 2 * t);
        float alpha = Mathf.Lerp(fromAlpha[i], on ? 1 : 0, ease);
        float scale = on ? (t < .35f ? Mathf.Lerp(fromScale[i], 1.3f, t / .35f) : Mathf.Lerp(1.3f, 1, (t - .35f) / .65f)) : Mathf.Lerp(fromScale[i], .65f, ease);
        lit[i].color = new Color(1, 1, 1, alpha); lit[i].rectTransform.localScale = Vector3.one * scale;
        flashes[i].color = new Color(1, 1, 1, on && t < 1 ? Mathf.Sin(t * Mathf.PI) * .8f : 0);
        flashes[i].rectTransform.localScale = Vector3.one * Mathf.Lerp(.8f, 1.2f, t);
    }
    public void ResetImmediate() {
        stars = combo = -1; progressFrom = progressTo = progressElapsed = progressDuration = 0;
        if (fill) fill.fillAmount = 0;
        for (int i = 0; i < 3; i++) {
            targets[i] = false; elapsed[i] = 1; fromAlpha[i] = 0; fromScale[i] = 1;
            if (lit[i]) { lit[i].color = new Color(1, 1, 1, 0); lit[i].rectTransform.localScale = Vector3.one; }
            if (flashes[i]) flashes[i].color = new Color(1, 1, 1, 0);
        }
    }
}
