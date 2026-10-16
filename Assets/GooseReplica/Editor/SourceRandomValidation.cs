using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SourceRandomValidation {
    sealed class Trace : IReplicaSourceRandom {
        public readonly List<Vector2> calls=new List<Vector2>();
        public float Range(float min,float max){calls.Add(new Vector2(min,max));return min+(max-min)*.5f;}
    }
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run(){
        var saved=UnityEngine.Random.state;string report="";
        try {
            const int seed=1926537;
            UnityEngine.Random.InitState(seed);
            var expected=new float[120];
            for(int i=0;i<expected.Length;i++)expected[i]=UnityEngine.Random.Range(i%3==0?-1f:0f,i%3==2?.26f:1f);
            var expectedFinal=UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);
            var beforeGlobal=UnityEngine.Random.state;
            var global=ReplicaSourceGlobalRandom.Instance;
            Check(beforeGlobal.Equals(UnityEngine.Random.state),"global wrapper construction changed RNG state");
            for(int i=0;i<expected.Length;i++){
                float min=i%3==0?-1f:0f,max=i%3==2?.26f:1f;
                // External draws must advance exactly the same stream as source generation.
                float value=i%2==0?global.Range(min,max):UnityEngine.Random.Range(min,max);
                Check(value==expected[i],"global wrapper isolated or reset shared RNG at "+i);
            }
            Check(expectedFinal.Equals(UnityEngine.Random.state),"global wrapper did not advance the original shared state");
            Check(!beforeGlobal.Equals(UnityEngine.Random.state),"global wrapper left shared state unchanged");
            UnityEngine.Random.state=saved;
            var stream=new ReplicaSourceRandom(seed);
            Check(saved.Equals(UnityEngine.Random.state),"constructor modified global random state");
            for(int i=0;i<expected.Length;i++){
                Check(stream.Range(i%3==0?-1f:0f,i%3==2?.26f:1f)==expected[i],"Unity float Range sequence mismatch "+i);
                Check(saved.Equals(UnityEngine.Random.state),"draw modified global random state");
            }
            Check(stream.DrawCount==120,"source draw counter");
            var trace=new Trace();
            var plan=ChallengePlan.GenerateSource(trace,2,new[]{new ChallengeLayer(6,2,14)});
            Check(trace.calls.Count==8,"two float type draws + six item draws; no spatial shuffle draws");
            Check(trace.calls[0]==new Vector2(0,2)&&trace.calls[1]==new Vector2(0,1),"source initial selection float bounds");
            Check(plan.Select(x=>x.type).SequenceEqual(new[]{1,1,1,0,0,0}),"preserve source insertion order before coordinate generation");
            ReplicaSourceEnvironment.Select("CountrysideStove");
            var profile=ReplicaSourceLevelPlan.Get("CountrysideStove");
            var a=new ReplicaSourceRandom(seed);var b=new ReplicaSourceRandom(seed);
            var pa=ChallengePlan.GenerateSource(a,profile.prefabs.Length,profile.layers);
            var pb=ChallengePlan.GenerateSource(b,profile.prefabs.Length,profile.layers);
            Check(pa.Select(x=>x.type).SequenceEqual(pb.Select(x=>x.type)),"same seed plan replay");
            foreach(var tile in pa){
                float low=tile.layer==0?1.5f:profile.layers[tile.layer-1].height;
                Check(ReplicaSourceEnvironment.Position(low,tile.height,a)==ReplicaSourceEnvironment.Position(low,tile.height,b),"continued coordinate replay");
            }
            Check(ReplicaSourceEnvironment.Rotation(a)==ReplicaSourceEnvironment.Rotation(b),"continued rotation replay");
            Check(a.DrawCount==b.DrawCount&&a.DrawCount>1200,"continuous stream consumed plan, positions, rotation");
            Check(saved.Equals(UnityEngine.Random.state),"whole plan/coordinates modified global random state");
            var rotationTrace=new Trace();ReplicaSourceEnvironment.Rotation(rotationTrace);
            Check(rotationTrace.calls.Count==3&&rotationTrace.calls.All(v=>v==new Vector2(0,1)),"rotation uses XYZ Range(0,1) times 360");
            report="PASS source random: production global wrapper and interleaved external draws advance one Unity RNG stream with no reseed/isolation; explicit replay stream matches 120 Unity float draws and preserves external RNG; float type selection/no shuffle trace; continuous plan-position-rotation replay; XYZ rotation bounds. Original package runtime seed and native cross-version bit identity remain unverified.\n";
        }catch(Exception e){report="FAIL "+e+"\n";}
        finally{UnityEngine.Random.state=saved;File.WriteAllText("Temp/goose-source-random-validation.txt",report);Debug.Log(report);}
    }
}
