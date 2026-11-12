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
    static bool Finite(float n)=>!float.IsNaN(n)&&!float.IsInfinity(n)&&Mathf.Abs(n)<=50;
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
    public bool IsValid(int count,int pool)=>id>=0&&id<count&&type>=0&&type<pool&&Vector(position,-50,50)&&Vector(scale,.00001f,100)&&Number(rotation.x,-1,1)&&Number(rotation.y,-1,1)&&Number(rotation.z,-1,1)&&Number(rotation.w,-1,1)&&Mathf.Abs(Quaternion.Dot(rotation,rotation)-1)<.002f;
}

public partial class Match3DFlow {
    public Match3DDailyStage CaptureDailyOpening(){
        if(CreationInProgress||!layeredPile.Running||!pilePhysics.DailyOpeningHeld)throw new InvalidOperationException("请在开局生成完毕、操作物品之前保存布局");
        var state=layeredPile.CaptureDailyState(Stage);
        state.items=LevelGenerator.Instance.objectsOnScene.Where(o=>o).Select(o=>{
            var body=o.GetComponent<Rigidbody>();
            return new Match3DDailyItem{id=o.GetComponent<Match3DTile>().id,type=int.Parse(o.name.Substring(5)),position=o.transform.position,rotation=o.transform.rotation,scale=o.transform.localScale,visible=layeredPile.DailyVisible(body),dynamic=pilePhysics.OpeningBodyWillMove(body)};
        }).OrderBy(o=>o.id).ToArray();
        return state;
    }
    void SpawnDailyOpening(GameObject[] prefabs,bool firstStage,Match3DThemePresentation reusedPresentation,GameObject reusedBoundary){
        var state=dailyLevel.opening.stages[Stage-1];
        var seconds=Stage==2?dailyLevel.seconds:SourceProfile(true).seconds;
        InitialCount=state.items.Length;LastResult="";CreationFailure=null;CreationInProgress=true;
        GameManager.Instance.currentLevelTime=seconds;GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;
        if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.Prepare(CurrentTheme.id,Stage,runDate,InitialCount,seconds);
        ItemColliderGeometry.ConfigureLayers();BoardPhysicsSettings.Begin();
        BoardEnvironment.Select(CurrentTheme.sourceEnvironment,firstStage);
        layeredPile.Begin(false,reusedPresentation,reusedBoundary,state.maxHeight,firstStage);
        pilePhysics.HoldDailyOpening();layeredPile.RestoreDailyState(state);
        int version=++creationVersion;
        StartCoroutine(RestoreDailyItems(state,prefabs,version));
    }
    IEnumerator RestoreDailyItems(Match3DDailyStage state,GameObject[] prefabs,int version){
        int batch=0;
        foreach(var item in state.items){
            if(version!=creationVersion)yield break;
            var tile=new ChallengeTile{id=item.id,type=item.type};
            var obj=CreateBoardItem(tile,prefabs[item.type],item.position,item.rotation,true,false,item);
            layeredPile.RestoreDailyVisibility(obj.GetComponent<Rigidbody>(),item.visible);
            pilePhysics.RememberDailyBody(obj.GetComponent<Rigidbody>(),item.dynamic);
            if(++batch%12==0)yield return null;
        }
        Physics.SyncTransforms();
        var repair=PrepareOpening(version);
        while(version==creationVersion){
            bool next;
            try{next=repair.MoveNext();}catch(Exception e){FailOpening(e);yield break;}
            if(!next)break;yield return null;
        }
        if(version!=creationVersion)yield break;
        CreationInProgress=false;
        // Cache the repaired opening for retry; never write server data from the client.
        dailyLevel.opening.stages[Stage-1]=CaptureDailyOpening();
        GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;
        if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.StartRecord();
        BoosterManager.Instance.BoostersInteractibility(true);
    }
#if UNITY_EDITOR || DAILY_WEB_PREVIEW
    public void PreviewDailyOpening(Match3DDailyLevel level,int stage){
        LocalPreview=true;BeginDailyPractice(0,level);LocalPreview=true;unlimitedMagnetRun=true;
        if(stage==2){StopBoard();Stage=2;StartStage();}
    }
#endif
    public string CreationFailure {get;private set;}
    IEnumerator PrepareOpening(int version){
        pilePhysics.HoldOpeningForRepair();
        var repair=new BoardOpeningSeparation(LevelGenerator.Instance.objectsOnScene,layeredPile.DailyVisible,Stage==1,layeredPile.Threshold);
        while(!repair.Complete){
            if(version!=creationVersion)yield break;
            var slice=System.Diagnostics.Stopwatch.StartNew();
            do{repair.Step();}while(!repair.Complete&&slice.Elapsed.TotalMilliseconds<CreationSliceBudgetMilliseconds);
            if(!repair.Complete)yield return 0f;
        }
        if(version!=creationVersion)yield break;
        repair.Apply();pilePhysics.PrepareOpeningRelease();
    }
    void FailOpening(Exception error){
        Debug.LogException(error);
        string message="开局布局无法完成，请重新选择关卡";
        Home();CreationFailure=message;Notice(message);
    }
    public string DailyShareQuery(){
        return dailyLevel==null?"":"dailyDate="+Uri.EscapeDataString(dailyLevel.date)+"&dailyTier="+Uri.EscapeDataString(dailyLevel.tier);
    }
}
