using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using WeChatWASM;

public static class WechatReleaseBuild {
    [Serializable] class ReleaseConfig { public string appid, projectName, scene, output, pvpEndpoint; }

    public static void Run() {
        try {
            HomeSkinAssets.Ensure();
            var release=JsonUtility.FromJson<ReleaseConfig>(File.ReadAllText("Tools/wechat/release.config.json"));
            if(!File.Exists(release.scene))throw new Exception("Release scene missing: "+release.scene);
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(release.scene,true)};
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;
            PlayerSettings.productName=release.projectName;
            PlayerSettings.WebGL.exceptionSupport=WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL,ManagedStrippingLevel.Low);
            var library=AssetDatabase.LoadAssetAtPath<ReplicaTheme>("Assets/GooseReplica/Resources/Replica/ThemeLibrary.asset");
            library.pvpEndpoint=release.pvpEndpoint;
            library.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/GooseReplica/Resources/Replica/Fonts/GooseSans.otf");
            if(!library.font)throw new Exception("Chinese font missing");
            EditorUtility.SetDirty(library);
            var config=WXConvertCore.config;
            config.ProjectConf.Appid=release.appid;
            config.ProjectConf.projectName=release.projectName;
            config.ProjectConf.relativeDST=Path.GetDirectoryName(release.output).Replace('\\','/');
            config.ProjectConf.DST=Path.GetFullPath(config.ProjectConf.relativeDST);
            config.ProjectConf.Orientation=(WXScreenOritation)0;
            config.ProjectConf.assetLoadType=1;
            config.ProjectConf.compressDataPackage=true;
            config.ProjectConf.MemorySize=256;
            config.ProjectConf.IOSDevicePixelRatio=2;
            config.CompileOptions.DevelopBuild=false;
            config.CompileOptions.Il2CppOptimizeSize=true;
            config.CompileOptions.Webgl2=true;
            config.CompileOptions.fbslim=false;
            config.CompileOptions.CustomNodePath="/opt/homebrew/bin/node";
            config.CompileOptions.enableIOSPerformancePlus=false;
            config.CompileOptions.showMonitorSuggestModal=false;
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            var result=WXConvertCore.DoExport();
            if(result!=WXConvertCore.WXExportError.SUCCEED)throw new Exception("WeChat export failed: "+result);
            if(config.ProjectConf.assetLoadType!=1)throw new Exception("Resources exceed the package limit. Use Tools/wechat/build.py to omit unused reference sprites.");
            var output=Path.GetFullPath(release.output);
            if(!File.Exists(Path.Combine(output,"game.js")))throw new Exception("Missing exported game.js");
            if(!Directory.Exists(Path.Combine(output,"data-package")) || Directory.GetFiles(Path.Combine(output,"data-package")).Length==0)
                throw new Exception("Missing packaged game resources");
            Debug.Log("GOOSE_WECHAT_EXPORT_OK "+output);
            if(Application.isBatchMode)EditorApplication.Exit(0);
        } catch(Exception e) {
            Debug.LogException(e);
            if(Application.isBatchMode)EditorApplication.Exit(1);
            else throw;
        }
    }
}
