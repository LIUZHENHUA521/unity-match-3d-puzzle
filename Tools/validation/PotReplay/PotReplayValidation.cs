using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public static class PotReplayValidation {
 public static void Run(){try{
  var data=new PotReplayData();
  for(int i=0;i<61;i++){bool full=i%10==0;var frame=new PotReplayFrame{t=i*.1f,full=full,tray=new int[0]};frame.items.Add(new PotReplayPose{id=0,p=new Vector3(i,0,0),r=Quaternion.identity,s=Vector3.one,visible=true});if(full)frame.items.Add(new PotReplayPose{id=1,p=Vector3.one,r=Quaternion.identity,s=Vector3.one,visible=false});data.frames.Add(frame);}
  foreach(int i in new[]{0,60,12,59,3,40,10}){var state=PotReplayState.At(data,i*.1f+.00001f,out int frame);if(frame!=i||state.Count!=2||state[0].p.x!=i||state[1].visible)throw new Exception("seek mismatch "+i);}
  var copy=JsonUtility.FromJson<PotReplayData>(JsonUtility.ToJson(data));if(!copy.Supported||copy.frames.Count!=61)throw new Exception("serialization mismatch");copy.resourceVersion="unknown";if(copy.Supported)throw new Exception("unknown resource accepted");
  Debug.Log("POT_REPLAY_VALIDATION_OK seek + serialization + version");EditorApplication.Exit(0);
 }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}}
}
