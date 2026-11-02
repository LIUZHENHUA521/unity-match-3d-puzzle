using System.Runtime.InteropServices;

// Shared by the home menu and home settings. Call directly from the user's tap.
public static class ReplicaCustomerService {
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern void GooseCustomerServiceOpen();
#endif
    public static void Open() {
#if UNITY_WEBGL && !UNITY_EDITOR
        GooseCustomerServiceOpen();
#else
        if (ReplicaFlow.Instance) ReplicaFlow.Instance.ShowHomeFeatureNotice("请在微信小游戏中联系客服");
#endif
    }
}
