using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// One entrance animation for every modal. Layout fitting remains on a separate transform.
public sealed class Match3DModalMotion : MonoBehaviour {
    public const float Duration=.42f;
    RectTransform content;CanvasGroup group;Image shade;
    public bool Playing {get;private set;}
    public static void Play(GameObject root){
        var motion=root.GetComponent<Match3DModalMotion>();
        if(!motion){motion=root.AddComponent<Match3DModalMotion>();motion.Initialize();}
        if(motion.Playing)return;
        motion.StartCoroutine(motion.Enter());
    }
    void Initialize(){
        var revival=GetComponent<Match3DReviveView>();var result=GetComponent<Match3DResultView>();
        if(revival){content=revival.animatedContent;group=revival.group;shade=revival.shade;}
        else if(result){content=result.animatedContent;group=result.contentGroup;shade=result.shade;}
        else{
            shade=GetComponent<Image>();
            // Wrap the full existing layout so fitters and serialized bindings keep working.
            var children=new Transform[transform.childCount];for(int i=0;i<children.Length;i++)children[i]=transform.GetChild(i);
            content=new GameObject("Modal motion",typeof(RectTransform),typeof(CanvasGroup)).GetComponent<RectTransform>();
            content.SetParent(transform,false);content.anchorMin=Vector2.zero;content.anchorMax=Vector2.one;content.offsetMin=content.offsetMax=Vector2.zero;
            foreach(var child in children)child.SetParent(content,false);
            group=content.GetComponent<CanvasGroup>();
        }
    }
    IEnumerator Enter(){
        Playing=true;group.interactable=false;content.localScale=Vector3.one*.78f;group.alpha=0;
        if(shade)shade.color=new Color(0,0,0,0);
        float elapsed=0;
        while(elapsed<Duration){
            yield return null;elapsed+=Time.unscaledDeltaTime;
            float t=Mathf.Clamp01(elapsed/Duration),u=t-1;
            content.localScale=Vector3.one*Mathf.LerpUnclamped(.78f,1,1+2.70158f*u*u*u+1.70158f*u*u);
            group.alpha=Mathf.Clamp01(elapsed/.18f);
            if(shade)shade.color=new Color(0,0,0,.64f*Mathf.Clamp01(elapsed/.22f));
        }
        content.localScale=Vector3.one;group.alpha=1;group.interactable=true;Playing=false;
    }
}
