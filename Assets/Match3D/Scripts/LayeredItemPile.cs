using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Runtime geometry and visibility for the solo layered challenge. No objects are added on a match.
public sealed class LayeredItemPile : MonoBehaviour {
    public static LayeredItemPile Instance {get;private set;}
    public bool Running {get;private set;}
    public bool Creating {get;private set;}
    bool creationGrowthRunning;float creationClock;
    bool ownsCreationTiming;float previousMaximumDeltaTime;
    public const float CreationGrowSeconds=.22f;
    public const int CreationGrowthWriteLimit=12,CreationReleaseLimit=3,CreationGrowingLimit=24;
    public int LastCreationGrowthWrites {get;private set;}
    public int LastCreationReleases {get;private set;}
    int creationGrowthCursor;
    float touchHeight,maxHeight,initialVisibleThreshold,optimizationElapsed;
    Match3DThemePresentation presentation;
    public float Threshold {get;private set;}
    public float SourceTouchHeight=>touchHeight;
    public float SourceRecordedMaxHeight=>maxHeight;
    // Record the selection height before click handling; defer the visibility scan.
    // Behavior provenance: docs/refactoring/2026-11-04-naming.md.
    public void RecordSelectionHeight(float height){if(height<touchHeight)touchHeight=Mathf.Max(height,touchHeight-BoardLayoutSettings.MaxHeightStep);}
    // Physics activity follows a separate dynamic height threshold with depth=.5.
    public float PhysicsThreshold {get;private set;}=float.PositiveInfinity;
    public bool NeedsPhysics(Rigidbody body,float? thresholdOverride=null){
        if(!Running)return true;if(!body)return false;
        entriesByBody.TryGetValue(body,out var e);
        // Keep growing items active even when they fall below the physics threshold.
        return body.position.y>=(thresholdOverride??PhysicsThreshold)||(e!=null&&(e.growth<1||(Creating&&e.creationScale<1)));
    }
    public int HiddenCount => entries.Count(e=>e.body&&!e.visible);
    class Entry { public Rigidbody body;public Renderer[] renderers;public Vector3 scale;public ItemColliderGeometry sourceColliders;public bool visible,visibilityInitialized;public bool changeObjScale;public float creationStarted;public float growth=1;public float creationScale=1;public bool finishCreate=true; }
    readonly List<Entry> entries=new List<Entry>();
    readonly Dictionary<Rigidbody,Entry> entriesByBody=new Dictionary<Rigidbody,Entry>();
    readonly List<Entry> creationGrowing=new List<Entry>();
    readonly List<Entry> creationPhysics=new List<Entry>();
    public int CreationGrowingCount=>creationGrowing.Count;
    public int CreationPhysicsCount=>creationPhysics.Count;
    GameObject boundary;
    void Awake(){Instance=this;}
    void OnDestroy(){Clear();if(Instance==this)Instance=null;}
    public static float Radius(float y)=>BoardLayoutSettings.HalfExtent(y);
    public void Begin(bool creating=false,Match3DThemePresentation reusedPresentation=null,GameObject reusedBoundary=null,float? limitHeight=null,bool firstStage=false){
        Clear();optimizationElapsed=0;Running=true;Creating=creating;creationGrowthRunning=creating;creationClock=0;
        if(creating){
            // Avoid a loading stall feeding a long physics catch-up into the next
            // frame. The board is input-locked; restore normal timing before PLAY.
            previousMaximumDeltaTime=Time.maximumDeltaTime;ownsCreationTiming=true;
            Time.maximumDeltaTime=Mathf.Min(previousMaximumDeltaTime,Time.fixedDeltaTime*2);
        }
        // LoopCreateObj j65137: scene-independent first-row LimitHeight seeds both trackers.
        maxHeight=limitHeight??LevelLayoutConfig.Get(BoardEnvironment.CurrentName).limitHeight;
        touchHeight=maxHeight-BoardLayoutSettings.VisibleMin*.25f;
        initialVisibleThreshold=maxHeight-BoardLayoutSettings.VisibleMin*1.5f;
        Threshold=initialVisibleThreshold;UpdatePhysicsThreshold();
        if(reusedPresentation){
            // MainLevel and the layered challenge use the same source theme.
            // Keep the already-rendered presentation and camera pose alive
            // across the stage transition instead of synchronously instantiating
            // the complete theme prefab a second time.
            presentation=reusedPresentation;
            presentation.Fit();
        }else{
            if(!presentation)presentation=gameObject.AddComponent<Match3DThemePresentation>();
            presentation.Begin(BoardEnvironment.CurrentName);
        }
        boundary=reusedBoundary??BoardEnvironment.CreateBoundary(firstStage);
        if(reusedBoundary&&!firstStage)BoardEnvironment.UseSecondStageBoundary(boundary);
        boundary.name=BoardEnvironment.CurrentName+(firstStage?" first stage container":" short transition container");
        boundary.SetActive(true);
        // Creation growth is advanced from LateUpdate.  Keeping it on the
        // frame loop avoids a WebGL coroutine resuming several WaitForSeconds
        // continuations together after a loading frame, which made the whole
        // pile look as if it appeared at once.
    }
    public void TakePresentation(out Match3DThemePresentation scene,out GameObject shell){
        scene=presentation;shell=boundary;presentation=null;boundary=null;
    }
    public static List<Vector3> Positions(int count,float minHeight,float maxHeight,System.Random random){
        var result=new List<Vector3>(count);
        for(int i=0;i<count;i++)result.Add(BoardEnvironment.Position(minHeight,maxHeight,random));
        return result;
    }
    // WX ObjectUnit._changeObjScale is a nonserialized private bool with no observed true write.
    // The optional argument preserves the dormant source branch for explicit evidence/fixtures only.
    public void Register(Rigidbody body,bool sourceChangeObjScale=false,float? initialHeight=null){
        if(!Running||!body||entriesByBody.ContainsKey(body))return;
        var token=body.GetComponent<BoardItemPoolToken>();token?.CacheComponents();
        // Pool.Get restores every cached renderer before registration. Other callers
        // retain their authored enabled-filter behavior.
        var e=new Entry{creationStarted=creationClock,body=body,renderers=token?token.EnabledRenderers():body.GetComponentsInChildren<Renderer>(true).Where(r=>r.enabled).ToArray(),scale=body.transform.localScale,sourceColliders=body.GetComponent<ItemColliderGeometry>(),changeObjScale=sourceChangeObjScale};entries.Add(e);
        entriesByBody.Add(body,e);
        // Init clears _isFinishCreate before this first SetVisible; FinishCreate later restores it.
        // Initial growth is controlled by LoopCrowObjSize, not the later reveal tween.
        e.finishCreate=!Creating;
        // Source CreateInfo stores visibility from the sampled spawn height.
        // A freshly cloned pool body may still report its template's -1000 Y
        // until Transform/physics synchronization; never use that for creation.
        SetVisible(e,IsInitiallyVisible(initialHeight??body.position.y),false);
        // _growUpInCreate is true in the source constructor. It is separate from reveal tween.
        if(Creating&&e.visible){e.creationScale=0;body.transform.localScale=Vector3.zero;}
        if(Creating){if(e.visible)creationGrowing.Add(e);else creationPhysics.Add(e);}
    }
    public bool IsInitiallyVisible(float height)=>height>=initialVisibleThreshold;
    public bool CanPick(Rigidbody body){return !body||!entriesByBody.TryGetValue(body,out var e)||(e.visible&&e.finishCreate&&e.growth>=1);}
    // Boosters may deliberately collect a hidden item; tray scale must always use the full board scale.
    public void Detach(Rigidbody body){
        if(!body||!entriesByBody.TryGetValue(body,out var e))return;
        entriesByBody.Remove(body);
        creationGrowing.Remove(e);creationPhysics.Remove(e);
        SetGrowth(e,1);body.GetComponent<SpecialItemAnimation>()?.SetSourceVisible(true);if(e.sourceColliders)e.sourceColliders.SetVisible(true);body.transform.localScale=e.scale;foreach(var r in e.renderers)if(r)r.enabled=true;entries.Remove(e);
    }
    // CheckObjMaxHeight updates a historical low; it does not itself scan visible/rigid states.
    public float RefreshSourceMaxHeight(){
        RemoveDestroyedEntries();
        float peak=Mathf.Max(0,entries.Select(e=>e.body.position.y).DefaultIfEmpty(0).Max());
        if(peak<maxHeight){maxHeight=peak;if(peak<touchHeight)touchHeight=Mathf.Max(peak,touchHeight-BoardLayoutSettings.MaxHeightStep);}
        return maxHeight;
    }
    public void RefreshVisibility(){
        if(!Running||Creating||(ItemPhysicsController.Instance&&ItemPhysicsController.Instance.DailyOpeningHeld))return;
        RefreshSourceMaxHeight();if(entries.Count==0)return;
        float u=Mathf.Clamp01((BoardLayoutSettings.MinVisibleY-touchHeight)/BoardLayoutSettings.MinVisibleY);
        float depth=Mathf.Lerp(BoardLayoutSettings.VisibleMin,BoardLayoutSettings.VisibleMax,u);
        Threshold=touchHeight-depth;
        PhysicsThreshold=touchHeight-depth*.5f;
        // Source loop checks height, then rigid state, then visibility before its one-second yield.
        if(ItemPhysicsController.Instance)ItemPhysicsController.Instance.RefreshHeightStates();
        foreach(var e in entries)SetVisible(e,e.body.position.y>=Threshold);
    }
    void UpdatePhysicsThreshold(){
        float u=Mathf.Clamp01((BoardLayoutSettings.MinVisibleY-touchHeight)/BoardLayoutSettings.MinVisibleY);
        PhysicsThreshold=touchHeight-Mathf.Lerp(BoardLayoutSettings.VisibleMin,BoardLayoutSettings.VisibleMax,u)*(Creating?1f:.5f);
    }
    void SetVisible(Entry e,bool visible,bool animate=true){
        // Always initialize hidden entries too; their renderers start enabled.
        // Thereafter mirror source j6602's state-change guard.
        if(e.visibilityInitialized&&e.visible==visible)return;
        e.visibilityInitialized=true;
        // j6602 can request growth on a completed renderer transition, but j19265 first
        // requires _changeObjScale. Ordinary WX objects leave this private gate false.
        bool changed=e.visible!=visible;e.visible=visible;
        e.body.GetComponent<SpecialItemAnimation>()?.SetSourceVisible(visible);
        if(e.sourceColliders)e.sourceColliders.SetVisible(visible);
        foreach(var r in e.renderers)if(r)r.enabled=visible;
        if(animate&&visible&&changed&&e.changeObjScale&&e.finishCreate&&e.growth>=1){e.finishCreate=false;SetGrowth(e,0);}
    }

    void SetGrowth(Entry e,float value){
        e.growth=value;if(value>=1)e.finishCreate=true;
        // Dormant DelayToGrowScale branch: Vector3.zero -> DOScale(1,.75), default Ease.OutQuad.
        // Stored scale accounts for this project's imported mesh normalization and size slider.
        float eased=1-(1-value)*(1-value);
        if(e.body&&e.body.transform.localScale!=e.scale*eased)e.body.transform.localScale=e.scale*eased;
    }
    public void StopCreationGrowth(){
        creationGrowthRunning=false;
        creationGrowing.Clear();creationPhysics.Clear();creationGrowthCursor=0;creationClock=0;LastCreationGrowthWrites=0;LastCreationReleases=0;
    }
    public void FinishCreation(){
        if(!Running||!Creating)return;
        StopCreationGrowth();
        foreach(var e in entries)if(e.body){e.creationScale=1;SetGrowth(e,1);}
        Creating=false;RestoreCreationTiming();optimizationElapsed=0;UpdatePhysicsThreshold();RefreshVisibility();
    }
    void StepCreationGrowth(float delta){
        LastCreationGrowthWrites=0;LastCreationReleases=0;
        if(!creationGrowthRunning||delta<=0)return;
        // Advance once per rendered frame. Progress depends on elapsed game time,
        // not on how often an entry wins a slot in the bounded round-robin queue.
        creationClock+=delta;
        CreationGrowthTick();
    }
    void CreationGrowthTick(){
        // Only moving bodies need repeated height checks during locked creation.
        // Retain dynamic bodies even after full growth: they may fall below the
        // band later and must still be parked. Deep kinematic supports stay put.
        for(int i=creationPhysics.Count-1;i>=0;i--){
            var e=creationPhysics[i];
            if(e.body&&ItemPhysicsController.Instance)ItemPhysicsController.Instance.RefreshHeightState(e.body);
            if(!e.body||e.body.isKinematic)creationPhysics.RemoveAt(i);
        }
        // Round-robin preserves progress for old items while new ones join the tail.
        // Scale changes also dirty compound colliders, so budget these separately
        // from cloning. A long frame samples one progress value, without replaying ticks.
        LastCreationGrowthWrites=0;LastCreationReleases=0;
        int remaining=creationGrowing.Count;
        while(remaining-->0&&creationGrowing.Count>0){
            creationGrowthCursor%=creationGrowing.Count;
            var e=creationGrowing[creationGrowthCursor];
            if(!e.body){creationGrowing.RemoveAt(creationGrowthCursor);continue;}
            if(e.creationScale<1){
                if(LastCreationGrowthWrites>=CreationGrowthWriteLimit)break;
                e.creationScale=Mathf.Clamp01((creationClock-e.creationStarted)/CreationGrowSeconds);
                e.body.transform.localScale=e.scale*e.creationScale;
                LastCreationGrowthWrites++;creationGrowthCursor++;
            }else{
                if(LastCreationReleases>=CreationReleaseLimit){creationGrowthCursor++;continue;}
                if(ItemPhysicsController.Instance)ItemPhysicsController.Instance.RefreshHeightState(e.body);
                if(!e.body.isKinematic)creationPhysics.Add(e);
                creationGrowing.RemoveAt(creationGrowthCursor);LastCreationReleases++;
            }
        }
    }

    void LateUpdate(){
        if(!Running)return;
        // The source's creation size loop is frame-driven in the player.  Do
        // this before the normal visibility scan so newly registered objects
        // start progressing on the next rendered frame.
        if(Creating){StepCreationGrowth(Time.deltaTime);return;}
        StepVisibility(Time.deltaTime);
    }
    public void StepVisibility(float delta){
        if(!Running)return;
        if(Creating){StepCreationGrowth(delta);return;}
        // Scaled game time, matching WaitForSeconds; do not catch up many checks after a slow frame.
        foreach(var e in entries)if(e.body&&e.growth<1)SetGrowth(e,Mathf.Min(1,e.growth+delta/BoardLayoutSettings.GrowSeconds));
        // If explicitly enabled, a source reveal tween is independent of LoopOptimization.
        if(BoardTools.Shuffling)return;
        optimizationElapsed+=Mathf.Max(0,delta);
        if(optimizationElapsed>=BoardLayoutSettings.OptimizationInterval){optimizationElapsed=0;RefreshVisibility();}
    }
    public Match3DDailyStage CaptureDailyState(int stage)=>new Match3DDailyStage{stage=stage,touchHeight=touchHeight,maxHeight=maxHeight,threshold=Threshold,physicsThreshold=PhysicsThreshold};
    public bool DailyVisible(Rigidbody body)=>entriesByBody.TryGetValue(body,out var e)&&e.visible;
    public void RestoreDailyState(Match3DDailyStage state){touchHeight=state.touchHeight;maxHeight=state.maxHeight;Threshold=state.threshold;PhysicsThreshold=state.physicsThreshold;optimizationElapsed=0;}
    public void RestoreDailyVisibility(Rigidbody body,bool visible){if(entriesByBody.TryGetValue(body,out var e))SetVisible(e,visible,false);}
    public void FinishSourceShuffle(Rigidbody body){
        if(!body)return;
        entriesByBody.TryGetValue(body,out var e);
        float u=Mathf.Clamp01((BoardLayoutSettings.MinVisibleY-touchHeight)/BoardLayoutSettings.MinVisibleY);
        float threshold=touchHeight-Mathf.Lerp(BoardLayoutSettings.VisibleMin,BoardLayoutSettings.VisibleMax,u);
        // RotaAnim completion is per candidate: scale, visibility, then CheckRigid(depth=1).
        // Do not run another max-height scan or overwrite the normal depth=.5 threshold.
        if(e!=null){SetGrowth(e,1);SetVisible(e,body.position.y>=threshold);}
        if(ItemPhysicsController.Instance)ItemPhysicsController.Instance.RefreshHeightState(body,threshold);
    }
    public void RestoreToolScale(Rigidbody body){if(body&&entriesByBody.TryGetValue(body,out var e))SetGrowth(e,1);}
    void RemoveDestroyedEntries(){
        for(int i=entries.Count-1;i>=0;i--)if(!entries[i].body){
            // Destroyed Unity objects still retain their managed dictionary key.
            creationGrowing.Remove(entries[i]);creationPhysics.Remove(entries[i]);
            entriesByBody.Remove(entries[i].body);entries.RemoveAt(i);
        }
    }
    public Vector3 ReturnPosition(){
        float y=entries.Count==0?1:entries.Where(e=>e.body).Select(e=>e.body.position.y).DefaultIfEmpty(1).Max()+.7f;
        if(BoardEnvironment.IsFirstStage)return new Vector3(0,Mathf.Min(y,36),.15f);
        return BoardEnvironment.GetBorder(Mathf.Min(y,SoftTransitionContainer.SpawnCeiling)).position;
    }
    void RestoreCreationTiming(){
        if(!ownsCreationTiming)return;
        Time.maximumDeltaTime=previousMaximumDeltaTime;ownsCreationTiming=false;
    }
    public void Clear(){
        RestoreCreationTiming();
        Running=false;Creating=false;StopCreationGrowth();
        foreach(var e in entries)if(e.body){SetGrowth(e,1);if(e.sourceColliders)e.sourceColliders.SetVisible(true);foreach(var r in e.renderers)if(r)r.enabled=true;}
        entries.Clear();entriesByBody.Clear();if(presentation)presentation.End();presentation=null;
        if(boundary){boundary.SetActive(false);Destroy(boundary);}
        boundary=null;
    }
}
