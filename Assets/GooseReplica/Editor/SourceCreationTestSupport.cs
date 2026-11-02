using System;
using UnityEngine;

// Existing geometry/unit fixtures need a completed board, without running a second spawn implementation.
// This consumes the very same production iterator. Live frame/cancellation behavior has its own test.
public static class SourceCreationTestSupport {
 public static void AdvanceGrowth(float wait){
  Physics.SyncTransforms();
  for(float t=0;t<Mathf.Max(wait,.02f)-.00001f;t+=.02f){
   ReplicaLayeredPile.Instance.StepVisibility(.02f);
   Physics.SyncTransforms();
  }
 }
 public static void FinishPending(ReplicaFlow flow){
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
