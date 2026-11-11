using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class Match3DSignInView : MonoBehaviour {
    [Serializable] public sealed class Card {
        public GameObject highlight,claimed;public Text tag,caption;
        public Image[] icons;public Text[] amounts;
    }
    public Card[] cards;public Sprite[] toolIcons;public Button claim;public Text buttonLabel,status;
    Match3DInvitations service;
    static readonly string[] Names={"移出","吸附","洗牌","加时"};
    public void Connect(Match3DInvitations value){
        service=value;claim.onClick.AddListener(Click);
        if(service){service.SignInChanged+=Refresh;service.Changed+=Refresh;service.RefreshSignIn();}Refresh();
    }
    void Click(){
        if(!service)return;
        if(Match3DFlow.Instance)Match3DFlow.Instance.HomeClick();
        if(service.SignInCurrent&&service.SignInState.canClaim)service.ClaimSignIn();else service.RefreshSignIn();
    }
    void Refresh(){
        var state=service?service.SignInState:null;
        bool current=service&&service.SignInCurrent,busy=service&&(service.SignInBusy||service.Busy);
        string label=busy?"请稍候…":!current?"刷新重试":state.canClaim?"立即签到":"今日已签到";
        string message=service?service.SignInStatus:"请在微信小游戏中登录后签到";
        if(string.IsNullOrEmpty(message))message=state!=null&&!state.canClaim?(state.claimedDays==7?"本轮已领完，明日开启新一轮":"今日奖励已到账，明天继续领取"):"累计签到，漏签不重置 · 每日零点刷新";
        Render(state??Match3DSignInState.Preview(),label,message,current);
        claim.interactable=service&&!busy&&(!current||state.canClaim);
    }
    public void Render(Match3DSignInState state,string label,string message,bool current=true){
        if(state?.days==null||state.days.Length!=7)return;
        buttonLabel.text=label;status.text=message;
        for(int i=0;i<cards.Length;i++){
            var card=cards[i];bool done=i<state.claimedDays,today=current&&state.canClaim&&i==state.day-1;
            card.highlight.SetActive(today);card.claimed.SetActive(done);
            card.tag.text=done?"已领取":today?"今日可领":"待解锁";
            card.tag.color=today?new Color32(204,112,20,255):new Color32(111,136,159,255);
            var quantities=state.days[i].rewards;int count=0;foreach(int n in quantities)if(n>0)count++;
            int slot=0;var names=new System.Collections.Generic.List<string>();
            for(int t=0;t<Math.Min(4,quantities.Length);t++)if(quantities[t]>0){
                var icon=card.icons[slot];icon.gameObject.SetActive(true);icon.sprite=toolIcons[t];
                float step=i==6?128:80,size=i==6?88:count==1?92:68;
                icon.rectTransform.sizeDelta=new Vector2(size,size);
                icon.rectTransform.anchoredPosition=new Vector2((slot-(count-1)*.5f)*step,-9);
                icon.color=new Color(1,1,1,current&&!done&&!today?.7f:1);
                card.amounts[slot].gameObject.SetActive(i==6);
                card.amounts[slot].rectTransform.anchoredPosition=new Vector2(icon.rectTransform.anchoredPosition.x,-72);
                card.amounts[slot].text=Names[t]+"×"+quantities[t];
                names.Add(Names[t]+"×"+quantities[t]);slot++;
            }
            for(int j=slot;j<card.icons.Length;j++){card.icons[j].gameObject.SetActive(false);card.amounts[j].gameObject.SetActive(false);}
            card.caption.text=i==6?"":string.Join("  ",names);card.caption.fontSize=count>1?18:23;
        }
    }
    void OnDestroy(){if(service){service.SignInChanged-=Refresh;service.Changed-=Refresh;}}
}
