using System;
using System.Collections.Generic;
using System.Linq;

[Serializable] public class ChallengeLayer {
    public int count, types;
    public float height;
    // Type-budget weights, not selection weights. Empty means equal budgets.
    public float[] weights;
    public ChallengeLayer(int count,int types,float height) { this.count=count;this.types=types;this.height=height; }
}
[Serializable] public class ChallengeTile { public int id,type,layer; public float height; }
public static class ChallengePlan {
    public static int DailySeed(string theme,int stage,DateTime utc) {
        string key=utc.AddHours(8).ToString("yyyyMMdd")+":"+theme+":"+stage+":v2";
        unchecked { int hash=17;foreach(char c in key)hash=hash*31+c;return hash&0x7fffffff; }
    }
    public static ChallengeLayer[] DefaultLayers(int pool) => new[]{
        new ChallengeLayer(150,Math.Min(20,pool),14f),new ChallengeLayer(60,Math.Min(10,pool),18f),
        new ChallengeLayer(45,Math.Min(8,pool),21f),new ChallengeLayer(45,Math.Min(7,pool),24f)};

    // All triples are budgeted globally. A layer's selection is a plan, not a final quota per type.
    public static List<ChallengeTile> Generate(int seed,int pool,ChallengeLayer[] layers) => GenerateInternal(seed,pool,layers,false);
    // WX CreateLevel j38517. Production shares Unity's global stream, without InitState.
    public static List<ChallengeTile> GenerateSource(int pool,ChallengeLayer[] layers) => GenerateSource(BoardGlobalRandom.Instance,pool,layers);
    // Explicit seeded replay / tests; normal stage 2 does not call this overload.
    public static List<ChallengeTile> GenerateSource(int seed,int pool,ChallengeLayer[] layers) => GenerateSource(new BoardReplayRandom(seed),pool,layers);
    public static List<ChallengeTile> GenerateSource(IBoardRandom random,int pool,ChallengeLayer[] layers) => GenerateInternal(0,pool,layers,true,random);
    public static int[] SourceGroupBudgets(int groups,float[] selectedWeights){
        if(groups<0||selectedWeights==null||selectedWeights.Length==0||selectedWeights.Any(x=>x<=0||float.IsNaN(x)||float.IsInfinity(x)))throw new ArgumentException("Invalid source budget");
        float total=0;foreach(float w in selectedWeights)total+=w;
        var result=new int[selectedWeights.Length];int remaining=groups;
        for(int i=0;i<result.Length-1;i++){
            // j38517 computes float32 quota; j1107 rounds half to even.
            float quota=groups*selectedWeights[i]/total;result[i]=(int)Math.Round(quota,MidpointRounding.ToEven);remaining-=result[i];
        }
        // Original keeps the signed remainder. No minimum-one or Hamilton correction.
        result[result.Length-1]=remaining;return result;
    }
    static List<ChallengeTile> GenerateInternal(int seed,int pool,ChallengeLayer[] layers,bool source,IBoardRandom sourceRandom=null) {
        if(pool<1||layers==null||layers.Length==0)throw new ArgumentException("Empty configuration");
        var random=new Random(seed);
        var budgets=new int[layers.Length,pool];
        var selectedTypes=new bool[layers.Length,pool];
        for(int layerIndex=0;layerIndex<layers.Length;layerIndex++) {
            // Validate each layer immediately before consuming its random draws.
            // Moving all validation up front would change failure-path RNG state.
            var layer=layers[layerIndex];
            var weights=ValidateLayer(layer,pool);
            var selected=SelectLayerTypes(pool,layer.types,random,source,sourceRandom);
            foreach(int type in selected)selectedTypes[layerIndex,type]=true;
            AllocateLayerBudgets(layerIndex,layer.count/3,selected,weights,budgets,source);
        }

        var placed=new List<int>[layers.Length];
        for(int layerIndex=0;layerIndex<layers.Length;layerIndex++)placed[layerIndex]=new List<int>();
        // Descending type order determines both placement and subsequent RNG calls.
        for(int type=pool-1;type>=0;type--)
            PlaceType(type,layers,budgets,selectedTypes,placed,random,source,sourceRandom);
        return BuildTiles(layers,placed,random,source);
    }

    static float[] ValidateLayer(ChallengeLayer layer,int pool) {
        if(layer==null||layer.count<=0||layer.count%3!=0||layer.types<1||layer.types>pool||layer.count/3<layer.types||float.IsNaN(layer.height)||float.IsInfinity(layer.height))
            throw new ArgumentException("Layer requires complete triples and enough types");
        var weights=layer.weights!=null&&layer.weights.Length>0?layer.weights:null;
        if(weights!=null&&(weights.Length!=pool||weights.Any(x=>x<=0||float.IsNaN(x)||float.IsInfinity(x))))
            throw new ArgumentException("Weights must be positive, finite and match pool");
        return weights;
    }

    static List<int> SelectLayerTypes(int pool,int count,Random random,bool source,IBoardRandom sourceRandom) {
        var candidates=Enumerable.Range(0,pool).ToList();
        var selected=new List<int>();
        if(source) {
            // Equal initial weights still use float draws and frequency correction.
            // The original algorithm mapping is retained in the provenance notes below.
            var typePool=new FrequencyPool(random,true,sourceRandom);
            foreach(int type in candidates)typePool.Add(type,1f);
            while(selected.Count<count) {
                int type=typePool.Roll();
                selected.Add(type);
                typePool.Remove(type);
            }
            return selected;
        }
        while(selected.Count<count) {
            int index=random.Next(candidates.Count);
            selected.Add(candidates[index]);
            candidates.RemoveAt(index);
        }
        return selected;
    }

    static void AllocateLayerBudgets(int layerIndex,int groupCount,List<int> selected,float[] weights,int[,] budgets,bool source) {
        if(source) {
            var selectedWeights=selected.Select(type=>weights==null?1f:weights[type]).ToArray();
            var groups=SourceGroupBudgets(groupCount,selectedWeights);
            for(int index=0;index<selected.Count;index++)budgets[layerIndex,selected[index]]=groups[index];
            return;
        }
        // Positive Hamilton allocation: one group each, then largest remainders.
        int remaining=groupCount-selected.Count;
        double total=selected.Sum(type=>weights==null?1d:weights[type]);
        var fractions=new Dictionary<int,double>();
        int used=0;
        foreach(int type in selected) {
            double quota=remaining*(weights==null?1d:weights[type])/total;
            int whole=(int)Math.Floor(quota);
            budgets[layerIndex,type]=1+whole;
            used+=whole;
            fractions[type]=quota-whole;
        }
        foreach(int type in selected.OrderByDescending(type=>fractions[type]).Take(remaining-used))budgets[layerIndex,type]++;
    }

    static void PlaceType(int type,ChallengeLayer[] layers,int[,] budgets,bool[,] selectedTypes,List<int>[] placed,Random random,bool source,IBoardRandom sourceRandom) {
        int remaining=0;
        var choices=new FrequencyPool(random,source,sourceRandom);
        for(int layerIndex=0;layerIndex<layers.Length;layerIndex++) {
            remaining+=3*budgets[layerIndex,type];
            if(source?selectedTypes[layerIndex,type]:budgets[layerIndex,type]>0)
                choices.Add(layerIndex,source?(float)budgets[layerIndex,type]/(layers[layerIndex].count/3):(double)budgets[layerIndex,type]/(layers[layerIndex].count/3));
        }
        while(remaining>0) {
            if(choices.Count==0) {
                int spare=FindAvailableLayer(layers,placed);
                if(spare<0) {
                    if(source)break;
                    throw new InvalidOperationException("Capacity exhausted");
                }
                choices.Add(spare,1);
            }
            int chosen=choices.Roll();
            if(placed[chosen].Count>=layers[chosen].count) {
                choices.Remove(chosen);
                continue;
            }
            placed[chosen].Add(type);
            remaining--;
        }
    }

    static int FindAvailableLayer(ChallengeLayer[] layers,List<int>[] placed) {
        for(int layerIndex=0;layerIndex<layers.Length;layerIndex++)
            if(placed[layerIndex].Count<layers[layerIndex].count)return layerIndex;
        return -1;
    }

    static List<ChallengeTile> BuildTiles(ChallengeLayer[] layers,List<int>[] placed,Random random,bool source) {
        var result=new List<ChallengeTile>();
        for(int layerIndex=0;layerIndex<layers.Length;layerIndex++) {
            // Keep insertion order for the shared-stream path; only legacy levels shuffle.
            if(!source)ShuffleLayer(placed[layerIndex],random);
            foreach(int type in placed[layerIndex])
                result.Add(new ChallengeTile{id=result.Count,type=type,layer=layerIndex,height=layers[layerIndex].height});
        }
        return result;
    }

    static void ShuffleLayer(List<int> types,Random random) {
        for(int index=types.Count-1;index>0;index--) {
            int other=random.Next(index+1);
            int type=types[index];
            types[index]=types[other];
            types[other]=type;
        }
    }
    // Type selection provenance: WX j38517, KFakeRandom.AddItem(1,type), Roll/RemoveItem.
    // Frequency correction from the shipped game; source draws use Unity float Range.
    sealed class FrequencyPool {
        sealed class Item { public int id,hits; public double initial,current; }
        readonly List<Item> items=new List<Item>();readonly Random random;readonly bool source;readonly IBoardRandom sourceRandom;int draws;float transfer;
        public FrequencyPool(Random random,bool source=false,IBoardRandom sourceRandom=null){this.random=random;this.source=source;this.sourceRandom=sourceRandom;}
        public int Count=>items.Count;
        public void Add(int id,double weight){items.Add(new Item{id=id,initial=weight,current=weight});}
        public void Remove(int id){items.RemoveAll(x=>x.id==id);}
        public int Roll(){
            if(source)return RollSource();
            double sum=items.Sum(x=>x.current);
            if(sum<=1e-12){foreach(var x in items)x.current=x.initial;sum=items.Sum(x=>x.current);}
            double roll=random.NextDouble()*sum;var chosen=items[items.Count-1];
            foreach(var x in items){roll-=x.current;if(roll<0){chosen=x;break;}}
            draws++;chosen.hits++;
            double delta=(double)chosen.hits/draws-chosen.initial;
            delta=Math.Max(chosen.current-1,Math.Min(chosen.current,delta));
            double other=items.Where(x=>x!=chosen).Sum(x=>x.initial);
            if(other>0){
                // Bound negative transfers so no probability can become negative.
                foreach(var x in items)if(x!=chosen)delta=Math.Max(delta,-x.current*other/x.initial);
                chosen.current=Math.Max(0,chosen.current-delta);
                foreach(var x in items)if(x!=chosen)x.current=Math.Max(0,x.current+delta*x.initial/other);
            }
            return chosen.id;
        }
        // Float32 cumulative selection / CalProToAlloc / AllocPro: j83882/83885/83886.
        int RollSource(){
            float sum=0;foreach(var x in items)sum+=(float)x.current;
            float roll=sourceRandom.Range(0f,sum),cumulative=0;var chosen=items[items.Count-1];bool hit=false;
            draws++;
            foreach(var x in items){cumulative+=(float)x.current;if(cumulative>roll){chosen=x;hit=true;break;}}
            if(hit){
                chosen.hits++;transfer=(float)chosen.hits/draws-(float)chosen.initial;
                float current=(float)chosen.current;
                if(transfer>0&&current-transfer<0)transfer=current;
                else if(transfer<0&&current-transfer>1)transfer=current-1;
                chosen.current=current-transfer;
            }
            // Deliberately preserve the source signed redistribution, including zero-weight entries.
            // The old port's reset and per-recipient nonnegative clamp change later draws.
            if(transfer!=0){
                float other=0;foreach(var x in items)if(x!=chosen)other+=(float)x.initial;
                float factor=transfer/other;
                foreach(var x in items)if(x!=chosen)x.current=(float)x.current+factor*(float)x.initial;
            }
            return chosen.id;
        }
    }
}
