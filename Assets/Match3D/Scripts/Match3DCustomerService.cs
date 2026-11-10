using System.Runtime.InteropServices;

// Shared by the home menu and home settings. Call directly from the user's tap.
public static class Match3DCustomerService {
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern void GooseCustomerServiceOpen();
#endif
    public static void Open() {
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        GooseCustomerServiceOpen();
#else
        if (Match3DFlow.Instance) Match3DFlow.Instance.ShowHomeFeatureNotice("请在微信小游戏中联系客服");
#endif
    }
}
