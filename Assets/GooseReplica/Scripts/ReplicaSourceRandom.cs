using UnityEngine;

public interface IReplicaSourceRandom {
    float Range(float minInclusive,float maxInclusive);
}

// Production source path: j1345 passes straight through to Unity's shared native RNG.
// It neither seeds nor saves/restores state, so other Unity draws remain part of the stream.
public sealed class ReplicaSourceGlobalRandom : IReplicaSourceRandom {
    public static readonly ReplicaSourceGlobalRandom Instance=new ReplicaSourceGlobalRandom();
    public float Range(float minInclusive,float maxInclusive)=>UnityEngine.Random.Range(minInclusive,maxInclusive);
}

// Explicit replay / validation only. The original session seed has not been recovered.
// Keep test replays independent without resetting the production game's global RNG.
public sealed class ReplicaSourceRandom : IReplicaSourceRandom {
    UnityEngine.Random.State state;
    public int DrawCount {get;private set;}
    public ReplicaSourceRandom(int seed){
        var previous=UnityEngine.Random.state;
        try {UnityEngine.Random.InitState(seed);state=UnityEngine.Random.state;}
        finally {UnityEngine.Random.state=previous;}
    }
    public float Range(float minInclusive,float maxInclusive){
        var previous=UnityEngine.Random.state;
        try {
            UnityEngine.Random.state=state;
            float value=UnityEngine.Random.Range(minInclusive,maxInclusive);
            state=UnityEngine.Random.state;DrawCount++;return value;
        } finally {UnityEngine.Random.state=previous;}
    }
}
