using System;
using UnityEngine;
using UnityEngine.UI;

// Presentation only. Entry points stay in ReplicaFlow so home art can change independently.
public sealed class ReplicaHomeView : MonoBehaviour {
    public const string BachelorReward="2", EliteReward="10";
    public enum Tab { Home, Collection, Pets }
    ReplicaFlow flow;
    RectTransform safe,content,bar,bachelor,elite;
    Rect lastSafe;
    Vector2 lastSize;
    public static ReplicaHomeView Create(ReplicaFlow flow,RectTransform parent,Tab tab){
        var root=new GameObject("Reskinned Home",typeof(RectTransform));
        var rect=root.GetComponent<RectTransform>();rect.SetParent(parent,false);Fill(rect);
        var view=root.AddComponent<ReplicaHomeView>();view.flow=flow;view.Build(tab);view.Layout();return view;
    }
    void Build(Tab selected){
        var backdrop=Picture(transform,"homeBackground",Vector2.zero,new Vector2(720,1475),false);
        Fill(backdrop.rectTransform);
        var fit=backdrop.gameObject.AddComponent<AspectRatioFitter>();
        fit.aspectRatio=720f/1475;fit.aspectMode=AspectRatioFitter.AspectMode.EnvelopeParent;
        safe=Rect("Safe content",transform,Vector2.zero,Vector2.zero);Fill(safe);
        content=Rect("Content",safe,Vector2.zero,Vector2.zero);Fill(content);content.offsetMin=new Vector2(0,160);
        if(selected==Tab.Home){
            var settings=ActionButton(content,"Settings","image_setting",new Vector2(62,-66),new Vector2(76,76),()=>flow.ShowSettings(false));
            Anchor(settings,new Vector2(0,1));
            var reward=ActionButton(content,"Rewards","",new Vector2(222,-66),new Vector2(210,76),flow.ShowRewardInfo);
            Anchor(reward,new Vector2(0,1));reward.GetComponent<Image>().color=new Color(.36f,.16f,.13f,.4f);
            Picture(reward,"redPacket",new Vector2(32,-38),new Vector2(48,58));
            Text(reward,"红包奖励",new Vector2(132,38),new Vector2(145,60),28,Color.white,new Color(.48f,.23f,.12f));
            bachelor=Entry("Bachelor","本科局",BachelorReward,"easyModeBtn",new Color(.12f,.38f,.65f),()=>flow.Begin(0));
            elite=Entry("Elite","清北局",EliteReward,"challengeBtn",new Color(.75f,.27f,.04f),flow.ShowThemes);
        }else{
            var panel=Rect("Placeholder",content,Vector2.zero,new Vector2(590,380));Anchor(panel,new Vector2(.5f,.5f));
            panel.gameObject.AddComponent<Image>().color=new Color(1,.97f,.88f,.95f);
            Picture(panel,selected==Tab.Collection?"image_menu_collect":"image_menu_pet",new Vector2(295,-100),new Vector2(132,122));
            Text(panel,selected==Tab.Collection?"收藏":"宠物",new Vector2(295,212),new Vector2(500,64),43,new Color(.43f,.26f,.17f));
            Text(panel,"即将开放",new Vector2(295,295),new Vector2(500,60),29,new Color(.65f,.45f,.30f));
        }
        // Full-width color extends below the device's home indicator; controls stay inside safe area.
        var footer=Rect("Footer background",transform,Vector2.zero,Vector2.zero);
        footer.anchorMin=Vector2.zero;footer.anchorMax=new Vector2(1,0);footer.pivot=new Vector2(.5f,0);
        footer.gameObject.AddComponent<Image>().color=new Color(.15f,.48f,.90f);
        bar=Rect("Bottom tabs",safe,Vector2.zero,new Vector2(0,160));bar.anchorMin=Vector2.zero;bar.anchorMax=new Vector2(1,0);bar.pivot=new Vector2(.5f,0);
        bar.SetAsLastSibling();safe.SetAsLastSibling();
        AddTab(Tab.Home,"主页","image_menu_home",selected,0);
        AddTab(Tab.Collection,"收藏","image_menu_collect",selected,1);
        AddTab(Tab.Pets,"宠物","image_menu_pet",selected,2);
        var line=Picture(bar,"image_tab_blue_cube",Vector2.zero,new Vector2(720,7),false);
        line.rectTransform.anchorMin=new Vector2(0,1);line.rectTransform.anchorMax=Vector2.one;line.rectTransform.sizeDelta=new Vector2(0,7);line.rectTransform.anchoredPosition=Vector2.zero;
    }
    RectTransform Entry(string name,string title,string amount,string sprite,Color stroke,Action action){
        var button=ActionButton(content,name,sprite,Vector2.zero,new Vector2(454,190),action);
        Anchor(button,new Vector2(.5f,0));
        Text(button,title,new Vector2(227,58),new Vector2(400,62),43,Color.white,stroke);
        Picture(button,"redPacket",new Vector2(68,-127),new Vector2(48,57));
        Text(button,"毕业可领",new Vector2(162,128),new Vector2(150,55),27,new Color(1,1,.60f),stroke);
        Text(button,amount,new Vector2(267,125),new Vector2(78,67),48,new Color(1,.30f,.12f),Color.white);
        Text(button,"元奖励",new Vector2(360,128),new Vector2(128,55),27,new Color(1,1,.60f),stroke);
        return button;
    }
    void AddTab(Tab tab,string title,string sprite,Tab selected,int index){
        bool active=tab==selected;
        var r=ActionButton(bar,"Tab "+tab,active?"btn_menu_selected":"btn_menu_normal",Vector2.zero,Vector2.zero,()=>flow.ShowHomeTab(tab));
        // Preserve the source edge width when widening five-tab art into three tabs.
        // Half-scale borders keep neighbouring edges from forming a thick dark band.
        var background=r.GetComponent<Image>();background.type=Image.Type.Sliced;
        background.pixelsPerUnitMultiplier=2f;
        r.anchorMin=new Vector2(index/3f,0);r.anchorMax=new Vector2((index+1)/3f,1);r.offsetMin=r.offsetMax=Vector2.zero;
        var icon=Picture(r,sprite,new Vector2(120,-62),new Vector2(active?110:98,active?100:90));
        icon.rectTransform.anchorMin=icon.rectTransform.anchorMax=new Vector2(.5f,1);icon.rectTransform.anchoredPosition=new Vector2(0,-62);
        var label=Text(r,title,new Vector2(120,130),new Vector2(230,48),active?34:30,Color.white,new Color(.08f,.31f,.50f));
        label.rectTransform.anchorMin=label.rectTransform.anchorMax=new Vector2(.5f,1);label.rectTransform.anchoredPosition=new Vector2(0,-130);
    }
    void LateUpdate(){
        var size=((RectTransform)transform).rect.size;
        if(size!=lastSize||Screen.safeArea!=lastSafe)Layout();
    }
    void Layout(){
        var root=(RectTransform)transform;lastSize=root.rect.size;lastSafe=Screen.safeArea;
        float scale=lastSize.x/Mathf.Max(1,Screen.width);
        float bottom=lastSafe.yMin*scale,top=(Screen.height-lastSafe.yMax)*scale;
        safe.offsetMin=new Vector2(lastSafe.xMin*scale,bottom);
        safe.offsetMax=new Vector2(-(Screen.width-lastSafe.xMax)*scale,-top);
        ((RectTransform)transform.Find("Footer background")).sizeDelta=new Vector2(0,160+bottom);
        float available=lastSize.y-top-bottom-160;
        if(bachelor&&elite){
            float buttonScale=available<1000?.82f:1f;
            float lower=available<1000?Mathf.Clamp(available*.13f,100,180):Mathf.Clamp(available*.16f,112,255);
            bachelor.localScale=elite.localScale=Vector3.one*buttonScale;
            elite.anchoredPosition=new Vector2(0,lower);
            bachelor.anchoredPosition=new Vector2(0,lower+205*buttonScale);
        }
    }
    RectTransform ActionButton(Transform parent,string name,string sprite,Vector2 position,Vector2 size,Action action){
        var im=Picture(parent,sprite,position,size,false);im.name=name;im.raycastTarget=true;
        var b=im.gameObject.AddComponent<Button>();b.targetGraphic=im;
        b.onClick.AddListener(()=>{flow.HomeClick();action();});return im.rectTransform;
    }
    Image Picture(Transform parent,string sprite,Vector2 position,Vector2 size,bool icon=true){
        var r=Rect(sprite,parent,position,size);var im=r.gameObject.AddComponent<Image>();
        if(sprite.Length>0)im.sprite=Resources.Load<Sprite>("Replica/HomeSkin/"+(sprite=="homeBackground"?"":"Icons/")+sprite);
        im.preserveAspect=icon;im.raycastTarget=false;return im;
    }
    Text Text(Transform parent,string copy,Vector2 pos,Vector2 size,int fontSize,Color color,Color? outline=null){
        var text=flow.Label(parent,copy,pos,size,fontSize,color,outline.HasValue);
        if(fontSize>=30)text.fontStyle=FontStyle.Bold;
        if(outline.HasValue){var stroke=text.GetComponent<Outline>();stroke.effectColor=outline.Value;stroke.effectDistance=new Vector2(2,-2);}
        return text;
    }
    static RectTransform Rect(string name,Transform parent,Vector2 pos,Vector2 size){
        var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);
        r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;return r;
    }
    static void Anchor(RectTransform r,Vector2 anchor){r.anchorMin=r.anchorMax=anchor;}
    static void Fill(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
}
