using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable] public sealed class PotReplayData {
    public const string Version="pot-replay-v2",Resources="pot-replay-resources-2";
    public string version=Version,resourceVersion=Resources,recordId,runId,status;
    public Match3DDailyStage opening;
    public int stage;public float duration;public Match3DDailyLevel level;
    public List<PotReplayFrame> frames=new List<PotReplayFrame>();
    public List<PotReplayEvent> events=new List<PotReplayEvent>();
    public bool Supported=>version==Version&&resourceVersion==Resources||version=="pot-replay-v1"&&resourceVersion=="pot-replay-resources-1";
}
[Serializable] public sealed class PotReplayPose {
    public bool[] render;public int id,zone;public Vector3 p,s;public Quaternion r;public bool visible;
    public bool Same(PotReplayPose other)=>other!=null&&zone==other.zone&&visible==other.visible&&(render??new bool[0]).SequenceEqual(other.render??new bool[0])&&(p-other.p).sqrMagnitude<.000001f&&(s-other.s).sqrMagnitude<.000001f&&Quaternion.Angle(r,other.r)<.1f;
}
[Serializable] public sealed class PotReplayFrame {
    public float t,remaining,fov,cameraSize;public bool full,orthographic;public Vector3 cameraPosition;public Quaternion cameraRotation;
    public PotReplayPresentation presentation;public PotReplayEvent[] events;
    public int[] tray;public List<PotReplayPose> items=new List<PotReplayPose>();
}
[Serializable] public sealed class PotReplayEvent {public int seq,id=-1,value;public float t;public string type;}
public static class PotReplayState {
    // Deterministic seek: replay deltas from the nearest complete checkpoint.
    public static Dictionary<int,PotReplayPose> At(PotReplayData data,float time,out int frame){
        frame=0;for(int i=1;i<data.frames.Count&&data.frames[i].t<=time;i++)frame=i;
        int start=frame;while(start>0&&!data.frames[start].full)start--;
        var state=new Dictionary<int,PotReplayPose>();
        for(int i=start;i<=frame;i++)foreach(var pose in data.frames[i].items)state[pose.id]=pose;
        return state;
    }
}
