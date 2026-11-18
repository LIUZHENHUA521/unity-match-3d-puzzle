using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public sealed class PotBenefits : MonoBehaviour {
    [Serializable] public class State { public int[] progress,granted; public bool[] claimed; }
    [Serializable] public class Evidence { public int requestId,scene,claim; public bool added,supportsUnknown,clubMembership; public string encryptedData,iv,error; }
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern void PotReadBenefits(string target,int club,int requestId);
#endif
    RectTransform overlay,panel;
    ScrollRect clubScroll;
    Text clubFooter;
    readonly Button[] actions=new Button[4];
    readonly Text[] counts=new Text[4],labels=new Text[4];
    bool club,busy;
    readonly bool[] claiming=new bool[4];
    static readonly int[] ClubTasks={5,2,3,4};
    static readonly string[] ClubTitles={"游戏圈加圈","点赞帖子2次","评论帖子2次","发表帖子1次"};
    static readonly string[] ClubCards={"club-like-row","club-like-row","club-comment-row","club-post-row"};
    int TaskAt(int row)=>club?ClubTasks[row]:row;
    static int Goal(int task)=>task==2||task==3?2:1;
    bool HasTask(int task)=>state?.progress!=null&&state.claimed!=null&&task<state.progress.Length&&task<state.claimed.Length;
    int navigationTask=-1;
    float nextClubRead;
    int returnReads;
    Material claimedMaterial;
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
    static Text RowText(RectTransform parent,string name,int x,int y,int width,int height,string value,int size){
        var box=Picture(parent,name,x,y,width,height);
        box.color=Color.clear;box.raycastTarget=false;
        var label=Caption(box.transform);
        label.fontSize=size;label.text=value;label.color=new Color32(18,65,120,255);
        return label;
    }
    RectTransform ClubList(){
        var viewport=Picture(panel,"Tasks",41,331,592,705);
        viewport.color=Color.clear;
        var view=viewport.rectTransform;
        view.gameObject.AddComponent<RectMask2D>();
        var scroll=view.gameObject.AddComponent<ScrollRect>();
        clubScroll=scroll;
        var content=new GameObject("Cards",typeof(RectTransform)).GetComponent<RectTransform>();
        content.SetParent(view,false);content.anchorMin=content.anchorMax=content.pivot=new Vector2(0,1);
        content.sizeDelta=new Vector2(592,946);
        scroll.viewport=view;scroll.content=content;scroll.horizontal=false;
        scroll.movementType=ScrollRect.MovementType.Clamped;
        var track=Picture(panel,"Scrollbar",637,333,8,700);
        track.color=new Color32(193,222,245,255);
        var thumb=Picture(track.transform,"Handle",0,0,0,0);
        thumb.color=new Color32(83,165,229,255);
        var bar=track.gameObject.AddComponent<Scrollbar>();
        bar.handleRect=thumb.rectTransform;bar.targetGraphic=thumb;
        bar.direction=Scrollbar.Direction.BottomToTop;scroll.verticalScrollbar=bar;
        clubFooter=RowText(panel,"Reset",44,1047,590,45,"加圈奖励仅领一次 · 其余每日零点刷新",23);
        return content;
    }
    RectTransform ClubCard(RectTransform content,int row){
        var card=Picture(content,"Task "+ClubTasks[row],0,row*241,592,223,ClubCards[row]).rectTransform;
        var title=RowText(card,"Title",30,9,202,50,ClubTitles[row],30);
        title.alignment=TextAnchor.MiddleLeft;
        counts[row]=RowText(card,"Progress",236,12,89,40,"—",25);
        string qty=row==0?"x2":"x1";
        RowText(card,"First quantity",63,165,100,40,qty,22);
        RowText(card,"Second quantity",220,165,100,40,qty,22);
        return card;
    }
    void Open(bool circle){
        if(overlay)return;
        club=circle;state=null;Array.Clear(claiming,0,claiming.Length);
        var shade=Picture(transform,"PotBenefitsOverlay",0,0,0,0);
        overlay=shade.rectTransform;overlay.anchorMin=Vector2.zero;overlay.anchorMax=Vector2.one;overlay.offsetMin=overlay.offsetMax=Vector2.zero;
        shade.color=new Color(0,0,0,.67f);
        var art=Picture(overlay,"Panel",0,0,689,club?1137:776,club?"club":"wechat");
        panel=art.rectTransform;panel.anchorMin=panel.anchorMax=panel.pivot=Vector2.one*.5f;
        var close=Picture(panel,"Close",608,162,81,81);close.color=Color.clear;
        Match3DUIView.AddClickListener(close.gameObject.AddComponent<Button>(),Close);
        var cards=club?ClubList():null;
        for(int i=0;i<(club?4:2);i++){
            int row=i;
            var parent=club?ClubCard(cards,i):panel;
            var button=Picture(parent,"Task button",club?380:461,club?77:366+i*176,club?203:164,club?101:92,club?"club-go":"go");
            actions[i]=button.gameObject.AddComponent<Button>();labels[i]=Caption(button.transform);
            labels[i].rectTransform.anchoredPosition=new Vector2(0,club?9:6);
            var outline=labels[i].gameObject.AddComponent<Outline>();
            outline.effectColor=new Color32(62,68,75,255);outline.effectDistance=new Vector2(2,-2);
            Match3DUIView.AddClickListener(actions[i],()=>Act(row));
        }
        Match3DModalMotion.Play(overlay.gameObject);
        Refresh();
    }
    void Close(){returnReads=0;navigationTask=-1;generation++;busy=false;Destroy(overlay.gameObject);overlay=null;}
    void Refresh(){if(overlay&&!busy)StartCoroutine(Read(-1));}
    [UnityEngine.Scripting.Preserve] public void OnPotClubClosed(string unused){
        if(!overlay||!club)return;
        returnReads=3;nextClubRead=0;
    }
    void OnApplicationFocus(bool focus){if(focus){if(navigationTask>=0)nextClubRead=0;else Refresh();}}
    void OnApplicationPause(bool paused){if(!paused){if(navigationTask>=0)nextClubRead=0;else Refresh();}}
    void Update(){
        if(!overlay)return;
        panel.localScale=Vector3.one*Mathf.Min(overlay.rect.width/720f,overlay.rect.height/(panel.rect.height+24));
        if((navigationTask>=0||returnReads>0)&&!busy&&Time.unscaledTime>=nextClubRead){
            if(returnReads>0&&--returnReads==0)navigationTask=-1;
            nextClubRead=Time.unscaledTime+6;
            Refresh();
        }
        var a=Match3DInvitations.Instance;
        if(day!=Match3DFlow.Today||account!=(a&&a.Ready?a.State.inviteCode:null))Refresh();
    }
    void Act(int row){
        int task=TaskAt(row);
        if(claiming[row]||(HasTask(task)&&state.claimed[task]))return;
        if(HasTask(task)&&state.progress[task]>=Goal(task)){StartCoroutine(Claim(row,task));return;}
        if(club){
            navigationTask=task;returnReads=0;nextClubRead=Time.unscaledTime+4;
            Match3DGameCommunity.Open();return;
        }
        else Match3DFlow.Instance.ShowHomeFeatureNotice(row==0?"点击微信右上角「…」，选择「添加到我的小程序」":"点击微信右上角「…」，选择「添加到桌面」，再从桌面图标进入游戏");
        Refresh();
    }
    IEnumerator Claim(int row,int task){
        var current=overlay;
        claiming[row]=true;
        Render();
        while(current&&overlay==current&&busy)yield return null;
        if(!current||overlay!=current)yield break;
        if(HasTask(task)&&!state.claimed[task]&&state.progress[task]>=Goal(task))
            yield return Read(task);
        if(!current||overlay!=current)yield break;
        claiming[row]=false;
        Render();
    }
    void OnDestroy(){if(claimedMaterial)Destroy(claimedMaterial);}
    void FitClubRows(){
        bool showJoin=HasTask(ClubTasks[0])&&!state.claimed[ClubTasks[0]];
        if(clubScroll.vertical==showJoin&&clubScroll.content.GetChild(0).gameObject.activeSelf==showJoin)return;
        clubScroll.StopMovement();
        var content=clubScroll.content;
        content.GetChild(0).gameObject.SetActive(showJoin);
        for(int i=1;i<ClubTasks.Length;i++){
            var row=(RectTransform)content.GetChild(i);
            row.anchoredPosition=new Vector2(0,-241*(showJoin?i:i-1));
        }
        content.sizeDelta=new Vector2(592,showJoin?946:705);
        content.anchoredPosition=Vector2.zero;
        clubScroll.vertical=showJoin;
        clubScroll.verticalScrollbar.gameObject.SetActive(showJoin);
        clubFooter.text=showJoin?"加圈奖励仅领一次 · 其余每日零点刷新":"每日零点刷新";
    }
    void Render(){
        if(club)FitClubRows();
        for(int row=0;row<(club?4:2);row++){
            int i=TaskAt(row);
            bool claimed=HasTask(i)&&state.claimed[i],ready=HasTask(i)&&state.progress[i]>=Goal(i);
            var b=actions[row];
            var colors=b.colors;
            colors.disabledColor=claimed?Color.white:ColorBlock.defaultColorBlock.disabledColor;
            b.colors=colors;
            b.interactable=!claiming[row]&&!claimed;
            string prefix=club?"Match3D/PotBenefits/pot-club-":"Match3D/PotBenefits/pot-";
            b.image.sprite=Resources.Load<Sprite>(prefix+(ready&&!claimed?"claim":"go"));
            labels[row].text=claimed?"已领取":"";
            if(claimed&&!claimedMaterial)claimedMaterial=new Material(Resources.Load<Shader>("Match3D/PotBenefits/PotClaimedButton"));
            b.image.material=claimed?claimedMaterial:null;
            if(club)counts[row].text=!HasTask(i)||state.progress[i]<0?"—":Mathf.Min(state.progress[i],Goal(i))+"/"+Goal(i);
        }
    }
    IEnumerator Read(int claim){
        busy=true;int version=++generation;
        if(club && (day!=Match3DFlow.Today || account!=Match3DInvitations.Instance?.State?.inviteCode))state=null;
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
            float deadline=Time.realtimeSinceStartup+(club?120:8);
            while(version==generation&&(evidence==null||evidence.requestId!=id)&&Time.realtimeSinceStartup<deadline)yield return null;
#endif
            if(version!=generation)yield break;
            var data=evidence!=null&&evidence.requestId==id?evidence:new Evidence();
            bool missing=club&&(string.IsNullOrEmpty(data.encryptedData)||string.IsNullOrEmpty(data.iv));
            if(missing&&string.IsNullOrEmpty(data.error)){
                data.error="timeout";
                Debug.LogWarning("[PotBenefits] WeChat callback unavailable, request "+id);
            }
            data.supportsUnknown=true;data.clubMembership=true;
            data.claim=missing?-1:claim;
            Match3DInvitations.Reply reply=null;
            yield return a.StartCoroutine(a.PotBenefitRequest(data,account,r=>reply=r));
            if(version!=generation)yield break;
            state=reply?.potBenefits;
            // Reuse the signed membership proof and the ordinary one-time claim transaction.
            if(club&&claim==-1&&!missing&&HasTask(5)&&state.progress[5]==1&&!state.claimed[5]){
                data.claim=5;
                Match3DInvitations.Reply joined=null;
                yield return a.StartCoroutine(a.PotBenefitRequest(data,account,r=>joined=r));
                if(version!=generation)yield break;
                if(joined!=null)state=joined.potBenefits;
                else Match3DFlow.Instance.ShowHomeFeatureNotice(a.Status);
            }
            if(missing&&state!=null){
                for(int i=2;i<state.progress.Length;i++)state.progress[i]=-1;
                Match3DFlow.Instance.ShowHomeFeatureNotice(ClubReadError(data.error));
            }else if(state==null)Match3DFlow.Instance.ShowHomeFeatureNotice(a.Status);
            if(navigationTask>=0&&(missing||!HasTask(navigationTask)||state.progress[navigationTask]>=Goal(navigationTask))){navigationTask=-1;returnReads=0;}

        }else if(a){navigationTask=-1;returnReads=0;Match3DFlow.Instance.ShowHomeFeatureNotice(a.Status);}
        busy=false;Render();
    }
    static string ClubReadError(string reason){
        switch(reason){
            case "privacy":return "请确认小游戏隐私授权后重试";
            case "denied":return "请在微信右上角设置中允许读取游戏圈数据，再重新打开福利";
            case "unsupported":return "当前微信环境不支持读取游戏圈进度";
            case "timeout":return "等待微信返回进度超时，请重新打开福利";
            default:return "微信暂未返回游戏圈进度，请稍后重试";
        }
    }
    [UnityEngine.Scripting.Preserve] public void OnPotBenefits(string json){
        var result=JsonUtility.FromJson<Evidence>(json);
        if(result!=null&&result.requestId==sequence)evidence=result;
    }
}
public sealed partial class Match3DInvitations {
    public IEnumerator PotBenefitRequest(PotBenefits.Evidence data,string owner,Action<Reply> done){
        while(Busy||logging)yield return null;
        if(!Ready||State.inviteCode!=owner)yield break;
        Busy=true;
        try{
            yield return Request("pot-benefits",JsonUtility.ToJson(data),r=>{
                Apply(r);done(r);
                if(data.claim>=0&&r.potBenefits?.granted!=null&&Match3DFlow.Instance)
                    Match3DFlow.Instance.ShowReceivedTools(r.potBenefits.granted,"开心收下");
            });
        }finally{Busy=false;Changed?.Invoke();}
    }
}
