using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

// All UI, including inactive prefab children, uses the font selected at startup.
public static class Match3DDefaultFont {
    public static Font Current { get; private set; }
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern int GooseCanLoadSystemFont();
#endif
    public static void InitializeFallback() {
        Current=Resources.Load<Font>("Match3D/Fonts/DefaultChinese");
        if(!Current)throw new InvalidOperationException("Default Chinese fallback font is missing");
    }
    public static IEnumerator Initialize() {
        InitializeFallback();
#if UNITY_WEBGL && !UNITY_EDITOR
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
    public static void Apply(GameObject root) {
        if(!Current)return;
        foreach(var text in root.GetComponentsInChildren<Text>(true))text.font=Current;
    }
}
