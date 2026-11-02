using UnityEngine;
using UnityEngine.UI;

// Authored PSD layout. Only safe-area containers and live quantities change at runtime.
public sealed class ReplicaGameplayHUD : MonoBehaviour {
    public RectTransform header,footer;
    public Text[] quantities;
    Vector2 headerPosition,footerPosition;
    Vector2Int windowSize;
    float platformTopFraction;
    // SDK safe-area coordinates use logical pixels; normalize to the window
    // before converting to Unity pixels. Never add the two providers' insets.
    public static float HeaderInset(float canvasWidth,float width,float height,Rect safeArea,float platformTop=0){
        if(width<=0||height<=0)return 0;
        float unityTop=safeArea.width>0&&safeArea.height>0?Mathf.Clamp(height-safeArea.yMax,0,height):0;
        return Mathf.Max(unityTop,Mathf.Clamp01(platformTop)*height)*canvasWidth/width;
    }
    void RefreshPlatformSafeArea(){
        windowSize=new Vector2Int(Screen.width,Screen.height);platformTopFraction=0;
#if UNITY_WEBGL && !UNITY_EDITOR
        try{
            var info=WeChatWASM.WX.GetWindowInfo();
            if(info.windowHeight>0){
                float top=(float)info.statusBarHeight;
                if(!ReferenceEquals(info.safeArea,null))top=Mathf.Max(top,(float)info.safeArea.top);
                platformTopFraction=Mathf.Clamp01((top-(float)info.screenTop)/(float)info.windowHeight);
            }
        }catch(System.Exception){ /* Older hosts retain Unity's safe-area fallback. */ }
#endif
    }
    void Awake(){headerPosition=header.anchoredPosition;footerPosition=footer.anchoredPosition;}
    void LateUpdate(){
        var size=((RectTransform)transform).rect.size;
        float units=size.x/Mathf.Max(1,Screen.width);
        if(windowSize!=new Vector2Int(Screen.width,Screen.height))RefreshPlatformSafeArea();
        header.anchoredPosition=headerPosition+Vector2.down*HeaderInset(size.x,Screen.width,Screen.height,Screen.safeArea,platformTopFraction);
        footer.anchoredPosition=footerPosition+Vector2.up*Screen.safeArea.yMin*units;
        bool limited=ReplicaSourceTools.Enabled;
        for(int i=0;i<quantities.Length;i++){
            quantities[i].transform.parent.gameObject.SetActive(limited);
            if(limited)quantities[i].text=ReplicaSourceTools.Remaining(i).ToString();
        }
    }
}
