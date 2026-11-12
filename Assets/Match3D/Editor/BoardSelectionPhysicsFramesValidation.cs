using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public static class BoardSelectionPhysicsFramesValidation {
    static readonly List<GameObject> probes=new List<GameObject>();
    static void Check(bool ok,string reason){if(!ok)throw new Exception(reason);}
    public static IEnumerator Run(){
        float scale=Time.timeScale;bool simulation=Physics.autoSimulation;string failure=null;
        var core=Core();while(true){bool next=false;try{next=core.MoveNext();}catch(Exception e){failure=e.ToString();}if(!next||failure!=null)break;yield return core.Current;}
        (core as IDisposable)?.Dispose();Match3DFlow.Instance.Home();
        foreach(var p in probes)if(p)UnityEngine.Object.DestroyImmediate(p);probes.Clear();
        Time.timeScale=scale;Physics.autoSimulation=simulation;
        string report=failure==null?"PASS selection physics: stationary highlight-only press/cancel; opening remains held until confirmed pickup; dynamic neighbour unchanged during press and flight; immediate collision isolation; visible contact solving and shuffle/reveal/height regressions.\n":"FAIL source selection physics: "+failure+"\n";
        Directory.CreateDirectory("Temp");File.WriteAllText("Temp/goose-source-selection-physics-frames-validation.txt",report);Debug.Log(report);
    }
    static Rigidbody Probe(string name,Vector3 position){
        var obj=GameObject.CreatePrimitive(PrimitiveType.Cube);obj.name=name;obj.tag="Collectable";obj.transform.position=position;obj.transform.localScale=Vector3.one*.2f;
        var body=obj.AddComponent<Rigidbody>();BoardPhysicsSettings.Apply(body);ItemPhysicsController.Instance.Register(body);body.useGravity=false;
        probes.Add(obj);LevelGenerator.Instance.objectsOnScene.Add(obj);return body;
    }
    static Match3DSelectionFeedback inputFeedback()=>InputController.Instance.GetComponent<Match3DSelectionFeedback>();
    static IEnumerator Setup(){
        var flow=Match3DFlow.Instance;flow.PreviewDailyOpening(OpeningSeparationValidation.LegacyLevel(),2);
        float deadline=Time.realtimeSinceStartup+120;
        while(flow.CreationInProgress){if(Time.realtimeSinceStartup>deadline)throw new Exception("selection fixture opening timeout");yield return null;}
        Check(string.IsNullOrEmpty(flow.CreationFailure),"selection fixture opens");
    }
    static IEnumerator Core(){
        Time.timeScale=1;Physics.autoSimulation=false;
        var flow=Match3DFlow.Instance;yield return Setup();
        var held=LevelGenerator.Instance.objectsOnScene.First(o=>LayeredItemPile.Instance.CanPick(o.GetComponent<Rigidbody>()));
        var heldPose=held.transform.position;var press=inputFeedback();
        press.Begin(held,Camera.main);yield return new WaitForSeconds(.08f);
        Check(ItemPhysicsController.Instance.DailyOpeningHeld&&held.transform.position==heldPose,"press preserves the opening hold and pose");
        press.Cancel();Check(ItemPhysicsController.Instance.DailyOpeningHeld,"cancel preserves the opening hold");
        foreach(var item in LevelGenerator.Instance.objectsOnScene)item.SetActive(false);
        var manager=ItemPhysicsController.Instance;manager.Clear();
        var input=InputController.Instance;var feedback=input.GetComponent<Match3DSelectionFeedback>();
        var position=new Vector3(0,50,0);var direction=-Camera.main.transform.forward;
        var target=Probe("selection source target",position);var sleeper=Probe("sleeping noncontact neighbour",position+Vector3.right*.7f);
        Physics.SyncTransforms();sleeper.Sleep();int count=manager.BoardCount;
        feedback.Begin(target.gameObject,Camera.main);
        Check(!target.isKinematic&&target.detectCollisions&&target.GetComponent<Collider>().enabled,"press preserves dynamic body and colliders");
        Check(target.interpolation==RigidbodyInterpolation.None&&target.collisionDetectionMode==CollisionDetectionMode.Discrete,"no invented collision/interpolation switches");
        Check(manager.Contains(target)&&manager.BoardCount==count,"selected body remains registered");
        manager.RefreshHeightStates();Check(!target.isKinematic,"press preserves normal height eligibility");
        DOTween.Goto(target.transform,.1f,false);Physics.SyncTransforms();
        Check(Vector3.Distance(target.transform.position,position)<.0001f,"press does not move the collision body");
        Check(sleeper.IsSleeping(),"press does not wake neighbouring noncontact body");
        feedback.Cancel();
        Check(!target.isKinematic&&target.velocity==Vector3.zero&&target.angularVelocity==Vector3.zero,"cancel preserves original physics");
        Check(sleeper.IsSleeping()&&manager.BoardCount==count,"cancel has no radial wake or register churn");
        target.position=position;target.transform.position=position;feedback.Begin(target.gameObject,Camera.main);
        Time.timeScale=0;var paused=target.transform.position;yield return new WaitForSecondsRealtime(.08f);
        Check(Vector3.Distance(target.transform.position,paused)<.0001f,"source selection tween uses scaled time");
        feedback.Cancel();Time.timeScale=1;
        target.position=position;target.transform.position=position;
        var touching=Probe("contact neighbour",position+direction*.3f);Physics.SyncTransforms();touching.Sleep();sleeper.Sleep();
        var before=touching.position;feedback.Begin(target.gameObject,Camera.main);
        for(int i=1;i<=10;i++){DOTween.Goto(target.transform,i*.02f,false);Physics.SyncTransforms();Physics.Simulate(.02f);}
        Check(Vector3.Distance(touching.position,before)<.00001f,"press cannot push the contact-path neighbour");
        Check(sleeper.IsSleeping(),"noncontact neighbour remains sleeping through physical lift");
        input.AddObjectOnStack(target.gameObject);
        Check(!feedback.Selected&&target.isKinematic&&!manager.Contains(target),"successful pickup commits without cancel/re-register");
        Check(!target.detectCollisions&&!target.GetComponent<Collider>().enabled,"manual pickup disables collision before its first animation frame");
        for(int i=1;i<=3;i++){DOTween.Goto(target.transform,i*.02f,true);Physics.SyncTransforms();Physics.Simulate(.02f);}
        Check(Vector3.Distance(touching.position,before)<.00001f&&touching.velocity.sqrMagnitude<.00000001f,"manual pickup flight cannot launch the dynamic neighbour");
        Check(target.interpolation==RigidbodyInterpolation.None&&target.collisionDetectionMode==CollisionDetectionMode.Discrete&&sleeper.IsSleeping(),"pickup does not change modes or wake noncontact neighbours");
        yield return new WaitForSeconds(.07f);
        Check(!target.detectCollisions&&!target.GetComponent<Collider>().enabled,"pickup remains isolated throughout flight");
        yield return new WaitForSeconds(.3f);
        Check(input.arrivedObject.Contains(target.gameObject)&&!input.IsBusy,"source flight completes after selected press");
        // Visible items must remain dynamic to resolve contact; hidden support stays parked.
        flow.Home();yield return Setup();
        var pile=LayeredItemPile.Instance;var board=LevelGenerator.Instance.objectsOnScene;
        Physics.SyncTransforms();manager.ReleaseDailyOpening();
        Check(board.Select(o=>o.GetComponent<Rigidbody>()).Where(b=>!b.isKinematic).All(b=>b.velocity==Vector3.zero&&b.angularVelocity==Vector3.zero),"opening release clears stale velocity");
        pile.RefreshVisibility();
        var candidate=board.First(o=>o.GetComponentInChildren<Renderer>().enabled).GetComponent<Rigidbody>();
        manager.RefreshHeightState(candidate);Check(!candidate.isKinematic,"visible item remains eligible for contact solving");
        var hidden=board.First(o=>!o.GetComponentInChildren<Renderer>().enabled);var hiddenBody=hidden.GetComponent<Rigidbody>();
        float normalThreshold=pile.PhysicsThreshold;
        pile.FinishSourceShuffle(candidate);
        Check(!candidate.isKinematic&&Mathf.Approximately(normalThreshold,pile.PhysicsThreshold),"shuffle completion preserves normal threshold");
        Check(!hidden.GetComponentInChildren<Renderer>().enabled,"per-item completion does not reveal another item");
        hiddenBody.position=new Vector3(0,pile.Threshold+.1f,0);pile.RefreshVisibility();
        Check(pile.CanPick(hiddenBody)&&hidden.transform.localScale.sqrMagnitude>.000001f,"ordinary reveal has no dormant growth or extra pick lock");
        var revealScale=hidden.transform.localScale;
        Check(BoardTools.RunPaid(2,()=>BoardTools.MessUp()),"shuffle starts after ordinary reveal");
        yield return new WaitForSeconds(.2f);
        Check(BoardTools.Shuffling&&Vector3.Distance(hidden.transform.localScale,revealScale)<.000001f,"ordinary reveal remains full size while optimization is paused by shuffle");
        flow.Home();yield return Setup();
        float touch=pile.SourceTouchHeight,peak=pile.SourceRecordedMaxHeight,threshold=pile.Threshold;
        var manual=board.First();manual.transform.position=new Vector3(0,touch-4,0);
        input.AddObjectOnStack(manual,true);
        Check(Mathf.Abs(pile.SourceTouchHeight-(touch-.5f))<.0001f,"manual low pickup lowers touch height by at most .5");
        Check(pile.SourceRecordedMaxHeight==peak&&pile.Threshold==threshold,"manual touch changes neither historical peak nor immediate visibility threshold");
        touch=pile.SourceTouchHeight;var automatic=board.First(x=>x&&x!=manual);automatic.transform.position=new Vector3(0,touch-4,0);
        input.AddObjectOnStack(automatic,false);
        Check(pile.SourceTouchHeight==touch,"auto-find pickup omits SetMinTouchHeight");
        pile.RecordSelectionHeight(touch-.1f);
        Check(Mathf.Abs(pile.SourceTouchHeight-(touch-.1f))<.0001f,"small touch-height reduction does not overshoot picked height");
    }
}
