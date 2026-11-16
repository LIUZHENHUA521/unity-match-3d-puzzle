using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

// Keep v2 JSON compatible while omitting floating-point tail digits in the largest field.
public static class PotReplayEncoding {
    public const float CheckpointSeconds=30;
    static readonly CultureInfo Culture=CultureInfo.InvariantCulture;
    static void Vector(StringBuilder b,Vector3 v){b.Append("{\"x\":").Append(v.x.ToString("0.###",Culture)).Append(",\"y\":").Append(v.y.ToString("0.###",Culture)).Append(",\"z\":").Append(v.z.ToString("0.###",Culture)).Append('}');}
    static float Round(float n,int digits)=>(float)Math.Round(n,digits);
    static Vector3 Round(Vector3 v)=>new Vector3(Round(v.x,3),Round(v.y,3),Round(v.z,3));
    public static PotReplayPose Compact(PotReplayPose pose,PotReplayPose previous){
        // Removed pooled bodies can be re-enabled by an effect far outside the board.
        // They cannot appear in the playable camera and must not poison a whole replay.
        if(pose.zone==3&&(Mathf.Abs(pose.p.x)>100||Mathf.Abs(pose.p.z)>100||pose.p.y< -100||pose.p.y>4096)){
            pose.visible=false;if(pose.render!=null)pose.render=new bool[pose.render.Length];
        }
        pose.p=Round(pose.p);pose.s=Round(pose.s);var q=pose.r.normalized;
        pose.r=new Quaternion(Round(q.x,4),Round(q.y,4),Round(q.z,4),Round(q.w,4));
        // Invisible motion is unobservable. Emit the current transform when visible again.
        if(previous!=null&&!pose.visible){pose.p=previous.p;pose.r=previous.r;pose.s=previous.s;}
        return pose;
    }
    public static bool Changed(PotReplayPose pose,PotReplayPose previous)=>!pose.Same(previous);
    public static string Frame(PotReplayFrame frame){
        var items=frame.items;string shell;
        frame.items=new List<PotReplayPose>();try{shell=JsonUtility.ToJson(frame);}finally{frame.items=items;}
        const string marker="\"items\":[]";int at=shell.IndexOf(marker,StringComparison.Ordinal);if(at<0)throw new InvalidOperationException("Replay frame items missing");
        var b=new StringBuilder(shell.Length+items.Count*200);b.Append(shell,0,at).Append("\"items\":[");
        for(int i=0;i<items.Count;i++){
            if(i>0)b.Append(',');var p=items[i];b.Append("{\"id\":").Append(p.id).Append(",\"zone\":").Append(p.zone).Append(",\"visible\":").Append(p.visible?"true":"false").Append(",\"render\":[");
            for(int j=0;j<(p.render?.Length??0);j++){if(j>0)b.Append(',');b.Append(p.render[j]?"true":"false");}b.Append("],\"p\":");Vector(b,p.p);b.Append(",\"s\":");Vector(b,p.s);
            b.Append(",\"r\":{\"x\":").Append(p.r.x.ToString("0.####",Culture)).Append(",\"y\":").Append(p.r.y.ToString("0.####",Culture)).Append(",\"z\":").Append(p.r.z.ToString("0.####",Culture)).Append(",\"w\":").Append(p.r.w.ToString("0.####",Culture)).Append("}}");
        }
        return b.Append(']').Append(shell,at+marker.Length,shell.Length-at-marker.Length).ToString();
    }
}
