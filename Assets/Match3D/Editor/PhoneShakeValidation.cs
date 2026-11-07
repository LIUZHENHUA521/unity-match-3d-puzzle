using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PhoneShakeValidation {
    static PhoneShakeValidation(){EditorApplication.update+=Poll;}
    static void Poll(){
        if(!File.Exists("Temp/run-phone-shake-validation")||!EditorApplication.isPlaying||!Match3DFlow.Instance||Match3DFlow.Instance.ScreenName!="home")return;
        File.Delete("Temp/run-phone-shake-validation");
        new GameObject("Phone shake validation").AddComponent<BoardValidationDriver>().StartCoroutine(Run());
    }
    static void Check(bool value,string reason){if(!value)throw new Exception(reason);}
    public static void CheckDetector(){
        var detector=new PhoneShakeDetector();
        for(int i=0;i<150;i++)Check(!detector.Sample(new Vector3(.1f*Mathf.Sin(i),-1,0),i*.02f),"stationary noise triggered");
        detector.Reset();
        for(int i=0;i<100;i++){float angle=i*Mathf.PI/100;Check(!detector.Sample(new Vector3(Mathf.Sin(angle),-Mathf.Cos(angle),0),4+i*.02f),"rotation triggered");}
        detector.Reset();Check(!detector.Sample(Vector3.down,7),"initial sample triggered");
        Check(!detector.Sample(new Vector3(2,-1,0),7.02f),"single bump triggered");
        Check(!detector.Sample(Vector3.down,7.04f),"bump return triggered");
        Check(detector.Sample(new Vector3(-2,-1,0),7.10f),"deliberate reversal missed");
        for(int i=1;i<95;i++)Check(!detector.Sample(new Vector3(i%2==0?2:-2,-1,0),7.1f+i*.02f),"cooldown failed");
        detector.Reset();Check(!detector.Sample(Vector3.down,9.2f),"resume first sample triggered");
        Check(!detector.Sample(new Vector3(2,-1,0),9.22f),"resume first impulse triggered");
        Check(detector.Sample(new Vector3(-2,-1,0),9.30f),"shake after cooldown missed");
        detector.Reset();Check(!detector.Sample(new Vector3(float.NaN,0,0),12),"invalid reading triggered");
        Check(!detector.Sample(Vector3.down,12),"initial gravity triggered");
        Check(!detector.Sample(new Vector3(2,-1,0),12.02f),"first impulse triggered");
        Check(!detector.Sample(new Vector3(-2,-1,0),13),"stale pre-background impulse triggered");
    }
    static IEnumerator Run(){
        string failure=null;var steps=Frames();
        while(true){bool more=false;try{more=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(!more||failure!=null)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();Time.timeScale=1;
        Match3DFlow.Instance.Home();
        Directory.CreateDirectory("Logs");
        File.WriteAllText("Logs/phone-shake-validation.txt",failure??"PASS detector: noise, rotation, bump, reversal, cooldown, resume and invalid readings. PASS gameplay: physical lift, count and tool inventory conservation, pause/home/shuffle gates, cooldown, tray/store protection.\n");
        Debug.Log(failure??"Phone shake validation PASS");
    }
    static IEnumerator Frames(){
        CheckDetector();
        var flow=Match3DFlow.Instance;flow.Begin(0);BoardCreationTestSupport.FinishPending(flow);
        GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;yield return null;
        var shake=UnityEngine.Object.FindObjectOfType<FoodPileShake>();
        Check(flow.GetComponent<Match3DPhoneShake>(),"phone listener missing");
        var physics=ItemPhysicsController.Instance;var board=LevelGenerator.Instance.objectsOnScene;
        var bodies=board.Where(o=>o&&o.activeInHierarchy).Select(o=>o.GetComponent<Rigidbody>()).Where(b=>b&&LayeredItemPile.Instance.NeedsPhysics(b)).ToArray();
        Check(bodies.Length>0,"no physical candidates");
        var velocities=bodies.Select(b=>b.velocity.y).ToArray();int count=board.Count,used=BoardTools.UsedCount(2),inventory=BoardTools.Remaining(2);
        Check(shake.TryShake(),"game shake rejected");
        yield return new WaitForFixedUpdate();
        Check(bodies.Select((b,i)=>b.velocity.y>velocities[i]+.5f).Any(v=>v),"no physical upward movement");
        Check(board.Count==count&&BoardTools.UsedCount(2)==used&&BoardTools.Remaining(2)==inventory,"shake consumed items or tools");
        Check(!shake.TryShake(),"gameplay cooldown bypassed");
        Time.timeScale=0;Check(!shake.CanShakeGameplay&&!shake.TryShake(),"pause allowed shake");Time.timeScale=1;
        yield return new WaitForSecondsRealtime(2.05f);
        var item=bodies[0].gameObject;InputController.Instance.AddObjectOnStack(item);
        yield return new WaitForSecondsRealtime(.7f);
        var position=item.transform.position;physics.ShakeAll();yield return new WaitForFixedUpdate();
        Check(Vector3.Distance(position,item.transform.position)<.001f,"tray item moved");
        Check(BoardTools.RunPaid(0,()=>InputController.Instance.MoveSourceItemsToStore()),"store setup failed");
        yield return new WaitForSecondsRealtime(.5f);position=item.transform.position;physics.ShakeAll();yield return new WaitForFixedUpdate();
        Check(BoardTools.IsStored(item)&&Vector3.Distance(position,item.transform.position)<.001f,"stored item moved");
        Check(BoardTools.RunPaid(2,()=>BoardTools.MessUp()),"shuffle setup failed");
        Check(!shake.CanShakeGameplay&&!shake.TryShake(),"shuffle allowed simultaneous shake");
        flow.Home();Check(!shake.CanShakeGameplay&&!shake.TryShake(),"home allowed shake");
    }
}
