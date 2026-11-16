using UnityEngine;

// The diploma fills the available width. Animation owns the child, layout owns its parent.
public sealed class PotCertificateLayout : MonoBehaviour {
    public RectTransform diploma,actions;
    public void Fit(){
        var size=((RectTransform)transform).rect.size;if(size.x<=0||size.y<=0)return;
        float scale=size.x/590f,height=431*scale;
        const float center=0;
        diploma.localScale=Vector3.one*scale;diploma.anchoredPosition=new Vector2(0,center);
        // Keep the main button on screen without shrinking the diploma on tablets.
        float buttonScale=Mathf.Min(1,size.x/720f);
        actions.localScale=Vector3.one*buttonScale;
        actions.anchoredPosition=new Vector2(0,Mathf.Max(-size.y*.5f+110,center-height*.5f-115));
    }
    void LateUpdate(){Fit();}
}
