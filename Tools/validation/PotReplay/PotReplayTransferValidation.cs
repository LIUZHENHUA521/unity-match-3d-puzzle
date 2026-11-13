using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
public static class PotReplayTransferValidation {
 public static void Run(){try{
  const string journal="/tmp/pot-stream-validation.recording",ready="/tmp/pot-stream-validation.ready",packed="/tmp/pot-stream-validation.upload";
  foreach(var p in new[]{journal,ready,packed})if(File.Exists(p))File.Delete(p);
  var data=new PotReplayData{version="pot-replay-v1",resourceVersion="pot-replay-resources-1",recordId=new string('a',32),runId=new string('b',32),stage=2,status="win",level=new Match3DDailyLevel{date="2026-11-14",tier="bachelor",version="stream-test",seed=1,generatorVersion="daily-v1",seconds=300,initialCount=300,layers=new[]{new ChallengeLayer(300,1,4)}},opening=new Match3DDailyStage{stage=2,items=new Match3DDailyItem[0]}};
  // Use the actual stage item type through JSON so this fixture stays independent of its class name.
  var itemJson="{\"id\":0,\"type\":0,\"position\":{\"x\":0,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1},\"scale\":{\"x\":1,\"y\":1,\"z\":1},\"visible\":true,\"dynamic\":true}";
  data.opening=JsonUtility.FromJson<Match3DDailyStage>("{\"stage\":2,\"items\":["+string.Join(",",Enumerable.Range(0,300).Select(i=>itemJson.Replace("\"id\":0","\"id\":"+i)))+"]}");
  var frame=new PotReplayFrame{full=true,remaining=300,fov=40,cameraSize=5,cameraRotation=Quaternion.identity,tray=new int[0]};
  for(int i=0;i<300;i++)frame.items.Add(new PotReplayPose{id=i,p=Vector3.zero,r=Quaternion.identity,s=Vector3.one,visible=true,zone=0});
  int count=0;using(var writer=new StreamWriter(journal,false,new UTF8Encoding(false))){writer.WriteLine(JsonUtility.ToJson(data));for(;count<700;count++){frame.t=count*.1f;writer.WriteLine(JsonUtility.ToJson(frame));}}
  data.duration=frame.t;data.events.Add(new PotReplayEvent{seq=0,t=data.duration,type="finish"});
  long size=new FileInfo(journal).Length;if(size<=16*1024*1024)throw new Exception("fixture too small");
  long before=GC.GetTotalMemory(true);PotReplayJournal.Ready(journal,data,count);long finishGrowth=GC.GetTotalMemory(false)-before;
  if(finishGrowth>8*1024*1024)throw new Exception("finish allocated a full replay copy");
  var key=Enumerable.Range(0,32).Select(i=>(byte)i).ToArray();var iv=new byte[16];
  var pack=PotReplayTransfer.PackFile(ready,packed,Convert.ToBase64String(key),Convert.ToBase64String(iv));while(pack.MoveNext()){}
  using(var aes=new AesManaged()){aes.Key=key;aes.IV=iv;var encrypted=File.ReadAllBytes(packed);var compressed=aes.CreateDecryptor().TransformFinalBlock(encrypted,0,encrypted.Length);var result=JsonUtility.FromJson<PotReplayData>(PotReplayTransfer.Unpack(compressed));if(result.frames.Count!=count||result.duration!=data.duration||result.events.Last().type!="finish")throw new Exception("stream round trip mismatch");}
  // Recovery ignores an incomplete trailing line and uses a full final checkpoint, without building all frames.
  File.Delete(ready);File.AppendAllText(journal,"{\"t\":");var recover=PotReplayJournal.Recover(journal);while(recover.MoveNext()){}
  var m=JsonUtility.FromJson<PotReplayJournal.Manifest>(File.ReadAllText(ready));if(m.frameCount!=count||!m.recovered||!m.lastFrame.full||m.lastFrame.items.Count!=300||m.data.status!="incomplete"||m.data.frames.Count!=0)throw new Exception("stream recovery mismatch");
  // Legacy pending JSON also streams through the same bounded compressor.
  string legacy="/tmp/pot-stream-legacy.pending";File.WriteAllText(legacy,"{\"payload\":\""+new string('x',17*1024*1024)+"\"}");pack=PotReplayTransfer.PackFile(legacy,"/tmp/pot-stream-legacy.upload",Convert.ToBase64String(key),Convert.ToBase64String(iv));while(pack.MoveNext()){}
  var go=new GameObject("Pot recording first-stage check");var recorder=go.AddComponent<PotReplayRecorder>();recorder.Begin(new Match3DGameRecords.Record{stage=1},new Match3DDailyLevel());if(PotReplayRecorder.Recording)throw new Exception("first stage recorded");UnityEngine.Object.DestroyImmediate(go);
  Debug.Log("POT_REPLAY_STREAM_OK bytes="+size+" frames="+count+" finishManagedGrowth="+finishGrowth+" packed="+new FileInfo(packed).Length);EditorApplication.Exit(0);
 }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}}
}
