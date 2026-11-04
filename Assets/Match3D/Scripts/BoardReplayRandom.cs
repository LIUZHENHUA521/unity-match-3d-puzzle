using UnityEngine;

public interface IBoardRandom {
    float Range(float minInclusive,float maxInclusive);
}

// Use Unity's shared RNG without reseeding or saving state. Other Unity draws
// intentionally advance the same stream. Provenance: docs/refactoring/2026-11-04-naming.md.
public sealed class BoardGlobalRandom : IBoardRandom {
    public static readonly BoardGlobalRandom Instance=new BoardGlobalRandom();
    public float Range(float minInclusive,float maxInclusive)=>UnityEngine.Random.Range(minInclusive,maxInclusive);
}

// Isolated replay stream for explicit seeds and validation. Preserve the
// production RNG state around every draw; this does not reproduce an unknown seed.
public sealed class BoardReplayRandom : IBoardRandom {
    UnityEngine.Random.State state;
    public int DrawCount {get;private set;}
    public BoardReplayRandom(int seed){
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
