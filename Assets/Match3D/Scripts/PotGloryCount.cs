using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Original Photoshop-rendered glyphs retain the gradient, multiple strokes and shadow.
// Native pixel sizes/baselines are preserved, with fitting only for unusually long totals.
public sealed class PotGloryCount : MonoBehaviour {
    public Image prefix,suffix;
    public Sprite[] digits;
    public int Value {get;private set;}
    readonly List<Image> glyphs=new List<Image>();
    static readonly int[] Top={0,1,0,0,1,1,1,1,1,0};
    public void SetCount(int value){
        Value=Math.Max(0,value);
        // Serialized preview children already exist after prefab instantiation/domain reload.
        if(glyphs.Count==0)foreach(Transform child in transform){var i=child.GetComponent<Image>();if(i&&i!=prefix&&i!=suffix)glyphs.Add(i);}
        string text=Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        while(glyphs.Count<text.Length){
            var rect=new GameObject("PSD digit",typeof(RectTransform),typeof(Image)).GetComponent<RectTransform>();rect.SetParent(transform,false);
            rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);
            var image=rect.GetComponent<Image>();image.raycastTarget=false;glyphs.Add(image);
        }
        float x=114; // Original prefix width 105 and 9px gap before the numeral.
        for(int i=0;i<glyphs.Count;i++){
            var image=glyphs[i];image.gameObject.SetActive(i<text.Length);if(i>=text.Length)continue;
            int n=text[i]-'0';image.sprite=digits[n];image.rectTransform.sizeDelta=digits[n].rect.size;
            image.rectTransform.anchoredPosition=new Vector2(x,-Top[n]);x+=digits[n].rect.width-12;
        }
        x+=16; // Glyph effect bounds overlap by 12px; leave 4px before the suffix.
        suffix.rectTransform.anchoredPosition=new Vector2(x,-7);
        float width=x+181;((RectTransform)transform).sizeDelta=new Vector2(width,60);
        transform.localScale=Vector3.one*Mathf.Min(1,390/width);
    }
}
