using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Authored at 720 x 1280. The fitter owns the outer transform; animation owns the inner one.
public sealed class ReplicaResultView : MonoBehaviour {
    public RectTransform animatedContent,marker;
    public CanvasGroup contentGroup;
    public Image shade,fill;
    public Text attempts,best;
    public Button primary,secondary,share;
    public void Configure(int count,float progress,float record,Action main,Action other,Action onShare){
        attempts.text="今日已挑战"+count+"次";
        best.text=Mathf.RoundToInt(record*100)+"%";
        primary.onClick.RemoveAllListeners();secondary.onClick.RemoveAllListeners();share.onClick.RemoveAllListeners();
        primary.onClick.AddListener(()=>{Click();main?.Invoke();});
        secondary.onClick.AddListener(()=>{Click();other?.Invoke();});
        share.onClick.AddListener(()=>{Click();onShare?.Invoke();});
        ReplicaModalMotion.Play(gameObject);StopAllCoroutines();StartCoroutine(AnimateProgress(Mathf.Clamp01(progress)));
    }
    static void Click(){if(ReplicaFlow.Instance)ReplicaFlow.Instance.HomeClick();}
    IEnumerator AnimateProgress(float target){
        float elapsed=0;
        while(elapsed<.75f){
            elapsed+=Time.unscaledDeltaTime;
            float p=target*(1-Mathf.Pow(1-Mathf.Clamp01((elapsed-.16f)/.59f),3));
            fill.fillAmount=p;marker.anchoredPosition=new Vector2(Mathf.Lerp(-290,290,p),-140);
            yield return null;
        }
        fill.fillAmount=target;
        marker.anchoredPosition=new Vector2(Mathf.Lerp(-290,290,target),-140);

    }
}
