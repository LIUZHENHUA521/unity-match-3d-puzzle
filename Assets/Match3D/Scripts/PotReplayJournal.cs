using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

// JSONL stays on disk. A ready manifest is small: no accumulated frame objects or full JSON copy.
public static class PotReplayJournal {
    [Serializable] public sealed class Manifest {public PotReplayData data;public int frameCount;public bool recovered;public PotReplayFrame lastFrame;}
    const int MaxLineChars=4*1024*1024;
    public static void Ready(string journal,PotReplayData data,int count,PotReplayFrame recovered=null){
        if(data.frames.Count!=0||count<1)throw new IOException("invalid streaming replay manifest");
        string target=Path.ChangeExtension(journal,".ready");
        var json=JsonUtility.ToJson(new Manifest{data=data,frameCount=count,recovered=recovered!=null,lastFrame=recovered});
        if(Encoding.UTF8.GetByteCount(json)>8*1024*1024)throw new IOException("录像索引超过保存上限");
        PotReplayFiles.Write(target+".tmp",json);PotReplayFiles.Move(target+".tmp",target);
    }
    public static string Line(StreamReader reader){
        var text=new StringBuilder(4096);int c;
        while((c=reader.Read())!=-1){if(c=='\n')break;if(text.Length>=MaxLineChars)throw new IOException("录像单帧过大");text.Append((char)c);}
        return c==-1&&text.Length==0?null:text.ToString();
    }
    public static IEnumerable<string> JsonPieces(string ready){
        if(PotReplayFiles.Size(ready)>8*1024*1024)throw new IOException("录像索引过大");
        var manifest=JsonUtility.FromJson<Manifest>(PotReplayFiles.Read(ready));
        if(manifest?.data==null||manifest.frameCount<1||manifest.frameCount>72002||manifest.data.frames.Count!=0)throw new IOException("录像索引无效");
        string metadata=JsonUtility.ToJson(manifest.data);const string marker="\"frames\":[]";int start=metadata.IndexOf(marker,StringComparison.Ordinal);
        if(start<0)throw new IOException("录像帧索引缺失");
        yield return metadata.Substring(0,start)+"\"frames\":[";
        using(var stream=PotReplayFiles.OpenRead(Path.ChangeExtension(ready,".recording")))using(var reader=new StreamReader(stream,Encoding.UTF8,false,16384)){
            if(Line(reader)==null)throw new IOException("录像初盘缺失");
            for(int i=0;i<manifest.frameCount;i++){
                string frame=Line(reader);if(frame==null||!frame.EndsWith("}",StringComparison.Ordinal))throw new IOException("录像帧不完整");
                if(i>0)yield return ",";
                yield return manifest.recovered&&i==manifest.frameCount-1?JsonUtility.ToJson(manifest.lastFrame):frame;
            }
        }
        yield return "]"+metadata.Substring(start+marker.Length);
    }
    // Old unfinished files are scanned once with O(board + events) memory, never ReadAllText/Split.
    public static System.Collections.IEnumerator Recover(string journal){
        using(var stream=PotReplayFiles.OpenRead(journal))using(var reader=new StreamReader(stream,Encoding.UTF8,false,16384)){
            var header=Line(reader);if(header==null)yield break;
            var data=JsonUtility.FromJson<PotReplayData>(header);if(data==null||data.stage!=2)yield break;
            data.frames.Clear();data.events.Clear();var state=new Dictionary<int,PotReplayPose>();PotReplayFrame last=null;int count=0;
            string line;while((line=Line(reader))!=null){
                PotReplayFrame frame=null;try{if(line.EndsWith("}",StringComparison.Ordinal))frame=JsonUtility.FromJson<PotReplayFrame>(line);}catch{}
                if(frame?.items==null||last!=null&&frame.t<last.t||count==0&&!frame.full)break;
                foreach(var pose in frame.items)state[pose.id]=pose;
                if(frame.events!=null)data.events.AddRange(frame.events);
                if(data.events.Count>50000||state.Count>6000||count>=72002)throw new IOException("录像恢复超过上限");
                last=frame;count++;if(count%16==0)yield return null;
            }
            if(last==null)yield break;
            last.full=true;last.items=state.Values.OrderBy(x=>x.id).ToList();
            data.duration=last.t;data.status="incomplete";
            data.events.RemoveAll(e=>e.type=="finish");for(int i=0;i<data.events.Count;i++)data.events[i].seq=i;
            data.events.Add(new PotReplayEvent{seq=data.events.Count,t=last.t,type="finish"});
            Ready(journal,data,count,last);
        }
    }
    public static void DeleteCompleted(string queue){
        PotReplayFiles.Delete(queue);
        if(queue.EndsWith(".ready",StringComparison.Ordinal))PotReplayFiles.Delete(Path.ChangeExtension(queue,".recording"));
    }
}
