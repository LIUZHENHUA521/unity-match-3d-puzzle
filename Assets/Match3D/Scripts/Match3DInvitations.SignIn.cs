using System;
using System.Collections;
using UnityEngine;

public sealed partial class Match3DInvitations {
    [Serializable] class SignInBody { public string date; public int cycle,day; }
    public Match3DSignInState SignInState {get;private set;}
    public string SignInStatus {get;private set;}="正在读取签到奖励…";
    public bool SignInBusy {get;private set;}
    public bool SignInCurrent => Ready && SignInState!=null && Time.realtimeSinceStartup<signInRefreshAt;
    public event Action SignInChanged;
    float signInRefreshAt,signInRetryAt;
    void ClearSignIn(){SignInState=null;signInRefreshAt=0;SignInChanged?.Invoke();}
    void PollSignIn(){if(Ready&&!Busy&&!logging&&!SignInBusy&&SignInState!=null&&!SignInCurrent&&Time.realtimeSinceStartup>=signInRetryAt)RefreshSignIn();}
    void OnApplicationPause(bool paused){if(!paused){signInRefreshAt=0;if(Ready)RefreshSignIn();}}
    public void RefreshSignIn(){if(!SignInBusy)StartCoroutine(SignInOperation(false));}
    public void ClaimSignIn(){if(!SignInBusy)StartCoroutine(SignInOperation(true));}
    void AcceptSignIn(Reply reply){
        Apply(reply);SignInState=reply.signIn;
        signInRefreshAt=Time.realtimeSinceStartup+Mathf.Clamp((reply.signIn.nextRefreshAt-reply.signIn.serverNow)/1000f,1,86400);
        SignInStatus="";
    }
    IEnumerator SignInOperation(bool claim) {
        SignInBusy=true;SignInStatus=claim?"正在领取…":"正在读取签到奖励…";SignInChanged?.Invoke();
        bool acquired=false;
        try {
            bool ready=false;yield return EnsureReady(ok=>ready=ok);
            if(!ready){SignInStatus=Status;yield break;}
            float deadline=Time.realtimeSinceStartup+35;
            while((Busy||logging)&&Time.realtimeSinceStartup<deadline)yield return null;
            if(Busy||logging||!Ready){SignInStatus="账号服务忙，请稍后重试";yield break;}
            // Share the inventory lock with login, mail, consume and refund.
            Busy=true;acquired=true;Changed?.Invoke();
            string owner=State.inviteCode;
            if(!claim||!SignInCurrent){
                Reply fresh=null;yield return Request("sign-in/state","{}",r=>fresh=r);
                if(fresh?.signIn==null||!fresh.signIn.Valid){signInRefreshAt=0;SignInStatus=string.IsNullOrEmpty(Status)?"签到数据暂不可用，请重试":Status;yield break;}
                AcceptSignIn(fresh);
            }
            if(!claim||!SignInState.canClaim)yield break;
            var body=new SignInBody{date=SignInState.date,cycle=SignInState.cycle,day=SignInState.day};
            Reply reply=null;yield return Request("sign-in/claim",JsonUtility.ToJson(body),r=>reply=r);
            if(reply?.signIn==null||!reply.signIn.Valid||reply.signInReceipt==null){
                // An uncertain grant is reconciled by state, never a local increment.
                signInRefreshAt=0;SignInStatus=string.IsNullOrEmpty(Status)?"领取结果待确认，请刷新重试":Status;yield break;
            }
            AcceptSignIn(reply);
            if(State.inviteCode==owner&&Match3DFlow.Instance)
                Match3DFlow.Instance.ShowReceivedTools(reply.signInReceipt.rewards,"开心收下");
        } finally {
            if(acquired)Busy=false;SignInBusy=false;signInRetryAt=Time.realtimeSinceStartup+30;
            Changed?.Invoke();SignInChanged?.Invoke();
        }
    }
}
