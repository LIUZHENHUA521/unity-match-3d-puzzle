using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

// WeChat's native user directory survives process exit; Unity's in-memory WebGL FS does not.
public static class PotReplayFiles {
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [Serializable] sealed class Request {public string action,path,text,target,fd;public long position;public int length;}
    [Serializable] sealed class Reply {public bool ok,exists;public string error,text;public string[] files;public long size;}
    [DllImport("__Internal")] static extern string PotReplayFile(string json);
    static Reply Call(string action,string path=null,string text=null,string target=null){var r=JsonUtility.FromJson<Reply>(PotReplayFile(JsonUtility.ToJson(new Request{action=action,path=path,text=text,target=target})));if(r==null||!r.ok)throw new IOException(r?.error??"replay filesystem unavailable");return r;}
    static Reply StreamCall(Request request){var r=JsonUtility.FromJson<Reply>(PotReplayFile(JsonUtility.ToJson(request)));if(r==null||!r.ok)throw new IOException(r?.error??"replay stream unavailable");return r;}
    public static Stream OpenRead(string p)=>new NativeStream(p,false);
    public static Stream OpenWrite(string p)=>new NativeStream(p,true);
    sealed class NativeStream:Stream {
        string fd;readonly bool write;long position,length;
        public NativeStream(string path,bool write){this.write=write;length=write?0:Size(path);fd=Call("open",path,target:write?"w":"r").text;}
        public override bool CanRead=>!write&&fd!=null;public override bool CanWrite=>write&&fd!=null;public override bool CanSeek=>false;
        public override long Length=>length;public override long Position{get=>position;set=>throw new NotSupportedException();}
        public override int Read(byte[] buffer,int offset,int count){if(!CanRead)throw new IOException("stream not readable");count=(int)Math.Min(Math.Min(count,32768),length-position);if(count<=0)return 0;var r=StreamCall(new Request{action="readBytes",fd=fd,position=position,length=count});var bytes=Convert.FromBase64String(r.text);if(bytes.Length==0||bytes.Length>count)throw new IOException("short replay read");Buffer.BlockCopy(bytes,0,buffer,offset,bytes.Length);position+=bytes.Length;return bytes.Length;}
        public override void Write(byte[] buffer,int offset,int count){if(!CanWrite)throw new IOException("stream not writable");while(count>0){int n=Math.Min(count,32768);var r=StreamCall(new Request{action="writeBytes",fd=fd,position=position,text=Convert.ToBase64String(buffer,offset,n)});if(r.size!=n)throw new IOException("short replay write");position+=n;length=position;offset+=n;count-=n;}}
        public override void Flush(){}public override long Seek(long offset,SeekOrigin origin)=>throw new NotSupportedException();public override void SetLength(long value)=>throw new NotSupportedException();
        protected override void Dispose(bool disposing){if(fd!=null){var closing=fd;fd=null;StreamCall(new Request{action="close",fd=closing});}base.Dispose(disposing);}
    }
    public static void Ensure()=>Call("mkdir");
    public static void Write(string p,string s)=>Call("write",p,s);
    public static void Append(string p,string s)=>Call("append",p,s);
    public static string Read(string p)=>Call("read",p).text;
    public static bool Exists(string p)=>Call("exists",p).exists;
    public static void Delete(string p)=>Call("delete",p);
    public static void Move(string p,string target)=>Call("move",p,target:target);
    public static long Size(string p)=>Call("size",p).size;
    public static string[] List(string pattern="*"){Ensure();string re="^"+Regex.Escape(pattern).Replace("\\*",".*")+"$";return (Call("list").files??new string[0]).Where(n=>Regex.IsMatch(n,re)).Select(n=>Path.Combine(PotReplayService.DirectoryPath,n)).ToArray();}
#else
    public static Stream OpenRead(string p)=>File.OpenRead(p);
    public static Stream OpenWrite(string p)=>File.Create(p);
    public static void Ensure()=>Directory.CreateDirectory(PotReplayService.DirectoryPath);
    public static void Write(string p,string s)=>File.WriteAllText(p,s);
    public static void Append(string p,string s)=>File.AppendAllText(p,s);
    public static string Read(string p)=>File.ReadAllText(p);
    public static bool Exists(string p)=>File.Exists(p);
    public static void Delete(string p)=>File.Delete(p);
    public static void Move(string p,string target)=>File.Move(p,target);
    public static long Size(string p)=>new FileInfo(p).Length;
    public static string[] List(string pattern="*"){Ensure();return Directory.GetFiles(PotReplayService.DirectoryPath,pattern);}
#endif
}
