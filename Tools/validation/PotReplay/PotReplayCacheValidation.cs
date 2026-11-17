using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class PotReplayCacheValidation {
    const string Owner="pot-cache-fixture",Other="pot-cache-other";
    static string Id(int n)=>n.ToString("x32");
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static System.Collections.IEnumerator ProtectedPrepare(string path,Func<bool> cancel){using(PotReplayRetention.Protect(Owner,Id(1)))yield return PotReplayCache.Prepare(path,cancel,()=>true);}
    static void Clean(){foreach(var owner in new[]{Owner,Other}){foreach(var p in PotReplayFiles.List(owner+"_*"))PotReplayFiles.Delete(p);PlayerPrefs.DeleteKey("pot.replay.state.v1."+owner);}}
    static string Add(int n,string result,long bytes=0,long age=0,string owner=Owner){
        string id=Id(n),path=PotReplayService.PathFor(owner,id,".ready");
        var data=new PotReplayData{recordId=id,status=result,stage=2,level=new Match3DDailyLevel()};
        PotReplayFiles.Write(path,JsonUtility.ToJson(new PotReplayJournal.Manifest{data=data,frameCount=1}));
        using(var file=File.Create(Path.ChangeExtension(path,".recording")))file.SetLength(bytes);
        PotReplayLocalState.Put(owner,new PotReplayLocalState.Entry{recordId=id,state="queued",result=result,updatedAt=PotReplayLocalState.Now-age,retainedAt=PotReplayLocalState.Now-age});
        return path;
    }
    public static void Run(){Clean();try{
        var win=Add(1,"win",20L*1024*1024);
        for(int i=2;i<7;i++)Add(i,"fail",40L*1024*1024);
        PotReplayRetention.Trim(Owner);
        Check(PotReplayFiles.Exists(win)&&PotReplayFiles.Size(Path.ChangeExtension(win,".recording"))==20L*1024*1024,"old nonwins reclaimed without losing winning payload");
        Check(PotReplayFiles.List(Owner+"_*").Sum(PotReplayFiles.Size)<=PotReplayRetention.RecordingHeadroom,"continuous failures leave space for a new full recording");
        Check(PotReplayLocalState.Get(Owner,Id(2)).state=="failed","eviction retains explanatory ledger");
        Clean();
        for(int i=1;i<=4;i++)Add(i,"fail",16,5000-i*100);
        PotReplayRetention.Trim(Owner);Check(!PotReplayFiles.Exists(PotReplayService.PathFor(Owner,Id(1),".ready"))&&PotReplayFiles.List(Owner+"_*.ready").Length==3,"keep most recent bounded count");
        Clean();
        string aged=Add(1,"quit",16,3L*86400000);long retained=PotReplayLocalState.Get(Owner,Id(1)).retainedAt;
        PotReplayLocalState.Retry(Owner,Id(1),"network");Check(PotReplayLocalState.Get(Owner,Id(1)).retainedAt==retained,"retry never refreshes retention age");
        PotReplayRetention.Trim(Owner);Check(!PotReplayFiles.Exists(aged),"expired nonwin reclaimed despite recent retry");
        Clean();
        string active=Add(1,"fail",40L*1024*1024),other=Add(2,"fail",40L*1024*1024,0,Other),unknown=Add(3,"",40L*1024*1024),corrupt=Add(4,"fail",40L*1024*1024),conflict=Add(5,"win",40L*1024*1024);
        PotReplayFiles.Write(corrupt,"bad json");var entry=PotReplayLocalState.Get(Owner,Id(5));entry.result="fail";PotReplayLocalState.Put(Owner,entry);
        using(PotReplayRetention.Protect(Owner,Id(1))){PotReplayRetention.Trim(Owner);PotReplayQueue.Reconcile(Owner);Check(PotReplayFiles.Exists(active)&&PotReplayLocalState.Get(Owner,Id(1)).state=="queued","active uploader protected");}
        foreach(var p in new[]{other,unknown,corrupt,conflict})Check(PotReplayFiles.Exists(p)&&PotReplayFiles.Exists(Path.ChangeExtension(p,".recording")),"unknown/conflicting/other-account files preserved");
        PotReplayRetention.Trim(Owner);Check(!PotReplayFiles.Exists(active),"released nonwin becomes reclaimable");
        Clean();
        // Resume cleanup if the process died after raw removal but before manifest deletion.
        string partial=Add(1,"fail",16);File.Delete(Path.ChangeExtension(partial,".recording"));PotReplayLocalState.Set(Owner,Id(1),"failed","旧非通关录像已按缓存预算清理");
        PotReplayRetention.Trim(Owner);Check(!File.Exists(partial),"interrupted cleanup completes even after usage drops");
        Clean();
        // Recovery is open across frames while Begin's reconciliation sees >96MiB pressure.
        string raw=PotReplayService.PathFor(Owner,Id(1),".recording"),pressure=PotReplayService.PathFor(Other,Id(2),".recording");
        var data=new PotReplayData{recordId=Id(1),stage=2,level=new Match3DDailyLevel()};
        using(var writer=new StreamWriter(raw)){writer.WriteLine(JsonUtility.ToJson(data));for(int i=0;i<40;i++)writer.WriteLine(JsonUtility.ToJson(new PotReplayFrame{t=i,full=i==0}));}
        using(var file=File.Create(pressure))file.SetLength(100L*1024*1024);
        PotReplayLocalState.Put(Owner,new PotReplayLocalState.Entry{recordId=Id(1),state="recording",updatedAt=PotReplayLocalState.Now});
        bool paused=false;var recover=PotReplayJournal.Recover(raw,()=>false,()=>paused);
        using(PotReplayRetention.Protect(Owner,Id(1))){
            Check(recover.MoveNext(),"recovery yields with input stream open");paused=true;
            PotReplayQueue.Reconcile(Owner);PotReplayRetention.Trim(Owner);
            Check(File.Exists(raw)&&PotReplayLocalState.Get(Owner,Id(1)).state=="recording","new game pressure cannot delete or fail active recovery");
            for(int i=0;i<10;i++)Check(recover.MoveNext(),"recovery remains paused");paused=false;while(recover.MoveNext()){}
            Check(File.Exists(Path.ChangeExtension(raw,".ready")),"recovery resumes from protected stream");
        }
        Check(!PotReplayRetention.IsActive(Owner,Id(1)),"recovery releases protection");
        Clean();
        // Account switch and service teardown must close suspended native streams before releasing leases.
        string source=PotReplayService.PathFor(Owner,Id(1),".pending");File.WriteAllText(source,"{\"recordId\":\""+Id(1)+"\",\"frames\":[]}");
        bool cancel=false;Exception cancelled=null;
        var routine=Match3DStartupRunner.Run(ProtectedPrepare(source,()=>cancel),e=>cancelled=e);
        Check(routine.MoveNext()&&PotReplayRetention.IsActive(Owner,Id(1)),"suspended compressor holds lease");cancel=true;while(routine.MoveNext()){}
        Check(cancelled is OperationCanceledException&&!PotReplayRetention.IsActive(Owner,Id(1))&&File.Exists(source)&&!File.Exists(Path.ChangeExtension(source,".packing")),"account cancellation releases lease and retains source");
        routine=Match3DStartupRunner.Run(ProtectedPrepare(source,()=>false),e=>{throw e;});Check(routine.MoveNext(),"teardown fixture paused");((IDisposable)routine).Dispose();
        Check(!PotReplayRetention.IsActive(Owner,Id(1))&&File.Exists(source)&&!File.Exists(Path.ChangeExtension(source,".packing")),"service teardown disposes nested compressor safely");
        Clean();
        var honor=new PotReplayService.Honor{certificate=new PotCertificateData{recordId=Id(1)},replayState="unavailable"};
        Check(PotGloryReplayState.Resolve(honor,Owner)=="unavailable","server failure visible on another device");
        honor.replayState=null;Check(PotGloryReplayState.Resolve(honor,Owner)=="pending","legacy server remains compatible");
        PotReplayLocalState.Put(Owner,new PotReplayLocalState.Entry{recordId=Id(1),state="failed"});
        Check(PotGloryReplayState.Resolve(honor,Owner)=="unavailable"&&PotGloryReplayState.Resolve(honor,Other)=="pending","local diagnostic account isolated");
        honor.replayId="confirmed";Check(PotGloryReplayState.Resolve(honor,Owner)=="ready","ready index overrides stale local failure");
        Check(!PotGloryReplayState.Explanation("unavailable").Contains("重试"),"unsaved does not promise retry recovery");
        Debug.Log("POT_REPLAY_CACHE_OK pressure/count/age/isolation/active-recovery/glory");
    }finally{Clean();}}
}
