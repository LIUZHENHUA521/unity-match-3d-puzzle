using System.Collections.Generic;
using UnityEngine;

// Height-driven rigid-body state. No active-count budget or timed forced freezing.
public class ReplicaPilePhysics : MonoBehaviour {
    public static ReplicaPilePhysics Instance {get;private set;}
    class Entry {public Rigidbody body;public CollisionDetectionMode detection;public bool selected;}
    readonly Dictionary<Rigidbody,Entry> entries=new Dictionary<Rigidbody,Entry>();
    float heightCheckElapsed;
    public double LastStepMilliseconds {get;private set;}
    static bool Eligible(Rigidbody body,float? thresholdOverride=null)=>!ReplicaLayeredPile.Instance||ReplicaLayeredPile.Instance.NeedsPhysics(body,thresholdOverride);
    public int ActiveCount {get {int count=0;foreach(var e in entries.Values)if(e.body&&!e.body.isKinematic&&!e.body.IsSleeping())count++;return count;}}
    public int DynamicCount {get {int count=0;foreach(var e in entries.Values)if(e.body&&!e.body.isKinematic)count++;return count;}}
    public int BoardCount=>entries.Count;
    public bool IsSettling {get {foreach(var e in entries.Values)if(e.body&&!e.selected&&((e.body.isKinematic&&Eligible(e.body))||(!e.body.isKinematic&&!e.body.IsSleeping())))return true;return false;}}
    void Awake(){Instance=this;}
    void OnDestroy(){if(Instance==this)Instance=null;}
    public void SetSelected(Rigidbody body,bool selected){if(body&&entries.TryGetValue(body,out var entry))entry.selected=selected;}
    public bool Contains(Rigidbody body)=>body&&entries.ContainsKey(body);
    public void Register(Rigidbody body,bool sourceCreation=false){
        if(!body||entries.ContainsKey(body))return;
        var entry=new Entry{body=body,detection=body.collisionDetectionMode};entries.Add(body,entry);
        if(sourceCreation)Park(body);else if(Eligible(body))Activate(entry);else Park(body);
    }
    static void Park(Rigidbody body){
        if(!SourceSelectionMode)body.collisionDetectionMode=CollisionDetectionMode.Discrete;
        body.isKinematic=true;
    }
    static void Activate(Entry entry){
        var body=entry.body;body.isKinematic=false;
        // Source SetRigid only changes isKinematic; no mode setters or explicit wake.
        if(!SourceSelectionMode){body.useGravity=true;body.collisionDetectionMode=entry.detection;body.WakeUp();}
    }
    static void ApplyShake(Rigidbody body){
        var inward=new Vector3(-body.position.x,0,.15f-body.position.z);
        body.velocity=Vector3.ClampMagnitude(body.velocity,1);
        body.AddForce(inward*.3f+Vector3.up*1.9f,ForceMode.VelocityChange);
        body.AddTorque(Random.insideUnitSphere*2,ForceMode.VelocityChange);
    }
    public void RemoveForPick(Rigidbody body){if(!body||!entries.Remove(body))return;WakeNear(body.position);}
    public void WakeSelectionNeighbours(Vector3 position){WakeNear(position);}
    static bool SourceSelectionMode=>InputController.Instance&&InputController.Instance.UsesSourceFlight;
    public void WakeNear(Vector3 position){
        if(SourceSelectionMode)return; // No source selection/collection radius wake scan.

        foreach(var entry in entries.Values){
            var body=entry.body;if(!body||!Eligible(body))continue;
            var p=body.position;
            if(p.y>=position.y-.35f&&new Vector2(p.x-position.x,p.z-position.z).sqrMagnitude<.9f*.9f){
                if(body.isKinematic)Activate(entry);else body.WakeUp();
            }
        }
    }
    public void ShakeAll(){
        foreach(var entry in entries.Values){if(!entry.body||!Eligible(entry.body))continue;if(entry.body.isKinematic)Activate(entry);ApplyShake(entry.body);}
    }
    void FixedUpdate(){if(Physics.autoSimulation)Step(Time.fixedDeltaTime);}
    public void Step(float delta){
        var started=System.Diagnostics.Stopwatch.GetTimestamp();
        heightCheckElapsed+=delta;
        // Layered levels share the source optimization loop's height/rigid/visible ordering.
        if((!ReplicaLayeredPile.Instance||!ReplicaLayeredPile.Instance.Running)&&heightCheckElapsed>=ReplicaSourceProfile.OptimizationInterval){
            heightCheckElapsed=0;RefreshHeightStates();
        }
        LastStepMilliseconds=(System.Diagnostics.Stopwatch.GetTimestamp()-started)*1000.0/System.Diagnostics.Stopwatch.Frequency;
    }
    public void RefreshHeightStates(){
        foreach(var entry in entries.Values)RefreshHeightState(entry.body);
    }
    public void RefreshHeightState(Rigidbody body,float? thresholdOverride=null){
        if(!body||!entries.TryGetValue(body,out var entry)||entry.selected)return;
        if(Eligible(body,thresholdOverride)){if(body.isKinematic)Activate(entry);}
        else if(!body.isKinematic&&body.velocity.sqrMagnitude<=.04f)Park(body);
    }
    public void Clear(){entries.Clear();heightCheckElapsed=0;LastStepMilliseconds=0;}
}
