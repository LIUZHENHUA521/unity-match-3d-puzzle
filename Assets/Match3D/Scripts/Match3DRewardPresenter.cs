using System;
using System.Collections.Generic;
using UnityEngine;

// Lives on the canvas so navigation and Flow.StopAllCoroutines cannot discard receipts.
public sealed class Match3DRewardPresenter : MonoBehaviour {
    sealed class Request {
        public Match3DRewardItem[] items;
        public string label;
        public long receivedAt;
        public Action completed;
    }
    readonly Queue<Request> pending = new Queue<Request>();
    Match3DRewardPopup current;
    public bool IsShowing => current && current.gameObject.activeSelf;
    public void Enqueue(IReadOnlyList<Match3DRewardItem> rewards, string label, long receivedAt, Action completed) {
        if (rewards == null || rewards.Count == 0) return;
        var snapshot = new Match3DRewardItem[rewards.Count];
        for (int i = 0; i < rewards.Count; i++) {
            if (rewards[i] == null) throw new ArgumentException("Reward entries cannot be null", nameof(rewards));
            snapshot[i] = rewards[i];
        }
        pending.Enqueue(new Request { items = snapshot, label = label, receivedAt = receivedAt, completed = completed });
    }
    void Update() {
        if (IsShowing || pending.Count == 0) return;
        var flow = Match3DFlow.Instance;
        // A late server response must not cover a running match or a revival prompt.
        if (flow && !flow.CanShowReceivedRewards) return;
        var request = pending.Dequeue();
        current = Match3DUIView.Load("RewardReceived", transform).GetComponent<Match3DRewardPopup>();
        current.transform.SetAsLastSibling();
        current.Configure(request.items, () => { current = null; request.completed?.Invoke(); }, request.label, request.receivedAt);
        Match3DModalMotion.Play(current.gameObject);
    }
}
