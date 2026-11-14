using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable] public sealed class Match3DDailySnapshot {
    public const string ResourceVersion="shared-opening-1";
    public string resourceVersion=ResourceVersion;
    public Match3DDailyStage[] stages;
    public bool IsValid(int count,int pool){
        return resourceVersion==ResourceVersion&&stages!=null&&stages.Length==2&&stages[0]!=null&&stages[1]!=null&&stages[0].stage==1&&stages[1].stage==2&&stages[0].IsValid(15,pool)&&stages[1].IsValid(count,pool);
    }
}
[Serializable] public sealed class Match3DDailyStage {
    public int stage;
    public float touchHeight,maxHeight,threshold,physicsThreshold;
    public Match3DDailyItem[] items;
    static bool Finite(float n)=>!float.IsNaN(n)&&!float.IsInfinity(n)&&n>=-50&&n<=PotCapacityPlan.MaximumHeight;
    public bool IsValid(int count,int pool){
        return Finite(touchHeight)&&Finite(maxHeight)&&Finite(threshold)&&Finite(physicsThreshold)&&items!=null&&items.Length==count&&items.All(i=>i!=null&&i.IsValid(count,pool))&&items.Select(i=>i.id).Distinct().Count()==count&&items.GroupBy(i=>i.type).All(g=>g.Count()%3==0)&&items.Any(i=>i.visible);
    }
}
[Serializable] public sealed class Match3DDailyItem {
    public int id,type;
    public Vector3 position,scale;
    public Quaternion rotation;
    public bool visible,dynamic;
    static bool Number(float n,float min,float max)=>!float.IsNaN(n)&&!float.IsInfinity(n)&&n>=min&&n<=max;
    static bool Vector(Vector3 v,float min,float max)=>Number(v.x,min,max)&&Number(v.y,min,max)&&Number(v.z,min,max);
    public bool IsValid(int count,int pool)=>id>=0&&id<count&&type>=0&&type<pool&&Number(position.x,-50,50)&&Number(position.z,-50,50)&&Number(position.y,-50,PotCapacityPlan.MaximumHeight)&&Vector(scale,.00001f,100)&&Number(rotation.x,-1,1)&&Number(rotation.y,-1,1)&&Number(rotation.z,-1,1)&&Number(rotation.w,-1,1)&&Mathf.Abs(Quaternion.Dot(rotation,rotation)-1)<.002f;
}

public partial class Match3DFlow {
    public Match3DDailyStage CaptureDailyOpening(){
        if(CreationInProgress||!layeredPile.Running)throw new InvalidOperationException("请等待关卡生成完毕");
        var state=layeredPile.CaptureDailyState(Stage);
        state.items=LevelGenerator.Instance.objectsOnScene.Where(o=>o).Select(o=>{
            var body=o.GetComponent<Rigidbody>();
            return new Match3DDailyItem{id=o.GetComponent<Match3DTile>().id,type=int.Parse(o.name.Substring(5)),position=o.transform.position,rotation=o.transform.rotation,scale=o.transform.localScale,visible=layeredPile.DailyVisible(body),dynamic=!body.isKinematic};
        }).OrderBy(o=>o.id).ToArray();
        return state;
    }
    void SpawnDailyOpening(GameObject[] prefabs,bool firstStage,Match3DThemePresentation reusedPresentation,GameObject reusedBoundary,bool animate=true){
        var state=dailyLevel.opening.stages[Stage-1];
        var seconds=Stage==2?dailyLevel.seconds:SourceProfile(true).seconds;
        InitialCount=state.items.Length;LastResult="";CreationInProgress=true;
        GameManager.Instance.currentLevelTime=seconds;GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;
        if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.Prepare(CurrentTheme.id,Stage,runDate,InitialCount,seconds);
        ItemColliderGeometry.ConfigureLayers();BoardPhysicsSettings.Begin();
        BoardEnvironment.Select(CurrentTheme.sourceEnvironment,firstStage);
        if(!firstStage)PotCapacityPlan.ConfigureLayers(dailyLevel.layers);
        layeredPile.Begin(false,reusedPresentation,reusedBoundary,state.maxHeight,firstStage);
        pilePhysics.HoldDailyOpening();layeredPile.RestoreDailyState(state);
        int version=++creationVersion;
        StartCoroutine(RestoreDailyItems(state,prefabs,version,animate));
    }
    IEnumerator RestoreDailyItems(Match3DDailyStage state,GameObject[] prefabs,int version,bool animate){
        int batch=0;
        var visible=new List<(Transform target,Vector3 scale)>();
        foreach(var item in state.items){
            if(version!=creationVersion)yield break;
            var tile=new ChallengeTile{id=item.id,type=item.type};
            var obj=CreateBoardItem(tile,prefabs[item.type],item.position,item.rotation,true,false,item);
            layeredPile.RestoreDailyVisibility(obj.GetComponent<Rigidbody>(),item.visible);
            pilePhysics.RememberDailyBody(obj.GetComponent<Rigidbody>(),item.dynamic);
            // Register keeps the full authored scale; only presentation is animated.
            // DailyOpeningHeld keeps colliders stationary until the first player action.
            if(animate&&item.visible){visible.Add((obj.transform,item.scale));obj.transform.localScale=Vector3.zero;}
            if(++batch%12==0)yield return null;
        }
        if(animate){
            visible.Sort((a,b)=>a.target.position.y.CompareTo(b.target.position.y));
            yield return AnimatePotDailyOpening(visible,version);
        }
        if(version!=creationVersion)yield break;
        Physics.SyncTransforms();CreationInProgress=false;
        GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;
        if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.StartRecord();
        BoosterManager.Instance.BoostersInteractibility(true);
    }
    IEnumerator AnimatePotDailyOpening(List<(Transform target,Vector3 scale)> visible,int version){
        var growing=new List<(Transform target,Vector3 scale,float started)>();
        int next=0;float clock=0,nextBatch=0;
        float interval=SourceCreateInterval(visible.Count);
        while(next<visible.Count||growing.Count>0){
            // Paint zero scale before advancing; pauses and cancelled boards never advance.
            yield return null;
            if(version!=creationVersion)yield break;
            if(Time.timeScale<=0)continue;
            // A loading hitch must not consume the whole visible animation in one frame.
            clock+=Mathf.Min(Time.deltaTime,Time.fixedDeltaTime*2);
            for(int i=growing.Count-1;i>=0;i--){
                var item=growing[i];
                float progress=Mathf.Clamp01((clock-item.started)/LayeredItemPile.CreationGrowSeconds);
                if(item.target)item.target.localScale=item.scale*progress;
                if(progress>=1||!item.target)growing.RemoveAt(i);
            }
            if(clock<nextBatch)continue;
            int count=Mathf.Min(SourceFrameCreateCount,LayeredItemPile.CreationGrowingLimit-growing.Count);
            for(int i=0;i<count&&next<visible.Count;i++){
                var item=visible[next++];growing.Add((item.target,item.scale,clock));
            }
            // One batch per rendered frame, with no catch-up burst after a stall.
            nextBatch=clock+interval;
        }
    }
#if UNITY_EDITOR || DAILY_WEB_PREVIEW
    public void PreviewDailyOpening(Match3DDailyLevel level,int stage){
        LocalPreview=true;BeginDailyPractice(0,level);LocalPreview=true;unlimitedMagnetRun=true;
        if(stage==2){StopBoard();Stage=2;StartStage();}
    }
#endif
    public string DailyShareQuery(){
        return dailyLevel==null?"":"dailyDate="+Uri.EscapeDataString(dailyLevel.date)+"&dailyTier="+Uri.EscapeDataString(dailyLevel.tier);
    }
}
