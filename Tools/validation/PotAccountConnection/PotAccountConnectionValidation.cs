using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
public static class PotAccountConnectionValidation {
    static string Failure(UnityWebRequest.Result result,long code,string error)=>(string)typeof(Match3DInvitations).GetMethod("PotRequestFailure",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{result,code,error});
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    public static void Run(){try{
        LoginReadinessValidation.Run();
        Check(Failure(UnityWebRequest.Result.ProtocolError,503,null).Contains("服务暂时繁忙"),"503 is a server failure");
        Check(Failure(UnityWebRequest.Result.ConnectionError,0,"Request timeout").Contains("超时"),"timeout has actionable message");
        Check(Failure(UnityWebRequest.Result.ConnectionError,0,"Cannot resolve host").Contains("连接账号服务失败"),"connection errors do not assert device is offline");
        Check(Failure(UnityWebRequest.Result.Success,200,null).Contains("返回异常"),"malformed 200 is not labeled offline");
        File.WriteAllText("../result.txt","PASS account failures and login readiness; full game compiled.\n");EditorApplication.Exit(0);
    }catch(Exception e){File.WriteAllText("../result.txt","FAIL "+e);Debug.LogException(e);EditorApplication.Exit(1);}}
}
