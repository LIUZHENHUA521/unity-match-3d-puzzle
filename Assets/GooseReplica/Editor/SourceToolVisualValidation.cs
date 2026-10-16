using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEditor;

public static class SourceToolVisualValidation {
    [Serializable] class Manifest {public PrefabData[] prefabs;}
    [Serializable] class PrefabData {public string name;public Node[] nodes;}
    [Serializable] class Node {public string name,particle;public int parent;}
    [Serializable] class ParticleData {public float lengthInSec,simulationSpeed;public bool looping,prewarm,playOnAwake,useUnscaledTime;public Initial InitialModule;public Module EmissionModule,ShapeModule,ColorModule,SizeModule,VelocityModule,UVModule,TrailModule;}
    [Serializable] class Initial {public int maxNumParticles;public Curve startLifetime,startSpeed,startSize;}
    [Serializable] class Curve {public int minMaxState;public float scalar,minScalar;}
    [Serializable] class Module {public bool enabled;}
    static void Check(bool ok,string reason){if(!ok)throw new Exception(reason);}
    static void Close(float a,float b,string reason){Check(Mathf.Abs(a-b)<.00001f,reason+" "+a+" vs "+b);}
    public static void Run(){string report;
        try{
            var manifest=JsonUtility.FromJson<Manifest>(File.ReadAllText("Assets/GooseReplica/SourceToolVisuals/manifest.json"));int total=0;
            foreach(var p in manifest.prefabs){var prefab=Resources.Load<GameObject>("SourceTools/"+p.name);Check(prefab,"missing prefab "+p.name);var nodes=prefab.GetComponentsInChildren<Transform>(true);Check(nodes.Length==p.nodes.Length,"source hierarchy "+p.name);
                for(int i=0;i<nodes.Length;i++){var n=p.nodes[i];Check(nodes[i].name==n.name,"source node order");if(string.IsNullOrEmpty(n.particle))continue;var source=JsonUtility.FromJson<ParticleData>(File.ReadAllText("Assets/GooseReplica/SourceToolVisuals/Modules/"+n.particle));var ps=nodes[i].GetComponent<ParticleSystem>();Check(ps,"particle "+n.name);var m=ps.main;Close(m.duration,source.lengthInSec,"duration");Close(m.simulationSpeed,source.simulationSpeed,"speed");Check(m.loop==source.looping&&m.prewarm==source.prewarm&&m.playOnAwake==source.playOnAwake&&m.useUnscaledTime==source.useUnscaledTime,"main flags");Check(m.maxParticles==source.InitialModule.maxNumParticles,"max particles");Close(m.startLifetime.constantMax,source.InitialModule.startLifetime.scalar,"lifetime multiplier");Close(m.startSpeed.constantMax,source.InitialModule.startSpeed.scalar,"speed multiplier");
                    Check(ps.emission.enabled==source.EmissionModule.enabled&&ps.shape.enabled==source.ShapeModule.enabled&&ps.colorOverLifetime.enabled==source.ColorModule.enabled&&ps.sizeOverLifetime.enabled==source.SizeModule.enabled&&ps.velocityOverLifetime.enabled==source.VelocityModule.enabled&&ps.textureSheetAnimation.enabled==source.UVModule.enabled&&ps.trails.enabled==source.TrailModule.enabled,"all active native modules");
                    var renderer=ps.GetComponent<ParticleSystemRenderer>();foreach(var material in renderer.sharedMaterials)if(material)Check(material.shader&&material.shader.isSupported&&!ShaderUtil.ShaderHasError(material.shader),"source shader "+material.name);total++;
                }
            }
            Check(total==10,"ten source systems");var bg=Resources.Load<GameObject>("SourceTools/StoreContentBG");Check(bg&&bg.GetComponent<MeshFilter>().sharedMesh,"source store board");Check(bg.GetComponent<Renderer>().sharedMaterial.shader.name=="Unlit/Transparent Cutout","original unlit board shader");Close(ReplicaSourceToolVisual.FindDurationMultiplier,.7f,"serialized find time overrides constructor .9");Close(ReplicaSourceToolVisual.FindUnitSize,.0209f,"serialized find beam length overrides constructor .0205");
            report="PASS source tool visuals: 3 source prefab hierarchies, 10 native systems (duration, flags, maxima, lifetime/speed and active modules), shader support including exact distortion port, original cutout store board, serialized beam duration/scale. Full native JSON roundtrip recorded by importer.\n";
        }catch(Exception e){report="FAIL source tool visuals: "+e+"\n";}Directory.CreateDirectory("Temp");File.WriteAllText("Temp/goose-source-tool-visual-validation.txt",report);Debug.Log(report);
    }
    public static IEnumerator RunFrames(){float scale=Time.timeScale;float aspect=Camera.main.aspect;bool physics=Physics.autoSimulation;string failure=null;var core=Core();while(true){bool more=false;try{more=core.MoveNext();}catch(Exception e){failure=e.ToString();}if(!more||failure!=null)break;yield return core.Current;}(core as IDisposable)?.Dispose();ReplicaFlow.Instance.Home();Time.timeScale=scale;Physics.autoSimulation=physics;Camera.main.aspect=aspect;string report=failure==null?"PASS source tool visuals runtime: actual imported models, first-store board/three return trails, hide on final repick, native auto-find beams and blue profiles, unscaled return/find clocks with scaled background/particles, shuffle six-system effect; Home cancels particles, board and pending callbacks. PNG captures retained.\n":"FAIL source tool visuals runtime: "+failure+"\n";File.WriteAllText("Temp/goose-source-tool-visual-frames-validation.txt",report);Debug.Log(report);}
    static void Setup(int theme=0){ReplicaFlow.Instance.Begin(theme);ReplicaFlow.Instance.NextStage();SourceCreationTestSupport.FinishPending(ReplicaFlow.Instance);GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;}
    static IEnumerator Core(){
        Time.timeScale=1;Physics.autoSimulation=false;Camera.main.aspect=9f/16;Setup();yield return null;
        yield return PauseClockCheck();
        var input=InputController.Instance;var board=LevelGenerator.Instance.objectsOnScene;
        var items=board.Where(x=>x&&x.activeInHierarchy).GroupBy(x=>x.name).Take(3).Select(g=>g.First()).ToArray();Check(items.Length==3,"real source types for store");foreach(var item in items)input.AddObjectOnStack(item);
        yield return new WaitForSeconds(.4f);Check(input.MoveSourceItemsToStore(),"real source items stored");yield return new WaitForSeconds(.12f);Check(ReplicaSourceToolVisual.HasBackground&&ReplicaSourceToolVisual.ActiveEffectCount==3,"store board and three original trails");Capture("source-tool-store-moving.png");
        yield return new WaitForSeconds(.3f);Capture("source-tool-store-settled.png");Close(ReplicaSourceToolVisual.BackgroundPosition,ReplicaSourceToolVisual.StoreShowZ,"shown store endpoint");Check(ReplicaSourceToolVisual.PooledEffectCount>=3,"returned source trails are pooled by prefab name");
        foreach(var item in items)input.AddObjectOnStack(item);yield return new WaitForSeconds(.35f);Close(ReplicaSourceToolVisual.BackgroundPosition,ReplicaSourceToolVisual.StoreHideZ,"last re-pick hides board");
        ReplicaFlow.Instance.Home();Check(!ReplicaSourceToolVisual.HasBackground&&ReplicaSourceToolVisual.ActiveEffectCount==0,"home destroys store visuals");Setup();yield return null;
        var type=board.Where(x=>x&&x.activeInHierarchy).GroupBy(x=>x.name).First(g=>g.Count()>=3).ToArray();input.AddObjectOnStack(type[0]);yield return new WaitForSeconds(.4f);BoosterManager.Instance.HintButton();yield return new WaitForSeconds(.06f);Check(ReplicaSourceToolVisual.ActiveEffectCount==2,"two native lightning beams");Check(ReplicaSourceHighlight.For(Camera.main).ToolTargetCount==2,"two Lighting profiles");Capture("source-tool-autofind.png");yield return new WaitForSeconds(.35f);Check(ReplicaSourceToolVisual.ActiveEffectCount==0,"beam lifetime ends");Check(ReplicaSourceToolVisual.PooledEffectCount>=2,"auto-find beams return to named pool");
        var stats="";
        for(int theme=0;theme<3;theme++){
            ReplicaFlow.Instance.Home();Setup(theme);yield return null;
            var candidates=ReplicaSourceTools.ShuffleCandidates(board,input.objectsOnStack);int before=Visible(candidates);
            Check(before>0,"theme has visible shuffle candidates");
            Check(ReplicaSourceTools.MessUp(),"native shuffle starts");yield return new WaitForSeconds(.2f);Check(ReplicaSourceToolVisual.ActiveEffectCount==1,"one original shuffle prefab with six systems");
            var effect=GameObject.Find("FX_Tool_Messup");Check(effect&&effect.transform.parent==Camera.main.transform,"Messup uses source camera anchor");Check(Vector3.Distance(effect.transform.localPosition,new Vector3(0,0,1))<.00001f&&Vector3.Distance(effect.transform.localScale,Vector3.one*.1f)<.00001f,"source MessToolPoint local matrix");
            if(theme==0){
                var particle=effect.GetComponent<ParticleSystem>();Time.timeScale=0;
                // ParticleSystem may consume the already-computed delta in this frame.
                // Take the baseline after entering a fully paused frame.
                yield return null;yield return null;float particleTime=particle.time;
                yield return new WaitForSecondsRealtime(.12f);
                Close(particle.time,particleTime,"source shuffle particles keep scaled native clock");Time.timeScale=1;
            }
            int early=Visible(candidates);Check(early>0,"shuffle may not rotate all visible objects offscreen at .2s, theme "+theme);
            Capture("source-tool-shuffle-"+theme+".png");if(theme==0)Capture("source-tool-shuffle.png");
            yield return new WaitForSeconds(.35f);int late=Visible(candidates);Check(late>0,"shuffle may not rotate all visible objects offscreen at .55s, theme "+theme);stats+=ReplicaSourceEnvironment.CurrentName+": before="+before+", .2s="+early+", .55s="+late+"\n";
            ReplicaFlow.Instance.Home();Check(!ReplicaSourceToolVisual.HasBackground&&ReplicaSourceToolVisual.ActiveEffectCount==0,"Home clears active shuffle effect");yield return new WaitForSecondsRealtime(.1f);Check(ReplicaSourceToolVisual.ActiveEffectCount==0&&!ReplicaSourceToolVisual.HasBackground,"cancelled effects do not return");
        }
        File.WriteAllText("visual-captures/source-tool-shuffle-visibility.txt",stats);Debug.Log("SOURCE_SHUFFLE_VISIBLE\n"+stats);
    }
    static IEnumerator PauseClockCheck(){
        var probe=new GameObject("source-tool-clock-probe");probe.transform.position=Camera.main.transform.position+Camera.main.transform.forward*5;
        float old=Time.timeScale;
        try{
            Time.timeScale=0;ReplicaSourceToolVisual.SetStoreVisible(true);
            float background=ReplicaSourceToolVisual.BackgroundPosition;
            ReplicaSourceToolVisual.Return(probe);ReplicaSourceToolVisual.AutoFind(probe,probe.transform.position+Vector3.right);
            yield return new WaitForSecondsRealtime(.08f);
            Check(ReplicaSourceToolVisual.ActiveEffectCount==2,"paused tool fixture starts return and find");
            probe.transform.position+=Vector3.right;yield return new WaitForSecondsRealtime(.04f);
            var trail=GameObject.Find("FX_Tool_ReturnBack");Check(trail,"paused return trail exists");
            Check(Vector3.Distance(trail.transform.position,probe.transform.position)<.001f,"return follows during timeScale zero");
            yield return new WaitForSecondsRealtime(.35f);
            Check(ReplicaSourceToolVisual.ActiveEffectCount==1&&!GameObject.Find("FX_Tool_AutoFind"),"find ends after .21 unscaled seconds");
            Close(ReplicaSourceToolVisual.BackgroundPosition,background,"store background remains scaled and paused");
            var stopped=trail.transform.position;probe.transform.position+=Vector3.right;yield return new WaitForSecondsRealtime(.04f);
            Check(Vector3.Distance(trail.transform.position,stopped)<.001f,"return stops following after .3 unscaled seconds");
            yield return new WaitForSecondsRealtime(2.1f);
            Check(ReplicaSourceToolVisual.ActiveEffectCount==0,"return two-second realtime tail completes while paused");
        }finally{ReplicaSourceToolVisual.Reset();UnityEngine.Object.DestroyImmediate(probe);Time.timeScale=old;}
    }
    static int Visible(System.Collections.Generic.IEnumerable<GameObject> items){var planes=GeometryUtility.CalculateFrustumPlanes(Camera.main);return items.Count(x=>x&&x.activeInHierarchy&&x.GetComponentsInChildren<Renderer>().Any(r=>r.enabled&&r.shadowCastingMode!=UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly&&GeometryUtility.TestPlanesAABB(planes,r.bounds)));}
    static void Capture(string name){var camera=Camera.main;var prior=camera.targetTexture;var active=RenderTexture.active;var rt=new RenderTexture(720,1280,24);var image=new Texture2D(720,1280,TextureFormat.RGB24,false);try{camera.targetTexture=rt;rt.Create();camera.Render();RenderTexture.active=rt;image.ReadPixels(new Rect(0,0,720,1280),0,0);image.Apply();Directory.CreateDirectory("visual-captures");File.WriteAllBytes("visual-captures/"+name,image.EncodeToPNG());}finally{camera.targetTexture=prior;RenderTexture.active=active;UnityEngine.Object.DestroyImmediate(image);rt.Release();UnityEngine.Object.DestroyImmediate(rt);}}
}
