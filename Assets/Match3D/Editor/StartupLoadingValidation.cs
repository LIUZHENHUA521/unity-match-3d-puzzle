using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class StartupLoadingValidation {
    const string Output="docs/validation/startup-loading";
    static int checks;
    static StartupLoadingValidation(){EditorApplication.update+=Poll;}
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);checks++;}
    static void Poll(){
        if(File.Exists("Temp/check-startup-webgl")&&!EditorApplication.isCompiling&&!EditorApplication.isUpdating&&!EditorApplication.isPlayingOrWillChangePlaymode){
            File.Delete("Temp/check-startup-webgl");
            try{
                Directory.CreateDirectory("Temp/startup-webgl-scripts");
                var settings=new UnityEditor.Build.Player.ScriptCompilationSettings{target=BuildTarget.WebGL,group=BuildTargetGroup.WebGL};
                var result=UnityEditor.Build.Player.PlayerBuildInterface.CompilePlayerScripts(settings,"Temp/startup-webgl-scripts");
                if(result.assemblies==null||result.assemblies.Count==0)throw new Exception("No WebGL assemblies compiled");
                File.WriteAllText(Output+"/webgl-scripts.txt","PASS compiled WebGL player scripts, including SDK/login startup path ("+result.assemblies.Count+" assemblies). Not a device/network test.\n");
            }catch(Exception e){File.WriteAllText(Output+"/webgl-scripts.txt","FAIL "+e);Debug.LogException(e);}
            return;
        }
        if(!File.Exists("Temp/check-startup-loading-assets")||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
        File.Delete("Temp/check-startup-loading-assets");
        try{Validate();}catch(Exception e){File.WriteAllText(Output+"/assets.txt","FAIL "+e);Debug.LogException(e);}
    }
    static IEnumerator Failing(){yield return null;throw new InvalidOperationException("模拟网络失败");}
    static IEnumerator Parent(){yield return Failing();}
    static IEnumerator Succeeding(){yield return null;}
    public static void Validate(){
        Directory.CreateDirectory(Output);checks=0;
        var prefab=Resources.Load<GameObject>("Match3D/UI/StartupLoading");Check(prefab,"loading prefab exists");
        var instance=UnityEngine.Object.Instantiate(prefab);
        try{
            var view=instance.GetComponent<Match3DStartupView>();
            var advice=instance.GetComponentsInChildren<Text>().Single(t=>t.name=="HealthAdvice");
            Check(advice.text==Match3DStartupView.HealthAdvice,"all source advice lines preserved");
            foreach(var c in advice.text.Where(c=>c!='\n').Distinct())Check(advice.font.HasCharacter(c),"missing packaged font glyph: "+c);
            var badge=instance.GetComponentsInChildren<Image>().Single(i=>i.name=="Age8");
            Check(AssetDatabase.GetAssetPath(badge.sprite).EndsWith("Startup/Age8.png"),"original copied age badge is bound");
            Check(Mathf.Abs(badge.rectTransform.rect.width/badge.rectTransform.rect.height-199f/266)<.005f,"badge aspect preserved");
            view.SetProgress(.65f,"正在同步账号数据…");view.SetProgress(.2f,"正在重试…");
            Check(Mathf.Approximately(view.progress.fillAmount,.65f),"progress never moves backwards on retry");
            int clicks=0;view.ShowError("网络连接失败，请重试",()=>clicks++);view.ShowError("网络连接失败，请重试",()=>clicks++);
            Check(view.retry.gameObject.activeSelf&&!view.progress.transform.parent.gameObject.activeSelf,"error displays retry instead of spinning progress");
            view.retry.onClick.Invoke();Check(clicks==1&&!view.retry.gameObject.activeSelf,"retry callback is not duplicated");
            view.SetProgress(.9f,"正在准备主页…");Check(view.progress.transform.parent.gameObject.activeSelf,"progress restored after retry");
            Exception failure=null;var runner=Match3DStartupRunner.Run(Parent(),error=>failure=error);
            Check(runner.MoveNext()&&failure==null,"nested initialization can suspend");
            Check(!runner.MoveNext()&&failure!=null&&failure.Message=="模拟网络失败","nested initialization error surfaces to retry UI");
            failure=null;runner=Match3DStartupRunner.Run(Succeeding(),error=>failure=error);
            Check(runner.MoveNext()&&!runner.MoveNext()&&failure==null,"new attempt completes normally");
            foreach(var size in new[]{new Vector2Int(720,1280),new Vector2Int(720,1560),new Vector2Int(750,1334)})
                Match3DStartupBuilder.Render(prefab,Output+"/loading-"+size.x+"x"+size.y+".png",size.x,size.y,false);
            Check(File.Exists(Match3DStartupBuilder.CoverPath),"pre-engine WeChat cover exists");
            var exporter=File.ReadAllText("Assets/Match3D/Editor/WechatReleaseBuild.cs");
            Check(exporter.Contains("config.ProjectConf.bgImageSrc=WechatReleaseAssets.StartupCoverPath"),"release exporter uses the matching cover");
            Check(exporter.Contains("config.ProjectConf.HideAfterCallMain=false"),"cover remains through engine handoff");
            File.WriteAllText(Output+"/assets.txt","PASS "+checks+" assertions: source badge/advice, font glyphs, retry/error handling, monotonic progress, release cover binding; rendered 720x1280, 720x1560 and 750x1334.\n");
        }finally{UnityEngine.Object.DestroyImmediate(instance);}
    }
}
