using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class PotReplayQueue {
    public static string Id(string owner,string path){var name=Path.GetFileName(path);return name.Substring(owner.Length+1).Split('.')[0];}
    public static PotReplayLocalState.Entry Import(string owner,string path){
        string id=Id(owner,path);var entry=PotReplayLocalState.Get(owner,id);
        if(entry!=null)return entry;
        entry=new PotReplayLocalState.Entry{recordId=id,state="queued",updatedAt=PotReplayLocalState.Now,retainedAt=PotReplayLocalState.Now};
        var data=ReadMetadata(path);
        if(data!=null){if(data.recordId!=id)throw new IOException("录像对局不匹配");entry.date=data.level?.date;entry.tier=data.level?.tier;entry.result=data.status;}
        PotReplayLocalState.Put(owner,entry);return entry;
    }
    public static PotReplayData ReadMetadata(string path){
        PotReplayData data=null;
        if(path.Contains(".ready")){
            if(PotReplayFiles.Size(path)>8*1024*1024)throw new IOException("录像索引过大");
            var m=JsonUtility.FromJson<PotReplayJournal.Manifest>(PotReplayFiles.Read(path));
            if(m?.data==null)throw new IOException("录像索引无效");
            data=m.data;
        }
        else if(path.Contains(".pending")){
            // Legacy full JSON stores metadata before the frame array; read only that prefix.
            using(var stream=PotReplayFiles.OpenRead(path))using(var reader=new StreamReader(stream)){
                var prefix=new StringBuilder();var buffer=new char[4096];int count;
                while(prefix.Length<8*1024*1024&&(count=reader.Read(buffer,0,buffer.Length))>0){
                    prefix.Append(buffer,0,count);string text=prefix.ToString();int index=text.IndexOf("\"frames\":[",StringComparison.Ordinal);
                    if(index>=0){data=JsonUtility.FromJson<PotReplayData>(text.Substring(0,index).TrimEnd(',')+"}");break;}
                }
            }
        }
        return data;
    }
    public static string Next(string owner){
        var candidates=PotReplayFiles.List(owner+"_*.ready").Concat(PotReplayFiles.List(owner+"_*.pending"));
        return candidates.Select(path=>new{path,entry=SafeImport(owner,path)}).Where(x=>x.entry!=null&&x.entry.nextAttempt<=PotReplayLocalState.Now&&x.entry.state!="failed"&&x.entry.state!="synced")
            .OrderBy(x=>x.entry.result=="win"?0:1).ThenBy(x=>x.entry.updatedAt).Select(x=>x.path).FirstOrDefault();
    }
    static PotReplayLocalState.Entry SafeImport(string owner,string path){
        try{return Import(owner,path);}catch(Exception e){var id=Id(owner,path);PotReplayLocalState.Put(owner,new PotReplayLocalState.Entry{recordId=id,state="failed",reason=e.Message,updatedAt=PotReplayLocalState.Now});return null;}
    }
    public static void RestoreRejected(string owner){
        foreach(var path in PotReplayFiles.List(owner+"_*.rejected"))try{
            if(!path.EndsWith(".ready.rejected")&&!path.EndsWith(".pending.rejected"))continue;
            var e=Import(owner,path);if(e.repairVersion>=1)continue;
            e.repairVersion=1;PotReplayLocalState.Put(owner,e);
            string target=path.Substring(0,path.Length-".rejected".Length);
            if(!PotReplayFiles.Exists(target))PotReplayFiles.Move(path,target);
            PotReplayLocalState.Set(owner,e.recordId,"queued","旧版录像重新校验");
        }catch(Exception error){Debug.LogWarning("Pot legacy replay: "+error.Message);}
    }
    public static void Reconcile(string owner){
        bool pressure=PotReplayFiles.List().Sum(p=>PotReplayFiles.Size(p))>96L*1024*1024;
        foreach(var e in PotReplayLocalState.All(owner)){
            if(PotReplayRetention.IsActive(owner,e.recordId))continue;
            if(e.state=="synced"){PotReplayCache.DeleteSynced(owner,e.recordId);continue;}
            string ready=PotReplayService.PathFor(owner,e.recordId,".ready"),pending=PotReplayService.PathFor(owner,e.recordId,".pending");
            bool complete=PotReplayFiles.Exists(ready)||PotReplayFiles.Exists(pending);
            if(complete&&(e.state=="recording"||e.state=="uploading"))PotReplayLocalState.Set(owner,e.recordId,"queued","中断后继续同步");
            else if(!complete&&(e.state=="recording"||e.state=="queued"||e.state=="uploading"))PotReplayLocalState.Set(owner,e.recordId,"failed","未找到完整录像文件");
            // Never remove complete payloads, even if permanently rejected by validation.
            if(e.state=="failed"&&(pressure||PotReplayLocalState.Now-e.updatedAt>7L*86400000)&&!complete&&!PotReplayFiles.Exists(ready+".rejected")&&!PotReplayFiles.Exists(pending+".rejected")&&!PotReplayFiles.Exists(PotReplayCache.Packed(ready))){
                string raw=PotReplayService.PathFor(owner,e.recordId,".recording");if(PotReplayFiles.Exists(raw))PotReplayFiles.Delete(raw);
            }
        }
        PotReplayLocalState.Prune(owner);
    }
}
