using System;
using System.Linq;
class Test {
 static void Main(){int split=0;
  foreach(int pool in new[]{8,18,21,25})for(int seed=0;seed<500;seed++){
   var layers=ChallengePlan.DefaultLayers(pool);
   if(seed%2==0)foreach(var layer in layers)layer.weights=Enumerable.Range(0,pool).Select(i=>(float)(i+1)).ToArray();
   var a=ChallengePlan.Generate(seed,pool,layers);var b=ChallengePlan.Generate(seed,pool,layers);
   if(a.Count!=300||a.Select(x=>x.id).Distinct().Count()!=300||a.Any(x=>x.type<0||x.type>=pool))throw new Exception("counts");
   if(!a.Select(x=>x.type).SequenceEqual(b.Select(x=>x.type)))throw new Exception("seed");
   if(a.GroupBy(x=>x.type).Any(g=>g.Count()%3!=0))throw new Exception("triples");
   if(!a.GroupBy(x=>x.layer).Select(g=>g.Count()).SequenceEqual(new[]{150,60,45,45}))throw new Exception("capacity");
   if(a.GroupBy(x=>new{x.layer,x.type}).Any(g=>g.Count()%3!=0))split++;
  }
  if(split==0)throw new Exception("no splitting");
  bool rejected=false;try{ChallengePlan.Generate(1,8,new[]{new ChallengeLayer(13,2,1)});}catch(ArgumentException){rejected=true;}if(!rejected)throw new Exception("invalid configuration accepted");
  Console.WriteLine("PASS 2000 seeds/configurations: capacities, global triples, type range, unique IDs, deterministic replay; split cases="+split);
 }
}
