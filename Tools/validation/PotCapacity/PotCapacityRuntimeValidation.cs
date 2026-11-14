using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad] public static class PotCapacityRuntimeValidation {
    const string Key="pot-capacity-runtime";
    static readonly List<string> lines=new List<string>();static int checks;
    static PotCapacityRuntimeValidation(){EditorApplication.playModeStateChanged+=Changed;}
    public static void Run(){SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
    static void Changed(PlayModeStateChange state){if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))EditorApplication.delayCall+=()=>new GameObject("Pot capacity validation").AddComponent<BoardValidationDriver>().StartCoroutine(Execute());}
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);checks++;}
    static IEnumerator Execute(){
        Screen.SetResolution(720,1558,false);yield return null;yield return null;Physics.autoSimulation=false;InputController.Instance.enabled=false;
        float size=BoardLayoutSettings.ModelSizeMultiplier;string error=null;var core=Core();
        while(true){bool more=false;try{more=core.MoveNext();}catch(Exception e){error=e.ToString();Debug.LogException(e);}if(!more||error!=null)break;yield return core.Current;}
        (core as IDisposable)?.Dispose();Match3DFlow.Instance.Home();Physics.autoSimulation=true;BoardLayoutSettings.ModelSizeMultiplier=size;
        File.WriteAllText("../runtime-result.txt",(error==null?"PASS "+checks+" runtime checks":error)+"\n"+string.Join("\n",lines));SessionState.SetBool(Key,false);EditorApplication.Exit(error==null?0:1);
    }
    static void Simulate(int frames){Physics.SyncTransforms();for(int i=0;i<frames;i++){LayeredItemPile.Instance.StepVisibility(.02f);Physics.Simulate(.02f);}}
    static IEnumerator Core(){
        var flow=Match3DFlow.Instance;flow.Home();yield return null;
        var target=new RenderTexture(720,1558,24);Camera.main.targetTexture=target;
        var catalog=Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
        foreach(var theme in catalog.themes)foreach(int layerCount in new[]{4,5,6}){
            BoardLayoutSettings.ModelSizeMultiplier=1;
            var configured=new[]{new ChallengeLayer(150,23,14),new ChallengeLayer(60,10,18),new ChallengeLayer(45,8,20),new ChallengeLayer(90,7,22),new ChallengeLayer(60,10,26),new ChallengeLayer(60,10,30)}.Take(layerCount).ToArray();
            int count=configured.Sum(l=>l.count);
            var level=new Match3DDailyLevel{date="2026-11-14",tier="elite",version="pot-capacity-local",generatorVersion="daily-v1",modelThemeId=theme.id,seed=18471,seconds=600,initialCount=count,limitHeight=configured.Last().height-.5f,layers=configured};
            flow.PreviewDailyOpening(level,1);BoardCreationTestSupport.FinishPending(flow);Simulate(20);
            var first=flow.CaptureDailyOpening();Check(first.items.Length==15,"tutorial unchanged");
            flow.PreviewDailyOpening(level,2);BoardCreationTestSupport.FinishPending(flow);Simulate(250);yield return null;
            Check(SoftTransitionContainer.Height==(layerCount==4?26:configured.Last().height+2),"configured height changed");
            var board=LevelGenerator.Instance.objectsOnScene;Check(board.Count==count,"runtime count");Check(board.All(o=>SoftTransitionContainer.Contains(o.transform.position,-.1f)),"runtime escaped");
            var visible=board.Where(o=>o.GetComponentsInChildren<Renderer>().Any(r=>r.enabled&&r.gameObject.activeInHierarchy)).ToArray();Check(visible.Length>=15&&visible.Length<count,"visible band");
            var camera=Camera.main;Check(camera.transform.position.y>SoftTransitionContainer.Height,"runtime camera below lid");
            var state=flow.CaptureDailyOpening();Check(state.IsValid(count,theme.profile.prefabs.Length),"captured tall snapshot invalid");
            lines.Add(theme.id+" count="+count+" ceiling="+SoftTransitionContainer.Height+" visible="+visible.Length+" active="+ItemPhysicsController.Instance.DynamicCount);
            var picker=typeof(InputController).GetMethod("PickExact",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
            Check(visible.Any(o=>picker.Invoke(InputController.Instance,new object[]{camera,(Vector2)camera.WorldToScreenPoint(o.GetComponentsInChildren<Renderer>().First(r=>r.enabled).bounds.center)})!=null),"visible models cannot be picked");
            if(theme.id=="foodie-party"&&layerCount==5){
                camera.Render();var oldTarget=RenderTexture.active;RenderTexture.active=target;var image=new Texture2D(720,1558,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,720,1558),0,0);image.Apply();File.WriteAllBytes("../pot-corrected-game.png",image.EncodeToPNG());RenderTexture.active=oldTarget;UnityEngine.Object.Destroy(image);
            }
            level.generatorVersion="shared-v2";level.opening=new Match3DDailySnapshot{stages=new[]{first,state}};
            flow.PreviewDailyOpening(level,2);float deadline=Time.realtimeSinceStartup+30;while(flow.CreationInProgress&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(!flow.CreationInProgress,"snapshot restoration timeout");ItemPhysicsController.Instance.ReleaseDailyOpening();var restored=flow.CaptureDailyOpening();
            Check(restored.items.Length==state.items.Length&&restored.items.Zip(state.items,(a,b)=>a.id==b.id&&a.position==b.position&&a.scale==b.scale&&a.rotation==b.rotation&&a.visible==b.visible&&a.dynamic==b.dynamic).All(v=>v),"snapshot changed layout");
            // Remove one triple and verify the remaining pile stays playable.
            foreach(var obj in LevelGenerator.Instance.objectsOnScene.OrderByDescending(o=>o.transform.position.y).Take(3).ToArray()){
                LayeredItemPile.Instance.Detach(obj.GetComponent<Rigidbody>());ItemPhysicsController.Instance.RemoveForPick(obj.GetComponent<Rigidbody>());LevelGenerator.Instance.objectsOnScene.Remove(obj);UnityEngine.Object.Destroy(obj);
            }
            yield return null;Simulate(150);Check(LevelGenerator.Instance.objectsOnScene.Count(o=>LayeredItemPile.Instance.DailyVisible(o.GetComponent<Rigidbody>()))>=15,"reserve does not reveal");
            flow.Home();yield return null;
        }
    }
}
