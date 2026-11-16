using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class PotGloryView : MonoBehaviour {
    public Text count,status;
    public RectTransform content;
    public ScrollRect scroll;
    public PotGloryRow gold,blue;
    public Button more,retry,close;
    readonly List<PotGloryRow> rows=new List<PotGloryRow>();
    Match3DFlow flow;PotReplayService service;Action dismiss;
    string owner;int cursor;bool loading,hasMore,opening;string newest;
    static string Account(){var a=Match3DInvitations.Instance;return a&&a.Ready?a.State.inviteCode:null;}
    bool Current()=>this&&isActiveAndEnabled&&Account()==owner;
    public void Connect(Match3DFlow f,Action onClose){
        flow=f;dismiss=onClose;owner=Account();service=PotReplayService.Instance;
        close.onClick.AddListener(()=>{MarkSeen();dismiss();});
        more.onClick.AddListener(()=>Fetch(false));retry.onClick.AddListener(()=>Fetch(cursor==0));
        if(service)service.Uploaded+=Uploaded;
        Fetch(true);
    }
    void MarkSeen(){if(!string.IsNullOrEmpty(owner)&&!string.IsNullOrEmpty(newest)){PlayerPrefs.SetString("pot.glory.seen."+owner,newest);PlayerPrefs.Save();}}
    void OnDestroy(){if(service)service.Uploaded-=Uploaded;}
    void Update(){if(!string.IsNullOrEmpty(owner)&&Account()!=owner){dismiss?.Invoke();flow.Notice("账号已变化，请重新打开我的荣耀");}}
    void Uploaded(string account){if(account==owner&&!opening&&!loading)Fetch(true);}
    void Fetch(bool reset){
        if(loading||opening)return;
        if(string.IsNullOrEmpty(owner)){owner=Account();if(string.IsNullOrEmpty(owner)){status.text="请先完成登录，再查看我的荣耀";retry.gameObject.SetActive(true);return;}}
        if(!service)service=PotReplayService.Instance;
        if(!service){status.text="荣耀服务暂不可用，请稍后重试";retry.gameObject.SetActive(true);return;}
        if(reset){foreach(var row in rows)Destroy(row.gameObject);rows.Clear();cursor=0;scroll.verticalNormalizedPosition=1;}
        loading=true;status.text="正在加载荣耀…";retry.gameObject.SetActive(false);more.gameObject.SetActive(false);
        StartCoroutine(Match3DStartupRunner.Run(service.Honors(cursor,reply=>{
            if(!Current())return;loading=false;
            if(reply==null||!reply.ok){status.text=reply?.error??"加载失败，请重试";retry.gameObject.SetActive(true);return;}
            Render(reply);
        }),error=>{if(!Current())return;loading=false;status.text="加载失败，请重试";retry.gameObject.SetActive(true);Debug.LogWarning(error.Message);}));
    }
    public void Render(PotReplayService.Reply reply){
        count.text="已收获 <color=#F65300><size=46>"+reply.total+"</size></color> 次通关荣耀";
        var honors=reply.honors??new PotReplayService.Honor[0];
        foreach(var honor in honors){
            if(honor?.certificate==null)continue;
            var row=Instantiate(honor.certificate.rewardTier=="elite"?gold:blue,content,false);row.gameObject.SetActive(true);
            var rect=(RectTransform)row.transform;rect.anchoredPosition=new Vector2(0,-rows.Count*277);
            bool first=rows.Count==0;if(first)newest=honor.certificate.certificateKey;
            row.Configure(honor,first&&!string.IsNullOrEmpty(owner)&&newest!=PlayerPrefs.GetString("pot.glory.seen."+owner),()=>{if(!Current())return;MarkSeen();flow.ShowPotCollectedCertificate(honor.certificate);},()=>{
                if(opening||!Current())return;
                if(string.IsNullOrEmpty(honor.replayId)){flow.Notice("这局录像尚未同步或未保存，请稍后重试");return;}
                opening=true;MarkSeen();flow.OpenPotGloryReplay(honor.replayId,Current,error=>{opening=false;});
            });rows.Add(row);
        }
        cursor+=honors.Length;hasMore=reply.hasMore&&honors.Length>0;
        content.sizeDelta=new Vector2(633,Mathf.Max(850,rows.Count*277+(hasMore?72:0)));
        more.gameObject.SetActive(hasMore);((RectTransform)more.transform).anchoredPosition=new Vector2(0,-rows.Count*277-8);
        status.text=rows.Count==0?"还没有通关荣耀\n通关本科局或清北局，收藏你的第一张奖状吧！":"";
    }
}
