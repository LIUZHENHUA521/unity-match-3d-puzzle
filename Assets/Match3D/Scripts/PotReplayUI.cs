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
    public static PotReplayProgress Progress(Transform parent,Vector2 position){
        // Preserve Photoshop artwork at its authored size, including the thumb's shadow.
        var root=Rect(parent,"Pot replay progress",position,new Vector2(640,81));
        root.gameObject.AddComponent<Image>().color=Color.clear;
        Art(root,"Track","PotReplayTrack",new Vector2(640,36));
        var fillArea=Rect(root,"Fill area",Vector2.zero,new Vector2(630,26));
        fillArea.anchorMin=fillArea.anchorMax=fillArea.pivot=new Vector2(.5f,.5f);
        var fill=Art(fillArea,"Fill","PotReplayFill",new Vector2(630,26));
        fill.rectTransform.sizeDelta=Vector2.zero;fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;
        var handleArea=Rect(root,"Handle area",Vector2.zero,new Vector2(630,81));
        handleArea.anchorMin=handleArea.anchorMax=handleArea.pivot=new Vector2(.5f,.5f);
        var handle=Art(handleArea,"Handle","PotReplayHandle",new Vector2(34,81));
        // Slider stretches the thumb vertically inside its movement area.
        handle.rectTransform.sizeDelta=new Vector2(34,0);
        var slider=root.gameObject.AddComponent<PotReplayProgress>();
        slider.fillRect=fill.rectTransform;slider.handleRect=handle.rectTransform;slider.targetGraphic=handle;
        slider.transition=Selectable.Transition.None;slider.navigation=new Navigation{mode=Navigation.Mode.None};
        slider.minValue=0;slider.maxValue=1;return slider;
    }
    static Image Art(Transform parent,string name,string asset,Vector2 size){
        var rect=Rect(parent,name,Vector2.zero,size);rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);
        var image=rect.gameObject.AddComponent<Image>();image.sprite=Resources.Load<Sprite>("Match3D/PotReplay/"+asset);
        if(!image.sprite)throw new InvalidOperationException("Missing pot replay art: "+asset);
        image.raycastTarget=false;return image;
    }
}
