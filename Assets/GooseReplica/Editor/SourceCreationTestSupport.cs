using System;
using UnityEngine;

// Existing geometry/unit fixtures need a completed board, without running a second spawn implementation.
// This consumes the very same production iterator. Live frame/cancellation behavior has its own test.
public static class SourceCreationTestSupport {
 public static void FinishPending(ReplicaFlow flow){
  if(!flow.CreationInProgress)return;
  var steps=flow.TakeOverSourceCreationForValidation();
  if(steps==null)throw new Exception("Creation iterator missing while creation is active");
  int guard=0;
  while(steps.MoveNext())if(++guard>1000)throw new Exception("Creation iterator did not terminate");
  (steps as IDisposable)?.Dispose();
  if(flow.CreationInProgress)throw new Exception("Creation iterator ended without finalizing");
 }
}
