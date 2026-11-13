using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

// WeChat's native user directory survives process exit; Unity's in-memory WebGL FS does not.
public static class PotReplayFiles {
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [Serializable] sealed class Request {public string action,path,text,target;}
    [Serializable] sealed class Reply {public bool ok,exists;public string error,text;public string[] files;public long size;}
    [DllImport("__Internal")] static extern string PotReplayFile(string json);
    static Reply Call(string action,string path=null,string text=null,string target=null){var r=JsonUtility.FromJson<Reply>(PotReplayFile(JsonUtility.ToJson(new Request{action=action,path=path,text=text,target=target})));if(r==null||!r.ok)throw new IOException(r?.error??"replay filesystem unavailable");return r;}
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
