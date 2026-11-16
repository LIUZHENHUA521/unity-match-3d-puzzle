using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class PotReplayBuild {
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [DllImport("__Internal")] static extern string PotReplayClientVersion();
#endif
    public static string Version {get {
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        try {var value=PotReplayClientVersion();if(!string.IsNullOrEmpty(value)&&value.Length<=80)return value;}catch(Exception){}
#endif
        return Application.version;
    }}
}
