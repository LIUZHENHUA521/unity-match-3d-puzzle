using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Disposable-project test: no scene startup, network requests or PlayerPrefs writes.
public static class PotRecordCompletionValidation {
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static int checks;
    static void Set(object target,string name,object value) => target.GetType().GetField(name,Private).SetValue(target,value);
    static object Get(object target,string name) => target.GetType().GetField(name,Private).GetValue(target);
    static void Check(bool value,string message) { if(!value)throw new Exception(message);checks++;Debug.Log("PASS "+message); }
    public static void Run() {
        try {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            var flow=new GameObject("Pot flow fixture").AddComponent<Match3DFlow>();
            typeof(Match3DFlow).GetField("<Instance>k__BackingField",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,flow);
            var input=new GameObject("Pot input fixture").AddComponent<InputController>();
            var records=new GameObject("Pot records fixture").AddComponent<Match3DGameRecords>();
            var combining=(HashSet<GameObject>)Get(input,"sourceCombining");
            var flying=(HashSet<GameObject>)Get(input,"sourceFlying");
            for(int stage=1;stage<=2;stage++) {
                Set(flow,"<Stage>k__BackingField",stage);
                Set(records,"active",new Match3DGameRecords.Record{initialCount=15});
                Set(records,"started",true);Set(records,"cleared",9);
                var first=new GameObject("Pot first centre");var last=new GameObject("Pot last centre");
                combining.Add(first);combining.Add(last);Set(input,"sourceCompletionCheckReady",false);
                Check(!input.CanEvaluateWin,"stage "+stage+": empty logical board waits for both matches");
                // Earlier completion callback opens the shared flag with only 12/15 counted.
                records.Matched();Set(input,"sourceCompletionCheckReady",true);combining.Remove(first);
                Check((int)Get(records,"cleared")==12&&!input.CanEvaluateWin,"stage "+stage+": first completion cannot settle incomplete record");
                records.Matched();
                Check(!input.CanEvaluateWin,"stage "+stage+": final removal delay remains gated");
                combining.Remove(last);
                Check(input.CanEvaluateWin&&(int)Get(records,"cleared")==15,"stage "+stage+": all callbacks finish before full-count victory");
                flying.Add(last);Check(!input.CanEvaluateWin,"stage "+stage+": pending flight blocks stale completion flag");
                flying.Clear();Set(input,"sourceCompletionCheckReady",false);
                Check(!input.CanEvaluateWin,"stage "+stage+": flight alone cannot replace a match checkpoint");
                UnityEngine.Object.DestroyImmediate(first);UnityEngine.Object.DestroyImmediate(last);
            }
            Set(flow,"<IsPvp>k__BackingField",true);
            Check(input.CanEvaluateWin,"PVP retains server-controlled completion behavior");
            File.WriteAllText("../result.txt","PASS "+checks+" completion regression assertions; full game compiled.\n");
            EditorApplication.Exit(0);
        } catch(Exception e) {File.WriteAllText("../result.txt","FAIL "+e);Debug.LogException(e);EditorApplication.Exit(1);}
    }
}
