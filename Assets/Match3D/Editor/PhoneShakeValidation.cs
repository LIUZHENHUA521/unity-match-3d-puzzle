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
    static void Near(Vector3 actual,Vector3 expected,string reason){Check(Vector3.Distance(actual,expected)<.001f,reason+": "+actual+" != "+expected);}
    public static void CheckDetector(){
        Near(PhoneShakeDetector.Force(Vector3.down,.02f,.02f,false),Vector3.zero,"gravity must not shake");
        Near(PhoneShakeDetector.Force(Vector3.right*2,.02f,.02f,false),Vector3.zero,"threshold boundary");
        Near(PhoneShakeDetector.Force(Vector3.right*3,.02f,.02f,false),Vector3.right*6.93f,"WX source force at 3g");
        Near(PhoneShakeDetector.Force(Vector3.right*3,.02f,.02f,true),Vector3.right*5.94f,"iOS source force at 3g");
        Near(PhoneShakeDetector.Force(new Vector3(0,0,3),.02f,.02f,false),Vector3.up*6.93f,"sensor Z maps to world Y");
        Near(PhoneShakeDetector.Force(new Vector3(0,-3,0),.02f,.02f,false),Vector3.back*6.93f,"sensor Y maps to signed world Z");
        Near(PhoneShakeDetector.Force(Vector3.left*20,.02f,.02f,false),Vector3.left*27.72f,"amplitude cap and sign");
        Near(PhoneShakeDetector.Force(Vector3.right*3,.1f,.02f,false),Vector3.right*21f,"callback time compensation");
        Near(PhoneShakeDetector.Force(Vector3.right*3,.1f,.04f,false),Vector3.right*10.5f,"fixed timestep compensation");
        Near(PhoneShakeDetector.Force(Vector3.right*3,10,.02f,false),Vector3.right*210f,"source interval upper clamp");
        Near(PhoneShakeDetector.Force(new Vector3(float.NaN,0,0),.02f,.02f,false),Vector3.zero,"invalid sample");
    }
    static IEnumerator Run(){
        string failure=null;var steps=Frames();
        while(true){bool more=false;try{more=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(!more||failure!=null)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();Time.timeScale=1;
        Match3DFlow.Instance.Home();
        Directory.CreateDirectory("Logs");
        File.WriteAllText("Logs/phone-shake-validation.txt",failure??"PASS source phone force: axes, threshold/cap, platform coefficients, callback/fixed timestep compensation. PASS actual callback/physics: continuous samples, reversal, mass response, no torque or parked-body activation; inventory/count, pause/background/resume/disable/modal/home/shuffle and tray/store protection.\n");
        Debug.Log(failure??"Phone shake validation PASS");
    }
    static IEnumerator Frames(){
        CheckDetector();
        var flow=Match3DFlow.Instance;flow.Begin(0);BoardCreationTestSupport.FinishPending(flow);
        GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;yield return null;
        var phone=flow.GetComponent<Match3DPhoneShake>();Check(phone,"phone listener missing");
        var physics=ItemPhysicsController.Instance;var board=LevelGenerator.Instance.objectsOnScene;
        var light=new GameObject("Shake force probe light").AddComponent<Rigidbody>();
        var heavy=new GameObject("Shake force probe heavy").AddComponent<Rigidbody>();
        var parked=new GameObject("Shake force probe parked").AddComponent<Rigidbody>();
        bool auto=Physics.autoSimulation;
        try{
            light.position=new Vector3(100,100,0);heavy.position=new Vector3(110,100,0);parked.position=new Vector3(120,100,0);
            foreach(var body in new[]{light,heavy,parked}){body.useGravity=false;body.drag=0;body.angularDrag=0;physics.Register(body);}
            light.mass=.05f;heavy.mass=.1f;parked.isKinematic=true;Physics.autoSimulation=false;
            int count=board.Count,used=BoardTools.UsedCount(2),inventory=BoardTools.Remaining(2);
            Check(phone.ProcessAcceleration(Vector3.down)==0,"resting gravity applied force");
            // Feed the actual callback entry repeatedly within one frame: no gesture or cooldown gate.
            Check(phone.ProcessAcceleration(Vector3.right*3)>0,"first sample rejected");
            Check(phone.ProcessAcceleration(Vector3.right*3)>0,"second sample rejected by cooldown");
            Physics.Simulate(.02f);
            Near(light.velocity,new Vector3(5.544f,0,0),"continuous Force must accumulate using mass");
            Near(heavy.velocity,new Vector3(2.772f,0,0),"mass response differs from VelocityChange");
            Near(light.angularVelocity,Vector3.zero,"phone added random torque");
            Check(parked.isKinematic,"phone activated deep parked body");
            Check(phone.ProcessAcceleration(Vector3.left*3)>0,"direction reversal rejected");Physics.Simulate(.02f);
            Near(light.velocity,new Vector3(2.772f,0,0),"reversal force overwrote rather than changed velocity");
            Check(board.Count==count&&BoardTools.UsedCount(2)==used&&BoardTools.Remaining(2)==inventory,"phone consumed items or tools");
            Time.timeScale=0;Check(phone.ProcessAcceleration(Vector3.right*3)==0,"pause allowed force");Time.timeScale=1;
            phone.SendMessage("OnApplicationPause",true);Check(phone.ProcessAcceleration(Vector3.right*3)==0,"background allowed force");
            phone.SendMessage("OnApplicationPause",false);yield return null;
            Check(phone.ProcessAcceleration(Vector3.right*3)>0,"resume failed");
            phone.enabled=false;Check(phone.ProcessAcceleration(Vector3.right*3)==0,"disabled listener applied force");phone.enabled=true;yield return null;
        }finally{
            foreach(var body in new[]{light,heavy,parked})if(body){physics.RemoveForPick(body);UnityEngine.Object.DestroyImmediate(body.gameObject);}
            Physics.autoSimulation=auto;Time.timeScale=1;
        }
        var item=board.First(o=>o&&o.activeInHierarchy);InputController.Instance.AddObjectOnStack(item);
        yield return new WaitForSecondsRealtime(.7f);
        var position=item.transform.position;phone.ProcessAcceleration(Vector3.right*3);yield return new WaitForFixedUpdate();
        Check(Vector3.Distance(position,item.transform.position)<.001f,"tray item moved");
        Check(BoardTools.RunPaid(0,()=>InputController.Instance.MoveSourceItemsToStore()),"store setup failed");
        yield return new WaitForSecondsRealtime(.5f);position=item.transform.position;phone.ProcessAcceleration(Vector3.right*3);yield return new WaitForFixedUpdate();
        Check(BoardTools.IsStored(item)&&Vector3.Distance(position,item.transform.position)<.001f,"stored item moved");
        flow.ShowSettings(true);Check(phone.ProcessAcceleration(Vector3.right*3)==0,"modal allowed force");
        flow.Home();Check(phone.ProcessAcceleration(Vector3.right*3)==0,"home allowed force");
        flow.Begin(0);BoardCreationTestSupport.FinishPending(flow);GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;yield return null;
        Check(BoardTools.RunPaid(2,()=>BoardTools.MessUp()),"shuffle setup failed");
        Check(phone.ProcessAcceleration(Vector3.right*3)==0,"shuffle allowed simultaneous force");
    }
}
