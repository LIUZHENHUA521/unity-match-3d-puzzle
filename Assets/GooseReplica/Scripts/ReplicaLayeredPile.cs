using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Runtime geometry and visibility for the solo layered challenge. No objects are added on a match.
public sealed class ReplicaLayeredPile : MonoBehaviour {
    public static ReplicaLayeredPile Instance {get;private set;}
    public bool Running {get;private set;}
    public bool Creating {get;private set;}
    bool creationGrowthRunning;float creationGrowthElapsed;Coroutine creationGrowthRoutine;
    public const float CreationGrowthInterval=.02f,CreationGrowthStep=.1f;
    float touchHeight,maxHeight,initialVisibleThreshold,optimizationElapsed;
    ReplicaThemePresentation presentation;
    public float Threshold {get;private set;}
    public float SourceTouchHeight=>touchHeight;
    public float SourceRecordedMaxHeight=>maxHeight;
    // GetUnit(playSound=true) applies SetMinTouchHeight before BeClick, without an immediate scan.
    public void RecordSourceTouchHeight(float height){if(height<touchHeight)touchHeight=Mathf.Max(height,touchHeight-ReplicaSourceProfile.MaxHeightStep);}
    // Source CheckRigid uses the dynamic visibility formula with depth=.5.
    public float PhysicsThreshold {get;private set;}=float.PositiveInfinity;
    public bool NeedsPhysics(Rigidbody body,float? thresholdOverride=null){
        if(!Running)return true;if(!body)return false;
        var e=entries.Find(x=>x.body==body);
        // CheckRigid freezes below threshold only after growth has finished.
        return body.position.y>=(thresholdOverride??PhysicsThreshold)||(e!=null&&(e.growth<1||(Creating&&e.creationScale<.99f)));
    }
    public int HiddenCount => entries.Count(e=>e.body&&!e.visible);
    class Entry { public Rigidbody body;public Renderer[] renderers;public Vector3 scale;public ReplicaSourceColliders sourceColliders;public bool visible;public bool changeObjScale;public float growth=1;public float creationScale=1;public bool finishCreate=true; }
    readonly List<Entry> entries=new List<Entry>();
    Transform pot;Collider[] oldWalls;bool[] wallStates;GameObject boundary;
    void Awake(){Instance=this;}
    void OnDestroy(){Clear();if(Instance==this)Instance=null;}
    public static float Radius(float y)=>ReplicaSourceProfile.HalfExtent(y);
    public void Begin(bool creating=false,ReplicaThemePresentation reusedPresentation=null,GameObject reusedBoundary=null){
        Clear();optimizationElapsed=0;Running=true;Creating=creating;creationGrowthRunning=creating;creationGrowthElapsed=0;
        // LoopCreateObj j65137: scene-independent first-row LimitHeight seeds both trackers.
        maxHeight=ReplicaSourceLevelPlan.Get(ReplicaSourceEnvironment.CurrentName).limitHeight;
        touchHeight=maxHeight-ReplicaSourceProfile.VisibleMin*.25f;
        initialVisibleThreshold=maxHeight-ReplicaSourceProfile.VisibleMin*1.5f;
        Threshold=initialVisibleThreshold;UpdatePhysicsThreshold();
        if(reusedPresentation){
            // MainLevel and the layered challenge use the same source theme.
            // Keep the already-rendered presentation and camera pose alive
            // across the stage transition instead of synchronously instantiating
            // the complete theme prefab a second time.
            presentation=reusedPresentation;
            presentation.Fit();
        }else{
            if(!presentation)presentation=gameObject.AddComponent<ReplicaThemePresentation>();
            presentation.Begin(ReplicaSourceEnvironment.CurrentName);
        }
        pot=GameObject.Find("Cooking Pot")?.transform;
        if(pot){
            oldWalls=pot.GetComponentsInChildren<Collider>();
            wallStates=oldWalls.Select(c=>c.enabled).ToArray();foreach(var c in oldWalls)c.enabled=false;
        }
        boundary=reusedBoundary??ReplicaSourceEnvironment.CreateBoundary();
        boundary.name=ReplicaSourceEnvironment.CurrentName+" source collision reference";
        boundary.SetActive(true);
        // Creation growth is advanced from LateUpdate.  Keeping it on the
        // frame loop avoids a WebGL coroutine resuming several WaitForSeconds
        // continuations together after a loading frame, which made the whole
        // pile look as if it appeared at once.
        creationGrowthRoutine=null;
    }
    public static List<Vector3> Positions(int count,float minHeight,float maxHeight,System.Random random){
        var result=new List<Vector3>(count);
        for(int i=0;i<count;i++)result.Add(ReplicaSourceEnvironment.Position(minHeight,maxHeight,random));
        return result;
    }
    // WX ObjectUnit._changeObjScale is a nonserialized private bool with no observed true write.
    // The optional argument preserves the dormant source branch for explicit evidence/fixtures only.
    public void Register(Rigidbody body,bool sourceChangeObjScale=false){
        if(!Running||!body||entries.Any(e=>e.body==body))return;
        var e=new Entry{body=body,renderers=body.GetComponentsInChildren<Renderer>(true).Where(r=>r.enabled).ToArray(),scale=body.transform.localScale,sourceColliders=body.GetComponent<ReplicaSourceColliders>(),changeObjScale=sourceChangeObjScale};entries.Add(e);
        // Init clears _isFinishCreate before this first SetVisible; FinishCreate later restores it.
        // Initial growth is controlled by LoopCrowObjSize, not the later reveal tween.
        e.finishCreate=!Creating;
        SetVisible(e,body.position.y>=initialVisibleThreshold,false);
        // _growUpInCreate is true in the source constructor. It is separate from reveal tween.
        if(Creating&&e.visible){e.creationScale=0;body.transform.localScale=Vector3.zero;}
    }
    public bool CanPick(Rigidbody body){var e=entries.Find(x=>x.body==body);return e==null||(e.visible&&e.finishCreate&&e.growth>=1);}
    // Boosters may deliberately collect a hidden item; tray scale must always use the full board scale.
    public void Detach(Rigidbody body){
        var e=entries.Find(x=>x.body==body);if(e==null)return;
        SetGrowth(e,1);body.GetComponent<ReplicaSourceGooseAnimation>()?.SetSourceVisible(true);if(e.sourceColliders)e.sourceColliders.SetVisible(true);body.transform.localScale=e.scale;foreach(var r in e.renderers)if(r)r.enabled=true;entries.Remove(e);
    }
    // CheckObjMaxHeight updates a historical low; it does not itself scan visible/rigid states.
    public float RefreshSourceMaxHeight(){
        entries.RemoveAll(e=>!e.body);
        float peak=Mathf.Max(0,entries.Select(e=>e.body.position.y).DefaultIfEmpty(0).Max());
        if(peak<maxHeight){maxHeight=peak;if(peak<touchHeight)touchHeight=Mathf.Max(peak,touchHeight-ReplicaSourceProfile.MaxHeightStep);}
        return maxHeight;
    }
    public void RefreshVisibility(){
        if(!Running||Creating)return;
        entries.RemoveAll(e=>!e.body);if(entries.Count==0)return;
        RefreshSourceMaxHeight();
        float u=Mathf.Clamp01((ReplicaSourceProfile.MinVisibleY-touchHeight)/ReplicaSourceProfile.MinVisibleY);
        float depth=Mathf.Lerp(ReplicaSourceProfile.VisibleMin,ReplicaSourceProfile.VisibleMax,u);
        Threshold=touchHeight-depth;
        PhysicsThreshold=touchHeight-depth*.5f;
        // Source loop checks height, then rigid state, then visibility before its one-second yield.
        if(ReplicaPilePhysics.Instance)ReplicaPilePhysics.Instance.RefreshHeightStates();
        foreach(var e in entries)SetVisible(e,e.body.position.y>=Threshold);
    }
    void UpdatePhysicsThreshold(){
        float u=Mathf.Clamp01((ReplicaSourceProfile.MinVisibleY-touchHeight)/ReplicaSourceProfile.MinVisibleY);
        PhysicsThreshold=touchHeight-Mathf.Lerp(ReplicaSourceProfile.VisibleMin,ReplicaSourceProfile.VisibleMax,u)*(Creating?1f:.5f);
    }
    void SetVisible(Entry e,bool visible,bool animate=true){
        // j6602 can request growth on a completed renderer transition, but j19265 first
        // requires _changeObjScale. Ordinary WX objects leave this private gate false.
        bool changed=e.visible!=visible;e.visible=visible;
        e.body.GetComponent<ReplicaSourceGooseAnimation>()?.SetSourceVisible(visible);
        if(e.sourceColliders)e.sourceColliders.SetVisible(visible);
        foreach(var r in e.renderers)if(r)r.enabled=visible;
        if(animate&&visible&&changed&&e.changeObjScale&&e.finishCreate&&e.growth>=1){e.finishCreate=false;SetGrowth(e,0);}
    }

    void SetGrowth(Entry e,float value){
        e.growth=value;if(value>=1)e.finishCreate=true;
        // Dormant DelayToGrowScale branch: Vector3.zero -> DOScale(1,.75), default Ease.OutQuad.
        // Stored scale accounts for this project's imported mesh normalization and size slider.
        float eased=1-(1-value)*(1-value);
        if(e.body)e.body.transform.localScale=e.scale*eased;
    }
    public void StopCreationGrowth(){
        creationGrowthRunning=false;
        if(creationGrowthRoutine!=null)StopCoroutine(creationGrowthRoutine);
        creationGrowthRoutine=null;
    }
    IEnumerator LoopCreationGrowth(){
        var wait=new WaitForSeconds(CreationGrowthInterval);
        while(creationGrowthRunning){CreationGrowthTick();yield return wait;}
    }
    public void FinishCreation(){
        if(!Running||!Creating)return;
        StopCreationGrowth();
        foreach(var e in entries)if(e.body){e.creationScale=1;SetGrowth(e,1);}
        Creating=false;optimizationElapsed=0;UpdatePhysicsThreshold();RefreshVisibility();
    }
    void StepCreationGrowth(float delta){
        if(!creationGrowthRunning)return;
        creationGrowthElapsed+=Mathf.Max(0,delta);
        // TimerManager.WaitFor0_02Second: one iteration at each resumed wait, no catch-up loop.
        if(creationGrowthElapsed<CreationGrowthInterval)return;
        creationGrowthElapsed=0;CreationGrowthTick();
    }
    void CreationGrowthTick(){
        foreach(var e in entries)if(e.body){
            if(e.creationScale<.99f){e.creationScale+=CreationGrowthStep;e.body.transform.localScale=e.scale*e.creationScale;}
            else if(ReplicaPilePhysics.Instance)ReplicaPilePhysics.Instance.RefreshHeightState(e.body);
        }
    }
    void LateUpdate(){
        if(!Running)return;
        // The source's creation size loop is frame-driven in the player.  Do
        // this before the normal visibility scan so newly registered objects
        // receive their first .1 scale step on the next rendered frame.
        if(Creating){StepCreationGrowth(Time.deltaTime);return;}
        StepVisibility(Time.deltaTime);
    }
    public void StepVisibility(float delta){
        if(!Running)return;
        if(Creating){StepCreationGrowth(delta);return;}
        // Scaled game time, matching WaitForSeconds; do not catch up many checks after a slow frame.
        foreach(var e in entries)if(e.body&&e.growth<1)SetGrowth(e,Mathf.Min(1,e.growth+delta/ReplicaSourceProfile.GrowSeconds));
        // If explicitly enabled, a source reveal tween is independent of LoopOptimization.
        if(ReplicaSourceTools.Shuffling)return;
        optimizationElapsed+=Mathf.Max(0,delta);
        if(optimizationElapsed>=ReplicaSourceProfile.OptimizationInterval){optimizationElapsed=0;RefreshVisibility();}
    }
    public void FinishSourceShuffle(Rigidbody body){
        if(!body)return;
        var e=entries.Find(x=>x.body==body);
        float u=Mathf.Clamp01((ReplicaSourceProfile.MinVisibleY-touchHeight)/ReplicaSourceProfile.MinVisibleY);
        float threshold=touchHeight-Mathf.Lerp(ReplicaSourceProfile.VisibleMin,ReplicaSourceProfile.VisibleMax,u);
        // RotaAnim completion is per candidate: scale, visibility, then CheckRigid(depth=1).
        // Do not run another max-height scan or overwrite the normal depth=.5 threshold.
        if(e!=null){SetGrowth(e,1);SetVisible(e,body.position.y>=threshold);}
        if(ReplicaPilePhysics.Instance)ReplicaPilePhysics.Instance.RefreshHeightState(body,threshold);
    }
    public void RestoreToolScale(Rigidbody body){var e=entries.FirstOrDefault(x=>x.body==body);if(e!=null)SetGrowth(e,1);}
    public Vector3 ReturnPosition(){
        float y=entries.Count==0?1:entries.Where(e=>e.body).Select(e=>e.body.position.y).DefaultIfEmpty(1).Max()+.7f;
        return new Vector3(0,Mathf.Min(y,36),.15f);
    }
    public void Clear(){
        Running=false;Creating=false;StopCreationGrowth();
        foreach(var e in entries)if(e.body){SetGrowth(e,1);if(e.sourceColliders)e.sourceColliders.SetVisible(true);foreach(var r in e.renderers)if(r)r.enabled=true;}
        entries.Clear();if(presentation)presentation.End();
        if(oldWalls!=null)for(int i=0;i<oldWalls.Length;i++)if(oldWalls[i])oldWalls[i].enabled=wallStates[i];
        if(boundary){boundary.SetActive(false);Destroy(boundary);}
        pot=null;oldWalls=null;boundary=null;
    }
}
