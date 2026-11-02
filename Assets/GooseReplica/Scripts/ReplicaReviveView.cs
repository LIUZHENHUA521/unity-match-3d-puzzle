using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public sealed class ReplicaReviveView : MonoBehaviour {
    public RectTransform animatedContent;
    public CanvasGroup group;
    public Image shade;
    public Text progress,status;
    public Button revive,close;
    bool decided,toolMode;
    public void Configure(float value,Action accept,Action decline){
        progress.text="关卡已完成："+Mathf.FloorToInt(Mathf.Clamp01(value)*100)+"%";
        decided=false;revive.onClick.RemoveAllListeners();close.onClick.RemoveAllListeners();
        revive.onClick.AddListener(()=>Decide(accept));close.onClick.AddListener(()=>Decide(decline));
        ReplicaModalMotion.Play(gameObject);
    }
    public void ConfigureTool(int type,Action accept,Action decline){
        string[] names={"移出","吸附","洗牌"};
        string[] icons={"image_btn_remove","image_btn_adsorpt","image_btn_shuffle"};
        string[] descriptions={"将槽位中最前面的 3 个物品\n移到暂存区，腾出槽位", "自动寻找同类物品吸入槽位\n帮你凑齐一组三消", "重新打乱场上物品的位置\n寻找新的消除机会"};
        Configure(0,accept,decline);toolMode=true;progress.text=names[type]+"道具";
        animatedContent.Find("Panel").GetComponent<Image>().sprite=Resources.Load<Sprite>("Replica/ProfileSkinPS/image_User_bg");
        progress.rectTransform.anchoredPosition=new Vector2(0,300);progress.rectTransform.sizeDelta=new Vector2(450,76);progress.fontSize=42;
        status.rectTransform.anchoredPosition=new Vector2(0,-305);status.fontSize=20;
        var icon=animatedContent.Find("Icon").GetComponent<Image>();
        icon.sprite=Resources.Load<Sprite>("Replica/GameplaySkin/"+icons[type]);icon.preserveAspect=true;icon.rectTransform.sizeDelta=new Vector2(150,160);
        var description=animatedContent.Find("Description");description.gameObject.SetActive(false);
        AddToolText("Tool description",animatedContent,new Vector2(0,-65),new Vector2(520,100),descriptions[type],30,new Color32(30,112,173,255));
        var buttonImage=revive.GetComponent<Image>();buttonImage.sprite=Resources.Load<Sprite>("Replica/SettingsSkinPS/image_btn_blue02");buttonImage.preserveAspect=true;
        buttonImage.rectTransform.anchoredPosition=new Vector2(0,-205);
        buttonImage.rectTransform.sizeDelta=new Vector2(300,300*buttonImage.sprite.rect.height/buttonImage.sprite.rect.width);
        AddToolText("Tool ad label",revive.transform,new Vector2(0,10),((RectTransform)revive.transform).sizeDelta,"看广告并使用",30,Color.white);
        var buttonLabel=revive.transform.Find("Tool ad label").GetComponent<Text>();buttonLabel.fontStyle=FontStyle.Bold;
        var outline=buttonLabel.gameObject.AddComponent<Outline>();outline.effectColor=new Color(0,.32f,.55f,.8f);outline.effectDistance=new Vector2(1,-1);
        status.text="";status.gameObject.SetActive(false);
    }
    void AddToolText(string name,Transform parent,Vector2 position,Vector2 size,string value,int fontSize,Color color){
        var obj=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));obj.transform.SetParent(parent,false);
        var rect=(RectTransform)obj.transform;rect.sizeDelta=size;rect.anchoredPosition=position;
        var text=obj.GetComponent<Text>();text.font=progress.font;text.fontSize=fontSize;text.alignment=TextAnchor.MiddleCenter;text.color=color;text.text=value;text.raycastTarget=false;
    }
    public void ShowAdStatus(string message){if(toolMode){if(ReplicaFlow.Instance)ReplicaFlow.Instance.ShowTips(message);return;}if(status)status.text=message;}
    public void AllowRetry(string message){decided=false;group.interactable=true;ShowAdStatus(message);}
    void Decide(Action action){if(decided||!group.interactable)return;decided=true;group.interactable=false;if(ReplicaFlow.Instance)ReplicaFlow.Instance.HomeClick();action?.Invoke();}
}
