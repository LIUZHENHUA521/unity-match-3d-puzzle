using System.Runtime.InteropServices;

public static class PotSameGames {
    public const string CurrentGameNotice = "当前游戏，无法跳转";
    public const string CrazyComboAppId = "wx85d24d5dbf72b423";
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern void PotSameGamesOpen(string appId);
#endif
    public static void OpenCrazyCombo() {
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        PotSameGamesOpen(CrazyComboAppId);
#else
        if (Match3DFlow.Instance) Match3DFlow.Instance.ShowHomeFeatureNotice("请在微信小游戏中打开疯狂动物消");
#endif
    }
}
