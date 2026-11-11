using System;
using UnityEngine;
using UnityEngine.UI;

// One authored layout is shared by the Unity loader and the exported WeChat cover.
public sealed class Match3DStartupView : MonoBehaviour {
    public const string HealthAdvice="抵制不良游戏，拒绝盗版游戏。\n注意自我保护，谨防受骗上当。\n适度游戏益脑，沉迷游戏伤身。\n合理安排时间，享受健康生活。";
    public RectTransform content;
    public Text status;
    public Image progress;
    public Button retry;
    float shownProgress;
    public void SetProgress(float value,string message){
        shownProgress=Mathf.Max(shownProgress,Mathf.Clamp01(value));
        progress.fillAmount=shownProgress;
        progress.rectTransform.anchorMax=new Vector2(shownProgress,1);
        progress.transform.parent.gameObject.SetActive(true);status.text=message;retry.gameObject.SetActive(false);
    }
    public void ShowError(string message,Action onRetry){
        status.text=message;progress.transform.parent.gameObject.SetActive(false);retry.gameObject.SetActive(true);retry.onClick.RemoveAllListeners();
        retry.onClick.AddListener(()=>{retry.gameObject.SetActive(false);onRetry();});
    }
    public void Fit(){
        var root=(RectTransform)transform;
        var size=root.rect.size;
        if(size.x<=0||size.y<=0)return;
        // Keep title, the original age badge and every advice line inside the safe area.
        var safe=Screen.safeArea;
        bool valid=Application.isPlaying&&Screen.width>0&&Screen.height>0&&safe.width>0&&safe.height>0&&safe.xMax<=Screen.width&&safe.yMax<=Screen.height;
        Vector2 available=valid?new Vector2(size.x*safe.width/Screen.width,size.y*safe.height/Screen.height):size;
        content.localScale=Vector3.one*Mathf.Min(available.x/720f,available.y/1280f);
        content.anchoredPosition=valid?new Vector2((safe.center.x/Screen.width-.5f)*size.x,(safe.center.y/Screen.height-.5f)*size.y):Vector2.zero;
    }
    void LateUpdate(){Fit();}
}
