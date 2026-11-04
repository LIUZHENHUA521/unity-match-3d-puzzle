using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public static class FirstStageHudValidation {
    static int checks;
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);checks++;}
    public static void Start(){var driver=new GameObject("First stage HUD validation").AddComponent<BoardValidationDriver>();driver.StartCoroutine(Run(driver));}
    static IEnumerator Run(BoardValidationDriver driver){
        var flow=Match3DFlow.Instance;bool auto=Physics.autoSimulation;float scale=Time.timeScale;checks=0;string error=null;var core=Core();
        while(true){bool next=false;try{next=core.MoveNext();}catch(Exception e){error=e.ToString();}if(!next||error!=null)break;yield return core.Current;}
        (core as IDisposable)?.Dispose();flow.Home();Physics.autoSimulation=auto;Time.timeScale=scale;
        string report=(error==null?"PASS ":"FAIL ")+"first-stage HUD/tools: "+checks+" assertions. "+error;
        File.WriteAllText("Temp/goose-first-stage-hud-validation.txt",report);Debug.Log(report);UnityEngine.Object.Destroy(driver.gameObject);
    }
    static Match3DUIView HUD()=>UnityEngine.Object.FindObjectOfType<Match3DGameplayHUD>().GetComponent<Match3DUIView>();
    static IEnumerator Core(){
        Check(Match3DGameplayHUD.HeaderInset(720,393,852,new Rect(0,0,393,852))==0,"full screen has no artificial inset");
        float expected=59f*720/393;
        Check(Mathf.Abs(Match3DGameplayHUD.HeaderInset(720,393,852,new Rect(0,34,393,759))-expected)<.001f,"synthetic island safe top");
        Check(Mathf.Abs(Match3DGameplayHUD.HeaderInset(720,393,852,new Rect(0,0,393,852),59f/852)-expected)<.001f,"WeChat repairs full-screen Unity safe area");
        Check(Mathf.Abs(Match3DGameplayHUD.HeaderInset(720,1179,2556,new Rect(0,0,1179,2556),59f/852)-expected)<.001f,"logical/physical pixel ratios agree");
        Check(Mathf.Abs(Match3DGameplayHUD.HeaderInset(720,393,852,new Rect(0,34,393,759),59f/852)-expected)<.001f,"safe providers are not added twice");
        Check(Match3DGameplayHUD.HeaderInset(720,393,852,Rect.zero)==0,"missing Unity area does not push header off screen");
        var flow=Match3DFlow.Instance;Time.timeScale=1;Physics.autoSimulation=false;flow.Begin(0);
        BoardCreationTestSupport.FinishPending(flow);yield return null;yield return null;
        var hud=HUD();var skin=hud.GetComponent<Match3DGameplayHUD>();
        float inset=Match3DGameplayHUD.HeaderInset(((RectTransform)skin.transform).rect.width,Screen.width,Screen.height,Screen.safeArea);
        Check(Mathf.Abs(skin.header.anchoredPosition.y+inset)<.1f,"header follows safe area without an extra top offset");
        foreach(string key in new[]{"Pause","Timer"}){
            var corners=new Vector3[4];hud.Get<RectTransform>(key).GetWorldCorners(corners);
            float top=RectTransformUtility.WorldToScreenPoint(null,corners[1]).y;
            Check(top<Screen.safeArea.yMax,"top controls lie below device safe-area top: "+key);
        }
        Check(BoardTools.Enabled,"stage one tools enabled");
        foreach(string key in new[]{"Undo","Hint","Shuffle"})Check(hud.Get<Button>(key).gameObject.activeInHierarchy,"first stage tool visible "+key);
        Check(skin.quantities.All(q=>q.transform.parent.gameObject.activeInHierarchy&&q.text=="1"),"first-stage counts shown");
        var flags=BindingFlags.Instance|BindingFlags.NonPublic;int version=(int)typeof(Match3DFlow).GetField("creationVersion",flags).GetValue(flow);
        var inventory=typeof(Match3DFlow).GetMethod("CanApplyInventory",flags);
        Check((bool)inventory.Invoke(flow,new object[]{version}),"inventory callback allowed on current first-stage board");
        var input=InputController.Instance;var board=LevelGenerator.Instance.objectsOnScene;int count=board.Count;
        var item=board.First(o=>o.name!="type_0");input.AddObjectOnStack(item);yield return new WaitForSeconds(.4f);yield return null;
        Check(hud.Get<Button>("Undo").interactable,"remove becomes usable with a selected item");hud.Get<Button>("Undo").onClick.Invoke();
        yield return new WaitForSeconds(.5f);yield return null;
        Check(input.objectsOnStack.Count==0&&BoardTools.IsStored(item)&&board.Count==count,"first-stage remove stores without losing item");
        Check(BoardTools.UsedCount(0)==1&&skin.quantities[0].text=="0","remove spends and updates count");
        input.AddObjectOnStack(item);yield return new WaitForSeconds(.4f);yield return null;
        Check(hud.Get<Button>("Hint").interactable,"auto-find enabled with tray item");hud.Get<Button>("Hint").onClick.Invoke();
        yield return new WaitForSeconds(1f);yield return null;
        Check(BoardTools.UsedCount(1)==1&&board.Count==count-3&&flow.Stage==1,"first-stage auto-find performs a triple");
        Check(skin.quantities[1].text=="0","auto-find count updates");
        Check(hud.Get<Button>("Shuffle").interactable,"first-stage shuffle enabled");hud.Get<Button>("Shuffle").onClick.Invoke();
        Check(BoardTools.Shuffling,"shuffle follows source animation path");float deadline=Time.realtimeSinceStartup+15;
        while(BoardTools.Shuffling&&Time.realtimeSinceStartup<deadline)yield return null;
        yield return null;Check(!BoardTools.Shuffling&&BoardTools.UsedCount(2)==1&&board.Count==count-3,"shuffle completes with same board count");
        Check(skin.quantities[2].text=="0","shuffle count updates");
        Directory.CreateDirectory("Builds/HudFix");ScreenCapture.CaptureScreenshot("Builds/HudFix/first-stage-tools.png");yield return new WaitForEndOfFrame();
        flow.NextStage();Check(!(bool)inventory.Invoke(flow,new object[]{version}),"old paid callback rejected after transition");
        BoardCreationTestSupport.FinishPending(flow);yield return null;yield return null;
        Check(BoardTools.Enabled&&Enumerable.Range(0,3).All(i=>BoardTools.UsedCount(i)==0),"second stage gets normal fresh free allowances");
        Check(HUD().GetComponent<Match3DGameplayHUD>().quantities.All(q=>q.text=="1"),"second-stage badges reset");
    }
}
