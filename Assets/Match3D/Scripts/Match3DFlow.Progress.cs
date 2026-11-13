using UnityEngine;

public partial class Match3DFlow {
    readonly Match3DProgressDisplay progressDisplay=new Match3DProgressDisplay();
    float nextProgressSample;
    void BeginProgressRun(){progressDisplay.BeginRun(Match3DABExperiments.GetABGroup(Match3DProgressDisplay.TestKey));}
    float DisplayProgress(bool won=false){
        if(CreationInProgress)return 0;
        return progressDisplay.Sample(InitialCount,LevelGenerator.Instance.objectsOnScene.Count,layeredPile?layeredPile.HiddenCount:0,won);
    }
    void TickProgressDisplay(){
        if(IsPvp||CreationInProgress||Time.unscaledTime<nextProgressSample)return;
        nextProgressSample=Time.unscaledTime+.2f;
        DisplayProgress();
    }
}
