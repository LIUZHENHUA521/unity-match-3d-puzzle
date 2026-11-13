using System;

// Presentation only: board removal counts and gameplay records remain authoritative.
public sealed class Match3DProgressDisplay {
    public const string TestKey="goose_progress_display_ab";
    public string Group {get;private set;}="A";
    public float HiddenWeight {get;private set;}=1f;
    public float Value {get;private set;}
    public void BeginRun(string group){
        Group=group=="B"||group=="C"||group=="D"?group:"A";
        HiddenWeight=Group=="B"?.5f:Group=="C"?.7f:Group=="D"?.3f:1f;
        ResetStage();
    }
    public void ResetStage(){Value=0;}
    public float Sample(int initial,int remaining,int hidden,bool won=false){
        if(won)return Value=1f;
        if(initial<=0)return Value;
        remaining=Math.Max(0,Math.Min(initial,remaining));
        hidden=Math.Max(0,Math.Min(remaining,hidden));
        int removed=initial-remaining;
        float denominator=removed+(remaining-hidden)+HiddenWeight*hidden;
        float next=denominator>0?removed/denominator:0;
        return Value=Math.Min(.99f,Math.Max(Value,next));
    }
    public string BestKey(string theme,int stage)=>"goose.result.best."+theme+"."+stage+(Group=="A"?"":".progress_v1_"+Group);
}
