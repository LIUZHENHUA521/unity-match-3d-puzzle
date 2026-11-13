using System;
using System.Collections;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

// The upload ticket is scoped to one encrypted object; no CloudBase login secrets enter the client.
public static class PotReplayTransfer {
    public const int MaxFileBytes=16*1024*1024;
    // Pack one disk-backed frame/chunk at a time; never allocate an entire uncompressed replay.
    public static IEnumerator PackFile(string source,string target,string key,string iv,Func<bool> pause=null){
        using(var output=new LimitedStream(PotReplayFiles.OpenWrite(target),MaxFileBytes))using(var aes=new AesManaged()){
            aes.Key=Convert.FromBase64String(key);aes.IV=Convert.FromBase64String(iv);aes.Mode=CipherMode.CBC;aes.Padding=PaddingMode.PKCS7;
            using(var cipher=new CryptoStream(output,aes.CreateEncryptor(),CryptoStreamMode.Write,true))using(var gzip=new GZipStream(cipher,System.IO.Compression.CompressionLevel.Fastest)){
                long total=0;float budget=Time.realtimeSinceStartup;
                if(source.EndsWith(".ready",StringComparison.Ordinal)){
                    using(var writer=new StreamWriter(gzip,new UTF8Encoding(false),32768,true))foreach(var part in PotReplayJournal.JsonPieces(source)){
                        if(pause!=null&&pause())throw new OperationCanceledException("录像将在空闲时继续同步");
                        total+=Encoding.UTF8.GetByteCount(part);if(total>PotReplayService.MaxJsonBytes)throw new IOException("录像展开大小超过64MiB");
                        writer.Write(part);
                        if(Time.realtimeSinceStartup-budget>.004f){writer.Flush();yield return null;budget=Time.realtimeSinceStartup;}
                    }
                }else{
                    using(var input=PotReplayFiles.OpenRead(source)){var buffer=new byte[32768];int count;
                        while((count=input.Read(buffer,0,buffer.Length))>0){
                            if(pause!=null&&pause())throw new OperationCanceledException("录像将在空闲时继续同步");
                            total+=count;if(total>PotReplayService.MaxJsonBytes)throw new IOException("录像展开大小超过64MiB");gzip.Write(buffer,0,count);
                            if(Time.realtimeSinceStartup-budget>.004f){yield return null;budget=Time.realtimeSinceStartup;}
                        }
                    }
                }
            }
        }
    }
    sealed class LimitedStream:Stream {
        readonly Stream output;readonly long limit;long length;
        public LimitedStream(Stream output,long limit){this.output=output;this.limit=limit;}
        public override bool CanRead=>false;public override bool CanSeek=>false;public override bool CanWrite=>true;
        public override long Length=>length;public override long Position{get=>length;set=>throw new NotSupportedException();}
        public override void Write(byte[] b,int offset,int count){if(length+count>limit)throw new IOException("录像压缩大小超过16MiB");output.Write(b,offset,count);length+=count;}
        public override void Flush()=>output.Flush();public override int Read(byte[] b,int offset,int count)=>throw new NotSupportedException();
        public override long Seek(long offset,SeekOrigin origin)=>throw new NotSupportedException();public override void SetLength(long value)=>throw new NotSupportedException();
        protected override void Dispose(bool disposing){if(disposing)output.Dispose();base.Dispose(disposing);}
    }
    public static string Unpack(byte[] bytes){
        using(var input=new MemoryStream(bytes))using(var gzip=new GZipStream(input,CompressionMode.Decompress))using(var output=new MemoryStream()){
            var buffer=new byte[32768];int count;
            while((count=gzip.Read(buffer,0,buffer.Length))>0){if(output.Length+count>PotReplayService.MaxJsonBytes)throw new IOException("录像展开大小超过64MiB");output.Write(buffer,0,count);}
            return Encoding.UTF8.GetString(output.ToArray());
        }
    }
}
