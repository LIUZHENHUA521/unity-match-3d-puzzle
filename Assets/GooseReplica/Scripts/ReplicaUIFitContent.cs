using UnityEngine;

// Fit a complete dialog/page on shorter displays without rewriting authored controls.
[RequireComponent(typeof(RectTransform))]
public sealed class ReplicaUIFitContent : MonoBehaviour {
    public RectTransform content;
    public void Fit(){
        if(!content)return;
        var available=((RectTransform)transform).rect.size;
        var design=content.rect.size;
        if(design.x<=0 || design.y<=0)return;
        float scale=Mathf.Min(1,Mathf.Min(available.x/design.x,available.y/design.y));
        content.localScale=Vector3.one*Mathf.Max(.001f,scale);
    }
    void LateUpdate(){Fit();}
}
