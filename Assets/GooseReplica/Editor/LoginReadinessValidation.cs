using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public static class LoginReadinessValidation {
    static void Set(object target,string field,object value){target.GetType().GetField(field,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(target,value);}
    static void Check(bool condition,string message){if(!condition)throw new Exception(message);Debug.Log("PASS login readiness: "+message);}
    public static void Run(){
        var root=new GameObject("Login readiness validation");
        try{
            var account=root.AddComponent<ReplicaInvitations>();
            Set(account,"logging",true);
            bool? first=null,second=null;
            IEnumerator a=account.EnsureReady(ok=>first=ok),b=account.EnsureReady(ok=>second=ok);
            Check(a.MoveNext()&&b.MoveNext()&&first==null&&second==null,"early clicks wait for the same WeChat login");
            Set(account,"logging",false);Set(account,"<Busy>k__BackingField",true);
            Check(a.MoveNext()&&b.MoveNext(),"wait continues during server token exchange");
            Set(account,"<State>k__BackingField",new ReplicaInvitations.Reply{inviteCode="validation",inventory=new int[3],unlimitedMagnet=true});
            Set(account,"token","validation-token");
            Check(!a.MoveNext()&&!b.MoveNext()&&first==true&&second==true,"both callers continue once authenticated, before mailbox finishes");
            bool? ready=null;Check(!account.EnsureReady(ok=>ready=ok).MoveNext()&&ready==true,"authenticated callers do not wait");
            Set(account,"token",null);Set(account,"logging",true);Set(account,"<Busy>k__BackingField",false);
            bool? cancelled=null;Check(!account.EnsureReady(ok=>cancelled=ok,()=>true).MoveNext()&&cancelled==false,"leaving the page cancels the pending start");
            bool? failed=null;var failure=account.EnsureReady(ok=>failed=ok);
            Check(failure.MoveNext(),"new attempt can wait after cancellation");
            account.OnInvitationBridge("{\"error\":\"微信登录失败，请重试\"}");
            Check(!failure.MoveNext()&&failed==false&&account.Status=="微信登录失败，请重试","login failure releases waiters and preserves the reason");
            Set(account,"logging",true);Set(account,"loginDeadline",Time.realtimeSinceStartup-1);
            bool? timedOut=null;var timeout=account.EnsureReady(ok=>timedOut=ok);Check(timeout.MoveNext(),"timeout attempt begins waiting");
            typeof(ReplicaInvitations).GetMethod("Update",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(account,null);
            Check(!timeout.MoveNext()&&timedOut==false&&account.Status=="登录超时，请重试","WeChat timeout releases waiters");
            bool? retry=null;Set(account,"logging",true);var next=account.EnsureReady(ok=>retry=ok);Check(next.MoveNext(),"retry waits for a fresh login");
            Set(account,"token","retry-token");Set(account,"logging",false);
            Check(!next.MoveNext()&&retry==true,"retry succeeds without restarting the game");
            Debug.Log("LOGIN READINESS VALIDATION PASSED");
        }finally{UnityEngine.Object.DestroyImmediate(root);}
    }
}
