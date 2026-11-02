using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public static class TrayDisplayValidation {
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Start(){new GameObject("Tray display validation").AddComponent<SourceValidationDriver>().StartCoroutine(Run());}
    static IEnumerator Run(){
        float time=Time.timeScale;bool auto=Physics.autoSimulation;string error=null;var checks=Core();
        while(true){bool next=false;try{next=checks.MoveNext();}catch(Exception e){error=e.ToString();}if(!next||error!=null)break;yield return checks.Current;}
        (checks as IDisposable)?.Dispose();ReplicaFlow.Instance.Home();Time.timeScale=time;Physics.autoSimulation=auto;
        string report=error==null?"PASS tray display: settings save/reload/reset; adapter ID survives prefab replacement; same-name originals isolated; all 23 countryside production pickups match configured position/rotation/scale; reflow, store/reselect and undo preserve size and board pose; merge and pool reuse clear old binding; unconfigured flight unchanged.\n":"FAIL tray display: "+error;
        File.WriteAllText("Temp/goose-tray-display-validation.txt",report);Debug.Log(report);
        var driver=GameObject.Find("Tray display validation");if(driver)UnityEngine.Object.Destroy(driver);
    }
    static GameObject Spawn(GameObject prefab,int type){
        var flow=ReplicaFlow.Instance;
        var method=typeof(ReplicaFlow).GetMethod("CreateBoardItem",BindingFlags.Instance|BindingFlags.NonPublic);
        method.Invoke(flow,new object[]{new ChallengeTile{id=9000+LevelGenerator.Instance.objectsOnScene.Count,type=type},prefab,new Vector3(0,25,0),Quaternion.Euler(32,41,18),true,false});
        return LevelGenerator.Instance.objectsOnScene.Last();
    }
    static Vector3 Target(GameObject obj,int slot){
        var b=ReplicaTrayDisplayBinding.For(obj);var tray=ReplicaTray.Instance;float spacing=Vector3.Distance(tray.SlotPosition(0),tray.SlotPosition(1));
        return ReplicaTrayDisplaySettings.Position(b?b.pose:null,tray.SlotPosition(slot),Camera.main.transform.rotation,spacing);
    }
    static IEnumerator Settle(GameObject obj){
        float deadline=Time.realtimeSinceStartup+3;
        do{yield return null;}while(obj&&DOTween.IsTweening(obj.transform)&&Time.realtimeSinceStartup<deadline);
        Check(obj&&!DOTween.IsTweening(obj.transform),"flight settles");
    }
    static void CheckPose(GameObject obj,int slot){
        var b=ReplicaTrayDisplayBinding.For(obj);Check(b,"configured binding exists");
        Check(Vector3.Distance(obj.transform.position,Target(obj,slot))<.002f,"configured offset target "+obj.name+" actual="+obj.transform.position+" expected="+Target(obj,slot)+" body="+obj.GetComponent<Rigidbody>().position+" kinematic="+obj.GetComponent<Rigidbody>().isKinematic+" active="+obj.activeInHierarchy);
        Check(Quaternion.Angle(obj.transform.rotation,ReplicaTrayDisplaySettings.Rotation(b.pose,Camera.main.transform.rotation))<.05f,"configured rotation "+obj.name);
        Check(Vector3.Distance(obj.transform.localScale,ReplicaSourceFlight.PickScale(obj,b.boardScale)*b.pose.scale)<.002f,"configured scale "+obj.name);
    }
    static void Persistence(GameObject adapter,GameObject original){
        const string path="Assets/GooseReplica/Editor/TrayDisplayValidation-temp.asset";
        Check(!File.Exists(path),"validation temporary asset is free");
        var settings=ScriptableObject.CreateInstance<ReplicaTrayDisplaySettings>();GameObject replacement=null;
        try{
            var p=new ReplicaTrayDisplaySettings.Pose{rotation=new Vector3(-15,180,8),scale=1.2f,offset=new Vector3(.1f,.2f,.05f)};
            settings.Set(adapter,p);settings.Set(original,new ReplicaTrayDisplaySettings.Pose{scale=.75f});
            replacement=new GameObject(adapter.name);replacement.AddComponent<ReplicaSourceIdentity>().Configure(ReplicaTrayDisplaySettings.AdapterId(adapter),"replacement",1);
            Check(settings.TryGet(replacement,out var found)&&found.scale==1.2f,"adapter replacement resolves stable ID");
            Check(settings.TryGet(original,out found)&&found.scale==.75f,"same-name original has independent pose");
            AssetDatabase.CreateAsset(settings,path);AssetDatabase.SaveAssets();AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceUpdate);
            var copy=AssetDatabase.LoadAssetAtPath<ReplicaTrayDisplaySettings>(path);
            Check(copy.TryGet(adapter,out found)&&found.offset==p.offset&&found.rotation==p.rotation,"serialized fields round-trip");
            copy.Remove(adapter);Check(!copy.TryGet(adapter,out _)&&copy.TryGet(original,out _),"reset removes only selected model");
            Check(!new ReplicaTrayDisplaySettings.Pose{scale=float.NaN}.IsValid,"reject invalid input");
        }finally{if(replacement)UnityEngine.Object.DestroyImmediate(replacement);AssetDatabase.DeleteAsset(path);if(settings&&!AssetDatabase.Contains(settings))UnityEngine.Object.DestroyImmediate(settings);}
    }
    static IEnumerator Core(){
        var flow=ReplicaFlow.Instance;flow.Home();Time.timeScale=1;Physics.autoSimulation=false;
        var country=ReplicaSourceLevelPlan.Get("CountrysideStove").prefabs;
        var adapter=country.First(p=>p.name=="Banana");var original=ReplicaSourceLevelPlan.Get("GroceryStore").prefabs.First(p=>p.name=="Banana");Persistence(adapter,original);
        flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);foreach(var item in LevelGenerator.Instance.objectsOnScene)item.SetActive(false);
        Canvas.ForceUpdateCanvases();yield return null;yield return null;
        var input=InputController.Instance;
        for(int i=0;i<country.Length;i++){
            var obj=Spawn(country[i],1000+i);
            Check(Vector3.Distance(country[i].transform.localScale*ReplicaSourceProfile.ModelScale(country[i]),obj.transform.localScale)<.001f&&Quaternion.Angle(Quaternion.Euler(32,41,18),obj.transform.rotation)<.001f,"binding does not modify board pose");
            input.AddObjectOnStack(obj,false);Check(input.objectsOnStack.Contains(obj),"production pickup accepted");yield return Settle(obj);CheckPose(obj,0);
            input.ResetStack();LevelGenerator.Instance.objectsOnScene.Remove(obj);ReplicaSourceObjectPool.Recycle(obj);
        }
        var a=Spawn(adapter,2000);var b=Spawn(country.First(p=>p.name=="Bottle"),2001);
        var binding=ReplicaTrayDisplayBinding.For(a);binding.pose.offset=new Vector3(.12f,.08f,.06f);
        Vector3 boardScale=a.transform.localScale;Quaternion boardRotation=a.transform.rotation;
        input.AddObjectOnStack(a,false);yield return Settle(a);CheckPose(a,0);
        input.AddObjectOnStack(b,false);yield return Settle(b);CheckPose(a,0);CheckPose(b,1);
        Check(input.MoveSourceItemsToStore(1),"configured object can move to temporary store");yield return new WaitForSeconds(.45f);
        Check(ReplicaSourceTools.IsStored(a),"object stored");CheckPose(b,0);
        input.AddObjectOnStack(a,false);yield return Settle(a);CheckPose(a,1);
        Check(binding.boardScale==boardScale,"store/reselect cannot compound scale");
        // Exercise undo and reselect with actual physics. Source Transform tweens must
        // retain non-interpolated rigidbodies rather than reusing stale physics poses.
        Physics.autoSimulation=true;input.UndoLast();yield return new WaitForSeconds(.45f);
        Check(a.GetComponent<Rigidbody>().interpolation==RigidbodyInterpolation.None,"source undo retains non-interpolated body");
        Check(Vector3.Distance(a.transform.localScale,boardScale)<.001f&&Quaternion.Angle(a.transform.rotation,boardRotation)<.05f,"undo restores pre-pick board pose");
        input.AddObjectOnStack(a,false);Check(input.objectsOnStack.Contains(a),"undo reselect accepted: "+GameManager.Instance.gameStatus+" tag="+a.tag);yield return Settle(a);CheckPose(a,1);
        var a2=Spawn(adapter,2000);var a3=Spawn(adapter,2000);input.AddObjectOnStack(a2,false);input.AddObjectOnStack(a3,false);
        yield return new WaitForSeconds(1.1f);Check(input.objectsOnStack.Count==1&&!input.HasPendingVisuals,"configured triple merges normally");CheckPose(b,0);
        var reused=Spawn(adapter,2000);Check(reused==a||reused==a2||reused==a3,"pool actually reuses a merged instance");
        Check(ReplicaTrayDisplayBinding.For(reused).pose.offset==Vector3.zero,"pool rebind drops per-instance draft offset");
        var plain=Spawn(original,2003);Check(!ReplicaTrayDisplayBinding.For(plain),"same-name unconfigured original has no override");
        var expectedScale=ReplicaSourceFlight.PickScale(plain,plain.transform.localScale);input.AddObjectOnStack(plain,false);yield return Settle(plain);
        Check(Vector3.Distance(plain.transform.position,ReplicaTray.Instance.SlotPosition(1))<.002f,"unconfigured position unchanged");
        Check(Quaternion.Angle(plain.transform.rotation,Camera.main.transform.rotation*Quaternion.Euler(-90,0,0))<.05f,"unconfigured rotation unchanged");
        Check(Vector3.Distance(plain.transform.localScale,expectedScale)<.002f,"unconfigured scale unchanged");
        flow.Home();
    }
}
