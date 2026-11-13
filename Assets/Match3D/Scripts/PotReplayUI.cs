using System;
using UnityEngine;
using UnityEngine.UI;
public static class PotReplayUI {
    public static RectTransform Rect(Transform parent,string name,Vector2 position,Vector2 size){
        var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=new Vector2(.5f,1);r.pivot=new Vector2(.5f,1);r.anchoredPosition=position;r.sizeDelta=size;return r;
    }
    public static Text Label(Transform parent,string title,Vector2 position,Vector2 size){var r=Rect(parent,"Pot replay text",position,size);var t=r.gameObject.AddComponent<Text>();t.font=Match3DDefaultFont.Current;t.fontSize=25;t.alignment=TextAnchor.MiddleCenter;t.color=new Color(.15f,.2f,.16f);t.text=title;t.raycastTarget=false;return t;}
    public static Button Button(Transform parent,string title,Vector2 position,Action click){
        var r=Rect(parent,"Pot replay button",position,new Vector2(175,60));r.gameObject.AddComponent<Image>().color=new Color(.82f,.91f,.78f);var b=r.gameObject.AddComponent<Button>();b.onClick.AddListener(()=>click());Label(r,title,Vector2.zero,new Vector2(175,60));return b;
    }
    public static Button SpeedButton(Transform parent,Vector2 position,Action click){
        var sprite=Resources.Load<Sprite>("Match3D/PotCameraZoomSkin/img_whiteLine_cube02");
        if(!sprite)throw new InvalidOperationException("Missing pot replay speed button art");
        var r=Rect(parent,"Pot replay speed",position,sprite.rect.size);
        var image=r.gameObject.AddComponent<Image>();image.sprite=sprite;image.color=Color.white;
        var button=r.gameObject.AddComponent<Button>();button.targetGraphic=image;
        button.navigation=new Navigation{mode=Navigation.Mode.None};
        button.onClick.AddListener(()=>{Match3DFlow.Instance?.PotVibrate();click();});
        var label=Label(r,"1×",Vector2.zero,sprite.rect.size);
        label.fontSize=32;label.fontStyle=FontStyle.Bold;label.color=Color.white;
        return button;
    }
    public static Slider Progress(Transform parent,Vector2 position){
        var r=Rect(parent,"Pot replay progress",position,new Vector2(570,35));r.gameObject.AddComponent<Image>().color=new Color(.65f,.72f,.65f);var s=r.gameObject.AddComponent<Slider>();
        var handle=Rect(r,"Handle",Vector2.zero,new Vector2(25,35));handle.gameObject.AddComponent<Image>().color=new Color(.15f,.5f,.25f);s.handleRect=handle;s.targetGraphic=handle.GetComponent<Image>();s.minValue=0;s.maxValue=1;return s;
    }
}
