using UnityEngine;
using UnityEngine.UI;

// Independent Photoshop layers preserve the original artwork while the lower row changes state.
public sealed class PotHomeReplayButton {
    public readonly Button button;
    readonly Image packet,reward;
    readonly Text caption;
    public PotHomeReplayButton(Button target,bool elite){
        button=target;var root=(RectTransform)target.transform;
        foreach(Transform child in root)child.gameObject.SetActive(false);
        target.image.color=Color.clear;
        string tier=elite?"Elite":"Bachelor";
        var background=Layer(root,tier+"Base",0,elite?3:5);
        Layer(root,tier+"Title",elite?97:91,elite?1:6);
        packet=Layer(root,tier+"Packet",47,91);
        reward=Layer(root,tier+"Reward",98,elite?97:99);
        target.targetGraphic=background;
        var rect=new GameObject("Pot replay caption",typeof(RectTransform)).GetComponent<RectTransform>();rect.SetParent(root,false);
        rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.anchoredPosition=new Vector2(40,-88);rect.sizeDelta=new Vector2(root.rect.width-80,62);
        caption=rect.gameObject.AddComponent<Text>();caption.font=Match3DDefaultFont.Current;caption.fontStyle=FontStyle.Bold;caption.fontSize=34;
        caption.alignment=TextAnchor.MiddleCenter;caption.color=new Color(1,.97f,.72f);caption.raycastTarget=false;
        var outline=rect.gameObject.AddComponent<Outline>();outline.effectColor=elite?new Color(.62f,.25f,.03f):new Color(0,.3f,.52f);outline.effectDistance=new Vector2(2,-2);
        Render(false,null);
    }
    static Image Layer(RectTransform parent,string part,float x,float y){
        var sprite=Resources.Load<Sprite>("Match3D/PotHomeReplaySkin/PotHome"+part);
        if(!sprite)throw new System.InvalidOperationException("Missing pot home art: "+part);
        var rect=new GameObject("Pot home "+part,typeof(RectTransform),typeof(Image)).GetComponent<RectTransform>();rect.SetParent(parent,false);
        rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=sprite.rect.size;
        var image=rect.GetComponent<Image>();image.sprite=sprite;image.raycastTarget=false;return image;
    }
    public void Render(bool won,string replayId){
        bool ready=!string.IsNullOrEmpty(replayId),normal=!won&&!ready;
        packet.gameObject.SetActive(normal);reward.gameObject.SetActive(normal);caption.gameObject.SetActive(!normal);
        caption.text=ready?"查看回放":"回放待同步";
    }
}
