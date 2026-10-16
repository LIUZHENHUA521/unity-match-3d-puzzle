using System;
using System.Linq;
class TestSourcePlan {
 static void Check(bool b,string s){if(!b)throw new Exception(s);}
 static void Main(){
  Check(ChallengePlan.SourceGroupBudgets(5,new[]{1f,1f}).SequenceEqual(new[]{2,3}),"half to even + last remainder");
  Check(ChallengePlan.SourceGroupBudgets(2,new[]{1f,1f,1f,1f,1f}).SequenceEqual(new[]{0,0,0,0,2}),"zero groups preserved");
  Check(ChallengePlan.SourceGroupBudgets(3,new[]{1f,1f,1f,1f,1f}).SequenceEqual(new[]{1,1,1,1,-1}),"signed remainder preserved");
  var themes=new float[][]{new float[]{0.800000011920929f,1.0f,0.6000000238418579f,0.4000000059604645f,0.800000011920929f,2.0f,2.0f,1.0f,1.0f,2.0f,1.0f,2.0f,0.800000011920929f,2.0f,2.0f,1.0f,2.0f,2.0f,1.0f,0.800000011920929f,0.800000011920929f,0.800000011920929f},new float[]{1.0f,0.800000011920929f,0.6000000238418579f,0.4000000059604645f,2.0f,0.800000011920929f,2.0f,2.0f,1.0f,1.5f,1.5f,2.0f,2.0f,2.0f,2.0f,2.0f,2.0f,2.0f,2.0f,2.0f,2.0f,2.0f},new float[]{0.5f,0.800000011920929f,0.5f,1.0f,2.0f,1.0f,2.0f,1.5f,1.5f,2.0f,2.0f,1.0f,1.5f,2.0f,2.0f,2.0f,2.0f,2.0f,2.0f,0.800000011920929f,0.800000011920929f}};int split=0;
  foreach(var weights in themes)for(int seed=0;seed<1000;seed++){
   var layers=new[]{new ChallengeLayer(150,weights.Length,14),new ChallengeLayer(60,10,18),new ChallengeLayer(45,8,20),new ChallengeLayer(45,7,22)};
   foreach(var l in layers)l.weights=weights;
   var a=ChallengePlan.GenerateSource(seed,weights.Length,layers);var b=ChallengePlan.GenerateSource(seed,weights.Length,layers);
   Check(a.Count==300&&a.Select(x=>x.id).Distinct().Count()==300,"counts seed "+seed);
   Check(a.All(x=>x.type>=0&&x.type<weights.Length),"range");
   Check(a.GroupBy(x=>x.type).All(g=>g.Count()%3==0),"triples seed "+seed);
   Check(a.GroupBy(x=>x.layer).Select(g=>g.Count()).SequenceEqual(new[]{150,60,45,45}),"capacity");
   Check(a.Select(x=>x.type).SequenceEqual(b.Select(x=>x.type)),"determinism");
   if(a.GroupBy(x=>new{x.layer,x.type}).Any(g=>g.Count()%3!=0))split++;
  }
  Check(split>0,"cross-layer splitting");
  Console.WriteLine("PASS source generation: original float32 rounded budgets incl half-even/zero/signed remainder; 3000 seeds across 22/22/21 original weighted pools; capacities, triples, IDs, deterministic replay; split="+split);
 }
}
