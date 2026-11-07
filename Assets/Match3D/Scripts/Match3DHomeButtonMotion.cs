using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Owns both entrance and press scale so the two animations cannot fight each other.
public sealed class Match3DHomeButtonMotion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
    RectTransform rect;
    Button button;
    CanvasGroup group;
    Vector2 restingPosition;
    Vector3 restingScale;
    float restingAlpha, delay, elapsed, pressScale=1;
    bool primary, entering, pressed, restingRaycasts;

    public static void Attach(Button target,float delay,bool primary){
        if(!target)return;
        var motion=target.gameObject.AddComponent<Match3DHomeButtonMotion>();
        motion.button=target;motion.rect=(RectTransform)target.transform;
        motion.group=target.GetComponent<CanvasGroup>();
        if(!motion.group)motion.group=target.gameObject.AddComponent<CanvasGroup>();
        motion.restingPosition=motion.rect.anchoredPosition;
        motion.restingScale=motion.rect.localScale;
        motion.restingAlpha=motion.group.alpha;
        motion.restingRaycasts=motion.group.blocksRaycasts;
        motion.delay=delay;motion.primary=primary;motion.entering=true;
        motion.group.blocksRaycasts=false;
        motion.ApplyEntrance(0);
    }

    void Update(){
        if(!rect)return;
        if(entering){
            elapsed+=Time.unscaledDeltaTime;
            float duration=primary?.4f:.24f;
            float t=Mathf.Clamp01((elapsed-delay)/duration);
            ApplyEntrance(t);
            if(t>=1){entering=false;Restore();}
            return;
        }
        if(!primary)return;
        if(!button.IsInteractable())pressed=false;
        pressScale=Mathf.MoveTowards(pressScale,pressed?.96f:1,Time.unscaledDeltaTime*.5f);
        rect.localScale=restingScale*pressScale;
    }

    void ApplyEntrance(float t){
        group.alpha=restingAlpha*Mathf.Clamp01(t*2);
        if(!primary)return;
        float ease=1-Mathf.Pow(1-t,3);
        rect.anchoredPosition=restingPosition+Vector2.down*(36*(1-ease));
        float scale=t<.7f?Mathf.Lerp(.92f,1.03f,1-Mathf.Pow(1-t/.7f,3))
            :Mathf.Lerp(1.03f,1,Mathf.SmoothStep(0,1,(t-.7f)/.3f));
        rect.localScale=restingScale*scale;
    }

    public void OnPointerDown(PointerEventData e){
        if(e.button==PointerEventData.InputButton.Left&&!entering&&button.IsInteractable())pressed=true;
    }
    public void OnPointerUp(PointerEventData e){pressed=false;}
    public void OnPointerExit(PointerEventData e){pressed=false;}
    void OnApplicationFocus(bool focus){if(!focus)pressed=false;}
    void OnDisable(){entering=false;pressed=false;pressScale=1;Restore();}
    void Restore(){
        if(rect){rect.anchoredPosition=restingPosition;rect.localScale=restingScale;}
        if(group){group.alpha=restingAlpha;group.blocksRaycasts=restingRaycasts;}
    }
}
