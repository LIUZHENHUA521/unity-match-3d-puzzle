using UnityEngine;
using UnityEngine.UI;

// One reusable, non-modal message. Real time keeps dismissal independent of gameplay pauses.
public sealed class Match3DTips : MonoBehaviour {
    public const float Duration = 1.5f;
    Text message;
    RectTransform panel;
    float hideAt;

    public static Match3DTips Create(Transform parent) {
        var view = Match3DUIView.Load("Notice", parent);
        view.name = "ShowTips";
        var tips = view.gameObject.AddComponent<Match3DTips>();
        tips.message = view.Get<Text>("Message");
        var overlay = view.gameObject.AddComponent<Canvas>();
        overlay.overrideSorting = true;
        overlay.sortingOrder = 100;
        var group = view.gameObject.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        tips.panel = new GameObject("Background", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        tips.panel.SetParent(view.transform, false);
        tips.panel.SetAsFirstSibling();
        tips.panel.anchorMin = tips.panel.anchorMax = new Vector2(.5f, .5f);
        var background = tips.panel.GetComponent<Image>();
        background.color = new Color(0, 0, 0, 108f / 255f);
        background.raycastTarget = false;
        tips.message.transform.SetParent(tips.panel, false);
        var rect = tips.message.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(24, 15);
        rect.offsetMax = new Vector2(-24, -15);
        tips.message.fontSize = 35;
        tips.message.fontStyle = FontStyle.Bold;
        tips.message.alignment = TextAnchor.MiddleCenter;
        tips.message.color = Color.white;
        tips.message.supportRichText = false;
        tips.message.raycastTarget = false;
        tips.message.horizontalOverflow = HorizontalWrapMode.Wrap;
        tips.message.verticalOverflow = VerticalWrapMode.Truncate;
        foreach (var shadow in tips.message.GetComponents<Shadow>()) shadow.enabled = false;
        tips.Hide();
        return tips;
    }

    public void Show(string value) {
        if (string.IsNullOrWhiteSpace(value)) return;
        message.text = value;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        Layout();
        hideAt = Time.realtimeSinceStartup + Duration;
    }

    void Layout() {
        float availableWidth = Mathf.Max(1, ((RectTransform)transform).rect.width - 48);
        float width = Mathf.Min(availableWidth, Mathf.Max(350, message.preferredWidth + 48));
        panel.sizeDelta = new Vector2(width, 70);
        panel.sizeDelta = new Vector2(width, Mathf.Max(70, message.preferredHeight + 30));
    }

    void Update() {
        if (Time.realtimeSinceStartup >= hideAt) Hide();
    }
    void OnRectTransformDimensionsChange() {
        if (panel && message && gameObject.activeSelf) Layout();
    }
    public void Hide() { gameObject.SetActive(false); }
}
