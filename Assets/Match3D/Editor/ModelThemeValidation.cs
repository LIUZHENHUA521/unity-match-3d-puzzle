using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public static class ModelThemeValidation {
    public static void Start(){new GameObject("Model theme validation").AddComponent<BoardValidationDriver>().StartCoroutine(Run());}
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static IEnumerator Run(){
        var input=InputController.Instance;bool enabled=input.enabled;input.enabled=false;
        string editorTheme=UnityEditor.EditorPrefs.GetString(Match3DFlow.EditorThemePreference,"");bool forced=Match3DTestPrivileges.ForceForValidation;
        var steps=Core();string error=null;
        while(true){bool more=false;try{more=steps.MoveNext();}catch(Exception e){error=e.ToString();}if(!more||error!=null)break;yield return steps.Current;}
        UnityEditor.EditorPrefs.SetString(Match3DFlow.EditorThemePreference,editorTheme);Match3DTestPrivileges.ForceForValidation=forced;input.enabled=enabled;Match3DFlow.Instance.Home();
        Directory.CreateDirectory("docs/validation/model-themes");File.WriteAllText("docs/validation/model-themes/result.txt",error??"PASS: all three model themes; tutorial and 300-item challenge use selected pool; triple removal; deterministic retry; unknown theme rejected; legacy fallback retained.");
        UnityEngine.Object.Destroy(GameObject.Find("Model theme validation"));
    }
    static IEnumerator Ready(){float until=Time.realtimeSinceStartup+60;while(Match3DFlow.Instance.CreationInProgress&&Time.realtimeSinceStartup<until)yield return null;Check(!Match3DFlow.Instance.CreationInProgress,"creation timeout");}
    static IEnumerator Core(){
        float until=Time.realtimeSinceStartup+60;while(!Match3DInvitations.Instance&&Time.realtimeSinceStartup<until)yield return null;
        var flow=Match3DFlow.Instance;var input=InputController.Instance;
        Check(!Match3DModelThemes.TryGet("unknown","CountrysideStove",out _),"unknown theme must fail");
        Check(Match3DModelThemes.Get(null,"CountrysideStove")==LevelLayoutConfig.Get("CountrysideStove"),"legacy fallback");
        foreach(var id in new[]{"playful-toys","tasty-kitchen","foodie-party"}){
            Check(Match3DModelThemes.Title(id,"missing")!="missing",id+" title");
            var profile=Match3DModelThemes.Get(id,"CountrysideStove");var names=profile.prefabs.Select(p=>p.name).ToArray();
            Check(names.Length==23&&names.Distinct().Count()==23,"23 unique models");
            var level=new Match3DDailyLevel{date="2026-11-09",tier="bachelor",sourceDate="2026-11-09",version="theme-validation",generatorVersion="daily-v1",modelThemeId=id,seed=15937,seconds=600,initialCount=300,limitHeight=21.5f,layers=new[]{new ChallengeLayer(150,23,14),new ChallengeLayer(60,10,18),new ChallengeLayer(45,8,20),new ChallengeLayer(45,7,22)}};
            Check(level.IsValid(23),"valid daily config");
            UnityEditor.EditorPrefs.SetString(Match3DFlow.EditorThemePreference,id);
            flow.BeginReward(0,"bachelor");yield return Ready();
            Check(LevelGenerator.Instance.objectsOnScene.All(o=>names.Contains(o.GetComponent<BoardItemIdentity>().LogicalId)),id+" editor preference");flow.Home();
            Match3DTestPrivileges.ForceForValidation=true;
            flow.ChooseTestTheme(0,level);
            var picker=GameObject.Find("Test theme selection");Check(picker,"test picker visible");
            var buttons=picker.GetComponentsInChildren<UnityEngine.UI.Button>();Check(buttons.Length==5,"all themes, default and cancel available");
            buttons.Single(b=>b.name.Contains(Match3DModelThemes.Title(id,"missing"))).onClick.Invoke();yield return Ready();
            var expectedAmbient=BoardLighting.ItemAmbientColor.r*(id=="playful-toys"?.65f:id=="tasty-kitchen"?.75f:1f);
            Check(Mathf.Abs(RenderSettings.ambientSkyColor.r-expectedAmbient)<.001f,id+" theme ambient");
            Check(Mathf.Abs(RenderSettings.sun.intensity-BoardLighting.ItemLightIntensity*(id=="playful-toys"?.8f:id=="tasty-kitchen"?.85f:1f))<.001f,id+" theme light");
            var board=LevelGenerator.Instance.objectsOnScene;
            Check(flow.Stage==1&&board.Count==15&&board.All(o=>names.Contains(o.GetComponent<BoardItemIdentity>().LogicalId)),id+" tutorial pool");
            var triple=board.GroupBy(o=>o.name).First().Take(3).ToArray();foreach(var item in triple)input.AddObjectOnStack(item);yield return new WaitForSeconds(1);
            Check(input.objectsOnStack.Count==0&&board.Count==12,id+" triple");
            flow.NextStage();yield return Ready();Check(board.Count==300&&board.All(o=>names.Contains(o.GetComponent<BoardItemIdentity>().LogicalId)),id+" challenge pool");
            var order=board.Select(o=>o.GetComponent<BoardItemIdentity>().LogicalId).ToArray();
            yield return new WaitForSeconds(2);Directory.CreateDirectory("work/model-themes");ScreenCapture.CaptureScreenshot(Path.GetFullPath("work/model-themes/"+id+".png"));yield return null;
            flow.RestartStage();yield return Ready();Check(order.SequenceEqual(board.Select(o=>o.GetComponent<BoardItemIdentity>().LogicalId)),id+" deterministic retry");flow.Home();
        }
    }
}
