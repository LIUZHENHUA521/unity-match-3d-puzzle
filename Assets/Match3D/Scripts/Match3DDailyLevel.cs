using System;
using System.Collections.Generic;

[Serializable]
public sealed class Match3DDailyLevel {
    public string date,tier,sourceDate,version,generatorVersion,modelThemeId;
    public int seed,seconds,initialCount;
    public float limitHeight;
    public ChallengeLayer[] layers;
    public bool IsValid(int pool) {
        DateTime parsed;
        if(!DateTime.TryParseExact(date,"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None,out parsed)||
           (tier!="bachelor"&&tier!="elite")||string.IsNullOrEmpty(version)||generatorVersion!="daily-v1"||seed<=0||seconds<30||seconds>3600||layers==null||layers.Length<1||layers.Length>10)return false;
        int total=0;float previous=1.5f;
        foreach(var l in layers){
            if(l==null||l.count<3||l.count>600||l.count%3!=0||l.types<1||l.types>Math.Min(pool,l.count/3)||float.IsNaN(l.height)||float.IsInfinity(l.height)||l.height<=previous||l.height>30)return false;
            total+=l.count;previous=l.height;
        }
        return total==initialCount&&total<=1200&&Math.Abs(limitHeight-(previous-.5f))<.001f;
    }
    public IBoardRandom RandomForStage(int stage) {return new Match3DDailyRandom(unchecked(seed^(stage*0x45d9f3b)));}
    // Positive budgets preserve exact per-layer counts and selected type counts for every valid configuration.
    public static List<ChallengeTile> Generate(int pool,ChallengeLayer[] layers,IBoardRandom random) {
        var result=new List<ChallengeTile>();
        for(int layer=0;layer<layers.Length;layer++){
            var row=layers[layer];var types=new List<int>();for(int t=0;t<pool;t++)types.Add(t);
            for(int i=types.Count-1;i>0;i--){int j=(int)random.Range(0,i+1);int t=types[i];types[i]=types[j];types[j]=t;}
            var items=new List<int>();int groups=row.count/3;
            for(int g=0;g<groups;g++)for(int k=0;k<3;k++)items.Add(types[g%row.types]);
            for(int i=items.Count-1;i>0;i--){int j=(int)random.Range(0,i+1);int t=items[i];items[i]=items[j];items[j]=t;}
            foreach(int type in items)result.Add(new ChallengeTile{id=result.Count,type=type,layer=layer,height=row.height});
        }
        return result;
    }
}
// Versioned, platform-independent PRNG. Never touches Unity's global random state.
public sealed class Match3DDailyRandom : IBoardRandom {
    uint state;
    public Match3DDailyRandom(int seed){state=unchecked((uint)seed);if(state==0)state=1;}
    public float Range(float minInclusive,float maxInclusive){
        uint x=state;x^=x<<13;x^=x>>17;x^=x<<5;state=x;
        // 23-bit fraction keeps integer selection strictly below max after float rounding.
        float ratio=(x>>9)*(1f/8388608f);
        return minInclusive+(maxInclusive-minInclusive)*ratio;
    }
}
