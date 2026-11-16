// A first/isolated warning lowers cost; repeated pressure within 30s stops safely.
public sealed class PotReplayMemoryPolicy {
    public bool Degraded {get;private set;}
    float lastWarning=float.NegativeInfinity;
    public float SampleInterval=>Degraded?.5f:.1f;
    public bool Stop(float now,int level=0){bool stop=level>=15||now-lastWarning<30;Degraded=true;lastWarning=now;return stop;}
}
