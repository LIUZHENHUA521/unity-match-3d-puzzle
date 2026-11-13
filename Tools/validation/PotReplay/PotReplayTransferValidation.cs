using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
public static class PotReplayTransferValidation {
 public static void Run(){try{
  string text="{\"test\":\""+new string('x',17*1024*1024)+"\"}";
  var key=new byte[32];var iv=new byte[16];for(int i=0;i<key.Length;i++)key[i]=(byte)i;
  var bytes=PotReplayTransfer.Pack(text,Convert.ToBase64String(key),Convert.ToBase64String(iv));
  using(var aes=new AesManaged()){aes.Key=key;aes.IV=iv;var plain=aes.CreateDecryptor().TransformFinalBlock(bytes,0,bytes.Length);if(PotReplayTransfer.Unpack(plain)!=text)throw new Exception("gzip/AES round trip failed");}
  File.WriteAllBytes("/tmp/pot-replay-transfer-validation.bin",bytes);
  var go=new GameObject("Pot recording first-stage check");var recorder=go.AddComponent<PotReplayRecorder>();
  recorder.Begin(new Match3DGameRecords.Record{stage=1},new Match3DDailyLevel());
  if(typeof(PotReplayRecorder).GetField("data",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(recorder)!=null)throw new Exception("first stage recorded");
  UnityEngine.Object.DestroyImmediate(go);
  Debug.Log("POT_REPLAY_TRANSFER_OK 17MiB gzip/AES roundtrip + first-stage skip");EditorApplication.Exit(0);
 }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}}
}
