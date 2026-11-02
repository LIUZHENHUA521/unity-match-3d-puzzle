public static class ReplicaTestPrivileges {
    public const string LevelVersion="test-unlimited-magnet-v1";
#if UNITY_EDITOR
    public static bool ForceForValidation;
#endif
    public static bool AccountEnabled {get {
#if UNITY_EDITOR
        if(ForceForValidation)return true;
#endif
        var account=ReplicaInvitations.Instance;
        return account&&account.Ready&&account.State.unlimitedMagnet;
    }}
    public static bool UnlimitedMagnet=>ReplicaFlow.Instance&&ReplicaFlow.Instance.UnlimitedMagnet;
}
