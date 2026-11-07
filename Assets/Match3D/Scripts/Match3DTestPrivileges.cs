public static class Match3DTestPrivileges {
    public const string LevelVersion="test-unlimited-magnet-v1";
#if UNITY_EDITOR
    public static bool ForceForValidation;
#endif
    public static bool AccountEnabled {get {
#if UNITY_EDITOR
        if(ForceForValidation)return true;
#endif
        var account=Match3DInvitations.Instance;
        return account&&account.Ready&&account.State.unlimitedMagnet;
    }}
    // Keep the existing account flag and record version compatible with the server.
    public static bool UnlimitedTools=>Match3DFlow.Instance&&Match3DFlow.Instance.UnlimitedTools;
    public static bool UnlimitedMagnet=>UnlimitedTools;
}
