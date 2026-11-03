using System.Runtime.InteropServices;

public static class ReplicaGameCommunity {
    // MP 游戏圈基础设置中的「游戏内跳转 ID」，不是小游戏 AppID。
    public const string OpenLink = "";
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern void GooseGameCommunityOpen(string openlink);
#endif
    public static void Open() {
#if UNITY_WEBGL && !UNITY_EDITOR
        GooseGameCommunityOpen(OpenLink);
#else
        if (ReplicaFlow.Instance) ReplicaFlow.Instance.ShowHomeFeatureNotice("请在微信小游戏中打开游戏圈");
#endif
    }
}
