using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Match3DDefaultFontValidation {
    public static void CompileWebGL() {
        var settings=new UnityEditor.Build.Player.ScriptCompilationSettings {
            target=BuildTarget.WebGL, group=BuildTargetGroup.WebGL
        };
        var result=UnityEditor.Build.Player.PlayerBuildInterface.CompilePlayerScripts(settings,"Temp/DefaultFontWebGL");
        if(result.assemblies==null || result.assemblies.Count==0)throw new Exception("WebGL script compilation failed");
        Debug.Log("DEFAULT_FONT_WEBGL_COMPILE_OK");
    }
    public static void Run() {
        var initialize=Match3DDefaultFont.Initialize();
        while(initialize.MoveNext()) {}
        var font=Match3DDefaultFont.Current;
        int characters=0, labels=0;
        foreach(var path in Directory.GetFiles("Assets/Match3D/Scripts","*.cs")) {
            foreach(Match literal in Regex.Matches(File.ReadAllText(path),"\"[^\"\\r\\n]*\""))
                foreach(char ch in literal.Value)
                    if(ch>='\u4e00' && ch<='\u9fff') {
                        if(!font.HasCharacter(ch))throw new Exception(path+": missing glyph "+ch);
                        characters++;
                    }
        }
        foreach(var prefab in Resources.LoadAll<Match3DUIView>("Match3D/UI")) {
            var view=Match3DUIView.Load(prefab.name,null);
            try {
                foreach(var text in view.GetComponentsInChildren<Text>(true)) {
                    if(text.font!=font)throw new Exception(prefab.name+": wrong font");
                    labels++;
                }
            } finally { UnityEngine.Object.DestroyImmediate(view.gameObject); }
        }
        if(labels==0 || characters==0)throw new Exception("No UI or Chinese characters checked");
        Debug.Log("DEFAULT_FONT_VALIDATION_OK labels="+labels+" Chinese characters="+characters);
    }
}
