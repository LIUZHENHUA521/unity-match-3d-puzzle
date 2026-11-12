using System;
using System.IO;
using UnityEngine;

// Existing geometry/unit fixtures need a completed board, without running a second spawn implementation.
// This consumes the very same production iterator. Live frame/cancellation behavior has its own test.
public static class BoardCreationTestSupport {
 // Shared snapshot retained for the later pickup regressions after reverting opening separation.
 [Serializable] sealed class Export {public Match3DDailySnapshot opening;}
    public static Match3DDailyLevel LegacyLevel()=>new Match3DDailyLevel{date="2026-11-13",tier="bachelor",candidateId="candidate-0123456789abcdef01234567",version="regression",generatorVersion="shared-v2",modelThemeId="foodie-party",seed=47231,seconds=600,initialCount=300,limitHeight=21.5f,layers=new[]{new ChallengeLayer(150,23,14),new ChallengeLayer(60,10,18),new ChallengeLayer(45,8,20),new ChallengeLayer(45,7,22)},opening=JsonUtility.FromJson<Export>(File.ReadAllText("Tools/validation/OpeningSeparation/legacy-opening.json")).opening};

 public static void AdvanceGrowth(float wait){
  Physics.SyncTransforms();
  for(float t=0;t<Mathf.Max(wait,.02f)-.00001f;t+=.02f){
   LayeredItemPile.Instance.StepVisibility(.02f);
   Physics.SyncTransforms();
  }
 }
 public static void FinishPending(Match3DFlow flow){
  if(!flow.CreationInProgress)return;
  var steps=flow.TakeOverSourceCreationForValidation();
  if(steps==null)throw new Exception("Creation iterator missing while creation is active");
  int guard=0;
  do{
   AdvanceGrowth((float)steps.Current);
   if(++guard>2000)throw new Exception("Creation iterator did not terminate");
  }while(steps.MoveNext());
  (steps as IDisposable)?.Dispose();
  if(flow.CreationInProgress)throw new Exception("Creation iterator ended without finalizing");
 }
}
