using System.Runtime.InteropServices;

public static class Match3DGameCommunity {
    // MP 游戏圈基础设置中的「游戏内跳转 ID」，不是小游戏 AppID。
    public const string OpenLink = "-SSEykJvFV3pORt5kTNpS4k-ebX1kSE7vhmgx5EkeT0ipmAI-MDD4_SRByLkhUNXMcXnrkJH5b0WrKZ7uR3OaNuLk9jTClATOa66oJ-GmXV73KuOrFbV2rLGzp9UPsxVIDE-58LuZi-1qeqXO5Uk6Jzl_GuGhnh2miSF3pf_NzijKA-1W7nFUkcdUk6yeNNQWY23Xc4YNUwiO8w-JZ_olafRz8SmiTVtEesFpy3Hu_poKZSfdfgPItjJ76KkcicsDCaK_f_ieRAYI2_z1njX131c9EJkd8_f5MssDGTsEBdW1cAIwOijHiQOJw5hY1SHUSfG6JJ05vn3qMRk9jNLwA";
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern void GooseGameCommunityOpen(string openlink);
#endif
    public static void Open() {
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        GooseGameCommunityOpen(OpenLink);
#else
        if (Match3DFlow.Instance) Match3DFlow.Instance.ShowHomeFeatureNotice("请在微信小游戏中打开游戏圈");
#endif
    }
}
