using UnityEngine;
using UnityEngine.UI;

// Authored PSD layout. Only safe-area containers and live quantities change at runtime.
public sealed class ReplicaGameplayHUD : MonoBehaviour {
    public RectTransform header,footer;
    public Text[] quantities;
    readonly Image[] toolBackgrounds=new Image[3];
    readonly Sprite[] normalBackgrounds=new Sprite[3];
    Sprite usedBackground;Texture2D usedTexture;
    void SetupToolBackgrounds(){
        var view=GetComponent<ReplicaUIView>();string[] keys={"Undo","Hint","Shuffle"};
        for(int i=0;i<3;i++){
            var button=view.Get<Button>(keys[i]);button.transition=Selectable.Transition.None;
            toolBackgrounds[i]=button.GetComponent<Image>();normalBackgrounds[i]=toolBackgrounds[i].sprite;
            toolBackgrounds[i].canvasRenderer.SetColor(Color.white);
        }
        var source=normalBackgrounds[0];if(!source)return;
        // Preserve the authored shape/alpha while desaturating only the background.
        var texture=source.texture;var rt=RenderTexture.GetTemporary(texture.width,texture.height,0,RenderTextureFormat.ARGB32);
        var previous=RenderTexture.active;
        try{
            Graphics.Blit(texture,rt);RenderTexture.active=rt;
            usedTexture=new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);
            usedTexture.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);usedTexture.Apply();
            var pixels=usedTexture.GetPixels32();
            for(int i=0;i<pixels.Length;i++){var c=pixels[i];byte grey=(byte)(c.r*.299f+c.g*.587f+c.b*.114f);pixels[i]=new Color32(grey,grey,grey,c.a);}
            usedTexture.SetPixels32(pixels);usedTexture.Apply();
            usedBackground=Sprite.Create(usedTexture,source.rect,new Vector2(source.pivot.x/source.rect.width,source.pivot.y/source.rect.height),source.pixelsPerUnit,0,SpriteMeshType.FullRect,source.border);
        }finally{RenderTexture.active=previous;RenderTexture.ReleaseTemporary(rt);}
    }
    void OnDestroy(){if(usedBackground)Destroy(usedBackground);if(usedTexture)Destroy(usedTexture);}
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
    void Awake(){SetupToolBackgrounds();headerPosition=header.anchoredPosition;footerPosition=footer.anchoredPosition;}
    void LateUpdate(){
        var size=((RectTransform)transform).rect.size;
        float units=size.x/Mathf.Max(1,Screen.width);
        if(windowSize!=new Vector2Int(Screen.width,Screen.height))RefreshPlatformSafeArea();
        header.anchoredPosition=headerPosition+Vector2.down*HeaderInset(size.x,Screen.width,Screen.height,Screen.safeArea,platformTopFraction);
        footer.anchoredPosition=footerPosition+Vector2.up*Screen.safeArea.yMin*units;
        bool limited=ReplicaSourceTools.Enabled;
        for(int i=0;i<quantities.Length;i++){
            quantities[i].transform.parent.gameObject.SetActive(limited);
            if(limited)quantities[i].text=i==1&&ReplicaTestPrivileges.UnlimitedMagnet?"∞":ReplicaSourceTools.Remaining(i).ToString();
            if(i<toolBackgrounds.Length&&toolBackgrounds[i])toolBackgrounds[i].sprite=limited&&!(i==1&&ReplicaTestPrivileges.UnlimitedMagnet)&&ReplicaSourceTools.UsedCount(i)>0&&usedBackground?usedBackground:normalBackgrounds[i];
        }
    }
}
