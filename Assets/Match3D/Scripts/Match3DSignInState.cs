using System;

[Serializable] public sealed class Match3DSignInDay { public int day; public int[] rewards; }
[Serializable] public sealed class Match3DSignInReceipt {
    public string date; public int cycle,day; public int[] rewards; public long claimedAt; public bool replayed;
}
[Serializable] public sealed class Match3DSignInState {
    public string date; public int cycle,claimedDays,day; public bool canClaim;
    public long serverNow,nextRefreshAt; public Match3DSignInDay[] days;
    public bool Valid {
        get {
            if(string.IsNullOrEmpty(date)||cycle<1||claimedDays<0||claimedDays>7||day<1||day>7||days==null||days.Length!=7||nextRefreshAt<=serverNow)return false;
            for(int i=0;i<days.Length;i++) {
                var entry=days[i];if(entry==null||entry.day!=i+1||entry.rewards==null||entry.rewards.Length!=4)return false;
                bool any=false;foreach(int n in entry.rewards){if(n<0||n>1000000)return false;any|=n>0;}if(!any)return false;
            }
            return true;
        }
    }
    // Initial artwork/editor preview only. Live rewards always come from the server.
    public static Match3DSignInState Preview() {
        int[][] rewards={new[]{1,0,0,0},new[]{0,0,0,1},new[]{0,1,0,0},new[]{2,0,0,0},new[]{0,0,2,0},new[]{0,1,0,2},new[]{2,2,2,2}};
        var value=new Match3DSignInState{cycle=1,day=1,canClaim=true,days=new Match3DSignInDay[7]};
        for(int i=0;i<7;i++)value.days[i]=new Match3DSignInDay{day=i+1,rewards=rewards[i]};return value;
    }
}
