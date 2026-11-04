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
    public static bool UnlimitedMagnet=>Match3DFlow.Instance&&Match3DFlow.Instance.UnlimitedMagnet;
}
