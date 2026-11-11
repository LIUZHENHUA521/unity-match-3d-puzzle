using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public sealed class Match3DReviveView : MonoBehaviour {
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
        Match3DModalMotion.Play(gameObject);
    }
    public void ConfigureTool(int type,Action accept,Action decline){
        string[] names={"移出","吸附","洗牌","加时"};
        string[] icons={"image_btn_remove","image_btn_adsorpt","image_btn_shuffle","img_btn_clock"};
        string[] descriptions={"move-description","find-description","shuffle-description"};
        Configure(0,accept,decline);toolMode=true;progress.text=names[type]+"道具";
        animatedContent.Find("Panel").GetComponent<Image>().sprite=Resources.Load<Sprite>("Match3D/ReviveSkinPS/panel");
        progress.rectTransform.anchoredPosition=new Vector2(0,300);progress.rectTransform.sizeDelta=new Vector2(450,76);progress.fontSize=42;
        progress.gameObject.SetActive(false);
        // The revive panel includes its title; cover it with the matching blank ribbon.
        AddToolArtwork("Tool title ribbon",animatedContent,new Vector2(-1,297.5f),"title-ribbon");
        AddToolArtwork("Tool title",animatedContent,new Vector2(0,310),"title");
        status.rectTransform.anchoredPosition=new Vector2(0,-305);status.fontSize=20;
        var icon=animatedContent.Find("Icon").GetComponent<Image>();
        icon.sprite=Resources.Load<Sprite>("Match3D/GameplaySkin/"+icons[type]);icon.preserveAspect=true;icon.rectTransform.sizeDelta=new Vector2(150,160);
        AddToolArtwork("Tool icon background",animatedContent,icon.rectTransform.anchoredPosition,"icon-circle");
        var circle=animatedContent.Find("Tool icon background").GetComponent<Image>();
        circle.transform.SetSiblingIndex(icon.transform.GetSiblingIndex());
        var description=animatedContent.Find("Description");description.gameObject.SetActive(false);
        AddToolArtwork("Tool description",animatedContent,new Vector2(0,-65),type==BoardTools.AddTimeType?"add-time-description":descriptions[type]);
        var buttonImage=revive.GetComponent<Image>();buttonImage.sprite=Resources.Load<Sprite>("Match3D/SettingsSkinPS/image_btn_blue02");buttonImage.preserveAspect=true;
        buttonImage.rectTransform.anchoredPosition=new Vector2(0,-205);
        buttonImage.rectTransform.sizeDelta=new Vector2(300,300*buttonImage.sprite.rect.height/buttonImage.sprite.rect.width);
        AddToolText("Tool ad label",revive.transform,new Vector2(0,10),((RectTransform)revive.transform).sizeDelta,"看广告并使用",30,Color.white);
        StyleAcquisitionLabel(revive.transform.Find("Tool ad label").GetComponent<Text>());
        status.text="";status.gameObject.SetActive(false);
    }
    void AddToolArtwork(string name,Transform parent,Vector2 position,string asset){
        var sprite=Resources.Load<Sprite>("Match3D/ToolTextSkinPS/"+asset);
        if(!sprite)throw new InvalidOperationException("Missing tool text artwork: "+asset);
        var obj=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));obj.transform.SetParent(parent,false);
        var image=obj.GetComponent<Image>();image.sprite=sprite;image.preserveAspect=true;image.raycastTarget=false;
        var rect=image.rectTransform;rect.anchoredPosition=position;
        // Pixel dimensions map to the existing 720 x 1280 UI canvas, not the PSD's 1564px height.
        rect.sizeDelta=sprite.rect.size;
    }
    void AddToolText(string name,Transform parent,Vector2 position,Vector2 size,string value,int fontSize,Color color){
        var obj=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));obj.transform.SetParent(parent,false);
        var rect=(RectTransform)obj.transform;rect.sizeDelta=size;rect.anchoredPosition=position;
        var text=obj.GetComponent<Text>();text.font=progress.font;text.fontSize=fontSize;text.alignment=TextAnchor.MiddleCenter;text.color=color;text.text=value;text.raycastTarget=false;
    }
    static void StyleAcquisitionLabel(Text text){
        text.fontStyle=FontStyle.Bold;text.fontSize=32;text.color=Color.white;
        var outline=text.GetComponent<Outline>();if(!outline)outline=text.gameObject.AddComponent<Outline>();
        outline.effectColor=new Color32(35,122,195,255);outline.effectDistance=new Vector2(2,-2);outline.useGraphicAlpha=true;
    }
    public void ShowAdStatus(string message){if(toolMode){if(Match3DFlow.Instance)Match3DFlow.Instance.ShowTips(message);return;}if(status)status.text=message;}
    public void SetAcquisitionRoute(Match3DRewardAcquisition.Route route){
        string verb=route==Match3DRewardAcquisition.Route.Free?"免费":route==Match3DRewardAcquisition.Route.Share?"分享":route==Match3DRewardAcquisition.Route.Ad?"看广告":"暂不可用";
        var label=revive.transform.Find("Tool ad label")??revive.transform.Find("Acquisition label");
        if(!label){
            // Replace the baked video artwork so a share/free offer never displays an ad icon.
            var buttonImage=revive.GetComponent<Image>();buttonImage.sprite=Resources.Load<Sprite>("Match3D/SettingsSkinPS/image_btn_blue02");buttonImage.preserveAspect=true;
            buttonImage.rectTransform.sizeDelta=new Vector2(300,300*buttonImage.sprite.rect.height/buttonImage.sprite.rect.width);
            AddToolText("Acquisition label",revive.transform,new Vector2(0,10),buttonImage.rectTransform.sizeDelta,"",30,Color.white);
            label=revive.transform.Find("Acquisition label");
        }
        StyleAcquisitionLabel(label.GetComponent<Text>());
        label.GetComponent<Text>().text=route==Match3DRewardAcquisition.Route.Unavailable?verb:verb+(toolMode?(route==Match3DRewardAcquisition.Route.Free?"使用":"并使用"):"复活");
        revive.interactable=route!=Match3DRewardAcquisition.Route.Unavailable;
        if(!toolMode)status.text=route==Match3DRewardAcquisition.Route.Ad?"观看完整视频后复活":route==Match3DRewardAcquisition.Route.Share?"分享后返回游戏继续":route==Match3DRewardAcquisition.Route.Free?"本次可免费复活":"暂时无法获取，请稍后再试";
    }
    public void ShowAcquisitionStatus(Match3DRewardAcquisition.Route route){ShowAdStatus(route==Match3DRewardAcquisition.Route.Share?"正在打开分享…":route==Match3DRewardAcquisition.Route.Ad?"正在加载视频…":"正在使用…");}
    public void AllowRetry(string message){decided=false;group.interactable=true;ShowAdStatus(message);}
    void Decide(Action action){if(decided||!group.interactable)return;decided=true;group.interactable=false;if(Match3DFlow.Instance)Match3DFlow.Instance.HomeClick();action?.Invoke();}
}
