using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public static class SourceSelectionPhysicsFramesValidation {
    static readonly List<GameObject> probes=new List<GameObject>();
    static void Check(bool ok,string reason){if(!ok)throw new Exception(reason);}
    public static IEnumerator Run(){
        float scale=Time.timeScale;bool simulation=Physics.autoSimulation;string failure=null;
        var core=Core();while(true){bool next=false;try{next=core.MoveNext();}catch(Exception e){failure=e.ToString();}if(!next||failure!=null)break;yield return core.Current;}
        (core as IDisposable)?.Dispose();ReplicaFlow.Instance.Home();
        foreach(var p in probes)if(p)UnityEngine.Object.DestroyImmediate(p);probes.Clear();
        Time.timeScale=scale;Physics.autoSimulation=simulation;
        string report=failure==null?"PASS source selection physics: .4 camera-axis Transform lift / .2 Linear; source Discrete/None preserved; selected body remains registered and skips height optimization; pause stops lift; noncontact sleeping neighbour stays asleep on press/cancel/pick; real solid contact displaces another body without radius wake; cancel clears velocities; successful pickup preserves solid grace window and no stale lift; per-candidate shuffle depth1 with no global scan; ordinary reveal preserves full scale/pickability through shuffle; manual-only touch-height update preserves separate peak/visibility records.\n":"FAIL source selection physics: "+failure+"\n";
        Directory.CreateDirectory("Temp");File.WriteAllText("Temp/goose-source-selection-physics-frames-validation.txt",report);Debug.Log(report);
    }
    static Rigidbody Probe(string name,Vector3 position){
        var obj=GameObject.CreatePrimitive(PrimitiveType.Cube);obj.name=name;obj.tag="Collectable";obj.transform.position=position;obj.transform.localScale=Vector3.one*.2f;
        var body=obj.AddComponent<Rigidbody>();ReplicaSourcePhysics.Apply(body);ReplicaPilePhysics.Instance.Register(body);body.useGravity=false;
        probes.Add(obj);LevelGenerator.Instance.objectsOnScene.Add(obj);return body;
    }
    static IEnumerator Core(){
        Time.timeScale=1;Physics.autoSimulation=false;
        var flow=ReplicaFlow.Instance;flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
        foreach(var item in LevelGenerator.Instance.objectsOnScene)item.SetActive(false);
        var manager=ReplicaPilePhysics.Instance;manager.Clear();
        var input=InputController.Instance;var feedback=input.GetComponent<ReplicaSelectionFeedback>();
        var position=new Vector3(0,50,0);var direction=-Camera.main.transform.forward;
        var target=Probe("selection source target",position);var sleeper=Probe("sleeping noncontact neighbour",position+Vector3.right*.7f);
        Physics.SyncTransforms();sleeper.Sleep();int count=manager.BoardCount;
        feedback.Begin(target.gameObject,Camera.main);
        Check(target.isKinematic&&target.detectCollisions&&target.GetComponent<Collider>().enabled,"solid kinematic press");
        Check(target.interpolation==RigidbodyInterpolation.None&&target.collisionDetectionMode==CollisionDetectionMode.Discrete,"no invented collision/interpolation switches");
        Check(manager.Contains(target)&&manager.BoardCount==count,"selected body remains registered");
        manager.RefreshHeightStates();Check(target.isKinematic,"optimization skips selected body even in exposed band");
        DOTween.Goto(target.transform,.1f,false);Physics.SyncTransforms();
        Check(Vector3.Distance(target.transform.position,position+direction*.2f)<.0001f,"linear halfway displacement, not OutQuad");
        Check(sleeper.IsSleeping(),"press does not wake neighbouring noncontact body");
        feedback.Cancel();
        Check(!target.isKinematic&&target.velocity==Vector3.zero&&target.angularVelocity==Vector3.zero,"LostTarget restores dynamic and clears both velocities");
        Check(sleeper.IsSleeping()&&manager.BoardCount==count,"cancel has no radial wake or register churn");
        target.position=position;target.transform.position=position;feedback.Begin(target.gameObject,Camera.main);
        Time.timeScale=0;var paused=target.transform.position;yield return new WaitForSecondsRealtime(.08f);
        Check(Vector3.Distance(target.transform.position,paused)<.0001f,"source selection tween uses scaled time");
        feedback.Cancel();Time.timeScale=1;
        target.position=position;target.transform.position=position;
        var touching=Probe("contact neighbour",position+direction*.3f);Physics.SyncTransforms();touching.Sleep();sleeper.Sleep();
        var before=touching.position;feedback.Begin(target.gameObject,Camera.main);
        for(int i=1;i<=10;i++){DOTween.Goto(target.transform,i*.02f,false);Physics.SyncTransforms();Physics.Simulate(.02f);}
        Check(Vector3.Distance(touching.position,before)>.005f,"real contact moves neighbour without manual WakeUp");
        Check(sleeper.IsSleeping(),"noncontact neighbour remains sleeping through physical lift");
        input.AddObjectOnStack(target.gameObject);
        Check(!feedback.Selected&&target.isKinematic&&!manager.Contains(target),"successful pickup commits without cancel/re-register");
        Check(target.detectCollisions&&target.GetComponent<Collider>().enabled,"source pickup keeps initial collider grace");
        Check(target.interpolation==RigidbodyInterpolation.None&&target.collisionDetectionMode==CollisionDetectionMode.Discrete&&sleeper.IsSleeping(),"pickup does not change modes or wake noncontact neighbours");
        yield return new WaitForSeconds(.07f);
        Check(!target.detectCollisions&&!target.GetComponent<Collider>().enabled,"collider grace closes after source scaled wait");
        yield return new WaitForSeconds(.3f);
        Check(input.arrivedObject.Contains(target.gameObject)&&!input.IsBusy,"source flight completes after selected press");
        // Shuffle completion checks only its candidates at source depth=1, then normal optimization resumes at .5.
        flow.Home();flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
        var pile=ReplicaLayeredPile.Instance;var board=LevelGenerator.Instance.objectsOnScene;
        var candidate=board.First(o=>o.GetComponentInChildren<Renderer>().enabled).GetComponent<Rigidbody>();
        candidate.isKinematic=false;candidate.velocity=Vector3.zero;candidate.angularVelocity=Vector3.zero;
        candidate.position=new Vector3(0,(pile.Threshold+pile.PhysicsThreshold)*.5f,0);pile.RestoreToolScale(candidate);
        manager.RefreshHeightState(candidate);Check(candidate.isKinematic,"normal .5 depth freezes the mid-band fixture");
        var hidden=board.First(o=>!o.GetComponentInChildren<Renderer>().enabled);var hiddenBody=hidden.GetComponent<Rigidbody>();
        var hiddenPosition=hiddenBody.position;hiddenBody.position=new Vector3(0,50,0);
        float normalThreshold=pile.PhysicsThreshold;
        pile.FinishSourceShuffle(candidate);
        Check(!candidate.isKinematic&&Mathf.Approximately(normalThreshold,pile.PhysicsThreshold),"per-item shuffle completion uses depth1 without replacing normal threshold");
        Check(!hidden.GetComponentInChildren<Renderer>().enabled,"per-item completion does not reveal another item");
        hiddenBody.position=hiddenPosition;manager.RefreshHeightState(candidate);
        Check(candidate.isKinematic,"later normal optimization returns to depth .5");
        hiddenBody.position=new Vector3(0,pile.Threshold+.1f,0);pile.RefreshVisibility();
        Check(pile.CanPick(hiddenBody)&&hidden.transform.localScale.sqrMagnitude>.000001f,"ordinary reveal has no dormant growth or extra pick lock");
        var revealScale=hidden.transform.localScale;
        Check(ReplicaSourceTools.MessUp(),"shuffle starts after ordinary reveal");
        yield return new WaitForSeconds(.2f);
        Check(ReplicaSourceTools.Shuffling&&Vector3.Distance(hidden.transform.localScale,revealScale)<.000001f,"ordinary reveal remains full size while optimization is paused by shuffle");
        flow.Home();flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
        float touch=pile.SourceTouchHeight,peak=pile.SourceRecordedMaxHeight,threshold=pile.Threshold;
        var manual=board.First();manual.transform.position=new Vector3(0,touch-4,0);
        input.AddObjectOnStack(manual,true);
        Check(Mathf.Abs(pile.SourceTouchHeight-(touch-.5f))<.0001f,"manual low pickup lowers touch height by at most .5");
        Check(pile.SourceRecordedMaxHeight==peak&&pile.Threshold==threshold,"manual touch changes neither historical peak nor immediate visibility threshold");
        touch=pile.SourceTouchHeight;var automatic=board.First(x=>x&&x!=manual);automatic.transform.position=new Vector3(0,touch-4,0);
        input.AddObjectOnStack(automatic,false);
        Check(pile.SourceTouchHeight==touch,"auto-find pickup omits SetMinTouchHeight");
        pile.RecordSourceTouchHeight(touch-.1f);
        Check(Mathf.Abs(pile.SourceTouchHeight-(touch-.1f))<.0001f,"small touch-height reduction does not overshoot picked height");
    }
}
