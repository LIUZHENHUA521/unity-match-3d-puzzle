using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// A small durable ledger survives missing payloads, process exit and failed disk writes.
public static class PotReplayLocalState {
    [Serializable] public sealed class Entry {
        public string recordId,date,tier,result,state,reason,phase,clientVersion,replayVersion;public long httpStatus,diagnosticAttempt;
        public long updatedAt,nextAttempt;public int attempts,repairVersion;public bool reported;
    }
    [Serializable] sealed class Ledger {public List<Entry> entries=new List<Entry>();}
    const string Prefix="pot.replay.state.v1.";
    public static long Now=>DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    static Ledger Read(string owner){
        try{var ledger=JsonUtility.FromJson<Ledger>(PlayerPrefs.GetString(Prefix+owner,""))??new Ledger();
            ledger.entries=(ledger.entries??new List<Entry>()).Where(e=>e!=null&&!string.IsNullOrEmpty(e.recordId)).ToList();return ledger;
        }catch{return new Ledger();}
    }
    public static Entry[] All(string owner)=>Read(owner).entries.ToArray();
    public static Entry Get(string owner,string id)=>Array.Find(All(owner),x=>x.recordId==id);
    public static void Put(string owner,Entry entry){
        if(string.IsNullOrEmpty(owner)||entry==null||string.IsNullOrEmpty(entry.recordId))return;
        var ledger=Read(owner);ledger.entries.RemoveAll(x=>x.recordId==entry.recordId);ledger.entries.Add(entry);
        try{PlayerPrefs.SetString(Prefix+owner,JsonUtility.ToJson(ledger));PlayerPrefs.Save();}catch(Exception error){Debug.LogWarning("Pot replay state: "+error.Message);}
    }
    public static void Prune(string owner){
        var ledger=Read(owner);
        ledger.entries.RemoveAll(e=>(e.state=="failed"||e.state=="synced")&&Now-e.updatedAt>30L*86400000&&!PotReplayFiles.List(owner+"_"+e.recordId+".*").Any());
        try{PlayerPrefs.SetString(Prefix+owner,JsonUtility.ToJson(ledger));PlayerPrefs.Save();}catch(Exception error){Debug.LogWarning(error.Message);}
    }
    public static void Start(string owner,Match3DGameRecords.Record record,Match3DDailyLevel level){Put(owner,new Entry{recordId=record.recordId,date=level.date,tier=level.tier,state="recording",updatedAt=Now});}
    public static void Set(string owner,string id,string state,string reason="",string result=null){
        var e=Get(owner,id);if(e==null)return;e.state=state;e.reason=(reason??"").Substring(0,Math.Min(240,(reason??"").Length));if(result!=null)e.result=result;
        e.updatedAt=Math.Max(Now,e.updatedAt+1);e.reported=false;if(state=="synced"){e.attempts=0;e.nextAttempt=0;}Put(owner,e);
    }
    public static void Phase(string owner,string id,string phase,long httpStatus=0){var e=Get(owner,id);if(e==null)return;e.phase=phase;e.httpStatus=Math.Max(0,Math.Min(599,httpStatus));e.clientVersion=PotReplayBuild.Version;e.replayVersion=PotReplayData.Version;e.updatedAt=Math.Max(Now,e.updatedAt+1);e.reported=false;Put(owner,e);}
    public static Entry NextDiagnostic(string owner)=>All(owner).Where(e=>!e.reported&&e.state!="recording").OrderBy(e=>e.diagnosticAttempt).ThenBy(e=>e.updatedAt).FirstOrDefault();
    public static void Retry(string owner,string id,string reason){var e=Get(owner,id);if(e==null)return;e.attempts=Math.Min(e.attempts+1,20);e.nextAttempt=Now+(long)Math.Min(300,5*Math.Pow(2,e.attempts-1))*1000;Put(owner,e);Set(owner,id,"queued",reason);}
    public static void Outcome(string owner,Match3DGameRecords.Record record){var e=Get(owner,record.recordId);if(e!=null){e.result=record.status;e.reported=false;Put(owner,e);}}
    public static string Caption(string owner,string date,string tier){
        var rows=All(owner).Where(x=>x.date==date&&x.tier==tier&&x.result=="win").ToArray();
        if(rows.Any(x=>x.state=="uploading"))return "回放同步中";
        if(rows.Any(x=>x.state=="queued"||x.state=="synced"))return "回放待同步";
        return "本局回放未保存";
    }
    public static string Explanation(string owner,string date,string tier){
        var caption=Caption(owner,date,tier);
        return caption=="本局回放未保存"?"已通关，但本局没有可用的完整回放。通关结果不受影响。":"已通关，回放正在同步，请保持联网并在首页稍候。";
    }
}
