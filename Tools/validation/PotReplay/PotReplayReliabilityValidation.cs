using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PotReplayReliabilityValidation {
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    public static void Run(){try{
        const string owner="pot-reliability-fixture",id="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        PlayerPrefs.DeleteKey("pot.replay.state.v1."+owner);
        foreach(var path in PotReplayFiles.List(owner+"_*"))PotReplayFiles.Delete(path);
        var level=new Match3DDailyLevel{date="2026-11-16",tier="bachelor"};
        var record=new Match3DGameRecords.Record{recordId=id,status="win",stage=2};
        PotReplayLocalState.Start(owner,record,level);PotReplayLocalState.Set(owner,id,"failed","memory");PotReplayLocalState.Outcome(owner,record);
        Check(PotReplayLocalState.Caption(owner,level.date,level.tier)=="本局回放未保存","failed recording not pending");
        PotReplayLocalState.Set(owner,id,"queued");PotReplayLocalState.Retry(owner,id,"network offline");var entry=PotReplayLocalState.Get(owner,id);
        Check(entry.state=="queued"&&entry.nextAttempt>PotReplayLocalState.Now,"network retry persists");
        for(int i=0;i<25;i++)PotReplayLocalState.Retry(owner,id,"offline");entry=PotReplayLocalState.Get(owner,id);
        Check(entry.state=="queued"&&entry.nextAttempt-PotReplayLocalState.Now<=300000,"backoff capped without terminal failure");
        Check(PotReplayLocalState.Get("another-owner",id)==null,"owner isolation");
        PotReplayQueue.Reconcile(owner);Check(PotReplayLocalState.Get(owner,id).state=="failed","missing file detected at restart");
        var data=new PotReplayData{recordId=id,stage=2,status="win",duration=1800,level=level,opening=new Match3DDailyStage{stage=2,items=new Match3DDailyItem[0]}};
        string journal=PotReplayService.PathFor(owner,id,".recording"),ready=PotReplayService.PathFor(owner,id,".ready"),packed=PotReplayService.PathFor(owner,id,".upload");
        var previous=new Dictionary<int,PotReplayPose>();int emitted=0;float nextFull=0;long before=GC.GetTotalMemory(true);
        using(var writer=new StreamWriter(journal)){
            writer.WriteLine(JsonUtility.ToJson(data));
            for(int n=0;n<=18000;n++){
                float t=n*.1f;bool full=t>=nextFull||n==18000;if(full)nextFull=t+PotReplayEncoding.CheckpointSeconds;
                var frame=new PotReplayFrame{t=t,full=full,tray=new int[0],cameraRotation=Quaternion.identity,fov=40,cameraSize=5,presentation=new PotReplayPresentation(),events=new PotReplayEvent[0]};
                for(int i=0;i<330;i++){
                    bool visible=i<100&&!(i<4&&n>9000);
                    var pose=new PotReplayPose{id=i,zone=i<4&&n>9000?3:0,visible=visible,render=new[]{visible},p=new Vector3(i%20,i/20,i<12?Mathf.Sin(t*.4f+i)*2:n%2*.0001f),r=Quaternion.Euler(0,i<12?t*4:i*13.731f,0),s=Vector3.one};
                    previous.TryGetValue(i,out var old);var compact=PotReplayEncoding.Compact(pose,old);
                    if(full||PotReplayEncoding.Changed(compact,old)){frame.items.Add(compact);previous[i]=compact;}
                }
                string json=PotReplayEncoding.Frame(frame);writer.WriteLine(json);emitted+=frame.items.Count;
                if(n%901==0){var decoded=JsonUtility.FromJson<PotReplayFrame>(json);Check(decoded.items.Count==frame.items.Count,"frame roundtrip");foreach(var p in decoded.items){var expected=frame.items.First(x=>x.id==p.id);Check(Vector3.Distance(p.p,expected.p)<.002f,"position precision");Check(Quaternion.Angle(p.r.normalized,expected.r.normalized)<.1f,"angle precision");}}
            }
        }
        for(int i=0;i<1000;i++){
            var q=Quaternion.Euler(i*.317f,i*13.731f,i*.71f);
            var pose=PotReplayEncoding.Compact(new PotReplayPose{r=q,s=Vector3.one,render=new bool[0]},null);
            Check(pose.Same(pose),"identical rounded quaternion never produces false motion");
        }
        // Subthreshold drift must accumulate against the emitted state, not disappear forever.
        var last=new PotReplayPose{p=Vector3.zero,s=Vector3.one,r=Quaternion.identity,render=new[]{true},visible=true};bool changed=false;
        for(int n=1;n<20;n++){var pose=new PotReplayPose{p=new Vector3(n*.0002f,0,0),s=Vector3.one,r=Quaternion.identity,render=new[]{true},visible=true};if(PotReplayEncoding.Changed(PotReplayEncoding.Compact(pose,last),last)){changed=true;break;}}
        Check(changed,"slow movement eventually emitted");
        long jsonBytes=new FileInfo(journal).Length;Check(jsonBytes<PotReplayService.MaxJsonBytes,"30min JSON under 64MiB: "+jsonBytes);
        data.events.Add(new PotReplayEvent{seq=0,t=1800,type="finish"});PotReplayJournal.Ready(journal,data,18001);
        PotReplayLocalState.Set(owner,id,"uploading",result:"win");PotReplayQueue.Reconcile(owner);
        Check(PotReplayLocalState.Get(owner,id).state=="queued","interrupted upload resumes");
        entry=PotReplayLocalState.Get(owner,id);entry.nextAttempt=0;PotReplayLocalState.Put(owner,entry);
        var loser=new PotReplayLocalState.Entry{recordId=new string('b',32),state="queued",result="fail",updatedAt=0};PotReplayLocalState.Put(owner,loser);
        string loserPath=PotReplayService.PathFor(owner,loser.recordId,".pending");File.WriteAllText(loserPath,"{}");
        Check(PotReplayQueue.Next(owner)==ready,"win priority");
        var pack=PotReplayTransfer.PackFile(ready,packed);while(pack.MoveNext()){}
        long packedBytes=new FileInfo(packed).Length;Check(packedBytes<PotReplayTransfer.MaxFileBytes,"30min gzip under16MiB");
        var decodedData=JsonUtility.FromJson<PotReplayData>(PotReplayTransfer.Unpack(File.ReadAllBytes(packed)));
        foreach(float time in new[]{0f,899.9f,901f,1500f,1800f}){int index;var state=PotReplayState.At(decodedData,time,out index);Check(state.Count==330,"seek full object state");Check(state[0].visible==(decodedData.frames[index].t<=900),"seek visibility transition");var expected=new Vector3(5,0,Mathf.Sin(decodedData.frames[index].t*.4f+5)*2);Check(Vector3.Distance(state[5].p,expected)<.002f,"seek moving pose");}
        // Cancellation never commits a partial gzip or removes its original journal.
        Exception cancelled=null;var interrupted=Match3DStartupRunner.Run(PotReplayCache.Prepare(ready,()=>true),e=>cancelled=e);while(interrupted.MoveNext()){}
        Check(cancelled is OperationCanceledException&&File.Exists(journal)&&!File.Exists(PotReplayCache.Packed(ready))&&!File.Exists(Path.ChangeExtension(ready,".packing")),"cancelled compression retains original, discards partial output");
        // Commit gzip before deleting journal. A repeated prepare reuses the committed bytes.
        var prepare=PotReplayCache.Prepare(ready,()=>false);while(prepare.MoveNext()){if(prepare.Current is System.Collections.IEnumerator child)while(child.MoveNext()){} }
        string durable=PotReplayCache.Packed(ready);Check(File.Exists(durable)&&!File.Exists(journal),"gzip commit frees raw journal");
        var committed=File.ReadAllBytes(durable);prepare=PotReplayCache.Prepare(ready,()=>false);while(prepare.MoveNext()){}
        Check(committed.SequenceEqual(File.ReadAllBytes(durable)),"retry reuses committed gzip without source");
        // Simulate crash after rename before cleanup; resume safely removes only redundant source.
        File.WriteAllText(journal,"redundant source");prepare=PotReplayCache.Prepare(ready,()=>false);while(prepare.MoveNext()){}
        Check(!File.Exists(journal)&&File.Exists(durable),"crash after rename preserves committed payload");
        File.WriteAllText(journal,"retained quarantine fixture");
        var memory=new PotReplayMemoryPolicy();Check(!memory.Stop(0)&&memory.SampleInterval==.5f,"first pressure degrades sampling");Check(memory.Stop(2),"repeated pressure stops");
        Check(new PotReplayMemoryPolicy().Stop(0,15),"critical pressure stops immediately");
        PotReplayLocalState.Phase(owner,id,"put",403);entry=PotReplayLocalState.Get(owner,id);Check(entry.phase=="put"&&entry.httpStatus==403&&entry.replayVersion==PotReplayData.Version,"structured upload failure persisted");
        entry.diagnosticAttempt=PotReplayLocalState.Now;PotReplayLocalState.Put(owner,entry);Check(PotReplayLocalState.NextDiagnostic(owner).recordId!=id,"diagnostics rotate beyond failing record");
        // Legacy rejected complete recordings get one repair retry, then stay retained.
        PotReplayFiles.Move(ready,ready+".rejected");PotReplayLocalState.Set(owner,id,"failed","old validation");PotReplayQueue.RestoreRejected(owner);Check(File.Exists(ready),"legacy repair queued");
        PotReplayFiles.Move(ready,ready+".rejected");PotReplayLocalState.Set(owner,id,"failed","validation");PotReplayQueue.RestoreRejected(owner);Check(File.Exists(ready+".rejected")&&!File.Exists(ready),"repair attempted once");
        entry=PotReplayLocalState.Get(owner,id);entry.updatedAt=PotReplayLocalState.Now-8L*86400000;PotReplayLocalState.Put(owner,entry);PotReplayQueue.Reconcile(owner);Check(File.Exists(journal)&&File.Exists(ready+".rejected"),"complete quarantine retained");
        var removed=PotReplayEncoding.Compact(new PotReplayPose{zone=3,visible=true,render=new[]{true},p=new Vector3(0,-10000,0),s=Vector3.one,r=Quaternion.identity},last);
        Check(!removed.visible&&!removed.render[0]&&removed.p==last.p,"far-away removed effect cannot poison frame");
        string staleId=new string('d',32);var stale=new PotReplayLocalState.Entry{recordId=staleId,state="failed",updatedAt=PotReplayLocalState.Now-8L*86400000};PotReplayLocalState.Put(owner,stale);
        string staleRaw=PotReplayService.PathFor(owner,staleId,".recording");File.WriteAllText(staleRaw,"interrupted");PotReplayQueue.Reconcile(owner);Check(!File.Exists(staleRaw)&&PotReplayLocalState.Get(owner,staleId).state=="failed","only aged incomplete payload cleaned, reason remains");
        string legacyId=new string('9',32);data.recordId=legacyId;string legacyPath=PotReplayService.PathFor(owner,legacyId,".pending");File.WriteAllText(legacyPath,JsonUtility.ToJson(data));
        var imported=PotReplayQueue.Import(owner,legacyPath);Check(imported.result=="win"&&imported.tier=="bachelor","legacy JSON metadata recovered for home and win priority");
        // Pressure reclaims only known failed, incomplete journals in the same account.
        string pressureId=new string('6',32),otherRaw=PotReplayService.PathFor("other-pressure-owner",pressureId,".recording"),failedRaw=PotReplayService.PathFor(owner,pressureId,".recording");
        PotReplayLocalState.Put(owner,new PotReplayLocalState.Entry{recordId=pressureId,state="failed",updatedAt=PotReplayLocalState.Now});
        File.WriteAllText(failedRaw,"abandoned partial");using(var sparse=File.Create(otherRaw))sparse.SetLength(97L*1024*1024);
        try{PotReplayQueue.Reconcile(owner);Check(!File.Exists(failedRaw)&&File.Exists(otherRaw)&&File.Exists(durable),"pressure cleanup preserves other owner and complete winning payload");}finally{File.Delete(otherRaw);}
        PotReplayLocalState.Set(owner,id,"synced");PotReplayQueue.Reconcile(owner);Check(!PotReplayFiles.Exists(durable)&&!PotReplayFiles.Exists(journal)&&!PotReplayFiles.Exists(ready+".rejected"),"server-confirmed leftovers cleaned on restart");
        foreach(var path in PotReplayFiles.List(owner+"_*"))PotReplayFiles.Delete(path);PlayerPrefs.DeleteKey("pot.replay.state.v1."+owner);
        Debug.Log("POT_REPLAY_RELIABILITY_OK seconds=1800 objects=330 samples=18001 json="+jsonBytes+" gzip="+packedBytes+" emitted="+emitted);EditorApplication.Exit(0);
    }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}}
}
