using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Complete non-winning recordings are useful briefly, but must not crowd out new winning runs.
public static class PotReplayRetention {
    public const long NonWinBudget=32L*1024*1024,RecordingHeadroom=64L*1024*1024;
    public const int NonWinCount=3;
    public const long RetainMilliseconds=2L*86400000;
    static readonly Dictionary<string,int> active=new Dictionary<string,int>();
    static string Key(string owner,string id)=>owner+"_"+id;
    public static bool IsActive(string owner,string id)=>active.ContainsKey(Key(owner,id));
    public static IDisposable Protect(string owner,string id)=>new Lease(Key(owner,id));
    sealed class Lease:IDisposable {
        string key;
        public Lease(string key){this.key=key;active.TryGetValue(key,out int count);active[key]=count+1;}
        public void Dispose(){if(key==null)return;if(active[key]==1)active.Remove(key);else active[key]--;key=null;}
    }
    sealed class Candidate {public PotReplayLocalState.Entry entry;public string[] files;public long bytes;}
    static bool NonWin(string result)=>result=="fail"||result=="quit"||result=="incomplete";
    static bool Metadata(string path)=>path.EndsWith(".ready")||path.EndsWith(".pending")||path.EndsWith(".ready.rejected")||path.EndsWith(".pending.rejected");
    public static void Trim(string owner){
        if(string.IsNullOrEmpty(owner))return;
        try{TrimFiles(owner);}catch(Exception error){Debug.LogWarning("Pot replay cache cleanup: "+error.Message);}
    }
    static void TrimFiles(string owner){
        var sizes=PotReplayFiles.List().ToDictionary(p=>p,p=>PotReplayFiles.Size(p));
        long total=sizes.Values.Sum();var candidates=new List<Candidate>();
        // Import old manifests before considering removal; never infer an outcome from file age alone.
        foreach(var path in sizes.Keys.Where(p=>Path.GetFileName(p).StartsWith(owner+"_",StringComparison.Ordinal)&&Metadata(p))){
            try{PotReplayQueue.Import(owner,path);}catch{ /* Unknown/corrupt payloads remain protected. */ }
        }
        foreach(var entry in PotReplayLocalState.All(owner)){
            if(!System.Text.RegularExpressions.Regex.IsMatch(entry.recordId,"^[a-zA-Z0-9_-]{16,80}$")||entry.state=="recording"||!NonWin(entry.result)||IsActive(owner,entry.recordId))continue;
            var files=sizes.Keys.Where(p=>Path.GetFileName(p).StartsWith(Key(owner,entry.recordId)+".",StringComparison.Ordinal)).ToArray();
            var metadata=files.Where(Metadata).ToArray();if(metadata.Length==0)continue;
            bool safe=true;
            foreach(var path in metadata)try{var data=PotReplayQueue.ReadMetadata(path);if(data==null||data.recordId!=entry.recordId||!NonWin(data.status))safe=false;}catch{safe=false;}
            if(!safe)continue;
            if(entry.retainedAt<=0){entry.retainedAt=entry.updatedAt>0?entry.updatedAt:PotReplayLocalState.Now;PotReplayLocalState.Put(owner,entry);}
            candidates.Add(new Candidate{entry=entry,files=files,bytes=files.Sum(p=>sizes[p])});
        }
        long bytes=candidates.Sum(c=>c.bytes);int count=candidates.Count;
        foreach(var c in candidates.OrderBy(c=>c.entry.retainedAt)){
            if(c.entry.reason!="旧非通关录像已按缓存预算清理"&&count<=NonWinCount&&bytes<=NonWinBudget&&total<=RecordingHeadroom&&PotReplayLocalState.Now-c.entry.retainedAt<=RetainMilliseconds)continue;
            // Mark before deleting. An interrupted cleanup cannot accidentally queue a partial payload.
            PotReplayLocalState.Set(owner,c.entry.recordId,"failed","旧非通关录像已按缓存预算清理");
            // Remove metadata last so an interrupted cleanup can be safely completed on next pass.
            foreach(var path in c.files.OrderBy(p=>Metadata(p)?1:0)){PotReplayFiles.Delete(path);total-=sizes[path];}
            bytes-=c.bytes;count--;
        }
    }
}
