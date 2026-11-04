using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class Match3DTipsValidation {
    const string Trigger = "Temp/run-tips-validation";
    const string Output = "Temp/tips-validation.txt";
    static Match3DTipsValidation() { EditorApplication.update += Watch; }
    [MenuItem("Goose Match3D/Validate ShowTips")]
    public static void Run() { File.WriteAllText(Trigger, "run"); }
    static void Watch() {
        if (!File.Exists(Trigger) || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        if (!EditorApplication.isPlayingOrWillChangePlaymode) {
            SessionState.SetBool("TipsValidationStartedPlay", true);
            EditorApplication.isPlaying = true;
            return;
        }
        if (!Application.isPlaying || !Match3DFlow.Instance || !UnityEngine.Object.FindObjectOfType<Match3DHomeView>()) return;
        File.Delete(Trigger);
        File.WriteAllText(Output, "");
        new GameObject("Tips validation driver").AddComponent<BoardValidationDriver>().StartCoroutine(Guard(Check()));
    }
    static IEnumerator Guard(IEnumerator routine) {
        while (true) {
            object current;
            try { if (!routine.MoveNext()) break; current = routine.Current; }
            catch (Exception e) { File.AppendAllText(Output, "FAIL " + e); Finish(); yield break; }
            yield return current;
        }
        File.AppendAllText(Output, "ALL PASSED\n");
        Finish();
    }
    static void Finish() {
        Time.timeScale = 1;
        if (SessionState.GetBool("TipsValidationStartedPlay", false)) {
            SessionState.SetBool("TipsValidationStartedPlay", false);
            EditorApplication.isPlaying = false;
        }
    }
    static void Assert(bool ok, string description) {
        if (!ok) throw new Exception(description);
        File.AppendAllText(Output, "PASS " + description + "\n");
    }
    static IEnumerator Check() {
        var flow = Match3DFlow.Instance;
        flow.ShowTips("操作成功");
        yield return null;
        var tips = UnityEngine.Object.FindObjectOfType<Match3DTips>();
        var label = tips.GetComponentInChildren<Text>();
        Assert(tips && label.text == "操作成功", "public ShowTips displays message");
        Assert(!tips.GetComponent<CanvasGroup>().blocksRaycasts && tips.GetComponentsInChildren<Graphic>().All(g => !g.raycastTarget), "overlay never intercepts clicks");
        flow.StopAllCoroutines();
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(.9f);
        flow.Notice("新的提示");
        yield return new WaitForSecondsRealtime(.8f);
        Assert(tips.gameObject.activeSelf && label.text == "新的提示", "replacement restarts lifetime; legacy Notice uses same component");
        flow.ShowTips("  ");
        Assert(label.text == "新的提示", "blank messages ignored");
        yield return new WaitForSecondsRealtime(.85f);
        Assert(!tips.gameObject.activeSelf, "auto hides while paused and after Flow.StopAllCoroutines");
        Time.timeScale = 1;
        flow.ShowUserProfile();
        flow.ShowTips("网络暂不可用，请检查网络连接后稍后重试。道具数量不会受到影响。");
        yield return null;
        var panel = (RectTransform)label.transform.parent;
        Assert(panel.rect.height > 70 && panel.rect.width <= ((RectTransform)tips.transform).rect.width - 47, "long message wraps within screen margins");
        Assert(tips.GetComponent<Canvas>().overrideSorting && tips.GetComponent<Canvas>().sortingOrder == 100, "tips render above modal");
        ScreenCapture.CaptureScreenshot("Temp/tips-game-preview.png");
        yield return new WaitForEndOfFrame();
        yield return null;
        flow.Home();
        Assert(!tips.gameObject.activeSelf, "navigation clears old message");
        flow.ShowHomeFeatureNotice("七日签到功能尚未接入");
        Assert(tips.gameObject.activeSelf && label.text == "七日签到功能尚未接入", "existing home feature entry reuses tips after navigation");
        yield return new WaitForSecondsRealtime(1.65f);
        Assert(!tips.gameObject.activeSelf, "normal 1.5 second dismissal");
    }
}
