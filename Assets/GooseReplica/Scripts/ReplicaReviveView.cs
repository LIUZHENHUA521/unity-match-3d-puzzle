using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public sealed class ReplicaReviveView : MonoBehaviour {
    public RectTransform animatedContent;
    public CanvasGroup group;
    public Image shade;
    public Text progress,status;
    public Button revive,close;
    bool decided;
    public void Configure(float value,Action accept,Action decline){
        progress.text="关卡已完成："+Mathf.FloorToInt(Mathf.Clamp01(value)*100)+"%";
        decided=false;revive.onClick.RemoveAllListeners();close.onClick.RemoveAllListeners();
        revive.onClick.AddListener(()=>Decide(accept));close.onClick.AddListener(()=>Decide(decline));
        StopAllCoroutines();StartCoroutine(Pop());
    }
    public void ShowAdStatus(string message){if(status)status.text=message;}
    public void AllowRetry(string message){decided=false;group.interactable=true;ShowAdStatus(message);}
    void Decide(Action action){if(decided||!group.interactable)return;decided=true;group.interactable=false;if(ReplicaFlow.Instance)ReplicaFlow.Instance.HomeClick();action?.Invoke();}
    IEnumerator Pop(){
        group.interactable=false;float elapsed=0;
        while(elapsed<.42f){elapsed+=Time.unscaledDeltaTime;float t=Mathf.Clamp01(elapsed/.42f),u=t-1;
            animatedContent.localScale=Vector3.one*Mathf.LerpUnclamped(.78f,1,1+2.70158f*u*u*u+1.70158f*u*u);
            group.alpha=Mathf.Clamp01(elapsed/.18f);shade.color=new Color(0,0,0,.64f*Mathf.Clamp01(elapsed/.22f));yield return null;}
        animatedContent.localScale=Vector3.one;group.alpha=1;group.interactable=true;
    }
}
