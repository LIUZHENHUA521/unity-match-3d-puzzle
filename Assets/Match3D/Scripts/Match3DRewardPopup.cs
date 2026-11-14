using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Match3DRewardPopup : MonoBehaviour {
    public RectTransform items;
    public ScrollRect scroll;
    public Match3DRewardItemView itemTemplate;
    public Sprite[] toolIcons;
    public Sprite redPacketIcon;
    public Text buttonLabel, timestamp;
    public Text potScreenshotHint;
    public Button confirm;
    bool confirmed;
    Action onConfirm;

    public void Configure(IReadOnlyList<Match3DRewardItem> rewards, Action completed = null,
                          string buttonText = "领取", long receivedAt = 0) {
        if (rewards == null || rewards.Count == 0) throw new ArgumentException("At least one reward is required", nameof(rewards));
        for (int i = 0; i < rewards.Count; i++)
            if (rewards[i] == null) throw new ArgumentException("Reward entries cannot be null", nameof(rewards));
        confirmed = false;
        onConfirm = completed;
        confirm.interactable = true;
        confirm.onClick.RemoveAllListeners();
        confirm.onClick.AddListener(Confirm);
        buttonLabel.text = string.IsNullOrWhiteSpace(buttonText) ? "领取" : buttonText;
        bool hasRedPacket = false;
        for (int i = 0; i < rewards.Count; i++) hasRedPacket |= rewards[i].IsRedPacket;
        if (potScreenshotHint) potScreenshotHint.gameObject.SetActive(hasRedPacket);
        timestamp.gameObject.SetActive(receivedAt > 0);
        timestamp.text = receivedAt > 0
            ? DateTimeOffset.FromUnixTimeMilliseconds(receivedAt).ToOffset(TimeSpan.FromHours(8)).ToString("yyyy-M-d HH:mm:ss") : "";

        for (int i = items.childCount - 1; i >= 0; i--) {
            var child = items.GetChild(i);
            if (child == itemTemplate.transform) continue;
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        int columns = rewards.Count == 4 ? 2 : Mathf.Min(3, rewards.Count);
        int rows = Mathf.CeilToInt((float)rewards.Count / columns);
        float height = Mathf.Max(480, rows * 230);
        items.sizeDelta = new Vector2(0, height);
        for (int i = 0; i < rewards.Count; i++) {
            var reward = rewards[i];
            var cell = Instantiate(itemTemplate, items, false);
            cell.gameObject.SetActive(true);
            cell.icon.sprite = reward.IsRedPacket ? redPacketIcon : toolIcons[reward.ToolIndex];
            cell.icon.preserveAspect = true;
            cell.icon.rectTransform.sizeDelta = reward.IsRedPacket ? new Vector2(113, 137) : new Vector2(140, 140);
            cell.caption.text = reward.Caption;
            int row = i / columns;
            int inRow = Mathf.Min(columns, rewards.Count - row * columns);
            var rect = (RectTransform)cell.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, 1);
            rect.anchoredPosition = new Vector2((i % columns - (inRow - 1) * .5f) * 210,
                -height * .5f + ((rows - 1) * .5f - row) * 230);
            rect.localScale = Vector3.one * (rewards.Count == 1 ? 1 : .68f);
        }
        scroll.StopMovement();
        scroll.vertical = height > 480;
        scroll.verticalNormalizedPosition = 1;
    }

    void Confirm() {
        if (confirmed || !confirm.interactable) return;
        confirmed = true;
        confirm.interactable = false;
        var callback = onConfirm;
        onConfirm = null;
        if (Match3DFlow.Instance) Match3DFlow.Instance.HomeClick();
        gameObject.SetActive(false);
        Destroy(gameObject);
        callback?.Invoke();
    }
}
