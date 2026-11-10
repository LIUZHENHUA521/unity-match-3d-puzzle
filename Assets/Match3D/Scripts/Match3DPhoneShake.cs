using UnityEngine;

// Added after WX.InitSDK has completed. The sensor is only active in gameplay.
public sealed class Match3DPhoneShake : MonoBehaviour {
    FoodPileShake shaker;
    bool listening,hidden,paused,ios;
    float lastSample;
    int screenVersion=-1;
    void Awake(){
        shaker=FindObjectOfType<FoodPileShake>();
        if(!shaker)shaker=gameObject.AddComponent<FoodPileShake>();
        ios=Application.platform==RuntimePlatform.IPhonePlayer;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        try{ios=string.Equals(WeChatWASM.WX.GetSystemInfoSync().platform,"ios",System.StringComparison.OrdinalIgnoreCase);}
        catch(System.Exception error){Debug.LogWarning("Phone shake platform detection failed: "+error.Message);}
#endif
    }
    void OnEnable(){
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        WeChatWASM.WX.OnAccelerometerChange(OnAcceleration);
        WeChatWASM.WX.OnHide(OnHide);
        WeChatWASM.WX.OnShow(OnShow);
#endif
    }
    bool CanListen=>!hidden&&!paused&&shaker&&shaker.CanShakeGameplay;
    void Update(){
        var flow=Match3DFlow.Instance;
        if(flow&&screenVersion!=flow.ScreenVersion){screenVersion=flow.ScreenVersion;lastSample=Time.unscaledTime;}
        SetListening(CanListen);
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if(listening)ProcessAcceleration(Input.acceleration);
#endif
    }
    public int ProcessAcceleration(Vector3 acceleration){
        float now=Time.unscaledTime,interval=now-lastSample;lastSample=now;
        if(!isActiveAndEnabled||!listening||!CanListen||!ItemPhysicsController.Instance)return 0;
        var force=PhoneShakeDetector.Force(acceleration,interval,Time.fixedDeltaTime,ios);
        return ItemPhysicsController.Instance.ApplyPhoneShake(force);
    }
    void SetListening(bool value){
        if(listening==value)return;
        listening=value;lastSample=Time.unscaledTime;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        if(value)WeChatWASM.WX.StartAccelerometer(new WeChatWASM.StartAccelerometerOption{interval="game",fail=error=>Debug.LogWarning("Phone shake sensor unavailable: "+error.errMsg)});
        else WeChatWASM.WX.StopAccelerometer(new WeChatWASM.StopAccelerometerOption());
#endif
    }
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    void OnAcceleration(WeChatWASM.OnAccelerometerChangeListenerResult value){ProcessAcceleration(new Vector3((float)value.x,(float)value.y,(float)value.z));}
    void OnHide(WeChatWASM.GeneralCallbackResult value){hidden=true;SetListening(false);}
    void OnShow(WeChatWASM.OnShowListenerResult value){hidden=false;lastSample=Time.unscaledTime;}
#endif
    void OnApplicationPause(bool value){paused=value;if(value)SetListening(false);else lastSample=Time.unscaledTime;}
    void OnDisable(){
        SetListening(false);
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        WeChatWASM.WX.OffAccelerometerChange(OnAcceleration);
        WeChatWASM.WX.OffHide(OnHide);
        WeChatWASM.WX.OffShow(OnShow);
#endif
    }
}
