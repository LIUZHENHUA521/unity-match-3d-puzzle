using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class DailyWebPreviewBuild {
    public static void Run(){
        // Defer scene loading until startup import and resource cleanup finish.
        EditorApplication.delayCall+=Build;
    }
    static void Build(){
        try{
            if(!File.Exists(".goose-build-stage"))throw new Exception("Web preview builds require an isolated stage");
            if(Directory.Exists("Packages/com.qq.weixin.minigame"))throw new Exception("Use Tools/daily-preview/build.py to remove WeChat-only SDK hooks from this stage");
            HomeSkinAssets.Ensure();BoardPrefabPreparation.Bake();
            WechatReleaseAssets.Prepare(Match3DBuilder.Scene);
            PlayerSettings.productName="每日关卡试玩";
            PlayerSettings.WebGL.template="APPLICATION:Default";
            PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback=true;
            PlayerSettings.WebGL.dataCaching=false;
            PlayerSettings.WebGL.exceptionSupport=WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
            PlayerSettings.WebGL.linkerTarget=WebGLLinkerTarget.Wasm;
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL,ManagedStrippingLevel.Low);
            PlayerSettings.colorSpace=ColorSpace.Gamma;
            var output="Builds/DailyWebPreview";
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{Match3DBuilder.Scene},locationPathName=output,target=BuildTarget.WebGL,extraScriptingDefines=new[]{"DAILY_WEB_PREVIEW"},options=BuildOptions.None});
            if(report.summary.result!=BuildResult.Succeeded)throw new Exception("WebGL build failed: "+report.summary.result);
            File.Copy("Tools/daily-preview/index.html",output+"/index.html",true);
            File.Copy("Tools/daily-preview/bridge.js",output+"/bridge.js",true);
            var files=Directory.GetFiles(output+"/Build").Select(Path.GetFileName).ToArray();
            var config=new WebConfig{loader=files.Single(x=>x.EndsWith(".loader.js")),dataUrl="Build/"+files.Single(x=>x.Contains(".data")),frameworkUrl="Build/"+files.Single(x=>x.Contains(".framework.js")),codeUrl="Build/"+files.Single(x=>x.Contains(".wasm"))};
            File.WriteAllText(output+"/config.json",JsonUtility.ToJson(config));
            Debug.Log("DAILY_WEB_PREVIEW_BUILD_OK "+Path.GetFullPath(output));EditorApplication.Exit(0);
        }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
    }
    [Serializable] sealed class WebConfig{public string loader,dataUrl,frameworkUrl,codeUrl;public string companyName="Goose",productName="Daily preview",productVersion="1";}
}
