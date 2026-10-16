using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SourceGenerationValidation {
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run(){
        bool auto=Physics.autoSimulation;string report="";
        try{
            Physics.autoSimulation=false;
            Check(ChallengePlan.SourceGroupBudgets(5,new[]{1f,1f}).SequenceEqual(new[]{2,3}),"source ties-to-even / final remainder");
            Check(ChallengePlan.SourceGroupBudgets(2,new[]{1f,1f,1f,1f,1f}).SequenceEqual(new[]{0,0,0,0,2}),"zero group budget preserved");
            Check(ChallengePlan.SourceGroupBudgets(3,new[]{1f,1f,1f,1f,1f}).SequenceEqual(new[]{1,1,1,1,-1}),"signed final remainder preserved");
            string[] themes={"CountrysideStove","GroceryStore","Barbecue"};int[] pools={22,22,21};
            for(int theme=0;theme<themes.Length;theme++){
                var profile=ReplicaSourceLevelPlan.Get(themes[theme]);
                Check(profile.prefabs.Length==pools[theme]&&profile.prefabs.All(x=>x),"complete source item pool "+themes[theme]);
                Check(profile.prefabs.Take(4).All(x=>x.name.StartsWith("Goose_")),"source special-item order");
                Check(profile.layers.Select(x=>x.height).SequenceEqual(new[]{14f,18f,20f,22f}),"per-theme source layer heights");
                Check(profile.layers[0].types==pools[theme]&&profile.layers.All(x=>x.weights.Length==pools[theme]),"source type counts and weights");
                for(int seed=0;seed<100;seed++){
                    var tiles=ChallengePlan.GenerateSource(seed,pools[theme],profile.layers);
                    Check(tiles.Count==300&&tiles.GroupBy(x=>x.type).All(g=>g.Count()%3==0),"source population / triples "+seed);
                    Check(tiles.GroupBy(x=>x.layer).Select(g=>g.Count()).SequenceEqual(new[]{150,60,45,45}),"source layer capacity "+seed);
                }
                ReplicaFlow.Instance.Begin(theme);ReplicaFlow.Instance.NextStage();SourceCreationTestSupport.FinishPending(ReplicaFlow.Instance);
                var pile=ReplicaLayeredPile.Instance;var board=LevelGenerator.Instance.objectsOnScene;
                Check(board.Count==300,"runtime complete source level");
                Check(board.Where(o=>o.GetComponentInChildren<Renderer>().enabled).All(o=>o.transform.localScale.sqrMagnitude>0&&pile.CanPick(o.GetComponent<Rigidbody>())),"initial visible objects full-size and creation-complete");
                Check(board.Where(o=>!o.GetComponentInChildren<Renderer>().enabled).All(o=>o.GetComponentsInChildren<Collider>().Any(c=>c.enabled&&c.gameObject.layer==10)),"initial hidden items retain real collision support");
                float touch=profile.limitHeight-ReplicaSourceProfile.VisibleMin*.25f;
                float depth=Mathf.Lerp(4,5,Mathf.Clamp01((21-touch)/21));
                Check(Mathf.Abs(pile.Threshold-(touch-depth))<.001f,"initial threshold from LimitHeight, not spawn peak");
                foreach(var obj in board)obj.GetComponent<Rigidbody>().position=Vector3.up*10;
                pile.RefreshVisibility();float once=pile.Threshold;
                Check(once<touch-depth,"new record low updates threshold");
                for(int i=0;i<5;i++)pile.RefreshVisibility();
                Check(Mathf.Abs(pile.Threshold-once)<.00001f,"unchanged peak does not repeatedly lower threshold");
                foreach(var obj in board)obj.GetComponent<Rigidbody>().position=Vector3.up*9;
                pile.RefreshVisibility();Check(pile.Threshold<once,"another record low advances threshold");
                Check(board.Count==300,"reveal creates no new objects");
                ReplicaFlow.Instance.Home();
                report+=themes[theme]+": original pool "+pools[theme]+", heights14/18/20/22, source weights, 100 seeds; limit-height visibility and record-low update PASS.\n";
            }
            report+="PASS source generation catalog, signed rounding fixtures, 300 weighted plans and runtime visibility.\n";
        }catch(Exception e){report+="FAIL "+e+"\n";}
        finally{if(ReplicaFlow.Instance)ReplicaFlow.Instance.Home();Physics.autoSimulation=auto;File.WriteAllText("Temp/goose-source-generation-validation.txt",report);Debug.Log(report);}
    }
}
