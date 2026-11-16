using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// All motion uses unscaled time; layout fitting owns a separate parent transform.
public sealed class PotCertificateView : MonoBehaviour {
    public RectTransform card,banner,stamp,footer,actions;
    public CanvasGroup mainGroup,bannerGroup,stampGroup,footerGroup,actionsGroup;
    public Text title,mode,duration,nickname,date,number,status,buttonLabel;
    public Button claim,home;
    public Image stampImage,shade;
    public Sprite[] bachelorStamps,eliteStamps;
    public AudioClip stampSound;
    bool stampPlayed;
    public int StampSoundCount {get;private set;}
    public bool Playing {get;private set;}
    public bool Confirmed {get;private set;}
    Vector2 cardPos,bannerPos,stampPos,footerPos,actionsPos;
    Action confirm;
    string collectionOwner;Action leaveCollection;
    public void GuardCollection(string owner,Action leave){collectionOwner=owner;leaveCollection=leave;}
    void Update(){
        if(string.IsNullOrEmpty(collectionOwner))return;
        var account=Match3DInvitations.Instance;
        if(!account||!account.Ready||account.State.inviteCode!=collectionOwner){collectionOwner=null;leaveCollection?.Invoke();}
    }
    public void Configure(PotCertificateData data,Action onConfirm,Action onHome){
        confirm=onConfirm;Confirmed=false;stampPlayed=false;StampSoundCount=0;
        cardPos=card.anchoredPosition;bannerPos=banner.anchoredPosition;stampPos=stamp.anchoredPosition;
        footerPos=footer.anchoredPosition;actionsPos=actions.anchoredPosition;
        Apply(data);status.text="通关记录同步中…";
        claim.onClick.AddListener(()=>{
            if(Playing){Skip();return;}if(Confirmed)return;
            if(Match3DFlow.Instance)Match3DFlow.Instance.HomeClick();
            Confirmed=true;claim.interactable=false;buttonLabel.text="正在确认…";confirm?.Invoke();
        });
        home.onClick.AddListener(()=>{if(Match3DFlow.Instance)Match3DFlow.Instance.HomeClick();onHome?.Invoke();});
        StartCoroutine(Enter());
    }
    public void Apply(PotCertificateData data){
        var pool=data.rewardTier=="elite"?eliteStamps:bachelorStamps;
        uint seed=0;foreach(char c in data.recordId??"")seed=unchecked(seed*31+c);
        stampImage.sprite=pool[seed%(uint)pool.Length];stampImage.preserveAspect=true;

        title.text="一锅消不完 · 通关奖状";mode.text=data.rewardTier=="elite"?"清北局毕业荣誉":"本科局毕业荣誉";
        long seconds=Math.Max(0,data.activeDurationMs/1000);duration.text=seconds/60+"分"+seconds%60+"秒";
        nickname.text=string.IsNullOrWhiteSpace(data.nickName)?"微信玩家":data.nickName;
        date.text=(data.dateKey??"").Replace('-','.');number.text=string.IsNullOrEmpty(data.certificateNo)?"待同步":data.certificateNo;
    }
    public void Message(string text,bool terminal=false){status.text=text;if(Confirmed&&Match3DFlow.Instance)Match3DFlow.Instance.Notice(text);if(terminal){buttonLabel.text="已完成";claim.interactable=false;}}
    public void Skip(){StopAllCoroutines();PlayStamp();Sample(2);Playing=false;}
    void PlayStamp(){if(stampPlayed)return;stampPlayed=true;var flow=Match3DFlow.Instance;if(flow&&flow.SettingsEffects&&stampSound){flow.PlayPotCertificateSound(stampSound);StampSoundCount++;}}
    IEnumerator Enter(){Playing=true;float elapsed=0;Sample(0);while(elapsed<1.6f){yield return null;elapsed+=Time.unscaledDeltaTime;if(elapsed>=1)PlayStamp();Sample(elapsed);}Sample(2);Playing=false;}
    static float Phase(float time,float start,float length)=>Mathf.Clamp01((time-start)/length);
    static float Back(float t){float u=t-1;return 1+2.70158f*u*u*u+1.70158f*u*u;}
    public void Sample(float time){
        float entrance=Back(Phase(time,0,.35f));card.anchoredPosition=cardPos+Vector2.down*260*(1-entrance);
        float impact=Phase(time,1.0f,.18f);float squeeze=time>=1&&time<1.18f?1-.015f*Mathf.Sin(impact*Mathf.PI):1;
        card.localScale=Vector3.one*Mathf.LerpUnclamped(.96f,1,entrance)*squeeze;
        shade.color=new Color(0,0,0,.64f*Phase(time,0,.22f));mainGroup.alpha=Phase(time,.35f,.25f);
        float ribbon=Back(Phase(time,.35f,.32f));bannerGroup.alpha=Phase(time,.35f,.18f);
        banner.anchoredPosition=bannerPos+Vector2.up*100*(1-ribbon);banner.localScale=Vector3.one*Mathf.LerpUnclamped(1.2f,1,ribbon);
        footerGroup.alpha=Phase(time,.45f,.28f);footer.anchoredPosition=footerPos+Vector2.down*25*(1-Back(Phase(time,.45f,.32f)));
        float seal=Phase(time,.65f,.35f);float fall=seal*seal*seal;stampGroup.alpha=Phase(time,.65f,.08f);
        stamp.anchoredPosition=stampPos+new Vector2(180,180)*(1-fall);
        float sealScale=time>=1&&time<1.22f?1-.08f*Mathf.Sin(Phase(time,1,.22f)*Mathf.PI):Mathf.Lerp(1.2f,1,fall);
        stamp.localScale=Vector3.one*sealScale;stamp.localRotation=Quaternion.Euler(0,0,Mathf.Lerp(-22,-12,fall));
        actionsGroup.alpha=Phase(time,1.14f,.2f);actions.anchoredPosition=actionsPos+Vector2.down*40*(1-Back(Phase(time,1.14f,.2f)));
    }
}
