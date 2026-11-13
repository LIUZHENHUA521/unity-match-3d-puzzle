using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;
[InitializeOnLoad]
public static class PotComboPraiseValidation {
 const string Key="pot-praise-validation", Output="work/pot-praise-results";
 static readonly BindingFlags Flags=BindingFlags.Static|BindingFlags.NonPublic;
 static PotComboPraiseValidation(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))EditorApplication.delayCall+=()=>new GameObject("Pot validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());};}
 public static void Run(){Directory.CreateDirectory(Output);File.WriteAllText(Output+"/checks.txt","");PotComboPraiseAssetBuilder.Build();SessionState.SetBool(Key,true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
 static void Check(bool ok,string name){if(!ok)throw new Exception(name);File.AppendAllText(Output+"/checks.txt","PASS "+name+"\n");}
 static IEnumerator Guard(){
  var deadline=Time.realtimeSinceStartup+60;while((!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete)&&Time.realtimeSinceStartup<deadline)yield return null;
  var baseline=new RenderTexture(720,1565,24);baseline.Create();Camera.main.targetTexture=baseline;Camera.main.aspect=720f/1565;
  string error=null;var steps=Core();while(true){bool more=false;try{more=steps.MoveNext();}catch(Exception e){error=e.ToString();}if(!more||error!=null)break;yield return steps.Current;}
  Time.timeScale=1;File.AppendAllText(Output+"/checks.txt",error==null?"COMPLETE\n":"FAIL "+error+"\n");SessionState.SetBool(Key,false);EditorApplication.Exit(error==null?0:1);
 }
 static void Start(){typeof(ComboProbeValidation).GetMethod("Start",Flags).Invoke(null,new object[]{2});}
 static void Capture(string name,int width=720,int height=1565){
  Directory.CreateDirectory(ComboProbeValidation.Output);
  typeof(ComboProbeValidation).GetMethod("Capture",Flags).Invoke(null,new object[]{name,width,height});
  File.Copy(ComboProbeValidation.Output+"/"+name+".png",Output+"/"+name+".png",true);
 }
 static IEnumerator Core(){
  Start();yield return new WaitForSeconds(3);var owner=ComboProbeController.Instance;var p=owner.View.Praise;
  foreach(var group in new string[]{null,"A","unexpected"}){p.BeginStage(group);p.ShowCombo(2);Check(!p.Graphic,"A/unassigned never creates renderer: "+group);}
  p.BeginStage("B");p.ShowCombo(1);Check(!p.Graphic,"first match has no praise");
  var dummy=new GameObject("Accepted match fixture");
  owner.PickAccepted(dummy,ProbePickSource.Manual,true);Check(owner.State.Combo==1&&!p.Graphic,"production first triple silent");
  owner.PickAccepted(dummy,ProbePickSource.Manual,true);Check(owner.State.Combo==2&&p.Graphic.gameObject.activeSelf,"production second triple triggers praise");
  Check(!p.Graphic.raycastTarget,"praise does not intercept taps");
  yield return new WaitForSeconds(.25f);Capture("pot-combo-2");
  var entry=p.Graphic.AnimationState.GetCurrent(0);float before=entry.TrackTime;Time.timeScale=0;yield return new WaitForSecondsRealtime(.2f);Check(Mathf.Abs(entry.TrackTime-before)<.001f,"pause freezes authored animation");Time.timeScale=1;
  owner.PickAccepted(dummy,ProbePickSource.Automatic,true);Check(owner.State.Combo==2&&p.Graphic.AnimationState.GetCurrent(0)==entry,"automatic match neither increments combo nor restarts praise");
  owner.PickAccepted(dummy,ProbePickSource.Manual,true);Check(p.Graphic.AnimationState.GetCurrent(0).Animation.Name=="appear3","quick next combo replaces previous animation");
  yield return new WaitForSeconds(1.5f);Check(!p.Graphic.gameObject.activeSelf,"animation hides on completion");
  var random=UnityEngine.Random.state;for(int combo=2;combo<=11;combo++){
   p.ShowCombo(combo);yield return new WaitForSeconds(.22f);Check(p.Graphic.canvasRenderer.GetMesh().vertexCount>0,"authored mesh renders combo "+combo);Capture("pot-combo-"+combo);
  }
  // Compare RNG around a synchronous display call, excluding physics/gameplay frames.
  random=UnityEngine.Random.state;p.ShowCombo(9);Check(random.Equals(UnityEngine.Random.state),"cosmetics do not consume gameplay RNG");
  foreach(var size in new[]{new Vector2Int(720,1280),new Vector2Int(768,1024)}){p.ShowCombo(2);p.Graphic.Update(.22f);Capture("pot-layout-"+size.x+"x"+size.y,size.x,size.y);}
  for(int i=0;i<3;i++)owner.PickAccepted(dummy,ProbePickSource.Manual,false);Check(owner.State.Combo==0&&!p.Graphic.gameObject.activeSelf,"broken combo clears praise");
  p.ShowCombo(5);GameManager.Instance.gameStatus=GameManager.GameStatus.FINISH;yield return null;Check(!p.Graphic.gameObject.activeSelf,"completed stage clears praise");GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;
  p.ShowCombo(5);p.BeginStage("A");Check(!p.Treatment&&!p.Graphic.gameObject.activeSelf,"new control stage clears treatment visual");p.ShowCombo(5);Check(!p.Graphic.gameObject.activeSelf,"control never reactivates existing graphic");
  p.BeginStage("B");p.ShowCombo(5);Start();Check(!p.Treatment&&!p.Graphic.gameObject.activeSelf,"restart refreshes group and clears stale tracks");
  p.BeginStage("B");p.ShowCombo(5);Match3DFlow.Instance.Home();Check(!p.Graphic.gameObject.activeInHierarchy,"home clears visual");Object.Destroy(dummy);
 }
}
