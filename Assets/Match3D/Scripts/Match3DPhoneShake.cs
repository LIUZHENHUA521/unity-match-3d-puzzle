using UnityEngine;

// Added after WX.InitSDK has completed. The sensor is only active in gameplay.
public sealed class Match3DPhoneShake : MonoBehaviour {
    readonly PhoneShakeDetector detector=new PhoneShakeDetector();
    FoodPileShake shaker;
    bool listening,hidden,paused;
    int screenVersion=-1;
    void Awake(){
        shaker=FindObjectOfType<FoodPileShake>();
        if(!shaker)shaker=gameObject.AddComponent<FoodPileShake>();
    }
    void OnEnable(){
#if UNITY_WEBGL && !UNITY_EDITOR
        WeChatWASM.WX.OnAccelerometerChange(OnAcceleration);
        WeChatWASM.WX.OnHide(OnHide);
        WeChatWASM.WX.OnShow(OnShow);
#endif
    }
    bool CanListen=>!hidden&&!paused&&shaker&&shaker.CanShakeGameplay;
    void Update(){
        var flow=Match3DFlow.Instance;
        if(flow&&screenVersion!=flow.ScreenVersion){screenVersion=flow.ScreenVersion;detector.Reset();}
        SetListening(CanListen);
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if(listening)Feed(Input.acceleration);
#endif
    }
    void Feed(Vector3 acceleration){
        if(!listening||!CanListen){detector.Reset();return;}
        if(detector.Sample(acceleration,Time.unscaledTime))shaker.TryShake();
    }
    void SetListening(bool value){
        if(listening==value)return;
        listening=value;detector.Reset();
#if UNITY_WEBGL && !UNITY_EDITOR
        if(value)WeChatWASM.WX.StartAccelerometer(new WeChatWASM.StartAccelerometerOption{interval="game",fail=error=>Debug.LogWarning("Phone shake sensor unavailable: "+error.errMsg)});
        else WeChatWASM.WX.StopAccelerometer(new WeChatWASM.StopAccelerometerOption());
#endif
    }
#if UNITY_WEBGL && !UNITY_EDITOR
    void OnAcceleration(WeChatWASM.OnAccelerometerChangeListenerResult value){Feed(new Vector3((float)value.x,(float)value.y,(float)value.z));}
    void OnHide(WeChatWASM.GeneralCallbackResult value){hidden=true;SetListening(false);}
    void OnShow(WeChatWASM.OnShowListenerResult value){hidden=false;detector.Reset();}
#endif
    void OnApplicationPause(bool value){paused=value;if(value)SetListening(false);else detector.Reset();}
    void OnDisable(){
        SetListening(false);
#if UNITY_WEBGL && !UNITY_EDITOR
        WeChatWASM.WX.OffAccelerometerChange(OnAcceleration);
        WeChatWASM.WX.OffHide(OnHide);
        WeChatWASM.WX.OffShow(OnShow);
#endif
    }
}
