using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class FirstStagePlacementValidation {
    const string Key="first-stage-placement-validation";
    const string Output="work/first-level-placement-audit";
    static FirstStagePlacementValidation(){EditorApplication.playModeStateChanged+=Changed;}
    public static void Run(){
        FirstStageValidation.ValidatePlans();
        SessionState.SetBool(Key,true);
        EditorSceneManager.OpenScene(Match3DBuilder.Scene);
        EditorApplication.isPlaying=true;
    }
    static void Changed(PlayModeStateChange state){
        if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))
            EditorApplication.delayCall+=()=>new GameObject("First stage placement validation").AddComponent<BoardValidationDriver>().StartCoroutine(RunChecks());
    }
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static IEnumerator RunChecks(){
        // EnteredPlayMode can precede the scene's Start methods.
        yield return null;yield return null;
        string failure=null;var steps=Core();
        while(true){bool next=false;try{next=steps.MoveNext();}catch(Exception e){failure=e.ToString();}if(failure!=null||!next)break;yield return steps.Current;}
        (steps as IDisposable)?.Dispose();Match3DFlow.Instance.Home();
        Directory.CreateDirectory(Output);
        string report=failure==null?"PASS: 1000 selection seeds; local and daily first-stage entry; 15 items / 6+6+3; curated shared models; deterministic daily replay; 1.5–6 spawn heights sorted bottom-up; gradual growth and physics release; all visible after settling; five triple clears enter stage 2.":"FAIL: "+failure;
        File.WriteAllText(Output+"/validation.txt",report);Debug.Log(report);
        SessionState.SetBool(Key,false);EditorApplication.Exit(failure==null?0:1);
    }
    static IEnumerator Core(){
        var flow=Match3DFlow.Instance;var board=LevelGenerator.Instance.objectsOnScene;
        var profile=LevelLayoutConfig.GetTutorial(flow.library.themes[0].sourceEnvironment);
        var daily=new Match3DDailyLevel{date="2026-11-04",tier="bachelor",version="placement-validation",generatorVersion="daily-v1",seed=47231,seconds=600,initialCount=300,limitHeight=21.5f,layers=LevelLayoutConfig.Get(flow.library.themes[0].sourceEnvironment).layers};
        var expected=LevelLayoutConfig.GenerateFirstStage(profile,daily.RandomForStage(1));
        var replay=LevelLayoutConfig.GenerateFirstStage(profile,daily.RandomForStage(1));
        Check(expected.Select(t=>t.type).SequenceEqual(replay.Select(t=>t.type)),"daily replay differs");
        // Observe sampled positions before any physical movement, using the production iterator.
        bool simulation=Physics.autoSimulation;Physics.autoSimulation=false;
        flow.BeginDailyPractice(0,daily);BoardCreationTestSupport.FinishPending(flow);
        Check(board.Count==15,"daily entry count");
        float previous=1.5f;
        foreach(var obj in board){
            float y=obj.transform.position.y;Check(y>=previous-.0001f&&y<=6f,"spawn heights/order");previous=y;
            int id=obj.GetComponent<Match3DTile>().id;
            Check(obj.name=="type_"+expected[id].type,"daily entry bypassed curated generator");
        }
        flow.Home();Physics.autoSimulation=simulation;yield return null;
        for(int mode=0;mode<2;mode++){
            if(mode==0)flow.Begin(0);else flow.BeginDailyPractice(0,daily);
            Check(board.Count==1&&flow.CreationInProgress,"first batch must contain one item");
            var shell=GameObject.Find(profile.environment+" first stage container");
            Check(shell.GetComponentsInChildren<Transform>().Any(t=>t.name=="Lv1Col"),"first-stage collision subtree");
            var walls=shell.GetComponentsInChildren<Transform>().Single(t=>t.name=="Lv1Col");
            Check(Vector3.Distance(walls.up,Vector3.up)<.00001f,"first-stage container axis must be perpendicular to the ground");
            for(float height=0;height<=30;height+=.5f)
                Check(Mathf.Abs(FirstStageContainer.GetBorder(height).position.z)<.00001f,"first-stage spawn region must not drift with height");
            Check(board[0].transform.localScale==Vector3.zero&&board[0].GetComponent<Rigidbody>().isKinematic,"initial zero scale and held body");
            bool sawGrowth=false;float deadline=Time.realtimeSinceStartup+30;
            while(flow.CreationInProgress&&Time.realtimeSinceStartup<deadline){
                foreach(var obj in board){
                    int type=int.Parse(obj.name.Substring(5));float full=profile.prefabs[type].transform.localScale.magnitude*BoardLayoutSettings.ModelScale(profile.prefabs[type]);
                    float size=obj.transform.localScale.magnitude;if(size>0&&size<full*.99f)sawGrowth=true;
                }
                yield return null;
            }
            Check(!flow.CreationInProgress&&GameManager.Instance.GameIsPlaying()&&sawGrowth,"progressive creation completes before play");
            Check(board.Count==15&&board.GroupBy(o=>o.name).Select(g=>g.Count()).OrderBy(n=>n).SequenceEqual(new[]{3,6,6}),"live 6+6+3");
            Check(board.All(o=>profile.tutorialNames.Contains(profile.prefabs[int.Parse(o.name.Substring(5))].name)),"live whitelist");
            yield return new WaitForSeconds(2);
            Capture(Output+(mode==0?"/first-stage-local.png":"/first-stage-daily.png"));
            File.WriteAllText(Output+"/settled-"+mode+".txt",string.Join("\n",board.Select(o=>o.name+" "+o.transform.position+" visible="+o.GetComponentsInChildren<Renderer>().Any(r=>r.enabled)+" kinematic="+o.GetComponent<Rigidbody>().isKinematic)));
            Check(board.All(o=>o.GetComponentsInChildren<Renderer>().Any(r=>r.enabled)&&o.transform.position.y> -2&&o.transform.position.y<6),"visible settled items inside height bounds");
            var triples=board.GroupBy(o=>o.name).SelectMany(g=>g.Select((o,i)=>new{o,i}).GroupBy(x=>x.i/3).Select(gr=>gr.Select(x=>x.o).ToArray())).ToArray();
            for(int i=0;i<triples.Length;i++){
                foreach(var obj in triples[i])InputController.Instance.AddObjectOnStack(obj);
                yield return new WaitForSeconds(.9f);
                Check(flow.Stage==(i==triples.Length-1?2:1),"clear / stage transition");
            }
            Check(GameObject.Find(profile.environment+" short transition container")==shell,"boundary ownership retained");
            Check(!shell.GetComponentsInChildren<Transform>().Any(t=>t.name=="Lv1Col")&&shell.GetComponentsInChildren<Collider>().Length>0,"first-stage walls removed at stage 2");
            deadline=Time.realtimeSinceStartup+30;
            while(flow.CreationInProgress&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(!flow.CreationInProgress&&board.Count==300&&GameManager.Instance.GameIsPlaying(),"second stage finishes normally");
            flow.Home();yield return null;
        }
    }
    static void Capture(string path){
        Directory.CreateDirectory(Output);var camera=Camera.main;var rt=new RenderTexture(540,960,24);
        var old=camera.targetTexture;var active=RenderTexture.active;camera.targetTexture=rt;rt.Create();
        Match3DFlow.Instance.GetComponent<Match3DThemePresentation>().Fit();camera.Render();RenderTexture.active=rt;
        var tex=new Texture2D(540,960,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,540,960),0,0);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());
        camera.targetTexture=old;RenderTexture.active=active;UnityEngine.Object.Destroy(tex);rt.Release();UnityEngine.Object.Destroy(rt);
    }
}
