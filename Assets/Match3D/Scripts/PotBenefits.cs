using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public sealed class PotBenefits : MonoBehaviour {
    [Serializable] public class State { public int[] progress; public bool[] claimed; }
    [Serializable] public class Evidence { public int requestId,scene,claim; public bool added; public string encryptedData,iv; }
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern void PotReadBenefits(string target,int club,int requestId);
#endif
    RectTransform overlay,panel;
    readonly Button[] actions=new Button[3];
    readonly Text[] counts=new Text[3],labels=new Text[3];
    bool club,busy;
    State state;
    string day,account;
    int generation;
    static int sequence;
    Evidence evidence;
    public void Configure(Match3DHomeView home){
        var icon=Picture(home.artwork,"PotWechatEntry",15,551,148,131,"wechat-entry");
        Match3DUIView.AddClickListener(icon.gameObject.AddComponent<Button>(),()=>Open(false));
        Match3DUIView.AddClickListener(home.community,()=>Open(true));
    }
    static Image Picture(Transform parent,string name,float x,float y,float w,float h,string asset=null){
        var obj=new GameObject(name,typeof(RectTransform),typeof(Image));
        var r=(RectTransform)obj.transform;r.SetParent(parent,false);
        r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);
        r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x,-y);
        var image=obj.GetComponent<Image>();
        if(asset!=null)image.sprite=Resources.Load<Sprite>("Match3D/PotBenefits/pot-"+asset);
        return image;
    }
    static Text Caption(Transform parent){
        var obj=new GameObject("Caption",typeof(RectTransform),typeof(Text));
        var r=(RectTransform)obj.transform;r.SetParent(parent,false);r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
        var text=obj.GetComponent<Text>();text.font=Match3DDefaultFont.Current;text.fontSize=29;text.fontStyle=FontStyle.Bold;text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;
        return text;
    }
    void Open(bool circle){
        if(overlay)return;
        club=circle;state=null;
        var shade=Picture(transform,"PotBenefitsOverlay",0,0,0,0);
        overlay=shade.rectTransform;overlay.anchorMin=Vector2.zero;overlay.anchorMax=Vector2.one;overlay.offsetMin=overlay.offsetMax=Vector2.zero;
        shade.color=new Color(0,0,0,.67f);
        var art=Picture(overlay,"Panel",0,0,689,club?1137:776,club?"club":"wechat");
        panel=art.rectTransform;panel.anchorMin=panel.anchorMax=panel.pivot=Vector2.one*.5f;
        var close=Picture(panel,"Close",608,162,81,81);close.color=Color.clear;
        Match3DUIView.AddClickListener(close.gameObject.AddComponent<Button>(),Close);
        for(int i=0;i<(club?3:2);i++){
            int row=i;
            var button=Picture(panel,"Task",club?421:461,club?408+i*241:366+i*176,club?203:164,club?101:92,club?"club-go":"go");
            actions[i]=button.gameObject.AddComponent<Button>();labels[i]=Caption(button.transform);
            Match3DUIView.AddClickListener(actions[i],()=>Act(row));
            if(club){
                var badge=Picture(panel,"Progress",277,347+i*241,89,35);badge.color=Color.clear;
                counts[i]=Caption(badge.transform);counts[i].fontSize=25;counts[i].color=new Color32(18,65,120,255);
            }
        }
        Match3DModalMotion.Play(overlay.gameObject);
        Refresh();
    }
    void Close(){generation++;busy=false;Destroy(overlay.gameObject);overlay=null;}
    void Refresh(){if(overlay&&!busy)StartCoroutine(Read(-1));}
    [UnityEngine.Scripting.Preserve] public void OnPotClubClosed(string unused){Refresh();}
    void OnApplicationFocus(bool focus){if(focus)Refresh();}
    void OnApplicationPause(bool paused){if(!paused)Refresh();}
    void Update(){
        if(!overlay)return;
        panel.localScale=Vector3.one*Mathf.Min(overlay.rect.width/720f,overlay.rect.height/(panel.rect.height+24));
        var a=Match3DInvitations.Instance;
        if(day!=Match3DFlow.Today||account!=(a&&a.Ready?a.State.inviteCode:null))Refresh();
    }
    void Act(int row){
        int task=club?row+2:row;
        if(busy)return;
        if(state!=null&&state.progress[task]>=(club?2:1)){StartCoroutine(Read(task));return;}
        if(club)Match3DGameCommunity.Open();
        else Match3DFlow.Instance.ShowHomeFeatureNotice(row==0?"点击微信右上角「…」，选择「添加到我的小程序」":"点击微信右上角「…」，选择「添加到桌面」，再从桌面图标进入游戏");
        Refresh();
    }
    void Render(){
        for(int row=0;row<(club?3:2);row++){
            int i=club?row+2:row;
            bool claimed=state!=null&&state.claimed[i],ready=state!=null&&state.progress[i]>=(club?2:1);
            var b=actions[row];b.interactable=!busy&&!claimed;
            string prefix=club?"Match3D/PotBenefits/pot-club-":"Match3D/PotBenefits/pot-";
            b.image.sprite=Resources.Load<Sprite>(prefix+(ready?"claim":"go"));
            labels[row].text=claimed?"已领取":"";
            if(claimed){b.image.sprite=null;b.image.color=new Color32(91,147,179,255);}else b.image.color=Color.white;
            if(club)counts[row].text=state==null?"—":state.progress[i]+"/2";
        }
    }
    IEnumerator Read(int claim){
        busy=true;int version=++generation;
        day=Match3DFlow.Today;
        var a=Match3DInvitations.Instance;account=a&&a.Ready?a.State.inviteCode:null;
        Render();
        bool ready=false;
        if(a)yield return a.EnsureReady(ok=>ready=ok);
        if(version!=generation)yield break;
        if(ready){
            account=a.State.inviteCode;
            int id=++sequence;evidence=null;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
            PotReadBenefits(gameObject.name,club?1:0,id);
            float deadline=Time.realtimeSinceStartup+8;
            while(version==generation&&(evidence==null||evidence.requestId!=id)&&Time.realtimeSinceStartup<deadline)yield return null;
#endif
            if(version!=generation)yield break;
            var data=evidence!=null&&evidence.requestId==id?evidence:new Evidence();
            data.claim=claim;Match3DInvitations.Reply reply=null;
            yield return a.StartCoroutine(a.PotBenefitRequest(data,account,r=>reply=r));
            if(version!=generation)yield break;
            state=reply?.potBenefits;
            if(claim>=0||state==null)Match3DFlow.Instance.ShowHomeFeatureNotice(state!=null?"领取成功，道具已到账":a.Status);
            else if(club&&string.IsNullOrEmpty(data.encryptedData))Match3DFlow.Instance.ShowHomeFeatureNotice("暂未取得游戏圈数据，请检查微信授权后重新打开福利");
        }else if(a)Match3DFlow.Instance.ShowHomeFeatureNotice(a.Status);
        busy=false;Render();
    }
    [UnityEngine.Scripting.Preserve] public void OnPotBenefits(string json){evidence=JsonUtility.FromJson<Evidence>(json);}
}
public sealed partial class Match3DInvitations {
    public IEnumerator PotBenefitRequest(PotBenefits.Evidence data,string owner,Action<Reply> done){
        while(Busy||logging)yield return null;
        if(!Ready||State.inviteCode!=owner)yield break;
        Busy=true;
        try{
            yield return Request("pot-benefits",JsonUtility.ToJson(data),r=>{Apply(r);done(r);});
        }finally{Busy=false;Changed?.Invoke();}
    }
}
