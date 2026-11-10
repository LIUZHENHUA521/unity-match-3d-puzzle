using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Run Tools/reward_popup_mock.py, then the menu below. No real rewards are issued.
[InitializeOnLoad]
public static class Match3DRewardPopupValidation {
    const string Trigger = "Temp/run-reward-popup-validation";
    const string Output = "Temp/reward-popup-validation";
    static Match3DRewardPopupValidation() { EditorApplication.update += Poll; }
    [MenuItem("Goose Match3D/Validate Received Reward Popup (local fixture)")]
    public static void Run() { File.WriteAllText(Trigger, "run"); }
    static void Poll() {
        if (!File.Exists(Trigger) || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        if (!EditorApplication.isPlayingOrWillChangePlaymode) {
            SessionState.SetBool("reward-popup-started-play", true);
            EditorApplication.isPlaying = true;
            return;
        }
        if (!Application.isPlaying || !Match3DFlow.Instance || !Match3DInvitations.Instance || !Match3DRewards.Instance) return;
        File.Delete(Trigger);
        Directory.CreateDirectory(Output);
        File.WriteAllText(Output + "/result.txt", "");
        new GameObject("Reward popup validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard(Check()));
    }
    static IEnumerator Guard(IEnumerator steps) {
        while (true) {
            object current;
            try { if (!steps.MoveNext()) break; current = steps.Current; }
            catch (Exception e) { File.AppendAllText(Output + "/result.txt", "FAIL " + e); Finish(); yield break; }
            yield return current;
        }
        File.AppendAllText(Output + "/result.txt", "ALL PASSED\n");
        Finish();
    }
    static void Finish() {
        Time.timeScale = 1;
        if (SessionState.GetBool("reward-popup-started-play", false)) {
            SessionState.SetBool("reward-popup-started-play", false);
            EditorApplication.isPlaying = false;
        }
    }
    static void Assert(bool pass, string message) {
        if (!pass) throw new Exception(message);
        File.AppendAllText(Output + "/result.txt", "PASS " + message + "\n");
    }
    static Match3DRewardPopup Popup => UnityEngine.Object.FindObjectOfType<Match3DRewardPopup>();
    static IEnumerator WaitFor(Func<bool> condition) {
        float until = Time.realtimeSinceStartup + 20;
        while (!condition() && Time.realtimeSinceStartup < until) yield return null;
    }
    static IEnumerator Settled() { yield return new WaitForSecondsRealtime(.55f); }
    static IEnumerator Check() {
        var flow = Match3DFlow.Instance;
        flow.Home();
        Assert(Match3DRewardItem.FromTools(new[] { 0, 2, -1, 4 }).Length == 2, "non-positive quantities filtered");
        flow.ShowReceivedTools(new int[4]); yield return null;
        Assert(!Popup, "empty rewards do not block input");
        int clicks = 0;
        Time.timeScale = 0;
        flow.ShowReceivedRewards(new[] { Match3DRewardItem.RedPacket(10) }, onConfirm: () => clicks++, receivedAt: 1794268800000);
        yield return WaitFor(() => Popup); yield return Settled();
        var popup = Popup;
        Assert(popup && flow.BlocksGameplay, "receipt overlays and blocks underlying input");
        Assert(popup.buttonLabel.text == "领取" && popup.buttonLabel.fontSize == 46 && popup.buttonLabel.fontStyle == FontStyle.Bold, "default button uses PSD 46 px bold");
        Assert(popup.buttonLabel.GetComponent<Match3DTextStroke>().radius == 4, "PSD 4 px outer stroke");
        Assert(popup.GetComponentInChildren<CanvasGroup>().interactable, "entrance completes while time scale is zero");
        Assert(popup.timestamp.text == "2026-11-10 8:00:00" || popup.timestamp.text == "2026-11-10 08:00:00", "receipt timestamp is supplied data in UTC+8");
        Capture("red-packet", 720, 1560);
        popup.confirm.onClick.Invoke(); popup.confirm.onClick.Invoke();
        Assert(clicks == 1, "double click invokes callback once");
        yield return null; Assert(!Popup && !flow.BlocksGameplay, "closing receipt restores home input");
        Time.timeScale = 1;

        foreach (int count in new[] { 2, 4, 5, 9 }) {
            var data = Enumerable.Range(0, count).Select(i => i == 4 ? Match3DRewardItem.RedPacket(2) : Match3DRewardItem.Tool(i % 4, i + 1)).ToArray();
            flow.ShowReceivedRewards(data, count == 5 ? "返回首页" : "领取");
            yield return WaitFor(() => Popup); yield return Settled(); popup = Popup;
            var cells = popup.items.GetComponentsInChildren<Match3DRewardItemView>();
            Assert(cells.Length == count && cells.All(c => c.icon.sprite && !string.IsNullOrEmpty(c.caption.text)), "all " + count + " rewards retain icons and quantities");
            Assert(popup.scroll.vertical == (count > 6), "overflow scrolling for " + count + " rewards");
            Assert(!popup.timestamp.gameObject.activeSelf, "timestamp hidden when unspecified");
            if (count == 4) { Capture("four-tools", 720, 1280); Capture("four-tools-short", 720, 960); }
            if (count == 5) { Assert(popup.buttonLabel.text == "返回首页", "custom button text"); Capture("mixed-rewards", 720, 1560); }
            if (count == 9) {
                popup.scroll.verticalNormalizedPosition = 0; Canvas.ForceUpdateCanvases();
                var last = cells.Last().transform.position;
                Assert(RectTransformUtility.RectangleContainsScreenPoint(popup.scroll.viewport, RectTransformUtility.WorldToScreenPoint(null, last)), "last reward reachable by scrolling");
            }
            popup.confirm.onClick.Invoke(); yield return null;
        }

        flow.ShowUserProfile();
        flow.ShowReceivedTools(new[] { 1, 0, 0, 0 });
        flow.ShowReceivedTools(new[] { 0, 1, 0, 0 }, "继续");
        yield return WaitFor(() => Popup); yield return Settled();
        Popup.confirm.onClick.Invoke(); yield return null; yield return Settled();
        Assert(Popup && Popup.buttonLabel.text == "继续", "successive receipts queue instead of overwriting");
        Popup.confirm.onClick.Invoke(); yield return null;
        Assert(flow.BlocksGameplay && UnityEngine.Object.FindObjectOfType<Match3DUserProfileView>(), "receipt preserves underlying modal and its input block");
        flow.Home();

        var screen = typeof(Match3DFlow).GetProperty("ScreenName");
        screen.SetValue(flow, "game");
        flow.ShowReceivedTools(new[] { 0, 0, 1, 0 }); yield return Settled();
        Assert(!Popup, "late reward waits during active gameplay");
        flow.Home(); yield return WaitFor(() => Popup); yield return Settled();
        Assert(Popup, "queued receipt survives navigation"); Popup.confirm.onClick.Invoke(); yield return null;

        var account = Match3DInvitations.Instance;
        account.Initialize("http://127.0.0.1:8107");
        account.OnInvitationBridge("{\"code\":\"reward-popup-fixture\"}");
        yield return WaitFor(() => !account.Busy);
        Assert(account.Ready && account.Mails.Count == 2, "local mail fixture connected");
        bool success = true;
        account.ClaimMail("fixture-failed", ok => success = ok);
        yield return WaitFor(() => !account.Busy); yield return Settled();
        Assert(!success && !Popup && account.Inventory(0) == 0, "failed claim does not display or credit rewards");
        flow.ShowMailbox(); yield return WaitFor(() => !account.Busy); yield return null;
        var mailbox = UnityEngine.Object.FindObjectOfType<Match3DMailboxView>();
        mailbox.list.GetComponentsInChildren<Match3DUIView>().First().Get<Button>("Open").onClick.Invoke();
        yield return WaitFor(() => !account.Busy); yield return null;
        UnityEngine.Object.FindObjectOfType<Match3DCommonDialog>().right.onClick.Invoke();
        yield return WaitFor(() => !account.Busy); yield return WaitFor(() => Popup); yield return Settled();
        Assert(Popup && Popup.items.GetComponentsInChildren<Match3DRewardItemView>().Length == 4 && account.Inventory(3) == 4, "actual mailbox claim credits and presents all four tools");
        Assert(!UnityEngine.Object.FindObjectOfType<Match3DCommonDialog>(), "successful mail detail closes before receipt");
        Capture("mail-claim", 720, 1280);
        Popup.confirm.onClick.Invoke(); yield return null;
        account.ClaimMail("fixture-tools", ok => success = ok);
        yield return WaitFor(() => !account.Busy); yield return Settled();
        Assert(success && !Popup && account.Inventory(0) == 2, "claim retry neither recredits nor repeats popup");
        flow.Home();

        var rewards = Match3DRewards.Instance; rewards.Initialize("http://127.0.0.1:8107");
        rewards.NotifyReceivedVoucher("fixture-voucher", account.State.inviteCode);
        yield return WaitFor(() => Popup); yield return Settled();
        Assert(Popup && Popup.items.GetComponentInChildren<Match3DRewardItemView>().caption.text == "10元", "confirmed voucher uses server amount");
        Popup.confirm.onClick.Invoke(); yield return null;
        rewards.NotifyReceivedVoucher("fixture-voucher", account.State.inviteCode); yield return Settled();
        Assert(!Popup, "duplicate voucher acknowledgement suppressed");
        rewards.NotifyReceivedVoucher("fixture-missing-detail", account.State.inviteCode);
        yield return WaitFor(() => Popup); yield return Settled();
        Assert(Popup && Popup.items.GetComponentInChildren<Match3DRewardItemView>().caption.text == "红包" && !Popup.timestamp.gameObject.activeSelf, "confirmed voucher with unavailable details never fabricates amount or time");
        Popup.confirm.onClick.Invoke(); yield return null;
    }
    static void Capture(string name, int width, int height) {
        var camera = Camera.main;
        var canvases = UnityEngine.Object.FindObjectsOfType<Canvas>().Where(c => c.isRootCanvas && c.gameObject.activeInHierarchy).ToArray();
        var modes = canvases.Select(c => c.renderMode).ToArray(); var cameras = canvases.Select(c => c.worldCamera).ToArray(); var distances = canvases.Select(c => c.planeDistance).ToArray();
        var target = new RenderTexture(width, height, 24); var old = camera.targetTexture; var active = RenderTexture.active;
        target.Create(); camera.targetTexture = target;
        try {
            for (int i = 0; i < canvases.Length; i++) { canvases[i].renderMode = RenderMode.ScreenSpaceCamera; canvases[i].worldCamera = camera; canvases[i].planeDistance = 1; }
            Canvas.ForceUpdateCanvases();
            foreach (var fit in UnityEngine.Object.FindObjectsOfType<Match3DUIFitContent>()) fit.Fit();
            Canvas.ForceUpdateCanvases(); camera.Render(); RenderTexture.active = target;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0); tex.Apply(); File.WriteAllBytes(Output + "/" + name + ".png", tex.EncodeToPNG()); UnityEngine.Object.Destroy(tex);
        } finally {
            camera.targetTexture = old; RenderTexture.active = active;
            for (int i = 0; i < canvases.Length; i++) { canvases[i].renderMode = modes[i]; canvases[i].worldCamera = cameras[i]; canvases[i].planeDistance = distances[i]; }
            target.Release(); UnityEngine.Object.Destroy(target); Canvas.ForceUpdateCanvases();
            foreach (var fit in UnityEngine.Object.FindObjectsOfType<Match3DUIFitContent>()) fit.Fit();
        }
    }
}
