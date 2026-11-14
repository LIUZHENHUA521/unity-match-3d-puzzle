using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Layer configuration alone controls the shell. Counts and model sizes never resize it.
public sealed class PotCapacityPlan {
    public const float MaximumHeight=4096; // Preserve existing snapshot validation compatibility.
    public float Ceiling {get;private set;}=SoftTransitionContainer.ConeHeight;
    public float CylinderStart {get;private set;}=SoftTransitionContainer.ConeHeight;
    public float BaseTop {get;private set;}
    public readonly Dictionary<int,float> layerTops=new Dictionary<int,float>();
    public static PotCapacityPlan Build(IEnumerable<ChallengeTile> tiles){
        var groups=tiles.GroupBy(t=>t.layer).OrderBy(g=>g.Key).ToArray();
        var plan=FromHeights(groups.Select(g=>g.First().height).ToArray());
        for(int i=0;i<groups.Length;i++)plan.layerTops.Add(groups[i].Key,i<4?BoardEnvironment.MapHeight(groups[i].First().height,plan.BaseTop):groups[i].First().height);
        return plan;
    }
    static PotCapacityPlan FromHeights(float[] heights){
        var plan=new PotCapacityPlan{BaseTop=heights.Take(4).Last()};
        if(heights.Length>4){
            plan.CylinderStart=BoardEnvironment.MapHeight(heights[3],plan.BaseTop);
            plan.Ceiling=Mathf.Max(SoftTransitionContainer.ConeHeight,heights.Last()+2);
        }
        return plan;
    }
    public void Apply()=>SoftTransitionContainer.ConfigureHeight(Ceiling,CylinderStart);
    public static void ConfigureLayers(ChallengeLayer[] layers)=>FromHeights(layers.Select(l=>l.height).ToArray()).Apply();
}
