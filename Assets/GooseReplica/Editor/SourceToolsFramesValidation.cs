using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public static class SourceToolsFramesValidation {
    static readonly List<GameObject> probes=new List<GameObject>();
    static void Check(bool ok,string reason){if(!ok)throw new Exception(reason);}
    public static IEnumerator Run(){
        float scale=Time.timeScale;bool simulation=Physics.autoSimulation;float aspect=Camera.main.aspect;
        string failure=null;var core=Core();
        while(true){bool next=false;try{next=core.MoveNext();}catch(Exception e){failure=e.ToString();}if(!next||failure!=null)break;yield return core.Current;}
        (core as IDisposable)?.Dispose();ReplicaFlow.Instance.Home();
        foreach(var p in probes)if(p){p.transform.DOKill();UnityEngine.Object.DestroyImmediate(p);}probes.Clear();
        Time.timeScale=scale;Physics.autoSimulation=simulation;Camera.main.aspect=aspect;
        string report=failure==null?"PASS source tools runtime: first-three store during flight; auto-find and shuffle during incoming flight without stranded moves; colliders and camera ray re-pick; return to tray with count conservation; each tool one-use gate; source auto-find fills and clears; shuffle input/countdown lock, scaled pause and completion; Home cancels old motion and resets per-round allowances.\n":"FAIL source tools runtime: "+failure+"\n";
        Directory.CreateDirectory("Temp");File.WriteAllText("Temp/goose-source-tools-frames-validation.txt",report);Debug.Log(report);
    }
    static void Setup(){
        var flow=ReplicaFlow.Instance;flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
        foreach(var obj in LevelGenerator.Instance.objectsOnScene)obj.SetActive(false);
        GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;
    }
    static GameObject Probe(string type,int index){
        var obj=GameObject.CreatePrimitive(PrimitiveType.Cube);obj.name=type;obj.tag="Collectable";
        obj.transform.position=new Vector3(index%3-1,18+index*.3f,index/3f);obj.transform.localScale=Vector3.one*.25f;
        obj.AddComponent<Rigidbody>().useGravity=false;LevelGenerator.Instance.objectsOnScene.Add(obj);probes.Add(obj);return obj;
    }
    static IEnumerator Core(){
        Time.timeScale=1;Physics.autoSimulation=false;Camera.main.aspect=9f/16;Setup();yield return null;
        var input=InputController.Instance;var board=LevelGenerator.Instance.objectsOnScene;
        var first=Probe("tool-A",0);var second=Probe("tool-B",1);var third=Probe("tool-C",2);var fourth=Probe("tool-D",3);
        foreach(var obj in new[]{first,second,third,fourth})input.AddObjectOnStack(obj);
        yield return new WaitForSeconds(.04f);
        Check(input.IsBusy,"move fixture still has incoming flights");
        var storeStart=first.transform.position;
        int count=board.Count;Check(input.MoveSourceItemsToStore(),"first source move succeeds");
        Check(input.objectsOnStack.SequenceEqual(new[]{fourth}),"first three slot objects removed, newest remains");
        Check(ReplicaSourceTools.StoredCount==3&&board.Count==count,"stored objects remain in level total");
        Check(!ReplicaSourceTools.CanUse(0)&&!input.MoveSourceItemsToStore(),"move uses only one allowance");
        // Probe the actual three movement tweens halfway through the source .3 s.
        // Independent analytic values: OutSine(.5), InBack(.5), OutBack(.5).
        var storeTarget=ReplicaSourceTools.StorePosition(0,0,1);
        first.transform.DOGoto(.15f,false);
        var expectedStore=new Vector3(Mathf.LerpUnclamped(storeStart.x,storeTarget.x,.70710678f),Mathf.LerpUnclamped(storeStart.y,storeTarget.y,-.0876975f),Mathf.LerpUnclamped(storeStart.z,storeTarget.z,1.0876975f));
        Check(Vector3.Distance(first.transform.position,expectedStore)<.001f,"store actual midflight OutSine/InBack/OutBack trajectory");
        first.transform.DOGoto(0,true);
        yield return new WaitForSeconds(.5f);Physics.SyncTransforms();
        Check(first.GetComponent<Collider>().enabled&&first.GetComponent<Rigidbody>().detectCollisions&&first.GetComponent<Rigidbody>().isKinematic,"stored body can raycast but stays kinematic");
        var screen=Camera.main.WorldToScreenPoint(first.transform.position);
        Check(input.PickAt(new Vector2(screen.x,screen.y))==first,"actual input ray can select stored item");
        input.AddObjectOnStack(first);Check(!ReplicaSourceTools.IsStored(first)&&ReplicaSourceTools.StoredCount==2,"re-pick removes stored reference");
        yield return new WaitForSeconds(.5f);
        Check(input.objectsOnStack.Count==2&&board.Count==count&&!input.IsBusy,"store-to-tray completes without losing objects or leaving busy state");
        ReplicaFlow.Instance.Home();Setup();yield return null;
        Check(ReplicaSourceTools.CanUse(0)&&ReplicaSourceTools.StoredCount==0,"new round restores allowance and empties store");
        var a=Probe("tool-match",4);var b=Probe("tool-match",5);var c=Probe("tool-match",6);
        input.AddObjectOnStack(a);yield return new WaitForSeconds(.04f);
        Check(input.IsBusy,"auto-find fixture still has incoming flight");
        int accepted=input.AcceptedPicks;BoosterManager.Instance.HintButton();
        Check(input.AcceptedPicks==accepted+2&&!ReplicaSourceTools.CanUse(1),"source auto-find picks missing two and consumes one allowance");
        yield return new WaitForSeconds(.9f);
        Check(!a&&!b&&!c&&input.objectsOnStack.Count==0,"auto-find goes through real triple removal");
        accepted=input.AcceptedPicks;BoosterManager.Instance.HintButton();Check(input.AcceptedPicks==accepted,"used auto-find cannot be invoked twice");
        ReplicaFlow.Instance.Home();Setup();yield return null;
        foreach(var old in board.ToArray())if(old)UnityEngine.Object.DestroyImmediate(old);board.Clear();
        var moving=Enumerable.Range(0,6).Select(i=>Probe("tool-shuffle-"+i,i)).ToArray();
        input.AddObjectOnStack(moving[0]);Check(input.IsBusy,"shuffle fixture has incoming flight");
        Check(ReplicaSourceTools.MessUp()&&ReplicaSourceTools.Shuffling&&!ReplicaSourceTools.CanUse(2),"shuffle starts and consumes allowance");
        float time=GameManager.Instance.currentLevelTime;accepted=input.AcceptedPicks;
        input.AddObjectOnStack(moving[0]);Check(input.AcceptedPicks==accepted,"shuffle rejects input");
        BoosterManager.Instance.HintButton();Check(ReplicaSourceTools.CanUse(1),"shuffle cannot consume another tool while input is locked");
        yield return new WaitForSeconds(.15f);
        Check(Mathf.Abs(GameManager.Instance.currentLevelTime-time)<.001f,"shuffle suspends countdown");
        Time.timeScale=0;yield return null;var positions=moving.Select(x=>x.transform.position).ToArray();
        yield return new WaitForSecondsRealtime(.1f);
        Check(ReplicaSourceTools.Shuffling&&moving.Select((x,i)=>i==0||Vector3.Distance(x.transform.position,positions[i])<.001f).All(x=>x),"shuffle rotation pauses with scaled time");
        Time.timeScale=1;float deadline=Time.realtimeSinceStartup+4;
        while(ReplicaSourceTools.Shuffling&&Time.realtimeSinceStartup<deadline)yield return null;
        Check(!ReplicaSourceTools.Shuffling&&board.Count==6&&moving.Skip(1).All(x=>x&&x.GetComponent<Collider>().enabled)&&input.objectsOnStack.SequenceEqual(new[]{moving[0]})&&!input.IsBusy,"shuffle completes preserving six objects and collisions");
        Check(!ReplicaSourceTools.MessUp(),"second shuffle denied this round");
        ReplicaFlow.Instance.Home();Setup();yield return null;
        Check(ReplicaSourceTools.CanUse(0)&&ReplicaSourceTools.CanUse(1)&&ReplicaSourceTools.CanUse(2),"all three allowances reset between rounds");
        Check(ReplicaSourceTools.MessUp(),"cancel fixture starts shuffle");
        yield return new WaitForSeconds(.1f);ReplicaFlow.Instance.Home();
        Check(!ReplicaSourceTools.Shuffling&&ReplicaSourceTools.StoredCount==0,"home cancels tool coroutine immediately");
        yield return new WaitForSecondsRealtime(.4f);
        Check(board.Count==0&&!GameManager.Instance.GameIsPlaying()&&!ReplicaSourceTools.Shuffling,"old shuffle cannot resurrect game after home");
    }
}
