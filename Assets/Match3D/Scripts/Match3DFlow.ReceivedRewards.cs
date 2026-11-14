using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Match3DFlow {
    Match3DRewardPresenter rewardPresenter;
    AudioClip potReceivedRewardSound;
    internal bool CanShowReceivedRewards => ScreenName != "game" || resultShown;

    internal void PotPlayReceivedRewardSound() {
        if (!SettingsEffects || !effects) return;
        // Same clip used by crazyCombo's PreAcceptEquipment.tipsAudio.
        if (!potReceivedRewardSound)
            potReceivedRewardSound = Resources.Load<AudioClip>("Match3D/SourceAudio/PotRewardReceived");
        if (potReceivedRewardSound) effects.PlayOneShot(potReceivedRewardSound);
    }

    // Call only after a successful grant. This acknowledges a receipt; it never grants twice.
    public void ShowReceivedRewards(IReadOnlyList<Match3DRewardItem> rewards, string buttonText = "领取",
                                    Action onConfirm = null, long receivedAt = 0) {
        if (rewards == null || rewards.Count == 0 || !canvas) return;
        if (!rewardPresenter) rewardPresenter = canvas.gameObject.AddComponent<Match3DRewardPresenter>();
        rewardPresenter.Enqueue(rewards, buttonText, receivedAt, onConfirm);
    }
    public void ShowReceivedTools(int[] quantities, string buttonText = "领取", Action onConfirm = null) {
        ShowReceivedRewards(Match3DRewardItem.FromTools(quantities), buttonText, onConfirm);
    }
}
