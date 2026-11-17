using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

// All UI, including inactive prefab children, uses the font selected at startup.
public static class Match3DDefaultFont {
    public static Font Current { get; private set; }
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern int GooseCanLoadSystemFont();
#endif
    public static void InitializeFallback() {
        Current=Resources.Load<Font>("Match3D/Fonts/DefaultChinese");
        if(!Current)throw new InvalidOperationException("Default Chinese fallback font is missing");
    }
    public static IEnumerator Initialize() {
        InitializeFallback();
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        // The bundled SDK cannot get the common font on iOS. Use the common-Chinese
        // packaged fallback there instead of requesting an empty fallback URL.
        if(GooseCanLoadSystemFont()!=0) {
            bool completed=false;
            try {
                WeChatWASM.WX.GetWXFont(string.Empty,font=>{
                    if(completed)return; // Ignore callbacks arriving after the timeout.
                    if(font)Current=font;
                    completed=true;
                });
            } catch(Exception error) {
                Debug.LogWarning("System font unavailable: "+error.Message);
                completed=true;
            }
            float deadline=Time.realtimeSinceStartup+8f;
            while(!completed && Time.realtimeSinceStartup<deadline)yield return null;
            if(!completed)Debug.LogWarning("System font timed out; using packaged Chinese fallback");
            completed=true;
        }
#endif
        Debug.Log("UI font: "+Current.name);
        yield break;
    }
    // Gameplay PSD: white lettering with a 3px outside #026DD1 stroke at ~26px glyph height.
    public static void ApplyPotLabelStyle(Text text) {
        text.color=Color.white;
        text.fontStyle=FontStyle.Bold;
        var outline=text.GetComponent<Outline>();
        if(!outline)outline=text.gameObject.AddComponent<Outline>();
        outline.effectColor=new Color32(2,109,209,255);
        float stroke=text.fontSize*3f/26f;
        outline.effectDistance=new Vector2(stroke,-stroke);
        outline.useGraphicAlpha=true;
    }
    public static void Apply(GameObject root) {
        if(!Current)return;
        foreach(var text in root.GetComponentsInChildren<Text>(true))text.font=Current;
    }
}
